	
	
	
	insert into form_input_settings(form_name,form_feature_name,form_feature_type, is_active,is_required,created_by, created_on
	) values('Cable','a_location_code','field',true,false,1,now());

insert into form_input_settings(form_name,form_feature_name,form_feature_type, is_active,is_required,created_by, created_on
	) values('Cable','b_location_code','field',true,false,1,now());
	
	
	
insert into form_input_settings(form_name,form_feature_name,form_feature_type, is_active,is_required,created_by, created_on
	) values('Trench','a_location_code','field',true,false,1,now());

insert into form_input_settings(form_name,form_feature_name,form_feature_type, is_active,is_required,created_by, created_on
	) values('Trench','b_location_code','field',true,false,1,now());


insert into form_input_settings(form_name,form_feature_name,form_feature_type, is_active,is_required,created_by, created_on
	) values('Duct','a_location_code','field',true,false,1,now());

insert into form_input_settings(form_name,form_feature_name,form_feature_type, is_active,is_required,created_by, created_on
	) values('Duct','b_location_code','field',true,false,1,now());
	
	
	insert into form_input_settings(form_name,form_feature_name,form_feature_type, is_active,is_required,created_by, created_on
	) values('FMS','installation_location_code','field',true,false,1,now());

insert into form_input_settings(form_name,form_feature_name,form_feature_type, is_active,is_required,created_by, created_on
	) values('FMS','fms_type','field',true,false,1,now());
	
	
	----------------------Below query only for VI.-------------------------------------------------
	update form_input_settings set is_required= true where form_name ='Cable' and form_feature_name ='a_location_code' ;
update form_input_settings set is_required= true where form_name ='Cable' and form_feature_name ='b_location_code' ;
update form_input_settings set is_required= true where form_name ='Trench' and form_feature_name ='a_location_code' ;
update form_input_settings set is_required= true where form_name ='Trench' and form_feature_name ='b_location_code' ;
update form_input_settings set is_required= true where form_name ='Duct' and form_feature_name ='a_location_code' ;
update form_input_settings set is_required= true where form_name ='Duct' and form_feature_name ='b_location_code' ;
update form_input_settings set is_required= true where form_name ='FMS' and form_feature_name ='installation_location_code' ;
update form_input_settings set is_required= true where form_name ='FMS' and form_feature_name ='fms_type' ;