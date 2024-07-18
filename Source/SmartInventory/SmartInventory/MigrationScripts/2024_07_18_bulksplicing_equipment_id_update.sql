CREATE OR REPLACE FUNCTION public.fn_validate_splicing(
	vuserid integer,
	p_splicing_type character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
Declare V_EQUIPMENT_COUNT Integer;
begin	
-- FOR NEGATIVE/INVALID PORTS
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Source Port no is not valid' where is_valid=true and SOURCE_PORT_NO<0 and uploaded_by = vuserid;
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Destination Port no is not valid' where is_valid=true and destination_PORT_NO<0 and uploaded_by = vuserid;

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
UPDATE TEMP_CONNECTION_INFO SET DESTINATION_PORT_NO=-1*DESTINATION_PORT_NO 
WHERE DESTINATION_PORT_TYPE='I' AND destination_entity_type in ('Splitter', 'ONT') AND is_valid=TRUE  AND UPLOADED_BY = VUSERID;

UPDATE TEMP_CONNECTION_INFO SET SOURCE_PORT_NO=-1*SOURCE_PORT_NO 
WHERE SOURCE_PORT_TYPE='I' AND SOURCE_entity_type in ('Splitter', 'ONT') AND IS_VALID=TRUE  AND UPLOADED_BY = VUSERID;

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
AND PM.ENTITY_TYPE=A.SOURCE_ENTITY_TYPE AND PM.COMMON_NAME=A.SOURCE_NETWORK_ID AND A.UPLOADED_BY = vuserid;

--UPDATE THE DESTINATION_SYSTEM_ID FOR POINT ENTITIES
UPDATE TEMP_CONNECTION_INFO A SET DESTINATION_SYSTEM_ID=PM.SYSTEM_ID FROM 
POINT_MASTER PM WHERE A.IS_VALID=TRUE AND A.DESTINATION_ENTITY_TYPE not in('Cable') 
AND PM.ENTITY_TYPE=A.DESTINATION_ENTITY_TYPE AND PM.COMMON_NAME=A.DESTINATION_NETWORK_ID AND A.UPLOADED_BY = vuserid;

--UPDATE THE EQUIPMENT_SYSTEM_ID FOR EQUIPMENT
select count(*) into V_EQUIPMENT_COUNT from TEMP_CONNECTION_INFO A  WHERE A.IS_VALID=TRUE 
AND COALESCE(A.EQUIPMENT_ENTITY_TYPE,'')!='' AND COALESCE(A.EQUIPMENT_NETWORK_ID,'')!='' 
AND A.UPLOADED_BY = vuserid;

IF(V_EQUIPMENT_COUNT>0) THEN   
UPDATE TEMP_CONNECTION_INFO A SET EQUIPMENT_SYSTEM_ID=PM.SYSTEM_ID FROM 
POINT_MASTER PM WHERE A.IS_VALID=TRUE AND A.EQUIPMENT_ENTITY_TYPE not in('Cable') 
AND PM.ENTITY_TYPE=A.EQUIPMENT_ENTITY_TYPE AND PM.COMMON_NAME=A.EQUIPMENT_NETWORK_ID AND A.UPLOADED_BY = vuserid;
END IF;

--UPDATE THE SOURCE_SYSTEM_ID FOR CABLE
UPDATE TEMP_CONNECTION_INFO A SET SOURCE_SYSTEM_ID=PM.SYSTEM_ID FROM 
LINE_MASTER PM WHERE A.IS_VALID=TRUE AND A.SOURCE_ENTITY_TYPE in('Cable')
AND PM.ENTITY_TYPE=A.SOURCE_ENTITY_TYPE AND PM.COMMON_NAME=A.SOURCE_NETWORK_ID AND A.UPLOADED_BY = vuserid;

--UPDATE THE DESTINATION_SYSTEM_ID FOR CABLE
UPDATE TEMP_CONNECTION_INFO A SET DESTINATION_SYSTEM_ID=PM.SYSTEM_ID FROM 
LINE_MASTER PM WHERE A.IS_VALID=TRUE AND A.DESTINATION_ENTITY_TYPE in('Cable') 
AND PM.ENTITY_TYPE=A.DESTINATION_ENTITY_TYPE AND PM.COMMON_NAME=A.DESTINATION_NETWORK_ID AND A.UPLOADED_BY = vuserid;

UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Destination does not exist' 
where is_valid=true and coalesce(DESTINATION_SYSTEM_ID,0)=0 AND A.UPLOADED_BY = vuserid;

UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Source does not exist'
where is_valid=true and coalesce(SOURCE_SYSTEM_ID,0)=0 AND A.UPLOADED_BY = vuserid;

IF(V_EQUIPMENT_COUNT>0) THEN  
UPDATE TEMP_CONNECTION_INFO A SET IS_VALID=false,error_msg ='Equipment does not exist' 
where is_valid=true and coalesce(EQUIPMENT_SYSTEM_ID,0)=0;
End IF;

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











CREATE OR REPLACE FUNCTION public.fn_auto_provisioning_save_connections(
	)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
DECLARE  
  v_equipment_port_no integer;
  v_equipment_system_id integer;
  v_equipment_network_id character varying(100);
  v_equipment_entity_type character varying(100);

  v_source_system_id integer;
  v_source_network_id character varying(100);
  v_source_entity_type character varying(100);
  v_source_port_no integer;
  v_destination_system_id integer;
  v_destination_network_id character varying(100);
  v_destination_entity_type character varying(100);
  v_destination_port_no integer;
  v_is_source_cable_a_end boolean  DEFAULT false;
  v_is_destination_cable_a_end boolean  DEFAULT false;
  v_created_by integer;
  v_splicing_source character varying(100);
  curgeommapping refcursor;

  counter INTEGER;
  V_SPL_INPUT_PORT INTEGER;
  V_SPL_OUTPUT_PORT INTEGER;

BEGIN
--v_splicing_source:='Mobile';

v_equipment_port_no:=1;
v_is_source_cable_a_end:=false;
v_is_destination_cable_a_end:=false;
counter:=0;

CREATE TEMP TABLE temp_connections
(
  id serial,
  source_system_id integer,
  source_network_id character varying(100),
  source_entity_type character varying(100),
  source_port_no integer,
  destination_system_id integer,
  destination_network_id character varying(100),
  destination_entity_type character varying(100),
  destination_port_no integer,
  is_cable_a_end boolean, 		
  created_by integer,
  splicing_source character varying(100),
  is_reorder_required boolean default false,
  APPROVED_BY INTEGER,
  APPROVED_ON TIMESTAMP WITHOUT TIME ZONE,
  IS_CUSTOMER_CONNECTED BOOLEAN DEFAULT FALSE,
  source_entity_sub_type character varying,
  destination_entity_sub_type character varying,
  source_tray_system_id integer,
  destination_tray_system_id integer,
  is_through_connection boolean DEFAULT false 			
) on commit drop;
  	
OPEN curgeommapping FOR select source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
destination_network_id,destination_entity_type,destination_port_no,coalesce(is_source_cable_a_end,false) as is_source_cable_a_end,coalesce(is_destination_cable_a_end,false) as is_destination_cable_a_end,equipment_system_id,equipment_network_id,
equipment_entity_type,created_by, coalesce(splicing_source,'') as splicing_source from temp_connection;
	LOOP

		truncate table temp_connections;
	
		FETCH  curgeommapping into v_source_system_id,v_source_network_id,v_source_entity_type,v_source_port_no,v_destination_system_id,
		v_destination_network_id,v_destination_entity_type,v_destination_port_no,v_is_source_cable_a_end,v_is_destination_cable_a_end,
		v_equipment_system_id,v_equipment_network_id,v_equipment_entity_type,v_created_by, v_splicing_source ;
		-- EXIT FOR LOOP
		IF NOT FOUND THEN
		EXIT;
		END IF;
     
     IF v_splicing_source = '' THEN  v_splicing_source := 'Mobile'; END IF;

IF(upper(v_source_entity_type)='SPLITTER')
THEN
	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type))=0)
	then
		select split_part(splitter_ratio, ':', 1)::INTEGER  ,split_part(splitter_ratio, ':', 2)::INTEGER  INTO V_SPL_INPUT_PORT,V_SPL_OUTPUT_PORT from att_details_splitter WHERE SYSTEM_ID=v_source_system_id;
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source)
		SELECT v_source_system_id,v_source_network_id,v_source_entity_type,-1*input,v_source_system_id,v_source_network_id,v_source_entity_type,output,false,v_splicing_source from generate_series(1,V_SPL_INPUT_PORT) input  
		cross join generate_series(1,V_SPL_OUTPUT_PORT) output;
	end if;

elsIF(upper(v_destination_entity_type)='SPLITTER')
THEN
	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type))=0)
	then
		select split_part(splitter_ratio, ':', 1)::INTEGER  ,split_part(splitter_ratio, ':', 2)::INTEGER  INTO V_SPL_INPUT_PORT,V_SPL_OUTPUT_PORT from att_details_splitter WHERE SYSTEM_ID=v_destination_system_id;
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source)
		SELECT v_destination_system_id,v_destination_network_id,v_destination_entity_type,-1*input,v_destination_system_id,v_destination_network_id,v_destination_entity_type,output,false,v_splicing_source from generate_series(1,V_SPL_INPUT_PORT) input  
		cross join generate_series(1,V_SPL_OUTPUT_PORT) output;
	end if;

elsif(upper(v_source_entity_type)='ONT')
then
	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type))=0)
	then
		select no_of_input_port,no_of_output_port  INTO V_SPL_INPUT_PORT,V_SPL_OUTPUT_PORT from att_details_ont WHERE SYSTEM_ID=v_source_system_id;
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source)
		SELECT v_source_system_id,v_source_network_id,v_source_entity_type,-1*input,v_source_system_id,v_source_network_id,v_source_entity_type,output,false,v_splicing_source from generate_series(1,V_SPL_INPUT_PORT) input  
		cross join generate_series(1,V_SPL_OUTPUT_PORT) output;
	end if;
	
elsif(upper(v_destination_entity_type)='ONT')
then	
	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type))=0)
	then
		select no_of_input_port,no_of_output_port INTO V_SPL_INPUT_PORT,V_SPL_OUTPUT_PORT from att_details_ont WHERE SYSTEM_ID=v_destination_system_id;
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source)
		SELECT v_destination_system_id,v_destination_network_id,v_destination_entity_type,-1*input,v_destination_system_id,v_destination_network_id,v_destination_entity_type,output,false,v_splicing_source from generate_series(1,V_SPL_INPUT_PORT) input  
		cross join generate_series(1,V_SPL_OUTPUT_PORT) output;
	end if;	
elsif(upper(v_source_entity_type)='FMS')
then
	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) 
	and source_port_no=case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end 
	and  destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type))=0)
	then			
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source)
		values(v_source_system_id,v_source_network_id,v_source_entity_type,(case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end),v_source_system_id,v_source_network_id,v_source_entity_type,
		(case when v_source_port_no<0 then -1*v_source_port_no else v_source_port_no end),false,v_splicing_source);
	end if;	
elsif(upper(v_destination_entity_type)='FMS')
then	
	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) 
	and source_port_no=case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end 
	and  destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type))=0)
	then			
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source)
		values(v_destination_system_id,v_destination_network_id,v_destination_entity_type,(case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end ),v_destination_system_id,v_destination_network_id,
		v_destination_entity_type,(case when v_destination_port_no<0 then -1*v_destination_port_no else v_destination_port_no end ),false,v_splicing_source);
	end if;	
elsif(upper(v_source_entity_type)='HTB')
then
	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) 
	and source_port_no=case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end 
	and  destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type))=0)
	then			
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source)
		values(v_source_system_id,v_source_network_id,v_source_entity_type,(case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end),v_source_system_id,v_source_network_id,v_source_entity_type,
		(case when v_source_port_no<0 then -1*v_source_port_no else v_source_port_no end),false,v_splicing_source);
	end if;	
elsif(upper(v_destination_entity_type)='HTB')
then	
	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) 
	and source_port_no=case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end 
	and  destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type))=0)
	then			
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source)
		values(v_destination_system_id,v_destination_network_id,v_destination_entity_type,(case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end ),v_destination_system_id,v_destination_network_id,
		v_destination_entity_type,(case when v_destination_port_no<0 then -1*v_destination_port_no else v_destination_port_no end ),false,v_splicing_source);
	end if;	
end if;

	
if(coalesce(v_equipment_entity_type,'') !='')
then
	if( (SELECT COUNT(1) FROM LAYER_DETAILS WHERE IS_VIRTUAL_PORT_ALLOWED=TRUE  AND ISVISIBLE=TRUE AND UPPER(LAYER_NAME)=UPPER(v_equipment_entity_type) AND (IS_SPLICER=TRUE or IS_ISP_SPLICER=TRUE))>0 or UPPER(v_equipment_entity_type)=UPPER('PatchCord'))
	then	
		if(UPPER(v_equipment_entity_type)!=UPPER('PatchCord'))
		then
			--GET THE NEXT PORT OF THE PARENT CONNECTOR	
			select (max(coalesce(destination_port_no))+1) into v_equipment_port_no from connection_info where destination_system_id=v_equipment_system_id and upper(destination_entity_type)=upper(v_equipment_entity_type);	
			v_equipment_port_no=coalesce(v_equipment_port_no,1);
		end if;

			insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source)
			values(v_source_system_id,v_source_network_id,v_source_entity_type,v_source_port_no,v_equipment_system_id,v_equipment_network_id,v_equipment_entity_type,v_equipment_port_no,v_is_source_cable_a_end,v_created_by,v_splicing_source),
			(v_equipment_system_id,v_equipment_network_id,v_equipment_entity_type,v_equipment_port_no,v_destination_system_id,v_destination_network_id,v_destination_entity_type,v_destination_port_no,v_is_destination_cable_a_end,v_created_by,v_splicing_source); 
			perform(fn_splicing_insert_into_connection_info());		
	end if;
else	
	insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source)
	values(v_source_system_id,v_source_network_id,v_source_entity_type,v_source_port_no,v_destination_system_id,v_destination_network_id,v_destination_entity_type,v_destination_port_no,case when upper(v_destination_entity_type)='CABLE' then v_is_destination_cable_a_end else v_is_source_cable_a_end end,v_created_by,v_splicing_source);

	perform(fn_splicing_insert_into_connection_info());	
end if;

END LOOP;
close curgeommapping;
	
return query
select row_to_json(row) from (
select true as status, 'success'::character varying as message 
 ) row;
 
END
$BODY$;

