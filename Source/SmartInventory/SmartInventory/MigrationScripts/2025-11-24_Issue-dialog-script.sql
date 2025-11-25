CREATE OR REPLACE FUNCTION public.fn_validate_geom_update(p_system_id integer, p_geom_type character varying, p_entity_type character varying, p_userid integer, p_longlat character varying, p_bld_buffer integer, p_source_ref_type character varying, p_source_ref_id character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
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

if((SELECT count(1) FROM polygon_master PM WHERE (ST_Within(PM.SP_GEOMETRY,ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)) or PM.SP_GEOMETRY=ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)) and lower(entity_type)=lower('subarea')) < v_total_child) 
THEN
RETURN QUERY
select false as status, '[SI_GBL_GBL_GBL_GBL_031]'::character varying as message; --Child entity cannot be outside from Area!
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
/*ELSIF UPPER(P_ENTITY_TYPE) != 'CABLE'
AND EXISTS(SELECT 1 FROM CONNECTION_INFO WHERE (SOURCE_SYSTEM_ID = p_system_id AND UPPER(SOURCE_ENTITY_TYPE) = UPPER(P_ENTITY_TYPE)) 
        OR 
        (DESTINATION_SYSTEM_ID = p_system_id AND UPPER(DESTINATION_ENTITY_TYPE) = UPPER(P_ENTITY_TYPE))
)
	then
	
	raise info 'P_ENTITY_TYPE %',P_ENTITY_TYPE ;
		RETURN QUERY
		select false as status,'Please remove the splicing first to change the location!'::character varying as message;--ROW is already applied!
*/
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
$function$
;

------------------------------------------------------------------------------------------------

alter table temp_auto_network_plan add column entity_name character varying null;
alter table att_details_spliceclosure add column sprout_site_id character varying null;
alter table att_details_pole add column sprout_site_id character varying null;
alter table att_details_manhole add column sprout_site_id character varying null;
alter table att_details_cable add column sprout_site_id character varying null;
alter table att_details_loop add column sprout_site_id character varying null;
alter table att_details_trench add column sprout_site_id character varying null;
alter table att_details_duct add column sprout_site_id character varying null;

---------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_network_planning_get_plan_bom_list(p_plan_name character varying, p_plan_mode character varying, p_cable_type character varying, is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_cable_length double precision, p_distance double precision, p_user_id integer, p_temp_plan_id integer, p_is_loop_require boolean, p_is_loop_update boolean, p_loop_length double precision, p_polepecvendor character varying, p_manholepecvendor character varying, p_scspecvendor character varying, p_loop_span double precision)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
sql text;
v_line_total_length DOUBLE PRECISION;
v_point_entity character varying;
v_line_geom geometry;
v_total_point_entity integer;
v_is_osp_tp boolean;
v_total_line integer;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_cable_length double precision;
v_end_entity_count integer;
v_middle_entity_count integer; 
v_arrow_point record;
v_loop_length double precision;
v_outer_query text;
v_temp_planid integer;
p_site_id character varying;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
sql:='';
v_total_line:=0;
v_temp_planid:=0;

p_scspecvendor = case when p_scspecvendor is null then '' else p_scspecvendor end;
p_manholepecvendor = case when p_manholepecvendor is null then '' else p_manholepecvendor end;
p_polepecvendor = case when p_polepecvendor is null then '' else p_polepecvendor end;

if(p_is_loop_update)
then
v_temp_planid:= p_temp_plan_id;
select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;

select count(plan_id)into v_total_point_entity from temp_auto_network_plan where plan_id=v_temp_planid and loop_length >0;

sql:=sql || 'select '||v_loop_length||' as length_qty,''Loop''::character varying as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
	raise info 'ssdfasdfql=%',sql;	
else

if(upper(p_cable_type)=upper('overhead'))
then
	v_point_entity:='Pole';
	else
	v_point_entity:='Manhole';
end if;

 
 raise info 'p_site_id =%',p_site_id;
	
select is_osp_tp into v_is_osp_tp from vw_termination_point_master where tp_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure')and layer_id=(select layer_id from layer_details where layer_name='Cable');

  select validation.status,validation.message from  fn_network_planning_validate(p_plan_mode,p_cable_type,is_create_trench,
  is_create_duct,v_is_osp_tp,p_cable_length,v_point_entity,p_user_id,0,p_line_geom) as validation into V_IS_VALID,V_MESSAGE;

 IF(V_IS_VALID)
THEN

-- creating points entity according to line geom;
select * from fn_temp_network_planning(p_plan_mode,p_cable_type,is_create_trench,is_create_duct,p_line_geom,p_cable_length,p_distance,p_user_id,p_plan_name,'', '', '', 0, '', 0, '',p_temp_plan_id)into v_arrow_point;

v_temp_planid:=v_arrow_point.temp_plan_id;


-- count of middle entity
select count(plan_id)into v_middle_entity_count from temp_auto_network_plan where plan_id=v_arrow_point.temp_plan_id and is_middle_point='true';

-- end entity
select count(plan_id) into v_end_entity_count from temp_auto_network_plan where plan_id=v_arrow_point.temp_plan_id and is_middle_point='false';

-- total count of point entity
v_total_point_entity:=v_middle_entity_count+v_end_entity_count;

 select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
 select round(st_length(v_line_geom,false)::numeric, 6)::double precision into v_line_total_length;

-- get counts of cable 
 if(v_line_total_length<p_cable_length)
 then
 	v_total_line:=1;
 else
 	select CEIL(v_line_total_length/p_cable_length)::integer into v_total_line;		
 end if;

-- for cable
sql := 'select '||v_line_total_length||' as length_qty,* from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';

if(is_create_trench) 
then 
	sql:=sql || ' union select '||v_line_total_length||' as length_qty,* from 
	fn_network_planning_audit_template_detail(''trench'','||p_user_id||')';
end if;

if(is_create_duct) 
then 
	sql:=sql || ' union select '||v_line_total_length||' as length_qty,* from fn_network_planning_audit_template_detail(''duct'','||p_user_id||')';
end if;
 

if(upper(p_plan_mode)=upper('auto'))
then
	--sql:=sql || ' union select '||v_total_point_entity||' as length_qty,* from fn_network_planning_audit_template_detail('''||v_point_entity||''','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';	
sql := sql || ' UNION SELECT ' || v_total_point_entity || ' AS length_qty, * 
FROM fn_network_planning_audit_template_detail('
|| quote_literal(v_point_entity) || ',' 
|| p_user_id || ',' 
|| quote_literal(p_polepecvendor) || ',' 
|| quote_literal(p_manholepecvendor) || ',' 
|| quote_literal(p_scspecvendor) || ')';

else
	--sql:=sql || ' union select '||v_total_point_entity||' as length_qty,* from fn_network_planning_audit_template_detail('''||v_point_entity||''','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';
sql := sql || ' UNION SELECT ' || v_total_point_entity || ' AS length_qty, * 
FROM fn_network_planning_audit_template_detail('
|| quote_literal(v_point_entity) || ',' 
|| p_user_id || ',' 
|| quote_literal(p_polepecvendor) || ',' 
|| quote_literal(p_manholepecvendor) || ',' 
|| quote_literal(p_scspecvendor) || ')';

raise info 'inner query11 =%',sql;
end if;

if(v_is_osp_tp)
then 
	sql:=sql || ' union select '||v_end_entity_count||' as length_qty,* from fn_network_planning_audit_template_detail(''spliceclosure'','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';
end if;

perform (fn_auto_network_plan_draft_update_loop_network(p_line_geom, p_temp_plan_id, p_user_id, p_loop_span, p_loop_length));

if(p_is_loop_require)
	then 	
		/*if(p_is_loop_update)
		then 
		 select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;
		else
			v_loop_length := v_total_point_entity * p_loop_length;
		end if;*/

		select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;
	
		sql:=sql || ' union select '||v_loop_length||' as length_qty,''Loop'' as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';

	end if;
end if;


end if;

raise info 'inner query =%',sql;

v_outer_query:= 'select '||V_IS_VALID||'::boolean AS is_template_extis,'||v_temp_planid||' as temp_plan_id,
 case when upper(entity_type)=''CABLE'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''DUCT'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''TRENCH'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
 when upper(entity_type)=''LOOP'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_point_entity||')::character varying else length_qty ::character varying end as length_qty,
(round((length_qty)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((length_qty)::numeric, 2))as amount,entity_type,geom_type,cost_per_unit,service_cost_per_unit from ('||sql||')e';

 raise info 'inner query fin=al =%',v_outer_query;

        -- First try with start point
        SELECT pod.site_id
        INTO p_site_id
        FROM point_master pm
        JOIN att_details_pod pod ON pod.system_id = pm.system_id 
        WHERE pm.entity_type = 'POD'
          AND ST_Within(
                pm.sp_geometry,
                st_buffer_meters(
                    ST_StartPoint(
                        ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
                    ), 3)
            )
        LIMIT 1;

        -- Update Manhole names (forward order)
        UPDATE temp_auto_network_plan t
        SET entity_name = 
            CASE
                WHEN p_site_id IS NULL THEN 'MH-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-MH-' || LPAD(x.seq::text, 2, '0')
            END
        FROM (
            SELECT system_id,
                   ROW_NUMBER() OVER (ORDER BY system_id) AS seq
            FROM temp_auto_network_plan
            WHERE entity_type = 'Manhole'
              AND plan_id = v_temp_planid
        ) x
        WHERE t.system_id = x.system_id
          AND t.plan_id = v_temp_planid;

        -- Update Pole names (forward order)
        UPDATE temp_auto_network_plan t
        SET entity_name = 
            CASE
                WHEN p_site_id IS NULL THEN 'PL-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-PL-' || LPAD(x.seq::text, 2, '0')
            END
        FROM (
            SELECT system_id,
                   ROW_NUMBER() OVER (ORDER BY system_id) AS seq
            FROM temp_auto_network_plan
            WHERE entity_type = 'Pole'
              AND plan_id = v_temp_planid
        ) x
        WHERE t.system_id = x.system_id
          AND t.plan_id = v_temp_planid;

        -- If not found, try with end point
        IF p_site_id IS null or p_site_id = '' THEN
            SELECT pod.site_id 
            INTO p_site_id
            FROM point_master pm
            JOIN att_details_pod pod ON pod.system_id = pm.system_id 
            WHERE pm.entity_type = 'POD'
              AND ST_Within(
                    pm.sp_geometry,
                    st_buffer_meters(
                        ST_EndPoint(
                            ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
          ), 3) )
            LIMIT 1;
           
       

         IF p_site_id is Not NULL then
                  

            -- Update Manhole names (reverse order)
            UPDATE temp_auto_network_plan t
            SET entity_name =  CASE
                WHEN p_site_id IS NULL THEN 'MH-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-MH-' || LPAD(x.seq::text, 2, '0')
            END
            FROM (
                SELECT system_id,
                       ROW_NUMBER() OVER (ORDER BY system_id DESC) AS seq
                FROM temp_auto_network_plan 
                WHERE entity_type = 'Manhole'
                  AND plan_id = v_temp_planid
            ) x
            WHERE t.system_id = x.system_id
              AND t.plan_id = v_temp_planid;

            -- Update Pole names (reverse order)
            UPDATE temp_auto_network_plan t
            SET entity_name =  CASE
                WHEN p_site_id IS NULL THEN 'PL-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-PL-' || LPAD(x.seq::text, 2, '0') 
            END
            FROM (
                SELECT system_id,
                       ROW_NUMBER() OVER (ORDER BY system_id DESC) AS seq
                FROM temp_auto_network_plan
                WHERE entity_type = 'Pole'
                  AND plan_id = v_temp_planid
            ) x
            WHERE t.system_id = x.system_id
              AND t.plan_id = v_temp_planid;
             
             p_site_id = p_site_id|| '--DESC';
        END IF;
   END IF;
       
IF (V_IS_VALID) THEN
    RETURN QUERY
    EXECUTE 'select row_to_json(row) 
             from (select *,'|| quote_literal(coalesce(p_site_id, '')) ||' as site_id 
                   from ('||v_outer_query||')t order by geom_type) row';
ELSE
    RETURN QUERY
    EXECUTE 'select row_to_json(row) 
             from (select '||V_IS_VALID||'::boolean AS is_template_extis, 
                          '||quote_literal(V_MESSAGE)||'::character varying AS msg, 
                          '||quote_literal(coalesce(p_site_id, ''))||'::character varying as site_id) row';
END IF;

            
 END
$function$
;

------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_network_planning_save_auto_planning(p_plan_mode character varying, p_cable_type character varying, is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_cable_length double precision, p_distance double precision, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, p_end_point_type character varying, p_end_point_buffer double precision, p_edit_path character varying, p_end_point_entity_id integer, p_end_point_entity_type character varying, p_temp_plan_id integer, p_is_loop_required boolean, p_loop_length double precision, p_polespecvendor character varying, p_manholespecvendor character varying, p_scspecvendor character varying)
 RETURNS TABLE(status boolean, message character varying, plan_id integer)
 LANGUAGE plpgsql
AS $function$

DECLARE

V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow record;
v_system_id integer;
v_entity_type character varying;
v_arow_network_id record;
v_arow_parent_details record;
v_arow_cable record;
v_arow_box record;
v_layer_table character varying;
v_arow_splitter record;
sql text;
v_arow_wcr record;
v_arow_layer_details record;
v_layer_wcr_mapping character varying;
v_new_wcr_mapping record;
v_current_system_id integer;
v_line_geom geometry;
v_region_province record;
v_arow_closure record;
v_counter integer;
v_arow_prev_closure record;
v_arow_prev_cable record;
-- v_other_end_geom geometry;
v_prev_qty_length double precision;
V_TOTAL_CALC_LENGTH DOUBLE PRECISION;
v_line_total_length DOUBLE PRECISION;
v_total_line integer;
p_rfs_type character varying;
v_manhole_item_id integer;
v_arow_manhole record;
v_sub_line_geom geometry;
v_sub_line_start_point geometry;
v_sub_line_end_point geometry;
v_sub_line character varying;
v_arow_prev_manhole record;
v_arow_trench record;
v_arow_duct record;
v_arow_fms record;
v_arow_pod record;
v_arow_first_closure record;
v_point_entity character varying;
v_is_osp_tp BOOLEAN;
p_plan_id integer;
v_audit_item_master_id_sc integer;
v_audit_item_master_id_point_entity integer;
v_audit_item_master_id_cable integer;
v_audit_item_master_id_duct integer;
v_audit_item_master_id_trench integer;
v_layer_ids character varying;
--v_arow_first_manhole record;
v_temp_point_entity record;
--------
v_current_point_geom geometry;
v_start_point character varying;
v_end_point character varying;
v_loop_length double precision;
v_point_entity_type character varying;
v_a_display_value CHARACTER VARYING;
v_inner_query text;
v_arow_end_manhole record;
v_arow_middle_manhole record;
v_total_record_temp integer;
v_counter_2 integer;
v_pole_spec_vendor TEXT[];
v_manhole_spec_vendor TEXT[];
v_sc_spec_vendor TEXT[];
is_endpoint_entity_exists boolean;
is_a_endpoint_entity_exists boolean;
is_b_endpoint_entity_exists boolean;
BEGIN
v_counter_2=0;
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
v_counter:=0;
sql:='';
v_prev_qty_length:=0;
p_rfs_type:='A-RFS';
p_plan_id=0;

CREATE temp TABLE temp_connection
(
source_system_id integer,
source_network_id character varying(100),
source_entity_type character varying(100),
source_port_no integer,
destination_system_id integer,
destination_network_id character varying(100),
destination_entity_type character varying(100),
destination_port_no integer,
is_source_cable_a_end boolean NOT NULL DEFAULT false,
is_destination_cable_a_end boolean NOT NULL DEFAULT false,
equipment_system_id integer,
equipment_network_id character varying(100),
equipment_entity_type character varying(100),
created_by integer,
splicing_source character varying(100)
) on commit drop;

IF(coalesce(p_line_geom,'')='' )
then
V_IS_VALID=FALSE;
V_MESSAGE:='Line Geom can not be blank !';
end if;

IF(V_IS_VALID)
THEN
select is_osp_tp into v_is_osp_tp from vw_termination_point_master where tp_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure')and layer_id=(select layer_id from layer_details where layer_name='Cable');

select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
select st_length(v_line_geom,false)::double precision into v_line_total_length;

    v_pole_spec_vendor := string_to_array(p_polespecvendor, ',');
    v_manhole_spec_vendor := string_to_array(p_manholespecvendor, ',');
    v_sc_spec_vendor := string_to_array(p_scspecvendor, ',');
  
if(upper(p_cable_type)=upper('overhead'))then
v_point_entity:='Pole';
else
v_point_entity:='Manhole';
end if;

select validation.status,validation.message from fn_network_planning_validate(p_plan_mode,p_cable_type,is_create_trench,is_create_duct,v_is_osp_tp,p_cable_length,v_point_entity,p_user_id,p_end_point_buffer,p_line_geom) as validation into V_IS_VALID,V_MESSAGE;

end if;

IF(V_IS_VALID)
THEN

------------ Get all entity audit_item_master_id =================================================

select (select (select * from fn_get_template_detail(p_user_id,'SpliceClosure',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_sc ;
select (select (select * from fn_get_template_detail(p_user_id,v_point_entity,''))->>'audit_item_master_id')::integer into v_audit_item_master_id_point_entity ;
select (select (select * from fn_get_template_detail(p_user_id,'cable',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_cable ;
select (select (select * from fn_get_template_detail(p_user_id,'duct',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_duct ;
select (select (select * from fn_get_template_detail(p_user_id,'trench',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_trench;

----------------========================layerList--------------=================================
select array_to_string(array_agg(layer_id), ',') into v_layer_ids from layer_details
where
(true =true and upper(layer_name) =upper('cable'))
or (is_create_trench =true and upper(layer_name) =upper('trench'))
or (is_create_duct =true and upper(layer_name) =upper('duct'))
or (v_is_osp_tp =true and upper(layer_name) =upper('SpliceClosure'))
or (p_is_loop_required =true and upper(layer_name) =upper('Loop'))
or (true =true and upper(layer_name) =upper(''||v_point_entity||''));

------------------------- save plan ==================================================================================
INSERT INTO public.att_details_network_plan
(planid,layer_id,plan_name, start_point, end_point, cable_type, is_create_trench, is_create_duct, pole_manhole_distance, cable_length, created_by,
created_on,planning_mode,
end_point_type,end_point_buffer,end_point_entity,edit_path,is_loop_required,loop_length)
VALUES(p_temp_plan_id,v_layer_ids,p_plan_name, p_startpoint, p_endpoint,p_cable_type,is_create_trench, is_create_duct, p_distance, p_cable_length, p_user_id, now(),p_plan_mode,
p_end_point_type,p_end_point_buffer,p_end_point_entity_id,p_edit_path,p_is_loop_required,p_loop_length)
RETURNING planid into p_plan_id;

 SELECT count(*)>0 into is_a_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = p_temp_plan_id and tp.fraction = 0
 ORDER BY tp.system_id LIMIT 1),4326),2));

 SELECT count(*)>0 into is_b_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = p_temp_plan_id and tp.fraction = 1
 ORDER BY tp.system_id LIMIT 1),4326),2));
--select count(1) from temp_auto_network_plan

FOR v_temp_point_entity IN select * from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id order by system_id
LOOP
 
is_endpoint_entity_exists :=
    CASE
        WHEN v_temp_point_entity.fraction = 0 THEN is_a_endpoint_entity_exists
        WHEN v_temp_point_entity.fraction = 1 THEN is_b_endpoint_entity_exists
        ELSE false
    END;

if(is_endpoint_entity_exists = true and v_temp_point_entity.fraction = 0) 
then 
 v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;

end if;

v_counter:=v_counter+1;

v_current_point_geom=case when v_temp_point_entity.fraction in (0,1) then v_temp_point_entity.sp_geometry else ST_LineInterpolatePoint(v_line_geom, v_temp_point_entity.fraction) end;

select count(1) from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point =false into v_total_record_temp;

v_loop_length := v_temp_point_entity.loop_length;
v_point_entity_type := v_temp_point_entity.entity_type;
select * into v_region_province from fn_getregionprovince(substring(left(St_astext(v_current_point_geom),-1),7),'Point');

update temp_auto_network_plan set province_id =v_region_province.province_id,region_id=v_region_province.region_id,created_by=p_user_id where system_id=v_temp_point_entity.system_id;

-- if we need to create entity in middle then only middle entity is created
if(v_temp_point_entity.is_middle_point=true)
then

SELECT * into v_arow_network_id FROM fn_get_clone_network_code( v_point_entity, 'Point', substring(left(St_astext(v_current_point_geom),-1),7),0,'')
as clone order by clone.status desc limit 1;

raise info '%is_endpoint_entity_exists',is_endpoint_entity_exists;
if(is_endpoint_entity_exists = false) then 

if(upper(v_point_entity)=upper('Manhole'))
then
raise info '%v_counter_1',v_counter_2;

INSERT INTO att_details_manhole(is_visible_on_map,audit_item_master_id,ownership_type,network_id, manhole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
,project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',v_arow_network_id.message,case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name  else v_arow_network_id.message end,
st_x(v_current_point_geom),st_y(v_current_point_geom),
v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,
code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',v_arow_network_id.o_p_system_id,
v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0, 0, 0, 0,'planning',p_plan_id
FROM item_template_master where id = v_manhole_spec_vendor[1]::integer and vendor_id = v_manhole_spec_vendor[3]::integer and code= v_manhole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_middle_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name)
values(v_arow_middle_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A',v_current_point_geom,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name  else fn_get_display_name(v_arow_middle_manhole.system_id, v_point_entity) end
);

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Manhole'::character varying ,v_region_province.province_id,0,false));

else
raise info '%v_counter_2',v_counter_2;

INSERT INTO att_details_pole(is_visible_on_map,audit_item_master_id,ownership_type,pole_type,network_id, pole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code,
vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
, project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',specify_type,v_arow_network_id.message,
case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else v_arow_network_id.message end,
st_x(v_current_point_geom),st_y(v_current_point_geom),
v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,
code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
v_arow_network_id.o_p_system_id,v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id,
0, 0, 0, 0 ,'planning',p_plan_id
FROM item_template_master where id = v_pole_spec_vendor[1]::integer and vendor_id = v_pole_spec_vendor[3]::integer and code = v_pole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_middle_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_middle_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A',v_current_point_geom,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',
case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else fn_get_display_name(v_arow_middle_manhole.system_id, v_point_entity) end,'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Pole'::character varying ,v_region_province.province_id,0,false));
end if;
end if;

else
raise info '%v_counter_3',v_counter_2;

raise info '%is_endpoint_entity_exists',is_endpoint_entity_exists;

SELECT * into v_arow_network_id FROM fn_get_clone_network_code( v_point_entity, 'Point', substring(left(St_astext(v_current_point_geom),-1),7),0,'')
as clone order by clone.status desc limit 1;
raise info '%v_counter_4',v_counter_2;

if( is_endpoint_entity_exists = false)
then 

raise info '%v_counter_5',v_counter_2;

if(upper(v_point_entity)=upper('Manhole'))
then
raise info '%v_counter_6',v_counter_2;

INSERT INTO att_details_manhole(is_visible_on_map,audit_item_master_id,ownership_type,network_id, manhole_name, longitude, latitude,
province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, type, brand,
model, construction, activation,accessibility, status,created_by, created_on,network_status,parent_system_id, parent_network_id,
parent_entity_type, sequence_id,project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',v_arow_network_id.message,case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else v_arow_network_id.message end,st_x(v_current_point_geom),
st_y(v_current_point_geom),v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,code,vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',v_arow_network_id.o_p_system_id,
v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0, 0, 0, 0,'planning',p_plan_id
FROM item_template_master where id = v_manhole_spec_vendor[1]::integer and vendor_id = v_manhole_spec_vendor[3]::integer and code = v_manhole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A',v_current_point_geom,now(),'Created', '' ,p_user_id,v_arow_manhole.network_id,
'P',case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else fn_get_display_name(v_arow_manhole.system_id, v_point_entity) end,'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_manhole.system_id,'Manhole'::character varying ,v_region_province.province_id,0,false));

else

raise info '%v_counter_7',v_counter_2;

INSERT INTO att_details_pole(is_visible_on_map,audit_item_master_id,ownership_type,pole_type,network_id, pole_name, longitude,
latitude, province_id,
region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, type, brand, model, construction,
activation,accessibility, status,created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type,
sequence_id, project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',specify_type,v_arow_network_id.message,
case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else v_arow_network_id.message end,
st_x(v_current_point_geom),st_y(v_current_point_geom),v_region_province.province_id,v_region_province.region_id, specification, category_reference,
 subcategory_1, subcategory_2, subcategory_3,code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
v_arow_network_id.o_p_system_id,v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0,
0, 0, 0,'planning',p_plan_id FROM item_template_master where id = v_pole_spec_vendor[1]::integer and vendor_id = v_pole_spec_vendor[3]::integer and code = v_pole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude
into v_arow_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A',v_current_point_geom,now(),'Created', '' ,p_user_id,v_arow_manhole.network_id,
'P',case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else fn_get_display_name(v_arow_manhole.system_id, v_point_entity) end ,'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_manhole.system_id,'Pole'::character varying, v_region_province.province_id,0,false));
end if;

--------- if need to create spliceclosure then we run this
if(v_is_osp_tp) then
raise info '%v_counter_8',v_counter_2;

select * into v_arow_network_id from fn_get_network_code('SpliceClosure','Point',0,'',substring(left(St_astext(v_current_point_geom),-1),7));

insert into att_details_spliceclosure(is_visible_on_map,audit_item_master_id,ownership_type,network_id,spliceclosure_name,latitude,
longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,
no_of_input_port,no_of_output_port,is_used,is_virtual,source_ref_type,source_ref_id)

select true,audit_id ,'OWN',v_arow_network_id.network_code,v_arow_network_id.network_code,st_y(v_current_point_geom),
st_x(v_current_point_geom),v_region_province.province_id,v_region_province.region_id,
specification, category_reference, subcategory_1, subcategory_2, subcategory_3,code, vendor_id, 0,0,0,0,0,0,'A',p_user_id,now(),
'P',v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
v_arow_network_id.sequence_id,no_of_port,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false,'planning',p_plan_id
from item_template_master where id = v_sc_spec_vendor[1]::integer and vendor_id = v_sc_spec_vendor[3]::integer and code = v_sc_spec_vendor[2] ::varchar limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port into v_arow_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
created_by,common_name, network_status,no_of_ports,display_name,source_ref_type,source_ref_id)
values(v_arow_closure.system_id, 'SpliceClosure',st_x(v_current_point_geom),st_y(v_current_point_geom),'A',
ST_GEOMFROMTEXT('POINT('||st_x(v_current_point_geom)||' '||st_y(v_current_point_geom)||')',4326),now(),'Created', ''
,p_user_id,v_arow_network_id.network_code, 'P',v_arow_closure.no_of_ports::character varying,
fn_get_display_name(v_arow_closure.system_id, 'SpliceClosure'),'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_closure.system_id,'SpliceClosure'::character varying ,v_region_province.province_id,0,false));

perform(fn_bulk_insert_port_info(v_arow_closure.no_of_ports,v_arow_closure.no_of_ports,'SpliceClosure',
v_arow_closure.system_id,v_arow_closure.network_id,p_user_id));

insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
entity_type,created_on,created_by,associated_display_name,entity_display_name)
values('SpliceClosure',v_arow_closure.system_id,v_arow_closure.network_id,v_arow_manhole.network_id,v_arow_manhole.system_id,v_point_entity,now(),p_user_id,
fn_get_display_name(v_arow_closure.system_id,'SpliceClosure'),fn_get_display_name(v_arow_manhole.system_id,v_point_entity));

end if;
end if;

-- update values in temp table related to loop
raise info '%v_counter_9',v_counter_2;

if(v_counter >1 and v_temp_point_entity.is_middle_point=false)

then
raise info '%v_counter_10',v_counter_2;
IF(v_temp_point_entity.FRACTION = 1) then 
v_arow_closure.longitude = v_temp_point_entity.longitude;
v_arow_closure.latitude = v_temp_point_entity.latitude;
v_arow_manhole.longitude = v_temp_point_entity.longitude;
v_arow_manhole.latitude = v_temp_point_entity.latitude;

end if;

select ST_GeomFromText('POINT('||v_arow_prev_closure.longitude||' '||v_arow_prev_closure.latitude||')',4326) into v_sub_line_start_point;
select ST_GeomFromText('POINT('||v_arow_closure.longitude||' '||v_arow_closure.latitude||')',4326) into v_sub_line_end_point;
select ST_LineSubstring(v_line_geom, ST_LineLocatePoint(v_line_geom, v_sub_line_start_point),
ST_LineLocatePoint(v_line_geom, v_sub_line_end_point)) into v_sub_line_geom;
select substring(left(st_astext(v_sub_line_geom),-1),12) into v_sub_line;

raise info '%v_sub_line',v_sub_line;
raise info '%v_sub_line1',v_arow_prev_closure.longitude||' '||v_arow_prev_closure.latitude;
raise info '%v_sub_line2',v_arow_closure.longitude||' '||v_arow_closure.latitude;

raise info '%v_arow_prev_manhole',v_arow_prev_manhole.longitude||' '||v_arow_closure.latitude;

if(is_create_trench)
then
select * into v_arow_trench from fn_network_planning_auto_create_trench(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,v_audit_item_master_id_trench)
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_trench.system_id,'Trench'::character varying ,v_region_province.province_id,0,false));

--- update trench id in temp table related to point_entity
update temp_auto_network_plan tp set trench_id=v_arow_trench.SYSTEM_ID,trench_network_id=v_arow_trench.network_id
where tp.system_id <= v_temp_point_entity.system_id and trench_id=0 and tp.plan_id=p_temp_plan_id;
end if;

if(is_create_duct)then

if(is_create_trench)then

select * into v_arow_duct from fn_network_planning_auto_create_duct(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,
v_audit_item_master_id_duct,v_arow_trench.system_id,v_arow_trench.network_id)
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_duct.system_id,'Duct'::character varying ,v_region_province.province_id,0,false));

else

select * into v_arow_duct from fn_network_planning_auto_create_duct(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,
v_audit_item_master_id_duct,0,'')
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_duct.system_id,'Duct'::character varying ,v_region_province.province_id,0,false));

end if;

--- update duct id in temp table related to point_entity
update temp_auto_network_plan tp set duct_id=v_arow_duct.SYSTEM_ID,duct_network_id=v_arow_duct.network_id
where tp.system_id <= v_temp_point_entity.system_id and duct_id=0 and tp.plan_id=p_temp_plan_id;
end if;

v_counter_2=v_counter_2+1;

--CREATE THE CABLE acco to duct

if(is_create_duct)
then

select * into v_arow_cable from fn_network_planning_auto_create_cable(v_arow_prev_closure.system_id,
'SpliceClosure'::character varying,v_arow_prev_closure.network_id,
v_arow_closure.system_id,'SpliceClosure'::character varying,v_arow_closure.network_id,v_sub_line,
--(v_arow_htb.longitude||' '||v_arow_htb.latitude||','||v_arow_closure.longitude||' '||v_arow_closure.latitude),
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,500,v_audit_item_master_id_cable,p_plan_id,
p_cable_type,v_arow_duct.system_id,v_arow_duct.network_id)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));

raise info '%v_counter_11',v_counter_2;
if(v_counter_2 = 1)
then

if(is_endpoint_entity_exists = true) 
then 
if(v_temp_point_entity.fraction = 0) 
then 
 v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;

/* elseif( v_temp_point_entity.fraction = 1) 
THEN
v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 --v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;
 */
end if;
else
raise info '%v_counter_12',v_counter_2;

v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
end if;

else

/*if(v_arow_prev_cable is not null)
then
insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,
destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,created_by,splicing_source,
equipment_system_id,equipment_network_id,equipment_entity_type)
values(v_arow_prev_cable.system_id,v_arow_prev_cable.network_id,'Cable',1,v_arow_cable.system_id,v_arow_cable.network_id,'Cable',1,false,true,
p_user_id,'PROVISIONNING',
v_arow_prev_closure.system_id, v_arow_prev_closure.network_id,'SpliceClosure');
end if;
*/
raise info '%v_counter_13',v_counter_2;

v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;

end if;
else
raise info '%v_counter_14',v_counter_2;

select * into v_arow_cable from fn_network_planning_auto_create_cable(v_arow_prev_closure.system_id,
'SpliceClosure'::character varying,v_arow_prev_closure.network_id,
v_arow_closure.system_id,'SpliceClosure'::character varying,v_arow_closure.network_id,v_sub_line,
--(v_arow_htb.longitude||' '||v_arow_htb.latitude||','||v_arow_closure.longitude||' '||v_arow_closure.latitude),
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,500,v_audit_item_master_id_cable,p_plan_id,
p_cable_type,0,'')
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));

if(v_counter_2 = 1)
then
raise info '%v_counter_15',v_counter_2;

if(is_endpoint_entity_exists = true) 
then 
if(v_temp_point_entity.fraction = 0) 
then 
 v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;
/*
elseif( v_temp_point_entity.fraction = 1) 
THEN
v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 --v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;
 */
end if;
 
raise info '%v_counter_16',v_counter_2;

 else
 raise info '%v_counter_17',v_counter_2;

--v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;

end if;

raise info 'v_arow_prev_cable%',v_arow_prev_cable;
else
/*
if(v_arow_prev_cable is not null)
then
insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,
destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,created_by,splicing_source,
equipment_system_id,equipment_network_id,equipment_entity_type)
values(v_arow_prev_cable.system_id,v_arow_prev_cable.network_id,'Cable',1,v_arow_cable.system_id,v_arow_cable.network_id,'Cable',1,false,true,
p_user_id,'PROVISIONNING',
v_arow_prev_closure.system_id, v_arow_prev_closure.network_id,'SpliceClosure');
end if;
*/
raise info '%v_counter_18',v_counter_2;

--v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
raise info '%v_counter_19',v_counter_2;

end if;
end if;

PERFORM(fn_network_planning_associate_end_entity(false,'',v_arow_prev_manhole.system_id,v_point_entity,v_arow_prev_manhole.network_id
,v_arow_manhole.system_id,v_point_entity,v_arow_manhole.network_id,'Cable',v_arow_cable.system_id,v_arow_cable.network_id,p_user_id,p_temp_plan_id));
--

PERFORM(fn_isp_create_OSP_Cable(v_arow_cable.SYSTEM_ID));

--- update cable id in temp table related to point_entity
update temp_auto_network_plan tp set cable_id=v_arow_cable.SYSTEM_ID,cable_network_id=v_arow_cable.network_id
where tp.system_id <= v_temp_point_entity.system_id and cable_id=0 and tp.plan_id=p_temp_plan_id;
--and loop_length !=0;

end if;

end if;
raise info '%v_counter_20',v_counter_2;

if(v_temp_point_entity.is_middle_point=true and is_endpoint_entity_exists = false)
then

raise info '%v_counter_21',v_counter_2;

select display_name into v_a_display_value from point_master where system_id=v_arow_middle_manhole.system_id and upper(entity_type)=upper(v_point_entity);

update temp_auto_network_plan set entity_type=v_point_entity,
entity_network_id=v_arow_middle_manhole.network_id,entity_system_id=v_arow_middle_manhole.system_id,display_name=v_a_display_value
where system_id=v_temp_point_entity.system_id and v_temp_point_entity.is_middle_point=true;
else
select display_name into v_a_display_value from point_master where system_id=v_arow_manhole.system_id and upper(entity_type)=upper(v_point_entity);

update temp_auto_network_plan set entity_type=v_point_entity,
entity_network_id=v_arow_manhole.network_id,entity_system_id=v_arow_manhole.system_id,display_name=v_a_display_value
where system_id=v_temp_point_entity.system_id and v_temp_point_entity.is_middle_point=false;
end if;

if(v_counter >1 and v_temp_point_entity.is_middle_point=false)
then

PERFORM(fn_network_planning_loop_entity(v_arow_cable.SYSTEM_ID,p_line_geom));
end if;
raise info '%v_counter_17',v_counter_2;

v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
end loop;

---------------------------associate cables to middle point entity like pole and manhole
if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then
PERFORM(fn_network_planning_associate_end_entity(true,'Cable',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

-- ---------------------------associate cables to middle point entity like pole and manhole
if(is_create_duct)
then

-- if middle entity exist then middle entity associate
if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then

PERFORM(fn_network_planning_associate_end_entity(true,'Duct',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

end if;

-- ---------------------------associate cables to middle point entity like pole and manhole
if(is_create_trench)
then

if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then

PERFORM(fn_network_planning_associate_end_entity(true,'Trench',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

end if;

end if;

-- delete the temp_auto_network_plan

-- delete from temp_auto_network_plan t where t.plan_id=p_temp_plan_id;

/*IF((SELECT COUNT(1) FROM temp_connection)>0)
THEN
perform(fn_auto_provisioning_save_connections());

END IF;
*/
--CUSTOMER AND SPLITTER MAPPING
IF(V_IS_VALID)
THEN
V_MESSAGE:='Network plan processed successfully!';
END IF;

RETURN QUERY SELECT V_IS_VALID::boolean AS STATUS, V_MESSAGE::CHARACTER VARYING AS MESSAGE, p_plan_id:: integer as plan_id;

END
$function$
;

-------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_network_planning_auto_create_cable(p_a_system_id integer, p_a_entity_type character varying, p_a_network_id character varying, p_b_system_id integer, p_b_entity_type character varying, p_b_network_id character varying, p_longlat character varying, p_structure_id integer, p_region_id integer, p_province_id integer, p_user_id integer, p_rfs_type character varying, p_length double precision, p_wcr_id integer, p_plan_id integer, p_cable_type character varying, p_duct_id integer, p_duct_network_id character varying)
 RETURNS record
 LANGUAGE plpgsql
AS $function$
DECLARE
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow_cable record;
v_arow_network_id record;
v_cable_display_value CHARACTER VARYING;
v_a_display_value CHARACTER VARYING;
v_b_display_value CHARACTER VARYING;
v_display_value CHARACTER VARYING;
v_display_name CHARACTER VARYING;
v_duct_value character varying;
BEGIN
V_IS_VALID:=true;
V_MESSAGE:='Successfully';
	
	select * into v_arow_network_id from fn_get_line_network_code('Cable',p_a_network_id,p_b_network_id,p_longlat,'OSP');
	
	INSERT INTO ATT_DETAILS_CABLE(is_visible_on_map,audit_item_master_id,network_id,cable_name,a_location,b_location,total_core,no_of_tube,no_of_core_per_tube,cable_measured_length,cable_calculated_length,
	cable_type,specification,category,subcategory1,subcategory2,subcategory3,item_code,	
	vendor_id,network_status,status,province_id,region_id,created_by,created_on,a_system_id,a_network_id,a_entity_type,
	b_system_id,b_network_id,b_entity_type,sequence_id,cable_category,parent_system_id,parent_network_id,parent_entity_type,start_reading,	
	end_reading,structure_id,utilization,duct_id,trench_id,is_used,source_ref_type,source_ref_id,ownership_type)

	select  true,p_wcr_id,v_arow_network_id.network_code,v_arow_network_id.network_code,p_a_network_id,p_b_network_id,total_core,no_of_tube,no_of_core_per_tube,ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),
	p_cable_type,specification,category, subcategory1, subcategory2, subcategory3,item_code,
	vendor_id,'P','A',p_province_id,p_region_id,p_user_id,now(),p_a_system_id,p_a_network_id,
	p_a_entity_type,p_b_system_id,p_b_network_id,p_b_entity_type,v_arow_network_id.sequence_id,'Feeder Cable',v_arow_network_id.parent_system_id,
	v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
	0,0,p_structure_id,'L',0,0,true,'planning',p_plan_id,'OWN' from item_template_cable itm where created_by=p_user_id and audit_item_master_id=p_wcr_id limit 1 
	RETURNING system_id,network_id,no_of_tube,total_core into v_arow_cable;


	INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,
	common_name,network_status,is_virtual,display_name)
	values(v_arow_cable.SYSTEM_ID,'Cable','A',
	ST_GeomFromText('LINESTRING('||p_longlat||')',4326)	
	,'','',
	p_user_id,0,v_arow_cable.network_id,'P',false,fn_get_display_name(v_arow_cable.SYSTEM_ID,'Cable'));
	
	/*select display_name into v_a_display_value from point_master where system_id=p_a_system_id and upper(entity_type)=upper(p_a_entity_type);
	select display_name into v_b_display_value from point_master where system_id=p_b_system_id and upper(entity_type)=upper(p_b_entity_type);

	insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,
	associated_entity_type,associated_display_name,created_on,created_by,is_termination_point)
	values(v_arow_cable.system_id,v_arow_cable.network_id,'Cable',v_display_value,p_a_system_id,p_a_network_id,p_a_entity_type,v_a_display_value,now(),p_user_id,true),
	(v_arow_cable.system_id,v_arow_cable.network_id,'Cable',v_display_value,p_b_system_id,p_b_network_id,p_b_entity_type,v_b_display_value,now(),p_user_id,true);
*/
	if(p_duct_id >0)
	then
	
	insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
		entity_type,created_on,created_by,associated_display_name,entity_display_name)
		values('Cable',v_arow_cable.system_id,v_arow_cable.network_id,p_duct_network_id,p_duct_id,'Duct',now(),p_user_id,
		fn_get_display_name(v_arow_cable.system_id,'Cable'),fn_get_display_name(p_duct_id,'Duct')); 
		
	end if;

	
	
	perform(fn_set_cable_color_info(v_arow_cable.system_id,p_user_id,v_arow_cable.no_of_tube,v_arow_cable.total_core));					
		
	RETURN v_arow_cable;          
END
$function$
;

-----------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_network_planning_loop_entity(p_cable_id integer, p_line_geom character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
v_current_point_geom geometry;
v_temp_point_entity record;
v_loop_network_id record;
v_line_geom geometry;
v_arow_loop record;
BEGIN

select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;

FOR  v_temp_point_entity IN select * from temp_auto_network_plan tp where  tp.cable_id=p_cable_id and loop_length >0 order by system_id
LOOP
	v_current_point_geom=ST_LineInterpolatePoint(v_line_geom, v_temp_point_entity.fraction);
	
	select * into v_loop_network_id  from fn_get_network_code('Loop','Point',0,'',substring(left(St_astext(v_current_point_geom),-1),7));

	INSERT INTO public.att_details_loop(
	network_id, loop_length, associated_system_id, associated_network_id, 
	associated_entity_type, cable_system_id, created_by, created_on, 
	network_status, start_reading, end_reading, 
	parent_system_id, sequence_id, province_id, region_id, parent_network_id, 
	parent_entity_type, status, 
	is_visible_on_map, source_ref_type, source_ref_id, 
	source_ref_description, latitude, longitude, is_new_entity)
	select v_loop_network_id.network_code,loop_length,entity_system_id,entity_network_id,
	entity_type,cable_id,created_by,now(),
	'P',0,0,v_loop_network_id.parent_system_id, v_loop_network_id.sequence_id,province_id, region_id,v_loop_network_id.parent_network_id,
	v_loop_network_id.parent_entity_type,
	'A',true,'planning', plan_id,'',latitude,longitude,true
	from temp_auto_network_plan tp where system_id=v_temp_point_entity.system_id
	RETURNING system_id,network_id,latitude,longitude,created_by into v_arow_loop; 

	INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
	creator_remark,approver_remark,created_by,common_name, network_status,display_name)
	values(v_arow_loop.system_id, 'Loop',st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
	'A',v_current_point_geom,now(),'Created', '' ,v_arow_loop.created_by,v_arow_loop.network_id,
	'P',fn_get_display_name(v_arow_loop.system_id, 'Loop'));

end loop;

		       
END
$function$
;

---------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_convert_to_asbuilt_network(p_plan_id integer, p_user_id integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_status    boolean;
    v_message   character varying;
    v_Geomtablename text;
    rec record;
BEGIN

    v_status := true;
    v_message := 'Success';

    FOR rec IN 
        SELECT layer_table, geom_type, layer_name
        FROM layer_details 
        WHERE layer_name IN ('Cable','SpliceClosure','Duct','Trench','Pole','Manhole')
    LOOP

        -- determine geometry table
        IF lower(rec.geom_type) = 'point' THEN
            v_Geomtablename := 'point_master';

        ELSIF lower(rec.geom_type) = 'line' THEN
            v_Geomtablename := 'line_master';

        ELSIF lower(rec.geom_type) = 'polygon' THEN
            v_Geomtablename := 'polygon_master';
        END IF;

        -- Update layer table
        EXECUTE
            'UPDATE ' || rec.layer_table ||
            ' SET network_status = ''A''' ||
            ' WHERE source_ref_id = ''' || p_plan_id || '''' ||
            ' AND source_ref_type = ''backbone planning''' ||
            ' AND created_by = ' || p_user_id || ';';

        -- Update geometry table
        EXECUTE
            'UPDATE ' || v_Geomtablename ||
            ' SET network_status = ''A''' ||
            ' WHERE source_ref_id = ''' || p_plan_id || '''' ||
            ' AND source_ref_type = ''backbone planning''' ||
            ' AND entity_type = ''' || rec.layer_name || '''' ||
            ' AND created_by = ' || p_user_id || ';';

    END LOOP;

    v_status := true;
    v_message := 'The Plan successfully converted from Planned to As-Built.';

    RETURN QUERY 
        SELECT v_status, v_message;

END
$function$
;

-------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_convert_to_planned_network(p_plan_id integer, p_user_id integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_status    boolean;
    v_message   character varying;
    v_Geomtablename text;
    rec record;
BEGIN

    v_status := true;
    v_message := 'Success';

    FOR rec IN 
        SELECT layer_table, geom_type, layer_name
        FROM layer_details 
        WHERE layer_name IN ('Cable','SpliceClosure','Duct','Trench','Pole','Manhole')
    LOOP

        -- determine geometry table
        IF lower(rec.geom_type) = 'point' THEN
            v_Geomtablename := 'point_master';

        ELSIF lower(rec.geom_type) = 'line' THEN
            v_Geomtablename := 'line_master';

        ELSIF lower(rec.geom_type) = 'polygon' THEN
            v_Geomtablename := 'polygon_master';
        END IF;

        -- Update layer table
        EXECUTE
            'UPDATE ' || rec.layer_table ||
            ' SET network_status = ''P''' ||
            ' WHERE source_ref_id = ''' || p_plan_id || '''' ||
            ' AND source_ref_type = ''backbone planning''' ||
            ' AND created_by = ' || p_user_id || ';';

        -- Update geometry table
        EXECUTE
            'UPDATE ' || v_Geomtablename ||
            ' SET network_status = ''P''' ||
            ' WHERE source_ref_id = ''' || p_plan_id || '''' ||
            ' AND source_ref_type = ''backbone planning''' ||
            ' AND entity_type = ''' || rec.layer_name || '''' ||
            ' AND created_by = ' || p_user_id || ';';

    END LOOP;

    v_status := true;
    v_message := 'The Plan successfully converted from As-Built to Planned.';

    RETURN QUERY 
        SELECT v_status, v_message;

END
$function$
;

------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_delete_sprout_site_network(p_plan_id integer, p_user_id integer, p_site_network_id character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
declare 

v_status boolean;
v_message character varying;
v_Asbuilt_count integer;
BEGIN
v_status:=true;
v_message:='Success';
v_Asbuilt_count:=0;

select SUM(count) into v_Asbuilt_count from (
select count(1) from att_details_cable where source_ref_id=''||p_plan_id||'' and lower(source_ref_type)='backbone planning' and upper(network_status)=upper('A') and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A') and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')and sprout_site_id = ''||p_site_network_id||'')row;

if(v_Asbuilt_count > 0)
then 
	v_status:=false;
	v_message:='Plan Can Not Be Deleted Because Some Entity is AsBuild !';
else
	----cable---
	delete from line_master where system_id in(
	select system_id from att_details_cable where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('cable');

	delete from att_details_cable where source_ref_id=''||p_plan_id||'' 
   and source_ref_type='backboneplanning' and sprout_site_id = ''||p_site_network_id||'';	
   
  ----duct---
	delete from line_master where system_id in(
	select system_id from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('duct');

	delete from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';	

	----trench---
	delete from line_master where system_id in(
	select system_id from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('trench');

	delete from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';	

	----trench---
	delete from line_master where system_id in(
	select system_id from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('spliceclosure');

	delete from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';

	----manhole---
	delete from line_master where system_id in(
	select system_id from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('manhole');

	delete from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';

	----pole---
	delete from point_master where system_id in(
	select system_id from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('pole');

	delete from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';

 -- delete from nearest site table
	delete from backbone_plan_nearest_site where plan_id =p_plan_id and network_id  = ''||p_site_network_id||'';

  delete from backbone_plan_network_details where plan_id =p_plan_id and site_network_id  = ''||p_site_network_id||'';
											
	v_status=true;
	v_message= 'The sprout network '|| p_site_network_id ||' has been deleted Successfully!'; 
end if;

return query select v_status,v_message::character varying;
END
$function$
;

--------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_backbone_update_sprout_network(p_linegeom character varying, p_systemid integer, p_plan_id integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_line_geom geometry;
    v_line_geom_length double precision;
	current_fraction double precision := 0.0;
	structure_location geometry;
	p_pole_span integer;
	p_manhole_span integer;
	p_backbone_fiber_type character varying;
	line_geom geometry;
	p_is_looprequired bool;
	p_looplength  double precision;
	step_fraction double precision;
	P_SITE_NETWORK_ID character varying;
    V_closet_point character varying;
    V_is_update bool;
   sp_route_length double precision;
   sp_totalroute_length double precision;
   p_nearest_pole_manhole geometry;
	p_threshold double precision;
	p_cable_drum_length double precision;
    p_cable_type character varying;
   v_entity_type character varying;
   v_geometry geometry;
   updated_line_geom geometry;
  p_sprout_cable_name character varying;
 p_loop_span double precision;
begin
	
-- get all requied fields to use 	
select bpd.pole_distance ,bpd.manhole_distance,bpd.loop_length ,bpd.is_loop_required,bpd.threshold,bpd.cable_length,bpd.loop_span  into p_pole_span,p_manhole_span,p_looplength,p_is_looprequired,p_threshold,p_cable_drum_length ,p_loop_span from backbone_plan_details bpd where plan_id = p_plan_id;
			
select fibertype,network_id,intersect_line_geom,is_update 
into p_backbone_fiber_type,P_SITE_NETWORK_ID, V_closet_point,V_is_update 
from backbone_plan_nearest_site bpns where id = p_systemid;	
----------------------------------------------------------------------
 
	select site_id ||'-' || site_name into p_sprout_cable_name from att_details_pod 
    where network_id = P_SITE_NETWORK_ID limit 1;

 	UPDATE backbone_plan_nearest_site
    SET line_geom = p_linegeom
    WHERE id = p_systemid AND plan_id = p_plan_id;
   
   -- when update srout draft network so should delete previous record and process the draft network for newly re-update record
	if(V_is_update)
	then 
	delete from backbone_plan_network_details bpnd 
	where site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id;

	end if;
-------------------------------------------------------------------------

	select ST_GeomFromText(concat('LINESTRING(', p_linegeom, ')'), 4326) into v_line_geom;
		
   select st_length(v_line_geom,false) into v_line_geom_length;
  
  v_geometry = ST_GeomFromText(
    concat('POINT(', trim(split_part(V_closet_point, ',', 2)), ' ', trim(split_part(V_closet_point, ',', 1)), ')'),
    4326
);
-- Check case sprout line geom length is less then cable drum length or sprout line geom is less then manhole span length or sprout line geom is less then pole span length then create one cable,sc, manhole or pole (because this is not possible to create multiple cable, pole, manhole )

	IF (v_line_geom_length < p_cable_drum_length or v_line_geom_length < p_manhole_span   or v_line_geom_length < p_pole_span) THEN

	if NOT EXISTS (
	        SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_geometry)
	          AND entity_type = 'RestrictedArea')
	 then 
	v_entity_type ='Pole';
    p_cable_type := 'Overhead';
	else 
	v_entity_type ='Manhole';
     p_cable_type := 'Underground';
	end if;

	END IF;

	/* IF (v_line_geom_length < p_manhole_span  ) 
	 THEN
      if not EXISTS (SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_geometry)
	          AND entity_type = 'RestrictedArea') then 
	          v_entity_type ='Pole';
	          p_cable_type := 'Overhead';
			else 
			v_entity_type ='Manhole';
		    p_cable_type := 'Underground';
	          
	       end if;
	  end if;
	 
 	IF (v_line_geom_length < p_pole_span) 
	 THEN
      if not EXISTS (SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_geometry)
	          AND entity_type = 'RestrictedArea') then 
	          v_entity_type ='Pole';
	          p_cable_type := 'Overhead';
			else 
			 v_entity_type ='Manhole';
			 p_cable_type := 'Underground';
	       end if;
	  end if;
      */
    if v_entity_type in ('Pole','Manhole')
    then   
    
    IF EXISTS (select 1 FROM backbone_plan_network_details
    	WHERE st_within(sp_geometry,
            st_buffer_meters(v_geometry, 200))
            AND plan_id = p_plan_id  and planned_cable_entity ='BackBone Cable'
            and entity_type in ('Pole','Manhole'))
     then

        SELECT ST_AddPoint(v_line_geom,sp_geometry, 0)  into updated_line_geom       
		FROM backbone_plan_network_details bpn
		WHERE bpn.plan_id = p_plan_id
		  AND bpn.entity_type IN ('Pole','Manhole') and 
		  planned_cable_entity ='BackBone Cable'
		  AND ST_Within(bpn.sp_geometry, st_buffer_meters(v_geometry, 200))
	   ORDER BY ST_Distance(bpn.sp_geometry, v_geometry)
	   LIMIT 1;
	  
    -- Insert Cable entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type, line_sp_geometry,
        planned_cable_entity, site_network_id,region_id ,province_id ,sp_geometry ,longitude ,latitude ,entity_name,cable_type 
    )
    VALUES (
        p_plan_id, 'Cable', p_user_id, p_backbone_fiber_type, updated_line_geom,
        'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(updated_line_geom), ST_X(ST_startPoint(updated_line_geom)),ST_Y(ST_startPoint(updated_line_geom)),p_sprout_cable_name,p_cable_type
    );
   else
	 
    -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type,
        loop_length, is_loop_required, planned_cable_entity, site_network_id,region_id ,province_id,sp_geometry ,longitude ,latitude  
    )
    VALUES (
        p_plan_id, v_entity_type,  p_user_id, p_backbone_fiber_type, 
        0, p_is_looprequired, 'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(updated_line_geom), ST_X(ST_startPoint(updated_line_geom)),ST_Y(ST_startPoint(updated_line_geom)));

     -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type,
         planned_cable_entity, site_network_id,region_id ,province_id ,sp_geometry ,longitude ,latitude 
    )
    VALUES (
        p_plan_id, 'SpliceClosure', p_user_id, p_backbone_fiber_type,
         'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(updated_line_geom), ST_X(ST_startPoint(updated_line_geom)),ST_Y(ST_startPoint(updated_line_geom))
    );

      -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type, line_sp_geometry,
        planned_cable_entity, site_network_id,region_id ,province_id ,sp_geometry ,longitude ,latitude ,entity_name,cable_type
    )
    VALUES (
        p_plan_id, 'Cable', p_user_id, p_backbone_fiber_type, updated_line_geom,
        'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(updated_line_geom), ST_X(ST_startPoint(updated_line_geom)),ST_Y(ST_startPoint(updated_line_geom)),
        p_sprout_cable_name,p_cable_type
    );
   END IF;
  
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
   into sp_route_length from backbone_plan_network_details
   where site_network_id = P_SITE_NETWORK_ID and entity_type = 'Cable' and 
 	plan_id = P_plan_id and line_sp_geometry is not null;
 
   -- total sprout cable 
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where entity_type = 'Cable' and plan_id = p_plan_id;
   
   UPDATE backbone_plan_network_details AS np
SET province_id = pm.id,
    region_id   = rm.id
FROM region_boundary AS rm,
     province_boundary AS pm
WHERE rm.isvisible = TRUE
  AND pm.isvisible = TRUE
  AND ST_Intersects(
         pm.sp_geometry,
         np.sp_geometry 
     )
  AND ST_Intersects(
         rm.sp_geometry,
          np.sp_geometry 
     )
     and np.planned_cable_entity ='Sprout Cable'
  AND np.plan_id  = P_plan_id
  AND np.created_by = p_user_id and np.sp_geometry is not null and site_network_id = P_SITE_NETWORK_ID;
 
  perform (fn_backbone_draft_update_loop_network(p_linegeom, P_plan_id, p_is_looprequired,p_user_id, p_loop_span,p_looplength,P_SITE_NETWORK_ID));
 
  RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;
return;

	
END IF;
  
      WHILE current_fraction < 1.0 LOOP
        -- Try pole first
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_pole_span / v_line_geom_length), 1.0));

        -- Check if it's in a restricted area
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));

            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,loop_length,is_loop_required  ,planned_cable_entity,site_network_id ,province_id ,region_id 
            ) VALUES (
                p_plan_id, 'Manhole', ST_X(ST_EndPoint(line_geom)),ST_Y(ST_EndPoint(line_geom)),
                ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction,0,p_is_looprequired,
                'Sprout Cable',P_SITE_NETWORK_ID,0,0
            );           
           
        else
        	line_geom = NULL;
            step_fraction := p_pole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));

            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,fraction ,planned_cable_entity,site_network_id ,province_id,region_id ,loop_length ,is_loop_required 
            ) 
            VALUES (p_plan_id, 'Pole', ST_X(ST_EndPoint(line_geom)) ,ST_Y(ST_EndPoint(line_geom)),ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,current_fraction,'Sprout Cable',P_SITE_NETWORK_ID,0,0,0,p_is_looprequired
            );

        END IF;

        -- Move to next step
        current_fraction := current_fraction + step_fraction;
        EXIT WHEN current_fraction >= 1.0;
    END LOOP;  
   
   	delete from backbone_plan_network_details where system_id =(
   select system_id from backbone_plan_network_details where plan_id = p_plan_id and 
   site_network_id =P_SITE_NETWORK_ID and entity_type in ('Manhole','Pole') order by system_id desc limit 1 );
  
   current_fraction := 0.0;

    WHILE current_fraction < 1.0 LOOP
       
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_cable_drum_length / v_line_geom_length), 1.0));         

        step_fraction := p_cable_drum_length / v_line_geom_length;
       
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
           

        -- cable type logic
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            p_cable_type := 'Underground';
           v_entity_type = 'Manhole';
        ELSE
            p_cable_type := 'Overhead';
           v_entity_type = 'Pole';
        END IF;
       
RAISE INFO 'line_geom = %', ST_AsText(line_geom);
RAISE INFO 'cable_type geom = %', p_cable_type;


        -- insert cable
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, created_by, fiber_type, fraction, planned_cable_entity, cable_type, line_sp_geometry, sp_geometry,site_network_id ,entity_name
        )
        VALUES (
            p_plan_id, 'Cable', p_user_id, p_backbone_fiber_type,
            current_fraction, 'Sprout Cable', p_cable_type, line_geom, ST_STARTPoint(line_geom),P_SITE_NETWORK_ID,p_sprout_cable_name
        );

        -- insert splice closure at segment end
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity,site_network_id 
        )
        VALUES (
            p_plan_id, 'SpliceClosure',
            ST_X(ST_STARTPoint(line_geom)), ST_Y(ST_STARTPoint(line_geom)),
            ST_STARTPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'Sprout Cable',P_SITE_NETWORK_ID
        );
       
        if not exists (SELECT 1 FROM backbone_plan_network_details
            WHERE plan_id = p_plan_id and ST_Intersects(sp_geometry, structure_location) and planned_cable_entity
='Sprout Cable' and site_network_id = P_SITE_NETWORK_ID AND entity_type = v_entity_type and sp_geometry is not null) 
              then 
		INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity,site_network_id ,loop_length ,is_loop_required 
        )
        VALUES (
            p_plan_id, v_entity_type , ST_X(ST_STARTPoint(line_geom)), ST_Y(ST_STARTPoint(line_geom)),
            ST_STARTPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'Sprout Cable',P_SITE_NETWORK_ID,0,p_is_looprequired
        );
       end if;
      
        current_fraction := current_fraction + step_fraction;

        EXIT WHEN current_fraction >= 1.0;
    END LOOP;
  
     IF EXISTS (select 1 FROM backbone_plan_network_details
    		WHERE st_within(sp_geometry,
            st_buffer_meters(v_geometry, 200))
            AND plan_id = p_plan_id  and planned_cable_entity ='BackBone Cable' and entity_type in ('Pole','Manhole'))
     then
     raise info 'test %',st_astext(v_geometry);

        SELECT sp_geometry into p_nearest_pole_manhole       
		FROM backbone_plan_network_details bpn
		WHERE bpn.plan_id = p_plan_id
		  AND bpn.entity_type  IN ('Pole','Manhole') and 
		  planned_cable_entity ='BackBone Cable'
		  AND st_within(sp_geometry,
            st_buffer_meters(v_geometry, 200))
	   ORDER BY ST_Distance(bpn.sp_geometry, v_geometry)
	   LIMIT 1;
	  RAISE INFO 'p_nearest_pole_manhole = %', ST_AsText(p_nearest_pole_manhole);

	  -- update  line geom if exists around pole, manhole	  	  
	  update backbone_plan_network_details set line_sp_geometry  = ST_AddPoint(line_sp_geometry,p_nearest_pole_manhole,0) 
	  where fraction = 0 and site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id
	  and entity_type  = 'Cable';
	
	  update backbone_plan_network_details set sp_geometry = v_geometry ,
	  latitude = ST_Y(v_geometry) , longitude = ST_X(v_geometry)
	  where fraction = 0 and site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id 
	  and entity_type = 'SpliceClosure';
	 	 
	 
	  end if;
	  
	 select sum(round(ST_Length(line_sp_geometry, false)::numeric,3)) into sp_route_length 
    from backbone_plan_network_details where site_network_id = P_SITE_NETWORK_ID and entity_type ='Cable'
    and plan_id = p_plan_id;
   
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where planned_cable_entity = 'Sprout Cable' and entity_type ='Cable' and plan_id = p_plan_id;
   
UPDATE backbone_plan_network_details AS np
SET province_id = pm.id,
    region_id   = rm.id
FROM region_boundary AS rm,
     province_boundary AS pm
WHERE rm.isvisible = TRUE
  AND pm.isvisible = TRUE
  AND ST_Intersects(
         pm.sp_geometry,
         np.sp_geometry 
     )
  AND ST_Intersects(
         rm.sp_geometry,
          np.sp_geometry 
     )
  and np.planned_cable_entity ='Sprout Cable'
  AND np.plan_id  = P_plan_id
  AND np.created_by = p_user_id and np.sp_geometry is not null;

  perform (fn_backbone_draft_update_loop_network(p_linegeom, P_plan_id, p_is_looprequired,p_user_id, p_loop_span,p_looplength,P_SITE_NETWORK_ID));
 
 RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;

END;
$function$
;


----------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_nearest_sites(line_geom character varying, buffer double precision, planid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_line geometry;
    v_buffer geometry;
    v_buffer_geojson json;
    v_sites json;
    v_total_length double precision := 0;
BEGIN
    IF line_geom IS NULL OR trim(line_geom) = '' THEN
        RETURN;
    END IF;

    -- Convert input text line into geometry
    SELECT ST_GeomFromText('LINESTRING(' || line_geom || ')', 4326)
    INTO v_line;

    -- Create a buffer around the line
    SELECT ST_Buffer(v_line::geography, buffer)::geometry
    INTO v_buffer;

    -- Convert buffer to GeoJSON
    SELECT ST_AsGeoJSON(v_buffer)::json
    INTO v_buffer_geojson;

    IF planid > 0 THEN
        IF EXISTS (
            SELECT 1
            FROM backbone_plan_nearest_site
            WHERE plan_id = planid
        ) THEN
            WITH s_all AS (
                SELECT DISTINCT ON (bpns.network_id)
                    bpns.network_id,
                    bpns.site_id,
                    bpns.site_name,
                    bpns.is_selected,
                    bpns.fibertype,
                    bpns.intersect_line_geom AS cable_geom,
                    bpns.site_geom,
                    bpns.line_geom,
                     COALESCE(ROUND(ST_Length(ST_GeomFromText('LINESTRING(' || bpns.line_geom || ')', 4326)::geography)::numeric, 3), 0) AS sprout_route_length,
                  bpns.id  
                FROM backbone_plan_nearest_site bpns
                WHERE bpns.plan_id = planid

                UNION

                SELECT
    adp.network_id,
    adp.site_id,
    adp.site_name,
    FALSE AS is_selected,
    '' AS fibertype,
    '' AS cable_geom,
    '' AS site_geom,
    '' AS line_geom,
    0 AS sprout_route_length,
    0 AS id
FROM point_master pm
JOIN att_details_pod adp
  ON pm.system_id = adp.system_id
WHERE 
    UPPER(pm.entity_type) = 'POD'
    AND adp.status = 'A'
    AND pm.network_status IN ('A')
    AND ST_Within(pm.sp_geometry, v_buffer)
    AND adp.network_id NOT IN (
        SELECT network_id
        FROM backbone_plan_nearest_site
        WHERE plan_id = planid
    )
    AND NOT EXISTS (
        SELECT 1
        FROM line_master lm
        WHERE lm.entity_type = 'Cable'
          AND (
                ST_DWithin(
                    ST_Transform(pm.sp_geometry, 3857),
                    ST_Transform(ST_StartPoint(lm.sp_geometry), 3857),
                    1  -- 1 meter tolerance
                )
                OR
                ST_DWithin(
                    ST_Transform(pm.sp_geometry, 3857),
                    ST_Transform(ST_EndPoint(lm.sp_geometry), 3857),
                    1
                )
              )
    )

                  --order by adp.network_id
            )
            SELECT 
                json_agg(
                    json_build_object(
                        'network_id', s_all.network_id,
                        'site_id', s_all.site_id,
                        'site_name', s_all.site_name,
                        'is_selected', s_all.is_selected,
                        'fibertype', s_all.fibertype,
                        'backbone_geom', s_all.cable_geom,
                        'geometry', s_all.site_geom,
                        'line_geom', s_all.line_geom,
                        'sprout_route_length', COALESCE(s_all.sprout_route_length, 0),
                        'id',s_all.id
                    )
                ),
                SUM(s_all.sprout_route_length)
            INTO v_sites, v_total_length
            FROM (select * from s_all order by s_all.network_id) s_all;

        ELSE
            SELECT json_agg(
                json_build_object(
                    'network_id', adp.network_id,
                    'site_id', adp.site_id,
                    'site_name', adp.site_name,
                    'is_selected', FALSE,
                    'fibertype', '',
                    'backbone_geom', '',
                    'geometry', '',
                    'line_geom', '',
                    'sprout_route_length', 0,
                    'id',0
                )
            )
            INTO v_sites
            FROM (
              SELECT
    adp.network_id,
    adp.site_id,
    adp.site_name,
    FALSE AS is_selected,
    '' AS fibertype,
    '' AS cable_geom,
    '' AS site_geom,
    '' AS line_geom,
    0 AS sprout_route_length,
    0 AS id
FROM point_master pm
JOIN att_details_pod adp
  ON pm.system_id = adp.system_id
WHERE 
    UPPER(pm.entity_type) = 'POD'
    AND adp.status = 'A'
    AND pm.network_status IN ('A')
    AND ST_Within(pm.sp_geometry, v_buffer)  
    AND NOT EXISTS (
        SELECT 1
        FROM line_master lm
        WHERE lm.entity_type = 'Cable'
          AND (
                ST_DWithin(
                    ST_Transform(pm.sp_geometry, 3857),
                    ST_Transform(ST_StartPoint(lm.sp_geometry), 3857),
                    1  -- 1 meter tolerance
                )
                OR
                ST_DWithin(
                    ST_Transform(pm.sp_geometry, 3857),
                    ST_Transform(ST_EndPoint(lm.sp_geometry), 3857),
                    1
                )
              )
    )
            ) adp;

          --  v_total_length := 0;
        END IF;
    END IF;

    RETURN NEXT json_build_object(
        'buffer_geometry', v_buffer_geojson,
        'sites', COALESCE(v_sites, '[]'::json),
        'total_sprout_length', ROUND(CAST(COALESCE(v_total_length, 0) AS numeric), 3)
    );
END;
$function$
;

------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_bom(p_plan_id integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
sql TEXT := '';
v_loop_length double precision;
v_calculated_loop_length double precision;
v_loop_entity_count integer;
k_specification character  varying;
k_code character  varying;
is_create_loop boolean;
begin
	
select count(1)>0 into is_create_loop  from backbone_plan_network_details bpnd where  plan_id = p_plan_id and is_loop_required = true;

SELECT  substring(fiber_type FROM '\(([^)]+)\)'), split_part(fiber_type, ')', 2) into k_code, k_specification   
FROM backbone_plan_network_details
WHERE plan_id = p_plan_id
  AND entity_type = 'Manhole'
LIMIT 1;

    -- Cable
    sql :=  'SELECT
     entity_type,
     geom_type,
     SUM(cost_per_unit)         AS cost_per_unit,
     SUM(service_cost_per_unit) AS service_cost_per_unit,
     ROUND(SUM(measured_length)::numeric, 2) || ''(m)/'' || SUM(length_qty)::int AS length_qty,
     SUM(amount) AS amount
 FROM (
     SELECT
         ''BackBone Cable''                              AS entity_type,
         ''Line''                                        AS geom_type,
         COALESCE(item.cost_per_unit,        0)          AS cost_per_unit,
         COALESCE(item.service_cost_per_unit,0)          AS service_cost_per_unit,
         COUNT(att.system_id)                            AS length_qty,
         SUM(att.cable_measured_length)                  AS measured_length,
         -- total for this cable slice
         (SUM(att.cable_measured_length) * COALESCE(item.service_cost_per_unit,0) +
          COALESCE(item.cost_per_unit,0)  * SUM(att.cable_measured_length)) AS amount
       FROM att_details_cable  att
       join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Cable''
       JOIN item_template_master item
         ON item.audit_id = att.audit_item_master_id
      WHERE att.source_ref_id      = ' || quote_literal(p_plan_id) || '
        AND att.source_ref_type    = ''backbone planning''
        AND item.category_reference = ''Cable'' and sprout_site_id is null
      GROUP BY item.cost_per_unit,
               item.service_cost_per_unit,
               att.specification,
               att.item_code, lm.sp_geometry
 ) sub
 GROUP BY entity_type, geom_type';

 IF EXISTS (SELECT 1 FROM att_details_cable WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id ::varchar and sprout_site_id is not null) THEN
      sql := sql || '
       UNION 
      SELECT
      entity_type,
     geom_type,
     SUM(cost_per_unit)         AS cost_per_unit,
     SUM(service_cost_per_unit) AS service_cost_per_unit,
     ROUND(SUM(measured_length)::numeric, 2) || ''(m)/'' || SUM(length_qty)::int AS length_qty,
     SUM(amount) AS amount
 FROM (
     SELECT
         ''Sprout Cable''                                AS entity_type,
         ''Line''                                        AS geom_type,
         COALESCE(item.cost_per_unit,        0)          AS cost_per_unit,
         COALESCE(item.service_cost_per_unit,0)          AS service_cost_per_unit,
         COUNT(att.system_id)                            AS length_qty,
         SUM(att.cable_measured_length)                  AS measured_length,
         -- total for this cable slice
         (SUM(att.cable_measured_length) * COALESCE(item.service_cost_per_unit,0) +
          COALESCE(item.cost_per_unit,0)  * SUM(att.cable_measured_length)) AS amount
       FROM att_details_cable  att
       join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Cable''
       JOIN item_template_master item
         ON item.audit_id = att.audit_item_master_id
      WHERE att.source_ref_id      = ' || quote_literal(p_plan_id) || '
        AND att.source_ref_type    = ''backbone planning''
        AND item.category_reference = ''Cable'' and sprout_site_id is not null
      GROUP BY item.cost_per_unit,
               item.service_cost_per_unit,
               att.specification,
               att.item_code, lm.sp_geometry
 ) sub
 GROUP BY entity_type, geom_type';
END IF;
-- Pole
IF EXISTS (SELECT 1 FROM att_details_pole WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id ::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''Pole'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(1) AS length_qty
            FROM att_details_pole att
            join point_master pm on pm.system_id = att.system_id
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''Pole'' and item.specification = ''Generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;

    -- Manhole
    IF EXISTS (SELECT 1 FROM att_details_manhole WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''Manhole'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                item.cost_per_unit AS cost_per_unit,
                item.service_cost_per_unit AS service_cost_per_unit,
                COUNT(1) AS length_qty
            FROM att_details_manhole att
            join point_master pm on pm.system_id = att.system_id
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''Manhole'' and item.specification = ''Generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;

    -- SpliceClosure
    IF EXISTS (SELECT 1 FROM att_details_spliceclosure WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''SpliceClosure'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(att.system_id) AS length_qty
            FROM att_details_spliceclosure att
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''SpliceClosure'' and item.specification = ''generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;
 if is_create_loop = true then   
sql:=sql ||' UNION SELECT 
    ''Loop'', 
    ''Point'', 
    0 AS cost_per_unit, 
    0 AS service_cost_per_unit,
    (loop_data.loop_length || ''(m)/''||  loop_data.loop_count) ::character varying  AS length_qty,
    0 AS amount
FROM (
    SELECT sum(loop_length) as loop_length ,count(1) as loop_count from att_details_loop  where source_ref_type = ''backbone planning'' and source_ref_id='''||p_plan_id||''' 
        
) AS loop_data';
end if;

if exists (SELECT 1 FROM att_details_trench WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) 
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty ,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount
	 from (
	select (select layer_title from layer_details where layer_name ilike ''trench'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty, st_length(lm.sp_geometry,false) as measured_length 
from att_details_trench att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Trench''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where att.source_ref_type = ''backbone planning'' and att.source_ref_id='''||p_plan_id||'''  GROUP BY
	 ITEM.COST_PER_UNIT,ITEM.SERVICE_COST_PER_UNIT,ATT.SPECIFICATION,ATT.ITEM_CODE,VM.NAME) e';
end if;

if exists (SELECT 1 FROM att_details_duct WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar)
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount
	 from 
(select (select layer_title from layer_details where layer_name ilike ''duct'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty,sum(calculated_length) as measured_length
from att_details_duct att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Duct''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where att.source_ref_type = ''backbone planning'' and att.source_ref_id='''||p_plan_id||'''  GROUP BY
	 ITEM.COST_PER_UNIT,ITEM.SERVICE_COST_PER_UNIT,ATT.SPECIFICATION,ATT.ITEM_CODE,VM.NAME) e';
end if;

    -- Debug SQL
    RAISE INFO 'Final SQL: %', sql;

    -- Execute the full dynamic query and return as JSON
    RETURN QUERY EXECUTE '
        SELECT row_to_json(row) 
        FROM (
            SELECT * FROM (' || sql || ') t 
            ORDER BY geom_type
        ) row';

END;
$function$
;


----------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_save_plan(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, backbone_plan_id integer)
 RETURNS TABLE(status boolean, message character varying, v_plan_id integer)
 LANGUAGE plpgsql
AS $function$

DECLARE
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow_cable record;
v_line_geom geometry;
v_region_province record;
v_arow_closure record;
v_arow_prev_closure record;
v_line_total_length DOUBLE PRECISION;
p_rfs_type character varying;
--backbone_plan_id integer;
v_temp_point_entity record;
v_geom_str character varying;
p_cable_type character varying;
p_skip_last_sc integer;
p_skip_last_manhole_pole integer;
v_geom geometry;
v_result TEXT;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';

--truncate table temp_generate_network_code;
--truncate table temp_point_geom;

create temp table temp_generate_network_code(
status boolean,
network_id character varying,
entity_type character varying,
sequence_id integer,
province_id integer,
network_code character varying,
parent_entity_type character varying
) on commit drop;

create temp table temp_point_geom(
entity_type character varying,
sp_geometry character varying
) on commit drop;

select system_id into p_skip_last_manhole_pole from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole','Pole') and planned_cable_entity = 'BackBone Cable' order by system_id desc limit 1;

select system_id into p_skip_last_sc from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('SpliceClosure') and planned_cable_entity = 'BackBone Cable' order by system_id desc limit 1;
EXECUTE 'ALTER TABLE point_master DISABLE TRIGGER ALL';

----- insert manhole
INSERT INTO att_details_manhole(manhole_name ,is_visible_on_map,ownership_type,
longitude, latitude,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status,
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,category,backbone_system_id,sprout_site_id)

SELECT entity_name ,true,'Own',
longitude,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id,region_id ,province_id,'Manhole' ,system_id,site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and sp_geometry is not NULL order by system_id;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'Manhole',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.manhole_name ,backbone_plan_id,'backbone planning'
from att_details_manhole mh
left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='Manhole' where pm.system_id is null;

UPDATE att_details_manhole adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'Manhole'
AND specification = 'Generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

---------insert sc

INSERT INTO att_details_spliceclosure (is_visible_on_map,ownership_type,
longitude, latitude,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status,
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,category,backbone_system_id,sprout_site_id)

SELECT true,'Own',
longitude ,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id,region_id ,province_id ,'SpliceClosure',system_id,site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('SpliceClosure') and sp_geometry is not NULL order by system_id;


INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'SpliceClosure',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.spliceclosure_name ,backbone_plan_id,'backbone planning'
from att_details_spliceclosure mh
left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='SpliceClosure' where pm.system_id is null;


UPDATE att_details_spliceclosure adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id,
no_of_ports = itm.no_of_port,
no_of_input_port= itm.no_of_input_port,
no_of_output_port = itm.no_of_output_port
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'SpliceClosure'
AND specification = 'generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

-----------insert pole

INSERT INTO att_details_pole(pole_name ,is_visible_on_map,ownership_type,
longitude, latitude,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status,
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,pole_type,category,backbone_system_id,sprout_site_id)

SELECT entity_name ,true,'Own',
longitude ,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id::varchar,region_id ,province_id ,'Electrical','Pole',system_id,site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Pole') and sp_geometry is not NULL order by system_id;


INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'Pole',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.pole_name ,backbone_plan_id::varchar,'backbone planning'
from att_details_pole mh
left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='Pole' where pm.system_id is null;


UPDATE att_details_pole adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'Pole'
AND specification = 'Generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

----------- insert cable
INSERT INTO ATT_DETAILS_CABLE(cable_name ,is_visible_on_map,total_core,no_of_tube,no_of_core_per_tube,cable_measured_length,cable_calculated_length,
cable_type,specification,category,subcategory1,subcategory2,subcategory3,item_code,
vendor_id,network_status,status,created_by,created_on,cable_category,start_reading,
end_reading,structure_id,utilization,duct_id,trench_id,is_used,source_ref_type,source_ref_id,ownership_type,region_id,province_id,backbone_system_id,a_system_id,b_system_id,sprout_site_id)
select entity_name,true,0,0,0,ST_Length(plan.line_sp_geometry,false),
ST_Length(plan.line_sp_geometry,false),plan.cable_type ,plan.fiber_type,'Cable', null, null, null,null,
0,'P','A',p_user_id,now(),case when plan.planned_cable_entity = 'BackBone Cable' then 'BackBone Cable' else 'Sprout Cable' end,
0,0,0,'L',0,0,true,'backbone planning',backbone_plan_id,'OWN',region_id,province_id , plan.system_id,0,0,plan.site_network_id 
from backbone_plan_network_details plan where plan.plan_id = backbone_plan_id and plan.created_by = p_user_id and plan.entity_type = 'Cable' and plan.line_sp_geometry is not null order by system_id;

EXECUTE 'ALTER TABLE line_master DISABLE TRIGGER ALL';

INSERT INTO LINE_MASTER(system_id,common_name ,display_name ,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,network_status,is_virtual,source_ref_type,source_ref_id,sp_geometry,status)
select cbl.system_id,cbl.network_id,cbl.cable_name ,'Cable','A','','', p_user_id,0 ,'P',false,'backbone planning',backbone_plan_id , np.line_sp_geometry ,'A'
from ATT_DETAILS_CABLE cbl
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id
where cbl.system_id not in (select system_id from line_master where entity_type='Cable') and np.entity_type = 'Cable' and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning' and np.line_sp_geometry is not null;

UPDATE att_details_cable adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id,
total_core = itm.other::integer,
no_of_tube = itm.no_of_tube,
no_of_core_per_tube = itm.no_of_core_per_tube
from
(select adm.system_id ,itm.* FROM att_details_cable adm
join item_template_master itm
on category_reference = 'Cable'
AND '(' || code || ')' || other = adm.specification
where adm.source_ref_id = backbone_plan_id::varchar
AND adm.source_ref_type = 'backbone planning'
) itm
WHERE adm.system_id = itm.system_id;

----------- insert trench
if is_create_trench = true then
insert into att_details_trench(is_visible_on_map,trench_name,trench_width,trench_height,customer_count,trench_type,no_of_ducts,network_status,status,pin_code,province_id,region_id,construction,activation,accessibility,category,subcategory1,subcategory2,subcategory3,vendor_id,type,
brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,mcgm_ward,strata_type,
is_used,source_ref_type,source_ref_id,ownership_type,backbone_system_id,a_system_id,b_system_id,sprout_site_id)
select true,'',0,0,0,'HDD',0,'P','A','',province_id,region_id,0,0,0,
'Trench' ,null,null,null, 0,0,0,0,'',p_user_id,now(),0 ,0,0,0,'','',false,'backbone planning',backbone_plan_id,'OWN',tp.system_id ,0,0,tp.site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and tp.line_sp_geometry is not null and created_by = p_user_id;

INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name,source_ref_id,source_ref_type,sp_geometry,status)
select cbl.system_id,'Trench','A','','', p_user_id,0 ,'','P',false,'',backbone_plan_id,'backbone planning',np.line_sp_geometry,'A'
from att_details_trench cbl
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id where cbl.system_id not in (select system_id from line_master where entity_type='Trench') and np.entity_type in ('Manhole') and np.line_sp_geometry is not null and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

UPDATE att_details_trench adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'Trench'
AND specification = 'generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

end if;

-------- insert duct
if is_create_duct = true then
insert into att_details_duct(is_visible_on_map,duct_name,calculated_length,manual_length,network_status,
status,pin_code,province_id,region_id,utilization,no_of_cables,offset_value,construction,activation,accessibility,specification,category,subcategory1,subcategory2,
subcategory3,item_code,vendor_id,type,brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,inner_dimension,outer_dimension,
is_used,source_ref_type,source_ref_id,trench_id,ownership_type,backbone_system_id,a_system_id,b_system_id,sprout_site_id)
select true,'',ST_Length(tp.line_sp_geometry,false),ST_Length(tp.line_sp_geometry,false),'P','A','',province_id,region_id,'L',0,0,0,0,0,'','Duct',null,null,null,'',0,0,0,0,'',p_user_id,now(),0 ,0,0,0,0,0,false,'backbone planning',backbone_plan_id,0,'OWN',tp.system_id ,0,0,tp.site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and created_by = p_user_id;

INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name,source_ref_id,source_ref_type,sp_geometry,status)
select cbl.system_id,'Duct','A','','', p_user_id,0 ,'','P',false,'',backbone_plan_id,'backbone planning' ,np.line_sp_geometry ,'A'
from att_details_duct cbl
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id where cbl.system_id not in (select system_id from line_master where entity_type='Duct') and np.entity_type ='Manhole' and np.line_sp_geometry is not null and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

UPDATE att_details_duct adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'Duct'
AND specification = 'generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';
end if;

FOR v_temp_point_entity IN
SELECT province_id
FROM backbone_plan_network_details
WHERE plan_id = backbone_plan_id
AND created_by = p_user_id
AND province_id IS NOT NULL
GROUP BY province_id
loop

INSERT INTO temp_point_geom(entity_type, sp_geometry)
SELECT 'Manhole', geom_text FROM (
SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type = 'Manhole'
-- and system_id not in (p_skip_last_manhole_pole)
AND sp_geometry IS NOT NULL
LIMIT 1
) AS manhole_geom

UNION ALL

SELECT 'Pole', geom_text FROM (
SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type = 'Pole'
--and system_id not in (p_skip_last_manhole_pole)
AND sp_geometry IS NOT NULL
LIMIT 1
) AS pole_geom

UNION ALL

SELECT 'Loop', geom_text FROM (
SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id and is_loop_required = true and loop_length > 0
AND entity_type in ('Manhole','Pole')
-- and system_id not in (p_skip_last_manhole_pole)
AND sp_geometry IS NOT NULL
LIMIT 1
) AS loop_geom

UNION ALL

SELECT 'SpliceClosure', geom_text FROM (
SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type IN ('SpliceClosure')
-- and system_id not in (p_skip_last_sc)
AND sp_geometry IS NOT NULL
LIMIT 1
) AS spliceclosure_geom;


--- generaTE NETWORK CODE FOR POINT ENTITY
INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id, network_code, parent_entity_type)
SELECT f.status, f.message, t.entity_type, f.o_sequence_id, v_temp_point_entity.province_id, f.o_p_network_id, f.o_p_entity_type
FROM temp_point_geom t,
LATERAL fn_get_clone_network_code(
t.entity_type,
'Point',
t.sp_geometry,
0,
''
) AS f
WHERE t.sp_geometry IS NOT NULL;

delete from temp_generate_network_code cc where cc.status = false;

-------------------update network id
PERFORM fn_backbone_update_network_code('att_details_manhole', 'Manhole',backbone_plan_id::text , 'backbone planning','manhole_name');
PERFORM fn_backbone_update_network_code('att_details_pole', 'Pole',backbone_plan_id::text, 'backbone planning', 'pole_name');
PERFORM fn_backbone_update_network_code('att_details_spliceclosure', 'SpliceClosure',backbone_plan_id::text, 'backbone planning', 'spliceclosure_name' );

---- generate network code cable

SELECT line_sp_geometry
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type in ('Cable')
AND line_sp_geometry IS NOT NULL
LIMIT 1;

if v_geom is not null
then
select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

if not exists (select 1 from temp_generate_network_code where province_id = v_temp_point_entity.province_id and entity_type = 'Cable') 
then 
INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Cable', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Cable','','',v_geom_str,'OSP') AS f;

PERFORM fn_backbone_update_network_code('att_details_cable', 'Cable', backbone_plan_id::text, 'backbone planning','cable_name');

end if;


end if;

--- generate network codes for trench and duct
if is_create_trench = true then
SELECT line_sp_geometry
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type in ('Manhole')
AND line_sp_geometry IS NOT NULL
LIMIT 1;

if v_geom is not null
then
select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

if not exists (select 1 from temp_generate_network_code where province_id = v_temp_point_entity.province_id and entity_type = 'Trench') 
then 
INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Trench', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Trench','','',v_geom_str,'OSP'::character varying,0,'') as f;

PERFORM fn_backbone_update_network_code('att_details_trench', 'Trench', backbone_plan_id::text, 'backbone planning','trench_name');
end if;

end if;
end if;

if is_create_duct = true
then
SELECT line_sp_geometry
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type in ('Manhole')
AND line_sp_geometry IS NOT NULL
LIMIT 1;

if v_geom is not null
then
select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

if not exists (select 1 from temp_generate_network_code where province_id = v_temp_point_entity.province_id and entity_type = 'Trench') 
then 
INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Duct', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Duct','','',v_geom_str,'OSP'::character varying,0,'') as f;

PERFORM fn_backbone_update_network_code('att_details_duct', 'Duct', backbone_plan_id::text, 'backbone planning','duct_name');
end if;

end if;
end if;

END LOOP;


INSERT INTO routing_data_core_plan (id, system_id, source, target, cost, reverse_cost, geom)
SELECT
lm.system_id ,lm.system_id,null as source,null as target,
ST_Length(lm.sp_geometry::geography) / 1000 AS cost,
ST_Length(lm.sp_geometry::geography) / 1000 AS reverse_cost,
ST_Force2D(lm.sp_geometry) AS geom
FROM line_master lm
WHERE lm.entity_type = 'Cable'
AND lm.source_ref_id = backbone_plan_id::varchar and lm.source_ref_type ='backbone planning';

SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
IF v_result <> 'OK' THEN
RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for INSERT' AS message;
END IF;
SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
IF v_result <> 'OK' THEN
RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for INSERT' AS message;
END IF;

PERFORM(fn_backbone_get_loop_entity(backbone_plan_id));

PERFORM fn_set_cable_color_info(c.system_id, p_user_id, c.no_of_tube, c.no_of_core_per_tube)
FROM att_details_cable c
WHERE c.source_ref_id = backbone_plan_id::varchar
AND c.source_ref_type = 'backbone planning';

EXECUTE 'ALTER TABLE line_master ENABLE TRIGGER ALL';
EXECUTE 'ALTER TABLE point_master ENABLE TRIGGER ALL';

V_MESSAGE:='BackBone network plan processed successfully!';
update backbone_plan_details t set status = 'Completed' where t.plan_id = backbone_plan_id and created_by = p_user_id;

RETURN QUERY SELECT V_IS_VALID::boolean AS STATUS, V_MESSAGE::CHARACTER VARYING AS MESSAGE,backbone_plan_id :: integer as v_plan_id ;

end;
$function$
;

-------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_loop_entity(p_plan_id integer)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
v_current_point_geom geometry;
v_temp_point_entity record;
v_loop_network_id record;
v_line_geom geometry;
v_arow_loop record;
BEGIN

	INSERT INTO public.att_details_loop(
	network_id, loop_length, associated_system_id, associated_network_id, 
	associated_entity_type, cable_system_id, created_by, created_on, 
	network_status, start_reading, end_reading, 
	parent_system_id, sequence_id, province_id, region_id, parent_network_id, 
	parent_entity_type, status, 
	is_visible_on_map, source_ref_type, source_ref_id, 
	source_ref_description, latitude, longitude, is_new_entity, backbone_system_id,sprout_site_id)
	select '',loop_length,entity_system_id,entity_network_id,
	entity_type,0,created_by,now(),
	'P',0,0,0, 0,province_id, region_id,'','',
	'A',true,'backbone planning', plan_id::varchar,'',latitude,longitude,true,tp.system_id,tp.site_network_id 
	from backbone_plan_network_details tp where entity_type in ('Pole','Manhole') 
	and is_loop_required = true and loop_length >0 and sp_geometry is not null and plan_id = p_plan_id 
	order by system_id;

	INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,
	creator_remark,approver_remark,created_by,common_name, network_status,display_name)
	select lp.system_id , 'Loop',tp.longitude ,tp.latitude ,'A',tp.sp_geometry ,'','',tp.created_by ,'','P',''
	from att_details_loop lp
	join backbone_plan_network_details tp on tp.system_id = lp.backbone_system_id
	where lp.system_id not in (select system_id 
	from point_master where entity_type='Loop');

if exists(select 1 from backbone_plan_network_details where is_loop_required = true and plan_id = p_plan_id) then
PERFORM fn_backbone_update_network_code('att_details_loop', 'Loop',p_plan_id::text, 'backbone planning', 'loop_name' );
end if;

UPDATE att_details_loop lp
SET associated_system_id = pole.system_id,
    associated_network_id = pole.network_id
FROM att_details_pole pole
WHERE pole.backbone_system_id = lp.backbone_system_id
  AND pole.source_ref_id = lp.source_ref_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar;

 UPDATE att_details_loop lp
SET associated_system_id = mh.system_id,
    associated_network_id = mh.network_id
FROM att_details_manhole mh
WHERE mh.backbone_system_id = lp.backbone_system_id
  AND mh.source_ref_id = lp.source_ref_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar;

UPDATE att_details_loop lp
SET cable_system_id = cbl.system_id
FROM line_master cbl
WHERE cbl.entity_type = 'Cable'
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar
  AND cbl.source_ref_id = p_plan_id::varchar
  AND cbl.source_ref_type = 'backbone planning'
  AND ST_DWithin(
        cbl.sp_geometry::geography,
        ST_SetSRID(ST_MakePoint(lp.longitude, lp.latitude), 4326)::geography,
        0.2
      );
 UPDATE att_details_cable lp
SET total_loop_length = sub.total_loop_length,
    total_loop_count  = sub.total_loop_count
FROM (
    SELECT cable_system_id,
           SUM(loop_length) AS total_loop_length,
           COUNT(*) AS total_loop_count
    FROM att_details_loop
    WHERE source_ref_id = p_plan_id::varchar
      AND source_ref_type = 'backbone planning'
    GROUP BY cable_system_id
) sub
WHERE lp.system_id = sub.cable_system_id
  AND lp.source_ref_id = p_plan_id::varchar
  AND lp.source_ref_type = 'backbone planning';


END
$function$
;
