

-------------------------------------Create function to get Pole/Manhole details----------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_getnearestfeasibility(
	lat double precision,
	lng double precision,
	mtrbuffer integer)
    RETURNS TABLE(geom_type text, entity_type character varying, entity_title character varying, system_id integer, common_name character varying, geom text, centroid_geom text, network_status character varying, display_name character varying, total_core text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 2

AS $BODY$
DECLARE 
    v_role_id integer;
    v_geometry_with_buffer geometry;
    v_geometry geometry;
	
BEGIN
    -- Create Point and Buffer Geometry
    v_geometry = ST_GEOMFROMTEXT('POINT('||LNG||' '||LAT||')',4326);
    v_geometry_with_buffer = ST_BUFFER_METERS(v_geometry, MTRBUFFER);
	
	

Raise info 'MTRBUFFER ->%',MTRBUFFER;
Raise info 'v_geometry_with_buffer ->%',v_geometry_with_buffer;

    -- Get Role ID
    --SELECT ROLE_ID INTO v_role_id FROM USER_MASTER WHERE USER_ID = P_USER_ID;

    -- Return Pole (Point) Data
    RETURN QUERY 
    SELECT 
        'Point' AS geom_type, 
        PTM.ENTITY_TYPE, 
		ld.layer_title AS entity_title, 
        PTM.SYSTEM_ID, 
        PTM.COMMON_NAME, 
        ST_astext(PTM.SP_GEOMETRY) AS geom, 
        ST_astext(PTM.SP_GEOMETRY) AS centroid_geom, 
        PTM.NETWORK_STATUS, 
        PTM.DISPLAY_NAME, 
        COALESCE(PTM.NO_OF_PORTS::text, 'N/A') AS total_core
    FROM POINT_MASTER PTM
	inner join layer_details ld on ld.layer_name=PTM.entity_type
    WHERE 
	UPPER(PTM.ENTITY_TYPE) IN ('MANHOLE', 'POLE') AND
     ST_WITHIN(PTM.SP_GEOMETRY, v_geometry_with_buffer)
	 ORDER BY ST_Distance(PTM.SP_GEOMETRY, ST_GeomFromText('POINT('||LNG||' '||LAT||')', 4326)) asc
    LIMIT 1;

    -- Return Cable (Line) Data
    RETURN QUERY 
    SELECT 
        'Line' AS geom_type, 
        L.ENTITY_TYPE,
		ld.layer_title AS entity_title, 
        L.SYSTEM_ID, 
        L.COMMON_NAME, 
        ST_AsText(ST_ClosestPoint(L.SP_GEOMETRY, ST_SetSRID(ST_MakePoint(lng, lat), 4326))) AS geom, 
        ST_astext(ST_Centroid(L.SP_GEOMETRY)) AS centroid_geom, 
        L.NETWORK_STATUS, 
        L.DISPLAY_NAME, 
        COALESCE(CBL.TOTAL_CORE::text, '0') || 'F' AS total_core
    FROM LINE_MASTER L
	inner join layer_details ld on ld.layer_name=L.entity_type
    LEFT JOIN att_details_cable CBL 
        ON L.SYSTEM_ID = CBL.SYSTEM_ID
    WHERE 
	UPPER(L.ENTITY_TYPE) = 'CABLE' AND 
    ST_INTERSECTS(ST_MAKEVALID(L.SP_GEOMETRY), v_geometry_with_buffer)
	ORDER BY ST_Distance(ST_ClosestPoint(L.SP_GEOMETRY,ST_SetSRID(ST_MakePoint(lng, lat), 4326)), ST_SetSRID(ST_MakePoint(lng, lat), 4326)) asc
    LIMIT 1;
END
$BODY$;

----------------------------------Create function to get the cable details near structure---------------------------------------
CREATE OR REPLACE FUNCTION public.fn_getroutebufferfeasibility(
	p_coordinates character varying,
	mtrbuffer integer)
    RETURNS TABLE(cable_network_id character varying, cable_name character varying, geometry text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 2

AS $BODY$
DECLARE 
    v_role_id integer;
    v_geometry_with_buffer geometry;
    v_geometry geometry;
	
BEGIN
    -- Create Point and Buffer Geometry
    v_geometry = ST_GEOMFROMTEXT('LINESTRING('||p_coordinates||')',4326);
    v_geometry_with_buffer = ST_BUFFER_METERS(v_geometry, MTRBUFFER);
	
	

Raise info 'MTRBUFFER ->%',MTRBUFFER;
Raise info 'v_geometry_with_buffer ->%',v_geometry_with_buffer;

   
   
    -- Return Cable (Line) Data
    RETURN QUERY 
    SELECT 
        L.COMMON_NAME AS cable_network_id, 
        L.display_name as cable_name, 
        ST_astext(L.SP_GEOMETRY)::TEXT AS geometry
        
    FROM LINE_MASTER L
    LEFT JOIN att_details_cable CBL 
        ON L.SYSTEM_ID = CBL.SYSTEM_ID
    WHERE 
	UPPER(L.ENTITY_TYPE) = 'CABLE' AND 
    ST_INTERSECTS(ST_MAKEVALID(L.SP_GEOMETRY), v_geometry_with_buffer)
    ;
END
$BODY$;

ALTER FUNCTION public.fn_getroutebufferfeasibility(text, integer)
    OWNER TO postgres;

    ------------------------------------------Get start and end point of cable--------------------------------------------
    CREATE OR REPLACE FUNCTION public.fn_get_start_endpoints(
	p_line_geom TEXT)
    RETURNS TABLE(start_point TEXT, end_point TEXT) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY 
    SELECT ST_astext(ST_StartPoint(p_line_geom)), ST_astext(ST_EndPoint(p_line_geom));
END;
$BODY$;

