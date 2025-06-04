
-------------------------------------------------Insert into res resources - Start -----------------------------------------------------------
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('de-DE', 'SI_OSP_GBL_GBL_GBL_326', 'Choose Action Cable', false, true, 'German', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-05-30 16:29:12.244', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_GBL_GBL_GBL_326', 'Choose Action Cable', true, true, 'English', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-05-30 16:29:12.244', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en-US', 'SI_OSP_GBL_GBL_GBL_326', 'Choose Action Cable', false, true, NULL, 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-05-30 16:29:12.244', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('fr', 'SI_OSP_GBL_GBL_GBL_326', 'Choose Action Cable', false, true, 'French', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-05-30 16:29:12.244', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_GBL_GBL_GBL_326', 'Choose Action Cable', false, true, 'Hindi', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-05-30 16:29:12.244', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ja-JP', 'SI_OSP_GBL_GBL_GBL_326', 'Choose Action Cable', false, true, 'Japanese', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-05-30 16:29:12.244', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ru-RU', 'SI_OSP_GBL_GBL_GBL_326', 'Choose Action Cable', false, true, 'Russian', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-05-30 16:29:12.244', 5, true, false);

-------------------------------------------------Delete duplicate data from the table-------------------------------------------------------

DELETE FROM fiber_link_status_master a
USING fiber_link_status_master b
WHERE
  a.ctid < b.ctid
  AND a.name = b.name
  AND a.id = b.id;

  --------------------------------------------------------Modify the view as required----------------------
 DROP FUNCTION IF EXISTS public.fn_get_fiber_link_status(integer, character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone);

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_status(
	p_systemid integer,
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_userid integer,
	p_searchfrom timestamp without time zone,
	p_searchto timestamp without time zone)
    RETURNS TABLE(fiber_link_status character varying, color_code character varying, fiber_link_count integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
Declare v_user_role_id integer;
 sql TEXT; 
BEGIN
sql:='';
select role_id into v_user_role_id from user_master where user_id=p_userid;

 

sql:= 'select tsm.name as fiber_link_status,tsm.color_code ,count(fl.system_id)::integer as fiber_link_count from vw_att_details_fiber_link fl right join 
fiber_link_status_master tsm on upper(tsm.name)=upper(fl.fiber_link_status) '; 

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('|| 'fl.'||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';
END IF;
 
sql:= sql || ' where tsm.is_active=true group by tsm.name,tsm.color_code ';
raise info'final%',sql;
RETURN QUERY
EXECUTE 'select  fiber_link_status, color_code, fiber_link_count from ('||sql||') row';

END ;
$BODY$;

ALTER FUNCTION public.fn_get_fiber_link_status(integer, character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone)
    OWNER TO postgres;



----------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_splicing_network_diagram(p_system_id integer, p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE nodes text;
 edges text; 
 legends text;
 checkbox text;
 v_layer_title character varying;
 v_layer_table character varying;
 v_entity_display_text character varying;
 arow record;
BEGIN

create temp table temp_vis_result
(
   row_id serial,
   system_id integer,
   entity_type character varying,
   entity_title character varying,  
   associated_system_id integer,
   associated_entity_type character varying,
   associated_entity_title character varying,
   associated_network_id character varying,
   cable_system_id integer,
   cable_network_id character varying,
   cable_label_text character varying,
   checkbox_entity_type character varying,
   other_end_system_id integer,  
   other_end_entity_type character varying,
   other_end_entity_title character varying,
   other_end_network_id character varying
   
) on commit drop;

-- GET ENTITY DISPLAY TEXT--
select layer_title,layer_table into v_layer_title,v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
execute 'select concat(network_id,''-('',network_status,'')'') from '||v_layer_table||' where system_id='||p_system_id||' limit 1 ' into  v_entity_display_text;


-- GET ALL ASSOCIATED ELEMENTS FOR ENTITY TYPE..
FOR arow IN  
select a.*,ld.layer_title as associated_entity_title  from (         
    select destination_system_id as associated_system_id, source_network_id as network_id, 
    destination_network_id as associated_network_id,destination_entity_type as associated_entity_type from connection_info
    where upper(source_entity_type)=upper(p_entity_type) and  source_system_id=p_system_id  
    UNION

     select source_system_id , destination_network_id as network_id,
    source_network_id ,source_entity_type  from connection_info
    where upper(destination_entity_type)=upper(p_entity_type) and  destination_system_id=p_system_id  
    
) a inner join layer_details ld on upper(ld.layer_name)=upper(a.associated_entity_type)

LOOP
    -- GET ASSOCIATED CABLES FOR ENTITY ASSOCIATED WITH MANHOLE--
    if( p_entity_type = 'SpliceClosure')
    then
    insert into temp_vis_result(system_id,entity_type,entity_title,associated_system_id,associated_entity_type,associated_entity_title,associated_network_id,
        cable_system_id,cable_network_id,cable_label_text,other_end_system_id,other_end_entity_type,other_end_network_id,other_end_entity_title,checkbox_entity_type)
        select p_system_id as system_id,p_entity_type as entity_type,v_layer_title as entity_title,arow.associated_system_id,
        arow.associated_entity_title,arow.associated_entity_type,arow.associated_network_id, b.cable_system_id,b.cable_network_id,b.cable_label_text,
        b.other_end_system_id,b.other_end_entity_type,b.other_end_network_id,ldnew.layer_title as other_end_entity_title,'Cable' as checkbox_entity_type
         from (
            select system_id as cable_system_id,network_id as cable_network_id,
            CONCAT(network_id,'-(',total_core,'F)-(',network_status,')') as cable_label_text ,
            (case when p_system_id=b_system_id then null else b_system_id end)  as other_end_system_id,
            (case when p_system_id=b_system_id then null else b_entity_type end) as other_end_entity_type,
            (case when p_system_id=b_system_id then null else b_location end) as other_end_network_id 
            from att_details_cable where system_id=arow.associated_system_id     
         ) b left join layer_details ldnew on upper(ldnew.layer_name)=upper(b.other_end_entity_type);

end if;

    
END LOOP;

-- NODES JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
    select p_system_id as id, v_layer_title as label,p_entity_type as group, 8 as value,'' as checkbox_entity_type
    union
    select  row_id as id, CONCAT(other_end_entity_title,'(',other_end_network_id,')')  as label, other_end_entity_type as group, 1 as value,checkbox_entity_type from temp_vis_result    
) x) into nodes;

-- EDGES JSON--
 Select (select array_to_json(array_agg(row_to_json(x)))from (
    select a.*,250 as length,
    'gray' as color,'gray' as fontcolor,'{"align": "top"}'::json as font,5 as width 
    from (
        select 0 as from,p_system_id as to,'' as label,'' as associated_entity_type,'' as checkbox_entity_type
        union
        select p_system_id as from,row_id as to,cable_label_text as label,associated_entity_type,checkbox_entity_type from temp_vis_result
    ) a
) x) into edges;

--LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (

    select distinct * from (
        select p_entity_type as entity_type, v_layer_title as entity_title 
        union
        select other_end_entity_type,other_end_entity_title from temp_vis_result     
     ) a
) x) into legends;


--CHECKBOX IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
    

    select checkbox_entity_type, count(checkbox_entity_type),'gray'  as color
from temp_vis_result t
group by checkbox_entity_type
) x) into checkbox;

return query select row_to_json(result) from (
select p_entity_type as entityType, v_layer_title as entityTitle,v_entity_display_text as entityDisplayText, nodes,edges,legends,checkbox
) result;
END; 
$function$
;

-----------------------------------------------------------------------------------

 DROP FUNCTION IF EXISTS public.fn_topology_get_selectedroute(character varying, integer, integer, integer);

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

    -- Convert input string to a valid LINESTRING geometry
    

 -- Buffer in meters: Convert to geography, buffer, then back to geometry
    v_buffered_route := st_buffer_meters(
                                ST_GeomFromText('LINESTRING(' || route_geom || ')', 4326),  
                                2
                            );
                        
			RAISE INFO 'v_buffered_route -> %', v_buffered_route;			
   
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
	 IF v_Count > 0 THEN
	 
	 delete from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2 and tr.route_name='Manual_Route';
	 
	insert into top_routes(route_name,cable_names,total_length,route_geom,agg1site_id,agg2site_id)
	
	SELECT 
  'Manual_Route' AS route_name,
  (select '{' || string_agg(network_id, ',') || '}' from temp_cable) as cable_names,
  0,
  v_source_geom AS  route_geom,
 p_agg1,
 p_agg2
 ;
 END IF;
 
 
 -- Return matching cable lines within 3 meters
   
    RETURN QUERY
    select Max(tr.route_id) AS route_id,tr.route_name from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2 and tr.route_name='Manual_Route'
    GROUP BY tr.route_name order by 1 desc ;

END;
$BODY$;

ALTER FUNCTION public.fn_topology_get_selectedroute(character varying, integer, integer, integer)
    OWNER TO postgres;


    ---------------------------------------------------Modify the function----------------------------------------


    ----------------------------------------------------------------------


DROP FUNCTION IF EXISTS public.fn_validate_geom_update(integer, character varying, character varying, integer, character varying, integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_validate_geom_update(
	p_system_id integer,
	p_geom_type character varying,
	p_entity_type character varying,
	p_userid integer,
	p_longlat character varying,
	p_bld_buffer integer,
	p_source_ref_type character varying,
	p_source_ref_id character varying)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
v_Query text;
cnt_1 integer default 0;
cnt_2 integer default 0;
result integer default 0;
geom character varying(250);
geo_data text;
s_longLat character varying;
v_old_child_element integer default 0;
v_new_child_element integer default 0;
v_new_child_element_plm integer default 0;
v_new_child_element_lm integer default 0;
v_new_child_element_pm integer default 0;
v_entity_network_id character varying;
v_total_child integer;
V_MESSAGE CHARACTER VARYING;
V_LAYER_TITLE character varying;
V_LAYER_TABLE character varying;
V_IS_ASSOCIATED bool;
v_old_start_geom geometry;
v_old_end_geom geometry;
v_new_start_geom geometry;
v_new_end_geom geometry;
v_spliced_a_end_entity_geom geometry;
v_spliced_b_end_entity_geom geometry;
v_a_system_id integer;
v_a_entity_type character varying;
v_b_system_id integer;
v_b_entity_type character varying;
v_is_virtual boolean;
v_parent_system_id integer;
V_LAYER_MAPPING record;
V_MAPPED_PARENT record;
V_STATUS boolean;
V_GEOM_TYPE CHARACTER VARYING;
v_is_shifting_allowed boolean;
v_associated_entity_type CHARACTER VARYING;
v_is_snapping_allowed boolean;
v_arow record;
v_associate_entity_geom character varying;
v_geom geometry;
V_PARENT_SEQUENCE int;
PARENT_OUTER_BOUNDARIES character varying;
PARENT_INNER_BOUNDARIES character varying;
V_INNER_ENTITY_TYPE character varying;
V_OUTER_ENTITY_TYPE character varying;
V_IS_INNER_ENTITITES boolean;
V_IS_OUTER_ENTITITES boolean;
V_PARENT_LAYER_TITLE character varying;
v_province_id integer;
v_old_a_distance double precision;
v_new_a_distance double precision;
v_old_b_distance double precision;
v_new_b_distance double precision;
BEGIN
v_Query:='';
V_IS_ASSOCIATED:=false;
v_is_snapping_allowed:=false;
v_is_virtual:=false;
V_STATUS:=true;
V_MESSAGE:='';
--GET THE LAYER TITLE
SELECT layer_title,layer_table into V_LAYER_TITLE,V_LAYER_TABLE FROM layer_details where upper(layer_name)=upper(p_entity_type);
SELECT * INTO V_LAYER_MAPPING FROM VW_LAYER_MAPPING WHERE (UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE)
AND (UPPER(PARENT_GEOM_TYPE)='POLYGON' OR (UPPER(PARENT_LAYER_NAME)=UPPER('PROVINCE'))) and IS_USED_FOR_NETWORK_ID=true) order by parent_sequence desc limit 1;

IF UPPER(p_geom_type)='POINT'
THEN
V_GEOM_TYPE:='POINT('||P_LONGLAT||')';
ELSIF UPPER(p_geom_type)='LINE'
THEN
V_GEOM_TYPE:='LINESTRING('||P_LONGLAT||')';
ELSE
V_GEOM_TYPE:='POLYGON(('||P_LONGLAT||'))';
END IF;

if COALESCE(p_source_ref_type,'')!='' and COALESCE(p_source_ref_id,'')!='' 
then
	if exists(
	
select 1 from polygon_master pm where system_id = p_source_ref_id::integer and Upper(entity_type)='NETWORK_TICKET' and st_within(st_geomfromtext(V_GEOM_TYPE,4326),pm.sp_geometry)=false
	)
	then
		V_MESSAGE := 'Entity can not be created out side the assigned ticket area. Please contact your administrator !';
		V_STATUS:=false;
	end if;
end if;

if (V_STATUS = true)
then
IF COALESCE(V_LAYER_MAPPING.PARENT_LAYER_NAME,'')!='' AND (UPPER(V_LAYER_MAPPING.CHILD_LAYER_NAME)!='PROVINCE')
THEN

IF(UPPER(P_GEOM_TYPE)='POLYGON')
THEN
SELECT PARENT_SEQUENCE INTO V_PARENT_SEQUENCE FROM VW_LAYER_MAPPING WHERE UPPER(PARENT_LAYER_NAME)=UPPER(P_ENTITY_TYPE) LIMIT 1;
IF(UPPER(P_ENTITY_TYPE)='AREA') THEN
V_PARENT_SEQUENCE:=1;
END IF;

SELECT STRING_AGG(PARENT_LAYER_NAME, ',') INTO PARENT_OUTER_BOUNDARIES FROM VW_LAYER_MAPPING WHERE PARENT_SEQUENCE < V_PARENT_SEQUENCE
AND UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE);
SELECT STRING_AGG(PARENT_LAYER_NAME, ',') INTO PARENT_INNER_BOUNDARIES FROM VW_LAYER_MAPPING WHERE PARENT_SEQUENCE > V_PARENT_SEQUENCE
AND UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE);

-------------------ANTRA POLYGON ENTITIES CHANGES-----------------------

--INTERSECTING WITH INNER BOUNDARY
SELECT ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326) into v_geom;
SELECT COUNT(1) > 0 ,F.ENTITY_TYPE INTO V_IS_INNER_ENTITITES,V_INNER_ENTITY_TYPE FROM POLYGON_MASTER F WHERE F.ENTITY_TYPE IN (
SELECT UNNEST(STRING_TO_ARRAY(PARENT_INNER_BOUNDARIES, ','))) AND ST_INTERSECTS(F.SP_GEOMETRY,V_GEOM) AND ST_TOUCHES(F.SP_GEOMETRY, V_GEOM) = FALSE
AND NOT ST_WITHIN(F.SP_GEOMETRY, V_GEOM) GROUP BY ENTITY_TYPE;

IF (V_IS_INNER_ENTITITES) THEN
V_STATUS:=FALSE;
V_MESSAGE:=V_LAYER_TITLE||' [SI_OSP_GBL_GBL_GBL_317] '||(SELECT LAYER_TITLE FROM LAYER_DETAILS WHERE LAYER_NAME=V_INNER_ENTITY_TYPE)||' boundary!';
RETURN QUERY
select V_STATUS as status, V_MESSAGE::character varying as message;
END IF;

--INTERSECTING WITH OUTER BOUNDARY
SELECT COUNT(1) > 0 ,F.ENTITY_TYPE INTO V_IS_OUTER_ENTITITES,V_OUTER_ENTITY_TYPE FROM POLYGON_MASTER F WHERE F.ENTITY_TYPE IN (
SELECT UNNEST(STRING_TO_ARRAY(PARENT_OUTER_BOUNDARIES, ','))) AND ST_INTERSECTS(V_GEOM, F.SP_GEOMETRY)
AND ST_TOUCHES(V_GEOM, F.SP_GEOMETRY) = FALSE AND NOT ST_WITHIN(V_GEOM, F.SP_GEOMETRY) GROUP BY ENTITY_TYPE;

IF (V_IS_OUTER_ENTITITES) THEN
V_STATUS:=FALSE;
V_MESSAGE:=V_LAYER_TITLE||' [SI_OSP_GBL_GBL_GBL_318] '||(SELECT LAYER_TITLE FROM LAYER_DETAILS WHERE LAYER_NAME=V_OUTER_ENTITY_TYPE)||' only!';
RETURN QUERY
select V_STATUS as status, V_MESSAGE::character varying as message;
END IF;

END IF;

--VALIDATE JP BOUNDRY AND CITY BOUNDRY
if ((select value from global_settings where upper(key)=upper('ISVALIDATECITYBOUNDARY'))::integer=1 and upper(p_entity_type) in ('AREA','SUBAREA'))
Then

if not EXISTS(SELECT substring(left(St_astext(rcb.geom),-3),16) FROM rjio_city_boundary rcb
where ST_WITHIN(ST_Centroid(ST_GeomFromText(('POLYGON (('||p_longlat||'))'),4326)),rcb.geom) limit 1)
Then
raise info '-----------------V_IS_ASSOCIATED%',p_longlat;
V_STATUS:=FALSE;
V_MESSAGE:='Drawn boundary must be within City Boundary!';
RETURN QUERY
select V_STATUS as status, V_MESSAGE::character varying as message;
END IF;
END IF;
if ((select value from global_settings where upper(key)=upper('ISVALIDATEJPBOUNDARY'))::integer=1 and upper(p_entity_type)=upper('SubArea'))
Then
if not EXISTS(SELECT substring(left(St_astext(rjp.geom),-3),16) FROM rjio_jp_boundary rjp
where ST_WITHIN(ST_GeomFromText('POLYGON (('||p_longlat||'))',4326), rjp.geom) limit 1)
Then
V_STATUS:=FALSE;
V_MESSAGE:='Drawn boundary must be within JP Boundary!';
RETURN QUERY
select V_STATUS as status, V_MESSAGE::character varying as message;
END IF;
END IF;
--------------------------------------END----------------------------------

ELSIF UPPER(p_geom_type)!='LINE' AND EXISTS(SELECT 1 FROM VW_LAYER_MAPPING WHERE (UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE) AND UPPER(PARENT_LAYER_NAME)='PROVINCE' and IS_USED_FOR_NETWORK_ID=true))
THEN
v_Query='SELECT PB.id FROM PROVINCE_BOUNDARY PB WHERE ST_Within(ST_GeomFromText('''||V_GEOM_TYPE||''',4326),PB.SP_GEOMETRY) ';
if(UPPER(P_ENTITY_TYPE)='NETWORK_TICKET')
then
v_Query:=v_Query||' and PB.id= (SELECT aa.province_id FROM '||V_LAYER_TABLE||' aa WHERE ticket_id='||P_SYSTEM_ID||')';
else
v_Query:=v_Query||' and PB.id= (SELECT aa.province_id FROM '||V_LAYER_TABLE||' aa WHERE system_id='||P_SYSTEM_ID||')';
end if;
EXECUTE V_QUERY INTO V_MAPPED_PARENT;
IF V_MAPPED_PARENT IS NULL
THEN
V_STATUS:=FALSE;
V_MESSAGE:=V_LAYER_TITLE||' [SI_OSP_GBL_GBL_GBL_319]'; --is outside the Province boundary';
RETURN QUERY
select V_STATUS as status, V_MESSAGE::character varying as message;
END IF;

END IF;

select case when count(1)>0 then true else false end into V_IS_ASSOCIATED
from associate_entity_info where ((entity_system_id=p_system_id and upper(entity_type)=upper(p_entity_type))
or (associated_system_id=p_system_id and upper(associated_entity_type)=upper(p_entity_type))) and is_termination_point=false;

raise info 'V_IS_ASSOCIATED%',V_IS_ASSOCIATED;

----REGION LOCATION UPDATION FOR Associated Entity Condition By ANTRA 

IF (V_IS_ASSOCIATED) THEN
FOR V_AROW IN SELECT distinct(associated_entity_type) from associate_entity_info
WHERE entity_system_id=p_system_id and upper(entity_type)=upper(p_entity_type) and is_termination_point=false
UNION
SELECT distinct(entity_type) from associate_entity_info
WHERE associated_system_id=p_system_id and upper(associated_entity_type)=upper(p_entity_type) and is_termination_point=false order by associated_entity_type

LOOP
--raise info 'V_AROW.associated_entity_type%',V_AROW.associated_entity_type;

--raise info 'p_entity_type%',p_entity_type;

select geom_type into v_associate_entity_geom from layer_details where upper(layer_name)= upper(V_AROW.associated_entity_type);
raise info 'v_associate_entity_geom%',v_associate_entity_geom;

SELECT is_shifting_allowed into v_is_shifting_allowed from associate_entity_master where layer_id=(select layer_id from layer_details
where upper(layer_name)=upper(p_entity_type)) and associate_layer_id = (select layer_id from layer_details
where upper(layer_name)=upper(V_AROW.associated_entity_type:: character varying));

raise info 'v_is_shifting_allowed%',v_is_shifting_allowed;

IF(v_is_shifting_allowed = true AND UPPER(v_associate_entity_geom) ='LINE') THEN
SELECT is_snapping_enabled into v_is_snapping_allowed from associate_entity_master where layer_id=(select layer_id from layer_details
where upper(layer_name)=upper(p_entity_type)) and associate_layer_id = (select layer_id from layer_details
where upper(layer_name)=upper(V_AROW.associated_entity_type:: character varying));

raise info 'v_is_snapping_allowed%',v_is_snapping_allowed;

IF(v_is_shifting_allowed = false OR v_is_snapping_allowed =false) THEN
RETURN QUERY SELECT FALSE AS STATUS, (V_LAYER_TITLE||'[SI_GBL_GBL_GBL_GBL_049]')::CHARACTER VARYING AS MESSAGE;
--location can not be changed, as it is associated with some entity!
END IF;

ELSE

IF(v_is_shifting_allowed = false) THEN
RETURN QUERY SELECT FALSE AS STATUS, (V_LAYER_TITLE||'[SI_GBL_GBL_GBL_GBL_049]')::CHARACTER VARYING AS MESSAGE;
--location can not be changed, as it is associated with some entity!
END IF;
END IF;

END LOOP;
END IF;

--END REGION

---------------Start Topology entity---------------------------------------

if (UPPER(P_ENTITY_TYPE) = 'POD' or UPPER(P_ENTITY_TYPE) = 'CABLE') and (SELECT SUM(count_val) AS total_count
FROM (
    SELECT COUNT(ts.id) AS count_val
    FROM top_segment ts
    inner JOIN top_segment_cable_mapping tscm ON tscm.segment_id = ts.id 
    WHERE agg1_site_id = p_system_id 
       OR agg2_site_id = p_system_id 
       OR tscm.cable_id = p_system_id

    UNION ALL

    SELECT COUNT(trsm.id) AS count_val
    FROM top_ring_site_mapping trsm  
    WHERE site_id = p_system_id
) AS sub)  > 0 then

RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_437]'::character varying as message;--Location cannot be changed, as it is associated with topology process!
END IF;

---------------Start Topology entity---------------------------------------

IF UPPER(P_ENTITY_TYPE) = 'BUILDING'
THEN
IF UPPER(P_GEOM_TYPE)='POLYGON'
THEN
--GET COUNT OF EXISTING ENTITIES INSIDE BUIDLING POLYGON ( AS PER OLD GEOMETRY)
select count(*) into cnt_1 from point_master pnt
where ST_Within(pnt.sp_geometry,(select sp_geometry from polygon_master where system_id=p_system_id and lower(entity_type)=lower(p_entity_type)))
and pnt.entity_type='Structure';

--GET COUNT OF EXISTING ENTITIES INSIDE BUIDLING POLYGON ( AS PER OLD GEOMETRY)
select count(*) into cnt_2 from point_master pnt
where ST_Within(pnt.sp_geometry,ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))and pnt.entity_type='Structure';

IF cnt_1 <> cnt_2
THEN
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_023]'::character varying as message;--Child entity count have increased or dicreased! 
END IF;

select count(1) into v_total_child from att_details_bld_structure str
inner join polygon_master pm on pm.system_id =str.system_id and upper(pm.entity_type)=upper('structure')
where parent_system_id=p_system_id and upper(parent_entity_type)=upper('building');
if((SELECT count(1) FROM polygon_master PM WHERE (ST_Within(PM.SP_GEOMETRY,ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))
or PM.SP_GEOMETRY=ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)) and lower(entity_type)=lower('structure'))<v_total_child)
THEN
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_026]'::character varying as message;--Child entity cannot be outside from building!
END IF;
RETURN QUERY
select * from fn_validate_building_geom(p_geom_type, p_longlat, p_userid, p_bld_buffer, p_system_id ) limit 1;
END IF;

ELSIF UPPER(p_entity_type) = 'STRUCTURE'
THEN
select bld.is_virtual,str.parent_system_id into v_is_virtual,v_parent_system_id
from att_details_building bld
inner join att_details_bld_structure str
on bld.system_id=str.parent_system_id and upper(str.parent_entity_type)=upper('Building')
where str.system_id=p_system_id;

IF upper(p_geom_type)='POINT' and EXISTS(select 1 from polygon_master PM WHERE ST_Within(ST_GeomFromText('POINT('||p_longlat||')',4326),PM.SP_GEOMETRY)
and lower(entity_type)='building' and PM.system_id!=v_parent_system_id and is_virtual=false)
THEN
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_024]'::character varying as message; --Structure location should be inside mapped building only!

ELSIF UPPER(P_GEOM_TYPE)='POLYGON'
AND EXISTS(SELECT 1 FROM POLYGON_MASTER PM WHERE ST_Within(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),PM.SP_GEOMETRY) AND LOWER(ENTITY_TYPE)='building'
AND PM.SYSTEM_ID!=V_PARENT_SYSTEM_ID AND IS_VIRTUAL=FALSE)
THEN
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_024]'::character varying as message; --Structure location should be inside mapped building only! 

ELSIF upper(p_geom_type)='POINT'
THEN
IF NOT EXISTS ( SELECT 1 FROM att_details_bld_structure where system_id=p_system_id
and building_id=( select PM.system_id from polygon_master PM WHERE ST_Within(ST_GeomFromText('POINT('||p_longlat||')',4326),PM.SP_GEOMETRY)
and lower(entity_type)='building' and is_virtual=v_is_virtual limit 1) ) THEN

RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_024]'::character varying as message;--Structure location should be inside mapped building only!
elsif(select count(1)>0 from att_details_cable where structure_id=p_system_id and (cable_type='Underground' or cable_type='Overhead'))then
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_027]'::character varying as message; --Structure can not move,as entities are used as terimination point in OSP!
END IF;
ELSIF upper(p_geom_type)='POLYGON' and NOT EXISTS (SELECT 1 FROM att_details_bld_structure str
where str.system_id=p_system_id and parent_system_id=
(
select PM.system_id from polygon_master PM WHERE ST_Within(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),PM.SP_GEOMETRY) and lower(entity_type)='building'
and is_virtual=v_is_virtual limit 1

) and lower(str.parent_entity_type)=lower('Building')) THEN

RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_028]'::character varying as message;--Structure is outside the mapped building boundary!
END IF;

RETURN QUERY
select * from fn_get_structure_validate(0,p_longlat,p_system_id,p_geom_type) a limit 1;
ELSIF UPPER(p_entity_type) = 'TOWER'
THEN
if exists(select 1 from att_details_tower where system_id=p_system_id and upper(parent_entity_type)='BUILDING') then
IF not EXISTS(select 1 from polygon_master PM WHERE ST_Within(ST_GeomFromText('POINT('||p_longlat||')',4326),PM.SP_GEOMETRY) and lower(entity_type)='building')
THEN
--raise info 'inside if block';
RETURN QUERY select false as status, 'Tower location should be inside mapped building only!'::character varying as message;
--Structure location should be inside mapped building only!
end if;
end if;
ELSIF UPPER(p_entity_type) = 'AREA'
THEN
select count(1) into v_total_child from att_details_subarea where parent_system_id=p_system_id and upper(parent_entity_type)=upper('area');
IF EXISTS(SELECT PB.system_id FROM Polygon_master PB WHERE ST_Intersects(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),PB.SP_GEOMETRY)
and entity_type='Area' and PB.system_id!=p_system_id )
THEN

RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_030]'::character varying as message;--Area is overlapping with other area boundary!
END IF;

if((SELECT count(1) FROM polygon_master PM WHERE (ST_Within(PM.SP_GEOMETRY,ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)) or PM.SP_GEOMETRY=ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)) and lower(entity_type)=lower('subarea'))<v_total_child)THEN

RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_031]'::character varying as message;--Child entity cannot be outside from Area!
END IF;

ELSIF UPPER(p_entity_type) = 'SUBAREA'
THEN
if exists(SELECT PB.system_id FROM Polygon_master PB WHERE ST_Intersects(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),PB.SP_GEOMETRY) and upper(entity_type)=upper('SubArea') and PB.system_id!=p_system_id)
then
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_033]'::character varying as message;--Selected boundary is overlapping other SubArea boundary!!
--ELSIF EXISTS(SELECT PB.system_id FROM Polygon_master PB WHERE upper(entity_type)='SUBAREA' and PB.system_id=p_system_id and coalesce(pb.gis_design_id,'')!='') THEN

--RETURN QUERY
--select false as status, 'Codification is done for this FSA, geometry change is not allowed'::character varying as message;

ELSE
SELECT count(1) into v_old_child_element FROM polygon_master PM,point_master ptm WHERE ST_Within(ptm.SP_GEOMETRY, PM.SP_GEOMETRY)
and PM.system_id=p_system_id and upper(PM.entity_type)='SUBAREA' and(upper(ptm.entity_type)
in (select upper(ld.layer_name) from layer_mapping lm
join layer_details ld on lm.layer_id=ld.layer_id where lm.parent_layer_id=(select layer_id from layer_details where upper(layer_name)='SUBAREA')));

SELECT count(1) into v_new_child_element FROM point_master ptm
WHERE ST_Within(ptm.SP_GEOMETRY, ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))
and(upper(ptm.entity_type)in (select upper(ld.layer_name) from layer_mapping lm join layer_details ld on lm.layer_id=ld.layer_id
where lm.parent_layer_id=(select layer_id from layer_details where upper(layer_name)='SUBAREA')));

IF(v_new_child_element<v_old_child_element) then
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_034]'::character varying as message;--Child Element is outside the Sub Area boundary!!

END IF;
END IF;
ELSIF UPPER(p_entity_type) = 'DSA'
THEN
select count(1) into v_total_child from att_details_csa where parent_system_id=p_system_id and upper(parent_entity_type)=upper('DSA');
IF EXISTS(SELECT PB.system_id FROM Polygon_master PB WHERE ST_Intersects(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),PB.SP_GEOMETRY)
and entity_type='DSA' and PB.system_id!=p_system_id ) THEN

RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_036]'::character varying as message;--DSA is overlapping with other DSA boundary!
END IF;
--IF EXISTS(SELECT PB.system_id FROM Polygon_master PB WHERE entity_type='DSA' and PB.system_id=p_system_id and coalesce(pb.gis_design_id,'')!='') THEN

--RETURN QUERY
--select false as status, 'Codification is done for this DSA, geometry change is not allowed'::character varying as message;
--END IF;
if((SELECT count(1) FROM polygon_master PM WHERE (ST_Within(PM.SP_GEOMETRY,ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)) or PM.SP_GEOMETRY=ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)) and lower(entity_type)=lower('csa'))<v_total_child)THEN

RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_037]'::character varying as message;--Child entity cannot be outside from DSA!
END IF;
ELSIF UPPER(p_entity_type) = 'CSA'
THEN
if exists(SELECT PB.system_id FROM Polygon_master PB WHERE ST_Intersects(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),PB.SP_GEOMETRY) and upper(entity_type)=upper('CSA') and PB.system_id!=p_system_id)
then
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_039]'::character varying as message;--Selected boundary is overlapping other CSA boundary!!
ELSE

SELECT count(1) into v_old_child_element FROM polygon_master PM,point_master ptm WHERE ST_Within(ptm.SP_GEOMETRY, PM.SP_GEOMETRY)
and PM.system_id=p_system_id and upper(PM.entity_type)='CSA' and(upper(ptm.entity_type)
in (select upper(ld.layer_name) from layer_mapping lm
join layer_details ld on lm.layer_id=ld.layer_id where lm.parent_layer_id=(select layer_id from layer_details where upper(layer_name)='CSA')));

SELECT count(1) into v_new_child_element FROM point_master ptm
WHERE ST_Within(ptm.SP_GEOMETRY, ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))
and(upper(ptm.entity_type)in (select upper(ld.layer_name) from layer_mapping lm join layer_details ld on lm.layer_id=ld.layer_id
where lm.parent_layer_id=(select layer_id from layer_details where upper(layer_name)='CSA')));

IF(v_new_child_element<v_old_child_element) then
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_040]'::character varying as message;--Child Element is outside the CSA boundary!!

END IF;
END IF;
ELSIF UPPER(P_ENTITY_TYPE) != 'CABLE'
AND EXISTS(SELECT 1 FROM CONNECTION_INFO WHERE (SOURCE_SYSTEM_ID = p_system_id AND UPPER(SOURCE_ENTITY_TYPE) = UPPER(P_ENTITY_TYPE)) 
        OR 
        (DESTINATION_SYSTEM_ID = p_system_id AND UPPER(DESTINATION_ENTITY_TYPE) = UPPER(P_ENTITY_TYPE))
)
	then
	
		RETURN QUERY
		select false as status,'Please remove the splicing first to change the location!'::character varying as message;--ROW is already applied!

ELSIF UPPER(P_ENTITY_TYPE) = 'CABLE' AND EXISTS(SELECT 1 FROM CONNECTION_INFO WHERE (SOURCE_SYSTEM_ID=p_system_id AND UPPER(SOURCE_ENTITY_TYPE)='CABLE') OR DESTINATION_SYSTEM_ID=p_system_id AND UPPER(DESTINATION_ENTITY_TYPE)='CABLE')
THEN
select
st_geomfromtext('POINT('||round(ST_X(st_StartPoint(sp_geometry))::numeric,10)||' '||round(ST_Y(st_StartPoint(sp_geometry))::numeric,10)||')',4326),
st_geomfromtext('POINT('||round(ST_X(st_Endpoint(sp_geometry))::numeric,10)||' '||round(ST_Y(st_Endpoint(sp_geometry))::numeric,10)||')',4326)
into v_old_start_geom,v_old_end_geom from line_master where system_id=p_system_id and upper(entity_type)='CABLE';

select
st_geomfromtext('POINT('||round(ST_X(st_StartPoint(ST_GeomFromText('LINESTRING('||p_longlat||')',4326)))::numeric,10)||' '||round(ST_Y(st_StartPoint(ST_GeomFromText('LINESTRING('||p_longlat||')',4326)))::numeric,10)||')',4326),
st_geomfromtext('POINT('||round(ST_X(st_Endpoint(ST_GeomFromText('LINESTRING('||p_longlat||')',4326)))::numeric,10)||' '||round(ST_Y(st_Endpoint(ST_GeomFromText('LINESTRING('||p_longlat||')',4326)))::numeric,10)||')',4326)
into v_new_start_geom,v_new_end_geom;

select v_a_system_id ,v_a_entity_type,v_b_system_id,v_b_entity_type into v_a_system_id ,v_a_entity_type,v_b_system_id,v_b_entity_type from att_details_cable where system_id=p_system_id;

raise info 'v_old_start_geom%',v_old_start_geom;
raise info 'v_new_start_geom%',v_new_start_geom;
SELECT p.SP_GEOMETRY into v_spliced_a_end_entity_geom FROM (
select distinct destination_system_id,DESTINATION_ENTITY_TYPE from CONNECTION_INFO C
where SOURCE_SYSTEM_ID=p_system_id AND UPPER(SOURCE_ENTITY_TYPE)='CABLE' and is_cable_a_end=true
UNION
select distinct SOURCE_SYSTEM_ID,SOURCE_ENTITY_TYPE from CONNECTION_INFO C
where destination_system_id=p_system_id AND UPPER(DESTINATION_ENTITY_TYPE)='CABLE' and is_cable_a_end=true
 )C 
inner join point_master p 
on c.destination_system_id=p.system_id and c.DESTINATION_ENTITY_TYPE=p.entity_type  limit 1;

v_old_a_distance:=(Cast(st_distance(v_spliced_a_end_entity_geom,v_old_start_geom,false)as decimal(10,2)));
v_new_a_distance:=(Cast(st_distance(v_spliced_a_end_entity_geom,v_new_start_geom,false)as decimal(10,2)));

SELECT p.SP_GEOMETRY into v_spliced_b_end_entity_geom FROM (
select distinct destination_system_id,DESTINATION_ENTITY_TYPE from CONNECTION_INFO C
where SOURCE_SYSTEM_ID=p_system_id AND UPPER(SOURCE_ENTITY_TYPE)='CABLE' and is_cable_a_end=false
UNION
select distinct SOURCE_SYSTEM_ID,SOURCE_ENTITY_TYPE from CONNECTION_INFO C
where destination_system_id=p_system_id AND UPPER(DESTINATION_ENTITY_TYPE)='CABLE' and is_cable_a_end=false
 )C 
inner join point_master p 
on c.destination_system_id=p.system_id and c.DESTINATION_ENTITY_TYPE=p.entity_type  limit 1;

v_old_b_distance:=(Cast(st_distance(v_spliced_b_end_entity_geom,v_old_end_geom,false)as decimal(10,2)));
v_new_b_distance:=(Cast(st_distance(v_spliced_b_end_entity_geom,v_new_end_geom,false)as decimal(10,2)));

if((select ST_Equals(v_old_start_geom,v_new_start_geom))=false) 
AND EXISTS(SELECT 1 FROM CONNECTION_INFO WHERE ((SOURCE_SYSTEM_ID=p_system_id AND UPPER(SOURCE_ENTITY_TYPE)='CABLE') OR DESTINATION_SYSTEM_ID=p_system_id AND UPPER(DESTINATION_ENTITY_TYPE)='CABLE') and is_cable_a_end=true)
and v_new_a_distance>v_old_a_distance
then
RETURN QUERY SELECT FALSE AS STATUS, (V_LAYER_TITLE||'[SI_GBL_GBL_GBL_GBL_019]')::CHARACTER VARYING AS MESSAGE;--start point can not be changed, as it is spliced with some entity!
elsif((select ST_Equals(v_old_end_geom,v_new_end_geom)=false)) AND EXISTS(SELECT 1 FROM CONNECTION_INFO WHERE ((SOURCE_SYSTEM_ID=p_system_id AND UPPER(SOURCE_ENTITY_TYPE)='CABLE') OR DESTINATION_SYSTEM_ID=p_system_id AND UPPER(DESTINATION_ENTITY_TYPE)='CABLE') and is_cable_a_end=false)
and v_new_b_distance>v_old_b_distance
then
RETURN QUERY SELECT FALSE AS STATUS, (V_LAYER_TITLE||'[SI_OSP_GBL_GBL_GBL_146]')::CHARACTER VARYING AS MESSAGE;--end point can not be changed, as it is spliced with some entity!
end if;
ELSIF UPPER(p_entity_type) = 'NETWORK_TICKET'
THEN
create temp table ticket_entity(
system_id integer,
entity_type character varying(50)
)on commit drop;
insert into ticket_entity (system_id,entity_type)
select system_id,entity_type from fn_nwt_get_entity_list(p_system_id);

select count(1) into v_old_child_element from ticket_entity;
select count(1) into v_new_child_element from ticket_entity gce inner join
(SELECT ptm.system_id ,ptm.entity_type,ptm.SP_GEOMETRY FROM point_master ptm
UNION
SELECT lm.system_id ,lm.entity_type,lm.SP_GEOMETRY FROM line_master lm
UNION
SELECT pm.system_id ,pm.entity_type,pm.SP_GEOMETRY FROM polygon_master pm
) a on gce.system_id=a.system_id and gce.entity_type=a.entity_type and ST_Within(a.SP_GEOMETRY, ST_GeomFromText('POLYGON(('||p_longlat||'))',4326));

IF(v_new_child_element<v_old_child_element) then
RETURN QUERY
select false as status, '[SI_OSP_GBL_NET_FRM_482]'::character varying as message;--Network entities is outside the ticket boundary!

END IF;
ELSIF UPPER(p_entity_type) = 'ROW'
THEN
if exists( select 1 from att_details_row_apply where row_system_id =p_system_id)
then
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_046]'::character varying as message;--ROW is already applied!
end if;

if exists (select 1
from circle_master pm
inner join att_details_row_pit pit
on
ST_Within(st_makevalid(pm.sp_geometry),ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))=false
and pm.system_id=pit.system_id and upper(pm.entity_type)=upper('PIT')
where pit.parent_system_id=p_system_id)
then
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_047]'::character varying as message;--PIT can not be outside the ROW! 
end if;
-- ELSIF(V_IS_ASSOCIATED=true AND v_is_shifting_allowed=false)
-- THEN
-- RETURN QUERY SELECT FALSE AS STATUS, (P_ENTITY_TYPE||'[SI_GBL_GBL_GBL_GBL_049]')::CHARACTER VARYING AS MESSAGE;--location can not be changed, as it is associated with some entity!
END IF;

-- IF UPPER(p_geom_type)='POINT'
-- THEN
-- 
-- IF EXISTS(Select 1 from polygon_master pm
-- where st_within((select sp_geometry from point_master where upper(entity_type)=upper(p_entity_type) and system_id=p_system_id),pm.sp_geometry) and entity_type='DSA' 
-- and coalesce(pm.gis_design_id,'')!='')
-- THEN 
-- 
-- Select system_id::text||entity_type from polygon_master pm into v_a_entity_type
-- where st_within((select sp_geometry from point_master where upper(entity_type)=upper(p_entity_type) and system_id=p_system_id),pm.sp_geometry) and
-- entity_type='DSA' and coalesce(pm.gis_design_id,'')!='';
-- 
-- Select system_id::text||entity_type from polygon_master into v_b_entity_type pm where ST_Within(ST_GeomFromText('POINT('||p_longlat||')',4326),pm.SP_GEOMETRY) 
-- and entity_type='DSA';
-- 
-- IF(coalesce(v_a_entity_type,'') != coalesce(v_b_entity_type,''))
-- THEN 
-- RETURN QUERY
-- select false as status, 'DSA codification is done geometry change is not allowed'::character varying as message;--PIT can not be outside the ROW!
-- END IF; 
-- 
-- END IF;
-- END IF;

RETURN QUERY
select true as status,'[SI_GBL_GBL_GBL_GBL_016]'::character varying as message;--location updated successfully!
--sql:='

else
RETURN QUERY
select V_STATUS as status, V_MESSAGE::character varying as message;
end if;
END
$BODY$;

ALTER FUNCTION public.fn_validate_geom_update(integer, character varying, character varying, integer, character varying, integer, character varying, character varying)
    OWNER TO postgres;
