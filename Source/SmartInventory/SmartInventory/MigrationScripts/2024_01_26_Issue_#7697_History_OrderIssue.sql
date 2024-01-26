CREATE OR REPLACE FUNCTION public.fn_redline_get_entity_status_history(p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_userid integer, p_task_id integer, p_status character varying, p_status_updated_on timestamp without time zone, p_status_updated_by character varying, p_remarks character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords integer; 

BEGIN
-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (p_sortcolname) = '' THEN 'nt.system_id'  WHEN (p_sortcolname) = 'status' THEN 'esm.status'

 WHEN (p_sortcolname) = 'created_on' THEN 'nt.created_on'

 WHEN (p_sortcolname) = 'remarks' THEN 'nt.remarks'

 ELSE p_sortcolname END || ' ' ||CASE WHEN (p_sorttype) ='' THEN 'asc' else p_sorttype end||') AS S_No

  ,nt.task_name,nt.task_type,esm.status,to_char(nt.created_on, ''YYYY-MM-DD HH24:MI:SS'') as created_on,um.user_name ,nt.remarks,nt.system_id from redline_status_history nt 

  join redline_status_master esm on esm.system_id=nt.status

 join user_master um on um.user_id=nt.created_by  

WHERE nt.task_id= '||p_task_id||' and  nt.assigned_to= '||p_userid||' ORDER BY created_on DESC' ;
 
 RAISE INFO '%', sql;
 
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

END ;

$function$
;