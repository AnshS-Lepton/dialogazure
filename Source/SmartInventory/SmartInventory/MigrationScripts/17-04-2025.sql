
-----------------------------------------Drop and Add column with new data type---------------------------------------------
DROP VIEW public.vw_topology_plan_details;

ALTER TABLE top_segment DROP COLUMN agg1_site_id;
ALTER TABLE top_segment DROP COLUMN agg2_site_id;

ALTER TABLE top_segment add COLUMN agg1_site_id integer;
ALTER TABLE top_segment add COLUMN agg2_site_id integer;

ALTER TABLE top_segment_cable_mapping DROP COLUMN cable_id;
ALTER TABLE top_segment_cable_mapping add COLUMN cable_id integer;

ALTER TABLE top_ring_cable_mapping DROP COLUMN cable_id;
ALTER TABLE top_ring_cable_mapping add COLUMN cable_id integer;

ALTER TABLE top_ring_site_mapping DROP COLUMN site_id;
ALTER TABLE top_ring_site_mapping add COLUMN site_id integer;

---------------------------------------------Create view ------------------------------------------------------------------------

CREATE OR REPLACE VIEW public.vw_topology_plan_details
 AS
 SELECT tr.id,
    ts.segment_code,
    tr.ring_code,
    att.site_id,
    att.site_name,
    tr.ring_capacity,
    ( SELECT CONCAT(site_id,' (', site_name,')')::varchar
           FROM att_details_pod
          WHERE att_details_pod.system_id = ts.agg1_site_id) as agg1_site_id,
   (SELECT CONCAT(site_id,' (', site_name,')')::varchar
           FROM att_details_pod
          WHERE att_details_pod.system_id = ts.agg2_site_id) as agg2_site_id,
    trr.region_name,
    tr.description,
    att.bh_status,
    att.ring_a_site_distance,
    att.ring_b_site_distance,
    ( SELECT att_details_pod.site_id
           FROM att_details_pod
          WHERE att_details_pod.system_id = att.ring_a_site_id) AS peer1,
    ( SELECT att_details_pod.site_id
           FROM att_details_pod
          WHERE att_details_pod.system_id = att.ring_b_site_id) AS peer2,
    att.is_agg_site
   FROM top_ring tr
     JOIN top_segment ts ON ts.id = tr.segment_id
     JOIN top_region trr ON trr.id = ts.region_id
     LEFT JOIN att_details_pod att ON tr.id = att.ring_id;




---------------------------------Modify view to add is_agg_site column----------------------------------------
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

RAISE INFO 'temp_aggsite row count -> %', (SELECT COUNT(1) FROM temp_aggsite);
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


RETURN QUERY

    SELECT distinct s.id, s.segment_code, s.region_id, CONCAT(adp.site_id,' (', adp.site_name, ')')::varchar AS agg1_site_id,CONCAT(adp2.site_id, ' (', adp2.site_name, ')')::varchar AS agg2_site_id,adp.system_id as agg_01,adp2.system_id as agg_02 
            FROM top_segment_cable_mapping ts
            left JOIN top_segment s ON s.id = ts.segment_id
            left join att_details_cable cbl on cbl.system_id =ts.cable_id
			left join att_details_pod adp on adp.system_id= s.agg1_site_id
            left join att_details_pod adp2 on adp2.system_id = s.agg2_site_id
            INNER JOIN temp_cable_routes tc ON tc.edge_targetid = cbl.system_id;
           

END;
$BODY$;



---------------------------------Modify fn_topology_get_sitedetails----------------------------------------

DROP FUNCTION IF EXISTS public.fn_topology_get_sitedetails(integer, integer);

-- FUNCTION: public.fn_topology_get_sitedetails(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_topology_get_sitedetails(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_sitedetails(
	p_site_id integer,
	p_user_id integer)
    RETURNS TABLE(site_id character varying, site_name character varying, top_type character varying, ring_capacity character varying, segment_code character varying, agg1_site_id character varying, agg2_site_id character varying, region_name character varying, ring_code character varying, ring_id integer, no_of_sites integer, max_distance_peer integer, ring_a_site_id integer, ring_b_site_id integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
   

BEGIN

RETURN QUERY

select adp.site_id,adp.site_name,adp.top_type,tr.ring_capacity,ts.segment_code, CONCAT(adp2.site_id,' (', adp2.site_name,')')::varchar as agg1_site_id ,CONCAT(adp3.site_id,' (', adp3.site_name,')')::varchar as agg2_site_id,tr2.region_name,tr.ring_code,adp.ring_id,
COALESCE(adp.no_of_sites, 0) AS no_of_sites,COALESCE(adp.max_distance_peer, 0) AS max_distance_peer,adp.ring_a_site_id,adp.ring_b_site_id 
            from top_ring_site_mapping sm 
            inner join att_details_pod adp on adp.system_id=sm.site_id 
            inner join top_ring tr on tr.id=adp.ring_id
             inner join top_segment ts on ts.id=tr.segment_id 
			 inner join att_details_pod adp2 on adp2.system_id=ts.agg1_site_id 
			 inner join att_details_pod adp3 on adp3.system_id=ts.agg2_site_id 
            inner join top_region tr2 on tr2.id=ts.region_id
            where adp.system_id=p_site_id order by sm.id  desc limit 1;
            
            

    

END;
$BODY$;

ALTER FUNCTION public.fn_topology_get_sitedetails(integer, integer)
    OWNER TO postgres;

---------------------------------Modify fn_insert_top_segment_ring_cable_mapping----------------------------------------


DROP FUNCTION IF EXISTS public.fn_insert_top_segment_ring_cable_mapping(integer, integer, integer, integer, integer, character varying, integer);

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
        FROM fn_topology_get_segment_cables(p_agg1_system_id, p_agg2_system_id,p_system_id, p_user_id)
    LOOP
	IF p_top_type = 'Ring' THEN
       /* INSERT INTO top_ring_cable_mapping (ring_id, cable_id) 
        VALUES (p_ring_id, rec.network_id);*/
		IF (SELECT COUNT(1) FROM top_ring_cable_mapping WHERE cable_id = (select system_id from att_details_cable where network_id=rec.network_id)) = 0 THEN
    INSERT INTO top_ring_cable_mapping (ring_id, cable_id)
    VALUES (p_ring_id, (select system_id from att_details_cable where network_id=rec.network_id));
ELSE
    UPDATE top_ring_cable_mapping
    SET ring_id = p_ring_id
    WHERE cable_id = (select system_id from att_details_cable where network_id=rec.network_id);
END IF;

		
		end if;

IF (SELECT COUNT(1) FROM top_segment_cable_mapping WHERE cable_id = (select system_id from att_details_cable where network_id=rec.network_id)) = 0 THEN
        INSERT INTO top_segment_cable_mapping (segment_id, cable_id) 
        VALUES (p_segment_id, (select system_id from att_details_cable where network_id=rec.network_id));
		ELSE
UPDATE top_segment_cable_mapping
    SET segment_id = p_segment_id
    WHERE cable_id = (select system_id from att_details_cable where network_id=rec.network_id);
	END IF;	
		
    END LOOP;
	
insert into top_ring_site_mapping(ring_id,site_id)select p_ring_id,p_system_id;
update att_details_pod set ring_id=p_ring_id where system_id IN (p_agg1_system_id, p_agg2_system_id);
IF (
    SELECT COUNT(1)
    FROM top_ring_site_mapping
    WHERE ring_id = p_ring_id
      AND site_id IN (
           p_agg1_system_id, p_agg2_system_id
      )
) = 0 THEN
    INSERT INTO top_ring_site_mapping (ring_id, site_id)
    VALUES  (p_ring_id, p_agg1_system_id),(p_ring_id, p_agg2_system_id);
ELSE
    UPDATE top_ring_site_mapping
    SET ring_id = p_ring_id
    WHERE site_id IN (
         p_agg1_system_id, p_agg2_system_id
    );
END IF;

	
    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN
        RAISE WARNING 'Error in function: %', SQLERRM;
        RETURN FALSE;
END;
$BODY$;




---------------------------------Modify fn_get_ring_assocication_details----------------------------------------


DROP FUNCTION IF EXISTS public.fn_get_ring_assocication_details(boolean, character varying, character varying, character varying, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_ring_assocication_details(
	filter_data_flag boolean,
	p_region_code character varying,
	p_segement_code character varying,
	p_ring_id character varying,
	p_user_id integer,
	cable_id character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    sql_query TEXT;
    p_network_id integer;
BEGIN
    -- Fetch network_id safely
   -- SELECT network_id INTO p_network_id FROM att_details_cable WHERE system_id = cable_id::int;
   
    SELECT cable_id::int INTO p_network_id;
    
	 RAISE INFO 'p_network_id SQL: %', p_network_id;
	 
    -- Base query with parameterized placeholders
    sql_query := 'SELECT row_to_json(row) FROM ( 
                  SELECT 
                      tr.ring_code, 
                      tre.region_name AS region_code, 
                      ts.segment_code, 
                      tr.ring_capacity AS ring_capacity,
                      tr.id as ring_id  
                  FROM top_ring_cable_mapping trcm
                  INNER JOIN top_segment_cable_mapping tscm ON tscm.cable_id = trcm.cable_id 
                  INNER JOIN top_segment ts ON ts.id = tscm.segment_id
                  INNER JOIN top_ring tr ON tr.id = trcm.ring_id 
                  INNER JOIN top_region tre ON tre.id = ts.region_id
                  WHERE trcm.cable_id = $1';

    -- Apply filtering if flag is TRUE
    IF filter_data_flag THEN
        IF p_region_code IS NOT NULL AND p_region_code <> '' THEN
            sql_query := sql_query || ' AND tre.region_name = $2';
        END IF;

        IF p_segement_code IS NOT NULL AND p_segement_code <> '' THEN
            sql_query := sql_query || ' AND ts.segment_code = $3';
        END IF;

        IF p_ring_id IS NOT NULL AND p_ring_id <> '' THEN
            sql_query := sql_query || ' AND tr.ring_code = $4';
        END IF;
    END IF;

    -- Close the query properly
    sql_query := sql_query || ') row';

    -- Debugging output
    RAISE INFO 'Executing SQL: %', sql_query;

    -- Execute dynamic SQL safely
    RETURN QUERY EXECUTE sql_query USING p_network_id, p_region_code, p_segement_code, p_ring_id;

END;
$BODY$;

---------------------------------Modify fn_get_ring_connectedelement_details----------------------------------------


DROP FUNCTION IF EXISTS public.fn_get_ring_connectedelement_details(integer, integer);

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
    inner join top_ring_site_mapping trs on trs.site_id = pod.system_id
	where trs.ring_id = p_site_id) sitedetails;
	
Raise info 'temp_cable_routes row ->%',(select count(1) from temp_cable_routes);

insert into temp_cableInfo(cable_system_id,cable_network_id,cable_type,cable_geom)
select distinct lm.system_id,cbl.network_id,lm.entity_type,st_astext(lm.sp_geometry) as connected_entity_geom 
from top_ring_cable_mapping trcm
inner join att_details_cable cbl on cbl.system_id=trcm.cable_id
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


---------------------------------Modify fn_splicing_save_osp_split_connection----------------------------------------



 DROP FUNCTION IF EXISTS public.fn_splicing_save_osp_split_connection(integer, integer, integer, integer, character varying, character varying, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_splicing_save_osp_split_connection(
	p_cable_one_system__id integer,
	p_cable_two_system_id integer,
	p_old_cable_system_id integer,
	p_split_entity_system_id integer,
	p_split_entity_network_id character varying,
	p_split_entity_type character varying,
	p_user_id integer,
	p_splicing_source character varying)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

declare 
  arow record; 
  v_cable_one_system_id integer;  
  v_is_cable_one_a_end boolean;  
  v_cable_one_total_core integer;
  v_cable_two_system_id integer;
  v_is_cable_two_a_end boolean;
  v_connection_id integer; 
  v_counter integer := 0;
  v_split_entity_port_no integer;  
  v_split_entity_longlat character varying;
  v_layer_table character varying;
  v_is_virtual_port_allowed boolean;
  V_END_POINT_GEOM GEOMETRY;
v_buffer integer;
V_CABLE_ONE_END RECORD;
V_CABLE_TOW_END RECORD;
p_cable_one_network_id character varying;
p_cable_two_network_id character varying;

begin
V_CABLE_ONE_END:=NULL;
V_CABLE_TOW_END:=NULL;
v_buffer:=0;
  v_split_entity_port_no:=1;
  v_counter:=0; 
  v_is_virtual_port_allowed:=false;	
select value::integer into v_buffer from global_settings where key='SplitCableBuffer';
select is_virtual_port_allowed into v_is_virtual_port_allowed from layer_details where upper(layer_name)=upper(p_split_entity_type);
 --if(v_is_virtual_port_allowed=true)
 --then
 CREATE temp TABLE TEMP_CONNECTIONS
 (
	ID SERIAL,
	SOURCE_SYSTEM_ID INTEGER,
	SOURCE_NETWORK_ID CHARACTER VARYING(100),
	SOURCE_ENTITY_TYPE CHARACTER VARYING(100),
	SOURCE_PORT_NO INTEGER,
	DESTINATION_SYSTEM_ID INTEGER,
	DESTINATION_NETWORK_ID CHARACTER VARYING(100),
	DESTINATION_ENTITY_TYPE CHARACTER VARYING(100),
	DESTINATION_PORT_NO INTEGER,
	IS_CUSTOMER_CONNECTED BOOLEAN DEFAULT FALSE,
	CREATED_ON TIMESTAMP WITHOUT TIME ZONE,
	CREATED_BY INTEGER,
	APPROVED_BY INTEGER,
	APPROVED_ON TIMESTAMP WITHOUT TIME ZONE,
	SPLICING_SOURCE CHARACTER VARYING,
	is_cable_a_end boolean default false,
	is_reorder_required boolean default false,
	source_entity_sub_type character varying,
	destination_entity_sub_type character varying,
	destination_tray_system_id integer default 0,
	source_tray_system_id integer default 0,
	is_through_connection boolean default false
)on commit drop ; 

 CREATE temp TABLE TEMP_ALL_CONNECTIONS
 (
	ID SERIAL,
	SOURCE_SYSTEM_ID INTEGER,
	SOURCE_NETWORK_ID CHARACTER VARYING(100),
	SOURCE_ENTITY_TYPE CHARACTER VARYING(100),
	SOURCE_PORT_NO INTEGER,
	DESTINATION_SYSTEM_ID INTEGER,
	DESTINATION_NETWORK_ID CHARACTER VARYING(100),
	DESTINATION_ENTITY_TYPE CHARACTER VARYING(100),
	DESTINATION_PORT_NO INTEGER,
	IS_CUSTOMER_CONNECTED BOOLEAN DEFAULT FALSE,
	CREATED_ON TIMESTAMP WITHOUT TIME ZONE,
	CREATED_BY INTEGER,
	APPROVED_BY INTEGER,
	APPROVED_ON TIMESTAMP WITHOUT TIME ZONE,
	SPLICING_SOURCE CHARACTER VARYING,
	is_cable_a_end boolean default false,
	is_reorder_required boolean default false,
	source_entity_sub_type character varying,
	destination_entity_sub_type character varying,
	destination_tray_system_id integer default 0,
	source_tray_system_id integer default 0,
	is_through_connection boolean default false
)on commit drop ; 

	--GET CABLE ONE DETAILS
	--select system_id,total_core into v_cable_one_system_id,v_cable_one_total_core from att_details_cable where network_id=p_cable_one_network_id ;  
select system_id,total_core,network_id into v_cable_one_system_id,v_cable_one_total_core,p_cable_one_network_id from att_details_cable where system_id=p_cable_one_system__id ; 
	--GET CABLE TWO DETAILS
	--select system_id INTO v_cable_two_system_id from att_details_cable where network_id=p_cable_two_network_id;
select system_id,network_id INTO v_cable_two_system_id,p_cable_two_network_id from att_details_cable where system_id=p_cable_two_system_id;
	--GET  SPLIT ENTITY DETAILS
	select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_split_entity_type);
	EXECUTE 'select longitude||'' ''||latitude from '||v_layer_table||' where system_id='||p_split_entity_system_id||'' into v_split_entity_longlat;

	--CHECK THE CABLE ONE LEFT A END OR B END NEAR SPLIT ENTITY BUFFER
	SELECT  case when ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer))=true then true else false end into v_is_cable_one_a_end
	from line_master lm where lm.system_id=v_cable_one_system_id  and upper(lm.entity_type)='CABLE' and lm.network_status !='D'  
	and (ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer))
	or
	ST_WITHIN(ST_EndPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer)));

	

	--CHECK THE CABLE TWO LEFT A END OR B END NEAR SPLIT ENTITY BUFFER
	SELECT  case when ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer))=true then true else false end into v_is_cable_two_a_end
	from line_master lm where lm.system_id=v_cable_two_system_id  and upper(lm.entity_type)='CABLE' and lm.network_status !='D'  
	and (ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer))
	or
	ST_WITHIN(ST_EndPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer)));

raise info 'v_is_cable_one_a_end%',v_is_cable_one_a_end;
raise info 'v_is_cable_two_a_end%',v_is_cable_two_a_end;

	-- REPLACE OLD CABLE LEFT ENTRY WITH NEW CABLE 1 ON LEFT SIDE..

	insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type, 
        source_port_no, destination_system_id, destination_network_id, 
        destination_entity_type, destination_port_no,is_cable_a_end,IS_CUSTOMER_CONNECTED,SPLICING_SOURCE,is_through_connection)

        SELECT  source_system_id, source_network_id,source_entity_type, 
        source_port_no,v_cable_one_system_id,p_cable_one_network_id, 
        destination_entity_type ,destination_port_no,(case when v_is_cable_one_a_end=true then false else true end),IS_CUSTOMER_CONNECTED,'OSP SPLIT',true  
	from connection_info where destination_system_id =p_old_cable_system_id and Upper(destination_entity_type)='CABLE' and is_cable_a_end=(case when v_is_cable_one_a_end=true then false else true end)
	UNION
        SELECT v_cable_one_system_id,p_cable_one_network_id,source_entity_type,source_port_no,
        destination_system_id,destination_network_id,destination_entity_type,destination_port_no,(case when v_is_cable_one_a_end=true then false else true end),IS_CUSTOMER_CONNECTED,'OSP SPLIT',true    
	from connection_info where source_system_id =p_old_cable_system_id and Upper(source_entity_type)='CABLE' and is_cable_a_end=(case when v_is_cable_one_a_end=true then false else true end);

	--NEW CABLE TWO TO OLD ENTITY WHICH IS ON RIGHT HAND SIDE 
	insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type, 
        source_port_no, destination_system_id, destination_network_id, 
        destination_entity_type, destination_port_no,is_cable_a_end,IS_CUSTOMER_CONNECTED,SPLICING_SOURCE,is_through_connection)      
        SELECT  source_system_id, source_network_id,source_entity_type, 
        source_port_no,v_cable_two_system_id,p_cable_two_network_id, 
        destination_entity_type ,destination_port_no,(case when v_is_cable_two_a_end=true then false else true end),IS_CUSTOMER_CONNECTED ,'OSP SPLIT',true   
	from connection_info where destination_system_id =p_old_cable_system_id and Upper(destination_entity_type)='CABLE' and is_cable_a_end=(case when v_is_cable_two_a_end=true then false else true end)	
	UNION
        SELECT v_cable_two_system_id,p_cable_two_network_id,source_entity_type,source_port_no,
        destination_system_id,destination_network_id,destination_entity_type,destination_port_no,(case when v_is_cable_two_a_end=true then false else true end),IS_CUSTOMER_CONNECTED ,'OSP SPLIT',true    
	from connection_info where source_system_id =p_old_cable_system_id and Upper(source_entity_type)='CABLE' and is_cable_a_end=(case when v_is_cable_two_a_end=true then false else true end);

	-- DELETE OLD CABLE CONNECTIONS
        delete from connection_info where (source_system_id=p_old_cable_system_id and upper(source_entity_type)=upper('CABLE')) 
        or (destination_system_id=p_old_cable_system_id and upper(destination_entity_type)=upper('CABLE'));

	perform(fn_splicing_insert_into_connection_info());
	
		insert into TEMP_ALL_CONNECTIONS(source_system_id, source_network_id, source_entity_type, 
        source_port_no, destination_system_id, destination_network_id, 
        destination_entity_type, destination_port_no,is_cable_a_end,IS_CUSTOMER_CONNECTED,SPLICING_SOURCE,is_through_connection)
		select source_system_id, source_network_id, source_entity_type, 
        source_port_no, destination_system_id, destination_network_id, 
        destination_entity_type, destination_port_no,is_cable_a_end,IS_CUSTOMER_CONNECTED,SPLICING_SOURCE,is_through_connection from TEMP_CONNECTIONS;
		
	truncate table TEMP_CONNECTIONS;

	if exists(select 1 from connection_info where destination_system_id=p_split_entity_system_id and upper(destination_entity_type)=upper(p_split_entity_type))
	then
			select (max(coalesce(destination_port_no))) into v_split_entity_port_no from connection_info where destination_system_id=p_split_entity_system_id and upper(destination_entity_type)=upper(p_split_entity_type);	
	
			--select (max(coalesce(destination_port_no))+1) into v_split_entity_port_no from TEMP_CONNECTIONS where destination_system_id=p_split_entity_system_id and upper(destination_entity_type)=upper(p_split_entity_type);
				
	end if;	
		
	if(v_is_virtual_port_allowed=true)
	then
	 LOOP 
	 EXIT WHEN v_cable_one_total_core <= v_counter  ; 
		v_counter := v_counter + 1 ; 
		
		v_split_entity_port_no:=v_split_entity_port_no+1;
		
		v_split_entity_port_no=coalesce(v_split_entity_port_no,1);

		if((select is_virtual_port_allowed from layer_details where upper(layer_name)=upper(p_split_entity_type))=false and (select count(1) from connection_info where source_system_id=p_split_entity_system_id and upper(source_entity_type)=                upper(p_split_entity_type) and source_port_no=(-1*v_counter) and destination_system_id=p_split_entity_system_id and upper(destination_entity_type)=upper(p_split_entity_type) 
		and destination_port_no=v_counter)=0)
		then	truncate table TEMP_CONNECTIONS;
			-- split entity internal connection      
			insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type,source_port_no, destination_system_id, destination_network_id,destination_entity_type, destination_port_no,is_cable_a_end,SPLICING_SOURCE,is_through_connection)
			values(p_split_entity_system_id,p_split_entity_network_id,p_split_entity_type,-1*v_counter,p_split_entity_system_id,p_split_entity_network_id,p_split_entity_type,v_counter,false,'OSP SPLIT',true);
			perform(fn_splicing_insert_into_connection_info());	
			
			insert into TEMP_ALL_CONNECTIONS(source_system_id, source_network_id, source_entity_type, 
			source_port_no, destination_system_id, destination_network_id, 
			destination_entity_type, destination_port_no,is_cable_a_end,IS_CUSTOMER_CONNECTED,SPLICING_SOURCE,is_through_connection)
			select source_system_id, source_network_id, source_entity_type, 
        source_port_no, destination_system_id, destination_network_id, 
        destination_entity_type, destination_port_no,is_cable_a_end,IS_CUSTOMER_CONNECTED,SPLICING_SOURCE,is_through_connection from TEMP_CONNECTIONS;
		
		end if;

		truncate table TEMP_CONNECTIONS;
		-- CABLE ONE TO SPLIT ENTITY
		insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type,source_port_no, 
		destination_system_id, destination_network_id,destination_entity_type, destination_port_no,is_cable_a_end,SPLICING_SOURCE,is_through_connection)
		values(v_cable_one_system_id,p_cable_one_network_id,'Cable',v_counter,p_split_entity_system_id,p_split_entity_network_id,p_split_entity_type,v_split_entity_port_no,coalesce(v_is_cable_one_a_end,false),'OSP SPLIT',true);		      	

		--  split entity to  cable two
		insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type,source_port_no,destination_system_id, destination_network_id,destination_entity_type, destination_port_no,is_cable_a_end,SPLICING_SOURCE,is_through_connection)
		values(p_split_entity_system_id,p_split_entity_network_id,p_split_entity_type,v_split_entity_port_no,v_cable_two_system_id,p_cable_two_network_id,'Cable',v_counter ,coalesce(v_is_cable_two_a_end,false),'OSP SPLIT',true);

	    perform(fn_splicing_insert_into_connection_info());	
		
		insert into TEMP_ALL_CONNECTIONS(source_system_id, source_network_id, source_entity_type,source_port_no, 
		destination_system_id, destination_network_id,destination_entity_type, destination_port_no,is_cable_a_end,SPLICING_SOURCE,is_through_connection)
		select source_system_id, source_network_id, source_entity_type,source_port_no, 
		destination_system_id, destination_network_id,destination_entity_type, destination_port_no,is_cable_a_end,SPLICING_SOURCE,is_through_connection from TEMP_CONNECTIONS;
	
	 END LOOP ; 
	end if; 

--CHECK THE FIRST CABLE IS OSP CABLE AND OLD CABLE IS LMC ASSOCIATED
IF EXISTS(SELECT 1 FROM ISP_LINE_MASTER WHERE ENTITY_ID=v_cable_one_system_id AND UPPER(ENTITY_TYPE)=UPPER('CABLE') AND UPPER(CABLE_TYPE)=UPPER('OSP'))
AND EXISTS(SELECT 1 FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=p_old_cable_system_id)
THEN
	SELECT CBL.A_SYSTEM_ID,CBL.A_ENTITY_TYPE,CBL.B_SYSTEM_ID,CBL.B_ENTITY_TYPE INTO V_CABLE_ONE_END FROM ATT_DETAILS_CABLE CBL INNER JOIN ISP_ENTITY_MAPPING MAP 
	ON (CBL.A_SYSTEM_ID=MAP.ENTITY_ID AND UPPER(CBL.A_ENTITY_TYPE)=UPPER(MAP.ENTITY_TYPE)) 
	WHERE CBL.SYSTEM_ID=v_cable_one_system_id;
	IF(V_CABLE_ONE_END IS NOT NULL)
	THEN 
		SELECT SP_GEOMETRY INTO V_END_POINT_GEOM FROM POINT_MASTER WHERE SYSTEM_ID=V_CABLE_ONE_END.B_SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_CABLE_ONE_END.B_ENTITY_TYPE);
		
		UPDATE ATT_DETAILS_LMC_CABLE_INFO 
		SET CABLE_SYSTEM_ID=v_cable_one_system_id,
		RTN_BUILDING_SIDE_TAPPING_LATITUDE=ST_Y(V_END_POINT_GEOM),
		RTN_BUILDING_SIDE_TAPPING_LONGITUDE=ST_X(V_END_POINT_GEOM)  
		WHERE CABLE_SYSTEM_ID=p_old_cable_system_id;
	END IF;

	SELECT CBL.A_SYSTEM_ID,CBL.A_ENTITY_TYPE,CBL.B_SYSTEM_ID,CBL.B_ENTITY_TYPE INTO V_CABLE_TOW_END FROM ATT_DETAILS_CABLE CBL INNER JOIN ISP_ENTITY_MAPPING MAP 
	ON (CBL.B_SYSTEM_ID=MAP.ENTITY_ID AND UPPER(CBL.B_ENTITY_TYPE)=UPPER(MAP.ENTITY_TYPE)) 
	WHERE CBL.SYSTEM_ID=v_cable_one_system_id;
	IF(V_CABLE_TOW_END IS NOT NULL)
	THEN 
		SELECT SP_GEOMETRY INTO V_END_POINT_GEOM FROM POINT_MASTER WHERE SYSTEM_ID=V_CABLE_TOW_END.A_SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_CABLE_TOW_END.A_ENTITY_TYPE);
		
		UPDATE ATT_DETAILS_LMC_CABLE_INFO 
		SET CABLE_SYSTEM_ID=v_cable_one_system_id,
		RTN_BUILDING_SIDE_TAPPING_LATITUDE=ST_Y(V_END_POINT_GEOM),
		RTN_BUILDING_SIDE_TAPPING_LONGITUDE=ST_X(V_END_POINT_GEOM) 
		WHERE CABLE_SYSTEM_ID=p_old_cable_system_id;
	END IF;
END IF;

--CHECK THE FIRST CABLE IS OSP CABLE AND OLD CABLE IS LMC ASSOCIATED
IF EXISTS(SELECT 1 FROM ISP_LINE_MASTER WHERE ENTITY_ID=v_cable_two_system_id AND UPPER(ENTITY_TYPE)=UPPER('CABLE') AND UPPER(CABLE_TYPE)=UPPER('OSP'))
AND EXISTS(SELECT 1 FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=p_old_cable_system_id)
THEN
	SELECT CBL.A_SYSTEM_ID,CBL.A_ENTITY_TYPE,CBL.B_SYSTEM_ID,CBL.B_ENTITY_TYPE INTO V_CABLE_ONE_END FROM ATT_DETAILS_CABLE CBL INNER JOIN ISP_ENTITY_MAPPING MAP 
	ON (CBL.A_SYSTEM_ID=MAP.ENTITY_ID AND UPPER(CBL.A_ENTITY_TYPE)=UPPER(MAP.ENTITY_TYPE)) 
	WHERE CBL.SYSTEM_ID=v_cable_two_system_id;
	IF(V_CABLE_ONE_END IS NOT NULL)
	THEN 
		SELECT SP_GEOMETRY INTO V_END_POINT_GEOM FROM POINT_MASTER WHERE SYSTEM_ID=V_CABLE_ONE_END.B_SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_CABLE_ONE_END.B_ENTITY_TYPE);
		
		UPDATE ATT_DETAILS_LMC_CABLE_INFO 
		SET CABLE_SYSTEM_ID=v_cable_two_system_id,
		RTN_BUILDING_SIDE_TAPPING_LATITUDE=ST_Y(V_END_POINT_GEOM),
		RTN_BUILDING_SIDE_TAPPING_LONGITUDE=ST_X(V_END_POINT_GEOM)  
		WHERE CABLE_SYSTEM_ID=p_old_cable_system_id;
	END IF;

	SELECT CBL.A_SYSTEM_ID,CBL.A_ENTITY_TYPE,CBL.B_SYSTEM_ID,CBL.B_ENTITY_TYPE INTO V_CABLE_TOW_END FROM ATT_DETAILS_CABLE CBL INNER JOIN ISP_ENTITY_MAPPING MAP 
	ON (CBL.B_SYSTEM_ID=MAP.ENTITY_ID AND UPPER(CBL.B_ENTITY_TYPE)=UPPER(MAP.ENTITY_TYPE)) 
	WHERE CBL.SYSTEM_ID=v_cable_two_system_id;
	IF(V_CABLE_TOW_END IS NOT NULL)
	THEN 
		SELECT SP_GEOMETRY INTO V_END_POINT_GEOM FROM POINT_MASTER WHERE SYSTEM_ID=V_CABLE_TOW_END.A_SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_CABLE_TOW_END.A_ENTITY_TYPE);
		
		UPDATE ATT_DETAILS_LMC_CABLE_INFO 
		SET CABLE_SYSTEM_ID=v_cable_two_system_id,
		RTN_BUILDING_SIDE_TAPPING_LATITUDE=ST_Y(V_END_POINT_GEOM),
		RTN_BUILDING_SIDE_TAPPING_LONGITUDE=ST_X(V_END_POINT_GEOM) 
		WHERE CABLE_SYSTEM_ID=p_old_cable_system_id;
	END IF;
END IF;

update att_details_cable_info a set link_system_id=b.link_system_id
from att_details_cable_info b where b.cable_id=p_old_cable_system_id and a.fiber_number=b.fiber_number and a.cable_id=p_cable_one_system__id;

update att_details_cable_info a set link_system_id=b.link_system_id
from att_details_cable_info b where b.cable_id=p_old_cable_system_id and a.fiber_number=b.fiber_number and a.cable_id=p_cable_two_system_id;
	
update att_details_cable_info a 
set a_end_status_id=2
from TEMP_ALL_CONNECTIONS b where b.source_system_id=a.cable_id and b.source_entity_type='Cable'
and a.fiber_number=b.source_port_no
and is_cable_a_end=true;

update att_details_cable_info a 
set a_end_status_id=2
from TEMP_ALL_CONNECTIONS b where b.destination_system_id=a.cable_id and b.destination_entity_type='Cable'
and a.fiber_number=b.destination_port_no
and is_cable_a_end=true;

update att_details_cable_info a 
set b_end_status_id=2
from TEMP_ALL_CONNECTIONS b where b.source_system_id=a.cable_id and b.source_entity_type='Cable'
and a.fiber_number=b.source_port_no
and is_cable_a_end=false;

update att_details_cable_info a 
set b_end_status_id=2
from TEMP_ALL_CONNECTIONS b where b.destination_system_id=a.cable_id and b.destination_entity_type='Cable'
and a.fiber_number=b.destination_port_no
and is_cable_a_end=false;


----------------------- For topology process-----------------------
if EXISTS(select cable_id from top_segment_cable_mapping where cable_id=p_old_cable_system_id )
then
INSERT INTO top_segment_cable_mapping (cable_id, segment_id)
SELECT p_cable_one_system__id, segment_id
FROM (
    SELECT segment_id 
    FROM top_segment_cable_mapping 
    WHERE cable_id = p_old_cable_system_id 
    LIMIT 1
) AS sub1
UNION
SELECT p_cable_two_system_id, segment_id
FROM (
    SELECT segment_id 
    FROM top_segment_cable_mapping 
    WHERE cable_id = p_old_cable_system_id 
    LIMIT 1
) AS sub2;


if EXISTS(select cable_id from top_segment_cable_mapping where cable_id=p_cable_one_system__id )
then
 delete from top_segment_cable_mapping where cable_id=p_old_cable_system_id ;
END IF;

END IF;
--end if;
    return true;
end;
$BODY$;

ALTER FUNCTION public.fn_splicing_save_osp_split_connection(integer, integer, integer, integer, character varying, character varying, integer, character varying)
    OWNER TO postgres;


---------------------------------Modify function to avoide aggregate site details ----------------------------------------

DROP FUNCTION IF EXISTS public.fn_get_ring_details(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_ring_details(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_region_name character varying,
	p_segment_code character varying,
	p_ring_code character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

 DECLARE
   sql TEXT; 
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords INTEGER; 
   LowerStart  character varying;
   LowerEnd  character varying;
   v_user_role_id integer;
   s_layer_columns_val text; 

BEGIN

sql:='';
LowerStart:='';
LowerEnd:='';

 --IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
    -- LowerStart:='LOWER(';
    -- LowerEnd:=')';
--END IF;
  

-- FETCH ALL COLUMNS FROM COLUMN SETTINGS TABLE
	 

	 
 
  
-- MANAGE SORT COLUMN NAME
/*IF (coalesce(TRIM(P_SORTCOLNAME,''))!='') THEN 

SELECT TRIM( trailing '	' from ''||P_SORTCOLNAME||'') into P_SORTCOLNAME;
select column_name into P_SORTCOLNAME from fiber_link_columns_settings WHERE UPPER(DISPLAY_NAME)=UPPER(P_SORTCOLNAME);
End IF;*/

/* raise info'P_SORTCOLNAME% ',P_SORTCOLNAME;
  raise info'S_LAYER_COLUMNS_VAL% ',S_LAYER_COLUMNS_VAL;*/

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'id' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS  S_No,id,segment_code,ring_code,site_id,site_name,ring_capacity,agg1_site_id,agg2_site_id,region_name,description,bh_status,COALESCE(ring_a_site_distance, 0)as ring_a_site_distance,COALESCE(ring_b_site_distance,0)as ring_b_site_distance,Peer1,Peer2  from vw_topology_plan_details where 1=1 AND is_agg_site IS NOT TRUE ';

 IF(p_region_name != '')THEN 

	sql:= sql ||'and region_name ilike ''%'||(p_region_name)||'%'' ';
 END IF;

if(p_segment_code != '') then

sql:= sql ||'and segment_code ilike ''%'||(p_segment_code)||'%'' ';
end if;

if(p_ring_code != '') then

sql:= sql ||'and ring_code ilike ''%'||(p_ring_code)||'%'' ';
end if;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2% ',sql;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
/*IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;*/
	-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a ' INTO TotalRecords;
 
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords,*
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$BODY$;
