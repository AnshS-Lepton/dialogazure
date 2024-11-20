CREATE OR REPLACE FUNCTION public.fn_get_core_planner_splicing(required_core integer, p_user_id integer, fiber_link_network_id character varying)
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
BEGIN
    v_network_id := '';
    v_system_id := 0;
    p_link_system_id =0;
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
 
 update core_planner_logs set used_core = used_core + required_core,avaiable= case when avaiable - required_core < 0 then 0 else avaiable - required_core end where user_id = p_user_id and is_valid =true;


if (SELECT COUNT(*) FROM temp_connection) > 1
then 
 raise info 'end splicing 1 : %',p_user_id;
perform(fn_auto_provisioning_save_connections());

FOR i in 1..required_core
loop 
	 raise info 'end splicing 2 1 : %',p_user_id;

perform fn_associate_fiber_link_cable((select cable_id from core_planner_logs where user_id = p_user_id limit 1),p_link_system_id,i,'A' );
	
end loop;

 raise info 'end loop fiberlink 2 : %',p_user_id;

end if;	


return query select row_to_json(row) from ( select true as status, 'Success' as message ) row;



END;
$function$
;


--------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation(source_network_id character varying, destination_network_id character varying, buffer integer, required_core integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
    rec RECORD;
    total_core integer;
    p_source character varying;
    p_destination character varying;
    avaiableCore integer;
    usedCore integer;
    source_system_id integer;
   p_destination_system_id integer;
BEGIN
    avaiableCore := 0;
    usedCore := 0;

    -- Create temporary tables
    CREATE TEMP TABLE temp_cable_routes(
        seq integer,
        path_seq integer, 
        edge_targetid integer, 
        roadline_geomtext text, 
        start_point character varying,
        end_point character varying
    ) ON COMMIT DROP;

    CREATE TEMP TABLE temp_isp_port_info(
        system_id int4 NULL,
        network_id varchar NULL,
        parent_system_id int4 NULL,
        parent_network_id varchar NULL,
        port_status_id int4 NULL,
        is_valid boolean DEFAULT true,
        port_number int4 NULL
    ) ON COMMIT DROP;

    -- Clear previous logs for the user
    DELETE FROM core_planner_logs WHERE user_id = p_user_id;

    -- Get source and destination points
    SELECT longitude || ' ' || latitude AS geom, system_id
    INTO p_source, source_system_id
    FROM att_details_fms
    WHERE network_id = source_network_id;

    SELECT longitude || ' ' || latitude AS geom,system_id
    INTO p_destination,p_destination_system_id
    FROM att_details_fms
    WHERE network_id = destination_network_id;

    -- Calculate available and used cores
    SELECT 
        COUNT(CASE WHEN port_status_id = 1 and input_output = 'O' THEN 1 END) AS avaiableCore,
        COUNT(CASE WHEN port_status_id > 1 and input_output = 'O' THEN 1 END) AS usedCore
    INTO avaiableCore, usedCore
    FROM isp_port_info a
    WHERE a.parent_system_id = source_system_id 
      AND a.parent_entity_type = 'FMS';

    -- Populate temporary cable routes table
    INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, roadline_geomtext, start_point, end_point)
    SELECT * FROM fn_sf_get_routes(p_source, p_destination, buffer, buffer);

    -- Populate temporary ISP port info table
    INSERT INTO temp_isp_port_info (system_id, network_id, parent_system_id, 
    parent_network_id, port_status_id, port_number)
    
    SELECT system_id, network_id, parent_system_id, parent_network_id, port_status_id, port_number
    FROM isp_port_info a WHERE a.parent_system_id = source_system_id  AND a.parent_entity_type = 'FMS'
    AND port_status_id = 1 and input_output = 'O' ORDER BY system_id;
   
   UPDATE temp_isp_port_info
   SET is_valid = FALSE
   WHERE system_id IN (
    SELECT system_id
    FROM temp_isp_port_info t
    WHERE ((SELECT COUNT(*) FROM att_details_cable_info  WHERE cable_id IN (SELECT edge_targetid 
        FROM temp_cable_routes) AND fiber_number = t.port_number  AND a_end_status_id = 1 
        AND b_end_status_id = 1 ) != (SELECT COUNT(*) FROM temp_cable_routes))
        and
        ((SELECT COUNT(*) FROM att_details_cable_info  WHERE cable_id IN (SELECT edge_targetid 
        FROM temp_cable_routes) AND fiber_number = (SELECT port_number FROM isp_port_info it
        where  it.parent_system_id = p_destination_system_id  AND it.parent_entity_type = 'FMS'
        and it.port_number = t.port_number and input_output = 'O' AND port_status_id = 1)  
        AND a_end_status_id = 1 AND b_end_status_id = 1 ) != (SELECT COUNT(*) FROM temp_cable_routes)) );

   /* -- Validate ports against cable routes
    FOR rec IN (SELECT * FROM temp_isp_port_info) LOOP
        IF (SELECT COUNT(*) FROM att_details_cable_info  WHERE cable_id IN (SELECT edge_targetid 
        FROM temp_cable_routes) AND fiber_number = rec.port_number  AND a_end_status_id = 1 
        AND b_end_status_id = 1 ) != (SELECT COUNT(*) FROM temp_cable_routes)
        THEN
            UPDATE temp_isp_port_info
            SET is_valid = FALSE
            WHERE system_id = rec.system_id;
        END IF;
       
       IF (SELECT COUNT(*) FROM att_details_cable_info  WHERE cable_id IN (SELECT edge_targetid 
        FROM temp_cable_routes) AND fiber_number = (SELECT port_number FROM isp_port_info it where  it.parent_system_id = p_destination_system_id  AND it.parent_entity_type = 'FMS' and it.port_number = rec.port_number 
        and input_output = 'O' AND port_status_id = 1  )  AND a_end_status_id = 1 
        AND b_end_status_id = 1 ) != (SELECT COUNT(*) FROM temp_cable_routes)
        THEN
            UPDATE temp_isp_port_info
            SET is_valid = FALSE
            WHERE system_id = rec.system_id;
        END IF;
       
    END LOOP; */
   
          -- Log valid cables
        INSERT INTO core_planner_logs( cable_id, cable_name, network_status, total_core, cable_length, 
        a_system_id, b_system_id, a_entity_type, b_entity_type, error_msg, is_valid, user_id, 
        a_network_id, b_network_id, cable_network_id, avaiable, used_core )
        
        SELECT att.system_id, att.cable_name, att.network_status, 
        att.total_core, att.cable_calculated_length, att.a_system_id, att.b_system_id, att.a_entity_type,
        att.b_entity_type, CASE WHEN (SELECT COUNT(*) FROM temp_isp_port_info WHERE is_valid = TRUE) 
        >= required_core THEN 'Available Port' ELSE 'Required core is not available' end, 
        (SELECT COUNT(*) FROM temp_isp_port_info WHERE is_valid = TRUE) >= required_core, 
        p_user_id, att.a_network_id, att.b_network_id, att.network_id, avaiableCore, usedCore FROM att_details_cable att WHERE att.system_id in 
        (select edge_targetid from temp_cable_routes);


      -- Validate if any record has 'FMS' in either `a_entity_type` or `b_entity_type`
    IF NOT EXISTS ( SELECT 1 FROM core_planner_logs logs WHERE (logs.a_entity_type = 'FMS' 
    OR logs.b_entity_type = 'FMS') AND logs.user_id = p_user_id ORDER BY logs.id ASC LIMIT 1) 
    THEN 
        UPDATE core_planner_logs 
        SET is_valid = FALSE 
        WHERE user_id = p_user_id;
    END IF;
   
    -- Validate if `a_system_id` or `b_system_id` is null
    IF EXISTS ( SELECT 1 FROM core_planner_logs logs WHERE logs.a_system_id IS NULL OR logs.b_system_id
    IS NULL AND logs.user_id = p_user_id) 
    THEN      
    UPDATE core_planner_logs log2 SET is_valid = FALSE WHERE user_id = p_user_id 
    AND (a_system_id IS NULL OR b_system_id IS NULL)
    AND EXISTS ( SELECT 1 FROM core_planner_logs log1 
    WHERE log1.cable_id = log2.cable_id AND log1.id = log2.id);
    END IF;

    -- Return result
    IF (SELECT COUNT(*) FROM core_planner_logs WHERE user_id = p_user_id AND is_valid = TRUE) > 0 THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT true AS status, 'Required Core avaiable' AS message) row;
    ELSE
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'No found avaiable core' AS message) row;
    END IF;

END;
$function$
;

 