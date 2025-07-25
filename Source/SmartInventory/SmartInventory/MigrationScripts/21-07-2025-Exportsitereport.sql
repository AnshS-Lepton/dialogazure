CREATE OR REPLACE FUNCTION public.fn_get_site_list(
	p_system_id integer DEFAULT NULL::integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

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
      -- AND pod.fiber_oh_distance_to_network IS NULL
     --  AND pod.fiber_ug_distance_to_network IS NULL
     --  AND pod.total_fiber_distance IS NULL
      -- AND pod.nearest_site IS null 
     AND (p_system_id IS NULL OR pod.system_id = p_system_id)
    ) r;

END;
$BODY$;



CREATE OR REPLACE FUNCTION public.fn_get_update_site_fiber_details(
	linestring character varying,
	nearestsite_system_id integer,
	p_system_id integer,
	nearestsite_distance double precision,
	p_nearest_cable_geom character varying,
	p_nearest_cable_system_id integer,
	p_cable_end_to_site_geom character varying)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$



DECLARE
    v_full_geom geometry;
    v_length double precision := 0;
   ug_distance double precision := 0;
   oh_distance double precision := 0;
   v_manhole_span integer;
   v_pole_span integer;
  v_restricted_intersect_count integer := 0;
  POLE_INTERSECT_COUNT INTEGER = 0;
   MANHOLE_INTERSECT_COUNT INTEGER = 0;
   v_intersect_point geometry;
  v_substring_geom geometry;
 fracPoint double precision := 0; 
BEGIN

select value:: integer from global_settings  where key ='NearBySitePoleSpan' into v_pole_span ;
select value:: integer from global_settings  where key ='NearBySiteManholeSpan' into v_manhole_span ;

    v_full_geom := ST_GeomFromText('LINESTRING(' || p_cable_end_to_site_geom || ')', 4326);
    v_length = ST_Length(v_full_geom::geography);
   
SELECT 
  (ST_Intersection(lm.sp_geometry , v_full_geom)) into v_intersect_point
FROM line_master lm
  --where ST_Intersects(lm.sp_geometry , v_full_geom) and
  where system_id = p_nearest_cable_system_id;
 
   RAISE NOTICE 'Intersection Point = %', ST_AsText(v_intersect_point);
  
   IF v_intersect_point IS NOT null AND NOT ST_IsEmpty(v_intersect_point)  THEN
    fracPoint := ST_LineLocatePoint(v_full_geom, v_intersect_point);
	v_substring_geom := ST_LineSubstring(v_full_geom, 0.0::double precision, fracPoint::double precision);
	END IF;

   RAISE NOTICE 'v_substring_geom Point = %', ST_AsText(v_substring_geom);

   SELECT COUNT(*) into v_restricted_intersect_count FROM polygon_master
   WHERE ST_Intersects(sp_geometry, v_full_geom)
   AND entity_type = 'RestrictedArea';
              
        IF  v_restricted_intersect_count > 0
        then
         SELECT 
          SUM(ST_Length(ST_Intersection(v_full_geom, poly.sp_geometry)::geography)) into ug_distance
        FROM polygon_master poly
        WHERE 
            ST_Intersects(v_full_geom, poly.sp_geometry)
            AND poly.entity_type = 'RestrictedArea';
   
         oh_distance = v_length - ug_distance ; 
         
        else 
       oh_distance  = v_length;
        ug_distance = 0;

        END IF;
        -- POLE_INTERSECT_COUNT = (v_restricted_intersect_count * 2)+ 2;
         MANHOLE_INTERSECT_COUNT = (v_restricted_intersect_count * 2);        
         
         
        UPDATE att_details_pod
        SET fiber_oh_distance_to_network = oh_distance,
            fiber_ug_distance_to_network = ug_distance,
            total_fiber_distance = v_length,
            fiber_distance_to_nearest_site = nearestsite_distance + v_length,
            pole_count = CEIL(oh_distance/v_pole_span::double precision):: int,
            manhole_count = CEIL(ug_distance/v_manhole_span::double precision):: int,
            spliceclosure_count = FLOOR(v_length / 2000.0) + 1 
                                  + (CASE WHEN MOD(v_length,  2000.0) > 0 THEN 1 ELSE 0 END) 
                                  + (v_restricted_intersect_count * 2) ,
            manhole_span = v_manhole_span,
            pole_span =  v_pole_span,
            cable_intersection_count = MANHOLE_INTERSECT_COUNT,
            nearest_site = (select network_id from att_details_pod where system_id = nearestsite_system_id),
	nearest_site_geometry = case when ST_IsEmpty(v_intersect_point) 
    then ST_GeomFromText('LINESTRING(' || p_cable_end_to_site_geom || ')', 4326)
    else v_substring_geom 
end

        WHERE system_id = p_system_id;

END;
$BODY$;


CREATE OR REPLACE FUNCTION public.fn_get_nearest_site_records(
	p_system_id integer,
	p_network_id character varying,
	v_buffer integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    v_geom geometry;
    V_Count integer;
    v_nearest_cable_end_geom geometry;
    v_nearest_cable_system_id integer;
    rec record;

BEGIN
     Drop table if exists temp_cable_route;
     DROP TABLE IF EXISTS temp_att_details_site;

    -- Create temp table
    CREATE TEMP TABLE temp_att_details_site(
        system_id integer,
        network_id character varying,
        site_geometry character varying,
        distance double precision default 0.00,
        nearest_cable_system_id integer,
        nearest_cable_end_geom character varying,
        nearest_cable_to_site_geometry geometry,
        start_point character varying,
        end_point character varying
    ) ON COMMIT DROP;

  CREATE TEMP TABLE temp_cable_route(
        system_id integer,
        seq integer,
      site_system_id integer
    ) ON COMMIT DROP;

    -- Exit early if buffer exceeds safety limit
    IF v_buffer > 3000 THEN
        RETURN;
    END IF;

    -- Get the geometry of the target POD
    SELECT sp_geometry INTO v_geom  
    FROM point_master 
    WHERE system_id = p_system_id AND entity_type = 'POD';

    -- Get the nearest cable endpoint
    SELECT 
        lm.system_id,
         ST_ClosestPoint(lm.sp_geometry,v_geom) AS nearest_point 
    INTO v_nearest_cable_system_id, v_nearest_cable_end_geom
    FROM line_master lm
    WHERE lm.entity_type = 'Cable' 
      AND lm.network_status = 'A'
      AND ST_Intersects(
            lm.sp_geometry, 
            ST_Buffer(v_geom, v_buffer)::geometry
          )
     ORDER BY 
    ST_Distance(lm.sp_geometry, v_geom)
    LIMIT 1;

    -- Debug Info
    RAISE INFO 'Value of variable v_nearest_cable_end_geom: %', v_nearest_cable_end_geom;

   if (v_nearest_cable_end_geom is not null) then
   
    -- Insert nearby sites into temp table
    INSERT INTO temp_att_details_site  (system_id,network_id,site_geometry,start_point,distance,nearest_cable_system_id,nearest_cable_end_geom,end_point)  
    SELECT 
        pm.system_id, 
        pm.common_name AS network_id,
        ST_Y(pm.sp_geometry) || ' ' || ST_X(pm.sp_geometry) AS site_geom,
        ST_X(pm.sp_geometry) || ' ' || ST_Y(pm.sp_geometry) AS start_point,
        ST_DistanceSphere(pm.sp_geometry, v_geom) AS distance, 
        v_nearest_cable_system_id,
        ST_Y(v_nearest_cable_end_geom) || ' ' || ST_X(v_nearest_cable_end_geom) AS nearest_cable_end_geom_text,
        ST_X(v_nearest_cable_end_geom) || ' ' || ST_Y(v_nearest_cable_end_geom) AS end_point
    FROM point_master pm
    WHERE pm.entity_type = 'POD'
      AND pm.system_id != p_system_id
      AND ST_Within(
            pm.sp_geometry,
            ST_Buffer(v_nearest_cable_end_geom, v_buffer)::geometry
          )
    ORDER BY distance
    LIMIT 5;
        
    if exists (select 1 from temp_att_details_site) 
    then 
    for rec in (select start_point,end_point,system_id from  temp_att_details_site)
    loop
    
      insert into temp_cable_route(seq,system_id,site_system_id)
      SELECT SEQ, EDGE,rec.system_id FROM PGR_DIJKSTRA('SELECT ID, SOURCE, TARGET, COST, REVERSE_COST 
      FROM ROUTING_DATA_CORE_PLAN', (SELECT ID
      FROM ROUTING_DATA_CORE_PLAN_VERTICES_PGR
      WHERE ST_WITHIN( THE_GEOM, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT(' || rec.start_point || ')', 4326), 3))
      LIMIT 1 ),(SELECT ID FROM ROUTING_DATA_CORE_PLAN_VERTICES_PGR
      WHERE ST_WITHIN( THE_GEOM, 
      ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('|| rec.end_point ||')', 4326), 3)) 
      LIMIT 1));
      
      delete from temp_cable_route where system_id = -1 and site_system_id = rec.system_id;
      if exists( select 1 from temp_cable_route where site_system_id =  rec.system_id)
      then 
     
      UPDATE temp_att_details_site tas
		SET distance = (
		    SELECT SUM(ST_Length(lm.sp_geometry::geography))
		    FROM temp_cable_route tc
		    JOIN line_master lm ON lm.system_id = tc.system_id
		    WHERE tc.site_system_id = rec.system_id
		)
	  WHERE tas.system_id = rec.system_id;
		
      end if;
      
      end loop;
     end if;

    -- Recursive call if none found
    IF NOT EXISTS (SELECT 1 FROM temp_att_details_site) THEN
        RETURN QUERY
        SELECT * FROM fn_get_nearest_site_records(p_system_id, p_network_id, v_buffer + 100);
    ELSE
       
        RETURN QUERY
        SELECT row_to_json(t)
        FROM ( select nearest_cable_end_geom,nearest_cable_system_id,distance,site_geometry,system_id, network_id , nearest_cable_to_site_geometry
 from temp_att_details_site where distance is not null ) t order by t.distance limit 1;
    END IF;
 END IF;
END;
$BODY$;

