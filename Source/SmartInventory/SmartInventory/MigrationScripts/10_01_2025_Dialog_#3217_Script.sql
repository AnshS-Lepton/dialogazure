/*------------------------------------------
CreatedBy: Chandra Shekhar Sahni
CreatedOn: 10 Jan 2025
Description: This function retrieves the Fiber Link details according to searched fiberlinks
ModifiedOn: 
ModifiedBy: 
Purpose: We have extended the search functionality by adding FiberLink details
------------------------------------------*/
-- DROP FUNCTION public.fn_fiberlink_list_by_linkids(varchar);

CREATE OR REPLACE FUNCTION public.fn_fiberlink_list_by_linkids(v_linkids character varying)
RETURNS SETOF json
LANGUAGE plpgsql
AS $function$
DECLARE
    v_fibergeom character varying;
BEGIN

-- Create a temporary table with ColorCode and GEOM columns
CREATE TEMP TABLE temp_fiberlink_details (
    system_id integer,
    network_id varchar,
    link_id varchar,
    link_name varchar,
    fiber_link_status varchar,
    link_type varchar,
    ColorCode varchar,
    GEOM geometry
);

-- Populate v_fibergeom with coordinates
SELECT string_agg(format('%s %s', pt.x, pt.y), ', ') INTO v_fibergeom
FROM (
    SELECT ST_X((dp).geom) AS x, ST_Y((dp).geom) AS y
    FROM (
        SELECT ST_DumpPoints(ST_Union(sp_geometry)) AS dp 
        FROM (
            SELECT cable_id 
            FROM att_details_cable_info 
            WHERE link_system_id  in(select system_id from att_details_fiber_link where link_id::varchar = ANY(string_to_array(v_linkids, ',')::varchar[]))
            GROUP BY cable_id, link_system_id
        ) a
        INNER JOIN line_master lm ON lm.system_id::varchar = a.cable_id::varchar AND lm.entity_type = 'Cable'
    ) AS subquery
) AS pt;

raise info'v_fibergeom%',v_fibergeom;

-- Insert data with ColorCode cycling from cable_color_master
INSERT INTO temp_fiberlink_details
SELECT 
    adf.system_id, adf.network_id, adf.link_id, adf.link_name, adf.fiber_link_status, adf.link_type,
    cc.color_code AS ColorCode,
    NULL::geometry AS GEOM
FROM (
    SELECT 
        system_id, network_id, link_id, link_name, fiber_link_status, link_type,
        ROW_NUMBER() OVER (ORDER BY link_id) AS row_num
    FROM att_details_fiber_link
    WHERE link_id::varchar = ANY(string_to_array(v_linkids, ',')::varchar[])
) adf
JOIN (
    SELECT color_code, ROW_NUMBER() OVER () - 1 AS rn
    FROM cable_color_master where type='Core'
) cc
ON adf.row_num % (SELECT COUNT(*) FROM cable_color_master) = cc.rn::integer;

RETURN QUERY
SELECT json_build_object(
    'FiberLinkDetails', (
        SELECT json_agg(
            json_build_object(
                'type', 'Feature',
                'geometry', v_fibergeom,
                'properties', json_build_object(
                    'system_id', system_id,
                    'network_id', network_id,
                    'link_id', link_id,
                    'link_name', link_name,
                    'fiber_link_status', fiber_link_status,
                    'link_type', link_type,
                    'ColorCode', ColorCode
                )
            )
        )
        FROM temp_fiberlink_details
    )
) AS geojson;

END;
$function$;