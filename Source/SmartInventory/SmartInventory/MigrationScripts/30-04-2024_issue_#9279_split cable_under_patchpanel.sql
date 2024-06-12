update layer_action_mapping set is_active = false where action_name = 'SplitCable' 
and layer_id in (select layer_id from layer_details where layer_name = 'PatchPanel');
