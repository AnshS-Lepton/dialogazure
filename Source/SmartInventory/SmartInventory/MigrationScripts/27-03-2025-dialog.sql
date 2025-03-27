-- FUNCTION: public.fn_get_fiberlink_schematicview(integer)

-- DROP FUNCTION IF EXISTS public.fn_get_fiberlink_schematicview(integer);
CREATE OR REPLACE FUNCTION public.fn_get_ring_schematicview(
	p_ring_id integer	
    )
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
v_from_id integer;
v_from_entity_type character varying;
nodes text;
edges text;
nodesDownstream text;
edgesDownstream text;
legends text;
cables text;
BEGIN

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

insert into cpf_temp_result(via_system_id ,via_network_id ,via_entity_type)
SELECT distinct B.SYSTEM_ID,B.NETWORK_ID,'Cable' FROM top_ring_cable_mapping a
INNER JOIN ATT_DETAILS_CABLE b
ON A.CABLE_ID=B.network_id
where a.ring_id=p_ring_id  ;


update cpf_temp_result set source_system_id=a.system_id,
source_entity_type=a.entity_type,
source_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on --(st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2))
st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2))
and pm.entity_type in('SpliceClosure','FDB','POD','BDB')
where a.source_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and source_system_id is null;

update cpf_temp_result set destination_system_id=a.system_id,
destination_entity_type=a.entity_type,
destination_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id
from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on (st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2)))
and pm.entity_type in('SpliceClosure','FDB','POD','BDB')
where a.destination_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and destination_system_id is null;

--delete from cpf_temp_result where source_network_id=destination_network_id;

 
insert into temp_nodes(source_system_id,source_network_id,source_entity_type)
select source_system_id,source_network_id,source_entity_type  from(
select * from(
select source_system_id,source_network_id,source_entity_type 
	from cpf_temp_result 
union
select destination_system_id,destination_network_id,destination_entity_type 
	from cpf_temp_result)b
	
)t group by source_system_id,source_network_id,source_entity_type;

update temp_nodes set node_name=fms.fms_name
from att_details_fms fms where source_system_id=fms.system_id and source_entity_type='FMS';

update temp_nodes set node_name=fms.spliceclosure_name
from att_details_spliceclosure fms where source_system_id=fms.system_id and source_entity_type='SpliceClosure';

--UPDATE FDB Nikhil Arora
update temp_nodes set node_name=fdb.fdb_name
from isp_fdb_info fdb where source_system_id=fdb.system_id and source_entity_type='FDB';

--END FDB UPDATE

insert into temp_edges(from_id,from_entity_type,
to_id,to_entity_type,label,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length)

	select source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,
network_status,cable_type,color_code,total_core,cable_category,
cable_calculated_length
from(
	select coalesce(source_system_id,cbl.a_system_id)as source_system_id, 
	coalesce(source_entity_type,a_entity_type) as source_entity_type,
coalesce(destination_system_id,b_system_id) as destination_system_id,
coalesce(destination_entity_type,b_entity_type) as destination_entity_type,via_network_id,
cbl.network_status,cbl.cable_type,cbl.total_core,cbl.cable_category,cbls.color_code,
Cast(cbl.cable_calculated_length as decimal(10,2))  as cable_calculated_length
from cpf_temp_result cpf
inner join att_details_cable cbl on cbl.system_id=cpf.via_system_id
left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type 
and cbls.cable_category=cbl.cable_category and cbls.fiber_count=cbl.total_core
)a
group by source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length;


   
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
concat(' (',t.cable_calculated_length::text,' m)') as label,

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
$BODY$;


