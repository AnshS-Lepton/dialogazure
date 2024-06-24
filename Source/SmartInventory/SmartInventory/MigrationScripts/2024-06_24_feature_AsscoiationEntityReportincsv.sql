CREATE OR REPLACE FUNCTION public.fn_get_association_report_summary_view_csv(
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
v_column_name text;

Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';
--select value into v_csv_delimeter  from global_settings where key ilike 'CsvDelimiter';
v_csv_delimeter:='^';
SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
IF ((p_geom) != '') THEN

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

v_column_name:= 'entity_network_id^entity_type^associated_network_id^associated_entity_type^associated_on^associated_by';
SELECT STRING_AGG('replace('''||v_column_name||''','||'''='''||','||'''''''='''||') ::character varying'||' as  DISPLAY_NAME ', ',') INTO S_LAYER_COLUMNS;

sql:='SELECT 0 as S_No,STRING_AGG(( '''||v_column_name ||'''  )|| '''||v_csv_delimeter||''','''') 

 union all ';
-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
--FETCH RECORD BASED ON SELECTED GEOMETRY.
sql:= sql || 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No,
    REPLACE(COALESCE(entity_network_id::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(entity_type::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(associated_network_id::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(associated_entity_type::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(associated_on::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(associated_by::text, ''''), E''\n'', '''') 
from (select ASS.system_id,ASS.entity_network_id,ASS.entity_type,
ASS.associated_network_id,ASS.associated_entity_type,ASS.associated_on,ASS.associated_by from '||S_REPORT_VIEW_NAME||' a
inner join temp_entity_inside_geom m on a.system_id=m.system_id
and upper(m.entity_type)=upper('''||s_layer_name||''') where 1=1 ';

ELSE

-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= sql || 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, 
      REPLACE(COALESCE(entity_network_id::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(entity_type::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(associated_network_id::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(associated_entity_type::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(associated_on::text, ''''), E''\n'', '''') || ''^'' || 
    REPLACE(COALESCE(associated_by::text, ''''), E''\n'', '''') 
from (select ASS.system_id,ASS.entity_network_id,ASS.entity_type,
ASS.associated_network_id,ASS.associated_entity_type,ASS.associated_on,ASS.associated_by from '||S_REPORT_VIEW_NAME||' a ';


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

sql:= sql ||' inner join vw_associate_entity_info ASS on (ASS.system_id=a.system_id and upper(ass.entity_layer_name)=upper('''||s_layer_name||'''))
or (ASS.associated_system_id=a.system_id and upper(ass.associated_entity_layer_name)=upper('''||s_layer_name||'''))';

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
RAISE INFO 'queryS_   %', sql;
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
RETURN QUERY EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;
$BODY$;