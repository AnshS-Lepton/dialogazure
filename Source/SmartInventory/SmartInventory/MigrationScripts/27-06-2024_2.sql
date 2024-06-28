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
v_entity_status character varying='A';
v_is_new_entity boolean=false;
BEGIN
query='';
V_ALLACCESSCOLUMNS='';
V_ALLACCESSCOLUMNROW='';
currentsystem_id:=0;
V_INPUT_PORT=0;
V_OUTPUT_PORT=0;
v_province_id:=0;
v_region_id:=0;
select coalesce(source_ref_id,''),upper(coalesce(source_ref_type,'WEB')) into v_source_ref_id,v_source_ref_type from temp_entity_details limit 1;
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

IF(v_source_ref_type='NETWORK_TICKET')
then
	v_entity_status='D';
	v_is_new_entity=true;
end if;

EXECUTE 'UPDATE '||v_layer_table||' set is_new_entity='||v_is_new_entity||',status='''||v_entity_status||''',source_ref_id='''||v_source_ref_id||''',source_ref_type='''||v_source_ref_type||''' where system_id= '||currentsystem_id||'';

INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,creator_remark,approver_remark,created_by,common_name, network_status,display_name,NO_OF_PORTS,status,source_ref_id,source_ref_type)
values(currentsystem_id, v_entity_type,v_latitude::double precision, v_longitude::double precision,
'A',ST_GEOMFROMTEXT('POINT('||v_longitude::character varying||' '||v_latitude::character varying||')',4326),now(),'Created', '' ,v_user_id,new_network_id, 'P',
fn_get_display_name(currentsystem_id,v_entity_type),
(case when (UPPER(v_entity_type)='SPLITTER' or UPPER(v_entity_type)='ONT') and coalesce(V_INPUT_PORT,0)>0
then (V_INPUT_PORT||':'||V_OUTPUT_PORT)::text
when coalesce(V_INPUT_PORT,0)>0 and coalesce(V_OUTPUT_PORT,0)>0 then V_INPUT_PORT::text
when coalesce(V_NO_OF_PORT,0)>0 then V_NO_OF_PORT::text else '' end),v_entity_status,v_source_ref_id,v_source_ref_type) ;

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
	
	

CREATE OR REPLACE FUNCTION public.fn_save_child_group_library(
	p_entity_type character varying,
	p_network_id character varying,
	p_parent_system_id integer,
	p_parent_entity_type character varying,
	p_longitude double precision,
	p_latitude double precision,
	p_created_by integer,
	p_id integer)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
V_ALLCOLUMNS CHARACTER VARYING;
QUERY CHARACTER VARYING;
V_AROW_NETWORK_ID record;
V_CHILD_LAYER_TABLE character varying;
V_CURRENTSYSTEM_ID INTEGER;
V_AROW_ACCESSORIES RECORD;
V_STATUS BOOLEAN;
V_MESSAGE CHARACTER VARYING;
V_ENTITYNAME CHARACTER VARYING;
V_INPUT_PORT integer;
V_OUTPUT_PORT integer;
V_AROW RECORD;
V_AROW_EQUIPMENT RECORD;
V_AROW_MODEL RECORD;
V_OLD_PARENT_ID CHARACTER VARYING;
V_SUPER_PARENT RECORD;
 v_province_id integer;
  v_region_id integer;
  v_allcolumnrows CHARACTER VARYING;
  V_ALLACCESSCOLUMNS character varying;
       V_ALLACCESSCOLUMNROW character varying;
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
v_source_ref_id character varying;
v_source_ref_type character varying;
v_entity_status character varying='A';
v_is_new_entity boolean=false;
BEGIN
V_STATUS:=true;
V_MESSAGE:='Success';
V_INPUT_PORT=0;
V_OUTPUT_PORT=0;
 v_province_id:=0;
 v_region_id:=0;
  V_ALLACCESSCOLUMNS='';
      V_ALLACCESSCOLUMNROW='';
V_ENTITYNAME:=UPPER(P_ENTITY_TYPE||'_NAME');
select coalesce(source_ref_id,''),upper(coalesce(source_ref_type,'WEB')) into v_source_ref_id,v_source_ref_type from temp_entity_details limit 1;
IF NOT EXISTS(SELECT 1 FROM VW_LAYER_MAPPING WHERE UPPER(CHILD_LAYER_NAME)=UPPER(p_entity_type) AND UPPER(PARENT_LAYER_NAME)=UPPER(p_parent_entity_type))
THEN

SELECT * INTO V_AROW_NETWORK_ID
FROM FN_GET_CLONE_NETWORK_CODE(P_ENTITY_TYPE,'Point', (P_LONGITUDE||' '||P_LATITUDE)::CHARACTER VARYING,0,'') order by status desc LIMIT 1;
ELSE

SELECT * INTO V_AROW_NETWORK_ID
FROM FN_GET_CLONE_NETWORK_CODE(P_ENTITY_TYPE,'Point', (P_LONGITUDE||' '||P_LATITUDE)::CHARACTER VARYING,P_PARENT_SYSTEM_ID,P_PARENT_ENTITY_TYPE) order by status desc LIMIT 1;
END IF;
  select id into v_province_id from province_boundary province where ST_Within(St_Geomfromtext('POINT('||p_longitude||' '||p_latitude||')',4326),province.sp_geometry);
	select id into v_region_id from region_boundary region where ST_Within(St_Geomfromtext('POINT('||p_longitude||' '||p_latitude||')',4326),region.sp_geometry);
SELECT LAYER_TABLE INTO V_CHILD_LAYER_TABLE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME)=LOWER(P_ENTITY_TYPE);

-- EXECUTE 'SELECT STRING_AGG(COLUMN_NAME, '','' ) FROM INFORMATION_SCHEMA.COLUMNS WHERE UPPER(TABLE_NAME) = UPPER('''||V_CHILD_LAYER_TABLE||''') AND UPPER(COLUMN_NAME) NOT IN
--(''SYSTEM_ID'',''LATITUDE'',''LONGITUDE'',''MODIFIED_ON'',''CREATED_ON'',''NETWORK_ID'',''CREATED_BY'',''PARENT_SYSTEM_ID'',''PARENT_NETWORK_ID'',''PARENT_ENTITY_TYPE'',''SEQUENCE_ID'',UPPER('''||P_ENTITY_TYPE||'_NAME''),''REGION_ID'',''PROVINCE_ID'')'
-- INTO V_ALLCOLUMNS;
select string_agg(distinct jkey, ', ')
from entity_group_library, json_object_keys(entity_data::json) as t(jkey) where id=p_id INTO v_allcolumns;

select string_agg(distinct format('(entity_data::json ->> %L)::'||Replace(sch.data_type,'USER-DEFINED','geometry')||' as %I',jkey, jkey), ', ')
from entity_group_library, json_object_keys(entity_data::json) as t(jkey) 
inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper(V_CHILD_LAYER_TABLE)
where id=p_id INTO v_allcolumnrows;


IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE UPPER(TABLE_NAME)=UPPER(V_CHILD_LAYER_TABLE) AND UPPER(COLUMN_NAME)
IN ('LATITUDE','LONGITUDE'))
THEN
raise info '%','QUERY'||QUERY;
QUERY='INSERT INTO '||V_CHILD_LAYER_TABLE||' ('||v_allcolumns||',NETWORK_ID,LATITUDE,LONGITUDE,CREATED_BY,CREATED_ON,PARENT_SYSTEM_ID,PARENT_NETWORK_ID,PARENT_ENTITY_TYPE,SEQUENCE_ID,'||V_ENTITYNAME||''||' ,region_id,province_id)
SELECT '||v_allcolumnrows||', '''||V_AROW_NETWORK_ID.MESSAGE||''', '||P_LATITUDE||', '||P_LONGITUDE||', '||P_CREATED_BY||',now(),'||V_AROW_NETWORK_ID.O_P_SYSTEM_ID||' , '''||V_AROW_NETWORK_ID.O_P_NETWORK_ID||''',
'''||V_AROW_NETWORK_ID.O_P_ENTITY_TYPE||''', '||V_AROW_NETWORK_ID.O_SEQUENCE_ID||','''||V_AROW_NETWORK_ID.MESSAGE||''','||v_region_id||','||v_province_id||'
FROM  entity_group_library where id = '||p_id||' RETURNING SYSTEM_ID';

ELSE
QUERY='INSERT INTO '||V_CHILD_LAYER_TABLE||' ('||V_ALLCOLUMNS||',NETWORK_ID,CREATED_BY,CREATED_ON,PARENT_SYSTEM_ID,PARENT_NETWORK_ID,PARENT_ENTITY_TYPE,SEQUENCE_ID,'||V_ENTITYNAME||''||',region_id,province_id )
SELECT '||v_allcolumnrows||', '''||V_AROW_NETWORK_ID.MESSAGE||''','||P_CREATED_BY||',now(),'||V_AROW_NETWORK_ID.O_P_SYSTEM_ID||' , '''||V_AROW_NETWORK_ID.O_P_NETWORK_ID||''', '''||V_AROW_NETWORK_ID.O_P_ENTITY_TYPE||''',
'||V_AROW_NETWORK_ID.O_SEQUENCE_ID||','''||V_AROW_NETWORK_ID.MESSAGE||''','||v_region_id||','||v_province_id||'
FROM entity_group_library where id = '||p_id||' RETURNING SYSTEM_ID';
END IF;


IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE UPPER(TABLE_NAME)=UPPER(V_CHILD_LAYER_TABLE) AND UPPER(COLUMN_NAME)
IN ('NO_OF_INPUT_PORT','SPLITTER_RATIO','NO_OF_OUTPUT_PORT'))
THEN
IF(UPPER(P_ENTITY_TYPE)='SPLITTER')
THEN
QUERY=QUERY||' ,SPLIT_PART(SPLITTER_RATIO,'':'',1)::INTEGER,SPLIT_PART(SPLITTER_RATIO,'':'',2)::INTEGER';
ELSE
QUERY=QUERY||' ,NO_OF_INPUT_PORT,NO_OF_OUTPUT_PORT';
END IF;

EXECUTE QUERY INTO V_CURRENTSYSTEM_ID,V_INPUT_PORT,V_OUTPUT_PORT;
ELSE
EXECUTE QUERY INTO V_CURRENTSYSTEM_ID;
END IF;

IF(v_source_ref_type='NETWORK_TICKET')
then
	v_entity_status='D';
	v_is_new_entity=true;
end if;

EXECUTE 'UPDATE '||V_CHILD_LAYER_TABLE||' set is_new_entity='||v_is_new_entity||',status='''||v_entity_status||''',source_ref_id='''||v_source_ref_id||''',source_ref_type='''||v_source_ref_type||''' where system_id= '||V_CURRENTSYSTEM_ID||'';

INSERT INTO POINT_MASTER(SYSTEM_ID,ENTITY_TYPE,LONGITUDE,LATITUDE,APPROVAL_FLAG,SP_GEOMETRY,APPROVAL_DATE,CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,COMMON_NAME, NETWORK_STATUS,DISPLAY_NAME,NO_OF_PORTS,status,source_ref_id,source_ref_type)
VALUES(V_CURRENTSYSTEM_ID, P_ENTITY_TYPE,P_LATITUDE::DOUBLE PRECISION, P_LONGITUDE::DOUBLE PRECISION,
'A',ST_GEOMFROMTEXT('POINT('||P_LONGITUDE::CHARACTER VARYING||' '||P_LATITUDE::CHARACTER VARYING||')',4326),NOW(),'CREATED', '' ,P_CREATED_BY,V_AROW_NETWORK_ID.MESSAGE, 'P',FN_GET_DISPLAY_NAME(V_CURRENTSYSTEM_ID,P_ENTITY_TYPE),
(case when (UPPER(P_ENTITY_TYPE)='SPLITTER' OR UPPER(P_ENTITY_TYPE)='ONT') and coalesce(V_INPUT_PORT,0)>0
then (V_INPUT_PORT||':'||V_OUTPUT_PORT)::text
when(coalesce(V_INPUT_PORT,0)>0 and coalesce(V_OUTPUT_PORT,0)>0) then V_INPUT_PORT::text else '' end),v_entity_status,v_source_ref_id,v_source_ref_type);

perform (fn_geojson_update_entity_attribute(V_CURRENTSYSTEM_ID,P_ENTITY_TYPE::character varying ,v_province_id::integer,0,false));


IF EXISTS(SELECT 1 FROM LAYER_DETAILS WHERE UPPER(LAYER_NAME)=UPPER(P_ENTITY_TYPE) AND IS_VIRTUAL_PORT_ALLOWED=FALSE)
THEN

PERFORM(FN_BULK_INSERT_PORT_INFO(V_INPUT_PORT,V_OUTPUT_PORT,P_ENTITY_TYPE, V_CURRENTSYSTEM_ID, V_AROW_NETWORK_ID.MESSAGE,P_CREATED_BY));

END IF;

IF EXISTS(SELECT 1 FROM ENTITY_GROUP_LIBRARY WHERE id=p_id AND is_accessories_required=TRUE)
THEN

-- EXECUTE 'SELECT STRING_AGG(COLUMN_NAME, '','' )
-- FROM INFORMATION_SCHEMA.COLUMNS WHERE UPPER(TABLE_NAME)=UPPER(''ATT_DETAILS_ACCESSORIES'') AND UPPER(COLUMN_NAME)
-- NOT IN (''SYSTEM_ID'',''MODIFIED_ON'',''CREATED_ON'',''CREATED_BY'',''PARENT_SYSTEM_ID'',''PARENT_NETWORK_ID'',''PARENT_ENTITY_TYPE'')'
-- INTO V_ALLCOLUMNS;
-- select string_agg(distinct jkey, ', ')
-- from entity_group_library, json_object_keys(accessories_data::json) as t(jkey)
-- inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper('ATT_DETAILS_ACCESSORIES')
--  where id=p_id INTO V_ALLACCESSCOLUMNS;
-- 
-- select string_agg(distinct format('(accessories_data::json ->> %L)::'||sch.data_type||' as %I',jkey, jkey), ', ')
-- from entity_group_library, json_object_keys(accessories_data::json) as t(jkey) 
-- inner join information_schema.columns sch on upper(sch.column_name)=upper(jkey) and upper(sch.table_name)=upper('ATT_DETAILS_ACCESSORIES')
-- where id=p_id INTO V_ALLACCESSCOLUMNROW;

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

QUERY='INSERT INTO ATT_DETAILS_ACCESSORIES ('||V_ALLACCESSCOLUMNS||',CREATED_BY,PARENT_SYSTEM_ID,PARENT_NETWORK_ID,PARENT_ENTITY_TYPE)
SELECT '||V_ALLACCESSCOLUMNROW||','||P_CREATED_BY||','||V_CURRENTSYSTEM_ID||' , '''||V_AROW_NETWORK_ID.MESSAGE||''', entity_type
FROM entity_group_library where id = '||p_id||' ';
EXECUTE QUERY;
END IF;

IF EXISTS(SELECT 1 FROM ENTITY_GROUP_LIBRARY WHERE id=p_id AND IS_ASSOCIATED_ENTITY=TRUE)
THEN
INSERT INTO ASSOCIATE_ENTITY_INFO(ENTITY_SYSTEM_ID,ENTITY_NETWORK_ID,ENTITY_TYPE,ASSOCIATED_SYSTEM_ID,ASSOCIATED_NETWORK_ID,ASSOCIATED_ENTITY_TYPE,CREATED_ON,CREATED_BY,IS_TERMINATION_POINT,ASSOCIATED_DISPLAY_NAME,ENTITY_DISPLAY_NAME)
VALUES(P_PARENT_SYSTEM_ID,p_network_id,p_parent_entity_type,V_CURRENTSYSTEM_ID,V_AROW_NETWORK_ID.MESSAGE,P_ENTITY_TYPE,NOW(),P_CREATED_BY,FALSE,FN_GET_DISPLAY_NAME(V_CURRENTSYSTEM_ID,P_ENTITY_TYPE),FN_GET_DISPLAY_NAME(p_parent_system_id,p_parent_entity_type));
END IF;

IF (( select count(tray_info_data) from entity_group_library where id=p_id)>0)
THEN
-- GET New Network-Id....
SELECT clone.status, clone.message, clone.o_p_system_id , clone.o_p_network_id, clone.o_p_entity_type, clone.o_sequence_id into
tray_status, tray_network_id, tray_p_system_id , tray_p_network_id, tray_p_entity_type, tray_sequence_id
FROM fn_get_clone_network_code('SpliceTray','Point', '',V_CURRENTSYSTEM_ID,p_entity_type) as clone limit 1;
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
SELECT '||V_TRAYCOLUMNS||','''||tray_network_id||''','||v_created_by||','||tray_p_system_id||' , '''||tray_p_network_id||''', entity_type
FROM ENTITY_GROUP_LIBRARY WHERE ID = '||p_id||' ';
EXECUTE QUERY;
END IF;
END IF;
RETURN QUERY SELECT V_STATUS,V_MESSAGE;
END
$BODY$;

ALTER FUNCTION public.fn_save_child_group_library(character varying, character varying, integer, character varying, double precision, double precision, integer, integer)
    OWNER TO postgres;
