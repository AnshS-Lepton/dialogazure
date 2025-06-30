
-----------------------------------6254---------------------------------------------------------

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='SubArea'), 'Image_upload', 'Depth verification', true, 5, now(), 5, now(), 'Depth verification', NULL, 'Image_upload', true, true, 0);

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='SubArea'), 'Image_upload', 'Entry PIT after PR', true, 5, now(), 5, now(), 'Entry PIT after PR', NULL, 'Image_upload', true, true, 0);

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='SubArea'), 'Image_upload', 'Entry pit depth verification', true, 5, now(), 5, now(), 'Entry pit depth verification', NULL, 'Image_upload', true, true, 0);

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='SubArea'), 'Image_upload', 'Exit PIT after PR', true, 5, now(), 5, now(), 'Exit PIT after PR', NULL, 'Image_upload', true, true, 0);

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='SubArea'), 'Image_upload', 'Exit pit depth verification', true, 5, now(), 5, now(), 'Exit pit depth verification', NULL, 'Image_upload', true, true, 0);

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='SubArea'), 'Image_upload', 'For HDD', true, 5, now(), 5, now(), 'For HDD', NULL, 'Image_upload', true, true, 0);

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='SubArea'), 'Image_upload', 'For MT OPEN', true, 5, now(), 5, now(), 'For MT OPEN', NULL, 'Image_upload', true, true, 0);

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='SubArea'), 'Image_upload', 'PR proof', true, 5, now(), 5, now(), 'PR proof', NULL, 'Image_upload', true, true, 0);

-------------------------------------------------------6244-----------------------------------------------------
update layer_action_mapping set is_active=false  where layer_id=(select layer_id from layer_details where layer_name='PEP') and action_name='EntityRoomView';

------------------------------------------6223----------------------------------------
update global_settings set value=0 where key='IsTrenchCustomerRequired';

-------------------------------------------#6138--------------------------------------
update legend_details set icon_path='Pole9.png' where layer_id=(select layer_id from layer_details where layer_name='Pole') and sub_layer ='9 M Light Pole';
update legend_details set icon_path='Pole8.png' where layer_id=(select layer_id from layer_details where layer_name='Pole') and sub_layer ='8 M Light Pole';
update legend_details set icon_path='Pole6.7.png' where layer_id=(select layer_id from layer_details where layer_name='Pole') and sub_layer ='6.7 M Light Pole';
update legend_details set icon_path='Pole5.6.png' where layer_id=(select layer_id from layer_details where layer_name='Pole') and sub_layer ='5.6 M Light Pole';
update legend_details set icon_path='Pole6.7.png' where layer_id=(select layer_id from layer_details where layer_name='Pole') and sub_layer ='7 M Light Pole';
update legend_details set icon_path='PoleFTTX.png' where layer_id=(select layer_id from layer_details where layer_name='Pole') and sub_layer ='FTTX';
update legend_details set icon_path='PoleLECO.png' where layer_id=(select layer_id from layer_details where layer_name='Pole') and sub_layer ='LECO';

-----------------------------------------------------6255--------------------------------

DROP FUNCTION IF EXISTS public.fn_get_export_report_summary(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, boolean, character varying, character varying, double precision, character varying);

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
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)	
	union 
	
	select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)
	union 
	
	select system_id,entity_type from point_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)';

if(coalesce(p_layerids::character varying,'')='') or exists (select 1 from layer_details ld where layer_id in (select unnest(string_to_array(p_layerids::character varying, ','))::integer) and upper(layer_name)='CABLE')
then
query_value_geom := '
select system_id,entity_type from Polygon_master m where  ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)	
	union 
	
	select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)
	union 
	
	select system_id,entity_type from point_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)

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

sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in(''' || p_networkStatues || ''')';

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

ALTER FUNCTION public.fn_get_export_report_summary(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, boolean, character varying, character varying, double precision, character varying)
    OWNER TO postgres;


	---------------------------------------------6148----------------------------------------------

 DROP FUNCTION IF EXISTS public.get_site_project_details(text);

CREATE OR REPLACE FUNCTION public.get_site_project_details(
	p_site_id text)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT row_to_json(t)
    FROM (
        SELECT 
		sp.id,sp.project_id, sp.site_id,sp.site_name,sp.site_owner,sp.project_category, sp.cable_plan_cores,sp.maximum_cost,
 sp.comment,sp.location_address,sp.ds_cmc_area,sp.priority,pod.destination_site_id,pod.destination_port_type,pod.no_of_cores
        FROM site_project_details sp left join att_details_pod pod on pod.site_id=sp.site_id
        WHERE sp.site_id::TEXT = p_site_id
    ) t;
END;
$BODY$;

ALTER FUNCTION public.get_site_project_details(text)
    OWNER TO postgres;