alter table temp_du_pole add column ownership_type character varying;

INSERT INTO public.data_uploader_template(
	layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, 
    description, example_value, column_sequence, max_length, created_by, created_on,
     is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, is_template_column_required,
    is_enable_for_template_column, default_value)
	VALUES ((select layer_id from layer_details where layer_name='Pole'), 'ownership_type', 'varchar', 'Ownership type', false,
             'Ownership type either Third Party or OWN', 'OWN', 15, 100, 1, now(),
            false, 'character varying', false, true, true, false, true, 'Own');
			
			
			

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_pole(
	p_uploadid integer,
	p_batchid integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

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
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,rec.ownership_type, 'DU', p_uploadid,
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
$BODY$;
