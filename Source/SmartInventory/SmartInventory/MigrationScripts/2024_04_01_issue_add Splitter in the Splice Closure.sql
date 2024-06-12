
INSERT INTO public.layer_action_mapping
(layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id)
VALUES(20, 'PrimaryAddSplitter', true, 10, 'Add Splitter', true, false, true, 'A', 9, 0, true, true, 0, 'SI_OSP_GBL_NET_FRM_256', false, 0);


INSERT INTO public.layer_action_mapping
(layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id)
VALUES(20,'SplitterTemplate', true, 11, 'Splitter Template', true, false, true, 'T', 9, 0, false, true, 0, 'SI_OSP_GBL_NET_FRM_257', false, 0);


INSERT INTO public.layer_mapping
(layer_id, parent_layer_id, parent_sequence, is_enable_inside_parent_info, is_used_for_network_id, network_code_format, is_default_code_format, is_default_parent)
VALUES(9, 20, 1, true, true, 'SPLnn', true,false);
