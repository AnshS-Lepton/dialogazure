
INSERT INTO public.module_master
( module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES('FiberAllocationReport', 'Fiber Allocation Report', '', '', 1, now(), 1, now(), 'Web', true, 'FBR-LRT', 0, 0, false, '', NULL);

INSERT INTO public.layer_action_mapping
(layer_id, action_name, is_active, action_sequence, action_title, is_visible, is_isp_action, is_osp_action, action_abbr, action_layer_id, action_module_id, is_mobile_action, is_web_action, action_mobile_module_id, res_field_key, is_enable_in_draft, parent_action_id)
VALUES((select layer_id from layer_details where layer_name='FMS'), 'FiberAllocationReport', true, 13, 'Fiber Allocation Report', true, false, true, 'E', 0, 0, false, true, 0, 'FiberAllocationReport', false, 0);
------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_entity_export_log(character varying, character varying, integer, integer, character varying, character varying, integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_entity_export_log(character varying, character varying, integer, integer, character varying, character varying, integer, character varying,character varying);
CREATE OR REPLACE FUNCTION public.fn_get_entity_export_log(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_user_id integer,
	p_timeinterval character varying,
    p_logtype character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   v_regex character varying;

BEGIN

	select value into v_regex from global_settings where key='SqlInjectionRegex';
	if (p_searchby !~* v_regex or p_searchtext !~* v_regex or p_sorttype !~* v_regex or p_timeinterval !~* v_regex or p_sortcolname !~* v_regex ) then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
	begin
----select * from fn_get_entity_export_log('','',1,10,'','',5,'2 months');
LowerStart:='';
LowerEnd:='';

 IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
   	IF EXISTS (select 1 from information_schema.columns where upper(table_name) = upper('export_report_log') and 
	upper(column_name) = upper(P_SORTCOLNAME) and upper(data_type) in('CHARACTER VARYING','TEXT')) THEN
		LowerStart:='LOWER(';
	else
		LowerStart:='(';
	end if;
	LowerEnd:=')';	
END IF;

RAISE INFO '%', sql;

-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'export_started_on' ELSE P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')
AS S_No,l.user_id, export_started_on, export_ended_on, file_name, file_type, total_entity, planned, asbuilt, dormant, sp_geometry, status, file_location
	,file_extension,um.user_name
    FROM export_report_log l inner join vw_user_master um on um.user_id=l.user_id
    where   export_started_on >= (now() - INTERVAL '''||$8||''') ';

IF ((p_logtype) != '') THEN
sql:= sql ||' AND lower(l.log_type) LIKE lower(''%'||$9||'%'')';
END IF;

IF (p_user_id>0) THEN
sql:= sql ||' and l.user_id = '||p_user_id||'';
END IF;


IF ((p_searchtext) != '') THEN
sql:= sql ||' AND page_title LIKE lower(''%'||$2||'%'')';
END IF;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 

RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

end;
	end if;
END ;
$BODY$;

ALTER FUNCTION public.fn_get_entity_export_log(character varying, character varying, integer, integer, character varying, character varying, integer, character varying,character varying)
    OWNER TO postgres;

---------------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_fiber_allocation(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_fiber_allocation(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, character varying, double precision, integer);

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

BEGIN
create temp table cpf_temp_result
(
id serial,
source_system_id integer,
source_network_id character varying(100),
source_port_no integer, 
source_entity_type character varying(100),
source_entity_title character varying(100),
destination_system_id integer,
destination_network_id character varying(100),
destination_port_no integer,
destination_entity_type character varying(100),
destination_entity_title character varying(100),
viya_system_id integer,
via_network_id character varying(100),
path_id integer,
via_entity_type character varying(100),
SOURCE_TUBE_COLOUR_CODE character varying(100),
DESTINATION_TUBE_COLOUR_CODE character varying(100),
SOURCE_PORT_COLOUR_CODE character varying(100),
DESTINATION_PORT_COLOUR_CODE character varying(100),
SOURCE_TUBE_NAME character varying(100),
DESTINATION_TUBE_NAME character varying(100),
fms_id integer
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
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
RAISE INFO '------------------------------------sql%', sql;
ELSE
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id from (select a.system_id from '||S_REPORT_VIEW_NAME||' a 
inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.parent_type in(select layer_title from layer_details where layer_name=''POD'') and 1=1 ';
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
RAISE INFO '%', sql;
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
	WHERE source_entity_type IN ('SpliceClosure','FDC','FAT')
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
	WHERE source_entity_type IN ('SpliceClosure','FDC','FAT')
	)
	DELETE FROM cpf_temp_result
	WHERE SOURCE_SYSTEM_ID IN (SELECT SOURCE_SYSTEM_ID FROM VIA_row);
else

	WITH VIA_row AS (
	SELECT * 
	FROM cpf_temp_result
	WHERE destination_entity_type IN ('SpliceClosure','FDC','FAT')
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
	WHERE destination_entity_type IN ('SpliceClosure','FDC','FAT')
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
SOURCE_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code 
FROM ATT_DETAILS_CABLE_INFO
WHERE cpf_temp_result.SOURCE_ENTITY_type='Cable'
and cpf_temp_result.source_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.source_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;

--select * from ATT_DETAILS_CABLE_INFO limit 20 where cable_id=847

UPDATE cpf_temp_result 
SET DESTINATION_TUBE_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.tube_color_code,
DESTINATION_TUBE_NAME =ATT_DETAILS_CABLE_INFO.tube_number,
DESTINATION_PORT_COLOUR_CODE=ATT_DETAILS_CABLE_INFO.core_color_code 
FROM ATT_DETAILS_CABLE_INFO
WHERE cpf_temp_result.DESTINATION_ENTITY_type='Cable'
and cpf_temp_result.DESTINATION_system_id=ATT_DETAILS_CABLE_INFO.cable_id
and cpf_temp_result.DESTINATION_port_no=ATT_DETAILS_CABLE_INFO.fiber_number;
-------------------------------------------------------------------------------------------------
return query
select row_to_json(result) 
from (select (select array_to_json(array_agg(row_to_json(x)))from (select *,v_maxcount as headerCol,v_maxpathid as rowsdataloop,v_maxfms_id as globaLoopcount
from cpf_temp_result order by id) x) as lstConnectionInfo
)result ;
END; 
$BODY$;
---------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- FUNCTION: public.fn_get_fms_connection(integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_fms_connection(integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_fms_connection(
	p_entity_system_id integer,
	p_entity_type character varying)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
	DECLARE
		V_AROW RECORD;
		v_entity_port_no character varying;
		v_maxcount integer;
		v_maxpathid integer;
		v_maxfms_id integer;
		
BEGIN

	

    FOR V_AROW IN
    SELECT port_number  from isp_port_info 
   
    WHERE  parent_system_id=p_entity_system_id and parent_entity_type= ''||p_entity_type||'' and port_status_id=2 and input_output='O'
    LOOP
 	
	insert into cpf_temp_result(source_system_id,source_network_id,source_port_no,source_entity_type,source_entity_title,
	destination_system_id,destination_network_id,destination_port_no,destination_entity_type,destination_entity_title,viya_system_id,via_network_id,path_id,via_entity_type,fms_id
	)
	select a.source_system_id,a.source_network_id,a.source_port_no,a.source_entity_type,a.source_entity_title,
	a.destination_system_id,a.destination_network_id,a.destination_port_no,a.destination_entity_type,
	a.destination_entity_title,null,null,V_AROW.port_number,null,p_entity_system_id
	
	from fn_get_fms_connection_path('', '', 0, 0,'', '',p_entity_system_id, V_AROW.port_number,p_entity_type)a WHERE a.source_system_id != a.destination_system_id AND 
	  a.source_entity_type != a.destination_entity_type;
drop  table cpf_temp;
end loop;
-------------------------------------------------------------------------------------------------

	
END; 
$BODY$;


----------------------------------------------------------------------------------------------------------------------------------------
update layer_columns_settings set is_duration_based_column=true where layer_id=36 and 
column_name in('created_on','modified_on') and setting_type='Report'
------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_reportcolumn_list(character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_reportcolumn_list(character varying);

CREATE OR REPLACE FUNCTION public.fn_get_reportcolumn_list(
	p_layer_name character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
v_layerId int;
BEGIN

select layer_id into v_layerId from layer_details where lower(layer_name) = lower(p_layer_name);

RETURN QUERY select row_to_json(row) from (select s.display_name as value, s.column_name as key  from layer_columns_settings s 
										   where s.layer_id=v_layerId and upper(s.setting_type)='REPORT' and s.is_active=true and column_name in('network_id','fms_name') order by column_sequence) row;

END
$BODY$;

ALTER FUNCTION public.fn_get_reportcolumn_list(character varying)
    OWNER TO postgres;
---------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_fms_connection_path(character varying, character varying, integer, integer, character varying, character varying, integer, integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_fms_connection_path(character varying, character varying, integer, integer, character varying, character varying, integer, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_fms_connection_path(
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_entity_system_id integer,
	p_entity_port_no integer,
	p_entity_type character varying)
    RETURNS TABLE(id integer, connection_id integer, rowid integer, parent_connection_id integer, source_system_id integer, source_network_id character varying, source_entity_type character varying, source_entity_title character varying, source_port_no integer, is_source_virtual boolean, destination_system_id integer, destination_network_id character varying, destination_entity_type character varying, destination_entity_title character varying, destination_port_no integer, is_destination_virtual boolean, is_customer_connected boolean, created_on timestamp without time zone, created_by integer, approved_by integer, approved_on timestamp without time zone, cable_calculated_length double precision, cable_measured_length double precision, cable_network_status character varying, splitter_ratio character varying, is_backward_path boolean, trace_end character varying, source_display_name character varying, destination_display_name character varying, source_tray_system_id integer, destination_tray_system_id integer, source_tray_display_name character varying, destination_tray_display_name character varying, source_entity_sub_type character varying, destination_entity_sub_type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

declare v_down_stream_end character varying(1);	
v_max_row_count integer;
BEGIN
v_down_stream_end:='';
v_max_row_count:=2000;
--TEMP TABLE TO STORE CONNECTION INFO RESULT..

create temp table cpf_temp
(
	level text,
	rowid integer,
	connection_id integer,
	parent_connection_id integer,
	source_system_id integer,
	source_network_id character varying(100),
	source_entity_type character varying(100),
	source_entity_title character varying(100),
	source_port_no integer,
	is_source_virtual boolean,
	destination_system_id integer,
	destination_network_id character varying(100),
	destination_entity_type character varying(100),
	destination_entity_title character varying(100),
	destination_port_no integer,
	is_destination_virtual boolean,
	is_customer_connected boolean,
	created_on timestamp ,  		
	created_by integer,
	approved_by integer,
	approved_on timestamp,
	trace_end character varying(1),
	cable_calculated_length double precision,
	cable_measured_length double precision,
	cable_network_status character varying(10), 
	splitter_ratio character varying(100),
	source_entity_sub_type character varying,
	destination_entity_sub_type character varying,
	source_display_name character varying,
	destination_display_name character varying,
	source_tray_system_id integer ,
	destination_tray_system_id integer, 
	source_tray_display_name character varying, 
	destination_tray_display_name character varying
) on commit drop;
	--select cable_measured_length from att_details_cable 
	
	insert into cpf_temp(level,rowid,connection_id,parent_connection_id,source_system_id,source_network_id,source_entity_type,source_entity_title,source_port_no,
	source_display_name,is_source_virtual,destination_system_id,destination_network_id,destination_entity_type,destination_entity_title,
	destination_port_no,destination_display_name,is_destination_virtual,is_customer_connected,
	created_on,created_by,approved_by,approved_on,trace_end,cable_calculated_length,cable_measured_length,cable_network_status,splitter_ratio,source_entity_sub_type,
	destination_entity_sub_type,source_tray_system_id,destination_tray_system_id,source_tray_display_name,destination_tray_display_name)
	select 
	cinfo.level,cinfo.rowid,cinfo.connection_id,cinfo.parent_connection_id,cinfo.source_system_id,cinfo.source_network_id,
	ls.layer_name as source_entity_type,
	case when upper(ls.layer_name)='EQUIPMENT' then coalesce(srcsubtype.layer_title,cinfo.source_entity_sub_type) else ls.layer_title end as source_entity_title
	,cinfo.source_port_no,cinfo.source_display_name,
	ls.is_virtual_port_allowed as is_source_virtual,
	cinfo.destination_system_id,cinfo.destination_network_id,
	ld.layer_name as destination_entity_type,
	case when upper(ld.layer_name)='EQUIPMENT' then coalesce(dstsubtype.layer_title,cinfo.destination_entity_sub_type) else ld.layer_title end  as destination_entity_title,
	cinfo.destination_port_no,cinfo.destination_display_name,
	ld.is_virtual_port_allowed as is_destination_virtual,
	cinfo.is_customer_connected,cinfo.created_on,cinfo.created_by,cinfo.approved_by,
	cinfo.approved_on,cinfo.trace_end,c.cable_calculated_length,c.cable_measured_length,c.network_status  as cable_network_status,
	s.splitter_ratio,cinfo.source_entity_sub_type,cinfo.destination_entity_sub_type,coalesce(cinfo.source_tray_system_id,0),coalesce(cinfo.destination_tray_system_id,0),
	cinfo.source_tray_display_name,cinfo.destination_tray_display_name from 
	(
	-- COLUMN SEQUENCE CHANGED AS SEARCH ENTITY ALWAYS SHOULD IN LEFT SIDE..
	   select  a.rowid,a.level,a.connection_id,a.parent_connection_id,a.destination_system_id as source_system_id ,a.destination_network_id as source_network_id,
	   a.destination_entity_type as source_entity_type,a.destination_port_no as source_port_no,a.source_system_id as destination_system_id,
	   a.source_network_id as destination_network_id,a.source_entity_type as destination_entity_type,a.source_port_no as destination_port_no,a.is_customer_connected,
	   a.created_on,a.created_by,a.approved_by,a.approved_on,a.destination_entity_sub_type as source_entity_sub_type,a.source_entity_sub_type as destination_entity_sub_type,
	   a.destination_display_name as source_display_name,a.source_display_name as destination_display_name,
	   a.destination_tray_system_id as source_tray_system_id,a.source_tray_system_id as destination_tray_system_id,
	   a.destination_tray_display_name as source_tray_display_name,a.source_tray_display_name as destination_tray_display_name
	   , 'A' as trace_end from (
		with RECURSIVE UpStream as (
			select  1 as rowid, '-1' as level, ci.connection_id,0 as parent_connection_id,ci.source_system_id,ci.source_network_id,ci.source_entity_type,ci.source_port_no,
			ci.destination_system_id ,ci.destination_network_id,ci.destination_entity_type,ci.destination_port_no,ci.is_customer_connected,
			ci.created_on,ci.created_by,ci.approved_by,ci.approved_on
			,ci.source_entity_sub_type,ci.destination_entity_sub_type,
			ci.source_display_name,ci.destination_display_name,ci.source_tray_system_id,ci.destination_tray_system_id,
			ci.source_tray_display_name,ci.destination_tray_display_name
			from connection_info ci
			where ci.destination_system_id=p_entity_system_id and ci.destination_port_no =p_entity_port_no and 
			lower(ci.destination_entity_type) = lower(p_entity_type) 
			and ((ci.source_entity_type::text)||(ci.source_system_id::text))!=((ci.destination_entity_type::text)||(ci.destination_system_id::text))
			union 
			select  cte.rowid+1 as rowid,
			cte.level || '-' || row_number() over (order by cte.level, cte.source_port_no)::text as level, 
			ci.connection_id,cte.connection_id as parent_connection_id,ci.source_system_id,ci.source_network_id,ci.source_entity_type,ci.source_port_no,
			ci.destination_system_id ,ci.destination_network_id,ci.destination_entity_type,ci.destination_port_no,
			ci.is_customer_connected,ci.created_on,ci.created_by,ci.approved_by,ci.approved_on
			,ci.source_entity_sub_type,ci.destination_entity_sub_type,
			ci.source_display_name,ci.destination_display_name ,ci.source_tray_system_id,ci.destination_tray_system_id,
			ci.source_tray_display_name,ci.destination_tray_display_name
			from connection_info ci 
			inner join UpStream cte on 
			ci.destination_system_id=cte.source_system_id and 
			ci.destination_port_no =cte.source_port_no and lower(ci.destination_entity_type) = lower(cte.source_entity_type)
			-- condition to get rid off from infinite loop in case of ring connectivity..
			where ((lower(ci.destination_entity_type)||ci.destination_system_id||ci.destination_port_no)!=(lower(p_entity_type)||p_entity_system_id||p_entity_port_no))
			and ci.destination_entity_type='FMS' and ci.destination_port_no<0
			and cte.rowid<=v_max_row_count
				

		) select * from UpStream  order by rowid desc) a

	 union all

	 select b.*,'B' as trace_end from (
		with RECURSIVE DownStream as (
			select  1 as rowid,'1' as level, ci.connection_id,0 as parent_connection_id,ci.source_system_id,ci.source_network_id,ci.source_entity_type,ci.source_port_no,
			ci.destination_system_id ,ci.destination_network_id,ci.destination_entity_type,ci.destination_port_no,
			ci.is_customer_connected,ci.created_on,ci.created_by,ci.approved_by,ci.approved_on
			,ci.source_entity_sub_type,ci.destination_entity_sub_type,
			ci.source_display_name,ci.destination_display_name,ci.source_tray_system_id,ci.destination_tray_system_id,
			ci.source_tray_display_name,ci.destination_tray_display_name
			from connection_info ci
			where ci.source_system_id=p_entity_system_id and ci.source_port_no =p_entity_port_no and lower(ci.source_entity_type) = lower(p_entity_type)
			and ((ci.source_entity_type::text)||(ci.source_system_id::text))!=((ci.destination_entity_type::text)||(ci.destination_system_id::text))
			union 
			select  cte.rowid+1 as rowid,
			cte.level || '-' || row_number() over (order by cte.level, cte.destination_port_no)::text as level, 
			ci.connection_id,cte.connection_id as parent_connection_id,ci.source_system_id,ci.source_network_id,ci.source_entity_type,ci.source_port_no,
			ci.destination_system_id ,ci.destination_network_id,ci.destination_entity_type,ci.destination_port_no,
			ci.is_customer_connected,ci.created_on,ci.created_by,ci.approved_by,ci.approved_on
			,ci.source_entity_sub_type,ci.destination_entity_sub_type,
			ci.source_display_name,ci.destination_display_name,ci.source_tray_system_id,ci.destination_tray_system_id,
			ci.source_tray_display_name,ci.destination_tray_display_name  
			from connection_info ci inner join DownStream cte on 
			ci.source_system_id=cte.destination_system_id and 
			ci.source_port_no =cte.destination_port_no and lower(ci.source_entity_type) = lower(cte.destination_entity_type)
			-- condition to get rid off from infinite loop in case of ring connectivity..
			where ((lower(ci.source_entity_type)||ci.source_system_id||ci.source_port_no)!=(lower(p_entity_type)||p_entity_system_id||p_entity_port_no))
			and ci.source_entity_type='FMS' and ci.source_port_no<0
			and cte.rowid<=v_max_row_count 
			
		) select * from DownStream
	 ) b
	) cinfo left join att_details_cable c on ((cinfo.destination_system_id=c.system_id  and upper(cinfo.destination_entity_type)='CABLE') 
	or (cinfo.source_system_id=c.system_id  and upper(cinfo.source_entity_type)='CABLE'))
	left join att_details_splitter s on cinfo.destination_system_id=s.system_id  and upper(cinfo.destination_entity_type)='SPLITTER'
	left join layer_details ls on upper(cinfo.source_entity_type)=upper(ls.layer_name) and (ls.isvisible=true or ls.is_visible_in_ne_library=true) --and ls.is_virtual_port_allowed=false
	left join layer_details ld on upper(cinfo.destination_entity_type)=upper(ld.layer_name) and (ld.isvisible=true or ld.is_visible_in_ne_library=true) --and ld.is_virtual_port_allowed=false;
	left join layer_details srcsubtype 
	on upper(cinfo.source_entity_sub_type)=upper(srcsubtype.layer_name) and (srcsubtype.isvisible=true or srcsubtype.is_visible_in_ne_library=true) 
	and srcsubtype.is_middleware_entity=true

	left join layer_details dstsubtype 
	on upper(cinfo.destination_entity_sub_type)=upper(dstsubtype.layer_name) and (dstsubtype.isvisible=true or dstsubtype.is_visible_in_ne_library=true) 
	and dstsubtype.is_middleware_entity=true;
	
	--UPDATE THE CABLE CALCULATE AND GIS LENGTH--
	with cte as(
	select *,row_number() over(PARTITION BY a.source_system_id,a.source_entity_type ORDER BY a.connection_id) rn from(
	select t1.connection_id, t1.source_system_id,t1.source_entity_type from cpf_temp t1 where upper(t1.source_entity_type)='CABLE'
	union
	select t2.connection_id,t2.destination_system_id,t2.destination_entity_type from cpf_temp t2 where upper(t2.destination_entity_type)='CABLE')a)
	update cpf_temp set cable_calculated_length=0, cable_measured_length=0 where cpf_temp.connection_id in(select cte.connection_id from cte where rn>1);

	--********************* DECIDE DOWNSTREAM AND UPSTREAM ************************----

	-- IF SEARCHED ENTITY IS  CUSTOMER THEN DOWN STREAM END ALWAYS SHOULD BE B
	if(coalesce(v_down_stream_end,'')='' and upper(p_entity_type)='CUSTOMER') then
			v_down_stream_end:='B';
	 end if;

	if(coalesce(v_down_stream_end,'')='' and upper(p_entity_type)='FMS' and 
		(select count(1) from att_details_fms where system_id=p_entity_system_id and parent_entity_type in ('POD','MPOD'))>0) then
		v_down_stream_end:='B';
	end if;

-- CHECK wheather FMS TRACE END VALUE is A
	if(coalesce(v_down_stream_end,'')='' and (select count(1) from cpf_temp cinfo 
		where cinfo.trace_end='A' and ((upper(cinfo.source_entity_type)='FMS' and (upper(cinfo.source_network_id) like '%POD%' or upper(cinfo.source_network_id) like '%MOD%' )) 
		or (upper(cinfo.destination_entity_type)='FMS' and (upper(cinfo.destination_network_id) like '%POD%' or upper(cinfo.destination_network_id) like '%MOD%' )))
		and  upper(cinfo.source_entity_type)!=upper(cinfo.destination_entity_type))>0) then
			v_down_stream_end:='B';
	end if;

     -- CHECK wheather FMS TRACE END VALUE is B
	if(coalesce(v_down_stream_end,'')='' and (select count(1) from cpf_temp cinfo 
		where cinfo.trace_end='B' and ((upper(cinfo.source_entity_type)='FMS' and (upper(cinfo.source_network_id) like '%POD%' or upper(cinfo.source_network_id) like '%MOD%' )) 
		or (upper(cinfo.destination_entity_type)='FMS' and (upper(cinfo.destination_network_id) like '%POD%' or upper(cinfo.destination_network_id) like '%MOD%' )))
		and  upper(cinfo.source_entity_type)!=upper(cinfo.destination_entity_type))>0) then
			v_down_stream_end:='A';
	end if;

	-- CHECK ON WHICH END MULTILEVEL CHILDS ARE EXIST..
	if(coalesce(v_down_stream_end,'')='') then
		select max(cinfo.trace_end) into v_down_stream_end  from cpf_temp cinfo where upper(cinfo.source_entity_type)!=upper(cinfo.destination_entity_type)
		group by cinfo.rowid,cinfo.trace_end having count(1)>1 order by cinfo.trace_end limit 1;
	end if;

	--CHECK CUSTOMER IN A END OR B-END
	if(coalesce(v_down_stream_end,'')='') then
		select cinfo.trace_end into v_down_stream_end from cpf_temp cinfo where ((upper(cinfo.source_entity_type)='CUSTOMER') 
		or (upper(cinfo.destination_entity_type)='CUSTOMER')) limit 1;
	end if;

	-- IF NO CONDITION IS MATCHED THEN DEFAULT DOWNSTREAM END IS B...
	v_down_stream_end:= case when coalesce(v_down_stream_end,'')!='' then v_down_stream_end else 'B' end;

raise info '-%',v_down_stream_end;

	--*********************END************************----

if((select count(1) from cpf_temp ci where ci.trace_end='A' and upper(ci.source_entity_type)!=upper(ci.destination_entity_type) and ((ci.source_system_id=p_entity_system_id and ci.source_port_no =p_entity_port_no and lower(ci.source_entity_type) = lower(p_entity_type)) or (ci.destination_system_id=p_entity_system_id and ci.destination_port_no =p_entity_port_no and 
			lower(ci.destination_entity_type) = lower(p_entity_type))))>1
			and upper(p_entity_type)!='EQUIPMENT') then
--RING  CONNECTIVITY... RETURN ONLY UPSTREAM ..
return query
	select  ROW_NUMBER() OVER (order by cinfo.is_backward_path,cinfo.rno)::integer as id, cinfo.connection_id,cinfo.rowid,cinfo.parent_connection_id,cinfo.source_system_id,
        cinfo.source_network_id,cinfo.source_entity_type ,cinfo.source_entity_title,
        cinfo.source_port_no,cinfo.is_source_virtual,cinfo.destination_system_id,cinfo.destination_network_id,
        cinfo.destination_entity_type,cinfo.destination_entity_title,
        cinfo.destination_port_no,cinfo.is_destination_virtual,cinfo.is_customer_connected,cinfo.created_on,cinfo.created_by,
	cinfo.approved_by,cinfo.approved_on,cinfo.cable_calculated_length,cinfo.cable_measured_length,cinfo.cable_network_status,
	cinfo.splitter_ratio,cinfo.is_backward_path,cinfo.trace_end,cinfo.source_display_name,cinfo.destination_display_name,
	cinfo.source_tray_system_id,cinfo.destination_tray_system_id,cinfo.source_tray_display_name,cinfo.destination_tray_display_name,
	cinfo.source_entity_sub_type,cinfo.destination_entity_sub_type
	from (
		select  ROW_NUMBER() OVER (order by a.level) as rno, a.* ,true as is_backward_path from cpf_temp a where a.trace_end='A'
	) cinfo 
	order by cinfo.is_backward_path desc,cinfo.rno;

else

	return query
	select  ROW_NUMBER() OVER (order by cinfo.is_backward_path,cinfo.rno)::integer as id, cinfo.connection_id,cinfo.rowid,cinfo.parent_connection_id,cinfo.source_system_id,
        cinfo.source_network_id,
        cinfo.source_entity_type,
        cinfo.source_entity_title
        ,cinfo.source_port_no,cinfo.is_source_virtual,cinfo.destination_system_id,cinfo.destination_network_id,
        cinfo.destination_entity_type,
        cinfo.destination_entity_title
        ,cinfo.destination_port_no,cinfo.is_destination_virtual,cinfo.is_customer_connected,cinfo.created_on,cinfo.created_by,
	cinfo.approved_by,cinfo.approved_on,cinfo.cable_calculated_length,cinfo.cable_measured_length,cinfo.cable_network_status,
	cinfo.splitter_ratio,cinfo.is_backward_path,cinfo.trace_end,cinfo.source_display_name,cinfo.destination_display_name,
	cinfo.source_tray_system_id,cinfo.destination_tray_system_id,cinfo.source_tray_display_name,cinfo.destination_tray_display_name,
	cinfo.source_entity_sub_type,cinfo.destination_entity_sub_type
	from (
		select  ROW_NUMBER() OVER (order by a.level) as rno, a.* ,true as is_backward_path from cpf_temp a where a.trace_end!=v_down_stream_end
		union
		select   ROW_NUMBER() OVER (order by b.level) as rno, b.*,false as is_backward_path from cpf_temp b where b.trace_end=v_down_stream_end
	) cinfo 
	order by cinfo.is_backward_path desc,cinfo.rno;
end if;
	
END;
$BODY$;

ALTER FUNCTION public.fn_get_fms_connection_path(character varying, character varying, integer, integer, character varying, character varying, integer, integer, character varying)
    OWNER TO postgres;
-----------------------------------------------------------------------------------------------------------------------------------------------------------------