-- FUNCTION: public.fn_get_corelogicsearchdetails(character varying,character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_odfdetails(character varying,character varying);
CREATE OR REPLACE FUNCTION public.fn_get_corelogicsearchdetails(
	p_searchtext character varying,
    p_searchType character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
sql TEXT;

BEGIN

sql:='';
if(p_searchType='ODF') then
sql:= 'select distinct network_id from att_details_fms where network_id ilike ''%' || p_searchText || '%'' order by network_id';
else
sql:= 'select network_id from att_details_fiber_link  where network_id ilike ''%' || p_searchtext || '%'' order by network_id';
end if;
RAISE INFO '%', sql;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$;

ALTER FUNCTION public.fn_get_corelogicsearchdetails(character varying,character varying)
    OWNER TO postgres;
	-------------------------------------------------------------------------------------------------------------------------------------
	INSERT INTO public.module_master(
	 module_name, module_description,  created_by, created_on,  type, is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled )
	VALUES ('Core Plan Logic', 'Core Plan Logic', 1, now(),  'Web', true, 'CPL', 0, 19, false);
	-------------------------------------------------------------------------------------------------------------------------------------------------
