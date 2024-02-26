insert into module_master(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES('Bulk Splicing DataUploader', 'Bulk Splicing DataUploader', NULL, NULL, NULL, Now(), 287, Now(), 'Web', true, 'BLK-SDU', (select  id from module_master where module_name='Wireline Tools'), 2, false, NULL, NULL);

insert into role_module_mapping(role_id, module_id)
values(1, (select id from module_master where module_name = 'Bulk Splicing DataUploader'))

insert into user_module_mapping(user_id, module_id, created_by, created_on, modified_by, modified_on)
values(1, (select id from module_master where module_name = 'Bulk Splicing DataUploader'), 1, now(), 1, now() )