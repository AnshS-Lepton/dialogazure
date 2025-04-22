
---------------------Modify existing function-------------------
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

    -- Prepare SQL for execution
    sql := 'SELECT distinct s.id, s.segment_code, s.region_id, s.agg1_site_id, s.agg2_site_id,adp.system_id as agg_01,adp2.system_id as agg_02 
            FROM top_segment_cable_mapping ts
            INNER JOIN top_segment s ON s.id = ts.segment_id
            inner join att_details_cable cbl on cbl.network_id =ts.cable_id
			inner join att_details_pod adp on UPPER(adp.site_id)= UPPER(SPLIT_PART(s.agg1_site_id, '' '', 1))
            inner join att_details_pod adp2 on UPPER(adp2.site_id)=UPPER(SPLIT_PART(s.agg2_site_id, '' '', 1))
            INNER JOIN temp_cable_routes tc ON tc.edge_targetid = cbl.system_id
            ';

    -- Execute the SQL and return the result
    RETURN QUERY EXECUTE sql;

END;
$BODY$;

----------------------- Modify existing function-------------------

DROP FUNCTION IF EXISTS public.fn_topology_get_segment_cables(integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_segment_cables(
	p_agg1_site_id integer,
	p_agg2_site_id integer,
	p_system_id integer,
	p_user_id integer)
    RETURNS TABLE(cable_name character varying, network_id character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE 
    p_site1_geom character varying;
    p_site2_geom character varying;
	
	 rec RECORD;

begin
create temp table temp_agg_site(
  agg_site_id integer,
	geom character varying
) ON COMMIT DROP;
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
        user_id integer
    ) ON COMMIT DROP;
   
   
   insert into temp_agg_site(agg_site_id,geom)(SELECT system_id,longitude || ' ' || latitude AS geom
    
     FROM att_details_pod
     WHERE system_id in (p_agg1_site_id,p_agg2_site_id) );
	 
    -- get site details from the p_agg1_site_id   
     SELECT longitude || ' ' || latitude AS geom
     INTO p_site1_geom
     FROM att_details_pod
     WHERE system_id  = p_system_id;   

FOR rec IN (SELECT * FROM temp_agg_site) LOOP
       p_site2_geom :='';
	   p_site2_geom =rec.geom;

	 -- Populate temporary cable routes table
      INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id)
		   
	  SELECT seq, path_seq, edge,p_user_id FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	  FROM routing_data_core_plan', (SELECT id
	  FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), 2))
	  limit 1 ),(SELECT id FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, 
	  ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_site2_geom ||')', 4326), 2)) 
	  limit 1));
	
	 END LOOP;
    delete from temp_cable_routes where edge_targetid = -1 and user_id = p_user_id;

    return QUERY select distinct cbl.cable_name ,cbl.network_id  from att_details_cable cbl
    inner join temp_cable_routes tcr on tcr.edge_targetid = cbl.system_id where user_id = p_user_id;

END;
$BODY$;

ALTER FUNCTION public.fn_topology_get_segment_cables(integer, integer, integer, integer)
    OWNER TO postgres;

    ----------------------------- Modify existing function-------------------

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
		IF (SELECT COUNT(1) FROM top_ring_cable_mapping WHERE cable_id = rec.network_id) = 0 THEN
    INSERT INTO top_ring_cable_mapping (ring_id, cable_id)
    VALUES (p_ring_id, rec.network_id);
ELSE
    UPDATE top_ring_cable_mapping
    SET ring_id = p_ring_id
    WHERE cable_id = rec.network_id;
END IF;

		
		end if;

IF (SELECT COUNT(1) FROM top_segment_cable_mapping WHERE cable_id = rec.network_id) = 0 THEN
        INSERT INTO top_segment_cable_mapping (segment_id, cable_id) 
        VALUES (p_segment_id, rec.network_id);
		ELSE
UPDATE top_segment_cable_mapping
    SET segment_id = p_segment_id
    WHERE cable_id = rec.network_id;
	END IF;	
		
    END LOOP;
	
insert into top_ring_site_mapping(ring_id,site_id)select p_ring_id,(select network_id from att_details_pod adp  where system_id=p_system_id);
update att_details_pod set ring_id=p_ring_id where system_id IN (p_agg1_system_id, p_agg2_system_id);
IF (
    SELECT COUNT(1)
    FROM top_ring_site_mapping
    WHERE ring_id = p_ring_id
      AND site_id IN (
          SELECT network_id
          FROM att_details_pod
          WHERE system_id IN (p_agg1_system_id, p_agg2_system_id)
      )
) = 0 THEN
    INSERT INTO top_ring_site_mapping (ring_id, site_id)
    SELECT p_ring_id, network_id
    FROM att_details_pod
    WHERE system_id IN (p_agg1_system_id, p_agg2_system_id);
ELSE
    UPDATE top_ring_site_mapping
    SET ring_id = p_ring_id
    WHERE site_id IN (
        SELECT network_id
        FROM att_details_pod
        WHERE system_id IN (p_agg1_system_id, p_agg2_system_id)
    );
END IF;

	
    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN
        RAISE WARNING 'Error in function: %', SQLERRM;
        RETURN FALSE;
END;
$BODY$;

ALTER FUNCTION public.fn_insert_top_segment_ring_cable_mapping(integer, integer, integer, integer, integer, character varying, integer)
    OWNER TO postgres;


    ----------------------------- Modify existing function-------------------

   

 DROP FUNCTION IF EXISTS public.fn_delete_entity(integer, character varying, character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_delete_entity(
	p_system_id integer,
	p_entity_type character varying,
	p_geom_type character varying,
	p_user_id integer)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE 
v_layer_table character varying;
v_is_parent_exist boolean;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
V_LAYER_TITLE character varying;
v_parent_system_id integer;
v_building_system_id integer;
v_building_code character varying;
v_network_id character varying;
v_target_ref_id integer;

BEGIN
--GET THE LAYER TITLE
SELECT layer_title into V_LAYER_TITLE FROM layer_details where upper(layer_name)=upper(p_entity_type);

V_IS_VALID:=TRUE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_065]');--(V_LAYER_TITLE||' has deleted successfully!');
--CHECK THE STRUCTURE CHILD EXIST
if(upper(p_entity_type)='STRUCTURE' and (select count(1) from isp_entity_mapping where structure_id=p_system_id)>0)
then 
V_IS_VALID=FALSE;
V_MESSAGE:=('[SI_GBL_GBL_GBL_GBL_051]');--Structure can not be deleted as there are some dependent elements! 

elsIF(upper(p_entity_type)='POD' and (select count(1) from user_module_mapping umm  where user_id=p_user_id and module_id=(select id from module_master where  module_abbr='TPLD'))=0 )
then 
if((select count(1) from top_ring_site_mapping where site_id=(select network_id from att_details_pod adp  where system_id=p_system_id))=0) then
V_IS_VALID=TRUE;
else
V_IS_VALID=FALSE;
V_MESSAGE:=('[SI_GBL_GBL_GBL_GBL_007]');--You are not authorized to delete entity!! 
end if;
elsIF(upper(p_entity_type)='CABLE' and (select count(1) from user_module_mapping umm  where user_id=p_user_id and module_id=(select id from module_master where module_abbr='TPLD'))=0 ) 
then 
if ((select count(1) from top_ring_cable_mapping where cable_id=(select network_id from att_details_cable adp  where system_id=p_system_id))=0)
then
V_IS_VALID=TRUE;
else
V_IS_VALID=FALSE;
V_MESSAGE:=('[SI_GBL_GBL_GBL_GBL_007]');--You are not authorized to delete entity!! 
end if;
--CHECK THE SPLICING(EXCLUDING THE INERNAL CONNECTIVITY OF THE DEVICE)
elsIF EXISTS(select 1 from connection_info 
where ((source_system_id=p_system_id and upper(source_entity_type)=upper(p_entity_type)) or ( destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type)))
and ( ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))))
THEN 
v_is_parent_exist=false;
V_IS_VALID=FALSE;   

IF EXISTS(SELECT 1 FROM CONNECTION_INFO where ((source_system_id=p_system_id and upper(source_entity_type)=upper(p_entity_type)) or ( destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type))) )
THEN 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!');       
END IF;

IF(UPPER(p_entity_type)='SPLITTER') 
AND EXISTS(SELECT 1 FROM CONNECTION_INFO CON JOIN ATT_DETAILS_SPLITTER SPL 
ON  SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE') 
OR (CON.DESTINATION_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.DESTINATION_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.SOURCE_ENTITY_TYPE)='CABLE')) )
THEN
v_is_parent_exist=true; 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 
        ELSIF(UPPER(p_entity_type)='SPLITTER')THEN 
v_is_parent_exist=TRUE;
END IF;
        
IF(UPPER(p_entity_type)='SPLICECLOSURE') 
AND EXISTS(SELECT 1 FROM CONNECTION_INFO CON JOIN ATT_DETAILS_SPLICECLOSURE SPL 
ON SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE') 
OR (CON.DESTINATION_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.DESTINATION_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.SOURCE_ENTITY_TYPE)='CABLE')))
THEN 
        v_is_parent_exist=true;
        V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 

ELSIF((SELECT PARENT_SYSTEM_ID FROM ATT_DETAILS_SPLICECLOSURE SPL WHERE SPL.SYSTEM_ID=p_system_id and UPPER(p_entity_type)='SPLICECLOSURE')!=0)THEN 
v_is_parent_exist=true;
END IF;

IF(v_is_parent_exist=false)
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 

end if; 
--CHECK THE ROUTE ENTITY ASSOCIATION FIRST
elsif exists(select 1 from  ASSOCIATE_ROUTE_INFO asso where asso.entity_id=p_system_id and upper(entity_type)=upper(p_entity_type))
then 
delete from ASSOCIATE_ROUTE_INFO where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);

-- elsif exists(select 1 from  ASSOCIATE_ROUTE_INFO asso where asso.entity_id=p_system_id and upper(entity_type)=upper(p_entity_type))
-- then 
-- V_IS_VALID=FALSE;
-- V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as route is associated with some entity!');

--CHECK THE ENTITY ASSOCIATION FIRST
elsif exists(select 1 from  associate_entity_info asso where ((asso.associated_system_id=p_system_id and upper(asso.associated_entity_type)=UPPER(p_entity_type)) or 
(asso.entity_system_id=p_system_id and upper(asso.entity_type)=UPPER(p_entity_type))) and asso.is_termination_point=false)
and not exists(select 1 from layer_details where upper(layer_name)=upper(p_entity_type) and ne_class_name='SPLICE_CLOSURE')
then 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_152]');--(V_LAYER_TITLE||'can not be deleted as it is associated with some entity!'); 

--CHECK THE TERMINATION POINT DUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('duct') and is_enabled=true) 
and exists(select  1 from att_details_duct duct where (duct.a_system_id=p_system_id and upper(duct.a_entity_type)=upper(p_entity_type)) 
or (duct.b_system_id=p_system_id and upper(duct.b_entity_type)=upper(p_entity_type)))
then 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_053]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with duct!'); 

--CHECK THE TERMINATION POINT TRENCH
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('trench') and is_enabled=true) 
and exists(select  1 from att_details_trench trench where (trench.a_system_id=p_system_id and upper(trench.a_entity_type)=upper(p_entity_type)) 
or (trench.b_system_id=p_system_id and upper(trench.b_entity_type)=upper(p_entity_type)))
then   
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_054]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with trench!'); 

--CHECK THE TERMINATION POINT CABLE
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('cable') and is_enabled=true) 
  and exists(select 1 from att_details_cable cable where (cable.a_system_id=p_system_id and upper(cable.a_entity_type)=upper(p_entity_type)) 
  or (cable.b_system_id=p_system_id and upper(cable.b_entity_type)=upper(p_entity_type)))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_055]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with cable!');

--CHECK THE TERMINATION POINT MICRODUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('microduct') and is_enabled=true) 
and exists(select  1 from att_details_microduct microduct where (microduct.a_system_id=p_system_id and upper(microduct.a_entity_type)=upper(p_entity_type)) 
or (microduct.b_system_id=p_system_id and upper(microduct.b_entity_type)=upper(p_entity_type)))
then 
V_IS_VALID=FALSE;
V_MESSAGE:=(V_LAYER_TITLE ||' can not be deleted as it is used as termination point with microduct!'); --//have change the message for microduct 

elsif(upper(p_entity_type)='ROW') 
and exists(select 1 from att_details_row_apply where row_system_id=p_system_id) 
and not exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id)
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_056]');--(V_LAYER_TITLE||' can not be deleted as it is applied!');

elsif(upper(p_entity_type)='ROW') 
and exists(select 1 from att_details_row_apply where row_system_id=p_system_id) 
and  exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id and  upper(row_status)=upper('Approved'))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_057]');--(V_LAYER_TITLE||' can not be deleted as it has been Approved!');

elsif(upper(p_entity_type)='PIT')
then 
select parent_system_id into v_parent_system_id from att_details_row_pit where system_id=p_system_id;

IF EXISTS(SELECT 1 FROM ATT_DETAILS_ROW_APPLY WHERE ROW_SYSTEM_ID=v_parent_system_id) 
THEN
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as  ROW has been Applied!'); 
END IF;
ELSIF(upper(p_entity_type)='CABLE') AND EXISTS(SELECT 1 FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=p_system_id)
then

V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as LMC is associated!');
ELSIF(upper(p_entity_type)='CABLE')
then
delete from att_details_loop where cable_system_id= p_system_id;

end if;

IF(upper(p_geom_type)='POLYGON') then

select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
raise info'V_LAYER_TABLE: %',V_LAYER_TABLE;

EXECUTE  'SELECT COALESCE(TARGET_REF_ID,0)  FROM '||V_LAYER_TABLE||' WHERE SYSTEM_ID='||P_SYSTEM_ID||'' INTO V_TARGET_REF_ID;

IF(upper(p_entity_type)!='SECTOR') THEN
IF exists (select 1 from 
(SELECT ptm.system_id ,ptm.entity_type,ptm.SP_GEOMETRY FROM point_master ptm  
UNION
SELECT lm.system_id ,lm.entity_type,lm.SP_GEOMETRY FROM line_master lm  
UNION
SELECT pm.system_id ,pm.entity_type,pm.SP_GEOMETRY FROM polygon_master pm  
) a WHERE ST_Within(a.SP_GEOMETRY, 
(select sp_geometry from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type))) 
and a.system_id!=p_system_id) 
then

V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as some child entities are present inside it!');

end if;

END IF;
-- IF(p_entity_type in('Area','Subarea','DSA') and
-- exists(select 1 from polygon_master pm 
-- where pm.system_id =p_system_id and upper(pm.entity_type) = upper(p_entity_type) and COALESCE(pm.gis_design_id ,'')!='') )
-- THEN 
-- V_IS_VALID=FALSE;
-- V_MESSAGE:='Reset the design ID to delete the entity';
-- END IF;
IF(V_TARGET_REF_ID>0 and (select COALESCE(value,'0')::INTEGER from global_settings gs where key='IS_DeleteEnabledForGISPush')=1)
THEN 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as it is been pushed to GIS!');

END IF;

END IF;

IF(V_IS_VALID)
THEN
IF(UPPER(p_entity_type)=UPPER('SUBAREA')) 
THEN
select building_system_id,building_code into v_building_system_id,v_building_code from att_details_subarea where system_id=p_system_id;
delete from att_details_building where system_id=v_building_system_id and network_id=v_building_code;
delete from polygon_master where system_id=v_building_system_id and upper(entity_type)=upper('Building'); 
END IF;
IF(upper(p_entity_type)='FMS')
THEN
DELETE  FROM ATT_DETAILS_MODEL B  
USING ATT_DETAILS_FMS C 
WHERE UPPER(B.NETWORK_ID)=UPPER(C.NETWORK_ID)
AND C.SYSTEM_ID=P_SYSTEM_ID;     
END IF;

perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,2,true));

select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
execute  'select network_id from '||v_layer_table||' where system_id='||p_system_id||'' into v_network_id; 
execute  'delete from '||v_layer_table||' where system_id='||p_system_id||''; 
execute 'delete from '||p_geom_type||'_master where system_id='||p_system_id||' and upper(entity_type)=upper('''||p_entity_type||''')';
delete from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from isp_entity_mapping where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from isp_port_info where parent_system_id=p_system_id and upper(parent_entity_type)=upper(p_entity_type);
delete from associate_entity_info where (associated_system_id=p_system_id 
and upper(associated_entity_type)=upper(p_entity_type)) or (entity_system_id=p_system_id and upper(entity_type)=upper(p_entity_type)) and is_termination_point=true;
delete from connection_info  where (source_system_id=p_system_id 
and upper(source_entity_type)=upper(p_entity_type)) or (destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type));
delete from isp_line_master  where entity_id=p_system_id  and upper(entity_type)=upper(p_entity_type);
delete from circle_master  where system_id=p_system_id  and upper(entity_type)=upper(p_entity_type);

if(UPPER(p_entity_type)=UPPER('POD') and (select count(1) from top_ring_site_mapping where site_id=v_network_id)>0) then

 delete from top_ring_site_mapping where site_id=v_network_id;
END IF;
if(UPPER(p_entity_type)=UPPER('Cable') and (select count(1) from top_ring_cable_mapping where cable_id=v_network_id)>0) then

 delete from top_ring_cable_mapping where cable_id=v_network_id;
END IF;
IF EXISTS (select 1 from layer_details where upper(layer_name)=upper(p_entity_type) and is_reference_allowed=true) THEN
BEGIN
delete from att_entity_reference where system_id=p_system_id;
END;

IF(UPPER(p_entity_type)=UPPER('CUSTOMER'))
THEN
delete from wcr_mapping where CUSTOMER_ID=p_system_id;
END IF;

END IF;

RETURN QUERY 
SELECT true AS STATUS, fn_Multilingual_Message_Convert('en',Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_065]')::CHARACTER VARYING)::CHARACTER VARYING AS MESSAGE ; --V_LAYER_TITLE||' has deleted successfully!')
ELSE
RETURN QUERY 
SELECT V_IS_VALID AS STATUS, fn_Multilingual_Message_Convert('en',V_MESSAGE::CHARACTER VARYING) ::CHARACTER VARYING AS MESSAGE;
END IF;

if(coalesce(v_network_id,'')!='')
then
insert into entity_delete_log(system_id,entity_type,network_id,action_date,action_by)
values(p_system_id,p_entity_type,v_network_id,now(),p_user_id);
end if;

END
$BODY$;

ALTER FUNCTION public.fn_delete_entity(integer, character varying, character varying, integer)
    OWNER TO postgres;


    ----------------------------- Modify existing VIEW-------------------

    DROP VIEW public.vw_topology_plan_details;

CREATE OR REPLACE VIEW public.vw_topology_plan_details
 AS
 SELECT tr.id,
    ts.segment_code,
    tr.ring_code,
    att.site_id,
    att.site_name,
    tr.ring_capacity,
    ts.agg1_site_id,
    ts.agg2_site_id,
    trr.region_name,
    tr.description,
    att.bh_status,
    att.ring_a_site_distance,
    att.ring_b_site_distance,
	(select site_id from att_details_pod where system_id=att.ring_a_site_id) as Peer1,
    (select site_id from att_details_pod where system_id=att.ring_b_site_id) as Peer2
   FROM top_ring tr
     JOIN top_segment ts ON ts.id = tr.segment_id
     JOIN top_region trr ON trr.id = ts.region_id
     LEFT JOIN att_details_pod att ON tr.id = att.ring_id;

     ------------------------------ call this function to sync column name-------------------
     select * from  fn_sync_layer_columns();
     ----------------------------------------------------------------------------------------

     ----------------------------- Modify existing function-------------------

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
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'id' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS  S_No,id,segment_code,ring_code,site_id,site_name,ring_capacity,agg1_site_id,agg2_site_id,region_name,description,bh_status,COALESCE(ring_a_site_distance, 0)as ring_a_site_distance,COALESCE(ring_b_site_distance,0)as ring_b_site_distance,Peer1,Peer2  from vw_topology_plan_details where 1=1  ';

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

ALTER FUNCTION public.fn_get_ring_details(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying)
    OWNER TO postgres;

    ----------------------------- update module name-------------------

    update module_master set module_name='Delete Site/Cable',module_description='Delete Site/Cable' where module_name='Delete Site' and module_abbr='TPLD';
