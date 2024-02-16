update layer_details set 
is_visible_in_mobile_lib = false
where layer_name in ('HTB', 'Customer', 'SurveyArea', 'ROW');