INSERT INTO public.res_resources(
	culture, key, value, is_default_lang, is_visible, language, description, created_on, created_by, is_jquery_used, is_mobile_key)
	VALUES ('en', 'SI_OSP_CAB_NET_FRM_076', 'Overlapping cable exists!', true, true, 'English', 'Smart Inventory_Osp_Cable_Dot Net_', now(), 1, false, false);
	
	
CREATE OR REPLACE FUNCTION public.fn_get_checkDuplicateDesignId(	
	p_design_id character varying,p_system_id integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
declare
result int:=0;
begin
		IF((select char_length(coalesce(p_design_id,'')))>5)
		THEN
			IF(p_system_id=0)
			THEN
				IF EXISTS(select 1 from (select gis_design_id from att_details_cable where char_length(coalesce(gis_design_id,''))>5) t1 where  
				substr(t1.gis_design_id, 1, length(t1.gis_design_id) - 3)=substr(p_design_id, 1, length(p_design_id) - 3))
				THEN
					result=1;
				END IF;
			ELSE
				IF EXISTS(select 1 from (select gis_design_id from att_details_cable where char_length(coalesce(gis_design_id,''))>5) t1 where  
				substr(t1.gis_design_id, 1, length(t1.gis_design_id) - 3)=substr(p_design_id, 1, length(p_design_id) - 3))
				THEN
					IF EXISTS(select 1 from (select system_id,gis_design_id from att_details_cable where char_length(coalesce(gis_design_id,''))>5) t1 where  
				substr(t1.gis_design_id, 1, length(t1.gis_design_id) - 3)=substr(p_design_id, 1, length(p_design_id) - 3) and t1.system_id=p_system_id)
					THEN
						result=0;
					ELSE
						result=1;
					END IF;
				END IF;
			END IF;
		END IF;
return result;

end;
$BODY$;



CREATE OR REPLACE FUNCTION public.fn_validate_entity_geom(
	p_geom_type character varying,
	p_entity_type character varying,
	p_longlat character varying,
	p_userid integer,
	p_parent_system_id integer)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
V_QUERY TEXT;
V_STATUS BOOLEAN;
V_MESSAGE CHARACTER VARYING;
V_COUNT INTEGER;
v_geom_type character varying;
V_LAYER_MAPPING record;
v_input_geom character varying;
v_bld_buffer integer;
v_layer_title character varying;
v_geom geometry;
v_startpoint geometry;
v_endpoint geometry;
V_PARENT_SEQUENCE int;
PARENT_OUTER_BOUNDARIES character varying;
PARENT_INNER_BOUNDARIES character varying;
V_INNER_ENTITY_TYPE character varying;
V_OUTER_ENTITY_TYPE character varying;
V_IS_INNER_ENTITITES boolean;
V_IS_OUTER_ENTITITES boolean;
BEGIN 
V_STATUS:=TRUE;
V_MESSAGE:='Validation Success!';
V_COUNT:=0;
V_GEOM_TYPE:='';
select layer_title into v_layer_title from layer_details where upper(layer_name)=upper(p_entity_type);
IF UPPER(P_GEOM_TYPE)='POINT' 
THEN 
	V_GEOM_TYPE:='POINT('||P_LONGLAT||')';
ELSIF UPPER(P_GEOM_TYPE)='LINE' 
THEN 
	V_GEOM_TYPE:='LINESTRING('||P_LONGLAT||')';
ELSE
	V_GEOM_TYPE:='POLYGON(('||P_LONGLAT||'))';
END IF;

SELECT COUNT(1) INTO V_COUNT FROM REGION_BOUNDARY RM, PROVINCE_BOUNDARY PM
WHERE RM.ISVISIBLE=TRUE AND PM.ISVISIBLE=TRUE AND ST_INTERSECTS(PM.SP_GEOMETRY, ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326)) 
AND ST_INTERSECTS(RM.SP_GEOMETRY, ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326));
        

IF(coalesce(V_COUNT,0)=0)
THEN
	V_STATUS:=false;
	V_MESSAGE:='No Region/Province exist at this location!';
--ELSIF(coalesce(V_COUNT,0)>1 AND( upper(p_entity_type)!='CABLE' AND upper(p_entity_type)!='DUCT' AND upper(p_entity_type)!='TRENCH') )
ELSIF(coalesce(V_COUNT,0)>1 AND( upper(P_GEOM_TYPE)!='LINE') )
THEN
	V_STATUS:=false;
	V_MESSAGE:='Multiple province exist at this location!';
ELSIF(coalesce(V_COUNT,0)=1)
THEN
	--VALIDATING THE PROVINCE IS AVAILABLE OR NOT
	IF EXISTS(SELECT 1 FROM REGION_BOUNDARY RM, PROVINCE_BOUNDARY PM
	WHERE RM.ISVISIBLE=TRUE AND PM.ISVISIBLE=TRUE AND ST_INTERSECTS(PM.SP_GEOMETRY, ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326)) 
	AND ST_INTERSECTS(RM.SP_GEOMETRY, ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326)) AND COALESCE(PM.PROVINCE_ABBREVIATION,'')='')
	THEN
		V_STATUS:=false;
		V_MESSAGE:='Province code is not available. Please contact administrator!';
	END IF;
			
	IF V_STATUS AND NOT EXISTS(SELECT 1 FROM VW_LAYER_MAPPING WHERE (UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE) AND UPPER(PARENT_GEOM_TYPE)='POLYGON' and IS_USED_FOR_NETWORK_ID=true) OR (UPPER(PARENT_LAYER_NAME)=UPPER('PROVINCE')))
	THEN
		V_STATUS:=FALSE;
		V_MESSAGE:='Parent does not exist at this location!';
	END IF;

	--VALIDATING THE PARENT IS EXIST OR NOT SELECTED LOCATION
	SELECT * INTO V_LAYER_MAPPING  FROM VW_LAYER_MAPPING WHERE UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE) AND (UPPER(PARENT_GEOM_TYPE)='POLYGON' OR (UPPER(PARENT_LAYER_NAME)=UPPER('PROVINCE')))
	and IS_USED_FOR_NETWORK_ID=true order by parent_sequence desc limit 1;

	IF V_STATUS AND EXISTS(SELECT 1 FROM VW_LAYER_MAPPING WHERE UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE) AND (UPPER(PARENT_GEOM_TYPE)='POLYGON'
	 OR (UPPER(PARENT_LAYER_NAME)=UPPER('PROVINCE'))) and IS_USED_FOR_NETWORK_ID=true)
	THEN
		raise info 'test % ',1;
		IF(UPPER(V_LAYER_MAPPING.PARENT_LAYER_NAME)='PROVINCE')
		THEN			
			IF NOT EXISTS(SELECT 1 FROM PROVINCE_BOUNDARY PM
			WHERE PM.ISVISIBLE=TRUE AND ST_INTERSECTS(PM.SP_GEOMETRY, ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326)))  
			then
				V_STATUS:=FALSE;
				V_MESSAGE:=v_layer_title||' [SI_OSP_GBL_GBL_FRM_142] '||V_LAYER_MAPPING.PARENT_LAYER_NAME||' [SI_OSP_GBL_GBL_FRM_143]';
			END IF;

			IF(V_STATUS AND UPPER(P_ENTITY_TYPE)='CABLE')
			THEN
				-- select ST_buffer_meters(ST_STARTPOINT(ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326)),5)into v_startpoint;
				-- select ST_buffer_meters(ST_ENDPOINT(ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326)),5)into v_endpoint;
				-- 
				-- IF EXISTS(SELECT 1 FROM LINE_MASTER WHERE (st_within(ST_ENDPOINT(sp_geometry),v_startpoint) and st_within(ST_STARTPOINT(sp_geometry),v_endpoint))
				-- OR (st_within(ST_STARTPOINT(sp_geometry),v_startpoint) and st_within(ST_ENDPOINT(sp_geometry),v_endpoint))
				-- AND ENTITY_TYPE='Cable')
				-- THEN
				-- V_STATUS:=FALSE;
				-- V_MESSAGE:='Parallel cable exist within 5 meter buffer';
				-- END IF;
				-- raise info 'v_startpoint % ',v_startpoint;
				-- raise info 'v_endpoint % ',v_endpoint;

				IF EXISTS(SELECT 1 FROM LINE_MASTER WHERE ST_Equals(ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326),sp_geometry)=true
				AND ENTITY_TYPE='Cable')
				THEN
					V_STATUS:=FALSE;
					V_MESSAGE:=' [SI_OSP_CAB_NET_FRM_076] ';
				END IF;
				
			END IF;					
		-- ELSIF(UPPER(V_LAYER_MAPPING.PARENT_GEOM_TYPE)='POLYGON') and NOT EXISTS(SELECT 1 FROM VW_LAYER_MAPPING LM
-- 			INNER JOIN POLYGON_MASTER PM ON UPPER(LM.PARENT_LAYER_NAME)=UPPER(PM.ENTITY_TYPE) 
-- 			AND ST_WITHIN(ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326),SP_GEOMETRY) and IS_USED_FOR_NETWORK_ID=true
-- 			WHERE UPPER(LM.CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE) AND UPPER(LM.PARENT_LAYER_NAME)=UPPER(V_LAYER_MAPPING.PARENT_LAYER_NAME))  
-- 			then
-- 				V_STATUS:=FALSE;
-- 				V_MESSAGE:=v_layer_title||' [SI_OSP_GBL_GBL_FRM_142] '||V_LAYER_MAPPING.PARENT_LAYER_NAME||' [SI_OSP_GBL_GBL_FRM_143]';
		END IF;	

		IF(UPPER(V_LAYER_MAPPING.CHILD_GEOM_TYPE)='POLYGON') THEN	

			SELECT PARENT_SEQUENCE INTO V_PARENT_SEQUENCE FROM VW_LAYER_MAPPING WHERE UPPER(PARENT_LAYER_NAME)=UPPER(P_ENTITY_TYPE) LIMIT 1;
			IF(UPPER(P_ENTITY_TYPE)='AREA') THEN
			V_PARENT_SEQUENCE:=1;
			END IF;
			SELECT STRING_AGG(PARENT_LAYER_NAME, ',')  INTO PARENT_OUTER_BOUNDARIES FROM VW_LAYER_MAPPING WHERE PARENT_SEQUENCE < V_PARENT_SEQUENCE 
			AND UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE);
			SELECT STRING_AGG(PARENT_LAYER_NAME, ',')  INTO PARENT_INNER_BOUNDARIES FROM VW_LAYER_MAPPING WHERE PARENT_SEQUENCE > V_PARENT_SEQUENCE 
			AND UPPER(CHILD_LAYER_NAME)=UPPER(P_ENTITY_TYPE);
	       			
			-------------------ANTRA POLYGON ENTITIES CHANGES-----------------------

			--INTERSECTING WITH INNER BOUNDARY
			SELECT ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326) into v_geom;
			SELECT COUNT(1) > 0 ,F.ENTITY_TYPE INTO V_IS_INNER_ENTITITES,V_INNER_ENTITY_TYPE FROM POLYGON_MASTER F WHERE F.ENTITY_TYPE IN (
			SELECT UNNEST(STRING_TO_ARRAY(PARENT_INNER_BOUNDARIES, ','))) AND ST_INTERSECTS(F.SP_GEOMETRY,V_GEOM) AND ST_TOUCHES(F.SP_GEOMETRY, V_GEOM) = FALSE 
			AND NOT ST_WITHIN(F.SP_GEOMETRY, V_GEOM) GROUP BY ENTITY_TYPE;

			IF (V_IS_INNER_ENTITITES)  THEN
			V_STATUS:=FALSE;
			V_MESSAGE:=V_LAYER_TITLE||' is intersecting with other inner '||(SELECT LAYER_TITLE FROM LAYER_DETAILS WHERE LAYER_NAME=V_INNER_ENTITY_TYPE) ||' boundary!';
			END IF;
			
			--INTERSECTING WITH OUTER BOUNDARY
			SELECT COUNT(1) > 0 ,F.ENTITY_TYPE INTO V_IS_OUTER_ENTITITES,V_OUTER_ENTITY_TYPE FROM POLYGON_MASTER F WHERE F.ENTITY_TYPE IN (
			SELECT UNNEST(STRING_TO_ARRAY(PARENT_OUTER_BOUNDARIES, ','))) AND ST_INTERSECTS(V_GEOM, F.SP_GEOMETRY) 
			AND ST_TOUCHES(V_GEOM, F.SP_GEOMETRY) = FALSE AND NOT ST_WITHIN(V_GEOM, F.SP_GEOMETRY) GROUP BY ENTITY_TYPE;

			IF (V_IS_OUTER_ENTITITES)  THEN
			V_STATUS:=FALSE;
			V_MESSAGE:=V_LAYER_TITLE||' is intersecting with other outer '|| (SELECT LAYER_TITLE FROM LAYER_DETAILS WHERE LAYER_NAME=V_OUTER_ENTITY_TYPE)||' boundary!';
			END IF;

			if ((select value from global_settings where upper(key)=upper('isvalidatecityboundary'))::integer=1 and upper(p_entity_type) in ('AREA','SUBAREA'))
			Then
			--SELECT ST_AsText(ST_Centroid(ST_GeomFromText(('POLYGON (('||p_longlat||'))')))) into V_RESULT;
			if not EXISTS( SELECT substring(left(St_astext(geom),-3),16) FROM rjio_city_boundary 
			where ST_WITHIN(ST_Centroid(ST_GeomFromText(('POLYGON (('||p_longlat||'))'),4326)),geom) limit 1)
			Then
			V_STATUS:=FALSE;
			V_MESSAGE:='Drawn boundary must be within City Boundary!';
			END IF;
			END IF;
			if ((select value from global_settings where upper(key)=upper('ISVALIDATEJPBOUNDARY'))::integer=1 and upper(p_entity_type)=upper('SubArea'))
			Then
			--SELECT ST_AsText(ST_Centroid(ST_GeomFromText(('POLYGON (('||p_longlat||'))')))) into V_RESULT;
			if not EXISTS(SELECT substring(left(St_astext(geom),-3),16) FROM rjio_jp_boundary 
			where ST_WITHIN(ST_GeomFromText('POLYGON (('||p_longlat||'))',4326), geom) limit 1)
			Then
			V_STATUS:=FALSE;
			V_MESSAGE:='Drawn boundary must be within JP Boundary!';
			END IF;
			END IF;
			-------------------------END ----------------------------

	      END IF;
	END IF;

	--IS THE POLYGON TYPE IS INTERSECTING OR NOT
	IF(V_STATUS AND UPPER(p_entity_type)!='RESTRICTEDAREA' AND UPPER(p_entity_type)!='SURVEYAREA' AND UPPER(P_GEOM_TYPE)='POLYGON' AND EXISTS(SELECT 1 FROM(SELECT P.SYSTEM_ID FROM POLYGON_MASTER P WHERE UPPER(P.ENTITY_TYPE) = UPPER(P_ENTITY_TYPE) AND ST_INTERSECTS(P.SP_GEOMETRY, ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326)) = 'T'
	UNION
	SELECT P.SYSTEM_ID FROM CIRCLE_MASTER P WHERE UPPER(P.ENTITY_TYPE) = UPPER(P_ENTITY_TYPE) AND ST_INTERSECTS(P.SP_GEOMETRY, ST_GEOMFROMTEXT(''||V_GEOM_TYPE||'',4326)) = 'T'
	)A) )
	THEN
	raise info ' % ',V_GEOM_TYPE;
		V_STATUS:=FALSE;
		V_MESSAGE:=v_layer_title||' is intersecting with other '||v_layer_title||' boundary!';
	END IF;

	IF(V_STATUS AND UPPER(P_ENTITY_TYPE)='BUILDING')
	THEN
		SELECT VALUE::INTEGER INTO V_BLD_BUFFER FROM GLOBAL_SETTINGS WHERE UPPER(KEY)='BLDBUFFERINMTR';
		SELECT A.STATUS,A.MESSAGE INTO V_STATUS,V_MESSAGE FROM FN_VALIDATE_BUILDING_GEOM(P_GEOM_TYPE,P_LONGLAT,P_USERID,V_BLD_BUFFER,0) A LIMIT 1;
	END IF;

	IF(V_STATUS AND UPPER(P_ENTITY_TYPE)='STRUCTURE')
	THEN
		
		SELECT A.STATUS,A.MESSAGE INTO V_STATUS,V_MESSAGE FROM FN_GET_STRUCTURE_VALIDATE(P_PARENT_SYSTEM_ID,P_LONGLAT,0,'Point') A LIMIT 1;
	END IF;
	
	
END IF;   
return  query 
select  V_STATUS,V_MESSAGE;
END
$BODY$;

ALTER FUNCTION public.fn_validate_entity_geom(character varying, character varying, character varying, integer, integer)
    OWNER TO postgres;