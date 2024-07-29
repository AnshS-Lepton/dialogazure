ALTER TABLE province_boundary
ADD COLUMN building_code VARCHAR(55);

-- FUNCTION: public.fn_save_customer_ticket(integer, character varying, character varying, double precision, double precision, integer, character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_save_customer_ticket(integer, character varying, character varying, double precision, double precision, integer, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_save_customer_ticket(
	p_can_id integer,
	p_name character varying,
	p_address character varying,
	p_latitude double precision,
	p_longitude double precision,
	p_connection_entity_id integer,
	p_ticket_source character varying,
	p_reference_id character varying,
	p_connection_entity_type character varying)
    RETURNS TABLE(reference_id character varying, ticketid character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 DECLARE
   createdby  INTEGER;
   buildingcode VARCHAR(55);
BEGIN
SELECT  building_code into buildingcode
FROM province_boundary 
WHERE ST_Within(
    ST_GeomFromText('POINT(' || p_longitude || ' ' || p_latitude || ')', 4326),
    sp_geometry
);
 select created_by into createdby from  public.api_consumer_master where source=p_ticket_source;
    RETURN QUERY
    INSERT INTO ticket_master (
        can_id,
        customer_name,
        address,
		latitude,
		longitude,
		ticket_type_id,
        reference_ticket_id,
        reference_type,
		connection_entity_type,
		building_code,
		created_by,
		assigned_date,
		created_on
    )
    VALUES (
        p_can_id,
        p_name,
        p_address,
		p_latitude,
		p_longitude,
		p_connection_entity_id,
		p_reference_id,
        p_ticket_source,
		p_connection_entity_type,
		buildingcode,
		createdby,	
		null,
		CURRENT_TIMESTAMP
    )
    RETURNING reference_ticket_id AS reference_id, ticket_id::Character varying;
END;
$BODY$;

ALTER FUNCTION public.fn_save_customer_ticket(integer, character varying, character varying, double precision, double precision, integer, character varying, character varying, character varying)
    OWNER TO postgres;



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
	   tm.ticket_status,
        tm.can_id ::text AS can_id,
		to_char(tm.created_on, 'DD-Mon-YY'::text) AS created_on,
          um.user_name::text AS assigned_to,
		to_char(tm.assigned_date, 'DD-Mon-YY'::text) AS assigned_date,
		to_char(tm.target_date, 'DD-Mon-YY'::text) AS target_date,     
        tm.remarks
    FROM 
        ticket_master tm
		left join user_master as  um on tm.assigned_by=um.user_id 
		left join ticket_status_master ttm ON tm.ticket_type_id=ttm.id 
    WHERE 
        tm.ticket_id = p_ticket_id;
END;
$BODY$;

ALTER FUNCTION public.fn_get_customer_ticket_status(integer)
    OWNER TO postgres;
