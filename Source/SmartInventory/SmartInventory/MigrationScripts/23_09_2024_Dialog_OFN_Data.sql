INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, modified_by,  dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details ld where layer_name ='Cable'), 'Cable_Category', 'Jump_Cable', true, 1,  NULL,  'Jump_Cable', NULL, 'Cable_Category', true, true, 0);


------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_export_cable_info_bylinkid(p_linkid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$ 
BEGIN

return query
select row_to_json(row) from(
select distinct cable.network_id,cable.cable_name,cable.total_core,cable.no_of_tube,fn_get_display_name(cable.a_system_id, cable.a_entity_type) AS a_location,
fn_get_display_name(cable.b_system_id, cable.b_entity_type) AS b_location,cableinfo.fiber_number,cable.cable_measured_length,cable.cable_calculated_length from att_details_cable_info as cableinfo 
inner join att_details_cable as cable on  cableinfo.cable_id= cable.system_id  where cableinfo.link_system_id=p_LinkId
)row;


END ;
$function$
;
