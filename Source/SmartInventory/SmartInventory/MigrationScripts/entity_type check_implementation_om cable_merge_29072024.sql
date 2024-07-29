-- FUNCTION: public.fn_merge_cable(integer, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_merge_cable(integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_merge_cable(
	mastercableid integer,
	secondcableid integer,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

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
case when '''||MinDistanceRoute ||'''=''S1S2Distance'' then (select b_location from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''S1E2Distance'' then (select a_location from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''E1E2Distance'' then a_location
else a_location end,
case when '''||MinDistanceRoute ||'''=''E1E2Distance'' then (select a_location from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''S1S2Distance'' then b_location
when '''||MinDistanceRoute ||''' =''S1E2Distance'' then b_location
else (select b_location from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable'')) end,
total_core,no_of_tube,no_of_core_per_tube,
to_char(st_length(ST_transform(ST_GEOMFROMTEXT('''||MergeGeometry||''',4326), utmzone(ST_Centroid(ST_GEOMFROMTEXT('''||MergeGeometry||''',4326))))),
''00000000000.000'')::float ,(cable_calculated_length+(Select cable_calculated_length from att_details_cable where
system_id='||secondCableID||' and upper(category)=upper(''cable'')))::float, cable_type, coreaccess, wavelength, optical_output_power, frequency, attenuation_db,
resistance_ohm, construction, activation, accessibility, specification, category, subcategory1, subcategory2, subcategory3, item_code,
vendor_id, "type", brand, model, network_status, status, pin_code, province_id, region_id, totalattenuationloss, chromaticdb, chromaticdispersion,
totalchromaticloss, remarks, '||p_user_id||', now(), null, modified_on, 
(case when '''||MinDistanceRoute ||'''=''S1S2Distance'' then (select b_system_id from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''S1E2Distance'' then (select a_system_id from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''E1E2Distance'' then a_system_id
else a_system_id end), a_network_id,
(case when '''||MinDistanceRoute ||'''=''S1S2Distance'' then (select b_entity_type from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''S1E2Distance'' then (select a_entity_type from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''E1E2Distance'' then a_entity_type
else a_entity_type end), 
(case when '''||MinDistanceRoute ||'''=''E1E2Distance'' then (select a_system_id from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''S1S2Distance'' then b_system_id
when '''||MinDistanceRoute ||''' =''S1E2Distance'' then b_system_id
else (select b_system_id from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable'')) end), b_network_id,
(case when '''||MinDistanceRoute ||'''=''E1E2Distance'' then (select a_entity_type from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable''))
when '''||MinDistanceRoute ||'''=''S1S2Distance'' then b_entity_type
when '''||MinDistanceRoute ||''' =''S1E2Distance'' then b_entity_type
else (select b_entity_type from att_Details_cable where system_id ='||SecondCableID||' and upper(category)=upper(''cable'')) end), sequence_id, duct_id, trench_id, project_id, planning_id, purpose_id, workorder_id, structure_id, cable_category, loop_count, loop_length,
total_loop_length, total_loop_count, route_id, parent_system_id, parent_network_id, parent_entity_type, start_reading, end_reading, cable_sub_category,
execution_method, is_used, barcode, drum_no, utilization, notification_status_id, acquire_from, circuit_id, thirdparty_circuit_id, ownership_type, third_party_vendor_id,
source_ref_type, source_ref_id, source_ref_description, audit_item_master_id, status_remark, status_updated_by, status_updated_on, is_visible_on_map, primary_pod_system_id,
secondary_pod_system_id, is_new_entity, inner_dimension, outer_dimension
from att_details_cable where system_id ='||mastercableId ||' and upper(category)=upper(''cable'') RETURNING system_id;';
raise info 'sql %',sql;
execute sql into v_arrow_cable;

sql := 'INSERT INTO public.line_master
(system_id,entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by, approver_id, common_name, db_flag,
approval_date, modified_on, network_status, is_virtual, display_name, modified_by)
select '|| v_arrow_cable||', entity_type, approval_flag, ST_GeomFromText(St_astext('''||MergeGeometry ::character varying||'''),4326), creator_remark, approver_remark, '||p_user_id||', approver_id, common_name, db_flag,
now(), now(), network_status, is_virtual, display_name, null
from line_master where system_id ='||masterCableId||' and upper(entity_type)=upper(''cable'');';
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

if EXISTS(select 1 from att_details_loop where (cable_system_id =mastercableID and upper(associated_entity_type)=upper('cable')))
then
if exists (select 1 from att_details_loop where (associated_system_id =mastercableID and upper(associated_entity_type)=upper('cable'))) then
update att_details_loop set cable_system_id =v_arrow_cable,associated_system_id =v_arrow_cable
where cable_system_id =mastercableID and upper(associated_entity_type)=upper('cable');
else
update att_details_loop set cable_system_id =v_arrow_cable
where cable_system_id =mastercableID and upper(associated_entity_type)=upper('cable');
end if;
end if;

if EXISTS(select 1 from att_details_loop where (cable_system_id =SecondCableID and upper(associated_entity_type)=upper('cable')))
then
if exists (select 1 from att_details_loop where (associated_system_id =SecondCableID and upper(associated_entity_type)=upper('cable'))) then
update att_details_loop set cable_system_id =v_arrow_cable,associated_system_id =v_arrow_cable,
associated_network_id = (select network_id from att_details_cable where system_id=mastercableID and upper(category)=upper('cable'))
where cable_system_id =SecondCableID and upper(associated_entity_type)=upper('cable');
else
update att_details_loop set cable_system_id =v_arrow_cable,
associated_network_id = (select network_id from att_details_cable where system_id=mastercableID and upper(category)=upper('cable'))
where cable_system_id =SecondCableID and upper(associated_entity_type)=upper('cable');
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

END; 
$BODY$;

ALTER FUNCTION public.fn_merge_cable(integer, integer, integer)
    OWNER TO postgres;
