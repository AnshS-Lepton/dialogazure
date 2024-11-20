INSERT INTO public.layer_mapping(
	layer_id, parent_layer_id, parent_sequence, is_enable_inside_parent_info, is_used_for_network_id, network_code_format, is_default_code_format, is_default_parent)
	VALUES ((select layer_id from layer_details where layer_name='ADB'), 13, 8, false, true, 'MHxxxx', true,false),
	((select layer_id from layer_details where layer_name='CDB'), 13, 8, false, true, 'MHxxxx', true,false);