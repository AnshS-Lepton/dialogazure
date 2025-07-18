alter table att_details_pod add pole_count integer default 0;
alter table att_details_pod add manhole_count integer;
alter table att_details_pod add spliceclosure_count integer;
alter table att_details_pod add manhole_span integer;
alter table att_details_pod add pole_span integer;
alter table att_details_pod add cable_intersection_count integer;

alter table audit_att_details_pod add pole_count integer;
alter table audit_att_details_pod add manhole_count integer;
alter table audit_att_details_pod add spliceclosure_count integer;
alter table audit_att_details_pod add manhole_span integer;
alter table audit_att_details_pod add pole_span integer;
alter table audit_att_details_pod add cable_intersection_count integer;

INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key, is_utilization_column)
VALUES((select layer_id from layer_details where layer_name='POD'), 'Report', 'spliceclosure_count', 138423, 'SpliceClosure Count', 'vw_att_details_pod_report', true, false, 1, now(), NULL, NULL, false, true, NULL,  false);
	  
INSERT INTO public.global_settings(
	key, value, description, type, is_edit_allowed, data_type, min_value, max_value, created_by, created_on,  is_mobile_key, is_web_key, is_edit_allowed_for_sa)
	VALUES ('SiteBuffer', '10', 'To calculate SiteBuffer', 'Web', true, 'int', 1, 1, 1, now(), false, true, false);	  

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
        END AS is_site_imported,
    pod.manhole_count,
    pod.pole_count,
    pod.spliceclosure_count
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


CREATE OR REPLACE FUNCTION public.fn_trg_audit_att_details_pod()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$

DECLARE
_value integer;
 _ignorecol character varying;  
 _ignorecol1 character varying;  
tempjson text; --for additional attributes
sql text;  --for additional attributes
BEGIN

IF (TG_OP = 'INSERT' ) THEN  
        INSERT INTO public.audit_att_details_pod(
            system_id, network_id, pod_name, latitude, longitude, 
            province_id, region_id, pincode, address, specification, category, 
            subcategory1, subcategory2, subcategory3, item_code, vendor_id, 
            type, brand, model, construction, activation, accessibility, 
            status, created_by, created_on, modified_by, modified_on, network_status, 
            parent_system_id, parent_network_id, parent_entity_type, sequence_id, 
            structure_id, shaft_id, floor_id, project_id, planning_id, purpose_id, 
            workorder_id, action,acquire_from,pod_type,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,remarks,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id) 
         select   new.system_id, new.network_id, new.pod_name, new.latitude, new.longitude, 
            new.province_id, new.region_id, new.pincode, new.address, new.specification, new.category, 
            new.subcategory1, new.subcategory2, new.subcategory3, new.item_code, new.vendor_id, 
            new.type, new.brand, new.model, new.construction, new.activation, new.accessibility, 
            new.status, new.created_by, new.created_on, new.modified_by, new.modified_on, new.network_status, 
            new.parent_system_id, new.parent_network_id, new.parent_entity_type, new.sequence_id, 
            new.structure_id, new.shaft_id, new.floor_id, new.project_id, new.planning_id, new.purpose_id, 
            new.workorder_id, 'I' as action,new.acquire_from,new.pod_type,new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.remarks,new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category,new.gis_design_id from att_details_pod where system_id=new.system_id;

           
         --raise info 'new.system_id %',new.system_id; 
          tempjson:='''"'||new.system_id||'"''';
         --raise info 'tempjson %',tempjson; 
   
	    --Update value of the key 'record_system_id' for additional attributes    
   		sql:='UPDATE att_details_pod SET other_info = jsonb_set(other_info::jsonb , ''{record_system_id}'','||tempjson||') where system_id = '||new.system_id||';';
   
  		 --raise info 'sql %',sql; 
	  	execute sql;
           
END IF;
IF (TG_OP = 'UPDATE' ) THEN  
_ignorecol := 'modified_on,gis_design_id,codification_sequence,area_system_id,area_id,subarea_system_id,subarea_id,dsa_system_id,dsa_id,csa_system_id,csa_id';
_ignorecol1 := 'modified_by';
select fn_check_history_record(OLD, NEW,_ignorecol,_ignorecol1) into _value;
if(_value = 1)then

            INSERT INTO public.audit_att_details_pod(
            system_id, network_id, pod_name, latitude, longitude, 
            province_id, region_id, pincode, address, specification, category, 
            subcategory1, subcategory2, subcategory3, item_code, vendor_id, 
            type, brand, model, construction, activation, accessibility, 
            status, created_by, created_on, modified_by, modified_on, network_status, 
            parent_system_id, parent_network_id, parent_entity_type, sequence_id, 
            structure_id, shaft_id, floor_id, project_id, planning_id, purpose_id, 
            workorder_id, action,acquire_from,pod_type,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,remarks,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id,
            pole_count, manhole_count, spliceclosure_count,manhole_span,pole_span,cable_intersection_count
            )   
          select  new.system_id, new.network_id, new.pod_name, new.latitude, new.longitude, 
            new.province_id, new.region_id, new.pincode, new.address, new.specification, new.category, 
            new.subcategory1, new.subcategory2, new.subcategory3, new.item_code, new.vendor_id, 
            new.type, new.brand, new.model, new.construction, new.activation, new.accessibility, 
            new.status, new.created_by, new.created_on, new.modified_by, new.modified_on, new.network_status, 
            new.parent_system_id, new.parent_network_id, new.parent_entity_type, new.sequence_id, 
            new.structure_id, new.shaft_id, new.floor_id, new.project_id, new.planning_id, new.purpose_id, 
            new.workorder_id,'U' as action,new.acquire_from,new.pod_type,new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.remarks,new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category,new.gis_design_id,
            new.pole_count,new.manhole_count,new.spliceclosure_count,new.manhole_span,new.pole_span,new.cable_intersection_count
            from att_details_pod where system_id=new.system_id;

END IF;
	
END IF; 				
IF (TG_OP = 'DELETE' ) THEN  

          INSERT INTO public.audit_att_details_pod(
            system_id, network_id, pod_name, latitude, longitude, 
            province_id, region_id, pincode, address, specification, category, 
            subcategory1, subcategory2, subcategory3, item_code, vendor_id, 
            type, brand, model, construction, activation, accessibility, 
            status, created_by, created_on, modified_by, modified_on, network_status, 
            parent_system_id, parent_network_id, parent_entity_type, sequence_id, 
            structure_id, shaft_id, floor_id, project_id, planning_id, purpose_id, 
            workorder_id, action,acquire_from,pod_type,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,remarks,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id) 
         values(old.system_id, old.network_id, old.pod_name, old.latitude, old.longitude, 
            old.province_id, old.region_id, old.pincode, old.address, old.specification, old.category, 
            old.subcategory1, old.subcategory2, old.subcategory3, old.item_code, old.vendor_id, 
            old.type, old.brand, old.model, old.construction, old.activation, old.accessibility, 
            old.status, old.created_by, old.created_on, old.modified_by, old.modified_on, old.network_status, 
            old.parent_system_id, old.parent_network_id, old.parent_entity_type, old.sequence_id, 
            old.structure_id, old.shaft_id, old.floor_id, old.project_id, old.planning_id, old.purpose_id, 
            old.workorder_id, 'D',old.acquire_from,old.pod_type,old.ownership_type,old.third_party_vendor_id,old.source_ref_type,old.source_ref_id,old.source_ref_description,old.audit_item_master_id,old.status_remark,old.status_updated_by,old.status_updated_on,old.is_visible_on_map,old.remarks,old.is_acquire_from,old.other_info,old.origin_from,old.origin_ref_id,old.origin_ref_code,old.origin_ref_description,old.request_ref_id,old.requested_by,old.request_approved_by,old.subarea_id,old.area_id,old.dsa_id,old.csa_id,old.bom_sub_category,old.gis_design_id);
          
         
          Delete from point_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='POD';

END IF; 		

RETURN NEW;
END;
$BODY$;



CREATE OR REPLACE FUNCTION public.fn_site_get_export_report_nearest_data_kml(
	p_networkstatues character varying,
	p_provinceids character varying,
	p_regionids character varying,
	p_layer_name character varying,
	p_searchby character varying,
	p_searchbytext character varying,
	p_fromdate character varying,
	p_todate character varying,
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

drop table if exists temp_site_pit;
  create temp table temp_site_pit(
  system_id integer,
  entity_type character varying,
  network_id character varying,
  entity_title character varying,
  geom character varying,
  geom_type character varying,
  nearest_site_geometry character varying
  ) on commit drop;

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..	
SELECT STRING_AGG(COLUMN_NAME||' as "'||DISPLAY_NAME||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN

	if(p_radius>0)
	then
		select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10)  into p_geom;
	end if;
	
	sql:= 'select a.system_id, '''||p_layer_name||''' as entity_type,ST_ASTEXT(sp_geometry) as geom, a.network_id,''Point'' as geom_type,''Site'' as entity_title, ST_ASTEXT(nearest_site_geometry) as nearest_site_geometry from att_details_pod a 
	inner join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POINT(('||p_geom||'))'',4326)) 
	inner join user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.total_fiber_distance > 0 and  1=1 ';	

ELSE

	sql:= ' select a.system_id,''Site'' as entity_type,ST_ASTEXT(sp_geometry) as geom, a.network_id,''Point'' as geom_type,''Site'' as entity_title, ST_ASTEXT(nearest_site_geometry) as nearest_site_geometry from att_details_pod a 
		inner join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')
		inner join user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and  a.total_fiber_distance > 0  ';
		
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

IF ((P_searchbytext) != '' and (P_searchby) != '') 
THEN
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

execute 'insert into temp_site_pit(system_id,entity_type,network_id,entity_title,geom,geom_type,nearest_site_geometry) select system_id,entity_type::character varying,network_id::character varying,entity_title::character varying,geom::character varying,geom_type::character varying, nearest_site_geometry::character varying from ('||sql||')x';

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from (select * from temp_site_pit ) row';

END ;
$BODY$;	
	
	
	
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
                           'fiber_distance_to_nearest_site','cost_based_on_rate_card_lkr','is_site_imported','nearest_site',
                          'manhole_count','pole_count','spliceclosure_count') ORDER BY COLUMN_SEQUENCE ) A;

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
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported,(select case pod.site_id is null then pod.network_id else pod.site_id end from att_details_pod pod where pod.network_id=a.nearest_site) as nearest_site,
     a.manhole_count,a.pole_count,a.spliceclosure_count
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
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported,(select site_id from att_details_pod pod where pod.network_id=a.nearest_site) as nearest_site,
     a.manhole_count,a.pole_count,a.spliceclosure_count
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
    v_nearest_cable_end_geom geometry;
    v_nearest_cable_system_id integer;
    rec record;

BEGIN
     Drop table if exists temp_cable_route;
     DROP TABLE IF EXISTS temp_att_details_site;


    -- Create temp table
    CREATE TEMP TABLE temp_att_details_site(
        system_id integer,
        network_id character varying,
        site_geometry character varying,
        distance double precision default 0.00,
        nearest_cable_system_id integer,
        nearest_cable_end_geom character varying,
        nearest_cable_to_site_geometry geometry,
        start_point character varying,
        end_point character varying
    ) ON COMMIT DROP;

  CREATE TEMP TABLE temp_cable_route(
        system_id integer,
        seq integer,
      site_system_id integer
    ) ON COMMIT DROP;

    -- Exit early if buffer exceeds safety limit
    IF v_buffer > 3000 THEN
        RETURN;
    END IF;

    -- Get the geometry of the target POD
    SELECT sp_geometry INTO v_geom  
    FROM point_master 
    WHERE system_id = p_system_id AND entity_type = 'POD';

    -- Get the nearest cable endpoint
    SELECT 
        lm.system_id,
         ST_ClosestPoint(lm.sp_geometry,v_geom) AS nearest_point 
    INTO v_nearest_cable_system_id, v_nearest_cable_end_geom
    FROM line_master lm
    WHERE lm.entity_type = 'Cable' 
      AND lm.network_status = 'A'
      AND ST_Intersects(
            lm.sp_geometry, 
            ST_Buffer(v_geom, v_buffer)::geometry
          )
     ORDER BY 
    ST_Distance(lm.sp_geometry, v_geom)
    LIMIT 1;

    -- Debug Info
    RAISE INFO 'Value of variable v_nearest_cable_end_geom: %', v_nearest_cable_end_geom;

   if (v_nearest_cable_end_geom is not null) then
   
    -- Insert nearby sites into temp table
    INSERT INTO temp_att_details_site  (system_id,network_id,site_geometry,start_point,distance,nearest_cable_system_id,nearest_cable_end_geom,end_point)  
    SELECT 
        pm.system_id, 
        pm.common_name AS network_id,
        ST_Y(pm.sp_geometry) || ' ' || ST_X(pm.sp_geometry) AS site_geom,
        ST_X(pm.sp_geometry) || ' ' || ST_Y(pm.sp_geometry) AS start_point,
        ST_DistanceSphere(pm.sp_geometry, v_geom) AS distance, 
        v_nearest_cable_system_id,
        ST_Y(v_nearest_cable_end_geom) || ' ' || ST_X(v_nearest_cable_end_geom) AS nearest_cable_end_geom_text,
        ST_X(v_nearest_cable_end_geom) || ' ' || ST_Y(v_nearest_cable_end_geom) AS end_point
    FROM point_master pm
    WHERE pm.entity_type = 'POD'
      AND pm.system_id != p_system_id
      AND ST_Within(
            pm.sp_geometry,
            ST_Buffer(v_nearest_cable_end_geom, v_buffer)::geometry
          )
    ORDER BY distance
    LIMIT 5;
        
    if exists (select 1 from temp_att_details_site) 
    then 
    for rec in (select start_point,end_point,system_id from  temp_att_details_site)
    loop
    
      insert into temp_cable_route(seq,system_id,site_system_id)
      SELECT SEQ, EDGE,rec.system_id FROM PGR_DIJKSTRA('SELECT ID, SOURCE, TARGET, COST, REVERSE_COST 
      FROM ROUTING_DATA_CORE_PLAN', (SELECT ID
      FROM ROUTING_DATA_CORE_PLAN_VERTICES_PGR
      WHERE ST_WITHIN( THE_GEOM, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT(' || rec.start_point || ')', 4326), 3))
      LIMIT 1 ),(SELECT ID FROM ROUTING_DATA_CORE_PLAN_VERTICES_PGR
      WHERE ST_WITHIN( THE_GEOM, 
      ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('|| rec.end_point ||')', 4326), 3)) 
      LIMIT 1));
      
      delete from temp_cable_route where system_id = -1 and site_system_id = rec.system_id;
      if exists( select 1 from temp_cable_route where site_system_id =  rec.system_id)
      then 
     
      UPDATE temp_att_details_site tas
		SET distance = (
		    SELECT ST_Length(lm.sp_geometry::geography)
		    FROM temp_cable_route tc
		    JOIN line_master lm ON lm.system_id = tc.system_id
		    WHERE tc.site_system_id = rec.system_id
		)
	  WHERE tas.system_id = rec.system_id;
		
      end if;
      
      end loop;
     end if;

    -- Recursive call if none found
    IF NOT EXISTS (SELECT 1 FROM temp_att_details_site) THEN
        RETURN QUERY
        SELECT * FROM fn_get_nearest_site_records(p_system_id, p_network_id, v_buffer + 100);
    ELSE
       
        RETURN QUERY
        SELECT row_to_json(t)
        FROM ( select nearest_cable_end_geom,nearest_cable_system_id,distance,site_geometry,system_id, network_id , nearest_cable_to_site_geometry
 from temp_att_details_site where distance is not null ) t order by t.distance limit 1;
    END IF;
 END IF;
END;
$BODY$;




--select * from fn_site_get_export_report_nearest_data_kml('','','','POD','','','','','','',0,5);
CREATE OR REPLACE FUNCTION public.fn_site_get_export_report_nearest_data_kml(
	p_networkstatues character varying,
	p_provinceids character varying,
	p_regionids character varying,
	p_layer_name character varying,
	p_searchby character varying,
	p_searchbytext character varying,
	p_fromdate character varying,
	p_todate character varying,
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

drop table if exists temp_site_pit;
  create temp table temp_site_pit(
  system_id integer,
  entity_type character varying,
  network_id character varying,
  entity_title character varying,
  geom character varying,
  geom_type character varying,
  nearest_site_geometry character varying
  ) on commit drop;

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..	
SELECT STRING_AGG(COLUMN_NAME||' as "'||DISPLAY_NAME||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN

	if(p_radius>0)
	then
		select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10)  into p_geom;
	end if;
	
	sql:= 'select a.system_id, '''||p_layer_name||''' as entity_type,ST_ASTEXT(sp_geometry) as geom, a.network_id,''Point'' as geom_type,''Site'' as entity_title, ST_ASTEXT(nearest_site_geometry) as nearest_site_geometry from att_details_pod a 
	inner join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POINT(('||p_geom||'))'',4326)) 
	inner join user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.total_fiber_distance > 0 and  1=1 ';	

ELSE

	sql:= ' select a.system_id,''Site'' as entity_type,ST_ASTEXT(sp_geometry) as geom, a.network_id,''Point'' as geom_type,''Site'' as entity_title, ST_ASTEXT(nearest_site_geometry) as nearest_site_geometry from att_details_pod a 
		inner join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')
		inner join user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and  a.total_fiber_distance > 0  ';
		
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

IF ((P_searchbytext) != '' and (P_searchby) != '') 
THEN
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

execute 'insert into temp_site_pit(system_id,entity_type,network_id,entity_title,geom,geom_type,nearest_site_geometry) select system_id,entity_type::character varying,network_id::character varying,entity_title::character varying,geom::character varying,geom_type::character varying, nearest_site_geometry::character varying from ('||sql||')x';

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from (select * from temp_site_pit ) row';

END ;
$BODY$;



CREATE OR REPLACE FUNCTION public.fn_get_update_site_fiber_details(
	linestring character varying,
	nearestsite_system_id integer,
	p_system_id integer,
	nearestsite_distance double precision,
	p_nearest_cable_geom character varying,
	p_nearest_cable_system_id integer,
	p_cable_end_to_site_geom character varying)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE
    v_full_geom geometry;
    v_length double precision := 0;
   ug_distance double precision := 0;
   oh_distance double precision := 0;
   v_manhole_span integer;
   v_pole_span integer;
  v_restricted_intersect_count integer := 0;
  POLE_INTERSECT_COUNT INTEGER = 0;
   MANHOLE_INTERSECT_COUNT INTEGER = 0;

BEGIN

select value:: integer from global_settings  where key ='NearBySitePoleSpan' into v_pole_span ;
select value:: integer from global_settings  where key ='NearBySiteManholeSpan' into v_manhole_span ;

    v_full_geom := ST_GeomFromText('LINESTRING(' || p_cable_end_to_site_geom || ')', 4326);
    v_length = ST_Length(v_full_geom::geography);
 
   SELECT COUNT(*) into v_restricted_intersect_count FROM polygon_master
   WHERE ST_Intersects(sp_geometry, v_full_geom)
   AND entity_type = 'RestrictedArea';
              
        IF  v_restricted_intersect_count > 0
        then
         SELECT 
          SUM(ST_Length(ST_Intersection(v_full_geom, poly.sp_geometry)::geography)) into ug_distance
        FROM polygon_master poly
        WHERE 
            ST_Intersects(v_full_geom, poly.sp_geometry)
            AND poly.entity_type = 'RestrictedArea';
   
         oh_distance = v_length - ug_distance ; 
         
        else 
       oh_distance  = v_length;
        ug_distance = 0;

        END IF;
        -- POLE_INTERSECT_COUNT = (v_restricted_intersect_count * 2)+ 2;
         MANHOLE_INTERSECT_COUNT = (v_restricted_intersect_count * 2);        
         
         
        UPDATE att_details_pod
        SET fiber_oh_distance_to_network = oh_distance,
            fiber_ug_distance_to_network = ug_distance,
            total_fiber_distance = v_length,
            fiber_distance_to_nearest_site = nearestsite_distance + v_length,
            pole_count = CEIL(oh_distance/v_pole_span::double precision):: int,
            manhole_count = CEIL(ug_distance/v_manhole_span::double precision):: int,
            spliceclosure_count = FLOOR(v_length / 2000) + 1 
                                  + (CASE WHEN MOD(v_length, 2000) > 0 THEN 1 ELSE 0 END) 
                                  + (v_restricted_intersect_count * 2) ,
            manhole_span = v_manhole_span,
            pole_span =  v_pole_span,
            cable_intersection_count = MANHOLE_INTERSECT_COUNT,
            nearest_site = (select network_id from att_details_pod where system_id = nearestsite_system_id),
			nearest_site_geometry='LINESTRING(' || p_cable_end_to_site_geom || ')'
        WHERE system_id = p_system_id;

END;
$BODY$;
