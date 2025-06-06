1. add moduleName in module_master

INSERT INTO public.module_master
( module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES( 'BackBone Planning', 'BackBone Planning', NULL, NULL, NULL, now(), 0 , now(), 'Web', true, 'BBPL', 0, 16, false, NULL, NULL);

--------------------------------------------------------------------------------------------------------------------------------------

2. add role_id into role_module_mapping( only add for 2 if want to show for any other than add with another role_id)

INSERT INTO public.role_module_mapping
(role_id, module_id)
VALUES(2, 541);

--------------------------------------------------------------------------------------------------------------------------------------

3. add User_id Into user_module_mapping( only add for 5 if want to show for any other than add with another user_id)
INSERT INTO public.user_module_mapping
( user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES( 5, 541, 5, now(), 0, NULL);

------------------------------------------------------------------------------------------------------------------

-- public.backbone_plan_details definition

-- Drop table

-- DROP TABLE public.backbone_plan_details;

CREATE TABLE public.backbone_plan_details (
	plan_id serial4 NOT NULL,
	plan_name varchar NOT NULL,
	start_point varchar NOT NULL,
	end_point varchar NOT NULL,
	cable_type varchar NULL,
	is_create_trench bool NULL,
	is_create_duct bool NULL,
	pole_distance float8 NULL,
	cable_length float8 NULL,
	created_by int4 NULL,
	created_on timestamp NULL DEFAULT now(),
	modified_by int4 NULL,
	modified_on timestamp NULL DEFAULT now(),
	region_id int4 NULL DEFAULT 0,
	province_id int4 NULL DEFAULT 0,
	fiber_type varchar NULL,
	sprout_fiber_type varchar NULL,
	backbone_fiber_type varchar NULL,
	status varchar NULL,
	manhole_distance float8 NULL,
	threshold float8 NULL
);

-- Permissions

ALTER TABLE public.backbone_plan_details OWNER TO postgres;
GRANT ALL ON TABLE public.backbone_plan_details TO postgres;

----------------------------------------------------------------------------------

-- public.backbone_plan_network_details definition

-- Drop table

-- DROP TABLE public.backbone_plan_network_details;

CREATE TABLE public.backbone_plan_network_details (
	system_id serial4 NOT NULL,
	plan_id int4 NULL,
	entity_type varchar(100) NULL,
	entity_network_id varchar(100) NULL,
	longitude float8 NULL,
	latitude float8 NULL,
	sp_geometry public.geometry NULL,
	province_id int4 NULL,
	region_id int4 NULL,
	created_by int4 NULL DEFAULT 0,
	line_sp_geometry public.geometry NULL,
	is_middle_point bool NULL,
	fraction float8 NULL,
	cable_id int4 NULL DEFAULT 0,
	entity_system_id int4 NULL,
	display_name varchar NULL,
	cable_network_id varchar NULL,
	duct_id int4 NULL DEFAULT 0,
	trench_id int4 NULL DEFAULT 0,
	trench_network_id varchar NULL,
	duct_network_id varchar NULL,
	fiber_type varchar NULL,
	intersect_line_geom text NULL,
	site_geom text NULL,
	cable_length float8 NULL
);

-- Permissions

ALTER TABLE public.backbone_plan_network_details OWNER TO postgres;
GRANT ALL ON TABLE public.backbone_plan_network_details TO postgres;

-----------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_list(p_user_id integer, p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_totalrecords integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
    sql TEXT;
    SqlQueryCnt TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;

BEGIN

RAISE INFO '%', sql;
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '||CASE WHEN (P_SORTCOLNAME) ='' THEN 'edf.created_on' else P_SORTCOLNAME end||' ' || CASE WHEN (P_SORTTYPE) = '' THEN 'desc' ELSE P_SORTTYPE END||') AS S_No,um.user_name AS created_by_text, edf.* FROM backbone_plan_details edf
LEFT JOIN user_master um ON edf.created_by = um.user_id where ( edf.status = ''Success'' and edf.created_by='||p_user_id||') ';

IF (upper(p_searchtext) != '') THEN
-- change filter to display_name
	sql:= sql ||'AND upper(plan_name) LIKE upper(''%'||p_searchtext||'%'')';
END IF;

raise info 'sql %' ,sql;

SqlQueryCnt:= 'SELECT COUNT(1)  FROM ('||sql||') as a';
 EXECUTE   SqlQueryCnt INTO P_TOTALRECORDS;
 IF((P_PAGENO) <> 0) THEN
  
    StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
    EndSNo:= P_PAGENO * P_PAGERECORD;
    sql:= 'SELECT '||P_TOTALRECORDS||' as totalRecords, *
                
		FROM (' || sql || ' order by '|| CASE WHEN (P_SORTCOLNAME) ='' THEN 'edf.created_on' ELSE P_SORTCOLNAME END||' '||CASE WHEN (p_sorttype) ='' THEN 'desc' ELSE p_sorttype END ||') T               
                WHERE S_No BETWEEN ' || StartSNo || ' AND 100'; 
                -- WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 
                
     ELSE
	sql:= 'SELECT '||P_TOTALRECORDS||' as totalRecords, * FROM (' || sql || ' order by '||CASE WHEN (P_SORTCOLNAME) ='' THEN 'edf.created_on' ELSE P_SORTCOLNAME END||' '|| CASE WHEN (p_sorttype) ='' THEN 'desc' ELSE p_sorttype END ||') T';                  
  END IF; 


RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';


END ;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_get_plan_list(int4, varchar, varchar, int4, int4, varchar, varchar, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_get_plan_list(int4, varchar, varchar, int4, int4, varchar, varchar, int4) TO postgres;


---------------------------------------------------------------------------------------------------

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

select common_name as network_id,entity_type ,system_id  into v_arow_prev_closure from point_master pm where entity_type = 'POD' and common_name = startpoint_networkId ; 

--------------   start backbone data query  -----------------------------------------
FOR v_temp_point_entity IN select * from backbone_plan_network_details tp where tp.plan_id=p_plan_id and entity_type in ('Pole','Manhole') order by system_id
LOOP	
------------ Get all entity audit_item_master_id =================================================
if (v_temp_point_entity.entity_network_id = endpoint_network_id)
then 
select common_name as network_id,entity_type ,system_id  into v_arow_prev_closure from point_master pm where entity_type = 'POD' and common_name = endpoint_network_id and v_temp_point_entity.entity_network_id = (select entity_network_id from backbone_plan_network_details ff where ff.plan_id = p_plan_id order by system_id desc limit 1) ; 
 end if;

select (select (select * from fn_get_template_detail(p_user_id,'SpliceClosure',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_sc ;
select (select (select * from fn_get_template_detail(p_user_id,v_temp_point_entity.entity_type,''))->>'audit_item_master_id')::integer into v_audit_item_master_id_point_entity ;
select (select (select * from fn_get_template_detail(p_user_id,'duct',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_duct ;
select (select (select * from fn_get_template_detail(p_user_id,'trench',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_trench;

select * into v_region_province from fn_getregionprovince(substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),'Point');
RAISE INFO 'v_region_province 1 : %', v_region_province;

update backbone_plan_network_details set province_id =v_region_province.province_id,region_id=v_region_province.region_id where system_id=v_temp_point_entity.system_id;


RAISE INFO 'v_region_province 1: %', v_region_province;

if (upper(v_temp_point_entity.entity_type)=upper('Manhole'))
then

SELECT * into v_arow_network_id FROM fn_get_clone_network_code( v_temp_point_entity.entity_type, 'Point', substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),0,'');

RAISE INFO 'v_arow_network_id m1: %', v_region_province;

IF COALESCE(v_temp_point_entity.entity_network_id, '') != endpoint_network_id 
then
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

end if;
--------------------- insert cable --------------------------
  select substring(left(St_astext(v_temp_point_entity.line_sp_geometry),-1),12) ::character varying INTO v_geom_str;

select * into v_arow_cable from fn_backbone_auto_create_cable(v_arow_prev_closure.system_id::integer,
v_arow_prev_closure.entity_type::character varying,v_arow_prev_closure.network_id ::character varying,
v_arow_closure.system_id::integer,v_arow_closure.entity_type::character varying,v_arow_closure.network_id ::character varying, 
v_geom_str ,
0,v_region_province.region_id :: integer,v_region_province.province_id,p_user_id,p_rfs_type::character varying,500,v_temp_point_entity.fiber_type,p_plan_id,
'Underground'::character varying,0,''::character varying)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));

if v_arow_prev_cable is not null then
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


else

RAISE INFO 'pole p1 : %', v_arow_network_id;

IF COALESCE(v_temp_point_entity.entity_network_id, '') != endpoint_network_id
then
RAISE INFO 'v_arow_network_id p1 : %', v_arow_network_id;
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
--------------------- insert cable --------------------------
   select substring(left(St_astext(v_temp_point_entity.line_sp_geometry),-1),12) ::character varying INTO v_geom_str;

select * into v_arow_cable from fn_backbone_auto_create_cable(v_arow_prev_closure.system_id::integer,
v_arow_prev_closure.entity_type::character varying,v_arow_prev_closure.network_id::character varying,
v_arow_closure.system_id::integer,v_arow_closure.entity_type::character varying,v_arow_closure.network_id::character varying, 
v_geom_str,
0,v_region_province.region_id::integer ,v_region_province.province_id,p_user_id,p_rfs_type ::character varying,500,v_temp_point_entity.fiber_type,p_plan_id,
'Overhead'::character varying,0,''::character varying)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));


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

end if;

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
end loop;
--------------   end sproutplan data query  -----------------------------------------

if ((SELECT COUNT(1) FROM temp_connection)>0)
then
 
perform(fn_auto_provisioning_save_connections());

END IF;
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

-- Permissions

ALTER FUNCTION public.fn_backbone_save_plan(bool, bool, varchar, float8, int4, varchar, varchar, varchar, int4, varchar, varchar) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_save_plan(bool, bool, varchar, float8, int4, varchar, varchar, varchar, int4, varchar, varchar) TO postgres;

-------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_delete_plan(p_plan_id integer, p_user_id integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
declare 

v_status boolean;
v_message character varying;
v_Asbuilt_count integer;
BEGIN
v_status:=true;
v_message:='Success';
v_Asbuilt_count:=0;

select SUM(count) into v_Asbuilt_count from (
select count(1) from att_details_cable where source_ref_id=''||p_plan_id||'' and lower(source_ref_type)='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A'))row;

if(v_Asbuilt_count > 0)
then 
	v_status:=false;
	v_message:='Plan Can Not Be Deleted Because Some Entity is AsBuild !';
else
	----cable---
	delete from line_master where system_id in(
	select system_id from att_details_cable where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('cable');

	delete from att_details_cable where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';																								----duct---
	delete from line_master where system_id in(
	select system_id from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('duct');

	delete from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';	

	----trench---
	delete from line_master where system_id in(
	select system_id from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('trench');

	delete from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';	

	----trench---
	delete from line_master where system_id in(
	select system_id from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('spliceclosure');

	delete from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';

	----manhole---
	delete from line_master where system_id in(
	select system_id from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('manhole');

	delete from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';

	----pole---
	delete from point_master where system_id in(
	select system_id from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('pole');

	delete from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';

	-- delete from temp table
	delete from backbone_plan_network_details where plan_id =p_plan_id;

	--Plan deleted
	delete from backbone_plan_details where plan_id=p_plan_id;
	
											
	v_status=true;
	v_message= '[SI_OSP_GBL_GBL_GBL_289]'; 
end if;

return query select v_status,v_message::character varying;
END
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_delete_plan(int4, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_delete_plan(int4, int4) TO postgres;


----------------------------------------------------------------------------------------------

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
  SELECT json_agg(json_build_object('common_name', common_name, 'display_name', display_name, 'geometry' ,  latitude ||' '|| longitude , 'network_status' , network_status))
  INTO v_sites
  FROM point_master lm  
  WHERE UPPER(lm.entity_type) = 'POD'
    AND ST_Within(lm.sp_geometry, v_buffer);

  -- Return as single JSON object
  RETURN NEXT json_build_object(
    'buffer_geometry', v_buffer_geojson,
    'sites', COALESCE(v_sites, '[]'::json)
  );

  RETURN;
END
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_get_nearest_sites(varchar, float8) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_get_nearest_sites(varchar, float8) TO postgres;


-----------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_draft_network(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, p_sprout_fiber_type character varying, p_backbone_fiber_type character varying, p_nearest_sites character varying, p_pole_span double precision, p_manhole_span double precision, v_buffer double precision, start_site_network_id character varying, end_site_network_id character varying, p_threshold double precision)
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
	
    INSERT INTO backbone_plan_details(plan_name, start_point, end_point, is_create_trench, is_create_duct, pole_distance, created_by, sprout_fiber_type, backbone_fiber_type)
    VALUES (p_plan_name, p_startpoint, p_endpoint, is_create_trench, is_create_duct, p_pole_span, p_user_id, p_sprout_fiber_type, p_backbone_fiber_type)
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
              AND entity_type = 'Restricted_Area'
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
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction
            ) VALUES (
                v_plan_id, 'Manhole', ST_X(ST_EndPoint(line_geom)), ST_Y(ST_EndPoint(line_geom)), ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction
            );

        END IF;


        -- Move to next step
        current_fraction := current_fraction + step_fraction;

        -- Exit safety
        EXIT WHEN current_fraction >= 1.0;
    END LOOP;  
   
   update backbone_plan_network_details set entity_network_id = START_SITE_NETWORK_ID where 
   plan_id = v_plan_id and fraction =0;
  
      update backbone_plan_network_details set entity_network_id = END_SITE_NETWORK_ID where plan_id = v_plan_id and  system_id = (select t.system_id from backbone_plan_network_details t order by t.system_id desc limit 1);
      
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
        plan_id, entity_type, created_by, fiber_type,intersect_line_geom,site_geom,entity_network_id 
    )
    VALUES (
        v_plan_id,'Cable', p_user_id, p_sprout_fiber_type,
         round(ST_Y(v_sprout_line_geom)::numeric, 6)::character varying || ',' || round(ST_X(v_sprout_line_geom)::numeric, 6)::character varying ,
   round(ST_Y(v_site_geom.sp_geometry)::numeric, 6)::character varying || ',' || round(ST_X(v_site_geom.sp_geometry)::numeric, 6)::character varying,
   v_site_geom.common_name 
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

-- Permissions

ALTER FUNCTION public.fn_backbone_draft_network(bool, bool, varchar, int4, varchar, varchar, varchar, varchar, varchar, varchar, float8, float8, float8, varchar, varchar, float8) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_draft_network(bool, bool, varchar, int4, varchar, varchar, varchar, varchar, varchar, varchar, float8, float8, float8, varchar, varchar, float8) TO postgres;


------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_backbone_update_sprout_network(p_linegeom character varying, p_systemid integer, p_cablelength double precision)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
   
begin
	
   update backbone_plan_network_details set line_sp_geometry = ST_GeomFromText(concat('LINESTRING(', p_linegeom, ')'), 4326), cable_length = p_cablelength where system_id =  p_systemId;
 
  return ;
END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_update_sprout_network(varchar, int4, float8) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_update_sprout_network(varchar, int4, float8) TO postgres;


-------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_bom(p_plan_id integer, p_user_id integer, p_backbone_fiber_type character varying, p_sprout_fibertype character varying, p_backbone_line_geom character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
 v_line_geom geometry;
 v_line_geom_length double precision;
begin
	
select ST_GeomFromText(concat('LINESTRING(', p_backbone_line_geom, ')'), 4326) into v_line_geom;
select st_length(v_line_geom,false) into v_line_geom_length;

return query
 select row_to_json(row_result) from (select t1.entity_type,t1.qty as cable_Length_qty,itm.cost_per_unit ,itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost  from (select entity_type ,count(*) as qty from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type in ('Pole') group by entity_type) t1
  join item_template_master itm on itm.category_reference = t1.entity_type where itm.cost_per_unit <> 0 and  itm.service_cost_per_unit <> 0
  
  UNION all
  
 select t1.entity_type,t1.qty,itm.cost_per_unit ,itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost  from (select entity_type ,count(*) as qty from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type in ('Manhole') group by entity_type) t1
  join item_template_master itm on itm.category_reference = t1.entity_type where itm.cost_per_unit <> 0 and  itm.service_cost_per_unit <> 0 
 
   UNION ALL
  
  select
  t.entity_type,
   round(SUM(t.qty)::numeric,3) AS total_qty,
  SUM(t.cost_per_unit) AS total_cost_per_unit,
  SUM(t.service_cost_per_unit) AS total_service_cost_per_unit,
  round(SUM(t.total_cost)::numeric,3) AS grand_total_cost
FROM (select t1.entity_type,t1.qty,itm.cost_per_unit ,itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost  from (select 'Cable' as entity_type,v_line_geom_length as qty ,fiber_type from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type <> 'Cable' and fiber_type = p_backbone_fiber_type group by entity_type,fiber_type) t1
  join item_template_master itm on itm.category_reference = t1.entity_type where '(' || itm.code::varchar || ')' || itm.other::varchar = t1.fiber_type
  
  UNION ALL 
  select t1.entity_type,t1.qty,itm.cost_per_unit ,itm.service_cost_per_unit ,
 ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) as total_cost  from (select entity_type,sum(cable_length) as qty ,fiber_type from backbone_plan_network_details bpnd where created_by = p_user_id and plan_id = p_plan_id and entity_type in ('Cable') and fiber_type = p_sprout_fibertype group by entity_type,fiber_type) t1
  join item_template_master itm on itm.category_reference = t1.entity_type where '(' || itm.code::varchar || ')' || itm.other::varchar = t1.fiber_type)t group by t.entity_type)row_result ;
 
 
END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_get_plan_bom(int4, int4, varchar, varchar, varchar) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_get_plan_bom(int4, int4, varchar, varchar, varchar) TO postgres;


---------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_bom(p_plan_id integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    sql TEXT := '';
BEGIN
    -- Cable
    sql := 'SELECT 
        entity_type::character varying,
        geom_type,
        cost_per_unit,
        service_cost_per_unit,
        ROUND(measured_length::numeric, 2) || ''(m)/'' || length_qty::character varying AS length_qty,
        (ROUND(measured_length::numeric, 2) * service_cost_per_unit + cost_per_unit * ROUND(measured_length::numeric, 2)) AS amount
    FROM (
        SELECT 
            (SELECT layer_title 
             FROM layer_details 
             WHERE layer_name ILIKE ''cable'')::character varying AS entity_type,
            ''Line''::character varying AS geom_type,
            COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
            COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
            COUNT(att.system_id) AS length_qty,
            SUM(att.cable_measured_length) AS measured_length
        FROM att_details_cable att
        JOIN item_template_master item 
            ON item.audit_id = att.audit_item_master_id
        WHERE att.source_ref_id = ' || quote_literal(p_plan_id) || '
          AND att.source_ref_type = ''backbone planning''
          AND item.category_reference = ''Cable''
        GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code
    ) e';

    -- Pole
    IF EXISTS (SELECT 1 FROM att_details_pole WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id ::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''Pole'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(att.system_id) AS length_qty
            FROM att_details_pole att
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN audit_item_template_master item ON item.audit_id = att.audit_item_master_id
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;

    -- Manhole
    IF EXISTS (SELECT 1 FROM att_details_manhole WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''Manhole'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(att.system_id) AS length_qty
            FROM att_details_manhole att
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN audit_item_template_master item ON item.audit_id = att.audit_item_master_id
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;

    -- SpliceClosure
    IF EXISTS (SELECT 1 FROM att_details_spliceclosure WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''SpliceClosure'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(att.system_id) AS length_qty
            FROM att_details_spliceclosure att
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN audit_item_template_master item ON item.audit_id = att.audit_item_master_id
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;

    -- Debug SQL
    RAISE INFO 'Final SQL: %', sql;

    -- Execute the full dynamic query and return as JSON
    RETURN QUERY EXECUTE '
        SELECT row_to_json(row) 
        FROM (
            SELECT * FROM (' || sql || ') t 
            ORDER BY geom_type
        ) row';

END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_get_plan_bom(int4, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_get_plan_bom(int4, int4) TO postgres;


-----------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_fibertype_dropdownlist()
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    
begin
return query	
select row_to_json(row) from (SELECT '(' || code::varchar || ')' || other::varchar as dropdown_value ,specification  as dropdown_type 
FROM item_template_master 
WHERE category_reference = 'Cable') row;

END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_get_fibertype_dropdownlist() OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_get_fibertype_dropdownlist() TO postgres;

---------------------------------------------------------------------------------------------------------

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

-- Permissions

ALTER FUNCTION public.fn_backbone_get_plan_network(int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_get_plan_network(int4) TO postgres;


------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_draft_line_geometry(p_plan_id integer, p_user_id integer)
 RETURNS TABLE(geojson text)
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    SELECT ST_AsGeoJSON(line_sp_geometry)
    FROM backbone_plan_network_details
    WHERE plan_id = p_plan_id
      AND created_by = p_user_id;
END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_backbone_get_draft_line_geometry(int4, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_backbone_get_draft_line_geometry(int4, int4) TO postgres;
