
--------------------Modify map view ------------------------------------------
DROP VIEW public.vw_att_details_pod_map;

CREATE OR REPLACE VIEW public.vw_att_details_pod_map
 AS
 SELECT
        CASE
            WHEN pod.agg_01 IS NULL THEN '0'::text
            ELSE '1'::text
        END AS agg_02,
        CASE
            WHEN dm.dropdown_value::text = 'Access'::text THEN '0'::text
            WHEN dm.dropdown_value::text = 'Aggregate'::text THEN '1'::text
            WHEN dm.dropdown_value::text = 'Core'::text THEN '2'::text
            ELSE '3'::text
        END AS agg,
    pod.system_id,
    pod.region_id,
    pod.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    pod.pod_name,
    pod.project_id,
    pod.planning_id,
    pod.purpose_id,
    pod.workorder_id,
    pod.gis_design_id,
    pod.status,
    pod.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pod.network_status::text) AS network_status,
    pod.ownership_type,
    pod.third_party_vendor_id,
    pod.source_ref_id,
    pod.source_ref_type,
    lim.icon_path,
    (pod.site_id::text || '-'::text) || pod.site_name::text AS label_column,
	pod.is_agg_site
   FROM point_master point
     JOIN att_details_pod pod ON point.system_id = pod.system_id AND point.entity_type::text = 'POD'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = pod.system_id AND adi1.entity_type::text = 'pod'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pod.network_status::text) AND lim.layer_name::text = 'POD'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = pod.system_id AND adi.entity_type::text = 'POD'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN dropdown_master dm ON dm.layer_id = lim.layer_id;

     ----------------------------------Modify function------------------------------------------

     DROP FUNCTION IF EXISTS public.fn_get_ring_details(integer, integer);

     CREATE OR REPLACE FUNCTION public.fn_get_ring_details(
	p_segment_id integer,
	p_numberofsites integer,
	p_ringcapacity character varying)
    RETURNS TABLE(ring_id integer, ring_info text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT 
    tr.id AS ring_id,
    CONCAT(tr.ring_code,' (', COUNT(adp.ring_id),')') AS ring_info
FROM 
    top_ring tr
LEFT JOIN 
    att_details_pod adp ON tr.id = adp.ring_id
WHERE 
    tr.segment_id = p_segment_id and tr.ring_capacity=p_ringcapacity
GROUP BY 
    tr.id, tr.ring_code 
	HAVING COUNT(adp.ring_id) <= p_numberofsites 
	;

END;
$BODY$;

---------------------Modify function--------------------------------------------

DROP FUNCTION IF EXISTS public.fn_insert_top_segment_ring_cable_mapping(integer, integer, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_insert_top_segment_ring_cable_mapping(
	p_agg1_system_id integer,
	p_agg2_system_id integer,
	p_user_id integer,
	p_ring_id integer,
	p_segment_id integer,
	p_top_type character varying
)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    rec RECORD;
BEGIN
    -- Use a cursor to loop through results from the function
    FOR rec IN 
        SELECT network_id 
        FROM fn_topology_get_segment_cables(p_agg1_system_id, p_agg2_system_id, p_user_id)
    LOOP
	IF p_top_type = 'Ring' THEN
        INSERT INTO top_ring_cable_mapping (ring_id, cable_id) 
        VALUES (p_ring_id, rec.network_id);
		end if;

        INSERT INTO top_segment_cable_mapping (segment_id, cable_id) 
        VALUES (p_segment_id, rec.network_id);
    END LOOP;

    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN
        RAISE WARNING 'Error in function: %', SQLERRM;
        RETURN FALSE;
END;
$BODY$;

