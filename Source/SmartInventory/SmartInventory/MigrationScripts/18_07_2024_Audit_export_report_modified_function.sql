-- DROP FUNCTION public.fn_get_audit_log_report_allexcel(varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, int4, int4, varchar, varchar, varchar, int4, int4, varchar, varchar, varchar, float8, varchar);

CREATE OR REPLACE FUNCTION public.fn_get_audit_log_report_allexcel(p_regionids character varying, p_provinceids character varying, p_networkstatues character varying, p_parentusers character varying, p_userids character varying, p_layer_name character varying, p_projectcodes character varying, p_planningcodes character varying, p_workordercodes character varying, p_purposecodes character varying, p_durationbasedon character varying, p_fromdate character varying, p_todate character varying, p_geom character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_advancefilter character varying, p_userid integer, p_roleid integer, p_ownership_type character varying, p_thirdparty_vendor_ids character varying, p_culturename character varying, p_radious double precision, p_route character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

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
v_audit_table_name  character varying;
Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME,audit_table_name, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,v_audit_table_name,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
raise info'S_REPORT_VIEW_NAME =%',S_REPORT_VIEW_NAME;

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
   
   Create temp table temp_entity_inside_geom
(
system_id integer,
entity_type character varying
) on commit drop;

   Create temp table temp_all_entity
(
entity_id integer,
entity_type character varying
) on commit drop;



raise info'p_layer_name =%',p_layer_name;


IF ((p_geom) != '') 
then
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
if(p_radious>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10) into p_geom;
end if;



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
from fn_get_audit_log_details('''||S_REPORT_VIEW_NAME||''', '''||v_system_id||''') a
inner join temp_entity_inside_geom m on a.system_id::integer=m.system_id	
where 1=1 ';
ELSE

sql:= 'SELECT  a.system_id from '||S_REPORT_VIEW_NAME||' a where 1=1' ;


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
end if;

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

raise info'sql =%',sql;

raise info'step1';


execute 'insert into temp_all_entity(entity_id,entity_type) select system_id,'''||p_layer_name||''' from ('||sql||') t';

--for v_arow in select * from temp_all_entity limit 3
--loop	
	--raise info'sql =%', v_arow.system_id;
	perform(fn_get_audit_log_details(v_audit_table_name, 0,p_layer_name)) a;	
--select * from fn_get_audit_log_details('audit_att_details_cable', 277493,'cable');
--end loop;

--raise info' bbb ', (select count(1) from temp_comparisons);

RETURN QUERY
EXECUTE 'select row_to_json(row) from (select * from temp_comparisons) row';

End;
$function$
;

---------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_audit_log_details(dynamic_table_name text, dynamic_system_id integer,dynamic_category_name text)
 RETURNS void--TABLE(user_name text, action text, log_table_name text, category text, system_id text, network_id text, colname text, oldvalue text, newvalue text, modified_on text)
 LANGUAGE plpgsql
AS $function$
DECLARE 
    v_column_name text; 
    query text := '';
    query_part text;
BEGIN
    
    
    FOR v_column_name IN 
        SELECT column_name 
        FROM information_schema.columns 
        WHERE table_name = dynamic_table_name 
        AND table_schema = 'public' 
         AND column_name NOT IN('modified_on', 'modified_by', 'created_by', 'created_on','audit_id','system_id')
    LOOP        
        query_part := '
            INSERT INTO temp_comparisons (user_name, action, log_table_name, category, system_id,network_id, colname, oldvalue, newvalue, modified_on)
            SELECT
                (SELECT user_name FROM user_master WHERE user_id = t1.created_by::integer) AS user_name,
                COALESCE(t1.action, '''') AS action,
                ''' || dynamic_table_name || ''' AS log_table_name,
                /*COALESCE(t1.category, '''') AS category*/ '''||dynamic_category_name||''' as category ,
                t1.system_id::text AS system_id,
                COALESCE(t1.network_id, '''') AS network_id,
                ''' || v_column_name || ''' AS colname, 
                COALESCE(t1."' || v_column_name || '"::text, '''') AS oldvalue,
                COALESCE(t2."' || v_column_name || '"::text, '''') AS newvalue,		        
                TO_CHAR(t1.modified_on, ''DD-MM-YYYY HH24:MI'') AS modified_on
            FROM 
                (SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS row_num 
                 FROM ' || dynamic_table_name || ' a
inner join temp_all_entity b on a.system_id=b.entity_id WHERE  action != ''I'') t1
            FULL OUTER JOIN 
                (SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS row_num 
                 FROM ' || dynamic_table_name || ' a inner join temp_all_entity b on a.system_id=b.entity_id WHERE  action != ''I'') t2 
            ON t1.row_num = t2.row_num - 1 
            WHERE 
                ( t1."' || v_column_name || '" <> t2."' || v_column_name || '");';
        
        query := query || query_part || ' ';
    END LOOP;

    EXECUTE query;
    --RETURN QUERY SELECT * FROM temp_comparisons tc WHERE tc.system_id IS NOT NULL AND tc.oldvalue <> ''; -- AND tc.newvalue <> '';
END 
$function$
;