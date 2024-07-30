



update display_name_settings set display_column_name ='network_id' where
layer_id= (select layer_id from layer_details where upper(layer_name)=upper('FMS'));
