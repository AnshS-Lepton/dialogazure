
------------------------------------- insert fiber link  filter type------------------------------------------------

INSERT INTO public.fiber_link_columns_settings
(column_name, column_sequence, display_name, is_active, created_by, created_on, modified_by, modified_on, res_field_key, is_kml_column)
VALUES('OP_ALIAS', 44, 'OP ALIAS', false, 1, now(), NULL, NULL, 'SI_OSP_GBL_GBL_FRM_102', false);



---------------------------------------- Modification to get fiber link columns mappings function ------------------------------- 


DROP FUNCTION IF EXISTS public.fn_get_fiber_link_columns_mappings();

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_columns_mappings(
	)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
BEGIN
	 
	-- EXECUTE AND RETURN QUERY RESULT..
	RETURN QUERY EXECUTE 'SELECT ROW_TO_JSON(ROW) FROM(SELECT column_name,is_active,display_name FROM fiber_link_columns_settings where is_active=true or column_name in (''sub_link_id'',''OP_ALIAS'') order by column_sequence) ROW';

END
$BODY$;

--------------------------------------Modification in fiber link status function ---------------------------------------

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
 main_link_Id  character varying;
BEGIN
sql:='';
select role_id into v_user_role_id from user_master where user_id=p_userid;

 

sql:= 'select tsm.name as fiber_link_status,tsm.color_code ,count(fl.system_id)::integer as fiber_link_count from vw_att_details_fiber_link fl right join 
fiber_link_status_master tsm on upper(tsm.name)=upper(fl.fiber_link_status) '; 

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
 if(p_searchby='sub_link_id')
 then 
 select fb.main_link_id into main_link_Id from vw_att_details_fiber_link fb where fb.link_id=p_searchtext;
 p_searchtext := main_link_Id;
 p_searchby := 'link_id';
 end if;
 if(p_searchby='OP_ALIAS')
 then 
 p_searchby := 'service_id';
 end if;
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

---------------------------------------------- Modification in fiber link details function ---------------------------------------


DROP FUNCTION IF EXISTS public.fn_get_fiber_link_details(integer, character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone);

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_details(
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
    main_link_Id  character varying;

BEGIN

sql:='';
LowerStart:='';
LowerEnd:='';

 --IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
    -- LowerStart:='LOWER(';
    -- LowerEnd:=')';
--END IF;
  if(p_sortcolname='OP_ALIAS')then p_sortcolname:='Service Id'; end if;
select role_id into v_user_role_id from user_master where user_id=p_userId;

-- FETCH ALL COLUMNS FROM COLUMN SETTINGS TABLE
	 
	SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM(
	 SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM fiber_link_columns_settings WHERE is_active=true and UPPER(column_name) not in('TOTAL_ROUTE_LENGTH','GIS_LENGTH','SUB_LINK_ID') order by Column_sequence) A;
	 

--- TO GET TOTAL_ROUTE_LENGTH & GIS_LENGTH
CREATE TEMP TABLE TEMP_CABLE_INFO(
 LINK_SYSTEM_ID INTEGER,
 CABLE_ID INTEGER
)ON COMMIT DROP;
 
CREATE TEMP TABLE TEMP_FIBER_LINK_DETAILS(
  LINK_SYSTEM_ID INTEGER,
  TOTAL_ROUTE_LENGTH DOUBLE PRECISION,
  GIS_LENGTH DOUBLE PRECISION
 )ON COMMIT DROP;

INSERT INTO TEMP_CABLE_INFO(LINK_SYSTEM_ID,CABLE_ID)
SELECT DISTINCT LINK_SYSTEM_ID,CABLE_ID FROM ATT_DETAILS_CABLE_INFO C
JOIN ATT_DETAILS_FIBER_LINK FIBER ON FIBER.SYSTEM_ID=C.LINK_SYSTEM_ID WHERE LINK_SYSTEM_ID>0;

 
INSERT INTO TEMP_FIBER_LINK_DETAILS(LINK_SYSTEM_ID,TOTAL_ROUTE_LENGTH,GIS_LENGTH)
 
SELECT DISTINCT T.LINK_SYSTEM_ID,ROUND((SUM(COALESCE(CABLE.cable_calculated_length,0)) + SUM(COALESCE(LOOP.LOOP_LENGTH,0)))::NUMERIC,2) AS TOTAL_ROUTE_LENGTH ,
ROUND((SUM(COALESCE(CABLE.CABLE_MEASURED_LENGTH,0)))::NUMERIC,2) AS CABLE_MEASURED_LENGTH
FROM TEMP_CABLE_INFO T
JOIN ATT_DETAILS_CABLE CABLE ON CABLE.SYSTEM_ID=T.CABLE_ID
LEFT JOIN( SELECT SUM(Loop_length) as loop_length,cable_system_id from ATT_DETAILS_LOOP LOOP group by LOOP.cable_system_id)loop ON LOOP.CABLE_SYSTEM_ID= CABLE.SYSTEM_ID
group by T.link_system_id
ORDER BY T.link_system_id DESC;

 
-- MANAGE SORT COLUMN NAME
IF (coalesce(TRIM(P_SORTCOLNAME,''))!='') THEN 

SELECT TRIM( trailing '	' from ''||P_SORTCOLNAME||'') into P_SORTCOLNAME;
select column_name into P_SORTCOLNAME from fiber_link_columns_settings WHERE UPPER(DISPLAY_NAME)=UPPER(P_SORTCOLNAME) ;
End IF;

 raise info'P_SORTCOLNAME% ',P_SORTCOLNAME;
  raise info'S_LAYER_COLUMNS_VAL% ',S_LAYER_COLUMNS_VAL;

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'fl.system_id' ELSE  'fl.'|| P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS S_No, 
fl.system_id,fl.network_id as "Network Id",'||S_LAYER_COLUMNS_VAL||', f2.TOTAL_ROUTE_LENGTH AS "Total Route Length(meter)",f2.GIS_LENGTH AS "GIS Length(meter)"
FROM vw_att_details_fiber_link fl
left join TEMP_FIBER_LINK_DETAILS f2 on fl.system_id=f2.link_system_id WHERE 1=1  ';
	

 IF(p_systemid >0 )THEN 
	sql:= sql ||'and fl.system_id='||p_systemid||'';
 END IF;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2 % ',sql;
 raise info'p_searchby % ',p_searchby;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
 if(p_searchby='sub_link_id')
 then 
 select fb.main_link_id into main_link_Id from vw_att_details_fiber_link fb where fb.link_id=p_searchtext;
 p_searchtext := main_link_Id;
 p_searchby := 'link_id';
 end if;
 if(p_searchby='OP_ALIAS')
 then 
 p_searchby := 'service_id';
 end if;
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;
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
RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$BODY$;

ALTER FUNCTION public.fn_get_fiber_link_details(integer, character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone)
    OWNER TO postgres;


