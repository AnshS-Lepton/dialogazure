update layer_columns_settings s set is_active=false 
where s.layer_id=10 and upper(s.setting_type)='REPORT' and s.display_name ilike '%pod%' and s.is_active=true 

update layer_columns_settings s set is_active=false 
where s.layer_id=10 and upper(s.setting_type)=upper('Info') and s.is_active=true and s.display_name ilike '%pod%'

update layer_columns_settings s set is_active=true,display_name='Site Type'
where s.layer_id=10 and upper(s.setting_type)=upper('Info') and s.column_name ilike 'pod_type'

update layer_columns_settings s set is_active=true,display_name='Site Type'
where s.layer_id=10 and upper(s.setting_type)=upper('REPORT') and s.column_name ilike 'pod_type'