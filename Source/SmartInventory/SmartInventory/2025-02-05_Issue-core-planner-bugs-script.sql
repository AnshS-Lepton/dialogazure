CREATE OR REPLACE FUNCTION public.fn_get_fiberlink_schematicview(p_link_system_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

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
	string_agg('T'||T.tube_number||'('|| T.core_ref||')', '|')::Text as core_ref,T.color_code,T.network_status,T.cable_type,T.cable_category,T.is_backward_path
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
	select CONCAT(t.source_network_id,t.source_entity_type) as from,CONCAT(t.destination_network_id,t.destination_entity_type) as to,'<b>'||COALESCE(t.via_cable_display_name,'')||(case when is_multi_connection then '' else '\n'||'<b>' end)||tube_ref||
	CASE WHEN core_ref!='' THEN ',(' ELSE '' END||
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
$function$
;

------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION fn_delete_core_planner_logs(p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 

BEGIN   

delete from core_planner_logs  where user_id =p_user_id;

END;
$function$
;


--------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_splicing(required_core integer, p_user_id integer, fiber_link_network_id character varying, source_network_id character varying, destination_network_id character varying, buffer integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE 
    rec RECORD;
   v_arow RECORD;
    v_network_id character varying;
    v_system_id integer;
    v_cable_details_left record;
    v_cable_details_right record;
    v_cable_details record;
   p_link_system_id integer;
   start_point integer;
  v_status bool;
  v_message  character varying;
V_IS_CABLE_A_END BOOLEAN;
p_destination_network_id character varying;
BEGIN
V_IS_CABLE_A_END:=FALSE;
CREATE TEMP TABLE TEMP_LINE_MASTER
(
SYSTEM_ID INTEGER,
entity_type character varying,
network_id character varying,
SP_GEOMETRY GEOMETRY
)ON COMMIT DROP;

CREATE TEMP TABLE TEMP_POINT_MASTER
(
A_SYSTEM_ID INTEGER,
A_entity_type character varying,
A_network_id character varying,
SP_GEOMETRY GEOMETRY
)ON COMMIT DROP;

INSERT INTO TEMP_POINT_MASTER(A_SYSTEM_ID,A_network_id,A_entity_type,SP_GEOMETRY)
select A.a_system_id,P.COMMON_NAME,A.a_entity_type,P.SP_GEOMETRY from (      
         SELECT a_system_id,a_entity_type FROM core_planner_logs  where user_id = p_user_id and is_valid =TRUE
        union
        SELECT b_system_id,b_entity_type FROM core_planner_logs where user_id = p_user_id and is_valid =TRUE )a
		INNER JOIN point_master P ON P.SYSTEM_ID=A.a_system_id AND P.entity_type=A.a_entity_type;

INSERT INTO TEMP_LINE_MASTER(SYSTEM_ID,network_id,entity_type,SP_GEOMETRY)       
SELECT CABLE_id,CABLE_network_id,'Cable',P.SP_GEOMETRY FROM core_planner_logs A 
INNER JOIN LINE_master P ON P.SYSTEM_ID=A.CABLE_id AND P.entity_type='Cable'
where user_id = p_user_id and is_valid =TRUE ;

		
    v_network_id := '';
    v_system_id := 0;
   p_destination_network_id = destination_network_id;

 --truncate table temp_connection restart identity;
    -- Create a temporary table for connection data
   CREATE TEMP TABLE temp_connection (
        source_system_id integer,
        source_network_id character varying(100),
        source_entity_type character varying(100),
        source_port_no integer,
        destination_system_id integer,
        destination_network_id character varying(100),
        destination_entity_type character varying(100),
        destination_port_no integer,
        is_source_cable_a_end boolean NOT NULL DEFAULT false,
        is_destination_cable_a_end boolean NOT NULL DEFAULT false,
        equipment_system_id integer,
        equipment_network_id character varying(100),
        equipment_entity_type character varying(100),
        created_by integer,
        splicing_source character varying(100)
    ) ON COMMIT DROP;
     
  SELECT 
        (result_json->>'status')::BOOLEAN, 
        result_json->>'message'
    INTO v_status, v_message
    FROM fn_get_core_planner_validation(source_network_id, destination_network_id, buffer, required_core, p_user_id) AS result_json;
   
   IF v_status = false  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT v_status AS status, v_message AS message) row;
    ELSE
   select system_id into p_link_system_id from att_details_fiber_link adfl 
   where upper(network_id) = upper(fiber_link_network_id)
   or upper(link_id)=upper(fiber_link_network_id);        
        FOR rec IN select * from TEMP_POINT_MASTER order by a_entity_type
        LOOP									
        raise info 'rec 1 : %',rec;
		if(rec.a_entity_type='FMS')
		then
		
			SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) AS IS_CABLE_A_END INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
			WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) or
			ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)));				    

					INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source)
					
					select rec.a_system_id, rec.a_network_id, rec.a_entity_type, 
                    port_number, 
                    v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    port_number,false,v_cable_details_left.IS_CABLE_A_END, 
                    p_user_id, 'CorePlanning' 
					from isp_port_info 
					where is_valid_for_core_plan=true 
					and parent_network_id =source_network_id
					and input_output='O' and port_status_id=1
					order by port_number limit required_core; 		   							           		   
		else

		

	SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) AS IS_CABLE_A_END 
	INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
	WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
	OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) LIMIT 1;
		
		
	SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) AS IS_CABLE_A_END 
	INTO v_cable_details_right FROM TEMP_LINE_MASTER A				
	WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
	OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) 
	AND SYSTEM_ID NOT IN(v_cable_details_left.SYSTEM_ID) LIMIT 1;
		
        

		INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source,equipment_system_id, equipment_network_id, equipment_entity_type)
				
		select v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    port_number, 
                    v_cable_details_right.system_id, v_cable_details_right.network_id, 'Cable', 
                    port_number,
                    v_cable_details_left.IS_CABLE_A_END,
                    v_cable_details_right.IS_CABLE_A_END, 
                    p_user_id, 'CorePlanning',rec.a_system_id,rec.a_network_id,rec.a_entity_type 
					from isp_port_info 
					-- inner join att_details_cable_info c 
-- 					on c.cable_id=v_cable_details_left.system_id and c.fiber_number=port_number
-- 					and (case when v_cable_details_left.IS_CABLE_A_END then is_a_end_through_connectivity=false
-- 							  when v_cable_details_left.IS_CABLE_A_END=false then is_b_end_through_connectivity=false
-- 						else 1=1 end)
					where is_valid_for_core_plan=true 
					and parent_network_id =source_network_id
					and input_output='O' and port_status_id=1					
					order by port_number limit required_core;							    		                 	
		end if;

            
        END LOOP;

		update att_details_cable_info set link_system_id=coalesce(p_link_system_id,0)
		from(
		select * 
		from isp_port_info 
		where is_valid_for_core_plan=true 
		and parent_network_id =source_network_id
		and input_output='O' and port_status_id=1					
		order by port_number limit required_core)b
		where cable_id in(select destination_system_id from temp_connection where destination_entity_type = 'Cable' and created_by = p_user_id
		union
		select source_system_id from temp_connection where source_entity_type = 'Cable' and created_by = p_user_id) 
		and fiber_number=b.port_number;

		update isp_port_info set link_system_id=coalesce(p_link_system_id,0)
		where is_valid_for_core_plan=true 
		and parent_network_id =source_network_id
		and input_output='O' and port_status_id=1;

		update att_details_fiber_link set fiber_link_status='Associated',
		gis_length = ROUND(
        COALESCE(
            (SELECT Sum(cable_measured_length) FROM att_details_cable WHERE system_id in ((select destination_system_id from temp_connection where destination_entity_type = 'Cable' and created_by = p_user_id
		union
		select source_system_id from temp_connection where source_entity_type = 'Cable' and created_by = p_user_id) )), 0
        )::NUMERIC, 3
    ),
    total_route_length = ROUND(
        COALESCE(
            (SELECT sum(cable_calculated_length) FROM att_details_cable WHERE system_id in ((select destination_system_id from temp_connection where destination_entity_type = 'Cable' and created_by = p_user_id
		union
		select source_system_id from temp_connection where source_entity_type = 'Cable' and created_by = p_user_id) )), 0
        )::NUMERIC, 3
    )
		where system_id=coalesce(p_link_system_id,0);
			

		update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id =source_network_id
		and parent_entity_type='FMS' 
		--and port_number=v_arow.port_number 
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;

	   update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id = p_destination_network_id
		and parent_entity_type='FMS' 
		--and port_number=v_arow.port_number 
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;

           raise info 'end loop 1 : %',p_user_id;
          
     -- UPDATE temp_connection tmp
     -- SET source_network_id = pm.common_name
     -- FROM point_master pm
     -- WHERE pm.entity_type=tmp.source_entity_type and tmp.source_entity_type IN ('SpliceClosure', 'FMS','FDB','BDB')
     -- AND pm.system_id = tmp.source_system_id ;

     -- UPDATE temp_connection tmp
     -- SET equipment_network_id  = pm.common_name
     -- FROM point_master pm
     -- WHERE pm.entity_type=tmp.equipment_entity_type and tmp.equipment_entity_type IN ('SpliceClosure','FDB','BDB')
     -- AND pm.system_id = tmp.equipment_system_id ;
 
 
if (SELECT COUNT(*) FROM temp_connection) > 1
then 
 raise info 'end splicing 1 : %',p_user_id;
perform(fn_auto_provisioning_save_connections());

-- FOR rec in (select destination_system_id,destination_port_no from temp_connection where destination_entity_type = 'Cable' and created_by = p_user_id
-- union 
-- select source_system_id, source_port_no from temp_connection where source_entity_type = 'Cable' and created_by = p_user_id)
-- loop 
-- 
-- perform fn_associate_fiber_link_cable(rec.destination_system_id,coalesce(p_link_system_id,0),rec.destination_port_no,'A' );
-- 	
-- end loop;

end if;	

  --    update core_planner_logs set used_core = used_core + required_core,avaiable= case when avaiable - required_core < 0 
   --   then 0 else avaiable - required_core end where user_id = p_user_id and is_valid =true;
     
       delete from core_planner_logs where user_id = p_user_id;

       return query select row_to_json(row) from ( select true as status, 'ODF to ODF feasibility successfully completed' as message ) row;
      end if;
  END;
 
$function$
;


-----------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_associated_fiber_link_info(p_cable_id integer, p_fiber_number integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$ 
DECLARE
s_layer_columns_val text;
sql TEXT; 
BEGIN 

SELECT STRING_AGG(COLUMN_NAME||' as "'||DISPLAY_NAME||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM(
	 SELECT  COLUMN_NAME,DISPLAY_NAME  FROM fiber_link_columns_settings WHERE is_active=true and COLUMN_NAME not in('created_by','created_on','modified_on','modified_by') order by id ) A;
	 
	sql:='select lnk.'||S_LAYER_COLUMNS_VAL||',um.user_name as "Created By", fn_get_date(lnk.created_on) as "Created On",um2.user_name as "Modified By", fn_get_date(lnk.modified_on) as "Modified On"
	 from att_details_fiber_link lnk
	inner join att_details_cable_info cbl on lnk.system_id=cbl.link_system_id
	left join user_master um ON um.user_id=lnk.created_by
	left join user_master um2 ON um2.user_id=lnk.modified_by
	where cbl.cable_id='||p_cable_id||' and cbl.fiber_number='||p_fiber_number||'';

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';
END
$function$
;

