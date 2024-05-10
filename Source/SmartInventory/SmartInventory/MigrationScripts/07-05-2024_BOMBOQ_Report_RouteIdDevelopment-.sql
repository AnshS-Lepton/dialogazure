DROP VIEW public.vw_att_details_cabinet_vector;

CREATE OR REPLACE VIEW public.vw_att_details_cabinet_vector
 AS
 SELECT cabinet.system_id,
    cabinet.region_id,
    cabinet.province_id,
    COALESCE(pm.system_id, 0) AS subarea_id,
    cabinet.cabinet_name,
    cabinet.network_id,
    cabinet.network_status,
    cabinet.gis_design_id,
    point.display_name,
    point.entity_category,
    cabinet.ownership_type,
    point.sp_geometry,
    coalesce(cabinet.third_party_vendor_id,0)third_party_vendor_id
   FROM point_master point
     JOIN att_details_cabinet cabinet ON point.system_id = cabinet.system_id AND point.entity_type::text = 'Cabinet'::text
     LEFT JOIN polygon_master pm ON pm.entity_type::text = 'SubArea'::text AND st_within(point.sp_geometry, pm.sp_geometry);

ALTER TABLE public.vw_att_details_cabinet_vector
    OWNER TO postgres;



truncate table Cabinet_geojson_master;
INSERT INTO Cabinet_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','Cabinet',
		   'layer_title','Cabinet',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_Cabinet_vector 
		) ROW;



DROP VIEW public.vw_att_details_htb_vector;

CREATE OR REPLACE VIEW public.vw_att_details_htb_vector
 AS
 SELECT htb.system_id,
    htb.region_id,
    htb.province_id,
    htb.htb_name,
    htb.htb_type,
    htb.network_status,
    htb.type,
    htb.gis_design_id,
    htb.category,
    point.display_name,
    point.entity_category,
    htb.ownership_type,
	 point.sp_geometry,
    COALESCE(htb.third_party_vendor_id, 0) AS third_party_vendor_id
   FROM point_master point
     JOIN isp_htb_info htb ON point.system_id = htb.system_id AND upper(point.entity_type)::text = 'HTB'::text;

ALTER TABLE public.vw_att_details_htb_vector
    OWNER TO postgres;



truncate table htb_geojson_master;
INSERT INTO htb_geojson_master(system_id, province_id, region_id, geojson)
		SELECT system_id, province_id, region_id, jsonb_build_object(
		   'type',       'Feature',
		   'entity_type','HTB',
		   'layer_title','ATB',
		   'geometry',   ST_AsGeoJSON(sp_geometry)::jsonb,
		   'properties', to_jsonb(row) - 'sp_geometry'
		) AS geojson
		FROM 
		(
			SELECT *  FROM vw_att_details_htb_vector 
		) ROW;