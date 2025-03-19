

update  fiber_link_columns_settings set is_active=true,
where column_name in('created_by','created_on','otdr_distance');

update  fiber_link_columns_settings set column_sequence=5 where column_name='otdr_distance';  
update  fiber_link_columns_settings set column_sequence=6 where column_name='link_type' ;
--------------------------------------------------------------------------------------------------------------------------------------------------------

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
	 SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM fiber_link_columns_settings WHERE is_active=true order by Column_sequence) A;
	 
	-- update fiber_link_status free when no fiberlink attach at any cable 
    update att_details_fiber_link set fiber_link_status ='Free' 
    where system_id not in ( select distinct link_system_id from att_details_cable_info 
    where link_system_id > 0)
    and fiber_link_status ='Associated' ; 

  
-- MANAGE SORT COLUMN NAME
IF (coalesce(TRIM(P_SORTCOLNAME,''))!='') THEN 

SELECT TRIM( trailing '	' from ''||P_SORTCOLNAME||'') into P_SORTCOLNAME;
select column_name into P_SORTCOLNAME from fiber_link_columns_settings WHERE UPPER(DISPLAY_NAME)=UPPER(P_SORTCOLNAME);
End IF;

 raise info'P_SORTCOLNAME% ',P_SORTCOLNAME;
  raise info'S_LAYER_COLUMNS_VAL% ',S_LAYER_COLUMNS_VAL;

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'fl.system_id' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS S_No, 
fl.system_id,'||S_LAYER_COLUMNS_VAL||'
	FROM vw_att_details_fiber_link fl WHERE 1=1  ';
	

 IF(p_systemid >0 )THEN 
	sql:= sql ||'and fl.system_id='||p_systemid||'';
 END IF;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2% ',sql;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
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
	----------------------------------------------------------------------------------------------------------------------------------------------
	-- FUNCTION: public.fn_get_fiber_link_prefix(character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_fiber_link_prefix(character varying);

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_prefix(
	p_link_prefix character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    sql TEXT;
BEGIN
IF  NOT EXISTS (SELECT 1 FROM att_details_fiber_link WHERE link_ID  ILIKE '' || p_link_prefix || '%' || '') 
THEN
       sql := ' SELECT '''||p_link_prefix||''' || ''0000000001'' AS link_prefix';
    ELSE
        sql := '
        WITH max_link AS (
            SELECT 
                MAX(CAST(NULLIF(REGEXP_REPLACE(SUBSTRING(link_ID FROM ' || (LENGTH(p_link_prefix) + 1) || '), ''[^0-9]'', '''', ''g''), '''') AS BIGINT)) AS max_id
            FROM 
                att_details_fiber_link
            WHERE 
                link_ID ILIKE ' || quote_literal(p_link_prefix || '%') || '
        )
        SELECT 
            ' || quote_literal(p_link_prefix) || ' || LPAD((COALESCE(max_id, 0) + 1)::TEXT, 12, ''0'') AS link_prefix
        FROM 
            max_link';
	
	END IF;
    RAISE INFO '%', sql;
    RETURN QUERY EXECUTE 'SELECT row_to_json(row) FROM (' || sql || ') row';

END;
$BODY$;

ALTER FUNCTION public.fn_get_fiber_link_prefix(character varying)
    OWNER TO postgres;
---------------------------------------------------------------------------------------------------------------------------------------------------------