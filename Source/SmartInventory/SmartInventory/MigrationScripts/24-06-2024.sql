DROP VIEW public.vw_att_details_networkticket_map;

CREATE OR REPLACE VIEW public.vw_att_details_networkticket_map
 AS
 SELECT nwtckt.ticket_id AS system_id,
    nwtckt.region_id,
    nwtckt.province_id,
    nwtckt.status,
        CASE
            WHEN nwtckt.ticket_status_id = 8 THEN false
            ELSE true
        END AS is_new_entity,
    COALESCE(adi.updated_geom, plgn.sp_geometry) AS sp_geometry,
    nwtckt.name,
    nwtckt.network_id,
    nwtckt.ticket_id::character varying AS source_ref_id,
    'Network_Ticket'::character varying AS source_ref_type,
    nwtckt.name AS label_column
   FROM polygon_master plgn
     JOIN att_details_networktickets nwtckt ON plgn.system_id = nwtckt.ticket_id AND upper(plgn.entity_type::text) = 'NETWORK_TICKET'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = nwtckt.ticket_id AND upper(adi.entity_type::text) = upper('NETWORK_TICKET'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text where nwtckt.ticket_status_id != 8;

ALTER TABLE public.vw_att_details_networkticket_map
    OWNER TO postgres;
	
	
	
	
DROP VIEW public.vw_att_details_network_ticket_vector;

CREATE OR REPLACE VIEW public.vw_att_details_network_ticket_vector
 AS
 SELECT adn.ticket_id AS system_id,
    adn.name,
    COALESCE(plgn.sp_geometry, pb.sp_geometry) AS sp_geometry,
    adn.region_id,
    adn.province_id,
    adn.status,
    adn.network_id,
        CASE
            WHEN adn.ticket_status_id = 8 THEN false
            ELSE true
        END AS is_new_entity,
    adn.ticket_id::character varying AS source_ref_id,
    'Network_Ticket'::character varying AS source_ref_type,
        CASE
            WHEN COALESCE(plgn.display_name, ''::character varying)::text = ''::text THEN adn.network_id
            ELSE plgn.display_name
        END AS display_name
   FROM att_details_networktickets adn
     LEFT JOIN polygon_master plgn ON plgn.system_id = adn.ticket_id AND plgn.entity_type::text = 'Network_Ticket'::text
     LEFT JOIN province_boundary pb ON pb.id = adn.province_id
     where adn.ticket_status_id != 8;

ALTER TABLE public.vw_att_details_network_ticket_vector
    OWNER TO postgres;




CREATE OR REPLACE FUNCTION public.fn_nwt_submit_entities(
	p_ticket_id integer,
	p_entity_ids_types character varying,
	p_remarks character varying,
	p_source character varying,
	p_userid integer,
	p_status character varying)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
v_table_name text;
v_message text;
v_status bool;
v_id_type record;
curidstypes refcursor;
v_new_entity_status character varying;
v_new_ticket_status_id integer;
v_ticket_update_value integer;
v_is_ticket_entities_updated integer;
v_existing_check integer;
   result_entities text;
   result_status_entities bool;
v_arow record;
v_total_entity integer;
v_status_entity integer;
v_sql character varying;
v_ticket_status character varying;
BEGIN
  v_message := 'process failed';
  v_status:=false;
  v_ticket_update_value := 0;
  v_is_ticket_entities_updated := 0;
  v_existing_check := 0;
  v_sql:='';
v_ticket_status:='';
 select tsm.name into v_ticket_status from ticket_status_master tsm
inner join att_details_networktickets att on tsm.id=att.ticket_status_id where ticket_id=p_ticket_id;

 create temp table temp_nt_entity_detalis
 (
 id integer,
 entity_type character varying(50),
 status character varying(50)
 )on commit drop;
create temp table temp_all_entity
(
id integer,
entity_type character varying(50),
status character varying(50)
)on commit drop;
v_new_entity_status := p_status;
IF(UPPER(P_SOURCE) = 'MOBILE')
THEN
	v_new_entity_status := 'S';
	select id into v_new_ticket_status_id from ticket_status_master where upper(module) = 'NETWORK_TICKET' and upper(name) = 'COMPLETED';
	if exists(select 1 from global_settings where key='isVerifyActionEnabled' and value='1')
	then 
		v_new_entity_status := 'N';
	end if;	
	update att_details_networktickets set ticket_status_id = v_new_ticket_status_id,modified_by = p_userid,modified_on = now() where ticket_id = p_ticket_id;
	v_message := 'Network has been submitted successfully.';
	v_status :=true;
ELSE IF(UPPER(P_SOURCE) = 'WEB' AND COALESCE(P_ENTITY_IDS_TYPES,'') = '')
THEN
	v_new_entity_status := p_status;
	select id into v_new_ticket_status_id from ticket_status_master where upper(module) = 'NETWORK_TICKET' and upper(name) = 'VERIFIED';
	v_message := 'Network has been verified successfully.';
	v_status :=true;	
END IF;
END IF;

IF(P_ENTITY_IDS_TYPES = '')
THEN
	INSERT INTO TEMP_NT_ENTITY_DETALIS(id,entity_type,status)
	SELECT a.system_id,a.entity_type,a.status FROM FN_NWT_GET_ENTITY_LIST(P_TICKET_ID) a;
ELSE
	INSERT INTO TEMP_NT_ENTITY_DETALIS(id,entity_type)
	SELECT SPLIT_PART(REGEXP_SPLIT_TO_TABLE(P_ENTITY_IDS_TYPES, ','), '~', 1)::INTEGER,SPLIT_PART(REGEXP_SPLIT_TO_TABLE(P_ENTITY_IDS_TYPES, ','), '~', 2);
END IF;

IF(UPPER(P_SOURCE) = 'MOBILE' and UPPER(v_ticket_status)='REJECTED')
then
v_message := 'Selected entities has been submitted successfully.';
delete from TEMP_NT_ENTITY_DETALIS tmp where tmp.status!='D';
elsif(coalesce(p_status,'')!='')
then
v_message := 'Selected entities has been '||fn_get_entity_status(p_status)||' successfully.';
END IF;

FOR V_AROW IN SELECT * FROM TEMP_NT_ENTITY_DETALIS
LOOP
	 
if(coalesce(v_arow.id,0) != 0)
then
raise info 'v_arow.id%'	,v_arow.id;
	
	select layer_table into v_table_name from layer_details where upper(layer_name)=upper(v_arow.entity_type);
	raise info 'v_table_name%',v_table_name;
	v_sql:= 'update '||v_table_name||' set status='''||v_new_entity_status||''',status_remark = '''||p_remarks||''',status_updated_by = '||p_userid||', status_updated_on = now() where source_ref_id::integer = '||p_ticket_id||' and system_id='||v_arow.id||'';
	execute v_sql; 
	PERFORM FN_GEOJSON_UPDATE_ENTITY_ATTRIBUTE(v_arow.id, v_arow.entity_type,0,1,FALSE);
end if;
	
v_status :=true;
END LOOP;

insert into temp_all_entity(id,entity_type,status)
select a.system_id,a.entity_type,a.status from fn_nwt_get_entity_list(p_ticket_id) a;

select count(1) into v_total_entity from temp_all_entity;

if exists(select 1 from temp_all_entity a where upper(a.status) in('R'))
then	
	update att_details_networktickets set ticket_status_id = (select id from ticket_status_master where upper(module) = 'NETWORK_TICKET' 
	and upper(name) = 'REJECTED'),modified_by = p_userid,modified_on = now()
	where ticket_id = p_ticket_id;
	v_message := 'Network has been rejected successfully.';
	v_status :=true;
elsif not exists(select 1 from temp_all_entity a where upper(a.status) in('N','S','R'))
then	
	update att_details_networktickets set ticket_status_id = (select id from ticket_status_master where upper(module) = 'NETWORK_TICKET' 
	and upper(name) = (case when p_status='A' then 'APPROVED' when p_status='V' then 'VERIFIED' end)),modified_by = p_userid,modified_on = now()
	where ticket_id = p_ticket_id;

	if(coalesce(p_status,'')!='')
	then
		v_message := 'Network has been '||fn_get_entity_status(p_status)||' successfully.';
	end if;
	
	FOR V_AROW IN SELECT * FROM TEMP_NT_ENTITY_DETALIS
	LOOP		 
	if(coalesce(v_arow.id,0) != 0)
	then	
		select layer_table into v_table_name from layer_details where upper(layer_name)=upper(v_arow.entity_type);
		v_sql:= 'update '||v_table_name||' set is_visible_on_map=true,is_new_entity=false where source_ref_id::integer = '||p_ticket_id||' and system_id='||v_arow.id||'';
		execute v_sql; 
		PERFORM FN_GEOJSON_UPDATE_ENTITY_ATTRIBUTE(v_arow.id, v_arow.entity_type,0,1,FALSE);
		--update att_details_edit_entity_info set updated_geom=null where source_ref_id::integer = p_ticket_id and entity_system_id=v_arow.id and upper(entity_type)=upper(v_arow.entity_type);
	end if;
	END LOOP;
	v_status :=true;
end if;
select * into result_status_entities,result_entities from fn_nwt_submit_entity_operations_details(p_ticket_id,p_entity_ids_types,p_userid);

-- IF(V_TICKET_UPDATE_VALUE = 1)
-- THEN
-- 	update att_details_networktickets set ticket_status_id = v_new_ticket_status_id,modified_by = p_userid,modified_on = now() where ticket_id = p_ticket_id;
-- ELSE

-- FOR V_AROW IN  SELECT * FROM TEMP_NT_ENTITY_DETALIS
-- LOOP		    
-- 	select layer_table into v_table_name from layer_details where upper(layer_name)=upper(v_arow.entity_type);
-- 	v_existing_check := 0;
-- 	execute 'select 1 from '||v_table_name||' where system_id = '||v_arow.id||' and upper(status) = ''V''' into v_existing_check;
-- 
-- 	IF(V_EXISTING_CHECK = 1)
-- 	THEN
-- 		V_IS_TICKET_ENTITIES_UPDATED := 1;
-- 	ELSE
-- 		V_IS_TICKET_ENTITIES_UPDATED := 0;
-- 	END IF;
-- END LOOP;
-- 	if(v_is_ticket_entities_updated = 1)
-- 	then
-- 		update att_details_networktickets set ticket_status_id = (select id from ticket_status_master where upper(module) = 'NETWORK_TICKET' and upper(name) = 'VERIFIED'),modified_by = p_userid,modified_on = now() where ticket_id = p_ticket_id;
-- 	end if;
-- end if;

--RAISE INFO 'final_ : %', v_is_ticket_entities_updated;

RETURN QUERY
         select v_status as status, v_message::character varying as message;
END; 
$BODY$;

ALTER FUNCTION public.fn_nwt_submit_entities(integer, character varying, character varying, character varying, integer, character varying)
    OWNER TO postgres;
	
	
	
	
CREATE OR REPLACE FUNCTION public.fn_nwt_get_ticket_details(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_userid integer,
	p_searchfrom timestamp without time zone,
	p_searchto timestamp without time zone,
	p_ticket_type_id integer,
	p_region_id integer,
	p_province_id integer,
	p_ticket_status_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

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
,* from vw_att_details_networktickets nt
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
$BODY$;

ALTER FUNCTION public.fn_nwt_get_ticket_details(character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone, integer, integer, integer, integer)
    OWNER TO postgres;
	
	
	
truncate table network_ticket_geojson_master;
INSERT INTO network_ticket_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Network_Ticket',
			'layer_title','NW Ticket Outline',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_network_ticket_vector
		) ROW;	