update res_resources set value ='Route Identification' where key='SI_OSP_TCH_NET_FRM_011' and culture in('en','fr','ja-JP','ru-RU','de-DE');
update res_resources set value ='????? ?? ?????' where key='SI_OSP_TCH_NET_FRM_011' and culture in('hi');

update layer_columns_settings set display_name ='Route Identification' 
where layer_id=13 and table_name='vw_att_details_manhole_report' and display_name='Mcgm Ward';

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_GBL_NET_GBL_280', 'Hierarchy Type', true, true, 'English', 'Smart Inventory_Osp_Global_Dot Net_', 0, now(), now(), 1, false, false);

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_GBL_NET_GBL_280', '????????? ??????', false, true, 'Hindi', 'Smart Inventory_Osp_Global_Dot Net_', 1, now(), now(), 1, false, false);

INSERT INTO public.form_input_settings(
	 form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, is_readonly)
	VALUES ( 'Manhole', 'hierarchy_type', 'field', true, false, 1, now(),false);
	
INSERT INTO public.form_input_settings(
	 form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, is_readonly)
	VALUES ( 'Cable', 'hierarchy_type', 'field', true, false, 1, now(),false);
	
INSERT INTO public.form_input_settings(
	 form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, is_readonly)
	VALUES ( 'SpliceClosure', 'hierarchy_type', 'field', true, false, 1, now(),false);
	
	INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 13, 'Hierarchy_type', 'Spiral', true, 1, now(),'Spiral', 'Hierarchy_type', true, true, 0);
	
	INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 19, 'Hierarchy_type', 'Spiral', true, 1, now(),'Spiral', 'Hierarchy_type', true, true, 0);
	
	INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 20, 'Hierarchy_type', 'Spiral', true, 1, now(),'Spiral', 'Hierarchy_type', true, true, 0);
	




	CREATE OR REPLACE FUNCTION public.fn_get_item_specification(
	entitytype character varying,
	typeid integer,
	brandid integer,
	specification character varying)
    RETURNS TABLE(key character varying, value character varying, ddtype character varying, type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
sql TEXT;
v_layer_id integer;
BEGIN
select layer_id into v_layer_id from layer_details where upper(layer_name)=upper(entitytype);

sql:= 'select specification as key,specification as value, ''Specification''::character varying as dd_type,specify_type as type
from item_template_master where layer_id='||v_layer_id||' and is_active=true
union
select prop_value as key,id::character varying as value, ''Accessibility''::character varying as dd_type,''''::character varying as type
from system_spec_master where lower(prop_name)=lower(''accessibility'')
union
select prop_value as key,id::character varying as value, ''Construction''::character varying as dd_type,''''::character varying as type
from system_spec_master where lower(prop_name)=lower(''construction'')
union
select prop_value as key,id::character varying as value, ''Activation''::character varying as dd_type,''''::character varying as type
from system_spec_master where lower(prop_name)=lower(''activation'')
union
select dropdown_key as key,dropdown_value::character varying as value, ''TypeMaster''::character varying as dd_type ,''''::character varying as type
from dropdown_master where dropdown_type=''Splitter_Type''
union
select dropdown_key as key,dropdown_value::character varying as value, ''type''::character varying as dd_type ,''''::character varying as type
from dropdown_master where layer_id='||v_layer_id||' and dropdown_type=''type''    
union
select dropdown_key as key,dropdown_value::character varying as value, ''Hierarchy_type''::character varying as dd_type ,''''::character varying as type
from dropdown_master where layer_id='||v_layer_id||' and dropdown_type=''Hierarchy_type''';

raise info 'V_SQLQUERY: %', sql;

if typeid !=0 then
sql:=sql ||' union select brand as key, id::character varying as value,''Brand''::character varying as dd_type,''''::character varying as type
from isp_brand_master where type_id='||typeid||' ';
end if;
if brandid !=0 then
sql:=sql ||' union select model as key, id::character varying as value,''Model''::character varying as dd_type,''''::character varying as type
from isp_base_model where brand_id='||brandid||' ';
end if;
if specification != '' then
sql:=sql ||' union select distinct i.vendor_id::character varying as key , v.name::character varying as value,''Vendor''::character varying as ddtype,''''::character varying as type
from item_template_master i inner join vendor_master v on i.vendor_id= v.id where lower(i.specification)=lower('''||specification||''' ) and i.is_active=true and i.is_master=true';
end if;

sql:= sql ||' order by dd_type,key';

RETURN QUERY
EXECUTE sql;
END;
$BODY$;



INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_CAB_NET_FRM_073', 'Section Name', true, true, 'English', 'Smart Inventory_Osp_Cable_Dot Net_', 0, now(), now(), 1, false, false);

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_CAB_NET_FRM_073', '?????? ?? ???', false, true, 'Hindi', 'Smart Inventory_Osp_cable_Dot Net_', 1, now(), now(), 1, false, false);


INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_CAB_NET_FRM_074', 'Generic Section Name', true, true, 'English', 'Smart Inventory_Osp_Cable_Dot Net_', 0, now(), now(), 1, false, false);

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_CAB_NET_FRM_074', '??????? ?????? ?? ???', false, true, 'Hindi', 'Smart Inventory_Osp_cable_Dot Net_', 1, now(), now(), 1, false, false);


INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_CAB_NET_FRM_075', 'Arial Location', true, true, 'English', 'Smart Inventory_Osp_Cable_Dot Net_', 0, now(), now(), 1, false, false);

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_CAB_NET_FRM_075', '????? ?????', false, true, 'Hindi', 'Smart Inventory_Osp_cable_Dot Net_', 1, now(), now(), 1, false, false);

alter table att_details_cable add column generic_section_name character varying(1000);

alter table att_details_manhole add column generic_section_name character varying(1000);

alter table att_details_spliceclosure add column generic_section_name character varying(1000);
alter table att_details_spliceclosure add column section_name character varying(1000);
alter table att_details_spliceclosure add column aerial_location character varying(1000);
alter table att_details_spliceclosure add column hierarchy_type character varying(1000);


 INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 19, 'Aerial_Location', 'Neutral', true, 1, now(),'Aerial', 'Aerial_Location', false, true, 0);
    
     INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 13, 'Aerial_Location', 'Neutral', true, 1, now(),'Aerial', 'Aerial_Location', false, true, 0);
    
     INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 20, 'Aerial_Location', 'Neutral', true, 1, now(),'Aerial', 'Aerial_Location', false, true, 0);

 alter table att_details_other add column own_vendor_id character varying(100);
 alter table att_details_loop add column own_vendor_id character varying(100);
 alter table att_details_bld_structure add column own_vendor_id character varying(100);
 alter table att_details_sector add column own_vendor_id character varying(100);
 alter table att_details_rack add column own_vendor_id character varying(100);
 alter table att_details_Antenna add column own_vendor_id character varying(100);
 alter table att_details_patchpanel add column own_vendor_id character varying(100);
 alter table att_details_handhole add column own_vendor_id character varying(100);
 alter table att_details_cabinet add column own_vendor_id character varying(100);
 alter table att_details_tower add column own_vendor_id character varying(100);
 alter table att_details_building add column own_vendor_id character varying(100);
 alter table att_details_duct add column own_vendor_id character varying(100);
 alter table att_details_Slack add column own_vendor_id character varying(100);
 alter table att_details_model add column own_vendor_id character varying(100);
 alter table att_details_fault add column own_vendor_id character varying(100);
 alter table att_details_cdb add column own_vendor_id character varying(100);
 alter table att_details_adb add column own_vendor_id character varying(100);
 alter table att_details_splitter add column own_vendor_id character varying(100);
 alter table isp_htb_info add column own_vendor_id character varying(100);
 alter table att_details_spliceclosure add column own_vendor_id character varying(100);
 alter table att_details_mpod add column own_vendor_id character varying(100);
 alter table att_details_fms add column own_vendor_id character varying(100);
 alter table att_details_trench add column own_vendor_id character varying(100);
 alter table isp_fdb_info add column own_vendor_id character varying(100);
 alter table att_details_tree add column own_vendor_id character varying(100);
 alter table att_details_pod add column own_vendor_id character varying(100);
 alter table att_details_ont add column own_vendor_id character varying(100);
 alter table att_details_wallmount add column own_vendor_id character varying(100);
 alter table att_details_bdb add column own_vendor_id character varying(100);
 alter table att_details_manhole add column own_vendor_id character varying(100);
 alter table att_details_microduct add column own_vendor_id character varying(100);
 alter table att_details_pole add column own_vendor_id character varying(100);
 alter table att_details_cable add column own_vendor_id character varying(100);
 
 
 
 
 
 
  DROP VIEW public.vw_att_details_cable;

CREATE OR REPLACE VIEW public.vw_att_details_cable
 AS
 SELECT cable.system_id,
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
    cable.route_name,
    cable.hierarchy_type,
    cable.section_name,
    cable.aerial_location,
    cable.cable_remark,
    cable.generic_section_name,
    cable.own_vendor_id
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



 DROP VIEW public.vw_att_details_manhole;
CREATE OR REPLACE VIEW public.vw_att_details_manhole
 AS
 SELECT manhole.system_id,
    manhole.network_id,
    manhole.manhole_name,
    round(manhole.latitude::numeric, 6) AS latitude,
    round(manhole.longitude::numeric, 6) AS longitude,
    manhole.address,
    manhole.specification,
    manhole.category,
    manhole.subcategory1,
    manhole.subcategory2,
    manhole.subcategory3,
    manhole.item_code,
    manhole.vendor_id,
    manhole.type,
    manhole.brand,
    manhole.model,
    manhole.construction,
    manhole.activation,
    manhole.accessibility,
    manhole.status,
    manhole.network_status,
    manhole.province_id,
    manhole.region_id,
    manhole.created_by,
    manhole.created_on,
    manhole.modified_by,
    manhole.modified_on,
    prov.province_name,
    reg.region_name,
    manhole.parent_system_id,
    manhole.parent_network_id,
    manhole.parent_entity_type,
    manhole.sequence_id,
    manhole.project_id,
    manhole.planning_id,
    manhole.purpose_id,
    manhole.workorder_id,
    manhole.is_virtual,
    manhole.construction_type,
    manhole.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS manhole_type,
    bd.brand AS manhole_brand,
    ml.model AS manhole_model,
    fn_get_date(manhole.created_on) AS created_date,
    fn_get_date(manhole.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.accessibility) AS manhole_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.construction) AS manhole_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.activation) AS manhole_activation,
    manhole.barcode,
    manhole.is_buried,
    manhole.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    manhole.source_ref_type,
    manhole.source_ref_id,
    manhole.source_ref_description,
    manhole.status_remark,
    manhole.status_updated_by,
    manhole.status_updated_on,
    manhole.is_visible_on_map,
    manhole.primary_pod_system_id,
    manhole.secondary_pod_system_id,
    manhole.is_new_entity,
    manhole.remarks,
    manhole.manhole_types,
    manhole.audit_item_master_id,
    manhole.is_acquire_from,
    manhole.origin_from,
    manhole.origin_ref_id,
    manhole.origin_ref_code,
    manhole.origin_ref_description,
    manhole.request_ref_id,
    manhole.requested_by,
    manhole.request_approved_by,
    manhole.subarea_id,
    manhole.area_id,
    manhole.dsa_id,
    manhole.csa_id,
    manhole.bom_sub_category,
    manhole.gis_design_id,
    manhole.st_x,
    manhole.st_y,
    manhole.ne_id,
    manhole.prms_id,
    manhole.jc_id,
    manhole.mzone_id,
    manhole.area,
    manhole.authority,
    manhole.route_name,
    manhole.mcgm_ward,
    manhole.hierarchy_type,
    manhole.section_name,
    manhole.aerial_location,
    manhole.chamber_remark,
    manhole.generic_section_name,
    manhole.own_vendor_id
   FROM att_details_manhole manhole
     JOIN province_boundary prov ON manhole.province_id = prov.id
     JOIN region_boundary reg ON manhole.region_id = reg.id
     JOIN user_master um ON manhole.created_by = um.user_id
     JOIN vendor_master vm ON manhole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON manhole.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON manhole.type = tp.id
     LEFT JOIN isp_brand_master bd ON manhole.brand = bd.id
     LEFT JOIN isp_base_model ml ON manhole.model = ml.id
     LEFT JOIN user_master um2 ON manhole.modified_by = um2.user_id;


INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_GBL_GBL_GBL_GBL_166', 'Own Vendor:', true, true, 'English', 'Smart Inventory_Osp_Global_Dot Net_', 0, now(), now(), 1, false, false);

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_GBL_GBL_GBL_GBL_166', '????? ?? ????????:', false, true, 'Hindi', 'Smart Inventory_Osp_Global_Dot Net_', 1, now(), now(), 1, false, false);



 DROP VIEW public.vw_att_details_spliceclosure;
CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure
 AS
 SELECT sc.system_id,
    sc.network_id,
    sc.spliceclosure_name,
    round(sc.latitude::numeric, 6) AS latitude,
    round(sc.longitude::numeric, 6) AS longitude,
    sc.address,
    sc.specification,
    sc.category,
    sc.subcategory1,
    sc.subcategory2,
    sc.subcategory3,
    sc.item_code,
    sc.vendor_id,
    sc.type,
    sc.brand,
    sc.model,
    sc.construction,
    sc.activation,
    sc.accessibility,
    sc.status,
    sc.network_status,
    sc.province_id,
    sc.region_id,
    sc.created_by,
    sc.created_on,
    sc.modified_by,
    sc.modified_on,
    sc.pincode,
    prov.province_name,
    reg.region_name,
    sc.parent_system_id,
    sc.parent_network_id,
    sc.parent_entity_type,
    sc.sequence_id,
    sc.project_id,
    sc.planning_id,
    sc.purpose_id,
    sc.workorder_id,
    sc.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS sc_type,
    bd.brand AS sc_brand,
    ml.model AS sc_model,
    fn_get_date(sc.created_on) AS created_date,
    fn_get_date(sc.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    sc.is_virtual,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.accessibility) AS sc_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.construction) AS sc_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.activation) AS sc_activation,
    sc.no_of_ports,
    sc.no_of_input_port,
    sc.no_of_output_port,
    ''::character varying AS additional_attributes,
        CASE
            WHEN sc.no_of_input_port <> 0 AND sc.no_of_output_port <> 0 THEN ((sc.no_of_input_port::character varying::text || ''::text) || sc.no_of_output_port::character varying::text)::character varying
            ELSE sc.no_of_ports::character varying
        END AS total_ports,
    ''::character varying AS entity_category,
    sc.spliceclosure_name AS entity_name,
    sc.barcode,
    sc.is_buried,
    sc.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    sc.source_ref_type,
    sc.source_ref_id,
    sc.source_ref_description,
    sc.status_remark,
    sc.status_updated_by,
    sc.status_updated_on,
    sc.is_visible_on_map,
    sc.primary_pod_system_id,
    sc.secondary_pod_system_id,
    sc.is_new_entity,
    sc.remarks,
    sc.audit_item_master_id,
    sc.is_acquire_from,
    s.shaft_name,
    f.floor_name,
    sc.origin_from,
    sc.origin_ref_id,
    sc.origin_ref_code,
    sc.origin_ref_description,
    sc.request_ref_id,
    sc.requested_by,
    sc.request_approved_by,
    sc.subarea_id,
    sc.area_id,
    sc.dsa_id,
    sc.csa_id,
    sc.bom_sub_category,
    sc.gis_design_id,
    sc.st_x,
    sc.st_y,
    sc.ne_id,
    sc.prms_id,
    sc.jc_id,
    sc.mzone_id,
    sc.hierarchy_type,
    sc.section_name,
    sc.aerial_location,
    sc.generic_section_name,
    sc.own_vendor_id
   FROM att_details_spliceclosure sc
     JOIN province_boundary prov ON sc.province_id = prov.id
     JOIN region_boundary reg ON sc.region_id = reg.id
     JOIN user_master um ON sc.created_by = um.user_id
     JOIN vendor_master vm ON sc.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sc.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON sc.type = tp.id
     LEFT JOIN isp_brand_master bd ON sc.brand = bd.id
     LEFT JOIN isp_base_model ml ON sc.model = ml.id
     LEFT JOIN user_master um2 ON sc.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'SPLICECLOSURE'::text) m1 ON sc.system_id = m1.entity_id
     LEFT JOIN isp_shaft_info s ON m1.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m1.floor_id = f.system_id;



 DROP VIEW public.vw_att_details_splitter;
CREATE OR REPLACE VIEW public.vw_att_details_splitter
 AS
 SELECT splt.system_id,
    splt.network_id,
    splt.splitter_name,
    round(splt.latitude::numeric, 6) AS latitude,
    round(splt.longitude::numeric, 6) AS longitude,
    splt.address,
    splt.splitter_ratio,
    splt.specification,
    splt.category,
    splt.subcategory1,
    splt.subcategory2,
    splt.subcategory3,
    splt.item_code,
    splt.vendor_id,
    splt.type,
    splt.brand,
    splt.model,
    splt.construction,
    splt.activation,
    splt.accessibility,
    splt.status,
    splt.network_status,
    splt.province_id,
    splt.region_id,
    splt.created_by,
    splt.created_on,
    splt.modified_by,
    splt.modified_on,
    splt.project_id,
    splt.planning_id,
    splt.purpose_id,
    splt.workorder_id,
    splt.parent_system_id,
    splt.parent_entity_type,
    splt.parent_network_id,
    prov.province_name,
    reg.region_name,
    splt.splitter_type,
    splt.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    bd.brand AS splitter_brand,
    ml.model AS splitter_model,
    fn_get_date(splt.created_on) AS created_date,
    fn_get_date(splt.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = splt.accessibility) AS splitter_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = splt.construction) AS splitter_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = splt.activation) AS splitter_activation,
    splt.splitter_type AS additional_attributes,
    splt.splitter_ratio AS total_ports,
    splt.splitter_type AS entity_category,
    splt.splitter_name AS entity_name,
    splt.barcode,
    splt.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    splt.source_ref_type,
    splt.source_ref_id,
    splt.source_ref_description,
    splt.status_remark,
    splt.status_updated_by,
    splt.status_updated_on,
    splt.is_visible_on_map,
    splt.primary_pod_system_id,
    splt.secondary_pod_system_id,
    splt.is_new_entity,
    splt.remarks,
    splt.audit_item_master_id,
    splt.is_acquire_from,
    splt.origin_from,
    splt.origin_ref_id,
    splt.origin_ref_code,
    splt.origin_ref_description,
    splt.request_ref_id,
    splt.requested_by,
    splt.request_approved_by,
    splt.subarea_id,
    splt.area_id,
    splt.dsa_id,
    splt.csa_id,
    splt.bom_sub_category,
    splt.gis_design_id,
    splt.st_x,
    splt.st_y,
    splt.ne_id,
    splt.prms_id,
    splt.jc_id,
    splt.mzone_id,
    splt.power_meter_reading,
    splt.own_vendor_id
   FROM att_details_splitter splt
     JOIN province_boundary prov ON splt.province_id = prov.id
     JOIN region_boundary reg ON splt.region_id = reg.id
     JOIN user_master um ON splt.created_by = um.user_id
     JOIN vendor_master vm ON splt.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON splt.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON splt.type = tp.id
     LEFT JOIN isp_brand_master bd ON splt.brand = bd.id
     LEFT JOIN isp_base_model ml ON splt.model = ml.id
     LEFT JOIN user_master um2 ON splt.modified_by = um2.user_id;

ALTER TABLE public.vw_att_details_splitter
    OWNER TO postgres;





 DROP VIEW public.vw_att_details_other;

CREATE OR REPLACE VIEW public.vw_att_details_other
 AS
 SELECT othr.system_id,
    othr.network_id,
    othr.entity_type,
    othr.construction,
    othr.activation,
    othr.accessibility,
    othr.specification,
    othr.category,
    othr.subcategory1,
    othr.subcategory2,
    othr.subcategory3,
    othr.item_code,
    othr.vendor_id,
    othr.type,
    othr.brand,
    othr.model,
    othr.network_status,
    othr.status,
    othr.pin_code,
    othr.province_id,
    othr.region_id,
    othr.remarks,
    othr.created_by,
    othr.created_on,
    othr.modified_by,
    othr.modified_on,
    othr.parent_system_id,
    othr.parent_network_id,
    othr.parent_entity_type,
    othr.cost_per_unit,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS cable_specification_type,
    bd.brand AS cable_brand,
    ml.model AS cable_model,
    fn_get_date(othr.created_on) AS created_date,
    fn_get_date(othr.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = othr.accessibility) AS othr_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = othr.construction) AS othr_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = othr.activation) AS othr_activation,
    othr.origin_from,
    othr.origin_ref_id,
    othr.origin_ref_code,
    othr.origin_ref_description,
    othr.request_ref_id,
    othr.requested_by,
    othr.request_approved_by,
    othr.st_x,
    othr.st_y,
    othr.own_vendor_id
   FROM att_details_other othr
     JOIN province_boundary prov ON othr.province_id = prov.id
     JOIN region_boundary reg ON othr.region_id = reg.id
     JOIN user_master um ON othr.created_by = um.user_id
     JOIN vendor_master vm ON othr.vendor_id = vm.id
     LEFT JOIN isp_type_master tp ON othr.type = tp.id
     LEFT JOIN isp_brand_master bd ON othr.brand = bd.id
     LEFT JOIN isp_base_model ml ON othr.model = ml.id
     LEFT JOIN user_master um2 ON othr.modified_by = um2.user_id;



 DROP VIEW public.vw_att_details_loop;

CREATE OR REPLACE VIEW public.vw_att_details_loop
 AS
 SELECT lop.system_id,
    lop.network_id,
    round(lop.loop_length::numeric, 2) AS loop_length,
    lop.associated_system_id,
    lop.associated_network_id,
    lop.associated_entity_type,
    cbl.network_id AS cable_network_id,
    fn_get_date(lop.created_on, true) AS created_on,
    um.user_name AS created_by_user,
    fn_get_date(lop.modified_on, false) AS modified_on,
    um2.user_name AS modified_by_user,
    lop.status,
    lop.province_id,
    lop.region_id,
    lop.status_remark,
    lop.status_updated_by,
    lop.status_updated_on,
    lop.is_visible_on_map,
    lop.source_ref_type,
    lop.source_ref_id,
    lop.source_ref_description,
    lop.created_by,
    lop.modified_by,
    round(lop.start_reading::numeric, 2) AS start_reading,
    round(lop.end_reading::numeric, 2) AS end_reading,
    cbl.system_id AS cable_id,
    lop.cable_system_id,
    lop.network_status,
    lop.parent_system_id,
    lop.parent_network_id,
    lop.parent_entity_type,
    lop.sequence_id,
    round(lop.latitude::numeric, 6) AS latitude,
    round(lop.longitude::numeric, 6) AS longitude,
    lop.is_new_entity,
    lop.project_id,
    lop.planning_id,
    lop.purpose_id,
    lop.workorder_id,
    lop.origin_from,
    lop.origin_ref_id,
    lop.origin_ref_code,
    lop.origin_ref_description,
    lop.request_ref_id,
    lop.requested_by,
    lop.request_approved_by,
    lop.subarea_id,
    lop.area_id,
    lop.dsa_id,
    lop.csa_id,
    reg.region_name,
    prov.province_name,
    lop.gis_design_id,
    lop.st_x,
    lop.st_y,
    lop.ne_id,
    lop.prms_id,
    lop.jc_id,
    lop.mzone_id,
    lop.own_vendor_id
   FROM att_details_loop lop
     LEFT JOIN att_details_cable cbl ON lop.cable_system_id = cbl.system_id
     JOIN province_boundary prov ON lop.province_id = prov.id
     JOIN region_boundary reg ON lop.region_id = reg.id
     JOIN user_master um ON lop.created_by = um.user_id
     LEFT JOIN user_master um2 ON lop.modified_by = um2.user_id;





 DROP VIEW public.vw_att_details_bld_structure;

CREATE OR REPLACE VIEW public.vw_att_details_bld_structure
 AS
 SELECT bld.system_id,
    bld.network_id,
    bld.structure_name,
    bld.business_pass,
    bld.home_pass,
    bld.no_of_floor,
    bld.no_of_flat,
    bld.no_of_occupants,
    bld.no_of_shaft,
    bld.building_id,
    bld.region_id,
    bld.province_id,
    bld.network_status,
    bld.status,
    bld.remarks,
    bld.created_by,
    bld.created_on,
    bld.modified_by,
    bld.modified_on,
    round(pm.latitude::numeric, 6) AS latitude,
    round(pm.longitude::numeric, 6) AS longitude,
    prov.province_name,
    reg.region_name,
    bld.parent_system_id,
    bld.parent_network_id,
    bld.parent_entity_type,
    bld.sequence_id,
    um.user_name,
    um2.user_name AS modified_by_user,
    bld.owner_name,
    bld.owner_address,
    bld.mobile_number,
    bld.fax_number,
    bld.email_id,
    round(bld.structure_height::numeric, 2) AS structure_height,
    bld.status_remark,
    bld.status_updated_by,
    bld.status_updated_on,
    bld.is_visible_on_map,
    bld.is_new_entity,
    um.user_name AS created_by_user,
    bld.no_of_flat AS no_of_units,
    bld.origin_from,
    bld.origin_ref_id,
    bld.origin_ref_code,
    bld.origin_ref_description,
    bld.request_ref_id,
    bld.requested_by,
    bld.request_approved_by,
    bld.subarea_id,
    bld.area_id,
    bld.dsa_id,
    bld.csa_id,
    bld.gis_design_id,
    bld.st_x,
    bld.st_y,
    bld.ne_id,
    bld.prms_id,
    bld.jc_id,
    bld.mzone_id,
    bld.own_vendor_id
   FROM att_details_bld_structure bld
     JOIN point_master pm ON bld.system_id = pm.system_id
     JOIN province_boundary prov ON bld.province_id = prov.id
     JOIN region_boundary reg ON bld.region_id = reg.id
     JOIN user_master um ON bld.created_by = um.user_id
     LEFT JOIN user_master um2 ON bld.modified_by = um2.user_id
  WHERE pm.entity_type::text = 'Structure'::text;



DROP VIEW public.vw_att_details_sector;

CREATE OR REPLACE VIEW public.vw_att_details_sector
 AS
 SELECT sector.system_id,
    sector.network_id,
    sector.network_name,
    sector.parent_system_id,
    sector.parent_network_id,
    sector.parent_entity_type,
    sector.sequence_id,
    sector.latitude,
    sector.longitude,
    sector.region_id,
    sector.province_id,
    sector.azimuth,
    sector.technology,
    sector.port_name,
    sector.down_link,
    sector.uplink,
    sector.brand_name,
    sector.remark,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    sector.ownership_type,
    sector.source_ref_type,
    sector.source_ref_id,
    sector.source_ref_description,
    sector.audit_item_master_id,
    sector.status_remark,
    sector.status_updated_by,
    sector.status_updated_on,
    sector.network_status,
    sector.status,
    sector.created_by,
    sector.created_on,
    sector.modified_by,
    sector.modified_on,
    sector.project_id,
    sector.planning_id,
    sector.purpose_id,
    sector.workorder_id,
    sector.vendor_id,
    sector.specification,
    sector.category,
    sector.subcategory1,
    sector.subcategory2,
    sector.subcategory3,
    sector.item_code,
    prov.province_name,
    reg.region_name,
    sector.frequency,
    sector.sector_type,
    sector.parent_site_id,
    sector.sector_layer_id,
    sector.node_identity,
    um.user_name,
    vm.name AS vendor_name,
    fn_get_date(sector.created_on) AS created_date,
    fn_get_date(sector.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    sector.network_name AS entity_name,
    i.installation_id,
    i.installation_number,
    i.installation_year,
    i.production_year,
    i.installation_company,
    i.installation_technician,
    i.installation,
    sector.is_visible_on_map,
    sector.is_new_entity,
    sector.remarks,
    round(sector.total_tilt::numeric, 2) AS total_tilt,
    sector.origin_from,
    sector.origin_ref_id,
    sector.origin_ref_code,
    sector.origin_ref_description,
    sector.request_ref_id,
    sector.requested_by,
    sector.request_approved_by,
    sector.subarea_id,
    sector.area_id,
    sector.dsa_id,
    sector.csa_id,
    sector.gis_design_id,
    sector.ne_id,
    sector.prms_id,
    sector.jc_id,
    sector.mzone_id,
    sector.own_vendor_id
   FROM att_details_sector sector
     JOIN province_boundary prov ON sector.province_id = prov.id
     JOIN region_boundary reg ON sector.region_id = reg.id
     JOIN user_master um ON sector.created_by = um.user_id
     JOIN vendor_master vm ON sector.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sector.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON sector.modified_by = um2.user_id
     LEFT JOIN installation_info i ON i.entity_system_id = sector.system_id AND upper(i.entity_type::text) = 'SECTOR'::text;




DROP VIEW public.vw_att_details_rack;

CREATE OR REPLACE VIEW public.vw_att_details_rack
 AS
 SELECT rack.system_id,
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
    rack.parent_entity_type,
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



 DROP VIEW public.vw_att_details_antenna;

CREATE OR REPLACE VIEW public.vw_att_details_antenna
 AS
 SELECT ant.system_id,
    ant.network_id,
    ant.network_name,
    round(ant.latitude::numeric, 6) AS latitude,
    round(ant.longitude::numeric, 6) AS longitude,
    ant.parent_system_id,
    ant.parent_network_id,
    ant.parent_entity_type,
    ant.sequence_id,
    ant.antenna_type,
    ant.antenna_sub_type,
    round(ant.minimum_frequency::numeric, 2) AS minimum_frequency,
    round(ant.maximum_frequency::numeric, 2) AS maximum_frequency,
    round(ant.diameter_in_meter::numeric, 2) AS diameter_in_meter,
    round(ant.maximum_gain::numeric, 2) AS maximum_gain,
    round(ant.boresight_gain::numeric, 2) AS boresight_gain,
    ant.user_cross_polor_pattern,
    round(ant.co_polor_vertical_maximum_gain::numeric, 2) AS co_polor_vertical_maximum_gain,
    round(ant.co_polor_vertical_fb::numeric, 2) AS co_polor_vertical_fb,
    round(ant.co_polor_vertical_bw::numeric, 2) AS co_polor_vertical_bw,
    round(ant.co_polor_vertical_boresight::numeric, 2) AS co_polor_vertical_boresight,
    round(ant.cross_polor_vertical_maximum_gain::numeric, 2) AS cross_polor_vertical_maximum_gain,
    round(ant.cross_polor_vertical_fb::numeric, 2) AS cross_polor_vertical_fb,
    round(ant.cross_polor_vertical_bw::numeric, 2) AS cross_polor_vertical_bw,
    round(ant.cross_polor_vertical_boresight::numeric, 2) AS cross_polor_vertical_boresight,
    round(ant.co_polor_horizontal_maximum_gain::numeric, 2) AS co_polor_horizontal_maximum_gain,
    round(ant.co_polor_horizontal_fb::numeric, 2) AS co_polor_horizontal_fb,
    round(ant.co_polor_horizontal_bw::numeric, 2) AS co_polor_horizontal_bw,
    round(ant.cross_polor_horizontal_maximum_gain::numeric, 2) AS cross_polor_horizontal_maximum_gain,
    round(ant.co_polor_horizontal_boresight::numeric, 2) AS co_polor_horizontal_boresight,
    round(ant.cross_polor_horizontal_fb::numeric, 2) AS cross_polor_horizontal_fb,
    round(ant.cross_polor_horizontal_bw::numeric, 2) AS cross_polor_horizontal_bw,
    round(ant.mechanical_tilt::numeric, 2) AS mechanical_tilt,
    round(ant.electrical_tilt::numeric, 2) AS electrical_tilt,
    round(ant.total_tilt::numeric, 2) AS total_tilt,
    ant.remark,
    vm.id AS third_party_vendor_id,
    vm.name AS third_party_vendor_name,
    ant.ownership_type,
    ant.source_ref_type,
    ant.source_ref_id,
    ant.source_ref_description,
    ant.audit_item_master_id,
    ant.status_remark,
    ant.status_updated_by,
    ant.vendor_id,
    ant.specification,
    vm.name AS vendor_name,
    ant.category,
    ant.subcategory1,
    ant.subcategory2,
    ant.subcategory3,
    ant.item_code,
    um3.user_name AS status_updated_by_user,
    fn_get_date(ant.status_updated_on) AS status_updated_date,
    ant.network_status,
    ant.status,
    ant.created_by,
    ant.created_on,
    ant.modified_by,
    ant.modified_on,
    um.user_name AS created_by_user,
    fn_get_date(ant.created_on) AS created_date,
    um2.user_name AS modified_by_user,
    fn_get_date(ant.modified_on) AS modified_date,
    round(ant.height::numeric, 2) AS height,
    round(ant.azimuth::numeric, 2) AS azimuth,
    ant.antenna_operator,
    ant.region_id,
    reg.region_name,
    ant.province_id,
    prov.province_name,
    ant.project_id,
    ant.planning_id,
    ant.workorder_id,
    ant.purpose_id,
    ant.is_new_entity,
    ant.remarks,
    advh.id AS vsat_id,
    ant.model_number,
    ant.manufacturer_name,
    ant.polarization,
    ant.origin_from,
    ant.origin_ref_id,
    ant.origin_ref_code,
    ant.origin_ref_description,
    ant.request_ref_id,
    ant.requested_by,
    ant.request_approved_by,
    ant.subarea_id,
    ant.area_id,
    ant.dsa_id,
    ant.csa_id,
    ant.bom_sub_category,
    ant.gis_design_id,
    ant.st_x,
    ant.st_y,
    ant.ne_id,
    ant.prms_id,
    ant.jc_id,
    ant.mzone_id,
    ant.own_vendor_id
   FROM att_details_antenna ant
     JOIN province_boundary prov ON ant.province_id = prov.id
     JOIN region_boundary reg ON ant.region_id = reg.id
     JOIN user_master um ON ant.created_by = um.user_id
     LEFT JOIN vendor_master vm ON ant.third_party_vendor_id = vm.id
     LEFT JOIN user_master um2 ON ant.modified_by = um2.user_id
     LEFT JOIN user_master um3 ON ant.status_updated_by = um3.user_id
     LEFT JOIN att_details_vsat_antenna advh ON advh.parent_system_id = ant.system_id
  ORDER BY ant.created_on;



 DROP VIEW public.vw_att_details_patchpanel;

CREATE OR REPLACE VIEW public.vw_att_details_patchpanel
 AS
 SELECT patchpanel.system_id,
    patchpanel.network_id,
    patchpanel.patchpanel_name,
    round(patchpanel.latitude::numeric, 6) AS latitude,
    round(patchpanel.longitude::numeric, 6) AS longitude,
    patchpanel.address,
    patchpanel.specification,
    patchpanel.category,
    patchpanel.subcategory1,
    patchpanel.subcategory2,
    patchpanel.subcategory3,
    patchpanel.item_code,
    patchpanel.vendor_id,
    patchpanel.type,
    patchpanel.brand,
    patchpanel.model,
    patchpanel.construction,
    patchpanel.activation,
    patchpanel.accessibility,
    patchpanel.status,
    patchpanel.network_status,
    patchpanel.province_id,
    patchpanel.region_id,
    patchpanel.created_by,
    patchpanel.created_on,
    patchpanel.modified_by,
    patchpanel.modified_on,
    patchpanel.pincode,
    prov.province_name,
    reg.region_name,
    patchpanel.parent_system_id,
    patchpanel.parent_network_id,
    patchpanel.parent_entity_type,
    patchpanel.sequence_id,
    patchpanel.project_id,
    patchpanel.planning_id,
    patchpanel.purpose_id,
    patchpanel.workorder_id,
    patchpanel.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    fn_get_date(patchpanel.created_on) AS created_date,
    fn_get_date(patchpanel.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    patchpanel.no_of_port,
    patchpanel.no_of_input_port,
    patchpanel.no_of_output_port,
    ''::character varying AS additional_attributes,
    (patchpanel.no_of_input_port::character varying::text || ''::text) || patchpanel.no_of_output_port::character varying::text AS total_ports,
    ''::text AS entity_category,
    patchpanel.patchpanel_name AS entity_name,
    patchpanel.barcode,
    patchpanel.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    patchpanel.source_ref_type,
    patchpanel.source_ref_id,
    patchpanel.source_ref_description,
    patchpanel.status_remark,
    patchpanel.status_updated_by,
    patchpanel.status_updated_on,
    patchpanel.is_visible_on_map,
    patchpanel.primary_pod_system_id,
    patchpanel.secondary_pod_system_id,
    patchpanel.is_new_entity,
    patchpanel.remarks,
    patchpanel.patchpanel_type,
    patchpanel.audit_item_master_id,
    patchpanel.is_acquire_from,
    patchpanel.origin_from,
    patchpanel.origin_ref_id,
    patchpanel.origin_ref_code,
    patchpanel.origin_ref_description,
    patchpanel.request_ref_id,
    patchpanel.requested_by,
    patchpanel.request_approved_by,
    patchpanel.subarea_id,
    patchpanel.area_id,
    patchpanel.dsa_id,
    patchpanel.csa_id,
    patchpanel.bom_sub_category,
    patchpanel.gis_design_id,
    patchpanel.st_x,
    patchpanel.st_y,
    patchpanel.ne_id,
    patchpanel.prms_id,
    patchpanel.jc_id,
    patchpanel.mzone_id,
    patchpanel.own_vendor_id
    FROM att_details_patchpanel patchpanel
     JOIN province_boundary prov ON patchpanel.province_id = prov.id
     JOIN region_boundary reg ON patchpanel.region_id = reg.id
     JOIN user_master um ON patchpanel.created_by = um.user_id
     JOIN vendor_master vm ON patchpanel.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON patchpanel.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON patchpanel.modified_by = um2.user_id;




 DROP VIEW public.vw_att_details_handhole;

CREATE OR REPLACE VIEW public.vw_att_details_handhole
 AS
 SELECT handhole.system_id,
    handhole.network_id,
    handhole.handhole_name,
    round(handhole.latitude::numeric, 6) AS latitude,
    round(handhole.longitude::numeric, 6) AS longitude,
    handhole.address,
    handhole.specification,
    handhole.category,
    handhole.subcategory1,
    handhole.subcategory2,
    handhole.subcategory3,
    handhole.item_code,
    handhole.vendor_id,
    handhole.type,
    handhole.brand,
    handhole.model,
    handhole.construction,
    handhole.activation,
    handhole.accessibility,
    handhole.status,
    handhole.network_status,
    handhole.province_id,
    handhole.region_id,
    handhole.created_by,
    handhole.created_on,
    handhole.modified_by,
    handhole.modified_on,
    prov.province_name,
    reg.region_name,
    handhole.parent_system_id,
    handhole.parent_network_id,
    handhole.parent_entity_type,
    handhole.sequence_id,
    handhole.project_id,
    handhole.planning_id,
    handhole.purpose_id,
    handhole.workorder_id,
    handhole.is_virtual,
    handhole.construction_type,
    handhole.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS handhole_type,
    bd.brand AS handhole_brand,
    ml.model AS handhole_model,
    fn_get_date(handhole.created_on) AS created_date,
    fn_get_date(handhole.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = handhole.accessibility) AS handhole_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = handhole.construction) AS handhole_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = handhole.activation) AS handhole_activation,
    handhole.barcode,
    handhole.is_buried,
    handhole.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    handhole.source_ref_type,
    handhole.source_ref_id,
    handhole.source_ref_description,
    handhole.status_remark,
    handhole.status_updated_by,
    handhole.status_updated_on,
    handhole.is_visible_on_map,
    handhole.primary_pod_system_id,
    handhole.secondary_pod_system_id,
    handhole.is_new_entity,
    handhole.remarks,
    handhole.audit_item_master_id,
    handhole.is_acquire_from,
    handhole.origin_from,
    handhole.origin_ref_id,
    handhole.origin_ref_code,
    handhole.origin_ref_description,
    handhole.request_ref_id,
    handhole.requested_by,
    handhole.request_approved_by,
    handhole.subarea_id,
    handhole.area_id,
    handhole.dsa_id,
    handhole.csa_id,
    handhole.bom_sub_category,
    handhole.gis_design_id,
    handhole.st_x,
    handhole.st_y,
    handhole.ne_id,
    handhole.prms_id,
    handhole.jc_id,
    handhole.mzone_id,
    handhole.own_vendor_id
   FROM att_details_handhole handhole
     JOIN province_boundary prov ON handhole.province_id = prov.id
     JOIN region_boundary reg ON handhole.region_id = reg.id
     JOIN user_master um ON handhole.created_by = um.user_id
     JOIN vendor_master vm ON handhole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON handhole.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON handhole.type = tp.id
     LEFT JOIN isp_brand_master bd ON handhole.brand = bd.id
     LEFT JOIN isp_base_model ml ON handhole.model = ml.id
     LEFT JOIN user_master um2 ON handhole.modified_by = um2.user_id;




 DROP VIEW public.vw_att_details_cabinet;

CREATE OR REPLACE VIEW public.vw_att_details_cabinet
 AS
 SELECT cabinet.system_id,
    cabinet.network_id,
    cabinet.cabinet_name,
    round(cabinet.latitude::numeric, 6) AS latitude,
    round(cabinet.longitude::numeric, 6) AS longitude,
    cabinet.pincode,
    cabinet.address,
    cabinet.specification,
    cabinet.category,
    cabinet.subcategory1,
    cabinet.subcategory2,
    cabinet.subcategory3,
    cabinet.item_code,
    cabinet.vendor_id,
    cabinet.type,
    cabinet.brand,
    cabinet.model,
    cabinet.construction,
    cabinet.activation,
    cabinet.accessibility,
    cabinet.status,
    cabinet.network_status,
    cabinet.province_id,
    cabinet.region_id,
    cabinet.created_by,
    cabinet.created_on,
    cabinet.modified_by,
    cabinet.modified_on,
    cabinet.parent_system_id,
    cabinet.parent_network_id,
    cabinet.parent_entity_type,
    cabinet.sequence_id,
    cabinet.project_id,
    cabinet.planning_id,
    cabinet.purpose_id,
    cabinet.workorder_id,
    cabinet.acquire_from,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS cabinet_types,
    bd.brand AS cabinet_brand,
    ml.model AS cabinet_model,
    fn_get_date(cabinet.created_on) AS created_date,
    fn_get_date(cabinet.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cabinet.accessibility) AS cabinet_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cabinet.construction) AS cabinet_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cabinet.activation) AS cabinet_activation,
    ''::character varying AS additional_attributes,
    ''::text AS total_ports,
    ''::text AS entity_category,
    cabinet.cabinet_name AS entity_name,
    cabinet.cabinet_type,
    cabinet.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    cabinet.source_ref_type,
    cabinet.source_ref_id,
    cabinet.source_ref_description,
    cabinet.status_remark,
    cabinet.status_updated_by,
    cabinet.status_updated_on,
    cabinet.is_visible_on_map,
    cabinet.remarks,
    cabinet.is_new_entity,
    cabinet.primary_pod_system_id,
    cabinet.secondary_pod_system_id,
    round(cabinet.length::numeric, 2) AS length,
    round(cabinet.width::numeric, 2) AS width,
    round(cabinet.height::numeric, 2) AS height,
    cabinet.audit_item_master_id,
    cabinet.is_acquire_from,
    cabinet.origin_from,
    cabinet.origin_ref_id,
    cabinet.origin_ref_code,
    cabinet.origin_ref_description,
    cabinet.request_ref_id,
    cabinet.requested_by,
    cabinet.request_approved_by,
    cabinet.subarea_id,
    cabinet.area_id,
    cabinet.dsa_id,
    cabinet.csa_id,
    prov.province_name,
    cabinet.bom_sub_category,
    cabinet.gis_design_id,
    cabinet.st_x,
    cabinet.st_y,
    cabinet.ne_id,
    cabinet.prms_id,
    cabinet.jc_id,
    cabinet.mzone_id,
    cabinet.own_vendor_id
   FROM att_details_cabinet cabinet
     JOIN province_boundary prov ON cabinet.province_id = prov.id
     JOIN region_boundary reg ON cabinet.region_id = reg.id
     JOIN user_master um ON cabinet.created_by = um.user_id
     JOIN vendor_master vm ON cabinet.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cabinet.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON cabinet.type = tp.id
     LEFT JOIN isp_brand_master bd ON cabinet.brand = bd.id
     LEFT JOIN isp_base_model ml ON cabinet.model = ml.id
     LEFT JOIN user_master um2 ON cabinet.modified_by = um2.user_id;
	 
	 
	 
	 
	  DROP VIEW public.vw_att_details_tower;

CREATE OR REPLACE VIEW public.vw_att_details_tower
 AS
 SELECT t.system_id,
    t.network_id,
    t.network_name,
    t.parent_system_id,
    t.parent_network_id,
    t.parent_entity_type,
    t.sequence_id,
    round(t.latitude::numeric, 6) AS latitude,
    round(t.longitude::numeric, 6) AS longitude,
    t.region_id,
    t.province_id,
    t.address,
    round(t.elevation::numeric, 2) AS elevation,
    round(t.tower_height::numeric, 6) AS tower_height,
    t.operator_name,
    t.no_of_sectors,
    t.tenancy,
    t.network_type,
    t.remark,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    t.ownership_type,
    t.source_ref_type,
    t.source_ref_id,
    t.source_ref_description,
    t.audit_item_master_id,
    t.status_remark,
    t.status_updated_by,
    t.status_updated_on,
    t.network_status,
    t.status,
    t.created_by,
    t.created_on,
    t.modified_by,
    t.modified_on,
    t.project_id,
    t.planning_id,
    t.purpose_id,
    t.workorder_id,
    t.acquire_from,
    t.vendor_id,
    t.specification,
    t.category,
    t.subcategory1,
    t.subcategory2,
    t.subcategory3,
    t.item_code,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    fn_get_date(t.created_on) AS created_date,
    fn_get_date(t.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    t.network_name AS entity_name,
    t.installation_number,
    t.installation_year,
    t.production_year,
    t.installation_company,
    t.installation_technician,
    t.installation,
    t.is_new_entity,
    t.origin_from,
    t.origin_ref_id,
    t.origin_ref_code,
    t.origin_ref_description,
    t.request_ref_id,
    t.requested_by,
    t.request_approved_by,
    t.subarea_id,
    t.area_id,
    t.dsa_id,
    t.csa_id,
    t.bom_sub_category,
    t.gis_design_id,
    t.st_x,
    t.st_y,
    t.ne_id,
    t.prms_id,
    t.jc_id,
    t.mzone_id,
    t.own_vendor_id
   FROM att_details_tower t
     JOIN province_boundary prov ON t.province_id = prov.id
     JOIN region_boundary reg ON t.region_id = reg.id
     JOIN user_master um ON t.created_by = um.user_id
     JOIN vendor_master vm ON t.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON t.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON t.modified_by = um2.user_id;



 DROP VIEW public.vw_att_details_building;

CREATE OR REPLACE VIEW public.vw_att_details_building
 AS
 SELECT building.system_id,
    building.network_id,
    building.building_name,
    building.building_no,
    round(building.latitude::numeric, 6) AS latitude,
    round(building.longitude::numeric, 6) AS longitude,
    building.location,
    building.area,
    building.street,
    building.address,
    building.pin_code,
    building.province_id,
    building.region_id,
    building.surveyarea_id,
    building.business_pass,
    building.building_type,
    building.home_pass,
    building.total_tower,
    building.no_of_floor,
    building.no_of_flat,
    building.no_of_occupants,
    building.building_status,
    building.network_status,
    building.status,
    building.db_flag,
    building.cluster_ref,
    building.pod_name,
    building.pod_code,
    building.rfs_status,
    building.rfs_date,
    building.customer_name,
    building.account_no,
    building.activation_date,
    building.deactivation_date,
    building.media,
    building.coverage_type_inside,
    building.requesting_customer,
    building.business_cluster,
    building.traffic_status,
    building.bldg_status_ring_spur,
    building.tenancy,
    building.category,
    building.subcategory,
    building.gis_address,
    building.rwa,
    building.rwa_contact_no,
    building.is_mobile,
    round(building.building_height::numeric, 2) AS building_height,
    building.remarks,
    building.status_updated_by,
    fn_get_date(building.status_updated_on) AS status_updated_on,
    building.created_by,
    building.created_on,
    building.modified_by,
    building.modified_on,
    prov.province_name,
    reg.region_name,
    survey.surveyarea_name,
    survey.network_id AS surveyarea_code,
    building.parent_system_id,
    building.parent_network_id,
    building.parent_entity_type,
    building.sequence_id,
    um.user_name,
    fn_get_date(building.created_on) AS created_date,
    fn_get_date(building.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    um.manager_id,
    building.is_virtual,
    building.is_visible_on_map,
    building.is_new_entity,
    advh.system_id AS vsat_system_id,
    building.origin_from,
    building.origin_ref_id,
    building.origin_ref_code,
    building.origin_ref_description,
    building.request_ref_id,
    building.requested_by,
    building.request_approved_by,
    building.subarea_id,
    building.area_id,
    building.dsa_id,
    building.csa_id,
    building.gis_design_id,
    building.source_ref_description,
    building.source_ref_type,
    building.source_ref_id,
    building.st_x,
    building.st_y,
    building.ne_id,
    building.prms_id,
    building.jc_id,
    building.mzone_id,
    building.locality,
    building.sub_locality,
    building.road,
    building.own_vendor_id
   FROM ( SELECT point_master.system_id,
            'Point'::text AS geom_type,
            point_master.sp_geometry
           FROM point_master
          WHERE point_master.entity_type::text = 'Building'::text
        UNION
         SELECT polygon_master.system_id,
            'Polygon'::text AS text,
            polygon_master.sp_geometry
           FROM polygon_master
          WHERE polygon_master.entity_type::text = 'Building'::text) plgn
     JOIN att_details_building building ON plgn.system_id = building.system_id
     JOIN province_boundary prov ON building.province_id = prov.id
     JOIN region_boundary reg ON building.region_id = reg.id
     JOIN user_master um ON building.created_by = um.user_id
     LEFT JOIN att_details_surveyarea survey ON building.surveyarea_id = survey.system_id
     LEFT JOIN user_master um2 ON building.modified_by = um2.user_id
     LEFT JOIN att_details_vsat_hub advh ON advh.parent_system_id = building.system_id;





 DROP VIEW public.vw_att_details_duct;

CREATE OR REPLACE VIEW public.vw_att_details_duct
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.duct_name,
    round(duct.calculated_length::numeric, 2) AS calculated_length,
    round(duct.manual_length::numeric, 2) AS manual_length,
    duct.a_system_id,
    duct.a_location,
    duct.a_entity_type,
    duct.b_system_id,
    duct.b_location,
    duct.b_entity_type,
    duct.sequence_id,
    duct.network_status,
    duct.status,
    duct.pin_code,
    duct.province_id,
    duct.region_id,
    duct.utilization,
    duct.no_of_cables,
    duct.offset_value,
    duct.construction,
    duct.activation,
    duct.accessibility,
    duct.specification,
    duct.category,
    round(duct.inner_dimension::numeric, 2) AS inner_dimension,
    round(duct.outer_dimension::numeric, 2) AS outer_dimension,
    duct.duct_type,
    duct.color_code,
    duct.subcategory1,
    duct.subcategory2,
    duct.subcategory3,
    duct.item_code,
    duct.vendor_id,
    duct.type,
    duct.brand,
    duct.model,
    duct.remarks,
    duct.created_by,
    duct.created_on,
    duct.modified_by,
    duct.modified_on,
    duct.trench_id,
    duct.project_id,
    duct.planning_id,
    duct.purpose_id,
    duct.workorder_id,
    prov.province_name,
    duct.parent_system_id,
    duct.parent_entity_type,
    duct.parent_network_id,
    duct.acquire_from,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    bd.brand AS duct_brand,
    ml.model AS duct_model,
    fn_get_date(duct.created_on) AS created_date,
    fn_get_date(duct.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.accessibility) AS duct_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.construction) AS duct_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.activation) AS duct_activation,
    duct.barcode,
    duct.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    duct.source_ref_type,
    duct.source_ref_id,
    duct.source_ref_description,
    duct.status_remark,
    duct.status_updated_by,
    duct.status_updated_on,
    duct.is_visible_on_map,
    duct.primary_pod_system_id,
    duct.secondary_pod_system_id,
    duct.is_new_entity,
    duct.audit_item_master_id,
    duct.is_acquire_from,
    duct.duct_capacity,
    duct.origin_from,
    duct.origin_ref_id,
    duct.origin_ref_code,
    duct.origin_ref_description,
    duct.request_ref_id,
    duct.requested_by,
    duct.request_approved_by,
    duct.subarea_id,
    duct.area_id,
    duct.dsa_id,
    duct.csa_id,
    duct.bom_sub_category,
    duct.gis_design_id,
    duct.ne_id,
    duct.prms_id,
    duct.jc_id,
    duct.mzone_id,
    duct.duct_color,
    duct.own_vendor_id,
    COALESCE(duct.total_slack_count, 0) AS total_slack_count,
    COALESCE(duct.total_slack_length, 0) AS total_slack_length
   FROM att_details_duct duct
     JOIN province_boundary prov ON duct.province_id = prov.id
     JOIN region_boundary reg ON duct.region_id = reg.id
     JOIN user_master um ON duct.created_by = um.user_id
     JOIN vendor_master vm ON duct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON duct.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON duct.type = tp.id
     LEFT JOIN isp_brand_master bd ON duct.brand = bd.id
     LEFT JOIN isp_base_model ml ON duct.model = ml.id
     LEFT JOIN user_master um2 ON duct.modified_by = um2.user_id;




 DROP VIEW public.vw_att_details_slack;

CREATE OR REPLACE VIEW public.vw_att_details_slack
 AS
 SELECT slk.system_id,
    slk.network_id,
    round(slk.slack_length::numeric, 2) AS slack_length,
    slk.associated_system_id,
    slk.associated_network_id,
    slk.associated_entity_type,
    dct.network_id AS duct_network_id,
    fn_get_date(slk.created_on, true) AS created_on,
    um.user_name AS created_by_user,
    fn_get_date(slk.modified_on, false) AS modified_on,
    um2.user_name AS modified_by_user,
    slk.status,
    slk.province_id,
    slk.region_id,
    slk.status_remark,
    slk.status_updated_by,
    slk.status_updated_on,
    slk.is_visible_on_map,
    slk.source_ref_type,
    slk.source_ref_id,
    slk.source_ref_description,
    slk.created_by,
    slk.modified_by,
    dct.system_id AS duct_id,
    slk.duct_system_id,
    slk.network_status,
    slk.parent_system_id,
    slk.parent_network_id,
    slk.parent_entity_type,
    slk.sequence_id,
    round(slk.latitude::numeric, 6) AS latitude,
    round(slk.longitude::numeric, 6) AS longitude,
    slk.is_new_entity,
    slk.project_id,
    slk.planning_id,
    slk.purpose_id,
    slk.workorder_id,
    slk.origin_from,
    slk.origin_ref_id,
    slk.origin_ref_code,
    slk.origin_ref_description,
    slk.request_ref_id,
    slk.requested_by,
    slk.request_approved_by,
    slk.subarea_id,
    slk.area_id,
    slk.dsa_id,
    slk.csa_id,
    reg.region_name,
    prov.province_name,
    slk.gis_design_id,
    slk.st_x,
    slk.st_y,
    slk.ne_id,
    slk.prms_id,
    slk.jc_id,
    slk.mzone_id,
    slk.own_vendor_id
   FROM att_details_slack slk
     LEFT JOIN att_details_duct dct ON slk.duct_system_id = dct.system_id
     JOIN province_boundary prov ON slk.province_id = prov.id
     JOIN region_boundary reg ON slk.region_id = reg.id
     JOIN user_master um ON slk.created_by = um.user_id
     LEFT JOIN user_master um2 ON slk.modified_by = um2.user_id;



 DROP VIEW public.vw_att_details_model;

CREATE OR REPLACE VIEW public.vw_att_details_model
 AS
 SELECT mtm.value AS equipment_type,
    m.network_id AS equipment_id,
    m.equipment_name,
    m.parent_entity_type AS parent_type,
    m.parent_network_id AS parent_id,
    vm.name AS vender,
    m.item_code,
    m.specification::text AS specification,
    m.system_id,
    m.rack_id,
    m.super_parent,
    m.parent_system_id,
    m.st_x,
    m.st_y,
    m.ne_id,
    m.prms_id,
    m.jc_id,
    m.mzone_id,
    m.own_vendor_id
   FROM att_details_model m
     LEFT JOIN isp_model_type_master mtm ON m.model_type_id = mtm.id
     LEFT JOIN vendor_master vm ON m.vendor_id = vm.id;






 DROP VIEW public.vw_att_details_fault;

CREATE OR REPLACE VIEW public.vw_att_details_fault
 AS
 SELECT flt.system_id,
    flt.network_id,
    flt.fault_id,
    round(flt.latitude::numeric, 6) AS latitude,
    round(flt.longitude::numeric, 6) AS longitude,
    flt.province_id,
    flt.region_id,
    flt.fault_type,
    flt.fault_entity_system_id,
    flt.fault_entity_type,
    flt.fault_entity_network_id,
    flt.fault_reason,
    flt.fault_ticket_type,
    flt.fault_ticket_id,
    flt.parent_system_id,
    flt.parent_entity_type,
    flt.parent_network_id,
    flt.remarks,
    flt.fault_status,
    flt.status,
    flt.network_status,
    flt.business_type,
    flt.created_by,
    fn_get_date(flt.created_on) AS created_date,
    um2.user_id AS modified_by,
    fn_get_date(flt.modified_on) AS modified_date,
    flt.status_remark,
    flt.status_updated_by,
    flt.status_updated_on,
    flt.is_visible_on_map,
    flt.source_ref_type,
    flt.source_ref_id,
    flt.source_ref_description,
    flt.primary_pod_system_id,
    flt.secondary_pod_system_id,
    flt.is_new_entity,
    flt.origin_from,
    flt.origin_ref_id,
    flt.origin_ref_code,
    flt.origin_ref_description,
    flt.request_ref_id,
    flt.requested_by,
    flt.request_approved_by,
    flt.subarea_id,
    flt.area_id,
    flt.dsa_id,
    flt.csa_id,
    reg.region_name,
    prov.province_name,
    flt.gis_design_id,
    flt.st_x,
    flt.st_y,
    flt.ne_id,
    flt.prms_id,
    flt.jc_id,
    flt.mzone_id,
    flt.own_vendor_id
   FROM att_details_fault flt
     JOIN province_boundary prov ON flt.province_id = prov.id
     JOIN region_boundary reg ON flt.region_id = reg.id
     JOIN user_master um ON flt.created_by = um.user_id
     LEFT JOIN user_master um2 ON flt.modified_by = um2.user_id;






 DROP VIEW public.vw_att_details_cdb;

CREATE OR REPLACE VIEW public.vw_att_details_cdb
 AS
 SELECT cdb.system_id,
    cdb.network_id,
    cdb.cdb_name,
    round(cdb.latitude::numeric, 6) AS latitude,
    round(cdb.longitude::numeric, 6) AS longitude,
    cdb.pincode,
    cdb.address,
    cdb.specification,
    cdb.category,
    cdb.subcategory1,
    cdb.subcategory2,
    cdb.subcategory3,
    cdb.item_code,
    cdb.vendor_id,
    cdb.type,
    cdb.brand,
    cdb.model,
    cdb.construction,
    cdb.activation,
    cdb.accessibility,
    cdb.status,
    cdb.network_status,
    cdb.province_id,
    cdb.region_id,
    cdb.created_by,
    cdb.created_on,
    cdb.modified_by,
    cdb.modified_on,
    cdb.project_id,
    cdb.planning_id,
    cdb.purpose_id,
    cdb.workorder_id,
    cdb.is_servingdb,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS cdb_type,
    bd.brand AS cdb_brand,
    ml.model AS cdb_model,
    cdb.parent_system_id,
    cdb.parent_network_id,
    cdb.parent_entity_type,
    cdb.sequence_id,
    cdb.acquire_from,
    fn_get_date(cdb.created_on) AS created_date,
    fn_get_date(cdb.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cdb.accessibility) AS cdb_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cdb.construction) AS cdb_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = cdb.activation) AS cdb_activation,
    cdb.no_of_input_port,
    cdb.no_of_output_port,
    cdb.entity_category,
    cdb.no_of_port,
    cdb.barcode,
    cdb.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    cdb.source_ref_type,
    cdb.source_ref_id,
    cdb.source_ref_description,
    cdb.status_remark,
    cdb.status_updated_by,
    cdb.status_updated_on,
    cdb.is_visible_on_map,
    cdb.primary_pod_system_id,
    cdb.secondary_pod_system_id,
    cdb.is_new_entity,
    cdb.remarks,
    cdb.audit_item_master_id,
    cdb.is_acquire_from,
    s.shaft_name,
    f.floor_name,
    cdb.origin_from,
    cdb.origin_ref_id,
    cdb.origin_ref_code,
    cdb.origin_ref_description,
    cdb.request_ref_id,
    cdb.requested_by,
    cdb.request_approved_by,
    cdb.subarea_id,
    cdb.area_id,
    cdb.dsa_id,
    cdb.csa_id,
    cdb.bom_sub_category,
    cdb.gis_design_id,
    cdb.st_x,
    cdb.st_y,
    cdb.ne_id,
    cdb.prms_id,
    cdb.jc_id,
    cdb.mzone_id,
    cdb.own_vendor_id
   FROM att_details_cdb cdb
     JOIN province_boundary prov ON cdb.province_id = prov.id
     JOIN region_boundary reg ON cdb.region_id = reg.id
     JOIN user_master um ON cdb.created_by = um.user_id
     JOIN vendor_master vm ON cdb.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cdb.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON cdb.type = tp.id
     LEFT JOIN isp_brand_master bd ON cdb.brand = bd.id
     LEFT JOIN isp_base_model ml ON cdb.model = ml.id
     LEFT JOIN user_master um2 ON cdb.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'CDB'::text) m1 ON cdb.system_id = m1.entity_id
     LEFT JOIN isp_shaft_info s ON m1.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m1.floor_id = f.system_id;






 DROP VIEW public.vw_att_details_adb;

CREATE OR REPLACE VIEW public.vw_att_details_adb
 AS
 SELECT adb.system_id,
    adb.network_id,
    adb.adb_name,
    round(adb.latitude::numeric, 6) AS latitude,
    round(adb.longitude::numeric, 6) AS longitude,
    adb.pincode,
    adb.address,
    adb.specification,
    adb.category,
    adb.subcategory1,
    adb.subcategory2,
    adb.subcategory3,
    adb.item_code,
    adb.vendor_id,
    adb.type,
    adb.brand,
    adb.model,
    adb.construction,
    adb.activation,
    adb.accessibility,
    adb.status,
    adb.network_status,
    adb.province_id,
    adb.region_id,
    adb.created_by,
    um.user_name AS created_by_user,
    adb.created_on,
    fn_get_date(adb.created_on) AS created_date,
    adb.modified_by,
    um2.user_name AS modified_by_user,
    adb.modified_on,
    fn_get_date(adb.modified_on) AS modified_date,
    adb.project_id,
    adb.planning_id,
    adb.purpose_id,
    adb.workorder_id,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS adb_type,
    bd.brand AS adb_brand,
    ml.model AS adb_model,
    adb.parent_system_id,
    adb.parent_network_id,
    adb.parent_entity_type,
    adb.is_servingdb,
    adb.acquire_from,
    adb.sequence_id,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = adb.accessibility) AS adb_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = adb.construction) AS adb_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = adb.activation) AS adb_activation,
    adb.no_of_input_port,
    adb.no_of_output_port,
    adb.entity_category,
    adb.no_of_port,
    adb.barcode,
    adb.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    adb.source_ref_type,
    adb.source_ref_id,
    adb.source_ref_description,
    adb.status_remark,
    adb.status_updated_by,
    adb.status_updated_on,
    adb.is_visible_on_map,
    adb.primary_pod_system_id,
    adb.secondary_pod_system_id,
    adb.is_new_entity,
    adb.remarks,
    adb.audit_item_master_id,
    adb.is_acquire_from,
    s.shaft_name,
    f.floor_name,
    adb.origin_from,
    adb.origin_ref_id,
    adb.origin_ref_code,
    adb.origin_ref_description,
    adb.request_ref_id,
    adb.requested_by,
    adb.request_approved_by,
    adb.subarea_id,
    adb.area_id,
    adb.dsa_id,
    adb.csa_id,
    adb.bom_sub_category,
    adb.gis_design_id,
    adb.st_x,
    adb.st_y,
    adb.ne_id,
    adb.prms_id,
    adb.jc_id,
    adb.mzone_id,
    adb.own_vendor_id
   FROM att_details_adb adb
     JOIN province_boundary prov ON adb.province_id = prov.id
     JOIN region_boundary reg ON adb.region_id = reg.id
     JOIN user_master um ON adb.created_by = um.user_id
     JOIN vendor_master vm ON adb.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON adb.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON adb.type = tp.id
     LEFT JOIN isp_brand_master bd ON adb.brand = bd.id
     LEFT JOIN isp_base_model ml ON adb.model = ml.id
     LEFT JOIN user_master um2 ON adb.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'ADB'::text) m1 ON adb.system_id = m1.entity_id
     LEFT JOIN isp_shaft_info s ON m1.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m1.floor_id = f.system_id
  ORDER BY adb.created_on;










 DROP VIEW public.vw_isp_htb_info;

CREATE OR REPLACE VIEW public.vw_isp_htb_info
 AS
 SELECT isp_htb_info.system_id,
    isp_htb_info.network_id,
    isp_htb_info.structure_id,
    isp_htb_info.floor_id,
    isp_htb_info.shaft_id,
    isp_htb_info.htb_name,
    isp_htb_info.htb_type,
    isp_htb_info.status,
    isp_htb_info.created_by,
    isp_htb_info.created_on,
    isp_htb_info.modified_by,
    isp_htb_info.modified_on,
    round(isp_htb_info.latitude::numeric, 6) AS latitude,
    round(isp_htb_info.longitude::numeric, 6) AS longitude,
    isp_htb_info.province_id,
    isp_htb_info.region_id,
    isp_htb_info.building_id,
    isp_htb_info.specification,
    isp_htb_info.category,
    isp_htb_info.subcategory1,
    isp_htb_info.subcategory2,
    isp_htb_info.subcategory3,
    isp_htb_info.item_code,
    isp_htb_info.vendor_id,
    isp_htb_info.type,
    isp_htb_info.brand,
    isp_htb_info.model,
    isp_htb_info.construction,
    isp_htb_info.activation,
    isp_htb_info.accessibility,
    isp_htb_info.network_status,
    isp_htb_info.project_id,
    isp_htb_info.planning_id,
    isp_htb_info.purpose_id,
    isp_htb_info.workorder_id,
    isp_htb_info.acquire_from,
    isp_htb_info.no_of_port,
    ''::character varying AS additional_attributes,
        CASE
            WHEN isp_htb_info.no_of_input_port <> 0 AND isp_htb_info.no_of_output_port <> 0 THEN ((isp_htb_info.no_of_input_port::character varying::text || ''::text) || isp_htb_info.no_of_output_port::character varying::text)::character varying
            ELSE isp_htb_info.no_of_port::character varying
        END AS total_ports,
    ''::character varying AS entity_category,
    isp_htb_info.htb_name AS entity_name,
    isp_htb_info.barcode,
    um.user_name AS created_by_user,
    vm.name AS vendor_name,
    isp_htb_info.parent_system_id,
    isp_htb_info.parent_network_id,
    isp_htb_info.parent_entity_type,
    isp_htb_info.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    isp_htb_info.source_ref_type,
    isp_htb_info.source_ref_id,
    isp_htb_info.source_ref_description,
    isp_htb_info.status_remark,
    isp_htb_info.status_updated_by,
    isp_htb_info.status_updated_on,
    isp_htb_info.is_visible_on_map,
    isp_htb_info.primary_pod_system_id,
    isp_htb_info.secondary_pod_system_id,
    isp_htb_info.is_new_entity,
    isp_htb_info.remarks,
    isp_htb_info.audit_item_master_id,
    isp_htb_info.is_middleware,
    isp_htb_info.is_acquire_from,
    isp_room_info.network_id AS unit_network_id,
    s.shaft_name,
    f.floor_name,
    isp_htb_info.origin_from,
    isp_htb_info.origin_ref_id,
    isp_htb_info.origin_ref_code,
    isp_htb_info.origin_ref_description,
    isp_htb_info.request_ref_id,
    isp_htb_info.requested_by,
    isp_htb_info.request_approved_by,
    isp_htb_info.subarea_id,
    isp_htb_info.area_id,
    isp_htb_info.dsa_id,
    isp_htb_info.csa_id,
    prov.province_name,
    reg.region_name,
    isp_htb_info.bom_sub_category,
    isp_htb_info.gis_design_id,
    isp_htb_info.st_x,
    isp_htb_info.st_y,
    isp_htb_info.ne_id,
    isp_htb_info.prms_id,
    isp_htb_info.jc_id,
    isp_htb_info.mzone_id,
    isp_htb_info.own_vendor_id
   FROM isp_htb_info
     JOIN province_boundary prov ON isp_htb_info.province_id = prov.id
     JOIN region_boundary reg ON isp_htb_info.region_id = reg.id
     JOIN user_master um ON um.user_id = isp_htb_info.created_by
     JOIN vendor_master vm ON isp_htb_info.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON isp_htb_info.third_party_vendor_id = vm2.id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'HTB'::text) m1 ON isp_htb_info.system_id = m1.entity_id
     LEFT JOIN ( SELECT isp_entity_mapping.id,
            isp_entity_mapping.structure_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id,
            isp_entity_mapping.entity_type,
            isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'UNIT'::text) m2 ON m1.parent_id = m2.id
     LEFT JOIN isp_room_info ON m2.entity_id = isp_room_info.system_id AND m2.floor_id = isp_room_info.floor_id
     LEFT JOIN isp_shaft_info s ON m2.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m2.floor_id = f.system_id;
	 
	 
	 
	  DROP VIEW public.vw_att_details_spliceclosure;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure
 AS
 SELECT sc.system_id,
    sc.network_id,
    sc.spliceclosure_name,
    round(sc.latitude::numeric, 6) AS latitude,
    round(sc.longitude::numeric, 6) AS longitude,
    sc.address,
    sc.specification,
    sc.category,
    sc.subcategory1,
    sc.subcategory2,
    sc.subcategory3,
    sc.item_code,
    sc.vendor_id,
    sc.type,
    sc.brand,
    sc.model,
    sc.construction,
    sc.activation,
    sc.accessibility,
    sc.status,
    sc.network_status,
    sc.province_id,
    sc.region_id,
    sc.created_by,
    sc.created_on,
    sc.modified_by,
    sc.modified_on,
    sc.pincode,
    prov.province_name,
    reg.region_name,
    sc.parent_system_id,
    sc.parent_network_id,
    sc.parent_entity_type,
    sc.sequence_id,
    sc.project_id,
    sc.planning_id,
    sc.purpose_id,
    sc.workorder_id,
    sc.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS sc_type,
    bd.brand AS sc_brand,
    ml.model AS sc_model,
    fn_get_date(sc.created_on) AS created_date,
    fn_get_date(sc.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    sc.is_virtual,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.accessibility) AS sc_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.construction) AS sc_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = sc.activation) AS sc_activation,
    sc.no_of_ports,
    sc.no_of_input_port,
    sc.no_of_output_port,
    ''::character varying AS additional_attributes,
        CASE
            WHEN sc.no_of_input_port <> 0 AND sc.no_of_output_port <> 0 THEN ((sc.no_of_input_port::character varying::text || ''::text) || sc.no_of_output_port::character varying::text)::character varying
            ELSE sc.no_of_ports::character varying
        END AS total_ports,
    ''::character varying AS entity_category,
    sc.spliceclosure_name AS entity_name,
    sc.barcode,
    sc.is_buried,
    sc.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    sc.source_ref_type,
    sc.source_ref_id,
    sc.source_ref_description,
    sc.status_remark,
    sc.status_updated_by,
    sc.status_updated_on,
    sc.is_visible_on_map,
    sc.primary_pod_system_id,
    sc.secondary_pod_system_id,
    sc.is_new_entity,
    sc.remarks,
    sc.audit_item_master_id,
    sc.is_acquire_from,
    s.shaft_name,
    f.floor_name,
    sc.origin_from,
    sc.origin_ref_id,
    sc.origin_ref_code,
    sc.origin_ref_description,
    sc.request_ref_id,
    sc.requested_by,
    sc.request_approved_by,
    sc.subarea_id,
    sc.area_id,
    sc.dsa_id,
    sc.csa_id,
    sc.bom_sub_category,
    sc.gis_design_id,
    sc.st_x,
    sc.st_y,
    sc.ne_id,
    sc.prms_id,
    sc.jc_id,
    sc.mzone_id,
    sc.hierarchy_type,
    sc.section_name,
    sc.aerial_location,
    sc.generic_section_name,
    sc.own_vendor_id
   FROM att_details_spliceclosure sc
     JOIN province_boundary prov ON sc.province_id = prov.id
     JOIN region_boundary reg ON sc.region_id = reg.id
     JOIN user_master um ON sc.created_by = um.user_id
     JOIN vendor_master vm ON sc.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sc.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON sc.type = tp.id
     LEFT JOIN isp_brand_master bd ON sc.brand = bd.id
     LEFT JOIN isp_base_model ml ON sc.model = ml.id
     LEFT JOIN user_master um2 ON sc.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'SPLICECLOSURE'::text) m1 ON sc.system_id = m1.entity_id
     LEFT JOIN isp_shaft_info s ON m1.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m1.floor_id = f.system_id;







 DROP VIEW public.vw_att_details_mpod;

CREATE OR REPLACE VIEW public.vw_att_details_mpod
 AS
 SELECT mpod.system_id,
    mpod.network_id,
    mpod.mpod_name,
    round(mpod.latitude::numeric, 6) AS latitude,
    round(mpod.longitude::numeric, 6) AS longitude,
    mpod.pincode,
    mpod.address,
    mpod.specification,
    mpod.category,
    mpod.subcategory1,
    mpod.subcategory2,
    mpod.subcategory3,
    mpod.item_code,
    mpod.vendor_id,
    mpod.type,
    mpod.brand,
    mpod.model,
    mpod.construction,
    mpod.activation,
    mpod.accessibility,
    mpod.status,
    mpod.network_status,
    mpod.province_id,
    mpod.region_id,
    mpod.created_by,
    mpod.created_on,
    mpod.modified_by,
    mpod.modified_on,
    prov.province_name,
    mpod.parent_system_id,
    mpod.parent_network_id,
    mpod.parent_entity_type,
    mpod.sequence_id,
    mpod.project_id,
    mpod.planning_id,
    mpod.purpose_id,
    mpod.workorder_id,
    mpod.acquire_from,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS mpod_types,
    bd.brand AS mpod_brand,
    ml.model AS mpod_model,
    fn_get_date(mpod.created_on) AS created_date,
    fn_get_date(mpod.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = mpod.accessibility) AS mpod_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = mpod.construction) AS mpod_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = mpod.activation) AS mpod_activation,
    ''::character varying AS additional_attributes,
    ''::text AS total_ports,
    ''::text AS entity_category,
    mpod.mpod_name AS entity_name,
    mpod.mpod_type,
    mpod.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    mpod.source_ref_type,
    mpod.source_ref_id,
    mpod.source_ref_description,
    mpod.status_remark,
    mpod.status_updated_by,
    mpod.status_updated_on,
    mpod.is_visible_on_map,
    mpod.primary_pod_system_id,
    mpod.secondary_pod_system_id,
    mpod.is_new_entity,
    mpod.remarks,
    mpod.audit_item_master_id,
    mpod.is_acquire_from,
    isp_room_info.network_id AS unit_network_id,
    s.shaft_name,
    f.floor_name,
    mpod.origin_from,
    mpod.origin_ref_id,
    mpod.origin_ref_code,
    mpod.origin_ref_description,
    mpod.request_ref_id,
    mpod.requested_by,
    mpod.request_approved_by,
    mpod.subarea_id,
    mpod.area_id,
    mpod.dsa_id,
    mpod.csa_id,
    mpod.bom_sub_category,
    mpod.gis_design_id,
    mpod.st_x,
    mpod.st_y,
    mpod.ne_id,
    mpod.prms_id,
    mpod.jc_id,
    mpod.mzone_id,
    mpod.own_vendor_id
   FROM att_details_mpod mpod
     JOIN province_boundary prov ON mpod.province_id = prov.id
     JOIN region_boundary reg ON mpod.region_id = reg.id
     JOIN user_master um ON mpod.created_by = um.user_id
     JOIN vendor_master vm ON mpod.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON mpod.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON mpod.type = tp.id
     LEFT JOIN isp_brand_master bd ON mpod.brand = bd.id
     LEFT JOIN isp_base_model ml ON mpod.model = ml.id
     LEFT JOIN user_master um2 ON mpod.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'HTB'::text) m1 ON mpod.system_id = m1.entity_id
     LEFT JOIN ( SELECT isp_entity_mapping.id,
            isp_entity_mapping.structure_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id,
            isp_entity_mapping.entity_type,
            isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'UNIT'::text) m2 ON m1.parent_id = m2.id
     LEFT JOIN isp_room_info ON m2.entity_id = isp_room_info.system_id AND m2.floor_id = isp_room_info.floor_id
     LEFT JOIN isp_shaft_info s ON m2.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m2.floor_id = f.system_id;



 DROP VIEW public.vw_att_details_fms;

CREATE OR REPLACE VIEW public.vw_att_details_fms
 AS
 SELECT fms.system_id,
    fms.network_id,
    fms.fms_name,
    round(fms.latitude::numeric, 6) AS latitude,
    round(fms.longitude::numeric, 6) AS longitude,
    fms.address,
    fms.specification,
    fms.category,
    fms.subcategory1,
    fms.subcategory2,
    fms.subcategory3,
    fms.item_code,
    fms.vendor_id,
    fms.type,
    fms.brand,
    fms.model,
    fms.construction,
    fms.activation,
    fms.accessibility,
    fms.status,
    fms.network_status,
    fms.province_id,
    fms.region_id,
    fms.created_by,
    fms.created_on,
    fms.modified_by,
    fms.modified_on,
    fms.pincode,
    prov.province_name,
    reg.region_name,
    fms.parent_system_id,
    fms.parent_network_id,
    fms.parent_entity_type,
    fms.sequence_id,
    fms.project_id,
    fms.planning_id,
    fms.purpose_id,
    fms.workorder_id,
    fms.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    fn_get_date(fms.created_on) AS created_date,
    fn_get_date(fms.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    fms.no_of_port,
    fms.no_of_input_port,
    fms.no_of_output_port,
    ''::character varying AS additional_attributes,
    (fms.no_of_input_port::character varying::text || ''::text) || fms.no_of_output_port::character varying::text AS total_ports,
    ''::text AS entity_category,
    fms.fms_name AS entity_name,
    fms.barcode,
    fms.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    fms.source_ref_type,
    fms.source_ref_id,
    fms.source_ref_description,
    fms.status_remark,
    fms.status_updated_by,
    fms.status_updated_on,
    fms.is_visible_on_map,
    fms.primary_pod_system_id,
    fms.secondary_pod_system_id,
    fms.is_new_entity,
    fms.remarks,
    fms.audit_item_master_id,
    fms.is_acquire_from,
    fms.origin_from,
    fms.origin_ref_id,
    fms.origin_ref_code,
    fms.origin_ref_description,
    fms.request_ref_id,
    fms.requested_by,
    fms.request_approved_by,
    fms.subarea_id,
    fms.area_id,
    fms.dsa_id,
    fms.csa_id,
    fms.bom_sub_category,
    fms.gis_design_id,
    fms.st_x,
    fms.st_y,
    fms.ne_id,
    fms.prms_id,
    fms.jc_id,
    fms.mzone_id,
    fms.own_vendor_id
   FROM att_details_fms fms
     JOIN province_boundary prov ON fms.province_id = prov.id
     JOIN region_boundary reg ON fms.region_id = reg.id
     JOIN user_master um ON fms.created_by = um.user_id
     JOIN vendor_master vm ON fms.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON fms.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON fms.modified_by = um2.user_id;





 DROP VIEW public.vw_att_details_trench;

CREATE OR REPLACE VIEW public.vw_att_details_trench
 AS
 SELECT trench.system_id,
    trench.network_id,
    trench.trench_name,
    trench.a_system_id,
    trench.a_location,
    trench.a_entity_type,
    trench.b_system_id,
    trench.b_location,
    trench.b_entity_type,
    round(trench.trench_length::numeric, 2) AS trench_length,
    round(trench.trench_width::numeric, 2) AS trench_width,
    round(trench.trench_height::numeric, 2) AS trench_height,
    trench.customer_count,
    trench.utilization,
    trench.trench_type,
    trench.no_of_ducts,
    trench.sequence_id,
    trench.network_status,
    trench.status,
    trench.pin_code,
    trench.province_id,
    trench.region_id,
    trench.construction,
    trench.activation,
    trench.accessibility,
    trench.specification,
    trench.category,
    trench.subcategory1,
    trench.subcategory2,
    trench.subcategory3,
    trench.item_code,
    trench.vendor_id,
    trench.type,
    trench.brand,
    trench.model,
    trench.remarks,
    trench.created_by,
    trench.created_on,
    trench.modified_by,
    trench.modified_on,
    trench.project_id,
    trench.planning_id,
    trench.purpose_id,
    trench.workorder_id,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    trench.mcgm_ward,
    trench.strata_type,
    trench.manufacture_year,
    trench.surface_type,
    trench.parent_system_id,
    trench.parent_entity_type,
    trench.parent_network_id,
    trench.acquire_from,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS trench_specification_type,
    bd.brand AS trench_brand,
    ml.model AS trench_model,
    fn_get_date(trench.created_on) AS created_date,
    fn_get_date(trench.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = trench.accessibility) AS trench_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = trench.construction) AS trench_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = trench.activation) AS trench_activation,
    trench.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    trench.source_ref_type,
    trench.source_ref_id,
    trench.source_ref_description,
    trench.status_remark,
    trench.status_updated_by,
    trench.status_updated_on,
    trench.is_visible_on_map,
    trench.primary_pod_system_id,
    trench.secondary_pod_system_id,
    trench.is_new_entity,
    trench.audit_item_master_id,
    trench.is_acquire_from,
    trench.origin_from,
    trench.origin_ref_id,
    trench.origin_ref_code,
    trench.origin_ref_description,
    trench.request_ref_id,
    trench.requested_by,
    trench.request_approved_by,
    trench.subarea_id,
    trench.area_id,
    trench.dsa_id,
    trench.csa_id,
    trench.bom_sub_category,
    trench.trench_serving_type,
    trench.gis_design_id,
    trench.ne_id,
    trench.prms_id,
    trench.jc_id,
    trench.mzone_id,
    trench.actual_no_of_ducts,
    trench.own_vendor_id
   FROM att_details_trench trench
     JOIN province_boundary prov ON trench.province_id = prov.id
     JOIN region_boundary reg ON trench.region_id = reg.id
     JOIN user_master um ON trench.created_by = um.user_id
     JOIN vendor_master vm ON trench.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON trench.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON trench.type = tp.id
     LEFT JOIN isp_brand_master bd ON trench.brand = bd.id
     LEFT JOIN isp_base_model ml ON trench.model = ml.id
     LEFT JOIN user_master um2 ON trench.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON trench.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON trench.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON trench.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON trench.purpose_id = purposem.system_id;





 DROP VIEW public.vw_isp_fdb_info;

CREATE OR REPLACE VIEW public.vw_isp_fdb_info
 AS
 SELECT isp_fdb_info.system_id,
    isp_fdb_info.network_id,
    isp_fdb_info.structure_id,
    isp_fdb_info.floor_id,
    isp_fdb_info.shaft_id,
    isp_fdb_info.fdb_name,
    isp_fdb_info.status,
    isp_fdb_info.created_by,
    isp_fdb_info.created_on,
    isp_fdb_info.modified_by,
    isp_fdb_info.modified_on,
    round(isp_fdb_info.latitude::numeric, 6) AS latitude,
    round(isp_fdb_info.longitude::numeric, 6) AS longitude,
    isp_fdb_info.province_id,
    isp_fdb_info.region_id,
    isp_fdb_info.building_id,
    isp_fdb_info.specification,
    isp_fdb_info.category,
    isp_fdb_info.subcategory1,
    isp_fdb_info.subcategory2,
    isp_fdb_info.subcategory3,
    isp_fdb_info.item_code,
    isp_fdb_info.vendor_id,
    isp_fdb_info.type,
    isp_fdb_info.brand,
    isp_fdb_info.model,
    isp_fdb_info.construction,
    isp_fdb_info.activation,
    isp_fdb_info.accessibility,
    isp_fdb_info.network_status,
    isp_fdb_info.project_id,
    isp_fdb_info.planning_id,
    isp_fdb_info.purpose_id,
    isp_fdb_info.workorder_id,
    isp_fdb_info.parent_system_id,
    isp_fdb_info.parent_entity_type,
    isp_fdb_info.parent_network_id,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS fdb_type,
    bd.brand AS fdb_brand,
    ml.model AS fdb_model,
    prov.province_name,
    reg.region_name,
    fn_get_date(isp_fdb_info.created_on) AS created_date,
    fn_get_date(isp_fdb_info.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = isp_fdb_info.accessibility) AS fdb_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = isp_fdb_info.construction) AS fdb_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = isp_fdb_info.activation) AS fdb_activation,
    ''::character varying AS additional_attributes,
        CASE
            WHEN isp_fdb_info.no_of_input_port <> 0 AND isp_fdb_info.no_of_output_port <> 0 THEN ((isp_fdb_info.no_of_input_port::character varying::text || ''::text) || isp_fdb_info.no_of_output_port::character varying::text)::character varying
            ELSE isp_fdb_info.no_of_port::character varying
        END AS total_ports,
    isp_fdb_info.no_of_port::character varying AS no_of_port,
    ''::character varying AS entity_category,
    isp_fdb_info.fdb_name AS entity_name,
    isp_fdb_info.barcode,
    isp_fdb_info.acquire_from,
    isp_fdb_info.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    isp_fdb_info.source_ref_type,
    isp_fdb_info.source_ref_id,
    isp_fdb_info.source_ref_description,
    isp_fdb_info.status_remark,
    isp_fdb_info.status_updated_by,
    isp_fdb_info.status_updated_on,
    isp_fdb_info.is_visible_on_map,
    isp_fdb_info.primary_pod_system_id,
    isp_fdb_info.secondary_pod_system_id,
    isp_fdb_info.is_new_entity,
    isp_fdb_info.remarks,
    isp_fdb_info.audit_item_master_id,
    isp_fdb_info.is_acquire_from,
    s.shaft_name,
    f.floor_name,
    isp_fdb_info.origin_from,
    isp_fdb_info.origin_ref_id,
    isp_fdb_info.origin_ref_code,
    isp_fdb_info.origin_ref_description,
    isp_fdb_info.request_ref_id,
    isp_fdb_info.requested_by,
    isp_fdb_info.request_approved_by,
    isp_fdb_info.subarea_id,
    isp_fdb_info.area_id,
    isp_fdb_info.dsa_id,
    isp_fdb_info.csa_id,
    isp_fdb_info.bom_sub_category,
    isp_fdb_info.gis_design_id,
    isp_fdb_info.st_x,
    isp_fdb_info.st_y,
    isp_fdb_info.ne_id,
    isp_fdb_info.prms_id,
    isp_fdb_info.jc_id,
    isp_fdb_info.mzone_id,
    isp_fdb_info.own_vendor_id
   FROM isp_fdb_info
     JOIN province_boundary prov ON isp_fdb_info.province_id = prov.id
     JOIN region_boundary reg ON isp_fdb_info.region_id = reg.id
     JOIN user_master um ON isp_fdb_info.created_by = um.user_id
     JOIN vendor_master vm ON isp_fdb_info.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON isp_fdb_info.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON isp_fdb_info.type = tp.id
     LEFT JOIN isp_brand_master bd ON isp_fdb_info.brand = bd.id
     LEFT JOIN isp_base_model ml ON isp_fdb_info.model = ml.id
     LEFT JOIN user_master um2 ON isp_fdb_info.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'FDB'::text) m1 ON isp_fdb_info.system_id = m1.entity_id
     LEFT JOIN isp_shaft_info s ON m1.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m1.floor_id = f.system_id;





 DROP VIEW public.vw_att_details_tree;

CREATE OR REPLACE VIEW public.vw_att_details_tree
 AS
 SELECT tree.system_id,
    tree.network_id,
    tree.tree_name,
    tree.tree_height,
    tree.tree_no,
    round(tree.latitude::numeric, 6) AS latitude,
    round(tree.longitude::numeric, 6) AS longitude,
    tree.address,
    tree.specification,
    tree.category,
    tree.subcategory1,
    tree.subcategory2,
    tree.subcategory3,
    tree.item_code,
    tree.vendor_id,
    tree.type,
    tree.brand,
    tree.model,
    tree.construction,
    tree.activation,
    tree.accessibility,
    tree.status,
    tree.network_status,
    tree.province_id,
    tree.region_id,
    tree.created_by,
    tree.created_on,
    tree.modified_by,
    tree.modified_on,
    tree.project_id,
    tree.planning_id,
    tree.purpose_id,
    tree.workorder_id,
    prov.province_name,
    reg.region_name,
    tree.parent_system_id,
    tree.parent_network_id,
    tree.parent_entity_type,
    tree.sequence_id,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS tree_type,
    bd.brand AS tree_brand,
    ml.model AS tree_model,
    fn_get_date(tree.created_on) AS created_date,
    fn_get_date(tree.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = tree.accessibility) AS tree_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = tree.construction) AS tree_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = tree.activation) AS tree_activation,
    tree.source_ref_type,
    tree.source_ref_id,
    tree.source_ref_description,
    tree.status_remark,
    tree.status_updated_by,
    tree.status_updated_on,
    tree.is_visible_on_map,
    tree.primary_pod_system_id,
    tree.secondary_pod_system_id,
    tree.is_new_entity,
    tree.remarks,
    tree.audit_item_master_id,
    tree.origin_from,
    tree.origin_ref_id,
    tree.origin_ref_code,
    tree.origin_ref_description,
    tree.request_ref_id,
    tree.requested_by,
    tree.request_approved_by,
    tree.subarea_id,
    tree.area_id,
    tree.dsa_id,
    tree.csa_id,
    tree.gis_design_id,
    tree.st_x,
    tree.st_y,
    tree.ne_id,
    tree.prms_id,
    tree.jc_id,
    tree.mzone_id,
    tree.ownership_type,
    tree.own_vendor_id
   FROM att_details_tree tree
     JOIN province_boundary prov ON tree.province_id = prov.id
     JOIN region_boundary reg ON tree.region_id = reg.id
     JOIN user_master um ON tree.created_by = um.user_id
     JOIN vendor_master vm ON tree.vendor_id = vm.id
     LEFT JOIN isp_type_master tp ON tree.type = tp.id
     LEFT JOIN isp_brand_master bd ON tree.brand = bd.id
     LEFT JOIN isp_base_model ml ON tree.model = ml.id
     LEFT JOIN user_master um2 ON tree.modified_by = um2.user_id;






 DROP VIEW public.vw_att_details_pod;

CREATE OR REPLACE VIEW public.vw_att_details_pod
 AS
 SELECT pod.system_id,
    pod.network_id,
    pod.pod_name,
    round(pod.latitude::numeric, 6) AS latitude,
    round(pod.longitude::numeric, 6) AS longitude,
    pod.pincode,
    pod.address,
    pod.specification,
    pod.category,
    pod.subcategory1,
    pod.subcategory2,
    pod.subcategory3,
    pod.item_code,
    pod.vendor_id,
    pod.type,
    pod.brand,
    pod.model,
    pod.construction,
    pod.activation,
    pod.accessibility,
    pod.status,
    pod.network_status,
    pod.province_id,
    pod.region_id,
    pod.created_by,
    pod.created_on,
    pod.modified_by,
    pod.modified_on,
    pod.parent_system_id,
    pod.parent_network_id,
    pod.parent_entity_type,
    pod.sequence_id,
    pod.structure_id,
    pod.shaft_id,
    pod.floor_id,
    pod.project_id,
    pod.planning_id,
    pod.purpose_id,
    pod.workorder_id,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS pod_types,
    bd.brand AS pod_brand,
    ml.model AS pod_model,
    fn_get_date(pod.created_on) AS created_date,
    fn_get_date(pod.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.accessibility) AS pod_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.construction) AS pod_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.activation) AS pod_activation,
    ''::character varying AS additional_attributes,
    ''::text AS total_ports,
    ''::text AS entity_category,
    pod.pod_name AS entity_name,
    pod.acquire_from,
    pod.pod_type,
    pod.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    pod.source_ref_type,
    pod.source_ref_id,
    pod.source_ref_description,
    pod.status_remark,
    pod.status_updated_by,
    pod.status_updated_on,
    pod.is_visible_on_map,
    pod.is_new_entity,
    pod.remarks,
    pod.audit_item_master_id,
    pod.is_acquire_from,
    isp_room_info.network_id AS unit_network_id,
    s.shaft_name,
    f.floor_name,
    pod.origin_from,
    pod.origin_ref_id,
    pod.origin_ref_code,
    pod.origin_ref_description,
    pod.request_ref_id,
    pod.requested_by,
    pod.request_approved_by,
    pod.subarea_id,
    pod.area_id,
    pod.dsa_id,
    pod.csa_id,
    pod.bom_sub_category,
    pod.gis_design_id,
    pod.st_x,
    pod.st_y,
    pod.ne_id,
    pod.prms_id,
    pod.jc_id,
    pod.mzone_id,
    pod.own_vendor_id
   FROM att_details_pod pod
     JOIN province_boundary prov ON pod.province_id = prov.id
     JOIN region_boundary reg ON pod.region_id = reg.id
     JOIN user_master um ON pod.created_by = um.user_id
     JOIN vendor_master vm ON pod.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pod.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON pod.type = tp.id
     LEFT JOIN isp_brand_master bd ON pod.brand = bd.id
     LEFT JOIN isp_base_model ml ON pod.model = ml.id
     LEFT JOIN user_master um2 ON pod.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'POD'::text) m1 ON pod.system_id = m1.entity_id
     LEFT JOIN ( SELECT isp_entity_mapping.id,
            isp_entity_mapping.structure_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id,
            isp_entity_mapping.entity_type,
            isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'UNIT'::text) m2 ON m1.parent_id = m2.id
     LEFT JOIN isp_room_info ON m2.entity_id = isp_room_info.system_id AND m2.floor_id = isp_room_info.floor_id
     LEFT JOIN isp_shaft_info s ON m2.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m2.floor_id = f.system_id;





 DROP VIEW public.vw_att_details_ont;

CREATE OR REPLACE VIEW public.vw_att_details_ont
 AS
 SELECT ont.system_id,
    ont.network_id,
    ont.ont_name,
    round(ont.latitude::numeric, 6) AS latitude,
    round(ont.longitude::numeric, 6) AS longitude,
    ont.serial_no,
    ont.specification,
    ont.category,
    ont.subcategory1,
    ont.subcategory2,
    ont.subcategory3,
    ont.item_code,
    ont.vendor_id,
    ont.type,
    ont.brand,
    ont.model,
    ont.construction,
    ont.activation,
    ont.accessibility,
    ont.status,
    ont.network_status,
    ont.province_id,
    ont.region_id,
    ont.created_by,
    ont.created_on,
    ont.modified_by,
    ont.modified_on,
    ont.project_id,
    ont.planning_id,
    ont.purpose_id,
    ont.workorder_id,
    ont.acquire_from,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS ont_type,
    bd.brand AS ont_brand,
    ml.model AS ont_model,
    fn_get_date(ont.created_on) AS created_date,
    fn_get_date(ont.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = ont.accessibility) AS ont_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = ont.construction) AS ont_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = ont.activation) AS ont_activation,
    ont.parent_network_id,
    ont.no_of_input_port,
    ont.no_of_output_port,
    ont.parent_system_id,
    ont.parent_entity_type,
    ''::character varying AS additional_attributes,
    (ont.no_of_input_port::character varying::text || ''::text) || ont.no_of_output_port::character varying::text AS total_ports,
    ''::character varying AS entity_category,
    ont.ont_name AS entity_name,
    ont.barcode,
    ont.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    ont.source_ref_type,
    ont.source_ref_id,
    ont.source_ref_description,
    ont.status_remark,
    ont.status_updated_by,
    ont.status_updated_on,
    ont.is_visible_on_map,
    ont.primary_pod_system_id,
    ont.secondary_pod_system_id,
    ont.is_new_entity,
    ont.remarks,
    ont.audit_item_master_id,
    ont.is_acquire_from,
    ont.cpe_type,
    isp_room_info.network_id AS unit_network_id,
    s.shaft_name,
    f.floor_name,
    ont.origin_from,
    ont.origin_ref_id,
    ont.origin_ref_code,
    ont.origin_ref_description,
    ont.request_ref_id,
    ont.requested_by,
    ont.request_approved_by,
    ont.subarea_id,
    ont.area_id,
    ont.dsa_id,
    ont.csa_id,
    ont.bom_sub_category,
    ont.gis_design_id,
    ont.st_x,
    ont.st_y,
    ont.ne_id,
    ont.prms_id,
    ont.jc_id,
    ont.mzone_id,
    ont.own_vendor_id
   FROM att_details_ont ont
     JOIN province_boundary prov ON ont.province_id = prov.id
     JOIN region_boundary reg ON ont.region_id = reg.id
     JOIN user_master um ON ont.created_by = um.user_id
     JOIN vendor_master vm ON ont.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON ont.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON ont.type = tp.id
     LEFT JOIN isp_brand_master bd ON ont.brand = bd.id
     LEFT JOIN isp_base_model ml ON ont.model = ml.id
     LEFT JOIN user_master um2 ON ont.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'HTB'::text) m1 ON ont.system_id = m1.entity_id
     LEFT JOIN ( SELECT isp_entity_mapping.id,
            isp_entity_mapping.structure_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id,
            isp_entity_mapping.entity_type,
            isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'UNIT'::text) m2 ON m1.parent_id = m2.id
     LEFT JOIN isp_room_info ON m2.entity_id = isp_room_info.system_id AND m2.floor_id = isp_room_info.floor_id
     LEFT JOIN isp_shaft_info s ON m2.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m2.floor_id = f.system_id;






 DROP VIEW public.vw_att_details_wallmount;

CREATE OR REPLACE VIEW public.vw_att_details_wallmount
 AS
 SELECT wallmount.system_id,
    wallmount.network_id,
    wallmount.wallmount_name,
    wallmount.wallmount_height,
    wallmount.wallmount_no,
    round(wallmount.latitude::numeric, 6) AS latitude,
    round(wallmount.longitude::numeric, 6) AS longitude,
    wallmount.address,
    wallmount.specification,
    wallmount.category,
    wallmount.subcategory1,
    wallmount.subcategory2,
    wallmount.subcategory3,
    wallmount.item_code,
    wallmount.vendor_id,
    wallmount.type,
    wallmount.brand,
    wallmount.model,
    wallmount.construction,
    wallmount.activation,
    wallmount.accessibility,
    wallmount.status,
    wallmount.network_status,
    wallmount.province_id,
    wallmount.region_id,
    wallmount.created_by,
    wallmount.created_on,
    wallmount.modified_by,
    wallmount.modified_on,
    prov.province_name,
    reg.region_name,
    wallmount.parent_system_id,
    wallmount.parent_network_id,
    wallmount.parent_entity_type,
    wallmount.sequence_id,
    wallmount.project_id,
    wallmount.planning_id,
    wallmount.purpose_id,
    wallmount.workorder_id,
    wallmount.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS wallmount_type,
    bd.brand AS wallmount_brand,
    ml.model AS wallmount_model,
    fn_get_date(wallmount.created_on) AS created_date,
    fn_get_date(wallmount.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = wallmount.accessibility) AS wallmount_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = wallmount.construction) AS wallmount_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = wallmount.activation) AS wallmount_activation,
    wallmount.barcode,
    wallmount.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    wallmount.source_ref_type,
    wallmount.source_ref_id,
    wallmount.source_ref_description,
    wallmount.status_remark,
    wallmount.status_updated_by,
    wallmount.status_updated_on,
    wallmount.is_visible_on_map,
    wallmount.primary_pod_system_id,
    wallmount.secondary_pod_system_id,
    wallmount.is_new_entity,
    wallmount.remarks,
    wallmount.audit_item_master_id,
    wallmount.is_acquire_from,
    wallmount.origin_from,
    wallmount.origin_ref_id,
    wallmount.origin_ref_code,
    wallmount.origin_ref_description,
    wallmount.request_ref_id,
    wallmount.requested_by,
    wallmount.request_approved_by,
    wallmount.subarea_id,
    wallmount.area_id,
    wallmount.dsa_id,
    wallmount.csa_id,
    wallmount.bom_sub_category,
    wallmount.gis_design_id,
    wallmount.st_x,
    wallmount.st_y,
    wallmount.ne_id,
    wallmount.prms_id,
    wallmount.jc_id,
    wallmount.mzone_id,
    wallmount.own_vendor_id
   FROM att_details_wallmount wallmount
     JOIN province_boundary prov ON wallmount.province_id = prov.id
     JOIN region_boundary reg ON wallmount.region_id = reg.id
     JOIN user_master um ON wallmount.created_by = um.user_id
     JOIN vendor_master vm ON wallmount.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON wallmount.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON wallmount.type = tp.id
     LEFT JOIN isp_brand_master bd ON wallmount.brand = bd.id
     LEFT JOIN isp_base_model ml ON wallmount.model = ml.id
     LEFT JOIN user_master um2 ON wallmount.modified_by = um2.user_id;





 DROP VIEW public.vw_att_details_bdb;

CREATE OR REPLACE VIEW public.vw_att_details_bdb
 AS
 SELECT bdb.system_id,
    bdb.network_id,
    bdb.bdb_name,
    round(bdb.latitude::numeric, 6) AS latitude,
    round(bdb.longitude::numeric, 6) AS longitude,
    bdb.pincode,
    bdb.address,
    bdb.specification,
    bdb.category,
    bdb.subcategory1,
    bdb.subcategory2,
    bdb.subcategory3,
    bdb.item_code,
    bdb.vendor_id,
    bdb.type,
    bdb.brand,
    bdb.model,
    bdb.construction,
    bdb.activation,
    bdb.accessibility,
    bdb.status,
    bdb.network_status,
    bdb.province_id,
    bdb.region_id,
    bdb.created_by,
    bdb.created_on,
    bdb.modified_by,
    bdb.modified_on,
    bdb.parent_system_id,
    bdb.parent_network_id,
    bdb.parent_entity_type,
    bdb.sequence_id,
    bdb.shaft_id,
    bdb.floor_id,
    bdb.project_id,
    bdb.planning_id,
    bdb.purpose_id,
    bdb.workorder_id,
    bdb.is_servingdb,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS bdb_type,
    bd.brand AS bdb_brand,
    ml.model AS bdb_model,
    fn_get_date(bdb.created_on) AS created_date,
    fn_get_date(bdb.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = bdb.accessibility) AS bdb_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = bdb.construction) AS bdb_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = bdb.activation) AS bdb_activation,
    bdb.no_of_input_port,
    bdb.no_of_output_port,
    bdb.no_of_port,
    bdb.entity_category,
    bdb.entity_category AS additional_attributes,
        CASE
            WHEN bdb.no_of_input_port <> 0 AND bdb.no_of_output_port <> 0 THEN ((bdb.no_of_input_port::character varying::text || ''::text) || bdb.no_of_output_port::character varying::text)::character varying
            ELSE bdb.no_of_port::character varying
        END AS total_ports,
    bdb.bdb_name AS entity_name,
    bdb.barcode,
    bdb.acquire_from,
    bdb.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    bdb.source_ref_type,
    bdb.source_ref_id,
    bdb.source_ref_description,
    bdb.status_remark,
    bdb.status_updated_by,
    bdb.status_updated_on,
    bdb.is_visible_on_map,
    bdb.primary_pod_system_id,
    bdb.secondary_pod_system_id,
    bdb.is_new_entity,
    bdb.remarks,
    bdb.audit_item_master_id,
    bdb.is_acquire_from,
    s.shaft_name,
    f.floor_name,
    bdb.origin_from,
    bdb.origin_ref_id,
    bdb.origin_ref_code,
    bdb.origin_ref_description,
    bdb.request_ref_id,
    bdb.requested_by,
    bdb.request_approved_by,
    bdb.subarea_id,
    bdb.area_id,
    bdb.dsa_id,
    bdb.csa_id,
    bdb.bom_sub_category,
    bdb.gis_design_id,
    bdb.st_x,
    bdb.st_y,
    bdb.ne_id,
    bdb.prms_id,
    bdb.jc_id,
    bdb.mzone_id,
    bdb.own_vendor_id
   FROM att_details_bdb bdb
     JOIN province_boundary prov ON bdb.province_id = prov.id
     JOIN region_boundary reg ON bdb.region_id = reg.id
     JOIN user_master um ON bdb.created_by = um.user_id
     JOIN vendor_master vm ON bdb.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON bdb.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON bdb.type = tp.id
     LEFT JOIN isp_brand_master bd ON bdb.brand = bd.id
     LEFT JOIN isp_base_model ml ON bdb.model = ml.id
     LEFT JOIN user_master um2 ON bdb.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'BDB'::text) m1 ON bdb.system_id = m1.entity_id
     LEFT JOIN isp_shaft_info s ON m1.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m1.floor_id = f.system_id;






 DROP VIEW public.vw_att_details_manhole;

CREATE OR REPLACE VIEW public.vw_att_details_manhole
 AS
 SELECT manhole.system_id,
    manhole.network_id,
    manhole.manhole_name,
    round(manhole.latitude::numeric, 6) AS latitude,
    round(manhole.longitude::numeric, 6) AS longitude,
    manhole.address,
    manhole.specification,
    manhole.category,
    manhole.subcategory1,
    manhole.subcategory2,
    manhole.subcategory3,
    manhole.item_code,
    manhole.vendor_id,
    manhole.type,
    manhole.brand,
    manhole.model,
    manhole.construction,
    manhole.activation,
    manhole.accessibility,
    manhole.status,
    manhole.network_status,
    manhole.province_id,
    manhole.region_id,
    manhole.created_by,
    manhole.created_on,
    manhole.modified_by,
    manhole.modified_on,
    prov.province_name,
    reg.region_name,
    manhole.parent_system_id,
    manhole.parent_network_id,
    manhole.parent_entity_type,
    manhole.sequence_id,
    manhole.project_id,
    manhole.planning_id,
    manhole.purpose_id,
    manhole.workorder_id,
    manhole.is_virtual,
    manhole.construction_type,
    manhole.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS manhole_type,
    bd.brand AS manhole_brand,
    ml.model AS manhole_model,
    fn_get_date(manhole.created_on) AS created_date,
    fn_get_date(manhole.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.accessibility) AS manhole_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.construction) AS manhole_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = manhole.activation) AS manhole_activation,
    manhole.barcode,
    manhole.is_buried,
    manhole.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    manhole.source_ref_type,
    manhole.source_ref_id,
    manhole.source_ref_description,
    manhole.status_remark,
    manhole.status_updated_by,
    manhole.status_updated_on,
    manhole.is_visible_on_map,
    manhole.primary_pod_system_id,
    manhole.secondary_pod_system_id,
    manhole.is_new_entity,
    manhole.remarks,
    manhole.manhole_types,
    manhole.audit_item_master_id,
    manhole.is_acquire_from,
    manhole.origin_from,
    manhole.origin_ref_id,
    manhole.origin_ref_code,
    manhole.origin_ref_description,
    manhole.request_ref_id,
    manhole.requested_by,
    manhole.request_approved_by,
    manhole.subarea_id,
    manhole.area_id,
    manhole.dsa_id,
    manhole.csa_id,
    manhole.bom_sub_category,
    manhole.gis_design_id,
    manhole.st_x,
    manhole.st_y,
    manhole.ne_id,
    manhole.prms_id,
    manhole.jc_id,
    manhole.mzone_id,
    manhole.area,
    manhole.authority,
    manhole.route_name,
    manhole.mcgm_ward,
    manhole.hierarchy_type,
    manhole.section_name,
    manhole.aerial_location,
    manhole.chamber_remark,
    manhole.generic_section_name,
    manhole.own_vendor_id
   FROM att_details_manhole manhole
     JOIN province_boundary prov ON manhole.province_id = prov.id
     JOIN region_boundary reg ON manhole.region_id = reg.id
     JOIN user_master um ON manhole.created_by = um.user_id
     JOIN vendor_master vm ON manhole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON manhole.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON manhole.type = tp.id
     LEFT JOIN isp_brand_master bd ON manhole.brand = bd.id
     LEFT JOIN isp_base_model ml ON manhole.model = ml.id
     LEFT JOIN user_master um2 ON manhole.modified_by = um2.user_id;









 DROP VIEW public.vw_att_details_microduct;

CREATE OR REPLACE VIEW public.vw_att_details_microduct
 AS
 SELECT microduct.system_id,
    microduct.network_id,
    microduct.microduct_name AS network_name,
    round(microduct.calculated_length::numeric, 2) AS calculated_length,
    round(microduct.manual_length::numeric, 2) AS manual_length,
    microduct.a_system_id,
    microduct.a_entity_type,
    microduct.b_system_id,
    microduct.a_location,
    microduct.b_location,
    microduct.b_entity_type,
    microduct.sequence_id,
    microduct.network_status,
    microduct.status,
    microduct.pin_code,
    microduct.province_id,
    microduct.region_id,
    microduct.utilization,
    microduct.no_of_cables,
    microduct.offset_value,
    microduct.construction,
    microduct.activation,
    microduct.accessibility,
    microduct.specification,
    microduct.category,
    round(microduct.inner_dimension::numeric, 2) AS inner_dimension,
    round(microduct.outer_dimension::numeric, 2) AS outer_dimension,
    microduct.microduct_type,
    microduct.color_code,
    microduct.subcategory1,
    microduct.subcategory2,
    microduct.subcategory3,
    microduct.item_code,
    microduct.vendor_id,
    microduct.type,
    microduct.brand,
    microduct.model,
    microduct.remarks,
    microduct.created_by,
    microduct.created_on,
    microduct.modified_by,
    microduct.modified_on,
    microduct.trench_id,
    microduct.project_id,
    microduct.planning_id,
    microduct.purpose_id,
    microduct.workorder_id,
    prov.province_name,
    microduct.parent_system_id,
    microduct.parent_entity_type,
    microduct.parent_network_id,
    microduct.acquire_from,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    bd.brand AS duct_brand,
    ml.model AS duct_model,
    fn_get_date(microduct.created_on) AS created_date,
    fn_get_date(microduct.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = microduct.accessibility) AS duct_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = microduct.construction) AS duct_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = microduct.activation) AS duct_activation,
    microduct.barcode,
    microduct.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    microduct.source_ref_type,
    microduct.source_ref_id,
    microduct.source_ref_description,
    microduct.status_remark,
    microduct.status_updated_by,
    microduct.status_updated_on,
    microduct.primary_pod_system_id,
    microduct.secondary_pod_system_id,
    microduct.is_new_entity,
    microduct.audit_item_master_id,
    microduct.internal_diameter,
    microduct.external_diameter,
    microduct.material_type,
    microduct.is_acquire_from,
    microduct.origin_from,
    microduct.origin_ref_id,
    microduct.origin_ref_code,
    microduct.origin_ref_description,
    microduct.request_ref_id,
    microduct.requested_by,
    microduct.request_approved_by,
    microduct.subarea_id,
    microduct.area_id,
    microduct.dsa_id,
    microduct.csa_id,
    microduct.bom_sub_category,
    microduct.gis_design_id,
    itm.no_of_ways,
    microduct.ne_id,
    microduct.prms_id,
    microduct.jc_id,
    microduct.mzone_id,
    microduct.own_vendor_id
   FROM att_details_microduct microduct
     JOIN province_boundary prov ON microduct.province_id = prov.id
     JOIN region_boundary reg ON microduct.region_id = reg.id
     JOIN user_master um ON microduct.created_by = um.user_id
     JOIN vendor_master vm ON microduct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON microduct.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON microduct.type = tp.id
     LEFT JOIN isp_brand_master bd ON microduct.brand = bd.id
     LEFT JOIN isp_base_model ml ON microduct.model = ml.id
     LEFT JOIN user_master um2 ON microduct.modified_by = um2.user_id
     LEFT JOIN item_template_microduct itm ON itm.vendor_id = microduct.vendor_id;





 DROP VIEW public.vw_att_details_pole;

CREATE OR REPLACE VIEW public.vw_att_details_pole
 AS
 SELECT pole.system_id,
    pole.network_id,
    pole.pole_name,
    pole.pole_type,
    pole.pole_height,
    pole.pole_no,
    round(pole.latitude::numeric, 6) AS latitude,
    round(pole.longitude::numeric, 6) AS longitude,
    pole.address,
    pole.specification,
    pole.category,
    pole.subcategory1,
    pole.subcategory2,
    pole.subcategory3,
    pole.item_code,
    pole.vendor_id,
    pole.type,
    pole.brand,
    pole.model,
    pole.construction,
    pole.activation,
    pole.accessibility,
    pole.status,
    pole.network_status,
    pole.province_id,
    pole.region_id,
    pole.created_by,
    pole.created_on,
    pole.modified_by,
    pole.modified_on,
    prov.province_name,
    reg.region_name,
    pole.parent_system_id,
    pole.parent_network_id,
    pole.parent_entity_type,
    pole.sequence_id,
    pole.project_id,
    pole.planning_id,
    pole.purpose_id,
    pole.workorder_id,
    pole.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS pole_specification_type,
    bd.brand AS pole_brand,
    ml.model AS pole_model,
    fn_get_date(pole.created_on) AS created_date,
    fn_get_date(pole.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pole.accessibility) AS pole_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pole.construction) AS pole_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pole.activation) AS pole_activation,
    pole.barcode,
    pole.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    pole.source_ref_type,
    pole.source_ref_id,
    pole.source_ref_description,
    pole.status_remark,
    pole.status_updated_by,
    pole.status_updated_on,
    pole.is_visible_on_map,
    pole.primary_pod_system_id,
    pole.secondary_pod_system_id,
    pole.is_new_entity,
    pole.remarks,
    pole.audit_item_master_id,
    pole.is_acquire_from,
    pole.origin_from,
    pole.origin_ref_id,
    pole.origin_ref_code,
    pole.origin_ref_description,
    pole.request_ref_id,
    pole.requested_by,
    pole.request_approved_by,
    pole.subarea_id,
    pole.area_id,
    pole.dsa_id,
    pole.csa_id,
    pole.bom_sub_category,
    pole.gis_design_id,
    pole.st_x,
    pole.st_y,
    pole.ne_id,
    pole.prms_id,
    pole.jc_id,
    pole.mzone_id,
    pole.own_vendor_id
   FROM att_details_pole pole
     JOIN province_boundary prov ON pole.province_id = prov.id
     JOIN region_boundary reg ON pole.region_id = reg.id
     JOIN user_master um ON pole.created_by = um.user_id
     JOIN vendor_master vm ON pole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pole.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON pole.type = tp.id
     LEFT JOIN isp_brand_master bd ON pole.brand = bd.id
     LEFT JOIN isp_base_model ml ON pole.model = ml.id
     LEFT JOIN user_master um2 ON pole.modified_by = um2.user_id;





 DROP VIEW public.vw_att_details_cable;

CREATE OR REPLACE VIEW public.vw_att_details_cable
 AS
 SELECT cable.system_id,
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
    cable.route_name,
    cable.hierarchy_type,
    cable.section_name,
    cable.aerial_location,
    cable.cable_remark,
    cable.generic_section_name,
    cable.own_vendor_id
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





DROP VIEW public.vw_att_details_cable_report;
CREATE OR REPLACE VIEW public.vw_att_details_cable_report
 AS
 SELECT cable.system_id,
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
    cable.gis_design_id AS evoid,
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
    (( SELECT count(*) AS count
           FROM att_details_cable_info cableinfo
          WHERE cableinfo.cable_id = cable.system_id AND cableinfo.link_system_id > 0)) + (( SELECT count(*) AS count
           FROM att_details_cable_info cableinfo
          WHERE cableinfo.cable_id = cable.system_id AND lower(cableinfo.core_comment::text) ~~* lower('reserved%'::text))) AS used_core,
    cable.hierarchy_type,
    cable.section_name,
    cable.aerial_location,
    cable.generic_section_name,
    vm3.name as own_vendor_name,
    cable.cable_remark,
    cable.parent_cable_system_id,
    cable.parent_cable_netwok_id,
    ( SELECT user_master.user_name AS splited_by
           FROM user_master
          WHERE user_master.user_id = cable.splited_by::integer) AS splited_by,
    to_char(cable.splitted_on, 'DD-Mon-YY'::text) AS splitted_on
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
     join vendor_master vm3 on vm3.id= cable.own_vendor_id::integer
     LEFT JOIN entity_status_master es ON es.status::text = cable.status::text;





 DROP VIEW public.vw_att_details_manhole_report;

CREATE OR REPLACE VIEW public.vw_att_details_manhole_report
 AS
 SELECT manhole.system_id,
    manhole.network_id,
    manhole.manhole_name,
    round(manhole.latitude::numeric, 6) AS latitude,
    round(manhole.longitude::numeric, 6) AS longitude,
    manhole.address,
    manhole.specification,
    manhole.category,
    manhole.subcategory1,
    manhole.subcategory2,
    manhole.subcategory3,
    manhole.item_code,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_network_status(manhole.network_status) AS network_status,
    COALESCE(fn_get_entity_status(manhole.status), manhole.status) AS status,
    prov.province_name,
    reg.region_name,
    manhole.region_id,
    manhole.province_id,
    manhole.parent_network_id AS parent_code,
    manhole.parent_entity_type AS parent_type,
        CASE
            WHEN manhole.is_virtual = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_virtual,
    manhole.construction_type,
    manhole.acquire_from,
    vm.name AS vendor_name,
    fn_get_date(manhole.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(manhole.modified_on) AS modified_on,
    um2.user_name AS modified_by,
        CASE
            WHEN manhole.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    manhole.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
        CASE
            WHEN manhole.is_buried = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_buried,
    manhole.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    manhole.source_ref_type,
    manhole.source_ref_id,
    manhole.source_ref_description,
    manhole.status_remark,
    manhole.status_updated_by,
    fn_get_date(manhole.status_updated_on) AS status_updated_on,
    manhole.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    manhole.remarks,
    manhole.manhole_types,
    reg.country_name,
    manhole.origin_from,
    manhole.origin_ref_id,
    manhole.origin_ref_code,
    manhole.origin_ref_description,
    manhole.request_ref_id,
    manhole.requested_by,
    manhole.request_approved_by,
    manhole.subarea_id,
    manhole.area_id,
    manhole.dsa_id,
    manhole.csa_id,
    manhole.bom_sub_category,
    manhole.gis_design_id AS design_id,
    manhole.st_x,
    manhole.st_y,
    manhole.ne_id,
    manhole.prms_id,
    manhole.mzone_id,
    manhole.jc_id,
    manhole.area,
    manhole.authority,
    manhole.route_name,
    manhole.served_by_ring,
    manhole.mcgm_ward,
    manhole.hierarchy_type,
    manhole.section_name,
    manhole.generic_section_name,
    manhole.aerial_location,
    vm3.name as own_vendor_name,
    manhole.chamber_remark
   FROM att_details_manhole manhole
     JOIN province_boundary prov ON manhole.province_id = prov.id
     JOIN region_boundary reg ON manhole.region_id = reg.id
     JOIN user_master um ON manhole.created_by = um.user_id
     JOIN vendor_master vm ON manhole.vendor_id = vm.id
     JOIN vendor_master vm3 ON manhole.own_vendor_id::integer = vm3.id
     LEFT JOIN vendor_master vm2 ON manhole.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON manhole.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON manhole.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON manhole.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON manhole.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON manhole.purpose_id = purposem.system_id
     LEFT JOIN att_details_pod primarypod ON manhole.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON manhole.secondary_pod_system_id = secondarypod.system_id;





 DROP VIEW public.vw_att_details_spliceclosure_report;
CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_report
 AS
 SELECT sc.system_id,
    sc.network_id,
    sc.spliceclosure_name,
    round(sc.latitude::numeric, 6) AS latitude,
    round(sc.longitude::numeric, 6) AS longitude,
    sc.address,
    sc.specification,
    sc.category,
    sc.subcategory1,
    sc.subcategory2,
    sc.subcategory3,
    sc.item_code,
        CASE
            WHEN sc.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN sc.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN sc.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, sc.status) AS status,
    sc.pincode,
    prov.province_name,
    reg.region_name,
    sc.parent_network_id AS parent_code,
    sc.parent_entity_type AS parent_type,
    vm.name AS vendor_name,
    sc.no_of_ports,
    sc.no_of_input_port,
    sc.no_of_output_port,
    sc.acquire_from,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    to_char(sc.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(sc.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    sc.region_id,
    sc.province_id,
        CASE
            WHEN sc.is_virtual = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_virtual,
        CASE
            WHEN sc.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    sc.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    sc.utilization,
        CASE
            WHEN sc.utilization::text = 'L'::text THEN 'Low'::text
            WHEN sc.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN sc.utilization::text = 'H'::text THEN 'High'::text
            WHEN sc.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
        CASE
            WHEN sc.is_buried = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_buried,
    sc.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    sc.source_ref_type,
    sc.source_ref_id,
    sc.source_ref_description,
    sc.status_remark,
    sc.status_updated_by,
    to_char(sc.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    sc.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    sc.remarks,
    reg.country_name,
    sc.origin_from,
    sc.origin_ref_id,
    sc.origin_ref_code,
    sc.origin_ref_description,
    sc.request_ref_id,
    sc.requested_by,
    sc.request_approved_by,
    sc.subarea_id,
    sc.area_id,
    sc.dsa_id,
    sc.csa_id,
    sc.bom_sub_category,
    sc.gis_design_id AS design_id,
    sc.st_x,
    sc.st_y,
    sc.ne_id,
    sc.prms_id,
    sc.jc_id,
    sc.mzone_id,
    sc.served_by_ring,
    sc.hierarchy_type,
    sc.section_name,
    sc.generic_section_name,
    sc.aerial_location,
    vm3.name as own_vendor_name
   FROM att_details_spliceclosure sc
     JOIN province_boundary prov ON sc.province_id = prov.id
     JOIN region_boundary reg ON sc.region_id = reg.id
     JOIN user_master um ON sc.created_by = um.user_id
     JOIN vendor_master vm ON sc.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sc.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON sc.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON sc.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON sc.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON sc.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON sc.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON sc.modified_by = um2.user_id
     LEFT JOIN entity_notification_status entnotifystatus ON sc.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON sc.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON sc.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = sc.status::text;








 DROP VIEW public.vw_att_details_sector_report;

CREATE OR REPLACE VIEW public.vw_att_details_sector_report
 AS
 SELECT t.system_id,
    t.network_id,
    t.network_name,
    t.parent_system_id,
    t.parent_network_id,
    t.parent_entity_type,
    t.sequence_id,
    t.latitude,
    t.longitude,
    t.region_id,
    t.province_id,
    t.azimuth,
    t.technology,
    t.port_name,
    t.down_link,
    t.uplink,
    t.brand_name,
    t.remark,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    t.ownership_type,
    t.source_ref_type,
    t.source_ref_id,
    t.source_ref_description,
    t.audit_item_master_id,
    t.status_remark,
    t.status_updated_by,
    t.status_updated_on,
    fn_get_network_status(t.network_status) AS network_status,
    COALESCE(fn_get_entity_status(t.status), t.status) AS status,
    t.created_by,
    t.created_on,
    t.modified_by,
    t.modified_on,
    t.vendor_id,
    t.specification,
    t.category,
    t.subcategory1,
    t.subcategory2,
    t.subcategory3,
    t.item_code,
    t.frequency,
    t.sector_type,
    t.parent_site_id,
    t.sector_layer_id,
    t.node_identity,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    fn_get_date(t.created_on) AS created_date,
    fn_get_date(t.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    t.network_name AS entity_name,
    t.installation_number,
    t.installation_year,
    t.production_year,
    t.installation_company,
    t.installation_technician,
    t.installation,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_id AS created_by_id,
    t.remarks,
    reg.country_name,
    round(t.total_tilt::numeric, 2) AS total_tilt,
    t.origin_from,
    t.origin_ref_id,
    t.origin_ref_code,
    t.origin_ref_description,
    t.request_ref_id,
    t.requested_by,
    t.request_approved_by,
    t.subarea_id,
    t.area_id,
    t.dsa_id,
    t.csa_id,
    t.gis_design_id AS design_id,
    t.ne_id,
    t.prms_id,
    t.jc_id,
    t.mzone_id,
    t.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_sector t
     JOIN province_boundary prov ON t.province_id = prov.id
     JOIN region_boundary reg ON t.region_id = reg.id
     JOIN user_master um ON t.created_by = um.user_id
     JOIN vendor_master vm ON t.vendor_id = vm.id
     JOIN sector_color_master s ON upper(s.type::text) = upper(t.sector_type::text) AND upper(s.frequency::text) = upper(t.frequency::text)
     LEFT JOIN vendor_master vm2 ON t.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON t.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON t.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON t.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON t.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON t.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON t.purpose_id = purposem.system_id;








 DROP VIEW public.vw_att_details_antenna_report;

CREATE OR REPLACE VIEW public.vw_att_details_antenna_report
 AS
 SELECT ant.system_id,
    ant.network_id,
    ant.network_name,
    round(ant.latitude::numeric, 6) AS latitude,
    round(ant.longitude::numeric, 6) AS longitude,
    ant.parent_system_id,
    ant.parent_network_id,
    ant.parent_entity_type,
    ant.sequence_id,
    ant.antenna_type,
    ant.antenna_sub_type,
    round(ant.minimum_frequency::numeric, 2) AS minimum_frequency,
    round(ant.maximum_frequency::numeric, 2) AS maximum_frequency,
    round(ant.diameter_in_meter::numeric, 2) AS diameter_in_meter,
    round(ant.maximum_gain::numeric, 2) AS maximum_gain,
    round(ant.boresight_gain::numeric, 2) AS boresight_gain,
    ant.user_cross_polor_pattern,
    round(ant.co_polor_vertical_maximum_gain::numeric, 2) AS co_polor_vertical_maximum_gain,
    round(ant.co_polor_vertical_fb::numeric, 2) AS co_polor_vertical_fb,
    round(ant.co_polor_vertical_bw::numeric, 2) AS co_polor_vertical_bw,
    round(ant.co_polor_vertical_boresight::numeric, 2) AS co_polor_vertical_boresight,
    round(ant.cross_polor_vertical_maximum_gain::numeric, 2) AS cross_polor_vertical_maximum_gain,
    round(ant.cross_polor_vertical_fb::numeric, 2) AS cross_polor_vertical_fb,
    round(ant.cross_polor_vertical_bw::numeric, 2) AS cross_polor_vertical_bw,
    round(ant.cross_polor_vertical_boresight::numeric, 2) AS cross_polor_vertical_boresight,
    round(ant.co_polor_horizontal_maximum_gain::numeric, 2) AS co_polor_horizontal_maximum_gain,
    round(ant.co_polor_horizontal_fb::numeric, 2) AS co_polor_horizontal_fb,
    round(ant.co_polor_horizontal_bw::numeric, 2) AS co_polor_horizontal_bw,
    round(ant.cross_polor_horizontal_maximum_gain::numeric, 2) AS cross_polor_horizontal_maximum_gain,
    round(ant.co_polor_horizontal_boresight::numeric, 2) AS co_polor_horizontal_boresight,
    round(ant.cross_polor_horizontal_fb::numeric, 2) AS cross_polor_horizontal_fb,
    round(ant.cross_polor_horizontal_bw::numeric, 2) AS cross_polor_horizontal_bw,
    round(ant.mechanical_tilt::numeric, 2) AS mechanical_tilt,
    round(ant.electrical_tilt::numeric, 2) AS electrical_tilt,
    round(ant.total_tilt::numeric, 2) AS total_tilt,
    ant.remark,
    vm.id AS third_party_vendor_id,
    vm.name AS third_party_vendor_name,
    ant.ownership_type,
    ant.source_ref_type,
    ant.source_ref_id,
    ant.source_ref_description,
    ant.audit_item_master_id,
    ant.status_remark,
    ant.status_updated_by,
    um3.user_name AS status_updated_by_user,
    fn_get_date(ant.status_updated_on) AS status_updated_date,
    fn_get_network_status(ant.network_status) AS network_status,
    COALESCE(fn_get_entity_status(ant.status), ant.status) AS status,
    um.user_id AS created_by_id,
    ant.created_by,
    um.user_name AS created_by_user,
    fn_get_date(ant.created_on) AS created_date,
    ant.modified_by,
    um2.user_name AS modified_by_user,
    fn_get_date(ant.modified_on) AS modified_date,
    round(ant.height::numeric, 2) AS height,
    round(ant.azimuth::numeric, 2) AS azimuth,
    ant.antenna_operator,
    ant.region_id,
    reg.region_name,
    ant.province_id,
    prov.province_name,
    ant.project_id,
    ant.planning_id,
    ant.workorder_id,
    ant.purpose_id,
    fn_get_date(ant.created_on) AS created_on,
    fn_get_date(ant.modified_on) AS modified_on,
    ant.is_visible_on_map,
    ant.remarks,
    vm2.name AS vendor_name,
    ant.specification,
    reg.country_name,
    ant.model_number,
    ant.manufacturer_name,
    ant.polarization,
    ant.origin_from,
    ant.origin_ref_id,
    ant.origin_ref_code,
    ant.origin_ref_description,
    ant.request_ref_id,
    ant.requested_by,
    ant.request_approved_by,
    ant.subarea_id,
    ant.area_id,
    ant.dsa_id,
    ant.csa_id,
    ant.bom_sub_category,
    ant.gis_design_id AS design_id,
    ant.st_x,
    ant.st_y,
    ant.ne_id,
    ant.prms_id,
    ant.jc_id,
    ant.mzone_id,
    ant.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_antenna ant
     JOIN province_boundary prov ON ant.province_id = prov.id
     JOIN region_boundary reg ON ant.region_id = reg.id
     JOIN user_master um ON ant.created_by = um.user_id
     JOIN vendor_master vm2 ON ant.vendor_id = vm2.id
     JOIN vendor_master vm3 ON ant.own_vendor_id::integer = vm3.id
     LEFT JOIN vendor_master vm ON ant.third_party_vendor_id = vm.id
     LEFT JOIN user_master um2 ON ant.modified_by = um2.user_id
     LEFT JOIN user_master um3 ON ant.status_updated_by = um3.user_id
  ORDER BY ant.created_on;









 DROP VIEW public.vw_att_details_patchpanel_report;

CREATE OR REPLACE VIEW public.vw_att_details_patchpanel_report
 AS
 SELECT patchpanel.system_id,
    patchpanel.network_id,
    patchpanel.patchpanel_name,
    round(patchpanel.latitude::numeric, 6) AS latitude,
    round(patchpanel.longitude::numeric, 6) AS longitude,
    patchpanel.address,
    patchpanel.specification,
    patchpanel.category,
    patchpanel.subcategory1,
    patchpanel.subcategory2,
    patchpanel.subcategory3,
    patchpanel.item_code,
    fn_get_network_status(patchpanel.network_status) AS network_status,
    COALESCE(fn_get_entity_status(patchpanel.status), patchpanel.status) AS status,
    patchpanel.pincode,
    prov.province_name,
    reg.region_name,
    patchpanel.parent_network_id AS parent_code,
    patchpanel.parent_entity_type AS parent_type,
    vm.name AS vendor_name,
    patchpanel.no_of_port,
    patchpanel.no_of_input_port,
    patchpanel.no_of_output_port,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    fn_get_date(patchpanel.created_on) AS created_on,
    fn_get_date(patchpanel.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    patchpanel.region_id,
    patchpanel.province_id,
    patchpanel.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    patchpanel.utilization,
    patchpanel.acquire_from,
    fn_utilization_get_full_text(patchpanel.utilization) AS utilization_text,
    patchpanel.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    patchpanel.source_ref_type,
    patchpanel.source_ref_id,
    patchpanel.source_ref_description,
    patchpanel.status_remark,
    patchpanel.status_updated_by,
    fn_get_date(patchpanel.status_updated_on) AS status_updated_on,
    patchpanel.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    patchpanel.remarks,
    patchpanel.patchpanel_type,
    reg.country_name,
    patchpanel.origin_from,
    patchpanel.origin_ref_id,
    patchpanel.origin_ref_code,
    patchpanel.origin_ref_description,
    patchpanel.request_ref_id,
    patchpanel.requested_by,
    patchpanel.request_approved_by,
    patchpanel.subarea_id,
    patchpanel.area_id,
    patchpanel.dsa_id,
    patchpanel.csa_id,
    patchpanel.bom_sub_category,
    patchpanel.gis_design_id AS design_id,
    patchpanel.st_x,
    patchpanel.st_y,
    patchpanel.ne_id,
    patchpanel.prms_id,
    patchpanel.jc_id,
    patchpanel.mzone_id,
    patchpanel.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_patchpanel patchpanel
     JOIN province_boundary prov ON patchpanel.province_id = prov.id
     JOIN region_boundary reg ON patchpanel.region_id = reg.id
     JOIN user_master um ON patchpanel.created_by = um.user_id
     JOIN vendor_master vm ON patchpanel.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON patchpanel.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON patchpanel.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON patchpanel.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON patchpanel.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON patchpanel.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON patchpanel.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON patchpanel.modified_by = um2.user_id
     LEFT JOIN att_details_pod primarypod ON patchpanel.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON patchpanel.secondary_pod_system_id = secondarypod.system_id;






 DROP VIEW public.vw_att_details_handhole_report;

CREATE OR REPLACE VIEW public.vw_att_details_handhole_report
 AS
 SELECT handhole.system_id,
    handhole.network_id,
    handhole.handhole_name,
    round(handhole.latitude::numeric, 6) AS latitude,
    round(handhole.longitude::numeric, 6) AS longitude,
    handhole.address,
    handhole.specification,
    handhole.category,
    handhole.subcategory1,
    handhole.subcategory2,
    handhole.subcategory3,
    handhole.item_code,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_network_status(handhole.network_status) AS network_status,
    COALESCE(fn_get_entity_status(handhole.status), handhole.status) AS status,
    prov.province_name,
    reg.region_name,
    handhole.region_id,
    handhole.province_id,
    handhole.parent_network_id AS parent_code,
    handhole.parent_entity_type AS parent_type,
        CASE
            WHEN handhole.is_virtual = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_virtual,
    handhole.construction_type,
    handhole.acquire_from,
    vm.name AS vendor_name,
    fn_get_date(handhole.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(handhole.modified_on) AS modified_on,
    um2.user_name AS modified_by,
        CASE
            WHEN handhole.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    handhole.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
        CASE
            WHEN handhole.is_buried = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_buried,
    handhole.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    handhole.source_ref_type,
    handhole.source_ref_id,
    handhole.source_ref_description,
    handhole.status_remark,
    handhole.status_updated_by,
    fn_get_date(handhole.status_updated_on) AS status_updated_on,
    handhole.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    handhole.remarks,
    reg.country_name,
    handhole.origin_from,
    handhole.origin_ref_id,
    handhole.origin_ref_code,
    handhole.origin_ref_description,
    handhole.request_ref_id,
    handhole.requested_by,
    handhole.request_approved_by,
    handhole.subarea_id,
    handhole.area_id,
    handhole.dsa_id,
    handhole.csa_id,
    handhole.bom_sub_category,
    handhole.gis_design_id AS design_id,
    handhole.st_x,
    handhole.st_y,
    handhole.ne_id,
    handhole.prms_id,
    handhole.mzone_id,
    handhole.jc_id,
    handhole.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_handhole handhole
     JOIN province_boundary prov ON handhole.province_id = prov.id
     JOIN region_boundary reg ON handhole.region_id = reg.id
     JOIN user_master um ON handhole.created_by = um.user_id
     JOIN vendor_master vm ON handhole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON handhole.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON handhole.own_vendor_id::integer = vm2.id
     LEFT JOIN user_master um2 ON handhole.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON handhole.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON handhole.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON handhole.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON handhole.purpose_id = purposem.system_id
     LEFT JOIN att_details_pod primarypod ON handhole.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON handhole.secondary_pod_system_id = secondarypod.system_id;







-- View: public.vw_att_details_cabinet_report

-- DROP VIEW public.vw_att_details_cabinet_report;

CREATE OR REPLACE VIEW public.vw_att_details_cabinet_report
 AS
 SELECT cabinet.system_id,
    cabinet.network_id,
    cabinet.cabinet_name,
    round(cabinet.latitude::numeric, 6) AS latitude,
    round(cabinet.longitude::numeric, 6) AS longitude,
    cabinet.pincode,
    cabinet.address,
    cabinet.specification,
    cabinet.category,
    cabinet.subcategory1,
    cabinet.subcategory2,
    cabinet.subcategory3,
    cabinet.item_code,
    fn_get_network_status(cabinet.network_status) AS network_status,
    COALESCE(fn_get_entity_status(cabinet.status), cabinet.status) AS status,
    prov.province_name,
    cabinet.parent_network_id AS parent_code,
    cabinet.parent_entity_type AS parent_type,
    cabinet.acquire_from,
    reg.region_name,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_date(cabinet.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(cabinet.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    cabinet.region_id,
    cabinet.province_id,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    cabinet.cabinet_type,
    cabinet.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    cabinet.source_ref_type,
    cabinet.source_ref_id,
    cabinet.source_ref_description,
    cabinet.status_remark,
    cabinet.status_updated_by,
    fn_get_date(cabinet.status_updated_on) AS status_updated_on,
    cabinet.is_visible_on_map,
    cabinet.remarks,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    round(cabinet.length::numeric, 2) AS length,
    round(cabinet.width::numeric, 2) AS width,
    round(cabinet.height::numeric, 2) AS height,
    reg.country_name,
    cabinet.origin_from,
    cabinet.origin_ref_id,
    cabinet.origin_ref_code,
    cabinet.origin_ref_description,
    cabinet.request_ref_id,
    cabinet.requested_by,
    cabinet.request_approved_by,
    cabinet.subarea_id,
    cabinet.area_id,
    cabinet.dsa_id,
    cabinet.csa_id,
    cabinet.bom_sub_category,
    cabinet.gis_design_id AS design_id,
    cabinet.st_x,
    cabinet.st_y,
    cabinet.ne_id,
    cabinet.prms_id,
    cabinet.jc_id,
    cabinet.mzone_id,
    cabinet.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_cabinet cabinet
     JOIN province_boundary prov ON cabinet.province_id = prov.id
     JOIN region_boundary reg ON cabinet.region_id = reg.id
     JOIN user_master um ON cabinet.created_by = um.user_id
     JOIN vendor_master vm ON cabinet.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cabinet.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON cabinet.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON cabinet.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON cabinet.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON cabinet.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON cabinet.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON cabinet.purpose_id = purposem.system_id
     LEFT JOIN att_details_pod primarypod ON cabinet.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON cabinet.secondary_pod_system_id = secondarypod.system_id;





 DROP VIEW public.vw_att_details_tower_report;

CREATE OR REPLACE VIEW public.vw_att_details_tower_report
 AS
 SELECT t.system_id,
    t.network_id,
    t.network_name,
    t.parent_system_id,
    t.parent_network_id,
    t.parent_entity_type,
    t.sequence_id,
    round(t.latitude::numeric, 6) AS latitude,
    round(t.longitude::numeric, 6) AS longitude,
    t.region_id,
    t.province_id,
    um.user_id AS created_by_id,
    t.address,
    round(t.elevation::numeric, 2) AS elevation,
    round(t.tower_height::numeric, 2) AS tower_height,
    t.operator_name,
    t.no_of_sectors,
    t.tenancy,
    t.network_type,
    t.remark,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    t.ownership_type,
    t.source_ref_type,
    t.source_ref_id,
    t.source_ref_description,
    t.audit_item_master_id,
    t.status_remark,
    t.status_updated_by,
    fn_get_date(t.status_updated_on) AS status_updated_on,
    fn_get_network_status(t.network_status) AS network_status,
    COALESCE(fn_get_entity_status(t.status), t.status) AS status,
    t.created_by,
    fn_get_date(t.created_on) AS created_on,
    t.modified_by,
    fn_get_date(t.modified_on) AS modified_on,
    t.project_id,
    t.planning_id,
    t.purpose_id,
    t.workorder_id,
    t.acquire_from,
    t.vendor_id,
    t.specification,
    t.category,
    t.subcategory1,
    t.subcategory2,
    t.subcategory3,
    t.item_code,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    fn_get_date(t.created_on) AS created_date,
    fn_get_date(t.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    t.network_name AS entity_name,
    t.installation_number,
    t.installation_year,
    t.production_year,
    t.installation_company,
    t.installation_technician,
    t.installation,
    reg.country_name,
    t.origin_ref_id,
    t.origin_ref_code,
    t.origin_ref_description,
    t.request_ref_id,
    t.requested_by,
    t.request_approved_by,
    t.subarea_id,
    t.area_id,
    t.dsa_id,
    t.csa_id,
    t.bom_sub_category,
    t.gis_design_id AS design_id,
    t.st_x,
    t.st_y,
    t.ne_id,
    t.prms_id,
    t.jc_id,
    t.mzone_id,
    t.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_tower t
     JOIN province_boundary prov ON t.province_id = prov.id
     JOIN region_boundary reg ON t.region_id = reg.id
     JOIN user_master um ON t.created_by = um.user_id
     JOIN vendor_master vm ON t.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON t.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON t.own_vendor_id::integer = vm2.id
     LEFT JOIN user_master um2 ON t.modified_by = um2.user_id;







 DROP VIEW public.vw_att_details_duct_report;

CREATE OR REPLACE VIEW public.vw_att_details_duct_report
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.duct_name,
    round(duct.calculated_length::numeric, 2) AS calculated_length,
    round(duct.manual_length::numeric, 2) AS manual_length,
    duct.a_location AS start_point_a,
    duct.b_location AS start_point_b,
    duct.a_entity_type,
    duct.b_entity_type,
    fn_get_network_status(duct.network_status) AS network_status,
    COALESCE(fn_get_entity_status(duct.status), duct.status) AS status,
    duct.pin_code,
    COALESCE(duct.utilization, '0'::character varying) AS utilization,
    duct.no_of_cables,
    duct.offset_value,
    duct.construction,
    duct.activation,
    duct.accessibility,
    duct.specification,
    duct.category,
    round(duct.inner_dimension::numeric, 2) AS inner_dimension,
    round(duct.outer_dimension::numeric, 2) AS outer_dimension,
    duct.duct_type,
    duct.color_code,
    duct.subcategory1,
    duct.subcategory2,
    duct.subcategory3,
    duct.item_code,
    duct.remarks,
    duct.parent_entity_type AS parent_type,
    duct.parent_network_id AS parent_code,
    duct.acquire_from,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_date(duct.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(duct.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    COALESCE(pmapping.province_id, duct.province_id) AS province_id,
    COALESCE(pmapping.region_id, duct.region_id) AS region_id,
        CASE
            WHEN duct.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    reg.region_name,
    prov.province_name,
    duct.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    duct.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    duct.source_ref_type,
    duct.source_ref_id,
    duct.source_ref_description,
    duct.status_remark,
    duct.status_updated_by,
    fn_get_date(duct.status_updated_on) AS status_updated_on,
    duct.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    vm.name AS vendor_name,
    reg.country_name,
    duct.duct_capacity,
    duct.origin_from,
    duct.origin_ref_id,
    duct.origin_ref_code,
    duct.origin_ref_description,
    duct.request_ref_id,
    duct.requested_by,
    duct.request_approved_by,
    duct.subarea_id,
    duct.area_id,
    duct.dsa_id,
    duct.csa_id,
    duct.bom_sub_category,
    duct.gis_design_id AS design_id,
    duct.ne_id,
    duct.prms_id,
    duct.jc_id,
    duct.mzone_id,
    duct.served_by_ring,
        CASE
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"P"'::text THEN 'Planned'::text
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"A"'::text THEN 'Asbuilt'::text
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"D"'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS ticket_network_status,
    duct.a_latitude,
    duct.b_longitude,
    duct.a_region,
    duct.b_region,
    duct.a_city,
    duct.b_city,
    duct.total_slack_count,
    duct.total_slack_length,
    vm3.name as own_vendor_name
   FROM att_details_duct duct
     JOIN user_master um ON duct.created_by = um.user_id
     JOIN vendor_master vm ON duct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON duct.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON duct.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON duct.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON duct.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON duct.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON duct.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON duct.purpose_id = purposem.system_id
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(duct.network_id::text) ~~ upper('NLD%'::text) THEN duct.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('DUCT'::text)
            ELSE 1 = 2
        END
     JOIN province_boundary prov ON COALESCE(pmapping.province_id, duct.province_id) = prov.id
     JOIN region_boundary reg ON COALESCE(pmapping.region_id, duct.region_id) = reg.id
     LEFT JOIN att_details_pod primarypod ON duct.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON duct.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN att_details_edit_entity_info adi ON adi.source_ref_id::text = duct.source_ref_id::text AND adi.source_ref_type::text = duct.source_ref_type::text AND duct.system_id = adi.entity_system_id AND upper('Duct'::text) = upper(adi.entity_type::text) AND adi.entity_action_id = 13;









 DROP VIEW public.vw_att_details_cdb_report;

CREATE OR REPLACE VIEW public.vw_att_details_cdb_report
 AS
 SELECT cdb.system_id,
    cdb.network_id,
    cdb.cdb_name,
    round(cdb.latitude::numeric, 6) AS latitude,
    round(cdb.longitude::numeric, 6) AS longitude,
    cdb.pincode,
    cdb.address,
    cdb.specification,
    cdb.category,
    cdb.subcategory1,
    cdb.subcategory2,
    cdb.subcategory3,
    cdb.item_code,
        CASE
            WHEN cdb.is_servingdb THEN 'YES'::text
            ELSE 'NO'::text
        END AS is_servingdb,
    fn_get_network_status(cdb.network_status) AS network_status,
    COALESCE(fn_get_entity_status(cdb.status), cdb.status) AS status,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    cdb.parent_network_id AS parent_code,
    cdb.parent_entity_type AS parent_type,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    cdb.no_of_input_port,
    cdb.no_of_output_port,
    cdb.entity_category,
    cdb.no_of_port,
    fn_get_date(cdb.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(cdb.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    cdb.region_id,
    cdb.province_id,
        CASE
            WHEN cdb.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    cdb.barcode,
    cdb.acquire_from,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    cdb.utilization,
    fn_utilization_get_full_text(cdb.utilization) AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    cdb.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    cdb.source_ref_type,
    cdb.source_ref_id,
    cdb.source_ref_description,
    cdb.status_remark,
    cdb.status_updated_by,
    fn_get_date(cdb.status_updated_on) AS status_updated_on,
    cdb.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    cdb.remarks,
    reg.country_name,
    cdb.origin_from,
    cdb.origin_ref_id,
    cdb.origin_ref_code,
    cdb.origin_ref_description,
    cdb.request_ref_id,
    cdb.requested_by,
    cdb.request_approved_by,
    cdb.subarea_id,
    cdb.area_id,
    cdb.dsa_id,
    cdb.csa_id,
    cdb.bom_sub_category,
    cdb.gis_design_id AS design_id,
    cdb.st_x,
    cdb.st_y,
    cdb.ne_id,
    cdb.prms_id,
    cdb.jc_id,
    cdb.mzone_id,
    cdb.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_cdb cdb
     JOIN province_boundary prov ON cdb.province_id = prov.id
     JOIN region_boundary reg ON cdb.region_id = reg.id
     JOIN user_master um ON cdb.created_by = um.user_id
     JOIN vendor_master vm ON cdb.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cdb.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON cdb.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON cdb.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON cdb.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON cdb.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON cdb.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON cdb.purpose_id = purposem.system_id
     LEFT JOIN entity_notification_status entnotifystatus ON cdb.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON cdb.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON cdb.secondary_pod_system_id = secondarypod.system_id;








 DROP VIEW public.vw_att_details_adb_report;

CREATE OR REPLACE VIEW public.vw_att_details_adb_report
 AS
 SELECT adb.system_id,
    adb.network_id,
    adb.adb_name,
    round(adb.latitude::numeric, 6) AS latitude,
    round(adb.longitude::numeric, 6) AS longitude,
    adb.pincode,
    adb.address,
    adb.specification,
    adb.category,
    adb.subcategory1,
    adb.subcategory2,
    adb.subcategory3,
    adb.acquire_from,
    adb.item_code,
        CASE
            WHEN adb.is_servingdb THEN 'YES'::text
            ELSE 'NO'::text
        END AS is_servingdb,
    COALESCE(fn_get_entity_status(adb.status), adb.status) AS status,
    fn_get_network_status(adb.network_status) AS network_status,
    um.user_name AS created_by,
    fn_get_date(adb.created_on) AS created_on,
    um2.user_name AS modified_by,
    fn_get_date(adb.modified_on) AS modified_on,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    adb.parent_network_id AS parent_code,
    adb.parent_entity_type AS parent_type,
    adb.no_of_input_port,
    adb.no_of_output_port,
    adb.entity_category,
    adb.no_of_port,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    adb.region_id,
    adb.province_id,
        CASE
            WHEN adb.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    adb.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    adb.utilization,
    fn_utilization_get_full_text(adb.utilization) AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    adb.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    adb.source_ref_type,
    adb.source_ref_id,
    adb.source_ref_description,
    adb.status_remark,
    adb.status_updated_by,
    fn_get_date(adb.status_updated_on) AS status_updated_on,
    adb.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    adb.remarks,
    reg.country_name,
    adb.origin_from,
    adb.origin_ref_id,
    adb.origin_ref_code,
    adb.origin_ref_description,
    adb.request_ref_id,
    adb.requested_by,
    adb.request_approved_by,
    adb.subarea_id,
    adb.area_id,
    adb.dsa_id,
    adb.csa_id,
    adb.bom_sub_category,
    adb.gis_design_id AS design_id,
    adb.st_x,
    adb.st_y,
    adb.ne_id,
    adb.prms_id,
    adb.jc_id,
    adb.mzone_id,
    adb.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_adb adb
     JOIN province_boundary prov ON adb.province_id = prov.id
     JOIN region_boundary reg ON adb.region_id = reg.id
     JOIN user_master um ON adb.created_by = um.user_id
     LEFT JOIN user_master um2 ON adb.modified_by = um2.user_id
     JOIN vendor_master vm ON adb.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON adb.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON adb.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON adb.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON adb.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON adb.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON adb.purpose_id = purposem.system_id
     LEFT JOIN entity_notification_status entnotifystatus ON adb.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON adb.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON adb.secondary_pod_system_id = secondarypod.system_id;










 DROP VIEW public.vw_att_details_splitter_report;

CREATE OR REPLACE VIEW public.vw_att_details_splitter_report
 AS
 SELECT splt.system_id,
    splt.network_id,
    splt.splitter_name,
    round(splt.latitude::numeric, 6) AS latitude,
    round(splt.longitude::numeric, 6) AS longitude,
    splt.address,
    splt.splitter_ratio,
    splt.specification,
    splt.category,
    splt.subcategory1,
    splt.subcategory2,
    splt.subcategory3,
    splt.item_code,
    splt.acquire_from,
        CASE
            WHEN splt.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN splt.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN splt.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.status, splt.status) AS status,
    prov.province_name,
    reg.region_name,
    splt.parent_network_id AS parent_code,
    ld.layer_title AS parent_type,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    splt.splitter_type,
    to_char(splt.created_on, 'DD-Mon-YY'::text) AS created_on,
    um.user_name AS created_by,
    to_char(splt.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    splt.region_id,
    splt.province_id,
    splt.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    splt.utilization,
        CASE
            WHEN splt.utilization::text = 'L'::text THEN 'Low'::text
            WHEN splt.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN splt.utilization::text = 'H'::text THEN 'High'::text
            WHEN splt.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    splt.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    splt.source_ref_type,
    splt.source_ref_id,
    splt.source_ref_description,
    splt.status_remark,
    splt.status_updated_by,
    to_char(splt.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    splt.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    splt.remarks,
    reg.country_name,
    splt.origin_from,
    splt.origin_ref_id,
    splt.origin_ref_code,
    splt.origin_ref_description,
    splt.request_ref_id,
    splt.requested_by,
    splt.request_approved_by,
    splt.subarea_id,
    splt.area_id,
    splt.dsa_id,
    splt.csa_id,
    splt.bom_sub_category,
    splt.gis_design_id AS design_id,
    splt.st_x,
    splt.st_y,
    splt.ne_id,
    splt.prms_id,
    splt.jc_id,
    splt.mzone_id,
    splt.served_by_ring,
    c.rfs_status,
    splt.power_meter_reading,
    vm3.name as own_vendor_name
   FROM att_details_splitter splt
     JOIN province_boundary prov ON splt.province_id = prov.id
     JOIN region_boundary reg ON splt.region_id = reg.id
     JOIN user_master um ON splt.created_by = um.user_id
     JOIN vendor_master vm ON splt.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON splt.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON splt.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON splt.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON splt.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON splt.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON splt.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON splt.modified_by = um2.user_id
     LEFT JOIN entity_notification_status entnotifystatus ON splt.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON splt.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON splt.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = splt.status::text
     LEFT JOIN layer_details ld ON ld.layer_name::text = splt.parent_entity_type::text
     LEFT JOIN att_details_csa c ON COALESCE(splt.csa_system_id, 0) > 0 AND splt.csa_system_id::text = c.system_id::text AND splt.splitter_type::text = 'Secondary'::text;






 DROP VIEW public.vw_att_details_mpod_report;

CREATE OR REPLACE VIEW public.vw_att_details_mpod_report
 AS
 SELECT mpod.system_id,
    mpod.network_id,
    mpod.mpod_name,
    round(mpod.latitude::numeric, 6) AS latitude,
    round(mpod.longitude::numeric, 6) AS longitude,
    mpod.pincode,
    mpod.address,
    mpod.specification,
    mpod.category,
    mpod.subcategory1,
    mpod.subcategory2,
    mpod.subcategory3,
    mpod.item_code,
    fn_get_network_status(mpod.network_status) AS network_status,
    COALESCE(fn_get_entity_status(mpod.status), mpod.status) AS status,
    prov.province_name,
    mpod.parent_network_id AS parent_code,
    mpod.parent_entity_type AS parent_type,
    mpod.acquire_from,
    reg.region_name,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_date(mpod.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(mpod.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    mpod.region_id,
    mpod.province_id,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    mpod.mpod_type,
    mpod.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    mpod.source_ref_type,
    mpod.source_ref_id,
    mpod.source_ref_description,
    mpod.status_remark,
    mpod.status_updated_by,
    fn_get_date(mpod.status_updated_on) AS status_updated_on,
    mpod.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    mpod.remarks,
    reg.country_name,
    mpod.origin_from,
    mpod.origin_ref_id,
    mpod.origin_ref_code,
    mpod.origin_ref_description,
    mpod.request_ref_id,
    mpod.requested_by,
    mpod.request_approved_by,
    mpod.subarea_id,
    mpod.area_id,
    mpod.dsa_id,
    mpod.csa_id,
    mpod.bom_sub_category,
    mpod.gis_design_id AS design_id,
    mpod.st_x,
    mpod.st_y,
    mpod.ne_id,
    mpod.prms_id,
    mpod.mzone_id,
    mpod.jc_id,
    mpod.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_mpod mpod
     JOIN province_boundary prov ON mpod.province_id = prov.id
     JOIN region_boundary reg ON mpod.region_id = reg.id
     JOIN user_master um ON mpod.created_by = um.user_id
     JOIN vendor_master vm ON mpod.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON mpod.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON mpod.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON mpod.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON mpod.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON mpod.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON mpod.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON mpod.purpose_id = purposem.system_id
     LEFT JOIN att_details_pod primarypod ON mpod.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON mpod.secondary_pod_system_id = secondarypod.system_id;





 DROP VIEW public.vw_att_details_fms_report;

CREATE OR REPLACE VIEW public.vw_att_details_fms_report
 AS
 SELECT fms.system_id,
    fms.network_id,
    fms.fms_name,
    round(fms.latitude::numeric, 6) AS latitude,
    round(fms.longitude::numeric, 6) AS longitude,
    fms.address,
    fms.specification,
    fms.category,
    fms.subcategory1,
    fms.subcategory2,
    fms.subcategory3,
    fms.item_code,
        CASE
            WHEN fms.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN fms.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN fms.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.status, fms.status) AS status,
    fms.pincode,
    prov.province_name,
    reg.region_name,
    fms.parent_network_id AS parent_code,
    ld.layer_title AS parent_type,
    vm.name AS vendor_name,
    fms.no_of_port,
    fms.no_of_input_port,
    fms.no_of_output_port,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    to_char(fms.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(fms.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    fms.region_id,
    fms.province_id,
    fms.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    fms.utilization,
    fms.acquire_from,
        CASE
            WHEN fms.utilization::text = 'L'::text THEN 'Low'::text
            WHEN fms.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN fms.utilization::text = 'H'::text THEN 'High'::text
            WHEN fms.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
    fms.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    fms.source_ref_type,
    fms.source_ref_id,
    fms.source_ref_description,
    fms.status_remark,
    fms.status_updated_by,
    to_char(fms.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    fms.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    fms.remarks,
    reg.country_name,
    fms.origin_from,
    fms.origin_ref_id,
    fms.origin_ref_code,
    fms.origin_ref_description,
    fms.request_ref_id,
    fms.requested_by,
    fms.request_approved_by,
    fms.subarea_id,
    fms.area_id,
    fms.dsa_id,
    fms.csa_id,
    fms.bom_sub_category,
    fms.gis_design_id AS design_id,
    fms.st_x,
    fms.st_y,
    fms.ne_id,
    fms.prms_id,
    fms.mzone_id,
    fms.jc_id,
    fms.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_fms fms
     JOIN province_boundary prov ON fms.province_id = prov.id
     JOIN region_boundary reg ON fms.region_id = reg.id
     JOIN user_master um ON fms.created_by = um.user_id
     JOIN vendor_master vm ON fms.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON fms.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON fms.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON fms.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON fms.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON fms.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON fms.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON fms.modified_by = um2.user_id
     LEFT JOIN att_details_pod primarypod ON fms.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON fms.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = fms.status::text
     LEFT JOIN layer_details ld ON ld.layer_name::text = fms.parent_entity_type::text;










 DROP VIEW public.vw_att_details_trench_report;

CREATE OR REPLACE VIEW public.vw_att_details_trench_report
 AS
 SELECT trench.system_id,
    trench.network_id,
    trench.trench_name,
    trench.a_location,
    trench.a_entity_type,
    trench.b_location,
    trench.b_entity_type,
    round(trench.trench_length::numeric, 2) AS trench_length,
    round(trench.trench_width::numeric, 2) AS trench_width,
    round(trench.trench_height::numeric, 2) AS trench_height,
    trench.customer_count,
    trench.utilization,
    trench.trench_type,
    trench.no_of_ducts,
    fn_get_network_status(trench.network_status) AS network_status,
    COALESCE(fn_get_entity_status(trench.status), trench.status) AS status,
    trench.pin_code,
    trench.specification,
    trench.category,
    trench.subcategory1,
    trench.subcategory2,
    trench.subcategory3,
    trench.item_code,
    trench.remarks,
    trench.mcgm_ward,
    trench.strata_type,
    trench.acquire_from,
    fn_get_date(trench.manufacture_year, false) AS manufacture_year,
    trench.surface_type,
    trench.parent_entity_type AS parent_type,
    trench.parent_network_id AS parent_code,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    fn_get_date(trench.created_on) AS created_on,
    fn_get_date(trench.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    COALESCE(pmapping.province_id, trench.province_id) AS province_id,
    COALESCE(pmapping.region_id, trench.region_id) AS region_id,
        CASE
            WHEN trench.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    prov.province_name AS city_name,
    reg.region_name,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    trench.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    trench.source_ref_type,
    trench.source_ref_id,
    trench.source_ref_description,
    trench.status_remark,
    trench.status_updated_by,
    fn_get_date(trench.status_updated_on) AS status_updated_on,
    trench.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    reg.country_name,
    trench.origin_from,
    trench.origin_ref_id,
    trench.origin_ref_code,
    trench.origin_ref_description,
    trench.request_ref_id,
    trench.requested_by,
    trench.request_approved_by,
    trench.subarea_id,
    trench.area_id,
    trench.dsa_id,
    trench.csa_id,
    trench.bom_sub_category,
    trench.trench_serving_type,
    trench.gis_design_id AS design_id,
    trench.ne_id,
    trench.prms_id,
    trench.jc_id,
    trench.mzone_id,
    trench.served_by_ring,
        CASE
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"P"'::text THEN 'Planned'::text
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"A"'::text THEN 'Asbuilt'::text
            WHEN ((adi.attribute_info -> 'curr_status'::text)::text) = '"D"'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS ticket_network_status,
    trench.a_latitude,
    trench.b_longitude,
    trench.a_region,
    trench.b_region,
    trench.a_city,
    trench.b_city,
    trench.actual_no_of_ducts,
    vm3.name as own_vendor_name
   FROM att_details_trench trench
     JOIN user_master um ON trench.created_by = um.user_id
     JOIN vendor_master vm ON trench.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON trench.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON trench.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON trench.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON trench.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON trench.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON trench.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON trench.modified_by = um2.user_id
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(trench.network_id::text) ~~ upper('NLD%'::text) THEN trench.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('TRENCH'::text)
            ELSE 1 = 2
        END
     JOIN province_boundary prov ON COALESCE(pmapping.province_id, trench.province_id) = prov.id
     JOIN region_boundary reg ON COALESCE(pmapping.region_id, trench.region_id) = reg.id
     LEFT JOIN att_details_pod primarypod ON trench.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON trench.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN att_details_edit_entity_info adi ON adi.source_ref_id::text = trench.source_ref_id::text AND adi.source_ref_type::text = trench.source_ref_type::text AND trench.system_id = adi.entity_system_id AND upper('TRENCH'::text) = upper(adi.entity_type::text) AND adi.entity_action_id = 13;











 DROP VIEW public.vw_att_details_pod_report;

CREATE OR REPLACE VIEW public.vw_att_details_pod_report
 AS
 SELECT pod.system_id,
    pod.network_id,
    pod.pod_name,
    round(pod.latitude::numeric, 6) AS latitude,
    round(pod.longitude::numeric, 6) AS longitude,
    pod.pincode,
    pod.address,
    pod.specification,
    pod.category,
    pod.subcategory1,
    pod.subcategory2,
    pod.subcategory3,
    pod.item_code,
        CASE
            WHEN pod.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN pod.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN pod.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, pod.status) AS status,
    pod.parent_network_id AS parent_code,
    pod.parent_entity_type AS parent_type,
    prov.province_name,
    reg.region_name,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    to_char(pod.created_on, 'DD-Mon-YY'::text) AS created_on,
    um.user_name AS created_by,
    to_char(pod.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pod.region_id,
    pod.province_id,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    pod.acquire_from,
    pod.pod_type,
    pod.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    pod.source_ref_type,
    pod.source_ref_id,
    pod.source_ref_description,
    pod.status_remark,
    pod.status_updated_by,
    to_char(pod.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    entattr.erp_code,
    entattr.erp_name,
    entattr.ef_customers,
    entattr.status AS locked,
    entattr.zone,
    to_char(entattr.rfs_date, 'DD-Mon-YY'::text) AS rfs_date,
    entattr.hub_maintained,
    entattr.hm_power_bb,
    entattr.hm_earthing_rating,
    entattr.hm_rack,
    entattr.hm_olt_bb,
    entattr.hm_fms,
    entattr.splicing_machine,
    entattr.optical_power_meter,
    entattr.otdr,
    entattr.laptop_with_giga_port,
    entattr.l3_updation_on_inms,
    pod.is_visible_on_map,
    pod.remarks,
    reg.country_name,
    pod.origin_from,
    pod.origin_ref_id,
    pod.origin_ref_code,
    pod.origin_ref_description,
    pod.request_ref_id,
    pod.requested_by,
    pod.request_approved_by,
    pod.subarea_id,
    pod.area_id,
    pod.dsa_id,
    pod.csa_id,
    pod.bom_sub_category,
    pod.gis_design_id AS design_id,
    pod.st_x,
    pod.st_y,
    pod.ne_id,
    pod.prms_id,
    pod.jc_id,
    pod.mzone_id,
    pod.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_pod pod
     JOIN province_boundary prov ON pod.province_id = prov.id
     JOIN region_boundary reg ON pod.region_id = reg.id
     JOIN user_master um ON pod.created_by = um.user_id
     JOIN vendor_master vm ON pod.vendor_id = vm.id
     LEFT JOIN entity_additional_attributes entattr ON entattr.system_id = pod.system_id AND upper(entattr.entity_type::text) = upper('POD'::text)
     LEFT JOIN vendor_master vm2 ON pod.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pod.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON pod.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON pod.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON pod.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON pod.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON pod.modified_by = um2.user_id
     LEFT JOIN entity_status_master es ON es.status::text = pod.status::text;







 DROP VIEW public.vw_att_details_ont_report;

CREATE OR REPLACE VIEW public.vw_att_details_ont_report
 AS
 SELECT ont.system_id,
    ont.network_id,
    ont.ont_name,
    round(ont.latitude::numeric, 6) AS latitude,
    round(ont.longitude::numeric, 6) AS longitude,
    ont.serial_no,
    ont.specification,
    ont.category,
    ont.subcategory1,
    ont.subcategory2,
    ont.subcategory3,
    ont.item_code,
    fn_get_network_status(ont.network_status) AS network_status,
    COALESCE(fn_get_entity_status(ont.status), ont.status) AS status,
    prov.province_name,
    reg.region_name,
    vm.name AS vendor_name,
    ont.no_of_input_port,
    ont.no_of_output_port,
    ont.parent_network_id AS parent_code,
    ont.parent_entity_type AS parent_type,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_date(ont.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(ont.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    ont.region_id,
    ont.province_id,
    ont.barcode,
    ont.acquire_from,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    ont.utilization,
    fn_utilization_get_full_text(ont.utilization) AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    ont.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    ont.source_ref_type,
    ont.source_ref_id,
    ont.source_ref_description,
    ont.status_remark,
    ont.status_updated_by,
    fn_get_date(ont.status_updated_on) AS status_updated_on,
    ont.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    ont.remarks,
    reg.country_name,
    ont.cpe_type,
    ont.origin_from,
    ont.origin_ref_id,
    ont.origin_ref_code,
    ont.origin_ref_description,
    ont.request_ref_id,
    ont.requested_by,
    ont.request_approved_by,
    ont.subarea_id,
    ont.area_id,
    ont.dsa_id,
    ont.csa_id,
    ont.bom_sub_category,
    ont.gis_design_id AS design_id,
    ont.st_x,
    ont.st_y,
    ont.ne_id,
    ont.prms_id,
    ont.mzone_id,
    ont.jc_id,
    ont.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_ont ont
     JOIN province_boundary prov ON ont.province_id = prov.id
     JOIN region_boundary reg ON ont.region_id = reg.id
     JOIN user_master um ON ont.created_by = um.user_id
     JOIN vendor_master vm ON ont.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON ont.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON ont.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON ont.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON ont.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON ont.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON ont.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON ont.modified_by = um2.user_id
     LEFT JOIN entity_notification_status entnotifystatus ON ont.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON ont.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON ont.secondary_pod_system_id = secondarypod.system_id;




 DROP VIEW public.vw_att_details_wallmount_report;

CREATE OR REPLACE VIEW public.vw_att_details_wallmount_report
 AS
 SELECT wallmount.system_id,
    wallmount.network_id,
    wallmount.wallmount_name,
    wallmount.wallmount_height,
    wallmount.wallmount_no,
    round(wallmount.latitude::numeric, 6) AS latitude,
    round(wallmount.longitude::numeric, 6) AS longitude,
    wallmount.address,
    wallmount.specification,
    wallmount.category,
    wallmount.subcategory1,
    wallmount.subcategory2,
    wallmount.subcategory3,
    wallmount.item_code,
    wallmount.accessibility,
    wallmount.acquire_from,
        CASE
            WHEN wallmount.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN wallmount.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN wallmount.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, wallmount.status) AS status,
    wallmount.parent_network_id AS parent_code,
    wallmount.parent_entity_type AS parent_type,
    prov.province_name,
    reg.region_name,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    to_char(wallmount.created_on, 'DD-Mon-YY'::text) AS created_on,
    um.user_name AS created_by,
    to_char(wallmount.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    wallmount.region_id,
    wallmount.province_id,
        CASE
            WHEN wallmount.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    wallmount.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    wallmount.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    wallmount.source_ref_type,
    wallmount.source_ref_id,
    wallmount.source_ref_description,
    wallmount.status_remark,
    wallmount.status_updated_by,
    to_char(wallmount.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    wallmount.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    wallmount.remarks,
    reg.country_name,
    wallmount.origin_from,
    wallmount.origin_ref_id,
    wallmount.origin_ref_code,
    wallmount.origin_ref_description,
    wallmount.request_ref_id,
    wallmount.requested_by,
    wallmount.request_approved_by,
    wallmount.subarea_id,
    wallmount.area_id,
    wallmount.dsa_id,
    wallmount.csa_id,
    wallmount.bom_sub_category,
    wallmount.gis_design_id AS design_id,
    wallmount.st_x,
    wallmount.st_y,
    wallmount.ne_id,
    wallmount.prms_id,
    wallmount.jc_id,
    wallmount.mzone_id,
    wallmount.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_wallmount wallmount
     JOIN province_boundary prov ON wallmount.province_id = prov.id
     JOIN region_boundary reg ON wallmount.region_id = reg.id
     JOIN user_master um ON wallmount.created_by = um.user_id
     JOIN vendor_master vm ON wallmount.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON wallmount.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON wallmount.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON wallmount.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON wallmount.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON wallmount.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON wallmount.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON wallmount.modified_by = um2.user_id
     LEFT JOIN att_details_pod primarypod ON wallmount.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON wallmount.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = wallmount.status::text;











 DROP VIEW public.vw_att_details_bdb_report;

CREATE OR REPLACE VIEW public.vw_att_details_bdb_report
 AS
 SELECT bdb.system_id,
    bdb.network_id,
    bdb.bdb_name,
    round(bdb.latitude::numeric, 6) AS latitude,
    round(bdb.longitude::numeric, 6) AS longitude,
    bdb.pincode,
    bdb.address,
    bdb.specification,
    bdb.category,
    bdb.subcategory1,
    bdb.subcategory2,
    bdb.subcategory3,
    bdb.item_code,
        CASE
            WHEN bdb.is_servingdb THEN 'YES'::text
            ELSE 'NO'::text
        END AS is_servingdb,
    COALESCE(es.description, bdb.status) AS status,
        CASE
            WHEN bdb.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN bdb.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN bdb.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    bdb.parent_network_id AS parent_code,
    bdb.parent_entity_type AS parent_type,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    to_char(bdb.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(bdb.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um.user_name AS created_by,
    um2.user_name AS modified_by,
    bdb.no_of_input_port,
    bdb.no_of_output_port,
    bdb.no_of_port,
    bdb.entity_category,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    bdb.region_id,
    bdb.province_id,
    bdb.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    bdb.utilization,
        CASE
            WHEN bdb.utilization::text = 'L'::text THEN 'Low'::text
            WHEN bdb.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN bdb.utilization::text = 'H'::text THEN 'High'::text
            WHEN bdb.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    bdb.acquire_from,
    bdb.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    bdb.source_ref_type,
    bdb.source_ref_id,
    bdb.source_ref_description,
    bdb.status_remark,
    bdb.status_updated_by,
    to_char(bdb.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    bdb.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    bdb.remarks,
    reg.country_name,
    bdb.origin_from,
    bdb.origin_ref_id,
    bdb.origin_ref_code,
    bdb.origin_ref_description,
    bdb.request_ref_id,
    bdb.requested_by,
    bdb.request_approved_by,
    bdb.subarea_id,
    bdb.area_id,
    bdb.dsa_id,
    bdb.csa_id,
    bdb.bom_sub_category,
    bdb.gis_design_id AS design_id,
    bdb.st_x,
    bdb.st_y,
    bdb.ne_id,
    bdb.prms_id,
    bdb.jc_id,
    bdb.mzone_id,
    bdb.served_by_ring,
    oi.dropdown,
    oi.dropdown22,
    oi.name_tab,
    oi.record_system_id,
    oi.singleline,
    oi.singlesinglesingle,
    vm3.name as own_vendor_name
   FROM att_details_bdb bdb
     JOIN province_boundary prov ON bdb.province_id = prov.id
     JOIN region_boundary reg ON bdb.region_id = reg.id
     JOIN user_master um ON bdb.created_by = um.user_id
     LEFT JOIN user_master um2 ON bdb.modified_by = um2.user_id
     JOIN vendor_master vm ON bdb.vendor_id = vm.id
     LEFT JOIN vendor_master vm3 ON bdb.own_vendor_id::integer = vm3.id
     LEFT JOIN vendor_master vm2 ON bdb.third_party_vendor_id = vm2.id
     LEFT JOIN att_details_project_master pm ON bdb.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON bdb.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON bdb.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON bdb.purpose_id = purposem.system_id
     LEFT JOIN entity_notification_status entnotifystatus ON bdb.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON bdb.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON bdb.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = bdb.status::text
     LEFT JOIN vw_att_details_bdb_oi oi ON oi.record_system_id = bdb.system_id::text;





 DROP VIEW public.vw_att_details_microduct_report;

CREATE OR REPLACE VIEW public.vw_att_details_microduct_report
 AS
 SELECT microduct.system_id,
    microduct.network_id,
    microduct.microduct_name AS network_name,
    round(microduct.calculated_length::numeric, 2) AS calculated_length,
    round(microduct.manual_length::numeric, 2) AS manual_length,
    microduct.a_location,
    microduct.b_location,
    microduct.a_entity_type,
    microduct.b_entity_type,
    fn_get_network_status(microduct.network_status) AS network_status,
    COALESCE(fn_get_entity_status(microduct.status), microduct.status) AS status,
    microduct.pin_code,
    microduct.utilization,
    microduct.no_of_cables,
    microduct.offset_value,
    microduct.construction,
    microduct.activation,
    microduct.accessibility,
    microduct.specification,
    microduct.category,
    round(microduct.inner_dimension::numeric, 2) AS inner_dimension,
    round(microduct.outer_dimension::numeric, 2) AS outer_dimension,
    microduct.microduct_type,
    microduct.color_code,
    microduct.subcategory1,
    microduct.subcategory2,
    microduct.subcategory3,
    microduct.item_code,
    microduct.remarks,
    microduct.parent_entity_type AS parent_type,
    microduct.parent_network_id AS parent_code,
    microduct.acquire_from,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_date(microduct.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(microduct.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    COALESCE(pmapping.province_id, microduct.province_id) AS province_id,
    COALESCE(pmapping.region_id, microduct.region_id) AS region_id,
        CASE
            WHEN microduct.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    reg.region_name,
    prov.province_name,
    microduct.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    microduct.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    microduct.source_ref_type,
    microduct.source_ref_id,
    microduct.source_ref_description,
    microduct.status_remark,
    microduct.status_updated_by,
    fn_get_date(microduct.status_updated_on) AS status_updated_on,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    reg.country_name,
    microduct.internal_diameter,
    microduct.external_diameter,
    microduct.material_type,
    microduct.origin_from,
    microduct.origin_ref_id,
    microduct.origin_ref_code,
    microduct.origin_ref_description,
    microduct.request_ref_id,
    microduct.requested_by,
    microduct.request_approved_by,
    microduct.subarea_id,
    microduct.area_id,
    microduct.dsa_id,
    microduct.csa_id,
    microduct.bom_sub_category,
    microduct.gis_design_id AS design_id,
    itm.no_of_ways,
    microduct.ne_id,
    microduct.prms_id,
    microduct.mzone_id,
    microduct.jc_id,
    microduct.served_by_ring,
    vm3.name as own_vendor_name
   FROM att_details_microduct microduct
     JOIN user_master um ON microduct.created_by = um.user_id
     JOIN vendor_master vm ON microduct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON microduct.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON microduct.own_vendor_id::integer = vm3.id
     LEFT JOIN user_master um2 ON microduct.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON microduct.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON microduct.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON microduct.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON microduct.purpose_id = purposem.system_id
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(microduct.network_id::text) ~~ upper('NLD%'::text) THEN microduct.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('MICRODUCT'::text)
            ELSE 1 = 2
        END
     JOIN province_boundary prov ON COALESCE(pmapping.province_id, microduct.province_id) = prov.id
     JOIN region_boundary reg ON COALESCE(pmapping.region_id, microduct.region_id) = reg.id
     LEFT JOIN att_details_pod primarypod ON microduct.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON microduct.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN item_template_microduct itm ON itm.vendor_id = microduct.vendor_id;





 DROP VIEW public.vw_att_details_pole_report;

CREATE OR REPLACE VIEW public.vw_att_details_pole_report
 AS
 SELECT pole.system_id,
    pole.network_id,
    pole.pole_name,
    pole.pole_type,
    pole.pole_height,
    pole.pole_no,
    round(pole.latitude::numeric, 6) AS latitude,
    round(pole.longitude::numeric, 6) AS longitude,
    pole.address,
    pole.acquire_from,
    pole.specification,
    pole.category,
    pole.subcategory1,
    pole.subcategory2,
    pole.subcategory3,
    pole.item_code,
        CASE
            WHEN pole.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
        CASE
            WHEN pole.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN pole.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN pole.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, pole.status) AS status,
    prov.province_name,
    reg.region_name,
    pole.parent_network_id AS parent_code,
    pole.parent_entity_type AS parent_type,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    to_char(pole.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(pole.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pole.region_id,
    pole.province_id,
    pole.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    pole.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    pole.source_ref_type,
    pole.source_ref_id,
    pole.source_ref_description,
    pole.status_remark,
    pole.status_updated_by,
    to_char(pole.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    pole.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    pole.remarks,
    reg.country_name,
    pole.origin_from,
    pole.origin_ref_id,
    pole.origin_ref_code,
    pole.origin_ref_description,
    pole.request_ref_id,
    pole.requested_by,
    pole.request_approved_by,
    pole.subarea_id,
    pole.area_id,
    pole.dsa_id,
    pole.csa_id,
    pole.bom_sub_category,
    pole.gis_design_id AS design_id,
    pole.st_x,
    pole.st_y,
    pole.ne_id,
    pole.prms_id,
    pole.jc_id,
    pole.mzone_id,
    pole.served_by_ring,
    oi.aaaa,
    oi.decimal_test,
    oi.record_system_id,
    vm3.name as own_vendor_name
   FROM att_details_pole pole
     JOIN province_boundary prov ON pole.province_id = prov.id
     JOIN region_boundary reg ON pole.region_id = reg.id
     JOIN user_master um ON pole.created_by = um.user_id
     JOIN vendor_master vm ON pole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pole.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pole.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON pole.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON pole.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON pole.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON pole.purpose_id = purposem.system_id
     LEFT JOIN att_details_pod primarypod ON pole.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON pole.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN user_master um2 ON pole.modified_by = um2.user_id
     LEFT JOIN entity_status_master es ON es.status::text = pole.status::text
     LEFT JOIN vw_att_details_pole_oi oi ON oi.record_system_id = pole.system_id::text;













