-- FUNCTION: public.fn_get_location_delta_audit_details(character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_location_delta_audit_details(character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_location_delta_audit_details(
	p_entity_name character varying,
	p_delta_date character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

 DECLARE
   sql TEXT;
   V_AUDIT_TABLE CHARACTER VARYING;
BEGIN
   V_AUDIT_TABLE:='';
 IF (coalesce(P_DELTA_DATE, '') = '') THEN
    P_DELTA_DATE := TO_CHAR(CURRENT_DATE, 'YYYY-MM-DD');
 END IF;

	
-- GET LAYER TABLE FROM LAYER DETAILS --
SELECT AUDIT_TABLE_NAME INTO V_AUDIT_TABLE FROM LAYER_DETAILS WHERE UPPER(LAYER_TITLE) = UPPER(P_ENTITY_NAME);

IF(coalesce(V_AUDIT_TABLE, '') != '') THEN
SQL:='SELECT '''||P_ENTITY_NAME||''' AS ENTITY_TYPE, PD.SYSTEM_ID, PD.NETWORK_ID, '''' AS SITE_CODE, PD.NETWORK_STATUS,
	json_build_object(''LATITUDE'',PD.LATITUDE,''LONGITUDE'',PD.LONGITUDE, ''BLOCK_CODE'', SPLIT_PART(PB.PROVINCE_ABBREVIATION, ''/'', 3),
	''BLOCK_NAME'', PB.PROVINCE_NAME ,   ''PROVINCE_CODE'', SPLIT_PART(PB.PROVINCE_ABBREVIATION, ''/'', 2),
	''PROVINCE_NAME'', RB.REGION_NAME , ''REGION_CODE'',SPLIT_PART(PB.PROVINCE_ABBREVIATION, ''/'', 1) ,''REGION_NAME'',RB.COUNTRY_NAME,
	''ADDRESS'', PD.ADDRESS) AS SITE_LOCATION, 
    PD.ACTION AS DELTA_TYPE, PD.CREATED_ON AS DELTA_ON, UM.USER_NAME AS DELTA_BY
    From '||V_AUDIT_TABLE||' PD 
    INNER JOIN PROVINCE_BOUNDARY PB ON PD.PROVINCE_ID = PB.ID 
    INNER JOIN REGION_BOUNDARY RB ON PD.REGION_ID = RB.ID
	INNER JOIN USER_MASTER UM ON PD.CREATED_BY = UM.USER_ID
    WHERE PD.CREATED_ON::DATE = '''||P_DELTA_DATE||''' ';
	
-- Raise info 'sql %', sql;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
END IF; 

END ;
$BODY$;

ALTER FUNCTION public.fn_get_location_delta_audit_details(character varying, character varying)
    OWNER TO postgres;
----------------------------------------------------------------------------------------


-- FUNCTION: public.fn_get_site_location(integer, integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_site_location(integer, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_site_location(
	p_page integer,
	p_page_size integer,
	p_entity_name character varying)
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
   LowerStart  CHARACTER VARYING;
   LowerEnd  CHARACTER VARYING;
   TotalPages INTEGER; 
   V_ENTITY_TABLE CHARACTER VARYING;

BEGIN
LowerStart:='';
LowerEnd:='';
V_ENTITY_TABLE:='';

IF(COALESCE(p_page, 0) = 0)
THEN
  p_page:= 1;	
END IF;

IF(COALESCE(p_page_size, 0) = 0)
THEN
  p_page_size:= 10;	
END IF;

-- GET LAYER TABLE FROM LAYER DETAILS --
SELECT LAYER_TABLE INTO V_ENTITY_TABLE FROM LAYER_DETAILS WHERE UPPER(LAYER_TITLE) = UPPER(P_ENTITY_NAME);

IF(COALESCE(V_ENTITY_TABLE, '') != '') THEN
    sql:='SELECT  ROW_NUMBER() OVER (ORDER BY SYSTEM_ID DESC)
    AS S_No, '''||P_ENTITY_NAME||''' AS ENTITY_TYPE, PD.SYSTEM_ID, PD.NETWORK_ID, '''' AS SITE_CODE, PD.NETWORK_STATUS,
	json_build_object(''LATITUDE'',PD.LATITUDE,''LONGITUDE'',PD.LONGITUDE, ''BLOCK_CODE'', SPLIT_PART(PB.PROVINCE_ABBREVIATION, ''/'', 3),
	''BLOCK_NAME'', PB.PROVINCE_NAME ,   ''PROVINCE_CODE'', SPLIT_PART(PB.PROVINCE_ABBREVIATION, ''/'', 2),
	''PROVINCE_NAME'', RB.REGION_NAME , ''REGION_CODE'',SPLIT_PART(PB.PROVINCE_ABBREVIATION, ''/'', 1) ,''REGION_NAME'',RB.COUNTRY_NAME,
	''ADDRESS'', PD.ADDRESS) AS SITE_LOCATION
    From '||V_ENTITY_TABLE||' PD 
    INNER JOIN PROVINCE_BOUNDARY PB ON PD.PROVINCE_ID = PB.ID 
    INNER JOIN REGION_BOUNDARY RB ON PD.REGION_ID = RB.ID
	INNER JOIN USER_MASTER UM ON PD.CREATED_BY = UM.USER_ID';

-- GET TOTAL RECORD COUNT
    EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET TOTAL PAGE COUNT
  TotalPages:= CASE WHEN TotalRecords % P_PAGE_SIZE = 0 THEN TotalRecords / P_PAGE_SIZE  ELSE (TotalRecords / P_PAGE_SIZE) + 1 END AS total_pages;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
  IF((P_PAGE) <> 0) THEN
	StartSNo:=  P_PAGE_SIZE * (P_PAGE - 1 ) + 1;
	EndSNo:= P_PAGE * P_PAGE_SIZE;

  sql:= 'SELECT json_build_object(''total_records'','||TotalRecords||',''page'','||P_PAGE||',''page_size'','||P_PAGE_SIZE||',''total_pages'','||TotalPages||') as pagination_metadata,*
                FROM (' || sql || ' ) T 
		       WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 
  ELSE
    sql:= 'SELECT json_build_object(''total_records'','||TotalRecords||',''page'','||P_PAGE||',''page_size'','||P_PAGE_SIZE||',''total_pages'','||TotalPages||') as pagination_metadata, * FROM (' || sql || ') T';   
  END IF; 

	
  RETURN QUERY
  EXECUTE 'select row_to_json(row) from ('||sql||') row';
END IF; 

END ;
$BODY$;

ALTER FUNCTION public.fn_get_site_location(integer, integer, character varying)
    OWNER TO postgres;

------------------------------------------------------------------