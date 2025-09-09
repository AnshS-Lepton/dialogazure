----- Used in map file---------------------------

CREATE OR REPLACE VIEW public.vw_segment_details_map
 AS
 SELECT tr.route_id,
    ts.id AS segment_id,
    ts.segment_code,
    tr.route_geom,
    ts.region_id AS segment_region_id,
    tr2.region_name
   FROM top_segment ts
     JOIN top_routes tr ON ts.route_id = tr.route_id
     LEFT JOIN top_region tr2 ON tr2.id = ts.region_id
  WHERE tr.route_geom IS NOT NULL;