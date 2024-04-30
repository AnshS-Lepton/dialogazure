CREATE OR REPLACE FUNCTION public.fn_uploader_insert_coupler(p_uploadid integer, p_batchid integer)
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
coalesce(rec.inner_dimention,'0')::double precision,coalesce(rec.outer_dimention,'0')::double precision,0,0,0,0,0,0,'Own', 'DU', p_uploadid,
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
$function$
;




CREATE OR REPLACE FUNCTION public.fn_uploader_insert_building(p_uploadid integer, p_batchid integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$

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
		rec.region_id,coalesce(rec.business_pass,0),coalesce(rec.home_pass,0),rec.total_tower,rec.no_of_floor,rec.no_of_flat,'Approved',
		coalesce(rec.network_status,'P'),'A',rec.upload_id,rec.pod_name,rec.pod_code,rec.rfs_status,rec.rfs_date,rec.tenancy,rec.category,
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
		VALUES(v_str_network_code,'Tower 1',coalesce(rec.business_pass,0),coalesce(rec.home_pass,0),1,1,1,2,v_building_system_id,rec.province_id,rec.region_id,'P','A',rec.created_by,now()
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
		perform (fn_geojson_update_entity_attribute(v_building_system_id::integer,'Building'::character varying ,rec.province_id::integer,0,false));

		END;	
         END LOOP;    
	return 1;
END;
$function$
;
