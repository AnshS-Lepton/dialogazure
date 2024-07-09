INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ((select role_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='SPLIT-LOG'));


-------------------------------------------------------------
-- FUNCTION: public.fn_get_split_report_summary(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, boolean, character varying, character varying, double precision, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_split_report_summary(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, boolean, character varying, character varying, double precision, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_split_report_summary(
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
v_column_name text;
p_layer_name text;

Begin

-- SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'P', 'gi') into p_networkstatues;
-- SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'A', 'gi') into p_networkstatues;
-- SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'D', 'gi') into p_networkstatues;

p_layer_name:='select layer_name from layer_details where layer_id='||p_layerids||'';

IF ((p_geom) != '')
THEN
if(p_radious>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
end if;               

query_value_geom =' select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)';

if(coalesce(p_layerids::character varying,'')='') or exists (select 1 from layer_details ld where layer_id in (select unnest(string_to_array(p_layerids::character varying, ','))::integer) and upper(layer_name)='CABLE')
then
query_value_geom := 'select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)
	and system_id in (select system_id from att_details_cable where splited_by is not null
					   union select system_id from att_details_duct where splited_by is not null
					  union select system_id from att_details_trench where splited_by is not null)
union 
     select cbl.system_id as system_id,  ''''  as entity_type from att_details_cable cbl
	inner join point_master pm on  ST_Intersects(pm.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))  
    and pm.system_id=cbl.structure_id and upper(pm.entity_type)=upper(''structure'') 
    and cbl.cable_type=''ISP'' ' ;
end if;
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

-- v_column_name:= 'parent_'||lower(p_layer_name)||'_netwok_id,child_'||lower(p_layer_name)||'_netwok_id,splitting_netwok_id,splitting_entity_Type,split_by,split_on';
-- SELECT STRING_AGG('replace('''||v_column_name||''','||'''='''||','||'''''''='''||') ::character varying'||' as  DISPLAY_NAME ', ',') INTO S_LAYER_COLUMNS_VAL;

IF(upper(S_LAYER_COLUMNS_VAL) like '%NETWORK_STATUS%')
THEN
-- SHOW NETWORK STATUS COUNT SEPERATLY --
columnquery :='cast('''||v_arow.layer_id||''' as integer)entityid,
cast('''||v_arow.layer_title||'''as text) entitytitle,
cast('''||v_arow.layer_name||'''as text) entityName, 
coalesce(sum(case when a.network_status=''P'' then 1 else 0 end),0) as planned_count, 
coalesce(sum(case when a.network_status=''A'' then 1 else 0 end),0) as asbuilt_count,
coalesce(sum(case when a.network_status=''D'' then 1 else 0 end),0) as dorment_count';
ELSE
-- IF NETWORK STATUS COLUMN IS NOT DEFINED FOR LAYER THEN SHOW THE ALL COUNT INTO AS-BUILT.
columnquery :='cast('''||v_arow.layer_id||''' as integer) entityid, 
cast('''||v_arow.layer_title||'''as text) entitytitle,
cast('''||v_arow.layer_name||'''as text) entityName,
CAST(0 AS INTEGER) as planned_count,
coalesce(COUNT(a.SYSTEM_ID),0) as asbuilt_count,
CAST(0 AS INTEGER)as dorment_count';
END IF;

IF ((p_geom) != '') THEN
sql:= 'select '||columnquery||' from '|| v_arow.layer_table||' a inner join ( ' || query_value_geom || ' ) m
on a.system_id=m.system_id and upper(m.entity_type)=upper('''||v_arow.layer_name||''')
JOIN (select user_id AS created_by_id, * from user_master) um ON a.created_by = um.user_id ';
else
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
sql:= sql ||' AND upper(a.network_status:: TEXT) in('||p_networkStatues||')';
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

-- RAISE INFO 'p_ownership_type%', p_ownership_type;

-- OWNER SHIP FILTER

IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN
 sql := sql || ' AND UPPER(a.ownership_type::TEXT) IN ('''||UPPER(p_ownership_type)||''')';
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

ALTER FUNCTION public.fn_get_split_report_summary(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, boolean, character varying, character varying, double precision, character varying)
    OWNER TO postgres;
-----------------------------------------------------------------------------------------


-- FUNCTION: public.fn_split_report_get_entity(integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_split_report_get_entity(integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_split_report_get_entity(
	p_roleid integer,
	p_purpose character varying)
    RETURNS TABLE(layer_id integer, layer_name character varying, layer_title character varying, is_utilization_enabled boolean, is_barcode_enabled boolean, geom_type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
sql text;
query text;
    begin
	
	if (p_purpose !~* '^[a-zA-Z0-9\s_]*$') then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
BEGIN
query := 'Select distinct ld.layer_id,ld.layer_name,ld.layer_title,ld.is_utilization_enabled,is_barcode_enabled,ld.geom_type 
from layer_details ld 
	inner join role_permission_entity pl on pl.layer_id = ld.layer_id 
	where viewonly = true and role_id = '||p_roleid||'  
	and (case when upper('''||quote_ident(lower($2))||''') = ''ENTITY'' then ld.is_report_enable end) = true
	and ld.isvisible = true and ld.geom_type = ''Line'' AND ld.layer_name NOT IN (''Microduct'', ''Conduit'', ''MicrowaveLink'')		
	Order by ld.layer_name';
RAISE INFO '%',query;	
return query
	execute query;
	end;
	end if;
END ;
$BODY$;

ALTER FUNCTION public.fn_split_report_get_entity(integer, character varying)
    OWNER TO postgres;
