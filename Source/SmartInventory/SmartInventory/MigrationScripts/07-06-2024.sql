DROP VIEW public.vw_att_details_networkticket_map;

CREATE OR REPLACE VIEW public.vw_att_details_networkticket_map
 AS
 SELECT nwtckt.ticket_id AS system_id,
    nwtckt.region_id,
    nwtckt.province_id,
    nwtckt.status,
   case when nwtckt.ticket_status_id=8 then false::boolean else true::boolean end  is_new_entity,
    COALESCE(adi.updated_geom, plgn.sp_geometry) AS sp_geometry,
    nwtckt.name,
    nwtckt.network_id,
    nwtckt.ticket_id::character varying AS source_ref_id,
    'Network_Ticket'::character varying AS source_ref_type,
    nwtckt.name AS label_column
   FROM polygon_master plgn
     JOIN att_details_networktickets nwtckt ON plgn.system_id = nwtckt.ticket_id AND upper(plgn.entity_type::text) = 'NETWORK_TICKET'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = nwtckt.ticket_id AND upper(adi.entity_type::text) = upper('NETWORK_TICKET'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_networkticket_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_adb_map;

CREATE OR REPLACE VIEW public.vw_att_details_adb_map
 AS
 SELECT adb.system_id,
    adb.region_id,
    adb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    adb.adb_name,
    adb.project_id,
    adb.planning_id,
    adb.workorder_id,
    adb.purpose_id,
    adb.specification,
    adb.network_id,
    COALESCE(adb.entity_category, 'Primary'::character varying) AS entity_category,
    adb.status,
    adb.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), adb.network_status::text) AS network_status,
    adb.ownership_type,
    adb.third_party_vendor_id,
    adb.primary_pod_system_id,
    adb.secondary_pod_system_id,
    adb.source_ref_id,
    adb.source_ref_type,
    lim.icon_path,
    round(adb.latitude::numeric, 2) AS label_column
   FROM point_master point
     JOIN att_details_adb adb ON point.system_id = adb.system_id AND point.entity_type::text = 'ADB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = adb.system_id AND adi1.entity_type::text = 'ADB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), adb.network_status::text) AND COALESCE(adb.entity_category, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND lim.layer_name::text = upper('ADB'::text)
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = adb.system_id AND adi.entity_type::text = 'ADB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_adb_map
    OWNER TO postgres;


DROP VIEW public.vw_att_details_antenna_map;

CREATE OR REPLACE VIEW public.vw_att_details_antenna_map
 AS
 SELECT antenna.system_id,
    antenna.region_id,
    antenna.province_id,
    antenna.network_id,
    antenna.is_new_entity,
    antenna.network_name,
    antenna.project_id,
    antenna.planning_id,
    antenna.purpose_id,
    antenna.workorder_id,
    antenna.latitude,
    antenna.longitude,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), antenna.network_status::text) AS network_status,
    antenna.ownership_type,
    antenna.third_party_vendor_id,
    antenna.status,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    antenna.source_ref_id,
    antenna.source_ref_type,
    lim.icon_path,
    antenna.network_name AS label_column
   FROM point_master point
     JOIN att_details_antenna antenna ON point.system_id = antenna.system_id AND upper(point.entity_type::text) = 'ANTENNA'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = antenna.system_id AND upper(adi1.entity_type::text) = upper('ANTENNA'::text) AND adi1.description::text = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), antenna.network_status::text) AND lim.layer_name::text = upper('ANTENNA'::text)
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = antenna.system_id AND upper(adi.entity_type::text) = upper('ANTENNA'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_antenna_map
    OWNER TO postgres;


DROP VIEW public.vw_att_details_area_map;

CREATE OR REPLACE VIEW public.vw_att_details_area_map
 AS
 SELECT area.system_id,
    area.region_id,
    area.province_id,
    COALESCE(adi.updated_geom, plgn.sp_geometry) AS sp_geometry,
    area.area_name,
    area.network_id,
    area.status,
    area.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), area.network_status::text) AS network_status,
    area.source_ref_id,
    area.source_ref_type,
    (('     '::text || COALESCE(area.gis_design_id, ''::character varying)::text) || '
     HP:-'::text) || area.no_of_home_pass::text AS label_column
   FROM polygon_master plgn
     JOIN att_details_area area ON plgn.system_id = area.system_id AND plgn.entity_type::text = 'Area'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = area.system_id AND adi.entity_type::text = 'Area'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = area.system_id AND adi1.entity_type::text = 'Area'::text AND adi1.description::text = upper('NS'::text);

ALTER TABLE public.vw_att_details_area_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_bdb_map;

CREATE OR REPLACE VIEW public.vw_att_details_bdb_map
 AS
 SELECT bdb.system_id,
    bdb.region_id,
    bdb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    bdb.bdb_name,
    bdb.project_id,
    bdb.planning_id,
    bdb.purpose_id,
    bdb.workorder_id,
    bdb.network_id,
    COALESCE(bdb.entity_category, 'Primary'::character varying) AS entity_category,
    bdb.status,
    bdb.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), bdb.network_status::text) AS network_status,
    bdb.ownership_type,
    bdb.third_party_vendor_id,
    bdb.primary_pod_system_id,
    bdb.secondary_pod_system_id,
    bdb.source_ref_id,
    bdb.source_ref_type,
    lim.icon_path,
    bdb.address AS label_column
   FROM point_master point
     JOIN att_details_bdb bdb ON point.system_id = bdb.system_id AND point.entity_type::text = 'BDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = bdb.system_id AND adi1.entity_type::text = 'BDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), bdb.network_status::text) AND COALESCE(bdb.entity_category, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND lim.layer_name::text = 'BDB'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = bdb.system_id AND adi.entity_type::text = 'BDB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_bdb_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_bld_structure_map;

CREATE OR REPLACE VIEW public.vw_att_details_bld_structure_map
 AS
 SELECT structure.system_id,
    structure.building_id,
    structure.network_id,
    structure.structure_name,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), structure.network_status::text) AS network_status,
    structure.status,
    structure.region_id,
    structure.province_id,
    structure.business_pass,
    structure.home_pass,
    structure.no_of_floor,
    structure.no_of_flat,
    structure.no_of_shaft,
    COALESCE(adi.updated_geom, pm.sp_geometry) AS sp_geometry,
    structure.source_ref_id,
    structure.source_ref_type,
    lim.icon_path,
    structure.gis_design_id AS label_column
   FROM point_master pm
     JOIN att_details_bld_structure structure ON pm.system_id = structure.system_id AND pm.entity_type::text = 'Structure'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = structure.system_id AND upper(adi1.entity_type::text) = upper('STRUCTURE'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), structure.network_status::text) AND upper(lim.layer_name::text) = upper('STRUCTURE'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = structure.system_id AND upper(adi.entity_type::text) = upper('STRUCTURE'::text);

ALTER TABLE public.vw_att_details_bld_structure_map
    OWNER TO postgres;

GRANT ALL ON TABLE public.vw_att_details_bld_structure_map TO postgres;
GRANT INSERT, SELECT ON TABLE public.vw_att_details_bld_structure_map TO safaricom;


DROP VIEW public.vw_att_details_building_map;

CREATE OR REPLACE VIEW public.vw_att_details_building_map
 AS
 SELECT building.system_id,
    building.surveyarea_id,
    building.building_name,
    building.building_no,
    building.address,
    building.gis_address,
    building.region_id,
    building.province_id,
    building.status,
    building.is_new_entity,
    building.building_status,
    COALESCE(adi1.updated_geom, plgn.sp_geometry) AS sp_geometry,
    plgn.sp_centroid,
    plgn.geom_type,
    building.rfs_status,
    building.business_pass,
    building.home_pass,
    building.category,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), building.network_status::text) AS network_status,
    building.source_ref_id,
    building.source_ref_type,
        CASE
            WHEN building.rfs_status::text = 'Non-RFS'::text OR building.rfs_status::text = 'N-RFS'::text THEN
            CASE
                WHEN building.category::text = 'Residential'::text THEN
                CASE
                    WHEN building.home_pass >= 10 THEN 'icons/N-RFSResidentialHighRise.png'::text
                    ELSE 'icons/N-RFSResidentialLowRise.png'::text
                END
                WHEN building.category::text = 'Commercial'::text THEN
                CASE
                    WHEN building.business_pass >= 10 THEN 'icons/N-RFSCommercialHighRise.png'::text
                    ELSE 'icons/N-RFSCommercialLowRise.png'::text
                END
                WHEN building.category::text = 'Both'::text THEN 'icons/N-RFSCommercialResidential.png'::text
                ELSE 'icons/building_green.png'::text
            END
            WHEN building.rfs_status::text = 'Non-RFE'::text OR building.rfs_status::text = 'N-RFE'::text THEN
            CASE
                WHEN building.category::text = 'Residential'::text THEN
                CASE
                    WHEN building.home_pass >= 10 THEN 'icons/N-RFEResidentialHighRise.png'::text
                    ELSE 'icons/N-RFEResidentialLowRise.png'::text
                END
                WHEN building.category::text = 'Commercial'::text THEN
                CASE
                    WHEN building.business_pass >= 10 THEN 'icons/N-RFECommercialHighRise.png'::text
                    ELSE 'icons/N-RFECommercialLowRise.png'::text
                END
                WHEN building.category::text = 'Both'::text THEN 'icons/N-RFECommercialResidential.png'::text
                ELSE 'icons/building_green.png'::text
            END
            WHEN COALESCE(building.rfs_status, ''::character varying)::text <> 'Non-RFS'::text AND COALESCE(building.rfs_status, ''::character varying)::text <> 'N-RFS'::text THEN
            CASE
                WHEN building.category::text = 'Residential'::text THEN
                CASE
                    WHEN building.home_pass >= 10 THEN 'icons/RFSResidentialHighRise.png'::text
                    ELSE 'icons/RFSResidentialLowRise.png'::text
                END
                WHEN building.category::text = 'Commercial'::text THEN
                CASE
                    WHEN building.business_pass >= 10 THEN 'icons/RFSCommercialHighRise.png'::text
                    ELSE 'icons/RFSCommercialLowRise.png'::text
                END
                WHEN building.category::text = 'Both'::text THEN 'icons/RFSCommercialResidential.png'::text
                ELSE 'icons/building_green.png'::text
            END
            WHEN building.building_status::text = 'Surveyed'::text THEN 'icons/building_yellow.png'::text
            WHEN building.building_status::text = 'Resurveyed'::text THEN 'icons/building_red.png'::text
            WHEN building.building_status::text = 'Approved'::text THEN 'icons/building_green.png'::text
            WHEN building.building_status::text = 'New'::text THEN 'icons/building_new.png'::text
            ELSE 'icons/building_green.png'::text
        END AS image_icon,
    building.is_virtual,
    lim.icon_path,
    building.building_name AS label_column
   FROM ( SELECT point_master.system_id,
            point_master.sp_geometry,
            NULL::geometry AS sp_centroid,
            'Point'::text AS geom_type
           FROM point_master
          WHERE point_master.entity_type::text = 'Building'::text
        UNION ALL
         SELECT polygon_master.system_id,
            polygon_master.sp_geometry,
            st_centroid(polygon_master.sp_geometry) AS sp_centroid,
            'Polygon'::text AS text
           FROM polygon_master
          WHERE polygon_master.entity_type::text = 'Building'::text) plgn
     JOIN att_details_building building ON plgn.system_id = building.system_id
     LEFT JOIN vw_layer_icon_map lim ON COALESCE(building.building_status, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND lim.layer_name::text = 'Building'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = building.system_id AND adi1.entity_type::text = 'Building'::text AND adi1.description::text = 'NS'::text
  WHERE building.is_virtual = false;

ALTER TABLE public.vw_att_details_building_map
    OWNER TO postgres;

DROP VIEW public.vw_att_details_cabinet_map;

CREATE OR REPLACE VIEW public.vw_att_details_cabinet_map
 AS
 SELECT cabinet.system_id,
    cabinet.region_id,
    cabinet.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    cabinet.cabinet_name,
    cabinet.project_id,
    cabinet.planning_id,
    cabinet.purpose_id,
    cabinet.workorder_id,
    cabinet.network_id,
    cabinet.status,
    cabinet.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), cabinet.network_status::text) AS network_status,
    cabinet.ownership_type,
    cabinet.third_party_vendor_id,
    cabinet.primary_pod_system_id,
    cabinet.secondary_pod_system_id,
    cabinet.source_ref_id,
    cabinet.source_ref_type,
    lim.icon_path,
    cabinet.network_id AS label_column
   FROM point_master point
     JOIN att_details_cabinet cabinet ON point.system_id = cabinet.system_id AND point.entity_type::text = 'Cabinet'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = cabinet.system_id AND adi1.entity_type::text = 'Cabinet'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), cabinet.network_status::text) AND lim.layer_name::text = 'Cabinet'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = cabinet.system_id AND adi.entity_type::text = 'Cabinet'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_cabinet_map
    OWNER TO postgres;


DROP VIEW public.vw_att_details_cable_map;

CREATE OR REPLACE VIEW public.vw_att_details_cable_map
 AS
 SELECT cable.system_id,
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
    cable.gis_design_id AS label_column
   FROM line_master lm
     JOIN att_details_cable cable ON lm.system_id = cable.system_id AND lm.entity_type::text = 'Cable'::text
     LEFT JOIN cable_color_settings s ON cable.cable_type::text = s.cable_type::text AND cable.cable_category::text = s.cable_category::text AND cable.total_core = s.fiber_count
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = cable.system_id AND adi.entity_type::text = 'Cable'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = cable.system_id AND adi1.entity_type::text = 'Cable'::text AND adi1.description::text = 'NS'::text;

ALTER TABLE public.vw_att_details_cable_map
    OWNER TO postgres;


 DROP VIEW public.vw_att_details_cdb_map;

CREATE OR REPLACE VIEW public.vw_att_details_cdb_map
 AS
 SELECT cdb.system_id,
    cdb.region_id,
    cdb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    cdb.cdb_name,
    cdb.project_id,
    cdb.planning_id,
    cdb.purpose_id,
    cdb.workorder_id,
    cdb.network_id,
    cdb.status,
    cdb.is_new_entity,
    COALESCE(cdb.entity_category, 'Primary'::character varying) AS entity_category,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), cdb.network_status::text) AS network_status,
    cdb.ownership_type,
    cdb.third_party_vendor_id,
    cdb.primary_pod_system_id,
    cdb.secondary_pod_system_id,
    cdb.source_ref_id,
    cdb.source_ref_type,
    lim.icon_path,
    cdb.network_id AS label_column
   FROM point_master point
     JOIN att_details_cdb cdb ON point.system_id = cdb.system_id AND point.entity_type::text = 'CDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = cdb.system_id AND upper(adi1.entity_type::text) = upper('CDB'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), cdb.network_status::text) AND COALESCE(cdb.entity_category, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND upper(lim.layer_name::text) = upper('CDB'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = cdb.system_id AND upper(adi.entity_type::text) = upper('CDB'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_cdb_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_competitor_map;

CREATE OR REPLACE VIEW public.vw_att_details_competitor_map
 AS
 SELECT comp.system_id,
    comp.region_id,
    comp.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    comp.name,
    comp.network_id,
    comp.status,
    comp.is_new_entity,
    'icons/Competitor/'::text || comp.icon_path::text AS icon_path,
    comp.icon_color_code,
    comp.source_ref_id,
    comp.source_ref_type,
    comp.network_id AS label_column
   FROM point_master point
     JOIN att_details_competitor comp ON point.system_id = comp.system_id AND point.entity_type::text = 'Competitor'::text
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = comp.system_id AND upper(adi.entity_type::text) = upper('Competitor'::text);

ALTER TABLE public.vw_att_details_competitor_map
    OWNER TO postgres;

DROP VIEW public.vw_att_details_conduit_map;

CREATE OR REPLACE VIEW public.vw_att_details_conduit_map
 AS
 SELECT conduit.system_id,
    conduit.network_id,
    conduit.network_name,
    conduit.is_new_entity,
    conduit.pin_code,
    conduit.province_id,
    conduit.region_id,
    conduit.a_location,
    conduit.b_location,
    conduit.calculated_length,
    conduit.manual_length,
    conduit.construction,
    conduit.activation,
    conduit.accessibility,
    conduit.specification,
    conduit.category,
    conduit.vendor_id,
    conduit.type,
    conduit.brand,
    conduit.model,
    conduit.created_by,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), conduit.network_status::text) AS network_status,
    conduit.project_id,
    conduit.planning_id,
    conduit.workorder_id,
    conduit.purpose_id,
    conduit.status,
    conduit.conduit_type,
    conduit.color_code,
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
        CASE
            WHEN conduit.no_of_cables = 0 THEN 'vacant'::text
            ELSE 'used'::text
        END AS utilization,
    conduit.ownership_type,
    conduit.third_party_vendor_id,
    conduit.source_ref_id,
    conduit.source_ref_type,
    0 AS primary_pod_system_id,
    0 AS secondary_pod_system_id,
    conduit.network_id AS label_column
   FROM line_master lm
     JOIN att_details_conduit conduit ON lm.system_id = conduit.system_id AND lm.entity_type::text = 'Conduit'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = conduit.system_id AND adi.entity_type::text = 'Conduit'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = conduit.system_id AND adi1.entity_type::text = 'Conduit'::text AND adi1.description::text = 'NS'::text;

ALTER TABLE public.vw_att_details_conduit_map
    OWNER TO postgres;


DROP VIEW public.vw_att_details_coupler_map;

CREATE OR REPLACE VIEW public.vw_att_details_coupler_map
 AS
 SELECT coupler.system_id,
    coupler.region_id,
    coupler.province_id,
    coupler.status,
    coupler.is_new_entity,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    coupler.coupler_name,
    coupler.project_id,
    coupler.planning_id,
    coupler.workorder_id,
    coupler.purpose_id,
    coupler.network_id,
    coupler.is_virtual,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), coupler.network_status::text) AS network_status,
    coupler.ownership_type,
    coupler.third_party_vendor_id,
    coupler.primary_pod_system_id,
    coupler.secondary_pod_system_id,
    coupler.source_ref_id,
    coupler.source_ref_type,
    lim.icon_path,
    coupler.is_used AS label_column
   FROM point_master point
     JOIN att_details_coupler coupler ON point.system_id = coupler.system_id AND upper(point.entity_type::text) = 'COUPLER'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = coupler.system_id AND upper(adi1.entity_type::text) = upper('COUPLER'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), coupler.network_status::text) AND upper(lim.layer_name::text) = upper('COUPLER'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = coupler.system_id AND upper(adi.entity_type::text) = upper('COUPLER'::text);

ALTER TABLE public.vw_att_details_coupler_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_csa_map;

CREATE OR REPLACE VIEW public.vw_att_details_csa_map
 AS
 SELECT csa.system_id,
    csa.csa_name,
    csa.network_id,
    plgn.sp_geometry,
    csa.region_id,
    csa.province_id,
    csa.status,
    csa.is_new_entity,
    csa.network_status,
    csa.source_ref_id,
    csa.source_ref_type,
    csa.rfs_status,
        CASE
            WHEN csa.rfs_status::text = 'S2_RFS'::text THEN '#c4f3c4'::text
            ELSE '#FFFFCC'::text
        END AS color_code,
    ((' ,     '::text || csa.gis_design_id::text) || '
     HP:-'::text) || csa.no_of_home_pass::text AS label_column
   FROM polygon_master plgn
     JOIN att_details_csa csa ON plgn.system_id = csa.system_id AND plgn.entity_type::text = 'CSA'::text;

ALTER TABLE public.vw_att_details_csa_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_customer_map;

CREATE OR REPLACE VIEW public.vw_att_details_customer_map
 AS
 SELECT customer.system_id,
    customer.region_id,
    customer.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    customer.customer_name,
    customer.status,
    customer.is_new_entity,
    customer.network_id,
    customer.customer_type,
    customer.primary_pod_system_id,
    customer.secondary_pod_system_id,
    customer.source_ref_id,
    customer.source_ref_type,
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

ALTER TABLE public.vw_att_details_customer_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_dsa_map;

CREATE OR REPLACE VIEW public.vw_att_details_dsa_map
 AS
 SELECT dsa.system_id,
    dsa.dsa_name,
    dsa.network_id,
    dsa.is_new_entity,
    plgn.sp_geometry,
    dsa.region_id,
    dsa.province_id,
    dsa.status,
    dsa.network_status,
    dsa.is_visible_on_map,
    ((' ,     '::text || COALESCE(dsa.gis_design_id, ''::character varying)::text) || '
     HP:-'::text) || dsa.no_of_home_pass::text AS label_column
   FROM polygon_master plgn
     JOIN att_details_dsa dsa ON plgn.system_id = dsa.system_id AND plgn.entity_type::text = 'DSA'::text;

ALTER TABLE public.vw_att_details_dsa_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_duct_map;

CREATE OR REPLACE VIEW public.vw_att_details_duct_map
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.is_new_entity,
    duct.duct_name,
    duct.pin_code,
    duct.province_id,
    duct.region_id,
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
    duct.third_party_vendor_id,
    duct.primary_pod_system_id,
    duct.secondary_pod_system_id,
    duct.source_ref_id,
    duct.source_ref_type,
    duct.network_id AS label_column,
    lsm.color_code_hex
   FROM line_master lm
     JOIN att_details_duct duct ON lm.system_id = duct.system_id AND lm.entity_type::text = 'Duct'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = duct.system_id AND adi.entity_type::text = 'Duct'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = duct.system_id AND adi1.entity_type::text = 'Duct'::text AND adi1.description::text = 'NS'::text
     JOIN layer_style_master lsm ON lsm.entity_category::text = duct.duct_color::text AND lsm.layer_id = 17;

ALTER TABLE public.vw_att_details_duct_map
    OWNER TO postgres;


DROP VIEW public.vw_att_details_equipment_map;

CREATE OR REPLACE VIEW public.vw_att_details_equipment_map
 AS
 SELECT equipment.system_id,
    equipment.region_id,
    equipment.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    equipment.equipment_name,
    equipment.status,
    equipment.is_new_entity,
    equipment.project_id,
    equipment.planning_id,
    equipment.purpose_id,
    equipment.workorder_id,
    equipment.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), equipment.network_status::text) AS network_status,
    ''::character varying AS ownership_type,
    0 AS third_party_vendor_id,
    ''::character varying AS source_ref_id,
    ''::character varying AS source_ref_type,
    lim.icon_path,
    equipment.network_id AS label_column
   FROM point_master point
     JOIN att_details_model equipment ON point.system_id = equipment.system_id AND lower(point.entity_type::text) = 'equipment'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = equipment.system_id AND upper(adi1.entity_type::text) = upper('equipment'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), equipment.network_status::text) AND upper(lim.layer_name::text) = upper('equipment'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = equipment.system_id AND upper(adi.entity_type::text) = upper('equipment'::text);

ALTER TABLE public.vw_att_details_equipment_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_fault_map;

CREATE OR REPLACE VIEW public.vw_att_details_fault_map
 AS
 SELECT fault.system_id,
    fault.region_id,
    fault.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    fault.fault_id,
    fault.network_id,
    fault.status,
    fault.is_new_entity,
    fault.fault_type,
    fault.fault_entity_type,
    fault.fault_entity_system_id,
    fault.fault_entity_network_id,
    fault.remarks,
    fault.fault_status,
    fault.primary_pod_system_id,
    fault.secondary_pod_system_id,
    fault.source_ref_id,
    fault.source_ref_type,
    lim.icon_path,
    fault.fault_status AS label_column
   FROM point_master point
     JOIN att_details_fault fault ON point.system_id = fault.system_id AND upper(point.entity_type::text) = 'FAULT'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = fault.system_id AND upper(adi1.entity_type::text) = upper('FAULT'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), fault.network_status::text) AND COALESCE(fault.fault_status, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND upper(lim.layer_name::text) = upper('FAULT'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = fault.system_id AND upper(adi.entity_type::text) = upper('FAULT'::text);

ALTER TABLE public.vw_att_details_fault_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_fdb_map;

CREATE OR REPLACE VIEW public.vw_att_details_fdb_map
 AS
 SELECT fdb.system_id,
    fdb.region_id,
    fdb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    fdb.fdb_name,
    fdb.project_id,
    fdb.planning_id,
    fdb.purpose_id,
    fdb.workorder_id,
    fdb.network_id,
    fdb.status,
    fdb.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), fdb.network_status::text) AS network_status,
    fdb.ownership_type,
    fdb.third_party_vendor_id,
    fdb.primary_pod_system_id,
    fdb.secondary_pod_system_id,
    fdb.source_ref_id,
    fdb.source_ref_type,
    lim.icon_path,
    fdb.gis_design_id AS label_column
   FROM point_master point
     JOIN isp_fdb_info fdb ON point.system_id = fdb.system_id AND point.entity_type::text = 'FDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = fdb.system_id AND adi1.entity_type::text = 'FDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), fdb.network_status::text) AND upper(lim.layer_name::text) = upper('FDB'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = fdb.system_id AND adi.entity_type::text = 'FDB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_fdb_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_fms_map;

CREATE OR REPLACE VIEW public.vw_att_details_fms_map
 AS
 SELECT fms.system_id,
    fms.region_id,
    fms.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    fms.fms_name,
    fms.project_id,
    fms.planning_id,
    fms.workorder_id,
    fms.purpose_id,
    fms.network_id,
    fms.status,
    fms.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), fms.network_status::text) AS network_status,
    fms.ownership_type,
    fms.third_party_vendor_id,
    fms.primary_pod_system_id,
    fms.secondary_pod_system_id,
    fms.source_ref_id,
    fms.source_ref_type,
    lim.icon_path,
    fms.area_id AS label_column
   FROM point_master point
     JOIN att_details_fms fms ON point.system_id = fms.system_id AND upper(point.entity_type::text) = 'FMS'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = fms.system_id AND upper(adi1.entity_type::text) = upper('FMS'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), fms.network_status::text) AND upper(lim.layer_name::text) = upper('FMS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = fms.system_id AND upper(adi.entity_type::text) = upper('FMS'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_fms_map
    OWNER TO postgres;


DROP VIEW public.vw_att_details_gipipe_map;

CREATE OR REPLACE VIEW public.vw_att_details_gipipe_map
 AS
 SELECT gipipe.system_id,
    gipipe.network_id,
    gipipe.is_new_entity,
    gipipe.gipipe_name,
    gipipe.pin_code,
    COALESCE(pmapping.province_id, gipipe.province_id) AS province_id,
    COALESCE(pmapping.region_id, gipipe.region_id) AS region_id,
    gipipe.a_location,
    gipipe.b_location,
    gipipe.calculated_length,
    gipipe.manual_length,
    gipipe.construction,
    gipipe.activation,
    gipipe.accessibility,
    gipipe.specification,
    gipipe.category,
    gipipe.vendor_id,
    gipipe.type,
    gipipe.brand,
    gipipe.model,
    gipipe.created_by,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), gipipe.network_status::text) AS network_status,
    gipipe.project_id,
    gipipe.planning_id,
    gipipe.workorder_id,
    gipipe.purpose_id,
    gipipe.status,
    gipipe.gipipe_type,
    gipipe.color_code,
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
        CASE
            WHEN gipipe.no_of_cables = 0 THEN 'vacant'::text
            ELSE 'used'::text
        END AS utilization,
    gipipe.ownership_type,
    gipipe.third_party_vendor_id,
    gipipe.primary_pod_system_id,
    gipipe.secondary_pod_system_id,
    gipipe.source_ref_id,
    gipipe.source_ref_type,
    gipipe.network_id AS label_column
   FROM line_master lm
     JOIN att_details_gipipe gipipe ON lm.system_id = gipipe.system_id AND lm.entity_type::text = 'Gipipe'::text
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(gipipe.network_id::text) ~~ 'NLD%'::text THEN gipipe.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('Gipipe'::text)
            ELSE 1 = 2
        END
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = gipipe.system_id AND upper(adi.entity_type::text) = upper('Gipipe'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = gipipe.system_id AND upper(adi1.entity_type::text) = upper('GIPIPE'::text) AND upper(adi1.description::text) = upper('NS'::text);

ALTER TABLE public.vw_att_details_gipipe_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_handhole_map;

CREATE OR REPLACE VIEW public.vw_att_details_handhole_map
 AS
 SELECT handhole.system_id,
    handhole.region_id,
    handhole.province_id,
    handhole.status,
    handhole.is_new_entity,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    handhole.handhole_name,
    handhole.project_id,
    handhole.planning_id,
    handhole.workorder_id,
    handhole.purpose_id,
    handhole.network_id,
    handhole.source_ref_id,
    handhole.source_ref_type,
    handhole.is_virtual,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), handhole.network_status::text) AS network_status,
    handhole.ownership_type,
    handhole.third_party_vendor_id,
    handhole.primary_pod_system_id,
    handhole.secondary_pod_system_id,
    lim.icon_path,
    handhole.network_id AS label_column
   FROM point_master point
     JOIN att_details_handhole handhole ON point.system_id = handhole.system_id AND point.entity_type::text = 'Handhole'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = handhole.system_id AND adi1.entity_type::text = 'Handhole'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), handhole.network_status::text) AND lim.layer_name::text = 'Handhole'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = handhole.system_id AND adi.entity_type::text = 'Handhole'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_handhole_map
    OWNER TO postgres;


DROP VIEW public.vw_att_details_htb_map;

CREATE OR REPLACE VIEW public.vw_att_details_htb_map
 AS
 SELECT htb.system_id,
    htb.region_id,
    htb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    htb.htb_name,
    htb.project_id,
    htb.planning_id,
    htb.workorder_id,
    htb.purpose_id,
    htb.network_id,
    htb.status,
    htb.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), htb.network_status::text) AS network_status,
    htb.ownership_type,
    htb.third_party_vendor_id,
    htb.primary_pod_system_id,
    htb.secondary_pod_system_id,
    htb.source_ref_id,
    htb.source_ref_type,
    lim.icon_path,
    htb.network_id AS label_column
   FROM point_master point
     JOIN isp_htb_info htb ON point.system_id = htb.system_id AND upper(point.entity_type::text) = 'HTB'::text
     JOIN user_master u ON u.user_id = htb.created_by
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = htb.system_id AND upper(adi1.entity_type::text) = upper('HTB'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), htb.network_status::text) AND upper(lim.layer_name::text) = upper('HTB'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = htb.system_id AND upper(adi.entity_type::text) = upper('HTB'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_htb_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_loop_map;

CREATE OR REPLACE VIEW public.vw_att_details_loop_map
 AS
 SELECT loop.system_id,
    loop.network_status,
    loop.status,
    loop.is_new_entity,
    cable.province_id,
    cable.region_id,
    cable.project_id,
    cable.planning_id,
    cable.workorder_id,
    cable.purpose_id,
    COALESCE(adi.updated_geom, pm.sp_geometry) AS sp_geometry,
    loop.source_ref_id,
    loop.source_ref_type,
    lim.icon_path,
    concat(loop.loop_length, ' (m)') AS label_column
   FROM point_master pm
     JOIN att_details_loop loop ON pm.system_id = loop.system_id AND pm.entity_type::text = 'Loop'::text
     JOIN att_details_cable cable ON loop.cable_system_id = cable.system_id
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = 'O'::text AND upper(lim.layer_name::text) = upper('Loop'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = loop.system_id AND upper(adi.entity_type::text) = upper('Loop'::text);

ALTER TABLE public.vw_att_details_loop_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_manhole_map;

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
    lim.icon_path,
    manhole.manhole_name AS label_column
   FROM point_master point
     JOIN att_details_manhole manhole ON point.system_id = manhole.system_id AND point.entity_type::text = 'Manhole'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = manhole.system_id AND adi1.entity_type::text = 'Manhole'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), manhole.network_status::text) AND lim.is_virtual = manhole.is_virtual AND lim.layer_name::text = 'Manhole'::text;

ALTER TABLE public.vw_att_details_manhole_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_microduct_map;

CREATE OR REPLACE VIEW public.vw_att_details_microduct_map
 AS
 SELECT microduct.system_id,
    microduct.network_id,
    microduct.is_new_entity,
    microduct.microduct_name AS network_name,
    microduct.pin_code,
    microduct.province_id,
    microduct.region_id,
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
    microduct.third_party_vendor_id,
    microduct.primary_pod_system_id,
    microduct.secondary_pod_system_id,
    microduct.source_ref_id,
    microduct.source_ref_type,
    microduct.microduct_name AS label_column
   FROM line_master lm
     JOIN att_details_microduct microduct ON lm.system_id = microduct.system_id AND lm.entity_type::text = 'Microduct'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = microduct.system_id AND adi.entity_type::text = 'Microduct'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = microduct.system_id AND adi1.entity_type::text = 'Microduct'::text AND adi1.description::text = 'NS'::text;

ALTER TABLE public.vw_att_details_microduct_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_microwavelink_map;

CREATE OR REPLACE VIEW public.vw_att_details_microwavelink_map
 AS
 SELECT microwavelink.system_id,
    microwavelink.network_id,
    microwavelink.network_name,
    microwavelink.is_new_entity,
    microwavelink.sequence_id,
    COALESCE(pmapping.province_id, microwavelink.province_id) AS province_id,
    COALESCE(pmapping.region_id, microwavelink.region_id) AS region_id,
    microwavelink.specification,
    microwavelink.category,
    microwavelink.vendor_id,
    microwavelink.type,
    microwavelink.brand,
    microwavelink.model,
    microwavelink.created_by,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), microwavelink.network_status::text) AS network_status,
    microwavelink.status,
    microwavelink.project_id,
    microwavelink.planning_id,
    microwavelink.workorder_id,
    microwavelink.purpose_id,
    microwavelink.link_type,
    microwavelink.link_name,
    microwavelink.ownership_type,
    microwavelink.service_id,
    microwavelink.total_capacity,
    microwavelink.third_party_vendor_id,
    microwavelink.free_capacity,
    microwavelink.user_id,
    microwavelink.construction_stage,
    microwavelink.activation_stage,
    microwavelink.accessibility,
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
    microwavelink.source_ref_id,
    microwavelink.source_ref_type,
    microwavelink.status AS label_column
   FROM line_master lm
     JOIN att_details_microwavelink microwavelink ON lm.system_id = microwavelink.system_id AND upper(lm.entity_type::text) = upper('Microwavelink'::text)
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(microwavelink.network_id::text) ~~ 'NLD%'::text THEN microwavelink.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('Microwavelink'::text)
            ELSE 1 = 2
        END
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = microwavelink.system_id AND upper(adi.entity_type::text) = upper('Microwavelink'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = microwavelink.system_id AND upper(adi1.entity_type::text) = upper('microwavelink'::text) AND upper(adi1.description::text) = upper('NS'::text);

ALTER TABLE public.vw_att_details_microwavelink_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_mpod_map;

CREATE OR REPLACE VIEW public.vw_att_details_mpod_map
 AS
 SELECT mpod.system_id,
    mpod.region_id,
    mpod.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    mpod.mpod_name,
    mpod.project_id,
    mpod.planning_id,
    mpod.purpose_id,
    mpod.workorder_id,
    mpod.network_id,
    mpod.status,
    mpod.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), mpod.network_status::text) AS network_status,
    mpod.ownership_type,
    mpod.third_party_vendor_id,
    mpod.primary_pod_system_id,
    mpod.secondary_pod_system_id,
    mpod.source_ref_id,
    mpod.source_ref_type,
    lim.icon_path,
    mpod.network_id AS label_column
   FROM point_master point
     JOIN att_details_mpod mpod ON point.system_id = mpod.system_id AND upper(point.entity_type::text) = 'MPOD'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = mpod.system_id AND upper(adi1.entity_type::text) = upper('MPOD'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), mpod.network_status::text) AND upper(lim.layer_name::text) = upper('MPOD'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = mpod.system_id AND upper(adi.entity_type::text) = upper('MPOD'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_mpod_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_ont_map;

CREATE OR REPLACE VIEW public.vw_att_details_ont_map
 AS
 SELECT ont.system_id,
    ont.region_id,
    ont.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    ont.ont_name,
    ont.project_id,
    ont.planning_id,
    ont.workorder_id,
    ont.purpose_id,
    ont.network_id,
    ont.status,
    ont.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), ont.network_status::text) AS network_status,
    ont.ownership_type,
    ont.third_party_vendor_id,
    ont.primary_pod_system_id,
    ont.secondary_pod_system_id,
    ont.source_ref_id,
    ont.source_ref_type,
    lim.icon_path,
    ont.gis_design_id AS label_column
   FROM point_master point
     JOIN att_details_ont ont ON point.system_id = ont.system_id AND upper(point.entity_type::text) = 'ONT'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = ont.system_id AND upper(adi1.entity_type::text) = upper('ONT'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), ont.network_status::text) AND upper(lim.layer_name::text) = upper('ONT'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = ont.system_id AND upper(adi.entity_type::text) = upper('ONT'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_ont_map
    OWNER TO postgres;
	
	
	
DROP VIEW public.vw_att_details_opticalrepeater_map;

CREATE OR REPLACE VIEW public.vw_att_details_opticalrepeater_map
 AS
 SELECT opticalrepeater.system_id,
    opticalrepeater.region_id,
    opticalrepeater.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    opticalrepeater.opticalrepeater_name,
    opticalrepeater.project_id,
    opticalrepeater.planning_id,
    opticalrepeater.workorder_id,
    opticalrepeater.purpose_id,
    opticalrepeater.network_id,
    opticalrepeater.status,
    opticalrepeater.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), opticalrepeater.network_status::text) AS network_status,
    opticalrepeater.ownership_type,
    opticalrepeater.third_party_vendor_id,
    opticalrepeater.source_ref_id,
    opticalrepeater.source_ref_type,
    lim.icon_path,
    opticalrepeater.network_id AS label_column
   FROM point_master point
     JOIN isp_opticalrepeater_info opticalrepeater ON point.system_id = opticalrepeater.system_id AND upper(point.entity_type::text) = 'OPTICALREPEATER'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = opticalrepeater.system_id AND upper(adi1.entity_type::text) = upper('OPTICALREPEATER'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), opticalrepeater.network_status::text) AND upper(lim.layer_name::text) = upper('OPTICALREPEATER'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = opticalrepeater.system_id AND upper(adi.entity_type::text) = upper('OPTICALREPEATER'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_opticalrepeater_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_patchpanel_map;

CREATE OR REPLACE VIEW public.vw_att_details_patchpanel_map
 AS
 SELECT patchpanel.system_id,
    patchpanel.region_id,
    patchpanel.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    patchpanel.patchpanel_name,
    patchpanel.project_id,
    patchpanel.planning_id,
    patchpanel.workorder_id,
    patchpanel.purpose_id,
    patchpanel.network_id,
    patchpanel.status,
    patchpanel.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), patchpanel.network_status::text) AS network_status,
    patchpanel.ownership_type,
    patchpanel.third_party_vendor_id,
    patchpanel.primary_pod_system_id,
    patchpanel.secondary_pod_system_id,
    patchpanel.source_ref_id,
    patchpanel.source_ref_type,
    lim.icon_path,
    patchpanel.network_id AS label_column
   FROM point_master point
     JOIN att_details_patchpanel patchpanel ON point.system_id = patchpanel.system_id AND upper(point.entity_type::text) = 'PATCHPANEL'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = patchpanel.system_id AND upper(adi1.entity_type::text) = upper('patchpanel'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), patchpanel.network_status::text) AND upper(lim.layer_name::text) = upper('PATCHPANEL'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = patchpanel.system_id AND upper(adi.entity_type::text) = upper('PATCHPANEL'::text);

ALTER TABLE public.vw_att_details_patchpanel_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_pod_map;

CREATE OR REPLACE VIEW public.vw_att_details_pod_map
 AS
 SELECT pod.system_id,
    pod.region_id,
    pod.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    pod.pod_name,
    pod.project_id,
    pod.planning_id,
    pod.purpose_id,
    pod.workorder_id,
    pod.network_id,
    pod.status,
    pod.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pod.network_status::text) AS network_status,
    pod.ownership_type,
    pod.third_party_vendor_id,
    pod.source_ref_id,
    pod.source_ref_type,
    lim.icon_path,
    pod.gis_design_id AS label_column
   FROM point_master point
     JOIN att_details_pod pod ON point.system_id = pod.system_id AND point.entity_type::text = 'POD'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = pod.system_id AND adi1.entity_type::text = 'pod'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pod.network_status::text) AND lim.layer_name::text = 'POD'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = pod.system_id AND adi.entity_type::text = 'POD'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_pod_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_pole_map;

CREATE OR REPLACE VIEW public.vw_att_details_pole_map
 AS
 SELECT pole.system_id,
    pole.region_id,
    pole.province_id,
    COALESCE(st_geomfromtext(((('Point('::text || pole.longitude) || ' '::text) || pole.latitude) || ')'::text, 4326), point.sp_geometry) AS sp_geometry,
    pole.pole_name,
    pole.project_id,
    pole.planning_id,
    pole.purpose_id,
    pole.workorder_id,
    pole.pole_type,
    pole.network_id,
    pole.status,
    pole.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pole.network_status::text) AS network_status,
    point.db_flag AS upload_id,
    pole.ownership_type,
    pole.third_party_vendor_id,
    pole.primary_pod_system_id,
    pole.secondary_pod_system_id,
    pole.source_ref_id,
    pole.source_ref_type,
        CASE
            WHEN (pole.pole_type::text = ANY (ARRAY['Electrical'::character varying::text, 'Light'::character varying::text, 'Other'::character varying::text])) AND COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pole.network_status::text) = 'P'::text THEN (('icons/Planned/'::text || pole.pole_type::text) || '_pole.png'::character varying::text)::character varying
            WHEN (pole.pole_type::text = ANY (ARRAY['Electrical'::character varying::text, 'Light'::character varying::text, 'Other'::character varying::text])) AND COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pole.network_status::text) = 'A'::text THEN (('icons/AsBuild/'::text || pole.pole_type::text) || '_pole.png'::character varying::text)::character varying
            WHEN (pole.pole_type::text = ANY (ARRAY['Electrical'::character varying::text, 'Light'::character varying::text, 'Other'::character varying::text])) AND COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pole.network_status::text) = 'D'::text THEN (('icons/Dorment/'::text || pole.pole_type::text) || '_pole.png'::character varying::text)::character varying
            ELSE 'icons/Planned/Electric_pole.png'::character varying
        END AS icon_path,
    pole.gis_design_id AS label_column
   FROM point_master point
     JOIN att_details_pole pole ON point.system_id = pole.system_id AND point.entity_type::text = 'Pole'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = pole.system_id AND adi1.entity_type::text = 'Pole'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pole.network_status::text) AND pole.pole_type::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND lim.layer_name::text = 'Pole'::text;

ALTER TABLE public.vw_att_details_pole_map
    OWNER TO postgres;




DROP VIEW public.vw_att_details_rack_map;

CREATE OR REPLACE VIEW public.vw_att_details_rack_map
 AS
 SELECT rack.system_id,
    rack.region_id,
    rack.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    rack.rack_name,
    rack.project_id,
    rack.planning_id,
    rack.purpose_id,
    rack.workorder_id,
    rack.network_id,
    rack.status,
    rack.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), rack.network_status::text) AS network_status,
    ''::character varying AS ownership_type,
    0 AS third_party_vendor_id,
    rack.is_visible_on_map,
    rack.source_ref_id,
    rack.source_ref_type,
    lim.icon_path,
    rack.network_id AS label_column
   FROM point_master point
     JOIN att_details_rack rack ON point.system_id = rack.system_id AND point.entity_type::text = 'Rack'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = rack.system_id AND adi1.entity_type::text = 'Rack'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), rack.network_status::text) AND lim.layer_name::text = 'Rack'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = rack.system_id AND adi.entity_type::text = 'Rack'::text;

ALTER TABLE public.vw_att_details_rack_map
    OWNER TO postgres;




DROP VIEW public.vw_att_details_row_map;

CREATE OR REPLACE VIEW public.vw_att_details_row_map
 AS
 SELECT "row".system_id,
    "row".network_id,
    "row".status,
    "row".is_new_entity,
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
    "row".network_id AS label_column
   FROM ( SELECT polygon_master.system_id,
            polygon_master.entity_type,
            polygon_master.sp_geometry,
            polygon_master.center_line_geom,
            'Polygon'::text AS geom_type,
            ''::text AS row_text
           FROM polygon_master
          WHERE polygon_master.entity_type::text = 'ROW'::text
        UNION ALL
         SELECT circle_master.system_id,
            circle_master.entity_type,
            circle_master.sp_geometry,
            NULL::geometry AS center_line_geom,
            'Circle'::text AS geom_type,
            ('Radius:'::text || circle_master.radious) || '(Mtr)'::text AS row_text
           FROM circle_master) plgn
     JOIN att_details_row "row" ON plgn.system_id = "row".system_id AND plgn.entity_type::text = 'ROW'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = "row".system_id AND adi.entity_type::text = 'ROW'::text;

ALTER TABLE public.vw_att_details_row_map
    OWNER TO postgres;




DROP VIEW public.vw_att_details_sector_map;

CREATE OR REPLACE VIEW public.vw_att_details_sector_map
 AS
 SELECT sector.system_id,
    sector.region_id,
    sector.province_id,
    sector.network_id,
    sector.is_new_entity,
    sector.network_name,
    sector.project_id,
    sector.planning_id,
    sector.purpose_id,
    sector.workorder_id,
    sector.latitude,
    sector.longitude,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), sector.network_status::text) AS network_status,
    sector.ownership_type,
    sector.third_party_vendor_id,
    sector.status,
    sector.sector_type,
    sector.frequency,
    COALESCE(s.color_code, '#1fdec6'::character varying) AS color_code,
    polygon.sp_geometry,
        CASE
            WHEN sector.sector_type::text = 'UMTS'::text THEN 1
            WHEN sector.sector_type::text = '3G'::text THEN 2
            ELSE 3
        END AS sec_order,
    sector.source_ref_id,
    sector.source_ref_type,
    sector.network_name AS label_column
   FROM polygon_master polygon
     JOIN att_details_sector sector ON polygon.system_id = sector.system_id AND upper(polygon.entity_type::text) = 'SECTOR'::text
     LEFT JOIN sector_color_master s ON s.type::text = sector.sector_type::text AND s.frequency::text = sector.frequency::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = sector.system_id AND upper(adi1.entity_type::text) = upper('SECTOR'::text) AND upper(adi1.description::text) = upper('NS'::text)
  ORDER BY (
        CASE
            WHEN sector.sector_type::text = 'UMTS'::text THEN 1
            WHEN sector.sector_type::text = '3G'::text THEN 2
            ELSE 3
        END) DESC;

ALTER TABLE public.vw_att_details_sector_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_slack_map;

CREATE OR REPLACE VIEW public.vw_att_details_slack_map
 AS
 SELECT slack.system_id,
    slack.network_id,
    slack.status,
    slack.is_new_entity,
    slack.network_status,
    cable.province_id,
    cable.region_id,
    cable.project_id,
    cable.planning_id,
    cable.workorder_id,
    cable.purpose_id,
    COALESCE(adi.updated_geom, pm.sp_geometry) AS sp_geometry,
    slack.source_ref_id,
    slack.source_ref_type,
    slack.network_id AS label_column
   FROM point_master pm
     JOIN att_details_slack slack ON pm.system_id = slack.system_id AND pm.entity_type::text = 'Slack'::text
     JOIN att_details_duct cable ON slack.duct_system_id = cable.system_id
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = slack.system_id AND upper(adi.entity_type::text) = 'SLACK'::text;

ALTER TABLE public.vw_att_details_slack_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_spliceclosure_map;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_map
 AS
 SELECT spliceclosure.system_id,
    spliceclosure.region_id,
    spliceclosure.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    spliceclosure.spliceclosure_name,
    spliceclosure.status,
    spliceclosure.is_new_entity,
    spliceclosure.project_id,
    spliceclosure.planning_id,
    spliceclosure.workorder_id,
    spliceclosure.purpose_id,
    spliceclosure.network_id,
    spliceclosure.is_virtual,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), spliceclosure.network_status::text) AS network_status,
    spliceclosure.ownership_type,
    spliceclosure.third_party_vendor_id,
    spliceclosure.primary_pod_system_id,
    spliceclosure.secondary_pod_system_id,
    spliceclosure.is_visible_on_map,
    spliceclosure.source_ref_id,
    spliceclosure.source_ref_type,
    lim.icon_path,
    spliceclosure.network_id AS label_column
   FROM point_master point
     JOIN att_details_spliceclosure spliceclosure ON point.system_id = spliceclosure.system_id AND point.entity_type::text = 'SpliceClosure'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = spliceclosure.system_id AND adi1.entity_type::text = 'SpliceClosure'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), spliceclosure.network_status::text) AND lim.is_virtual = spliceclosure.is_virtual AND lim.layer_name::text = 'SpliceClosure'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = spliceclosure.system_id AND adi.entity_type::text = 'SpliceClosure'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_spliceclosure_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_splitter_map;

CREATE OR REPLACE VIEW public.vw_att_details_splitter_map
 AS
 SELECT splitter.system_id,
    splitter.region_id,
    splitter.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    splitter.splitter_name,
    splitter.network_id,
    splitter.project_id,
    splitter.planning_id,
    splitter.is_new_entity,
    splitter.status,
    splitter.purpose_id,
    splitter.workorder_id,
    splitter.splitter_type,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), splitter.network_status::text) AS network_status,
    splitter.ownership_type,
    splitter.third_party_vendor_id,
    splitter.primary_pod_system_id,
    splitter.secondary_pod_system_id,
    splitter.source_ref_id,
    splitter.source_ref_type,
    lim.icon_path,
    splitter.gis_design_id AS label_column
   FROM point_master point
     JOIN att_details_splitter splitter ON point.system_id = splitter.system_id AND upper(point.entity_type::text) = 'SPLITTER'::text
     LEFT JOIN isp_type_master tp ON splitter.type = tp.id
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = splitter.system_id AND upper(adi1.entity_type::text) = upper('SPLITTER'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), splitter.network_status::text) AND COALESCE(splitter.splitter_type, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND upper(lim.layer_name::text) = upper('SPLITTER'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = splitter.system_id AND upper(adi.entity_type::text) = upper('SPLITTER'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
  ORDER BY (
        CASE
            WHEN splitter.splitter_type::text = 'Primary'::text THEN 1
            ELSE 0
        END);

ALTER TABLE public.vw_att_details_splitter_map
    OWNER TO postgres;




DROP VIEW public.vw_att_details_structure_map;

CREATE OR REPLACE VIEW public.vw_att_details_structure_map
 AS
 SELECT structure.system_id,
    plgn.sp_geometry,
    structure.structure_name,
    structure.network_id,
    structure.building_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), structure.network_status::text) AS network_status,
    structure.status,
    structure.is_new_entity,
    structure.region_id,
    structure.province_id,
    plgn.geom_type,
    plgn.sp_centroid,
    plgn.display_name,
    lim.icon_path,
    structure.structure_name AS label_column,
    structure.source_ref_id,
    structure.source_ref_type
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
                  WHERE point_master.entity_type::text = 'Structure'::text
                UNION ALL
                 SELECT polygon_master.system_id,
                    polygon_master.sp_geometry,
                    st_centroid(polygon_master.sp_geometry) AS sp_centroid,
                    'Polygon'::text AS text,
                    1 AS order_colum,
                    polygon_master.display_name
                   FROM polygon_master
                  WHERE polygon_master.entity_type::text = 'Structure'::text) tbl) plgn
     JOIN att_details_bld_structure structure ON plgn.system_id = structure.system_id AND plgn.row_num = 1
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = structure.system_id AND adi1.entity_type::text = 'Structure'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), structure.network_status::text) AND lim.layer_name::text = 'Structure'::text;

ALTER TABLE public.vw_att_details_structure_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_subarea_map;

CREATE OR REPLACE VIEW public.vw_att_details_subarea_map
 AS
 SELECT subarea.system_id,
    subarea.region_id,
    subarea.province_id,
    COALESCE(adi.updated_geom, subarea.sp_geometry) AS sp_geometry,
    subarea.subarea_name,
    subarea.network_id,
    subarea.building_system_id,
    clr.color_code,
    subarea.network_status,
    subarea.status,
    subarea.is_new_entity,
    subarea.source_ref_id,
    subarea.source_ref_type,
    (('     '::text || COALESCE(subarea.network_id, ''::character varying)::text) || '
     HP:-'::text) || subarea.no_of_home_pass::text AS label_column
   FROM ( SELECT subarea_1.system_id,
            subarea_1.region_id,
            subarea_1.province_id,
            plgn.sp_geometry,
            subarea_1.status,
            subarea_1.is_new_entity,
            subarea_1.subarea_name,
            subarea_1.network_id,
            subarea_1.building_system_id,
            subarea_1.parent_system_id,
            subarea_1.source_ref_id,
            subarea_1.source_ref_type,
            row_number() OVER (PARTITION BY subarea_1.parent_system_id, subarea_1.parent_entity_type ORDER BY subarea_1.created_on) AS bg_color_id,
            subarea_1.network_status,
            subarea_1.no_of_home_pass,
            subarea_1.gis_design_id
           FROM polygon_master plgn
             JOIN att_details_subarea subarea_1 ON plgn.system_id = subarea_1.system_id AND plgn.entity_type::text = 'SubArea'::text) subarea
     LEFT JOIN subarea_color_master clr ON
        CASE
            WHEN (subarea.bg_color_id % 5::bigint) = 0 THEN 1::bigint
            ELSE subarea.bg_color_id % 5::bigint
        END = clr.color_id
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = subarea.system_id AND adi.entity_type::text = 'SubArea'::text;

ALTER TABLE public.vw_att_details_subarea_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_tower_map;

CREATE OR REPLACE VIEW public.vw_att_details_tower_map
 AS
 SELECT tower.system_id,
    tower.region_id,
    tower.province_id,
    tower.network_id,
    tower.network_name,
    tower.project_id,
    tower.planning_id,
    tower.purpose_id,
    tower.workorder_id,
    tower.latitude,
    tower.longitude,
    tower.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), tower.network_status::text) AS network_status,
    tower.ownership_type,
    tower.third_party_vendor_id,
    tower.status,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    tower.source_ref_id,
    tower.source_ref_type,
    lim.icon_path,
    tower.tenancy AS label_column
   FROM point_master point
     JOIN att_details_tower tower ON point.system_id = tower.system_id AND upper(point.entity_type::text) = 'TOWER'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = tower.system_id AND upper(adi1.entity_type::text) = upper('TOWER'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), tower.network_status::text) AND upper(lim.layer_name::text) = upper('TOWER'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = tower.system_id AND upper(adi.entity_type::text) = upper('TOWER'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_tower_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_tree_map;

CREATE OR REPLACE VIEW public.vw_att_details_tree_map
 AS
 SELECT tree.system_id,
    tree.region_id,
    tree.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    tree.tree_name,
    tree.project_id,
    tree.planning_id,
    tree.workorder_id,
    tree.purpose_id,
    tree.network_id,
    tree.status,
    tree.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), tree.network_status::text) AS network_status,
    ''::text AS ownership_type,
    tree.primary_pod_system_id,
    tree.secondary_pod_system_id,
    tree.source_ref_id,
    tree.source_ref_type,
    lim.icon_path,
    tree.tree_name AS label_column
   FROM point_master point
     JOIN att_details_tree tree ON point.system_id = tree.system_id AND point.entity_type::text = 'Tree'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = tree.system_id AND adi1.entity_type::text = 'Tree'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), tree.network_status::text) AND lim.layer_name::text = 'Tree'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = tree.system_id AND adi.entity_type::text = 'Tree'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_tree_map
    OWNER TO postgres;




DROP VIEW public.vw_att_details_trench_map;

CREATE OR REPLACE VIEW public.vw_att_details_trench_map
 AS
 SELECT trench.system_id,
    trench.network_id,
    trench.is_new_entity,
    trench.trench_name,
    trench.province_id,
    trench.region_id,
    trench.no_of_ducts::text || 'D'::text AS count_ducts,
    trench.construction,
    trench.activation,
    trench.accessibility,
    trench.specification,
    trench.category,
    trench.vendor_id,
    trench.type,
    trench.brand,
    trench.model,
    trench.created_by,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), trench.network_status::text) AS network_status,
    trench.status,
    trench.project_id,
    trench.planning_id,
    trench.workorder_id,
    trench.purpose_id,
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
    trench.ownership_type,
    trench.third_party_vendor_id,
    trench.primary_pod_system_id,
    trench.secondary_pod_system_id,
    trench.source_ref_id,
    trench.source_ref_type,
    trench.network_id AS label_column
   FROM line_master lm
     JOIN att_details_trench trench ON lm.system_id = trench.system_id AND lm.entity_type::text = 'Trench'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = trench.system_id AND adi.entity_type::text = 'Trench'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = trench.system_id AND adi1.entity_type::text = 'Trench'::text AND adi1.description::text = 'NS'::text;

ALTER TABLE public.vw_att_details_trench_map
    OWNER TO postgres;



DROP VIEW public.vw_att_details_vault_map;

CREATE OR REPLACE VIEW public.vw_att_details_vault_map
 AS
 SELECT vault.system_id,
    vault.region_id,
    vault.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    vault.vault_name,
    vault.project_id,
    vault.planning_id,
    vault.purpose_id,
    vault.workorder_id,
    vault.network_id,
    vault.status,
    vault.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), vault.network_status::text) AS network_status,
    vault.ownership_type,
    vault.third_party_vendor_id,
    vault.source_ref_id,
    vault.source_ref_type,
    lim.icon_path,
    vault.network_id AS label_column
   FROM point_master point
     JOIN att_details_vault vault ON point.system_id = vault.system_id AND upper(point.entity_type::text) = 'VAULT'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = vault.system_id AND upper(adi1.entity_type::text) = upper('Pole'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), vault.network_status::text) AND upper(lim.layer_name::text) = upper('VAULT'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = vault.system_id AND upper(adi.entity_type::text) = upper('VAULT'::text);

ALTER TABLE public.vw_att_details_vault_map
    OWNER TO postgres;




DROP VIEW public.vw_att_details_wallmount_map;

CREATE OR REPLACE VIEW public.vw_att_details_wallmount_map
 AS
 SELECT wallmount.system_id,
    wallmount.region_id,
    wallmount.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    wallmount.wallmount_name,
    wallmount.project_id,
    wallmount.planning_id,
    wallmount.workorder_id,
    wallmount.purpose_id,
    wallmount.network_id,
    wallmount.status,
    wallmount.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), wallmount.network_status::text) AS network_status,
    wallmount.ownership_type,
    wallmount.third_party_vendor_id,
    wallmount.primary_pod_system_id,
    wallmount.secondary_pod_system_id,
    wallmount.source_ref_id,
    wallmount.source_ref_type,
    lim.icon_path,
    wallmount.network_id AS label_column
   FROM point_master point
     JOIN att_details_wallmount wallmount ON point.system_id = wallmount.system_id AND point.entity_type::text = 'WallMount'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = wallmount.system_id AND adi1.entity_type::text = 'WallMount'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), wallmount.network_status::text) AND lim.layer_name::text = 'WallMount'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = wallmount.system_id AND adi.entity_type::text = 'WallMount'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_wallmount_map
    OWNER TO postgres;

