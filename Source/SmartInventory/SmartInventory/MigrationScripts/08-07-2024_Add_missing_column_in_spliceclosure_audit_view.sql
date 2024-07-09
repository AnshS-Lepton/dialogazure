-- View: public.vw_att_details_spliceclosure_audit

-- DROP VIEW public.vw_att_details_spliceclosure_audit;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_audit
 AS
 SELECT sc.audit_id,
    sc.system_id,
    sc.network_id,
    sc.spliceclosure_name,
    round(sc.latitude::numeric, 6) AS latitude,
    round(sc.longitude::numeric, 6) AS longitude,
    reg.region_name,
    prov.province_name,
    sc.address,
    sc.pincode,
    sc.specification,
    vm.name AS vendor_name,
    sc.item_code,
    sc.category,
    sc.subcategory1,
    sc.subcategory2,
    sc.parent_network_id,
    sc.parent_entity_type,
    sc.acquire_from,
    sc.no_of_ports,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_date(sc.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(sc.modified_on) AS modified_on,
    ums.user_name AS modified_by,
    fn_get_network_status(sc.network_status) AS network_status,
    fn_get_status(sc.status) AS status,
    fn_get_action(sc.action) AS action,
        CASE
            WHEN sc.is_virtual = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_virtual,
    sc.barcode,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
        CASE
            WHEN sc.is_buried = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_buried,
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
    sc.remarks,
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
    sc.ne_id,
    sc.prms_id,
    sc.jc_id,
    sc.mzone_id,
    sc.spliceclosure_type,
    oi.is_plice,
    oi.record_system_id
     FROM audit_att_details_spliceclosure sc
     JOIN province_boundary prov ON sc.province_id = prov.id
     JOIN region_boundary reg ON sc.region_id = reg.id
     JOIN user_master um ON sc.created_by = um.user_id
     LEFT JOIN user_master ums ON sc.modified_by = ums.user_id
     JOIN vendor_master vm ON sc.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sc.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON sc.type = tp.id
     LEFT JOIN isp_brand_master bd ON sc.brand = bd.id
     LEFT JOIN isp_base_model ml ON sc.model = ml.id
     LEFT JOIN att_details_project_master pm ON sc.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON sc.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON sc.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON sc.purpose_id = purposem.system_id
     LEFT JOIN entity_notification_status entnotifystatus ON sc.notification_status_id = entnotifystatus.id
	 JOIN vw_att_details_spliceclosure_audit_oi oi ON oi.audit_id = sc.audit_id;

ALTER TABLE public.vw_att_details_spliceclosure_audit
    OWNER TO postgres;

