/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: public.fn_geojson_update_metadata(varchar, int4, int4)
Purpose: applied upper function for layer_name to get the correct value in v_layer_delta_table
ModifiedOn: 06 Jan 2025
ModifiedBy: Chandra Shekhar Sahni
------------------------------------------*/
-- DROP FUNCTION public.fn_geojson_update_metadata(varchar, int4, int4);

CREATE OR REPLACE FUNCTION public.fn_geojson_update_metadata(p_entity_type character varying, p_province_id integer, p_delta_type integer)
 RETURNS TABLE(status boolean, message text)
 LANGUAGE plpgsql
AS $function$
DECLARE
   v_layer_delta_table character varying;   
   v_sql character varying;
   v_arow record;
begin
	if(coalesce(p_province_id,0) != 0) then
		select delta_metadata_table into v_layer_delta_table from layer_details where upper(layer_name)=upper(p_entity_type);

		execute 'delete from '||v_layer_delta_table||' where province_id='||p_province_id||'';
		execute 'insert into '||v_layer_delta_table||'(province_id,last_delta_type,last_delta_at) values('||p_province_id||','||p_delta_type||',now())';
	end if;
	RETURN QUERY Select true as status, 'Data updated successfully!'::text  as message;
END; 

$function$
;