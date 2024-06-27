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
feature_type character varying,
source_ref_id character varying,
source_ref_type character varying
)ON COMMIT DROP ;

insert into temp_entity_details(library_id,line_id,user_id ,system_id,network_id,network_name,termination_type,node_type,display_name,type,geom,feature_type,source_ref_id,source_ref_type)
select library_id,line_id,user_id ,system_id,network_id,network_name,termination_type,node_type,display_name,type,geom,feature_type,source_ref_id,source_ref_type
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

PERFORM(FN_GROUPLIBRARY_SAVE_POINT_ENTITY(V_LIBRARY_ID,V_GTYPE));
NEW_STATUS=TRUE;

END IF;

RETURN QUERY
select new_status as status,tgl.network_id as networkid ,tgl.entity_type from temp_group_library tgl ;
END
$BODY$;

ALTER FUNCTION public.fn_grouplibrary_save_entity(character varying)
    OWNER TO postgres;
	

CREATE OR REPLACE FUNCTION public.fn_grouplibrary_save_point_entity(
	p_id integer,
	p_gtype character varying)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$



DECLARE
new_network_id character varying;
new_status boolean;
--RETURN VALUES
s_p_system_id integer;
s_p_network_id character varying;
s_p_entity_type character varying;
s_p_geom_type character varying;
s_sequence_id integer;
v_latitude double precision;
v_longitude double precision;
v_layer_table character varying;
v_allcolumns character varying;
v_allcolumnrows character varying;
v_parent_system_id integer;
v_parent_entity_type character varying;
v_entity_type character varying;
V_INPUT_PORT integer;
V_OUTPUT_PORT integer;
currentsystem_id integer;
v_created_by integer;
v_is_accessories_required boolean;
V_ALLACCESSCOLUMNS character varying;
V_ALLACCESSCOLUMNROW character varying;
curgeommapping refcursor;
v_entityname character varying;
query character varying;
V_STATUS RECORD;
v_arow record;
v_province_id integer;
v_region_id integer;
V_NO_OF_PORT integer;
V_ACCESS json;
tray_status boolean;
tray_network_id character varying;
tray_p_system_id integer;
tray_p_network_id character varying;
tray_p_entity_type character varying;
tray_p_geom_type character varying;
tray_sequence_id integer;
V_TRAY_INFO json;
V_TRAYHEADER character varying;
V_TRAYCOLUMNS character varying;
v_tray record;
v_user_id integer;
v_source_ref_id character varying;
v_source_ref_type character varying;
BEGIN
query='';
V_ALLACCESSCOLUMNS='';
V_ALLACCESSCOLUMNROW='';
currentsystem_id:=0;
V_INPUT_PORT=0;
V_OUTPUT_PORT=0;
v_province_id:=0;
v_region_id:=0;
select source_ref_id,upper(source_ref_type) into v_source_ref_id,v_source_ref_type from temp_entity_details limit 1;
IF (LOWER(p_gtype)='point')

THEN
select is_accessories_required ,entity_type,created_by from entity_group_library where id=p_id into v_is_accessories_required ,v_entity_type,v_created_by;

select layer_table into v_layer_table from layer_details where lower(layer_name)=lower(v_entity_type);

select string_agg(distinct jkey, ', ')
from entity_group_library, json_object_keys(entity_data::json) as t(jkey) where id=p_id INTO v_allcolumns;

select string_agg(distinct format('(entity_data::json ->> %L)::'||Replace(sch.data_type,'USER-DEFINED','geometry')||' as %I',jkey, jkey), ', ')
from entity_group_library, json_object_keys(entity_data::json) as t(jkey)
inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper(v_layer_table)
where id=p_id INTO v_allcolumnrows;

select user_id from temp_entity_details limit 1 into v_user_id;

OPEN curgeommapping FOR select split_part(geom, ' ', 1)::double precision as latitude,split_part(geom, ' ', 2)::double precision as longitude from TEMP_ENTITY_DETAILS where line_id=0;
LOOP
FETCH curgeommapping into v_longitude,v_latitude;
IF NOT FOUND
THEN
EXIT;
END IF;
-- GET New Network-Id....
SELECT clone.status, clone.message, clone.o_p_system_id , clone.o_p_network_id, clone.o_p_entity_type, clone.o_sequence_id into
new_status, new_network_id, s_p_system_id , s_p_network_id, s_p_entity_type, s_sequence_id
FROM fn_get_clone_network_code(v_entity_type,p_gtype, v_longitude||' '||v_latitude::character varying,0,'') as clone limit 1;

select id into v_province_id from province_boundary province where ST_Within(St_Geomfromtext('POINT('||v_longitude||' '||v_latitude||')',4326),province.sp_geometry);
select id into v_region_id from region_boundary region where ST_Within(St_Geomfromtext('POINT('||v_longitude||' '||v_latitude||')',4326),region.sp_geometry);

select lower(v_entity_type||'_name') into v_entityname;

IF (new_status=true)
THEN
raise info '%','new_status'||new_status;
IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name=v_layer_table AND column_name in ('latitude','longitude'))
THEN
--raise info '%','new_status'||1++;
query='INSERT INTO '||v_layer_table||' ('||v_allcolumns||',network_id,latitude,longitude,created_by,parent_system_id,parent_network_id,parent_entity_type,sequence_id,'||v_entityname||''||',region_id,province_id)
select '||v_allcolumnrows||', '''||new_network_id||''', '||v_latitude||', '||v_longitude||', '||v_user_id||','||s_p_system_id||' , '''||s_p_network_id||''', '''||s_p_entity_type||''', '||s_sequence_id||','''||new_network_id||''','||v_region_id||','||v_province_id||' from entity_group_library where id = '||p_id||' RETURNING system_id';


ELSE
query='INSERT INTO '||v_layer_table||' ('||v_allcolumns||',network_id,created_by,parent_system_id,parent_network_id,parent_entity_type,sequence_id,'||v_entityname||''||',region_id,province_id)
select '||v_allcolumnrows||', '''||new_network_id||''','||v_user_id||','||s_p_system_id||' , '''||s_p_network_id||''', '''||s_p_entity_type||''', '||s_sequence_id||','''||new_network_id||''' ,'||v_region_id||','||v_province_id||' from entity_group_library where id = '||p_id||' RETURNING system_id';
END IF;
raise info '%','Rkquery'||query;
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE UPPER(TABLE_NAME)=UPPER(v_layer_table) AND UPPER(COLUMN_NAME)
IN ('NO_OF_INPUT_PORT','SPLITTER_RATIO','NO_OF_OUTPUT_PORT','NO_OF_PORT'))
THEN
IF(UPPER(v_entity_type)='SPLITTER')
THEN
QUERY=QUERY||' ,SPLIT_PART(SPLITTER_RATIO,'':'',1)::INTEGER,SPLIT_PART(SPLITTER_RATIO,'':'',2)::INTEGER,0';
ELSE
QUERY=QUERY||' ,NO_OF_INPUT_PORT,NO_OF_OUTPUT_PORT,NO_OF_PORT';
END IF;
EXECUTE QUERY INTO currentsystem_id,V_INPUT_PORT,V_OUTPUT_PORT,V_NO_OF_PORT;
ELSE
EXECUTE QUERY INTO currentsystem_id;
END IF;

EXECUTE 'UPDATE '||v_layer_table||' set is_new_entity=true,status=''D'',source_ref_id='''||(select source_ref_id from temp_entity_details limit 1)||''',source_ref_type='''||(select upper(source_ref_type) from temp_entity_details limit 1)||''' where system_id= '||currentsystem_id||'';

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,common_name, network_status,display_name,NO_OF_PORTS,status,source_ref_id,source_ref_type)
values(currentsystem_id, v_entity_type,v_latitude::double precision, v_longitude::double precision,
'A',ST_GEOMFROMTEXT('POINT('||v_longitude::character varying||' '||v_latitude::character varying||')',4326),now(),'Created', '' ,v_user_id,new_network_id, 'P',
fn_get_display_name(currentsystem_id,v_entity_type),
(case when (UPPER(v_entity_type)='SPLITTER' or UPPER(v_entity_type)='ONT') and coalesce(V_INPUT_PORT,0)>0
then (V_INPUT_PORT||':'||V_OUTPUT_PORT)::text
when coalesce(V_INPUT_PORT,0)>0 and coalesce(V_OUTPUT_PORT,0)>0 then V_INPUT_PORT::text
when coalesce(V_NO_OF_PORT,0)>0 then V_NO_OF_PORT::text else '' end),'D',(select source_ref_id from temp_entity_details limit 1),(select upper(source_ref_type) from temp_entity_details limit 1)) ;

perform (fn_geojson_update_entity_attribute(currentsystem_id,v_entity_type::character varying ,v_province_id::integer,0,false));

 Insert into temp_group_library (network_id,entity_type)
 values(new_network_id,v_entity_type);

IF EXISTS(SELECT 1 FROM LAYER_DETAILS WHERE UPPER(LAYER_NAME)=UPPER(V_ENTITY_TYPE) AND IS_VIRTUAL_PORT_ALLOWED=FALSE)
THEN

PERFORM(FN_BULK_INSERT_PORT_INFO(V_INPUT_PORT,V_OUTPUT_PORT,V_ENTITY_TYPE, CURRENTSYSTEM_ID,
NEW_NETWORK_ID,V_CREATED_BY));
END IF;

IF (v_is_accessories_required)
THEN
  select json_array_elements(accessories_data::json)  from entity_group_library where id =p_id limit 1 into V_ACCESS;
select string_agg(distinct jkey, ', ')
from entity_group_library, json_object_keys(V_ACCESS::json) as t(jkey)
inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper('ATT_DETAILS_ACCESSORIES')
where id=p_id INTO V_ALLACCESSCOLUMNS;

raise info '%','Rkquery1 '||V_ALLACCESSCOLUMNS;

select string_agg(distinct format('(json_array_elements(accessories_data::json) ->> %L)::'||sch.data_type||' as %I',jkey, jkey), ', ')
from entity_group_library, json_object_keys(V_ACCESS::json) as t(jkey)
inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper('ATT_DETAILS_ACCESSORIES')
where id=p_id INTO V_ALLACCESSCOLUMNROW;

raise info '%','Rkquery2 '||V_ALLACCESSCOLUMNROW;
QUERY='INSERT INTO ATT_DETAILS_ACCESSORIES ('||V_ALLACCESSCOLUMNS||',CREATED_BY,PARENT_SYSTEM_ID,PARENT_NETWORK_ID,PARENT_ENTITY_TYPE)
SELECT '||V_ALLACCESSCOLUMNROW||','||v_user_id||','||CURRENTSYSTEM_ID||' , '''||NEW_NETWORK_ID||''', entity_type
FROM ENTITY_GROUP_LIBRARY WHERE ID = '||p_id||' ';
EXECUTE QUERY;
END IF;
IF (( select count(tray_info_data) from entity_group_library where id=p_id)>0)
THEN
-- GET New Network-Id....
SELECT clone.status, clone.message, clone.o_p_system_id , clone.o_p_network_id, clone.o_p_entity_type, clone.o_sequence_id into
tray_status, tray_network_id, tray_p_system_id , tray_p_network_id, tray_p_entity_type, tray_sequence_id
FROM fn_get_clone_network_code('SpliceTray',p_gtype, '',CURRENTSYSTEM_ID,v_entity_type) as clone limit 1;
IF (tray_status=true)
THEN
  select json_array_elements(tray_info_data::json)  from entity_group_library where id =p_id limit 1 into V_TRAY_INFO;
select string_agg(distinct jkey, ', ')
from entity_group_library, json_object_keys(V_TRAY_INFO::json) as t(jkey)
inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper('ATT_DETAILS_SPLICE_TRAY')
where id=p_id INTO V_TRAYHEADER;

--raise info '%','Rkquery1 '||V_TRAYCOLUMNS;

select string_agg(distinct format('(json_array_elements(tray_info_data::json) ->> %L)::'||sch.data_type||' as %I',jkey, jkey), ', ')
from entity_group_library, json_object_keys(V_TRAY_INFO::json) as t(jkey)
inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper('ATT_DETAILS_SPLICE_TRAY')
where id=p_id INTO V_TRAYCOLUMNS;

--raise info '%','Rkquery2 '||V_ALLACCESSCOLUMNROW;
QUERY='INSERT INTO ATT_DETAILS_SPLICE_TRAY ('||V_TRAYHEADER||',NETWORK_ID,CREATED_BY,PARENT_SYSTEM_ID,PARENT_NETWORK_ID,PARENT_ENTITY_TYPE)
SELECT '||V_TRAYCOLUMNS||','''||tray_network_id||''','||v_user_id||','||tray_p_system_id||' , '''||tray_p_network_id||''', entity_type
FROM ENTITY_GROUP_LIBRARY WHERE ID = '||p_id||' ';
EXECUTE QUERY;
END IF;
END IF;
 IF EXISTS(SELECT 1 FROM entity_group_library where id=p_id)
THEN
 FOR V_AROW IN SELECT * FROM entity_group_library WHERE parent_id=p_id and is_active=true
 LOOP

SELECT * INTO V_STATUS FROM
 fn_save_child_group_library(V_AROW.ENTITY_TYPE,NEW_NETWORK_ID, CURRENTSYSTEM_ID, v_entity_type,V_LONGITUDE::DOUBLE PRECISION,V_LATITUDE::DOUBLE PRECISION,v_user_id,V_AROW.ID);
 
 END LOOP;
END IF;

END IF;
END LOOP;
close curgeommapping;
END IF;

RETURN QUERY
select new_status as status, new_network_id as message;

END
$BODY$;

ALTER FUNCTION public.fn_grouplibrary_save_point_entity(integer, character varying)
    OWNER TO postgres;