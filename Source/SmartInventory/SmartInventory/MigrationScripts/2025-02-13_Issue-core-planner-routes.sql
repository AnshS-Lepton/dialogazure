CREATE OR REPLACE FUNCTION public.fn_update_routing_topology(p_cable_id integer, p_operation text)
 RETURNS TABLE(status boolean, message text)
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_existing_count INTEGER;
    v_result TEXT;
    v_geom GEOMETRY;
    v_cost DOUBLE PRECISION;
    v_reverse_cost DOUBLE PRECISION;
BEGIN
    SELECT 
        ST_Force2D(lm.sp_geometry) AS geom,
        ST_Length(lm.sp_geometry::geography) / 1000 AS cost,
        ST_Length(lm.sp_geometry::geography) / 1000 AS reverse_cost
    INTO v_geom, v_cost, v_reverse_cost
    FROM line_master lm
    JOIN vw_att_details_cable c 
        ON lm.system_id = c.system_id
    WHERE lm.entity_type = 'Cable'
    AND c.system_id = p_cable_id;



IF p_cable_id = 0 THEN

            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph' AS message;
            END IF;

            RETURN QUERY SELECT TRUE AS status, 'Routing topology updated' AS message;

else
	IF v_geom IS NULL THEN
	        RETURN QUERY SELECT FALSE AS status, 'Cable ID not found or no geometry data available.' AS message;
	    END IF;

    IF p_operation = 'INSERT' THEN
        SELECT COUNT(*) INTO v_existing_count
        FROM routing_data_core_plan
        WHERE id = p_cable_id;

        IF v_existing_count = 0 THEN
            INSERT INTO routing_data_core_plan (id, system_id, source, target, cost, reverse_cost, geom)
            VALUES (p_cable_id, p_cable_id, NULL, NULL, v_cost, v_reverse_cost, v_geom);

            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for INSERT' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for INSERT' AS message;
            END IF;

            RETURN QUERY SELECT TRUE AS status, 'Cable inserted successfully and graph updated' AS message;
        ELSE
            RETURN QUERY SELECT FALSE AS status, 'Cable ID already exists for INSERT operation' AS message;
        END IF;

    ELSIF p_operation = 'UPDATE' THEN
        SELECT COUNT(*) INTO v_existing_count
        FROM routing_data_core_plan
        WHERE id = p_cable_id;

        IF v_existing_count > 0 THEN
            UPDATE routing_data_core_plan
            SET geom = v_geom, cost = v_cost, reverse_cost = v_reverse_cost, source = NULL, target = NULL
            WHERE id = p_cable_id;

            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for UPDATE' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for UPDATE' AS message;
            END IF;
           
            -- remove unused vertices					
			delete from routing_data_core_plan_vertices_pgr where id in(					
				select v.id from routing_data_core_plan_vertices_pgr v left join routing_data_core_plan e
				on v.id = e.source or v.id = e.target where e.id is null);

            RETURN QUERY SELECT TRUE AS status, 'Cable updated successfully and graph updated' AS message;
        ELSE
            RETURN QUERY SELECT FALSE AS status, 'Cable ID does not exist for UPDATE operation' AS message;
        END IF;

    ELSIF p_operation = 'DELETE' THEN
        SELECT COUNT(*) INTO v_existing_count
        FROM routing_data_core_plan
        WHERE id = p_cable_id;

        IF v_existing_count > 0 THEN
            DELETE FROM routing_data_core_plan
            WHERE id = p_cable_id;

            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for DELETE' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for DELETE' AS message;
            END IF;
           
            -- remove unused vertices					
			delete from routing_data_core_plan_vertices_pgr where id in(					
				select v.id from routing_data_core_plan_vertices_pgr v left join routing_data_core_plan e
				on v.id = e.source or v.id = e.target where e.id is null);

            RETURN QUERY SELECT TRUE AS status, 'Cable deleted successfully and graph updated' AS message;
        ELSE
            RETURN QUERY SELECT FALSE AS status, 'Cable ID does not exist for DELETE operation' AS message;
        END IF;
    ELSE
        RETURN QUERY SELECT FALSE AS status, 'Invalid operation type. Please use INSERT, UPDATE, or DELETE.' AS message;
    END IF;
   END IF;
END;
$function$
;


-----------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation(source_network_id character varying, destination_network_id character varying, buffer integer, required_core integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
    rec RECORD;
    total_core integer;
    p_source character varying;
    p_destination character varying;
    availableCoreDestination integer;
    usedCore integer;
    p_source_system_id integer;
    p_destination_system_id integer;
    availableCoreSource integer;
   t_count integer;
  cableNetworkId character varying;
BEGIN
    availableCoreDestination := 0;
    usedCore := 0;
    t_count :=0;
    -- Create temporary tables
       CREATE temp TABLE temp_cable_routes(
        seq integer,
        path_seq integer, 
        edge_targetid integer, 
        roadline_geomtext text, 
        start_point character varying,
        end_point character varying,
        message varchar NULL,
        is_valid boolean DEFAULT true,
        avaiable_core_count integer DEFAULT 0,
        user_id integer
    ) ON COMMIT DROP;

    CREATE TEMP TABLE temp_isp_port_info(
        system_id int4 NULL,
        network_id varchar NULL,
        parent_system_id int4 NULL,
        parent_network_id varchar NULL,
        port_status_id int4 NULL,
        is_valid boolean DEFAULT true,
        port_number int4 null,
        parent_entity_type varchar NULL
    ) ON COMMIT DROP;

    -- Clear previous logs for the user
    DELETE FROM core_planner_logs WHERE user_id = p_user_id;
   
   -- Get source and destination points
   -- Check if source exists in FMS, otherwise get it source from SpliceClosure
   IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) THEN
     SELECT longitude || ' ' || latitude AS geom, system_id
     INTO p_source, p_source_system_id
     FROM att_details_fms
     WHERE network_id = source_network_id;
   
       -- Calculate the available cores for the source system
    SELECT   
        COUNT(CASE WHEN port_status_id = 1 and input_output = 'O' THEN 1 END) AS availableCore,
        COUNT(CASE WHEN port_status_id > 1 and input_output = 'O' THEN 1 END) AS usedCore
     INTO availableCoreSource, usedCore
     FROM isp_port_info
     WHERE parent_system_id = p_source_system_id 
     AND parent_entity_type = 'FMS';
     
   else
    p_source_system_id =0;
     -- Get SpliceClosure From the Source
     SELECT longitude || ' ' || latitude AS geom
     INTO p_source
     FROM att_details_spliceclosure
     WHERE network_id = source_network_id;
   END IF;

   -- Check if destination exists in FMS, get it destination from SpliceClosure
   IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
    SELECT longitude || ' ' || latitude AS geom, system_id
    INTO p_destination, p_destination_system_id
    FROM att_details_fms
    WHERE network_id = destination_network_id;
   
       -- Calculate the available cores for the destination system
    SELECT COUNT(*)
    INTO availableCoreDestination
    FROM isp_port_info
    WHERE parent_system_id = p_destination_system_id 
      AND parent_entity_type = 'FMS' 
      AND port_status_id = 1 
      AND input_output = 'O';     
   else
   p_destination_system_id =0;
    -- Get SpliceClosure From the Destination
    SELECT longitude || ' ' || latitude AS geom
    INTO p_destination
    FROM att_details_spliceclosure
    WHERE network_id = destination_network_id;
   END IF;

    -- check if Both FMS does not exist        
    IF NOT EXISTS (
    SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) and
    NOT EXISTS (
    SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'At least one ODF is required. Please provide a valid ODF.' AS message) row;
    END IF;
    
     -- Check if source FMS does not exist
   IF NOT EXISTS ( SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) THEN
    -- Check if source network_id exists in att_details_spliceclosure
    IF NOT EXISTS (
        SELECT 1 FROM att_details_spliceclosure WHERE network_id = source_network_id) THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (SELECT false AS status, 
           'Please enter a valid ODF/Splice Closure.' AS message) row;
    END IF;
   END IF;

   -- Check if destination FMS does not exist
   IF NOT EXISTS (
    SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
    -- Check if destination network_id exists in att_details_spliceclosure
    IF NOT EXISTS (SELECT 1 FROM att_details_spliceclosure WHERE network_id = destination_network_id) THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (SELECT false AS status, 
           'Please enter a valid ODF/Splice Closure.' AS message) row;
    END IF;
  END IF;
   
    -- Check if both FMS (source and destination) exist
    IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
    AND EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
     -- Validate Core/ Port if both counts match the required core for source and destination
     IF availableCoreSource < required_core OR availableCoreDestination < required_core THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (
            SELECT false AS status, 
           'Required ports are unavailable on the ODF. Please check and update the port availability.' 
            AS message) row;
     END IF;   
    -- Check if only source FMS exists
    ELSIF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) THEN
     -- Validate Core/ Port if source count matches the required core
     IF availableCoreSource < required_core THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (
            SELECT false AS status, 
            'Required ports are unavailable on the ODF. Please check and update the port availability.' 
            AS message) row;
     END IF;   
    -- Check if only destination FMS exists
    ELSIF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
    -- Validate Core/ Port if destination count matches the required core
    IF availableCoreDestination < required_core THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (
            SELECT false AS status, 
            'Required ports are unavailable on the ODF. Please check and update the port availability.' 
            AS message) row;
     END IF;   
    END IF;

	 -- Populate temporary cable routes table
      INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id)
		   
	  SELECT seq, path_seq, edge,p_user_id FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	  FROM routing_data_core_plan', (SELECT id
	  FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_source || ')', 4326), 3))
	  limit 1 ),(SELECT id FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, 
	  ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_destination ||')', 4326), 3)) 
	  limit 1));
	
    delete from temp_cable_routes where edge_targetid = -1 and user_id = p_user_id;

   if not exists (select 1 from temp_cable_routes where user_id = p_user_id) 
   then

    RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'There is no existing route between the specified ODFs or Splice Closure.' AS message) row;
    RETURN;
    end if;
   
    -- Populate temporary ISP port info table
    INSERT INTO temp_isp_port_info (system_id, network_id, parent_system_id, 
    parent_network_id,   port_status_id, port_number,parent_entity_type)
    SELECT system_id, network_id, parent_system_id, parent_network_id, 
    port_status_id, port_number,parent_entity_type
    FROM isp_port_info a 
    WHERE a.parent_system_id = CASE 
        WHEN p_source_system_id !=0 THEN p_source_system_id 
        ELSE p_destination_system_id 
    END
    AND a.parent_entity_type = 'FMS'
    AND port_status_id = 1 
    AND input_output = 'O'
    ORDER BY system_id;

    -- Loop through each record in temp_isp_port_info
    FOR rec IN (SELECT * FROM temp_isp_port_info)
    LOOP
        -- Update temp_cable_routes for mismatched cable_ids
        UPDATE temp_cable_routes
        SET           
            avaiable_core_count = COALESCE(avaiable_core_count, 0) + 1
	   WHERE edge_targetid IN ( 
	   SELECT DISTINCT aci.cable_id FROM att_details_cable_info aci
       WHERE aci.cable_id IN ( SELECT edge_targetid FROM temp_cable_routes 
       WHERE user_id = p_user_id )
       AND aci.fiber_number = rec.port_number AND (aci.a_end_status_id = 1 or (aci.is_a_end_through_connectivity = true and aci.a_end_status_id =2)) 
       AND (aci.b_end_status_id = 1 or (aci.is_b_end_through_connectivity = true and aci.b_end_status_id =2))) and  user_id = p_user_id;

    if ( (select count(1) from (SELECT DISTINCT aci.cable_id FROM att_details_cable_info aci
    WHERE aci.cable_id IN ( SELECT edge_targetid FROM temp_cable_routes 
    WHERE user_id = p_user_id )
    AND aci.fiber_number = rec.port_number  
    AND (aci.a_end_status_id = 1 or (aci.is_a_end_through_connectivity = true and aci.a_end_status_id =2)) 
       AND (aci.b_end_status_id = 1 or (aci.is_b_end_through_connectivity = true and aci.b_end_status_id =2)))a) = (SELECT count(1) FROM temp_cable_routes where  user_id = p_user_id))
   then
  
   IF  (p_source_system_id != 0 AND p_destination_system_id != 0 )
   then 
     update isp_port_info set is_valid_for_core_plan=true where parent_system_id=p_destination_system_id
     and parent_entity_type=rec.parent_entity_type and port_number = rec.port_number AND input_output = 'O'
     and port_status_id = 1;
   else 
     update isp_port_info set is_valid_for_core_plan=true where parent_system_id=rec.parent_system_id
     and parent_entity_type=rec.parent_entity_type and port_number=rec.port_number AND input_output = 'O';
  end if;
 
  end if;
  END LOOP;

   IF  (p_source_system_id != 0 AND p_destination_system_id != 0 ) 
      then
     update isp_port_info set is_valid_for_core_plan=true where parent_system_id = p_source_system_id
     and parent_entity_type='FMS' and port_number in (SELECT port_number FROM isp_port_info
     WHERE parent_system_id = p_destination_system_id  AND parent_entity_type = 'FMS' AND 
     input_output = 'O' AND port_status_id = 1  AND is_valid_for_core_plan = true) AND input_output = 'O'
     and port_status_id = 1;
   end if;

   update temp_cable_routes set message = 'Required core is unavailable', is_valid = false where 
   avaiable_core_count  <  required_core and user_id = p_user_id;
 
 if (SELECT count(*) 
    FROM temp_cable_routes 
    WHERE avaiable_core_count >= required_core 
    AND user_id = p_user_id) = (select count(*) from temp_cable_routes)
    then
    IF p_source_system_id IS NOT NULL AND p_source_system_id != 0
      AND p_destination_system_id IS NOT NULL AND p_destination_system_id != 0 
      then
  if(SELECT COUNT(1) FROM isp_port_info 
    WHERE parent_system_id = p_destination_system_id 
      AND is_valid_for_core_plan = true 
      AND parent_entity_type = 'FMS'
      AND input_output = 'O' AND port_status_id = 1) < required_core
      then 
       RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Required ports are unavailable on the ODF. Please check and update the port availability.' AS message) row;
RETURN;
      end if;
    end if;
   end if;
    -- Log valid cables
    INSERT INTO core_planner_logs(
        cable_id, cable_name, network_status, total_core, cable_length, 
		--a_system_id, b_system_id, a_entity_type, b_entity_type, 
		error_msg, is_valid, user_id, 
       -- a_network_id, b_network_id, 
		cable_network_id, avaiable, used_core
    )
    SELECT 
        att.system_id, att.cable_name, att.network_status, att.total_core, att.cable_calculated_length,
        --att.a_system_id, att.b_system_id, att.a_entity_type, att.b_entity_type, 
		info.message, info.is_valid,
        p_user_id, --att.a_network_id, att.b_network_id, 
		att.network_id, 
        (SELECT COUNT(*) FROM att_details_cable_info aci WHERE aci.cable_id = att.system_id
         AND aci.a_end_status_id = 1 AND aci.b_end_status_id = 1) AS available_cores,
        (SELECT COUNT(*) FROM att_details_cable_info aci WHERE aci.cable_id = info.edge_targetid
         AND (aci.a_end_status_id > 1 or aci.b_end_status_id > 1 ) ) AS used_core
    FROM att_details_cable att 
    LEFT JOIN temp_cable_routes info ON att.system_id = info.edge_targetid and  info.user_id = p_user_id
    WHERE att.system_id IN (SELECT edge_targetid FROM temp_cable_routes where user_id = p_user_id);

    -- Update system_id and entity_type for a and b ends
	
	with startCte as(
   SELECT pm.system_id,pm.entity_type,pm.common_name,cable_id 
                       FROM core_planner_logs
                       INNER JOIN line_master lm ON lm.system_id = core_planner_logs.cable_id 
                       AND lm.entity_type = 'Cable' 
						inner join  point_master pm 
                       on ST_WITHIN(pm.sp_geometry, ST_BUFFER_METERS(ST_STARTPOINT(lm.sp_geometry), 3)) 
                       AND pm.entity_type IN ('BDB','FDB','SpliceClosure','FMS') 
                       WHERE core_planner_logs.user_id = p_user_id )
    UPDATE core_planner_logs 
    SET 
        a_system_id = startCte.system_id,
        a_entity_type = startCte.entity_type,
		a_network_id=startCte.common_name
		from startCte
     WHERE core_planner_logs.user_id = p_user_id and core_planner_logs.cable_id=startCte.cable_id;

with endCte as(
SELECT pm.system_id,pm.entity_type,pm.common_name,cable_id 
                       FROM core_planner_logs
                       INNER JOIN line_master lm ON lm.system_id = core_planner_logs.cable_id 
                       AND lm.entity_type = 'Cable' 
						inner join  point_master pm 
                       on ST_WITHIN(pm.sp_geometry, ST_BUFFER_METERS(ST_ENDPOINT(lm.sp_geometry), 3)) 
                       AND pm.entity_type IN ('BDB','FDB','SpliceClosure','FMS') 
                       WHERE core_planner_logs.user_id = p_user_id )
    UPDATE core_planner_logs 
    SET 
        b_system_id = endCte.system_id,
        b_entity_type = endCte.entity_type,
		b_network_id=endCte.common_name
		from endCte
    WHERE core_planner_logs.user_id = p_user_id and core_planner_logs.cable_id=endCte.cable_id;

    -- Mark invalid records if no termination points
    UPDATE core_planner_logs 
    SET is_valid = false 
    WHERE coalesce(b_system_id, 0) = 0 OR coalesce(a_system_id, 0) = 0 
    AND user_id = p_user_id;

       -- Validate if both FMS points are found
    if NOT EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
   OR NOT EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) 
    then
       select logs.cable_network_id  into cableNetworkId from core_planner_logs logs inner join att_details_fms fms 
       on logs.a_system_id = fms.system_id or logs.b_system_id = fms.system_id 
       WHERE fms.system_id not in (p_source_system_id ,p_destination_system_id ) limit 1;
      if cableNetworkId is not null
      then 
       RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Splice Closure is not found at the end of the cable '||cableNetworkId  AS message) row; 
      end if;
    end if ;
   
    -- Validate if both FMS points are found
    IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
    AND EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
	IF (select COUNT(1)!= 2 from (
	SELECT *
        FROM core_planner_logs a WHERE a.a_entity_type = 'FMS' and a.user_id = p_user_id

        union all
        select * from core_planner_logs b
        WHERE b.b_entity_type = 'FMS' AND b.user_id = p_user_id)t)  
         THEN
        -- Invalidate all records for the user
        UPDATE core_planner_logs
        SET is_valid = FALSE
        WHERE user_id = p_user_id;
    END IF; 
   end if;
  
    -- Validate if `a_system_id` or `b_system_id` is null
  UPDATE core_planner_logs log2
  SET 
  is_valid = FALSE,
  error_msg = 'Cable is not terminated properly'
  FROM core_planner_logs log1
  WHERE log2.user_id = p_user_id
  AND log2.user_id = log1.user_id
  AND (log2.a_system_id IS NULL OR log2.b_system_id IS NULL)
  AND log2.cable_id = log1.cable_id;

    -- Return result
    IF exists (SELECT 1 FROM core_planner_logs WHERE user_id = p_user_id AND is_valid = false )  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Required core is unavailable from '||source_network_id ||' to '||destination_network_id  AS message) row;
    ELSE
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT true AS status, 'Required Core available' AS message) row;
    END IF;

END;
$function$
;

--------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_trg_audit_line_master()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$

declare

v_served_by_ring text;

v_dsa_system_id integer;

v_layer_table text;
v_subarea_system_id integer;

begin

                v_dsa_system_id:=0;

IF (TG_OP = 'INSERT' ) THEN 

 

				INSERT INTO public.audit_line_master(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,is_virtual,action,modified_by,gis_design_id)

				values(new.system_id,new.entity_type,new.approval_flag,new.sp_geometry,new.creator_remark,new.approver_remark,new.created_by,new.approver_id,new.common_name,new.db_flag,new.approval_date,now(),new.network_status,new.is_virtual,'I',new.modified_by,new.gis_design_id);

 

                select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(new.entity_type);

                select system_id into v_dsa_system_id  from polygon_master where entity_type='DSA' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1;                    
                update att_details_subarea set is_association_completed=false where system_id in(select system_id from polygon_master where entity_type='SubArea' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1);

				if(coalesce(v_dsa_system_id,0)>0)

                then

                                select served_by_ring into v_served_by_ring from att_details_dsa where system_id=v_dsa_system_id;

                                execute 'update '||v_layer_table||' set served_by_ring='''||v_served_by_ring||''' where system_id='||new.system_id||' ';

                end if;
				-- IF (new.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 				THEN
-- 					PERFORM fn_update_entity_geojson('INSERT',new.entity_type,'Line',new.SYSTEM_ID, new.sp_geometry);
-- 				END IF;
    if(upper(new.entity_type) = 'CABLE') then
       PERFORM fn_update_routing_topology(new.system_id,'INSERT');
   end if;
END IF;

 

IF (TG_OP = 'UPDATE' ) THEN 

IF((select st_astext(new.sp_geometry)) !=(select st_astext(old.sp_geometry))) then

 

                INSERT INTO public.audit_line_master(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,is_virtual,action,modified_by,gis_design_id)

                values(new.system_id,new.entity_type,new.approval_flag,new.sp_geometry,new.creator_remark,new.approver_remark,new.created_by,new.approver_id,new.common_name,new.db_flag,new.approval_date,now(),new.network_status,new.is_virtual,'U',new.modified_by,new.gis_design_id); 

 

                select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(new.entity_type);

                select system_id into v_dsa_system_id  from polygon_master where entity_type='DSA' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1;                    
                update att_details_subarea set is_association_completed=false where system_id in(select system_id from polygon_master where entity_type='SubArea' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1);

                if(coalesce(v_dsa_system_id,0)>0)

                then

                                select served_by_ring into v_served_by_ring from att_details_dsa where system_id=v_dsa_system_id;

                                execute 'update '||v_layer_table||' set served_by_ring='''||v_served_by_ring||''' where system_id='||new.system_id||' ';

                end if;
				
				-- IF (new.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 				THEN
-- 					PERFORM fn_update_entity_geojson('UPDATE', new.entity_type, 'Line', new.SYSTEM_ID, new.sp_geometry);
-- 				END IF;
END IF;
    if(upper(new.entity_type) = 'CABLE') then
        perform fn_update_routing_topology(new.system_id,'UPDATE');
    END IF;
END IF;

 
IF (TG_OP = 'DELETE' ) THEN 
-- 		IF (old.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 		THEN
-- 			PERFORM fn_update_entity_geojson('DELETE', old.entity_type, 'Line', old.SYSTEM_ID, old.sp_geometry);
-- 		END IF;
  if(upper(old.entity_type) = 'CABLE') then
    PERFORM fn_update_routing_topology(old.system_id,'DELETE');
   END IF;
	RETURN OLD;
END IF;
RETURN NEW;

END;

$function$
;
