-------------------------------CDB---------------------------------
CREATE OR REPLACE VIEW public.vw_att_details_cdb_vector
 AS
 SELECT cdb.system_id,
    cdb.region_id,
    cdb.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    cdb.cdb_name,
    cdb.network_id,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), cdb.network_status::text) AS network_status,
    cdb.gis_design_id AS label_column,
    point.display_name,
    COALESCE(point.entity_category, ''::character varying) AS entity_category
   FROM point_master point
     JOIN att_details_cdb cdb ON point.system_id = cdb.system_id AND point.entity_type::text = 'CDB'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = cdb.system_id AND adi1.entity_type::text = 'CDB'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = cdb.system_id AND adi.entity_type::text = 'CDB'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text;

ALTER TABLE public.vw_att_details_cdb_vector
    OWNER TO postgres;
------------------------------------------------------------------------------------------------
	-- Table: public.cdb_geojson_master

-- DROP TABLE IF EXISTS public.cdb_geojson_master;

CREATE TABLE IF NOT EXISTS public.cdb_geojson_master
(
    id serial,
    system_id integer,
    province_id integer,
    region_id integer,
    geojson jsonb
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.cdb_geojson_master
    OWNER to postgres;
-- Index: idx_cdb_geojson_master_province_id

-- DROP INDEX IF EXISTS public.idx_cdb_geojson_master_province_id;

CREATE INDEX IF NOT EXISTS idx_cdb_geojson_master_province_id
    ON public.cdb_geojson_master USING btree
    (province_id ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: idx_cdb_geojson_master_system_id

-- DROP INDEX IF EXISTS public.idx_cdb_geojson_master_system_id;

CREATE INDEX IF NOT EXISTS idx_cdb_geojson_master_system_id
    ON public.cdb_geojson_master USING btree
    (system_id ASC NULLS LAST)
    TABLESPACE pg_default;
---------------------------------------------------------------------------------------------
	-- Table: public.cdb_delta_metadata

-- DROP TABLE IF EXISTS public.cdb_delta_metadata;

CREATE TABLE IF NOT EXISTS public.cdb_delta_metadata
(
    subarea_system_id integer,
    province_id integer,
    last_delta_type integer,
    last_delta_at timestamp without time zone
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.cdb_delta_metadata
    OWNER to postgres;
--------------------------------------------------------------------------------
	INSERT INTO public.layer_style_master(
	layer_id, color_code_hex,  opacity, label_font_size,
	label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name,
	line_width, 
	 created_on, created_by, 
	label_expression, 	
	layer_sequence)
	VALUES ((select layer_id from layer_details where layer_name='CDB'),
	'',  0.4, 15, '#a80000', '#ddded3', 'Content/images/icons/map/',
	'CDB.svg', 1, 	  now(), 1,  
	'[
  {
    "type": "UD",
    "Value": "ont1"
  },
  {
    "type": "Col",
    "Value": "display_name"
  },
  {
    "type": "UD",
    "Value": "ont1"
  },
  {
    "type": "Col",
    "Value": "network_status"
  }
]', 18 );
-------------------------------------------------------------MICRODUCT---------------------------------------------------------------------------
ALTER TABLE att_details_microduct RENAME COLUMN network_name to microduct_name;
update layer_details set is_vector_layer_implemented=true where layer_name='Microduct';
update layer_details set template_form_url='vw_att_details_microduct_vector' where layer_name='Microduct';
update layer_details set template_form_url='ItemTemplate/MicroductTemplate' where layer_name='Microduct';
update layer_details set isvisible=true where layer_name='Microduct';
update layer_details set entity_geojson_table='microduct_geojson_master' where layer_name='Microduct';
-----------------------------------------------------------------------------------------------------------------------------------------------------
-- Table: public.microduct_geojson_master

-- DROP TABLE IF EXISTS public.microduct_geojson_master;

CREATE TABLE IF NOT EXISTS public.microduct_geojson_master
(
    id serial,
    system_id integer,
    province_id integer,
    region_id integer,
    geojson jsonb
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.microduct_geojson_master
    OWNER to postgres;
-- Index: idx_microduct_geojson_master_province_id

-- DROP INDEX IF EXISTS public.idx_microduct_geojson_master_province_id;

CREATE INDEX IF NOT EXISTS idx_microduct_geojson_master_province_id
    ON public.microduct_geojson_master USING btree
    (province_id ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: idx_microduct_geojson_master_system_id

-- DROP INDEX IF EXISTS public.idx_microduct_geojson_master_system_id;

CREATE INDEX IF NOT EXISTS idx_microduct_geojson_master_system_id
    ON public.microduct_geojson_master USING btree
    (system_id ASC NULLS LAST)
    TABLESPACE pg_default;
-----------------------------------------------------------------------------------------------------------------------------------------------------
-- Table: public.microduct_delta_metadata

-- DROP TABLE IF EXISTS public.microduct_delta_metadata;

CREATE TABLE IF NOT EXISTS public.microduct_delta_metadata
(
    subarea_system_id integer,
    province_id integer,
    last_delta_type integer,
    last_delta_at timestamp without time zone
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.microduct_delta_metadata
    OWNER to postgres;
-----------------------------------------------------------------------------------------------------------------------------------------------

-- View: public.vw_att_details_microduct_vector

-- DROP VIEW public.vw_att_details_microduct_vector;

CREATE OR REPLACE VIEW public.vw_att_details_microduct_vector
 AS
 SELECT microduct.system_id,
    microduct.network_id,
    microduct.microduct_name,
    microduct.pin_code,
    COALESCE(pmapping.province_id, microduct.province_id) AS province_id,
    COALESCE(pmapping.region_id, microduct.region_id) AS region_id,
    microduct.a_location,
    microduct.b_location,
    microduct.calculated_length,
    microduct.manual_length,
    microduct.construction,
    microduct.activation,
    microduct.accessibility,
    microduct.specification,
    microduct.category,
    microduct.vendor_id,
    microduct.type,
    microduct.brand,
    microduct.model,
    microduct.created_by,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), microduct.network_status::text) AS network_status,
    microduct.project_id,
    microduct.planning_id,
    microduct.workorder_id,
    microduct.purpose_id,
    microduct.status,
    microduct.microduct_type,
    microduct.color_code,
    COALESCE(adi.updated_geom, lm.sp_geometry) AS sp_geometry,
        CASE
            WHEN microduct.no_of_cables = 0 THEN 'vacant'::text
            ELSE 'used'::text
        END AS utilization,
    microduct.ownership_type,
    microduct.third_party_vendor_id,
    microduct.primary_pod_system_id,
    microduct.secondary_pod_system_id,
    microduct.source_ref_id,
    microduct.source_ref_type,
    microduct.microduct_name AS label_column,
    COALESCE(microduct.category, ''::character varying) AS entity_category,
        CASE
            WHEN COALESCE(microduct.gis_design_id, ''::character varying)::text = ''::text THEN microduct.network_id
            ELSE microduct.gis_design_id
        END AS display_name
   FROM line_master lm
     JOIN att_details_microduct microduct ON lm.system_id = microduct.system_id AND lm.entity_type::text = 'Microduct'::text
     LEFT JOIN entity_region_province_mapping pmapping ON
        CASE
            WHEN upper(microduct.network_id::text) ~~ 'NLD%'::text THEN microduct.system_id = pmapping.entity_id AND upper(pmapping.entity_type::text) = upper('Microduct'::text)
            ELSE 1 = 2
        END
     LEFT JOIN vw_att_details_edit_entity_info adi ON adi.entity_system_id = microduct.system_id AND upper(adi.entity_type::text) = upper('Microduct'::text) AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = microduct.system_id AND upper(adi1.entity_type::text) = upper('Microduct'::text) AND upper(adi1.description::text) = upper('NS'::text);

ALTER TABLE public.vw_att_details_microduct_vector
    OWNER TO postgres;

-------------------------------------------------------------------------------------------------------------------------------------------------------------------
INSERT INTO public.layer_style_master(
	layer_id, color_code_hex,  opacity, label_font_size,
	label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name,
	line_width, 
	 created_on, created_by, 
	label_expression, 	
	layer_sequence)
	VALUES ((select layer_id from layer_details where layer_name='Microduct'),
	'#14d29c',  0.4, 11, '#a80000', '#ddded3', 'Content/images/icons/map/',
	'microduct.png', 1, 	  now(), 1,  
	'[
  {
    "type": "UD",
    "Value": "ont"
  },
  {
    "type": "Col",
    "Value": "category"
  },
  {
    "type": "UD",
    "Value": "ont"
  },
  {
    "type": "Col",
    "Value": "network_status"
  }
]', 18 );
--------------------------------------------------------------------------------------------------------------------------------------------------------------
INSERT INTO global_settings( key, value,
 created_on, 
	created_by, description,type,data_type,is_web_key,min_value,max_value)
	VALUES ( 'SplitMicroductBuffer', '2',
			 now(), 1, 'in meter','Web','int',true,1,1);
			
INSERT INTO global_settings( key, value,
 created_on, 
	created_by, description,type,data_type,is_web_key,min_value,max_value)
	VALUES ( 'MicroductOffset', '1.5',
			 now(), 1, 'in meter','Web','double precision',true,5,1000);
---------------------------------------------------------------------------------------------------
			
	-- FUNCTION: public.fn_get_near_ducts(integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_near_ducts(integer, character varying);


CREATE OR REPLACE FUNCTION public.fn_get_near_microducts(
	p_systemid integer,
	p_entitytype character varying)
    RETURNS TABLE(geom_type character varying, entity_type character varying, system_id integer, common_name text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


DECLARE
  lat double precision;
  lng double precision;
  mtrBuffer integer;
  s_tbl_name text;
  
BEGIN
s_tbl_name := 'att_details_'||P_entityType;
EXECUTE 'select latitude,longitude from '||s_tbl_name||' where system_id='||P_systemId||'' into lat,lng;
EXECUTE 'select value from global_settings where key=''SplitMicroductBuffer''' into mtrBuffer;

raise info 'lat%',lat;
raise info 'lng%',lng;
raise info 'mtrBuffer%',mtrBuffer;

RETURN QUERY
  
select distinct 'Line'::character varying as geom_type, tbl.entity_type, tbl.system_id, CONCAT(tbl.display_name,' ', '(' ,tbl.network_status ,')' ) as common_name from ( 	
	select l.entity_type, l.system_id, l.display_name,add.network_status from line_master l 
	inner join att_details_microduct as add on upper(l.entity_type)='MICRODUCT' 
	and not ST_Intersects(ST_STARTPOINT(l.sp_geometry), ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer))
        and not ST_Intersects(ST_ENDPOINT(l.sp_geometry), ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer))
        and ST_Intersects(st_makevalid(l.sp_geometry), ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer)) and l.system_id=add.system_id 
        ) tbl order by entity_type;
END
$BODY$;

ALTER FUNCTION public.fn_get_near_microducts(integer, character varying)
    OWNER TO postgres;
	---------------------------------------------------------------------------------------------------------------------------------------------------------
	-- FUNCTION: public.fn_get_split_microduct_details(integer, character varying, character varying, integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_split_microduct_details(integer, character varying, character varying, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_split_micromicroduct_details(
	p_splitentitysystemid integer,
	p_splitentitytype character varying,
	p_split_entity_networkid character varying,
	p_microductid integer,
	p_entity_type character varying)
    RETURNS TABLE(system_id integer, common_name character varying, network_status character varying, geom_microduct1 text, geom_microduct2 text, microduct_one_a_system_id integer, microduct_one_a_entity_type character varying, microduct_one_a_location character varying, microduct_one_b_system_id integer, microduct_one_b_entity_type character varying, microduct_one_b_location character varying, microduct_two_a_system_id integer, microduct_two_a_entity_type character varying, microduct_two_a_location character varying, microduct_two_b_system_id integer, microduct_two_b_entity_type character varying, microduct_two_b_location character varying, microduct1length double precision, microduct2length double precision) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

declare 
v_a_end_geometry geometry;
v_b_end_geometry geometry;
v_a_system_id integer;
v_a_entity_type character varying;
v_b_system_id integer;
v_b_entity_type character varying;
v_a_location character varying;
v_b_location character varying;
BEGIN

select p.sp_geometry,a_system_id,a_entity_type,a_location into v_a_end_geometry,v_a_system_id,v_a_entity_type,v_a_location from point_master p join att_details_microduct c on p.system_id=c.a_system_id and upper(p.entity_type)=upper(c.a_entity_type) where c.system_id=p_microductid;
select p.sp_geometry,b_system_id,b_entity_type,b_location into v_b_end_geometry,v_b_system_id,v_b_entity_type,v_b_location from point_master p join att_details_microduct c on p.system_id=c.b_system_id and upper(p.entity_type)=upper(c.b_entity_type) where c.system_id=p_microductid;

    RETURN QUERY

select geo3.SYSTEM_ID,geo3.COMMON_NAME,geo3.network_status,geo3.microduct1,geo3.microduct2,
case when geo3.microduct_one_start_matched_with_a then v_a_system_id 
when geo3.microduct_one_start_matched_with_b then v_b_system_id  end as microduct_one_a_system_id, 

case when geo3.microduct_one_start_matched_with_a then v_a_entity_type 
when geo3.microduct_one_start_matched_with_b then v_b_entity_type end as microduct_one_a_entity_type,
case when geo3.microduct_one_start_matched_with_a then v_a_location 
when geo3.microduct_one_start_matched_with_b then v_b_location end as microduct_one_a_location,

p_splitentitysystemid as microduct_one_b_system_id,
p_splitentitytype as microduct_one_b_entity_type,
p_split_entity_networkId as microduct_one_b_location,

p_splitentitysystemid as microduct_two_a_system_id,
p_splitentitytype as microduct_two_a_entity_type,
p_split_entity_networkId as microduct_two_a_location,
case when geo3.microduct_two_end_matched_with_a then v_a_system_id  
when geo3.microduct_two_end_matched_with_b then v_b_system_id  end as microduct_two_b_system_id, 
case when geo3.microduct_two_end_matched_with_a then v_a_entity_type 
when geo3.microduct_two_end_matched_with_b then v_b_entity_type end as microduct_two_b_entity_type,
case when geo3.microduct_two_end_matched_with_a then v_a_location 
when geo3.microduct_two_end_matched_with_b then v_b_location end as microduct_two_b_location
,(select * from getgeometrylength(''|| geo3.microduct1||''))as microduct1Length ,(select * from getgeometrylength(''|| geo3.microduct2||''))as microduct2Length  from (
select 	 geo1.SYSTEM_ID,geo1.COMMON_NAME,geo1.network_status,REPLACE(REPLACE(geo1.microduct1, 'LINESTRING(', ''),')','')as microduct1,REPLACE(REPLACE (geo1.microduct2, 'LINESTRING(', ''),')','')as microduct2,
case when st_startpoint(st_geomfromtext(geo1.microduct1,4326))=v_a_end_geometry then true else false end as microduct_one_start_matched_with_a,
case when st_startpoint(st_geomfromtext(geo1.microduct1,4326))=v_b_end_geometry then true else false end as microduct_one_start_matched_with_b,
case when st_endpoint(st_geomfromtext(geo1.microduct2,4326))=v_a_end_geometry then true else false end  as microduct_two_end_matched_with_a,
case when st_endpoint(st_geomfromtext(geo1.microduct2,4326))=v_b_end_geometry then true else false end  as microduct_two_end_matched_with_b

  from (
select geom.SYSTEM_ID,geom.COMMON_NAME,geom.network_status, 
ST_ASTEXT(ST_LINESUBSTRING(ST_GEOMFROMTEXT(ST_ASTEXT(EXISTING_GEOMETRY)),0,geom.INTERPOLATE_POINT)) AS microduct1,
ST_ASTEXT(ST_LINESUBSTRING(ST_GEOMFROMTEXT(ST_ASTEXT(EXISTING_GEOMETRY)), geom.INTERPOLATE_POINT, 1)) AS microduct2

FROM (
SELECT LM.SYSTEM_ID, 
 LM.COMMON_NAME,
  ST_LINELOCATEPOINT(ST_ASTEXT(LM.SP_GEOMETRY), point.THE_POINT) AS INTERPOLATE_POINT,LM.SP_GEOMETRY as EXISTING_GEOMETRY ,LM.network_status 
  FROM (
 SELECT ST_ASTEXT(SP_GEOMETRY) AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = P_splitEntitySystemId and lower(entity_type)=lower(P_splitEntityType)
  ) AS point,
   LINE_MASTER AS LM  WHERE LM.system_id = P_microductId and lower(entity_type)=lower(P_entity_type)) AS geom) as geo1)as geo3;

END
$BODY$;

ALTER FUNCTION public.fn_get_split_microduct_details(integer, character varying, character varying, integer, character varying)
    OWNER TO postgres;
-----------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_microduct_count(integer)

-- DROP FUNCTION IF EXISTS public.fn_get_micromicroduct_count(integer);

CREATE OR REPLACE FUNCTION public.fn_get_microduct_count(
	p_system_id integer)
    RETURNS integer
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

declare 
microduct_count int;
 BEGIN
if (p_system_id!=0)then
select count(*) into microduct_count  from att_details_microduct dm where  dm.trench_id  =p_system_id;
else 
microduct_count=0;
end if;

return microduct_count;
END
$BODY$;

ALTER FUNCTION public.fn_get_microduct_count(integer)
    OWNER TO postgres;
-------------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_update_microduct_location(integer, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_update_microduct_location(integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_update_microduct_location(
	p_system_id integer,
	p_distance character varying,
	offsetdir character varying)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE 
    trenctId integer := 0;
    microductGeom text:='';
    microductGeomLength double precision:= 0;
    microductOffsetVal double precision:=0;
BEGIN
    SELECT trench_id INTO trenctId FROM att_details_microduct WHERE system_id = p_system_id;
    SELECT sp_geometry INTO microductGeom FROM line_master WHERE system_id = p_system_id AND entity_type = 'Microduct';
    select value into microductOffsetVal from global_settings where key='MicroductOffset';
   
    SELECT (microductOffsetVal/ST_Length(microductGeom::geography)) INTO microductGeomLength;
    
    raise notice 'microductGeomLength %' ,microductGeomLength;
    IF lower(offsetdir) = 'rightoffset' THEN
        UPDATE line_master
        SET sp_geometry = (
            SELECT ST_MakeLine(ARRAY[
                    st_startpoint(sp_geometry) ,
                    ST_OffsetCurve(
                         ST_LineSubstring(sp_geometry, microductGeomLength, 1 - microductGeomLength),                       
                    p_distance::double precision
                    ),
                    st_endpoint(sp_geometry)
                ]) 
            FROM line_master 
            WHERE system_id = p_system_id AND entity_type = 'Microduct'
        )
        WHERE system_id = p_system_id AND entity_type = 'Microduct';
    ELSE
        UPDATE line_master
        SET sp_geometry = (
            SELECT ST_MakeLine(ARRAY[
                    st_endpoint(sp_geometry) ,
                    ST_OffsetCurve(
                         ST_LineSubstring(sp_geometry, microductGeomLength, 1- microductGeomLength),                     
                        p_distance::double precision
                    ),
                    st_startpoint(sp_geometry)
                ]) 
            FROM line_master 
            WHERE system_id = p_system_id AND entity_type = 'Microduct'
        )
        WHERE system_id = p_system_id AND entity_type = 'Microduct';
    END IF;
END;

$BODY$;

ALTER FUNCTION public.fn_update_microduct_location(integer, character varying, character varying)
    OWNER TO postgres;

-----------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_update_microduct_color_code(integer, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_update_microduct_color_code(integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_update_microduct_color_code(
	p_system_id integer,
	trench_system_id integer,
	microductcount integer)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE
    p_microduct_count int;
begin
	if(trench_system_id!=0)  then
    p_microduct_count := (SELECT * FROM fn_get_microduct_count(trench_system_id));
   -- Update att_details_microduct table
    UPDATE att_details_microduct adt
    SET   
        color_code = CHR(64 + p_microduct_count)
    WHERE adt.system_id = p_system_id;
   else 
    UPDATE att_details_microduct adt
    SET   
       color_code = CHR(64 + microductCount)
    WHERE adt.system_id = p_system_id;
   end if;
END;
$BODY$;

ALTER FUNCTION public.fn_update_microduct_color_code(integer, integer, integer)
    OWNER TO postgres;

-----------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_trg_audit_att_details_microduct()

-- DROP FUNCTION IF EXISTS public.fn_trg_audit_att_details_microduct();

CREATE OR REPLACE FUNCTION public.fn_trg_audit_att_details_microduct()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$

DECLARE
_value integer;
 _ignorecol character varying;  
 _ignorecol1 character varying;  
 v_no_of_ways character varying;
 v_sum_no_of_ways integer;
v_parent_system_id integer;
v_no_of_cables integer;
tempjson text; --for additional attributes
sql text;  --for additional attributes

BEGIN

IF (TG_OP = 'INSERT' ) THEN  
         INSERT INTO public.audit_att_details_microduct(
            system_id, network_id, microduct_name, calculated_length, 
            manual_length, a_system_id, a_location, a_entity_type, b_system_id, 
            b_location, b_entity_type, sequence_id, network_status, status, 
            pin_code, province_id, region_id, utilization, no_of_cables, 
            offset_value, construction, activation, accessibility, specification, 
            category, subcategory1, subcategory2, subcategory3, item_code, 
            vendor_id, type, brand, model, remarks, created_by, created_on, 
            modified_by, modified_on, trench_id, project_id, planning_id, 
            purpose_id, workorder_id,inner_dimension,outer_dimension,microduct_type,color_code, action,parent_system_id,parent_network_id,parent_entity_type,barcode,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,no_of_ways,internal_diameter,external_diameter,material_type,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id)
         select new.system_id, new.network_id, new.microduct_name, new.calculated_length, 
            new.manual_length, new.a_system_id, new.a_location, new.a_entity_type, new.b_system_id, 
            new.b_location, new.b_entity_type, new.sequence_id, new.network_status, new.status, 
            new.pin_code, new.province_id, new.region_id, new.utilization, new.no_of_cables, 
            new.offset_value, new.construction, new.activation, new.accessibility, new.specification, 
            new.category, new.subcategory1, new.subcategory2, new.subcategory3, new.item_code, 
            new.vendor_id, new.type, new.brand, new.model, new.remarks, new.created_by, new.created_on, 
            new.modified_by, new.modified_on, new.trench_id, new.project_id, new.planning_id, 
            new.purpose_id, new.workorder_id,new.inner_dimension,new.outer_dimension,new.microduct_type,new.color_code, 'I' as action ,new.parent_system_id,new.parent_network_id,new.parent_entity_type,new.barcode,new.acquire_from,new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,new.status_updated_by,new.status_updated_on,new.no_of_ways,new.internal_diameter,new.external_diameter,new.material_type,new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category ,new.gis_design_id from att_details_microduct where system_id=new.system_id;

	--raise info 'new.system_id %',new.system_id; 
          tempjson:='''"'||new.system_id||'"''';
         --raise info 'tempjson %',tempjson; 
   
	--Update value of the key 'record_system_id' for additional attributes    
   	sql:='UPDATE att_details_microduct SET other_info = jsonb_set(other_info::jsonb , ''{record_system_id}'','||tempjson||') where system_id = '||new.system_id||';';
   
  	 --raise info 'sql %',sql; 

END IF;
IF (TG_OP = 'UPDATE' ) THEN  
_ignorecol := 'modified_on';
_ignorecol1 := 'modified_by';
select fn_check_history_record(OLD, NEW,_ignorecol,_ignorecol1) into _value;
if(_value = 1)then

              INSERT INTO public.audit_att_details_microduct(
            system_id, network_id, microduct_name, calculated_length, 
            manual_length, a_system_id, a_location, a_entity_type, b_system_id, 
            b_location, b_entity_type, sequence_id, network_status, status, 
            pin_code, province_id, region_id, utilization, no_of_cables, 
            offset_value, construction, activation, accessibility, specification, 
            category, subcategory1, subcategory2, subcategory3, item_code, 
            vendor_id, type, brand, model, remarks, created_by, created_on, 
            modified_by, modified_on, trench_id, project_id, planning_id, 
            purpose_id, workorder_id,inner_dimension,outer_dimension,microduct_type,color_code, action,parent_system_id,parent_network_id,parent_entity_type,barcode,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,no_of_ways,internal_diameter,external_diameter,material_type,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id)
           select new.system_id, new.network_id, new.microduct_name, new.calculated_length, 
            new.manual_length, new.a_system_id, new.a_location, new.a_entity_type, new.b_system_id, 
            new.b_location, new.b_entity_type, new.sequence_id, new.network_status, new.status, 
            new.pin_code, new.province_id, new.region_id, new.utilization, new.no_of_cables, 
            new.offset_value, new.construction, new.activation, new.accessibility, new.specification, 
            new.category, new.subcategory1, new.subcategory2, new.subcategory3, new.item_code, 
            new.vendor_id, new.type, new.brand, new.model, new.remarks, new.created_by, new.created_on, 
            new.modified_by, new.modified_on, new.trench_id, new.project_id, new.planning_id, 
            new.purpose_id, new.workorder_id,new.inner_dimension,new.outer_dimension,new.microduct_type,new.color_code, 'U' as action,new.parent_system_id,new.parent_network_id,new.parent_entity_type,new.barcode,new.acquire_from,new.ownership_type,new.third_party_vendor_id,new.source_ref_type,new.source_ref_id,new.source_ref_description,new.audit_item_master_id,new.status_remark,new.status_updated_by,new.status_updated_on,new.no_of_ways,new.internal_diameter,new.external_diameter,new.material_type,new.is_acquire_from,new.other_info,new.origin_from,new.origin_ref_id,new.origin_ref_code,new.origin_ref_description,new.request_ref_id,new.requested_by,new.request_approved_by,new.subarea_id,new.area_id,new.dsa_id,new.csa_id,new.bom_sub_category ,new.gis_design_id from att_details_microduct where system_id=new.system_id;

             select sum((RTRIM(coalesce(no_of_ways,'1'), ' Way')::integer)), sum(no_of_cables) into v_sum_no_of_ways,v_no_of_cables from att_details_microduct where parent_system_id=new.parent_system_id;          
	     update att_details_duct set duct_capacity= v_sum_no_of_ways  where system_id=new.parent_system_id; 
 

END IF; 				
END IF; 				
IF (TG_OP = 'DELETE' ) THEN 

         INSERT INTO public.audit_att_details_microduct(
            system_id, network_id, microduct_name, calculated_length, 
            manual_length, a_system_id, a_location, a_entity_type, b_system_id, 
            b_location, b_entity_type, sequence_id, network_status, status, 
            pin_code, province_id, region_id, utilization, no_of_cables, 
            offset_value, construction, activation, accessibility, specification, 
            category, subcategory1, subcategory2, subcategory3, item_code, 
            vendor_id, type, brand, model, remarks, created_by, created_on, 
            modified_by, modified_on, trench_id, project_id, planning_id, 
            purpose_id, workorder_id,inner_dimension,outer_dimension,microduct_type,color_code, action,parent_system_id,parent_network_id,parent_entity_type,barcode,acquire_from,ownership_type,third_party_vendor_id,source_ref_type,source_ref_id,source_ref_description,audit_item_master_id,status_remark,status_updated_by,status_updated_on,no_of_ways,internal_diameter,external_diameter,material_type,is_acquire_from,other_info,origin_from,origin_ref_id,origin_ref_code,origin_ref_description,request_ref_id,requested_by,request_approved_by,subarea_id,area_id,dsa_id,csa_id,bom_sub_category ,gis_design_id)
         values(old.system_id, old.network_id, old.microduct_name, old.calculated_length, 
            old.manual_length, old.a_system_id, old.a_location, old.a_entity_type, old.b_system_id, 
            old.b_location, old.b_entity_type, old.sequence_id, old.network_status, old.status, 
            old.pin_code, old.province_id, old.region_id, old.utilization, old.no_of_cables, 
            old.offset_value, old.construction, old.activation, old.accessibility, old.specification, 
            old.category, old.subcategory1, old.subcategory2, old.subcategory3, old.item_code, 
            old.vendor_id, old.type, old.brand, old.model, old.remarks, old.created_by, old.created_on, 
            old.modified_by, old.modified_on, old.trench_id, old.project_id, old.planning_id, 
            old.purpose_id, old.workorder_id,old.inner_dimension,old.outer_dimension,old.microduct_type,old.color_code, 'D',old.parent_system_id,old.parent_network_id,old.parent_entity_type,old.barcode,old.acquire_from,old.ownership_type,old.third_party_vendor_id,old.source_ref_type,old.source_ref_id,old.source_ref_description,old.audit_item_master_id,old.status_remark,old.status_updated_by,old.status_updated_on,old.no_of_ways,old.internal_diameter,old.external_diameter,old.material_type,old.is_acquire_from,old.other_info,old.origin_from,old.origin_ref_id,old.origin_ref_code,old.origin_ref_description,old.request_ref_id,old.requested_by,old.request_approved_by,old.subarea_id,old.area_id,old.dsa_id,old.csa_id,old.bom_sub_category ,old.gis_design_id);    

	   select sum((RTRIM(coalesce(no_of_ways,'1'), ' Way')::integer)), sum(no_of_cables) into v_sum_no_of_ways,v_no_of_cables from att_details_microduct where parent_system_id=old.parent_system_id;
	     update att_details_duct set duct_capacity= v_sum_no_of_ways  where system_id=old.parent_system_id; 
	  

          update att_details_trench set no_of_ducts=case when (no_of_ducts::integer)>0 then(no_of_ducts::integer-1)else 0 end 
	  where system_id=(select trench_id from att_details_microduct where system_id=old.SYSTEM_ID);
          Delete from line_master where system_id=old.SYSTEM_ID and UPPER(entity_type)='MICRODUCT';
	
          

END IF; 		

RETURN NEW;
END;

$BODY$;

ALTER FUNCTION public.fn_trg_audit_att_details_microduct()
    OWNER TO postgres;

--------------------------------------------------------------------------------------------------------------------------------------------------------
INSERT INTO public.legend_details(
	 layer_id, group_name, sub_layer, icon_path, created_by, created_on,  type, layer_sub_column,  is_active, sequence_id)
	VALUES ((select layer_id from layer_details where layer_name='Microduct'), 
			'Microduct','As-Built', 'Microduct.svg', 1,now(), 'Web', 'Microduct',   true, 8);
			
	INSERT INTO public.legend_details(
	 layer_id, group_name, sub_layer, icon_path, created_by, created_on,  type, layer_sub_column,  is_active, sequence_id)
	VALUES ((select layer_id from layer_details where layer_name='Microduct'), 
			'Microduct','Dormant', 'Microduct.svg', 1,now(), 'Web', 'Microduct',   true, 8);
			
update legend_details set group_name='Microduct',sub_layer='Planned'
where layer_id in(select layer_id from layer_details where layer_name='Microduct');
----------------------------------------------------------------------------------------------------------------------------------------------------------------------