CREATE OR REPLACE FUNCTION public.fn_get_rootid_list(
	)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
v_Query text;
BEGIN
v_Query:='';
RETURN QUERY EXECUTE 'SELECT row_to_json(row) FROM (SELECT LM.system_id AS value, CONCAT(LM.route_name,''-'', ''('', NETWORK_ID, '')'') AS key FROM att_details_cable LM WHERE LM.route_id IS NOT NULL) row';
 
END
$BODY$;

ALTER FUNCTION public.fn_get_rootid_list()
    OWNER TO postgres;