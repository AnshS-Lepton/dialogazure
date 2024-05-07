DROP VIEW public.vw_att_details_slack_report;

CREATE OR REPLACE VIEW public.vw_att_details_slack_report
 AS
 SELECT slk.system_id,
    slk.network_id,
    round(slk.slack_length::numeric, 2) AS slack_length,
    slk.associated_network_id,
    slk.associated_entity_type,
    cbl.network_id AS duct_network_id,
    fn_get_date(slk.created_on) AS created_on,
    fn_get_date(slk.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    slk.status_remark,
    slk.status_updated_by,
    um.user_name AS created_by,
    um.user_id AS created_by_id,
    fn_get_date(slk.status_updated_on) AS status_updated_on,
    slk.is_visible_on_map,
    slk.source_ref_type,
    slk.source_ref_id,
    slk.source_ref_description,
    slk.province_id,
    slk.region_id,
    round(slk.latitude::numeric, 6) AS latitude,
    round(slk.longitude::numeric, 6) AS longitude,
    reg.country_name,
    slk.status,
    slk.origin_from,
    slk.origin_ref_id,
    slk.origin_ref_code,
    slk.origin_ref_description,
    slk.request_ref_id,
    slk.requested_by,
    slk.request_approved_by,
    slk.subarea_id,
    slk.area_id,
    slk.dsa_id,
    slk.csa_id,
    reg.region_name,
    prov.province_name,
    slk.gis_design_id AS design_id,
    slk.st_x,
    slk.st_y,
    slk.ne_id,
    slk.prms_id,
    slk.mzone_id,
    slk.jc_id,
    slk.served_by_ring,
     CASE
            WHEN slk.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN slk.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN slk.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status
   FROM att_details_slack slk
     LEFT JOIN att_details_duct cbl ON slk.duct_system_id = cbl.system_id
     LEFT JOIN province_boundary prov ON slk.province_id = prov.id
     LEFT JOIN region_boundary reg ON slk.region_id = reg.id
     JOIN user_master um ON slk.created_by = um.user_id
     LEFT JOIN user_master um2 ON slk.modified_by = um2.user_id
     JOIN point_master point ON point.system_id = slk.system_id AND upper(point.entity_type::text) = 'SLACK'

ALTER TABLE public.vw_att_details_slack_report
    OWNER TO postgres;
	

DROP VIEW public.vw_att_details_loop_report;

CREATE OR REPLACE VIEW public.vw_att_details_loop_report
 AS
 SELECT lop.system_id,
    lop.network_id,
    round(lop.loop_length::numeric, 2) AS loop_length,
    lop.associated_network_id,
    lop.associated_entity_type,
    cbl.network_id AS cable_network_id,
    fn_get_date(lop.created_on) AS created_on,
    fn_get_date(lop.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    lop.status_remark,
    lop.status_updated_by,
    um.user_name AS created_by,
    um.user_id AS created_by_id,
    fn_get_date(lop.status_updated_on) AS status_updated_on,
    lop.is_visible_on_map,
    lop.source_ref_type,
    lop.source_ref_id,
    lop.source_ref_description,
    round(lop.start_reading::numeric, 2) AS start_reading,
    round(lop.end_reading::numeric, 2) AS end_reading,
    lop.province_id,
    lop.region_id,
    round(lop.latitude::numeric, 6) AS latitude,
    round(lop.longitude::numeric, 6) AS longitude,
    reg.country_name,
    lop.status,
    lop.origin_from,
    lop.origin_ref_id,
    lop.origin_ref_code,
    lop.origin_ref_description,
    lop.request_ref_id,
    lop.requested_by,
    lop.request_approved_by,
    lop.subarea_id,
    lop.area_id,
    lop.dsa_id,
    lop.csa_id,
    reg.region_name,
    prov.province_name,
    lop.gis_design_id AS design_id,
    lop.st_x,
    lop.st_y,
    lop.ne_id,
    lop.prms_id,
    lop.mzone_id,
    lop.jc_id,
    lop.served_by_ring,
      CASE
            WHEN lop.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN lop.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN lop.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status
   FROM att_details_loop lop
     LEFT JOIN att_details_cable cbl ON lop.cable_system_id = cbl.system_id
     JOIN province_boundary prov ON lop.province_id = prov.id
     JOIN region_boundary reg ON lop.region_id = reg.id
     JOIN user_master um ON lop.created_by = um.user_id
     LEFT JOIN user_master um2 ON lop.modified_by = um2.user_id
     JOIN point_master point ON point.system_id = lop.system_id AND upper(point.entity_type::text) = upper('Loop'::text);

ALTER TABLE public.vw_att_details_loop_report
    OWNER TO postgres;