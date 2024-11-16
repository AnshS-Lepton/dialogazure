
INSERT INTO public.layer_details
( layer_name, isvisible, minzoomlevel, maxzoomlevel, minboundvalue, maxboundvalue, parent_layer_id, layer_abbr, map_abbr, layer_title, layer_form_url, layer_seq, is_network_entity, is_visible_in_ne_library, is_template_required, network_id_type, geom_type, template_form_url, layer_table, network_code_seperator, save_entity_url, is_direct_save, layer_view, is_clone, is_isp_layer, is_shaft_element, is_osp_layer, unit_type, unit_input_type, is_multi_clone, is_mobile_layer, is_visible_in_mobile_lib, is_vendor_spec_required, is_splicer, report_view_name, is_report_enable, is_networktype_required, is_isp_splicer, is_label_change_allowed, layer_network_group, is_multi_association, history_view_name, is_history_enabled, is_info_enabled, is_cpe_entity, is_virtual_port_allowed, is_reference_allowed, is_isp_child_layer, map_layer_seq, layer_template_table, is_isp_parent_layer, is_floor_element, is_osp_layer_freezed_in_library, is_isp_layer_freezed_in_library, is_other_wcr_layer, is_barcode_enabled, barcode_column, is_at_enabled, is_maintainence_charges_enabled, is_feasibility_layer, feasibility_network_group, is_row_association_enabled, is_logicalview_enabled, is_site_enabled, is_networkcode_change_enabled, is_middleware_entity, is_lmc_enabled, is_utilization_enabled, is_layer_for_rights_permission, is_data_upload_enabled, is_fault_entity, data_upload_max_count, data_upload_table, is_moredetails_enable, is_project_spec_allowed, is_fiber_link_enabled, is_visible_on_mobile_map, is_loop_allowed, is_pod_association_allowed, is_remark_required_from_mobile, is_trayinfo_enabled, is_split_allowed, is_association_enabled, is_association_mandatory, is_auto_plan_end_point, is_network_ticket_entity, is_mobile_isp_layer, is_tp_layer, is_offline_allowed, specification_dropdown_type, is_entity_along_direction, is_vsat_enabled, other_info_view, other_info_view_audit, audit_table_name, is_bomboq_enabled, is_dynamic_control_enable, is_dynamic_enabled, category_name, type_name, ne_class_name, gis_class_name, layer_display_abbr, is_unique_column_name, unique_column_name, vc_view_name, delta_metadata_table, is_vector_layer_implemented, entity_geojson_table, additional_report_view_name)
VALUES('FiberLink', true, 12, 21, 7, 21, 0, 'FBR', 'FBR', 'FiberLink', '', 6, true, true, true, 'A', 'Line', '', 'att_details_fiber_link', '-', '', false, 'vw_att_details_fiber_link', false, false, false, false, NULL, '', true, true, true, true, false, 'vw_att_details_fiber_link', true, true, false, true, 'Layers', true, 'vw_att_details_fiber_link_audit', true, true, false, false, false, false, NULL, '', false, false, false, false, false, true, '', true, true, false, NULL, true, false, false, true, false, true, true, true, true, true, 2000, '', false, true, false, true, true, true, false, false, false, false, false, false, true, false, false, false, NULL, true, false, 'vw_att_details_fiber_link', '', 'audit_att_details_fiber_link', true, true, false, 'FIBER', 'AERIAL', 'TRANSMEDIA', 'Transmedia', 'FBR', false, NULL, '', '', true, '', '');
-------------------------------------------------------------------------------------------------------
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by,  modified_by, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES( (select layer_id from layer_details where layer_name ='FiberLink'), 'Info', 'network_id', 4, 'Network_id', 'vw_att_details_fiber_link', true, false, 1,  1, false, true, NULL);
----------------------------------------------------------------------------------------------------------
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by,  modified_by, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES( (select layer_id from layer_details where layer_name ='FiberLink'), 'Info', 'link_id', 4, 'Link_id', 'vw_att_details_fiber_link', true, false, 1,  1, false, true, NULL);

------------------------------------------------------------------------------------------

INSERT INTO public.search_settings
( layer_id, search_columns, created_by,  modified_by)
VALUES( (select layer_id from layer_details where layer_name ='FiberLink'), 'network_id,link_id', 1,  1);

-------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE VIEW public.vw_att_details_fiber_link
AS SELECT link.system_id,
    link.link_id,
    link.link_name,
    link.link_type,
    link.network_id,
    link.start_point_type,
    link.start_point_network_id,
    link.start_point_location,
    link.end_point_type,
    link.end_point_network_id,
    link.end_point_location,
    link.no_of_lmc,
    link.each_lmc_length,
    link.total_route_length,
    link.gis_length,
    link.otdr_distance,
    link.no_of_pair,
    link.tube_and_core_details,
    link.existing_route_length_otdr,
    link.new_building_route_length,
    link.otm_length,
    link.otl_length,
        CASE
            WHEN link.any_row_portion = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS any_row_portion,
    link.row_authority,
    link.total_row_segments,
    link.total_row_length,
    link.total_row_reccuring_charges,
    fn_get_date(link.handover_date, false) AS handover_date,
    fn_get_date(link.hoto_signoff_date, false) AS hoto_signoff_date,
    link.fiber_link_status,
    link.remarks,
    um.user_name AS created_by,
    fn_get_date(link.created_on) AS created_on,
    fn_get_date(link.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    um.user_id AS created_by_id,
    link.service_id,
    link.main_link_type,
    link.main_link_id,
    link.redundant_link_type,
    link.redundant_link_id,
    pm1.latitude AS start_point_latitude,
    pm1.longitude AS start_point_longitude,
    pm2.latitude AS end_point_latitude,
    pm2.longitude AS end_point_longitude,
    cbl.status, 
    cbl.region_id,
    cbl.province_id,
   fn_get_network_status(cbl.network_status) AS network_status
   FROM att_details_fiber_link link
     JOIN user_master um ON link.created_by = um.user_id
     LEFT JOIN user_master um2 ON link.modified_by = um2.user_id
     LEFT JOIN point_master pm1 ON replace(link.start_point_type::text, ' '::text, ''::text) = pm1.entity_type::text AND link.start_point_network_id::text = pm1.common_name::text
     LEFT JOIN point_master pm2 ON replace(link.end_point_type::text, ' '::text, ''::text) = pm1.entity_type::text AND link.end_point_network_id::text = pm1.common_name::text
     inner join att_details_cable_info ci on ci.link_system_id=link.system_id
	inner join att_details_cable cbl ON ci.cable_id=cbl.system_id;