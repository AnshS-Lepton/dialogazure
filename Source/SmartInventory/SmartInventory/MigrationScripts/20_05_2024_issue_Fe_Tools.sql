INSERT INTO public.module_master
( module_name, module_description, icon_content, icon_class, created_by, modified_by, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES( 'FE Tools', 'To FE Tools.', '', '', 1,  287, 'Admin', true, 'USR-VIEW-FE_Tools', 98, 6, false, 'FETools/Viewfetools', NULL);
------------------------------------------------------------------------------------------------------------------
INSERT INTO public.global_settings
( "key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES( 'validDocumentTypesFetools', '.pdf,.jpeg,.jpg,.png,', 'valid File upload extensions', 'Web', false, 'string', 1.0, 1.0, 5, '2021-02-11 17:14:08.380', NULL, NULL, true, true, false, NULL, NULL);
----------------------------------------------------------------------------------------------------------------------------------------------
ALTER TABLE public.user_fe_tool_mapping ADD barcode varchar NULL;
ALTER TABLE public.user_fe_tool_mapping ADD serial_number varchar NULL;
ALTER TABLE public.user_fe_tool_mapping ADD created_by integer default 0;
ALTER TABLE public.user_fe_tool_mapping ADD modified_by integer default 0;
ALTER TABLE public.user_fe_tool_mapping ADD created_on timestamp DEFAULT now() null;
ALTER TABLE public.user_fe_tool_mapping ADD modified_on  timestamp NULL;
ALTER TABLE public.user_fe_tool_mapping ADD date_value  Date ;

ALTER TABLE public.user_fe_tool_mapping RENAME TO user_tools_mapping;

-----------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE public.user_tools_attachment (
	file_name varchar(255) NULL,
	file_extension varchar(20) NULL,
	file_location varchar(500) NULL,
	upload_type varchar(20) NULL,
	uploaded_by int4 NULL,
	uploaded_on timestamp NULL,
	file_size int4 NULL,
	id int8 DEFAULT nextval('fe_tools_attachement_id_seq'::regclass) NOT NULL,
	tools_mapping_id int8 NULL
);
----------------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_user_details( )
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
v_Query text;
BEGIN
v_Query:='';

 RETURN QUERY EXECUTE 'select row_to_json(row) from (select user_id  as value, user_name as key from vw_user_master  where is_active =true) row';


END
$function$
;




----------------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_user_details(id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
v_Query text;
BEGIN
v_Query:='';

 RETURN QUERY EXECUTE 'select row_to_json(row) from (select user_id  as value, user_name as key from vw_user_master  where user_id = '||id||' and  is_active =true) row';


END
$function$
;
----------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_get_fetools_record(pid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
    sql TEXT;
 
begin
			-- RAISE INFO '%', sql;  
return query
select row_to_json(row) from (SELECT id,user_name, tool_name,barcode, serial_number,created_by_text, modified_by_text FROM vw_fe_tools_master WHERE id=pid
) row;

	
END ;
$function$
;
----------------------------------------------------------------------------------------------------------------
-- DROP FUNCTION public.fn_get_fetools_details(varchar, varchar, int4, int4, varchar, varchar, int4);


-- DROP FUNCTION public.fn_get_fetools_details(varchar, varchar, int4, int4, varchar, varchar, int4);

CREATE OR REPLACE FUNCTION public.fn_get_fetools_details(searchby character varying, searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_totalrecords integer)
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
begin
	
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
------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_fe_tool_data(pid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    EXECUTE 'select row_to_json(row) from (select id  as value, tool_name as key from fe_tools_master  where id = '||pid||') row';
END;
$function$
;

-----------------------------------------------------------------------------------------

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
          GROUP BY all_types.id) AS upload_type
   FROM user_tools_mapping lgm
     JOIN user_master um ON lgm.user_id = um.user_id
     LEFT JOIN user_master um1 ON lgm.created_by = um.user_id
     LEFT JOIN user_master um2 ON um1.user_id = lgm.modified_by
     JOIN fe_tools_master ftm ON ftm.id = lgm.tool_id;
	 -------------------------------------------------------------------------