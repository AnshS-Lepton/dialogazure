INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) VALUES('de-DE', 'SI_OSP_GBL_NET_RPT_419', 'Fiber Link Deletion is not allowed due to its association with a cable!', false, true, 'German', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2024-12-16 16:28:08.726', 5, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) VALUES('en', 'SI_OSP_GBL_NET_RPT_419', 'Fiber Link Deletion is not allowed due to its association with a cable!', true, true, 'English', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2024-12-16 16:28:08.726', 5, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) VALUES('fr', 'SI_OSP_GBL_NET_RPT_419', 'Fiber Link Deletion is not allowed due to its association with a cable!', false, true, 'French', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2024-12-16 16:28:08.726', 5, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) VALUES('hi', 'SI_OSP_GBL_NET_RPT_419', 'Fiber Link Deletion is not allowed due to its association with a cable!', false, true, 'Hindi', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2024-12-16 16:28:08.726', 5, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) VALUES('ja-JP', 'SI_OSP_GBL_NET_RPT_419', 'Fiber Link Deletion is not allowed due to its association with a cable!', false, true, 'Japanese', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2024-12-16 16:28:08.726', 5, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) VALUES('ru-RU', 'SI_OSP_GBL_NET_RPT_419', 'Fiber Link Deletion is not allowed due to its association with a cable!', false, true, 'Russian', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2024-12-16 16:28:08.726', 5, true, false);

------------------------------------------------------
update DISPLAY_NAME_SETTINGS 
set display_column_name='area_name',display_network_name='area_name' 
where layer_id =1;

update DISPLAY_NAME_SETTINGS 
set display_column_name='surveyarea_name',display_network_name='surveyarea_name' 
where layer_id =2;

update DISPLAY_NAME_SETTINGS 
set display_column_name='building_name',display_network_name='building_name' 
where layer_id =3;

update DISPLAY_NAME_SETTINGS 
set display_column_name='subarea_name',display_network_name='subarea_name' 
where layer_id =4;

update DISPLAY_NAME_SETTINGS 
set display_column_name='structure_name',display_network_name='structure_name' 
where layer_id =5;

update DISPLAY_NAME_SETTINGS 
set display_column_name='adb_name',display_network_name='adb_name' 
where layer_id =6;

update DISPLAY_NAME_SETTINGS 
set display_column_name='bdb_name',display_network_name='bdb_name' 
where layer_id =7;

update DISPLAY_NAME_SETTINGS 
set display_column_name='cdb_name',display_network_name='cdb_name' 
where layer_id =8;

update DISPLAY_NAME_SETTINGS 
set display_column_name='splitter_name',display_network_name='splitter_name' 
where layer_id =9;

update DISPLAY_NAME_SETTINGS 
set display_column_name='pod_name',display_network_name='pod_name' 
where layer_id =10;

update DISPLAY_NAME_SETTINGS 
set display_column_name='manhole_name',display_network_name='manhole_name' 
where layer_id =13;

update DISPLAY_NAME_SETTINGS 
set display_column_name='pole_name',display_network_name='pole_name' 
where layer_id =14;

update DISPLAY_NAME_SETTINGS 
set display_column_name='tree_name',display_network_name='tree_name' 
where layer_id =15;

update DISPLAY_NAME_SETTINGS 
set display_column_name='trench_name',display_network_name='trench_name' 
where layer_id =16;

update DISPLAY_NAME_SETTINGS 
set display_column_name='duct_name',display_network_name='duct_name' 
where layer_id =17;

update DISPLAY_NAME_SETTINGS 
set display_column_name='cable_name',display_network_name='cable_name' 
where layer_id =19;

update DISPLAY_NAME_SETTINGS 
set display_column_name='spliceclosure_name',display_network_name='spliceclosure_name' 
where layer_id =20;

update DISPLAY_NAME_SETTINGS 
set display_column_name='customer_name',display_network_name='customer_name' 
where layer_id =21;

update DISPLAY_NAME_SETTINGS 
set display_column_name='mpod_name',display_network_name='mpod_name' 
where layer_id =22;

update DISPLAY_NAME_SETTINGS 
set display_column_name='ont_name',display_network_name='ont_name' 
where layer_id =23;

update DISPLAY_NAME_SETTINGS 
set display_column_name='wallmount_name',display_network_name='wallmount_name' 
where layer_id =25;

update DISPLAY_NAME_SETTINGS 
set display_column_name='room_name',display_network_name='room_name' 
where layer_id =27;

update DISPLAY_NAME_SETTINGS 
set display_column_name='htb_name',display_network_name='htb_name' 
where layer_id =28;

update DISPLAY_NAME_SETTINGS 
set display_column_name='fdb_name',display_network_name='fdb_name' 
where layer_id =29;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_id',display_network_name='network_id' 
where layer_id =35;

update DISPLAY_NAME_SETTINGS 
set display_column_name='fms_name',display_network_name='fms_name' 
where layer_id =36;

update DISPLAY_NAME_SETTINGS 
set display_column_name='coupler_name',display_network_name='coupler_name' 
where layer_id =37;

update DISPLAY_NAME_SETTINGS 
set display_column_name='row_name',display_network_name='row_name' 
where layer_id =39;

update DISPLAY_NAME_SETTINGS 
set display_column_name='patch_cord_name',display_network_name='patch_cord_name' 
where layer_id =40;

update DISPLAY_NAME_SETTINGS 
set display_column_name='equipment_name',display_network_name='equipment_name' 
where layer_id =43;

update DISPLAY_NAME_SETTINGS 
set display_column_name='dsa_name',display_network_name='dsa_name' 
where layer_id =44;

update DISPLAY_NAME_SETTINGS 
set display_column_name='csa_name',display_network_name='csa_name' 
where layer_id =45;

update DISPLAY_NAME_SETTINGS 
set display_column_name='rack_name',display_network_name='rack_name' 
where layer_id =46;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_id',display_network_name='network_id' 
where layer_id =49;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_id',display_network_name='network_id' 
where layer_id =51;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_name',display_network_name='network_name' 
where layer_id =54;

update DISPLAY_NAME_SETTINGS 
set display_column_name='name',display_network_name='name' 
where layer_id =57;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_name',display_network_name='network_name' 
where layer_id =58;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_name',display_network_name='network_name' 
where layer_id =59;

update DISPLAY_NAME_SETTINGS 
set display_column_name='province_name',display_network_name='province_name' 
where layer_id =60;

update DISPLAY_NAME_SETTINGS 
set display_column_name='name',display_network_name='name' 
where layer_id =61;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_name',display_network_name='network_name' 
where layer_id =62;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_name',display_network_name='network_name' 
where layer_id =63;

update DISPLAY_NAME_SETTINGS 
set display_column_name='cabinet_name',display_network_name='cabinet_name' 
where layer_id =65;

update DISPLAY_NAME_SETTINGS 
set display_column_name='vault_name',display_network_name='vault_name' 
where layer_id =66;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_name',display_network_name='network_name' 
where layer_id =67;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_id',display_network_name='network_id' 
where layer_id =68;

update DISPLAY_NAME_SETTINGS 
set display_column_name='handhole_name',display_network_name='handhole_name' 
where layer_id =70;

update DISPLAY_NAME_SETTINGS 
set display_column_name='patchpanel_name',display_network_name='patchpanel_name' 
where layer_id =71;

update DISPLAY_NAME_SETTINGS 
set display_column_name='restricted_area_name',display_network_name='restricted_area_name' 
where layer_id =73;

update DISPLAY_NAME_SETTINGS 
set display_column_name='network_id',display_network_name='network_id' 
where layer_id =76;