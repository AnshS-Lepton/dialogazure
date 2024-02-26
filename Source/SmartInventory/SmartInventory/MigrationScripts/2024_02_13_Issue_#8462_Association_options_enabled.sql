update layer_action_mapping set action_layer_id=0 where action_title='Associate' and action_name='ItemAssociate' and layer_id =(select layer_id from layer_details where layer_name='Trench')
