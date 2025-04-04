

--------------- Insert Topology Details and Topology Plan modules ----------------

 INSERT INTO public.module_master
(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled)
VALUES('Topology Details', 'Topology Details', '', '', NULL, now(), 287, now(), 'Web', true, 'TPL', 0, 0, false);

 INSERT INTO public.module_master
(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled)
VALUES('Topology Details', 'Topology Plan', '', '', NULL, now(), 287, now(), 'Web', true, 'TPLP', (select id from module_master where module_name='Topology Details' and module_abbr='TPL' ), 0, false);
  
  --------------- update Topology Details and Topology Plan modules ----------------
update public.module_master set parent_module_id= (select id from module_master where module_name='Topology Details' and module_abbr='TPL' ) where module_abbr='RNG' and module_name ='Ring Details';

------------------------Modify map view for the icon visiblity ----------------