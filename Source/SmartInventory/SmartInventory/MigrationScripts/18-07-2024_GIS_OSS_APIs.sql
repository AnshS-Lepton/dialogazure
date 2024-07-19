-- FUNCTION: public.fn_api_get_entitylocation(character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_api_get_entitylocation(character varying, character varying);

-- select fn_api_get_entitylocation('FDP','WAI-CBT004-FMS01')

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
	v_layer_name character varying;
BEGIN
    
   SELECT layer_table, layer_name INTO v_layer_table_name, v_layer_name
    FROM layer_details
    WHERE layer_title = p_entity_type; -- search by title and use layer name

   
    v_sql_query := 'SELECT row_to_json(row) 
                  FROM (
                      SELECT 
                          lt.network_id AS entity_id, 
                          pm.entity_type AS entity_type, 
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



-- FUNCTION: public.fn_api_get_intermediateentities(character varying, character varying, character varying, character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_api_get_intermediateentities(character varying, character varying, character varying, character varying, integer);
-- select fn_api_get_intermediateentities('FDC','WAI-CBT004-FMS01','ONT','GGN-ONT03',1)
CREATE OR REPLACE FUNCTION public.fn_api_get_intermediateentities(
	p_source_entity_type character varying,
	p_source_id character varying,
	p_destination_entity_type character varying,
	p_destination_id character varying,
	p_port integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    v_source_system_id integer;
    
    
	v_total_distance_meters double precision;
	v_sql_query text;
	v_layer_table_name text;
	v_layer_name character varying ;
	
BEGIN
    create temp table cpf_temp_result
	(
	id serial,	
	rowid integer,
	connection_id integer,
	parent_connection_id integer,
	source_system_id integer,
	source_network_id character varying(100),
	source_entity_type character varying(100),
	destination_network_id character varying(100),
	destination_entity_type	character varying(100),
	source_port_no integer,
	cable_calculated_length double precision
	);
	 SELECT layer_table, layer_name INTO v_layer_table_name, v_layer_name
    FROM layer_details
    WHERE layer_title = p_source_entity_type; -- search by title and use layer name
	
	IF v_layer_table_name IS NULL OR v_layer_name IS NULL THEN
        RETURN QUERY SELECT json_build_object(
            'error', 'Layer details not found for source entity type ' || p_source_entity_type
        )::json;
        DROP TABLE IF EXISTS cpf_temp_result;
        RETURN;
    END IF;
	
	 v_sql_query := 'SELECT (row_to_json(t) ->> ''system_id'')::integer
                  FROM (
                      SELECT lt.system_id
                      FROM ' || quote_ident(v_layer_table_name) || ' lt
                      JOIN point_master pm ON lt.network_id = pm.common_name
                      WHERE lt.network_id = ''' || p_source_id || '''
                      LIMIT 1
                  ) t';
	
		
   EXECUTE v_sql_query INTO v_source_system_id;
	
	insert into cpf_temp_result(rowid,connection_id,parent_connection_id,source_system_id,source_network_id,
	source_entity_type,destination_network_id,destination_entity_type, source_port_no,
	cable_calculated_length)
	
	select  a.rowid,a.connection_id,a.parent_connection_id,a.source_system_id,a.source_network_id
	,a.source_entity_type,a.destination_network_id,a.destination_entity_type
	, a.source_port_no, a.cable_calculated_length
	from fn_get_connection_info('', '', 0, 0, '', '', v_source_system_id, p_port, v_layer_name)a
	WHERE (a.source_entity_type || a.source_system_id) <> (a.destination_entity_type || a.destination_system_id)
	AND UPPER(a.source_network_id) <> UPPER(a.destination_network_id)
	AND a.source_entity_type <> 'Cable'
	AND a.destination_entity_type<> 'Cable'
	;
	
	
	SELECT COALESCE(SUM(cable_calculated_length), 0)
    INTO v_total_distance_meters
    FROM cpf_temp_result;
	
	RETURN QUERY 
    SELECT json_build_object(
        'intermediate_entities', (
            SELECT json_agg(json_build_object(
                'entity_type', x.source_entity_type,
                'entity_id', x.source_network_id
				
				
            ))
            FROM cpf_temp_result x
        ),
        'source_entity', json_build_object(
            'entity_type', v_layer_name,
            'entity_id', p_source_id
        ),
		'destination_entity', json_build_object(
            'entity_type', p_destination_entity_type,
            'entity_id', p_destination_id
        ),
		'distance_meters', v_total_distance_meters
    );
    
	DROP TABLE IF EXISTS cpf_temp_result;
    
END;
$BODY$;

ALTER FUNCTION public.fn_api_get_intermediateentities(character varying, character varying, character varying, character varying, integer)
    OWNER TO postgres;




-- FUNCTION: public.fn_api_update_alarmstatusdetails(character varying, character varying, integer, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_api_update_alarmstatusdetails(character varying, character varying, integer, character varying, character varying);
-- select fn_api_update_alarmstatusdetails('WAI-POD047-FMS01','FDP',2,'Faulty','DOWN')
CREATE OR REPLACE FUNCTION public.fn_api_update_alarmstatusdetails(
	p_network_id character varying,
	p_entity_type character varying,
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
	
	 v_sql_query := 'SELECT (row_to_json(t) ->> ''system_id'')::integer
                  FROM (
                      SELECT lt.system_id
                      FROM ' || quote_ident(v_layer_table_name) || ' lt
                      JOIN point_master pm ON lt.network_id = pm.common_name
                      WHERE lt.network_id = ''' || p_network_id || '''
                      LIMIT 1
                  ) t';
	
		
   EXECUTE v_sql_query INTO v_source_system_id;
    
   
    SELECT system_id INTO v_port_id FROM port_status_master port
    WHERE port.status = p_alarm_status
    LIMIT 1;
    
   
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

EXCEPTION
     WHEN OTHERS THEN
        -- Return error message in case of an exception
        RETURN QUERY
        SELECT 'false' AS status,CONCAT(v_layer_name, ': ', SQLERRM)::text AS message
               ;
               

END
$BODY$;

ALTER FUNCTION public.fn_api_update_alarmstatusdetails(character varying, character varying, integer, character varying, character varying)
    OWNER TO postgres;
