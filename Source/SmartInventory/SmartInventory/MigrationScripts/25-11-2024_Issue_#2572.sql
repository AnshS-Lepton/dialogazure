update layer_action_mapping set is_active =false where action_title='Create route id' 
-------------#2458
update file_type_master set is_active=false;
update file_type_master set is_active=true where file_type in (select file_type from file_type_master where file_type ilike '%SH%');
update file_type_master set is_active=true where file_type in (select file_type from file_type_master where file_type ilike '%EX%');
update file_type_master set is_active=true where file_type in (select file_type from file_type_master where file_type ilike '%KML%');