/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: This function is used to pull the network ticket details
ModifiedOn: 14 Jan 2025 
ModifiedBy: Chandra Shekhar Sahni 
Purpose: Modified to pull network ticket data from vw_att_details_networktickets and ticket_type_role_mapping only
------------------------------------------*/
-- DROP FUNCTION public.fn_nwt_get_ticket_details(varchar, varchar, int4, int4, varchar, varchar, int4, timestamp, timestamp, int4, int4, int4, int4);

CREATE OR REPLACE FUNCTION public.fn_nwt_get_ticket_details(p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_userid integer, p_searchfrom timestamp without time zone, p_searchto timestamp without time zone, p_ticket_type_id integer, p_region_id integer, p_province_id integer, p_ticket_status_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE
sql TEXT;
NWstatus TEXT;
StartSNo INTEGER;
EndSNo INTEGER;
TotalRecords integer;
v_user_role_id integer;
v_is_admin_rights_enabled boolean;
BEGIN

p_searchby:=replace(replace(p_searchby,'(',''),')','');

select role_id,is_admin_rights_enabled into v_user_role_id,v_is_admin_rights_enabled from user_master where user_id=p_userid;

-- DYNAMIC QUERY
raise info'role_id:-% ',v_user_role_id;
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (p_sortcolname) = '' THEN 'nt.ticket_id' ELSE 'nt.'||p_sortcolname END || ' ' ||CASE WHEN (p_sorttype) ='' THEN 'desc' else p_sorttype end||') AS S_No
,nt.*,ttrm.id,ttrm.ticket_type_id,ttrm.role_id,ttrm.is_create,ttrm.is_edit,ttrm.is_view,ttrm.is_approve,ttrm.ticket_type from vw_att_details_networktickets nt
inner join ticket_type_role_mapping ttrm on nt.ticket_type_id=ttrm.ticket_type_id and ttrm.role_id ='||v_user_role_id||' and ttrm.is_view=true
inner join user_permission_area upa on nt.province_id=upa.province_id and upa.user_id='||p_userid||'
WHERE 1=1';

raise info'sql:-% ',sql;
NWstatus:= 'SELECT tsm.name::character varying as ticket_status,count(nt.ticket_status_id) as ticket_count,tsm.color_code,tsm.opacity from
vw_att_details_networktickets nt right join
ticket_status_master tsm on tsm.id=nt.ticket_status_id
inner join user_permission_area upa on nt.province_id=upa.province_id and upa.user_id='||p_userid||'
';


IF(p_region_id >0 )THEN
sql:= sql ||'and nt.region_id='||p_region_id||'';
NWstatus:= NWstatus ||'and nt.region_id='||p_region_id||'';
END IF;
IF(p_province_id >0 )THEN
sql:= sql ||'and nt.province_id='||p_province_id||'';
NWstatus:= NWstatus ||'and nt.province_id='||p_province_id||'';
END IF;
IF(p_ticket_status_id >0 )THEN
sql:= sql ||'and nt.ticket_status_id='||p_ticket_status_id||'';
NWstatus:= NWstatus ||'and nt.ticket_status_id='||p_ticket_status_id||'';
END IF;

IF(p_ticket_type_id >0 )THEN
sql:= sql ||'and nt.ticket_type_id='||p_ticket_type_id||'';
NWstatus:= NWstatus ||'and nt.ticket_type_id='||p_ticket_type_id||'';
END IF;
raise info'v_user_role_id :-% ',v_user_role_id;

IF(v_user_role_id >0 )THEN
sql:= sql ||' and case when ('||v_user_role_id||' =1 or '||v_is_admin_rights_enabled||'=true) then 1=1 else 1=1 end AND (nt.assigned_to='||p_userid||' OR cast(nt.created_by as integer) in (select user_id from user_master where manager_id in ('||p_userid||') or user_id in ('||p_userid||')))';

NWstatus:= NWstatus ||' and case when ('||v_user_role_id||' =1 or '||v_is_admin_rights_enabled||'=true) then 1=1 else nt.ticket_type_id in (select ticket_type_id from ticket_type_role_mapping where role_id= '||v_user_role_id||' and is_view=true) end';
END IF;

raise info'sqltest% ',sql;

IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
NWstatus:= NWstatus ||' AND lower(nt.'||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;

IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND nt.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and nt.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';
NWstatus:= NWstatus ||' AND nt.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and nt.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';
END IF;
raise info'sql-record:-% ',sql;
-- GET TOTAL RECORD COUNT
EXECUTE 'SELECT COUNT(1) FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
IF((p_pageno) <> 0) THEN
StartSNo:= p_pagerecord * (p_pageno - 1 ) + 1;
EndSNo:= p_pageno * p_pagerecord;
sql:= 'SELECT '||TotalRecords||' as totalRecords, *
FROM (' || sql || ' ) T
WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo;

ELSE
sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T ';
END IF;

RAISE INFO 'lstNWStatus:-%', NWstatus;
RAISE INFO 'lstNWDetails:-%', sql;

RETURN QUERY
EXECUTE 'select row_to_json(row) from (
select(
select array_to_json(array_agg(row_to_json(x)))
from (
'||NWstatus||' where tsm.module=''Network_Ticket'' and is_active=true group by tsm.name,nt.ticket_status_id,tsm.color_code,tsm.action_seq,tsm.opacity order by tsm.action_seq
) x
) as lstNWStatus,
(
select array_to_json(array_agg(row_to_json(x)))
from('||sql||')x

) as lstNWDetails
) row';

END ;
$function$
;

------------------------------------------------------------------------------------------

/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: This function is used to update or insert site/pod details in att_pod_details table
ModifiedOn: 14 Jan 2025 
ModifiedBy: Chandra Shekhar Sahni 
Purpose: Modified this function to update ,origin_ref_code,origin_ref_id columns also in att_details_pod table
------------------------------------------*/

-- DROP FUNCTION public.fn_process_save_pod_details(in int4, out int4, out int4);

CREATE OR REPLACE FUNCTION public.fn_process_save_pod_details(process_id_input integer, OUT updated_count integer, OUT inserted_count integer)
 RETURNS record
 LANGUAGE plpgsql
AS $function$
DECLARE
--    process_id_input INTEGER := 1;  -- Example process_id_input, replace as needed
--    inserted_count INTEGER;
    rec RECORD;
    v_status TEXT;
    v_message TEXT;
    v_p_system_id INTEGER;
    v_p_network_id INTEGER;
    v_p_entity_type TEXT;
    v_sequence_id INTEGER;
BEGIN
    -- First condition: Update records in att_details_pod based on matching site_id in process_site_details
    WITH update_cte AS (
        UPDATE public.att_details_pod AS ads
        SET 
            site_name = psd.site_name,
            on_air_date = psd.on_air_date,
            removed_date = psd.removed_date,
            tx_type = psd.tx_type,
            tx_technology = psd.tx_technology,
            tx_segment = psd.tx_segment,
            tx_ring = psd.tx_ring,
            address = psd.address,
            region = psd.region,
            province = psd.province,
            district = psd.district,
            region_address = psd.region_address,
            depot = psd.depot,
            ds_division = psd.ds_division,
            local_authority = psd.local_authority,
            latitude = CASE WHEN psd.network_status <> 'A' THEN psd.latitude ELSE ads.latitude END,
            longitude = CASE WHEN psd.network_status <> 'A' THEN psd.longitude ELSE ads.longitude END,
            owner_name = psd.owner_name,
            access_24_7 = psd.access_24_7,
            tower_type = psd.tower_type,
            tower_height = psd.tower_height,
            cabinet_type = psd.cabinet_type,
            solution_type = psd.solution_type,
            site_rank = psd.site_rank,
            self_tx_traffic = psd.self_tx_traffic,
            agg_tx_traffic = psd.agg_tx_traffic,
            metro_ring_utilization = psd.metro_ring_utilization,
            csr_count = psd.csr_count,
            dti_circuit = psd.dti_circuit,
            agg_01 = psd.agg_01,
            agg_02 = psd.agg_02,
            bandwidth = psd.bandwidth,
            ring_type = psd.ring_type,
            link_id = psd.link_id,
            alias_name = psd.alias_name,
            created_on = psd.created_on,
            created_by = psd.created_by,
            tx_agg = psd.tx_agg,
            bh_status = psd.bh_status,
            elevation = psd.elevation::double precision,
            segment = psd.segment,
            ring = psd.ring,
            maximum_cost = psd.maximum_cost,
            project_category = psd.project_category,
            priority = psd.priority,
            no_of_cores = psd.no_of_cores,
            fiber_link_type = psd.fiber_link_type,
            "comment" = psd.comment,
            plan_cost = psd.plan_cost,
            fiber_distance = psd.fiber_distance,
            fiber_link_code = psd.fiber_link_code,
            port_type = psd.port_type,
            destination_site_id = psd.destination_site_id,
            destination_port_type = psd.destination_port_type,
            destination_no_of_cores = psd.destination_no_of_cores,
            project_id_dialog = psd.project_id_dialog,
			network_status=psd.network_status
        FROM process_site_details AS psd
        WHERE ads.site_id = psd.site_id
          AND psd.process_id = process_id_input
        RETURNING 1
    )
    SELECT COUNT(*) INTO updated_count FROM update_cte;

    -- Second condition: Insert records into att_details_pod where site_id from process_site_details doesn't exist in att_details_pod
    -- Create a temporary table
    CREATE TEMPORARY TABLE temp_pod_details AS
    SELECT 
        psd.site_id, psd.site_name, psd.on_air_date, psd.removed_date, psd.tx_type, psd.tx_technology, psd.tx_segment,
        psd.tx_ring, psd.address, psd.region, psd.province, psd.district, psd.region_address, psd.depot,
        psd.ds_division, psd.local_authority, psd.latitude, psd.longitude, psd.owner_name, psd.access_24_7,
        psd.tower_type, psd.tower_height, psd.cabinet_type, psd.solution_type, psd.site_rank, psd.self_tx_traffic,
        psd.agg_tx_traffic, psd.metro_ring_utilization, psd.csr_count, psd.dti_circuit, psd.agg_01, psd.agg_02,
        psd.bandwidth, psd.ring_type, psd.link_id, psd.alias_name, psd.created_on, psd.created_by, psd.province_id, psd.region_id,
        psd.network_status, psd.tx_agg, psd.bh_status, psd.elevation::double precision, psd.segment, psd.ring, psd.maximum_cost,
        psd.project_category, psd.priority, psd.no_of_cores, psd.fiber_link_type, psd.comment, psd.plan_cost, psd.fiber_distance,
        psd.fiber_link_code, psd.port_type, psd.destination_site_id, psd.destination_port_type, psd.destination_no_of_cores,
        psd.project_id_dialog, '' AS status, '' AS network_id, 0 AS parent_system_id, 0 AS parent_network_id,
        '' AS parent_entity_type, 0 AS sequence_id,
        0 AS vendor_id, 0 AS type, 0 AS brand, '' AS ownership_type, '' AS bom_sub_category,
        '' AS item_code, 0 AS model, 0 AS construction, 0 AS activation, 0 AS accessibility
    FROM process_site_details AS psd
    WHERE psd.process_id = process_id_input
      AND psd.site_id NOT IN (SELECT site_id FROM public.att_details_pod WHERE site_id IS NOT NULL);

	-- Populate the temporary table with additional data
    FOR rec IN (SELECT * FROM temp_pod_details) LOOP
        BEGIN
            -- Call the function to get the required data and check if message is not NULL or empty
            SELECT status, message, o_p_system_id, o_p_network_id, o_p_entity_type, o_sequence_id
            INTO v_status, v_message, v_p_system_id, v_p_network_id, v_p_entity_type, v_sequence_id
            FROM fn_get_clone_network_code('POD', 'Point', rec.longitude || ' ' || rec.latitude, NULL, NULL)
            WHERE message IS NOT NULL AND message <> '';
            
            -- Update the temporary table with the obtained data
            UPDATE temp_pod_details
            SET 
                status = v_status,
                network_id = v_message,
                parent_system_id = v_p_system_id,
                parent_network_id = v_p_network_id,
                parent_entity_type = v_p_entity_type,
                sequence_id = v_sequence_id
            WHERE 
                site_id = rec.site_id;
        EXCEPTION
            WHEN OTHERS THEN
                -- Handle any exceptions and skip to the next record
                RAISE NOTICE 'Exception occurred for site_id: %, skipping to next record.', rec.site_id;
                CONTINUE;
        END;
    END LOOP;

	 -- Insert data from the temporary table into the att_details_pod table
    INSERT INTO public.att_details_pod (
        site_id, site_name, on_air_date, removed_date, tx_type, tx_technology, tx_segment, tx_ring,
        address, region, province, district, region_address, depot, ds_division, local_authority,
        latitude, longitude, owner_name, access_24_7, tower_type, tower_height, cabinet_type, solution_type,
        site_rank, self_tx_traffic, agg_tx_traffic, metro_ring_utilization, csr_count, dti_circuit, agg_01, agg_02,
        bandwidth, ring_type, link_id, alias_name, created_on, created_by, province_id, region_id, network_status, tx_agg, bh_status, elevation, segment,
        ring, maximum_cost, project_category, priority, no_of_cores, fiber_link_type, "comment", plan_cost,
        fiber_distance, fiber_link_code, port_type, destination_site_id, destination_port_type, destination_no_of_cores,
        project_id_dialog, parent_system_id, sequence_id, vendor_id, type, brand, ownership_type, bom_sub_category, item_code, model, construction, activation, accessibility,
        status, network_id, pod_name, parent_network_id,parent_entity_type,origin_ref_code,origin_ref_id
    )
    SELECT 
        site_id, site_name, on_air_date, removed_date, tx_type, tx_technology, tx_segment, tx_ring,
        address, region, province, district, region_address, depot, ds_division, local_authority,
        latitude, longitude, owner_name, access_24_7, tower_type, tower_height, cabinet_type, solution_type,
        site_rank, self_tx_traffic, agg_tx_traffic, metro_ring_utilization, csr_count, dti_circuit, agg_01, agg_02,
        bandwidth, ring_type, link_id, alias_name, created_on, created_by, province_id, region_id, network_status, tx_agg, bh_status, elevation, segment,
        ring, maximum_cost, project_category, priority, no_of_cores, fiber_link_type, "comment", plan_cost,
        fiber_distance, fiber_link_code, port_type, destination_site_id, destination_port_type, destination_no_of_cores,
        project_id_dialog, parent_system_id, sequence_id, vendor_id, type, brand, ownership_type, bom_sub_category, item_code, model, construction, activation, accessibility,
        status, network_id, network_id,  parent_network_id, parent_entity_type,'DTS',process_id_input
    FROM temp_pod_details;
	
	SELECT COUNT(*) INTO inserted_count FROM temp_pod_details;
    -- Drop the temporary table
    DROP TABLE IF EXISTS temp_pod_details;

END;
$function$
;
