CREATE TABLE public.tower_delta_metadata (
	subarea_system_id int4 NULL,
	province_id int4 NULL,
	last_delta_type int4 NULL,
	last_delta_at timestamp NULL
);
CREATE INDEX idx_tower_delta_metadata_province_id ON public.tower_delta_metadata USING btree (province_id);

CREATE TABLE public.slack_delta_metadata (
	subarea_system_id int4 NULL,
	province_id int4 NULL,
	last_delta_type int4 NULL,
	last_delta_at timestamp NULL
);
CREATE INDEX idx_slack_delta_metadata_province_id ON public.slack_delta_metadata USING btree (province_id);

CREATE TABLE public.sector_delta_metadata (
	subarea_system_id int4 NULL,
	province_id int4 NULL,
	last_delta_type int4 NULL,
	last_delta_at timestamp NULL
);
CREATE INDEX idx_sector_delta_metadata_province_id ON public.sector_delta_metadata USING btree (province_id);

CREATE TABLE public.slack_geojson_master (
	id serial4 NOT NULL,
	system_id int4 NULL,
	province_id int4 NULL,
	region_id int4 NULL,
	geojson jsonb NULL
);
CREATE INDEX idx_slack_geojson_master_system_id ON public.slack_geojson_master USING btree (system_id);

CREATE TABLE public.sector_geojson_master (
	id serial4 NOT NULL,
	system_id int4 NULL,
	province_id int4 NULL,
	region_id int4 NULL,
	geojson jsonb NULL
);
CREATE INDEX idx_sector_geojson_master_system_id ON public.sector_geojson_master USING btree (system_id);

CREATE TABLE public.tower_geojson_master (
	id serial4 NOT NULL,
	system_id int4 NULL,
	province_id int4 NULL,
	region_id int4 NULL,
	geojson jsonb NULL
);
CREATE INDEX idx_tower_geojson_master_system_id ON public.tower_geojson_master USING btree (system_id);

INSERT INTO public.layer_style_master(
	layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression,  layer_sequence, planned, asbuild, dormant)
	select (select  layer_id from layer_details where layer_name = 'Tower'), color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, 'tower.svg', line_width, entity_category, entity_sub_category,  created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant
	from layer_style_master where layer_id in (select  layer_id from layer_details where layer_name = 'Cabinet');
	
		
		
INSERT INTO public.layer_style_master(
	layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression,  layer_sequence, planned, asbuild, dormant)
	select (select  layer_id from layer_details where layer_name = 'Slack'), color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, 'slack.svg', line_width, entity_category, entity_sub_category,  created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant
	from layer_style_master where layer_id in (select  layer_id from layer_details where layer_name = 'Cabinet');
	
INSERT INTO public.layer_style_master(
	layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression,  layer_sequence, planned, asbuild, dormant)
	select (select  layer_id from layer_details where layer_name = 'Sector'), color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, 'sector.svg', line_width, entity_category, entity_sub_category,  created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant
	from layer_style_master where layer_id in (select  layer_id from layer_details where layer_name = 'Cabinet');
	
update layer_details set vc_view_name='vw_att_details_tower_vector',delta_metadata_table='tower_delta_metadata',is_vector_layer_implemented=true,entity_geojson_table='tower_geojson_master' where upper(layer_name)='TOWER';

update layer_details set vc_view_name='vw_att_details_slack_vector',delta_metadata_table='slack_delta_metadata',is_vector_layer_implemented=true,entity_geojson_table='slack_geojson_master' where upper(layer_name)='SLACK';

update layer_details set vc_view_name='vw_att_details_sector_vector',delta_metadata_table='sector_delta_metadata',is_vector_layer_implemented=true,entity_geojson_table='sector_geojson_master' where upper(layer_name)='SECTOR';


CREATE OR REPLACE FUNCTION public.fn_save_slack_details(p_slacks text)
 RETURNS TABLE(status character varying, message character varying, systemid integer)
 LANGUAGE plpgsql
AS $function$
 
DECLARE  
  
 v_geometry geometry;
 v_longitude double precision;
 v_latitude double precision; 
 arow record;
 v_snapped_geom geometry;
   
BEGIN

CREATE temp TABLE temp_slack
(  
  system_id serial,
  network_id character varying(500),
  slack_length double precision,
  associated_system_id integer,
  associated_network_id character varying(200),
  associated_entity_type character varying(100),
  duct_system_id integer,
  created_by integer,
  modified_by integer,
  network_status character varying(25) NOT NULL DEFAULT 'P'::character varying,
  parent_system_id integer,
  sequence_id integer,
  province_id integer,
  region_id integer,
  parent_network_id character varying(100),
  parent_entity_type character varying(100),
  status_remark character varying(2000),
  status_updated_by integer,
  source_ref_type character varying(100),
  source_ref_id character varying,
  source_ref_description character varying,
  latitude double precision,
  longitude double precision,
  origin_from character varying,
  origin_ref_id character varying,
  origin_ref_code character varying,
  origin_ref_description character varying,
  request_ref_id character varying,
  requested_by character varying,
  request_approved_by character varying,
  subarea_id character varying,
  area_id character varying,
  dsa_id character varying,
  csa_id character varying,
  ne_id character varying,
  prms_id character varying,
  jc_id character varying,
  mzone_id character varying
) ON COMMIT DROP ;  


FOR arow IN  
select system_id,network_id ,slack_length ,associated_system_id ,associated_network_id ,associated_entity_type,duct_system_id ,created_by ,modified_by ,network_status,parent_system_id ,sequence_id ,province_id ,region_id ,parent_network_id ,parent_entity_type ,status_remark ,status_updated_by ,source_ref_type ,source_ref_id ,source_ref_description ,latitude ,longitude ,origin_from ,origin_ref_id ,origin_ref_code ,origin_ref_description ,request_ref_id ,requested_by ,request_approved_by ,subarea_id ,area_id ,dsa_id ,csa_id ,ne_id ,prms_id ,jc_id ,mzone_id from json_populate_record(null::temp_slack,replace(p_Slacks,'\','')::json)

LOOP

	
IF(arow.system_id = 0  AND arow.slack_length > 0) THEN
		select ST_ClosestPoint(sp_geometry,ST_GeomFromText('POINT('||arow.longitude||' '||arow.latitude||')',4326)) into v_geometry from  line_master  where  system_id=arow.associated_system_id and upper(entity_type)=upper('DUCT') ;
		
		v_longitude=st_x(v_geometry);
		v_latitude=st_y(v_geometry);
		

	 INSERT INTO att_details_slack(network_id,slack_length,associated_system_id,associated_network_id,associated_entity_type,
	duct_system_id,network_status,parent_system_id,sequence_id,province_id,region_id,parent_network_id,parent_entity_type,status_remark,
	source_ref_type,source_ref_id,source_ref_description,created_by,status_updated_by,latitude,longitude,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,ne_id,prms_id,jc_id,mzone_id)
	 values( arow.network_id,arow.slack_length,arow.associated_system_id,arow.associated_network_id,arow.associated_entity_type,
	arow.duct_system_id,coalesce(arow.network_status,'P'),arow.parent_system_id,arow.sequence_id,arow.province_id,
	arow.region_id,arow.parent_network_id,arow.parent_entity_type,arow.status_remark,arow.source_ref_type,arow.source_ref_id,arow.source_ref_description,
	arow.created_by,arow.status_updated_by,v_latitude,v_longitude,arow.origin_from,arow.origin_ref_id,arow.origin_ref_code,arow.origin_ref_description,
	arow.request_ref_id,arow.requested_by,arow.request_approved_by,arow.subarea_id,arow.area_id,arow.dsa_id,arow.csa_id,arow.ne_id,arow.prms_id,arow.jc_id,arow.mzone_id) returning system_id into arow.system_id;

--PointMaster entry--
	INSERT INTO point_master(system_id, entity_type, longitude, latitude, approval_flag, sp_geometry,approval_date, creator_remark, approver_remark, created_by,common_name, db_flag,network_status,display_name)
	VALUES (arow.system_id,'Slack', arow.longitude,arow.latitude,'A',ST_GEOMFROMTEXT('POINT('||v_longitude||' '||v_latitude||')',4326),now(),'NA','NA',arow.created_by,arow.network_id,0,coalesce(arow.network_status,'P'),fn_get_display_name(arow.system_id,'Slack'));

PERFORM (fn_geojson_update_entity_attribute(arow.system_id,'Slack',arow.province_id,0,false));

-- Snapping cable to loop
SELECT ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01) into v_snapped_geom
  FROM (
 SELECT SP_GEOMETRY AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = arow.system_id and lower(entity_type)=lower('Slack')
  ) AS point,
   LINE_MASTER AS LM  WHERE LM.system_id = arow.duct_system_id and lower(entity_type)=lower('duct');

update LINE_MASTER set SP_GEOMETRY=v_snapped_geom  WHERE system_id = arow.duct_system_id and lower(entity_type)=lower('duct');

return query select 'True'::character varying as status, 'Slack Inserted Sucessfully'::character varying as message,arow.system_id as systemId ;

ELSE

UPDATE att_details_slack set slack_length= arow.slack_length ,associated_system_id=arow.associated_system_id ,associated_network_id=arow.associated_network_id,associated_entity_type=arow.associated_entity_type,duct_system_id=arow.duct_system_id,modified_by=arow.modified_by,modified_on=now(),ne_id=arow.ne_id,prms_id=arow.prms_id,jc_id=arow.jc_id,mzone_id=arow.mzone_id where system_id=arow.system_id;

PERFORM (fn_geojson_update_entity_attribute(arow.system_id,'Slack',arow.province_id,1,false));


return query select 'True'::character varying as status, 'Slack Updated Sucessfully'::character varying as message,arow.system_id as systemId;

END IF;
END LOOP;
	

END
$function$
;

CREATE OR REPLACE VIEW public.vw_att_details_tower_vector
AS SELECT tower.system_id,
    tower.region_id,
    tower.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    tower.network_name,
    tower.project_id,
    tower.planning_id,
    tower.workorder_id,
    tower.purpose_id,
    tower.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), tower.network_status::text) AS network_status,
    tower.ownership_type,
    tower.third_party_vendor_id,
    tower.source_ref_id,
    tower.source_ref_type,
    tower.network_id AS label_column,
        CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN tower.network_id
            ELSE point.display_name
        END AS display_name
     FROM point_master point
     JOIN att_details_tower tower ON point.system_id = tower.system_id AND upper(point.entity_type::text) = 'TOWER'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = tower.system_id AND upper(adi1.entity_type::text) = upper('tower'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = tower.system_id AND upper(adi.entity_type::text) = upper('tower'::text);


CREATE OR REPLACE VIEW public.vw_att_details_slack_vector
AS SELECT slack.system_id,
    slack.region_id,
    slack.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    slack.project_id,
    slack.planning_id,
    slack.workorder_id,
    slack.purpose_id,
    slack.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), slack.network_status::text) AS network_status,
    slack.source_ref_id,
    slack.source_ref_type,
    slack.network_id AS label_column,
        CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN slack.network_id
            ELSE point.display_name
        END AS display_name
   FROM point_master point
     JOIN att_details_slack slack ON point.system_id = slack.system_id AND upper(point.entity_type::text) = 'SLACK'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = slack.system_id AND upper(adi1.entity_type::text) = upper('slack'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = slack.system_id AND upper(adi.entity_type::text) = upper('slack'::text);




CREATE OR REPLACE VIEW public.vw_att_details_sector_vector
AS SELECT sector.system_id,
    sector.region_id,
    sector.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    sector.network_name,
    sector.project_id,
    sector.planning_id,
    sector.workorder_id,
    sector.purpose_id,
    sector.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), sector.network_status::text) AS network_status,
    sector.ownership_type,
    sector.third_party_vendor_id,
    sector.source_ref_id,
    sector.source_ref_type,
    sector.network_id AS label_column,
        CASE
            WHEN COALESCE(point.display_name, ''::character varying)::text = ''::text THEN sector.network_id
            ELSE point.display_name
        END AS display_name
     FROM polygon_master point
     JOIN att_details_sector sector ON point.system_id = sector.system_id AND upper(point.entity_type::text) = 'SECTOR'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = sector.system_id AND upper(adi1.entity_type::text) = upper('sector'::text) AND upper(adi1.description::text) = upper('NS'::text)
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = sector.system_id AND upper(adi.entity_type::text) = upper('sector'::text); 