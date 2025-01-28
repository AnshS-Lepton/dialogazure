-- FUNCTION: public.fn_get_fiberlink_schematicview(integer)

-- DROP FUNCTION IF EXISTS public.fn_get_fiberlink_schematicview(integer);

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
from isp_port_info where link_system_id=p_link_system_id;

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
    	is_deleted boolean default false	
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
	is_backward_path boolean default false
	) on commit drop;


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
core_ref,via_entity_type,cable_type,color_code,network_status,rank,is_backward_path)
select t.source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,tube_ref,
core_ref,via_entity_type,cable_type,color_code,network_status
,(row_number() over(partition by t.source_network_id,t.destination_network_id)),is_backward_path
	from(Select distinct source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,(Select * from fn_get_sequence_arrange(tube_ref,'T')) as tube_ref,
	(select * from fn_get_fibre_merge_sequence(core_ref))as core_ref,
	via_entity_Type,cable_type,color_code,network_status,is_backward_path from temp_sld_data where destination_entity_title!='Splitter' and is_backward_path=false)t;

insert into temp_final_edges(source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,tube_ref,
core_ref,via_entity_type,cable_type,color_code,network_status,rank,is_backward_path)
select t.source_network_id,destination_network_id,source_entity_type,destination_entity_type,via_cable_display_name,tube_ref,
core_ref,via_entity_type,cable_type,color_code,network_status,
(row_number() over(partition by t.source_network_id,t.destination_network_id)),is_backward_path
	from(Select distinct source_network_id,source_entity_type,destination_entity_type,destination_network_id,via_cable_display_name,
	(Select * from fn_get_sequence_arrange(tube_ref,'T')) as tube_ref,
	(select * from fn_get_fibre_merge_sequence(core_ref))as core_ref,
	via_entity_Type,cable_type,color_code,network_status,is_backward_path from temp_sld_data 
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
	select CONCAT(source_network_id,source_entity_type) as id,
	case when upper(source_entity_type)!='CABLE'	
	then
	CONCAT('<b>',CASE WHEN UPPER(source_entity_title)='SPLICE CLOSURE' THEN 'SC' ELSE source_entity_title END,
	CASE WHEN COALESCE(Source_no_of_ports,'')!='' then ' (' else '' end,
	COALESCE(source_no_of_ports,''),
	CASE WHEN COALESCE(source_no_of_ports,'')!='' then ')' else '' end,'</b>','\n',source_display_name)
	else '' end as label,
	source_entity_type as group, null as title from temp_sld_data where source_entity_title!='Splitter' and is_backward_path=true
	union
	select  CONCAT(destination_network_id,destination_entity_type) as id,
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
	select CONCAT(t.source_network_id,t.source_entity_type) as from,CONCAT(t.destination_network_id,t.destination_entity_type) as to,'<b>'||COALESCE(t.via_cable_display_name,'')||(case when is_multi_connection  then '' else '\n'||'<b>' end)||tube_ref||
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
	select distinct * from (
	select CONCAT(source_network_id,source_entity_type) as id,
case when upper(source_entity_type)!='CABLE'	
	then
	CONCAT('<b>',CASE WHEN UPPER(source_entity_title)='SPLICE CLOSURE' THEN 'SC' ELSE source_entity_title END,
	CASE WHEN COALESCE(Source_no_of_ports,'')!='' then ' (' else '' end,
	COALESCE(source_no_of_ports,''),
	CASE WHEN COALESCE(source_no_of_ports,'')!='' then ')' else '' end,'</b>','\n',source_display_name)
	 else '' end as label

	,source_entity_type as group, null as title from temp_sld_data where source_entity_title!='Splitter' and is_backward_path=false
	union
	select  CONCAT(destination_network_id,destination_entity_type) as id,
case when upper(destination_entity_type)!='CABLE'	
	then
	CONCAT('<b>',CASE WHEN UPPER(destination_entity_title)='SPLICE CLOSURE' THEN 'SC' ELSE destination_entity_title END ,
	CASE WHEN COALESCE(destination_no_of_ports,'')!='' then ' (' else '' end,
	COALESCE(destination_no_of_ports,''),
	CASE WHEN COALESCE(destination_no_of_ports,'')!='' then ')' else '' end,'</b>','\n',destination_display_name) else '' end
	 as label, destination_entity_type as group, null as title from temp_sld_data where destination_entity_title!='Splitter' and is_backward_path=false	
	) a
) x) into nodesDownstream;



-- EDGES DOWNSTREAM JSON--
 Select (select array_to_json(array_agg(row_to_json(x)))from (
	select CONCAT(t.source_network_id,t.source_entity_type) as from,CONCAT(t.destination_network_id,t.destination_entity_type) as to,'<b>'||COALESCE(t.via_cable_display_name,'')||(case when is_multi_connection then '' else '\n'||'<b>' end)||tube_ref||
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
	from  (select t.*,(select count(1)>1 from temp_final_edges t2
	where upper(t2.source_network_id)=upper(t.source_network_id) 
	and upper(t2.destination_network_id)=upper(t.destination_network_id) 
	and is_backward_path=false) as is_multi_connection from temp_final_edges t 
	where is_backward_path=false)
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
	created_on,nodes,edges,nodesDownstream,edgesDownstream,legends,cables
) result;
END; 
$BODY$;

-- FUNCTION: public.fn_get_fiberlink_schematicview_date(character varying, character varying, integer, integer, character varying, character varying, integer, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_fiberlink_schematicview_date(character varying, character varying, integer, integer, character varying, character varying, integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_fiberlink_schematicview_date(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_entity_system_id integer,
	p_entity_port_no character varying,
	p_entity_type character varying)
    RETURNS TABLE(id integer, rowid integer, level text, connection_id integer, parent_connection_id integer, source_system_id integer, source_network_id character varying, source_entity_type character varying, source_entity_title character varying, source_port_no integer, is_source_virtual boolean, destination_system_id integer, destination_network_id character varying, destination_entity_type character varying, destination_entity_title character varying, destination_port_no integer, is_destination_virtual boolean, is_customer_connected boolean, created_on timestamp without time zone, created_by integer, approved_by integer, approved_on timestamp without time zone, source_no_of_ports character varying, destination_no_of_ports character varying, is_backward_path boolean, trace_end character varying, source_display_name character varying, destination_display_name character varying, source_entity_category character varying, destination_entity_category character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
declare v_down_stream_end character varying(1);	
v_arow_port record;
v_max_row_count integer;
BEGIN
v_down_stream_end:='';
--WE HAVE PROVIDED THE MAX COUNT BECAUSE OF RECURSION DOES NOT SUPPORT GREATER THEN 32000 RECORDS.
v_max_row_count:=2000;
--TEMP TABLE TO STORE CONNECTION INFO RESULT..
 create temp table cpf_temp
 (
 	level text,
 	rowid integer,
 	connection_id integer,
 	parent_connection_id integer,
 	source_system_id integer,
 	source_network_id character varying(100),
 	source_entity_type character varying(100),
 	source_entity_title character varying(100),
 	source_port_no integer,
 	is_source_virtual boolean,
 	destination_system_id integer,
 	destination_network_id character varying(100),
 	destination_entity_type character varying(100),
 	destination_entity_title character varying(100),
 	destination_port_no integer,
 	is_destination_virtual boolean,
 	is_customer_connected boolean,
 	created_on timestamp ,  		
 	created_by integer,
 	approved_by integer,
 	approved_on timestamp,
 	trace_end character varying(1),
 	source_no_of_ports character varying(100),
 	destination_no_of_ports character varying(100),
 	source_display_name character varying,
	destination_display_name character varying,
	source_entity_category character varying,
	destination_entity_category character varying
 ) on commit drop;
	--select cable_measured_length from att_details_cable 

	insert into cpf_temp(level,rowid,connection_id,parent_connection_id,source_system_id,source_network_id,source_entity_type,source_entity_title,source_port_no,source_display_name,is_source_virtual,
	destination_system_id,destination_network_id,destination_entity_type,destination_entity_title,destination_port_no,destination_display_name,is_destination_virtual,
	is_customer_connected,
	created_on,created_by,approved_by,approved_on,source_entity_category,destination_entity_category,trace_end,
	source_no_of_ports,destination_no_of_ports)
	select 
	cinfo.level,cinfo.rowid,cinfo.connection_id,cinfo.parent_connection_id,cinfo.source_system_id,cinfo.source_network_id,
	cinfo.source_entity_type,
	case when upper(ls.layer_name)='EQUIPMENT' then coalesce(srcsubtype.layer_title,cinfo.source_entity_sub_type) else ls.layer_title end as source_entity_title,
	cinfo.source_port_no,cinfo.source_display_name,
	ls.is_virtual_port_allowed as is_source_virtual,
	cinfo.destination_system_id,cinfo.destination_network_id,
	cinfo.destination_entity_type,
	case when upper(ld.layer_name)='EQUIPMENT' then coalesce(dstsubtype.layer_title,cinfo.destination_entity_sub_type) else ld.layer_title end  as destination_entity_title,
	cinfo.destination_port_no,cinfo.destination_display_name,
	ld.is_virtual_port_allowed as is_destination_virtual,
	cinfo.is_customer_connected,cinfo.created_on,cinfo.created_by,cinfo.approved_by,
	cinfo.approved_on,
	p1.entity_category as source_entity_category,
	p2.entity_category as destination_entity_category, 
	cinfo.trace_end,
	p1.no_of_ports as source_no_of_ports,
	p2.no_of_ports as destination_no_of_ports
	 from 
	(

	   select  a.*,'A' as trace_end from (
		with RECURSIVE UpStream as (
			select  1 as rowid, '-1' as level, ci.connection_id,0 as parent_connection_id,ci.source_system_id,ci.source_network_id,
			ci.source_entity_type,ci.source_port_no,
			ci.destination_system_id ,ci.destination_network_id,ci.destination_entity_type,ci.destination_port_no,ci.is_customer_connected,
			ci.created_on,ci.created_by,ci.approved_by,ci.approved_on,
			ci.source_entity_sub_type,ci.destination_entity_sub_type,
			ci.source_display_name,ci.destination_display_name
			from vw_connection_info_sld ci
			where ci.destination_system_id=p_entity_system_id and ci.destination_port_no  in (SELECT regexp_split_to_table(p_entity_port_no, ',')::integer) and 
			lower(ci.destination_entity_type) = lower(p_entity_type)
			--and ((ci.source_entity_type::text)||(ci.source_system_id::text))!=((ci.destination_entity_type::text)||(ci.destination_system_id::text))
			union 
			select  cte.rowid+1 as rowid,
			cte.level || '-' || row_number() over (order by cte.level, cte.source_port_no)::text as level, 
			ci.connection_id,cte.connection_id as parent_connection_id,ci.source_system_id,ci.source_network_id,ci.source_entity_type,ci.source_port_no,
			ci.destination_system_id ,ci.destination_network_id,ci.destination_entity_type,ci.destination_port_no,
			ci.is_customer_connected,ci.created_on,ci.created_by,ci.approved_by,ci.approved_on,
			ci.source_entity_sub_type,ci.destination_entity_sub_type,ci.source_display_name,ci.destination_display_name
			from vw_connection_info_sld ci 
			inner join UpStream cte on 
			ci.destination_system_id=cte.source_system_id and 
			ci.destination_port_no =cte.source_port_no and lower(ci.destination_entity_type) = lower(cte.source_entity_type)
			-- condition to get rid off from infinite loop in case of ring connectivity..
			where ((lower(ci.destination_entity_type)||ci.destination_system_id||ci.destination_port_no) not in  (SELECT lower(p_entity_type)||p_entity_system_id||regexp_split_to_table(p_entity_port_no, ',')))
			-- and  
-- 			case 
-- 			when upper(ci.destination_entity_type)='CUSTOMER' then 1=2  
-- 			when upper(ci.destination_entity_type)='SPLITTER' and coalesce(p_sld_type,'')!='' and upper(coalesce(ci.destination_splitter_type,''))=upper(coalesce(p_sld_type,'')) 
-- 			then 
-- 			1=2 
-- 			else 1=1 end

			and  case when upper(ci.destination_entity_type)=upper('FMS') then 1=2 else 1=1 end
			and cte.rowid<=v_max_row_count	

		) select * from UpStream  order by rowid desc) a

	 union all

	 select b.*,'B' as trace_end from (
		with RECURSIVE DownStream as (
			select  1 as rowid,'1' as level, ci.connection_id,0 as parent_connection_id,ci.source_system_id,ci.source_network_id,ci.source_entity_type,ci.source_port_no,
			ci.destination_system_id ,ci.destination_network_id,ci.destination_entity_type,ci.destination_port_no,
			ci.is_customer_connected,ci.created_on,ci.created_by,ci.approved_by,ci.approved_on,
			ci.source_entity_sub_type,ci.destination_entity_sub_type,
			ci.source_display_name,ci.destination_display_name
			from vw_connection_info_sld ci
			where ci.source_system_id=p_entity_system_id and ci.source_port_no in (SELECT regexp_split_to_table(p_entity_port_no, ',')::integer) and lower(ci.source_entity_type) = lower(p_entity_type)
			--and ((ci.source_entity_type::text)||(ci.source_system_id::text))!=((ci.destination_entity_type::text)||(ci.destination_system_id::text))
			union 
			select  cte.rowid+1 as rowid,
			cte.level || '-' || row_number() over (order by cte.level, cte.destination_port_no)::text as level, 
			ci.connection_id,cte.connection_id as parent_connection_id,ci.source_system_id,ci.source_network_id,ci.source_entity_type,ci.source_port_no,
			ci.destination_system_id ,ci.destination_network_id,ci.destination_entity_type,ci.destination_port_no,
			ci.is_customer_connected,ci.created_on,ci.created_by,ci.approved_by,			
			ci.approved_on,
			ci.source_entity_sub_type,ci.destination_entity_sub_type,
			ci.source_display_name,ci.destination_display_name
			from vw_connection_info_sld ci inner join DownStream cte on 
			ci.source_system_id=cte.destination_system_id and 
			ci.source_port_no =cte.destination_port_no and lower(ci.source_entity_type) = lower(cte.destination_entity_type)
			-- condition to get rid off from infinite loop in case of ring connectivity..
			where ((lower(ci.source_entity_type)||ci.source_system_id||ci.source_port_no) not in  (SELECT lower(p_entity_type)||p_entity_system_id||regexp_split_to_table(p_entity_port_no, ',')))
			-- and  
-- 			case 
-- 			when upper(ci.source_entity_type)='CUSTOMER' then 1=2
-- 			when upper(ci.source_entity_type)='SPLITTER' and coalesce(p_sld_type,'')!='' and upper(coalesce(ci.source_splitter_type,''))=upper(coalesce(p_sld_type,'')) then 1=2 else 1=1 end
-- 			and  case when upper(ci.source_entity_type)=upper('FMS') then 1=2 else 1=1 end
			and cte.rowid<=v_max_row_count
		) select * from DownStream
	 ) b
	) cinfo 
	left join layer_details ls on upper(cinfo.source_entity_type)=upper(ls.layer_name) and (ls.isvisible=true or ls.is_visible_in_ne_library=true) --and ls.is_virtual_port_allowed=false
	left join layer_details ld on upper(cinfo.destination_entity_type)=upper(ld.layer_name) and (ld.isvisible=true or ld.is_visible_in_ne_library=true) --and ld.is_virtual_port_allowed=false;
	left join point_master p1 on cinfo.source_system_id=p1.system_id and upper(cinfo.source_entity_type)=upper(p1.entity_type)
	left join point_master p2 on cinfo.destination_system_id=p2.system_id and upper(cinfo.destination_entity_type)=upper(p2.entity_type)
left join layer_details srcsubtype 
	on upper(cinfo.source_entity_sub_type)=upper(srcsubtype.layer_name) and (srcsubtype.isvisible=true or srcsubtype.is_visible_in_ne_library=true) 
	and srcsubtype.is_middleware_entity=true

	left join layer_details dstsubtype 
	on upper(cinfo.destination_entity_sub_type)=upper(dstsubtype.layer_name) and (dstsubtype.isvisible=true or dstsubtype.is_visible_in_ne_library=true) 
	and dstsubtype.is_middleware_entity=true;

	--********************* DECIDE DOWNSTREAM AND UPSTREAM ************************----

	-- IF SEARCHED ENTITY IS  CUSTOMER THEN DOWN STREAM END ALWAYS SHOULD BE B
	if(coalesce(v_down_stream_end,'')='' and upper(p_entity_type)='CUSTOMER') then
			v_down_stream_end:='B';
	 end if;

	if(coalesce(v_down_stream_end,'')='' and upper(p_entity_type)='FMS' and 
		(select count(1) from att_details_fms where system_id=p_entity_system_id and parent_entity_type in ('POD','MPOD'))>0) then
		v_down_stream_end:='B';
	end if;

-- CHECK wheather FMS TRACE END VALUE is A
	if(coalesce(v_down_stream_end,'')='' and (select count(1) from cpf_temp cinfo 
		where cinfo.trace_end='A' and ((upper(cinfo.source_entity_type)='FMS' and (upper(cinfo.source_network_id) like '%POD%' or upper(cinfo.source_network_id) like '%MOD%' )) 
		or (upper(cinfo.destination_entity_type)='FMS' and (upper(cinfo.destination_network_id) like '%POD%' or upper(cinfo.destination_network_id) like '%MOD%' )))
		and  upper(cinfo.source_entity_type)!=upper(cinfo.destination_entity_type))>0) then
			v_down_stream_end:='B';
	end if;

     -- CHECK wheather FMS TRACE END VALUE is B
	if(coalesce(v_down_stream_end,'')='' and (select count(1) from cpf_temp cinfo 
		where cinfo.trace_end='B' and ((upper(cinfo.source_entity_type)='FMS' and (upper(cinfo.source_network_id) like '%POD%' or upper(cinfo.source_network_id) like '%MOD%' )) 
		or (upper(cinfo.destination_entity_type)='FMS' and (upper(cinfo.destination_network_id) like '%POD%' or upper(cinfo.destination_network_id) like '%MOD%' )))
		and  upper(cinfo.source_entity_type)!=upper(cinfo.destination_entity_type))>0) then
			v_down_stream_end:='A';
	end if;

	-- CHECK ON WHICH END MULTILEVEL CHILDS ARE EXIST..
	if(coalesce(v_down_stream_end,'')='') then
		select max(cinfo.trace_end) into v_down_stream_end  from cpf_temp cinfo where upper(cinfo.source_entity_type)!=upper(cinfo.destination_entity_type)
		group by cinfo.rowid,cinfo.trace_end having count(1)>1 order by cinfo.trace_end limit 1;
	end if;

	--CHECK CUSTOMER IN A END OR B-END
	if(coalesce(v_down_stream_end,'')='') then
		select cinfo.trace_end into v_down_stream_end from cpf_temp cinfo where ((upper(cinfo.source_entity_type)='CUSTOMER') 
		or (upper(cinfo.destination_entity_type)='CUSTOMER')) limit 1;
	end if;

	-- IF NO CONDITION IS MATCHED THEN DEFAULT DOWNSTREAM END IS B...
	v_down_stream_end:= case when coalesce(v_down_stream_end,'')!='' then v_down_stream_end else 'B' end;

raise info '-%',v_down_stream_end;

	--*********************END************************---- v_arow_port
for v_arow_port in (SELECT regexp_split_to_table(p_entity_port_no, ',')::integer as port_number)
loop

if((select count(1) from cpf_temp ci where ci.trace_end='A' and upper(ci.source_entity_type)!=upper(ci.destination_entity_type) and ((ci.source_system_id=p_entity_system_id and ci.source_port_no =v_arow_port.port_number ) and lower(ci.source_entity_type) = lower(p_entity_type)) or (ci.destination_system_id=p_entity_system_id and ci.destination_port_no=v_arow_port.port_number and 
lower(ci.destination_entity_type) = lower(p_entity_type)))>1) 
then
	--Delete the B-End Data for RING  CONNECTIVITY... RETURN ONLY UPSTREAM ..
	delete from cpf_temp ci
	where ci.trace_end='B' and upper(ci.source_entity_type)!=upper(ci.destination_entity_type) and ((ci.source_system_id=p_entity_system_id and ci.source_port_no =v_arow_port.port_number ) and lower(ci.source_entity_type) = lower(p_entity_type))
	or (ci.destination_system_id=p_entity_system_id and ci.destination_port_no=v_arow_port.port_number and lower(ci.destination_entity_type) = lower(p_entity_type));			
end if;

end loop;

	return query
	select  ROW_NUMBER() OVER (order by cinfo.is_backward_path,cinfo.rno)::integer as id,cinfo.rowid,cinfo.level, cinfo.connection_id,cinfo.parent_connection_id,cinfo.source_system_id,
        cinfo.source_network_id,cinfo.source_entity_type,cinfo.source_entity_title,cinfo.source_port_no,cinfo.is_source_virtual,cinfo.destination_system_id,cinfo.destination_network_id,
        cinfo.destination_entity_type,cinfo.destination_entity_title,cinfo.destination_port_no,cinfo.is_destination_virtual,cinfo.is_customer_connected,cinfo.created_on,cinfo.created_by,
	cinfo.approved_by,cinfo.approved_on,
	cinfo.source_no_of_ports,cinfo.destination_no_of_ports,cinfo.is_backward_path,cinfo.trace_end,cinfo.source_display_name,cinfo.destination_display_name,cinfo.source_entity_category,cinfo.destination_entity_category 
	from (
		select  ROW_NUMBER() OVER (order by a.level) as rno, a.* ,true as is_backward_path from cpf_temp a where a.trace_end!=v_down_stream_end 
		union
		select   ROW_NUMBER() OVER (order by b.level) as rno, b.*,false as is_backward_path from cpf_temp b where b.trace_end=v_down_stream_end
	) cinfo 
	order by cinfo.is_backward_path desc,cinfo.rno;

END; 
$BODY$;



alter table isp_port_info add column link_system_id integer;

--------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_associate_fiber_link_cable(p_cable_id integer, p_link_system_id integer, p_fiber_number integer, p_action character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
 
DECLARE
rec record;
p_a_end_id integer;
p_b_end_id integer;
v_count integer;
v_fms_count integer;
BEGIN

v_fms_count:= 0;
 --TEMP TABLE TO STORE CONNECTION INFO RESULT..
create temp table cpf_temp_bkp
(
	id integer, 
	connection_id integer,
	rowid integer,
	parent_connection_id integer,
	source_system_id integer, 
	source_network_id character varying,
	source_entity_type character varying, 
	source_port_no integer, 
	is_source_virtual boolean,
	destination_system_id integer, 
	destination_network_id character varying,
	destination_entity_type character varying,
	destination_port_no integer, 
	is_destination_virtual boolean, 
	is_customer_connected boolean, 
	created_on timestamp without time zone, 
	created_by integer, 
	approved_by integer, 
	approved_on timestamp without time zone, 
	cable_calculated_length double precision,
	cable_measured_length double precision,
	splitter_ratio character varying,
	is_backward_path boolean,
	trace_end character varying,
	source_display_name character varying,
	destination_display_name character varying
	 
	) on commit drop;

	create temp table cpf_temp_route
	(
	route_id character varying,
	fiber_number integer
	) on commit drop;
	--select cable_measured_length from att_details_cable 
	
	insert into cpf_temp_bkp(id, connection_id,rowid,parent_connection_id,source_system_id, source_network_id,source_entity_type, source_port_no, is_source_virtual,destination_system_id, destination_network_id,
		destination_entity_type,destination_port_no, is_destination_virtual, is_customer_connected, created_on, created_by, approved_by, approved_on, cable_calculated_length ,
		cable_measured_length,splitter_ratio,is_backward_path,trace_end,source_display_name,destination_display_name)
		select a.id, a.connection_id,a.rowid,a.parent_connection_id,a.source_system_id, a.source_network_id,a.source_entity_type, a.source_port_no, a.is_source_virtual,a.destination_system_id, a.destination_network_id,a.destination_entity_type,a.destination_port_no, a.is_destination_virtual, a.is_customer_connected, a.created_on, a.created_by, a.approved_by, a.approved_on, a.cable_calculated_length ,
		a.cable_measured_length,a.splitter_ratio,a.is_backward_path,a.trace_end,a.source_display_name,a.destination_display_name from fn_get_connection_info('', '', 1, 10,null, null, p_cable_id, p_fiber_number,'Cable') a;

insert into cpf_temp_route(route_id,fiber_number)
select route_id,fiber_number
from att_details_cable_info inf
inner join 
att_details_cable cbl on cbl.system_id=inf.cable_id
where link_system_id=p_link_system_id
group by cbl.route_id,inf.fiber_number;

select id into p_a_end_id from cpf_temp_bkp where trace_end='A' and (lower(source_entity_type) in ('splitter','ont','fms') or(lower(destination_entity_type) 
in ('splitter','ont','fms'))) order by id limit 1;

select id into p_b_end_id from cpf_temp_bkp where trace_end='B' and (lower(source_entity_type) in ('splitter','ont','fms') or(lower(destination_entity_type) 
in ('splitter','ont','fms'))) order by id limit 1;


CREATE TEMP TABLE temp_cable_info AS
select  distinct a.* from (
select source_system_id as cable_system_id,source_network_id as cable_network_id,source_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='A'  and case when coalesce(p_a_end_id,0)>0 then id<p_a_end_id else true end and upper(source_entity_type)=upper('cable')
union
select destination_system_id as cable_system_id,destination_network_id as cable_network_id,destination_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='A'  and case when coalesce(p_a_end_id,0)>0 then id<p_a_end_id else true end and upper(destination_entity_type)=upper('Cable')
union
select source_system_id as cable_system_id,source_network_id as cable_network_id,source_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='B'  and case when coalesce(p_b_end_id,0)>0 then id<p_b_end_id else true end and upper(source_entity_type)=upper('cable')
union
select destination_system_id as cable_system_id,destination_network_id as cable_network_id,destination_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='B'  and case when coalesce(p_b_end_id,0)>0 then id<p_b_end_id else true end  and upper(destination_entity_type)=upper('Cable')
) a;
  IF EXISTS(SELECT 1 from ATT_DETAILS_CABLE_INFO where cable_id=p_cable_id)THEN
	IF(p_action='A')THEN 
		UPDATE ATT_DETAILS_CABLE_INFO set link_system_id=p_link_system_id where cable_id=p_cable_id and fiber_number=p_fiber_number;
		-- UPDATE FIBER LINK STATUS TO ASSOCIATE 
IF (SELECT COUNT(*) FROM cpf_temp_bkp) > 0 then

 UPDATE att_details_fiber_link
SET fiber_link_status = 'Associated',
    gis_length = ROUND(
        COALESCE(
            (SELECT SUM(ctb.cable_measured_length) FROM cpf_temp_bkp ctb), 0
        )::NUMERIC, 3
    ),
    total_route_length = ROUND(
        COALESCE(
            (SELECT SUM(ctb.cable_calculated_length) FROM cpf_temp_bkp ctb), 0
        )::NUMERIC, 3
    )
WHERE system_id = p_link_system_id;

else

  UPDATE att_details_fiber_link
SET fiber_link_status = 'Associated',
    gis_length = ROUND(
        (COALESCE(
            (SELECT cable_measured_length FROM att_details_cable WHERE system_id = p_cable_id), 0
        ) + COALESCE(gis_length, 0))::NUMERIC, 3
    ),
    total_route_length = ROUND(
        (COALESCE(
            (SELECT cable_calculated_length FROM att_details_cable WHERE system_id = p_cable_id), 0
        ) + COALESCE(total_route_length, 0))::NUMERIC, 3
    )
WHERE system_id = p_link_system_id;

           
           
END IF;

		UPDATE att_details_cable_info A SET link_system_id= p_link_system_id
		FROM temp_cable_info B WHERE  B.cable_system_id=A.cable_id and B.cable_fiber_number=A.fiber_number;
		drop table temp_cable_info;
	
	
	update isp_port_info set link_system_id =p_link_system_id
	where parent_system_id in(
	select source_system_id  from cpf_temp_bkp 
	where  upper(source_entity_type)=upper('FMS')
union
select destination_system_id  from cpf_temp_bkp where upper(destination_entity_type)=upper('FMS'))
	and parent_entity_type ='FMS'
and port_number =p_fiber_number;
	
	
	ELSIF(p_action='D')THEN
	raise info'Delete1%',p_link_system_id;
	select count(*) into v_count from att_details_cable_info where link_system_id=p_link_system_id;
	raise info'Delete1%',v_count;
		
		UPDATE ATT_DETAILS_CABLE_INFO set link_system_id=0 where cable_id=p_cable_id and fiber_number=p_fiber_number;
		
		UPDATE att_details_cable_info A SET link_system_id= 0
		FROM temp_cable_info B WHERE  B.cable_system_id=A.cable_id and B.cable_fiber_number=A.fiber_number;
		drop table temp_cable_info;
		IF NOT Exists(select 1 from att_details_cable_info where link_system_id=p_link_system_id)THEN
			UPDATE att_details_fiber_link set fiber_link_status='Free',gis_length = 0,total_route_length= 0 where system_id=p_link_system_id;
			raise info'Delete2%',45;
		END IF;
		 
		update isp_port_info set link_system_id =0
	where parent_system_id in(
	select source_system_id  from cpf_temp_bkp 
	where  upper(source_entity_type)=upper('FMS')
union
select destination_system_id  from cpf_temp_bkp where upper(destination_entity_type)=upper('FMS'))
	and parent_entity_type ='FMS'
and port_number =p_fiber_number;

	END IF;
drop table if exists cpf_temp_bkp;
drop table if exists cpf_temp_route;
drop table if exists cpf_temp;

		RETURN QUERY 
		SELECT TRUE AS status,'Success!'::CHARACTER VARYING AS message;
		RETURN;
	ELSE
		RETURN QUERY
		SELECT FALSE AS status,'FAIL!'::CHARACTER VARYING AS message;
		RETURN;
	END IF;
	  
END
$function$
;


