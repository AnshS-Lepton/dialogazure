



update layer_mapping set network_code_format='nnn' where
layer_id =(Select layer_id from  layer_details where UPPER(layer_name) = UPPER('Structure'));