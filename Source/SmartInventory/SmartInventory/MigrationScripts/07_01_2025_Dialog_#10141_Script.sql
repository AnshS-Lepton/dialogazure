/*<------------>Version:<------------>Query Type:View<------------>PR NO:3130<------------>Created By:Arabind Kumar<------------>Created On:27-Dec-24 05:23 PM<------------>*/

-- View: public.vw_att_details_pod

-- DROP VIEW public.vw_att_details_pod;

CREATE OR REPLACE VIEW public.vw_att_details_pod
 AS
 SELECT pod.system_id,
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
    (pod.site_id::text || '-'::text) || pod.site_name::text AS site_id_site_name
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

ALTER TABLE public.vw_att_details_pod
    OWNER TO postgres;
	
/*<------------>Version:<------------>Query Type:Function<------------>PR NO:3130<------------>Created By:Arabind Kumar<------------>Created On:27-Dec-24 05:21 PM<------------>*/

-- FUNCTION: public.fn_updatelabelvalue_map(character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_updatelabelvalue_map(character varying, character varying);
CREATE OR REPLACE FUNCTION public.fn_updatelabelvalue_map(
	entity_id character varying,
	label_column character varying)
    RETURNS TABLE(status character varying, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

	DECLARE
	from_position int;
	last_comma_position int;	
	before_from_query text;
	old_label_part text;
	new_label_part text;
	view_query text;
	view_name text;
	entity_type text;
	v_layer_table character varying;
	v_last_round_position int;
	sql text;
	

BEGIN

	select layer_name,layer_table into entity_type,v_layer_table from layer_details where lower(layer_id::character varying) = lower(''||entity_id||'');
		
	--raise info 'entity_type:%',entity_type;

	if not exists(select 1 from layer_details where upper(layer_name)=upper(entity_type) and is_label_change_allowed=true)
	then
	RAISE NOTICE 'This is a notice message';
		RETURN QUERY
		SELECT 'Falied'::character varying as status, ('Label change does not allowed for '||entity_type)::character varying as message;		
	else

	IF EXISTS(SELECT TRUE from INFORMATION_SCHEMA.VIEWS where lower(table_name)=lower('vw_att_details_'||entity_type||'_map'))
	THEN
		--VIEW NAME
	RAISE NOTICE 'This is a notice message11';
		view_name:='vw_att_details_'||entity_type||'_map';
		--raise info 'view_name:%',view_name;
		--GET VIEW DEFINATION
		select view_definition into view_query from INFORMATION_SCHEMA.views where upper(table_name) = upper(view_name);
		
		--raise info 'view_query:%',view_query;
		--PREPARE LABEL PART	
		--Comment the below code for new_label_part , and add condition for additional attributes	
		--new_label_part:=  entity_type||'.'||label_column||' AS label_column ';
		if not exists(select 1 from information_schema.columns WHERE upper(table_name) = upper(v_layer_table) AND upper(table_schema) = 'PUBLIC' 
		and upper(column_name)=upper(label_column))then
			if(lower(label_column)='site_id_site_name')then
			new_label_part:= '(pod.site_id::text || ''-''::text) || pod.site_name::text AS label_column';
			else
			new_label_part:=  entity_type||'.other_info::json ->>'''||label_column||''' AS label_column ';
			end if;	
		else

		     if exists(select 1 from information_schema.columns WHERE upper(table_name) = upper(v_layer_table) AND upper(table_schema) = 'PUBLIC' and
		      upper(column_name)=upper(label_column) and upper(data_type)='DOUBLE PRECISION')then

		      raise info'label_column1--%',label_column;

			new_label_part:=  'round('||entity_type||'.'||label_column||'::numeric,2) AS label_column '; 
		     else
			raise info'%label_column2222--',label_column;
		     new_label_part:=  entity_type||'.'||label_column||' AS label_column ';
		     end if;
			raise info 'label_column3 :%',new_label_part;
		end if;

		-- GET FROM POSITION  FROM VIEW DEFINATION
		SELECT   POSITION('FROM' in upper(view_query)) into from_position;

		raise info 'fromposition:%',from_position;
		--raise info 'view_query:%',view_query;
		-- GET BEFORE FROM STRING..
		SELECT Substring(view_query,1,from_position-1) into before_from_query;

		--raise info 'beforeFromString:%',before_from_query;
		--raise info 'beforeFromString:-----%',reverse(before_from_query);
		
		--GET COMMAN POSITION (LAST COMMA JUST BEFORE FROM KEYWORD
		if exists(select 1 from information_schema.columns WHERE upper(table_name) = upper(v_layer_table) AND upper(table_schema) = 'PUBLIC' and
		      upper(column_name) ilike upper('round%'))then
		      raise info 'XXXX--------%','111';
		select position('dnuor' in reverse(before_from_query)) +5 into v_last_round_position;
		
		raise info'last_round_position%----',v_last_round_position;

		 ELSE
		
		Select length(before_from_query) - position(',' in reverse(before_from_query)) + 1 into last_comma_position;

		END IF;  
		--else
		--Select length(before_from_query) - position(',' in reverse(before_from_query)) + 1 into last_comma_position;
		--end if;

		raise info 'commaposition:%',last_comma_position;
		
		
		--GET OLD LABEL PART FROM VIEW DEFINATION
		
		--select substring(view_query,last_comma_position+1,(from_position-1)-last_comma_position) into old_label_part;--28-11-2024 
       SELECT substring(view_query from ',\s*([^,]+)\s+FROM') into old_label_part;
		
		raise info 'length:%',LENGTH(old_label_part);
		raise info 'old_label_part:%',old_label_part;

		
		
		-- if(LENGTH(old_label_part) <> 0)
-- 		then
-- 		RETURN QUERY
-- 		SELECT 'failed'::character varying as status, 'label not found'::character varying as message;
-- 		end if;
		
		
		
		-- REPLACE OLD LABEL PART WITH NEW LABEL PART
		--commented by pk--select replace (view_query,old_label_part,new_label_part) into view_query;
		
		--implemented by pk-- REPLACE OLD LABEL PART WITH NEW LABEL PART quote_literal
		 --SELECT replace(view_query, quote_literal(old_label_part), quote_literal(new_label_part) INTO view_query;
       
	   
	   
	   SELECT replace(view_query, old_label_part, new_label_part) INTO view_query;
		

        raise info 'old_label_partpk1:%',old_label_part;
		raise info 'new_label_partpk2:%',new_label_part;

       -- raise info 'sqleee%',sql;
		raise info 'finalQuerypkkk:%',view_query;
		-- DROP EXISTING VIEW 

		

		EXECUTE('DROP VIEW '||UPPER(view_name));
		
		--CREATE VIEW AGAIN WITH UPDATED DEFINATION
		
  		--EXECUTE('CREATE OR REPLACE VIEW '||UPPER(view_name)||' AS '||view_query);	--arv
		EXECUTE('CREATE OR REPLACE VIEW '||UPPER(view_name)||' AS '||view_query);	

		-- insertquery = 'insert into label_settings(layer_id, label_columns, created_by)values ('||entity_id||', '''||label_column||''', '''||user_id||''')';
-- 
 		raise info 'finalQuery:%',view_query;
-- 
-- 		EXECUTE(insertquery);

		RETURN QUERY
		SELECT 'ok'::character varying as status, 'successfully'::character varying as message;

		
		
	
ELSE
RETURN QUERY
SELECT 'Falied'::character varying as status, 'View Not found'::character varying as message;		
END IF;	
   END IF;	
 exception 
 when others then 
 RETURN QUERY
  SELECT 'Exception'::character varying as status, SQLERRM ::character varying as message;
  
END;
$BODY$;

ALTER FUNCTION public.fn_updatelabelvalue_map(character varying, character varying)
    OWNER TO postgres;
	
	
---------------------------------------------------------------------------------------
/*<------------>Version:<------------>Query Type:Insert<------------>PR NO:3130<------------>Created By:Arabind Kumar<------------>Created On:27-Dec-24 05:24 PM<------------>*/

INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name='POD'), 'Info', 'site_id_site_name', 12, 'site_id_site_name', 'vw_att_details_pod_report', true, false, 1, now(), NULL, NULL, false, true, NULL);

--select * from layer_details where layer_name ilike '%survey%' in('CSA','DSA','Area','Antenna','HTB','CDB','Tree','ONT','Network_Ticket')

update layer_details set is_visible_in_ne_library=false,is_visible_in_mobile_lib=false,is_visible_on_mobile_map=false,isvisible=false where layer_name in('SurveyArea','CSA','DSA','Area','Antenna','HTB','CDB','Tree','ONT','Network_Ticket','Slack');

--select * from module_master where module_name ilike '%LMC%' and is_active=true

update module_master set is_active=false where module_name in('LMC Report','LMC Information','Survey Assignment','Building Survey','Survey Module','Survey Building','Survey');

