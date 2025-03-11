CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_path(
	p_linksystemid integer,
	p_userid integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
DECLARE 
v_Query text;
Query text; 
rec record;
BEGIN
	create temp table temp_connectedElement(
		connected_system_id integer, 
		connected_network_id character varying(100), 
		connected_port_no integer,
		connected_entity_type character varying(100),
		connected_entity_geom character varying ,
		connected_entity_category character varying(100),
		 is_virtual  boolean
	)on commit drop;
	
	 create temp table temp_cableInfo(
		link_system_id integer,
	        link_network_id character varying(100),
	        link_id character varying(100), 
	        link_name character varying(100), 
	        cable_system_id integer,
	        cable_network_id character varying(100),
	        cable_type character varying(100),
	        cable_geom character varying,
	        fiber_number integer,
	        core_status character varying(100),
	        core_comment character varying(100),
	        core_number character varying(10),
	        connected_system_id integer, 
		connected_network_id character varying(100), 
		connected_port_no integer,
		connected_entity_type character varying(100) 
	)on commit drop;
	 
	-- fetch linkInfo ..
	insert into temp_cableInfo(link_system_id,link_network_id,link_id,link_name,cable_system_id,cable_network_id,
	 cable_type, cable_geom, fiber_number,core_status,core_comment,core_number)
	 
	select distinct fl.system_id as link_system_id,fl.network_id as link_network_id,fl.link_id,fl.link_name,cbl.system_id as cable_system_id,cbl.network_id as cable_network_id,
	 cbl.cable_type, st_astext(lm.sp_geometry) as cable_geom, ci.fiber_number, psm.Status as core_status,coalesce(ci.core_comment,'') as core_comment, ci.core_number::character varying
	from att_details_cable_info ci
	inner join att_details_cable cbl ON ci.cable_id=cbl.system_id
	inner join att_details_fiber_link fl ON ci.link_system_id=fl.system_id
	left join line_master lm ON ci.cable_id=lm.system_id and upper(lm.entity_type)=upper('Cable')
	left join Port_status_master psm ON  (case when (ci.a_end_status_id > ci.b_end_status_id) then ci.a_end_status_id
						else  ci.b_end_status_id end)= psm.system_id
	WHERE ci.link_system_id=p_linkSystemId;
    

for rec in select * from temp_cableInfo where upper(core_status)=upper('Connected')
    loop
        IF EXISTS(select 1 from connection_info where source_system_id=rec.cable_system_id and source_network_id=rec.cable_network_id and upper(source_entity_type)=upper('Cable'))THEN
		insert into temp_connectedElement(connected_system_id,connected_network_id,connected_entity_type,connected_port_no,connected_entity_geom,connected_entity_category,is_virtual)
		select destination_system_id,destination_network_id,destination_entity_type,destination_port_no,st_astext(pm.sp_geometry) as connected_entity_geom,pm.entity_category as connected_entity_category,pm.is_virtual from connection_info ci
		inner join point_master pm ON upper(ci.destination_entity_type)= upper(pm.entity_type) and ci.destination_system_id=pm.system_id
		 
		where source_system_id=rec.cable_system_id and source_network_id=rec.cable_network_id and upper(source_entity_type)=upper('Cable');

		--update temp_cableInfo set connected_system_id=destination_system_id,connected_network_id=destination_network_id,connected_entity_type=destination_entity_type
		--,connected_port_no=destination_port_no where source_system_id=rec.cable_system_id and source_network_id=rec.cable_network_id and upper(source_entity_type)=upper('Cable');
        END IF;
        IF EXISTS(select 1 from connection_info where destination_system_id=rec.cable_system_id and destination_network_id=rec.cable_network_id and upper(destination_entity_type)=upper('Cable'))THEN
		insert into temp_connectedElement(connected_system_id,connected_network_id,connected_entity_type,connected_port_no,connected_entity_geom,connected_entity_category,is_virtual)
		select source_system_id,source_network_id,source_entity_type,source_port_no,st_astext(pm.sp_geometry) as connected_entity_geom,pm.entity_category as connected_entity_category,pm.is_virtual from connection_info ci
		inner join point_master pm ON upper(ci.source_entity_type)= upper(pm.entity_type) and ci.source_system_id=pm.system_id
		where destination_system_id=rec.cable_system_id and destination_network_id=rec.cable_network_id and upper(destination_entity_type)=upper('Cable');

		--update temp_cableInfo set connected_system_id=source_system_id,connected_network_id=source_network_id,connected_entity_type=source_entity_type
		--,connected_port_no=source_port_no where destination_system_id=rec.cable_system_id and destination_network_id=rec.cable_network_id and upper(destination_entity_type)=upper('Cable');
        END IF;

end loop;
   
	RETURN QUERY select row_to_json(result) 
	from (
		select (
			select array_to_json(array_agg(row_to_json(x)))
			from (
				select * from temp_cableInfo
				
			) x
		) as lstCableInfo,
		(
			select array_to_json(array_agg(row_to_json(x)))
			from (
				select distinct connected_system_id,connected_network_id,connected_entity_type,case when is_virtual_port_allowed=true then null else connected_port_no end as connected_port_no
				 ,connected_entity_geom,ld.is_virtual_port_allowed , connected_entity_category,is_virtual from temp_connectedElement temp
				inner join layer_details ld on upper(temp.connected_entity_type)=upper(ld.layer_name)
				
			) x
		) as lstConnectedElements
	)result;
 

END 
$BODY$;