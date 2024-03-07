-- FUNCTION: public.fn_uploader_insert_pole(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_pole(integer, integer);

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
FOR REC IN select * from temp_du_pole where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
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
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,rec.pole_no,
rec.pole_height,rec.specification,
rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now()
,coalesce(rec.network_status,'P'),rec.pole_type,
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,'Own', 'DU', p_uploadid,
rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;


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

ALTER FUNCTION public.fn_uploader_insert_pole(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_adb(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_adb(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_adb(
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
v_shaft_system_id integer;
v_str_system_id integer;
v_flr_system_id integer;
v_unit_id integer;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='ADB';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_adb where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN
v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;
v_network_name:=rec.adb_name;
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
from vw_layer_mapping where upper(child_layer_name)=upper('ADB') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('ADB', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
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
--IF NETWORK NAME SEND BLANK
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;

END IF;
--INSERT DATA INTO MAIN TABLE
insert into att_details_adb(network_id, adb_name, latitude, longitude, province_id, region_id, pincode, address,
specification, category,subcategory1, subcategory2, subcategory3, item_code, vendor_id, status, created_by,
created_on, network_status, parent_system_id,parent_network_id, parent_entity_type, sequence_id, entity_category,
type,brand,model,construction,activation,accessibility,no_of_input_port,no_of_output_port,no_of_port,
ownership_type, audit_item_master_id, source_ref_type, source_ref_id, remarks,origin_from,
origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.pin_code,
rec.address, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A', rec.created_by,
now(),coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id, rec.entity_category,0,0,0,0,
0,0,rec.no_of_input_port,rec.no_of_output_port,rec.no_of_port,'Own', rec.audit_item_master_id, 'DU', p_uploadid,
rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT DATA INTO POINT TABLE
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,
approver_id, common_name, db_flag,no_of_ports, network_status,entity_category,display_name,st_x,st_y
)
select v_current_system_id,'ADB',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id,v_geom_port, coalesce(rec.network_status,'P'),rec.entity_category,
fn_get_display_name(v_current_system_id,'ADB'),rec.st_x,rec.st_y;

if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'ADB', v_current_system_id, v_network_code,rec.created_by);
end if;

-- INSERT isp_entity_mapping..

IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;

insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'ADB', v_current_system_id, coalesce(v_unit_id,0));
End if;
END;
-- UPDATE STATUS INTO TEMP TABLE..
--update temp_du_adb set is_processed=true where system_id=rec.system_id;

update temp_du_adb set is_processed=true, system_network_id= v_network_code where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'ADB'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_adb(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_antenna(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_antenna(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_antenna(
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
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='TOWER';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_antenna where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
LOOP
BEGIN
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.network_name;

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Antenna') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Antenna', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

--IF NETWORK NAME SEND BLANK
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT DATA IN MAIN TABLE
insert into att_details_antenna(network_id,network_name,latitude,longitude,province_id,
region_id, specification,category,subcategory1,subcategory2,subcategory3,
item_code,vendor_id,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,ownership_type,antenna_type, source_ref_type,
source_ref_id,remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,
request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y
)

select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id, rec.specification,
rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,
now(),coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,'Own',rec.antenna_type, 'DU',
p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,
rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

--INSERT DATA IN POINT TABLE
insert into point_master (system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y
)
select v_current_system_id,'Antenna',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Antenna'),rec.st_x,rec.st_y;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_antenna set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Antenna'::character varying ,rec.province_id::integer,0,false));

END;
END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_antenna(integer, integer)
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



-- FUNCTION: public.fn_uploader_insert_building(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_building(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_building(
	p_uploadid integer,
	p_batchid integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

declare
REC RECORD;
v_building_system_id integer;
v_building_parent_system_id integer;
v_building_parent_network_id character varying;
v_building_parent_entity_type character varying;
v_building_network_code character varying;
v_building_sequence_id integer;
v_latitude double precision;
v_longitude double precision;
v_network_id_type character varying;
v_building_network_code_seperator character varying;
v_str_system_id integer;
p_floor_id integer;
v_str_parent_system_id integer;
v_str_parent_network_id character varying;
v_str_parent_entity_type character varying; 
v_str_network_code character varying;
v_str_sequence_id integer;
v_default_structure_allowed integer;
v_DefaultShaftWidth double precision;
v_DefaultFloorHeight double precision;
v_DefaultFloorWidth double precision;
v_DefaultFloorLength double precision;
v_DefaultShaftLength double precision;
 v_sp_geom character varying;
v_auto_character_count integer;
v_building_network_name character varying;

BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_building_network_code_seperator from layer_details where upper(layer_name)='BUILDING';

select value::integer into v_default_structure_allowed from global_settings where key='IsBuildingBulkuploadDefaultstuctrueAllowed';

select value::double precision into v_DefaultShaftWidth  from global_settings where key='DefaultShaftWidth';
select value::double precision into v_DefaultShaftLength  from global_settings where key='DefaultShaftLength';

select value::double precision into v_DefaultFloorHeight  from global_settings where key='DefaultFloorHeight';
select value::double precision into v_DefaultFloorWidth  from global_settings where key='DefaultFloorWidth';
select value::double precision into v_DefaultFloorLength  from global_settings where key='DefaultFloorLength';

 
-- INSERT BATCH WISE RECORDS..
FOR REC IN  select * from temp_du_building where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
	LOOP
		BEGIN
		v_latitude:=rec.latitude::double precision;
		v_longitude:=rec.longitude::double precision;
		v_building_sequence_id:=0;
		v_building_network_name:=rec.building_name;

		raise info'v_latitude%',v_latitude;
		raise info'v_latitude%',v_latitude;

IF(coalesce(rec.sp_geometry,'')!='')THEN
v_sp_geom=ST_GeomFromText('POLYGON(('||rec.sp_geometry||'))',4326);
v_latitude:= ST_Y(ST_Centroid(v_sp_geom::geometry));
v_longitude:= ST_X(ST_Centroid(v_sp_geom::geometry));
raise info'v_latitude%',v_latitude;
raise info'v_longitude%',v_longitude;
ELSE
v_sp_geom=null;
END IF;
		 -- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M'and coalesce(REC.network_id,'')!='') 
		then
			v_building_network_code=rec.parent_network_id||v_building_network_code_seperator||rec.network_id;	
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then
			v_building_network_code=rec.parent_network_id||v_building_network_code_seperator||rec.network_id;
			select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
			from vw_layer_mapping where upper(child_layer_name)=upper('Building') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
			v_building_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
			-- GET NETWORK CODE & PARENT DETAILS..
			select parent_system_id,parent_network_id,parent_entity_type,network_code,sequence_id 
		from fn_get_network_code('Building','Point',0, '',''||v_longitude||' '||v_latitude||'') into
		v_building_parent_system_id,v_building_parent_network_id,v_building_parent_entity_type,v_building_network_code,v_building_sequence_id;
		end if;	
		 
		--INSERT DATA IN MAIN TABLE
		--IF(coalesce(rec.sp_geometry,'')!='')THEN   /*-IF Building Geometry is Polygon -*/
		-- validate geom
		--ALTER TABLE att_details_building DISABLE TRIGGER trg_update_building_geometry ; 

		--ELSE
		--	ALTER TABLE att_details_building ENABLE TRIGGER trg_update_building_geometry ;
		--END IF;
		
		IF(coalesce(v_building_network_name,'')='')
  		then
			v_building_network_name=v_building_network_code;
  		END IF;

		insert into att_details_building(network_id,building_name,building_no,latitude,longitude,area,address,province_id,
		region_id,business_pass,home_pass,total_tower,no_of_floor,no_of_flat,building_status,network_status,status,db_flag,pod_name,
		pod_code,rfs_status,rfs_date,tenancy,category,is_mobile,created_by,created_on,parent_system_id,parent_network_id,parent_entity_type,
		status_updated_by,status_updated_on,sequence_id,surveyarea_id,no_of_occupants,building_height,temp_geometry, source_ref_type, 		     source_ref_id,remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,
		requested_by,request_approved_by,st_x,st_y)  
		select v_building_network_code,v_building_network_name,rec.building_no,v_latitude,v_longitude,rec.area,rec.address,rec.province_id,
		rec.region_id,rec.business_pass,rec.home_pass,rec.total_tower,rec.no_of_floor,rec.no_of_flat,'Approved',
		rec.network_status,'A',rec.upload_id,rec.pod_name,rec.pod_code,rec.rfs_status,rec.rfs_date,rec.tenancy,rec.category,
		false,rec.created_by,now(),
		rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,rec.created_by,now(),v_building_sequence_id,
		rec.surveyarea_id,rec.no_of_occupants,rec.building_height,v_sp_geom::geometry, 'DU', p_uploadid,rec.remarks,rec.origin_from,
		rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,
		rec.request_approved_by,rec.st_x,rec.st_y returning system_id into v_building_system_id;

		
 
	--CREATE DEFAULT STRUCTURE---
IF(v_default_structure_allowed=1 and rec.plan_type is null)THEN
	-- GET Structrue NETWORK CODE & PARENT DETAILS..
		 select parent_system_id,parent_network_id,parent_entity_type,network_code,sequence_id 
 		from fn_get_network_code('Structure','Point',0, '',''||v_longitude||' '||v_latitude||'') into
 		v_str_parent_system_id,v_str_parent_network_id,v_str_parent_entity_type,v_str_network_code,v_str_sequence_id;
		
		 INSERT INTO att_details_bld_structure(network_id,structure_name,business_pass,home_pass,no_of_floor,no_of_flat,no_of_occupants,
		no_of_shaft,building_id,province_id,region_id,network_status,status,created_by,created_on,parent_system_id,
		parent_network_id,parent_entity_type,
		sequence_id,latitude,longitude, remarks)
		VALUES(v_str_network_code,'Tower 1',rec.business_pass,rec.home_pass,1,1,1,2,v_building_system_id,rec.province_id,rec.region_id,'P','A',rec.created_by,now()
		,v_building_system_id,v_building_network_code,'Building',v_str_sequence_id,v_latitude,v_longitude,rec.remarks) RETURNING system_id INTO v_str_system_id;

		 Raise INFO'v_str_system_id::%',v_str_system_id;

		-- --CREATE DEFAULT SHAFT---
		INSERT INTO isp_shaft_info(structure_id,shaft_name,status,remarks,created_by,created_on,length,width,with_riser,shaft_position,network_status,is_virtual)
		VALUES(v_str_system_id,'Shaft_1','A','',rec.created_by,now(),v_DefaultShaftLength,v_DefaultShaftWidth,false,'left','P',true),
		(v_str_system_id,'Shaft_2','A','',rec.created_by,now(),v_DefaultShaftLength,v_DefaultShaftWidth,false,'right','P',true);

		---CREATE DEFAULT FLOOR----
		INSERT INTO isp_floor_info(structure_id,floor_name,status,remarks,created_by,created_on,height,length,width,network_status,no_of_units,
		parent_entity_type,parent_network_id,parent_system_id,latitude,longitude,sequence_id)
		VALUES(v_str_system_id,'Floor_1','A','',rec.created_by,now(),v_DefaultFloorHeight,v_DefaultFloorLength,v_DefaultFloorWidth,'P',1,'Structure',
		v_str_network_code,v_str_system_id,v_latitude,v_longitude,1) RETURNING system_id INTO p_floor_id;

 
		---INSERT STRUCTURE INTO POINT_MASTER---
		  insert into point_master (system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
 		approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
  		select v_str_system_id,'Structure',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
  		now(),'NA','NA',rec.created_by,0,v_str_network_code,rec.upload_id,'P',fn_get_display_name(v_str_system_id,'Structure'),rec.st_x,rec.st_y;
		raise info'v_current_system_id%',v_building_system_id;
	
		 update att_details_building set total_tower=1 where system_id=v_building_system_id;
		  
END IF;	 
		
		-- UPDATE STATUS INTO TEMP TABLE..
		update temp_du_building set is_processed=true where system_id=rec.system_id;
		--update temp_du_adb set is_processed=true, system_network_id= v_network_code where system_id=rec.system_id;
		perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Building'::character varying ,rec.province_id::integer,0,false));

		END;	
         END LOOP;    
	return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_building(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_cabinet(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_cabinet(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_cabinet(
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
v_network_code_seperator character varying;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)=upper('Cabinet');

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_cabinet where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN

v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.cabinet_name;
-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;

elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Cabinet') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Cabinet', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

RAISE INFO 'v_network_code:% ',v_network_code;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_cabinet(network_id, cabinet_name, latitude, longitude, province_id, region_id, address,
pincode, specification, category,
subcategory1, subcategory2, subcategory3, item_code, vendor_id, status, created_by, created_on, network_status, parent_system_id,
parent_network_id, parent_entity_type, sequence_id,type,brand,model,construction,activation,accessibility,
ownership_type, source_ref_type, source_ref_id,remarks,origin_from,origin_Ref_id,
origin_Ref_code,origin_Ref_description,request_ref_id,
requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.address,
rec.pin_code, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A', rec.created_by,
now(), coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,0,0,0,'Own', 'DU', p_uploadid,
rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag, sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id, common_name, db_flag, network_status,display_name,st_x,st_y)
select v_current_system_id,'Cabinet',v_longitude,v_latitude,'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Cabinet'),rec.st_x,rec.st_y;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_cabinet set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Cabinet'::character varying ,rec.province_id::integer,0,false));

END;

END LOOP;
return 1;
END;

$BODY$;

ALTER FUNCTION public.fn_uploader_insert_cabinet(integer, integer)
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
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,structure_id)

SELECT v_network_code,v_network_name, v_a_network_id, v_b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),v_a_system_id,
v_a_network_id,
v_a_entity_type,v_b_system_id,v_b_network_id,v_b_entity_type,cable_category,sub_category,
route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',v_structure_id
--case when v_structure_id is null or v_structure_id ='' then 
--0 --else v_structure_id end
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
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category)
SELECT v_network_code,v_network_name, a_network_id, b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),a_system_id,a_network_id,
a_entity_type,b_system_id,b_network_id,b_entity_type,cable_category,sub_category,
route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed'
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
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Cable'::character varying ,rec.province_id::integer,0,false));

insert into associate_entity_info(entity_system_id,entity_type,entity_network_id,entity_display_name,associated_system_id,associated_entity_type,associated_network_id,associated_display_name,created_by,created_on,is_termination_point)
values(current_system_id,'Cable',v_network_code,fn_get_display_name(current_system_id, 'Cable'),REC.a_system_id,REC.a_entity_type,REC.a_network_id,v_a_display_value,rec.created_by,now(),true),
(current_system_id,'Cable',v_network_code,fn_get_display_name(current_system_id, 'Cable'),REC.b_system_id,REC.b_entity_type,REC.b_network_id,v_b_display_value,rec.created_by,now(),true);

END;
END LOOP;
return 1;

END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_cable(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_cdb(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_cdb(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_cdb(
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
v_shaft_system_id integer;
v_flr_system_id integer;
v_unit_id integer;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='CDB';

v_str_system_id:=0;
v_unit_id:=0;

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_cdb where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN

v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;
v_network_name:=rec.cdb_name;
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
from vw_layer_mapping where upper(child_layer_name)=upper('CDB') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..

select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('CDB', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
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
--INSERT DATA INTO MAIN TABLE
insert into att_details_cdb(network_id, cdb_name, latitude, longitude, province_id, region_id, pincode, address, specification, category,
subcategory1, subcategory2, subcategory3, item_code, vendor_id, status, created_by, created_on, network_status, parent_system_id,
parent_network_id, parent_entity_type, sequence_id, entity_category,type,brand,model,construction,activation,
accessibility,no_of_input_port,no_of_output_port,no_of_port,ownership_type, audit_item_master_id, source_ref_type,
source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,
requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.pin_code,
rec.address, rec.specification,rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code,
rec.vendor_id, 'A', rec.created_by, now(), coalesce(rec.network_status,'P'),rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,
v_sequence_id, rec.entity_category ,0,0,0,0,0,0,rec.no_of_input_port,rec.no_of_output_port,rec.no_of_port,
'Own', rec.audit_item_master_id, 'DU', p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT DATA INTO POINT TABLE
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,
approver_id, common_name, db_flag,no_of_ports, network_status,entity_category,display_name,st_x,st_y)
select v_current_system_id,'CDB',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id,v_geom_port, coalesce(rec.network_status,'P'), rec.entity_category ,fn_get_display_name(v_current_system_id,'CDB'),rec.st_x,rec.st_y;

if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'CDB', v_current_system_id, v_network_code,rec.created_by);
end if;

-- INSERT isp_entity_mapping..

IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;

insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'CDB', v_current_system_id, coalesce(v_unit_id,0));
End if;
END;

-- UPDATE STATUS INTO TEMP TABLE..
--update temp_du_cdb set is_processed=true where system_id=rec.system_id;
update temp_du_cdb set is_processed=true, system_network_id= v_network_code where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'CDB'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_cdb(integer, integer)
    OWNER TO postgres;


-- FUNCTION: public.fn_uploader_insert_coupler(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_coupler(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_coupler(
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
v_inner_dimention double precision;
v_outer_dimention double precision;
v_network_id_type character varying;
v_network_code_seperator character varying;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='COUPLER';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_coupler where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
LOOP
BEGIN
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.coupler_name;

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Coupler') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Coupler', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT DATA IN MAIN TABLE
insert into att_details_coupler(network_id, coupler_name, latitude, longitude, province_id, region_id, address,
specification, category,
subcategory1, subcategory2, subcategory3, item_code, vendor_id, status, created_by, created_on, network_status, parent_system_id,
parent_network_id, parent_entity_type, sequence_id, coupler_type,inner_dimention,outer_dimention,type,
brand,model,construction,activation,accessibility,ownership_type, source_ref_type, source_ref_id, remarks,
origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y
)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.address, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A', rec.created_by,
now(), coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id, rec.coupler_type,
rec.inner_dimention::double precision,rec.outer_dimention::double precision,0,0,0,0,0,0,'Own', 'DU', p_uploadid,
rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y
)
select v_current_system_id,'Coupler',v_longitude,v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by, 0, v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Coupler'),rec.st_x,rec.st_y;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_coupler set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Coupler'::character varying ,rec.province_id::integer,0,false));

END;
END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_coupler(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_customer(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_customer(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_customer(
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
v_str_system_id integer; 
v_unit_id integer;
v_flr_system_id integer;
v_unit_system_id integer;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='CUSTOMER';

-- INSERT BATCH WISE RECORDS..
FOR REC IN  select * from temp_du_customer where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
	LOOP
		BEGIN
		v_str_system_id:=0;
		v_unit_id:=0;		
		v_flr_system_id:=0;
		
		v_latitude:=rec.latitude::double precision;
		v_longitude:=rec.longitude::double precision;
		v_sequence_id:=0;
		v_network_name:=rec.customer_name;
		v_flr_system_id:= rec.floor_id;
		-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M' and coalesce(REC.network_id,'')!='') 
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;	
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
			select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
			from vw_layer_mapping where upper(child_layer_name)=upper('CUSTOMER') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
			v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
			-- GET NETWORK CODE & PARENT DETAILS..
			select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id 
			from fn_get_clone_network_code('CUSTOMER', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type)  into
			v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
		end if;

		IF(upper(rec.parent_entity_type)='STRUCTURE')THEN
		  	select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure 
		  	where network_id=rec.parent_network_id;
		 ElsIF(upper(rec.parent_entity_type)='UNIT')THEN
		  	select  str.system_id, str.longitude, str.latitude, Isp.id, Isp.floor_id into v_str_system_id, v_longitude, v_latitude,v_unit_id,
		  	v_flr_system_id  from att_details_bld_structure str inner join (select id,floor_id, structure_id  from isp_entity_mapping 
		  	where entity_id=rec.parent_system_id and upper(entity_type)=upper(rec.parent_entity_type))as  Isp on str.system_id = isp.structure_id; 
		END IF;

		
		IF(coalesce(v_network_name,'')='')
  		then
			v_network_name=v_network_code;
  		END IF;
		--INSERT INTO MAIN TABLE
		insert into att_details_customer(network_id,customer_name,province_id,region_id,latitude,longitude,status,created_by,created_on,parent_system_id,
		parent_network_id,parent_entity_type,customer_status_id,network_status,agl,is_power_back_up_available,is_visible_on_map,is_new_entity,mobile_no,
		origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,remarks,sequence_id,address,st_x,st_y
)  

		select rec.network_id,rec.customer_name,rec.province_id,rec.region_id,v_latitude,v_longitude,'A',rec.created_by,now(),
		rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,1,'A',0,false,true,false,rec.mobile_no,
		rec.origin_ref_id,rec.origin_ref_code,rec.origin_ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,rec.remarks,0 ,rec.address,rec.st_x,rec.st_y
		returning system_id into v_current_system_id;

		

		--INSERT INTO POINT MASTER
		insert into point_master (system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
		approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y
)
		select v_current_system_id,'Customer',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
		now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,'P',fn_get_display_name(v_current_system_id,'Customer'),rec.st_x,rec.st_y;

		IF(v_str_system_id>0)THEN 
			raise info'v_str_system_id::%',v_str_system_id;			
			raise info'v_flr_system_id::%',v_flr_system_id;
			raise info'v_str_system_id::%',v_str_system_id; 
			raise info'v_unit_id::%',v_unit_id; 
				IF(rec.unit_system_id > 0) THEN 
			select id,floor_id into v_unit_id,v_flr_system_id from isp_entity_mapping where entity_id=rec.unit_system_id and 
			structure_id = v_str_system_id  and  upper(entity_type)= upper('UNIT'); 
			End IF; 
		 insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
		(v_str_system_id, 0, v_flr_system_id, 'Customer', v_current_system_id, coalesce(v_unit_id,0));
		End if;
		END;
		-- UPDATE STATUS INTO TEMP TABLE..
		update temp_du_customer set is_processed=true where system_id=rec.system_id;
		perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Customer'::character varying ,rec.province_id::integer,0,false));

         END LOOP;    
	return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_customer(integer, integer)
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
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Duct'::character varying ,rec.province_id::integer,0,false));

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


-- FUNCTION: public.fn_uploader_insert_fms(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_fms(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_fms(
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
v_is_virtual_port_allowed boolean;
v_network_code_seperator character varying;
v_input_port integer;
v_output_port integer;
v_geom_port character varying;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='FMS';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_fms where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN
if rec.no_of_input_port>0 and rec.no_of_output_port>0 then
v_input_port:=rec.no_of_input_port;
v_output_port:=rec.no_of_output_port;
v_geom_port:=rec.no_of_input_port::character varying||':'||rec.no_of_output_port::character varying;
else
v_input_port:=rec.no_of_port;
v_output_port:=rec.no_of_port;
v_geom_port:=rec.no_of_port::character varying;
end if;
v_network_name:=rec.fms_name;
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('FMS') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('FMS', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;

end if;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_fms(network_id, fms_name, latitude, longitude, province_id, region_id, address, pincode,
specification, category,subcategory1,subcategory2,subcategory3, item_code, vendor_id, status, created_by,
created_on, network_status,parent_system_id,parent_network_id, parent_entity_type,sequence_id,type,brand,
model,construction,activation,accessibility,no_of_input_port,no_of_output_port,no_of_port,ownership_type,
source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,
request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y
)
select v_network_code,v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.address,
rec.pin_code, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A', rec.created_by,
now(), coalesce(rec.network_status,'P'),rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,
0,0,0,v_input_port,v_output_port,v_input_port,'Own', 'DU', p_uploadid, rec.remarks, rec.origin_from,
rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id, entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark, created_by,approver_id, common_name, db_flag,no_of_ports, network_status,display_name,st_x,st_y
)
select v_current_system_id, 'FMS', v_longitude, v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
now(), 'NA', 'NA', rec.created_by,0,v_network_code, rec.upload_id,v_geom_port::character varying, coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'FMS'),rec.st_x,rec.st_y;

-- INSERT PORT DETAILS..
if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'FMS', v_current_system_id, v_network_code,rec.created_by);
end if;
END;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_fms set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'FMS'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_fms(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_handhole(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_handhole(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_handhole(
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
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='HANDHOLE';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_handhole where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
LOOP
BEGIN
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.handhole_name;
-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('HANDHOLE') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Handhole', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE

--select * from att_details_handhole order by system_id desc limit 10
insert into att_details_handhole(network_id,handhole_name,latitude,longitude,province_id,
region_id, address,specification,category,subcategory1,subcategory2,subcategory3,
item_code,vendor_id,status,created_by,created_on,network_status,construction_type,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,type,brand,model,construction,activation,accessibility,is_virtual,ownership_type, source_ref_type, source_ref_id, remarks,bom_sub_category,st_x,st_y)
select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,rec.specification,
rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),coalesce(rec.network_status,'P'),
rec.construction_type,rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,rec.is_virtual,'Own', 'DU', p_uploadid, rec.remarks,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master (system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
select v_current_system_id,'Handhole',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Handhole'),rec.st_x,rec.st_y;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_handhole set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Handhole'::character varying ,rec.province_id::integer,0,false));

END;
END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_handhole(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_htb(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_htb(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_htb(
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
v_is_virtual_port_allowed boolean;
v_network_code_seperator character varying;
v_input_port integer;
v_output_port integer;
v_str_system_id integer;
v_unit_id integer;
v_flr_system_id integer;
v_shaft_system_id integer;
v_unit_system_id integer;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='HTB';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_htb where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN

v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;
v_network_name:=rec.htb_name;
if rec.no_of_input_port>0 and rec.no_of_output_port>0 then
v_input_port:=rec.no_of_input_port;
v_output_port:=rec.no_of_output_port;
else
v_input_port:=rec.no_of_port;
v_output_port:=rec.no_of_port;
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
from vw_layer_mapping where upper(child_layer_name)=upper('HTB') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('HTB', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(upper(rec.parent_entity_type)='STRUCTURE')THEN
select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
where network_id=rec.parent_network_id;
ElsIF(upper(rec.parent_entity_type)='UNIT')THEN
select str.system_id, str.longitude, str.latitude, Isp.id, Isp.floor_id into v_str_system_id, v_longitude, v_latitude, v_unit_id,v_flr_system_id from att_details_bld_structure str inner join (select id,floor_id, structure_id from isp_entity_mapping where entity_id=rec.parent_system_id and upper(entity_type)=upper(rec.parent_entity_type))as Isp on str.system_id = isp.structure_id;
END IF;

raise info'v_parent_system_id::%',v_parent_system_id;
raise info'v_parent_network_id::%',v_parent_network_id;
raise info'v_parent_entity_type::%',v_parent_entity_type;

IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;

--INSERT INTO MAIN TABLE
insert into isp_htb_info(network_id,htb_name,latitude,longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,network_status,created_by,created_on,
project_id,planning_id,purpose_id,workorder_id,type,brand,model,construction,activation,accessibility,structure_id,
shaft_id,floor_id,no_of_input_port,no_of_output_port, parent_system_id,parent_entity_type,parent_network_id,sequence_id,utilization,
audit_item_master_id, ownership_type, status, source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,
origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code,v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id,rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id,
coalesce(rec.network_status,'P'), rec.created_by, now(),0,0,0,0,0,0,0,0,0,0,v_str_system_id,0,0,v_input_port,v_output_port,
rec.parent_system_id,REC.PARENT_ENTITY_TYPE,rec.parent_network_id,v_sequence_id,'L',rec.audit_item_master_id, 'Own', 'A', 'DU',
p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,network_status,no_of_ports, display_name,st_x,st_y)
select v_current_system_id,'HTB',v_longitude,v_latitude,'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0,v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),v_input_port, fn_get_display_name(v_current_system_id,'HTB'),rec.st_x,rec.st_y;

-- INSERT PORT DETAILS..
if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'HTB', v_current_system_id, v_network_code,rec.created_by);
end if;

-- INSERT isp_entity_mapping..

IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;
IF(rec.unit_system_id > 0) THEN
select id,floor_id,shaft_id into v_unit_id,v_flr_system_id,v_shaft_system_id from isp_entity_mapping where entity_id=rec.unit_system_id and structure_id = v_str_system_id and upper(entity_type)= upper('UNIT');
End IF;
insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'HTB', v_current_system_id, coalesce(v_unit_id,0));
End if;
END;
-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_htb set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'HTB'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_htb(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_loop(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_loop(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_loop(
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
v_geometry geometry;
v_snapped_geom geometry;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='LOOP';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_loop where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
LOOP
BEGIN
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,'n',''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('LOOP') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE)
and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Loop', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

select ST_ClosestPoint(sp_geometry,ST_GeomFromText('POINT('||v_longitude||' '||v_latitude||')',4326)) into v_geometry from
line_master where system_id=rec.associated_system_id and upper(entity_type)=upper('CABLE') ;

v_longitude=st_x(v_geometry);
v_latitude=st_y(v_geometry);

--INSERT INTO MAIN TABLE
INSERT INTO att_details_loop(network_id,loop_length,associated_system_id,associated_network_id,associated_entity_type,
cable_system_id,network_status,start_reading,end_reading,parent_system_id,sequence_id,province_id,region_id,parent_network_id,parent_entity_type,
source_ref_type,source_ref_id,created_by,latitude,longitude,origin_from,origin_ref_id,
origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,st_x,st_y)

values( v_network_code,rec.loop_length,rec.associated_system_id,rec.associated_network_id,rec.associated_entity_type,
rec.cable_system_id,coalesce(rec.network_status,'P'),rec.start_reading,rec.end_reading,rec.parent_system_id,v_sequence_id,rec.province_id,rec.region_id,
rec.parent_network_id,rec.parent_entity_type,'DU',p_uploadid,rec.created_by,v_latitude,v_longitude,
rec.origin_from,rec.origin_ref_id,rec.origin_ref_code,rec.origin_ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,rec.st_x,rec.st_y )returning system_id into v_current_system_id;

--PointMaster entry--
INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
created_by,common_name, db_flag,network_status,display_name,st_x,st_y)
VALUES (v_current_system_id,'Loop', rec.longitude::double precision,rec.latitude::double precision,'A',
ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),now(),
'NA','NA',rec.created_by,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Loop'),rec.st_x,rec.st_y);

-- Snapping cable to loop
SELECT ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01) into v_snapped_geom
FROM (
SELECT SP_GEOMETRY AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = v_current_system_id and lower(entity_type)=lower('Loop')
) AS point,
LINE_MASTER AS LM WHERE LM.system_id = rec.cable_system_id and lower(entity_type)=lower('cable');

update LINE_MASTER set SP_GEOMETRY=v_snapped_geom WHERE system_id = rec.cable_system_id and lower(entity_type)=lower('cable');

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_loop set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Loop'::character varying ,rec.province_id::integer,0,false));

END;
END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_loop(integer, integer)
    OWNER TO postgres;



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
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y,area,authority,route_name
)
select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,rec.specification,
rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),coalesce(rec.network_status,'P'),
rec.construction_type,rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
rec.is_virtual,'Own', 'DU', p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y,rec.area,rec.authority,rec.route_name
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

ALTER FUNCTION public.fn_uploader_insert_manhole(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_mpod(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_mpod(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_mpod(
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
v_network_code_seperator character varying;
v_str_system_id integer;
v_unit_id integer;
v_unit_system_id integer;
v_flr_system_id integer;
v_shaft_system_id integer;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='MPOD';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_mpod where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN

v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;
v_network_name:=rec.mpod_name;
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
from vw_layer_mapping where upper(child_layer_name)=upper('MPOD') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('MPOD', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(upper(rec.parent_entity_type)='STRUCTURE')THEN
select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
where network_id=rec.parent_network_id;
ElsIF(upper(rec.parent_entity_type)='UNIT') THEN
select str.system_id, str.longitude, str.latitude, Isp.id, Isp.floor_id into v_str_system_id, v_longitude,
v_latitude, v_unit_id,v_flr_system_id from att_details_bld_structure str inner join (select id,floor_id,
structure_id from isp_entity_mapping where entity_id=rec.parent_system_id and upper(entity_type)=
upper(rec.parent_entity_type))as
Isp on str.system_id = isp.structure_id;
END IF;
-- --IF NETWORK ID SEND BLANK
-- IF(rec.network_id='' OR rec.network_id IS NULL )
-- then
-- raise info 'start';
-- UPDATE temp_du_mpod tp set network_id = rec.parent_network_id where rec.upload_id = p_uploadid and tp.system_id = rec.system_id;
-- select network_id into v_network_code from temp_du_mpod tp where rec.upload_id = p_uploadid and tp.system_id = rec.system_id;
-- raise info 'success';
-- END IF;
-- raise info 'st=%',v_network_code;

--IF NETWORK NAME SEND BLANK
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
--UPDATE temp_du_mpod tp set mpod_name = tp.network_id where rec.upload_id = p_uploadid and tp.system_id = rec.system_id;
--select mpod_name into v_mpod_name from temp_du_mpod tp where rec.upload_id = p_uploadid and tp.system_id = rec.system_id;

END IF;
raise info'v_parent_system_id::%',v_parent_system_id;
raise info'v_parent_network_id::%',v_parent_network_id;
raise info'v_parent_entity_type::%',v_parent_entity_type;

--INSERT INTO MAIN TABLE
insert into att_details_mpod(network_id, mpod_name, latitude, longitude, province_id, region_id, address, pincode,
specification, category,subcategory1, subcategory2, subcategory3, item_code, vendor_id, status, created_by,
created_on, network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id,type,brand,model,
construction,activation,accessibility,ownership_type, audit_item_master_id,
source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code,v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.address, rec.pin_code, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A', rec.created_by,
now(), coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,0,0,0,'Own', rec.audit_item_master_id,
'DU',p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag, sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id, common_name, db_flag, network_status,display_name,st_x,st_y)
select v_current_system_id,'MPOD',v_longitude,v_latitude,'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'MPOD'),rec.st_x,rec.st_y;

-- INSERT isp_entity_mapping..

IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;
IF(rec.unit_system_id > 0) THEN
select id,floor_id,shaft_id into v_unit_id,v_flr_system_id,v_shaft_system_id from isp_entity_mapping where entity_id=rec.unit_system_id and structure_id = v_str_system_id and upper(entity_type)= upper('UNIT');
End IF;
insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'MPOD', v_current_system_id, coalesce(v_unit_id,0));
End if;
END;
-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_mpod set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'MPOD'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_mpod(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_ont(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_ont(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_ont(
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
v_is_virtual_port_allowed boolean;
v_network_code_seperator character varying;
v_input_port integer;
v_output_port integer;
v_geom_port character varying;
v_unit_system_id integer;
v_str_system_id integer;
v_flr_system_id integer;
v_shaft_system_id integer;
v_unit_id integer;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='ONT';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_ont where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN
v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;
v_network_name:=rec.ont_name;
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
from vw_layer_mapping where upper(child_layer_name)=upper('ONT') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..

select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('ONT', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(upper(rec.parent_entity_type)='STRUCTURE')THEN
select system_id,latitude,longitude into v_str_system_id, v_latitude,v_longitude from att_details_bld_structure
where network_id=rec.parent_network_id;
ELSIF(upper(rec.parent_entity_type)='UNIT')THEN
select str.system_id, str.longitude, str.latitude, Isp.id, Isp.floor_id into v_str_system_id, v_longitude, v_latitude, v_unit_id,v_flr_system_id from att_details_bld_structure str inner join (select id,floor_id, structure_id from isp_entity_mapping where entity_id=rec.parent_system_id and upper(entity_type)=upper(rec.parent_entity_type))as Isp on str.system_id = isp.structure_id;
END IF;

raise info'v_str_system_id::%',v_str_system_id;
raise info'v_parent_system_id::%',v_parent_system_id;
raise info'v_parent_network_id::%',v_parent_network_id;
raise info'v_parent_entity_type::%',v_parent_entity_type;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_ont(network_id,ont_name,latitude,longitude,province_id,region_id,serial_no,specification,category,
subcategory1,subcategory2,subcategory3,item_code,vendor_id,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,no_of_input_port,no_of_output_port,type,brand,model,construction,
activation,accessibility,ownership_type, structure_id, audit_item_master_id, source_ref_type, source_ref_id, remarks,
origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id,
rec.serial_no, rec.specification,rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3,
rec.item_code, rec.vendor_id, 'A', rec.created_by, now(),coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,
v_input_port,v_output_port,0,0,0,0,0,0,'Own', coalesce(v_str_system_id::integer,0), rec.audit_item_master_id,
'DU', p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,network_status,no_of_ports,display_name,st_x,st_y)
select v_current_system_id,'ONT',v_longitude,v_latitude,'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0,v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),v_geom_port,fn_get_display_name(v_current_system_id,'ONT'),rec.st_x,rec.st_y;

-- INSERT PORT DETAILS..
if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'ONT', v_current_system_id, v_network_code,rec.created_by);
end if;

-- INSERT isp_entity_mapping..

IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;

IF(rec.unit_system_id > 0) THEN
select id,floor_id,shaft_id into v_unit_id,v_flr_system_id,v_shaft_system_id from isp_entity_mapping where entity_id=rec.unit_system_id and structure_id = v_str_system_id and upper(entity_type)= upper('UNIT');
End IF;

insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'ONT', v_current_system_id, coalesce(v_unit_id,0));
End IF;
END;
-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_ont set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'ONT'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_ont(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_patchpanel(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_patchpanel(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_patchpanel(
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
v_is_virtual_port_allowed boolean;
v_network_code_seperator character varying;
v_input_port integer;
v_output_port integer;
v_geom_port character varying;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='PATCHPANEL';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_patchpanel where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN
if rec.no_of_input_port>0 and rec.no_of_output_port>0 then
v_input_port:=rec.no_of_input_port;
v_output_port:=rec.no_of_output_port;
v_geom_port:=rec.no_of_input_port::character varying||':'||rec.no_of_output_port::character varying;
else
v_input_port:=rec.no_of_port;
v_output_port:=rec.no_of_port;
v_geom_port:=rec.no_of_port::character varying;
end if;
v_network_name:=rec.patchpanel_name;
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('PATCHPANEL') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('PATCHPANEL', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_patchpanel(network_id, patchpanel_name, latitude, longitude, province_id, region_id, address, pincode,
specification, category,subcategory1,subcategory2,subcategory3, item_code, vendor_id, status, created_by,
created_on, network_status,parent_system_id,parent_network_id, parent_entity_type,sequence_id,type,brand,model,
construction,activation,accessibility,
no_of_input_port,no_of_output_port,no_of_port,ownership_type, source_ref_type, source_ref_id,
remarks,patchpanel_type,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,
requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.address,
rec.pin_code, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A', rec.created_by,
now(),coalesce(rec.network_status,'P'),rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,
0,0,0,v_input_port,v_output_port,v_input_port,'Own', 'DU', p_uploadid, rec.remarks, rec.patchpanel_type,rec.origin_from,
rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id, entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark, created_by,approver_id, common_name, db_flag,no_of_ports, network_status,display_name,st_x,st_y)
select v_current_system_id, 'PATCHPANEL', v_longitude, v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
now(), 'NA', 'NA', rec.created_by,0,v_network_code, rec.upload_id,v_geom_port::character varying, coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'PATCHPANEL'),rec.st_x,rec.st_y;

-- INSERT PORT DETAILS..
if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(v_input_port,v_output_port,'PATCHPANEL', v_current_system_id, v_network_code,rec.created_by);
end if;
END;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_patchpanel set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'PATCHPANEL'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_patchpanel(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_pod(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_pod(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_pod(
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
v_network_code_seperator character varying;
v_display_name character varying;
v_display_value character varying;
v_str_system_id integer;
v_flr_system_id integer;
v_shaft_system_id integer;
v_unit_id integer;
v_unit_system_id integer;
v_auto_character_count integer;
v_network_name character varying;

-- Added by Diksha
v_specification character varying;
v_category_reference character varying; 
v_subcategory_1 character varying; 
v_subcategory_2  character varying; 
v_subcategory_3 character varying;  
v_vendor_id integer;
v_item_code character varying;
v_another_current_system_id integer;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='POD';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_pod where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN

v_str_system_id:=0;
v_unit_id:=0;
v_shaft_system_id:=0;
v_flr_system_id:=0;
v_sequence_id:=0;
v_network_name:=rec.pod_name;
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
from vw_layer_mapping where upper(child_layer_name)=upper('POD') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('POD', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(upper(rec.parent_entity_type)='UNIT') THEN
select str.system_id, str.longitude, str.latitude, Isp.id, Isp.floor_id into v_str_system_id, v_longitude, v_latitude, v_unit_id,v_flr_system_id from att_details_bld_structure str inner join (select id,floor_id, structure_id from isp_entity_mapping where entity_id=rec.parent_system_id and upper(entity_type)=upper(rec.parent_entity_type))as Isp on str.system_id = isp.structure_id;
END IF;
raise info'v_parent_system_id::%',v_parent_system_id;
raise info'v_parent_network_id::%',v_parent_network_id;
raise info'v_parent_entity_type::%',v_parent_entity_type;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_pod(network_id, pod_name, latitude, longitude, province_id, region_id, address, pincode,
specification, category,subcategory1, subcategory2, subcategory3, item_code, vendor_id, status, created_by,
created_on, network_status, parent_system_id,parent_network_id, parent_entity_type, sequence_id,type,brand,model,
construction,activation,accessibility,ownership_type, structure_id, audit_item_master_id, source_ref_type, source_ref_id,
remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.address,
rec.pin_code, rec.specification,rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code,
rec.vendor_id, 'A', rec.created_by, now(), coalesce(rec.network_status,'P'),rec.parent_system_id,rec.parent_network_id,
REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,0,0,0,'Own', v_str_system_id, rec.audit_item_master_id, 'DU',
p_uploadid, rec.remarks ,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,
rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
select v_current_system_id,'POD',v_longitude,v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA',rec.created_by,0, v_network_code, rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'POD'),rec.st_x,rec.st_y;

-- INSERT isp_entity_mapping..

IF(upper(rec.parent_entity_type)='STRUCTURE' and rec.parent_system_id is not null)
THEN
select rec.parent_system_id into v_str_system_id;
END IF;
IF(v_str_system_id>0)THEN
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_shaft_system_id::%',v_shaft_system_id;
raise info'v_flr_system_id::%',v_flr_system_id;
raise info'v_str_system_id::%',v_str_system_id;
raise info'v_unit_id::%',v_unit_id;
IF(rec.unit_system_id > 0)
THEN
select id,floor_id,shaft_id into v_unit_id,v_flr_system_id,v_shaft_system_id
from isp_entity_mapping where entity_id=rec.unit_system_id
and structure_id = v_str_system_id and upper(entity_type)= upper('UNIT');
End IF;

insert into isp_entity_mapping(structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)values
(v_str_system_id, v_shaft_system_id, v_flr_system_id, 'POD', v_current_system_id, coalesce(v_unit_id,0));

End if;
END;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_pod set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'POD'::character varying ,rec.province_id::integer,0,false));

if(rec.origin_Ref_id is not null) then 
--Create ODF for POP in SP 
raise info 'POP Start' ;
select specification, category_reference , subcategory_1, subcategory_2, subcategory_3, code, vendor_id  from item_template_master itm 
where layer_id =(select layer_id  from layer_details where layer_name ='FMS' ) and specification ='Generic' into 
v_specification, v_category_reference , v_subcategory_1, v_subcategory_2, v_subcategory_3, v_item_code, v_vendor_id;

	
INSERT INTO public.att_details_fms
(network_id, fms_name, latitude, longitude, province_id, region_id, pincode, address, 
specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id, 
"type", brand, model, 
construction, activation, accessibility, status, created_by, created_on, network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id, no_of_port, project_id, planning_id, purpose_id, workorder_id, structure_id, no_of_input_port, no_of_output_port, ownership,ownership_type, audit_item_master_id, is_visible_on_map, is_new_entity, is_middleware, is_acquire_from,origin_ref_code)

select 
(select message from fn_get_clone_network_code('FMS', 'Point',''||p.longitude||' '||p.latitude||'', p.system_id, p.category) limit 1) as network_id , 
(select message from fn_get_clone_network_code('FMS', 'Point',''||p.longitude||' '||p.latitude||'', p.system_id, p.category) limit 1) as network_code, 
p.latitude, p.longitude, p.province_id , p.region_id, p.pincode, p.address, 
v_specification, v_category_reference , v_subcategory_1, v_subcategory_2, v_subcategory_3, v_item_code, v_vendor_id, 0, 0, 0, 0, 0, 0, 'A'::character varying,p.created_by, now(), 'P'::character varying, p.system_id, p.network_id, p.category, 
(select o_sequence_id from fn_get_clone_network_code('FMS', 'Point',''||p.longitude||' '||p.latitude||'', p.system_id, p.category) limit 1),
48, 0, 0, 0, 0, 0, 0, 0, 'Own','Own', 0,  true,false, true, false,p_uploadid
from att_details_pod p where p.system_id=v_current_system_id  returning system_id into v_another_current_system_id;

raise info 'system_id%',v_another_current_system_id;
--------------------------------------------------------------------------

--Create ODF for POP in SP 
raise info 'POP Start' ;
select specification, category_reference , subcategory_1, subcategory_2, subcategory_3, code, vendor_id  from item_template_master itm 
where layer_id =(select layer_id  from layer_details where layer_name ='FMS' ) and specification ='Generic' into 
v_specification, v_category_reference , v_subcategory_1, v_subcategory_2, v_subcategory_3, v_item_code, v_vendor_id;

raise info 'Insert Statement % , % , % , % , % , % , %',v_specification, v_category_reference , v_subcategory_1, v_subcategory_2, v_subcategory_3, v_item_code, v_vendor_id ;

---Insert FMS details into point_master
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,modified_on,network_status,no_of_ports,entity_category,display_name)

select system_id ,'FMS',longitude,latitude,'A',ST_GeomFromText('POINT('||longitude||' '||latitude||')',4326),'NA','NA',created_by ,0,network_id ,0,now(),network_status ,no_of_port ,category ,fn_get_display_name(system_id,'FMS') from att_details_fms adf where system_id =v_another_current_system_id;

------Inserting port data by fn_bulk_insert_port_info....

perform fn_bulk_insert_port_info(48,48,'FMS',v_another_current_system_id,(select network_id from att_details_fms where system_id=v_another_current_system_id),(select created_by from att_details_fms where system_id=v_another_current_system_id));
 end if;
END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_pod(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_row(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_row(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_row(
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
v_geom character varying;
v_network_id_type character varying;
v_network_code_seperator character varying;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='ROW';

-- INSERT BATCH WISE RECORDS..
FOR REC IN  select * from temp_du_row where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
	LOOP
		BEGIN
		v_geom:=replace(replace(upper(rec.sp_geometry),'POLYGON(',''),')','');
		v_sequence_id:=0;
		v_network_name:=rec.row_name;
		 -- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M') 
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
			select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
			from vw_layer_mapping where upper(child_layer_name)='ROW' and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
			v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
			-- GET NETWORK CODE & PARENT DETAILS..
			select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id 
			from fn_get_clone_network_code('ROW', 'Polygon',''||v_geom||'', rec.parent_system_id, rec.parent_entity_type)  into
			v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
			
		end if;		
		 IF(coalesce(v_network_name,'')='')
  		then
			v_network_name=v_network_code;
  		END IF;
		--INSERT DATA IN MAIN TABLE
		insert into att_details_row(network_id,row_name,row_stage,province_id,region_id,created_by,created_on,sequence_id,parent_system_id,parent_network_id,
		parent_entity_type,row_type,geom_type,is_visible_on_map,is_new_entity,Remarks,requested_by,origin_ref_code,request_approved_by,origin_from,origin_ref_id,
		origin_ref_description,request_ref_id,network_status)

		select v_network_code,v_network_name,rec.row_stage,rec.province_id,rec.region_id,0,now(),v_sequence_id,rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,
		rec.row_type,'Line',true,false,rec.Remarks,rec.requested_by,rec.origin_ref_code,rec.request_approved_by,rec.origin_from,rec.origin_ref_id,
		rec.origin_ref_description,rec.request_ref_id,rec.network_status returning system_id into v_current_system_id;
	

		--INSERT DATA IN POINT TABLE
		insert into polygon_master (system_id,entity_type,approval_flag,sp_geometry,approval_date,creator_remark,
		approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name)
		select v_current_system_id,'ROW',rec.network_status,ST_GEOMFROMTEXT('POLYGON(('||rec.sp_geometry||'))',4326),
		now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'ROW');
		
		-- UPDATE STATUS INTO TEMP TABLE..
		update temp_du_row set is_processed=true where system_id=rec.system_id;
		perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'ROW'::character varying ,rec.province_id::integer,0,false));

		END;	
         END LOOP;    
	return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_row(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_sector(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_sector(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_sector(
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
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='SECTOR';

-- INSERT BATCH WISE RECORDS..
FOR REC IN  select * from temp_du_sector where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
	LOOP
		BEGIN
		v_latitude:=rec.latitude::double precision;
		v_longitude:=rec.longitude::double precision;
		v_sequence_id:=0;
		v_network_name:=rec.network_name;
		 -- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M') 
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;	
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
			select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
			from vw_layer_mapping where upper(child_layer_name)=upper('Sector') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
			v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
			-- GET NETWORK CODE & PARENT DETAILS..
			select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id 
			from fn_get_clone_network_code('Sector', 'Polygon',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type)  into
			v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
			
		end if;		
		 IF(coalesce(v_network_name,'')='')
  		then
			v_network_name=v_network_code;
  		END IF;
		--INSERT DATA IN MAIN TABLE
		insert into att_details_sector(network_id,network_name,latitude,longitude,province_id,
		region_id, azimuth,technology,port_name,down_link,uplink,brand_name,sector_type,frequency, specification,category,
		subcategory1,subcategory2,subcategory3,item_code,vendor_id,status,created_by,created_on,network_status,
		parent_system_id,parent_network_id,parent_entity_type,sequence_id,ownership_type,status_updated_by,
		status_updated_on,project_id,planning_id,workorder_id,purpose_id, source_ref_type, source_ref_id, remarks,origin_from,
		origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by)  

		select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id, (case when coalesce(rec.azimuth::character varying =  '') then 0 else rec.azimuth::double precision  end),rec.technology,rec.port_name,
		rec.down_link,rec.uplink,rec.brand_name,rec.sector_type,rec.frequency, rec.specification,
		rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,
		now(),coalesce(rec.network_status,'P'),
		rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,'Own',0,now(),0,0,0,0, 'DU', 
		p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
		rec.request_ref_id,rec.requested_by,rec.request_approved_by returning system_id into v_current_system_id;

	

		--INSERT DATA IN POINT TABLE
		insert into polygon_master (system_id,entity_type,approval_flag,sp_geometry,approval_date,creator_remark,
		approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name)
		select v_current_system_id,'Sector','A',ST_GEOMFROMTEXT('POLYGON(('||rec.sp_geometry||'))',4326),
		now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Sector');
		
		-- UPDATE STATUS INTO TEMP TABLE..
		update temp_du_tower set is_processed=true where system_id=rec.system_id;
		perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Sector'::character varying ,rec.province_id::integer,0,false));

		END;	
         END LOOP;    
	return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_sector(integer, integer)
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
requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,
rec.specification,rec.category,
rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,now(),
coalesce(rec.network_status,'P'),rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,
rec.no_of_input_port,rec.no_of_output_port,
rec.no_of_port,'Own', rec.audit_item_master_id, v_str_system_id, 'DU', p_uploadid,
rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

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

ALTER FUNCTION public.fn_uploader_insert_spliceclosure(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_splitter(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_splitter(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_splitter(
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
v_is_virtual_port_allowed boolean;
v_network_code_seperator character varying;
v_auto_character_count integer;
v_network_name character varying;
v_plan_id character varying;
v_data_upload_table character varying;
sql character varying;
v_PlanId_parent_network_id character varying;
v_layer_table character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,is_virtual_port_allowed,network_code_seperator into v_network_id_type,v_is_virtual_port_allowed,v_network_code_seperator from layer_details where upper(layer_name)='SPLITTER';

select plan_id into v_plan_id from upload_summary where id=p_uploadid limit 1;

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_splitter where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN

v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.splitter_name;
-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M'and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Splitter') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
IF(coalesce(v_plan_id, '')!= '')
then

--Select upper(layer_name) into rec.parent_entity_type from layer_details where upper(layer_title)= upper(rec.parent_entity_type);
--Select data_upload_table, layer_table into v_data_upload_table,v_layer_table
-- from layer_details where upper(layer_name)= upper(rec.parent_entity_type);

Select data_upload_table, layer_table, upper(layer_Name) into v_data_upload_table,
v_layer_table, rec.parent_entity_type from layer_details
where (upper(layer_name)=upper(rec.parent_entity_type) or upper(layer_title)= upper(rec.parent_entity_type));

raise info'v_data_upload_table%',v_data_upload_table;
raise info'v_layer_table%',v_layer_table;
raise info'rec.parent_entity_type:%',rec.parent_entity_type;

-- Commented by Diksha
--sql:='Select system_network_id from '||v_data_upload_table||' t
--where t.is_valid=true and t.client_id = '||rec.parent_client_id||' and t.client_id > 0 and is_processed=true and upload_id =
--(Select id from upload_summary where plan_id= '''||v_plan_id||''' and upper(entity_type) = upper('''||rec.parent_entity_type||''') order by id desc limit 1)';

--sql:='Select p.system_id,p.network_id,p.province_id,p.region_id from temp_du_splitter t inner join '|| v_layer_table||' p 
--on  cast(t.parent_client_id as character varying) = p.origin_ref_id 
 --order by 1 desc limit 1';

--raise info'sql:%',sql;
--execute sql into v_parent_system_id,v_PlanId_parent_network_id,rec.province_id,rec.region_id;

--raise info'v_PlanId_parent_network_id::%',v_PlanId_parent_network_id;

--sql:='Select system_id, province_id, region_id from '||v_layer_table||' where upper(network_id) = upper('''||v_PlanId_parent_network_id||''')';
--raise 'Sql 2:%',sql;
--execute sql into rec.parent_system_id,rec.province_id,rec.region_id;

raise info'rec.parent_system_id::%',rec.parent_system_id;
end if;

select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Splitter', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id limit 1;
End if;

IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;

IF(coalesce(v_plan_id, '')!= '' and coalesce(v_PlanId_parent_network_id, '')!= '')
THEN
v_parent_network_id=v_PlanId_parent_network_id;
END IF;

--INSERT INTO MAIN TABLE
insert into att_details_splitter(network_id, splitter_name, latitude, longitude, province_id,
region_id, address, splitter_ratio, specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, status, created_by, created_on, network_status, parent_system_id, parent_network_id,
parent_entity_type, sequence_id, splitter_type,type,brand,model,construction,activation,accessibility,
ownership_type, source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,
rec.no_of_input_port||':'||rec.no_of_output_port as port_ratio,
rec.specification,rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code,
rec.vendor_id, 'A', rec.created_by,
now(), coalesce(rec.network_status,'P'),v_parent_system_id,v_parent_network_id,REC.PARENT_ENTITY_TYPE,
v_sequence_id,rec.splitter_type,0,0,0,0,0,0,'Own', 'DU', rec.upload_id, rec.remarks,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y
returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,
approver_id,common_name,db_flag,network_status,no_of_ports,entity_category,display_name,st_x,st_y)
select v_current_system_id, 'Splitter', v_longitude, v_latitude, 'A',
ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by, 0, v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),
rec.no_of_input_port||':'||rec.no_of_output_port as port_ratio, rec.splitter_type ,fn_get_display_name(v_current_system_id,'Splitter'),rec.st_x,rec.st_y;

-- INSERT PORT DETAILS..
if v_is_virtual_port_allowed = false then
perform public.fn_bulk_insert_port_info(rec.no_of_input_port,rec.no_of_output_port,'Splitter', v_current_system_id, v_network_code,rec.created_by);
end if;

IF EXISTS (SELECT id FROM isp_entity_mapping p WHERE upper(p.entity_type)= upper(rec.parent_entity_type) and p.entity_id = v_parent_system_id) THEN
insert into isp_entity_mapping (structure_id,shaft_id,floor_id,entity_type,entity_id,parent_id)
Select p.structure_id,p.shaft_id,p.floor_id,'SPLITTER',v_current_system_id,p.id from isp_entity_mapping p
where upper(p.entity_type)= upper(rec.parent_entity_type) and p.entity_id = v_parent_system_id;
END IF;

END;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_splitter set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'SPLITTER'::character varying ,rec.province_id::integer,0,false));

END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_splitter(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_structure(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_structure(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_structure(
	p_uploadid integer,
	p_batchid integer)
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
	v_current_system_id integer;
	v_parent_system_id integer;
	v_parent_network_id character varying;
	v_parent_entity_type character varying;
	v_network_code character varying;
	v_sequence_id integer;
	v_network_id_type character varying;
	v_network_code_seperator character varying;
	v_latitude double precision;
	v_longitude double precision;
	v_shaft_length double precision;
	v_shaft_width double precision;
	v_floor_length double precision;
	v_floor_width double precision;
	v_floor_height double precision;
	v_arow_record record;
	v_auto_character_count integer;
	v_network_name character varying;
    v_province_id integer;
    v_region_id integer;
	v_floor_count integer;
v_shaft_count integer;
v_basement_count integer;
v_counter integer;
v_network_floor record;
	-- v_plan_id character varying;
-- 	v_data_upload_table character varying;
-- 	sql character varying;
-- 	v_PlanId_parent_network_id character varying;
-- 	v_layer_table  character varying;
BEGIN
v_sequence_id:=0;
--alter table att_details_bld_structure disable trigger all;
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='STRUCTURE';

--select plan_id into v_plan_id from upload_summary where id=p_uploadid limit 1; 

FOR REC IN  select * from temp_du_structure where upload_id=p_uploadid and is_valid=true
LOOP
	BEGIN	 
		
		
	 
    
	RAISE INFO 'v_network_id_type%', v_network_id_type;
		 
		v_latitude:=rec.latitude::double precision;
		v_longitude:=rec.longitude::double precision;
		v_sequence_id:=0;
		v_network_name:=rec.structure_name;
	    
	if(rec.origin_Ref_id is not null and rec.origin_from='SP') then 
		      Select bld.system_id,bld.network_id,bld.province_id ,bld.region_id 
		      into v_parent_system_id,v_parent_network_id,v_province_id,v_region_id
		     from  temp_du_structure str inner join att_details_building bld on
		    cast(str.parent_client_id as character varying) =bld.origin_Ref_id where str.system_id =rec.system_id order by 1 desc limit 1;
		   
		   update temp_du_structure set parent_system_id =v_parent_system_id, parent_network_id =v_parent_network_id,
		   province_id =v_province_id,region_id =v_region_id
		   where system_id  =rec.system_id;
			 
		    select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id 
			from fn_get_clone_network_code('Structure', 'Point',''||v_longitude||' '||v_latitude||'', 
			v_parent_system_id, rec.parent_entity_type) limit 1  into
			v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
			
		 -- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		elseif (v_network_id_type='M' and coalesce(REC.network_id,'')!='') 
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;	
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then
		
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		raise info 'Network code :%',v_network_code;
			select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
			from vw_layer_mapping where upper(child_layer_name)=upper('Structure') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
			v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
			-- GET NETWORK CODE & PARENT DETAILS..
			
			
			select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id 
			from fn_get_clone_network_code('Structure', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) limit 1  into
			v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
		
		
			
		end if;
		RAISE INFO 'v_network_code%', v_network_code;
		IF(coalesce(v_network_name,'')='')
  		then
			v_network_name=v_network_code;
  		END IF;
		--INSERT INTO MAIN TABLE
		
		insert into att_details_bld_structure(network_id, structure_name, latitude, longitude, province_id, region_id, 
		business_pass, home_pass, no_of_flat, no_of_occupants, status, created_by, created_on, network_status, 
			parent_system_id, parent_network_id, parent_entity_type, sequence_id,no_of_shaft,no_of_floor,building_id, 
			source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,origin_Ref_description,
			request_ref_id,requested_by,request_approved_by,st_x,st_y) 
			select v_network_code, v_network_name,v_latitude,v_longitude, province_id, region_id, 
			business_pass::integer,home_pass::integer,no_of_units::integer,no_of_occupants::integer, 'A', created_by, 
			created_on, coalesce(rec.network_status,'P'),
			parent_system_id,parent_network_id,PARENT_ENTITY_TYPE, v_sequence_id,no_of_shaft,no_of_floor,parent_system_id, 'DU',
			 p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
			 rec.request_ref_id,rec.requested_by,rec.request_approved_by,rec.st_x,rec.st_y
		from temp_du_structure where system_id = rec.system_id returning system_id into v_current_system_id;

		-- select v_network_code, v_network_name,v_latitude,v_longitude, province_id, region_id, 
-- 			business_pass::integer, home_pass::integer, no_of_units::integer, no_of_occupants::integer, 'A', created_by, created_on, 'P',
-- 			rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,2,1,v_parent_system_id, 'DU', p_uploadid, rec.remarks
-- 		from temp_du_structure where system_id = rec.system_id returning system_id into v_current_system_id;

		
		
		select value::double precision into v_shaft_length from global_settings where upper(key)=upper('DefaultShaftLength');
                select value::double precision into v_shaft_width from global_settings where upper(key)=upper('DefaultShaftWidth');
		select value::double precision into v_floor_length from global_settings where upper(key)=upper('DefaultFloorLength');
                select value::double precision into v_floor_width from global_settings where upper(key)=upper('DefaultFloorWidth');
		select value::double precision into v_floor_height from global_settings where upper(key)=upper('DefaultFloorHeight');
		
		Select no_of_shaft,no_of_floor,no_of_basement into v_shaft_count,v_floor_count,v_basement_count
	      from temp_du_structure where system_id = rec.system_id;
		
         v_counter  := 1; 
        while v_counter <= v_shaft_count
		LOOP
		BEGIN
		insert into isp_shaft_info(structure_id,shaft_name,status,created_by,created_on,length,width,shaft_position,network_status,is_virtual)
		values(v_current_system_id,concat('Shaft_',v_counter),'A',REC.created_by,now(),v_shaft_length,v_shaft_width,'left','P',true);
	    v_counter := v_counter + 1 ; 	
	    end ;
		end loop;
		
	    v_counter  := 1 ; 
		while v_counter <= v_basement_count
		LOOP
		BEGIN
         select * into v_arow_record from fn_get_network_code('Floor', 'Point', v_current_system_id, 'Structure', '');
		insert into isp_floor_info(network_id,structure_id,floor_name,status,created_by,created_on,height,length,width,network_status,no_of_units,
		parent_system_id,parent_entity_type,parent_network_id,sequence_id)
		values(v_arow_record.network_code,v_current_system_id,concat('Basement_',v_counter),'A',REC.created_by,now(),
		v_floor_length,v_floor_width,v_floor_height,
		coalesce(rec.network_status,'P'),1,v_current_system_id,'Structure',v_network_code,v_arow_record.sequence_id);
		v_counter := v_counter + 1 ; 	
		end;
		end loop;
		
	
		v_counter  := 1 ; 
         while v_counter <= v_floor_count
		LOOP
		BEGIN
         select * into v_network_floor from fn_get_network_code('Floor', 'Point', v_current_system_id, 'Structure', '');
		
		insert into isp_floor_info(network_id,structure_id,floor_name,status,created_by,created_on,height,length,width,network_status,no_of_units,
		parent_system_id,parent_entity_type,parent_network_id,sequence_id)
		values(v_arow_record.network_code,v_current_system_id,concat('Floor ',v_counter),'A',REC.created_by,now(),
		v_floor_length,v_floor_width,v_floor_height,
		coalesce(rec.network_status,'P'),1,v_current_system_id,'Structure',v_network_code,v_arow_record.sequence_id);
		v_counter := v_counter + 1 ; 	
		end;
		end loop;
		
		--INSERT INTO POINT MASTER
		insert into point_master(system_id, entity_type, longitude, latitude, approval_flag, 
		sp_geometry, approval_date, creator_remark, approver_remark, created_by,
		approver_id, common_name, db_flag, network_status,display_name,st_x,st_y)
		values(v_current_system_id, 'Structure', v_longitude, v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326), 
		now(), 'NA', 'NA', REC.created_by, 
		0, v_network_code, p_uploadid, 'P',fn_get_display_name(v_current_system_id,'Structure'),rec.st_x,rec.st_y);
		
		-- UPDATE STATUS INTO TEMP TABLE..
		update temp_du_structure set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Structure'::character varying ,rec.province_id::integer,0,false));

		
		END;
		
         END LOOP;  
	return 1;
	--alter table att_details_bld_structure enable trigger all;

END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_structure(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_tower(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_tower(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_tower(
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
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='TOWER';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_tower where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
LOOP
BEGIN
v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.network_name;
-- GET NETWORK CODE & PARENT DETAILS..

-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Tower') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Tower', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;

--INSERT DATA IN MAIN TABLE
insert into att_details_tower(network_id,network_name,latitude,longitude,province_id,
region_id, address,elevation,tower_height,operator_name, specification,category,subcategory1,subcategory2,subcategory3,
item_code,vendor_id,status,created_by,created_on,network_status,parent_system_id,no_of_sectors,
parent_network_id,parent_entity_type,sequence_id,ownership_type, source_ref_type, source_ref_id,remark,origin_from,
origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)

select v_network_code,v_network_name,v_latitude,v_longitude,rec.province_id,rec.region_id,rec.address,
rec.elevation,rec.tower_height,
rec.operator_name, rec.specification,
rec.category,rec.subcategory1,rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id,'A',rec.created_by,
now(),coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.no_of_sectors,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,'Own', 'DU',
p_uploadid, rec.remark,rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

--INSERT DATA IN POINT TABLE
insert into point_master (system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
select v_current_system_id,'Tower',v_longitude,v_latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),
now(),'NA','NA',rec.created_by,0,v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Tower'),rec.st_x,rec.st_y;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_tower set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Tower'::character varying ,rec.province_id::integer,0,false));

END;
END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_tower(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_tree(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_tree(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_tree(
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
v_network_code_seperator character varying;
v_latitude double precision;
v_longitude double precision;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='TREE';
-- INSERT BATCH WISE RECORDS..
FOR REC IN  select * from temp_du_tree where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
	LOOP
		BEGIN	 
		v_latitude:=rec.latitude::double precision;
		v_longitude:=rec.longitude::double precision; 
		v_sequence_id:=0; 
		v_network_name:=rec.tree_name;
-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M' and coalesce(REC.network_id,'')!='') 
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;	
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then
			v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
			select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
			from vw_layer_mapping where upper(child_layer_name)=upper('Tree') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
			v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
			-- GET NETWORK CODE & PARENT DETAILS..
			

			select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id 
			from fn_get_clone_network_code('Tree', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type)  into
			v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
		end if;
		IF(coalesce(v_network_name,'')='')
  		then
			v_network_name=v_network_code;
  		END IF;
		--INSERT INTO MAIN TABLE
		insert into att_details_tree(network_id, tree_name, latitude, longitude, province_id, region_id, 
		tree_no, tree_height, address,specification,category,subcategory1,subcategory2,subcategory3,
		item_code,vendor_id, status, created_by, created_on, network_status, 
		parent_system_id, parent_network_id, parent_entity_type, sequence_id,type,brand,model,construction,activation,
		accessibility, source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,origin_Ref_code,
		origin_Ref_description,request_ref_id,requested_by,request_approved_by,st_x,st_y)  
		select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, 
		rec.tree_no, rec.tree_height, rec.address,rec.specification,rec.category,rec.subcategory1,rec.subcategory2,
		rec.subcategory3,rec.item_code,
		rec.vendor_id,'A', rec.created_by, now(), coalesce(rec.network_status,'P'), rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE,
		v_sequence_id,0,0,0,0,0,0, 'DU', p_uploadid, rec.remarks,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,rec.st_x,rec.st_y 
		returning system_id into v_current_system_id;

		
		--INSERT INTO POINT MASTER
		insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
		creator_remark, approver_remark, created_by,approver_id, common_name, db_flag, network_status,display_name,st_x,st_y)
		select v_current_system_id, 'Tree', v_longitude, v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
		 now(), 'NA', 'NA', rec.created_by, 0, v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Tree'),rec.st_x,rec.st_y;

		-- UPDATE STATUS INTO TEMP TABLE..
		update temp_du_tree set is_processed=true where system_id=rec.system_id;
		perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Tree'::character varying ,rec.province_id::integer,0,false));

		END;
         END LOOP;  
	return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_tree(integer, integer)
    OWNER TO postgres;



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
			perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Trench'::character varying ,rec.province_id::integer,0,false));

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



-- FUNCTION: public.fn_uploader_insert_vault(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_vault(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_vault(
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
v_network_code_seperator character varying;
v_auto_character_count integer;
v_network_name character varying;
BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='VAULT';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_vault where upload_id=p_uploadid and is_valid=true and batch_id= p_batchid
LOOP
BEGIN

v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.vault_name;
-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('Vault') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..
select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('Vault', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;

end if;
IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_vault(network_id, vault_name, latitude, longitude, province_id, region_id, address, pincode,
specification, category,
subcategory1, subcategory2, subcategory3, item_code, vendor_id, status, created_by, created_on, network_status, parent_system_id,
parent_network_id, parent_entity_type, sequence_id,type,brand,model,construction,activation,accessibility,
ownership_type, source_ref_type, source_ref_id, audit_item_master_id, no_of_entry_exit_points, remarks,origin_from,
origin_Ref_id,origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id, rec.address,
rec.pin_code, rec.specification,
rec.category, rec.subcategory1, rec.subcategory2, rec.subcategory3, rec.item_code, rec.vendor_id, 'A',
rec.created_by, now(), coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,0,0,0,'Own', 'DU',
p_uploadid, rec.audit_item_master_id,0, rec.remarks,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,
rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag, sp_geometry,approval_date,creator_remark,
approver_remark,created_by,approver_id, common_name, db_flag, network_status,display_name,st_x,st_y)
select v_current_system_id,'Vault',v_longitude,v_latitude,'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(), 'NA', 'NA', rec.created_by,0, v_network_code, rec.upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'Vault'),rec.st_x,rec.st_y;

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_vault set is_processed=true where system_id=rec.system_id;
perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'Vault'::character varying ,rec.province_id::integer,0,false));

END;

END LOOP;
return 1;
END;

$BODY$;

ALTER FUNCTION public.fn_uploader_insert_vault(integer, integer)
    OWNER TO postgres;



-- FUNCTION: public.fn_uploader_insert_wallmount(integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_uploader_insert_wallmount(integer, integer);

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_wallmount(
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
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='WALLMOUNT';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_wallmount where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid
LOOP
BEGIN

v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.wallmount_name;
-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('WallMount') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..

select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('WallMount', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_wallmount(network_id, wallmount_name, latitude, longitude, province_id, region_id,
wallmount_no, wallmount_height, address,specification,category,subcategory1,subcategory2,subcategory3,
item_code,vendor_id, status, created_by, created_on, network_status,
parent_system_id, parent_network_id, parent_entity_type, sequence_id,type,brand,model,construction,activation,
accessibility,ownership_type,source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,
origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id,
rec.wallmount_no, rec.wallmount_height, rec.address,rec.specification,rec.category,rec.subcategory1,
rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id, 'A', rec.created_by, now(),coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,0,0,0,'Own', 'DU', p_uploadid, rec.remarks,
rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
select v_current_system_id, 'WallMount', v_longitude, v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(),'NA','NA',rec.created_by,0, v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'WallMount'),rec.st_x,rec.st_y;

perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'WallMount'::character varying ,rec.province_id::integer,0,false));

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_wallmount set is_processed=true where system_id=rec.system_id;

END;
END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_wallmount(integer, integer)
    OWNER TO postgres;







