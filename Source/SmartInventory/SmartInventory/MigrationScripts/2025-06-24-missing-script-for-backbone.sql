CREATE OR REPLACE FUNCTION public.fn_backbone_plan_auto_create_trench(p_a_system_id integer, p_a_entity_type character varying, p_a_network_id character varying, p_b_system_id integer, p_b_entity_type character varying, p_b_network_id character varying, p_longlat character varying, p_structure_id integer, p_region_id integer, p_province_id integer, p_user_id integer, p_rfs_type character varying, p_plan_id integer, p_audit_item_master_id integer)
 RETURNS record
 LANGUAGE plpgsql
AS $function$
DECLARE
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow_cable record;
v_arow_network_id record;
v_arow_trench record;
v_arow_duct record;
v_cable_display_value CHARACTER VARYING;
v_a_display_value CHARACTER VARYING;
v_b_display_value CHARACTER VARYING;
v_display_name CHARACTER VARYING;
v_display_value CHARACTER VARYING;
BEGIN
V_IS_VALID:=true;
V_MESSAGE:='Successfully';

		/*==================================================Place the trench================================================================================= */


	
	select * into v_arow_network_id from fn_get_line_network_code('Trench'::character varying,p_a_network_id,p_b_network_id,p_longlat,'OSP'::character varying,0,'');

	raise info 'v_arow_network_id %' ,v_arow_network_id;
	
	insert into att_details_trench(is_visible_on_map,audit_item_master_id,network_id,trench_name,a_system_id,a_location,a_entity_type,b_system_id,b_location,b_entity_type,trench_length,trench_width,trench_height,customer_count,utilization,trench_type,no_of_ducts,
	sequence_id,network_status,status,pin_code,province_id,region_id,construction,activation,accessibility,specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,type,
	brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,mcgm_ward,strata_type,
	parent_system_id,parent_network_id,parent_entity_type,is_used,source_ref_type,source_ref_id,ownership_type)

	select true,p_audit_item_master_id,v_arow_network_id.network_code,v_arow_network_id.network_code,p_a_system_id,p_a_network_id,p_a_entity_type,p_b_system_id,p_b_network_id,p_b_entity_type,
	ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),0,0,0,0,'HDD',0,v_arow_network_id.sequence_id,'P','A','',p_province_id,p_region_id,0,0,0,
	specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,0,0,0,'',p_user_id,now(),0,0,0,0,'','',v_arow_network_id.parent_system_id,
	v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,false,'backbone planning',p_plan_id,'OWN'
	from item_template_trench where created_by=p_user_id limit 1 RETURNING system_id,network_id into v_arow_trench;

	INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name)
	values(v_arow_trench.SYSTEM_ID,'Trench','A',ST_GeomFromText('LINESTRING('||p_longlat||')',4326),'','',p_user_id,0,v_arow_trench.network_id,'P',false,fn_get_display_name(v_arow_trench.SYSTEM_ID,'Trench'));

	
	select display_name into v_a_display_value from point_master where system_id=p_a_system_id and upper(entity_type)=upper(p_a_entity_type);
	select display_name into v_b_display_value from point_master where system_id=p_b_system_id and upper(entity_type)=upper(p_b_entity_type);


	 insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,associated_entity_type,associated_display_name,created_on,created_by,is_termination_point)
	values(v_arow_trench.system_id,v_arow_trench.network_id,'Trench',v_display_value,p_a_system_id,p_a_network_id,p_a_entity_type,v_a_display_value,now(),p_user_id,true),
	(v_arow_trench.system_id,v_arow_trench.network_id,'Trench',v_display_value,p_b_system_id,p_b_network_id,p_b_entity_type,v_b_display_value,now(),p_user_id,true);


				
		
	RETURN v_arow_trench;          
END
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_plan_auto_create_trench(int4, varchar, varchar, int4, varchar, varchar, varchar, int4, int4, int4, int4, varchar, int4, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_plan_auto_create_trench(int4, varchar, varchar, int4, varchar, varchar, varchar, int4, int4, int4, int4, varchar, int4, int4) TO postgres;


------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_plan_auto_create_duct(p_a_system_id integer, p_a_entity_type character varying, p_a_network_id character varying, p_b_system_id integer, p_b_entity_type character varying, p_b_network_id character varying, p_longlat character varying, p_structure_id integer, p_region_id integer, p_province_id integer, p_user_id integer, p_rfs_type character varying, p_plan_id integer, p_audit_item_master_id integer, p_trench_id integer, p_trench_network_id character varying)
 RETURNS record
 LANGUAGE plpgsql
AS $function$
DECLARE
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow_cable record;
v_arow_network_id record;
v_arow_duct record;
v_cable_display_value CHARACTER VARYING;
v_a_display_value CHARACTER VARYING;
v_b_display_value CHARACTER VARYING;
v_display_name CHARACTER VARYING;
v_display_value CHARACTER VARYING;
v_trench_value character varying;
BEGIN
V_IS_VALID:=true;
V_MESSAGE:='Successfully';

raise info ' test p_a_system_id=%',p_a_system_id;
raise info ' tesat p_b_system_id=%',p_b_system_id;


/*==================================================Place the duct================================================================================= */
	select * into v_arow_network_id from fn_get_line_network_code('Duct',p_a_network_id,p_b_network_id,p_longlat,'OSP');

	raise info 'v_arow_network_id =%',v_arow_network_id;
	
	insert into att_details_duct(is_visible_on_map,audit_item_master_id,network_id,duct_name,a_system_id,a_location,a_entity_type,b_system_id,b_location,b_entity_type,calculated_length,manual_length,sequence_id,network_status,
	status,pin_code,province_id,region_id,utilization,no_of_cables,offset_value,construction,activation,accessibility,specification,category,subcategory1,subcategory2,
	subcategory3,item_code,vendor_id,type,brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,inner_dimension,outer_dimension,
	parent_system_id,parent_network_id,parent_entity_type,is_used,source_ref_type,source_ref_id,trench_id,ownership_type)

	select true,p_audit_item_master_id,v_arow_network_id.network_code,v_arow_network_id.network_code,p_a_system_id,p_a_network_id,p_a_entity_type,p_b_system_id,p_b_network_id,p_b_entity_type,
	ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),v_arow_network_id.sequence_id,'P','A','',
	p_province_id,p_region_id,0,0,0,0,0,0,specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id,0,0,0,'',p_user_id,now(),0,0,0,0,0,0,
	v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
	false,'backbone planning',p_plan_id,0,'Own'
	from item_template_duct itm where created_by=p_user_id limit 1 RETURNING system_id,network_id into v_arow_duct;

	
	INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name)
	values(v_arow_duct.SYSTEM_ID,'Duct','A',ST_GeomFromText('LINESTRING('||p_longlat||')',4326),'','',p_user_id,0,v_arow_duct.network_id,'P',false,fn_get_display_name(v_arow_duct.SYSTEM_ID,'Duct'));

	
	select display_name into v_a_display_value from point_master where system_id=p_a_system_id and upper(entity_type)=upper(p_a_entity_type);
	select display_name into v_b_display_value from point_master where system_id=p_b_system_id and upper(entity_type)=upper(p_b_entity_type);


	insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,associated_entity_type,associated_display_name,created_on,created_by,is_termination_point)
	values(v_arow_duct.system_id,v_arow_duct.network_id,'Duct',v_display_value,p_a_system_id,p_a_network_id,p_a_entity_type,v_a_display_value,now(),p_user_id,true),
	(v_arow_duct.system_id,v_arow_duct.network_id,'Duct',v_display_value,p_b_system_id,p_b_network_id,p_b_entity_type,v_b_display_value,now(),p_user_id,true);

	if(p_trench_id>0)
	then	
		
		insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
		entity_type,created_on,created_by,associated_display_name,entity_display_name)
		values('Duct',v_arow_duct.system_id,v_arow_duct.network_id,p_trench_network_id,p_trench_id,'Trench',now(),p_user_id,
		fn_get_display_name(p_trench_id,'Trench'),fn_get_display_name(p_trench_id,'Trench')); 
		
	end if;
				
		
	RETURN v_arow_duct;          
END
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_plan_auto_create_duct(int4, varchar, varchar, int4, varchar, varchar, varchar, int4, int4, int4, int4, varchar, int4, int4, int4, varchar) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_plan_auto_create_duct(int4, varchar, varchar, int4, varchar, varchar, varchar, int4, int4, int4, int4, varchar, int4, int4, int4, varchar) TO postgres;
