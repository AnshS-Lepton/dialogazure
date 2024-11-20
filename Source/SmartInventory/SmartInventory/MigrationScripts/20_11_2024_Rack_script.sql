CREATE OR REPLACE FUNCTION public.fn_get_equipment_export_detail(vtype text, vid text, p_parent_type text, p_parent_id text)
 RETURNS TABLE(equipment_type text, equipment_id text, equipment_name text, parent_type text, parent_id text, address text, vender text, item_code text, specification text, description text, system_id integer)
 LANGUAGE plpgsql
AS $function$

BEGIN 
	
create temp table temp_result(t_equipment_type text, t_equipment_id text,t_equipment_name text,t_parent_type text,t_parent_id text,t_address text,t_vender text,t_item_code text,t_specification text,t_description text,t_system_id integer) on commit drop ;
if(vtype='POP') then 
WITH RECURSIVE cte_name AS(
			  select 'Rack'::text as r_equipment_type,r.network_id as r_equipment_id ,r.rack_name as r_equipment_name,
			  fn_get_parent_type(r.parent_entity_type) as r_parent_type ,r.parent_network_id as r_parent_id,p.address::text as r_address,
			  vm.name::text as r_vender,r.item_code as r_item_code,r.specification::text as r_specification ,
			  r.system_id as r_system_id ,fn_get_equipment_export_detail_description('Rack',r.system_id) as r_description 
			  from att_details_rack r
			  join vendor_master vm on r.vendor_id=vm.id
			  join att_details_pod p on p.network_id =r.parent_network_id 
			  where r.parent_network_id =vid or r.network_id =vid or r.parent_system_id::text =vid or r.system_id::text =vid
		UNION 
			  select m.equipment_type, m.equipment_id ,m.equipment_name ,m.parent_type ,m.parent_id ,cte_name.r_address,m.Vender::text,m.item_code,m.specification ::text ,
			   m.system_id as r_system_id,fn_get_equipment_export_detail_description(m.equipment_type,m.system_id) as r_description
			  from vw_att_details_model m join cte_name on m.rack_id =cte_name.r_system_id
		) 
		 insert into temp_result
		 (SELECT r_equipment_type , r_equipment_id ,r_equipment_name ,r_parent_type ,r_parent_id ,r_address ,r_vender ,r_item_code ,r_specification, r_description,r_system_id FROM cte_name
	     UNION
			select vm.equipment_type as r_equipment_type, vm.equipment_id as r_equipment_id,vm.equipment_name as r_equipment_name  ,fn_get_parent_type(vm.parent_type) as r_parent_type,vm.parent_id as r_parent_id,'' as r_address,vm.vender as r_vender ,vm.item_code as r_item_code ,vm.specification as r_specification,fn_get_equipment_export_detail_description('equipment',vm.system_id) as r_description,vm.system_id as r_system_id   from vw_att_details_model vm where vm.parent_type  = case when vtype='POP' then 'POD' else vtype end and  vm.rack_id = 0 and vm.parent_system_id::text = vid);

	elsif(vtype='Cabinet') then 
	WITH RECURSIVE cte_name AS(
			  select 'Rack'::text as r_equipment_type,r.network_id as r_equipment_id ,r.rack_name as r_equipment_name,
			  r.parent_entity_type as r_parent_type ,r.parent_network_id as r_parent_id,p.address::text as r_address,
			  vm.name::text as r_vender,r.item_code as r_item_code,r.specification::text as r_specification ,
			  r.system_id as r_system_id ,fn_get_equipment_export_detail_description('Rack',r.system_id) as r_description 
			  from att_details_rack r
			  join vendor_master vm on r.vendor_id=vm.id
			  join att_details_cabinet p on p.network_id =r.parent_network_id 
			  where r.parent_network_id =vid or r.network_id =vid or r.parent_system_id::text =vid or r.system_id::text =vid
		UNION 
			  select m.equipment_type, m.equipment_id ,m.equipment_name ,m.parent_type ,m.parent_id ,cte_name.r_address,m.Vender::text,m.item_code,
			  m.specification ::text , m.system_id as r_system_id,fn_get_equipment_export_detail_description(m.equipment_type,m.system_id) as r_description
			  from vw_att_details_model m join cte_name on m.rack_id =cte_name.r_system_id
		) 
		 insert into temp_result
		 (SELECT r_equipment_type,r_equipment_id,r_equipment_name,r_parent_type,r_parent_id,r_address,r_vender,r_item_code,r_specification,r_description,r_system_id 
		 FROM cte_name
	     UNION
			select vm.equipment_type as r_equipment_type, vm.equipment_id as r_equipment_id,vm.equipment_name as r_equipment_name  ,vm.parent_type as r_parent_type,vm.parent_id as r_parent_id,'' as r_address,vm.vender as r_vender ,vm.item_code as r_item_code ,vm.specification as r_specification,fn_get_equipment_export_detail_description('equipment',vm.system_id) as r_description,vm.system_id as r_system_id   from vw_att_details_model vm where vm.parent_type  = vtype  and  vm.rack_id = 0 and vm.parent_system_id::text = vid);	
		
elsif (vtype  = 'Rack') then

		if(vid   = '999999') then
		      insert into temp_result
		      select vm.equipment_type as r_equipment_type, vm.equipment_id as r_equipment_id,vm.equipment_name as r_equipment_name  ,vm.parent_type as r_parent_type,vm.parent_id as r_parent_id,'' as                      r_address,vm.vender as r_vender ,vm.item_code as r_item_code ,vm.specification as r_specification,fn_get_equipment_export_detail_description('equipment',vm.system_id) as r_description,                      vm.system_id as r_system_id   from vw_att_details_model vm where vm.parent_type  = p_parent_type and  vm.rack_id = 0 and vm.parent_system_id::text = p_parent_id;

		else
		 
		WITH RECURSIVE cte_name AS(
					  select 'Rack'::text as r_equipment_type,r.network_id as r_equipment_id ,r.rack_name as r_equipment_name,
					  r.parent_entity_type as r_parent_type ,r.parent_network_id as r_parent_id,'' as r_address,
					  vm.name::text as r_vender,r.item_code as r_item_code,r.specification::text as r_specification ,
					  r.system_id as r_system_id ,fn_get_equipment_export_detail_description('Rack',r.system_id) as r_description 
					  from att_details_rack r
					  join vendor_master vm on r.vendor_id=vm.id
					  --join att_details_pod p on p.network_id =r.parent_network_id 
					  where r.system_id::text =vid
				UNION 
					  select m.equipment_type, m.equipment_id ,m.equipment_name ,m.parent_type ,m.parent_id ,cte_name.r_address,m.Vender::text,m.item_code,m.specification ::text ,
					   m.system_id as r_system_id,fn_get_equipment_export_detail_description(m.equipment_type,m.system_id) as r_description
					  from vw_att_details_model m join cte_name on m.rack_id =cte_name.r_system_id
				) 
				 insert into temp_result
				 (SELECT r_equipment_type , r_equipment_id ,r_equipment_name ,r_parent_type ,r_parent_id ,r_address ,r_vender ,r_item_code ,r_specification, r_description,r_system_id FROM                                 cte_name
				 UNION
					select vm.equipment_type as r_equipment_type, vm.equipment_id as r_equipment_id,vm.equipment_name as r_equipment_name  ,vm.parent_type as r_parent_type,vm.parent_id as                                         r_parent_id,'' as r_address,vm.vender as r_vender ,vm.item_code as r_item_code ,vm.specification as r_specification,fn_get_equipment_export_detail_description(                                         'equipment',vm.system_id) as             r_description,vm.system_id as r_system_id   from vw_att_details_model vm where vm.parent_type  = case when vtype='POP' then                                         'POD' else vtype end and  vm.rack_id = 0 and vm.parent_system_id::text = vid);
				

			end if;	



		
 elsif(vtype='Floor' or vtype='UNIT') then 
     WITH RECURSIVE cte_name AS(
  
	  select 'Rack'::text as r_equipment_type,r.network_id as r_equipment_id ,r.rack_name as r_equipment_name,
			  r.parent_entity_type as r_parent_type ,r.parent_network_id as r_parent_id,'' as r_address,
			  vm.name::text as r_vender,r.item_code as r_item_code,r.specification::text as r_specification ,
			  r.system_id as r_system_id ,fn_get_equipment_export_detail_description('Rack',r.system_id) as r_description 
			  from att_details_rack r
			  join vendor_master vm on r.vendor_id=vm.id
	  where r.parent_network_id =vid or r.network_id =vid or r.parent_system_id::text =vid or r.system_id::text =vid 
        UNION 
			  select m.equipment_type, m.equipment_id ,m.equipment_name ,m.parent_type ,m.parent_id ,cte_name.r_address,m.Vender::text,m.item_code,m.specification ::text ,
			   m.system_id as r_system_id,fn_get_equipment_export_detail_description(m.equipment_type,m.system_id) as r_description
			  from vw_att_details_model m join cte_name on m.rack_id =cte_name.r_system_id
		) 
		 insert into temp_result
		 (SELECT r_equipment_type , r_equipment_id ,r_equipment_name ,r_parent_type ,r_parent_id ,r_address ,r_vender ,r_item_code ,r_specification, r_description,r_system_id FROM cte_name
		  UNION
		select vm.equipment_type as r_equipment_type, vm.equipment_id as r_equipment_id,vm.equipment_name as r_equipment_name  ,vm.parent_type as r_parent_type,vm.parent_id as r_parent_id,'' as r_address,vm.vender as r_vender ,vm.item_code as r_item_code ,vm.specification as r_specification,fn_get_equipment_export_detail_description('equipment',vm.system_id) as r_description,vm.system_id as r_system_id   from vw_att_details_model vm where vm.parent_type = vtype and  vm.rack_id = 0 and vm.parent_system_id::text = vid);

 
else
insert into temp_result
select m.equipment_type as r_equipment_type, m.equipment_id as r_equipment_id,m.equipment_name as r_equipment_name ,
m.parent_type as r_parent_type ,m.parent_id as r_parent_id ,'' as r_address,--p.address::text as r_address,
m.Vender::text as r_vender,m.item_code as r_item_code,m.specification ::text as r_specification ,
fn_get_equipment_export_detail_description(m.equipment_type,m.system_id) as r_description, m.system_id as r_system_id
	  from vw_att_details_model m  --join att_details_pod p on p.network_id =m.parent_id 
	  where m.equipment_id =vid or m.system_id::text=vid;

end if;
return query select t_equipment_type , t_equipment_id ,t_equipment_name ,t_parent_type ,t_parent_id ,t_address ,t_vender ,t_item_code ,t_specification,t_description,t_system_id  from temp_result;
END;
$function$
;
-------------------------------------------------------------------------------------------------

drop view vw_att_details_rack;
----------------------------------------------------------------
CREATE OR REPLACE VIEW public.vw_att_details_rack
AS SELECT rack.system_id,
    rack.network_id,
    rack.rack_name,
    rack.rack_type,
    round(rack.length::numeric, 2) AS length,
    round(rack.width::numeric, 2) AS width,
    round(rack.height::numeric, 2) AS height,
    round(rack.border_width::numeric, 2) AS border_width,
    rack.no_of_units,
    rack.structure_id,
    round(rack.latitude::numeric, 6) AS latitude,
    round(rack.longitude::numeric, 6) AS longitude,
    rack.province_id,
    rack.region_id,
    rack.parent_system_id,
    rack.parent_network_id,
    fn_get_parent_type(rack.parent_entity_type) AS parent_entity_type,
    rack.status,
    round(rack.pos_x::numeric, 2) AS pos_x,
    round(rack.pos_y::numeric, 2) AS pos_y,
    round(rack.pos_z::numeric, 2) AS pos_z,
    rack.item_code,
    rack.vendor_id,
    rack.specification,
    rack.category,
    rack.subcategory1,
    rack.subcategory2,
    rack.subcategory3,
    rack.sequence_id,
    rack.created_by,
    rack.modified_by,
    rack.network_status,
    rack.project_id,
    rack.planning_id,
    rack.purpose_id,
    rack.workorder_id,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    fn_get_date(rack.created_on) AS created_on,
    fn_get_date(rack.modified_on) AS modified_on,
    vm.name AS vendor_name,
    rack.is_visible_on_map,
    rack.is_new_entity,
    rack.remarks,
    rack.audit_item_master_id,
    rack.origin_from,
    rack.origin_ref_id,
    rack.origin_ref_code,
    rack.origin_ref_description,
    rack.request_ref_id,
    rack.requested_by,
    rack.request_approved_by,
    rack.subarea_id,
    rack.area_id,
    rack.dsa_id,
    rack.csa_id,
    rack.bom_sub_category,
    rack.gis_design_id,
    rack.st_x,
    rack.st_y,
    rack.ne_id,
    rack.prms_id,
    rack.jc_id,
    rack.mzone_id,
    rack.own_vendor_id
   FROM att_details_rack rack
     JOIN user_master um ON rack.created_by = um.user_id
     JOIN vendor_master vm ON rack.vendor_id = vm.id
     LEFT JOIN user_master um2 ON rack.modified_by = um2.user_id;