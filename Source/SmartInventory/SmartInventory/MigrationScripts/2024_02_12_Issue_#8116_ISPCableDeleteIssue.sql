CREATE OR REPLACE FUNCTION public.fn_get_rlcc_geometry_details(p_system_id integer, p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$


DECLARE

v_latitude character varying;
v_longitude character varying;
v_target_ref_id integer;
v_layer_table character varying;
v_gis_design_id character varying;
v_geom_type character varying;

BEGIN

select layer_table,geom_type into v_layer_table,v_geom_type from layer_details where upper(layer_name)=upper(p_entity_type);
Execute 'Select target_ref_id from '||v_layer_table||' where system_id='||p_system_id||'' into v_target_ref_id;
Execute 'Select gis_design_id from '||v_layer_table||' where system_id='||p_system_id||'' into v_gis_design_id;

raise info 'v_geom_type%',v_geom_type;

IF(upper(v_geom_type)='POLYGON') THEN

RETURN QUERY

select row_to_json(row) FROM (select st_x((ST_DumpPoints(sp_geometry)).geom) as longitude,st_y((ST_DumpPoints(sp_geometry)).geom) as latitude,coalesce(v_target_ref_id,0) as objectId,coalesce(v_gis_design_id,'') as GisDesignId
 from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type) ) row;

-- SELECT string_agg(((('X="'::text || st_x((t_1.dp).geom)::text) || '" Y="'::text) || st_y((t_1.dp).geom)::text) || '"'::text, ','::text ORDER BY ((t_1.dp).path[1]))::character varying AS coordinates FROM ( SELECT st_dumppoints(st_transform(st_setsrid( v_sp_geometry::geometry, 4326), 7755)) AS dp) t_1  into v_geometry;

END IF;

IF(upper(v_geom_type)='POINT') THEN

RETURN QUERY

select row_to_json(row) FROM (select st_x((ST_DumpPoints(sp_geometry)).geom) as longitude,st_y((ST_DumpPoints(sp_geometry)).geom) as latitude,0 as objectId,coalesce(v_gis_design_id,'') as GisDesignId from point_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type) ) row;

 END IF;

 IF(upper(v_geom_type)='POINT' OR upper(p_entity_type)='BUILDING' ) THEN

RETURN QUERY

select row_to_json(row) FROM (select st_x((ST_DumpPoints(sp_geometry)).geom) as longitude,st_y((ST_DumpPoints(sp_geometry)).geom) as latitude,0 as objectId,coalesce(v_gis_design_id,'') as GisDesignId from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type) ) row;

 END IF;

IF(upper(v_geom_type)='LINE') THEN

IF(Upper(p_entity_type)='CABLE') THEN

RETURN QUERY
select row_to_json(row) FROM (select st_x((ST_DumpPoints(sp_geometry)).geom) as longitude,st_y((ST_DumpPoints(sp_geometry)).geom) as latitude,0 as objectId,coalesce(v_gis_design_id,'') as GisDesignId from line_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type) 
union 
select '0.00' as longitude,'0.00' as latitude,0 as objectId,coalesce(v_gis_design_id,'') as GisDesignId from att_details_cable_info where cable_id=p_system_id) row;

ELSE

RETURN QUERY
select row_to_json(row) FROM (select st_x((ST_DumpPoints(sp_geometry)).geom) as longitude,st_y((ST_DumpPoints(sp_geometry)).geom) as latitude,0 as objectId,coalesce(v_gis_design_id,'') as GisDesignId from line_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type)) row; 

END IF;
END IF;

END

$function$
;
