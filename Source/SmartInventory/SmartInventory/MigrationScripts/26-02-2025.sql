

----------------------modify function for POINT and LINESTRING-----------------------


 DROP FUNCTION IF EXISTS public.fn_getroutebufferfeasibility(character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_getroutebufferfeasibility(
	p_coordinates character varying,
	mtrbuffer integer)
    RETURNS TABLE(cable_network_id character varying, cable_name character varying, geometry text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 2

AS $BODY$
DECLARE 
    v_role_id integer;
    v_geometry_with_buffer geometry;
    v_geometry geometry;
	
BEGIN
    -- Create Point and Buffer Geometry
    v_geometry = ST_GEOMFROMTEXT(p_coordinates,4326);
    v_geometry_with_buffer = ST_BUFFER_METERS(v_geometry, MTRBUFFER);
	
	

Raise info 'MTRBUFFER ->%',MTRBUFFER;
Raise info 'v_geometry_with_buffer ->%',v_geometry_with_buffer;

   
   
    -- Return Cable (Line) Data
    RETURN QUERY 
    SELECT 
        L.COMMON_NAME AS cable_network_id, 
        L.display_name as cable_name, 
        ST_astext(L.SP_GEOMETRY)::TEXT AS geometry
        
    FROM LINE_MASTER L
    LEFT JOIN att_details_cable CBL 
        ON L.SYSTEM_ID = CBL.SYSTEM_ID
    WHERE 
	UPPER(L.ENTITY_TYPE) = 'CABLE' AND 
    ST_INTERSECTS(ST_MAKEVALID(L.SP_GEOMETRY), v_geometry_with_buffer)
    ;
END
$BODY$;

ALTER FUNCTION public.fn_getroutebufferfeasibility(character varying, integer)
    OWNER TO postgres;


----------------------modify function for POINT and LINESTRING-----------------------


 DROP FUNCTION IF EXISTS public.fn_get_start_endpoints(text);

CREATE OR REPLACE FUNCTION public.fn_get_start_endpoints(
	p_line_geom text)
    RETURNS TABLE(start_point text, end_point text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY 
    SELECT ST_astext(ST_StartPoint(p_line_geom)), COALESCE(ST_AsText(ST_EndPoint(ST_GeomFromText(p_line_geom))), ST_AsText(ST_StartPoint(ST_GeomFromText(p_line_geom))));
END;
$BODY$;

ALTER FUNCTION public.fn_get_start_endpoints(text)
    OWNER TO postgres;

