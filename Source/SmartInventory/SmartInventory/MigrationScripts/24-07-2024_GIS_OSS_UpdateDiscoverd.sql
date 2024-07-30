-------Query Adding Column--------


ALTER TABLE att_details_ont
ADD COLUMN ip_address character varying(50);

ALTER TABLE att_details_model
ADD COLUMN serial_no character varying(100),
ADD COLUMN ip_address character varying(50);





---------- Funtions-------




CREATE OR REPLACE FUNCTION public.fn_api_Update_DiscoveredEntity(
    p_entity_type character varying,
    p_network_id character varying,
    p_serial_no character,
    p_ip_address character varying,
    p_ports Text)
    RETURNS TABLE(status text, message text) 
	--RETURNS TABLE(port_index integer, port_name character varying) 
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
    SELECT 'false' AS status, CONCAT('Details not found for Entity type:', p_entity_type)::text AS message;
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
        SELECT 'false' AS status,CONCAT('Details not found for Network ID:',p_network_id)::text 
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
        SELECT 'false' AS status, CONCAT('Number of ports mismatch: expected ', v_ports_count, ', found ', v_db_ports_count)::text AS message;
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
    SELECT 'true' AS status, 'Discovered entity updated in GIS successfully.'::text AS message;

EXCEPTION
    WHEN OTHERS THEN
        
        RAISE NOTICE 'Error: %', SQLERRM;
        RETURN QUERY 
        SELECT NULL::integer AS isp_port_index, 
               NULL::character varying AS isp_port_name, 
               NULL::integer AS system_id;
END
$BODY$;
