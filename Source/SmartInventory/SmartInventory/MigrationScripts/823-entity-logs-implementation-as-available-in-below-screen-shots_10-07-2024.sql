INSERT INTO public.module_master(
module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, type, is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES ( 'AUDIT Log Export Report', 'AUDIT Log Export Report', null, null, (select user_id from user_master where user_name='sa'), now(), (select user_id from user_master where user_name='sa'), now(), 'Web', true, 'AUDIT_HISTORY_EXRPT', 0, 1, false, null, '1');

INSERT INTO public.module_master(
module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, type, is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES ( 'Draw Rectangle', 'Draw Rectangle', null, null, (select user_id from user_master where user_name='sa'), now(), (select user_id from user_master where user_name='sa'), now(), 'Web', true, 'AUDIT-HISTORY-RECT', (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'), 0, false, null, null);

INSERT INTO public.module_master(
module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, type, is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES ( 'Draw Polygon', 'Draw Polygon', null, null, (select user_id from user_master where user_name='sa'), now(), (select user_id from user_master where user_name='sa'), now(), 'Web', true, 'AUDIT-HISTORY-POLY', (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'), 0, false, null, null);

INSERT INTO public.module_master(
module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, type, is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES ( 'Draw Circle', 'Draw Circle', null, null, (select user_id from user_master where user_name='sa'), now(), (select user_id from user_master where user_name='sa'), now(), 'Web', true, 'AUDIT-HISTORY-CIR', (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'), 0, false, null, null);

INSERT INTO public.module_master(
module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, type, is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES ( 'Audit Log', 'Audit Log', null, null, (select user_id from user_master where user_name='sa'), now(), (select user_id from user_master where user_name='sa'), now(), 'Web', true, 'AUDIT-HISTORY-LOG', (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'), 0, false, null, null);


INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT-HISTORY-LOG'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT-HISTORY-LOG'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT-HISTORY-RECT'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT-HISTORY-RECT'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT-HISTORY-POLY'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT-HISTORY-POLY'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT-HISTORY-CIR'));

INSERT INTO public.role_module_mapping(
role_id, module_id)
VALUES ( (select role_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT-HISTORY-CIR'));




INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());

INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());


INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT-HISTORY-RECT'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());

INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT-HISTORY-RECT'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());


INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT-HISTORY-POLY'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());

INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT-HISTORY-POLY'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());


INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT-HISTORY-CIR'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());

INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT-HISTORY-CIR'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());


INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='sa'), (select id from module_master where module_abbr ='AUDIT-HISTORY-LOG'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());

INSERT INTO public.user_module_mapping(
user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES ( (select user_id from user_master where user_name='admin'), (select id from module_master where module_abbr ='AUDIT-HISTORY-LOG'),(select user_id from user_master where user_name='sa'),now(),(select user_id from user_master where user_name='sa'),now());




INSERT INTO public.file_type_master(
file_type, file_extension, file_display_name, module_id, module_abbr, is_active, created_by, created_on, modified_by, modified_on)
VALUES ('EXCEL', '.xls,.xlsx', 'Summary(Excel)', (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'), 'AUDIT_HISTORY_EXRPT', true, (select user_id from user_master where user_name='sa'), now(), (select user_id from user_master where user_name='sa'), now());

INSERT INTO public.file_type_master(
file_type, file_extension, file_display_name, module_id, module_abbr, is_active, created_by, created_on, modified_by, modified_on)
VALUES ('ALLEXCEL', '.xls,.xlsx', 'All Data(Excel)', (select id from module_master where module_abbr ='AUDIT_HISTORY_EXRPT'), 'AUDIT_HISTORY_EXRPT', true, (select user_id from user_master where user_name='sa'), now(), (select user_id from user_master where user_name='sa'), now());


-------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_audit_log_report_allexcel(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, double precision, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_audit_log_report_allexcel(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, double precision, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_audit_log_report_allexcel(
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
s_table_name character varying;
v_geometry geometry;
v_system_id integer;

Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

select st_geomfromtext('POLYGON(('||p_geom||'))',4326) into v_geometry;

IF(UPPER(S_GEOM_TYPE) = UPPER('point')) THEN
select system_id into v_system_id from point_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('POLYGON')) THEN
select system_id into v_system_id from Polygon_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('circle')) THEN
select system_id into v_system_id from circle_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('Line')) THEN
select system_id into v_system_id from Line_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

-- raise info'v_system_id =%',v_system_id;
s_table_name := 'audit_att_details_' || lower(p_layer_name);

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

IF (coalesce(P_SORTCOLNAME,'')!='') THEN
IF EXISTS (select 1 from information_schema.columns where upper(table_name) = upper(S_REPORT_VIEW_NAME) and
upper(column_name) = upper(P_SORTCOLNAME) and upper(data_type) in('CHARACTER VARYING','TEXT')) THEN
LowerStart:='LOWER(';
else
LowerStart:='(';
end if;
LowerEnd:=')';
END IF;

if(p_pageno <>0)
then
SELECT STRING_AGG(COLUMN_NAME,',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
else
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)='REPORT' AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
end if;
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)='REPORT' ORDER BY st.COLUMN_SEQUENCE) A;

IF ((p_geom) != '') THEN
sql:= 'SELECT user_name as USER_NAME, CASE WHEN action = ''U'' THEN ''UPDATE'' ELSE ''CREATE'' END AS LOG_ACTION,
	 log_table_name as LOG_TABLE_NAME, category as NE_TYPE, a.system_id as SYSTEM_ID, a.network_id as NE_ID,
	 colname as LOG_COLUMN_NAME, oldvalue as COL_OLD_VAL, newvalue COL_NEW_VAL, modified_on as LOG_DATE
from fn_get_audit_log_details('''||s_table_name||''', '''||v_system_id||''') a
inner join temp_entity_inside_geom m on a.system_id::integer=m.system_id	
where 1=1 ';
ELSE
-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= 'SELECT user_name as USER_NAME, CASE WHEN action = ''U'' THEN ''UPDATE'' ELSE ''CREATE'' END AS LOG_ACTION,
	 log_table_name as LOG_TABLE_NAME, category as NE_TYPE, a.system_id as SYSTEM_ID, a.network_id as NE_ID,
	 colname as LOG_COLUMN_NAME, oldvalue as COL_OLD_VAL, newvalue COL_NEW_VAL, modified_on as LOG_DATE
from fn_get_audit_log_details('''||s_table_name||''', '''||v_system_id||''') a' ;

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
sql:= sql ||' AND upper(a.network_status:: TEXT) in('''||p_networkStatues||''')';
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
sql := sql ||' AND cast(a.created_by as integer) in ('||p_userids||')';
else
P_PARENTUSERS := Replace(P_PARENTUSERS,'0,','');
sql := sql ||' AND cast(a.created_by as integer) in (select * from fn_get_report_mapped_users('''||p_parentusers||'''))';
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
IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN
sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''')';
else IF ((p_ownership_type) != '') then
sql:= sql ||'';
END IF;
end if;

IF ((p_thirdparty_vendor_ids) != '' and upper(S_LAYER_COLUMNS_VAL) like '%THIRD_PARTY_VENDOR_ID%') THEN
sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''') AND a.third_party_vendor_id in('''||p_thirdparty_vendor_ids||''')';
else IF ((p_thirdparty_vendor_ids) != '') then
sql:= sql ||'';
end if;
end if;

-- raise info'sql =%',sql;
--sql:= sql ||' order by splited_by desc' ;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;
$BODY$;

ALTER FUNCTION public.fn_get_audit_log_report_allexcel(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, double precision, character varying)
    OWNER TO postgres;

-----------------------------------------------------------------------------------

-- FUNCTION: public.fn_get_audit_log_details(text, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_audit_log_details(text, integer);

CREATE OR REPLACE FUNCTION public.fn_get_audit_log_details(
	dynamic_table_name text,
	dynamic_system_id integer)
    RETURNS TABLE(user_name text, action text, log_table_name text, category text, system_id text, network_id text, colname text, oldvalue text, newvalue text, modified_on text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    v_column_name text; 
    query text := '';
    query_part text;
BEGIN
    CREATE TEMP TABLE temp_comparisons (
        user_name text,
        action text,
        log_table_name TEXT,
        category TEXT,
        system_id text,
        network_id TEXT,
        colname TEXT,
        oldvalue TEXT,
        newvalue TEXT,
        modified_on TEXT
    ) ON COMMIT DROP;    
    
    FOR v_column_name IN 
        SELECT column_name 
        FROM information_schema.columns 
        WHERE table_name = dynamic_table_name 
        AND table_schema = 'public' 
        -- AND column_name NOT IN ('modified_on', 'modified_by', 'created_by', 'created_on')
    LOOP        
        query_part := '
            INSERT INTO temp_comparisons (user_name, action, log_table_name, category, system_id,network_id, colname, oldvalue, newvalue, modified_on)
            SELECT
                (SELECT user_name FROM user_master WHERE user_id = t1.created_by::integer) AS user_name,
                COALESCE(t1.action, '''') AS action,
                ''' || dynamic_table_name || ''' AS log_table_name,
                COALESCE(t1.category, '''') AS category,
                t1.system_id::text AS system_id,
                COALESCE(t1.network_id, '''') AS network_id,
                ''' || v_column_name || ''' AS colname, 
                COALESCE(t1."' || v_column_name || '"::text, '''') AS oldvalue,
                COALESCE(t2."' || v_column_name || '"::text, '''') AS newvalue,		        
                TO_CHAR(t1.modified_on, ''DD-MM-YYYY HH24:MI'') AS modified_on
            FROM 
                (SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS row_num 
                 FROM ' || dynamic_table_name || ' WHERE system_id = ' || dynamic_system_id || ' AND action != ''I'') t1
            FULL OUTER JOIN 
                (SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS row_num 
                 FROM ' || dynamic_table_name || ' WHERE system_id = ' || dynamic_system_id || ' AND action != ''I'') t2 
            ON t1.row_num = t2.row_num - 1 
            WHERE 
                (COALESCE(t1."' || v_column_name || '"::text, '''') <> '' OR COALESCE(t2."' || v_column_name || '"::text, '''') <> '')
                AND (t1."' || v_column_name || '" IS NULL OR t2."' || v_column_name || '" IS NULL OR t1."' || v_column_name || '" <> t2."' || v_column_name || '");';
        
        query := query || query_part || ' ';
    END LOOP;

    EXECUTE query;
    RETURN QUERY SELECT * FROM temp_comparisons tc WHERE tc.system_id IS NOT NULL AND tc.oldvalue <> ''; -- AND tc.newvalue <> '';
END 
$BODY$;

ALTER FUNCTION public.fn_get_audit_log_details(text, integer)
    OWNER TO postgres;

