-- FUNCTION: public.fn_get_connection_info_path(character varying, character varying, integer, integer, character varying, character varying, integer, integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_connection_info_path(character varying, character varying, integer, integer, character varying, character varying, integer, integer, character varying);

update layer_details set is_visible_in_ne_library=true where layer_name='PatchCord';
CREATE OR REPLACE FUNCTION public.fn_get_connection_info_path(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_entity_system_id integer,
	p_entity_port_no integer,
	p_entity_type character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN

	create temp table cpf_temp_result
	(
	id serial,	
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
	cable_calculated_length double precision,
	cable_measured_length double precision,
	splitter_ratio character varying(100),	
	is_backward_path boolean,
	source_display_name character varying,
	destination_display_name character varying,	
	source_tray_system_id integer ,
	destination_tray_system_id integer, 
	source_tray_display_name character varying, 
	destination_tray_display_name character varying,
	cable_network_status character varying,
	source_port_display_name character varying, 
	destination_port_display_name character varying
	) on commit drop;

	create temp table cpf_final_result
	(	
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
	cable_calculated_length double precision,
	cable_measured_length double precision,
	splitter_ratio character varying(100),	
	is_backward_path boolean,
	source_display_name character varying,
	destination_display_name character varying,
	source_tray_system_id integer ,
	destination_tray_system_id integer, 
	source_tray_display_name character varying, 
	destination_tray_display_name character varying,
	cable_network_status character varying,
	source_port_display_name character varying, 
	destination_port_display_name character varying,
	source_cable_calculated_length double precision,
	destination_cable_calculated_length double precision	
	) on commit drop;	

 	
	insert into cpf_temp_result(rowid,connection_id,parent_connection_id,source_system_id,source_network_id,source_entity_type,source_entity_title,
	source_port_no,is_source_virtual,destination_system_id,destination_network_id,destination_entity_type,destination_entity_title,
	destination_port_no,is_destination_virtual,is_customer_connected,created_on,created_by,approved_by,approved_on,trace_end,
	cable_calculated_length,cable_measured_length,splitter_ratio,is_backward_path,source_display_name,destination_display_name,source_tray_system_id,destination_tray_system_id,
	source_tray_display_name,destination_tray_display_name,cable_network_status,source_port_display_name,destination_port_display_name)
	
	select  a.rowid,a.connection_id,a.parent_connection_id,a.source_system_id,a.source_network_id,a.source_entity_type,a.source_entity_title,
	a.source_port_no,coalesce(a.is_source_virtual,false) as is_source_virtual ,a.destination_system_id,a.destination_network_id,a.destination_entity_type,
	a.destination_entity_title,a.destination_port_no,coalesce(a.is_destination_virtual, false) as is_destination_virtual,a.is_customer_connected,a.created_on,a.created_by,
	a.approved_by,a.approved_on,a.trace_end,a.cable_calculated_length,a.cable_measured_length,a.splitter_ratio,a.is_backward_path,a.source_display_name,
	a.destination_display_name,
	a.source_tray_system_id,a.destination_tray_system_id,a.source_tray_display_name,a.destination_tray_display_name,a.cable_network_status,
	fn_get_port_text(a.source_system_id,coalesce(a.is_source_virtual,false),a.source_port_no,a.source_entity_type),fn_get_port_text(a.destination_system_id,a.is_destination_virtual,a.destination_port_no,a.destination_entity_type) from fn_get_connection_info(p_searchby, p_searchtext, p_pageno, p_pagerecord,p_sortcolname, p_sorttype,p_entity_system_id, p_entity_port_no,p_entity_type)a;

insert into cpf_final_result(rowid,connection_id,parent_connection_id,source_system_id,source_network_id,source_entity_type,source_entity_title,source_port_no,is_source_virtual,
	destination_system_id,destination_network_id,destination_entity_type,destination_entity_title,destination_port_no,is_destination_virtual,is_customer_connected,
	created_on,created_by,approved_by,approved_on,trace_end,cable_calculated_length,cable_measured_length,splitter_ratio,is_backward_path,
	source_display_name,destination_display_name,source_tray_system_id,destination_tray_system_id,source_tray_display_name,
	destination_tray_display_name,cable_network_status,source_port_display_name,destination_port_display_name,source_cable_calculated_length,destination_cable_calculated_length)
	select  a.rowid,a.connection_id,a.parent_connection_id,a.source_system_id,a.source_network_id,
	a.source_entity_type,
	a.source_entity_title,a.source_port_no,coalesce(a.is_source_virtual,false),
	a.destination_system_id,a.destination_network_id,
	a.destination_entity_type,
	a.destination_entity_title,a.destination_port_no,coalesce(a.is_destination_virtual,false),
	a.is_customer_connected,a.created_on,a.created_by,a.approved_by,a.approved_on,a.trace_end,a.cable_calculated_length,a.cable_measured_length,
	a.splitter_ratio,a.is_backward_path,
	(case when 
	upper(a.source_entity_type)='EQUIPMENT' 
	then fn_get_display_name(coalesce((select rack_id from att_details_model where system_id=a.source_system_id),0),'Rack')||'/'||
	(select concat(a.source_display_name,'-(',m.no_of_port,')','-(',m.network_status,')') 
	from att_details_model m where system_id=a.source_system_id) 
	else 
	
		CASE WHEN UPPER(A.SOURCE_ENTITY_TYPE)='CABLE' 
		THEN CONCAT(A.SOURCE_DISPLAY_NAME,'-(',LEFTCBL.TOTAL_CORE,'F)','-(',LEFTCBL.NETWORK_STATUS,')')
		ELSE
		--WHEN A.IS_SOURCE_VIRTUAL=false then
			CONCAT(A.SOURCE_DISPLAY_NAME,(CASE WHEN COALESCE(P1.NO_OF_PORTS,'')!='' AND UPPER(A.SOURCE_ENTITY_TYPE) NOT IN('CUSTOMER')  THEN '-('||P1.NO_OF_PORTS||')' END),
			(CASE WHEN COALESCE(P1.NETWORK_STATUS,'')!='' AND UPPER(A.SOURCE_ENTITY_TYPE) NOT IN('CUSTOMER') THEN '-('||P1.NETWORK_STATUS||')' END))
		--ELSE
		--	A.SOURCE_DISPLAY_NAME
		END 
	end),
	(case when upper(a.destination_entity_type)='EQUIPMENT' 
	then fn_get_display_name(coalesce((select rack_id from att_details_model where system_id=a.destination_system_id),0),'Rack')||'/'||
	(select concat(a.destination_display_name,'-(',m.no_of_port,')','-(',m.network_status,')') 
	from att_details_model m where system_id=a.destination_system_id) 
	else 
		CASE WHEN UPPER(A.DESTINATION_ENTITY_TYPE)='CABLE' 
		THEN CONCAT(A.DESTINATION_DISPLAY_NAME,'-(',RIGHTCBL.TOTAL_CORE,'F)','-(',RIGHTCBL.NETWORK_STATUS,')')
		ELSE
		--WHEN A.IS_DESTINATION_VIRTUAL=false then
			CONCAT(A.DESTINATION_DISPLAY_NAME,
			(CASE WHEN COALESCE(P2.NO_OF_PORTS,'')!='' AND UPPER(A.DESTINATION_ENTITY_TYPE) NOT IN('CUSTOMER') THEN '-('||P2.NO_OF_PORTS||')' END),
			(CASE WHEN COALESCE(P2.NETWORK_STATUS,'')!='' AND UPPER(A.DESTINATION_ENTITY_TYPE) NOT IN('CUSTOMER')  THEN '-('||P2.NETWORK_STATUS||')' END))
		--ELSE
		--	A.DESTINATION_DISPLAY_NAME
		END
	end),
	a.source_tray_system_id,a.destination_tray_system_id,
	a.source_tray_display_name,a.destination_tray_display_name,a.cable_network_status,
	fn_get_port_text(a.source_system_id,coalesce(a.is_source_virtual,false),a.source_port_no,a.source_entity_type),
	fn_get_port_text(a.destination_system_id,a.is_destination_virtual,a.destination_port_no,a.destination_entity_type),LEFTCBL.cable_calculated_length,RIGHTCBL.cable_calculated_length
	from cpf_temp_result a
	left join point_master p1 on a.source_system_id=p1.system_id and upper(a.source_entity_type)=upper(p1.entity_type)
	left join point_master p2 on a.destination_system_id=p2.system_id and upper(a.destination_entity_type)=upper(p2.entity_type) 
	LEFT JOIN ATT_DETAILS_CABLE LEFTCBL ON LEFTCBL.SYSTEM_ID=A.SOURCE_SYSTEM_ID AND UPPER(A.SOURCE_ENTITY_TYPE)=UPPER('CABLE')
	LEFT JOIN ATT_DETAILS_CABLE RIGHTCBL ON RIGHTCBL.SYSTEM_ID=A.DESTINATION_SYSTEM_ID AND UPPER(A.DESTINATION_ENTITY_TYPE)=UPPER('CABLE') order by a.id;

return query
	select row_to_json(result) 
from (select (select array_to_json(array_agg(row_to_json(x)))from (select * from cpf_final_result) x) as lstConnectionInfo
)result	;
END; 
$BODY$;

ALTER FUNCTION public.fn_get_connection_info_path(character varying, character varying, integer, integer, character varying, character varying, integer, integer, character varying)
    OWNER TO postgres;

GRANT EXECUTE ON FUNCTION public.fn_get_connection_info_path(character varying, character varying, integer, integer, character varying, character varying, integer, integer, character varying) TO PUBLIC;


GRANT EXECUTE ON FUNCTION public.fn_get_connection_info_path(character varying, character varying, integer, integer, character varying, character varying, integer, integer, character varying) TO postgres;

