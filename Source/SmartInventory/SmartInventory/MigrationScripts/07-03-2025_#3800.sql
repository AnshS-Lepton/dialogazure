

-------------------------Create Function to get cable and fiberlink details-------------------------

CREATE OR REPLACE FUNCTION public.fn_get_cable_fiberlink_detail(
	p_entity_type character varying,
	p_entity_name character varying,
	p_network_id character varying,
	p_route_buffer integer)
    RETURNS jsonb
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE 
result_json jsonb;

    v_role_id integer;
    v_system_id integer;
    v_lat double precision;
    v_lng double precision;
    v_layer_table character varying;
    v_geometry_with_buffer geometry;
    v_geometry geometry;
    v_sql text;
BEGIN
    -- Create temporary tables
    CREATE TEMP TABLE IF NOT EXISTS temp_cable_result (
        cable_network_id character varying,
        cable_name character varying,
        entity_type character varying,
		linl_network_id character varying, 
        link_id character varying,
        link_name character varying
    ) ON COMMIT DELETE ROWS;

    
    -- Fetch cable details
    IF UPPER(p_entity_type) = 'CABLE' THEN
        INSERT INTO temp_cable_result (cable_network_id, cable_name, entity_type) 
        SELECT cb.network_id, cb.cable_name, 'Cable'
        FROM att_details_cable_info AS cbl
        LEFT JOIN att_details_cable AS cb ON cbl.cable_id = cb.system_id
        WHERE cb.network_id = p_network_id
          AND cb.cable_name = p_entity_name
          AND cbl.link_system_id <> 0
        GROUP BY cb.network_id, cb.cable_name;
    

else
    -- Fetch system_id for other entity types
    SELECT ld.layer_table 
    INTO v_layer_table 
    FROM layer_details AS ld  
    WHERE UPPER(ld.layer_name) = UPPER(p_entity_type);

    IF v_layer_table IS NULL THEN
        RAISE EXCEPTION 'Layer table not found for entity type: %', p_entity_type;
    END IF;

    -- Fetch system_id dynamically
    v_sql := 'SELECT system_id FROM ' || v_layer_table || ' WHERE network_id = $1';
    EXECUTE v_sql INTO v_system_id USING p_network_id;

    -- Fetch DUCT-related cables
    IF UPPER(p_entity_type) = 'DUCT' THEN
        INSERT INTO temp_cable_result (cable_network_id, cable_name, entity_type) 
        SELECT cb.network_id, cb.cable_name, 'Cable'
        FROM att_details_cable_info AS cbl
        LEFT JOIN att_details_cable AS cb ON cbl.cable_id = cb.system_id
        WHERE cb.duct_id = v_system_id
        GROUP BY cb.network_id, cb.cable_name;
    
else
    -- Fetch longitude and latitude for spatial search
    SELECT pm.longitude, pm.latitude 
    INTO v_lng, v_lat 
    FROM POINT_MASTER AS pm  
    WHERE UPPER(pm.entity_type) = UPPER(p_entity_type) 
      AND pm.system_id = v_system_id;

    -- Check if lat/lng are NULL before spatial operations
    IF v_lng IS NULL OR v_lat IS NULL THEN
        RAISE EXCEPTION 'Longitude/Latitude not found for entity type: % with system_id: %', p_entity_type, v_system_id;
    END IF;

    -- Create buffer geometry
    v_geometry := ST_SetSRID(ST_MakePoint(v_lng, v_lat), 4326);
    v_geometry_with_buffer := ST_Buffer(v_geometry::geography, p_route_buffer)::geometry;

    -- Fetch cables within buffer
    INSERT INTO temp_cable_result (cable_network_id, cable_name, entity_type) 
    SELECT 
        cbl.network_id, 
        cbl.cable_name, 
        'Cable' 
    FROM LINE_MASTER AS lm
    INNER JOIN layer_details AS ld ON ld.layer_name = lm.entity_type
    LEFT JOIN att_details_cable AS cbl 
        ON lm.system_id = cbl.system_id
    WHERE UPPER(lm.entity_type) = 'CABLE'
      AND ST_Intersects(ST_MakeValid(lm.sp_geometry), v_geometry_with_buffer)
    ORDER BY ST_Distance(
        ST_ClosestPoint(lm.sp_geometry, ST_SetSRID(ST_MakePoint(v_lng, v_lat), 4326)), 
        ST_SetSRID(ST_MakePoint(v_lng, v_lat), 4326)
    ) ASC;
	
	END IF;

  
	
	
END IF;

  -- Fetch fiber link details and insert into temp_fiber_result
    INSERT INTO temp_cable_result (cable_network_id,linl_network_id, link_id, link_name, entity_type)
    SELECT cb.network_id,fl.network_id, fl.link_id, fl.link_name, 'FiberLink'
    FROM att_details_cable_info AS cbl 
    LEFT JOIN att_details_cable AS cb ON cbl.cable_id = cb.system_id
    INNER JOIN att_details_fiber_link AS fl ON cbl.link_system_id = fl.system_id 
    WHERE cb.network_id IN (SELECT DISTINCT tc.cable_network_id FROM temp_cable_result AS tc);
	
    SELECT jsonb_build_object(
        'Cables', COALESCE(jsonb_agg(
            jsonb_build_object(
                'Network_Id', cable_network_id,
                'Cable_Name', cable_name,
                'Fiberlinks', (
                    SELECT COALESCE(jsonb_agg(
                        jsonb_build_object(
                            'network_id', linl_network_id,
                            'link_id', link_id,
                            'link_name', link_name
                        )
                    ), '[]'::jsonb) -- Ensures an empty array if no fiber links exist
                    FROM (
                        SELECT DISTINCT fl.linl_network_id, fl.link_id, fl.link_name 
                        FROM temp_cable_result fl
                        WHERE fl.cable_network_id = cables.cable_network_id and fl.entity_type='FiberLink'
                    ) AS fiberlinks
                )
            )
        ), '[]'::jsonb) -- Ensures empty list if no cables exist
    )
    INTO result_json
    FROM (
        SELECT DISTINCT cable_network_id, cable_name 
        FROM temp_cable_result where entity_type='Cable'
    ) AS cables;

 
    RETURN jsonb_pretty(result_json);

END;
$BODY$;