alter table audit_att_details_cable  add column route_name varchar(20);

---------------------------------------------------------------------------

alter table att_details_cable add column route_name varchar(20);

-------------------------------------------------------------------------
-- public.vw_att_details_cable source

CREATE OR REPLACE VIEW public.vw_att_details_cable
AS SELECT cable.system_id,
    cable.network_id,
    cable.cable_name,
    fn_get_display_name(cable.a_system_id, cable.a_entity_type) AS a_location,
    fn_get_display_name(cable.b_system_id, cable.b_entity_type) AS b_location,
    cable.total_core,
    cable.no_of_tube,
    cable.no_of_core_per_tube,
    round(cable.cable_measured_length::numeric, 2) AS cable_measured_length,
    round(cable.cable_calculated_length::numeric, 2) AS cable_calculated_length,
    cable.cable_type,
    cable.coreaccess,
    cable.wavelength,
    cable.optical_output_power,
    cable.frequency,
    cable.attenuation_db,
    cable.resistance_ohm,
    cable.construction,
    cable.activation,
    cable.accessibility,
    cable.specification,
    cable.category,
    cable.subcategory1,
    cable.subcategory2,
    cable.subcategory3,
    cable.item_code,
    cable.vendor_id,
    cable.type,
    cable.brand,
    cable.model,
    cable.network_status,
    cable.status,
    cable.pin_code,
    cable.province_id,
    cable.region_id,
    cable.utilization,
    cable.totalattenuationloss,
    cable.chromaticdb,
    cable.chromaticdispersion,
    cable.totalchromaticloss,
    cable.remarks,
    cable.route_id,
    cable.created_by,
    cable.created_on,
    cable.modified_by,
    cable.modified_on,
    cable.a_system_id,
    cable.a_entity_type,
    cable.b_system_id,
    cable.b_entity_type,
    cable.sequence_id,
    cable.duct_id,
    cable.trench_id,
    cable.project_id,
    cable.planning_id,
    cable.purpose_id,
    cable.workorder_id,
    cable.structure_id,
    cable.cable_category,
    cable.execution_method,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS cable_specification_type,
    bd.brand AS cable_brand,
    ml.model AS cable_model,
    cable.total_loop_count,
    cable.total_loop_length,
    fn_get_date(cable.created_on) AS created_date,
    fn_get_date(cable.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    cable.parent_system_id,
    cable.parent_entity_type,
    cable.parent_network_id,
    round(cable.start_reading::numeric, 2) AS start_reading,
    round(cable.end_reading::numeric, 2) AS end_reading,
    cable.cable_sub_category,
    cable.acquire_from,
    cable.circuit_id,
    cable.thirdparty_circuit_id,
    cable.drum_no,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cable.accessibility) AS cable_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cable.construction) AS cable_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cable.activation) AS cable_activation,
    cable.barcode,
    cable.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    cable.source_ref_type,
    cable.source_ref_id,
    cable.source_ref_description,
    cable.status_remark,
    cable.status_updated_by,
    cable.status_updated_on,
    cable.is_visible_on_map,
    cable.primary_pod_system_id,
    cable.secondary_pod_system_id,
    cable.is_new_entity,
    round(cable.inner_dimension::numeric, 2) AS inner_dimension,
    round(cable.outer_dimension::numeric, 2) AS outer_dimension,
    cable.calculated_length_remark,
    cable.audit_item_master_id,
    cable.is_acquire_from,
    cable.origin_from,
    cable.origin_ref_id,
    cable.origin_ref_code,
    cable.origin_ref_description,
    cable.request_ref_id,
    cable.requested_by,
    cable.request_approved_by,
    cable.subarea_id,
    cable.area_id,
    cable.dsa_id,
    cable.csa_id,
    cable.bom_sub_category,
    cable.gis_design_id,
    cable.ne_id,
    cable.prms_id,
    cable.jc_id,
    cable.mzone_id,
    cable.a_location_name,
    cable.b_location_name,
    cable.route_name
   FROM att_details_cable cable
     JOIN province_boundary prov ON cable.province_id = prov.id
     JOIN region_boundary reg ON cable.region_id = reg.id
     JOIN user_master um ON cable.created_by = um.user_id
     JOIN vendor_master vm ON cable.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cable.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON cable.type = tp.id
     LEFT JOIN isp_brand_master bd ON cable.brand = bd.id
     LEFT JOIN isp_base_model ml ON cable.model = ml.id
     LEFT JOIN user_master um2 ON cable.modified_by = um2.user_id;	 
	 
-----------------------------------------------------------------------------------------------------
-- public.vw_att_details_cable_audit source

CREATE OR REPLACE VIEW public.vw_att_details_cable_audit
AS SELECT cable.audit_id,
    cable.system_id,
    cable.network_id,
    cable.cable_name,
    fn_get_display_name(cable.a_system_id, cable.a_entity_type) AS start_point,
    fn_get_display_name(cable.b_system_id, cable.b_entity_type) AS end_point,
    reg.region_name,
    prov.province_name,
    cable.pin_code,
    cable.a_entity_type,
    cable.b_entity_type,
    cable.cable_type,
    cable.no_of_tube,
    cable.no_of_core_per_tube,
    round(cable.cable_measured_length::numeric, 2) AS cable_measured_length,
    round(cable.cable_calculated_length::numeric, 2) AS cable_calculated_length,
    cable.coreaccess,
    cable.wavelength,
    cable.optical_output_power,
    cable.frequency,
    cable.attenuation_db AS attenuation,
    cable.resistance_ohm AS resistance,
    cable.specification,
    vm.name AS vendor_name,
    cable.item_code,
    cable.category,
    cable.subcategory1,
    cable.subcategory2,
    cable.cable_category,
    cable.total_loop_length,
    cable.total_loop_count,
    cable.route_id,
    round(cable.start_reading::numeric, 2) AS start_reading,
    round(cable.end_reading::numeric, 2) AS end_reading,
    cable.cable_sub_category,
    cable.execution_method,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    cable.remarks,
    cable.acquire_from,
    cable.circuit_id,
    cable.thirdparty_circuit_id,
    fn_get_date(cable.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(cable.modified_on) AS modified_on,
    ums.user_name AS modified_by,
    fn_get_network_status(cable.network_status) AS network_status,
    fn_get_status(cable.status) AS status,
    fn_get_action(cable.action) AS action,
    cable.barcode,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    cable.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    cable.source_ref_type,
    cable.source_ref_id,
    cable.source_ref_description,
    cable.status_remark,
    cable.status_updated_by,
    cable.status_updated_on,
    cable.is_visible_on_map,
    cable.primary_pod_system_id,
    cable.secondary_pod_system_id,
    round(cable.inner_dimension::numeric, 2) AS inner_dimension,
    round(cable.outer_dimension::numeric, 2) AS outer_dimension,
    cable.calculated_length_remark,
    cable.origin_from,
    cable.origin_ref_id,
    cable.origin_ref_code,
    cable.origin_ref_description,
    cable.request_ref_id,
    cable.requested_by,
    cable.request_approved_by,
    cable.subarea_id,
    cable.area_id,
    cable.dsa_id,
    cable.csa_id,
    cable.bom_sub_category,
    cable.gis_design_id,
    cable.ne_id,
    cable.prms_id,
    cable.jc_id,
    cable.mzone_id,
    oi.aaaa,
    oi.demo,
    oi.record_system_id,
    cable.route_name
   FROM audit_att_details_cable cable
     JOIN province_boundary prov ON cable.province_id = prov.id
     JOIN region_boundary reg ON cable.region_id = reg.id
     LEFT JOIN user_master um ON cable.created_by = um.user_id
     LEFT JOIN user_master ums ON cable.modified_by = ums.user_id
     JOIN vendor_master vm ON cable.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cable.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON cable.type = tp.id
     LEFT JOIN isp_brand_master bd ON cable.brand = bd.id
     LEFT JOIN isp_base_model ml ON cable.model = ml.id
     LEFT JOIN att_details_project_master pm ON cable.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON cable.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON cable.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON cable.purpose_id = purposem.system_id
     LEFT JOIN entity_notification_status entnotifystatus ON cable.notification_status_id = entnotifystatus.id
     JOIN vw_att_details_cable_audit_oi oi ON oi.audit_id = cable.audit_id;
	 
----------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_trg_audit_att_details_cable()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$

DECLARE
 _value integer;
 _ignorecol character varying;
 _ignorecol1 character varying;
tempjson text; --for additional attributes
sql text;  --for additional attributes
BEGIN

IF (TG_OP = 'INSERT' ) THEN  
         INSERT INTO public.audit_att_details_cable(
            system_id, network_id, cable_name, a_location, b_location, total_core, 
            no_of_tube, no_of_core_per_tube, cable_measured_length, cable_calculated_length, 
            cable_type, coreaccess, wavelength, optical_output_power, frequency, 
            attenuation_db, resistance_ohm, construction, activation, accessibility, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, network_status, status, 
            pin_code, province_id, region_id, utilization, totalattenuationloss, 
            chromaticdb, chromaticdispersion, totalchromaticloss, remarks,route_id,
            created_by, created_on, modified_by, modified_on, a_system_id, 
            a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type, 
            sequence_id, duct_id, trench_id, project_id, planning_id, purpose_id, 
            workorder_id, structure_id, cable_category, total_loop_length, 
            total_loop_count,action,start_reading,end_reading,cable_sub_category,parent_system_id,parent_network_id,parent_entity_type,execution_method,acquire_from,circuit_id,thirdparty_circuit_id,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,inner_dimension,outer_dimension,calculated_length_remark,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id,route_name)   
         select  new.system_id, new.network_id, new.cable_name, new.a_location, new.b_location, new.total_core, 
            new.no_of_tube, new.no_of_core_per_tube, new.cable_measured_length, new.cable_calculated_length, 
            new.cable_type, new.coreaccess, new.wavelength, new.optical_output_power, new.frequency, 
            new.attenuation_db, new.resistance_ohm, new.construction, new.activation, new.accessibility, 
            new.specification, new.category, new.subcategory1, new.subcategory2, new.subcategory3, 
            new.item_code, new.vendor_id, new.type, new.brand, new.model, new.network_status, new.status, 
            new.pin_code, new.province_id, new.region_id, new.utilization, new.totalattenuationloss, 
            new.chromaticdb, new.chromaticdispersion, new.totalchromaticloss, new.remarks,new.route_id,
            new.created_by, new.created_on, new.modified_by, new.modified_on, new.a_system_id, 
            new.a_network_id, new.a_entity_type, new.b_system_id, new.b_network_id, new.b_entity_type, 
            new.sequence_id, new.duct_id, new.trench_id, new.project_id, new.planning_id, new.purpose_id, 
            new.workorder_id, new.structure_id, new.cable_category, new.total_loop_length, 
            new.total_loop_count, 'I' as action,new.start_reading,new.end_reading,new.cable_sub_category,new.parent_system_id,new.parent_network_id,new.parent_entity_type,new.execution_method, new.acquire_from,new.circuit_id,new.thirdparty_circuit_id,new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.inner_dimension,new.outer_dimension,new.calculated_length_remark,new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category ,new.gis_design_id,new.route_name from att_details_cable where system_id=new.system_id;
      
	--raise info 'new.system_id %',new.system_id; 
          tempjson:='''"'||new.system_id||'"''';
         --raise info 'tempjson %',tempjson; 
   
	--Update value of the key 'record_system_id' for additional attributes    
   	sql:='UPDATE att_details_cable SET other_info = jsonb_set(other_info::jsonb , ''{record_system_id}'','||tempjson||') where system_id = '||new.system_id||';';
   
  	 --raise info 'sql %',sql; 
	execute sql;

END IF;
IF (TG_OP = 'UPDATE' ) THEN  
_ignorecol := 'modified_on,gis_design_id,codification_sequence,area_system_id,area_id,subarea_system_id,subarea_id,dsa_system_id,dsa_id,csa_system_id,csa_id';
_ignorecol1 := 'modified_by';
select fn_check_history_record(OLD, NEW,_ignorecol,_ignorecol1) into _value;
if(_value = 1)then
          INSERT INTO public.audit_att_details_cable(
            system_id, network_id, cable_name, a_location, b_location, total_core, 
            no_of_tube, no_of_core_per_tube, cable_measured_length, cable_calculated_length, 
            cable_type, coreaccess, wavelength, optical_output_power, frequency, 
            attenuation_db, resistance_ohm, construction, activation, accessibility, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, network_status, status, 
            pin_code, province_id, region_id, utilization, totalattenuationloss, 
            chromaticdb, chromaticdispersion, totalchromaticloss, remarks,route_id,
            created_by, created_on, modified_by, modified_on, a_system_id, 
            a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type, 
            sequence_id, duct_id, trench_id, project_id, planning_id, purpose_id, 
            workorder_id, structure_id, cable_category, total_loop_length, 
            total_loop_count,action,start_reading,end_reading,cable_sub_category,parent_system_id,parent_network_id,parent_entity_type,execution_method,barcode,acquire_from,circuit_id,thirdparty_circuit_id,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,inner_dimension,outer_dimension,calculated_length_remark,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id,route_name) 
         select  new.system_id, new.network_id, new.cable_name, new.a_location, new.b_location, new.total_core, 
            new.no_of_tube, new.no_of_core_per_tube, new.cable_measured_length, new.cable_calculated_length, 
            new.cable_type, new.coreaccess, new.wavelength, new.optical_output_power, new.frequency, 
            new.attenuation_db, new.resistance_ohm, new.construction, new.activation, new.accessibility, 
            new.specification, new.category, new.subcategory1, new.subcategory2, new.subcategory3, 
            new.item_code, new.vendor_id, new.type, new.brand, new.model, new.network_status, new.status, 
            new.pin_code, new.province_id, new.region_id, new.utilization, new.totalattenuationloss, 
            new.chromaticdb, new.chromaticdispersion, new.totalchromaticloss, new.remarks,new.route_id,
            new.created_by, new.created_on, new.modified_by, new.modified_on, new.a_system_id, 
            new.a_network_id, new.a_entity_type, new.b_system_id, new.b_network_id, new.b_entity_type, 
            new.sequence_id, new.duct_id, new.trench_id, new.project_id, new.planning_id, new.purpose_id, 
            new.workorder_id, new.structure_id, new.cable_category, new.total_loop_length, 
            new.total_loop_count,'U' as action ,new.start_reading,new.end_reading,new.cable_sub_category ,new.parent_system_id,new.parent_network_id,new.parent_entity_type,new.execution_method,new.barcode
            ,new.acquire_from,new.circuit_id,new.thirdparty_circuit_id,new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.inner_dimension,new.outer_dimension,new.calculated_length_remark,new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category ,new.gis_design_id,new.route_name from att_details_cable where system_id=new.system_id;
End if;
	
END IF; 				
IF (TG_OP = 'DELETE' ) THEN  

          INSERT INTO public.audit_att_details_cable(
            system_id, network_id, cable_name, a_location, b_location, total_core, 
            no_of_tube, no_of_core_per_tube, cable_measured_length, cable_calculated_length, 
            cable_type, coreaccess, wavelength, optical_output_power, frequency, 
            attenuation_db, resistance_ohm, construction, activation, accessibility, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, network_status, status, 
            pin_code, province_id, region_id, utilization, totalattenuationloss, 
            chromaticdb, chromaticdispersion, totalchromaticloss, remarks,route_id,
            created_by, created_on, modified_by, modified_on, a_system_id, 
            a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type, 
            sequence_id, duct_id, trench_id, project_id, planning_id, purpose_id, 
            workorder_id, structure_id, cable_category, total_loop_length, 
            total_loop_count,action,start_reading,end_reading,cable_sub_category,parent_system_id,parent_network_id,parent_entity_type,execution_method,barcode,acquire_from,circuit_id,thirdparty_circuit_id,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,inner_dimension,outer_dimension,calculated_length_remark,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id,route_name) 
         values(old.system_id, old.network_id, old.cable_name, old.a_location, old.b_location, old.total_core, 
            old.no_of_tube, old.no_of_core_per_tube, old.cable_measured_length, old.cable_calculated_length, 
            old.cable_type, old.coreaccess, old.wavelength, old.optical_output_power, old.frequency, 
            old.attenuation_db, old.resistance_ohm, old.construction, old.activation, old.accessibility, 
            old.specification, old.category, old.subcategory1, old.subcategory2, old.subcategory3, 
            old.item_code, old.vendor_id, old.type, old.brand, old.model, old.network_status, old.status, 
            old.pin_code, old.province_id, old.region_id, old.utilization, old.totalattenuationloss, 
            old.chromaticdb, old.chromaticdispersion, old.totalchromaticloss, old.remarks,old.route_id,
            old.created_by, old.created_on, old.modified_by, old.modified_on, old.a_system_id, 
            old.a_network_id, old.a_entity_type, old.b_system_id, old.b_network_id, old.b_entity_type, 
            old.sequence_id, old.duct_id, old.trench_id, old.project_id, old.planning_id, old.purpose_id, 
            old.workorder_id, old.structure_id, old.cable_category, old.total_loop_length, 
            old.total_loop_count, 'D',old.start_reading,old.end_reading,old.cable_sub_category,old.parent_system_id,old.parent_network_id,old.parent_entity_type,old.execution_method,old.barcode
            ,old.acquire_from,old.circuit_id,old.thirdparty_circuit_id,old.ownership_type,old.third_party_vendor_id,old.source_ref_type,old.source_ref_id,old.source_ref_description,old.audit_item_master_id,old.status_remark,old.status_updated_by,old.status_updated_on,old.is_visible_on_map,old.primary_pod_system_id,old.secondary_pod_system_id,old.inner_dimension,old.outer_dimension,old.calculated_length_remark,old.is_acquire_from,old.other_info,old.origin_from,old.origin_ref_id,old.origin_ref_code,old.origin_ref_description,old.request_ref_id,old.requested_by,old.request_approved_by,old.subarea_id,old.area_id,old.dsa_id,old.csa_id,old.bom_sub_category ,old.gis_design_id,old.route_name);
                   
	  update ATT_DETAILS_FIBER_LINK set fiber_link_status='Free' 
	  where system_id in(select link_system_id from att_details_cable_info where cable_id=old.system_id);

	  Delete from line_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='CABLE';
	  delete from att_details_cable_info where cable_id=old.SYSTEM_ID;

END IF; 		

RETURN NEW;
END;
$function$
;


-------------------------------------------------------------------------------------------------------------

-- public.vw_att_details_cable_report source

CREATE OR REPLACE VIEW public.vw_att_details_cable_report
AS SELECT cable.system_id,
    cable.network_id,
    cable.cable_name,
    cable.total_core,
    cable.no_of_tube,
    cable.no_of_core_per_tube,
    round(cable.cable_measured_length::numeric, 2) AS cable_measured_length,
    round(cable.cable_calculated_length::numeric, 2) AS cable_calculated_length,
    cable.cable_type,
    cable.coreaccess,
    cable.wavelength,
    cable.optical_output_power,
    cable.frequency,
    cable.attenuation_db,
    cable.resistance_ohm,
    cable.construction,
    cable.activation,
    cable.accessibility,
    cable.specification,
    cable.category,
    cable.subcategory1,
    cable.subcategory2,
    cable.subcategory3,
    cable.item_code,
        CASE
            WHEN cable.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN cable.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN cable.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.status, cable.status) AS status,
    cable.pin_code,
    cable.utilization,
    cable.totalattenuationloss,
    cable.chromaticdb,
    cable.chromaticdispersion,
    cable.totalchromaticloss,
    cable.remarks,
    cable.a_entity_type,
    cable.b_entity_type,
    cable.cable_category,
    cable.execution_method,
    cable.total_loop_count,
    cable.total_loop_length,
    cable.parent_entity_type AS parent_type,
    cable.parent_network_id AS parent_code,
    cable.route_id,
    round(cable.start_reading::numeric, 2) AS start_reading,
    round(cable.end_reading::numeric, 2) AS end_reading,
    cable.cable_sub_category,
    um.user_name AS created_by,
    to_char(cable.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(cable.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    cable.drum_no,
    cable.acquire_from,
    cable.circuit_id,
    cable.thirdparty_circuit_id,
    cable.province_id,
    cable.region_id,
        CASE
            WHEN cable.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    reg.region_name,
    cable.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    prov.province_name,
        CASE
            WHEN cable.utilization::text = 'L'::text THEN 'Low'::text
            WHEN cable.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN cable.utilization::text = 'H'::text THEN 'High'::text
            WHEN cable.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    cable.ownership_type,
    vm.name AS vendor_name,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    cable.source_ref_type,
    cable.source_ref_id,
    cable.source_ref_description,
    cable.status_remark,
    cable.status_updated_by,
    to_char(cable.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    cable.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    round(cable.inner_dimension::numeric, 2) AS inner_dimension,
    round(cable.outer_dimension::numeric, 2) AS outer_dimension,
    cable.calculated_length_remark,
    reg.country_name,
    cable.origin_from,
    cable.origin_ref_id,
    cable.origin_ref_code,
    cable.origin_ref_description,
    cable.request_ref_id,
    cable.requested_by,
    cable.request_approved_by,
    cable.subarea_id,
    cable.area_id,
    cable.dsa_id,
    cable.csa_id,
    cable.bom_sub_category,
    cable.gis_design_id AS design_id,
    cable.ne_id,
    cable.prms_id,
    cable.jc_id,
    cable.mzone_id,
    cable.served_by_ring,
    cable.a_latitude,
    cable.b_longitude,
    cable.a_region,
    cable.b_region,
    cable.a_city,
    cable.b_city,
    oi.aaaa,
    oi.demo,
    oi.record_system_id,
    cable.route_name
   FROM att_details_cable cable
     JOIN user_master um ON cable.created_by = um.user_id
     JOIN vendor_master vm ON cable.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cable.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON cable.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON cable.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON cable.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON cable.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON cable.purpose_id = purposem.system_id
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(cable.network_id::text) ~~ upper('NLD%'::text) THEN cable.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('CABLE'::text) AND cable.province_id = pmapping.province_id
            ELSE 1 = 2
        END
     JOIN province_boundary prov ON prov.id = COALESCE(pmapping.province_id, cable.province_id)
     JOIN region_boundary reg ON reg.id = COALESCE(pmapping.region_id, cable.region_id)
     LEFT JOIN entity_notification_status entnotifystatus ON cable.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON cable.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON cable.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = cable.status::text
     LEFT JOIN vw_att_details_cable_oi oi ON oi.record_system_id = cable.system_id::text;
	 
--------------------------------------------------------------------------------------------------------------
-- update Design ID value
UPDATE public.res_resources
SET value='Design ID' where value ='Design ID:' and key ='SI_GBL_GBL_GBL_GBL_164';	 