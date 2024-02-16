update LAYER_COLUMNS_SETTINGS set display_name = 'Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'FMS') 
and setting_type = 'Info' and column_name = 'area_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Sub Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'FMS') 
and setting_type = 'Info' and column_name = 'subarea_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'FDB') 
and setting_type = 'Info' and column_name = 'area_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Sub Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'FDB') 
and setting_type = 'Info' and column_name = 'subarea_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'BDB') 
and setting_type = 'Info' and column_name = 'area_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Sub Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'BDB') 
and setting_type = 'Info' and column_name = 'subarea_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'BDB') 
and setting_type = 'History' and column_name = 'area_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Sub Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'BDB') 
and setting_type = 'History' and column_name = 'subarea_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'FDB') 
and setting_type = 'History' and column_name = 'area_id';
update LAYER_COLUMNS_SETTINGS set display_name = 'Sub Area ID' where  layer_id in
(select layer_id from layer_details where layer_name = 'FDB') 
and setting_type = 'History' and column_name = 'subarea_id';




