

-------------------------------------------Modify Existing function for multi ringcode filter--------------------------------------------------------------------

 DROP FUNCTION IF EXISTS public.fn_get_ring_details(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_ring_details(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_region_name character varying,
	p_segment_code character varying,
	p_ring_code character varying,
	p_site_id character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

 DECLARE
   sql TEXT; 
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords INTEGER; 
   LowerStart  character varying;
   LowerEnd  character varying;
   v_user_role_id integer;
   s_layer_columns_val text; 

BEGIN

sql:='';
LowerStart:='';
LowerEnd:='';


-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'id' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS  S_No,id,segment_code,ring_code,site_id,site_name,ring_capacity,agg1_site_id,agg2_site_id,region_name,description,bh_status,COALESCE(ring_a_site_distance, 0)as ring_a_site_distance,COALESCE(ring_b_site_distance,0)as ring_b_site_distance,Peer1,Peer2  from vw_topology_plan_details where 1=1 and peer1 is not null and peer2 is not null AND is_agg_site IS NOT TRUE ';

 IF(p_region_name != '')THEN 

	sql:= sql ||'and region_name ilike ''%'||(p_region_name)||'%'' ';
 END IF;

if(p_segment_code != '') then

sql:= sql ||'and segment_code ilike ''%'||(p_segment_code)||'%'' ';
end if;

if(p_ring_code != '') then

sql:= sql ||'and ring_code in (''' || replace(p_ring_code, ':', ''',''') || ''') ';
end if;
if(p_site_id != '') then

sql:= sql ||'and site_id in (''' || p_site_id || ''') ';
end if;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2% ',sql;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
/*IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;*/
	-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a ' INTO TotalRecords;
 
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords,*
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$BODY$;

ALTER FUNCTION public.fn_get_ring_details(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying, character varying)
    OWNER TO postgres;
-------------------------------------------Modify Existing function for Symetric View--------------------------------------------------------------------

 DROP FUNCTION IF EXISTS public.fn_get_ring_schematicview(integer);

CREATE OR REPLACE FUNCTION public.fn_get_ring_schematicview(
	p_ring_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
DECLARE 
v_Query text;
Query text; 
rec record;
v_from_id integer;
v_from_entity_type character varying;
nodes text;
edges text;
nodesDownstream text;
edgesDownstream text;
legends text;
cables text;
BEGIN

create temp table cpf_temp_result
(
id serial,
source_system_id integer,
source_network_id character varying,
source_port_no integer, 
source_entity_type character varying,
source_entity_title character varying,
destination_system_id integer,
destination_network_id character varying,
destination_port_no integer,
destination_entity_type character varying,
destination_entity_title character varying,
via_system_id integer,
via_network_id character varying,
via_entity_type character varying,
via_port_no integer
)on commit drop;

create temp table temp_nodes
(
id serial,
source_system_id integer,
source_network_id character varying,
connected_ports character varying, 
source_entity_type character varying,
source_entity_title character varying,
node_name 	character varying
)on commit drop;

create temp table temp_edges
(
id serial,
from_id integer,
from_entity_type character varying,
from_network_id character varying,
to_id integer,
to_network_id character varying,
to_entity_type character varying,
label character varying,
via_ports character varying,
network_status character varying,
cable_type character varying,
color_code character varying,
total_core integer,
cable_category character varying,
	cable_calculated_length double precision
)on commit drop;

insert into cpf_temp_result(via_system_id ,via_network_id ,via_entity_type)
SELECT distinct B.SYSTEM_ID,B.NETWORK_ID,'Cable' FROM top_ring_cable_mapping a
INNER JOIN ATT_DETAILS_CABLE b
ON A.CABLE_ID=B.SYSTEM_ID
where a.ring_id=p_ring_id  ;

update cpf_temp_result set source_system_id=a.system_id,
source_entity_type=a.entity_type,
source_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on --(st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2))
st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2))
and pm.entity_type in('SpliceClosure','FDB','POD','BDB')
where a.source_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and source_system_id is null;

update cpf_temp_result set destination_system_id=a.system_id,
destination_entity_type=a.entity_type,
destination_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id
from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on (st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2)))
and pm.entity_type in('SpliceClosure','FDB','POD','BDB')
where a.destination_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and destination_system_id is null;

--delete from cpf_temp_result where source_network_id=destination_network_id;


 
insert into temp_nodes(source_system_id,source_network_id,source_entity_type)
select source_system_id,source_network_id,source_entity_type  from(
select * from(
select source_system_id,source_network_id,source_entity_type 
	from cpf_temp_result 
union
select destination_system_id,destination_network_id,destination_entity_type 
	from cpf_temp_result)b
	
)t group by source_system_id,source_network_id,source_entity_type;

update temp_nodes set node_name=fms.fms_name
from att_details_fms fms where source_system_id=fms.system_id and source_entity_type='FMS';

update temp_nodes set node_name=fms.spliceclosure_name
from att_details_spliceclosure fms where source_system_id=fms.system_id and source_entity_type='SpliceClosure';

--UPDATE FDB Nikhil Arora
update temp_nodes set node_name=fdb.fdb_name
from isp_fdb_info fdb where source_system_id=fdb.system_id and source_entity_type='FDB';

--UPDATE POD Deep Shukla
update temp_nodes SET node_name = adp.site_id || ' (' || adp.site_name || ')'
from att_details_pod adp  where source_system_id=adp.system_id and source_entity_type='POD';


--END FDB UPDATE

insert into temp_edges(from_id,from_entity_type,
to_id,to_entity_type,label,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length)

	select source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,
network_status,cable_type,color_code,total_core,cable_category,
cable_calculated_length
from(
	select coalesce(source_system_id,cbl.a_system_id)as source_system_id, 
	coalesce(source_entity_type,a_entity_type) as source_entity_type,
coalesce(destination_system_id,b_system_id) as destination_system_id,
coalesce(destination_entity_type,b_entity_type) as destination_entity_type,via_network_id,
cbl.network_status,cbl.cable_type,cbl.total_core,cbl.cable_category,cbls.color_code,
Cast(cbl.cable_calculated_length as decimal(10,2))  as cable_calculated_length
from cpf_temp_result cpf
inner join att_details_cable cbl on cbl.system_id=cpf.via_system_id
left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type 
and cbls.cable_category=cbl.cable_category and cbls.fiber_count=cbl.total_core
)a
group by source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length;

   
   -- NODES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select * from(
select row_number() over(partition by id) rn,* from(
select distinct * from (
select CONCAT(source_system_id,source_entity_type) as id,
CONCAT('<b>',replace(node_name,E'\t',''),'</b>','\n',case when ld.is_virtual_port_allowed=false then concat('(',connected_ports,')') end) as label

,source_entity_type as group, null as title from temp_nodes t
	inner join layer_details ld on ld.layer_name=t.source_entity_type where source_entity_type!='Splitter'

) a)b)b where rn=1
) x) into nodesDownstream;

-- EDGES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select CONCAT(t.from_id,t.from_entity_type) as from,
CONCAT(t.to_id,t.to_entity_type) as to,'<b>'
||(case when t.total_core is not null then '('||t.total_core||'F)' else '' end)||
concat(' (',t.cable_calculated_length::text,' m)') as label,

case WHEN t.color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '{"color": "#FF0000"}'::json
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '{"color": "#0000FF"}'::json
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '{"color": "#FD10FD"}'::json
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '{"color": "#000"}'::json
ELSE '{"color": "#0000FF"}'::json
END
ELSE '{"color": "#000"}'::json
END as color,
0 as length,
CASE
WHEN upper(t.network_status::text) = 'P'::text THEN true
WHEN upper(t.network_status::text) = 'A'::text THEN false
ELSE true
END as dashes,
1.5  as width,
concat('{"type": "''curvedCCW''", "roundness":"0.5"}')::json as smooth
from temp_edges t
) x) into edgesDownstream;
   
   --LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (

select distinct * from (
select source_entity_type as entity_type, 
(select layer_title from layer_details where layer_name=source_entity_type) as entity_title,false as upstream 
from temp_nodes
) a where a.entity_type not in ('Cable','Splitter') order by entity_title
) x) into legends;

--CABLE LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
Select case WHEN color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '#FF0000'::text
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '#0000FF'::text
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '#FD10FD'::text
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '#000'::text
ELSE '#0000FF'::text
END
ELSE t.color_code::text
END as color_code,CONCAT(COALESCE(t.cable_category,''),
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN ' OH'
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN ' UG'
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN ' WC'
WHEN upper(t.cable_type::text) = 'ISP'::text THEN ' ISP'
ELSE ''
END ,
(case when total_core is not null then '('||t.total_core||'F)' else '' end))as text,false as upstream
from(
select color_code,cable_category,cable_type,total_core from temp_edges
group by color_code,cable_category,cable_type,total_core )t where t.cable_type is not null
) x) into cables;
   
	return query select row_to_json(result) from (
select '' as entityType, '' as entityTitle,'' as entityDisplayText,
0 as latitude,0 as longitude,
now(),null as nodes,null as edges,nodesDownstream,edgesDownstream,legends,cables
) result;
 

END 
$BODY$;

ALTER FUNCTION public.fn_get_ring_schematicview(integer)
    OWNER TO postgres;


------------------------------------- Modify function to get info, entities created from the application------------------------------------

 DROP FUNCTION IF EXISTS public.fn_getnearbyentities(double precision, double precision, integer, integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_getnearbyentities(
	lat double precision,
	lng double precision,
	mtrbuffer integer,
	p_user_id integer,
	p_source_ref_id character varying,
	p_source_ref_type character varying)
    RETURNS TABLE(geom_type character varying, entity_type character varying, entity_title character varying, system_id integer, common_name character varying, geom text, centroid_geom text, network_status character varying, display_name character varying, total_core character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

declare 
v_role_id integer;
v_geometry_with_buffer geometry;
v_geometry geometry;
BEGIN
	v_geometry = ST_GEOMFROMTEXT('POINT('||LNG||' '||LAT||')',4326);
	v_geometry_with_buffer = ST_BUFFER_METERS(v_geometry,MTRBUFFER);
		Raise info 'v_geometry ->%',v_geometry;
		Raise info 'v_geometry_with_buffer ->%',v_geometry_with_buffer;

	SELECT ROLE_ID INTO V_ROLE_ID FROM USER_MASTER WHERE USER_ID=P_USER_ID;

	CREATE TEMP TABLE TEMP_REGION_PROVINCE(
	REGION_ID INTEGER,
	PROVINCE_ID INTEGER
	) ON COMMIT DROP;

	CREATE TEMP TABLE TEMP_POINT_MASTER(
	SYSTEM_ID INTEGER,ENTITY_TYPE CHARACTER VARYING,LONGITUDE DOUBLE PRECISION,LATITUDE DOUBLE PRECISION,APPROVAL_FLAG CHARACTER VARYING,SP_GEOMETRY geometry,APPROVAL_DATE TIMESTAMP WITHOUT TIME ZONE,CREATOR_REMARK TEXT,APPROVER_REMARK TEXT,CREATED_BY INTEGER,APPROVER_ID INTEGER,COMMON_NAME CHARACTER VARYING,DB_FLAG INTEGER,MODIFIED_ON TIME WITHOUT TIME ZONE,NETWORK_STATUS CHARACTER VARYING,NO_OF_PORTS CHARACTER VARYING,IS_VIRTUAL BOOLEAN,ENTITY_CATEGORY CHARACTER VARYING,IS_BURIED BOOLEAN,DISPLAY_NAME CHARACTER VARYING,MODIFIED_BY INTEGER,IS_PROCESSED BOOLEAN,GIS_DESIGN_ID CHARACTER VARYING,ST_X DOUBLE PRECISION,ST_Y DOUBLE PRECISION
	)ON COMMIT DROP;
	
	--CREATE INDEX IF NOT EXISTS TEMP_POINT_MASTER_system_id_entity_type_idx ON public.TEMP_POINT_MASTER USING btree( system_id, upper(entity_type) COLLATE pg_catalog."default" ASC NULLS LAST) TABLESPACE pg_default;
	

	CREATE TEMP TABLE TEMP_LINE_MASTER(
	system_id integer,entity_type character varying,approval_flag character varying,sp_geometry geometry,creator_remark text,approver_remark text,created_by integer,approver_id integer,common_name character varying,db_flag integer,approval_date timestamp without time zone,modified_on timestamp without time zone,network_status character varying,is_virtual boolean,display_name character varying,modified_by integer,is_processed boolean,gis_design_id character varying
	)ON COMMIT DROP;

	CREATE TEMP TABLE TEMP_POLYGON_MASTER(
	SYSTEM_ID INTEGER,APPROVAL_DATE TIMESTAMP WITHOUT TIME ZONE,MODIFIED_ON TIMESTAMP WITHOUT TIME ZONE,IS_VIRTUAL BOOLEAN,CENTER_LINE_GEOM geometry,BUFFER_WIDTH DOUBLE PRECISION,MODIFIED_BY INTEGER,SP_CENTROID geometry,IS_PROCESSED BOOLEAN,SP_GEOMETRY geometry,CREATED_BY INTEGER,APPROVER_ID INTEGER,DB_FLAG INTEGER,ENTITY_TYPE CHARACTER VARYING,APPROVAL_FLAG CHARACTER VARYING,DISPLAY_NAME CHARACTER VARYING,CREATOR_REMARK TEXT,APPROVER_REMARK TEXT,GIS_DESIGN_ID CHARACTER VARYING,NETWORK_STATUS CHARACTER VARYING,COMMON_NAME CHARACTER VARYING
	) ON COMMIT DROP;
	
	Raise info 'Query Start to Insert Point Entity ->%', clock_timestamp();
	INSERT INTO TEMP_POINT_MASTER(SYSTEM_ID,ENTITY_TYPE,LONGITUDE,LATITUDE,APPROVAL_FLAG,SP_GEOMETRY,APPROVAL_DATE,CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,APPROVER_ID,COMMON_NAME,DB_FLAG,MODIFIED_ON,NETWORK_STATUS,NO_OF_PORTS,IS_VIRTUAL,ENTITY_CATEGORY,IS_BURIED,DISPLAY_NAME,
	MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID,ST_X,ST_Y)
	
	SELECT PTM.SYSTEM_ID,PTM.ENTITY_TYPE,PTM.LONGITUDE,PTM.LATITUDE,APPROVAL_FLAG,coalesce(draftinfo.updated_geom,PTM.SP_GEOMETRY),APPROVAL_DATE,
	CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,APPROVER_ID,PTM.COMMON_NAME,DB_FLAG,MODIFIED_ON,PTM.NETWORK_STATUS,
	NO_OF_PORTS,IS_VIRTUAL,ENTITY_CATEGORY,IS_BURIED,PTM.DISPLAY_NAME,MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID,ST_X,ST_Y
	FROM POINT_MASTER PTM 
	LEFT JOIN --lateral
	(
		select draftinfo.* 
		from att_details_edit_entity_info draftinfo  
		inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id 
		and opr.description ='EL' --and draftinfo.entity_system_id = PTM.system_id and upper(draftinfo.entity_type)=upper(PTM.entity_type)
	) draftinfo	on draftinfo.entity_system_id = PTM.system_id and upper(draftinfo.entity_type)=upper(PTM.entity_type)
	WHERE ST_WITHIN(coalesce(draftinfo.updated_geom,PTM.SP_GEOMETRY), v_geometry_with_buffer) ;
   --Commented out since entities created from the application are not visible in Info----------
    --and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(PTM.status,'A') = 'A' end;
	--Commented out since entities created from the application are not visible in Info----------
	
	
	
	/*SELECT PTM.SYSTEM_ID,PTM.ENTITY_TYPE,PTM.LONGITUDE,PTM.LATITUDE,APPROVAL_FLAG,PTM.SP_GEOMETRY,APPROVAL_DATE,
	CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,APPROVER_ID,PTM.COMMON_NAME,DB_FLAG,MODIFIED_ON,PTM.NETWORK_STATUS,
	NO_OF_PORTS,IS_VIRTUAL,ENTITY_CATEGORY,IS_BURIED,PTM.DISPLAY_NAME,MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID,ST_X,ST_Y
	FROM POINT_MASTER PTM
	WHERE ST_WITHIN(PTM.SP_GEOMETRY,ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||LNG||' '||LAT||')',4326),MTRBUFFER));
	*/
	Raise info 'Query End to Insert Point Entity ->%', clock_timestamp();
	Raise info 'Query Start to Insert Line Entity ->%', clock_timestamp();
	
	INSERT INTO TEMP_LINE_MASTER(SYSTEM_ID,ENTITY_TYPE,APPROVAL_FLAG,SP_GEOMETRY,CREATOR_REMARK,
	APPROVER_REMARK,CREATED_BY,APPROVER_ID,COMMON_NAME,DB_FLAG,APPROVAL_DATE,MODIFIED_ON,NETWORK_STATUS,IS_VIRTUAL,DISPLAY_NAME,MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID)

	SELECT PTM.SYSTEM_ID,PTM.ENTITY_TYPE,PTM.APPROVAL_FLAG,coalesce(draftinfo.updated_geom,PTM.sp_geometry),CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,APPROVER_ID,
	PTM.COMMON_NAME,DB_FLAG,APPROVAL_DATE,MODIFIED_ON,PTM.NETWORK_STATUS,IS_VIRTUAL,PTM.DISPLAY_NAME,MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID 
	FROM LINE_MASTER PTM
	left join 
	(
		select draftinfo.* from att_details_edit_entity_info draftinfo  
		inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL'
	) draftinfo
	on draftinfo.entity_system_id=PTM.system_id and upper(draftinfo.entity_type)=upper(PTM.entity_type)
	WHERE ST_Intersects(st_makevalid(coalesce(draftinfo.updated_geom,PTM.sp_geometry)),v_geometry_with_buffer) 
    
    and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(PTM.status,'A') = 'A' end;

	Raise info 'Query end to Insert Line Entity ->%', clock_timestamp();

	Raise info 'Query Start to Insert Polygon Entity ->%', clock_timestamp();
	INSERT INTO TEMP_POLYGON_MASTER(	
	SYSTEM_ID,APPROVAL_DATE,MODIFIED_ON,IS_VIRTUAL,CENTER_LINE_GEOM,BUFFER_WIDTH,
	MODIFIED_BY,SP_CENTROID,IS_PROCESSED,SP_GEOMETRY,CREATED_BY,APPROVER_ID,
	DB_FLAG,ENTITY_TYPE,APPROVAL_FLAG,DISPLAY_NAME,CREATOR_REMARK,APPROVER_REMARK,
	GIS_DESIGN_ID,NETWORK_STATUS,COMMON_NAME)
	SELECT POLY.SYSTEM_ID,APPROVAL_DATE,MODIFIED_ON,IS_VIRTUAL,CENTER_LINE_GEOM,
	BUFFER_WIDTH,MODIFIED_BY,SP_CENTROID,IS_PROCESSED,SP_GEOMETRY,CREATED_BY,APPROVER_ID,
	DB_FLAG,POLY.ENTITY_TYPE,APPROVAL_FLAG,POLY.DISPLAY_NAME,CREATOR_REMARK,APPROVER_REMARK,
	GIS_DESIGN_ID,POLY.NETWORK_STATUS,POLY.COMMON_NAME 
	FROM POLYGON_MASTER POLY
	WHERE ST_INTERSECTS(v_geometry_with_buffer,POLY.SP_GEOMETRY) 
    and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(POLY.status,'A') = 'A' end;
    
	Raise info 'Query end to Insert Polygon Entity ->%', clock_timestamp();
	INSERT INTO TEMP_REGION_PROVINCE(REGION_ID,PROVINCE_ID)
	SELECT REGION_ID,ID FROM PROVINCE_BOUNDARY PROVINCE WHERE ST_WITHIN(v_geometry,PROVINCE.SP_GEOMETRY);

	  Raise info 'Query Start to return query ->%', clock_timestamp();
   RETURN QUERY
   select  tbl.geom_type::character varying as geom_type, tbl.entity_type,l.layer_title, tbl.system_id, tbl.common_name,tbl.geom,tbl.centroid_geom,
   case when l.is_network_entity  = true then tbl.network_status else '' end network_status,tbl.display_name::character varying,tbl.total_core from (
select *, row_number() over(partition by tbl1.system_id,tbl1.entity_type order by order_colum asc) as row_num 
       from (
    
	select 'Point' as geom_type, p.entity_type, p.system_id, p.common_name,
    ST_astext(p.sp_geometry) as geom,
	ST_astext(p.sp_geometry) as centroid_geom,
	coalesce(draftinfo1.ATTRIBUTE_INFO->>lower('curr_status'),p.network_status)::character varying as network_status,
	2 as order_colum,coalesce(draftinfo2.display_name,p.display_name) as display_name,
	coalesce(draftinfo2.ATTRIBUTE_INFO->>lower('no_of_core_per_tube'),p.no_of_ports)::character varying as total_core
	from TEMP_POINT_MASTER p
	inner join vw_user_permission_area upa on user_id=p_user_id
	inner join temp_region_province province on upa.region_id=province.region_id and upa.province_id=province.province_id 
	--inner join province_boundary province on ST_Within(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),province.sp_geometry) 	
	--left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	--inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL' 
	--and (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	--) draftinfo
	--on draftinfo.entity_system_id=p.system_id and upper(draftinfo.entity_type)=upper(p.entity_type)
	left join (select draftinfo1.* from att_details_edit_entity_info draftinfo1  
	inner join entity_operations_master opr1 on draftinfo1.entity_action_id=opr1.id and upper(opr1.description)='NS' 
	--and (draftinfo1.updated_by=p_user_id or (draftinfo1.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo1
	on draftinfo1.entity_system_id=p.system_id and upper(draftinfo1.entity_type)=upper(p.entity_type)
	left join (select draftinfo2.* from att_details_edit_entity_info draftinfo2  
	inner join entity_operations_master opr on draftinfo2.entity_action_id=opr.id and upper(opr.description)='ED') draftinfo2
	on draftinfo2.entity_system_id=p.system_id and upper(draftinfo2.entity_type)=upper(p.entity_type)
	where p.approval_flag <> 'U' and ST_WithIn(p.sp_geometry, ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer)) 
	--and lower(p.entity_type) != 'loop'
	union
	select 'Line' as geom_type, l.entity_type, l.system_id, l.common_name,ST_astext(l.sp_geometry) as geom,
	ST_astext(ST_Centroid(l.sp_geometry)) as centroid_geom,coalesce(draftinfo1.ATTRIBUTE_INFO->>lower('curr_status'),
	l.network_status)::character varying as network_status ,3 as order_colum,coalesce(draftinfo2.display_name,l.display_name) as display_name,
	coalesce(draftinfo2.ATTRIBUTE_INFO->>lower('total_core')||'F',cbl.total_core||'F')::character varying as total_core
        from temp_LINE_MASTER l 
        left join att_details_cable cbl on case when upper(l.entity_type)='CABLE' then l.system_id=cbl.system_id else 1=2 end
	inner join vw_user_permission_area upa on user_id=p_user_id
	inner join temp_region_province province on upa.region_id=province.region_id and upa.province_id=province.province_id 
	--left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	--inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL'  
	--and (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))

	--) draftinfo
	--on draftinfo.entity_system_id=l.system_id and upper(draftinfo.entity_type)=upper(l.entity_type)
	left join (select draftinfo1.* from att_details_edit_entity_info draftinfo1  
	inner join entity_operations_master opr1 on draftinfo1.entity_action_id=opr1.id and upper(opr1.description)='NS' 
	--and (draftinfo1.updated_by=p_user_id or (draftinfo1.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo1
	on draftinfo1.entity_system_id=l.system_id and upper(draftinfo1.entity_type)=upper(l.entity_type)
	left join (select draftinfo2.* from att_details_edit_entity_info draftinfo2  
	inner join entity_operations_master opr on draftinfo2.entity_action_id=opr.id and upper(opr.description)='ED') draftinfo2
	on draftinfo2.entity_system_id=l.system_id and upper(draftinfo2.entity_type)=upper(l.entity_type)
        where l.approval_flag <> 'U' and ST_Intersects(st_makevalid(l.sp_geometry), 
        ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer))
	union 
	select 'Polygon' as geom_type, poly.entity_type, poly.system_id, poly.common_name,ST_astext(coalesce(draftinfo.updated_geom,poly.sp_geometry)) as geom,
	ST_astext(ST_Centroid(coalesce(draftinfo.updated_geom,poly.sp_geometry))) as centroid_geom,
	coalesce(draftinfo1.ATTRIBUTE_INFO->>lower('curr_status'),poly.network_status)::character varying as network_status,1 as order_colum,
	coalesce(draftinfo2.display_name,poly.display_name) as display_name,null as total_core   from 
	TEMP_POLYGON_MASTER poly
	inner join vw_user_permission_area upa on user_id=p_user_id
	inner join temp_region_province province on upa.region_id=province.region_id and upa.province_id=province.province_id 
	left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL' 
	--and  (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo
	on draftinfo.entity_system_id=poly.system_id and upper(draftinfo.entity_type)=upper(poly.entity_type)
	left join (select draftinfo1.* from att_details_edit_entity_info draftinfo1  
	inner join entity_operations_master opr1 on draftinfo1.entity_action_id=opr1.id and upper(opr1.description)='NS' 
	--and (draftinfo1.updated_by=p_user_id or (draftinfo1.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo1
	on draftinfo.entity_system_id=poly.system_id and upper(draftinfo.entity_type)=upper(poly.entity_type)
	left join (select draftinfo2.* from att_details_edit_entity_info draftinfo2  
	inner join entity_operations_master opr on draftinfo2.entity_action_id=opr.id and upper(opr.description)='ED') draftinfo2
	on draftinfo2.entity_system_id=poly.system_id and upper(draftinfo2.entity_type)=upper(poly.entity_type)
	where poly.approval_flag <> 'U' and 
	ST_Intersects(ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer),coalesce(draftinfo.updated_geom,poly.sp_geometry)) 
	and case when upper(poly.entity_type)=upper('building') and poly.is_virtual=true then 1=2 else 1=1 end
	union 
	select 'Polygon' as geom_type,'Province' as entity_type, province.id as system_id, province.province_name||'('||province_abbreviation||')' as common_name,ST_astext(province.sp_geometry) as geom,
	ST_astext(ST_Centroid(province.sp_geometry)) as centroid_geom,
	'A' as network_status,1 as order_colum,province.province_name||'('||province_abbreviation||')' as display_name, null as total_core   
	from province_boundary province
	inner join temp_region_province tmpprovince on province.region_id=tmpprovince.region_id and province.id=tmpprovince.province_id
	inner join vw_user_permission_area upa on user_id=p_user_id and upa.region_id=tmpprovince.region_id and upa.province_id=tmpprovince.province_id
	 
	--left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	--inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL'  
	--and (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	--) draftinforo
	
	--on draftinfo.entity_system_id=province.id and upper(draftinfo.entity_type)=upper('Province')
	--inner join vw_user_permission_area upa on ST_Within(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),coalesce(draftinfo.updated_geom,province.sp_geometry)) and upa.region_id=province.region_id and upa.province_id=province.id and user_id=p_user_id	 
	union
	select 'Circle' as geom_type, poly.entity_type, poly.system_id, poly.common_name,ST_astext(coalesce(draftinfo.updated_geom,poly.sp_geometry)) as geom,
	ST_astext(ST_Centroid(coalesce(draftinfo.updated_geom,poly.sp_geometry))) as centroid_geom,
	coalesce(draftinfo1.ATTRIBUTE_INFO->>lower('curr_status'),poly.network_status)::character varying as network_status,1 as order_colum,poly.display_name||'-('||coalesce(draftinfo1.ATTRIBUTE_INFO->>'CURR_STATUS',poly.network_status)||')' as display_name, null as total_core   from circle_master poly 
	inner join vw_user_permission_area upa on user_id=p_user_id
	inner join temp_region_province province on upa.region_id=province.region_id and upa.province_id=province.province_id  
	left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL' 
	--and  (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo
	on draftinfo.entity_system_id=poly.system_id and upper(draftinfo.entity_type)=upper(poly.entity_type)
	left join (select draftinfo1.* from att_details_edit_entity_info draftinfo1  
	inner join entity_operations_master opr1 on draftinfo1.entity_action_id=opr1.id and upper(opr1.description)='NS' 
	--and (draftinfo1.updated_by=p_user_id or (draftinfo1.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo1
	on draftinfo1.entity_system_id=poly.system_id and upper(draftinfo1.entity_type)=upper(poly.entity_type)
	where poly.approval_flag <> 'U' and 
	ST_Intersects(coalesce(draftinfo.updated_geom,poly.sp_geometry),ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer)) 
	) tbl1
) tbl join layer_details l on upper(tbl.entity_type)=upper(l.layer_name)

 inner  join role_permission_entity rp on l.layer_id  = rp.layer_id and rp.role_id = v_role_id and rp.network_status = tbl.network_status and rp.viewonly = true
 and l.is_info_enabled=true and case when upper(l.layer_name)='PIT'  then 1=1 else l.isvisible=true end
 where row_num=1 order by tbl.geom_type asc
 , tbl.entity_type desc,tbl.display_name asc;
 --tbl.entity_type;
 Raise info 'Query end to return query ->%', clock_timestamp();
    
END
$BODY$;

ALTER FUNCTION public.fn_getnearbyentities(double precision, double precision, integer, integer, character varying, character varying)
    OWNER TO postgres;

