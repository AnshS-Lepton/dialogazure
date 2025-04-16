
------------------------------------Modify existing delete entity function-----------------------------------

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

IF(V_IS_VALID and p_entity_type='Cable' and exists(select 1 from top_ring_cable_mapping where cable_id=(select network_id from att_details_cable where system_id=p_system_id) ))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as cable already in a topology so please rearrange it!');
end if;

IF(V_IS_VALID and upper(p_entity_type)='POD' and exists(select 1 from top_ring_site_mapping where site_id=(select network_id from att_details_pod where system_id=p_system_id) ))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as site already in a topology so please rearrange it!');
end if;

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

    ------------------------------------Modify existing sites dissociation function-----------------------------------
    DROP FUNCTION IF EXISTS public.fn_topology_get_sites(integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_sites(
	p_site_id integer,
	p_ring_id integer,
	p_distance integer,
	p_user_id integer)
    RETURNS TABLE(siteid integer, sitename character varying, sitedistance numeric, ringid integer,is_agg_site boolean) 
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
        user_id integer,
		 is_agg_site boolean
    ) ON COMMIT DROP;
	
	--truncate table temp_site_geom_route;
	
 -- Create a temporary table to store site geometries
    CREATE TEMP TABLE temp_site_geom_route( 
        site_id integer, 
        site_name character varying, 
        geom geometry,
		ringid integer,
        user_id integer,
		 cbl_distance numeric,
		 is_agg_site boolean
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
    INSERT INTO temp_site_geom_route(site_id, site_name, geom,ringid, user_id,cbl_distance,is_agg_site)
     select distinct sitedetails.system_id , sitedetails.site_name , ST_GeomFromText('POINT(' || sitedetails.longitude || ' ' || sitedetails.latitude || ')', 4326),sitedetails.ring_id,p_user_id,sitedetails.length_meters,sitedetails.is_agg_site  from  (select pod.system_id,pod.site_name,pod.longitude,pod.latitude,pod.ring_id,ST_Length(ST_Transform(lm.sp_geometry, 3857)) AS length_meters, case when pod.is_agg_site IS NULL THEN false else pod.is_agg_site end as is_agg_site    from att_details_pod pod
    inner join temp_cable_routes tcr on tcr.agg_site_id = pod.system_id
	inner join line_master lm on lm.system_id=tcr.edge_targetid and entity_type='Cable'
	where user_id = p_user_id) sitedetails where sitedetails.system_id <>p_site_id;
	
Raise info 'temp_cable_routes row ->%',(select count(1) from temp_cable_routes);

    -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom(site_id, site_name, geom,ringid, user_id,is_agg_site)
    SELECT distinct
        p.site_id, 
       CASE 
        WHEN tr.ring_code IS NULL OR TRIM(tr.ring_code) = '' THEN COALESCE(p.site_name, '')  
        ELSE CONCAT(p.site_name, ' (', tr.ring_code, ')') 
    END  AS site_name,    
        p.geom,
		p.ringid,
        p_user_id,
		p.is_agg_site
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
                    COALESCE(t.ringid, 0) AS ringid,
					t.is_agg_site
                FROM temp_site_geom t 
                ORDER BY sitedistance';

    -- Execute the SQL and return the result
    RETURN QUERY EXECUTE sql;
	
END;
$BODY$;

ALTER FUNCTION public.fn_topology_get_sites(integer, integer, integer, integer)
    OWNER TO postgres;


    ------------------------------------Modify existing sites dissociation function-----------------------------------

    DROP FUNCTION IF EXISTS public.fn_topology_sites_dissociation(integer, integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_sites_dissociation(
	p_base_site_id integer,
	p_site_id integer,
	p_ring_id integer,
	p_distance integer,
	p_user_id integer)
    RETURNS TABLE(siteid integer, sitename character varying, sitedistance numeric, ringid integer,is_agg_site boolean) 
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
        fn.siteid, fn.sitename, fn.sitedistance, fn.ringid, fn.is_agg_site  
    FROM fn_topology_get_sites(p_base_site_id, p_ring_id, p_distance, p_user_id) AS fn; 
	
END;
$BODY$;

----------------------------- Modify existing function to get the site details for the given ring id -----------------------------

DROP FUNCTION IF EXISTS public.fn_get_ring_details(integer, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_ring_details(
	p_segment_id integer,
	p_numberofsites integer,
	p_ringcapacity character varying)
    RETURNS TABLE(ring_id integer, ring_info text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 
    tr.id AS ring_id,
    CONCAT(tr.ring_code,' (', COUNT(adp.ring_id),')') AS ring_info
FROM 
    top_ring tr
LEFT JOIN 
    att_details_pod adp ON tr.id = adp.ring_id and adp.is_agg_site IS NOT TRUE
WHERE 
    tr.segment_id = p_segment_id and tr.ring_capacity=p_ringcapacity
GROUP BY 
    tr.id, tr.ring_code 
	HAVING COUNT(adp.ring_id) <= p_numberofsites 
	;

END;
$BODY$;

ALTER FUNCTION public.fn_get_ring_details(integer, integer, character varying)
    OWNER TO postgres;