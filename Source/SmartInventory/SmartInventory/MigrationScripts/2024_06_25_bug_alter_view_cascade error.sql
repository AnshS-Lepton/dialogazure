alter table att_details_cable add column hierarchy_type character varying(1000); 
alter table att_details_manhole add column hierarchy_type character varying(1000); 

 DROP MATERIALIZED VIEW IF EXISTS "APP_LCO"."LCO_REPORT";

CREATE MATERIALIZED VIEW IF NOT EXISTS "APP_LCO"."LCO_REPORT"
TABLESPACE pg_default
AS
 WITH fsa AS (
         SELECT jc.zonename,
            jc.jiostatecode,
            jc.jiostatename,
            jc.maintenancezonecode,
            jc.maintenancezonename,
            jc.sap_id AS jc_sap_id,
            jc.jiocentername,
            fsa_1.system_id AS fsa_system_id,
            fsa_1.subarea_name AS fsa_code,
            fsa_1.network_id AS fsa_network_id,
            fsa_1.vendor_id,
            fsa_1.prms_id,
            fsa_1.no_of_home_pass AS fsa_no_of_home_pass
           FROM att_details_subarea fsa_1,
            province_boundary pb,
            rjio_jc_boundary jc
          WHERE fsa_1.province_id = pb.id AND pb.province_abbreviation::text = jc.sapplantcode::text
        ), dsa AS (
         SELECT fsa_1.fsa_system_id,
            count(*) AS dsa_count,
            sum(dsa_1.no_of_home_pass) AS dsa_no_of_home_pass
           FROM att_details_dsa dsa_1,
            fsa fsa_1
          WHERE dsa_1.parent_system_id = fsa_1.fsa_system_id
          GROUP BY fsa_1.fsa_system_id
        ), csa AS (
         SELECT fsa_1.fsa_system_id,
            count(*) AS csa_count,
            sum(csa_1.no_of_home_pass) AS csa_no_of_home_pass
           FROM att_details_csa csa_1,
            att_details_dsa dsa_1,
            fsa fsa_1
          WHERE csa_1.parent_system_id = dsa_1.system_id AND dsa_1.parent_system_id = fsa_1.fsa_system_id
          GROUP BY fsa_1.fsa_system_id
        ), fat AS (
         SELECT fat_1.subarea_system_id,
            count(*) AS fat_count
           FROM isp_fdb_info fat_1,
            fsa fsa_1
          WHERE fat_1.subarea_system_id = fsa_1.fsa_system_id
          GROUP BY fat_1.subarea_system_id
        ), fat_status AS (
         SELECT fat_1.subarea_system_id,
            count(
                CASE
                    WHEN fat_1.network_status::text = 'P'::text THEN 1
                    ELSE NULL::integer
                END) AS "FAT_PLANNED_COUNT",
            count(
                CASE
                    WHEN fat_1.network_status::text = 'A'::text THEN 1
                    ELSE NULL::integer
                END) AS "FAT_ASBUILT_COUNT"
           FROM isp_fdb_info fat_1,
            fsa fsa_1
          WHERE fat_1.subarea_system_id = fsa_1.fsa_system_id
          GROUP BY fat_1.subarea_system_id, fat_1.network_status
        ), fdc AS (
         SELECT fdc_1.subarea_system_id,
            count(*) AS fdc_count
           FROM att_details_bdb fdc_1,
            fsa fsa_1
          WHERE fdc_1.subarea_system_id = fsa_1.fsa_system_id
          GROUP BY fdc_1.subarea_system_id
        ), fdc_status AS (
         SELECT fdc_1.subarea_system_id,
            count(
                CASE
                    WHEN fdc_1.network_status::text = 'P'::text THEN 1
                    ELSE NULL::integer
                END) AS "FDC_PLANNED_COUNT",
            count(
                CASE
                    WHEN fdc_1.network_status::text = 'A'::text THEN 1
                    ELSE NULL::integer
                END) AS "FDC_ASBUILT_COUNT"
           FROM att_details_bdb fdc_1,
            fsa fsa_1
          WHERE fdc_1.subarea_system_id = fsa_1.fsa_system_id
          GROUP BY fdc_1.subarea_system_id, fdc_1.network_status
        ), s1 AS (
         SELECT s1_1.subarea_system_id,
            count(*) AS s1_count
           FROM att_details_splitter s1_1,
            fsa fsa_1
          WHERE s1_1.subarea_system_id = fsa_1.fsa_system_id AND s1_1.splitter_ratio::text = '2:8'::text
          GROUP BY s1_1.subarea_system_id
        ), s1_status AS (
         SELECT s1_1.subarea_system_id,
            count(
                CASE
                    WHEN s1_1.network_status::text = 'P'::text THEN 1
                    ELSE NULL::integer
                END) AS "S1_PLANNED_COUNT",
            count(
                CASE
                    WHEN s1_1.network_status::text = 'A'::text THEN 1
                    ELSE NULL::integer
                END) AS "S1_ASBUILT_COUNT"
           FROM att_details_splitter s1_1,
            fsa fsa_1
          WHERE s1_1.subarea_system_id = fsa_1.fsa_system_id AND s1_1.splitter_ratio::text = '2:8'::text
          GROUP BY s1_1.subarea_system_id, s1_1.network_status
        ), s2 AS (
         SELECT s1_1.subarea_system_id,
            count(*) AS s2_count
           FROM att_details_splitter s1_1,
            fsa fsa_1
          WHERE s1_1.subarea_system_id = fsa_1.fsa_system_id AND s1_1.splitter_ratio::text = '1:8'::text
          GROUP BY s1_1.subarea_system_id
        ), s2_status AS (
         SELECT s1_1.subarea_system_id,
            count(
                CASE
                    WHEN s1_1.network_status::text = 'P'::text THEN 1
                    ELSE NULL::integer
                END) AS "S2_PLANNED_COUNT",
            count(
                CASE
                    WHEN s1_1.network_status::text = 'A'::text THEN 1
                    ELSE NULL::integer
                END) AS "S2_ASBUILT_COUNT"
           FROM att_details_splitter s1_1,
            fsa fsa_1
          WHERE s1_1.subarea_system_id = fsa_1.fsa_system_id AND s1_1.splitter_ratio::text = '1:8'::text
          GROUP BY s1_1.subarea_system_id, s1_1.network_status
        ), splice AS (
         SELECT s1_1.subarea_system_id,
            count(1) AS splice_count,
            count(
                CASE
                    WHEN s1_1.network_status::text = 'P'::text THEN 1
                    ELSE NULL::integer
                END) AS "SPLICE_PLANNED_COUNT",
            count(
                CASE
                    WHEN s1_1.network_status::text = 'A'::text THEN 1
                    ELSE NULL::integer
                END) AS "SPLICE_ASBUILT_COUNT"
           FROM att_details_spliceclosure s1_1,
            fsa fsa_1
          WHERE s1_1.subarea_system_id = fsa_1.fsa_system_id
          GROUP BY s1_1.subarea_system_id
        ), feeder AS (
         SELECT c.subarea_id,
            c.cable_category AS feeder,
            sum(
                CASE
                    WHEN c.total_core = 48 THEN c.cable_measured_length
                    ELSE NULL::numeric
                END) AS "FEEDER_48_MEASURED_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 48 AND c.network_status::text = 'P'::text THEN c.cable_measured_length
                    ELSE NULL::numeric
                END) AS "FEEDER_48_MEASURED_PLANNED_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 48 AND c.network_status::text = 'A'::text THEN c.cable_measured_length
                    ELSE NULL::numeric
                END) AS "FEEDER_48_MEASURED_ASBUILT_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 48 THEN c.cable_calculated_length
                    ELSE NULL::numeric
                END) AS "FEEDER_48_CALCULATED_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 48 AND c.network_status::text = 'P'::text THEN c.cable_calculated_length
                    ELSE NULL::numeric
                END) AS "FEEDER_48_CALCULATED_PLANNED_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 48 AND c.network_status::text = 'A'::text THEN c.cable_calculated_length
                    ELSE NULL::numeric
                END) AS "FEEDER_48_CALCULATED_ASBUILT_LENGTH"
           FROM vw_att_details_cable c
          WHERE c.cable_category::text = 'Feeder'::text
          GROUP BY c.subarea_id, c.cable_category
        ), distribution AS (
         SELECT c.subarea_id,
            c.cable_category AS distribution,
            sum(
                CASE
                    WHEN c.total_core = 12 THEN c.cable_measured_length
                    ELSE NULL::numeric
                END) AS "DISTRIBUTION_12_MEASURED_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 12 AND c.network_status::text = 'P'::text THEN c.cable_measured_length
                    ELSE NULL::numeric
                END) AS "DISTRIBUTION_12_MEASURED_PLANNED_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 12 AND c.network_status::text = 'A'::text THEN c.cable_measured_length
                    ELSE NULL::numeric
                END) AS "DISTRIBUTION_12_MEASURED_ASBUILT_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 12 THEN c.cable_calculated_length
                    ELSE NULL::numeric
                END) AS "DISTRIBUTION_12_CALCULATED_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 12 AND c.network_status::text = 'P'::text THEN c.cable_calculated_length
                    ELSE NULL::numeric
                END) AS "DISTRIBUTION_12_CALCULATED_PLANNED_LENGTH",
            sum(
                CASE
                    WHEN c.total_core = 12 AND c.network_status::text = 'A'::text THEN c.cable_calculated_length
                    ELSE NULL::numeric
                END) AS "DISTRIBUTION_12_CALCULATED_ASBUILT_LENGTH"
           FROM vw_att_details_cable c
          WHERE c.cable_category::text = 'Distribution'::text
          GROUP BY c.subarea_id, c.cable_category
        ), poles AS (
         SELECT att_details_pole.subarea_id,
            sum(
                CASE
                    WHEN upper(att_details_pole.bom_sub_category::text) = 'EXISTING'::text THEN 1
                    ELSE NULL::integer
                END) AS "EXISTING_POLE_COUNT",
            sum(
                CASE
                    WHEN upper(att_details_pole.bom_sub_category::text) = 'PROPOSED'::text THEN 1
                    ELSE NULL::integer
                END) AS "PROPOSED_POLE_COUNT",
            sum(
                CASE
                    WHEN upper(att_details_pole.bom_sub_category::text) = 'PROPOSED'::text AND att_details_pole.network_status::text = 'A'::text THEN 1
                    ELSE NULL::integer
                END) AS "ASBUILT_POLE_COUNT"
           FROM att_details_pole
          GROUP BY att_details_pole.subarea_id, att_details_pole.bom_sub_category
        )
 SELECT DISTINCT fsa.zonename,
    fsa.jiostatecode,
    fsa.jiostatename,
    fsa.maintenancezonecode,
    fsa.maintenancezonename,
    fsa.jc_sap_id,
    fsa.jiocentername,
    fsa.fsa_system_id,
    fsa.fsa_code,
    fsa.fsa_network_id,
    fsa.vendor_id,
    fsa.prms_id,
    fsa.fsa_no_of_home_pass,
    dsa.dsa_count,
    dsa.dsa_no_of_home_pass,
    csa.csa_count,
    csa.csa_no_of_home_pass,
    fat.fat_count,
    fat_status."FAT_PLANNED_COUNT",
    fat_status."FAT_ASBUILT_COUNT",
    fdc.fdc_count,
    fdc_status."FDC_PLANNED_COUNT",
    fdc_status."FDC_ASBUILT_COUNT",
    s1.s1_count,
    s1_status."S1_PLANNED_COUNT",
    s1_status."S1_ASBUILT_COUNT",
    s2.s2_count,
    s2_status."S2_PLANNED_COUNT",
    s2_status."S2_ASBUILT_COUNT",
    splice.splice_count,
    splice."SPLICE_PLANNED_COUNT",
    splice."SPLICE_ASBUILT_COUNT",
    feeder."FEEDER_48_MEASURED_LENGTH",
    feeder."FEEDER_48_MEASURED_PLANNED_LENGTH",
    feeder."FEEDER_48_MEASURED_ASBUILT_LENGTH",
    feeder."FEEDER_48_CALCULATED_LENGTH",
    feeder."FEEDER_48_CALCULATED_PLANNED_LENGTH",
    feeder."FEEDER_48_CALCULATED_ASBUILT_LENGTH",
    distribution."DISTRIBUTION_12_MEASURED_LENGTH",
    distribution."DISTRIBUTION_12_MEASURED_PLANNED_LENGTH",
    distribution."DISTRIBUTION_12_MEASURED_ASBUILT_LENGTH",
    distribution."DISTRIBUTION_12_CALCULATED_LENGTH",
    distribution."DISTRIBUTION_12_CALCULATED_PLANNED_LENGTH",
    distribution."DISTRIBUTION_12_CALCULATED_ASBUILT_LENGTH",
    poles."EXISTING_POLE_COUNT",
    poles."PROPOSED_POLE_COUNT",
    poles."ASBUILT_POLE_COUNT"
   FROM fsa
     LEFT JOIN dsa ON fsa.fsa_system_id = dsa.fsa_system_id
     LEFT JOIN csa ON fsa.fsa_system_id = csa.fsa_system_id
     LEFT JOIN fat ON fsa.fsa_system_id = fat.subarea_system_id
     LEFT JOIN fat_status ON fsa.fsa_system_id = fat_status.subarea_system_id
     LEFT JOIN fdc ON fsa.fsa_system_id = fdc.subarea_system_id
     LEFT JOIN fdc_status ON fsa.fsa_system_id = fdc_status.subarea_system_id
     LEFT JOIN s1 ON fsa.fsa_system_id = s1.subarea_system_id
     LEFT JOIN s2 ON fsa.fsa_system_id = s2.subarea_system_id
     LEFT JOIN s1_status ON fsa.fsa_system_id = s1_status.subarea_system_id
     LEFT JOIN s2_status ON fsa.fsa_system_id = s2_status.subarea_system_id
     LEFT JOIN splice ON fsa.fsa_system_id = splice.subarea_system_id
     LEFT JOIN feeder ON fsa.fsa_network_id::text = feeder.subarea_id::text
     LEFT JOIN distribution ON fsa.fsa_network_id::text = distribution.subarea_id::text
     LEFT JOIN poles ON fsa.fsa_network_id::text = poles.subarea_id::text
WITH NO DATA;

ALTER TABLE IF EXISTS "APP_LCO"."LCO_REPORT"
    OWNER TO postgres;


CREATE INDEX lco_report_jc_sap_id_idx
    ON "APP_LCO"."LCO_REPORT" USING btree
    (jc_sap_id COLLATE pg_catalog."default")
    TABLESPACE pg_default;
CREATE INDEX lco_report_jiostatecode_idx
    ON "APP_LCO"."LCO_REPORT" USING btree
    (jiostatecode COLLATE pg_catalog."default")
    TABLESPACE pg_default;
	
	
	
DROP VIEW public.vw_att_details_cable;
CREATE OR REPLACE VIEW public.vw_att_details_cable
             AS
             SELECT cable.system_id,
                cable.network_id,
                cable.cable_name,
                fn_get_display_name(cable.a_system_id, cable.a_entity_type) AS a_location,
                fn_get_display_name(cable.b_system_id, cable.b_entity_type) AS b_location,
                cable.total_core,
                cable.no_of_tube,
                cable.no_of_core_per_tube,
                round(cable.cable_measured_length::numeric, 2) AS cable_measured_length,
                round(cable.cable_calculated_length::numeric, 2) AS cable_calculated_length,
                cable.cable_type,
                cable.coreaccess,
                cable.wavelength,
                cable.optical_output_power,
                cable.frequency,
                cable.attenuation_db,
                cable.resistance_ohm,
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
                cable.network_status,
                cable.status,
                cable.pin_code,
                cable.province_id,
                cable.region_id,
                cable.utilization,
                cable.totalattenuationloss,
                cable.chromaticdb,
                cable.chromaticdispersion,
                cable.totalchromaticloss,
                cable.remarks,
                cable.route_id,
                cable.created_by,
                cable.created_on,
                cable.modified_by,
                cable.modified_on,
                cable.a_system_id,
                cable.a_entity_type,
                cable.b_system_id,
                cable.b_entity_type,
                cable.sequence_id,
                cable.duct_id,
                cable.trench_id,
                cable.project_id,
                cable.planning_id,
                cable.purpose_id,
                cable.workorder_id,
                cable.structure_id,
                cable.cable_category,
                cable.execution_method,
                prov.province_name,
                reg.region_name,
                um.user_name,
                vm.name AS vendor_name,
                tp.type AS cable_specification_type,
                bd.brand AS cable_brand,
                ml.model AS cable_model,
                cable.total_loop_count,
                cable.total_loop_length,
                fn_get_date(cable.created_on) AS created_date,
                fn_get_date(cable.modified_on) AS modified_date,
                um.user_name AS created_by_user,
                um2.user_name AS modified_by_user,
                cable.parent_system_id,
                cable.parent_entity_type,
                cable.parent_network_id,
                round(cable.start_reading::numeric, 2) AS start_reading,
                round(cable.end_reading::numeric, 2) AS end_reading,
                cable.cable_sub_category,
                cable.acquire_from,
                cable.circuit_id,
                cable.thirdparty_circuit_id,
                cable.drum_no,
                ( SELECT system_spec_master.prop_name
                       FROM system_spec_master
                      WHERE system_spec_master.id = cable.accessibility) AS cable_accessibility,
                ( SELECT system_spec_master.prop_name
                       FROM system_spec_master
                      WHERE system_spec_master.id = cable.construction) AS cable_construction,
                ( SELECT system_spec_master.prop_name
                       FROM system_spec_master
                      WHERE system_spec_master.id = cable.activation) AS cable_activation,
                cable.barcode,
                cable.ownership_type,
                vm2.id AS third_party_vendor_id,
                vm2.name AS third_party_vendor_name,
                cable.source_ref_type,
                cable.source_ref_id,
                cable.source_ref_description,
                cable.status_remark,
                cable.status_updated_by,
                cable.status_updated_on,
                cable.is_visible_on_map,
                cable.primary_pod_system_id,
                cable.secondary_pod_system_id,
                cable.is_new_entity,
                round(cable.inner_dimension::numeric, 2) AS inner_dimension,
                round(cable.outer_dimension::numeric, 2) AS outer_dimension,
                cable.calculated_length_remark,
                cable.audit_item_master_id,
                cable.is_acquire_from,
                cable.origin_from,
                cable.origin_ref_id,
                cable.origin_ref_code,
                cable.origin_ref_description,
                cable.request_ref_id,
                cable.requested_by,
                cable.request_approved_by,
                cable.subarea_id,
                cable.area_id,
                cable.dsa_id,
                cable.csa_id,
                cable.bom_sub_category,
                cable.gis_design_id,
                cable.ne_id,
                cable.prms_id,
                cable.jc_id,
                cable.mzone_id,
                cable.a_location_name,
                cable.b_location_name,
                cable.route_name,
                cable.hierarchy_type,
                cable.section_name,
                cable.aerial_location,
                cable.cable_remark,
                cable.generic_section_name,
                cable.own_vendor_id
               FROM att_details_cable cable
                 JOIN province_boundary prov ON cable.province_id = prov.id
                 JOIN region_boundary reg ON cable.region_id = reg.id
                 JOIN user_master um ON cable.created_by = um.user_id
                 JOIN vendor_master vm ON cable.vendor_id = vm.id
                 LEFT JOIN vendor_master vm2 ON cable.third_party_vendor_id = vm2.id
                 LEFT JOIN isp_type_master tp ON cable.type = tp.id
                 LEFT JOIN isp_brand_master bd ON cable.brand = bd.id
                 LEFT JOIN isp_base_model ml ON cable.model = ml.id
                 LEFT JOIN user_master um2 ON cable.modified_by = um2.user_id;
           
		   
		   
		   
DROP VIEW public.vw_att_details_pole_report;
 CREATE OR REPLACE VIEW public.vw_att_details_pole_report
             AS
             SELECT pole.system_id,
                pole.network_id,
                pole.pole_name,
                pole.pole_type,
                pole.pole_height,
                pole.pole_no,
                round(pole.latitude::numeric, 6) AS latitude,
                round(pole.longitude::numeric, 6) AS longitude,
                pole.address,
                pole.acquire_from,
                pole.specification,
                pole.category,
                pole.subcategory1,
                pole.subcategory2,
                pole.subcategory3,
                pole.item_code,
                    CASE
                        WHEN pole.is_used = true THEN 'Yes'::text
                        ELSE 'No'::text
                    END AS is_used,
                    CASE
                        WHEN pole.network_status::text = 'P'::text THEN 'Planned'::text
                        WHEN pole.network_status::text = 'A'::text THEN 'As Built'::text
                        WHEN pole.network_status::text = 'D'::text THEN 'Dormant'::text
                        ELSE NULL::text
                    END AS network_status,
                COALESCE(es.description, pole.status) AS status,
                prov.province_name,
                reg.region_name,
                pole.parent_network_id AS parent_code,
                pole.parent_entity_type AS parent_type,
                vm.name AS vendor_name,
                pm.project_code,
                pm.project_name,
                plningm.planning_code,
                plningm.planning_name,
                workorm.workorder_code,
                workorm.workorder_name,
                purposem.purpose_code,
                purposem.purpose_name,
                um.user_name AS created_by,
                to_char(pole.created_on, 'DD-Mon-YY'::text) AS created_on,
                to_char(pole.modified_on, 'DD-Mon-YY'::text) AS modified_on,
                um2.user_name AS modified_by,
                pole.region_id,
                pole.province_id,
                pole.barcode,
                pm.system_id AS project_id,
                plningm.system_id AS planning_id,
                workorm.system_id AS workorder_id,
                purposem.system_id AS purpose_id,
                um.user_id AS created_by_id,
                pole.ownership_type,
                vm2.name AS third_party_vendor_name,
                vm2.id AS third_party_vendor_id,
                pole.source_ref_type,
                pole.source_ref_id,
                pole.source_ref_description,
                pole.status_remark,
                pole.status_updated_by,
                to_char(pole.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
                pole.is_visible_on_map,
                primarypod.network_id AS primary_pod_network_id,
                secondarypod.network_id AS secondary_pod_network_id,
                primarypod.pod_name AS primary_pod_name,
                secondarypod.pod_name AS secondary_pod_name,
                pole.remarks,
                reg.country_name,
                pole.origin_from,
                pole.origin_ref_id,
                pole.origin_ref_code,
                pole.origin_ref_description,
                pole.request_ref_id,
                pole.requested_by,
                pole.request_approved_by,
                pole.subarea_id,
                pole.area_id,
                pole.dsa_id,
                pole.csa_id,
                pole.bom_sub_category,
                pole.gis_design_id AS design_id,
                pole.st_x,
                pole.st_y,
                pole.ne_id,
                pole.prms_id,
                pole.jc_id,
                pole.mzone_id,
                pole.served_by_ring,
                vm3.name as own_vendor_name
               FROM att_details_pole pole
                 JOIN province_boundary prov ON pole.province_id = prov.id
                 JOIN region_boundary reg ON pole.region_id = reg.id
                 JOIN user_master um ON pole.created_by = um.user_id
                 JOIN vendor_master vm ON pole.vendor_id = vm.id
                 LEFT JOIN vendor_master vm2 ON pole.third_party_vendor_id = vm2.id
                 LEFT JOIN vendor_master vm3 ON pole.own_vendor_id::integer = vm3.id
                 LEFT JOIN att_details_project_master pm ON pole.project_id = pm.system_id
                 LEFT JOIN att_details_planning_master plningm ON pole.planning_id = plningm.system_id
                 LEFT JOIN att_details_workorder_master workorm ON pole.workorder_id = workorm.system_id
                 LEFT JOIN att_details_purpose_master purposem ON pole.purpose_id = purposem.system_id
                 LEFT JOIN att_details_pod primarypod ON pole.primary_pod_system_id = primarypod.system_id
                 LEFT JOIN att_details_pod secondarypod ON pole.secondary_pod_system_id = secondarypod.system_id
                 LEFT JOIN user_master um2 ON pole.modified_by = um2.user_id
                 LEFT JOIN entity_status_master es ON es.status::text = pole.status::text;
				 
				 
				 
				 
				 
				 
				 
				 
				 
				 
				 DROP VIEW public.vw_att_details_bdb_report;
CREATE OR REPLACE VIEW public.vw_att_details_bdb_report
             AS
             SELECT bdb.system_id,
                bdb.network_id,
                bdb.bdb_name,
                round(bdb.latitude::numeric, 6) AS latitude,
                round(bdb.longitude::numeric, 6) AS longitude,
                bdb.pincode,
                bdb.address,
                bdb.specification,
                bdb.category,
                bdb.subcategory1,
                bdb.subcategory2,
                bdb.subcategory3,
                bdb.item_code,
                    CASE
                        WHEN bdb.is_servingdb THEN 'YES'::text
                        ELSE 'NO'::text
                    END AS is_servingdb,
                COALESCE(es.description, bdb.status) AS status,
                    CASE
                        WHEN bdb.network_status::text = 'P'::text THEN 'Planned'::text
                        WHEN bdb.network_status::text = 'A'::text THEN 'As Built'::text
                        WHEN bdb.network_status::text = 'D'::text THEN 'Dormant'::text
                        ELSE NULL::text
                    END AS network_status,
                bdb.parent_network_id AS parent_code,
                bdb.parent_entity_type AS parent_type,
                prov.province_name,
                reg.region_name,
                um.user_name,
                vm.name AS vendor_name,
                to_char(bdb.created_on, 'DD-Mon-YY'::text) AS created_on,
                to_char(bdb.modified_on, 'DD-Mon-YY'::text) AS modified_on,
                um.user_name AS created_by,
                um2.user_name AS modified_by,
                bdb.no_of_input_port,
                bdb.no_of_output_port,
                bdb.no_of_port,
                bdb.entity_category,
                pm.project_code,
                pm.project_name,
                plningm.planning_code,
                plningm.planning_name,
                workorm.workorder_code,
                workorm.workorder_name,
                purposem.purpose_code,
                purposem.purpose_name,
                bdb.region_id,
                bdb.province_id,
                bdb.barcode,
                pm.system_id AS project_id,
                plningm.system_id AS planning_id,
                workorm.system_id AS workorder_id,
                purposem.system_id AS purpose_id,
                um.user_id AS created_by_id,
                bdb.utilization,
                    CASE
                        WHEN bdb.utilization::text = 'L'::text THEN 'Low'::text
                        WHEN bdb.utilization::text = 'M'::text THEN 'Moderate'::text
                        WHEN bdb.utilization::text = 'H'::text THEN 'High'::text
                        WHEN bdb.utilization::text = 'O'::text THEN 'Over'::text
                        ELSE NULL::text
                    END AS utilization_text,
                    CASE
                        WHEN entnotifystatus.status IS NULL OR entnotifystatus.status THEN 'Un-blocked'::text
                        ELSE 'Blocked'::text
                    END AS notification_status,
                bdb.acquire_from,
                bdb.ownership_type,
                vm2.name AS third_party_vendor_name,
                vm2.id AS third_party_vendor_id,
                bdb.source_ref_type,
                bdb.source_ref_id,
                bdb.source_ref_description,
                bdb.status_remark,
                bdb.status_updated_by,
                to_char(bdb.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
                bdb.is_visible_on_map,
                primarypod.network_id AS primary_pod_network_id,
                secondarypod.network_id AS secondary_pod_network_id,
                primarypod.pod_name AS primary_pod_name,
                secondarypod.pod_name AS secondary_pod_name,
                bdb.remarks,
                reg.country_name,
                bdb.origin_from,
                bdb.origin_ref_id,
                bdb.origin_ref_code,
                bdb.origin_ref_description,
                bdb.request_ref_id,
                bdb.requested_by,
                bdb.request_approved_by,
                bdb.subarea_id,
                bdb.area_id,
                bdb.dsa_id,
                bdb.csa_id,
                bdb.bom_sub_category,
                bdb.gis_design_id AS design_id,
                bdb.st_x,
                bdb.st_y,
                bdb.ne_id,
                bdb.prms_id,
                bdb.jc_id,
                bdb.mzone_id,
                bdb.served_by_ring,
                vm3.name as own_vendor_name
               FROM att_details_bdb bdb
                 JOIN province_boundary prov ON bdb.province_id = prov.id
                 JOIN region_boundary reg ON bdb.region_id = reg.id
                 JOIN user_master um ON bdb.created_by = um.user_id
                 LEFT JOIN user_master um2 ON bdb.modified_by = um2.user_id
                 JOIN vendor_master vm ON bdb.vendor_id = vm.id
                 LEFT JOIN vendor_master vm3 ON bdb.own_vendor_id::integer = vm3.id
                 LEFT JOIN vendor_master vm2 ON bdb.third_party_vendor_id = vm2.id
                 LEFT JOIN att_details_project_master pm ON bdb.project_id = pm.system_id
                 LEFT JOIN att_details_planning_master plningm ON bdb.planning_id = plningm.system_id
                 LEFT JOIN att_details_workorder_master workorm ON bdb.workorder_id = workorm.system_id
                 LEFT JOIN att_details_purpose_master purposem ON bdb.purpose_id = purposem.system_id
                 LEFT JOIN entity_notification_status entnotifystatus ON bdb.notification_status_id = entnotifystatus.id
                 LEFT JOIN att_details_pod primarypod ON bdb.primary_pod_system_id = primarypod.system_id
                 LEFT JOIN att_details_pod secondarypod ON bdb.secondary_pod_system_id = secondarypod.system_id
                 LEFT JOIN entity_status_master es ON es.status::text = bdb.status::text;

