CREATE OR REPLACE FUNCTION public.fn_get_utilization_report_summary(p_regionids character varying, p_provinceids character varying, p_layerids character varying, p_networkstatues character varying, p_projectcodes character varying, p_planningcodes character varying, p_workordercodes character varying, p_purposecodes character varying, p_geom character varying, p_userid integer, p_roleid integer)
 RETURNS TABLE(summary_id integer, network_status character varying, entity_id integer, entity_title character varying, entity_name character varying, region_id integer, region character varying, province_id integer, province character varying, low_count integer, moderate_count integer, high_count integer, over_count integer, utilization_text character varying)
 LANGUAGE plpgsql
AS $function$

Declare v_arow record;
sql TEXT;
s_layer_columns_val text;
columnquery TEXT;
v_layer_ids text;
v_mduct_system_id integer;
chkIsMicrodct boolean;
Begin

p_networkstatues := replace(coalesce(p_networkstatues, ''), '@', '''');

-- TEMP TABLE TO STORE THE SUMMARY OF EACH ENTITY..
create temp table temputilizationreportsummary
(
    network_status character varying,
    entity_id integer, 
    entity_title character varying, 
    entity_name character varying,
    region_id integer,
    region character varying,
    province_id integer,
    province character varying,
    low_count integer,
    moderate_count integer,
    high_count integer,
    over_count integer,
    utilization_text character varying
)
on commit drop;

if(p_layerids = '')then
select STRING_AGG(LAYER_ID::character varying, ',') into v_layer_ids from LAYER_DETAILS where isvisible = true and is_utilization_enabled = true;
else
v_layer_ids := (p_layerids):: text;
end if;


IF ((p_geom) != '')
THEN

  Create temp table temp_entity_inside_geom
  (
      system_id integer,
      entity_type character varying
  )
  on commit drop;
  -- HERE WE ARE FETCHING ALL THE ENTITY WITHIN THE ARAE SELECTED BY USER INTO A TEMP TABLE.

  raise info 'v_layer_ids%',v_layer_ids;
  perform(fn_get_export_report_entity_within_geom(v_layer_ids,p_geom));
  
end if;

for v_arow in select LAYER_ID,LAYER_TITLE, REPORT_VIEW_NAME, GEOM_TYPE,layer_name from LAYER_DETAILS where isvisible = true and is_utilization_enabled = true  and case when coalesce(v_layer_ids,'')!='' then  (layer_id) in (select unnest(string_to_array(v_layer_ids, ','))::integer) else true end order by layer_name
  loop
	sql := '';
	-- FETCH ALL REPORT COLUMNS FROM LAYER COLUMN SETTINGS TABLE
	SELECT STRING_AGG(COLUMN_NAME, ',') INTO S_LAYER_COLUMNS_VAL FROM (
	SELECT  COLUMN_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=v_arow.LAYER_ID  AND UPPER(SETTING_TYPE)='REPORT'
	ORDER BY COLUMN_SEQUENCE) A;
   IF(upper(S_LAYER_COLUMNS_VAL) like '%NETWORK_STATUS%' and upper(S_LAYER_COLUMNS_VAL) like '%UTILIZATION%')THEN

		IF(upper(v_arow.layer_name)='DUCT') THEN
		
		columnquery :='network_status,cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,
		cast('''||v_arow.layer_name||'''as text)entityName,region_id,region_name,province_id,province_name,
		sum(1) as low_count,
		0 as moderate_count,
		0 as high_count,
		0 as over_count,coalesce(a.utilization,''0'')||''% utilized'' as utilization_text';
		

		ELSE
		-- SHOW NETWORK STATUS COUNT SEPERATLY --
		columnquery :='network_status,cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,
		cast('''||v_arow.layer_name||'''as text)entityName,region_id,region_name,province_id,province_name,
		coalesce(sum(case when ((network_status=''Planned'' or network_status=''As Built'') and utilization=''L'') then 1 else 0 end),0)low_count,
		coalesce(sum(case when ((network_status=''Planned'' or network_status=''As Built'') and utilization=''M'') then 1 else 0 end),0)moderate_count,
		coalesce(sum(case when ((network_status=''Planned'' or network_status=''As Built'') and utilization=''H'') then 1 else 0 end),0)high_count,
		coalesce(sum(case when ((network_status=''Planned'' or network_status=''As Built'') and utilization=''O'') then 1 else 0 end),0)over_count,'''' utilization_text';
		
		END IF;
	
	   
	--DYNAMIC QUERY TO FETCH ENTITY SUMMARY
	IF ((p_geom) != '') THEN
	
		--FETCH RECORD BASED ON SELECTED GEOMETRY.
		sql:= 'select '||columnquery||' from '|| v_arow.REPORT_VIEW_NAME||' a inner join temp_entity_inside_geom m
		on a.system_id=m.system_id and upper(m.entity_type)=upper('''||v_arow.layer_name||''') 
		where 1=1 ';
	ELSE
	
		-- FETCH RECORDS BASED ON SELECTED FILTERS.
		sql:= 'select '||columnquery||' from '|| v_arow.REPORT_VIEW_NAME||' a where 1=1 ';
		
	END IF;
	
	-- REGION FILTER
	IF ((p_RegionIds) != '') THEN
		sql:= sql ||' AND a.region_id IN ('||p_RegionIds||')';
	END IF;
	
	-- PROVINCE FILTER
	IF ((P_ProvinceIds) != '') THEN
		sql:= sql ||' AND a.province_id IN ('||P_ProvinceIds||')';
	END IF;
	
	-- NETOWRK STATUS FILTER
	IF ((p_networkStatues) != '' and upper(S_LAYER_COLUMNS_VAL) like '%NETWORK_STATUS%') THEN
		sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in('||p_networkStatues||')';
	END IF;
	
	-- PROJECT CODE FILTER
	IF ((p_projectcodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PROJECT_ID%') THEN
		sql:= sql ||' AND  a.project_id IN ('||p_projectcodes||')';
	else IF ((p_projectcodes) != '') then 
		sql:= sql ||' AND  0 = 1';
	END IF;
	END IF;
	-- PLANNING CODE FILTER
	IF ((p_planningcodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PLANNING_ID%') THEN
		sql:= sql ||' AND  a.planning_id IN ('||p_planningcodes||')';
	END IF;
	-- WORKORDER CODE FILTER
	IF ((p_workordercodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%WORKORDER_ID%') THEN
		sql:= sql ||' AND  a.workorder_id IN ('||p_workordercodes||')';
	END IF;
	-- PURPOSE CODE FILTER
	IF ((p_purposecodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PURPOSE_ID%') THEN
		sql:= sql ||' AND  a.purpose_id IN ('||p_purposecodes||')';
	END IF;
	IF(upper(v_arow.layer_name)='DUCT') THEN

	sql:= sql || ' group by utilization,network_status,region_id,region_name,province_id,province_name';
	execute ('insert into tempUtilizationReportSummary Select network_status,entityid,entitytitle,entityName,region_id,region_name,province_id,province_name,(low_count),
	(moderate_count),(high_count),(over_count),utilization_text from( '||sql||')result');
	ELSE
	sql:= sql || ' group by network_status,region_id,region_name,province_id,province_name';
	execute ('insert into tempUtilizationReportSummary Select network_status,entityid,entitytitle,entityName,region_id,region_name,province_id,province_name,(low_count),
	(moderate_count),(high_count),(over_count),fn_get_utilization_text (entityid,left(network_status,1),region_id,province_id) from( '||sql||')result');
	
	END IF;
	
	--RAISE EXCEPTION  'Calling result(%)', S_LAYER_COLUMNS;
	--RAISE EXCEPTION  'Calling result(%)', sql;
	-- sql:= sql ||' )X';
	--execute ('insert into tempUtilizationReportSummary Select network_status,entityid,entitytitle,entityName,region_id,region_name,province_id,province_name,sum(low_count),
	--sum(moderate_count),sum(high_count),sum(over_count),fu_get_utilization_text(entityid,network_status,region_id,province_id) from( '||sql||')result group by network_status,entityid,entitytitle,entityName,region_id,region_name,province_id,province_name,utilization_text');
	
	RAISE INFO 'queryFinal % ', sql;
end if;
end loop;

/*RETURN QUERY
Select cast(ROW_NUMBER() OVER (ORDER BY e.network_status desc) as integer)summary_id,* from tempUtilizationReportSummary e 
where (e.low_count + e.moderate_count + e.high_count+ e.over_count) > 0 or upper(e.entity_name)='DUCT'
order by e.network_status desc,entity_title,region,province asc;
End;*/

RETURN QUERY
SELECT 
    CAST(ROW_NUMBER() OVER (
        ORDER BY 
            CASE 
                WHEN e.utilization_text ~ '^[0-9]+%' 
                THEN CAST(split_part(e.utilization_text, '%', 1) AS INTEGER) 
                ELSE 0 
            END DESC,
            e.network_status DESC,
            e.entity_title,
            e.region,
            e.province ASC
    ) AS INTEGER) AS summary_id,
    *
FROM tempUtilizationReportSummary e
WHERE (e.low_count + e.moderate_count + e.high_count+ e.over_count) > 0 
   OR upper(e.entity_name)='DUCT';
end;

$function$
;
-----------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_utilization_text(
    p_layer_id integer, 
    p_network_status text, 
    p_region_id integer, 
    p_province_id integer
) 
RETURNS text
LANGUAGE plpgsql
AS $function$
DECLARE
    v_utilization_text text;	
BEGIN
    IF EXISTS (
        SELECT 1 
        FROM entity_utilization_settings 
        WHERE layer_id = p_layer_id 
          AND network_status = p_network_status 
          AND region_id = p_region_id 
          AND province_id = p_province_id
    ) THEN
        SELECT 
            'Over : ' || utilization_range_over_from || '% to ' || utilization_range_over_to || '%' ||
            ' | High : ' || utilization_range_high_from || '% to ' || utilization_range_high_to || '%' ||
            ' | Moderate : ' || utilization_range_moderate_from || '% to ' || utilization_range_moderate_to || '%' ||
            ' | Low : below ' || utilization_range_low_to || '%'
        INTO v_utilization_text
        FROM entity_utilization_settings
        WHERE layer_id = p_layer_id 
          AND network_status = p_network_status 
          AND region_id = p_region_id 
          AND province_id = p_province_id;
    ELSE
        SELECT 
            'Over : ' || utilization_range_over_from || '% to ' || utilization_range_over_to || '%' ||
            ' | High : ' || utilization_range_high_from || '% to ' || utilization_range_high_to || '%' ||
            ' | Moderate : ' || utilization_range_moderate_from || '% to ' || utilization_range_moderate_to || '%' ||
            ' | Low : below ' || utilization_range_low_to || '%'
        INTO v_utilization_text
        FROM entity_utilization_settings
        WHERE layer_id = 0;
    END IF;

    RETURN v_utilization_text;
END;
$function$;

------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_utilization_show_on_map(
    p_networkstatues character varying,
    p_regionids character varying,
    p_provinceids character varying,
    p_layer_ids character varying,
    p_projectcode character varying,
    p_planningcode character varying,
    p_workordercode character varying,
    p_purposecode character varying,
    p_geom character varying
)
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

    -- Loop through selected layers
    FOR v_layer IN
        SELECT DISTINCT ld.layer_id, ld.layer_name, ld.REPORT_VIEW_NAME, ld.GEOM_TYPE, ld.LAYER_TITLE
        FROM layer_details ld
        WHERE case when p_layer_ids !='' then ld.LAYER_ID = ANY(string_to_array(p_layer_ids, ',')::int[]) else 1=1 end
          AND ld.is_utilization_enabled = true
        ORDER BY ld.layer_id
    loop
	    
	       raise info 'sql1%',v_layer.layer_name;

        -- Build dynamic SQL for geometry filter
        
	    IF p_geom != '' THEN
            IF lower(v_layer.layer_name) = 'building' THEN
                sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization)
                        SELECT a.system_id, ST_Centroid(m.sp_geometry), a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization
                        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
                        INNER JOIN polygon_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.LAYER_NAME || ''')
                        WHERE ST_Intersects(m.sp_geometry, ST_GeomFromText(''POLYGON((' || p_geom || '))'', 4326))
                        UNION
                        SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization
                        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
                        INNER JOIN point_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.LAYER_NAME || ''')
                        WHERE ST_Intersects(m.sp_geometry, ST_GeomFromText(''POLYGON((' || p_geom || '))'', 4326))';
            ELSE
                sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization)
                        SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization
                        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
                        LEFT JOIN ' || v_layer.GEOM_TYPE || '_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.layer_name || ''')
                        WHERE 1=1';
            END IF;
        ELSE
            -- No geometry filter
            IF lower(v_layer.layer_name) = 'building' THEN
                sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization)
                        SELECT a.system_id, ST_ASTEXT(m.sp_geometry), a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), a.building_name, a.utilization
                        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
                        INNER JOIN point_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.LAYER_NAME || ''')
                        UNION
                        SELECT a.system_id, ST_ASTEXT(ST_Centroid(m.sp_geometry)), a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), a.building_name, a.utilization
                        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
                        INNER JOIN polygon_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.LAYER_NAME || ''')';
            ELSE
                sql := 'INSERT INTO temp_geojson_features(system_id, geom, network_id, entity_title, entity_name, utilization)
                        SELECT a.system_id, m.sp_geometry, a.network_id, upper(''' || v_layer.LAYER_TITLE || '''), upper(''' || v_layer.LAYER_NAME || '''), a.utilization
                        FROM ' || v_layer.REPORT_VIEW_NAME || ' a
                        LEFT JOIN ' || v_layer.GEOM_TYPE || '_master m ON a.system_id = m.system_id AND upper(m.entity_type) = upper(''' || v_layer.layer_name || ''')
                        WHERE 1=1';
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
$function$;

----------------------------------------------------------------------------------------------------------------

