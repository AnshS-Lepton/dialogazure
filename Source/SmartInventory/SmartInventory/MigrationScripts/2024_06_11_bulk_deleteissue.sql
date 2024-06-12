-- FUNCTION: public.fn_get_bulk_entity_by_geom(text, integer, character varying, double precision, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_bulk_entity_by_geom(text, integer, character varying, double precision, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_bulk_entity_by_geom(
	p_geom text,
	p_userid integer,
	p_selectiontype character varying,
	p_radius double precision,
	p_entity_type character varying,
	p_network_status character varying)
    RETURNS TABLE(system_id integer, entity_type character varying, network_status character varying, network_id character varying, project_id integer, created_by integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

Declare
 v_role_id integer;
  v_geometry geometry;
BEGIN

select into v_role_id role_id from user_master where user_id = p_userid;
if not exists(select 1 from information_schema.tables where upper(table_name)='TEMP_REGION_PROV')
then
	create temp table temp_region_prov(
	region_id integer,
	id integer
	)on commit drop;
end if;

IF(p_selectiontype <> 'circle') 
THEN
	select st_geomfromtext('POLYGON(('||p_geom||'))',4326) into v_geometry;
else 
	select st_buffer_meters(St_Geomfromtext('Point('||p_geom||')',4326), p_radius) into v_geometry;
end if;

truncate table temp_region_prov;
insert into temp_region_prov(region_id,id)
select region_id,id from province_boundary prov where ST_Intersects(v_geometry,prov.sp_geometry);

RETURN QUERY
select bulkentity.system_id, bulkentity.entity_type, bulkentity.network_status, bulkentity.network_id,bulkentity.project_id,bulkentity.created_by from (
select pm.system_id, pm.entity_type, pm.network_status,pm.common_name as network_id,pm.project_id,pm.created_by from point_master as pm
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id 
where case when coalesce(p_entity_type,'')!='' then upper(pm.entity_type)=upper(p_entity_type) else 1=1 end and 
st_within(pm.sp_geometry,v_geometry)
union all
select lm.system_id, lm.entity_type, lm.network_status, lm.common_name as network_id,lm.project_id,lm.created_by from line_master as lm
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id 
where case when coalesce(p_entity_type,'')!='' then upper(lm.entity_type)=upper(p_entity_type) else 1=1 end and 
st_intersects(lm.sp_geometry, v_geometry)
union all
select poly.system_id, poly.entity_type, poly.network_status,poly.common_name as network_id,poly.project_id,poly.created_by from polygon_master poly
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id
where case when coalesce(p_entity_type,'')!='' then upper(poly.entity_type)=upper(p_entity_type) else 1=1 end 
and st_within(poly.sp_geometry, v_geometry)
union  
select room.system_id, 'Unit' as entity_type, room.network_status,room.network_id,room.project_id,null as created_by
	from point_master pm 
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id
inner join  isp_entity_mapping mapp on pm.system_id=mapp.structure_id and pm.entity_type='Structure'
inner join isp_room_info room On mapp.structure_id=room.parent_system_id  
where case when (upper(p_entity_type)='UNIT' or upper(p_entity_type)='') then upper(pm.entity_type)=upper('structure') end 
and st_within(pm.sp_geometry, v_geometry)
union
select cbl.system_id, 'Cable' as entity_type, cbl.network_status,cbl.network_id,cbl.project_id,cbl.created_by  from isp_line_master lm  
join att_details_cable cbl on cbl.system_id=lm.entity_id and lm.cable_type='ISP'
join point_master pm
on pm.system_id=lm.structure_id and upper(pm.entity_type)='STRUCTURE'
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id
where case when (upper(p_entity_type)=upper('Cable') or upper(p_entity_type)='') then 1=1 end 
and st_within(pm.sp_geometry,v_geometry)
union 
select pit.system_id, 'PIT' as entity_type, 'P' as network_status,pit.network_id,0 as project_id,null as created_by  from circle_master cm
 inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id  
join att_details_row_pit pit
on pit.system_id=cm.system_id  
join att_details_row row
on row.system_id=pit.parent_system_id and upper(pit.parent_entity_type)='ROW'
where case when (upper(p_entity_type)=upper('PIT') or upper(p_entity_type)='') then 1=1  end 
and st_within(cm.sp_geometry, v_geometry)
Union
select rack.system_id, 'Rack' as entity_type, rack.network_status,rack.network_id,0 as project_id,null as created_by  from point_master pm 
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id 
inner join  isp_entity_mapping mapp on pm.system_id=mapp.entity_id and upper(pm.entity_type)=upper('POD')
inner join att_details_rack rack 
On mapp.entity_id=rack.parent_system_id  
where case when (upper(p_entity_type)='RACK' or upper(p_entity_type)='') then upper(pm.entity_type)=upper('POD') end 
and st_within(pm.sp_geometry, v_geometry)
Union
select model.system_id, 'Equipment' as entity_type, model.network_status,model.network_id,0 as project_id,null as created_by  from point_master pm
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id 
inner join  isp_entity_mapping mapp on pm.system_id=mapp.entity_id and upper(pm.entity_type)=upper('POD')
inner join att_details_model model 
On mapp.entity_id=model.parent_system_id  
where case when (upper('Equipment')='EQUIPMENT' or upper('Equipment')='') then upper(pm.entity_type)=upper('POD') end 
and st_within(pm.sp_geometry, v_geometry)
) as bulkentity 
left join layer_details l on upper(bulkentity.entity_type)=upper(l.layer_name)   
inner join vw_network_layers nl on nl.layer_id = l.layer_id and nl.role_id = v_role_id
and (COALESCE(nl.planned_view,false) = true or COALESCE(nl.asbuild_view,false)  = true or COALESCE(nl.dormant_view,false)  = true )
where upper(p_network_status) =upper(bulkentity.network_status) order by bulkentity.entity_type ;
END; 
$BODY$;

ALTER FUNCTION public.fn_get_bulk_entity_by_geom(text, integer, character varying, double precision, character varying, character varying)
    OWNER TO postgres;
