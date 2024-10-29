-- FUNCTION: public.fn_get_utilization_report_kml_shape(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_utilization_report_kml_shape(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_get_utilization_report_kml_shape(
	p_regionids character varying,
	p_provinceids character varying,
	p_networkstatues character varying,
	p_layer_name character varying,
	p_projectcode character varying,
	p_planningcode character varying,
	p_workordercode character varying,
	p_purposecode character varying,
	p_geom character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_advancefilter character varying,
	p_filetype character varying,
	p_userid integer,
	p_roleid integer)
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
LowerStart  character varying;
LowerEnd  character varying;
TotalRecords integer; 
WhereCondition character varying;
s_report_view_name character varying;
s_geom_type character varying;
s_layer_id integer; 
s_layer_columns text; 
s_entity_title character varying; 
s_layer_columns_val text; 

Begin
LowerStart:='';
LowerEnd:='';

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_title INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_entity_title FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE COLUMNS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER // FOR SHAPE FILE - ALL ACTIVE COLUMNS, FOR KML ONLY CONFIGURED COLUMN	
SELECT STRING_AGG(COLUMN_NAME||' as "'||DISPLAY_NAME||'"', ',') INTO S_LAYER_COLUMNS FROM (
Select * from (
SELECT  COLUMN_NAME,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE case when p_filetype ='SHAPE' THEN 1=1 ELSE  is_kml_column_required = true END and LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE)cl
union 
  SELECT  'Utilization','Utilization' 
) A;

--SELECT * FROM S_LAYER_COLUMNS; -- Not required--

-- SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG(COLUMN_NAME||' as "'||DISPLAY_NAME||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' ORDER BY COLUMN_SEQUENCE) A;
  
-- IF NULL THEN SET IT BLANK TO AVOID ERROR
S_LAYER_COLUMNS := coalesce(S_LAYER_COLUMNS,'');

-- DYNAMIC QUERY
IF ((p_geom) != '') 
THEN
	create temp table temp_entity_inside_geom(
	system_id integer,
	sp_geometry geometry,
	entity_type character varying
	)on commit drop;

	IF(lower(p_layer_name) = lower('Building')) 
	THEN
		execute 'insert into temp_entity_inside_geom(system_id,entity_type,sp_geometry) 
		select system_id ,entity_type,ST_Centroid(m.sp_geometry) from POLYGON_MASTER m where upper(m.entity_type)=upper('''||P_LAYER_NAME||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))
		UNION
		select system_id ,entity_type,m.sp_geometry from POINT_MASTER m where upper(m.entity_type)=upper('''||P_LAYER_NAME||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))';
	ELSE	
		execute 'insert into temp_entity_inside_geom(system_id,entity_type,sp_geometry) 
		select system_id ,entity_type,m.sp_geometry from '||s_geom_type||'_master m
		where upper(m.entity_type)=upper('''||P_LAYER_NAME||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))';	
	END IF;

	-- ISP CABLE WILL NOT BE THE PART OF KML AND SHAPE FILE AS THEIR IS NO GEOM FOR ISP CABLE.
	sql:= 'select  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,'''||s_geom_type||''' as 
 	geom_type,'''||s_entity_title||''' as entity_title,''''as entity_name,ST_ASTEXT(sp_geometry) as 
 	geom, '||S_LAYER_COLUMNS||' from(select m.sp_geometry,a.* from '||S_REPORT_VIEW_NAME||' a inner join temp_entity_inside_geom m
 	on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')  
 	inner join user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' and 1=1 ';
ELSE
	IF(lower(p_layer_name) = lower('Building')) 
	THEN	
	       sql:= 'SELECT * FROM ( SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,geom_type,entity_title,entity_name,
	       geom, '||S_LAYER_COLUMNS||' from (
	         select a.*,ST_ASTEXT(sp_geometry) as geom,'''||s_geom_type||''' as geom_type,'''||s_entity_title||''' as   
	         entity_title,  a.building_name as entity_name from '||S_REPORT_VIEW_NAME||' a inner join point_master m
	         on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
	         inner join user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' 
	         union
	         select a.*,ST_ASTEXT(ST_Centroid(sp_geometry))  as geom,''Point'' as geom_type,
	         '''||s_entity_title||''' as entity_title,a.building_name as entity_name from '||S_REPORT_VIEW_NAME||' a 
	         inner join Polygon_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')
	         inner join user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||'
	       ) a where 1=1 ';
	else
	      -- ISP CABLE WILL NOT BE THE PART OF KML AND SHAPE FILE AS THEIR IS NO GEOM FOR ISP CABLE.
	      sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,'''||s_geom_type||''' as geom_type,
              '''||s_entity_title||''' as entity_title,''''as entity_name,geom, '||S_LAYER_COLUMNS||' from (
                select a.*,ST_ASTEXT(sp_geometry) as geom from '||S_REPORT_VIEW_NAME||' a 
                inner join user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||'
                left join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||'''
              ) where 1=1 ';
	END IF;	
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
RAISE INFO 'network status %', 1;
	sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in('||p_networkStatues||')';
END IF;
-- PROJECT CODE FILTER
IF ((p_projectcode) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PROJECT_ID%') THEN
	sql:= sql ||' AND  a.project_id IN ('||p_projectcode||')';
else IF ((p_projectcode) != '') then 
	sql:= sql ||' AND  0 = 1';
END IF;
END IF;
-- PLANNING CODE FILTER
IF ((p_planningcode) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PLANNING_ID%') THEN
	sql:= sql ||' AND  a.planning_id IN ('||p_planningcode||')';
END IF;
-- WORKORDER CODE FILTER
IF ((p_workordercode) != '' and upper(S_LAYER_COLUMNS_VAL) like '%WORKORDER_ID%') THEN
	sql:= sql ||' AND  a.workorder_id IN ('||p_workordercode||')';
END IF;
-- PURPOSE CODE FILTER
IF ((p_purposecode) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PURPOSE_ID%') THEN
	sql:= sql ||' AND  a.purpose_id IN ('||p_purposecode||')';
END IF;

-- ADVANCED FILTER
IF ((p_advancefilter) != '') THEN
sql:= sql ||''|| p_advancefilter||'';
END IF;

sql:= sql ||' )X';

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
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 

sql:= 'Select * from ('||sql||')a where geom is not null';

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;

$BODY$;

ALTER FUNCTION public.fn_get_utilization_report_kml_shape(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, integer, integer)
    OWNER TO postgres;
