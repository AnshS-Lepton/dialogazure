
CREATE OR REPLACE FUNCTION public.fn_getregionprovince(
	geometry character varying,
	entitytype character varying)
    RETURNS TABLE(region_id integer, region_name character varying, region_abbreviation character varying, province_id integer, province_name character varying, province_abbreviation character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

 DECLARE
    sql TEXT;
    entityid TEXT;
BEGIN

RAISE INFO '%', sql;

IF upper(entitytype)='POINT' then 
	----For Check Point Type Entity
         sql:= 'SELECT RM.ID as REGION_ID,RM.REGION_NAME,RM.REGION_ABBREVIATION,PM.ID as PROVINCE_ID,  PM.PROVINCE_NAME, PM.PROVINCE_ABBREVIATION 
                FROM REGION_BOUNDARY RM, PROVINCE_BOUNDARY PM
        	WHERE RM.ISVISIBLE=TRUE AND PM.ISVISIBLE=TRUE AND ST_INTERSECTS(PM.SP_GEOMETRY, ST_GEOMFROMTEXT(''POINT('||geometry||')'',4326)) 
        	and   ST_INTERSECTS(RM.SP_GEOMETRY, ST_GEOMFROMTEXT(''POINT('||geometry||')'',4326)) ';

ELSIF upper(entitytype)='LINE' then 
	----For Check Line Type Entity
         sql:= 'SELECT RM.ID as REGION_ID,RM.REGION_NAME,RM.REGION_ABBREVIATION,PM.ID as PROVINCE_ID,  PM.PROVINCE_NAME, PM.PROVINCE_ABBREVIATION 
                FROM REGION_BOUNDARY RM join PROVINCE_BOUNDARY PM on RM.id =Pm.region_id
        	WHERE RM.ISVISIBLE=TRUE AND PM.ISVISIBLE=TRUE 
			AND ST_INTERSECTS(PM.SP_GEOMETRY, ST_GEOMFROMTEXT(''LINESTRING('||geometry||')'',4326))';
else
	----For Check Polygon Type Entity
   sql:= 'SELECT RM.ID as REGION_ID,RM.REGION_NAME,RM.REGION_ABBREVIATION,PM.ID as PROVINCE_ID,  PM.PROVINCE_NAME, PM.PROVINCE_ABBREVIATION 
                FROM REGION_BOUNDARY RM join PROVINCE_BOUNDARY PM on RM.id =Pm.region_id
        	WHERE RM.ISVISIBLE=TRUE AND PM.ISVISIBLE=TRUE AND ST_INTERSECTS(PM.SP_GEOMETRY, ST_GEOMFROMTEXT(''POLYGON(('||geometry||'))'',4326))';
        	
        	END IF;

RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE sql;
	
END; 
$BODY$;