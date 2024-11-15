update layer_details set layer_title='ODF' where layer_name='FMS';
update layer_mapping set network_code_format='ODFnn' where layer_id=36 and parent_layer_id=10;