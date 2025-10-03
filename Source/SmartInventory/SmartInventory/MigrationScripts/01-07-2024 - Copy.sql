
--------------------------------- Add key for the export report in mobile application----------------------------------
INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('IsExportReportEnabled', '0', 'To enable / disable Export summary ', 'Mobile', false, 'string', 0, 0, 1, now(), NULL, NULL, true, false, false, NULL, NULL);


---------------------------- Used for Network ticket details-----------------------------------------

 DROP FUNCTION IF EXISTS public.fn_nwt_get_ticket_entity_details(integer, integer, integer, integer, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_nwt_get_ticket_entity_details(
	p_pageno integer,
	p_pagerecord integer,
	p_userid integer,
	p_ticket_id integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_searchtext character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
lyrtitle record;
sql TEXT;
NWEntityStatus TEXT;
StartSNo INTEGER;
EndSNo INTEGER;
TotalRecords integer;
v_entity_name character varying;
v_new_entity_name character varying;
v_user_role_id integer;
v_ticket_status character varying;
v_isAuthorizedToApprove boolean;
BEGIN
v_isAuthorizedToApprove:=false;
sql:='';
select sm.name,
(select count(1)>0 from user_manager_mapping map where map.user_id=att.assigned_to and map.manager_id=p_userid)
into v_ticket_status,v_isAuthorizedToApprove from ticket_status_master sm
inner join att_details_networktickets att on sm.id=att.ticket_status_id where att.ticket_id=p_ticket_id;

create temp table temp_nt_entity_detalis
(
id integer,
entity_type character varying(50),
entity_title character varying(50),
network_id character varying(50),
entity_name character varying,
region character varying(50),
province character varying(50),
network_ticket_id character varying(50),
geom_type character varying(50),
status character varying(10),
status_description character varying(50),
status_color character varying(50),
layer_name character varying(50),
entity_action character varying(50),
is_freezed Boolean,
action_color_code text,
created_on timestamp,
status_remarks character varying(500),
entity_action_atr character varying(50),
entity_status character varying,
opacity integer,
display_name character varying,
is_revert_allowed boolean,
is_manual_barcode boolean,
is_barcode_verified boolean,
is_trace_validate boolean,
splitter_type character varying,
network_status character varying,
is_manual_meter_reading boolean,
is_meter_reading_verified boolean    
)on commit drop;

FOR lyrtitle IN select distinct layer_title,layer_table,layer_name,geom_type from layer_details where isvisible=true and is_network_ticket_entity=true
LOOP
RAISE INFO 'table_name %', lyrtitle.layer_table;
RAISE INFO 'entity_name %', v_new_entity_name;
v_entity_name:=lower(lyrtitle.layer_name)||'_name';
select coalesce((SELECT column_name into v_new_entity_name FROM information_schema.columns
WHERE table_name=lyrtitle.layer_table and column_name in('name',v_entity_name,'network_name','fault_id')),'Null');
IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name=lyrtitle.layer_table and column_name ='source_ref_id')THEN
BEGIN
sql:='insert into
temp_nt_entity_detalis(id,entity_type,entity_title,network_id,entity_name,region,province,
network_ticket_id,geom_type,status,layer_name,entity_action,
action_color_code,created_on,entity_action_atr,status_description,is_freezed,status_color,entity_status,
opacity,display_name,is_revert_allowed,is_manual_barcode,is_barcode_verified,is_trace_validate,splitter_type,network_status,
is_manual_meter_reading,is_meter_reading_verified)

select tbl.system_id,'''||lyrtitle.layer_name||''' ,'''||lyrtitle.layer_title||''',tbl.network_id,tbl.network_name,
tbl.region_name,tbl.province_name, tbl.ticket_network_id,'''||lyrtitle.geom_type||''',tbl.status,'''||lyrtitle.layer_name||'''
,tbl.entity_action,tbl.color_code,tbl.created_on,tbl.entity_action_abbr,esm.description,
esm.is_freezed,esm.color_code,tbl.entity_status,esm.opacity,tbl.display_name,coalesce(esm.is_revert_allowed,true),is_manual_barcode,
is_barcode_verified,is_trace_validate,splitter_type,network_status,is_manual_meter_reading,is_meter_reading_verified from(
Select lv.system_id,lv.network_id,lv.'||v_new_entity_name||' as network_name,
rb.region_name,pb.province_name, adn.network_id as ticket_network_id,
'''||lyrtitle.geom_type||''',lv.status,'''||lyrtitle.layer_name||''',
case when coalesce(eom.entity_action,'''') = '''' then ''New'' else eom.entity_action end as entity_action
,eom.color_code
,lv.created_on,case when coalesce(eom.description,'''') = '''' then ''NW'' else eom.description end as entity_action_abbr,
(case when lv.status=''D'' and lv.is_new_entity=true then ''NEW''
when lv.status=''D'' and (attribute_info->>''curr_status'')::text is not null and (attribute_info->>''curr_status'')::text=''D''
then ''DMT''
when lv.status=''D'' and lv.is_new_entity=false then ''MFD''
else lv.status end) as entity_status,pm.display_name'; 
IF lyrtitle.layer_name NOT IN ('Fault', 'Loop', 'Coupler') THEN
sql:=sql||',lv.is_manual_barcode,lv.is_barcode_verified';
else
sql:=sql||','||false||' as is_manual_barcode,'||false||' as is_barcode_verified';
end if;

if(lyrtitle.layer_name='Splitter')
then
    sql:=sql||',lv.is_trace_validate,lv.splitter_type,lv.network_status,lv.is_manual_meter_reading,is_meter_reading_verified';
else
    sql:=sql||','||false||' as is_trace_validate,'''' as splitter_type,'''' as network_status,'||false||' as is_manual_meter_reading,
    '||false||' as is_meter_reading_verified';
end if;
sql:=sql||' from '||lyrtitle.layer_table||' lv

-- from '||lyrtitle.layer_table||' lv
join att_details_networktickets adn on lv.source_ref_id=adn.ticket_id::character varying and Upper(source_ref_type)=''NETWORK_TICKET''
join region_boundary rb on rb.id=lv.region_id join province_boundary pb on pb.id=lv.province_id
left join vw_att_details_edit_entity_info eei on lv.system_id=eei.entity_system_id and upper(eei.entity_type)=upper('''||lyrtitle.layer_name||''')
left join entity_operations_master eom on eom.id=eei.entity_action_id
inner join '||lyrtitle.geom_type||'_master pm on pm.system_id=lv.system_id and upper(pm.entity_type)=upper('''||lyrtitle.layer_name||''')
where adn.ticket_id='||p_ticket_id||')tbl
left join entity_status_master esm on upper(tbl.entity_status)=upper(esm.status)' ;

execute sql;
--when lv.status=''D'' and lv.is_new_entity=false then ''MDFT''
RAISE INFO '%', sql;
END;
END IF ;
END LOOP;

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (coalesce(p_sortcolname,'')) = '' THEN 'nt.id' ELSE p_sortcolname END || ' ' ||CASE WHEN
(coalesce(p_sorttype,'')) ='' THEN 'ASC' else p_sorttype end||') AS S_No
,* from temp_nt_entity_detalis nt WHERE 1=1 ' ;

RAISE INFO '||%||', sql;

IF ((p_searchtext) != '') THEN
NWEntityStatus:= 'SELECT tsm.description::character varying as ticket_status ,count(nt.status) as ticket_count,tsm.color_code,tsm.opacity from
entity_status_master tsm left join temp_nt_entity_detalis nt on upper(tsm.status)=upper(nt.entity_status)
where is_active=true and lower(entity_title::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
sql:= sql ||' AND lower(entity_title::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
ELSE
NWEntityStatus:= 'SELECT tsm.description::character varying as ticket_status ,coalesce(count(nt.status),0) as ticket_count,tsm.color_code,tsm.opacity from
entity_status_master tsm left join temp_nt_entity_detalis nt on upper(tsm.status)=upper(nt.entity_status)
where is_active=true';
END IF;
if upper(v_ticket_status)='INPROGRESS'
then
NWEntityStatus:=NWEntityStatus||' and tsm.status in (''NEW'',''MFD'',''DMT'')';
elsif exists((select 1 from global_settings where key='isVerifyActionEnabled' and value='1'))and upper(v_ticket_status)!='INPROGRESS'
then
NWEntityStatus:=NWEntityStatus||' and tsm.status in (''N'',''V'')';
elsif exists((select 1 from global_settings where key='isVerifyActionEnabled' and value='0'))
and (upper(v_ticket_status)='COMPLETED')
then
NWEntityStatus:=NWEntityStatus||' and tsm.status in (''A'',''R'',''S'')';
elsif exists((select 1 from global_settings where key='isVerifyActionEnabled' and value='0'))and upper(v_ticket_status)='REJECTED'
then
NWEntityStatus:=NWEntityStatus||' and tsm.status in (''A'',''R'',''S'',''NEW'',''MFD'',''DMT'')';
elsif exists((select 1 from global_settings where key='isVerifyActionEnabled' and value='0'))and upper(v_ticket_status)='APPROVED'
then
NWEntityStatus:=NWEntityStatus||' and tsm.status in (''A'',''S'',''NEW'',''MFD'',''DMT'')';
end if;
NWEntityStatus:=NWEntityStatus||' group by tsm.description,nt.entity_status,tsm.color_code,tsm.column_sequence,tsm.opacity order by tsm.column_sequence';

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

--sql:= sql||' order by created_on desc';

RETURN QUERY
EXECUTE 'select row_to_json(row) from (
select(
select array_to_json(array_agg(row_to_json(x)))
from (
'||NWEntityStatus||'
) x
) as lstNWEntityStatus,
(
select array_to_json(array_agg(row_to_json(x)))
from('||sql ||')x

) as lstNWEntityDetails,'||v_isAuthorizedToApprove||' as isAuthorizedToApprove
) row';

END ;

$BODY$;

ALTER FUNCTION public.fn_nwt_get_ticket_entity_details(integer, integer, integer, integer, character varying, character varying, character varying)
    OWNER TO postgres;


