

---------------------------------Modify the function fn_topology_get_sites--------------------------------------------

DROP FUNCTION IF EXISTS public.fn_topology_get_sites(integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_sites(
	p_site_id integer,
	p_ring_id integer,
	p_distance integer,
	p_user_id integer)
    RETURNS TABLE(siteid integer, sitename character varying, sitedistance numeric, ringid integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE 
    p_site1_geom geometry;
	v_geometry_with_buffer geometry;
	sql TEXT;
	 geom_text TEXT;

BEGIN
    -- Create a temporary table to store site geometries
    CREATE TEMP TABLE temp_site_geom( 
        site_id integer, 
        site_name character varying, 
        geom geometry,
		ringid integer,
        user_id integer
    ) ON COMMIT DROP;

    -- Get the reference site's geometry
    SELECT ST_GeomFromText('POINT(' || longitude || ' ' || latitude || ')', 4326) 
    INTO p_site1_geom
    FROM att_details_pod  
    WHERE system_id = p_site_id;

Raise info 'p_site1_geom ->%',p_site1_geom;
v_geometry_with_buffer = ST_BUFFER_METERS(p_site1_geom, p_distance);

Raise info 'v_geometry_with_buffer ->%',v_geometry_with_buffer;

    -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom(site_id, site_name, geom,ringid, user_id)
    SELECT 
        p.system_id, 
       CONCAT(p.site_name,' (', tr.ring_code, ' )') AS site_name,  
        ST_GeomFromText('POINT(' || p.longitude || ' ' || p.latitude || ')', 4326),
		p.ring_id,
        p_user_id   
	FROM  att_details_pod p
	left JOIN 
	top_ring tr ON tr.id = p.ring_id

   WHERE  ROUND(ST_DistanceSphere(p_site1_geom, ST_GeomFromText('POINT(' || p.longitude || ' ' || p.latitude || ')', 4326))::numeric, 2)<= p_distance
 --  group by p.system_id,p.ring_id,tr.ring_code,p.site_name
   ;

Raise info 'Temp table row ->%',(select count(1) from temp_site_geom);

   -- WHERE ring_id = p_ring_id;
 geom_text := ST_AsText(p_site1_geom);
 
 
        sql := 'SELECT 
                    t.site_id AS siteid,
                    t.site_name AS sitename, 
                    ROUND(ST_DistanceSphere('|| quote_literal(geom_text) || ', t.geom)::numeric, 2) AS sitedistance,
                    COALESCE(t.ringid, 0) AS ringid
                FROM temp_site_geom t 
                ORDER BY sitedistance';

    -- Execute the SQL and return the result
    RETURN QUERY EXECUTE sql;
	
END;
$BODY$;

---------------------------------Create  function fn_topology_sites_dissociation--------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_topology_sites_dissociation(
	p_base_site_id integer,
	p_site_id integer,
	p_ring_id integer,
	p_distance integer,
	p_user_id integer)
    RETURNS TABLE(siteid integer, sitename character varying, sitedistance numeric, ringid integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE 
    p_site1_geom geometry;
	v_geometry_with_buffer geometry;
	sql TEXT;
	 geom_text TEXT;

BEGIN
   
-- site ring dissociation

update att_details_pod set ring_id=null where system_id=p_site_id and ring_id=p_ring_id;
    -- Get the reference site's geometry
   

    -- Execute the SQL and return the result
    RETURN QUERY
	
	SELECT 
        fn.siteid, fn.sitename, fn.sitedistance, fn.ringid  
    FROM fn_topology_get_sites(p_base_site_id, p_ring_id, p_distance, p_user_id) AS fn; 
	
END;
$BODY$;
