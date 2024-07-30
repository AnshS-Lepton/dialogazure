                --------fn_api_update_alarmstatusdetails---------



CREATE OR REPLACE FUNCTION public.fn_api_update_alarmstatusdetails(
	p_entity_type character varying,
	p_network_id character varying,
	p_port_number integer,
	p_alarm_status character varying,
	p_comments character varying)
    RETURNS TABLE(status text, message text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE 
    v_source_system_id integer;
    v_port_id integer;
	v_sql_query text;
	v_layer_table_name text;
	v_layer_name character varying;

BEGIN
    
    SELECT layer_table, layer_name INTO v_layer_table_name, v_layer_name
    FROM layer_details
    WHERE layer_title = p_entity_type; -- search by title and use layer name
	
	IF v_layer_table_name IS NULL OR v_layer_name IS NULL THEN
         RETURN QUERY
        SELECT 'false' AS status,CONCAT('Details not found for Entity type:',p_entity_type)::text AS message
               ;
        
        RETURN;
    END IF;
	
	 v_sql_query := 'SELECT (row_to_json(t) ->> ''system_id'')::integer
                  FROM (
                      SELECT lt.system_id
                      FROM ' || quote_ident(v_layer_table_name) || ' lt
                      JOIN point_master pm ON lt.network_id = pm.common_name
                      WHERE lt.network_id = ''' || p_network_id || '''
                      LIMIT 1
                  ) t';
	
		
   EXECUTE v_sql_query INTO v_source_system_id;
    
	IF v_source_system_id IS NULL THEN
         RETURN QUERY
        SELECT 'false' AS status,CONCAT('Details not found for Network ID:',p_network_id)::text AS message
               ;
        
        RETURN;
    END IF;
	
    SELECT system_id INTO v_port_id FROM port_status_master port
    WHERE port.status = p_alarm_status
    LIMIT 1;
    
	IF v_port_id IS NULL THEN
         RETURN QUERY
        SELECT 'false' AS status,CONCAT('Details not found for alarm_status:',p_alarm_status)::text AS message
               ;
        RETURN;
    END IF;
    IF v_layer_name = upper('Cable') THEN
	update att_details_cable_info
	set core_comment=p_comments,
	a_end_status_id=v_port_id,
	b_end_status_id=v_port_id
	where cable_id=v_source_system_id
	and fiber_number=p_port_number;
	RETURN QUERY
        SELECT 'true' AS status, 'Alarm status updated successfully_.'::text AS message;

	ELSE
    	UPDATE isp_port_info
    	SET 
        port_status_id = v_port_id,
        port_status = p_alarm_status,
        port_comment = p_comments
   		WHERE 
        parent_system_id = v_source_system_id
        AND Upper(parent_network_id) = Upper(p_network_id)
        AND Upper(parent_entity_type) = Upper(v_layer_name)
        AND port_number = p_port_number
        AND input_output = 'O';
    
   
    RETURN QUERY
    SELECT 
           'true' AS status, 
           CONCAT('','Alarm status updated successfully.')::text AS message;
	END IF;

EXCEPTION
     WHEN OTHERS THEN
        
        RETURN QUERY
        SELECT 'false' AS status,CONCAT(v_layer_name, ': ', SQLERRM)::text AS message;
               

END
$BODY$;

ALTER FUNCTION public.fn_api_update_alarmstatusdetails(character varying, character varying, integer, character varying, character varying)
    OWNER TO postgres;



             ------------------fn_api_get_ossserviceability---------------

     
CREATE OR REPLACE FUNCTION public.fn_api_get_ossserviceability(
	p_lng double precision,
	p_lat double precision)
    RETURNS TABLE(geom_type character varying, entity_type character varying, 
				  entity_title character varying, system_id integer,
				  common_name character varying, geom text, centroid_geom text, 
				  network_status character varying, display_name character varying, 
				  total_core character varying, distance numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    v_buffer double precision;
 	BEGIN
    CREATE TEMP TABLE temp_result (
        geom_type character varying,
        entity_type character varying,
        entity_title character varying,
        system_id integer,
        common_name character varying,
        geom text,
        centroid_geom text,
        network_status character varying,
        display_name character varying,
        total_core character varying,
        distance numeric(10,2)
    ) ON COMMIT DROP;

    SELECT value INTO v_buffer FROM global_settings WHERE key='serviceability_buffer';
BEGIN
    INSERT INTO temp_result (
        geom_type, entity_type, entity_title, system_id,
        geom, centroid_geom, network_status,
        display_name, total_core, distance
    )
    SELECT 
        'Point', p.entity_type, p.entity_type, p.system_id,
        ST_AsText(sp_geometry), sp_geometry, p.network_status,
        p.common_name, p.no_of_ports,
        
            ST_Distance(
                ST_GeomFromText('POINT(' || p.longitude || ' ' || p.latitude || ')', 4326),
                ST_GeomFromText('POINT(' || p_lng || ' ' || p_lat || ')', 4326)
            ) 
        	
    FROM point_master p
    WHERE ST_Within(
        p.sp_geometry, 
        ST_Buffer_Meters(
            ST_GeomFromText('POINT(' || p_lng || ' ' || p_lat || ')', 4326), 
            v_buffer
        )
    )AND p.entity_type IN ('FDB');
	
	EXCEPTION WHEN OTHERS THEN
        RAISE NOTICE 'Error executing main function: %', SQLERRM;
    END;

   RETURN QUERY
   SELECT 
        t.geom_type, t.entity_type, ld.layer_title, t.system_id, t.common_name,
        t.geom, t.centroid_geom, t.network_status, t.display_name, t.total_core,
        t.distance
    FROM temp_result t
    JOIN layer_details ld
    ON ld.layer_name = t.entity_type
    ORDER BY t.distance;
	
	EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Error executing main function: %', SQLERRM;
END;


$BODY$;

ALTER FUNCTION public.fn_api_get_ossserviceability(double precision, double precision)
    OWNER TO postgres;

            -------------------------fn_api_get_entitylocation-------------

-- FUNCTION: public.fn_api_get_entitylocation(character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_api_get_entitylocation(character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_api_get_entitylocation(
	p_entity_type character varying,
	p_entity_network_id character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    v_layer_table_name text;
    v_sql_query text;
	v_sql_query1 text;
	v_layer_name character varying;
	v_source_system_id integer;
BEGIN
    
	
	
   SELECT layer_table, layer_name INTO v_layer_table_name, v_layer_name
    FROM layer_details
    WHERE layer_title = p_entity_type; -- search by title and use layer name

    IF v_layer_table_name IS NULL OR v_layer_name IS NULL OR v_layer_name NOT IN ('ONT', 'OLT')THEN
        RETURN QUERY SELECT json_build_object(
            'error', 'Invalid Entity type:' || p_entity_type
        )::json;

        RETURN;
    END IF;
	
	v_sql_query1 := 'SELECT (row_to_json(t) ->> ''system_id'')::integer
                  FROM (
                      SELECT lt.system_id
                      FROM ' || quote_ident(v_layer_table_name) || ' lt
                      JOIN point_master pm ON lt.network_id = pm.common_name
                      WHERE lt.network_id = ''' || p_entity_network_id || '''
                      LIMIT 1
                  ) t';
		
   EXECUTE v_sql_query1 INTO v_source_system_id;
   
   IF v_source_system_id IS NULL THEN
        RETURN QUERY SELECT json_build_object(
            'error', 'Details not found for Network ID ' || p_entity_network_id
        )::json;
        
        RETURN;
    END IF;
   
    v_sql_query := 'SELECT row_to_json(row) 
                  FROM (
                      SELECT 
                          lt.network_id AS entity_id, 
                          ''' || p_entity_type || ''' AS entity_type, 
                          pb.province_name  AS province, 
                          rb.region_name AS region, 
                          json_build_object(
                              ''latitude'', pm.latitude, 
                              ''longitude'', pm.longitude
                          ) AS location
                      FROM ' || quote_ident(v_layer_table_name) || ' lt
                      JOIN point_master pm ON lt.network_id = pm.common_name
                       JOIN province_boundary pb ON lt.province_id = pb.id
                       JOIN region_boundary rb ON lt.region_id = rb.id
                      WHERE lt.network_id = ''' || p_entity_network_id || '''
                  ) row';

    
    RETURN QUERY EXECUTE v_sql_query;
END;
$BODY$;

ALTER FUNCTION public.fn_api_get_entitylocation(character varying, character varying)
    OWNER TO postgres;
