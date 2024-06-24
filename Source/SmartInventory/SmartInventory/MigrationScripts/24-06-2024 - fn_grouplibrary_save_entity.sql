-- FUNCTION: public.fn_grouplibrary_save_entity01(character varying)

-- DROP FUNCTION IF EXISTS public.fn_grouplibrary_save_entity(character varying);

CREATE OR REPLACE FUNCTION public.fn_grouplibrary_save_entity(
	p_properties character varying)
    RETURNS TABLE(status boolean, networkid character varying, entity_type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE 
new_status boolean;
message character varying;
v_entity_type character varying;
v_system_id integer;
v_created_by integer;
v_parent_system_id integer;
v_parent_network_id character varying;
v_parent_entity_type character varying;
v_parent_geom_type character varying;
v_sequence_id integer;
v_allcolumns text;
query text;
v_allcolumnrows text;
currentsystem_id integer;
v_arow record;
v_layer_table character varying;
new_network_id character varying;
v_province_id integer;
v_region_id integer;
v_entityname character varying;
v_library_id integer;
v_gtype character varying;
v_total_core integer;
v_no_of_tube integer;
v_entity_details record;
v_le_details record;
v_user_id integer;
BEGIN
new_status=false;
create temp table temp_group_library
(
network_id character varying,
entity_type character varying
)ON COMMIT DROP ;
create temp table temp_entity_details
(
library_id integer,
line_id integer,
user_id integer,
system_id integer,
network_id character varying,
network_name character varying,
termination_type character varying,
node_type character varying,
display_name character varying,
type character varying,
geom character varying,
feature_type character varying
)ON COMMIT DROP ;

insert into temp_entity_details(library_id,line_id,user_id ,system_id,network_id,network_name,termination_type,node_type,display_name,type,geom,feature_type)
select library_id,line_id,user_id ,system_id,network_id,network_name,termination_type,node_type,display_name,type,geom,feature_type
from json_populate_recordset(null::temp_entity_details,replace(p_properties,'\','')::json);

IF EXISTS(select upper(type) from temp_entity_details where upper(type)=upper('Linestring') limit 1)
then
FOR V_AROW IN select line_id,count(line_id) from temp_entity_details  where line_id!=0 group by line_id
LOOP
raise info 'V_AROW.count%',V_AROW.count;
if(V_AROW.count=1)
Then

select library_id,line_id,user_id,type,geom,''::character varying as a_location,''::character varying as b_location,0 ::integer as a_system_id,
0::integer as b_system_id,''::character varying as a_entity_type, ''::character varying as b_entity_type
 from temp_entity_details where line_id=V_AROW.line_id into v_le_details;
ElSIF(V_AROW.count=3)
THEN
 select tle2.library_id,tle.user_id,tle.system_id as a_system_id,tle.network_id as a_location,tle.network_name as a_entity_type,
 tle1.system_id as b_system_id,tle1.network_id as b_location,tle1.network_name as b_entity_type,tle2.type,tle2.geom
 from temp_entity_details tle inner join temp_entity_details tle1 on tle.line_id=tle1.line_id
 inner join temp_entity_details tle2 on tle.line_id=tle2.line_id where tle.node_type='start' and tle1.node_type='end' and tle2.library_id!=0
and tle1.line_id!=0 and tle2.line_id!=0 and tle.line_id!=0 and tle.line_id=V_AROW.line_id into v_le_details;
End IF;
select egl.entity_type,egl.system_id,egl.created_by from entity_group_library egl where id=v_le_details.library_id into v_entity_details;

select layer_table into v_layer_table from layer_details where lower(layer_name)=lower(v_entity_details.entity_type);
select string_agg(distinct jkey, ', ')
from entity_group_library, json_object_keys(entity_data::json) as t(jkey) where id=v_le_details.library_id INTO v_allcolumns;

select string_agg(distinct format('(entity_data::json ->> %L)::'||sch.data_type||' as %I',jkey, jkey), ', ')
from entity_group_library, json_object_keys(entity_data::json) as t(jkey)
inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper(v_layer_table)
where id=v_le_details.library_id INTO v_allcolumnrows;

-- GET New Network-Id....

select user_id from temp_entity_details limit 1 into v_user_id;

SELECT clone.parent_system_id, clone.parent_network_id, clone.parent_entity_type , clone.parent_geom_type, clone.network_code, clone.sequence_id into
v_parent_system_id, v_parent_network_id, v_parent_entity_type , v_parent_geom_type, new_network_id, v_sequence_id
FROM fn_get_line_network_code(v_entity_details.entity_type,'','',v_le_details.geom,'OSP',0,'') as clone limit 1;

select id into v_province_id from province_boundary province where ST_Within(St_Geomfromtext(v_le_details.type||'('||v_le_details.geom||')',4326),province.sp_geometry);
select id into v_region_id from region_boundary region where ST_Within(St_Geomfromtext(v_le_details.type||'('||v_le_details.geom||')',4326),region.sp_geometry);
select lower(v_entity_details.entity_type||'_name') into v_entityname;

raise info 'v_allcolumnrows :%',v_allcolumnrows;

query='INSERT INTO '||v_layer_table||' ('||v_allcolumns||',network_id,created_by,parent_system_id,parent_network_id,parent_entity_type,sequence_id,
'||v_entityname||''||',region_id,province_id,a_location,a_system_id,a_entity_type,b_location,b_system_id,b_entity_type)
select '||v_allcolumnrows||', '''||new_network_id||''', '||v_user_id||','||v_parent_system_id||' ,
'''||v_parent_network_id||''', '''||v_parent_entity_type||''', '||v_sequence_id||','''||new_network_id||''','||v_region_id||','||v_province_id||'
,'''||v_le_details.a_location||''','||v_le_details.a_system_id||','''||v_le_details.a_entity_type||''','''||v_le_details.b_location||''','||v_le_details.b_system_id||',
'''||v_le_details.b_entity_type||'''
from entity_group_library where id = '||v_le_details.library_id||' RETURNING system_id';
EXECUTE QUERY into currentsystem_id ;
INSERT INTO line_master(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,common_name,approval_date,
network_status,display_name)
values(currentsystem_id, v_entity_details.entity_type,'A',ST_GEOMFROMTEXT(v_le_details.type||'('||v_le_details.geom||')',4326),'Created',
'' ,v_user_id,new_network_id,
now(),'P',fn_get_display_name(currentsystem_id,v_entity_details.entity_type));

perform (fn_geojson_update_entity_attribute(currentsystem_id,v_entity_details.entity_type::character varying ,v_province_id::integer,0,false));

IF(upper(v_entity_details.entity_type)=upper('Cable'))
Then
select total_core,no_of_tube into v_total_core,v_no_of_tube from att_details_cable where system_id=currentsystem_id;
perform(fn_set_cable_color_info(currentsystem_id, v_entity_details.created_by,v_total_core, v_no_of_tube));
END IF;

Insert into temp_group_library (network_id,entity_type)
values(new_network_id,v_entity_details.entity_type);

END LOOP;
new_status=true;
ELSIF((select upper(type) from temp_entity_details limit 1) = upper('Point'))
THEN
select library_id,type into V_LIBRARY_ID,V_GTYPE from temp_entity_details limit 1;

PERFORM(fn_grouplibrary_save_point_entity01(V_LIBRARY_ID,V_GTYPE));
NEW_STATUS=TRUE;

END IF;

RETURN QUERY
select new_status as status,tgl.network_id as networkid ,tgl.entity_type from temp_group_library tgl ;
END
$BODY$;

ALTER FUNCTION public.fn_grouplibrary_save_entity(character varying)
    OWNER TO postgres;
