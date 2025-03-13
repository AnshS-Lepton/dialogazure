

----------------------------create master data for the bom_boq_master--------------------------------

INSERT INTO public.bom_boq_master
(layer_id, entity_sub_type_column, calculated_length_column, gis_length_column, is_enabled, order_by_statement)
VALUES((select layer_id from layer_details where layer_name='PEP'), '', '', '', true, 'ENTITY_SUB_TYPE');