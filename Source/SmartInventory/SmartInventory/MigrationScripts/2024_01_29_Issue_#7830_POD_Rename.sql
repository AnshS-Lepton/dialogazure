update layer_columns_settings set display_name = 'Area ID'
where display_name='PSA ID' and layer_id= (select layer_id from layer_details where layer_name='POD');

update layer_columns_settings set display_name = 'SubArea ID'
where display_name='FSA ID' and layer_id= (select layer_id from layer_details where layer_name='POD');