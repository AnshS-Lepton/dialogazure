
CREATE OR replace  VIEW public.vw_topology_plan_details
AS select 
tr.id,
tr.ring_code,
tr.ring_capacity,
ts.segment_code,
ts.agg1_site_id,
ts.agg2_site_id,
trr.region_name 
from top_ring tr
join top_segment ts on ts.id =tr.segment_id
join top_region trr on trr.id =ts.region_id ;

-----------------------------------------------------------------------------------------------------------------------
-- DROP FUNCTION public.fn_get_ring_details(varchar, varchar, int4, int4, varchar, varchar, varchar);
CREATE OR REPLACE FUNCTION public.fn_get_regionlist()
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

BEGIN

RETURN QUERY select row_to_json(row) from (select id as key ,region_name as value from top_region) row;

END
$function$
;

------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_segmentlist()
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

BEGIN

RETURN QUERY select row_to_json(row) from (select id as key ,segment_code as value from top_segment) row;

END
$function$
;
------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_ringtypelist()
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

BEGIN

RETURN QUERY select row_to_json(row) from (select id as key ,ring_code as value from top_ring where ring_code is not null ) row;

END
$function$
;

----------------------------

CREATE OR REPLACE FUNCTION public.fn_get_ring_details(p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_region_name character varying,p_segment_code character varying,p_ring_code character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

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
  

-- FETCH ALL COLUMNS FROM COLUMN SETTINGS TABLE
	 

	 
 
  
-- MANAGE SORT COLUMN NAME
/*IF (coalesce(TRIM(P_SORTCOLNAME,''))!='') THEN 

SELECT TRIM( trailing '	' from ''||P_SORTCOLNAME||'') into P_SORTCOLNAME;
select column_name into P_SORTCOLNAME from fiber_link_columns_settings WHERE UPPER(DISPLAY_NAME)=UPPER(P_SORTCOLNAME);
End IF;*/

/* raise info'P_SORTCOLNAME% ',P_SORTCOLNAME;
  raise info'S_LAYER_COLUMNS_VAL% ',S_LAYER_COLUMNS_VAL;*/

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'id' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS  S_No,id,ring_code,ring_capacity,segment_code,agg1_site_id,agg2_site_id,region_name  from vw_topology_plan_details where 1=1  ';

 IF(p_region_name != '')THEN 

	sql:= sql ||'and region_name ilike ''%'||(p_region_name)||'%'' ';
 END IF;

if(p_segment_code != '') then

sql:= sql ||'and segment_code ilike ''%'||(p_segment_code)||'%'' ';
end if;

if(p_ring_code != '') then

sql:= sql ||'and ring_code ilike ''%'||(p_ring_code)||'%'' ';
end if;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2% ',sql;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
/*IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;*/
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

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$function$
;