
insert into dropdown_master(layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active) 
 values(0,'FMS_Type','FMS Type1',true,1,now(),'FMS_Type1','FMS_Type',true,true);


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
    vm3.name AS own_vendor_name,
	fms.installation_location_code,
	fms.fms_type
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

ALTER TABLE public.vw_att_details_fms_report
    OWNER TO postgres;

-----------------

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
    vm3.name AS own_vendor_name,
	fms.installation_location_code,
	fms.fms_type
   FROM att_details_fms fms
     JOIN province_boundary prov ON fms.province_id = prov.id
     JOIN region_boundary reg ON fms.region_id = reg.id
     JOIN user_master um ON fms.created_by = um.user_id
     JOIN vendor_master vm ON fms.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON fms.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON fms.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON fms.modified_by = um2.user_id;

ALTER TABLE public.vw_att_details_fms
    OWNER TO postgres;

-------------------------------------------