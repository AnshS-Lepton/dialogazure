CREATE OR REPLACE VIEW public.vw_att_details_duct
 AS
 SELECT duct.system_id,
    duct.network_id,
    duct.duct_name,
    round(duct.calculated_length::numeric, 2) AS calculated_length,
    round(duct.manual_length::numeric, 2) AS manual_length,
    duct.a_system_id,
    duct.a_location,
    duct.a_entity_type,
    duct.b_system_id,
    duct.b_location,
    duct.b_entity_type,
    duct.sequence_id,
    duct.network_status,
    duct.status,
    duct.pin_code,
    duct.province_id,
    duct.region_id,
    duct.utilization,
    duct.no_of_cables,
    duct.offset_value,
    duct.construction,
    duct.activation,
    duct.accessibility,
    duct.specification,
    duct.category,
    round(duct.inner_dimension::numeric, 2) AS inner_dimension,
    round(duct.outer_dimension::numeric, 2) AS outer_dimension,
    duct.duct_type,
    duct.color_code,
    duct.subcategory1,
    duct.subcategory2,
    duct.subcategory3,
    duct.item_code,
    duct.vendor_id,
    duct.type,
    duct.brand,
    duct.model,
    duct.remarks,
    duct.created_by,
    duct.created_on,
    duct.modified_by,
    duct.modified_on,
    duct.trench_id,
    duct.project_id,
    duct.planning_id,
    duct.purpose_id,
    duct.workorder_id,
    prov.province_name,
    duct.parent_system_id,
    duct.parent_entity_type,
    duct.parent_network_id,
    duct.acquire_from,
    reg.region_name,
    um.user_name,
    vm.name AS vendor_name,
    bd.brand AS duct_brand,
    ml.model AS duct_model,
    fn_get_date(duct.created_on) AS created_date,
    fn_get_date(duct.modified_on) AS modified_date,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.accessibility) AS duct_accessibility,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.construction) AS duct_construction,
    ( SELECT system_spec_master.prop_name
           FROM system_spec_master
          WHERE system_spec_master.id = duct.activation) AS duct_activation,
    duct.barcode,
    duct.ownership_type,
    vm2.id AS third_party_vendor_id,
    vm2.name AS third_party_vendor_name,
    duct.source_ref_type,
    duct.source_ref_id,
    duct.source_ref_description,
    duct.status_remark,
    duct.status_updated_by,
    duct.status_updated_on,
    duct.is_visible_on_map,
    duct.primary_pod_system_id,
    duct.secondary_pod_system_id,
    duct.is_new_entity,
    duct.audit_item_master_id,
    duct.is_acquire_from,
    duct.duct_capacity,
    duct.origin_from,
    duct.origin_ref_id,
    duct.origin_ref_code,
    duct.origin_ref_description,
    duct.request_ref_id,
    duct.requested_by,
    duct.request_approved_by,
    duct.subarea_id,
    duct.area_id,
    duct.dsa_id,
    duct.csa_id,
    duct.bom_sub_category,
    duct.gis_design_id,
    duct.ne_id,
    duct.prms_id,
    duct.jc_id,
    duct.mzone_id,
    duct.duct_color,
    coalesce(duct.total_slack_count,0) as total_slack_count,
    coalesce(duct.total_slack_length,0) as total_slack_length
   FROM att_details_duct duct
     JOIN province_boundary prov ON duct.province_id = prov.id
     JOIN region_boundary reg ON duct.region_id = reg.id
     JOIN user_master um ON duct.created_by = um.user_id
     JOIN vendor_master vm ON duct.vendor_id = vm.id
     LEFT JOIN vendor_master vm2 ON duct.third_party_vendor_id = vm2.id
     LEFT JOIN isp_type_master tp ON duct.type = tp.id
     LEFT JOIN isp_brand_master bd ON duct.brand = bd.id
     LEFT JOIN isp_base_model ml ON duct.model = ml.id
     LEFT JOIN user_master um2 ON duct.modified_by = um2.user_id;








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







CREATE OR REPLACE FUNCTION public.fn_get_structure_cables(
	structid integer)
    RETURNS TABLE(system_id integer, network_id character varying, cable_name character varying, total_core integer, a_entity_type character varying, b_entity_type character varying, a_system_id integer, b_system_id integer, line_geom character varying, cable_type character varying, network_status character varying, a_location character varying, b_location character varying, entity_name character varying, entity_title character varying, a_node_type character varying, b_node_type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

BEGIN
 RETURN QUERY
 select * from (

 --select c.system_id,c.network_id,c.cable_name,c.total_core,c.a_entity_type,c.b_entity_type,c.a_system_id,c.b_system_id,icm.line_geom,COALESCE(icm.cable_type,'ISP') as cabletype,c.network_status,c.a_location,c.b_location  
 --from att_details_cable c
--left join isp_line_master icm on (icm.entity_id=c.system_id and upper(entity_type)=upper('cable'))
--where upper(c.cable_type)='ISP' and c.structure_id=structid or ((c.a_entity_type='BDB' or c.b_entity_type='BDB' or c.a_entity_type='FDB' or c.b_entity_type='FDB') and c.structure_id=structid )

select c.system_id,c.network_id,c.cable_name,c.total_core,c.a_entity_type,c.b_entity_type,c.a_system_id,c.b_system_id,
COALESCE(isplm.line_geom,'') as line_geom,COALESCE(isplm.cable_type,'ISP') as cabletype,c.network_status,
	 c.a_location,c.b_location, ld.layer_name as entity_name,ld.layer_title as entity_title,isplm.a_node_type,isplm.b_node_type
from isp_line_master isplm 
inner join att_details_cable c
on isplm.entity_id=c.system_id and upper(isplm.entity_type)=upper('cable')
left join layer_details ld
on upper(isplm.entity_type)=upper(ld.layer_name) where isplm.structure_id=structid
     
     union
select c.system_id,c.network_id,c.duct_name as cable_name, 0 as total_core,c.a_entity_type,c.b_entity_type,c.a_system_id,c.b_system_id,
COALESCE(isplm.line_geom,'') as line_geom,COALESCE(isplm.cable_type,'ISP') as cabletype,c.network_status,
	 c.a_location,c.b_location, ld.layer_name as entity_name,ld.layer_title as entity_title,isplm.a_node_type,isplm.b_node_type
from isp_line_master isplm 
inner join att_details_duct c
on isplm.entity_id=c.system_id and upper(isplm.entity_type)=upper('duct')
left join layer_details ld
on upper(isplm.entity_type)=upper(ld.layer_name) where isplm.structure_id=structid

) as tblMain order by system_id asc;


END;

$BODY$;





update layer_details set is_isp_layer = true where layer_name ='Duct' and layer_template_table='item_template_duct';
update layer_action_mapping set is_isp_action = false where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='ViewAccessories';
update layer_action_mapping set is_isp_action = true where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='Save';
update layer_action_mapping set is_isp_action = false where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='ItemAssociate';
update layer_action_mapping set is_isp_action = false where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='InsideCable';
update layer_action_mapping set is_isp_action = true where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='Delete';
update layer_action_mapping set is_isp_action = true where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='Detail';
update layer_action_mapping set is_isp_action = true where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='Cancel';
update layer_action_mapping set is_isp_action = true where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='Export';
update layer_action_mapping set is_isp_action = true where layer_id = (select layer_id from layer_details  where layer_name='Duct') and action_name ='History';