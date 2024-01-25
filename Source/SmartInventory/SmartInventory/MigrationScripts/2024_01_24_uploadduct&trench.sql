CREATE OR REPLACE FUNCTION public.fn_uploader_insert_duct(p_upload_id integer, p_batch_id integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$
declare
result int:=0;
cnt int:=0;
texttoappend character varying;
REC RECORD;
v_start_geom character varying;
v_end_geom character varying;
current_system_id integer;
v_parent_system_id integer;
v_parent_network_id character varying;
v_parent_entity_type character varying;
v_network_code character varying;
v_sequence_id integer;
v_geom character varying;
v_entity_name character varying;
v_network_Id_type character varying;
v_network_code_seperator character varying;
v_a_display_value character varying;
v_b_display_value character varying;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
select entity_type into v_entity_name from upload_summary where id=p_upload_id;
select network_id_type,network_code_seperator into v_network_Id_type,v_network_code_seperator from layer_details where upper(layer_name)=upper(v_entity_name);
FOR REC IN select * from temp_du_duct where upload_id=p_upload_id and is_valid=true and batch_id=p_batch_id
LOOP
BEGIN
-- select rec.a_end_lng::text||' '|| rec.a_end_lat::text into v_start_geom;
-- select rec.b_end_lng::text||' '|| rec.b_end_lat::text into v_end_geom;

v_sequence_id:=0;
v_network_name:=rec.duct_name;
v_geom:=replace(replace(upper(rec.sp_geometry),'LINESTRING(',''),')','');
v_geom:=replace(v_geom,'LINESTRING (','');
v_geom:=replace(v_geom,'LINESTRINGZ (','');
v_geom:=replace(v_geom,'LINESTRINGZ(','');

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Duct') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
-- select a.parent_system_id,a.parent_network_id, a.parent_entity_type,a.network_code,a.sequence_id
-- from fn_get_line_network_code('Duct','Line','0','' || v_start_geom || ' , ' || v_end_geom ||'','')a into
-- v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;

-- select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
-- from fn_get_clone_network_code('Duct', 'Line',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
-- v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;

select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Duct', 'Line',''||v_geom||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;

end if;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;

If(upper(coalesce(v_parent_network_id, ''))!= 'NLD')
then
v_parent_network_id=rec.parent_network_id;
END IF;

--INSERT INTO MAIN TABLE
INSERT INTO att_details_duct(network_id, duct_name,calculated_length,manual_length, a_system_id, a_location,
a_entity_type, b_system_id, b_location, b_entity_type,trench_id,network_status, status, province_id,region_id,specification,
category, subcategory1, subcategory2, subcategory3, item_code,vendor_id,created_by, created_on,parent_system_id,
parent_network_id, parent_entity_type,sequence_id,type,brand,model,construction,activation,accessibility,offset_value,
utilization,no_of_cables,ownership_type, source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,
origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,duct_color)

select v_network_code, v_network_name, calculated_length::double precision,0,a_system_id,a_network_id,a_entity_type,b_system_id,
b_network_id,b_entity_type,0,coalesce(rec.network_status,'P'),'A',province_id, region_id,specification,category,subcategory1,
subcategory2,subcategory3,item_code,vendor_id,created_by, now(),rec.parent_system_id,v_parent_network_id,REC.PARENT_ENTITY_TYPE,
v_sequence_id,0,0,0,0,0,0,0,0,0,'Own', 'DU', p_upload_id, rec.remarks,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed','A' from temp_du_duct
where system_id=rec.system_id returning system_id into current_system_id;


select display_name into v_a_display_value from point_master where system_id=REC.a_system_id and upper(entity_type)=upper(REC.a_entity_type);
select display_name into v_b_display_value from point_master where system_id=REC.b_system_id and upper(entity_type)=upper(REC.b_entity_type);

--INSERT INTO LINE MASTER
INSERT INTO line_master(system_id, entity_type, approval_flag,sp_geometry,approval_date,creator_remark,approver_remark, created_by, approver_id, common_name, db_flag, network_status,display_name)
values(current_system_id, 'Duct','A',st_geomfromtext(rec.sp_geometry,4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, p_upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(current_system_id, 'Duct'));
--from temp_du_duct t join kml_attributes k on upper(k.cable_name)=upper(t.duct_name) and upload_id=k.uploaded_id
--where system_id = rec.system_id;

perform(fn_duct_set_end_point(current_system_id));

insert into associate_entity_info(entity_system_id,entity_type,entity_network_id,entity_display_name,associated_system_id,associated_entity_type,associated_network_id,associated_display_name,created_by,created_on,is_termination_point)
values(current_system_id,'Duct',v_network_code,fn_get_display_name(current_system_id, 'Duct'),REC.a_system_id,REC.a_entity_type,REC.a_network_id,v_a_display_value,rec.created_by,now(),true),
(current_system_id,'Duct',v_network_code,fn_get_display_name(current_system_id, 'Duct'),REC.b_system_id,REC.b_entity_type,REC.b_network_id,v_b_display_value,rec.created_by,now(),true);


END;

END LOOP;

return 1;
END;
$function$
;


----------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_trench(p_upload_id integer, p_batch_id integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$
            declare
            result int:=0;
            cnt int:=0;
            texttoappend character varying;
            REC RECORD;
            v_start_geom character varying;
            v_end_geom character varying;
            current_system_id integer;
            v_parent_system_id integer;
            v_parent_network_id character varying;
            v_parent_entity_type character varying;
            v_network_code character varying;
            v_sequence_id integer;
            v_geom character varying;
            v_entity_name character varying;
            v_network_Id_type character varying;
            v_network_code_seperator character varying;
            v_a_display_value character varying;
            v_b_display_value character varying;
            v_auto_character_count integer;
            v_network_name character varying;
            BEGIN
            select entity_type from upload_summary where id=p_upload_id into v_entity_name;
            select network_id_type,network_code_seperator from layer_details where upper(layer_name)=upper(v_entity_name) into v_network_Id_type,v_network_code_seperator;
            FOR REC IN select * from temp_du_trench where upload_id=p_upload_id and is_valid=true and batch_id=p_batch_id
            LOOP
            BEGIN
            v_sequence_id:=0;
            v_network_name:=rec.trench_name;
            -- select rec.a_end_lng::text||' '|| rec.a_end_lat::text into v_start_geom;
            -- select rec.b_end_lng::text||' '|| rec.b_end_lat::text into v_end_geom;
            v_sequence_id:=0;
            v_network_name:=rec.trench_name;
            v_geom:=replace(replace(upper(rec.sp_geometry),'LINESTRING(',''),')','');
            v_geom:=replace(v_geom,'LINESTRING (','');
            v_geom:=replace(v_geom,'LINESTRINGZ (','');
            v_geom:=replace(v_geom,'LINESTRINGZ(','');
            RAISE INFO 'system_id:% ',rec.system_id;
            -- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
            if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
            then
            v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
            elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
            then
            v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
            select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
            from vw_layer_mapping where upper(child_layer_name)=upper('Trench') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
            v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
            else
            -- GET NETWORK CODE & PARENT DETAILS..
            -- select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
            -- from fn_get_clone_network_code('Trench', 'Line',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
            -- v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
            select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
            from fn_get_clone_network_code('Trench', 'Line',''||v_geom||'', rec.parent_system_id, rec.parent_entity_type) into
            v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
            end if;
            IF(coalesce(v_network_name,'')='')
            then
            v_network_name=v_network_code;
            END IF;
            If(upper(coalesce(v_parent_network_id, ''))!= 'NLD')
            then
            v_parent_network_id=rec.parent_network_id;
            END IF;
            --INSERT INTO MAIN TABLE
            INSERT INTO att_details_trench(network_id, trench_name, a_system_id, a_location,a_entity_type, b_system_id, b_location, b_entity_type, trench_length
            ,trench_type,network_status, status,province_id,region_id,specification,
            category, subcategory1, subcategory2, subcategory3, item_code,vendor_id,created_by, created_on,parent_system_id,
            parent_network_id, parent_entity_type,sequence_id,type,brand,model,construction,activation,accessibility,customer_count,
            utilization,trench_width,trench_height,ownership_type, source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,
            origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,trench_serving_type)
            select v_network_code, v_network_name, a_system_id,a_network_id,a_entity_type,b_system_id,b_network_id,b_entity_type,
            trench_length,trench_type,coalesce(rec.network_status,'P'),'A',province_id, region_id,specification,category,subcategory1,subcategory2,
            subcategory3,item_code,vendor_id,created_by, created_on,rec.parent_system_id,v_parent_network_id,REC.PARENT_ENTITY_TYPE,
            v_sequence_id,0,0,0,0,0,0,0,0,0,0,'Own', 'DU', p_upload_id, rec.remarks,rec.origin_from,rec.origin_Ref_id,
            rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed','Distribution Serving type'
            from temp_du_trench where system_id=rec.system_id returning system_id into current_system_id;
            select display_name into v_a_display_value from point_master where system_id=REC.a_system_id and upper(entity_type)=upper(REC.a_entity_type);
            select display_name into v_b_display_value from point_master where system_id=REC.b_system_id and upper(entity_type)=upper(REC.b_entity_type);
            --INSERT INTO LINE MASTER
            INSERT INTO line_master(system_id, entity_type, approval_flag,sp_geometry,approval_date,creator_remark,approver_remark, created_by, approver_id, common_name, db_flag, network_status,display_name)
            values(current_system_id, 'Trench','A',st_geomfromtext(REC.sp_geometry,4326),
            now(), 'NA', 'NA', rec.created_by,0, v_network_code, p_upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(current_system_id, 'Trench'));
            --from temp_du_trench join kml_attributes k on upload_id=k.uploaded_id where k.cable_name=rec.trench_name and system_id = rec.system_id and rec.upload_id=p_upload_id;
            perform(fn_trench_set_end_point(current_system_id));
            insert into associate_entity_info(entity_system_id,entity_type,entity_network_id,entity_display_name,associated_system_id,associated_entity_type,associated_network_id,associated_display_name,created_by,created_on,is_termination_point)
            values(current_system_id,'Trench',v_network_code,fn_get_display_name(current_system_id, 'Trench'),REC.a_system_id,REC.a_entity_type,REC.a_network_id,v_a_display_value,rec.created_by,now(),true),
            (current_system_id,'Trench',v_network_code,fn_get_display_name(current_system_id, 'Trench'),REC.b_system_id,REC.b_entity_type,REC.b_network_id,v_b_display_value,rec.created_by,now(),true);
            END;
            END LOOP;
            return 1;
            END;
            $function$
;
