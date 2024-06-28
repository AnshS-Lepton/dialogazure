delete from dropdown_master where dropdown_type='Manhole_types' and dropdown_value='Aerial Chamber';
update res_resources set value='Normal(OK)' where key='SI_OSP_SC_NET_FRM_005' and culture!='hi';


ALTER TABLE public.att_details_spliceclosure ADD spliceclosure_type varchar;
ALTER TABLE public.audit_att_details_spliceclosure ADD spliceclosure_type varchar;

INSERT INTO public.dropdown_master (layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where upper(layer_name)='SPLICECLOSURE'), 'Spliceclosure_type', 'Normal', true, 1, '2024-01-18 20:54:21.125', NULL, NULL, 'Normal', NULL, 'Spliceclosure_type', true, true, 0),
((select layer_id from layer_details where upper(layer_name)='SPLICECLOSURE'), 'Spliceclosure_type', 'Buried', true, 1, '2024-01-18 20:54:21.125', NULL, NULL, 'Buried', NULL, 'Spliceclosure_type', true, true, 0),
((select layer_id from layer_details where upper(layer_name)='SPLICECLOSURE'), 'Spliceclosure_type', 'Open', true, 1, '2024-01-18 20:54:21.125', NULL, NULL, 'Open', NULL, 'Spliceclosure_type', true, true, 0),
((select layer_id from layer_details where upper(layer_name)='SPLICECLOSURE'), 'Spliceclosure_type', 'Aerial', true, 1, '2024-01-18 20:54:21.125', NULL, NULL, 'Aerial', NULL, 'Spliceclosure_type', true, true, 0);


-- public.vw_att_details_spliceclosure source

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure
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
    sc.own_vendor_id,
    sc.spliceclosure_type
   FROM att_details_spliceclosure sc
     JOIN province_boundary prov ON sc.province_id = prov.id
     JOIN region_boundary reg ON sc.region_id = reg.id
     JOIN user_master um ON sc.created_by = um.user_id
     JOIN vendor_master vm ON sc.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sc.third_party_vendor_id = vm2.id
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
	 
drop view vw_att_details_spliceclosure_map;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_map
AS SELECT spliceclosure.system_id,
    spliceclosure.region_id,
    spliceclosure.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    spliceclosure.spliceclosure_name,
    spliceclosure.status,
    spliceclosure.is_new_entity,
    spliceclosure.project_id,
    spliceclosure.planning_id,
    spliceclosure.workorder_id,
        CASE
            WHEN spliceclosure.is_buried = true THEN 1
            ELSE 0
        END AS is_buried,
    spliceclosure.purpose_id,
    spliceclosure.network_id,
    spliceclosure.is_virtual,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), spliceclosure.network_status::text) AS network_status,
    spliceclosure.ownership_type,
    spliceclosure.third_party_vendor_id,
    spliceclosure.primary_pod_system_id,
    spliceclosure.secondary_pod_system_id,
    spliceclosure.is_visible_on_map,
    spliceclosure.source_ref_id,
    spliceclosure.source_ref_type,
    spliceclosure.spliceclosure_type,
    spliceclosure.network_id AS label_column
   FROM point_master point
     JOIN att_details_spliceclosure spliceclosure ON point.system_id = spliceclosure.system_id AND point.entity_type::text = 'SpliceClosure'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = spliceclosure.system_id AND adi1.entity_type::text = 'SpliceClosure'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = spliceclosure.system_id AND adi.entity_type::text = 'SpliceClosure'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;


-- public.vw_att_details_spliceclosure_audit source

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_audit
AS SELECT sc.audit_id,
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
    sc.spliceclosure_type
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
     LEFT JOIN entity_notification_status entnotifystatus ON sc.notification_status_id = entnotifystatus.id;


drop view vw_att_details_spliceclosure_report;

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