
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
		 section_name, generic_section_name,hierarchy_type,aerial_location,route_id
		)
		select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,rec.specification,
		rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),coalesce(rec.network_status,'P'),
		rec.construction_type,rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
		rec.is_virtual,'Own', 'DU', p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,
		rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,rec.area,rec.authority,rec.route_name,
		rec.section_name, rec.generic_section_name,rec.hierarchy_type,rec.aerial_location,rec.route_id
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
		requested_by,request_approved_by,bom_sub_category,st_x,st_y, section_name, generic_section_name,hierarchy_type,aerial_location,route_id)
		select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,
		rec.specification,rec.category,
		rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),
		coalesce(rec.network_status,'P'),rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
		rec.no_of_input_port,rec.no_of_output_port,
		rec.no_of_port,'Own', rec.audit_item_master_id, v_str_system_id, 'DU', p_uploadid,
		rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
		rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,
		rec.section_name, rec.generic_section_name,rec.hierarchy_type,rec.aerial_location,route_id
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




drop view vw_att_details_manhole_report;
CREATE OR REPLACE VIEW public.vw_att_details_manhole_report
 AS
 SELECT manhole.system_id,
    manhole.network_id,
    manhole.manhole_name,
    round(manhole.latitude::numeric, 6) AS latitude,
    round(manhole.longitude::numeric, 6) AS longitude,
    manhole.address,
    manhole.specification,
    manhole.category,
    manhole.subcategory1,
    manhole.subcategory2,
    manhole.subcategory3,
    manhole.item_code,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    fn_get_network_status(manhole.network_status) AS network_status,
    COALESCE(fn_get_entity_status(manhole.status), manhole.status) AS status,
    prov.province_name,
    reg.region_name,
    manhole.region_id,
    manhole.province_id,
    manhole.parent_network_id AS parent_code,
    manhole.parent_entity_type AS parent_type,
        CASE
            WHEN manhole.is_virtual = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_virtual,
    manhole.construction_type,
    manhole.acquire_from,
    vm.name AS vendor_name,
    fn_get_date(manhole.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(manhole.modified_on) AS modified_on,
    um2.user_name AS modified_by,
        CASE
            WHEN manhole.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    manhole.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
        CASE
            WHEN manhole.is_buried = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_buried,
    manhole.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    manhole.source_ref_type,
    manhole.source_ref_id,
    manhole.source_ref_description,
    manhole.status_remark,
    manhole.status_updated_by,
    fn_get_date(manhole.status_updated_on) AS status_updated_on,
    manhole.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    manhole.remarks,
    manhole.manhole_types,
    reg.country_name,
    manhole.origin_from,
    manhole.origin_ref_id,
    manhole.origin_ref_code,
    manhole.origin_ref_description,
    manhole.request_ref_id,
    manhole.requested_by,
    manhole.request_approved_by,
    manhole.subarea_id,
    manhole.area_id,
    manhole.dsa_id,
    manhole.csa_id,
    manhole.bom_sub_category,
    manhole.gis_design_id AS design_id,
    manhole.st_x,
    manhole.st_y,
    manhole.ne_id,
    manhole.prms_id,
    manhole.mzone_id,
    manhole.jc_id,
    manhole.area,
    manhole.authority,
    manhole.route_name,
    manhole.served_by_ring,
    manhole.mcgm_ward,
    manhole.hierarchy_type,
    manhole.section_name,
    manhole.generic_section_name,
    manhole.aerial_location,
    manhole.route_id,
    vm3.name AS own_vendor_name,
    manhole.chamber_remark
   FROM att_details_manhole manhole
     JOIN province_boundary prov ON manhole.province_id = prov.id
     JOIN region_boundary reg ON manhole.region_id = reg.id
     JOIN user_master um ON manhole.created_by = um.user_id
     JOIN vendor_master vm ON manhole.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON manhole.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON manhole.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON manhole.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON manhole.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON manhole.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON manhole.purpose_id = purposem.system_id
     LEFT JOIN att_details_pod primarypod ON manhole.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON manhole.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN vendor_master vm3 ON manhole.own_vendor_id::integer = vm3.id;
	 
	 
	 
	 
	 
	  DROP VIEW public.vw_att_details_spliceclosure_report;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_report
 AS
 SELECT sc.system_id,
    sc.network_id,
    sc.spliceclosure_name,
    round(sc.latitude::numeric, 6) AS latitude,
    round(sc.longitude::numeric, 6) AS longitude,
    sc.address,
    sc.specification,
    sc.category,
    sc.subcategory1,
    sc.subcategory2,
    sc.subcategory3,
    sc.item_code,
        CASE
            WHEN sc.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN sc.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN sc.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, sc.status) AS status,
    sc.pincode,
    prov.province_name,
    reg.region_name,
    sc.parent_network_id AS parent_code,
    sc.parent_entity_type AS parent_type,
    vm.name AS vendor_name,
    sc.no_of_ports,
    sc.no_of_input_port,
    sc.no_of_output_port,
    sc.acquire_from,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    um.user_name AS created_by,
    to_char(sc.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(sc.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    sc.region_id,
    sc.province_id,
        CASE
            WHEN sc.is_virtual = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_virtual,
        CASE
            WHEN sc.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    sc.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    sc.utilization,
        CASE
            WHEN sc.utilization::text = 'L'::text THEN 'Low'::text
            WHEN sc.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN sc.utilization::text = 'H'::text THEN 'High'::text
            WHEN sc.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
        CASE
            WHEN sc.is_buried = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_buried,
    sc.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    sc.source_ref_type,
    sc.source_ref_id,
    sc.source_ref_description,
    sc.status_remark,
    sc.status_updated_by,
    to_char(sc.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    sc.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    sc.remarks,
    reg.country_name,
    sc.origin_from,
    sc.origin_ref_id,
    sc.origin_ref_code,
    sc.origin_ref_description,
    sc.request_ref_id,
    sc.requested_by,
    sc.request_approved_by,
    sc.subarea_id,
    sc.area_id,
    sc.dsa_id,
    sc.csa_id,
    sc.bom_sub_category,
    sc.gis_design_id AS design_id,
    sc.st_x,
    sc.st_y,
    sc.ne_id,
    sc.prms_id,
    sc.jc_id,
    sc.mzone_id,
    sc.served_by_ring,
    sc.hierarchy_type,
    sc.section_name,
    sc.generic_section_name,
    sc.aerial_location,
    sc.route_id,
    vm3.name AS own_vendor,
    sc.spliceclosure_type
   FROM att_details_spliceclosure sc
     JOIN province_boundary prov ON sc.province_id = prov.id
     JOIN region_boundary reg ON sc.region_id = reg.id
     JOIN user_master um ON sc.created_by = um.user_id
     JOIN vendor_master vm ON sc.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON sc.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON sc.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON sc.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON sc.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON sc.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON sc.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON sc.modified_by = um2.user_id
     LEFT JOIN entity_notification_status entnotifystatus ON sc.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON sc.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON sc.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = sc.status::text;











 DROP VIEW public.vw_att_details_cable_report;

CREATE OR REPLACE VIEW public.vw_att_details_cable_report
 AS
 SELECT cable.system_id,
    cable.network_id,
    cable.cable_name,
    cable.total_core,
    cable.no_of_tube,
    cable.no_of_core_per_tube,
    round(cable.cable_measured_length::numeric, 2) AS cable_measured_length,
    round(cable.cable_calculated_length::numeric, 2) AS cable_calculated_length,
    cable.cable_type,
    cable.coreaccess,
    cable.wavelength,
    cable.optical_output_power,
    cable.frequency,
    cable.attenuation_db,
    cable.resistance_ohm,
    cable.construction,
    cable.activation,
    cable.accessibility,
    cable.specification,
    cable.category,
    cable.subcategory1,
    cable.subcategory2,
    cable.subcategory3,
    cable.item_code,
        CASE
            WHEN cable.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN cable.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN cable.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.status, cable.status) AS status,
    cable.pin_code,
    cable.utilization,
    cable.totalattenuationloss,
    cable.chromaticdb,
    cable.chromaticdispersion,
    cable.totalchromaticloss,
    cable.remarks,
    cable.a_entity_type,
    cable.b_entity_type,
    cable.cable_category,
    cable.execution_method,
    cable.total_loop_count,
    cable.total_loop_length,
    cable.parent_entity_type AS parent_type,
    cable.parent_network_id AS parent_code,
    cable.route_id,
    round(cable.start_reading::numeric, 2) AS start_reading,
    round(cable.end_reading::numeric, 2) AS end_reading,
    cable.cable_sub_category,
    um.user_name AS created_by,
    to_char(cable.created_on, 'DD-Mon-YY'::text) AS created_on,
    to_char(cable.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    cable.drum_no,
    cable.acquire_from,
    cable.circuit_id,
    cable.thirdparty_circuit_id,
    cable.province_id,
    cable.region_id,
        CASE
            WHEN cable.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    reg.region_name,
    cable.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    prov.province_name,
        CASE
            WHEN cable.utilization::text = 'L'::text THEN 'Low'::text
            WHEN cable.utilization::text = 'M'::text THEN 'Moderate'::text
            WHEN cable.utilization::text = 'H'::text THEN 'High'::text
            WHEN cable.utilization::text = 'O'::text THEN 'Over'::text
            ELSE NULL::text
        END AS utilization_text,
        CASE
            WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
            ELSE 'Blocked'::text
        END AS notification_status,
    cable.ownership_type,
    vm.name AS vendor_name,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    cable.source_ref_type,
    cable.source_ref_id,
    cable.source_ref_description,
    cable.status_remark,
    cable.status_updated_by,
    to_char(cable.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    cable.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    round(cable.inner_dimension::numeric, 2) AS inner_dimension,
    round(cable.outer_dimension::numeric, 2) AS outer_dimension,
    cable.calculated_length_remark,
    reg.country_name,
    cable.origin_from,
    cable.origin_ref_id,
    cable.origin_ref_code,
    cable.origin_ref_description,
    cable.request_ref_id,
    cable.requested_by,
    cable.request_approved_by,
    cable.subarea_id,
    cable.area_id,
    cable.dsa_id,
    cable.csa_id,
    cable.bom_sub_category,
    cable.gis_design_id AS evoid,
    cable.ne_id,
    cable.prms_id,
    cable.jc_id,
    cable.mzone_id,
    cable.served_by_ring,
    cable.a_latitude,
    cable.b_longitude,
    cable.a_region,
    cable.b_region,
    cable.a_city,
    cable.b_city,
    (( SELECT count(*) AS count
           FROM att_details_cable_info cableinfo
          WHERE cableinfo.cable_id = cable.system_id AND cableinfo.link_system_id > 0)) + (( SELECT count(*) AS count
           FROM att_details_cable_info cableinfo
          WHERE cableinfo.cable_id = cable.system_id AND lower(cableinfo.core_comment::text) ~~* lower('reserved%'::text))) AS used_core,
    cable.hierarchy_type,
    cable.section_name,
    cable.aerial_location,
    cable.generic_section_name,
    vm3.name AS own_vendor,
    cable.cable_remark
   FROM att_details_cable cable
     JOIN user_master um ON cable.created_by = um.user_id
     JOIN vendor_master vm ON cable.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON cable.third_party_vendor_id = vm2.id
     LEFT JOIN user_master um2 ON cable.modified_by = um2.user_id
     LEFT JOIN att_details_project_master pm ON cable.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON cable.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON cable.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON cable.purpose_id = purposem.system_id
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(cable.network_id::text) ~~ upper('NLD%'::text) THEN cable.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('CABLE'::text) AND cable.province_id = pmapping.province_id
            ELSE 1 = 2
        END
     JOIN province_boundary prov ON prov.id = COALESCE(pmapping.province_id, cable.province_id)
     JOIN region_boundary reg ON reg.id = COALESCE(pmapping.region_id, cable.region_id)
     LEFT JOIN entity_notification_status entnotifystatus ON cable.notification_status_id = entnotifystatus.id
     LEFT JOIN att_details_pod primarypod ON cable.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON cable.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN vendor_master vm3 ON vm3.id = cable.own_vendor_id::integer
     LEFT JOIN entity_status_master es ON es.status::text = cable.status::text;
	 
	 
	 
	 INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 13, 'Aerial_Location', 'Aerial Start', true, 1, now(),'Aerial Start', 'Aerial_Location', false, true, 0);
  
   INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 13, 'Aerial_Location', 'Aerial End', true, 1, now(),'Aerial End', 'Aerial_Location', false, true, 0);
    
     INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 13, 'Aerial_Location', 'Aerial End & Start', true, 1, now(),'Aerial End & Start', 'Aerial_Location', false, true, 0);
    
       INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 19, 'Aerial_Location', 'Aerial Start', true, 1, now(),'Aerial Start', 'Aerial_Location', false, true, 0);
  
   INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 19, 'Aerial_Location', 'Aerial End', true, 1, now(),'Aerial End', 'Aerial_Location', false, true, 0);
    
     INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 19, 'Aerial_Location', 'Aerial End & Start', true, 1, now(),'Aerial End & Start', 'Aerial_Location', false, true, 0);
    
    
    
     INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 20, 'Aerial_Location', 'Aerial Start', true, 1, now(),'Aerial Start', 'Aerial_Location', false, true, 0);
  
   INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 20, 'Aerial_Location', 'Aerial End', true, 1, now(),'Aerial End', 'Aerial_Location', false, true, 0);
    
     INSERT INTO public.dropdown_master(
	 layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, dropdown_key,  db_column_name, is_action_allowed, is_active, parent_id)
	VALUES ( 20, 'Aerial_Location', 'Aerial End & Start', true, 1, now(),'Aerial End & Start', 'Aerial_Location', false, true, 0);
    
    


















