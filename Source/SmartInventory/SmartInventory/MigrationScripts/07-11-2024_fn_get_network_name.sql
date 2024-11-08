
CREATE OR REPLACE FUNCTION public.fn_get_network_name(p_entity_type character varying, p_entity_system_id integer)
 RETURNS text
 LANGUAGE plpgsql
AS $function$
DECLARE
   v_tablename TEXT;
   v_layername TEXT;
   v_networkname TEXT;
  begin
  
  select layer_table, layer_name  into v_tablename, v_layername from layer_details where upper(layer_name) = upper(p_entity_type);
  EXECUTE  'select '||v_layername||'_NAME from '||v_tablename||' where system_id='||p_entity_system_id||'' into v_networkname;
  return v_networkname;
    end;
  
$function$
;