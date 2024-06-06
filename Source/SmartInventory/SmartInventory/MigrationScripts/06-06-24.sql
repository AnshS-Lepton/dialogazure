CREATE OR REPLACE FUNCTION public.fn_get_export_report_summary(p_regionids character varying, p_provinceids character varying, p_networkstatues character varying, p_parentusers character varying, p_userids character varying, p_layerids character varying, p_projectcodes character varying, p_planningcodes character varying, p_workordercodes character varying, p_purposecodes character varying, p_durationbasedon character varying, p_fromdate character varying, p_todate character varying, p_geom character varying, p_userid integer, p_roleid integer, p_is_all_provience_assigned boolean, p_ownership_type character varying, p_thirdparty_vendor_ids character varying, p_radious double precision, p_route character varying)
 RETURNS TABLE(entity_id integer, entity_title character varying, entity_name character varying, planned_count integer, as_built_count integer, dormant_count integer)
 LANGUAGE plpgsql
AS $function$


 

Declare v_arow record;

sql TEXT;
s_layer_columns_val text;
columnquery TEXT;
s_TempSql_columns_val text;
query_value_geom text;
query_value_summary text;
Begin

SELECT regexp_replace(p_networkstatues, '\mP(s?)\M', 'P', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mA(s?)\M', 'A', 'gi') into p_networkstatues;
SELECT regexp_replace(p_networkstatues, '\mD(s?)\M', 'D', 'gi') into p_networkstatues;

-- Create temp table tempExportReportSummary
-- (
--     entity_id integer,
--     entity_title character varying,
--     entity_name character varying,
--     planned_count integer,
--     as_built_count integer,
--     dormant_count integer
-- )on commit drop;

IF ((p_geom) != '')
THEN
if(p_radious>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10)  into p_geom;
end if;               



query_value_geom ='select system_id,entity_type from Polygon_master m where  ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)	
	union 
	
	select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)
	union 
	
	select system_id,entity_type from point_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)';

if(coalesce(p_layerids::character varying,'')='') or exists (select 1 from layer_details ld where layer_id in (select unnest(string_to_array(p_layerids::character varying, ','))::integer) and upper(layer_name)='CABLE')
then
query_value_geom := '
select system_id,entity_type from Polygon_master m where  ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)	
	union 
	
	select system_id,entity_type from line_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true  
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)
	union 
	
	select system_id,entity_type from point_master m where   ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	and upper(m.entity_type) in 
	(select upper(layer_name) from LAYER_DETAILS where isvisible = true and is_report_enable = true
    and case when coalesce('''|| p_layerids::character varying||''','''')!='''' then  (layer_id) in (select regexp_split_to_table((select unnest(string_to_array('''|| p_layerids::character varying||''', '''',''''))), E'','')::integer) else true end)

union 
     select cbl.system_id as system_id,  ''''  as entity_type   from att_details_cable cbl
	inner join point_master pm on  ST_Intersects(pm.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326))  
    and pm.system_id=cbl.structure_id and upper(pm.entity_type)=upper(''structure'') 
    and cbl.cable_type=''ISP'' ' ;
end if;

raise info 'queryyyy: %', query_value_geom;

 

end if;

 

for v_arow in select distinct L.LAYER_ID,L.LAYER_TITLE, L.REPORT_VIEW_NAME, L.GEOM_TYPE,L.layer_name,L.layer_table from LAYER_DETAILS L

inner join role_permission_entity pe on pe.layer_id = L.Layer_Id

where   pe.role_id = p_roleid and pe.viewonly = true and L.isvisible = true and L.is_report_enable = true 
and case when coalesce(p_layerids,'')!='' then  (L.layer_id) in (select unnest(string_to_array(p_layerids, ','))::integer)
else true end order by L.layer_name

  loop

sql := '';

 

-- FETCH ALL REPORT COLUMNS FROM LAYER COLUMN SETTINGS TABLE

SELECT STRING_AGG(COLUMN_NAME, ',') INTO S_LAYER_COLUMNS_VAL FROM (

SELECT  COLUMN_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=v_arow.LAYER_ID  AND UPPER(SETTING_TYPE)='REPORT'

ORDER BY COLUMN_SEQUENCE) A;

 

 

IF(upper(S_LAYER_COLUMNS_VAL) like '%NETWORK_STATUS%')

THEN

-- SHOW NETWORK STATUS COUNT SEPERATLY --

columnquery :='cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,

cast('''||v_arow.layer_name||'''as text)entityName,coalesce(sum(case when a.network_status=''P'' then 1 else 0 end),0)

as planned_count,coalesce(sum(case when a.network_status=''A'' then 1 else 0 end),0) as asbuilt_count,

coalesce(sum(case when a.network_status=''D'' then 1 else 0 end),0) as dorment_count';

ELSE

-- IF NETWORK STATUS COLUMN IS NOT DEFINED FOR LAYER THEN SHOW THE ALL COUNT INTO AS-BUILT.

columnquery :='cast('''||v_arow.layer_id||''' as integer)entityid,cast('''||v_arow.layer_title||'''as text)entitytitle,

cast('''||v_arow.layer_name||'''as text)entityName,CAST(0 AS INTEGER) as planned_count,coalesce(COUNT(a.SYSTEM_ID),0)

as asbuilt_count,CAST(0 AS INTEGER)as dorment_count';

END IF;

 

    

--DYNAMIC QUERY TO FETCH ENTITY SUMMARY

IF ((p_geom) != '') THEN

 

--FETCH RECORD BASED ON SELECTED GEOMETRY.

sql:= 'select '||columnquery||' from '|| v_arow.layer_table||' a inner join ( ' || query_value_geom || ' ) m

on a.system_id=m.system_id and upper(m.entity_type)=upper('''||v_arow.layer_name||''')

JOIN (select user_id AS created_by_id, * from user_master) um ON a.created_by = um.user_id ';

else

 

-- FETCH RECORDS BASED ON SELECTED FILTERS.

sql:= 'select '||columnquery||' from '|| v_arow.layer_table||' a';

 

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

IF ((p_route) != '') THEN

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.system_id and ass.cable_id in('||p_route||')';

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

if not exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)

and not exists(select 1 from user_module_mapping m

inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)

and p_roleid > 1

then

if(p_parentusers != '' and p_parentusers != '0') THEN

 

if(p_userids != '')THEN

sql := sql ||' AND cast('||' a.created_by' ||' as integer) in ('||p_userids||')';

else

P_PARENTUSERS := Replace(P_PARENTUSERS,'0,','');
sql := sql ||' AND cast('|| ' a.created_by'  ||' as integer) in (select * from fn_get_report_mapped_users('''||p_parentusers||'''))';

END IF;

 

END IF;

END IF;

 

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

-- DURATION FILTER 

 

IF(P_fromDate != '' and P_toDate != '') THEN

sql:= sql ||' AND a.'||p_durationbasedon||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and

a.'||p_durationbasedon||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

 

-- OWNER SHIP FILTER

IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN

sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''')';

else IF ((p_ownership_type) != '') then

sql:= sql ||' AND  0 = 1';

END IF;

end if;

  raise notice 'sql: % ', sql;  

IF ((p_thirdparty_vendor_ids) != '' and upper(S_LAYER_COLUMNS_VAL) like '%THIRD_PARTY_VENDOR_ID%') THEN

sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''') AND a.third_party_vendor_id in('||p_thirdparty_vendor_ids||')';

else IF ((p_thirdparty_vendor_ids) != '') then

sql:= sql ||' AND  0 = 1';

end if;

end if;
s_TempSql_columns_val:='';
 s_TempSql_columns_val:= 'Select entityid, entitytitle:: character varying ,entityName:: character varying, CAST(sum(planned_count) AS INTEGER) AS planned_count ,
 CAST(sum(asbuilt_count) AS INTEGER) AS asbuilt_count ,CAST(sum(dorment_count) AS INTEGER) AS dorment_count from( '||sql||')result 
 where (planned_count + asbuilt_count + dorment_count) > 0 group by entityid,entitytitle,entityName';

    raise notice 'checking query: % ', s_TempSql_columns_val;  
    RETURN QUERY execute s_TempSql_columns_val;
end loop;

 

End;

 

$function$
;


update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='WallMount')
and associate_layer_id=(select layer_id from layer_details where layer_name='CDB');
update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Pole')
and associate_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure');

update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Pole')
and associate_layer_id=(select layer_id from layer_details where layer_name='BDB');
update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Pole')
and associate_layer_id=(select layer_id from layer_details where layer_name='FDB');
update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Tree')
and associate_layer_id=(select layer_id from layer_details where layer_name='ADB');
update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Tree')
and associate_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure');
update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Tree')
and associate_layer_id=(select layer_id from layer_details where layer_name='FDB');
update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Tree')
and associate_layer_id=(select layer_id from layer_details where layer_name='BDB');
update associate_entity_master set is_shifting_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Tree')
and associate_layer_id=(select layer_id from layer_details where layer_name='CDB');


CREATE OR REPLACE FUNCTION public.fn_delete_entity(p_system_id integer, p_entity_type character varying, p_geom_type character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE 
v_layer_table character varying;
v_is_parent_exist boolean;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
V_LAYER_TITLE character varying;
v_parent_system_id integer;
v_building_system_id integer;
v_building_code character varying;
BEGIN
--GET THE LAYER TITLE
SELECT layer_title into V_LAYER_TITLE FROM layer_details where upper(layer_name)=upper(p_entity_type);

V_IS_VALID:=TRUE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_065]');--(V_LAYER_TITLE||' has deleted successfully!');
--CHECK THE STRUCTURE CHILD EXIST
if(upper(p_entity_type)='STRUCTURE' and (select count(1) from isp_entity_mapping where structure_id=p_system_id)>0)
then	
V_IS_VALID=FALSE;
V_MESSAGE:=('[SI_GBL_GBL_GBL_GBL_051]');--Structure can not be deleted as there are some dependent elements!		


--CHECK THE SPLICING(EXCLUDING THE INERNAL CONNECTIVITY OF THE DEVICE)
elsIF EXISTS(select 1 from connection_info 
where ((source_system_id=p_system_id and upper(source_entity_type)=upper(p_entity_type)) or ( destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type)))
and ( ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))))
THEN 
	v_is_parent_exist=false;
	V_IS_VALID=FALSE;   

	IF EXISTS(SELECT 1 FROM CONNECTION_INFO where ((source_system_id=p_system_id and upper(source_entity_type)=upper(p_entity_type)) or ( destination_system_id=p_system_id and upper(destination_entity_type)=upper(		p_entity_type))) )
	THEN	
		V_IS_VALID=FALSE;
		V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!');       
	END IF;
	
	IF(UPPER(p_entity_type)='SPLITTER') 
	AND EXISTS(SELECT 1 FROM CONNECTION_INFO CON JOIN ATT_DETAILS_SPLITTER SPL 
	ON  SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE') 
	OR (CON.DESTINATION_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.DESTINATION_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.SOURCE_ENTITY_TYPE)='CABLE')) )
	THEN
	v_is_parent_exist=true;	
	V_IS_VALID=FALSE;
	V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 
        ELSIF(UPPER(p_entity_type)='SPLITTER')THEN	
	v_is_parent_exist=TRUE;
	END IF;
        
	IF(UPPER(p_entity_type)='SPLICECLOSURE') 
	AND EXISTS(SELECT 1 FROM CONNECTION_INFO CON JOIN ATT_DETAILS_SPLICECLOSURE SPL 
	ON SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE') 
	OR (CON.DESTINATION_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.DESTINATION_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.SOURCE_ENTITY_TYPE)='CABLE')))
	THEN	
        v_is_parent_exist=true;
        V_IS_VALID=FALSE;
	V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 
	
	ELSIF((SELECT PARENT_SYSTEM_ID FROM ATT_DETAILS_SPLICECLOSURE SPL WHERE SPL.SYSTEM_ID=p_system_id and UPPER(p_entity_type)='SPLICECLOSURE')!=0)THEN	
	v_is_parent_exist=true;
	END IF;

	IF(v_is_parent_exist=false)
	then
		V_IS_VALID=FALSE;
	V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 
		
	end if;			
--CHECK THE ENTITY ASSOCIATION FIRST
elsif exists(select 1 from  associate_entity_info asso where ((asso.associated_system_id=p_system_id and upper(asso.associated_entity_type)=UPPER(p_entity_type)) or 
(asso.entity_system_id=p_system_id and upper(asso.entity_type)=UPPER(p_entity_type))) and asso.is_termination_point=false)
then	
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_152]');--(V_LAYER_TITLE||'can not be deleted as it is associated with some entity!'); 
	
--CHECK THE TERMINATION POINT DUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('duct') and is_enabled=true) 
and exists(select  1 from att_details_duct duct where (duct.a_system_id=p_system_id and upper(duct.a_entity_type)=upper(p_entity_type)) 
or (duct.b_system_id=p_system_id and upper(duct.b_entity_type)=upper(p_entity_type)))
then	
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_053]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with duct!');	
		
--CHECK THE TERMINATION POINT TRENCH
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('trench') and is_enabled=true) 
and exists(select  1 from att_details_trench trench where (trench.a_system_id=p_system_id and upper(trench.a_entity_type)=upper(p_entity_type)) 
or (trench.b_system_id=p_system_id and upper(trench.b_entity_type)=upper(p_entity_type)))
then   
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_054]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with trench!');	
		
--CHECK THE TERMINATION POINT CABLE
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('cable') and is_enabled=true) 
  and exists(select 1 from att_details_cable cable where (cable.a_system_id=p_system_id and upper(cable.a_entity_type)=upper(p_entity_type)) 
  or (cable.b_system_id=p_system_id and upper(cable.b_entity_type)=upper(p_entity_type)))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_055]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with cable!');

--CHECK THE TERMINATION POINT MICRODUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('microduct') and is_enabled=true) 
and exists(select  1 from att_details_microduct microduct where (microduct.a_system_id=p_system_id and upper(microduct.a_entity_type)=upper(p_entity_type)) 
or (microduct.b_system_id=p_system_id and upper(microduct.b_entity_type)=upper(p_entity_type)))
then	
V_IS_VALID=FALSE;
V_MESSAGE:=(V_LAYER_TITLE||' can not be deleted as it is used as termination point with microduct!'); --//have change the message for microduct	
	

elsif(upper(p_entity_type)='ROW') 
and exists(select 1 from att_details_row_apply where row_system_id=p_system_id) 
and not exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id)
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_056]');--(V_LAYER_TITLE||' can not be deleted as it is applied!');

elsif(upper(p_entity_type)='ROW') 
and exists(select 1 from att_details_row_apply where row_system_id=p_system_id) 
and  exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id and  upper(row_status)=upper('Approved'))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_057]');--(V_LAYER_TITLE||' can not be deleted as it has been Approved!');
elsif(upper(p_entity_type)='PIT')
then			
	select parent_system_id into v_parent_system_id from att_details_row_pit where system_id=p_system_id;

	IF EXISTS(SELECT 1 FROM ATT_DETAILS_ROW_APPLY WHERE ROW_SYSTEM_ID=v_parent_system_id) 
	THEN
		V_IS_VALID=FALSE;
		V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as  ROW has been Applied!');	
	END IF;
ELSIF(upper(p_entity_type)='CABLE') AND EXISTS(SELECT 1 FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=p_system_id)
then

	V_IS_VALID=FALSE;
	V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as LMC is associated!');
ELSIF(upper(p_entity_type)='CABLE')
then
delete from att_details_loop where cable_system_id= p_system_id;

end if;

IF(V_IS_VALID)
THEN
	IF(UPPER(p_entity_type)=UPPER('SUBAREA')) 
	THEN
		select building_system_id,building_code into v_building_system_id,v_building_code from att_details_subarea where system_id=p_system_id;
		delete from att_details_building where system_id=v_building_system_id and network_id=v_building_code;
		delete from polygon_master where system_id=v_building_system_id and upper(entity_type)=upper('Building'); 
	END IF;

	IF(upper(p_entity_type)='FMS')
	THEN
		DELETE  FROM ATT_DETAILS_MODEL B  
		USING ATT_DETAILS_FMS C 
		WHERE UPPER(B.NETWORK_ID)=UPPER(C.NETWORK_ID)
		AND C.SYSTEM_ID=P_SYSTEM_ID;     
	END IF;

	perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,2,true));
	
	select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
	raise info'%v_layer_table',v_layer_table;
	raise info'%p_entity_type',p_entity_type;
	IF(upper(p_entity_type)='NETWORK_TICKET')
	THEN
	execute  'delete from '||v_layer_table||' where ticket_id='||p_system_id||'';
	ELSE
	execute  'delete from '||v_layer_table||' where system_id='||p_system_id||'';
	END IF;
	execute 'delete from '||p_geom_type||'_master where system_id='||p_system_id||' and upper(entity_type)=upper('''||p_entity_type||''')';
	delete from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type);
	delete from isp_entity_mapping where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);
	delete from isp_port_info where parent_system_id=p_system_id and upper(parent_entity_type)=upper(p_entity_type);
	delete from associate_entity_info where (associated_system_id=p_system_id 
	and upper(associated_entity_type)=upper(p_entity_type)) or (entity_system_id=p_system_id and upper(entity_type)=upper(p_entity_type)) and is_termination_point=true;
	delete from connection_info  where (source_system_id=p_system_id 
	and upper(source_entity_type)=upper(p_entity_type)) or (destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type));
	delete from isp_line_master  where entity_id=p_system_id  and upper(entity_type)=upper(p_entity_type);
	delete from circle_master  where system_id=p_system_id  and upper(entity_type)=upper(p_entity_type);

	IF EXISTS (select 1 from layer_details where upper(layer_name)=upper(p_entity_type) and is_reference_allowed=true) THEN
	BEGIN
		delete from att_entity_reference where system_id=p_system_id;
	END;

	IF(UPPER(p_entity_type)=UPPER('CUSTOMER'))
	THEN
		delete from wcr_mapping where CUSTOMER_ID=p_system_id;
	END IF;

	
	
	END IF;
	
	RETURN QUERY 
	SELECT true AS STATUS, fn_Multilingual_Message_Convert('en',Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_065]')::CHARACTER VARYING)::CHARACTER VARYING AS MESSAGE ; --V_LAYER_TITLE||' has deleted successfully!')
ELSE
RETURN QUERY 
	SELECT V_IS_VALID AS STATUS, fn_Multilingual_Message_Convert('en',V_MESSAGE::CHARACTER VARYING) ::CHARACTER VARYING AS MESSAGE;
END IF;
	
  
END
$function$
;

update res_resources set value='Service Cost Per' where key='SI_OSP_GBL_NET_RPT_017' and culture='en';