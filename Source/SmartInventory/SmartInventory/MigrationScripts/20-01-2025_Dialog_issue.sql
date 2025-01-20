
------------------------------To add 'Site report' _#10156------------------------------------------------------------------------------------------
--------------*Note : Once data inserted in "module_master" the update parent_module_id of the submodule with reference to module name "Site Report"-----
---------------------------------------------------------------------------------------------------------------------------------------------------------

insert into module_master(module_name,module_description,type,is_active,module_abbr,parent_module_id,module_sequence)
	  values('Site Report','Site Report','Web',true,'SR',0,0),
	        ('Site Report Data','Site Report Data','Web',true,'SRD',0,0);

insert into role_module_mapping(role_id,module_id)
	 values(2,(select id from module_master where module_name='Site Report')),
	       (2,(select id from module_master where module_name='Site Report Data'));

insert into user_module_mapping(user_id,module_id,modified_by,created_by)
	  values(5,(select id from module_master where module_name='Site Report'),0,1),
	        (5,(select id from module_master where module_name='Site Report Data'),0,1);

			--------------------------------------------------------Issue_#10161_-----------------------------------------------
			--------------------------------------------------------------------------------------------------------------------
			update legend_details set is_active=false where group_name='Access Cable' and type='Web'

			-------------------------------------------------------Issue _ #10155 _----------------------------------------------
			---------------------------------------------------------------------------------------------------------------------

			update res_resources set value='Manhole' where key='SI_OSP_GBL_JQ_FRM_217' and culture='en'

			--------------------------------------------------------Issue_ #10147 _---------------------------------------------------
			--------------------------------------------------------------------------------------------------------------------------

			update display_name_settings set display_column_name='microduct_name' where layer_id=(select layer_id from layer_details where upper(layer_name)=upper('Microduct'))

			---------------------------------------------------------Issue _ #10158 _---------------------------------------------------------------------
			----------------------------------------------------------------------------------------------------------------------------------------------

			-- View: public.vw_att_details_pod_map

-- DROP VIEW public.vw_att_details_pod_map;

CREATE OR REPLACE VIEW public.vw_att_details_pod_map
 AS
 SELECT pod.system_id,
    pod.region_id,
    pod.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    pod.pod_name,
    pod.project_id,
    pod.planning_id,
    pod.purpose_id,
    pod.workorder_id,
    pod.gis_design_id,
    pod.status,
    pod.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pod.network_status::text) AS network_status,
    pod.ownership_type,
    pod.third_party_vendor_id,
    pod.source_ref_id,
    pod.source_ref_type,
    lim.icon_path,
    (pod.site_id::text || '-'::text) || pod.site_name::text AS label_column
   FROM point_master point
     JOIN att_details_pod pod ON point.system_id = pod.system_id AND point.entity_type::text = 'POD'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = pod.system_id AND adi1.entity_type::text = 'pod'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pod.network_status::text) AND lim.layer_name::text = 'POD'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = pod.system_id AND adi.entity_type::text = 'POD'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_pod_map
    OWNER TO postgres;