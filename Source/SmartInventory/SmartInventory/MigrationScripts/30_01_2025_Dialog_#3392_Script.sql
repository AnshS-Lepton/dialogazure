/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: This function sets the display_name of different entities
ModifiedOn: 30 Jan 2025
ModifiedBy: Chandra Shekhar Sahni
Purpose: Added exception handing to log relavent error messages in the error_log table
------------------------------------------*/
-- DROP FUNCTION public.fn_splicing_sync_connection_label(int);

CREATE OR REPLACE FUNCTION public.fn_splicing_sync_connection_label(
    p_layer_id INTEGER)
    RETURNS VOID
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_display_column_name CHARACTER VARYING;
    v_layer_name CHARACTER VARYING;
    v_layer_table CHARACTER VARYING;
    v_geom_type CHARACTER VARYING;
    v_display_default_column_name CHARACTER VARYING;
    v_query TEXT;
    v_error_message TEXT;
    v_error_context TEXT;
BEGIN
    UPDATE display_name_settings SET status = 'InProgress' WHERE display_name_settings.layer_id = p_layer_id; 
    PERFORM pg_sleep(50);

    SELECT layer_name, layer_table, geom_type INTO v_layer_name, v_layer_table, v_geom_type FROM layer_details WHERE layer_id = p_layer_id;
    SELECT display_column_name, default_display_column_name INTO v_display_column_name, v_display_default_column_name FROM display_name_settings WHERE layer_id = p_layer_id;

    -- Query 1
    v_query := 'UPDATE connection_info SET source_display_name = (CASE WHEN COALESCE(ent.' || v_display_column_name || ', '''') = '''' THEN ent.' || v_display_default_column_name || ' ELSE ent.' || v_display_column_name || ' END) FROM ' || v_layer_table || ' ent WHERE connection_info.source_system_id = ent.system_id AND upper(source_entity_type) = upper(''' || v_layer_name || ''')';
    EXECUTE v_query;

    -- Query 2
    v_query := 'UPDATE connection_info SET destination_display_name = (CASE WHEN COALESCE(ent.' || v_display_column_name || ', '''') = '''' THEN ent.' || v_display_default_column_name || ' ELSE ent.' || v_display_column_name || ' END) FROM ' || v_layer_table || ' ent WHERE connection_info.destination_system_id = ent.system_id AND upper(destination_entity_type) = upper(''' || v_layer_name || ''')';
    EXECUTE v_query;

    -- Query 3
    v_query := 'UPDATE point_master SET display_name = (CASE WHEN COALESCE(ent.' || v_display_column_name || ', '''') = '''' THEN ent.' || v_display_default_column_name || ' ELSE ent.' || v_display_column_name || ' END) FROM ' || v_layer_table || ' ent WHERE point_master.system_id = ent.system_id AND upper(point_master.entity_type) = upper(''' || v_layer_name || ''')';
    EXECUTE v_query;

    -- Query 4
    v_query := 'UPDATE line_master SET display_name = (CASE WHEN COALESCE(ent.' || v_display_column_name || ', '''') = '''' THEN ent.' || v_display_default_column_name || ' ELSE ent.' || v_display_column_name || ' END) FROM ' || v_layer_table || ' ent WHERE line_master.system_id = ent.system_id AND upper(line_master.entity_type) = upper(''' || v_layer_name || ''')';
    EXECUTE v_query;

    -- Query 5
    v_query := 'UPDATE isp_line_master SET display_name = (CASE WHEN COALESCE(ent.' || v_display_column_name || ', '''') = '''' THEN ent.' || v_display_default_column_name || ' ELSE ent.' || v_display_column_name || ' END) FROM ' || v_layer_table || ' ent WHERE isp_line_master.entity_id = ent.system_id AND upper(isp_line_master.entity_type) = upper(''' || v_layer_name || ''')';
    EXECUTE v_query;

    -- Query 6
    v_query := 'UPDATE polygon_master SET display_name = (CASE WHEN COALESCE(ent.' || v_display_column_name || ', '''') = '''' THEN ent.' || v_display_default_column_name || ' ELSE ent.' || v_display_column_name || ' END) FROM ' || v_layer_table || ' ent WHERE polygon_master.system_id = ent.system_id AND upper(polygon_master.entity_type) = upper(''' || v_layer_name || ''')';
    EXECUTE v_query;

    -- Query 7
    v_query := 'UPDATE associate_entity_info SET entity_display_name = (CASE WHEN COALESCE(ent.' || v_display_column_name || ', '''') = '''' THEN ent.' || v_display_default_column_name || ' ELSE ent.' || v_display_column_name || ' END) FROM ' || v_layer_table || ' ent WHERE associate_entity_info.entity_system_id = ent.system_id AND upper(associate_entity_info.entity_type) = upper(''' || v_layer_name || ''')';
    EXECUTE v_query;

    -- Query 8
    v_query := 'UPDATE associate_entity_info SET associated_display_name = (CASE WHEN COALESCE(ent.' || v_display_column_name || ', '''') = '''' THEN ent.' || v_display_default_column_name || ' ELSE ent.' || v_display_column_name || ' END) FROM ' || v_layer_table || ' ent WHERE associate_entity_info.associated_system_id = ent.system_id AND upper(associate_entity_info.associated_entity_type) = upper(''' || v_layer_name || ''')';
    EXECUTE v_query;

    UPDATE display_name_settings SET status = 'Completed' WHERE display_name_settings.layer_id = p_layer_id;

EXCEPTION  
    WHEN OTHERS THEN 
        -- Get error details
        GET STACKED DIAGNOSTICS v_error_message = MESSAGE_TEXT, v_error_context = PG_EXCEPTION_CONTEXT;

        -- Logging the detailed exception
        INSERT INTO error_log (err_message, err_label, err_description, created_on)
        VALUES (v_error_message, 'fn_splicing_sync_connection_label', 'Error in query: ' || v_query || ' Context: ' || v_error_context, NOW());

        -- Optionally raise an info message
        RAISE INFO 'An error occurred while updating display name settings: %', v_error_message;

        -- Updating status to 'Failed'
        UPDATE display_name_settings SET status = 'Failed' WHERE display_name_settings.layer_id = p_layer_id;

        -- Re-raise the exception if needed
        RAISE;
END;
$BODY$;

ALTER FUNCTION public.fn_splicing_sync_connection_label(integer)
    OWNER TO postgres;
	
	
-----------------------------------------------------------------
--Onetime update to point_master table
UPDATE point_master 
SET display_name = 
    CASE 
        WHEN COALESCE(ent.site_id, '') = '' OR COALESCE(ent.site_name, '') = '' THEN ent.network_id
        ELSE site_id || '-' || site_name 
    END 
FROM att_details_pod ent 
WHERE point_master.system_id = ent.system_id 
  AND UPPER(point_master.entity_type) = UPPER('POD');
  
---------------------------------------------------------------
---------------This Change is given by Deepak Sir--------------
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
if((select ST_Equals(v_old_start_geom,v_new_start_geom))=false) 
AND EXISTS(SELECT 1 FROM CONNECTION_INFO WHERE ((SOURCE_SYSTEM_ID=p_system_id AND UPPER(SOURCE_ENTITY_TYPE)='CABLE') OR DESTINATION_SYSTEM_ID=p_system_id AND UPPER(DESTINATION_ENTITY_TYPE)='CABLE') and is_cable_a_end=true)
and (Cast(st_distance(v_old_start_geom,v_new_start_geom,false)as decimal(10,2))>2)
then
RETURN QUERY SELECT FALSE AS STATUS, (V_LAYER_TITLE||'[SI_GBL_GBL_GBL_GBL_019]')::CHARACTER VARYING AS MESSAGE;--start point can not be changed, as it is spliced with some entity!
elsif((select ST_Equals(v_old_end_geom,v_new_end_geom)=false)) AND EXISTS(SELECT 1 FROM CONNECTION_INFO WHERE ((SOURCE_SYSTEM_ID=p_system_id AND UPPER(SOURCE_ENTITY_TYPE)='CABLE') OR DESTINATION_SYSTEM_ID=p_system_id AND UPPER(DESTINATION_ENTITY_TYPE)='CABLE') and is_cable_a_end=false)
and (Cast(st_distance(v_old_end_geom,v_new_end_geom,false)as decimal(10,2))>2)
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