-- FUNCTION: public.fn_get_entity_along_direction(character varying, character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_entity_along_direction(character varying, character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_get_entity_along_direction(
	p_entity_types character varying,
	p_line_geom character varying,
	p_user_id integer)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
v_province_id integer;
v_region_id integer;
v_termination_required integer;
v_arow_network_id record;
-- Trench
v_arow_trench record;
-- Cable
v_arow_cable record;
--Duct
v_arow_duct record;
--Microduct
v_arow_microduct record;
--Manhole
v_arow_start_manhole_network_id record;
v_arow_end_manhole_network_id record;
v_arow_start_manhole record;
v_arow_end_manhole record;
--splice closure
v_arow_start_closure_network_id record;
v_arow_end_closure_network_id record;
v_arow_start_closure record;
v_arow_end_closure record;

--Geom
v_start_geom geometry;
v_end_geom geometry;
v_endlatlong character varying;
v_startlatlong character varying;

--CalculatedLength
v_measured_length double precision;
v_calculated_length double precision;
v_CableExtraLengthPercentage integer;
v_template boolean;
v_entites text;
v_status boolean;
v_message character varying;
CNT_CHK integer;
BEGIN
v_template=true;
v_entites='';
v_status=true;
SELECT id,region_id into v_province_id,v_region_id FROM PROVINCE_BOUNDARY WHERE ST_INTERSECTS(SP_GEOMETRY, ST_GEOMFROMTEXT('LINESTRING('||p_line_geom||')',4326)) AND ISVISIBLE=TRUE;

select value into v_termination_required from global_settings where key = 'isterminationpointenable';
--CalculatedLength
select ST_Length(ST_GeomFromText('LINESTRING('||p_line_geom||')',4326),false) into v_measured_length;
raise info 'v_measured_length:%',v_measured_length;

IF(v_termination_required>0) THEN
select value into v_CableExtraLengthPercentage from global_settings where key='CableExtraLengthPercentage';
IF EXISTS(select 1 from global_settings where upper(key)=upper('isExtraLengthRequired'))
then
v_calculated_length=v_measured_length+(((v_CableExtraLengthPercentage*v_measured_length)/100));
end if;
raise info 'v_calculated_length:%',v_calculated_length;
ELSE
v_calculated_length:=v_measured_length;
END IF;

SELECT split_part(p_line_geom, ',',1) into v_startlatlong;
SELECT regexp_replace(p_line_geom, '^.*,', '') into v_endlatlong;

select ST_GeomFromText('POINT('||v_startlatlong||')', 4326) into v_start_geom;
select ST_GeomFromText('POINT('||v_endlatlong||')', 4326) into v_end_geom;

--initialize the record set for manhole
select 0 as system_id,'' network_id,'' as entity into v_arow_start_manhole;
select 0 as system_id,'' network_id,'' as entity into v_arow_end_manhole ;

--initialize the record set for the splice closure

select 0 as system_id,'' network_id,'' as entity,0 as no_of_ports into v_arow_start_closure ;
select 0 as system_id,'' network_id,'' as entity,0 as no_of_ports into v_arow_end_closure;
if(POSITION(upper('Cable') in upper(p_entity_types))>0)
Then
	select * from fn_chk_layer_template_exist((select layer_id from layer_details where upper(layer_name)='CABLE'),p_user_id) into v_template;

	if((v_template) ='false')
	then
		v_entites:='Cable,';
		v_status=false;

	end if;
END IF;
IF(POSITION(upper('DUCT') in upper(p_entity_types))>0)
Then
	select * from fn_chk_layer_template_exist((select layer_id from layer_details where upper(layer_name)='DUCT'),p_user_id) into v_template;
	if((v_template) ='false')
	then
		v_entites:=v_entites||'Duct,';
		v_status=false;
	END IF;
END IF;
IF(POSITION(upper('TRENCH') in upper(p_entity_types))>0)
Then
	select * from fn_chk_layer_template_exist((select layer_id from layer_details where upper(layer_name)='TRENCH'),p_user_id) into v_template;
	if((v_template) ='false')
	then
		v_entites:=v_entites||'Trench';
		v_status=false;
	END IF;
END IF;

if(v_entites<>'')
Then
	select RTRIM(v_entites, ',') into v_entites;
	v_message:='Please fill '||v_entites||' templates first!';
	RETURN QUERY 
	select false as status, v_message::character varying as message;
ELSE

if(POSITION(upper('Cable') in upper(p_entity_types)) > 0 and v_termination_required > 0)
then
--Add Splice Closure

--START SPLICE CLOSURE
select * into v_arow_start_closure_network_id from fn_get_network_code('SpliceClosure','Point',0,'',v_startlatlong);

insert into att_details_spliceclosure(network_id,spliceclosure_name,latitude,longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,
no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,no_of_input_port,no_of_output_port,
is_used,is_virtual,ownership_type,bom_sub_category)
select v_arow_start_closure_network_id.network_code,v_arow_start_closure_network_id.network_code,st_y(v_start_geom),st_x(v_start_geom),v_province_id,v_region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,type,brand,model,construction,activation,accessibility,'A',p_user_id,now(),'P', v_arow_start_closure_network_id.parent_system_id, v_arow_start_closure_network_id.parent_network_id,
v_arow_start_closure_network_id.parent_entity_type,v_arow_start_closure_network_id.sequence_id,
no_of_ports,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false ,'Own','Proposed'
from item_template_spliceclosure where created_by = p_user_id order by modified_on desc limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port,province_id,region_id,'SpliceClosure' as entity into v_arow_start_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,common_name, network_status,no_of_ports,display_name)
values(v_arow_start_closure.system_id, 'SpliceClosure',st_x(v_start_geom),st_y(v_start_geom),'A',ST_GEOMFROMTEXT('POINT('||v_startlatlong||')',4326),now(),'Created', '',p_user_id,v_arow_start_closure_network_id.network_code, 'P',v_arow_start_closure.no_of_ports::character varying,fn_get_display_name(v_arow_start_closure.system_id,'SpliceClosure'));


perform (fn_geojson_update_entity_attribute(v_arow_start_closure.system_id::integer,'SpliceClosure'::character varying ,v_arow_start_closure.province_id::integer,0,false));

--END SPLICE CLOSURE
select * into v_arow_end_closure_network_id from fn_get_network_code('SpliceClosure','Point',0,'',v_endlatlong);

insert into att_details_spliceclosure(network_id,spliceclosure_name,latitude,longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,
no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,no_of_input_port,no_of_output_port,
is_used,is_virtual,ownership_type,bom_sub_category)
select v_arow_end_closure_network_id.network_code,v_arow_end_closure_network_id.network_code,st_y(v_end_geom),st_x(v_end_geom),v_province_id,v_region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,type,brand,model,construction,activation,accessibility,'A',p_user_id,now(),'P', v_arow_end_closure_network_id.parent_system_id, v_arow_end_closure_network_id.parent_network_id,v_arow_end_closure_network_id.parent_entity_type,v_arow_end_closure_network_id.sequence_id,
no_of_ports,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false ,'Own','Proposed'
from item_template_spliceclosure where created_by = p_user_id order by modified_on desc limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port,province_id,region_id,'SpliceClosure' as entity into v_arow_end_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,common_name, network_status,no_of_ports,display_name)
values(v_arow_end_closure.system_id, 'SpliceClosure',st_x(v_end_geom),st_y(v_end_geom),'A',ST_GEOMFROMTEXT('POINT('||v_endlatlong||')',4326),now(),'Created', ''
,p_user_id,v_arow_end_closure_network_id.network_code, 'P',v_arow_end_closure.no_of_ports::character varying,fn_get_display_name(v_arow_end_closure.system_id,'SpliceClosure'));

perform (fn_geojson_update_entity_attribute(v_arow_end_closure.system_id::integer,'SpliceClosure'::character varying ,v_arow_end_closure.province_id::integer,0,false));


end if;

if ((POSITION(upper('Trench') in upper(p_entity_types))> 0 or POSITION(upper('Duct') in upper(p_entity_types))> 0) and v_termination_required > 0 )
--Add Manhole
then

--START MANHOLE
select * into v_arow_start_manhole_network_id from fn_get_network_code('Manhole','Point',0,'',v_startlatlong);

INSERT INTO att_details_manhole(network_id, manhole_name, longitude, latitude, province_id,region_id,
specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
, project_id, planning_id, purpose_id, workorder_id,ownership_type,bom_sub_category)

SELECT v_arow_start_manhole_network_id.network_code,v_arow_start_manhole_network_id.network_code,st_x(v_start_geom),st_y(v_start_geom),v_province_id,v_region_id, specification, category,subcategory1, subcategory2,
subcategory3,item_code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
0,v_arow_start_manhole_network_id.parent_network_id,v_arow_start_manhole_network_id.parent_entity_type,v_arow_start_manhole_network_id.sequence_id, 0, 0, 0, 0 ,'Own','Proposed'
FROM item_template_manhole where created_by = p_user_id order by modified_on desc limit 1
RETURNING system_id,network_id,latitude,longitude,'Manhole' as entity into v_arow_start_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name)
values(v_arow_start_manhole.system_id, 'Manhole',st_x(v_start_geom)::double precision, st_y(v_start_geom)::double precision,
'A',v_start_geom,now(),'Created', '' ,p_user_id,v_arow_start_manhole.network_id, 'P',fn_get_display_name(v_arow_start_manhole.system_id,'Manhole'));

perform (fn_geojson_update_entity_attribute(v_arow_start_manhole.system_id::integer,'Manhole'::character varying ,v_arow_start_manhole.province_id::integer,0,false));


--END MANHOLE
select * into v_arow_end_manhole_network_id from fn_get_network_code('Manhole','Point',0,'',v_endlatlong);

INSERT INTO att_details_manhole(network_id, manhole_name, longitude, latitude, province_id,region_id,
specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
, project_id, planning_id, purpose_id, workorder_id,ownership_type,bom_sub_category)

SELECT v_arow_end_manhole_network_id.network_code,v_arow_end_manhole_network_id.network_code,st_x(v_end_geom),st_y(v_end_geom),v_province_id,v_region_id, specification, category,subcategory1, subcategory2,
subcategory3,item_code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
0,v_arow_end_manhole_network_id.parent_network_id,v_arow_end_manhole_network_id.parent_entity_type,v_arow_end_manhole_network_id.sequence_id, 0, 0, 0, 0, 'Own','Proposed'
FROM item_template_manhole where created_by = p_user_id order by modified_on desc limit 1
RETURNING system_id,network_id,latitude,longitude,'Manhole' as entity into v_arow_end_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name)
values(v_arow_end_manhole.system_id, 'Manhole',st_x(v_end_geom)::double precision, st_y(v_end_geom)::double precision,
'A',v_end_geom,now(),'Created', '' ,p_user_id,v_arow_end_manhole.network_id, 'P',fn_get_display_name(v_arow_end_manhole.system_id,'Manhole'));

perform (fn_geojson_update_entity_attribute(v_arow_end_manhole.system_id::integer,'Manhole'::character varying ,v_arow_end_manhole.province_id::integer,0,false));


end if;

if (POSITION(upper('Trench') in upper(p_entity_types)) > 0)
then

select * into v_arow_network_id from fn_get_line_network_code('Trench','','',p_line_geom,'OSP');

insert into att_details_trench(network_id,trench_name,a_system_id,a_location,a_entity_type,b_system_id,b_location,b_entity_type,trench_length,trench_width,trench_height,
customer_count,utilization,trench_type,no_of_ducts,sequence_id,network_status,status,pin_code,province_id,region_id,construction,activation,accessibility,specification,
category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,type,brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,mcgm_ward,
strata_type,parent_system_id,parent_network_id,parent_entity_type,is_used,ownership_type,bom_sub_category,trench_serving_type)

select v_arow_network_id.network_code,v_arow_network_id.network_code,v_arow_start_manhole.system_id,v_arow_start_manhole.network_id,v_arow_start_manhole.entity,
v_arow_end_manhole.system_id,v_arow_end_manhole.network_id,v_arow_end_manhole.entity,
ST_Length(ST_GeomFromText('LINESTRING('||p_line_geom||')',4326),false),trench_width,trench_height,0,0,trench_type,0,v_arow_network_id.sequence_id,'P','A',null,v_province_id,v_region_id,0,0,0,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,0,0,0,'',p_user_id,now(),0,0,0,0,'','',v_arow_network_id.parent_system_id,
v_arow_network_id.parent_network_id, v_arow_network_id.parent_entity_type,false,'Own','Proposed',trench_serving_type
from item_template_trench where created_by = p_user_id order by modified_on desc limit 1
RETURNING system_id,network_id,province_id into v_arow_trench;

INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name)
values(v_arow_trench.SYSTEM_ID,'Trench','A',ST_GeomFromText('LINESTRING('||p_line_geom||')',4326),'','',p_user_id,0,v_arow_trench.network_id,'P',false,fn_get_display_name(v_arow_trench.SYSTEM_ID,'Trench'));

perform (fn_geojson_update_entity_attribute(v_arow_trench.system_id::integer,'Trench'::character varying ,v_arow_trench.province_id::integer,0,false));


insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,is_termination_point)
values(v_arow_trench.system_id,v_arow_trench.network_id,'Trench',0,'','',now(),p_user_id,true),
(v_arow_trench.system_id,v_arow_trench.network_id,'Trench',0,'','',now(),p_user_id,true);

end if;

if (POSITION(upper('Duct') in upper(p_entity_types)) > 0)
then

select * into v_arow_network_id from fn_get_line_network_code('Duct','','',p_line_geom,'OSP');

insert into att_details_duct(network_id,duct_name,a_system_id,a_location,a_entity_type,b_system_id,b_location,b_entity_type,calculated_length,manual_length,sequence_id,network_status,
status,pin_code,province_id,region_id,utilization,no_of_cables,offset_value,construction,activation,accessibility,specification,category,subcategory1,subcategory2,
subcategory3,item_code,vendor_id,type,brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,inner_dimension,outer_dimension,
parent_system_id,parent_network_id,parent_entity_type,is_used,trench_id,ownership_type,bom_sub_category,duct_color)

select v_arow_network_id.network_code,v_arow_network_id.network_code,v_arow_start_manhole.system_id,v_arow_start_manhole.network_id,v_arow_start_manhole.entity,v_arow_end_manhole.system_id,v_arow_end_manhole.network_id,v_arow_end_manhole.entity,
ST_Length(ST_GeomFromText('LINESTRING('||p_line_geom||')',4326),false),v_calculated_length,v_arow_network_id.sequence_id,'P','A',null,
v_province_id,v_region_id,0,0,0,0,0,0,specification,category,subcategory1,subcategory2,subcategory3,item_code,
vendor_id,0,0,0,'',p_user_id,now(),0,0,0,0,0,0,v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,false,0,'Own'
,'Proposed','A' from item_template_duct where created_by = p_user_id order by modified_on desc limit 1
RETURNING system_id,network_id,province_id into v_arow_duct;

INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name)
values(v_arow_duct.SYSTEM_ID,'Duct','A',ST_GeomFromText('LINESTRING('||p_line_geom||')',4326),'','',p_user_id,0,v_arow_duct.network_id,'P',false,fn_get_display_name(v_arow_duct.SYSTEM_ID,'Duct'));

perform (fn_geojson_update_entity_attribute(v_arow_duct.system_id::integer,'Duct'::character varying ,v_arow_duct.province_id::integer,0,false));


insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,is_termination_point)
values(v_arow_duct.system_id,v_arow_duct.network_id,'Duct',0,'','',now(),p_user_id,true),
(v_arow_duct.system_id,v_arow_duct.network_id,'Duct',0,'','',now(),p_user_id,true);

end if;

if (POSITION(upper('Cable') in upper(p_entity_types)) > 0)
then
select * into v_arow_network_id from fn_get_line_network_code('Cable','','',p_line_geom,'OSP');
raise info 'network_code:%',v_arow_network_id.network_code;

INSERT INTO ATT_DETAILS_CABLE(source_ref_id,network_id,cable_name,a_location,b_location,total_core,no_of_tube,no_of_core_per_tube,cable_measured_length,cable_calculated_length,
cable_type,specification,category,subcategory1,subcategory2,subcategory3,item_code,
vendor_id,network_status,status,province_id,region_id,created_by,created_on,a_system_id,a_network_id,a_entity_type,
b_system_id,b_network_id,b_entity_type,sequence_id,cable_category,parent_system_id,parent_network_id,parent_entity_type,start_reading,
end_reading,structure_id,utilization,duct_id,trench_id,is_used,ownership_type,bom_sub_category,AUDIT_ITEM_MASTER_ID)

select 0,v_arow_network_id.network_code,v_arow_network_id.network_code,v_arow_start_closure.network_id,v_arow_end_closure.network_id,total_core,no_of_tube,no_of_core_per_tube,ST_Length(ST_GeomFromText('LINESTRING('||p_line_geom||')',4326),false),v_calculated_length,
cable_type,specification,category,subcategory1,subcategory2,subcategory3,item_code,
vendor_id,'P','A',v_province_id,v_region_id,p_user_id,now(),v_arow_start_closure.system_id,v_arow_start_closure.network_id,v_arow_start_closure.entity,v_arow_end_closure.system_id,v_arow_end_closure.network_id,v_arow_end_closure.entity,
v_arow_network_id.sequence_id,cable_category,v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
0,0,0,'L',0,0,true,'Own','Proposed',AUDIT_ITEM_MASTER_ID from item_template_cable where created_by = p_user_id order by modified_on desc limit 1
RETURNING system_id,network_id,no_of_tube,total_core,no_of_core_per_tube,province_id into v_arow_cable;

INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name)
values(v_arow_cable.SYSTEM_ID,'Cable','A',ST_GeomFromText('LINESTRING('||p_line_geom||')',4326),'','',p_user_id,0,v_arow_cable.network_id,'P',false,fn_get_display_name(v_arow_cable.SYSTEM_ID,'Cable'));

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id::integer,'Cable'::character varying ,v_arow_cable.province_id::integer,0,false));


insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,is_termination_point)
values(v_arow_cable.system_id,v_arow_cable.network_id,'Cable',0,'','',now(),p_user_id,true),
(v_arow_cable.system_id,v_arow_cable.network_id,'Cable',0,'','',now(),p_user_id,true);

SELECT COUNT(1) INTO CNT_CHK FROM ATT_DETAILS_CABLE_INFO WHERE CABLE_ID=v_arow_cable.SYSTEM_ID;
IF (CNT_CHK=0) THEN
PERFORM(FN_SET_CABLE_COLOR_INFO(v_arow_cable.SYSTEM_ID,P_USER_ID,v_arow_cable.NO_OF_TUBE,v_arow_cable.NO_OF_CORE_PER_TUBE));	
END IF;

end if;


RETURN QUERY SELECT true::boolean AS STATUS, 'Network Placed Successfully!'::CHARACTER VARYING AS MESSAGE;
END IF;
END
$BODY$;

ALTER FUNCTION public.fn_get_entity_along_direction(character varying, character varying, integer)
    OWNER TO postgres;
