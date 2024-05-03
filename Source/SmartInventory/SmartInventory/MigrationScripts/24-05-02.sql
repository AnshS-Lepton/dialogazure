delete from legend_details where layer_id=(select layer_id from layer_details where layer_name='Sector');

INSERT INTO legend_details ( layer_id, group_name, sub_layer, icon_path, created_by, created_on, modified_by, modified_on, "type", color_code, layer_sub_column, network_type, is_active, sequence_id) VALUES((select layer_id from layer_details where layer_name='Sector'), 'Sector', 'As-Built', 'As-built_sector.svg', 1, '2021-03-02 11:53:09.024', NULL, NULL, 'Web', NULL, 'As-build', NULL, true, NULL);
INSERT INTO legend_details ( layer_id, group_name, sub_layer, icon_path, created_by, created_on, modified_by, modified_on, "type", color_code, layer_sub_column, network_type, is_active, sequence_id) VALUES((select layer_id from layer_details where layer_name='Sector'), 'Sector', 'Planned', 'planned_sector.svg', 1, '2021-03-02 11:53:09.087', NULL, NULL, 'Web', NULL, 'planned', NULL, true, NULL);
INSERT INTO legend_details ( layer_id, group_name, sub_layer, icon_path, created_by, created_on, modified_by, modified_on, "type", color_code, layer_sub_column, network_type, is_active, sequence_id) VALUES((select layer_id from layer_details where layer_name='Sector'), 'Sector', 'Dorment', 'dormant_sector.svg', 1, '2021-03-02 11:53:09.055', NULL, NULL, 'Web', NULL, 'dorment', 'Dorment', true, NULL);


update layer_details set is_visible_in_mobile_lib=false where layer_id=(select layer_id from layer_details where layer_name='LandBase');
update layer_action_mapping set is_active=false where layer_id=(select layer_id from layer_details where layer_name='CSA') and is_mobile_action=true and action_name='ConvertToDormant';

update layer_action_mapping set is_active=false where layer_id=(select layer_id from layer_details where layer_name='DSA') and is_mobile_action=true and action_name='ConvertToDormant';
update layer_action_mapping set is_active=false where layer_id=(select layer_id from layer_details where layer_name='Area') and is_mobile_action=true and action_name='ConvertToDormant';
update layer_action_mapping set is_active=false where layer_id=(select layer_id from layer_details where layer_name='SubArea') and is_mobile_action=true and action_name='ConvertToDormant';

update layer_details set is_network_ticket_entity=false where layer_name='RestrictedArea';

Update layer_columns_settings set display_name='EVO Id' where layer_id=(select layer_id from layer_details where layer_name='Cable') and display_name='Evoid';
update res_resources set value='ADD ODF' where key='SI_OSP_GBL_NET_FRM_258' and culture='en';

update layer_action_mapping set is_mobile_action=false where layer_id=(select layer_id from layer_details where layer_name='Tree') and action_name='Barcode';

update layer_columns_settings set display_name='Router No' where display_name ='Wallmount No';
update layer_columns_settings set display_name='Router Name' where display_name='WallMount Name';

update layer_action_mapping set is_mobile_action=false where layer_id=(select layer_id from layer_details where layer_name='Loop') and action_name='Barcode';

update module_master set is_active=true where module_name ='Approve/Reject Building';
update layer_mapping set network_code_format='nnn' where layer_id=(select layer_id from layer_details where layer_name='Structure');

update layer_details set is_mobile_layer=false,is_visible_in_mobile_lib=false where layer_name in('Tower','Fault','Microduct','PatchPanel','Equipment','LandBase');

update layer_columns_settings set display_name='FAT Name' where display_name='Fdb Name';

update layer_columns_settings set display_name='ODF Name' where display_name='Fms Name';

update layer_columns_settings set display_name='Hm ODF' where display_name='Hm Fms';

update layer_mapping set network_code_format='nnn' where layer_id=(select layer_id from layer_details where layer_name='Structure');