
CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary_view_kml(p_regionids character varying,
p_provinceids character varying, p_networkstatues character varying,
p_parentusers character varying, p_userids character varying, p_layer_name character varying, 
p_projectcode character varying, p_planningcode character varying, p_workordercode character varying,
p_purposecode character varying, p_durationbasedon character varying, p_fromdate character varying, 
p_todate character varying, p_geom character varying, p_pageno integer, p_pagerecord integer,
p_sortcolname character varying, p_sorttype character varying, p_advancefilter character varying,
p_filetype character varying, p_userid integer, p_roleid integer, p_ownership_type character varying, p_thirdparty_vendor_ids character varying, p_radious double precision)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$


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
query_value text;
Begin
LowerStart:='';
LowerEnd:='';
SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_title INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_entity_title FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE COLUMNS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER // FOR SHAPE FILE - ALL ACTIVE COLUMNS, FOR KML ONLY CONFIGURED COLUMN	
SELECT STRING_AGG(COLUMN_NAME||' as "'||DISPLAY_NAME||'"', ',') INTO S_LAYER_COLUMNS x
FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME  
FROM LAYER_COLUMNS_SETTINGS WHERE 
--case when p_filetype ='SHAPE' THEN 1=1 ELSE  
is_kml_column_required = true 
--END 
and LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

--SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
--SELECT STRING_AGG(COLUMN_NAME||' as "'||DISPLAY_NAME||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
--SELECT  COLUMN_NAME,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' ORDER BY COLUMN_SEQUENCE) A;

-- IF NULL THEN SET IT BLANK TO AVOID ERROR
S_LAYER_COLUMNS := coalesce(S_LAYER_COLUMNS,'');
S_LAYER_COLUMNS_VAL := coalesce(S_LAYER_COLUMNS,'');

raise info 'S_LAYER_COLUMNS  %',S_LAYER_COLUMNS;
raise info 'S_LAYER_COLUMNS_VAL  %',S_LAYER_COLUMNS_VAL;

	IF(S_LAYER_COLUMNS != '')
	THEN 
	S_LAYER_COLUMNS = ','||S_LAYER_COLUMNS;
	END IF;

	-- DYNAMIC QUERY
	IF ((p_geom) != '') 
	THEN

	if(p_radious>0)
	then
	select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10) into p_geom;
	end if;
		
        
--         create temp table temp_entity_inside_geom(
-- 		system_id integer,
-- 		sp_geometry geometry,
-- 		entity_type character varying
-- 		)on commit drop;

		IF(lower(p_layer_name) = lower('Building')) 
		THEN
			
		query_value:=	'select system_id ,entity_type,ST_Centroid(m.sp_geometry) as sp_geometry from POLYGON_MASTER m where upper(m.entity_type)=upper('''||P_LAYER_NAME||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))
			UNION
			select system_id ,entity_type,m.sp_geometry from POINT_MASTER m where upper(m.entity_type)=upper('''||P_LAYER_NAME||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))';
		ELSE	
			--execute 'insert into temp_entity_inside_geom(system_id,entity_type,sp_geometry) 
			query_value:='select system_id ,entity_type,m.sp_geometry from '||s_geom_type||'_master m
			where upper(m.entity_type)=upper('''||P_LAYER_NAME||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))';	
		END IF;

		-- ISP CABLE WILL NOT BE THE PART OF KML AND SHAPE FILE AS THEIR IS NO GEOM FOR ISP CABLE.
		sql:= 'select  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,'''||s_geom_type||''' as 
		geom_type,'''||s_entity_title||''' as entity_title,''''as entity_name,ST_ASTEXT(sp_geometry) as 
		geom '||S_LAYER_COLUMNS||' from(select m.sp_geometry,a.* from '||S_REPORT_VIEW_NAME||' a inner join ( ' || query_value || ' )  m
		on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')  
		inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
	ELSE
		IF(lower(p_layer_name) = lower('Building')) 
		THEN	
		       sql:= 'SELECT * FROM ( SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,geom_type,entity_title,entity_name,
		       geom '||S_LAYER_COLUMNS||' from (
			 select a.*,ST_ASTEXT(sp_geometry) as geom,'''||s_geom_type||''' as geom_type,'''||s_entity_title||''' as   
			 entity_title,  a.building_name as entity_name from '||S_REPORT_VIEW_NAME||' a inner join point_master m
			 on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
			 inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||'
			 union
			 select a.*,ST_ASTEXT(ST_Centroid(sp_geometry))  as geom,''Point'' as geom_type,
			 '''||s_entity_title||''' as entity_title,a.building_name as entity_name from '||S_REPORT_VIEW_NAME||' a 
			 inner join Polygon_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')
			 inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||'
		       ) a where 1=1 ';
		else
		      -- ISP CABLE WILL NOT BE THE PART OF KML AND SHAPE FILE AS THEIR IS NO GEOM FOR ISP CABLE.
		      sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,'''||s_geom_type||''' as geom_type,
		      '''||s_entity_title||''' as entity_title,''''as entity_name,geom '||S_LAYER_COLUMNS||' from (
			select a.*,ST_ASTEXT(sp_geometry) as geom from '||S_REPORT_VIEW_NAME||' a 
			inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' 
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
		sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in('||p_networkStatues||')';
	END IF;
	-- PARENT/CHILD USER FILTER
	if(p_parentusers != '' and p_parentusers != '0') THEN
	if(p_userids != '')THEN
		sql := sql ||' AND cast(a.created_by_id as integer) in ('||p_userids||')';
	else
		--sql := sql ||' AND cast(a.created_by_id as integer) in (select user_id from user_master where manager_id in ('||p_parentusers||') or user_id in ('||p_parentusers||'))';
		--sql := sql ||' AND cast(a.created_by_id as integer) in (select userid from fn_get_report_mapped_users('||p_parentusers||'))';
		if(p_roleid <>1) THEN
				
				      sql := sql ||' AND cast(a.created_by_id as integer) in (select * from fn_get_report_mapped_users('||p_parentusers||'))';
				
				ELSE 
					sql := sql ||' AND cast(a.created_by_id as integer) in (select user_id from user_master where manager_id 
							in ('||p_parentusers||') or user_id in ('||p_parentusers||'))';
				END IF;
	END IF;
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

	-- DURATION FILTER
	IF(P_fromDate != '' and P_toDate != '') THEN
	sql:= sql ||' AND a.'||p_durationbasedon||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_durationbasedon||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';
	END IF;

	-- ADVANCED FILTER
	IF ((p_advancefilter) != '') THEN
	sql:= sql ||''|| p_advancefilter||'';
	END IF;

	-- OWNER SHIP FILTER
		IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN
			sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''')';
		else IF ((p_ownership_type) != '') then 
			sql:= sql ||' AND  0 = 1';
		END IF;	
		end if;
		
		IF ((p_thirdparty_vendor_ids) != '' and upper(S_LAYER_COLUMNS_VAL) like '%THIRD_PARTY_VENDOR_ID%') THEN
			sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''') AND a.third_party_vendor_id in('||p_thirdparty_vendor_ids||')';
		else IF ((p_thirdparty_vendor_ids) != '') then 
			sql:= sql ||' AND  0 = 1';
		end if;
		end if;
		
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

RAISE INFO 'QUERY final %   ', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;

$function$
;