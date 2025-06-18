
---------------------------------Add column--------------------------------------------

alter table att_details_pod add column fiber_oh_distance_to_network double precision null;
alter table att_details_pod add column fiber_ug_distance_to_network double precision null;
alter table att_details_pod add column total_fiber_distance double precision null;
alter table att_details_pod add column fiber_distance_to_nearest_site double precision null;
alter table att_details_pod add column nearest_site character varying;


----------------------------------------Insert missing resource key------------------------------------

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_147', 'Ward', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL, now(), 1, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_148', 'Project Route Number', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL, now(), 1, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_149', 'No Of Duct', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL, now(), 1, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_150', 'Permission Type', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL, now(), 1, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_151', 'Authority', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL,now(), 1, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_152', 'Oh/Ug', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL, now(), 1, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_153', 'Date', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL, now(), 1, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_154', 'No Of Cable', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL, now(), 1, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_ROW_NET_FRM_155', 'Lepton Id', true, true, 'English', 'Smart Inventory_Osp_Row_Dot Net_', NULL, NULL, now(), 1, false, false);



------------------- Modify view POD report -------------------------------

DROP VIEW public.vw_att_details_pod_report;

CREATE OR REPLACE VIEW public.vw_att_details_pod_report
AS SELECT pod.system_id,
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
    pod.fiber_oh_distance_to_network ::text  AS fiber_oh_distance_to_network,
    pod.fiber_ug_distance_to_network ::text  AS fiber_ug_distance_to_network,
    pod.total_fiber_distance ::text  AS total_fiber_distance,
    pod.fiber_distance_to_nearest_site ::text  AS fiber_distance_to_nearest_site,
    pod.nearest_site ::text  AS nearest_site,
    ''::text AS cost_based_on_rate_card_lkr,
    tr.ring_code,
        CASE
            WHEN pod.is_site_imported = true THEN 'Import'::text
            ELSE 'Not Import'::text
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

-- Permissions

ALTER TABLE public.vw_att_details_pod_report OWNER TO postgres;
GRANT ALL ON TABLE public.vw_att_details_pod_report TO postgres;

--------------------------- Modify function to get nearest site records --------------------------------

DROP FUNCTION IF EXISTS public.fn_get_nearest_site_records(integer, character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_get_nearest_site_records(
	p_system_id integer,
	p_network_id character varying,
	v_buffer integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    v_geom geometry;
	V_Count integer;
begin
	
	DROP TABLE IF EXISTS temp_att_details_site;
	
	CREATE TEMP TABLE temp_att_details_site(
    system_id integer,
    network_id character varying,
    site_geometry character varying,
    distance double precision
     )on commit drop;
 
     -- Exit if buffer exceeded limit
    IF v_buffer > 500 THEN
        RETURN;
    END IF;
   
     -- Get geometry of the given POD
    SELECT sp_geometry 
    INTO v_geom 
    FROM point_master 
    WHERE system_id = p_system_id 
      AND entity_type = 'POD';
     
	insert into temp_att_details_site
     SELECT pm.system_id, pm.common_name as network_id,
     ST_Y(pm.sp_geometry) || ' ' || ST_X(pm.sp_geometry) AS site_geom,
     ST_DistanceSphere(pm.sp_geometry, v_geom) AS distance
     FROM point_master pm
        WHERE pm.entity_type = 'POD'
           AND pm.system_id not in (p_system_id) 
           AND ST_Within(pm.sp_geometry, ST_Buffer(v_geom::geography, v_buffer)::geometry)
        ORDER BY 
            distance
        LIMIT 5;
		
		SELECT count(1) into V_Count FROM temp_att_details_site;
		RAISE INFO 'temp_att_details_site------%',V_Count;
       
      IF NOT EXISTS (SELECT 1 FROM temp_att_details_site) THEN
        
       RETURN QUERY
	   
        SELECT * FROM fn_get_nearest_site_records(p_system_id, p_network_id, v_buffer + 100);
      
       ELSE
        -- Return result as JSON
        RETURN QUERY
        SELECT row_to_json(t)
        FROM temp_att_details_site t;
    END IF;
   
END;
$BODY$;

ALTER FUNCTION public.fn_get_nearest_site_records(integer, character varying, integer)
    OWNER TO postgres;

    ------------------------------------------- Modify function to get FMS connection report data --------------------------------

 DROP FUNCTION IF EXISTS public.fn_get_fms_connection_report(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer);

CREATE OR REPLACE FUNCTION public.fn_get_fms_connection_report(
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
V_AROW RECORD;
v_entity_port_no character varying;
v_maxcount integer;
v_maxpathid integer;
v_ststem_id integer;
v_entity_type integer;
v_maxfms_id integer;
sql TEXT;
StartSNo INTEGER; 
EndSNo INTEGER;
LowerStart character varying;
LowerEnd character varying;
TotalRecords integer; 
WhereCondition character varying;
s_report_view_name character varying;
s_geom_type character varying;
s_layer_id integer; 
s_layer_columns text;
TotalAppliedRecords integer; 
TotalRejectedRecords integer;
 r RECORD;
    ss_report_view_name TEXT;
    d_report_view_name TEXT;
    v_report_view_name TEXT;
    ss_layer_name TEXT;
    update_sql TEXT;
s_latitude text;
d_layer_name TEXT;
v_layer_name TEXT;
BEGIN
create temp table cpf_temp_result
(
id serial,
source_system_id integer,
source_network_id character varying(500),
source_port_no integer, 
source_entity_type character varying(100),
source_entity_title character varying(100),
destination_system_id integer,
destination_network_id character varying(500),
destination_port_no integer,
destination_entity_type character varying(100),
destination_entity_title character varying(100),
viya_system_id integer,
via_network_id character varying(500),
path_id integer,
via_entity_type character varying(100),
SOURCE_TUBE_COLOUR_CODE character varying(100),
DESTINATION_TUBE_COLOUR_CODE character varying(100),
SOURCE_PORT_COLOUR_CODE character varying(100),
DESTINATION_PORT_COLOUR_CODE character varying(100),
SOURCE_TUBE_NAME character varying(100),
DESTINATION_TUBE_NAME character varying(100),
fms_id integer,
splitter_id integer,
entity_type character varying(100),
client_id character varying,
client_link_id character varying,
client_core_id character varying,
otdr_length float(8) default (0)
)on commit drop;
-------------------------------------------------------------------------------------------------------------------
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER.. 
SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,res_field_key,DISPLAY_NAME FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
IF ((p_geom) != '') 
THEN
if(p_radius>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10) into p_geom;
end if;
RAISE INFO '------------------------------------S_LAYER_COLUMNS%', S_LAYER_COLUMNS;
sql:= 'SELECT system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join '||s_geom_type||'_master m
on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||'  and 1=1 ';
--inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
RAISE INFO '------------------------------------sql%', sql;
ELSE
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
--inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
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
if(substring(P_searchbytext from 1 for 1)='"' and substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
then
sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
else
sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
end if;
END IF;
IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND a.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';
END IF;
sql:= sql ||' )X where system_id in(select parent_system_id from isp_port_info where parent_entity_type=''FMS'' and port_status_id=2 )';
RAISE INFO 'rrr%', sql;
--------------------------------------------------------------------------------------------------------------------
FOR V_AROW IN
-- SELECT 'FMS' as entity_type,system_id from att_details_fms 
-- where system_id in(808,805,810)--province_id = ANY (string_to_array(p_province, ',')::integer[]) and region_id = ANY (string_to_array(p_region, ',')::integer[])
EXECUTE 'SELECT ''FMS'' as entity_type, system_id FROM ('||sql||')a'
loop
perform(fn_get_fms_connection(V_AROW.system_id,V_AROW.entity_type));
end loop;
--------------------------------------------------------------------------------------------------
delete from cpf_temp_result where source_network_id=destination_network_id;
if exists(select 1 from cpf_temp_result where source_entity_type='FMS')
then
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE source_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	-- Update query
	UPDATE cpf_temp_result 
	SET 
	destination_network_id = VIA_row.destination_network_id,
	destination_system_id = VIA_row.destination_system_id,
	destination_port_no = VIA_row.destination_port_no,
	destination_entity_type = VIA_row.destination_entity_type,
	destination_entity_title = VIA_row.destination_entity_title,
	viya_system_id = VIA_row.source_system_id,
	via_network_id = VIA_row.source_network_id,
	via_entity_type = VIA_row.source_entity_type

	FROM VIA_row
	WHERE 
	cpf_temp_result.destination_entity_type = VIA_row.source_entity_type
	AND cpf_temp_result.destination_system_id = VIA_row.source_system_id
	AND cpf_temp_result.destination_network_id = VIA_row.source_network_id
	AND cpf_temp_result.destination_PORT_NO = VIA_row.source_PORT_NO;
	-- Delete query
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result 
	WHERE source_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	DELETE FROM cpf_temp_result
	WHERE SOURCE_SYSTEM_ID IN (SELECT SOURCE_SYSTEM_ID FROM VIA_row);
else

	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE destination_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	-- Update query
	UPDATE cpf_temp_result 
	SET 
	source_network_id = VIA_row.source_network_id,
	source_system_id = VIA_row.source_system_id,
	source_port_no = VIA_row.source_port_no,
	source_entity_type = VIA_row.source_entity_type,
	source_entity_title = VIA_row.source_entity_title,
	viya_system_id = VIA_row.destination_system_id,
	via_network_id = VIA_row.destination_network_id,
	via_entity_type = VIA_row.destination_entity_type

	FROM VIA_row
	WHERE 
	cpf_temp_result.source_entity_type = VIA_row.source_entity_type
	AND cpf_temp_result.source_system_id = VIA_row.source_system_id
	AND cpf_temp_result.source_network_id = VIA_row.source_network_id
	AND cpf_temp_result.source_PORT_NO = VIA_row.source_PORT_NO;
	-- Delete query
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result 
	WHERE destination_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	DELETE FROM cpf_temp_result
	WHERE destination_SYSTEM_ID IN (SELECT destination_SYSTEM_ID FROM VIA_row);
 
end if;

--select count(*) into v_maxcount from cpf_temp_result group by path_id order by count(*) desc limit 1;--header
select count(*) into v_maxcount from cpf_temp_result group by fms_id,path_id order by count(*) desc limit 1;
SELECT COUNT(DISTINCT path_id) AS path_count
FROM cpf_temp_result group by fms_id order by COUNT(DISTINCT path_id) desc limit 1 
INTO v_maxpathid;---dataloop
SELECT COUNT(DISTINCT fms_id) AS fms_id_count from cpf_temp_result
INTO v_maxfms_id;
------------------------------------------------------------------------------------------------------------------------------------
UPDATE cpf_temp_result 
SET SOURCE_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
SOURCE_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
SOURCE_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code ,client_id =adfl.customer_id,client_link_id =adfl.main_link_id,client_core_id=adfl.link_id,otdr_length=adfl.otdr_distance
FROM ATT_DETAILS_CABLE_INFO left join att_details_fiber_link adfl on adfl.system_id =ATT_DETAILS_CABLE_INFO.link_system_id 
WHERE cpf_temp_result.SOURCE_ENTITY_type='Cable'
and cpf_temp_result.source_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.source_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;

--select * from ATT_DETAILS_CABLE_INFO limit 20 where cable_id=847

UPDATE cpf_temp_result 
SET DESTINATION_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
DESTINATION_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
DESTINATION_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code ,client_id =adfl.customer_id,client_link_id =adfl.main_link_id,client_core_id=adfl.link_id,otdr_length=adfl.otdr_distance
FROM ATT_DETAILS_CABLE_INFO left join att_details_fiber_link adfl on adfl.system_id =ATT_DETAILS_CABLE_INFO.link_system_id 
WHERE cpf_temp_result.DESTINATION_ENTITY_type='Cable'
and cpf_temp_result.DESTINATION_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.DESTINATION_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;

--       UPDATE cpf_temp_result 
--       SET 
--       splitter_id =destination_system_id
--       where destination_entity_type = 'Splitter' and 
--       destination_system_id not in(select source_system_id from cpf_temp_result where source_entity_type = 'Splitter') ; 
   
--       UPDATE cpf_temp_result 
--       SET 
--       entity_type ='Splitter'
--       where destination_entity_type = 'Splitter' ; 

       for  r in  select  * from cpf_temp_result loop

select report_view_name,layer_name into ss_report_view_name,ss_layer_name from layer_details where lower(layer_name) =lower(r.source_entity_type);
select report_view_name,layer_name into d_report_view_name,d_layer_name from layer_details where lower(layer_name) =lower(r.destination_entity_type);
if r.via_entity_type is not null then 
select report_view_name,layer_name into v_report_view_name,v_layer_name from layer_details where lower(layer_name) =lower(r.via_entity_type);
end if;
if lower(ss_layer_name) = 'patchcord'   then 
ss_layer_name ='patch_cord';
end if;
if  lower(d_layer_name) = 'patchcord'  then 
d_layer_name ='patch_cord';
end if;
if   lower(v_layer_name) = 'patchcord' then 
v_layer_name ='patch_cord';
end if ;

ss_layer_name:=ss_layer_name||'_name';
d_layer_name:= d_layer_name||'_name';
v_layer_name =v_layer_name||'_name';
s_latitude ='latitude';
raise info 'ss_report_view_name% ',ss_report_view_name;
raise info 'd_report_view_name% ',d_report_view_name;
raise info 'v_report_view_name% ',v_report_view_name;

if ss_report_view_name IS NOT NULL then
if lower(r.source_entity_type) != 'cable' then 

EXECUTE '
        UPDATE cpf_temp_result ctf
        SET source_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || ss_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(ss_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.source_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
else 

EXECUTE 
        'UPDATE cpf_temp_result ctf
        SET source_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || ss_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(ss_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.source_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';

end if;
end if;
 if d_report_view_name IS NOT NULL then 
if lower(r.destination_entity_type) != 'cable' then 
EXECUTE ' UPDATE cpf_temp_result ctf
        SET destination_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || d_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(d_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.destination_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || '; ';

else
EXECUTE 
        'UPDATE cpf_temp_result ctf
        SET destination_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || d_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(d_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.destination_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
end if;
end if;
if v_report_view_name IS NOT NULL then 
if lower(r.via_entity_type) != 'cable' then 
if r.via_entity_type is not null and r.via_entity_type !='' then 
raise info 'test12% ','test12';

EXECUTE '
        UPDATE cpf_temp_result ctf
        SET via_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || v_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(v_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.viya_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';

end if;

else

if r.via_entity_type is not null and r.via_entity_type !='' then 
   EXECUTE      'UPDATE cpf_temp_result ctf
        SET via_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || v_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(v_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.viya_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
end if;
end if;
end if;
end loop;
-------------------------------------------------------------------------------------------------
return query
select row_to_json(result) 
from (select (select array_to_json(array_agg(row_to_json(x)))from (select *,v_maxcount as headerCol,v_maxpathid as rowsdataloop,v_maxfms_id as globaLoopcount
from cpf_temp_result order by id) x) as lstConnectionInfo
)result ;
END; 
$BODY$;


-------------------------------------------Select query to sync layer columns--------------------------------

select * from  fn_sync_layer_columns() ;


