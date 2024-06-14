CREATE OR REPLACE FUNCTION public.fn_associate_split_entities(p_entity_one_system_id integer, p_entity_two_system_id integer, p_entity_one_network_id character varying, p_entity_two_network_id character varying, p_entity_type character varying, p_old_entity_system_id integer)
RETURNS SETOF json
LANGUAGE plpgsql
AS $function$

declare
v_p_geom_type character varying;
v_geometry character varying;
v_geometry2 CHARACTER VARYING;
v_buffer integer;
v_cable_type character varying;
v_user_id integer;
v_one_display_name character varying;
v_two_display_name character varying;
arow record;
v_entity_one_system_id integer;
v_entity_two_system_id integer;

BEGIN

if(upper(p_entity_type)='CABLE')
then
select cable_type into v_cable_type from att_details_cable where system_id=p_old_entity_system_id;
--select created_by into v_user_id from att_details_cable where system_id=p_entity_one_system_id;
select created_by into v_user_id from att_details_cable where network_id=p_entity_one_network_id;
IF(p_entity_one_system_id=0)
THEN
select system_id into p_entity_one_system_id from att_details_cable where network_id=p_entity_one_network_id;
END IF;
IF(p_entity_two_system_id=0)
THEN
select system_id into p_entity_two_system_id from att_details_cable where network_id=p_entity_two_network_id;
END IF;
end if;

if(upper(p_entity_type)='DUCT')
then
select created_by into v_user_id from att_details_duct where system_id=p_entity_one_system_id;
end if;

select geom_type into v_p_geom_type from layer_details where upper(layer_name)=upper(p_entity_type);
execute 'select pm.sp_geometry,pm.display_name from '||v_p_geom_type||'_master pm where pm.system_id='||p_entity_one_system_id||' and upper(pm.entity_type)=upper('''||p_entity_type||''')' into v_geometry,v_one_display_name;
execute 'select pm.sp_geometry,pm.display_name from '||v_p_geom_type||'_master pm where pm.system_id='||p_entity_two_system_id||' and upper(pm.entity_type)=upper('''||p_entity_type||''')' into v_geometry2,v_two_display_name;
select value::integer into v_buffer from global_settings where key='AssociateEntityBuffer' and type='Web';
--------------------LEFT SIDE ASSOCIATION------------------------------------
insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,associated_entity_type,associated_display_name,created_on,created_by)
select p_entity_one_system_id,p_entity_one_network_id,p_entity_type,v_one_display_name,ass.entity_system_id,ass.entity_network_id,ass.entity_type,display_name,now(),v_user_id
from (
select
pm.system_id,
pm.entity_type,pm.common_name,null as cable_type,'Point' as geom_type,pm.display_name
from point_master pm
where upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type) and upper(coalesce(layer_subtype,''))=case when upper(p_entity_type)='CABLE' then upper(v_cable_type) else upper(coalesce(layer_subtype,'')) end)
and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer) )

union
select lm.system_id,lm.entity_type,lm.common_name,cbl.cable_type,lm.geom_type,lm.display_name from (
select
pm.system_id,
pm.entity_type,pm.common_name,'Line' as geom_type,pm.display_name
from line_master pm
where
upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
and
ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer) )) lm
left join att_details_cable cbl on lm.system_id=cbl.system_id and upper(lm.entity_type)='CABLE'
where cable_type is null or upper(cable_type) in (select upper(layer_subtype) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
) tblmain
inner join
(
select entity_system_id,entity_network_id,entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
union
select associated_system_id,associated_network_id,associated_entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
) ass on tblmain.system_id=ass.entity_system_id and upper(tblmain.entity_type)=upper(ass.entity_type)
--and ass.entity_system_id!=p_old_entity_system_id
and (upper(ass.entity_type)!=upper(p_entity_type))
--left join user_master um on um.user_id=ass.created_by
left join layer_details ld on upper(ld.layer_name)=upper(tblmain.entity_type)
where case when ld.is_isp_layer=true and ld.geom_type='Point' and ass.entity_system_id is null then 1=2 else 1=1 end;

RAISE INFO ' % v_geometry', v_geometry;

---Update loop code
IF((select count(pm.system_id)
from point_master pm
inner join att_details_loop lp on pm.system_id=lp.system_id
where lp.associated_system_id=p_old_entity_system_id and upper(pm.entity_type) =upper('LOOP') and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry::geometry,v_buffer)))>0)
THEN
update att_details_loop set associated_system_id=p_entity_one_system_id,associated_network_id=p_entity_one_network_id,cable_system_id=p_entity_one_system_id
where cable_system_id=p_old_entity_system_id;
END IF;

------------------------------------------------------------LEFT SIDE ASSOCIATION---------------------------------------------------------------------------------
insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,associated_entity_type,associated_display_name,created_on,created_by)
select p_entity_two_system_id,p_entity_two_network_id,p_entity_type,v_two_display_name,ass.entity_system_id,ass.entity_network_id,ass.entity_type,display_name ,now(),v_user_id
from (
select
pm.system_id,
pm.entity_type,pm.common_name,null as cable_type,'Point' as geom_type,pm.display_name
from point_master pm
where upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type) and upper(coalesce(layer_subtype,''))=case when upper(p_entity_type)='CABLE' then upper(v_cable_type) else upper(coalesce(layer_subtype,'')) end)
and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer) )

union
select lm.system_id,lm.entity_type,lm.common_name,cbl.cable_type,lm.geom_type,lm.display_name from (
select
pm.system_id,
pm.entity_type,pm.common_name,'Line' as geom_type,pm.display_name
from line_master pm
where
upper(pm.entity_type) in (select upper(associate_layer_name) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
and
ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer) )) lm
left join att_details_cable cbl on lm.system_id=cbl.system_id and upper(lm.entity_type)='CABLE'
where cable_type is null or upper(cable_type) in (select upper(layer_subtype) from vw_associate_entity_master where upper(layer_name)=upper(p_entity_type))
) tblmain
inner join
(
select entity_system_id,entity_network_id,entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
union
select associated_system_id,associated_network_id,associated_entity_type,ass.created_by,ass.created_on,ass.is_termination_point from associate_entity_info ass
where ((ass.entity_system_id=p_old_entity_system_id and upper(ass.entity_type)=upper(p_entity_type)) or ass.associated_system_id=p_old_entity_system_id and upper(ass.associated_entity_type)=upper(p_entity_type)) and ass.is_termination_point=false
) ass on tblmain.system_id=ass.entity_system_id and upper(tblmain.entity_type)=upper(ass.entity_type)
--and ass.entity_system_id!=p_old_entity_system_id
and (upper(ass.entity_type)!=upper(p_entity_type))
--left join user_master um on um.user_id=ass.created_by
left join layer_details ld on upper(ld.layer_name)=upper(tblmain.entity_type)
where case when ld.is_isp_layer=true and ld.geom_type='Point' and ass.entity_system_id is null then 1=2 else 1=1 end;

RAISE INFO ' % v_geometry2 ', v_geometry2;

---Update loop code
IF((select count(pm.system_id) from point_master pm
inner join att_details_loop lp on pm.system_id=lp.system_id
where lp.associated_system_id=p_old_entity_system_id and upper(pm.entity_type) =upper('LOOP') and ST_Intersects(st_makevalid(pm.sp_geometry),ST_buffer_meters(v_geometry2::geometry,v_buffer)))>0)
THEN
update att_details_loop set associated_system_id=p_entity_two_system_id,associated_network_id=p_entity_two_network_id,cable_system_id=p_entity_two_system_id
where cable_system_id=p_old_entity_system_id;
END IF;


---FIBER LINK ASSOCIATION BY ANTRA

select system_id into v_entity_one_system_id from att_details_cable where network_id=p_entity_one_network_id;
select system_id into v_entity_two_system_id from att_details_cable where network_id=p_entity_two_network_id;

for arow in select link_system_id,fiber_number from att_details_cable_info where cable_id=p_old_entity_system_id
loop


UPDATE att_details_cable_info  SET link_system_id= arow.link_system_id WHERE  cable_id= v_entity_one_system_id and fiber_number=arow.fiber_number;

END LOOP;

for arow in select link_system_id,fiber_number from att_details_cable_info where cable_id=p_old_entity_system_id
loop

UPDATE att_details_cable_info  SET link_system_id= arow.link_system_id WHERE  cable_id= v_entity_two_system_id and fiber_number=arow.fiber_number;

END LOOP;

for arow in select link_system_id,fiber_number from att_details_cable_info where cable_id=p_old_entity_system_id
loop

UPDATE att_details_fiber_link SET  fiber_link_status = 'Associated' WHERE system_id = arow.link_system_id;

END LOOP;

for arow in select link_system_id,fiber_number from att_details_cable_info where cable_id=p_old_entity_system_id
loop

UPDATE att_details_fiber_link SET  fiber_link_status = 'Associated' WHERE system_id = arow.link_system_id;

END LOOP;

----END FIBER LINK ASSOCIATION 

END;
$function$
;