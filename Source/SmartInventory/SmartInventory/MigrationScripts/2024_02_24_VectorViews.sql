drop view vw_att_details_adb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_adb_vector
AS SELECT adb.system_id,
    adb.region_id,
    adb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    adb.adb_name,
    adb.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), adb.network_status::text) AS network_status,
    adb.gis_design_id AS label_column,
    CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN adb.network_id
            ELSE point.display_name
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_adb adb ON point.system_id = adb.system_id AND point.entity_type::text = 'ADB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = adb.system_id AND adi1.entity_type::text = 'ADB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = adb.system_id AND adi.entity_type::text = 'ADB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;


drop view vw_att_details_area_vector;

CREATE OR REPLACE VIEW public.vw_att_details_area_vector
AS SELECT area.system_id,
    area.area_name,
    plgn.sp_geometry,
    area.region_id,
    area.province_id,
    area.network_status,
    area.network_id,
    CASE
            WHEN COALESCE(plgn.display_name, ''::character varying)::text = ''::text THEN area.network_id
            ELSE plgn.display_name
        END AS display_name
   FROM polygon_master plgn
     JOIN att_details_area area ON plgn.system_id = area.system_id AND plgn.entity_type::text = 'Area'::text;


drop view vw_att_details_bdb_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_bdb_vector
AS SELECT bdb.system_id,
    bdb.region_id,
    bdb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    bdb.bdb_name,
    bdb.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), bdb.network_status::text) AS network_status,
    bdb.gis_design_id AS label_column,
    CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN bdb.network_id
            ELSE point.display_name
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_bdb bdb ON point.system_id = bdb.system_id AND point.entity_type::text = 'BDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = bdb.system_id AND adi1.entity_type::text = 'BDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = bdb.system_id AND adi.entity_type::text = 'BDB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;



drop view vw_att_details_cabinet_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_cabinet_vector
AS SELECT cabinet.system_id,
    cabinet.region_id,
    cabinet.province_id,
    cabinet.cabinet_name,
    cabinet.network_id,
    cabinet.network_status,
    cabinet.gis_design_id,
    CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN cabinet.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN att_details_cabinet cabinet ON point.system_id = cabinet.system_id AND point.entity_type::text = 'Cabinet'::text;


drop view vw_att_details_cable_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_cable_vector
AS SELECT cable.system_id,
    cable.region_id,
    cable.province_id,
    cable.cable_name,
    cable.cable_type,
    cable.cable_category,
    cable.network_status,
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
            WHEN COALESCE(lm.display_name, ''::character varying)::text = ''::text THEN cable.network_id
            ELSE lm.display_name
        END AS display_name,
    COALESCE(cable.cable_category, ''::character varying) AS entity_category
   FROM line_master lm
     JOIN att_details_cable cable ON lm.system_id = cable.system_id AND lm.entity_type::text = 'Cable'::text;


drop view vw_att_details_csa_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_csa_vector
AS SELECT csa.system_id,
    csa.csa_name,
    plgn.sp_geometry,
    csa.region_id,
    csa.province_id,
    csa.network_status,
    csa.network_id,
    CASE
            WHEN COALESCE(plgn.display_name, ''::character varying)::text = ''::text THEN csa.network_id
            ELSE plgn.display_name
        END AS display_name,
    csa.rfs_status
   FROM polygon_master plgn
     JOIN att_details_csa csa ON plgn.system_id = csa.system_id AND plgn.entity_type::text = 'CSA'::text;


drop view vw_att_details_customer_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_customer_vector
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
    lim.icon_path,
    customer.network_id AS label_column,
    customer.network_status,
    ''::text AS entity_category,
    CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN customer.network_id
            ELSE point.display_name
        END AS display_name
   FROM point_master point
     JOIN att_details_customer customer ON point.system_id = customer.system_id AND upper(point.entity_type::text) = 'CUSTOMER'::text
     LEFT JOIN vw_layer_icon_master lim ON lim.network_abbreviation::text = customer.network_status::text AND COALESCE(customer.customer_type, ''::character varying)::text =
        CASE
            WHEN COALESCE(lim.category, ''::character varying)::text <> ''::text THEN lim.category
            ELSE ''::character varying
        END::text AND upper(lim.layer_name::text) = upper('CUSTOMER'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = customer.system_id AND upper(adi.entity_type::text) = upper('CUSTOMER'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;


drop view vw_att_details_dsa_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_dsa_vector
AS SELECT dsa.system_id,
    dsa.dsa_name,
    plgn.sp_geometry,
    dsa.region_id,
    dsa.province_id,
    dsa.network_status,
    dsa.network_id,
   CASE
            WHEN COALESCE(plgn.display_name, ''::character varying)::text = ''::text THEN  dsa.network_id
            ELSE plgn.display_name
        END AS display_name
   FROM polygon_master plgn
     JOIN att_details_dsa dsa ON plgn.system_id = dsa.system_id AND plgn.entity_type::text = 'DSA'::text;


drop view vw_att_details_duct_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_duct_vector
AS SELECT duct.system_id,
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
    duct.third_party_vendor_id,
    duct.primary_pod_system_id,
    duct.secondary_pod_system_id,
    duct.source_ref_id,
    duct.source_ref_type,
    duct.duct_name AS label_column,
    COALESCE(duct.category, ''::character varying) AS entity_category,
   CASE
            WHEN COALESCE(lm.display_name, ''::character varying)::text = ''::text THEN  duct.network_id
            ELSE lm.display_name
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


drop view vw_att_details_equipment_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_equipment_vector
AS SELECT equipment.system_id,
    equipment.region_id,
    equipment.province_id,
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
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  equipment.network_id
            ELSE point.display_name
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_model equipment ON point.system_id = equipment.system_id AND lower(point.entity_type::text) = 'equipment'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = equipment.system_id AND upper(adi1.entity_type::text) = upper('equipment'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = equipment.system_id AND upper(adi.entity_type::text) = upper('equipment'::text);


drop view vw_att_details_fdb_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_fdb_vector
AS SELECT fdb.system_id,
    fdb.region_id,
    fdb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    fdb.fdb_name,
    fdb.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), fdb.network_status::text) AS network_status,
    fdb.gis_design_id AS label_column,
   CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  fdb.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN isp_fdb_info fdb ON point.system_id = fdb.system_id AND point.entity_type::text = 'FDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = fdb.system_id AND adi1.entity_type::text = 'FDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = fdb.system_id AND adi.entity_type::text = 'FDB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;


drop view vw_att_details_fms_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_fms_vector
AS SELECT fms.system_id,
    fms.region_id,
    fms.province_id,
    fms.fms_name,
    fms.network_id,
    fms.network_status::text AS network_status,
    fms.network_id AS label_column,
    point.sp_geometry,
   CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  fms.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_fms fms ON point.system_id = fms.system_id AND upper(point.entity_type::text) = 'FMS'::text;


drop view vw_att_details_handhole_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_handhole_vector
AS SELECT handhole.system_id,
    handhole.region_id,
    handhole.province_id,
    handhole.subarea_system_id AS subarea_id,
    point.sp_geometry,
    handhole.handhole_name,
    handhole.category,
    handhole.network_id,
    handhole.network_status,
   CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  handhole.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category,
    COALESCE(handhole.is_virtual, false) AS is_virtual
   FROM point_master point
     JOIN att_details_handhole handhole ON point.system_id = handhole.system_id AND point.entity_type::text = 'Handhole'::text;


drop view vw_att_details_htb_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_htb_vector
AS SELECT htb.system_id,
    htb.region_id,
    htb.province_id,
    htb.htb_name,
    htb.htb_type,
    htb.network_status,
    htb.type,
    htb.gis_design_id,
    htb.category,
   CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  htb.network_id
            ELSE point.display_name
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN isp_htb_info htb ON point.system_id = htb.system_id AND point.entity_type::text = 'HTB'::text;


drop view vw_att_details_manhole_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_manhole_vector
AS SELECT manhole.system_id,
    manhole.region_id,
    manhole.province_id,
    COALESCE(pm.system_id, 0) AS subarea_id,
    point.sp_geometry,
    manhole.manhole_name,
    manhole.category,
    manhole.network_id,
    manhole.network_status,
   CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  manhole.network_id
            ELSE point.display_name
        END AS display_name,
        CASE
            WHEN point.entity_category::text = ANY (ARRAY['Primary'::character varying::text, 'Secondary'::character varying::text]) THEN point.entity_category
            ELSE ''::character varying
        END AS entity_category,
    COALESCE(manhole.is_virtual, false) AS is_virtual
   FROM point_master point
     JOIN att_details_manhole manhole ON point.system_id = manhole.system_id AND point.entity_type::text = 'Manhole'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry);


drop view vw_att_details_network_ticket_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_network_ticket_vector
AS SELECT adn.ticket_id AS system_id,
    adn.name,
    COALESCE(plgn.sp_geometry, pb.sp_geometry) AS sp_geometry,
    adn.region_id,
    adn.province_id,
    adn.status,
    adn.network_id,
   CASE
            WHEN COALESCE(plgn.display_name, ''::character varying)::text = ''::text THEN  adn.network_id
            ELSE plgn.display_name
        END AS display_name
   FROM att_details_networktickets adn
     LEFT JOIN polygon_master plgn ON plgn.system_id = adn.ticket_id AND plgn.entity_type::text = 'Network_Ticket'::text
     LEFT JOIN province_boundary pb ON pb.id = adn.province_id;


drop view vw_att_details_ont_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_ont_vector
AS SELECT ont.system_id,
    ont.region_id,
    ont.province_id,
    ont.ont_name,
    ont.network_id,
    ont.network_status::text AS network_status,
    ont.network_id AS label_column,
    point.sp_geometry,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  ont.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_ont ont ON point.system_id = ont.system_id AND upper(point.entity_type::text) = 'ONT'::text;


drop view vw_att_details_patchpanel_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_patchpanel_vector
AS SELECT patchpanel.system_id,
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
    patchpanel.third_party_vendor_id,
    patchpanel.primary_pod_system_id,
    patchpanel.secondary_pod_system_id,
    patchpanel.source_ref_id,
    patchpanel.source_ref_type,
    patchpanel.network_id AS label_column,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  patchpanel.network_id
            ELSE point.display_name
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_patchpanel patchpanel ON point.system_id = patchpanel.system_id AND upper(point.entity_type::text) = 'PATCHPANEL'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = patchpanel.system_id AND upper(adi1.entity_type::text) = upper('patchpanel'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = patchpanel.system_id AND upper(adi.entity_type::text) = upper('PATCHPANEL'::text);


drop view vw_att_details_pod_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_pod_vector
AS SELECT pod.system_id,
    pod.region_id,
    pod.province_id,
    pod.pod_name,
    pod.network_id,
    pod.network_status::text AS network_status,
    pod.gis_design_id AS label_column,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  pod.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN att_details_pod pod ON point.system_id = pod.system_id AND point.entity_type::text = 'POD'::text;


drop view vw_att_details_pole_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_pole_vector
AS SELECT pole.system_id,
    pole.region_id,
    pole.province_id,
    point.sp_geometry,
    pole.pole_name,
    pole.pole_type,
    pole.network_id,
    pole.network_status,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  pole.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_pole pole ON point.system_id = pole.system_id AND point.entity_type::text = 'Pole'::text;


drop view vw_att_details_rack_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_rack_vector
AS SELECT rack.system_id,
    rack.region_id,
    rack.province_id,
    rack.rack_name,
    rack.rack_type,
    rack.category,
    rack.no_of_units,
    rack.structure_id,
    rack.status,
    rack.network_id,
    rack.network_status,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  rack.network_id
            ELSE point.display_name
        END AS display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN att_details_rack rack ON point.system_id = rack.system_id AND point.entity_type::text = 'Rack'::text;



drop view vw_att_details_spliceclosure_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_spliceclosure_vector
AS SELECT spliceclosure.system_id,
    spliceclosure.region_id,
    spliceclosure.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    spliceclosure.spliceclosure_name,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), spliceclosure.network_status::text) AS network_status,
    spliceclosure.network_id,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  spliceclosure.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category,
    COALESCE(spliceclosure.is_virtual, false) AS is_virtual
   FROM point_master point
     JOIN att_details_spliceclosure spliceclosure ON point.system_id = spliceclosure.system_id AND upper(point.entity_type::text) = 'SPLICECLOSURE'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = spliceclosure.system_id AND upper(adi1.entity_type::text) = upper('SPLICECLOSURE'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = spliceclosure.system_id AND upper(adi.entity_type::text) = upper('SPLICECLOSURE'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;


drop view vw_att_details_splitter_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_splitter_vector
AS SELECT splitter.system_id,
    splitter.region_id,
    splitter.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    splitter.splitter_name,
    splitter.splitter_type,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), splitter.network_status::text) AS network_status,
    splitter.network_id,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  splitter.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_splitter splitter ON point.system_id = splitter.system_id AND upper(point.entity_type::text) = 'SPLITTER'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = splitter.system_id AND upper(adi1.entity_type::text) = upper('SPLITTER'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = splitter.system_id AND upper(adi.entity_type::text) = upper('SPLITTER'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
  ORDER BY (
        CASE
            WHEN splitter.splitter_type::text = 'Primary'::text THEN 1
            ELSE 0
        END);


drop view vw_att_details_surveyarea_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_surveyarea_vector
AS SELECT surveyarea.system_id,
    surveyarea.region_id,
    surveyarea.province_id,
    surveyarea.surveyarea_name,
    surveyarea.network_id,
    surveyarea.status,
    COALESCE(adi.updated_geom, plgn.sp_geometry) AS sp_geometry,
    surveyarea.source_ref_id,
    surveyarea.source_ref_type,
    surveyarea.surveyarea_name AS label_column,
CASE
            WHEN COALESCE(plgn.display_name, ''::character varying)::text = ''::text THEN  surveyarea.network_id
            ELSE plgn.display_name
        END AS display_name,
    ''::text AS entity_category
   FROM polygon_master plgn
     JOIN att_details_surveyarea surveyarea ON plgn.system_id = surveyarea.system_id AND plgn.entity_type::text = 'SurveyArea'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = surveyarea.system_id AND upper(adi.entity_type::text) = upper('SurveyArea'::text);


drop view vw_att_details_tower_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_tower_vector
AS SELECT tower.system_id,
    tower.region_id,
    tower.province_id,
    COALESCE(pm.system_id, 0) AS subarea_id,
    point.sp_geometry,
    tower.network_name,
    tower.category,
    tower.network_id,
    tower.network_status,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  tower.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_tower tower ON point.system_id = tower.system_id AND point.entity_type::text = 'Tower'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry);


drop view vw_att_details_tree_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_tree_vector
AS SELECT tree.system_id,
    tree.region_id,
    tree.province_id,
    tree.tree_name,
    tree.network_id,
    tree.status,
    tree.category,
    tree.network_status::text AS network_status,
    tree.tree_name AS label_column,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  tree.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category,
    point.sp_geometry
   FROM point_master point
     JOIN att_details_tree tree ON point.system_id = tree.system_id AND point.entity_type::text = 'Tree'::text;


drop view vw_att_details_trench_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_trench_vector
AS SELECT trench.system_id,
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
CASE
            WHEN COALESCE(lm.display_name, ''::character varying)::text = ''::text THEN  trench.network_id
            ELSE lm.display_name
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


drop view vw_att_details_wallmount_vector ;

CREATE OR REPLACE VIEW public.vw_att_details_wallmount_vector
AS SELECT wallmount.system_id,
    wallmount.region_id,
    wallmount.province_id,
    point.sp_geometry,
    wallmount.wallmount_name,
    wallmount.network_id,
    wallmount.network_status,
CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN  wallmount.network_id
            ELSE point.display_name
        END AS display_name,
    point.entity_category
   FROM point_master point
     JOIN att_details_wallmount wallmount ON point.system_id = wallmount.system_id AND point.entity_type::text = 'WallMount'::text;