CREATE OR REPLACE FUNCTION public.fn_otdr_get_fiber_cut_details(p_entity_system_id integer, p_entity_port_no integer, p_entity_type character varying, p_distance double precision, p_node_type character varying, p_is_backword_path boolean)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
declare
v_main_path geometry;
v_sub_line_start_point geometry;
v_sub_line_end_point geometry;
v_sub_line_geom geometry;
v_line_total_length double precision;
v_cable_calculated_length double precision;
v_faulty_record record;
v_last_termination_point record;
v_input_cable_end_point geometry;
v_main_fiber_end_point geometry;
v_arow record;
v_main_path_geom text;
v_total_loop_count integer;
v_total_loop_length double precision;
v_total_physical_distance double precision;
v_sub_start geometry;
v_sub_end geometry;
v_sub_line geometry;
v_endgeom geometry;
v_a_data_exist character varying;
v_b_data_exist character varying;
BEGIN
v_last_termination_point:=null;

v_a_data_exist :='0';
v_b_data_exist :='0';

--TEMP TABLE TO STORE CONNECTION INFO RESULT..

create temp table cpf_temp_result
(
id serial,
source_system_id integer,
source_network_id character varying(100),
source_entity_type character varying(100),
source_port_no integer,
destination_system_id integer,
destination_network_id character varying(100),
destination_entity_type character varying(100),
destination_port_no integer,
cable_measured_length double precision,
is_backward_path boolean default false,
cable_calculated_length double precision
)on commit drop;

--truncate table temp_fibers;
create temp table temp_fibers
(
id serial,
cable_id integer,
sp_geometry text,
cable_calculated_length double precision,
cable_loop_length double precision
) on commit drop;

create temp table temp_loop
(
id serial,
entity_type character varying,
loop_length double precision
) on commit drop;


insert into cpf_temp_result(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,
destination_port_no,cable_measured_length,cable_calculated_length,is_backward_path)
select a.source_system_id,a.source_network_id,a.source_entity_type,a.source_port_no,a.destination_system_id,a.destination_network_id,a.destination_entity_type,a.destination_port_no,
a.cable_measured_length,a.cable_calculated_length,a.is_backward_path
from fn_otdr_get_fiber_list(p_entity_system_id, p_entity_port_no,p_entity_type,p_node_type) a;




if(p_is_backword_path) then 

insert into temp_fibers(cable_id,sp_geometry)
select source_system_id,lm.sp_geometry from(
select id, source_system_id from cpf_temp_result cpf where upper(cpf.source_entity_type)=upper('CABLE') and is_backward_path=p_is_backword_path
and case when p_is_backword_path=true and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='B' then 1=1
when p_is_backword_path=true and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='A' then 
source_system_id!=p_entity_system_id
when p_is_backword_path=false and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='A' then source_system_id!=p_entity_system_id else 1=1 end
union
select id, destination_system_id from cpf_temp_result cpf where upper(cpf.destination_entity_type)=upper('CABLE') and is_backward_path=p_is_backword_path
and case when p_is_backword_path=true and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='A' then 1=1
when p_is_backword_path=true and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='B' then destination_system_id!=p_entity_system_id
when p_is_backword_path=false and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='B' then destination_system_id!=p_entity_system_id else 1=1 end
)a inner join line_master lm on a.source_system_id=lm.system_id and upper(lm.entity_type)=upper('CABLE') order by a.id ;

else 

insert into temp_fibers(cable_id,sp_geometry)
select source_system_id,lm.sp_geometry from(
select id, source_system_id from cpf_temp_result cpf where upper(cpf.source_entity_type)=upper('CABLE') and is_backward_path=p_is_backword_path
and case when p_is_backword_path=true and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='A' then 1=1
when p_is_backword_path=true and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='B' then 
source_system_id!=p_entity_system_id
when p_is_backword_path=false and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='B' then source_system_id!=p_entity_system_id else 1=1 end
union
select id, destination_system_id from cpf_temp_result cpf where upper(cpf.destination_entity_type)=upper('CABLE') and is_backward_path=p_is_backword_path
and case when p_is_backword_path=true and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='B' then 1=1
when p_is_backword_path=true and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='A' then destination_system_id!=p_entity_system_id
when p_is_backword_path=false and upper(p_entity_type)=upper('CABLE') and coalesce(p_node_type,'')='A' then destination_system_id!=p_entity_system_id else 1=1 end
)a inner join line_master lm on a.source_system_id=lm.system_id and upper(lm.entity_type)=upper('CABLE') order by a.id ;

end if;


DELETE FROM temp_fibers T1 USING temp_fibers T2
WHERE T1.id < T2.id and t1.cable_id=t2.cable_id;





-- UPDATE temp_fibers AS v
-- SET cable_calculated_length = ((Cast(s.cable_calculated_length as decimal(10,2)))+ (Cast(s.total_loop_length as decimal(10,2))))
-- FROM att_details_cable AS s
-- WHERE v.cable_id = s.system_id ;

UPDATE temp_fibers AS v
SET cable_calculated_length = (Cast(s.cable_calculated_length as decimal(10,2))),
cable_loop_length=(Cast(s.total_loop_length as decimal(10,2)))
FROM att_details_cable AS s
WHERE v.cable_id = s.system_id ;



IF UPPER(p_entity_type) != 'CABLE'
THEN
SELECT sp_geometry INTO v_endgeom FROM point_master WHERE entity_type=p_entity_type AND system_id=p_entity_system_id;

END IF;

v_line_total_length:= p_distance;

FOR v_arow in SELECT * FROM temp_fibers
loop

raise info 'v_endgeom :%',st_astext(v_endgeom);

raise info 'Distance calculated: %',(SELECT ST_Distance(v_endgeom,st_startpoint(v_arow.sp_geometry)));

raise info 'v_arow 123 :%',v_arow;
v_faulty_record:=v_arow;
raise info'% v_arow.cable_loop_length',v_arow.cable_loop_length;
IF(v_endgeom is not null)
THEN
RAISE INFO ' % v_endgeom_not_cable', v_endgeom;

IF(SELECT ST_Distance(v_endgeom,st_startpoint(v_arow.sp_geometry))>0)
THEN
RAISE INFO ' % ST_Distance', (SELECT ST_Distance(v_endgeom,st_startpoint(v_arow.sp_geometry)));
v_arow.sp_geometry:= st_reverse(v_arow.sp_geometry);

END IF;

v_endgeom:= st_endpoint(v_arow.sp_geometry);
else

if(p_is_backword_path=true ) then 
v_arow.sp_geometry:= st_reverse(v_arow.sp_geometry); 
end if;
raise info 'v_arow.sp_geometry :%',St_astext(v_arow.sp_geometry);

v_endgeom:= st_endpoint(v_arow.sp_geometry);

END IF;


raise info'% v_line_total_length',v_line_total_length;
raise info'% v_arow.cable_calculated_length',v_arow.cable_calculated_length;
raise info'% v_arow.cable_loop_length',v_arow.cable_loop_length;


IF(CAST(v_line_total_length AS DECIMAL(10,2)) > (v_arow.cable_calculated_length + (Cast(v_arow.cable_loop_length as decimal(10,2)))))
THEN


v_arow.cable_calculated_length:= ((Cast(v_arow.cable_calculated_length as decimal(10,2)))+ (Cast(v_arow.cable_loop_length as decimal(10,2))));
v_total_physical_distance:= CAST(st_length(v_arow.sp_geometry::geometry,false)AS DECIMAL(10,2))+COALESCE(v_total_physical_distance,0);
v_line_total_length:= (v_line_total_length - COALESCE(v_arow.cable_calculated_length,0));

ELSE
v_arow.cable_calculated_length:= ((Cast(v_arow.cable_calculated_length as decimal(10,2)) ));--+ (Cast(v_arow.cable_loop_length as decimal(10,2))));
v_total_physical_distance:= CAST(st_length(v_arow.sp_geometry::geometry,false)AS DECIMAL(10,2))+COALESCE(v_total_physical_distance,0);
select st_startpoint(v_arow.sp_geometry) into v_sub_start;
select ST_LineInterpolatePoint(v_arow.sp_geometry, v_line_total_length/v_arow.cable_calculated_length) into v_sub_end;

select ST_Line_Substring(v_arow.sp_geometry, ST_Line_Locate_Point(v_arow.sp_geometry, v_sub_start),
ST_Line_Locate_Point(v_arow.sp_geometry, v_sub_end)) into v_sub_line;

RAISE INFO ' % v_sub_start', St_astext(v_sub_start);
RAISE INFO ' % v_sub_end', st_astext(v_sub_end);
RAISE INFO ' % v_sub_line', st_astext(v_sub_line);
RAISE INFO ' % v_arow.cable_id', v_arow.cable_id;

---Update loop code

--truncate table temp_loop;

INSERT INTO temp_loop(id,entity_type,loop_length)
select pm.system_id,pm.entity_type,lp.loop_length from point_master pm
inner join att_details_loop lp on pm.system_id=lp.system_id
where lp.cable_system_id=v_arow.cable_id and upper(pm.entity_type) =upper('LOOP')
and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_sub_line::geometry,5));

RAISE INFO ' % v_line_total_length', v_line_total_length;
RAISE INFO ' % v_total_physical_distance', v_total_physical_distance;

v_total_physical_distance:= COALESCE(v_line_total_length,0) + COALESCE(v_total_physical_distance,0);
v_cable_calculated_length:=v_arow.cable_calculated_length;
v_main_path:= v_arow.sp_geometry;
v_faulty_record:=v_arow;

raise info 'v_faulty_record :%',v_faulty_record;

RAISE INFO ' % v_total_physical_distance', v_total_physical_distance;

EXIT;
END IF;
END LOOP;

if(upper(p_entity_type)=upper('CABLE'))
then

if(p_node_type='B')
then


select st_endpoint(sp_geometry) into v_input_cable_end_point from line_master where system_id=p_entity_system_id and
upper(entity_type)=upper('CABLE');

v_main_fiber_end_point:=st_endpoint(v_main_path);
if(Cast(st_distance(v_main_fiber_end_point,v_input_cable_end_point,false)as decimal(10,2))=0)
then
RAISE INFO ' %b_end', v_line_total_length;
v_main_path:=st_reverse(v_main_path);
end if;

elsif(p_node_type='A')
then
select st_startpoint(sp_geometry) into v_input_cable_end_point from line_master where system_id=p_entity_system_id and
upper(entity_type)=upper('CABLE');
v_main_fiber_end_point:=st_startpoint(v_main_path);
if(Cast(st_distance(v_main_fiber_end_point,v_input_cable_end_point,false)as decimal(10,2))>0)
then
v_main_path:=st_reverse(v_main_path);
end if;
end if;
end if;

Select sum(loop_length) from temp_loop into v_total_loop_length;
raise info 'v_total_loop_length%',v_total_loop_length;
raise info 'v_line_total_length%',v_line_total_length;

v_line_total_length=v_line_total_length-COALESCE(v_total_loop_length,0);




raise info 'v_faulty_record :%',v_faulty_record;
raise info 'v_main_path :%',st_astext(v_main_path);
raise info 'v_cable_calculated_length final:%',v_cable_calculated_length;
select st_startpoint(v_main_path) into v_sub_line_start_point;
raise info 'select ST_LineInterpolatePoint(st_geomfromtext(''%'',4326),%/%)',st_astext(v_main_path),v_line_total_length,v_cable_calculated_length;
select ST_LineInterpolatePoint(v_main_path, v_line_total_length/v_cable_calculated_length) into v_sub_line_end_point;


select ST_Line_Substring(v_main_path, ST_Line_Locate_Point(v_main_path, v_sub_line_start_point), ST_Line_Locate_Point(v_main_path, v_sub_line_end_point))
into v_sub_line_geom;

--END REGION

raise info 'v_faulty_record: %' , v_faulty_record;
if exists(select 1 from att_details_cable cbl
inner join point_master pm
on cbl.system_id=v_faulty_record.cable_id and cbl.a_system_id=pm.system_id
and upper(pm.entity_type)=upper(cbl.a_entity_type) and st_intersects(v_sub_line_geom, pm.sp_geometry)) THEN


select ld.layer_title as t_point_type,ld.layer_name as t_point_name,a_location as t_point_network_id,
Cast(st_distance(v_sub_line_end_point,pm.sp_geometry,false)as decimal(10,2)) as distance,pm.sp_geometry as last_tp_geom into v_last_termination_point
from att_details_cable cbl inner join point_master pm
on cbl.system_id=v_faulty_record.cable_id and cbl.a_system_id=pm.system_id
and upper(pm.entity_type)=upper(cbl.a_entity_type) and st_intersects(v_sub_line_geom, pm.sp_geometry)
inner join layer_details ld on upper(ld.layer_name)=upper(pm.entity_type);
end if;


if exists(select 1 from att_details_cable cbl
inner join point_master pm
on cbl.system_id=v_faulty_record.cable_id and cbl.b_system_id=pm.system_id
and upper(pm.entity_type)=upper(cbl.b_entity_type) and st_intersects(v_sub_line_geom, pm.sp_geometry))

then

select ld.layer_title as t_point_type,ld.layer_name as t_point_name,b_location as t_point_network_id,Cast(st_distance(v_sub_line_end_point,pm.sp_geometry,false)as decimal(10,2)) as distance,pm.sp_geometry as last_tp_geom into v_last_termination_point from att_details_cable cbl
inner join point_master pm
on cbl.system_id=v_faulty_record.cable_id and cbl.b_system_id=pm.system_id
and upper(pm.entity_type)=upper(cbl.b_entity_type) and st_intersects(v_sub_line_geom, pm.sp_geometry)
inner join layer_details ld on upper(ld.layer_name)=upper(pm.entity_type);
end if;


select 1 from att_details_cable cbl
inner join point_master pm
on cbl.system_id=v_faulty_record.cable_id and cbl.a_system_id=pm.system_id
and upper(pm.entity_type)=upper(cbl.a_entity_type) and st_intersects(v_sub_line_geom, pm.sp_geometry) into v_a_data_exist;

select 1 from att_details_cable cbl
inner join point_master pm
on cbl.system_id=v_faulty_record.cable_id and cbl.b_system_id=pm.system_id
and upper(pm.entity_type)=upper(cbl.b_entity_type) and st_intersects(v_sub_line_geom, pm.sp_geometry) into v_b_data_exist;

IF(coalesce(v_a_data_exist,'')='' AND coalesce(v_b_data_exist,'')='' ) THEN
raise info 'v_a_data_exist :%',v_a_data_exist;
raise info 'v_b_data_exist:%',v_b_data_exist;
return query select row_to_json(result) from (
select
cast(st_y(v_sub_line_end_point)as decimal(10,6)) as fault_lat,
cast(st_x(v_sub_line_end_point)as decimal(10,6)) as fault_long,
v_line_total_length as physical_distance,
0.0 as last_tp_distance,
'NA'as last_tp_type,
'NA' as last_tp_name,
'NA' as last_tp_network_id,
0.0 as last_tp_lat,
0.0 as last_tp_long,
st_astext(v_main_path) as faulty_fiber_geom,
v_faulty_record.cable_id as faulty_cable_id
)result;

ELSE

return query select row_to_json(result) from (
select
cast(st_y(v_sub_line_end_point)as decimal(10,6)) as fault_lat,
cast(st_x(v_sub_line_end_point)as decimal(10,6)) as fault_long,
v_total_physical_distance as physical_distance,
v_last_termination_point.distance as last_tp_distance,
v_last_termination_point.t_point_type as last_tp_type,
v_last_termination_point.t_point_name as last_tp_name,
v_last_termination_point.t_point_network_id as last_tp_network_id,
st_y(v_last_termination_point.last_tp_geom) as last_tp_lat,
st_x(v_last_termination_point.last_tp_geom) as last_tp_long,
st_astext(v_main_path) as faulty_fiber_geom,
v_faulty_record.cable_id as faulty_cable_id
)result;


END IF;
END; $function$
;
