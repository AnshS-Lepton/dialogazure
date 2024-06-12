
CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary_view_additional(
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
column_count INTEGER;

Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';

SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, ADDITIONAL_REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME) and is_dynamic_control_enable = true  ;

select count(1) from layer_columns_settings  where layer_id=S_LAYER_ID and UPPER(SETTING_TYPE)=upper('OIReport') AND IS_ACTIVE=TRUE into column_count;

if(coalesce(S_REPORT_VIEW_NAME,'')!='' and column_count > 0) then

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
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)=upper('OIReport') AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
else
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)=upper('OIReport') AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
end if;
-- SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)=upper('OIReport') ORDER BY st.COLUMN_SEQUENCE) A;

RAISE INFO 'S_LAYER_COLUMNS---%', S_LAYER_COLUMNS;
RAISE INFO 'P_SORTCOLNAME%', P_SORTCOLNAME;

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
--FETCH RECORD BASED ON SELECTED GEOMETRY.
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'record_system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a

inner join temp_entity_inside_geom m on a.record_system_id:: integer =m.system_id
and upper(m.entity_type)=upper('''||s_layer_name||''') where 1=1 ';

RAISE INFO '123_%', sql;

ELSE
-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'record_system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a ';

if exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
then 
--sql:= sql ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';
else 
--sql:= sql ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';
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

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.record_system_id:: integer and ass.cable_id in('||p_route||')';

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

else 
sql := 'select 0 as Record_Count';
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

end if;

End;
$BODY$;





CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary_view_csv_additional(
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
column_count INTEGER;
Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';
select value into v_csv_delimeter  from global_settings where key ilike 'CsvDelimiter';
SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, ADDITIONAL_REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

select count(1) from layer_columns_settings  where layer_id=S_LAYER_ID and UPPER(SETTING_TYPE)=upper('OIReport') AND IS_ACTIVE=TRUE into column_count;

if(coalesce(S_REPORT_VIEW_NAME,'')!='' and column_count > 0) then

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
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)=upper('OIReport') AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
else
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)=upper('OIReport') AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
end if;
-- SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG('replace('''||COLUMN_NAME||'''::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)=upper('OIReport') ORDER BY st.COLUMN_SEQUENCE) A;

SELECT STRING_AGG('REPLACE(COALESCE('''||COLUMN_NAME||'''::text,''''),E''\n'','''')','||''|''||') INTO s_layer_csv_columns FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)=upper('OIReport') AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

RAISE INFO 'S_LAYER_COLUMNS---%', S_LAYER_COLUMNS;
RAISE INFO 'P_SORTCOLNAME%', P_SORTCOLNAME;

sql:='SELECT 0 as S_No,STRING_AGG((case when coalesce(res_field_key,'''') ='''' then DISPLAY_NAME else res_field_key end)||'''||v_csv_delimeter||''','''') 
FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(''en'')
WHERE st.LAYER_ID='||S_LAYER_ID||' AND UPPER(st.SETTING_TYPE)=''OIREPORT'' AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A union all ';

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
--FETCH RECORD BASED ON SELECTED GEOMETRY.
sql:= sql || 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'record_system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||s_layer_csv_columns||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a

inner join temp_entity_inside_geom m on a.record_system_id::integer = m.system_id
and upper(m.entity_type)=upper('''||s_layer_name||''') where 1=1 ';

RAISE INFO '123_%', sql;

ELSE

-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= sql || 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'record_system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||s_layer_csv_columns||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a ';

if exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
then 
sql:= sql ;-- ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';
else 
sql:= sql ;-- ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';
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

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.record_system_id::integer and ass.cable_id in('||p_route||')';

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

else 
sql := 'select 0 as Record_Count';
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
end if;

End;
$BODY$;




CREATE OR REPLACE FUNCTION public.fn_refresh_vw_report_oi(
	p_layer_name character varying)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

declare
vw_report_name character varying;
vw_history_name character varying;
vw_other_info_name character varying;
vw_other_info_audit_name character varying;
vw_report_definition text;
vw_audit_definition text;
att_table_name character varying;
audit_table_name character varying;
l_keys text;
from_position int;
before_from_query text;
old_label_part text;
new_label_part text;
first_oi_position int;
entity_type text;
layerID int;
order_by_position int;
vw_before_order_by text;
vw_after_part_with_order_by text;
first_dot_position int;
alias_value text;
vw_dependent_definition text;
dependent_view text;

begin
--UNCOMMENT ALL raise to DEBUG

select ld.report_view_name,ld.layer_table,ld.history_view_name,
ld.layer_name,ld.other_info_view,ld.other_info_view_audit,ld.layer_id,ld.audit_table_name 
into vw_report_name,att_table_name,vw_history_name,entity_type,vw_other_info_name,
vw_other_info_audit_name,layerID,audit_table_name from layer_details ld where upper(layer_name)=upper(p_layer_name);

--commment the below code we getting the audit table from the layer_details table
--audit_table_name:='audit_'||att_table_name||'';
--REPORT VIEW DEFINATION
select definition into vw_report_definition from pg_views where upper(viewname)=upper(vw_report_name);

--AUDIT VIEW DEFINATION
select definition into vw_audit_definition from pg_views where upper(viewname)=upper(vw_history_name);
select 'vw_jfp_'||entity_type into dependent_view;
if exists(select table_schema as schema_name,table_name as view_name from information_schema.views
          where upper(table_name) = upper(''||dependent_view||''))
Then
raise info'dependent_view:%',dependent_view;
select definition into vw_dependent_definition from pg_views where upper(viewname)=upper(dependent_view);
END IF;

-- raise info'dependent_view:%',dependent_view;
-- raise info'vw_report_name:%',vw_report_name;
-- raise info'att_table_name:%',att_table_name;
-- raise info'vw_report_defination:%',vw_report_definition;

-- raise info'vw_other_info_audit_name:%',vw_other_info_audit_name;
-- raise info'vw_history_name:%',vw_history_name;
-- raise info'audit_table_name:%',audit_table_name;
-- raise info'vw_audit_defination:%',vw_audit_definition;

-- raise info'layer_id :%',layerID;
----REGION REPORT VIEW
--raise notice'----====--START OF DYNAMIC REPORT VIEW CREATION--====----:%',now();
if exists(select table_schema as schema_name,table_name as view_name from information_schema.views where upper(table_name) = upper(''||dependent_view||''))
Then

execute 'drop view if exists '||dependent_view;
END IF;
raise info'vw_other_info_name:%',vw_other_info_name;
execute 'drop view if exists '||vw_other_info_name||' cascade';

execute 'drop view if exists '||vw_report_name;

--Comment the below code to get the json keys from attribute table
--execute 'select string_agg(distinct format(''other_info::json ->> %L as %I'',jkey, jkey), '', '')
     
--   from '||att_table_name||', json_object_keys(other_info::json) as t(jkey);' into l_keys;

--raise info'l_keys:%',l_keys;

--now get the json keys from dynamic_controls table
execute 'select string_agg(distinct format(''other_info::json ->> %L as %I'',jkey, jkey), '', '')
     
   from dynamic_controls, json_object_keys(other_info::json) as t(jkey) where entity_id ='||layerID||';' into l_keys;

--raise info'l_keys:%',l_keys;

--ONLY CREATE VIEW WHEN KEYS ARE PRESENT
if(l_keys is not null) then

   execute 'create view '||vw_other_info_name||' as select '||l_keys||' from '||att_table_name;
update layer_details set additional_report_view_name =  vw_other_info_name where layer_id = layerID;
---Set new_label_part without system_id
select STRING_AGG('oi.' || column_name,', ')
 FROM information_schema.columns WHERE upper(table_name) = upper(vw_other_info_name) AND upper(table_schema) = 'PUBLIC' AND upper(column_name)!='SYSTEM_ID' into new_label_part;

end if;
---creating dynamic report view
--raise info'actual view defination :%',vw_report_definition;

-- GET FROM POSITION  FROM VIEW DEFINATION
	SELECT   POSITION('FROM ' in upper(vw_report_definition)) into from_position;

	--raise info 'fromposition:%',from_position;

-- GET BEFORE FROM STRING..
	SELECT Substring(vw_report_definition,1,from_position-1) into before_from_query;

	--raise info 'beforeFromString:%',before_from_query;

	SELECT position('oi.' in before_from_query)-1 into first_oi_position;
	--raise info 'oiposition before :%',first_oi_position;

if (first_oi_position>0)then

--IF oi IS PRESENT IN VIEW 

--GET OLD LABEL PART FROM VIEW DEFINATION
	select substring(vw_report_definition,first_oi_position,(from_position-1)-first_oi_position) into old_label_part;

	--raise info 'length:%',LENGTH(old_label_part);
	--raise info 'queryA:%',vw_report_definition;
	--raise info 'old_label_part:%',old_label_part;
	--raise info 'new_label_part:%',new_label_part;

		--IF THE KEYS ARE NOT PRESENT THEN REPLACE IT WITH THE EMPTY STRING ELSE REPLACE WITH NEW LABEL
		if(l_keys is null) then
			--raise info '---===---VIEW RESET---===---';
			select substring(vw_report_definition,first_oi_position-5,(from_position-1)-first_oi_position+2) into old_label_part;
			raise info 'To reset view old_label_part:%',old_label_part;
			select replace (vw_report_definition,old_label_part,'') into vw_report_definition;
		else
			--raise info 'new_label_part:%',new_label_part;
		-- REPLACE OLD LABEL PART WITH NEW LABEL PART
			select replace (vw_report_definition,old_label_part,new_label_part) into vw_report_definition;
		end if;
else
--IF oi IS NOT PRESENT IN VIEW 
before_from_query:=before_from_query||', ' ||new_label_part;
--raise info 'old_label_part:%',old_label_part;
--raise info 'before_from_query:%',before_from_query;

select substring(vw_report_definition,(from_position-1),position(';' in vw_report_definition)+1) into vw_report_definition;

vw_report_definition:=before_from_query||' '||vw_report_definition;

end if;

	--raise info 'finalQuery:%',vw_report_definition;

--raise info 'Get Alias value';
--Get first dot position from view
SELECT   POSITION(upper('.') in upper(vw_report_definition)) into first_dot_position;
--raise info 'first_dot_position:%',first_dot_position;

--Get the substring for alias
select substring(vw_report_definition,1,first_dot_position) into alias_value;
--raise info 'alias_value:%',alias_value;

--Final alias value by triming select from string
select ltrim(alias_value,'SELECT ') into alias_value;
--raise info 'alias_value:%',alias_value;

--new_label_part:=' LEFT JOIN '||vw_other_info_name||' oi on oi.record_system_id='||alias_value||'system_id::text;';
--raise info 'new_label_part:%',new_label_part;

----Get the position of order by if present
SELECT   POSITION(upper('order by ') in upper(vw_report_definition)) into order_by_position;
--raise info 'order_by_position:%',order_by_position;

if(select position(upper(vw_other_info_name) in upper(vw_report_definition))=0)then

		if(order_by_position>0) then --order by is present in the view
			--RAISE INFO 'Adding other_info join condition before order by if present';
		
			----Get the view part before order by
			SELECT Substring(vw_report_definition,1,order_by_position-1) into vw_before_order_by;
			--raise info 'vw_before_order_by:%',vw_before_order_by;
		
			----Get the view after part with order by
			SELECT Substring(vw_report_definition,order_by_position) into vw_after_part_with_order_by;
			--raise info 'vw_after_part_with_order_by:%',vw_after_part_with_order_by;
		
		
			--concat the new_label_part
			vw_report_definition:=vw_before_order_by||' '||RTRIM(new_label_part, ';')||' '||vw_after_part_with_order_by;
		
		else

			--raise info 'replace ; with join :%',now();
			select replace (vw_report_definition,';',new_label_part) into vw_report_definition;
		end if;
	end if;

--REPLACE THE JOIN PART WITH EMPTY STRING IF KEYS ARE NOT PRESENT
if(l_keys is null) then
		--raise info '---===---VIEW RESET---===---';
		new_label_part:=' LEFT JOIN '||vw_other_info_name||' oi ON ((oi.record_system_id = ('||alias_value||'system_id)::text))';
		--raise info 'To reset view new_label_part:%',new_label_part;
		select replace (vw_report_definition,new_label_part,'') into vw_report_definition;
end if;

	--raise info 'finalQuery:%',vw_report_definition;

  execute 'create view '||vw_report_name||' as '||vw_report_definition;
  if exists(select table_schema as schema_name,table_name as view_name from information_schema.views where upper(table_name) = upper(''||dependent_view||''))
Then
  
  execute 'create view '||dependent_view||' as '||vw_dependent_definition;
END IF;
--raise notice'----====--END OF DYNAMIC REPORT VIEW CREATION--====----:%',now();

--raise notice'----====--START OF DYNAMIC AUDIT VIEW CREATION--====----:%',now();

execute 'drop view if exists '||vw_other_info_audit_name||' cascade';

execute 'drop view if exists '||vw_history_name;

--Comment the below code to get the json keys from attribute table
--execute 'select string_agg(distinct format(''other_info::json ->> %L as %I'',jkey, jkey), '', '')
     
--   from '||att_table_name||', json_object_keys(other_info::json) as t(jkey);' into l_keys;

--l_keys:=l_keys||',audit_id';
--raise info'l_keys:%',l_keys;

--Reset variable values
new_label_part:=null;
--raise info'new_label_part:%',new_label_part;
old_label_part:=null;
--raise info'old_label_part:%',old_label_part;

--now get the json keys from dynamic_controls table
execute 'select string_agg(distinct format(''other_info::json ->> %L as %I'',jkey, jkey), '', '')
     
   from dynamic_controls, json_object_keys(other_info::json) as t(jkey) where entity_id ='||layerID||';' into l_keys;

l_keys:=l_keys||',audit_id';
--raise info'l_keys:%',l_keys;

--ONLY CREATE VIEW WHEN KEYS ARE PRESENT
if(l_keys is not null) then

   execute 'create view '||vw_other_info_audit_name||' as select '||l_keys||' from '||audit_table_name||' where ('||audit_table_name||'.other_info ->>''record_system_id''<>''0'' and '||audit_table_name||'.other_info ->>''record_system_id''<>'''') or '||audit_table_name||'.other_info ->>''record_system_id'' is null';

---Set new_label_part without system_id
select STRING_AGG('oi.' || column_name,', ')
 FROM information_schema.columns WHERE upper(table_name) = upper(vw_other_info_audit_name) AND upper(table_schema) = 'PUBLIC' AND upper(column_name) not in ('SYSTEM_ID','AUDIT_ID') into new_label_part;

end if;

---creating dynamic audit view
--raise info'actual view defination :%',vw_audit_definition;

-- GET FROM POSITION  FROM VIEW DEFINATION
	SELECT   POSITION('FROM ' in upper(vw_audit_definition)) into from_position;

	--raise info 'fromposition:%',from_position;

-- GET BEFORE FROM STRING..
	SELECT Substring(vw_audit_definition,1,from_position-1) into before_from_query;

	--raise info 'beforeFromString:%',before_from_query;

	SELECT position('oi.' in before_from_query)-1 into first_oi_position;
	--raise info 'oiposition before :%',first_oi_position;

if (first_oi_position>0)then

--IF oi IS PRESENT IN VIEW 
--GET OLD LABEL PART FROM VIEW DEFINATION
	select substring(vw_audit_definition,first_oi_position,(from_position-1)-first_oi_position) into old_label_part;

	--raise info 'length:%',LENGTH(old_label_part);
	--raise info 'queryA:%',vw_audit_definition;
	--raise info 'old_label_part:%',old_label_part;
	--raise info 'new_label_part:%',new_label_part;

			--IF THE KEYS ARE NOT PRESENT THEN REPLACE IT WITH THE EMPTY STRING ELSE REPLACE WITH NEW LABEL
		if(l_keys is null) then
			--raise info '---===---VIEW RESET---===---';
			select substring(vw_audit_definition,first_oi_position-5,(from_position-1)-first_oi_position+2) into old_label_part;
			--raise info 'To reset view old_label_part:%',old_label_part;
			select replace (vw_audit_definition,old_label_part,'') into vw_audit_definition;
		else
			--raise info 'new_label_part:%',new_label_part;
		-- REPLACE OLD LABEL PART WITH NEW LABEL PART
			select replace (vw_audit_definition,old_label_part,new_label_part) into vw_audit_definition;
		end if;

else
--IF oi IS NOT PRESENT IN VIEW 
before_from_query:=before_from_query||', ' ||new_label_part;
--raise info 'old_label_part:%',old_label_part;
--raise info 'before_from_query:%',before_from_query;

select substring(vw_audit_definition,(from_position-1),position(';' in vw_audit_definition)+1) into vw_audit_definition;

vw_audit_definition:=before_from_query||' '||vw_audit_definition;

end if;

	--raise info 'finalQuery:%',vw_audit_definition;

--raise info 'Get Alias value';
--Get first dot position from view
SELECT   POSITION(upper('.') in upper(vw_audit_definition)) into first_dot_position;
--raise info 'first_dot_position:%',first_dot_position;

--Get the substring for alias
select substring(vw_audit_definition,1,first_dot_position) into alias_value;
--raise info 'alias_value:%',alias_value;

--Final alias value by triming select from string
select ltrim(alias_value,'SELECT ') into alias_value;
--raise info 'alias_value:%',alias_value;

new_label_part:=' JOIN '||vw_other_info_audit_name||' oi on oi.audit_id='||alias_value||'audit_id;';
--raise info 'new_label_part:%',new_label_part;

	
----Get the position of order by if present
SELECT   POSITION(upper('order by ') in upper(vw_audit_definition)) into order_by_position;
--raise info 'order_by_position:%',order_by_position;

if(select position(upper(vw_other_info_audit_name) in upper(vw_audit_definition))=0)then

		if(order_by_position>0) then --order by is present in the view
			--RAISE INFO 'Adding other_info join condition before order by if present';
		
			----Get the view part before order by
			SELECT Substring(vw_audit_definition,1,order_by_position-1) into vw_before_order_by;
			--raise info 'vw_before_order_by:%',vw_before_order_by;
		
			----Get the view after part with order by
			SELECT Substring(vw_audit_definition,order_by_position) into vw_after_part_with_order_by;
			--raise info 'vw_after_part_with_order_by:%',vw_after_part_with_order_by;
		
		
			--concat the new_label_part
			vw_audit_definition:=vw_before_order_by||' '||RTRIM(new_label_part, ';')||' '||vw_after_part_with_order_by;
		
		else

			--raise info 'replace ; with join :%',now();
			select replace (vw_audit_definition,';',new_label_part) into vw_audit_definition;
		end if;
	end if;

--REPLACE THE JOIN PART WITH EMPTY STRING IF KEYS ARE NOT PRESENT
if(l_keys is null) then
		--raise info '---===---VIEW RESET---===---';
		new_label_part:=' JOIN '||vw_other_info_audit_name||' oi ON ((oi.audit_id = '||alias_value||'audit_id))';
		--raise info 'To reset view new_label_part:%',new_label_part;
		select replace (vw_audit_definition,new_label_part,'') into vw_audit_definition;
end if;

--raise info 'finalQuery:%',vw_audit_definition;

  execute 'create view '||vw_history_name||' as '||vw_audit_definition;

--raise notice'----====--END OF DYNAMIC AUDIT VIEW CREATION--====----:%',now();

end;

$BODY$;