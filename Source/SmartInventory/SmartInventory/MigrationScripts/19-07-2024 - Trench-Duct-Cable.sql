ALTER TABLE temp_du_cable
ADD COLUMN a_location_code varchar(50),
ADD COLUMN b_location_code varchar(50);

ALTER TABLE temp_du_trench
ADD COLUMN a_location_code varchar(50),
ADD COLUMN b_location_code varchar(50);

ALTER TABLE temp_du_duct
ADD COLUMN a_location_code varchar(50),
ADD COLUMN b_location_code varchar(50);

INSERT INTO data_uploader_template (
    layer_id,
    db_column_name,
	 db_column_data_type,
    template_column_name,
    example_value,
    column_sequence,
    max_length,
    display_column_data_type,
    is_kml_attribute,
    is_excel_attribute
)
VALUES
    (19, 'a_location_code', 'varchar', 'a_location_code', '', 45, 100, 'Character varying', 'true', true),
    (19, 'b_location_code', 'varchar', 'b_location_code', '', 46, 100, 'Character varying', 'true', true),
    (16, 'a_location_code', 'varchar', 'a_location_code', '', 47, 100, 'Character varying', 'true', true),
    (16, 'b_location_code', 'varchar', 'b_location_code', '', 48, 100, 'Character varying', 'true', true),
    (17, 'a_location_code', 'varchar', 'a_location_code', '', 49, 100, 'Character varying', 'true', true),
    (17, 'b_location_code', 'varchar', 'b_location_code', '', 50, 100, 'Character varying', 'true', true);



    -- FUNCTION: public.fn_uploader_insert_trench(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_trench(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_trench(
	p_upload_id integer,
	p_batch_id integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

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
            RAISE INFO 'v_parent_system_id:% ',v_parent_system_id;
            RAISE INFO 'v_parent_network_id:% ',v_parent_network_id;
            RAISE INFO 'v_parent_entity_type:% ',v_parent_entity_type;
            RAISE INFO 'v_network_code:% ',v_network_code;
            RAISE INFO 'v_sequence_id:% ',v_sequence_id;
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
            origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,trench_serving_type,a_location_code,b_location_code)
            select v_network_code, v_network_name, a_system_id,a_network_id,a_entity_type,b_system_id,b_network_id,b_entity_type,
            trench_length,trench_type,coalesce(rec.network_status,'P'),'A',province_id, region_id,specification,category,subcategory1,subcategory2,
            subcategory3,item_code,vendor_id,created_by, created_on,rec.parent_system_id,v_parent_network_id,REC.PARENT_ENTITY_TYPE,
            v_sequence_id,0,0,0,0,0,0,0,0,0,0,'Own', 'DU', p_upload_id, rec.remarks,rec.origin_from,rec.origin_Ref_id,
            rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed','Distribution Serving type',a_location_code,b_location_code
            from temp_du_trench where system_id=rec.system_id returning system_id into current_system_id;
            select display_name into v_a_display_value from point_master where system_id=REC.a_system_id and upper(entity_type)=upper(REC.a_entity_type);
            select display_name into v_b_display_value from point_master where system_id=REC.b_system_id and upper(entity_type)=upper(REC.b_entity_type);
            --INSERT INTO LINE MASTER
            INSERT INTO line_master(system_id, entity_type, approval_flag,sp_geometry,approval_date,creator_remark,approver_remark, created_by, approver_id, common_name, db_flag, network_status,display_name)
            values(current_system_id, 'Trench','A',st_geomfromtext(REC.sp_geometry,4326),
            now(), 'NA', 'NA', rec.created_by,0, v_network_code, p_upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(current_system_id, 'Trench'));
            --from temp_du_trench join kml_attributes k on upload_id=k.uploaded_id where k.cable_name=rec.trench_name and system_id = rec.system_id and rec.upload_id=p_upload_id;
            
			perform(fn_trench_set_end_point(current_system_id));
			PERFORM (FN_CREATE_ROUTE_ID(current_system_id,'Trench',0,'Trench'));
			perform (fn_geojson_update_entity_attribute(current_system_id::integer,'Trench'::character varying ,rec.province_id::integer,0,false));

            insert into associate_entity_info(entity_system_id,entity_type,entity_network_id,entity_display_name,associated_system_id,associated_entity_type,associated_network_id,associated_display_name,created_by,created_on,is_termination_point)
            values(current_system_id,'Trench',v_network_code,fn_get_display_name(current_system_id, 'Trench'),REC.a_system_id,REC.a_entity_type,REC.a_network_id,v_a_display_value,rec.created_by,now(),true),
            (current_system_id,'Trench',v_network_code,fn_get_display_name(current_system_id, 'Trench'),REC.b_system_id,REC.b_entity_type,REC.b_network_id,v_b_display_value,rec.created_by,now(),true);
            END;
            END LOOP;
            return 1;
            END;
            
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_trench(integer, integer)
    OWNER TO postgres;


    -- FUNCTION: public.fn_uploader_insert_duct(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_duct(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_duct(
	p_upload_id integer,
	p_batch_id integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

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
origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,duct_color,a_location_code,b_location_code)

select v_network_code, v_network_name, calculated_length::double precision,0,a_system_id,a_network_id,a_entity_type,b_system_id,
b_network_id,b_entity_type,0,coalesce(rec.network_status,'P'),'A',province_id, region_id,specification,category,subcategory1,
subcategory2,subcategory3,item_code,vendor_id,created_by, now(),rec.parent_system_id,v_parent_network_id,REC.PARENT_ENTITY_TYPE,
v_sequence_id,0,0,0,0,0,0,0,0,0,'Own', 'DU', p_upload_id, rec.remarks,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed','A',a_location_code,b_location_code from temp_du_duct
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
PERFORM (FN_CREATE_ROUTE_ID(current_system_id,'Duct',0,'Duct'));
perform (fn_geojson_update_entity_attribute(current_system_id::integer,'Duct'::character varying ,rec.province_id::integer,0,false));

insert into associate_entity_info(entity_system_id,entity_type,entity_network_id,entity_display_name,associated_system_id,associated_entity_type,associated_network_id,associated_display_name,created_by,created_on,is_termination_point)
values(current_system_id,'Duct',v_network_code,fn_get_display_name(current_system_id, 'Duct'),REC.a_system_id,REC.a_entity_type,REC.a_network_id,v_a_display_value,rec.created_by,now(),true),
(current_system_id,'Duct',v_network_code,fn_get_display_name(current_system_id, 'Duct'),REC.b_system_id,REC.b_entity_type,REC.b_network_id,v_b_display_value,rec.created_by,now(),true);

END;

END LOOP;

return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_duct(integer, integer)
    OWNER TO postgres;



    -- FUNCTION: public.fn_uploader_insert_cable(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_cable(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_cable(
	p_upload_id integer,
	p_batch_id integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

		declare
		result int:=0;
		cnt int:=0;
		texttoappend character varying;
		REC RECORD;
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
		v_a_system_id integer;
		v_a_entity_type character varying;
		v_b_system_id integer;
		v_b_entity_type character varying;
		v_structure_id integer;
		v_a_network_id character varying;
		v_b_network_id character varying;
		v_cable_a_primary_table character varying;
		v_cable_a_secondary_table character varying;
		v_cable_b_primary_table character varying;
		v_cable_b_secondary_table character varying;
		v_a_connectivity character varying;
		v_b_connectivity character varying;
		sql text;
		BEGIN
		select entity_type into v_entity_name from upload_summary where id=p_upload_id;
		select network_id_type,network_code_seperator into v_network_Id_type,v_network_code_seperator from layer_details where upper(layer_name)=upper(v_entity_name);

		FOR REC IN select * from temp_du_cable t
		--inner join kml_attributes k on upper(k.cable_name)=upper(t.cable_name) and t.upload_id=k.uploaded_id
		where upload_id=p_upload_id and is_valid=true and batch_id=p_batch_id
		LOOP
		BEGIN
		-- v_geom:=replace(replace(rec.sp_geometry,'LINESTRING(',''),')','');
		-- v_geom:=replace(replace(rec.sp_geometry,'LINESTRING (',''),')','');

		v_geom:=replace(replace(upper(rec.sp_geometry),'LINESTRING(',''),')','');
		v_geom:=replace(v_geom,'LINESTRING (','');
		v_geom:=replace(v_geom,'LINESTRINGZ (','');
		v_geom:=replace(v_geom,'LINESTRINGZ(','');

		--v_geom:=replace(v_geom,'LINESTRING (','');
		v_network_name:=rec.cable_name;
		v_sequence_id:=0;

		-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
		then
		v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then

		v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
		from vw_layer_mapping where upper(child_layer_name)=upper('Cable') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
		v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
		-- GET NETWORK CODE & PARENT DETAILS..
		-- select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
		-- from fn_get_clone_network_code('Cable', 'Line',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
		-- v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
		raise info 'REC.network_id%',v_geom;
		raise info 'rec.parent_entity_type%',rec.parent_entity_type;
		select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
		from fn_get_clone_network_code('Cable', 'Line',''||v_geom||'', rec.parent_system_id, rec.parent_entity_type) into
		v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;

		end if;

		IF(coalesce(v_network_name,'')='')
		then
		v_network_name=v_network_code;
		END IF;

		RAISE INFO 'v_parent_system_id : %',v_parent_system_id;
		RAISE INFO 'v_parent_network_id : %',v_parent_network_id;
		RAISE INFO 'v_parent_entity_type : %',v_parent_entity_type;
		RAISE INFO 'v_network_code : %',v_network_code;
		RAISE INFO 'v_sequence_id : %',v_sequence_id;

		If(upper(coalesce(v_parent_network_id, ''))!= 'NLD')
		then
		v_parent_network_id=rec.parent_network_id;
		END IF;
		--INSERT INTO MAIN TABLE

		raise info 'Result of rec :%',rec.origin_ref_id;
		if(rec.origin_ref_id is not null and rec.origin_from ='SP') then 
		if(rec.cable_type ='ISP') then
		Select  adb2.system_id as a_system_id,adb2.network_id, 'BDB' as a_entity_type ,
				ifi2.system_id as b_system_id ,ifi2.network_id ,'FDB' as b_entity_type,ifi2.parent_system_id 
				into v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type,v_structure_id
				from 
				(select * from temp_du_cable where system_id =rec.system_id) cable 
		inner join att_details_splitter adb   
		on cast(cable.a_end as character varying) = adb.origin_Ref_id 
		inner join att_details_splitter ifi 
		on cast(cable.b_end as character varying) = ifi.origin_ref_id
		inner join att_details_bdb adb2 
		on adb2.system_id = adb.parent_system_id 
		inner join isp_fdb_info ifi2  
		on ifi2.system_id = ifi.parent_system_id order by 1 desc,4 desc  limit 1;
		else
		raise info 'Processed';
		 if(rec.a_entity_type ='BDB') then 
		 
		 v_cable_a_primary_table :='att_details_splitter';
		 v_cable_a_secondary_table := 'att_details_bdb';
		v_a_connectivity :='on a_secondary.system_id = a_primary.parent_system_id' ;
		elseif (rec.a_entity_type='FMS') then
		 
		 v_cable_a_primary_table :='att_details_pod';
		 v_cable_a_secondary_table := 'att_details_fms';
		v_a_connectivity :='on a_secondary.parent_system_id = a_primary.system_id' ;
		end if;
		if ((rec.b_entity_type ='BDB')) then 

		v_cable_b_primary_table :='att_details_splitter';
		 v_cable_b_secondary_table := 'att_details_bdb';
		v_b_connectivity :='on b_secondary.system_id = b_primary.parent_system_id' ;
		elseif ((rec.b_entity_type ='FMS')) then 
		v_cable_b_primary_table :='att_details_pod';
		 v_cable_b_secondary_table := 'att_details_fms';
		v_b_connectivity :='on b_secondary.parent_system_id = b_primary.system_id' ;
		end if;
		raise info 'Processed :% ,:%, :% ,:%',v_cable_a_primary_table,v_cable_b_primary_table,v_cable_a_secondary_table,v_cable_b_secondary_table;
		sql := 'Select  a_secondary.system_id as a_system_id,a_secondary.network_id, cable.a_entity_type ,
				b_secondary.system_id as b_system_id ,b_secondary.network_id ,cable.b_entity_type
				from 
				(select * from temp_du_cable where system_id ='||rec.system_id ||') cable 
		inner join '|| v_cable_a_primary_table ||' a_primary   
		on cast(cable.a_end as character varying) = a_primary.origin_Ref_id 
		inner join '|| v_cable_b_primary_table ||' b_primary 
		on cast(cable.b_end as character varying) = b_primary.origin_ref_id
		inner join '|| v_cable_a_secondary_table || ' a_secondary ' ||
		v_a_connectivity ||'
		inner join '|| v_cable_b_secondary_table ||' b_secondary  
		' ||
		v_b_connectivity ||' order by 1 desc,4 desc  limit 1;';
		 raise info 'Sql:% ',sql;
		 execute sql  into v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type;
		v_structure_id :=0;
		end if;
		raise info 'Values :%,:%,:%,:%,%,%',v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type;
		INSERT INTO public.att_details_cable(network_id, cable_name, a_location, b_location, total_core,
		no_of_tube, no_of_core_per_tube,
		cable_measured_length, cable_calculated_length,
		cable_type, specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id, network_status,
		status,province_id,
		region_id,created_by,created_on, a_system_id,a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type,
		cable_category,cable_sub_category,route_id, parent_system_id, parent_network_id, parent_entity_type,sequence_id,
		type,brand,model,construction,activation,accessibility,duct_id,utilization,ownership_type, source_ref_type,
		source_ref_id, remarks,audit_item_master_id,origin_from,origin_Ref_id,origin_Ref_code,
		origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,structure_id,
		section_name, generic_section_name,hierarchy_type,aerial_location)

		SELECT v_network_code,v_network_name, v_a_network_id, v_b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
		rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
		subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),v_a_system_id,
		v_a_network_id,
		v_a_entity_type,v_b_system_id,v_b_network_id,v_b_entity_type,cable_category,sub_category,
		route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
		p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',v_structure_id,
		section_name, generic_section_name,hierarchy_type,aerial_location
		from temp_du_cable where system_id=rec.system_id
		returning system_id into current_system_id;
		else
		INSERT INTO public.att_details_cable(network_id, cable_name, a_location, b_location, total_core,no_of_tube, no_of_core_per_tube,
		cable_measured_length, cable_calculated_length,
		cable_type, specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id, network_status, status,province_id,
		region_id,created_by,created_on, a_system_id,a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type,
		cable_category,cable_sub_category,route_id, parent_system_id, parent_network_id, parent_entity_type,sequence_id,
		type,brand,model,construction,activation,accessibility,duct_id,utilization,ownership_type, source_ref_type,
		source_ref_id, remarks,audit_item_master_id,origin_from,origin_Ref_id,origin_Ref_code,
		origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,section_name, generic_section_name,hierarchy_type,aerial_location,a_location_code,b_location_code)
		SELECT v_network_code,v_network_name, a_network_id, b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
		rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
		subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),a_system_id,a_network_id,
		a_entity_type,b_system_id,b_network_id,b_entity_type,cable_category,sub_category,
		route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
		p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',
		section_name, generic_section_name,hierarchy_type,aerial_location,a_location_code,b_location_code
		from temp_du_cable where system_id=rec.system_id
		returning system_id into current_system_id;
		end if;

		select display_name into v_a_display_value from point_master where system_id=REC.a_system_id and upper(entity_type)=upper(REC.a_entity_type);
		select display_name into v_b_display_value from point_master where system_id=REC.b_system_id and upper(entity_type)=upper(REC.b_entity_type);

		--INSERT INTO LINE MASTER

		if (coalesce(rec.cable_type,'') !='ISP') then 
		Raise info '------------------------------------rec.cable_type123456%',rec.cable_type;
		INSERT INTO line_master(system_id, entity_type, approval_flag,sp_geometry,approval_date,creator_remark,approver_remark, created_by,
		approver_id, common_name, db_flag, network_status,display_name)
		values(current_system_id, 'Cable','A',st_geomfromtext(rec.sp_geometry,4326),
		now(), 'NA', 'NA', rec.created_by,0, v_network_code, p_upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(current_system_id, 'Cable'));
		--from temp_du_cable t join kml_attributes k on upper(k.cable_name)=upper(k.cable_name) and upload_id=k.uploaded_id
		--where t.system_id = rec.system_id and t.upload_id=p_upload_id;

		end if;

		perform(fn_isp_create_OSP_Cable(current_system_id));
		perform(fn_cable_set_end_point(current_system_id));
		perform fn_set_cable_color_info(current_system_id, rec.created_by, rec.no_of_tube, rec.core_per_tube);
		PERFORM (FN_CREATE_ROUTE_ID(current_system_id,'Cable',0,'Cable'));
		perform (fn_geojson_update_entity_attribute(current_system_id::integer,'Cable'::character varying ,rec.province_id::integer,0,false));

		insert into associate_entity_info(entity_system_id,entity_type,entity_network_id,entity_display_name,associated_system_id,associated_entity_type,associated_network_id,associated_display_name,created_by,created_on,is_termination_point)
		values(current_system_id,'Cable',v_network_code,fn_get_display_name(current_system_id, 'Cable'),REC.a_system_id,REC.a_entity_type,REC.a_network_id,v_a_display_value,rec.created_by,now(),true),
		(current_system_id,'Cable',v_network_code,fn_get_display_name(current_system_id, 'Cable'),REC.b_system_id,REC.b_entity_type,REC.b_network_id,v_b_display_value,rec.created_by,now(),true);

		insert into att_details_cable_cdb (circle_name, major_route_name, route_id, section_name, section_id, route_category, distance, fiber_pairs_laid, total_used_pair, fiber_pairs_used_by_vil, fiber_pairs_given_to_airtel, fiber_pairs_given_to_others, fiber_pairs_free, faulty_fiber_pairs, start_latitude, start_longitude, end_latitude, end_longitude, count_non_vil_tenancies_on_route, route_lit_up_date, aerial_km, avg_loss_per_km, avg_last_six_months_fiber_cut, row, material, execution, row_availablity, iru_given_airtel, iru_given_jio, iru_given_ttsl_or_ttml, network_category, row_valid_or_exp, remarks, cable_owner, route_type, operator_type, fiber_type, cable_id, iru_given_tcl, iru_given_others)
		select circle_name, major_route_name, route_id, section_name, section_id, route_category, distance, fiber_pairs_laid, total_used_pair, fiber_pairs_used_by_vil, fiber_pairs_given_to_airtel, fiber_pairs_given_to_others, fiber_pairs_free, faulty_fiber_pairs, start_latitude, start_longitude, end_latitude, end_longitude, count_non_vil_tenancies_on_route, route_lit_up_date, aerial_km, avg_loss_per_km, avg_last_six_months_fiber_cut, row, material, execution, row_availablity, iru_given_airtel, iru_given_jio, iru_given_ttsl_or_ttml, network_category, row_valid_or_exp, remarks, cable_owner, route_type, operator_type, fiber_type, (SELECT att.system_id FROM  att_details_cable AS att JOIN  temp_du_att_details_cable_cdb AS tempcdb ON  att.network_id  = tempcdb.cable_id WHERE  tempcdb.cable_id = v_network_code  AND tempcdb.upload_id = p_upload_id) as cable_id, iru_given_tcl, iru_given_others from temp_du_att_details_cable_cdb where cable_id = v_network_code and upload_id = p_upload_id and is_valid=true;
		raise info '--------------network id :%',v_network_code;

		END;
		END LOOP;

		return 1;

		END;
		
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_cable(integer, integer)
    OWNER TO postgres;
