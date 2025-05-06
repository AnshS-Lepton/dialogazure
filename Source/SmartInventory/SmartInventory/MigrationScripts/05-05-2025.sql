
---------------------------------- Add this column to filter data as per segment created date------------------------------------------

ALTER TABLE top_segment
ADD COLUMN created_on TIMESTAMP DEFAULT now();


----------------------------------Insert data into res_resources ------------------------------------------------------------------

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('de-DE', 'SI_GBL_GBL_GBL_GBL_437', 'Location cannot be changed, as it is associated with topology process!', false, true, 'German', 'Smart Inventory_Global_Global_Global_', NULL, NULL, '2025-05-02 10:57:30.159', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_GBL_GBL_GBL_GBL_437', 'Location cannot be changed, as it is associated with topology process!', true, true, 'English', 'Smart Inventory_Global_Global_Global_', NULL, NULL, '2025-05-02 10:57:30.159', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en-US', 'SI_GBL_GBL_GBL_GBL_437', 'Location cannot be changed, as it is associated with topology process!', false, true, NULL, 'Smart Inventory_Global_Global_Global_', NULL, NULL, '2025-05-02 10:57:30.159', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('fr', 'SI_GBL_GBL_GBL_GBL_437', 'Location cannot be changed, as it is associated with topology process!', false, true, 'French', 'Smart Inventory_Global_Global_Global_', NULL, NULL, '2025-05-02 10:57:30.159', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_GBL_GBL_GBL_GBL_437', 'Location cannot be changed, as it is associated with topology process!', false, true, 'Hindi', 'Smart Inventory_Global_Global_Global_', NULL, NULL, '2025-05-02 10:57:30.159', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ja-JP', 'SI_GBL_GBL_GBL_GBL_437', 'Location cannot be changed, as it is associated with topology process!', false, true, 'Japanese', 'Smart Inventory_Global_Global_Global_', NULL, NULL, '2025-05-02 10:57:30.159', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ru-RU', 'SI_GBL_GBL_GBL_GBL_437', 'Location cannot be changed, as it is associated with topology process!', false, true, 'Russian', 'Smart Inventory_Global_Global_Global_', NULL, NULL, '2025-05-02 10:57:30.159', 5, false, false);



----------------------------------- To avoide change location while enitity associated in tolology-----------------------------------
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

---------------End Topology entity---------------------------------------

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



---------------------------------------------Create function for segment report--------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_segment_report_data(
	p_searchby character varying,
	p_searchbytext character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_duration_based_column character varying,
	p_userid integer)
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
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   
BEGIN

LowerStart:='';
LowerEnd:='';


-- DYNAMIC QUERY
	sql := '
SELECT ROW_NUMBER() OVER (ORDER BY ' || 
       COALESCE(NULLIF(p_sortcolname, ''), 'system_id') || ' ' ||
       COALESCE(NULLIF(p_sorttype, ''), 'desc') || ')::Integer AS S_No,
       system_id,
       region_name,
       segment_code,
       description,
       sequence,
       agg1,
       agg2,
       route_name,
       cable_name,
       status
FROM (
    SELECT ts.route_id AS system_id,
           tr2.region_name,
           ts.segment_code,
           ts.description,
           ts."sequence",
           adp.site_id || '' ('' || adp.network_id || '')'' AS agg1,
           adp2.site_id || '' ('' || adp2.network_id || '')'' AS agg2,
           tr.route_name,
           vadc.cable_name,
           CASE 
               WHEN ts.agg1_site_id IS NULL THEN ''Unassigned''
               ELSE ''Assigned''
           END AS status
    FROM top_segment ts
    INNER JOIN top_region tr2 ON tr2.id = ts.region_id 
    LEFT JOIN vw_att_details_pod adp ON ts.agg1_site_id = adp.system_id 
    LEFT JOIN vw_att_details_pod adp2 ON adp2.system_id = ts.agg2_site_id
    LEFT JOIN top_segment_cable_mapping tscm ON tscm.segment_id = ts.id 
    LEFT JOIN vw_att_details_cable vadc ON vadc.system_id = tscm.cable_id 
    LEFT JOIN top_routes tr ON tr.route_id = ts.route_id where 1=1 ';

		RAISE INFO '------------------------------------sql1%', sql;

IF ((P_searchbytext) != '' and (P_searchby) != '') THEN
sql:= sql ||' AND upper(Cast(ts.'||P_searchby||' as TEXT)) LIKE upper(''%'||trim(P_searchbytext)||'%'')';

	--if(substring(P_searchbytext from 1 for 1)='"' and  substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
--	then
--		sql:= sql ||' AND upper(Cast(ts.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
--	else
		--sql:= sql ||' AND upper(Cast(ts.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
--	end if;

END IF;

IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND ts.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and ts.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

sql:= sql ||' )X';
RAISE INFO '%', sql;
-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords,* FROM (' || sql || ') T';                  
 END IF; 

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$;

------------------------------------------------------- Modify delete function---------------------------------------

DROP FUNCTION IF EXISTS public.fn_topology_sites_dissociation(integer, integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_sites_dissociation(
	p_base_site_id integer,
	p_site_id integer,
	p_ring_id integer,
	p_distance integer,
	p_user_id integer)
    RETURNS TABLE(siteid integer, sitename character varying, sitedistance numeric, ringid integer, is_agg_site boolean) 
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
   
   delete from top_ring_site_mapping trsm where site_id=p_site_id and ring_id=p_ring_id;

    -- Execute the SQL and return the result
    RETURN QUERY
	
	SELECT 
        fn.siteid, fn.sitename, fn.sitedistance, fn.ringid, fn.is_agg_site  
    FROM fn_topology_get_sites(p_base_site_id, p_ring_id, p_distance, p_user_id) AS fn; 
	
END;
$BODY$;

ALTER FUNCTION public.fn_topology_sites_dissociation(integer, integer, integer, integer, integer)
    OWNER TO postgres;
-------------------------------------------Modify Multi route function----------------------------------------

 DROP FUNCTION IF EXISTS public.fn_topology_get_cableroute(integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_cableroute(
	p_agg1site_id integer,
	p_agg2site_id integer,
	p_user_id integer)
    RETURNS TABLE(route_id integer, route_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    v_source_geom geometry;
    v_dest_geom geometry;
    MAX_DEPTH integer := 20;
BEGIN

/*create temp table temp_routes
(
rout_id serial,
route_name character varying,
cable_names character varying,
total_length double precision,
route_geom geometry
) on commit drop;*/

    -- Get source site geometry (start point)
    SELECT ST_SetSRID(ST_MakePoint(longitude, latitude), 4326)
    INTO v_source_geom
    FROM att_details_pod
    WHERE att_details_pod.system_id = p_agg1site_id::int;

    -- Get destination site geometry (end point)
    SELECT ST_SetSRID(ST_MakePoint(longitude, latitude), 4326)
    INTO v_dest_geom
    FROM att_details_pod
    WHERE att_details_pod.system_id = p_agg2site_id::int;

WITH RECURSIVE
-- 1. Input points
 input AS (
  SELECT 
    v_source_geom::geometry AS source,
    v_dest_geom::geometry AS dest
),

-- 2. Find all cables starting near the source point
start_cables AS (
  SELECT 
    c.system_id,
    c.common_name,
    c.sp_geometry,
    CASE 
      WHEN  st_distance(ST_StartPoint(c.sp_geometry), i.source,false) < st_distance(ST_endPoint(c.sp_geometry), i.source,false)
      THEN ST_endPoint(c.sp_geometry)
      ELSE ST_StartPoint(c.sp_geometry)
    END AS current_point,
    i.dest
  FROM line_master c
  CROSS JOIN input i
  WHERE ST_DWithin(c.sp_geometry, i.source, 0.00001)
),

-- 3. Recursive traversal
path AS (
  SELECT 
    ARRAY[c.system_id] AS cable_ids,
    ARRAY[c.common_name]::varchar[] AS cable_names,
    c.sp_geometry,
    c.current_point,
    c.system_id,
    c.dest,
    1 AS depth,
    ST_Length(c.sp_geometry::geography) AS total_length,
    c.sp_geometry AS full_path_geom
  FROM start_cables c

  UNION ALL

  SELECT 
    p.cable_ids || c.system_id,
    p.cable_names || c.common_name,
    c.sp_geometry,
    CASE 
      WHEN ST_Within(ST_StartPoint(c.sp_geometry),st_buffer_meters(p.current_point,3))
      THEN ST_EndPoint(c.sp_geometry)
      ELSE ST_StartPoint(c.sp_geometry)
    END,
    c.system_id,
    p.dest,
    p.depth + 1,
    p.total_length + ST_Length(c.sp_geometry::geography),
    ST_Union(p.full_path_geom, c.sp_geometry)
  FROM path p
  JOIN line_master c 
    ON (
      (
        ST_Within(ST_StartPoint(c.sp_geometry),st_buffer_meters(p.current_point,3))
        OR ST_Within(ST_EndPoint(c.sp_geometry),st_buffer_meters(p.current_point,3))
      )
      AND NOT c.system_id = ANY(p.cable_ids)
    )
),
--select * from path
-- select b.* from line_master a
-- inner join line_master b on ST_Within( ST_StartPoint(b.sp_geometry),st_buffer_meters(st_endpoint(a.sp_geometry),3))
-- where a.common_name='AMP-CBL000765'

-- 4. Valid paths that reach near destination
valid_routes AS (
  SELECT 
    ROW_NUMBER() OVER (ORDER BY total_length) AS route_rank,
    cable_names,
    cable_ids,
    total_length,
    ST_LineMerge(ST_Union(full_path_geom)) AS route_geom
  FROM path
  WHERE ST_Within(dest,st_buffer_meters(current_point,3))
  GROUP BY cable_names, cable_ids,total_length
)

-- 5. Final output with route name
insert into top_routes(route_name,cable_names,total_length,route_geom,agg1site_id,agg2site_id)
SELECT 
  'Route_' || route_rank AS route_name,
  cable_names,
  total_length,
  CASE 
    WHEN GeometryType(route_geom) = 'LINESTRING' THEN route_geom
    WHEN GeometryType(route_geom) = 'MULTILINESTRING' THEN
      ST_MakeLine(
        (SELECT array_agg(pt.geom ORDER BY pt.path)
         FROM ST_DumpPoints(route_geom) AS pt)
      )
    ELSE NULL
  END AS  route_geom,
 p_agg1site_id,
 p_agg2site_id
FROM valid_routes
ORDER BY route_rank;

    RETURN QUERY
    select Max(tr.route_id) AS route_id,tr.route_name from top_routes tr where agg1site_id=p_agg1site_id and agg2site_id=p_agg2site_id
    GROUP BY tr.route_name;
END;
$BODY$;

ALTER FUNCTION public.fn_topology_get_cableroute(integer, integer, integer)
    OWNER TO postgres;

-------------------------------------------------------------------- Get site details in modification----------------------------------------------

-- FUNCTION: public.fn_topology_get_sites_test(integer, integer, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_topology_get_sites_test(integer, integer, integer, integer);

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
     SELECT p.system_id,p.common_name, p.longitude || ' ' || p.latitude 
     FROM  point_master p
     WHERE p.system_id in (SELECT agg1_site_id AS combined_column FROM top_segment ts where ts.id=p_segment_id
UNION ALL
SELECT agg2_site_id FROM top_segment ts2 where ts2.id=p_segment_id );

 -- Insert all mobile sites which is associated to that segment
insert into temp_aggsite(system_id,network_id,geom)
     SELECT p.system_id,p.common_name, p.longitude || ' ' || p.latitude 
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

ALTER FUNCTION public.fn_topology_get_sites_test(integer, integer, integer, integer)
    OWNER TO postgres;


-----------------------------------------------------------Remove site association-----------------------------------------



-- DROP FUNCTION IF EXISTS public.fn_topology_sites_dissociation(integer, integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_sites_dissociation_test(
	p_base_site_id integer,
	p_site_id integer,
	p_ring_id integer,
	p_distance integer,
	p_user_id integer)
    RETURNS TABLE(siteid integer, sitename character varying, sitedistance numeric, ringid integer, is_agg_site boolean) 
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
	 p_segment_id integer;

BEGIN
   
-- site ring dissociation

update att_details_pod set ring_id=null where system_id=p_site_id and ring_id=p_ring_id;
    -- Get the reference site's geometry
   
   select segment_id into p_segment_id from top_ring where id=p_ring_id;
   IF p_segment_id > 0 THEN
   delete from top_ring_site_mapping trsm where site_id=p_site_id and ring_id=p_ring_id;
end if;
    -- Execute the SQL and return the result
    RETURN QUERY
	
	SELECT 
        fn.siteid, fn.sitename, fn.sitedistance, fn.ringid, fn.is_agg_site  
    FROM fn_topology_get_sites(p_base_site_id, p_ring_id,p_segment_id, p_distance, p_user_id) AS fn; 
	
END;
$BODY$;

ALTER FUNCTION public.fn_topology_sites_dissociation_test(integer, integer, integer, integer, integer)
    OWNER TO postgres;

--------------------------------------Modification in functions----------------------------------------


-- FUNCTION: public.fn_topology_get_segmentdetailssitewise(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_topology_get_segmentdetailssitewise(integer, integer);

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
		   
	  SELECT seq, path_seq, edge,p_user_id,p_system_id,ROUND(ST_DistanceSphere(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), ST_GeomFromText('POINT(' || p_site2_geom || ')', 4326))::numeric, 2) AS sitedistance FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	  FROM routing_data_core_plan', (SELECT pgr.id
	  FROM routing_data_core_plan_vertices_pgr pgr
	  WHERE ST_Within( pgr.the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_site1_geom || ')', 4326), 2))
	  limit 1 ),(SELECT pgr.id FROM routing_data_core_plan_vertices_pgr pgr
	  WHERE ST_Within( pgr.the_geom, 
	  ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_site2_geom ||')', 4326), 2)) 
	  limit 1));

   END LOOP;
   
    DELETE FROM temp_cable_routes WHERE edge_targetid = -1 AND user_id = p_user_id;

----------------------------------------------Modify the Function ---------------------------------------
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
                  SELECT  distinct
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
-----------------------------------------------------------------------


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


----------------------------------- Validation function to update entity details----------------------


DROP FUNCTION IF EXISTS public.fn_validate_point_geometry(integer, character varying, character varying, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_validate_point_geometry(
	p_system_id integer,
	p_entity_type character varying,
	p_latitude character varying,
	p_longitude character varying,
	p_region_id integer,
	p_province_id integer)
    RETURNS TABLE(status character varying, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
BEGIN


-- Start topology Plan
if (UPPER(p_entity_type) = 'POD') and (SELECT SUM(count_val) AS total_count
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

status := 'FAILD';
message := 'The selected entity is used in the topology plan.';

  return query select status,message;
END IF;
 -- End topology Plan

    -- Check if the point is within a 100-meter buffer and matches the entity type and system ID
    IF EXISTS (
        SELECT 1 
        FROM point_master PM
        WHERE ST_Within(
                  PM.sp_geometry,
                  ST_Buffer(
                      ST_GeomFromText('POINT(' || p_longitude || ' ' || p_latitude || ')', 4326)::geography,
                      100
                  )::geometry
              ) 
          AND entity_type = p_entity_type 
          AND system_id = p_system_id
    ) THEN
        
        -- Further validate the region and province boundaries
        IF p_region_id = (
                SELECT RM.id 
                FROM REGION_BOUNDARY RM
                WHERE ST_Intersects(
                          RM.sp_geometry, 
                          ST_GeomFromText('POINT(' || p_longitude || ' ' || p_latitude || ')', 4326)::geography
                      )
                LIMIT 1
            ) 
          AND p_province_id = (
                SELECT PM.id 
                FROM PROVINCE_BOUNDARY PM
                WHERE ST_Intersects(
                          PM.sp_geometry, 
                          ST_GeomFromText('POINT(' || p_longitude || ' ' || p_latitude || ')', 4326)::geography
                      )
                LIMIT 1
            ) THEN
            status := 'OK';
            message := '';
        ELSE 
            status := 'FAILED';
            message := 'The geometry is outside the 100-meter buffer.';
        END IF;
        
    ELSE 
        status := 'FAILED';
        message := 'The geometry is outside the 100-meter buffer.';
    END IF;

    RETURN QUERY SELECT status, message;
END;
$BODY$;

-----------------


