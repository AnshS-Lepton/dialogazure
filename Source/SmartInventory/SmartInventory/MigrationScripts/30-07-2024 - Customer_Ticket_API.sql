insert into ticket_status_master(name,created_by,module,is_active,opacity)
		values('Unassigned',5,'Network_Ticket',true,70)

-- FUNCTION: public.fn_get_customer_ticket_status(integer)

-- DROP FUNCTION IF EXISTS public.fn_get_customer_ticket_status(integer);

CREATE OR REPLACE FUNCTION public.fn_get_customer_ticket_status(
	p_ticket_id integer)
    RETURNS TABLE(ticket_id text, ticket_status character varying, can_id text, created_on text, assigned_to text, assigned_date text, target_date text, remarks character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 
        tm.ticket_id ::text AS ticket_id,
        ttm.name as ticket_status,
        tm.can_id ::text AS can_id,
		to_char(tm.created_on, 'DD-Mon-YY'::text) AS created_on,
          um.user_name::text AS assigned_to,
		to_char(tm.assigned_date, 'DD-Mon-YY'::text) AS assigned_date,
		to_char(tm.target_date, 'DD-Mon-YY'::text) AS target_date,     
        tm.remarks
    FROM 
        ticket_master tm
		left join user_master as  um on tm.assigned_to=um.user_id 
		left join ticket_status_master ttm ON tm.ticket_status::integer=ttm.id 
    WHERE 
        tm.ticket_id = p_ticket_id;
END;
$BODY$;

ALTER FUNCTION public.fn_get_customer_ticket_status(integer)
    OWNER TO postgres;
