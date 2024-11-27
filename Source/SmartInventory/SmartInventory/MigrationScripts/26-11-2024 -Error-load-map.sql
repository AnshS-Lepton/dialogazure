CREATE OR REPLACE VIEW public.vw_att_details_manhole_map
 AS
 SELECT manhole.system_id,
    manhole.region_id,
    manhole.province_id,
    COALESCE(st_geomfromtext(((('Point('::text || manhole.longitude) || ' '::text) || manhole.latitude) || ')'::text, 4326), point.sp_geometry) AS sp_geometry,
    manhole.manhole_name,
    manhole.project_id,
    manhole.planning_id,
    manhole.workorder_id,
    manhole.purpose_id,
    manhole.network_id,
    manhole.status,
    manhole.is_new_entity,
    manhole.source_ref_id,
    manhole.source_ref_type,
    manhole.is_virtual,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), manhole.network_status::text) AS network_status,
    manhole.ownership_type,
    manhole.third_party_vendor_id,
    manhole.primary_pod_system_id,
    manhole.secondary_pod_system_id,
    manhole.is_visible_on_map,
    manhole.manhole_types,
        CASE
            WHEN manhole.is_buried = true THEN 1
            ELSE 0
        END AS is_buried,
    manhole.manhole_name AS label_column,
	 lim.icon_path
   FROM point_master point
     JOIN att_details_manhole manhole ON point.system_id = manhole.system_id AND point.entity_type::text = 'Manhole'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = manhole.system_id AND adi1.entity_type::text = 'Manhole'::text AND adi1.description::text = 'NS'::text
 LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), manhole.network_status::text)
 AND lim.is_virtual = manhole.is_virtual AND upper(lim.layer_name::text) = upper('Manhole'::text);

ALTER TABLE public.vw_att_details_manhole_map
    OWNER TO postgres;