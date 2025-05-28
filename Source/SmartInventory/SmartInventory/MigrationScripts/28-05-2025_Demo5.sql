
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

