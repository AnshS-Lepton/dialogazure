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
   v_source INTEGER;
   v_target INTEGER;
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

        IF v_existing_count > 0 then
        
        SELECT "source" ,target INTO v_source,v_target
        FROM routing_data_core_plan
        WHERE id = p_cable_id;
       
        update routing_data_core_plan set "source" = null, target = null where "source"  in (v_source,v_target)
       		or target in (v_source,v_target);  
      
       	delete from routing_data_core_plan_vertices_pgr where id in (v_source,v_target);         	
       
            UPDATE routing_data_core_plan
            SET geom = v_geom, cost = v_cost, reverse_cost = v_reverse_cost
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
