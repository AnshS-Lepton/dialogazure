

---------------------create a new function to get the topology of the network---------------------

CREATE OR REPLACE FUNCTION public.fn_get_agg_sitedetail_routewise(
	p_site_id integer,
	p_user_id integer,
	p_site_name character varying)
    RETURNS TABLE(system_id integer, network_id character varying, site_id character varying, site_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
	DECLARE 
   p_site1_geom character varying;
    p_site2_geom character varying;
	  rec RECORD;

BEGIN
 
create temp table temp_aggsite(
system_id integer ,
	network_id character varying,
	geom character varying
)ON COMMIT DROP;

 --- Create temporary table
	 CREATE temp TABLE temp_cable_routes(
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
		 agg_site_id integer
    ) ON COMMIT DROP;
   
    -- get site details from the p_agg1_site_id   
     SELECT longitude || ' ' || latitude AS geom
     INTO p_site1_geom
     FROM att_details_pod pd 
     WHERE pd.system_id  = p_site_id;   
	 
	  -- Get  site details From the p_agg2_site_id
	 
	 insert into temp_aggsite(system_id,network_id,geom)
     SELECT p.system_id,p.network_id, p.longitude || ' ' || p.latitude AS geom
     FROM att_details_pod p
     WHERE p.system_id <> p_site_id and p.is_agg_site ='true';
	 
	 FOR rec IN (SELECT * FROM temp_aggsite) LOOP
       p_site2_geom :='';
	   p_site2_geom =rec.geom;
	   
	   Raise info 'p_site2_geom ->%',p_site2_geom;
	    -- Populate temporary cable routes table
      INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id,agg_site_id)
		   
	  SELECT seq, path_seq, edge,p_user_id,rec.system_id FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	  FROM routing_data_core_plan', (SELECT id
	  FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), 2))
	  limit 1 ),(SELECT id FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, 
	  ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_site2_geom ||')', 4326), 2)) 
	  limit 1));
       
    END LOOP;
	
	
	
    delete from temp_cable_routes where edge_targetid = -1 and user_id = p_user_id;

    return QUERY select distinct sitedetails.system_id ,sitedetails.network_id,sitedetails.site_id , sitedetails.site_name from  (select pod.system_id,pod.network_id,pod.site_id,pod.site_name  from att_details_pod pod
    inner join temp_cable_routes tcr on tcr.agg_site_id = pod.system_id 
    where user_id = p_user_id) sitedetails where sitedetails.site_name ilike'%'||p_site_name||'%';

END;
	
	
$BODY$;

------------------------------- modify the existing function to get the topology of the network---------------------

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
    p_site1_geom character varying;
	v_geometry_with_buffer geometry;
	sql TEXT;
	 geom_text TEXT;
	 p_site2_geom character varying;
	  rec RECORD;
	  startroute integer default 1;

BEGIN

    -- Create a temporary table to store site geometries
    CREATE TEMP TABLE temp_site_geom( 
        site_id integer, 
        site_name character varying, 
        geom geometry,
		ringid integer,
        user_id integer
    ) ON COMMIT DROP;
	
	--truncate table temp_site_geom_route;
	
 -- Create a temporary table to store site geometries
    CREATE TEMP TABLE temp_site_geom_route( 
        site_id integer, 
        site_name character varying, 
        geom geometry,
		ringid integer,
        user_id integer,
		 cbl_distance numeric
    ) ON COMMIT DROP; 
	
	
	--truncate table temp_cable_routes;
	
	--- Create temporary table
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
		 agg_site_id integer,
		  routenumber integer
    ) ON COMMIT DROP; 
   
	
	create temp table temp_aggsite(
system_id integer ,
	network_id character varying,
	geom character varying
)ON COMMIT DROP;

    -- Get the reference site's geometry
    SELECT longitude || ' ' || latitude AS geom
    INTO p_site1_geom
    FROM att_details_pod  
    WHERE system_id = p_site_id;

Raise info 'p_site1_geom ->%',p_site1_geom;
v_geometry_with_buffer = ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326) ;

Raise info 'v_geometry_with_buffer ->%',v_geometry_with_buffer;

 -- Get  site details From the p_agg2_site_id
	 
	 insert into temp_aggsite(system_id,network_id,geom)
     SELECT p.system_id,p.common_name, p.longitude || ' ' || p.latitude 
     FROM  point_master p
     WHERE p.system_id <> p_site_id and st_within(p.sp_geometry,st_buffer_meters(v_geometry_with_buffer,p_distance))
	 and p.entity_type='POD'; --and p.is_agg_site ='true';
	 
	 FOR rec IN (SELECT * FROM temp_aggsite) LOOP
       p_site2_geom :='';
	   p_site2_geom =rec.geom;
	   
	   Raise info 'p_site2_geom ->%',p_site2_geom;
	     Raise info 'startroute ->%',startroute;
	    -- Populate temporary cable routes table
      INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id,agg_site_id,routenumber)
		   
	  SELECT seq, path_seq, edge,p_user_id,rec.system_id,startroute FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	  FROM routing_data_core_plan', (SELECT id
	  FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), 2))
	  limit 1 ),(SELECT id FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, 
	  ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_site2_geom ||')', 4326), 2)) 
	  limit 1));
       
	    startroute := startroute + 1;
    END LOOP;
	
	
	
    delete from temp_cable_routes where edge_targetid = -1 and user_id = p_user_id;

 -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom_route(site_id, site_name, geom,ringid, user_id,cbl_distance)
     select distinct sitedetails.system_id , sitedetails.site_name , ST_GeomFromText('POINT(' || sitedetails.longitude || ' ' || sitedetails.latitude || ')', 4326),sitedetails.ring_id,p_user_id,sitedetails.length_meters  from  (select pod.system_id,pod.site_name,pod.longitude,pod.latitude,pod.ring_id,ST_Length(ST_Transform(lm.sp_geometry, 3857)) AS length_meters  from att_details_pod pod
    inner join temp_cable_routes tcr on tcr.agg_site_id = pod.system_id
	inner join line_master lm on lm.system_id=tcr.edge_targetid and entity_type='Cable'
	where user_id = p_user_id) sitedetails where sitedetails.system_id <>p_site_id;
	
Raise info 'temp_cable_routes row ->%',(select count(1) from temp_cable_routes);

    -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom(site_id, site_name, geom,ringid, user_id)
    SELECT distinct
        p.site_id, 
       CASE 
        WHEN tr.ring_code IS NULL OR TRIM(tr.ring_code) = '' THEN COALESCE(p.site_name, '')  
        ELSE CONCAT(p.site_name, ' (', tr.ring_code, ')') 
    END  AS site_name,    
        p.geom,
		p.ringid,
        p_user_id   
	FROM  temp_site_geom_route p
	left JOIN 
	top_ring tr ON tr.id = p.ringid
   WHERE  p.cbl_distance <= p_distance
 
;
   

Raise info 'Temp table row ->%',(select count(1) from temp_site_geom);

 geom_text := ST_AsText(v_geometry_with_buffer);
 
 
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

ALTER FUNCTION public.fn_topology_get_sites(integer, integer, integer, integer)
    OWNER TO postgres;


    ------------------------