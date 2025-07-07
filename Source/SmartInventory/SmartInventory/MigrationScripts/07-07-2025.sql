
----------------------------------- Update columns value--------------------------------------

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
    pod.fiber_oh_distance_to_network::text AS fiber_oh_distance_to_network,
    pod.fiber_ug_distance_to_network::text AS fiber_ug_distance_to_network,
    pod.total_fiber_distance::text AS total_fiber_distance,
    pod.fiber_distance_to_nearest_site::text AS fiber_distance_to_nearest_site,
    pod.nearest_site::text AS nearest_site,
    pod.plan_cost AS cost_based_on_rate_card_lkr,
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

     --------------------------- Modify the report function --------------------------------------


     DROP FUNCTION IF EXISTS public.fn_sr_get_export_report_data(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer);

CREATE OR REPLACE FUNCTION public.fn_sr_get_export_report_data(
	p_networkstatues character varying,
	p_provinceids character varying,
	p_regionids character varying,
	p_layer_name character varying,
	p_searchby character varying,
	p_searchbytext character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_geom character varying,
	p_duration_based_column character varying,
	p_radius double precision,
	p_userid integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

   DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
   s_report_view_name character varying;
   s_geom_type character varying;
s_layer_id integer; 
s_layer_columns text;
TotalAppliedRecords integer; 
TotalRejectedRecords integer;
BEGIN

LowerStart:='';
LowerEnd:='';
WhereCondition:='';

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..	
-- SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS FROM (
-- SELECT  COLUMN_NAME,res_field_key,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

--SELECT (SPECIFIC) ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG(COLUMN_NAME ||' as "'|| CASE WHEN COALESCE(res_field_key, '') = '' THEN DISPLAY_NAME ELSE res_field_key END || '"', ',') INTO S_LAYER_COLUMNS 
FROM (SELECT COLUMN_NAME, res_field_key, DISPLAY_NAME FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID = S_LAYER_ID AND UPPER(SETTING_TYPE) = 'REPORT' AND IS_ACTIVE = TRUE
       AND COLUMN_NAME IN ('system_id','site_id','site_name','latitude','longitude',
                           'fiber_oh_distance_to_network','fiber_ug_distance_to_network','total_fiber_distance',
                           'fiber_distance_to_nearest_site','cost_based_on_rate_card_lkr','is_site_imported','nearest_site') ORDER BY COLUMN_SEQUENCE ) A;

-- DYNAMIC QUERY
IF ((p_geom) != '') 
THEN
	if(p_radius>0)
	then
		select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10)  into p_geom;
	end if;
RAISE INFO '------------------------------------S_LAYER_COLUMNS%', S_LAYER_COLUMNS;

	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id, '||S_LAYER_COLUMNS||' from (select 
     a.system_id,a.site_id,a.site_name,a.latitude,a.longitude, 
     a.fiber_oh_distance_to_network,ROUND(a.fiber_ug_distance_to_network::NUMERIC, 2) as fiber_ug_distance_to_network,ROUND(a.total_fiber_distance::NUMERIC, 2) as total_fiber_distance,
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported,(select site_id from att_details_pod pod where pod.network_id=a.nearest_site) as nearest_site
   from '||S_REPORT_VIEW_NAME||' a inner join '||s_geom_type||'_master m
	on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
	and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
	RAISE INFO '------------------------------------sql%', sql;
ELSE
     -- a.system_id, a.design_id,a.network_id,a.port_type

	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id, '||S_LAYER_COLUMNS||' from (select 
     a.system_id, a.site_id,a.site_name,a.latitude,a.longitude, 
     a.fiber_oh_distance_to_network,ROUND(a.fiber_ug_distance_to_network::NUMERIC, 2) as fiber_ug_distance_to_network,ROUND(a.total_fiber_distance::NUMERIC, 2) as total_fiber_distance,
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported,(select site_id from att_details_pod pod where pod.network_id=a.nearest_site) as nearest_site
    from '||S_REPORT_VIEW_NAME||' a 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
		RAISE INFO '------------------------------------sql1%', sql;
END IF;

IF ((p_networkStatues) != '' and upper(S_LAYER_COLUMNS) like '%NETWORK_STATUS%') THEN
	sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in('||p_networkStatues||')';
END IF;

IF ((p_RegionIds) != '') THEN
	sql:= sql ||' AND a.region_id IN ('||p_RegionIds||')';
END IF;

IF ((P_ProvinceIds) != '') THEN
	sql:= sql ||' AND a.province_id IN ('||P_ProvinceIds||')';
END IF;

IF ((P_searchbytext) != '' and (P_searchby) != '') THEN
--sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(''%'||trim(P_searchbytext)||'%'')';

	if(substring(P_searchbytext from 1 for 1)='"' and  substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
	then
		sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
	else
		sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
	end if;

END IF;

IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND a.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

sql:= sql ||' )X';
RAISE INFO '%', sql;
-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
 RAISE INFO 'page%',P_PAGENO;
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords,* FROM (' || sql || ') T';                  
 END IF; 

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$;

----------------------------------- Update plan cost value---------------------------------------

DROP FUNCTION IF EXISTS public.fn_update_site_bomboq_amount(integer, integer, double precision);

CREATE OR REPLACE FUNCTION public.fn_update_site_bomboq_amount(
	p_id integer,
	p_userid integer,
	p_amount double precision,
	OUT v_result boolean,
	OUT v_message character varying)
    RETURNS record
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN

update att_details_pod set plan_cost=p_amount where system_id=p_id;

    v_result := TRUE;
    v_message := 'Record updated successfully.';

END;
$BODY$;


----------------------------------- get bom boq details for sites---------------------------------------

DROP FUNCTION IF EXISTS public.fn_site_get_bom_boq(integer, double precision, double precision, integer);

CREATE OR REPLACE FUNCTION public.fn_site_get_bom_boq(
	p_site_id integer,
	p_pole_span double precision,
	p_manhole_span double precision,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    v_line_geom geometry;
    v_line_geom_length double precision;
    current_fraction double precision := 0.0;
    structure_location geometry;
    step_fraction double precision;
    line_geom geometry;
BEGIN
    -- Create temp table
    CREATE TEMP TABLE tempMPole (
        entity_type VARCHAR,
        longitude DOUBLE PRECISION,
        latitude DOUBLE PRECISION,
        sp_geometry GEOMETRY,
        line_sp_geometry GEOMETRY,
        fraction DOUBLE PRECISION
    ) ON COMMIT DROP;

    -- Get main line geometry
    SELECT nearest_site_geometry INTO v_line_geom
    FROM att_details_pod po
    WHERE po.system_id = p_site_id;

    -- Calculate total length of line
    SELECT ST_Length(v_line_geom, false) INTO v_line_geom_length;

    -- Walk along the line
    WHILE current_fraction < 1.0 LOOP
        structure_location := ST_LineInterpolatePoint(
            v_line_geom,
            LEAST(current_fraction + (p_pole_span / v_line_geom_length), 1.0)
        );

        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            step_fraction := p_pole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(
                v_line_geom,
                current_fraction,
                LEAST(current_fraction + step_fraction, 1.0)
            );

            INSERT INTO tempMPole (
                entity_type, longitude, latitude, sp_geometry, line_sp_geometry, fraction
            )
            VALUES (
                'Pole',
                ST_X(ST_EndPoint(line_geom)),
                ST_Y(ST_EndPoint(line_geom)),
                ST_EndPoint(line_geom),
                line_geom,
                current_fraction
            );

        ELSE
            line_geom := NULL;
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(
                v_line_geom,
                current_fraction,
                LEAST(current_fraction + step_fraction, 1.0)
            );

            INSERT INTO tempMPole (
                entity_type, longitude, latitude, sp_geometry, line_sp_geometry, fraction
            )
            VALUES (
                'Manhole',
                ST_X(ST_EndPoint(line_geom)),
                ST_Y(ST_EndPoint(line_geom)),
                ST_EndPoint(line_geom),
                line_geom,
                current_fraction
            );
        END IF;

        current_fraction := current_fraction + step_fraction;

        EXIT WHEN current_fraction >= 1.0;
    END LOOP;

    -- Return BOQ as JSON
    RETURN QUERY
    SELECT row_to_json(row_result)
    FROM (
        SELECT
            t1.entity_type,
            t1.qty::text AS qty,
            itm.cost_per_unit,
            itm.service_cost_per_unit,
            ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) AS total_cost
        FROM (
            SELECT entity_type, COUNT(*) AS qty
            FROM tempMPole
            WHERE entity_type = 'Pole'
            GROUP BY entity_type
        ) t1
        JOIN item_template_master itm
            ON itm.category_reference = t1.entity_type
        WHERE itm.specification = 'Generic'

        UNION ALL

        SELECT
            t1.entity_type,
            t1.qty::text,
            itm.cost_per_unit,
            itm.service_cost_per_unit,
            ((t1.qty * itm.cost_per_unit) + (t1.qty * itm.service_cost_per_unit)) AS total_cost
        FROM (
            SELECT entity_type, COUNT(*) AS qty
            FROM tempMPole
            WHERE entity_type = 'Manhole'
            GROUP BY entity_type
        ) t1
        JOIN item_template_master itm
            ON itm.category_reference = t1.entity_type
        WHERE itm.specification = 'Generic'
    ) AS row_result;

END;
$BODY$;


-------------------------------------------- Insert global key used in generate plan cost-----------------------------------------------------------------------

INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa)
VALUES('NearBySiteManholeSpan', '10', 'Distance between two manholes', 'Web', false, 'Integer', 1.0, 70.0, 1, now(), NULL, NULL, false, true, false);

INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa)
VALUES('NearBySitePoleSpan', '10', 'Distance between two poles', 'Web', false, 'Integer', 1.0, 70.0, 1, now(), NULL, NULL, false, true, false);


-------------------------------------------- Update column display name with required units-----------------------------------------------------------------------

update LAYER_COLUMNS_SETTINGS set display_name='Cost Based On Rate Card (Lkr)' where column_name='cost_based_on_rate_card_lkr' and  layer_id=(select layer_id from layer_details ld where layer_name='POD') and setting_type='Report';
update LAYER_COLUMNS_SETTINGS set display_name='Fiber Oh Distance To Network (m)' where column_name='fiber_oh_distance_to_network' and  layer_id=(select layer_id from layer_details ld where layer_name='POD') and setting_type='Report';
update LAYER_COLUMNS_SETTINGS set display_name='Fiber Ug Distance To Network (m)' where column_name='fiber_ug_distance_to_network' and  layer_id=(select layer_id from layer_details ld where layer_name='POD') and setting_type='Report';
update LAYER_COLUMNS_SETTINGS set display_name='Total Fiber Distance (m)' where column_name='total_fiber_distance' and  layer_id=(select layer_id from layer_details ld where layer_name='POD') and setting_type='Report';

update LAYER_COLUMNS_SETTINGS set display_name='Fiber Distance To Nearest Site (m)' where column_name='fiber_distance_to_nearest_site' and  layer_id=(select layer_id from layer_details ld where layer_name='POD') and setting_type='Report';