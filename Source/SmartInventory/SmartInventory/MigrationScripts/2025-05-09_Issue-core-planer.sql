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
v_connection record;
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
	
	ELSIF(p_action='D')
	THEN
	
		/*for v_connection in (select * from connection_info where (source_system_id=p_cable_id and source_entity_type='Cable' and destination_entity_type='FMS' and source_port_no=p_fiber_number)
	or (destination_system_id=p_cable_id and destination_entity_type='Cable' and source_entity_type='FMS' and destination_port_no=p_fiber_number))  
	loop
		if(v_connection.destination_entity_type='FMS')
		then
				update isp_port_info set link_system_id=0 where	parent_system_id=v_connection.destination_system_id 
				and parent_entity_type='FMS'
				and input_output='O' and port_number=v_connection.destination_port_no;
		end if;
		
		if(v_connection.source_entity_type='FMS')
		then
				update isp_port_info set link_system_id=0 where	parent_system_id=v_connection.source_system_id 
				and parent_entity_type='FMS'
				and input_output='O' and port_number=v_connection.source_port_no;
		end if;
	end loop;*/
	
	raise info'Delete1%',p_link_system_id;
	select count(*) into v_count from att_details_cable_info where link_system_id=p_link_system_id;
	raise info'Delete1%',v_count;
		
		UPDATE ATT_DETAILS_CABLE_INFO set link_system_id=0 where cable_id=p_cable_id 
	    and fiber_number=p_fiber_number;
		
		update isp_port_info info set link_system_id=0 
		from cpf_temp_bkp ctb where	parent_system_id=ctb.destination_system_id 
				and ctb.destination_entity_type='FMS' and parent_entity_type='FMS'
				and input_output='O' and port_number=ctb.destination_port_no;
				
				update isp_port_info set link_system_id=0 
				from cpf_temp_bkp ctb  where	parent_system_id=ctb.source_system_id 
				and ctb.source_entity_type='FMS' and parent_entity_type='FMS'
				and input_output='O' and port_number=ctb.source_port_no;
				
		/*UPDATE att_details_cable_info A SET link_system_id= 0
		FROM temp_cable_info B WHERE  B.cable_system_id=A.cable_id and B.cable_fiber_number=A.fiber_number;*/
			
		drop table temp_cable_info;
		IF NOT Exists(select 1 from att_details_cable_info where link_system_id=p_link_system_id)THEN
			UPDATE att_details_fiber_link set fiber_link_status='Free',gis_length = 0,total_route_length= 0 where system_id=p_link_system_id;
			raise info'Delete2%',45;
		END IF;
		 
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

-- Permissions

ALTER FUNCTION public.fn_associate_fiber_link_cable(int4, int4, int4, varchar) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_associate_fiber_link_cable(int4, int4, int4, varchar) TO public;
GRANT ALL ON FUNCTION public.fn_associate_fiber_link_cable(int4, int4, int4, varchar) TO postgres;


---------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_path(p_linksystemid integer, p_userid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
 
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


	 CREATE TEMP TABLE temp_connection_info(		
		source_system_id integer, 
		source_network_id varchar(100), 
		source_port_no integer,
		source_entity_type varchar(100),
		destination_system_id integer, 
		destination_network_id varchar(100), 
		destination_port_no integer,
		destination_entity_type varchar(100)  
	) ON COMMIT DROP;
	
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

    insert into temp_connection_info(source_system_id ,source_network_id,source_port_no,source_entity_type,destination_system_id, destination_network_id,destination_port_no,destination_entity_type)
select source_system_id ,source_network_id,source_port_no,source_entity_type,destination_system_id, destination_network_id,destination_port_no,destination_entity_type  
from connection_info c
inner join temp_cableInfo b on (c.destination_system_id=b.cable_system_id and destination_entity_type='Cable' and b.fiber_number=c.destination_port_no)
or (c.source_system_id=b.cable_system_id and source_entity_type='Cable' and b.fiber_number=c.source_port_no);


with source as(
SELECT
		ci.destination_system_id,
		ci.destination_network_id,		
		ci.destination_entity_type,
		ci.source_port_no
	FROM temp_cableInfo tci
	JOIN temp_connection_info ci 
		ON ci.source_system_id = tci.cable_system_id		
		AND UPPER(ci.source_entity_type) = 'CABLE'
		and tci.fiber_number=ci.source_port_no
		--where tci.core_status='Connected'
),
 dest as(
SELECT
		ci.source_system_id,
		ci.source_network_id,		
		ci.source_entity_type,
		ci.destination_port_no
	FROM temp_cableInfo tci
	JOIN temp_connection_info ci 
		ON ci.destination_system_id = tci.cable_system_id		
		AND UPPER(ci.destination_entity_type) = 'CABLE'
		and tci.fiber_number=ci.destination_port_no
		--where tci.core_status='Connected'
)
INSERT INTO temp_connectedElement(connected_system_id,connected_network_id,connected_entity_type,connected_entity_geom,connected_entity_category,is_virtual,connected_port_no)
select destination_system_id, destination_network_id, destination_entity_type
,ST_AsText(pm.sp_geometry), pm.entity_category, pm.is_virtual,source_port_no
from source A
inner JOIN point_master pm  ON A.destination_system_id = pm.system_id AND UPPER(A.destination_entity_type) = UPPER(pm.entity_type)
union
select source_system_id, source_network_id, source_entity_type
,ST_AsText(pm.sp_geometry), pm.entity_category, pm.is_virtual, destination_port_no
from dest B
inner JOIN point_master pm  ON b.source_system_id = pm.system_id AND UPPER(b.source_entity_type) = UPPER(pm.entity_type);

   
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
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_fiber_link_path(int4, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_fiber_link_path(int4, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_fiber_link_path(int4, int4) TO postgres;

---------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_fiberlink_schematicview(p_link_system_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
 
DECLARE 
v_Query text;
Query text; 
rec record;
v_from_id integer;
v_from_entity_type character varying;
nodes text;
edges text;
nodesDownstream text;
edgesDownstream text;
legends text;
cables text;
BEGIN
--truncate table temp_edges;
--truncate table cpf_temp_result;
--truncate table temp_nodes;

 create temp table cpf_temp_result
 (
 id serial,
 source_system_id integer,
 source_network_id character varying,
 source_port_no integer, 
 source_entity_type character varying,
 source_entity_title character varying,
 destination_system_id integer,
 destination_network_id character varying,
 destination_port_no integer,
 destination_entity_type character varying,
 destination_entity_title character varying,
 via_system_id integer,
 via_network_id character varying,
 via_entity_type character varying,
 via_port_no integer
 )on commit drop;
 
 create temp table temp_nodes
 (
 id serial,
 source_system_id integer,
 source_network_id character varying,
 connected_ports character varying, 
 source_entity_type character varying,
 source_entity_title character varying,
 node_name 	character varying
 )on commit drop;
 
 create temp table temp_edges
 (
 id serial,
 from_id integer,
 from_entity_type character varying,
 from_network_id character varying,
 to_id integer,
 to_network_id character varying,
 to_entity_type character varying,
 label character varying,
 via_ports character varying,
 network_status character varying,
 cable_type character varying,
 color_code character varying,
 total_core integer,
 cable_category character varying,
 	cable_calculated_length double precision
 )on commit drop;

insert into cpf_temp_result(source_system_id ,source_network_id,source_port_no , source_entity_type 
,destination_system_id ,destination_network_id ,destination_port_no ,destination_entity_type,
via_system_id ,via_network_id ,via_port_no ,via_entity_type)
select c1.source_system_id ,c1.source_network_id,c1.source_port_no , c1.source_entity_type 
,c2.destination_system_id ,c2.destination_network_id ,c2.destination_port_no ,c2.destination_entity_type,
c1.destination_system_id,c1.destination_network_id,c1.destination_port_no ,c1.destination_entity_type
from att_details_cable_info ci
inner join connection_info c1 on ci.cable_id=c1.destination_system_id and c1.destination_entity_type='Cable' and c1.destination_port_no=ci.fiber_number
left join connection_info c2 on c2.source_system_id=c1.destination_system_id and c2.source_entity_type=c1.destination_entity_type and c2.source_port_no=c1.destination_port_no
where ci.link_system_id=p_link_system_id
union
select c1.source_system_id ,c1.source_network_id,c1.source_port_no , c1.source_entity_type 
,c2.destination_system_id ,c2.destination_network_id ,c2.destination_port_no ,c2.destination_entity_type,
c1.destination_system_id,c1.destination_network_id,c1.destination_port_no ,c1.destination_entity_type
from att_details_cable_info ci
inner join connection_info c1 on ci.cable_id=c1.destination_system_id and c1.destination_entity_type='Cable' and c1.destination_port_no=ci.fiber_number
and c1.source_entity_type='FMS'
left join connection_info c2 on c2.source_system_id=c1.destination_system_id 
and c2.source_entity_type=c1.source_entity_type and c2.source_port_no=c1.destination_port_no
where ci.link_system_id=p_link_system_id;

insert into cpf_temp_result(source_system_id ,source_network_id,source_port_no , source_entity_type 
,destination_system_id ,destination_network_id ,destination_port_no ,destination_entity_type,
via_system_id ,via_network_id ,via_port_no ,via_entity_type)
select  pm.system_id ,pm.common_name,null , coalesce(pm.entity_type) 
,pm2.system_id ,pm2.common_name ,null ,coalesce(pm2.entity_type) ,
lm.system_id,lm.common_name,ci.fiber_number ,lm.entity_type
from att_details_cable_info ci
inner join line_master lm on ci.cable_id=lm.system_id and lm.entity_type='Cable'
left join point_master pm on st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2)) and pm.entity_type in('SpliceClosure','FMS')
left join point_master pm2 on st_within(pm2.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2))
and pm2.entity_type in('SpliceClosure','FMS')
where ci.link_system_id=p_link_system_id and ci.cable_id not in(select via_system_id from cpf_temp_result);

update cpf_temp_result set source_system_id=a.system_id,
source_entity_type=a.entity_type,
source_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on (st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2))
or st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2)))
and pm.entity_type in('SpliceClosure')
where a.source_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and source_system_id is null;

update cpf_temp_result set destination_system_id=a.system_id,
destination_entity_type=a.entity_type,
destination_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id
from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on (st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2))
or st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2)))
and pm.entity_type in('SpliceClosure')
where a.destination_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and destination_system_id is null;

delete from cpf_temp_result where source_network_id=destination_network_id;

 --delete from cpf_temp_result where id in(
--select B.id from cpf_temp_result A
	-- inner join cpf_temp_result b on A.source_system_id=b.destination_system_id
	 --and A.source_entity_type=b.destination_entity_type and a.via_system_id=b.via_system_id);
	

WITH duplicates1 AS (
    SELECT id FROM (
        SELECT id, 
               ROW_NUMBER() OVER (PARTITION BY source_system_id , source_entity_type ,destination_system_id ,destination_entity_type 
,via_network_id,via_port_no
								  ORDER BY id) AS row_num
        FROM cpf_temp_result
    ) subquery
    WHERE row_num > 1
)
delete from cpf_temp_result where id in(select id from duplicates1);

WITH duplicates2 AS (
    SELECT id FROM (
        SELECT id, source_network_id ,source_port_no,via_network_id,via_port_no,
               ROW_NUMBER() OVER (PARTITION BY source_network_id ,source_port_no
,via_network_id,via_port_no
								  ORDER BY id) AS row_num
        FROM cpf_temp_result where destination_system_id is null
    ) subquery
    WHERE row_num = 1
)
delete from cpf_temp_result where id in(select id from duplicates2);

insert into temp_nodes(source_system_id,source_network_id,source_entity_type,connected_ports)
select source_system_id,source_network_id,source_entity_type,string_agg(source_port_no::text,',')  from(
select * from(
select source_system_id,source_network_id,source_entity_type,source_port_no 
	from cpf_temp_result 
union
select destination_system_id,destination_network_id,destination_entity_type,destination_port_no 
	from cpf_temp_result)b
	order by b.source_port_no
)t group by source_system_id,source_network_id,source_entity_type;

update temp_nodes set node_name=fms.fms_name
from att_details_fms fms where source_system_id=fms.system_id and source_entity_type='FMS';

update temp_nodes set node_name=fms.spliceclosure_name
from att_details_spliceclosure fms where source_system_id=fms.system_id and source_entity_type='SpliceClosure';

insert into temp_edges(from_id,from_entity_type,
to_id,to_entity_type,label,via_ports,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length)

	select source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,string_agg(via_port_no::text,','),
network_status,cable_type,color_code,total_core,cable_category,
cable_calculated_length
from(
	select coalesce(source_system_id,cbl.a_system_id)as source_system_id, 
	coalesce(source_entity_type,a_entity_type) as source_entity_type,
coalesce(destination_system_id,b_system_id) as destination_system_id,
coalesce(destination_entity_type,b_entity_type) as destination_entity_type
,via_network_id,via_port_no,
cbl.network_status,cbl.cable_type,cbl.total_core,cbl.cable_category,cbls.color_code,
Cast(cbl.cable_calculated_length as decimal(10,2))  as cable_calculated_length
from cpf_temp_result cpf
inner join att_details_cable cbl on cbl.system_id=cpf.via_system_id
left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type 
and cbls.cable_category=cbl.cable_category and cbls.fiber_count=cbl.total_core
order by via_port_no
)a
group by source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length;

update temp_edges set via_ports=b.via_ports
from
(
select label,string_agg(via_ports,',') as via_ports from (select * from temp_edges order by via_ports)a group by label)
b where temp_edges.label=b.label;

WITH duplicates AS (
    SELECT id FROM (
        SELECT id, 
               ROW_NUMBER() OVER (PARTITION BY label
								  ORDER BY id) AS row_num
        FROM temp_edges
    ) subquery
    WHERE row_num > 1
)
delete from temp_edges where id in(select id from duplicates);

   
   -- NODES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select * from(
select row_number() over(partition by id) rn,* from(
select distinct * from (
select CONCAT(source_system_id,source_entity_type) as id,
CONCAT('<b>',replace(node_name,E'\t',''),'</b>','\n',case when ld.is_virtual_port_allowed=false then concat('(',connected_ports,')') end) as label

,source_entity_type as group, null as title from temp_nodes t
	inner join layer_details ld on ld.layer_name=t.source_entity_type where source_entity_type!='Splitter'

) a)b)b where rn=1
) x) into nodesDownstream;

-- EDGES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select CONCAT(t.from_id,t.from_entity_type) as from,
CONCAT(t.to_id,t.to_entity_type) as to,'<b>'
||(case when t.total_core is not null then '('||t.total_core||'F)' else '' end)||
concat(' (',t.cable_calculated_length::text,' m)')||
concat(' (',t.via_ports,')')||
(CASE WHEN t.via_ports!='' THEN '\n'||'\n' else '' end) as label,

case WHEN t.color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '{"color": "#FF0000"}'::json
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '{"color": "#0000FF"}'::json
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '{"color": "#FD10FD"}'::json
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '{"color": "#000"}'::json
ELSE '{"color": "#0000FF"}'::json
END
ELSE '{"color": "#000"}'::json
END as color,
0 as length,
CASE
WHEN upper(t.network_status::text) = 'P'::text THEN true
WHEN upper(t.network_status::text) = 'A'::text THEN false
ELSE true
END as dashes,
1.5  as width,
concat('{"type": "''curvedCCW''", "roundness":"0.5"}')::json as smooth
from temp_edges t
) x) into edgesDownstream;
   
   --LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (

select distinct * from (
select source_entity_type as entity_type, 
(select layer_title from layer_details where layer_name=source_entity_type) as entity_title,false as upstream 
from temp_nodes
) a where a.entity_type not in ('Cable','Splitter') order by entity_title
) x) into legends;

--CABLE LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
Select case WHEN color_code IS NULL THEN
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
(case when total_core is not null then '('||t.total_core||'F)' else '' end))as text,false as upstream
from(
select color_code,cable_category,cable_type,total_core from temp_edges
group by color_code,cable_category,cable_type,total_core )t where t.cable_type is not null
) x) into cables;
   
	return query select row_to_json(result) from (
select '' as entityType, '' as entityTitle,'' as entityDisplayText,
0 as latitude,0 as longitude,
now(),null as nodes,null as edges,nodesDownstream,edgesDownstream,legends,cables
) result;
 

END 
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_fiberlink_schematicview(int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_fiberlink_schematicview(int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_fiberlink_schematicview(int4) TO postgres;

------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_splicing_delete_connection(p_listconnection text, p_userid integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE geo_data text;
v_connection_info json;
curgeommapping refcursor;  
  v_source_system_id integer;  
  v_source_entity_type character varying(100);
  v_source_port_no integer;
  v_destination_system_id integer;
  v_destination_entity_type character varying(100);
  v_destination_port_no integer;
  v_is_left_cable_start_point boolean NOT NULL DEFAULT false;
  v_is_right_cable_start_point boolean NOT NULL DEFAULT false;
  v_is_source_cable_a_end boolean;
  v_is_destination_cable_a_end boolean;
  v_equipment_system_id integer;
  v_equipment_entity_type character varying(100);  
  v_source_network_id character varying;
  v_isvalid boolean;
  v_message character varying;
  v_count integer;
  v_isvalid_request boolean;
   v_fms_mapping record;
      destionation_linksystemId integer;
   source_linksystemId integer;
BEGIN
v_isvalid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_062]';--Connection has deleted successfully!
v_count:=0;

raise info 'p_listconnection %',p_listconnection;

CREATE temp TABLE temp_port_status_table 
(  
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
  equipment_tray_system_id integer,
  created_by integer,
  splicing_source character varying(100),
  source_entity_sub_type character varying,
  destination_entity_sub_type character varying,
  is_through_connection BOOLEAN DEFAULT FALSE
) ON COMMIT DROP ;

CREATE temp TABLE temp_connection2
(  
  source_system_id integer,  
  source_entity_type character varying(100),
  source_port_no integer,
  destination_system_id integer, 
  destination_entity_type character varying(100),
  destination_port_no integer,  
  is_source_cable_a_end boolean NOT NULL DEFAULT false,
  is_destination_cable_a_end boolean NOT NULL DEFAULT false,
  equipment_system_id  integer,  
  equipment_entity_type character varying(100)
) ON COMMIT DROP ;

OPEN curgeommapping FOR select source_system_id,source_entity_type,source_port_no,destination_system_id,destination_entity_type,
destination_port_no,equipment_system_id,equipment_entity_type,is_source_cable_a_end,is_destination_cable_a_end 
from json_populate_recordset(null::temp_connection2,replace(p_listconnection,'\','')::json);
	LOOP
		FETCH  curgeommapping into v_source_system_id,v_source_entity_type,v_source_port_no,v_destination_system_id,v_destination_entity_type,v_destination_port_no,v_equipment_system_id,v_equipment_entity_type,v_is_source_cable_a_end,v_is_destination_cable_a_end;
		-- EXIT FOR LOOP
		IF NOT FOUND THEN
		EXIT;
		END IF;

 -- check link_system_id given cable_id 
  /*  select distinct link_system_id into destionation_linksystemId from att_details_cable_info 
    where cable_id = v_destination_system_id and v_destination_entity_type='Cable';
	
	  select distinct link_system_id into source_linksystemId from att_details_cable_info 
    where cable_id = v_source_system_id and v_source_entity_type='Cable';
	
update isp_port_info set link_system_id=0 
where parent_system_id=v_source_system_id
and parent_entity_type=v_source_entity_type
and port_number=v_source_port_no;

update isp_port_info set link_system_id=0 
where parent_system_id=v_destination_system_id
and parent_entity_type=v_destination_entity_type
and port_number=v_destination_port_no;

update att_details_cable_info set link_system_id=0 
where cable_id=v_source_system_id
and v_source_entity_type='Cable'
and fiber_number=v_source_port_no;

update att_details_cable_info set link_system_id=0 
where cable_id=v_destination_system_id
and v_destination_entity_type='Cable'
and fiber_number=v_destination_port_no;

  
	if not exists (SELECT 1 FROM att_details_cable_info WHERE link_system_id= destionation_linksystemId)
    then
	-- update fiber_link_status free when no fiberlink attach at any cable 
    update att_details_fiber_link set fiber_link_status ='Free' 
    where system_id = destionation_linksystemId
    and fiber_link_status ='Associated'; 	
	end if;
	
	if not exists (SELECT 1 FROM att_details_cable_info WHERE link_system_id= source_linksystemId)
    then
	-- update fiber_link_status free when no fiberlink attach at any cable 
    update att_details_fiber_link set fiber_link_status ='Free' 
    where system_id = source_linksystemId
    and fiber_link_status ='Associated'; 	
	end if;
	*/
	
			v_isvalid_request:=true;
			
			select count(1) into v_count from (
			select * FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))
			union
			--PARENT CONNECTOR TO CABLE
			select * FROM CONNECTION_INFO 
			WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))
			) a;

		--if(upper(v_source_entity_type)!=upper('equipment') and v_count=2 )
		if(upper(v_source_entity_type)!=upper('equipment')  )
		then

INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO  WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
       
            -- --CABLE TO PARENT CONNECTOR
			DELETE FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
		
        
            INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
        
        --PARENT CONNECTOR TO CABLE
			DELETE FROM CONNECTION_INFO 
			WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
		elsif(upper(v_source_entity_type)=upper('EQUIPMENT') and upper(V_DESTINATION_ENTITY_TYPE)=upper('EQUIPMENT'))
		then
        
        INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE
     ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			or (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)); 

			-- --CABLE TO PARENT CONNECTOR
			DELETE FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			or (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)); 
			
             INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)
			or (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false));
            
			--PARENT CONNECTOR TO CABLE
			DELETE FROM CONNECTION_INFO 
			WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)
			or (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false));
		else 	
			v_isvalid_request:=false;
			v_message:='[SI_GBL_GBL_GBL_GBL_063]';--Invalid delete request!
			insert into temp_invalid_delete_request(request,created_on) values(p_listconnection,now());		
		end if;

if(v_isvalid_request)
then
		--CHECK THE SPLITTER CONNECTION IN DESTINATION
		if(upper(v_source_entity_type)='SPLITTER' or upper(v_source_entity_type)='ONT')
		then
     
     INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));



		       DELETE FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
			
			
			--SPLITTER TO SPLITTER or ONT TO ONT
			if((select count(1) from connection_info where (source_system_id=v_source_system_id and 
			upper(source_entity_type)=upper(v_source_entity_type) and upper(destination_entity_type)!=upper(v_source_entity_type))
			 or (destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) 
			and upper(source_entity_type)!=upper(v_source_entity_type)))=0)
			then
                 
                 INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE  source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type)
				and destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and is_cable_a_end=false;			
	
				delete from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type)
				and destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and is_cable_a_end=false;			
			end if;

		elsif(upper(v_destination_entity_type)='SPLITTER' or upper(v_destination_entity_type)='ONT')
		then
INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE  ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));



			DELETE FROM CONNECTION_INFO 
			WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));

			--SPLITTER TO SPLITTER or ONT TO ONT
			if((select count(1) from connection_info where (source_system_id=v_destination_system_id and 
			upper(source_entity_type)=upper(v_destination_entity_type) and upper(destination_entity_type)!=upper(v_destination_entity_type)) 
			or (destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
			and upper(source_entity_type)!=upper(v_destination_entity_type)))=0)
			then		
            
            INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type)
				and destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
				and is_cable_a_end=false;	
            
				delete from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type)
				and destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
				and is_cable_a_end=false;				
			end if;
		
		elsif(upper(v_source_entity_type)=upper('FMS'))
		then
        
        INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE
    (source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) and source_port_no=v_source_port_no) 
			or (destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and destination_port_no=v_source_port_no);

        
			delete from connection_info 
			where (source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) and source_port_no=v_source_port_no) 
			or (destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and destination_port_no=v_source_port_no);

			select network_id into v_source_network_id from att_details_fms where system_id=v_source_system_id;

			--CHECK THE MIDDLEWARE CONNECTION EXIST AT SOURCE
			
			if exists(select 1 from att_details_model where network_id=v_source_network_id)
			then
				select * into v_fms_mapping from att_details_model where network_id=v_source_network_id;
				
				if not exists(select 1 from connection_info where 
				(source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' 
				and source_port_no=(-1*v_source_port_no) and upper(destination_entity_type)='PATCHCORD')
				or (destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
				and destination_port_no=(-1*v_source_port_no) and upper(source_entity_type)='PATCHCORD')) 				
				then
                
                INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='FMS';
                                              
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='FMS';
                    
                    INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='FMS';

					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='FMS';
										
				end if;
			end if;		
		ELSIF(upper(v_destination_entity_type)=upper('FMS'))
		THEN
        INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where (source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and 
			source_port_no=v_destination_port_no) 
			or(destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
			and destination_port_no=v_destination_port_no);

        
			delete from connection_info 
			where (source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and 
			source_port_no=v_destination_port_no) 
			or(destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
			and destination_port_no=v_destination_port_no);

			select network_id into v_source_network_id from att_details_fms where system_id=v_destination_system_id;

			if exists(select 1 from att_details_model where network_id=v_source_network_id)
			then			
				select * into v_fms_mapping from att_details_model where network_id=v_source_network_id;

				if not exists(select 1 from connection_info 
				where  
				(source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' 
				and source_port_no=(-1*v_destination_port_no) and upper(destination_entity_type)='PATCHCORD')
				or (destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
				and destination_port_no=(-1*v_destination_port_no) and upper(source_entity_type)='PATCHCORD'))
				then	
                INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='FMS';
                
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='FMS';

INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_destination_port_no
					and destination_system_id=v_destination_system_id and upper(destination_entity_type)='FMS';

					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_destination_port_no
					and destination_system_id=v_destination_system_id and upper(destination_entity_type)='FMS';
				end if;	
			end if;	
		elsif(upper(v_source_entity_type)=upper('EQUIPMENT') and upper(V_DESTINATION_ENTITY_TYPE)=upper('EQUIPMENT'))
		then
			if not exists(select * from connection_info where ((destination_system_id=v_source_system_id 
			and upper(destination_entity_type)=upper(v_source_entity_type) and destination_port_no=v_source_port_no)
			or (source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) 
			and source_port_no=v_source_port_no)) and (source_port_no>0 and destination_port_no>0))
			then
            INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO ci
				where ((lower(ci.destination_entity_type)||ci.destination_system_id)=(lower(ci.source_entity_type)||ci.source_system_id) 
				and ci.source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) and
				((ci.source_port_no=(-1*v_source_port_no) and ci.destination_port_no=v_source_port_no) or 
				(ci.destination_port_no=(-1*v_source_port_no) and ci.source_port_no=v_source_port_no)));
            
				delete from connection_info ci
				where ((lower(ci.destination_entity_type)||ci.destination_system_id)=(lower(ci.source_entity_type)||ci.source_system_id) 
				and ci.source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) and
				((ci.source_port_no=(-1*v_source_port_no) and ci.destination_port_no=v_source_port_no) or 
				(ci.destination_port_no=(-1*v_source_port_no) and ci.source_port_no=v_source_port_no)));

			end if;
				
			if not exists(select * from connection_info where ((destination_system_id=v_destination_system_id and 
			upper(destination_entity_type)=upper(v_destination_entity_type) and destination_port_no=v_destination_port_no)
			or (source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and source_port_no=v_destination_port_no)) 
			and (source_port_no>0 and destination_port_no>0))
			then
            
            INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO ci
				where ((lower(ci.destination_entity_type)||ci.destination_system_id)=(lower(ci.source_entity_type)||ci.source_system_id) 
				and ci.source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and
				((ci.source_port_no=(-1*v_destination_port_no) and ci.destination_port_no=v_destination_port_no) or 
				(ci.destination_port_no=(-1*v_destination_port_no) and ci.source_port_no=v_destination_port_no)));
    
				delete from connection_info ci
				where ((lower(ci.destination_entity_type)||ci.destination_system_id)=(lower(ci.source_entity_type)||ci.source_system_id) 
				and ci.source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and
				((ci.source_port_no=(-1*v_destination_port_no) and ci.destination_port_no=v_destination_port_no) or 
				(ci.destination_port_no=(-1*v_destination_port_no) and ci.source_port_no=v_destination_port_no)));
				
			end if;	

			--DELETE THE MIDDLE WARE CONNECTION IF SOURCE IS MIDDLEWARE ENTITY
			select network_id into v_source_network_id from att_details_model where system_id=v_source_system_id;

			if  v_source_network_id is not null
			then
				select * into v_fms_mapping from att_details_fms where network_id=v_source_network_id;
				
				if not exists(select 1 from connection_info where 
				(source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' 
				and source_port_no=ABS(v_source_port_no) and upper(destination_entity_type)='CABLE')
				or (destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' 
				and destination_port_no=ABS(v_source_port_no) and upper(source_entity_type)='CABLE')) 				
				then					
					INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='EQUIPMENT';
    
    
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='EQUIPMENT';

INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='EQUIPMENT';
    
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='EQUIPMENT';

				end if;
			end if;	

			
			--DELETE THE MIDDLE WARE CONNECTION IF DESTINATION IS MIDDLEWARE ENTITY
			select network_id into v_source_network_id from att_details_model where system_id=v_destination_system_id;

			if  v_source_network_id is not null
			then
				select * into v_fms_mapping from att_details_fms where network_id=v_source_network_id;
				
				if not exists(select 1 from connection_info where 
				(source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' 
				and source_port_no=ABS(v_destination_port_no) and upper(destination_entity_type)='CABLE')
				or (destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' 
				and destination_port_no=ABS(v_destination_port_no) and upper(source_entity_type)='CABLE')) 				
				then					
					INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='EQUIPMENT';
                    
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='EQUIPMENT';

INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' and source_port_no=v_destination_port_no
					and destination_system_id=v_destination_system_id and upper(destination_entity_type)='EQUIPMENT';
    
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' and source_port_no=v_destination_port_no
					and destination_system_id=v_destination_system_id and upper(destination_entity_type)='EQUIPMENT';

				end if;
			end if;
								
		end if;
end if;
	END LOOP;
	close curgeommapping;
  insert into temp_port_status_table (
  source_system_id,source_entity_type,source_port_no,destination_system_id,
  destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,
  equipment_system_id, equipment_entity_type
) select source_system_id,source_entity_type,source_port_no,destination_system_id,
 destination_entity_type,destination_port_no,coalesce(is_source_cable_a_end,false) as is_source_cable_a_end,coalesce(is_destination_cable_a_end,false) as is_destination_cable_a_end,
 equipment_system_id, coalesce(equipment_entity_type ,'') as equipment_entity_type
from json_populate_recordset(null::temp_connection2,replace(p_listconnection,'\','')::json);

--pass hardcoded status = 1 for update ports

Perform(fn_update_port_status_bulk_splicing(1));

RETURN QUERY  SELECT v_isvalid AS STATUS, v_message::CHARACTER VARYING AS MESSAGE;	  
END
$function$
;

-- Permissions

ALTER FUNCTION public.fn_splicing_delete_connection(text, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_splicing_delete_connection(text, int4) TO public;
GRANT ALL ON FUNCTION public.fn_splicing_delete_connection(text, int4) TO postgres;


-----------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_delete_entity(p_system_id integer, p_entity_type character varying, p_geom_type character varying, p_user_id integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$

DECLARE 
v_layer_table character varying;
v_is_parent_exist boolean;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
V_LAYER_TITLE character varying;
v_parent_system_id integer;
v_building_system_id integer;
v_building_code character varying;
v_network_id character varying;
v_target_ref_id integer;

BEGIN
--GET THE LAYER TITLE
SELECT layer_title into V_LAYER_TITLE FROM layer_details where upper(layer_name)=upper(p_entity_type);

V_IS_VALID:=TRUE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_065]');--(V_LAYER_TITLE||' has deleted successfully!');
--CHECK THE STRUCTURE CHILD EXIST
if(upper(p_entity_type)='STRUCTURE' and (select count(1) from isp_entity_mapping where structure_id=p_system_id)>0)
then 
V_IS_VALID=FALSE;
V_MESSAGE:=('[SI_GBL_GBL_GBL_GBL_051]');--Structure can not be deleted as there are some dependent elements! 
--CHECK THE SPLICING(EXCLUDING THE INERNAL CONNECTIVITY OF THE DEVICE)
elsIF EXISTS(select 1 from connection_info 
where ((source_system_id=p_system_id and upper(source_entity_type)=upper(p_entity_type)) or ( destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type)))
and ( ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))))
THEN 
v_is_parent_exist=false;
V_IS_VALID=FALSE;   

IF EXISTS(SELECT 1 FROM CONNECTION_INFO where ((source_system_id=p_system_id and upper(source_entity_type)=upper(p_entity_type)) or ( destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type))) )
THEN 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!');       
END IF;

IF(UPPER(p_entity_type)='SPLITTER') 
AND EXISTS(SELECT 1 FROM CONNECTION_INFO CON JOIN ATT_DETAILS_SPLITTER SPL 
ON  SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE') 
OR (CON.DESTINATION_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.DESTINATION_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.SOURCE_ENTITY_TYPE)='CABLE')) )
THEN
v_is_parent_exist=true; 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 
        ELSIF(UPPER(p_entity_type)='SPLITTER')THEN 
v_is_parent_exist=TRUE;
END IF;
        
IF(UPPER(p_entity_type)='SPLICECLOSURE') 
AND EXISTS(SELECT 1 FROM CONNECTION_INFO CON JOIN ATT_DETAILS_SPLICECLOSURE SPL 
ON SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE') 
OR (CON.DESTINATION_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.DESTINATION_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.SOURCE_ENTITY_TYPE)='CABLE')))
THEN 
        v_is_parent_exist=true;
        V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 

ELSIF((SELECT PARENT_SYSTEM_ID FROM ATT_DETAILS_SPLICECLOSURE SPL WHERE SPL.SYSTEM_ID=p_system_id and UPPER(p_entity_type)='SPLICECLOSURE')!=0)THEN 
v_is_parent_exist=true;
END IF;

IF(v_is_parent_exist=false)
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 

end if; 
--CHECK THE ROUTE ENTITY ASSOCIATION FIRST
elsif exists(select 1 from  ASSOCIATE_ROUTE_INFO asso where asso.entity_id=p_system_id and upper(entity_type)=upper(p_entity_type))
then 
delete from ASSOCIATE_ROUTE_INFO where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);

-- elsif exists(select 1 from  ASSOCIATE_ROUTE_INFO asso where asso.entity_id=p_system_id and upper(entity_type)=upper(p_entity_type))
-- then 
-- V_IS_VALID=FALSE;
-- V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as route is associated with some entity!');

--CHECK THE ENTITY ASSOCIATION FIRST
elsif exists(select 1 from  associate_entity_info asso where ((asso.associated_system_id=p_system_id and upper(asso.associated_entity_type)=UPPER(p_entity_type)) or 
(asso.entity_system_id=p_system_id and upper(asso.entity_type)=UPPER(p_entity_type))) and asso.is_termination_point=false)
and not exists(select 1 from layer_details where upper(layer_name)=upper(p_entity_type) and ne_class_name='SPLICE_CLOSURE')
then 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_152]');--(V_LAYER_TITLE||'can not be deleted as it is associated with some entity!'); 

--CHECK THE TERMINATION POINT DUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('duct') and is_enabled=true) 
and exists(select  1 from att_details_duct duct where (duct.a_system_id=p_system_id and upper(duct.a_entity_type)=upper(p_entity_type)) 
or (duct.b_system_id=p_system_id and upper(duct.b_entity_type)=upper(p_entity_type)))
then 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_053]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with duct!'); 

--CHECK THE TERMINATION POINT TRENCH
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('trench') and is_enabled=true) 
and exists(select  1 from att_details_trench trench where (trench.a_system_id=p_system_id and upper(trench.a_entity_type)=upper(p_entity_type)) 
or (trench.b_system_id=p_system_id and upper(trench.b_entity_type)=upper(p_entity_type)))
then   
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_054]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with trench!'); 

--CHECK THE TERMINATION POINT CABLE
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('cable') and is_enabled=true) 
  and exists(select 1 from att_details_cable cable where (cable.a_system_id=p_system_id and upper(cable.a_entity_type)=upper(p_entity_type)) 
  or (cable.b_system_id=p_system_id and upper(cable.b_entity_type)=upper(p_entity_type)))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_055]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with cable!');

--CHECK THE TERMINATION POINT MICRODUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('microduct') and is_enabled=true) 
and exists(select  1 from att_details_microduct microduct where (microduct.a_system_id=p_system_id and upper(microduct.a_entity_type)=upper(p_entity_type)) 
or (microduct.b_system_id=p_system_id and upper(microduct.b_entity_type)=upper(p_entity_type)))
then 
V_IS_VALID=FALSE;
V_MESSAGE:=(V_LAYER_TITLE ||' can not be deleted as it is used as termination point with microduct!'); --//have change the message for microduct 

elsif(upper(p_entity_type)='ROW') 
and exists(select 1 from att_details_row_apply where row_system_id=p_system_id) 
and not exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id)
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_056]');--(V_LAYER_TITLE||' can not be deleted as it is applied!');

elsif(upper(p_entity_type)='ROW') 
and exists(select 1 from att_details_row_apply where row_system_id=p_system_id) 
and  exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id and  upper(row_status)=upper('Approved'))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_057]');--(V_LAYER_TITLE||' can not be deleted as it has been Approved!');

elsif(upper(p_entity_type)='PIT')
then 
select parent_system_id into v_parent_system_id from att_details_row_pit where system_id=p_system_id;

IF EXISTS(SELECT 1 FROM ATT_DETAILS_ROW_APPLY WHERE ROW_SYSTEM_ID=v_parent_system_id) 
THEN
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as  ROW has been Applied!'); 
END IF;
ELSIF(upper(p_entity_type)='CABLE') AND EXISTS(SELECT 1 FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=p_system_id)
then

V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as LMC is associated!');
ELSIF(upper(p_entity_type)='CABLE')
then
delete from att_details_loop where cable_system_id= p_system_id;

end if;

IF(upper(p_geom_type)='POLYGON') then

select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
raise info'V_LAYER_TABLE: %',V_LAYER_TABLE;

EXECUTE  'SELECT COALESCE(TARGET_REF_ID,0)  FROM '||V_LAYER_TABLE||' WHERE SYSTEM_ID='||P_SYSTEM_ID||'' INTO V_TARGET_REF_ID;

IF(upper(p_entity_type)!='SECTOR') THEN
IF exists (select 1 from 
(SELECT ptm.system_id ,ptm.entity_type,ptm.SP_GEOMETRY FROM point_master ptm  
UNION
SELECT lm.system_id ,lm.entity_type,lm.SP_GEOMETRY FROM line_master lm  
UNION
SELECT pm.system_id ,pm.entity_type,pm.SP_GEOMETRY FROM polygon_master pm  
) a WHERE ST_Within(a.SP_GEOMETRY, 
(select sp_geometry from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type))) 
and a.system_id!=p_system_id) 
then

V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as some child entities are present inside it!');

end if;

END IF;
-- IF(p_entity_type in('Area','Subarea','DSA') and
-- exists(select 1 from polygon_master pm 
-- where pm.system_id =p_system_id and upper(pm.entity_type) = upper(p_entity_type) and COALESCE(pm.gis_design_id ,'')!='') )
-- THEN 
-- V_IS_VALID=FALSE;
-- V_MESSAGE:='Reset the design ID to delete the entity';
-- END IF;
IF(V_TARGET_REF_ID>0 and (select COALESCE(value,'0')::INTEGER from global_settings gs where key='IS_DeleteEnabledForGISPush')=1)
THEN 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as it is been pushed to GIS!');

END IF;

END IF;

IF(V_IS_VALID and p_entity_type='Cable' and exists(select 1 from top_ring_cable_mapping where cable_id=p_system_id ))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as cable already in a topology so please rearrange it!');
end if;

IF(V_IS_VALID and upper(p_entity_type)='POD' and exists(select 1 from top_ring_site_mapping where site_id=p_system_id ))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as site already in a topology so please rearrange it!');
end if;

IF(V_IS_VALID)
THEN
IF(UPPER(p_entity_type)=UPPER('SUBAREA')) 
THEN
select building_system_id,building_code into v_building_system_id,v_building_code from att_details_subarea where system_id=p_system_id;
delete from att_details_building where system_id=v_building_system_id and network_id=v_building_code;
delete from polygon_master where system_id=v_building_system_id and upper(entity_type)=upper('Building'); 
END IF;
IF(upper(p_entity_type)='FMS')
THEN
DELETE  FROM ATT_DETAILS_MODEL B  
USING ATT_DETAILS_FMS C 
WHERE UPPER(B.NETWORK_ID)=UPPER(C.NETWORK_ID)
AND C.SYSTEM_ID=P_SYSTEM_ID;     
END IF;

perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,2,true));

select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
execute  'select network_id from '||v_layer_table||' where system_id='||p_system_id||'' into v_network_id; 
execute  'delete from '||v_layer_table||' where system_id='||p_system_id||''; 
execute 'delete from '||p_geom_type||'_master where system_id='||p_system_id||' and upper(entity_type)=upper('''||p_entity_type||''')';
delete from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from isp_entity_mapping where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from isp_port_info where parent_system_id=p_system_id and upper(parent_entity_type)=upper(p_entity_type);
delete from associate_entity_info where (associated_system_id=p_system_id 
and upper(associated_entity_type)=upper(p_entity_type)) or (entity_system_id=p_system_id and upper(entity_type)=upper(p_entity_type)) and is_termination_point=true;
delete from connection_info  where (source_system_id=p_system_id 
and upper(source_entity_type)=upper(p_entity_type)) or (destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type));
delete from isp_line_master  where entity_id=p_system_id  and upper(entity_type)=upper(p_entity_type);
delete from circle_master  where system_id=p_system_id  and upper(entity_type)=upper(p_entity_type);

if(UPPER(p_entity_type)=UPPER('POD') and (select count(1) from top_ring_site_mapping where site_id=p_system_id)>0) then

 delete from top_ring_site_mapping where site_id=p_system_id;
END IF;
if(UPPER(p_entity_type)=UPPER('Cable') and (select count(1) from top_ring_cable_mapping where cable_id=p_system_id)>0) then

 delete from top_ring_cable_mapping where cable_id=p_system_id;
END IF;
IF EXISTS (select 1 from layer_details where upper(layer_name)=upper(p_entity_type) and is_reference_allowed=true) THEN
BEGIN
delete from att_entity_reference where system_id=p_system_id;
END;

IF(UPPER(p_entity_type)=UPPER('CUSTOMER'))
THEN
delete from wcr_mapping where CUSTOMER_ID=p_system_id;
END IF;

END IF;

RETURN QUERY 
SELECT true AS STATUS, fn_Multilingual_Message_Convert('en',Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_065]')::CHARACTER VARYING)::CHARACTER VARYING AS MESSAGE ; --V_LAYER_TITLE||' has deleted successfully!')
ELSE
RETURN QUERY 
SELECT V_IS_VALID AS STATUS, fn_Multilingual_Message_Convert('en',V_MESSAGE::CHARACTER VARYING) ::CHARACTER VARYING AS MESSAGE;
END IF;

if(coalesce(v_network_id,'')!='')
then
insert into entity_delete_log(system_id,entity_type,network_id,action_date,action_by)
values(p_system_id,p_entity_type,v_network_id,now(),p_user_id);
end if;

END
$function$
;

-- Permissions

ALTER FUNCTION public.fn_delete_entity(int4, varchar, varchar, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_delete_entity(int4, varchar, varchar, int4) TO public;
GRANT ALL ON FUNCTION public.fn_delete_entity(int4, varchar, varchar, int4) TO postgres;

------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_associated_fiber_link_details(p_systemid integer, p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_userid integer, p_searchfrom timestamp without time zone, p_searchto timestamp without time zone)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
   sql TEXT; 
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords INTEGER; 
   LowerStart  character varying;
   LowerEnd  character varying;
   v_user_role_id integer;
   s_layer_columns_val text; 

BEGIN

sql:='';
LowerStart:='';
LowerEnd:='';

-- IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
--     LowerStart:='LOWER(';
--     LowerEnd:=')';
--   END IF;
  
select role_id into v_user_role_id from user_master where user_id=p_userId;

-- FETCH ALL COLUMNS FROM COLUMN SETTINGS TABLE
	 
	 SELECT STRING_AGG(COLUMN_NAME,',') INTO S_LAYER_COLUMNS_VAL  FROM fiber_link_columns_settings WHERE is_active=true;
 
  
-- MANAGE SORT COLUMN NAME
IF (coalesce(P_SORTCOLNAME,'')!='') THEN 
	select column_name into P_SORTCOLNAME from fiber_link_columns_settings WHERE DISPLAY_NAME=P_SORTCOLNAME;
End IF;
-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'fl.system_id' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS S_No, 
fl.system_id,fl.network_id,'||S_LAYER_COLUMNS_VAL||'
	FROM vw_att_details_fiber_link fl WHERE upper(fl.fiber_link_status)=upper(''Associated'') and 1=1  ';
	

 IF(p_systemid >0 )THEN 
	sql:= sql ||'and fl.system_id='||p_systemid||'';
 END IF;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2% ',sql;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;
	-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a ' INTO TotalRecords;
 
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords,*
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 


RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_associated_fiber_link_details(int4, varchar, varchar, int4, int4, varchar, varchar, int4, timestamp, timestamp) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_associated_fiber_link_details(int4, varchar, varchar, int4, int4, varchar, varchar, int4, timestamp, timestamp) TO public;
GRANT ALL ON FUNCTION public.fn_get_associated_fiber_link_details(int4, varchar, varchar, int4, int4, varchar, varchar, int4, timestamp, timestamp) TO postgres;

------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation(source_network_id character varying, destination_network_id character varying, buffer integer, required_core integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE     
    P_SOURCE CHARACTER VARYING;
    P_DESTINATION CHARACTER VARYING;    
    P_SOURCE_SYSTEM_ID INTEGER;
    P_DESTINATION_SYSTEM_ID INTEGER;
    P_SOURCE_NETWORK_ID CHARACTER VARYING;
	P_DESTINATION_NETWORK_ID CHARACTER VARYING;
	V_IS_VALID BOOLEAN;
	V_MESSAGE CHARACTER VARYING;
	FIBER_ODF_COUNT INTEGER;
    FIBER_CABLE_COUNT INTEGER;
   Is_AvailableODF BOOLEAN;
  v_rec RECORD;
 SC_NETWORK_ID CHARACTER VARYING;
BEGIN
	V_IS_VALID:=TRUE;
	V_MESSAGE:='Required Core available';
	P_SOURCE_NETWORK_ID:=SOURCE_NETWORK_ID;
	P_DESTINATION_NETWORK_ID:=DESTINATION_NETWORK_ID;
	P_SOURCE_SYSTEM_ID:=0;
	P_DESTINATION_SYSTEM_ID:=0;
	FIBER_ODF_COUNT:=0;
    FIBER_CABLE_COUNT:=0;
     --TRUNCATE TABLE TEMP_CABLE_ROUTES; 
     --TRUNCATE TABLE TEMP_ISP_PORT_INFO;
     --TRUNCATE TABLE TEMP_ISP_PORT_INFO; 
     --TRUNCATE TABLE TEMP_ROUTE_CONNECTION; 
     --TRUNCATE table connected_fiberlink_cable;

-- CREATE TEMPORARY TABLES
        CREATE TEMP TABLE TEMP_CABLE_ROUTES(
        SEQ INTEGER,
        PATH_SEQ INTEGER, 
        EDGE_TARGETID INTEGER, 
        ROADLINE_GEOMTEXT TEXT, 
        START_POINT CHARACTER VARYING,
        END_POINT CHARACTER VARYING,
        MESSAGE VARCHAR NULL,
        IS_VALID BOOLEAN DEFAULT TRUE,
        AVAIABLE_CORE_COUNT INTEGER DEFAULT 0,
        USER_ID INTEGER,
        A_SYSTEM_ID INTEGER,
        A_ENTITY_TYPE CHARACTER VARYING,
        A_NETWORK_ID CHARACTER VARYING,
        B_SYSTEM_ID INTEGER,
        B_ENTITY_TYPE CHARACTER VARYING,
        B_NETWORK_ID CHARACTER VARYING,
        A_POINT_Geom geometry,
        B_POINT_Geom geometry,
        IS_MULTIPLE_ODF BOOLEAN DEFAULT TRUE
    ) ON COMMIT DROP;

   CREATE TEMP TABLE TEMP_ISP_PORT_INFO(
        SYSTEM_ID INT4 NULL,
        NETWORK_ID VARCHAR NULL,
        PARENT_SYSTEM_ID INT4 NULL,
        PARENT_NETWORK_ID VARCHAR NULL,
        PORT_STATUS_ID INT4 NULL,
       -- IS_VALID BOOLEAN DEFAULT TRUE,
        PORT_NUMBER INT4 NULL,
        PARENT_ENTITY_TYPE VARCHAR NULL,
		IS_VALID BOOLEAN NOT NULL DEFAULT TRUE
    ) ON COMMIT DROP;

	CREATE TEMP TABLE TEMP_ROUTE_CONNECTION (
		ID SERIAL,
		CONNECTION_ID INT4,
		SOURCE_SYSTEM_ID INT4 NULL,
		SOURCE_NETWORK_ID VARCHAR(100) NULL,
		SOURCE_ENTITY_TYPE VARCHAR(100) NULL,
		SOURCE_PORT_NO INT4 NULL,
		DESTINATION_SYSTEM_ID INT4 NULL,
		DESTINATION_NETWORK_ID VARCHAR(100) NULL,
		DESTINATION_ENTITY_TYPE VARCHAR(100) NULL,
		DESTINATION_PORT_NO INT4 null,
		IS_CABLE_A_END BOOLEAN,
		IS_CABLE_B_END BOOLEAN,
		IS_VALID boolean default true,
		connection_network_id VARCHAR NULL,
		Connection_entity_type VARCHAR NULL
	)ON COMMIT DROP;
    

  	 create TEMP TABLE connected_fiberlink_cable (
     id SERIAL PRIMARY KEY,
     cable_id INTEGER NOT NULL,
     fiber_number INTEGER NOT NULL,
     is_valid boolean default true,
     link_system_id INTEGER,
	 a_end_status_id INTEGER,
	 b_end_status_id INTEGER,
	 error_msg VARCHAR null
	) ON COMMIT DROP;

    -- CLEAR PREVIOUS LOGS FOR THE USER
    DELETE FROM CORE_PLANNER_LOGS WHERE USER_ID = P_USER_ID;
    DELETE  FROM CORE_PLANNER_FIBER_INFO WHERE USER_ID = P_USER_ID;
    
    IF V_IS_VALID and REQUIRED_CORE > 2
    THEN 
	V_IS_VALID:=FALSE;
	V_MESSAGE:='A maximum of two ports is permitted!';        
    END IF;
   
    IF V_IS_VALID and NOT EXISTS(SELECT 1 FROM POINT_MASTER WHERE COMMON_NAME IN(SOURCE_NETWORK_ID) and entity_type in('FMS','SpliceClosure'))
    THEN 
	V_IS_VALID:=FALSE;
	V_MESSAGE:='Please enter a valid ODF/Splice Closure at start!';        
    END IF;

    IF V_IS_VALID and NOT EXISTS(SELECT 1 FROM POINT_MASTER WHERE COMMON_NAME IN(DESTINATION_NETWORK_ID) and entity_type in('FMS','SpliceClosure'))
    THEN 
	V_IS_VALID:=FALSE;
	V_MESSAGE:='Please enter a valid ODF/Splice Closure at end!';        
    END IF;

    IF V_IS_VALID AND (SELECT COUNT(1)=0 FROM ATT_DETAILS_FMS WHERE NETWORK_ID IN(SOURCE_NETWORK_ID,DESTINATION_NETWORK_ID))
    THEN
	V_IS_VALID:=FALSE;
	V_MESSAGE:='AT LEAST ONE ODF IS REQUIRED. PLEASE PROVIDE A VALID ODF.!'  ;      
    END IF;
        
  if(V_IS_VALID)
  then           
	  SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
	  INTO P_SOURCE, P_SOURCE_SYSTEM_ID
	  FROM att_details_fms
	  WHERE NETWORK_ID IN(SOURCE_NETWORK_ID);

	  SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
	  INTO P_DESTINATION, P_DESTINATION_SYSTEM_ID 
	  FROM att_details_fms
	  WHERE NETWORK_ID IN(DESTINATION_NETWORK_ID);

	IF COALESCE(P_SOURCE_SYSTEM_ID,0)=0
	THEN	
	    SC_NETWORK_ID = SOURCE_NETWORK_ID;
		SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
		INTO P_SOURCE
		FROM att_details_spliceclosure
		WHERE network_id  IN(SOURCE_NETWORK_ID);
	end if;

	IF COALESCE(P_DESTINATION_SYSTEM_ID,0)=0
	then
		SC_NETWORK_ID = DESTINATION_NETWORK_ID;
		SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
		INTO P_DESTINATION
		FROM att_details_spliceclosure
		WHERE network_id IN(DESTINATION_NETWORK_ID);
	end if;


      IF P_DESTINATION_SYSTEM_ID <> 0 and P_SOURCE_SYSTEM_ID <> 0 then
       
	    INSERT INTO TEMP_ISP_PORT_INFO (SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, 
	    PARENT_NETWORK_ID,   PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE)
	    SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID, 
	    PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE
	    FROM ISP_PORT_INFO A 
	    WHERE A.PARENT_SYSTEM_ID = P_DESTINATION_SYSTEM_ID 
	    AND A.PARENT_ENTITY_TYPE = 'FMS' AND INPUT_OUTPUT = 'O' --and port_status_id = 1 
	   ORDER BY SYSTEM_ID;
	 end if;
	 
	  WITH SORTED_DATA AS (
		            SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID, 
	    PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE,
		            LAG(PORT_NUMBER) OVER (PARTITION BY PARENT_SYSTEM_ID ORDER BY PORT_NUMBER) AS PREV_FIBER
		            FROM ISP_PORT_INFO 
		            WHERE  PARENT_SYSTEM_ID = (CASE WHEN P_SOURCE_SYSTEM_ID <> 0 THEN P_SOURCE_SYSTEM_ID
                   ELSE P_DESTINATION_SYSTEM_ID END) AND 
		            PORT_NUMBER % 2 = 1 AND	INPUT_OUTPUT = 'O' and PARENT_ENTITY_TYPE = 'FMS' 
		            --and port_status_id = 1
		        ),
		        FILTERED_DATA AS (
		            SELECT A.SYSTEM_ID, A.NETWORK_ID, A.PARENT_SYSTEM_ID, A.PARENT_NETWORK_ID, 
	    A.PORT_STATUS_ID, A.PORT_NUMBER,A.PARENT_ENTITY_TYPE
		            FROM ISP_PORT_INFO A
		            INNER JOIN SORTED_DATA B 
		                ON A.PARENT_SYSTEM_ID = B.PARENT_SYSTEM_ID 
		                AND (A.PORT_NUMBER = B.PORT_NUMBER + 1 OR A.PORT_NUMBER = B.PORT_NUMBER)
		            WHERE A.INPUT_OUTPUT = 'O' and A.PARENT_ENTITY_TYPE = 'FMS'
		            --and A.port_status_id = 1
		        ),
		        FIBER_DATA AS (
		            SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID, 
	                 PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE,
		            LAG(PORT_NUMBER) OVER (PARTITION BY PARENT_SYSTEM_ID ORDER BY PORT_NUMBER) AS PREV_FIBER,
		            LEAD(PORT_NUMBER) OVER (PARTITION BY PARENT_SYSTEM_ID ORDER BY PORT_NUMBER) AS NEXT_FIBER        
		            FROM FILTERED_DATA 
		            WHERE PARENT_SYSTEM_ID = (CASE WHEN P_SOURCE_SYSTEM_ID <> 0 THEN P_SOURCE_SYSTEM_ID
                   ELSE P_DESTINATION_SYSTEM_ID END)
		        )		        
       INSERT INTO TEMP_ISP_PORT_INFO (SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, 
	   PARENT_NETWORK_ID,   PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE)
	   
	    SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID, 
	                 PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE FROM FIBER_DATA
		WHERE PORT_NUMBER = PREV_FIBER + 1 OR PORT_NUMBER = NEXT_FIBER - 1;
		        		        
          -- Populate temporary cable routes table
	  INSERT INTO TEMP_CABLE_ROUTES(SEQ, PATH_SEQ, EDGE_TARGETID, USER_ID)		  
	  SELECT SEQ, PATH_SEQ, EDGE,P_USER_ID FROM PGR_DIJKSTRA('SELECT ID, SOURCE, TARGET, COST, REVERSE_COST 
	  FROM ROUTING_DATA_CORE_PLAN', (SELECT ID
	  FROM ROUTING_DATA_CORE_PLAN_VERTICES_PGR
	  WHERE ST_WITHIN( THE_GEOM, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT(' || P_SOURCE || ')', 4326), 3))
	  LIMIT 1 ),(SELECT ID FROM ROUTING_DATA_CORE_PLAN_VERTICES_PGR
	  WHERE ST_WITHIN( THE_GEOM, 
	  ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('|| P_DESTINATION ||')', 4326), 3)) 
	  LIMIT 1));
	
	  DELETE FROM TEMP_CABLE_ROUTES WHERE EDGE_TARGETID = -1 AND USER_ID = P_USER_ID;

	IF NOT EXISTS(SELECT 1 FROM TEMP_CABLE_ROUTES)
	THEN
		V_IS_VALID:=false;
		V_MESSAGE:='Route not found between ODF/Splice Closure!';
	END IF;

	-----------UPDATE THE A END ID
	WITH STARTCTE AS(
	SELECT PM.SYSTEM_ID,PM.ENTITY_TYPE,PM.COMMON_NAME,EDGE_TARGETID as CABLE_ID,
	ST_ENDPOINT(LM.SP_GEOMETRY) AS ENDPOINT,PM.sp_geometry as point_geom
	FROM TEMP_CABLE_ROUTES RS
	INNER JOIN LINE_MASTER LM 
	ON LM.SYSTEM_ID = RS.EDGE_TARGETID AND UPPER(LM.ENTITY_TYPE) = 'CABLE' 
	INNER JOIN  POINT_MASTER PM  
	ON ST_WITHIN(PM.SP_GEOMETRY, ST_BUFFER_METERS(ST_STARTPOINT(LM.SP_GEOMETRY), 3)) 
	AND (UPPER(PM.ENTITY_TYPE) in ('BDB','FDB','SPLICECLOSURE')
        OR (
            UPPER(PM.ENTITY_TYPE) = 'FMS'
            AND PM.COMMON_NAME IN (SOURCE_NETWORK_ID, DESTINATION_NETWORK_ID)
        )) )
			
	UPDATE TEMP_CABLE_ROUTES 
	SET 
	A_SYSTEM_ID = STARTCTE.SYSTEM_ID,
	A_ENTITY_TYPE = STARTCTE.ENTITY_TYPE,
	A_NETWORK_ID=STARTCTE.COMMON_NAME,
	A_POINT_Geom = STARTCTE.point_geom
	FROM STARTCTE
	WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID=STARTCTE.CABLE_ID;

	-----------UPDATE THE B END ID
	WITH ENDCTE AS(
	SELECT PM.SYSTEM_ID,PM.ENTITY_TYPE,PM.COMMON_NAME,EDGE_TARGETID as CABLE_ID, ST_ENDPOINT(LM.SP_GEOMETRY) AS ENDPOINT ,PM.sp_geometry as point_geom
	FROM TEMP_CABLE_ROUTES RS
	INNER JOIN LINE_MASTER LM 
	ON LM.SYSTEM_ID = RS.EDGE_TARGETID AND UPPER(LM.ENTITY_TYPE) = 'CABLE' 
	INNER JOIN  POINT_MASTER PM  
	ON ST_WITHIN(PM.SP_GEOMETRY, ST_BUFFER_METERS(ST_ENDPOINT(LM.SP_GEOMETRY),3)) 
	AND  (UPPER(PM.ENTITY_TYPE) in ('BDB','FDB','SPLICECLOSURE')
        OR (
            UPPER(PM.ENTITY_TYPE) = 'FMS'
            AND PM.COMMON_NAME IN (SOURCE_NETWORK_ID, DESTINATION_NETWORK_ID)
        ))  )
			
	UPDATE TEMP_CABLE_ROUTES 
	SET 
	B_SYSTEM_ID = ENDCTE.SYSTEM_ID,
	B_ENTITY_TYPE = ENDCTE.ENTITY_TYPE,
	B_NETWORK_ID=ENDCTE.COMMON_NAME,
	B_POINT_Geom = ENDCTE.point_geom
	FROM ENDCTE
	WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID=ENDCTE.CABLE_ID;

 
	/*IF V_IS_VALID AND exists ( select 1 from TEMP_CABLE_ROUTES where (A_ENTITY_TYPE = 'FMS' or B_ENTITY_TYPE = 'FMS'))
	then     
		V_IS_VALID:=FALSE;
		V_MESSAGE:='Multiple ODF found on this route!';
	END IF;*/

	IF V_IS_VALID AND (SELECT COUNT(1)>0 FROM TEMP_CABLE_ROUTES WHERE COALESCE(B_ENTITY_TYPE,'')=''
	OR COALESCE(A_ENTITY_TYPE,'')='')
	THEN
		UPDATE TEMP_CABLE_ROUTES SET IS_VALID=false,
		MESSAGE='Multiple ODFs found along this route, or the Splice Closure is missing at the end of the cable.'
		WHERE COALESCE(B_ENTITY_TYPE,'')='' OR COALESCE(A_ENTITY_TYPE,'')='';		 
	
		V_IS_VALID:=FALSE;
		V_MESSAGE:='Multiple ODFs found along this route, or the Splice Closure is missing at the end of the cable.';
	END IF;

    IF(V_IS_VALID = FALSE)
    THEN 
    
     INSERT INTO CORE_PLANNER_LOGS (cable_network_id,CABLE_ID,CABLE_NAME,NETWORK_STATUS,
	  TOTAL_CORE,CABLE_LENGTH,ERROR_MSG,USER_ID,IS_VALID,a_system_id,b_system_id)
	  SELECT AC.NETWORK_ID,trc.edge_targetid  ,AC.CABLE_NAME ,AC.NETWORK_STATUS ,
	  AC.TOTAL_CORE,AC.CABLE_CALCULATED_LENGTH,TRC.MESSAGE,P_USER_ID,TRC.is_valid,0,0  FROM TEMP_CABLE_ROUTES TRC 
      JOIN ATT_DETAILS_CABLE AC ON AC.SYSTEM_ID = TRC.EDGE_TARGETID AND TRC.USER_ID = P_USER_ID;
     
    END IF;
   
	IF(V_IS_VALID)
	THEN
		--GET THE ALL CABLE FIBER DETAILS	
		WITH SORTED_DATA AS (
		    SELECT CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, 
		           IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, LINK_SYSTEM_ID,
		           LAG(FIBER_NUMBER) OVER (PARTITION BY CABLE_ID ORDER BY FIBER_NUMBER) AS PREV_FIBER
		    FROM ATT_DETAILS_CABLE_INFO 
		    WHERE CABLE_ID IN (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) 
		          AND FIBER_NUMBER % 2 = 1
		          AND LINK_SYSTEM_ID = 0  ORDER BY FIBER_NUMBER
		),
		FILTERED_DATA AS (
		    SELECT A.CABLE_ID, A.FIBER_NUMBER, A.A_END_STATUS_ID, A.B_END_STATUS_ID, 
		           A.IS_A_END_THROUGH_CONNECTIVITY, A.IS_B_END_THROUGH_CONNECTIVITY, 
		            A.LINK_SYSTEM_ID
		    FROM ATT_DETAILS_CABLE_INFO A
		    INNER JOIN SORTED_DATA B 
		        ON A.CABLE_ID = B.CABLE_ID 
		        AND (A.FIBER_NUMBER = B.FIBER_NUMBER + 1 OR A.FIBER_NUMBER = B.FIBER_NUMBER)
		    WHERE A.LINK_SYSTEM_ID = 0 
		),
		FIBER_DATA AS (
		    SELECT CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, 
		           IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, 
		            LINK_SYSTEM_ID,  -- Fixed missing comma
		           LAG(FIBER_NUMBER) OVER (PARTITION BY CABLE_ID ORDER BY FIBER_NUMBER) AS PREV_FIBER,
		           LEAD(FIBER_NUMBER) OVER (PARTITION BY CABLE_ID ORDER BY FIBER_NUMBER) AS NEXT_FIBER        
		    FROM FILTERED_DATA 
		    WHERE CABLE_ID IN (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) ORDER BY FIBER_NUMBER
		)		
		-- Use correct INSERT INTO format
		INSERT INTO CORE_PLANNER_FIBER_INFO 
		    (CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, 
		     IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, USER_ID, LINK_SYSTEM_ID,seq)
		SELECT CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, 
		IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, P_USER_ID, LINK_SYSTEM_ID, tcr.SEQ
		FROM FIBER_DATA
		JOIN TEMP_CABLE_ROUTES tcr 
	    ON CABLE_ID = tcr.EDGE_TARGETID
		--WHERE 
		--    FIBER_NUMBER = PREV_FIBER + 1 OR FIBER_NUMBER = NEXT_FIBER - 1
		ORDER BY 
		    tcr.SEQ, FIBER_NUMBER;


		--GET THE CONNECTION OF CABLE FROM THIS ROUTE
		INSERT INTO TEMP_ROUTE_CONNECTION(
		SOURCE_SYSTEM_ID,SOURCE_NETWORK_ID,SOURCE_ENTITY_TYPE,SOURCE_PORT_NO,DESTINATION_SYSTEM_ID,
		DESTINATION_NETWORK_ID,DESTINATION_ENTITY_TYPE,DESTINATION_PORT_NO,IS_CABLE_A_END,IS_CABLE_B_END,
		Connection_network_id, Connection_entity_type)

		select A.SOURCE_SYSTEM_ID,A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,
		B.DESTINATION_SYSTEM_ID,B.DESTINATION_NETWORK_ID,B.DESTINATION_ENTITY_TYPE,
		B.DESTINATION_PORT_NO,A.IS_CABLE_A_END,B.IS_CABLE_A_END,A.DESTINATION_NETWORK_ID,
		A.DESTINATION_ENTITY_TYPE 
		FROM CONNECTION_INFO A
		INNER JOIN CONNECTION_INFO B 
		ON A.DESTINATION_SYSTEM_ID=B.SOURCE_SYSTEM_ID AND A.DESTINATION_ENTITY_TYPE=B.SOURCE_ENTITY_TYPE
		AND A.DESTINATION_PORT_NO=B.SOURCE_PORT_NO WHERE A.SOURCE_SYSTEM_ID 
		in (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) 
		AND A.SOURCE_ENTITY_TYPE='Cable' AND B.DESTINATION_ENTITY_TYPE='Cable'

		UNION 
		SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
		A.DESTINATION_PORT_NO,B.SOURCE_SYSTEM_ID, B.SOURCE_NETWORK_ID,B.SOURCE_ENTITY_TYPE,
		B.SOURCE_PORT_NO,A.IS_CABLE_A_END,B.is_cable_a_end,A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE
		FROM CONNECTION_INFO A
		INNER JOIN CONNECTION_INFO B 
		ON B.DESTINATION_SYSTEM_ID=A.SOURCE_SYSTEM_ID AND B.DESTINATION_ENTITY_TYPE=A.SOURCE_ENTITY_TYPE
		AND B.DESTINATION_PORT_NO=A.SOURCE_PORT_NO WHERE A.DESTINATION_SYSTEM_ID in 
		(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) AND A.DESTINATION_ENTITY_TYPE='Cable' AND B.SOURCE_ENTITY_TYPE='Cable'	
		
		UNION 
		SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
		A.DESTINATION_PORT_NO,
		A.SOURCE_SYSTEM_ID,A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,false,A.IS_CABLE_A_END,
		null,null
		FROM CONNECTION_INFO A
		WHERE A.SOURCE_ENTITY_TYPE='FMS' and
		A.SOURCE_NETWORK_ID IN(P_SOURCE_NETWORK_ID,P_DESTINATION_NETWORK_ID) 
		AND A.DESTINATION_ENTITY_TYPE='Cable'
		UNION 
		SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
		A.DESTINATION_PORT_NO,
		A.SOURCE_SYSTEM_ID, A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,A.IS_CABLE_A_END,
		false,null,null
		FROM CONNECTION_INFO A
		WHERE A.DESTINATION_ENTITY_TYPE='FMS' AND 
		A.DESTINATION_NETWORK_ID IN(P_SOURCE_NETWORK_ID,P_DESTINATION_NETWORK_ID)
		AND A.SOURCE_ENTITY_TYPE='Cable';
	
	 WITH combined AS (
    SELECT 
        t1.id, t1.source_system_id, t1.source_network_id ,t1.source_entity_type,t1.source_port_no,
        t1.destination_system_id, t1.destination_network_id, t1.destination_entity_type, 		t1.destination_port_no,t1.is_cable_a_end, t1.is_cable_b_end 
    FROM TEMP_ROUTE_CONNECTION t1
    UNION ALL

    SELECT 
        t2.id, t2.destination_system_id AS source_system_id, t2.destination_network_id AS source_network_id,
        t2.destination_entity_type AS source_entity_type, t2.destination_port_no AS source_port_no,
        t2.source_system_id AS destination_system_id,t2.source_network_id AS destination_network_id,
        t2.source_entity_type AS destination_entity_type,t2.source_port_no AS destination_port_no,
        t2.is_cable_b_end AS is_cable_a_end,t2.is_cable_a_end AS is_cable_b_end
    FROM TEMP_ROUTE_CONNECTION t2
    ),
    ranked AS (
     SELECT *,
           ROW_NUMBER() OVER (
               PARTITION BY 
                   combined.source_system_id,combined.source_network_id, combined.source_entity_type, 				 					combined.source_port_no,combined.destination_system_id,combined.destination_network_id,
                   combined.destination_entity_type,combined.destination_port_no,
                   combined.is_cable_a_end,combined.is_cable_b_end
               ORDER by combined.id
           ) AS rn
    FROM combined
	),
	removeDuplicateConnections AS (
	    SELECT id 
	    FROM ranked
	    WHERE rn > 1
	)
	DELETE FROM TEMP_ROUTE_CONNECTION where id in (SELECT * FROM removeDuplicateConnections);
	
		--VALIDAT THE CONNECTION IS EXIST IN SAME ROUTE OR NOT
		UPDATE core_planner_fiber_info SET IS_VALID=false, error_msg = 'Other Route Connection'	
		FROM TEMP_ROUTE_CONNECTION A
		WHERE core_planner_fiber_info.FIBER_NUMBER=A.SOURCE_PORT_NO AND A.SOURCE_ENTITY_TYPE='Cable'
		AND core_planner_fiber_info.CABLE_ID = A.SOURCE_SYSTEM_ID
		AND A.DESTINATION_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) and A.DESTINATION_ENTITY_TYPE='Cable' and core_planner_fiber_info.user_id = p_user_id and case when SC_NETWORK_ID is not null then A.connection_network_id not in (SC_NETWORK_ID) else 1=1 end; 

		UPDATE core_planner_fiber_info SET IS_VALID=false, error_msg = 'Other Route Connection'	
		FROM TEMP_ROUTE_CONNECTION A
		WHERE core_planner_fiber_info.FIBER_NUMBER=A.DESTINATION_PORT_NO 
		AND A.DESTINATION_ENTITY_TYPE='Cable' AND core_planner_fiber_info.CABLE_ID = A.DESTINATION_SYSTEM_ID
		AND A.SOURCE_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) and A.SOURCE_ENTITY_TYPE='Cable' and core_planner_fiber_info.user_id = p_user_id and case when SC_NETWORK_ID is not null then A.connection_network_id not in (SC_NETWORK_ID) else 1=1 end; 
	
        --update TEMP_ISP_PORT_INFO is_valid FALSE which already connected to others cable

		UPDATE TEMP_ISP_PORT_INFO SET IS_VALID=FALSE
		FROM TEMP_ROUTE_CONNECTION A
		WHERE TEMP_ISP_PORT_INFO.PORT_NUMBER = A.SOURCE_PORT_NO AND A.SOURCE_ENTITY_TYPE='FMS' 
		AND  A.DESTINATION_ENTITY_TYPE='Cable' AND 
		TEMP_ISP_PORT_INFO.parent_system_id = A.SOURCE_SYSTEM_ID
		AND A.DESTINATION_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES); 

		UPDATE TEMP_ISP_PORT_INFO SET IS_VALID=FALSE
		FROM TEMP_ROUTE_CONNECTION A
		WHERE  TEMP_ISP_PORT_INFO.PORT_NUMBER = A.DESTINATION_PORT_NO 
		AND A.DESTINATION_ENTITY_TYPE='FMS'
		AND A.SOURCE_ENTITY_TYPE='Cable' AND TEMP_ISP_PORT_INFO.parent_system_id 
		= A.DESTINATION_SYSTEM_ID
		AND A.SOURCE_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES); 	
	
	   UPDATE TEMP_ISP_PORT_INFO tpi SET IS_VALID = FALSE
       FROM (
	    SELECT adi.fiber_number, subqu.parent_system_id,subqu.DESTINATION_port_no
	    FROM (
	         SELECT distinct A.SOURCE_SYSTEM_ID,A.source_port_no ,
	        A.DESTINATION_SYSTEM_ID as parent_system_id,A.DESTINATION_port_no 
	        FROM TEMP_ROUTE_CONNECTION A
	        JOIN TEMP_ISP_PORT_INFO
	          ON --TEMP_ISP_PORT_INFO.PORT_NUMBER = A.DESTINATION_PORT_NO
	          TEMP_ISP_PORT_INFO.parent_system_id = A.DESTINATION_SYSTEM_ID 
	          AND A.DESTINATION_ENTITY_TYPE = 'FMS'
	         AND A.SOURCE_ENTITY_TYPE = 'Cable'
	        WHERE A.SOURCE_SYSTEM_ID IN (
	            SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES
	        ) 
	    ) subqu
	    JOIN att_details_cable_info adi
	      ON subqu.SOURCE_SYSTEM_ID = adi.cable_id and subqu.source_port_no = adi.fiber_number
	    WHERE adi.link_system_id <> 0
		  ) outqu
		WHERE tpi.parent_system_id = outqu.parent_system_id
		  AND tpi.PORT_NUMBER = outqu.DESTINATION_port_no;

	  UPDATE TEMP_ISP_PORT_INFO tpi SET IS_VALID = FALSE
       FROM (
	    SELECT adi.fiber_number, subqu.parent_system_id,subqu.source_port_no
	    FROM (
	        SELECT distinct A.DESTINATION_SYSTEM_ID,A.DESTINATION_port_no,
	        A.SOURCE_SYSTEM_ID as parent_system_id,A.source_port_no 
	        FROM TEMP_ROUTE_CONNECTION A
	        JOIN TEMP_ISP_PORT_INFO
	          ON TEMP_ISP_PORT_INFO.parent_system_id = A.SOURCE_SYSTEM_ID 
	          AND A.SOURCE_ENTITY_TYPE = 'FMS'
	         AND A.DESTINATION_ENTITY_TYPE = 'Cable'
	        WHERE A.DESTINATION_SYSTEM_ID IN (
	            SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES
	        ) 
	    ) subqu
	    JOIN att_details_cable_info adi
	      ON subqu.DESTINATION_SYSTEM_ID = adi.cable_id and subqu.DESTINATION_port_no = adi.fiber_number
	    WHERE adi.link_system_id <> 0
	  ) outqu
	  WHERE tpi.parent_system_id = outqu.parent_system_id
	  AND tpi.PORT_NUMBER = outqu.source_port_no;

		UPDATE TEMP_ISP_PORT_INFO tpi SET IS_VALID = false 
		from (SELECT t1.*
		FROM (
		    SELECT DISTINCT 
		        A.DESTINATION_SYSTEM_ID,
		        A.DESTINATION_port_no,
		        A.SOURCE_SYSTEM_ID AS parent_system_id,
		        A.source_port_no 
		    FROM TEMP_ROUTE_CONNECTION A
		    JOIN TEMP_ISP_PORT_INFO
		        ON TEMP_ISP_PORT_INFO.parent_system_id = A.SOURCE_SYSTEM_ID 
		    WHERE A.SOURCE_ENTITY_TYPE = 'FMS'
		      AND A.DESTINATION_ENTITY_TYPE = 'Cable'
		      AND A.DESTINATION_SYSTEM_ID IN (
		          SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES
		      )
		) t1
		LEFT JOIN CORE_PLANNER_FIBER_INFO inf
		    ON inf.fiber_number = t1.DESTINATION_port_no
		   AND inf.cable_id = t1.DESTINATION_SYSTEM_ID
		WHERE inf.fiber_number IS null) tp where tp.parent_system_id = tpi.parent_system_id  and tp.source_port_no = tpi.port_number;
	
	UPDATE TEMP_ISP_PORT_INFO tpi SET IS_VALID = false 
		from (SELECT t1.*
		FROM (
		     SELECT distinct A.SOURCE_SYSTEM_ID,A.source_port_no ,
	        A.DESTINATION_SYSTEM_ID as parent_system_id,A.DESTINATION_port_no 
	        FROM TEMP_ROUTE_CONNECTION A
	        JOIN TEMP_ISP_PORT_INFO
	          ON --TEMP_ISP_PORT_INFO.PORT_NUMBER = A.DESTINATION_PORT_NO
	          TEMP_ISP_PORT_INFO.parent_system_id = A.DESTINATION_SYSTEM_ID 
	          AND A.DESTINATION_ENTITY_TYPE = 'FMS'
	         AND A.SOURCE_ENTITY_TYPE = 'Cable'
	        WHERE A.SOURCE_SYSTEM_ID IN (
	            SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES
	        ) 
		) t1
		LEFT JOIN CORE_PLANNER_FIBER_INFO inf
		    ON inf.fiber_number = t1.source_port_no
		   AND inf.cable_id = t1.SOURCE_SYSTEM_ID
		WHERE inf.fiber_number IS null) tp where tp.parent_system_id = tpi.parent_system_id  and tp.DESTINATION_port_no = tpi.port_number;
 	        
		 UPDATE TEMP_ISP_PORT_INFO AS even
		 SET is_valid = false
		 FROM TEMP_ISP_PORT_INFO AS odd
		 WHERE 
	    odd.port_number % 2 = 1
	    AND odd.is_valid = false
	    AND even.port_number = odd.port_number + 1
	    AND even.parent_system_id = odd.parent_system_id
    	 AND even.is_valid = true;
    
	    UPDATE TEMP_ISP_PORT_INFO AS odd
		SET is_valid = false
		FROM TEMP_ISP_PORT_INFO AS even
		WHERE 
	    even.port_number % 2 = 0
	    AND even.is_valid = false
	    AND odd.port_number = even.port_number - 1
	    AND odd.parent_system_id = even.parent_system_id
	    AND odd.is_valid = true;
	   
	 	 INSERT INTO connected_fiberlink_cable (cable_id, fiber_number,link_system_id,
	 	 a_end_status_id,b_end_status_id )
	     select cable_id ,fiber_number,link_system_id,a_end_status_id,b_end_status_id 
	     from att_details_cable_info where cable_id in 
	     (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES);
	
		 update connected_fiberlink_cable set is_valid = false where link_system_id > 0
		 and ( a_end_status_id = 2 or b_end_status_id =2 ) ;
		 
		for v_rec in (select edge_targetid  from TEMP_CABLE_ROUTES order by seq) 
		loop		 		
			
		WITH invalid_sources AS (
        SELECT ci.destination_system_id,
           ci.destination_port_no,
		   ci.destination_entity_type
		    FROM temp_route_connection ci
		    JOIN connected_fiberlink_cable src
		      ON src.cable_id = ci.source_system_id
		     AND src.fiber_number = ci.source_port_no
		     AND src.is_valid = false
		    WHERE ci.source_entity_type = 'Cable' and ci.source_system_id = v_rec.edge_targetid
		)
		UPDATE connected_fiberlink_cable dst
		SET is_valid = false, error_msg ='Occupied Connection'
		FROM invalid_sources t
		WHERE dst.cable_id = t.destination_system_id
		  AND dst.fiber_number = t.destination_port_no
		  AND t.destination_entity_type = 'Cable';
		 
		 WITH invalid_destinations AS (
		       SELECT ci.source_system_id,
		           ci.source_port_no,
		           ci.source_entity_type
		    FROM temp_route_connection ci
		    JOIN connected_fiberlink_cable src
		      ON src.cable_id = ci.destination_system_id
		     AND src.fiber_number = ci.destination_port_no
		     AND src.is_valid = false
		    WHERE ci.destination_entity_type = 'Cable' and ci.destination_system_id = v_rec.edge_targetid
		)
		UPDATE connected_fiberlink_cable dst
		SET is_valid = false,error_msg ='Occupied Connection'
		FROM invalid_destinations t
		WHERE dst.cable_id = t.source_system_id
		  AND dst.fiber_number = t.source_port_no
		  AND t.source_entity_type = 'Cable';
	   
		end loop;
	
		  update CORE_PLANNER_FIBER_INFO t1 set is_valid = false ,error_msg = t2.error_msg 
		  from connected_fiberlink_cable t2 where t1.cable_id = t2.cable_id and 
		  t1.fiber_number = t2.fiber_number and t2.is_valid = false 
		  and t1.user_id =p_user_id;

		 
     	 SELECT (SELECT COUNT(*)FROM (
      	 SELECT parent_system_id  FROM TEMP_ISP_PORT_INFO 
      	 WHERE IS_VALID = true GROUP BY parent_system_id HAVING COUNT(*) >= REQUIRED_CORE
      	  ) AS t1) <> (SELECT COUNT(distinct PARENT_SYSTEM_ID) FROM ISP_PORT_INFO 
      	 WHERE PARENT_SYSTEM_ID IN (P_SOURCE_SYSTEM_ID, P_DESTINATION_SYSTEM_ID)) into Is_AvailableODF;
       
		IF V_IS_VALID and Is_AvailableODF
		THEN
			V_IS_VALID:=FALSE;
			V_MESSAGE:='All the ports of the ODF are connected to different routes, or adjacent ports are not available in the source/destination ODF!';
		END IF;	
	
     if (REQUIRED_CORE = 2) then    
	    WITH CABLECTE AS (
		    SELECT 
		        tcr.edge_targetid AS CABLE_ID,
		        COALESCE(2 * COUNT(next_fiber.fiber_number), 0) AS AVAILABLE_CORE
		    FROM TEMP_CABLE_ROUTES tcr
		    LEFT JOIN CORE_PLANNER_FIBER_INFO adi
		        ON tcr.edge_targetid = adi.cable_id
		        AND adi.USER_ID = P_USER_ID
		        AND adi.link_system_id = 0
		        AND adi.fiber_number % 2 = 1
		    LEFT JOIN CORE_PLANNER_FIBER_INFO next_fiber 
		        ON next_fiber.cable_id = adi.cable_id
		        AND next_fiber.fiber_number = adi.fiber_number + 1
		        AND next_fiber.link_system_id = 0
		    GROUP BY tcr.edge_targetid
		)
		UPDATE TEMP_CABLE_ROUTES 
		SET IS_VALID = FALSE, 
		    MESSAGE = 'Unavailable Core'
		FROM CABLECTE
		WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID = CABLECTE.CABLE_ID 
		  AND CABLECTE.AVAILABLE_CORE < REQUIRED_CORE;
	end if;

		if (REQUIRED_CORE = 1) then
		WITH CABLECTE AS (
		    SELECT 
		        tcr.edge_targetid AS CABLE_ID,
		        SUM(CASE WHEN adi.link_system_id = 0 THEN 1 ELSE 0 END) AS AVAILABLE_CORE
		    FROM CORE_PLANNER_FIBER_INFO adi
		    JOIN TEMP_CABLE_ROUTES tcr ON tcr.edge_targetid = adi.cable_id
		    WHERE adi.USER_ID = P_USER_ID
		    GROUP BY tcr.edge_targetid
		)
		UPDATE TEMP_CABLE_ROUTES 
		SET IS_VALID = FALSE, 
		    MESSAGE = 'Unavailable Core'
		FROM CABLECTE
		WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID = CABLECTE.CABLE_ID 
		  AND CABLECTE.AVAILABLE_CORE < REQUIRED_CORE;
		end if;
	 
		
		IF V_IS_VALID AND EXISTS(SELECT 1 FROM TEMP_CABLE_ROUTES WHERE IS_VALID = FALSE AND
		MESSAGE = 'Unavailable Core')
		THEN
			V_IS_VALID:=FALSE;
			V_MESSAGE:='Required/Adjacent Cores are unavailable or currently connected to other routes!';
		END IF;	
	
	    UPDATE TEMP_CABLE_ROUTES tcr
		SET is_valid = false,
		MESSAGE = 'Required/Adjacent Cores are unavailable or currently connected to other routes!'
		FROM att_details_cable_info adi
		LEFT JOIN CORE_PLANNER_FIBER_INFO cpfi
		    ON cpfi.cable_id = adi.cable_id AND cpfi.is_valid = true and cpfi.user_id = p_user_id
		WHERE tcr.edge_targetid = adi.cable_id
		  AND  cpfi.cable_id IS NULL ;
		
		 if V_IS_VALID AND EXISTS(SELECT 1 FROM TEMP_CABLE_ROUTES WHERE IS_VALID = false)
		then 
			V_IS_VALID := FALSE;
	        V_MESSAGE := 'Required/Adjacent Cores are unavailable or currently connected to other routes!';
		 end if;
		 
	 	IF V_IS_VALID AND EXISTS(SELECT cable_id FROM CORE_PLANNER_FIBER_INFO
              WHERE user_id = p_user_id
			  GROUP BY cable_id
				HAVING SUM(CASE 
             	WHEN is_valid = false AND error_msg = 'Occupied Connection' THEN 1 
             ELSE 0 
           END) = COUNT(*))
		then
		
			V_IS_VALID:=FALSE;
			V_MESSAGE:='Unable to Find Connectivity Cores in the Specified Cables!';
		END IF;
	

		INSERT INTO 
		CORE_PLANNER_LOGS(CABLE_ID, CABLE_NAME, NETWORK_STATUS, TOTAL_CORE, CABLE_LENGTH,ERROR_MSG, IS_VALID, USER_ID,CABLE_NETWORK_ID, AVAIABLE, USED_CORE,B_SYSTEM_ID,A_SYSTEM_ID,a_entity_type ,b_entity_type,SEQ)
		
		SELECT  ATT.SYSTEM_ID, ATT.CABLE_NAME, ATT.NETWORK_STATUS, ATT.TOTAL_CORE, ATT.CABLE_CALCULATED_LENGTH,INFO.MESSAGE, INFO.IS_VALID,
		P_USER_ID,ATT.NETWORK_ID, 
		(SELECT COUNT(*) FROM ATT_DETAILS_CABLE_INFO ACI WHERE ACI.CABLE_ID = ATT.SYSTEM_ID AND (ACI.LINK_SYSTEM_ID=0)) AS AVAILABLE_CORES,
		(SELECT COUNT(*) FROM ATT_DETAILS_CABLE_INFO ACI WHERE ACI.CABLE_ID = INFO.EDGE_TARGETID AND (ACI.LINK_SYSTEM_ID > 0 ) ) AS USED_CORE,
		INFO.B_SYSTEM_ID,INFO.A_SYSTEM_ID,
		INFO.a_entity_type ,INFO.b_entity_type ,INFO.seq 
		FROM ATT_DETAILS_CABLE ATT 
		INNER JOIN TEMP_CABLE_ROUTES INFO ON ATT.SYSTEM_ID = INFO.EDGE_TARGETID AND  INFO.USER_ID = P_USER_ID order by INFO.seq ;
	
		UPDATE CORE_PLANNER_LOGS cpl
		       SET used_core = cpl.used_core + t.used_core_count,
		      avaiable  = avaiable - t.used_core_count
		FROM (
		    SELECT 
		        cable_id, 
		        COUNT(*) AS used_core_count
		    FROM CORE_PLANNER_FIBER_INFO
		    WHERE 
		        user_id = P_USER_ID 
		        AND error_msg = 'Other Route Connection' and is_valid = false
		        AND (a_end_status_id = 2 OR b_end_status_id = 2)
		    GROUP BY cable_id
		) t
		WHERE cpl.cable_id = t.cable_id
		  AND cpl.user_id = P_USER_ID;
						 				 		 
	END IF;
END IF;

RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT V_IS_VALID AS status, V_MESSAGE AS message) row;
END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) TO postgres;


-------------------------------------------------------------------------------
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
   p_odf_network_id character varying;
  v_source_ports character varying;
  v_destination_ports character varying;
 
BEGIN
V_IS_CABLE_A_END:=FALSE;
 --truncate table TEMP_LINE_MASTER;
 --truncate table TEMP_POINT_MASTER;


CREATE TEMP TABLE TEMP_LINE_MASTER
(
SYSTEM_ID INTEGER,
entity_type character varying,
network_id character varying,
SP_GEOMETRY GEOMETRY
)ON COMMIT DROP; 

CREATE TEMP TABLE TEMP_POINT_MASTER
(
ID serial4 NOT NULL,
A_SYSTEM_ID INTEGER,
A_entity_type character varying,
A_network_id character varying,
SP_GEOMETRY GEOMETRY,
SEQ INTEGER
)ON COMMIT DROP;
		
     v_network_id := '';
     v_system_id := 0;
    p_destination_network_id = destination_network_id;
    --truncate table temp_connection restart identity;
    --truncate table connection_log_details;
    -- Create a temporary table for connection data
   CREATE temp TABLE temp_connection (
        id serial4 NOT null,
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
        splicing_source character varying(100),
        is_selected boolean NOT NULL DEFAULT true
    ) ON COMMIT DROP; 
   
     
	 	CREATE TEMP TABLE isp_port_info_tbl(
        system_id int4 NULL,
        network_id varchar NULL,
        parent_system_id int4 NULL,
        parent_network_id varchar NULL,
        port_status_id int4 NULL,
        is_valid boolean DEFAULT true,
        port_number int4 null,
        parent_entity_type varchar NULL
       ) ON COMMIT DROP;
	
	-- TEMP TABLE FOR CPF RESULT --
		create temp table temp_cpf_result
		(
		id serial,
		connection_id integer,
		source_system_id integer,
		source_network_id character varying(100),
		source_entity_type character varying(100),
		source_port_no integer,
		destination_system_id integer,
		destination_network_id character varying(100),
		destination_entity_type character varying(100),
		destination_port_no integer
		) on commit drop;

		create temp table temp_connected_cables
		(
		id serial,
		cable_id integer,
		fiber_number integer
		) on commit drop;

		create temp table connection_log_details
		(
		id serial,
		destination_systemid integer,		
		destination_networkid character varying,source_no integer,
		destination_port_no integer,
		destination_entitytype character varying
		) on commit drop;


     SELECT 
        (result_json->>'status')::BOOLEAN, 
        result_json->>'message'
     INTO v_status, v_message
     FROM fn_get_core_planner_validation(source_network_id, destination_network_id, buffer, 
     required_core, p_user_id) AS result_json;
   
     IF v_status = false  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT v_status AS status, v_message AS message) row;
       
     else
     
     -- check if fiber link does not exist 
     IF NOT EXISTS (SELECT 1 FROM att_details_fiber_link 
      WHERE upper(network_id) = upper(fiber_link_network_id) OR upper(link_id) = upper(fiber_link_network_id)) 
      THEN 
      RETURN QUERY 
      SELECT row_to_json(row) 
      FROM (
        SELECT false AS status, 
               'The FiberLink does not exist. Please enter a valid FiberLink.' AS message) row;
              return ;
     END IF;
    
   --  insert record from core_planner_logs table to temp table TEMP_POINT_MASTER,TEMP_LINE_MASTER
      INSERT INTO TEMP_POINT_MASTER(SEQ,A_SYSTEM_ID,A_network_id,A_entity_type,SP_GEOMETRY  )
      select A.seq, A.a_system_id,P.COMMON_NAME,A.a_entity_type,P.SP_GEOMETRY from (      
      SELECT seq,a_system_id,a_entity_type  FROM core_planner_logs  where user_id = p_user_id and is_valid =TRUE
      union
      SELECT seq,b_system_id,b_entity_type FROM core_planner_logs where user_id = p_user_id and is_valid =TRUE )a
      INNER JOIN point_master P ON P.SYSTEM_ID=A.a_system_id AND P.entity_type=A.a_entity_type;
     
    delete from TEMP_POINT_MASTER t1 WHERE seq > (
    SELECT MIN(seq)
    FROM TEMP_POINT_MASTER t2
    WHERE t1.a_system_id = t2.a_system_id
    AND t1.a_network_id = t2.a_network_id
    );    

		if((SELECT COUNT(*) FROM core_planner_logs where user_id = p_user_id) = 1 AND 
       (SELECT COUNT(*) FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') = 2)
		   then 
		UPDATE TEMP_POINT_MASTER
		SET seq = 
		  case when a_network_id = source_network_id THEN 0     
		    ELSE 1 end  WHERE a_entity_type = 'FMS';
		else 
		UPDATE TEMP_POINT_MASTER
		SET seq = 0
		  where a_entity_type = 'FMS' AND seq = 1 and (SELECT COUNT(*) FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') = 2;
		end if;

      INSERT INTO TEMP_LINE_MASTER(SYSTEM_ID,network_id,entity_type,SP_GEOMETRY)       
      SELECT CABLE_id,CABLE_network_id,'Cable',P.SP_GEOMETRY FROM core_planner_logs A 
      INNER JOIN LINE_master P ON P.SYSTEM_ID=A.CABLE_id AND P.entity_type='Cable'
      where user_id = p_user_id and is_valid =TRUE ; 
           
    --- get fiberlink into p_link_system_id
      select system_id into p_link_system_id from att_details_fiber_link adfl 
      where upper(network_id) = upper(fiber_link_network_id)
      or upper(link_id)=upper(fiber_link_network_id);  
     
    -- check if odf exists in source or destination store value into p_odf_network_id
    IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id)
     AND EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) 
     THEN
      p_odf_network_id = source_network_id;

    ELSIF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
    THEN
      p_odf_network_id = source_network_id;
    ELSIF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id)
    THEN
      p_odf_network_id = destination_network_id;
    END IF;
   	
        FOR rec IN (SELECT * FROM TEMP_POINT_MASTER ORDER BY 
    CASE WHEN (SELECT COUNT(*) FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') = 1
             AND (SELECT seq FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') <> 1
        THEN -seq
        ELSE seq
	    END
	 )
        LOOP									
		    if(rec.a_entity_type='FMS')
		    then

			 SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) 
			  AS IS_CABLE_A_END INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
			  WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) or
			  ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)));				    
										
            raise info 'v_cable_details_left A : %',v_cable_details_left;
                   		    
		    
			WITH recursive table1 AS (
			    -- Simulating Table-1			    
			   ( SELECT parent_system_id,parent_network_id, port_number FROM temp_isp_port_info 
			    where parent_system_id=rec.a_system_id --and port_status_id =1 
			    and is_valid = true and p_odf_network_id = parent_network_id 
			    ORDER BY port_number limit required_core
			    )
			    union all
			    
			    (SELECT parent_system_id,parent_network_id, port_number FROM temp_isp_port_info 
			    where parent_system_id=rec.a_system_id --and port_status_id =1 
			    and is_valid = true and p_odf_network_id <> parent_network_id
			    ORDER BY port_number )
			    
			),
		   fiber_pairs AS (
		   
		   	select * from(SELECT 
			    t1.id AS id, 
			    t1.cable_id,
			    t1.fiber_number AS fiber1,
			    COALESCE(t2.fiber_number, 0) AS fiber2,  -- If no pair, set fiber2 to 0
			    LEAD(t1.cable_id) OVER (PARTITION BY t1.fiber_number ORDER BY t1.cable_id) AS next_cable,
			    CASE 
			        WHEN t2.fiber_number IS NULL THEN 0  -- If pair is missing (no even-numbered fiber for odd)
			        WHEN t2.fiber_number != t1.fiber_number + 1 THEN 0  -- If next fiber is not a valid pair
			        ELSE t1.seq
			    END AS seq 
			FROM core_planner_fiber_info t1
			LEFT JOIN core_planner_fiber_info t2 
			    ON t1.cable_id = t2.cable_id 
			    AND t2.fiber_number = t1.fiber_number + 1  -- Pair with the next fiber (even-numbered)
			    AND t2.user_id = p_user_id and t2.is_valid = true 
			WHERE t1.user_id = p_user_id and t1.is_valid = true 
			ORDER BY t1.id, t1.fiber_number) t where NOT (fiber2 = 0 AND fiber1 % 2 = 0)
			    
			),
			-- Start the chain from every valid fiber pair
			chain_start AS (
			    SELECT 
			        seq,fiber1, fiber2, cable_id,
			        1 AS depth,
			        cable_id AS first_cable
			    FROM fiber_pairs where cable_id = v_cable_details_left.system_id   
			),
			-- Recursively add more cables in the same fiber pair
			recursive_chain AS (
			    SELECT * FROM chain_start
			    UNION ALL
			    SELECT 
			        fs.seq,fs.fiber1, fs.fiber2, fs.cable_id,
			        rc.depth + 1,
			        rc.first_cable
			    FROM fiber_pairs fs
			    JOIN recursive_chain rc
			        ON fs.fiber1 = rc.fiber1
			        AND fs.fiber2 = rc.fiber2
			        AND fs.seq = rc.seq+1  -- move "downward" in cable_id
			),
     	   existing_connections AS (
		    -- Find already connected fibers in TEMP_ROUTE_CONNECTION
		      SELECT SOURCE_SYSTEM_ID,
		        SOURCE_ENTITY_TYPE,
		        SOURCE_PORT_NO,
		        DESTINATION_SYSTEM_ID , 
		        DESTINATION_ENTITY_TYPE , 
		        DESTINATION_PORT_NO, 
		        IS_CABLE_A_END FROM (
		        SELECT  
		        SOURCE_SYSTEM_ID,
		        SOURCE_ENTITY_TYPE,
		        SOURCE_PORT_NO,
		        DESTINATION_SYSTEM_ID , 
		        DESTINATION_ENTITY_TYPE , 
		        DESTINATION_PORT_NO, 
		        IS_CABLE_A_END
		        FROM TEMP_ROUTE_CONNECTION
		        WHERE SOURCE_ENTITY_TYPE = 'FMS' and SOURCE_SYSTEM_ID = rec.a_system_id 
		        and destination_ENTITY_TYPE = 'Cable' and destination_system_id= 
		        v_cable_details_left.SYSTEM_ID and IS_CABLE_A_END = v_cable_details_left.IS_CABLE_A_END 
		        UNION
		        SELECT 
		        DESTINATION_SYSTEM_ID , 
		        DESTINATION_ENTITY_TYPE , 
		        DESTINATION_PORT_NO,
		        SOURCE_SYSTEM_ID,
		        SOURCE_ENTITY_TYPE,
		        SOURCE_PORT_NO,
		        IS_CABLE_B_END	       
		        FROM TEMP_ROUTE_CONNECTION
		        WHERE DESTINATION_ENTITY_TYPE = 'FMS' and DESTINATION_SYSTEM_ID = rec.a_system_id 
		        and SOURCE_ENTITY_TYPE = 'Cable' and source_system_id= 
		        v_cable_details_left.SYSTEM_ID  and IS_CABLE_B_END = v_cable_details_left.IS_CABLE_A_END 
		     ) AS existing
	      ),
			    right_existing_connection as(
			    select * from existing_connections ec where 
			    ec.source_system_id = rec.a_system_id  and  ec.source_port_no not in (select port_number from table1 order by port_number limit required_core) and ec.DESTINATION_SYSTEM_ID = v_cable_details_left.system_id  and p_odf_network_id = (select parent_network_id from table1 limit 1)  			    			
			    ),
			    checking_right_existing_connection as(
			    SELECT port_number
					FROM (
						SELECT b.port_number, 1 AS priority
						FROM existing_connections ec
						JOIN table1 b ON ec.source_system_id = b.parent_system_id
						WHERE ec.DESTINATION_SYSTEM_ID = v_cable_details_LEFT.system_id
						  AND ec.DESTINATION_ENTITY_TYPE = 'Cable'
						  AND ec.DESTINATION_PORT_NO IN (
							  SELECT destination_port_no 
							  FROM connection_log_details 
							  ORDER BY id DESC 
							  LIMIT required_core
						  )
						  AND ec.source_port_no = b.port_number

						UNION ALL

						SELECT b2.port_number, 2 AS priority
						FROM table1 b2
						WHERE NOT EXISTS (
							SELECT 1
							FROM existing_connections ec
							WHERE ec.source_system_id = b2.parent_system_id
							  AND ec.DESTINATION_SYSTEM_ID = v_cable_details_LEFT.system_id
							  AND ec.DESTINATION_ENTITY_TYPE = 'Cable'
							  AND ec.DESTINATION_PORT_NO IN (
								  SELECT destination_port_no 
								  FROM connection_log_details 
								  ORDER BY id DESC 
								  LIMIT required_core
							  )
							  AND ec.source_port_no = b2.port_number
						) and port_number not in (select source_port_no from existing_connections ec where 
			    ec.source_system_id =  rec.a_system_id and ec.DESTINATION_port_no not in (
		                    SELECT destination_port_no 
		                    FROM connection_log_details 
		                    ORDER BY id DESC 
		                    LIMIT required_core ) and ec.DESTINATION_SYSTEM_ID = v_cable_details_LEFT.system_id  ) ) AS combined
					ORDER BY priority, port_number
					LIMIT required_core

			    ),
			    
			    
			fiber_availability AS (
		    SELECT  fiber1,fiber2, first_cable, continuous_count from 
		   (SELECT  fiber1,fiber2, first_cable, COUNT(*) AS continuous_count
            FROM recursive_chain
            GROUP BY fiber1, fiber2, first_cable
            ORDER BY continuous_count DESC, fiber1 
			) t1 where (t1.fiber1 in				
				(
				select b.DESTINATION_port_no from table1 a
				inner join right_existing_connection b on (a.port_number=b.source_port_no
				and a.parent_network_id = p_odf_network_id 	)
				)
				or 				
				t1.fiber1 not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b 
				)
				)
				AND 
				( ( required_core = 1 AND (t1.fiber2 IS NULL OR t1.fiber2 = 0) ) or 
				t1.fiber2 in				
				(
				(select b.DESTINATION_port_no from table1 a
				inner join right_existing_connection b on (a.port_number=b.source_port_no
				and a.parent_network_id = p_odf_network_id 	)) 
				) 
				or 				
				t1.fiber2 not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b
				)
				)
				ORDER BY t1.continuous_count DESC, t1.fiber1
           ),
           
			match_avialibility AS (
			select COALESCE( (select inf.continuous_count from (select * from table1 
			order by port_number limit 1) t1 join 
			fiber_availability inf on inf.fiber1 = t1.port_number or inf.fiber2 = t1.port_number+1
			where t1.parent_network_id = p_odf_network_id and inf.fiber1 % 2 = 1),0) >=
			 COALESCE((SELECT MAX(continuous_count) FROM fiber_availability ), 0) 
			 as higher_match_avialibility
			),
			exact_match AS (
			SELECT COUNT(*) = required_core AS is_match
			FROM (
			    SELECT distinct *
			    FROM table1 t1
			    JOIN core_planner_fiber_info inf 
			        ON inf.fiber_number = t1.port_number
			    WHERE inf.cable_id = v_cable_details_left.system_id and inf.user_id = p_user_id  and inf.is_valid = true
			      and true = (select higher_match_avialibility from match_avialibility)
			 ) AS selected_fibers_match
			),
			downward_match AS (
			   -- Rank fiber pairs based on availability (highest first), then by fiber1 descending
			  SELECT  fiber1, fiber2, continuous_count,
			  RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
			  FROM fiber_availability F
			  JOIN table1 d  -- Properly referencing the threshold
		      on f.fiber1 > d.port_number where f.fiber1 % 2 = 1 
			 ),	
			upward_match AS (
				   -- Rank fiber pairs based on availability (highest first), then by fiber1 descending
		      SELECT  fiber1, fiber2, continuous_count,
			  RANK() OVER (ORDER BY continuous_count DESC, fiber1 asc) AS rank
			  FROM fiber_availability F
			  JOIN table1 d  -- Properly referencing the threshold
              on f.fiber1 <= d.port_number where f.fiber1 % 2 = 1  and  NOT EXISTS (
	        -- Check if an exact match is available
	        SELECT 1
	        FROM downward_match 
	        ) ),
	  cable_connection_squence as(
				    (select source_port_no,DESTINATION_PORT_NO from existing_connections where source_port_no in (select port_number from table1) and  destination_system_id = v_cable_details_left.system_id and (select parent_network_id from table1 limit 1)= p_odf_network_id )
			 union 
		(select source_port_no,DESTINATION_PORT_NO from existing_connections where destination_port_no in 	(select destination_port_no from connection_log_details order by id desc limit required_core)
		 and  MOD(source_port_no, 2) <> MOD(destination_port_no, 2) and destination_system_id = v_cable_details_left.system_id and (select parent_network_id from table1 limit 1) <> p_odf_network_id)
				    
				    ),

		   matched_fibers AS (			
			SELECT DISTINCT 
			    t1.parent_system_id, 
			    t1.parent_network_id, 
			    t1.port_number,
			    t2.cable_id, 
			    v_cable_details_left.network_id AS network_id,
			    t2.fiber_number, 
			    v_cable_details_left.IS_CABLE_A_END AS IS_CABLE_A_END,
			    ROW_NUMBER() OVER (
			        PARTITION BY t1.port_number 
			        ORDER BY t2.fiber_number
			    ) AS port_rank
				from (SELECT * FROM table1 WHERE
                CASE 
                WHEN p_odf_network_id <> parent_network_id THEN 
                port_number in (select * from checking_right_existing_connection)
                ELSE TRUE
            END
        ORDER BY port_number
        LIMIT required_core) t1
					JOIN core_planner_fiber_info t2 
					    ON t2.cable_id = v_cable_details_left.system_id 
					     and t2.user_id = p_user_id and t2.is_valid = true
					LEFT JOIN exact_match em ON TRUE
					WHERE (  case 
					            WHEN p_odf_network_id <> t1.parent_network_id THEN 
		                t2.fiber_number IN (
		                    SELECT destination_port_no 
		                    FROM connection_log_details 
		                    ORDER BY id DESC 
		                    LIMIT required_core )
		            WHEN em.is_match = TRUE THEN 
		                t2.fiber_number = t1.port_number
		                else
		                t2.fiber_number IN (
		                    SELECT fiber1 FROM downward_match WHERE rank = 1
		                    UNION
		                    SELECT fiber2 FROM downward_match WHERE rank = 1
		                    UNION 
		                    SELECT fiber1 FROM upward_match WHERE rank = 1
		                    UNION
		                    SELECT fiber2 FROM upward_match WHERE rank = 1 )
		        END )
		
				ORDER BY t1.port_number		
				--LIMIT required_core
              ),
              
             final_result AS (
               SELECT * from (SELECT *, 
               ROW_NUMBER() OVER (
               PARTITION BY fiber_number 
               ORDER BY port_number
                ) AS fiber_rank
                FROM matched_fibers )t WHERE case when (select count(1) from cable_connection_squence) <> 1
                then
                t.port_rank =  t.fiber_rank 
                else 
                (t.port_number = (select source_port_no from cable_connection_squence)  and t.fiber_number = (select destination_port_no from cable_connection_squence))
    	           or 
    	          ( t.port_number <> (select source_port_no from cable_connection_squence) and 
    	           t.fiber_number <> (select destination_port_no from cable_connection_squence)) 
                end                             
             ),	
          
			check_existing AS (
		    -- Check if ANY fiber in final_result is already in TEMP_ROUTE_CONNECTION
	        SELECT  source_system_id,source_entity_type,source_port_no,is_cable_a_end,
		    destination_system_id, DESTINATION_PORT_NO from 
		    (SELECT e.source_system_id,e.source_entity_type,e.source_port_no,e.is_cable_a_end,
		    e.destination_system_id, e.DESTINATION_PORT_NO
		    FROM (select * from final_result ) fr1
		    JOIN existing_connections e
		      ON e.source_system_id = fr1.parent_system_id and
				    e.destination_system_id = v_cable_details_left.system_id and 
				    (e.DESTINATION_PORT_NO = fr1.fiber_number or e.source_port_no = fr1.port_number)
			)t	   
			
			),			
			filtered_final_result as(
			 SELECT fr.* FROM (select * from final_result ) fr
				LEFT JOIN check_existing ce 
				    ON ce.source_system_id = fr.parent_system_id AND ce.source_port_no = fr.port_number				    
				WHERE ce.source_port_no IS null 
				limit required_core
			),
				final_insert AS (
		    -- Insert into temp_connection only if NOT in check_existing
		    INSERT INTO temp_connection (
		        source_system_id, source_network_id, source_entity_type, source_port_no, 
		        destination_system_id, destination_network_id, destination_entity_type,
		        destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
		        created_by, splicing_source
		    ) 
			    SELECT 
			        fr.parent_system_id, fr.parent_network_id, 'FMS', fr.port_number, 
			        fr.cable_id, fr.network_id, 'Cable', fr.fiber_number, 
			        FALSE, fr.IS_CABLE_A_END, p_user_id, 'CorePlanning'
			    FROM filtered_final_result fr
			    ORDER BY fr.port_number
			    LIMIT required_core
			),
			insert_log_filtered AS (
		    -- Insert records from filtered_final_result
		   INSERT INTO connection_log_details (
			    destination_systemid, destination_networkid, destination_port_no, destination_entitytype
			)
			SELECT 
			    fr.cable_id, fr.network_id, fr.fiber_number, 'Cable'
			FROM filtered_final_result fr
			 where fr.cable_id = v_cable_details_left.SYSTEM_ID
		    LIMIT required_core
		  )
     	  INSERT INTO connection_log_details (
       		 destination_systemid, destination_networkid, destination_port_no, destination_entitytype
		    )
		    SELECT 
		        distinct destination_system_id, NULL, DESTINATION_PORT_NO, 'Cable'
		    FROM check_existing ce
		    where ce.destination_system_id = v_cable_details_left.system_id	 ;
		   -- ORDER BY source_port_no;
	  
		else		

			SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
			AS IS_CABLE_A_END 
			INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
			WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
			OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) 
			and a.system_id in
		   (select destination_systemid from connection_log_details order by id desc limit 1)  LIMIT 1;
				
			SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) 
			AS IS_CABLE_A_END 
			INTO v_cable_details_right FROM TEMP_LINE_MASTER A				
			WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
			OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) 
			AND SYSTEM_ID NOT IN(v_cable_details_left.SYSTEM_ID)  LIMIT 1;

		 raise info 'v_cable_details_left 1 : %',v_cable_details_left;


		if (v_cable_details_left is not null and v_cable_details_right is not null)
		then
            raise info 'v_cable_details_right 1 : %',v_cable_details_right;
     

			WITH recursive table1 AS (
				SELECT cable_id, fiber_number,v_cable_details_left.network_id as network_id,
				v_cable_details_left.IS_CABLE_A_END as IS_CABLE_A_END
				FROM core_planner_fiber_info where cable_id = v_cable_details_left.system_id				
				and is_valid = true  and fiber_number IN 
				(SELECT destination_port_no FROM connection_log_details t1 
				order by t1.id  desc limit required_core) and core_planner_fiber_info.user_id =p_user_id order by fiber_number  
				limit required_core 				
			) ,
			  fiber_pairs AS (			
			select * from(SELECT 
			    t1.id AS id, 
			    t1.cable_id,
			    t1.fiber_number AS fiber1,
			    COALESCE(t2.fiber_number, 0) AS fiber2,  -- If no pair, set fiber2 to 0
			    LEAD(t1.cable_id) OVER (PARTITION BY t1.fiber_number ORDER BY t1.cable_id) AS next_cable,
			    CASE 
			        WHEN t2.fiber_number IS NULL THEN 0  -- If pair is missing (no even-numbered fiber for odd)
			        WHEN t2.fiber_number != t1.fiber_number + 1 THEN 0  -- If next fiber is not a valid pair
			        ELSE t1.seq
			    END AS seq 
			FROM core_planner_fiber_info t1
			LEFT JOIN core_planner_fiber_info t2 
			    ON t1.cable_id = t2.cable_id 
			    AND t2.fiber_number = t1.fiber_number + 1  -- Pair with the next fiber (even-numbered)
			    AND t2.user_id = p_user_id and t2.is_valid = true 
			WHERE t1.user_id = p_user_id and t1.is_valid = true 
			ORDER BY t1.id, t1.fiber_number) t where NOT (fiber2 = 0 AND fiber1 % 2 = 0)
		),
				-- Start the chain from every valid fiber pair
				chain_start AS (
				    SELECT 
				        seq,fiber1, fiber2, cable_id,
				        1 AS depth,
				        cable_id AS first_cable
				    FROM fiber_pairs where cable_id = v_cable_details_right.system_id  
				),
				-- Recursively add more cables in the same fiber pair
				recursive_chain AS (
					SELECT * FROM chain_start
					UNION ALL
					SELECT fs.seq,fs.fiber1, fs.fiber2, fs.cable_id,
					rc.depth + 1,rc.first_cable
					FROM fiber_pairs fs
					JOIN recursive_chain rc
					ON fs.fiber1 = rc.fiber1
					AND fs.fiber2 = rc.fiber2
					AND fs.seq = rc.seq+1 
					),	
				  existing_connections AS (
			    -- Find already connected fibers in TEMP_ROUTE_CONNECTION
			      SELECT SOURCE_SYSTEM_ID,
			        SOURCE_ENTITY_TYPE,
			        SOURCE_PORT_NO,
			        DESTINATION_SYSTEM_ID , 
			        DESTINATION_ENTITY_TYPE , 
			        DESTINATION_PORT_NO, 
			        IS_CABLE_A_END FROM (
			        SELECT  
			        SOURCE_SYSTEM_ID,
			        SOURCE_ENTITY_TYPE,
			        SOURCE_PORT_NO,
			        DESTINATION_SYSTEM_ID , 
			        DESTINATION_ENTITY_TYPE , 
			        DESTINATION_PORT_NO, 
			        IS_CABLE_A_END
			        FROM TEMP_ROUTE_CONNECTION
			        WHERE SOURCE_ENTITY_TYPE = 'Cable' and SOURCE_SYSTEM_ID = v_cable_details_left.system_id  and IS_CABLE_A_END = v_cable_details_left.IS_CABLE_A_END 
			        and destination_system_id=  v_cable_details_right.system_id 
			        UNION
			        SELECT 
			        DESTINATION_SYSTEM_ID , 
			        DESTINATION_ENTITY_TYPE , 
			        DESTINATION_PORT_NO,
			        SOURCE_SYSTEM_ID,
			        SOURCE_ENTITY_TYPE,
			        SOURCE_PORT_NO,
			        IS_CABLE_B_END	       
			        FROM TEMP_ROUTE_CONNECTION
			        WHERE DESTINATION_ENTITY_TYPE = 'Cable' and DESTINATION_SYSTEM_ID = v_cable_details_left.system_id  and IS_CABLE_B_END = v_cable_details_left.IS_CABLE_A_END 
			        and SOURCE_SYSTEM_ID= v_cable_details_right.system_id
			     ) AS existing
			    ),
			    right_existing_connection as(
			    select * from existing_connections  ec where ec.SOURCE_SYSTEM_ID = v_cable_details_left.system_id and SOURCE_ENTITY_TYPE ='Cable' AND source_PORT_NO not in (SELECT destination_port_no FROM connection_log_details t1 
				order by t1.id  desc limit required_core) and ec.DESTINATION_SYSTEM_ID = v_cable_details_right.system_id
			    ),			     
				fiber_availability AS (
			    SELECT distinct  fiber1,fiber2, first_cable, continuous_count from (  
			    SELECT  fiber1,fiber2, first_cable, COUNT(*) AS continuous_count
	            FROM recursive_chain
	            GROUP BY fiber1, fiber2, first_cable
	            ORDER BY continuous_count DESC, fiber1 
				) t1 where (t1.fiber1 in				
				(
				select b.DESTINATION_port_no from table1 a
				inner join right_existing_connection b on a.fiber_number=b.SOURCE_PORT_NO 		
				)
				or 				
				t1.fiber1 not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b
				)
				)
				AND 
				(t1.fiber2 in				
				(
				select b.DESTINATION_port_no from table1 a
				inner join right_existing_connection b on a.fiber_number=b.SOURCE_PORT_NO			
				) 
				or 				
				t1.fiber2 not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b
				)
				) or t1.fiber2 IS NULL OR t1.fiber2 = 0
				ORDER BY t1.continuous_count DESC, t1.fiber1  
				),
				selected_fibers_match AS (
			    -- Step 3: Select the fiber availability where fiber1 is odd
			    SELECT distinct t2.FIBER_number,t1.FIBER_number as C1_FIBER_NUMBER
			    FROM table1 t1  
			    JOIN core_planner_fiber_info t2  
			        ON t2.FIBER_number = t1.FIBER_number and t2.cable_id  = v_cable_details_right.system_id and t2.user_id = p_user_id and t2.is_valid = true
			    WHERE  t2.FIBER_number in				
				(
				select a.fiber_number from table1 a
				inner join right_existing_connection b on a.fiber_number=b.source_port_no			
				)
				or 				
				t2.FIBER_number not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b
				)
					
			    order by t1.FIBER_number 
			   ),
				exact_match AS (
		        SELECT COUNT(*) = required_core AS is_match
		        FROM selected_fibers_match
		        ),
					downward_match AS (						
				    SELECT distinct fiber1, fiber2, continuous_count,
				    RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
				    FROM fiber_availability f
                    JOIN table1 d  -- Properly referencing the threshold
                   on f.fiber1 > d.fiber_number where f.fiber1 % 2 = 1 
				),
					upward_match AS (
				   -- Rank fiber pairs based on availability (highest first), then by fiber1 descending
				    SELECT distinct fiber1, fiber2, continuous_count,
				    RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
				    FROM fiber_availability F
				    JOIN table1 d  -- Properly referencing the threshold
                    on f.fiber1 <= d.fiber_number where f.fiber1 % 2 = 1  and NOT EXISTS (
				        SELECT 1
				        FROM downward_match 
				    )) ,
				    cable_connection_squence as(
				    select source_port_no,DESTINATION_PORT_NO from existing_connections where SOURCE_PORT_NO in (select destination_port_no from connection_log_details order by id desc limit required_core) and  (MOD(source_port_no, 2) <> MOD(destination_port_no, 2))
				    ),
		 
		      matched_fibers AS (
				SELECT t1.cable_id as c1_cable_id, t1.network_id as c1_network_id,
				t1.fiber_number as c1_fiber_number,t1.IS_CABLE_A_END as C1_IS_CABLE_A_END,
				t2.cable_id as c2_cable_id,v_cable_details_right.network_id as c2_network_id,
				t2.fiber_number as c2_fiber_number, v_cable_details_right.IS_CABLE_A_END  
				as C2_IS_CABLE_A_END ,
				ROW_NUMBER() OVER (
	            PARTITION BY t1.fiber_number 
	            ORDER BY t2.fiber_number
	            ) AS fiber1_rank
			    FROM table1 t1
			    JOIN core_planner_fiber_info t2 ON  t2.cable_id = v_cable_details_right.system_id  			   
				and t2.user_id = p_user_id 	and t2.is_valid = true	
			    LEFT JOIN exact_match em 
                    ON TRUE
                WHERE 
       		     (CASE 
        		    WHEN em.is_match = TRUE THEN 
        		    case when (select count(1) from cable_connection_squence) = 1 then 
        		     (t1.fiber_number = (select source_port_no from cable_connection_squence)  and t2.fiber_number = (select destination_port_no from cable_connection_squence))
    	           or 
    	           (t2.fiber_number = (select source_port_no from cable_connection_squence) and 
    	           t1.fiber_number = (select destination_port_no from cable_connection_squence))
    	           else
    	            t2.fiber_number = t1.fiber_number end
            	    --AND t2.fiber_number = t1.fiber_number + 1
           		 ELSE 
                t2.fiber_number IN (
                    SELECT fiber1 FROM downward_match WHERE rank = 1
                    UNION
                    SELECT fiber2 FROM downward_match WHERE rank = 1
                    union 
                    SELECT fiber1 FROM upward_match WHERE rank = 1
                    UNION
                    SELECT fiber2 FROM upward_match WHERE rank = 1
                ) 
       			 END  
       			 )
                order by t1.fiber_number --limit required_core
			    
	    	),
	    	
	     final_result AS (
               SELECT * from (SELECT *, 
               ROW_NUMBER() OVER (
               PARTITION BY c2_fiber_number 
               ORDER BY c1_fiber_number
                ) AS fiber2_rank
                FROM matched_fibers)t  WHERE t.fiber1_rank =  t.fiber2_rank
                ),

			check_existing AS (
		    -- Check if ANY fiber in final_result is already in TEMP_ROUTE_CONNECTION
		    SELECT source_system_id,source_entity_type,source_port_no,is_cable_a_end,
		    destination_system_id, DESTINATION_PORT_NO from (SELECT e.source_system_id,e.source_entity_type,e.source_port_no,e.is_cable_a_end,
		    e.destination_system_id, e.DESTINATION_PORT_NO
		    FROM (select * from final_result LIMIT required_core) fr1
		    JOIN existing_connections e
		      ON e.source_system_id = fr1.c1_cable_id 
		     where e.source_port_no = fr1.c1_fiber_number 
		     AND e.is_cable_a_end = fr1.C1_IS_CABLE_A_END) t where t.destination_system_id = v_cable_details_right.system_id --and t.DESTINATION_PORT_NO = c2_fiber_number
			),			
			
			filtered_final_result as(
			 SELECT fr.* FROM (select * from final_result limit required_core ) fr
				LEFT JOIN check_existing ce 
				    ON (ce.source_system_id = fr.c1_cable_id AND ce.source_port_no = fr.c1_fiber_number) 				    
				WHERE ce.source_port_no IS NULL
			),

			final_insert AS (
			-- Final selection: prioritize exact match, then downward, then upward
			INSERT INTO temp_connection (
            source_system_id, source_network_id, source_entity_type, source_port_no, 
            destination_system_id, destination_network_id, destination_entity_type,
            destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
            created_by, splicing_source,equipment_system_id, equipment_network_id, equipment_entity_type)
				
			SELECT c1_cable_id,c1_network_id,'Cable', c1_fiber_number,
			c2_cable_id ,c2_network_id,'Cable',c2_fiber_number,c1_IS_CABLE_A_END,c2_IS_CABLE_A_END,
			p_user_id, 'CorePlanning',rec.a_system_id,rec.a_network_id,rec.a_entity_type
			FROM filtered_final_result				  
			ORDER BY c1_fiber_number 
		),
		insert_log_filtered AS (
		    -- Insert records from filtered_final_result
		    INSERT INTO connection_log_details (
		        destination_systemid, destination_networkid, destination_port_no, destination_entitytype
		    )
		    SELECT 
		        fr.c2_cable_id, fr.c2_network_id, fr.c2_fiber_number, 'Cable'
		    FROM filtered_final_result fr
		    ORDER BY c2_fiber_number 
		    LIMIT required_core
		)
     	  INSERT INTO connection_log_details (
       		 destination_systemid, destination_networkid, destination_port_no, destination_entitytype
		    )
		    SELECT 
		        distinct destination_system_id, NULL, DESTINATION_PORT_NO, 'Cable'
		    FROM existing_connections ce
		    where ce.source_system_id = v_cable_details_left.system_id	and source_port_no in (select destination_port_no from connection_log_details order by id desc limit required_core)
		    and ce.destination_system_id = v_cable_details_right.system_id;    
		    --ORDER BY source_port_no;
		end if;	  
	   
	end if;
        END LOOP;

		if (SELECT COUNT(*) FROM temp_connection) > 0
		then 
		 raise info 'end splicing 1 : %',p_user_id;
	    perform(fn_auto_provisioning_save_connections());
		
		
		end if;	
	
		update att_details_cable_info  set a_end_status_id=2
		from temp_connection tc
		where cable_id=tc.source_system_id and tc.source_entity_type='Cable'
		and fiber_number=tc.source_port_no
		and tc.is_source_cable_a_end =true;
		
		update att_details_cable_info  set b_end_status_id=2 
		from temp_connection tc
		where cable_id=tc.source_system_id and tc.source_entity_type='Cable'
		and fiber_number=tc.source_port_no
		and tc.is_source_cable_a_end=false;
		
		update att_details_cable_info set a_end_status_id=2 
		from temp_connection tc
		where cable_id=tc.destination_system_id and tc.destination_entity_type='Cable'
		and fiber_number=tc.destination_port_no
		and tc.is_destination_cable_a_end=true;
		
		update att_details_cable_info  set b_end_status_id=2 
		from temp_connection tc
		where cable_id=tc.destination_system_id and tc.destination_entity_type='Cable'
		and fiber_number=tc.destination_port_no
		and tc.is_destination_cable_a_end=false;
	
	    update isp_port_info  set port_status_id=2 
		from temp_connection tc
		where parent_system_id=tc.destination_system_id 
		and upper(parent_entity_type)=upper(tc.destination_entity_type)
		and port_number = tc.destination_port_no ;
	
	    update isp_port_info  set port_status_id=2 
		from temp_connection tc
		where parent_system_id=tc.source_system_id 
		and upper(parent_entity_type)=upper(tc.source_entity_type)
		and port_number = tc.source_port_no ;

		insert into temp_connected_cables(cable_id,fiber_number)
		select destination_systemid, destination_port_no from connection_log_details;					
					
		update att_details_cable_info set  fiber_status ='Reserved',
		link_system_id=coalesce(p_link_system_id,0)
		from temp_connected_cables b
		where att_details_cable_info.cable_id=b.cable_id 
	    and b.fiber_number=att_details_cable_info.fiber_number;


  		update att_details_fiber_link set fiber_link_status='Associated',
		gis_length = ROUND(
        COALESCE((SELECT Sum(a.cable_measured_length) FROM att_details_cable a
        inner join TEMP_LINE_MASTER b on a.system_id =b.system_id and b.entity_type ='Cable' ), 0
        )::NUMERIC, 3
    ),
    total_route_length = ROUND(
        COALESCE(
            (SELECT sum(a.cable_calculated_length) FROM att_details_cable a
            inner join TEMP_LINE_MASTER b on a.system_id =b.system_id and b.entity_type ='Cable' ), 0
        )::NUMERIC, 3
    ) 
		where system_id=coalesce(p_link_system_id,0);
		

       return query select row_to_json(row) from ( select true as status, 'The required core has been spliced, and the fiber link has been successfully attached.' as message ) row;
      end if;
  END;
 
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) TO postgres;

