-- FUNCTION: public.fn_get_fiber_link_prefix(character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_fiber_link_prefix(character varying);
CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_prefix(
	p_link_prefix character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    sql TEXT;
BEGIN
IF  EXISTS (SELECT 1 FROM att_details_fiber_link WHERE link_ID  ILIKE '' || quote_literal(p_link_prefix || '%') || '') 
THEN
       sql := ' SELECT '''||p_link_prefix||''' || ''00000001'' AS link_prefix';
    ELSE
        sql := '
        WITH max_link AS (
            SELECT 
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
	
	END IF;
    RAISE INFO '%', sql;
    RETURN QUERY EXECUTE 'SELECT row_to_json(row) FROM (' || sql || ') row';

END;
$BODY$;

ALTER FUNCTION public.fn_get_fiber_link_prefix(character varying)
    OWNER TO postgres;
