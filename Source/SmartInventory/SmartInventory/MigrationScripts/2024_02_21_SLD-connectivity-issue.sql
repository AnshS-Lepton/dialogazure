CREATE OR REPLACE FUNCTION public.fn_get_schematic_view(p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_entity_system_id integer, p_entity_port_no integer, p_entity_type character varying, p_isupstream boolean, p_isconnected_ports boolean)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE nodes text;
edges text;
legends text;
cables text;
arow record;
tube_num integer;
core_num integer;
v_parent_con_id integer;
v_gen integer;
v_middle_row record;
v_rnum integer;
v_parent_connection_id integer;
new_connection_id integer;
middle_via record;
v_parent_connection record;
v_layer_title character varying;
v_display_name_enabled boolean;
BEGIN
v_display_name_enabled:=true;
SELECT COUNT(1)>0 into v_display_name_enabled FROM GLOBAL_SETTINGS WHERE UPPER(KEY)='ISSCHEMATICVIEWDISPLAYNAMEENABLED' AND VALUE='1';


if(upper(p_entity_type)='EQUIPMENT')
then
select m.value into v_layer_title from isp_model_type_master m
inner join att_details_model att on att.model_type_id=m.id and m.model_id=att.model_id and att.system_id=p_entity_system_id;
else 
select layer_title into v_layer_title from layer_details where upper(layer_name)=upper(p_entity_type);
end if;


  
             CREATE TEMP TABLE CPF_TEMP_RESULT
             (
             ID SERIAL,
             RNUM INTEGER,
             ROWID INTEGER,
             CONNECTION_ID INTEGER,
             PARENT_CONNECTION_ID INTEGER,
             SOURCE_SYSTEM_ID INTEGER,
             SOURCE_NETWORK_ID CHARACTER VARYING(100),
             SOURCE_ENTITY_TYPE CHARACTER VARYING(100),
             SOURCE_ENTITY_TITLE CHARACTER VARYING(100),
             SOURCE_PORT_NO INTEGER,
             IS_SOURCE_VIRTUAL BOOLEAN,
             DESTINATION_SYSTEM_ID INTEGER,
             DESTINATION_NETWORK_ID CHARACTER VARYING(100),
             DESTINATION_ENTITY_TYPE CHARACTER VARYING(100),
             DESTINATION_ENTITY_TITLE CHARACTER VARYING(100),
             DESTINATION_PORT_NO INTEGER,
             IS_DESTINATION_VIRTUAL BOOLEAN,
             IS_CUSTOMER_CONNECTED BOOLEAN,
             CREATED_ON TIMESTAMP ,
             CREATED_BY INTEGER,
             APPROVED_BY INTEGER,
             APPROVED_ON TIMESTAMP,
             TRACE_END CHARACTER VARYING(1),
             CABLE_CALCULATED_LENGTH DOUBLE PRECISION,
             CABLE_MEASURED_LENGTH DOUBLE PRECISION,
             SPLITTER_RATIO CHARACTER VARYING(100),
             IS_BACKWARD_PATH BOOLEAN,
             SOURCE_DISPLAY_NAME CHARACTER VARYING,
             DESTINATION_DISPLAY_NAME CHARACTER VARYING,
             VIA_ENTITY_TYPE CHARACTER VARYING(100),
             VIA_ENTITY_NETWORK_ID CHARACTER VARYING(100),
             VIA_ENTITY_DISPLAY_NAME CHARACTER VARYING(100),
             VIA_PORT_NO INTEGER,
             IS_DELETED BOOLEAN,COLOR_CODE TEXT,CABLE_CATEGORY TEXT,CABLE_TYPE TEXT,NETWORK_STATUS TEXT,TOTAL_CORE INTEGER
             ) ON COMMIT DROP;
     
         CREATE TEMP TABLE TEMP_PATH_RESULT(
         ID CHARACTER VARYING,
         ROWID INTEGER,
         CONNECTION_ID INTEGER,
         PARENT_CONNECTION_ID INTEGER,
         SYSTEM_ID INTEGER,
         IS_BACKWARD_PATH BOOLEAN,
         ENTITY_PORT CHARACTER VARYING,
         ENTITY_TYPE CHARACTER VARYING,
         ENTITY_TITLE CHARACTER VARYING,
         ENTITY_DISPLAY_NAME CHARACTER VARYING,
         VIA_ENTITY_TYPE CHARACTER VARYING(100),
         VIA_ENTITY_NETWORK_ID CHARACTER VARYING(100),
         VIA_ENTITY_DISPLAY_NAME CHARACTER VARYING,
         VIA_PORT_NO INTEGER,
         IS_DELETED BOOLEAN,
         NO_CONNECTION BOOLEAN,COLOR_CODE TEXT,CABLE_CATEGORY TEXT,CABLE_TYPE TEXT,NETWORK_STATUS TEXT,TOTAL_CORE INTEGER,
         ROUNDNESS INTEGER DEFAULT 0,
         IS_MULTI_CONNECTION BOOLEAN DEFAULT FALSE,
         ENTITY_PORT_NO CHARACTER VARYING,
         entity_category CHARACTER VARYING,
         no_of_ports CHARACTER VARYING,
         actual_entity_port integer
         ) ON COMMIT DROP;
        


-- GET CONNECTION INFO DATA AND INSERT INTO TEMP TABLE--
insert into cpf_temp_result(rnum,rowid,connection_id,parent_connection_id,source_system_id,source_network_id,source_entity_type,source_entity_title,
source_port_no,is_source_virtual,destination_system_id,destination_network_id,destination_entity_type,destination_entity_title,destination_port_no,
is_destination_virtual,is_customer_connected,created_on,created_by,approved_by,approved_on,trace_end,cable_calculated_length,cable_measured_length,
splitter_ratio,is_backward_path,source_display_name,destination_display_name,is_deleted)
select row_number() OVER () as rnum,a.rowid,a.connection_id,a.parent_connection_id,a.source_system_id,a.source_network_id,a.source_entity_type,
a.source_entity_title,a.source_port_no,a.is_source_virtual,a.destination_system_id,a.destination_network_id,a.destination_entity_type,
a.destination_entity_title,a.destination_port_no,a.is_destination_virtual,a.is_customer_connected,a.created_on,a.created_by,a.approved_by,a.approved_on,
a.trace_end,a.cable_calculated_length,a.cable_measured_length, a.splitter_ratio,a.is_backward_path,
(case when v_display_name_enabled then a.source_display_name else a.source_network_id end),
(case when v_display_name_enabled then a.destination_display_name else a.destination_network_id end),
false from
fn_get_connection_info(p_searchby, p_searchtext, p_pageno, p_pagerecord,p_sortcolname, p_sorttype, p_entity_system_id, p_entity_port_no,p_entity_type)a where a.is_backward_path=p_isUpstream;




update cpf_temp_result set is_deleted=true 
where upper(source_network_id)=upper(destination_network_id) 
and upper(source_entity_type)=upper(destination_entity_type) 
and upper(source_entity_type)='EQUIPMENT';

FOR AROW IN SELECT * FROM CPF_TEMP_RESULT WHERE IS_DELETED=FALSE and 
(UPPER(SOURCE_ENTITY_TYPE)=UPPER(DESTINATION_ENTITY_TYPE)) and UPPER(SOURCE_ENTITY_TYPE) in('FMS','HTB')ORDER BY ID
LOOP
	IF(UPPER(AROW.SOURCE_ENTITY_TYPE)=UPPER(AROW.DESTINATION_ENTITY_TYPE))
	THEN
		SELECT PARENT_CONNECTION_ID INTO V_PARENT_CON_ID FROM CPF_TEMP_RESULT WHERE ID=AROW.ID;
		UPDATE CPF_TEMP_RESULT SET PARENT_CONNECTION_ID=V_PARENT_CON_ID WHERE PARENT_CONNECTION_ID=AROW.CONNECTION_ID;
		UPDATE CPF_TEMP_RESULT SET IS_DELETED=TRUE WHERE ID=AROW.ID;
	END IF;
 END LOOP;


for v_middle_row in select * from cpf_temp_result where source_network_id=destination_network_id and source_entity_type!=destination_entity_type
loop
	 select connection_id,rnum into v_parent_connection_id,v_rnum from cpf_temp_result where parent_connection_id=v_middle_row.connection_id;
	 update cpf_temp_result set parent_connection_id=v_middle_row.parent_connection_id where parent_connection_id=v_parent_connection_id;	
	 update cpf_temp_result set is_deleted=true where rnum=v_middle_row.rnum;
	 update cpf_temp_result set is_deleted=true where rnum=v_rnum;
 end loop;	



--UPDATE CPF_TEMP_RESULT SET IS_DELETED=TRUE WHERE SOURCE_NETWORK_ID=DESTINATION_NETWORK_ID AND UPPER(SOURCE_ENTITY_TYPE)='EQUIPMENT' AND UPPER(DESTINATION_ENTITY_TYPE)='EQUIPMENT';
--SKIPPING CABLE NODES AND MAINTAINED THE SAME AS VIA INFORMATION --
FOR arow IN select * from cpf_temp_result where is_deleted=false order by id
LOOP
IF ((upper(arow.source_entity_type)='CABLE' or upper(arow.source_entity_type)='PATCHCORD')) 
THEN
	v_gen:=arow.parent_connection_id;
	--Select parent_connection_id into v_parent_con_id from cpf_temp_result where rnum=arow.rnum+1;commented by deepak
	--UPDATE cpf_temp_result SET parent_connection_id=arow.parent_connection_id where rnum=arow.rnum+1;
	UPDATE cpf_temp_result SET parent_connection_id=arow.parent_connection_id where parent_connection_id=arow.connection_id;--v_parent_con_id;
	UPDATE cpf_temp_result SET is_deleted=true where rnum =arow.rnum;

	--FOR MIDDLEWARE ENTITY
	if(upper(arow.destination_entity_type)='EQUIPMENT' 
	and exists(select 1 from cpf_temp_result 
	inner join layer_details ld on upper(ld.layer_name)=upper('FMS') and ld.is_middleware_entity=true
	where destination_system_id=arow.source_system_id and upper(destination_entity_type)='PATCHCORD'))
	then
	
		UPDATE cpf_temp_result SET 
		source_network_id=arow.destination_network_id,
		source_display_name=arow.destination_display_name,
		source_entity_type=arow.destination_entity_type,
		source_entity_title=arow.destination_entity_title,
		source_port_no=arow.destination_port_no 
		where connection_id=arow.parent_connection_id;

--middle_via
		UPDATE cpf_temp_result SET
		via_entity_type=a.via_entity_type,
		via_entity_network_id=a.via_entity_network_id,
		via_entity_display_name=a.via_entity_display_name,
		via_port_no=a.via_port_no
		from (select * from cpf_temp_result where connection_id=arow.parent_connection_id)a 
		where cpf_temp_result.connection_id=a.parent_connection_id;
			
	end if;
--for internal connectivity
	
END IF;


UPDATE cpf_temp_result set color_code =a.color_code,
cable_category=(case when a.color_code is null then null else a.cable_category end),cable_type =a.cable_type,
network_status=a.network_status,total_core=(case when a.color_code is null then null else core_num end)
from 
(
select cbl.system_id,cbl.network_status,cbls.color_code,cbls.cable_category,cbl.cable_type from att_details_cable cbl
left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type and cbls.cable_category=cbl.cable_category and cbl.total_core=cbls.fiber_count 
where cbl.system_id=arow.source_system_id and upper(arow.source_entity_type)='CABLE' 
)a where a.system_id=cpf_temp_result.source_system_id ;


-- IF ((upper(arow.source_entity_type)='HTB' and upper(arow.destination_entity_type)='HTB')
--  --OR (upper(arow.source_entity_type)='EQUIPMENT' and upper(arow.destination_entity_type)='EQUIPMENT')
-- --or (upper(arow.source_entity_type)='FMS' and upper(arow.destination_entity_type)='FMS')
-- ) 
-- THEN
-- 	Select parent_connection_id into v_parent_con_id from cpf_temp_result where rnum=arow.rnum+1;
-- 	--UPDATE cpf_temp_result SET parent_connection_id=arow.parent_connection_id where rnum=arow.rnum+1;
-- 
-- 	UPDATE cpf_temp_result SET parent_connection_id=v_gen where parent_connection_id=v_parent_con_id;
-- 	UPDATE cpf_temp_result SET is_deleted=true where rnum =arow.rnum;
-- END IF;




IF (upper(arow.destination_entity_type)='CABLE')
THEN
	-- GET TUBE AND CORE NUMBER--
	Select tube_number,core_number
	into tube_num,core_num from att_details_cable_info
	where cable_id=arow.destination_system_id and fiber_number=arow.destination_port_no limit(1);


	UPDATE cpf_temp_result set color_code =a.color_code,
	cable_category=(case when a.color_code is null then null else a.cable_category end) ,cable_type =a.cable_type,
	network_status=a.network_status,total_core=(case when a.color_code is null then null else core_num end)
	from 
	(
	select cbl.system_id,cbl.network_status,cbls.color_code,cbls.cable_category,cbl.cable_type from att_details_cable cbl
	left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type and cbls.cable_category=cbl.cable_category  and cbl.total_core=cbls.fiber_count
	where cbl.system_id=arow.destination_system_id
	)a where a.system_id=cpf_temp_result.destination_system_id;


	-- UPDATE cpf_temp_result set color_code =cbls.color_code,cable_category=cbl.cable_category ,cable_type =cbl.cable_type,
	-- network_status=cbl.network_status,total_core=core_num
	-- from 
	-- att_details_cable cbl
	-- left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type and cbls.cable_category=cbl.cable_category
	-- where cbl.system_id=arow.destination_system_id;
	-- 
	-- select core_num from cable_color_settings limit 1


	-- UPDATE VIA TEXT--
	UPDATE cpf_temp_result 
	SET via_entity_type=arow.destination_entity_type,
	via_entity_network_id=arow.destination_network_id,
	via_entity_display_name=
	CONCAT(arow.destination_display_name
	,'(',coalesce((select Cast(cable_calculated_length as decimal(10,2)) 
	from att_details_cable where system_id=arow.destination_system_id),0),'m)','(T',tube_num,'-F',core_num,' )'),
	via_port_no=arow.destination_port_no 
	where rnum=arow.rnum;

	-- select * into v_parent_connection from cpf_temp_result where parent_connection_id=arow.connection_id;
-- 	if(upper(v_parent_connection.source_entity_type)=upper(arow.destination_entity_type))
-- 	then
-- 		update cpf_temp_result set
-- 		destination_network_id=v_parent_connection.destination_network_id,
-- 		destination_display_name=v_parent_connection.destination_display_name,
-- 		destination_entity_type=v_parent_connection.destination_entity_type,
-- 		destination_entity_title=v_parent_connection.destination_entity_title,
-- 		destination_port_no=v_parent_connection.destination_port_no 
-- 		where connection_id=arow.connection_id;
-- 		UPDATE cpf_temp_result set is_deleted=true where connection_id=v_parent_connection.connection_id;
-- 	end if;
END IF;

IF (upper(arow.destination_entity_type)='PATCHCORD' or upper(arow.destination_entity_title)='PATCH CORD')
THEN
-- UPDATE VIA TEXT--
UPDATE cpf_temp_result SET via_entity_type=arow.destination_entity_type,
via_entity_network_id=arow.destination_network_id,
via_entity_display_name=arow.destination_display_name 
where rnum=arow.rnum;
END IF;

-- IF(UPPER(AROW.SOURCE_NETWORK_ID)=UPPER(AROW.DESTINATION_NETWORK_ID) 
-- AND (SELECT COUNT(1) FROM LAYER_DETAILS LD WHERE LD.IS_MIDDLEWARE_ENTITY=TRUE AND UPPER(LD.LAYER_NAME)=(AROW.SOURCE_ENTITY_TYPE) OR LD.LAYER_NAME=UPPER(AROW.DESTINATION_ENTITY_TYPE))>0)
-- THEN
-- 	UPDATE cpf_temp_result SET IS_DELETED=TRUE where rnum=arow.rnum;
-- END IF;

END LOOP;


-- TRANSFORM CONNECTION DATA (SOURCE & DESTINATIONS) INTO SCHEMATIC VIEW FORMAT--
insert into temp_path_result(rowid,connection_id,parent_connection_id,is_backward_path,id,entity_port,entity_type,entity_title,entity_display_name,via_entity_type,
via_entity_network_id,via_entity_display_name,via_port_no,no_connection,system_id,color_code,cable_category,cable_type,network_status,total_core,roundness,is_multi_connection,entity_category,no_of_ports,actual_entity_port)

select a.rowid,a.connection_id,a.parent_connection_id,a.is_backward_path,a.id,a.entity_port,a.entity_type,a.entity_title,a.entity_display_name,a.via_entity_type,
a.via_entity_network_id,a.via_entity_display_name,a.via_port_no,a.no_connection,a.system_id,a.color_code,a.cable_category,a.cable_type,a.network_status,a.total_core,
(case when (row_number() over(partition by a.parent_connection_id,a.connection_id))>1 
then (row_number() over(partition by a.parent_connection_id,a.connection_id))*0.4 else 0 end ),
(row_number() over(partition by a.parent_connection_id,a.connection_id))>1,
coalesce(pm.entity_category,''),
(case when UPPER(ld.LAYER_NAME) NOT IN('CUSTOMER') THEN pm.no_of_ports END),
--(case when ld.is_virtual_port_allowed=false then pm.no_of_ports else null end),
a.actual_entity_port	
 from(
select distinct t1.rowid,t1.connection_id,t1.parent_connection_id,t1.is_backward_path,
(case when t2.connection_id is null then t1.connection_id::character varying||'123' else t1.connection_id::character varying end) as id,
case when t2.connection_id is null and upper(t1.destination_entity_type) not in('CABLE','PATCHCORD')
then fn_get_port_text(t1.destination_system_id,t1.is_destination_virtual,t1.destination_port_no,t1.destination_entity_type)
else fn_get_port_text(t1.source_system_id,t1.is_source_virtual,t1.source_port_no,t1.source_entity_type)
end as entity_port,

case when t2.connection_id is null and upper(t1.destination_entity_type) not in('CABLE','PATCHCORD') then t1.destination_entity_type
else t1.source_entity_type end as entity_type,
case when t2.connection_id is null and upper(t1.destination_entity_type) not in('CABLE','PATCHCORD') then t1.destination_entity_title
else t1.source_entity_title end as entity_title,

case when t2.connection_id is null and upper(t1.destination_entity_type) not in('CABLE','PATCHCORD') then t1.destination_display_name
else t1.source_display_name end as entity_display_name,


t1.via_entity_type,t1.via_entity_network_id,t1.via_entity_display_name,t1.via_port_no,
(case when t2.connection_id is null then true else false end) as no_connection,
case when t2.connection_id is null and upper(t1.destination_entity_type) not in('CABLE','PATCHCORD') then t1.destination_system_id
else t1.source_system_id end as system_id,
t2.color_code,t2.cable_category,t2.cable_type,coalesce(t2.network_status,t1.network_status) as network_status,t2.total_core,
case when t2.connection_id is null and upper(t1.destination_entity_type) not in('CABLE','PATCHCORD')
then t1.destination_port_no else t1.source_port_no end as actual_entity_port
from cpf_temp_result t1
left join cpf_temp_result t2 on t2.parent_connection_id=t1.connection_id and t2.is_deleted=false 
where t1.is_deleted=false order by t1.rowid)a
left join point_master pm on pm.system_id=a.system_id and upper(pm.entity_type)=upper(a.entity_type)
left join layer_details ld on upper(ld.layer_name)=upper(pm.entity_type);

--DELETE THE CONNECTIONS IF ALL OUT PORT IS NOT CONNECTED FOR SPLITTER AND ONT
DELETE FROM TEMP_PATH_RESULT B  
USING
(SELECT * FROM(
SELECT MAX(RN)::TEXT AS MAX_RECORD,SYSTEM_ID,ENTITY_TYPE,TOTAL_OUT FROM(
SELECT SPLIT_PART(NO_OF_PORTS, ':', 2) AS TOTAL_OUT,T1.SYSTEM_ID,T1.ENTITY_TYPE
,ROW_NUMBER() OVER(PARTITION BY SYSTEM_ID,ENTITY_TYPE) RN FROM  TEMP_PATH_RESULT T1 
INNER JOIN LAYER_DETAILS LD ON UPPER(LD.LAYER_NAME)=UPPER(T1.ENTITY_TYPE) AND LD.IS_VIRTUAL_PORT_ALLOWED=FALSE AND UPPER(UNIT_INPUT_TYPE)='IOPDDL' 
WHERE NO_CONNECTION=TRUE AND ACTUAL_ENTITY_PORT>0)A GROUP BY TOTAL_OUT,SYSTEM_ID,ENTITY_TYPE
)B WHERE B.TOTAL_OUT=B.MAX_RECORD) C
 WHERE B.SYSTEM_ID=C.SYSTEM_ID AND
 UPPER(B.ENTITY_TYPE)=UPPER(C.ENTITY_TYPE) AND
 B.ACTUAL_ENTITY_PORT>0;


--DELETE THE CONNECTIONS IF ALL INPUT PORT IS NOT CONNECTED FOR SPLITTER AND ONT
DELETE FROM TEMP_PATH_RESULT B  
USING
(SELECT * FROM(
SELECT MAX(RN)::TEXT AS MAX_RECORD,SYSTEM_ID,ENTITY_TYPE,TOTAL_OUT FROM(
SELECT SPLIT_PART(NO_OF_PORTS, ':', 1) AS TOTAL_OUT,T1.SYSTEM_ID,T1.ENTITY_TYPE
,ROW_NUMBER() OVER(PARTITION BY SYSTEM_ID,ENTITY_TYPE) RN FROM  TEMP_PATH_RESULT T1 
INNER JOIN LAYER_DETAILS LD ON UPPER(LD.LAYER_NAME)=UPPER(T1.ENTITY_TYPE) AND LD.IS_VIRTUAL_PORT_ALLOWED=FALSE AND UPPER(UNIT_INPUT_TYPE)='IOPDDL' 
WHERE NO_CONNECTION=TRUE AND ACTUAL_ENTITY_PORT<0)A GROUP BY TOTAL_OUT,SYSTEM_ID,ENTITY_TYPE
)B WHERE B.TOTAL_OUT=B.MAX_RECORD) C
 WHERE B.SYSTEM_ID=C.SYSTEM_ID AND
 UPPER(B.ENTITY_TYPE)=UPPER(C.ENTITY_TYPE) AND
 B.ACTUAL_ENTITY_PORT<0;

 

IF (p_isConnected_ports=false)
then

	-- STREAM NODES JSON --
	select (select array_to_json(array_agg(row_to_json(x)))from (
	select distinct parent_connection_id as id,
	concat(entity_title,('('||no_of_ports||')'),
	(case when coalesce(entity_port,'')!='' and upper(entity_title)!='CUSTOMER' then '('||entity_port||')' else '' end ),
	'\n',entity_display_name)as label,
	concat(coalesce(entity_category,''),entity_type) as group,null as title
	from temp_path_result where no_connection =false
	union
	-- FETCHING THE BLANK NODES SEPERATLY 
	--(NOT CONNECTED FURTHER i.e NO_CONNECTION=TRUE).. 'iopddl' IS USED TO FILTER THE RATIO BASED PORT DEVICES.
	
	select distinct t1.connection_id as id,
	concat(entity_title,('('||no_of_ports||')'),
	(case when coalesce(entity_port,'')!='' and upper(entity_title)!='CUSTOMER' then '('||entity_port||')' else '' end ),
	'\n',entity_display_name)as label
	,concat(coalesce(t1.entity_category,''),t1.entity_type) as group,null as title from temp_path_result t1
	INNER JOIN layer_details t2 on t1.entity_type=t2.layer_name
	where t1.no_connection =true and t2.unit_input_type='iopddl' and t2.geom_type='Point'
	) x) into nodes;

ELSE
	-- STREAM NODES JSON --
	select (select array_to_json(array_agg(row_to_json(x)))from (
	select distinct parent_connection_id as id,
	concat(entity_title,('('||no_of_ports||')'),
	(case when coalesce(entity_port,'')!='' and upper(entity_title)!='CUSTOMER' then '('||entity_port||')' else '' end ),
	'\n',entity_display_name)as label,
	concat(coalesce(entity_category,''),entity_type) as group,null as title
	from temp_path_result t1
	INNER JOIN layer_details t2 on upper(t1.entity_type)=upper(t2.layer_name)
	where no_connection =false
	union
	select distinct parent_connection_id as id,
	concat(entity_title,('('||no_of_ports||')'),
	(case when coalesce(entity_port,'')!='' and upper(entity_title)!='CUSTOMER' then '('||entity_port||')' else '' end )
	,'\n',entity_display_name)as label,
	concat(coalesce(entity_category,''),entity_type) as group,null as title
	from temp_path_result t1
	INNER JOIN layer_details t2 on upper(t1.entity_type)=upper(t2.layer_name)
	where no_connection =true
	and t2.unit_input_type!='iopddl' 
	) x) into nodes;
END IF;



-- STREAM EDGES JSON--
--'<b>',entity_name,'</b>\n <b></b>',
Select (select array_to_json(array_agg(row_to_json(x)))from (
select parent_connection_id as from,connection_id as to,coalesce(via_entity_display_name,'') as label,
CASE WHEN UPPER(via_entity_type)!='PATCHCORD' 
then
case WHEN color_code IS NULL THEN
CASE
WHEN upper(cable_type::text) = 'OVERHEAD'::text THEN '{"color": "#FF0000"}'::json
WHEN upper(cable_type::text) = 'UNDERGROUND'::text THEN '{"color": "#0000FF"}'::json
WHEN upper(cable_type::text) = 'WALL CLAMPED'::text THEN '{"color": "#FD10FD"}'::json
WHEN upper(cable_type::text) = 'ISP'::text THEN '{"color": "#448ee4"}'::json
ELSE '{"color": "#0000FF"}'::json
END
ELSE concat('{"color": "',color_code,'"}')::json
END
ELSE '{"color": "#000"}'::json
END as color,
50 as length,'{"from": {"enabled": false,"scaleFactor": 0.3,"type": "circle"},"to": null}'::json as arrows,
CASE WHEN UPPER(via_entity_type)!='PATCHCORD' and UPPER(via_entity_type)='CABLE'
THEN
CASE
WHEN upper(network_status::text) = 'P'::text THEN true
WHEN upper(network_status::text) = 'A'::text THEN false
ELSE true
END
ELSE false end as dashes,
CASE WHEN UPPER(via_entity_type)='PATCHCORD' then 3 else 1.5 end as width, 
concat('{"type": "curvedCW", "roundness": "',roundness,'"}')::json as smooth
from temp_path_result
) x) into edges;

--LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
select distinct concat(coalesce(entity_category,''),entity_type) as entity_type,(coalesce(entity_category,'')||' '||entity_title) as entity_title,is_backward_path as upstream from temp_path_result where entity_type!='Cable' order by entity_title 
) x) into legends;

--CABLE LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
Select case WHEN color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '#FF0000'::text
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '#0000FF'::text
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '#FD10FD'::text
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '#448ee4'::text
ELSE '#0000FF'::text
END
ELSE t.color_code::text
END as color_code,CONCAT(COALESCE(t.cable_category,''),
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN ' OH'
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN ' UG'
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN ' WC'
WHEN upper(t.cable_type::text) = 'ISP'::text THEN ' ISP'
ELSE ''
END ,
(case when t.total_core is not null then '('||t.total_core||'F)' else '' end) )as text from(
select color_code,cable_category,cable_type,total_core
from temp_path_result
group by color_code,cable_category,cable_type,total_core
)t where t.cable_type is not null
) x) into cables;


-- -- IF CASE OF NULL DATA
    IF COALESCE(nodes,'')=''  THEN
       Select(select array_to_json(array_agg(row_to_json(a)))from (
 	Select p_entity_system_id as id,v_layer_title as label,p_entity_type as group , null as title)a) into nodes;
 	END IF;
	
 IF edges is null THEN
 Select (select array_to_json(array_agg(row_to_json(x)))from (
 select p_entity_system_id as from,0 as to,v_layer_title as label
 ) x) into edges;
 END IF;


 IF COALESCE(legends,'')='' AND UPPER(p_entity_type)!='CABLE' THEN
 select (select array_to_json(array_agg(row_to_json(x)))from (
 	select p_entity_type as entity_type, v_layer_title as entity_title,p_isupstream as is_backward_path
 ) x) into legends;
 END IF;


-- RETURNING ALL ABOVE JSON'S.
return query
select row_to_json(result)
from (select nodes,edges,legends,cables
)result;

END;
$function$
;
