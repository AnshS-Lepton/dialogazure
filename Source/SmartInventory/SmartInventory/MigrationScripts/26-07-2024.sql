update dropdown_master set dropdown_value='1' where dropdown_key='Completed' and  layer_id=(select layer_id from layer_details where upper(layer_name)='TRENCH') and dropdown_type='Model';
update dropdown_master set dropdown_value='2' where dropdown_key='BOQ' and  layer_id=(select layer_id from layer_details where upper(layer_name)='TRENCH') and dropdown_type='Model';
update dropdown_master set dropdown_value='3' where dropdown_key='Desktop design' and  layer_id=(select layer_id from layer_details where upper(layer_name)='TRENCH') and dropdown_type='Model';
update dropdown_master set dropdown_value='4' where dropdown_key='NFA Approval Done' and  layer_id=(select layer_id from layer_details where upper(layer_name)='TRENCH') and dropdown_type='Model';


update dropdown_master set dropdown_value='1' where dropdown_key='Completed' and  layer_id=(select layer_id from layer_details where upper(layer_name)='DUCT') and dropdown_type='Model';
update dropdown_master set dropdown_value='2' where dropdown_key='BOQ' and  layer_id=(select layer_id from layer_details where upper(layer_name)='DUCT') and dropdown_type='Model';
update dropdown_master set dropdown_value='3' where dropdown_key='Desktop design' and  layer_id=(select layer_id from layer_details where upper(layer_name)='DUCT') and dropdown_type='Model';
update dropdown_master set dropdown_value='4' where dropdown_key='NFA Approval Done' and  layer_id=(select layer_id from layer_details where upper(layer_name)='DUCT') and dropdown_type='Model';

update dropdown_master set dropdown_value='1' where dropdown_key='Completed' and  layer_id=(select layer_id from layer_details where upper(layer_name)='CABLE') and dropdown_type='Model';
update dropdown_master set dropdown_value='2' where dropdown_key='BOQ' and  layer_id=(select layer_id from layer_details where upper(layer_name)='CABLE') and dropdown_type='Model';
update dropdown_master set dropdown_value='3' where dropdown_key='Desktop design' and  layer_id=(select layer_id from layer_details where upper(layer_name)='CABLE') and dropdown_type='Model';
update dropdown_master set dropdown_value='4' where dropdown_key='NFA Approval Done' and  layer_id=(select layer_id from layer_details where upper(layer_name)='CABLE') and dropdown_type='Model';