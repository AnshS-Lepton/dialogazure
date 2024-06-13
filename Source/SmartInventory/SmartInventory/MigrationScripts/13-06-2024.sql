INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('IsVectorLayerEnabled', '1', 'Vector layer enabled for entity rendering on map', 'Web', false, 'string', 1.0, 1.0, 1, now(), NULL, NULL, true, true, false, NULL, NULL);
-------------------------------------------------------------

DROP VIEW public.vw_att_details_adb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_adb_vector
 AS
 SELECT adb.system_id,
    adb.region_id,
    adb.province_id,
    adb.status,
    adb.is_new_entity,
    adb.source_ref_id,
    adb.source_ref_type,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    adb.adb_name,
    adb.network_id,
    adb.ownership_type,
    COALESCE(adb.third_party_vendor_id, 0) AS third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), adb.network_status::text) AS network_status,
    adb.gis_design_id AS label_column,
        CASE
            WHEN COALESCE(adb.gis_design_id, ''::character varying)::text = ''::text THEN adb.network_id
            ELSE adb.gis_design_id
        END::character varying AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_adb adb ON point.system_id = adb.system_id AND point.entity_type::text = 'ADB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = adb.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = adb.system_id AND adi1.entity_type::text = 'ADB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'ADB'::text;

ALTER TABLE public.vw_att_details_adb_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_antenna_vector;

CREATE OR REPLACE VIEW public.vw_att_details_antenna_vector
 AS
 SELECT antenna.system_id,
    antenna.region_id,
    antenna.province_id,
     antenna.status,
    antenna.is_new_entity,
     antenna.source_ref_id,
    antenna.source_ref_type,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(pm.gis_design_id, ''::character varying) AS gis_design_id,
    antenna.network_id AS antenna_name,
    antenna.network_status,
    pm.sp_geometry,
    antenna.network_id AS label_column,
    antenna.network_id,
    antenna.network_id AS display_name,
    antenna.ownership_type,
    COALESCE(antenna.third_party_vendor_id, 0) AS third_party_vendor_id
   FROM point_master pm
     JOIN att_details_antenna antenna ON pm.system_id = antenna.system_id AND pm.entity_type::text = 'Antenna'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = pm.system_id AND aei.entity_type::text = 'Antenna'::text;

ALTER TABLE public.vw_att_details_antenna_vector
    OWNER TO postgres;




DROP VIEW public.vw_att_details_cable_vector;

CREATE OR REPLACE VIEW public.vw_att_details_cable_vector
 AS
 SELECT cable.system_id,
    cable.region_id,
    cable.province_id,
    cable.cable_name,
     cable.status,
    cable.is_new_entity,
     cable.source_ref_id,
    cable.source_ref_type,
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
     LEFT JOIN system_spec_master ssm ON ssm.id = cable.activation;

ALTER TABLE public.vw_att_details_cable_vector
    OWNER TO postgres;




DROP VIEW public.vw_att_details_manhole_vector;

CREATE OR REPLACE VIEW public.vw_att_details_manhole_vector
 AS
 SELECT manhole.system_id,
    manhole.region_id,
    manhole.province_id,
    manhole.status,
    manhole.is_new_entity,
      manhole.source_ref_id,
    manhole.source_ref_type,
    COALESCE(pm.system_id, 0) AS subarea_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    manhole.manhole_name,
    manhole.category,
    manhole.network_id,
    manhole.network_status,
    manhole.ownership_type,
    COALESCE(manhole.third_party_vendor_id, 0) AS third_party_vendor_id,
        CASE
            WHEN COALESCE(manhole.gis_design_id, ''::character varying)::text = ''::text THEN manhole.network_id
            ELSE manhole.gis_design_id
        END::character varying AS display_name,
    point.entity_category,
    COALESCE(manhole.is_virtual, false) AS is_virtual
   FROM point_master point
     JOIN att_details_manhole manhole ON point.system_id = manhole.system_id AND point.entity_type::text = 'Manhole'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Manhole'::text;

ALTER TABLE public.vw_att_details_manhole_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_pole_vector;

CREATE OR REPLACE VIEW public.vw_att_details_pole_vector
 AS
 SELECT pole.system_id,
    pole.region_id,
    pole.province_id,
    pole.status,
    pole.is_new_entity,
         pole.source_ref_id,
    pole.source_ref_type,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    pole.pole_name,
    pole.pole_type,
    pole.network_id,
    pole.network_status,
    pole.ownership_type,
    COALESCE(pole.third_party_vendor_id, 0) AS third_party_vendor_id,
        CASE
            WHEN COALESCE(pole.gis_design_id, ''::character varying)::text = ''::text THEN pole.network_id
            ELSE pole.gis_design_id
        END::character varying AS display_name,
    pole.pole_type AS entity_category
   FROM point_master point
     JOIN att_details_pole pole ON point.system_id = pole.system_id AND point.entity_type::text = 'Pole'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Pole'::text;

ALTER TABLE public.vw_att_details_pole_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_area_vector;

CREATE OR REPLACE VIEW public.vw_att_details_area_vector
 AS
 SELECT area.system_id,
    area.area_name,
    plgn.sp_geometry,
    COALESCE(plgn.gis_design_id, ''::character varying) AS gis_design_id,
    area.region_id,
    area.province_id,
    area.network_status,
    area.network_id,
    plgn.display_name,
    area.status,
    area.is_new_entity,
    area.source_ref_id,
    area.source_ref_type
   FROM polygon_master plgn
     JOIN att_details_area area ON plgn.system_id = area.system_id AND plgn.entity_type::text = 'Area'::text;

ALTER TABLE public.vw_att_details_area_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_bdb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_bdb_vector
 AS
 SELECT bdb.system_id,
    bdb.region_id,
    bdb.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    bdb.bdb_name,
    bdb.network_id,
    bdb.ownership_type,
    COALESCE(bdb.third_party_vendor_id, 0) AS third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), bdb.network_status::text) AS network_status,
    bdb.gis_design_id AS label_column,
        CASE
            WHEN COALESCE(bdb.gis_design_id, ''::character varying)::text = ''::text THEN bdb.network_id
            ELSE bdb.gis_design_id
        END::character varying AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category,
       bdb.status,
    bdb.is_new_entity,
    bdb.source_ref_id,
    bdb.source_ref_type
   FROM point_master point
     JOIN att_details_bdb bdb ON point.system_id = bdb.system_id AND point.entity_type::text = 'BDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = bdb.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = bdb.system_id AND adi1.entity_type::text = 'BDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'BDB'::text;

ALTER TABLE public.vw_att_details_bdb_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_building_vector;

CREATE OR REPLACE VIEW public.vw_att_details_building_vector
 AS
 SELECT building.system_id,
    building.region_id,
    building.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    building.building_name,
    building.network_id,
    building.status,
    building.building_status,
    building.rfs_status,
    building.category,    
    building.is_new_entity,
    building.source_ref_id,
    building.source_ref_type,
    building.network_status::text AS network_status,
    building.gis_design_id AS label_column,
        CASE
            WHEN COALESCE(building.gis_design_id, ''::character varying)::text = ''::text THEN building.network_id
            ELSE building.gis_design_id
        END AS display_name,
    NULL::text AS entity_category,
    COALESCE(polygon.sp_geometry, point.sp_geometry) AS sp_geometry
   FROM att_details_building building
     LEFT JOIN point_master point ON point.system_id = building.system_id AND point.entity_type::text = 'Building'::text
     LEFT JOIN polygon_master polygon ON polygon.system_id = building.system_id AND polygon.entity_type::text = 'Building'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Building'::text;

ALTER TABLE public.vw_att_details_building_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_cabinet_vector;

CREATE OR REPLACE VIEW public.vw_att_details_cabinet_vector
 AS
 SELECT cabinet.system_id,
    cabinet.region_id,
    cabinet.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(pm.system_id, 0) AS subarea_id,
    cabinet.cabinet_name,
    cabinet.network_id,
    cabinet.network_status,
    point.display_name,
    cabinet.status,
    cabinet.is_new_entity,
    cabinet.source_ref_id,
    cabinet.source_ref_type,
    point.entity_category,
    cabinet.ownership_type,
    point.sp_geometry,
    COALESCE(cabinet.third_party_vendor_id, 0) AS third_party_vendor_id
   FROM point_master point
     JOIN att_details_cabinet cabinet ON point.system_id = cabinet.system_id AND point.entity_type::text = 'Cabinet'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Cabinet'::text;

ALTER TABLE public.vw_att_details_cabinet_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_cdb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_cdb_vector
 AS
 SELECT cdb.system_id,
    cdb.region_id,
    cdb.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    cdb.cdb_name,
    cdb.network_id,
       cdb.status,
    cdb.is_new_entity,
    cdb.source_ref_id,
    cdb.source_ref_type,
    cdb.ownership_type,
    COALESCE(cdb.third_party_vendor_id, 0) AS third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), cdb.network_status::text) AS network_status,
    cdb.gis_design_id AS label_column,
    point.display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_cdb cdb ON point.system_id = cdb.system_id AND point.entity_type::text = 'CDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = cdb.system_id AND adi1.entity_type::text = 'CDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = cdb.system_id AND adi.entity_type::text = 'CDB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'CDB'::text;

ALTER TABLE public.vw_att_details_cdb_vector
    OWNER TO postgres;




DROP VIEW public.vw_att_details_csa_vector;

CREATE OR REPLACE VIEW public.vw_att_details_csa_vector
 AS
 SELECT csa.system_id,
    csa.csa_name,
    plgn.sp_geometry,
    csa.region_id,
    COALESCE(plgn.gis_design_id, ''::character varying) AS gis_design_id,
    csa.province_id,
    csa.network_status,
       csa.status,
    csa.is_new_entity,
    csa.source_ref_id,
    csa.source_ref_type,
    csa.network_id,
    plgn.display_name,
    csa.rfs_status
   FROM polygon_master plgn
     JOIN att_details_csa csa ON plgn.system_id = csa.system_id AND plgn.entity_type::text = 'CSA'::text;

ALTER TABLE public.vw_att_details_csa_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_customer_vector;

CREATE OR REPLACE VIEW public.vw_att_details_customer_vector
 AS
 SELECT customer.system_id,
    customer.region_id,
    customer.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    customer.customer_name,
    customer.network_id,
    customer.status,
    customer.is_new_entity,     
    customer.customer_type,
    customer.primary_pod_system_id,
    customer.secondary_pod_system_id,
    customer.source_ref_id,
    customer.source_ref_type,
    lim.icon_path,
    customer.network_id AS label_column,
    customer.network_status,
    ''::text AS entity_category,
        CASE
            WHEN COALESCE(customer.gis_design_id, ''::character varying)::text = ''::text THEN customer.network_id
            ELSE customer.gis_design_id
        END AS display_name
   FROM point_master point
     JOIN att_details_customer customer ON point.system_id = customer.system_id AND upper(point.entity_type::text) = 'CUSTOMER'::text
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = customer.network_status::text AND COALESCE(customer.customer_type, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND upper(lim.layer_name::text) = upper('CUSTOMER'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = customer.system_id AND upper(adi.entity_type::text) = upper('CUSTOMER'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Customer'::text;

ALTER TABLE public.vw_att_details_customer_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_dsa_vector;

CREATE OR REPLACE VIEW public.vw_att_details_dsa_vector
 AS
 SELECT dsa.system_id,
    dsa.dsa_name,
    plgn.sp_geometry,
    dsa.region_id,
    dsa.status,
    dsa.is_new_entity,
    dsa.source_ref_id,
    dsa.source_ref_type,
    COALESCE(plgn.gis_design_id, ''::character varying) AS gis_design_id,
    dsa.province_id,
    dsa.network_status,
    dsa.network_id,
    plgn.display_name
   FROM polygon_master plgn
     JOIN att_details_dsa dsa ON plgn.system_id = dsa.system_id AND plgn.entity_type::text = 'DSA'::text;

ALTER TABLE public.vw_att_details_dsa_vector
    OWNER TO postgres;




DROP VIEW public.vw_att_details_subarea_vector;

CREATE OR REPLACE VIEW public.vw_att_details_subarea_vector
 AS
 SELECT subarea.system_id,
    subarea.subarea_name,
    subarea.sp_geometry,
    subarea.region_id,
    COALESCE(subarea.gis_design_id, ''::character varying) AS gis_design_id,
    subarea.province_id,
    subarea.network_status,
    subarea.network_id,
    subarea.display_name,
       subarea.status,
    subarea.is_new_entity,
    subarea.source_ref_id,
    subarea.source_ref_type,
    clr.color_code
   FROM ( SELECT subarea_1.system_id,
            subarea_1.region_id,
            subarea_1.province_id,
            plgn.sp_geometry,
            plgn.display_name,
            subarea_1.subarea_name,
            subarea_1.network_id,
            subarea_1.parent_system_id,
               subarea_1.status,
    subarea_1.is_new_entity,
    subarea_1.source_ref_id,
    subarea_1.source_ref_type,
            row_number() OVER (PARTITION BY subarea_1.parent_system_id, subarea_1.parent_entity_type ORDER BY subarea_1.created_on) AS bg_color_id,
            subarea_1.network_status,
            plgn.gis_design_id
           FROM polygon_master plgn
             JOIN att_details_subarea subarea_1 ON plgn.system_id = subarea_1.system_id AND upper(plgn.entity_type::text) = 'SUBAREA'::text) subarea
     LEFT JOIN subarea_color_master clr ON
        CASE
            WHEN (subarea.bg_color_id % 5::bigint) = 0 THEN 1::bigint
            ELSE subarea.bg_color_id % 5::bigint
        END = clr.color_id;

ALTER TABLE public.vw_att_details_subarea_vector
    OWNER TO postgres;




DROP VIEW public.vw_att_details_network_ticket_vector;

CREATE OR REPLACE VIEW public.vw_att_details_network_ticket_vector
 AS
 SELECT adn.ticket_id AS system_id,
    adn.name,
    COALESCE(plgn.sp_geometry, pb.sp_geometry) AS sp_geometry,
    adn.region_id,
    adn.province_id,
    adn.status,
    adn.network_id,   
    case when adn.ticket_status_id=8 then false::boolean else true::boolean end  is_new_entity,
    adn.ticket_id::character varying AS source_ref_id,
    'Network_Ticket'::character varying AS source_ref_type,
        CASE
            WHEN COALESCE(plgn.display_name, ''::character varying)::text = ''::text THEN adn.network_id
            ELSE plgn.display_name
        END AS display_name
   FROM att_details_networktickets adn
     LEFT JOIN polygon_master plgn ON plgn.system_id = adn.ticket_id AND plgn.entity_type::text = 'Network_Ticket'::text
     LEFT JOIN province_boundary pb ON pb.id = adn.province_id;

ALTER TABLE public.vw_att_details_network_ticket_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_duct_vector;

CREATE OR REPLACE VIEW public.vw_att_details_duct_vector
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.duct_name,
    duct.pin_code,  
    duct.is_new_entity,   
    COALESCE(pmapping.province_id, duct.province_id) AS province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(lm.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(pmapping.region_id, duct.region_id) AS region_id,
    duct.a_location,
    duct.b_location,
    duct.calculated_length,
    duct.manual_length,
    duct.construction,
    duct.activation,
    duct.accessibility,
    duct.specification,
    duct.category,
    duct.vendor_id,
    duct.type,
    duct.brand,
    duct.model,
    duct.created_by,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), duct.network_status::text) AS network_status,
    duct.project_id,
    duct.planning_id,
    duct.workorder_id,
    duct.purpose_id,
    duct.status,
    duct.duct_type,
    duct.color_code,
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
        CASE
            WHEN duct.no_of_cables = 0 THEN 'vacant'::text
            ELSE 'used'::text
        END AS utilization,
    duct.ownership_type,
    COALESCE(duct.third_party_vendor_id, 0) AS third_party_vendor_id,
    duct.primary_pod_system_id,
    duct.secondary_pod_system_id,
    duct.source_ref_id,
    duct.source_ref_type,
    duct.duct_name AS label_column,
    COALESCE(duct.category, ''::character varying) AS entity_category,
        CASE
            WHEN COALESCE(duct.gis_design_id, ''::character varying)::text = ''::text THEN duct.network_id
            ELSE duct.gis_design_id
        END AS display_name
   FROM line_master lm
     JOIN att_details_duct duct ON lm.system_id = duct.system_id AND lm.entity_type::text = 'Duct'::text
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(duct.network_id::text) ~~ 'NLD%'::text THEN duct.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('Duct'::text)
            ELSE 1 = 2
        END
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = duct.system_id AND upper(adi.entity_type::text) = upper('Duct'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = duct.system_id AND upper(adi1.entity_type::text) = upper('DUCT'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = lm.system_id AND aei.entity_type::text = 'Duct'::text;

ALTER TABLE public.vw_att_details_duct_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_fault_vector;

CREATE OR REPLACE VIEW public.vw_att_details_fault_vector
 AS
 SELECT antenna.system_id,
    antenna.region_id,
    antenna.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(pm.gis_design_id, ''::character varying) AS gis_design_id,
    antenna.network_id AS fault_name,
    antenna.network_status,
    pm.sp_geometry,
    antenna.network_id::character varying AS label_column,
    antenna.network_id,
    antenna.network_id AS display_name,
    antenna.status,
    antenna.is_new_entity,
    antenna.source_ref_id,
    antenna.source_ref_type
   FROM point_master pm
     JOIN att_details_fault antenna ON pm.system_id = antenna.system_id AND pm.entity_type::text = 'Fault'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = pm.system_id AND aei.entity_type::text = 'Fault'::text;

ALTER TABLE public.vw_att_details_fault_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_fdb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_fdb_vector
 AS
 SELECT fdb.system_id,
    fdb.region_id,
    fdb.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    fdb.fdb_name,
    fdb.network_id,
    fdb.ownership_type,
       fdb.status,
    fdb.is_new_entity,
    fdb.source_ref_id,
    fdb.source_ref_type,
    COALESCE(fdb.third_party_vendor_id, 0) AS third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), fdb.network_status::text) AS network_status,
    fdb.gis_design_id AS label_column,
        CASE
            WHEN COALESCE(fdb.gis_design_id, ''::character varying)::text = ''::text THEN fdb.network_id
            ELSE fdb.gis_design_id
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN isp_fdb_info fdb ON point.system_id = fdb.system_id AND point.entity_type::text = 'FDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = fdb.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = fdb.system_id AND adi1.entity_type::text = 'FDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'FDB'::text;

ALTER TABLE public.vw_att_details_fdb_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_fms_vector;

CREATE OR REPLACE VIEW public.vw_att_details_fms_vector
 AS
 SELECT fms.system_id,
    fms.region_id,
    fms.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    fms.fms_name,
    fms.network_id,
    fms.network_status::text AS network_status,
    fms.network_id AS label_column,
    point.sp_geometry,
    fms.ownership_type,
    fms.status,
    fms.is_new_entity,
    fms.source_ref_id,
    fms.source_ref_type,
    COALESCE(fms.third_party_vendor_id, 0) AS third_party_vendor_id,
        CASE
            WHEN COALESCE(fms.gis_design_id, ''::character varying)::text = ''::text THEN fms.network_id
            ELSE fms.gis_design_id
        END::character varying AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_fms fms ON point.system_id = fms.system_id AND upper(point.entity_type::text) = 'FMS'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'FMS'::text;

ALTER TABLE public.vw_att_details_fms_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_handhole_vector;

CREATE OR REPLACE VIEW public.vw_att_details_handhole_vector
 AS
 SELECT handhole.system_id,
    handhole.region_id,
    handhole.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    handhole.subarea_system_id AS subarea_id,
    point.sp_geometry,
    handhole.handhole_name,
    handhole.category,
    handhole.network_id,
    handhole.network_status,
    point.display_name,
    point.entity_category,
    handhole.ownership_type,
    handhole.status,
    handhole.is_new_entity,
    handhole.source_ref_id,
    handhole.source_ref_type,
    COALESCE(handhole.third_party_vendor_id, 0) AS third_party_vendor_id,
    COALESCE(handhole.is_virtual, false) AS is_virtual
   FROM point_master point
     JOIN att_details_handhole handhole ON point.system_id = handhole.system_id AND point.entity_type::text = 'Handhole'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Handhole'::text;

ALTER TABLE public.vw_att_details_handhole_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_htb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_htb_vector
 AS
 SELECT htb.system_id,
    htb.region_id,
    htb.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    htb.htb_name,
    htb.htb_type,
    htb.network_status,
    htb.type,
    htb.category,
    point.display_name,
    point.entity_category,
    htb.ownership_type,
       htb.status,
    htb.is_new_entity,
    htb.source_ref_id,
    htb.source_ref_type,
    COALESCE(htb.third_party_vendor_id, 0) AS third_party_vendor_id
   FROM point_master point
     JOIN isp_htb_info htb ON point.system_id = htb.system_id AND point.entity_type::text = 'HTB'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'HTB'::text;

ALTER TABLE public.vw_att_details_htb_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_loop_vector;

CREATE OR REPLACE VIEW public.vw_att_details_loop_vector
 AS
 SELECT loop.system_id,
    loop.region_id,
    loop.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(pm.gis_design_id, ''::character varying) AS gis_design_id,
    loop.network_id AS loop_name,
    loop.loop_length,
    loop.network_status,
    pm.sp_geometry,
       loop.status,
    loop.is_new_entity,
    loop.source_ref_id,
    loop.source_ref_type,
    loop.network_id::character varying AS label_column,
    loop.network_id,
    loop.network_id AS display_name
   FROM point_master pm
     JOIN att_details_loop loop ON pm.system_id = loop.system_id AND pm.entity_type::text = 'Loop'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = pm.system_id AND aei.entity_type::text = 'Loop'::text;

ALTER TABLE public.vw_att_details_loop_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_microduct_vector;

CREATE OR REPLACE VIEW public.vw_att_details_microduct_vector
 AS
 SELECT microduct.system_id,
    microduct.network_id,
    microduct.microduct_name,
    microduct.pin_code,
    COALESCE(pmapping.province_id, microduct.province_id) AS province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(lm.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(pmapping.region_id, microduct.region_id) AS region_id,
    microduct.a_location,
    microduct.b_location,
    microduct.calculated_length,
    microduct.manual_length,
    microduct.construction,
    microduct.activation,
    microduct.accessibility,
    microduct.specification,
    microduct.category,
    microduct.vendor_id,
    microduct.type,
    microduct.brand,
    microduct.model,
    microduct.created_by,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), microduct.network_status::text) AS network_status,
    microduct.project_id,
    microduct.planning_id,
    microduct.workorder_id,
    microduct.purpose_id,
    microduct.status,
    microduct.microduct_type,
    microduct.color_code,   
    microduct.is_new_entity,
    microduct.source_ref_id,
    microduct.source_ref_type,
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
        CASE
            WHEN microduct.no_of_cables = 0 THEN 'vacant'::text
            ELSE 'used'::text
        END AS utilization,
    microduct.ownership_type,
    COALESCE(microduct.third_party_vendor_id, 0) AS third_party_vendor_id,
    microduct.primary_pod_system_id,
    microduct.secondary_pod_system_id,  
    microduct.microduct_name AS label_column,
    COALESCE(microduct.category, ''::character varying) AS entity_category,
        CASE
            WHEN COALESCE(microduct.gis_design_id, ''::character varying)::text = ''::text THEN microduct.network_id
            ELSE microduct.gis_design_id
        END AS display_name
   FROM line_master lm
     JOIN att_details_microduct microduct ON lm.system_id = microduct.system_id AND lm.entity_type::text = 'Microduct'::text
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(microduct.network_id::text) ~~ 'NLD%'::text THEN microduct.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('Microduct'::text)
            ELSE 1 = 2
        END
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = microduct.system_id AND upper(adi.entity_type::text) = upper('Microduct'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = microduct.system_id AND upper(adi1.entity_type::text) = upper('Microduct'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = lm.system_id AND aei.entity_type::text = 'Microduct'::text;

ALTER TABLE public.vw_att_details_microduct_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_ont_vector;

CREATE OR REPLACE VIEW public.vw_att_details_ont_vector
 AS
 SELECT ont.system_id,
    ont.region_id,
    ont.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    ont.ont_name,
    ont.network_id,
    ont.ownership_type,
       ont.status,
    ont.is_new_entity,
    ont.source_ref_id,
    ont.source_ref_type,
    COALESCE(ont.third_party_vendor_id, 0) AS third_party_vendor_id,
    ont.network_status::text AS network_status,
    ont.network_id AS label_column,
    point.sp_geometry,
        CASE
            WHEN COALESCE(ont.gis_design_id, ''::character varying)::text = ''::text THEN ont.network_id
            ELSE ont.gis_design_id
        END::character varying AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_ont ont ON point.system_id = ont.system_id AND upper(point.entity_type::text) = 'ONT'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'ONT'::text;

ALTER TABLE public.vw_att_details_ont_vector
    OWNER TO postgres;




DROP VIEW public.vw_att_details_patchpanel_vector;

CREATE OR REPLACE VIEW public.vw_att_details_patchpanel_vector
 AS
 SELECT patchpanel.system_id,
    patchpanel.region_id,
    patchpanel.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    patchpanel.patchpanel_name,
    patchpanel.project_id,
    patchpanel.planning_id,
    patchpanel.workorder_id,
    patchpanel.purpose_id,
    patchpanel.network_id,
    patchpanel.status,
    patchpanel.is_new_entity,
    patchpanel.source_ref_id,
    patchpanel.source_ref_type,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), patchpanel.network_status::text) AS network_status,
    patchpanel.ownership_type,
    COALESCE(patchpanel.third_party_vendor_id, 0) AS third_party_vendor_id,
    patchpanel.primary_pod_system_id,
    patchpanel.secondary_pod_system_id,   
    patchpanel.network_id AS label_column,
        CASE
            WHEN COALESCE(patchpanel.gis_design_id, ''::character varying)::text = ''::text THEN patchpanel.network_id
            ELSE patchpanel.gis_design_id
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_patchpanel patchpanel ON point.system_id = patchpanel.system_id AND upper(point.entity_type::text) = 'PATCHPANEL'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = patchpanel.system_id AND upper(adi1.entity_type::text) = upper('patchpanel'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = patchpanel.system_id AND upper(adi.entity_type::text) = upper('PATCHPANEL'::text)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND upper(aei.entity_type::text) = 'PATCHPANEL'::text;

ALTER TABLE public.vw_att_details_patchpanel_vector
    OWNER TO postgres;




DROP VIEW public.vw_att_details_pod_vector;

CREATE OR REPLACE VIEW public.vw_att_details_pod_vector
 AS
 SELECT pod.system_id,
    pod.region_id,
    pod.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    pod.pod_name,
    pod.network_id,
       pod.status,
    pod.is_new_entity,
    pod.source_ref_id,
    pod.source_ref_type,
    pod.network_status::text AS network_status,
    pod.gis_design_id AS label_column,
    pod.ownership_type,
    COALESCE(pod.third_party_vendor_id, 0) AS third_party_vendor_id,
        CASE
            WHEN COALESCE(pod.gis_design_id, ''::character varying)::text = ''::text THEN pod.network_id
            ELSE pod.gis_design_id
        END::character varying AS display_name,
    point.entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN att_details_pod pod ON point.system_id = pod.system_id AND point.entity_type::text = 'POD'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'POD'::text;

ALTER TABLE public.vw_att_details_pod_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_rack_vector;

CREATE OR REPLACE VIEW public.vw_att_details_rack_vector
 AS
 SELECT rack.system_id,
    rack.region_id,
    rack.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    rack.rack_name,
    rack.rack_type,
    rack.category,
    rack.no_of_units,
    rack.structure_id,   
    rack.network_id,
    rack.network_status,
    rack.status,
    rack.is_new_entity,
    rack.source_ref_id,
    rack.source_ref_type,
        CASE
            WHEN COALESCE(rack.gis_design_id, ''::character varying)::text = ''::text THEN rack.network_id
            ELSE rack.gis_design_id
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN att_details_rack rack ON point.system_id = rack.system_id AND point.entity_type::text = 'Rack'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Rack'::text;

ALTER TABLE public.vw_att_details_rack_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_row_vector;

CREATE OR REPLACE VIEW public.vw_att_details_row_vector
 AS
 SELECT "row".system_id,
    "row".network_id,
    "row".row_name,
    COALESCE(adi.updated_geom, plgn.sp_geometry) AS sp_geometry,
    plgn.geom_type::character varying AS geom_type,
    "row".region_id,
    "row".province_id,
    plgn.row_text,
    plgn.center_line_geom,
    "row".project_id,
    "row".planning_id,
    "row".workorder_id,
    "row".purpose_id,
       "row".status,
    "row".is_new_entity,    
        CASE
            WHEN upper("row".row_stage::text) = upper('NEW'::text) THEN '#ffff00'::text
            WHEN upper("row".row_stage::text) = upper('Applied'::text) THEN '#fac803'::text
            WHEN upper("row".row_stage::text) = upper('Approved'::text) AND (EXISTS ( SELECT 1
               FROM att_details_row_associate_entity_info
              WHERE att_details_row_associate_entity_info.parent_system_id = "row".system_id AND upper(att_details_row_associate_entity_info.parent_entity_type::text) = upper('ROW'::text))) THEN '#02a3f4'::text
            WHEN upper("row".row_stage::text) = upper('Approved'::text) THEN '#05f66f'::text
            WHEN upper("row".row_stage::text) = upper('Rejected'::text) THEN '#f84c4c'::text
            ELSE NULL::text
        END AS color_code,
    "row".source_ref_id,
    "row".source_ref_type,
    "row".network_id AS label_column,
    ''::text AS entity_category,
        CASE
            WHEN COALESCE("row".gis_design_id, ''::character varying)::text = ''::text THEN "row".network_id
            ELSE "row".gis_design_id
        END AS display_name,
    'A'::text AS network_status
   FROM ( SELECT polygon_master.system_id,
            polygon_master.entity_type,
            polygon_master.sp_geometry,
            polygon_master.center_line_geom,
            'Polygon'::text AS geom_type,
            ''::text AS row_text
           FROM polygon_master
        UNION
         SELECT circle_master.system_id,
            circle_master.entity_type,
            circle_master.sp_geometry,
            NULL::geometry AS center_line_geom,
            'Circle'::text AS geom_type,
            ('Radius:'::text || circle_master.radious) || '(Mtr)'::text AS row_text
           FROM circle_master) plgn
     JOIN att_details_row "row" ON plgn.system_id = "row".system_id AND upper(plgn.entity_type::text) = 'ROW'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = "row".system_id AND upper(adi.entity_type::text) = upper('ROW'::text);

ALTER TABLE public.vw_att_details_row_vector
    OWNER TO postgres;




DROP VIEW public.vw_att_details_sector_vector;

CREATE OR REPLACE VIEW public.vw_att_details_sector_vector
 AS
 SELECT sector.system_id,
    sector.region_id,
    sector.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    sector.network_name,
    sector.project_id,
    sector.planning_id,
    sector.workorder_id,
    sector.purpose_id,
    sector.network_id,
       sector.status,
    sector.is_new_entity,
 
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), sector.network_status::text) AS network_status,
    sector.ownership_type,
    COALESCE(sector.third_party_vendor_id, 0) AS third_party_vendor_id,
    sector.source_ref_id,
    sector.source_ref_type,
    sector.network_id AS label_column,
        CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN sector.network_id
            ELSE point.display_name
        END AS display_name
   FROM polygon_master point
     JOIN att_details_sector sector ON point.system_id = sector.system_id AND upper(point.entity_type::text) = 'SECTOR'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = sector.system_id AND upper(adi1.entity_type::text) = 'SECTOR'::text AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = sector.system_id AND upper(adi.entity_type::text) = 'SECTOR'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND upper(aei.entity_type::text) = 'SECTOR'::text;

ALTER TABLE public.vw_att_details_sector_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_slack_vector;

CREATE OR REPLACE VIEW public.vw_att_details_slack_vector
 AS
 SELECT slack.system_id,
    slack.region_id,
    slack.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    slack.project_id,
    slack.planning_id,
    slack.workorder_id,
    slack.purpose_id,
    slack.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), slack.network_status::text) AS network_status,
    slack.source_ref_id,
    slack.source_ref_type,
    slack.network_id AS label_column,
       slack.status,
    slack.is_new_entity,
   
        CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN slack.network_id
            ELSE point.display_name
        END AS display_name
   FROM point_master point
     JOIN att_details_slack slack ON point.system_id = slack.system_id AND upper(point.entity_type::text) = 'SLACK'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = slack.system_id AND upper(adi1.entity_type::text) = upper('slack'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = slack.system_id AND upper(adi.entity_type::text) = upper('slack'::text)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND upper(aei.entity_type::text) = 'SLACK'::text;

ALTER TABLE public.vw_att_details_slack_vector
    OWNER TO postgres;



 DROP VIEW public.vw_att_details_spliceclosure_vector;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_vector
 AS
 SELECT spliceclosure.system_id,
    spliceclosure.region_id,
    spliceclosure.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    spliceclosure.spliceclosure_name,
    spliceclosure.ownership_type,
       spliceclosure.status,
    spliceclosure.is_new_entity,
    spliceclosure.source_ref_id,
    spliceclosure.source_ref_type,
    COALESCE(spliceclosure.third_party_vendor_id, 0) AS third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), spliceclosure.network_status::text) AS network_status,
    spliceclosure.network_id,
        CASE
            WHEN COALESCE(spliceclosure.gis_design_id, ''::character varying)::text = ''::text THEN spliceclosure.network_id
            ELSE spliceclosure.gis_design_id
        END::character varying AS display_name,
    point.entity_category,
    COALESCE(spliceclosure.is_virtual, false) AS is_virtual
   FROM point_master point
     JOIN att_details_spliceclosure spliceclosure ON point.system_id = spliceclosure.system_id AND upper(point.entity_type::text) = 'SPLICECLOSURE'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = spliceclosure.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = spliceclosure.system_id AND upper(adi1.entity_type::text) = upper('SPLICECLOSURE'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND upper(aei.entity_type::text) = 'SPLICECLOSURE'::text;

ALTER TABLE public.vw_att_details_spliceclosure_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_splitter_vector;

CREATE OR REPLACE VIEW public.vw_att_details_splitter_vector
 AS
 SELECT splitter.system_id,
    splitter.region_id,
    splitter.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    splitter.splitter_name,
    splitter.splitter_type,
    splitter.ownership_type,
       splitter.status,
    splitter.is_new_entity,
    splitter.source_ref_id,
    splitter.source_ref_type,
    COALESCE(splitter.third_party_vendor_id, 0) AS third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), splitter.network_status::text) AS network_status,
    splitter.network_id,
        CASE
            WHEN COALESCE(splitter.gis_design_id, ''::character varying)::text = ''::text THEN splitter.network_id
            ELSE splitter.gis_design_id
        END::character varying AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_splitter splitter ON point.system_id = splitter.system_id AND upper(point.entity_type::text) = 'SPLITTER'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = splitter.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = splitter.system_id AND upper(adi1.entity_type::text) = upper('SPLITTER'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Splitter'::text
  ORDER BY (
        CASE
            WHEN splitter.splitter_type::text = 'Primary'::text THEN 1
            ELSE 0
        END);

ALTER TABLE public.vw_att_details_splitter_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_structure_vector;

CREATE OR REPLACE VIEW public.vw_att_details_structure_vector
 AS
 SELECT structure.system_id,
    plgn.sp_geometry,
    structure.structure_name,
    structure.network_id,
    structure.building_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), structure.network_status::text) AS network_status,
    structure.status,
    structure.region_id,
    structure.province_id,
    structure.business_pass,
    structure.home_pass,
    structure.no_of_floor,
    structure.no_of_flat,
    structure.no_of_shaft,  
    structure.is_new_entity,   
    plgn.geom_type,
    plgn.sp_centroid,
    structure.source_ref_id,
    structure.source_ref_type,
    lim.icon_path,
    structure.structure_name AS label_column,
    ''::text AS entity_category,
        CASE
            WHEN COALESCE(structure.gis_design_id, ''::character varying)::text = ''::text THEN structure.network_id
            ELSE structure.gis_design_id
        END AS display_name
   FROM ( SELECT tbl.system_id,
            tbl.sp_geometry,
            tbl.sp_centroid,
            tbl.geom_type,
            tbl.order_colum,
            tbl.display_name,
            row_number() OVER (PARTITION BY tbl.system_id ORDER BY tbl.order_colum) AS row_num
           FROM ( SELECT point_master.system_id,
                    point_master.sp_geometry,
                    NULL::geometry AS sp_centroid,
                    'Point'::text AS geom_type,
                    2 AS order_colum,
                    point_master.display_name
                   FROM point_master
                  WHERE upper(point_master.entity_type::text) = upper('Structure'::text)
                UNION
                 SELECT polygon_master.system_id,
                    polygon_master.sp_geometry,
                    st_centroid(polygon_master.sp_geometry) AS sp_centroid,
                    'Polygon'::text AS text,
                    1 AS order_colum,
                    polygon_master.display_name
                   FROM polygon_master
                  WHERE upper(polygon_master.entity_type::text) = upper('Structure'::text)) tbl) plgn
     JOIN att_details_bld_structure structure ON plgn.system_id = structure.system_id AND plgn.row_num = 1
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = structure.system_id AND upper(adi1.entity_type::text) = upper('Structure'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), structure.network_status::text) AND upper(lim.layer_name::text) = upper('Structure'::text);

ALTER TABLE public.vw_att_details_structure_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_tower_vector;

CREATE OR REPLACE VIEW public.vw_att_details_tower_vector
 AS
 SELECT tower.system_id,
    tower.region_id,
    tower.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(pm.system_id, 0) AS subarea_id,
    point.sp_geometry,
    tower.network_name,
    tower.category,
    tower.network_id,
    tower.status,
    tower.is_new_entity,
    tower.source_ref_id,
    tower.source_ref_type,
    tower.ownership_type,
    COALESCE(tower.third_party_vendor_id, 0) AS third_party_vendor_id,
    tower.network_status,
    point.display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_tower tower ON point.system_id = tower.system_id AND point.entity_type::text = 'Tower'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry)
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Tower'::text;

ALTER TABLE public.vw_att_details_tower_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_tree_vector;

CREATE OR REPLACE VIEW public.vw_att_details_tree_vector
 AS
 SELECT tree.system_id,
    tree.region_id,
    tree.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    tree.tree_name,
    tree.network_id,
    tree.status,
    tree.category,   
    tree.is_new_entity,
    tree.source_ref_id,
    tree.source_ref_type,
    tree.network_status::text AS network_status,
    tree.tree_name AS label_column,
    point.display_name,
    point.entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN att_details_tree tree ON point.system_id = tree.system_id AND point.entity_type::text = 'Tree'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'TREE'::text;

ALTER TABLE public.vw_att_details_tree_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_trench_vector;

CREATE OR REPLACE VIEW public.vw_att_details_trench_vector
 AS
 SELECT trench.system_id,
    trench.network_id,
    trench.trench_name,
    COALESCE(pmapping.province_id, trench.province_id) AS province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(lm.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(pmapping.region_id, trench.region_id) AS region_id,
    trench.no_of_ducts::text || 'D'::text AS count_ducts,
    trench.category,
    trench.type,
    trench.network_status::text AS network_status,
    trench.status,
   
    trench.is_new_entity,
    trench.source_ref_id,
    trench.source_ref_type,
    lm.sp_geometry,
    trench.ownership_type,
    COALESCE(trench.third_party_vendor_id, 0) AS third_party_vendor_id,
        CASE
            WHEN COALESCE(trench.gis_design_id, ''::character varying)::text = ''::text THEN trench.network_id
            ELSE trench.gis_design_id
        END AS display_name,
    trench.network_id AS label_column,
    COALESCE(trench.category, ''::character varying) AS entity_category
   FROM line_master lm
     JOIN att_details_trench trench ON lm.system_id = trench.system_id AND lm.entity_type::text = 'Trench'::text
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(trench.network_id::text) ~~ 'NLD%'::text THEN trench.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('Trench'::text)
            ELSE 1 = 2
        END
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = lm.system_id AND aei.entity_type::text = 'Trench'::text;

ALTER TABLE public.vw_att_details_trench_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_wallmount_vector;

CREATE OR REPLACE VIEW public.vw_att_details_wallmount_vector
 AS
 SELECT wallmount.system_id,
    wallmount.region_id,
    wallmount.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    point.sp_geometry,
    wallmount.wallmount_name,
    wallmount.network_id,
    wallmount.network_status,
    wallmount.status,
    wallmount.is_new_entity,
    wallmount.source_ref_id,
    wallmount.source_ref_type,
    wallmount.ownership_type,
    COALESCE(wallmount.third_party_vendor_id, 0) AS third_party_vendor_id,
        CASE
            WHEN COALESCE(wallmount.gis_design_id, ''::character varying)::text = ''::text THEN wallmount.network_id
            ELSE wallmount.gis_design_id
        END::character varying AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_wallmount wallmount ON point.system_id = wallmount.system_id AND point.entity_type::text = 'WallMount'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'WallMount'::text;

ALTER TABLE public.vw_att_details_wallmount_vector
    OWNER TO postgres;


------------------------------------------------------------------------------


truncate table pod_geojson_master;
INSERT INTO pod_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','POD',
		   'layer_title','Central_Office',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_pod_vector
		) ROW;	
--------------------------------------------
truncate table rack_geojson_master;
INSERT INTO rack_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Rack',
		   'layer_title','Rack',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_rack_vector
		) ROW;	

--------------------------------------------	
truncate table fms_geojson_master;
INSERT INTO fms_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','FMS',
		   'layer_title','FDP',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_fms_vector
		) ROW;	

--------------------------------------------	
truncate table ont_geojson_master;
INSERT INTO ont_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','ONT',
		   'layer_title','CPE',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_ont_vector
		) ROW;	

-------------------------------------------

truncate table manhole_geojson_master;
INSERT INTO manhole_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Manhole',
		   'layer_title','Manhole',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_Manhole_vector
		) ROW;
		

-------------------------------------------
-------------------------------------------		
		
truncate table adb_geojson_master;
INSERT INTO adb_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','ADB',
		   'layer_title','PDP',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_adb_vector
		) ROW;
		

		
-------------------------------------------
truncate table splitter_geojson_master;
INSERT INTO splitter_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Splitter',
		   'layer_title','Splitter',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_splitter_vector
		) ROW;	
-------------------------------------------			
truncate table bdb_geojson_master;
INSERT INTO bdb_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','BDB',
		   'layer_title','FDC',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_bdb_vector 
		) ROW;
-------------------------------------------
truncate table building_geojson_master;
INSERT INTO building_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Building',
		   'layer_title','Building',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_building_vector 
		) ROW;
-------------------------------------------	
truncate table SpliceClosure_geojson_master;
INSERT INTO SpliceClosure_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','SpliceClosure',
			'layer_title','Splice Closure',			
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_SpliceClosure_vector
		) ROW;
	-------------------------------------------
truncate table cable_geojson_master;
INSERT INTO cable_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Cable',
		   'layer_title','Cable',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_cable_vector where sp_geometry is not null
		) ROW;		
-------------------------------------------
truncate table Trench_geojson_master;
INSERT INTO Trench_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Trench',
		   'layer_title','Trench',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_Trench_vector
		) ROW;	






-----------------------------------------------------------------------------------------------------

truncate table fdb_geojson_master;
INSERT INTO fdb_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','FDB',
		   'layer_title','FAT',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_fdb_vector
		) ROW;
-----------------------------------------------------------------------------------
truncate table pole_geojson_master;
INSERT INTO pole_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Pole',
		   'layer_title','Pole',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_pole_vector
		) ROW;
---------------------------------------------------------
truncate table wallmount_geojson_master;
INSERT INTO wallmount_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','WallMount',
		   'layer_title','WallMount',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_Wallmount_vector
		) ROW;
		
---------------------------------------------------------		
truncate table Handhole_geojson_master;
INSERT INTO Handhole_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Handhole',
		   'layer_title','Handhole',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_handhole_vector
		) ROW;	
		
---------------------------------------------------------			
truncate table Cabinet_geojson_master;
INSERT INTO Cabinet_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Cabinet',
		   'layer_title','Cabinet',
		    'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,		  
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_Cabinet_vector
		) ROW;	
		
---------------------------------------------------------			
truncate table PatchPanel_geojson_master;
INSERT INTO PatchPanel_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','PatchPanel',
		   'layer_title','Patch Panel',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_PatchPanel_vector
		) ROW;	
		
---------------------------------------------------------			
truncate table HTB_geojson_master;
INSERT INTO HTB_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','HTB',
		   'layer_title','ATB',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_HTB_vector
		) ROW;	
---------------------------------------------------------			
truncate table loop_geojson_master;
INSERT INTO loop_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Loop',
		   'layer_title','Loop',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_loop_vector
		) ROW;			
---------------------------------------------------------		
truncate table Antenna_geojson_master;
INSERT INTO Antenna_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Antenna',
		   'layer_title','Antenna',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_Antenna_vector
		) ROW;	
---------------------------------------------------------			
truncate table Duct_geojson_master;
INSERT INTO Duct_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Duct',
		   'layer_title','Duct',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_Duct_vector
		) ROW;	
---------------------------------------------------------			
truncate table equipment_geojson_master;
INSERT INTO equipment_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Equipment',
		   'layer_title','Equipment',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_equipment_vector
		) ROW;	
		
---------------------------------------------------------			
truncate table fault_geojson_master;
INSERT INTO fault_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Fault',
		   'layer_title','Fault',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_fault_vector
		) ROW;	
---------------------------------------------------------			
truncate table tower_geojson_master;
INSERT INTO tower_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Tower',
		   'layer_title','Tower',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_tower_vector
		) ROW;	


---------------------------------------------------------		
	
	truncate table microduct_geojson_master;
INSERT INTO microduct_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Microduct',
		   'layer_title','Microduct',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_microduct_vector
		) ROW;	
		

---------------------------------------------------------		
		
truncate table cdb_geojson_master;
INSERT INTO cdb_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','CDB',
		   'layer_title','CDB',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_cdb_vector
		) ROW;	


---------------------------------------------------------	
truncate table slack_geojson_master;
INSERT INTO slack_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Slack',
		   'layer_title','Slack',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_slack_vector
		) ROW;	

---------------------------------------------------------	
truncate table area_geojson_master;
INSERT INTO area_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Area',
		   'layer_title','Area',
			
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_area_vector
		) ROW;
-------------------------------------------
truncate table subarea_geojson_master;
INSERT INTO subarea_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','SubArea',
			'layer_title','SubArea',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_subarea_vector
		) ROW;

-------------------------------------------
truncate table dsa_geojson_master;
INSERT INTO dsa_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','DSA',
			'layer_title','DSA_Boundary',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_dsa_vector
		) ROW;

-------------------------------------------
truncate table CSA_geojson_master;
INSERT INTO csa_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','CSA',
			'layer_title','CSA_Boundary',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_csa_vector
		) ROW;	


		-------------------------------------------
truncate table network_ticket_geojson_master;
INSERT INTO network_ticket_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Network_Ticket',
			'layer_title','NW Ticket Outline',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_network_ticket_vector
		) ROW;	

select * from network_ticket_geojson_master

-------------------------------------------
truncate table customer_geojson_master;
INSERT INTO customer_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Customer',
			'layer_title','Customer',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_customer_vector
		) ROW;	
-------------------------------------------
truncate table sector_geojson_master;
INSERT INTO sector_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Sector',
			'layer_title','Sector',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_sector_vector
		) ROW;	


-------------------------------------------
truncate table tree_geojson_master;
INSERT INTO tree_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Tree',
			'layer_title','Tree',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_tree_vector
		) ROW;	
