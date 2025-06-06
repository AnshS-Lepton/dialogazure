CREATE OR REPLACE FUNCTION public.fn_get_update_site_fiber_details(linestring character varying, nearestsite_system_id integer, p_system_id integer, nearestsite_distance double precision)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_full_geom geometry;
    v_length double precision := 0;
   ug_distance double precision := 0;
   oh_distance double precision := 0;
BEGIN
    v_full_geom := ST_GeomFromText('LINESTRING(' || lineString || ')', 4326);
    v_length = ST_Length(v_full_geom::geography);
 
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, v_full_geom)
              AND entity_type = 'Restricted_Area'
        ) 
        then
         SELECT 
          SUM(ST_Length(ST_Intersection(v_full_geom, poly.sp_geometry)::geography)) into oh_distance
        FROM polygon_master poly
        WHERE 
            ST_Intersects(v_full_geom, poly.sp_geometry)
            AND poly.entity_type = 'Restricted_Area';
   
           ug_distance = v_length - oh_distance ;
  

        else 
        ug_distance = v_length;
        oh_distance = 0;

        END IF;
    
        UPDATE att_details_pod
        SET fiber_oh_distance_to_network = oh_distance,
            fiber_ug_distance_to_network = ug_distance,
            total_fiber_distance = v_length,
            fiber_distance_to_nearest_site = nearestsite_distance,
            nearest_site = (select network_id from att_details_pod where system_id = nearestsite_system_id)
        WHERE system_id = p_system_id;


END;
$function$
;

-------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_nearest_site_records(p_system_id integer, p_network_id character varying, v_buffer integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_geom geometry;
begin
	
	CREATE TEMP TABLE temp_att_details_site(
    system_id integer,
    network_id character varying,
    site_geometry character varying,
    distance double precision
     )on commit drop;
 
     -- Exit if buffer exceeded limit
    IF v_buffer > 500 THEN
        RETURN;
    END IF;
   
     -- Get geometry of the given POD
    SELECT sp_geometry 
    INTO v_geom 
    FROM point_master 
    WHERE system_id = p_system_id 
      AND entity_type = 'POD';
     
	insert into temp_att_details_site
     SELECT pm.system_id, pm.common_name as network_id,
     ST_Y(pm.sp_geometry) || ' ' || ST_X(pm.sp_geometry) AS site_geom,
     ST_DistanceSphere(pm.sp_geometry, v_geom) AS distance
     FROM point_master pm
        WHERE pm.entity_type = 'POD'
           AND pm.system_id not in (p_system_id) 
           AND ST_Within(pm.sp_geometry, ST_Buffer(v_geom::geography, v_buffer)::geometry)
        ORDER BY 
            distance
        LIMIT 5;
       
      IF NOT EXISTS (SELECT 1 FROM temp_att_details_site) THEN
        
       RETURN QUERY
        SELECT * FROM fn_get_nearest_site_records(p_system_id, p_network_id, v_buffer + 100);
      
       ELSE
        -- Return result as JSON
        RETURN QUERY
        SELECT row_to_json(t)
        FROM temp_att_details_site t;
    END IF;
   
END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_nearest_site_records(int4, varchar, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_nearest_site_records(int4, varchar, int4) TO postgres;


----------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_site_list()
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
BEGIN
 RETURN QUERY
    SELECT row_to_json(r)
    FROM 
    (SELECT
        pod.fiber_oh_distance_to_network,
        pod.fiber_ug_distance_to_network,
        pod.total_fiber_distance,
        pod.fiber_distance_to_nearest_site,
        pod.nearest_site,
        pod.network_id,
        pm.system_id,
        ST_Y(pm.sp_geometry) || ' ' || ST_X(pm.sp_geometry) as sp_geometry
    FROM
        att_details_pod pod
        JOIN point_master pm ON pod.system_id = pm.system_id
    WHERE
        pm.entity_type = 'POD' 
       AND pod.fiber_oh_distance_to_network IS NULL
       AND pod.fiber_ug_distance_to_network IS NULL
       AND pod.total_fiber_distance IS NULL
       AND pod.nearest_site IS null) r;

END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_site_list() OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_site_list() TO postgres;
