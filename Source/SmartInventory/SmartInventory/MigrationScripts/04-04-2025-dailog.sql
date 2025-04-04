
-----------------------------------Create table-----------------------------------

CREATE TABLE IF NOT EXISTS public.top_ring_site_mapping
(
    id SERIAL PRIMARY KEY,
    ring_id integer,
    site_id character varying COLLATE pg_catalog."default"
);

-----------------------------------MModify existing function-----------------------------------

DROP FUNCTION IF EXISTS public.fn_insert_top_segment_ring_cable_mapping(integer, integer, integer, integer, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_insert_top_segment_ring_cable_mapping(
	p_agg1_system_id integer,
	p_agg2_system_id integer,
	p_user_id integer,
	p_ring_id integer,
	p_segment_id integer,
	p_top_type character varying,
	p_system_id integer)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    rec RECORD;
BEGIN
    -- Use a cursor to loop through results from the function
    FOR rec IN 
        SELECT network_id 
        FROM fn_topology_get_segment_cables(p_agg1_system_id, p_agg2_system_id, p_user_id)
    LOOP
	IF p_top_type = 'Ring' THEN
        INSERT INTO top_ring_cable_mapping (ring_id, cable_id) 
        VALUES (p_ring_id, rec.network_id);
		end if;

        INSERT INTO top_segment_cable_mapping (segment_id, cable_id) 
        VALUES (p_segment_id, rec.network_id);
		
		
    END LOOP;
	
insert into top_ring_site_mapping(ring_id,site_id)select p_ring_id,(select network_id from att_details_pod adp  where system_id=p_system_id);
    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN
        RAISE WARNING 'Error in function: %', SQLERRM;
        RETURN FALSE;
END;
$BODY$;

-----------------------------------Create new function for show on map -----------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_ring_connectedelement_details(
	p_site_id integer,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
DECLARE 
v_Query text;
p_site1_geom character varying;
	v_geometry_with_buffer geometry;
	 p_site2_geom character varying;
	 startroute integer default 1;
	 geom_text TEXT;
Query text; 
rec record;

BEGIN
	create temp table temp_connectedElement(
		connected_system_id integer, 
		connected_network_id character varying(100), 
		connected_entity_type character varying(100),
		connected_entity_geom character varying ,
		is_virtual boolean,
		is_agg_site boolean
	)on commit drop;
	
	 create temp table temp_cableInfo(
		
	        cable_system_id integer,
	        cable_network_id character varying(100),
	        cable_type character varying(100),
	        cable_geom character varying
	       
	)on commit drop;
	
 -- Create a temporary table to store site geometries
    CREATE TEMP TABLE temp_site_geom_route( 
        site_id integer, 
        site_name character varying, 
        geom geometry,
		ringid integer,
        user_id integer,
		 cbl_distance numeric
    ) ON COMMIT DROP; 
	


Raise info 'p_site_id ->%',p_site_id;
    -- Get the reference site's geometry
    SELECT longitude || ' ' || latitude AS geom
    INTO p_site1_geom
    FROM att_details_pod  
    WHERE system_id = p_site_id;

Raise info 'p_site1_geom ->%',p_site1_geom;
v_geometry_with_buffer = ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326) ;

Raise info 'v_geometry_with_buffer ->%',v_geometry_with_buffer;


	
 -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom_route(site_id, site_name, geom,ringid, user_id,cbl_distance)
     select distinct sitedetails.system_id , sitedetails.site_name , ST_GeomFromText('POINT(' || sitedetails.longitude || ' ' || sitedetails.latitude || ')', 4326),sitedetails.ring_id,p_user_id,sitedetails.length_meters 
	 from  (select pod.system_id,pod.site_name,pod.longitude,pod.latitude,pod.ring_id,0 AS length_meters  
			from att_details_pod pod
    inner join top_ring_site_mapping trs on trs.site_id = pod.network_id
	where trs.ring_id = p_site_id) sitedetails;
	
Raise info 'temp_cable_routes row ->%',(select count(1) from temp_cable_routes);

insert into temp_cableInfo(cable_system_id,cable_network_id,cable_type,cable_geom)
select distinct lm.system_id,cbl.network_id,lm.entity_type,st_astext(lm.sp_geometry) as connected_entity_geom 
from top_ring_cable_mapping trcm
inner join att_details_cable cbl on cbl.network_id=trcm.cable_id
		inner join line_master lm ON  upper(lm.entity_type)='CABLE' and cbl.system_id=lm.system_id
		where trcm.ring_id = p_site_id;

Raise info 'Temp table row ->%',(select count(1) from temp_site_geom);
Raise info 'temp_cableInfo row ->%',(select count(1) from temp_cableInfo);
		
insert into temp_connectedElement(connected_system_id,connected_network_id,connected_entity_type,connected_entity_geom,is_virtual,is_agg_site)
		select pm.system_id,pod.network_id,pm.entity_type,st_astext(pm.sp_geometry) as connected_entity_geom,pm.is_virtual,pod.is_agg_site from temp_site_geom_route ci
		inner join att_details_pod pod on pod.system_id=ci.site_id
		inner join point_master pm ON  upper(pm.entity_type)='POD' and pod.system_id=pm.system_id;
		
Raise info 'temp_connectedElement row ->%',(select count(1) from temp_connectedElement);

   
	RETURN QUERY select row_to_json(result) 
	from (
		select (
			select array_to_json(array_agg(row_to_json(x)))
			from (
				select * from temp_cableInfo
				
			) x
		) as lstCableInfo,
		(
			select array_to_json(array_agg(row_to_json(x)))
			from (
				select distinct connected_system_id,connected_network_id,connected_entity_type
				 ,connected_entity_geom ,is_virtual,COALESCE(is_agg_site, false) AS is_agg_site from temp_connectedElement temp
				inner join layer_details ld on upper(temp.connected_entity_type)=upper(ld.layer_name)
				
			) x
		) as lstConnectedElements
	)result;
 

END 
$BODY$;

-----------------------------------Create new function for segment list -----------------------------------

CREATE OR REPLACE FUNCTION public.fn_topology_get_segmentdetailssitewise(
	p_site_id integer,
	p_user_id integer)
    RETURNS TABLE(id integer, segment_code character varying, region_id integer, agg1_site_id character varying, agg2_site_id character varying) 
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
        user_id integer
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

 RAISE INFO 'p_site_id -> %', p_site_id;
    RAISE INFO 'p_site1_geom -> %', p_site1_geom;

    v_geometry_with_buffer := ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326);

    RAISE INFO 'v_geometry_with_buffer -> %', v_geometry_with_buffer;

   FOR rec IN (SELECT * FROM temp_aggsite) LOOP
       p_site2_geom :='';
	   p_site2_geom =rec.geom;
	   
	
    RAISE INFO 'p_site2_geom -> %', p_site2_geom;
  

     -- Populate temporary cable routes table
      INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id)
		   
	  SELECT seq, path_seq, edge,p_user_id FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
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

    -- Prepare SQL for execution
    sql := 'SELECT distinct s.id, s.segment_code, s.region_id, s.agg1_site_id, s.agg2_site_id 
            FROM top_segment_cable_mapping ts
            INNER JOIN top_segment s ON s.id = ts.segment_id
            inner join att_details_cable cbl on cbl.network_id =ts.cable_id
            INNER JOIN temp_cable_routes tc ON tc.edge_targetid = cbl.system_id
            ';

    -- Execute the SQL and return the result
    RETURN QUERY EXECUTE sql;

END;
$BODY$;