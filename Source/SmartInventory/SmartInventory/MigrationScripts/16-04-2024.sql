CREATE OR REPLACE FUNCTION public.fn_get_route_info(
	p_province_id character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN

RETURN QUERY SELECT ROW_TO_JSON(ROW) FROM (
		SELECT CBL.SYSTEM_ID AS CABLE_ID,CBL.NETWORK_ID AS ENTITY_NETWORK_ID,CBL.NETWORK_ID||' ('||CBL.ROUTE_ID||'-'||CBL.ROUTE_NAME||')' as Route_Id,false IS_ASSOCIATED
		FROM LINE_MASTER LM 
		INNER JOIN ATT_DETAILS_CABLE CBL on LM.ENTITY_TYPE='Cable' AND CBL.SYSTEM_ID=LM.SYSTEM_ID
		INNER JOIN ASSOCIATE_ROUTE_INFO ASS ON ASS.CABLE_ID=CBL.SYSTEM_ID
	) ROW;
END
$BODY$;



CREATE OR REPLACE FUNCTION public.fn_get_route_info(
	p_province_id character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN

RETURN QUERY SELECT ROW_TO_JSON(ROW) FROM (
		SELECT CBL.SYSTEM_ID AS CABLE_ID,CBL.NETWORK_ID AS ENTITY_NETWORK_ID,CBL.NETWORK_ID||' ('||CBL.ROUTE_ID||'-'||CBL.ROUTE_NAME||')' as Route_Id,false IS_ASSOCIATED
		FROM LINE_MASTER LM 
		INNER JOIN ATT_DETAILS_CABLE CBL on LM.ENTITY_TYPE='Cable' AND CBL.SYSTEM_ID=LM.SYSTEM_ID
		INNER JOIN ASSOCIATE_ROUTE_INFO ASS ON ASS.CABLE_ID=CBL.SYSTEM_ID
	) ROW;
END
$BODY$;



CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary(
	p_regionids character varying,
	p_provinceids character varying,
	p_networkstatues character varying,
	p_parentusers character varying,
	p_userids character varying,
	p_layerids character varying,
	p_projectcodes character varying,
	p_planningcodes character varying,
	p_workordercodes character varying,
	p_purposecodes character varying,
	p_durationbasedon character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_geom character varying,
	p_userid integer,
	p_roleid integer,
	p_is_all_provience_assigned boolean,
	p_ownership_type character varying,
	p_thirdparty_vendor_ids character varying,
	p_radious double precision,
	p_route character varying)
    RETURNS TABLE(entity_id integer, entity_title character varying, entity_name character varying, planned_count integer, as_built_count integer, dormant_count integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


 

Declare v_arow record;

sql TEXT;
s_layer_columns_val text;
columnquery TEXT;
s_TempSql_columns_val text;
query_value_geom text;
query_value_summary text;
Begin

SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'P', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'A', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'D', 'gi') into p_networkstatues;

-- Create temp table tempExportReportSummary
-- (
--     entity_id integer,
--     entity_title character varying,
--     entity_name character varying,
--     planned_count integer,
--     as_built_count integer,
--     dormant_count integer
-- )on commit drop;

IF ((p_geom) != '')
THEN
if(p_radious>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
end if;               



query_value_geom ='select system_id,entity_type from Polygon_master m where  ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)	
	union 
	
	select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)
	union 
	
	select system_id,entity_type from point_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)';

if(coalesce(p_layerids::character varying,'')='') or exists (select 1 from layer_details ld where layer_id in (select unnest(string_to_array(p_layerids::character varying, ','))::integer) and upper(layer_name)='CABLE')
then
query_value_geom := '
select system_id,entity_type from Polygon_master m where  ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)	
	union 
	
	select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)
	union 
	
	select system_id,entity_type from point_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)

union 
     select cbl.system_id as system_id,  ''''  as entity_type   from att_details_cable cbl
	inner join point_master pm on  ST_Intersects(pm.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))  
    and pm.system_id=cbl.structure_id and upper(pm.entity_type)=upper(''structure'') 
    and cbl.cable_type=''ISP'' ' ;
end if;

raise info 'queryyyy: %', query_value_geom;

 

end if;

 

for v_arow in select distinct L.LAYER_ID,L.LAYER_TITLE, L.REPORT_VIEW_NAME, L.GEOM_TYPE,L.layer_name,L.layer_table from LAYER_DETAILS L

inner join role_permission_entity pe on pe.layer_id = L.Layer_Id

where   pe.role_id = p_roleid and pe.viewonly = true and L.isvisible = true and L.is_report_enable = true 
and case when coalesce(p_layerids,'')!='' then  (L.layer_id) in (select unnest(string_to_array(p_layerids, ','))::integer)
else true end order by L.layer_name

  loop

sql := '';

 

-- FETCH ALL REPORT COLUMNS FROM LAYER COLUMN SETTINGS TABLE

SELECT STRING_AGG(COLUMN_NAME, ',') INTO S_LAYER_COLUMNS_VAL FROM (

SELECT  COLUMN_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=v_arow.LAYER_ID  AND UPPER(SETTING_TYPE)='REPORT'

ORDER BY COLUMN_SEQUENCE) A;

 

 

IF(upper(S_LAYER_COLUMNS_VAL) like '%NETWORK_STATUS%')

THEN

-- SHOW NETWORK STATUS COUNT SEPERATLY --

columnquery :='cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,

cast('''||v_arow.layer_name||'''as text)entityName,coalesce(sum(case when a.network_status=''P'' then 1 else 0 end),0)

as planned_count,coalesce(sum(case when a.network_status=''A'' then 1 else 0 end),0) as asbuilt_count,

coalesce(sum(case when a.network_status=''D'' then 1 else 0 end),0) as dorment_count';

ELSE

-- IF NETWORK STATUS COLUMN IS NOT DEFINED FOR LAYER THEN SHOW THE ALL COUNT INTO AS-BUILT.

columnquery :='cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,

cast('''||v_arow.layer_name||'''as text)entityName,CAST(0 AS INTEGER) as planned_count,coalesce(COUNT(a.SYSTEM_ID),0)

as asbuilt_count,CAST(0 AS INTEGER)as dorment_count';

END IF;

 

    

--DYNAMIC QUERY TO FETCH ENTITY SUMMARY

IF ((p_geom) != '') THEN

 

--FETCH RECORD BASED ON SELECTED GEOMETRY.

sql:= 'select '||columnquery||' from '|| v_arow.layer_table||' a inner join ( ' || query_value_geom || ' ) m

on a.system_id=m.system_id and upper(m.entity_type)=upper('''||v_arow.layer_name||''')

JOIN (select user_id AS created_by_id, * from user_master) um ON a.created_by = um.user_id ';

else

 

-- FETCH RECORDS BASED ON SELECTED FILTERS.

sql:= 'select '||columnquery||' from '|| v_arow.layer_table||' a';

 

if exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)

then

sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)

then

sql:= sql ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';

else

sql:= sql ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';

end if;

 

END IF;

IF ((p_route) != '') THEN

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.system_id and ass.cable_id in('||p_route||')';

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

-- PARENT/CHILD USER FILTER

if not exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)

and not exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)

and p_roleid > 1

then

if(p_parentusers != '' and p_parentusers != '0') THEN

 

if(p_userids != '')THEN

sql := sql ||' AND cast('||' a.created_by' ||' as integer) in ('||p_userids||')';

else

P_PARENTUSERS := Replace(P_PARENTUSERS,'0,','');
sql := sql ||' AND cast('|| ' a.created_by'  ||' as integer) in (select * from fn_get_report_mapped_users('''||p_parentusers||'''))';

END IF;

 

END IF;

END IF;

 

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

-- DURATION FILTER 

 

IF(P_fromDate != '' and P_toDate != '') THEN

sql:= sql ||' AND a.'||p_durationbasedon||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and

a.'||p_durationbasedon||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

 

-- OWNER SHIP FILTER

IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN

sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''')';

else IF ((p_ownership_type) != '') then

sql:= sql ||' AND  0 = 1';

END IF;

end if;

  raise notice 'sql: % ', sql;  

IF ((p_thirdparty_vendor_ids) != '' and upper(S_LAYER_COLUMNS_VAL) like '%THIRD_PARTY_VENDOR_ID%') THEN

sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''') AND a.third_party_vendor_id in('||p_thirdparty_vendor_ids||')';

else IF ((p_thirdparty_vendor_ids) != '') then

sql:= sql ||' AND  0 = 1';

end if;

end if;
s_TempSql_columns_val:='';
 s_TempSql_columns_val:= 'Select entityid, entitytitle:: character varying ,entityName:: character varying, CAST(sum(planned_count) AS INTEGER) AS planned_count ,
 CAST(sum(asbuilt_count) AS INTEGER) AS asbuilt_count ,CAST(sum(dorment_count) AS INTEGER) AS dorment_count from( '||sql||')result 
 where (planned_count + asbuilt_count + dorment_count) > 0 group by entityid,entitytitle,entityName';

    raise notice 'checking query: % ', s_TempSql_columns_val;  
    RETURN QUERY execute s_TempSql_columns_val;
end loop;

 

End;

 

$BODY$;


CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary(
	p_regionids character varying,
	p_provinceids character varying,
	p_networkstatues character varying,
	p_parentusers character varying,
	p_userids character varying,
	p_layerids character varying,
	p_projectcodes character varying,
	p_planningcodes character varying,
	p_workordercodes character varying,
	p_purposecodes character varying,
	p_durationbasedon character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_geom character varying,
	p_userid integer,
	p_roleid integer,
	p_is_all_provience_assigned boolean,
	p_ownership_type character varying,
	p_thirdparty_vendor_ids character varying,
	p_radious double precision,
	p_route character varying)
    RETURNS TABLE(entity_id integer, entity_title character varying, entity_name character varying, planned_count integer, as_built_count integer, dormant_count integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


 

Declare v_arow record;

sql TEXT;
s_layer_columns_val text;
columnquery TEXT;
s_TempSql_columns_val text;
query_value_geom text;
query_value_summary text;
Begin

SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'P', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'A', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'D', 'gi') into p_networkstatues;

-- Create temp table tempExportReportSummary
-- (
--     entity_id integer,
--     entity_title character varying,
--     entity_name character varying,
--     planned_count integer,
--     as_built_count integer,
--     dormant_count integer
-- )on commit drop;

IF ((p_geom) != '')
THEN
if(p_radious>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
end if;               



query_value_geom ='select system_id,entity_type from Polygon_master m where  ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)	
	union 
	
	select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)
	union 
	
	select system_id,entity_type from point_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)';

if(coalesce(p_layerids::character varying,'')='') or exists (select 1 from layer_details ld where layer_id in (select unnest(string_to_array(p_layerids::character varying, ','))::integer) and upper(layer_name)='CABLE')
then
query_value_geom := '
select system_id,entity_type from Polygon_master m where  ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)	
	union 
	
	select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)
	union 
	
	select system_id,entity_type from point_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))::integer) else true end)

union 
     select cbl.system_id as system_id,  ''''  as entity_type   from att_details_cable cbl
	inner join point_master pm on  ST_Intersects(pm.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))  
    and pm.system_id=cbl.structure_id and upper(pm.entity_type)=upper(''structure'') 
    and cbl.cable_type=''ISP'' ' ;
end if;

raise info 'queryyyy: %', query_value_geom;

 

end if;

 

for v_arow in select distinct L.LAYER_ID,L.LAYER_TITLE, L.REPORT_VIEW_NAME, L.GEOM_TYPE,L.layer_name,L.layer_table from LAYER_DETAILS L

inner join role_permission_entity pe on pe.layer_id = L.Layer_Id

where   pe.role_id = p_roleid and pe.viewonly = true and L.isvisible = true and L.is_report_enable = true 
and case when coalesce(p_layerids,'')!='' then  (L.layer_id) in (select unnest(string_to_array(p_layerids, ','))::integer)
else true end order by L.layer_name

  loop

sql := '';

 

-- FETCH ALL REPORT COLUMNS FROM LAYER COLUMN SETTINGS TABLE

SELECT STRING_AGG(COLUMN_NAME, ',') INTO S_LAYER_COLUMNS_VAL FROM (

SELECT  COLUMN_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=v_arow.LAYER_ID  AND UPPER(SETTING_TYPE)='REPORT'

ORDER BY COLUMN_SEQUENCE) A;

 

 

IF(upper(S_LAYER_COLUMNS_VAL) like '%NETWORK_STATUS%')

THEN

-- SHOW NETWORK STATUS COUNT SEPERATLY --

columnquery :='cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,

cast('''||v_arow.layer_name||'''as text)entityName,coalesce(sum(case when a.network_status=''P'' then 1 else 0 end),0)

as planned_count,coalesce(sum(case when a.network_status=''A'' then 1 else 0 end),0) as asbuilt_count,

coalesce(sum(case when a.network_status=''D'' then 1 else 0 end),0) as dorment_count';

ELSE

-- IF NETWORK STATUS COLUMN IS NOT DEFINED FOR LAYER THEN SHOW THE ALL COUNT INTO AS-BUILT.

columnquery :='cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,

cast('''||v_arow.layer_name||'''as text)entityName,CAST(0 AS INTEGER) as planned_count,coalesce(COUNT(a.SYSTEM_ID),0)

as asbuilt_count,CAST(0 AS INTEGER)as dorment_count';

END IF;

 

    

--DYNAMIC QUERY TO FETCH ENTITY SUMMARY

IF ((p_geom) != '') THEN

 

--FETCH RECORD BASED ON SELECTED GEOMETRY.

sql:= 'select '||columnquery||' from '|| v_arow.layer_table||' a inner join ( ' || query_value_geom || ' ) m

on a.system_id=m.system_id and upper(m.entity_type)=upper('''||v_arow.layer_name||''')

JOIN (select user_id AS created_by_id, * from user_master) um ON a.created_by = um.user_id ';

else

 

-- FETCH RECORDS BASED ON SELECTED FILTERS.

sql:= 'select '||columnquery||' from '|| v_arow.layer_table||' a';

 

if exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)

then

sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)

then

sql:= sql ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';

else

sql:= sql ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';

end if;

 

END IF;

IF ((p_route) != '') THEN

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.system_id and ass.cable_id in('||p_route||')';

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

-- PARENT/CHILD USER FILTER

if not exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)

and not exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)

and p_roleid > 1

then

if(p_parentusers != '' and p_parentusers != '0') THEN

 

if(p_userids != '')THEN

sql := sql ||' AND cast('||' a.created_by' ||' as integer) in ('||p_userids||')';

else

P_PARENTUSERS := Replace(P_PARENTUSERS,'0,','');
sql := sql ||' AND cast('|| ' a.created_by'  ||' as integer) in (select * from fn_get_report_mapped_users('''||p_parentusers||'''))';

END IF;

 

END IF;

END IF;

 

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

-- DURATION FILTER 

 

IF(P_fromDate != '' and P_toDate != '') THEN

sql:= sql ||' AND a.'||p_durationbasedon||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and

a.'||p_durationbasedon||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

 

-- OWNER SHIP FILTER

IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN

sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''')';

else IF ((p_ownership_type) != '') then

sql:= sql ||' AND  0 = 1';

END IF;

end if;

  raise notice 'sql: % ', sql;  

IF ((p_thirdparty_vendor_ids) != '' and upper(S_LAYER_COLUMNS_VAL) like '%THIRD_PARTY_VENDOR_ID%') THEN

sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''') AND a.third_party_vendor_id in('||p_thirdparty_vendor_ids||')';

else IF ((p_thirdparty_vendor_ids) != '') then

sql:= sql ||' AND  0 = 1';

end if;

end if;
s_TempSql_columns_val:='';
 s_TempSql_columns_val:= 'Select entityid, entitytitle:: character varying ,entityName:: character varying, CAST(sum(planned_count) AS INTEGER) AS planned_count ,
 CAST(sum(asbuilt_count) AS INTEGER) AS asbuilt_count ,CAST(sum(dorment_count) AS INTEGER) AS dorment_count from( '||sql||')result 
 where (planned_count + asbuilt_count + dorment_count) > 0 group by entityid,entitytitle,entityName';

    raise notice 'checking query: % ', s_TempSql_columns_val;  
    RETURN QUERY execute s_TempSql_columns_val;
end loop;

 

End;

 

$BODY$;




CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary_view(
	p_regionids character varying,
	p_provinceids character varying,--
	p_networkstatues character varying,
	p_parentusers character varying,
	p_userids character varying,
	p_layer_name character varying,
	p_projectcodes character varying,
	p_planningcodes character varying,
	p_workordercodes character varying,
	p_purposecodes character varying,
	p_durationbasedon character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_geom character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_advancefilter character varying,
	p_userid integer,
	p_roleid integer,
	p_ownership_type character varying,
	p_thirdparty_vendor_ids character varying,
	p_culturename character varying,
	p_radious double precision,
	p_route character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


Declare v_arow record;
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
s_layer_name character varying;
s_layer_columns text;
s_layer_columns_val text;

Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';

SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
IF ((p_geom) != '') THEN

--select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
if(p_radious>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10) into p_geom;
end if;

Create temp table temp_entity_inside_geom
(
system_id integer,
entity_type character varying
) on commit drop;

-- HERE WE ARE FETCHING ALL THE ENTITY WITHIN THE ARAE SELECTED BY USER INTO A TEMP TABLE.
perform(fn_get_export_report_entity_within_geom(S_LAYER_ID::character varying,p_geom));

end if;

RAISE INFO 'sortcol%', P_SORTCOLNAME;
-- MANAGE SORT COLUMN NAME
IF (coalesce(P_SORTCOLNAME,'')!='') THEN

-- CHECK THE DATA TYPE OF SORT COLUMN, IF TEXT OR CHARACTER VARYING THEN SORT WITH LOWER FUNCTION TO MAKE IT CASE INSENSITIVE
IF EXISTS (select 1 from information_schema.columns where upper(table_name) = upper(S_REPORT_VIEW_NAME) and
upper(column_name) = upper(P_SORTCOLNAME) and upper(data_type) in('CHARACTER VARYING','TEXT')) THEN
LowerStart:='LOWER(';
else
LowerStart:='(';
end if;
LowerEnd:=')';
END IF;

-- SELECT ALL ACTIVE FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
if(p_pageno <>0)-- p_pageno 0 denotes when we downloading report and we required columns with keys
then
SELECT STRING_AGG(COLUMN_NAME,',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
else
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)='REPORT' AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
end if;
-- SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)='REPORT' ORDER BY st.COLUMN_SEQUENCE) A;

RAISE INFO 'S_LAYER_COLUMNS---%', S_LAYER_COLUMNS;
RAISE INFO 'P_SORTCOLNAME%', P_SORTCOLNAME;

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
--FETCH RECORD BASED ON SELECTED GEOMETRY.
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a

inner join temp_entity_inside_geom m on a.system_id=m.system_id
and upper(m.entity_type)=upper('''||s_layer_name||''') where 1=1 ';

RAISE INFO '123_%', sql;

ELSE
-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a ';

if exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';
else 
sql:= sql ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';
end if;
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

IF ((p_route) != '') THEN

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.system_id and ass.cable_id in('||p_route||')';

END IF;


-- PARENT/CHILD USER FILTER
if not exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
and not exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
and p_roleid>1
then 
IF(P_PARENTUSERS != '' AND P_PARENTUSERS != '0') 
THEN
IF(P_USERIDS != '')
THEN
sql := sql ||' AND cast(a.created_by_id as integer) in ('||p_userids||')';
else
P_PARENTUSERS := Replace(P_PARENTUSERS,'0,','');
-- sql := sql ||' AND cast(a.created_by_id as integer) in (select user_id from user_master where
-- manager_id in ('||p_parentusers||') or user_id in ('||p_parentusers||'))';
-- sql := sql ||' AND cast(a.created_by_id as integer) in (select userid from fn_get_report_mapped_users('||p_parentusers||'))';
--if(p_roleid <>1) 
--THEN
sql := sql ||' AND cast(a.created_by_id as integer) in (select * from fn_get_report_mapped_users('''||p_parentusers||'''))';
--ELSE
-- sql := sql ||' AND cast(a.created_by_id as integer) in (select user_id from user_master where manager_id
-- in ('||p_parentusers||') or user_id in ('||p_parentusers||'))';
--END IF;
END IF;
END IF;
END IF;

-- PROJECT CODE FILTER
IF ((p_projectcodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PROJECT_ID%') THEN
sql:= sql ||' AND a.project_id IN ('||p_projectcodes||')';
else IF ((p_projectcodes) != '') then
sql:= sql ||' AND 0 = 1';
END IF;
END IF;
-- PLANNING CODE FILTER
IF ((p_planningcodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PLANNING_ID%') THEN
sql:= sql ||' AND a.planning_id IN ('||p_planningcodes||')';
END IF;
-- WORKORDER CODE FILTER
IF ((p_workordercodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%WORKORDER_ID%') THEN
sql:= sql ||' AND a.workorder_id IN ('||p_workordercodes||')';
END IF;
-- PURPOSE CODE FILTER
IF ((p_purposecodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PURPOSE_ID%') THEN
sql:= sql ||' AND a.purpose_id IN ('||p_purposecodes||')';
END IF;

-- DURATION FILTER
IF(P_fromDate != '' and P_toDate != '') THEN
sql:= sql ||' AND a.'||p_durationbasedon||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_durationbasedon||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';
END IF;

-- ADVANCE FILTER SELCTED BY USER
IF ((p_advancefilter) != '') THEN
sql:= sql ||''|| p_advancefilter||'';
END IF;

-- OWNER SHIP FILTER
--IF ((p_ownership) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP%') THEN
IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN
sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''')';
else IF ((p_ownership_type) != '') then
sql:= sql ||' AND 0 = 1';
END IF;
end if;

IF ((p_thirdparty_vendor_ids) != '' and upper(S_LAYER_COLUMNS_VAL) like '%THIRD_PARTY_VENDOR_ID%') THEN
sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''') AND a.third_party_vendor_id in('||p_thirdparty_vendor_ids||')';
else IF ((p_thirdparty_vendor_ids) != '') then
sql:= sql ||' AND 0 = 1';
end if;
end if;

sql:= sql ||' )X';
RAISE INFO 'queryS_%', sql;
-- GET TOTAL RECORD COUNT
EXECUTE 'SELECT COUNT(1) FROM ('||sql||') as a' INTO TotalRecords;
-- RAISE EXCEPTION 'Calling result(%)', sql;
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
IF((P_PAGENO) <> 0) THEN
StartSNo:= P_PAGERECORD * (P_PAGENO - 1 ) + 1;
EndSNo:= P_PAGENO * P_PAGERECORD;
sql:= 'SELECT '||TotalRecords||' as totalRecords, *
FROM (' || sql || ' ) T
WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo;

ELSE
sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';
END IF;

RAISE INFO 'QUERY %', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;
$BODY$;




CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary_view_csv(
	p_regionids character varying,
	p_provinceids character varying,
	p_networkstatues character varying,
	p_parentusers character varying,
	p_userids character varying,
	p_layer_name character varying,
	p_projectcodes character varying,
	p_planningcodes character varying,
	p_workordercodes character varying,
	p_purposecodes character varying,
	p_durationbasedon character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_geom character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_advancefilter character varying,
	p_userid integer,
	p_roleid integer,
	p_ownership_type character varying,
	p_thirdparty_vendor_ids character varying,
	p_culturename character varying,
	p_radious double precision,
	p_route character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$



Declare v_arow record;
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
s_layer_name character varying;
s_layer_columns text;
s_layer_columns_val text;
s_layer_csv_columns text;
v_csv_delimeter text;
Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';
select value into v_csv_delimeter  from global_settings where key ilike 'CsvDelimiter';
SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
IF ((p_geom) != '') THEN

--select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
if(p_radious>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10) into p_geom;
end if;

Create temp table temp_entity_inside_geom
(
system_id integer,
entity_type character varying
) on commit drop;

-- HERE WE ARE FETCHING ALL THE ENTITY WITHIN THE ARAE SELECTED BY USER INTO A TEMP TABLE.
perform(fn_get_export_report_entity_within_geom(S_LAYER_ID::character varying,p_geom));

end if;

RAISE INFO 'sortcol%', P_SORTCOLNAME;
-- MANAGE SORT COLUMN NAME
IF (coalesce(P_SORTCOLNAME,'')!='') THEN

-- CHECK THE DATA TYPE OF SORT COLUMN, IF TEXT OR CHARACTER VARYING THEN SORT WITH LOWER FUNCTION TO MAKE IT CASE INSENSITIVE
IF EXISTS (select 1 from information_schema.columns where upper(table_name) = upper(S_REPORT_VIEW_NAME) and
upper(column_name) = upper(P_SORTCOLNAME) and upper(data_type) in('CHARACTER VARYING','TEXT')) THEN
LowerStart:='LOWER(';
else
LowerStart:='(';
end if;
LowerEnd:=')';
END IF;

-- SELECT ALL ACTIVE FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
if(p_pageno <>0)-- p_pageno 0 denotes when we downloading report and we required columns with keys
then
SELECT STRING_AGG(COLUMN_NAME,',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
else
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)='REPORT' AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
end if;
-- SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)='REPORT' ORDER BY st.COLUMN_SEQUENCE) A;

SELECT STRING_AGG('REPLACE(COALESCE('||COLUMN_NAME||'::text,''''),E''\n'','''')','||''|''||') INTO s_layer_csv_columns FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

RAISE INFO 'S_LAYER_COLUMNS---%', S_LAYER_COLUMNS;
RAISE INFO 'P_SORTCOLNAME%', P_SORTCOLNAME;

sql:='SELECT 0 as S_No,STRING_AGG((case when coalesce(res_field_key,'''') ='''' then DISPLAY_NAME else res_field_key end)||'''||v_csv_delimeter||''','''') 
FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(''en'')
WHERE st.LAYER_ID='||S_LAYER_ID||' AND UPPER(st.SETTING_TYPE)=''REPORT'' AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A union all ';

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
--FETCH RECORD BASED ON SELECTED GEOMETRY.
sql:= sql || 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||s_layer_csv_columns||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a

inner join temp_entity_inside_geom m on a.system_id=m.system_id
and upper(m.entity_type)=upper('''||s_layer_name||''') where 1=1 ';

RAISE INFO '123_%', sql;

ELSE

-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= sql || 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||s_layer_csv_columns||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a ';

if exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';
else 
sql:= sql ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';
end if;
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

IF ((p_route) != '') THEN

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.system_id and ass.cable_id in('||p_route||')';

END IF;



-- PARENT/CHILD USER FILTER
if not exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
and not exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
and p_roleid>1
then 
IF(P_PARENTUSERS != '' AND P_PARENTUSERS != '0') 
THEN
IF(P_USERIDS != '')
THEN
sql := sql ||' AND cast(a.created_by_id as integer) in ('||p_userids||')';
ELSE
-- sql := sql ||' AND cast(a.created_by_id as integer) in (select user_id from user_master where
-- manager_id in ('||p_parentusers||') or user_id in ('||p_parentusers||'))';
-- sql := sql ||' AND cast(a.created_by_id as integer) in (select userid from fn_get_report_mapped_users('||p_parentusers||'))';
--if(p_roleid <>1) 
--THEN
sql := sql ||' AND cast(a.created_by_id as integer) in (select * from fn_get_report_mapped_users('''||p_parentusers||'''))';
--ELSE
-- sql := sql ||' AND cast(a.created_by_id as integer) in (select user_id from user_master where manager_id
-- in ('||p_parentusers||') or user_id in ('||p_parentusers||'))';
--END IF;
END IF;
END IF;
END IF;

-- PROJECT CODE FILTER
IF ((p_projectcodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PROJECT_ID%') THEN
sql:= sql ||' AND a.project_id IN ('||p_projectcodes||')';
else IF ((p_projectcodes) != '') then
sql:= sql ||' AND 0 = 1';
END IF;
END IF;
-- PLANNING CODE FILTER
IF ((p_planningcodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PLANNING_ID%') THEN
sql:= sql ||' AND a.planning_id IN ('||p_planningcodes||')';
END IF;
-- WORKORDER CODE FILTER
IF ((p_workordercodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%WORKORDER_ID%') THEN
sql:= sql ||' AND a.workorder_id IN ('||p_workordercodes||')';
END IF;
-- PURPOSE CODE FILTER
IF ((p_purposecodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PURPOSE_ID%') THEN
sql:= sql ||' AND a.purpose_id IN ('||p_purposecodes||')';
END IF;

-- DURATION FILTER
IF(P_fromDate != '' and P_toDate != '') THEN
sql:= sql ||' AND a.'||p_durationbasedon||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_durationbasedon||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';
END IF;

-- ADVANCE FILTER SELCTED BY USER
IF ((p_advancefilter) != '') THEN
sql:= sql ||''|| p_advancefilter||'';
END IF;

-- OWNER SHIP FILTER
--IF ((p_ownership) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP%') THEN
IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN
sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''')';
else IF ((p_ownership_type) != '') then
sql:= sql ||' AND 0 = 1';
END IF;
end if;

IF ((p_thirdparty_vendor_ids) != '' and upper(S_LAYER_COLUMNS_VAL) like '%THIRD_PARTY_VENDOR_ID%') THEN
sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''') AND a.third_party_vendor_id in('||p_thirdparty_vendor_ids||')';
else IF ((p_thirdparty_vendor_ids) != '') then
sql:= sql ||' AND 0 = 1';
end if;
end if;

sql:= sql ||' )X';
RAISE INFO 'queryS_%', sql;
-- GET TOTAL RECORD COUNT
EXECUTE 'SELECT COUNT(1) FROM ('||sql||') as a' INTO TotalRecords;
-- RAISE EXCEPTION 'Calling result(%)', sql;
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
IF((P_PAGENO) <> 0) THEN
StartSNo:= P_PAGERECORD * (P_PAGENO - 1 ) + 1;
EndSNo:= P_PAGENO * P_PAGERECORD;
sql:= 'SELECT '||TotalRecords||' as totalRecords, *
FROM (' || sql || ' ) T
WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo;

ELSE
sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';
END IF;

RAISE INFO 'QUERY %', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;
$BODY$;