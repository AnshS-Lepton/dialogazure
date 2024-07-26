


update dropdown_master set layer_id = 0 where  dropdown_type ='A_location' ;


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
    vm3.name AS own_vendor_name,
    trench.ward,
	trench.a_location_code,
	trench.b_location_code
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

ALTER TABLE public.vw_att_details_trench
    OWNER TO postgres;

-- View: public.vw_att_details_duct

-- DROP VIEW public.vw_att_details_duct;

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
    COALESCE(duct.total_slack_length, 0) AS total_slack_length,
	duct.a_location_code,
	duct.b_location_code
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

ALTER TABLE public.vw_att_details_duct
    OWNER TO postgres;


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
    vm3.name AS own_vendor_name,
	trench.a_location_code,
	trench.b_location_code
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

ALTER TABLE public.vw_att_details_trench_report
    OWNER TO postgres;
	
	
	
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
    vm3.name AS own_vendor_name,
	duct.a_location_code,
	duct.b_location_code
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

ALTER TABLE public.vw_att_details_duct_report
    OWNER TO postgres;





