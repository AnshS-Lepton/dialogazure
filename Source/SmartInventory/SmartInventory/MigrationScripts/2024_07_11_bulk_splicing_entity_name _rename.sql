DROP TABLE IF EXISTS public.temp_connection_info;

CREATE TABLE IF NOT EXISTS public.temp_connection_info
(
    connection_id integer,
    source_system_id integer DEFAULT 0,
    source_network_id character varying COLLATE pg_catalog."default",
    source_entity_type character varying COLLATE pg_catalog."default",
    source_port_no integer,
    destination_system_id integer DEFAULT 0,
    destination_network_id character varying COLLATE pg_catalog."default",
    destination_entity_type character varying COLLATE pg_catalog."default",
    destination_port_no integer,
    is_customer_connected boolean,
    created_on timestamp without time zone,
    created_by integer,
    approved_by integer,
    approved_on timestamp without time zone,
    splicing_source character varying COLLATE pg_catalog."default",
    is_cable_a_end boolean,
    source_entity_sub_type character varying COLLATE pg_catalog."default",
    destination_entity_sub_type character varying COLLATE pg_catalog."default",
    source_display_name character varying COLLATE pg_catalog."default",
    destination_display_name character varying COLLATE pg_catalog."default",
    source_tray_system_id integer,
    destination_tray_system_id integer,
    source_tray_display_name character varying COLLATE pg_catalog."default",
    destination_tray_display_name character varying COLLATE pg_catalog."default",
    is_through_connection boolean,
    uploaded_by bigint,
    source_port_type character varying COLLATE pg_catalog."default",
    destination_port_type character varying COLLATE pg_catalog."default",
    is_valid boolean,
    error_msg character varying COLLATE pg_catalog."default",
    id serial,
    equipment_system_id integer,
    equipment_network_id character varying COLLATE pg_catalog."default",
    equipment_entity_type character varying COLLATE pg_catalog."default",
    is_source_cable_a_end boolean DEFAULT false,
    is_destination_cable_a_end boolean DEFAULT false
);




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
--UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Equipment does not exist' where is_valid=true and coalesce(EQUIPMENT_SYSTEM_ID,0)=0;

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






CREATE OR REPLACE FUNCTION public.fn_get_export_associate_entity(
	p_system_id integer,
	p_entity_type character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
    sql TEXT;  
    v_layer_table character varying;
    v_entity_name character varying;
BEGIN
    -- Get the layer table name based on the entity type
    SELECT layer_table INTO v_layer_table 
    FROM layer_details 
    WHERE upper(layer_name) = upper(p_entity_type);

    -- Generate the dynamic column name
    v_entity_name := lower(p_entity_type) || '_name';

    -- Construct the SQL query dynamically
    sql := format(
        'SELECT row_to_json(row) 
        FROM (
            SELECT 
                ld.layer_title AS SI_OSP_GBL_GBL_GBL_134,
                tblmain.entity_network_id AS SI_OSP_GBL_GBL_GBL_133,
                tblmain.entity_display_name AS "Display Name",
                CASE 
                    WHEN tblmain.is_termination_point THEN ''YES'' 
                    ELSE ''NO'' 
                END AS SI_GBL_GBL_NET_FRM_194,
                fn_get_date(tblmain.created_on) AS SI_OSP_CAB_NET_RPT_004,
                um.user_name AS SI_OSP_CAB_NET_RPT_003,
                vld.%I AS "Entity name", vld.network_id as "entity_network_id",
                $2 as "Entity Type"
            FROM (
                SELECT 
                    entity_system_id,
                    entity_network_id,
                    entity_type,
                    entity_display_name,
                    ass.created_by,
                    ass.created_on,
                    ass.is_termination_point
                FROM 
                    associate_entity_info ass 
                WHERE  
                    (
                        (ass.entity_system_id = $1 AND upper(ass.entity_type) = upper($2)) 
                        OR 
                        (ass.associated_system_id = $1 AND upper(ass.associated_entity_type) = upper($2))
                    ) 
                UNION
                SELECT 
                    associated_system_id,
                    associated_network_id,
                    associated_entity_type,
                    associated_display_name,
                    ass.created_by,
                    ass.created_on,
                    ass.is_termination_point
                FROM 
                    associate_entity_info ass
                WHERE  
                    (
                        (ass.entity_system_id = $1 AND upper(ass.entity_type) = upper($2)) 
                        OR 
                        (ass.associated_system_id = $1 AND upper(ass.associated_entity_type) = upper($2))
                    )
            ) tblmain 
            LEFT JOIN user_master um ON um.user_id = tblmain.created_by
            LEFT JOIN layer_details ld ON upper(ld.layer_name) = upper(tblmain.entity_type) 
            LEFT JOIN %I vld ON  vld.system_id =  $1
            WHERE 
                tblmain.entity_system_id != $1 
                AND upper(tblmain.entity_type) != upper($2)
        ) row', 
        v_entity_name, v_layer_table);

    -- Execute the dynamic SQL
    RETURN QUERY EXECUTE sql USING p_system_id, p_entity_type;
END;
$BODY$;




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







