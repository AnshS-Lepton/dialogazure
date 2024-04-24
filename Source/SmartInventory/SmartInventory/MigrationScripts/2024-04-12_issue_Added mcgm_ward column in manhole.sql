alter table att_details_manhole 
add column mcgm_ward varchar(100) NULL;

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('Manhole', 'mcgm_ward', 'field', true, false, 1, now(), 287, NULL, NULL, false);

select layer_id,*from layer_details where layer_name 'Manhole'
INSERT INTO public.dropdown_master
(  layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'A', true, 1, now(), 1, null, 'A', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'B', true, 1, now(), 1, null, 'B', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'C', true, 1, now(), 1, null, 'C', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'D', true, 1, now(), 1,null, 'D', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'E', true, 1, now(), 1, null, 'E', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'F/South', true, 1, now(), 1, null, 'F/South', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'F/North', true, 1, now(), 1, null, 'F/North', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'G/South', true, 1, now(), 1, null, 'G/South', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'G/North', true, 1, now(), 1, null, 'G/North', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'H/East', true, 1, now(), 1, null, 'H/East', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'H/West', true, 1, now(), 1, null, 'H/West', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'K/East', true, 1, now(), 1, null, 'K/East', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'K/West', true, 1,now(), 1, null, 'K/West', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'L', true, 1, now(), 1, null, 'L', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'M/East', true, 1, now(), 1, null, 'M/East', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'M/West', true, 1, now(), 1,null, 'M/West', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'N', true, 1, now(), 1,null, 'N', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'P/South', true, 1, now(), 1, null, 'P/South', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'P/North', true, 1, now(), 1, null, 'P/North', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'R/South', true, 1, now(), 1, null, 'R/South', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'R/Central', true, 1, now(), 1, null, 'R/Central', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'R/North', true, 1, now(), 1, null, 'R/North', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'S', true, 1,now(), 1, null, 'S', NULL, 'MCGM_Ward', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name = 'Manhole'), 'MCGM_Ward', 'T', true, 1, now(), 1, null, 'T', NULL, 'MCGM_Ward', true, true, 0);






-- public.vw_att_details_manhole source

CREATE OR replace  VIEW public.vw_att_details_manhole
AS SELECT manhole.system_id,
    manhole.network_id,
    manhole.manhole_name,
    round(manhole.latitude::numeric, 6) AS latitude,
    round(manhole.longitude::numeric, 6) AS longitude,
    manhole.address,
    manhole.specification,
    manhole.category,
    manhole.subcategory1,
    manhole.subcategory2,
    manhole.subcategory3,
    manhole.item_code,
    manhole.vendor_id,
    manhole.type,
    manhole.brand,
    manhole.model,
    manhole.construction,
    manhole.activation,
    manhole.accessibility,
    manhole.status,
    manhole.network_status,
    manhole.province_id,
    manhole.region_id,
    manhole.created_by,
    manhole.created_on,
    manhole.modified_by,
    manhole.modified_on,
    prov.province_name,
    reg.region_name,
    manhole.parent_system_id,
    manhole.parent_network_id,
    manhole.parent_entity_type,
    manhole.sequence_id,
    manhole.project_id,
    manhole.planning_id,
    manhole.purpose_id,
    manhole.workorder_id,
    manhole.is_virtual,
    manhole.construction_type,
    manhole.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS manhole_type,
    bd.brand AS manhole_brand,
    ml.model AS manhole_model,
    fn_get_date(manhole.created_on) AS created_date,
    fn_get_date(manhole.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.accessibility) AS manhole_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.construction) AS manhole_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.activation) AS manhole_activation,
    manhole.barcode,
    manhole.is_buried,
    manhole.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    manhole.source_ref_type,
    manhole.source_ref_id,
    manhole.source_ref_description,
    manhole.status_remark,
    manhole.status_updated_by,
    manhole.status_updated_on,
    manhole.is_visible_on_map,
    manhole.primary_pod_system_id,
    manhole.secondary_pod_system_id,
    manhole.is_new_entity,
    manhole.remarks,
    manhole.manhole_types,
    manhole.audit_item_master_id,
    manhole.is_acquire_from,
    manhole.origin_from,
    manhole.origin_ref_id,
    manhole.origin_ref_code,
    manhole.origin_ref_description,
    manhole.request_ref_id,
    manhole.requested_by,
    manhole.request_approved_by,
    manhole.subarea_id,
    manhole.area_id,
    manhole.dsa_id,
    manhole.csa_id,
    manhole.bom_sub_category,
    manhole.gis_design_id,
    manhole.st_x,
    manhole.st_y,
    manhole.ne_id,
    manhole.prms_id,
    manhole.jc_id,
    manhole.mzone_id,
    manhole.area,
    manhole.authority,
    manhole.route_name,
    manhole.mcgm_ward
   FROM att_details_manhole manhole
     JOIN province_boundary prov ON manhole.province_id = prov.id
     JOIN region_boundary reg ON manhole.region_id = reg.id
     JOIN user_master um ON manhole.created_by = um.user_id
     JOIN vendor_master vm ON manhole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON manhole.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON manhole.type = tp.id
     LEFT JOIN isp_brand_master bd ON manhole.brand = bd.id
     LEFT JOIN isp_base_model ml ON manhole.model = ml.id
     LEFT JOIN user_master um2 ON manhole.modified_by = um2.user_id;
     
  
    -- public.vw_att_details_manhole_report source
    -- drop VIEW public.vw_att_details_manhole_report
    select * from vw_att_details_manhole_report vadmr 
    
CREATE OR replace  VIEW public.vw_att_details_manhole_report
AS SELECT manhole.system_id,
    manhole.network_id,
    manhole.manhole_name,
    round(manhole.latitude::numeric, 6) AS latitude,
    round(manhole.longitude::numeric, 6) AS longitude,
    manhole.address,
    manhole.specification,
    manhole.category,
    manhole.subcategory1,
    manhole.subcategory2,
    manhole.subcategory3,
    manhole.item_code,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_network_status(manhole.network_status) AS network_status,
    COALESCE(fn_get_entity_status(manhole.status), manhole.status) AS status,
    prov.province_name,
    reg.region_name,
    manhole.region_id,
    manhole.province_id,
    manhole.parent_network_id AS parent_code,
    manhole.parent_entity_type AS parent_type,
        CASE
            WHEN manhole.is_virtual = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_virtual,
    manhole.construction_type,
    manhole.acquire_from,
    vm.name AS vendor_name,
    fn_get_date(manhole.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(manhole.modified_on) AS modified_on,
    um2.user_name AS modified_by,
        CASE
            WHEN manhole.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    manhole.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
        CASE
            WHEN manhole.is_buried = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_buried,
    manhole.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    manhole.source_ref_type,
    manhole.source_ref_id,
    manhole.source_ref_description,
    manhole.status_remark,
    manhole.status_updated_by,
    fn_get_date(manhole.status_updated_on) AS status_updated_on,
    manhole.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    manhole.remarks,
    manhole.manhole_types,
    reg.country_name,
    manhole.origin_from,
    manhole.origin_ref_id,
    manhole.origin_ref_code,
    manhole.origin_ref_description,
    manhole.request_ref_id,
    manhole.requested_by,
    manhole.request_approved_by,
    manhole.subarea_id,
    manhole.area_id,
    manhole.dsa_id,
    manhole.csa_id,
    manhole.bom_sub_category,
    manhole.gis_design_id AS design_id,
    manhole.st_x,
    manhole.st_y,
    manhole.ne_id,
    manhole.prms_id,
    manhole.mzone_id,
    manhole.jc_id,
    manhole.area,
    manhole.authority,
    manhole.route_name,
    manhole.served_by_ring,
    manhole.mcgm_ward
   FROM att_details_manhole manhole
     JOIN province_boundary prov ON manhole.province_id = prov.id
     JOIN region_boundary reg ON manhole.region_id = reg.id
     JOIN user_master um ON manhole.created_by = um.user_id
     JOIN vendor_master vm ON manhole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON manhole.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON manhole.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON manhole.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON manhole.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON manhole.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON manhole.purpose_id = purposem.system_id
     LEFT JOIN att_details_pod primarypod ON manhole.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON manhole.secondary_pod_system_id = secondarypod.system_id;
    
  select * from layer_details 
