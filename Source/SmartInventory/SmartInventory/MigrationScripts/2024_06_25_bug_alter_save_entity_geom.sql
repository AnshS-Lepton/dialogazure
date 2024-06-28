

CREATE OR REPLACE FUNCTION public.fn_save_entity_geom(
	p_system_id integer,
	p_geom_type character varying,
	p_entity_type character varying,
	p_userid integer,
	p_longlat character varying,
	p_common_name character varying,
	p_network_status character varying,
	p_ports character varying,
	p_entity_category character varying,
	p_center_line_geom character varying,
	p_buffer_width double precision,
	p_project_id integer)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE geo_data text;
s_longLat character varying;
sql text;
s_long text;
s_lat text;
v_radious double precision;
v_close_point character varying;
v_centroid geometry;
v_center_line_geom character varying;
v_display_name character varying;
v_display_value character varying;
v_layer_table character varying;
v_source_ref_id character varying;
v_source_ref_type character varying;
v_status character varying;

BEGIN


v_radious:=0;

select layer_table into v_layer_table from layer_details where Upper(layer_name) = upper(p_entity_type) ;
raise info 'check layertable:  %',v_layer_table; 

EXECUTE FORMAT('SELECT source_ref_id, source_ref_type, status 
                    FROM %I 
                    WHERE system_id = $1', v_layer_table)
    INTO v_source_ref_id, v_source_ref_type, v_status
    USING p_system_id;

if lower(p_geom_type)='point' then
s_longLat:='POINT('||p_longlat||')';
geo_data := 'ST_GeomFromText('''||s_longLat||''',4326)';
s_long:= SPLIT_PART(''||p_longlat||'', ' ',1);
s_lat:= SPLIT_PART(''||p_longlat||'', ' ',2);

sql:='insert into point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,modified_on,network_status,no_of_ports,entity_category,display_name,project_id,status,source_ref_id,source_ref_type)
values('||p_system_id||','''||p_entity_type||''','''||s_long||''','''||s_lat||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),'''||p_network_status||''','''||p_ports||''','''||p_entity_category||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';

end if;

if lower(p_geom_type)='line' then
s_longLat := 'LINESTRING('||p_longlat||')';
geo_data := 'ST_GeomFromText('''|| s_longLat||''',4326)';
sql:='INSERT INTO line_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,display_name,project_id,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';
end if;
if lower(p_geom_type)='polygon' then
s_longLat := 'POLYGON(('||p_longlat||'))';
geo_data := 'ST_GeomFromText(''' ||s_longLat ||''',4326)';
if(upper(p_entity_type)=upper('ROW'))
then
sql:='INSERT INTO polygon_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,center_line_geom,buffer_width,display_name,project_id,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',ST_GeomFromText(''LINESTRING('||p_center_line_geom||')'',4326),'||p_buffer_width||',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''') ';
else

sql:='INSERT INTO polygon_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,display_name,project_id,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||p_project_id||''','''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''') ';
end if;

end if;

if lower(p_geom_type)='circle' then
s_longLat := 'POLYGON(('||p_longlat||'))';
geo_data := 'ST_GeomFromText(''' ||s_longLat ||''',4326)';

select Cast(st_distance(ST_centroid('POLYGON(('||p_longlat||'))'),(ST_DumpPoints('POLYGON(('||p_longlat||'))')).geom,false)as decimal(10,2)) into v_radious limit 1;

select center_line_geom into v_center_line_geom
from polygon_master pm inner join att_details_row_pit pit on pm.system_id=pit.parent_system_id and upper(pm.entity_type)=upper('ROW') where pit.system_id=p_system_id;

v_close_point:='ST_ClosestPoint('''||v_center_line_geom||''', ST_centroid('||geo_data||')) ';

geo_data:='ST_buffer_meters('||v_close_point||','||coalesce(v_radious,1)||')';
sql:='INSERT INTO circle_master(system_id, entity_type, approval_flag, sp_geometry, creator_remark, approver_remark, created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,sp_center,radious,display_name,status,source_ref_id,source_ref_type)
VALUES ('||p_system_id||','''||p_entity_type||''',''A'','||geo_data||',''NA'',''NA'','||p_userid||',0,'''||p_common_name||''',0,now(),now(),'''||p_network_status||''','||v_close_point||','||coalesce(v_radious,0)||',fn_get_display_name('||p_system_id||','''||p_entity_type||'''),'''||v_status||''','''||v_source_ref_id||''','''||v_source_ref_type||''')';

end if;
 -- raise info 'SQL %',SQL;
IF UPPER(P_GEOM_TYPE)='POINT' AND NOT EXISTS(SELECT 1 FROM POINT_MASTER PM
INNER JOIN LAYER_DETAILS LD ON UPPER(PM.COMMON_NAME)=UPPER(P_COMMON_NAME) AND UPPER(PM.ENTITY_TYPE)=UPPER(LD.LAYER_NAME) 
AND IS_MiDDLEWARE_ENTITY=TRUE)
THEN
EXECUTE SQL;
ELSIF UPPER(P_GEOM_TYPE)!='POINT'
THEN
EXECUTE SQL;
END IF;
END
$BODY$;