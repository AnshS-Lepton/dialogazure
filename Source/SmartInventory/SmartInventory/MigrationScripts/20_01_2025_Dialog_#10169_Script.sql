/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: This function is used to pull the network ticket details
ModifiedOn: 14 Jan 2025 
ModifiedBy: Chandra Shekhar Sahni 
Purpose: Modified to pull network ticket data from vw_att_details_networktickets and ticket_type_role_mapping only
------------------------------------------*/
-- DROP FUNCTION public.fn_nwt_get_ticket_details(varchar, varchar, int4, int4, varchar, varchar, int4, timestamp, timestamp, int4, int4, int4, int4);

CREATE OR REPLACE FUNCTION public.fn_nwt_get_ticket_details(p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_userid integer, p_searchfrom timestamp without time zone, p_searchto timestamp without time zone, p_ticket_type_id integer, p_region_id integer, p_province_id integer, p_ticket_status_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE
sql TEXT;
NWstatus TEXT;
StartSNo INTEGER;
EndSNo INTEGER;
TotalRecords integer;
v_user_role_id integer;
v_is_admin_rights_enabled boolean;
BEGIN

p_searchby:=replace(replace(p_searchby,'(',''),')','');

select role_id,is_admin_rights_enabled into v_user_role_id,v_is_admin_rights_enabled from user_master where user_id=p_userid;

-- DYNAMIC QUERY
raise info'role_id:-% ',v_user_role_id;
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (p_sortcolname) = '' THEN 'nt.ticket_id' ELSE 'nt.'||p_sortcolname END || ' ' ||CASE WHEN (p_sorttype) ='' THEN 'desc' else p_sorttype end||') AS S_No
,nt.*,ttrm.id,ttrm.ticket_type_id,ttrm.role_id,ttrm.is_create,ttrm.is_edit,ttrm.is_view,ttrm.is_approve,ttrm.ticket_type from vw_att_details_networktickets nt
inner join ticket_type_role_mapping ttrm on nt.ticket_type_id=ttrm.ticket_type_id and ttrm.role_id ='||v_user_role_id||' and ttrm.is_view=true
inner join user_permission_area upa on nt.province_id=upa.province_id and upa.user_id='||p_userid||'
WHERE 1=1';

raise info'sql:-% ',sql;
NWstatus:= 'SELECT tsm.name::character varying as ticket_status,count(nt.ticket_status_id) as ticket_count,tsm.color_code,tsm.opacity from
vw_att_details_networktickets nt right join
ticket_status_master tsm on tsm.id=nt.ticket_status_id
inner join user_permission_area upa on nt.province_id=upa.province_id and upa.user_id='||p_userid||'
';


IF(p_region_id >0 )THEN
sql:= sql ||'and nt.region_id='||p_region_id||'';
NWstatus:= NWstatus ||'and nt.region_id='||p_region_id||'';
END IF;
IF(p_province_id >0 )THEN
sql:= sql ||'and nt.province_id='||p_province_id||'';
NWstatus:= NWstatus ||'and nt.province_id='||p_province_id||'';
END IF;
IF(p_ticket_status_id >0 )THEN
sql:= sql ||'and nt.ticket_status_id='||p_ticket_status_id||'';
NWstatus:= NWstatus ||'and nt.ticket_status_id='||p_ticket_status_id||'';
END IF;

IF(p_ticket_type_id >0 )THEN
sql:= sql ||'and nt.ticket_type_id='||p_ticket_type_id||'';
NWstatus:= NWstatus ||'and nt.ticket_type_id='||p_ticket_type_id||'';
END IF;
raise info'v_user_role_id :-% ',v_user_role_id;

IF(v_user_role_id >0 )THEN
sql:= sql ||' and case when ('||v_user_role_id||' =1 or '||v_is_admin_rights_enabled||'=true) then 1=1 else 1=1 end AND (nt.assigned_to='||p_userid||' OR cast(nt.created_by as integer) in (select user_id from user_master where manager_id in ('||p_userid||') or user_id in ('||p_userid||')))';

NWstatus:= NWstatus ||' and case when ('||v_user_role_id||' =1 or '||v_is_admin_rights_enabled||'=true) then 1=1 else nt.ticket_type_id in (select ticket_type_id from ticket_type_role_mapping where role_id= '||v_user_role_id||' and is_view=true) end';
END IF;

raise info'sqltest% ',sql;

IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
NWstatus:= NWstatus ||' AND lower(nt.'||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;

IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND nt.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and nt.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';
NWstatus:= NWstatus ||' AND nt.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and nt.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';
END IF;
raise info'sql-record:-% ',sql;
-- GET TOTAL RECORD COUNT
EXECUTE 'SELECT COUNT(1) FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
IF((p_pageno) <> 0) THEN
StartSNo:= p_pagerecord * (p_pageno - 1 ) + 1;
EndSNo:= p_pageno * p_pagerecord;
sql:= 'SELECT '||TotalRecords||' as totalRecords, *
FROM (' || sql || ' ) T
WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo;

ELSE
sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T ';
END IF;

RAISE INFO 'lstNWStatus:-%', NWstatus;
RAISE INFO 'lstNWDetails:-%', sql;

RETURN QUERY
EXECUTE 'select row_to_json(row) from (
select(
select array_to_json(array_agg(row_to_json(x)))
from (
'||NWstatus||' where tsm.module=''Network_Ticket'' and is_active=true group by tsm.name,nt.ticket_status_id,tsm.color_code,tsm.action_seq,tsm.opacity order by tsm.action_seq
) x
) as lstNWStatus,
(
select array_to_json(array_agg(row_to_json(x)))
from('||sql||')x

) as lstNWDetails
) row';

END ;
$function$
;
