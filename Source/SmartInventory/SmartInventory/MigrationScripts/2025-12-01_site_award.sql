
alter table att_details_networktickets add column site_id varchar NULL ;
alter table att_details_networktickets add column site_project_id varchar NULL ;
alter table att_details_networktickets add column site_name varchar NULL ;
alter table site_project_details add column status varchar NULL ;

----------------------------------------------------------------------------------------------------------

INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('WH24grantType', 'client_credentials', '', 'Web', false, 'character varying', 1.0, 1.0, 1, now(), NULL, NULL, false, true, false, NULL, NULL);

INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('WH24ClientSecret', 'HEm2-OYKyH0ImbqULDWY-G-Ld2Xq0qlVFwco7G8I5O7_oQiyBQNV5rZBTut2mxh0YpLG1Q54', '', 'Web', false, 'character varying', 1.0, 1.0, 1, now(), NULL, NULL, false, true, false, NULL, NULL);

INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('WH24ClientId', 'X4EQQRCL5ABA2HKR5XUEOXT6WDYT65OEESOOU5O2', '', 'Web', false, 'character varying', 1.0, 1.0, 1, now(), NULL, NULL, false, true, false, NULL, NULL);

INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('WH24AuthBaseURL', 'https://dialog.workhub24.com/', '','Web', false, 'character varying', 1.0, 1.0, 1, now(), NULL, NULL, false, true, false, NULL, NULL);


INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('WH24URL', 'https://dialog.workhub24.com/api/workflows/G66I72NENBAI24D3BHRZCU6PYSHPQ57U/we7ccab4204/cards', '','Web', false, 'character varying', 1.0, 1.0, 1, now(), NULL, NULL, false, true, false, null,null);


INSERT INTO public.module_master
(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES('Site Awarding', 'Site Awarding', NULL, NULL, 1, now(), NULL, NULL, 'Web', true, 'SAD', 0, 18, true, NULL, NULL);
------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_imported_site_project_details(p_page_no integer, p_page_size integer)
 RETURNS json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_result JSON;
BEGIN
    SELECT json_build_object(
        'totalRecord', (SELECT COUNT(DISTINCT ps.site_id) FROM site_project_details ps join att_details_pod pod on pod.site_id = ps.site_id),
        'records', json_agg(row_to_json(r))
    )
    INTO v_result
    FROM (
         SELECT DISTINCT ON ( ps.site_id)
         ps.id,
               ps.site_id,
               ps.site_name,
                ps.created_on,
                ps.project_id,
                ps.status,
                pod.latitude ||',' || pod.longitude as site_geom
        FROM site_project_details ps
        join att_details_pod pod on pod.site_id = ps.site_id 
        LIMIT p_page_size OFFSET (p_page_no - 1) * p_page_size
    ) r;

    RETURN v_result;
END;
$function$
;

-- Permissions

-------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_nwt_insert_update_ticket(p_ticket_id integer, p_ticket_type_id integer, p_reference_type character varying, p_reference_description character varying, p_regionid integer, p_provinceid integer, p_network_id character varying, p_name character varying, p_assigned_to integer, p_target_date character varying, p_network_status character varying, p_remarks character varying, p_created_by integer, p_geom character varying, p_reference_ticket_id character varying, p_project_code character varying, p_account_code character varying, p_project_id character varying)
 RETURNS SETOF character varying
 LANGUAGE plpgsql
AS $function$


declare
 v_system_id   integer;
 v_network_id character varying;
 v_geom_query geometry;
 p_site_id character varying;
 p_site_name character varying;
BEGIN
 IF EXISTS (SELECT 1 FROM att_details_networktickets  WHERE  ticket_id=p_ticket_id) then
begin
	
	select site_id ,site_name into p_site_id,p_site_name from site_project_details spd where project_id = p_project_id;

   Update att_details_networktickets set ticket_type_id=p_ticket_type_id,reference_type=p_reference_type,region_id=p_regionId,province_id=p_provinceId,name=p_name,assigned_to=p_assigned_to,
   target_date=p_target_date::date,for_network_type=p_network_status,remarks=p_remarks,reference_description=p_reference_description,modified_by=p_created_by,modified_on=now(),
   reference_ticket_id=p_reference_ticket_id,project_code=p_project_code,account_code=p_account_code,site_project_id = p_project_id,site_id = p_site_id,site_name =p_site_id where ticket_id=p_ticket_id and network_id=p_network_id;

  update site_project_details set status = 'Assigned' where project_id = p_project_id;  
 
  return query  select 'Update'::character varying as msg;
END;
 ELSE 
 	select site_id ,site_name into p_site_id,p_site_name from site_project_details spd where project_id = p_project_id;
  update site_project_details set status = 'Assigned' where project_id = p_project_id;  
 
BEGIN
select * from fn_nwt_get_network_id(p_ticket_type_id,p_provinceId)into v_network_id;
--ticket_status_id 4 is Name "Assigned" module "NetworkTicket";
  insert into att_details_networktickets (ticket_type_id,reference_type,region_id,province_id,network_id,name,assigned_to,target_date,for_network_type,
  remarks,reference_description,created_by,created_on,ticket_status_id,reference_ticket_id,project_code,account_code,site_project_id,site_id,site_name) 
  values(p_ticket_type_id,p_reference_type,p_regionId,p_provinceId,v_network_id,p_name,p_assigned_to,p_target_date::date,p_network_status,p_remarks,
  p_reference_description,p_created_by,now(),4,p_reference_ticket_id,p_project_code,p_account_code,p_project_id,p_site_id,p_site_name);
  select ticket_id into v_system_id from att_details_networktickets where network_id=v_network_id;

IF(p_geom!='')Then
--select ticket_id into v_system_id from att_details_networktickets where network_id=v_network_id;
v_geom_query = ST_FORCE2D(ST_GEOMFROMTEXT('POLYGON(('||p_geom||'))',4326));
--v_geom_query = ST_Buffer(ST_FORCE2D(ST_GEOMFROMTEXT('POLYGON(('||p_geom||'))',4326)),10);
insert into polygon_master(system_id,entity_type,approval_flag,sp_geometry,created_by,common_name,approval_date,network_status,display_name)
values(v_system_id,'Network_Ticket','A',v_geom_query::geometry,p_created_by,v_network_id,now(),p_network_status,fn_get_display_name(v_system_id,'Network_Ticket'));

END IF;


 perform fn_geojson_update_entity_attribute(v_system_id, 'Network_Ticket',p_provinceId,0,false);
  return query  select 'Save'::character varying as msg;
END;
END IF;

END;
$function$
;

-- Permissions
