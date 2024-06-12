drop view vw_att_details_cable_map ;

CREATE OR REPLACE VIEW public.vw_att_details_cable_map
AS SELECT cable.system_id,
    cable.network_id,
    cable.is_new_entity,
    cable.cable_name,
    cable.a_location,
    cable.b_location,
    cable.total_core,
    cable.no_of_tube,
    cable.no_of_core_per_tube,
    cable.cable_measured_length,
    cable.cable_calculated_length,
    cable.cable_type,
    cable.cable_category,
    cable.construction,
    cable.activation,
    cable.accessibility,
    cable.specification,
    cable.category,
    cable.subcategory1,
    cable.subcategory2,
    cable.subcategory3,
    cable.item_code,
    cable.vendor_id,
    cable.type,
    cable.brand,
    cable.model,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), cable.network_status::text) AS network_status,
    cable.status,
    cable.pin_code,
    cable.ownership_type,
    cable.third_party_vendor_id,
    cable.circuit_id,
    cable.thirdparty_circuit_id,
    cable.province_id,
    cable.region_id,
    cable.project_id,
    cable.planning_id,
    cable.workorder_id,
    cable.purpose_id,
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
    ((cable.total_core || 'F'::text) ||
        CASE
            WHEN cable.cable_type::text = 'Overhead'::text THEN ' OH'::text
            WHEN cable.cable_type::text = 'Underground'::text THEN ' UG'::text
            WHEN cable.cable_type::text = 'Wall Clamped'::text THEN ' WC'::text
            ELSE ''::text
        END) ||
        CASE
            WHEN COALESCE(cable.ownership_type, ''::character varying)::text = 'Third Party'::text THEN 'Third Party'::text
            ELSE ''::text
        END AS cable_cores,
        CASE
            WHEN s.color_code IS NULL THEN
            CASE
                WHEN upper(cable.cable_type::text) = 'OVERHEAD'::text THEN '#FF0000'::text
                WHEN upper(cable.cable_type::text) = 'UNDERGROUND'::text THEN '#0000FF'::text
                WHEN upper(cable.cable_type::text) = 'WALL CLAMPED'::text THEN '#FD10FD'::text
                ELSE '#0000FF'::text
            END::character varying
            ELSE s.color_code
        END AS color_code,
    cable.primary_pod_system_id,
    cable.secondary_pod_system_id,
    cable.source_ref_id,
    cable.source_ref_type,
    CASE
            WHEN lower(b.route_category::text) = 'own'::text OR strpos(lower(b.route_category::text), 'own'::text) > 0 THEN 1
            ELSE 0
        END AS own,
        CASE
            WHEN lower(b.route_category::text) = 'iru'::text OR strpos(lower(b.route_category::text), 'iru'::text) > 0 THEN 1
            ELSE 0
        END AS iru,
        CASE
            WHEN lower(b.cable_owner::text) = 'ttsl'::text OR lower(b.cable_owner::text) = 'ttml'::text OR lower(b.cable_owner::text) = 'bharti'::text OR lower(b.cable_owner::text) = 'rcom'::text OR lower(b.cable_owner::text) = 'depl'::text OR lower(b.cable_owner::text) = 'vtl'::text OR lower(b.cable_owner::text) = 'tcl'::text OR lower(b.cable_owner::text) = 'jio'::text OR lower(b.cable_owner::text) = 'vodafone'::text OR lower(b.cable_owner::text) = 'idea'::text THEN upper(b.cable_owner::text)
            ELSE 'OTHER'::text
        END AS cable_owner,
        CASE
            WHEN lower(spec.prop_value::text) = 'approved'::text OR lower(spec.prop_value::text) = 'completed'::text OR lower(spec.prop_value::text) = 'planning'::text OR lower(spec.prop_value::text) = 'rejected'::text THEN upper(spec.prop_value::text)
            ELSE 'OTHER'::text
        END AS activation_stage,
    upper(bm.brand::text) AS cable_brand,
    1 AS ismapped,
    cable.gis_design_id AS label_column
   FROM line_master lm
     JOIN att_details_cable cable ON lm.system_id = cable.system_id AND lm.entity_type::text = 'Cable'::text
     LEFT JOIN cable_color_settings s ON cable.cable_type::text = s.cable_type::text AND cable.cable_category::text = s.cable_category::text AND cable.total_core = s.fiber_count
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = cable.system_id AND adi.entity_type::text = 'Cable'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = cable.system_id AND adi1.entity_type::text = 'Cable'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN att_details_cable_cdb b ON b.cable_id = cable.system_id
     LEFT JOIN system_spec_master spec ON cable.activation = spec.id
     LEFT JOIN isp_brand_master bm ON bm.id = cable.brand;