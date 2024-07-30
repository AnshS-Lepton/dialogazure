INSERT INTO public.associate_entity_master(
 layer_id, associate_layer_id, layer_subtype, is_enabled, created_on, created_by, is_snapping_enabled, is_shifting_allowed)
	VALUES ((Select layer_id from layer_details where layer_name='Cable'), (Select layer_id from layer_details where layer_name='SpliceClosure'), 'Aerial (Structured)', true, now(), 0, false, false),
           ((Select layer_id from layer_details where layer_name='Cable'), (Select layer_id from layer_details where layer_name='SpliceClosure'), 'Aerial (Unstructured)', true, now(), 0, false, false);
           
  