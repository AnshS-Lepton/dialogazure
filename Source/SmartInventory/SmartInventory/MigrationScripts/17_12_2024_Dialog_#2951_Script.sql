INSERT INTO public.data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes, is_overlapping_rule_enabled) 
										VALUES(14, 'gis_design_id', 'varchar', 				'gis_design_id', 		false, 		NULL, 'gis_design_id', 'ABC', 		22, 			50, 			1, 			Now(), 	NULL, 			NULL, 		false, 		'character varying', 			false, 			true, 			true, 			NULL, 				false, 					true, 							NULL, 		NULL, 		NULL, 		NULL, 						NULL, 					false, 						false),
												(14, 'subarea_id', 'varchar', 				'subarea_id', 			false, 		NULL, 'subarea_id', 	'DEF', 		23, 			50, 			1, 			Now(), 	NULL, 			NULL, 		false, 		'character varying', 			false, 			true, 			true, 			NULL, 				false, 					true, 							NULL, 		NULL, 		NULL, 		NULL, 						NULL, 					false, 						false);
												
-----------------------------------

update data_uploader_template set template_column_name='network_status' where db_column_name='network_status' and layer_id =14

-----------------------------------

--update layer_columns_settings set display_name='network_status' where layer_id =14 and column_name ilike 'network_status' and setting_type ='Report'

-----------------------------------
--template_id 6 is for Pole
update data_uploader_column_mapping set imported_column_name= where template_db_column_name='network_status' and template_id =6

-----------------------------------
-- Add new columns
ALTER TABLE temp_du_pole
ADD COLUMN gis_design_id VARCHAR,
ADD COLUMN subarea_id VARCHAR;

---------------------------------------
UPDATE public.data_uploader_template SET column_sequence=8 WHERE layer_id=14 AND db_column_name='specification';
UPDATE public.data_uploader_template SET column_sequence=15 WHERE layer_id=14 AND db_column_name='origin_from';
UPDATE public.data_uploader_template SET column_sequence=14 WHERE layer_id=14 AND db_column_name='origin_ref_id';
UPDATE public.data_uploader_template SET column_sequence=13 WHERE layer_id=14 AND db_column_name='origin_ref_code';
UPDATE public.data_uploader_template SET column_sequence=20 WHERE layer_id=14 AND db_column_name='origin_ref_description';
UPDATE public.data_uploader_template SET column_sequence=18 WHERE layer_id=14 AND db_column_name='requested_by';
UPDATE public.data_uploader_template SET column_sequence=17 WHERE layer_id=14 AND db_column_name='request_approved_by';
UPDATE public.data_uploader_template SET column_sequence=11 WHERE layer_id=14 AND db_column_name='parent_network_id';
UPDATE public.data_uploader_template SET column_sequence=1 WHERE layer_id=14 AND db_column_name='network_id';
UPDATE public.data_uploader_template SET column_sequence=2 WHERE layer_id=14 AND db_column_name='pole_name';
UPDATE public.data_uploader_template SET column_sequence=7 WHERE layer_id=14 AND db_column_name='pole_height';
UPDATE public.data_uploader_template SET column_sequence=5 WHERE layer_id=14 AND db_column_name='address';
UPDATE public.data_uploader_template SET column_sequence=6 WHERE layer_id=14 AND db_column_name='pole_no';
UPDATE public.data_uploader_template SET column_sequence=12 WHERE layer_id=14 AND db_column_name='parent_entity_type';
UPDATE public.data_uploader_template SET column_sequence=9 WHERE layer_id=14 AND db_column_name='vendor_name';
UPDATE public.data_uploader_template SET column_sequence=21 WHERE layer_id=14 AND db_column_name='network_status';
UPDATE public.data_uploader_template SET column_sequence=3 WHERE layer_id=14 AND db_column_name='latitude';
UPDATE public.data_uploader_template SET column_sequence=4 WHERE layer_id=14 AND db_column_name='longitude';
UPDATE public.data_uploader_template SET column_sequence=10 WHERE layer_id=14 AND db_column_name='pole_type';
UPDATE public.data_uploader_template SET column_sequence=22 WHERE layer_id=14 AND db_column_name='gis_design_id';
UPDATE public.data_uploader_template SET column_sequence=23 WHERE layer_id=14 AND db_column_name='subarea_id';
UPDATE public.data_uploader_template SET column_sequence=16 WHERE layer_id=14 AND db_column_name='remarks';
UPDATE public.data_uploader_template SET column_sequence=19 WHERE layer_id=14 AND db_column_name='request_ref_id';

-------------------MANHOLE SECTION--------------------


INSERT INTO public.data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes, is_overlapping_rule_enabled) 
										VALUES(13, 'gis_design_id', 'varchar', 				'gis_design_id', 		false, 		NULL, 'gis_design_id', 'ABC', 		22, 			50, 			1, 			Now(), 	NULL, 			NULL, 		false, 		'character varying', 			false, 			true, 			true, 			NULL, 				false, 					true, 							NULL, 		NULL, 		NULL, 		NULL, 						NULL, 					false, 						false),
												(13, 'subarea_id', 'varchar', 				'subarea_id', 			false, 		NULL, 'subarea_id', 	'DEF', 		23, 			50, 			1, 			Now(), 	NULL, 			NULL, 		false, 		'character varying', 			false, 			true, 			true, 			NULL, 				false, 					true, 							NULL, 		NULL, 		NULL, 		NULL, 						NULL, 					false, 						false);
---------------------------------------
ALTER TABLE temp_du_manhole
ADD COLUMN gis_design_id VARCHAR,
ADD COLUMN subarea_id VARCHAR;
---------------------------------------
update data_uploader_template set template_column_name='network_status' where db_column_name='network_status' and layer_id =13
---------------------------------------
UPDATE public.data_uploader_template SET column_sequence=1 WHERE layer_id=13 AND db_column_name='network_id';
UPDATE public.data_uploader_template SET column_sequence=2 WHERE layer_id=13 AND db_column_name='manhole_name';
UPDATE public.data_uploader_template SET column_sequence=3 WHERE layer_id=13 AND db_column_name='latitude';
UPDATE public.data_uploader_template SET column_sequence=4 WHERE layer_id=13 AND db_column_name='longitude';
UPDATE public.data_uploader_template SET column_sequence=5 WHERE layer_id=13 AND db_column_name='address';
UPDATE public.data_uploader_template SET column_sequence=6 WHERE layer_id=13 AND db_column_name='specification';
UPDATE public.data_uploader_template SET column_sequence=7 WHERE layer_id=13 AND db_column_name='vendor_name';
UPDATE public.data_uploader_template SET column_sequence=8 WHERE layer_id=13 AND db_column_name='is_virtual';
UPDATE public.data_uploader_template SET column_sequence=9 WHERE layer_id=13 AND db_column_name='parent_network_id';
UPDATE public.data_uploader_template SET column_sequence=10 WHERE layer_id=13 AND db_column_name='parent_entity_type';
UPDATE public.data_uploader_template SET column_sequence=11 WHERE layer_id=13 AND db_column_name='request_ref_id';
UPDATE public.data_uploader_template SET column_sequence=12 WHERE layer_id=13 AND db_column_name='origin_ref_description';
UPDATE public.data_uploader_template SET column_sequence=13 WHERE layer_id=13 AND db_column_name='origin_ref_code';
UPDATE public.data_uploader_template SET column_sequence=14 WHERE layer_id=13 AND db_column_name='origin_ref_id';
UPDATE public.data_uploader_template SET column_sequence=15 WHERE layer_id=13 AND db_column_name='origin_from';
UPDATE public.data_uploader_template SET column_sequence=16 WHERE layer_id=13 AND db_column_name='request_approved_by';
UPDATE public.data_uploader_template SET column_sequence=17 WHERE layer_id=13 AND db_column_name='requested_by';
UPDATE public.data_uploader_template SET column_sequence=18 WHERE layer_id=13 AND db_column_name='remarks';
UPDATE public.data_uploader_template SET column_sequence=19 WHERE layer_id=13 AND db_column_name='network_status';
UPDATE public.data_uploader_template SET column_sequence=20 WHERE layer_id=13 AND db_column_name='authority';
UPDATE public.data_uploader_template SET column_sequence=21 WHERE layer_id=13 AND db_column_name='area';
UPDATE public.data_uploader_template SET column_sequence=22 WHERE layer_id=13 AND db_column_name='route_name';
UPDATE public.data_uploader_template SET column_sequence=23 WHERE layer_id=13 AND db_column_name='section_name';
UPDATE public.data_uploader_template SET column_sequence=24 WHERE layer_id=13 AND db_column_name='generic_section_name';
UPDATE public.data_uploader_template SET column_sequence=25 WHERE layer_id=13 AND db_column_name='hierarchy_type';
UPDATE public.data_uploader_template SET column_sequence=26 WHERE layer_id=13 AND db_column_name='aerial_location';
UPDATE public.data_uploader_template SET column_sequence=27 WHERE layer_id=13 AND db_column_name='route_id';
UPDATE public.data_uploader_template SET column_sequence=28 WHERE layer_id=13 AND db_column_name='gis_design_id';
UPDATE public.data_uploader_template SET column_sequence=29 WHERE layer_id=13 AND db_column_name='subarea_id';

---------------------------------------
-- DROP FUNCTION public.fn_uploader_insert_pole(int4, int4);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_pole(p_uploadid integer, p_batchid integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$

declare
REC RECORD;
v_current_system_id integer;
v_parent_system_id integer;
v_parent_network_id character varying;
v_parent_entity_type character varying;
v_network_code character varying;
v_sequence_id integer;
v_latitude double precision;
v_longitude double precision;
v_network_id_type character varying;
v_network_code_seperator character varying;
v_auto_character_count integer;
v_network_name character varying;
v_geom text;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='POLE';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_pole where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid and is_processed = false
LOOP
BEGIN
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.pole_name;

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Pole') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..

select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Pole', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT DATA IN MAIN TABLE
select case when (select value from global_settings where key= 'IsCustomProjectionAllowed')::integer=1 then ST_TRANSFORM(ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',(select value from global_settings where key= 'customProjection')::integer),4326) else ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326)
END into v_geom;

insert into att_details_pole(network_id,pole_name,latitude,longitude,province_id,
region_id, address,pole_no,pole_height,specification,category,subcategory1,subcategory2,subcategory3,
item_code,vendor_id,status,created_by,created_on,network_status,pole_type,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,type,brand,model,construction,activation,accessibility,
ownership_type, source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y,gis_design_id,subarea_id)
select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,rec.pole_no,
rec.pole_height,rec.specification,
rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now()
,coalesce(rec.network_status,'P'),rec.pole_type,
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,'Own', 'DU', p_uploadid,
rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,rec.gis_design_id,rec.subarea_id returning system_id into v_current_system_id;

--INSERT DATA IN POINT TABLE
insert into point_master (system_id,entity_type,latitude,longitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
select v_current_system_id,'Pole',v_longitude,v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Pole'),rec.st_x,rec.st_y;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_pole set is_processed=true where system_id=rec.system_id;

perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Pole'::character varying ,rec.province_id::integer,0,false));

END;
END LOOP;
return 1;
END;
$function$
;


--------------------------------------------
-- DROP FUNCTION public.fn_uploader_insert_manhole(int4, int4);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_manhole(p_uploadid integer, p_batchid integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$

		declare
		REC RECORD;
		v_current_system_id integer;
		v_parent_system_id integer;
		v_parent_network_id character varying;
		v_parent_entity_type character varying;
		v_network_code character varying;
		v_sequence_id integer;
		v_latitude double precision;
		v_longitude double precision;
		v_network_id_type character varying;
		v_network_code_seperator character varying;
		v_auto_character_count integer;
		v_network_name character varying;
		BEGIN
		-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
		select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='MANHOLE';

		-- INSERT BATCH WISE RECORDS..
		FOR REC IN select * from temp_du_manhole where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
		LOOP
		BEGIN
		v_latitude:=rec.latitude::double precision;
		v_longitude:=rec.longitude::double precision;
		v_sequence_id:=0;
		v_network_name:=rec.manhole_name;
		-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
		then
		v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then
		v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
		from vw_layer_mapping where upper(child_layer_name)=upper('MANHOLE') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
		v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
		-- GET NETWORK CODE & PARENT DETAILS..
		select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
		from fn_get_clone_network_code('Manhole', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
		v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
		end if;
		IF(coalesce(v_network_name,'')='')
		then
		v_network_name=v_network_code;
		END IF;
		--INSERT INTO MAIN TABLE
		insert into att_details_manhole(
		network_id,manhole_name,latitude,longitude,province_id,
		region_id, address,specification,category,subcategory1,subcategory2,subcategory3,
		item_code,vendor_id,status,created_by,created_on,network_status,construction_type,parent_system_id,
		parent_network_id,parent_entity_type,sequence_id,type,brand,model,construction,activation,accessibility,is_virtual,
		ownership_type, source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,
		origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y,area,authority,route_name,
		 section_name, generic_section_name,hierarchy_type,aerial_location,route_id,gis_design_id,subarea_id
		)
		select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,rec.specification,
		rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),coalesce(rec.network_status,'P'),
		rec.construction_type,rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
		rec.is_virtual,'Own', 'DU', p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,
		rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,rec.area,rec.authority,rec.route_name,
		rec.section_name, rec.generic_section_name,rec.hierarchy_type,rec.aerial_location,rec.route_id,rec.gis_design_id,rec.subarea_id
		returning system_id into v_current_system_id;

		--INSERT INTO POINT MASTER
		insert into point_master (system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
		approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
		select v_current_system_id,'Manhole',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
		now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Manhole'),rec.st_x,rec.st_y ;

		-- UPDATE STATUS INTO TEMP TABLE..
		update temp_du_manhole set is_processed=true where system_id=rec.system_id;
		perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Manhole'::character varying ,rec.province_id::integer,0,false));

		END;
		END LOOP;
		return 1;
		END;
		
$function$
;
