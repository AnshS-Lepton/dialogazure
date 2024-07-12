-- FUNCTION: public.fn_uploader_insert_fdb(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_fdb(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_fdb(
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
v_str_system_id integer;
v_unit_id integer;
v_flr_system_id integer;
v_shaft_system_id integer;
v_auto_character_count integer;
v_network_name character varying;
v_fdb_structure_id integer;
v_building_id integer;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='FDB';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_fdb where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN
v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;
v_network_name:=rec.fdb_name;
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
--v_flr_system_id:= rec.floor_id;
if(UPPER(rec.parent_entity_type)=UPPER('Structure')) then 
if exists (Select floor.structure_id, floor.system_id  from 
att_details_bld_structure str 
inner join isp_floor_info floor  
on str.system_id = floor.structure_id and UPPER(rec.floor_name)  = UPPER(replace(floor.floor_name,'_',''))
where    str.origin_ref_id = ''||rec.parent_client_id ||'' order by floor.system_id desc limit 1) then 

Select floor.structure_id, floor.system_id into v_fdb_structure_id,v_flr_system_id  from 
att_details_bld_structure str 
inner join isp_floor_info floor  
on str.system_id = floor.structure_id and UPPER(rec.floor_name)  = UPPER(replace(floor.floor_name,'_',''))
where    str.origin_ref_id = ''||rec.parent_client_id ||'' order by floor.system_id desc limit 1;

else 
v_flr_system_id:= rec.floor_id;
end if;
else 
v_flr_system_id:= rec.floor_id;
end if;

if(rec.origin_Ref_id is not null and rec.origin_from='SP') then 
		      Select str.system_id,str.network_id,str.building_id,floor.system_id
		      into v_parent_system_id,v_parent_network_id,v_building_id,v_flr_system_id
		     from  temp_du_fdb bdb inner join att_details_bld_structure str on
		    cast(bdb.parent_client_id as character varying) =str.origin_Ref_id 
			inner join isp_floor_info floor  on str.system_id = floor.structure_id	
			and 	UPPER(bdb.floor_name)  = replace(UPPER(floor.floor_name),'_',' ' )
		where bdb.system_id =rec.system_id order by 1 desc limit 1;
		   
		   update temp_du_fdb set parent_system_id =v_parent_system_id, parent_network_id =v_parent_network_id
		   where system_id  =rec.system_id;
			 
		    select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('FDB', 'Point',''||v_longitude||' '||v_latitude||'', v_parent_system_id, rec.parent_entity_type) order by 1 desc  limit 1 into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
			

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
elseif (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('FDB') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('FDB', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id,
rec.parent_entity_type) order by o_sequence_id desc limit 1 into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
if(v_network_code ='' or v_network_code is null) then 

v_network_code := rec.parent_network_id ||'-FDB'||v_sequence_id;
end if;

end if;

IF(upper(rec.parent_entity_type)='STRUCTURE')THEN

if(rec.parent_client_id is not null) then
v_str_system_id := v_parent_system_id;
 --select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
--where origin_ref_id = ''|| rec.parent_client_id || ''  order by 1 desc limit 1;
else
select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
where network_id=rec.parent_network_id;
end if;
end if;
--IF(upper(rec.parent_entity_type)='STRUCTURE')THEN
--select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
--where network_id=rec.parent_network_id;
--END IF;

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
--INSERT DATA INTO INSERT TABLE
insert into isp_fdb_info(network_id, fdb_name, latitude, longitude, province_id, region_id,
specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id,
status,created_by, created_on, network_status, parent_system_id, parent_network_id, parent_entity_type,
sequence_id,type,brand,model,construction,activation,accessibility,structure_id,floor_id,
shaft_id,no_of_input_port,no_of_output_port,no_of_port,ownership_type, audit_item_master_id, source_ref_type,
source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,
request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y
)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A', rec.created_by,
now(), coalesce(rec.network_status,'P'),parent_system_id,parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,0,0,0,
coalesce(v_fdb_structure_id,0),v_flr_system_id,rec.shaft_id,v_input_port,v_output_port,v_input_port,'Own', rec.audit_item_master_id, 'DU'
, p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
from temp_du_fdb where system_id =rec.system_id
returning system_id into v_current_system_id;

--INSERT DATA INTO POINT TABLE
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,
approver_id, common_name, db_flag,no_of_ports, network_status,display_name,st_x,st_y
)
select v_current_system_id,'FDB',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id,v_geom_port, coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'FDB'),rec.st_x,rec.st_y;


--INSERT INTO ASSOCIATION TABLE
		IF(coalesce(REC.PARENT_ENTITY_TYPE,'')!='' and  coalesce(REC.PARENT_ENTITY_TYPE,'')!='Province')
		THEN			
			insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,associated_system_id,associated_network_id,
			associated_entity_type,created_on,created_by,is_termination_point,entity_display_name,associated_display_name)
			select parent_system_id,parent_network_id,REC.PARENT_ENTITY_TYPE,v_current_system_id,v_network_code,
			'FDB',now(),rec.created_by,false,parent_network_id,v_network_code from temp_du_fdb where system_id =rec.system_id;
		END IF;


-- FDB MDU MAPPING IS PENDING..

if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'FDB', v_current_system_id, v_network_code,rec.created_by);
end if;

-- INSERT isp_entity_mapping..

IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;

insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'FDB', v_current_system_id, coalesce(v_unit_id,0));
End if;

END;
-- UPDATE STATUS INTO TEMP TABLE..
--update temp_du_fdb set is_processed=true where system_id=rec.system_id;

update temp_du_fdb set is_processed=true, system_network_id= v_network_code where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'FDB'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_fdb(integer, integer)
    OWNER TO postgres;


-- FUNCTION: public.fn_uploader_insert_spliceclosure(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_spliceclosure(integer, integer);

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

		--INSERT INTO ASSOCIATION TABLE
		IF(coalesce(REC.PARENT_ENTITY_TYPE,'')!='' and  coalesce(REC.PARENT_ENTITY_TYPE,'')!='Province')
		THEN			
			insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,associated_system_id,associated_network_id,
			associated_entity_type,created_on,created_by,is_termination_point,entity_display_name,associated_display_name)
			select rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_current_system_id,v_network_code,
			'SpliceClosure',now(),rec.created_by,false,rec.parent_network_id,v_network_code;
		END IF;
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

ALTER FUNCTION public.fn_uploader_insert_spliceclosure(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_bdb(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_bdb(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_bdb(
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
v_unit_system_id integer;
v_str_system_id integer;
v_shaft_system_id integer;
v_unit_id integer;
v_flr_system_id integer;
v_auto_character_count integer;
v_network_name character varying;
v_building_id integer;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='BDB';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_bdb where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN
v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;
v_network_name:=rec.bdb_name;
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

if(UPPER(rec.parent_entity_type)!=UPPER('Structure')) then 

v_flr_system_id:= rec.floor_id;
end if;

if(rec.origin_Ref_id is not null and rec.origin_from='SP') then 
		      Select str.system_id,str.network_id,str.building_id,floor.system_id
		      into v_parent_system_id,v_parent_network_id,v_building_id,v_flr_system_id
		     from  temp_du_bdb bdb inner join att_details_bld_structure str on
		    cast(bdb.parent_client_id as character varying) =str.origin_Ref_id 
			inner join isp_floor_info floor  on str.system_id = floor.structure_id	
			and 	UPPER(bdb.floor_name)  = replace(UPPER(floor.floor_name),'_',' ' )
		where bdb.system_id =rec.system_id order by 1 desc limit 1;
		   
		   update temp_du_bdb set parent_system_id =v_parent_system_id, parent_network_id =v_parent_network_id
		   where system_id  =rec.system_id;
			 
		    select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('BDB', 'Point',''||v_longitude||' '||v_latitude||'', v_parent_system_id, rec.parent_entity_type) order by 1 desc  limit 1 into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
			

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
elseif (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('BDB') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('BDB', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) limit 1 into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(upper(rec.parent_entity_type)='STRUCTURE')THEN
if(rec.parent_client_id is not null) then
 select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
where origin_ref_id = ''|| rec.parent_client_id || ''  order by 1 desc limit 1;
else
select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
where network_id=rec.parent_network_id;
end if;

end if;
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
--INSERT DATA INTO MAIN TABLE
insert into att_details_bdb(network_id, bdb_name, latitude, longitude, province_id, region_id, address, specification, category,
subcategory1, subcategory2, subcategory3, item_code, vendor_id, status, created_by, created_on, network_status, parent_system_id,
parent_network_id, parent_entity_type, sequence_id, entity_category,type,brand,model,construction,activation,
accessibility,no_of_input_port,no_of_output_port,no_of_port,ownership_type, audit_item_master_id, source_ref_type,
source_ref_id,remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id
,requested_by,request_approved_by,bom_sub_category,floor_id,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.address, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A', rec.created_by,
now(), coalesce(rec.network_status,'P'),
parent_system_id,parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,rec.entity_category,0,0,0,0,0,0, rec.no_of_output_port,rec.no_of_output_port,rec.no_of_port,'Own', rec.audit_item_master_id, 'DU',
p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,
rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',v_flr_system_id,rec.st_x,rec.st_y
from temp_du_bdb where system_id = rec.system_id
returning system_id into v_current_system_id;

--INSERT DATA INTO POINT TABLE
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,
approver_id, common_name, db_flag,no_of_ports, network_status,entity_category,display_name,st_x,st_y)
select v_current_system_id,'BDB',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id,v_geom_port,coalesce(rec.network_status,'P'), rec.entity_category ,fn_get_display_name(v_current_system_id,'BDB'),rec.st_x,rec.st_y;

--INSERT INTO ASSOCIATION TABLE
		IF(coalesce(REC.PARENT_ENTITY_TYPE,'')!='' and  coalesce(REC.PARENT_ENTITY_TYPE,'')!='Province')
		THEN			
			insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,associated_system_id,associated_network_id,
			associated_entity_type,created_on,created_by,is_termination_point,entity_display_name,associated_display_name)
			select parent_system_id,parent_network_id,REC.PARENT_ENTITY_TYPE,v_current_system_id,v_network_code,
			'BDB',now(),rec.created_by,false,parent_network_id,v_network_code from temp_du_bdb where system_id = rec.system_id;
		END IF;

if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'BDB', v_current_system_id, v_network_code,rec.created_by);
end if;

-- INSERT isp_entity_mapping..

IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;

insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'BDB', v_current_system_id, coalesce(v_unit_id,0));
End if;
END;

-- UPDATE STATUS INTO TEMP TABLE..
--update temp_du_bdb set is_processed=true where system_id=rec.system_id;
update temp_du_bdb set is_processed=true, system_network_id= v_network_code where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'BDB'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_bdb(integer, integer)
    OWNER TO postgres;



CREATE OR REPLACE FUNCTION public.fn_uploader_validate_line_parent_details(
	p_upload_id integer,
	p_entity_type character varying)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

declare
RESULT INT:=0;
SQL CHARACTER VARYING;
V_LAYER_ID INTEGER;
REC RECORD;
REC_IS_DEFAULT_PARENT RECORD;
V_GEOM_TYPE CHARACTER VARYING;
V_DATA_UPLOAD_TABLE CHARACTER VARYING;
V_TP_TYPE CHARACTER VARYING;
V_USER_ID INTEGER;
V_LAYER_MAPPING RECORD;
V_TEMP_TABLE CHARACTER VARYING;
V_PLAN_ID CHARACTER VARYING;
V_TEMP_UPLOAD_TABLE  CHARACTER VARYING;
V_GEOM_CABLE GEOMETRY;
begin							

	SELECT USER_ID,PLAN_ID 
	INTO V_USER_ID,V_PLAN_ID 
	FROM UPLOAD_SUMMARY 
	WHERE ID=P_UPLOAD_ID LIMIT 1;
	
	SELECT LAYER_ID,GEOM_TYPE,DATA_UPLOAD_TABLE 
	INTO V_LAYER_ID,V_GEOM_TYPE,V_DATA_UPLOAD_TABLE 
	FROM LAYER_DETAILS 
	WHERE UPPER(LAYER_NAME)=UPPER(P_ENTITY_TYPE);

	
	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T  
	SET IS_VALID=(SELECT IS_VALID FROM FN_ISVALIDGEOMETRY('''||(CASE WHEN UPPER(V_GEOM_TYPE)=UPPER('LINE') 
	THEN 'LINESTRING' ELSE UPPER(V_GEOM_TYPE) END)||''', ''''||SP_GEOMETRY||'''')),  
	ERROR_MSG=(SELECT MESSAGE FROM FN_ISVALIDGEOMETRY('''||(CASE WHEN UPPER(V_GEOM_TYPE)=UPPER('LINE') 
	THEN 'LINESTRING' ELSE UPPER(V_GEOM_TYPE) END)||''', ''''||SP_GEOMETRY||'''')) 
	WHERE  T.IS_VALID=TRUE AND UPLOAD_ID='||P_UPLOAD_ID||''; 
	EXECUTE SQL;

	SELECT * INTO V_LAYER_MAPPING FROM VW_LAYER_MAPPING WHERE CHILD_LAYER_ID=V_LAYER_ID AND PARENT_LAYER_ID>0 LIMIT 1;  
	------------------  FOR CONVERTING GEOMETRY MUTLILINESTRING INTO LINESTRING  ----------------
	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET SP_GEOMETRY=(SELECT ST_ASTEXT(ST_SETSRID(ST_MAKELINE(GEOM),4326)) FROM (
	SELECT (ST_DUMPPOINTS(ST_GEOMFROMTEXT(T2.SP_GEOMETRY,4326))).GEOM
	FROM '||V_DATA_UPLOAD_TABLE||' T2  WHERE T2.SYSTEM_ID=T.SYSTEM_ID)A) 
	WHERE UPPER(T.SP_GEOMETRY)  ILIKE ''MULTILINESTRING%'' AND COALESCE(T.SP_GEOMETRY,'''')!='''' AND T.UPLOAD_ID='||P_UPLOAD_ID||'';
	EXECUTE SQL;

	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' SET SP_GEOMETRY=''LINESTRING(''||SP_GEOMETRY||'')'' 
	WHERE UPPER(SP_GEOMETRY) NOT LIKE ''LINESTRING%'' AND COALESCE(SP_GEOMETRY,'''')!='''' AND UPLOAD_ID='||P_UPLOAD_ID||'';
	EXECUTE SQL;

	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET PARENT_NETWORK_ID=COALESCE(I.PROVINCE_ABBREVIATION,'''') 
	FROM PROVINCE_BOUNDARY I WHERE st_within(st_startpoint(ST_GEOMFROMTEXT(T.SP_GEOMETRY,4326)),I.SP_GEOMETRY ) AND 
	UPLOAD_ID='||P_UPLOAD_ID||' AND UPPER(T.PARENT_ENTITY_TYPE) = UPPER(''PROVINCE'') AND I.ISVISIBLE=TRUE 
	AND I.PROVINCE_ABBREVIATION IS NOT NULL AND T.IS_VALID=TRUE AND COALESCE(T.PARENT_NETWORK_ID,'''')='''' ';		
	EXECUTE SQL;

	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' SET IS_VALID=FALSE,ERROR_MSG=''GEOMETRY NOT FOUND IN KML''
	WHERE IS_VALID=TRUE AND COALESCE(SP_GEOMETRY,'''')='''' AND UPLOAD_ID='||P_UPLOAD_ID||'';
	EXECUTE SQL;

	IF(V_LAYER_MAPPING IS NOT NULL)
	THEN
		--UPDATING THE PARENT SYSTEM_ID,PARENT_ENTITY_TYPE,REGION_ID,PROVINCE_ID		
		SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET PARENT_SYSTEM_ID=P.SYSTEM_ID,
		PARENT_ENTITY_TYPE='''||V_LAYER_MAPPING.PARENT_LAYER_NAME||''',REGION_ID=P.REGION_ID,PROVINCE_ID=P.PROVINCE_ID';		
		SQL:=SQL||' FROM '||V_LAYER_MAPPING.PARENT_LAYER_TABLE||' P 
		WHERE T.PARENT_NETWORK_ID=P.NETWORK_ID AND T.UPLOAD_ID='||P_UPLOAD_ID||' AND T.IS_VALID=TRUE AND UPPER(T.PARENT_ENTITY_TYPE)=UPPER('''||V_LAYER_MAPPING.PARENT_LAYER_NAME||''') ';
		EXECUTE SQL;

		SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET IS_VALID=FALSE,
		ERROR_MSG=T.PARENT_ENTITY_TYPE||'' CAN NOT BE PARENT!'' 	
		WHERE  T.UPLOAD_ID='||P_UPLOAD_ID||' AND T.IS_VALID=TRUE AND UPPER(T.PARENT_ENTITY_TYPE) 
		NOT IN (SELECT UPPER(PARENT_LAYER_NAME) FROM VW_LAYER_MAPPING WHERE CHILD_LAYER_ID='||V_LAYER_ID||')';
		EXECUTE SQL;
	END IF;
					
	--VALIDATIING THE PARENT ENTITY TYPE
	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET IS_VALID=FALSE,
	ERROR_MSG=''PARENT ENTITY TYPE SHOULD BE PROVINCE!'' 
	WHERE UPPER(PARENT_ENTITY_TYPE)!=UPPER(''PROVINCE'') AND IS_VALID=TRUE AND COALESCE(T.PARENT_SYSTEM_ID,0)=0 AND UPLOAD_ID='||P_UPLOAD_ID||' ';			
	EXECUTE SQL;
				
	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' C SET PROVINCE_ID=I.ID,REGION_ID=I.REGION_ID FROM PROVINCE_BOUNDARY I
	WHERE st_within(st_startpoint(ST_GEOMFROMTEXT(C.SP_GEOMETRY,4326)),I.SP_GEOMETRY )  
	AND UPLOAD_ID='||P_UPLOAD_ID||' 
	AND I.ISVISIBLE=TRUE AND I.PROVINCE_ABBREVIATION IS NOT NULL AND IS_VALID=TRUE';
	RAISE INFO'SQL55%',SQL;	
	EXECUTE SQL;
	
	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' SET IS_VALID=FALSE,ERROR_MSG='''||P_ENTITY_TYPE||' FALLS OUTSIDE BOUNDARY'' 
	WHERE COALESCE(REGION_ID,0)=0 AND COALESCE(PROVINCE_ID,0)=0 AND IS_VALID=TRUE AND UPLOAD_ID='||P_UPLOAD_ID||' ;';
	EXECUTE SQL;

	--VALIDATIING THE PARENT NETWORK ID
	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T 
	SET IS_VALID=(UPPER(T.PARENT_NETWORK_ID)=UPPER(COALESCE(P.PROVINCE_ABBREVIATION,''''))),
	ERROR_MSG=CASE WHEN UPPER(T.PARENT_NETWORK_ID)!=UPPER(COALESCE(P.PROVINCE_ABBREVIATION,'''')) 
	THEN ''PARENT NETWORK ID SHOULD BE ''||P.PROVINCE_ABBREVIATION||''!'' END 
	FROM PROVINCE_BOUNDARY P 
	WHERE T.PROVINCE_ID=P.ID AND T.UPLOAD_ID='||P_UPLOAD_ID||' AND T.IS_VALID=TRUE 
	AND UPPER(T.PARENT_ENTITY_TYPE)=UPPER(''PROVINCE'')';				
	EXECUTE SQL;
			
	--VALIDATING THE A END TYPE 
	SQL:= 'UPDATE '||V_DATA_UPLOAD_TABLE||' T1 SET IS_VALID=FALSE, ERROR_MSG=T1.A_ENTITY_TYPE||'' CAN NOT BE THE TERMINATION POINT FOR '||P_ENTITY_TYPE||' ''
	WHERE IS_VALID=TRUE AND ((UPPER(COALESCE(T1.A_ENTITY_TYPE,''''))!='''' AND (SELECT COUNT(1) FROM GLOBAL_SETTINGS WHERE UPPER(KEY)=UPPER(''ISTERMINATIONPOINTENABLE'') AND VALUE=''1'')=0)
	OR (SELECT COUNT(1) FROM GLOBAL_SETTINGS WHERE UPPER(KEY)=UPPER(''ISTERMINATIONPOINTENABLE'') AND VALUE=''1'')>0)
	AND UPPER(T1.A_ENTITY_TYPE) NOT IN (SELECT UPPER(TP_LAYER_NAME) FROM VW_TERMINATION_POINT_MASTER WHERE LAYER_ID='||V_LAYER_ID||')';
	EXECUTE SQL;
	
	--VALIDATING THE B END TYPE 
	SQL:= 'UPDATE '||V_DATA_UPLOAD_TABLE||' T1 SET IS_VALID=FALSE, ERROR_MSG=T1.B_ENTITY_TYPE||'' CAN NOT BE THE TERMINATION POINT FOR '||P_ENTITY_TYPE||' ''
	WHERE  IS_VALID=TRUE AND ((UPPER(COALESCE(T1.B_ENTITY_TYPE,''''))!='''' AND (SELECT COUNT(1) FROM GLOBAL_SETTINGS WHERE UPPER(KEY)=UPPER(''ISTERMINATIONPOINTENABLE'') AND VALUE=''1'')=0)
	OR (SELECT COUNT(1) FROM GLOBAL_SETTINGS WHERE UPPER(KEY)=UPPER(''ISTERMINATIONPOINTENABLE'') AND VALUE=''1'')>0) AND UPPER(T1.B_ENTITY_TYPE) NOT IN (SELECT UPPER(TP_LAYER_NAME) FROM VW_TERMINATION_POINT_MASTER WHERE LAYER_ID='||V_LAYER_ID||')';
	EXECUTE SQL;
			
	FOR REC IN SELECT LD.LAYER_ID,LD.LAYER_NAME,LD.GEOM_TYPE,LD.LAYER_TABLE FROM VW_TERMINATION_POINT_MASTER TP 
	LEFT JOIN LAYER_DETAILS LD ON TP.TP_LAYER_ID=LD.LAYER_ID WHERE TP.LAYER_ID=V_LAYER_ID AND TP.IS_OSP_TP=TRUE AND TP.IS_ENABLED=TRUE
	LOOP			
		--UPDATING START POINTS OF LINE TYPE FROM TERMINATION POINTS 
		IF UPPER(REC.GEOM_TYPE)='POINT' 
		THEN		
			SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET A_SYSTEM_ID=P.SYSTEM_ID,A_ENTITY_TYPE='''||REC.LAYER_NAME||''',A_END_LAT=P.LATITUDE,
			A_END_LNG=P.LONGITUDE ,REGION_ID=P.REGION_ID,PROVINCE_ID=P.PROVINCE_ID FROM '||REC.LAYER_TABLE||' P 
			WHERE UPPER(T.A_NETWORK_ID)=UPPER(P.NETWORK_ID) AND T.UPLOAD_ID='||P_UPLOAD_ID||' AND T.IS_VALID=TRUE AND UPPER(T.A_ENTITY_TYPE)=UPPER('''||REC.LAYER_NAME||''')';
			EXECUTE SQL;
						
			--UPDATING THOSE RECORDS WHICH HAVING NO START POINT MATCHING
			SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET IS_VALID=FALSE,ERROR_MSG=''START TERMINATION POINT GIVEN BY YOU DOES NOT EXISTS IN APPLICATION!'' 
			WHERE CASE WHEN T.A_SYSTEM_ID=0 AND (SELECT COUNT(1) FROM GLOBAL_SETTINGS WHERE UPPER(KEY)=UPPER(''ISTERMINATIONPOINTENABLE'') AND VALUE=''1'')>0
			THEN 1=1 ELSE 1=2 END
			AND T.UPLOAD_ID='||P_UPLOAD_ID||' AND T.IS_VALID=TRUE AND UPPER(T.A_ENTITY_TYPE)=UPPER('''||REC.LAYER_NAME||''')';
			EXECUTE SQL;
			
			--UPDATING END POINTS OF LINE TYPE FROM TERMINATION POINTS 
			SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET B_SYSTEM_ID=P.SYSTEM_ID,B_ENTITY_TYPE='''||REC.LAYER_NAME||''',REGION_ID=P.REGION_ID,PROVINCE_ID=P.PROVINCE_ID,
			B_END_LAT=P.LATITUDE,B_END_LNG=P.LONGITUDE FROM '||REC.LAYER_TABLE||' P 
			WHERE UPPER(T.B_NETWORK_ID)=UPPER(P.NETWORK_ID) AND T.UPLOAD_ID='||P_UPLOAD_ID||' AND T.IS_VALID=TRUE AND UPPER(T.B_ENTITY_TYPE)=UPPER('''||REC.LAYER_NAME||''')';
			EXECUTE SQL;

			SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET IS_VALID=FALSE,ERROR_MSG=''END TERMINATION POINT GIVEN BY YOU DOES NOT EXISTS IN APPLICATION!'' 
			WHERE CASE WHEN T.B_SYSTEM_ID=0 AND (SELECT COUNT(1) FROM GLOBAL_SETTINGS WHERE UPPER(KEY)=UPPER(''ISTERMINATIONPOINTENABLE'') AND VALUE=''1'')>0
			THEN 1=1 ELSE 1=2 END
			AND T.UPLOAD_ID='||P_UPLOAD_ID||' AND T.IS_VALID=TRUE AND UPPER(T.B_ENTITY_TYPE)=UPPER('''||REC.LAYER_NAME||''')';
			EXECUTE SQL;											
		END IF;		
	END LOOP;

	SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET IS_VALID=FALSE,ERROR_MSG=''BOTH TERMINATION POINT GIVEN BY YOU DOES NOT EXISTS IN APPLICATION!'' 
	WHERE CASE WHEN (SELECT COUNT(1) FROM GLOBAL_SETTINGS WHERE UPPER(KEY)=UPPER(''ISTERMINATIONPOINTENABLE'') AND VALUE=''1'')>0 
	THEN T.A_SYSTEM_ID=0 AND T.B_SYSTEM_ID=0 ELSE 1=2 END AND T.IS_VALID=TRUE AND T.UPLOAD_ID='||P_UPLOAD_ID||'';
	EXECUTE SQL;

	IF(UPPER(P_ENTITY_TYPE)='CABLE')
	THEN
		
		

		SQL:='SELECT ST_GEOMFROMTEXT(T.SP_GEOMETRY,4326) FROM '||V_DATA_UPLOAD_TABLE||' T WHERE T.UPLOAD_ID='||P_UPLOAD_ID||'';
		EXECUTE SQL into V_GEOM_CABLE ;
		
		IF EXISTS(SELECT 1 FROM LINE_MASTER WHERE ST_Equals(V_GEOM_CABLE,sp_geometry)=true)
		THEN
			SQL:='UPDATE '||V_DATA_UPLOAD_TABLE||' T SET IS_VALID=FALSE,ERROR_MSG=(select value from res_resources where key=''SI_OSP_CAB_NET_FRM_076'')
			WHERE T.UPLOAD_ID='||P_UPLOAD_ID||'';
			EXECUTE SQL;
		END IF;
		
		
	END IF;	
end;
$BODY$;
