-- FUNCTION: public.fn_update_selected_vendor_details(integer, integer, double precision)

-- DROP FUNCTION IF EXISTS public.fn_update_selected_vendor_details(integer, integer, double precision);

CREATE OR REPLACE FUNCTION public.fn_update_selected_vendor_details(
	reference_id integer,
	user_id integer,
	_vendorcost double precision)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


Declare sql TEXT;

BEGIN
	
	If(reference_id >0 and  user_id >0 and _vendorcost >=0 )THEN
	update att_details_pod set contracktorId=user_id,vendorCost=_vendorcost where system_id=reference_id;
	
		
	end if;
	sql:='select user_id,user_name from user_master where user_id='||user_id||' and user_type=''Vendor''';
	
	RAISE INFO '%', sql;
	RETURN QUERY
	EXECUTE 'select row_to_json(row) from ('||sql||') row';
	
END ;

$BODY$;

ALTER FUNCTION public.fn_update_selected_vendor_details(integer, integer, double precision)
    OWNER TO postgres;
