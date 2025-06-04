
------------------------------------------Modify existing function------------------------------------------

DROP FUNCTION IF EXISTS public.fn_get_route_connectedelement_details(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_get_route_connectedelement_details(
	p_route_id integer,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
DECLARE 
v_Query text;
p_site1_geom character varying;
	v_geometry_with_buffer geometry;
	 p_site2_geom character varying;
	 startroute integer default 1;
	 geom_text TEXT;
Query text; 
rec record;

BEGIN
	create temp table temp_connectedElement(
		connected_system_id integer, 
		connected_network_id character varying(100), 
		connected_entity_type character varying(100),
		connected_entity_geom character varying ,
		is_virtual boolean,
		is_agg_site boolean
	)on commit drop;
	
	 create temp table temp_cableInfo(
		
	        cable_system_id integer,
	        cable_network_id character varying(100),
	        cable_type character varying(100),
	        cable_geom character varying
	       
	)on commit drop;
	
 -- Create a temporary table to store site geometries
    CREATE TEMP TABLE temp_site_geom_route( 
        site_id integer, 
        site_name character varying, 
        geom geometry,
		ringid integer,
        user_id integer,
		 cbl_distance numeric
    ) ON COMMIT DROP; 
	

Raise info 'p_route_id ->%',p_route_id;
 
 -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom_route(site_id, site_name, geom,ringid, user_id,cbl_distance)
     select distinct sitedetails.system_id , sitedetails.site_name , ST_GeomFromText('POINT(' || sitedetails.longitude || ' ' || sitedetails.latitude || ')', 4326),sitedetails.ring_id,p_user_id,sitedetails.length_meters 
	 from  (select pod.system_id,pod.site_name,pod.longitude,pod.latitude,pod.ring_id,0 AS length_meters  
			from att_details_pod pod
    where pod.system_id in (SELECT agg1site_id AS site_id FROM top_routes WHERE route_id = p_route_id
UNION ALL
SELECT agg2site_id AS site_id FROM top_routes WHERE route_id = p_route_id)) sitedetails;
	

insert into temp_cableInfo(cable_system_id,cable_network_id,cable_type,cable_geom)
select distinct lm.system_id,cbl.network_id,lm.entity_type,st_astext(lm.sp_geometry) as connected_entity_geom 
from att_details_cable cbl 
		inner join line_master lm ON  upper(lm.entity_type)='CABLE' and cbl.system_id=lm.system_id
		where cbl.network_id in (select unnest(string_to_array(regexp_replace(cable_names, '[\{\}\n\r]+', ',', 'g'), ',')) AS code from top_routes where route_id=p_route_id) ;


Raise info 'temp_cableInfo row ->%',(select count(1) from temp_cableInfo);
		
insert into temp_connectedElement(connected_system_id,connected_network_id,connected_entity_type,connected_entity_geom,is_virtual,is_agg_site)
		select pm.system_id,pod.network_id,pm.entity_type,st_astext(pm.sp_geometry) as connected_entity_geom,pm.is_virtual,pod.is_agg_site from temp_site_geom_route ci
		inner join att_details_pod pod on pod.system_id=ci.site_id
		inner join point_master pm ON  upper(pm.entity_type)='POD' and pod.system_id=pm.system_id;
		
Raise info 'temp_connectedElement row ->%',(select count(1) from temp_connectedElement);

   
	RETURN QUERY select row_to_json(result) 
	from (
		select (
			select array_to_json(array_agg(row_to_json(x)))
			from (
				select * from temp_cableInfo
				
			) x
		) as lstCableInfo,
		(
			select array_to_json(array_agg(row_to_json(x)))
			from (
				select distinct connected_system_id,connected_network_id,connected_entity_type
				 ,connected_entity_geom ,is_virtual,COALESCE(is_agg_site, false) AS is_agg_site from temp_connectedElement temp
				inner join layer_details ld on upper(temp.connected_entity_type)=upper(ld.layer_name)
				
			) x
		) as lstConnectedElements
	)result;
 

END 
$BODY$;

ALTER FUNCTION public.fn_get_route_connectedelement_details(integer, integer)
    OWNER TO postgres;

	-------------------------------------------Modify existing function as per need by mobile team ID 5039------------------------------------------
	
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
	SYSTEM_ID INTEGER,ENTITY_TYPE CHARACTER VARYING,LONGITUDE DOUBLE PRECISION,LATITUDE DOUBLE PRECISION,
    APPROVAL_FLAG CHARACTER VARYING,SP_GEOMETRY geometry,APPROVAL_DATE TIMESTAMP WITHOUT TIME ZONE,
    CREATOR_REMARK TEXT,APPROVER_REMARK TEXT,CREATED_BY INTEGER,APPROVER_ID INTEGER,
    COMMON_NAME CHARACTER VARYING,DB_FLAG INTEGER,MODIFIED_ON TIME WITHOUT TIME ZONE,
    NETWORK_STATUS CHARACTER VARYING,NO_OF_PORTS CHARACTER VARYING,IS_VIRTUAL BOOLEAN,
    ENTITY_CATEGORY CHARACTER VARYING,IS_BURIED BOOLEAN,DISPLAY_NAME CHARACTER VARYING,
    MODIFIED_BY INTEGER,IS_PROCESSED BOOLEAN,GIS_DESIGN_ID CHARACTER VARYING,ST_X DOUBLE PRECISION,
    ST_Y DOUBLE PRECISION
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
    
  --  commented by Nikhil Arora suggested by Deepak Yadav
    --and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(PTM.status,'A') = 'A' end;
	
	
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
	WHERE ST_Intersects(st_makevalid(coalesce(draftinfo.updated_geom,PTM.sp_geometry)),v_geometry_with_buffer) ;
    --  commented by Nikhil Arora suggested by Deepak Yadav
   -- and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(PTM.status,'A') = 'A' end;

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
	WHERE ST_INTERSECTS(v_geometry_with_buffer,POLY.SP_GEOMETRY) ;
   -- and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(POLY.status,'A') = 'A' end;
    
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
