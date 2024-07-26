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


IF(p_action='A') THEN

select count(*) into v_fms_count from cpf_temp_bkp where (lower(source_entity_type) = ('fms') or(lower(destination_entity_type) =('fms'))) and (source_system_id!=destination_system_id and upper(source_entity_type)!=upper(destination_entity_type));

raise info'v_fms_count%',v_fms_count;

	IF(v_fms_count <=1) THEN
	RETURN QUERY 
	SELECT FALSE AS status,'Fiber Link created but not associated as ODF to ODF connectivity not found!'::CHARACTER VARYING AS message;
	RETURN;
	END IF;
END IF;

IF(p_action='A') THEN

select count(*) into v_fms_count from cpf_temp_route;
raise info'v_fms_count-----%',v_fms_count;

	IF(v_fms_count>=2) THEN
	RETURN QUERY 
	SELECT FALSE AS status,'Same link cannot be associated with more than 2 fibers!'::CHARACTER VARYING AS message;
	RETURN;
	END IF;
END IF;


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

    UPDATE att_details_fiber_link SET  fiber_link_status = 'Associated',gis_length = COALESCE(
            (SELECT SUM(ctb.cable_measured_length) FROM cpf_temp_bkp ctb), 0 ), total_route_length = COALESCE(
            (SELECT SUM(ctb.cable_calculated_length) FROM cpf_temp_bkp ctb), 0) WHERE system_id = p_link_system_id;
else

   UPDATE att_details_fiber_link SET fiber_link_status = 'Associated',
    gis_length = COALESCE(
        (SELECT cable_measured_length FROM att_details_cable WHERE system_id = p_cable_id), 0) + COALESCE(gis_length, 0),
    total_route_length = COALESCE(
        (SELECT cable_calculated_length FROM att_details_cable WHERE system_id = p_cable_id), 0) + COALESCE(total_route_length, 0)
   WHERE  system_id = p_link_system_id;
           
           
END IF;

		UPDATE att_details_cable_info A SET link_system_id= p_link_system_id
		FROM temp_cable_info B WHERE  B.cable_system_id=A.cable_id and B.cable_fiber_number=A.fiber_number;
		drop table temp_cable_info;
	
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
		 
	END IF;

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

CREATE OR REPLACE FUNCTION public.fn_get_export_cable_info_bylinkid(p_linkid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$ 
BEGIN

return query
select row_to_json(row) from(
select distinct cable.network_id,cable.cable_name,cable.total_core,cable.no_of_tube,fn_get_display_name(cable.a_system_id, cable.a_entity_type) AS a_location,
fn_get_display_name(cable.b_system_id, cable.b_entity_type) AS b_location,cableinfo.fiber_number from att_details_cable_info as cableinfo 
inner join att_details_cable as cable on  cableinfo.cable_id= cable.system_id  where cableinfo.link_system_id=p_LinkId
)row;


END ;
$function$
;