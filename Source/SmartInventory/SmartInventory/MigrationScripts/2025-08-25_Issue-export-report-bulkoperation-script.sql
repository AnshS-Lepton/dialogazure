CREATE OR REPLACE FUNCTION public.fn_bulk_get_pod_details(
    p_geom character varying,
    p_selectiontype character varying
)
RETURNS SETOF json
LANGUAGE plpgsql
AS $function$
DECLARE
    v_geom geometry;
BEGIN
    IF lower(p_selectiontype) = 'circle' THEN 
        v_geom := ST_GeomFromText('POINT(' || p_geom || ')', 4326);
    ELSE 
        v_geom := ST_GeomFromText('POLYGON((' || p_geom || '))', 4326);
    END IF;

    RETURN QUERY
    SELECT row_to_json(row)
    FROM (
        SELECT a.network_id,
               a.pod_type,
               a.pod_name,
               a.network_status,
               fn_format_distance(a.distance)::varchar AS distance,
               a.system_id
        FROM (
            SELECT pod.network_id,
                   pod.pod_type,
                   pod.pod_name,
                   pod.network_status,
                   ROUND(
                       ST_Distance(
                           geography(ST_MakePoint(pod.longitude, pod.latitude)),
                           geography(ST_Centroid(v_geom))
                       )::numeric, 2
                   ) AS distance,
                   pod.system_id
            FROM att_details_pod pod
            ORDER BY distance ASC
            LIMIT 5
        ) a
    ) row;
END;
$function$;


-----------------------------------------------------------------------------------------------

update LAYER_DETAILS set is_report_enable = false where LOWER(LAYER_NAME) = LOWER('RestrictedArea');

----------------------------------------------------------------------------------------------

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details ld  where layer_title='CPE'), 'ddl_entity_checklist', 'Image :', true, 5, 'now()', 5, now(), 'Image', NULL, 'Image_upload', true, true, 0);


select * from  fn_sync_layer_columns();
