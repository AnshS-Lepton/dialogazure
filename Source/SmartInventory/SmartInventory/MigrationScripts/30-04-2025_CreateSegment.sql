
-------------------------------------------------Create table-------------------------------------------------------------------------
create table top_routes
(
route_id serial,
route_name character varying,
cable_names character varying,
total_length double precision,
agg1site_id integer,
agg2site_id integer,
route_geom geometry
) ;

--------------------------------------Add column to save selected route details-----------------------------------------------------
alter table top_segment add column route_id integer;

--------------------------------------insert data into module master	------------------------------------------------

INSERT INTO public.module_master
(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled)
VALUES('Create Segment', 'Create Segment', '', '', NULL, now(), 287, now(), 'Web', true, 'TPCS', (select id from module_master m where module_abbr='TPL' and module_name='Topology Details' ), 0, false, NULL, NULL);


----------------------------------------------------Get existing segment details-----------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_topology_get_existingsegmentdetails(
	p_regionid integer,
	p_agg1_site_id integer,
	p_agg2_site_id integer,
	p_route character varying,
	p_user_id integer)
    RETURNS TABLE(id integer, sequence integer, region_name character varying, segment_code character varying, route_name character varying, description character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
   

BEGIN

 IF p_route = '' THEN
  RETURN QUERY
select ts.id,ts.sequence,tr2.region_name,ts.segment_code,(select tr.route_name from top_routes tr where tr.route_id=ts.route_id)as route_name,ts.description from top_segment ts 
  left join top_region tr2 on tr2.id =ts.region_id 
  where ts.agg1_site_id=p_agg1_site_id and ts.agg2_site_id=p_agg2_site_id and ts.region_id=p_regionId order by ts.id desc limit 1;
  else
   RETURN QUERY
  select ts.id,ts.sequence,tr2.region_name,ts.segment_code,tr.route_name,ts.description from top_segment ts 
  inner join top_routes tr on tr.route_id=ts.route_id
  left join top_region tr2 on tr2.id =ts.region_id 
  where ts.agg1_site_id=p_agg1_site_id and ts.agg2_site_id=p_agg2_site_id and ts.region_id=p_regionId and tr.route_name=p_route order by ts.id desc limit 1;
  end if;
            

    

END;
$BODY$;

---------------------------------------------------------- segment cable mapping function------------------------------------

DROP FUNCTION IF EXISTS public.fn_insert_top_segment_cable_mapping(integer, integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_insert_top_segment_cable_mapping(
	p_agg1_system_id integer,
	p_agg2_system_id integer,
	p_route_id integer,
	p_user_id integer,
	p_segment_id integer)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE
        p_cable_names character varying;
BEGIN
    
        
    -- Correct usage of SELECT INTO
    SELECT cable_names
    INTO p_cable_names
    FROM top_routes
    WHERE route_id = p_route_id;

    -- If no cable names were found, raise an exception
    IF p_cable_names IS NULL THEN
        RAISE EXCEPTION 'No cable names found for route_id: %', p_route_id;
    END IF;
    
    -- Log the cable names for debugging purposes
    RAISE INFO 'Cable Names: %', p_cable_names;

    -- Insert into top_segment_cable_mapping
    INSERT INTO top_segment_cable_mapping (segment_id, cable_id)
    SELECT p_segment_id, system_id
    FROM att_details_cable adc
    WHERE network_id IN (
        SELECT unnest(string_to_array(trim(both '{}' from p_cable_names), ',')) AS cable_id
    );

    -- Return true if the insertion was successful
    RETURN TRUE;

EXCEPTION
    WHEN OTHERS THEN
        -- Log the exception for debugging purposes
        RAISE WARNING 'Error occurred during function execution: %', SQLERRM;
        RETURN FALSE;
END;
$BODY$;

ALTER FUNCTION public.fn_insert_top_segment_cable_mapping(integer, integer, integer, integer, integer)
    OWNER TO postgres;

--------------------------------- get geometry details for route to show on map ---------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_geometrydetailbyroute(
	p_route_id integer,
	p_geomtype character varying)
    RETURNS TABLE(sp_geometry text, geometry_extent text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE sp_geometry text ; 
        geometry_extent text;
        v_sp_geometry geometry ;

BEGIN
 execute 'select tr.route_geom  from top_routes tr where tr.route_id='||p_route_id||''into v_sp_geometry ;
 select ST_AsText(v_sp_geometry)::text into sp_geometry ;
 select ST_Extent(v_sp_geometry)::text into geometry_extent;
RETURN QUERY
select  sp_geometry,geometry_extent;
END
$BODY$;

----------------------------------------------- Modify the function to get connected aggregate site details-----------------------------------------



 DROP FUNCTION IF EXISTS public.fn_topology_get_segmentdetailssitewise(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_segmentdetailssitewise(
	p_site_id integer,
	p_user_id integer)
    RETURNS TABLE(id integer, segment_code character varying, region_id integer, agg1_site_id character varying, agg2_site_id character varying, agg1_system_id integer, agg2_system_id integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
p_system_id integer;
    p_site1_geom character varying;
    v_geometry_with_buffer geometry;
    sql TEXT;
    geom_text TEXT;
    p_site2_geom character varying;
	 rec RECORD;
 
   

BEGIN

create temp table temp_aggsite(
system_id integer ,
	network_id character varying,
	geom character varying
)ON COMMIT DROP;

  -- Create temporary table
    CREATE TEMP TABLE temp_cable_routes(
        seq integer,
        path_seq integer, 
        edge_targetid integer, 
        roadline_geomtext text, 
        start_point character varying,
        end_point character varying,
        message varchar NULL,
        is_valid boolean DEFAULT true,
        avaiable_core_count integer DEFAULT 0,
        user_id integer,
		system_id integer
    ) ON COMMIT DROP;
	

    -- Get the reference site's geometry
    SELECT longitude || ' ' || latitude AS geom
    INTO p_site1_geom
    FROM att_details_pod  
    WHERE system_id = p_site_id;
	
	 -- Get  site details From the p_agg2_site_id
	
	 insert into temp_aggsite(system_id,network_id,geom)
     SELECT p.system_id,p.network_id, p.longitude || ' ' || p.latitude AS geom
     FROM att_details_pod p
     WHERE p.system_id <> p_site_id and p.is_agg_site ='true';

RAISE INFO 'temp_aggsite row count -> %', (SELECT COUNT(1) FROM temp_aggsite);
 RAISE INFO 'p_site_id -> %', p_site_id;
    RAISE INFO 'p_site1_geom -> %', p_site1_geom;

    v_geometry_with_buffer := ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326);

    RAISE INFO 'v_geometry_with_buffer -> %', v_geometry_with_buffer;

   FOR rec IN (SELECT * FROM temp_aggsite) LOOP
       p_site2_geom :='';
	   p_site2_geom =rec.geom;
	   p_system_id=0;
	   p_system_id=rec.system_id;
	   
	 RAISE INFO 'p_system_id -> %', p_system_id;
    RAISE INFO 'p_site2_geom -> %', p_site2_geom;
  

     -- Populate temporary cable routes table
      INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id,system_id)
		   
	  SELECT seq, path_seq, edge,p_user_id,p_system_id FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	  FROM routing_data_core_plan', (SELECT pgr.id
	  FROM routing_data_core_plan_vertices_pgr pgr
	  WHERE ST_Within( pgr.the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), 2))
	  limit 1 ),(SELECT pgr.id FROM routing_data_core_plan_vertices_pgr pgr
	  WHERE ST_Within( pgr.the_geom, 
	  ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_site2_geom ||')', 4326), 2)) 
	  limit 1));

   END LOOP;
   
    DELETE FROM temp_cable_routes WHERE edge_targetid = -1 AND user_id = p_user_id;

    RAISE INFO 'temp_cable_routes row count -> %', (SELECT COUNT(1) FROM temp_cable_routes);
	  RAISE INFO 'temp_cable_routes row count -> %', (SELECT edge_targetid FROM temp_cable_routes LIMIT 1);

RETURN QUERY

    SELECT distinct s.id, CONCAT(s.segment_code,'/', tr.region_name)::varchar as segment_code, s.region_id, CONCAT(adp.site_id,' (', adp.site_name, ')')::varchar AS agg1_site_id,CONCAT(adp2.site_id, ' (', adp2.site_name, ')')::varchar AS agg2_site_id,COALESCE(adp.system_id, 0) as agg_01,COALESCE(adp2.system_id,0) as agg_02 
            FROM top_segment_cable_mapping ts
            left JOIN top_segment s ON s.id = ts.segment_id
			left join top_region tr on tr.id=s.region_id
            left join att_details_cable cbl on cbl.system_id =ts.cable_id 
			left join att_details_pod adp on adp.system_id= s.agg1_site_id AND EXISTS ( SELECT 1 FROM temp_cable_routes tcr WHERE tcr.system_id = s.agg1_site_id GROUP BY tcr.system_id HAVING COUNT(*) > 1)
            left join att_details_pod adp2 on adp2.system_id = s.agg2_site_id AND EXISTS ( SELECT 1 FROM temp_cable_routes tcr WHERE tcr.system_id = s.agg2_site_id GROUP BY tcr.system_id HAVING COUNT(*) > 1)
            INNER JOIN temp_cable_routes tc ON tc.edge_targetid = cbl.system_id;
           

END;
$BODY$;

ALTER FUNCTION public.fn_topology_get_segmentdetailssitewise(integer, integer)
    OWNER TO postgres;


---------------------------------------------Modify Existing Function ---------------------------------------------------

DROP FUNCTION IF EXISTS public.fn_topology_get_cableroute(character varying, character varying, integer);


CREATE OR REPLACE FUNCTION public.fn_topology_get_cableroute(
	p_agg1site_id integer,
	p_agg2site_id integer,
	p_user_id integer)
    RETURNS TABLE(route_id integer, route_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    v_source_geom geometry;
    v_dest_geom geometry;
    MAX_DEPTH integer := 20;
BEGIN

/*create temp table temp_routes
(
rout_id serial,
route_name character varying,
cable_names character varying,
total_length double precision,
route_geom geometry
) on commit drop;*/

    -- Get source site geometry (start point)
    SELECT ST_SetSRID(ST_MakePoint(longitude, latitude), 4326)
    INTO v_source_geom
    FROM att_details_pod
    WHERE att_details_pod.system_id = p_agg1site_id::int;

    -- Get destination site geometry (end point)
    SELECT ST_SetSRID(ST_MakePoint(longitude, latitude), 4326)
    INTO v_dest_geom
    FROM att_details_pod
    WHERE att_details_pod.system_id = p_agg2site_id::int;

WITH RECURSIVE
-- 1. Input points
 input AS (
  SELECT 
    v_source_geom::geometry AS source,
    v_dest_geom::geometry AS dest
),

-- 2. Find all cables starting near the source point
start_cables AS (
  SELECT 
    c.system_id,
    c.common_name,
    c.sp_geometry,
    CASE 
      WHEN  st_distance(ST_StartPoint(c.sp_geometry), i.source,false) < st_distance(ST_endPoint(c.sp_geometry), i.source,false)
      THEN ST_endPoint(c.sp_geometry)
      ELSE ST_StartPoint(c.sp_geometry)
    END AS current_point,
    i.dest
  FROM line_master c
  CROSS JOIN input i
  WHERE ST_DWithin(c.sp_geometry, i.source, 0.00001)
),

-- 3. Recursive traversal
path AS (
  SELECT 
    ARRAY[c.system_id] AS cable_ids,
    ARRAY[c.common_name]::varchar[] AS cable_names,
    c.sp_geometry,
    c.current_point,
    c.system_id,
    c.dest,
    1 AS depth,
    ST_Length(c.sp_geometry::geography) AS total_length,
    c.sp_geometry AS full_path_geom
  FROM start_cables c

  UNION ALL

  SELECT 
    p.cable_ids || c.system_id,
    p.cable_names || c.common_name,
    c.sp_geometry,
    CASE 
      WHEN ST_Within(ST_StartPoint(c.sp_geometry),st_buffer_meters(p.current_point,3))
      THEN ST_EndPoint(c.sp_geometry)
      ELSE ST_StartPoint(c.sp_geometry)
    END,
    c.system_id,
    p.dest,
    p.depth + 1,
    p.total_length + ST_Length(c.sp_geometry::geography),
    ST_Union(p.full_path_geom, c.sp_geometry)
  FROM path p
  JOIN line_master c 
    ON (
      (
        ST_Within(ST_StartPoint(c.sp_geometry),st_buffer_meters(p.current_point,3))
        OR ST_Within(ST_EndPoint(c.sp_geometry),st_buffer_meters(p.current_point,3))
      )
      AND NOT c.system_id = ANY(p.cable_ids)
    )
),
--select * from path
-- select b.* from line_master a
-- inner join line_master b on ST_Within( ST_StartPoint(b.sp_geometry),st_buffer_meters(st_endpoint(a.sp_geometry),3))
-- where a.common_name='AMP-CBL000765'

-- 4. Valid paths that reach near destination
valid_routes AS (
  SELECT 
    ROW_NUMBER() OVER (ORDER BY total_length) AS route_rank,
    cable_names,
    cable_ids,
    total_length,
    ST_LineMerge(ST_Union(full_path_geom)) AS route_geom
  FROM path
  WHERE ST_Within(dest,st_buffer_meters(current_point,3))
  GROUP BY cable_names, cable_ids,total_length
)

-- 5. Final output with route name
insert into top_routes(route_name,cable_names,total_length,route_geom,agg1site_id,agg2site_id)
SELECT 
  'Route_' || route_rank AS route_name,
  cable_names,
  total_length,
 route_geom,
 p_agg1site_id,
 p_agg2site_id
FROM valid_routes
ORDER BY route_rank;

    RETURN QUERY
    select Max(tr.route_id) AS route_id,tr.route_name from top_routes tr where agg1site_id=p_agg1site_id and agg2site_id=p_agg2site_id
    GROUP BY tr.route_name;
END;
$BODY$;
