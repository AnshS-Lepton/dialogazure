
---------------------------------------------------- Insert data to add filter type  in create fiber link form ----------------------------------------------------

 INSERT INTO public.dropdown_master
(layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES(0, 'FiberLinkType', 'Sub Link', true, 1, now(), NULL, now(), 'Sub Link', NULL, '', false, true, 0); 
  
  ---------------------------- Update dropdown_master to set is_active false for 'Spur Link' in create fiber link form ----------------------------
update dropdown_master set is_active=false  where dropdown_type='FiberLinkType' and dropdown_key ='Spur Link';
-------------------------------------------------------------------- modification in function to get details----------------------

CREATE OR REPLACE FUNCTION public.fn_get_fms_connection_report(
	p_networkstatues character varying,
	p_provinceids character varying,
	p_regionids character varying,
	p_layer_name character varying,
	p_searchby character varying,
	p_searchbytext character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_geom character varying,
	p_duration_based_column character varying,
	p_radius double precision,
	p_userid integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
V_AROW RECORD;
v_entity_port_no character varying;
v_maxcount integer;
v_maxpathid integer;
v_ststem_id integer;
v_entity_type integer;
v_maxfms_id integer;
sql TEXT;
StartSNo INTEGER; 
EndSNo INTEGER;
LowerStart character varying;
LowerEnd character varying;
TotalRecords integer; 
WhereCondition character varying;
s_report_view_name character varying;
s_geom_type character varying;
s_layer_id integer; 
s_layer_columns text;
TotalAppliedRecords integer; 
TotalRejectedRecords integer;
 r RECORD;
    ss_report_view_name TEXT;
    d_report_view_name TEXT;
    v_report_view_name TEXT;
    ss_layer_name TEXT;
    update_sql TEXT;
s_latitude text;
d_layer_name TEXT;
v_layer_name TEXT;
BEGIN

create temp table cpf_temp_result
(
id serial,
source_system_id integer,
source_network_id character varying(500),
source_port_no integer, 
source_entity_type character varying(100),
source_entity_title character varying(100),
destination_system_id integer,
destination_network_id character varying(500),
destination_port_no integer,
destination_entity_type character varying(100),
destination_entity_title character varying(100),
viya_system_id integer,
via_network_id character varying(500),
path_id integer,
via_entity_type character varying(100),
SOURCE_TUBE_COLOUR_CODE character varying(100),
DESTINATION_TUBE_COLOUR_CODE character varying(100),
SOURCE_PORT_COLOUR_CODE character varying(100),
DESTINATION_PORT_COLOUR_CODE character varying(100),
SOURCE_TUBE_NAME character varying(100),
DESTINATION_TUBE_NAME character varying(100),
fms_id integer,
splitter_id integer,
entity_type character varying(100),
client_id character varying,
client_link_id character varying,
client_core_id character varying,
otdr_length float(8) default (0)
)on commit drop; 

-------------------------------------------------------------------------------------------------------------------
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);
-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER.. 
SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key end||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT COLUMN_NAME,res_field_key,DISPLAY_NAME FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;
IF ((p_geom) != '') 
THEN
if(p_radius>0)
then
select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10) into p_geom;
end if;
RAISE INFO '------------------------------------S_LAYER_COLUMNS%', S_LAYER_COLUMNS;
sql:= 'SELECT system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join '||s_geom_type||'_master m
on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||'  and 1=1 ';
--inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
RAISE INFO '------------------------------------sql%', sql;
ELSE
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
--inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
RAISE INFO '------------------------------------sql1%', sql;
END IF;

IF ((p_networkStatues) != '' and upper(S_LAYER_COLUMNS) like '%NETWORK_STATUS%') THEN
sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in('||p_networkStatues||')';
END IF;
IF ((p_RegionIds) != '') THEN
sql:= sql ||' AND a.region_id IN ('||p_RegionIds||')';
END IF;
IF ((P_ProvinceIds) != '') THEN
sql:= sql ||' AND a.province_id IN ('||P_ProvinceIds||')';
END IF;
IF ((P_searchbytext) != '' and (P_searchby) != '') THEN
if(substring(P_searchbytext from 1 for 1)='"' and substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
then
sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
else
sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
end if;
END IF;
IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND a.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';
END IF;
sql:= sql ||' )X where system_id in(select parent_system_id from isp_port_info where parent_entity_type=''FMS'' and port_status_id=2 )';
RAISE INFO 'rrr%', sql;
--------------------------------------------------------------------------------------------------------------------
FOR V_AROW IN
-- SELECT 'FMS' as entity_type,system_id from att_details_fms 
-- where system_id in(808,805,810)--province_id = ANY (string_to_array(p_province, ',')::integer[]) and region_id = ANY (string_to_array(p_region, ',')::integer[])
EXECUTE 'SELECT ''FMS'' as entity_type, system_id FROM ('||sql||')a'
loop
perform(fn_get_fms_connection(V_AROW.system_id,V_AROW.entity_type));
end loop;
--------------------------------------------------------------------------------------------------
delete from cpf_temp_result where source_network_id=destination_network_id;
if exists(select 1 from cpf_temp_result where source_entity_type='FMS')
then
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE source_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	-- Update query
	UPDATE cpf_temp_result 
	SET 
	destination_network_id = VIA_row.destination_network_id,
	destination_system_id = VIA_row.destination_system_id,
	destination_port_no = VIA_row.destination_port_no,
	destination_entity_type = VIA_row.destination_entity_type,
	destination_entity_title = VIA_row.destination_entity_title,
	viya_system_id = VIA_row.source_system_id,
	via_network_id = VIA_row.source_network_id,
	via_entity_type = VIA_row.source_entity_type

	FROM VIA_row
	WHERE 
	cpf_temp_result.destination_entity_type = VIA_row.source_entity_type
	AND cpf_temp_result.destination_system_id = VIA_row.source_system_id
	AND cpf_temp_result.destination_network_id = VIA_row.source_network_id
	AND cpf_temp_result.destination_PORT_NO = VIA_row.source_PORT_NO;
	-- Delete query
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result 
	WHERE source_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	DELETE FROM cpf_temp_result
	WHERE SOURCE_SYSTEM_ID IN (SELECT SOURCE_SYSTEM_ID FROM VIA_row);
else

	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE destination_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	-- Update query
	UPDATE cpf_temp_result 
	SET 
	source_network_id = VIA_row.source_network_id,
	source_system_id = VIA_row.source_system_id,
	source_port_no = VIA_row.source_port_no,
	source_entity_type = VIA_row.source_entity_type,
	source_entity_title = VIA_row.source_entity_title,
	viya_system_id = VIA_row.destination_system_id,
	via_network_id = VIA_row.destination_network_id,
	via_entity_type = VIA_row.destination_entity_type

	FROM VIA_row
	WHERE 
	cpf_temp_result.source_entity_type = VIA_row.source_entity_type
	AND cpf_temp_result.source_system_id = VIA_row.source_system_id
	AND cpf_temp_result.source_network_id = VIA_row.source_network_id
	AND cpf_temp_result.source_PORT_NO = VIA_row.source_PORT_NO;
	-- Delete query
	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result 
	WHERE destination_entity_type IN ('SpliceClosure','BDB','FDB','ADB')
	)
	DELETE FROM cpf_temp_result
	WHERE destination_SYSTEM_ID IN (SELECT destination_SYSTEM_ID FROM VIA_row);
 
end if;

--select count(*) into v_maxcount from cpf_temp_result group by path_id order by count(*) desc limit 1;--header
select count(*) into v_maxcount from cpf_temp_result group by fms_id,path_id order by count(*) desc limit 1;
SELECT COUNT(DISTINCT path_id) AS path_count
FROM cpf_temp_result group by fms_id order by COUNT(DISTINCT path_id) desc limit 1 
INTO v_maxpathid;---dataloop
SELECT COUNT(DISTINCT fms_id) AS fms_id_count from cpf_temp_result
INTO v_maxfms_id;
------------------------------------------------------------------------------------------------------------------------------------
UPDATE cpf_temp_result 
SET SOURCE_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
SOURCE_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
SOURCE_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code ,client_id =adfl.customer_id,client_link_id =adfl.link_id,client_core_id=adfl.link_id,otdr_length=adfl.otdr_distance
FROM ATT_DETAILS_CABLE_INFO left join att_details_fiber_link adfl on adfl.system_id =ATT_DETAILS_CABLE_INFO.link_system_id 
WHERE cpf_temp_result.SOURCE_ENTITY_type='Cable'
and cpf_temp_result.source_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.source_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;

--select * from ATT_DETAILS_CABLE_INFO limit 20 where cable_id=847

UPDATE cpf_temp_result 
SET DESTINATION_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
DESTINATION_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
DESTINATION_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code ,client_id =adfl.customer_id,client_link_id =adfl.link_id,client_core_id=adfl.link_id,otdr_length=adfl.otdr_distance
FROM ATT_DETAILS_CABLE_INFO 
left join att_details_fiber_link adfl on adfl.system_id =ATT_DETAILS_CABLE_INFO.link_system_id 
WHERE cpf_temp_result.DESTINATION_ENTITY_type='Cable'
and cpf_temp_result.DESTINATION_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.DESTINATION_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;

-- select * from att_details_cable where network_id='MAT-CBL000356'
-- select link_system_id from ATT_DETAILS_CABLE_INFO  where cable_id=361597
-- select link_id,customer_id,main_link_id from att_details_fiber_link  where system_id in(select link_system_id from ATT_DETAILS_CABLE_INFO  where cable_id=361597)

--       UPDATE cpf_temp_result 
--       SET 
--       splitter_id =destination_system_id
--       where destination_entity_type = 'Splitter' and 
--       destination_system_id not in(select source_system_id from cpf_temp_result where source_entity_type = 'Splitter') ; 
   
--       UPDATE cpf_temp_result 
--       SET 
--       entity_type ='Splitter'
--       where destination_entity_type = 'Splitter' ; 

       for  r in  select  * from cpf_temp_result loop

select report_view_name,layer_name into ss_report_view_name,ss_layer_name from layer_details where lower(layer_name) =lower(r.source_entity_type);
select report_view_name,layer_name into d_report_view_name,d_layer_name from layer_details where lower(layer_name) =lower(r.destination_entity_type);
if r.via_entity_type is not null then 
select report_view_name,layer_name into v_report_view_name,v_layer_name from layer_details where lower(layer_name) =lower(r.via_entity_type);
end if;
if lower(ss_layer_name) = 'patchcord'   then 
ss_layer_name ='patch_cord';
end if;
if  lower(d_layer_name) = 'patchcord'  then 
d_layer_name ='patch_cord';
end if;
if   lower(v_layer_name) = 'patchcord' then 
v_layer_name ='patch_cord';
end if ;

ss_layer_name:=ss_layer_name||'_name';
d_layer_name:= d_layer_name||'_name';
v_layer_name =v_layer_name||'_name';
s_latitude ='latitude';
raise info 'ss_report_view_name% ',ss_report_view_name;
raise info 'd_report_view_name% ',d_report_view_name;
raise info 'v_report_view_name% ',v_report_view_name;

if ss_report_view_name IS NOT NULL then
if lower(r.source_entity_type) != 'cable' then 

EXECUTE '
        UPDATE cpf_temp_result ctf
        SET source_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || ss_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(ss_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.source_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
else 

EXECUTE 
        'UPDATE cpf_temp_result ctf
        SET source_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || ss_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(ss_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.source_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';

end if;
end if;
 if d_report_view_name IS NOT NULL then 
if lower(r.destination_entity_type) != 'cable' then 
EXECUTE ' UPDATE cpf_temp_result ctf
        SET destination_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || d_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(d_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.destination_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || '; ';

else
EXECUTE 
        'UPDATE cpf_temp_result ctf
        SET destination_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || d_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(d_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.destination_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
end if;
end if;
if v_report_view_name IS NOT NULL then 
if lower(r.via_entity_type) != 'cable' then 
if r.via_entity_type is not null and r.via_entity_type !='' then 
raise info 'test12% ','test12';

EXECUTE '
        UPDATE cpf_temp_result ctf
        SET via_network_id = concat(''Network ID: '',srv.network_id, E''\n'', ''Name: '',srv.' || v_layer_name || ', E''\n'', '' Location: '',latitude,'' '', longitude)::character varying 
           
    
        FROM ' || quote_ident(v_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.viya_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';

end if;

else

if r.via_entity_type is not null and r.via_entity_type !='' then 
   EXECUTE      'UPDATE cpf_temp_result ctf
        SET via_network_id = concat(''Network ID: '',srv.network_id, E''\n'', '' Name: '',srv.' || v_layer_name || ',E''\n'',''Total Core '',srv.total_core, ''F'')::character varying 
           
    
        FROM ' || quote_ident(v_report_view_name) || ' srv
        WHERE srv.system_id = ' || quote_literal(r.viya_system_id) || '
          AND ctf.id = ' || quote_literal(r.id) || ';
    ';
end if;
end if;
end if;
end loop;
-------------------------------------------------------------------------------------------------

IF v_maxcount > 100 THEN
   v_maxcount := 100; -- cap headers to 100
END IF;

IF v_maxpathid > 100 THEN
   v_maxpathid := 100; -- cap data rows to 100
END IF;


return query
select row_to_json(result) 
from (select (select array_to_json(array_agg(row_to_json(x)))from (select *,v_maxcount as headerCol,v_maxpathid as rowsdataloop,v_maxfms_id as globaLoopcount
from cpf_temp_result order by id) x) as lstConnectionInfo
)result ;
END; 
$BODY$;

ALTER FUNCTION public.fn_get_fms_connection_report(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer)
    OWNER TO postgres;
