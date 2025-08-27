
-------------------------- Utilization show on map (advance filter, utilizatoin percentage and avoide network_id having null data)-----------------


-- FUNCTION: public.fn_get_utilization_show_on_map(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_utilization_show_on_map(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_utilization_show_on_map(
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
	p_utilizationtype character varying,
	p_ductutilization character varying)
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
s_fiber_type character varying;
s_fiber_status character varying; 
S_LAYER_COLUMNS_VAL text;
v_ductutilizationval character varying;
p_split_utilizationtype character varying;
c_utilization character varying;
Begin
LowerStart:='';
LowerEnd:='';

raise info '1%','1';

IF p_utilizationtype IS NULL OR p_utilizationtype = '' THEN
    p_split_utilizationtype := NULL;
else

SELECT replace(replace(p_utilizationtype, '''', ''), ',', '') into c_utilization;

    SELECT string_agg(quote_literal(c), ',')
    INTO p_split_utilizationtype
    FROM regexp_split_to_table(c_utilization, '') AS c;
END IF;

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_title INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_entity_title FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE COLUMNS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER // FOR SHAPE FILE - ALL ACTIVE COLUMNS, FOR KML ONLY CONFIGURED COLUMN	
SELECT STRING_AGG(COLUMN_NAME||' as "'||COLUMN_NAME||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE 1=1 and LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

SELECT STRING_AGG(COLUMN_NAME, ',') INTO S_LAYER_COLUMNS_VAL FROM (
	SELECT  COLUMN_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID  AND UPPER(SETTING_TYPE)='REPORT'
	ORDER BY COLUMN_SEQUENCE) A;
	
-- IF NULL THEN SET IT BLANK TO AVOID ERROR
S_LAYER_COLUMNS := coalesce(S_LAYER_COLUMNS,'');

raise info '2%','2';
	IF(lower(p_layer_name) = lower('duct')) then
		select split_part(p_ductutilization,'% utilized','1') into v_ductutilizationval;
	end if;

raise info 'v_ductutilizationval%',v_ductutilizationval;

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
 	geom,system_id, '||S_LAYER_COLUMNS_VAL||' from(select m.sp_geometry,a.* from '||S_REPORT_VIEW_NAME||' a inner join temp_entity_inside_geom m
 	on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')  where 1=1  and  a.network_id is not null ';
ELSE
	IF(lower(p_layer_name) = lower('Building')) 
	THEN	
	       sql:= 'SELECT * FROM ( SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,geom_type,entity_title,entity_name,
	       geom,system_id, '||S_LAYER_COLUMNS_VAL||' from (
	         select a.*,ST_ASTEXT(sp_geometry) as geom,'''||s_geom_type||''' as geom_type,'''||s_entity_title||''' as   
	         entity_title,  a.building_name as entity_name from '||S_REPORT_VIEW_NAME||' a inner join point_master m
	         on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
	         union
	         select a.*,ST_ASTEXT(ST_Centroid(sp_geometry))  as geom,''Point'' as geom_type,
	         '''||s_entity_title||''' as entity_title,a.building_name as entity_name from '||S_REPORT_VIEW_NAME||' a 
	         inner join Polygon_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')
	       ) a where 1=1  and  a.network_id is not null';
	       
	else
	      -- ISP CABLE WILL NOT BE THE PART OF KML AND SHAPE FILE AS THEIR IS NO GEOM FOR ISP CABLE.
	      sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,'''||s_geom_type||''' as geom_type,
              '''||s_entity_title||''' as entity_title,''''as entity_name,geom,system_id, '||S_LAYER_COLUMNS_VAL||' from (
                select a.*,ST_ASTEXT(sp_geometry) as geom from '||S_REPORT_VIEW_NAME||' a 
					left join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||'''
              ) where 1=1  and  a.network_id is not null';
              
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

/* commeted due issue while fetching details */
-- ADVANCED FILTER
IF (p_advancefilter != '') THEN
    p_advancefilter := replace(p_advancefilter, '@' , '''');  -- ? store back
    sql := sql || p_advancefilter;
    RAISE NOTICE 'Final SQL1234: %', p_advancefilter;
END IF;


-- UTILIZATION TYPE FILTER
IF(lower(p_layer_name) = lower('duct')) THEN
sql:= sql ||' AND  a.utilization = trim('''||v_ductutilizationval||''')';

ELSE IF ((p_split_utilizationtype) != '' and upper(S_LAYER_COLUMNS_VAL) like '%UTILIZATION%' and lower(p_layer_name) != lower('duct')) THEN
	sql:= sql ||' AND  a.utilization IN ('||p_split_utilizationtype||')';
END IF;
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

RAISE INFO 'queryFinal % ', sql;
--create table temp_utilizationOnMap(system_id integer,geom geometry,network_id character varying);
--execute 'insert into temp_utilizationOnMap Select system_id,geom,network_id from ('||sql||')d';

RETURN QUERY
--EXECUTE 'select row_to_json(row) from ('||sql||') row';
execute 'SELECT row_to_json(row) AS geojson FROM
(SELECT ''FeatureCollection'' As type, array_to_json(array_agg(f))
As features FROM (SELECT ''Feature'' As type, system_id as id,
ST_AsGeoJSON((tbl.geom),15,0)::json As geometry,
(select row_to_json(_) from (select tbl.network_id,upper('''||s_entity_title||''')as entity_title,upper('''||P_LAYER_NAME||''')as entity_name,tbl.utilization
) As _)as properties
FROM ('||sql||') As tbl) As f ) As row';

End;

$BODY$;

ALTER FUNCTION public.fn_get_utilization_show_on_map(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying)
    OWNER TO postgres;



	--------------------------------------------------------------------