CREATE OR REPLACE VIEW public.vw_att_details_row_map
 AS
 SELECT "row".system_id,
    "row".network_id,
    COALESCE("row".status, 'A'::character varying) AS status,
    "row".is_new_entity,
    "row".row_name,
    COALESCE(adi.updated_geom, plgn.sp_geometry) AS sp_geometry,
    plgn.geom_type::character varying AS geom_type,
    "row".region_id,
    "row".province_id,
    plgn.row_text,
    plgn.center_line_geom,
    "row".project_id,
    "row".planning_id,
    "row".workorder_id,
    "row".purpose_id,
        CASE
            WHEN upper("row".row_stage::text) = upper('NEW'::text) THEN '#ffff00'::text
            WHEN upper("row".row_stage::text) = upper('Applied'::text) THEN '#fac803'::text
            WHEN upper("row".row_stage::text) = upper('Approved'::text) AND (EXISTS ( SELECT 1
               FROM att_details_row_associate_entity_info
              WHERE att_details_row_associate_entity_info.parent_system_id = "row".system_id AND upper(att_details_row_associate_entity_info.parent_entity_type::text) = upper('ROW'::text))) THEN '#02a3f4'::text
            WHEN upper("row".row_stage::text) = upper('Approved'::text) THEN '#05f66f'::text
            WHEN upper("row".row_stage::text) = upper('Rejected'::text) THEN '#f84c4c'::text
            ELSE NULL::text
        END AS color_code,
    "row".source_ref_id,
    "row".source_ref_type,
    "row".network_id AS label_column
   FROM ( SELECT polygon_master.system_id,
            polygon_master.entity_type,
            polygon_master.sp_geometry,
            polygon_master.center_line_geom,
            'Polygon'::text AS geom_type,
            ''::text AS row_text
           FROM polygon_master
          WHERE polygon_master.entity_type::text = 'ROW'::text
        UNION ALL
         SELECT circle_master.system_id,
            circle_master.entity_type,
            circle_master.sp_geometry,
            NULL::geometry AS center_line_geom,
            'Circle'::text AS geom_type,
            ('Radius:'::text || circle_master.radious) || '(Mtr)'::text AS row_text
           FROM circle_master) plgn
     JOIN att_details_row "row" ON plgn.system_id = "row".system_id AND plgn.entity_type::text = 'ROW'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = "row".system_id AND adi.entity_type::text = 'ROW'::text;
	 
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

           IF NOT EXISTS(select 1 from att_details_cable_info where link_system_id=(select link_system_id from att_details_cable_info where cable_id=old.system_id) limit 1) THEN                   
				update ATT_DETAILS_FIBER_LINK set fiber_link_status='Free' where system_id in(select link_system_id from att_details_cable_info where cable_id=old.system_id);
		   END IF;

	  Delete from line_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='CABLE';
	  delete from att_details_cable_info where cable_id=old.SYSTEM_ID;

END IF; 		

RETURN NEW;
END;
$function$
;

INSERT INTO public.global_settings ( "key", value, description, "type", is_edit_allowed, data_type, max_value, created_on, created_by, min_value, is_mobile_key, is_web_key)
 VALUES( 'MaxLineEntityLength', '100', 'Max length a line entity can be created', 'Web', true, 'double precision', 100.0, now(), 1, 1.0, false, true);
 
 CREATE OR REPLACE FUNCTION public.fn_associate_split_entities(p_entity_one_system_id integer, p_entity_two_system_id integer, p_entity_one_network_id character varying, p_entity_two_network_id character varying, p_entity_type character varying, p_old_entity_system_id integer)
RETURNS SETOF json
LANGUAGE plpgsql
AS $function$

declare
v_p_geom_type character varying;
v_geometry character varying;
v_geometry2 CHARACTER VARYING;
v_buffer integer;
v_cable_type character varying;
v_user_id integer;
v_one_display_name character varying;
v_two_display_name character varying;
arow record;
v_entity_one_system_id integer;
v_entity_two_system_id integer;

BEGIN

if(upper(p_entity_type)='CABLE')
then
select cable_type into v_cable_type from att_details_cable where system_id=p_old_entity_system_id;
--select created_by into v_user_id from att_details_cable where system_id=p_entity_one_system_id;
select created_by into v_user_id from att_details_cable where network_id=p_entity_one_network_id;
IF(p_entity_one_system_id=0)
THEN
select system_id into p_entity_one_system_id from att_details_cable where network_id=p_entity_one_network_id;
END IF;
IF(p_entity_two_system_id=0)
THEN
select system_id into p_entity_two_system_id from att_details_cable where network_id=p_entity_two_network_id;
END IF;
end if;

if(upper(p_entity_type)='DUCT')
then
select created_by into v_user_id from att_details_duct where system_id=p_entity_one_system_id;
end if;

select geom_type into v_p_geom_type from layer_details where upper(layer_name)=upper(p_entity_type);
execute 'select pm.sp_geometry,pm.display_name from '||v_p_geom_type||'_master pm where pm.system_id='||p_entity_one_system_id||' and upper(pm.entity_type)=upper('''||p_entity_type||''')' into v_geometry,v_one_display_name;
execute 'select pm.sp_geometry,pm.display_name from '||v_p_geom_type||'_master pm where pm.system_id='||p_entity_two_system_id||' and upper(pm.entity_type)=upper('''||p_entity_type||''')' into v_geometry2,v_two_display_name;
select value::integer into v_buffer from global_settings where key='AssociateEntityBuffer' and type='Web';
--------------------LEFT SIDE ASSOCIATION------------------------------------
insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,associated_entity_type,associated_display_name,created_on,created_by)
select p_entity_one_system_id,p_entity_one_network_id,p_entity_type,v_one_display_name,ass.entity_system_id,ass.entity_network_id,ass.entity_type,display_name,now(),v_user_id
from (
select
pm.system_id,
pm.entity_type,pm.common_name,null as cable_type,'Point' as geom_type,pm.display_name
from point_master pm
where upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type) and upper(coalesce(layer_subtype,''))=case when upper(p_entity_type)='CABLE' then upper(v_cable_type) else upper(coalesce(layer_subtype,'')) end)
and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer) )

union
select lm.system_id,lm.entity_type,lm.common_name,cbl.cable_type,lm.geom_type,lm.display_name from (
select
pm.system_id,
pm.entity_type,pm.common_name,'Line' as geom_type,pm.display_name
from line_master pm
where
upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
and
ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer) )) lm
left join att_details_cable cbl on lm.system_id=cbl.system_id and upper(lm.entity_type)='CABLE'
where cable_type is null or upper(cable_type) in (select upper(layer_subtype) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
) tblmain
inner join
(
select entity_system_id,entity_network_id,entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
union
select associated_system_id,associated_network_id,associated_entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
) ass on tblmain.system_id=ass.entity_system_id and upper(tblmain.entity_type)=upper(ass.entity_type)
--and ass.entity_system_id!=p_old_entity_system_id
and (upper(ass.entity_type)!=upper(p_entity_type))
--left join user_master um on um.user_id=ass.created_by
left join layer_details ld on upper(ld.layer_name)=upper(tblmain.entity_type)
where case when ld.is_isp_layer=true and ld.geom_type='Point' and ass.entity_system_id is null then 1=2 else 1=1 end;

RAISE INFO ' % v_geometry', v_geometry;

---Update loop code
IF((select count(pm.system_id)
from point_master pm
inner join att_details_loop lp on pm.system_id=lp.system_id
where lp.associated_system_id=p_old_entity_system_id and upper(pm.entity_type) =upper('LOOP') and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer)))>0)
THEN
update att_details_loop set associated_system_id=p_entity_one_system_id,associated_network_id=p_entity_one_network_id,cable_system_id=p_entity_one_system_id
where cable_system_id=p_old_entity_system_id;
END IF;

------------------------------------------------------------LEFT SIDE ASSOCIATION---------------------------------------------------------------------------------
insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,associated_entity_type,associated_display_name,created_on,created_by)
select p_entity_two_system_id,p_entity_two_network_id,p_entity_type,v_two_display_name,ass.entity_system_id,ass.entity_network_id,ass.entity_type,display_name ,now(),v_user_id
from (
select
pm.system_id,
pm.entity_type,pm.common_name,null as cable_type,'Point' as geom_type,pm.display_name
from point_master pm
where upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type) and upper(coalesce(layer_subtype,''))=case when upper(p_entity_type)='CABLE' then upper(v_cable_type) else upper(coalesce(layer_subtype,'')) end)
and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer) )

union
select lm.system_id,lm.entity_type,lm.common_name,cbl.cable_type,lm.geom_type,lm.display_name from (
select
pm.system_id,
pm.entity_type,pm.common_name,'Line' as geom_type,pm.display_name
from line_master pm
where
upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
and
ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer) )) lm
left join att_details_cable cbl on lm.system_id=cbl.system_id and upper(lm.entity_type)='CABLE'
where cable_type is null or upper(cable_type) in (select upper(layer_subtype) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
) tblmain
inner join
(
select entity_system_id,entity_network_id,entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
union
select associated_system_id,associated_network_id,associated_entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
) ass on tblmain.system_id=ass.entity_system_id and upper(tblmain.entity_type)=upper(ass.entity_type)
--and ass.entity_system_id!=p_old_entity_system_id
and (upper(ass.entity_type)!=upper(p_entity_type))
--left join user_master um on um.user_id=ass.created_by
left join layer_details ld on upper(ld.layer_name)=upper(tblmain.entity_type)
where case when ld.is_isp_layer=true and ld.geom_type='Point' and ass.entity_system_id is null then 1=2 else 1=1 end;

RAISE INFO ' % v_geometry2 ', v_geometry2;

---Update loop code
IF((select count(pm.system_id) from point_master pm
inner join att_details_loop lp on pm.system_id=lp.system_id
where lp.associated_system_id=p_old_entity_system_id and upper(pm.entity_type) =upper('LOOP') and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer)))>0)
THEN
update att_details_loop set associated_system_id=p_entity_two_system_id,associated_network_id=p_entity_two_network_id,cable_system_id=p_entity_two_system_id
where cable_system_id=p_old_entity_system_id;
END IF;


---FIBER LINK ASSOCIATION BY ANTRA

select system_id into v_entity_one_system_id from att_details_cable where network_id=p_entity_one_network_id;
select system_id into v_entity_two_system_id from att_details_cable where network_id=p_entity_two_network_id;

update att_details_cable_info t set link_system_id=tmp.link_system_id from
(
select a.link_system_id ,a.fiber_number from att_details_cable_info a
WHERE a.cable_id=p_old_entity_system_id
)
tmp where t.fiber_number=tmp.fiber_number and t.cable_id=v_entity_one_system_id;

update att_details_cable_info t set link_system_id=tmp.link_system_id from
(
select a.link_system_id ,a.fiber_number from att_details_cable_info a
WHERE a.cable_id=p_old_entity_system_id
)
tmp where t.fiber_number=tmp.fiber_number and t.cable_id=v_entity_two_system_id;

----END FIBER LINK ASSOCIATION 

END;
$function$
;
CREATE OR REPLACE FUNCTION public.fn_associate_split_entities(p_entity_one_system_id integer, p_entity_two_system_id integer, p_entity_one_network_id character varying, p_entity_two_network_id character varying, p_entity_type character varying, p_old_entity_system_id integer)
RETURNS SETOF json
LANGUAGE plpgsql
AS $function$

declare
v_p_geom_type character varying;
v_geometry character varying;
v_geometry2 CHARACTER VARYING;
v_buffer integer;
v_cable_type character varying;
v_user_id integer;
v_one_display_name character varying;
v_two_display_name character varying;
arow record;
v_entity_one_system_id integer;
v_entity_two_system_id integer;

BEGIN

if(upper(p_entity_type)='CABLE')
then
select cable_type into v_cable_type from att_details_cable where system_id=p_old_entity_system_id;
--select created_by into v_user_id from att_details_cable where system_id=p_entity_one_system_id;
select created_by into v_user_id from att_details_cable where network_id=p_entity_one_network_id;
IF(p_entity_one_system_id=0)
THEN
select system_id into p_entity_one_system_id from att_details_cable where network_id=p_entity_one_network_id;
END IF;
IF(p_entity_two_system_id=0)
THEN
select system_id into p_entity_two_system_id from att_details_cable where network_id=p_entity_two_network_id;
END IF;
end if;

if(upper(p_entity_type)='DUCT')
then
select created_by into v_user_id from att_details_duct where system_id=p_entity_one_system_id;
end if;

select geom_type into v_p_geom_type from layer_details where upper(layer_name)=upper(p_entity_type);
execute 'select pm.sp_geometry,pm.display_name from '||v_p_geom_type||'_master pm where pm.system_id='||p_entity_one_system_id||' and upper(pm.entity_type)=upper('''||p_entity_type||''')' into v_geometry,v_one_display_name;
execute 'select pm.sp_geometry,pm.display_name from '||v_p_geom_type||'_master pm where pm.system_id='||p_entity_two_system_id||' and upper(pm.entity_type)=upper('''||p_entity_type||''')' into v_geometry2,v_two_display_name;
select value::integer into v_buffer from global_settings where key='AssociateEntityBuffer' and type='Web';
--------------------LEFT SIDE ASSOCIATION------------------------------------
insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,associated_entity_type,associated_display_name,created_on,created_by)
select p_entity_one_system_id,p_entity_one_network_id,p_entity_type,v_one_display_name,ass.entity_system_id,ass.entity_network_id,ass.entity_type,display_name,now(),v_user_id
from (
select
pm.system_id,
pm.entity_type,pm.common_name,null as cable_type,'Point' as geom_type,pm.display_name
from point_master pm
where upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type) and upper(coalesce(layer_subtype,''))=case when upper(p_entity_type)='CABLE' then upper(v_cable_type) else upper(coalesce(layer_subtype,'')) end)
and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer) )

union
select lm.system_id,lm.entity_type,lm.common_name,cbl.cable_type,lm.geom_type,lm.display_name from (
select
pm.system_id,
pm.entity_type,pm.common_name,'Line' as geom_type,pm.display_name
from line_master pm
where
upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
and
ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer) )) lm
left join att_details_cable cbl on lm.system_id=cbl.system_id and upper(lm.entity_type)='CABLE'
where cable_type is null or upper(cable_type) in (select upper(layer_subtype) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
) tblmain
inner join
(
select entity_system_id,entity_network_id,entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
union
select associated_system_id,associated_network_id,associated_entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
) ass on tblmain.system_id=ass.entity_system_id and upper(tblmain.entity_type)=upper(ass.entity_type)
--and ass.entity_system_id!=p_old_entity_system_id
and (upper(ass.entity_type)!=upper(p_entity_type))
--left join user_master um on um.user_id=ass.created_by
left join layer_details ld on upper(ld.layer_name)=upper(tblmain.entity_type)
where case when ld.is_isp_layer=true and ld.geom_type='Point' and ass.entity_system_id is null then 1=2 else 1=1 end;

RAISE INFO ' % v_geometry', v_geometry;

---Update loop code
IF((select count(pm.system_id)
from point_master pm
inner join att_details_loop lp on pm.system_id=lp.system_id
where lp.associated_system_id=p_old_entity_system_id and upper(pm.entity_type) =upper('LOOP') and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer)))>0)
THEN
update att_details_loop set associated_system_id=p_entity_one_system_id,associated_network_id=p_entity_one_network_id,cable_system_id=p_entity_one_system_id
where cable_system_id=p_old_entity_system_id;
END IF;

------------------------------------------------------------LEFT SIDE ASSOCIATION---------------------------------------------------------------------------------
insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,associated_entity_type,associated_display_name,created_on,created_by)
select p_entity_two_system_id,p_entity_two_network_id,p_entity_type,v_two_display_name,ass.entity_system_id,ass.entity_network_id,ass.entity_type,display_name ,now(),v_user_id
from (
select
pm.system_id,
pm.entity_type,pm.common_name,null as cable_type,'Point' as geom_type,pm.display_name
from point_master pm
where upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type) and upper(coalesce(layer_subtype,''))=case when upper(p_entity_type)='CABLE' then upper(v_cable_type) else upper(coalesce(layer_subtype,'')) end)
and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer) )

union
select lm.system_id,lm.entity_type,lm.common_name,cbl.cable_type,lm.geom_type,lm.display_name from (
select
pm.system_id,
pm.entity_type,pm.common_name,'Line' as geom_type,pm.display_name
from line_master pm
where
upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
and
ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer) )) lm
left join att_details_cable cbl on lm.system_id=cbl.system_id and upper(lm.entity_type)='CABLE'
where cable_type is null or upper(cable_type) in (select upper(layer_subtype) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
) tblmain
inner join
(
select entity_system_id,entity_network_id,entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
union
select associated_system_id,associated_network_id,associated_entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
) ass on tblmain.system_id=ass.entity_system_id and upper(tblmain.entity_type)=upper(ass.entity_type)
--and ass.entity_system_id!=p_old_entity_system_id
and (upper(ass.entity_type)!=upper(p_entity_type))
--left join user_master um on um.user_id=ass.created_by
left join layer_details ld on upper(ld.layer_name)=upper(tblmain.entity_type)
where case when ld.is_isp_layer=true and ld.geom_type='Point' and ass.entity_system_id is null then 1=2 else 1=1 end;

RAISE INFO ' % v_geometry2 ', v_geometry2;

---Update loop code
IF((select count(pm.system_id) from point_master pm
inner join att_details_loop lp on pm.system_id=lp.system_id
where lp.associated_system_id=p_old_entity_system_id and upper(pm.entity_type) =upper('LOOP') and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer)))>0)
THEN
update att_details_loop set associated_system_id=p_entity_two_system_id,associated_network_id=p_entity_two_network_id,cable_system_id=p_entity_two_system_id
where cable_system_id=p_old_entity_system_id;
END IF;


---FIBER LINK ASSOCIATION BY ANTRA

select system_id into v_entity_one_system_id from att_details_cable where network_id=p_entity_one_network_id;
select system_id into v_entity_two_system_id from att_details_cable where network_id=p_entity_two_network_id;

update att_details_cable_info t set link_system_id=tmp.link_system_id from
(
select a.link_system_id ,a.fiber_number from att_details_cable_info a
WHERE a.cable_id=p_old_entity_system_id
)
tmp where t.fiber_number=tmp.fiber_number and t.cable_id=v_entity_one_system_id;

update att_details_cable_info t set link_system_id=tmp.link_system_id from
(
select a.link_system_id ,a.fiber_number from att_details_cable_info a
WHERE a.cable_id=p_old_entity_system_id
)
tmp where t.fiber_number=tmp.fiber_number and t.cable_id=v_entity_two_system_id;

----END FIBER LINK ASSOCIATION 

END;
$function$
;
