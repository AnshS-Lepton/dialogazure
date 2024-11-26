-- public.vw_att_details_row_report source

CREATE OR REPLACE VIEW public.vw_att_details_row_report
AS SELECT enrow.system_id,
    enrow.network_id,
    enrow.row_type,
    enrow.row_name,
    reg.region_name,
    prov.province_name,
    enrow.parent_network_id AS parent_code,
    enrow.parent_entity_type AS parent_type,
    enrow.remarks,
    fn_get_date(enrow.created_on) AS created_on,
    fn_get_date(enrow.modified_on) AS modified_on,
    um.user_name AS created_by,
    um2.user_name AS modified_by,
    enrow.region_id,
    enrow.province_id,
    enrow.row_stage,
    poly.buffer_width AS row_width,
    st_length(poly.center_line_geom::geography, false)::numeric(30,2) AS row_length,
    (poly.buffer_width * st_length(poly.center_line_geom::geography, false))::numeric(30,2) AS row_area,
    _row.road_name,
    _row.radius,
    _row.route_length,
    _row.supervision,
    _row.no_of_hh,
    _row.ri_rate::numeric(30,2) AS ri_rate,
    _row.ri_amount::numeric(30,2) AS ri_amount,
    _row.local_management::numeric(30,2) AS local_management,
    _row.dit_liaisoning::numeric(30,2) AS dit_liaisoning,
    _row.row_liaisoning::numeric(30,2) AS row_liaisoning,
    _row.otm_liasoning::numeric(30,2) AS otm_liasoning,
    _row.autocad_drawing::numeric(30,2) AS autocad_drawing,
    _row.total_amount AS applied_total_amount,
    _row.start_point,
    _row.end_point,
    _row.cl_rly_fly_brg_laisioning::numeric(30,2) AS cl_rly_fly_brg_laisioning,
    _row.cl_ft_brg_laisioning::numeric(30,2) AS cl_ft_brg_laisioning,
    _row.refundable_amount::numeric(30,2) AS refundable_amount,
    _row.non_refundable_amount::numeric(30,2) AS non_refundable_amount,
    _row.mcgm_ward,
    _row.applied_pit,
    _row.pit_charges,
    _row.access_charges::numeric(30,2) AS access_charges,
    _row.considered_dlp_factor::numeric(30,2) AS considered_dlp_factor,
    _row.starta_type,
    um3.user_name AS applied_by,
    fn_get_date(_row.apply_date, false) AS apply_date,
        CASE
            WHEN upper(enrow.row_type::text) = upper('PIT'::text) THEN _row.row_area
            ELSE NULL::double precision
        END AS total_pit_area,
    _row.budget_material_cost::numeric(30,2) AS budget_material_cost,
    _row.budget_contractor_cost::numeric(30,2) AS budget_contractor_cost,
    _row.no_of_pulled_cables,
    _row.cable_pulling_rate,
    _row.bdotm_otm_trenching_ducting_rate,
    _row.bdotm_otm_trenching_ducting_length,
    _row.bdotm_otm_ri_rate,
    _row.bdotm_otm_ri_length,
    _row.bdotm_otm_liasioning_rate,
    _row.bdotm_otm_liasioning_length,
    _row.bdotl_otl_trenching_ducting_rate,
    _row.bdotl_otl_trenching_ducting_length,
    _row.bdotl_otl_ri_rate,
    _row.bdotl_otl_ri_length,
    _row.cl_rly_fly_brg_length,
    _row.cl_ft_brg_length,
    _row.handhole_rate,
    row_stage.demand_note,
    row_stage.row_status,
    row_stage.actual_row_route::numeric(30,2) AS actual_row_route,
    row_stage.dlp_factor::numeric(30,2) AS dlp_factor,
    row_stage.actual_rate::numeric(30,2) AS actual_rate,
    row_stage.actual_deposit_amount::numeric(30,2) AS actual_deposit_amount,
    row_stage.actual_ri_amount::numeric(30,2) AS actual_ri_amount,
    row_stage.actual_access_charge_amount::numeric(30,2) AS actual_access_charge_amount,
    row_stage.actual_refundable_amount::numeric(30,2) AS actual_refundable_amount,
    row_stage.penalty_amount::numeric(30,2) AS penalty_amount,
    row_stage.total_paid_amount::numeric(30,2) AS total_paid_amount,
    fn_get_date(row_stage.payment_date, false) AS payment_date,
    fn_get_date(row_stage.start_date, false) AS start_date,
    fn_get_date(row_stage.end_date, false) AS end_date,
    fn_get_date(row_stage.access_charges_start_date, false) AS access_charges_start_date,
    fn_row_get_access_charge_due_date(row_stage.row_system_id) AS recurring_access_charge_due_date,
    date_part('year'::text, age(now(), row_stage.access_charges_start_date::timestamp with time zone)) * row_stage.actual_access_charge_amount AS total_recurring_charge,
        CASE
            WHEN (EXISTS ( SELECT 1
               FROM att_details_row_associate_entity_info ass
              WHERE ass.parent_system_id = enrow.system_id)) THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_entity_associated,
    um.user_id AS created_by_id,
    pm.project_name,
    pm.project_code,
    plningm.planning_name,
    plningm.planning_code,
    workorm.workorder_name,
    workorm.workorder_code,
    purposem.purpose_name,
    purposem.purpose_code,
    enrow.status_remark,
    enrow.status_updated_by,
    fn_get_date(enrow.status_updated_on) AS status_updated_on,
    enrow.is_visible_on_map,
    reg.country_name,
    enrow.origin_from,
    enrow.origin_ref_id,
    enrow.origin_ref_code,
    enrow.origin_ref_description,
    enrow.request_ref_id,
    enrow.requested_by,
    enrow.request_approved_by,
    enrow.gis_design_id AS design_id,
    enrow.ne_id,
    enrow.prms_id,
    enrow.jc_id,
    enrow.mzone_id,
    enrow.served_by_ring
   FROM att_details_row enrow
     JOIN province_boundary prov ON enrow.province_id = prov.id
     JOIN region_boundary reg ON enrow.region_id = reg.id
     LEFT JOIN user_master um ON enrow.created_by = um.user_id
     LEFT JOIN user_master um2 ON enrow.modified_by = um2.user_id
     LEFT JOIN att_details_row_apply _row ON _row.row_system_id = enrow.system_id
     LEFT JOIN user_master um3 ON _row.created_by = um3.user_id
     LEFT JOIN att_details_row_approve_reject row_stage ON row_stage.row_system_id = enrow.system_id
     LEFT JOIN user_master um4 ON row_stage.created_by = um4.user_id
     LEFT JOIN polygon_master poly ON poly.system_id = enrow.system_id AND upper(poly.entity_type::text) = upper('ROW'::text)
     LEFT JOIN att_details_project_master pm ON enrow.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON enrow.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON enrow.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON enrow.purpose_id = purposem.system_id
     LEFT JOIN att_details_csa csa ON csa.system_id = enrow.system_id;
	 
-------------------
UPDATE public.res_resources SET value='kann nicht in Dormant umgewandelt werden, da es beim Spleißen verwendet wird' WHERE culture='de-DE' AND "key"='SI_GBL_GBL_GBL_GBL_141';
UPDATE public.res_resources SET value='can not convert into dormant,as it is used in splicing' WHERE culture='en' AND "key"='SI_GBL_GBL_GBL_GBL_141';
UPDATE public.res_resources SET value='can not convert into dormant,as it is used in splicing' WHERE culture='ja-JP' AND "key"='SI_GBL_GBL_GBL_GBL_141';

UPDATE public.res_resources SET value='kann nicht in „dormant“ umgewandelt werden, da es mit einer Entität verknüpft ist!' WHERE culture='de-DE' AND "key"='SI_GBL_GBL_GBL_GBL_142';
UPDATE public.res_resources SET value='can not convert into dormant,as it is associated with some entity!' WHERE culture='en' AND "key"='SI_GBL_GBL_GBL_GBL_142';
UPDATE public.res_resources SET value='can not convert into dormant,as it is associated with some entity!' WHERE culture='ja-JP' AND "key"='SI_GBL_GBL_GBL_GBL_142';

------------------
update global_settings set value='Ad+cluHdxNN0VdcMn2gxd2gbPROLlmtgg5IhSewtKC8rmTI/BSeTsCXG/U0HuC34E/rhcry8NmAhRicezJTZ1F046UJX1OqDbU1s/e5I72vk0Q+upbaLNXIuX6xOHwfXnAcfl7ZCiA3PqiiFf6hGWuNlIZN2tNip5b6vPh+BGEbY7hZHTWJMBMf7wEBq1u5I21HQDZrG3HHsYlAQl2IfUZHARx3d134aI/ujV0PXPcirI7ntVdbW/yEXzXVNfXXjLcrjYemiPTHvW6zxuBi7hZSvA09KtYMoxcdnZaCa6rV3pErUsF5U8G5ZTdnFZyHhHj0msbZmlRBQI+lyI8/Yug==' where key ilike 'product_license_%'

---------------------------------------------
-- public.vw_att_details_spliceclosure_report source

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_report
AS SELECT sc.system_id,
    sc.network_id,
    sc.spliceclosure_name,
    round(sc.latitude::numeric, 6) AS latitude,
    round(sc.longitude::numeric, 6) AS longitude,
    sc.address,
    sc.specification,
    sc.category,
    sc.subcategory1,
    sc.subcategory2,
    sc.subcategory3,
    sc.item_code,
        CASE
            WHEN sc.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN sc.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN sc.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, sc.status) AS status,
    sc.pincode,
    prov.province_name,
    reg.region_name,
    sc.parent_network_id AS parent_code,
    sc.parent_entity_type AS parent_type,
    vm.name AS vendor_name,
    sc.no_of_ports,
    sc.no_of_input_port,
    sc.no_of_output_port,
    sc.acquire_from,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    to_char(sc.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(sc.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    sc.region_id,
    sc.province_id,
        CASE
            WHEN sc.is_virtual = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_virtual,
        CASE
            WHEN sc.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    sc.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    sc.utilization,
        CASE
            WHEN sc.utilization::text = 'L'::text THEN 'Low'::text
            WHEN sc.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN sc.utilization::text = 'H'::text THEN 'High'::text
            WHEN sc.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
        CASE
            WHEN sc.is_buried = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_buried,
    sc.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    sc.source_ref_type,
    sc.source_ref_id,
    sc.source_ref_description,
    sc.status_remark,
    sc.status_updated_by,
    to_char(sc.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    sc.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    sc.remarks,
    reg.country_name,
    sc.origin_from,
    sc.origin_ref_id,
    sc.origin_ref_code,
    sc.origin_ref_description,
    sc.request_ref_id,
    sc.requested_by,
    sc.request_approved_by,
    sc.subarea_id,
    sc.area_id,
    sc.dsa_id,
    sc.csa_id,
    sc.bom_sub_category,
    sc.gis_design_id AS design_id,
    sc.st_x,
    sc.st_y,
    sc.ne_id,
    sc.prms_id,
    sc.jc_id,
    sc.mzone_id,
    sc.served_by_ring,
    sc.hierarchy_type,
    sc.section_name,
    sc.generic_section_name,
    sc.aerial_location,
    sc.route_id,
    vm3.name AS own_vendor,
    sc.spliceclosure_type
   FROM att_details_spliceclosure sc
     JOIN province_boundary prov ON sc.province_id = prov.id
     JOIN region_boundary reg ON sc.region_id = reg.id
     JOIN user_master um ON sc.created_by = um.user_id
     JOIN vendor_master vm ON sc.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sc.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON sc.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON sc.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON sc.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON sc.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON sc.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON sc.modified_by = um2.user_id
     LEFT JOIN entity_notification_status entnotifystatus ON sc.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON sc.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON sc.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = sc.status::text;
	 
----------------------------------------
select * from fn_sync_layer_columns()
--------------------------------------------------
-- DROP FUNCTION public.fn_get_association_dropdownlist(varchar, varchar);

CREATE OR REPLACE FUNCTION public.fn_get_association_dropdownlist(entitytype character varying, dropdowntype character varying)
 RETURNS TABLE(dropdown_type character varying, dropdown_value character varying, dropdown_key character varying)
 LANGUAGE plpgsql
AS $function$

 DECLARE
    sql TEXT;
BEGIN

IF(entitytype != '' and dropdownType != '' and dropdownType ='splitter_ratio' ) THEN

	sql:= 'select dropdown_type,dropdown_value,dropdown_key from (select SPLIT_PART(dd.dropdown_value,'':'', 1)::integer as input,SPLIT_PART(dd.dropdown_value,'':'', 2)::integer as output, dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id where UPPER(l.layer_name)= upper('''||entitytype||''') 
	and UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
	and lower(dd.dropdown_type) not like ''remark_%'' order by dd.dropdown_value)t order by t.input,t.output' ;

elsIF(entitytype != '' and dropdownType != '' and upper(entitytype) !=upper('entity_type')) THEN

	sql:= 'select dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id where UPPER(l.layer_name)= upper('''||entitytype||''') and UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
	and lower(dd.dropdown_type) not like ''remark_%'' and dd.is_active=true order by dd.id';

	
elsIF(entitytype != '' and dropdownType != '' and upper(entitytype) !=upper('landbase')) THEN

	sql:= 'select dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id where UPPER(l.layer_name)= upper('''||entitytype||''') and UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
	and lower(dd.dropdown_type) not like ''remark_%'' order by dd.id';

elsIF(entitytype != '' and upper(entitytype) !=upper('landbase')) THEN  
     -- sql:= 'SELECT dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id
--       WHERE UPPER(l.layer_name)= upper('''||entitytype||''') and lower(dd.dropdown_type) not like ''remark_%'' order by dd.id';
      sql:= 'SELECT dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id
      WHERE UPPER(l.layer_name)= upper('''||entitytype||''') and lower(dd.dropdown_type) not like ''remark_%'' order by dd.id';

elsIF(dropdownType != '' and upper(entitytype) !=upper('landbase')) THEN

 sql:= 'select dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd where UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
  and lower(dd.dropdown_type) not like ''remark_%'' and layer_id=(select coalesce((select COALESCE(layer_id,0) from layer_details where Upper(layer_name)=upper('''||entitytype||''')),0)) and dd.is_active=true order by dd.id';

 elseIF(upper(entitytype)=upper('landbase'))THEN
 sql:='SELECT dd.type as dropdown_type,dd.value as dropdown_value,dd.value as dropdown_key FROM landbase_dropdown_master dd
	UNION
	SELECT  ''Layer Type'' as dropdown_type, dd.layer_name as dropdown_value,dd.layer_name as dropdown_key  FROM landbase_layer_master dd  ' ;

elseIF(coalesce(entitytype,'')='') THEN
sql:= 'SELECT dropdown_type,dropdown_value,dropdown_key FROM dropdown_master where layer_id=0 and is_active=true';
end if; 
raise info 'sql:%',sql;

RETURN QUERY EXECUTE sql;	
  END; 
$function$
;
---------------------------------------------------
-- public.vw_att_details_splitter_report source

CREATE OR REPLACE VIEW public.vw_att_details_splitter_report
AS SELECT splt.system_id,
    splt.network_id,
    splt.splitter_name,
    round(splt.latitude::numeric, 6) AS latitude,
    round(splt.longitude::numeric, 6) AS longitude,
    splt.address,
    splt.splitter_ratio,
    splt.specification,
    splt.category,
    splt.subcategory1,
    splt.subcategory2,
    splt.subcategory3,
    splt.item_code,
    splt.acquire_from,
        CASE
            WHEN splt.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN splt.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN splt.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.status, splt.status) AS status,
    prov.province_name,
    reg.region_name,
    splt.parent_network_id AS parent_code,
    ld.layer_title AS parent_type,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    splt.splitter_type,
    to_char(splt.created_on, 'DD-Mon-YY'::text) AS created_on,
    um.user_name AS created_by,
    to_char(splt.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    splt.region_id,
    splt.province_id,
    splt.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    splt.utilization,
        CASE
            WHEN splt.utilization::text = 'L'::text THEN 'Low'::text
            WHEN splt.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN splt.utilization::text = 'H'::text THEN 'High'::text
            WHEN splt.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    splt.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    splt.source_ref_type,
    splt.source_ref_id,
    splt.source_ref_description,
    splt.status_remark,
    splt.status_updated_by,
    to_char(splt.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    splt.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    splt.remarks,
    reg.country_name,
    splt.origin_from,
    splt.origin_ref_id,
    splt.origin_ref_code,
    splt.origin_ref_description,
    splt.request_ref_id,
    splt.requested_by,
    splt.request_approved_by,
    splt.subarea_id,
    splt.area_id,
    splt.dsa_id,
    splt.csa_id,
    splt.bom_sub_category,
    splt.gis_design_id AS design_id,
    splt.st_x,
    splt.st_y,
    splt.ne_id,
    splt.prms_id,
    splt.jc_id,
    splt.mzone_id,
    splt.served_by_ring,
    c.rfs_status,
    splt.power_meter_reading,
    vm3.name AS own_vendor_name
   FROM att_details_splitter splt
     JOIN province_boundary prov ON splt.province_id = prov.id
     JOIN region_boundary reg ON splt.region_id = reg.id
     JOIN user_master um ON splt.created_by = um.user_id
     JOIN vendor_master vm ON splt.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON splt.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON splt.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON splt.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON splt.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON splt.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON splt.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON splt.modified_by = um2.user_id
     LEFT JOIN entity_notification_status entnotifystatus ON splt.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON splt.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON splt.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = splt.status::text
     LEFT JOIN layer_details ld ON ld.layer_name::text = splt.parent_entity_type::text
     LEFT JOIN att_details_csa c ON COALESCE(splt.csa_system_id, 0) > 0 AND splt.csa_system_id::text = c.system_id::text AND splt.splitter_type::text = 'Secondary'::text;
	 
	 --------------------------------------------------------------
	 update module_master set connection_id =null where module_abbr = ('SPLIT_EXRPT')
	 
	 -------------------------------------------------------------
	 select * from fn_sync_layer_columns()
	 
-------------------------------------Association Report Duration Based Column Data Created---------------------
INSERT INTO public.dropdown_master (layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id) VALUES(0, 'Association_Report', 'Modified_On', true, 1, Now(), NULL, Now(), 'Modified_On', NULL, 'Association_Report', false, true, 0);
INSERT INTO public.dropdown_master (layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id) VALUES(0, 'Association_Report', 'Created_On', true, 1, Now(), NULL, Now(), 'Created_On', NULL, 'Association_Report', false, true, 0);