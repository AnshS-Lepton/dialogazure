alter table item_template_gipipe add column  hierarchy_type character varying(100) ;
alter table isp_template_room add column  hierarchy_type character varying(100) ;
alter table item_template_sector add column  hierarchy_type character varying(100) ;
alter table item_template_vault add column  hierarchy_type character varying(100) ;
alter table item_template_antenna add column  hierarchy_type character varying(100) ;
alter table item_template_patchpanel add column  hierarchy_type character varying(100) ;
alter table item_template_conduit add column  hierarchy_type character varying(100) ;
alter table item_template_handhole add column  hierarchy_type character varying(100) ;
alter table item_template_row add column  hierarchy_type character varying(100) ;
alter table item_template_cabinet add column  hierarchy_type character varying(100) ;
alter table item_template_tower add column  hierarchy_type character varying(100) ;
alter table item_template_duct add column  hierarchy_type character varying(100) ;
alter table item_template_cdb add column  hierarchy_type character varying(100) ;
alter table item_template_adb add column  hierarchy_type character varying(100) ;
alter table isp_template_opticalrepeater add column  hierarchy_type character varying(100) ;
alter table item_template_coupler add column  hierarchy_type character varying(100) ;
alter table item_template_patchcord add column  hierarchy_type character varying(100) ;
alter table item_template_splitter add column  hierarchy_type character varying(100) ;
alter table isp_template_htb add column  hierarchy_type character varying(100) ;
alter table item_template_spliceclosure add column  hierarchy_type character varying(100) ;
alter table item_template_mpod add column  hierarchy_type character varying(100) ;
alter table item_template_fms add column  hierarchy_type character varying(100) ;
alter table item_template_trench add column  hierarchy_type character varying(100) ;
alter table isp_template_fdb add column  hierarchy_type character varying(100) ;
alter table item_template_tree add column  hierarchy_type character varying(100) ;
alter table item_template_pod add column  hierarchy_type character varying(100) ;
alter table item_template_ont add column  hierarchy_type character varying(100) ;
alter table item_template_wallmount add column  hierarchy_type character varying(100) ;
alter table item_template_bdb add column  hierarchy_type character varying(100) ;
alter table item_template_manhole add column  hierarchy_type character varying(100) ;
alter table item_template_microduct add column  hierarchy_type character varying(100) ;
alter table item_template_pole add column  hierarchy_type character varying(100) ;
alter table item_template_cable add column  hierarchy_type character varying(100) ;
alter table item_template_cable add column  aerial_location character varying(100) ;
alter table item_template_manhole add column  aerial_location character varying(100) ;
alter table item_template_spliceclosure add column  aerial_location character varying(100) ;


INSERT INTO public.data_uploader_template(
	 layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value,
     column_sequence, max_length, created_by, created_on,  is_dropdown, display_column_data_type, is_kml_attribute,
     is_nullable, is_excel_attribute, is_template_column_required, is_enable_for_template_column,  is_allowed_for_update, 
    is_allowed_for_update_admin, is_cdb_attributes)
	VALUES (19, 'route_id', 'varchar', 'Route_Id', false, 'route Id', '101',
            46, 100, 1, now(), false, 'character varying', true,
            false, true, false, true, true, 
            false, false);
			
			
			
			


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
		origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,section_name, generic_section_name,hierarchy_type,aerial_location)
		SELECT v_network_code,v_network_name, a_network_id, b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
		rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
		subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),a_system_id,a_network_id,
		a_entity_type,b_system_id,b_network_id,b_entity_type,cable_category,sub_category,
		route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
		p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',
		section_name, generic_section_name,hierarchy_type,aerial_location
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










		-- FUNCTION: public.fn_uploader_insert_manhole(integer, integer)

		-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_manhole(integer, integer);

		CREATE OR REPLACE FUNCTION public.fn_uploader_insert_manhole(
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
		 section_name, generic_section_name,hierarchy_type,aerial_location
		)
		select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,rec.specification,
		rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),coalesce(rec.network_status,'P'),
		rec.construction_type,rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
		rec.is_virtual,'Own', 'DU', p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,
		rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,rec.area,rec.authority,rec.route_name,
		rec.section_name, rec.generic_section_name,rec.hierarchy_type,rec.aerial_location
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
		$BODY$;




		CREATE OR REPLACE FUNCTION public.fn_uploader_insert_spliceclosure(
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
		v_network_id_type character varying;
		v_latitude double precision;
		v_longitude double precision;
		v_input_port integer;
		v_output_port integer;
		v_is_virtual_port_allowed boolean;
		v_network_code_seperator character varying;
		v_geom_port character varying;
		v_display_name character varying;
		v_display_value character varying;
		v_str_system_id integer;
		v_flr_system_id integer;
		v_shaft_system_id integer;
		v_unit_id integer;
		v_auto_character_count integer;
		v_network_name character varying;
		BEGIN
		-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
		select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='SPLICECLOSURE';

		-- INSERT BATCH WISE RECORDS..
		FOR REC IN select * from temp_du_spliceclosure where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
		LOOP
		BEGIN
		v_network_name:=rec.spliceclosure_name;
		v_str_system_id:=0;
		v_unit_id:=0;
		v_shaft_system_id:=0;
		v_flr_system_id:=0;
		v_sequence_id:=0;

		if rec.no_of_input_port>0 and rec.no_of_output_port>0 then
		v_input_port:=rec.no_of_input_port;
		v_output_port:=rec.no_of_output_port;
		v_geom_port:=rec.no_of_input_port::character varying||':'||rec.no_of_output_port::character varying;
		else
		v_input_port:=rec.no_of_port;
		v_output_port:=rec.no_of_port;
		v_geom_port:=rec.no_of_port::character varying;
		end if;

		v_latitude:=rec.latitude::double precision;
		v_longitude:=rec.longitude::double precision;
		v_shaft_system_id:= rec.shaft_id;
		v_flr_system_id:= rec.floor_id;

		-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
		then
		v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then
		v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
		from vw_layer_mapping where upper(child_layer_name)=upper('SpliceClosure') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
		v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
		-- GET NETWORK CODE & PARENT DETAILS..
		select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
		from fn_get_clone_network_code('SpliceClosure', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
		v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
		end if;

		IF(upper(rec.parent_entity_type)='STRUCTURE')THEN
		select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
		where network_id=rec.parent_network_id;
		END IF;

		IF(upper(rec.parent_entity_type)!='STRUCTURE')
		THEN
		select id into v_unit_id
		from isp_entity_mapping where entity_id=rec.parent_system_id and upper(entity_type)=upper(rec.parent_entity_type);
		END IF;
		raise info'v_parent_system_id::%',v_parent_system_id;
		raise info'v_parent_network_id::%',v_parent_network_id;
		raise info'v_parent_entity_type::%',v_parent_entity_type;
		IF(coalesce(v_network_name,'')='')
		then
		v_network_name=v_network_code;
		END IF;
		--INSERT INTO MAIN TABLE
		insert into att_details_spliceclosure(network_id, spliceclosure_name, latitude, longitude, province_id,
		region_id, address, specification, category, subcategory1, subcategory2, subcategory3,
		item_code, vendor_id, status, created_by, created_on, network_status, parent_system_id, parent_network_id,
		parent_entity_type, sequence_id,type,brand,model,construction,activation,accessibility,no_of_input_port,
		no_of_output_port,no_of_ports,ownership_type, audit_item_master_id, structure_id, source_ref_type,
		source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,
		requested_by,request_approved_by,bom_sub_category,st_x,st_y, section_name, generic_section_name,hierarchy_type,aerial_location)
		select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,
		rec.specification,rec.category,
		rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),
		coalesce(rec.network_status,'P'),rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
		rec.no_of_input_port,rec.no_of_output_port,
		rec.no_of_port,'Own', rec.audit_item_master_id, v_str_system_id, 'DU', p_uploadid,
		rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
		rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,
		rec.section_name, rec.generic_section_name,rec.hierarchy_type,rec.aerial_location
		returning system_id into v_current_system_id;

		select clabel.display_column_name into v_display_name from display_name_settings clabel
		inner join layer_details ld on clabel.layer_id=ld.layer_id where upper(ld.layer_name)=upper('SpliceClosure');
		execute 'select '||v_display_name||' from att_details_spliceclosure where system_id='||v_current_system_id||'' into v_display_value;

		--INSERT INTO POINT MASTER
		insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
		creator_remark,approver_remark,created_by,approver_id, common_name, db_flag, network_status,no_of_ports,display_name,st_x,st_y)
		select v_current_system_id, 'SpliceClosure',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
		now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),v_geom_port,fn_get_display_name(v_current_system_id,'SpliceClosure'),rec.st_x,rec.st_y;

		-- INSERT PORT DETAILS..
		if v_is_virtual_port_allowed = false then
		perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'SpliceClosure', v_current_system_id, v_network_code,rec.created_by);
		end if;

		-- INSERT isp_entity_mapping..

		IF(v_str_system_id>0)THEN
		raise info'v_str_system_id::%',v_str_system_id;
		raise info'v_shaft_system_id::%',v_shaft_system_id;
		raise info'v_flr_system_id::%',v_flr_system_id;
		raise info'v_str_system_id::%',v_str_system_id;
		raise info'v_unit_id::%',v_unit_id;

		insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
		(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'SpliceClosure', v_current_system_id, coalesce(v_unit_id,0));
		End if;
		END;
		-- UPDATE STATUS INTO TEMP TABLE..
		update temp_du_spliceclosure set is_processed=true where system_id=rec.system_id;
		perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'SpliceClosure'::character varying ,rec.province_id::integer,0,false));

		END LOOP;
		return 1;
		END;
		$BODY$;			