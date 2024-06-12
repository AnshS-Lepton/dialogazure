DROP VIEW public.vw_att_details_cable_vector;

CREATE OR REPLACE VIEW public.vw_att_details_cable_vector
 AS
 SELECT cable.system_id,
    cable.region_id,
    cable.province_id,
    cable.cable_name,
    cable.cable_type,
    cable.cable_category,
    cable.network_status,
	cable.ownership_type,
	coalesce(cable.third_party_vendor_id,0)third_party_vendor_id,
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
     JOIN att_details_cable cable ON lm.system_id = cable.system_id AND lm.entity_type::text = 'Cable'::text;

ALTER TABLE public.vw_att_details_cable_vector
    OWNER TO postgres;
	
	
	 DROP VIEW public.vw_att_details_adb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_adb_vector
 AS
 SELECT adb.system_id,
    adb.region_id,
    adb.province_id,
    point.sp_geometry,
    adb.adb_name,
    adb.network_id,
    adb.ownership_type,
    coalesce(adb.third_party_vendor_id,0)third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), adb.network_status::text) AS network_status,
    adb.gis_design_id AS label_column,
        CASE
            WHEN COALESCE(adb.gis_design_id, ''::character varying)::text = ''::text THEN adb.network_id
            ELSE adb.gis_design_id
        END::character varying AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_adb adb ON point.system_id = adb.system_id AND point.entity_type::text = 'ADB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = adb.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = adb.system_id AND adi1.entity_type::text = 'ADB'::text AND adi1.description::text = 'NS'::text;

ALTER TABLE public.vw_att_details_adb_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_bdb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_bdb_vector
 AS
 SELECT bdb.system_id,
    bdb.region_id,
    bdb.province_id,
    point.sp_geometry,
    bdb.bdb_name,
    bdb.network_id,
    bdb.ownership_type,
    coalesce(bdb.third_party_vendor_id,0)third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), bdb.network_status::text) AS network_status,
    bdb.gis_design_id AS label_column,
        CASE
            WHEN COALESCE(bdb.gis_design_id, ''::character varying)::text = ''::text THEN bdb.network_id
            ELSE bdb.gis_design_id
        END::character varying AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_bdb bdb ON point.system_id = bdb.system_id AND point.entity_type::text = 'BDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = bdb.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = bdb.system_id AND adi1.entity_type::text = 'BDB'::text AND adi1.description::text = 'NS'::text;

ALTER TABLE public.vw_att_details_bdb_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_cabinet_vector;

CREATE OR REPLACE VIEW public.vw_att_details_cabinet_vector
 AS
 SELECT cabinet.system_id,
    cabinet.region_id,
    cabinet.province_id,
    COALESCE(pm.system_id, 0) AS subarea_id,
    cabinet.cabinet_name,
    cabinet.network_id,
    cabinet.network_status,
    cabinet.gis_design_id,
    point.display_name,
    point.entity_category,
    cabinet.ownership_type,
    coalesce(cabinet.third_party_vendor_id,0)third_party_vendor_id
   FROM point_master point
     JOIN att_details_cabinet cabinet ON point.system_id = cabinet.system_id AND point.entity_type::text = 'Cabinet'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry);

ALTER TABLE public.vw_att_details_cabinet_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_fdb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_fdb_vector
 AS
 SELECT fdb.system_id,
    fdb.region_id,
    fdb.province_id,
    point.sp_geometry,
    fdb.fdb_name,
    fdb.network_id,
    fdb.ownership_type,
    coalesce(fdb.third_party_vendor_id,0)third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), fdb.network_status::text) AS network_status,
    fdb.gis_design_id AS label_column,
        CASE
            WHEN COALESCE(fdb.gis_design_id, ''::character varying)::text = ''::text THEN fdb.network_id
            ELSE fdb.gis_design_id
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN isp_fdb_info fdb ON point.system_id = fdb.system_id AND point.entity_type::text = 'FDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = fdb.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = fdb.system_id AND adi1.entity_type::text = 'FDB'::text AND adi1.description::text = 'NS'::text;

ALTER TABLE public.vw_att_details_fdb_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_fms_vector;

CREATE OR REPLACE VIEW public.vw_att_details_fms_vector
 AS
 SELECT fms.system_id,
    fms.region_id,
    fms.province_id,
    fms.fms_name,
    fms.network_id,
    fms.network_status::text AS network_status,
    fms.network_id AS label_column,
    point.sp_geometry,
    fms.ownership_type,
    coalesce(fms.third_party_vendor_id,0)third_party_vendor_id,
        CASE
            WHEN COALESCE(fms.gis_design_id, ''::character varying)::text = ''::text THEN fms.network_id
            ELSE fms.gis_design_id
        END::character varying AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_fms fms ON point.system_id = fms.system_id AND upper(point.entity_type::text) = 'FMS'::text;

ALTER TABLE public.vw_att_details_fms_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_handhole_vector;

CREATE OR REPLACE VIEW public.vw_att_details_handhole_vector
 AS
 SELECT handhole.system_id,
    handhole.region_id,
    handhole.province_id,
    handhole.subarea_system_id AS subarea_id,
    point.sp_geometry,
    handhole.handhole_name,
    handhole.category,
    handhole.network_id,
    handhole.network_status,
    point.display_name,
    point.entity_category,
    handhole.ownership_type,
    coalesce(handhole.third_party_vendor_id,0)third_party_vendor_id,
    COALESCE(handhole.is_virtual, false) AS is_virtual
   FROM point_master point
     JOIN att_details_handhole handhole ON point.system_id = handhole.system_id AND point.entity_type::text = 'Handhole'::text;

ALTER TABLE public.vw_att_details_handhole_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_htb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_htb_vector
 AS
 SELECT htb.system_id,
    htb.region_id,
    htb.province_id,
    htb.htb_name,
    htb.htb_type,
    htb.network_status,
    htb.type,
    htb.gis_design_id,
    htb.category,
    point.display_name,
    point.entity_category,
    htb.ownership_type,
    coalesce(htb.third_party_vendor_id,0)third_party_vendor_id
   FROM point_master point
     JOIN isp_htb_info htb ON point.system_id = htb.system_id AND point.entity_type::text = 'htb'::text;

ALTER TABLE public.vw_att_details_htb_vector
    OWNER TO postgres;



DROP VIEW public.vw_att_details_manhole_vector;

CREATE OR REPLACE VIEW public.vw_att_details_manhole_vector
 AS
 SELECT manhole.system_id,
    manhole.region_id,
    manhole.province_id,
    COALESCE(pm.system_id, 0) AS subarea_id,
    point.sp_geometry,
    manhole.manhole_name,
    manhole.category,
    manhole.network_id,
    manhole.network_status,
    manhole.ownership_type,
    coalesce(manhole.third_party_vendor_id,0)third_party_vendor_id,
        CASE
            WHEN COALESCE(manhole.gis_design_id, ''::character varying)::text = ''::text THEN manhole.network_id
            ELSE manhole.gis_design_id
        END::character varying AS display_name,
    point.entity_category,
    COALESCE(manhole.is_virtual, false) AS is_virtual
   FROM point_master point
     JOIN att_details_manhole manhole ON point.system_id = manhole.system_id AND point.entity_type::text = 'Manhole'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry);

ALTER TABLE public.vw_att_details_manhole_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_ont_vector;

CREATE OR REPLACE VIEW public.vw_att_details_ont_vector
 AS
 SELECT ont.system_id,
    ont.region_id,
    ont.province_id,
    ont.ont_name,
    ont.network_id,
    ont.ownership_type,
    coalesce(ont.third_party_vendor_id,0)third_party_vendor_id,
    ont.network_status::text AS network_status,
    ont.network_id AS label_column,
    point.sp_geometry,
        CASE
            WHEN COALESCE(ont.gis_design_id, ''::character varying)::text = ''::text THEN ont.network_id
            ELSE ont.gis_design_id
        END::character varying AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_ont ont ON point.system_id = ont.system_id AND upper(point.entity_type::text) = 'ONT'::text;

ALTER TABLE public.vw_att_details_ont_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_pod_vector;

CREATE OR REPLACE VIEW public.vw_att_details_pod_vector
 AS
 SELECT pod.system_id,
    pod.region_id,
    pod.province_id,
    pod.pod_name,
    pod.network_id,
    pod.network_status::text AS network_status,
    pod.gis_design_id AS label_column,
    pod.ownership_type,
    coalesce(pod.third_party_vendor_id,0)third_party_vendor_id,
        CASE
            WHEN COALESCE(pod.gis_design_id, ''::character varying)::text = ''::text THEN pod.network_id
            ELSE pod.gis_design_id
        END::character varying AS display_name,
    point.entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN att_details_pod pod ON point.system_id = pod.system_id AND point.entity_type::text = 'POD'::text;

ALTER TABLE public.vw_att_details_pod_vector
    OWNER TO postgres;

DROP VIEW public.vw_att_details_pole_vector;

CREATE OR REPLACE VIEW public.vw_att_details_pole_vector
 AS
 SELECT pole.system_id,
    pole.region_id,
    pole.province_id,
    point.sp_geometry,
    pole.pole_name,
    pole.pole_type,
    pole.network_id,
    pole.network_status,
    pole.ownership_type,
    coalesce(pole.third_party_vendor_id,0)third_party_vendor_id,
        CASE
            WHEN COALESCE(pole.gis_design_id, ''::character varying)::text = ''::text THEN pole.network_id
            ELSE pole.gis_design_id
        END::character varying AS display_name,
    pole.pole_type AS entity_category
   FROM point_master point
     JOIN att_details_pole pole ON point.system_id = pole.system_id AND point.entity_type::text = 'Pole'::text;

ALTER TABLE public.vw_att_details_pole_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_spliceclosure_vector;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_vector
 AS
 SELECT spliceclosure.system_id,
    spliceclosure.region_id,
    spliceclosure.province_id,
    point.sp_geometry,
    spliceclosure.spliceclosure_name,
    spliceclosure.ownership_type,
    coalesce(spliceclosure.third_party_vendor_id,0)third_party_vendor_id,
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
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.source_ref_id::text = spliceclosure.source_ref_id::text AND upper(adi1.source_ref_type::text) = 'NETWORK_TICKET'::text AND adi1.entity_system_id = spliceclosure.system_id AND upper(adi1.entity_type::text) = upper('SPLICECLOSURE'::text) AND upper(adi1.description::text) = upper('NS'::text);

ALTER TABLE public.vw_att_details_spliceclosure_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_splitter_vector;

CREATE OR REPLACE VIEW public.vw_att_details_splitter_vector
 AS
 SELECT splitter.system_id,
    splitter.region_id,
    splitter.province_id,
    point.sp_geometry,
    splitter.splitter_name,
    splitter.splitter_type,
    splitter.ownership_type,
    coalesce(splitter.third_party_vendor_id,0)third_party_vendor_id,
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
  ORDER BY (
        CASE
            WHEN splitter.splitter_type::text = 'Primary'::text THEN 1
            ELSE 0
        END);

ALTER TABLE public.vw_att_details_splitter_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_tower_vector;

CREATE OR REPLACE VIEW public.vw_att_details_tower_vector
 AS
 SELECT tower.system_id,
    tower.region_id,
    tower.province_id,
    COALESCE(pm.system_id, 0) AS subarea_id,
    point.sp_geometry,
    tower.network_name,
    tower.category,
    tower.network_id,
    tower.ownership_type,
    coalesce(tower.third_party_vendor_id,0)third_party_vendor_id,
    tower.network_status,
    point.display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_tower tower ON point.system_id = tower.system_id AND point.entity_type::text = 'Tower'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry);

ALTER TABLE public.vw_att_details_tower_vector
    OWNER TO postgres;

DROP VIEW public.vw_att_details_trench_vector;

CREATE OR REPLACE VIEW public.vw_att_details_trench_vector
 AS
 SELECT trench.system_id,
    trench.network_id,
    trench.trench_name,
    COALESCE(pmapping.province_id, trench.province_id) AS province_id,
    COALESCE(pmapping.region_id, trench.region_id) AS region_id,
    trench.no_of_ducts::text || 'D'::text AS count_ducts,
    trench.category,
    trench.type,
    trench.network_status::text AS network_status,
    trench.status,
    lm.sp_geometry,
    trench.ownership_type,
    coalesce(trench.third_party_vendor_id,0)third_party_vendor_id,
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
        END;

ALTER TABLE public.vw_att_details_trench_vector
    OWNER TO postgres;


DROP VIEW public.vw_att_details_wallmount_vector;

CREATE OR REPLACE VIEW public.vw_att_details_wallmount_vector
 AS
 SELECT wallmount.system_id,
    wallmount.region_id,
    wallmount.province_id,
    point.sp_geometry,
    wallmount.wallmount_name,
    wallmount.network_id,
    wallmount.network_status,
    wallmount.ownership_type,
    coalesce(wallmount.third_party_vendor_id,0)third_party_vendor_id,
        CASE
            WHEN COALESCE(wallmount.gis_design_id, ''::character varying)::text = ''::text THEN wallmount.network_id
            ELSE wallmount.gis_design_id
        END::character varying AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_wallmount wallmount ON point.system_id = wallmount.system_id AND point.entity_type::text = 'WallMount'::text;

ALTER TABLE public.vw_att_details_wallmount_vector
    OWNER TO postgres;
	
 DROP VIEW public.vw_att_details_antenna_vector;

CREATE OR REPLACE VIEW public.vw_att_details_antenna_vector
 AS
 SELECT antenna.system_id,
    antenna.region_id,
    antenna.province_id,
    antenna.network_id AS antenna_name,
    antenna.network_status,
    pm.sp_geometry,
    antenna.network_id AS label_column,
    antenna.network_id,
    antenna.network_id AS display_name,
    antenna.ownership_type,
    coalesce(antenna.third_party_vendor_id,0)third_party_vendor_id
   FROM point_master pm
     JOIN att_details_antenna antenna ON pm.system_id = antenna.system_id AND pm.entity_type::text = 'Antenna'::text;

ALTER TABLE public.vw_att_details_antenna_vector
    OWNER TO postgres;



CREATE OR REPLACE VIEW public.vw_att_details_duct_vector
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.duct_name,
    duct.pin_code,
    COALESCE(pmapping.province_id, duct.province_id) AS province_id,
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
    coalesce(duct.third_party_vendor_id,0)third_party_vendor_id,
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
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = duct.system_id AND upper(adi1.entity_type::text) = upper('DUCT'::text) AND upper(adi1.description::text) = upper('NS'::text);

ALTER TABLE public.vw_att_details_duct_vector
    OWNER TO postgres;
	
	
	
DROP VIEW public.vw_att_details_patchpanel_vector;

CREATE OR REPLACE VIEW public.vw_att_details_patchpanel_vector
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
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), patchpanel.network_status::text) AS network_status,
    patchpanel.ownership_type,
    coalesce(patchpanel.third_party_vendor_id,0)third_party_vendor_id,
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
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = patchpanel.system_id AND upper(adi.entity_type::text) = upper('PATCHPANEL'::text);

ALTER TABLE public.vw_att_details_patchpanel_vector
    OWNER TO postgres;
	
	
	
	DROP VIEW public.vw_att_details_microduct_vector;

CREATE OR REPLACE VIEW public.vw_att_details_microduct_vector
 AS
 SELECT microduct.system_id,
    microduct.network_id,
    microduct.microduct_name,
    microduct.pin_code,
    COALESCE(pmapping.province_id, microduct.province_id) AS province_id,
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
    coalesce(microduct.third_party_vendor_id,0)third_party_vendor_id,
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
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = microduct.system_id AND upper(adi1.entity_type::text) = upper('Microduct'::text) AND upper(adi1.description::text) = upper('NS'::text);

ALTER TABLE public.vw_att_details_microduct_vector
    OWNER TO postgres;
	
	
	
	DROP VIEW public.vw_att_details_sector_vector;

CREATE OR REPLACE VIEW public.vw_att_details_sector_vector
 AS
 SELECT sector.system_id,
    sector.region_id,
    sector.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    sector.network_name,
    sector.project_id,
    sector.planning_id,
    sector.workorder_id,
    sector.purpose_id,
    sector.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), sector.network_status::text) AS network_status,
    sector.ownership_type,
    coalesce(sector.third_party_vendor_id,0)third_party_vendor_id,
    sector.source_ref_id,
    sector.source_ref_type,
    sector.network_id AS label_column,
        CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN sector.network_id
            ELSE point.display_name
        END AS display_name
   FROM polygon_master point
     JOIN att_details_sector sector ON point.system_id = sector.system_id AND upper(point.entity_type::text) = 'SECTOR'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = sector.system_id AND upper(adi1.entity_type::text) = upper('sector'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = sector.system_id AND upper(adi.entity_type::text) = upper('sector'::text);

ALTER TABLE public.vw_att_details_sector_vector
    OWNER TO postgres;
	
	DROP VIEW public.vw_att_details_cdb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_cdb_vector
 AS
 SELECT cdb.system_id,
    cdb.region_id,
    cdb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    cdb.cdb_name,
    cdb.network_id,
    cdb.ownership_type,
    coalesce(cdb.third_party_vendor_id,0)third_party_vendor_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), cdb.network_status::text) AS network_status,
    cdb.gis_design_id AS label_column,
    point.display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_cdb cdb ON point.system_id = cdb.system_id AND point.entity_type::text = 'CDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = cdb.system_id AND adi1.entity_type::text = 'CDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = cdb.system_id AND adi.entity_type::text = 'CDB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_cdb_vector
    OWNER TO postgres;
	
	---------------------------------------------
	
	
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






---------------------------18 Aug 12 PM----------------------------------------------------------

truncate table cable_geojson_master
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
	SELECT *  FROM vw_att_details_cable_vector
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
		  
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_HTB_vector
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