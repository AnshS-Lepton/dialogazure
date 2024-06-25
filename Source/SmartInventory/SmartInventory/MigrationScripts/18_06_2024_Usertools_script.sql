CREATE OR REPLACE FUNCTION public.fn_get_fetools_details(searchby character varying, searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_totalrecords integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
    sql TEXT;
    SqlQueryCnt TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying; 
   v_role_id integer;
begin
	select role_id into v_role_id from user_master where user_id=p_user_id;
	
	if (searchby !~* '^[a-zA-Z0-9\s_]*$' or searchtext !~* '^[a-zA-Z0-9\s_]*$' or p_sortcolname !~* '^[a-zA-Z0-9\s_]*$' or
		p_sorttype !~* '^[a-zA-Z0-9\s_]*$') then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
		BEGIN

			-- RAISE INFO '%', sql;  
			sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (P_SORTCOLNAME) = '' THEN 'id' ELSE P_SORTCOLNAME END ||' ' ||CASE WHEN 

			(P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No,id,user_name, tool_name,barcode, serial_number,  

			 created_by_text, modified_by_text,date_value,upload_type,is_accepted,user_id,created_by FROM vw_fe_tools_master WHERE 1=1';

			if(coalesce(v_role_id,0)>1) 
			then
				sql:=sql||' and (user_id = '||p_user_id||' or created_by = '||p_user_id||' )';
			end if;



			IF (upper(searchText) != '' and (searchby) != '') THEN
			--sql:= sql ||' AND lower('||searchby||') LIKE lower(''%'||searchText||'%'')';
			sql:= sql ||' AND lower("'|| quote_ident($1) ||'") LIKE lower(''%'|| $2 ||'%'')';

			END IF;

			SqlQueryCnt:= 'SELECT COUNT(1)  FROM ('||sql||') as a';
			 EXECUTE   SqlQueryCnt INTO P_TOTALRECORDS;
			 IF((P_PAGENO) <> 0) THEN
			  
				StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
				EndSNo:= P_PAGENO * P_PAGERECORD;
				sql:= 'SELECT '||P_TOTALRECORDS||' as totalRecords, *
							FROM (' || sql || ' ) T 
							WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

				 ELSE
						   sql:= 'SELECT '||P_TOTALRECORDS||' as totalRecords, * FROM (' || sql || ' order by created_on desc) T';                  
			  END IF; 

			RAISE INFO '%', sql;
				
			RETURN QUERY
			EXECUTE 'select row_to_json(row) from ('||sql||') row';

		end;
	end if;
END ;
$function$
;

-----------------------------------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_user_details(role_id integer, active character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
v_Query text;
BEGIN
v_Query:='';
---here role_id is User_id;
if role_id =1 then 
 RETURN QUERY EXECUTE 'select row_to_json(row) from (select user_id  as value, user_name as key from vw_user_master  where  is_active =true) row';
else
 RETURN QUERY EXECUTE 'select row_to_json(row) from (select user_id  as value, user_name as key from vw_user_master  where manager_id = '||role_id||' and  is_active =true) row';

end if;
END
$function$
;
