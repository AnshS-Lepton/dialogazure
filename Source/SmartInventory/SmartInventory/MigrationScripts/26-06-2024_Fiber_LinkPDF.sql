CREATE OR REPLACE FUNCTION public.fn_get_export_cable_info_bylink_systemids(p_linksystemids character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$ 
BEGIN
 
return query

select row_to_json(row) from(
 
  
select distinct lnk.network_id as link_network_id,lnk.Link_id,cable.network_id as cable_network_id,cable.cable_name,cable.total_core,cable.no_of_tube,fn_get_display_name(cable.a_system_id, cable.a_entity_type) AS a_location,
fn_get_display_name(cable.b_system_id, cable.b_entity_type) AS b_location,cableinfo.fiber_number from att_details_cable_info as cableinfo 
inner join att_details_cable as cable on  cableinfo.cable_id= cable.system_id 
inner join att_details_fiber_link lnk on cableinfo.link_system_id= lnk.system_id where cableinfo.link_system_id in(select regexp_split_to_table(p_LinkSystemIds,',')::int as id)
)row;


END ;
$function$
;