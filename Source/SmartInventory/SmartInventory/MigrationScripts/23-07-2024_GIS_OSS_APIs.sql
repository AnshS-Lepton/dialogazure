-- FUNCTION: public.fn_api_get_ossserviceability(double precision, double precision)

-- DROP FUNCTION IF EXISTS public.fn_api_get_ossserviceability(double precision, double precision);
-- select fn_api_get_ossserviceability('40.041395','1.765633')
-- select * from point_master where latitude='1.765633'
 
CREATE OR REPLACE FUNCTION public.fn_api_get_ossserviceability(
	p_lng double precision,
	p_lat double precision)
    RETURNS TABLE(geom_type character varying, entity_type character varying, 
				  entity_title character varying, system_id integer,
				  common_name character varying, geom text, centroid_geom text, 
				  network_status character varying, display_name character varying, 
				  total_core character varying, distance numeric) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    v_buffer double precision;
BEGIN
    CREATE TEMP TABLE temp_result (
        geom_type character varying,
        entity_type character varying,
        entity_title character varying,
        system_id integer,
        common_name character varying,
        geom text,
        centroid_geom text,
        network_status character varying,
        display_name character varying,
        total_core character varying,
        distance numeric(10,2)
    ) ON COMMIT DROP;

    SELECT value INTO v_buffer FROM global_settings WHERE key='serviceability_buffer';

    INSERT INTO temp_result (
        geom_type, entity_type, entity_title, system_id,
        geom, centroid_geom, network_status,
        display_name, total_core, distance
    )
    SELECT 
        'Point', p.entity_type, p.entity_type, p.system_id,
        ST_AsText(sp_geometry), sp_geometry, p.network_status,
        p.common_name, p.no_of_ports,
        
            ST_Distance(
                ST_GeomFromText('POINT(' || p.longitude || ' ' || p.latitude || ')', 4326),
                ST_GeomFromText('POINT(' || p_lng || ' ' || p_lat || ')', 4326)
            ) 
        	
    FROM point_master p
    WHERE ST_Within(
        p.sp_geometry, 
        ST_Buffer_Meters(
            ST_GeomFromText('POINT(' || p_lng || ' ' || p_lat || ')', 4326), 
            v_buffer
        )
    );

    RETURN QUERY
    SELECT 
        t.geom_type, t.entity_type, t.entity_title, t.system_id, t.common_name,
        t.geom, t.centroid_geom, t.network_status, t.display_name, t.total_core,
		t.distance
    FROM temp_result t 
    ORDER BY t.distance;

END;
$BODY$;

ALTER FUNCTION public.fn_api_get_ossserviceability(double precision, double precision)
    OWNER TO postgres;
