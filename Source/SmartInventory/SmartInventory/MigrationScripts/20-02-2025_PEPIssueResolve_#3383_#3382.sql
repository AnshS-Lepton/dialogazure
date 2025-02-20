
-------------------- trigger function for audit_att_details_pep ----------------------------

 DROP FUNCTION IF EXISTS public.fn_trg_audit_att_details_pep();

CREATE OR REPLACE FUNCTION public.fn_trg_audit_att_details_pep()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$

DECLARE
_value integer;
 _ignorecol character varying;  
_ignorecol1 character varying;
tempjson text; --for additional attributes
sql text;  --for additional attributes
BEGIN

IF (TG_OP = 'INSERT' ) THEN  
       INSERT INTO public.audit_att_details_pep(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,
            status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,remarks,is_acquire_from,
            other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         select  new.system_id, new.network_id, new.pep_name, new.latitude, new.longitude, 
            new.province_id, new.region_id, new.address, new.pep_no, new.pep_height, 
            new.specification, new.category, new.subcategory1, new.subcategory2, new.subcategory3, 
            new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, new.activation, 
            new.accessibility, new.status, new.created_by, new.created_on, new.modified_by, new.modified_on, 
            new.network_status, new.parent_system_id, new.parent_network_id, new.parent_entity_type, 
            new.sequence_id, new.project_id, new.planning_id, new.purpose_id, new.workorder_id, 'I' as action,new.acquire_from,new.ownership_type,
            new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,
            new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.remarks,
            new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,
            new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category,new.gis_design_id
             from att_details_pep where system_id=new.system_id;

	
END IF;
IF (TG_OP = 'UPDATE' ) THEN  
_ignorecol := 'modified_on,gis_design_id,codification_sequence,area_system_id,area_id,subarea_system_id,subarea_id,dsa_system_id,dsa_id,csa_system_id,csa_id';
_ignorecol1 := 'modified_by';
select fn_check_history_record(OLD, NEW,_ignorecol,_ignorecol1) into _value;
if(_value = 1)then

           INSERT INTO public.audit_att_details_pep(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,
            audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,
            remarks,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,
            request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         select  new.system_id, new.network_id, new.pep_name, new.latitude, new.longitude, 
            new.province_id, new.region_id, new.address, new.pep_no, new.pep_height, 
            new.specification, new.category, new.subcategory1, new.subcategory2, new.subcategory3, 
            new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, new.activation, 
            new.accessibility, new.status, new.created_by, new.created_on, new.modified_by, new.modified_on, 
            new.network_status, new.parent_system_id, new.parent_network_id, new.parent_entity_type, 
            new.sequence_id, new.project_id, new.planning_id, new.purpose_id, new.workorder_id, 'U' as action,new.acquire_from,
            new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,
            new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.remarks,new.is_acquire_from,
            new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,
            new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category,new.gis_design_id
             from att_details_pep where system_id=new.system_id;

END IF; 

END IF; 								
IF (TG_OP = 'DELETE' ) THEN  

          INSERT INTO public.audit_att_details_pep(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,
            status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,remarks,is_acquire_from,
            other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         values( old.system_id, old.network_id, old.pep_name, old.latitude, old.longitude, 
            old.province_id, old.region_id, old.address, old.pep_no, old.pep_height, 
            old.specification, old.category, old.subcategory1, old.subcategory2, old.subcategory3, 
            old.item_code, old.vendor_id, old.type, old.brand, old.model, old.construction, old.activation, 
            old.accessibility, old.status, old.created_by, old.created_on, old.modified_by, old.modified_on, 
            old.network_status, old.parent_system_id, old.parent_network_id, old.parent_entity_type, 
            old.sequence_id, old.project_id, old.planning_id, old.purpose_id, old.workorder_id, 'D',old.acquire_from,old.ownership_type,old.third_party_vendor_id,
            old.source_ref_type,old.source_ref_id,old.source_ref_description,old.audit_item_master_id,old.status_remark,old.status_updated_by,
            old.status_updated_on,old.is_visible_on_map,old.primary_pod_system_id,old.secondary_pod_system_id,old.remarks,old.is_acquire_from,
            old.other_info,old.origin_from,old.origin_ref_id,old.origin_ref_code,old.origin_ref_description,old.request_ref_id,old.requested_by,
            old.request_approved_by,old.subarea_id,old.area_id,old.dsa_id,old.csa_id,old.bom_sub_category,old.gis_design_id);
          
         
          Delete from point_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='PEP';

END IF; 		

RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.fn_trg_audit_att_details_pep()
    OWNER TO postgres;
