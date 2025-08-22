CREATE OR REPLACE FUNCTION public.fn_network_planning_get_plan_bom_list(p_plan_name character varying, p_plan_mode character varying, p_cable_type character varying, is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_cable_length double precision, p_distance double precision, p_user_id integer, p_temp_plan_id integer, p_is_loop_require boolean, p_is_loop_update boolean, p_loop_length double precision, p_polepecvendor character varying, p_manholepecvendor character varying, p_scspecvendor character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
sql text;
v_line_total_length DOUBLE PRECISION;
v_point_entity character varying;
v_line_geom geometry;
v_total_point_entity integer;
v_is_osp_tp boolean;
v_total_line integer;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_cable_length double precision;
v_end_entity_count integer;
v_middle_entity_count integer; 
v_arrow_point record;
v_loop_length double precision;
v_outer_query text;
v_temp_planid integer;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
sql:='';
v_total_line:=0;
v_temp_planid:=0;

if(p_is_loop_update)
then
v_temp_planid:= p_temp_plan_id;
select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;

select count(plan_id)into v_total_point_entity from temp_auto_network_plan where plan_id=v_temp_planid and loop_length >0;

sql:=sql || 'select '||v_loop_length||' as length_qty,''Loop''::character varying as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
	raise info 'ssdfasdfql=%',sql;	
else

if(upper(p_cable_type)=upper('overhead'))
then
	v_point_entity:='Pole';
	else
	v_point_entity:='Manhole';
end if;

	
select is_osp_tp into v_is_osp_tp from vw_termination_point_master where tp_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure')and layer_id=(select layer_id from layer_details where layer_name='Cable');

  select validation.status,validation.message from  fn_network_planning_validate(p_plan_mode,p_cable_type,is_create_trench,
  is_create_duct,v_is_osp_tp,p_cable_length,v_point_entity,p_user_id,0,p_line_geom) as validation into V_IS_VALID,V_MESSAGE;

 IF(V_IS_VALID)
THEN

-- creating points entity according to line geom;
select * from fn_temp_network_planning(p_plan_mode,p_cable_type,is_create_trench,is_create_duct,p_line_geom,p_cable_length,p_distance,p_user_id,p_plan_name,'', '', '', 0, '', 0, '',p_temp_plan_id)into v_arrow_point;

v_temp_planid:=v_arrow_point.temp_plan_id;

-- count of middle entity
select count(plan_id)into v_middle_entity_count from temp_auto_network_plan where plan_id=v_arrow_point.temp_plan_id and is_middle_point='true';

-- end entity
select count(plan_id) into v_end_entity_count from temp_auto_network_plan where plan_id=v_arrow_point.temp_plan_id and is_middle_point='false';

-- total count of point entity
v_total_point_entity:=v_middle_entity_count+v_end_entity_count;

 select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
 select round(st_length(v_line_geom,false)::numeric, 6)::double precision into v_line_total_length;

-- get counts of cable 
 if(v_line_total_length<p_cable_length)
 then
 	v_total_line:=1;
 else
 	select CEIL(v_line_total_length/p_cable_length)::integer into v_total_line;		
 end if;

-- for cable
sql := 'select '||v_line_total_length||' as length_qty,* from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';

if(is_create_trench) 
then 
	sql:=sql || ' union select '||v_line_total_length||' as length_qty,* from 
	fn_network_planning_audit_template_detail(''trench'','||p_user_id||')';
end if;

if(is_create_duct) 
then 
	sql:=sql || ' union select '||v_line_total_length||' as length_qty,* from fn_network_planning_audit_template_detail(''duct'','||p_user_id||')';
end if;
 

if(upper(p_plan_mode)=upper('auto'))
then
	--sql:=sql || ' union select '||v_total_point_entity||' as length_qty,* from fn_network_planning_audit_template_detail('''||v_point_entity||''','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';	
sql := sql || ' UNION SELECT ' || v_total_point_entity || ' AS length_qty, * 
FROM fn_network_planning_audit_template_detail('
|| quote_literal(v_point_entity) || ',' 
|| p_user_id || ',' 
|| quote_literal(p_polepecvendor) || ',' 
|| quote_literal(p_manholepecvendor) || ',' 
|| quote_literal(p_scspecvendor) || ')';

else
	--sql:=sql || ' union select '||v_total_point_entity||' as length_qty,* from fn_network_planning_audit_template_detail('''||v_point_entity||''','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';
sql := sql || ' UNION SELECT ' || v_total_point_entity || ' AS length_qty, * 
FROM fn_network_planning_audit_template_detail('
|| quote_literal(v_point_entity) || ',' 
|| p_user_id || ',' 
|| quote_literal(p_polepecvendor) || ',' 
|| quote_literal(p_manholepecvendor) || ',' 
|| quote_literal(p_scspecvendor) || ')';

end if;

if(v_is_osp_tp)
then 
	sql:=sql || ' union select '||v_end_entity_count||' as length_qty,* from fn_network_planning_audit_template_detail(''spliceclosure'','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';
end if;


if(p_is_loop_require)
	then 
		if(p_is_loop_update)
		then 
		 select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;
		else
			v_loop_length := v_total_point_entity * p_loop_length;
		end if;
		
		sql:=sql || ' union select '||v_loop_length||' as length_qty,''Loop'' as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';

	end if;
end if;


end if;

raise info 'inner query =%',sql;

v_outer_query:= 'select '||V_IS_VALID||'::boolean AS is_template_extis,'||v_temp_planid||' as temp_plan_id,
 case when upper(entity_type)=''CABLE'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''DUCT'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''TRENCH'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
 when upper(entity_type)=''LOOP'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_point_entity||')::character varying else length_qty ::character varying end as length_qty,
(round((length_qty)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((length_qty)::numeric, 2))as amount,entity_type,geom_type,cost_per_unit,service_cost_per_unit from ('||sql||')e';


IF(V_IS_VALID)
then 

RETURN QUERY
EXECUTE 'select row_to_json(row) from (select * from ('||v_outer_query||')t order by geom_type) row';
else 

RETURN QUERY
EXECUTE 'select row_to_json(row) from (select '||V_IS_VALID||'::boolean AS is_template_extis, '''||V_MESSAGE||'''::CHARACTER VARYING AS msg) row';
end if;
            
 END
$function$
;

----------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_network_planning_save_auto_planning(p_plan_mode character varying, p_cable_type character varying, is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_cable_length double precision, p_distance double precision, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, p_end_point_type character varying, p_end_point_buffer double precision, p_edit_path character varying, p_end_point_entity_id integer, p_end_point_entity_type character varying, p_temp_plan_id integer, p_is_loop_required boolean, p_loop_length double precision, p_polespecvendor character varying, p_manholespecvendor character varying, p_scspecvendor character varying)
 RETURNS TABLE(status boolean, message character varying, plan_id integer)
 LANGUAGE plpgsql
AS $function$

DECLARE

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
v_total_record_temp integer;
v_counter_2 integer;
v_pole_spec_vendor TEXT[];
v_manhole_spec_vendor TEXT[];
v_sc_spec_vendor TEXT[];
BEGIN
v_counter_2=0;
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
v_counter:=0;
sql:='';
v_prev_qty_length:=0;
p_rfs_type:='A-RFS';
p_plan_id=0;

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

IF(coalesce(p_line_geom,'')='' )
then
V_IS_VALID=FALSE;
V_MESSAGE:='Line Geom can not be blank !';
end if;

IF(V_IS_VALID)
THEN
select is_osp_tp into v_is_osp_tp from vw_termination_point_master where tp_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure')and layer_id=(select layer_id from layer_details where layer_name='Cable');

select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
select st_length(v_line_geom,false)::double precision into v_line_total_length;

    v_pole_spec_vendor := string_to_array(p_polespecvendor, ',');
    v_manhole_spec_vendor := string_to_array(p_manholespecvendor, ',');
    v_sc_spec_vendor := string_to_array(p_scspecvendor, ',');
  
if(upper(p_cable_type)=upper('overhead'))then
v_point_entity:='Pole';
else
v_point_entity:='Manhole';
end if;

select validation.status,validation.message from fn_network_planning_validate(p_plan_mode,p_cable_type,is_create_trench,is_create_duct,v_is_osp_tp,p_cable_length,v_point_entity,p_user_id,p_end_point_buffer,p_line_geom) as validation into V_IS_VALID,V_MESSAGE;

end if;

IF(V_IS_VALID)
THEN

------------ Get all entity audit_item_master_id =================================================

select (select (select * from fn_get_template_detail(p_user_id,'SpliceClosure',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_sc ;
select (select (select * from fn_get_template_detail(p_user_id,v_point_entity,''))->>'audit_item_master_id')::integer into v_audit_item_master_id_point_entity ;
select (select (select * from fn_get_template_detail(p_user_id,'cable',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_cable ;
select (select (select * from fn_get_template_detail(p_user_id,'duct',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_duct ;
select (select (select * from fn_get_template_detail(p_user_id,'trench',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_trench;

----------------========================layerList--------------=================================
select array_to_string(array_agg(layer_id), ',') into v_layer_ids from layer_details
where
(true =true and upper(layer_name) =upper('cable'))
or (is_create_trench =true and upper(layer_name) =upper('trench'))
or (is_create_duct =true and upper(layer_name) =upper('duct'))
or (v_is_osp_tp =true and upper(layer_name) =upper('SpliceClosure'))
or (p_is_loop_required =true and upper(layer_name) =upper('Loop'))
or (true =true and upper(layer_name) =upper(''||v_point_entity||''));

------------------------- save plan ==================================================================================
INSERT INTO public.att_details_network_plan
(planid,layer_id,plan_name, start_point, end_point, cable_type, is_create_trench, is_create_duct, pole_manhole_distance, cable_length, created_by,
created_on,planning_mode,
end_point_type,end_point_buffer,end_point_entity,edit_path,is_loop_required,loop_length)
VALUES(p_temp_plan_id,v_layer_ids,p_plan_name, p_startpoint, p_endpoint,p_cable_type,is_create_trench, is_create_duct, p_distance, p_cable_length, p_user_id, now(),p_plan_mode,
p_end_point_type,p_end_point_buffer,p_end_point_entity_id,p_edit_path,p_is_loop_required,p_loop_length)
RETURNING planid into p_plan_id;

--select count(1) from temp_auto_network_plan

FOR v_temp_point_entity IN select * from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id order by system_id
LOOP

v_counter:=v_counter+1;

v_current_point_geom=ST_LineInterpolatePoint(v_line_geom, v_temp_point_entity.fraction);

select count(1) from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point =false into v_total_record_temp;

v_loop_length := v_temp_point_entity.loop_length;
v_point_entity_type := v_temp_point_entity.entity_type;
select * into v_region_province from fn_getregionprovince(substring(left(St_astext(v_current_point_geom),-1),7),'Point');

update temp_auto_network_plan set province_id =v_region_province.province_id,region_id=v_region_province.region_id,created_by=p_user_id where system_id=v_temp_point_entity.system_id;

-- if we need to create entity in middle then only middle entity is created
if(v_temp_point_entity.is_middle_point=true)
then

SELECT * into v_arow_network_id FROM fn_get_clone_network_code( v_point_entity, 'Point', substring(left(St_astext(v_current_point_geom),-1),7),0,'')
as clone order by clone.status desc limit 1;

if(upper(v_point_entity)=upper('Manhole'))
then
INSERT INTO att_details_manhole(is_visible_on_map,audit_item_master_id,ownership_type,network_id, manhole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
,project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',v_arow_network_id.message,v_arow_network_id.message,
st_x(v_current_point_geom),st_y(v_current_point_geom),
v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,
code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',v_arow_network_id.o_p_system_id,
v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0, 0, 0, 0,'planning',p_plan_id
FROM item_template_master where id = v_manhole_spec_vendor[1]::integer and vendor_id = v_manhole_spec_vendor[3]::integer and code= v_manhole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_middle_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name)
values(v_arow_middle_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A',v_current_point_geom,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',
fn_get_display_name(v_arow_middle_manhole.system_id, v_point_entity));

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Manhole'::character varying ,v_region_province.province_id,0,false));

else

INSERT INTO att_details_pole(is_visible_on_map,audit_item_master_id,ownership_type,pole_type,network_id, pole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code,
vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
, project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',specify_type,v_arow_network_id.message,v_arow_network_id.message,
st_x(v_current_point_geom),st_y(v_current_point_geom),
v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,
code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
v_arow_network_id.o_p_system_id,v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id,
0, 0, 0, 0 ,'planning',p_plan_id
FROM item_template_master where id = v_pole_spec_vendor[1]::integer and vendor_id = v_pole_spec_vendor[3]::integer and code = v_pole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_middle_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_middle_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A',v_current_point_geom,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',
fn_get_display_name(v_arow_middle_manhole.system_id, v_point_entity),'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Pole'::character varying ,v_region_province.province_id,0,false));
end if;

else
SELECT * into v_arow_network_id FROM fn_get_clone_network_code( v_point_entity, 'Point', substring(left(St_astext(v_current_point_geom),-1),7),0,'')
as clone order by clone.status desc limit 1;

if(upper(v_point_entity)=upper('Manhole'))
then
INSERT INTO att_details_manhole(is_visible_on_map,audit_item_master_id,ownership_type,network_id, manhole_name, longitude, latitude,
province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, type, brand,
model, construction, activation,accessibility, status,created_by, created_on,network_status,parent_system_id, parent_network_id,
parent_entity_type, sequence_id,project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',v_arow_network_id.message,v_arow_network_id.message,st_x(v_current_point_geom),
st_y(v_current_point_geom),v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,code,vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',v_arow_network_id.o_p_system_id,
v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0, 0, 0, 0,'planning',p_plan_id
FROM item_template_master where id = v_manhole_spec_vendor[1]::integer and vendor_id = v_manhole_spec_vendor[3]::integer and code = v_manhole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A',v_current_point_geom,now(),'Created', '' ,p_user_id,v_arow_manhole.network_id,
'P',fn_get_display_name(v_arow_manhole.system_id, v_point_entity),'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_manhole.system_id,'Manhole'::character varying ,v_region_province.province_id,0,false));

else
INSERT INTO att_details_pole(is_visible_on_map,audit_item_master_id,ownership_type,pole_type,network_id, pole_name, longitude,
latitude, province_id,
region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, type, brand, model, construction,
activation,accessibility, status,created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type,
sequence_id, project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',specify_type,v_arow_network_id.message,v_arow_network_id.message,
st_x(v_current_point_geom),st_y(v_current_point_geom),v_region_province.province_id,v_region_province.region_id, specification, category_reference,
 subcategory_1, subcategory_2, subcategory_3,code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
v_arow_network_id.o_p_system_id,v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0,
0, 0, 0,'planning',p_plan_id FROM item_template_master where id = v_pole_spec_vendor[1]::integer and vendor_id = v_pole_spec_vendor[3]::integer and code = v_pole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude
into v_arow_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A',v_current_point_geom,now(),'Created', '' ,p_user_id,v_arow_manhole.network_id,
'P',fn_get_display_name(v_arow_manhole.system_id, v_point_entity),'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_manhole.system_id,'Pole'::character varying, v_region_province.province_id,0,false));
end if;

--------- if need to create spliceclosure then we run this
if(v_is_osp_tp) then

select * into v_arow_network_id from fn_get_network_code('SpliceClosure','Point',0,'',substring(left(St_astext(v_current_point_geom),-1),7));

insert into att_details_spliceclosure(is_visible_on_map,audit_item_master_id,ownership_type,network_id,spliceclosure_name,latitude,
longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,
no_of_input_port,no_of_output_port,is_used,is_virtual,source_ref_type,source_ref_id)

select true,v_audit_item_master_id_sc,'OWN',v_arow_network_id.network_code,v_arow_network_id.network_code,st_y(v_current_point_geom),
st_x(v_current_point_geom),v_region_province.province_id,v_region_province.region_id,
specification, category_reference, subcategory_1, subcategory_2, subcategory_3,code, vendor_id, 0,0,0,0,0,0,'A',p_user_id,now(),
'P',v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
v_arow_network_id.sequence_id,no_of_port,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false,'planning',p_plan_id
from item_template_master where id = v_sc_spec_vendor[1]::integer and vendor_id = v_sc_spec_vendor[3]::integer and code = v_sc_spec_vendor[2] ::varchar limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port into v_arow_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
created_by,common_name, network_status,no_of_ports,display_name,source_ref_type,source_ref_id)
values(v_arow_closure.system_id, 'SpliceClosure',st_x(v_current_point_geom),st_y(v_current_point_geom),'A',
ST_GEOMFROMTEXT('POINT('||st_x(v_current_point_geom)||' '||st_y(v_current_point_geom)||')',4326),now(),'Created', ''
,p_user_id,v_arow_network_id.network_code, 'P',v_arow_closure.no_of_ports::character varying,
fn_get_display_name(v_arow_closure.system_id, 'SpliceClosure'),'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_closure.system_id,'SpliceClosure'::character varying ,v_region_province.province_id,0,false));

perform(fn_bulk_insert_port_info(v_arow_closure.no_of_ports,v_arow_closure.no_of_ports,'SpliceClosure',
v_arow_closure.system_id,v_arow_closure.network_id,p_user_id));

insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
entity_type,created_on,created_by,associated_display_name,entity_display_name)
values('SpliceClosure',v_arow_closure.system_id,v_arow_closure.network_id,v_arow_manhole.network_id,v_arow_manhole.system_id,v_point_entity,now(),p_user_id,
fn_get_display_name(v_arow_closure.system_id,'SpliceClosure'),fn_get_display_name(v_arow_manhole.system_id,v_point_entity));

end if;

-- update values in temp table related to loop

if(v_counter >1 and v_temp_point_entity.is_middle_point=false)

then

select ST_GeomFromText('POINT('||v_arow_prev_closure.longitude||' '||v_arow_prev_closure.latitude||')',4326) into v_sub_line_start_point;
select ST_GeomFromText('POINT('||v_arow_closure.longitude||' '||v_arow_closure.latitude||')',4326) into v_sub_line_end_point;
select ST_LineSubstring(v_line_geom, ST_LineLocatePoint(v_line_geom, v_sub_line_start_point),
ST_LineLocatePoint(v_line_geom, v_sub_line_end_point)) into v_sub_line_geom;
select substring(left(st_astext(v_sub_line_geom),-1),12) into v_sub_line;

if(is_create_trench)
then
select * into v_arow_trench from fn_network_planning_auto_create_trench(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,v_audit_item_master_id_trench)
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_trench.system_id,'Trench'::character varying ,v_region_province.province_id,0,false));

--- update trench id in temp table related to point_entity
update temp_auto_network_plan tp set trench_id=v_arow_trench.SYSTEM_ID,trench_network_id=v_arow_trench.network_id
where tp.system_id <= v_temp_point_entity.system_id and trench_id=0 and tp.plan_id=p_temp_plan_id;
end if;

if(is_create_duct)then

if(is_create_trench)then

select * into v_arow_duct from fn_network_planning_auto_create_duct(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,
v_audit_item_master_id_duct,v_arow_trench.system_id,v_arow_trench.network_id)
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_duct.system_id,'Duct'::character varying ,v_region_province.province_id,0,false));

else

select * into v_arow_duct from fn_network_planning_auto_create_duct(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,
v_audit_item_master_id_duct,0,'')
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_duct.system_id,'Duct'::character varying ,v_region_province.province_id,0,false));

end if;

--- update duct id in temp table related to point_entity
update temp_auto_network_plan tp set duct_id=v_arow_duct.SYSTEM_ID,duct_network_id=v_arow_duct.network_id
where tp.system_id <= v_temp_point_entity.system_id and duct_id=0 and tp.plan_id=p_temp_plan_id;
end if;

v_counter_2=v_counter_2+1;

--CREATE THE CABLE acco to duct

if(is_create_duct)
then

select * into v_arow_cable from fn_network_planning_auto_create_cable(v_arow_prev_closure.system_id,
'SpliceClosure'::character varying,v_arow_prev_closure.network_id,
v_arow_closure.system_id,'SpliceClosure'::character varying,v_arow_closure.network_id,v_sub_line,
--(v_arow_htb.longitude||' '||v_arow_htb.latitude||','||v_arow_closure.longitude||' '||v_arow_closure.latitude),
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,500,v_audit_item_master_id_cable,p_plan_id,
p_cable_type,v_arow_duct.system_id,v_arow_duct.network_id)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));

if(v_counter_2 = 1)
then
v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;

else
if(v_arow_prev_cable is not null)
then
insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,
destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,created_by,splicing_source,
equipment_system_id,equipment_network_id,equipment_entity_type)
values(v_arow_prev_cable.system_id,v_arow_prev_cable.network_id,'Cable',1,v_arow_cable.system_id,v_arow_cable.network_id,'Cable',1,false,true,
p_user_id,'PROVISIONNING',
v_arow_prev_closure.system_id, v_arow_prev_closure.network_id,'SpliceClosure');
end if;
v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;

end if;
else

select * into v_arow_cable from fn_network_planning_auto_create_cable(v_arow_prev_closure.system_id,
'SpliceClosure'::character varying,v_arow_prev_closure.network_id,
v_arow_closure.system_id,'SpliceClosure'::character varying,v_arow_closure.network_id,v_sub_line,
--(v_arow_htb.longitude||' '||v_arow_htb.latitude||','||v_arow_closure.longitude||' '||v_arow_closure.latitude),
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,500,v_audit_item_master_id_cable,p_plan_id,
p_cable_type,0,'')
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));

if(v_counter_2 = 1)
then

v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
raise info 'v_arow_prev_cable%',v_arow_prev_cable;
else
if(v_arow_prev_cable is not null)
then
insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,
destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,created_by,splicing_source,
equipment_system_id,equipment_network_id,equipment_entity_type)
values(v_arow_prev_cable.system_id,v_arow_prev_cable.network_id,'Cable',1,v_arow_cable.system_id,v_arow_cable.network_id,'Cable',1,false,true,
p_user_id,'PROVISIONNING',
v_arow_prev_closure.system_id, v_arow_prev_closure.network_id,'SpliceClosure');
end if;
v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
end if;
end if;

PERFORM(fn_network_planning_associate_end_entity(false,'',v_arow_prev_manhole.system_id,v_point_entity,v_arow_prev_manhole.network_id
,v_arow_manhole.system_id,v_point_entity,v_arow_manhole.network_id,'Cable',v_arow_cable.system_id,v_arow_cable.network_id,p_user_id,p_temp_plan_id));
--

PERFORM(fn_isp_create_OSP_Cable(v_arow_cable.SYSTEM_ID));

--- update cable id in temp table related to point_entity
update temp_auto_network_plan tp set cable_id=v_arow_cable.SYSTEM_ID,cable_network_id=v_arow_cable.network_id
where tp.system_id <= v_temp_point_entity.system_id and cable_id=0 and tp.plan_id=p_temp_plan_id;
--and loop_length !=0;

end if;

end if;

if(v_temp_point_entity.is_middle_point=true)
then
select display_name into v_a_display_value from point_master where system_id=v_arow_middle_manhole.system_id and upper(entity_type)=upper(v_point_entity);

update temp_auto_network_plan set entity_type=v_point_entity,
entity_network_id=v_arow_middle_manhole.network_id,entity_system_id=v_arow_middle_manhole.system_id,display_name=v_a_display_value
where system_id=v_temp_point_entity.system_id and v_temp_point_entity.is_middle_point=true;
else
select display_name into v_a_display_value from point_master where system_id=v_arow_manhole.system_id and upper(entity_type)=upper(v_point_entity);

update temp_auto_network_plan set entity_type=v_point_entity,
entity_network_id=v_arow_manhole.network_id,entity_system_id=v_arow_manhole.system_id,display_name=v_a_display_value
where system_id=v_temp_point_entity.system_id and v_temp_point_entity.is_middle_point=false;
end if;

if(v_counter >1 and v_temp_point_entity.is_middle_point=false)
then

PERFORM(fn_network_planning_loop_entity_testarv(v_arow_cable.SYSTEM_ID,p_line_geom));
end if;

v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
end loop;

---------------------------associate cables to middle point entity like pole and manhole
if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then
PERFORM(fn_network_planning_associate_end_entity(true,'Cable',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

-- ---------------------------associate cables to middle point entity like pole and manhole
if(is_create_duct)
then

-- if middle entity exist then middle entity associate
if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then

PERFORM(fn_network_planning_associate_end_entity(true,'Duct',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

end if;

-- ---------------------------associate cables to middle point entity like pole and manhole
if(is_create_trench)
then

if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then

PERFORM(fn_network_planning_associate_end_entity(true,'Trench',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

end if;

end if;

-- delete the temp_auto_network_plan

-- delete from temp_auto_network_plan t where t.plan_id=p_temp_plan_id;

IF((SELECT COUNT(1) FROM temp_connection)>0)
THEN
perform(fn_auto_provisioning_save_connections());

END IF;

--CUSTOMER AND SPLITTER MAPPING
IF(V_IS_VALID)
THEN
V_MESSAGE:='Network plan processed successfully!';
END IF;

RETURN QUERY SELECT V_IS_VALID::boolean AS STATUS, V_MESSAGE::CHARACTER VARYING AS MESSAGE, p_plan_id:: integer as plan_id;

END
$function$
;

---------------------------------------------------------------------------------------------------------