CREATE OR REPLACE FUNCTION public.fn_splicing_get_entity(p_longitude double precision, p_latitude double precision, p_buffer_radius double precision, p_role_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
BEGIN
RETURN QUERY select row_to_json(row) from (

select a.*,is_splicer,is_cpe_entity,
case when map.entity_id is null and upper(a.geom_Type)=upper('POINT') then false 
when a.is_start_point=true and map2.entity_id is null and upper(a.geom_Type)=upper('LINE') then false
when a.is_end_point=true and map3.entity_id is null and upper(a.geom_Type)=upper('LINE') then false 
else true end 
is_isp_entity,
	(select count(1) from vw_termination_point_master where UPPER(layer_name)=UPPER('Cable') and UPPER(tp_layer_name)=UPPER(a.entity_type) and is_enabled=true)>0 as isCableTPPoint,l.is_middleware_entity,l.is_virtual_port_allowed as is_virtual_entity,l.layer_title,l.layer_abbr,l.layer_display_abbr from 
	(
	-- GET ALL POINT ENTITIES EXCEPT FMS & CUSTOMER WHITIN BUFFER FOR WHICH SPLICING IS ALLOWED..
	select 0 as a_system_id,null as a_entity_type,0 as b_system_id,null as b_entity_type, pm.system_id entity_system_id, pm.entity_type entity_type,'Point' as geom_Type,pm.network_status, pm.display_name as common_name,pm.no_of_ports as total_core_ports,
	false as is_start_point,false as is_end_point,pm.is_virtual  from point_master pm 
        WHERE pm.network_status !='D' and pm.is_buried=false and  ST_WITHIN(pm.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326), p_buffer_radius))
        and UPPER(pm.entity_type) IN (select upper(layer_name) from layer_details where is_splicer=true and isvisible=true and upper(layer_name) not in ('FMS','CUSTOMER'))
	union
	-- GET FMS ENTITIES WHOSE PARENT IS POD & MPOD.
	select 0 as a_system_id,null as a_entity_type,0 as b_system_id,null as b_entity_type, pm.system_id entity_system_id, pm.entity_type entity_type,'Point' as geom_Type,pm.network_status, pm.display_name as common_name,pm.no_of_ports as total_core_ports,
	false as is_start_point,false as is_end_point,pm.is_virtual from point_master pm inner join att_details_fms fms on pm.system_id=fms.system_id and upper(pm.entity_type)='FMS' 
	and upper(fms.parent_entity_type) in ('POD','MPOD','PROVINCE')
        WHERE pm.network_status !='D' and pm.is_buried=false and  ST_WITHIN(pm.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326), p_buffer_radius))
	-- GET CUSTOMER WITHIN BUFER WITH NAME AND CAN ID.
	union 
	select 0 as a_system_id,null as a_entity_type,0 as b_system_id,null as b_entity_type, pm.system_id entity_system_id, pm.entity_type entity_type,'Point' as geom_Type,pm.network_status, pm.display_name||'('||pm.common_name||')' as common_name,pm.no_of_ports as total_core_ports,
	false as is_start_point,false as is_end_point,pm.is_virtual from point_master pm  inner join att_details_customer cust on pm.system_id=cust.system_id and upper(pm.entity_type)='CUSTOMER' 
	WHERE pm.network_status !='D' and pm.is_buried=false and  ST_WITHIN(pm.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326), p_buffer_radius))
        union
	-- GET CABLES WITHIN BUFFER INCLUDING START AND END NODE...
	SELECT a_system_id,a_entity_type,b_system_id,b_entity_type,lm.system_id,lm.entity_type,'Line' as geom_Type,lm.network_status,lm.display_name,cbl.total_core||'F' as total_core,
	ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer_radius)) as is_start_point,
	ST_WITHIN(ST_EndPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer_radius)) as is_end_point,lm.is_virtual
	from att_details_cable cbl inner join line_master lm on cbl.system_id=lm.system_id  
	and upper(lm.entity_type)='CABLE'  where lm.network_status !='D'
	and (ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer_radius))
	or
	ST_WITHIN(ST_EndPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer_radius)))	
) 
a 
  left join layer_details l on upper(a.entity_type)=upper(l.layer_name)
  inner join role_permission_entity rp on l.layer_id=rp.layer_id and rp.role_id=p_role_id and upper(rp.network_status)=upper(a.network_status) and rp.viewonly=true
  left join isp_entity_mapping map on a.geom_Type='Point' and a.entity_system_id=map.entity_id and upper(a.entity_type)=upper(map.entity_type) and map.structure_id!=0
  left join isp_entity_mapping map2 on (a.a_system_id=map2.entity_id and a.a_entity_type=upper(map2.entity_type)) 
  left join isp_entity_mapping map3 on (a.b_system_id=map3.entity_id and a.b_entity_type=upper(map3.entity_type))
) row;
	
END; $function$
;

------------------------------------------------------------------------------------------------------
-- enable NE Libraray for Web and Mobile both

UPDATE public.layer_details
SET is_visible_in_ne_library=true where layer_id =36;

-----------------------------------------------------------------------------------------------------

INSERT INTO public.layer_mapping
(layer_id, parent_layer_id, parent_sequence, is_enable_inside_parent_info, is_used_for_network_id, network_code_format, is_default_code_format,  is_default_parent)
VALUES(36, 0, 7, false, true, 'FMSnnn', true,false);