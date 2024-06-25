
CREATE OR REPLACE FUNCTION public.fn_save_entity_geom(
	p_system_id integer,
	p_geom_type character varying,
	p_entity_type character varying,
	p_userid integer,
	p_longlat character varying,
	p_common_name character varying,
	p_network_status character varying,
	p_ports character varying,
	p_entity_category character varying,
	p_center_line_geom character varying,
	p_buffer_width double precision,
	p_project_id integer)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE geo_data text;
s_longLat character varying;
sql text;
s_long text;
s_lat text;
v_radious double precision;
v_close_point character varying;
v_centroid geometry;
v_center_line_geom character varying;
v_display_name character varying;
v_display_value character varying;
v_layer_table character varying;
v_source_ref_id character varying;
v_source_ref_type character varying;
v_status character varying;

BEGIN

v_radious:=0;

select layer_table into v_layer_table from layer_details where Upper(layer_name) = upper(p_entity_type) ;
select source_ref_id,source_ref_type,status into v_source_ref_id,v_source_ref_type,v_status from v_layer_table where system_id= p_system_id ;

if lower(p_geom_type)='point' then
s_longLat:='POINT('||p_longlat||')';
geo_data := 'ST_GeomFromText('''||s_longLat||''',4326)';
s_long:= SPLIT_PART(''||p_longlat||'', ' ',1);
s_lat:= SPLIT_PART(''||p_longlat||'', ' ',2);

sql:='insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,modified_on,network_status,no_of_ports,entity_category,display_name,project_id,status,source_ref_id,source_ref_type)
values('||p_system_id||','''||p_entity_type||''','''||s_long||''','''||s_lat||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),'''||p_network_status||''','''||p_ports||''','''||p_entity_category||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';

end if;

if lower(p_geom_type)='line' then
s_longLat := 'LINESTRING('||p_longlat||')';
geo_data := 'ST_GeomFromText('''|| s_longLat||''',4326)';
sql:='INSERT INTO line_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,display_name,project_id,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';
end if;
if lower(p_geom_type)='polygon' then
s_longLat := 'POLYGON(('||p_longlat||'))';
geo_data := 'ST_GeomFromText(''' ||s_longLat ||''',4326)';
if(upper(p_entity_type)=upper('ROW'))
then
sql:='INSERT INTO polygon_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,center_line_geom,buffer_width,display_name,project_id,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',ST_GeomFromText(''LINESTRING('||p_center_line_geom||')'',4326),'||p_buffer_width||',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''') ';
else

sql:='INSERT INTO polygon_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,display_name,project_id,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''') ';
end if;

end if;

if lower(p_geom_type)='circle' then
s_longLat := 'POLYGON(('||p_longlat||'))';
geo_data := 'ST_GeomFromText(''' ||s_longLat ||''',4326)';

select Cast(st_distance(ST_centroid('POLYGON(('||p_longlat||'))'),(ST_DumpPoints('POLYGON(('||p_longlat||'))')).geom,false)as decimal(10,2)) into v_radious limit 1;

select center_line_geom into v_center_line_geom
from polygon_master pm inner join att_details_row_pit pit on pm.system_id=pit.parent_system_id and upper(pm.entity_type)=upper('ROW') where pit.system_id=p_system_id;

v_close_point:='ST_ClosestPoint('''||v_center_line_geom||''', ST_centroid('||geo_data||')) ';

geo_data:='ST_buffer_meters('||v_close_point||','||coalesce(v_radious,1)||')';
sql:='INSERT INTO circle_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,sp_center,radious,display_name,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''','||v_close_point||','||coalesce(v_radious,0)||',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';

end if;
 -- raise info 'SQL %',SQL;
IF UPPER(P_GEOM_TYPE)='POINT' AND NOT EXISTS(SELECT 1 FROM POINT_MASTER PM
INNER JOIN LAYER_DETAILS LD ON UPPER(PM.COMMON_NAME)=UPPER(P_COMMON_NAME) AND UPPER(PM.ENTITY_TYPE)=UPPER(LD.LAYER_NAME) 
AND IS_MiDDLEWARE_ENTITY=TRUE)
THEN
EXECUTE SQL;
ELSIF UPPER(P_GEOM_TYPE)!='POINT'
THEN
EXECUTE SQL;
END IF;
END
$BODY$;













-- FUNCTION: public.fn_getnearbyentities(double precision, double precision, integer, integer)
-- DROP FUNCTION IF EXISTS public.fn_getnearbyentities(double precision, double precision, integer, integer);
--select * from fn_getnearbyentities(26.155637,85.890555,15,5,'','')
CREATE OR REPLACE FUNCTION public.fn_getnearbyentities(
	lat double precision,
	lng double precision,
	mtrbuffer integer,
	p_user_id integer,
    p_source_ref_id character varying,
    p_source_ref_type character varying )
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
	WHERE ST_WITHIN(coalesce(draftinfo.updated_geom,PTM.SP_GEOMETRY), v_geometry_with_buffer) and case when coalesce(p_source_ref_id,'')!='' Then PTM.source_ref_id = PTM.source_ref_id  else  PTM.source_ref_id = p_source_ref_id end;
	
	
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
	WHERE ST_Intersects(st_makevalid(coalesce(draftinfo.updated_geom,PTM.sp_geometry)),v_geometry_with_buffer) and case when coalesce(p_source_ref_id,'')!='' Then PTM.source_ref_id = PTM.source_ref_id  else  PTM.source_ref_id = p_source_ref_id end;

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
	WHERE ST_INTERSECTS(v_geometry_with_buffer,POLY.SP_GEOMETRY)  and case when coalesce(p_source_ref_id,'')!='' Then POLY.source_ref_id = POLY.source_ref_id  else  POLY.source_ref_id = p_source_ref_id end;
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

    alter table point_master add column status character varying(10);
    alter table point_master add column source_ref_id character varying(100);
    alter table point_master add column source_ref_type character varying(100);
	
    alter table line_master add column status character varying(10);
    alter table line_master add column source_ref_id character varying(100);
    alter table line_master add column source_ref_type character varying(100);  
	
    alter table polygon_master add column status character varying(10);
    alter table polygon_master add column source_ref_id character varying(100);
    alter table polygon_master add column source_ref_type character varying(100);   
	
    alter table circle_master add column status character varying(10);
    alter table circle_master add column source_ref_id character varying(100);
    alter table circle_master add column source_ref_type character varying(100);
	
	
--	select 'update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status from  ' || layer_table|| 
--   ' ld  where ld.system_id= pm.system_id and entity_type = ld.'||layer_name   from layer_details where geom_type='Point' and isvisible = true;

update point_master pm  set  status = ld.status from  att_details_other ld  where ld.system_id= pm.system_id and ld.entity_type ='Other';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_loop ld  where ld.system_id= pm.system_id and entity_type ='Loop';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_bld_structure ld  where ld.system_id= pm.system_id and entity_type ='Structure';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_rack ld  where ld.system_id= pm.system_id and entity_type ='Rack';
--update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_vsat_hub ld  where ld.system_id= pm.system_id and entity_type ='VSAT';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_Antenna ld  where ld.system_id= pm.system_id and entity_type ='Antenna';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_patchpanel ld  where ld.system_id= pm.system_id and entity_type ='PatchPanel';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_handhole ld  where ld.system_id= pm.system_id and entity_type ='Handhole';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_cabinet ld  where ld.system_id= pm.system_id and entity_type = 'Cabinet';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_tower ld  where ld.system_id= pm.system_id and entity_type = 'Tower';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_building ld  where ld.system_id= pm.system_id and entity_type ='Building';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_Slack ld  where ld.system_id= pm.system_id and entity_type ='Slack';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_model ld  where ld.system_id= pm.system_id and entity_type ='Equipment';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_fault ld  where ld.system_id= pm.system_id and entity_type ='Fault';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_cdb ld  where ld.system_id= pm.system_id and entity_type ='CDB';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_adb ld  where ld.system_id= pm.system_id and entity_type ='ADB';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_splitter ld  where ld.system_id= pm.system_id and entity_type ='Splitter';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  isp_htb_info ld  where ld.system_id= pm.system_id and entity_type ='HTB';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_spliceclosure ld  where ld.system_id= pm.system_id and entity_type ='SpliceClosure';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_mpod ld  where ld.system_id= pm.system_id and entity_type ='MPOD';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_fms ld  where ld.system_id= pm.system_id and entity_type ='FMS';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  isp_fdb_info ld  where ld.system_id= pm.system_id and entity_type ='FDB';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_tree ld  where ld.system_id= pm.system_id and entity_type ='Tree';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_pod ld  where ld.system_id= pm.system_id and entity_type ='POD';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_ont ld  where ld.system_id= pm.system_id and entity_type ='ONT';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_wallmount ld  where ld.system_id= pm.system_id and entity_type ='WallMount';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_bdb ld  where ld.system_id= pm.system_id and entity_type ='BDB';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_manhole ld  where ld.system_id= pm.system_id and entity_type ='Manhole';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_pole ld  where ld.system_id= pm.system_id and entity_type ='Pole';
update point_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_customer ld  where ld.system_id= pm.system_id and entity_type ='Customer';

update line_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_duct ld  where ld.system_id= pm.system_id and entity_type ='Duct';
update line_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_trench ld  where ld.system_id= pm.system_id and entity_type = 'Trench';
update line_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_microduct ld  where ld.system_id= pm.system_id and entity_type ='Microduct';
update line_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_cable ld  where ld.system_id= pm.system_id and entity_type ='Cable';


update polygon_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_area ld  where ld.system_id= pm.system_id and entity_type ='Area';
update polygon_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_sector ld  where ld.system_id= pm.system_id and entity_type ='Sector';
update polygon_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_csa ld  where ld.system_id= pm.system_id and entity_type ='CSA';
update polygon_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_subarea ld  where ld.system_id= pm.system_id and entity_type ='SubArea';
update polygon_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_ROW ld  where ld.system_id= pm.system_id and entity_type ='ROW';
update polygon_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_dsa ld  where ld.system_id= pm.system_id and entity_type ='DSA';
update polygon_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_surveyarea ld  where ld.system_id= pm.system_id and entity_type ='SurveyArea';
update polygon_master pm  set source_ref_id= ld.source_ref_id, status = ld.status, source_ref_type = ld.source_ref_type from  att_details_restricted_area ld  where ld.system_id= pm.system_id and entity_type ='Restricted_Area';






CREATE OR REPLACE FUNCTION public.fn_uploader_insert_cable(
	p_upload_id integer,
	p_batch_id integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$


declare
result int:=0;
cnt int:=0;
texttoappend character varying;
REC RECORD;
current_system_id integer;
v_parent_system_id integer;
v_parent_network_id character varying;
v_parent_entity_type character varying;
v_network_code character varying;
v_sequence_id integer;
v_geom character varying;
v_entity_name character varying;
v_network_Id_type character varying;
v_network_code_seperator character varying;
v_a_display_value character varying;
v_b_display_value character varying;
v_auto_character_count integer;
v_network_name character varying;
v_a_system_id integer;
v_a_entity_type character varying;
v_b_system_id integer;
v_b_entity_type character varying;
v_structure_id integer;
v_a_network_id character varying;
v_b_network_id character varying;
v_cable_a_primary_table character varying;
v_cable_a_secondary_table character varying;
v_cable_b_primary_table character varying;
v_cable_b_secondary_table character varying;
v_a_connectivity character varying;
v_b_connectivity character varying;
sql text;
BEGIN
select entity_type into v_entity_name from upload_summary where id=p_upload_id;
select network_id_type,network_code_seperator into v_network_Id_type,v_network_code_seperator from layer_details where upper(layer_name)=upper(v_entity_name);

FOR REC IN select * from temp_du_cable t
--inner join kml_attributes k on upper(k.cable_name)=upper(t.cable_name) and t.upload_id=k.uploaded_id
where upload_id=p_upload_id and is_valid=true and batch_id=p_batch_id
LOOP
BEGIN
-- v_geom:=replace(replace(rec.sp_geometry,'LINESTRING(',''),')','');
-- v_geom:=replace(replace(rec.sp_geometry,'LINESTRING (',''),')','');

v_geom:=replace(replace(upper(rec.sp_geometry),'LINESTRING(',''),')','');
v_geom:=replace(v_geom,'LINESTRING (','');
v_geom:=replace(v_geom,'LINESTRINGZ (','');
v_geom:=replace(v_geom,'LINESTRINGZ(','');

--v_geom:=replace(v_geom,'LINESTRING (','');
v_network_name:=rec.cable_name;
v_sequence_id:=0;

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then

v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Cable') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
-- select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
-- from fn_get_clone_network_code('Cable', 'Line',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
-- v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
raise info 'REC.network_id%',v_geom;
raise info 'rec.parent_entity_type%',rec.parent_entity_type;
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Cable', 'Line',''||v_geom||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;

end if;

IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;

RAISE INFO 'v_parent_system_id : %',v_parent_system_id;
RAISE INFO 'v_parent_network_id : %',v_parent_network_id;
RAISE INFO 'v_parent_entity_type : %',v_parent_entity_type;
RAISE INFO 'v_network_code : %',v_network_code;
RAISE INFO 'v_sequence_id : %',v_sequence_id;

If(upper(coalesce(v_parent_network_id, ''))!= 'NLD')
then
v_parent_network_id=rec.parent_network_id;
END IF;
--INSERT INTO MAIN TABLE

raise info 'Result of rec :%',rec.origin_ref_id;
if(rec.origin_ref_id is not null and rec.origin_from ='SP') then 
if(rec.cable_type ='ISP') then
Select  adb2.system_id as a_system_id,adb2.network_id, 'BDB' as a_entity_type ,
        ifi2.system_id as b_system_id ,ifi2.network_id ,'FDB' as b_entity_type,ifi2.parent_system_id 
        into v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type,v_structure_id
        from 
        (select * from temp_du_cable where system_id =rec.system_id) cable 
inner join att_details_splitter adb   
on cast(cable.a_end as character varying) = adb.origin_Ref_id 
inner join att_details_splitter ifi 
on cast(cable.b_end as character varying) = ifi.origin_ref_id
inner join att_details_bdb adb2 
on adb2.system_id = adb.parent_system_id 
inner join isp_fdb_info ifi2  
on ifi2.system_id = ifi.parent_system_id order by 1 desc,4 desc  limit 1;
else
raise info 'Processed';
 if(rec.a_entity_type ='BDB') then 
 
 v_cable_a_primary_table :='att_details_splitter';
 v_cable_a_secondary_table := 'att_details_bdb';
v_a_connectivity :='on a_secondary.system_id = a_primary.parent_system_id' ;
elseif (rec.a_entity_type='FMS') then
 
 v_cable_a_primary_table :='att_details_pod';
 v_cable_a_secondary_table := 'att_details_fms';
v_a_connectivity :='on a_secondary.parent_system_id = a_primary.system_id' ;
end if;
if ((rec.b_entity_type ='BDB')) then 

v_cable_b_primary_table :='att_details_splitter';
 v_cable_b_secondary_table := 'att_details_bdb';
v_b_connectivity :='on b_secondary.system_id = b_primary.parent_system_id' ;
elseif ((rec.b_entity_type ='FMS')) then 
v_cable_b_primary_table :='att_details_pod';
 v_cable_b_secondary_table := 'att_details_fms';
v_b_connectivity :='on b_secondary.parent_system_id = b_primary.system_id' ;
end if;
raise info 'Processed :% ,:%, :% ,:%',v_cable_a_primary_table,v_cable_b_primary_table,v_cable_a_secondary_table,v_cable_b_secondary_table;
sql := 'Select  a_secondary.system_id as a_system_id,a_secondary.network_id, cable.a_entity_type ,
        b_secondary.system_id as b_system_id ,b_secondary.network_id ,cable.b_entity_type
        from 
        (select * from temp_du_cable where system_id ='||rec.system_id ||') cable 
inner join '|| v_cable_a_primary_table ||' a_primary   
on cast(cable.a_end as character varying) = a_primary.origin_Ref_id 
inner join '|| v_cable_b_primary_table ||' b_primary 
on cast(cable.b_end as character varying) = b_primary.origin_ref_id
inner join '|| v_cable_a_secondary_table || ' a_secondary ' ||
v_a_connectivity ||'
inner join '|| v_cable_b_secondary_table ||' b_secondary  
' ||
v_b_connectivity ||' order by 1 desc,4 desc  limit 1;';
 raise info 'Sql:% ',sql;
 execute sql  into v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type;
v_structure_id :=0;
end if;
raise info 'Values :%,:%,:%,:%,%,%',v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type;
INSERT INTO public.att_details_cable(network_id, cable_name, a_location, b_location, total_core,
no_of_tube, no_of_core_per_tube,
cable_measured_length, cable_calculated_length,
cable_type, specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id, network_status,
status,province_id,
region_id,created_by,created_on, a_system_id,a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type,
cable_category,cable_sub_category,route_id, parent_system_id, parent_network_id, parent_entity_type,sequence_id,
type,brand,model,construction,activation,accessibility,duct_id,utilization,ownership_type, source_ref_type,
source_ref_id, remarks,audit_item_master_id,origin_from,origin_Ref_id,origin_Ref_code,
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,structure_id,
section_name, generic_section_name,hierarchy_type,aerial_loaction)


SELECT v_network_code,v_network_name, v_a_network_id, v_b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),v_a_system_id,
v_a_network_id,
v_a_entity_type,v_b_system_id,v_b_network_id,v_b_entity_type,cable_category,sub_category,
route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',v_structure_id,
section_name, generic_section_name,hierarchy_type,aerial_loaction
from temp_du_cable where system_id=rec.system_id
returning system_id into current_system_id;
else
INSERT INTO public.att_details_cable(network_id, cable_name, a_location, b_location, total_core,no_of_tube, no_of_core_per_tube,
cable_measured_length, cable_calculated_length,
cable_type, specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id, network_status, status,province_id,
region_id,created_by,created_on, a_system_id,a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type,
cable_category,cable_sub_category,route_id, parent_system_id, parent_network_id, parent_entity_type,sequence_id,
type,brand,model,construction,activation,accessibility,duct_id,utilization,ownership_type, source_ref_type,
source_ref_id, remarks,audit_item_master_id,origin_from,origin_Ref_id,origin_Ref_code,
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,section_name, generic_section_name,hierarchy_type,aerial_loaction)
SELECT v_network_code,v_network_name, a_network_id, b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),a_system_id,a_network_id,
a_entity_type,b_system_id,b_network_id,b_entity_type,cable_category,sub_category,
route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',
section_name, generic_section_name,hierarchy_type,aerial_loaction
from temp_du_cable where system_id=rec.system_id
returning system_id into current_system_id;
end if;

select display_name into v_a_display_value from point_master where system_id=REC.a_system_id and upper(entity_type)=upper(REC.a_entity_type);
select display_name into v_b_display_value from point_master where system_id=REC.b_system_id and upper(entity_type)=upper(REC.b_entity_type);

--INSERT INTO LINE MASTER

if (coalesce(rec.cable_type,'') !='ISP') then 
Raise info '------------------------------------rec.cable_type123456%',rec.cable_type;
INSERT INTO line_master(system_id, entity_type, approval_flag,sp_geometry,approval_date,creator_remark,approver_remark, created_by,
approver_id, common_name, db_flag, network_status,display_name)
values(current_system_id, 'Cable','A',st_geomfromtext(rec.sp_geometry,4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, p_upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(current_system_id, 'Cable'));
--from temp_du_cable t join kml_attributes k on upper(k.cable_name)=upper(k.cable_name) and upload_id=k.uploaded_id
--where t.system_id = rec.system_id and t.upload_id=p_upload_id;

end if;

perform(fn_isp_create_OSP_Cable(current_system_id));
perform(fn_cable_set_end_point(current_system_id));
perform fn_set_cable_color_info(current_system_id, rec.created_by, rec.no_of_tube, rec.core_per_tube);
perform (fn_geojson_update_entity_attribute(current_system_id::integer,'Cable'::character varying ,rec.province_id::integer,0,false));

insert into associate_entity_info(entity_system_id,entity_type,entity_network_id,entity_display_name,associated_system_id,associated_entity_type,associated_network_id,associated_display_name,created_by,created_on,is_termination_point)
values(current_system_id,'Cable',v_network_code,fn_get_display_name(current_system_id, 'Cable'),REC.a_system_id,REC.a_entity_type,REC.a_network_id,v_a_display_value,rec.created_by,now(),true),
(current_system_id,'Cable',v_network_code,fn_get_display_name(current_system_id, 'Cable'),REC.b_system_id,REC.b_entity_type,REC.b_network_id,v_b_display_value,rec.created_by,now(),true);

insert into att_details_cable_cdb (circle_name, major_route_name, route_id, section_name, section_id, route_category, distance, fiber_pairs_laid, total_used_pair, fiber_pairs_used_by_vil, fiber_pairs_given_to_airtel, fiber_pairs_given_to_others, fiber_pairs_free, faulty_fiber_pairs, start_latitude, start_longitude, end_latitude, end_longitude, count_non_vil_tenancies_on_route, route_lit_up_date, aerial_km, avg_loss_per_km, avg_last_six_months_fiber_cut, row, material, execution, row_availablity, iru_given_airtel, iru_given_jio, iru_given_ttsl_or_ttml, network_category, row_valid_or_exp, remarks, cable_owner, route_type, operator_type, fiber_type, cable_id, iru_given_tcl, iru_given_others)
select circle_name, major_route_name, route_id, section_name, section_id, route_category, distance, fiber_pairs_laid, total_used_pair, fiber_pairs_used_by_vil, fiber_pairs_given_to_airtel, fiber_pairs_given_to_others, fiber_pairs_free, faulty_fiber_pairs, start_latitude, start_longitude, end_latitude, end_longitude, count_non_vil_tenancies_on_route, route_lit_up_date, aerial_km, avg_loss_per_km, avg_last_six_months_fiber_cut, row, material, execution, row_availablity, iru_given_airtel, iru_given_jio, iru_given_ttsl_or_ttml, network_category, row_valid_or_exp, remarks, cable_owner, route_type, operator_type, fiber_type, (SELECT att.system_id FROM  att_details_cable AS att JOIN  temp_du_att_details_cable_cdb AS tempcdb ON  att.network_id  = tempcdb.cable_id WHERE  tempcdb.cable_id = v_network_code  AND tempcdb.upload_id = p_upload_id) as cable_id, iru_given_tcl, iru_given_others from temp_du_att_details_cable_cdb where cable_id = v_network_code and upload_id = p_upload_id and is_valid=true;
raise info '--------------network id :%',v_network_code;

END;
END LOOP;



return 1;

END;
$BODY$;










-- FUNCTION: public.fn_uploader_insert_manhole(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_manhole(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_manhole(
	p_uploadid integer,
	p_batchid integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

declare
REC RECORD;
v_current_system_id integer;
v_parent_system_id integer;
v_parent_network_id character varying;
v_parent_entity_type character varying;
v_network_code character varying;
v_sequence_id integer;
v_latitude double precision;
v_longitude double precision;
v_network_id_type character varying;
v_network_code_seperator character varying;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='MANHOLE';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_manhole where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
LOOP
BEGIN
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.manhole_name;
-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('MANHOLE') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Manhole', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_manhole(
network_id,manhole_name,latitude,longitude,province_id,
region_id, address,specification,category,subcategory1,subcategory2,subcategory3,
item_code,vendor_id,status,created_by,created_on,network_status,construction_type,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,type,brand,model,construction,activation,accessibility,is_virtual,
ownership_type, source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y,area,authority,route_name,
 section_name, generic_section_name,hierarchy_type,aerial_loaction
)
select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,rec.specification,
rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),coalesce(rec.network_status,'P'),
rec.construction_type,rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
rec.is_virtual,'Own', 'DU', p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,rec.area,rec.authority,rec.route_name,
rec.section_name, rec.generic_section_name,rec.hierarchy_type,rec.aerial_loaction
returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master (system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
select v_current_system_id,'Manhole',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Manhole'),rec.st_x,rec.st_y ;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_manhole set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Manhole'::character varying ,rec.province_id::integer,0,false));

END;
END LOOP;
return 1;
END;
$BODY$;




CREATE OR REPLACE FUNCTION public.fn_uploader_insert_spliceclosure(
	p_uploadid integer,
	p_batchid integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

declare
REC RECORD;
v_current_system_id integer;
v_parent_system_id integer;
v_parent_network_id character varying;
v_parent_entity_type character varying;
v_network_code character varying;
v_sequence_id integer;
v_network_id_type character varying;
v_latitude double precision;
v_longitude double precision;
v_input_port integer;
v_output_port integer;
v_is_virtual_port_allowed boolean;
v_network_code_seperator character varying;
v_geom_port character varying;
v_display_name character varying;
v_display_value character varying;
v_str_system_id integer;
v_flr_system_id integer;
v_shaft_system_id integer;
v_unit_id integer;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='SPLICECLOSURE';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_spliceclosure where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN
v_network_name:=rec.spliceclosure_name;
v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;

if rec.no_of_input_port>0 and rec.no_of_output_port>0 then
v_input_port:=rec.no_of_input_port;
v_output_port:=rec.no_of_output_port;
v_geom_port:=rec.no_of_input_port::character varying||':'||rec.no_of_output_port::character varying;
else
v_input_port:=rec.no_of_port;
v_output_port:=rec.no_of_port;
v_geom_port:=rec.no_of_port::character varying;
end if;

v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_shaft_system_id:= rec.shaft_id;
v_flr_system_id:= rec.floor_id;

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('SpliceClosure') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('SpliceClosure', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(upper(rec.parent_entity_type)='STRUCTURE')THEN
select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
where network_id=rec.parent_network_id;
END IF;

IF(upper(rec.parent_entity_type)!='STRUCTURE')
THEN
select id into v_unit_id
from isp_entity_mapping where entity_id=rec.parent_system_id and upper(entity_type)=upper(rec.parent_entity_type);
END IF;
raise info'v_parent_system_id::%',v_parent_system_id;
raise info'v_parent_network_id::%',v_parent_network_id;
raise info'v_parent_entity_type::%',v_parent_entity_type;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_spliceclosure(network_id, spliceclosure_name, latitude, longitude, province_id,
region_id, address, specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, status, created_by, created_on, network_status, parent_system_id, parent_network_id,
parent_entity_type, sequence_id,type,brand,model,construction,activation,accessibility,no_of_input_port,
no_of_output_port,no_of_ports,ownership_type, audit_item_master_id, structure_id, source_ref_type,
source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,
requested_by,request_approved_by,bom_sub_category,st_x,st_y, section_name, generic_section_name,hierarchy_type,aerial_loaction)
select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,
rec.specification,rec.category,
rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),
coalesce(rec.network_status,'P'),rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
rec.no_of_input_port,rec.no_of_output_port,
rec.no_of_port,'Own', rec.audit_item_master_id, v_str_system_id, 'DU', p_uploadid,
rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,
rec.section_name, rec.generic_section_name,rec.hierarchy_type,rec.aerial_location
returning system_id into v_current_system_id;

select clabel.display_column_name into v_display_name from display_name_settings clabel
inner join layer_details ld on clabel.layer_id=ld.layer_id where upper(ld.layer_name)=upper('SpliceClosure');
execute 'select '||v_display_name||' from att_details_spliceclosure where system_id='||v_current_system_id||'' into v_display_value;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,approver_id, common_name, db_flag, network_status,no_of_ports,display_name,st_x,st_y)
select v_current_system_id, 'SpliceClosure',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),v_geom_port,fn_get_display_name(v_current_system_id,'SpliceClosure'),rec.st_x,rec.st_y;

-- INSERT PORT DETAILS..
if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'SpliceClosure', v_current_system_id, v_network_code,rec.created_by);
end if;

-- INSERT isp_entity_mapping..

IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;

insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'SpliceClosure', v_current_system_id, coalesce(v_unit_id,0));
End if;
END;
-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_spliceclosure set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'SpliceClosure'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;















