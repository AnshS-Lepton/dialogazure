-- FUNCTION: public.fn_trg_audit_line_master()

-- DROP FUNCTION IF EXISTS public.fn_trg_audit_line_master();

CREATE OR REPLACE FUNCTION public.fn_trg_audit_line_master()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$

declare

v_served_by_ring text;

v_dsa_system_id integer;

v_layer_table text;
v_subarea_system_id integer;

begin

                v_dsa_system_id:=0;

IF (TG_OP = 'INSERT' ) THEN 

 

				INSERT INTO public.audit_line_master(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,is_virtual,action,modified_by,gis_design_id)

				values(new.system_id,new.entity_type,new.approval_flag,new.sp_geometry,new.creator_remark,new.approver_remark,new.created_by,new.approver_id,new.common_name,new.db_flag,new.approval_date,now(),new.network_status,new.is_virtual,'I',new.modified_by,new.gis_design_id);

       
                select system_id into v_dsa_system_id  from polygon_master where entity_type='DSA' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1;                    
                update att_details_subarea set is_association_completed=false where system_id in(select system_id from polygon_master where entity_type='SubArea' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1);

				if(coalesce(v_dsa_system_id,0)>0)

                then

                                select served_by_ring into v_served_by_ring from att_details_dsa where system_id=v_dsa_system_id;

                                execute 'update '||v_layer_table||' set served_by_ring='''||v_served_by_ring||''' where system_id='||new.system_id||' ';

                end if;
				-- IF (new.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 				THEN
-- 					PERFORM fn_update_entity_geojson('INSERT',new.entity_type,'Line',new.SYSTEM_ID, new.sp_geometry);
-- 				END IF;
    if(upper(new.entity_type) = 'CABLE') then
       PERFORM fn_update_routing_topology(new.system_id,'INSERT');
   end if;
END IF;

 

IF (TG_OP = 'UPDATE' ) THEN 

IF((select st_astext(new.sp_geometry)) !=(select st_astext(old.sp_geometry))) then

 

                INSERT INTO public.audit_line_master(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,is_virtual,action,modified_by,gis_design_id)

                values(new.system_id,new.entity_type,new.approval_flag,new.sp_geometry,new.creator_remark,new.approver_remark,new.created_by,new.approver_id,new.common_name,new.db_flag,new.approval_date,now(),new.network_status,new.is_virtual,'U',new.modified_by,new.gis_design_id); 


                select system_id into v_dsa_system_id  from polygon_master where entity_type='DSA' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1;   
				
                update att_details_subarea set is_association_completed=false where system_id in(select system_id from polygon_master where entity_type='SubArea' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1);

                if(coalesce(v_dsa_system_id,0)>0)

                then

                                select served_by_ring into v_served_by_ring from att_details_dsa where system_id=v_dsa_system_id;

                                execute 'update '||v_layer_table||' set served_by_ring='''||v_served_by_ring||''' where system_id='||new.system_id||' ';

                end if;
				
				-- IF (new.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 				THEN
-- 					PERFORM fn_update_entity_geojson('UPDATE', new.entity_type, 'Line', new.SYSTEM_ID, new.sp_geometry);
-- 				END IF;
END IF;
    if(upper(new.entity_type) = 'CABLE') then
        perform fn_update_routing_topology(new.system_id,'UPDATE');
    END IF;
END IF;

 
IF (TG_OP = 'DELETE' ) THEN 
-- 		IF (old.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 		THEN
-- 			PERFORM fn_update_entity_geojson('DELETE', old.entity_type, 'Line', old.SYSTEM_ID, old.sp_geometry);
-- 		END IF;
  if(upper(old.entity_type) = 'CABLE') then
    PERFORM fn_update_routing_topology(old.system_id,'DELETE');
   END IF;
	RETURN OLD;
END IF;

if(upper(new.entity_type) = 'CABLE') then
                select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(new.entity_type);
 		 
         EXECUTE 'UPDATE ' || v_layer_table ||
        ' SET cable_category = ''Feeder Cable'' ' ||
        ' WHERE cable_category IS NULL AND system_id = ' || NEW.system_id;
end if;
RETURN NEW;

END;

$BODY$;

ALTER FUNCTION public.fn_trg_audit_line_master()
    OWNER TO postgres;
------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_utilization_show_on_map(p_networkstatues character varying, p_regionids character varying, p_provinceids character varying, p_layer_ids character varying, p_projectcode character varying, p_planningcode character varying, p_workordercode character varying, p_purposecode character varying, p_geom character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_layer record;
    sql TEXT;
BEGIN
    -- Normalize network status
    p_networkstatues := CASE WHEN upper(p_networkstatues) = 'AS-BUILT' THEN 'AS BUILT' ELSE upper(p_networkstatues) END;

    -- Temp table to accumulate features
    CREATE TEMP TABLE temp_geojson_features(
        system_id integer,
        geom geometry,
        network_id text,
        entity_title text,
        entity_name text,
        utilization text
    ) ON COMMIT DROP;

   	       raise info 'sql1%',p_networkstatues;

IF ((p_geom) != '')
THEN

  Create temp table temp_entity_inside_geom
  (
      system_id integer,
      entity_type character varying
  )
  on commit drop;
  -- HERE WE ARE FETCHING ALL THE ENTITY WITHIN THE ARAE SELECTED BY USER INTO A TEMP TABLE.

  perform(fn_get_export_report_entity_within_geom(p_layer_ids,p_geom));
  
end if;

    -- Loop through selected layers
    FOR v_layer IN
        SELECT DISTINCT ld.layer_id, ld.layer_name, ld.REPORT_VIEW_NAME, ld.GEOM_TYPE, ld.LAYER_TITLE
        FROM layer_details ld
        WHERE case when p_layer_ids !='' then ld.LAYER_ID = ANY(string_to_array(p_layer_ids, ',')::int[]) else 1=1 end
          AND ld.is_utilization_enabled = true and ld.isvisible = true
        ORDER BY ld.layer_id
    loop
	    
	       raise info 'sql1%',v_layer.layer_name;

        -- Build dynamic SQL for geometry filter
        
	    IF p_geom != '' THEN
           
         sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization)
        SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization
        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
        INNER JOIN temp_entity_inside_geom tm ON tm.system_id = a.system_id
        LEFT JOIN ' || v_layer.GEOM_TYPE || '_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.layer_name || ''') and upper(tm.entity_type) = upper(''' || v_layer.layer_name || ''')
        WHERE 1=1';

                       
                       	--FETCH RECORD BASED ON SELECTED GEOMETRY.
	/*	sql:= 'select a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization
                        from '|| v_arow.REPORT_VIEW_NAME||' a inner join temp_entity_inside_geom m
		on a.system_id=m.system_id and upper(m.entity_type)=upper('''||v_arow.layer_name||''') 
		where 1=1 ';
       */    
        ELSE
       
                sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization)
                        SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization
                        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
                        LEFT JOIN ' || v_layer.GEOM_TYPE || '_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.layer_name || ''')
                        WHERE 1=1';
                raise info 'sql1%',sql;       
        END IF;

        -- Apply filters
        IF p_regionids != '' THEN sql := sql || ' AND a.region_id IN (' || p_regionids || ')'; END IF;
        IF p_provinceids != '' THEN sql := sql || ' AND a.province_id IN (' || p_provinceids || ')'; END IF;
        IF p_networkstatues != '' THEN sql := sql || ' AND upper(a.network_status) IN (''' || p_networkstatues || ''')'; END IF;
        IF p_projectcode != '' THEN sql := sql || ' AND a.project_id IN (' || p_projectcode || ')'; END IF;
        IF p_planningcode != '' THEN sql := sql || ' AND a.planning_id IN (' || p_planningcode || ')'; END IF;
        IF p_workordercode != '' THEN sql := sql || ' AND a.workorder_id IN (' || p_workordercode || ')'; END IF;
        IF p_purposecode != '' THEN sql := sql || ' AND a.purpose_id IN (' || p_purposecode || ')'; END IF;
   raise info 'sql%',sql;

        EXECUTE sql;
    END LOOP;
 
  
    -- Return a single GeoJSON FeatureCollection
    RETURN QUERY EXECUTE
    'SELECT row_to_json(row) AS geojson
     FROM (
         SELECT ''FeatureCollection'' AS type,
                array_to_json(array_agg(f)) AS features
         FROM (
             SELECT ''Feature'' AS type,
                    system_id AS id,
                    ST_AsGeoJSON(geom, 15, 0)::json AS geometry,
                    json_build_object(
                        ''network_id'', network_id,
                        ''entity_title'', entity_title,
                        ''entity_name'', entity_name,
                        ''utilization'', utilization
                    ) AS properties
             FROM temp_geojson_features
             WHERE geom IS NOT NULL
         ) AS f
     ) AS row';
END;
$function$
;


------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_utilization_show_on_map(p_networkstatues character varying, p_regionids character varying, p_provinceids character varying, p_layer_ids character varying, p_projectcode character varying, p_planningcode character varying, p_workordercode character varying, p_purposecode character varying, p_geom character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_layer record;
    sql TEXT;
BEGIN
    -- Normalize network status
    p_networkstatues := CASE WHEN upper(p_networkstatues) = 'AS-BUILT' THEN 'AS BUILT' ELSE upper(p_networkstatues) END;

    -- Temp table to accumulate features
   CREATE TEMP TABLE temp_geojson_features(
        system_id integer,
        geom geometry,
        network_id text,
        entity_title text,
        entity_name text,
        utilization text,
        low_count integer,
        moderate_count integer, 
        high_count integer,
        over_count integer
        
    ) ON COMMIT DROP;

   	       raise info 'sql1%',p_networkstatues;

IF ((p_geom) != '')
THEN

  Create temp table temp_entity_inside_geom
  (
      system_id integer,
      entity_type character varying
  )
  on commit drop;
  -- HERE WE ARE FETCHING ALL THE ENTITY WITHIN THE ARAE SELECTED BY USER INTO A TEMP TABLE.

  perform(fn_get_export_report_entity_within_geom(p_layer_ids,p_geom));
  
end if;

    -- Loop through selected layers
    FOR v_layer IN
        SELECT DISTINCT ld.layer_id, ld.layer_name, ld.REPORT_VIEW_NAME, ld.GEOM_TYPE, ld.LAYER_TITLE
        FROM layer_details ld
        WHERE case when p_layer_ids !='' then ld.LAYER_ID = ANY(string_to_array(p_layer_ids, ',')::int[]) else 1=1 end
          AND ld.is_utilization_enabled = true and ld.isvisible = true
        ORDER BY ld.layer_id
    loop
	    
	       raise info 'sql1%',v_layer.layer_name;

        -- Build dynamic SQL for geometry filter
        
	    IF p_geom != '' THEN
         IF upper(v_layer.LAYER_NAME) = 'DUCT' THEN

          sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization,low_count,moderate_count,high_count,over_count )
        SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization, 0,0,0,0
        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
        INNER JOIN temp_entity_inside_geom tm ON tm.system_id = a.system_id
        LEFT JOIN ' || v_layer.GEOM_TYPE || '_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.layer_name || ''') and upper(tm.entity_type) = upper(''' || v_layer.layer_name || ''')
        WHERE 1=1';
       
         else
         sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization,low_count,moderate_count,high_count,over_count )
        SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization, coalesce(sum(case when ((a.network_status=''Planned'' or a.network_status=''As Built'') and a.utilization=''L'') then 1 else 0 end),0)low_count,
		coalesce(sum(case when ((a.network_status=''Planned'' or a.network_status=''As Built'') and a.utilization=''M'') then 1 else 0 end),0)moderate_count,
		coalesce(sum(case when ((a.network_status=''Planned'' or a.network_status=''As Built'') and a.utilization=''H'') then 1 else 0 end),0)high_count,
		coalesce(sum(case when ((a.network_status=''Planned'' or a.network_status=''As Built'') and a.utilization=''O'') then 1 else 0 end),0)over_count
        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
        INNER JOIN temp_entity_inside_geom tm ON tm.system_id = a.system_id
        LEFT JOIN ' || v_layer.GEOM_TYPE || '_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.layer_name || ''') and upper(tm.entity_type) = upper(''' || v_layer.layer_name || ''')
        WHERE 1=1
';
   end if;
        else
        
  IF upper(v_layer.LAYER_NAME) = 'DUCT' 
  THEN
    sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization,low_count,moderate_count,high_count,over_count)
            SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization,0,0,0,0
            FROM ' || v_layer.REPORT_VIEW_NAME || ' a
            LEFT JOIN ' || v_layer.GEOM_TYPE || '_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.layer_name || ''')
            WHERE 1=1';
ELSE

          sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization,low_count,moderate_count,high_count,over_count)
                        SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization, coalesce(sum(case when ((a.network_status=''Planned'' or a.network_status=''As Built'') and a.utilization=''L'') then 1 else 0 end),0)low_count,
		coalesce(sum(case when ((a.network_status=''Planned'' or a.network_status=''As Built'') and a.utilization=''M'') then 1 else 0 end),0)moderate_count,
		coalesce(sum(case when ((a.network_status=''Planned'' or a.network_status=''As Built'') and a.utilization=''H'') then 1 else 0 end),0)high_count,
		coalesce(sum(case when ((a.network_status=''Planned'' or a.network_status=''As Built'') and a.utilization=''O'') then 1 else 0 end),0)over_count
                        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
                        LEFT JOIN ' || v_layer.GEOM_TYPE || '_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.layer_name || ''')
                        WHERE 1=1 
';
                raise info 'sql1%',sql;       
        END IF;
        END IF;

        -- Apply filters
        IF p_regionids != '' THEN sql := sql || ' AND a.region_id IN (' || p_regionids || ')'; END IF;
        IF p_provinceids != '' THEN sql := sql || ' AND a.province_id IN (' || p_provinceids || ')'; END IF;
        IF p_networkstatues != '' THEN sql := sql || ' AND upper(a.network_status) IN (''' || p_networkstatues || ''')'; END IF;
        IF p_projectcode != '' THEN sql := sql || ' AND a.project_id IN (' || p_projectcode || ')'; END IF;
        IF p_planningcode != '' THEN sql := sql || ' AND a.planning_id IN (' || p_planningcode || ')'; END IF;
        IF p_workordercode != '' THEN sql := sql || ' AND a.workorder_id IN (' || p_workordercode || ')'; END IF;
        IF p_purposecode != '' THEN sql := sql || ' AND a.purpose_id IN (' || p_purposecode || ')'; END IF;
   raise info 'sql%',sql;

  sql = sql || ' GROUP BY a.system_id, m.sp_geometry, a.network_id, a.utilization';
        EXECUTE sql;
    END LOOP;
 
  
    -- Return a single GeoJSON FeatureCollection
    RETURN QUERY EXECUTE
    'SELECT row_to_json(row) AS geojson
     FROM (
         SELECT ''FeatureCollection'' AS type,
                array_to_json(array_agg(f)) AS features
         FROM (
             SELECT ''Feature'' AS type,
                    system_id AS id,
                    ST_AsGeoJSON(geom, 15, 0)::json AS geometry,
                    json_build_object(
                        ''network_id'', network_id,
                        ''entity_title'', entity_title,
                        ''entity_name'', entity_name,
                        ''utilization'', utilization,
						''low_count'', low_count,
                         ''moderate_count'', moderate_count,
                         ''high_count'', high_count,
                         ''over_count'', over_count
                    ) AS properties
             FROM temp_geojson_features
             WHERE geom IS NOT NULL
         ) AS f
     ) AS row';
END;
$function$
;

------------------------------------------------------------------------------------------------

--DROP TABLE cpf_temp_result
CREATE OR REPLACE FUNCTION public.fn_get_fms_connection_report_new(
	p_networkstatues character varying,
	p_provinceids character varying,
	p_regionids character varying,
	p_layer_name character varying,
	p_searchby character varying,
	p_searchbytext character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_geom character varying,
	p_duration_based_column character varying,
	p_radius double precision,
	p_userid integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
V_AROW RECORD;
v_entity_port_no character varying;
v_maxcount integer;
v_maxpathid integer;
v_ststem_id integer;
v_entity_type integer;
v_maxfms_id integer;
sql TEXT;
StartSNo INTEGER; 
EndSNo INTEGER;
LowerStart character varying;
LowerEnd character varying;
TotalRecords integer; 
WhereCondition character varying;
s_report_view_name character varying;
s_geom_type character varying;
s_layer_id integer; 
s_layer_columns text;
TotalAppliedRecords integer; 
TotalRejectedRecords integer;
 r RECORD;
    ss_report_view_name TEXT;
    d_report_view_name TEXT;
    v_report_view_name TEXT;
    ss_layer_name TEXT;
    update_sql TEXT;
s_latitude text;
d_layer_name TEXT;
v_layer_name TEXT;
BEGIN

create TEMP  table cpf_temp_result
(
id serial,
source_system_id integer,
source_network_id character varying(500),
source_port_no integer, 
source_entity_type character varying(100),
source_entity_title character varying(100),
destination_system_id integer,
destination_network_id character varying(500),
destination_port_no integer,
destination_entity_type character varying(100),
destination_entity_title character varying(100),
viya_system_id integer,
via_network_id character varying(500),
path_id integer,
via_entity_type character varying(100),
SOURCE_TUBE_COLOUR_CODE character varying(100),
DESTINATION_TUBE_COLOUR_CODE character varying(100),
SOURCE_PORT_COLOUR_CODE character varying(100),
DESTINATION_PORT_COLOUR_CODE character varying(100),
SOURCE_TUBE_NAME character varying(100),
DESTINATION_TUBE_NAME character varying(100),
fms_id integer,
splitter_id integer,
entity_type character varying(100),
client_id character varying,
client_link_id character varying,
client_core_id character varying,
otdr_length float(8) default (0),
op_alias character varying
)on commit drop; 

-------------------------------------------------------------------------------------------------------------------
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER.. 
SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,res_field_key,DISPLAY_NAME FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
IF ((p_geom) != '') 
THEN
if(p_radius>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10) into p_geom;
end if;
RAISE INFO '------------------------------------S_LAYER_COLUMNS%', S_LAYER_COLUMNS;
sql:= 'SELECT system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join '||s_geom_type||'_master m
on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||'  and 1=1 ';
--inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
RAISE INFO '------------------------------------sql%', sql;
ELSE
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
--inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
RAISE INFO '------------------------------------sql1%', sql;
END IF;

IF ((p_networkStatues) != '' and upper(S_LAYER_COLUMNS) like '%NETWORK_STATUS%') THEN
sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in('||p_networkStatues||')';
END IF;
IF ((p_RegionIds) != '') THEN
sql:= sql ||' AND a.region_id IN ('||p_RegionIds||')';
END IF;
IF ((P_ProvinceIds) != '') THEN
sql:= sql ||' AND a.province_id IN ('||P_ProvinceIds||')';
END IF;
IF ((P_searchbytext) != '' and (P_searchby) != '') THEN
if(substring(P_searchbytext from 1 for 1)='"' and substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
then
sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
else
sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
end if;
END IF;
IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND a.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';
END IF;
sql:= sql ||' )X where system_id in(select parent_system_id from isp_port_info where parent_entity_type=''FMS'' and port_status_id=2 )';
RAISE INFO 'rrr%', sql;
--------------------------------------------------------------------------------------------------------------------
FOR V_AROW IN
EXECUTE 'SELECT ''FMS'' as entity_type, system_id FROM ('||sql||')a'
loop
perform(fn_get_fms_connection12(V_AROW.system_id,V_AROW.entity_type));
end loop;
--------------------------------------------------------------------------------------------------
delete from cpf_temp_result where source_network_id=destination_network_id;
if exists(select 1 from cpf_temp_result where source_entity_type='FMS')
then
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE source_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	-- Update query
	UPDATE cpf_temp_result 
	SET 
	destination_network_id = VIA_row.destination_network_id,
	destination_system_id = VIA_row.destination_system_id,
	destination_port_no = VIA_row.destination_port_no,
	destination_entity_type = VIA_row.destination_entity_type,
	destination_entity_title = VIA_row.destination_entity_title,
	viya_system_id = VIA_row.source_system_id,
	via_network_id = VIA_row.source_network_id,
	via_entity_type = VIA_row.source_entity_type

	FROM VIA_row
	WHERE 
	cpf_temp_result.destination_entity_type = VIA_row.source_entity_type
	AND cpf_temp_result.destination_system_id = VIA_row.source_system_id
	AND cpf_temp_result.destination_network_id = VIA_row.source_network_id
	AND cpf_temp_result.destination_PORT_NO = VIA_row.source_PORT_NO;
	-- Delete query
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result 
	WHERE source_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	DELETE FROM cpf_temp_result
	WHERE SOURCE_SYSTEM_ID IN (SELECT SOURCE_SYSTEM_ID FROM VIA_row);
else

	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE destination_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	-- Update query
	UPDATE cpf_temp_result 
	SET 
	source_network_id = VIA_row.source_network_id,
	source_system_id = VIA_row.source_system_id,
	source_port_no = VIA_row.source_port_no,
	source_entity_type = VIA_row.source_entity_type,
	source_entity_title = VIA_row.source_entity_title,
	viya_system_id = VIA_row.destination_system_id,
	via_network_id = VIA_row.destination_network_id,
	via_entity_type = VIA_row.destination_entity_type

	FROM VIA_row
	WHERE 
	cpf_temp_result.source_entity_type = VIA_row.source_entity_type
	AND cpf_temp_result.source_system_id = VIA_row.source_system_id
	AND cpf_temp_result.source_network_id = VIA_row.source_network_id
	AND cpf_temp_result.source_PORT_NO = VIA_row.source_PORT_NO;
	-- Delete query
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result 
	WHERE destination_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	DELETE FROM cpf_temp_result
	WHERE destination_SYSTEM_ID IN (SELECT destination_SYSTEM_ID FROM VIA_row);
 
end if;

--select count(*) into v_maxcount from cpf_temp_result group by path_id order by count(*) desc limit 1;--header
select count(*) into v_maxcount from cpf_temp_result group by fms_id,path_id order by count(*) desc limit 1;
SELECT COUNT(DISTINCT path_id) AS path_count
FROM cpf_temp_result group by fms_id order by COUNT(DISTINCT path_id) desc limit 1 
INTO v_maxpathid;---dataloop
SELECT COUNT(DISTINCT fms_id) AS fms_id_count from cpf_temp_result
INTO v_maxfms_id;
------------------------------------------------------------------------------------------------------------------------------------
UPDATE cpf_temp_result 
SET SOURCE_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
SOURCE_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
SOURCE_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code ,client_id =adfl.customer_id,client_link_id =adfl.link_id,client_core_id=adfl.link_id,otdr_length=adfl.otdr_distance,op_alias=adfl.service_id
FROM ATT_DETAILS_CABLE_INFO 
left join att_details_fiber_link adfl on adfl.system_id =ATT_DETAILS_CABLE_INFO.link_system_id 
WHERE cpf_temp_result.SOURCE_ENTITY_type='Cable'
and cpf_temp_result.source_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.source_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;

--select * from ATT_DETAILS_CABLE_INFO limit 20 where cable_id=847

UPDATE cpf_temp_result 
SET DESTINATION_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
DESTINATION_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
DESTINATION_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code ,client_id =adfl.customer_id,client_link_id =adfl.link_id,client_core_id=adfl.link_id,otdr_length=adfl.otdr_distance,op_alias=adfl.service_id
FROM ATT_DETAILS_CABLE_INFO 
left join att_details_fiber_link adfl on adfl.system_id =ATT_DETAILS_CABLE_INFO.link_system_id  
WHERE cpf_temp_result.DESTINATION_ENTITY_type='Cable'
and cpf_temp_result.DESTINATION_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.DESTINATION_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;



for  r in  select  * from cpf_temp_result loop

select report_view_name,layer_name into ss_report_view_name,ss_layer_name from layer_details where lower(layer_name) =lower(r.source_entity_type);
select report_view_name,layer_name into d_report_view_name,d_layer_name from layer_details where lower(layer_name) =lower(r.destination_entity_type);
if r.via_entity_type is not null then 
select report_view_name,layer_name into v_report_view_name,v_layer_name from layer_details where lower(layer_name) =lower(r.via_entity_type);
end if;
if lower(ss_layer_name) = 'patchcord'   then 
ss_layer_name ='patch_cord';
end if;
if  lower(d_layer_name) = 'patchcord'  then 
d_layer_name ='patch_cord';
end if;
if   lower(v_layer_name) = 'patchcord' then 
v_layer_name ='patch_cord';
end if ;

ss_layer_name:=ss_layer_name||'_name';
d_layer_name:= d_layer_name||'_name';
v_layer_name =v_layer_name||'_name';
s_latitude ='latitude';
END LOOP;
  UPDATE cpf_temp_result t
SET source_network_id = p.source_network_id
FROM cpf_temp_result p
WHERE 
  --t.client_link_id = p.client_link_id
   t.source_network_id ILIKE '%CBL%'
  AND p.source_network_id ILIKE '%POD%';

-- Update destination_network_id if it contains CBL
UPDATE cpf_temp_result t
SET destination_network_id = p.destination_network_id
FROM cpf_temp_result p
WHERE 
   t.destination_network_id ILIKE '%CBL%'
  AND p.destination_network_id ILIKE '%POD%';
  

return query
select row_to_json(result) 
from (select (select array_to_json(array_agg(row_to_json(x)))from (select  source_port_no,client_link_id,source_network_id,destination_network_id,op_alias
from cpf_temp_result  where source_ENTITY_TYPE='FMS' OR DESTINATION_ENTITY_TYPE='FMS' order by id) x) as lstConnectionInfo
)result ;

END; 
$BODY$;
