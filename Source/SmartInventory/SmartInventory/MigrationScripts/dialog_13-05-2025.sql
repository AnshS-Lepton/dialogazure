-- FUNCTION: public.fn_sr_get_export_report_data(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer)

-- DROP FUNCTION IF EXISTS public.fn_sr_get_export_report_data(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer);

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
                           'fiber_distance_to_nearest_site','cost_based_on_rate_card_lkr','is_site_imported') ORDER BY COLUMN_SEQUENCE ) A;

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
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported
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
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported
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
 RAISE INFO 'page%',P_PAGENO;
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords,* FROM (' || sql || ') T';                  
 END IF; 

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$;

ALTER FUNCTION public.fn_sr_get_export_report_data(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer)
    OWNER TO postgres;
