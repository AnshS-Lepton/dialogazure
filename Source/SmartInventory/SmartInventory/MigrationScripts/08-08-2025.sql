
--------------------------- Modificaton for multiple network status in utilization report summary-------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_utilization_report_summary(
	p_regionids character varying,
	p_provinceids character varying,
	p_layerids character varying,
	p_networkstatues character varying,
	p_projectcodes character varying,
	p_planningcodes character varying,
	p_workordercodes character varying,
	p_purposecodes character varying,
	p_geom character varying,
	p_userid integer,
	p_roleid integer)
    RETURNS TABLE(summary_id integer, network_status character varying, entity_id integer, entity_title character varying, entity_name character varying, region_id integer, region character varying, province_id integer, province character varying, low_count integer, moderate_count integer, high_count integer, over_count integer, utilization_text character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

Declare v_arow record;
sql TEXT;
s_layer_columns_val text;
columnquery TEXT;
v_layer_ids text;
v_mduct_system_id integer;
chkIsMicrodct boolean;
Begin

p_networkstatues := replace(coalesce(p_networkstatues, ''), '@', '''');

-- TEMP TABLE TO STORE THE SUMMARY OF EACH ENTITY..
create temp table temputilizationreportsummary
(
    network_status character varying,
    entity_id integer, 
    entity_title character varying, 
    entity_name character varying,
    region_id integer,
    region character varying,
    province_id integer,
    province character varying,
    low_count integer,
    moderate_count integer,
    high_count integer,
    over_count integer,
    utilization_text character varying
)
on commit drop;

if(p_layerids = '')then
select STRING_AGG(LAYER_ID::character varying, ',') into v_layer_ids from LAYER_DETAILS where isvisible = true and is_utilization_enabled = true;
else
v_layer_ids := (p_layerids):: text;
end if;


IF ((p_geom) != '')
THEN

  Create temp table temp_entity_inside_geom
  (
      system_id integer,
      entity_type character varying
  )
  on commit drop;
  -- HERE WE ARE FETCHING ALL THE ENTITY WITHIN THE ARAE SELECTED BY USER INTO A TEMP TABLE.

  raise info 'v_layer_ids%',v_layer_ids;
  perform(fn_get_export_report_entity_within_geom(v_layer_ids,p_geom));
  
end if;

for v_arow in select LAYER_ID,LAYER_TITLE, REPORT_VIEW_NAME, GEOM_TYPE,layer_name from LAYER_DETAILS where isvisible = true and is_utilization_enabled = true  and case when coalesce(v_layer_ids,'')!='' then  (layer_id) in (select unnest(string_to_array(v_layer_ids, ','))::integer) else true end order by layer_name
  loop
	sql := '';
	-- FETCH ALL REPORT COLUMNS FROM LAYER COLUMN SETTINGS TABLE
	SELECT STRING_AGG(COLUMN_NAME, ',') INTO S_LAYER_COLUMNS_VAL FROM (
	SELECT  COLUMN_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=v_arow.LAYER_ID  AND UPPER(SETTING_TYPE)='REPORT'
	ORDER BY COLUMN_SEQUENCE) A;
   IF(upper(S_LAYER_COLUMNS_VAL) like '%NETWORK_STATUS%' and upper(S_LAYER_COLUMNS_VAL) like '%UTILIZATION%')THEN

		IF(upper(v_arow.layer_name)='DUCT') THEN
		
		columnquery :='network_status,cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,
		cast('''||v_arow.layer_name||'''as text)entityName,region_id,region_name,province_id,province_name,
		sum(1) as low_count,
		0 as moderate_count,
		0 as high_count,
		0 as over_count,coalesce(a.utilization,''0'')||''% utilized'' as utilization_text';
		

		ELSE
		-- SHOW NETWORK STATUS COUNT SEPERATLY --
		columnquery :='network_status,cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,
		cast('''||v_arow.layer_name||'''as text)entityName,region_id,region_name,province_id,province_name,
		coalesce(sum(case when ((network_status=''Planned'' or network_status=''As Built'') and utilization=''L'') then 1 else 0 end),0)low_count,
		coalesce(sum(case when ((network_status=''Planned'' or network_status=''As Built'') and utilization=''M'') then 1 else 0 end),0)moderate_count,
		coalesce(sum(case when ((network_status=''Planned'' or network_status=''As Built'') and utilization=''H'') then 1 else 0 end),0)high_count,
		coalesce(sum(case when ((network_status=''Planned'' or network_status=''As Built'') and utilization=''O'') then 1 else 0 end),0)over_count,'''' utilization_text';
		
		END IF;
	
	   
	--DYNAMIC QUERY TO FETCH ENTITY SUMMARY
	IF ((p_geom) != '') THEN
	
		--FETCH RECORD BASED ON SELECTED GEOMETRY.
		sql:= 'select '||columnquery||' from '|| v_arow.REPORT_VIEW_NAME||' a inner join temp_entity_inside_geom m
		on a.system_id=m.system_id and upper(m.entity_type)=upper('''||v_arow.layer_name||''') 
		where 1=1 ';
	ELSE
	
		-- FETCH RECORDS BASED ON SELECTED FILTERS.
		sql:= 'select '||columnquery||' from '|| v_arow.REPORT_VIEW_NAME||' a where 1=1 ';
		
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
	IF(upper(v_arow.layer_name)='DUCT') THEN

	sql:= sql || ' group by utilization,network_status,region_id,region_name,province_id,province_name';
	execute ('insert into tempUtilizationReportSummary Select network_status,entityid,entitytitle,entityName,region_id,region_name,province_id,province_name,(low_count),
	(moderate_count),(high_count),(over_count),utilization_text from( '||sql||')result');
	ELSE
	sql:= sql || ' group by network_status,region_id,region_name,province_id,province_name';
	execute ('insert into tempUtilizationReportSummary Select network_status,entityid,entitytitle,entityName,region_id,region_name,province_id,province_name,(low_count),
	(moderate_count),(high_count),(over_count),fn_get_utilization_text (entityid,left(network_status,1),region_id,province_id) from( '||sql||')result');
	
	END IF;
	
	--RAISE EXCEPTION  'Calling result(%)', S_LAYER_COLUMNS;
	--RAISE EXCEPTION  'Calling result(%)', sql;
	-- sql:= sql ||' )X';
	--execute ('insert into tempUtilizationReportSummary Select network_status,entityid,entitytitle,entityName,region_id,region_name,province_id,province_name,sum(low_count),
	--sum(moderate_count),sum(high_count),sum(over_count),fu_get_utilization_text(entityid,network_status,region_id,province_id) from( '||sql||')result group by network_status,entityid,entitytitle,entityName,region_id,region_name,province_id,province_name,utilization_text');
	
	RAISE INFO 'queryFinal % ', sql;
end if;
end loop;

RETURN QUERY
Select cast(ROW_NUMBER() OVER (ORDER BY e.network_status desc) as integer)summary_id,* from tempUtilizationReportSummary e 
where (e.low_count + e.moderate_count + e.high_count+ e.over_count) > 0 or upper(e.entity_name)='DUCT'
order by e.network_status desc,entity_title,region,province asc;
End;

$BODY$;

--------------------------- Modificaton for multiple advance filter in utilization report view-------------------------------------

-- FUNCTION: public.fn_get_utilization_report_view(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_utilization_report_view(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer, integer);

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

-- DROP TABLE temp_core;

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


										/*Commented as changes asked from the client side*/
									--	core_join_query:='select * from (select cable_id, count(case when a_end_status_id=2 then 1 else 0 end) as a_end_utilized,
--count(case when a_end_status_id=1 then 1 else 0 end) as a_end_available, count(case when b_end_status_id=2 then 1 else 0 end) as b_end_utilized
--,count(case when b_end_status_id=1 then 1 else 0 end) as b_end_available from att_details_cable_info  group by cable_id)
                                     --   cfo ';
										
--core_select_query:=' ,x.a_end_utilized as "Utilized core(a)",x.a_end_available as "Available core(a)",x.b_end_utilized as "Utilized core(b)",x.b_end_available as "Available core(b)"
--,
--x.a_end_utilized * 100/X.total_core as "Utilized Core(a) Percentage(%)",x.b_end_utilized * 100/x.total_core as "Utilized Core(b) Percentage(%)"';


core_join_query:='select * from (select inf.cable_id,
			   count(inf.link_system_id) as a_end_utilized
,(cbl.total_core - count(inf.link_system_id)) as b_end_available,0,0
			   from att_details_cable_info inf left join att_details_cable cbl on cbl.system_id=inf.cable_id where inf.link_system_id <>0  group by inf.cable_id,cbl.total_core)
                                        cfo ';
										
core_select_query:=' ,COALESCE(x.a_end_utilized, 0) as "Utilized core",COALESCE(x.a_end_available,0) as "Available core",
ROUND(COALESCE(x.a_end_utilized, 0) * 100/X.total_core::NUMERIC,2) as "Utilized Core Percentage(%)",ROUND(COALESCE(x.a_end_available,0) * 100/X.total_core ::NUMERIC,2) as "Available Core Percentage(%)"';

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
	pa.user_id = '||p_userid||' and 1=1 '||case when P_LAYER_NAME='Cable' then ' inner join temp_core cfo on cfo.cable_id=a.system_id ' else ' left join temp_port cfo on cfo.parent_system_id=a.system_id
                           and Replace(a.category,'' '','''')=cfo.parent_entity_type ' end||'
	';

ELSE
	-- FETCH RECORDS BASED ON SELECTED FILTERS.
	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'system_id' ELSE P_SORTCOLNAME END
	||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||' 
	'||case when P_LAYER_NAME='Cable' then core_select_query else core_select_query_1 end||'
	from (select a.system_id,a.region_id,a.province_id,'||s_layer_columns_val2||',cfo.*  from '||S_REPORT_VIEW_NAME||' a 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' and 1=1 
	'||case when P_LAYER_NAME='Cable' then ' inner join temp_core cfo on cfo.cable_id=a.system_id ' else 'left join temp_port cfo on cfo.parent_system_id=a.system_id
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


---------------------------------------------------------- ODF File generation-------------------------------------------

-- FUNCTION: public.fn_get_fms_connection_report(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_fms_connection_report(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer);

CREATE OR REPLACE FUNCTION public.fn_get_fms_connection_report(
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
V_AROW RECORD;
v_entity_port_no character varying;
v_maxcount integer;
v_maxpathid integer;
v_ststem_id integer;
v_entity_type integer;
v_maxfms_id integer;
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
s_layer_columns text;
TotalAppliedRecords integer; 
TotalRejectedRecords integer;
 r RECORD;
    ss_report_view_name TEXT;
    d_report_view_name TEXT;
    v_report_view_name TEXT;
    ss_layer_name TEXT;
    update_sql TEXT;
s_latitude text;
d_layer_name TEXT;
v_layer_name TEXT;
BEGIN

create temp table cpf_temp_result
(
id serial,
source_system_id integer,
source_network_id character varying(500),
source_port_no integer, 
source_entity_type character varying(100),
source_entity_title character varying(100),
destination_system_id integer,
destination_network_id character varying(500),
destination_port_no integer,
destination_entity_type character varying(100),
destination_entity_title character varying(100),
viya_system_id integer,
via_network_id character varying(500),
path_id integer,
via_entity_type character varying(100),
SOURCE_TUBE_COLOUR_CODE character varying(100),
DESTINATION_TUBE_COLOUR_CODE character varying(100),
SOURCE_PORT_COLOUR_CODE character varying(100),
DESTINATION_PORT_COLOUR_CODE character varying(100),
SOURCE_TUBE_NAME character varying(100),
DESTINATION_TUBE_NAME character varying(100),
fms_id integer,
splitter_id integer,
entity_type character varying(100),
client_id character varying,
client_link_id character varying,
client_core_id character varying,
otdr_length float(8) default (0)
)on commit drop; 

-------------------------------------------------------------------------------------------------------------------
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER.. 
SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,res_field_key,DISPLAY_NAME FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
IF ((p_geom) != '') 
THEN
if(p_radius>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10) into p_geom;
end if;
RAISE INFO '------------------------------------S_LAYER_COLUMNS%', S_LAYER_COLUMNS;
sql:= 'SELECT system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join '||s_geom_type||'_master m
on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||'  and 1=1 ';
--inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
RAISE INFO '------------------------------------sql%', sql;
ELSE
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
--inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
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
if(substring(P_searchbytext from 1 for 1)='"' and substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
then
sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
else
sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
end if;
END IF;
IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND a.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';
END IF;
sql:= sql ||' )X where system_id in(select parent_system_id from isp_port_info where parent_entity_type=''FMS'' and port_status_id=2 )';
RAISE INFO 'rrr%', sql;
--------------------------------------------------------------------------------------------------------------------
FOR V_AROW IN
-- SELECT 'FMS' as entity_type,system_id from att_details_fms 
-- where system_id in(808,805,810)--province_id = ANY (string_to_array(p_province, ',')::integer[]) and region_id = ANY (string_to_array(p_region, ',')::integer[])
EXECUTE 'SELECT ''FMS'' as entity_type, system_id FROM ('||sql||')a'
loop
perform(fn_get_fms_connection12(V_AROW.system_id,V_AROW.entity_type));
end loop;
--------------------------------------------------------------------------------------------------
delete from cpf_temp_result where source_network_id=destination_network_id;
if exists(select 1 from cpf_temp_result where source_entity_type='FMS')
then
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE source_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	-- Update query
	UPDATE cpf_temp_result 
	SET 
	destination_network_id = VIA_row.destination_network_id,
	destination_system_id = VIA_row.destination_system_id,
	destination_port_no = VIA_row.destination_port_no,
	destination_entity_type = VIA_row.destination_entity_type,
	destination_entity_title = VIA_row.destination_entity_title,
	viya_system_id = VIA_row.source_system_id,
	via_network_id = VIA_row.source_network_id,
	via_entity_type = VIA_row.source_entity_type

	FROM VIA_row
	WHERE 
	cpf_temp_result.destination_entity_type = VIA_row.source_entity_type
	AND cpf_temp_result.destination_system_id = VIA_row.source_system_id
	AND cpf_temp_result.destination_network_id = VIA_row.source_network_id
	AND cpf_temp_result.destination_PORT_NO = VIA_row.source_PORT_NO;
	-- Delete query
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result 
	WHERE source_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	DELETE FROM cpf_temp_result
	WHERE SOURCE_SYSTEM_ID IN (SELECT SOURCE_SYSTEM_ID FROM VIA_row);
else

	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE destination_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	-- Update query
	UPDATE cpf_temp_result 
	SET 
	source_network_id = VIA_row.source_network_id,
	source_system_id = VIA_row.source_system_id,
	source_port_no = VIA_row.source_port_no,
	source_entity_type = VIA_row.source_entity_type,
	source_entity_title = VIA_row.source_entity_title,
	viya_system_id = VIA_row.destination_system_id,
	via_network_id = VIA_row.destination_network_id,
	via_entity_type = VIA_row.destination_entity_type

	FROM VIA_row
	WHERE 
	cpf_temp_result.source_entity_type = VIA_row.source_entity_type
	AND cpf_temp_result.source_system_id = VIA_row.source_system_id
	AND cpf_temp_result.source_network_id = VIA_row.source_network_id
	AND cpf_temp_result.source_PORT_NO = VIA_row.source_PORT_NO;
	-- Delete query
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result 
	WHERE destination_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	DELETE FROM cpf_temp_result
	WHERE destination_SYSTEM_ID IN (SELECT destination_SYSTEM_ID FROM VIA_row);
 
end if;

--select count(*) into v_maxcount from cpf_temp_result group by path_id order by count(*) desc limit 1;--header
select count(*) into v_maxcount from cpf_temp_result group by fms_id,path_id order by count(*) desc limit 1;
SELECT COUNT(DISTINCT path_id) AS path_count
FROM cpf_temp_result group by fms_id order by COUNT(DISTINCT path_id) desc limit 1 
INTO v_maxpathid;---dataloop
SELECT COUNT(DISTINCT fms_id) AS fms_id_count from cpf_temp_result
INTO v_maxfms_id;
------------------------------------------------------------------------------------------------------------------------------------
UPDATE cpf_temp_result 
SET SOURCE_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
SOURCE_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
SOURCE_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code ,client_id =adfl.customer_id,client_link_id =adfl.link_id,client_core_id=adfl.link_id,otdr_length=adfl.otdr_distance
FROM ATT_DETAILS_CABLE_INFO 
left join att_details_fiber_link adfl on adfl.system_id =ATT_DETAILS_CABLE_INFO.link_system_id 
WHERE cpf_temp_result.SOURCE_ENTITY_type='Cable'
and cpf_temp_result.source_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.source_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;

--select * from ATT_DETAILS_CABLE_INFO limit 20 where cable_id=847

UPDATE cpf_temp_result 
SET DESTINATION_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
DESTINATION_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
DESTINATION_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code ,client_id =adfl.customer_id,client_link_id =adfl.link_id,client_core_id=adfl.link_id,otdr_length=adfl.otdr_distance
FROM ATT_DETAILS_CABLE_INFO 
left join att_details_fiber_link adfl on adfl.system_id =ATT_DETAILS_CABLE_INFO.link_system_id  
WHERE cpf_temp_result.DESTINATION_ENTITY_type='Cable'
and cpf_temp_result.DESTINATION_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.DESTINATION_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;

-- select * from att_details_cable where network_id='MAT-CBL000356'
-- select link_system_id from ATT_DETAILS_CABLE_INFO  where cable_id=361597
-- select link_id,customer_id,main_link_id from att_details_fiber_link  where system_id in(select link_system_id from ATT_DETAILS_CABLE_INFO  where cable_id=361597)

--       UPDATE cpf_temp_result 
--       SET 
--       splitter_id =destination_system_id
--       where destination_entity_type = 'Splitter' and 
--       destination_system_id not in(select source_system_id from cpf_temp_result where source_entity_type = 'Splitter') ; 
   
--       UPDATE cpf_temp_result 
--       SET 
--       entity_type ='Splitter'
--       where destination_entity_type = 'Splitter' ; 

       for  r in  select  * from cpf_temp_result loop

select report_view_name,layer_name into ss_report_view_name,ss_layer_name from layer_details where lower(layer_name) =lower(r.source_entity_type);
select report_view_name,layer_name into d_report_view_name,d_layer_name from layer_details where lower(layer_name) =lower(r.destination_entity_type);
if r.via_entity_type is not null then 
select report_view_name,layer_name into v_report_view_name,v_layer_name from layer_details where lower(layer_name) =lower(r.via_entity_type);
end if;
if lower(ss_layer_name) = 'patchcord'   then 
ss_layer_name ='patch_cord';
end if;
if  lower(d_layer_name) = 'patchcord'  then 
d_layer_name ='patch_cord';
end if;
if   lower(v_layer_name) = 'patchcord' then 
v_layer_name ='patch_cord';
end if ;

ss_layer_name:=ss_layer_name||'_name';
d_layer_name:= d_layer_name||'_name';
v_layer_name =v_layer_name||'_name';
s_latitude ='latitude';
raise info 'ss_report_view_name% ',ss_report_view_name;
raise info 'd_report_view_name% ',d_report_view_name;
raise info 'v_report_view_name% ',v_report_view_name;

if ss_report_view_name IS NOT NULL then
if lower(r.source_entity_type) != 'cable' then 

EXECUTE '
        UPDATE cpf_temp_result ctf
        SET source_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || ss_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(ss_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.source_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
else 

EXECUTE 
        'UPDATE cpf_temp_result ctf
        SET source_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || ss_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(ss_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.source_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';

end if;
end if;
 if d_report_view_name IS NOT NULL then 
if lower(r.destination_entity_type) != 'cable' then 
EXECUTE ' UPDATE cpf_temp_result ctf
        SET destination_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || d_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(d_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.destination_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || '; ';

else
EXECUTE 
        'UPDATE cpf_temp_result ctf
        SET destination_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || d_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(d_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.destination_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
end if;
end if;
if v_report_view_name IS NOT NULL then 
if lower(r.via_entity_type) != 'cable' then 
if r.via_entity_type is not null and r.via_entity_type !='' then 
raise info 'test12% ','test12';

EXECUTE '
        UPDATE cpf_temp_result ctf
        SET via_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || v_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(v_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.viya_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';

end if;

else

if r.via_entity_type is not null and r.via_entity_type !='' then 
   EXECUTE      'UPDATE cpf_temp_result ctf
        SET via_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || v_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(v_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.viya_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
end if;
end if;
end if;
end loop;
-------------------------------------------------------------------------------------------------
/*
IF v_maxcount > 100 THEN
   v_maxcount := 100; -- cap headers to 100
END IF;

IF v_maxpathid > 100 THEN
   v_maxpathid := 100; -- cap data rows to 100
END IF;
*/
return query
select row_to_json(result) 
from (select (select array_to_json(array_agg(row_to_json(x)))from (select *,v_maxcount as headerCol,v_maxpathid as rowsdataloop,v_maxfms_id as globaLoopcount
from cpf_temp_result order by id) x) as lstConnectionInfo
)result ;
END; 
$BODY$;

ALTER FUNCTION public.fn_get_fms_connection_report(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer)
    OWNER TO postgres;

	---------------------------------------------------------- ODF File generation-------------------------------------------

	CREATE OR REPLACE FUNCTION public.fn_get_fms_connection12(
	p_entity_system_id integer,
	p_entity_type character varying)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    V_AROW RECORD;
BEGIN
    FOR V_AROW IN
        SELECT port_number
        FROM isp_port_info
        WHERE parent_system_id = p_entity_system_id
          AND parent_entity_type = p_entity_type
          AND port_status_id = 2
          AND input_output = 'O'
    LOOP
        WITH ranked_data AS (
            SELECT
                a.source_system_id,
                a.source_network_id,
                a.source_port_no,
                a.source_entity_type,
                a.source_entity_title,
                a.destination_system_id,
                a.destination_network_id,
                a.destination_port_no,
                a.destination_entity_type,
                a.destination_entity_title,
                NULL::INTEGER AS viya_system_id,
                NULL:: INTEGER AS via_network_id,
                V_AROW.port_number AS path_id,
                NULL AS via_entity_type,
                p_entity_system_id AS fms_id,
                CASE WHEN a.source_entity_type = 'Splitter' THEN a.source_system_id ELSE NULL END AS splitter_id,
                CASE WHEN a.source_entity_type = 'Splitter' THEN a.source_entity_type ELSE NULL END AS entity_type,
                ROW_NUMBER() OVER (
                    PARTITION BY a.source_port_no,a.source_SYSTEM_ID, a.destination_port_no,a.destination_SYSTEM_ID
                    --ORDER BY a.destination_system_id ASC
                ) AS rn
            FROM fn_get_fms_connection_path(
                     '', '', 0, 0, '', '',
                     p_entity_system_id, V_AROW.port_number, p_entity_type
                 ) a
            WHERE a.source_system_id != a.destination_system_id
              AND a.source_entity_type != a.destination_entity_type
        )
        INSERT INTO cpf_temp_result (
            source_system_id, source_network_id, source_port_no, source_entity_type, source_entity_title,
            destination_system_id, destination_network_id, destination_port_no, destination_entity_type, destination_entity_title,
            viya_system_id, via_network_id, path_id, via_entity_type, fms_id, splitter_id, entity_type
        )
        SELECT
            source_system_id, source_network_id, source_port_no, source_entity_type, source_entity_title,
            destination_system_id, destination_network_id, destination_port_no, destination_entity_type, destination_entity_title,
            viya_system_id, via_network_id, path_id, via_entity_type, fms_id, splitter_id, entity_type
        FROM ranked_data
        WHERE rn = 1;
		drop  table cpf_temp;
    END LOOP;
END;
$BODY$;

ALTER FUNCTION public.fn_get_fms_connection12(integer, character varying)
    OWNER TO postgres;

	---------------------------------------------------------- Download file from the FTP-------------------------------------------

	CREATE OR REPLACE FUNCTION public.fn_get_downloadtextformatfile(
	p_entity_system_id integer)
    RETURNS TABLE(id integer, entity_system_id integer, entity_type character varying, org_file_name character varying, file_name character varying, file_extension character varying, file_location character varying, upload_type character varying, uploaded_by character varying, file_size integer, entity_feature_name character varying, uploaded_on timestamp without time zone, is_barcode_image boolean, is_meter_reading_image boolean, document_type character varying, ticket_id integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

BEGIN
    RETURN QUERY
    SELECT 
        la.id AS Id,
        la.entity_system_id AS EntitySystemId, 
        la.entity_type AS EntityType,
        la.org_file_name AS OrgFileName,
        la.file_name AS FileName,
        la.file_extension AS FileExtension,
        la.file_location AS FileLocation,
        la.upload_type AS UploadType, 
        um.user_name AS UploadedBy,
        la.file_size,
        la.entity_feature_name,
        la.uploaded_on,
        la.is_barcode_image,
        la.is_meter_reading_image,
        la.document_type,
		  la.ticket_id
    FROM library_attachments la 
    INNER JOIN user_master um ON la.uploaded_by = um.user_id 
    --INNER JOIN att_details_cable att ON att.system_id = la.entity_system_id 
    INNER JOIN vw_network_layers vnl 
        ON vnl.layer_id = (SELECT layer_id FROM layer_details WHERE upper(layer_name) = upper('Cable')) 
        AND vnl.role_id = um.role_id
    WHERE la.id = p_entity_system_id;
END;
$BODY$;

ALTER FUNCTION public.fn_get_downloadtextformatfile(integer)
    OWNER TO postgres;
