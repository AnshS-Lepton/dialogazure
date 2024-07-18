ALTER TABLE audit_att_details_cable
ADD COLUMN start_name VARCHAR;

--------------------------------------------------
ALTER TABLE audit_att_details_cable
ADD COLUMN end_name VARCHAR;


------------------------------------------------------
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
    cable.hierarchy_type,
    cable.section_name,
    cable.aerial_location,
    cable.cable_remark,
    cable.start_name,
    cable.end_name
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
     LEFT JOIN entity_notification_status entnotifystatus ON cable.notification_status_id = entnotifystatus.id;
	 
	 ---------------------------------------
	 
	 -- DROP FUNCTION public.fn_trg_audit_att_details_cable();

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
            total_loop_count,action,start_reading,end_reading,cable_sub_category,parent_system_id,parent_network_id,parent_entity_type,execution_method,acquire_from,circuit_id,thirdparty_circuit_id,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,inner_dimension,outer_dimension,calculated_length_remark,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id,route_name,start_name,end_name)   
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
            new.total_loop_count, 'I' as action,new.start_reading,new.end_reading,new.cable_sub_category,new.parent_system_id,new.parent_network_id,new.parent_entity_type,new.execution_method, new.acquire_from,new.circuit_id,new.thirdparty_circuit_id,new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.inner_dimension,new.outer_dimension,new.calculated_length_remark,new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category ,new.gis_design_id,new.route_name,new.a_location_name,new.b_location_name from att_details_cable where system_id=new.system_id;
      
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
            total_loop_count,action,start_reading,end_reading,cable_sub_category,parent_system_id,parent_network_id,parent_entity_type,execution_method,barcode,acquire_from,circuit_id,thirdparty_circuit_id,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,inner_dimension,outer_dimension,calculated_length_remark,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id,route_name,start_name,end_name) 
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
            ,new.acquire_from,new.circuit_id,new.thirdparty_circuit_id,new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.inner_dimension,new.outer_dimension,new.calculated_length_remark,new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category ,new.gis_design_id,new.route_name,new.a_location_name,new.b_location_name from att_details_cable where system_id=new.system_id;
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
            total_loop_count,action,start_reading,end_reading,cable_sub_category,parent_system_id,parent_network_id,parent_entity_type,execution_method,barcode,acquire_from,circuit_id,thirdparty_circuit_id,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,inner_dimension,outer_dimension,calculated_length_remark,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id,route_name,start_name,end_name) 
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
            ,old.acquire_from,old.circuit_id,old.thirdparty_circuit_id,old.ownership_type,old.third_party_vendor_id,old.source_ref_type,old.source_ref_id,old.source_ref_description,old.audit_item_master_id,old.status_remark,old.status_updated_by,old.status_updated_on,old.is_visible_on_map,old.primary_pod_system_id,old.secondary_pod_system_id,old.inner_dimension,old.outer_dimension,old.calculated_length_remark,old.is_acquire_from,old.other_info,old.origin_from,old.origin_ref_id,old.origin_ref_code,old.origin_ref_description,old.request_ref_id,old.requested_by,old.request_approved_by,old.subarea_id,old.area_id,old.dsa_id,old.csa_id,old.bom_sub_category ,old.gis_design_id,old.route_name,old.a_location_name,old.b_location_name);

           IF NOT EXISTS(select 1 from att_details_cable_info where link_system_id=(select link_system_id from att_details_cable_info where cable_id=old.system_id limit 1)
           limit 1) THEN                   
		update ATT_DETAILS_FIBER_LINK set fiber_link_status='Free' where system_id in(select link_system_id from att_details_cable_info 
		where cable_id=old.system_id);
	   END IF;

	  Delete from line_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='CABLE';
	  delete from att_details_cable_info where cable_id=old.SYSTEM_ID;

END IF; 		

RETURN NEW;
END;
$function$
;


------------------------------------------------------------------------------------------------
-- DROP FUNCTION public.fn_get_audit_log_report_allexcel(varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, varchar, int4, int4, varchar, varchar, varchar, int4, int4, varchar, varchar, varchar, float8, varchar);

CREATE OR REPLACE FUNCTION public.fn_get_audit_log_report_allexcel(p_regionids character varying, p_provinceids character varying, p_networkstatues character varying, p_parentusers character varying, p_userids character varying, p_layer_name character varying, p_projectcodes character varying, p_planningcodes character varying, p_workordercodes character varying, p_purposecodes character varying, p_durationbasedon character varying, p_fromdate character varying, p_todate character varying, p_geom character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_advancefilter character varying, p_userid integer, p_roleid integer, p_ownership_type character varying, p_thirdparty_vendor_ids character varying, p_culturename character varying, p_radious double precision, p_route character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

Declare v_arow record;
sql TEXT;
StartSNo INTEGER;
EndSNo INTEGER;
LowerStart character varying;
LowerEnd character varying;
TotalRecords integer;
WhereCondition character varying;
s_report_view_name character varying;
s_geom_type character varying;
s_layer_id integer;
s_layer_name character varying;
s_layer_columns text;
s_layer_columns_val text;
s_table_name character varying;
v_geometry geometry;
v_system_id integer;

Begin
LowerStart:='';
LowerEnd:='';
WhereCondition:='';

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE,layer_name INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE,s_layer_name FROM LAYER_DETAILS
WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
/*
select st_geomfromtext('POLYGON(('||p_geom||'))',4326) into v_geometry;

IF(UPPER(S_GEOM_TYPE) = UPPER('point')) THEN
select system_id into v_system_id from point_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('POLYGON')) THEN
select system_id into v_system_id from Polygon_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('circle')) THEN
select system_id into v_system_id from circle_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('Line')) THEN
select system_id into v_system_id from Line_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;*/

-- raise info'v_system_id =%',v_system_id;
s_table_name := 'audit_att_details_' || lower(p_layer_name);
if lower(p_layer_name) ='structure' then 
s_table_name :='audit_att_details_bld_structure';
end if;
if lower(p_layer_name) ='fdb' then 
s_table_name :='audit_isp_fdb_info';
end if;
if lower(p_layer_name) ='equipment' then 
s_table_name :='audit_att_details_model';
end if;



IF ((p_geom) != '') then
select st_geomfromtext('POLYGON(('||p_geom||'))',4326) into v_geometry;

IF(UPPER(S_GEOM_TYPE) = UPPER('point')) THEN
select system_id into v_system_id from point_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('POLYGON')) THEN
select system_id into v_system_id from Polygon_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('circle')) THEN
select system_id into v_system_id from circle_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;

IF(UPPER(S_GEOM_TYPE) = UPPER('Line')) THEN
select system_id into v_system_id from Line_master m where   ST_Intersects(m.sp_geometry, v_geometry)
	and upper(m.entity_type) =upper(P_LAYER_NAME);
END IF;
if(p_radious>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radious)),-2),10) into p_geom;
end if;

Create temp table temp_entity_inside_geom
(
system_id integer,
entity_type character varying
) on commit drop;

-- HERE WE ARE FETCHING ALL THE ENTITY WITHIN THE ARAE SELECTED BY USER INTO A TEMP TABLE.
perform(fn_get_export_report_entity_within_geom(S_LAYER_ID::character varying,p_geom));
end if;

IF (coalesce(P_SORTCOLNAME,'')!='') THEN
IF EXISTS (select 1 from information_schema.columns where upper(table_name) = upper(S_REPORT_VIEW_NAME) and
upper(column_name) = upper(P_SORTCOLNAME) and upper(data_type) in('CHARACTER VARYING','TEXT')) THEN
LowerStart:='LOWER(';
else
LowerStart:='(';
end if;
LowerEnd:=')';
END IF;

if(p_pageno <>0)
then
SELECT STRING_AGG(COLUMN_NAME,',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,DISPLAY_NAME,res_field_key FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
else
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)='REPORT' AND st.IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
end if;
SELECT STRING_AGG('replace('||COLUMN_NAME||'::character varying,'||'''='''||','||'''''''='''||')'||' as "'||case when coalesce(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM (
SELECT st.COLUMN_NAME,st.DISPLAY_NAME,res.value as res_field_key FROM LAYER_COLUMNS_SETTINGS st
left join res_resources res on res.key= st.res_field_key and upper(culture)=upper(p_culturename)
WHERE st.LAYER_ID=S_LAYER_ID AND UPPER(st.SETTING_TYPE)='REPORT' ORDER BY st.COLUMN_SEQUENCE) A;

IF ((p_geom) != '') THEN
sql:= 'SELECT user_name as USER_NAME, CASE WHEN action = ''U'' THEN ''UPDATE'' ELSE ''CREATE'' END AS LOG_ACTION,
	 log_table_name as LOG_TABLE_NAME, category as NE_TYPE, a.system_id as SYSTEM_ID, a.network_id as NE_ID,
	 colname as LOG_COLUMN_NAME, oldvalue as COL_OLD_VAL, newvalue COL_NEW_VAL, modified_on as LOG_DATE
from fn_get_audit_log_details('''||s_table_name||''', '''||v_system_id||''') a
inner join temp_entity_inside_geom m on a.system_id::integer=m.system_id	
where 1=1 ';
ELSE
-- FETCH RECORDS BASED ON SELECTED FILTERS.
/*sql:= 'SELECT user_name as USER_NAME, CASE WHEN action = ''U'' THEN ''UPDATE'' ELSE ''CREATE'' END AS LOG_ACTION,
	 log_table_name as LOG_TABLE_NAME, category as NE_TYPE, a.system_id as SYSTEM_ID, a.network_id as NE_ID,
	 colname as LOG_COLUMN_NAME, oldvalue as COL_OLD_VAL, newvalue COL_NEW_VAL, modified_on as LOG_DATE
from fn_get_audit_log_details_all('''||s_table_name||''', '''||p_layer_name||''') a' ;
*/
sql:= 'SELECT  CASE WHEN action = ''U'' THEN ''UPDATE'' ELSE ''CREATE'' END AS LOG_ACTION, '''||p_layer_name||''' as NE_TYPE, a.system_id as SYSTEM_ID, a.network_id as NE_ID
from '||s_table_name||' a' ;

if exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join region_boundary rb on rb.id = a.region_id ';

elsif exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
then 
sql:= sql ||' inner join vw_user_permission_region pa on pa.region_id = a.region_id and pa.user_id = '||p_userid||' ';
else 
sql:= sql ||' inner join vw_user_permission_area pa on pa.province_id = a.province_id and pa.user_id = '||p_userid||' ';
end if;
END IF;
-- REGION FILTER
IF ((p_RegionIds) != '') THEN
sql:= sql ||' AND a.region_id IN ('||p_RegionIds||')';
END IF;
-- PROVINCE FILTER
IF ((P_ProvinceIds) != '') THEN
sql:= sql ||' AND a.province_id IN ('||P_ProvinceIds||')';
END IF;
-- NETOWRK STATUS FILTER
IF ((p_networkStatues) != '' and upper(S_LAYER_COLUMNS_VAL) like '%NETWORK_STATUS%') THEN
sql:= sql ||' AND upper(a.network_status:: TEXT) in('''||p_networkStatues||''')';
END IF;

IF ((p_route) != '') THEN

	sql:= sql ||' inner join ASSOCIATE_ROUTE_INFO ASS on ASS.entity_id=a.system_id and ass.cable_id in('||p_route||')';

END IF;

-- PARENT/CHILD USER FILTER
if not exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='PANEXRPT' where user_id=p_userid)
and not exists(select 1 from user_module_mapping m 
inner join module_master mm on mm.id=m.module_id and upper(mm.module_abbr)='STATEEXRPT' where user_id=p_userid)
and p_roleid>1
then 
IF(P_PARENTUSERS != '' AND P_PARENTUSERS != '0') 
THEN
IF(P_USERIDS != '')
THEN
sql := sql ||' AND cast(a.created_by as integer) in ('||p_userids||')';
else
P_PARENTUSERS := Replace(P_PARENTUSERS,'0,','');
sql := sql ||' AND cast(a.created_by as integer) in (select * from fn_get_report_mapped_users('''||p_parentusers||'''))';
END IF;
END IF;
END IF;

-- PROJECT CODE FILTER
IF ((p_projectcodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PROJECT_ID%') THEN
sql:= sql ||' AND a.project_id IN ('||p_projectcodes||')';
else IF ((p_projectcodes) != '') then
sql:= sql ||' AND 0 = 1';
END IF;
END IF;
-- PLANNING CODE FILTER
IF ((p_planningcodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PLANNING_ID%') THEN
sql:= sql ||' AND a.planning_id IN ('||p_planningcodes||')';
END IF;
-- WORKORDER CODE FILTER
IF ((p_workordercodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%WORKORDER_ID%') THEN
sql:= sql ||' AND a.workorder_id IN ('||p_workordercodes||')';
END IF;
-- PURPOSE CODE FILTER
IF ((p_purposecodes) != '' and upper(S_LAYER_COLUMNS_VAL) like '%PURPOSE_ID%') THEN
sql:= sql ||' AND a.purpose_id IN ('||p_purposecodes||')';
END IF;

-- DURATION FILTER
IF(P_fromDate != '' and P_toDate != '') THEN
sql:= sql ||' AND a.'||p_durationbasedon||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_durationbasedon||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';
END IF;

-- ADVANCE FILTER SELCTED BY USER
IF ((p_advancefilter) != '') THEN
sql:= sql ||''|| p_advancefilter||'';
END IF;

-- OWNER SHIP FILTER
IF ((p_ownership_type) != '' and upper(S_LAYER_COLUMNS_VAL) like '%OWNERSHIP_TYPE%') THEN
sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''')';
else IF ((p_ownership_type) != '') then
sql:= sql ||'';
END IF;
end if;

IF ((p_thirdparty_vendor_ids) != '' and upper(S_LAYER_COLUMNS_VAL) like '%THIRD_PARTY_VENDOR_ID%') THEN
sql:= sql ||' AND (Cast(a.ownership_type as TEXT)) in('''||p_ownership_type||''') AND a.third_party_vendor_id in('''||p_thirdparty_vendor_ids||''')';
else IF ((p_thirdparty_vendor_ids) != '') then
sql:= sql ||'';
end if;
end if;

 raise info'sql =%',sql;
--sql:= sql ||' order by splited_by desc' ;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
End;
$function$
;