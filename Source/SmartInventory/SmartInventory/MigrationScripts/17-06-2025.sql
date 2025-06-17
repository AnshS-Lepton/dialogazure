
==========================================================================================================================
alter table audit_att_details_spliceclosure add action_date timestamp without time zone;

==========================================================================================================================

 DROP FUNCTION IF EXISTS public.fn_get_reportcolumn_list(character varying);

CREATE OR REPLACE FUNCTION public.fn_get_reportcolumn_list(
	p_layer_name character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
v_layerId int;
BEGIN

select layer_id into v_layerId from layer_details where lower(layer_name) = lower(p_layer_name);

RETURN QUERY select row_to_json(row) from (select s.display_name as value, s.column_name as key  from layer_columns_settings s 
										   where s.layer_id=v_layerId and upper(s.setting_type)='REPORT' and s.is_active=true and column_name in('network_id','fms_name','site_id') order by column_sequence) row;

END
$BODY$;

ALTER FUNCTION public.fn_get_reportcolumn_list(character varying)
    OWNER TO postgres;

===========================================================================================================================


 DROP VIEW public.vw_att_details_pod_report;

CREATE OR REPLACE VIEW public.vw_att_details_pod_report
 AS
 SELECT pod.system_id,
    (pod.site_id::text || '-'::text) || pod.site_name::text AS site_id_site_name,
    pod.network_id,
    pod.pod_name,
    round(pod.latitude::numeric, 6) AS latitude,
    round(pod.longitude::numeric, 6) AS longitude,
    pod.pincode,
    pod.address,
    pod.specification,
    pod.category,
    pod.subcategory1,
    pod.subcategory2,
    pod.subcategory3,
    pod.item_code,
        CASE
            WHEN pod.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN pod.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN pod.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, pod.status) AS status,
    pod.parent_network_id AS parent_code,
    pod.parent_entity_type AS parent_type,
    prov.province_name,
    reg.region_name,
    vm.name AS vendor_name,
    pm.project_code,
    pm.project_name,
    plningm.planning_code,
    plningm.planning_name,
    workorm.workorder_code,
    workorm.workorder_name,
    purposem.purpose_code,
    purposem.purpose_name,
    to_char(pod.created_on, 'DD-Mon-YY'::text) AS created_on,
    um.user_name AS created_by,
    to_char(pod.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pod.region_id,
    pod.province_id,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    pod.acquire_from,
    pod.pod_type,
    pod.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    pod.source_ref_type,
    pod.source_ref_id,
    pod.source_ref_description,
    pod.status_remark,
    pod.status_updated_by,
    to_char(pod.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    entattr.erp_code,
    entattr.erp_name,
    entattr.ef_customers,
    entattr.status AS locked,
    entattr.zone,
    to_char(entattr.rfs_date, 'DD-Mon-YY'::text) AS rfs_date,
    entattr.hub_maintained,
    entattr.hm_power_bb,
    entattr.hm_earthing_rating,
    entattr.hm_rack,
    entattr.hm_olt_bb,
    entattr.hm_fms,
    entattr.splicing_machine,
    entattr.optical_power_meter,
    entattr.otdr,
    entattr.laptop_with_giga_port,
    entattr.l3_updation_on_inms,
    pod.is_visible_on_map,
    pod.remarks,
    reg.country_name,
    pod.origin_from,
    pod.origin_ref_id,
    pod.origin_ref_code,
    pod.origin_ref_description,
    pod.request_ref_id,
    pod.requested_by,
    pod.request_approved_by,
    pod.subarea_id,
    pod.area_id,
    pod.dsa_id,
    pod.csa_id,
    pod.bom_sub_category,
    pod.gis_design_id AS design_id,
    pod.st_x,
    pod.st_y,
    pod.ne_id,
    pod.prms_id,
    pod.jc_id,
    pod.mzone_id,
    pod.served_by_ring,
    pod.hierarchy_type,
    vm3.name AS own_vendor_name,
    pod.site_id,
    pod.site_name,
    um3.user_name AS contactor_name,
    COALESCE(pod.vendorcost, 0.00::double precision) AS vendor_cost,
    pod.on_air_date,
    pod.removed_date,
    pod.tx_type,
    pod.tx_technology,
    pod.tx_segment,
    pod.tx_ring,
    pod.region,
    pod.province,
    pod.district,
    pod.region_address,
    pod.depot,
    pod.ds_division,
    pod.local_authority,
    pod.owner_name,
    pod.access_24_7,
    pod.tower_type,
    pod.tower_height,
    pod.cabinet_type,
    pod.solution_type,
    pod.site_rank,
    pod.self_tx_traffic,
    pod.agg_tx_traffic,
    pod.metro_ring_utilization,
    pod.csr_count,
    pod.dti_circuit,
    pod.agg_01,
    pod.agg_02,
    pod.bandwidth,
    pod.ring_type,
    pod.link_id,
    pod.alias_name,
    pod.tx_agg,
    pod.bh_status,
    pod.segment,
    pod.ring,
    pod.maximum_cost,
    pod.project_category,
    pod.priority,
    pod.no_of_cores,
    pod.fiber_link_type,
    pod.comment,
    pod.plan_cost,
    pod.fiber_distance,
    pod.fiber_link_code,
    pod.port_type,
    pod.destination_site_id,
    pod.destination_port_type,
    pod.destination_no_of_cores,
    pod.project_id_dialog,
    pod.vendorcost,
    pod.contracktorid,
    ''::text AS fiber_oh_distance_to_network,
    ''::text AS fiber_ug_distance_to_network,
    ''::text AS total_fiber_distance,
    ''::text AS fiber_distance_to_nearest_site,
    ''::text AS nearest_site,
    ''::text AS cost_based_on_rate_card_lkr,
    tr.ring_code,
        CASE
            WHEN pod.is_site_imported = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_site_imported
   FROM att_details_pod pod
     JOIN province_boundary prov ON pod.province_id = prov.id
     JOIN region_boundary reg ON pod.region_id = reg.id
     JOIN user_master um ON pod.created_by = um.user_id
     JOIN vendor_master vm ON pod.vendor_id = vm.id
     LEFT JOIN entity_additional_attributes entattr ON entattr.system_id = pod.system_id AND upper(entattr.entity_type::text) = upper('POD'::text)
     LEFT JOIN vendor_master vm2 ON pod.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pod.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON pod.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON pod.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON pod.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON pod.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON pod.modified_by = um2.user_id
     LEFT JOIN user_master um3 ON pod.contracktorid = um3.user_id
     LEFT JOIN entity_status_master es ON es.status::text = pod.status::text
     LEFT JOIN top_ring tr ON tr.id = pod.ring_id;

ALTER TABLE public.vw_att_details_pod_report
    OWNER TO postgres;


=====================================================================================================================

DROP FUNCTION IF EXISTS public.get_site_project_details(text);

CREATE OR REPLACE FUNCTION public.get_site_project_details(
	p_site_id text)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT row_to_json(t)
    FROM (
        SELECT 
		sp.id,sp.project_id, sp.site_id,sp.site_name,sp.site_owner,sp.project_category, sp.cable_plan_cores,sp.maximum_cost,
 sp.comment,sp.location_address,sp.ds_cmc_area,sp.priority,pod.destination_site_id,pod.destination_port_type,pod.no_of_cores
        FROM site_project_details sp left join vw_att_details_pod pod on pod.site_id=sp.site_id
        WHERE sp.site_id::TEXT = p_site_id
    ) t;
END;
$BODY$;

ALTER FUNCTION public.get_site_project_details(text)
    OWNER TO postgres;

========================================================================================================================



CREATE OR REPLACE FUNCTION public.update_site_project_detail(
	p_id integer,
	p_userid integer,
	p_site_name character varying,
	p_project_category character varying,
	p_cable_plan_cores character varying,
	p_comment character varying,
	p_site_owner character varying,
	p_maximum_cost integer,
	p_location_address character varying,
	p_ds_cmc_area character varying,
	p_destination_site_id character varying,
	p_destination_port_type character varying,
	p_no_of_cores integer,
	p_project_id character varying,
	p_site_id character varying,
	OUT v_result boolean,
	OUT v_message character varying)
    RETURNS record
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN

update site_project_details set site_name=p_site_name,project_category=p_project_category,
cable_plan_cores=p_cable_plan_cores,comment=p_comment,site_owner=p_site_owner,maximum_cost=p_maximum_cost,
location_address=p_location_address,ds_cmc_area=p_ds_cmc_area,
modified_by=p_userId, modified_on=now() where id=p_id;

    IF FOUND THEN
	update att_details_pod set destination_site_id=p_destination_site_id,
	destination_port_type=p_destination_port_type,
	no_of_cores=p_no_of_cores,
	project_id=p_id 
	where site_id=p_site_id;
    v_result := TRUE;
    v_message := 'Record updated successfully.';
ELSE
    v_result := FALSE;
    v_message := 'No matching record found to update.';
END IF;
END;
$BODY$;

ALTER FUNCTION public.update_site_project_detail(integer, integer, character varying, character varying, character varying, character varying, character varying, integer, character varying, character varying, character varying, character varying, integer, character varying, character varying)
    OWNER TO postgres;

===============================================================================================================================================

CREATE OR REPLACE FUNCTION public.fn_get_site_list(
	p_system_id integer DEFAULT NULL::integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

BEGIN
 RETURN QUERY
    SELECT row_to_json(r)
    FROM 
    (SELECT
        pod.fiber_oh_distance_to_network,
        pod.fiber_ug_distance_to_network,
        pod.total_fiber_distance,
        pod.fiber_distance_to_nearest_site,
        pod.nearest_site,
        pod.network_id,
        pm.system_id,
        ST_Y(pm.sp_geometry) || ' ' || ST_X(pm.sp_geometry) as sp_geometry
    FROM
        att_details_pod pod
        JOIN point_master pm ON pod.system_id = pm.system_id
    WHERE
        pm.entity_type = 'POD' 
       AND pod.fiber_oh_distance_to_network IS NULL
       AND pod.fiber_ug_distance_to_network IS NULL
       AND pod.total_fiber_distance IS NULL
       AND pod.nearest_site IS null AND (p_system_id IS NULL OR pod.system_id = p_system_id)) r;

END;
$BODY$;


=================================================================================================================================================
-- FUNCTION: public.fn_fiberlink_list_by_linkids(character varying)

-- DROP FUNCTION IF EXISTS public.fn_fiberlink_list_by_linkids(character varying);

CREATE OR REPLACE FUNCTION public.fn_fiberlink_list_by_linkids(
	v_linkids character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    v_fibergeom character varying;
BEGIN

-- Create a temporary table with ColorCode and GEOM columns
CREATE TEMP TABLE temp_fiberlink_details (
    system_id integer,
    network_id varchar,
    link_id varchar,
    link_name varchar,
    fiber_link_status varchar,
    link_type varchar,
    ColorCode varchar,
    GEOM TEXT
) ON COMMIT DROP;

-- Create a temporary table
CREATE TEMPORARY TABLE temp_fiber_geom (
    geom text,
    link_system_id int
) ON COMMIT DROP;

-- Insert data into the temporary table
INSERT INTO temp_fiber_geom (geom, link_system_id)
SELECT string_agg(pt.geom, ' | '), pt.link_system_id
FROM (
    SELECT regexp_replace(ST_AsText(sp_geometry), 'LINESTRING\(|\)', '', 'g') AS geom, link_system_id
    FROM (
        SELECT sp_geometry, link_system_id
        FROM (
            SELECT cable_id, link_system_id
            FROM att_details_cable_info
            WHERE link_system_id IN (
                SELECT system_id
                FROM att_details_fiber_link
                WHERE link_id::varchar = ANY(string_to_array(v_linkids, ',')::varchar[]) or main_link_id::varchar = ANY(string_to_array(v_linkids, ',')::varchar[])
            )
            GROUP BY cable_id, link_system_id
        ) a
        INNER JOIN line_master lm ON lm.system_id::varchar = a.cable_id::varchar AND lm.entity_type = 'Cable'
    ) AS subquery
) AS pt
GROUP BY pt.link_system_id;


--raise info'v_fibergeom%',v_fibergeom;

-- Insert data with ColorCode cycling from cable_color_master
INSERT INTO temp_fiberlink_details
SELECT 
    adf.system_id, adf.network_id, adf.link_id, adf.link_name, adf.fiber_link_status, adf.link_type,
    cc.color_code AS ColorCode,
    adf.geom AS GEOM
FROM (
    SELECT 
        system_id, network_id, link_id, link_name, fiber_link_status, link_type,tfg.geom,
        ROW_NUMBER() OVER (ORDER BY link_id) AS row_num
    FROM att_details_fiber_link f
	INNER JOIN temp_fiber_geom tfg on tfg.link_system_id=f.system_id
    WHERE link_id::varchar = ANY(string_to_array(v_linkids, ',')::varchar[]) or main_link_id::varchar = ANY(string_to_array(v_linkids, ',')::varchar[])
) adf
JOIN (
    SELECT color_code, ROW_NUMBER() OVER () - 1 AS rn
    FROM cable_color_master where type='Core'
) cc
ON adf.row_num % (SELECT COUNT(*) FROM cable_color_master) = cc.rn::integer;

RETURN QUERY
SELECT json_build_object(
    'FiberLinkDetails', (
        SELECT json_agg(
            json_build_object(
                'type', 'Feature',
                'geometry', GEOM,
                'properties', json_build_object(
                    'system_id', system_id,
                    'network_id', network_id,
                    'link_id', link_id,
                    'link_name', link_name,
                    'fiber_link_status', fiber_link_status,
                    'link_type', link_type,
                    'ColorCode', ColorCode
                )
            )
        )
        FROM temp_fiberlink_details
    )
) AS geojson;

END;
$BODY$;

ALTER FUNCTION public.fn_fiberlink_list_by_linkids(character varying)
    OWNER TO postgres;

=========================================================================================================

-- FUNCTION: public.fn_validate_linkids(character varying)

-- DROP FUNCTION IF EXISTS public.fn_validate_linkids(character varying);

CREATE OR REPLACE FUNCTION public.fn_validate_linkids(
	p_link_ids character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    sql TEXT;
BEGIN
    -- Construct the dynamic SQL query
    sql := 'WITH temp_ids AS (
        SELECT unnest(string_to_array(''' || p_link_ids || ''', '','')) AS link_id
    )
    SELECT string_agg(temp_ids.link_id, '','') AS invalidLinkIds  -- Aggregate invalid ids into a comma-separated list
    FROM temp_ids
    LEFT JOIN att_details_fiber_link ON (temp_ids.link_id = att_details_fiber_link.link_id or temp_ids.link_id = att_details_fiber_link.main_link_id)
    WHERE att_details_fiber_link.link_id IS NULL';
    RAISE INFO 'Executing SQL: %', sql;
    RETURN QUERY EXECUTE 'SELECT row_to_json(row) FROM (' || sql || ') row';
END;
$BODY$;

ALTER FUNCTION public.fn_validate_linkids(character varying)
    OWNER TO postgres;

=========================================================================================================

