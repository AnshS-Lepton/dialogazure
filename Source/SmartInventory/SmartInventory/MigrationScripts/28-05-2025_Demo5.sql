
------------------------------------------- Create new function to get selected route -------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_topology_get_selectedroute(
	route_geom character varying,
	p_agg1 integer,
	p_agg2 integer,
	p_user_id integer)
    RETURNS TABLE(route_id integer, route_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    v_source_geom geometry;
	v_buffered_route geometry;
	v_agg1site_id integer;
	v_agg2site_id integer;
	v_Count integer;
BEGIN

create temp table temp_cable
(

network_id character varying

) on commit drop;

--select agg1site_id,agg2site_id into v_agg1site_id,v_agg2site_id  from top_routes order by 1 desc limit 1;

    -- Convert input string to a valid LINESTRING geometry
    v_source_geom := ST_GeomFromText('LINESTRING(' || route_geom || ')', 4326);

 -- Buffer in meters: Convert to geography, buffer, then back to geometry
    v_buffered_route := ST_Transform(
                            ST_Buffer(
                                ST_Transform(v_source_geom, 3857),  -- project to meters (Web Mercator)
                                2
                            ),
                            4326
                        );
						
   
	insert into temp_cable (network_id)
    SELECT 
        cbl.network_id AS route_id
    FROM 
        line_master l
    INNER JOIN 
        att_details_cable cbl 
        ON cbl.system_id = l.system_id
    WHERE 
        UPPER(l.entity_type) = 'CABLE'
        AND ST_Within(
            l.sp_geometry,
            v_buffered_route
        );
		
		select count(1) into v_Count from temp_cable;
		
		RAISE INFO 'Row Count -> %', v_Count;
		
	insert into top_routes(route_name,cable_names,total_length,route_geom,agg1site_id,agg2site_id)
	
	SELECT 
  'Manual_Route' AS route_name,
  (select '{' || string_agg(network_id, ',') || '}' from temp_cable) as cable_names,
  0,
  v_source_geom AS  route_geom,
 p_agg1,
 p_agg2
 ;

 -- Return matching cable lines within 3 meters
   
    RETURN QUERY
    select Max(tr.route_id) AS route_id,tr.route_name from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2
    GROUP BY tr.route_name order by 1 desc ;

END;
$BODY$;



------------------------------------------- Create new function to get validate route entity -------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_getvalidateroutetopologyentities(
	lat double precision,
	lng double precision,
	p_geom character varying,
	p_user_id integer)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_role_id INTEGER;
    v_result BOOLEAN := false;
    v_geom GEOMETRY;
    v_point GEOMETRY := ST_SetSRID(ST_MakePoint(lng, lat), 4326);
BEGIN
    -- Optional: Fetch role ID
    SELECT role_id INTO v_role_id 
    FROM user_master 
    WHERE user_id = p_user_id;
 RAISE INFO 'v_point -> %', ST_AsText(v_point);
 
    -- Convert raw coordinate list into proper LINESTRING geometry
    v_geom := ST_GeomFromText('LINESTRING(' || p_geom || ')', 4326);

RAISE INFO 'StartPoint -> %', ST_AsText(ST_StartPoint(v_geom));
RAISE INFO 'EndPoint -> %', ST_AsText(ST_EndPoint(v_geom));
RAISE INFO 'v_geom -> %', ST_AsText(v_geom);

   -- Create 2m buffers around start and end points
 IF ST_Within(v_point, st_buffer_meters(ST_StartPoint(v_geom), 2))
    OR ST_Within(v_point, st_buffer_meters(ST_EndPoint(v_geom), 2)) THEN
    v_result := true;
END IF;

    RETURN v_result;
END;
$BODY$;

--------------------------------------------------- Create new function to get near by entity -------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_getnearbytopologyentities(
	lat double precision,
	lng double precision,
	mtrbuffer integer,
	p_user_id integer,
	p_source_ref_id character varying,
	p_source_ref_type character varying)
    RETURNS TABLE(system_id integer, entity_type character varying, common_name character varying, display_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

declare 
v_role_id integer;

BEGIN
	

	SELECT ROLE_ID INTO V_ROLE_ID FROM USER_MASTER WHERE USER_ID=P_USER_ID;

create temp table temp_entity (
system_id integer,
entity_type character varying,
common_name character varying
)ON COMMIT DROP;

insert into temp_entity select ent.system_id,ent.entity_type,ent.common_name from fn_getnearbyentities(lat,lng,mtrbuffer,p_user_id,p_source_ref_id,p_source_ref_type) as ent;

return query
select adp.system_id,en.entity_type,adp.site_id::character varying,concat(adp.site_name,' ( ',adp.network_id,' )')::character varying  from att_details_pod adp inner join temp_entity en on en.system_id=adp.system_id  where en.entity_type='POD';
	
    
END
$BODY$;

--------------------------------------------------- Modify existing function used in ring details -------------------------------------------


 DROP FUNCTION IF EXISTS public.fn_get_ring_details(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_ring_details(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_region_name character varying,
	p_segment_code character varying,
	p_ring_code character varying,
	p_site_id character varying)
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
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'id' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS  S_No,id,segment_code,ring_code,site_id,site_name,ring_capacity,agg1_site_id,agg2_site_id,region_name,description,bh_status,COALESCE(ring_a_site_distance, 0)as ring_a_site_distance,COALESCE(ring_b_site_distance,0)as ring_b_site_distance,Peer1,Peer2  from vw_topology_plan_details where 1=1 and peer1 is not null and peer2 is not null AND is_agg_site IS NOT TRUE ';

 IF(p_region_name != '')THEN 

	sql:= sql ||'and region_name ilike ''%'||(p_region_name)||'%'' ';
 END IF;

if(p_segment_code != '') then

sql:= sql ||'and segment_code ilike ''%'||(p_segment_code)||'%'' ';
end if;

if(p_ring_code != '') then

sql:= sql ||'and ring_code in (''' || p_ring_code || ''') ';
end if;
if(p_site_id != '') then

sql:= sql ||'and site_id in (''' || p_site_id || ''') ';
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

ALTER FUNCTION public.fn_get_ring_details(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, character varying, character varying)
    OWNER TO postgres;

    -------------------------------- Modify existing function to get schematicview -------------------------------------------
 DROP FUNCTION IF EXISTS public.fn_get_ring_schematicview(integer);

CREATE OR REPLACE FUNCTION public.fn_get_ring_schematicview(
	p_ring_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
DECLARE 
v_Query text;
Query text; 
rec record;
v_from_id integer;
v_from_entity_type character varying;
nodes text;
edges text;
nodesDownstream text;
edgesDownstream text;
legends text;
cables text;
BEGIN

create temp table cpf_temp_result
(
id serial,
source_system_id integer,
source_network_id character varying,
source_port_no integer, 
source_entity_type character varying,
source_entity_title character varying,
destination_system_id integer,
destination_network_id character varying,
destination_port_no integer,
destination_entity_type character varying,
destination_entity_title character varying,
via_system_id integer,
via_network_id character varying,
via_entity_type character varying,
via_port_no integer
)on commit drop;

create temp table temp_nodes
(
id serial,
source_system_id integer,
source_network_id character varying,
connected_ports character varying, 
source_entity_type character varying,
source_entity_title character varying,
node_name 	character varying
)on commit drop;

create temp table temp_edges
(
id serial,
from_id integer,
from_entity_type character varying,
from_network_id character varying,
to_id integer,
to_network_id character varying,
to_entity_type character varying,
label character varying,
via_ports character varying,
network_status character varying,
cable_type character varying,
color_code character varying,
total_core integer,
cable_category character varying,
	cable_calculated_length double precision
)on commit drop;

insert into cpf_temp_result(via_system_id ,via_network_id ,via_entity_type)
SELECT distinct B.SYSTEM_ID,B.NETWORK_ID,'Cable' FROM top_ring_cable_mapping a
INNER JOIN ATT_DETAILS_CABLE b
ON A.CABLE_ID=B.SYSTEM_ID
where a.ring_id=p_ring_id  ;

update cpf_temp_result set source_system_id=a.system_id,
source_entity_type=a.entity_type,
source_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on --(st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2))
st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2))
and pm.entity_type in('SpliceClosure','FDB','POD','BDB')
where a.source_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and source_system_id is null;

update cpf_temp_result set destination_system_id=a.system_id,
destination_entity_type=a.entity_type,
destination_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id
from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on (st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2)))
and pm.entity_type in('SpliceClosure','FDB','POD','BDB')
where a.destination_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and destination_system_id is null;

--delete from cpf_temp_result where source_network_id=destination_network_id;

 
insert into temp_nodes(source_system_id,source_network_id,source_entity_type)
select source_system_id,source_network_id,source_entity_type  from(
select * from(
select source_system_id,source_network_id,source_entity_type 
	from cpf_temp_result 
union
select destination_system_id,destination_network_id,destination_entity_type 
	from cpf_temp_result)b
	
)t group by source_system_id,source_network_id,source_entity_type;

update temp_nodes set node_name=fms.fms_name
from att_details_fms fms where source_system_id=fms.system_id and source_entity_type='FMS';

update temp_nodes set node_name=fms.spliceclosure_name
from att_details_spliceclosure fms where source_system_id=fms.system_id and source_entity_type='SpliceClosure';

--UPDATE FDB Nikhil Arora
update temp_nodes set node_name=fdb.fdb_name
from isp_fdb_info fdb where source_system_id=fdb.system_id and source_entity_type='FDB';

--END FDB UPDATE

insert into temp_edges(from_id,from_entity_type,
to_id,to_entity_type,label,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length)

	select source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,
network_status,cable_type,color_code,total_core,cable_category,
cable_calculated_length
from(
	select coalesce(source_system_id,cbl.a_system_id)as source_system_id, 
	coalesce(source_entity_type,a_entity_type) as source_entity_type,
coalesce(destination_system_id,b_system_id) as destination_system_id,
coalesce(destination_entity_type,b_entity_type) as destination_entity_type,via_network_id,
cbl.network_status,cbl.cable_type,cbl.total_core,cbl.cable_category,cbls.color_code,
Cast(cbl.cable_calculated_length as decimal(10,2))  as cable_calculated_length
from cpf_temp_result cpf
inner join att_details_cable cbl on cbl.system_id=cpf.via_system_id
left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type 
and cbls.cable_category=cbl.cable_category and cbls.fiber_count=cbl.total_core
)a
group by source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length;

   
   -- NODES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select * from(
select row_number() over(partition by id) rn,* from(
select distinct * from (
select CONCAT(source_system_id,source_entity_type) as id,
CONCAT('<b>',replace(node_name,E'\t',''),'</b>','\n',case when ld.is_virtual_port_allowed=false then concat('(',connected_ports,')') end) as label

,source_entity_type as group, null as title from temp_nodes t
	inner join layer_details ld on ld.layer_name=t.source_entity_type where source_entity_type!='Splitter'

) a)b)b where rn=1
) x) into nodesDownstream;

-- EDGES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select CONCAT(t.from_id,t.from_entity_type) as from,
CONCAT(t.to_id,t.to_entity_type) as to,'<b>'
||(case when t.total_core is not null then '('||t.total_core||'F)' else '' end)||
concat(' (',t.cable_calculated_length::text,' m)') as label,

case WHEN t.color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '{"color": "#FF0000"}'::json
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '{"color": "#0000FF"}'::json
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '{"color": "#FD10FD"}'::json
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '{"color": "#000"}'::json
ELSE '{"color": "#0000FF"}'::json
END
ELSE '{"color": "#000"}'::json
END as color,
0 as length,
CASE
WHEN upper(t.network_status::text) = 'P'::text THEN true
WHEN upper(t.network_status::text) = 'A'::text THEN false
ELSE true
END as dashes,
1.5  as width,
concat('{"type": "''curvedCCW''", "roundness":"0.5"}')::json as smooth
from temp_edges t
) x) into edgesDownstream;
   
   --LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (

select distinct * from (
select source_entity_type as entity_type, 
(select layer_title from layer_details where layer_name=source_entity_type) as entity_title,false as upstream 
from temp_nodes
) a where a.entity_type not in ('Cable','Splitter') order by entity_title
) x) into legends;

--CABLE LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
Select case WHEN color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '#FF0000'::text
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '#0000FF'::text
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '#FD10FD'::text
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '#000'::text
ELSE '#0000FF'::text
END
ELSE t.color_code::text
END as color_code,CONCAT(COALESCE(t.cable_category,''),
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN ' OH'
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN ' UG'
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN ' WC'
WHEN upper(t.cable_type::text) = 'ISP'::text THEN ' ISP'
ELSE ''
END ,
(case when total_core is not null then '('||t.total_core||'F)' else '' end))as text,false as upstream
from(
select color_code,cable_category,cable_type,total_core from temp_edges
group by color_code,cable_category,cable_type,total_core )t where t.cable_type is not null
) x) into cables;
   
	return query select row_to_json(result) from (
select '' as entityType, '' as entityTitle,'' as entityDisplayText,
0 as latitude,0 as longitude,
now(),null as nodes,null as edges,nodesDownstream,edgesDownstream,legends,cables
) result;
 

END 
$BODY$;

ALTER FUNCTION public.fn_get_ring_schematicview(integer)
    OWNER TO postgres;

    ------------------------------------------------Modify existing function to get segment details list view -------------------------------------------

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
		system_id integer,
		distance numeric
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
	RAISE INFO 'sitedistance -> %',ROUND(ST_DistanceSphere(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), ST_GeomFromText('POINT(' || p_site2_geom || ')', 4326))::numeric, 2);
  

     -- Populate temporary cable routes table
      INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id,system_id,distance)
		   
	  SELECT seq, path_seq, edge,p_user_id,p_system_id,ROUND(ST_DistanceSphere(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), ST_GeomFromText('POINT(' || p_site2_geom || ')', 4326))::numeric, 2) AS sitedistance FROM pgr_ksp('SELECT id, source, target, cost, reverse_cost 
	  FROM routing_data_core_plan', (SELECT pgr.id
	  FROM routing_data_core_plan_vertices_pgr pgr
	  WHERE ST_Within( pgr.the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), 2))
	  limit 1 ),(SELECT pgr.id FROM routing_data_core_plan_vertices_pgr pgr
	  WHERE ST_Within( pgr.the_geom, 
	  ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_site2_geom ||')', 4326), 2)) 
	  limit 1),5);

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
			left join att_details_pod adp on adp.system_id= s.agg1_site_id-- AND EXISTS ( SELECT 1 FROM temp_cable_routes tcr WHERE tcr.system_id = s.agg1_site_id GROUP BY tcr.system_id HAVING COUNT(*) > 1)
            left join att_details_pod adp2 on adp2.system_id = s.agg2_site_id-- AND EXISTS ( SELECT 1 FROM temp_cable_routes tcr WHERE tcr.system_id = s.agg2_site_id GROUP BY tcr.system_id HAVING COUNT(*) > 1)
            INNER JOIN temp_cable_routes tc ON tc.edge_targetid = cbl.system_id;
           

END;
$BODY$;

ALTER FUNCTION public.fn_topology_get_segmentdetailssitewise(integer, integer)
    OWNER TO postgres;

    --------------------------------------- Modify existing function to get  connected route element -------------------------------------------

    DROP FUNCTION IF EXISTS public.fn_get_route_connectedelement_details(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_get_route_connectedelement_details(
	p_route_id integer,
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
	

Raise info 'p_route_id ->%',p_route_id;
 
 -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom_route(site_id, site_name, geom,ringid, user_id,cbl_distance)
     select distinct sitedetails.system_id , sitedetails.site_name , ST_GeomFromText('POINT(' || sitedetails.longitude || ' ' || sitedetails.latitude || ')', 4326),sitedetails.ring_id,p_user_id,sitedetails.length_meters 
	 from  (select pod.system_id,pod.site_name,pod.longitude,pod.latitude,pod.ring_id,0 AS length_meters  
			from att_details_pod pod
    where pod.system_id in (SELECT agg1site_id AS site_id FROM top_routes WHERE route_id = p_route_id
UNION ALL
SELECT agg2site_id AS site_id FROM top_routes WHERE route_id = p_route_id)) sitedetails;
	

insert into temp_cableInfo(cable_system_id,cable_network_id,cable_type,cable_geom)
select distinct lm.system_id,cbl.network_id,lm.entity_type,st_astext(lm.sp_geometry) as connected_entity_geom 
from att_details_cable cbl 
		inner join line_master lm ON  upper(lm.entity_type)='CABLE' and cbl.system_id=lm.system_id
		where cbl.network_id in (select unnest(string_to_array(regexp_replace(cable_names, '[\{\}\n\r]+', ',', 'g'), ',')) AS code from top_routes where route_id=p_route_id) ;

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

ALTER FUNCTION public.fn_get_route_connectedelement_details(integer, integer)
    OWNER TO postgres;

