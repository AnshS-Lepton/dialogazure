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




CREATE OR REPLACE FUNCTION public.fn_get_fiberlink_schematicview(
	p_link_system_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


DECLARE nodes text;
 edges text; 
 nodesDownstream text;
 edgesDownstream text;
 legends text;
 cables text;
 v_layer_title character varying;
 v_layer_table character varying;
 v_entity_display_text character varying;
 latitude double precision;
 longitude double precision;
 created_on character varying;
 arow record;
parentrow record;
v_is_virtual boolean;
v_display_name_enabled boolean;
v_arow_child record;
v_port_query text;
v_child_entity_ports character varying;
v_child_system_id integer;
v_arow record;
V_AROW_SPLITTER RECORD;
v_entity_type character varying;
v_system_id integer;
v_fms_row record;
BEGIN
v_display_name_enabled:=true;
v_port_query:='';
v_entity_type:='FMS';
v_system_id:=0;

create temp table temp_fms(
id serial,
system_id integer,
port_no integer,
is_processed boolean default false
) on commit drop;

insert into temp_fms(system_id,port_no)
select  parent_system_id,port_number
from isp_port_info where link_system_id=p_link_system_id and input_output='O';

SELECT COUNT(1)>0 into v_display_name_enabled FROM GLOBAL_SETTINGS WHERE UPPER(KEY)='ISSCHEMATICVIEWDISPLAYNAMEENABLED' AND VALUE='1';

select is_virtual_port_allowed,layer_title,layer_table 
into v_is_virtual,v_layer_title,v_layer_table 
from layer_details where upper(layer_name)=upper(v_entity_type);

-- create temp table temp_entity_ports(
-- port_no integer
-- ) on commit drop;

  -- TEMP TABLE FOR CPF RESULT --
      create temp table temp_cpf_result
       (
     	id serial,
     	rowid integer,
     	level text,  	
       	connection_id integer,
       	parent_connection_id integer,
       	source_system_id integer,
       	source_network_id character varying(100),
       	source_entity_type character varying(100),
       	source_entity_title character varying(100),
      	source_port_no integer,
      	source_no_of_ports character varying(100),
      	source_display_name character varying,
      	source_entity_category character varying,
      	is_source_virtual boolean,
       	destination_system_id integer,
     	destination_network_id character varying(100),
      	destination_entity_type character varying(100),
      	destination_entity_title character varying(100),
     	destination_port_no integer,
     	destination_no_of_ports character varying(100),	
    	destination_display_name character varying,	
    	destination_entity_category character varying,
     	is_destination_virtual boolean,
      	is_customer_connected boolean,	
      	is_backward_path boolean,
     	trace_end character varying(1),
    	connected_on character varying(50),
    	via_entity_system_id integer,	
    	via_entity_network_id character varying(100),
    	via_entity_display_name character varying,
    	via_entity_Type character varying,
    	via_fiber_no integer,
    	is_deleted boolean default false,
        is_valid boolean default false		
     ) on commit drop;

  -- TEMP TABLE TO STORE THE PRIMARY SLD DATA

       CREATE TEMP TABLE temp_sld_data
     (
    	id serial ,
    	source_system_id integer,
    	source_network_id character varying(100),
    	source_entity_type character varying(100),
    	source_entity_title character varying(100),
    	source_display_name character varying,
    	source_no_of_ports character varying(100),
    	destination_system_id integer,
    	destination_display_name character varying,
    	destination_no_of_ports character varying(100),	
    	destination_network_id character varying(100),
    	destination_entity_type character varying(100),
    	destination_entity_title character varying(100),
    	via_cable_system_id integer,
    	via_cable_network_id character varying(100),
    	via_cable_display_name text,
    	via_entity_Type character varying,
    	total_core integer,
    	no_of_tube integer,
    	no_of_core_per_tube integer,
    	cable_calculated_length double precision,
    	tube_ref text,
    	core_ref text,
    	color_code character varying(50),
    	network_status text,
    	cable_type character varying(50),
    	cable_category character varying(50),
    	is_backward_path boolean
     ) on commit drop;

	create temp table temp_final_edges(
	source_network_id character varying,
	destination_network_id character varying,
	source_entity_type character varying,
	destination_entity_type  character varying,
	via_cable_display_name character varying,
	tube_ref text,
	core_ref text,
	via_entity_type character varying,
	cable_type character varying,
	color_code character varying,
	network_status character varying,
	rank int,
	--is_multi_connection boolean default false,
	is_backward_path boolean default false,
		source_no_of_ports character varying(100),
	destination_no_of_ports character varying(100)
	) on commit drop;

-- truncate table temp_final_edges;
-- truncate table temp_cpf_result;
-- truncate table temp_sld_data;

for v_fms_row in select system_id from temp_fms where is_processed=false group by system_id
loop

IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME)=UPPER('cpf_temp'))
		THEN
		DROP TABLE cpf_temp;
		END IF;

	v_system_id:=v_fms_row.system_id;

	-- GET ENTITY DISPLAY TEXT--
	--select layer_title,layer_table into v_layer_title,v_layer_table from layer_details where upper(layer_name)=upper(v_entity_type);
	execute 'select concat(network_id,''-('',network_status,'')''), latitude, longitude, now() from '||v_layer_table||' where system_id='||v_system_id||' limit 1 ' 
	into  v_entity_display_text,latitude,longitude,created_on;

	select round(latitude::numeric,6) into latitude;
	select round(longitude::numeric,6) into longitude;

	-- GET CONNECTION DATA BASED ON EQUIPMENT OR DEVICE 	   
	insert into temp_cpf_result(rowid,level,connection_id,parent_connection_id,source_system_id,source_network_id,source_entity_type,source_entity_title,
 	source_port_no,source_no_of_ports,source_display_name,source_entity_category,is_source_virtual,destination_system_id,
 	destination_network_id,destination_entity_type,destination_entity_title,destination_port_no,destination_no_of_ports,destination_display_name,	
	destination_entity_category,is_destination_virtual,is_customer_connected,is_backward_path,trace_end,connected_on
)
 	select distinct con.rowid,con.level,
 	con.connection_id,con.parent_connection_id,con.source_system_id,con.source_network_id,con.source_entity_type,con.source_entity_title,
 	con.source_port_no,con.source_no_of_ports,
 	(case when v_display_name_enabled then fn_get_display_network_name(con.source_system_id,con.source_entity_type) else con.source_display_name end),
 	con.source_entity_category,
 	con.is_source_virtual,con.destination_system_id,
 	con.destination_network_id,con.destination_entity_type,con.destination_entity_title,
 	con.destination_port_no,con.destination_no_of_ports,
 	(case when v_display_name_enabled then fn_get_display_network_name(con.destination_system_id,con.destination_entity_type) else con.destination_display_name end),	
	con.destination_entity_category,con.is_destination_virtual,con.is_customer_connected,con.is_backward_path,con.trace_end,
	fn_get_date(con.created_on)	
 	from fn_get_fiberlink_schematicview_date('','',1,100,'','',v_system_id,
 	(select string_agg(port_number::character varying,',')::character varying  from isp_port_info 
 	where parent_system_id=v_system_id and upper(parent_entity_type)=upper(v_entity_type) and upper(input_output)=upper('O')
 	and port_number in(select port_no from temp_fms where system_id=v_system_id))
 	,v_entity_type) con;

	update temp_fms set is_processed=true where system_id in
	(select source_system_id from temp_cpf_result where source_entity_type='FMS'
	union select destination_system_id from temp_cpf_result where destination_entity_type='FMS');

update TEMP_CPF_RESULT T set is_valid=true 
from att_details_cable_info cinfo 
where T.source_system_id=cinfo.cable_id and T.source_entity_type='Cable' and T.source_port_no=cinfo.fiber_number
and cinfo.link_system_id=p_link_system_id;

update TEMP_CPF_RESULT T set is_valid=true 
from att_details_cable_info cinfo 
where T.destination_system_id=cinfo.cable_id and T.destination_entity_type='Cable' and T.destination_port_no=cinfo.fiber_number
and cinfo.link_system_id=p_link_system_id;

update TEMP_CPF_RESULT set is_deleted=true where is_valid=false ; 

FOR AROW IN SELECT * FROM TEMP_CPF_RESULT WHERE IS_DELETED=FALSE and 
	(UPPER(SOURCE_ENTITY_TYPE)=UPPER(DESTINATION_ENTITY_TYPE)) and UPPER(SOURCE_ENTITY_TYPE) in('FMS','HTB')ORDER BY ID
	LOOP
		IF(UPPER(AROW.SOURCE_ENTITY_TYPE)=UPPER(AROW.DESTINATION_ENTITY_TYPE))
		THEN
			UPDATE temp_cpf_result SET PARENT_CONNECTION_ID=AROW.PARENT_CONNECTION_ID WHERE PARENT_CONNECTION_ID=AROW.CONNECTION_ID;
			UPDATE temp_cpf_result SET IS_DELETED=TRUE WHERE ID=AROW.ID;
		END IF;
	 END LOOP;

	UPDATE temp_cpf_result SET is_deleted=true where parent_connection_id=0 and upper(source_entity_type)='CABLE';

	--SKIPPING CABLE NODES AND MAINTAINED THE SAME AS VIA INFORMATION --
	FOR AROW IN SELECT * FROM TEMP_CPF_RESULT A WHERE A.IS_DELETED=FALSE AND UPPER(DESTINATION_ENTITY_TYPE)in('CABLE','PATCHCORD')
	LOOP
		SELECT * INTO V_AROW FROM TEMP_CPF_RESULT 
		WHERE SOURCE_SYSTEM_ID=AROW.DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(AROW.DESTINATION_ENTITY_TYPE)
		AND SOURCE_PORT_NO=AROW.DESTINATION_PORT_NO;

		UPDATE TEMP_CPF_RESULT SET IS_DELETED=TRUE WHERE ID =V_AROW.ID;
		UPDATE TEMP_CPF_RESULT SET DESTINATION_SYSTEM_ID=V_AROW.DESTINATION_SYSTEM_ID,
		DESTINATION_NETWORK_ID=V_AROW.DESTINATION_NETWORK_ID,
		DESTINATION_DISPLAY_NAME=V_AROW.DESTINATION_DISPLAY_NAME,
		DESTINATION_ENTITY_TYPE=V_AROW.DESTINATION_ENTITY_TYPE,
		DESTINATION_ENTITY_TITLE=V_AROW.DESTINATION_ENTITY_TITLE,
		DESTINATION_PORT_NO=V_AROW.DESTINATION_PORT_NO,
		DESTINATION_NO_OF_PORTS=V_AROW.DESTINATION_NO_OF_PORTS,
		VIA_ENTITY_SYSTEM_ID=V_AROW.SOURCE_SYSTEM_ID,
		VIA_ENTITY_NETWORK_ID= V_AROW.SOURCE_NETWORK_ID,
		VIA_ENTITY_DISPLAY_NAME=V_AROW.SOURCE_DISPLAY_NAME,
		VIA_ENTITY_TYPE=V_AROW.SOURCE_ENTITY_TYPE,
		VIA_FIBER_NO=V_AROW.SOURCE_PORT_NO
		WHERE CONNECTION_ID =AROW.CONNECTION_ID;
	END LOOP;

	FOR AROW IN SELECT * FROM TEMP_CPF_RESULT A WHERE A.IS_DELETED=FALSE AND UPPER(source_ENTITY_TYPE) in ('CABLE','PATCHCORD')
	LOOP
		SELECT * INTO V_AROW FROM TEMP_CPF_RESULT 
		WHERE DESTINATION_SYSTEM_ID=AROW.SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(AROW.SOURCE_ENTITY_TYPE)
		AND DESTINATION_PORT_NO=AROW.SOURCE_PORT_NO;

		UPDATE TEMP_CPF_RESULT SET IS_DELETED=TRUE WHERE ID =V_AROW.ID;
		UPDATE TEMP_CPF_RESULT SET DESTINATION_SYSTEM_ID=V_AROW.DESTINATION_SYSTEM_ID,
		SOURCE_NETWORK_ID=V_AROW.DESTINATION_NETWORK_ID,
		SOURCE_DISPLAY_NAME=V_AROW.DESTINATION_DISPLAY_NAME,
		SOURCE_ENTITY_TYPE=V_AROW.DESTINATION_ENTITY_TYPE,
		SOURCE_ENTITY_TITLE=V_AROW.DESTINATION_ENTITY_TITLE,
		SOURCE_PORT_NO=V_AROW.DESTINATION_PORT_NO,
		SOURCE_NO_OF_PORTS=V_AROW.DESTINATION_NO_OF_PORTS,
		VIA_ENTITY_SYSTEM_ID=V_AROW.SOURCE_SYSTEM_ID,
		VIA_ENTITY_NETWORK_ID= V_AROW.SOURCE_NETWORK_ID,
		VIA_ENTITY_DISPLAY_NAME=V_AROW.SOURCE_DISPLAY_NAME,
		VIA_ENTITY_TYPE=V_AROW.SOURCE_ENTITY_TYPE,
		VIA_FIBER_NO=V_AROW.SOURCE_PORT_NO
		WHERE CONNECTION_ID =AROW.CONNECTION_ID;
	END LOOP;        

end loop;

	-- QUERY TO FETCH AND INSERT THE PROCESSED SLD DATA INTO TEMP_SLD_DATA TABLE..
	insert into temp_sld_data(source_system_id,source_network_id,source_entity_type,source_entity_title,source_display_name,
	source_no_of_ports,
	destination_system_id,destination_display_name,destination_no_of_ports,destination_network_id,destination_entity_type,destination_entity_title,
	via_cable_system_id,via_cable_network_id,via_cable_display_name,via_entity_Type,total_core,
	no_of_tube,no_of_core_per_tube,cable_calculated_length,tube_ref,core_ref,color_code,network_status,cable_type,cable_category,is_backward_path)
	select 
	T.source_system_id,T.source_network_id,T.source_entity_type,T.source_entity_title,T.source_display_name,T.source_no_of_ports,
	T.destination_system_id,T.destination_display_name,T.destination_no_of_ports,T.destination_network_id,T.destination_entity_type,T.destination_entity_title,
	T.via_entity_system_id,T.via_entity_network_id,
	--T.via_entity_display_name,
	case when upper(T.via_entity_Type)='CABLE' then
	concat(T.via_entity_display_name,'(',coalesce((select Cast(cable_calculated_length as decimal(10,2)) from att_details_cable where system_id=t.via_entity_system_id),0),'m)')
	else T.via_entity_display_name end,
	T.via_entity_Type,T.total_core,
	T.no_of_tube,T.no_of_core_per_tube,T.cable_calculated_length,        
	string_agg(T.tube_ref, ', ')::Text as tube_ref,
	string_agg('('|| T.core_ref||')', '|')::Text as core_ref,T.color_code,T.network_status,T.cable_type,T.cable_category,T.is_backward_path
	 from (
		 select 
		 ci.source_system_id,
		 ci.source_network_id,
		 ci.source_entity_type,
		 ci.source_entity_title,
		 ci.source_display_name,
		 ci.source_no_of_ports,
		 ci.destination_system_id,
		 ci.destination_display_name,
		 ci.destination_no_of_ports,
		 ci.destination_network_id,
		 ci.destination_entity_type,
		 ci.destination_entity_title,
		 ci.via_entity_system_id,
		 ci.via_entity_network_id,
		 CASE WHEN Upper(ci.via_entity_type)='CABLE'
		 THEN
		 COALESCE(ci.via_entity_display_name,'') ||'('|| ci.no_of_tube ||'*'||ci.no_of_core_per_tube||')' 
		 ELSE ci.via_entity_display_name END  as via_entity_display_name,
		 ci.via_entity_Type,
		 ci.no_of_tube,
		 ci.tube_number,
		 count(1)::int as tube_wise_core_count,
		 ci.no_of_core_per_tube,ci.total_core,ci.cable_calculated_length,
		 Case 
		     when  count(1)::int = ci.no_of_core_per_tube
		     then 'T'::text||ci.tube_number::text
		     ELSE null
		     END as tube_ref,
		  Case 
		     when  count(1)::int = ci.no_of_core_per_tube then null
		     ELSE string_agg('F'::text ||  ci.fiber_number::text, ', ')::Text 
		     END as core_ref,
		     ci.color_code,
		     ci.network_status,
		     ci.cable_type,
		     ci.cable_category,
		     ci.is_backward_path			 
		 from (
			   select 
			cpf.connection_id,cpf.parent_connection_id,cpf.source_system_id,cpf.source_network_id,cpf.source_entity_type,
			cpf.source_entity_title,cpf.source_port_no,cpf.source_no_of_ports,cpf.source_display_name,cpf.source_entity_category,
			cpf.is_source_virtual,cpf.destination_system_id,cpf.destination_network_id,cpf.destination_entity_type,cpf.destination_entity_title,
			cpf.destination_port_no,cpf.destination_no_of_ports,cpf.destination_display_name,cpf.destination_entity_category,
			cpf.is_destination_virtual,cpf.is_customer_connected,cpf.is_backward_path,cpf.trace_end,cpf.connected_on,
			cpf.via_entity_system_id,cpf.via_entity_network_id,cpf.via_entity_display_name,cpf.via_entity_type,cpf.via_fiber_no,cpf.is_deleted
			   ,null as tube_number,null as core_number,null as fiber_number,null as no_of_tube,null as no_of_core_per_tube,
			   null as total_core,null as cable_calculated_length,null as color_code,null as network_status,null as cable_type,
			   null as cable_category from temp_cpf_result cpf where cpf.via_entity_type is null 

		           union
			   select 
			cpf.connection_id,cpf.parent_connection_id,cpf.source_system_id,cpf.source_network_id,cpf.source_entity_type,
				cpf.source_entity_title,cpf.source_port_no,cpf.source_no_of_ports,cpf.source_display_name,cpf.source_entity_category,
				cpf.is_source_virtual,cpf.destination_system_id,cpf.destination_network_id,cpf.destination_entity_type,cpf.destination_entity_title,
				cpf.destination_port_no,cpf.destination_no_of_ports,cpf.destination_display_name,cpf.destination_entity_category,
				cpf.is_destination_virtual,cpf.is_customer_connected,cpf.is_backward_path,cpf.trace_end,cpf.connected_on,
			cpf.via_entity_system_id,cpf.via_entity_network_id,cpf.via_entity_display_name,
			cpf.via_entity_type,cpf.via_fiber_no,cpf.is_deleted
			   ,cblinfo.tube_number,cblinfo.core_number,cblinfo.fiber_number,cbl.no_of_tube,cbl.no_of_core_per_tube,
			   (case when cbls.color_code is not null then cbl.total_core else null end)
			   ,cbl.cable_calculated_length,cbls.color_code,cbl.network_status,cbl.cable_type,
			   (case when cbls.color_code is not null then cbl.cable_category else null end) 
			   from temp_cpf_result cpf 
			   left join att_details_cable_info cblinfo 
			   on cpf.via_entity_system_id=cblinfo.cable_id and cpf.via_fiber_no=cblinfo.fiber_number
			   inner join att_details_cable  cbl 
			   on cpf.via_entity_system_id=cbl.system_id and cpf.via_entity_type='Cable'  -- via entity-type='cable'
			   left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type and cbls.cable_category=cbl.cable_category  
			   and cbls.fiber_count=cbl.total_core
			   union

			   select 
				cpf.connection_id,cpf.parent_connection_id,cpf.source_system_id,cpf.source_network_id,cpf.source_entity_type,
				cpf.source_entity_title,cpf.source_port_no,cpf.source_no_of_ports,cpf.source_display_name,cpf.source_entity_category,
				cpf.is_source_virtual,cpf.destination_system_id,cpf.destination_network_id,cpf.destination_entity_type,cpf.destination_entity_title,
				cpf.destination_port_no,cpf.destination_no_of_ports,cpf.destination_display_name,cpf.destination_entity_category,
				cpf.is_destination_virtual,cpf.is_customer_connected,cpf.is_backward_path,cpf.trace_end,cpf.connected_on,
				cpf.via_entity_system_id,cpf.via_entity_network_id,cpf.via_entity_display_name,cpf.via_entity_type,cpf.via_fiber_no,cpf.is_deleted
			   ,null as tube_number,null as core_number,null as fiber_number,null as no_of_tube,null as no_of_core_per_tube,
			   null as total_core,null as cable_calculated_length,null as color_code,null as network_status,null as cable_type,
			   null as cable_category from temp_cpf_result cpf
			   inner join att_details_patchCord pch on cpf.via_entity_system_id=pch.system_id  and upper(cpf.via_entity_type)='PATCHCORD'
		   ) ci 
		where ci.is_deleted=false 
		and upper(ci.source_system_id||ci.source_entity_type)!=upper(ci.destination_system_id||ci.destination_entity_type)
		Group By ci.source_system_id,ci.source_network_id,ci.source_entity_type,ci.source_entity_title,ci.source_display_name,ci.source_no_of_ports,
		ci.destination_system_id,ci.destination_display_name,ci.destination_no_of_ports,ci.destination_network_id,ci.destination_entity_type,destination_entity_title,
		ci.via_entity_system_id,ci.via_entity_network_id, ci.via_entity_display_name,ci.via_entity_type,ci.no_of_tube,
		ci.tube_number,ci.no_of_core_per_tube,ci.total_core,ci.cable_calculated_length,ci.color_code,ci.network_status,ci.cable_type,ci.cable_category,ci.is_backward_path
		order by ci.source_system_id, ci.tube_number
	 ) T
	 Group By T.source_system_id,T.source_network_id,T.source_entity_type,source_entity_title,T.source_display_name,T.source_no_of_ports,
	 T.destination_system_id,T.destination_display_name,T.destination_no_of_ports,T.destination_network_id,T.destination_entity_type,destination_entity_title,
	 T.via_entity_system_id,T.via_entity_network_id, T.via_entity_display_name,T.via_entity_Type,T.total_core,T.cable_calculated_length, T.no_of_tube,
	 T.no_of_core_per_tube,
	 T.cable_calculated_length,T.color_code,T.network_status,T.cable_type,T.cable_category,T.is_backward_path
	 order by T.source_system_id;  

insert into temp_final_edges(source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,tube_ref,
core_ref,via_entity_type,cable_type,color_code,network_status,rank,is_backward_path,source_no_of_ports,destination_no_of_ports)
select t.source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,tube_ref,
core_ref,via_entity_type,cable_type,color_code,network_status
,(row_number() over(partition by t.source_network_id,t.destination_network_id)),is_backward_path,source_no_of_ports,destination_no_of_ports 
	from(Select distinct source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,(Select * from fn_get_sequence_arrange(tube_ref,'T')) as tube_ref,
	(select * from fn_get_fibre_merge_sequence(core_ref))as core_ref,
	via_entity_Type,cable_type,color_code,network_status,is_backward_path,source_no_of_ports,destination_no_of_ports 
	from temp_sld_data where destination_entity_title!='Splitter' and is_backward_path=false)t;

insert into temp_final_edges(source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,tube_ref,
core_ref,via_entity_type,cable_type,color_code,network_status,rank,is_backward_path,source_no_of_ports,destination_no_of_ports)
select t.source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,tube_ref,
core_ref,via_entity_type,cable_type,color_code,network_status,
(row_number() over(partition by t.source_network_id,t.destination_network_id)),is_backward_path,source_no_of_ports,destination_no_of_ports 
	from(Select distinct source_network_id,source_entity_type,destination_entity_type,destination_network_id,via_cable_display_name,
	(Select * from fn_get_sequence_arrange(tube_ref,'T')) as tube_ref,
	(select * from fn_get_fibre_merge_sequence(core_ref))as core_ref,
	via_entity_Type,cable_type,color_code,network_status,is_backward_path,source_no_of_ports,destination_no_of_ports 
	from temp_sld_data 
	where destination_entity_title!='Splitter' and is_backward_path=true)t;

-- update temp_final_edges set is_multi_connection=true 
-- where upper(source_network_id)=upper(destination_network_id) and rank>1 and is_backward_path=false;
-- 
-- update temp_final_edges set is_multi_connection=true 
-- where upper(source_network_id)=upper(destination_network_id) and rank>1 and is_backward_path=true;
-- 

-- NODES UPSREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
	select distinct * from (
	select CONCAT(source_network_id,source_entity_type,source_no_of_ports) as id,
	case when upper(source_entity_type)!='CABLE'	
	then
	CONCAT('<b>',CASE WHEN UPPER(source_entity_title)='SPLICE CLOSURE' THEN 'SC' ELSE source_entity_title END,
	CASE WHEN COALESCE(Source_no_of_ports,'')!='' then ' (' else '' end,
	COALESCE(source_no_of_ports,''),
	CASE WHEN COALESCE(source_no_of_ports,'')!='' then ')' else '' end,'</b>','\n',source_display_name)
	else '' end as label,
	source_entity_type as group, null as title from temp_sld_data where source_entity_title!='Splitter' and is_backward_path=true
	union
	select  CONCAT(destination_network_id,destination_entity_type,destination_no_of_ports) as id,
	case when upper(destination_entity_type)!='CABLE'
	then
	CONCAT('<b>',CASE WHEN UPPER(destination_entity_title)='SPLICE CLOSURE' THEN 'SC' ELSE destination_entity_title END ,
	CASE WHEN COALESCE(destination_no_of_ports,'')!='' then ' (' else '' end,
	COALESCE(destination_no_of_ports,''),
	CASE WHEN COALESCE(destination_no_of_ports,'')!='' then ')' else '' end,'</b>','\n',destination_display_name)
	else '' end
	 as label, destination_entity_type as group, null as title from temp_sld_data where destination_entity_title!='Splitter' and is_backward_path=true	
	) a
) x) into nodes;

raise info'%nodes',nodes;

-- EDGES UPSTREAM JSON--
 Select (select array_to_json(array_agg(row_to_json(x)))from (
	select CONCAT(t.source_network_id,t.source_entity_type,t.source_no_of_ports) as from,CONCAT(t.destination_network_id,t.destination_entity_type,t.destination_no_of_ports) as to,'<b>'||COALESCE(t.via_cable_display_name,'')||(case when is_multi_connection  then '' else '\n'||'<b>' end)||tube_ref||
	CASE WHEN core_ref!='' THEN ',(' ELSE '' END||
	core_ref||
	CASE WHEN core_ref!='' THEN ')' ELSE '' END||
	(case when is_multi_connection then '' else '</b>' end)||(CASE WHEN core_ref!='' THEN '\n'||'\n' else '' end) as label,

	 CASE WHEN UPPER(t.via_entity_Type)!='PATCHCORD' then
	case WHEN t.color_code IS NULL THEN  
	 CASE
                WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '{"color": "#FF0000"}'::json
                WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '{"color": "#0000FF"}'::json  
                WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '{"color": "#FD10FD"}'::json 
                WHEN upper(t.cable_type::text) = 'ISP'::text THEN '{"color": "#000"}'::json 
                ELSE '{"color": "#0000FF"}'::json
            END 
	ELSE concat('{"color": "',t.color_code,'"}')::json
	END
	ELSE '{"color": "#000"}'::json 
	END  as color,
	0 as length,
	 CASE WHEN UPPER(t.via_entity_Type)!='PATCHCORD'
	THEN
	CASE 
		WHEN upper(t.network_status::text) = 'P'::text THEN true 
		WHEN upper(t.network_status::text) = 'A'::text THEN false
	ELSE true 
	END
	ELSE false end as dashes,
	CASE WHEN UPPER(t.via_entity_type)='PATCHCORD' then 3 else 1.5 end as width,
	concat('{"type": "'||(case when rank%2=0 then 'curvedCW' else 'curvedCCW' end)||'", "roundness": "',
	(case when is_multi_connection and rank>1 then rank*0.1 else 0 end ),'"}')::json as smooth
	from (select t.*,(select count(1)>1 from temp_final_edges t2
	where upper(t2.source_network_id)=upper(t.source_network_id) 
	and upper(t2.destination_network_id)=upper(t.destination_network_id) 
	and is_backward_path=true) as is_multi_connection from temp_final_edges t 
	where is_backward_path=true)
	t --where is_backward_path=true

	-- (select t.*,(row_number() over(partition by t.source_network_id,t.destination_network_id)) as rank,
-- 	(row_number() over(partition by t.source_network_id,t.destination_network_id))>1 as is_multi_connection 
-- 	from(Select distinct source_network_id,source_entity_type,destination_entity_type,destination_network_id,via_cable_display_name,
-- 	(Select * from fn_get_sequence_arrange(tube_ref,'T')) as tube_ref,
-- 	(select * from fn_get_fibre_merge_sequence(core_ref))as core_ref,
-- 	via_entity_Type,cable_type,color_code,network_status from temp_sld_data 
-- 	where destination_entity_title!='Splitter' and is_backward_path=true
-- 	)t) 

) x) into edges;

 -- NODES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select * from(
select row_number() over(partition by id) rn,* from(
	select distinct * from (
	select CONCAT(source_network_id,source_entity_type) as id,
case when upper(source_entity_type)!='CABLE'	
	then
	CONCAT('<b>',CASE WHEN UPPER(source_entity_title)='SPLICE CLOSURE' THEN 'SC' ELSE source_entity_title END,
	CASE WHEN COALESCE(Source_no_of_ports,'')!='' then ' (' else '' end,
	COALESCE(source_no_of_ports,''),
	CASE WHEN COALESCE(source_no_of_ports,'')!='' then ')' else '' end,'</b>','\n',source_display_name)
	 else '' end as label

	,source_entity_type as group, null as title from temp_sld_data where source_entity_title!='Splitter' 
	--and is_backward_path=false
	union
	select  CONCAT(destination_network_id,destination_entity_type) as id,
case when upper(destination_entity_type)!='CABLE'	
	then
	CONCAT('<b>',CASE WHEN UPPER(destination_entity_title)='SPLICE CLOSURE' THEN 'SC' ELSE destination_entity_title END ,
	CASE WHEN COALESCE(destination_no_of_ports,'')!='' then ' (' else '' end,
	COALESCE(destination_no_of_ports,''),
	CASE WHEN COALESCE(destination_no_of_ports,'')!='' then ')' else '' end,'</b>','\n',destination_display_name) else '' end
	 as label, destination_entity_type as group, null as title from temp_sld_data where destination_entity_title!='Splitter' 
	 --and is_backward_path=false	
	) a)b)b where rn=1
) x) into nodesDownstream;

-- EDGES DOWNSTREAM JSON--
 Select (select array_to_json(array_agg(row_to_json(x)))from (
	select CONCAT(t.source_network_id,t.source_entity_type) as from,CONCAT(t.destination_network_id,t.destination_entity_type) as to,'<b>'||COALESCE(t.via_cable_display_name,'')||(case when is_multi_connection then '' else '\n'||'<b>' end)
     --||tube_ref
     ||
	CASE WHEN core_ref!='' THEN '(' ELSE '' END||
	Replace(core_ref,'F','Core-')||
	CASE WHEN core_ref!='' THEN ')' ELSE '' END||
	(case when is_multi_connection then '' else '</b>' end)||(CASE WHEN core_ref!='' THEN '\n'||'\n' else '' end) as label,

	 CASE WHEN UPPER(t.via_entity_Type)!='PATCHCORD' then
	case WHEN t.color_code IS NULL THEN  
	 CASE
                WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '{"color": "#FF0000"}'::json
                WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '{"color": "#0000FF"}'::json  
                WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '{"color": "#FD10FD"}'::json 
                WHEN upper(t.cable_type::text) = 'ISP'::text THEN '{"color": "#000"}'::json 
                ELSE '{"color": "#0000FF"}'::json
            END 
	ELSE concat('{"color": "',t.color_code,'"}')::json
	END
	ELSE '{"color": "#000"}'::json 
	END  as color,
	0 as length,
	CASE WHEN UPPER(t.via_entity_Type)!='PATCHCORD'
	THEN
	CASE 
		WHEN upper(t.network_status::text) = 'P'::text THEN true 
		WHEN upper(t.network_status::text) = 'A'::text THEN false
	ELSE true 
	END
	ELSE false end as dashes,
	CASE WHEN UPPER(t.via_entity_type)='PATCHCORD' then 3 else 1.5 end as width, 
	concat('{"type": "'||(case when rank%2=0 then 'curvedCW' else 'curvedCCW' end)||'", "roundness": "',
	(case when is_multi_connection and rank>1 then rank*0.1 else 0 end ),'"}')::json as smooth
	from  (select t.*,(select count(1)>1 from temp_final_edges t2
	where upper(t2.source_network_id)=upper(t.source_network_id) 
	and upper(t2.destination_network_id)=upper(t.destination_network_id) 
	--and is_backward_path=false
	) as is_multi_connection from temp_final_edges t 
	--where is_backward_path=false
	)
	t
) x) into edgesDownstream;

--LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (

	select distinct * from (
		select source_entity_type as entity_type, source_entity_title as entity_title,is_backward_path as upstream from temp_sld_data
		union
		select destination_entity_type,destination_entity_title,is_backward_path as upstream from temp_sld_data 	
	 ) a where a.entity_title not in ('Cable','Splitter') order by entity_title
) x) into legends;

--select * from temp_sld_data

--CABLE LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
	 Select  case WHEN color_code IS NULL THEN  
	 CASE
                WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '#FF0000'::text
                WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '#0000FF'::text  
                WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '#FD10FD'::text 
                WHEN upper(t.cable_type::text) = 'ISP'::text THEN '#000'::text 
                ELSE '#0000FF'::text
            END 
	ELSE t.color_code::text
	END as color_code,CONCAT(COALESCE(t.cable_category,''),
	 CASE
                WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN ' OH'
                WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN ' UG' 
                WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN ' WC'
                WHEN upper(t.cable_type::text) = 'ISP'::text THEN ' ISP'
		ELSE ''
            END ,
	(case when total_core is not null then '('||t.total_core||'F)' else '' end))as text,is_backward_path as upstream 
	from( 
	select color_code,cable_category,cable_type,total_core,is_backward_path from temp_sld_data 
	 group by color_code,cable_category,cable_type,total_core,is_backward_path )t where t.cable_type is not null
) x) into cables;

-- -- IF CASE OF NULL DATA
--    IF nodes is null THEN
--       Select(select array_to_json(array_agg(row_to_json(a)))from (
-- 	Select v_system_id as id,v_layer_title as label,v_entity_type as group , null as title)a) into nodes;
-- 	END IF;

-- IF edges is null THEN
-- Select (select array_to_json(array_agg(row_to_json(x)))from (
-- select v_system_id as from,0 as to,v_layer_title as label
-- ) x) into edges;
-- END IF;

-- IF legends is null THEN
-- select (select array_to_json(array_agg(row_to_json(x)))from (
-- 
-- 	select v_entity_type as entity_type, v_layer_title as entity_title,true as is_backward_path
-- 	union
-- 	select v_entity_type as entity_type, v_layer_title as entity_title,false as is_backward_path
-- ) x) into legends;
-- END IF;
-----------------------------------------------------------------------------------
return query select row_to_json(result) from (
select v_entity_type as entityType, v_layer_title as entityTitle,v_entity_display_text as entityDisplayText,latitude,longitude,
	now(),nodes,edges,nodesDownstream,edgesDownstream,legends,cables
) result;
END; 
$BODY$;
