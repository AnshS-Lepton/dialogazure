DROP VIEW public.vw_att_details_adb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_adb_vector
 AS
 SELECT adb.system_id,
    adb.region_id,
    adb.province_id,
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
    plgn.display_name
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
    COALESCE(point.entity_category, ''::character varying) AS entity_category
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




DROP VIEW public.vw_att_details_cable_vector;

CREATE OR REPLACE VIEW public.vw_att_details_cable_vector
 AS
 SELECT cable.system_id,
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
    COALESCE(cable.cable_category, ''::character varying) AS entity_category
   FROM line_master lm
     JOIN att_details_cable cable ON lm.system_id = cable.system_id AND lm.entity_type::text = 'Cable'::text
     LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = lm.system_id AND aei.entity_type::text = 'Cable'::text;

ALTER TABLE public.vw_att_details_cable_vector
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
    COALESCE(plgn.gis_design_id, ''::character varying) AS gis_design_id,
    dsa.province_id,
    dsa.network_status,
    dsa.network_id,
    plgn.display_name
   FROM polygon_master plgn
     JOIN att_details_dsa dsa ON plgn.system_id = dsa.system_id AND plgn.entity_type::text = 'DSA'::text;

ALTER TABLE public.vw_att_details_dsa_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_duct_vector;

CREATE OR REPLACE VIEW public.vw_att_details_duct_vector
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.duct_name,
    duct.pin_code,
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




DROP VIEW public.vw_att_details_equipment_vector;

CREATE OR REPLACE VIEW public.vw_att_details_equipment_vector
 AS
 SELECT equipment.system_id,
    equipment.region_id,
    equipment.province_id,
    COALESCE(aei.route_cable_id, ''::text) AS route_cable_id,
    COALESCE(point.gis_design_id, ''::character varying) AS gis_design_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    equipment.equipment_name,
    equipment.project_id,
    equipment.planning_id,
    equipment.purpose_id,
    equipment.workorder_id,
    equipment.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), equipment.network_status::text) AS network_status,
    ''::character varying AS ownership_type,
    ''::character varying AS source_ref_id,
    ''::character varying AS source_ref_type,
    equipment.network_id AS label_column,
        CASE
            WHEN COALESCE(equipment.gis_design_id, ''::character varying)::text = ''::text THEN equipment.network_id
            ELSE equipment.gis_design_id
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_model equipment ON point.system_id = equipment.system_id AND lower(point.entity_type::text) = 'equipment'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = equipment.system_id AND upper(adi1.entity_type::text) = upper('equipment'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = equipment.system_id AND upper(adi.entity_type::text) = upper('equipment'::text)
      LEFT JOIN ( SELECT associate_route_info.entity_id,
            associate_route_info.entity_type,
            string_agg(associate_route_info.cable_id::text, ','::text) AS route_cable_id
           FROM associate_route_info
          GROUP BY associate_route_info.entity_id, associate_route_info.entity_type) aei ON aei.entity_id = point.system_id AND aei.entity_type::text = 'Equipment'::text;


ALTER TABLE public.vw_att_details_equipment_vector
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
    antenna.network_id AS display_name
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


DROP VIEW public.vw_att_details_manhole_vector;

CREATE OR REPLACE VIEW public.vw_att_details_manhole_vector
 AS
 SELECT manhole.system_id,
    manhole.region_id,
    manhole.province_id,
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
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
        CASE
            WHEN microduct.no_of_cables = 0 THEN 'vacant'::text
            ELSE 'used'::text
        END AS utilization,
    microduct.ownership_type,
    COALESCE(microduct.third_party_vendor_id, 0) AS third_party_vendor_id,
    microduct.primary_pod_system_id,
    microduct.secondary_pod_system_id,
    microduct.source_ref_id,
    microduct.source_ref_type,
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
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), patchpanel.network_status::text) AS network_status,
    patchpanel.ownership_type,
    COALESCE(patchpanel.third_party_vendor_id, 0) AS third_party_vendor_id,
    patchpanel.primary_pod_system_id,
    patchpanel.secondary_pod_system_id,
    patchpanel.source_ref_id,
    patchpanel.source_ref_type,
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



DROP VIEW public.vw_att_details_pole_vector;

CREATE OR REPLACE VIEW public.vw_att_details_pole_vector
 AS
 SELECT pole.system_id,
    pole.region_id,
    pole.province_id,
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
    rack.status,
    rack.network_id,
    rack.network_status,
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
    clr.color_code
   FROM ( SELECT subarea_1.system_id,
            subarea_1.region_id,
            subarea_1.province_id,
            plgn.sp_geometry,
            plgn.display_name,
            subarea_1.subarea_name,
            subarea_1.network_id,
            subarea_1.parent_system_id,
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

	
CREATE OR REPLACE FUNCTION public.fn_get_route_info(
	p_province_id character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN

RETURN QUERY SELECT ROW_TO_JSON(ROW) FROM (
		SELECT distinct CBL.SYSTEM_ID AS CABLE_ID,CBL.NETWORK_ID AS ENTITY_NETWORK_ID,CBL.NETWORK_ID||' ('||coalesce(CBL.ROUTE_ID,'')||'-'||coalesce(CBL.ROUTE_NAME,'')||')' as Route_Id,false IS_ASSOCIATED
		FROM LINE_MASTER LM 
		INNER JOIN ATT_DETAILS_CABLE CBL on LM.ENTITY_TYPE='Cable' AND CBL.SYSTEM_ID=LM.SYSTEM_ID
		INNER JOIN ASSOCIATE_ROUTE_INFO ASS ON ASS.CABLE_ID=CBL.SYSTEM_ID
	) ROW;
END
$BODY$;

ALTER FUNCTION public.fn_get_route_info(character varying)
    OWNER TO postgres;
	
	
---------------------------------------

---------------------------------------------------------------------------------------------------------------
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




insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
select layer_id,'gis_design_id','gis design id',0,true,'gis_design_id' from layer_details where 
 is_visible_in_ne_library=true and layer_name not in ('Other','Province','Loop','SpliceTray','ProjectArea','Restricted_Area',
'Fault',
'LandBase','Clamp','PIT')


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
select layer_id,'route_cable_id','Route Id',0,true,'route_cable_id' from layer_details where 
 is_visible_in_ne_library=true and layer_name not in ('Other','Province','Loop','SpliceTray','ProjectArea','Restricted_Area',
'Fault',
'LandBase','Clamp','PIT')



INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('IsGeometryUpdateOnAssociationAllowed', '1', 'Line entity geometry update based on parent entity from association and location edit', 'Web', false, 'string', 1.0, 1.0, 1, now(), NULL, NULL, true, true, false, NULL, NULL);

---------------------------------------------------------------------------------------





CREATE OR REPLACE FUNCTION public.fn_save_entity_assocition(
	p_line_associate_info character varying,
	p_parent_system_id integer,
	p_parent_entity_type character varying,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
DECLARE 
 curgeommapping refcursor;  
 v_system_id integer;
 v_network_id character varying;
 v_entity_type character varying;
 v_parent_netwok_id character varying;
 v_is_associated bool;
 v_line_associate_info json;
 v_snapped_geom geometry;
 v_point_geom geometry;
 v_CableExtraLengthPercentage integer;
 v_cablemeasured_length double precision;
 v_total_loop_length integer;

 v_start_point geometry;
 v_end_point geometry;
 v_total_points integer;
 v_old_measured_length double precision;
 v_new_measured_length double precision;
 v_old_calculated_length double precision;

 v_old_line_geom geometry;
 v_parent_display_name character varying;
 v_parent_display_value character varying;
 v_layer_table character varying;
 v_display_value character varying;
 v_associated_system_id integer;
 v_associated_entity_type character varying;
 b_arow record;
 V_BROW record;
 v_is_geomtryedit_allowed integer;
BEGIN

create temp table tmp_association
(
system_id integer,
entity_type character varying,
entity_network_id character varying,
is_associated bool
) on commit drop;

select replace(p_line_associate_info,'\','') into v_line_associate_info;
EXECUTE 'SELECT network_id  from att_details_'||p_parent_entity_type||' where system_id='||p_parent_system_id||'' into v_parent_netwok_id;
OPEN curgeommapping FOR select system_id,entity_network_id,entity_type,is_associated  from json_populate_recordset(null::tmp_association,v_line_associate_info);
	LOOP
		FETCH  curgeommapping into v_system_id,v_network_id,v_entity_type,v_is_associated;
		-- EXIT FOR LOOP
		IF NOT FOUND THEN
		EXIT;
		END IF;

	if(v_is_associated=true)
	then
		raise info '%11',11;
-- 		raise info '%v_system_id',v_system_id;
-- 		raise info '%v_entity_type',v_entity_type;
-- 		raise info '%p_parent_system_id',p_parent_system_id;
-- 		raise info '%p_parent_entity_type',p_parent_entity_type;
-- 		
		if(not exists(select entity_system_id from associate_entity_info 
		where (associated_system_id=v_system_id and upper(associated_entity_type)=upper(v_entity_type) and entity_system_id=p_parent_system_id and 
		upper(entity_type)=upper(p_parent_entity_type))
		or (entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and associated_system_id=p_parent_system_id and 
		upper(associated_entity_type)=upper(p_parent_entity_type))))
		then
			raise info '%12',12;
			v_is_geomtryedit_allowed=(select (coalesce((select value::integer from global_settings where key='IsGeometryUpdateOnAssociationAllowed'),0))::integer);
			
			insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,
			associated_entity_type,associated_display_name,created_on,created_by) 
			values(p_parent_system_id,v_parent_netwok_id,p_parent_entity_type,fn_get_display_name(p_parent_system_id,p_parent_entity_type),v_system_id,
			v_network_id,v_entity_type,fn_get_display_name(v_system_id,v_entity_type),now(),p_user_id);

			

					
			if((upper(p_parent_entity_type)='CABLE' or lower(p_parent_entity_type)='duct' or lower(p_parent_entity_type)='trench'))
			then			
				raise info '%13',13;
				--select LM.sp_geometry into v_old_line_geom from line_master LM where WHERE LM.system_id = p_parent_system_id 
				--and lower(entity_type)=lower(p_parent_entity_type);

				if(upper(p_parent_entity_type)='TRENCH' and upper(v_entity_type)='DUCT' and v_is_geomtryedit_allowed=1)
					then
						update att_details_duct set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
						where system_id=v_system_id;
						update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Trench' and system_id=p_parent_system_id)
						where entity_type=v_entity_type and system_id=v_system_id;
						delete from associate_entity_info where entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and 
						UPPER(ASSOCIATED_ENTITY_TYPE) NOT IN ('CABLE','DUCT');
						perform(fn_geojson_update_entity_attribute(v_system_id,v_entity_type,0,1,true));

						FOR V_BROW IN SELECT * FROM ASSOCIATE_ENTITY_INFO WHERE ENTITY_SYSTEM_ID=v_system_id AND UPPER(ENTITY_TYPE)=UPPER(v_entity_type) and 
						UPPER(ASSOCIATED_ENTITY_TYPE)='CABLE'
						LOOP

							update att_details_cable set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
							where system_id=V_BROW.associated_system_id;
							update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Duct' and system_id=v_system_id)
							where entity_type=V_BROW.associated_entity_type and system_id=V_BROW.associated_system_id;
							delete from associate_entity_info where entity_system_id=V_BROW.associated_system_id and upper(entity_type)=
							upper(V_BROW.associated_entity_type) and UPPER(ASSOCIATED_ENTITY_TYPE) NOT IN ('CABLE','DUCT');
							perform(fn_geojson_update_entity_attribute(V_BROW.associated_system_id,V_BROW.associated_entity_type,0,1,true));
						END LOOP;
				end if;
				if(upper(p_parent_entity_type)='DUCT' and upper(v_entity_type)='CABLE' and v_is_geomtryedit_allowed=1)
					then
						update att_details_cable set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
						where system_id=v_system_id;
						update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Duct' and system_id=p_parent_system_id)
						where entity_type=v_entity_type and system_id=v_system_id;
						delete from associate_entity_info where entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and 
						UPPER(ASSOCIATED_ENTITY_TYPE) NOT IN ('CABLE','DUCT');
						perform(fn_geojson_update_entity_attribute(v_system_id,v_entity_type,0,1,true));
				end if;
				
				SELECT ST_RemoveRepeatedPoints(ST_AddPoint(
				ST_AddPoint(ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01),st_startpoint(LM.SP_GEOMETRY),0)
				,st_endpoint(LM.SP_GEOMETRY),ST_NPoints(ST_AddPoint(ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01),
				st_startpoint(LM.SP_GEOMETRY),0)))) into v_snapped_geom
				FROM (SELECT SP_GEOMETRY AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = v_system_id and lower(entity_type)=lower(v_entity_type)) AS point,
				LINE_MASTER AS LM  WHERE LM.system_id = p_parent_system_id and lower(entity_type)=lower(p_parent_entity_type);

				
				if(v_snapped_geom is not null) then
					update LINE_MASTER set SP_GEOMETRY=v_snapped_geom  WHERE system_id = p_parent_system_id and lower(entity_type)=lower(p_parent_entity_type);
									
				if(upper(p_parent_entity_type)='CABLE')
				then    select value  into v_CableExtraLengthPercentage from global_settings where key='CableExtraLengthPercentage';
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER 
					WHERE system_id = p_parent_system_id and lower(entity_type)=lower(p_parent_entity_type))) into v_cablemeasured_length;
					select total_loop_length into v_total_loop_length from att_details_cable where system_id=p_parent_system_id;

					
					 
					update att_details_cable set cable_measured_length=v_cablemeasured_length, 
					cable_calculated_length=v_cablemeasured_length+v_total_loop_length+(((v_CableExtraLengthPercentage*v_cablemeasured_length)/100)-
					v_total_loop_length) where system_id=p_parent_system_id;
					
					
				end if; 

				if(upper(p_parent_entity_type)='DUCT')
				then
					select calculated_length,manual_length into v_old_measured_length,v_old_calculated_length from att_details_duct WHERE  
					system_id=p_parent_system_id; 
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER 
					WHERE  system_id=p_parent_system_id and upper(entity_type)=upper(p_parent_entity_type))) into v_new_measured_length;
					
					if(v_new_measured_length!=v_old_measured_length)
					then
					update att_details_duct set calculated_length=v_new_measured_length,
					manual_length= 
					case when coalesce(v_old_calculated_length,0)>0 and coalesce(v_new_measured_length,0)>coalesce(v_old_measured_length,0) 
					then (v_old_calculated_length+(v_new_measured_length-v_old_measured_length))
					when coalesce(v_old_calculated_length,0)>0 and coalesce(v_new_measured_length,0)<coalesce(v_old_measured_length,0) 
					then (v_old_calculated_length-(v_old_measured_length-v_new_measured_length)) end
					WHERE  system_id=p_parent_system_id;
					end if;
				end if;

				if(upper(p_parent_entity_type)='TRENCH')
				then
				raise info '%19',19;
					select trench_length into v_old_measured_length from att_details_trench WHERE  system_id=p_parent_system_id; 
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE  system_id=p_parent_system_id 
					and upper(entity_type)=upper(p_parent_entity_type))) into v_new_measured_length;

					if(v_new_measured_length!=v_old_measured_length)
					then
						update att_details_trench set 
						trench_length= 
						case 
						when coalesce(v_old_measured_length,0)>0 and coalesce(v_new_measured_length,0)>coalesce(v_old_measured_length,0) 
						then (v_old_measured_length+(v_new_measured_length-v_old_measured_length))
						when coalesce(v_old_measured_length,0)>0 and coalesce(v_new_measured_length,0)<coalesce(v_old_measured_length,0) 
						then (v_old_measured_length-(v_old_measured_length-v_new_measured_length)) end
						WHERE  system_id=p_parent_system_id;
					end if;
					
				end if;
				end if;
							
			end if;

			end if;

-- select 1 from vw_associate_entity_master 
-- where ((upper(layer_name)=upper(p_parent_entity_type) and upper(associate_layer_name)=upper(v_entity_type) and upper(associated_layer_geom_type)='LINE')
-- or (upper(associate_layer_name)=upper(p_parent_entity_type) and upper(layer_name)=upper(v_entity_type)) and upper(layer_geom_type)='LINE')						
-- and is_snapping_enabled=true
--IF(upper(p_parent_entity_type)='POLE')

			
			if exists(select 1 from vw_associate_entity_master 
			where ((upper(layer_name)=upper(p_parent_entity_type) and upper(associate_layer_name)=upper(v_entity_type) and upper(associated_layer_geom_type)='LINE')
			or (upper(associate_layer_name)=upper(p_parent_entity_type) and upper(layer_name)=upper(v_entity_type)) and upper(layer_geom_type)='LINE')						
			and is_snapping_enabled=true)
			then
				raise info '%14',14;
				SELECT ST_RemoveRepeatedPoints(ST_AddPoint(
				ST_AddPoint(ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01),st_startpoint(LM.SP_GEOMETRY),0)
				,st_endpoint(LM.SP_GEOMETRY),ST_NPoints(ST_AddPoint(ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01),
				st_startpoint(LM.SP_GEOMETRY),0)))) into v_snapped_geom
				FROM (SELECT SP_GEOMETRY AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = p_parent_system_id 
				and lower(entity_type)=lower(p_parent_entity_type)) AS point,
				LINE_MASTER AS LM  WHERE LM.system_id = v_system_id and lower(entity_type)=lower(v_entity_type);

				raise info '%v_snapped_geom',v_snapped_geom;
				
				if(v_snapped_geom is not null) then
					update LINE_MASTER set SP_GEOMETRY=v_snapped_geom  WHERE system_id = v_system_id and lower(entity_type)=lower(v_entity_type);
				--BY ANTRA--	
				raise info '%v_system_id',v_system_id;
				raise info '%v_entity_type',v_entity_type;
				raise info '%p_parent_entity_type',p_parent_entity_type;
				
			for b_arow in	select associated_system_id,associated_entity_type  from associate_entity_info 
				where (entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and is_termination_point=false 
				and lower(entity_type) !=lower(p_parent_entity_type))		
				union 
				select entity_system_id,entity_type from associate_entity_info where (associated_system_id=v_system_id 
				and upper(associated_entity_type)=upper(v_entity_type) and is_termination_point=false and lower(entity_type) 
				!=lower(p_parent_entity_type))
				
				LOOP

				if(v_snapped_geom is not null) then
					update LINE_MASTER set SP_GEOMETRY=v_snapped_geom  WHERE system_id = b_arow.associated_system_id
					and lower(entity_type)=lower( b_arow.associated_entity_type);
				end if;
				END LOOP;
				--END					
				if(upper(p_parent_entity_type)='CABLE')
				then    select value  into v_CableExtraLengthPercentage from global_settings where key='CableExtraLengthPercentage';
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER 
					WHERE system_id = v_system_id and lower(entity_type)=lower(v_entity_type))) into v_cablemeasured_length;
					select total_loop_length into v_total_loop_length from att_details_cable where system_id=v_system_id;					
					 
					update att_details_cable set cable_measured_length=v_cablemeasured_length, 
					cable_calculated_length=v_cablemeasured_length+v_total_loop_length+(((v_CableExtraLengthPercentage*v_cablemeasured_length)/100)-
					v_total_loop_length) where system_id=v_system_id;				
					
				end if; 

				if(upper(p_parent_entity_type)='DUCT')
				then
					select calculated_length,manual_length into v_old_measured_length,v_old_calculated_length from att_details_duct WHERE  system_id=v_system_id; 
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER 
					WHERE  system_id=v_system_id and upper(entity_type)=upper(v_entity_type))) into v_new_measured_length;
					
					if(v_new_measured_length!=v_old_measured_length)
					then
					update att_details_duct set calculated_length=v_new_measured_length,
					manual_length= 
					case when coalesce(v_old_calculated_length,0)>0 and coalesce(v_new_measured_length,0)>coalesce(v_old_measured_length,0) 
					then (v_old_calculated_length+(v_new_measured_length-v_old_measured_length))
					when coalesce(v_old_calculated_length,0)>0 and coalesce(v_new_measured_length,0)<coalesce(v_old_measured_length,0) 
					then (v_old_calculated_length-(v_old_measured_length-v_new_measured_length)) end
					WHERE  system_id=v_system_id;
					end if;
				end if;

				if(upper(p_parent_entity_type)='TRENCH')
				then
					select trench_length into v_old_measured_length from att_details_trench WHERE  system_id=v_system_id; 
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE  system_id=v_system_id 
					and upper(entity_type)=upper(v_entity_type))) into v_new_measured_length;

					if(v_new_measured_length!=v_old_measured_length)
					then
						update att_details_trench set 
						trench_length= 
						case 
						when coalesce(v_old_measured_length,0)>0 and coalesce(v_new_measured_length,0)>coalesce(v_old_measured_length,0) 
						then (v_old_measured_length+(v_new_measured_length-v_old_measured_length))
						when coalesce(v_old_measured_length,0)>0 and coalesce(v_new_measured_length,0)<coalesce(v_old_measured_length,0) 
						then (v_old_measured_length-(v_old_measured_length-v_new_measured_length)) end
						WHERE  system_id=v_system_id;
					end if;
				end if;
				end if;
			
		end if;

	else
		if(exists(
		select entity_system_id from associate_entity_info 
		where (associated_system_id=v_system_id and upper(associated_entity_type)=upper(v_entity_type)and entity_system_id=p_parent_system_id and 
		upper(entity_type)=upper(p_parent_entity_type)) 
		or (associated_system_id=p_parent_system_id  and upper(associated_entity_type)=upper(p_parent_entity_type) and entity_system_id=v_system_id and 
		upper(entity_type)=upper(v_entity_type))
		))
		then
		
			delete from associate_entity_info 
			where (associated_system_id=v_system_id and  upper(associated_entity_type)=upper(v_entity_type) and entity_system_id=p_parent_system_id and 
			upper(entity_type)=upper(p_parent_entity_type))	or (associated_system_id=p_parent_system_id  and  upper(associated_entity_type)=
			upper(p_parent_entity_type)  and entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) );
		end if;
	end if;
END LOOP;
close curgeommapping;
	--if(upper(p_parent_entity_type)='CABLE')
	--then
		--select value  into v_CableExtraLengthPercentage from global_settings where key='CableExtraLengthPercentage';
		--select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE system_id = p_parent_system_id and lower(entity_type)=lower('cable'))) into v_cablemeasured_length;
		--select total_loop_length into v_total_loop_length from att_details_cable where system_id=p_parent_system_id; 
		--update att_details_cable set cable_measured_length=v_cablemeasured_length, cable_calculated_length=v_cablemeasured_length+v_total_loop_length+(((v_CableExtraLengthPercentage*v_cablemeasured_length)/100)-v_total_loop_length) where system_id=p_parent_system_id; 
	--end if;
return query
select row_to_json(row) from (
select true as status, 'Success' as message 
 ) row;
 
END
$BODY$;

ALTER FUNCTION public.fn_save_entity_assocition(character varying, integer, character varying, integer)
    OWNER TO postgres;

----------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_get_layer_actions(
	p_layer_name character varying,
	p_isosp_type boolean,
	p_network_status character varying,
	p_role_id integer,
	p_user_id integer,
	p_ismobileaction boolean,
	p_system_id integer,
	p_source_ref_id character varying,
	p_source_ref_type character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE STRICT PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

Declare v_layerid integer;
v_network_status_filter character varying;
v_layer_view character varying;
v_entity_status character varying;
v_updated_by integer;
v_draft_network_status character varying;
v_is_new_entity boolean;
v_buffer_id integer;
v_source_ref_id character varying;
v_source_ref_type character varying;
V_MANAGER_ID integer;
V_SUBAREA_ID integer:=0;
V_NE_CLASS_NAME character varying;
V_LAYER_TABLE character varying;
BEGIN

-- Select *From fn_get_layer_actions('Pole',true,'P',78,5,false,590102,'','');

-- public.temp_layer_actions definition

 

-- Drop table

 

-- DROP TABLE temp_layer_actions;

CREATE temp TABLE temp_layer_actions (

                id  integer,

                layer_id integer,

                action_name character varying,

                action_title character varying,

                is_enabled boolean default false,

                delete_authority boolean default false,

                is_template_filled boolean default false,

                edit boolean default false,

                is_visible boolean default false,

                parent_action_id integer,

                is_buffer_needed boolean default false,

                action_abbr character varying,

                action_layer_id integer,

is_active boolean default true,

          action_sequence integer,

          is_web_action boolean default false

) on commit drop;

 

 

v_is_new_entity:=false;

                select layer_id,layer_view,ne_class_name,layer_table into v_layerid,v_layer_view,v_ne_class_name,v_layer_table from layer_details where ((upper(layer_name)=upper(p_layer_name)) or (upper(layer_title)=upper(p_layer_name))) limit 1;

                execute 'select status,is_new_entity,created_by from '||v_layer_view||' where system_id='||p_system_id into v_entity_status,v_is_new_entity,v_updated_by;

 

                raise info'v_is_new_entity%',v_is_new_entity;

                raise info'v_entity_status%',v_entity_status;

                raise info'v_updated_by%',v_updated_by;

 

        select id into v_buffer_id from layer_action_mapping where layer_id = v_layerid and upper(action_name) = 'BUFFEROPERATIONS';

        SELECT MANAGER_ID INTO V_MANAGER_ID FROM USER_MASTER WHERE USER_ID=v_updated_by;

        raise info'V_MANAGER_ID%',V_MANAGER_ID;

 

        SELECT MANAGER_ID INTO V_MANAGER_ID FROM USER_MASTER WHERE USER_ID=P_USER_ID;

               

         if((v_entity_status='D'  OR v_entity_status='S') and v_is_new_entity=false)
         then
                select attribute_info->>'curr_status' ,updated_by,source_ref_id,source_ref_type 
                into v_draft_network_status,v_updated_by,v_source_ref_id,v_source_ref_type
                from ATT_DETAILS_EDIT_ENTITY_INFO q
                --inner join entity_operations_master opr on q.entity_action_id=opr.id and upper(opr.description)='NS' and
                where q.entity_system_id=p_system_id  and upper(q.entity_type)=upper(p_layer_name);     
                               
             --   if(coalesce(v_updated_by,0)=p_user_id and coalesce(v_draft_network_status,'')!='')then p_network_status=v_draft_network_status; end if;
         end if;                         

        select case when p_network_status='D' then 'ND,NA' else 'N'||p_network_status end into v_network_status_filter;

 

 

 

        insert into temp_layer_actions (is_web_action,action_sequence,id,layer_id,action_name,action_title,is_enabled,delete_authority,is_template_filled,edit,is_visible,parent_action_id,is_buffer_needed,action_abbr,action_layer_id)

        select is_web_action,action_sequence,id,layer_id,action_name,

                                (case

                                --when v_entity_status='D' and coalesce(v_updated_by,0)>0 and coalesce(v_updated_by,0)!=p_user_id

                                --then 'Entity is in draft mode'

                                when coalesce(res_field_key,'')='' then action_title else res_field_key  end) as action_title

                               

                               

 

                                , coalesce(is_enabled,true) as is_enabled,coalesce(delete_authority,false) as delete_authority ,is_template_filled,coalesce(edit,true) as edit,is_visible,parent_action_id,is_buffer_needed,action_abbr,action_layer_id from (

                                select action_sequence,la.id,la.layer_id,la.action_name,la.action_title,res_field_key,la.action_abbr,la.action_layer_id, rp.edit,

                                case when v_entity_status IN ('D','S')  and ((coalesce(v_updated_by,0)>0 and coalesce(v_updated_by,0)!=p_user_id)

                                or (v_is_new_entity and coalesce(v_updated_by,0)!=p_user_id) or

                                ( coalesce(v_updated_by,0)=p_user_id and v_source_ref_id!=p_source_ref_id and upper(v_source_ref_type)!=Upper(p_source_ref_type)))

                                then

                                               

                                                case when la.action_abbr='A' then (coalesce(rp.add,false) and la.is_enable_in_draft)

                                                when la.action_abbr='T' then (coalesce(rp.add,false) and la.is_enable_in_draft)

                                                when la.action_abbr='E' then (rp.edit and la.is_enable_in_draft)

                                                when la.action_abbr='D' then (rp.delete and la.is_enable_in_draft)

                                                WHEN(upper(la.action_abbr)='SPLT' AND p_network_status='D') THEN false

                                                WHEN(upper(la.action_abbr)='NWTSTS' AND ld.is_network_entity AND v_entity_status IN ('A','V','D','R')) THEN false

                                                when la.action_abbr='M' then (coalesce(umm.id,0)>0 and la.is_enable_in_draft)

                                                when la.action_abbr='GA'

                                                then case when (select count(id) from layer_action_mapping lam where lam.parent_action_id = la.id and is_active = true and is_visible = true) >0 then true else false end

                                                when la.action_abbr in ('NP','NA','ND')

                                               

                                                then case when ld.is_networktype_required  then

                                                (select nsttaus.add from role_permission_entity nsttaus where upper(nsttaus.network_status)=replace(la.action_abbr,'N','') and

                                                nsttaus.role_id=p_role_id and nsttaus.layer_id=la.layer_id) and la.is_enable_in_draft

                                                else false end   

                                                when upper(la.action_name)='BARCODE' then ld.is_barcode_enabled and la.is_enable_in_draft

                                                else la.is_enable_in_draft

                                                end

                                               

                                else

                               

                                                case when la.action_abbr='A' then coalesce(rp.add,false)

                                                when la.action_abbr='T' then coalesce(rp.add,false)

                                                when la.action_abbr='E' then rp.edit

                                                when (la.action_abbr='D') then rp.delete

                                                --case when (v_updated_by=p_user_id or p_user_id=V_MANAGER_ID) then true else false end

                                                when la.action_abbr='M' then coalesce(umm.id,0)>0

                                                WHEN(upper(la.action_abbr)='SPLT' AND p_network_status='D') THEN false

                                                WHEN(upper(la.action_abbr)='NWTSTS' AND ld.is_network_entity AND v_entity_status IN ('A','V','D','R')) THEN false

                                                when la.action_abbr='GA'

                                                then case when (select count(id) from layer_action_mapping lam where lam.parent_action_id = la.id and is_active = true and is_visible = true) >0 then true else false end

                                                when la.action_abbr in ('NP','NA','ND')

                                               

                                                then case when ld.is_networktype_required --and coalesce(v_is_new_entity,false)=false

                                                then

                                                (select nsttaus.add from role_permission_entity nsttaus where upper(nsttaus.network_status)=replace(la.action_abbr,'N','')

                                                and nsttaus.role_id=p_role_id and nsttaus.layer_id=la.layer_id)

                                                else false end   

                                               

                                                when upper(la.action_name)='BARCODE' then ld.is_barcode_enabled

                                                else true end

                                               

                                end       

                                as is_enabled,

                                true as delete_authority,

                                coalesce(layer.is_template_required,false) as is_template_required,

                                case when la.action_abbr = 'T' then public.fn_chk_layer_template_exist(la.action_layer_id,p_user_id) else false end as  is_template_filled,

                                la.is_visible,la.parent_action_id,

                                (case when parent_action_id <>0 and parent_action_id = v_buffer_id then true else  false end) as is_buffer_needed,

                                la.is_web_action

                                from vw_layer_action_mapping la  

                                left join role_permission_entity rp on upper(rp.network_status)= upper((case when coalesce(p_network_status,'')!='' then p_network_status else 'P' end))

                                and rp.role_id= p_role_id

                                and rp.layer_id= case when la.action_layer_id>0 and coalesce(la.action_abbr, '') != '' then la.action_layer_id else la.layer_id end

                                left join user_module_mapping umm on umm.module_id = case when p_isMobileAction = true then la.action_mobile_module_id else la.action_module_id end

                                and umm.user_id= p_user_id

                                left join layer_details layer on layer.layer_id = la.action_layer_id and la.action_abbr='T'

                                left join layer_details ld on ld.layer_id = la.layer_id

                                where la.layer_id = v_layerid and (case when upper(p_layer_name)=upper('landbase') then  upper(la.layer_name)=upper('landbase') else la.is_layer_visible=true end)

                                and case when p_isosp_type then la.is_osp_action = true and la.is_web_action=true

                                when p_isosp_type=false and p_isMobileAction=false then  la.is_isp_action = true and la.is_web_action=true

                                when p_isMobileAction=true then la.is_mobile_action=true

                                end

                               

                                and la.is_active=true

                                order by la.action_sequence

                                ) a where a.is_template_required=true or coalesce(a.action_abbr, '') != 'T' and (coalesce(a.action_abbr, '')

                                not in(select regexp_split_to_table(v_network_status_filter, ',')));

 

if(p_ismobileaction=false)
then
	update temp_layer_actions set  action_title='Permission is denied!' 
	where  is_enabled=false and is_web_action=true;
end if;

if(upper(p_layer_name)='CABLE' and exists(select 1 from associate_entity_info where associated_system_id=p_system_id and upper(associated_entity_type)=upper(p_layer_name) 
	and upper(entity_type) IN ('DUCT','TRENCH')))
then
	update temp_layer_actions set is_enabled=false,
	action_title='Location can be edited from the parent entity!'
	where  action_name= 'LocationEdit';
end if;
if(upper(p_layer_name)='DUCT' and exists(select 1 from associate_entity_info where associated_system_id=p_system_id and upper(associated_entity_type)=upper(p_layer_name) 
	and upper(entity_type)='TRENCH'))
then
	update temp_layer_actions set is_enabled=false,
	action_title='Location can be edited from the parent entity!'
	where  action_name= 'LocationEdit';
end if;
if(upper(p_layer_name)='SUBAREA'
and exists(select 1 from att_details_subarea where is_association_completed=true and system_id=p_system_id))
and exists(select 1 from bulk_assocation_request_logs where subarea_system_id=p_system_id and user_id=p_user_id)
then
	update temp_layer_actions set is_enabled=true,
	action_title='Download the association logs!'
	where  action_name= 'Downloadlog';
elsif upper(p_layer_name)='SUBAREA' 
and exists(select 1 from att_details_subarea where is_association_completed=false and system_id=p_system_id)
then

	update temp_layer_actions set is_enabled=false,
	action_title='Logs will be available to downlod after successful bulk assocation!'
	where  action_name= 'Downloadlog';

elsif not exists(select 1 from bulk_assocation_request_logs where subarea_system_id=p_system_id and user_id=p_user_id)
then
	update temp_layer_actions set is_enabled=false,
	action_title='Logs will be available to downlod after successful bulk assocation!'
	where  action_name= 'Downloadlog';
end if;

if(upper(p_layer_name)='SUBAREA' and exists(select 1 from att_details_subarea where is_association_completed=false and system_id=p_system_id))
then
		update temp_layer_actions
		set is_enabled=false,
		action_title='Please run the association tool first!'
		where action_name='XMLDASHBOARD';
end if;

if exists(select 1 from temp_layer_actions where action_name='Delete' and delete_authority=false)
then
		update temp_layer_actions
		set is_enabled=false,
		action_title='Delete!'
		where action_name='Delete';
end if;

if((coalesce(p_source_ref_id,'') != '') and exists(select ttm.ticket_type from att_details_networktickets adn
inner join ticket_type_master ttm on ttm.id=adn.ticket_type_id 
where adn.ticket_id = p_source_ref_id::integer and ttm.ticket_type = 'Survey'))
then
  update temp_layer_actions set is_active = false where action_name = 'ConvertToAsBuilt';
end If;

                  

                   if(upper(V_NE_CLASS_NAME)='STRUCTURE' or upper(p_layer_name)='FDB')
                   then
                         execute 'select subarea_system_id from '||v_layer_table||' where system_id='||p_system_id into V_SUBAREA_ID;
                         if(coalesce(V_SUBAREA_ID,0)=0)
                         then
                             execute 'select pm.system_id from point_master lm
                                      inner join polygon_master pm on st_within(lm.sp_geometry,pm.sp_geometry)
                                      and pm.entity_type=''SubArea'' and lm.system_id= '||p_system_id||' and upper(lm.entity_type)=upper('''||p_layer_name||''')' into V_SUBAREA_ID;
                          end if;
                          if(exists(Select 1 from layer_permission_boundary lpb
                                    inner join layer_details ld on ld.layer_name=lpb.layer_name and ld.layer_name='FDB'
                                    where lpb.add=false and lpb.is_locked=true and lpb.boundary_name='SubArea' and lpb.boundary_id=V_SUBAREA_ID ))
                         then
                                  update temp_layer_actions
                                  set is_enabled=false,
                                  action_title='FAT addition has been restricted due to design BOM is submitted!'
                                  where action_name='AddFDB';

                         end if;  

                         if(exists(Select 1 from layer_permission_boundary lpb
                                  inner join layer_details ld on ld.layer_name=lpb.layer_name and ld.layer_name='Splitter'
                                  where lpb.add=false and lpb.is_locked=true and lpb.boundary_name='SubArea' and lpb.boundary_id=V_SUBAREA_ID ))

                         then

                                  update temp_layer_actions
                                  set is_enabled=false,
                                  action_title='Splitter addition has been restricted due to design BOM is submitted!'
                                  where action_name='SecondaryAddSplitter';

                         end if;                

                   end if;

			IF((Select value From global_settings where key='IsTraceEnabled')= '1')
			THEN
				update temp_layer_actions set action_title= 'Design Validation' ,action_name='DesignValidation' where action_name='DesignBOMBOQ';
			END IF;

              

              

                RETURN QUERY

                SELECT ROW_TO_JSON(ROW) FROM(select * from temp_layer_actions where is_active=true order by action_sequence) ROW;

END

$BODY$;

ALTER FUNCTION public.fn_get_layer_actions(character varying, boolean, character varying, integer, integer, boolean, integer, character varying, character varying)
    OWNER TO postgres;
----------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_update_line_associated_child_geometry(
	p_system_id integer,
	p_entity_type character varying,
	p_user_id integer)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 DECLARE  
 v_entity_type character varying;
 v_system_id integer;
 V_AROW record;
 V_BROW record;
 v_is_geomtryedit_allowed integer;
BEGIN

v_is_geomtryedit_allowed=(select (coalesce((select value::integer from global_settings where key='IsGeometryUpdateOnAssociationAllowed'),0))::integer);
	FOR V_AROW IN SELECT * FROM ASSOCIATE_ENTITY_INFO WHERE ENTITY_SYSTEM_ID=p_system_id AND UPPER(ENTITY_TYPE)=UPPER(p_entity_type) and UPPER(ASSOCIATED_ENTITY_TYPE)
	IN ('DUCT','CABLE')
	LOOP 
		raise info '%13 ',12;
		v_entity_type=V_AROW.associated_entity_type;
		v_system_id=V_AROW.associated_system_id;
		raise info '%v_entity_type ',v_entity_type;
		raise info '%v_system_id ',v_system_id;
		if(upper(p_entity_type)='TRENCH' and UPPER(v_entity_type)='DUCT' and v_is_geomtryedit_allowed=1)
		then
			update att_details_duct set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
			where system_id=v_system_id;
			update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Trench' and system_id=p_system_id)
			where entity_type=v_entity_type and system_id=v_system_id;
			delete from associate_entity_info where entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and 
			UPPER(ASSOCIATED_ENTITY_TYPE) NOT IN ('CABLE','DUCT'); 
			perform(fn_geojson_update_entity_attribute(v_system_id,v_entity_type,0,1,true));


			FOR V_BROW IN SELECT * FROM ASSOCIATE_ENTITY_INFO WHERE ENTITY_SYSTEM_ID=v_system_id AND UPPER(ENTITY_TYPE)=UPPER(v_entity_type) and 
			UPPER(ASSOCIATED_ENTITY_TYPE)='CABLE'
			LOOP
				raise info '%V_BROW.associated_entity_type ',V_BROW.associated_entity_type;
				raise info '%V_BROW.associated_system_id ',V_BROW.associated_system_id;
				update att_details_cable set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
				where system_id=V_BROW.associated_system_id;
				update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Duct' and system_id=v_system_id)
				where entity_type=V_BROW.associated_entity_type and system_id=V_BROW.associated_system_id;
				delete from associate_entity_info where entity_system_id=V_BROW.associated_system_id and upper(entity_type)=upper(V_BROW.associated_entity_type) and 
				UPPER(ASSOCIATED_ENTITY_TYPE)!='CABLE';
				perform(fn_geojson_update_entity_attribute(V_BROW.associated_system_id,V_BROW.associated_entity_type,0,1,true));
			END LOOP;
			
		end if;
		if(upper(p_entity_type)='DUCT' and upper(v_entity_type)='CABLE' and v_is_geomtryedit_allowed=1)
		then
			update att_details_cable set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
			where system_id=v_system_id;
			update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Duct' and system_id=p_system_id)
			where entity_type=v_entity_type and system_id=v_system_id;
			delete from associate_entity_info where entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type);
			perform(fn_geojson_update_entity_attribute(v_system_id,v_entity_type,0,1,true));
		end if;

	END LOOP;
RETURN QUERY
	SELECT false AS STATUS, 'Successfully' ::CHARACTER VARYING AS MESSAGE; 			
END ;
$BODY$;

ALTER FUNCTION public.fn_update_tp_association(integer, character varying, integer)
    OWNER TO postgres;
-------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_update_entity_geom(
	p_system_id integer,
	p_geom_type character varying,
	p_entity_type character varying,
	p_userid integer,
	p_longlat character varying,
	p_network_status character varying,
	p_center_line_geom character varying)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE geo_data text;
s_longLat character varying;
sql text;
en_sql text;
s_long text;
s_lat text;
s_tbl_name text; 
 v_old_measured_length double precision;
 v_new_measured_length double precision;
 v_old_calculated_length double precision;
 v_new_calculated_length double precision;
 v_layer_table character varying;
 arow record;
v_parent_entity_type character varying;
v_parent_system_id integer;
v_parent_geom_type character varying;
v_parent_layer_table character varying;
v_arow_line record;
v_radious double precision;
v_center_line_geom geometry;
s_centroid_long text;
s_centroid_lat text;
v_geom_query geometry;
v_old_lat text;
v_old_long text;
v_geom character varying;
V_IS_ASSOCIATED boolean;

BEGIN
	create temp table tmp_associations
	(
	system_id integer,
	entity_type character varying
	) on commit drop;

	V_IS_ASSOCIATED:=false;
	if lower(p_geom_type)='point' then

	   s_long    := SPLIT_PART(p_longlat, ' ',1);
	   s_lat     := SPLIT_PART(p_longlat, ' ',2);

	   v_old_lat:=s_lat;
	   v_old_long:=s_long;

   		select layer_table into s_tbl_name from layer_details where upper(layer_name)=upper(p_entity_type);

		--s_tbl_name := 'att_details_'||p_entity_type;

		if lower(p_entity_type) = 'structure' 
		then
			s_tbl_name := 'att_details_bld_structure';
		end if;
	
		if(lower(p_entity_type)='structure')
		then
			for arow in select map.entity_id,map.entity_type from isp_entity_mapping map inner join 
			vw_termination_point_master tp on upper(map.entity_type)=upper(tp.tp_layer_name) where map.structure_id=p_system_id and upper(layer_name)=upper('Cable') and is_osp_tp=true
			loop
				select layer_table into s_tbl_name from layer_details where upper(layer_name)=upper(arow.entity_type);
				EXECUTE  'update '||s_tbl_name||' set longitude='||s_long||',latitude='||s_lat||'modified_on=now(),modified_by='||p_userid||' where system_id='||p_system_id||' ';
				perform(fn_snap_end_point(arow.entity_id,arow.entity_type,p_longlat));
				update point_master set longitude=s_long::double precision,latitude=s_lat::double precision,
				sp_geometry=ST_GeomFromText('POINT('||p_longlat||')',4326),modified_by=p_userid,modified_on=now()
				where system_id=arow.entity_id and lower(entity_type)=lower(arow.entity_type); 
				perform(fn_geojson_update_entity_attribute(arow.entity_id,arow.entity_type,0,1,true));
			end loop;
		
			update point_master set longitude=s_long::double precision,latitude=s_lat::double precision,sp_geometry=ST_GeomFromText('POINT('||p_longlat||')',4326),modified_by=p_userid,modified_on=now()
			where system_id=p_system_id and lower(entity_type)=lower(p_entity_type); 
			update att_details_bld_structure set longitude=s_long::double precision,latitude=s_lat::double precision,modified_on=now(),modified_by=p_userid where system_id=p_system_id;
			update att_details_site_info set longitude=s_long::double precision,latitude=s_lat::double precision,modified_on=now(),modified_by=p_userid where parent_system_id=p_system_id;
			perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,1,true));
		
		else

			select sp_geometry into v_geom from point_master where system_id=p_system_id and entity_type=p_entity_type ;
			raise info '%v_geom',v_geom;
			raise info '%p_entity_type',p_entity_type;
			raise info '%p_system_id',p_system_id;
			raise info '%p_longlat',p_longlat;
			EXECUTE  'update '||s_tbl_name||' set longitude='||s_long||',latitude='||s_lat||',modified_on=now(),modified_by='||p_userid||' where system_id='||p_system_id||' ';
			perform(fn_snap_end_point(p_system_id,p_entity_type,p_longlat));
			--UPDATE GEOMETRY IN POINT MASTER
			update point_master set longitude=s_long::double precision,latitude=s_lat::double precision,sp_geometry=ST_GeomFromText('POINT('||p_longlat||')',4326),modified_by=p_userid,modified_on=now()
			where system_id=p_system_id and lower(entity_type)=lower(p_entity_type); 
			perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,1,true));

			execute 'select parent_system_id,parent_entity_type from '||s_tbl_name||' where system_id='||p_system_id into v_parent_system_id, v_parent_entity_type;
			select layer_table,geom_type into v_parent_layer_table,v_parent_geom_type from layer_details where upper(layer_name)=upper(v_parent_entity_type);
		
	
			if(upper(v_parent_geom_type)='POINT' and v_parent_layer_table is not null)
			and Exists(SELECT 1 FROM information_schema.columns WHERE upper(table_name) = upper(v_parent_layer_table) AND Upper(column_name) = Upper('longitude'))
			and Exists(SELECT 1 FROM information_schema.columns WHERE upper(table_name) = upper(v_parent_layer_table) AND Upper(column_name) = Upper('latitude'))
			then
				EXECUTE  'update '||v_parent_layer_table||' set longitude='||s_long||',latitude='||s_lat||',modified_on=now(),modified_by='||p_userid||' where system_id='||v_parent_system_id||' ';
				raise info'%test','1';
				perform(fn_snap_end_point(v_parent_system_id,v_parent_entity_type,p_longlat));
				--UPDATE GEOMETRY IN POINT MASTER
				update point_master set longitude=s_long::double precision,latitude=s_lat::double precision,sp_geometry=ST_GeomFromText('POINT('||p_longlat||')',4326),modified_by=p_userid,modified_on=now()
				where system_id=v_parent_system_id and lower(entity_type)=lower(v_parent_entity_type);
				perform(fn_geojson_update_entity_attribute(v_parent_system_id,v_parent_entity_type,0,1,true)); 
			end if;

			--REGION LOCATION UPDATION FOR Associated Entity Condition By ANTRA---                               

			select case when count(1)>0 then  true else false end into V_IS_ASSOCIATED
			from associate_entity_info where ((entity_system_id=p_system_id and upper(entity_type)=upper(p_entity_type)) or (associated_system_id=p_system_id
			and upper(associated_entity_type)=upper(p_entity_type))) and is_termination_point=false;
			--raise info '%V_IS_ASSOCIATED',V_IS_ASSOCIATED;
			IF (V_IS_ASSOCIATED) THEN
				perform(fn_snap_associated_entities(p_system_id,p_entity_type,p_longlat,v_geom,false,p_userid));	
			END IF;	
		END IF;
	        
        RETURN QUERY
         
           select true as status, '[SI_GBL_GBL_GBL_GBL_016 1]'::character varying as message;--location updated successfully
   
	end if;

			if lower(p_geom_type)='line' then
			Raise info '-----------p_geom_type%',p_geom_type;
			update line_master set sp_geometry=ST_GeomFromText('LINESTRING('||p_longlat||')',4326),modified_by=p_userid,modified_on=now() where system_id=p_system_id 
			and lower(entity_type)=lower(p_entity_type);
			select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE  system_id=p_system_id and 
			upper(entity_type)=upper(p_entity_type))) into v_new_measured_length;
			perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,1,true));
			if(upper(p_entity_type)='CABLE')
			then
				select cable_measured_length,cable_calculated_length into v_old_measured_length,v_old_calculated_length from att_details_cable 
				WHERE  system_id=p_system_id; 
				
				update att_details_cable set cable_measured_length=v_new_measured_length, 
				cable_calculated_length= case when v_old_calculated_length>0 and v_new_measured_length>v_old_measured_length 
				then (v_old_calculated_length+(v_new_measured_length-v_old_measured_length))
				when v_old_calculated_length>0 and v_new_measured_length<v_old_measured_length 
				then (v_old_calculated_length-(v_old_measured_length-v_new_measured_length)) 
				else cable_calculated_length end,modified_on=now(),modified_by=p_userid
				WHERE  system_id=p_system_id;

			perform(fn_cable_set_end_point(p_system_id));
				
			end if;

			if(upper(p_entity_type)='DUCT')
			then
				select calculated_length,manual_length into v_old_measured_length,v_old_calculated_length from att_details_duct WHERE  system_id=p_system_id; 				

				update att_details_duct set calculated_length=v_new_measured_length,
				manual_length= case when v_old_calculated_length>0 and v_new_measured_length>v_old_measured_length then (v_old_calculated_length+(v_new_measured_length-v_old_measured_length))
				when v_old_calculated_length>0 and v_new_measured_length<v_old_measured_length 
				then (v_old_calculated_length-(v_old_measured_length-v_new_measured_length)) 
				else manual_length end,modified_on=now(),modified_by=p_userid
				WHERE  system_id=p_system_id;

				perform(fn_duct_set_end_point(p_system_id));
			end if;

			if(upper(p_entity_type)='TRENCH')
			then
				select trench_length into v_old_measured_length from att_details_trench WHERE  system_id=p_system_id; 				

				update att_details_trench set 
				trench_length= case when v_old_measured_length>0 and v_new_measured_length>v_old_measured_length then (v_old_measured_length+(v_new_measured_length-v_old_measured_length))
				when v_old_measured_length>0 and v_new_measured_length<v_old_measured_length 
				then (v_old_measured_length-(v_old_measured_length-v_new_measured_length)) 
				else trench_length end,modified_on=now(),modified_by=p_userid
				WHERE  system_id=p_system_id;

				perform(fn_trench_set_end_point(p_system_id));
			end if;	
			perform(fn_update_line_associated_child_geometry(p_system_id,p_entity_type,p_userid));			
		--END IF;	
	RETURN QUERY
          select true as status, '[SI_GBL_GBL_GBL_GBL_016 2]'::character varying as message;--location updated successfully
end if;

if (lower(p_geom_type)='polygon')
then

	if(upper(p_entity_type)=upper('Structure')) and not exists( select 1 from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type))
	then

		insert into polygon_master(system_id,entity_type,approval_flag,sp_geometry,created_by,common_name,display_name)
		select system_id,p_entity_type,'A',ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),p_userid,network_id,fn_get_display_name(p_system_id,p_entity_type) from att_details_bld_structure where system_id=p_system_id;
		
	elsIf(upper(p_entity_type)=upper('row'))
	then
		update polygon_master set sp_geometry=ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),center_line_geom=ST_GeomFromText('LINESTRING('||p_center_line_geom||')',4326),modified_by=p_userid,modified_on=now()
		where system_id=p_system_id and lower(entity_type)=lower(p_entity_type);	
		perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,1,true));
	else
		update polygon_master set sp_geometry=ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),modified_by=p_userid,modified_on=now() where system_id=p_system_id and lower(entity_type)=lower(p_entity_type);		
		perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,1,true));
	end if;
	
	
	
	if(lower(p_entity_type)='building') then
		-- Update Building Lat/longs
		update att_details_building set longitude= ROUND(ST_X(ST_CENTROID(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)))::numeric,6),
		latitude= ROUND(ST_Y(ST_CENTROID(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)))::numeric,6),modified_on=now(),modified_by=p_userid where system_id=p_system_id;
	elsif(upper(p_entity_type)=upper('Structure'))
	then
	        s_centroid_long:=ST_X(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)));
		s_centroid_lat:=ST_y(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)));

	
		update point_master  set sp_geometry=ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)),longitude=ST_X(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)))
		,latitude=ST_Y(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))),modified_by=p_userid,modified_on=now() where system_id=p_system_id and lower(entity_type)=lower(p_entity_type);

		update att_details_bld_structure set longitude=ST_X(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)))
		,latitude=ST_Y(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))),modified_on=now(),modified_by=p_userid where system_id=p_system_id;

		for arow in select map.entity_id,map.entity_type from isp_entity_mapping map inner join 
		vw_termination_point_master tp on upper(map.entity_type)=upper(tp.tp_layer_name) where map.structure_id=p_system_id and upper(layer_name)=upper('Cable') and is_osp_tp=true
		loop
			select layer_table into s_tbl_name from layer_details where upper(layer_name)=upper(arow.entity_type);
			EXECUTE  'update '||s_tbl_name||' set longitude='||s_centroid_long||',latitude='||s_centroid_lat||'modified_on=now(),modified_by='||p_userid||' where system_id='||p_system_id||' ';
			perform(fn_snap_end_point(arow.entity_id,arow.entity_type,s_centroid_long||' '||s_centroid_lat));
			update point_master set longitude=s_centroid_long::double precision,latitude=s_centroid_lat::double precision,sp_geometry=ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)),modified_by=p_userid,modified_on=now()
			where system_id=arow.entity_id and lower(entity_type)=lower(arow.entity_type); 
		end loop;

		perform(fn_update_child_entity_geom(p_system_id,'Polygon','Structure',p_userid,
		ST_X(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)))||' '||ST_Y(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)))));
		
		perform(fn_update_child_entity_geom(p_system_id,'Polygon','Structure',p_userid,
		ST_X(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)))||' '||ST_Y(ST_Centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326)))));

		perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,1,true));
	end if;
	
	       
	RETURN QUERY
          select true as status, '[SI_GBL_GBL_GBL_GBL_016 3]'::character varying as message;--location updated successfully
           
end if; 
if lower(p_geom_type)='circle' then
	select Cast(st_distance(ST_centroid('POLYGON(('||p_longlat||'))'),(ST_DumpPoints('POLYGON(('||p_longlat||'))')).geom,false)as decimal(10,2)) into v_radious  limit 1;

	select center_line_geom into v_center_line_geom
	from polygon_master pm inner join  att_details_row_pit pit on pm.system_id=pit.parent_system_id and upper(pm.entity_type)=upper('ROW') where pit.system_id=p_system_id;

	--update circle_master set sp_geometry=ST_GeomFromText('POLYGON(('||p_longlat||'))',4326),sp_center=ST_centroid('POLYGON(('||p_longlat||'))'),radious=coalesce(v_radious,0) where system_id=p_system_id and lower(entity_type)=lower(p_entity_type);
	update circle_master set sp_geometry=ST_buffer_meters(ST_ClosestPoint(v_center_line_geom,
	ST_centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))),
	coalesce(v_radious,0)),sp_center=ST_ClosestPoint(v_center_line_geom,ST_centroid(ST_GeomFromText('POLYGON(('||p_longlat||'))',4326))),radious=coalesce(v_radious,0) where system_id=p_system_id and lower(entity_type)=lower(p_entity_type);		
RETURN QUERY
          select true as status, '[SI_GBL_GBL_GBL_GBL_016 4]'::character varying as message;--location updated successfully
end if; 
RETURN QUERY
         select false as status, '[SI_GBL_GBL_GBL_GBL_064]'::character varying as message;--location not updated

END
$BODY$;

ALTER FUNCTION public.fn_update_entity_geom(integer, character varying, character varying, integer, character varying, character varying, character varying)
    OWNER TO postgres;
-------------------------------------------------------------------------------	