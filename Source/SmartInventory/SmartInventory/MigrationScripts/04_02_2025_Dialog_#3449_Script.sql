/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: 
ModifiedOn: 04 Feb 2025
ModifiedBy: Navika Jaiswal
Purpose: updated validation for core planner port
------------------------------------------*/
-- DROP FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4);

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
    SELECT longitude || ' ' || latitude AS geom, system_id
    INTO p_source, p_source_system_id
    FROM att_details_fms
    WHERE network_id = source_network_id;

    SELECT longitude || ' ' || latitude AS geom, system_id
    INTO p_destination, p_destination_system_id
    FROM att_details_fms
    WHERE network_id = destination_network_id;

    -- Calculate the available cores for the source system
    SELECT  
        COUNT(CASE WHEN port_status_id = 1 and input_output = 'O' THEN 1 END) AS availableCore,
        COUNT(CASE WHEN port_status_id > 1 and input_output = 'O' THEN 1 END) AS usedCore
    INTO availableCoreSource, usedCore
    FROM isp_port_info
    WHERE parent_system_id = p_source_system_id 
      AND parent_entity_type = 'FMS';

    -- Calculate the available cores for the destination system
    SELECT COUNT(*)
    INTO availableCoreDestination
    FROM isp_port_info
    WHERE parent_system_id = p_destination_system_id 
      AND parent_entity_type = 'FMS' 
      AND port_status_id = 1 
      AND input_output = 'O';

      -- Validate if both counts match the requiredCore
    if not exists (select 1 from att_details_fms where network_id = source_network_id) THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'The ODF1 does not exist. Please enter a valid ODF.' AS message) row;
    END IF;
   
    if not exists (select 1 from att_details_fms where network_id = destination_network_id) THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'The ODF2 does not exist. Please enter a valid ODF.' AS message) row;
    END IF;
   
    -- Validate if both counts match the requiredCore
    IF availableCoreSource < required_core OR availableCoreDestination < required_core THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Required ports are unavailable on the ODF. Please check and update the port availability.' AS message) row;
       RETURN;
    END IF;

	    -- Populate temporary cable routes table
		  INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id)
		   
		SELECT seq, path_seq, edge,p_user_id FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	    FROM routing_data_core_plan', (SELECT id
		FROM routing_data_core_plan_vertices_pgr
		WHERE ST_Within(
		    the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_source || ')', 4326), 2)
		)limit 1 ),
		(SELECT id
		FROM routing_data_core_plan_vertices_pgr
		WHERE ST_Within(
		    the_geom, 
		    ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_destination ||')', 4326), 2)
		) limit 1 )
		);
	
delete from temp_cable_routes where edge_targetid = -1 and user_id = p_user_id;

if not exists (select 1 from temp_cable_routes where user_id = p_user_id) 
then

 RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'There is no existing route between the ODFs.' AS message) row;
RETURN;
 
end if;
    -- Populate temporary ISP port info table
    INSERT INTO temp_isp_port_info (system_id, network_id, parent_system_id, parent_network_id, port_status_id, port_number,parent_entity_type)
    SELECT system_id, network_id, parent_system_id, parent_network_id, port_status_id, port_number,parent_entity_type
    FROM isp_port_info a 
    WHERE a.parent_system_id = p_source_system_id  
    AND a.parent_entity_type = 'FMS'
    AND port_status_id = 1 
    AND input_output = 'O'
    ORDER BY system_id;

    -- Loop through each record in temp_isp_port_info
 FOR rec IN (SELECT * FROM temp_isp_port_info) LOOP

        -- Update temp_cable_routes for mismatched cable_ids
        UPDATE temp_cable_routes
        SET           
            avaiable_core_count = COALESCE(avaiable_core_count, 0) + 1
			
        WHERE edge_targetid IN (
    SELECT DISTINCT aci.cable_id
    FROM att_details_cable_info aci
    WHERE aci.cable_id IN (
        SELECT edge_targetid 
        FROM temp_cable_routes 
        WHERE user_id = p_user_id
    )
    AND aci.fiber_number = rec.port_number  
    AND aci.a_end_status_id = 1
    AND aci.b_end_status_id = 1
) and  user_id = p_user_id;

if ((select count(1) from (
    SELECT DISTINCT aci.cable_id
    FROM att_details_cable_info aci
    WHERE aci.cable_id IN (
        SELECT edge_targetid 
        FROM temp_cable_routes 
        WHERE user_id = p_user_id
    )
    AND aci.fiber_number = rec.port_number  
    AND aci.a_end_status_id = 1
    AND aci.b_end_status_id = 1
)a) = (SELECT count(1) FROM temp_cable_routes where  user_id = p_user_id))
then

--update isp_port_info set is_valid_for_core_plan=true where parent_system_id=rec.parent_system_id
--and parent_entity_type=rec.parent_entity_type and port_number=rec.port_number AND input_output = 'O';

update isp_port_info set is_valid_for_core_plan=true where parent_system_id=p_destination_system_id
and parent_entity_type=rec.parent_entity_type and port_number = rec.port_number AND input_output = 'O'
and port_status_id = 1;

end if;

END LOOP;

update isp_port_info set is_valid_for_core_plan=true where parent_system_id = p_source_system_id
and parent_entity_type='FMS' and port_number in (SELECT port_number FROM isp_port_info
    WHERE parent_system_id = p_destination_system_id  AND parent_entity_type = 'FMS' AND input_output = 'O'  AND port_status_id = 1  AND is_valid_for_core_plan = true) AND input_output = 'O' and port_status_id = 1;

   update temp_cable_routes set message = 'Required core is not available', is_valid = false where 
  avaiable_core_count  <  required_core and user_id = p_user_id;
 
 if  (SELECT count(*) 
    FROM temp_cable_routes 
    WHERE avaiable_core_count >= required_core 
    AND user_id = p_user_id) = (select count(*) from temp_cable_routes)
    THEN
  if (SELECT COUNT(1) FROM isp_port_info 
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
	IF ( select COUNT(1)!= 2 from (
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
        FROM (SELECT false AS status, 'Required core is not available from '||source_network_id ||' to '||destination_network_id  AS message) row;
    ELSE
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT true AS status, 'Required Core available' AS message) row;
    END IF;

END;
$function$
;
---------------------------------------------------------------------------------------------------
/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: 
ModifiedOn: 04 Feb 2025
ModifiedBy: Chandra Shekhar
Purpose: Changed the datatype to BigInt also increased the length to 15 character long
------------------------------------------*/
CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_prefix(p_link_prefix character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    sql TEXT;
BEGIN
    -- Construct the SQL query dynamically
    sql := '
    WITH max_link AS (
        SELECT 
			--MAX(CAST(NULLIF(SUBSTRING(link_ID FROM ' || (LENGTH(p_link_prefix) + 1) || '), '''') AS INTEGER)) AS max_id
			MAX(CAST(NULLIF(REGEXP_REPLACE(SUBSTRING(link_ID FROM ' || (LENGTH(p_link_prefix) + 1) || '), ''[^0-9]'', '''', ''g''), '''') AS BIGINT)) AS max_id
        FROM 
            att_details_fiber_link
        WHERE 
            link_ID ILIKE ' || quote_literal(p_link_prefix || '%') || '
    )
    SELECT 
        ' || quote_literal(p_link_prefix) || ' || LPAD((COALESCE(max_id, 0) + 1)::TEXT, 10, ''0'') AS link_prefix
    FROM 
        max_link';

    -- Debugging Information
    RAISE INFO '%', sql;

    -- Execute the query and return the results
    RETURN QUERY EXECUTE 'SELECT row_to_json(row) FROM (' || sql || ') row';

END;
$function$
;