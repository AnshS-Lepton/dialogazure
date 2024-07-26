

-- FUNCTION: public.fn_save_customer_ticket(integer, character varying, character varying, double precision, double precision, integer, character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_save_customer_ticket(integer, character varying, character varying, double precision, double precision, integer, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_save_customer_ticket(
	p_can_id integer,
	p_name character varying,
	p_address character varying,
	p_latitude double precision,
	p_longitude double precision,
	p_connection_entity_id integer,
	p_ticket_source character varying,
	p_reference_id character varying,
	p_connection_entity_type character varying)
    RETURNS TABLE(reference_id character varying, ticketid character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 DECLARE
   createdby  INTEGER;
BEGIN
 select created_by into createdby from  public.api_consumer_master where source=p_ticket_source;
    RETURN QUERY
    INSERT INTO ticket_master (
        can_id,
        customer_name,
        address,
		latitude,
		longitude,
		ticket_type_id,
        reference_ticket_id,
        reference_type,
		connection_entity_type,
		created_by,
		--assigned_date,
		created_on
    )
    VALUES (
        p_can_id,
        p_name,
        p_address,
		p_latitude,
		p_longitude,
		p_connection_entity_id,
		p_reference_id,
        p_ticket_source,
		p_connection_entity_type,
		createdby,		
		CURRENT_TIMESTAMP
    )
    RETURNING reference_ticket_id AS reference_id, ticket_id::Character varying;
END;
$BODY$;

ALTER FUNCTION public.fn_save_customer_ticket(integer, character varying, character varying, double precision, double precision, integer, character varying, character varying, character varying)
    OWNER TO postgres;

    -- FUNCTION: public.fn_get_customer_ticket_status(text)

-- DROP FUNCTION IF EXISTS public.fn_get_customer_ticket_status(text);

CREATE OR REPLACE FUNCTION public.fn_get_customer_ticket_status(
	p_ticket_id text)
    RETURNS TABLE(ticket_id text, ticket_status character varying, can_id text, created_on text, assigned_to text, assigned_date text, target_date text, remarks character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 
        tm.ticket_id ::text AS ticket_id,
        ttm.name as ticket_status,
        tm.can_id ::text AS can_id,
		to_char(tm.created_on, 'DD-Mon-YY'::text) AS created_on,
          um.user_name::text AS assigned_to,
		to_char(tm.assigned_date, 'DD-Mon-YY'::text) AS assigned_date,
		to_char(tm.target_date, 'DD-Mon-YY'::text) AS target_date,     
        tm.remarks
    FROM 
        ticket_master tm
		left join user_master as  um on tm.assigned_by=um.user_id 
		left join ticket_status_master ttm ON tm.ticket_type_id=ttm.id 
    WHERE 
        tm.ticket_id = p_ticket_id;
END;
$BODY$;

ALTER FUNCTION public.fn_get_customer_ticket_status(text)
    OWNER TO postgres;

-- FUNCTION: public.fn_get_tickets_by_id(character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone)

-- DROP FUNCTION IF EXISTS public.fn_get_tickets_by_id(character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone);

CREATE OR REPLACE FUNCTION public.fn_get_tickets_by_id(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_userid integer,
	p_searchfrom timestamp without time zone,
	p_searchto timestamp without time zone)
    RETURNS TABLE(totalrecords integer, s_no integer, ticket_id integer, customer_name character varying, ticket_type character varying, ticket_description character varying, reference_type character varying, ticket_status character varying, assigned_date timestamp without time zone, can_id integer, assigned_by character varying, assigned_to character varying, target_date timestamp without time zone, building_code character varying, bld_rfs_type character varying, completed_on timestamp without time zone, completed_by character varying, address character varying) 
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
BEGIN

sql:='';
LowerStart:='';
LowerEnd:='';

-- IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
--     LowerStart:='LOWER(';
--     LowerEnd:=')';
--   END IF;
RAISE INFO '%', sql;
select role_id into v_user_role_id from user_master where user_id=p_userId;
-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'tm.ticket_id' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS S_No, tm.ticket_id,tm.customer_name,ttm.ticket_type,tm.ticket_description,tm.reference_type,COALESCE(tm.ticket_status,'''') as ticket_status,COALESCE(tm.tm.assigned_date,'''') as assigned_date,tm.can_id, COALESCE(um.user_name, '''') AS assigned_by,um2.user_name as assinged_to,COALESCE(um.target_date, '''') as target_date,tm.building_code,bld.rfs_status as bld_rfs_type,COALESCE(um.completed_on, '''') as completed_on,um3.user_name as completed_by,tm.address
	FROM ticket_master tm 
	left join ticket_type_master ttm ON tm.ticket_type_id=ttm.id 
	left join user_master um ON tm.assigned_by=um.user_id
	left join user_master um2 ON tm.assigned_to=um2.user_id
	left join user_master um3 ON tm.completed_by=um3.user_id
	left join att_details_building bld ON tm.building_code=bld.network_id ';
	

--IF ((p_searchtext) != '' and (p_searchby) != '') THEN
--sql:= sql ||' AND upper(Cast('||p_searchby||' as TEXT)) LIKE upper(''%'||trim(p_searchtext)||'%'')';
RAISE INFO 'v_user_role_id %', v_user_role_id;
IF(v_user_role_id!=1)THEN
 
  sql:= sql ||' where (tm.assigned_to IS NOT NULL OR tm.assigned_to in (select * from fn_get_report_mapped_users('''||p_userId||''')))';--where tm.assigned_by='||p_userId||'';

--sql:= sql ||' where tm.assigned_to in (select * from fn_get_report_mapped_users('''||p_userId||'''))';--where tm.assigned_by='||p_userId||'';
Raise Info'query: %',sql;
else
sql:= sql ||'';
END IF;
IF ((p_searchtext) != '') THEN
sql:= sql ||' AND upper(coalesce(tm.ticket_status,'''') ||coalesce(tm.building_code,'''') ||coalesce(fn_get_date(tm.target_date),'''') ||coalesce(bld.rfs_status,'''')|| coalesce(tm.ticket_description,'''')|| coalesce(tm.reference_type,'''') || coalesce(tm.customer_name,'''') ||coalesce(tm.ticket_id,0) || coalesce(ttm.ticket_type,'''')|| coalesce(um2.user_name,''0'') || coalesce(um3.user_name,''0'') || coalesce(tm.can_id,''0'') || coalesce(fn_get_date(tm.assigned_date),'''') || coalesce(fn_get_date(tm.completed_on),'''')) LIKE upper(''%'||trim(p_searchtext)||'%'')';
--sql:= sql ||' AND coalesce(tm.ticket_status,'') LIKE upper(''%'||trim(p_searchtext)||'%'')';
 
END IF;

Raise Info'p_searchfrom01: %',p_searchfrom;
Raise Info'p_searchto01: %',p_searchto;
raise info'sql%',sql;
IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND tm.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and tm.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;
sql:= sql ||'';
	-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;
--GET New Tickets Count---
 raise info'sql%',sql;
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

EXECUTE ' ('||sql||')';

END ;
$BODY$;

ALTER FUNCTION public.fn_get_tickets_by_id(character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone)
    OWNER TO postgres;
