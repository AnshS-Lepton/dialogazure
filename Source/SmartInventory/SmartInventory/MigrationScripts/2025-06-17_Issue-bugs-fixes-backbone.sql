alter table backbone_plan_network_details add column loop_length double PRECISION NOT NULL DEFAULT 0;

alter table backbone_plan_network_details add column is_loop_required bool NULL DEFAULT false;

alter table backbone_plan_details add column loop_length  double PRECISION NOT NULL DEFAULT 0;

alter table backbone_plan_details add column is_loop_required bool NULL DEFAULT false;

alter table backbone_plan_details add column buffer double PRECISION NOT NULL DEFAULT 0;

------------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_backbone_get_loop_entity(p_plan_id integer)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
v_current_point_geom geometry;
v_temp_point_entity record;
v_loop_network_id record;
v_line_geom geometry;
v_arow_loop record;
BEGIN


FOR  v_temp_point_entity IN select * from backbone_plan_network_details bpnd where
entity_type ='Manhole' and loop_length > 0 and plan_id = p_plan_id order by system_id
LOOP
	
	select * into v_loop_network_id  from fn_get_network_code('Loop','Point',0,'',substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7));

	INSERT INTO public.att_details_loop(
	network_id, loop_length, associated_system_id, associated_network_id, 
	associated_entity_type, cable_system_id, created_by, created_on, 
	network_status, start_reading, end_reading, 
	parent_system_id, sequence_id, province_id, region_id, parent_network_id, 
	parent_entity_type, status, 
	is_visible_on_map, source_ref_type, source_ref_id, 
	source_ref_description, latitude, longitude, is_new_entity)
	select v_loop_network_id.network_code,loop_length,entity_system_id,entity_network_id,
	entity_type,cable_id,created_by,now(),
	'P',0,0,v_loop_network_id.parent_system_id, v_loop_network_id.sequence_id,province_id, region_id,v_loop_network_id.parent_network_id,
	v_loop_network_id.parent_entity_type,
	'A',true,'backbone planning', plan_id,'',latitude,longitude,true
	from backbone_plan_network_details tp where system_id=v_temp_point_entity.system_id
	RETURNING system_id,network_id,latitude,longitude,created_by into v_arow_loop; 

	INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
	creator_remark,approver_remark,created_by,common_name, network_status,display_name)
	values(v_arow_loop.system_id, 'Loop',st_x(v_temp_point_entity.sp_geometry)::double precision, st_y(v_temp_point_entity.sp_geometry)::double precision,
	'A',v_temp_point_entity.sp_geometry,now(),'Created', '' ,v_arow_loop.created_by,v_arow_loop.network_id,
	'P',fn_get_display_name(v_arow_loop.system_id, 'Loop'));

end loop;

		       
END
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_get_loop_entity(int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_get_loop_entity(int4) TO public;
GRANT ALL ON FUNCTION public.fn_backbone_get_loop_entity(int4) TO postgres;

----------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_network(p_plan_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
 lyrtitle record;
 v_entity_name character varying;
 v_new_entity_name character varying;
 sql text='';
 v_region_ids character varying='';
 v_province_ids character varying='';
 v_sub_sql text='';
 v_layer_id text;
BEGIN

SELECT string_agg(layer_id::text, ',') into v_layer_id
FROM layer_details 
WHERE upper(layer_name) IN ('POLE', 'MANHOLE', 'CABLE', 'SPLICECLOSURE','POD');


		FOR lyrtitle IN select distinct layer_title,layer_table,layer_name,geom_type from layer_details 
		where is_auto_plan_end_point=true or upper(layer_name) in (upper('cable'),upper('manhole'),upper('pole'))
LOOP
			v_entity_name:=lower(lyrtitle.layer_name)||'_name';
			
			select coalesce((SELECT column_name into v_new_entity_name FROM information_schema.columns
			WHERE table_name=lyrtitle.layer_table and column_name in('name',v_entity_name)),'Null');
IF EXISTS 		(SELECT 1  FROM information_schema.columns WHERE table_name=lyrtitle.layer_table and column_name ='source_ref_id')THEN
BEGIN

 IF(sql!='')THEN
 sql:=sql||' UNION ';
 END IF; 
			--execute 'insert into temp_nwt_entity_detalis
			sql:=sql||'Select lv.region_id,lv.province_id  from '||lyrtitle.layer_table||' as lv 
 			where lv.source_ref_id='||p_plan_id||'::character varying and Upper(source_ref_type)=''BACKBONE PLANNING''';
 END;
 END IF	;		
END LOOP;

 
 v_sub_sql:= 'select array_to_string(array_agg(row.region_id), '','')as region_id ,array_to_string(array_agg(row.province_id), '','')as province_id from('||sql||') row';

Execute v_sub_sql into v_region_ids,v_province_ids;

RAISE INFO ' v_sub_sql =%', v_region_ids;

v_sub_sql:='select '||v_region_ids||' as  region_ids, '||v_province_ids||' as province_ids, '|| quote_literal(v_layer_id)||' as layer_id ,* from backbone_plan_details where plan_id='||p_plan_id;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||v_sub_sql||') row';
END ;

$function$
;


-------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_auto_create_cable(p_a_system_id integer, p_a_entity_type character varying, p_a_network_id character varying, p_b_system_id integer, p_b_entity_type character varying, p_b_network_id character varying, p_longlat character varying, p_structure_id integer, p_region_id integer, p_province_id integer, p_user_id integer, p_rfs_type character varying, p_length double precision, itm_spec_cable character varying, p_plan_id integer, p_cable_type character varying, p_duct_id integer, p_duct_network_id character varying)
 RETURNS record
 LANGUAGE plpgsql
AS $function$
DECLARE
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow_cable record;
v_arow_network_id record;
v_cable_display_value CHARACTER VARYING;
v_a_display_value CHARACTER VARYING;
v_b_display_value CHARACTER VARYING;
v_display_value CHARACTER VARYING;
v_display_name CHARACTER VARYING;
v_duct_value character varying;
itm_spec_rec record;
itm_code character varying;
BEGIN
V_IS_VALID:=true;
V_MESSAGE:='Successfully';
 
    SELECT split_part(itm_spec_cable, ')', 1) AS itm_code, split_part(itm_spec_cable, ')', 2) AS fiber_core into itm_spec_rec;
  itm_code = REPLACE(itm_spec_rec.itm_code,'(','');
	--RAISE INFO 'itm_spec_rec: %', REPLACE(itm_spec_rec.itm_code,'(','') ;

	select * into v_arow_network_id from fn_get_line_network_code('Cable',p_a_network_id,p_b_network_id,p_longlat,'OSP');
		
	INSERT INTO ATT_DETAILS_CABLE(is_visible_on_map,audit_item_master_id,network_id,cable_name,a_location,b_location,total_core,no_of_tube,no_of_core_per_tube,cable_measured_length,cable_calculated_length,
	cable_type,specification,category,subcategory1,subcategory2,subcategory3,item_code,	
	vendor_id,network_status,status,province_id,region_id,created_by,created_on,a_system_id,a_network_id,a_entity_type,
	b_system_id,b_network_id,b_entity_type,sequence_id,cable_category,parent_system_id,parent_network_id,parent_entity_type,start_reading,	
	end_reading,structure_id,utilization,duct_id,trench_id,is_used,source_ref_type,source_ref_id,ownership_type)

select  true,audit_id,v_arow_network_id.network_code,v_arow_network_id.network_code,p_a_network_id,p_b_network_id,other::integer,no_of_tube,no_of_core_per_tube,ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),
	p_cable_type,specification,'Cable', null, null, null,code,
	vendor_id,'P','A',p_province_id,p_region_id,p_user_id,now(),p_a_system_id,p_a_network_id,
	p_a_entity_type,p_b_system_id,p_b_network_id,p_b_entity_type,v_arow_network_id.sequence_id,'Feeder',v_arow_network_id.parent_system_id,
	v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
	0,0,p_structure_id,'L',0,0,true,'backbone planning',p_plan_id,'OWN' from item_template_master itm where category_reference ='Cable' and code = itm_code and other = itm_spec_rec.fiber_core limit 1 
	RETURNING system_id,network_id,no_of_tube,total_core into v_arow_cable;

		RAISE INFO 'v_arow_cable: %', v_arow_cable.SYSTEM_ID;

	INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,
	common_name,network_status,is_virtual,display_name,source_ref_type,source_ref_id)
	values(v_arow_cable.SYSTEM_ID,'Cable','A',
	ST_GeomFromText('LINESTRING('||p_longlat||')',4326)	
	,'','',
	p_user_id,0,v_arow_cable.network_id,'P',false,fn_get_display_name(v_arow_cable.SYSTEM_ID,'Cable'),'backbone planning',p_plan_id);
	
	select display_name into v_a_display_value from point_master where system_id=p_a_system_id and upper(entity_type)=upper(p_a_entity_type);
	select display_name into v_b_display_value from point_master where system_id=p_b_system_id and upper(entity_type)=upper(p_b_entity_type);

	insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,
	associated_entity_type,associated_display_name,created_on,created_by,is_termination_point)
	values(v_arow_cable.system_id,v_arow_cable.network_id,'Cable',v_display_value,p_a_system_id,p_a_network_id,p_a_entity_type,v_a_display_value,now(),p_user_id,true),
	(v_arow_cable.system_id,v_arow_cable.network_id,'Cable',v_display_value,p_b_system_id,p_b_network_id,p_b_entity_type,v_b_display_value,now(),p_user_id,true);

	if(p_duct_id >0)
	then
	
	insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
		entity_type,created_on,created_by,associated_display_name,entity_display_name)
		values('Cable',v_arow_cable.system_id,v_arow_cable.network_id,p_duct_network_id,p_duct_id,'Duct',now(),p_user_id,
		fn_get_display_name(v_arow_cable.system_id,'Cable'),fn_get_display_name(p_duct_id,'Duct')); 
		
	end if;

	
	
	perform(fn_set_cable_color_info(v_arow_cable.system_id,p_user_id,v_arow_cable.no_of_tube,v_arow_cable.total_core));					
		
	RETURN v_arow_cable;          
END
$function$
;

----------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_save_plan(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_cable_length double precision, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, plan_id integer, startpoint_networkid character varying, endpoint_network_id character varying)
 RETURNS TABLE(status boolean, message character varying, v_plan_id integer)
 LANGUAGE plpgsql
AS $function$

DECLARE
vsystem_id integer;
v_network_id character varying;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow record;
v_system_id integer;
v_entity_type character varying;
v_arow_network_id record;
v_arow_parent_details record;
v_arow_cable record;
v_arow_box record;
v_layer_table character varying;
v_arow_splitter record;
sql text;
v_arow_wcr record;
v_arow_layer_details record;
v_layer_wcr_mapping character varying;
v_new_wcr_mapping record;
v_current_system_id integer;
v_line_geom geometry;
v_region_province record;
v_arow_closure record;
v_counter integer;
v_arow_prev_closure record;
v_arow_prev_cable record;
-- v_other_end_geom geometry;
v_prev_qty_length double precision;
V_TOTAL_CALC_LENGTH DOUBLE PRECISION;
v_line_total_length DOUBLE PRECISION;
v_total_line integer;
p_rfs_type character varying;
v_manhole_item_id integer;
v_arow_manhole record;
v_sub_line_geom geometry;
v_sub_line_start_point geometry;
v_sub_line_end_point geometry;
v_sub_line character varying;
v_arow_prev_manhole record;
v_arow_trench record;
v_arow_duct record;
v_arow_fms record;
v_arow_pod record;
v_arow_first_closure record;
v_point_entity character varying;
v_is_osp_tp BOOLEAN;
p_plan_id integer;
v_audit_item_master_id_sc integer;
v_audit_item_master_id_point_entity integer;
v_audit_item_master_id_cable integer;
v_audit_item_master_id_duct integer;
v_audit_item_master_id_trench integer;
v_layer_ids character varying;
--v_arow_first_manhole record;
v_temp_point_entity record;
--------
v_current_point_geom geometry;
v_start_point character varying;
v_end_point character varying;
v_loop_length double precision;
v_point_entity_type character varying;
v_a_display_value CHARACTER VARYING;
v_inner_query text;
v_arow_end_manhole record;
v_arow_middle_manhole record;
v_arow_middle_manhole1 record;
v_total_record_temp integer;
v_counter_2 integer;
v_geom_str character varying;
p_cable_type character varying;
p_skip_point_id integer;
BEGIN
v_counter_2=0;
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
v_counter:=0;
sql:='';
v_prev_qty_length:=0;
p_rfs_type:='A-RFS';
p_plan_id=plan_id;

CREATE temp TABLE temp_connection
(
source_system_id integer,
source_network_id character varying(100),
source_entity_type character varying(100),
source_port_no integer,
destination_system_id integer,
destination_network_id character varying(100),
destination_entity_type character varying(100),
destination_port_no integer,
is_source_cable_a_end boolean NOT NULL DEFAULT false,
is_destination_cable_a_end boolean NOT NULL DEFAULT false,
equipment_system_id integer,
equipment_network_id character varying(100),
equipment_entity_type character varying(100),
created_by integer,
splicing_source character varying(100)
) on commit drop;


IF(V_IS_VALID)
THEN

select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
select st_length(v_line_geom,false)::double precision into v_line_total_length;

IF(V_IS_VALID)
THEN

select system_id into p_skip_point_id  from backbone_plan_network_details tp where tp.plan_id = p_plan_id and entity_type in ('Manhole','SpliceClosure') order by system_id desc limit 1;

select '' as network_id,'' as entity_type ,0 as system_id  into v_arow_prev_closure; 
--select common_name as network_id,entity_type ,system_id  into v_arow_prev_closure from point_master pm where entity_type = 'POD' and common_name = startpoint_networkId ; 


--------------   start backbone data query  -----------------------------------------
FOR v_temp_point_entity IN select * from backbone_plan_network_details tp where tp.plan_id=p_plan_id and entity_type in ('Pole','Manhole') order by system_id
LOOP	
------------ Get all entity audit_item_master_id =================================================

select (select (select * from fn_get_template_detail(p_user_id,'SpliceClosure',''))->>'audit_item_master_id')::integer into 
v_audit_item_master_id_sc;
select (select (select * from fn_get_template_detail(p_user_id,v_temp_point_entity.entity_type,''))->>'audit_item_master_id')::integer into v_audit_item_master_id_point_entity ;
select (select (select * from fn_get_template_detail(p_user_id,'duct',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_duct ;
select (select (select * from fn_get_template_detail(p_user_id,'trench',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_trench;

select * into v_region_province from fn_getregionprovince(substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),'Point');
RAISE INFO 'v_region_province 1 : %', v_region_province;

update backbone_plan_network_details set province_id =v_region_province.province_id,region_id=v_region_province.region_id where system_id=v_temp_point_entity.system_id;


RAISE INFO 'v_region_province 1: %', v_region_province;

if v_temp_point_entity.system_id != p_skip_point_id 
then
if (upper(v_temp_point_entity.entity_type)=upper('Manhole'))
then

SELECT * into v_arow_network_id FROM fn_get_clone_network_code( v_temp_point_entity.entity_type, 'Point', substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),0,'');

INSERT INTO att_details_manhole(is_visible_on_map,audit_item_master_id,ownership_type,network_id, manhole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
,project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',v_arow_network_id.message,v_arow_network_id.message,
v_temp_point_entity.longitude,v_temp_point_entity.latitude,
v_region_province.province_id,v_region_province.region_id, specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',v_arow_network_id.o_p_system_id,
v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0, 0, 0, 0,'backbone planning',p_plan_id
FROM item_template_manhole where created_by=p_user_id limit 1 
 RETURNING * into v_arow_middle_manhole;
-------------
  RAISE INFO 'v_arow_middle_manhole ID1: %', v_arow_middle_manhole.system_id ;
 
INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)
values(v_arow_middle_manhole.system_id, v_temp_point_entity.entity_type,v_temp_point_entity.latitude,v_temp_point_entity.longitude,
'A',v_temp_point_entity.sp_geometry,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',
fn_get_display_name(v_arow_middle_manhole.system_id, v_temp_point_entity.entity_type),p_plan_id,'backbone planning');

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Manhole'::character varying ,v_region_province.province_id,0,false));

 update backbone_plan_network_details set 
entity_network_id = v_arow_middle_manhole.network_id, entity_system_id = v_arow_middle_manhole.system_id
where system_id = v_temp_point_entity.system_id;

------------- insert child spliceclosure ------------------------------------
v_arow_network_id = null;
select * into v_arow_network_id from fn_get_network_code('SpliceClosure','Point',0,'Province',substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7));
  RAISE INFO 'v_arow_network_id ID1: %', v_arow_network_id.network_code ;
 
insert into att_details_spliceclosure(is_visible_on_map,audit_item_master_id,ownership_type,network_id,spliceclosure_name,latitude,
longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,
no_of_input_port,no_of_output_port,is_used,is_virtual,source_ref_type,source_ref_id)

select true,v_audit_item_master_id_sc,'OWN',v_arow_network_id.network_code,v_arow_network_id.network_code,v_temp_point_entity.latitude,v_temp_point_entity.longitude,v_region_province.province_id,v_region_province.region_id,
specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, 0,0,0,0,0,0,'A',p_user_id,now(),
'P',v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
v_arow_network_id.sequence_id,no_of_ports,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false,
'backbone planning',p_plan_id
from item_template_spliceClosure where created_by=p_user_id limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port,'Splice Closure' as entity_type into v_arow_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
created_by,common_name, network_status,no_of_ports,display_name,source_ref_id ,source_ref_type)
values(v_arow_closure.system_id, 'SpliceClosure',v_temp_point_entity.latitude,v_temp_point_entity.longitude,'A',
v_temp_point_entity.sp_geometry,now(),'Created', ''
,p_user_id,v_arow_network_id.network_code, 'P',v_arow_closure.no_of_ports::character varying,
fn_get_display_name(v_arow_closure.system_id, 'SpliceClosure'),p_plan_id,'backbone planning');

perform (fn_geojson_update_entity_attribute(v_arow_closure.system_id,'SpliceClosure'::character varying ,v_region_province.province_id,0,false));


perform(fn_bulk_insert_port_info(v_arow_closure.no_of_ports,v_arow_closure.no_of_ports,'SpliceClosure',
v_arow_closure.system_id,v_arow_closure.network_id,p_user_id));

insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
entity_type,created_on,created_by,associated_display_name,entity_display_name)
values('SpliceClosure',v_arow_closure.system_id,v_arow_closure.network_id,v_arow_middle_manhole.network_id,v_arow_middle_manhole.system_id,v_temp_point_entity.entity_type,now(),p_user_id,
fn_get_display_name(v_arow_closure.system_id,'SpliceClosure'),fn_get_display_name(v_arow_middle_manhole.system_id,v_temp_point_entity.entity_type));


else

v_arow_network_id = null;

select * into v_arow_network_id from fn_get_network_code('Pole','Point',0,'Province',substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7));
RAISE INFO 'v_arow_network_id p: %', v_arow_network_id;

INSERT INTO att_details_pole(is_visible_on_map,audit_item_master_id,ownership_type,pole_type,network_id, pole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code,
vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
, project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',pole_type,v_arow_network_id.network_code,v_arow_network_id.network_code,
v_temp_point_entity.longitude,v_temp_point_entity.latitude,
v_region_province.province_id,v_region_province.region_id, specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,v_arow_network_id.sequence_id,
0, 0, 0, 0 ,'backbone planning',p_plan_id
FROM item_template_pole where created_by=p_user_id limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_middle_manhole;

RAISE INFO 'v_arow_middle_manhole p: %', v_arow_middle_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_middle_manhole.system_id, v_temp_point_entity.entity_type,v_temp_point_entity.longitude,v_temp_point_entity.latitude,
'A',v_temp_point_entity.sp_geometry,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',
fn_get_display_name(v_arow_middle_manhole.system_id, 'Pole'),'backbone planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Pole'::character varying ,v_region_province.province_id,0,false));

 update backbone_plan_network_details set 
entity_network_id = v_arow_middle_manhole.network_id, entity_system_id = v_arow_middle_manhole.system_id
where system_id = v_temp_point_entity.system_id;

------------- insert child spliceclosure ------------------------------------

select * into v_arow_network_id from fn_get_network_code('SpliceClosure','Point',0,'Province',substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7));

insert into att_details_spliceclosure(is_visible_on_map,audit_item_master_id,ownership_type,network_id,spliceclosure_name,latitude,
longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,
no_of_input_port,no_of_output_port,is_used,is_virtual,source_ref_type,source_ref_id)

select true,v_audit_item_master_id_sc,'OWN',v_arow_network_id.network_code,v_arow_network_id.network_code,v_temp_point_entity.latitude,v_temp_point_entity.longitude,v_region_province.province_id,v_region_province.region_id,
specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, 0,0,0,0,0,0,'A',p_user_id,now(),
'P',v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
v_arow_network_id.sequence_id,no_of_ports,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false,
'backbone planning',p_plan_id
from item_template_spliceClosure where created_by=p_user_id limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port,'Splice Closure' as entity_type into v_arow_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
created_by,common_name, network_status,no_of_ports,display_name,source_ref_type,source_ref_id)
values(v_arow_closure.system_id, 'SpliceClosure',v_temp_point_entity.latitude,v_temp_point_entity.longitude,'A',
v_temp_point_entity.sp_geometry,now(),'Created', ''
,p_user_id,v_arow_network_id.network_code, 'P',v_arow_closure.no_of_ports::character varying,
fn_get_display_name(v_arow_closure.system_id, 'SpliceClosure'),'backbone planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_closure.system_id,'SpliceClosure'::character varying ,v_region_province.province_id,0,false));

perform(fn_bulk_insert_port_info(v_arow_closure.no_of_ports,v_arow_closure.no_of_ports,'SpliceClosure',
v_arow_closure.system_id,v_arow_closure.network_id,p_user_id));

insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
entity_type,created_on,created_by,associated_display_name,entity_display_name)
values('SpliceClosure',v_arow_closure.system_id,v_arow_closure.network_id,v_arow_middle_manhole.network_id,v_arow_middle_manhole.system_id,v_temp_point_entity.entity_type,now(),p_user_id,
fn_get_display_name(v_arow_closure.system_id,'SpliceClosure'),fn_get_display_name(v_arow_middle_manhole.system_id,v_temp_point_entity.entity_type));

end if;

end if;
--------------------- insert cable --------------------------
if(p_skip_point_id = v_temp_point_entity.system_id) then
select '' as network_id,'' as entity_type ,0 as system_id  into v_arow_prev_closure; 
end if;

select substring(left(St_astext(v_temp_point_entity.line_sp_geometry),-1),12) ::character varying INTO v_geom_str;
select * into v_arow_cable from fn_backbone_auto_create_cable(v_arow_prev_closure.system_id::integer,
v_arow_prev_closure.entity_type::character varying,v_arow_prev_closure.network_id::character varying,
v_arow_closure.system_id::integer,v_arow_closure.entity_type::character varying,v_arow_closure.network_id::character varying, 
v_geom_str,
0,v_region_province.region_id::integer ,v_region_province.province_id,p_user_id,p_rfs_type ::character varying,500,v_temp_point_entity.fiber_type,p_plan_id,
'Overhead'::character varying,0,''::character varying)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));

 update backbone_plan_network_details set 
cable_network_id = v_arow_cable.network_id, cable_id = v_arow_cable.system_id
where system_id = v_temp_point_entity.system_id;

if v_arow_prev_cable is not null
then
insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,
destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,created_by,splicing_source,
equipment_system_id,equipment_network_id,equipment_entity_type)
values(v_arow_prev_cable.system_id,v_arow_prev_cable.network_id,'Cable',1,v_arow_cable.system_id,v_arow_cable.network_id,'Cable',1,false,true,
p_user_id,'PROVISIONNING',
v_arow_prev_closure.system_id, v_arow_prev_closure.network_id,v_arow_prev_closure.entity_type);
end if;
v_arow_prev_closure.system_id := v_arow_closure.system_id;
v_arow_prev_closure.entity_type := v_arow_closure.entity_type::varchar;
v_arow_prev_closure.network_id := v_arow_closure.network_id::varchar;
v_arow_prev_cable := v_arow_cable;


end loop;
--------------   end backbone data query  -----------------------------------------

--------------   start sproutplan data query  -----------------------------------------

FOR v_temp_point_entity IN (select * from backbone_plan_network_details tp where tp.plan_id=p_plan_id and 	entity_type = 'Cable' order by system_id)
loop 

select common_name as network_id,entity_type ,system_id  into v_arow_closure 
	from point_master pm where entity_type = 'POD' and common_name = v_temp_point_entity.entity_network_id;

select substring(left(St_astext(v_temp_point_entity.line_sp_geometry),-1),12) ::character varying INTO v_geom_str;

    IF EXISTS (SELECT 1 FROM polygon_master 
      WHERE ST_Intersects(
        sp_geometry,
        ST_GeomFromText('LINESTRING(' || v_geom_str || ')', 4326))
        AND entity_type = 'Restricted_Area')
      then 
        p_cable_type = 'Overhead';
        else 
        p_cable_type = 'Underground';
     end if;
       

if (v_temp_point_entity.sp_geometry is not null)
then 
   select * into v_region_province from fn_getregionprovince(substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),'Point');
	
	update backbone_plan_network_details set province_id =v_region_province.province_id,region_id=v_region_province.region_id where system_id=v_temp_point_entity.system_id;

 if(p_cable_type = 'Underground') 
 then	
	v_arow_network_id =null;

	SELECT * into v_arow_network_id FROM fn_get_clone_network_code( 'Manhole', 'Point', substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),0,'');

INSERT INTO att_details_manhole(is_visible_on_map,audit_item_master_id,ownership_type,network_id, manhole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
,project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',v_arow_network_id.message,v_arow_network_id.message,
ST_Y(v_temp_point_entity.sp_geometry),ST_X(v_temp_point_entity.sp_geometry),
v_region_province.province_id,v_region_province.region_id, specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',v_arow_network_id.o_p_system_id,
v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0, 0, 0, 0,'backbone planning',p_plan_id
FROM item_template_manhole where created_by=p_user_id limit 1 
 RETURNING system_id::integer,network_id :: character varying,'Manhole'::character varying as entity_type into v_arow_middle_manhole;
-------------

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_middle_manhole.system_id, 'Manhole',ST_Y(v_temp_point_entity.sp_geometry),ST_X(v_temp_point_entity.sp_geometry),
'A',v_temp_point_entity.sp_geometry,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',
fn_get_display_name(v_arow_middle_manhole.system_id, 'Manhole'),'backbone planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Manhole'::character varying ,v_region_province.province_id,0,false)) ;

else 
	v_arow_network_id =null;

	SELECT * into v_arow_network_id FROM fn_get_clone_network_code( 'Pole', 'Point', substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),0,'');

INSERT INTO att_details_pole(is_visible_on_map,audit_item_master_id,ownership_type,pole_type,network_id, pole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code,
vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
, project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',pole_type,v_arow_network_id.message::character varying,v_arow_network_id.message::character varying,
ST_Y(v_temp_point_entity.sp_geometry),ST_X(v_temp_point_entity.sp_geometry),
v_region_province.province_id,v_region_province.region_id, specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
v_arow_network_id.o_p_system_id ::integer,v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id,
0, 0, 0, 0 ,'backbone planning',p_plan_id
FROM item_template_pole where created_by=p_user_id limit 1 RETURNING system_id::integer,network_id::character varying,'Pole'::character varying as entity_type into v_arow_middle_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_middle_manhole.system_id, 'Pole',ST_Y(v_temp_point_entity.sp_geometry),ST_X(v_temp_point_entity.sp_geometry),
'A',v_temp_point_entity.sp_geometry,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',
fn_get_display_name(v_arow_middle_manhole.system_id, 'Pole'),'backbone planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Pole'::character varying ,v_region_province.province_id,0,false));

end if;

select * into v_arow_network_id from fn_get_network_code('SpliceClosure','Point',0,'Province',substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7));

insert into att_details_spliceclosure(is_visible_on_map,audit_item_master_id,ownership_type,network_id,spliceclosure_name,latitude,
longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,
no_of_input_port,no_of_output_port,is_used,is_virtual,source_ref_type,source_ref_id)

select true,v_audit_item_master_id_sc,'OWN',v_arow_network_id.network_code,v_arow_network_id.network_code,v_temp_point_entity.latitude,v_temp_point_entity.longitude,v_region_province.province_id,v_region_province.region_id,
specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, 0,0,0,0,0,0,'A',p_user_id,now(),
'P',v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
v_arow_network_id.sequence_id,no_of_ports,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false,
'backbone planning',p_plan_id
from item_template_spliceClosure where created_by=p_user_id limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port,'Splice Closure' as entity_type into v_arow_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
created_by,common_name, network_status,no_of_ports,display_name,source_ref_type,source_ref_id)
values(v_arow_closure.system_id, 'SpliceClosure',v_temp_point_entity.latitude,v_temp_point_entity.longitude,'A',
v_temp_point_entity.sp_geometry,now(),'Created', ''
,p_user_id,v_arow_network_id.network_code, 'P',v_arow_closure.no_of_ports::character varying,
fn_get_display_name(v_arow_closure.system_id, 'SpliceClosure'),'backbone planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_closure.system_id,'SpliceClosure'::character varying ,v_region_province.province_id,0,false));

perform(fn_bulk_insert_port_info(v_arow_closure.no_of_ports,v_arow_closure.no_of_ports,'SpliceClosure',
v_arow_closure.system_id,v_arow_closure.network_id,p_user_id));

insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
entity_type,created_on,created_by,associated_display_name,entity_display_name)
values('SpliceClosure',v_arow_closure.system_id,v_arow_closure.network_id,v_arow_middle_manhole.network_id,v_arow_middle_manhole.system_id,v_temp_point_entity.entity_type,now(),p_user_id,
fn_get_display_name(v_arow_closure.system_id,'SpliceClosure'),fn_get_display_name(v_arow_middle_manhole.system_id,v_temp_point_entity.entity_type));

else

SELECT distinct pm.common_name as network_id,pm.entity_type ,pm.system_id  into v_arow_middle_manhole 
FROM point_master pm
JOIN att_details_manhole mh 
    ON mh.source_ref_type = pm.source_ref_type 
   AND mh.source_ref_id = pm.source_ref_id 
WHERE pm.entity_type = 'Manhole'
  AND lower(pm.source_ref_type) = 'backbone planning'
  AND pm.source_ref_id = p_plan_id::varchar
  AND ST_DWithin(pm.sp_geometry, ST_GeomFromText(
            'POINT(' || 
            split_part(v_temp_point_entity.intersect_line_geom, ',', 2)::double precision || ' ' || 
            split_part(v_temp_point_entity.intersect_line_geom, ',', 1)::double precision || 
            ')', 
            4326),1.0)
ORDER BY pm.system_id desc limit 1;
           
end if;

select * into v_arow_cable from fn_backbone_auto_create_cable(v_arow_middle_manhole.system_id::integer,
v_arow_middle_manhole.entity_type::character varying,v_arow_middle_manhole.network_id::character varying,
v_arow_closure.system_id::integer,v_arow_closure.entity_type::character varying,v_arow_closure.network_id::character varying, 
v_geom_str,
0,v_region_province.region_id::integer,v_region_province.province_id,p_user_id,p_rfs_type ::character varying,500,v_temp_point_entity.fiber_type,p_plan_id,
p_cable_type,0,''::character varying)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);


perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));	

update backbone_plan_network_details set 
cable_network_id = v_arow_cable.network_id, cable_id = v_arow_cable.system_id
where system_id = v_temp_point_entity.system_id;
end loop;
--------------   end sproutplan data query  -----------------------------------------

if ((SELECT COUNT(1) FROM temp_connection)>0)
then
 
perform(fn_auto_provisioning_save_connections());

END IF;

PERFORM(fn_backbone_get_loop_entity(p_plan_id));


END IF;

IF(V_IS_VALID)
THEN
V_MESSAGE:='Network plan processed successfully!';
update backbone_plan_details t set status = 'Success' where t.plan_id = p_plan_id and created_by = p_user_id;
END IF;
end if;
RETURN QUERY SELECT V_IS_VALID::boolean AS STATUS, V_MESSAGE::CHARACTER VARYING AS MESSAGE,plan_id :: integer as v_plan_id ;

end;
$function$
;

---------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_draft_network(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, p_sprout_fiber_type character varying, p_backbone_fiber_type character varying, p_nearest_sites character varying, p_pole_span double precision, p_manhole_span double precision, v_buffer double precision, start_site_network_id character varying, end_site_network_id character varying, p_threshold double precision, p_looplength double precision, p_is_looprequired boolean)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_plan_id INTEGER;
    rec record;
    v_sprout_line_geom geometry;
    v_site_geom record;
    v_common_names text[];
   geomtxt character varying;
  	v_line_geom geometry;
 	v_line_geom_length double precision;
   current_fraction double precision := 0.0;
   structure_location geometry;
   step_fraction double precision;
    line_geom geometry;
begin
	

    INSERT INTO backbone_plan_details(plan_name, start_point, end_point, is_create_trench, is_create_duct, pole_distance, created_by, sprout_fiber_type, backbone_fiber_type,threshold,buffer,is_loop_required,loop_length)
    VALUES (p_plan_name, p_startpoint, p_endpoint, is_create_trench, is_create_duct, p_pole_span, p_user_id, p_sprout_fiber_type, p_backbone_fiber_type,p_threshold,v_buffer,p_is_looprequired,p_looplength)
    RETURNING plan_id INTO v_plan_id;
   
select ST_GeomFromText(concat('LINESTRING(', p_line_geom, ')'), 4326) into v_line_geom;
select st_length(v_line_geom,false) into v_line_geom_length;

         WHILE current_fraction < 1.0 LOOP
        -- Try pole first
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_pole_span / v_line_geom_length), 1.0));

        -- Check if it's in a restricted area
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            step_fraction := p_pole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
            
            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction
            ) VALUES (
                v_plan_id, 'Pole', ST_X(ST_EndPoint(line_geom)),  ST_Y(ST_EndPoint(line_geom)) , ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction
            );           
           
        else
        	line_geom = NULL;
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
            
            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,loop_length,is_loop_required 
            ) VALUES (
                v_plan_id, 'Manhole', ST_X(ST_EndPoint(line_geom)), ST_Y(ST_EndPoint(line_geom)), ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction,p_looplength,p_is_looprequired
            );

        END IF;


        -- Move to next step
        current_fraction := current_fraction + step_fraction;

        -- Exit safety
        EXIT WHEN current_fraction >= 1.0;
    END LOOP;  
      
   FOR v_site_geom IN 
   (SELECT sp_geometry,common_name  FROM point_master WHERE common_name in ( SELECT unnest(string_to_array(p_nearest_sites, ',')) )
   )
loop
		
SELECT 
    ST_ClosestPoint(line.line_sp_geometry, v_site_geom.sp_geometry) AS nearest_point INTO v_sprout_line_geom
FROM 
    backbone_plan_network_details line
WHERE 
    ST_Intersects(
        line.line_sp_geometry,
        ST_Buffer(v_site_geom.sp_geometry, v_buffer)  -- ~5 meters buffer (in degrees)
    )
    AND plan_id = v_plan_id
ORDER BY 
    ST_Distance(line.line_sp_geometry, v_site_geom.sp_geometry)
LIMIT 1;

    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type,intersect_line_geom,site_geom 
    )
    VALUES (
        v_plan_id,'Cable', p_user_id, p_sprout_fiber_type,
         round(ST_Y(v_sprout_line_geom)::numeric, 6)::character varying || ',' || round(ST_X(v_sprout_line_geom)::numeric, 6)::character varying ,
   round(ST_Y(v_site_geom.sp_geometry)::numeric, 6)::character varying || ',' || round(ST_X(v_site_geom.sp_geometry)::numeric, 6)::character varying
    ) RETURNING 
    intersect_line_geom,
    system_id,
    plan_id 
INTO rec;

WITH threshold_closest_line AS (
    SELECT
        system_id,
        line_sp_geometry,
        sp_geometry,
        ST_DistanceSphere(
            sp_geometry,
            v_sprout_line_geom
        ) AS distance_meters
    FROM
        backbone_plan_network_details
    WHERE
        st_within(
            sp_geometry,
            st_buffer_meters(v_sprout_line_geom, p_threshold)
        )
        AND plan_id = v_plan_id
    ORDER BY
        distance_meters ASC
    LIMIT 1
)
UPDATE backbone_plan_network_details target
SET intersect_line_geom =  
    round(ST_Y(threshold_closest_line.sp_geometry)::numeric, 6)::varchar || ',' || 
    round(ST_X(threshold_closest_line.sp_geometry)::numeric, 6)::varchar
FROM threshold_closest_line
WHERE target.system_id = rec.system_id;

IF NOT EXISTS (
    select  1
    FROM
        backbone_plan_network_details
    WHERE
        st_within(
            sp_geometry,
            st_buffer_meters(v_sprout_line_geom, p_threshold)
        )
        AND plan_id = v_plan_id  
) THEN
    UPDATE backbone_plan_network_details
    SET sp_geometry = v_sprout_line_geom
    WHERE system_id = rec.system_id;
END IF;


END LOOP;

 UPDATE backbone_plan_network_details
 SET loop_length = 0
 WHERE system_id = (select system_id  from backbone_plan_network_details where entity_type in ('Manhole','Pole') and plan_id = v_plan_id order by system_id desc limit 1);
   


	if exists (select 1 from backbone_plan_network_details where plan_id = v_plan_id and entity_type = 'Cable' )
    then
  -- Return as JSON
    RETURN QUERY 
    SELECT row_to_json(t)
    FROM (
        SELECT system_id,plan_id, fiber_type, intersect_line_geom, site_geom, created_by 
        FROM backbone_plan_network_details 
        WHERE created_by = p_user_id and plan_id = v_plan_id and fiber_type = p_sprout_fiber_type and entity_type = 'Cable' 
    ) AS t;
   else 
   RETURN QUERY 
    SELECT row_to_json(t)
    FROM (
        SELECT v_plan_id as plan_id
    ) AS t;
   end if;
   
END;
$function$
;


-------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_bom(p_plan_id integer, p_user_id integer, p_backbone_fiber_type character varying, p_sprout_fibertype character varying, p_backbone_line_geom character varying, p_endpoint_network_id character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
 v_line_geom geometry;
 v_line_geom_length double precision;
  v_skip_systemid integer;
begin
	
select ST_GeomFromText(concat('LINESTRING(', p_backbone_line_geom, ')'), 4326) into v_line_geom;
select st_length(v_line_geom,false) into v_line_geom_length;

select system_id into v_skip_systemid from backbone_plan_network_details where entity_type in ('Pole','Manhole') order by system_id desc limit 1;

return query
 select row_to_json(row_result) from (
 select t1.entity_type,t1.qty::text as cable_Length_qty,itm.cost_per_unit ,itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost  from (select entity_type ,count(*) as qty from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type in ('Pole') and system_id not in (v_skip_systemid) group by entity_type) t1
  join item_template_master itm on itm.category_reference = t1.entity_type where specification ='Generic' --itm.cost_per_unit <> 0 and  itm.service_cost_per_unit <> 0
  
  UNION all
  
 select t1.entity_type,t1.qty ::text ,itm.cost_per_unit ,itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost  from (select entity_type ,count(*) as qty from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type in ('Manhole') and system_id not in (v_skip_systemid)  group by entity_type) t1
  join item_template_master itm on itm.category_reference = t1.entity_type where specification ='Generic' --itm.cost_per_unit <> 0 and  itm.service_cost_per_unit <> 0 
  
  union all 
  
   select t1.entity_type,SUM(t1.qty)::text as cable_Length_qty,SUM(itm.cost_per_unit),SUM(itm.service_cost_per_unit) ,
 SUM(((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit))) as total_cost  from (select 'SpliceClosure' as entity_type ,count(*) as qty from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type in ('Pole','Manhole') and system_id not in (v_skip_systemid) group by entity_type) t1
  join item_template_master itm on itm.category_reference = 'SpliceClosure' where specification ='generic'
   group by t1.entity_type --itm.cost_per_unit <> 0 and  itm.service_cost_per_unit <> 0
 
   UNION all
   
select t1.entity_type, t1.qty::text, itm.cost_per_unit , itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost
from (
 select 'Loop' as entity_type, count(1) as qty 
 from backbone_plan_network_details bpnd 
 where created_by = p_user_id 
   and plan_id = p_plan_id  
   and entity_type in ('Manhole') 
   and system_id not in (v_skip_systemid) 
 group by entity_type, fiber_type
) t1
join item_template_master itm 
  on itm.category_reference = 'Cable' 
 where '(' || itm.code::varchar || ')' || itm.other::varchar = p_backbone_fiber_type

 
   UNION ALL
  select
  t.entity_type,
   CONCAT(ROUND(SUM(t.qty)::NUMERIC, 3), '/', SUM(t.count_cable)) AS total_qty,
  SUM(t.cost_per_unit) AS total_cost_per_unit,
  SUM(t.service_cost_per_unit) AS total_service_cost_per_unit,
  round(SUM(t.total_cost)::numeric,3) AS grand_total_cost
FROM (
select t1.entity_type,t1.qty,t1.count_cable::INTEGER,itm.cost_per_unit ,itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost  from (
 
 select 'Cable' as entity_type,v_line_geom_length as qty ,fiber_type,count(1) as count_cable from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type <> 'Cable' and fiber_type = p_backbone_fiber_type group by entity_type,fiber_type
 
 ) t1
  join item_template_master itm on itm.category_reference = t1.entity_type where '(' || itm.code::varchar || ')' || itm.other::varchar = t1.fiber_type
  
  UNION ALL 
  select t1.entity_type,t1.qty,t1.count_cable::INTEGER,itm.cost_per_unit ,itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost  from (select entity_type,sum(cable_length) as qty ,fiber_type,count(1) as count_cable from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type in ('Cable') and fiber_type = p_sprout_fibertype group by entity_type,fiber_type) t1
  join item_template_master itm on itm.category_reference = t1.entity_type where '(' || itm.code::varchar || ')' || itm.other::varchar = t1.fiber_type
 
  )t group by t.entity_type)row_result ;
 
 
END;
$function$
;


-------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_nearest_sites(line_geom character varying, buffer double precision)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE  
  v_line geometry;
  v_buffer geometry;
  v_buffer_geojson json;
  v_sites json;
BEGIN  

	IF line_geom IS NULL OR trim(line_geom) = '' THEN
  RETURN;
END IF;
  -- Convert input line to geometry
  SELECT ST_GeomFromText('LINESTRING(' || line_geom || ')', 4326)
  INTO v_line;

  -- Create buffer (in meters)
  SELECT ST_Buffer(v_line::geography, buffer)::geometry
  INTO v_buffer;

  -- Convert buffer to GeoJSON
  SELECT ST_AsGeoJSON(v_buffer)::json
  INTO v_buffer_geojson;

  -- Get all POD sites within buffer
  SELECT json_agg(json_build_object('network_id', adp.network_id, 'site_name', adp.site_id , 'network_status' , adp.network_status))
  INTO v_sites
  FROM point_master pm  
  join att_details_pod adp on pm.system_id = adp.system_id and UPPER(pm.entity_type) = 'POD' and pm.status ='A'  and pm.network_status in('P','A') 
  WHERE ST_Within(pm.sp_geometry, v_buffer);

  -- Return as single JSON object
  RETURN NEXT json_build_object(
    'buffer_geometry', v_buffer_geojson,
    'sites', COALESCE(v_sites, '[]'::json)
  );

  RETURN;
END
$function$
;


--------------------------------------------------------------------------------------------------------------

