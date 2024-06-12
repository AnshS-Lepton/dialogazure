-- public.vw_att_details_cable_vector source

CREATE OR REPLACE VIEW public.vw_att_details_cable_vector
AS SELECT cable.system_id,
    cable.region_id,
    cable.province_id,
    cable.cable_name,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(lm.gis_design_id, ''::character varying) AS gis_design_id,
    cable.cable_type,
    cable.cable_category,
    cable.network_status,
    cable.ownership_type,
    COALESCE(cable.third_party_vendor_id, 0) AS third_party_vendor_id,
    lm.sp_geometry,
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
    cable.gis_design_id AS label_column,
    cable.network_id,
        CASE
            WHEN COALESCE(cable.gis_design_id, ''::character varying)::text = ''::text THEN cable.network_id
            ELSE cable.gis_design_id
        END AS display_name,
    COALESCE(cable.cable_category, ''::character varying) AS entity_category,
    vm.name AS vendor_name,
    ssm.prop_name AS activation_stage
   FROM line_master lm
     JOIN att_details_cable cable ON lm.system_id = cable.system_id AND lm.entity_type::text = 'Cable'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = lm.system_id AND aei.entity_type::text = 'Cable'::text
               LEFT JOIN vendor_master vm ON vm.id = cable.third_party_vendor_id
     LEFT JOIN system_spec_master ssm ON ssm.id = cable.activation
          ;