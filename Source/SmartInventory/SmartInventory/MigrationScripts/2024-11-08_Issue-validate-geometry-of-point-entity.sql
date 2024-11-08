CREATE OR REPLACE FUNCTION public.fn_validate_point_geometry(p_system_id integer, p_entity_type character varying, p_latitude character varying, p_longitude character varying, p_region_id integer, p_province_id integer)
 RETURNS TABLE(status character varying, message character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE
BEGIN
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
$function$
;
