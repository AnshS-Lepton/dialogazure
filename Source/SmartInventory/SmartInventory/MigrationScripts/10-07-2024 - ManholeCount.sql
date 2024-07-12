ALTER TABLE att_details_cable ADD COLUMN manhole_count integer;


insert into res_resources (culture,key,value,is_default_lang,is_visible,language,description,modified_by,created_by)
values('en','SI_OSP_CAB_NET_FRM_077','Manhole Count',true,true,'English','Smart Inventory_Osp_Cable_Dot Net_',0)