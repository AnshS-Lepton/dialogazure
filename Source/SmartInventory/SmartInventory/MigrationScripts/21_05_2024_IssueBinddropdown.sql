 CREATE OR REPLACE FUNCTION public.fn_get_user_tool_data()
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    SELECT json_build_object(
               'value', ft.id,
               'key', ft.tool_name,
               'tool_description', ft.tool_description
           )
    FROM fe_tools_master AS ft;
END;
$function$
;
-----------------------------------------------------
UPDATE public.user_tools_mapping
SET  date_value='2024-05-20'