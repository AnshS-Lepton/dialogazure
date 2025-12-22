CREATE OR REPLACE FUNCTION public.fn_network_planning_auto_create_cable(p_a_system_id integer, p_a_entity_type character varying, p_a_network_id character varying, p_b_system_id integer, p_b_entity_type character varying, p_b_network_id character varying, p_longlat character varying, p_structure_id integer, p_region_id integer, p_province_id integer, p_user_id integer, p_rfs_type character varying, p_length double precision, p_wcr_id integer, p_plan_id integer, p_cable_type character varying, p_duct_id integer, p_duct_network_id character varying)
 RETURNS record
 LANGUAGE plpgsql
AS $function$
DECLARE
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow_cable record;
v_arow_network_id record;
v_cable_display_value CHARACTER VARYING;
v_a_display_value CHARACTER VARYING;
v_b_display_value CHARACTER VARYING;
v_display_value CHARACTER VARYING;
v_display_name CHARACTER VARYING;
v_duct_value character varying;
BEGIN
V_IS_VALID:=true;
V_MESSAGE:='Successfully';
	
	select * into v_arow_network_id from fn_get_line_network_code('Cable',p_a_network_id,p_b_network_id,p_longlat,'OSP');
	
	INSERT INTO ATT_DETAILS_CABLE(is_visible_on_map,audit_item_master_id,network_id,cable_name,a_location,b_location,total_core,no_of_tube,no_of_core_per_tube,cable_measured_length,cable_calculated_length,
	cable_type,specification,category,subcategory1,subcategory2,subcategory3,item_code,	
	vendor_id,network_status,status,province_id,region_id,created_by,created_on,a_system_id,a_network_id,a_entity_type,
	b_system_id,b_network_id,b_entity_type,sequence_id,cable_category,parent_system_id,parent_network_id,parent_entity_type,start_reading,	
	end_reading,structure_id,utilization,duct_id,trench_id,is_used,source_ref_type,source_ref_id,ownership_type)

	select  true,p_wcr_id,v_arow_network_id.network_code,v_arow_network_id.network_code,p_a_network_id,p_b_network_id,total_core,no_of_tube,no_of_core_per_tube,ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),ST_Length(ST_GeomFromText('LINESTRING('||p_longlat||')',4326),false),
	p_cable_type,specification,category, subcategory1, subcategory2, subcategory3,item_code,
	vendor_id,'P','A',p_province_id,p_region_id,p_user_id,now(),p_a_system_id,p_a_network_id,
	p_a_entity_type,p_b_system_id,p_b_network_id,p_b_entity_type,v_arow_network_id.sequence_id,'Feeder Cable',v_arow_network_id.parent_system_id,
	v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
	0,0,p_structure_id,'L',0,0,true,'planning',p_plan_id,'OWN' from item_template_cable itm where created_by=p_user_id and audit_item_master_id=p_wcr_id limit 1 
	RETURNING system_id,network_id,no_of_tube,total_core into v_arow_cable;


	INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,
	common_name,network_status,is_virtual,display_name,source_ref_type,source_ref_id)
	values(v_arow_cable.SYSTEM_ID,'Cable','A',
	ST_GeomFromText('LINESTRING('||p_longlat||')',4326)	
	,'','',
	p_user_id,0,v_arow_cable.network_id,'P',false,fn_get_display_name(v_arow_cable.SYSTEM_ID,'Cable'),'planning',p_plan_id);
	
	/*select display_name into v_a_display_value from point_master where system_id=p_a_system_id and upper(entity_type)=upper(p_a_entity_type);
	select display_name into v_b_display_value from point_master where system_id=p_b_system_id and upper(entity_type)=upper(p_b_entity_type);

	insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,
	associated_entity_type,associated_display_name,created_on,created_by,is_termination_point)
	values(v_arow_cable.system_id,v_arow_cable.network_id,'Cable',v_display_value,p_a_system_id,p_a_network_id,p_a_entity_type,v_a_display_value,now(),p_user_id,true),
	(v_arow_cable.system_id,v_arow_cable.network_id,'Cable',v_display_value,p_b_system_id,p_b_network_id,p_b_entity_type,v_b_display_value,now(),p_user_id,true);
*/
	if(p_duct_id >0)
	then
	
	insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
		entity_type,created_on,created_by,associated_display_name,entity_display_name)
		values('Cable',v_arow_cable.system_id,v_arow_cable.network_id,p_duct_network_id,p_duct_id,'Duct',now(),p_user_id,
		fn_get_display_name(v_arow_cable.system_id,'Cable'),fn_get_display_name(p_duct_id,'Duct')); 
		
	end if;

	
	
	perform(fn_set_cable_color_info(v_arow_cable.system_id,p_user_id,v_arow_cable.no_of_tube,v_arow_cable.total_core));					
		
	RETURN v_arow_cable;          
END
$function$
;

---------------------------------------------------------------------------------------------

alter table att_details_pod add column asbuit_ibw_distance double precision default 0;
alter table att_details_pod add column asbuit_osp_distance double precision default 0;
alter table att_details_pod add column asbuit_ibw_osp_total_distance double precision default 0;

---------------------------------------------------------------------------------------------------

DELETE FROM public.dropdown_master
WHERE dropdown_value ='CRM' and layer_id = (select layer_id  from layer_details  where layer_abbr = 'NT');

---------------------------------------------------------------------------------------------------
INSERT INTO public.ticket_type_master
(ticket_type, description, color_code, icon_content, icon_class, created_on, created_by, modified_on, modified_by, "module", abbreviation)
VALUES('Design Approval', 'Design Approval', '#000000', '','', now(), 1, NULL, NULL, 'Network_Ticket', 'DA');

INSERT INTO public.ticket_type_master
(ticket_type, description, color_code, icon_content, icon_class, created_on, created_by, modified_on, modified_by, "module", abbreviation)
VALUES('Way Leave', 'Way Leave', '#000000', '','', now(), 1, NULL, NULL, 'Network_Ticket', 'WL');


INSERT INTO public.ticket_type_master
(ticket_type, description, color_code, icon_content, icon_class, created_on, created_by, modified_on, modified_by, "module", abbreviation)
VALUES('Estimate Payment', 'Estimate Payment', '#000000', '','', now(), 1, NULL, NULL, 'Network_Ticket', 'EP');

--------------------------------------------------------------------------------------------------

CREATE OR REPLACE VIEW public.vw_att_details_pod
AS SELECT pod.system_id,
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
    pod.vendor_id,
    pod.type,
    pod.brand,
    pod.model,
    pod.construction,
    pod.activation,
    pod.accessibility,
    pod.status,
    pod.network_status,
    pod.province_id,
    pod.region_id,
    pod.created_by,
    pod.created_on,
    pod.modified_by,
    pod.modified_on,
    pod.parent_system_id,
    pod.parent_network_id,
    pod.parent_entity_type,
    pod.sequence_id,
    pod.structure_id,
    pod.shaft_id,
    pod.floor_id,
    pod.project_id,
    pod.planning_id,
    pod.purpose_id,
    pod.workorder_id,
    prov.province_name,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS pod_types,
    bd.brand AS pod_brand,
    ml.model AS pod_model,
    fn_get_date(pod.created_on) AS created_date,
    fn_get_date(pod.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.accessibility) AS pod_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.construction) AS pod_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pod.activation) AS pod_activation,
    ''::character varying AS additional_attributes,
    ''::text AS total_ports,
    ''::text AS entity_category,
    pod.pod_name AS entity_name,
    pod.acquire_from,
    pod.pod_type,
    pod.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    pod.source_ref_type,
    pod.source_ref_id,
    pod.source_ref_description,
    pod.status_remark,
    pod.status_updated_by,
    pod.status_updated_on,
    pod.is_visible_on_map,
    pod.is_new_entity,
    pod.remarks,
    pod.audit_item_master_id,
    pod.is_acquire_from,
    isp_room_info.network_id AS unit_network_id,
    s.shaft_name,
    f.floor_name,
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
    pod.gis_design_id,
    pod.st_x,
    pod.st_y,
    pod.ne_id,
    pod.prms_id,
    pod.jc_id,
    pod.mzone_id,
    pod.hierarchy_type,
    vm3.name AS own_vendor_name,
    pod.site_id,
    pod.site_name,
    pod.on_air_date,
    pod.removed_date,
    pod.tx_type,
    pod.tx_technology,
    pod.tx_segment,
    pod.tx_ring,
    pod.province,
    pod.district,
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
    pod.csr_count,
    pod.dti_circuit,
    pod.agg_01,
    pod.agg_02,
    pod.bandwidth,
    pod.ring_type,
    pod.link_id,
    pod.alias_name,
    pod.bh_status,
    (pod.site_id::text || '-'::text) || pod.site_name::text AS "(siteid-sitename)",
    COALESCE(pod.is_agg_site, false) AS is_agg_site,
    pod.project_category,
    pod.cable_plan_cores,
    pod.comment,
    pod.asbuit_osp_distance ,
    pod.asbuit_ibw_distance ,
    pod.asbuit_ibw_osp_total_distance 
   FROM att_details_pod pod
     JOIN province_boundary prov ON pod.province_id = prov.id
     JOIN region_boundary reg ON pod.region_id = reg.id
     JOIN user_master um ON pod.created_by = um.user_id
     JOIN vendor_master vm ON pod.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pod.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pod.own_vendor_id::integer = vm3.id
     LEFT JOIN isp_type_master tp ON pod.type = tp.id
     LEFT JOIN isp_brand_master bd ON pod.brand = bd.id
     LEFT JOIN isp_base_model ml ON pod.model = ml.id
     LEFT JOIN user_master um2 ON pod.modified_by = um2.user_id
     LEFT JOIN ( SELECT isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'POD'::text) m1 ON pod.system_id = m1.entity_id
     LEFT JOIN ( SELECT isp_entity_mapping.id,
            isp_entity_mapping.structure_id,
            isp_entity_mapping.shaft_id,
            isp_entity_mapping.floor_id,
            isp_entity_mapping.entity_type,
            isp_entity_mapping.entity_id,
            isp_entity_mapping.parent_id
           FROM isp_entity_mapping
          WHERE isp_entity_mapping.entity_type::text = 'UNIT'::text) m2 ON m1.parent_id = m2.id
     LEFT JOIN isp_room_info ON m2.entity_id = isp_room_info.system_id AND m2.floor_id = isp_room_info.floor_id
     LEFT JOIN isp_shaft_info s ON m2.shaft_id = s.system_id
     LEFT JOIN isp_floor_info f ON m2.floor_id = f.system_id;

--------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_update_network_code(entity_table text, entity_type text, source_ref_id_val text, source_ref_type_val text, update_entity_name text)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
    sql_stmt text;
begin
	if entity_type in ('Loop','Cable','Manhole','Pole') then 
	 sql_stmt := format($sql$
        UPDATE %I AS t
        SET
            network_id = sub.prefix || LPAD((sub.base_number + sub.rn)::text, 6, '0'),
            parent_system_id = 0,
            parent_network_id = sub.network_code,
            sequence_id = LPAD((sub.base_number + sub.rn)::text, 6, '0')::integer,
            parent_entity_type = sub.parent_entity_type
        FROM (
            SELECT
                t.system_id,
                regexp_replace(c.network_id, '\d', '', 'g') AS prefix,
                COALESCE(NULLIF(regexp_replace(c.network_id, '\D', '', 'g'), ''), '0')::int AS base_number,
                ROW_NUMBER() OVER (PARTITION BY t.province_id ORDER BY t.system_id) - 1 AS rn,
                c.network_code,
                c.parent_entity_type
            FROM %I t
            JOIN temp_generate_network_code c
                ON c.province_id = t.province_id
                AND c.entity_type = %L
            WHERE t.source_ref_id = %L
              AND t.source_ref_type = %L
        ) AS sub
        WHERE t.system_id = sub.system_id;
    $sql$,
    entity_table,
    entity_table,
    entity_type,
    source_ref_id_val,
    source_ref_type_val
    );
   
	else 
    -- Base update for network codes
    sql_stmt := format($sql$
        UPDATE %I AS t
        SET
            network_id = sub.prefix || LPAD((sub.base_number + sub.rn)::text, 6, '0'),
            %I = sub.prefix || LPAD((sub.base_number + sub.rn)::text, 6, '0'),
            parent_system_id = 0,
            parent_network_id = sub.network_code,
            sequence_id = LPAD((sub.base_number + sub.rn)::text, 6, '0')::integer,
            parent_entity_type = sub.parent_entity_type
        FROM (
            SELECT
                t.system_id,
                regexp_replace(c.network_id, '\d', '', 'g') AS prefix,
                COALESCE(NULLIF(regexp_replace(c.network_id, '\D', '', 'g'), ''), '0')::int AS base_number,
                ROW_NUMBER() OVER (PARTITION BY t.province_id ORDER BY t.system_id) - 1 AS rn,
                c.network_code,
                c.parent_entity_type
            FROM %I t
            JOIN temp_generate_network_code c
                ON c.province_id = t.province_id
                AND c.entity_type = %L
            WHERE t.source_ref_id = %L
              AND t.source_ref_type = %L
        ) AS sub
        WHERE t.system_id = sub.system_id;
    $sql$,
    entity_table,
    update_entity_name,
    entity_table,
    entity_type,
    source_ref_id_val,
    source_ref_type_val
    );
end if;
    -- Append correct table update based on entity type
    IF entity_type IN ('Pole','Manhole','SpliceClosure') THEN
         sql_stmt := sql_stmt || format($sql$
            UPDATE point_master AS pm
            SET
                common_name = m.network_id,
                display_name = m.%I      -- parameter update_entity_name
            FROM %I AS m
            WHERE m.system_id = pm.system_id
              AND pm.entity_type = %L
              AND m.source_ref_id = %L
              AND m.source_ref_type = %L;
        $sql$,
        update_entity_name,
        entity_table,
        entity_type,
        source_ref_id_val,
        'backbone planning'
        );
    ELSIF entity_type IN ('Duct','Cable','Trench') THEN
       sql_stmt := sql_stmt || format($sql$
            UPDATE line_master AS pm
            SET
                common_name = m.network_id,
                display_name = m.%I       -- parameter update_entity_name
            FROM %I AS m
            WHERE m.system_id = pm.system_id
              AND pm.entity_type = %L
              AND m.source_ref_id = %L
              AND m.source_ref_type = %L;
        $sql$,
        update_entity_name,
        entity_table,
        entity_type,
        source_ref_id_val,
        'backbone planning'
        );
	ELSIF entity_type = 'Loop' THEN
         sql_stmt := sql_stmt || format($sql$
            UPDATE point_master AS pm
            SET
                common_name = m.network_id,
                display_name = m.network_id    
            FROM %I AS m
            WHERE m.system_id = pm.system_id
              AND pm.entity_type = %L
              AND m.source_ref_id = %L
              AND m.source_ref_type = %L;
        $sql$,        
        entity_table,
        entity_type,
        source_ref_id_val,
        'backbone planning'
        );
    END IF;

    -- Run everything in one go
    EXECUTE sql_stmt;
END;
$function$
;

-- Permissions
-----------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_delete_plan(p_plan_id integer, p_user_id integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
declare 

v_status boolean;
v_message character varying;
v_Asbuilt_count integer;
BEGIN
v_status:=true;
v_message:='Success';
v_Asbuilt_count:=0;

select SUM(count) into v_Asbuilt_count from (
select count(1) from att_details_cable where source_ref_id=''||p_plan_id||'' and lower(source_ref_type)='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')
union
select count(1) from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A'))row;

if(v_Asbuilt_count > 0)
then 
	v_status:=false;
	v_message:='Plan Can Not Be Deleted Because Some Entity is AsBuild !';
else

EXECUTE 'ALTER TABLE point_master DISABLE TRIGGER ALL';
EXECUTE 'ALTER TABLE line_master DISABLE TRIGGER ALL';
	----cable---
	delete from line_master where system_id in(
	select system_id from att_details_cable where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('cable');

	delete from att_details_cable where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';																								----duct---
	delete from line_master where system_id in(
	select system_id from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('duct');

	delete from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';	

	----trench---
	delete from line_master where system_id in(
	select system_id from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('trench');

	delete from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';	

	----trench---
	delete from line_master where system_id in(
	select system_id from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('spliceclosure');

	delete from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';

	----manhole---
	delete from line_master where system_id in(
	select system_id from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('manhole');

	delete from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';

	----pole---
	delete from point_master where system_id in(
	select system_id from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning') and 
	upper(entity_type)=upper('pole');

	delete from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning';

	-- delete from temp table
	delete from backbone_plan_network_details where plan_id =p_plan_id;

	--Plan deleted
	delete from backbone_plan_details where plan_id=p_plan_id;
	EXECUTE 'ALTER TABLE line_master ENABLE TRIGGER ALL';
    EXECUTE 'ALTER TABLE point_master ENABLE TRIGGER ALL';
											
	v_status=true;
	v_message= '[SI_OSP_GBL_GBL_GBL_289]'; 
end if;

return query select v_status,v_message::character varying;
END
$function$
;

------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_delete_sprout_site_network(p_plan_id integer, p_user_id integer, p_site_network_id character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
declare 

v_status boolean;
v_message character varying;
v_Asbuilt_count integer;
BEGIN
v_status:=true;
v_message:='Success';
v_Asbuilt_count:=0;

select SUM(count) into v_Asbuilt_count from (
select count(1) from att_details_cable where source_ref_id=''||p_plan_id||'' and lower(source_ref_type)='backbone planning' and upper(network_status)=upper('A') and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A') and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')and sprout_site_id = ''||p_site_network_id||''
union
select count(1) from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and upper(network_status)=upper('A')and sprout_site_id = ''||p_site_network_id||'')row;

if(v_Asbuilt_count > 0)
then 
	v_status:=false;
	v_message:='Plan Can Not Be Deleted Because Some Entity is AsBuild !';
else
EXECUTE 'ALTER TABLE point_master DISABLE TRIGGER ALL';
EXECUTE 'ALTER TABLE line_master DISABLE TRIGGER ALL';
	----cable---
	delete from line_master where system_id in(
	select system_id from att_details_cable where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('cable');

	delete from att_details_cable where source_ref_id=''||p_plan_id||'' 
   and source_ref_type='backboneplanning' and sprout_site_id = ''||p_site_network_id||'';	
   
  ----duct---
	delete from line_master where system_id in(
	select system_id from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('duct');

	delete from att_details_duct where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';	

	----trench---
	delete from line_master where system_id in(
	select system_id from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('trench');

	delete from att_details_trench where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';	

	----trench---
	delete from line_master where system_id in(
	select system_id from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('spliceclosure');

	delete from att_details_spliceclosure where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';

	----manhole---
	delete from line_master where system_id in(
	select system_id from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('manhole');

	delete from att_details_manhole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';

	----pole---
	delete from point_master where system_id in(
	select system_id from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'') and 
	upper(entity_type)=upper('pole');

	delete from att_details_pole where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and sprout_site_id = ''||p_site_network_id||'';

 -- delete from nearest site table
	delete from backbone_plan_nearest_site where plan_id =p_plan_id and network_id  = ''||p_site_network_id||'';

  delete from backbone_plan_network_details where plan_id =p_plan_id and site_network_id  = ''||p_site_network_id||'';
	EXECUTE 'ALTER TABLE line_master ENABLE TRIGGER ALL';
    EXECUTE 'ALTER TABLE point_master ENABLE TRIGGER ALL';
   
	v_status=true;
	v_message= 'The sprout network '|| p_site_network_id ||' has been deleted Successfully!'; 
end if;

return query select v_status,v_message::character varying;
END
$function$
;

-- Permissions
------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_update_sprout_network(p_linegeom character varying, p_systemid integer, p_plan_id integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_line_geom geometry;
    v_line_geom_length double precision;
	current_fraction double precision := 0.0;
	structure_location geometry;
	p_pole_span integer;
	p_manhole_span integer;
	p_backbone_fiber_type character varying;
	line_geom geometry;
	p_is_looprequired bool;
	p_looplength  double precision;
	step_fraction double precision;
	P_SITE_NETWORK_ID character varying;
    V_closet_point character varying;
    V_is_update bool;
   sp_route_length double precision;
   sp_totalroute_length double precision;
   p_nearest_pole_manhole geometry;
	p_threshold double precision;
	p_cable_drum_length double precision;
    p_cable_type character varying;
   v_entity_type character varying;
   v_geometry geometry;
   updated_line_geom geometry;
  p_sprout_cable_name character varying;
 p_loop_span double precision;
sublinestring geometry;
intersectpoint geometry;
begin
	
-- get all requied fields to use 	
select bpd.pole_distance ,bpd.manhole_distance,bpd.loop_length ,bpd.is_loop_required,bpd.threshold,bpd.cable_length,bpd.loop_span  into p_pole_span,p_manhole_span,p_looplength,p_is_looprequired,p_threshold,p_cable_drum_length ,p_loop_span from backbone_plan_details bpd where plan_id = p_plan_id;
			
select fibertype,network_id,intersect_line_geom,is_update 
into p_backbone_fiber_type,P_SITE_NETWORK_ID, V_closet_point,V_is_update 
from backbone_plan_nearest_site bpns where id = p_systemid;	
----------------------------------------------------------------------
 
	select site_id ||'-' || site_name into p_sprout_cable_name from att_details_pod 
    where network_id = P_SITE_NETWORK_ID limit 1;

 	UPDATE backbone_plan_nearest_site
    SET line_geom = p_linegeom
    WHERE id = p_systemid AND plan_id = p_plan_id;
   
   -- when update srout draft network so should delete previous record and process the draft network for newly re-update record
	if(V_is_update)
	then 
	delete from backbone_plan_network_details bpnd 
	where site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id;

	end if;
-------------------------------------------------------------------------
		
		WITH intersections AS (
		    SELECT 
		        ST_Intersection(
		            ST_Buffer(
		                ST_GeomFromText('LINESTRING(' || c2.line_geom || ')', 4326)::geography,
		                5  -- buffer in meters
		            )::geometry,
		            ST_GeomFromText('LINESTRING(' || c1.backbone_geometry || ')', 4326)
		        ) AS inter_geom,
		        ST_GeomFromText('LINESTRING(' || c1.backbone_geometry || ')', 4326) AS main_line,
		        ST_GeomFromText('LINESTRING(' || c2.line_geom || ')', 4326) AS b_line,
		        c2.id
		    FROM backbone_plan_details c1
		    JOIN backbone_plan_nearest_site c2
		      ON c1.plan_id = p_plan_id AND c2.plan_id = p_plan_id and c2.id =p_systemid 
		    WHERE ST_DWithin(
		        ST_GeomFromText('LINESTRING(' || c1.backbone_geometry || ')', 4326)::geography,
		        ST_GeomFromText('LINESTRING(' || c2.line_geom || ')', 4326)::geography,
		        5) 
		)
		SELECT
    CASE
        WHEN st_length(
            ST_LineSubstring(
                b_line,
                ST_LineLocatePoint(b_line, ST_endPoint(inter_geom)),
                1
            ),
            false
        ) >
        st_length(
            ST_LineSubstring(
                b_line,
                ST_LineLocatePoint(b_line, ST_startPoint(inter_geom)),
                1
            ),
            false
        )
        THEN
            ST_LineSubstring(
                b_line,
                ST_LineLocatePoint(b_line, ST_startPoint(inter_geom)),
                1
            )
        ELSE
            ST_LineSubstring(
                b_line,
                ST_LineLocatePoint(b_line, ST_endPoint(inter_geom)),
                1
            )
    END AS subline_after_intersection,

    CASE
        WHEN st_length(
            ST_LineSubstring(
                b_line,
                ST_LineLocatePoint(b_line, ST_endPoint(inter_geom)),
                1
            ),
            false
        ) >
        st_length(
            ST_LineSubstring(
                b_line,
                ST_LineLocatePoint(b_line, ST_startPoint(inter_geom)),
                1
            ),
            false
        )
        THEN ST_startPoint(inter_geom)
        ELSE ST_endPoint(inter_geom)
    END AS v_geometry

INTO sublinestring, v_geometry
FROM intersections;  

  if sublinestring is null OR ST_IsEmpty(sublinestring) then
	select ST_GeomFromText(concat('LINESTRING(', p_linegeom, ')'), 4326) into v_line_geom;
	v_geometry = ST_GeomFromText(
		    concat('POINT(', trim(split_part(V_closet_point, ',', 2)), ' ', trim(split_part(V_closet_point, ',', 1)), ')'),
		    4326
		);
   else
   v_line_geom = sublinestring;

  end if;
  
   select st_length(v_line_geom,false) into v_line_geom_length;  
  
-- Check case sprout line geom length is less then cable drum length or sprout line geom is less then manhole span length or sprout line geom is less then pole span length then create one cable,sc, manhole or pole (because this is not possible to create multiple cable, pole, manhole )

	IF (v_line_geom_length < p_cable_drum_length or v_line_geom_length < p_manhole_span   or v_line_geom_length < p_pole_span) THEN
 raise info 'v_line_geom_length%',v_line_geom_length;

	if NOT EXISTS (
	        SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_geometry)
	          AND entity_type = 'RestrictedArea')
	 then 
	v_entity_type ='Pole';
    p_cable_type := 'Overhead';
	else 
	v_entity_type ='Manhole';
     p_cable_type := 'Underground';
	end if;

	END IF;

    if v_entity_type in ('Pole','Manhole')
    then   

    IF EXISTS (select 1 FROM backbone_plan_network_details
    	WHERE st_within(sp_geometry,
            st_buffer_meters(v_geometry, 200))
            AND plan_id = p_plan_id  and planned_cable_entity ='BackBone Cable'
            and entity_type in ('Pole','Manhole'))
     then
     raise info 'v_entity_type%',v_entity_type;

        SELECT ST_AddPoint(v_line_geom,sp_geometry, 0)  into updated_line_geom       
		FROM backbone_plan_network_details bpn
		WHERE bpn.plan_id = p_plan_id
		  AND bpn.entity_type IN ('Pole','Manhole') and 
		  planned_cable_entity ='BackBone Cable'
		  AND ST_Within(bpn.sp_geometry, st_buffer_meters(v_geometry, 200))
	   ORDER BY ST_Distance(bpn.sp_geometry, v_geometry)
	   LIMIT 1;
	       raise info 'updated_line_geom%',st_astext(updated_line_geom);

    -- Insert Cable entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type, line_sp_geometry,
        planned_cable_entity, site_network_id,region_id ,province_id ,sp_geometry ,longitude ,latitude ,entity_name,cable_type 
    )
    VALUES (
        p_plan_id, 'Cable', p_user_id, p_backbone_fiber_type, updated_line_geom,
        'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(updated_line_geom), ST_X(ST_startPoint(updated_line_geom)),ST_Y(ST_startPoint(updated_line_geom)),p_sprout_cable_name,p_cable_type
    );
   else
	 
    -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type,
        loop_length, is_loop_required, planned_cable_entity, site_network_id,region_id ,province_id,sp_geometry ,longitude ,latitude  
    )
    VALUES (
        p_plan_id, v_entity_type,  p_user_id, p_backbone_fiber_type, 
        0, p_is_looprequired, 'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(v_line_geom), ST_X(ST_startPoint(v_line_geom)),ST_Y(ST_startPoint(v_line_geom)));

     -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type,
         planned_cable_entity, site_network_id,region_id ,province_id ,sp_geometry ,longitude ,latitude 
    )
    VALUES (
        p_plan_id, 'SpliceClosure', p_user_id, p_backbone_fiber_type,
         'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(v_line_geom), ST_X(ST_startPoint(v_line_geom)),ST_Y(ST_startPoint(v_line_geom))
    );

      -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type, line_sp_geometry,
        planned_cable_entity, site_network_id,region_id ,province_id ,sp_geometry ,longitude ,latitude ,entity_name,cable_type
    )
    VALUES (
        p_plan_id, 'Cable', p_user_id, p_backbone_fiber_type, v_line_geom,
        'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(v_line_geom), ST_X(ST_startPoint(v_line_geom)),ST_Y(ST_startPoint(v_line_geom)),
        p_sprout_cable_name,p_cable_type
    );
   END IF;
  
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
   into sp_route_length from backbone_plan_network_details
   where site_network_id = P_SITE_NETWORK_ID and entity_type = 'Cable' and 
 	plan_id = P_plan_id and line_sp_geometry is not null;
 
   -- total sprout cable 
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where entity_type = 'Cable' and plan_id = p_plan_id;
   
   UPDATE backbone_plan_network_details AS np
SET province_id = pm.id,
    region_id   = rm.id
FROM region_boundary AS rm,
     province_boundary AS pm
WHERE rm.isvisible = TRUE
  AND pm.isvisible = TRUE
  AND ST_Intersects(
         pm.sp_geometry,
         np.sp_geometry 
     )
  AND ST_Intersects(
         rm.sp_geometry,
          np.sp_geometry 
     )
     and np.planned_cable_entity ='Sprout Cable'
  AND np.plan_id  = P_plan_id
  AND np.created_by = p_user_id and np.sp_geometry is not null and site_network_id = P_SITE_NETWORK_ID;
 
  perform (fn_backbone_draft_update_loop_network(p_linegeom, P_plan_id, p_is_looprequired,p_user_id, p_loop_span,p_looplength,P_SITE_NETWORK_ID));
 
  RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;
return;

	
END IF;
  
      WHILE current_fraction < 1.0 LOOP
        -- Try pole first
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_pole_span / v_line_geom_length), 1.0));

        -- Check if it's in a restricted area
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));

            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,loop_length,is_loop_required  ,planned_cable_entity,site_network_id ,province_id ,region_id 
            ) VALUES (
                p_plan_id, 'Manhole', ST_X(ST_EndPoint(line_geom)),ST_Y(ST_EndPoint(line_geom)),
                ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction,0,p_is_looprequired,
                'Sprout Cable',P_SITE_NETWORK_ID,0,0
            );           
           
        else
        	line_geom = NULL;
            step_fraction := p_pole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));

            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,fraction ,planned_cable_entity,site_network_id ,province_id,region_id ,loop_length ,is_loop_required 
            ) 
            VALUES (p_plan_id, 'Pole', ST_X(ST_EndPoint(line_geom)) ,ST_Y(ST_EndPoint(line_geom)),ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,current_fraction,'Sprout Cable',P_SITE_NETWORK_ID,0,0,0,p_is_looprequired
            );

        END IF;

        -- Move to next step
        current_fraction := current_fraction + step_fraction;
        EXIT WHEN current_fraction >= 1.0;
    END LOOP;  
   
   	delete from backbone_plan_network_details where system_id =(
   select system_id from backbone_plan_network_details where plan_id = p_plan_id and 
   site_network_id =P_SITE_NETWORK_ID and entity_type in ('Manhole','Pole') order by system_id desc limit 1 );
  
   current_fraction := 0.0;

    WHILE current_fraction < 1.0 LOOP
       
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_cable_drum_length / v_line_geom_length), 1.0));         

        step_fraction := p_cable_drum_length / v_line_geom_length;
       
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
           

        -- cable type logic
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            p_cable_type := 'Underground';
           v_entity_type = 'Manhole';
        ELSE
            p_cable_type := 'Overhead';
           v_entity_type = 'Pole';
        END IF;
       
RAISE INFO 'line_geom = %', ST_AsText(line_geom);
RAISE INFO 'cable_type geom = %', p_cable_type;


        -- insert cable
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, created_by, fiber_type, fraction, planned_cable_entity, cable_type, line_sp_geometry, sp_geometry,site_network_id ,entity_name
        )
        VALUES (
            p_plan_id, 'Cable', p_user_id, p_backbone_fiber_type,
            current_fraction, 'Sprout Cable', p_cable_type, line_geom, ST_STARTPoint(line_geom),P_SITE_NETWORK_ID,p_sprout_cable_name
        );

        -- insert splice closure at segment end
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity,site_network_id 
        )
        VALUES (
            p_plan_id, 'SpliceClosure',
            ST_X(ST_STARTPoint(line_geom)), ST_Y(ST_STARTPoint(line_geom)),
            ST_STARTPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'Sprout Cable',P_SITE_NETWORK_ID
        );
       
        if not exists (SELECT 1 FROM backbone_plan_network_details
            WHERE plan_id = p_plan_id and ST_Intersects(sp_geometry, structure_location) and planned_cable_entity
='Sprout Cable' and site_network_id = P_SITE_NETWORK_ID AND entity_type = v_entity_type and sp_geometry is not null) 
              then 
		INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity,site_network_id ,loop_length ,is_loop_required 
        )
        VALUES (
            p_plan_id, v_entity_type , ST_X(ST_STARTPoint(line_geom)), ST_Y(ST_STARTPoint(line_geom)),
            ST_STARTPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'Sprout Cable',P_SITE_NETWORK_ID,0,p_is_looprequired
        );
       end if;
      
        current_fraction := current_fraction + step_fraction;

        EXIT WHEN current_fraction >= 1.0;
    END LOOP;
  /*
     IF EXISTS (select 1 FROM backbone_plan_network_details
    		WHERE st_within(sp_geometry,
            st_buffer_meters(v_geometry, 200))
            AND plan_id = p_plan_id  and planned_cable_entity ='BackBone Cable' and entity_type in ('Pole','Manhole'))
     then
     raise info 'test %',st_astext(v_geometry);

        SELECT sp_geometry into p_nearest_pole_manhole       
		FROM backbone_plan_network_details bpn
		WHERE bpn.plan_id = p_plan_id
		  AND bpn.entity_type  IN ('Pole','Manhole') and 
		  planned_cable_entity ='BackBone Cable'
		  AND st_within(sp_geometry,
            st_buffer_meters(v_geometry, 200))
	   ORDER BY ST_Distance(bpn.sp_geometry, v_geometry)
	   LIMIT 1;
	  RAISE INFO 'p_nearest_pole_manhole = %', ST_AsText(p_nearest_pole_manhole);

	 -- update  line geom if exists around pole, manhole	  	  
	  update backbone_plan_network_details set line_sp_geometry  = ST_AddPoint(line_sp_geometry,p_nearest_pole_manhole,0) 
	  where fraction = 0 and site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id
	  and entity_type  = 'Cable';
	
	  update backbone_plan_network_details set sp_geometry = v_geometry ,
	  latitude = ST_Y(v_geometry) , longitude = ST_X(v_geometry)
	  where fraction = 0 and site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id 
	  and entity_type = 'SpliceClosure';
	 	
	 
	  end if;
	  */
	 select sum(round(ST_Length(line_sp_geometry, false)::numeric,3)) into sp_route_length 
    from backbone_plan_network_details where site_network_id = P_SITE_NETWORK_ID and entity_type ='Cable'
    and plan_id = p_plan_id;
   
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where planned_cable_entity = 'Sprout Cable' and entity_type ='Cable' and plan_id = p_plan_id;
   
UPDATE backbone_plan_network_details AS np
SET province_id = pm.id,
    region_id   = rm.id
FROM region_boundary AS rm,
     province_boundary AS pm
WHERE rm.isvisible = TRUE
  AND pm.isvisible = TRUE
  AND ST_Intersects(
         pm.sp_geometry,
         np.sp_geometry 
     )
  AND ST_Intersects(
         rm.sp_geometry,
          np.sp_geometry 
     )
  and np.planned_cable_entity ='Sprout Cable'
  AND np.plan_id  = P_plan_id
  AND np.created_by = p_user_id and np.sp_geometry is not null;

  perform (fn_backbone_draft_update_loop_network(p_linegeom, P_plan_id, p_is_looprequired,p_user_id, p_loop_span,p_looplength,P_SITE_NETWORK_ID));
 
 RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;

END;
$function$
;

-----------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_backbone_draft_network(p_planid integer, p_userid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
   rec record; 
  v_site_geom record;
    v_sprout_intersect_geom geometry;
begin
   
DELETE FROM backbone_plan_network_details 
WHERE planned_cable_entity  = 'Sprout Cable'
  AND plan_id = p_planid ;

	
  FOR v_site_geom IN (SELECT distinct
  pm.longitude , pm.latitude,pm.sp_geometry  ,pm.common_name,bpns.fibertype,bpns.line_geom ,pm.system_id , bpns.id 
  FROM point_master pm 
  join att_details_pod pod on pm.system_id = pod.system_id and pm.entity_type = 'POD' 
  left join backbone_plan_nearest_site bpns on bpns.network_id = pod.network_id 
	  WHERE bpns.plan_id = p_planId and bpns.user_id = p_userid)
	loop
	
		SELECT ST_ClosestPoint(line.line_sp_geometry, v_site_geom.sp_geometry) 
		AS nearest_point INTO v_sprout_intersect_geom
		FROM backbone_plan_network_details line
		WHERE plan_id = p_planId
		ORDER BY ST_Distance(line.line_sp_geometry, v_site_geom.sp_geometry)
		LIMIT 1;

	UPDATE backbone_plan_nearest_site
	SET 
	    intersect_line_geom = 
	        round(ST_y(v_sprout_intersect_geom)::numeric, 6)::character varying  || ',' || 
	        round(ST_x(v_sprout_intersect_geom)::numeric, 6)::character varying ,
	       site_geom = round(ST_Y(v_site_geom.sp_geometry)::numeric, 6)::character varying  || ',' || round(ST_X(v_site_geom.sp_geometry)::numeric, 6)::character varying 
	WHERE id = v_site_geom.id 
	RETURNING id, plan_id, intersect_line_geom INTO rec;

	
	WITH threshold_closest_line AS (
    SELECT
       -- system_id,
        line_sp_geometry,
        sp_geometry,
        ST_DistanceSphere(
            sp_geometry,
            v_sprout_intersect_geom
        ) AS distance_meters
    FROM
        backbone_plan_network_details
    WHERE
        st_within(
            sp_geometry,
            st_buffer_meters(v_sprout_intersect_geom, (select threshold from backbone_plan_details bpd where plan_id = p_planId and created_by = p_userid and entity_type in ('Manhole','Pole')))
        )
        AND plan_id = p_planId
    ORDER BY
        distance_meters ASC
    LIMIT 1
)
UPDATE backbone_plan_nearest_site target
SET intersect_line_geom =  
    round(ST_Y(threshold_closest_line.sp_geometry)::numeric, 6)::varchar || ',' || 
    round(ST_X(threshold_closest_line.sp_geometry)::numeric, 6)::varchar
FROM threshold_closest_line
WHERE target.id = rec.id;

IF NOT EXISTS (
    select  1
    FROM
        backbone_plan_network_details
    WHERE
        st_within(
            sp_geometry,
            st_buffer_meters(v_sprout_intersect_geom, (select threshold from backbone_plan_details bpd where plan_id = p_planId and created_by = p_userid))
        )
        AND plan_id = p_planId  
) THEN
    UPDATE backbone_plan_nearest_site
    SET intersect_line_geom = round(ST_y(v_sprout_intersect_geom)::numeric, 6)::character varying  || ',' || 
	        round(ST_x(v_sprout_intersect_geom)::numeric, 6)::character varying,
	        sp_geometry = v_sprout_intersect_geom
    WHERE id = rec.id;
END IF;

END LOOP;

  if exists (select 1 from backbone_plan_nearest_site 
        where plan_id = p_planid and user_id = p_userId)
    then
  -- Return as JSON
    RETURN QUERY 
    SELECT row_to_json(t)
    FROM (
        SELECT id,plan_id, fibertype as fiber_type, intersect_line_geom, site_geom,ST_AsText(line_sp_geometry) as site_to_nearestcable_line_geom 
        FROM backbone_plan_nearest_site 
        WHERE user_id = p_userId and plan_id = p_planId 
    	) AS t;
   
  end if;   
END;
$function$
;

-- Permissions

---------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_draft_network(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, p_backbone_fiber_type character varying, p_pole_span double precision, p_manhole_span double precision, v_buffer double precision, p_threshold double precision, p_looplength double precision, p_is_looprequired boolean, cable_drum_length double precision, p_loop_span double precision)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_plan_id INTEGER;
    v_line_geom geometry;
    v_line_geom_length double precision;
    current_fraction double precision := 0.0;
    structure_location geometry;
    step_fraction double precision;
    line_geom geometry;
    manhole_fraction double precision;
    p_cable_type character varying;
    p_entity_type character varying;
BEGIN
    -- Create plan entry
    INSERT INTO backbone_plan_details (
        plan_name, start_point, end_point, is_create_trench, is_create_duct, 
        pole_distance, created_by, backbone_fiber_type, 
        threshold, buffer, is_loop_required, loop_length, manhole_distance, backbone_geometry,cable_length ,loop_span 
    )
    VALUES (
        p_plan_name, p_startpoint, p_endpoint, is_create_trench, is_create_duct,
        p_pole_span, p_user_id, p_backbone_fiber_type,
        p_threshold, v_buffer, p_is_looprequired, COALESCE(p_looplength,0),
        p_manhole_span, p_line_geom,cable_drum_length,p_loop_span
    )
    RETURNING plan_id INTO v_plan_id;

    -- Convert geometry
    SELECT ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326) INTO v_line_geom;
    SELECT ST_Length(v_line_geom::geography) INTO v_line_geom_length;

    -------------------------------------------------------------------
    -- Structure placement (skip first + last point)
    -------------------------------------------------------------------
     WHILE current_fraction < 1.0 LOOP
        -- Try pole first
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_pole_span / v_line_geom_length), 1.0));

        -- Check if it's in a restricted area
        IF NOT EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            step_fraction := p_pole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
         
            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,fraction,planned_cable_entity ,is_loop_required ,loop_length 
            ) VALUES (
                v_plan_id, 'Pole', ST_X(ST_EndPoint(line_geom)),  ST_Y(ST_EndPoint(line_geom)) , ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,current_fraction,'BackBone Cable',p_is_looprequired,0
            );           
           
        else
        	line_geom = NULL;
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
          
            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,loop_length ,planned_cable_entity , is_loop_required
            ) VALUES (
                v_plan_id, 'Manhole', ST_X(ST_EndPoint(line_geom)), ST_Y(ST_EndPoint(line_geom)), ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction,0,'BackBone Cable',p_is_looprequired
            );

        END IF;

        -- Move to next step
        current_fraction := current_fraction + step_fraction;

        -- Exit safety
        EXIT WHEN current_fraction >= 1.0;
    END LOOP; 
   
    delete from backbone_plan_network_details where system_id = (select system_id from backbone_plan_network_details where plan_id = v_plan_id and entity_type in ('Pole','Manhole') order by system_id desc limit 1);
   
 --	update backbone_plan_network_details set is_loop_required = false where system_id =
  --  (select system_id from backbone_plan_network_details where plan_id = v_plan_id and entity_type in ('Pole','Manhole')
   -- order by system_id desc limit 1);
    -------------------------------------------------------------------
    -- Add SpliceClosure at start
    -------------------------------------------------------------------
  /*  INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity
    )
    VALUES (
        v_plan_id, 'SpliceClosure',
        ST_X(ST_StartPoint(v_line_geom)), ST_Y(ST_StartPoint(v_line_geom)),
        ST_StartPoint(v_line_geom), p_user_id, p_backbone_fiber_type, 0.0, 'BackBone Cable'
    );*/

    -------------------------------------------------------------------
    -- Cable placement
    -------------------------------------------------------------------
    current_fraction := 0.0;

    WHILE current_fraction < 1.0 LOOP
        step_fraction := cable_drum_length / v_line_geom_length;

        -- Prevent overshoot
        IF current_fraction + step_fraction > 1.0 THEN
            step_fraction := 1.0 - current_fraction;
        END IF;

        line_geom := ST_LineSubstring(v_line_geom, current_fraction, current_fraction + step_fraction);

        structure_location := ST_LineInterpolatePoint(v_line_geom, current_fraction + step_fraction);

        -- cable type logic
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            p_cable_type := 'Underground';
            p_entity_type = 'Manhole';
        ELSE
            p_cable_type := 'Overhead';
            p_entity_type = 'Pole';
        END IF;

        -- insert cable
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, created_by, fiber_type, fraction, planned_cable_entity, cable_type, line_sp_geometry, sp_geometry,entity_name 
        )
        VALUES (
            v_plan_id, 'Cable', p_user_id, p_backbone_fiber_type,
            current_fraction, 'BackBone Cable', p_cable_type, line_geom, ST_EndPoint(line_geom),p_plan_name
        );

        -- insert splice closure at segment end
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity
        )
        VALUES (
            v_plan_id, 'SpliceClosure',
            ST_X(ST_EndPoint(line_geom)), ST_Y(ST_EndPoint(line_geom)),
            ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'BackBone Cable'
        );
       
       if not exists (SELECT 1 FROM backbone_plan_network_details
            WHERE plan_id = v_plan_id and ST_Intersects(sp_geometry, structure_location)
              AND entity_type = p_entity_type and sp_geometry is not null) 
              then 
 		INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity,loop_length ,is_loop_required 
        )
        VALUES (
            v_plan_id, p_entity_type,
            ST_X(ST_EndPoint(line_geom)), ST_Y(ST_EndPoint(line_geom)),
            ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'BackBone Cable', 0,p_is_looprequired
        );
       end if;
      
        current_fraction := current_fraction + step_fraction;

        EXIT WHEN current_fraction >= 1.0;
    END LOOP;
   
delete from backbone_plan_network_details where system_id = (select system_id from backbone_plan_network_details where plan_id = v_plan_id and entity_type in ('Pole','Manhole') order by system_id desc limit 1);
  
delete from backbone_plan_network_details where system_id = (select system_id from backbone_plan_network_details where plan_id = v_plan_id and entity_type in ('SpliceClosure') order by system_id desc limit 1);
   

 --  update backbone_plan_network_details set is_loop_required = false where system_id =
 --  (select system_id from backbone_plan_network_details where plan_id = v_plan_id and entity_type in ('Pole','Manhole')
 -- order by system_id desc limit 1);
   
perform (fn_backbone_draft_update_loop_network(p_line_geom, v_plan_id, p_is_looprequired,p_user_id, p_loop_span,p_looplength,''));

 -------------------------------------------------------------------
    -- Update region + province
    -------------------------------------------------------------------
    UPDATE backbone_plan_network_details AS np
    SET province_id = pm.id,
        region_id   = rm.id
    FROM region_boundary AS rm,
         province_boundary AS pm
    WHERE rm.isvisible = TRUE
      AND pm.isvisible = TRUE
      AND ST_Intersects(pm.sp_geometry, np.sp_geometry)
      AND ST_Intersects(rm.sp_geometry, np.sp_geometry)
      AND np.plan_id = v_plan_id
      AND np.created_by = p_user_id
      AND np.sp_geometry IS NOT NULL;

    RETURN QUERY
    SELECT row_to_json(t)
    FROM (SELECT v_plan_id AS plan_id) AS t;
END;
$function$
;

-------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_save_plan(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, backbone_plan_id integer)
 RETURNS TABLE(status boolean, message character varying, v_plan_id integer)
 LANGUAGE plpgsql
AS $function$

DECLARE
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow_cable record;
v_line_geom geometry;
v_region_province record;
v_arow_closure record;
v_arow_prev_closure record;
v_line_total_length DOUBLE PRECISION;
p_rfs_type character varying;
--backbone_plan_id integer;
v_temp_point_entity record;
v_geom_str character varying;
p_cable_type character varying;
p_skip_last_sc integer;
p_skip_last_manhole_pole integer;
v_geom geometry;
v_result TEXT;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';

--truncate table temp_generate_network_code;
--truncate table temp_point_geom;

create temp table temp_generate_network_code(
status boolean,
network_id character varying,
entity_type character varying,
sequence_id integer,
province_id integer,
network_code character varying,
parent_entity_type character varying
) on commit drop;

create temp table temp_point_geom(
entity_type character varying,
sp_geometry character varying
) on commit drop;

select system_id into p_skip_last_manhole_pole from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole','Pole') and planned_cable_entity = 'BackBone Cable' order by system_id desc limit 1;

select system_id into p_skip_last_sc from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('SpliceClosure') and planned_cable_entity = 'BackBone Cable' order by system_id desc limit 1;
EXECUTE 'ALTER TABLE point_master DISABLE TRIGGER ALL';

----- insert manhole
INSERT INTO att_details_manhole(manhole_name ,is_visible_on_map,ownership_type,
longitude, latitude,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status,
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,category,backbone_system_id,sprout_site_id)

SELECT entity_name ,true,'Own',
longitude,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id,region_id ,province_id,'Manhole' ,system_id,site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and sp_geometry is not NULL order by system_id;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'Manhole',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.manhole_name ,backbone_plan_id,'backbone planning'
from att_details_manhole mh
left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='Manhole' where pm.system_id is null;

UPDATE att_details_manhole adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'Manhole'
AND specification = 'Generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

---------insert sc

INSERT INTO att_details_spliceclosure (is_visible_on_map,ownership_type,
longitude, latitude,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status,
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,category,backbone_system_id,sprout_site_id)

SELECT true,'Own',
longitude ,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id,region_id ,province_id ,'SpliceClosure',system_id,site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('SpliceClosure') and sp_geometry is not NULL order by system_id;


INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'SpliceClosure',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.spliceclosure_name ,backbone_plan_id,'backbone planning'
from att_details_spliceclosure mh
left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='SpliceClosure' where pm.system_id is null;


UPDATE att_details_spliceclosure adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id,
no_of_ports = itm.no_of_port,
no_of_input_port= itm.no_of_input_port,
no_of_output_port = itm.no_of_output_port
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'SpliceClosure'
AND specification = 'generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

-----------insert pole

INSERT INTO att_details_pole(pole_name ,is_visible_on_map,ownership_type,
longitude, latitude,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status,
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,pole_type,category,backbone_system_id,sprout_site_id)

SELECT entity_name ,true,'Own',
longitude ,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id::varchar,region_id ,province_id ,'Electrical','Pole',system_id,site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Pole') and sp_geometry is not NULL order by system_id;


INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'Pole',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.pole_name ,backbone_plan_id::varchar,'backbone planning'
from att_details_pole mh
left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='Pole' where pm.system_id is null;


UPDATE att_details_pole adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'Pole'
AND specification = 'Generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

----------- insert cable
INSERT INTO ATT_DETAILS_CABLE(cable_name ,is_visible_on_map,total_core,no_of_tube,no_of_core_per_tube,cable_measured_length,cable_calculated_length,
cable_type,specification,category,subcategory1,subcategory2,subcategory3,item_code,
vendor_id,network_status,status,created_by,created_on,cable_category,start_reading,
end_reading,structure_id,utilization,duct_id,trench_id,is_used,source_ref_type,source_ref_id,ownership_type,region_id,province_id,backbone_system_id,a_system_id,b_system_id,sprout_site_id)
select entity_name,true,0,0,0,ST_Length(plan.line_sp_geometry,false),
ST_Length(plan.line_sp_geometry,false),plan.cable_type ,plan.fiber_type,'Cable', null, null, null,null,
0,'P','A',p_user_id,now(),case when plan.planned_cable_entity = 'BackBone Cable' then 'BackBone Cable' else 'Sprout Cable' end,
0,0,0,'L',0,0,true,'backbone planning',backbone_plan_id,'OWN',region_id,province_id , plan.system_id,0,0,plan.site_network_id 
from backbone_plan_network_details plan where plan.plan_id = backbone_plan_id and plan.created_by = p_user_id and plan.entity_type = 'Cable' and plan.line_sp_geometry is not null order by system_id;

EXECUTE 'ALTER TABLE line_master DISABLE TRIGGER ALL';

INSERT INTO LINE_MASTER(system_id,common_name ,display_name ,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,network_status,is_virtual,source_ref_type,source_ref_id,sp_geometry,status)
select cbl.system_id,cbl.network_id,cbl.cable_name ,'Cable','A','','', p_user_id,0 ,'P',false,'backbone planning',backbone_plan_id , np.line_sp_geometry ,'A'
from ATT_DETAILS_CABLE cbl
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id
where cbl.system_id not in (select system_id from line_master where entity_type='Cable') and np.entity_type = 'Cable' and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning' and np.line_sp_geometry is not null;

UPDATE att_details_cable adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id,
total_core = itm.other::integer,
no_of_tube = itm.no_of_tube,
no_of_core_per_tube = itm.no_of_core_per_tube
from
(select adm.system_id ,itm.* FROM att_details_cable adm
join item_template_master itm
on category_reference = 'Cable'
AND '(' || code || ')' || other = adm.specification
where adm.source_ref_id = backbone_plan_id::varchar
AND adm.source_ref_type = 'backbone planning'
) itm
WHERE adm.system_id = itm.system_id;

----------- insert trench
if is_create_trench = true then
insert into att_details_trench(is_visible_on_map,trench_name,trench_width,trench_height,customer_count,trench_type,no_of_ducts,network_status,status,pin_code,province_id,region_id,construction,activation,accessibility,category,subcategory1,subcategory2,subcategory3,vendor_id,type,
brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,mcgm_ward,strata_type,
is_used,source_ref_type,source_ref_id,ownership_type,backbone_system_id,a_system_id,b_system_id,sprout_site_id)
select true,'',0,0,0,'HDD',0,'P','A','',province_id,region_id,0,0,0,
'Trench' ,null,null,null, 0,0,0,0,'',p_user_id,now(),0 ,0,0,0,'','',false,'backbone planning',backbone_plan_id,'OWN',tp.system_id ,0,0,tp.site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and tp.line_sp_geometry is not null and created_by = p_user_id;

INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name,source_ref_id,source_ref_type,sp_geometry,status)
select cbl.system_id,'Trench','A','','', p_user_id,0 ,'','P',false,'',backbone_plan_id,'backbone planning',np.line_sp_geometry,'A'
from att_details_trench cbl
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id where cbl.system_id not in (select system_id from line_master where entity_type='Trench') and np.entity_type in ('Manhole') and np.line_sp_geometry is not null and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

UPDATE att_details_trench adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'Trench'
AND specification = 'generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

end if;

-------- insert duct
if is_create_duct = true then
insert into att_details_duct(is_visible_on_map,duct_name,calculated_length,manual_length,network_status,
status,pin_code,province_id,region_id,utilization,no_of_cables,offset_value,construction,activation,accessibility,specification,category,subcategory1,subcategory2,
subcategory3,item_code,vendor_id,type,brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,inner_dimension,outer_dimension,
is_used,source_ref_type,source_ref_id,trench_id,ownership_type,backbone_system_id,a_system_id,b_system_id,sprout_site_id)
select true,'',ST_Length(tp.line_sp_geometry,false),ST_Length(tp.line_sp_geometry,false),'P','A','',province_id,region_id,'L',0,0,0,0,0,'','Duct',null,null,null,'',0,0,0,0,'',p_user_id,now(),0 ,0,0,0,0,0,false,'backbone planning',backbone_plan_id,0,'OWN',tp.system_id ,0,0,tp.site_network_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and created_by = p_user_id;

INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name,source_ref_id,source_ref_type,sp_geometry,status)
select cbl.system_id,'Duct','A','','', p_user_id,0 ,'','P',false,'',backbone_plan_id,'backbone planning' ,np.line_sp_geometry ,'A'
from att_details_duct cbl
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id where cbl.system_id not in (select system_id from line_master where entity_type='Duct') and np.entity_type ='Manhole' and np.line_sp_geometry is not null and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

UPDATE att_details_duct adm
SET
specification = itm.specification,
item_code = itm.code,
audit_item_master_id = itm.audit_id,
vendor_id = itm.vendor_id
FROM (
SELECT *
FROM item_template_master
WHERE category_reference = 'Duct'
AND specification = 'generic'
LIMIT 1
) itm
WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';
end if;

FOR v_temp_point_entity IN
SELECT province_id
FROM backbone_plan_network_details
WHERE plan_id = backbone_plan_id
AND created_by = p_user_id
AND province_id IS NOT NULL
GROUP BY province_id
loop

INSERT INTO temp_point_geom(entity_type, sp_geometry)
SELECT 'Manhole', geom_text FROM (
SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type = 'Manhole'
-- and system_id not in (p_skip_last_manhole_pole)
AND sp_geometry IS NOT NULL
LIMIT 1
) AS manhole_geom

UNION ALL

SELECT 'Pole', geom_text FROM (
SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type = 'Pole'
--and system_id not in (p_skip_last_manhole_pole)
AND sp_geometry IS NOT NULL
LIMIT 1
) AS pole_geom

UNION ALL

SELECT 'Loop', geom_text FROM (
SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id and is_loop_required = true and loop_length > 0
AND entity_type in ('Manhole','Pole')
-- and system_id not in (p_skip_last_manhole_pole)
AND sp_geometry IS NOT NULL
LIMIT 1
) AS loop_geom

UNION ALL

SELECT 'SpliceClosure', geom_text FROM (
SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type IN ('SpliceClosure')
-- and system_id not in (p_skip_last_sc)
AND sp_geometry IS NOT NULL
LIMIT 1
) AS spliceclosure_geom;


--- generaTE NETWORK CODE FOR POINT ENTITY
INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id, network_code, parent_entity_type)
SELECT f.status, f.message, t.entity_type, f.o_sequence_id, v_temp_point_entity.province_id, f.o_p_network_id, f.o_p_entity_type
FROM temp_point_geom t,
LATERAL fn_get_clone_network_code(
t.entity_type,
'Point',
t.sp_geometry,
0,
''
) AS f
WHERE t.sp_geometry IS NOT NULL;

delete from temp_generate_network_code cc where cc.status = false;

-------------------update network id
PERFORM fn_backbone_update_network_code('att_details_manhole', 'Manhole',backbone_plan_id::text , 'backbone planning','manhole_name');
PERFORM fn_backbone_update_network_code('att_details_pole', 'Pole',backbone_plan_id::text, 'backbone planning', 'pole_name');
PERFORM fn_backbone_update_network_code('att_details_spliceclosure', 'SpliceClosure',backbone_plan_id::text, 'backbone planning', 'spliceclosure_name' );

---- generate network code cable

SELECT line_sp_geometry
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type in ('Cable')
AND line_sp_geometry IS NOT NULL
LIMIT 1;

if v_geom is not null
then
select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

if not exists (select 1 from temp_generate_network_code where province_id = v_temp_point_entity.province_id and entity_type = 'Cable') 
then 
INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Cable', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Cable','','',v_geom_str,'OSP') AS f;

PERFORM fn_backbone_update_network_code('att_details_cable', 'Cable', backbone_plan_id::text, 'backbone planning','cable_name');

end if;


end if;

--- generate network codes for trench and duct
if is_create_trench = true then
SELECT line_sp_geometry
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type in ('Manhole')
AND line_sp_geometry IS NOT NULL
LIMIT 1;

if v_geom is not null
then
select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

if not exists (select 1 from temp_generate_network_code where province_id = v_temp_point_entity.province_id and entity_type = 'Trench') 
then 
INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Trench', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Trench','','',v_geom_str,'OSP'::character varying,0,'') as f;

PERFORM fn_backbone_update_network_code('att_details_trench', 'Trench', backbone_plan_id::text, 'backbone planning','trench_name');
end if;

end if;
end if;

if is_create_duct = true
then
SELECT line_sp_geometry
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
AND plan_id = backbone_plan_id
AND created_by = p_user_id
AND entity_type in ('Manhole')
AND line_sp_geometry IS NOT NULL
LIMIT 1;

if v_geom is not null
then
select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

if not exists (select 1 from temp_generate_network_code where province_id = v_temp_point_entity.province_id and entity_type = 'Trench') 
then 
INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Duct', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Duct','','',v_geom_str,'OSP'::character varying,0,'') as f;

PERFORM fn_backbone_update_network_code('att_details_duct', 'Duct', backbone_plan_id::text, 'backbone planning','duct_name');
end if;

end if;
end if;

END LOOP;


INSERT INTO routing_data_core_plan (id, system_id, source, target, cost, reverse_cost, geom)
SELECT
lm.system_id ,lm.system_id,null as source,null as target,
ST_Length(lm.sp_geometry::geography) / 1000 AS cost,
ST_Length(lm.sp_geometry::geography) / 1000 AS reverse_cost,
ST_Force2D(lm.sp_geometry) AS geom
FROM line_master lm
WHERE lm.entity_type = 'Cable'
AND lm.source_ref_id = backbone_plan_id::varchar and lm.source_ref_type ='backbone planning';

SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
IF v_result <> 'OK' THEN
RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for INSERT' AS message;
END IF;
SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
IF v_result <> 'OK' THEN
RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for INSERT' AS message;
END IF;

PERFORM(fn_backbone_get_loop_entity(backbone_plan_id));

PERFORM fn_set_cable_color_info(c.system_id, p_user_id, c.no_of_tube, c.no_of_core_per_tube)
FROM att_details_cable c
WHERE c.source_ref_id = backbone_plan_id::varchar
AND c.source_ref_type = 'backbone planning';

EXECUTE 'ALTER TABLE line_master ENABLE TRIGGER ALL';
EXECUTE 'ALTER TABLE point_master ENABLE TRIGGER ALL';

V_MESSAGE:='BackBone network plan processed successfully!';
update backbone_plan_details t set status = 'Completed' where t.plan_id = backbone_plan_id and created_by = p_user_id;

RETURN QUERY SELECT V_IS_VALID::boolean AS STATUS, V_MESSAGE::CHARACTER VARYING AS MESSAGE,backbone_plan_id :: integer as v_plan_id ;

end;
$function$
;

----------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_network_planning_get_plan_bom_list(p_plan_name character varying, p_plan_mode character varying, p_cable_type character varying, is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_cable_length double precision, p_distance double precision, p_user_id integer, p_temp_plan_id integer, p_is_loop_require boolean, p_is_loop_update boolean, p_loop_length double precision, p_polepecvendor character varying, p_manholepecvendor character varying, p_scspecvendor character varying, p_loop_span double precision)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
sql text;
v_line_total_length DOUBLE PRECISION;
v_point_entity character varying;
v_line_geom geometry;
v_total_point_entity integer;
v_is_osp_tp boolean;
v_total_line integer;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_cable_length double precision;
v_end_entity_count integer;
v_middle_entity_count integer; 
v_arrow_point record;
v_loop_length double precision;
v_outer_query text;
v_temp_planid integer;
p_site_id character varying;
is_a_endpoint_entity_exists bool;
is_b_endpoint_entity_exists bool;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
sql:='';
v_total_line:=0;
v_temp_planid:=0;

p_scspecvendor = case when p_scspecvendor is null then '' else p_scspecvendor end;
p_manholepecvendor = case when p_manholepecvendor is null then '' else p_manholepecvendor end;

if(p_line_geom is null or p_line_geom = '') 
then 

SELECT substring(st_astext(line_sp_geometry), 12, length(st_astext(line_sp_geometry)) - 12) INTO p_line_geom from temp_auto_network_plan where plan_id = p_temp_plan_id and line_sp_geometry is not null
limit 1;

end if;

p_polepecvendor = case when p_polepecvendor is null then '' else p_polepecvendor end;

if(p_is_loop_update)
then
v_temp_planid:= p_temp_plan_id;
select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;

select count(plan_id)into v_total_point_entity from temp_auto_network_plan where plan_id=v_temp_planid and loop_length >0;

sql:=sql || 'select '||v_loop_length||' as length_qty,''Loop''::character varying as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
	raise info 'ssdfasdfql=%',sql;	
else

if(upper(p_cable_type)=upper('overhead'))
then
	v_point_entity:='Pole';
	else
	v_point_entity:='Manhole';
end if;

 
 raise info 'p_site_id =%',p_site_id;
	
select is_osp_tp into v_is_osp_tp from vw_termination_point_master where tp_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure')and layer_id=(select layer_id from layer_details where layer_name='Cable');

  select validation.status,validation.message from  fn_network_planning_validate(p_plan_mode,p_cable_type,is_create_trench,
  is_create_duct,v_is_osp_tp,p_cable_length,v_point_entity,p_user_id,0,p_line_geom) as validation into V_IS_VALID,V_MESSAGE;

 IF(V_IS_VALID)
THEN

-- creating points entity according to line geom;
select * from fn_temp_network_planning(p_plan_mode,p_cable_type,is_create_trench,is_create_duct,p_line_geom,p_cable_length,p_distance,p_user_id,p_plan_name,'', '', '', 0, '', 0, '',p_temp_plan_id)into v_arrow_point;

v_temp_planid:=v_arrow_point.temp_plan_id;


-- count of middle entity
select count(plan_id)into v_middle_entity_count from temp_auto_network_plan where plan_id=v_arrow_point.temp_plan_id and is_middle_point='true';

-- end entity
select count(plan_id) into v_end_entity_count from temp_auto_network_plan where plan_id=v_arrow_point.temp_plan_id and is_middle_point='false';

-- total count of point entity
v_total_point_entity:=v_middle_entity_count+v_end_entity_count;

 select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
 select round(st_length(v_line_geom,false)::numeric, 6)::double precision into v_line_total_length;

-- get counts of cable 
 if(v_line_total_length<p_cable_length)
 then
 	v_total_line:=1;
 else
 	select CEIL(v_line_total_length/p_cable_length)::integer into v_total_line;		
 end if;

-- for cable
sql := 'select '||v_line_total_length||' as length_qty,* from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';

if(is_create_trench) 
then 
	sql:=sql || ' union select '||v_line_total_length||' as length_qty,* from 
	fn_network_planning_audit_template_detail(''trench'','||p_user_id||')';
end if;

if(is_create_duct) 
then 
	sql:=sql || ' union select '||v_line_total_length||' as length_qty,* from fn_network_planning_audit_template_detail(''duct'','||p_user_id||')';
end if;
 

if(upper(p_plan_mode)=upper('auto'))
then
	--sql:=sql || ' union select '||v_total_point_entity||' as length_qty,* from fn_network_planning_audit_template_detail('''||v_point_entity||''','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';	
sql := sql || ' UNION SELECT ' || v_total_point_entity || ' AS length_qty, * 
FROM fn_network_planning_audit_template_detail('
|| quote_literal(v_point_entity) || ',' 
|| p_user_id || ',' 
|| quote_literal(p_polepecvendor) || ',' 
|| quote_literal(p_manholepecvendor) || ',' 
|| quote_literal(p_scspecvendor) || ')';

else
	--sql:=sql || ' union select '||v_total_point_entity||' as length_qty,* from fn_network_planning_audit_template_detail('''||v_point_entity||''','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';
sql := sql || ' UNION SELECT ' || v_total_point_entity || ' AS length_qty, * 
FROM fn_network_planning_audit_template_detail('
|| quote_literal(v_point_entity) || ',' 
|| p_user_id || ',' 
|| quote_literal(p_polepecvendor) || ',' 
|| quote_literal(p_manholepecvendor) || ',' 
|| quote_literal(p_scspecvendor) || ')';

raise info 'inner query11 =%',sql;
end if;

if(v_is_osp_tp)
then 
	sql:=sql || ' union select '||v_end_entity_count||' as length_qty,* from fn_network_planning_audit_template_detail(''spliceclosure'','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';
end if;

end if;


end if;

raise info 'inner query =%',sql;

 SELECT count(*)>0 into is_a_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole','SpliceClosure','BDB','FDB', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = v_temp_planid and tp.fraction = 0
 ORDER BY tp.system_id LIMIT 1),4326),2));

 SELECT count(*)>0 into is_b_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure','BDB','FDB', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = v_temp_planid and tp.fraction = 1
 ORDER BY tp.system_id LIMIT 1),4326),2));

        -- First try with start point
        SELECT pod.site_id
        INTO p_site_id
        FROM point_master pm
        JOIN att_details_pod pod ON pod.system_id = pm.system_id 
        WHERE pm.entity_type = 'POD'
          AND ST_Within(
                pm.sp_geometry,
                st_buffer_meters(
                    ST_StartPoint(
                        ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
                    ), 3)
            )
        LIMIT 1;
       
        p_site_id = case when p_site_id is not null then p_site_id else '' end;
         -- Update Manhole names (forward order)
        UPDATE temp_auto_network_plan t
        SET entity_name = 
            CASE
                WHEN p_site_id IS null or p_site_id ='' THEN 'MH-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-MH-' || LPAD(x.seq::text, 2, '0')
            END
        FROM (
           SELECT system_id,
          ROW_NUMBER() OVER (ORDER BY system_id desc) AS seq
		FROM temp_auto_network_plan
		WHERE entity_type = 'Manhole'
		  AND plan_id = v_temp_planid
		  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
             
        ) x
        WHERE t.system_id = x.system_id
          AND t.plan_id = v_temp_planid;

        -- Update Pole names (forward order)
        UPDATE temp_auto_network_plan t
        SET entity_name = 
            CASE
                WHEN p_site_id IS null or p_site_id ='' THEN 'PL-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-PL-' || LPAD(x.seq::text, 2, '0')
            END
        FROM (
            SELECT system_id,
                   ROW_NUMBER() OVER (ORDER BY system_id desc) AS seq
            FROM temp_auto_network_plan
            WHERE entity_type = 'Pole'
              AND plan_id = v_temp_planid
              AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
        ) x
        WHERE t.system_id = x.system_id
          AND t.plan_id = v_temp_planid;

        -- If not found, try with end point
        IF p_site_id IS null or p_site_id = '' THEN
            SELECT pod.site_id 
            INTO p_site_id
            FROM point_master pm
            JOIN att_details_pod pod ON pod.system_id = pm.system_id 
            WHERE pm.entity_type = 'POD'
              AND ST_Within(
                    pm.sp_geometry,
                    st_buffer_meters(
                        ST_EndPoint(
                            ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
          ), 3) )
            LIMIT 1;
                                   

            -- Update Manhole names (reverse order)
            UPDATE temp_auto_network_plan t
            SET entity_name =  CASE
                WHEN p_site_id IS NULL  or p_site_id ='' THEN 'MH-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-MH-' || LPAD(x.seq::text, 2, '0')
            END
            FROM (
                SELECT system_id,
                       ROW_NUMBER() OVER (ORDER BY system_id ) AS seq
                FROM temp_auto_network_plan 
                WHERE entity_type = 'Manhole'
                  AND plan_id = v_temp_planid
                  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
            ) x
            WHERE t.system_id = x.system_id
              AND t.plan_id = v_temp_planid;

            -- Update Pole names (reverse order)
            UPDATE temp_auto_network_plan t
            SET entity_name =  CASE
                WHEN p_site_id IS NULL or p_site_id ='' THEN 'PL-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-PL-' || LPAD(x.seq::text, 2, '0') 
            END
            FROM (
                SELECT system_id,
                       ROW_NUMBER() OVER (ORDER BY system_id) AS seq
                FROM temp_auto_network_plan
                WHERE entity_type = 'Pole'
                  AND plan_id = v_temp_planid
                  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
            ) x
            WHERE t.system_id = x.system_id
              AND t.plan_id = v_temp_planid;
             
            
   END IF;
 
  perform (fn_auto_network_plan_draft_update_loop_network(p_line_geom, v_temp_planid, p_user_id, p_loop_span, p_loop_length));
 
 if(p_is_loop_require)
	then 
  select coalesce(sum(loop_length)::double precision,0) ,count(*)  from temp_auto_network_plan where plan_id=v_temp_planid AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      ) into v_loop_length,v_total_point_entity;
	
		sql:=sql || ' union select '||v_loop_length||' as length_qty,''Loop'' as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
end if;
	
v_outer_query:= 'select '||V_IS_VALID||'::boolean AS is_template_extis,'||v_temp_planid||' as temp_plan_id,
 case when upper(entity_type)=''CABLE'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''DUCT'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''TRENCH'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
 when upper(entity_type)=''LOOP'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_point_entity||')::character varying else length_qty ::character varying end as length_qty,
(round((length_qty)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((length_qty)::numeric, 2))as amount,entity_type,geom_type,cost_per_unit,service_cost_per_unit from ('||sql||')e';

 raise info 'inner query fin=al =%',v_outer_query;
  
IF (V_IS_VALID) THEN
    RETURN QUERY
    EXECUTE 'select row_to_json(row) 
             from (select *,'|| quote_literal(coalesce(p_site_id, '')) ||' as site_id 
                   from ('||v_outer_query||')t order by geom_type) row';
ELSE
    RETURN QUERY
    EXECUTE 'select row_to_json(row) 
             from (select '||V_IS_VALID||'::boolean AS is_template_extis, 
                          '||quote_literal(V_MESSAGE)||'::character varying AS msg, 
                          '||quote_literal(coalesce(p_site_id, ''))||'::character varying as site_id) row';
END IF;

            
 END
$function$
;

-- Permissions

-------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_network_planning_save_auto_planning(p_plan_mode character varying, p_cable_type character varying, is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_cable_length double precision, p_distance double precision, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, p_end_point_type character varying, p_end_point_buffer double precision, p_edit_path character varying, p_end_point_entity_id integer, p_end_point_entity_type character varying, p_temp_plan_id integer, p_is_loop_required boolean, p_loop_length double precision, p_polespecvendor character varying, p_manholespecvendor character varying, p_scspecvendor character varying)
 RETURNS TABLE(status boolean, message character varying, plan_id integer)
 LANGUAGE plpgsql
AS $function$

DECLARE

V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow record;
v_system_id integer;
v_entity_type character varying;
v_arow_network_id record;
v_arow_parent_details record;
v_arow_cable record;
v_arow_box record;
v_layer_table character varying;
v_arow_splitter record;
sql text;
v_arow_wcr record;
v_arow_layer_details record;
v_layer_wcr_mapping character varying;
v_new_wcr_mapping record;
v_current_system_id integer;
v_line_geom geometry;
v_region_province record;
v_arow_closure record;
v_counter integer;
v_arow_prev_closure record;
v_arow_prev_cable record;
-- v_other_end_geom geometry;
v_prev_qty_length double precision;
V_TOTAL_CALC_LENGTH DOUBLE PRECISION;
v_line_total_length DOUBLE PRECISION;
v_total_line integer;
p_rfs_type character varying;
v_manhole_item_id integer;
v_arow_manhole record;
v_sub_line_geom geometry;
v_sub_line_start_point geometry;
v_sub_line_end_point geometry;
v_sub_line character varying;
v_arow_prev_manhole record;
v_arow_trench record;
v_arow_duct record;
v_arow_fms record;
v_arow_pod record;
v_arow_first_closure record;
v_point_entity character varying;
v_is_osp_tp BOOLEAN;
p_plan_id integer;
v_audit_item_master_id_sc integer;
v_audit_item_master_id_point_entity integer;
v_audit_item_master_id_cable integer;
v_audit_item_master_id_duct integer;
v_audit_item_master_id_trench integer;
v_layer_ids character varying;
--v_arow_first_manhole record;
v_temp_point_entity record;
--------
v_current_point_geom geometry;
v_start_point character varying;
v_end_point character varying;
v_loop_length double precision;
v_point_entity_type character varying;
v_a_display_value CHARACTER VARYING;
v_inner_query text;
v_arow_end_manhole record;
v_arow_middle_manhole record;
v_total_record_temp integer;
v_counter_2 integer;
v_pole_spec_vendor TEXT[];
v_manhole_spec_vendor TEXT[];
v_sc_spec_vendor TEXT[];
is_endpoint_entity_exists boolean;
is_a_endpoint_entity_exists boolean;
is_b_endpoint_entity_exists boolean;
v_result TEXT;

BEGIN
v_counter_2=0;
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
v_counter:=0;
sql:='';
v_prev_qty_length:=0;
p_rfs_type:='A-RFS';
p_plan_id=0;

CREATE temp TABLE temp_connection
(
source_system_id integer,
source_network_id character varying(100),
source_entity_type character varying(100),
source_port_no integer,
destination_system_id integer,
destination_network_id character varying(100),
destination_entity_type character varying(100),
destination_port_no integer,
is_source_cable_a_end boolean NOT NULL DEFAULT false,
is_destination_cable_a_end boolean NOT NULL DEFAULT false,
equipment_system_id integer,
equipment_network_id character varying(100),
equipment_entity_type character varying(100),
created_by integer,
splicing_source character varying(100)
) on commit drop;

IF(coalesce(p_line_geom,'')='' )
then
V_IS_VALID=FALSE;
V_MESSAGE:='Line Geom can not be blank !';
end if;

IF(V_IS_VALID)
THEN
select is_osp_tp into v_is_osp_tp from vw_termination_point_master where tp_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure')and layer_id=(select layer_id from layer_details where layer_name='Cable');

select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
select st_length(v_line_geom,false)::double precision into v_line_total_length;

    v_pole_spec_vendor := string_to_array(p_polespecvendor, ',');
    v_manhole_spec_vendor := string_to_array(p_manholespecvendor, ',');
    v_sc_spec_vendor := string_to_array(p_scspecvendor, ',');
  
if(upper(p_cable_type)=upper('overhead'))then
v_point_entity:='Pole';
else
v_point_entity:='Manhole';
end if;

select validation.status,validation.message from fn_network_planning_validate(p_plan_mode,p_cable_type,is_create_trench,is_create_duct,v_is_osp_tp,p_cable_length,v_point_entity,p_user_id,p_end_point_buffer,p_line_geom) as validation into V_IS_VALID,V_MESSAGE;

end if;

IF(V_IS_VALID)
THEN

------------ Get all entity audit_item_master_id =================================================

select (select (select * from fn_get_template_detail(p_user_id,'SpliceClosure',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_sc ;
select (select (select * from fn_get_template_detail(p_user_id,v_point_entity,''))->>'audit_item_master_id')::integer into v_audit_item_master_id_point_entity ;
select (select (select * from fn_get_template_detail(p_user_id,'cable',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_cable ;
select (select (select * from fn_get_template_detail(p_user_id,'duct',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_duct ;
select (select (select * from fn_get_template_detail(p_user_id,'trench',''))->>'audit_item_master_id')::integer into v_audit_item_master_id_trench;

----------------========================layerList--------------=================================
select array_to_string(array_agg(layer_id), ',') into v_layer_ids from layer_details
where
(true =true and upper(layer_name) =upper('cable'))
or (is_create_trench =true and upper(layer_name) =upper('trench'))
or (is_create_duct =true and upper(layer_name) =upper('duct'))
or (v_is_osp_tp =true and upper(layer_name) =upper('SpliceClosure'))
or (p_is_loop_required =true and upper(layer_name) =upper('Loop'))
or (true =true and upper(layer_name) =upper(''||v_point_entity||''));

------------------------- save plan ==================================================================================
INSERT INTO public.att_details_network_plan
(planid,layer_id,plan_name, start_point, end_point, cable_type, is_create_trench, is_create_duct, pole_manhole_distance, cable_length, created_by,
created_on,planning_mode,
end_point_type,end_point_buffer,end_point_entity,edit_path,is_loop_required,loop_length)
VALUES(p_temp_plan_id,v_layer_ids,p_plan_name, p_startpoint, p_endpoint,p_cable_type,is_create_trench, is_create_duct, p_distance, p_cable_length, p_user_id, now(),p_plan_mode,
p_end_point_type,p_end_point_buffer,p_end_point_entity_id,p_edit_path,p_is_loop_required,p_loop_length)
RETURNING planid into p_plan_id;

 SELECT count(*)>0 into is_a_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'BDB','FDB','Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = p_temp_plan_id and tp.fraction = 0
 ORDER BY tp.system_id LIMIT 1),4326),2));

 SELECT count(*)>0 into is_b_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure','BDB','FDB','Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = p_temp_plan_id and tp.fraction = 1
 ORDER BY tp.system_id LIMIT 1),4326),2));
--select count(1) from temp_auto_network_plan


EXECUTE 'ALTER TABLE point_master DISABLE TRIGGER ALL';
EXECUTE 'ALTER TABLE line_master DISABLE TRIGGER ALL';

FOR v_temp_point_entity IN select * from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id order by system_id
LOOP
 
is_endpoint_entity_exists :=
    CASE
        WHEN v_temp_point_entity.fraction = 0 THEN is_a_endpoint_entity_exists
        WHEN v_temp_point_entity.fraction = 1 THEN is_b_endpoint_entity_exists
        ELSE false
    END;

if(is_endpoint_entity_exists = true and v_temp_point_entity.fraction = 0) 
then 
 v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;

end if;

v_counter:=v_counter+1;

v_current_point_geom=case when v_temp_point_entity.fraction in (0,1) then v_temp_point_entity.sp_geometry else ST_LineInterpolatePoint(v_line_geom, v_temp_point_entity.fraction) end;

select count(1) from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point =false into v_total_record_temp;

v_loop_length := v_temp_point_entity.loop_length;
v_point_entity_type := v_temp_point_entity.entity_type;
select * into v_region_province from fn_getregionprovince(substring(left(St_astext(v_current_point_geom),-1),7),'Point');

update temp_auto_network_plan set province_id =v_region_province.province_id,region_id=v_region_province.region_id,created_by=p_user_id where system_id=v_temp_point_entity.system_id;


if exists (SELECT 1 FROM point_master pm
 WHERE pm.entity_type IN ('Pole','Manhole')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1)
  
and not exists (SELECT 1 FROM point_master pm
 WHERE pm.entity_type IN ('SpliceClosure')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1)  then 
 
select * into v_arow_network_id from fn_get_network_code('SpliceClosure','Point',0,'',substring(left(St_astext(v_current_point_geom),-1),7));

insert into att_details_spliceclosure(is_visible_on_map,audit_item_master_id,ownership_type,network_id,spliceclosure_name,latitude,
longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,
no_of_input_port,no_of_output_port,is_used,is_virtual,source_ref_type,source_ref_id)

select true,audit_id ,'OWN',v_arow_network_id.network_code,v_arow_network_id.network_code,st_y(v_current_point_geom),
st_x(v_current_point_geom),v_region_province.province_id,v_region_province.region_id,
specification, category_reference, subcategory_1, subcategory_2, subcategory_3,code, vendor_id, 0,0,0,0,0,0,'A',p_user_id,now(),
'P',v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
v_arow_network_id.sequence_id,no_of_port,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false,'planning',p_plan_id
from item_template_master where id = v_sc_spec_vendor[1]::integer and vendor_id = v_sc_spec_vendor[3]::integer and code = v_sc_spec_vendor[2] ::varchar limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port into v_arow_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
created_by,common_name, network_status,no_of_ports,display_name,source_ref_type,source_ref_id)
values(v_arow_closure.system_id, 'SpliceClosure',st_x(v_current_point_geom),st_y(v_current_point_geom),'A',
ST_GEOMFROMTEXT('POINT('||st_x(v_current_point_geom)||' '||st_y(v_current_point_geom)||')',4326),now(),'Created', ''
,p_user_id,v_arow_network_id.network_code, 'P',v_arow_closure.no_of_ports::character varying,
fn_get_display_name(v_arow_closure.system_id, 'SpliceClosure'),'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_closure.system_id,'SpliceClosure'::character varying ,v_region_province.province_id,0,false));

perform(fn_bulk_insert_port_info(v_arow_closure.no_of_ports,v_arow_closure.no_of_ports,'SpliceClosure',
v_arow_closure.system_id,v_arow_closure.network_id,p_user_id));

end if;

-- if we need to create entity in middle then only middle entity is created
if(v_temp_point_entity.is_middle_point=true)
then

SELECT * into v_arow_network_id FROM fn_get_clone_network_code( v_point_entity, 'Point', substring(left(St_astext(v_current_point_geom),-1),7),0,'')
as clone order by clone.status desc limit 1;

raise info '%is_endpoint_entity_exists',is_endpoint_entity_exists;
if(is_endpoint_entity_exists = false) then 

if(upper(v_point_entity)=upper('Manhole'))
then
raise info '%v_counter_1',v_counter_2;

INSERT INTO att_details_manhole(is_visible_on_map,audit_item_master_id,ownership_type,network_id, manhole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
,project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',v_arow_network_id.message,case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name  else v_arow_network_id.message end,
st_x(v_current_point_geom),st_y(v_current_point_geom),
v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,
code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',v_arow_network_id.o_p_system_id,
v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0, 0, 0, 0,'planning',p_plan_id
FROM item_template_master where id = v_manhole_spec_vendor[1]::integer and vendor_id = v_manhole_spec_vendor[3]::integer and code= v_manhole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_middle_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name)
values(v_arow_middle_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A', ST_GEOMFROMTEXT('POINT('||st_x(v_current_point_geom)||' '||st_y(v_current_point_geom)||')',4326),now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name  else fn_get_display_name(v_arow_middle_manhole.system_id, v_point_entity) end
);

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Manhole'::character varying ,v_region_province.province_id,0,false));

else
raise info '%v_counter_2',v_counter_2;

INSERT INTO att_details_pole(is_visible_on_map,audit_item_master_id,ownership_type,pole_type,network_id, pole_name,
longitude, latitude, province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code,
vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type, sequence_id
, project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',specify_type,v_arow_network_id.message,
case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else v_arow_network_id.message end,
st_x(v_current_point_geom),st_y(v_current_point_geom),
v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,
code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
v_arow_network_id.o_p_system_id,v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id,
0, 0, 0, 0 ,'planning',p_plan_id
FROM item_template_master where id = v_pole_spec_vendor[1]::integer and vendor_id = v_pole_spec_vendor[3]::integer and code = v_pole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_middle_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_middle_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A', ST_GEOMFROMTEXT('POINT('||st_x(v_current_point_geom)||' '||st_y(v_current_point_geom)||')',4326) ,now(),'Created', '' ,p_user_id,v_arow_middle_manhole.network_id, 'P',
case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else fn_get_display_name(v_arow_middle_manhole.system_id, v_point_entity) end,'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_middle_manhole.system_id,'Pole'::character varying ,v_region_province.province_id,0,false));
end if;
end if;

else
raise info '%v_counter_3',v_counter_2;

raise info '%is_endpoint_entity_exists',is_endpoint_entity_exists;

SELECT * into v_arow_network_id FROM fn_get_clone_network_code( v_point_entity, 'Point', substring(left(St_astext(v_current_point_geom),-1),7),0,'')
as clone order by clone.status desc limit 1;
raise info '%v_counter_4',v_counter_2;

if( is_endpoint_entity_exists = false)
then 

raise info '%v_counter_5',v_counter_2;

if(upper(v_point_entity)=upper('Manhole'))
then
raise info '%v_counter_6',v_counter_2;

INSERT INTO att_details_manhole(is_visible_on_map,audit_item_master_id,ownership_type,network_id, manhole_name, longitude, latitude,
province_id,region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, type, brand,
model, construction, activation,accessibility, status,created_by, created_on,network_status,parent_system_id, parent_network_id,
parent_entity_type, sequence_id,project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',v_arow_network_id.message,case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else v_arow_network_id.message end,st_x(v_current_point_geom),
st_y(v_current_point_geom),v_region_province.province_id,v_region_province.region_id, specification, category_reference,  subcategory_1, subcategory_2, subcategory_3,code,vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',v_arow_network_id.o_p_system_id,
v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0, 0, 0, 0,'planning',p_plan_id
FROM item_template_master where id = v_manhole_spec_vendor[1]::integer and vendor_id = v_manhole_spec_vendor[3]::integer and code = v_manhole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude into v_arow_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A', ST_GEOMFROMTEXT('POINT('||st_x(v_current_point_geom)||' '||st_y(v_current_point_geom)||')',4326),now(),'Created', '' ,p_user_id,v_arow_manhole.network_id,
'P',case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else fn_get_display_name(v_arow_manhole.system_id, v_point_entity) end,'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_manhole.system_id,'Manhole'::character varying ,v_region_province.province_id,0,false));

else

raise info '%v_counter_7',v_counter_2;

INSERT INTO att_details_pole(is_visible_on_map,audit_item_master_id,ownership_type,pole_type,network_id, pole_name, longitude,
latitude, province_id,
region_id,specification, category, subcategory1, subcategory2, subcategory3,item_code, vendor_id, type, brand, model, construction,
activation,accessibility, status,created_by, created_on,network_status, parent_system_id, parent_network_id, parent_entity_type,
sequence_id, project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id)

SELECT true,v_audit_item_master_id_point_entity,'Own',specify_type,v_arow_network_id.message,
case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else v_arow_network_id.message end,
st_x(v_current_point_geom),st_y(v_current_point_geom),v_region_province.province_id,v_region_province.region_id, specification, category_reference,
 subcategory_1, subcategory_2, subcategory_3,code, vendor_id, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P',
v_arow_network_id.o_p_system_id,v_arow_network_id.o_p_network_id,v_arow_network_id.o_p_entity_type,v_arow_network_id.o_sequence_id, 0,
0, 0, 0,'planning',p_plan_id FROM item_template_master where id = v_pole_spec_vendor[1]::integer and vendor_id = v_pole_spec_vendor[3]::integer and code = v_pole_spec_vendor[2]::varchar limit 1 RETURNING system_id,network_id,latitude,longitude
into v_arow_manhole;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_type,source_ref_id)
values(v_arow_manhole.system_id, v_point_entity,st_x(v_current_point_geom)::double precision, st_y(v_current_point_geom)::double precision,
'A', ST_GEOMFROMTEXT('POINT('||st_x(v_current_point_geom)||' '||st_y(v_current_point_geom)||')',4326) ,now(),'Created', '' ,p_user_id,v_arow_manhole.network_id,
'P',case when v_temp_point_entity.entity_name is not null then v_temp_point_entity.entity_name else fn_get_display_name(v_arow_manhole.system_id, v_point_entity) end ,'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_manhole.system_id,'Pole'::character varying, v_region_province.province_id,0,false));
end if;

--------- if need to create spliceclosure then we run this
if(v_is_osp_tp) then
raise info '%v_counter_8',v_counter_2;

select * into v_arow_network_id from fn_get_network_code('SpliceClosure','Point',0,'',substring(left(St_astext(v_current_point_geom),-1),7));

insert into att_details_spliceclosure(is_visible_on_map,audit_item_master_id,ownership_type,network_id,spliceclosure_name,latitude,
longitude,province_id,region_id,
specification,category,subcategory1,subcategory2,subcategory3,item_code,vendor_id,
type,brand,model,construction,activation,accessibility,status,created_by,created_on,network_status,parent_system_id,
parent_network_id,parent_entity_type,sequence_id,no_of_ports,project_id,planning_id,purpose_id,workorder_id,structure_id,
no_of_input_port,no_of_output_port,is_used,is_virtual,source_ref_type,source_ref_id)

select true,audit_id ,'OWN',v_arow_network_id.network_code,v_arow_network_id.network_code,st_y(v_current_point_geom),
st_x(v_current_point_geom),v_region_province.province_id,v_region_province.region_id,
specification, category_reference, subcategory_1, subcategory_2, subcategory_3,code, vendor_id, 0,0,0,0,0,0,'A',p_user_id,now(),
'P',v_arow_network_id.parent_system_id,v_arow_network_id.parent_network_id,v_arow_network_id.parent_entity_type,
v_arow_network_id.sequence_id,no_of_port,0,0,0,0,0,no_of_input_port,no_of_output_port,true,false,'planning',p_plan_id
from item_template_master where id = v_sc_spec_vendor[1]::integer and vendor_id = v_sc_spec_vendor[3]::integer and code = v_sc_spec_vendor[2] ::varchar limit 1
RETURNING system_id,network_id,latitude,longitude,no_of_ports,no_of_input_port,no_of_output_port into v_arow_closure;

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,
created_by,common_name, network_status,no_of_ports,display_name,source_ref_type,source_ref_id)
values(v_arow_closure.system_id, 'SpliceClosure',st_x(v_current_point_geom),st_y(v_current_point_geom),'A',
ST_GEOMFROMTEXT('POINT('||st_x(v_current_point_geom)||' '||st_y(v_current_point_geom)||')',4326),now(),'Created', ''
,p_user_id,v_arow_network_id.network_code, 'P',v_arow_closure.no_of_ports::character varying,
fn_get_display_name(v_arow_closure.system_id, 'SpliceClosure'),'planning',p_plan_id);

perform (fn_geojson_update_entity_attribute(v_arow_closure.system_id,'SpliceClosure'::character varying ,v_region_province.province_id,0,false));

perform(fn_bulk_insert_port_info(v_arow_closure.no_of_ports,v_arow_closure.no_of_ports,'SpliceClosure',
v_arow_closure.system_id,v_arow_closure.network_id,p_user_id));

insert into associate_entity_info(associated_entity_type,associated_system_id,associated_network_id,entity_network_id,entity_system_id,
entity_type,created_on,created_by,associated_display_name,entity_display_name)
values('SpliceClosure',v_arow_closure.system_id,v_arow_closure.network_id,v_arow_manhole.network_id,v_arow_manhole.system_id,v_point_entity,now(),p_user_id,
fn_get_display_name(v_arow_closure.system_id,'SpliceClosure'),fn_get_display_name(v_arow_manhole.system_id,v_point_entity));

end if;
end if;

-- update values in temp table related to loop
raise info '%v_counter_9',v_counter_2;

if(v_counter >1 and v_temp_point_entity.is_middle_point=false)

then
raise info '%v_counter_10',v_counter_2;
IF(v_temp_point_entity.FRACTION = 1) then 
v_arow_closure.longitude = v_temp_point_entity.longitude;
v_arow_closure.latitude = v_temp_point_entity.latitude;
v_arow_manhole.longitude = v_temp_point_entity.longitude;
v_arow_manhole.latitude = v_temp_point_entity.latitude;

end if;

select ST_GeomFromText('POINT('||v_arow_prev_closure.longitude||' '||v_arow_prev_closure.latitude||')',4326) into v_sub_line_start_point;
select ST_GeomFromText('POINT('||v_arow_closure.longitude||' '||v_arow_closure.latitude||')',4326) into v_sub_line_end_point;
select ST_LineSubstring(v_line_geom, ST_LineLocatePoint(v_line_geom, v_sub_line_start_point),
ST_LineLocatePoint(v_line_geom, v_sub_line_end_point)) into v_sub_line_geom;
select substring(left(st_astext(v_sub_line_geom),-1),12) into v_sub_line;

raise info '%v_sub_line',v_sub_line;
raise info '%v_sub_line1',v_arow_prev_closure.longitude||' '||v_arow_prev_closure.latitude;
raise info '%v_sub_line2',v_arow_closure.longitude||' '||v_arow_closure.latitude;

raise info '%v_arow_prev_manhole',v_arow_prev_manhole.longitude||' '||v_arow_closure.latitude;

if(is_create_trench)
then
select * into v_arow_trench from fn_network_planning_auto_create_trench(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,v_audit_item_master_id_trench)
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_trench.system_id,'Trench'::character varying ,v_region_province.province_id,0,false));

--- update trench id in temp table related to point_entity
update temp_auto_network_plan tp set trench_id=v_arow_trench.SYSTEM_ID,trench_network_id=v_arow_trench.network_id
where tp.system_id <= v_temp_point_entity.system_id and trench_id=0 and tp.plan_id=p_temp_plan_id;
end if;

if(is_create_duct)then

if(is_create_trench)then

select * into v_arow_duct from fn_network_planning_auto_create_duct(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,
v_audit_item_master_id_duct,v_arow_trench.system_id,v_arow_trench.network_id)
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_duct.system_id,'Duct'::character varying ,v_region_province.province_id,0,false));

else

select * into v_arow_duct from fn_network_planning_auto_create_duct(v_arow_prev_manhole.system_id,
'Manhole'::character varying,v_arow_prev_manhole.network_id,
v_arow_manhole.system_id,'Manhole'::character varying,v_arow_manhole.network_id,v_sub_line,
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,p_plan_id,
v_audit_item_master_id_duct,0,'')
as x(system_id integer,network_id character varying);

perform (fn_geojson_update_entity_attribute(v_arow_duct.system_id,'Duct'::character varying ,v_region_province.province_id,0,false));

end if;

--- update duct id in temp table related to point_entity
update temp_auto_network_plan tp set duct_id=v_arow_duct.SYSTEM_ID,duct_network_id=v_arow_duct.network_id
where tp.system_id <= v_temp_point_entity.system_id and duct_id=0 and tp.plan_id=p_temp_plan_id;
end if;

v_counter_2=v_counter_2+1;

--CREATE THE CABLE acco to duct

if(is_create_duct)
then

select * into v_arow_cable from fn_network_planning_auto_create_cable(v_arow_prev_closure.system_id,
'SpliceClosure'::character varying,v_arow_prev_closure.network_id,
v_arow_closure.system_id,'SpliceClosure'::character varying,v_arow_closure.network_id,v_sub_line,
--(v_arow_htb.longitude||' '||v_arow_htb.latitude||','||v_arow_closure.longitude||' '||v_arow_closure.latitude),
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,500,v_audit_item_master_id_cable,p_plan_id,
p_cable_type,v_arow_duct.system_id,v_arow_duct.network_id)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));

raise info '%v_counter_11',v_counter_2;
if(v_counter_2 = 1)
then

if(is_endpoint_entity_exists = true) 
then 
if(v_temp_point_entity.fraction = 0) 
then 
 v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;

/* elseif( v_temp_point_entity.fraction = 1) 
THEN
v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 --v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;
 */
end if;
else
raise info '%v_counter_12',v_counter_2;

v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
end if;

else

/*if(v_arow_prev_cable is not null)
then
insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,
destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,created_by,splicing_source,
equipment_system_id,equipment_network_id,equipment_entity_type)
values(v_arow_prev_cable.system_id,v_arow_prev_cable.network_id,'Cable',1,v_arow_cable.system_id,v_arow_cable.network_id,'Cable',1,false,true,
p_user_id,'PROVISIONNING',
v_arow_prev_closure.system_id, v_arow_prev_closure.network_id,'SpliceClosure');
end if;
*/
raise info '%v_counter_13',v_counter_2;

v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;

end if;
else
raise info '%v_counter_14',v_counter_2;

select * into v_arow_cable from fn_network_planning_auto_create_cable(v_arow_prev_closure.system_id,
'SpliceClosure'::character varying,v_arow_prev_closure.network_id,
v_arow_closure.system_id,'SpliceClosure'::character varying,v_arow_closure.network_id,v_sub_line,
--(v_arow_htb.longitude||' '||v_arow_htb.latitude||','||v_arow_closure.longitude||' '||v_arow_closure.latitude),
0,v_region_province.region_id,v_region_province.province_id,p_user_id,p_rfs_type,500,v_audit_item_master_id_cable,p_plan_id,
p_cable_type,0,'')
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id,'Cable'::character varying ,v_region_province.province_id,0,false));

if(v_counter_2 = 1)
then
raise info '%v_counter_15',v_counter_2;

if(is_endpoint_entity_exists = true) 
then 
if(v_temp_point_entity.fraction = 0) 
then 
 v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;
/*
elseif( v_temp_point_entity.fraction = 1) 
THEN
v_arow_closure = null;

 SELECT pm.system_id ,pm.common_name as network_id,pm.entity_type, pm.longitude ,pm.latitude  into v_arow_closure FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
  WHERE tp.plan_id = p_temp_plan_id and tp.fraction = v_temp_point_entity.fraction and tp.fraction in (0,1)
  ORDER BY tp.system_id LIMIT 1),4326),2)) limit 1;
 
 --v_arow_prev_closure=v_arow_closure;
 v_arow_manhole = v_arow_closure;
 */
end if;
 
raise info '%v_counter_16',v_counter_2;

 else
 raise info '%v_counter_17',v_counter_2;

--v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;

end if;

raise info 'v_arow_prev_cable%',v_arow_prev_cable;
else
/*
if(v_arow_prev_cable is not null)
then
insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,
destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,created_by,splicing_source,
equipment_system_id,equipment_network_id,equipment_entity_type)
values(v_arow_prev_cable.system_id,v_arow_prev_cable.network_id,'Cable',1,v_arow_cable.system_id,v_arow_cable.network_id,'Cable',1,false,true,
p_user_id,'PROVISIONNING',
v_arow_prev_closure.system_id, v_arow_prev_closure.network_id,'SpliceClosure');
end if;
*/
raise info '%v_counter_18',v_counter_2;

--v_arow_prev_cable=v_arow_cable;
v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
raise info '%v_counter_19',v_counter_2;

end if;
end if;

PERFORM(fn_network_planning_associate_end_entity(false,'',v_arow_prev_manhole.system_id,v_point_entity,v_arow_prev_manhole.network_id
,v_arow_manhole.system_id,v_point_entity,v_arow_manhole.network_id,'Cable',v_arow_cable.system_id,v_arow_cable.network_id,p_user_id,p_temp_plan_id));
--

PERFORM(fn_isp_create_OSP_Cable(v_arow_cable.SYSTEM_ID));

--- update cable id in temp table related to point_entity
update temp_auto_network_plan tp set cable_id=v_arow_cable.SYSTEM_ID,cable_network_id=v_arow_cable.network_id
where tp.system_id <= v_temp_point_entity.system_id and cable_id=0 and tp.plan_id=p_temp_plan_id;
--and loop_length !=0;

end if;

end if;
raise info '%v_counter_20',v_counter_2;

if(v_temp_point_entity.is_middle_point=true and is_endpoint_entity_exists = false)
then

raise info '%v_counter_21',v_counter_2;

select display_name into v_a_display_value from point_master where system_id=v_arow_middle_manhole.system_id and upper(entity_type)=upper(v_point_entity);

update temp_auto_network_plan set entity_type=v_point_entity,
entity_network_id=v_arow_middle_manhole.network_id,entity_system_id=v_arow_middle_manhole.system_id,display_name=v_a_display_value
where system_id=v_temp_point_entity.system_id and v_temp_point_entity.is_middle_point=true;
else
select display_name into v_a_display_value from point_master where system_id=v_arow_manhole.system_id and upper(entity_type)=upper(v_point_entity);

update temp_auto_network_plan set entity_type=v_point_entity,
entity_network_id=v_arow_manhole.network_id,entity_system_id=v_arow_manhole.system_id,display_name=v_a_display_value
where system_id=v_temp_point_entity.system_id and v_temp_point_entity.is_middle_point=false;
end if;

if(v_counter >1 and v_temp_point_entity.is_middle_point=false)
then

PERFORM(fn_network_planning_loop_entity(v_arow_cable.SYSTEM_ID,p_line_geom,is_a_endpoint_entity_exists,is_b_endpoint_entity_exists));
end if;
raise info '%v_counter_17',v_counter_2;

v_arow_prev_closure=v_arow_closure;
v_arow_prev_manhole=v_arow_manhole;
end loop;

---------------------------associate cables to middle point entity like pole and manhole
if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then
PERFORM(fn_network_planning_associate_end_entity(true,'Cable',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

-- ---------------------------associate cables to middle point entity like pole and manhole
if(is_create_duct)
then

-- if middle entity exist then middle entity associate
if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then

PERFORM(fn_network_planning_associate_end_entity(true,'Duct',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

end if;

-- ---------------------------associate cables to middle point entity like pole and manhole
if(is_create_trench)
then

if exists(select 1 from temp_auto_network_plan tp where tp.plan_id=p_temp_plan_id and is_middle_point=true) then

PERFORM(fn_network_planning_associate_end_entity(true,'Trench',0,'','',0,'','','Cable',0,'',p_user_id,p_temp_plan_id));
end if;

end if;

end if;

INSERT INTO routing_data_core_plan (id, system_id, source, target, cost, reverse_cost, geom)
SELECT
lm.system_id ,lm.system_id,null as source,null as target,
ST_Length(lm.sp_geometry::geography) / 1000 AS cost,
ST_Length(lm.sp_geometry::geography) / 1000 AS reverse_cost,
ST_Force2D(lm.sp_geometry) AS geom
FROM line_master lm
WHERE lm.entity_type = 'Cable'
AND lm.source_ref_id = p_temp_plan_id::varchar and lm.source_ref_type ='planning';

SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
IF v_result <> 'OK' THEN
RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for INSERT' AS message;
END IF;
SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
IF v_result <> 'OK' THEN
RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for INSERT' AS message;
END IF;


EXECUTE 'ALTER TABLE line_master ENABLE TRIGGER ALL';
EXECUTE 'ALTER TABLE point_master ENABLE TRIGGER ALL';

IF(V_IS_VALID)
THEN
V_MESSAGE:='Network plan processed successfully!';
END IF;

RETURN QUERY SELECT V_IS_VALID::boolean AS STATUS, V_MESSAGE::CHARACTER VARYING AS MESSAGE, p_plan_id:: integer as plan_id;

END
$function$
;

-- Permissions

------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_projectwise_fiber_distance_report(p_region_id integer, p_province_id integer)
 RETURNS json
 LANGUAGE plpgsql
AS $function$

DECLARE
    result JSON;
BEGIN
    SELECT json_agg(t)
    INTO result
    FROM (
        select
		
        spd.project_Id as Project_code,
            (select province_name from province_boundary r where id=adp.province_id)  AS City,
            (select province_name from province_boundary r where id=adp.province_id) as District,
		(select region_name from region_boundary r where id=adp.region_id) as Region,
        'Site' as Catergory,
		adp.site_id,
		adp.site_name, 
        adp.address ,
        0 as gis_length,
        0 as calculated_length,
        0 as micro_trench_distance,
        adp.asbuit_ibw_distance  as Asbuilt_IBW_Distance,
        adp.asbuit_osp_distance  AS Asbuilt_OSP_Distance,
		adp.asbuit_ibw_osp_total_distance  as Total_Distance,
        adp.network_status  AS Network_Status,
       CASE 
    WHEN adp.network_status = 'A' THEN 
        CASE 
            WHEN adp.modified_on IS NOT NULL THEN to_char(adp.modified_on, 'YYYY-MM-DD HH24:MI')
            ELSE to_char(adp.created_on, 'YYYY-MM-DD HH24:MI')
        END
    ELSE NULL
END AS site_asbuilt_date,
        CASE 
    WHEN adp.network_status = 'D' THEN 
        CASE 
            WHEN adp.modified_on IS NOT NULL THEN to_char(adp.modified_on, 'YYYY-MM-DD HH24:MI')
            ELSE to_char(adp.created_on, 'YYYY-MM-DD HH24:MI')
        END
    ELSE NULL
END AS site_dormant_date,
        coalesce (adp.manhole_count,0) as Manhole,
        coalesce (adp.pole_count,0) as pole,
        coalesce (adp.fiber_oh_distance_to_network,0) as arial_distance,
        coalesce (adp.fiber_ug_distance_to_network,0) as ug_distance
        FROM att_details_pod adp
        join point_master pm on pm.system_id = adp.system_id and pm.entity_type ='POD'
        join site_project_details spd on adp.site_id = spd.site_id 
       /* left join (select lm.sp_geometry,adc.cable_measured_length ,adc.cable_calculated_length ,adc.system_id from att_details_cable adc 
        join line_master lm on lm.entity_type ='Cable' 
        and lm.system_id = adc.system_id) lm on
        (ST_Intersects(st_startpoint(lm.sp_geometry), st_buffer_meters(pm.sp_geometry, 3))
or ST_Intersects(st_endpoint(lm.sp_geometry), st_buffer_meters(pm.sp_geometry, 3)))*/
        WHERE adp.region_id = p_region_id and adp.province_id= p_province_id 
    ) t;
	
   	if result is null 
   	then
   	return json_agg(jsonb_build_object('No Data','No Data')); 
   	else
   	RETURN result;
   	end if;
    
END;
$function$
;

---------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_imported_site_project_details(p_page_no integer, p_page_size integer, p_sort_col character varying, p_sort_dir character varying)
 RETURNS json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_result    JSON;
    v_sql       TEXT;
    v_order_by  TEXT;
BEGIN

    v_order_by := CASE
        WHEN p_sort_col = 'site_id'    THEN 'ps.site_id'
        WHEN p_sort_col = 'site_name'  THEN 'ps.site_name'
        WHEN p_sort_col = 'created_on' THEN 'ps.created_on'
        WHEN p_sort_col = 'project_id' THEN 'ps.project_id'
        WHEN p_sort_col = 'status'     THEN 'ps.status'
        ELSE 'ps.created_on'
    END || ' ' ||
    CASE
        WHEN upper(p_sort_dir) = 'ASC' THEN 'ASC'
        ELSE 'DESC'
    END;

    v_sql := '
        SELECT json_build_object(
            ''totalRecord'',
                (SELECT COUNT(*)
                 FROM site_project_details ps
                 JOIN att_details_pod pod
                   ON pod.site_id = ps.site_id),
            ''records'',
                json_agg(row_to_json(r))
        )
        FROM (
            SELECT 
                ps.id,
                ps.site_id,
                ps.site_name,
                ps.created_on,
                ps.project_id,
                ps.status,
                pod.latitude || '','' || pod.longitude AS site_geom
            FROM site_project_details ps
            JOIN att_details_pod pod
              ON pod.site_id = ps.site_id
            ORDER BY ' || v_order_by || '
            LIMIT ' || p_page_size || '
            OFFSET ' || (p_page_no - 1) * p_page_size || '
        ) r
    ';

    EXECUTE v_sql INTO v_result;

    RETURN v_result;
END;
$function$
;

-- Permissions

-----------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.update_site_project_detail(p_id integer, p_userid integer, p_site_name character varying, p_project_category character varying, p_cable_plan_cores character varying, p_comment character varying, p_site_owner character varying, p_maximum_cost integer, p_location_address character varying, p_ds_cmc_area character varying, p_destination_site_id character varying, p_destination_port_type character varying, p_no_of_cores integer, p_latitude double precision, p_longitude double precision, p_priority integer, p_fiber_link_type character varying, p_fiber_link_code character varying, p_total_fiber_distance double precision, p_plan_cost integer, p_site_id character varying, p_project_id character varying, OUT v_result boolean, OUT v_message character varying)
 RETURNS record
 LANGUAGE plpgsql
AS $function$
BEGIN

update site_project_details set site_name=p_site_name,project_category=p_project_category,
cable_plan_cores=p_cable_plan_cores,comment=p_comment,site_owner=p_site_owner,maximum_cost=p_maximum_cost,
location_address=p_location_address,ds_cmc_area=p_ds_cmc_area,priority=p_priority,
modified_by=p_userId, modified_on=now() where id=p_id;

  --  IF FOUND THEN
	 update att_details_pod set destination_site_id=p_destination_site_id,
	destination_port_type=p_destination_port_type,
	no_of_cores=p_no_of_cores,
	plan_cost=p_plan_cost, 
	--latitude=p_latitude,longitude=p_longitude,
	fiber_link_type=p_fiber_link_type,
	fiber_link_code=p_fiber_link_code,
	total_fiber_distance=p_total_fiber_distance,
	project_id=p_id 
	where LOWER(site_id) = LOWER(p_site_id);
	
	IF FOUND THEN
   RAISE NOTICE 'Row updated for site_id=%', p_site_id;
ELSE
   RAISE NOTICE 'No matching row for site_id=%', p_site_id;
END IF;

END;
$function$
;

-- Permissions

--------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_splicing_get_entity(p_longitude double precision, p_latitude double precision, p_buffer_radius double precision, p_role_id integer)
 RETURNS SETOF jsonb
 LANGUAGE plpgsql
AS $function$
BEGIN
RETURN QUERY
SELECT row_to_json(row)::jsonb
FROM (
    SELECT
        a.*,
        is_splicer,
        is_cpe_entity,
        CASE
            WHEN map.entity_id IS NULL AND upper(a.geom_type) = 'POINT' THEN false
            WHEN a.is_start_point = true AND map2.entity_id IS NULL AND upper(a.geom_type) = 'LINE' THEN false
            WHEN a.is_end_point = true AND map3.entity_id IS NULL AND upper(a.geom_type) = 'LINE' THEN false
            ELSE true
        END AS is_isp_entity,
        (
            SELECT count(1)
            FROM vw_termination_point_master
            WHERE upper(layer_name) = 'CABLE'
              AND upper(tp_layer_name) = upper(a.entity_type)
              AND is_enabled = true
        ) > 0 AS isCableTPPoint,
        l.is_middleware_entity,
        l.is_virtual_port_allowed AS is_virtual_entity,
        l.layer_title,
        l.layer_abbr,
        l.layer_display_abbr
    FROM (
        /* ---------- POINT ENTITIES ---------- */
        	select 0 as a_system_id,null as a_entity_type,0 as b_system_id,null as b_entity_type, pm.system_id entity_system_id, pm.entity_type entity_type,'Point' as geom_Type,pm.network_status, pm.display_name as common_name,pm.no_of_ports as total_core_ports,
	false as is_start_point,false as is_end_point,pm.is_virtual , ST_AsGeoJSON(pm.sp_geometry)::jsonb  as geometry from point_master pm 
        WHERE pm.network_status !='D' and pm.is_buried=false and  ST_WITHIN(pm.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326), p_buffer_radius))
        and UPPER(pm.entity_type) IN (select upper(layer_name) from layer_details where is_splicer=true and isvisible=true and upper(layer_name) not in ('FMS','CUSTOMER'))

        UNION 

       select 0 as a_system_id,null as a_entity_type,0 as b_system_id,null as b_entity_type, pm.system_id entity_system_id, pm.entity_type entity_type,'Point' as geom_Type,pm.network_status, pm.display_name as common_name,pm.no_of_ports as total_core_ports,
	false as is_start_point,false as is_end_point,pm.is_virtual ,
		ST_AsGeoJSON(pm.sp_geometry)::jsonb as geometry from point_master pm inner join att_details_fms fms on pm.system_id=fms.system_id and upper(pm.entity_type)='FMS' 
	and upper(fms.parent_entity_type) in ('POD','MPOD','PROVINCE')
        WHERE pm.network_status !='D' and pm.is_buried=false and  ST_WITHIN(pm.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326), p_buffer_radius))

        UNION 
select 0 as a_system_id,null as a_entity_type,0 as b_system_id,null as b_entity_type, pm.system_id entity_system_id, pm.entity_type entity_type,'Point' as geom_Type,pm.network_status, pm.display_name||'('||pm.common_name||')' as common_name,pm.no_of_ports as total_core_ports,
	false as is_start_point,false as is_end_point,pm.is_virtual,
	ST_AsGeoJSON(pm.sp_geometry)::jsonb as geometry from point_master pm  inner join att_details_customer cust on pm.system_id=cust.system_id and upper(pm.entity_type)='CUSTOMER' 
	WHERE pm.network_status !='D' and pm.is_buried=false and  ST_WITHIN(pm.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326), p_buffer_radius))

        UNION 
	
		SELECT a_system_id,a_entity_type,b_system_id,b_entity_type,lm.system_id,lm.entity_type,'Line' as geom_Type,lm.network_status,lm.display_name,cbl.total_core||'F' as total_core,
	ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer_radius)) as is_start_point,
	ST_WITHIN(ST_EndPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer_radius)) as is_end_point,lm.is_virtual,
		 ST_AsGeoJSON(lm.sp_geometry)::jsonb as geometry
	from att_details_cable cbl inner join line_master lm on cbl.system_id=lm.system_id  
	and upper(lm.entity_type)='CABLE'  where lm.network_status !='D'
	and (ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer_radius))
	or
	ST_WITHIN(ST_EndPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||p_longitude||' '||p_latitude||')',4326),p_buffer_radius)))	
    ) a
    JOIN layer_details l ON upper(a.entity_type) = upper(l.layer_name)
    JOIN role_permission_entity rp
        ON rp.layer_id = l.layer_id
       AND rp.role_id = p_role_id
       AND rp.viewonly = true
       AND upper(rp.network_status) = upper(a.network_status)
    LEFT JOIN isp_entity_mapping map
        ON a.geom_type = 'Point'
       AND a.entity_system_id = map.entity_id
    LEFT JOIN isp_entity_mapping map2
        ON a.a_system_id = map2.entity_id
    LEFT JOIN isp_entity_mapping map3
        ON a.b_system_id = map3.entity_id
) row;
END;
$function$
;

-- Permissions
------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_delete_site_project_detail(p_id integer, p_userid integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_result BOOLEAN := FALSE;
    v_message VARCHAR := 'Deletion is not allowed as the site was imported by another user.';
BEGIN
    IF p_id > 0 then
     
    if exists(select 1 from site_project_details
        WHERE id = p_id AND created_by = p_userid ) then 
        
        DELETE FROM site_project_details
        WHERE id = p_id AND created_by = p_userid;
       
       v_message =  'Record deleted successfully.';
       v_result = true;
      
       RETURN QUERY
    SELECT 
        v_result AS status,v_message  AS message;
       
     else
     RETURN QUERY
    SELECT 
        v_result AS status,v_message  AS message;
       
     end if;
       
    END IF;

    
END;
$function$
;
