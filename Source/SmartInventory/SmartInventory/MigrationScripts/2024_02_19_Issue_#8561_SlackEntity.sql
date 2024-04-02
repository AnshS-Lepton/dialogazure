CREATE TABLE public.att_details_slack (
	system_id serial,
	network_id varchar(500) NULL,
	slack_length float8 NULL,
	associated_system_id int4 NULL,
	associated_network_id varchar(200) NULL,
	associated_entity_type varchar(100) NULL,
	duct_system_id int4 NULL,
	created_by int4 NULL,
	created_on timestamp NULL DEFAULT now(),
	modified_by int4 NULL,
	modified_on timestamp NULL,
	network_status varchar(25) NOT NULL DEFAULT 'P'::character varying,
	parent_system_id int4 NULL,
	sequence_id int4 NULL,
	province_id int4 NULL,
	region_id int4 NULL,
	parent_network_id varchar(100) NULL,
	parent_entity_type varchar(100) NULL,
	status varchar(10) NULL DEFAULT 'A'::character varying,
	status_remark varchar(2000) NULL,
	status_updated_by int4 NULL,
	status_updated_on timestamp NULL,
	is_visible_on_map bool NULL DEFAULT true,
	source_ref_type varchar(100) NULL,
	source_ref_id varchar NULL,
	source_ref_description varchar NULL,
	latitude float8 NULL,
	longitude float8 NULL,
	is_new_entity bool NOT NULL DEFAULT false,
	project_id int4 NULL DEFAULT 0,
	planning_id int4 NULL DEFAULT 0,
	purpose_id int4 NULL DEFAULT 0,
	workorder_id int4 NULL DEFAULT 0,
	origin_from varchar NULL,
	origin_ref_id varchar NULL,
	origin_ref_code varchar NULL,
	origin_ref_description varchar NULL,
	request_ref_id varchar NULL,
	requested_by varchar NULL,
	request_approved_by varchar NULL,
	subarea_id varchar NULL,
	area_id varchar NULL,
	dsa_id varchar NULL,
	csa_id varchar NULL,
	gis_design_id varchar(100) NULL,
	csa_system_id int4 NULL,
	dsa_system_id int4 NULL,
	area_system_id int4 NULL,
	subarea_system_id int4 NULL,
	codification_sequence int4 NULL,
	target_ref_code varchar NULL,
	target_ref_description varchar NULL,
	target_ref_id int4 NULL,
	st_x float8 NULL,
	st_y float8 NULL,
	ne_id varchar NULL,
	prms_id varchar NULL,
	jc_id varchar NULL,
	mzone_id varchar NULL,
	served_by_ring varchar NULL
);

CREATE INDEX att_details_slack_network_id_idx ON public.att_details_slack USING btree (network_id);
CREATE INDEX att_details_slack_province_network_status_idx ON public.att_details_slack USING btree (region_id, province_id, network_status);
CREATE INDEX att_details_slack_region_province_idx ON public.att_details_slack USING btree (region_id, province_id);
CREATE INDEX att_details_slack_system_id_idx ON public.att_details_slack USING btree (system_id);

CREATE TABLE public.audit_att_details_slack (
	audit_id serial4 NOT NULL,
	system_id int4 NULL,
	network_id varchar(500) NULL,
	slack_length int4 NULL,
	associated_system_id int4 NULL,
	associated_network_id varchar(200) NULL,
	associated_entity_type varchar(100) NULL,
	duct_system_id int4 NULL,
	created_by int4 NULL,
	created_on timestamp NULL DEFAULT now(),
	modified_by int4 NULL,
	modified_on timestamp NULL,
	network_status varchar(25) NOT NULL DEFAULT 'P'::character varying,
	"action" varchar(10) NULL,
	parent_system_id int4 NULL,
	sequence_id int4 NULL,
	province_id int4 NULL,
	region_id int4 NULL,
	parent_network_id varchar(100) NULL,
	parent_entity_type varchar(100) NULL,
	status_remark varchar(2000) NULL,
	status_updated_by int4 NULL,
	status_updated_on timestamp NULL,
	is_visible_on_map bool NULL DEFAULT true,
	source_ref_type varchar(100) NULL,
	source_ref_id varchar NULL,
	source_ref_description varchar NULL,
	latitude float8 NULL,
	longitude float8 NULL,
	project_id int4 NULL DEFAULT 0,
	planning_id int4 NULL DEFAULT 0,
	purpose_id int4 NULL DEFAULT 0,
	workorder_id int4 NULL DEFAULT 0,
	ticket_id int4 NULL,
	origin_from varchar NULL,
	origin_ref_id varchar NULL,
	origin_ref_code varchar NULL,
	origin_ref_description varchar NULL,
	request_ref_id varchar NULL,
	requested_by varchar NULL,
	request_approved_by varchar NULL,
	subarea_id varchar NULL,
	area_id varchar NULL,
	dsa_id varchar NULL,
	csa_id varchar NULL,
	gis_design_id varchar(100) NULL,
	csa_system_id int4 NULL,
	dsa_system_id int4 NULL,
	area_system_id int4 NULL,
	subarea_system_id int4 NULL,
	target_ref_code varchar NULL,
	target_ref_description varchar NULL,
	target_ref_id int4 NULL,
	st_x float8 NULL,
	st_y float8 NULL,
	ne_id varchar NULL,
	prms_id varchar NULL,
	jc_id varchar NULL,
	mzone_id varchar NULL,
	served_by_ring varchar NULL
);


alter table att_details_duct add column slack_count integer;
alter table att_details_duct add column slack_length integer;
alter table att_details_duct add column total_slack_length integer;
alter table att_details_duct add column total_slack_count integer;

alter table audit_att_details_duct add column slack_count integer;
alter table audit_att_details_duct add column slack_length integer;
alter table audit_att_details_duct add column total_slack_length integer;
alter table audit_att_details_duct add column total_slack_count integer;

INSERT INTO public.layer_details ( layer_name, isvisible, minzoomlevel, maxzoomlevel, minboundvalue, maxboundvalue, parent_layer_id, layer_abbr, map_abbr, layer_title, layer_form_url, layer_seq, is_network_entity, is_visible_in_ne_library, is_template_required, network_id_type, geom_type, template_form_url, layer_table, network_code_seperator, save_entity_url, is_direct_save, layer_view, is_clone, is_isp_layer, is_shaft_element, is_osp_layer, unit_type, unit_input_type, is_multi_clone, is_mobile_layer, is_visible_in_mobile_lib, is_vendor_spec_required, is_splicer, report_view_name, is_report_enable, is_networktype_required, is_isp_splicer, is_label_change_allowed, layer_network_group, is_multi_association, history_view_name, is_history_enabled, is_info_enabled, is_cpe_entity, is_virtual_port_allowed, is_reference_allowed, is_isp_child_layer, map_layer_seq, layer_template_table, is_isp_parent_layer, is_floor_element, is_osp_layer_freezed_in_library, is_isp_layer_freezed_in_library, is_other_wcr_layer, is_barcode_enabled, barcode_column, is_at_enabled, is_maintainence_charges_enabled, is_feasibility_layer, feasibility_network_group, is_row_association_enabled, is_logicalview_enabled, is_site_enabled, is_networkcode_change_enabled, is_middleware_entity, is_lmc_enabled, is_utilization_enabled, is_layer_for_rights_permission, is_data_upload_enabled, is_fault_entity, data_upload_max_count, data_upload_table, is_moredetails_enable, is_project_spec_allowed, is_fiber_link_enabled, is_visible_on_mobile_map, is_loop_allowed, is_pod_association_allowed, is_remark_required_from_mobile, is_trayinfo_enabled, is_split_allowed, is_association_enabled, is_association_mandatory, is_auto_plan_end_point, is_network_ticket_entity, is_mobile_isp_layer, is_tp_layer, is_offline_allowed, specification_dropdown_type, is_entity_along_direction, is_vsat_enabled, other_info_view, other_info_view_audit, audit_table_name, is_bomboq_enabled, is_dynamic_control_enable, is_dynamic_enabled, category_name, type_name, ne_class_name, gis_class_name, layer_display_abbr, is_unique_column_name, unique_column_name, vc_view_name, delta_metadata_table, is_vector_layer_implemented) VALUES('Slack', true, 12, 21, 4, 21, 0, 'SLK', 'SLK', 'Slack', '/AddSlack', 7, true, true, false, 'A', 'Point', '', 'att_details_Slack', '-', '/SaveSlack', false, 'vw_att_details_Slack', false, false, false, true, '', '', false, true, true, false, false, 'vw_att_details_Slack_report', true, true, false, true, 'Layers', true, 'vw_att_details_Slack_audit', true, true, false, true, false, false, NULL, NULL, false, false, false, false, false, false, NULL, false, false, false, NULL, false, false, false, true, false, false, false, true, false, false, 6000, 'temp_du_Slack', false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, NULL, false, false, NULL, NULL, 'audit_att_details_Slack', true, false, true, NULL, NULL, NULL, NULL, 'SLK', false, NULL, NULL, 'Slack_delta_metadata', NULL);


INSERT INTO public.layer_mapping (layer_id, parent_layer_id, parent_sequence, is_enable_inside_parent_info, is_used_for_network_id, network_code_format, is_default_code_format, is_default_parent) 
VALUES((Select layer_id from layer_details where upper(layer_name)='SLACK'), 0, 1, false, true, 'SLKnnnnnn', true, false);

INSERT INTO public.layer_action_mapping ( layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id) 
VALUES( (Select layer_id from layer_details where upper(layer_name)='SLACK'), 'Detail', true, 4, 'Network Element Detail', true, false, true, 'M', 0, (select id from module_master where module_name ='Entity Network Element Details'), true, true, (select id from module_master where module_name ='Entity Network Element Details'), NULL, false, 0);
INSERT INTO public.layer_action_mapping (layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id) 
VALUES( (Select layer_id from layer_details where upper(layer_name)='SLACK'), 'History', true, 3, 'History', true, false, true, NULL, 0, 0, false, true, 0, 'SI_OSP_GBL_NET_FRM_339', true, 0);
INSERT INTO public.layer_action_mapping ( layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id) 
VALUES( (Select layer_id from layer_details where upper(layer_name)='SLACK'), 'Print', true, 11, 'Print', true, false, true, '', 0, 0, false, true, 0, 'SI_OSP_GBL_NET_GBL_156', false, 408);
INSERT INTO public.layer_action_mapping ( layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id) 
VALUES( (Select layer_id from layer_details where upper(layer_name)='SLACK'), 'BufferOperations', false, 10, 'Buffer Operations', true, false, true, '', 0, 0, false, true, 0, '', false, 0);
INSERT INTO public.layer_action_mapping ( layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id) 
VALUES( (Select layer_id from layer_details where upper(layer_name)='SLACK'), 'Export', true, 1, 'Export', true, false, true, NULL, 0, 0, false, true, 0, 'GBL_GBL_GBL_GBL_GBL_010', true, 0);
INSERT INTO public.layer_action_mapping ( layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id) 
VALUES( (Select layer_id from layer_details where upper(layer_name)='SLACK'), 'Delete', true, 5, 'Delete', true, false, true, 'D', 0, 0, true, true, 0, 'SI_GBL_GBL_GBL_GBL_002', false, 0);
INSERT INTO public.layer_action_mapping ( layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id) 
VALUES( (Select layer_id from layer_details where upper(layer_name)='SLACK'), 'Layout', true, 12, 'Layout', true, false, true, 'M', 0, (select id from module_master where module_name ='Map Layout'), false, true, 0, '', false, 408);

update layer_details set is_loop_allowed=true where layer_id=(select layer_id from layer_details where layer_name='Duct');

INSERT INTO public.layer_icon_master ( layer_id, category, subcategory, icon_name, icon_path, network_status, status, created_by, created_on, modified_by, modified_on, is_virtual, landbase_layer_id) VALUES( (select layer_id from layer_details where layer_name='Slack'), '', NULL, 'slack.png', 'icons/Other/slack.png', 'O', true, 1, '2022-02-28 13:36:53.046', NULL, NULL, false, NULL);


INSERT INTO public.display_name_settings ( layer_id, display_column_name, created_by, created_on, modified_by, modified_on, status, display_network_name, default_display_column_name)VALUES( (select layer_id from layer_details where layer_name='Slack'), 'network_id', 1, '2021-06-15 13:52:29.936', NULL, NULL, 'Completed', 'network_id', 'network_id');


CREATE OR REPLACE FUNCTION public.fn_get_slack_details(p_longitude double precision, p_latitude double precision, p_associated_system_id integer, p_associated_system_type character varying, structure_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 

DECLARE

    buffer INTEGER;
    v_loop_allowed BOOLEAN;
    

BEGIN

 EXECUTE 'select value from global_settings where key=''LoopBuffer''' into buffer;
 select is_loop_allowed from layer_details where upper(layer_name)= upper(''||p_associated_system_type||'') into v_loop_allowed;
 

 IF(structure_id = 0) THEN

RETURN QUERY select row_to_json(row) from (select lm.display_name||'-('||coalesce(lm.network_status,'')||')':: character varying as duct_id,
round((duct.calculated_length)::numeric, 2) AS duct_calculated_length,
round((duct.calculated_length- duct.total_slack_length)::numeric, 2) as available_calculated_length,
slack.associated_entity_type,duct.duct_type, duct.duct_name, case when slack.system_id IS NULL then 0 else slack.system_id end as system_id , 
slack.network_id,round(slack.slack_length::numeric, 2) AS slack_length,duct.system_id as duct_system_id,
case when slack.associated_system_id IS NULL then 0 else slack.associated_system_id end as associated_system_id,
case when slack.latitude IS NULL then 0 else slack.latitude end as latitude,
case when slack.longitude IS NULL then 0 else slack.longitude end as longitude,case when duct.total_slack_length IS NULL then 0 else duct.total_slack_length end as total_slack_length,
case when duct.total_slack_count IS NULL then 0 else duct.total_slack_count end as total_slack_count,
slack.associated_network_id,slack.associated_entity_type, slack.created_by, slack.created_on,slack.modified_by, 
slack.modified_on,duct.network_status,case when slack.region_id IS NULL then 0 else slack.region_id end as region_id ,case when slack.province_id IS NULL then 0 else slack.province_id end as province_id,slack.parent_network_id,slack.parent_entity_type,
case when slack.sequence_id IS NULL then 0 else slack.sequence_id end as sequence_id from att_details_duct duct
inner join line_master lm ON lm.system_id = duct.system_id  and upper(lm.entity_type)='DUCT' 
Left Join att_Details_slack slack on slack.duct_system_id = duct.system_id and slack.associated_system_id = p_associated_system_id
WHERE case when v_loop_allowed =false OR upper(p_associated_system_type) = 'DUCT' then 1=1  else 1=1 end and
st_intersects(ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),buffer),lm.SP_GEOMETRY)
) row;

END IF;

--RAISE INFO '%',QUERY;                  

END; 
$function$
;


CREATE OR REPLACE FUNCTION public.fn_save_slack_details(p_slacks text)
 RETURNS TABLE(status character varying, message character varying, systemid integer)
 LANGUAGE plpgsql
AS $function$
 
DECLARE  
  
 v_geometry geometry;
 v_longitude double precision;
 v_latitude double precision; 
 arow record;
 v_snapped_geom geometry;
   
BEGIN

CREATE temp TABLE temp_slack
(  
  system_id serial,
  network_id character varying(500),
  slack_length double precision,
  associated_system_id integer,
  associated_network_id character varying(200),
  associated_entity_type character varying(100),
  duct_system_id integer,
  created_by integer,
  modified_by integer,
  network_status character varying(25) NOT NULL DEFAULT 'P'::character varying,
  parent_system_id integer,
  sequence_id integer,
  province_id integer,
  region_id integer,
  parent_network_id character varying(100),
  parent_entity_type character varying(100),
  status_remark character varying(2000),
  status_updated_by integer,
  source_ref_type character varying(100),
  source_ref_id character varying,
  source_ref_description character varying,
  latitude double precision,
  longitude double precision,
  origin_from character varying,
  origin_ref_id character varying,
  origin_ref_code character varying,
  origin_ref_description character varying,
  request_ref_id character varying,
  requested_by character varying,
  request_approved_by character varying,
  subarea_id character varying,
  area_id character varying,
  dsa_id character varying,
  csa_id character varying,
  ne_id character varying,
  prms_id character varying,
  jc_id character varying,
  mzone_id character varying
) ON COMMIT DROP ;  


FOR arow IN  
select system_id,network_id ,slack_length ,associated_system_id ,associated_network_id ,associated_entity_type,duct_system_id ,created_by ,modified_by ,network_status,parent_system_id ,sequence_id ,province_id ,region_id ,parent_network_id ,parent_entity_type ,status_remark ,status_updated_by ,source_ref_type ,source_ref_id ,source_ref_description ,latitude ,longitude ,origin_from ,origin_ref_id ,origin_ref_code ,origin_ref_description ,request_ref_id ,requested_by ,request_approved_by ,subarea_id ,area_id ,dsa_id ,csa_id ,ne_id ,prms_id ,jc_id ,mzone_id from json_populate_record(null::temp_slack,replace(p_Slacks,'\','')::json)

LOOP

	
IF(arow.system_id = 0  AND arow.slack_length > 0) THEN
		select ST_ClosestPoint(sp_geometry,ST_GeomFromText('POINT('||arow.longitude||' '||arow.latitude||')',4326)) into v_geometry from  line_master  where  system_id=arow.associated_system_id and upper(entity_type)=upper('DUCT') ;
		
		v_longitude=st_x(v_geometry);
		v_latitude=st_y(v_geometry);
		

	 INSERT INTO att_details_slack(network_id,slack_length,associated_system_id,associated_network_id,associated_entity_type,
	duct_system_id,network_status,parent_system_id,sequence_id,province_id,region_id,parent_network_id,parent_entity_type,status_remark,
	source_ref_type,source_ref_id,source_ref_description,created_by,status_updated_by,latitude,longitude,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,ne_id,prms_id,jc_id,mzone_id)
	 values( arow.network_id,arow.slack_length,arow.associated_system_id,arow.associated_network_id,arow.associated_entity_type,
	arow.duct_system_id,coalesce(arow.network_status,'P'),arow.parent_system_id,arow.sequence_id,arow.province_id,
	arow.region_id,arow.parent_network_id,arow.parent_entity_type,arow.status_remark,arow.source_ref_type,arow.source_ref_id,arow.source_ref_description,
	arow.created_by,arow.status_updated_by,v_latitude,v_longitude,arow.origin_from,arow.origin_ref_id,arow.origin_ref_code,arow.origin_ref_description,
	arow.request_ref_id,arow.requested_by,arow.request_approved_by,arow.subarea_id,arow.area_id,arow.dsa_id,arow.csa_id,arow.ne_id,arow.prms_id,arow.jc_id,arow.mzone_id) returning system_id into arow.system_id;

--PointMaster entry--
	INSERT INTO point_master(system_id, entity_type, longitude, latitude, approval_flag, sp_geometry,approval_date, creator_remark, approver_remark, created_by,common_name, db_flag,network_status,display_name)
	VALUES (arow.system_id,'Slack', arow.longitude,arow.latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),now(),'NA','NA',arow.created_by,arow.network_id,0,coalesce(arow.network_status,'P'),fn_get_display_name(arow.system_id,'Slack'));

-- Snapping cable to loop
SELECT ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01) into v_snapped_geom
  FROM (
 SELECT SP_GEOMETRY AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = arow.system_id and lower(entity_type)=lower('Slack')
  ) AS point,
   LINE_MASTER AS LM  WHERE LM.system_id = arow.duct_system_id and lower(entity_type)=lower('duct');

update LINE_MASTER set SP_GEOMETRY=v_snapped_geom  WHERE system_id = arow.duct_system_id and lower(entity_type)=lower('duct');

return query select 'True'::character varying as status, 'Slack Inserted Sucessfully'::character varying as message,arow.system_id as systemId ;

ELSE

UPDATE att_details_slack set slack_length= arow.slack_length ,associated_system_id=arow.associated_system_id ,associated_network_id=arow.associated_network_id,associated_entity_type=arow.associated_entity_type,duct_system_id=arow.duct_system_id,modified_by=arow.modified_by,modified_on=now(),ne_id=arow.ne_id,prms_id=arow.prms_id,jc_id=arow.jc_id,mzone_id=arow.mzone_id where system_id=arow.system_id;

return query select 'True'::character varying as status, 'Slack Updated Sucessfully'::character varying as message,arow.system_id as systemId;

END IF;
END LOOP;
	

END
$function$
;


CREATE OR REPLACE FUNCTION public.fn_get_nearbyducts(p_longitude double precision, p_latitude double precision, p_buffer integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$


BEGIN

RETURN QUERY select row_to_json(row) from (select lm.display_name||'-('||coalesce(lm.network_status,'')||')':: character varying as duct_id,duct.network_id,
duct.duct_type, duct.duct_name,duct.system_id as duct_system_id,fn_get_date(duct.created_on, true) AS created_on , duct.calculated_length,duct.total_slack_length,duct.total_slack_count,
(duct.calculated_length - duct.total_slack_length) as available_duct_length  from att_details_duct duct
inner join line_master lm ON lm.system_id = duct.system_id  and upper(lm.entity_type)='DUCT' 
WHERE st_intersects(ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer),lm.SP_GEOMETRY)) row;


--RAISE INFO '%',QUERY;                  

END; 
$function$;

CREATE OR REPLACE FUNCTION public.fn_get_slack_details_for_duct(p_duct_system_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
    buffer INTEGER;
BEGIN

RETURN QUERY select row_to_json(row) from (
select slack.system_id,slack.network_id, slack.associated_entity_type, concat (slack.associated_network_id,' ( 

',ass_entity_name.associated_entity_name,' ) ') as associated_network_id,slack.slack_length,slack.duct_system_id as duct_system_id, 

slack.created_by,slack.created_on,slack.modified_by, slack.modified_on from 
(
select manhole.manhole_name AS associated_entity_name, manhole.system_id, manhole.network_id from att_details_manhole as manhole
union 
select pole.pole_name AS associated_entity_name, pole.system_id, pole.network_id from att_details_pole as pole
union 
select tree.tree_name AS associated_entity_name, tree.system_id, tree.network_id from att_details_tree as tree
union 
select wallmount.wallmount_name AS associated_entity_name, wallmount.system_id, wallmount.network_id from att_details_wallmount as wallmount
union 
select bdb.bdb_name AS associated_entity_name, bdb.system_id, bdb.network_id from att_details_bdb as bdb
union 
select fdb.fdb_name AS associated_entity_name, fdb.system_id, fdb.network_id from isp_fdb_info as fdb
union 
select adb.adb_name AS associated_entity_name, adb.system_id, adb.network_id from att_details_adb as adb
union 
select cdb.cdb_name AS associated_entity_name, cdb.system_id, cdb.network_id from att_details_cdb as cdb
union 
select sc.spliceclosure_name AS associated_entity_name, sc.system_id, sc.network_id from att_details_spliceclosure as sc
union 
select fms.fms_name AS associated_entity_name, fms.system_id, fms.network_id from att_details_fms as fms
union 
select ont.ont_name AS associated_entity_name, ont.system_id, ont.network_id from att_details_ont as ont
union 
select htb.htb_name AS associated_entity_name, htb.system_id, htb.network_id from isp_htb_info as htb
union 
select duct.duct_name AS associated_entity_name, duct.system_id, duct.network_id from att_details_duct as duct
) as ass_entity_name
Join att_details_slack as slack
on ass_entity_name.system_id = slack.associated_system_id and ass_entity_name.network_id = slack.associated_network_id
where slack.duct_system_id = p_duct_system_id
union
select slack.system_id,slack.network_id, 'NA' as associated_entity_type, 'NA' as associated_network_id,slack.slack_length,slack.slack_system_id as slack_system_id, 
slack.created_by,slack.created_on,slack.modified_by, slack.modified_on from att_details_slack slack where 
slack.slack_system_id=p_slack_system_id and  associated_network_id is null and associated_entity_type is null
) row;

 
--RAISE INFO '%',myst;		
END; $function$
;

CREATE OR REPLACE FUNCTION public.fn_trg_audit_att_details_slack()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$
DECLARE
 _value integer;
 _ignorecol character varying;  
_ignorecol1 character varying;
BEGIN

IF (TG_OP = 'INSERT' ) THEN  
         INSERT INTO public.audit_att_details_slack(
            system_id, network_id, slack_length, associated_system_id, 
            associated_network_id, associated_entity_type, duct_system_id, 
            created_by, created_on, modified_by, modified_on, network_status, 
            action,status_remark,status_updated_by,status_updated_on,is_visible_on_map,source_ref_type,source_ref_id,source_ref_description,latitude,longitude,project_id,planning_id,purpose_id,workorder_id,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,gis_design_id,province_id,region_id)
         select new.system_id, new.network_id, new.slack_length, new.associated_system_id, 
            new.associated_network_id, new.associated_entity_type, new.duct_system_id, 
            new.created_by, new.created_on, new.modified_by, new.modified_on, new.network_status, 'I' as action,new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.latitude,new.longitude,new.project_id,new.planning_id,new.purpose_id,new.workorder_id,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.gis_design_id,new.province_id,new.region_id from att_details_slack where system_id=new.system_id;

END IF;
IF (TG_OP = 'UPDATE' ) THEN  
_ignorecol := 'modified_on';
_ignorecol1 := 'modified_by';
select fn_check_history_record(OLD, NEW,_ignorecol,_ignorecol1) into _value;
if(_value = 1)then

           INSERT INTO public.audit_att_details_slack(
            system_id, network_id, slack_length, associated_system_id, 
            associated_network_id, associated_entity_type, duct_system_id, 
            created_by, created_on, modified_by, modified_on, network_status, 
            action,status_remark,status_updated_by,status_updated_on,is_visible_on_map,source_ref_type,source_ref_id,source_ref_description,latitude,longitude,project_id,planning_id,purpose_id,workorder_id,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,gis_design_id,province_id,region_id)
            select new.system_id, new.network_id, new.slack_length, new.associated_system_id, 
            new.associated_network_id, new.associated_entity_type, new.duct_system_id, 
            new.created_by, new.created_on, new.modified_by, new.modified_on, new.network_status, 'U' as action ,new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.latitude,new.longitude,new.project_id,new.planning_id,new.purpose_id,new.workorder_id,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.gis_design_id,new.province_id,new.region_id from att_details_slack where system_id=new.system_id;

END IF; 
END IF; 								
IF (TG_OP = 'DELETE' ) THEN  

       INSERT INTO public.audit_att_details_slack(
            system_id, network_id, slack_length, associated_system_id, 
            associated_network_id, associated_entity_type, duct_system_id, 
            created_by, created_on, modified_by, modified_on, network_status, 
            action,status_remark,status_updated_by,status_updated_on,is_visible_on_map,source_ref_type,source_ref_id,source_ref_description,latitude,longitude,project_id,planning_id,purpose_id,workorder_id,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,gis_design_id,province_id,region_id)
         values(old.system_id, old.network_id, old.slack_length, old.associated_system_id, 
            old.associated_network_id, old.associated_entity_type, old.duct_system_id, 
            old.created_by, old.created_on, old.modified_by, old.modified_on, old.network_status, 'D',old.status_remark,old.status_updated_by,old.status_updated_on,old.is_visible_on_map,old.source_ref_type,old.source_ref_id,old.source_ref_description,old.latitude,old.longitude,old.project_id,old.planning_id,old.purpose_id,old.workorder_id,old.origin_from,old.origin_ref_id,old.origin_ref_code,old.origin_ref_description,old.request_ref_id,old.requested_by,old.request_approved_by,old.subarea_id,old.area_id,old.dsa_id,old.csa_id,old.gis_design_id,old.province_id,old.region_id);    
         
          Delete from point_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='SLACK';

END IF; 		

RETURN NEW;
END;
$function$
;


CREATE OR REPLACE FUNCTION public.fn_trg_updateductcalculatedlength()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$

BEGIN



---UPdating cable calculated length of duct --

IF tg_op = 'INSERT' THEN

               

   UPDATE att_details_duct

   SET total_slack_length = coalesce(total_slack_length,0) + new.slack_length,

   total_slack_count  =coalesce(total_slack_count,0)+1

   where system_id =new.duct_system_id;

   return new;

END IF;

--------------

IF tg_op = 'UPDATE' THEN

               

                UPDATE att_details_duct

                SET total_slack_length = (coalesce(total_slack_length,0) - coalesce(old.slack_length,0))+ coalesce(new.slack_length,0)

                where system_id =new.duct_system_id;           

return new;

END IF;

 

 

 

IF tg_op = 'DELETE' THEN

 

                UPDATE att_details_duct

                SET total_slack_length = (coalesce(total_slack_length,0) - coalesce(old.slack_length,0)),

                    total_slack_count = coalesce(total_slack_count,0)-1

                where system_id =old.duct_system_id;             

 

     RETURN old;

END IF;

 

END

$function$
;



-- Table Triggers

create trigger trg_audit_att_details_slack after
insert
    or
delete
    or
update
    on
    public.att_details_slack for each row execute function fn_trg_audit_att_details_slack();
    
create trigger trg_trg_entity_status_history after
insert
    or
delete
    or
update
    on
    public.att_details_slack for each row execute function fn_trg_entity_status_history();
    
create trigger trg_update_entity_attribute after
insert
    or
update
    on
    public.att_details_slack for each row execute function fn_trg_update_entity_attribute();
    
create trigger trg_updateductcalculatedlength after
insert
    or
delete
    or
update
    on
    public.att_details_slack for each row execute function fn_trg_updateductcalculatedlength();
	
CREATE OR REPLACE VIEW public.vw_att_details_slack
AS SELECT slk.system_id,
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
    slk.mzone_id
   FROM att_details_slack slk
     LEFT JOIN att_details_duct dct ON slk.duct_system_id = dct.system_id
     JOIN province_boundary prov ON slk.province_id = prov.id
     JOIN region_boundary reg ON slk.region_id = reg.id
     JOIN user_master um ON slk.created_by = um.user_id
     LEFT JOIN user_master um2 ON slk.modified_by = um2.user_id;
	 
	 
-- public.vw_att_details_slack_audit source

CREATE OR REPLACE VIEW public.vw_att_details_slack_audit
AS SELECT slk.audit_id,
    slk.system_id,
    slk.network_id,
    round(slk.slack_length::numeric, 2) AS slack_length,
    slk.associated_system_id,
    slk.associated_network_id,
    slk.associated_entity_type,
    cbl.network_id AS duct_network_id,
    fn_get_date(slk.created_on, true) AS created_on,
    um.user_name AS created_by,
    fn_get_date(slk.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    fn_get_action(slk.action) AS action,
    slk.province_id,
    slk.region_id,
    slk.status_remark,
    slk.status_updated_by,
    slk.status_updated_on,
    slk.is_visible_on_map,
    slk.source_ref_type,
    slk.source_ref_id,
    slk.source_ref_description,
    cbl.system_id AS duct_id,
    slk.duct_system_id,
    slk.network_status,
    slk.parent_system_id,
    slk.parent_network_id,
    slk.parent_entity_type,
    slk.sequence_id,
    round(slk.latitude::numeric, 6) AS latitude,
    round(slk.longitude::numeric, 6) AS longitude,
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
    slk.ne_id,
    slk.prms_id,
    slk.jc_id,
    slk.mzone_id
   FROM audit_att_details_slack slk
     LEFT JOIN att_details_duct cbl ON slk.duct_system_id = cbl.system_id
     JOIN province_boundary prov ON slk.province_id = prov.id
     JOIN region_boundary reg ON slk.region_id = reg.id
     JOIN user_master um ON slk.created_by = um.user_id
     LEFT JOIN user_master um2 ON slk.modified_by = um2.user_id;
	 
	 

CREATE OR REPLACE VIEW public.vw_att_details_slack_map
AS SELECT slack.system_id,
slack.network_id,
    slack.network_status,
    cable.province_id,
    cable.region_id,
    cable.project_id,
    cable.planning_id,
    cable.workorder_id,
    cable.purpose_id,
    COALESCE(adi.updated_geom, pm.sp_geometry) AS sp_geometry,
    slack.source_ref_id,
    slack.source_ref_type,
    slack.network_id AS label_column
   FROM point_master pm
     JOIN att_details_slack slack ON pm.system_id = slack.system_id AND pm.entity_type::text = 'Slack'::text
     JOIN att_details_duct cable ON slack.duct_system_id = cable.system_id
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = slack.system_id AND upper(adi.entity_type::text) = 'SLACK';
	 
	 
CREATE OR REPLACE VIEW public.vw_att_details_slack_report
AS SELECT slk.system_id,
    slk.network_id,
    round(slk.slack_length::numeric, 2) AS slack_length,
    slk.associated_network_id,
    slk.associated_entity_type,
    cbl.network_id AS duct_network_id,
    fn_get_date(slk.created_on) AS created_on,
    fn_get_date(slk.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    slk.status_remark,
    slk.status_updated_by,
    um.user_name AS created_by,
    um.user_id AS created_by_id,
    fn_get_date(slk.status_updated_on) AS status_updated_on,
    slk.is_visible_on_map,
    slk.source_ref_type,
    slk.source_ref_id,
    slk.source_ref_description,
    slk.province_id,
    slk.region_id,
    round(slk.latitude::numeric, 6) AS latitude,
    round(slk.longitude::numeric, 6) AS longitude,
    reg.country_name,
    slk.status,
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
    slk.gis_design_id AS design_id,
    slk.st_x,
    slk.st_y,
    slk.ne_id,
    slk.prms_id,
    slk.mzone_id,
    slk.jc_id,
    slk.served_by_ring,
    slk.network_status
   FROM att_details_slack slk
     LEFT JOIN att_details_duct cbl ON slk.duct_system_id = cbl.system_id
     JOIN province_boundary prov ON slk.province_id = prov.id
     JOIN region_boundary reg ON slk.region_id = reg.id
     JOIN user_master um ON slk.created_by = um.user_id
     LEFT JOIN user_master um2 ON slk.modified_by = um2.user_id
     JOIN point_master point ON point.system_id = slk.system_id AND upper(point.entity_type::text) = upper('slack'::text);
	 
	 
	 -- public.vw_att_details_duct source

CREATE OR REPLACE VIEW public.vw_att_details_duct
AS SELECT duct.system_id,
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
    duct.total_slack_count,
    duct.total_slack_length
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
	 
-- public.vw_att_details_duct_report source

CREATE OR REPLACE VIEW public.vw_att_details_duct_report
AS SELECT duct.system_id,
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
    duct.total_slack_length
   FROM att_details_duct duct
     JOIN user_master um ON duct.created_by = um.user_id
     JOIN vendor_master vm ON duct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON duct.third_party_vendor_id = vm2.id
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
	 
-- public.vw_att_details_duct_audit source

CREATE OR REPLACE VIEW public.vw_att_details_duct_audit
AS SELECT duct.audit_id,
    duct.system_id,
    duct.network_id,
    duct.duct_name,
    duct.a_location,
    duct.b_location,
    reg.region_name,
    prov.province_name,
    round(duct.calculated_length::numeric, 2) AS calculated_length,
    round(duct.manual_length::numeric, 2) AS manual_length,
    duct.a_system_id,
    duct.a_entity_type,
    duct.b_system_id,
    duct.b_entity_type,
    duct.pin_code,
    duct.utilization,
    duct.no_of_cables,
    duct.offset_value,
    duct.specification,
    duct.category,
    duct.subcategory1,
    duct.subcategory2,
    duct.subcategory3,
    round(duct.inner_dimension::numeric, 2) AS inner_dimension,
    round(duct.outer_dimension::numeric, 2) AS outer_dimension,
    duct.duct_type,
    duct.color_code,
    duct.item_code,
    duct.parent_system_id,
    duct.parent_network_id,
    duct.parent_entity_type,
    duct.acquire_from,
    vm.name AS vendor_name,
    bd.brand AS duct_brand,
    ml.model AS duct_model,
    duct.remarks,
    fn_get_date(duct.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(duct.modified_on) AS modified_on,
    ums.user_name AS modified_by,
    fn_get_network_status(duct.network_status) AS network_status,
    fn_get_status(duct.status) AS status,
    fn_get_action(duct.action) AS action,
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
    duct.total_slack_count,
    duct.total_slack_length
   FROM audit_att_details_duct duct
     JOIN province_boundary prov ON duct.province_id = prov.id
     JOIN region_boundary reg ON duct.region_id = reg.id
     JOIN user_master um ON duct.created_by = um.user_id
     LEFT JOIN user_master ums ON duct.modified_by = ums.user_id
     JOIN vendor_master vm ON duct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON duct.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON duct.type = tp.id
     LEFT JOIN isp_brand_master bd ON duct.brand = bd.id
     LEFT JOIN isp_base_model ml ON duct.model = ml.id;
	 
select * from fn_sync_layer_columns();
	
	
