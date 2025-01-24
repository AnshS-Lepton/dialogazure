
----------------------------     1.Entry in layer -- Every entity should be mentioned in this table  --------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.layer_details
(layer_name, isvisible, minzoomlevel, maxzoomlevel, minboundvalue, maxboundvalue, parent_layer_id, layer_abbr, map_abbr, layer_title, layer_form_url, layer_seq, is_network_entity, is_visible_in_ne_library, is_template_required, network_id_type, geom_type, template_form_url, layer_table, network_code_seperator, save_entity_url, is_direct_save, layer_view, is_clone, is_isp_layer, is_shaft_element, is_osp_layer, unit_type, unit_input_type, is_multi_clone, is_mobile_layer, is_visible_in_mobile_lib, is_vendor_spec_required, is_splicer, report_view_name, is_report_enable, is_networktype_required, is_isp_splicer, is_label_change_allowed, layer_network_group, is_multi_association, history_view_name, is_history_enabled, is_info_enabled, is_cpe_entity, is_virtual_port_allowed, is_reference_allowed, is_isp_child_layer, map_layer_seq, layer_template_table, is_isp_parent_layer, is_floor_element, is_feasibility_layer, is_osp_layer_freezed_in_library, is_isp_layer_freezed_in_library, feasibility_network_group, is_other_wcr_layer, is_barcode_enabled, barcode_column, is_at_enabled, is_maintainence_charges_enabled, is_row_association_enabled, is_logicalview_enabled, is_site_enabled, is_networkcode_change_enabled, is_middleware_entity, is_lmc_enabled, is_utilization_enabled, is_layer_for_rights_permission, is_data_upload_enabled, is_fault_entity, data_upload_table, data_upload_max_count, is_moredetails_enable, is_project_spec_allowed, is_fiber_link_enabled, is_visible_on_mobile_map, is_loop_allowed)
VALUES('PEP', true, 11, 21, 7, 19, 0, 'PEP', 'PEP', 'PEP', '/AddPep', 21, true, true, true, 'A', 'Point', 'ItemTemplate/PepTemplate', 'att_details_pep', '-', '/Savepep', false, 'vw_att_details_pep', true, false, false, true, NULL, 'other', true, true, true, true, false, 'vw_att_details_pep_report', true, true, false, true, 'Layers', true, 'vw_att_details_pep_audit', true, true, false, true, false, false, 10, 'item_template_pep', false, false, false, false, false, NULL, false, true, 'barcode', false, false, false, false, false, true, false, false, false, true, true, false, 'temp_du_pep', 1000, false, false, false, true, true);

---------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------- 2.Entry in layer_mapping  table --------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.layer_mapping(
            layer_id, parent_layer_id, parent_sequence, is_enable_inside_parent_info, 
            is_used_for_network_id, network_code_format, is_default_code_format)
    VALUES ((select layer_id from layer_Details where upper(layer_name)='PEP'),(select layer_id from layer_Details where upper(layer_name)='UNIT'),6,false,false,'PEPnnn',false),
	   ((select layer_id from layer_Details where upper(layer_name)='PEP'),0,1,false,true,'PEPnnn',true);
--------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------- 3. Creating the main entity table -----------------------------------
--------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.att_details_pep
(
    system_id serial NOT NULL,
    network_id character varying(100) COLLATE pg_catalog."default",
    pep_name character varying(100) COLLATE pg_catalog."default",
    latitude double precision,
    longitude double precision,
    province_id integer,
    region_id integer,
    address character varying(200) COLLATE pg_catalog."default",
    pep_no character varying(50) COLLATE pg_catalog."default",
    pep_height character varying(20) COLLATE pg_catalog."default",
    specification character varying(100) COLLATE pg_catalog."default",
    category character varying(100) COLLATE pg_catalog."default",
    subcategory1 character varying(100) COLLATE pg_catalog."default",
    subcategory2 character varying(100) COLLATE pg_catalog."default",
    subcategory3 character varying(100) COLLATE pg_catalog."default",
    item_code character varying(50) COLLATE pg_catalog."default",
    vendor_id integer,
    type integer,
    brand integer,
    model integer,
    construction integer,
    activation integer,
    accessibility integer,
    status character varying(10) COLLATE pg_catalog."default" DEFAULT 'A'::character varying,
    created_by integer,
    created_on timestamp without time zone DEFAULT now(),
    modified_by integer,
    modified_on timestamp without time zone,
    network_status character varying(25) COLLATE pg_catalog."default" NOT NULL DEFAULT 'P'::character varying,
    parent_system_id integer,
    parent_network_id character varying(100) COLLATE pg_catalog."default",
    parent_entity_type character varying(100) COLLATE pg_catalog."default",
    sequence_id integer,
    project_id integer DEFAULT 0,
    planning_id integer DEFAULT 0,
    purpose_id integer DEFAULT 0,
    workorder_id integer DEFAULT 0,
    is_used boolean DEFAULT false,
    barcode character varying(100) COLLATE pg_catalog."default",
    acquire_from character varying(200) COLLATE pg_catalog."default",
    third_party_vendor_id integer,
    ownership_type character varying(100) COLLATE pg_catalog."default",
    source_ref_type character varying(100) COLLATE pg_catalog."default",
    source_ref_id character varying COLLATE pg_catalog."default",
    source_ref_description character varying COLLATE pg_catalog."default",
    status_remark character varying(2000) COLLATE pg_catalog."default",
    status_updated_by integer,
    status_updated_on timestamp without time zone,
    audit_item_master_id integer,
    is_visible_on_map boolean DEFAULT true,
    primary_pod_system_id integer,
    secondary_pod_system_id integer,
    is_new_entity boolean NOT NULL DEFAULT false,
    remarks character varying(1000) COLLATE pg_catalog."default",
    is_acquire_from boolean DEFAULT false,
    other_info jsonb,
    origin_from character varying COLLATE pg_catalog."default",
    origin_ref_id character varying COLLATE pg_catalog."default",
    origin_ref_code character varying COLLATE pg_catalog."default",
    origin_ref_description character varying COLLATE pg_catalog."default",
    request_ref_id character varying COLLATE pg_catalog."default",
    requested_by character varying COLLATE pg_catalog."default",
    request_approved_by character varying COLLATE pg_catalog."default",
    subarea_id character varying COLLATE pg_catalog."default",
    area_id character varying COLLATE pg_catalog."default",
    dsa_id character varying COLLATE pg_catalog."default",
    csa_id character varying COLLATE pg_catalog."default",
    bom_sub_category character varying COLLATE pg_catalog."default",
    gis_design_id character varying(100) COLLATE pg_catalog."default",
    csa_system_id integer,
    dsa_system_id integer,
    area_system_id integer,
    subarea_system_id integer,
    codification_sequence integer,
    target_ref_code character varying COLLATE pg_catalog."default",
    target_ref_description character varying COLLATE pg_catalog."default",
    target_ref_id integer,
    st_x double precision,
    st_y double precision,
    ne_id character varying COLLATE pg_catalog."default",
    prms_id character varying COLLATE pg_catalog."default",
    jc_id character varying COLLATE pg_catalog."default",
    mzone_id character varying COLLATE pg_catalog."default",
    served_by_ring character varying COLLATE pg_catalog."default",
    sp_geolocation geometry,
    is_manual_barcode boolean DEFAULT false,
    is_barcode_verified boolean DEFAULT false,
    own_vendor_id character varying(100) COLLATE pg_catalog."default",
    hierarchy_type character varying(1000) COLLATE pg_catalog."default",
    CONSTRAINT att_details_pep_pkey PRIMARY KEY (system_id)
)
-------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------- 4. Creating a table for audit or change the information of entity from info toolbar ----------
-------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.audit_att_details_pep
(
    audit_id serial NOT NULL,
    system_id integer,
    network_id character varying(100) COLLATE pg_catalog."default",
    pep_name character varying(100) COLLATE pg_catalog."default",
    latitude double precision,
    longitude double precision,
    province_id integer,
    region_id integer,
    address character varying(200) COLLATE pg_catalog."default",
    pep_no character varying(50) COLLATE pg_catalog."default",
    pep_height character varying(20) COLLATE pg_catalog."default",
    specification character varying(100) COLLATE pg_catalog."default",
    category character varying(100) COLLATE pg_catalog."default",
    subcategory1 character varying(100) COLLATE pg_catalog."default",
    subcategory2 character varying(100) COLLATE pg_catalog."default",
    subcategory3 character varying(100) COLLATE pg_catalog."default",
    item_code character varying(50) COLLATE pg_catalog."default",
    vendor_id integer,
    type integer,
    brand integer,
    model integer,
    construction integer,
    activation integer,
    accessibility integer,
    status character varying(10) COLLATE pg_catalog."default" DEFAULT 'A'::character varying,
    created_by integer,
    created_on timestamp without time zone DEFAULT now(),
    modified_by integer,
    modified_on timestamp without time zone,
    network_status character varying(25) COLLATE pg_catalog."default" NOT NULL DEFAULT 'P'::character varying,
    parent_system_id integer,
    parent_network_id character varying(100) COLLATE pg_catalog."default",
    parent_entity_type character varying(100) COLLATE pg_catalog."default",
    sequence_id integer,
    project_id integer DEFAULT 0,
    planning_id integer DEFAULT 0,
    purpose_id integer DEFAULT 0,
    workorder_id integer DEFAULT 0,
    action character varying(10) COLLATE pg_catalog."default",
    acquire_from character varying(200) COLLATE pg_catalog."default",
    third_party_vendor_id integer,
    ownership_type character varying(100) COLLATE pg_catalog."default",
    source_ref_type character varying(100) COLLATE pg_catalog."default",
    source_ref_id character varying COLLATE pg_catalog."default",
    source_ref_description character varying COLLATE pg_catalog."default",
    status_remark character varying(2000) COLLATE pg_catalog."default",
    status_updated_by integer,
    status_updated_on timestamp without time zone,
    audit_item_master_id integer,
    is_visible_on_map boolean DEFAULT true,
    primary_pod_system_id integer,
    secondary_pod_system_id integer,
    remarks character varying(1000) COLLATE pg_catalog."default",
    is_acquire_from boolean DEFAULT false,
    other_info jsonb,
    ticket_id integer,
    origin_from character varying COLLATE pg_catalog."default",
    origin_ref_id character varying COLLATE pg_catalog."default",
    origin_ref_code character varying COLLATE pg_catalog."default",
    origin_ref_description character varying COLLATE pg_catalog."default",
    request_ref_id character varying COLLATE pg_catalog."default",
    requested_by character varying COLLATE pg_catalog."default",
    request_approved_by character varying COLLATE pg_catalog."default",
    subarea_id character varying COLLATE pg_catalog."default",
    area_id character varying COLLATE pg_catalog."default",
    dsa_id character varying COLLATE pg_catalog."default",
    csa_id character varying COLLATE pg_catalog."default",
    bom_sub_category character varying COLLATE pg_catalog."default",
    gis_design_id character varying(100) COLLATE pg_catalog."default",
    csa_system_id integer,
    dsa_system_id integer,
    area_system_id integer,
    subarea_system_id integer,
    target_ref_code character varying COLLATE pg_catalog."default",
    target_ref_description character varying COLLATE pg_catalog."default",
    target_ref_id integer,
    st_x double precision,
    st_y double precision,
    ne_id character varying COLLATE pg_catalog."default",
    prms_id character varying COLLATE pg_catalog."default",
    jc_id character varying COLLATE pg_catalog."default",
    mzone_id character varying COLLATE pg_catalog."default",
    action_date timestamp without time zone,
    served_by_ring character varying COLLATE pg_catalog."default"
)
------------------------------------------------------------------------------------------------------------------------------
-------------------------------- 5. Creating a new table for entity template -------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.item_template_pep
(
    id serial NOT NULL,
    specification character varying(100) COLLATE pg_catalog."default",
    category character varying(100) COLLATE pg_catalog."default",
    subcategory1 character varying(100) COLLATE pg_catalog."default",
    subcategory2 character varying(100) COLLATE pg_catalog."default",
    subcategory3 character varying(100) COLLATE pg_catalog."default",
    item_code character varying(50) COLLATE pg_catalog."default",
    vendor_id integer,
    type integer,
    brand integer,
    model integer,
    construction integer,
    activation integer,
    accessibility integer,
    created_by integer,
    created_on timestamp without time zone DEFAULT now(),
    modified_by integer,
    modified_on timestamp without time zone,
    audit_item_master_id integer,
    hierarchy_type character varying(100) COLLATE pg_catalog."default",
    CONSTRAINT item_template_pep_pkey PRIMARY KEY (id)
)

--------------------------------------------------------------------------------------------------------------------------------------------------
--------------------------------------------- 2-For storing change in template table ---------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.audit_item_template_pep
(
    audit_id serial NOT NULL,
    id integer,
    specification character varying(100) COLLATE pg_catalog."default",
    category character varying(100) COLLATE pg_catalog."default",
    subcategory1 character varying(100) COLLATE pg_catalog."default",
    subcategory2 character varying(100) COLLATE pg_catalog."default",
    subcategory3 character varying(100) COLLATE pg_catalog."default",
    item_code character varying(50) COLLATE pg_catalog."default",
    vendor_id integer,
    type integer,
    brand integer,
    model integer,
    construction integer,
    activation integer,
    accessibility integer,
    created_by integer,
    created_on timestamp without time zone DEFAULT now(),
    modified_by integer,
    modified_on timestamp without time zone,
    action character varying(10) COLLATE pg_catalog."default",
    audit_item_master_id integer
)

--------------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------- 3. Table fpr data uploader -----------------------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.temp_du_pep
(
    system_id serial NOT NULL,
    network_id character varying COLLATE pg_catalog."default",
    pep_name character varying COLLATE pg_catalog."default",
    latitude character varying COLLATE pg_catalog."default",
    longitude character varying COLLATE pg_catalog."default",
    province_id integer DEFAULT 0,
    region_id integer DEFAULT 0,
    pep_no character varying COLLATE pg_catalog."default",
    pep_height character varying COLLATE pg_catalog."default",
    address character varying COLLATE pg_catalog."default",
    specification character varying COLLATE pg_catalog."default",
    specification_id integer DEFAULT 0,
    vendor_id integer DEFAULT 0,
    vendor_name character varying COLLATE pg_catalog."default",
    parent_system_id integer DEFAULT 0,
    parent_network_id character varying COLLATE pg_catalog."default",
    parent_entity_type character varying COLLATE pg_catalog."default",
    upload_id integer,
    created_by integer,
    created_on timestamp without time zone DEFAULT now(),
    is_valid boolean,
    error_msg character varying COLLATE pg_catalog."default",
    category character varying COLLATE pg_catalog."default",
    subcategory1 character varying COLLATE pg_catalog."default",
    subcategory2 character varying COLLATE pg_catalog."default",
    subcategory3 character varying COLLATE pg_catalog."default",
    batch_id integer,
    item_code character varying COLLATE pg_catalog."default",
    is_exists_in_boundary boolean DEFAULT false,
    is_processed boolean DEFAULT false,
    row_order integer DEFAULT 0,
    audit_item_master_id integer,
    remarks character varying COLLATE pg_catalog."default",
    origin_from character varying COLLATE pg_catalog."default",
    origin_ref_id character varying COLLATE pg_catalog."default",
    origin_ref_code character varying COLLATE pg_catalog."default",
    origin_ref_description character varying COLLATE pg_catalog."default",
    request_ref_id character varying COLLATE pg_catalog."default",
    requested_by character varying COLLATE pg_catalog."default",
    request_approved_by character varying COLLATE pg_catalog."default",
    network_status character varying COLLATE pg_catalog."default",
    st_x double precision,
    st_y double precision
)
-----------------------------------------------------------------------------------------------------------------------------------
------------------------------------------ 4. For inserting data in dropdowns (in template and PEP both) --------------------------
-----------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active)
VALUES((select layer_id from layer_details where layer_name='PEP'), 'type', '1', true, 5, now(), 5, now(), 'Type1', NULL, 'type', true, true);

insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,modified_by,modified_on,dropdown_key,parent_value,db_column_name,is_action_allowed,is_active)
values 
((select layer_id from layer_details where layer_name='PEP'),'PEP_Type','Type 15',true,1,now(),1,now(),'Type 15',null,'PEP_Type',true,true),
((select layer_id from layer_details where layer_name='PEP'),'PEP_Type','Type 16',true,1,now(),1,now(),'Type 16',null,'PEP_Type',true,true),
((select layer_id from layer_details where layer_name='PEP'),'PEP_Type','Type 17',true,1,now(),1,now(),'Type 17',null,'PEP_Type',true,true),
((select layer_id from layer_details where layer_name='PEP'),'PEP_Type','Type 18',true,1,now(),1,now(),'Type 18',null,'PEP_Type',true,true);

------------------------------------------------------------------------------------------------------------------------------------
----------------------------------- 5. For getting that entity in search -----------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------

insert into search_settings (layer_id,search_columns,created_by,created_on,modified_by,modified_on)
values((select layer_id from layer_details where layer_name='PEP'),'network_id',1,now(),null,null);
------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------6. For showing all the icon in info of PEP ------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO layer_action_mapping(
            layer_id, action_name, is_active, action_sequence, action_title, 
            is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, 
            action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, 
            res_field_key, is_enable_in_draft)
    VALUES ((select layer_id from layer_Details where upper(layer_name)='PEP'),'ViewAccessories',true,24,'View Accessories',true,true,true,'M',0,61,false,true,0,'SI_GBL_GBL_GBL_FRM_005',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'Cancel',true,7,'Cancel',false,false,true,'',0,0,false,true,0,'GBL_GBL_GBL_GBL_GBL_006',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'Detail',true,4,'Detail',true,true,true,'',0,0,false,true,0,'SI_ISP_GBL_GBL_GBL_007',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'EntityRoomView',true,23,'RoomView',true,false,true,'',0,0,false,true,0,'SI_GBL_GBL_GBL_FRM_009',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'Export',true,1,'Export',true,true,true,'',0,0,false,true,0,'GBL_GBL_GBL_GBL_GBL_010',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'History',true,3,'History',true,true,true,'',0,0,false,true,0,'SI_OSP_GBL_NET_FRM_339',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'RoomView',true,15,'Room View',true,true,false,'',0,0,false,true,0,'SI_ISP_GBL_NET_FRM_079',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'Save',true,6,'Save',false,false,true,'',0,0,false,true,0,'SI_OSP_SBA_NET_FRM_013',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'Shift',true,11,'Shift',true,true,false,'E',0,0,false,true,0,'SI_GBL_GBL_GBL_FRM_003',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'UploadImage',true,9,'Upload Image/Document',true,false,true,'E',0,0,false,true,0,'SI_GBL_GBL_GBL_GBL_132',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'Zoom',true,2,'Zoom on Map',true,false,true,'',0,0,false,true,0,'SI_OSP_GBL_NET_FRM_252',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'ConvertToAsBuilt',true,13,'Convert To AsBuilt',true,true,true,'NA',0,0,false,true,0,'SI_GBL_GBL_GBL_FRM_006',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'ConvertToDormant',true,14,'Convert To Dormant',true,true,true,'ND',0,0,false,true,0,'SI_OSP_GBL_NET_RPT_031',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'ConvertToPlanned',true,12,'Convert To Planned',true,true,true,'NP',0,0,false,true,0,'SI_OSP_GBL_NET_FRM_189',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'Delete',true,8,'Delete',true,true,true,'D',0,0,false,true,0,'SI_GBL_GBL_GBL_GBL_002',true),
((select layer_id from layer_Details where upper(layer_name)='PEP'),'LocationEdit',true,5,'Location Edit',true,false,true,'E',0,0,false,true,0,'SI_OSP_GBL_NET_FRM_254',true);

--------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------- 7.For setting automatic save off(if on)--------------------------------------------------
--------------------------------------------------------------------------------------------------------------------------------------------

update layer_details set is_direct_save = false where layer_name='PEP';

-------------------------------------------------------------------------------------------------------------------------------------------
--------------------------  8. Some important View for getting the consolidated report----------------------------------------------------
-------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE VIEW public.vw_att_details_pep_report
 AS
 SELECT pep.system_id,
    pep.network_id,
    pep.pep_name,
    pep.pep_height,
    pep.pep_no,
    round(pep.latitude::numeric, 6) AS latitude,
    round(pep.longitude::numeric, 6) AS longitude,
    pep.address,
    pep.specification,
    pep.category,
    pep.subcategory1,
    pep.subcategory2,
    pep.subcategory3,
    pep.item_code,
    pep.accessibility,
    pep.acquire_from,
        CASE
            WHEN pep.network_status::text = 'P'::text THEN 'Planned'::text
            WHEN pep.network_status::text = 'A'::text THEN 'As Built'::text
            WHEN pep.network_status::text = 'D'::text THEN 'Dormant'::text
            ELSE NULL::text
        END AS network_status,
    COALESCE(es.description, pep.status) AS status,
    pep.parent_network_id AS parent_code,
    pep.parent_entity_type AS parent_type,
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
    to_char(pep.created_on, 'DD-Mon-YY'::text) AS created_on,
    um.user_name AS created_by,
    to_char(pep.modified_on, 'DD-Mon-YY'::text) AS modified_on,
    um2.user_name AS modified_by,
    pep.region_id,
    pep.province_id,
        CASE
            WHEN pep.is_used = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_used,
    pep.barcode,
    pm.system_id AS project_id,
    plningm.system_id AS planning_id,
    workorm.system_id AS workorder_id,
    purposem.system_id AS purpose_id,
    um.user_id AS created_by_id,
    pep.ownership_type,
    vm2.name AS third_party_vendor_name,
    vm2.id AS third_party_vendor_id,
    pep.source_ref_type,
    pep.source_ref_id,
    pep.source_ref_description,
    pep.status_remark,
    pep.status_updated_by,
    to_char(pep.status_updated_on, 'DD-Mon-YY'::text) AS status_updated_on,
    pep.is_visible_on_map,
    primarypod.network_id AS primary_pod_network_id,
    secondarypod.network_id AS secondary_pod_network_id,
    primarypod.pod_name AS primary_pod_name,
    secondarypod.pod_name AS secondary_pod_name,
    pep.remarks,
    reg.country_name,
    pep.origin_from,
    pep.origin_ref_id,
    pep.origin_ref_code,
    pep.origin_ref_description,
    pep.request_ref_id,
    pep.requested_by,
    pep.request_approved_by,
    pep.subarea_id,
    pep.area_id,
    pep.dsa_id,
    pep.csa_id,
    pep.bom_sub_category,
    pep.gis_design_id AS design_id,
    pep.st_x,
    pep.st_y,
    pep.ne_id,
    pep.prms_id,
    pep.jc_id,
    pep.mzone_id,
    pep.served_by_ring,
    vm3.name AS own_vendor_name
   FROM att_details_pep pep
     JOIN province_boundary prov ON pep.province_id = prov.id
     JOIN region_boundary reg ON pep.region_id = reg.id
     JOIN user_master um ON pep.created_by = um.user_id
     JOIN vendor_master vm ON pep.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pep.third_party_vendor_id = vm2.id
     LEFT JOIN vendor_master vm3 ON pep.own_vendor_id::integer = vm3.id
     LEFT JOIN att_details_project_master pm ON pep.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON pep.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON pep.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON pep.purpose_id = purposem.system_id
     LEFT JOIN user_master um2 ON pep.modified_by = um2.user_id
     LEFT JOIN att_details_pod primarypod ON pep.primary_pod_system_id = primarypod.system_id
     LEFT JOIN att_details_pod secondarypod ON pep.secondary_pod_system_id = secondarypod.system_id
     LEFT JOIN entity_status_master es ON es.status::text = pep.status::text;
------------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------9. CREATE OR REPLACE VIEW public.vw_att_details_pep_map AS--------------------------------------------------
------------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE VIEW public.vw_att_details_pep_map
 AS
 SELECT pep.system_id,
    pep.region_id,
    pep.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    pep.pep_name,
    pep.project_id,
    pep.planning_id,
    pep.workorder_id,
    pep.purpose_id,
    pep.network_id,
    pep.status,
    pep.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pep.network_status::text) AS network_status,
    pep.ownership_type,
    pep.third_party_vendor_id,
    pep.primary_pod_system_id,
    pep.secondary_pod_system_id,
    pep.source_ref_id,
    pep.source_ref_type,
    lim.icon_path,
    pep.network_id AS label_column
   FROM point_master point
     JOIN att_details_pep pep ON point.system_id = pep.system_id AND point.entity_type::text = 'PEP'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = pep.system_id AND adi1.entity_type::text = 'PEP'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pep.network_status::text) AND lim.layer_name::text = 'PEP'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = pep.system_id AND adi.entity_type::text = 'PEP'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

-----------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------- 10. View for getting PEP audited data ------------------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------



CREATE OR REPLACE VIEW public.vw_att_details_pep_audit_oi
 AS
 SELECT audit_att_details_pep.other_info::json ->> 'is_wall'::text AS is_wall,
    audit_att_details_pep.other_info::json ->> 'record_system_id'::text AS record_system_id,
    audit_att_details_pep.audit_id
   FROM audit_att_details_pep
  WHERE (audit_att_details_pep.other_info ->> 'record_system_id'::text) <> '0'::text AND (audit_att_details_pep.other_info ->> 'record_system_id'::text) <> ''::text OR (audit_att_details_pep.other_info ->> 'record_system_id'::text) IS NULL;

-----------------------------------------------------------------------

CREATE OR REPLACE VIEW public.vw_att_details_pep_audit
 AS
 SELECT pep.audit_id,
    pep.system_id,
    pep.network_id,
    pep.pep_name,
    round(pep.latitude::numeric, 6) AS latitude,
    round(pep.longitude::numeric, 6) AS longitude,
    reg.region_name,
    prov.province_name,
    pep.pep_height,
    pep.pep_no,
    pep.address,
    pep.specification,
    vm.name AS vendor_name,
    pep.item_code,
    pep.category,
    pep.subcategory1,
    pep.subcategory2,
    pep.vendor_id,
    pep.parent_system_id,
    pep.parent_network_id,
    pep.parent_entity_type,
    pep.acquire_from,
    tp.type AS pep_type,
    bd.brand AS pep_brand,
    ml.model AS pep_model,
    fn_get_date(pep.created_on) AS created_on,
    um.user_name AS created_by,
    fn_get_date(pep.modified_on) AS modified_on,
    ums.user_name AS modified_by,
    fn_get_network_status(pep.network_status) AS network_status,
    fn_get_status(pep.status) AS status,
    fn_get_action(pep.action) AS action,
    pep.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    pep.source_ref_type,
    pep.source_ref_id,
    pep.source_ref_description,
    pep.status_remark,
    pep.status_updated_by,
    pep.status_updated_on,
    pep.is_visible_on_map,
    pep.primary_pod_system_id,
    pep.secondary_pod_system_id,
    pep.remarks,
    pep.origin_from,
    pep.origin_ref_id,
    pep.origin_ref_code,
    pep.origin_ref_description,
    pep.request_ref_id,
    pep.requested_by,
    pep.request_approved_by,
    pep.subarea_id,
    pep.area_id,
    pep.dsa_id,
    pep.csa_id,
    pep.bom_sub_category,
    pep.gis_design_id,
    pep.ne_id,
    pep.prms_id,
    pep.jc_id,
    pep.mzone_id,
    oi.is_wall,
    oi.record_system_id
   FROM audit_att_details_pep pep
     JOIN province_boundary prov ON pep.province_id = prov.id
     JOIN region_boundary reg ON pep.region_id = reg.id
     JOIN user_master um ON pep.created_by = um.user_id
     LEFT JOIN user_master ums ON pep.modified_by = ums.user_id
     JOIN vendor_master vm ON pep.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pep.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON pep.type = tp.id
     LEFT JOIN isp_brand_master bd ON pep.brand = bd.id
     LEFT JOIN isp_base_model ml ON pep.model = ml.id
     JOIN vw_att_details_pep_audit_oi oi ON oi.audit_id = pep.audit_id;

---------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------- 11. View for getting PEP data ----------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE VIEW public.vw_att_details_pep
 AS
 SELECT pep.system_id,
    pep.network_id,
    pep.pep_name,
    pep.pep_height,
    pep.pep_no,
    round(pep.latitude::numeric, 6) AS latitude,
    round(pep.longitude::numeric, 6) AS longitude,
    pep.address,
    pep.specification,
    pep.category,
    pep.subcategory1,
    pep.subcategory2,
    pep.subcategory3,
    pep.item_code,
    pep.vendor_id,
    pep.type,
    pep.brand,
    pep.model,
    pep.construction,
    pep.activation,
    pep.accessibility,
    pep.status,
    pep.network_status,
    pep.province_id,
    pep.region_id,
    pep.created_by,
    pep.created_on,
    pep.modified_by,
    pep.modified_on,
    prov.province_name,
    reg.region_name,
    pep.parent_system_id,
    pep.parent_network_id,
    pep.parent_entity_type,
    pep.sequence_id,
    pep.project_id,
    pep.planning_id,
    pep.purpose_id,
    pep.workorder_id,
    pep.acquire_from,
    um.user_name,
    vm.name AS vendor_name,
    tp.type AS pep_type,
    bd.brand AS pep_brand,
    ml.model AS pep_model,
    fn_get_date(pep.created_on) AS created_date,
    fn_get_date(pep.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pep.accessibility) AS pep_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pep.construction) AS pep_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = pep.activation) AS pep_activation,
    pep.barcode,
    pep.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    pep.source_ref_type,
    pep.source_ref_id,
    pep.source_ref_description,
    pep.status_remark,
    pep.status_updated_by,
    pep.status_updated_on,
    pep.is_visible_on_map,
    pep.primary_pod_system_id,
    pep.secondary_pod_system_id,
    pep.is_new_entity,
    pep.remarks,
    pep.audit_item_master_id,
    pep.is_acquire_from,
    pep.origin_from,
    pep.origin_ref_id,
    pep.origin_ref_code,
    pep.origin_ref_description,
    pep.request_ref_id,
    pep.requested_by,
    pep.request_approved_by,
    pep.subarea_id,
    pep.area_id,
    pep.dsa_id,
    pep.csa_id,
    pep.bom_sub_category,
    pep.gis_design_id,
    pep.st_x,
    pep.st_y,
    pep.ne_id,
    pep.prms_id,
    pep.jc_id,
    pep.mzone_id,
    pep.own_vendor_id
   FROM att_details_pep pep
     JOIN province_boundary prov ON pep.province_id = prov.id
     JOIN region_boundary reg ON pep.region_id = reg.id
     JOIN user_master um ON pep.created_by = um.user_id
     JOIN vendor_master vm ON pep.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON pep.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON pep.type = tp.id
     LEFT JOIN isp_brand_master bd ON pep.brand = bd.id
     LEFT JOIN isp_base_model ml ON pep.model = ml.id
     LEFT JOIN user_master um2 ON pep.modified_by = um2.user_id;

------------------------------------------------------------------------------------------------------------------------------------
-------------------12. For getting the template data , you should add your entity column in below function just like PEP in "fn_get_template_detail"--------
------------------------------------------------------------------------------------------------------------------------------------

-- FUNCTION: public.fn_get_template_detail(integer, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_template_detail(integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_template_detail(
	p_userid integer,
	p_entitytype character varying,
	p_sub_entitytype character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


DECLARE s_tblname character varying ; 
     sql text;
BEGIN

IF upper(p_entitytype) = 'ADB' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_input_port,a.no_of_output_port,a.no_of_port,a.entity_category,a.audit_item_master_id,'ADB'as entitytype from item_template_adb a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'BDB' THEN

RETURN QUERY select row_to_json(row) from ( 
select * from (
select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_input_port,a.no_of_output_port,a.no_of_port,a.entity_category,a.audit_item_master_id,'BDB'as entitytype  from item_template_bdb a 
where a.created_by=0

union

select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_input_port,a.no_of_output_port,a.no_of_port,a.entity_category,a.audit_item_master_id,'BDB'as entitytype   from item_template_bdb a 
where a.created_by=p_userid)a  order by created_by desc  limit 1

) row;

ELSIF upper(p_entitytype) = 'CDB' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_input_port,a.no_of_output_port,a.no_of_port,a.entity_category,a.audit_item_master_id,'CDB'as entitytype   from item_template_cdb a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'POD' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.pod_type,a.audit_item_master_id,'POD'as entitytype  from item_template_pod a where a.created_by=p_userid  limit 1) row;
ELSIF upper(p_entitytype) = 'MPOD' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.mpod_type,a.audit_item_master_id,'MPOD'as entitytype  from item_template_mpod a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'TREE' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Tree'as entitytype  from item_template_tree a where a.created_by=p_userid  limit 1) row;
ELSIF upper(p_entitytype) = 'POLE' THEN

RETURN QUERY select row_to_json(row) from ( 
select * from (
select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.pole_type,a.audit_item_master_id,'Pole'as entitytype from item_template_pole a where a.created_by=0 
union
select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.pole_type,a.audit_item_master_id,'Pole'as entitytype from item_template_pole a where a.created_by=p_userid )a 
order by created_by desc limit 1
) row;

ELSIF upper(p_entitytype) = 'SPLITTER' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,
a.type,
a.splitter_type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.splitter_ratio,a.audit_item_master_id,'Splitter'as entitytype 
from item_template_splitter a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'MANHOLE' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,a.manhole_types,'Manhole'as entitytype  from item_template_manhole a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'CABLE' THEN

RETURN QUERY select row_to_json(row) from ( 
select * from(
select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model,a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on, a.cable_type,COALESCE (a.total_core,0) total_core,COALESCE (a.no_of_tube,0) no_of_tube,
COALESCE (a.no_of_core_per_tube,0) no_of_core_per_tube,a.cable_category ,a.cable_sub_category,a.audit_item_master_id,'Cable'as entitytype from item_template_cable a 
where a.created_by=0 and case when p_sub_entitytype ='' then a.cable_type!='ISP' else a.cable_type='ISP' end
union
select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model,a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on, a.cable_type,COALESCE (a.total_core,0) total_core,COALESCE (a.no_of_tube,0) no_of_tube,
COALESCE (a.no_of_core_per_tube,0) no_of_core_per_tube,a.cable_category ,a.cable_sub_category,a.audit_item_master_id,'Cable'as entitytype from item_template_cable a 
where a.created_by=p_userid and case when p_sub_entitytype ='' then a.cable_type!='ISP' else a.cable_type='ISP' end)a order by created_by desc
limit 1 
) row;

ELSIF upper(p_entitytype) = 'SPLICECLOSURE' THEN

RETURN QUERY select row_to_json(row) from ( 
select * from(
select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_ports,a.no_of_input_port,a.no_of_output_port,a.audit_item_master_id,'SpliceClosure'as entitytype  
from item_template_spliceclosure a where a.created_by=p_userid  

union

select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_ports,a.no_of_input_port,a.no_of_output_port,a.audit_item_master_id,'SpliceClosure'as entitytype  
from item_template_spliceclosure a where a.created_by=0  )b
order by created_by desc limit 1
) row;

ELSIF upper(p_entitytype) = 'FMS' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_port,a.no_of_input_port,a.no_of_output_port,a.audit_item_master_id,'FMS'as entitytype   from item_template_fms a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'ONT' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_input_port,a.no_of_output_port,a.audit_item_master_id,'ONT' as entitytype  from item_template_ont a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'TRENCH' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,trench_type,trench_height,trench_width,a.audit_item_master_id,trench_serving_type,'Trench' as entitytype   from item_template_trench a where a.created_by=p_userid  limit 1) row; 

ELSIF upper(p_entitytype) = 'DUCT' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Duct' as entitytype   from item_template_duct a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'CONDUIT' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Conduit' as entitytype  from item_template_conduit a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'MICRODUCT' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,a.no_of_ways,'MicroDuct' as entitytype  from item_template_microduct a where a.created_by=p_userid  limit 1) row; 

ELSIF upper(p_entitytype) = 'WALLMOUNT' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'WallMount' as entitytype   from item_template_wallmount a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'PEP' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'PEP' as entitytype   from item_template_pep a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'HTB' THEN

RETURN QUERY select row_to_json(row) from ( select a.system_id as id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_port,a.audit_item_master_id,'HTB' as entitytype   from isp_template_htb a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'FDB' THEN

RETURN QUERY select row_to_json(row) from (
select * from(
select a.system_id as id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_port,a.audit_item_master_id,'FDB' as entitytype from isp_template_fdb a where a.created_by=0
union

select a.system_id as id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_port,a.audit_item_master_id,'FDB' as entitytype from isp_template_fdb a where a.created_by=p_userid)a order by created_by desc limit 1

) row;

ELSIF upper(p_entitytype) = 'COUPLER' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Coupler' as entitytype  from item_template_coupler a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'ROW' THEN

RETURN QUERY select row_to_json(row) from ( 
--select a.id,a.type,a.width,
--a.created_by,a.created_on,a.modified_by,a.modified_on from item_template_row a where a.created_by=p_userid  limit 1

(select a.id,a.type,a.width,
a.created_by,a.created_on,a.modified_by,a.modified_on  from (
select a.id,a.type,a.width,
a.created_by,a.created_on,a.modified_by,a.modified_on,'ROW' as entitytype from item_template_row a where created_by=0
union
select b.id,b.type,b.width,
b.created_by,b.created_on,b.modified_by,b.modified_on,'ROW' as entitytype  from item_template_row b  where created_by=p_userid) a order by created_by desc  limit 1)

) row;

ELSIF upper(p_entitytype) = 'TOWER' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Tower' as entitytype   from item_template_tower a where a.created_by=p_userid  limit 1) row; 

ELSIF upper(p_entitytype) = 'SECTOR' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Sector' as entitytype  from item_template_sector a where a.created_by=p_userid  limit 1) row; 


ELSIF upper(p_entitytype) = 'ANTENNA' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Antenna' as entitytype  from item_template_Antenna a where a.created_by=p_userid  limit 1) row; 
ELSIF upper(p_entitytype) = 'PATCHCORD' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model,a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.patch_cord_category ,a.patch_cord_sub_category,a.audit_item_master_id,'PatchCord' as entitytype  from item_template_patchcord a 
where a.created_by in(p_userid,0) order by a.created_by desc 
limit 1 ) row;
ELSIF upper(p_entitytype) = 'PIT' THEN

RETURN QUERY select row_to_json(row) from (  select a.id,a.radius,
a.created_by,a.created_on,a.modified_by,a.modified_on,'PIT' as entitytype  from item_template_pit a 
where a.created_by in(p_userid,0) order by a.created_by desc 
limit 1 ) row;
    
ELSIF upper(p_entitytype) = 'CABINET' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.cabinet_type,a.audit_item_master_id,'Cabinet' as entitytype  from item_template_Cabinet a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'VAULT' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.vault_type,a.audit_item_master_id,'Vault' as entitytype from item_template_vault a where a.created_by=p_userid  limit 1) row;
ELSIF upper(p_entitytype) = 'OPTICALREPEATER' THEN
RETURN QUERY select row_to_json(row) from ( select a.system_id as id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_port,a.audit_item_master_id,'OpticalRepeater' as entitytype  from isp_template_opticalrepeater a where a.created_by=p_userid  limit 1) row;
ELSIF upper(p_entitytype) = 'HANDHOLE' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Handhole' as entitytype  from item_template_handhole a where a.created_by=p_userid  limit 1) row;
ELSIF upper(p_entitytype) = 'OPTICALREPEATER' THEN

RETURN QUERY select row_to_json(row) from ( select a.system_id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'OpticalRepeater' as entitytype  from isp_template_opticalrepeater a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'PATCHPANEL' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.no_of_port,a.no_of_input_port,a.no_of_output_port,a.audit_item_master_id,'PatchPanel' as entitytype  from item_template_patchpanel a where a.created_by=p_userid  limit 1) row;

ELSIF upper(p_entitytype) = 'GIPIPE' THEN

RETURN QUERY select row_to_json(row) from ( select a.id,a.specification,a.category,a.subcategory1,a.subcategory2,
a.subcategory3,a.item_code,a.vendor_id,a.type,a.brand,a.model, a.activation,a.accessibility,a.construction,
a.created_by,a.created_on,a.modified_by,a.modified_on,a.audit_item_master_id,'Gipipe' as entitytype   from item_template_gipipe a where a.created_by=p_userid  limit 1) row;
END IF;
END;
$BODY$;

ALTER FUNCTION public.fn_get_template_detail(integer, character varying, character varying)
    OWNER TO postgres;

----------------------------------------------------------------------------------------------------------------------------------------------
-----------------------------13. To create a new function for inserting  data uploader data---------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_uploader_insert_pep(
	p_uploadid integer,
	p_batchid integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

declare
REC RECORD;
v_current_system_id integer;
v_parent_system_id integer;
v_parent_network_id character varying;
v_parent_entity_type character varying;
v_network_code character varying;
v_sequence_id integer;
v_latitude double precision;
v_longitude double precision;
v_network_id_type character varying;
v_network_code_seperator character varying;
v_auto_character_count integer;
v_network_name character varying;

BEGIN
-- GET NETWORK ID TYPE (AUTO/MANUAL) AND NETWORK CODE SEPERATOR FROM LAYER DETAILS..
select network_id_type,network_code_seperator into v_network_id_type,v_network_code_seperator from layer_details where upper(layer_name)='PEP';

-- INSERT BATCH WISE RECORDS..
FOR REC IN select * from temp_du_pep where upload_id=p_uploadid and is_valid=true and batch_id=p_batchid and is_processed = false
LOOP
BEGIN

v_latitude:=rec.latitude::double precision;
v_longitude:=rec.longitude::double precision;
v_sequence_id:=0;
v_network_name:=rec.pep_name;
-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
then
v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
from vw_layer_mapping where upper(child_layer_name)=upper('PEP') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
else
-- GET NETWORK CODE & PARENT DETAILS..

select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
from fn_get_clone_network_code('PEP', 'Point',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
end if;

IF(coalesce(v_network_name,'')='')
then
v_network_name=v_network_code;
END IF;
--INSERT INTO MAIN TABLE
insert into att_details_pep(network_id, pep_name, latitude, longitude, province_id, region_id,
pep_no, pep_height, address,specification,category,subcategory1,subcategory2,subcategory3,
item_code,vendor_id, status, created_by, created_on, network_status,
parent_system_id, parent_network_id, parent_entity_type, sequence_id,type,brand,model,construction,activation,
accessibility,ownership_type,source_ref_type, source_ref_id, remarks,origin_from,origin_Ref_id,
origin_Ref_code,origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,st_x,st_y)
select v_network_code, v_network_name, v_latitude, v_longitude, rec.province_id, rec.region_id,
rec.pep_no, rec.pep_height, rec.address,rec.specification,rec.category,rec.subcategory1,
rec.subcategory2,rec.subcategory3,rec.item_code,rec.vendor_id, 'A', rec.created_by, now(),coalesce(rec.network_status,'P'),
rec.parent_system_id,rec.parent_network_id,REC.PARENT_ENTITY_TYPE, v_sequence_id,0,0,0,0,0,0,'Own', 'DU', p_uploadid, rec.remarks,
rec.origin_from,rec.origin_Ref_id,rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,
rec.requested_by,rec.request_approved_by,'Proposed',rec.st_x,rec.st_y returning system_id into v_current_system_id;

--INSERT INTO POINT MASTER
insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,network_status,display_name,st_x,st_y)
select v_current_system_id, 'PEP', v_longitude, v_latitude, 'A', ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')', 4326),
now(),'NA','NA',rec.created_by,0, v_network_code,rec.upload_id,coalesce(rec.network_status,'P'),fn_get_display_name(v_current_system_id,'PEP'),rec.st_x,rec.st_y;

perform (fn_geojson_update_entity_attribute(v_current_system_id::integer,'PEP'::character varying ,rec.province_id::integer,0,false));

-- UPDATE STATUS INTO TEMP TABLE..
update temp_du_pep set is_processed=true where system_id=rec.system_id;

END;
END LOOP;
return 1;
END;
$BODY$;

ALTER FUNCTION public.fn_uploader_insert_pep(integer, integer)
    OWNER TO postgres;

------------------------------------------------------------------------------------------------------------------------------------
--------------------14. For getting the element template (*Note : Wallmount not found inside this function "fn_getelementtemplate")-
------------------------------------------------------------------------------------------------------------------------------------

-- FUNCTION: public.fn_getelementtemplate(character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_getelementtemplate(character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_getelementtemplate(
	elementtype character varying,
	userid integer)
    RETURNS TABLE(systemid integer, element_type character varying, element_template_name character varying, element_height double precision, element_width double precision, element_length double precision, template_form_url character varying, istemplatefilled boolean, entity_category character varying, entityclass character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


BEGIN
elementType:=upper(elementType);
if(elementType='UNIT') then  
 RETURN QUERY
 select coalesce(system_id, 0) as system_id,'UNIT'::character varying as room_type ,template_name,coalesce(room_height, 0)::double precision as room_height ,coalesce(room_width, 0)::double precision room_width,
 coalesce(room_length, 0)::double precision room_length ,LD.template_form_url,case when exists(select system_id from isp_template_room a where a.created_by=userid) then true else false end as isTemplateFilled,''::character varying as entity_category  
, 'UNIT'::character varying as entityclass   
 from isp_template_room ITM 
 right join layer_details LD
 on upper(LD.layer_name)='UNIT' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%UnitTemplate' order by coalesce(system_id, 0) desc limit 1;
end if;
if(elementType='HTB') then  
 RETURN QUERY
 select coalesce(system_id, 0) as system_id,'HTB'::character varying as eType,template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url
,case when exists(select system_id from isp_template_htb a where a.created_by=userid) then true else false end as isTemplateFilled,''::character varying as entity_category
, 'AHTB'::character varying as entityclass      
 from isp_template_htb   ITM 
 right join layer_details LD
 on upper(LD.layer_name)='HTB' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%HTBTemplate' order by coalesce(system_id, 0) desc limit 1;
end if;
if(elementType='ADB') then  
 RETURN QUERY  
  select coalesce(id, 0) as id,'ADB'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l,LD.template_form_url   
,case when exists(select id from item_template_ADB a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category 
, 'AADB'::character varying as entityclass        
  from item_template_ADB ITM 
 right join layer_details LD
 on upper(LD.layer_name)='ADB'  and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%ADBTemplate' order by coalesce(id, 0) desc limit 1;
end if;
if(elementType='CDB') then  
 RETURN QUERY  
  select coalesce(id, 0) as id,'CDB'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l,LD.template_form_url   
,case when exists(select id from item_template_CDB a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category 
, 'ACDB'::character varying as entityclass        
  from item_template_CDB ITM 
 right join layer_details LD
 on upper(LD.layer_name)='CDB'  and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%CDBTemplate' order by coalesce(id, 0) desc limit 1;
end if;
   if(elementType='FDB') then  
 RETURN QUERY
 select coalesce(system_id, 0) as system_id,'FDB'::character varying as eType,template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l,LD.template_form_url 
,case when exists(select system_id from isp_template_fdb a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category 
, 'AFDB'::character varying as entityclass      
from isp_template_fdb ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='FDB' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%FDBTemplate' order by coalesce(system_id, 0) desc limit 1;
end if;
 if(elementType='BDB') then  
 RETURN QUERY
 select coalesce(id, 0) as id,'BDB'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url
,case when exists(select id from item_template_bdb a where a.created_by=userid) then true else false end as isTemplateFilled , ITM.entity_category 
,(case when upper(ITM.entity_category)=upper('primary') then 'ABDB' else 'ABDB2' end)::character varying as entityclass      
 from item_template_bdb  ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='BDB' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%BDBTemplate' order by coalesce(id, 0) desc limit 1;
end if;
if(elementType='SPLITTER') then  
 RETURN QUERY
  select coalesce(id, 0) as id,'SPLITTER'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_splitter a where a.created_by=userid) then true else false end as isTemplateFilled ,splitter_type as entity_category        
,(case when upper(ITM.splitter_type)=upper('primary') then 'ASPLITTER' else 'ASPLITTER2' end)::character varying as entityclass
  from item_template_splitter ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='SPLITTER' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%SplitterTemplate' order by coalesce(id, 0) desc limit 1;
end if;
if(elementType='SPLICECLOSURE') then  
 RETURN QUERY  
  select coalesce(id, 0) as id,'SPLICECLOSURE'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l,LD.template_form_url   
,case when exists(select id from item_template_spliceclosure a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category 
, 'ASPLICECLOSURE'::character varying as entityclass        
  from item_template_spliceclosure ITM 
 right join layer_details LD
 on upper(LD.layer_name)='SPLICECLOSURE'  and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%SCTemplate' order by coalesce(id, 0) desc limit 1;
end if;
if(elementType='ONT') then  
 RETURN QUERY

 select coalesce(id, 0) as id,'ONT'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_ont a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
, 'AONT'::character varying as entityclass      
 from item_template_ont ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='ONT' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%ONTTemplate' order by coalesce(id, 0) desc limit 1;
end if;

if(elementType='FMS') then  
 RETURN QUERY

 select coalesce(id, 0) as id,'FMS'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_fms a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
, 'FMS'::character varying as entityclass      
 from item_template_fms ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='FMS' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%FMSTemplate' order by coalesce(id, 0) desc limit 1;
end if;

if(elementType='POD') then  
 RETURN QUERY

 select coalesce(id, 0) as id,'POD'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_fms a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
, 'POD'::character varying as entityclass      
 from item_template_pod ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='POD' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%PODTemplate' order by coalesce(id, 0) desc  limit 1;
end if;

if(elementType='PEP') then  
 RETURN QUERY

 select coalesce(id, 0) as id,'PEP'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_pep a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
, 'PEP'::character varying as entityclass      
 from item_template_pep ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='PEP' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%PEPTemplate' order by coalesce(id, 0) desc  limit 1;
end if;

if(elementType='MPOD') then  
 RETURN QUERY

 select coalesce(id, 0) as id,'MPOD'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_fms a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
, 'MPOD'::character varying as entityclass      
 from item_template_MPOD ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='MPOD' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%MPODTemplate'  order by coalesce(id, 0) desc limit 1;
end if;

if(upper(elementType)='CABLE') then  
 RETURN QUERY
     select coalesce(id, 0) as id,'Cable'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
    ,case when exists(select id from item_template_cable a where a.created_by=userid and cable_type='ISP') then true else false end as isTemplateFilled ,''::character varying as entity_category
    , 'Cable'::character varying as entityclass      
     from item_template_cable ITM 
     right join layer_details LD 
     on upper(LD.layer_name)='CABLE' and created_by=userid
     where is_isp_layer=true and LD.template_form_url like '%CableTemplate' order by coalesce(id, 0) desc limit 1;
end if;

if(upper(elementType)='DUCT') then  
 RETURN QUERY
     select coalesce(id, 0) as id,'Duct'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
    ,case when exists(select id from item_template_duct a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
    , 'Duct'::character varying as entityclass      
     from item_template_duct ITM 
     right join layer_details LD 
     on upper(LD.layer_name)='DUCT' and created_by=userid
     where is_isp_layer=true and LD.template_form_url like '%DuctTemplate' order by coalesce(id, 0) desc limit 1;
end if;

if(elementType='CABINET') then  
 RETURN QUERY

 select coalesce(id, 0) as id,'Cabinet'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_fms a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
, 'Cabinet'::character varying as entityclass      
 from item_template_Cabinet ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='CABINET' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%CabinetTemplate'  order by coalesce(id, 0) desc limit 1;
end if;

if(elementType='VAULT') then  
 RETURN QUERY

 select coalesce(id, 0) as id,'Vault'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_fms a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
, 'Vault'::character varying as entityclass      
 from item_template_vault ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='VAULT' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%VaultTemplate'  order by coalesce(id, 0) desc limit 1;
end if;

if(elementType='OPTICALREPEATER') then  
 RETURN QUERY
 select coalesce(system_id, 0) as system_id,'OpticalRepeater'::character varying as eType,template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url
,case when exists(select system_id from isp_template_opticalrepeater a where a.created_by=userid) then true else false end as isTemplateFilled,''::character varying as entity_category
, 'AOpticalRepeater'::character varying as entityclass      
 from isp_template_opticalrepeater   ITM 
 right join layer_details LD
 on upper(LD.layer_name)='OPTICALREPEATER' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%OpticalRepeaterTemplate' order by coalesce(system_id, 0) desc limit 1;
end if;

if(elementType='PATCHPANEL') then  
 RETURN QUERY

 select coalesce(id, 0) as id,'PATCHPANEL'::character varying as eType,specification as template_name,0::double precision  as h,0::double precision  as w,0::double precision  as l ,LD.template_form_url  
,case when exists(select id from item_template_patchpanel a where a.created_by=userid) then true else false end as isTemplateFilled ,''::character varying as entity_category
, 'PATCHPANEL'::character varying as entityclass      
 from item_template_patchpanel ITM 
 right join layer_details LD 
 on upper(LD.layer_name)='PATCHPANEL' and created_by=userid
 where is_isp_layer=true and LD.template_form_url like '%PatchPanelTemplate' order by coalesce(id, 0) desc limit 1;
end if;

END;

$BODY$;

ALTER FUNCTION public.fn_getelementtemplate(character varying, integer)
    OWNER TO postgres;



------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------15. For getting  the layer Add in function :public.fn_get_network_layers
------------------------------------------------------------------------------------------------------------------------------------

-- FUNCTION: public.fn_get_network_layers(integer, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_network_layers(integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_get_network_layers(
	user_id integer,
	group_id integer,
	p_role_id integer)
    RETURNS TABLE(layerid integer, layername character varying, parentlayerid integer, parentlayername character varying, minzoomlevel integer, maxzoomlevel integer, layerabbr character varying, mapabbr character varying, layertitle character varying, layerformurl character varying, isvisibleinnelibrary boolean, istemplaterequired boolean, isnetworkentity boolean, networkidtype character varying, geomtype character varying, templateformurl character varying, templateid integer, saveentityurl character varying, isdirectsave boolean, isnetworktyperequired boolean, layernetworkgroup character varying, maplayerseq integer, isvisible boolean, is_osp_layer_freezed_in_library boolean, strokewidth double precision, planned_view boolean, asbuild_view boolean, dormant_view boolean, planned_add boolean, asbuild_add boolean, dormant_add boolean, planned_edit boolean, asbuild_edit boolean, dormant_edit boolean, planned_delete boolean, asbuild_delete boolean, dormant_delete boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


DECLARE sql text;
BEGIN

RETURN QUERY SELECT 
	v.layer_id as layerId,
	v.layer_name as layerName,
	v.parent_layer_id as parentLayerId,
	''::character varying as parentLayerName, 
	v.minZoomLevel, v.maxZoomLevel,
	v.layer_abbr as layerAbbr,
	v.map_abbr as mapAbbr,
	v.layer_title as layertitle,
	('Library'||v.layer_form_url)::character varying as layerformurl,
	is_visible_in_ne_library as  isVisibleInNeLibrary,
	is_template_required as isTemplateRequired,
	is_network_entity as isNetworkEntity,
	network_id_type as networkIdType,
	case when t.geom_type is null then v.geom_type else t.geom_type end as geomType,
	template_form_url as templateFormUrl,
    case when t.id is null then 0 else t.id end as
	templateId,
	('Library'||save_entity_url)::character varying as saveEntityUrl,
	is_direct_save as isDirectSave,
	is_networktype_required as isNetworkTypeRequired,
	lgm.group_name as layerNetworkGroup,
	COALESCE(v.map_layer_seq,0) as maplayerseq,
	v.isvisible,
	v.is_osp_layer_freezed_in_library,
	COALESCE(t.strokewidth,0) as strokewidth,
	COALESCE(nl.planned_view,false) as planned_view,COALESCE(nl.asbuild_view,false) as asbuild_view,COALESCE(nl.dormant_view,false) as dormant_view,
	COALESCE(nl.planned_add,true) as planned_add,COALESCE(nl.asbuild_add,true) as asbuild_add,COALESCE(nl.dormant_add,true) as dormant_add,
	COALESCE(nl.planned_edit,true) as planned_edit,COALESCE(nl.asbuild_edit,true) as asbuild_edit,COALESCE(nl.dormant_edit,true) as dormant_edit,
	COALESCE(nl.planned_delete,true) as planned_delete,COALESCE(nl.asbuild_delete,true) as asbuild_delete,COALESCE(nl.dormant_delete,true) as dormant_delete
	
	FROM Layer_Details v left join (
	(select 'ADB' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_adb where created_by=user_id limit 1)
	union
	(select 'BDB' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_bdb where created_by=user_id limit 1)
	union
	(select 'FDB' entity_type, system_id,0::double precision  as strokewidth,null::character varying as geom_type from isp_template_fdb where created_by=user_id limit 1)
	union 
	(select 'CDB' entity_type, id,0::double precision as strokewidth,null::character varying as geom_type from item_template_cdb where created_by=user_id limit 1)
	union
	(select 'Pole' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_pole where created_by=user_id limit 1)
	union
	(select 'POD' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_pod where created_by=user_id limit 1)
	union
	(select 'Tree' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_tree where created_by=user_id limit 1)
	union
	(select 'Splitter' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_splitter where created_by=user_id limit 1)
	union
	(select 'Manhole' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_manhole where created_by=user_id limit 1)
	union
	(select 'Handhole' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_handhole where created_by=user_id limit 1)
	union
(select 'OpticalRepeater' entity_type, system_id,0::double precision as strokewidth,null::character varying as geom_type from isp_template_opticalrepeater where created_by=user_id limit 1)
union
	(select 'SpliceClosure' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_spliceclosure where created_by=user_id limit 1)
	union
	(select 'MPOD' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_mpod where created_by=user_id limit 1)
	union
	(select 'Cable' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_cable where created_by=user_id and cable_type!='ISP' limit 1)
	union
	(select 'Trench' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_trench where created_by=user_id limit 1)
	union
	(select 'Duct' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_duct where created_by=user_id limit 1)
	union
	(select 'Gipipe' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_gipipe where created_by=user_id limit 1)
	union
	(select 'ONT' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_ont where created_by=user_id limit 1)
	union
	(select 'Coupler' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_coupler where created_by=user_id limit 1)	
	union
	(select 'WallMount' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_wallmount where created_by=user_id limit 1)
		union
	(select 'PEP' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_pep where created_by=user_id limit 1)
	union
	(select 'HTB' entity_type, system_id as id,0::double precision  as strokewidth,null::character varying as geom_type from isp_template_htb where created_by=user_id limit 1)
	union
     (select 'Cabinet' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_cabinet where created_by=user_id limit 1)
     union
     (select 'Vault' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_vault where created_by=user_id limit 1)
     union
     (select 'Conduit' entity_type, id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_conduit where created_by=user_id limit 1)
     union
     (select 'Tower' entity_type, id,0::double precision  as strokewidth,null::character varying as
	  geom_type from item_template_tower where created_by=user_id limit 1)
    union
		(select 'MicroDuct' entity_type, id,0::double precision  as strokewidth,null::character varying 
		 as geom_type from item_template_microduct where created_by=user_id limit 1)
     union
(select 'Antenna' entity_type, id,0::double precision  as strokewidth,null::character varying 
		 as geom_type from item_template_Antenna where created_by=user_id limit 1)
     union
		(select 'FMS' entity_type, id,0::double precision  as strokewidth,null::character varying 
		 as geom_type from item_template_FMS where created_by=user_id limit 1)
     union
	(select 'PatchCord' entity_type, id as id,0::double precision  as strokewidth,null::character varying as geom_type from item_template_patchcord where created_by=user_id limit 1)
	union
	(select tbl.entity_type, tbl.id, tbl.strokewidth, tbl.geom_type from (
	select 'ROW' entity_type, id as id,width as strokewidth,type as geom_type,created_by from item_template_row where created_by=0
	union
	select 'ROW' entity_type, id as id,width as strokewidth,type as geom_type,created_by  from item_template_row where created_by=user_id) tbl order by created_by desc  limit 1)
	) t on upper(v.layer_name)=upper(t.entity_type) 
	left join vw_network_layers nl on v.layer_id = nl.layer_id and nl.role_id = p_role_id
	left join vw_layer_group_mapping lgm on v.layer_id = lgm.layer_id
	where (v.isvisible=true or is_visible_in_ne_library=true) and v.is_osp_layer=true 
	and (	COALESCE(nl.planned_view,false) = true or COALESCE(nl.asbuild_view,false)  = true or COALESCE(nl.dormant_view,false)  = true )
	order by lgm.layer_seq;
END

$BODY$;

ALTER FUNCTION public.fn_get_network_layers(integer, integer, integer)
    OWNER TO postgres;


---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------17. Trigger function for deleting the entry from point table
---------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_trg_delete_pep_geom()
  RETURNS trigger AS
$BODY$
BEGIN

IF (TG_OP = 'DELETE' ) THEN
  Delete from point_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='PEP';
END IF;		

RETURN NEW;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;

---------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------18. Creating trigger for inserting,updating and deleting the template data in table audit_item_template_pep
---------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_trg_audit_item_template_pep()
  RETURNS trigger AS
$BODY$
BEGIN

IF (TG_OP = 'INSERT' ) THEN  
       INSERT INTO public.audit_item_template_pep(
            id, specification, category, subcategory1, subcategory2, 
            subcategory3, item_code, vendor_id, type, brand, model, construction, 
            activation, accessibility, created_by, created_on, modified_by, 
            modified_on, action,audit_item_master_id)
         select  new.id, new.specification, new.category, new.subcategory1, new.subcategory2, 
            new.subcategory3, new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, 
            new.activation, new.accessibility, new.created_by, new.created_on, new.modified_by, 
            new.modified_on, 'I' as action,new.audit_item_master_id from item_template_pep where id=new.id;

END IF;
IF (TG_OP = 'UPDATE' ) THEN  

            INSERT INTO public.audit_item_template_pep(
            id, specification, category, subcategory1, subcategory2, 
            subcategory3, item_code, vendor_id, type, brand, model, construction, 
            activation, accessibility, created_by, created_on, modified_by, 
            modified_on, action,audit_item_master_id)
         select  new.id, new.specification, new.category, new.subcategory1, new.subcategory2, 
            new.subcategory3, new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, 
            new.activation, new.accessibility, new.created_by, new.created_on, new.modified_by, 
            new.modified_on, 'U' as action,new.audit_item_master_id from item_template_pep where id=new.id;

END IF; 				
IF (TG_OP = 'DELETE' ) THEN  

       INSERT INTO public.audit_item_template_pep(
            id, specification, category, subcategory1, subcategory2, 
            subcategory3, item_code, vendor_id, type, brand, model, construction, 
            activation, accessibility, created_by, created_on, modified_by, 
            modified_on, action,audit_item_master_id)
         values( old.id, old.specification, old.category, old.subcategory1, old.subcategory2, 
            old.subcategory3, old.item_code, old.vendor_id, old.type, old.brand, old.model, old.construction, 
            old.activation, old.accessibility, old.created_by, old.created_on, old.modified_by, 
            old.modified_on,'D',old.audit_item_master_id);    
         
         

END IF;		

RETURN NEW;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;

---------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------19. Creating a trigger for inserting,updating and deleting  data in table audit_att_details_pep
---------------------------------------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_trg_audit_att_details_pep()
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
       INSERT INTO public.audit_att_details_pep(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,
            status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,remarks,is_acquire_from,
            other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         select  new.system_id, new.network_id, new.pep_name, new.latitude, new.longitude, 
            new.province_id, new.region_id, new.address, new.pep_no, new.pep_height, 
            new.specification, new.category, new.subcategory1, new.subcategory2, new.subcategory3, 
            new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, new.activation, 
            new.accessibility, new.status, new.created_by, new.created_on, new.modified_by, new.modified_on, 
            new.network_status, new.parent_system_id, new.parent_network_id, new.parent_entity_type, 
            new.sequence_id, new.project_id, new.planning_id, new.purpose_id, new.workorder_id, 'I' as action,new.acquire_from,new.ownership_type,
            new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,
            new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.remarks,
            new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,
            new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category,new.gis_design_id
             from att_details_pep where system_id=new.system_id;

	
END IF;
IF (TG_OP = 'UPDATE' ) THEN  
_ignorecol := 'modified_on,gis_design_id,codification_sequence,area_system_id,area_id,subarea_system_id,subarea_id,dsa_system_id,dsa_id,csa_system_id,csa_id';
_ignorecol1 := 'modified_by';
select fn_check_history_record(OLD, NEW,_ignorecol,_ignorecol1) into _value;
if(_value = 1)then

           INSERT INTO public.audit_att_details_wallmount(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,
            audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,
            remarks,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,
            request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         select  new.system_id, new.network_id, new.pep_name, new.latitude, new.longitude, 
            new.province_id, new.region_id, new.address, new.pep_no, new.pep_height, 
            new.specification, new.category, new.subcategory1, new.subcategory2, new.subcategory3, 
            new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, new.activation, 
            new.accessibility, new.status, new.created_by, new.created_on, new.modified_by, new.modified_on, 
            new.network_status, new.parent_system_id, new.parent_network_id, new.parent_entity_type, 
            new.sequence_id, new.project_id, new.planning_id, new.purpose_id, new.workorder_id, 'U' as action,new.acquire_from,
            new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,
            new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.remarks,new.is_acquire_from,
            new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,
            new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category,new.gis_design_id
             from att_details_pep where system_id=new.system_id;

END IF; 

END IF; 								
IF (TG_OP = 'DELETE' ) THEN  

          INSERT INTO public.audit_att_details_pep(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,
            status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,remarks,is_acquire_from,
            other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         values( old.system_id, old.network_id, old.pep_name, old.latitude, old.longitude, 
            old.province_id, old.region_id, old.address, old.pep_no, old.pep_height, 
            old.specification, old.category, old.subcategory1, old.subcategory2, old.subcategory3, 
            old.item_code, old.vendor_id, old.type, old.brand, old.model, old.construction, old.activation, 
            old.accessibility, old.status, old.created_by, old.created_on, old.modified_by, old.modified_on, 
            old.network_status, old.parent_system_id, old.parent_network_id, old.parent_entity_type, 
            old.sequence_id, old.project_id, old.planning_id, old.purpose_id, old.workorder_id, 'D',old.acquire_from,old.ownership_type,old.third_party_vendor_id,
            old.source_ref_type,old.source_ref_id,old.source_ref_description,old.audit_item_master_id,old.status_remark,old.status_updated_by,
            old.status_updated_on,old.is_visible_on_map,old.primary_pod_system_id,old.secondary_pod_system_id,old.remarks,old.is_acquire_from,
            old.other_info,old.origin_from,old.origin_ref_id,old.origin_ref_code,old.origin_ref_description,old.request_ref_id,old.requested_by,
            old.request_approved_by,old.subarea_id,old.area_id,old.dsa_id,old.csa_id,old.bom_sub_category,old.gis_design_id);
          
         
          Delete from point_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='PEP';

END IF; 		

RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.fn_trg_audit_att_details_pep()
    OWNER TO postgres;


---------------------------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------20. Creating a trigger  for history show in table att_details_pep
---------------------------------------------------------------------------------------------------------------------------------------------

CREATE TRIGGER trg_trg_entity_status_history
  AFTER INSERT OR UPDATE OR DELETE
  ON public.att_details_pep
  FOR EACH ROW
  EXECUTE PROCEDURE public.fn_trg_entity_status_history();

---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------21. Creating a trigger for altering on table att_details_pep 
---------------------------------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_trg_audit_att_details_pep()
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
       INSERT INTO public.audit_att_details_pep(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,
            status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,remarks,is_acquire_from,
            other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         select  new.system_id, new.network_id, new.pep_name, new.latitude, new.longitude, 
            new.province_id, new.region_id, new.address, new.pep_no, new.pep_height, 
            new.specification, new.category, new.subcategory1, new.subcategory2, new.subcategory3, 
            new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, new.activation, 
            new.accessibility, new.status, new.created_by, new.created_on, new.modified_by, new.modified_on, 
            new.network_status, new.parent_system_id, new.parent_network_id, new.parent_entity_type, 
            new.sequence_id, new.project_id, new.planning_id, new.purpose_id, new.workorder_id, 'I' as action,new.acquire_from,new.ownership_type,
            new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,
            new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.remarks,
            new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,
            new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category,new.gis_design_id
             from att_details_pep where system_id=new.system_id;

	 
END IF;
IF (TG_OP = 'UPDATE' ) THEN  
_ignorecol := 'modified_on,gis_design_id,codification_sequence,area_system_id,area_id,subarea_system_id,subarea_id,dsa_system_id,dsa_id,csa_system_id,csa_id';
_ignorecol1 := 'modified_by';
select fn_check_history_record(OLD, NEW,_ignorecol,_ignorecol1) into _value;
if(_value = 1)then

           INSERT INTO public.audit_att_details_pep(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,
            audit_item_master_id,status_remark,status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,
            remarks,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,
            request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         select  new.system_id, new.network_id, new.pep_name, new.latitude, new.longitude, 
            new.province_id, new.region_id, new.address, new.pep_no, new.pep_height, 
            new.specification, new.category, new.subcategory1, new.subcategory2, new.subcategory3, 
            new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, new.activation, 
            new.accessibility, new.status, new.created_by, new.created_on, new.modified_by, new.modified_on, 
            new.network_status, new.parent_system_id, new.parent_network_id, new.parent_entity_type, 
            new.sequence_id, new.project_id, new.planning_id, new.purpose_id, new.workorder_id, 'U' as action,new.acquire_from,
            new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,
            new.status_remark,new.status_updated_by,new.status_updated_on,new.is_visible_on_map,new.primary_pod_system_id,new.secondary_pod_system_id,new.remarks,new.is_acquire_from,
            new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,
            new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category,new.gis_design_id
             from att_details_pep where system_id=new.system_id;

END IF; 
	
END IF; 								
IF (TG_OP = 'DELETE' ) THEN  

          INSERT INTO public.audit_att_details_pep(
            system_id, network_id, pep_name, latitude, longitude, 
            province_id, region_id, address, pep_no, pep_height, 
            specification, category, subcategory1, subcategory2, subcategory3, 
            item_code, vendor_id, type, brand, model, construction, activation, 
            accessibility, status, created_by, created_on, modified_by, modified_on, 
            network_status, parent_system_id, parent_network_id, parent_entity_type, 
            sequence_id, project_id, planning_id, purpose_id, workorder_id, 
            action,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,
            status_updated_by,status_updated_on,is_visible_on_map,primary_pod_system_id,secondary_pod_system_id,remarks,is_acquire_from,
            other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category,gis_design_id)
         values( old.system_id, old.network_id, old.pep_name, old.latitude, old.longitude, 
            old.province_id, old.region_id, old.address, old.pep_no, old.pep_height, 
            old.specification, old.category, old.subcategory1, old.subcategory2, old.subcategory3, 
            old.item_code, old.vendor_id, old.type, old.brand, old.model, old.construction, old.activation, 
            old.accessibility, old.status, old.created_by, old.created_on, old.modified_by, old.modified_on, 
            old.network_status, old.parent_system_id, old.parent_network_id, old.parent_entity_type, 
            old.sequence_id, old.project_id, old.planning_id, old.purpose_id, old.workorder_id, 'D',old.acquire_from,old.ownership_type,old.third_party_vendor_id,
            old.source_ref_type,old.source_ref_id,old.source_ref_description,old.audit_item_master_id,old.status_remark,old.status_updated_by,
            old.status_updated_on,old.is_visible_on_map,old.primary_pod_system_id,old.secondary_pod_system_id,old.remarks,old.is_acquire_from,
            old.other_info,old.origin_from,old.origin_ref_id,old.origin_ref_code,old.origin_ref_description,old.request_ref_id,old.requested_by,
            old.request_approved_by,old.subarea_id,old.area_id,old.dsa_id,old.csa_id,old.bom_sub_category,old.gis_design_id);
          
         
          Delete from point_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='PEP';

END IF; 		

RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.fn_trg_audit_att_details_pep()
    OWNER TO postgres;
-------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TRIGGER trg_audit_att_details_pep
  AFTER INSERT OR UPDATE OR DELETE
  ON public.att_details_pep
  FOR EACH ROW
  EXECUTE PROCEDURE public.fn_trg_audit_att_details_pep();

---------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------22. Creating  a trigger for geom entry 
---------------------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_trg_delete_pep_geom()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN

IF (TG_OP = 'DELETE' ) THEN
  Delete from point_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='PEP';
END IF;		

RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.fn_trg_delete_pep_geom()
    OWNER TO postgres;
--------------------------------------------------------------------------------------------------------------------------------------------
CREATE TRIGGER fn_trg_delete_pep_geom
  AFTER INSERT OR UPDATE OR DELETE
  ON public.att_details_pep
  FOR EACH ROW
  EXECUTE PROCEDURE public.fn_trg_delete_pep_geom();



---------------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------------------23. Creating a trigger for template data 
---------------------------------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_trg_audit_item_template_pep()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
BEGIN

IF (TG_OP = 'INSERT' ) THEN  
       INSERT INTO public.audit_item_template_pep(
            id, specification, category, subcategory1, subcategory2, 
            subcategory3, item_code, vendor_id, type, brand, model, construction, 
            activation, accessibility, created_by, created_on, modified_by, 
            modified_on, action,audit_item_master_id)
         select  new.id, new.specification, new.category, new.subcategory1, new.subcategory2, 
            new.subcategory3, new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, 
            new.activation, new.accessibility, new.created_by, new.created_on, new.modified_by, 
            new.modified_on, 'I' as action,new.audit_item_master_id from item_template_pep where id=new.id;

END IF;
IF (TG_OP = 'UPDATE' ) THEN  

            INSERT INTO public.audit_item_template_pep(
            id, specification, category, subcategory1, subcategory2, 
            subcategory3, item_code, vendor_id, type, brand, model, construction, 
            activation, accessibility, created_by, created_on, modified_by, 
            modified_on, action,audit_item_master_id)
         select  new.id, new.specification, new.category, new.subcategory1, new.subcategory2, 
            new.subcategory3, new.item_code, new.vendor_id, new.type, new.brand, new.model, new.construction, 
            new.activation, new.accessibility, new.created_by, new.created_on, new.modified_by, 
            new.modified_on, 'U' as action,new.audit_item_master_id from item_template_pep where id=new.id;

END IF; 				
IF (TG_OP = 'DELETE' ) THEN  

       INSERT INTO public.audit_item_template_pep(
            id, specification, category, subcategory1, subcategory2, 
            subcategory3, item_code, vendor_id, type, brand, model, construction, 
            activation, accessibility, created_by, created_on, modified_by, 
            modified_on, action,audit_item_master_id)
         values( old.id, old.specification, old.category, old.subcategory1, old.subcategory2, 
            old.subcategory3, old.item_code, old.vendor_id, old.type, old.brand, old.model, old.construction, 
            old.activation, old.accessibility, old.created_by, old.created_on, old.modified_by, 
            old.modified_on,'D',old.audit_item_master_id);    
         
         

END IF;		

RETURN NEW;
END;
$BODY$;

ALTER FUNCTION public.fn_trg_audit_item_template_pep()
    OWNER TO postgres;
-------------------------------------------------------------------------------------------------------------------------------------------


CREATE TRIGGER trg_audit_item_template_pep
  AFTER INSERT OR UPDATE OR DELETE
  ON public.item_template_pep
  FOR EACH ROW
  EXECUTE PROCEDURE public.fn_trg_audit_item_template_pep();

---------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------24. insert brand details in isp_brand_master table w.r.t PEP
---------------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.isp_brand_master
(brand, type_id, created_by, created_on, modified_by, modified_on, is_active, layer_id)
VALUES('brand1', 1, 5, now(), 5, now(), true, (select layer_id from layer_details where layer_name='PEP'));


---------------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------25. insert model details in isp_base_model table w.r.t PEP
---------------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.isp_base_model
(model, brand_id, inputport, outputport, rate_code, power_supply, device_length, device_width, device_height, created_by, created_on, modified_by, modified_on, is_active, layer_id, type_id)
VALUES('Model1', (select id from isp_brand_master where layer_id=(select layer_id from layer_details where layer_name='PEP')), 0, 0, NULL, 0, 0.0, 0.0, 0.0, 5, now(), 5, now(), true, (select layer_id from layer_details where layer_name='PEP'), 1);

--------------------------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------26. insert form input setting in form_input_settings table w.r.t PEP
--------------------------------------------------------------------------------------------------------------------------------------------


INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('PEP', 'type', 'field', true, true, 1, now(), 1, now(), NULL, false);

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('PEP', 'brand', 'field', true, true, 1, now(), 1, now(), NULL, false);

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('PEP', 'model', 'field', true, true, 1, now(), 1, now(), NULL, false);

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('PEP', 'construction', 'field', true, true, 1, now(), 1, now(), NULL, false);

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('PEP', 'activation', 'field', true, true, 1, now(), 1, now(), NULL, false);

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('PEP', 'accessibility', 'field', true, true, 1, now(), 1, now(), NULL, false);

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('PEP', 'hierarchy_type', 'field', true, false, 1, now(), NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('WallMount', 'latitude', 'field', true, true, NULL, now(), NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
(form_name, form_feature_name, form_feature_type, is_active, is_required, created_by, created_on, modified_by, modified_on, feature_description, is_readonly)
VALUES('PEP', 'longitude', 'field', true, true, NULL, now(), NULL, NULL, NULL, false);

--------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------27. insert form input setting in res_dropdown_master table w.r.t PEP for the submodule of resouce key
--------------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.res_dropdown_master
(dropdown_type, dropdown_value, dropdown_key, dropdown_status, is_used_for_mobile, is_used_for_web, created_by, created_on, modified_by, modified_on)
VALUES( 'Res_Sub_Module', 'PEP', 'PEP', true, false, true, 1, now(), NULL, now());

--------------------------------------------------------------------------------------------------------------------------------------------
-----------------------------------------------------28. insert form input setting in res_resources table w.r.t PEP for newly generated keys
--------------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('de-DE', 'SI_OSP_PEP_NET_FRM_001', 'PEP template saved successfully.', false, true, 'German', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_PEP_NET_FRM_001', 'PEP template saved successfully.', true, true, 'English', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en-US', 'SI_OSP_PEP_NET_FRM_001', 'PEP template saved successfully.', false, true, NULL, 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('fr', 'SI_OSP_PEP_NET_FRM_001', 'PEP template saved successfully.', false, true, 'French', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_PEP_NET_FRM_001', 'PEP template saved successfully.', false, true, 'Hindi', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ja-JP', 'SI_OSP_PEP_NET_FRM_001', 'PEP template saved successfully.', false, true, 'Japanese', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ru-RU', 'SI_OSP_PEP_NET_FRM_001', 'PEP template saved successfully.', false, true, 'Russian', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('de-DE', 'SI_OSP_PEP_NET_FRM_002', 'PEP template update successfully.', false, true, 'German', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_PEP_NET_FRM_002', 'PEP template update successfully.', true, true, 'English', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en-US', 'SI_OSP_PEP_NET_FRM_002', 'PEP template update successfully.', false, true, NULL, 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('fr', 'SI_OSP_PEP_NET_FRM_002', 'PEP template update successfully.', false, true, 'French', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_PEP_NET_FRM_002', 'PEP template update successfully.', false, true, 'Hindi', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ja-JP', 'SI_OSP_PEP_NET_FRM_002', 'PEP template update successfully.', false, true, 'Japanese', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ru-RU', 'SI_OSP_PEP_NET_FRM_002', 'PEP template update successfully.', false, true, 'Russian', 'Smart Inventory_Osp_PEP_Dot Net_', NULL, NULL, now(), 5, false, false);

--------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------29. sync the column name inside the 'LAYER_COLUMNS_SETTINGS' table  for 'Report' ,'Info','History'
--------------------------------------------------------------------------------------------------------------------------------------------
select * from  fn_sync_layer_columns()

--------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------30. insert display layer in information popup window using 'display_name_settings' table
--------------------------------------------------------------------------------------------------------------------------------------------
INSERT INTO public.display_name_settings
(layer_id, display_column_name, created_by, created_on, modified_by, modified_on, status, display_network_name, default_display_column_name)
VALUES((select layer_id from layer_details where layer_name='PEP'), 'network_id', 1, now(), NULL, NULL, 'Completed', 'pep_name', 'network_id');

--------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------31. insert data for map icons 'layer_icon_master' table
--------------------------------------------------------------------------------------------------------------------------------------------


INSERT INTO public.layer_icon_master
(layer_id, category, subcategory, icon_name, icon_path, network_status, status, created_by, created_on, modified_by, modified_on, is_virtual, landbase_layer_id)
VALUES((select layer_id from layer_details where layer_name='PEP'), NULL, NULL, 'PEP.png', 'icons/Planned/PEP.png', 'P', true, 1, now(), NULL, NULL, false, NULL);

INSERT INTO public.layer_icon_master
(layer_id, category, subcategory, icon_name, icon_path, network_status, status, created_by, created_on, modified_by, modified_on, is_virtual, landbase_layer_id)
VALUES((select layer_id from layer_details where layer_name='PEP'), NULL, NULL, 'PEP.png', 'icons/AsBuild/PEP.png', 'A', true, 1, now(), NULL, NULL, false, NULL);

INSERT INTO public.layer_icon_master
(layer_id, category, subcategory, icon_name, icon_path, network_status, status, created_by, created_on, modified_by, modified_on, is_virtual, landbase_layer_id)
VALUES((select layer_id from layer_details where layer_name='PEP'), NULL, NULL, 'PEP.png', 'icons/Dorment/PEP.png', 'D', true, 1, now(), NULL, NULL, false, NULL);