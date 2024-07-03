 DROP VIEW public.vw_att_details_trench_report;

CREATE OR REPLACE VIEW public.vw_att_details_trench_report
 AS
 SELECT trench.system_id,
    trench.network_id,
    trench.trench_name,
    trench.a_location,
    trench.a_entity_type,
    trench.b_location,
    trench.b_entity_type,
    round(trench.trench_length::numeric, 2) AS trench_length,
    round(trench.trench_width::numeric, 2) AS trench_width,
    round(trench.trench_height::numeric, 2) AS trench_height,
    trench.customer_count,
    trench.utilization,
    trench.trench_type,
    trench.no_of_ducts,
    fn_get_network_status(trench.network_status) AS network_status,
    COALESCE(fn_get_entity_status(trench.status), trench.status) AS status,
    trench.pin_code,
    trench.specification,
    trench.category,
    trench.subcategory1,
    trench.subcategory2,
    trench.subcategory3,
    trench.item_code,
    trench.remarks,
    trench.mcgm_ward,
    trench.strata_type,
    trench.acquire_from,
    fn_get_date(trench.manufacture_year, false) AS manufacture_year,
    trench.surface_type,
    trench.parent_entity_type AS parent_type,
    trench.parent_network_id AS parent_code,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    fn_get_date(trench.created_on) AS created_on,
    fn_get_date(trench.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    COALESCE(pmapping.province_id, trench.province_id) AS province_id,
    COALESCE(pmapping.region_id, trench.region_id) AS region_id,
        CASE
            WHEN trench.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    prov.province_name AS city_name,
    reg.region_name,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    trench.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    trench.source_ref_type,
    trench.source_ref_id,
    trench.source_ref_description,
    trench.status_remark,
    trench.status_updated_by,
    fn_get_date(trench.status_updated_on) AS status_updated_on,
    trench.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    reg.country_name,
    trench.origin_from,
    trench.origin_ref_id,
    trench.origin_ref_code,
    trench.origin_ref_description,
    trench.request_ref_id,
    trench.requested_by,
    trench.request_approved_by,
    trench.subarea_id,
    trench.area_id,
    trench.dsa_id,
    trench.csa_id,
    trench.bom_sub_category,
    trench.trench_serving_type,
    trench.gis_design_id AS design_id,
    trench.ne_id,
    trench.prms_id,
    trench.jc_id,
    trench.mzone_id,
    trench.served_by_ring,
        CASE
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"P"'::text THEN 'Planned'::text
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"A"'::text THEN 'Asbuilt'::text
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"D"'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS ticket_network_status,
    trench.a_latitude,
    trench.b_longitude,
    trench.a_region,
    trench.b_region,
    trench.a_city,
    trench.b_city,
    trench.actual_no_of_ducts,
    trench.hierarchy_type,
    vm3.name AS own_vendor_name
   FROM att_details_trench trench
     JOIN user_master um ON trench.created_by = um.user_id
     JOIN vendor_master vm ON trench.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON trench.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON trench.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON trench.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON trench.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON trench.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON trench.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON trench.modified_by = um2.user_id
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(trench.network_id::text) ~~ upper('NLD%'::text) THEN trench.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('TRENCH'::text)
            ELSE 1 = 2
        END
     JOIN province_boundary prov ON COALESCE(pmapping.province_id, trench.province_id) = prov.id
     JOIN region_boundary reg ON COALESCE(pmapping.region_id, trench.region_id) = reg.id
     LEFT JOIN att_details_pod primarypod ON trench.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON trench.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN att_details_edit_entity_info adi ON adi.source_ref_id::text = trench.source_ref_id::text AND adi.source_ref_type::text = trench.source_ref_type::text AND trench.system_id = adi.entity_system_id AND upper('TRENCH'::text) = upper(adi.entity_type::text) AND adi.entity_action_id = 13;




 DROP VIEW public.vw_att_details_fms_report;

CREATE OR REPLACE VIEW public.vw_att_details_fms_report
 AS
 SELECT fms.system_id,
    fms.network_id,
    fms.fms_name,
    round(fms.latitude::numeric, 6) AS latitude,
    round(fms.longitude::numeric, 6) AS longitude,
    fms.address,
    fms.specification,
    fms.category,
    fms.subcategory1,
    fms.subcategory2,
    fms.subcategory3,
    fms.item_code,
        CASE
            WHEN fms.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN fms.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN fms.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.status, fms.status) AS status,
    fms.pincode,
    prov.province_name,
    reg.region_name,
    fms.parent_network_id AS parent_code,
    ld.layer_title AS parent_type,
    vm.name AS vendor_name,
    fms.no_of_port,
    fms.no_of_input_port,
    fms.no_of_output_port,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    to_char(fms.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(fms.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    fms.region_id,
    fms.province_id,
    fms.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    fms.utilization,
    fms.acquire_from,
        CASE
            WHEN fms.utilization::text = 'L'::text THEN 'Low'::text
            WHEN fms.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN fms.utilization::text = 'H'::text THEN 'High'::text
            WHEN fms.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
    fms.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    fms.source_ref_type,
    fms.source_ref_id,
    fms.source_ref_description,
    fms.status_remark,
    fms.status_updated_by,
    to_char(fms.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    fms.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    fms.remarks,
    reg.country_name,
    fms.origin_from,
    fms.origin_ref_id,
    fms.origin_ref_code,
    fms.origin_ref_description,
    fms.request_ref_id,
    fms.requested_by,
    fms.request_approved_by,
    fms.subarea_id,
    fms.area_id,
    fms.dsa_id,
    fms.csa_id,
    fms.bom_sub_category,
    fms.gis_design_id AS design_id,
    fms.st_x,
    fms.st_y,
    fms.ne_id,
    fms.prms_id,
    fms.mzone_id,
    fms.jc_id,
    fms.served_by_ring,
    fms.hierarchy_type,
    vm3.name AS own_vendor_name
   FROM att_details_fms fms
     JOIN province_boundary prov ON fms.province_id = prov.id
     JOIN region_boundary reg ON fms.region_id = reg.id
     JOIN user_master um ON fms.created_by = um.user_id
     JOIN vendor_master vm ON fms.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON fms.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON fms.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON fms.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON fms.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON fms.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON fms.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON fms.modified_by = um2.user_id
     LEFT JOIN att_details_pod primarypod ON fms.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON fms.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = fms.status::text
     LEFT JOIN layer_details ld ON ld.layer_name::text = fms.parent_entity_type::text;
	 
	 
	 
	 
	 
	 
	  DROP VIEW public.vw_att_details_pod_report;

CREATE OR REPLACE VIEW public.vw_att_details_pod_report
 AS
 SELECT pod.system_id,
    pod.network_id,
    pod.pod_name,
    round(pod.latitude::numeric, 6) AS latitude,
    round(pod.longitude::numeric, 6) AS longitude,
    pod.pincode,
    pod.address,
    pod.specification,
    pod.category,
    pod.subcategory1,
    pod.subcategory2,
    pod.subcategory3,
    pod.item_code,
        CASE
            WHEN pod.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN pod.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN pod.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, pod.status) AS status,
    pod.parent_network_id AS parent_code,
    pod.parent_entity_type AS parent_type,
    prov.province_name,
    reg.region_name,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    to_char(pod.created_on, 'DD-Mon-YY'::text) AS created_on,
    um.user_name AS created_by,
    to_char(pod.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pod.region_id,
    pod.province_id,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    pod.acquire_from,
    pod.pod_type,
    pod.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    pod.source_ref_type,
    pod.source_ref_id,
    pod.source_ref_description,
    pod.status_remark,
    pod.status_updated_by,
    to_char(pod.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    entattr.erp_code,
    entattr.erp_name,
    entattr.ef_customers,
    entattr.status AS locked,
    entattr.zone,
    to_char(entattr.rfs_date, 'DD-Mon-YY'::text) AS rfs_date,
    entattr.hub_maintained,
    entattr.hm_power_bb,
    entattr.hm_earthing_rating,
    entattr.hm_rack,
    entattr.hm_olt_bb,
    entattr.hm_fms,
    entattr.splicing_machine,
    entattr.optical_power_meter,
    entattr.otdr,
    entattr.laptop_with_giga_port,
    entattr.l3_updation_on_inms,
    pod.is_visible_on_map,
    pod.remarks,
    reg.country_name,
    pod.origin_from,
    pod.origin_ref_id,
    pod.origin_ref_code,
    pod.origin_ref_description,
    pod.request_ref_id,
    pod.requested_by,
    pod.request_approved_by,
    pod.subarea_id,
    pod.area_id,
    pod.dsa_id,
    pod.csa_id,
    pod.bom_sub_category,
    pod.gis_design_id AS design_id,
    pod.st_x,
    pod.st_y,
    pod.ne_id,
    pod.prms_id,
    pod.jc_id,
    pod.mzone_id,
    pod.served_by_ring,
    pod.hierarchy_type,
    vm3.name AS own_vendor_name
   FROM att_details_pod pod
     JOIN province_boundary prov ON pod.province_id = prov.id
     JOIN region_boundary reg ON pod.region_id = reg.id
     JOIN user_master um ON pod.created_by = um.user_id
     JOIN vendor_master vm ON pod.vendor_id = vm.id
     LEFT JOIN entity_additional_attributes entattr ON entattr.system_id = pod.system_id AND upper(entattr.entity_type::text) = upper('POD'::text)
     LEFT JOIN vendor_master vm2 ON pod.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pod.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON pod.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON pod.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON pod.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON pod.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON pod.modified_by = um2.user_id
     LEFT JOIN entity_status_master es ON es.status::text = pod.status::text;
	 
	 
	 
	 
	  DROP VIEW public.vw_att_details_duct_report;

CREATE OR REPLACE VIEW public.vw_att_details_duct_report
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.duct_name,
    round(duct.calculated_length::numeric, 2) AS calculated_length,
    round(duct.manual_length::numeric, 2) AS manual_length,
    duct.a_location AS start_point_a,
    duct.b_location AS start_point_b,
    duct.a_entity_type,
    duct.b_entity_type,
    fn_get_network_status(duct.network_status) AS network_status,
    COALESCE(fn_get_entity_status(duct.status), duct.status) AS status,
    duct.pin_code,
    COALESCE(duct.utilization, '0'::character varying) AS utilization,
    duct.no_of_cables,
    duct.offset_value,
    duct.construction,
    duct.activation,
    duct.accessibility,
    duct.specification,
    duct.category,
    round(duct.inner_dimension::numeric, 2) AS inner_dimension,
    round(duct.outer_dimension::numeric, 2) AS outer_dimension,
    duct.duct_type,
    duct.color_code,
    duct.subcategory1,
    duct.subcategory2,
    duct.subcategory3,
    duct.item_code,
    duct.remarks,
    duct.parent_entity_type AS parent_type,
    duct.parent_network_id AS parent_code,
    duct.acquire_from,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_date(duct.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(duct.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    COALESCE(pmapping.province_id, duct.province_id) AS province_id,
    COALESCE(pmapping.region_id, duct.region_id) AS region_id,
        CASE
            WHEN duct.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    reg.region_name,
    prov.province_name,
    duct.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    duct.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    duct.source_ref_type,
    duct.source_ref_id,
    duct.source_ref_description,
    duct.status_remark,
    duct.status_updated_by,
    fn_get_date(duct.status_updated_on) AS status_updated_on,
    duct.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    vm.name AS vendor_name,
    reg.country_name,
    duct.duct_capacity,
    duct.origin_from,
    duct.origin_ref_id,
    duct.origin_ref_code,
    duct.origin_ref_description,
    duct.request_ref_id,
    duct.requested_by,
    duct.request_approved_by,
    duct.subarea_id,
    duct.area_id,
    duct.dsa_id,
    duct.csa_id,
    duct.bom_sub_category,
    duct.gis_design_id AS design_id,
    duct.ne_id,
    duct.prms_id,
    duct.jc_id,
    duct.mzone_id,
    duct.served_by_ring,
        CASE
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"P"'::text THEN 'Planned'::text
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"A"'::text THEN 'Asbuilt'::text
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"D"'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS ticket_network_status,
    duct.a_latitude,
    duct.b_longitude,
    duct.a_region,
    duct.b_region,
    duct.a_city,
    duct.b_city,
    duct.total_slack_count,
    duct.total_slack_length,
    duct.hierarchy_type,
    vm3.name AS own_vendor_name
   FROM att_details_duct duct
     JOIN user_master um ON duct.created_by = um.user_id
     JOIN vendor_master vm ON duct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON duct.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON duct.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON duct.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON duct.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON duct.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON duct.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON duct.purpose_id = purposem.system_id
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(duct.network_id::text) ~~ upper('NLD%'::text) THEN duct.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('DUCT'::text)
            ELSE 1 = 2
        END
     JOIN province_boundary prov ON COALESCE(pmapping.province_id, duct.province_id) = prov.id
     JOIN region_boundary reg ON COALESCE(pmapping.region_id, duct.region_id) = reg.id
     LEFT JOIN att_details_pod primarypod ON duct.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON duct.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN att_details_edit_entity_info adi ON adi.source_ref_id::text = duct.source_ref_id::text AND adi.source_ref_type::text = duct.source_ref_type::text AND duct.system_id = adi.entity_system_id AND upper('Duct'::text) = upper(adi.entity_type::text) AND adi.entity_action_id = 13;












drop view vw_att_details_trench;
CREATE OR REPLACE VIEW public.vw_att_details_trench
 AS
 SELECT trench.system_id,
    trench.network_id,
    trench.trench_name,
    trench.a_system_id,
    trench.a_location,
    trench.a_entity_type,
    trench.b_system_id,
    trench.b_location,
    trench.b_entity_type,
    round(trench.trench_length::numeric, 2) AS trench_length,
    round(trench.trench_width::numeric, 2) AS trench_width,
    round(trench.trench_height::numeric, 2) AS trench_height,
    trench.customer_count,
    trench.utilization,
    trench.trench_type,
    trench.no_of_ducts,
    trench.sequence_id,
    trench.network_status,
    trench.status,
    trench.pin_code,
    trench.province_id,
    trench.region_id,
    trench.construction,
    trench.activation,
    trench.accessibility,
    trench.specification,
    trench.category,
    trench.subcategory1,
    trench.subcategory2,
    trench.subcategory3,
    trench.item_code,
    trench.vendor_id,
    trench.type,
    trench.brand,
    trench.model,
    trench.remarks,
    trench.created_by,
    trench.created_on,
    trench.modified_by,
    trench.modified_on,
    trench.project_id,
    trench.planning_id,
    trench.purpose_id,
    trench.workorder_id,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    trench.mcgm_ward,
    trench.strata_type,
    trench.manufacture_year,
    trench.surface_type,
    trench.parent_system_id,
    trench.parent_entity_type,
    trench.parent_network_id,
    trench.acquire_from,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS trench_specification_type,
    bd.brand AS trench_brand,
    ml.model AS trench_model,
    fn_get_date(trench.created_on) AS created_date,
    fn_get_date(trench.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = trench.accessibility) AS trench_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = trench.construction) AS trench_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = trench.activation) AS trench_activation,
    trench.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    trench.source_ref_type,
    trench.source_ref_id,
    trench.source_ref_description,
    trench.status_remark,
    trench.status_updated_by,
    trench.status_updated_on,
    trench.is_visible_on_map,
    trench.primary_pod_system_id,
    trench.secondary_pod_system_id,
    trench.is_new_entity,
    trench.audit_item_master_id,
    trench.is_acquire_from,
    trench.origin_from,
    trench.origin_ref_id,
    trench.origin_ref_code,
    trench.origin_ref_description,
    trench.request_ref_id,
    trench.requested_by,
    trench.request_approved_by,
    trench.subarea_id,
    trench.area_id,
    trench.dsa_id,
    trench.csa_id,
    trench.bom_sub_category,
    trench.trench_serving_type,
    trench.gis_design_id,
    trench.ne_id,
    trench.prms_id,
    trench.jc_id,
    trench.mzone_id,
    trench.actual_no_of_ducts,
    trench.hierarchy_type,
    vm3.name AS own_vendor_name
   FROM att_details_trench trench
     JOIN province_boundary prov ON trench.province_id = prov.id
     JOIN region_boundary reg ON trench.region_id = reg.id
     JOIN user_master um ON trench.created_by = um.user_id
     JOIN vendor_master vm ON trench.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON trench.third_party_vendor_id = vm2.id
	 LEFT JOIN vendor_master vm3 ON trench.own_vendor_id::integer = vm3.id
     LEFT JOIN isp_type_master tp ON trench.type = tp.id
     LEFT JOIN isp_brand_master bd ON trench.brand = bd.id
     LEFT JOIN isp_base_model ml ON trench.model = ml.id
     LEFT JOIN user_master um2 ON trench.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON trench.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON trench.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON trench.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON trench.purpose_id = purposem.system_id;
	 
	 
	 
	 
	 
 DROP VIEW public.vw_att_details_duct;
CREATE OR REPLACE VIEW public.vw_att_details_duct
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.duct_name,
    round(duct.calculated_length::numeric, 2) AS calculated_length,
    round(duct.manual_length::numeric, 2) AS manual_length,
    duct.a_system_id,
    duct.a_location,
    duct.a_entity_type,
    duct.b_system_id,
    duct.b_location,
    duct.b_entity_type,
    duct.sequence_id,
    duct.network_status,
    duct.status,
    duct.pin_code,
    duct.province_id,
    duct.region_id,
    duct.utilization,
    duct.no_of_cables,
    duct.offset_value,
    duct.construction,
    duct.activation,
    duct.accessibility,
    duct.specification,
    duct.category,
    round(duct.inner_dimension::numeric, 2) AS inner_dimension,
    round(duct.outer_dimension::numeric, 2) AS outer_dimension,
    duct.duct_type,
    duct.color_code,
    duct.subcategory1,
    duct.subcategory2,
    duct.subcategory3,
    duct.item_code,
    duct.vendor_id,
    duct.type,
    duct.brand,
    duct.model,
    duct.remarks,
    duct.created_by,
    duct.created_on,
    duct.modified_by,
    duct.modified_on,
    duct.trench_id,
    duct.project_id,
    duct.planning_id,
    duct.purpose_id,
    duct.workorder_id,
    prov.province_name,
    duct.parent_system_id,
    duct.parent_entity_type,
    duct.parent_network_id,
    duct.acquire_from,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    bd.brand AS duct_brand,
    ml.model AS duct_model,
    fn_get_date(duct.created_on) AS created_date,
    fn_get_date(duct.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.accessibility) AS duct_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.construction) AS duct_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.activation) AS duct_activation,
    duct.barcode,
    duct.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    duct.source_ref_type,
    duct.source_ref_id,
    duct.source_ref_description,
    duct.status_remark,
    duct.status_updated_by,
    duct.status_updated_on,
    duct.is_visible_on_map,
    duct.primary_pod_system_id,
    duct.secondary_pod_system_id,
    duct.is_new_entity,
    duct.audit_item_master_id,
    duct.is_acquire_from,
    duct.duct_capacity,
    duct.origin_from,
    duct.origin_ref_id,
    duct.origin_ref_code,
    duct.origin_ref_description,
    duct.request_ref_id,
    duct.requested_by,
    duct.request_approved_by,
    duct.subarea_id,
    duct.area_id,
    duct.dsa_id,
    duct.csa_id,
    duct.bom_sub_category,
    duct.gis_design_id,
    duct.ne_id,
    duct.prms_id,
    duct.jc_id,
    duct.mzone_id,
    duct.duct_color,
    duct.own_vendor_id,
    duct.hierarchy_type,
    vm3.name AS own_vendor_name,
    COALESCE(duct.total_slack_count, 0) AS total_slack_count,
    COALESCE(duct.total_slack_length, 0) AS total_slack_length
   FROM att_details_duct duct
     JOIN province_boundary prov ON duct.province_id = prov.id
     JOIN region_boundary reg ON duct.region_id = reg.id
     JOIN user_master um ON duct.created_by = um.user_id
     JOIN vendor_master vm ON duct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON duct.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON duct.own_vendor_id::integer = vm3.id
     LEFT JOIN isp_type_master tp ON duct.type = tp.id
     LEFT JOIN isp_brand_master bd ON duct.brand = bd.id
     LEFT JOIN isp_base_model ml ON duct.model = ml.id
     LEFT JOIN user_master um2 ON duct.modified_by = um2.user_id;
	 
	 

 DROP VIEW public.vw_att_details_pod;

CREATE OR REPLACE VIEW public.vw_att_details_pod
 AS
 SELECT pod.system_id,
    pod.network_id,
    pod.pod_name,
    round(pod.latitude::numeric, 6) AS latitude,
    round(pod.longitude::numeric, 6) AS longitude,
    pod.pincode,
    pod.address,
    pod.specification,
    pod.category,
    pod.subcategory1,
    pod.subcategory2,
    pod.subcategory3,
    pod.item_code,
    pod.vendor_id,
    pod.type,
    pod.brand,
    pod.model,
    pod.construction,
    pod.activation,
    pod.accessibility,
    pod.status,
    pod.network_status,
    pod.province_id,
    pod.region_id,
    pod.created_by,
    pod.created_on,
    pod.modified_by,
    pod.modified_on,
    pod.parent_system_id,
    pod.parent_network_id,
    pod.parent_entity_type,
    pod.sequence_id,
    pod.structure_id,
    pod.shaft_id,
    pod.floor_id,
    pod.project_id,
    pod.planning_id,
    pod.purpose_id,
    pod.workorder_id,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS pod_types,
    bd.brand AS pod_brand,
    ml.model AS pod_model,
    fn_get_date(pod.created_on) AS created_date,
    fn_get_date(pod.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.accessibility) AS pod_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.construction) AS pod_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.activation) AS pod_activation,
    ''::character varying AS additional_attributes,
    ''::text AS total_ports,
    ''::text AS entity_category,
    pod.pod_name AS entity_name,
    pod.acquire_from,
    pod.pod_type,
    pod.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    pod.source_ref_type,
    pod.source_ref_id,
    pod.source_ref_description,
    pod.status_remark,
    pod.status_updated_by,
    pod.status_updated_on,
    pod.is_visible_on_map,
    pod.is_new_entity,
    pod.remarks,
    pod.audit_item_master_id,
    pod.is_acquire_from,
    isp_room_info.network_id AS unit_network_id,
    s.shaft_name,
    f.floor_name,
    pod.origin_from,
    pod.origin_ref_id,
    pod.origin_ref_code,
    pod.origin_ref_description,
    pod.request_ref_id,
    pod.requested_by,
    pod.request_approved_by,
    pod.subarea_id,
    pod.area_id,
    pod.dsa_id,
    pod.csa_id,
    pod.bom_sub_category,
    pod.gis_design_id,
    pod.st_x,
    pod.st_y,
    pod.ne_id,
    pod.prms_id,
    pod.jc_id,
    pod.mzone_id,
    pod.hierarchy_type,
    vm3.name AS own_vendor_name
    FROM att_details_pod pod
     JOIN province_boundary prov ON pod.province_id = prov.id
     JOIN region_boundary reg ON pod.region_id = reg.id
     JOIN user_master um ON pod.created_by = um.user_id
     JOIN vendor_master vm ON pod.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pod.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pod.own_vendor_id::integer = vm3.id
     LEFT JOIN isp_type_master tp ON pod.type = tp.id
     LEFT JOIN isp_brand_master bd ON pod.brand = bd.id
     LEFT JOIN isp_base_model ml ON pod.model = ml.id
     LEFT JOIN user_master um2 ON pod.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'POD'::text) m1 ON pod.system_id = m1.entity_id
     LEFT JOIN ( SELECT isp_entity_mapping.id,
            isp_entity_mapping.structure_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id,
            isp_entity_mapping.entity_type,
            isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'UNIT'::text) m2 ON m1.parent_id = m2.id
     LEFT JOIN isp_room_info ON m2.entity_id = isp_room_info.system_id AND m2.floor_id = isp_room_info.floor_id
     LEFT JOIN isp_shaft_info s ON m2.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m2.floor_id = f.system_id;













 DROP VIEW public.vw_att_details_fms;

CREATE OR REPLACE VIEW public.vw_att_details_fms
 AS
 SELECT fms.system_id,
    fms.network_id,
    fms.fms_name,
    round(fms.latitude::numeric, 6) AS latitude,
    round(fms.longitude::numeric, 6) AS longitude,
    fms.address,
    fms.specification,
    fms.category,
    fms.subcategory1,
    fms.subcategory2,
    fms.subcategory3,
    fms.item_code,
    fms.vendor_id,
    fms.type,
    fms.brand,
    fms.model,
    fms.construction,
    fms.activation,
    fms.accessibility,
    fms.status,
    fms.network_status,
    fms.province_id,
    fms.region_id,
    fms.created_by,
    fms.created_on,
    fms.modified_by,
    fms.modified_on,
    fms.pincode,
    prov.province_name,
    reg.region_name,
    fms.parent_system_id,
    fms.parent_network_id,
    fms.parent_entity_type,
    fms.sequence_id,
    fms.project_id,
    fms.planning_id,
    fms.purpose_id,
    fms.workorder_id,
    fms.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    fn_get_date(fms.created_on) AS created_date,
    fn_get_date(fms.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    fms.no_of_port,
    fms.no_of_input_port,
    fms.no_of_output_port,
    ''::character varying AS additional_attributes,
    (fms.no_of_input_port::character varying::text || ''::text) || fms.no_of_output_port::character varying::text AS total_ports,
    ''::text AS entity_category,
    fms.fms_name AS entity_name,
    fms.barcode,
    fms.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    fms.source_ref_type,
    fms.source_ref_id,
    fms.source_ref_description,
    fms.status_remark,
    fms.status_updated_by,
    fms.status_updated_on,
    fms.is_visible_on_map,
    fms.primary_pod_system_id,
    fms.secondary_pod_system_id,
    fms.is_new_entity,
    fms.remarks,
    fms.audit_item_master_id,
    fms.is_acquire_from,
    fms.origin_from,
    fms.origin_ref_id,
    fms.origin_ref_code,
    fms.origin_ref_description,
    fms.request_ref_id,
    fms.requested_by,
    fms.request_approved_by,
    fms.subarea_id,
    fms.area_id,
    fms.dsa_id,
    fms.csa_id,
    fms.bom_sub_category,
    fms.gis_design_id,
    fms.st_x,
    fms.st_y,
    fms.ne_id,
    fms.prms_id,
    fms.jc_id,
    fms.mzone_id,
    fms.hierarchy_type,
    vm3.name AS own_vendor_name
    FROM att_details_fms fms
     JOIN province_boundary prov ON fms.province_id = prov.id
     JOIN region_boundary reg ON fms.region_id = reg.id
     JOIN user_master um ON fms.created_by = um.user_id
     JOIN vendor_master vm ON fms.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON fms.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON fms.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON fms.modified_by = um2.user_id;






 DROP VIEW public.vw_att_details_spliceclosure;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure
 AS
 SELECT sc.system_id,
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
    sc.vendor_id,
    sc.type,
    sc.brand,
    sc.model,
    sc.construction,
    sc.activation,
    sc.accessibility,
    sc.status,
    sc.network_status,
    sc.province_id,
    sc.region_id,
    sc.created_by,
    sc.created_on,
    sc.modified_by,
    sc.modified_on,
    sc.pincode,
    prov.province_name,
    reg.region_name,
    sc.parent_system_id,
    sc.parent_network_id,
    sc.parent_entity_type,
    sc.sequence_id,
    sc.project_id,
    sc.planning_id,
    sc.purpose_id,
    sc.workorder_id,
    sc.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS sc_type,
    bd.brand AS sc_brand,
    ml.model AS sc_model,
    fn_get_date(sc.created_on) AS created_date,
    fn_get_date(sc.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    sc.is_virtual,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.accessibility) AS sc_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.construction) AS sc_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.activation) AS sc_activation,
    sc.no_of_ports,
    sc.no_of_input_port,
    sc.no_of_output_port,
    ''::character varying AS additional_attributes,
        CASE
            WHEN sc.no_of_input_port <> 0 AND sc.no_of_output_port <> 0 THEN ((sc.no_of_input_port::character varying::text || ''::text) || sc.no_of_output_port::character varying::text)::character varying
            ELSE sc.no_of_ports::character varying
        END AS total_ports,
    ''::character varying AS entity_category,
    sc.spliceclosure_name AS entity_name,
    sc.barcode,
    sc.is_buried,
    sc.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    sc.source_ref_type,
    sc.source_ref_id,
    sc.source_ref_description,
    sc.status_remark,
    sc.status_updated_by,
    sc.status_updated_on,
    sc.is_visible_on_map,
    sc.primary_pod_system_id,
    sc.secondary_pod_system_id,
    sc.is_new_entity,
    sc.remarks,
    sc.audit_item_master_id,
    sc.is_acquire_from,
    s.shaft_name,
    f.floor_name,
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
    sc.gis_design_id,
    sc.st_x,
    sc.st_y,
    sc.ne_id,
    sc.prms_id,
    sc.jc_id,
    sc.mzone_id,
    sc.hierarchy_type,
    sc.section_name,
    sc.aerial_location,
    sc.generic_section_name,
    vm3.name AS own_vendor_name,
    sc.spliceclosure_type
   FROM att_details_spliceclosure sc
     JOIN province_boundary prov ON sc.province_id = prov.id
     JOIN region_boundary reg ON sc.region_id = reg.id
     JOIN user_master um ON sc.created_by = um.user_id
     JOIN vendor_master vm ON sc.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sc.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON sc.own_vendor_id::integer = vm3.id
     LEFT JOIN isp_type_master tp ON sc.type = tp.id
     LEFT JOIN isp_brand_master bd ON sc.brand = bd.id
     LEFT JOIN isp_base_model ml ON sc.model = ml.id
     LEFT JOIN user_master um2 ON sc.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'SPLICECLOSURE'::text) m1 ON sc.system_id = m1.entity_id
     LEFT JOIN isp_shaft_info s ON m1.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m1.floor_id = f.system_id;




CREATE OR REPLACE FUNCTION public.fn_get_export_associate_entity(
    p_system_id integer,
    p_entity_type character varying
)
RETURNS SETOF json 
LANGUAGE 'plpgsql'
COST 100
VOLATILE PARALLEL UNSAFE
ROWS 1000

AS $BODY$
DECLARE
    sql TEXT;  
    v_layer_table character varying;
    v_entity_name character varying;
BEGIN
    -- Get the layer table name based on the entity type
    SELECT layer_table INTO v_layer_table 
    FROM layer_details 
    WHERE upper(layer_name) = upper(p_entity_type);

    -- Generate the dynamic column name
    v_entity_name := lower(p_entity_type) || '_name';

    -- Construct the SQL query dynamically
    sql := format(
        'SELECT row_to_json(row) 
        FROM (
            SELECT 
                ld.layer_title AS SI_OSP_GBL_GBL_GBL_134,
                tblmain.entity_network_id AS SI_OSP_GBL_GBL_GBL_133,
                tblmain.entity_display_name AS "Display Name",
                CASE 
                    WHEN tblmain.is_termination_point THEN ''YES'' 
                    ELSE ''NO'' 
                END AS SI_GBL_GBL_NET_FRM_194,
                fn_get_date(tblmain.created_on) AS SI_OSP_CAB_NET_RPT_004,
                um.user_name AS SI_OSP_CAB_NET_RPT_003,
                vld.%I AS "Entity name", vld.network_id as "entity_network_id"
            FROM (
                SELECT 
                    entity_system_id,
                    entity_network_id,
                    entity_type,
                    entity_display_name,
                    ass.created_by,
                    ass.created_on,
                    ass.is_termination_point 
                FROM 
                    associate_entity_info ass 
                WHERE  
                    (
                        (ass.entity_system_id = $1 AND upper(ass.entity_type) = upper($2)) 
                        OR 
                        (ass.associated_system_id = $1 AND upper(ass.associated_entity_type) = upper($2))
                    ) 
                UNION
                SELECT 
                    associated_system_id,
                    associated_network_id,
                    associated_entity_type,
                    associated_display_name,
                    ass.created_by,
                    ass.created_on,
                    ass.is_termination_point 
                FROM 
                    associate_entity_info ass 
                WHERE  
                    (
                        (ass.entity_system_id = $1 AND upper(ass.entity_type) = upper($2)) 
                        OR 
                        (ass.associated_system_id = $1 AND upper(ass.associated_entity_type) = upper($2))
                    )
            ) tblmain 
            LEFT JOIN user_master um ON um.user_id = tblmain.created_by
            LEFT JOIN layer_details ld ON upper(ld.layer_name) = upper(tblmain.entity_type) 
            LEFT JOIN %I vld ON  vld.system_id =  $1
            WHERE 
                tblmain.entity_system_id != $1 
                AND upper(tblmain.entity_type) != upper($2)
        ) row', 
        v_entity_name, v_layer_table);

    -- Execute the dynamic SQL
    RETURN QUERY EXECUTE sql USING p_system_id, p_entity_type;
END;
$BODY$;
