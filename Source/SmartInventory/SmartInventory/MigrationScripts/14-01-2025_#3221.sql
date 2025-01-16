
----------------------------------------------------------------------To a column inside the user_master and audit_user_master tables-----------------------------
---------------------------------------------------------------------------------------------------------------------------------

alter table user_master add column vendorcost double precision

alter table audit_user_master add column vendorCost double precision

alter table att_details_pod add column vendorCost double precision

alter table att_details_pod add column contracktorId int

alter table audit_att_details_pod add column contracktorId int
alter table audit_att_details_pod add column vendorCost double precision

------------------------------------------------------ Insert script for res keys--------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('de-DE', 'SI_OSP_GBL_GBL_RPT_002', 'Award a site to a vendor', false, true, 'German', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-01-10 18:19:02.930', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_GBL_GBL_RPT_002', 'Award a site to a vendor', true, true, 'English', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-01-10 18:19:02.930', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en-US', 'SI_OSP_GBL_GBL_RPT_002', 'Award a site to a vendor', false, true, NULL, 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-01-10 18:19:02.930', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('fr', 'SI_OSP_GBL_GBL_RPT_002', 'Award a site to a vendor', false, true, 'French', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-01-10 18:19:02.930', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_GBL_GBL_RPT_002', 'Award a site to a vendor', false, true, 'Hindi', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-01-10 18:19:02.930', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ja-JP', 'SI_OSP_GBL_GBL_RPT_002', 'Award a site to a vendor', false, true, 'Japanese', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-01-10 18:19:02.930', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ru-RU', 'SI_OSP_GBL_GBL_RPT_002', 'Award a site to a vendor', false, true, 'Russian', 'Smart Inventory_Osp_Global_Global_', NULL, NULL, '2025-01-10 18:19:02.930', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('de-DE', 'SI_OSP_GBL_NET_RPT_420', 'Vendor List', false, true, 'German', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2025-01-14 12:08:21.421', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_GBL_NET_RPT_420', 'Vendor List', true, true, 'English', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2025-01-14 12:08:21.421', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en-US', 'SI_OSP_GBL_NET_RPT_420', 'Vendor List', false, true, NULL, 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2025-01-14 12:08:21.421', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('fr', 'SI_OSP_GBL_NET_RPT_420', 'Vendor List', false, true, 'French', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2025-01-14 12:08:21.421', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_GBL_NET_RPT_420', 'Vendor List', false, true, 'Hindi', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2025-01-14 12:08:21.421', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ja-JP', 'SI_OSP_GBL_NET_RPT_420', 'Vendor List', false, true, 'Japanese', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2025-01-14 12:08:21.421', 5, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ru-RU', 'SI_OSP_GBL_NET_RPT_420', 'Vendor List', false, true, 'Russian', 'Smart Inventory_Osp_Global_Dot Net_', NULL, NULL, '2025-01-14 12:08:21.421', 5, true, false);
----------------------------------Update query for user_master table-------------------------------------------------
---------------------------------------------------------------------------------------------------------------------

update dropdown_master set dropdown_value='Vendor', dropdown_key='Vendor' where UPPER(dropdown_type)= upper('UserType') and dropdown_key='Partner' and is_active=true

-----------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------

-- FUNCTION: public.fn_trg_audit_user_master()

-- DROP FUNCTION IF EXISTS public.fn_trg_audit_user_master();

CREATE OR REPLACE FUNCTION public.fn_trg_audit_user_master()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
DECLARE
 _value integer;
 _ignorecol character varying;  
_ignorecol1 character varying; 
BEGIN

IF (TG_OP = 'INSERT' ) THEN  
       INSERT INTO public.audit_user_master(
            user_id,user_name,password,name,user_email,mobile_number,application_access ,is_admin_rights_enabled,manager_id,role_id,module_id,user_img,
    		template_id,group_id,is_active,is_deleted,remarks,created_on,created_by,modified_on,modified_by,action,user_type,pan,prms_id,vendor_id,jc_id,vendorCost)
         	select new.user_id,new.user_name,new.password,new.name,new.user_email,new.mobile_number,new.application_access ,new.is_admin_rights_enabled,new.manager_id,
            new.role_id,new.module_id,new.user_img,new.template_id,new.group_id,new.is_active,
            new.is_deleted,new.remarks,new.created_on,new.created_by,new.modified_on,new.modified_by, 'I' as action,new.user_type,new.pan,new.prms_id,new.vendor_id,new.jc_id,new.vendorCost from user_master where user_id=new.user_id;

END IF;
IF (TG_OP = 'UPDATE' ) THEN  
_ignorecol := 'modified_on';
_ignorecol1 := 'modified_by';
select fn_check_history_record(OLD, NEW,_ignorecol,_ignorecol1) into _value;
if(_value = 1)then  
  INSERT INTO public.audit_user_master(
            user_id,user_name,password,name,user_email,mobile_number,application_access ,is_admin_rights_enabled,manager_id,role_id,module_id,user_img,
    		template_id,group_id,is_active,is_deleted,remarks,created_on,created_by,modified_on,modified_by,action,user_type,pan,prms_id,vendor_id,jc_id,vendorCost)
         	select new.user_id,new.user_name,new.password,new.name,new.user_email,
         	new.mobile_number,new.application_access ,new.is_admin_rights_enabled,
         	new.manager_id,new.role_id,new.module_id,new.user_img,new.template_id,new.group_id,new.is_active,
            new.is_deleted,new.remarks,new.created_on,new.created_by,
            new.modified_on,new.modified_by, 'U' as action,new.user_type,new.pan,new.prms_id,new.vendor_id,new.jc_id,new.vendorCost from user_master where user_id=new.user_id;

END IF; 		
END IF; 		
IF (TG_OP = 'DELETE' ) THEN  

     INSERT INTO public.audit_user_master(
            user_id,user_name,password,name,user_email,mobile_number,application_access ,is_admin_rights_enabled,manager_id,role_id,module_id,user_img,
    		template_id,group_id,is_active,is_deleted,remarks,created_on,created_by,modified_on,modified_by,action,user_type,pan,prms_id,vendor_id,jc_id,vendorCost) values(
         	old.user_id,old.user_name,old.password,old.name,old.user_email,
         	old.mobile_number,old.application_access ,old.is_admin_rights_enabled,
         	old.manager_id,old.role_id,old.module_id,old.user_img,
    		old.template_id,old.group_id,old.is_active,old.is_deleted,old.remarks,old.created_on,old.created_by,old.modified_on,old.modified_by, 'D',old.user_type,old.pan,old.prms_id,old.vendor_id,old.jc_id,old.vendorCost);    
         
       

END IF; 		

RETURN NEW;
END;

$BODY$;

ALTER FUNCTION public.fn_trg_audit_user_master()
    OWNER TO postgres;


----------------------------------------------------------------Modify the View as vw_att_user_details------------------------

---------------------------------------------------------------------------------------------------------------------------

-- View: public.vw_att_user_details

-- DROP VIEW public.vw_att_user_details;

CREATE OR REPLACE VIEW public.vw_att_user_details
 AS
 SELECT umaster.user_id,
    umaster.user_name,
    umaster.password,
    umaster.name,
    umaster.user_email,
    umaster.user_img,
    umaster.role_id,
    rm.role_name,
    umaster.is_active,
    umaster.is_deleted,
    umaster.mobile_number,
    umaster.created_by,
    umaster.created_on,
    umaster.modified_by,
    umaster.modified_on,
    um.user_name AS created_by_text,
    um.user_name AS modified_by_text,
    umaster.application_access,
    umaster.is_admin_rights_enabled,
    umaster.is_all_provience_assigned,
    umaster.manager_id,
    COALESCE(multimanager.fn_get_multimanager, ''::text)::character varying(200) AS reporting_manager,
    umaster.user_type,
    umaster.prms_id,
    umaster.vendorCost
   FROM user_master umaster
     LEFT JOIN user_master um ON umaster.created_by = um.user_id
     LEFT JOIN user_master um1 ON um1.user_id = umaster.modified_by
     LEFT JOIN user_master um2 ON um2.user_id = umaster.manager_id
     LEFT JOIN role_master rm ON rm.role_id = umaster.role_id
     LEFT JOIN LATERAL ( SELECT fn_get_multimanager.fn_get_multimanager
           FROM fn_get_multimanager(umaster.user_id) fn_get_multimanager(fn_get_multimanager)) multimanager ON true;

ALTER TABLE public.vw_att_user_details
    OWNER TO postgres;

GRANT ALL ON TABLE public.vw_att_user_details TO postgres;
GRANT INSERT, SELECT ON TABLE public.vw_att_user_details TO safaricom;



----------------------------------------------------------Create new procedure fn_get_vendor_details----------------------------------
-----------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_vendor_details(character varying, character varying, boolean, integer, integer, character varying, character varying, character varying, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_vendor_details(character varying, character varying, boolean, integer, integer, character varying, character varying, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_get_vendor_details(
	p_searchby character varying,
	p_searchtext character varying,
	p_isactive boolean,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_application_access character varying,
	p_role_id integer,
	p_user_id integer)
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

BEGIN
	if (p_searchby !~* '^[a-zA-Z0-9\s_]*$' or p_searchtext !~* '^[a-zA-Z0-9\s_.\-]*$' or p_sortcolname !~* '^[a-zA-Z0-9\s_]*$' or
		p_sorttype !~* '^[a-zA-Z0-9\s_]*$' or p_application_access !~* '^[a-zA-Z0-9\s_]*$') then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
	begin
LowerStart:='';
LowerEnd:='';
/*IF (pg_typeof(''||P_SORTCOLNAME||'')= character varying) THEN
	LowerStart:='';
	LowerEnd:='';
ELSE*/

 IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
   	IF EXISTS (select 1 from information_schema.columns where upper(table_name) = upper('vw_att_user_details') and 
	upper(column_name) = upper(P_SORTCOLNAME) and upper(data_type) in('CHARACTER VARYING','TEXT')) THEN
		LowerStart:='LOWER(';
	else
		LowerStart:='(';
	end if;
	LowerEnd:=')';	
END IF;
-- END IF;
RAISE INFO '%', sql;

 if(p_role_id != 1 and p_role_id != 0)then
create temp table user_details_temp as
WITH RECURSIVE users AS (
	select user_id, user_name,
 password,name,user_email,role_id,role_name,mobile_number,application_access,is_active,created_by, created_on, modified_by, modified_on, created_by_text, modified_by_text,manager_id,reporting_manager,user_type,vendorCost
 FROM vw_att_user_details WHERE  role_id!=1 AND is_active=true and user_type='Vendor'
	--UNION
	--SELECT ud.user_id, ud.user_name,
	-- ud.password,ud.name,ud.user_email,ud.role_id,ud.role_name,ud.mobile_number,ud.application_access,ud.is_active,ud.created_by,ud.created_on,ud.modified_by, ud.modified_on, 
	-- ud.created_by_text, ud.modified_by_text,ud.manager_id,ud.reporting_manager,ud.user_type
	-- FROM vw_att_user_details ud
	--INNER JOIN users s ON s.user_id = ud.manager_id
) SELECT * FROM users;
else
create temp table user_details_temp as
select ud.user_id, ud.user_name,
	 ud.password,ud.name,ud.user_email,ud.role_id,ud.role_name,ud.mobile_number,ud.application_access,ud.is_active,ud.created_by,
	 ud.created_on,ud.modified_by, ud.modified_on, 
	 ud.created_by_text, ud.modified_by_text,ud.manager_id,ud.reporting_manager,ud.user_type,ud.vendorCost from vw_att_user_details ud;
end if;
-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| LowerStart ||   CASE WHEN (P_SORTCOLNAME) = '' THEN 'user_id' ELSE P_SORTCOLNAME END ||LowerEnd || ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No,user_id, user_name,
 password,name,user_email,role_id,role_name,mobile_number,application_access,is_active,created_by, created_on, modified_by, modified_on, created_by_text, modified_by_text,manager_id,reporting_manager,user_type,vendorCost FROM user_details_temp WHERE  role_id!=1 AND is_active ='||p_isactive||' and user_type=''Vendor''';

IF ((p_searchtext) != '' and (p_searchby) != '') THEN
--sql:= sql ||' AND lower('||p_searchby||') LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
sql:= sql ||' AND lower('|| quote_ident($1) ||') LIKE lower(''%'|| $2 ||'%'')';
END IF;

IF ((p_application_access) != '') THEN
--sql:= sql ||' AND application_access in ('''||p_application_access ||''',''BOTH'') ';
sql := sql || ' AND UPPER(application_access) IN (UPPER(''' || $8 || '''))';
--sql:= sql ||' AND application_access in ('''||$8 ||''',''BOTH'') ';
END IF;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' order by created_on desc) T ';                  
 END IF; 

RAISE INFO '%', sql;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

drop table user_details_temp;
end;
end if;
END ;

$BODY$;

ALTER FUNCTION public.fn_get_vendor_details(character varying, character varying, boolean, integer, integer, character varying, character varying, character varying, integer, integer)
    OWNER TO postgres;



-------------------------------------------------------------------Create new function-------------------------------------------------

----------------------------------------------------------------------------------------------------------------------------------------

-- FUNCTION: public.fn_update_selected_vendor_details(integer, integer, double precision)

-- DROP FUNCTION IF EXISTS public.fn_update_selected_vendor_details(integer, integer, double precision);

CREATE OR REPLACE FUNCTION public.fn_update_selected_vendor_details(
	reference_id integer,
	user_id integer,
	vendorcost double precision)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

Declare sql TEXT;

BEGIN
	
	If(reference_id >0 and  user_id >0 and vendorcost >=0 )THEN
	update att_details_pod set contracktorId=user_id,vendorCost=vendorCost where system_id=reference_id;
	
		
	end if;
	sql:='select user_id,user_name from user_master where user_id='||user_id||' and user_type=''Vendor''';
	
	RAISE INFO '%', sql;
	RETURN QUERY
	EXECUTE 'select row_to_json(row) from ('||sql||') row';
	
END ;

$BODY$;

ALTER FUNCTION public.fn_update_selected_vendor_details(integer, integer, double precision)
    OWNER TO postgres;

----------------------------------------------------------Modify vw_att_details_pod_report view to add two columns for the site report--------------------
----------------------------------------------------------------------------------------------------------------------------------------------------------

-- View: public.vw_att_details_pod_report

-- DROP VIEW public.vw_att_details_pod_report;

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
	um3.user_name as contactor_name,
	COALESCE(pod.vendorcost, 0.00) AS vendor_cost
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
     LEFT JOIN entity_status_master es ON es.status::text = pod.status::text;

ALTER TABLE public.vw_att_details_pod_report
    OWNER TO postgres;

    ------------------------------------------Execute this function to sync the column names in the view-------------------------------------
    ------------------------------------------------------------------------------------------------------------------------------------------

    select * from  fn_sync_layer_columns()

