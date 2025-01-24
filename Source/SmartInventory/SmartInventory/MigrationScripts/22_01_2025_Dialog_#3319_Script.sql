/*------------------------------------------
CreatedBy: Chandra Shekhar Sahni
CreatedOn: 22 Jan 2025
Description: This function returns all the cable details which has maximum no of cores linked to passed fiberlinks parameter to this function.
ModifiedOn:  
ModifiedBy:  
Purpose: 
Example: select * from fn_get_cables_by_fiberlink_ids('TXCHANDRA1,TXCHANDRA2')
------------------------------------------*/
CREATE OR REPLACE FUNCTION fn_get_cables_by_fiberlink_ids(link_ids TEXT)
RETURNS SETOF JSONB
LANGUAGE plpgsql
AS $function$
DECLARE
    json_result JSONB;
    max_link_count INTEGER;
    v_cable_id INTEGER;
    max_cables JSONB := '[]'::JSONB;  -- Initialize as empty JSON array
	feature_collection JSONB;
BEGIN
    -- Call the existing function and store the result in json_result
    SELECT public.fn_cable_list_by_linkids(link_ids)::JSONB INTO json_result;
    
-- Check if json_result has data and 'features' array is present and has elements
    IF json_result IS NULL OR jsonb_typeof(json_result->'features') <> 'array' OR jsonb_array_length(json_result->'features') = 0 THEN
        -- Return an empty FeatureCollection
        feature_collection := jsonb_build_object(
            'type', 'FeatureCollection',
            'features', '[]'::JSONB
        );
        RETURN QUERY SELECT feature_collection;
        RETURN;
    END IF;

    -- Initialize max_link_count to a very small number
    max_link_count := -1;
    
    -- Iterate over the features array in the JSON result to find the max link_count
    FOR i IN 0 .. jsonb_array_length(json_result->'features') - 1 LOOP
        -- Check if the current feature has a link_count greater than max_link_count
        IF (json_result->'features'->i->'properties'->>'link_count')::INTEGER > max_link_count THEN
            -- Update max_link_count
            max_link_count := (json_result->'features'->i->'properties'->>'link_count')::INTEGER;
        END IF;
    END LOOP;
    
    -- Iterate again to collect all cables with the max link_count
    FOR i IN 0 .. jsonb_array_length(json_result->'features') - 1 LOOP
        IF (json_result->'features'->i->'properties'->>'link_count')::INTEGER = max_link_count THEN
            -- Append the cable to the max_cables array
            max_cables := jsonb_insert(max_cables, '{0}', json_result->'features'->i, true);
        END IF;
    END LOOP;
	raise info '%max_link_count: ',max_link_count;

    -- Return the JSON result with the cables having max link_count
	feature_collection := jsonb_build_object(
        'type', 'FeatureCollection',
        'features', max_cables
    );
    RETURN QUERY 
    SELECT feature_collection;
END;
$function$;
----------------------------------------------------------------------------
/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: This function returns all the cables linked to fiberlinks passed in parameter
ModifiedOn:  22 Jan 2025
ModifiedBy:  Chandra Shekhar Sahni
Purpose: Added region_name and province_name in the resulted output
Example: select * from fn_cable_list_by_linkids('TXCHANDRA1,TXCHANDRA2')
------------------------------------------*/
-- DROP FUNCTION public.fn_cable_list_by_linkids(varchar);
CREATE OR REPLACE FUNCTION public.fn_cable_list_by_linkids(v_linkids character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
begin
	RETURN QUERY 
	SELECT json_build_object(
    'type', 'FeatureCollection',
    'features', json_agg(
        json_build_object(
            'type', 'Feature',
            'geometry', ST_AsGeoJSON(sp_geometry)::json,
            'properties', json_build_object(
                'system_id', system_id ,
                'cable_name',cable_name,
                'network_id', network_id,
                'a_location',a_location,
                'b_location',b_location,
                'total_core',total_core,
                'link_count',link_count,
				'region_name',region_name,
				'province_name',province_name
            )
        )
    )
) AS geojson 
FROM (
select cableinfo.*,lm.sp_geometry,c.cable_name,c.a_location ,c.b_location 
	,c.total_core,c.system_id,c.network_id,
rb.region_name,pb.province_name 
	from (
select ci.cable_id as system_idd--,ci.link_system_id ,fl.network_id as link_network_id ,fl.link_id  ,fl.link_name 
		,count(1) as link_count
from att_details_cable_info ci 
left join att_details_fiber_link fl on ci.link_system_id =fl.system_id 
where fl.link_id =ANY(string_to_array(v_linkids, ',')::varchar[])
group by ci.cable_id--,ci.link_system_id ,fl.network_id ,fl.link_id ,fl.link_name
	) as cableinfo
join line_master lm on lm.system_id =cableinfo.system_idd and lm.entity_type ='Cable'
join  att_details_cable c on cableinfo.system_idd  = c.system_id
inner join region_boundary rb on rb.id=c.region_id
inner join province_boundary pb on pb.id=c.province_id

order by link_count desc

) ft	;
  END;
$function$
;
----------------------------------------------------------------------------