
CREATE OR REPLACE FUNCTION public.fn_accessories_layer_mapping_get_list(p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_userid integer, p_isactive integer, p_searchby character varying, p_searchtext character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords integer; 
begin
	
	if (p_searchby !~* '^[a-zA-Z0-9\s_]*$' or p_searchtext !~* '^[a-zA-Z0-9\s_@]*$' or p_sortcolname !~* '^[a-zA-Z0-9\s_]*$' or
		p_sorttype !~* '^[a-zA-Z0-9\s_]*$') then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
BEGIN

 if(coalesce(p_sortcolname,'')!='')THEN
p_sortcolname:='am.'||p_sortcolname;
END IF;

-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (p_sortcolname) = '' THEN 'am.id' ELSE p_sortcolname END || ' ' ||CASE WHEN (p_sorttype) ='' THEN 'desc' else p_sorttype end||') AS S_No
  ,am.id,am.name,am.layer_name,am.min_quantity,am.max_quantity ,created_by_text,am.created_on AS created_on,am.is_active,am.modified_by_text,am.modified_on from vw_accessories_layer_mapping am where 1=1  ' ;
  IF ((p_isactive) != 0 ) THEN
sql:= sql ||' AND is_active='||(CASE WHEN (p_isactive) = 1 THEN true ELSE false END)||'';
END IF;
   IF ((p_searchtext) != '' and (p_searchby) != '') THEN
--sql:= sql ||' AND lower('||p_searchby||') LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
sql:= sql ||' AND lower('||quote_ident($7)||') LIKE lower(''%'||TRIM(($8))||'%'')';
END IF;
-- GET TOTAL RECORD COUNT
EXECUTE  'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((p_pageno) <> 0) THEN
	StartSNo:=  p_pagerecord * (p_pageno - 1 ) + 1;
	EndSNo:= p_pageno * p_pagerecord;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T ';                  
 END IF; 

RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

	end;
	end if;
END ;

$function$
;

CREATE OR REPLACE FUNCTION public.fn_accessories_get_list(p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_userid integer, p_isactive integer, p_searchby character varying, p_searchtext character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords integer; 

begin
	
	if (p_searchby !~* '^[a-zA-Z0-9\s_]*$' or p_searchtext !~* '^[a-zA-Z0-9\s_@]*$' or p_sortcolname !~* '^[a-zA-Z0-9\s_]*$' or
		p_sorttype !~* '^[a-zA-Z0-9\s_]*$') then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
BEGIN

 if(coalesce(p_sortcolname,'')!='')THEN
p_sortcolname:='am.'||p_sortcolname;
END IF;


-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (p_sortcolname) = '' THEN 'am.id' ELSE p_sortcolname END || ' ' ||CASE WHEN (p_sorttype) ='' THEN 'desc' else p_sorttype end||') AS S_No
  ,id,am.name,am.display_name,um.user_name as created_by_text,fn_get_date(am.created_on) AS created_on,am.is_active,um1.user_name as modified_by_text ,fn_get_date(am.modified_on) AS modified_on from accessories_master am 
  join user_master um on um.user_id=am.created_by left join user_master um1 on um1.user_id=am.modified_by where 1=1 ' ;
    IF ((p_isactive) != 0 ) THEN
sql:= sql ||' AND am.is_active='||(CASE WHEN (p_isactive) = 1 THEN true ELSE false END)||'';
END IF;
  IF ((p_searchtext) != '' and (p_searchby) != '') THEN
--sql:= sql ||' AND lower(am.'||p_searchby||') LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
sql:= sql ||' AND lower(am.'||quote_ident($7)||') LIKE lower(''%'||TRIM($8)||'%'')';

END IF;
-- GET TOTAL RECORD COUNT
EXECUTE  'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((p_pageno) <> 0) THEN
	StartSNo:=  p_pagerecord * (p_pageno - 1 ) + 1;
	EndSNo:= p_pageno * p_pagerecord;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, * 
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T ';                  
 END IF; 

RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

	end;
	end if;
END ;

$function$
;

