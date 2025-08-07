
------------------------------------- Update fiber link  filter type------------------------------------------------

update LAYER_COLUMNS_SETTINGS set is_utilization_column=true where  LAYER_ID=(SELECT LAYER_ID FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER('Cable') ) AND UPPER(SETTING_TYPE)='REPORT' and  column_name  in ('network_id','cable_name');

------------------------------------- insert fiber link  filter type------------------------------------------------

INSERT INTO public.fiber_link_columns_settings
(column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column)
VALUES('OP_ALIAS', 44, 'OP ALIAS', false, 1, now(), NULL, NULL, 'SI_OSP_GBL_GBL_FRM_102', false);



---------------------------------------- Modification to get fiber link columns mappings function ------------------------------- 


DROP FUNCTION IF EXISTS public.fn_get_fiber_link_columns_mappings();

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_columns_mappings(
	)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
BEGIN
	 
	-- EXECUTE AND RETURN QUERY RESULT..
	RETURN QUERY EXECUTE 'SELECT ROW_TO_JSON(ROW) FROM(SELECT column_name,is_active,display_name FROM fiber_link_columns_settings where is_active=true or column_name in (''sub_link_id'',''OP_ALIAS'') order by column_sequence) ROW';

END
$BODY$;

--------------------------------------Modification in fiber link status function ---------------------------------------

DROP FUNCTION IF EXISTS public.fn_get_fiber_link_status(integer, character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone);

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_status(
	p_systemid integer,
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_userid integer,
	p_searchfrom timestamp without time zone,
	p_searchto timestamp without time zone)
    RETURNS TABLE(fiber_link_status character varying, color_code character varying, fiber_link_count integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
Declare v_user_role_id integer;
 sql TEXT;
 main_link_Id  character varying;
BEGIN
sql:='';
select role_id into v_user_role_id from user_master where user_id=p_userid;

 

sql:= 'select tsm.name as fiber_link_status,tsm.color_code ,count(fl.system_id)::integer as fiber_link_count from vw_att_details_fiber_link fl right join 
fiber_link_status_master tsm on upper(tsm.name)=upper(fl.fiber_link_status) '; 

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
 if(p_searchby='sub_link_id')
 then 
 select fb.main_link_id into main_link_Id from vw_att_details_fiber_link fb where fb.link_id=p_searchtext;
 p_searchtext := main_link_Id;
 p_searchby := 'link_id';
 end if;
 if(p_searchby='OP_ALIAS')
 then 
 p_searchby := 'service_id';
 end if;
sql:= sql ||' AND lower('|| 'fl.'||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';
END IF;
 
sql:= sql || ' where tsm.is_active=true group by tsm.name,tsm.color_code ';
raise info'final%',sql;
RETURN QUERY
EXECUTE 'select  fiber_link_status, color_code, fiber_link_count from ('||sql||') row';

END ;
$BODY$;

---------------------------------------------- Modification in fiber link details function ---------------------------------------


DROP FUNCTION IF EXISTS public.fn_get_fiber_link_details(integer, character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone);

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_details(
	p_systemid integer,
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_userid integer,
	p_searchfrom timestamp without time zone,
	p_searchto timestamp without time zone)
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
   TotalRecords INTEGER; 
   LowerStart  character varying;
   LowerEnd  character varying;
   v_user_role_id integer;
   s_layer_columns_val text; 
    main_link_Id  character varying;

BEGIN

sql:='';
LowerStart:='';
LowerEnd:='';

 --IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
    -- LowerStart:='LOWER(';
    -- LowerEnd:=')';
--END IF;
  if(p_sortcolname='OP_ALIAS')then p_sortcolname:='Service Id'; end if;
select role_id into v_user_role_id from user_master where user_id=p_userId;

-- FETCH ALL COLUMNS FROM COLUMN SETTINGS TABLE
	 
	SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM(
	 SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM fiber_link_columns_settings WHERE is_active=true and UPPER(column_name) not in('TOTAL_ROUTE_LENGTH','GIS_LENGTH','SUB_LINK_ID') order by Column_sequence) A;
	 

--- TO GET TOTAL_ROUTE_LENGTH & GIS_LENGTH
CREATE TEMP TABLE TEMP_CABLE_INFO(
 LINK_SYSTEM_ID INTEGER,
 CABLE_ID INTEGER
)ON COMMIT DROP;
 
CREATE TEMP TABLE TEMP_FIBER_LINK_DETAILS(
  LINK_SYSTEM_ID INTEGER,
  TOTAL_ROUTE_LENGTH DOUBLE PRECISION,
  GIS_LENGTH DOUBLE PRECISION
 )ON COMMIT DROP;

INSERT INTO TEMP_CABLE_INFO(LINK_SYSTEM_ID,CABLE_ID)
SELECT DISTINCT LINK_SYSTEM_ID,CABLE_ID FROM ATT_DETAILS_CABLE_INFO C
JOIN ATT_DETAILS_FIBER_LINK FIBER ON FIBER.SYSTEM_ID=C.LINK_SYSTEM_ID WHERE LINK_SYSTEM_ID>0;

 
INSERT INTO TEMP_FIBER_LINK_DETAILS(LINK_SYSTEM_ID,TOTAL_ROUTE_LENGTH,GIS_LENGTH)
 
SELECT DISTINCT T.LINK_SYSTEM_ID,ROUND((SUM(COALESCE(CABLE.cable_calculated_length,0)) + SUM(COALESCE(LOOP.LOOP_LENGTH,0)))::NUMERIC,2) AS TOTAL_ROUTE_LENGTH ,
ROUND((SUM(COALESCE(CABLE.CABLE_MEASURED_LENGTH,0)))::NUMERIC,2) AS CABLE_MEASURED_LENGTH
FROM TEMP_CABLE_INFO T
JOIN ATT_DETAILS_CABLE CABLE ON CABLE.SYSTEM_ID=T.CABLE_ID
LEFT JOIN( SELECT SUM(Loop_length) as loop_length,cable_system_id from ATT_DETAILS_LOOP LOOP group by LOOP.cable_system_id)loop ON LOOP.CABLE_SYSTEM_ID= CABLE.SYSTEM_ID
group by T.link_system_id
ORDER BY T.link_system_id DESC;

 
-- MANAGE SORT COLUMN NAME
IF (coalesce(TRIM(P_SORTCOLNAME,''))!='') THEN 

SELECT TRIM( trailing '	' from ''||P_SORTCOLNAME||'') into P_SORTCOLNAME;
select column_name into P_SORTCOLNAME from fiber_link_columns_settings WHERE UPPER(DISPLAY_NAME)=UPPER(P_SORTCOLNAME) ;
End IF;

 raise info'P_SORTCOLNAME% ',P_SORTCOLNAME;
  raise info'S_LAYER_COLUMNS_VAL% ',S_LAYER_COLUMNS_VAL;

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'fl.system_id' ELSE  'fl.'|| P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS S_No, 
fl.system_id,fl.network_id as "Network Id",'||S_LAYER_COLUMNS_VAL||', f2.TOTAL_ROUTE_LENGTH AS "Total Route Length(meter)",f2.GIS_LENGTH AS "GIS Length(meter)"
FROM vw_att_details_fiber_link fl
left join TEMP_FIBER_LINK_DETAILS f2 on fl.system_id=f2.link_system_id WHERE 1=1  ';
	

 IF(p_systemid >0 )THEN 
	sql:= sql ||'and fl.system_id='||p_systemid||'';
 END IF;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2 % ',sql;
 raise info'p_searchby % ',p_searchby;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
 if(p_searchby='sub_link_id')
 then 
 select fb.main_link_id into main_link_Id from vw_att_details_fiber_link fb where fb.link_id=p_searchtext;
 p_searchtext := main_link_Id;
 p_searchby := 'link_id';
 end if;
 if(p_searchby='OP_ALIAS')
 then 
 p_searchby := 'service_id';
 end if;
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;
	-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a ' INTO TotalRecords;
 
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords,*
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 
RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$BODY$;

ALTER FUNCTION public.fn_get_fiber_link_details(integer, character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone)
    OWNER TO postgres;

	----------------------------------------------------------------- Modification in select columns and thier values-------------------
	
 DROP FUNCTION IF EXISTS public.fn_get_utilization_report_view(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_get_utilization_report_view(
	p_regionids character varying,
	p_provinceids character varying,
	p_networkstatues character varying,
	p_layer_name character varying,
	p_projectcodes character varying,
	p_planningcodes character varying,
	p_workordercodes character varying,
	p_purposecodes character varying,
	p_geom character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_advancefilter character varying,
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
s_layer_name character varying;
s_layer_columns text; 
s_layer_columns_val text; 
core_join_query text;
core_select_query text;
core_join_query_1 text;
core_select_query_1 text;
s_layer_columns_val2 text;
Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';
p_advancefilter := replace(coalesce(p_advancefilter, ''), '@', '''');
-- public.temp_port definition

-- Drop table

-- DROP TABLE temp_port;

CREATE temp TABLE temp_port (
	parent_system_id integer,
	parent_entity_type character varying,
	out_utilized integer,
	out_available integer,
	input_utilized integer,
	input_available integer
)on commit drop;


CREATE temp TABLE temp_core (
	cable_id integer,
	a_end_utilized integer,
	a_end_available integer,
	b_end_utilized integer,
	b_end_available integer
)on commit drop;

core_join_query:='select * from (select cable_id, count(case when a_end_status_id=2 then 1 end) as a_end_utilized,
count(case when a_end_status_id=1 then 1 end) as a_end_available, count(case when b_end_status_id=2 then 1 end) as b_end_utilized
,count(case when b_end_status_id=1 then 1 end) as b_end_available from att_details_cable_info group by cable_id)
                                        cfo ';
										/*Commented as changes asked from the client side*/
--core_select_query:=' ,x.a_end_utilized as "Utilized core(a)",x.a_end_available as "Available core(a)",x.b_end_utilized as "Utilized core(b)",x.b_end_available as "Available core(b)"
--,
--x.a_end_utilized * 100/X.total_core as "Utilized Core(a) Percentage(%)",x.b_end_utilized * 100/x.total_core as "Utilized Core(b) Percentage(%)"';

core_select_query:=' ,x.a_end_utilized as "Utilized core",x.a_end_available as "Available core",
ROUND(x.a_end_utilized * 100/X.total_core::NUMERIC,2) as "Utilized Core Percentage(%)",ROUND(x.a_end_available * 100/X.total_core ::NUMERIC,2) as "Available Core Percentage(%)"';


core_join_query_1:='select * from (select parent_system_id,parent_entity_type, count(case when port_status_id=2 and
                                input_output=''O'' then 1 end)
                           as out_utilized,count(case when port_status_id=1 and input_output=''O'' then 1 end) as out_available,
                           count(case when port_status_id=2 and input_output=''I'' then 1 end) as input_utilized,
                           count(case when port_status_id=1 and input_output=''I'' then 1 end) as input_available
                           from isp_port_info group by parent_system_id,parent_entity_type)cfo 
						   ';
core_select_query_1:=' ,x.input_utilized as "Utilized port(I)",x.input_available as "Available port(I)",
                                        x.out_utilized as "Utilized port(O)",x.out_available as "Available port(O)"';

					
execute 'insert into temp_port (parent_system_id ,parent_entity_type ,out_utilized ,out_available ,input_utilized ,input_available) '||core_join_query_1||' '; 
execute 'insert into temp_core (cable_id ,a_end_utilized ,a_end_available ,b_end_utilized ,b_end_available) '||core_join_query||' '; 


-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS 
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

IF ((p_geom) != '') THEN

 Create temp table temp_entity_inside_geom
  (
      system_id integer,
      entity_type character varying
  )
  on commit drop;
  -- HERE WE ARE FETCHING ALL THE ENTITY WITHIN THE ARAE SELECTED BY USER INTO A TEMP TABLE.
  perform(fn_get_export_report_entity_within_geom(S_LAYER_ID::character varying,p_geom));
  
end if;

-- MANAGE SORT COLUMN NAME
IF (coalesce(P_SORTCOLNAME,'')!='') THEN 

	--SELECT ACTUAL COLUMN NAME FROM LAYER_COLUMN_SETTING BASED ON DISPLAY NAME.
	select column_name into P_SORTCOLNAME  from layer_columns_settings where layer_id=S_LAYER_ID and upper(setting_type)='REPORT' and 
	upper(display_name)=upper(P_SORTCOLNAME) and is_active=true;
	
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
SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE and is_utilization_column=true ORDER BY COLUMN_SEQUENCE) A;

-- SELECT ALL ACTIVE FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..	
SELECT STRING_AGG(COLUMN_NAME, ',') INTO s_layer_columns_val2 FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE and is_utilization_column=true ORDER BY COLUMN_SEQUENCE) A;



-- SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' and is_utilization_column=true ORDER BY COLUMN_SEQUENCE) A;
  
-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
	--FETCH RECORD BASED ON SELECTED GEOMETRY.
	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
	||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, 
	'||S_LAYER_COLUMNS||' 
	'||case when P_LAYER_NAME='Cable' then core_select_query else core_select_query_1 end||'
	from (select a.system_id,a.region_id,a.province_id,'||s_layer_columns_val2||',cfo.* from '||S_REPORT_VIEW_NAME||' a inner join temp_entity_inside_geom m on a.system_id=m.system_id 
	and upper(m.entity_type)=upper('''||s_layer_name||''') 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id and 
	pa.user_id = '||p_userid||' and 1=1 '||case when P_LAYER_NAME='Cable' then ' left join temp_core cfo on cfo.cable_id=a.system_id ' else ' left join temp_port cfo on cfo.parent_system_id=a.system_id
                           and Replace(a.category,'' '','''')=cfo.parent_entity_type ' end||'
	';

ELSE
	-- FETCH RECORDS BASED ON SELECTED FILTERS.
	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
	||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||' 
	'||case when P_LAYER_NAME='Cable' then core_select_query else core_select_query_1 end||'
	from (select a.system_id,a.region_id,a.province_id,'||s_layer_columns_val2||',cfo.*  from '||S_REPORT_VIEW_NAME||' a 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' and 1=1 
	'||case when P_LAYER_NAME='Cable' then ' left join temp_core cfo on cfo.cable_id=a.system_id ' else 'left join temp_port cfo on cfo.parent_system_id=a.system_id
                           and Replace(a.category,'' '','''')=cfo.parent_entity_type ' end||'
	';
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

-- ADVANCE FILTER SELCTED BY USER
IF ((p_advancefilter) != '') THEN
	sql:= sql ||''|| p_advancefilter||'';
END IF;

sql:= sql ||' )X';
RAISE INFO 'queryS_ %', sql;
-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;
-- RAISE EXCEPTION  'Calling result(%)', sql;
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
-- insert into data_query(data_sql,created_date)
--select sql,now();

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;

$BODY$;

ALTER FUNCTION public.fn_get_utilization_report_view(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer)
    OWNER TO postgres;

GRANT EXECUTE ON FUNCTION public.fn_get_utilization_report_view(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public.fn_get_utilization_report_view(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer) TO postgres;

