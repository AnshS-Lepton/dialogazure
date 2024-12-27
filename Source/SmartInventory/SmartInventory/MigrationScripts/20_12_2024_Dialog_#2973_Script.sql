/*------------------------------------------
CreatedBy: Chandra Shekhar Sahni
CreatedOn: 20 Dec 2024
Description: This function retrieves all the associated fiberlinks
Purpose: This was a new requirement to get all the fiberlinks and bind to choosen dropdown
ModifiedOn: <>
ModifiedBy: <>
------------------------------------------*/
-- DROP FUNCTION public.fn_get_fiber_links(varchar, int4, int4, varchar, int4);

CREATE OR REPLACE FUNCTION public.fn_get_fiber_links(p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sorttype character varying, p_userid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
   sql TEXT; 
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords INTEGER; 
   s_layer_columns_val text; 

BEGIN

sql:='';
	 
	SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM(
	 SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM fiber_link_columns_settings WHERE is_active=true and column_name in ('link_name','link_id','network_id','link_type')
		order by Column_sequence) A;

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY fl.system_id' || ' ' || CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS S_No, 
fl.system_id,'||S_LAYER_COLUMNS_VAL||'
	FROM vw_att_details_fiber_link fl WHERE 1=1 AND fiber_link_status!=''Free'' AND fl.created_by_id='||p_userId||'';

IF (p_searchtext != '') THEN
    sql := sql || ' AND (lower(fl.link_id::text) ILIKE lower(''%' || TRIM(p_searchtext) || '%'') ' ||
                   ' OR lower(fl.link_name::text) ILIKE lower(''%' || TRIM(p_searchtext) || '%'') )';
END IF;
RAISE INFO '%', sql;
-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a ' INTO TotalRecords;
 
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords,*
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$function$
;
---------------------------------------------------------
/*------------------------------------------
CreatedBy: ---
CreatedOn: ---
Description: ---
Purpose: ---
ModifiedBy: Chandra Shekhar
ModifiedOn: 20 Dec 2024
Purpose: This view was returning duplicate data to added the DISTINCT keyword in the select clause
------------------------------------------*/
-- public.vw_att_details_fiber_link source

CREATE OR REPLACE VIEW public.vw_att_details_fiber_link
AS select DISTINCT link.system_id,
    link.link_id,
    link.link_name,
    link.link_type,
    link.network_id,
    link.start_point_type,
    link.start_point_network_id,
    link.start_point_location,
    link.end_point_type,
    link.end_point_network_id,
    link.end_point_location,
    link.no_of_lmc,
    link.each_lmc_length,
    link.total_route_length,
    link.gis_length,
    link.otdr_distance,
    link.no_of_pair,
    link.tube_and_core_details,
    link.existing_route_length_otdr,
    link.new_building_route_length,
    link.otm_length,
    link.otl_length,
        CASE
            WHEN link.any_row_portion = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS any_row_portion,
    link.row_authority,
    link.total_row_segments,
    link.total_row_length,
    link.total_row_reccuring_charges,
    fn_get_date(link.handover_date, false) AS handover_date,
    fn_get_date(link.hoto_signoff_date, false) AS hoto_signoff_date,
    link.fiber_link_status,
    link.remarks,
    um.user_name AS created_by,
    fn_get_date(link.created_on) AS created_on,
    fn_get_date(link.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    um.user_id AS created_by_id,
    link.service_id,
    link.main_link_type,
    link.main_link_id,
    link.redundant_link_type,
    link.redundant_link_id,
    pm1.latitude AS start_point_latitude,
    pm1.longitude AS start_point_longitude,
    pm2.latitude AS end_point_latitude,
    pm2.longitude AS end_point_longitude,
    cbl.status,
    cbl.region_id,
    cbl.province_id,
    fn_get_network_status(cbl.network_status) AS network_status
   FROM att_details_fiber_link link
     JOIN user_master um ON link.created_by = um.user_id
     LEFT JOIN user_master um2 ON link.modified_by = um2.user_id
     LEFT JOIN point_master pm1 ON replace(link.start_point_type::text, ' '::text, ''::text) = pm1.entity_type::text AND link.start_point_network_id::text = pm1.common_name::text
     LEFT JOIN point_master pm2 ON replace(link.end_point_type::text, ' '::text, ''::text) = pm1.entity_type::text AND link.end_point_network_id::text = pm1.common_name::text
     JOIN att_details_cable_info ci ON ci.link_system_id = link.system_id
     JOIN att_details_cable cbl ON ci.cable_id = cbl.system_id;
	 
------------------------------------------------------------------------------------
UPDATE public.data_uploader_template SET template_column_name='Route Name' WHERE db_column_name='subarea_id' AND layer_id=13;
UPDATE public.data_uploader_template SET template_column_name='Route Name' WHERE db_column_name='subarea_id' AND layer_id=14;
UPDATE public.data_uploader_template SET template_column_name='EE Area' WHERE db_column_name='gis_design_id' AND layer_id=13;
UPDATE public.data_uploader_template SET template_column_name='EE Area' WHERE db_column_name='gis_design_id' AND layer_id=14;
------------------------------------------------------------------------------------
/*------------------------------------------
CreatedBy: ---
CreatedOn: ---
Description: ---
Purpose: ---
ModifiedBy: Chandra Shekhar
ModifiedOn: 23 Dec 2024
Purpose: increased input variable to the function if we pass link_id then 
result will be completely depend on link id otherwise it will work as per previous functionality
------------------------------------------*/
-- DROP FUNCTION public.fn_get_search_equipment(varchar, int4, varchar);

CREATE OR REPLACE FUNCTION public.fn_get_search_equipment(p_searchtext character varying, p_user_id integer, p_link_id character varying)
 RETURNS TABLE(system_id integer, network_id character varying, entity_type character varying, geom_type character varying, network_status character varying, no_of_ports character varying, display_name text)
 LANGUAGE plpgsql
AS $function$ 
declare
v_role_id integer;
v_link_system_id integer;       
BEGIN
	 -- Fetch role ID
	select role_id into v_role_id from user_master where user_id=p_user_id;

	-- Drop the temporary table if it already exists 
	--DROP TABLE IF EXISTS temp_search_results;

   -- Create temporary table for initial query results
    CREATE TEMP TABLE temp_search_results ON COMMIT DROP AS
	select a.* from (
	-- SEARCH ALL POINT ENTITY WITH SEARCH TEXT EXCEPT CUSOTMER.
	select pm.system_id, pm.common_name as network_id, pm.entity_type,'Point'::character varying as geom_Type,pm.network_status,pm.no_of_ports,
	pm.display_name||'-('||coalesce(pm.no_of_ports,'')||')-('||coalesce(pm.network_status,'')||')' as display_name from point_master pm
	inner join province_boundary province on ST_Within(pm.sp_geometry,province.sp_geometry) 
	inner join user_permission_area upa on upa.region_id=province.region_id and upa.province_id=province.id and upa.user_id=p_user_id
        WHERE pm.network_status !='D' and UPPER(pm.entity_type) IN (select upper(layer_name) from layer_details where isvisible=true and 
	is_virtual_port_allowed=false and upper(layer_name)!='CUSTOMER') and (upper(pm.display_name) like '%'||upper(p_searchtext)||'%' or upper(pm.common_name) like '%'||upper(p_searchtext)||'%')
	union
	-- SEARCH CUSTOMERS.
	 select pm.system_id,pm.common_name as network_id,pm.entity_type,'Point'::character varying as geom_Type,pm.network_status, pm.no_of_ports,
        pm.display_name||'('||coalesce(cust.customer_name,'')||')' as display_name from point_master pm inner join att_details_customer cust 
	on pm.system_id=cust.system_id and upper(pm.entity_type)='CUSTOMER'
	inner join user_permission_area upa on upa.region_id=cust.region_id and upa.province_id=cust.province_id and upa.user_id=p_user_id 
	WHERE upper(cust.network_id) like '%'||upper(p_searchtext)||'%' or upper(cust.customer_name) like '%'||upper(p_searchtext)||'%'or
	 upper(pm.display_name) like '%'||upper(p_searchtext)||'%'
        union
	--SEARCH CABLES.
	SELECT lm.system_id,lm.common_name AS network_id, lm.entity_type,'Line'::character varying as geom_Type,lm.network_status,cbl.total_core||'F' as no_of_ports,
	lm.display_name||'-('||coalesce(cbl.total_core,0)||'F)-('||coalesce(lm.network_status,'')||')' as display_name
	from att_details_cable cbl inner join line_master lm on cbl.system_id=lm.system_id and upper(lm.entity_type)='CABLE' 
	inner join user_permission_area upa on upa.region_id=cbl.region_id and upa.province_id=cbl.province_id and upa.user_id=p_user_id 
	where lm.network_status !='D' and  (upper(lm.display_name) like '%'||upper(p_searchtext)||'%' or upper(lm.common_name) like '%'||upper(p_searchtext)||'%')
	--SEARCH ISP CABLES--
	UNION
	SELECT cbl.system_id,cbl.network_id AS network_id, lm.entity_type,'Line'::character varying as geom_Type,cbl.network_status,cbl.total_core||'F' as no_of_ports,
	lm.display_name||'-('||coalesce(cbl.total_core,0)||'F)-('||coalesce(cbl.network_status,'')||')' as display_name
	from att_details_cable cbl 
	inner join isp_line_master lm on cbl.system_id=lm.entity_id  and upper(lm.entity_type)='CABLE' and lm.structure_id>0 
	inner join user_permission_area upa on upa.region_id=cbl.region_id and upa.province_id=cbl.province_id and upa.user_id=p_user_id 
	where upper(lm.display_name) like '%'||upper(p_searchtext)||'%' or upper(cbl.network_id) like '%'||upper(p_searchtext)||'%'
) a 
left join layer_details l on upper(a.entity_type)=upper(l.layer_name)
inner  join role_permission_entity rp on l.layer_id  = rp.layer_id and rp.role_id = v_role_id and rp.network_status = a.network_status and rp.viewonly = true
order by display_name desc;
--RAISE INFO '%', 'QUERY';

-- Fetch system_id from att_details_fiber_link if p_link_id is provided
    IF p_link_id IS NOT NULL AND p_link_id <> '' THEN
		
		RETURN QUERY 
        SELECT tsr.*
        FROM temp_search_results tsr
        INNER JOIN att_details_cable_info ci ON tsr.system_id = ci.cable_id 
		where ci.link_system_id>0 AND ci.link_system_id IN (
			SELECT fl.system_id 
        FROM att_details_fiber_link fl
        WHERE fl.link_id = p_link_id
		) LIMIT 10;
	ELSE
        RETURN QUERY 
        SELECT *
        FROM temp_search_results LIMIT 10;
    END IF;

END
$function$
;
---------------------------------------------------------------------------------------
UPDATE att_details_cable
SET network_id = REPLACE(network_id, chr(10), '')
WHERE network_id in (SELECT network_id
FROM att_details_cable
WHERE network_id LIKE '%' || chr(10) || '%'
);
---------------------------------------------------------------------------------------
select * from  fn_sync_layer_columns();
---------------------------------------------------------------------------------------