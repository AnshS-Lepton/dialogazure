
CREATE OR REPLACE FUNCTION public.fn_splicing_bulk_upload(
	p_splicing character varying,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
source_record record;
destination_record record;
v_is_source_cable_a_end bool;
v_is_destination_cable_a_end bool;
v_count integer;
sql_query text ;
v_terminationPointsBuffer integer;
v_equipment_geom geometry;
begin

v_is_source_cable_a_end := false;	
v_is_destination_cable_a_end := false;
create temp TABLE temp_connection
(
source_system_id integer,
source_network_id character varying(100),
source_entity_type character varying(100),
source_port_no integer,
destination_system_id integer,
destination_network_id character varying(100),
destination_entity_type character varying(100),
destination_port_no integer,
is_source_cable_a_end boolean NOT NULL DEFAULT false,
is_destination_cable_a_end boolean NOT NULL DEFAULT false,
equipment_system_id integer,
equipment_network_id character varying(100),
equipment_entity_type character varying(100),
created_by integer,
splicing_source character varying(100)
) ON COMMIT DROP ;

select value::integer into v_terminationPointsBuffer from global_settings where key='terminationPointsBuffer';

-- FMS To Cable
if(p_splicing = 'FMS_TO_CABLE') then 

update temp_connection_info set is_source_cable_a_end =a.is_a_end from 
(
select id,st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),v_terminationPointsBuffer)) as is_a_end 
from line_master lm 
inner join temp_connection_info tci on tci.source_entity_type='Cable' and tci.source_system_id = lm.system_id 
and tci.source_entity_type=lm.entity_type
inner join point_master pm on pm.system_id=tci.destination_system_id and pm.entity_type=tci.destination_entity_type 
where tci.is_valid =true and tci.uploaded_by =  p_user_id
)a where  temp_connection_info.id=a.id;

update temp_connection_info set is_destination_cable_a_end =a.is_a_end from 
(
select id,st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),v_terminationPointsBuffer)) as is_a_end 
from line_master lm 
inner join temp_connection_info tci on tci.destination_entity_type='Cable' and tci.destination_system_id = lm.system_id  --273686
and tci.destination_entity_type=lm.entity_type
inner join point_master pm on pm.system_id=tci.source_system_id and pm.entity_type=tci.source_entity_type  
where tci.is_valid =true and tci.uploaded_by = p_user_id
)a where  temp_connection_info.id=a.id;
insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,
                            destination_system_id,destination_network_id,destination_entity_type,destination_port_no,                            
                            is_source_cable_a_end, is_destination_cable_a_end,created_by,splicing_source) 
                    select  source_system_id,source_network_id,source_entity_type,source_port_no,
                            destination_system_id,destination_network_id,destination_entity_type,destination_port_no,
                            is_source_cable_a_end, is_destination_cable_a_end,uploaded_by,'Bulk Splicing' 
                             from temp_connection_info where  is_valid =true;
end if;

--Cable To Cable
if(p_splicing = 'CABLE_TO_CABLE') then

update temp_connection_info set is_source_cable_a_end =a.is_a_end from 
(
select id,st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),v_terminationPointsBuffer)) as is_a_end 
from line_master lm 
inner join temp_connection_info tci on tci.source_entity_type='Cable' and tci.source_system_id = lm.system_id 
and tci.source_entity_type=lm.entity_type
inner join point_master pm on pm.system_id=tci.equipment_system_id and pm.entity_type=tci.equipment_entity_type   
where tci.is_valid =true and tci.uploaded_by = p_user_id
)a where  temp_connection_info.id=a.id;

update temp_connection_info set is_destination_cable_a_end =a.is_a_end from 
(
select id,st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),v_terminationPointsBuffer)) as is_a_end 
from line_master lm 
inner join temp_connection_info tci on tci.destination_entity_type='Cable' and tci.destination_system_id = lm.system_id 
and tci.destination_entity_type=lm.entity_type
inner join point_master pm on pm.system_id=tci.equipment_system_id and pm.entity_type=tci.equipment_entity_type   
where tci.is_valid =true and tci.uploaded_by = p_user_id
)a where  temp_connection_info.id=a.id;

insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,
                            destination_system_id,destination_network_id,destination_entity_type,destination_port_no,
                            is_source_cable_a_end,is_destination_cable_a_end,created_by,splicing_source,                            
                            equipment_system_id,equipment_network_id,equipment_entity_type )                            
                     select source_system_id,source_network_id,source_entity_type,source_port_no,
                            destination_system_id,destination_network_id,destination_entity_type,destination_port_no,
                            is_source_cable_a_end,is_destination_cable_a_end,uploaded_by,'Bulk Splicing',
                            equipment_system_id,equipment_network_id,equipment_entity_type 
                            from temp_connection_info where  is_valid =true;
                            
end if;

--Cable To Cpe
if(p_splicing = 'CABLE_TO_CPE') then

update temp_connection_info set is_source_cable_a_end =a.is_a_end from 
(
select id,st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),20)) as is_a_end 
from line_master lm 
inner join temp_connection_info tci on tci.source_entity_type='Cable' and tci.source_system_id = lm.system_id 
and tci.source_entity_type=lm.entity_type
inner join point_master pm on pm.system_id=tci.destination_system_id and pm.entity_type=tci.destination_entity_type  
where tci.is_valid =true and tci.uploaded_by = p_user_id
)a where  temp_connection_info.id=a.id;

update temp_connection_info set is_destination_cable_a_end =a.is_a_end from 
(
select id,st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),20)) as is_a_end 
from line_master lm 
inner join temp_connection_info tci on tci.destination_entity_type='Cable' and tci.destination_system_id = lm.system_id 
and tci.destination_entity_type=lm.entity_type
inner join point_master pm on pm.system_id=tci.source_system_id and pm.entity_type=tci.source_entity_type   
where tci.is_valid =true and tci.uploaded_by = p_user_id
)a where  temp_connection_info.id=a.id;

insert into temp_connection(source_system_id,source_network_id,source_entity_type,source_port_no,
                            destination_system_id,destination_network_id,destination_entity_type,destination_port_no,
                            is_source_cable_a_end,is_destination_cable_a_end,created_by,
                            splicing_source,equipment_system_id,equipment_network_id,equipment_entity_type) 
                    select  source_system_id,source_network_id,source_entity_type,source_port_no,
                            destination_system_id,destination_network_id,destination_entity_type,destination_port_no,
                            is_source_cable_a_end,is_destination_cable_a_end,uploaded_by,'Bulk Splicing',
                            equipment_system_id,equipment_network_id,equipment_entity_type 
                            from temp_connection_info where  is_valid =true;
end if;
--Cpe To Customer
if(p_splicing = 'CPE_TO_CUSTOMER') then

insert into temp_connection(
                            source_system_id,source_network_id,source_entity_type,source_port_no,
                            destination_system_id,destination_network_id,destination_entity_type,destination_port_no,
                            is_source_cable_a_end,is_destination_cable_a_end,created_by,
                            splicing_source,equipment_system_id,equipment_network_id,equipment_entity_type) 
                    select  source_system_id,source_network_id,source_entity_type,source_port_no,
                            destination_system_id,destination_network_id,destination_entity_type,destination_port_no,
                            false,false,uploaded_by,'Bulk Splicing',
                            equipment_system_id,equipment_network_id,equipment_entity_type 
                            from temp_connection_info where  is_valid =true;
end if;

select count(id) into v_count from temp_connection_info where is_valid =true;
RAISE INFO 'step3 count: %',v_count;
if(v_count >0) then 
perform(fn_auto_provisioning_save_connections());
end if;
	
	return query select row_to_json(row) from ( select true as status, 'Success' as message ) row;

	end;
$BODY$;





CREATE OR REPLACE FUNCTION public.fn_validate_splicing(
	vuserid integer,
	p_splicing_type character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
begin	

-- FOR NEGATIVE/INVALID PORTS
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Source Port no is not valid' where is_valid=true and SOURCE_PORT_NO<0;
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Destination Port no is not valid' where is_valid=true and destination_PORT_NO<0;

-- UPDATE SOURCE_ENTITY_TYPE ON BASIS OF LAYER_TITLE
UPDATE TEMP_CONNECTION_INFO AS TCI SET SOURCE_ENTITY_TYPE = LD.LAYER_NAME
FROM LAYER_DETAILS AS LD WHERE UPPER(LD.LAYER_TITLE) = UPPER(TCI.SOURCE_ENTITY_TYPE) AND TCI.UPLOADED_BY = VUSERID;

-- UPDATE DESTINATION_ENTITY_TYPE ON BASIS OF LAYER_TITLE
UPDATE TEMP_CONNECTION_INFO AS TCI SET DESTINATION_ENTITY_TYPE = LD.LAYER_NAME
FROM LAYER_DETAILS AS LD WHERE UPPER(LD.LAYER_TITLE) = UPPER(TCI.DESTINATION_ENTITY_TYPE) AND TCI.UPLOADED_BY = VUSERID;

-- UPDATE EQUIPMENT_ENTITY_TYPE ON BASIS OF LAYER_TITLE
UPDATE TEMP_CONNECTION_INFO AS TCI SET EQUIPMENT_ENTITY_TYPE = LD.LAYER_NAME
FROM LAYER_DETAILS AS LD WHERE UPPER(LD.LAYER_TITLE) = UPPER(TCI.EQUIPMENT_ENTITY_TYPE) AND TCI.UPLOADED_BY = VUSERID;

--CHECK THE DUPLICATE CONNECTION AVAILABLE OR NOT AT SOURCE
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=B.TOTAL_COUNT>1,error_msg ='Duplicate Entries of Same Source Type not allowed' 
FROM(
SELECT COUNT(1) AS TOTAL_COUNT,SOURCE_ENTITY_TYPE,SOURCE_NETWORK_ID,SOURCE_PORT_NO,SOURCE_SYSTEM_ID,SOURCE_PORT_TYPE FROM TEMP_CONNECTION_INFO 
GROUP BY SOURCE_ENTITY_TYPE,SOURCE_NETWORK_ID,SOURCE_PORT_NO,SOURCE_SYSTEM_ID,SOURCE_PORT_TYPE)B
WHERE B.TOTAL_COUNT>1 
AND A.IS_VALID=TRUE 
AND A.SOURCE_ENTITY_TYPE=B.SOURCE_ENTITY_TYPE 
AND A.SOURCE_NETWORK_ID=B.SOURCE_NETWORK_ID 
AND A.SOURCE_PORT_NO=B.SOURCE_PORT_NO 
AND A.SOURCE_SYSTEM_ID=B.SOURCE_SYSTEM_ID 
AND A.SOURCE_PORT_TYPE=B.SOURCE_PORT_TYPE
AND UPLOADED_BY = VUSERID;

--CHECK THE DUPLICATE CONNECTION AVAILABLE OR NOT AT DESTINATION
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=B.TOTAL_COUNT>1,error_msg ='Duplicate Entries of Same destination Type not allowed' 
FROM(
SELECT COUNT(1) AS TOTAL_COUNT,DESTINATION_ENTITY_TYPE,DESTINATION_NETWORK_ID,DESTINATION_PORT_NO,DESTINATION_SYSTEM_ID,DESTINATION_PORT_TYPE FROM TEMP_CONNECTION_INFO 
GROUP BY DESTINATION_ENTITY_TYPE,DESTINATION_NETWORK_ID,DESTINATION_PORT_NO,DESTINATION_SYSTEM_ID,DESTINATION_PORT_TYPE)B
WHERE B.TOTAL_COUNT>1 
AND A.IS_VALID=TRUE 
AND A.DESTINATION_ENTITY_TYPE=B.DESTINATION_ENTITY_TYPE 
AND A.DESTINATION_NETWORK_ID=B.DESTINATION_NETWORK_ID 
AND A.DESTINATION_PORT_NO=B.DESTINATION_PORT_NO 
AND A.DESTINATION_SYSTEM_ID=B.DESTINATION_SYSTEM_ID 
AND A.DESTINATION_PORT_TYPE=B.DESTINATION_PORT_TYPE 
AND UPLOADED_BY = VUSERID;

--UPDATE THE -VITE FOR SPLITTER AND ONT
UPDATE TEMP_CONNECTION_INFO SET DESTINATION_PORT_NO=-1*DESTINATION_PORT_NO WHERE DESTINATION_PORT_TYPE='I' and destination_entity_type in ('Splitter', 'ONT');
UPDATE TEMP_CONNECTION_INFO SET SOURCE_PORT_NO=-1*SOURCE_PORT_NO WHERE SOURCE_PORT_TYPE='I' and SOURCE_entity_type in ('Splitter', 'ONT');

IF(P_SPLICING_TYPE = 'FMS_TO_CABLE') 
THEN
	UPDATE TEMP_CONNECTION_INFO 
	SET IS_VALID=FALSE ,
	ERROR_MSG='Source/Destination entity type is not valid' 
	where is_valid=true 
	and (SOURCE_ENTITY_TYPE NOT IN('FMS','Cable') or DESTINATION_ENTITY_TYPE NOT IN('FMS','Cable'));	
END IF;

IF(P_SPLICING_TYPE = 'CABLE_TO_CABLE') 
THEN
	UPDATE TEMP_CONNECTION_INFO 
	SET IS_VALID=FALSE ,
	ERROR_MSG='Source/Destination entity type should be splitter/cable' 
	where is_valid=true 
	and (SOURCE_ENTITY_TYPE NOT IN('Splitter','Cable') or DESTINATION_ENTITY_TYPE NOT IN('Splitter','Cable'));
    
--For reverse connectns 
update temp_connection_info set is_valid = false , error_msg ='Invalid Connection Configuration from Source not allowed' 
where concat(source_system_id,source_entity_type,is_source_cable_a_end,source_port_no,source_network_id)
in(concat(destination_system_id,destination_entity_type,is_destination_cable_a_end,destination_port_no,destination_network_id) )
and uploaded_by = vuserid and (error_msg is null or error_msg ='');


update temp_connection_info set is_valid = false , error_msg ='Invalid Connection Configuration from Destination not allowed' 
where concat(destination_system_id,destination_entity_type,is_destination_cable_a_end,destination_port_no,destination_network_id)
in(concat(source_system_id,source_entity_type,is_source_cable_a_end,source_port_no,source_network_id))
and uploaded_by = vuserid and (error_msg is null or error_msg ='');

END IF;

IF(P_SPLICING_TYPE = 'CABLE_TO_CPE') 
THEN
	UPDATE TEMP_CONNECTION_INFO 
	SET IS_VALID=FALSE,
	ERROR_MSG='Source/Destination entity type should be ONT/CPE/HTB/Cable' 
	WHERE IS_VALID=TRUE 
	AND (SOURCE_ENTITY_TYPE NOT IN('ONT','HTB','Cable') or DESTINATION_ENTITY_TYPE NOT IN('ONT','HTB','Cable'));
END IF;

IF(P_SPLICING_TYPE = 'CPE_TO_CUSTOMER') 
THEN
	UPDATE TEMP_CONNECTION_INFO 
	SET IS_VALID=FALSE,
	ERROR_MSG='Source/Destination entity type should be ONT/CPE/HTB/Customer' 
	WHERE IS_VALID=TRUE 
	AND (SOURCE_ENTITY_TYPE NOT IN('ONT','HTB','Customer') or DESTINATION_ENTITY_TYPE NOT IN('ONT','HTB','Customer'));
END IF;

--UPDATE THE SOURCE_SYSTEM_ID FOR POINT ENTITIES
UPDATE TEMP_CONNECTION_INFO A SET SOURCE_SYSTEM_ID=PM.SYSTEM_ID FROM 
POINT_MASTER PM WHERE A.IS_VALID=TRUE AND A.SOURCE_ENTITY_TYPE not in('Cable') 
AND PM.ENTITY_TYPE=A.SOURCE_ENTITY_TYPE AND PM.COMMON_NAME=A.SOURCE_NETWORK_ID;

--UPDATE THE DESTINATION_SYSTEM_ID FOR POINT ENTITIES
UPDATE TEMP_CONNECTION_INFO A SET DESTINATION_SYSTEM_ID=PM.SYSTEM_ID FROM 
POINT_MASTER PM WHERE A.IS_VALID=TRUE AND A.DESTINATION_ENTITY_TYPE not in('Cable') 
AND PM.ENTITY_TYPE=A.DESTINATION_ENTITY_TYPE AND PM.COMMON_NAME=A.DESTINATION_NETWORK_ID;

-- --UPDATE THE EQUIPMENT_SYSTEM_ID FOR EQUIPMENT
-- UPDATE TEMP_CONNECTION_INFO A SET EQUIPMENT_SYSTEM_ID=PM.SYSTEM_ID FROM 
-- POINT_MASTER PM WHERE A.IS_VALID=TRUE AND A.EQUIPMENT_ENTITY_TYPE not in('Cable') 
-- AND PM.ENTITY_TYPE=A.EQUIPMENT_ENTITY_TYPE AND PM.COMMON_NAME=A.EQUIPMENT_NETWORK_ID;

--UPDATE THE SOURCE_SYSTEM_ID FOR CABLE
UPDATE TEMP_CONNECTION_INFO A SET SOURCE_SYSTEM_ID=PM.SYSTEM_ID FROM 
LINE_MASTER PM WHERE A.IS_VALID=TRUE AND A.SOURCE_ENTITY_TYPE in('Cable')
AND PM.ENTITY_TYPE=A.SOURCE_ENTITY_TYPE AND PM.COMMON_NAME=A.SOURCE_NETWORK_ID;

--UPDATE THE DESTINATION_SYSTEM_ID FOR CABLE
UPDATE TEMP_CONNECTION_INFO A SET DESTINATION_SYSTEM_ID=PM.SYSTEM_ID FROM 
LINE_MASTER PM WHERE A.IS_VALID=TRUE AND A.DESTINATION_ENTITY_TYPE in('Cable') 
AND PM.ENTITY_TYPE=A.DESTINATION_ENTITY_TYPE AND PM.COMMON_NAME=A.DESTINATION_NETWORK_ID;

UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Destination does not exist' where is_valid=true and coalesce(DESTINATION_SYSTEM_ID,0)=0;
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Source does not exist' where is_valid=true and coalesce(SOURCE_SYSTEM_ID,0)=0;
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Equipment does not exist' where is_valid=true and coalesce(EQUIPMENT_SYSTEM_ID,0)=0;




UPDATE TEMP_CONNECTION_INFO A 
SET IS_VALID= COALESCE(B.SYSTEM_ID,0)>0 
FROM ISP_PORT_INFO B
WHERE A.SOURCE_ENTITY_TYPE IN ('FMS','ONT','Splitter')
AND A.SOURCE_SYSTEM_ID=B.PARENT_SYSTEM_ID 
AND A.SOURCE_ENTITY_TYPE=B.PARENT_ENTITY_TYPE
AND A.SOURCE_PORT_NO=B.PORT_NUMBER 
AND IS_VALID=TRUE 
AND UPLOADED_BY=VUSERID;

UPDATE TEMP_CONNECTION_INFO 
SET ERROR_MSG='Source entity port is not valid' 
WHERE IS_VALID=FALSE 
--AND DESTINATION_ENTITY_TYPE IN ('FMS','ONT','Splitter') 
AND SOURCE_ENTITY_TYPE IN ('FMS','ONT','Splitter') 
AND UPLOADED_BY = VUSERID 
AND COALESCE(ERROR_MSG,'')='';

UPDATE TEMP_CONNECTION_INFO A 
SET IS_VALID =COALESCE(B.SYSTEM_ID,0)>0 
FROM ISP_PORT_INFO B
WHERE A.DESTINATION_ENTITY_TYPE IN ('FMS','ONT','Splitter')
AND A.DESTINATION_SYSTEM_ID=B.PARENT_SYSTEM_ID 
AND A.DESTINATION_ENTITY_TYPE=B.PARENT_ENTITY_TYPE
AND A.DESTINATION_PORT_NO=B.PORT_NUMBER 
AND IS_VALID=TRUE 
AND UPLOADED_BY = VUSERID;

UPDATE TEMP_CONNECTION_INFO 
SET ERROR_MSG='Destination Port is not valid'
WHERE IS_VALID=FALSE 
AND DESTINATION_ENTITY_TYPE IN ('FMS','ONT','Splitter') 
AND UPLOADED_BY = VUSERID 
AND COALESCE(ERROR_MSG,'')='';


UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=(COALESCE(CON.CONNECTION_ID,0)=0) 
FROM CONNECTION_INFO CON
WHERE A.IS_VALID=TRUE AND CON.DESTINATION_ENTITY_TYPE=A.DESTINATION_ENTITY_TYPE 
AND CON.DESTINATION_NETWORK_ID=A.DESTINATION_NETWORK_ID 
AND CON.DESTINATION_PORT_NO=A.DESTINATION_PORT_NO 
AND CON.DESTINATION_SYSTEM_ID=A.DESTINATION_SYSTEM_ID;

UPDATE TEMP_CONNECTION_INFO A 
SET ERROR_MSG ='Connection already exist' 
WHERE IS_VALID=FALSE 
AND COALESCE(ERROR_MSG,'')='';

UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=(COALESCE(CON.CONNECTION_ID,0)=0) 
FROM CONNECTION_INFO CON
WHERE A.IS_VALID=TRUE AND CON.SOURCE_ENTITY_TYPE=A.SOURCE_ENTITY_TYPE 
AND CON.SOURCE_NETWORK_ID=A.SOURCE_NETWORK_ID 
AND CON.SOURCE_PORT_NO=A.SOURCE_PORT_NO 
AND CON.SOURCE_SYSTEM_ID=A.SOURCE_SYSTEM_ID;

UPDATE TEMP_CONNECTION_INFO A 
SET IS_VALID=FALSE,error_msg ='Connection already exist' 
WHERE IS_VALID=FALSE 
AND COALESCE(ERROR_MSG,'')='';

UPDATE TEMP_CONNECTION_INFO A 
SET IS_VALID =COALESCE(B.CABLE_ID,0)>0
FROM ATT_DETAILS_CABLE_INFO B
WHERE A.SOURCE_ENTITY_TYPE = 'Cable' 
AND  B.CABLE_ID=A.SOURCE_SYSTEM_ID 
AND B.FIBER_NUMBER IN(A.SOURCE_PORT_NO) 
AND IS_VALID=TRUE 
AND UPLOADED_BY = VUSERID;

UPDATE TEMP_CONNECTION_INFO 
SET ERROR_MSG='Source Fiber number is not valid' 
WHERE IS_VALID=FALSE 
AND SOURCE_ENTITY_TYPE = 'Cable' 
AND UPLOADED_BY = VUSERID 
AND COALESCE(ERROR_MSG,'')='';

UPDATE TEMP_CONNECTION_INFO A 
SET IS_VALID =COALESCE(B.CABLE_ID,0)>0 
FROM ATT_DETAILS_CABLE_INFO B
WHERE A.DESTINATION_ENTITY_TYPE = 'Cable' 
AND  B.CABLE_ID=A.DESTINATION_SYSTEM_ID 
AND B.FIBER_NUMBER IN(A.SOURCE_PORT_NO) 
AND IS_VALID=TRUE AND UPLOADED_BY = VUSERID;

UPDATE TEMP_CONNECTION_INFO 
SET ERROR_MSG='Destination Fiber number is not valid' 
WHERE IS_VALID=FALSE 
AND DESTINATION_ENTITY_TYPE = 'Cable'
AND UPLOADED_BY = VUSERID 
AND COALESCE(ERROR_MSG,'')='';

UPDATE TEMP_CONNECTION_INFO A 
SET IS_VALID =COALESCE(B.LAYER_ID,0)>0 
FROM LAYER_DETAILS B
WHERE UPPER(A.SOURCE_ENTITY_TYPE)=UPPER(B.LAYER_NAME) 
AND IS_VALID=TRUE 
AND UPLOADED_BY= VUSERID;

UPDATE TEMP_CONNECTION_INFO 
SET ERROR_MSG='Source entity type is not valid' 
WHERE IS_VALID=FALSE 
AND UPLOADED_BY = VUSERID 
AND COALESCE(ERROR_MSG,'')='';

UPDATE TEMP_CONNECTION_INFO A 
SET IS_VALID =COALESCE(B.LAYER_ID,0)>0 
FROM LAYER_DETAILS B
WHERE UPPER(A.DESTINATION_ENTITY_TYPE)=UPPER(B.LAYER_NAME) 
AND IS_VALID=TRUE 
AND UPLOADED_BY = VUSERID;

UPDATE TEMP_CONNECTION_INFO 
SET ERROR_MSG='Destination entity type is not valid' 
WHERE IS_VALID=FALSE 
AND UPLOADED_BY = VUSERID 
AND COALESCE(ERROR_MSG,'')='';

UPDATE TEMP_CONNECTION_INFO A 
SET IS_VALID =COALESCE(B.LAYER_ID,0)>0 
FROM LAYER_DETAILS B
WHERE UPPER(A.EQUIPMENT_ENTITY_TYPE)=UPPER(B.LAYER_NAME) 
AND IS_VALID=TRUE AND UPLOADED_BY = VUSERID;

UPDATE TEMP_CONNECTION_INFO 
SET ERROR_MSG='Equipment type is not valid' 
WHERE IS_VALID=FALSE 
AND UPLOADED_BY = VUSERID 
AND COALESCE(ERROR_MSG,'')='';

RETURN QUERY
SELECT ROW_TO_JSON(ROW) FROM (SELECT TRUE AS STATUS, 'SUCCESS' AS MESSAGE) ROW;

end;
$BODY$;
