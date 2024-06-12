CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary_view_cdb(
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

Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';

SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, 'vw_att_details_cable_cdb_attributes', GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER('CABLE');
IF ((p_geom) != '') THEN

--select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
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

S_LAYER_COLUMNS:='circle_name ,major_route_name,route_id,section_name,section_id,route_category,distance,
fiber_pairs_laid,total_used_pair,fiber_pairs_used_by_vil,fiber_pairs_given_to_airtel,fiber_pairs_given_to_others,
fiber_pairs_free,faulty_fiber_pairs,start_latitude,start_longitude,end_latitude,end_longitude,count_non_vil_tenancies_on_route,
route_lit_up_date,aerial_km,avg_loss_per_km,avg_last_six_months_fiber_cut,material,execution,row_availablity,
iru_given_airtel,iru_given_jio,iru_given_ttsl_or_ttml,network_category,row_valid_or_exp,remarks,cable_owner,route_type,
operator_type,fiber_type, cable_id,iru_given_tcl,iru_given_others';

RAISE INFO 'S_LAYER_COLUMNS---%', S_LAYER_COLUMNS;
RAISE INFO 'P_SORTCOLNAME%', P_SORTCOLNAME;

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
--FETCH RECORD BASED ON SELECTED GEOMETRY.
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'cable_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||'
from (select circle_name ,major_route_name,route_id,section_name,section_id,route_category,distance,
fiber_pairs_laid,total_used_pair,fiber_pairs_used_by_vil,fiber_pairs_given_to_airtel,fiber_pairs_given_to_others,
fiber_pairs_free,faulty_fiber_pairs,start_latitude,start_longitude,end_latitude,end_longitude,count_non_vil_tenancies_on_route,
route_lit_up_date,aerial_km,avg_loss_per_km,avg_last_six_months_fiber_cut,material,execution,row_availablity,
iru_given_airtel,iru_given_jio,iru_given_ttsl_or_ttml,network_category,row_valid_or_exp,remarks,cable_owner,route_type,
operator_type,fiber_type, cable_id,iru_given_tcl,iru_given_others from '||S_REPORT_VIEW_NAME||' a

inner join temp_entity_inside_geom m on a.cable_id=m.system_id
and upper(m.entity_type)=upper('''||s_layer_name||''') where 1=1 ';

RAISE INFO '123_%', sql;

ELSE
-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'cable_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||'
from (select circle_name ,major_route_name,route_id,section_name,section_id,route_category,distance,
fiber_pairs_laid,total_used_pair,fiber_pairs_used_by_vil,fiber_pairs_given_to_airtel,fiber_pairs_given_to_others,
fiber_pairs_free,faulty_fiber_pairs,start_latitude,start_longitude,end_latitude,end_longitude,count_non_vil_tenancies_on_route,
route_lit_up_date,aerial_km,avg_loss_per_km,avg_last_six_months_fiber_cut,material,execution,row_availablity,
iru_given_airtel,iru_given_jio,iru_given_ttsl_or_ttml,network_category,row_valid_or_exp,remarks,cable_owner,route_type,
operator_type,fiber_type, cable_id,iru_given_tcl,iru_given_others from '||S_REPORT_VIEW_NAME||' a ';

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
sql := sql ||' AND cast(a.created_by_id as integer) in ('||p_userids||')';
else
P_PARENTUSERS := Replace(P_PARENTUSERS,'0,','');
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
RAISE INFO 'queryS_%', sql;
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
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;
$BODY$;











CREATE OR REPLACE FUNCTION public.fn_sync_layer_columns(
	)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

 declare arow record;
begin
---------------------------------------
---------- SYNC REPORT COLUMNS---------
---------------------------------------
-- ADD A COLUMN IF ADDED INTO VIEW..
-- ADD A UNION  VIEW FOR ADDITIONAL ATTRIBUTES
insert into layer_columns_settings (layer_id,setting_type,column_name,column_sequence,display_name,table_name,is_active,created_by)
select l.layer_id,'Report'::character varying as setting_type, l.column_name,0 as column_sequence,initcap(replace(l.column_name,'_',' ')),l.report_view_name,true as is_active,1 as created_by  from 	  layer_columns_settings s right join (
	select l.layer_id,'Report'::character varying as setting_type,c.column_name,l.report_view_name from layer_details l
	inner join information_schema.columns c on upper(l.report_view_name)=upper(c.table_name) and upper(c.table_schema) = upper('public')
	where (is_report_enable=true  and isvisible = true) or upper(layer_name) in ('PIT','RACK','EQUIPMENT','PROVINCE') 	
	) l
	on s.layer_id=l.layer_id and upper(s.setting_type)=upper(l.setting_type) and s.column_name=l.column_name
	where s.id is null and upper(l.column_name) not in ('RECORD_SYSTEM_ID','SYSTEM_ID');

--DELETE COLUMNS IF REMOVED FROM VIEW.
	
delete from layer_columns_settings where id in (
	select id from layer_columns_settings s left join (
		select l.layer_id,'Report'::character varying as setting_type,c.column_name,l.report_view_name from layer_details l
		inner join information_schema.columns c on upper(l.report_view_name)=upper(c.table_name) and upper(c.table_schema) = upper('public')
		where (is_report_enable=true  and isvisible = true) or upper(layer_name)  in ('PIT','RACK','EQUIPMENT','PROVINCE')		
		) l
		on s.layer_id=l.layer_id and  upper(l.setting_type)=upper(s.setting_type) and s.column_name=l.column_name
	where upper(s.setting_type)='REPORT' and l.layer_id is null and s.layer_id not in(select layer_id from layer_details where upper(layer_name) in ('ROW','PIT')) ); 


-- ADD A UNION WITH OI(Other_Info) VIEW FOR ADDITIONAL ATTRIBUTES
insert into layer_columns_settings (layer_id,setting_type,column_name,column_sequence,display_name,table_name,is_active,created_by)
select s.id,l.layer_id,'OIReport'::character varying as setting_type, l.column_name,0 as column_sequence,
initcap(replace(l.column_name,'_',' ')),l.additional_report_view_name,true as is_active,1 as created_by  from 	 layer_columns_settings s
right join (	
	select l.layer_id,'OIReport'::character varying as setting_type,c.column_name,l.additional_report_view_name from layer_details l
	inner join information_schema.columns c on upper(l.additional_report_view_name)=upper(c.table_name) and upper(c.table_schema) = upper('public')
	where ((is_report_enable=true  and isvisible = true) or upper(layer_name) in ('PIT','RACK','EQUIPMENT','PROVINCE'))
	) l
	on s.layer_id=l.layer_id and upper(s.setting_type)=upper(l.setting_type) and s.column_name=l.column_name
	where --s.id is null and 
    upper(l.column_name) not in ('RECORD_SYSTEM_ID','SYSTEM_ID');
    
    
    

--DELETE COLUMNS IF REMOVED FROM VIEW.
	
delete from layer_columns_settings where id in (
	select id from layer_columns_settings s left join (
		
	select l.layer_id,'OIReport'::character varying as setting_type,c.column_name,l.report_view_name from layer_details l
	inner join information_schema.columns c on upper(l.additional_report_view_name)=upper(c.table_name) and upper(c.table_schema) = upper('public')
	where ((is_report_enable=true  and isvisible = true) or upper(layer_name) in ('PIT','RACK','EQUIPMENT','PROVINCE'))
		) l
		on s.layer_id=l.layer_id and  upper(l.setting_type)=upper(s.setting_type) and s.column_name=l.column_name
	where upper(s.setting_type)='OIReport' and l.layer_id is null and s.layer_id not in(select layer_id from layer_details where upper(layer_name) in ('ROW','PIT')) ); 



-----------------END-----------------------

---------------------------------------------
---------SYNC INFO SETTING COLUMNS---------
---------------------------------------------
-- ADD A COLUMN IF ADDED INTO VIEW..
-- ADD A UNION WITH OI(Other_Info) VIEW FOR ADDITIONAL ATTRIBUTES
insert into layer_columns_settings (layer_id,setting_type,column_name,column_sequence,display_name,table_name,is_active,created_by)
select l.layer_id,'Info'::character varying as setting_type, l.column_name,0 as column_sequence,initcap(replace(l.column_name,'_',' ')),l.report_view_name,true as is_active,1 as created_by  from 	  layer_columns_settings s right join (
	select l.layer_id,'Info'::character varying as setting_type,c.column_name,l.report_view_name from layer_details l
	inner join information_schema.columns c on upper(l.report_view_name)=upper(c.table_name) and upper(c.table_schema) = upper('public')
	where (is_info_enabled=true and isvisible = true) or upper(layer_name)  in ('PIT','RACK','EQUIPMENT')
	union 
	select l.layer_id,'Info'::character varying as setting_type,c.column_name,l.report_view_name from layer_details l
	inner join information_schema.columns c on upper(replace(l.report_view_name,'report','oi'))=upper(c.table_name) and upper(c.table_schema) = upper('public')
	where ((is_report_enable=true  and isvisible = true) or upper(layer_name) in ('PIT','RACK','EQUIPMENT'))
	) l
	on s.layer_id=l.layer_id and upper(s.setting_type)=upper(l.setting_type) and s.column_name=l.column_name
	where s.id is null and upper(l.column_name) not in ('RECORD_SYSTEM_ID','SYSTEM_ID');

--DELETE COLUMNS IF REMOVED FROM VIEW.
	
delete from layer_columns_settings where id in (
	select id from layer_columns_settings s left join (
		select l.layer_id,'Info'::character varying as setting_type,c.column_name,l.layer_view from layer_details l
		inner join information_schema.columns c on upper(l.report_view_name)=upper(c.table_name) and upper(c.table_schema) = upper('public')
		where (is_info_enabled=true and isvisible = true) or upper(layer_name)  in ('PIT','RACK','EQUIPMENT')
		union 
	select l.layer_id,'Info'::character varying as setting_type,c.column_name,l.report_view_name from layer_details l
	inner join information_schema.columns c on upper(replace(l.report_view_name,'report','oi'))=upper(c.table_name) and upper(c.table_schema) = upper('public')
	where ((is_report_enable=true  and isvisible = true) or upper(layer_name) in ('PIT','RACK','EQUIPMENT'))
		) l
		on s.layer_id=l.layer_id and  upper(l.setting_type)=upper(s.setting_type) and s.column_name=l.column_name
	where upper(s.setting_type)='INFO' and l.layer_id is null); 

-----------------END-----------------------

---------------------------------------------
----SYNC HISTORY  COLUMNS-----
---------------------------------------------
-- ADD A COLUMN IF ADDED INTO VIEW..
-- ADD A UNION WITH OI(Other_Info) VIEW FOR ADDITIONAL ATTRIBUTES
insert into layer_columns_settings (layer_id,setting_type,column_name,column_sequence,display_name,table_name,is_active,created_by)
select l.layer_id,'History'::character varying as setting_type, l.column_name,0 as column_sequence,initcap(replace(l.column_name,'_',' ')),l.HISTORY_VIEW_NAME,true as is_active,1 as created_by  from 	  layer_columns_settings s right join (
	select l.layer_id,'History'::character varying as setting_type,c.column_name,l.HISTORY_VIEW_NAME from layer_details l
	inner join information_schema.columns c on upper(l.HISTORY_VIEW_NAME)=upper(c.table_name) and upper(c.table_schema) = upper('public')
	where (is_history_enabled=true  and isvisible = true) or upper(layer_name)  in ('PIT','RACK','EQUIPMENT')
	union
	select l.layer_id,'History'::character varying as setting_type,c.column_name,l.HISTORY_VIEW_NAME from layer_details l
	inner join information_schema.columns c on upper(replace(l.report_view_name,'report','oi'))=upper(c.table_name) and upper(c.table_schema) = upper('public')
	where (is_history_enabled=true  and isvisible = true) or upper(layer_name)  in ('PIT','RACK','EQUIPMENT')
	) l
	on s.layer_id=l.layer_id and upper(s.setting_type)=upper(l.setting_type) and s.column_name=l.column_name
	where s.id is null and upper(l.column_name) not in ('RECORD_SYSTEM_ID','SYSTEM_ID','AUDIT_ID');

--DELETE COLUMNS IF REMOVED FROM VIEW.
	

delete from layer_columns_settings where id in (
	select id from layer_columns_settings s left join (
		select l.layer_id,'History'::character varying as setting_type,c.column_name,l.report_view_name from layer_details l
		inner join information_schema.columns c on upper(l.HISTORY_VIEW_NAME)=upper(c.table_name) and upper(c.table_schema) = upper('public')
		where (is_history_enabled=true  and isvisible = true)or upper(layer_name)  in ('PIT','RACK','EQUIPMENT')
		union 
	select l.layer_id,'History'::character varying as setting_type,c.column_name,l.report_view_name from layer_details l
		inner join information_schema.columns c on upper(replace(l.report_view_name,'report','oi'))=upper(c.table_name) and upper(c.table_schema) = upper('public')
		where (is_report_enable=true  and isvisible = true)or upper(layer_name)  in ('PIT','RACK','EQUIPMENT')
		) l
		on s.layer_id=l.layer_id and  upper(l.setting_type)=upper(s.setting_type) and s.column_name=l.column_name
	where upper(s.setting_type)='HISTORY' and l.layer_id is null and s.layer_id not in(select layer_id from layer_details where upper(layer_name)in ('ROW','PIT'))); 

-----------------END-----------------------
--select * from fn_row_sync_report_columns();
	
    return true;
end;
$BODY$;


















CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary_view_csv_additional(
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
Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';
select value into v_csv_delimeter  from global_settings where key ilike 'CsvDelimiter';
SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, ADDITIONAL_REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
IF ((p_geom) != '') THEN

--select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
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

-- SELECT ALL ACTIVE FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
if(p_pageno <>0)-- p_pageno 0 denotes when we downloading report and we required columns with keys
then
SELECT STRING_AGG(COLUMN_NAME,',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)=upper('OIReport') AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
else
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)=upper('OIReport') AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
end if;
-- SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG('replace('''||COLUMN_NAME||'''::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)=upper('OIReport') ORDER BY st.COLUMN_SEQUENCE) A;

SELECT STRING_AGG('REPLACE(COALESCE('''||COLUMN_NAME||'''::text,''''),E''\n'','''')','||''|''||') INTO s_layer_csv_columns FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)=upper('OIReport') AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

RAISE INFO 'S_LAYER_COLUMNS---%', S_LAYER_COLUMNS;
RAISE INFO 'P_SORTCOLNAME%', P_SORTCOLNAME;

sql:='SELECT 0 as S_No,STRING_AGG((case when coalesce(res_field_key,'''') ='''' then DISPLAY_NAME else res_field_key end)||'''||v_csv_delimeter||''','''') 
FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(''en'')
WHERE st.LAYER_ID='||S_LAYER_ID||' AND UPPER(st.SETTING_TYPE)=''OIREPORT'' AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A union all ';

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
--FETCH RECORD BASED ON SELECTED GEOMETRY.
sql:= sql || 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'record_system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||s_layer_csv_columns||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a

inner join temp_entity_inside_geom m on a.record_system_id=m.system_id
and upper(m.entity_type)=upper('''||s_layer_name||''') where 1=1 ';

RAISE INFO '123_%', sql;

ELSE

-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= sql || 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'record_system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||s_layer_csv_columns||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a ';

if exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
then 
sql:= sql ;-- ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';
else 
sql:= sql ;-- ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';
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

IF ((p_route) != '') THEN

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.record_system_id and ass.cable_id in('||p_route||')';

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
RAISE INFO 'queryS_%', sql;
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
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;
$BODY$;




CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary_view_additional(
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

Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';

SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'PLANNED', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'AS BUILT', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'DORMANT', 'gi') into p_networkstatues;
-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, ADDITIONAL_REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME) and is_dynamic_control_enable = true ;
IF ((p_geom) != '') THEN
--select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
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

-- SELECT ALL ACTIVE FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
if(p_pageno <>0)-- p_pageno 0 denotes when we downloading report and we required columns with keys
then
SELECT STRING_AGG(COLUMN_NAME,',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)=upper('OIReport') AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
else
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)=upper('OIReport') AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
end if;
-- SELECT ALL FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)=upper('OIReport') ORDER BY st.COLUMN_SEQUENCE) A;

RAISE INFO 'S_LAYER_COLUMNS---%', S_LAYER_COLUMNS;
RAISE INFO 'P_SORTCOLNAME%', P_SORTCOLNAME;

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN
--FETCH RECORD BASED ON SELECTED GEOMETRY.
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'record_system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a

inner join temp_entity_inside_geom m on a.record_system_id=m.system_id
and upper(m.entity_type)=upper('''||s_layer_name||''') where 1=1 ';

RAISE INFO '123_%', sql;

ELSE
-- FETCH RECORDS BASED ON SELECTED FILTERS.
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'record_system_id' ELSE P_SORTCOLNAME END
||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No, '||S_LAYER_COLUMNS||'
from (select a.* from '||S_REPORT_VIEW_NAME||' a ';

if exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
then 
--sql:= sql ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';
else 
--sql:= sql ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';
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

IF ((p_route) != '') THEN

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.record_system_id and ass.cable_id in('||p_route||')';

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
sql := sql ||' AND cast(a.created_by_id as integer) in ('||p_userids||')';
else
P_PARENTUSERS := Replace(P_PARENTUSERS,'0,','');
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
RAISE INFO 'queryS_%', sql;
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
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;
$BODY$;





DROP VIEW public.vw_att_details_cable_cdb_attributes;

CREATE OR REPLACE VIEW public.vw_att_details_cable_cdb_attributes
 AS
 SELECT cdb.circle_name,
    cdb.major_route_name,
    cdb.route_id,
    cdb.section_name,
    cdb.section_id,
    cdb.route_category,
    cdb.distance,
    cdb.fiber_pairs_laid,
    cdb.total_used_pair,
    cdb.fiber_pairs_used_by_vil,
    cdb.fiber_pairs_given_to_airtel,
    cdb.fiber_pairs_given_to_others,
    cdb.fiber_pairs_free,
    cdb.faulty_fiber_pairs,
    cdb.start_latitude,
    cdb.start_longitude,
    cdb.end_latitude,
    cdb.end_longitude,
    cdb.count_non_vil_tenancies_on_route,
    cdb.route_lit_up_date,
    cdb.aerial_km,
    cdb.avg_loss_per_km,
    cdb.avg_last_six_months_fiber_cut,
    cdb."row",
    cdb.material,
    cdb.execution,
    cdb.row_availablity,
    cdb.iru_given_airtel,
    cdb.iru_given_jio,
    cdb.iru_given_ttsl_or_ttml,
    cdb.network_category,
    cdb.row_valid_or_exp,
    cdb.remarks,
    cdb.cable_owner,
    cdb.route_type,
    cdb.operator_type,
    cdb.fiber_type,
    cdb.cable_id,
    cdb.iru_given_tcl,
    cdb.iru_given_others,
    adc.network_id,
    adc.region_id,
    adc.province_id
   FROM att_details_cable_cdb cdb
     JOIN att_details_cable adc ON adc.system_id = cdb.cable_id;

	 alter table layer_details add column additional_report_view_name character varying(100);
update layer_details set additional_report_view_name ='vw_att_details_pole_oi' ,
is_dynamic_control_enable= true where layer_id =14;

update layer_details set additional_report_view_name ='vw_att_details_cable_oi' ,
is_dynamic_control_enable= true where layer_id =19;

update layer_details set additional_report_view_name ='vw_att_details_bdb_oi' ,
is_dynamic_control_enable= true where layer_id =07;