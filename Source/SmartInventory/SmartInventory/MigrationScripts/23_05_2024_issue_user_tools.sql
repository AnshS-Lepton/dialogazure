-- DROP FUNCTION public.fn_get_fetools_details(varchar, varchar, int4, int4, varchar, varchar, int4, int4);

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

			 created_by_text, modified_by_text,date_value,upload_type FROM vw_fe_tools_master WHERE 1=1';

			if(coalesce(v_role_id,0)>1) 
			then
				sql:=sql||' and user_id = '||p_user_id||' ';
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

-------------------------------------------------------------------------------------------------------

update module_master set module_name ='User Tools' where id =496
-----------------------------------------------------------------------------------------------------------

CREATE OR REPLACE VIEW public.vw_fe_tools_master
AS SELECT lgm.id,
    um.user_name,
    ftm.tool_name,
    lgm.barcode,
    lgm.serial_number,
    lgm.created_by,
    lgm.created_on,
    lgm.modified_by,
    lgm.modified_on,
    lgm.date_value,
    um1.user_name AS created_by_text,
    um2.user_name AS modified_by_text,
    ( SELECT
                CASE
                    WHEN count(user_tools_attachment.upload_type) > 1 THEN 'Both'::text
                    ELSE max(user_tools_attachment.upload_type::text)
                END AS upload_type
           FROM ( SELECT DISTINCT user_tools_mapping.id
                   FROM user_tools_mapping) all_types
             LEFT JOIN user_tools_attachment ON all_types.id = user_tools_attachment.tools_mapping_id
          WHERE all_types.id = lgm.id
          GROUP BY all_types.id) AS upload_type,
    lgm.user_id
   FROM user_tools_mapping lgm
     JOIN user_master um ON lgm.user_id = um.user_id
     LEFT JOIN user_master um1 ON lgm.created_by = um.user_id
     LEFT JOIN user_master um2 ON um1.user_id = lgm.modified_by
     JOIN fe_tools_master ftm ON ftm.id = lgm.tool_id;
	 
	 ----------------------------------------------------------------------------------------------