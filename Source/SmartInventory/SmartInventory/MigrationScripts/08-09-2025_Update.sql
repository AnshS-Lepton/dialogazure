

-------------------------get connected  structure details-------------------------


CREATE OR REPLACE FUNCTION public.fn_get_connected_structure(
	lat double precision,
	lng double precision,
	p_network_id character varying)
    RETURNS TABLE(cable_network_id character varying, cable_geom text, structure_id character varying, structure_type character varying, structure_geom text, longitude double precision, latitude double precision, distance_meters double precision) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1

AS $BODY$
DECLARE 
    v_geometry geometry;
    v_geometry_with_buffer geometry;
    v_network_id varchar;
    v_cable_geom geometry;
BEGIN
    -- Create customer point geometry 
    v_geometry := ST_SetSRID(ST_MakePoint(lng, lat), 4326);

  

    -- Get path point geometry using  network_id
    SELECT cbl.network_id, lm.sp_geometry
    INTO v_network_id, v_cable_geom
    FROM line_master lm
    INNER JOIN att_details_cable cbl 
        ON cbl.system_id = lm.system_id
    WHERE cbl.network_id=p_network_id;

    IF v_network_id IS NULL THEN
        RAISE NOTICE 'No cable found within buffer %m', mtrbuffer;
        RETURN;
    END IF;

    --  Find NEAREST structure (Pole/Manhole) intersecting 2m buffer of cable
    RETURN QUERY
    SELECT 
        v_network_id AS cable_network_id,
        st_astext(v_cable_geom) AS cable_geom,
        pm.COMMON_NAME AS structure_id,
        pm.entity_type AS structure_type,
        st_astext(pm.sp_geometry) AS structure_geom,
        ST_X(pm.sp_geometry) AS longitude,
        ST_Y(pm.sp_geometry) AS latitude,
        ROUND(CAST(ST_Distance(v_cable_geom::geography, pm.sp_geometry::geography) AS numeric), 2)::double precision AS distance_meters
    FROM point_master pm
    WHERE UPPER(pm.ENTITY_TYPE) IN ('MANHOLE', 'POLE') AND ST_Intersects(pm.sp_geometry, ST_Buffer(v_cable_geom::geography, 2)::geometry)
    ORDER BY ST_Distance(v_cable_geom, pm.sp_geometry)
    LIMIT 1;

END
$BODY$;


-----------------------------get nearest fiber path point------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_nearestfiberpoint(
	lat double precision,
	lng double precision,
	mtrbuffer integer)
    RETURNS TABLE(nearest_fiber_point character varying, longitude double precision, latitude double precision, distance_meters double precision) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 2

AS $BODY$
DECLARE 
    v_geometry geometry;
    v_geometry_with_buffer geometry;
BEGIN
    -- Create customer point geometry
    v_geometry := ST_SetSRID(ST_MakePoint(lng, lat), 4326);

    -- Create buffer around the point
    v_geometry_with_buffer := ST_Buffer(v_geometry::geography, mtrbuffer)::geometry;

    RAISE INFO 'MTRBUFFER -> %', mtrbuffer;
    RAISE INFO 'v_geometry_with_buffer -> %', v_geometry_with_buffer;

    -- Return Nearest Fiber Path Point (from CABLE only)
    RETURN QUERY
    SELECT 
        network_id AS nearest_fiber_point,
        ST_X(nearest_point) AS longitude,
        ST_Y(nearest_point) AS latitude,
         ROUND(CAST(ST_Distance(v_geometry::geography, nearest_point::geography) AS numeric), 2)::double precision AS distance_meters
    FROM (
        SELECT cbl.network_id, ST_ClosestPoint(lm.sp_geometry, v_geometry) AS nearest_point
        FROM line_master lm
        INNER JOIN att_details_cable cbl 
            ON cbl.system_id = lm.system_id
        WHERE UPPER(lm.entity_type) = UPPER('Cable')
          AND ST_Intersects(lm.sp_geometry, v_geometry_with_buffer)
        ORDER BY ST_Distance(lm.sp_geometry, v_geometry)
        LIMIT 1
    ) sub;

END
$BODY$;