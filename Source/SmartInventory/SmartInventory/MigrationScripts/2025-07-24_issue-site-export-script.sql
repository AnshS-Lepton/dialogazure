CREATE OR REPLACE FUNCTION public.fn_get_nearest_site_records(p_system_id integer, p_network_id character varying, v_buffer integer, p_page_number integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_geom geometry;
    v_nearest_cable_end_geom geometry;
    v_nearest_cable_system_id integer;
    rec record;
    cableStartPoint geometry;
    cableEndPoint geometry;
    v_full_geom geometry;
    v_substring_geom geometry;
    fracPoint double precision;
    v_page_size INTEGER := 10;
    v_offset INTEGER := (p_page_number - 1) * 10;
    v_nearest_cable_geom geometry;
BEGIN
    IF v_buffer > 10000 THEN RETURN; END IF;

    SELECT sp_geometry INTO v_geom  
    FROM point_master 
    WHERE system_id = p_system_id AND entity_type = 'POD';

    SELECT 
        lm.system_id,
        ST_ClosestPoint(lm.sp_geometry, v_geom)
    INTO 
        v_nearest_cable_system_id,
        v_nearest_cable_end_geom
    FROM line_master lm
    WHERE lm.entity_type = 'Cable' 
      AND lm.network_status = 'A'
      AND ST_Intersects(lm.sp_geometry, ST_Buffer(v_geom, v_buffer)::geometry)
    ORDER BY ST_Distance(lm.sp_geometry, v_geom)
    LIMIT 1;

    IF v_nearest_cable_end_geom IS NULL THEN RETURN; END IF;
	 Drop table if exists temp_cable_route;
     DROP TABLE IF EXISTS temp_att_details_site;
    --TRUNCATE TABLE temp_cable_route;
    --TRUNCATE TABLE temp_att_details_site;

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

   
    INSERT INTO temp_att_details_site(system_id, network_id, site_geometry, start_point, nearest_cable_system_id, nearest_cable_end_geom)
    SELECT 
        pm.system_id, 
        pm.common_name AS network_id,
        ST_Y(pm.sp_geometry) || ' ' || ST_X(pm.sp_geometry),
        ST_X(pm.sp_geometry) || ' ' || ST_Y(pm.sp_geometry),
        v_nearest_cable_system_id,
        ST_Y(v_nearest_cable_end_geom) || ' ' || ST_X(v_nearest_cable_end_geom)
    FROM point_master pm
    WHERE pm.entity_type = 'POD'
      AND pm.system_id != p_system_id
      AND pm.network_status = 'A'
      AND ST_Within(pm.sp_geometry, ST_Buffer(v_nearest_cable_end_geom::geography, v_buffer)::geometry)
    ORDER BY ST_DistanceSphere(pm.sp_geometry, v_nearest_cable_end_geom)
    LIMIT v_page_size OFFSET v_offset;

    FOR rec IN SELECT * FROM temp_att_details_site loop
	    
        SELECT ST_StartPoint(sp_geometry), ST_EndPoint(sp_geometry)
        INTO cableStartPoint, cableEndPoint
        FROM line_master
        WHERE system_id = rec.nearest_cable_system_id
          AND entity_type = 'Cable';

        INSERT INTO temp_cable_route(seq, system_id, site_system_id)
        SELECT seq, edge, rec.system_id
        FROM pgr_dijkstra(
            'SELECT id, source, target, cost, reverse_cost FROM routing_data_core_plan',
            (
                SELECT id FROM routing_data_core_plan_vertices_pgr
                WHERE ST_Within(the_geom, ST_Buffer_Meters(ST_GeomFromText('POINT(' || rec.start_point || ')', 4326), 3))
                LIMIT 1
            ),
            (
                SELECT id FROM routing_data_core_plan_vertices_pgr
                WHERE ST_Within(the_geom, ST_Buffer_Meters(cableStartPoint, 3))
                LIMIT 1
            )
        );

        DELETE FROM temp_cable_route WHERE system_id = -1 AND site_system_id = rec.system_id;

        IF EXISTS (
            SELECT 1 FROM temp_cable_route cbl
            JOIN line_master lm ON lm.system_id = cbl.system_id
            WHERE lm.entity_type = 'Cable'
              AND cbl.site_system_id = rec.system_id
              AND ST_DWithin(lm.sp_geometry, v_nearest_cable_end_geom, 1)
        ) then

        SELECT sp_geometry INTO v_nearest_cable_geom
        FROM line_master
        WHERE system_id = rec.nearest_cable_system_id AND entity_type = 'Cable';
           
            fracPoint := ST_LineLocatePoint(v_nearest_cable_geom, v_nearest_cable_end_geom);

            v_substring_geom := ST_LineSubstring(v_nearest_cable_geom, 0.0, fracPoint::double precision);


		IF GeometryType(v_substring_geom) = 'POINT' THEN
		   
		SELECT ST_LineMerge(ST_CollectionExtract(ST_Union(geom.geometry), 2))
		INTO v_full_geom
		FROM (
		    SELECT v_nearest_cable_geom AS geometry
		    UNION
		    SELECT lm.sp_geometry AS geometry
		    FROM temp_cable_route tc
		    JOIN line_master lm ON lm.system_id = tc.system_id
		    WHERE lm.network_status = 'A'
		      AND tc.site_system_id = rec.system_id
		      AND tc.system_id != rec.nearest_cable_system_id
		    GROUP BY tc.site_system_id, lm.sp_geometry
		) geom;

		else 

		SELECT ST_LineMerge(ST_CollectionExtract(ST_Union(geom.geometry), 2))
		INTO v_full_geom
		FROM (
		    SELECT v_substring_geom AS geometry
		    UNION
		    SELECT lm.sp_geometry AS geometry
		    FROM temp_cable_route tc
		    JOIN line_master lm ON lm.system_id = tc.system_id
		    WHERE lm.network_status = 'A'
		      AND tc.site_system_id = rec.system_id
		      AND tc.system_id != rec.nearest_cable_system_id
		    GROUP BY tc.site_system_id, lm.sp_geometry
		) AS geom;

		END IF;

        ELSE
            DELETE FROM temp_cable_route WHERE site_system_id = rec.system_id;

            INSERT INTO temp_cable_route(seq, system_id, site_system_id)
            SELECT seq, edge, rec.system_id
            FROM pgr_dijkstra(
                'SELECT id, source, target, cost, reverse_cost FROM routing_data_core_plan',
                (
                    SELECT id FROM routing_data_core_plan_vertices_pgr
                    WHERE ST_Within(the_geom, ST_Buffer_Meters(ST_GeomFromText('POINT(' || rec.start_point || ')', 4326), 3))
                    LIMIT 1
                ),
                (
                    SELECT id FROM routing_data_core_plan_vertices_pgr
                    WHERE ST_Within(the_geom, ST_Buffer_Meters(cableEndPoint, 3))
                    LIMIT 1
                )
            );

            DELETE FROM temp_cable_route WHERE system_id = -1 AND site_system_id = rec.system_id;

            IF EXISTS (
                SELECT 1 FROM temp_cable_route cbl
                JOIN line_master lm ON lm.system_id = cbl.system_id
                WHERE lm.entity_type = 'Cable'
                  AND cbl.site_system_id = rec.system_id
                  AND ST_DWithin(lm.sp_geometry, v_nearest_cable_end_geom, 1)
            ) THEN

                SELECT sp_geometry INTO v_nearest_cable_geom
                FROM line_master
                WHERE system_id = rec.nearest_cable_system_id AND entity_type = 'Cable';

                fracPoint := ST_LineLocatePoint(v_nearest_cable_geom, v_nearest_cable_end_geom);

                v_substring_geom := ST_LineSubstring(v_nearest_cable_geom, fracPoint, 1.0);

		IF GeometryType(v_substring_geom) = 'POINT' THEN
	
		SELECT ST_LineMerge(ST_CollectionExtract(ST_Union(geom.geometry), 2))
		INTO v_full_geom
		FROM (
		    SELECT lm.sp_geometry AS geometry
		    FROM temp_cable_route tc
		    JOIN line_master lm ON lm.system_id = tc.system_id
		    WHERE lm.network_status = 'A'
		      AND tc.site_system_id = rec.system_id
		      AND tc.system_id != rec.nearest_cable_system_id
		    GROUP BY tc.site_system_id, lm.sp_geometry
		
		    UNION
		
		    SELECT v_nearest_cable_geom AS geometry
		) AS geom;

          
		else 

	    SELECT ST_LineMerge(ST_CollectionExtract(ST_Union(geom.geometry), 2))
		INTO v_full_geom
		FROM (
		    SELECT lm.sp_geometry AS geometry
		    FROM temp_cable_route tc
		    JOIN line_master lm ON lm.system_id = tc.system_id
		    WHERE lm.network_status = 'A'
		      AND tc.site_system_id = rec.system_id
		      AND tc.system_id != rec.nearest_cable_system_id
		    GROUP BY tc.site_system_id, lm.sp_geometry 
		    UNION
		
		    SELECT v_substring_geom AS geometry
		) AS geom;

		
		END IF;
                         
        END IF;
        END IF;

        UPDATE temp_att_details_site
        SET nearest_cable_to_site_geometry = v_full_geom,
            distance = ST_Length(v_full_geom::geography)
        WHERE system_id = rec.system_id;
    END LOOP;

    IF NOT EXISTS (SELECT 1 FROM temp_cable_route) THEN
        RETURN QUERY
        SELECT * FROM fn_get_nearest_site_records(p_system_id, p_network_id, (v_buffer + 500), (p_page_number + 1));
    ELSE
        RETURN QUERY
        SELECT row_to_json(t)
        FROM (
            SELECT site.nearest_cable_end_geom, site.nearest_cable_system_id,
                   site.distance, site.site_geometry, site.system_id, 
                   site.network_id, site.nearest_cable_to_site_geometry
            FROM temp_att_details_site site
            JOIN temp_cable_route tmp ON tmp.site_system_id = site.system_id
            ORDER BY site.distance
            LIMIT 1
        ) t;
    END IF;
END;
$function$
;

-------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_update_site_fiber_details(linestring character varying, nearestsite_system_id integer, p_system_id integer, nearestsite_distance double precision, p_nearest_cable_geom character varying, p_nearest_cable_system_id integer, p_cable_end_to_site_geom character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$


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
  start_frac double precision;
  end_frac double precision;
BEGIN

	select value:: integer from global_settings  where key ='NearBySitePoleSpan' 
    into v_pole_span ;
	select value:: integer from global_settings  where key ='NearBySiteManholeSpan' 
    into v_manhole_span ;

    v_full_geom := ST_GeomFromText('LINESTRING(' || p_cable_end_to_site_geom || ')', 4326);
    v_length = ST_Length(v_full_geom::geography);
   
	SELECT 
	  (ST_Intersection(lm.sp_geometry , v_full_geom)) into v_intersect_point
	FROM line_master lm
	  where system_id = p_nearest_cable_system_id;
 
   IF v_intersect_point IS NOT null AND NOT ST_IsEmpty(v_intersect_point)  then
   IF ST_GeometryType(v_intersect_point) = 'ST_LineString' or  ST_GeometryType(v_intersect_point) = 'ST_MultiLineString'  
   then 
  start_frac := ST_LineLocatePoint(v_full_geom, 							ST_StartPoint(v_intersect_point));
  end_frac := ST_LineLocatePoint(v_full_geom, 							ST_EndPoint(v_intersect_point));
   fracPoint := LEAST(start_frac, end_frac);
   v_substring_geom := ST_LineSubstring(v_full_geom, 0.0::double precision, 	fracPoint::double precision);
   else
    fracPoint := ST_LineLocatePoint(v_full_geom, v_intersect_point);
	v_substring_geom := ST_LineSubstring(v_full_geom, 0.0::double precision, 	fracPoint::double precision);

  END IF;
  END IF;

   SELECT COUNT(*) into v_restricted_intersect_count FROM polygon_master
   WHERE ST_Intersects(sp_geometry, v_full_geom)
   AND entity_type = 'RestrictedArea';
           
        IF  v_restricted_intersect_count > 0
        then
          SELECT 
          SUM(ST_Length(ST_Intersection(v_full_geom, poly.sp_geometry)::geography)) 
          into ug_distance
          FROM polygon_master poly
           WHERE 
            ST_Intersects(v_full_geom, poly.sp_geometry)
            AND poly.entity_type = 'RestrictedArea';
   
           oh_distance = v_length - ug_distance ; 

        else 
          oh_distance  = v_length;
          ug_distance = 0;

        END IF;
       
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
            nearest_site = (select network_id from att_details_pod 
            where system_id = nearestsite_system_id),
	nearest_site_geometry = case when ST_IsEmpty(v_intersect_point) 
    then ST_GeomFromText('LINESTRING(' || p_cable_end_to_site_geom || ')', 4326)
    else v_substring_geom 
	end WHERE system_id = p_system_id;

	WITH filtered_items AS (
		SELECT cost_per_unit, category_reference
		FROM item_template_master
		WHERE category_reference IN ('Pole', 'Manhole', 'SpliceClosure')
		  AND specification IN ('Generic', '48 Port SpliceClouser')
	),
	cost_calc AS (
		SELECT
			a.system_id,
			SUM(
				CASE 
					WHEN f.category_reference = 'Pole' THEN a.pole_count * f.cost_per_unit
					WHEN f.category_reference = 'Manhole' THEN a.manhole_count * f.cost_per_unit
					WHEN f.category_reference = 'SpliceClosure' THEN a.spliceclosure_count * f.cost_per_unit
					ELSE 0
				END
			) AS total_cost
		FROM att_details_pod a
		JOIN filtered_items f
		  ON f.category_reference IN ('Pole', 'Manhole', 'SpliceClosure')
		WHERE a.system_id = p_system_id
		GROUP BY a.system_id
	)
	UPDATE att_details_pod a
	SET plan_cost = c.total_cost
	FROM cost_calc c
	WHERE a.system_id = c.system_id;
	 
END;
$function$
;

-----------------------------------------------------------------------------------
DROP FUNCTION IF EXISTS fn_get_site_list(integer);

CREATE OR REPLACE FUNCTION public.fn_get_site_list(p_system_id integer DEFAULT NULL::integer)
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
      -- AND pod.fiber_oh_distance_to_network IS NULL
     --  AND pod.fiber_ug_distance_to_network IS NULL
     --  AND pod.total_fiber_distance IS NULL
      -- AND pod.nearest_site IS null 
     AND (p_system_id IS NULL OR pod.system_id = p_system_id)
    ) r;

END;
$function$
;


-------------------------------------------------------------------

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
       order by pod.system_id
      ) r;
-- 
END;
$function$
;

