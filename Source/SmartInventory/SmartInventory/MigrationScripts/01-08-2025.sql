

--------------------------------------------
---------------------------------------------- Update file size value while uploading files----------------------------------

update global_settings set value='20480' where key='MaxFileUploadSizeLimit';

----------------------------------Increase buffer range to retrieve details of nearby entities during segment creation.-------

DROP FUNCTION IF EXISTS public.fn_topology_get_selectedroute(character varying, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_selectedroute(
	route_geom character varying,
	p_agg1 integer,
	p_agg2 integer,
	p_user_id integer)
    RETURNS TABLE(route_id integer, route_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    v_source_geom geometry;
	v_buffered_route geometry;
	v_agg1site_id integer;
	v_agg2site_id integer;
	v_Count integer;
	R_Count integer;
BEGIN

create temp table temp_cable
(

network_id character varying

) on commit drop;

    -- Convert input string to a valid LINESTRING geometry
    

 -- Buffer in meters: Convert to geography, buffer, then back to geometry
    v_buffered_route := st_buffer_meters(
                                ST_GeomFromText('LINESTRING(' || route_geom || ')', 4326),  
                                100
                            );
                        
			RAISE INFO 'v_buffered_route -> %', v_buffered_route;			
   
	insert into temp_cable (network_id)
    SELECT 
        cbl.network_id AS route_id
    FROM 
        line_master l
    INNER JOIN 
        att_details_cable cbl 
        ON cbl.system_id = l.system_id
    WHERE 
        UPPER(l.entity_type) = 'CABLE'
        AND ST_Within(
            l.sp_geometry,
            v_buffered_route
        );
		
		select count(1) into v_Count from temp_cable;
		
		RAISE INFO 'Row Count -> %', v_Count;
	 IF v_Count > 0 THEN
	 
	 select count(1) into R_Count from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2 and tr.route_name ILike '%Manual_Route%';
	 --delete from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2 and tr.route_name='Manual_Route';
	 
	insert into top_routes(route_name,cable_names,total_length,route_geom,agg1site_id,agg2site_id)
	
	SELECT 
  'Manual_Route_' || (R_Count + 1)::text AS route_name,
  (select '{' || string_agg(network_id, ',') || '}' from temp_cable) as cable_names,
  0,
  v_source_geom AS  route_geom,
 p_agg1,
 p_agg2
 ;
 END IF;
 
 
 -- Return matching cable lines within 3 meters
   
    RETURN QUERY
    select Max(tr.route_id) AS route_id,tr.route_name from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2 and tr.route_name ILike '%Manual_Route%'
    GROUP BY tr.route_name order by 1 desc ;

END;
$BODY$;


--------------------------------------- Site owner related changes in project details--------------------------

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
		sp.id,sp.project_id, sp.site_id,sp.site_name,pod.owner_name as site_owner,sp.project_category, sp.cable_plan_cores, sp.maximum_cost,
 sp.comment,sp.location_address,sp.ds_cmc_area,sp.priority,pod.destination_site_id,pod.destination_port_type,pod.no_of_cores
        FROM site_project_details sp left join att_details_pod pod on pod.site_id=sp.site_id
        WHERE sp.site_id::TEXT = p_site_id
    ) t;
END;
$BODY$;

-----------------------------------------------------------------------------------------------------------------------------

DROP FUNCTION IF EXISTS public.fn_site_get_export_report_nearest_data_kml(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, double precision, integer);

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
	
	sql:= 'select a.system_id, '''||p_layer_name||''' as entity_type,ST_ASTEXT(sp_geometry) as geom, a.network_id,''Point'' as geom_type,a.site_id as entity_title, ST_ASTEXT(nearest_site_geometry) as nearest_site_geometry from att_details_pod a 
	inner join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POINT(('||p_geom||'))'',4326)) 
	inner join user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.total_fiber_distance > 0 and  1=1 ';	

ELSE

	sql:= ' select a.system_id,''Site'' as entity_type,ST_ASTEXT(sp_geometry) as geom, a.network_id,''Point'' as geom_type,a.site_id as entity_title, ST_ASTEXT(nearest_site_geometry) as nearest_site_geometry from att_details_pod a 
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
---------------------------------------------------Modify the view--------------------------------------------

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
    pod.spliceclosure_count,
	pod.nearest_site_geometry
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
   
   -------------------------------------------------Modify the function ---------------------------------------------
   
   
   CREATE OR REPLACE FUNCTION public.fn_sr_get_export_report_data(p_networkstatues character varying, p_provinceids character varying, p_regionids character varying, p_layer_name character varying, p_searchby character varying, p_searchbytext character varying, p_fromdate character varying, p_todate character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_geom character varying, p_duration_based_column character varying, p_radius double precision, p_userid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

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
                          'manhole_count','pole_count','spliceclosure_count','owner_name') ORDER BY COLUMN_SEQUENCE ) A;

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
     a.fiber_oh_distance_to_network,ROUND(a.fiber_ug_distance_to_network::NUMERIC, 2) as fiber_ug_distance_to_network,ROUND(ST_Length(a.nearest_site_geometry::geography)::NUMERIC,2) as total_fiber_distance,
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported,(select case pod.site_id is null then pod.network_id else pod.site_id end from att_details_pod pod where pod.network_id=a.nearest_site limit 1) as nearest_site,
     a.manhole_count,a.pole_count,a.spliceclosure_count,a.owner_name
   from '||S_REPORT_VIEW_NAME||' a inner join '||s_geom_type||'_master m
	on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
	and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
	RAISE INFO '------------------------------------sql%', sql;
ELSE
     -- a.system_id, a.design_id,a.network_id,a.port_type

	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id, '||S_LAYER_COLUMNS||' from (select 
     a.system_id, a.site_id,a.site_name,a.latitude,a.longitude, 
     a.fiber_oh_distance_to_network,ROUND(a.fiber_ug_distance_to_network::NUMERIC, 2) as fiber_ug_distance_to_network,ROUND(ST_Length(a.nearest_site_geometry::geography)::NUMERIC,2) as total_fiber_distance,
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported,(select site_id from att_details_pod pod where pod.network_id=a.nearest_site limit 1) as nearest_site,
     a.manhole_count,a.pole_count,a.spliceclosure_count,a.owner_name
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
$function$
;


