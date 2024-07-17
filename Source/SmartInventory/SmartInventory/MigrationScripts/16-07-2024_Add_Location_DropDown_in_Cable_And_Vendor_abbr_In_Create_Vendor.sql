
CREATE OR REPLACE VIEW public.vw_att_details_cable
 AS
 SELECT cable.system_id,
    cable.network_id,
    cable.cable_name,
    fn_get_display_name(cable.a_system_id, cable.a_entity_type) AS a_location,
    fn_get_display_name(cable.b_system_id, cable.b_entity_type) AS b_location,
    cable.total_core,
    cable.no_of_tube,
    cable.no_of_core_per_tube,
    round(cable.cable_measured_length::numeric, 2) AS cable_measured_length,
    round(cable.cable_calculated_length::numeric, 2) AS cable_calculated_length,
    cable.cable_type,
    cable.coreaccess,
    cable.wavelength,
    cable.optical_output_power,
    cable.frequency,
    cable.attenuation_db,
    cable.resistance_ohm,
    cable.construction,
    cable.activation,
    cable.accessibility,
    cable.specification,
    cable.category,
    cable.subcategory1,
    cable.subcategory2,
    cable.subcategory3,
    cable.item_code,
    cable.vendor_id,
    cable.type,
    cable.brand,
    cable.model,
    cable.network_status,
    cable.status,
    cable.pin_code,
    cable.province_id,
    cable.region_id,
    cable.utilization,
    cable.totalattenuationloss,
    cable.chromaticdb,
    cable.chromaticdispersion,
    cable.totalchromaticloss,
    cable.remarks,
    cable.route_id,
    cable.created_by,
    cable.created_on,
    cable.modified_by,
    cable.modified_on,
    cable.a_system_id,
    cable.a_entity_type,
    cable.b_system_id,
    cable.b_entity_type,
    cable.sequence_id,
    cable.duct_id,
    cable.trench_id,
    cable.project_id,
    cable.planning_id,
    cable.purpose_id,
    cable.workorder_id,
    cable.structure_id,
    cable.cable_category,
    cable.execution_method,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS cable_specification_type,
    bd.brand AS cable_brand,
    ml.model AS cable_model,
    cable.total_loop_count,
    cable.total_loop_length,
    fn_get_date(cable.created_on) AS created_date,
    fn_get_date(cable.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    cable.parent_system_id,
    cable.parent_entity_type,
    cable.parent_network_id,
    round(cable.start_reading::numeric, 2) AS start_reading,
    round(cable.end_reading::numeric, 2) AS end_reading,
    cable.cable_sub_category,
    cable.acquire_from,
    cable.circuit_id,
    cable.thirdparty_circuit_id,
    cable.drum_no,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cable.accessibility) AS cable_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cable.construction) AS cable_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cable.activation) AS cable_activation,
    cable.barcode,
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
    cable.is_new_entity,
    round(cable.inner_dimension::numeric, 2) AS inner_dimension,
    round(cable.outer_dimension::numeric, 2) AS outer_dimension,
    cable.calculated_length_remark,
    cable.audit_item_master_id,
    cable.is_acquire_from,
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
    cable.a_location_name,
    cable.b_location_name,
    cable.route_name,
    cable.hierarchy_type,
    cable.section_name,
    cable.aerial_location,
    cable.cable_remark,
    cable.generic_section_name,
    cable.own_vendor_id,
    cable.manhole_count,
	cable.a_location_code,
	cable.b_location_code
   FROM att_details_cable cable
     JOIN province_boundary prov ON cable.province_id = prov.id
     JOIN region_boundary reg ON cable.region_id = reg.id
     JOIN user_master um ON cable.created_by = um.user_id
     JOIN vendor_master vm ON cable.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cable.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON cable.type = tp.id
     LEFT JOIN isp_brand_master bd ON cable.brand = bd.id
     LEFT JOIN isp_base_model ml ON cable.model = ml.id
     LEFT JOIN user_master um2 ON cable.modified_by = um2.user_id;

ALTER TABLE public.vw_att_details_cable
    OWNER TO postgres;

-------------------------------------------------
-- View: public.vw_att_details_cable_report

-- DROP VIEW public.vw_att_details_cable_report;

CREATE OR REPLACE VIEW public.vw_att_details_cable_report
 AS
 SELECT cable.system_id,
    cable.network_id,
    cable.cable_name,
    cable.total_core,
    cable.no_of_tube,
    cable.no_of_core_per_tube,
    round(cable.cable_measured_length::numeric, 2) AS cable_measured_length,
    round(cable.cable_calculated_length::numeric, 2) AS cable_calculated_length,
    cable.cable_type,
    cable.coreaccess,
    cable.wavelength,
    cable.optical_output_power,
    cable.frequency,
    cable.attenuation_db,
    cable.resistance_ohm,
    cable.construction,
    cable.activation,
    cable.accessibility,
    cable.specification,
    cable.category,
    cable.subcategory1,
    cable.subcategory2,
    cable.subcategory3,
    cable.item_code,
        CASE
            WHEN cable.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN cable.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN cable.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.status, cable.status) AS status,
    cable.pin_code,
    cable.utilization,
    cable.totalattenuationloss,
    cable.chromaticdb,
    cable.chromaticdispersion,
    cable.totalchromaticloss,
    cable.remarks,
    cable.a_entity_type,
    cable.b_entity_type,
    cable.cable_category,
    cable.execution_method,
    cable.total_loop_count,
    cable.total_loop_length,
    cable.parent_entity_type AS parent_type,
    cable.parent_network_id AS parent_code,
    cable.route_id,
    round(cable.start_reading::numeric, 2) AS start_reading,
    round(cable.end_reading::numeric, 2) AS end_reading,
    cable.cable_sub_category,
    um.user_name AS created_by,
    to_char(cable.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(cable.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    cable.drum_no,
    cable.acquire_from,
    cable.circuit_id,
    cable.thirdparty_circuit_id,
    cable.province_id,
    cable.region_id,
        CASE
            WHEN cable.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    reg.region_name,
    cable.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    prov.province_name,
        CASE
            WHEN cable.utilization::text = 'L'::text THEN 'Low'::text
            WHEN cable.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN cable.utilization::text = 'H'::text THEN 'High'::text
            WHEN cable.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    cable.ownership_type,
    vm.name AS vendor_name,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    cable.source_ref_type,
    cable.source_ref_id,
    cable.source_ref_description,
    cable.status_remark,
    cable.status_updated_by,
    to_char(cable.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    cable.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    round(cable.inner_dimension::numeric, 2) AS inner_dimension,
    round(cable.outer_dimension::numeric, 2) AS outer_dimension,
    cable.calculated_length_remark,
    reg.country_name,
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
    cable.gis_design_id AS evoid,
    cable.ne_id,
    cable.prms_id,
    cable.jc_id,
    cable.mzone_id,
    cable.served_by_ring,
    cable.a_latitude,
    cable.b_longitude,
    cable.a_region,
    cable.b_region,
    cable.a_city,
    cable.b_city,
    (( SELECT count(*) AS count
           FROM att_details_cable_info cableinfo
          WHERE cableinfo.cable_id = cable.system_id AND cableinfo.link_system_id > 0)) + (( SELECT count(*) AS count
           FROM att_details_cable_info cableinfo
          WHERE cableinfo.cable_id = cable.system_id AND lower(cableinfo.core_comment::text) ~~* lower('reserved%'::text))) AS used_core,
    cable.hierarchy_type,
    cable.section_name,
    cable.aerial_location,
    cable.generic_section_name,
    vm3.name AS own_vendor,
    cable.cable_remark,
    cable.manhole_count,
	cable.a_location_code,
	cable.b_location_code
   FROM att_details_cable cable
     JOIN user_master um ON cable.created_by = um.user_id
     JOIN vendor_master vm ON cable.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cable.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON cable.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON cable.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON cable.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON cable.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON cable.purpose_id = purposem.system_id
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(cable.network_id::text) ~~ upper('NLD%'::text) THEN cable.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('CABLE'::text) AND cable.province_id = pmapping.province_id
            ELSE 1 = 2
        END
     JOIN province_boundary prov ON prov.id = COALESCE(pmapping.province_id, cable.province_id)
     JOIN region_boundary reg ON reg.id = COALESCE(pmapping.region_id, cable.region_id)
     LEFT JOIN entity_notification_status entnotifystatus ON cable.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON cable.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON cable.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN vendor_master vm3 ON vm3.id = cable.own_vendor_id::integer
     LEFT JOIN entity_status_master es ON es.status::text = cable.status::text;

ALTER TABLE public.vw_att_details_cable_report
    OWNER TO postgres;
