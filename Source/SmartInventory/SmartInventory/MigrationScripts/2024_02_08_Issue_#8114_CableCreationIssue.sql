INSERT INTO public.global_settings ( "key", value, description, "type", is_edit_allowed, data_type, max_value, created_on, created_by, min_value, is_mobile_key, is_web_key)
 VALUES( 'MaxLineEntityLength', '100', 'Max length a line entity can be created', 'Web', true, 'double precision', 100.0, now(), 1, 1.0, false, true);


CREATE OR REPLACE FUNCTION public.fn_get_validate_entitycreation_area(p_geom text, p_userid integer, p_selectiontype character varying, p_ticket_id integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE

result text;
result_status bool;
maxAreaAllowed integer;
v_sp_geometry text;
v_province_id integer;
Is_NLD integer =0;
v_province_abbr character varying;
BEGIN
--INITIALIZE VARIABLE
result := 'successfully process';
select value into Is_NLD from global_settings where key='IsNLD';


IF(lower(p_selectiontype) = 'polygon' or lower(p_selectiontype) = 'circle') THEN

RAISE INFO 'sqkm : %',(select st_geomfromtext('POLYGON(('||p_geom||'))',4326));
if exists(select 1 from province_boundary pb join vw_user_permission_area pa on pb.id = pa.province_id where pa.user_id = p_userid and 
ST_Intersects(pb.sp_geometry,(select st_geomfromtext('POLYGON(('||p_geom||'))',4326))))then
		result := 'Success'; 
		result_status:=true;
	else
		--result := 'Selected area do not have permission. Please contact your administrator !';
		result := 'Selected area not exists in permitted region/province. Please contact your administrator !';
		result_status:=false;
	end if;

ELSE IF(lower(p_selectiontype) = 'line') THEN


   if exists(select 1 from province_boundary pb join vw_user_permission_area pa on pb.id = pa.province_id 
   where pa.user_id = p_userid and ST_Intersects(pb.sp_geometry,(select St_Geomfromtext('LineString('||p_geom||')',4326))))then


	if exists(select count(1) from province_boundary pb join vw_user_permission_area pa on pb.id = pa.province_id 
	where pa.user_id = p_userid and ST_Intersects(pb.sp_geometry,(select St_Geomfromtext('LineString('||p_geom||')',4326))))then


	
	if (Is_NLD = 1) then 
		result := 'Success'; 
		result_status:=true;

	else
	
	SELECT PROVINCE_ABBREVIATION INTO v_province_abbr FROM  PROVINCE_BOUNDARY WHERE ST_WITHIN( ST_StartPoint(ST_GEOMFROMTEXT('LineString('||p_geom||')',4326)),SP_GEOMETRY)
	AND ISVISIBLE=TRUE;
	
	IF(coalesce(v_province_abbr,'')='') THEN
		result := 'Selected area not exists in permitted region/province. Please contact your administrator !';
		result_status:=false;
	Else

		result := 'Success'; 
		result_status:=true;

	END IF;
	
	END IF;
	END IF;
	
    else
		--result := 'Selected area do not have permission. Please contact your administrator !';
		result := 'Selected area not exists in permitted region/province. Please contact your administrator !';
		result_status:=false;
	end if;
ELSE


if exists(select 1 from province_boundary pb join vw_user_permission_area pa on pb.id = pa.province_id
where pa.user_id = p_userid and ST_Intersects(pb.sp_geometry,(select St_Geomfromtext('Point('||p_geom||')',4326))))then
		result := 'Success'; 
		result_status:=true;
	else
		--result := 'Selected area do not have permission. Please contact your administrator !';
		result := 'Selected area not exists in permitted region/province. Please contact your administrator !';
		result_status:=false;
	end if;
END IF;
END IF;
if(p_ticket_id > 0)then
if exists(select 1 from polygon_master where system_id = p_ticket_id and Upper(entity_type)='NETWORK_TICKET')then
if exists(select 1 from polygon_master pm where system_id = p_ticket_id and Upper(entity_type)='NETWORK_TICKET' and
ST_Intersects(pm.sp_geometry,case when(lower(p_selectiontype) = 'polygon' or lower(p_selectiontype)= 'circle') then (select st_geomfromtext('POLYGON(('||p_geom||'))',4326)) else
case when lower(p_selectiontype) = 'line' then (select St_Geomfromtext('linestring('||p_geom||')',4326)) else (select St_Geomfromtext('Point('||p_geom||')',4326)) end end))then
		result := 'Success'; 
		result_status:=true;
	else
		result := 'Entity can not be created out side the assigned ticket area. Please contact your administrator !';
		result_status:=false;
	end if;
else
select province_id into v_province_id from att_details_networktickets where ticket_id=p_ticket_id ;
if(v_province_id = 0)then
	result := 'Success'; 
	result_status:=true;
else
if exists(select 1 from province_boundary where id=v_province_id)then
if exists(select 1 from province_boundary pb where id=v_province_id and ST_Intersects(pb.sp_geometry,case when(lower(p_selectiontype) = 'polygon' or lower(p_selectiontype)= 'circle') then(select st_geomfromtext('POLYGON(('||p_geom||'))',4326)) else
case when lower(p_selectiontype) = 'line' then (select St_Geomfromtext('LINESTRING('||p_geom||')',4326))
 else (select St_Geomfromtext('Point('||p_geom||')',4326)) end end))then
		result := 'Success'; 
		result_status:=true;
	else
		result := 'Entity can not be created out side the assigned ticket area. Please contact your administrator !';
		result_status:=false;
	end if;

END IF;
end if;
end if;
end if;

RETURN QUERY
         select result_status  as status, result::character varying as message;

END; 
$function$
;
