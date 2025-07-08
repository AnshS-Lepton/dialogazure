
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
		Loop_rec RECORD;
        v_geom_rec RECORD;
		current_system_id integer;
		v_parent_system_id integer;
		v_parent_network_id character varying;
		v_parent_entity_type character varying;
        v_parent_system_id_loop integer;
        v_parent_network_id_loop character varying;
        v_parent_entity_type_loop character varying;
        v_network_id_type_loop character varying;
        v_loop_system_id integer;
		v_network_code character varying;
        v_network_code_loop character varying;
		v_sequence_id integer;
		v_sequence_id_loop integer;
		v_geom character varying;
		v_entity_name character varying;
		v_network_Id_type character varying;
		v_network_code_seperator character varying;
		v_a_display_value character varying;
		v_b_display_value character varying;
		v_auto_character_count integer;
		v_auto_character_count_loop integer;
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
        v_network_code_seperator_loop character varying;
        v_geometry_loop geometry;
        v_snapped_geom_loop geometry;
        v_latitude double precision;
        v_longitude double precision;
		sql text;
		BEGIN
        
		select entity_type into v_entity_name from upload_summary where id=p_upload_id;
		select network_id_type,network_code_seperator into v_network_Id_type,v_network_code_seperator from layer_details where upper(layer_name)=upper(v_entity_name);

		FOR v_geom_rec IN select DISTINCT sp_geometry from temp_du_cable t
		where upload_id=p_upload_id and is_valid=true and batch_id=p_batch_id
		LOOP
		BEGIN
        SELECT * INTO REC FROM temp_du_cable WHERE sp_geometry = v_geom_rec.sp_geometry
		AND upload_id = p_upload_id AND batch_id = p_batch_id AND is_valid = true LIMIT 1;
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
		rec.cable_calculated_length,coalesce(cable_type,'Overhead'), specification, category,subcategory1, subcategory2,
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
		rec.cable_calculated_length,coalesce(cable_type,'Overhead'), specification, category,subcategory1, subcategory2,
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

        SELECT region_id, province_id,loop_start_reading, loop_end_reading,loop_latitude,loop_longitude, 
        loop_parent_network_id, loop_parent_entity_type, loop_remarks
        INTO Loop_rec FROM temp_du_cable WHERE sp_geometry = rec.sp_geometry
		AND upload_id = p_upload_id AND batch_id = p_batch_id AND is_valid = true;
        
        Loop
        Begin
        v_latitude:=Loop_rec.loop_latitude::double precision;
         v_longitude:=Loop_rec.loop_longitude::double precision;
         v_sequence_id_loop:=0;
        select network_id_type,network_code_seperator into v_network_id_type_loop,v_network_code_seperator_loop from layer_details where upper(layer_name)='LOOP';

        -- GET NETWORK CODE & PARENT DETAILS..
        select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
        from fn_get_clone_network_code('Loop', 'Point',''||v_longitude||' '||v_latitude||'', cable_system_id, 'PROVINCE') into
        v_parent_system_id_loop,v_parent_network_id_loop,v_parent_entity_type_loop,v_network_code_loop,v_sequence_id_loop;
        
        select ST_ClosestPoint(sp_geometry,ST_GeomFromText('POINT('||v_longitude||' '||v_latitude||')',4326)) into v_geometry_loop from
        line_master where system_id=current_system_id and upper(entity_type)=upper('CABLE') ;
        
        v_longitude=st_x(v_geometry_loop);
        v_latitude=st_y(v_geometry_loop);
        
        INSERT INTO att_details_loop(network_id,loop_length,associated_system_id,associated_network_id,associated_entity_type,
        cable_system_id,network_status,start_reading,end_reading,parent_system_id,sequence_id,
         province_id,region_id,parent_network_id,parent_entity_type,
        source_ref_type,source_ref_id,created_by,latitude)

        values( v_network_code_loop,Loop_rec.loop_end_reading::integer- Loop_rec.loop_start_reading::integer,cable_system_id,v_network_code,'Cable',
              cable_system_id, 'P',Loop_rec.loop_start_reading, Loop_rec.loop_end_reading, cable_system_id,v_sequence_id_loop,
              Loop_rec.region_id, Loop_rec.province_id,v_network_code,'Cable','DU',p_upload_id, rec.created_by,v_latitude,v_longitude )
              returning system_id into v_loop_system_id;
              
        --PointMaster entry--
        INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
        created_by,common_name, db_flag,network_status,display_name)
        VALUES (v_loop_system_id,'Loop', Loop_rec.loop_longitude::double precision,Loop_rec.loop_latitude::double precision,'A',
        ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),now(),
        'NA','NA',rec.created_by,v_network_code,p_upload_id,'P',fn_get_display_name(v_loop_system_id,'Loop'));
        
        -- Snapping cable to loop
        SELECT ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01) into v_snapped_geom_loop
        FROM (
        SELECT SP_GEOMETRY AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = v_loop_system_id and lower(entity_type)=lower('Loop')
        ) AS point,
        LINE_MASTER AS LM WHERE LM.system_id = cable_system_id and lower(entity_type)=lower('cable');
        
        update LINE_MASTER set SP_GEOMETRY=v_snapped_geom_loop WHERE system_id = cable_system_id and lower(entity_type)=lower('cable');
       perform (fn_geojson_update_entity_attribute(v_loop_system_id::integer,'Loop'::character varying ,Loop_rec.province_id::integer,0,false));
       
        End;
        End Loop;
		END;

END LOOP;
		return 1;
		END;
		
$BODY$;


INSERT INTO public.data_uploader_template(
	layer_id, db_column_name, db_column_data_type, template_column_name, 
    is_mandatory, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown,
    display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, is_template_column_required, is_enable_for_template_column,
     is_cdb_attributes, is_overlapping_rule_enabled)
	VALUES ((select Layer_id from layer_details where layer_name='Cable'),'loop_start_reading', 'double', 'loop_start_reading',
            false,'loop start reading', '4567.345', 48, 100, 1, now(), 1, now(), false, 
            'double',false, true, true, false, false,  false, false),
            ((select Layer_id from layer_details where layer_name='Cable'),'loop_end_reading', 'double', 'loop_end_reading',
            false,'loop end reading', '4567.345', 49, 100, 1, now(), 1, now(), false, 
            'double',false, true, true, false, false,  false, false),
            ((select Layer_id from layer_details where layer_name='Cable'),'loop_latitude', 'double', 'loop_latitude',
            false,'loop latitude', '4567.345', 50, 100, 1, now(), 1, now(), false, 
            'double',false, true, true, false, false,  false, false),
            ((select Layer_id from layer_details where layer_name='Cable'),'loop_longitude', 'double', 'loop_longitude',
            false,'loop longitude', '4567.345', 51, 100, 1, now(), 1, now(), false, 
            'double',false, true, true, false, false,  false, false),
            ((select Layer_id from layer_details where layer_name='Cable'),'loop_remarks', 'varchar', 'loop_remarks',
            false,'loop Remarks', 'remarks for loop', 54, 100, 1, now(), 1, now(), false, 
            'Character Varying',false, true, true, false, false,  false, false);


alter table temp_du_cable add cable_network_id character varying;
alter table temp_du_cable add loop_start_reading double precision;
alter table temp_du_cable add loop_end_reading double precision;
alter table temp_du_cable add loop_latitude double precision;
alter table temp_du_cable add loop_longitude double precision;
alter table temp_du_cable add loop_parent_network_id character varying;
alter table temp_du_cable add loop_parent_entity_type character varying;
alter table temp_du_cable add loop_remarks character varying;