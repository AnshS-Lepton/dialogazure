insert into associate_entity_master(layer_id,associate_layer_id,layer_subtype,is_enabled,created_on,created_by,
									modified_on,modified_by,is_snapping_enabled,is_shifting_allowed)
									values((select layer_id from layer_details where layer_name='PatchPanel'),
										   (select layer_id from layer_details where layer_name='BDB'),
										   '','true',now(),0,now(),0,'false','false')