
CREATE OR REPLACE FUNCTION public.fn_get_core_planner_splicing(required_core integer, p_user_id integer, fiber_link_network_id character varying, source_network_id character varying, destination_network_id character varying, buffer integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE 
    rec RECORD;
    v_network_id character varying;
    v_system_id integer;
    v_cable_details_left record;
    v_cable_details_right record;
    v_cable_details record;
   p_link_system_id integer;
   start_point integer;
  v_status bool;
  v_message  character varying;

BEGIN
    v_network_id := '';
    v_system_id := 0;

    -- Create a temporary table for connection data
    CREATE TEMP TABLE temp_connection (
        source_system_id integer,
        source_network_id character varying(100),
        source_entity_type character varying(100),
        source_port_no integer,
        destination_system_id integer,
        destination_network_id character varying(100),
        destination_entity_type character varying(100),
        destination_port_no integer,
        is_source_cable_a_end boolean NOT NULL DEFAULT false,
        is_destination_cable_a_end boolean NOT NULL DEFAULT false,
        equipment_system_id integer,
        equipment_network_id character varying(100),
        equipment_entity_type character varying(100),
        created_by integer,
        splicing_source character varying(100)
    ) ON COMMIT DROP;
     
  SELECT 
        (result_json->>'status')::BOOLEAN, 
        result_json->>'message'
    INTO v_status, v_message
    FROM fn_get_core_planner_validation(source_network_id, destination_network_id, buffer, required_core, p_user_id) AS result_json;
   
   IF v_status = false  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT v_status AS status, v_message AS message) row;
    ELSE
   select system_id into p_link_system_id from att_details_fiber_link adfl where network_id = fiber_link_network_id;
        -- Loop through records to populate temp_connection for valid routes
        FOR rec IN select * from (      
         SELECT a_system_id,a_network_id,a_entity_type FROM core_planner_logs  where user_id = p_user_id and is_valid =TRUE
        union
        SELECT b_system_id,b_network_id,b_entity_type FROM core_planner_logs where user_id = p_user_id and is_valid =TRUE )a
        LOOP
    raise info 'rec 1 : %',rec;
		if(rec.a_entity_type='FMS')
		then

		select b.* into v_cable_details_left 
		from core_planner_logs a
		inner join att_details_cable b on a.cable_id=b.system_id where (b.a_system_id=rec.a_system_id or b.b_system_id=rec.a_system_id ) and (a.a_entity_type=rec.a_entity_type or b.b_entity_type = rec.a_entity_type);
   raise info 'v_cable_details_left 1 : %',v_cable_details_left;
  
		INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source
                )
		    SELECT 
                   rec.a_system_id, rec.a_network_id, rec.a_entity_type, 
                    a.port_number, 
                    v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    a.port_number,false, 
                    (case when v_cable_details_left.a_entity_type=rec.a_entity_type and v_cable_details_left.a_system_id=rec.a_system_id
                    then true else false end), 
                    p_user_id, 'CorePlanning'
		    FROM  isp_port_info a where a.parent_system_id=rec.a_system_id and a.parent_entity_type=rec.a_entity_type and a.input_output='O' and port_status_id=1 limit required_core;
       
		   

		else

		select b.* into v_cable_details_left 
		from core_planner_logs a
		inner join att_details_cable b on a.cable_id=b.system_id 
	where (a.a_system_id=rec.a_system_id or a.b_system_id =rec.a_system_id )  
and (a.a_entity_type=rec.a_entity_type or a.b_entity_type=rec.a_entity_type) 
and a.user_id = p_user_id and a.is_valid =true order by a.id asc limit 1;
   raise info 'v_cable_details_left 2 : %',v_cable_details_left;

		select b.* into v_cable_details_right 
		from core_planner_logs a
		inner join att_details_cable b on a.cable_id=b.system_id where (a.b_system_id=rec.a_system_id or a.a_system_id = rec.a_system_id) and (a.a_entity_type=rec.a_entity_type or a.b_entity_type = rec.a_entity_type) and a.user_id = 5 and a.is_valid =true order by a.id desc limit 1;
   raise info 'v_cable_details_right 3 : %',v_cable_details_left;

		INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source,equipment_system_id, equipment_network_id, equipment_entity_type 
                )
		    SELECT 
                    v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    a.fiber_number, 
                    v_cable_details_right.system_id, v_cable_details_right.network_id, 'Cable', 
                    a.fiber_number,
                   (case when v_cable_details_left.a_entity_type=rec.a_entity_type
                   and v_cable_details_left.a_system_id=rec.a_system_id
                   then true else false end),
                    (case when v_cable_details_right.a_entity_type=rec.a_entity_type  
                    and v_cable_details_right.a_system_id=rec.a_system_id
                    then true else false end), 
                    p_user_id, 'CorePlanning',rec.a_system_id,rec.a_network_id,rec.a_entity_type
		    FROM  att_details_cable_info a join core_planner_logs logs on logs.cable_id = a.cable_id  where a.cable_id=v_cable_details_left.system_id and a.a_end_status_id=1 and a.b_end_status_id=1 limit required_core;
               

		
		end if;

            
        END LOOP;

           raise info 'end loop 1 : %',p_user_id;
          
 UPDATE temp_connection tmp
SET source_network_id = pm.common_name
FROM point_master pm
WHERE pm.entity_type IN ('SpliceClosure', 'FMS','FDB','BDB')
  AND pm.system_id = tmp.source_system_id ;

  UPDATE temp_connection tmp
SET equipment_network_id  = pm.common_name
FROM point_master pm
WHERE pm.entity_type IN ('SpliceClosure', 'FMS','FDB','BDB')
  AND pm.system_id = tmp.equipment_system_id ;
 
if (SELECT COUNT(*) FROM temp_connection) > 1
then 
 raise info 'end splicing 1 : %',p_user_id;
perform(fn_auto_provisioning_save_connections());


FOR rec in (select destination_system_id,destination_port_no from temp_connection where destination_entity_type = 'Cable' and created_by = p_user_id
union 
select source_system_id, source_port_no from temp_connection where source_entity_type = 'Cable' and created_by = p_user_id)
loop 

perform fn_associate_fiber_link_cable(rec.destination_system_id,p_link_system_id,rec.destination_port_no,'A' );
	
end loop;
end if;	

 update core_planner_logs set used_core = used_core + required_core,avaiable= case when avaiable - required_core < 0 
 then 0 else avaiable - required_core end where user_id = p_user_id and is_valid =true;

return query select row_to_json(row) from ( select true as status, 'ODF to ODF feasibility successfully completed' as message ) row;
end if;
END;
$function$
;

