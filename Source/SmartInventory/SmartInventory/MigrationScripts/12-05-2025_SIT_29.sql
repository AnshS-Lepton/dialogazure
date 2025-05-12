
---------------------------------Modification in existing function-----------------------------------


 DROP FUNCTION IF EXISTS public.fn_topology_get_sites(integer, integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_sites(
	p_site_id integer,
	p_ring_id integer,
	p_segment_id integer,
	p_distance integer,
	p_user_id integer)
    RETURNS TABLE(siteid integer, sitename character varying, sitedistance numeric, ringid integer, is_agg_site boolean) 
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
		network_id character varying, 
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
 
 -- Insert all aggregate sites which is connected to that segment
	 insert into temp_aggsite(system_id,network_id,geom)
     SELECT p.system_id,p.common_name, concat(st_x(p.sp_geometry)::text,' ',st_y(p.sp_geometry)::text)
     FROM  point_master p
     WHERE p.system_id in (SELECT agg1_site_id AS combined_column FROM top_segment ts where ts.id=p_segment_id
UNION ALL
SELECT agg2_site_id FROM top_segment ts2 where ts2.id=p_segment_id );

 -- Insert all mobile sites which is associated to that segment
insert into temp_aggsite(system_id,network_id,geom)
     SELECT p.system_id,p.common_name, concat(st_x(p.sp_geometry)::text,' ',st_y(p.sp_geometry)::text)
     FROM  point_master p
     WHERE p.system_id <> p_site_id 
	 and p.system_id 
	 in (select site_id from top_ring_site_mapping trsm inner join top_ring tr on tr.id=trsm.ring_id where tr.segment_id=p_segment_id );

	 
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
    INSERT INTO temp_site_geom_route(site_id, site_name, geom,ringid, user_id,cbl_distance,is_agg_site,network_id)
     select distinct sitedetails.system_id , sitedetails.site_name , ST_GeomFromText('POINT(' || sitedetails.longitude || ' ' || sitedetails.latitude || ')', 4326),sitedetails.ring_id,p_user_id,sitedetails.length_meters,sitedetails.is_agg_site ,sitedetails.network_id from  (select pod.system_id,pod.site_name,pod.longitude,pod.latitude,pod.ring_id,ST_Length(ST_Transform(lm.sp_geometry, 3857)) AS length_meters, case when pod.is_agg_site IS NULL THEN false else pod.is_agg_site end as is_agg_site,pod.network_id    from att_details_pod pod
    inner join temp_cable_routes tcr on tcr.agg_site_id = pod.system_id
	inner join line_master lm on lm.system_id=tcr.edge_targetid and entity_type='Cable'
	where user_id = p_user_id) sitedetails where sitedetails.system_id <>p_site_id;
	
Raise info 'temp_cable_routes row ->%',(select count(1) from temp_cable_routes);

    -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom(site_id, site_name, geom,ringid, user_id,is_agg_site)
    SELECT distinct
        p.site_id, 
       CASE 
        WHEN tr.ring_code IS NULL OR TRIM(tr.ring_code) = '' THEN COALESCE(CONCAT(p.site_name,' (',p.network_id,']'),'') 
        ELSE CONCAT(p.site_name,' (',p.network_id, ')', ' (', tr.ring_code, ')') 
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

ALTER FUNCTION public.fn_topology_get_sites(integer, integer, integer, integer, integer)
    OWNER TO postgres;

	---------------------- For multi Selection in ring details-------------------------------


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

sql:= sql ||'and ring_code in (''' || p_ring_code || ''') ';
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



	 FUNCTION: public.fn_delete_entity(integer, character varying, character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_delete_entity(integer, character varying, character varying, integer);

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
ON SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE')
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
elsif exists(select 1 from ASSOCIATE_ROUTE_INFO asso where asso.entity_id=p_system_id and upper(entity_type)=upper(p_entity_type))
then
delete from ASSOCIATE_ROUTE_INFO where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);

-- elsif exists(select 1 from ASSOCIATE_ROUTE_INFO asso where asso.entity_id=p_system_id and upper(entity_type)=upper(p_entity_type))
-- then
-- V_IS_VALID=FALSE;
-- V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as route is associated with some entity!');

--CHECK THE ENTITY ASSOCIATION FIRST
elsif exists(select 1 from associate_entity_info asso where ((asso.associated_system_id=p_system_id and upper(asso.associated_entity_type)=UPPER(p_entity_type)) or
(asso.entity_system_id=p_system_id and upper(asso.entity_type)=UPPER(p_entity_type))) and asso.is_termination_point=false)
and not exists(select 1 from layer_details where upper(layer_name)=upper(p_entity_type) and ne_class_name='SPLICE_CLOSURE')
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_152]');--(V_LAYER_TITLE||'can not be deleted as it is associated with some entity!');

--CHECK THE TERMINATION POINT DUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('duct') and is_enabled=true)
and exists(select 1 from att_details_duct duct where (duct.a_system_id=p_system_id and upper(duct.a_entity_type)=upper(p_entity_type))
or (duct.b_system_id=p_system_id and upper(duct.b_entity_type)=upper(p_entity_type)))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_053]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with duct!');

--CHECK THE TERMINATION POINT TRENCH
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('trench') and is_enabled=true)
and exists(select 1 from att_details_trench trench where (trench.a_system_id=p_system_id and upper(trench.a_entity_type)=upper(p_entity_type))
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
and exists(select 1 from att_details_microduct microduct where (microduct.a_system_id=p_system_id and upper(microduct.a_entity_type)=upper(p_entity_type))
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
and exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id and upper(row_status)=upper('Approved'))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_057]');--(V_LAYER_TITLE||' can not be deleted as it has been Approved!');

elsif(upper(p_entity_type)='PIT')
then
select parent_system_id into v_parent_system_id from att_details_row_pit where system_id=p_system_id;

IF EXISTS(SELECT 1 FROM ATT_DETAILS_ROW_APPLY WHERE ROW_SYSTEM_ID=v_parent_system_id)
THEN
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as ROW has been Applied!');
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

EXECUTE 'SELECT COALESCE(TARGET_REF_ID,0) FROM '||V_LAYER_TABLE||' WHERE SYSTEM_ID='||P_SYSTEM_ID||'' INTO V_TARGET_REF_ID;

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

IF(V_IS_VALID and p_entity_type='Cable' and(exists(select 1 from top_ring_cable_mapping where cable_id=p_system_id ) or exists(select 1 from top_segment_cable_mapping where cable_id=p_system_id )) )
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as cable already in a topology so please rearrange it!');
end if;

IF(V_IS_VALID and upper(p_entity_type)='POD' and exists(select 1 from top_ring_site_mapping where site_id=p_system_id ))
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
DELETE FROM ATT_DETAILS_MODEL B
USING ATT_DETAILS_FMS C
WHERE UPPER(B.NETWORK_ID)=UPPER(C.NETWORK_ID)
AND C.SYSTEM_ID=P_SYSTEM_ID;
END IF;

perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,2,true));

select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
execute 'select network_id from '||v_layer_table||' where system_id='||p_system_id||'' into v_network_id;
execute 'delete from '||v_layer_table||' where system_id='||p_system_id||'';
execute 'delete from '||p_geom_type||'_master where system_id='||p_system_id||' and upper(entity_type)=upper('''||p_entity_type||''')';
delete from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from isp_entity_mapping where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from isp_port_info where parent_system_id=p_system_id and upper(parent_entity_type)=upper(p_entity_type);
delete from associate_entity_info where (associated_system_id=p_system_id
and upper(associated_entity_type)=upper(p_entity_type)) or (entity_system_id=p_system_id and upper(entity_type)=upper(p_entity_type)) and is_termination_point=true;
delete from connection_info where (source_system_id=p_system_id
and upper(source_entity_type)=upper(p_entity_type)) or (destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type));
delete from isp_line_master where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from circle_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type);

if(UPPER(p_entity_type)=UPPER('POD') and (select count(1) from top_ring_site_mapping where site_id=p_system_id)>0) then

delete from top_ring_site_mapping where site_id=p_system_id;
END IF;
if(UPPER(p_entity_type)=UPPER('Cable') and (select count(1) from top_ring_cable_mapping where cable_id=p_system_id)>0) then

delete from top_ring_cable_mapping where cable_id=p_system_id;
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

GRANT EXECUTE ON FUNCTION public.fn_delete_entity(integer, character varying, character varying, integer) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public.fn_delete_entity(integer, character varying, character varying, integer) TO postgres;

--------------------------------------------Modify the view ------------------------------------------------

DROP VIEW public.vw_att_details_pod;

CREATE OR REPLACE VIEW public.vw_att_details_pod
 AS
 SELECT pod.system_id,
    pod.network_id,
    pod.pod_name,
    round(pod.latitude::numeric, 6) AS latitude,
    round(pod.longitude::numeric, 6) AS longitude,
    pod.pincode,
    pod.address,
    pod.specification,
    pod.category,
    pod.subcategory1,
    pod.subcategory2,
    pod.subcategory3,
    pod.item_code,
    pod.vendor_id,
    pod.type,
    pod.brand,
    pod.model,
    pod.construction,
    pod.activation,
    pod.accessibility,
    pod.status,
    pod.network_status,
    pod.province_id,
    pod.region_id,
    pod.created_by,
    pod.created_on,
    pod.modified_by,
    pod.modified_on,
    pod.parent_system_id,
    pod.parent_network_id,
    pod.parent_entity_type,
    pod.sequence_id,
    pod.structure_id,
    pod.shaft_id,
    pod.floor_id,
    pod.project_id,
    pod.planning_id,
    pod.purpose_id,
    pod.workorder_id,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS pod_types,
    bd.brand AS pod_brand,
    ml.model AS pod_model,
    fn_get_date(pod.created_on) AS created_date,
    fn_get_date(pod.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.accessibility) AS pod_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.construction) AS pod_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.activation) AS pod_activation,
    ''::character varying AS additional_attributes,
    ''::text AS total_ports,
    ''::text AS entity_category,
    pod.pod_name AS entity_name,
    pod.acquire_from,
    pod.pod_type,
    pod.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    pod.source_ref_type,
    pod.source_ref_id,
    pod.source_ref_description,
    pod.status_remark,
    pod.status_updated_by,
    pod.status_updated_on,
    pod.is_visible_on_map,
    pod.is_new_entity,
    pod.remarks,
    pod.audit_item_master_id,
    pod.is_acquire_from,
    isp_room_info.network_id AS unit_network_id,
    s.shaft_name,
    f.floor_name,
    pod.origin_from,
    pod.origin_ref_id,
    pod.origin_ref_code,
    pod.origin_ref_description,
    pod.request_ref_id,
    pod.requested_by,
    pod.request_approved_by,
    pod.subarea_id,
    pod.area_id,
    pod.dsa_id,
    pod.csa_id,
    pod.bom_sub_category,
    pod.gis_design_id,
    pod.st_x,
    pod.st_y,
    pod.ne_id,
    pod.prms_id,
    pod.jc_id,
    pod.mzone_id,
    pod.hierarchy_type,
    vm3.name AS own_vendor_name,
    pod.site_id,
    pod.site_name,
    pod.on_air_date,
    pod.removed_date,
    pod.tx_type,
    pod.tx_technology,
    pod.tx_segment,
    pod.tx_ring,
    pod.province,
    pod.district,
    pod.depot,
    pod.ds_division,
    pod.local_authority,
    pod.owner_name,
    pod.access_24_7,
    pod.tower_type,
    pod.tower_height,
    pod.cabinet_type,
    pod.solution_type,
    pod.site_rank,
    pod.self_tx_traffic,
    pod.agg_tx_traffic,
    pod.csr_count,
    pod.dti_circuit,
    pod.agg_01,
    pod.agg_02,
    pod.bandwidth,
    pod.ring_type,
    pod.link_id,
    pod.alias_name,
    (pod.site_id::text || '-'::text) || pod.site_name::text AS site_id_site_name,
	COALESCE(pod.is_agg_site, false) AS is_agg_site
   FROM att_details_pod pod
     JOIN province_boundary prov ON pod.province_id = prov.id
     JOIN region_boundary reg ON pod.region_id = reg.id
     JOIN user_master um ON pod.created_by = um.user_id
     JOIN vendor_master vm ON pod.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pod.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pod.own_vendor_id::integer = vm3.id
     LEFT JOIN isp_type_master tp ON pod.type = tp.id
     LEFT JOIN isp_brand_master bd ON pod.brand = bd.id
     LEFT JOIN isp_base_model ml ON pod.model = ml.id
     LEFT JOIN user_master um2 ON pod.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'POD'::text) m1 ON pod.system_id = m1.entity_id
     LEFT JOIN ( SELECT isp_entity_mapping.id,
            isp_entity_mapping.structure_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id,
            isp_entity_mapping.entity_type,
            isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'UNIT'::text) m2 ON m1.parent_id = m2.id
     LEFT JOIN isp_room_info ON m2.entity_id = isp_room_info.system_id AND m2.floor_id = isp_room_info.floor_id
     LEFT JOIN isp_shaft_info s ON m2.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m2.floor_id = f.system_id;

