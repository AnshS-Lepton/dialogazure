CREATE OR REPLACE FUNCTION public.fn_merge_cable(mastercableid integer, secondcableid integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
declare
MinDistanceRoute character varying;
MinDistance double precision;
MergeGeometry text;
v_arrow_cable integer;
v_arow record;
sql text;
v_province_id integer;
begin
 

select Route:: character varying ,(Distance):: double precision into MinDistanceRoute,MinDistance from (
select 'S1S2Distance' as Route ,st_distance((select ST_StartPoint(sp_geometry) from line_master lm where system_id =mastercableid
and entity_type='Cable'),
(select ST_StartPoint(sp_geometry) from line_master lm where system_id =secondcableid and entity_type='Cable' ),false) as Distance
union
select 'E1S2Distance' ,st_distance((select ST_EndPoint(sp_geometry) from line_master lm where system_id =mastercableid and entity_type='Cable'),
(select ST_StartPoint(sp_geometry) from line_master lm where system_id =secondcableid and entity_type='Cable'),false)
union
select 'S1E2Distance', st_distance((select ST_StartPoint(sp_geometry) from line_master lm where system_id =mastercableid and entity_type='Cable'),
(select ST_EndPoint(sp_geometry) from line_master lm where system_id =secondcableid and entity_type='Cable'),false)
union
select 'E1E2Distance', st_distance((select ST_EndPoint(sp_geometry) from line_master lm where system_id =mastercableid and entity_type='Cable'),
(select ST_EndPoint(sp_geometry) from line_master lm where system_id =secondcableid and entity_type='Cable'),false) ) T
order by Distance asc limit 1;
raise info 'MainDistanceRoute: %,Distance : %',MinDistanceRoute,MinDistance;



if(MinDistance >(select value::double precision from global_settings gs where key='MergeCableToleranceInMtr'))
then
return query select row_to_json(Result) from (Select 'Distance between 2 cables should be less than 5 meters' as message, 'Error' as status ) result;
else

if(MinDistanceRoute ='S1S2Distance')
then
select st_astext(ST_MakeLine((select st_reverse(sp_geometry) from line_master lm where system_id = secondcableid and entity_type='Cable'),
(select sp_geometry from line_master lm where system_id =mastercableid and entity_type='Cable'))) ::text into MergeGeometry;

elseif (MinDistanceRoute ='S1E2Distance') then
select st_astext(ST_MakeLine((select (sp_geometry) from line_master lm where system_id = secondcableid and entity_type='Cable'),
(select sp_geometry from line_master lm where system_id =mastercableid and entity_type='Cable'))) ::text into MergeGeometry ;

elseif(MinDistanceRoute='E1E2Distance' ) then
select st_astext(ST_MakeLine((select (sp_geometry) from line_master lm where system_id =mastercableid and entity_type='Cable'),
(select st_reverse(sp_geometry) from line_master lm where system_id = secondcableid and entity_type='Cable'))) ::text into MergeGeometry;
else
select st_astext(ST_MakeLine((select (sp_geometry) from line_master lm where system_id =mastercableid and entity_type='Cable'),
(select sp_geometry from line_master lm where system_id = secondcableid and entity_type='Cable' ))) ::text into MergeGeometry ;

end if;

raise info 'MergeGeometry %',(MergeGeometry);

sql:='insert into att_details_cable (network_id, cable_name, a_location, b_location, total_core, no_of_tube, no_of_core_per_tube, cable_measured_length,
cable_calculated_length, cable_type, coreaccess, wavelength, optical_output_power, frequency, attenuation_db, resistance_ohm, construction, activation,
accessibility, specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id, "type", brand, model, network_status, status,
pin_code, province_id, region_id, totalattenuationloss, chromaticdb, chromaticdispersion, totalchromaticloss, remarks, created_by, created_on, modified_by,
modified_on, a_system_id, a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type, sequence_id, duct_id, trench_id, project_id, planning_id,
purpose_id, workorder_id, structure_id, cable_category, loop_count, loop_length, total_loop_length, total_loop_count, route_id, parent_system_id, parent_network_id,
parent_entity_type, start_reading, end_reading, cable_sub_category, execution_method, is_used, barcode, drum_no, utilization, notification_status_id, acquire_from,
circuit_id, thirdparty_circuit_id, ownership_type, third_party_vendor_id, source_ref_type, source_ref_id, source_ref_description, audit_item_master_id, status_remark,
status_updated_by, status_updated_on, is_visible_on_map, primary_pod_system_id, secondary_pod_system_id, is_new_entity, inner_dimension, outer_dimension)
select network_id, cable_name,
case when '''||MinDistanceRoute ||'''=''S1S2Distance'' then (select b_location from att_Details_cable where system_id ='||SecondCableID||')
when '''||MinDistanceRoute ||'''=''S1E2Distance'' then (select a_location from att_Details_cable where system_id ='||SecondCableID||')
when '''||MinDistanceRoute ||'''=''E1E2Distance'' then a_location
else a_location end,
case when '''||MinDistanceRoute ||'''=''E1E2Distance'' then (select a_location from att_Details_cable where system_id ='||SecondCableID||' )
when '''||MinDistanceRoute ||'''=''S1S2Distance'' then b_location
when '''||MinDistanceRoute ||''' =''S1E2Distance'' then b_location
else (select b_location from att_Details_cable where system_id ='||SecondCableID||' ) end,
total_core,no_of_tube,no_of_core_per_tube,
to_char(st_length(ST_transform(ST_GEOMFROMTEXT('''||MergeGeometry||''',4326), utmzone(ST_Centroid(ST_GEOMFROMTEXT('''||MergeGeometry||''',4326))))),
''00000000000.000'')::float ,(cable_calculated_length+(Select cable_calculated_length from att_details_cable where
system_id='||secondCableID||' ))::float, cable_type, coreaccess, wavelength, optical_output_power, frequency, attenuation_db,
resistance_ohm, construction, activation, accessibility, specification, category, subcategory1, subcategory2, subcategory3, item_code,
vendor_id, "type", brand, model, network_status, status, pin_code, province_id, region_id, totalattenuationloss, chromaticdb, chromaticdispersion,
totalchromaticloss, remarks, '||p_user_id||', now(), null, modified_on, 
(case when '''||MinDistanceRoute ||'''=''S1S2Distance'' then (select b_system_id from att_Details_cable where system_id ='||SecondCableID||')
when '''||MinDistanceRoute ||'''=''S1E2Distance'' then (select a_system_id from att_Details_cable where system_id ='||SecondCableID||')
when '''||MinDistanceRoute ||'''=''E1E2Distance'' then a_system_id
else a_system_id end), a_network_id,
(case when '''||MinDistanceRoute ||'''=''S1S2Distance'' then (select b_entity_type from att_Details_cable where system_id ='||SecondCableID||')
when '''||MinDistanceRoute ||'''=''S1E2Distance'' then (select a_entity_type from att_Details_cable where system_id ='||SecondCableID||')
when '''||MinDistanceRoute ||'''=''E1E2Distance'' then a_entity_type
else a_entity_type end), 
(case when '''||MinDistanceRoute ||'''=''E1E2Distance'' then (select a_system_id from att_Details_cable where system_id ='||SecondCableID||' )
when '''||MinDistanceRoute ||'''=''S1S2Distance'' then b_system_id
when '''||MinDistanceRoute ||''' =''S1E2Distance'' then b_system_id
else (select b_system_id from att_Details_cable where system_id ='||SecondCableID||' ) end), b_network_id,
(case when '''||MinDistanceRoute ||'''=''E1E2Distance'' then (select a_entity_type from att_Details_cable where system_id ='||SecondCableID||' )
when '''||MinDistanceRoute ||'''=''S1S2Distance'' then b_entity_type
when '''||MinDistanceRoute ||''' =''S1E2Distance'' then b_entity_type
else (select b_entity_type from att_Details_cable where system_id ='||SecondCableID||' ) end), sequence_id, duct_id, trench_id, project_id, planning_id, purpose_id, workorder_id, structure_id, cable_category, loop_count, loop_length,
total_loop_length, total_loop_count, route_id, parent_system_id, parent_network_id, parent_entity_type, start_reading, end_reading, cable_sub_category,
execution_method, is_used, barcode, drum_no, utilization, notification_status_id, acquire_from, circuit_id, thirdparty_circuit_id, ownership_type, third_party_vendor_id,
source_ref_type, source_ref_id, source_ref_description, audit_item_master_id, status_remark, status_updated_by, status_updated_on, is_visible_on_map, primary_pod_system_id,
secondary_pod_system_id, is_new_entity, inner_dimension, outer_dimension
from att_details_cable where system_id ='||mastercableId ||' RETURNING system_id;';
raise info 'sql %',sql;
execute sql into v_arrow_cable;

sql := 'INSERT INTO public.line_master
(system_id,entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by, approver_id, common_name, db_flag,
approval_date, modified_on, network_status, is_virtual, display_name, modified_by)
select '|| v_arrow_cable||', entity_type, approval_flag, ST_GeomFromText(St_astext('''||MergeGeometry ::character varying||'''),4326), creator_remark, approver_remark, '||p_user_id||', approver_id, common_name, db_flag,
now(), now(), network_status, is_virtual, display_name, null
from line_master where system_id ='||masterCableId||';';
raise info 'sql %',sql;
execute sql;

sql ='INSERT INTO public.att_details_cable_info
(cable_id, tube_number, tube_color, core_number, core_color, fiber_number, fiber_usage_status, physical_utilization, tube_color_code, core_color_code, is_connected, created_by, created_on, modified_by, modified_on, a_end_status_id, b_end_status_id, is_available, core_comment, link_system_id)
Select '|| v_arrow_cable||', tube_number, tube_color, core_number, core_color, fiber_number, fiber_usage_status, physical_utilization, tube_color_code, core_color_code, is_connected, '||p_user_id||', created_on,null, modified_on, 1, 1, is_available,
core_comment, link_system_id from att_details_cable_info where cable_id ='||masterCableId||';';
raise info 'sql %',sql;
execute sql;

select province_id from att_details_cable where system_id =mastercableid into v_province_id;
perform (fn_geojson_update_entity_attribute(v_arrow_cable::integer,'Cable'::character varying ,v_province_id::integer,0,false));

if EXISTS(select 1 from att_details_loop where (cable_system_id =mastercableID ))
then
if exists (select 1 from att_details_loop where (associated_system_id =mastercableID )) then
update att_details_loop set cable_system_id =v_arrow_cable,associated_system_id =v_arrow_cable
where cable_system_id =mastercableID;
else
update att_details_loop set cable_system_id =v_arrow_cable
where cable_system_id =mastercableID;
end if;
end if;

if EXISTS(select 1 from att_details_loop where (cable_system_id =SecondCableID ))
then
if exists (select 1 from att_details_loop where (associated_system_id =SecondCableID )) then
update att_details_loop set cable_system_id =v_arrow_cable,associated_system_id =v_arrow_cable,
associated_network_id = (select network_id from att_details_cable where system_id=mastercableID)

where cable_system_id =SecondCableID;
else
update att_details_loop set cable_system_id =v_arrow_cable,
associated_network_id = (select network_id from att_details_cable where system_id=mastercableID)
where cable_system_id =SecondCableID;
end if;
end if;

--DELETE SPLICING::::----

--By ANTRA--
-- --CABLE TO PARENT CONNECTOR
FOR V_AROW IN
SELECT SOURCE_SYSTEM_ID,SOURCE_ENTITY_TYPE,SOURCE_PORT_NO,DESTINATION_SYSTEM_ID,DESTINATION_ENTITY_TYPE,DESTINATION_PORT_NO,IS_CABLE_A_END FROM CONNECTION_INFO WHERE (SOURCE_SYSTEM_ID IN(MASTERCABLEID,SECONDCABLEID) AND UPPER(SOURCE_ENTITY_TYPE)='CABLE') 
UNION
SELECT DESTINATION_SYSTEM_ID,DESTINATION_ENTITY_TYPE,DESTINATION_PORT_NO,SOURCE_SYSTEM_ID,SOURCE_ENTITY_TYPE,SOURCE_PORT_NO,IS_CABLE_A_END FROM CONNECTION_INFO WHERE (DESTINATION_SYSTEM_ID IN(MASTERCABLEID,SECONDCABLEID) AND UPPER(DESTINATION_ENTITY_TYPE)='CABLE') 
LOOP			
	DELETE FROM CONNECTION_INFO 
	WHERE ((SOURCE_SYSTEM_ID=V_AROW.SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_AROW.SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_AROW.SOURCE_PORT_NO)
	OR (DESTINATION_SYSTEM_ID=V_AROW.SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_AROW.SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_AROW.SOURCE_PORT_NO)); 
	
	
	DELETE FROM CONNECTION_INFO 
	WHERE ((DESTINATION_SYSTEM_ID=V_AROW.DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_AROW.DESTINATION_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_AROW.DESTINATION_PORT_NO)
	OR (SOURCE_SYSTEM_ID=V_AROW.DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_AROW.DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_AROW.DESTINATION_PORT_NO));			
END LOOP;

update att_details_cable_info set a_end_status_id=1,b_end_status_id=1 where cable_id in(mastercableId,SecondCableID);

--END BY ANTRA

IF EXISTS(select 1 from connection_info where ((source_system_id=mastercableId and upper(source_entity_type)=upper('cable'))
or (destination_system_id=mastercableId and upper(destination_entity_type)=upper('cable')))
and (((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))))
THEN
delete from connection_info where (source_system_id=mastercableId and upper(source_entity_type)=upper('cable'))
or (destination_system_id=mastercableId and upper(destination_entity_type)=upper('cable'));
END IF;


IF EXISTS(select 1 from connection_info where ((source_system_id=SecondCableID and upper(source_entity_type)=upper('cable'))
or (destination_system_id=SecondCableID and upper(destination_entity_type)=upper('cable')))
and (((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))))
THEN
delete from connection_info where (source_system_id=SecondCableID and upper(source_entity_type)=upper('cable'))
or (destination_system_id=SecondCableID and upper(destination_entity_type)=upper('cable'));
END IF;



--DELETE ASSOCIATION::::---
IF EXISTS(select 1 from associate_entity_info asso where ((asso.associated_system_id=mastercableId and upper(asso.associated_entity_type)=UPPER('cable'))
OR (asso.entity_system_id=mastercableId and upper(asso.entity_type)=UPPER('cable'))))
THEN
delete from associate_entity_info where (associated_system_id=mastercableId and upper(associated_entity_type)=upper('cable'))
or (entity_system_id=mastercableId and upper(entity_type)=upper('cable'));
END IF;

IF EXISTS(select 1 from associate_entity_info asso where ((asso.associated_system_id=SecondCableID and upper(asso.associated_entity_type)=UPPER('cable'))
OR (asso.entity_system_id=SecondCableID and upper(asso.entity_type)=UPPER('cable'))))
THEN
delete from associate_entity_info where (associated_system_id=SecondCableID and upper(associated_entity_type)=upper('cable'))
or (entity_system_id=SecondCableID and upper(entity_type)=upper('cable'));
END IF;
delete from att_details_cable where system_id in (mastercableId,SecondCableID);
delete from att_details_cable_info where cable_id in (mastercableId,SecondCableID);
delete from cable_geojson_master where system_id in (mastercableId,SecondCableID);

IF EXISTS(SELECT 1 FROM ISP_LINE_MASTER WHERE ENTITY_ID=MASTERCABLEID AND UPPER(ENTITY_TYPE)='CABLE' AND UPPER(CABLE_TYPE)='OSP')
THEN
	UPDATE ISP_LINE_MASTER SET ENTITY_ID=V_ARROW_CABLE WHERE ENTITY_ID=MASTERCABLEID AND UPPER(ENTITY_TYPE)='CABLE'; 
END IF;

IF EXISTS(SELECT 1 FROM ISP_LINE_MASTER WHERE ENTITY_ID=SecondCableID AND UPPER(ENTITY_TYPE)='CABLE' AND UPPER(CABLE_TYPE)='OSP')
THEN
	UPDATE ISP_LINE_MASTER SET ENTITY_ID=V_ARROW_CABLE WHERE ENTITY_ID=SecondCableID AND UPPER(ENTITY_TYPE)='CABLE';
END IF;


return query select row_to_json(Result) from (Select 'Cable has been merged successfully' as message, 'OK' as status ) result;
end if;


END; $function$
;

CREATE OR REPLACE FUNCTION public.fn_nwt_get_ticket_details(p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_userid integer, p_searchfrom timestamp without time zone, p_searchto timestamp without time zone, p_ticket_type_id integer, p_region_id integer, p_province_id integer, p_ticket_status_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
sql TEXT;
NWstatus TEXT;
StartSNo INTEGER;
EndSNo INTEGER;
TotalRecords integer;
v_user_role_id integer;
v_is_admin_rights_enabled boolean;
BEGIN

p_searchby:=replace(replace(p_searchby,'(',''),')','');

select role_id,is_admin_rights_enabled into v_user_role_id,v_is_admin_rights_enabled from user_master where user_id=p_userid;

-- DYNAMIC QUERY
raise info'role_id:-% ',v_user_role_id;
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (p_sortcolname) = '' THEN 'nt.ticket_id' ELSE 'nt.'||p_sortcolname END || ' ' ||CASE WHEN (p_sorttype) ='' THEN 'desc' else p_sorttype end||') AS S_No
,* from vw_att_details_networktickets nt
inner join ticket_type_role_mapping ttrm on nt.ticket_type_id=ttrm.ticket_type_id and ttrm.role_id ='||v_user_role_id||' and ttrm.is_view=true
inner join user_permission_area upa on nt.province_id=upa.province_id and upa.user_id='||p_userid||'
WHERE 1=1';

raise info'sql:-% ',sql;
NWstatus:= 'SELECT tsm.name::character varying as ticket_status,count(nt.ticket_status_id) as ticket_count,tsm.color_code,tsm.opacity from
vw_att_details_networktickets nt right join
ticket_status_master tsm on tsm.id=nt.ticket_status_id
inner join user_permission_area upa on nt.province_id=upa.province_id and upa.user_id='||p_userid||'
';


IF(p_region_id >0 )THEN
sql:= sql ||'and nt.region_id='||p_region_id||'';
NWstatus:= NWstatus ||'and nt.region_id='||p_region_id||'';
END IF;
IF(p_province_id >0 )THEN
sql:= sql ||'and nt.province_id='||p_province_id||'';
NWstatus:= NWstatus ||'and nt.province_id='||p_province_id||'';
END IF;
IF(p_ticket_status_id >0 )THEN
sql:= sql ||'and nt.ticket_status_id='||p_ticket_status_id||'';
NWstatus:= NWstatus ||'and nt.ticket_status_id='||p_ticket_status_id||'';
END IF;

IF(p_ticket_type_id >0 )THEN
sql:= sql ||'and nt.ticket_type_id='||p_ticket_type_id||'';
NWstatus:= NWstatus ||'and nt.ticket_type_id='||p_ticket_type_id||'';
END IF;
raise info'v_user_role_id :-% ',v_user_role_id;

IF(v_user_role_id >0 )THEN
sql:= sql ||' and case when ('||v_user_role_id||' =1 or '||v_is_admin_rights_enabled||'=true) then 1=1 else 1=1 end AND (nt.assigned_to='||p_userid||' OR cast(nt.created_by as integer) in (select user_id from user_master where manager_id in ('||p_userid||') or user_id in ('||p_userid||')))';

NWstatus:= NWstatus ||' and case when ('||v_user_role_id||' =1 or '||v_is_admin_rights_enabled||'=true) then 1=1 else nt.ticket_type_id in (select ticket_type_id from ticket_type_role_mapping where role_id= '||v_user_role_id||' and is_view=true) end';
END IF;

raise info'sqltest% ',sql;

IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
NWstatus:= NWstatus ||' AND lower(nt.'||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;

IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND nt.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and nt.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';
NWstatus:= NWstatus ||' AND nt.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and nt.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';
END IF;
raise info'sql-record:-% ',sql;
-- GET TOTAL RECORD COUNT
EXECUTE 'SELECT COUNT(1) FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
IF((p_pageno) <> 0) THEN
StartSNo:= p_pagerecord * (p_pageno - 1 ) + 1;
EndSNo:= p_pageno * p_pagerecord;
sql:= 'SELECT '||TotalRecords||' as totalRecords, *
FROM (' || sql || ' ) T
WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo;

ELSE
sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T ';
END IF;

RAISE INFO 'lstNWStatus:-%', NWstatus;
RAISE INFO 'lstNWDetails:-%', sql;

RETURN QUERY
EXECUTE 'select row_to_json(row) from (
select(
select array_to_json(array_agg(row_to_json(x)))
from (
'||NWstatus||' where tsm.module=''Network_Ticket'' and is_active=true group by tsm.name,nt.ticket_status_id,tsm.color_code,tsm.action_seq,tsm.opacity order by tsm.action_seq
) x
) as lstNWStatus,
(
select array_to_json(array_agg(row_to_json(x)))
from('||sql||')x

) as lstNWDetails
) row';

END ;
$function$
;

truncate table fiber_link_columns_settings;

INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(1, 'network_id', 1, 'Network Id', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_OSP_GBL_NET_RPT_070', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(2, 'link_id', 2, 'Link/Route Id', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_153', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(3, 'link_name', 3, 'Link/Route Name', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_127', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(4, 'start_point_type', 4, 'Start Point Type', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_128', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(5, 'start_point_network_id', 5, 'Start Point Network Id', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_129', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(6, 'start_point_location', 6, 'Start Point Location', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_130', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(7, 'end_point_type', 7, 'End Point Type', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_131', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(8, 'end_point_network_id', 8, 'End Point Network Id', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_132', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(9, 'end_point_location', 9, 'End Point Location', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_133', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(10, 'no_of_lmc', 10, 'No. Of LMC', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_134', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(15, 'no_of_pair', 15, 'No. Of Pairs', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_141', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(16, 'tube_and_core_details', 16, 'Tube And Core Details', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_142', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(17, 'existing_route_length_otdr', 17, 'Existing Route Length(OTDR)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_143', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(21, 'Any_row_portion', 21, 'Any ROW Portion', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_147', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(22, 'row_authority', 22, 'ROW Authority', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_148', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(23, 'total_row_segments', 23, 'Total ROW Segments', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_149', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(26, 'handover_date', 26, 'Handover Date', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_139', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(27, 'hoto_signoff_date', 27, 'HOTO Signoff Date', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_140', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(29, 'created_by', 29, 'Created By', true, 1, '2020-12-04 18:35:35.757', NULL, NULL, 'SI_OSP_GBL_GBL_GBL_056', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(30, 'created_on', 30, 'Created On', true, 1, '2020-12-04 18:35:35.757', NULL, NULL, 'SI_OSP_GBL_GBL_GBL_055', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(31, 'modified_by', 31, 'Modified By', true, 1, '2020-12-04 18:35:35.757', NULL, NULL, 'SI_OSP_GBL_GBL_GBL_058', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(32, 'modified_on', 32, 'Modified On', true, 1, '2020-12-04 18:35:35.757', NULL, NULL, 'SI_OSP_GBL_GBL_GBL_057', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(33, 'fiber_link_status', 33, 'Fiber Link Status', true, 1, '2020-12-04 18:36:13.511', NULL, NULL, 'SI_OSP_GBL_GBL_GBL_148', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(36, 'link_type', 36, 'Link Type', true, 1, '2021-04-22 16:56:24.164', NULL, NULL, 'SI_OSP_GBL_GBL_GBL_155', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(37, 'service_id', 37, 'Service Id', true, 1, '2021-09-30 12:14:06.870', NULL, NULL, 'SI_OSP_GBL_GBL_FRM_102', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(39, 'main_link_type', 39, 'Main Link Type', true, 1, '2022-02-03 18:40:23.613', NULL, NULL, 'SI_OSP_GBL_GBL_FRM_206', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(40, 'main_link_id', 40, 'Main Link Id', true, 1, '2022-02-03 18:40:23.667', NULL, NULL, 'SI_OSP_GBL_NET_GBL_190', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(41, 'redundant_link_type', 41, 'Redundant Link Type', true, 1, '2022-02-03 18:40:23.699', NULL, NULL, 'SI_OSP_GBL_NET_GBL_191', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(42, 'redundant_link_id', 42, 'Redundant Link Id', true, 1, '2022-02-03 18:40:23.730', NULL, NULL, 'SI_OSP_GBL_NET_GBL_192', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(11, 'each_lmc_length', 11, 'Each LMC Length(Meter)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_135', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(12, 'total_route_length', 12, 'Total Route Length(Meter)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_136', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(13, 'gis_length', 13, 'GIS Length(Meter)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_137', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(14, 'otdr_distance', 14, 'OTDR Distance(Meter)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_138', true);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(18, 'new_building_route_length', 18, 'New Build Route Length(Meter)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_144', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(19, 'otm_length', 19, 'OTM Length(Meter)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_145', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(20, 'otl_length', 20, 'OTL Length(Meter)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_146', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(24, 'total_row_length', 24, 'Total ROW Length(Meter)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_150', false);
INSERT INTO public.fiber_link_columns_settings (id, column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column) VALUES(25, 'total_row_reccuring_charges', 25, 'Total Row Recurring Charges/Annum(Rs.)', true, 1, '2020-12-04 17:58:54.061', NULL, NULL, 'SI_GBL_GBL_NET_FRM_151', false);


UPDATE public.layer_action_mapping SET action_name='SECONDARYADDSPLITTER' WHERE layer_id=(select layer_id from layer_details where layer_name='CDB') and lower(action_name)='addsplitter';

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='Pole') and associate_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure');

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='Pole') and associate_layer_id=(select layer_id from layer_details where layer_name='BDB');

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='Pole') and associate_layer_id=(select layer_id from layer_details where layer_name='FDB');

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='SpliceClosure') and associate_layer_id=(select layer_id from layer_details where layer_name='Pole');

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='Tree') and associate_layer_id=(select layer_id from layer_details where layer_name='BDB');

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='Tree') and associate_layer_id=(select layer_id from layer_details where layer_name='FDB');

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='Tree') and associate_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure');

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='Tree') and associate_layer_id=(select layer_id from layer_details where layer_name='CDB');

update associate_entity_master set is_shifting_allowed=false where layer_id=(select layer_id from layer_details where layer_name='Tree') and associate_layer_id=(select layer_id from layer_details where layer_name='ADB');

INSERT INTO public.associate_entity_master (layer_id, associate_layer_id, layer_subtype, is_enabled, created_on, created_by, modified_on, modified_by, is_snapping_enabled, is_shifting_allowed) VALUES((select layer_id from layer_details where layer_name='BDB'), (select layer_id from layer_details where layer_name='Pole'), NULL, true, '2023-11-22 16:27:26.903', 0, '2023-11-22 16:27:26.903', 0, false, false);

INSERT INTO public.associate_entity_master (layer_id, associate_layer_id, layer_subtype, is_enabled, created_on, created_by, modified_on, modified_by, is_snapping_enabled, is_shifting_allowed) VALUES((select layer_id from layer_details where layer_name='Tree'), (select layer_id from layer_details where layer_name='FDB'), NULL, true, '2023-11-22 16:27:26.903', 0, '2023-11-22 16:27:26.903', 0, false, false);

INSERT INTO public.associate_entity_master (layer_id, associate_layer_id, layer_subtype, is_enabled, created_on, created_by, modified_on, modified_by, is_snapping_enabled, is_shifting_allowed) VALUES((select layer_id from layer_details where layer_name='BDB'), (select layer_id from layer_details where layer_name='Tree'), NULL, true, '2023-11-22 16:27:26.903', 0, '2023-11-22 16:27:26.903', 0, false, false);


CREATE OR REPLACE FUNCTION public.fn_get_bom_boq_report_by_boundary(p_bom_boq_id integer, p_boundary_system_id integer, p_boundary_name character varying, p_user_id integer, p_user_name character varying, p_action character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

--select * from fn_get_bom_boq_report_by_boundary(0,43,'SubArea',5,'admin','DESIGNBOMBOQ')
--select * from fn_get_bom_boq_report_by_boundary(0,2,'SubArea',5,'admin','ConstructionBOMBOQ')
DECLARE
l_column_list record;
l_fsa_id character varying;
l_fsa_name character varying;
l_bom_boq_id int;
l_revision int;
l_geom_text text;
l_current_status character varying;
l_role_id int;
l_user_ids character varying;
l_gis_length_mtr numeric(18,10);
l_feeder_gis_length_mtr numeric(18,10);
l_distribution_gis_length_mtr numeric(18,10);
l_gis_fat_count numeric(18,10);
l_gis_fdc_count numeric(18,10);
l_gis_joint_closure_count numeric(18,10);
l_temp_table_column_list text;
l_create_table_string text;
l_dynamic_query text;
l_formula_text text;
--select * from fn_get_bom_boq_report_by_boundary(0,314,'SubArea',5,'admin','DESIGNBOMBOQ')
BEGIN
IF upper(p_action) = 'DESIGNBOMBOQ' Then
BEGIN
l_create_table_string = '';
l_formula_text = '';
----------------------------------------------------------------
--Fetch the List of Users
---------------------------------------------------------------
select u.role_id, string_agg(t.user_id::character varying,',') as user_ids
into l_role_id, l_user_ids
from user_master u
left join lateral
(
select user_id from user_master where manager_id = u.user_id
--select user_id from user_manager_mapping where manager_id = u.user_id
union
select child_user_id from user_report_mapping where parent_user_id = u.user_id
)t on true
where u.user_id = p_user_id
Group by u.user_id;
Raise info 'l_role_id - %, user_ids - %', l_role_id, l_user_ids;
--------------------------
--Process to generate the new BOM
--------------------------
---Fetch Geom and attribute data from attribute table and polygon master
--------------------------
select coalesce(sa.gis_design_id,sa.network_id), sa.subarea_name, replace(replace(st_astext(sp_geometry),'POLYGON((',''),'))','')
from att_details_subarea sa
into l_fsa_id, l_fsa_name, l_geom_text
inner join polygon_master p on sa.system_id = p.system_id and entity_type ='SubArea'
where sa.system_id = p_boundary_system_id;
raise info 'FSA Geometry -%,l_role_id-%,p_user_id-%,l_user_ids-%',l_geom_text,l_role_id,p_user_id,l_user_ids;
--------------------------
---Create a copy of bom_boq_info table as temp_bom_boq_info
--------------------------
DROP TABLE IF EXISTS temp_bom_boq_info;
CREATE TEMP TABLE temp_bom_boq_info AS
SELECT * from bom_boq_info_draft where 1 = 2;
--------------------------
---Call internal BOM function and put json data in "temp_bom_boq_json" table
--------------------------
Raise info 'Start Executing the BOM_BOQ Function- %', clock_timestamp();
DROP TABLE IF EXISTS temp_bom_boq_json;
CREATE TEMP TABLE temp_bom_boq_json AS
SELECT fn_get_bom_boq_report_new_2 as json_data from public.fn_get_bom_boq_report_new_2(''::character varying,''::character varying, ''::character varying, ''::character varying,
''::character varying, ''::character varying, ''::character varying, ''::character varying, ''::character varying, null::character varying,
null::character varying, l_geom_text::character varying,
p_user_id, l_role_id,''::character varying, ''::character varying, 'Polygon'::character varying, 0::double precision, p_user_id::character varying,
'', p_boundary_system_id::integer, 'SubArea'::character varying);

---------------------------------------
----Prepare the Data for Hierarchy Level 1 to Show L1 Items in Grid
---------------------------------------
INSERT INTO temp_bom_boq_info(fsa_id, fsa_name, entity_class, gis_uom, uom_sap,
Item_code, is_master, item_master_code, hierarchy_level,
created_by_user_id, created_by_user_name, created_on, modified_by_user_id, modified_user_name, modified_on
,original_design_qty, design_qty, additional_qty, additional_non_design_material_qty,cost_per_unit, gis_qty, is_non_design_item, display_sequence, is_design_qty_editable
)
SELECT Distinct l_fsa_id, l_fsa_name, L.ne_class_name,
Case when J.geom_type = 'Line' then 'KM' when J.geom_type = 'Point' then 'EA' end,
Case when J.geom_type = 'Line' then 'KM' when J.geom_type = 'Point' then 'EA' end,
L.ne_class_name, true, L.ne_class_name, 1,
p_user_id, p_user_name, now(),p_user_id, p_user_name, now(),
0,0,0,0,0,0, false, 1, true
FROM temp_bom_boq_json, json_to_record(temp_bom_boq_json.json_data::json)
AS j
(id int, entity_type character varying,layer_name character varying, item_code character varying, geom_type character varying,
entity_sub_type character varying, cost_per_unit numeric, service_cost_per_unit numeric,
specification character varying, name character varying, total_cost numeric,total_count numeric, calculated_length numeric, gis_length numeric, is_header boolean, networkstatus character varying
)
Inner Join item_template_master I ON I.code = J.item_code
INNER JOIN layer_details L ON I.layer_id = L.layer_id and L.layer_name = j.layer_name
where COALESCE(L.ne_class_name,'') <> '' and j.is_header = false and networkstatus = 'P' and j.entity_type <> 'Central_Office';

---------------------------------------
----Prepare the Data for Hierarchy Level 2 to Show L2 Items in Grid
---------------------------------------
INSERT INTO temp_bom_boq_info(fsa_id, fsa_name, entity_class, entity_sub_class, gis_uom, uom_sap,
Item_code, is_master, item_master_code, hierarchy_level,
created_by_user_id, created_by_user_name, created_on, modified_by_user_id, modified_user_name, modified_on,
original_design_qty, design_qty, additional_qty, additional_non_design_material_qty,cost_per_unit, gis_qty, is_non_design_item, is_design_qty_editable
)
SELECT Distinct l_fsa_id, l_fsa_name, L.ne_class_name,
Case When j.layer_name in('Pole','Manhole') then j.layer_name else I.specify_type End,
Case when J.geom_type = 'Line' then 'KM' when J.geom_type = 'Point' then 'EA' end,
Case when J.geom_type = 'Line' then 'KM' when J.geom_type = 'Point' then 'EA' end,
L.ne_class_name, true, L.ne_class_name, 2,
p_user_id, p_user_name, now(),p_user_id, p_user_name, now(),
0,0,0,0,0,0, false, true
FROM temp_bom_boq_json, json_to_record(temp_bom_boq_json.json_data::json)
AS j
(id int, entity_type character varying,layer_name character varying, item_code character varying, geom_type character varying,
entity_sub_type character varying, cost_per_unit numeric, service_cost_per_unit numeric,
specification character varying, name character varying, total_cost numeric,total_count numeric, calculated_length numeric, gis_length numeric, is_header boolean, networkstatus character varying
)
Inner Join item_template_master I ON I.code = J.item_code
INNER JOIN layer_details L ON I.layer_id = L.layer_id and L.layer_name = j.layer_name
where COALESCE(L.ne_class_name,'') <> '' AND COALESCE(I.specify_type,'') <> '' AND j.is_header = false and networkstatus = 'P' and j.entity_type <> 'Central_Office';

---------------------------------------
----Call the Design BOM Function to get the Design Qty
---------------------------------------

INSERT INTO temp_bom_boq_info(fsa_id, fsa_name, entity_type, entity_class, entity_sub_class, layer_name, specification, original_design_qty, overhead_percentage, design_qty, additional_qty, additional_non_design_material_qty, item_code, pts_code,
cost_per_unit, gis_qty, gis_uom, is_master, item_master_code, created_by_user_id, created_by_user_name, created_on, modified_by_user_id, modified_user_name, modified_on, uom_sap, hierarchy_level, is_design_qty_editable
)
SELECT l_fsa_id, l_fsa_name, j.entity_type, L.ne_class_name,
Case When j.layer_name in('Pole','Manhole') then j.layer_name else I.specify_type End,
j.layer_name,
Case When j.layer_name in('Pole','Manhole') then j.layer_name else I.specify_type End,
Case when j.geom_type = 'Line' then calculated_length when j.geom_type = 'Point' then total_count end,
COALESCE(COALESCE(fsa.default_overhead, f.default_overhead),'0')::numeric AS default_overhead,
Case
WHEN j.geom_type = 'Line' then calculated_length + (calculated_length * (COALESCE(COALESCE(fsa.default_overhead, f.default_overhead),'0')::numeric)/100)
WHEN j.geom_type = 'Point' then total_count
END,
0, 0, j.item_code, LPAD(j.item_code,18,'0') as pts_code,
j.cost_per_unit, Case when j.geom_type = 'Line' then gis_length when j.geom_type = 'Point' then total_count end,
Case when j.geom_type = 'Line' then 'KM' when j.geom_type = 'Point' then 'EA' end, true, j.item_code as item_master_code,
p_user_id, p_user_name, now(),p_user_id, p_user_name, now(),
Case when j.geom_type = 'Line' then 'KM' when j.geom_type = 'Point' then 'EA' end, 3, true
FROM temp_bom_boq_json,
json_to_record(temp_bom_boq_json.json_data::json)
AS j
(id int, entity_type character varying,layer_name character varying, item_code character varying, geom_type character varying,
entity_sub_type character varying, cost_per_unit numeric, service_cost_per_unit numeric,
specification character varying, name character varying, total_cost numeric,total_count numeric, calculated_length numeric, gis_length numeric, is_header boolean, networkstatus character varying
)
Inner Join item_template_master I ON COALESCE(I.code,'') = J.item_code
INNER JOIN layer_details L ON I.layer_id = L.layer_id and l.layer_name = j.layer_name
LEFT JOIN construction_oh_logic_master f on LOWER(f.category) = LOWER(L.ne_class_name) AND LOWER(f.entity_sub_class) = LOWER(I.specify_type) AND f.is_default = true and f.subarea_system_id = 0
LEFT JOIN construction_oh_logic_master fSA on LOWER(L.ne_class_name) = LOWER(fSA.category) and LOWER(fSA.entity_sub_class) = LOWER(I.specify_type) AND fSA.is_default = false and fSA.subarea_system_id = p_boundary_system_id
WHERE COALESCE(L.ne_class_name,'') <> '' AND COALESCE(I.specify_type,'') <> '' and j.is_header = false and networkstatus = 'P' and j.entity_type <> 'Central_Office';

---------------------------------
-- Add alternate items for every is_master item for Design Items
---------------------------------

INSERT INTO temp_bom_boq_info(fsa_id, fsa_name, entity_type,layer_name, entity_class, specification,original_design_qty, design_qty, additional_qty, additional_non_design_material_qty, item_code, pts_code,
cost_per_unit, gis_qty, gis_uom, is_master, item_master_code, created_by_user_id, created_by_user_name, created_on, modified_by_user_id, modified_user_name, modified_on, uom_sap, entity_sub_class, hierarchy_level, is_design_qty_editable
)
select t.fsa_id, t.fsa_name, t.entity_type,t.layer_name, t.entity_class, s.specification,0, 0, 0, 0, s.code, LPAD(s.code,18,'0'),
s.cost_per_unit, 0, t.gis_uom, false, s.item_master_code,
p_user_id, p_user_name, clock_timestamp(),p_user_id, p_user_name, clock_timestamp(),
t.uom_sap, t.entity_sub_class, 3, t.is_design_qty_editable
from temp_bom_boq_info t
inner join public.item_template_master s
on t.item_code = s.item_master_code and s.is_master = false
where s.is_active = true;
---------------------------------
-- Update Class Type from the layer_details
---------------------------------
/*update temp_bom_boq_info
set entity_class = ne_class_name
from layer_details
where upper(temp_bom_boq_info.layer_name) =upper(layer_details.layer_name);*/

---------------------------------
-- Update attributes from specification from item_template_master table
---------------------------------
update temp_bom_boq_info
set sub_category_1 = subcategory_1,
sub_category_2 = subcategory_2,
overhead_percentage = extra_percentage
--,gis_uom = unit_measurement
from item_template_master
where item_code = code;

---------------------------------
-- Add Non Design items from the table non_design_item_template_master(Hierarchy Level already)
---------------------------------

INSERT INTO temp_bom_boq_info(fsa_id, fsa_name, entity_type, entity_class, specification,original_design_qty, design_qty, additional_qty, additional_non_design_material_qty, item_code, pts_code,
cost_per_unit, gis_qty, gis_uom, is_master, item_master_code, created_by_user_id, created_by_user_name, created_on, modified_by_user_id, modified_user_name, modified_on, uom_sap,overhead_percentage,is_non_design_item,sub_category_1,sub_category_2,
entity_sub_class, hierarchy_level, is_dependent_calculation, display_sequence, is_design_qty_editable
)
select l_fsa_id, l_fsa_name,
category_reference as entity_type, gis_class as entity_class, s.specification,0, 0, 0, 0, s.code, LPAD(s.code,18,'0'),
s.cost_per_unit, 0, unit_measurement as gis_uom, is_master, s.item_master_code,
p_user_id, p_user_name, clock_timestamp(),p_user_id, p_user_name, clock_timestamp(),
unit_measurement as uom_sap, s.extra_percentage, true as is_non_design_item, subcategory_1,subcategory_2,
entity_sub_class, hierarchy_level, coalesce(is_dependent_calculation, false) as is_dependent_calculation,
coalesce(display_sequence, 0), s.is_design_qty_editable
from non_design_item_template_master s
where s.is_active = true;
--------------------------
-----Preapare a Temp Table to Evalute the Formulas from the construction_oh_logic_master
-------------------------
--select * from construction_oh_logic_master
select string_agg(column_name, ' NUMERIC(18,2),' ORDER by execution_sequence) || ' NUMERIC(18,2) ' into l_temp_table_column_list
from construction_oh_logic_master
where is_default = true;

DROP TABLE IF EXISTS tbl_bom_quatity_calculation;
l_create_table_string = 'CREATE LOCAL TEMP TABLE tbl_bom_quatity_calculation('||lower(l_temp_table_column_list)||');';
EXECUTE (l_create_table_string);
raise info 'l_create_table_string - %', l_create_table_string;

--l_feeder_gis_length_mtr numeric(18,10);
--l_distribution_gis_length_mtr numeric(18,10);
-------------------------------------------
----Calculate the GIS Length of Cables, FAT Count, FDC Count Joint Closure count and update in temp table
------------------------------------------
select COALESCE(SUM(COALESCE(original_design_qty,0)),0) *1000 --COALESCE(SUM(COALESCE(design_qty,0)),0) *1000
INTO l_feeder_gis_length_mtr
FROM temp_bom_boq_info WHERE UPPER(entity_class) = 'TRANSMEDIA' AND UPPER(entity_sub_class) = 'FEEDER' AND hierarchy_level = 3;
RAISE INFO 'l_feeder_gis_length_mtr - %' ,l_feeder_gis_length_mtr;

select COALESCE(SUM(COALESCE(original_design_qty,0)),0) *1000 --COALESCE(SUM(COALESCE(design_qty,0)),0) *1000
INTO l_distribution_gis_length_mtr
FROM temp_bom_boq_info WHERE UPPER(entity_class) = 'TRANSMEDIA' AND UPPER(entity_sub_class) = 'DISTRIBUTION' AND hierarchy_level = 3;
RAISE INFO 'l_distribution_gis_length_mtr - %' ,l_distribution_gis_length_mtr;

select COALESCE(SUM(COALESCE(original_design_qty,0)),0) *1000 --COALESCE(SUM(COALESCE(design_qty,0)),0) *1000
INTO l_gis_length_mtr
FROM temp_bom_boq_info WHERE UPPER(entity_class) = 'TRANSMEDIA' AND hierarchy_level = 3;
RAISE INFO 'l_gis_length_mtr - %' ,l_gis_length_mtr;

SELECT COALESCE(SUM(COALESCE(design_qty,0)),0) INTO l_gis_fat_count
FROM temp_bom_boq_info WHERE UPPER(entity_sub_class) = 'FAT' and hierarchy_level = 3;

SELECT COALESCE(SUM(COALESCE(design_qty,0)),0) INTO l_gis_fdc_count
FROM temp_bom_boq_info WHERE UPPER(entity_sub_class) = 'FDC' and hierarchy_level = 3;

SELECT COALESCE(SUM(COALESCE(design_qty,0)),0) INTO l_gis_joint_closure_count
FROM temp_bom_boq_info WHERE UPPER(entity_sub_class) = 'Joint Closure' and hierarchy_level = 3;
-------------------------------------------
----Insert a Single Rows with columns calculated from design Values
------------------------------------------
INSERT INTO tbl_bom_quatity_calculation(feeder_gis_length, distribution_gis_length, gis_length, splice_closure_count, fdc_count, fat_count)
VALUES(l_feeder_gis_length_mtr,l_distribution_gis_length_mtr, l_gis_length_mtr, l_gis_joint_closure_count, l_gis_fdc_count, l_gis_fat_count);
-------------------------------------------
----Loop over every columns exits in tbl_bom_quatity_calculation table
------------------------------------------
FOR l_column_list IN
SELECT lower(Col.column_name) As column_name, COALESCE(fSA.oh_logic, f.oh_logic) AS oh_logic
FROM information_schema.columns Col
INNER JOIN construction_oh_logic_master f on lower(f.column_name) = lower(Col.Column_name)
LEFT JOIN construction_oh_logic_master fSA on lower(fSA.column_name) = lower(Col.Column_name) AND fSA.is_default = false and fSA.subarea_system_id = p_boundary_system_id AND fSA.is_active = true AND fsa.oh_logic <> ''
WHERE table_name = 'tbl_bom_quatity_calculation'
AND f.execution_sequence > 3 AND f.is_default = true AND f.is_active = true AND f.oh_logic <> ''
ORDER BY Col.ordinal_position
LOOP
-------------------------------------------
----Create and exceute a dynamic query to evalute the formula configured in table construction_oh_logic_master
------------------------------------------
RAISE INFO 'oh_logic - %', l_column_list.oh_logic;
--l_formula_text = l_column_list.oh_logic;
l_formula_text = replace(l_column_list.oh_logic,'/','*1.00/');
--l_dynamic_query = 'Update tbl_bom_quatity_calculation set ' || l_column_list.column_name || ' = ceil(COALESCE(' || l_column_list.oh_logic ||',0))';
l_dynamic_query = 'Update tbl_bom_quatity_calculation set ' || l_column_list.column_name || ' = ceil(COALESCE(' || l_formula_text ||',0))';
RAISE INFO 'l_dynamic_query - %', l_dynamic_query;
EXECUTE (l_dynamic_query);
-------------------------------------------
----Create and exceute a dynamic query to update evaluated values in temp_bom_boq_info table
------------------------------------------
l_dynamic_query = 'UPDATE temp_bom_boq_info bom SET design_qty = COALESCE(t.'|| l_column_list.column_name ||' , 0)
FROM construction_oh_logic_master temp, tbl_bom_quatity_calculation t
where bom.entity_class = temp.category AND (COALESCE(bom.entity_sub_class,'''') = '''' OR bom.entity_sub_class = temp.entity_sub_class)
and temp.column_name = '''|| l_column_list.column_name ||'''and bom.hierarchy_level = temp.hierarchy_level ' ;
RAISE INFO 'l_dynamic_query - %', l_dynamic_query;
EXECUTE (l_dynamic_query);
END LOOP;
--------------------------------------
----Copy The design Qty of L2 Items to L3 Items If L2 has only one L3 Item
--------------------------------------
Update temp_bom_boq_info bom1
SET design_qty = bom2.design_qty
FROM temp_bom_boq_info bom2,
(
select entity_class, entity_sub_class, 2 as hierarchy_level
from temp_bom_boq_info
where hierarchy_level = 3 and is_non_design_item = true
Group By entity_class, entity_sub_class
Having count(1) = 1
) AS Temp
Where bom2.entity_class = Temp.entity_class AND bom2.entity_sub_class = Temp.entity_sub_class
AND bom2.hierarchy_level = Temp.hierarchy_level
AND bom2.entity_class = bom1.entity_class AND bom2.entity_sub_class = bom1.entity_sub_class
AND bom1.hierarchy_level = 3;

---------------------------------
-- Update the temp table that will be calculated
---------------------------------
update temp_bom_boq_info
set final_qty = (design_qty + additional_qty + additional_non_design_material_qty),
jpf_total_capex = ((design_qty + additional_qty + additional_non_design_material_qty) * cost_per_unit);
---------------------------------
-- Update the Summary over entity_class
---------------------------------
update temp_bom_boq_info b
set design_qty=temp.design_qty,
additional_qty=temp.additional_qty,
additional_non_design_material_qty = temp.additional_non_design_material_qty,
final_qty = temp.final_qty,
jpf_total_capex = temp.jpf_total_capex
FROM(
SELECT entity_class, SUM(design_qty) AS design_qty, SUM(additional_qty) AS additional_qty, SUM(additional_non_design_material_qty) AS additional_non_design_material_qty, SUM(final_qty) AS final_qty, SUM(jpf_total_capex) AS jpf_total_capex
FROM temp_bom_boq_info
WHERE hierarchy_level = 3
GROUP BY entity_class
) temp
WHERE temp.entity_class = b.entity_class AND b.hierarchy_level = 1 and b.is_non_design_item = false;

---------------------------------
-- Update the Summary over entity_sub_class
---------------------------------
update temp_bom_boq_info b
set design_qty=temp.design_qty,
additional_qty=temp.additional_qty,
additional_non_design_material_qty = temp.additional_non_design_material_qty,
final_qty = temp.final_qty,
jpf_total_capex = temp.jpf_total_capex
FROM(
SELECT entity_class, entity_sub_class, SUM(design_qty) AS design_qty, SUM(additional_qty) AS additional_qty, SUM(additional_non_design_material_qty) AS additional_non_design_material_qty, SUM(final_qty) AS final_qty, SUM(jpf_total_capex) AS jpf_total_capex
FROM temp_bom_boq_info
WHERE hierarchy_level = 3
GROUP BY entity_class, entity_sub_class
) temp
WHERE temp.entity_class = b.entity_class AND temp.entity_sub_class = b.entity_sub_class AND b.hierarchy_level = 2 and b.is_non_design_item = false;

--------------------------
---Delete the Existing Temp BOM for the selected boundary
--------------------------
DELETE FROM public.bom_boq_info_draft WHERE bom_boq_id in (
SELECT bom_boq_revision_info.bom_boq_id FROM bom_boq_revision_info
WHERE status = 'TEMP' AND fsa_system_id = p_boundary_system_id
);

DELETE FROM bom_boq_revision_info
WHERE status = 'TEMP' AND fsa_system_id = p_boundary_system_id;
--------------------------
---Fetch The current revision number of BOM for the Selected Boundary
--------------------------
SELECT (coalesce(Max(revision),0) + 1) into l_revision
from bom_boq_revision_info
where STATUS = 'SUBMITTED' and fsa_system_id = p_boundary_system_id;
--TEMP/SAVED/SUBMITTED
Raise info 'l_revision - %',l_revision;
--------------------------
---Insert Master Entry in bom_boq_revision_info table
--------------------------
INSERT INTO bom_boq_revision_info(fsa_system_id, fsa_id, fsa_name, revision, status, remarks, created_by_user_id, created_by_user_name, created_on,
modified_by_user_id, modified_user_name, modified_on)
Values(p_boundary_system_id, l_fsa_id, l_fsa_name, l_revision,'TEMP','', p_user_id, p_user_name, clock_timestamp(),p_user_id, p_user_name, clock_timestamp())
RETURNING bom_boq_id into l_bom_boq_id;
Raise info 'New BOM id generated - %', l_bom_boq_id;

---------------------------------
-- INSERT INTO Details table "bom_boq_info"
---------------------------------
INSERT INTO public.bom_boq_info_draft(
bom_boq_id, fsa_system_id, fsa_id, fsa_name, entity_type, entity_class, specification, original_design_qty, design_qty, additional_qty, additional_non_design_material_qty, item_code, pts_code, cost_per_unit, sub_category_1, sub_category_2, uom_sap, revision, final_qty, jpf_total_capex, jc_sap_id, jc_name, gis_qty, gis_uom, overhead_percentage,
is_master,item_master_code, created_by_user_id, created_by_user_name, created_on, modified_by_user_id, modified_user_name, modified_on, is_non_design_item, entity_sub_class, hierarchy_level, is_dependent_calculation, display_sequence, is_design_qty_editable)
select l_bom_boq_id, p_boundary_system_id,
fsa_id, fsa_name, entity_type, entity_class, specification, original_design_qty, design_qty, additional_qty, additional_non_design_material_qty, item_code, pts_code, cost_per_unit, sub_category_1, sub_category_2, uom_sap, coalesce(l_revision,0), final_qty, jpf_total_capex, jc_sap_id, jc_name, gis_qty, gis_uom, coalesce(overhead_percentage,0),
is_master,item_master_code, created_by_user_id, created_by_user_name, created_on, modified_by_user_id, modified_user_name, modified_on, coalesce(is_non_design_item, false), entity_sub_class, hierarchy_level, coalesce(is_dependent_calculation, false) as is_dependent_calculation, Coalesce(display_sequence, 0), is_design_qty_editable
from temp_bom_boq_info;
--------------------------
---Return The Design Bom Data(Disign BOM will not have NON Design Item)
--------------------------
Return query
select row_to_json(bom_boq_info_draft) from public.bom_boq_info_draft
where bom_boq_id = l_bom_boq_id and is_non_design_item = false and is_master = true -- Hide the non-design and alternate items from design BOM/BOQ
order by CASE WHEN is_non_design_item = true THEN 1 ELSE 0 End ASC,
entity_class ASC, COALESCE(entity_sub_class,'000000') ASC, hierarchy_level ASC, entity_type, item_code, id;
END;
Else if lower(p_action) = 'export' Then
--------------------------
---This block is used for Export the Items in Excel
--------------------------
SELECT status INTO l_current_status
FROM public.bom_boq_revision_info where bom_boq_id = p_bom_boq_id;
Return query
select row_to_json(bom_boq_info_draft) from public.bom_boq_info_draft
where bom_boq_id = p_bom_boq_id and (l_current_status <> 'TEMP' OR (is_non_design_item = false and is_master = true))-- Hide the non-design and alternate items from design BOM/BOQ Export
AND hierarchy_level = 3
order by CASE WHEN is_non_design_item = true THEN 1 ELSE 0 End ASC, entity_class ASC, COALESCE(entity_sub_class,'000000') ASC, hierarchy_level ASC, entity_type, item_code, id;
Else
--------------------------
--Return the existing BOM Data( Coustruction BOMBOQ/DATA)
--------------------------
SELECT bom_boq_id into p_bom_boq_id from bom_boq_revision_info
where fsa_system_id = p_boundary_system_id and (status = 'SAVED' OR status = 'SUBMITTED')
ORDER BY
CASE WHEN status = 'SAVED' THEN 0 ELSE 1 End ASC,
revision desc
limit 1;
Raise info 'p_bom_boq_id - %', p_bom_boq_id;
IF p_bom_boq_id is null Then
p_bom_boq_id = 0;
End if;
Return query
select row_to_json(bom_boq_info_draft) from public.bom_boq_info_draft
where bom_boq_id = p_bom_boq_id
order by CASE WHEN is_non_design_item = true THEN 1 ELSE 0 End ASC, entity_class ASC, COALESCE(entity_sub_class,'000000') ASC, hierarchy_level ASC, entity_type, item_code, id;
End If;
End If;
END;
$function$
;