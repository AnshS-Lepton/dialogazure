DROP FUNCTION IF EXISTS public.fn_get_split_cable_details(integer, character varying, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_split_cable_details(
	p_splitentitysystemid integer,
	p_splitentitytype character varying,
	p_cableid integer,
	p_entity_type character varying)
    RETURNS TABLE(system_id integer, common_name character varying, network_status character varying, geom_cable1 text, geom_cable2 text, cable1_a_system_id integer, cable1_a_entity_type character varying, cable1_a_location character varying, cable1_b_system_id integer, cable1_b_entity_type character varying, cable2_a_system_id integer, cable2_a_entity_type character varying, cable2_b_system_id integer, cable2_b_entity_type character varying, cable2_b_location character varying,   cable1length double precision, cable2length double precision,   cable1_name character varying,cable2_name character varying) 
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

select p.sp_geometry,a_system_id,a_entity_type,a_location into v_a_end_geometry,v_a_system_id,v_a_entity_type,v_a_location from point_master p join att_details_cable c on p.system_id=c.a_system_id and upper(p.entity_type)=upper(c.a_entity_type) where c.system_id=p_cableid;
select p.sp_geometry,b_system_id,b_entity_type,b_location into v_b_end_geometry,v_b_system_id,v_b_entity_type,v_b_location from point_master p join att_details_cable c on p.system_id=c.b_system_id and upper(p.entity_type)=upper(c.b_entity_type) where c.system_id=p_cableid;

    RETURN QUERY

select geo3.SYSTEM_ID,geo3.COMMON_NAME,geo3.network_status,geo3.CABLE1,geo3.CABLE2, 
case when geo3.cable1_start_matched_with_a then v_a_system_id 
when geo3.cable1_start_matched_with_b then v_b_system_id  end as cable1_a_system_id, 
case when geo3.cable1_start_matched_with_a then v_a_entity_type 
when geo3.cable1_start_matched_with_b then v_b_entity_type end as cable1_a_entity_type,
case when geo3.cable1_start_matched_with_a then v_a_location 
when geo3.cable1_start_matched_with_b then v_b_location end as cable1_a_location,

p_splitentitysystemid as cable1_b_system_id,
p_splitentitytype as cable1_b_entity_type,

p_splitentitysystemid as cable2_a_system_id,
p_splitentitytype as cable2_a_entity_type,
case when geo3.cable2_end_matched_with_a then v_a_system_id  
when geo3.cable2_end_matched_with_b then v_b_system_id  end as cable2_b_system_id, 
case when geo3.cable2_end_matched_with_a then v_a_entity_type 
when geo3.cable2_end_matched_with_b then v_b_entity_type end as cable2_b_entity_type,
case when geo3.cable2_end_matched_with_a then v_a_location 
when geo3.cable2_end_matched_with_b then v_b_location end as cable2_b_location,
(select * from getgeometrylength(''|| geo3.CABLE1||''))as cable1Length ,
(select * from getgeometrylength(''|| geo3.CABLE2||''))as cable2Length,

fn_get_cable_name(p_splitentitysystemid,p_splitentitytype,case when geo3.cable1_start_matched_with_a then v_a_location 
when geo3.cable1_start_matched_with_b then v_b_location end) as cable1_name,
fn_get_cable_name(p_splitentitysystemid,p_splitentitytype,case when geo3.cable2_end_matched_with_a then v_a_location 
when geo3.cable2_end_matched_with_b then v_b_location end) as cable2_name
from (
select 	 geo1.SYSTEM_ID,geo1.COMMON_NAME,geo1.network_status,REPLACE(REPLACE(geo1.CABLE1, 'LINESTRING(', ''),')','')as CABLE1,REPLACE(REPLACE (geo1.CABLE2, 'LINESTRING(', ''),')','')as CABLE2,
 case when st_startpoint(st_geomfromtext(geo1.CABLE1,4326))=v_a_end_geometry then true else false end as cable1_start_matched_with_a,
 case when st_startpoint(st_geomfromtext(geo1.CABLE1,4326))=v_b_end_geometry then true else false end as cable1_start_matched_with_b,

 case when st_endpoint(st_geomfromtext(geo1.CABLE2,4326))=v_a_end_geometry then true else false end  as cable2_end_matched_with_a,
 case when st_endpoint(st_geomfromtext(geo1.CABLE2,4326))=v_b_end_geometry then true else false end  as cable2_end_matched_with_b

 from (
select geom.SYSTEM_ID,geom.COMMON_NAME,geom.network_status, 
ST_ASTEXT(ST_LINESUBSTRING(ST_GEOMFROMTEXT(ST_ASTEXT(EXISTING_GEOMETRY)),0,geom.INTERPOLATE_POINT)) AS CABLE1,
ST_ASTEXT(ST_LINESUBSTRING(ST_GEOMFROMTEXT(ST_ASTEXT(EXISTING_GEOMETRY)), geom.INTERPOLATE_POINT, 1)) AS CABLE2,
ST_LINESUBSTRING(ST_GEOMFROMTEXT(ST_ASTEXT(EXISTING_GEOMETRY)),0,geom.INTERPOLATE_POINT) as cable1_geom,
ST_LINESUBSTRING(ST_GEOMFROMTEXT(ST_ASTEXT(EXISTING_GEOMETRY)), geom.INTERPOLATE_POINT, 1) as cable2_geom

FROM (
SELECT LM.SYSTEM_ID, 
 LM.COMMON_NAME,
  ST_LINELOCATEPOINT(ST_ASTEXT(LM.SP_GEOMETRY), point.THE_POINT) AS INTERPOLATE_POINT,LM.SP_GEOMETRY as EXISTING_GEOMETRY ,LM.network_status 
  FROM (
 SELECT ST_ASTEXT(SP_GEOMETRY) AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = P_splitEntitySystemId and lower(entity_type)=lower(P_splitEntityType)
  ) AS point,
   LINE_MASTER AS LM  WHERE LM.system_id = P_cableId and lower(entity_type)=lower(P_entity_type)) AS geom) as geo1)as geo3;

END
$BODY$;




CREATE OR REPLACE FUNCTION public.fn_get_cable_name(
	p_splitentitysystemid integer,
	p_splitentitytype character varying,
	p_cable_point character varying )
     RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE

AS $BODY$
Declare end_point_name character varying;
Declare att_table_name character varying;
Declare entity_name character varying;
sql_query TEXT;

BEGIN

att_table_name := 'att_details_' || p_splitentitytype;


select common_name from point_master where entity_type = p_splitentitytype and system_id = p_splitentitysystemid into end_point_name;

sql_query := 'SELECT ' || p_splitentitytype || '_name ' || ' FROM ' || att_table_name || ' WHERE system_id = $1';
EXECUTE sql_query INTO entity_name USING p_splitentitysystemid;

if(coalesce(p_cable_point,'')!='' )
Then 
end_point_name := p_cable_point || '-' || end_point_name;
else
end_point_name := entity_name;
End if;

Return end_point_name;

END
$BODY$;