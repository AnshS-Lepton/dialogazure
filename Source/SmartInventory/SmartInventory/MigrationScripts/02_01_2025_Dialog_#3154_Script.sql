/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: This function generated new fiber link Id
ModifiedOn: 02 Jan 2025
ModifiedBy: Chandra Shekhar Sahni
Purpose: This function was mis behaving while generating link Id same is corrected by using <!-- MAX(CAST(NULLIF(SUBSTRING( -->
------------------------------------------*/
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
    -- Construct the SQL query dynamically
    sql := '
    WITH max_link AS (
        SELECT 
			MAX(CAST(NULLIF(SUBSTRING(link_ID FROM ' || (LENGTH(p_link_prefix) + 1) || '), '''') AS INTEGER)) AS max_id
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
$BODY$;

ALTER FUNCTION public.fn_get_fiber_link_prefix(character varying)
    OWNER TO postgres;

-----------------------------------------------------------------------
/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: This function retrieves the networkId and fiberlinkId according to search crieteria
ModifiedOn: 02 Jan 2025
ModifiedBy: Chandra Shekhar Sahni
Purpose: We have extended the search functionality by adding linkId along with networkId also limited the results to latest top 10 records
------------------------------------------*/
-- DROP FUNCTION public.fn_get_corelogicsearchdetails(varchar, varchar);

CREATE OR REPLACE FUNCTION public.fn_get_corelogicsearchdetails(p_searchtext character varying, p_searchtype character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE
sql TEXT;

BEGIN

sql:='';
if(p_searchType='ODF') then
sql:= 'select distinct network_id from att_details_fms where network_id ilike ''%' || p_searchText || '%'' order by network_id desc limit 10';
else
sql:= 'select network_id,link_id from att_details_fiber_link  where network_id ilike ''%' || p_searchtext || '%'' OR link_id ilike ''%' || p_searchtext || '%'' order by network_id desc limit 10';
end if;
RAISE INFO '%', sql;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$function$
;
