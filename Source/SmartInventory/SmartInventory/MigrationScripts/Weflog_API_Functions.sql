----------  fn_api_update_alarmstatusdetails --------------

-- FUNCTION: public.fn_api_update_alarmstatusdetails(character varying, character varying, integer, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_api_update_alarmstatusdetails(character varying, character varying, integer, character varying, character varying);

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
        SELECT 'Failed' AS status,CONCAT('Details not found for Entity type:',p_entity_type)::text AS message
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
        SELECT 'Failed' AS status,CONCAT('Details not found for Network ID:',p_network_id)::text AS message
               ;
        
        RETURN;
    END IF;
	
    SELECT system_id INTO v_port_id FROM port_status_master port
    WHERE port.status = p_alarm_status
    LIMIT 1;
    
	IF v_port_id IS NULL THEN
         RETURN QUERY
        SELECT 'Failed' AS status,CONCAT('Details not found for alarm_status:',p_alarm_status)::text AS message
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
        SELECT 'OK' AS status, 'Alarm status updated successfully_.'::text AS message;

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
           'OK' AS status, 
           CONCAT('','Alarm status updated successfully.')::text AS message;
	END IF;

EXCEPTION
     WHEN OTHERS THEN
        
        RETURN QUERY
        SELECT 'Failed' AS status,CONCAT(v_layer_name, ': ', SQLERRM)::text AS message;
               

END
$BODY$;

ALTER FUNCTION public.fn_api_update_alarmstatusdetails(character varying, character varying, integer, character varying, character varying)
    OWNER TO postgres;


---------------------------------fn_api_update_discoveredentity-----------------------





CREATE OR REPLACE FUNCTION public.fn_api_update_discoveredentity(
	p_entity_type character varying,
	p_network_id character varying,
	p_serial_no character,
	p_ip_address character varying,
	p_ports text)
    RETURNS TABLE(status text, message text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    v_layer_table_name text;
    v_layer_name character varying;
    v_sql_query text;
	 v_sql_query1 text;
    v_source_system_id integer;
    v_ports_count integer;
    v_db_ports_count integer;
    v_port_record jsonb;
BEGIN
    
    CREATE TEMP TABLE temp_p_ports (
        port_index  serial,
        port_name character varying
    ) ON COMMIT DROP;
	 
	
	SELECT layer_table, layer_name INTO v_layer_table_name, v_layer_name
    FROM layer_details
    WHERE layer_title = p_entity_type; -- search by title and use layer name
	
	IF v_layer_table_name IS NULL OR v_layer_name IS NULL OR (v_layer_name <> 'ONT' AND v_layer_name <> 'OLT') THEN
    RETURN QUERY
    SELECT 'Failed' AS status, CONCAT('Details not found for Entity type:', p_entity_type)::text AS message;
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
        SELECT 'Failed' AS status,CONCAT('Details not found for Network ID:',p_network_id)::text 
		AS message;        
        RETURN;
    END IF;
	
	 v_sql_query1 := 'UPDATE ' || quote_ident(v_layer_table_name) || ' lt ' ||
                    'SET serial_no = ' || quote_literal(p_serial_no) || ', ' ||
                    'ip_address = ' || quote_literal(p_ip_address) || ' ' ||
                    'WHERE lt.network_id = ' || quote_literal(p_network_id) || ' ' ||
                    'AND lt.system_id = ' || v_source_system_id;
    EXECUTE v_sql_query1;
	
	v_ports_count := jsonb_array_length(p_ports::jsonb);
	
	RAISE NOTICE ' % rows in JSON',
        (v_ports_count);
	
	SELECT COUNT(*) INTO v_db_ports_count
    FROM isp_port_info
    WHERE parent_network_id = p_network_id
    AND parent_entity_type = v_layer_name
	AND input_output = 'O';
	
	IF v_ports_count <> v_db_ports_count THEN
        RETURN QUERY
        SELECT 'Failed' AS status, CONCAT('Number of ports mismatch: expected ', v_ports_count, ', found ', v_db_ports_count)::text AS message;
        RETURN;
    END IF;

    
    FOR v_port_record IN SELECT * FROM jsonb_array_elements(p_ports::jsonb) LOOP
        INSERT INTO temp_p_ports (port_name)
        VALUES (
            v_port_record->>'port_name'
        );
    END LOOP;
	
	 RAISE NOTICE ' % rows in temp_isp_ports',
        (SELECT COUNT(*) FROM temp_p_ports);
	
	
	UPDATE isp_port_info
	SET port_name=p.port_name
	FROM temp_p_ports p
	WHERE isp_port_info.port_number=p.port_index
	AND parent_network_id = p_network_id
	AND parent_entity_type = v_layer_name
	AND input_output = 'O';
	
	
	
	

   
    RETURN QUERY
	--select * from temp_p_ports;
    SELECT 'OK' AS status, 'Discovered entity updated in GIS successfully.'::text AS message;

EXCEPTION
    WHEN OTHERS THEN
        
        RAISE NOTICE 'Error: %', SQLERRM;
        RETURN QUERY 
        SELECT NULL::integer AS isp_port_index, 
               NULL::character varying AS isp_port_name, 
               NULL::integer AS system_id;
END
$BODY$;

ALTER FUNCTION public.fn_api_update_discoveredentity(character varying, character varying, character, character varying, text)
    OWNER TO postgres;



-------------------------------- fn_api_get_entitylocation--------------

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
    
	IF Upper(p_entity_type) IN('OLT') THEN
        SELECT ld.layer_name, ld.layer_table INTO v_layer_name,v_layer_table_name
        FROM isp_model_master a
        INNER JOIN isp_model_type_master b ON a.id = b.model_id AND b.key = p_entity_type
        INNER JOIN layer_details ld ON Upper(ld.layer_name) = Upper(a.key);
		RAISE INFO 'OLT case: layer_table_name: %, layer_name: %', v_layer_table_name, v_layer_name;
    ELSE
        SELECT layer_table, layer_name INTO v_layer_table_name, v_layer_name
        FROM layer_details
        WHERE layer_title = p_entity_type; -- search by title and use layer name
    END IF;
	
    IF v_layer_table_name IS NULL OR v_layer_name IS NULL OR v_layer_name NOT IN ('ONT', 'Equipment')
	THEN
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