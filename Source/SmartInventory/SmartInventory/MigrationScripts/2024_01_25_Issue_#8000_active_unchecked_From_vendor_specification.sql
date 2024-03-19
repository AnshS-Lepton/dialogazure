CREATE OR REPLACE FUNCTION public.fn_get_item_specification(
	entitytype character varying,
	typeid integer,
	brandid integer,
	specification character varying)
    RETURNS TABLE(key character varying, value character varying, ddtype character varying, type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    sql TEXT;
v_layer_id integer;
BEGIN
select layer_id into v_layer_id from layer_details where upper(layer_name)=upper(entitytype);
  
   sql:= 'select  specification as key,specification as value, ''Specification''::character varying as dd_type,specify_type as type
from item_template_master where layer_id='||v_layer_id||' and is_active=true 
union
select  prop_value as key,id::character varying as value, ''Accessibility''::character varying as dd_type,''''::character varying as type
from system_spec_master where lower(prop_name)=lower(''accessibility'')
union
select  prop_value as key,id::character varying as value, ''Construction''::character varying as dd_type,''''::character varying as type
from system_spec_master where lower(prop_name)=lower(''construction'')
union
select  prop_value as key,id::character varying as value, ''Activation''::character varying as dd_type,''''::character varying as type 
from system_spec_master where lower(prop_name)=lower(''activation'')
union 
select dropdown_key as key,dropdown_value::character varying  as value, ''TypeMaster''::character varying as dd_type ,''''::character varying as type
from dropdown_master where dropdown_type=''Splitter_Type''';

raise info 'V_SQLQUERY: %', sql;

if typeid !=0 then
  sql:=sql ||' union select brand as key, id::character varying  as value,''Brand''::character varying as dd_type,''''::character varying as type from isp_brand_master where type_id='||typeid||' ';
end if;
if brandid !=0 then
  sql:=sql ||' union select model as key, id::character varying  as value,''Model''::character varying as dd_type,''''::character varying as type from isp_base_model where brand_id='||brandid||' ';
end if;
if specification != '' then
  sql:=sql ||' union select distinct i.vendor_id::character varying as key , v.name::character varying as value,''Vendor''::character varying  as ddtype,''''::character varying as type
   from item_template_master i inner join vendor_master v   on i.vendor_id= v.id  where lower(i.specification)=lower('''||specification||''' ) and i.is_active=true and i.is_master=true';
end if;

sql:= sql ||' order by dd_type,key';

 RETURN QUERY  
 EXECUTE sql;
END;
$BODY$;

ALTER FUNCTION public.fn_get_item_specification(character varying, integer, integer, character varying)
    OWNER TO postgres;
