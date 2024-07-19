alter table export_report_log
add column log_type character varying;

--------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_auditlog_entity_export_log(character varying, character varying, integer, integer, character varying, character varying, integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_auditlog_entity_export_log(character varying, character varying, integer, integer, character varying, character varying, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_auditlog_entity_export_log(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_user_id integer,
	p_timeinterval character varying)
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
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   v_regex character varying;

BEGIN

	select value into v_regex from global_settings where key='SqlInjectionRegex';
	if (p_searchby !~* v_regex or p_searchtext !~* v_regex or p_sorttype !~* v_regex or p_timeinterval !~* v_regex or p_sortcolname !~* v_regex ) then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
	begin
----select * from fn_get_entity_export_log('','',1,10,'','',5,'2 months');
LowerStart:='';
LowerEnd:='';

 IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
   	IF EXISTS (select 1 from information_schema.columns where upper(table_name) = upper('export_report_log') and 
	upper(column_name) = upper(P_SORTCOLNAME) and upper(data_type) in('CHARACTER VARYING','TEXT')) THEN
		LowerStart:='LOWER(';
	else
		LowerStart:='(';
	end if;
	LowerEnd:=')';	
END IF;

RAISE INFO '%', sql;

-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'export_started_on' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')
AS S_No,l.user_id, export_started_on, export_ended_on, file_name, file_type, total_entity, planned, asbuilt, dormant, sp_geometry, status, file_location
	,file_extension,um.user_name,log_type
    FROM export_report_log l inner join vw_user_master um on um.user_id=l.user_id
    where l.log_type=''audit'' and export_started_on >= (now() - INTERVAL '''||$8||''') ';

IF (p_user_id>0) THEN
sql:= sql ||' and l.user_id = '||p_user_id||'';
END IF;

IF ((p_searchtext) != '') THEN
sql:= sql ||' AND page_title LIKE lower(''%'||$2||'%'')';
END IF;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 

RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

end;
	end if;
END ;
$BODY$;

ALTER FUNCTION public.fn_get_auditlog_entity_export_log(character varying, character varying, integer, integer, character varying, character varying, integer, character varying)
    OWNER TO postgres;
