drop view vw_att_details_customer_map;

CREATE OR REPLACE VIEW public.vw_att_details_customer_map
AS SELECT customer.system_id,
    customer.region_id,
    customer.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    customer.customer_name,
    customer.network_id,
    customer.customer_type,
    customer.primary_pod_system_id,
    customer.secondary_pod_system_id,
    customer.source_ref_id,
    customer.source_ref_type,
    customer.project_id,
    customer.planning_id,
    customer.workorder_id,
    customer.purpose_id,
    lim.icon_path,
    customer.network_id AS label_column
   FROM point_master point
     JOIN att_details_customer customer ON point.system_id = customer.system_id AND point.entity_type::text = 'Customer'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = customer.network_status::text AND COALESCE(customer.customer_type, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND lim.layer_name::text = 'Customer'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = customer.system_id AND adi.entity_type::text = 'Customer'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;




     