CREATE OR REPLACE FUNCTION public.fn_getroutebufferfeasibility(
    p_coordinates character varying,
    mtrbuffer integer
)
RETURNS TABLE(
    cable_network_id character varying,
    cable_name character varying,
    geometry text
)
LANGUAGE plpgsql
COST 100
VOLATILE PARALLEL UNSAFE
ROWS 2
AS $BODY$
DECLARE
    v_geometry geometry;
    v_geometry_with_buffer geometry;
    v_clean_coords text;
BEGIN
    -- Check if input contains 'LINESTRING('
    IF POSITION('LINESTRING(' IN UPPER(p_coordinates)) > 0 THEN
        -- Extract just the coordinates from the WKT
        v_clean_coords := REGEXP_REPLACE(p_coordinates, 'LINESTRING\((.*)\)', '\1', 'i');
    ELSE
        v_clean_coords := p_coordinates;
    END IF;

    -- Build proper WKT LINESTRING and create geometry
    v_geometry := ST_GeomFromText('LINESTRING(' || v_clean_coords || ')', 4326);
    v_geometry_with_buffer := ST_Buffer_Meters(v_geometry, mtrbuffer);

    RAISE INFO 'Buffer Distance (m): %', mtrbuffer;
    RAISE INFO 'Cleaned Coordinates: %', v_clean_coords;
    RAISE INFO 'Buffered Geometry: %', ST_AsText(v_geometry_with_buffer);

    -- Return matching cable lines
    RETURN QUERY
    SELECT
        L.COMMON_NAME AS cable_network_id,
        L.display_name AS cable_name,
        ST_AsText(L.SP_GEOMETRY)::text AS geometry
    FROM
        LINE_MASTER L
    LEFT JOIN
        att_details_cable CBL ON L.SYSTEM_ID = CBL.SYSTEM_ID
    WHERE
        UPPER(L.ENTITY_TYPE) = 'CABLE'
        AND ST_Intersects(ST_MakeValid(L.SP_GEOMETRY), v_geometry_with_buffer);
END;
$BODY$;

ALTER FUNCTION public.fn_getroutebufferfeasibility(character varying, integer)
    OWNER TO postgres;
