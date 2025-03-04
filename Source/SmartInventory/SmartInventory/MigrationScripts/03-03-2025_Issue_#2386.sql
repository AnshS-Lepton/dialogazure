

----------------------------Disable action ItemAssociate for WallMount-----------------------------------

update layer_action_mapping set is_active=false  where layer_id=(select layer_id from layer_details where layer_name='WallMount') and action_name='ItemAssociate';
