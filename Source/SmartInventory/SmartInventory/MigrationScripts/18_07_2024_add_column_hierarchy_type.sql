



Drop view vw_att_details_cable_audit;
CREATE OR REPLACE VIEW public.vw_att_details_cable_audit
AS SELECT cable.audit_id,
    cable.system_id,
    cable.network_id,
    cable.cable_name,
    fn_get_display_name(cable.a_system_id, cable.a_entity_type) AS start_point,
    fn_get_display_name(cable.b_system_id, cable.b_entity_type) AS end_point,
    reg.region_name,
    prov.province_name,
    cable.pin_code,
    cable.a_entity_type,
    cable.b_entity_type,
    cable.cable_type,
    cable.no_of_tube,
    cable.no_of_core_per_tube,
    round(cable.cable_measured_length::numeric, 2) AS cable_measured_length,
    round(cable.cable_calculated_length::numeric, 2) AS cable_calculated_length,
    cable.coreaccess,
    cable.wavelength,
    cable.optical_output_power,
    cable.frequency,
    cable.attenuation_db AS attenuation,
    cable.resistance_ohm AS resistance,
    cable.specification,
    vm.name AS vendor_name,
    cable.item_code,
    cable.category,
    cable.subcategory1,
    cable.subcategory2,
    cable.cable_category,
    cable.total_loop_length,
    cable.total_loop_count,
    cable.route_id,
    round(cable.start_reading::numeric, 2) AS start_reading,
    round(cable.end_reading::numeric, 2) AS end_reading,
    cable.cable_sub_category,
    cable.execution_method,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    cable.remarks,
    cable.acquire_from,
    cable.circuit_id,
    cable.thirdparty_circuit_id,
    fn_get_date(cable.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(cable.modified_on) AS modified_on,
    ums.user_name AS modified_by,
    fn_get_network_status(cable.network_status) AS network_status,
    fn_get_status(cable.status) AS status,
    fn_get_action(cable.action) AS action,
    cable.barcode,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    cable.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    cable.source_ref_type,
    cable.source_ref_id,
    cable.source_ref_description,
    cable.status_remark,
    cable.status_updated_by,
    cable.status_updated_on,
    cable.is_visible_on_map,
    cable.primary_pod_system_id,
    cable.secondary_pod_system_id,
    round(cable.inner_dimension::numeric, 2) AS inner_dimension,
    round(cable.outer_dimension::numeric, 2) AS outer_dimension,
    cable.calculated_length_remark,
    cable.origin_from,
    cable.origin_ref_id,
    cable.origin_ref_code,
    cable.origin_ref_description,
    cable.request_ref_id,
    cable.requested_by,
    cable.request_approved_by,
    cable.subarea_id,
    cable.area_id,
    cable.dsa_id,
    cable.csa_id,
    cable.bom_sub_category,
    cable.gis_design_id,
    cable.ne_id,
    cable.prms_id,
    cable.jc_id,
    cable.mzone_id,
    cable.cable_remark,
    cable.start_name,
    cable.end_name
   FROM audit_att_details_cable cable
     JOIN province_boundary prov ON cable.province_id = prov.id
     JOIN region_boundary reg ON cable.region_id = reg.id
     LEFT JOIN user_master um ON cable.created_by = um.user_id
     LEFT JOIN user_master ums ON cable.modified_by = ums.user_id
     JOIN vendor_master vm ON cable.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cable.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON cable.type = tp.id
     LEFT JOIN isp_brand_master bd ON cable.brand = bd.id
     LEFT JOIN isp_base_model ml ON cable.model = ml.id
     LEFT JOIN att_details_project_master pm ON cable.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON cable.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON cable.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON cable.purpose_id = purposem.system_id
     LEFT JOIN entity_notification_status entnotifystatus ON cable.notification_status_id = entnotifystatus.id;