insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'Strata_Type','Hard Soil',true,1,now(),'Hard Soil','Strata_Type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'Strata_Type','Soft Soil',true,1,now(),'Soft Soil','Strata_Type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'Strata_Type','Generic',true,1,now(),'Generic','Strata_Type',true,true,0 from layer_details where upper(layer_name)='TRENCH';


insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'trench_serving_type','Back Bom',true,1,now(),'Back Bom','trench_serving_type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'trench_serving_type','Access',true,1,now(),'Access','trench_serving_type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'trench_serving_type','Back Bom LM',true,1,now(),'Back Bom LM','trench_serving_type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'trench_serving_type','Access LM',true,1,now(),'Access LM','trench_serving_type',true,true,0 from layer_details where upper(layer_name)='TRENCH'; 


alter table att_details_cable add a_location_code character varying, 
add b_location_code character varying;


alter table vendor_master add abbr character varying(20);