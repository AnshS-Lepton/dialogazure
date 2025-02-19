INSERT INTO public.layer_action_mapping
(layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id)
VALUES((select layer_id from layer_details where layer_name = 'SpliceClosure'), 'butterfly', true, 26, 'butterfly', true, false, true, '', 0, 0, false, true, 0, '', false, 0);


CREATE OR REPLACE FUNCTION public.fn_get_splicing_network_diagram(p_system_id integer, p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE nodes text;
 edges text; 
 legends text;
 checkbox text;
 v_layer_title character varying;
 v_layer_table character varying;
 v_entity_display_text character varying;
 arow record;
BEGIN

create temp table temp_vis_result
(
   row_id serial,
   system_id integer,
   entity_type character varying,
   entity_title character varying,  
   associated_system_id integer,
   associated_entity_type character varying,
   associated_entity_title character varying,
   associated_network_id character varying,
   cable_system_id integer,
   cable_network_id character varying,
   cable_label_text character varying,
   checkbox_entity_type character varying,
   other_end_system_id integer,  
   other_end_entity_type character varying,
   other_end_entity_title character varying,
   other_end_network_id character varying
   
) on commit drop;

-- GET ENTITY DISPLAY TEXT--
select layer_title,layer_table into v_layer_title,v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
execute 'select concat(network_id,''-('',network_status,'')'') from '||v_layer_table||' where system_id='||p_system_id||' limit 1 ' into  v_entity_display_text;


-- GET ALL ASSOCIATED ELEMENTS FOR ENTITY TYPE..
FOR arow IN  
select a.*,ld.layer_title as associated_entity_title  from (         
    select destination_system_id as associated_system_id, source_network_id as network_id, 
    destination_network_id as associated_network_id,destination_entity_type as associated_entity_type from connection_info
    where upper(source_entity_type)=upper(p_entity_type) and  source_system_id=p_system_id  
    UNION

     select source_system_id , destination_network_id as network_id,
    source_network_id ,source_entity_type  from connection_info
    where upper(destination_entity_type)=upper(p_entity_type) and  destination_system_id=p_system_id  
    
) a inner join layer_details ld on upper(ld.layer_name)=upper(a.associated_entity_type)

LOOP
	-- GET ASSOCIATED CABLES FOR ENTITY ASSOCIATED WITH MANHOLE--
	if( p_entity_type = 'SpliceClosure')
	then
	insert into temp_vis_result(system_id,entity_type,entity_title,associated_system_id,associated_entity_type,associated_entity_title,associated_network_id,
		cable_system_id,cable_network_id,cable_label_text,other_end_system_id,other_end_entity_type,other_end_network_id,other_end_entity_title,checkbox_entity_type)
		select p_system_id as system_id,p_entity_type as entity_type,v_layer_title as entity_title,arow.associated_system_id,
		arow.associated_entity_title,arow.associated_entity_type,arow.associated_network_id, b.cable_system_id,b.cable_network_id,b.cable_label_text,
		b.other_end_system_id,b.other_end_entity_type,b.other_end_network_id,ldnew.layer_title as other_end_entity_title,'Cable' as checkbox_entity_type
		 from (
			select system_id as cable_system_id,network_id as cable_network_id,
			CONCAT(network_id,'-(',total_core,'F)-(',network_status,')') as cable_label_text ,b_system_id as other_end_system_id,
			b_entity_type as other_end_entity_type,
			b_location as other_end_network_id from att_details_cable where system_id=arow.associated_system_id 	
		 ) b left join layer_details ldnew on upper(ldnew.layer_name)=upper(b.other_end_entity_type);

end if;

	
END LOOP;

-- NODES JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
	select p_system_id as id, v_layer_title as label,p_entity_type as group, 8 as value,'' as checkbox_entity_type
	union
	select  row_id as id, CONCAT(other_end_entity_title,'(',other_end_network_id,')')  as label, other_end_entity_type as group, 1 as value,checkbox_entity_type from temp_vis_result	
) x) into nodes;

-- EDGES JSON--
 Select (select array_to_json(array_agg(row_to_json(x)))from (
	select a.*,250 as length,
	'gray' as color,'gray' as fontcolor,'{"align": "top"}'::json as font,5 as width 
	from (
		select 0 as from,p_system_id as to,'' as label,'' as associated_entity_type,'' as checkbox_entity_type
		union
		select p_system_id as from,row_id as to,cable_label_text as label,associated_entity_type,checkbox_entity_type from temp_vis_result
	) a
) x) into edges;

--LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (

	select distinct * from (
		select p_entity_type as entity_type, v_layer_title as entity_title 
		union
		select other_end_entity_type,other_end_entity_title from temp_vis_result 	
	 ) a
) x) into legends;


--CHECKBOX IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
	

	select checkbox_entity_type, count(checkbox_entity_type),'gray'  as color
from temp_vis_result t
group by checkbox_entity_type
) x) into checkbox;

return query select row_to_json(result) from (
select p_entity_type as entityType, v_layer_title as entityTitle,v_entity_display_text as entityDisplayText, nodes,edges,legends,checkbox
) result;
END; 
$function$
;