CREATE OR REPLACE FUNCTION public.fn_sr_get_export_report_data(
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
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
   s_report_view_name character varying;
   s_geom_type character varying;
s_layer_id integer; 
s_layer_columns text;
TotalAppliedRecords integer; 
TotalRejectedRecords integer;
BEGIN


LowerStart:='';
LowerEnd:='';
WhereCondition:='';


-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..	
-- SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS FROM (
-- SELECT  COLUMN_NAME,res_field_key,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

--SELECT (SPECIFIC) ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG(COLUMN_NAME ||' as "'|| CASE WHEN COALESCE(res_field_key, '') = '' THEN DISPLAY_NAME ELSE res_field_key END || '"', ',') INTO S_LAYER_COLUMNS 
FROM (SELECT COLUMN_NAME, res_field_key, DISPLAY_NAME FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID = S_LAYER_ID AND UPPER(SETTING_TYPE) = 'REPORT' AND IS_ACTIVE = TRUE
       AND COLUMN_NAME IN ('system_id','site_id','site_name','latitude','longitude',
                           'fiber_oh_distance_to_network','fiber_ug_distance_to_network','total_fiber_distance',
                           'fiber_distance_to_nearest_site','cost_based_on_rate_card_lkr') ORDER BY COLUMN_SEQUENCE ) A;

-- DYNAMIC QUERY
IF ((p_geom) != '') 
THEN
	if(p_radius>0)
	then
		select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10)  into p_geom;
	end if;
RAISE INFO '------------------------------------S_LAYER_COLUMNS%', S_LAYER_COLUMNS;


	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id, '||S_LAYER_COLUMNS||' from (select 
     a.system_id,a.site_id,a.site_name,a.latitude,a.longitude, 
     a.fiber_oh_distance_to_network,a.fiber_ug_distance_to_network,a.total_fiber_distance,
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr
   from '||S_REPORT_VIEW_NAME||' a inner join '||s_geom_type||'_master m
	on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
	and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
	RAISE INFO '------------------------------------sql%', sql;
ELSE
     -- a.system_id, a.design_id,a.network_id,a.port_type

	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id, '||S_LAYER_COLUMNS||' from (select 
     a.system_id, a.site_id,a.site_name,a.latitude,a.longitude, 
     a.fiber_oh_distance_to_network,a.fiber_ug_distance_to_network,a.total_fiber_distance,
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr
    from '||S_REPORT_VIEW_NAME||' a 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
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
--sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(''%'||trim(P_searchbytext)||'%'')';

	if(substring(P_searchbytext from 1 for 1)='"' and  substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
	then
		sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
	else
		sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
	end if;

END IF;

IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND a.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

sql:= sql ||' )X';
RAISE INFO '%', sql;
-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;


--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, FROM (' || sql || ') T';                  
 END IF; 


RAISE INFO 'QUERY %', sql;
	
RETURN QUERY EXECUTE 'select row_to_json(row) from ('||sql||') row';


END ;
$BODY$;


CREATE OR REPLACE FUNCTION public.fn_get_user_details(
	p_searchby character varying,
	p_searchtext character varying,
	p_isactive boolean,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_application_access character varying,
	p_role_id integer,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 

BEGIN
	if (p_searchby !~* '^[a-zA-Z0-9\s_]*$' or p_searchtext !~* '^[a-zA-Z0-9\s_.\-]*$' or p_sortcolname !~* '^[a-zA-Z0-9\s_]*$' or
		p_sorttype !~* '^[a-zA-Z0-9\s_]*$' or p_application_access !~* '^[a-zA-Z0-9\s_]*$') then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
	begin
LowerStart:='';
LowerEnd:='';
/*IF (pg_typeof(''||P_SORTCOLNAME||'')= character varying) THEN
	LowerStart:='';
	LowerEnd:='';
ELSE*/

 IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
   	IF EXISTS (select 1 from information_schema.columns where upper(table_name) = upper('vw_att_user_details') and 
	upper(column_name) = upper(P_SORTCOLNAME) and upper(data_type) in('CHARACTER VARYING','TEXT')) THEN
		LowerStart:='LOWER(';
	else
		LowerStart:='(';
	end if;
	LowerEnd:=')';	
END IF;
-- END IF;
RAISE INFO '%', sql;

 if(p_role_id != 1 and p_role_id != 0  )then

create temp table user_details_temp as
WITH RECURSIVE users AS (
	select udd.user_id, user_name,
    password,name,user_email,role_id,role_name,mobile_number,application_access,is_active,created_by, created_on, modified_by, modified_on, created_by_text, modified_by_text,udd.manager_id,reporting_manager,user_type
     FROM user_manager_mapping umm 
    inner join vw_att_user_details  udd on umm.user_id = udd.user_id
    WHERE  role_id !=1 and is_active=true and umm.manager_id = p_user_id	
    UNION ALL
	SELECT ud.user_id, ud.user_name,
	 ud.password,ud.name,ud.user_email,ud.role_id,ud.role_name,ud.mobile_number,ud.application_access,ud.is_active,ud.created_by,ud.created_on,ud.modified_by, ud.modified_on, 
	 ud.created_by_text, ud.modified_by_text,ud.manager_id,ud.reporting_manager,ud.user_type
	 FROM vw_att_user_details ud
	 inner join user_manager_mapping umm on umm.user_id = ud.user_id
	 INNER JOIN users s ON s.user_id = umm.manager_id
    ) SELECT * FROM users ;
else
create temp table user_details_temp as
select ud.user_id, ud.user_name,
	 ud.password,ud.name,ud.user_email,ud.role_id,ud.role_name,ud.mobile_number,ud.application_access,ud.is_active,ud.created_by,
	 ud.created_on,ud.modified_by, ud.modified_on, 
	 ud.created_by_text, ud.modified_by_text,ud.manager_id,ud.reporting_manager,ud.user_type from vw_att_user_details ud;
end if;
-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| LowerStart ||   CASE WHEN (P_SORTCOLNAME) = '' THEN 'user_id' ELSE P_SORTCOLNAME END ||LowerEnd || ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No,user_id, user_name,
 password,name,user_email,role_id,role_name,mobile_number,application_access,is_active,created_by, created_on, modified_by, modified_on, created_by_text, modified_by_text,manager_id,reporting_manager,user_type FROM user_details_temp WHERE  role_id!=1 AND is_active ='||p_isactive||'';

IF ((p_searchtext) != '' and (p_searchby) != '') THEN
--sql:= sql ||' AND lower('||p_searchby||') LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
sql:= sql ||' AND lower('|| quote_ident($1) ||') LIKE lower(''%'|| $2 ||'%'')';
END IF;

IF ((p_application_access) != '') THEN
--sql:= sql ||' AND application_access in ('''||p_application_access ||''',''BOTH'') ';
sql := sql || ' AND UPPER(application_access) IN (UPPER(''' || $8 || '''))';
--sql:= sql ||' AND application_access in ('''||$8 ||''',''BOTH'') ';
END IF;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' order by created_on desc) T ';                  
 END IF; 

RAISE INFO '%', sql;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

drop table user_details_temp;
end;
end if;
END ;

$BODY$;


