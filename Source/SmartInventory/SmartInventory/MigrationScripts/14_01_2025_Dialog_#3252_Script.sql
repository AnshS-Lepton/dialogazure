/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: This function generates new fiber link Id
ModifiedOn: 14 Jan 2025
ModifiedBy: Chandra Shekhar Sahni
Purpose: This function was mis behaving while generating link Id same is corrected by using <!-- REGEXP_REPLACE() --> function 
------------------------------------------*/
-- DROP FUNCTION public.fn_get_fiber_link_prefix(varchar);

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
			MAX(CAST(NULLIF(REGEXP_REPLACE(SUBSTRING(link_ID FROM ' || (LENGTH(p_link_prefix) + 1) || '), ''[^0-9]'', '''', ''g''), '''') AS INTEGER)) AS max_id
        FROM 
            att_details_fiber_link
        WHERE 
            link_ID ILIKE ' || quote_literal(p_link_prefix || '%') || '
    )
    SELECT 
        ' || quote_literal(p_link_prefix) || ' || LPAD((COALESCE(max_id, 0) + 1)::TEXT, 8, ''0'') AS link_prefix
    FROM 
        max_link';

    -- Debugging Information
    RAISE INFO '%', sql;

    -- Execute the query and return the results
    RETURN QUERY EXECUTE 'SELECT row_to_json(row) FROM (' || sql || ') row';

END;
$function$
;
