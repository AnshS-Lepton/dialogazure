-- public.vw_att_details_wallmount_report source

CREATE OR REPLACE VIEW public.vw_att_details_wallmount_report
AS SELECT wallmount.system_id,
    wallmount.network_id,
    wallmount.wallmount_name,
    wallmount.wallmount_height,
    wallmount.wallmount_no,
    round(wallmount.latitude::numeric, 6) AS latitude,
    round(wallmount.longitude::numeric, 6) AS longitude,
    wallmount.address,
    wallmount.specification,
    wallmount.category,
    wallmount.subcategory1,
    wallmount.subcategory2,
    wallmount.subcategory3,
    wallmount.item_code,
    wallmount.accessibility,
    wallmount.acquire_from,
        CASE
            WHEN wallmount.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN wallmount.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN wallmount.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, wallmount.status) AS status,
    wallmount.parent_network_id AS parent_code,
    wallmount.parent_entity_type AS parent_type,
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
    to_char(wallmount.created_on, 'DD-Mon-YY'::text) AS created_on,
    um.user_name AS created_by,
    to_char(wallmount.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    wallmount.region_id,
    wallmount.province_id,
        CASE
            WHEN wallmount.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    wallmount.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    wallmount.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    wallmount.source_ref_type,
    wallmount.source_ref_id,
    wallmount.source_ref_description,
    wallmount.status_remark,
    wallmount.status_updated_by,
    to_char(wallmount.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    wallmount.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    wallmount.remarks,
    reg.country_name,
    wallmount.origin_from,
    wallmount.origin_ref_id,
    wallmount.origin_ref_code,
    wallmount.origin_ref_description,
    wallmount.request_ref_id,
    wallmount.requested_by,
    wallmount.request_approved_by,
    wallmount.subarea_id,
    wallmount.area_id,
    wallmount.dsa_id,
    wallmount.csa_id,
    wallmount.bom_sub_category,
    wallmount.gis_design_id AS design_id,
    wallmount.st_x,
    wallmount.st_y,
    wallmount.ne_id,
    wallmount.prms_id,
    wallmount.jc_id,
    wallmount.mzone_id,
    wallmount.served_by_ring,
    vm3.name AS own_vendor_name
   FROM att_details_wallmount wallmount
     JOIN province_boundary prov ON wallmount.province_id = prov.id
     JOIN region_boundary reg ON wallmount.region_id = reg.id
     JOIN user_master um ON wallmount.created_by = um.user_id
     JOIN vendor_master vm ON wallmount.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON wallmount.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON wallmount.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON wallmount.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON wallmount.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON wallmount.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON wallmount.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON wallmount.modified_by = um2.user_id
     LEFT JOIN att_details_pod primarypod ON wallmount.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON wallmount.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = wallmount.status::text;
	 
	 
	 -------------------------------pole-------------
	 -- public.vw_att_details_pole_report source

CREATE OR REPLACE VIEW public.vw_att_details_pole_report
AS SELECT pole.system_id,
    pole.network_id,
    pole.pole_name,
    pole.pole_type,
    pole.pole_height,
    pole.pole_no,
    round(pole.latitude::numeric, 6) AS latitude,
    round(pole.longitude::numeric, 6) AS longitude,
    pole.address,
    pole.acquire_from,
    pole.specification,
    pole.category,
    pole.subcategory1,
    pole.subcategory2,
    pole.subcategory3,
    pole.item_code,
        CASE
            WHEN pole.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
        CASE
            WHEN pole.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN pole.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN pole.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, pole.status) AS status,
    prov.province_name,
    reg.region_name,
    pole.parent_network_id AS parent_code,
    pole.parent_entity_type AS parent_type,
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
    to_char(pole.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(pole.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pole.region_id,
    pole.province_id,
    pole.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    pole.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    pole.source_ref_type,
    pole.source_ref_id,
    pole.source_ref_description,
    pole.status_remark,
    pole.status_updated_by,
    to_char(pole.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    pole.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    pole.remarks,
    reg.country_name,
    pole.origin_from,
    pole.origin_ref_id,
    pole.origin_ref_code,
    pole.origin_ref_description,
    pole.request_ref_id,
    pole.requested_by,
    pole.request_approved_by,
    pole.subarea_id,
    pole.area_id,
    pole.dsa_id,
    pole.csa_id,
    pole.bom_sub_category,
    pole.gis_design_id AS design_id,
    pole.st_x,
    pole.st_y,
    pole.ne_id,
    pole.prms_id,
    pole.jc_id,
    pole.mzone_id,
    pole.served_by_ring,
    vm3.name AS own_vendor_name
   FROM att_details_pole pole
     JOIN province_boundary prov ON pole.province_id = prov.id
     JOIN region_boundary reg ON pole.region_id = reg.id
     JOIN user_master um ON pole.created_by = um.user_id
     JOIN vendor_master vm ON pole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pole.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pole.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON pole.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON pole.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON pole.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON pole.purpose_id = purposem.system_id
     LEFT JOIN att_details_pod primarypod ON pole.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON pole.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN user_master um2 ON pole.modified_by = um2.user_id
     LEFT JOIN entity_status_master es ON es.status::text = pole.status::text;