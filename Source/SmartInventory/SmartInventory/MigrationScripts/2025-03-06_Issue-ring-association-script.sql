CREATE OR REPLACE FUNCTION public.fn_get_ring_assocication_details(
    filter_data_flag BOOLEAN, 
    p_region_code CHARACTER VARYING, 
    p_segement_code CHARACTER VARYING, 
    p_ring_id CHARACTER VARYING, 
    p_user_id INTEGER,
    cable_id CHARACTER VARYING
)
RETURNS SETOF JSON
LANGUAGE plpgsql
AS $function$
DECLARE
    sql_query TEXT;
    p_network_id CHARACTER VARYING;
BEGIN
    -- Fetch network_id safely
    SELECT network_id INTO p_network_id FROM att_details_cable WHERE system_id = cable_id::int;
    
    -- Base query with parameterized placeholders
    sql_query := 'SELECT row_to_json(row) FROM ( 
                  SELECT 
                      tr.ring_code, 
                      tre.region_name AS region_code, 
                      ts.segment_code, 
                      tr.ring_capacity AS ring_capacity  
                  FROM top_ring_cable_mapping trcm
                  INNER JOIN top_segment_cable_mapping tscm ON tscm.cable_id = trcm.cable_id 
                  INNER JOIN top_segment ts ON ts.id = tscm.segment_id
                  INNER JOIN top_ring tr ON tr.id = trcm.ring_id 
                  INNER JOIN top_region tre ON tre.id = ts.region_id
                  WHERE trcm.cable_id = $1';

    -- Apply filtering if flag is TRUE
    IF filter_data_flag THEN
        IF p_region_code IS NOT NULL AND p_region_code <> '' THEN
            sql_query := sql_query || ' AND tre.region_name = $2';
        END IF;

        IF p_segement_code IS NOT NULL AND p_segement_code <> '' THEN
            sql_query := sql_query || ' AND ts.segment_code = $3';
        END IF;

        IF p_ring_id IS NOT NULL AND p_ring_id <> '' THEN
            sql_query := sql_query || ' AND tr.ring_code = $4';
        END IF;
    END IF;

    -- Close the query properly
    sql_query := sql_query || ') row';

    -- Debugging output
    RAISE INFO 'Executing SQL: %', sql_query;

    -- Execute dynamic SQL safely
    RETURN QUERY EXECUTE sql_query USING p_network_id, p_region_code, p_segement_code, p_ring_id;

END;
$function$;

--------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_remove_ring_assocication_details(p_ring_id integer, p_user_id integer,cable_id character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
 
DECLARE
p_network_id character varying;
BEGIN
	
select network_id into p_network_id from att_details_cable where system_id = cable_id;

delete from top_ring_cable_mapping trc where ring_id = p_ring_id and cable_id = p_network_id;

return query
select true as v_status, 'Ring Association has been deleted Successfully!.' as v_message;
END
$function$
;
