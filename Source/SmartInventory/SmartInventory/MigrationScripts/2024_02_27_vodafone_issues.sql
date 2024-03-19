
CREATE TABLE IF NOT EXISTS public.loop_geojson_master
(
    id serial,
    system_id integer,
    province_id integer,
    region_id integer,
    geojson jsonb
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.loop_geojson_master
    OWNER to postgres;


CREATE INDEX IF NOT EXISTS idx_loop_geojson_master_province_id
    ON public.loop_geojson_master USING btree
    (province_id ASC NULLS LAST)
    TABLESPACE pg_default;


CREATE INDEX IF NOT EXISTS idx_loop_geojson_master_system_id
    ON public.loop_geojson_master USING btree
    (system_id ASC NULLS LAST)
    TABLESPACE pg_default;



    
CREATE TABLE IF NOT EXISTS public.antenna_geojson_master
(
    id serial,
    system_id integer,
    province_id integer,
    region_id integer,
    geojson jsonb
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.antenna_geojson_master
    OWNER to postgres;


CREATE INDEX IF NOT EXISTS idx_antenna_geojson_master_province_id
    ON public.antenna_geojson_master USING btree
    (province_id ASC NULLS LAST)
    TABLESPACE pg_default;


CREATE INDEX IF NOT EXISTS idx_antenna_geojson_master_system_id
    ON public.antenna_geojson_master USING btree
    (system_id ASC NULLS LAST)
    TABLESPACE pg_default;




    CREATE TABLE IF NOT EXISTS public.fault_geojson_master
(
    id serial,
    system_id integer,
    province_id integer,
    region_id integer,
    geojson jsonb
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.fault_geojson_master
    OWNER to postgres;


CREATE INDEX IF NOT EXISTS idx_fault_geojson_master_province_id
    ON public.fault_geojson_master USING btree
    (province_id ASC NULLS LAST)
    TABLESPACE pg_default;


CREATE INDEX IF NOT EXISTS idx_fault_geojson_master_system_id
    ON public.fault_geojson_master USING btree
    (system_id ASC NULLS LAST)
    TABLESPACE pg_default;
	
	
	CREATE OR REPLACE VIEW public.vw_att_details_loop_vector
 AS
 SELECT loop.system_id,
    loop.region_id,
    loop.province_id,
    loop.network_id loop_name,   
    loop.loop_length,
    loop.network_status,   
    pm.sp_geometry,
    loop.network_id::character varying AS label_column,
    loop.network_id,
    loop.network_id display_name
   FROM point_master pm
     JOIN att_details_loop loop ON pm.system_id = loop.system_id AND pm.entity_type::text = 'Loop'::text;

ALTER TABLE public.vw_att_details_loop_vector
    OWNER TO postgres;







    CREATE TABLE IF NOT EXISTS public.loop_delta_metadata
(
    subarea_system_id integer,
    province_id integer,
    last_delta_type integer,
    last_delta_at timestamp without time zone
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.loop_delta_metadata
    OWNER to postgres;





    CREATE OR REPLACE VIEW public.vw_att_details_antenna_vector
 AS
 SELECT antenna.system_id,
    antenna.region_id,
    antenna.province_id,
    antenna.network_id antenna_name,      
    antenna.network_status,   
    pm.sp_geometry,
    antenna.network_id::character varying AS label_column,
    antenna.network_id,
    antenna.network_id display_name
   FROM point_master pm
     JOIN att_details_antenna antenna ON pm.system_id = antenna.system_id AND pm.entity_type::text = 'Antenna'::text;

ALTER TABLE public.vw_att_details_antenna_vector
    OWNER TO postgres;




       CREATE TABLE IF NOT EXISTS public.antenna_delta_metadata
(
    subarea_system_id integer,
    province_id integer,
    last_delta_type integer,
    last_delta_at timestamp without time zone
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.antenna_delta_metadata
    OWNER to postgres;





    CREATE OR REPLACE VIEW public.vw_att_details_fault_vector
 AS
 SELECT antenna.system_id,
    antenna.region_id,
    antenna.province_id,
    antenna.network_id fault_name,      
    antenna.network_status,   
    pm.sp_geometry,
    antenna.network_id::character varying AS label_column,
    antenna.network_id,
    antenna.network_id display_name
   FROM point_master pm
     JOIN att_details_fault antenna ON pm.system_id = antenna.system_id AND pm.entity_type::text = 'Fault'::text;

ALTER TABLE public.vw_att_details_fault_vector
    OWNER TO postgres;




       CREATE TABLE IF NOT EXISTS public.fault_delta_metadata
(
    subarea_system_id integer,
    province_id integer,
    last_delta_type integer,
    last_delta_at timestamp without time zone
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.fault_delta_metadata
    OWNER to postgres;


insert into layer_style_master(opacity,label_font_size,label_color_hex,label_bg_color_hex,icon_base_path,icon_file_name,line_width,label_expression,layer_sequence,layer_id) values
(0.4,10,'#a80000','#ddded3','/Content/images/icons/map/','loop.png',0,'[
  {
    "type": "UD",
    "Value": "loop1"
  },
  {
    "type": "Col",
    "Value": "display_name"
  },
  {
    "type": "UD",
    "Value": "loop1"
  },
  {
    "type": "Col",
    "Value": "network_status"
  }
]',25,(select layer_id from layer_details where layer_name='Loop'));




insert into layer_style_master(opacity,label_font_size,label_color_hex,label_bg_color_hex,icon_base_path,icon_file_name,line_width,label_expression,layer_sequence,layer_id) values
(0.4,10,'#a80000','#ddded3','/Content/images/icons/map/','antenna1.png',0,'[
  {
    "type": "UD",
    "Value": "antenna1"
  },
  {
    "type": "Col",
    "Value": "display_name"
  },
  {
    "type": "UD",
    "Value": "antenna1"
  },
  {
    "type": "Col",
    "Value": "network_status"
  }
]',26,(select layer_id from layer_details where layer_name='Antenna'));


update layer_details set isvisible=true,vc_view_name='vw_att_details_loop_vector',is_vector_layer_implemented=true,entity_geojson_table='loop_geojson_master'
where layer_id=(select layer_id from layer_details where layer_name='Loop');


update layer_details set is_visible_in_ne_library=true,isvisible=true,is_layer_for_rights_permission=true,vc_view_name='vw_att_details_antenna_vector',is_vector_layer_implemented=true,entity_geojson_table='antenna_geojson_master'
where layer_id=(select layer_id from layer_details where layer_name='Antenna');


update layer_details set is_visible_in_ne_library=true,isvisible=true,is_layer_for_rights_permission=true,vc_view_name='vw_att_details_fault_vector',is_vector_layer_implemented=true,entity_geojson_table='fault_geojson_master'
where layer_id=(select layer_id from layer_details where layer_name='Fault');