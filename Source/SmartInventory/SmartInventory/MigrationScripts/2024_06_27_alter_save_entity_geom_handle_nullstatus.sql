
CREATE OR REPLACE FUNCTION public.fn_save_entity_geom(
	p_system_id integer,
	p_geom_type character varying,
	p_entity_type character varying,
	p_userid integer,
	p_longlat character varying,
	p_common_name character varying,
	p_network_status character varying,
	p_ports character varying,
	p_entity_category character varying,
	p_center_line_geom character varying,
	p_buffer_width double precision,
	p_project_id integer)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE geo_data text;
s_longLat character varying;
sql text;
s_long text;
s_lat text;
v_radious double precision;
v_close_point character varying;
v_centroid geometry;
v_center_line_geom character varying;
v_display_name character varying;
v_display_value character varying;
v_layer_table character varying;
v_source_ref_id character varying;
v_source_ref_type character varying;
v_status character varying;

BEGIN

v_radious:=0;

select layer_table into v_layer_table from layer_details where Upper(layer_name) = upper(p_entity_type) ;
raise info 'check layertable:  %',v_layer_table; 
--select source_ref_id,source_ref_type,status from ''||v_layer_table||'' where system_id= p_system_id  into v_source_ref_id,v_source_ref_type,v_status  ;

EXECUTE FORMAT('SELECT source_ref_id, source_ref_type, status 
                    FROM %I 
                    WHERE system_id = $1', v_layer_table)
    INTO v_source_ref_id, v_source_ref_type, v_status
    USING p_system_id;
v_status:=coalesce(v_status,'');
v_source_ref_id:=coalesce(v_source_ref_id,'');
v_source_ref_type:=coalesce(v_source_ref_type,'');
	if lower(p_geom_type)='point' then
	s_longLat:='POINT('||p_longlat||')';
	geo_data := 'ST_GeomFromText('''||s_longLat||''',4326)';
	s_long:= SPLIT_PART(''||p_longlat||'', ' ',1);
	s_lat:= SPLIT_PART(''||p_longlat||'', ' ',2);
	
	sql:='insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,modified_on,network_status,no_of_ports,entity_category,display_name,project_id,status,source_ref_id,source_ref_type)
	values('||p_system_id||','''||p_entity_type||''','''||s_long||''','''||s_lat||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),'''||p_network_status||''','''||p_ports||''','''||p_entity_category||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';
	
	end if;

	if lower(p_geom_type)='line' then
	s_longLat := 'LINESTRING('||p_longlat||')';
	geo_data := 'ST_GeomFromText('''|| s_longLat||''',4326)';
	sql:='INSERT INTO line_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,display_name,project_id,status,source_ref_id,source_ref_type)
	VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';
	end if;

	if lower(p_geom_type)='polygon' then
	s_longLat := 'POLYGON(('||p_longlat||'))';
	geo_data := 'ST_GeomFromText(''' ||s_longLat ||''',4326)';
	if(upper(p_entity_type)=upper('ROW'))
	then
	sql:='INSERT INTO polygon_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,center_line_geom,buffer_width,display_name,project_id,status,source_ref_id,source_ref_type)
	VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',ST_GeomFromText(''LINESTRING('||p_center_line_geom||')'',4326),'||p_buffer_width||',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''') ';
	else
	
	sql:='INSERT INTO polygon_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,display_name,project_id,status,source_ref_id,source_ref_type)
	VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''') ';
	end if;

end if;

if lower(p_geom_type)='circle' then
s_longLat := 'POLYGON(('||p_longlat||'))';
geo_data := 'ST_GeomFromText(''' ||s_longLat ||''',4326)';

select Cast(st_distance(ST_centroid('POLYGON(('||p_longlat||'))'),(ST_DumpPoints('POLYGON(('||p_longlat||'))')).geom,false)as decimal(10,2)) into v_radious limit 1;

select center_line_geom into v_center_line_geom
from polygon_master pm inner join att_details_row_pit pit on pm.system_id=pit.parent_system_id and upper(pm.entity_type)=upper('ROW') where pit.system_id=p_system_id;

v_close_point:='ST_ClosestPoint('''||v_center_line_geom||''', ST_centroid('||geo_data||')) ';

geo_data:='ST_buffer_meters('||v_close_point||','||coalesce(v_radious,1)||')';
sql:='INSERT INTO circle_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,sp_center,radious,display_name,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''','||v_close_point||','||coalesce(v_radious,0)||',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';

end if;
 -- raise info 'SQL %',SQL;
IF UPPER(P_GEOM_TYPE)='POINT' AND NOT EXISTS(SELECT 1 FROM POINT_MASTER PM
INNER JOIN LAYER_DETAILS LD ON UPPER(PM.COMMON_NAME)=UPPER(P_COMMON_NAME) AND UPPER(PM.ENTITY_TYPE)=UPPER(LD.LAYER_NAME) 
AND IS_MiDDLEWARE_ENTITY=TRUE)
THEN
EXECUTE SQL;
ELSIF UPPER(P_GEOM_TYPE)!='POINT'
THEN
EXECUTE SQL;
END IF;
END
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
		rec.section_name, rec.generic_section_name,rec.hierarchy_type,rec.aerial_location,rec.route_id
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

insert into LAYER_MAPPING(layer_id,parent_layer_id,parent_sequence,
	 is_enable_inside_parent_info,
	is_used_for_network_id,network_code_format,is_default_code_format,is_default_parent)
values
(20, 13, 7, false, true, 'MHxxxx', true, false),
(36, 10, 7, false, true, 'PODxxxx', true, false),
(20, 14, 7, false, true, 'POLxxxx', true, false);
