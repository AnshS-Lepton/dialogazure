insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
select layer_id,'status','status',0,true,'status' from layer_details where
is_visible_in_ne_library=true and layer_name not in ('Other','Province','Loop','SpliceTray','ProjectArea','Restricted_Area',
'Fault',
'LandBase','Clamp','PIT') and layer_id not in (select layer_id from LABEL_COLUMN_SETTINGS where column_name='status');