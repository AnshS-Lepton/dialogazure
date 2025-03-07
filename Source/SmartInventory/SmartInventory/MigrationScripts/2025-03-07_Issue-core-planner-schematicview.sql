-- FUNCTION: public.fn_splicing_save_osp_split_connection(integer, integer, integer, integer, character varying, character varying, integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_splicing_save_osp_split_connection(integer, integer, integer, integer, character varying, character varying, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_splicing_save_osp_split_connection(
	p_cable_one_system__id integer,
	p_cable_two_system_id integer,
	p_old_cable_system_id integer,
	p_split_entity_system_id integer,
	p_split_entity_network_id character varying,
	p_split_entity_type character varying,
	p_user_id integer,
	p_splicing_source character varying)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
declare 
  arow record; 
  v_cable_one_system_id integer;  
  v_is_cable_one_a_end boolean;  
  v_cable_one_total_core integer;
  v_cable_two_system_id integer;
  v_is_cable_two_a_end boolean;
  v_connection_id integer; 
  v_counter integer := 0;
  v_split_entity_port_no integer;  
  v_split_entity_longlat character varying;
  v_layer_table character varying;
  v_is_virtual_port_allowed boolean;
  V_END_POINT_GEOM GEOMETRY;
v_buffer integer;
V_CABLE_ONE_END RECORD;
V_CABLE_TOW_END RECORD;
p_cable_one_network_id character varying;
p_cable_two_network_id character varying;

begin
V_CABLE_ONE_END:=NULL;
V_CABLE_TOW_END:=NULL;
v_buffer:=0;
  v_split_entity_port_no:=1;
  v_counter:=0; 
  v_is_virtual_port_allowed:=false;	
select value::integer into v_buffer from global_settings where key='SplitCableBuffer';
select is_virtual_port_allowed into v_is_virtual_port_allowed from layer_details where upper(layer_name)=upper(p_split_entity_type);
 --if(v_is_virtual_port_allowed=true)
 --then
 CREATE temp TABLE TEMP_CONNECTIONS
 (
	ID SERIAL,
	SOURCE_SYSTEM_ID INTEGER,
	SOURCE_NETWORK_ID CHARACTER VARYING(100),
	SOURCE_ENTITY_TYPE CHARACTER VARYING(100),
	SOURCE_PORT_NO INTEGER,
	DESTINATION_SYSTEM_ID INTEGER,
	DESTINATION_NETWORK_ID CHARACTER VARYING(100),
	DESTINATION_ENTITY_TYPE CHARACTER VARYING(100),
	DESTINATION_PORT_NO INTEGER,
	IS_CUSTOMER_CONNECTED BOOLEAN DEFAULT FALSE,
	CREATED_ON TIMESTAMP WITHOUT TIME ZONE,
	CREATED_BY INTEGER,
	APPROVED_BY INTEGER,
	APPROVED_ON TIMESTAMP WITHOUT TIME ZONE,
	SPLICING_SOURCE CHARACTER VARYING,
	is_cable_a_end boolean default false,
	is_reorder_required boolean default false,
	source_entity_sub_type character varying,
	destination_entity_sub_type character varying,
	destination_tray_system_id integer default 0,
	source_tray_system_id integer default 0,
	is_through_connection boolean default false
)on commit drop ; 

	--GET CABLE ONE DETAILS
	--select system_id,total_core into v_cable_one_system_id,v_cable_one_total_core from att_details_cable where network_id=p_cable_one_network_id ;  
select system_id,total_core,network_id into v_cable_one_system_id,v_cable_one_total_core,p_cable_one_network_id from att_details_cable where system_id=p_cable_one_system__id ; 
	--GET CABLE TWO DETAILS
	--select system_id INTO v_cable_two_system_id from att_details_cable where network_id=p_cable_two_network_id;
select system_id,network_id INTO v_cable_two_system_id,p_cable_two_network_id from att_details_cable where system_id=p_cable_two_system_id;
	--GET  SPLIT ENTITY DETAILS
	select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_split_entity_type);
	EXECUTE 'select longitude||'' ''||latitude from '||v_layer_table||' where system_id='||p_split_entity_system_id||'' into v_split_entity_longlat;

	--CHECK THE CABLE ONE LEFT A END OR B END NEAR SPLIT ENTITY BUFFER
	SELECT  case when ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer))=true then true else false end into v_is_cable_one_a_end
	from line_master lm where lm.system_id=v_cable_one_system_id  and upper(lm.entity_type)='CABLE' and lm.network_status !='D'  
	and (ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer))
	or
	ST_WITHIN(ST_EndPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer)));

	

	--CHECK THE CABLE TWO LEFT A END OR B END NEAR SPLIT ENTITY BUFFER
	SELECT  case when ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer))=true then true else false end into v_is_cable_two_a_end
	from line_master lm where lm.system_id=v_cable_two_system_id  and upper(lm.entity_type)='CABLE' and lm.network_status !='D'  
	and (ST_WITHIN(ST_StartPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer))
	or
	ST_WITHIN(ST_EndPoint(sp_geometry), ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||v_split_entity_longlat||')',4326),v_buffer)));

raise info 'v_is_cable_one_a_end%',v_is_cable_one_a_end;
raise info 'v_is_cable_two_a_end%',v_is_cable_two_a_end;

	-- REPLACE OLD CABLE LEFT ENTRY WITH NEW CABLE 1 ON LEFT SIDE..

	insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type, 
        source_port_no, destination_system_id, destination_network_id, 
        destination_entity_type, destination_port_no,is_cable_a_end,IS_CUSTOMER_CONNECTED,SPLICING_SOURCE,is_through_connection)

        SELECT  source_system_id, source_network_id,source_entity_type, 
        source_port_no,v_cable_one_system_id,p_cable_one_network_id, 
        destination_entity_type ,destination_port_no,(case when v_is_cable_one_a_end=true then false else true end),IS_CUSTOMER_CONNECTED,'OSP SPLIT',true  
	from connection_info where destination_system_id =p_old_cable_system_id and Upper(destination_entity_type)='CABLE' and is_cable_a_end=(case when v_is_cable_one_a_end=true then false else true end)
	UNION
        SELECT v_cable_one_system_id,p_cable_one_network_id,source_entity_type,source_port_no,
        destination_system_id,destination_network_id,destination_entity_type,destination_port_no,(case when v_is_cable_one_a_end=true then false else true end),IS_CUSTOMER_CONNECTED,'OSP SPLIT',true    
	from connection_info where source_system_id =p_old_cable_system_id and Upper(source_entity_type)='CABLE' and is_cable_a_end=(case when v_is_cable_one_a_end=true then false else true end);

	--NEW CABLE TWO TO OLD ENTITY WHICH IS ON RIGHT HAND SIDE 
	insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type, 
        source_port_no, destination_system_id, destination_network_id, 
        destination_entity_type, destination_port_no,is_cable_a_end,IS_CUSTOMER_CONNECTED,SPLICING_SOURCE,is_through_connection)      
        SELECT  source_system_id, source_network_id,source_entity_type, 
        source_port_no,v_cable_two_system_id,p_cable_two_network_id, 
        destination_entity_type ,destination_port_no,(case when v_is_cable_two_a_end=true then false else true end),IS_CUSTOMER_CONNECTED ,'OSP SPLIT',true   
	from connection_info where destination_system_id =p_old_cable_system_id and Upper(destination_entity_type)='CABLE' and is_cable_a_end=(case when v_is_cable_two_a_end=true then false else true end)	
	UNION
        SELECT v_cable_two_system_id,p_cable_two_network_id,source_entity_type,source_port_no,
        destination_system_id,destination_network_id,destination_entity_type,destination_port_no,(case when v_is_cable_two_a_end=true then false else true end),IS_CUSTOMER_CONNECTED ,'OSP SPLIT',true    
	from connection_info where source_system_id =p_old_cable_system_id and Upper(source_entity_type)='CABLE' and is_cable_a_end=(case when v_is_cable_two_a_end=true then false else true end);

	-- DELETE OLD CABLE CONNECTIONS
        delete from connection_info where (source_system_id=p_old_cable_system_id and upper(source_entity_type)=upper('CABLE')) 
        or (destination_system_id=p_old_cable_system_id and upper(destination_entity_type)=upper('CABLE'));

	perform(fn_splicing_insert_into_connection_info());	
	truncate table TEMP_CONNECTIONS;

	if exists(select 1 from connection_info where destination_system_id=p_split_entity_system_id and upper(destination_entity_type)=upper(p_split_entity_type))
	then
			select (max(coalesce(destination_port_no))) into v_split_entity_port_no from connection_info where destination_system_id=p_split_entity_system_id and upper(destination_entity_type)=upper(p_split_entity_type);	
	
			--select (max(coalesce(destination_port_no))+1) into v_split_entity_port_no from TEMP_CONNECTIONS where destination_system_id=p_split_entity_system_id and upper(destination_entity_type)=upper(p_split_entity_type);
				
	end if;	
		
	if(v_is_virtual_port_allowed=true)
	then
	 LOOP 
	 EXIT WHEN v_cable_one_total_core <= v_counter  ; 
		v_counter := v_counter + 1 ; 
		
		v_split_entity_port_no:=v_split_entity_port_no+1;
		
		v_split_entity_port_no=coalesce(v_split_entity_port_no,1);

		if((select is_virtual_port_allowed from layer_details where upper(layer_name)=upper(p_split_entity_type))=false and (select count(1) from connection_info where source_system_id=p_split_entity_system_id and upper(source_entity_type)=                upper(p_split_entity_type) and source_port_no=(-1*v_counter) and destination_system_id=p_split_entity_system_id and upper(destination_entity_type)=upper(p_split_entity_type) 
		and destination_port_no=v_counter)=0)
		then	truncate table TEMP_CONNECTIONS;
			-- split entity internal connection      
			insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type,source_port_no, destination_system_id, destination_network_id,destination_entity_type, destination_port_no,is_cable_a_end,SPLICING_SOURCE,is_through_connection)
			values(p_split_entity_system_id,p_split_entity_network_id,p_split_entity_type,-1*v_counter,p_split_entity_system_id,p_split_entity_network_id,p_split_entity_type,v_counter,false,'OSP SPLIT',true);
			perform(fn_splicing_insert_into_connection_info());	
		end if;

		truncate table TEMP_CONNECTIONS;
		-- CABLE ONE TO SPLIT ENTITY
		insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type,source_port_no, 
		destination_system_id, destination_network_id,destination_entity_type, destination_port_no,is_cable_a_end,SPLICING_SOURCE,is_through_connection)
		values(v_cable_one_system_id,p_cable_one_network_id,'Cable',v_counter,p_split_entity_system_id,p_split_entity_network_id,p_split_entity_type,v_split_entity_port_no,coalesce(v_is_cable_one_a_end,false),'OSP SPLIT',true);		      	

		--  split entity to  cable two
		insert into TEMP_CONNECTIONS(source_system_id, source_network_id, source_entity_type,source_port_no,destination_system_id, destination_network_id,destination_entity_type, destination_port_no,is_cable_a_end,SPLICING_SOURCE,is_through_connection)
		values(p_split_entity_system_id,p_split_entity_network_id,p_split_entity_type,v_split_entity_port_no,v_cable_two_system_id,p_cable_two_network_id,'Cable',v_counter ,coalesce(v_is_cable_two_a_end,false),'OSP SPLIT',true);

	       perform(fn_splicing_insert_into_connection_info());	
	
	 END LOOP ; 
	end if; 

--CHECK THE FIRST CABLE IS OSP CABLE AND OLD CABLE IS LMC ASSOCIATED
IF EXISTS(SELECT 1 FROM ISP_LINE_MASTER WHERE ENTITY_ID=v_cable_one_system_id AND UPPER(ENTITY_TYPE)=UPPER('CABLE') AND UPPER(CABLE_TYPE)=UPPER('OSP'))
AND EXISTS(SELECT 1 FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=p_old_cable_system_id)
THEN
	SELECT CBL.A_SYSTEM_ID,CBL.A_ENTITY_TYPE,CBL.B_SYSTEM_ID,CBL.B_ENTITY_TYPE INTO V_CABLE_ONE_END FROM ATT_DETAILS_CABLE CBL INNER JOIN ISP_ENTITY_MAPPING MAP 
	ON (CBL.A_SYSTEM_ID=MAP.ENTITY_ID AND UPPER(CBL.A_ENTITY_TYPE)=UPPER(MAP.ENTITY_TYPE)) 
	WHERE CBL.SYSTEM_ID=v_cable_one_system_id;
	IF(V_CABLE_ONE_END IS NOT NULL)
	THEN 
		SELECT SP_GEOMETRY INTO V_END_POINT_GEOM FROM POINT_MASTER WHERE SYSTEM_ID=V_CABLE_ONE_END.B_SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_CABLE_ONE_END.B_ENTITY_TYPE);
		
		UPDATE ATT_DETAILS_LMC_CABLE_INFO 
		SET CABLE_SYSTEM_ID=v_cable_one_system_id,
		RTN_BUILDING_SIDE_TAPPING_LATITUDE=ST_Y(V_END_POINT_GEOM),
		RTN_BUILDING_SIDE_TAPPING_LONGITUDE=ST_X(V_END_POINT_GEOM)  
		WHERE CABLE_SYSTEM_ID=p_old_cable_system_id;
	END IF;

	SELECT CBL.A_SYSTEM_ID,CBL.A_ENTITY_TYPE,CBL.B_SYSTEM_ID,CBL.B_ENTITY_TYPE INTO V_CABLE_TOW_END FROM ATT_DETAILS_CABLE CBL INNER JOIN ISP_ENTITY_MAPPING MAP 
	ON (CBL.B_SYSTEM_ID=MAP.ENTITY_ID AND UPPER(CBL.B_ENTITY_TYPE)=UPPER(MAP.ENTITY_TYPE)) 
	WHERE CBL.SYSTEM_ID=v_cable_one_system_id;
	IF(V_CABLE_TOW_END IS NOT NULL)
	THEN 
		SELECT SP_GEOMETRY INTO V_END_POINT_GEOM FROM POINT_MASTER WHERE SYSTEM_ID=V_CABLE_TOW_END.A_SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_CABLE_TOW_END.A_ENTITY_TYPE);
		
		UPDATE ATT_DETAILS_LMC_CABLE_INFO 
		SET CABLE_SYSTEM_ID=v_cable_one_system_id,
		RTN_BUILDING_SIDE_TAPPING_LATITUDE=ST_Y(V_END_POINT_GEOM),
		RTN_BUILDING_SIDE_TAPPING_LONGITUDE=ST_X(V_END_POINT_GEOM) 
		WHERE CABLE_SYSTEM_ID=p_old_cable_system_id;
	END IF;
END IF;

--CHECK THE FIRST CABLE IS OSP CABLE AND OLD CABLE IS LMC ASSOCIATED
IF EXISTS(SELECT 1 FROM ISP_LINE_MASTER WHERE ENTITY_ID=v_cable_two_system_id AND UPPER(ENTITY_TYPE)=UPPER('CABLE') AND UPPER(CABLE_TYPE)=UPPER('OSP'))
AND EXISTS(SELECT 1 FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=p_old_cable_system_id)
THEN
	SELECT CBL.A_SYSTEM_ID,CBL.A_ENTITY_TYPE,CBL.B_SYSTEM_ID,CBL.B_ENTITY_TYPE INTO V_CABLE_ONE_END FROM ATT_DETAILS_CABLE CBL INNER JOIN ISP_ENTITY_MAPPING MAP 
	ON (CBL.A_SYSTEM_ID=MAP.ENTITY_ID AND UPPER(CBL.A_ENTITY_TYPE)=UPPER(MAP.ENTITY_TYPE)) 
	WHERE CBL.SYSTEM_ID=v_cable_two_system_id;
	IF(V_CABLE_ONE_END IS NOT NULL)
	THEN 
		SELECT SP_GEOMETRY INTO V_END_POINT_GEOM FROM POINT_MASTER WHERE SYSTEM_ID=V_CABLE_ONE_END.B_SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_CABLE_ONE_END.B_ENTITY_TYPE);
		
		UPDATE ATT_DETAILS_LMC_CABLE_INFO 
		SET CABLE_SYSTEM_ID=v_cable_two_system_id,
		RTN_BUILDING_SIDE_TAPPING_LATITUDE=ST_Y(V_END_POINT_GEOM),
		RTN_BUILDING_SIDE_TAPPING_LONGITUDE=ST_X(V_END_POINT_GEOM)  
		WHERE CABLE_SYSTEM_ID=p_old_cable_system_id;
	END IF;

	SELECT CBL.A_SYSTEM_ID,CBL.A_ENTITY_TYPE,CBL.B_SYSTEM_ID,CBL.B_ENTITY_TYPE INTO V_CABLE_TOW_END FROM ATT_DETAILS_CABLE CBL INNER JOIN ISP_ENTITY_MAPPING MAP 
	ON (CBL.B_SYSTEM_ID=MAP.ENTITY_ID AND UPPER(CBL.B_ENTITY_TYPE)=UPPER(MAP.ENTITY_TYPE)) 
	WHERE CBL.SYSTEM_ID=v_cable_two_system_id;
	IF(V_CABLE_TOW_END IS NOT NULL)
	THEN 
		SELECT SP_GEOMETRY INTO V_END_POINT_GEOM FROM POINT_MASTER WHERE SYSTEM_ID=V_CABLE_TOW_END.A_SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_CABLE_TOW_END.A_ENTITY_TYPE);
		
		UPDATE ATT_DETAILS_LMC_CABLE_INFO 
		SET CABLE_SYSTEM_ID=v_cable_two_system_id,
		RTN_BUILDING_SIDE_TAPPING_LATITUDE=ST_Y(V_END_POINT_GEOM),
		RTN_BUILDING_SIDE_TAPPING_LONGITUDE=ST_X(V_END_POINT_GEOM) 
		WHERE CABLE_SYSTEM_ID=p_old_cable_system_id;
	END IF;
END IF;

update att_details_cable_info a set link_system_id=b.link_system_id
from att_details_cable_info b where b.cable_id=p_old_cable_system_id and a.fiber_number=b.fiber_number and a.cable_id=p_cable_one_system__id;

update att_details_cable_info a set link_system_id=b.link_system_id
from att_details_cable_info b where b.cable_id=p_old_cable_system_id and a.fiber_number=b.fiber_number and a.cable_id=p_cable_two_system_id;
	
update att_details_cable_info a 
set a_end_status_id=2
from TEMP_CONNECTIONS b where b.source_system_id=a.cable_id and b.source_entity_type='Cable'
and a.fiber_number=b.source_port_no
and is_cable_a_end=true;

update att_details_cable_info a 
set a_end_status_id=2
from TEMP_CONNECTIONS b where b.destination_system_id=a.cable_id and b.destination_entity_type='Cable'
and a.fiber_number=b.destination_port_no
and is_cable_a_end=true;

update att_details_cable_info a 
set b_end_status_id=2
from TEMP_CONNECTIONS b where b.source_system_id=a.cable_id and b.source_entity_type='Cable'
and a.fiber_number=b.source_port_no
and is_cable_a_end=false;

update att_details_cable_info a 
set b_end_status_id=2
from TEMP_CONNECTIONS b where b.destination_system_id=a.cable_id and b.destination_entity_type='Cable'
and a.fiber_number=b.destination_port_no
and is_cable_a_end=false;



--end if;
    return true;
end;
$BODY$;

ALTER FUNCTION public.fn_splicing_save_osp_split_connection(integer, integer, integer, integer, character varying, character varying, integer, character varying)
    OWNER TO postgres;


---------------------------------------------------------------------------------------------------------


-- FUNCTION: public.fn_get_core_planner_splicing(integer, integer, character varying, character varying, character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_core_planner_splicing(integer, integer, character varying, character varying, character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_splicing(
	required_core integer,
	p_user_id integer,
	fiber_link_network_id character varying,
	source_network_id character varying,
	destination_network_id character varying,
	buffer integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE 
    rec RECORD;
   v_arow RECORD;
    v_network_id character varying;
    v_system_id integer;
    v_cable_details_left record;
    v_cable_details_right record;
    v_cable_details record;
   p_link_system_id integer;
   start_point integer;
  v_status bool;
  v_message  character varying;
V_IS_CABLE_A_END BOOLEAN;
p_destination_network_id character varying;
p_odf_network_id character varying;
BEGIN
V_IS_CABLE_A_END:=FALSE;
 --truncate table TEMP_LINE_MASTER;
 --truncate table TEMP_POINT_MASTER;

CREATE TEMP TABLE TEMP_LINE_MASTER
(
SYSTEM_ID INTEGER,
entity_type character varying,
network_id character varying,
SP_GEOMETRY GEOMETRY
)ON COMMIT DROP; 

CREATE TEMP TABLE TEMP_POINT_MASTER
(
A_SYSTEM_ID INTEGER,
A_entity_type character varying,
A_network_id character varying,
SP_GEOMETRY GEOMETRY
)ON COMMIT DROP;
		
     v_network_id := '';
     v_system_id := 0;
    p_destination_network_id = destination_network_id;
  --  truncate table temp_connection restart identity;
    -- Create a temporary table for connection data
   CREATE TEMP TABLE temp_connection (
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
    ) ON COMMIT DROP;
     
	 	CREATE TEMP TABLE isp_port_info_tbl(
        system_id int4 NULL,
        network_id varchar NULL,
        parent_system_id int4 NULL,
        parent_network_id varchar NULL,
        port_status_id int4 NULL,
        is_valid boolean DEFAULT true,
        port_number int4 null,
        parent_entity_type varchar NULL
    ) ON COMMIT DROP;
	
     SELECT 
        (result_json->>'status')::BOOLEAN, 
        result_json->>'message'
     INTO v_status, v_message
     FROM fn_get_core_planner_validation(source_network_id, destination_network_id, buffer, 
     required_core, p_user_id) AS result_json;
   
     IF v_status = false  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT v_status AS status, v_message AS message) row;
       
     else
     
     -- check if fiber link does not exist 
     IF NOT EXISTS (SELECT 1 FROM att_details_fiber_link 
      WHERE upper(network_id) = upper(fiber_link_network_id) OR upper(link_id) = upper(fiber_link_network_id)) 
      THEN 
      RETURN QUERY 
      SELECT row_to_json(row) 
      FROM (
        SELECT false AS status, 
               'The FiberLink does not exist. Please enter a valid FiberLink.' AS message) row;
              return ;
     END IF;
    
   --  insert record from core_planner_logs table to temp table TEMP_POINT_MASTER,TEMP_LINE_MASTER
      INSERT INTO TEMP_POINT_MASTER(A_SYSTEM_ID,A_network_id,A_entity_type,SP_GEOMETRY )
      select A.a_system_id,P.COMMON_NAME,A.a_entity_type,P.SP_GEOMETRY from (      
      SELECT a_system_id,a_entity_type FROM core_planner_logs  where user_id = p_user_id and is_valid =TRUE
      union
      SELECT b_system_id,b_entity_type FROM core_planner_logs where user_id = p_user_id and is_valid =TRUE )a
      INNER JOIN point_master P ON P.SYSTEM_ID=A.a_system_id AND P.entity_type=A.a_entity_type;

      INSERT INTO TEMP_LINE_MASTER(SYSTEM_ID,network_id,entity_type,SP_GEOMETRY)       
      SELECT CABLE_id,CABLE_network_id,'Cable',P.SP_GEOMETRY FROM core_planner_logs A 
      INNER JOIN LINE_master P ON P.SYSTEM_ID=A.CABLE_id AND P.entity_type='Cable'
      where user_id = p_user_id and is_valid =TRUE ;    
      
    --- get fiberlink into p_link_system_id
      select system_id into p_link_system_id from att_details_fiber_link adfl 
      where upper(network_id) = upper(fiber_link_network_id)
      or upper(link_id)=upper(fiber_link_network_id);  
     
    -- check if odf exists in source or destination store value into p_odf_network_id
    IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id)
     AND EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) 
     THEN
      p_odf_network_id = source_network_id;
    ELSIF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
    THEN
      p_odf_network_id = source_network_id;
    ELSIF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id)
    THEN
      p_odf_network_id = destination_network_id;
    END IF;
  
     -- Populate temporary ISP port info table
    INSERT INTO isp_port_info_tbl (system_id, network_id, parent_system_id, 
    parent_network_id,   port_status_id, port_number,parent_entity_type)
    SELECT system_id, network_id, parent_system_id, parent_network_id, 
    port_status_id, port_number,parent_entity_type
    FROM isp_port_info a 
    WHERE a.parent_network_id=p_odf_network_id
    AND a.parent_entity_type = 'FMS'  
    AND input_output = 'O' and link_system_id =0
	and is_valid_for_core_plan=true 
    ORDER BY port_number limit required_core;
	
        FOR rec IN select * from TEMP_POINT_MASTER  order by a_entity_type
        LOOP									
		    if(rec.a_entity_type='FMS')
		    then
		
			  SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) 
			  AS IS_CABLE_A_END INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
			  WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) or
			  ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)));				    

					INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source)
					
					select rec.a_system_id, rec.a_network_id, rec.a_entity_type, 
                    port_number, 
                    v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    port_number,false,v_cable_details_left.IS_CABLE_A_END, 
                    p_user_id, 'CorePlanning' 
					from isp_port_info_tbl 
					where --is_valid_for_core_plan=true and 
					--parent_system_id = rec.a_system_id and parent_entity_type ='FMS'
					-- and input_output='O' and
					 port_status_id=1
					order by port_number; -- limit required_core; 		   							           		   
		else		

	SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) AS IS_CABLE_A_END 
	INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
	WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
	OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) LIMIT 1;
		
	SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) AS IS_CABLE_A_END 
	INTO v_cable_details_right FROM TEMP_LINE_MASTER A				
	WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
	OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) 
	AND SYSTEM_ID NOT IN(v_cable_details_left.SYSTEM_ID)  LIMIT 1;

		if (v_cable_details_left is not null and v_cable_details_right is not null)
		then

		INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source,equipment_system_id, equipment_network_id, equipment_entity_type)
				
		select v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    port_number, 
                     v_cable_details_right.system_id,  v_cable_details_right.network_id , 'Cable', 
                    port_number,
                    v_cable_details_left.IS_CABLE_A_END,v_cable_details_right.IS_CABLE_A_END, 
                    p_user_id, 'CorePlanning',rec.a_system_id,rec.a_network_id,rec.a_entity_type 
					from isp_port_info 
					
					inner join att_details_cable_info c 
					on c.cable_id=v_cable_details_left.system_id and c.fiber_number=port_number
					and ((v_cable_details_left.IS_CABLE_A_END=true and a_end_status_id = 1)
 					  or(v_cable_details_left.IS_CABLE_A_END =false and b_end_status_id = 1)
 				
					--and ((v_cable_details_left.IS_CABLE_A_END=true and is_a_end_through_connectivity = false)
 					 -- or(v_cable_details_left.IS_CABLE_A_END =false and is_b_end_through_connectivity = false)
 				     )
 				    
					where is_valid_for_core_plan=true 
					and parent_network_id = p_odf_network_id
					and input_output='O' --and port_status_id=1					
					order by port_number limit required_core;							    		                 	
		end if;   
	end if;
        END LOOP;
	
	    update att_details_cable_info set  fiber_status ='Reserved',
		link_system_id=coalesce(p_link_system_id,0)
		from(
		select * 
		from isp_port_info_tbl 
		where --is_valid_for_core_plan=true and 
		parent_network_id =p_odf_network_id
		-- and input_output='O' and port_status_id=1					
		order by port_number -- limit required_core
		)b
		where cable_id in (select system_id from TEMP_LINE_MASTER) 
		and fiber_number=b.port_number;

		update att_details_fiber_link set fiber_link_status='Associated',
		gis_length = ROUND(
        COALESCE((SELECT Sum(a.cable_measured_length) FROM att_details_cable a
        inner join TEMP_LINE_MASTER b on a.system_id =b.system_id and b.entity_type ='Cable' ), 0
        )::NUMERIC, 3
    ),
    total_route_length = ROUND(
        COALESCE(
            (SELECT sum(a.cable_calculated_length) FROM att_details_cable a
            inner join TEMP_LINE_MASTER b on a.system_id =b.system_id and b.entity_type ='Cable' ), 0
        )::NUMERIC, 3
    ) 
		where system_id=coalesce(p_link_system_id,0);
			
	--- update odf is_valid_for_core_plan,link_system_id
        IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
		then
		update isp_port_info set link_system_id=coalesce(p_link_system_id,0)
		where parent_network_id =source_network_id
		and input_output='O'-- and port_status_id=1 
		and port_number in (select port_number 
		from isp_port_info_tbl 
		---where parent_network_id =source_network_id
		--and input_output='O' -- and port_status_id=1					
		order by port_number limit required_core);
		
		update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id =source_network_id
		and parent_entity_type='FMS' 
		--and port_number=v_arow.port_number 
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;
	
	
       end if;
      
      IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = p_destination_network_id) 
	  then
	    update isp_port_info set link_system_id=coalesce(p_link_system_id,0)
		where parent_network_id =p_destination_network_id
		and input_output='O' --and port_status_id=1 
		and port_number in (select port_number 
		from isp_port_info_tbl 
		-- where parent_network_id = p_destination_network_id
		-- and input_output='O' --and port_status_id=1					
		order by port_number limit required_core);
		
	   update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id = p_destination_network_id
		and parent_entity_type='FMS' 
		--and port_number=v_arow.port_number 
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;	
			
       end if;
 
 
if (SELECT COUNT(*) FROM temp_connection) > 0
then 
 raise info 'end splicing 1 : %',p_user_id;
perform(fn_auto_provisioning_save_connections());

-- FOR rec in (select destination_system_id,destination_port_no from temp_connection where destination_entity_type = 'Cable' and created_by = p_user_id
-- union 
-- select source_system_id, source_port_no from temp_connection where source_entity_type = 'Cable' and created_by = p_user_id)
-- loop 
-- 
-- perform fn_associate_fiber_link_cable(rec.destination_system_id,coalesce(p_link_system_id,0),rec.destination_port_no,'A' );
-- 	
-- end loop;

end if;	

  --    update core_planner_logs set used_core = used_core + required_core,avaiable= case when avaiable - required_core < 0 
   --   then 0 else avaiable - required_core end where user_id = p_user_id and is_valid =true;

       delete from core_planner_logs where user_id = p_user_id;

       return query select row_to_json(row) from ( select true as status, 'The required core has been spliced, and the fiber link has been successfully attached.' as message ) row;
      end if;
  END;
 
$BODY$;

ALTER FUNCTION public.fn_get_core_planner_splicing(integer, integer, character varying, character varying, character varying, integer)
    OWNER TO postgres;


--------------------------------------------------------------------------------------------------

-- FUNCTION: public.fn_get_core_planner_validation(character varying, character varying, integer, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_core_planner_validation(character varying, character varying, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation(
	source_network_id character varying,
	destination_network_id character varying,
	buffer integer,
	required_core integer,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE 
    rec RECORD;
    total_core integer;
    p_source character varying;
    p_destination character varying;
    availableCoreDestination integer;
    usedCore integer;
    p_source_system_id integer;
    p_destination_system_id integer;
    availableCoreSource integer;
   t_count integer;
  cableNetworkId character varying;
BEGIN
    availableCoreDestination := 0;
    usedCore := 0;
    t_count :=0;
   
      --truncate table temp_cable_routes; 
      
    -- Create temporary tables
       CREATE temp TABLE temp_cable_routes(
        seq integer,
        path_seq integer, 
        edge_targetid integer, 
        roadline_geomtext text, 
        start_point character varying,
        end_point character varying,
        message varchar NULL,
        is_valid boolean DEFAULT true,
        avaiable_core_count integer DEFAULT 0,
        user_id integer
    ) ON COMMIT DROP;

    CREATE TEMP TABLE temp_isp_port_info(
        system_id int4 NULL,
        network_id varchar NULL,
        parent_system_id int4 NULL,
        parent_network_id varchar NULL,
        port_status_id int4 NULL,
       -- is_valid boolean DEFAULT true,
        port_number int4 null,
        parent_entity_type varchar NULL,
		is_valid boolean not null default true
    ) ON COMMIT DROP;

	CREATE TEMP TABLE temp_route_connection (
		id serial,
		connection_id int4,
		source_system_id int4 NULL,
		source_network_id varchar(100) NULL,
		source_entity_type varchar(100) NULL,
		source_port_no int4 NULL,
		destination_system_id int4 NULL,
		destination_network_id varchar(100) NULL,
		destination_entity_type varchar(100) NULL,
		destination_port_no int4 NULL	
	)ON COMMIT DROP;

    -- Clear previous logs for the user
    DELETE FROM core_planner_logs WHERE user_id = p_user_id;
   
   -- Get source and destination points
   -- Check if source exists in FMS, otherwise get it source from SpliceClosure
   IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) THEN
     SELECT longitude || ' ' || latitude AS geom, system_id
     INTO p_source, p_source_system_id
     FROM att_details_fms
     WHERE network_id = source_network_id;
   
     -- Calculate the available cores for the source system    
     SELECT COUNT(*)
    INTO availableCoreSource
    FROM isp_port_info
    WHERE parent_system_id = p_source_system_id 
      AND parent_entity_type = 'FMS' 
     -- AND port_status_id = 1 
      AND input_output = 'O' and  link_system_id = 0; 
         
   else
    p_source_system_id =0;
     -- Get SpliceClosure From the Source
     SELECT longitude || ' ' || latitude AS geom
     INTO p_source
     FROM att_details_spliceclosure
     WHERE network_id = source_network_id;
   END IF;

   -- Check if destination exists in FMS, get it destination from SpliceClosure
   IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
    SELECT longitude || ' ' || latitude AS geom, system_id
    INTO p_destination, p_destination_system_id
    FROM att_details_fms
    WHERE network_id = destination_network_id;
   
       -- Calculate the available cores for the destination system
    SELECT COUNT(*)
    INTO availableCoreDestination
    FROM isp_port_info
    WHERE parent_system_id = p_destination_system_id 
      AND parent_entity_type = 'FMS' 
      --AND port_status_id = 1 
      AND input_output = 'O' and  link_system_id =0;    
   else
   p_destination_system_id =0;
    -- Get SpliceClosure From the Destination
    SELECT longitude || ' ' || latitude AS geom
    INTO p_destination
    FROM att_details_spliceclosure
    WHERE network_id = destination_network_id;
   END IF;

    -- check if Both FMS does not exist        
    IF NOT EXISTS (
    SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) and
    NOT EXISTS (
    SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'At least one ODF is required. Please provide a valid ODF.' AS message) row;
    END IF;
    
     -- Check if source FMS does not exist
   IF NOT EXISTS ( SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) THEN
    -- Check if source network_id exists in att_details_spliceclosure
    IF NOT EXISTS (
        SELECT 1 FROM att_details_spliceclosure WHERE network_id = source_network_id) THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (SELECT false AS status, 
           'Please enter a valid ODF/Splice Closure.' AS message) row;
    END IF;
   END IF;

   -- Check if destination FMS does not exist
   IF NOT EXISTS (
    SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
    -- Check if destination network_id exists in att_details_spliceclosure
    IF NOT EXISTS (SELECT 1 FROM att_details_spliceclosure WHERE network_id = destination_network_id) THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (SELECT false AS status, 
           'Please enter a valid ODF/Splice Closure.' AS message) row;
    END IF;
  END IF;
   
    -- Check if both FMS (source and destination) exist
    IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
    AND EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
     -- Validate Core/ Port if both counts match the required core for source and destination
     IF availableCoreSource < required_core OR availableCoreDestination < required_core THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (
            SELECT false AS status, 
           'Required ports are unavailable on the ODF. Please check and update the port availability.' 
            AS message) row;
     END IF;   
    -- Check if only source FMS exists
    ELSIF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) THEN
     -- Validate Core/ Port if source count matches the required core
     IF availableCoreSource < required_core THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (
            SELECT false AS status, 
            'Required ports are unavailable on the ODF. Please check and update the port availability.' 
            AS message) row;
     END IF;   
    -- Check if only destination FMS exists
    ELSIF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
    -- Validate Core/ Port if destination count matches the required core
    IF availableCoreDestination < required_core THEN
        RETURN QUERY 
        SELECT row_to_json(row) 
        FROM (
            SELECT false AS status, 
            'Required ports are unavailable on the ODF. Please check and update the port availability.' 
            AS message) row;
     END IF;   
    END IF;

	 -- Populate temporary cable routes table
      INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id)
		   
	  SELECT seq, path_seq, edge,p_user_id FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	  FROM routing_data_core_plan', (SELECT id
	  FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_source || ')', 4326), 3))
	  limit 1 ),(SELECT id FROM routing_data_core_plan_vertices_pgr
	  WHERE ST_Within( the_geom, 
	  ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_destination ||')', 4326), 3)) 
	  limit 1));
	
    delete from temp_cable_routes where edge_targetid = -1 and user_id = p_user_id;

   if not exists (select 1 from temp_cable_routes where user_id = p_user_id) 
   then

    RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'There is no existing route between the specified ODFs or Splice Closure.' AS message) row;
    RETURN;
    end if;
   
    insert into temp_route_connection(
	source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no)

	 select A.source_system_id,A.source_network_id,A.source_entity_type,A.source_port_no,B.destination_system_id,
	  B.destination_network_id,B.destination_entity_type,B.destination_port_no
	from connection_info A
	inner join connection_info B 
	on A.destination_system_id=B.source_system_id and A.destination_entity_type=B.source_entity_type
	and A.destination_port_no=B.source_port_no where A.source_system_id in(select edge_targetid from temp_cable_routes) and A.source_entity_type='Cable'

	union 
	select A.destination_system_id,A.destination_network_id,A.destination_entity_type,A.destination_port_no,
	B.source_system_id, B.source_network_id,B.source_entity_type,B.source_port_no
	from connection_info A
	inner join connection_info B 
	on B.destination_system_id=A.Source_system_id and B.destination_entity_type=A.Source_entity_type
	and B.destination_port_no=A.Source_port_no where A.destination_system_id in(select edge_targetid from temp_cable_routes) and A.destination_entity_type='Cable';

    -- Populate temporary ISP port info table
    INSERT INTO temp_isp_port_info (system_id, network_id, parent_system_id, 
    parent_network_id,   port_status_id, port_number,parent_entity_type)
    SELECT system_id, network_id, parent_system_id, parent_network_id, 
    port_status_id, port_number,parent_entity_type
    FROM isp_port_info a 
    WHERE a.parent_system_id = CASE 
        WHEN p_source_system_id !=0 THEN p_source_system_id 
        ELSE p_destination_system_id 
    END
    AND a.parent_entity_type = 'FMS'
  --  AND port_status_id = 1 
    AND input_output = 'O' and link_system_id =0
    ORDER BY system_id;

	update temp_isp_port_info set is_valid=false
	from temp_route_connection A
	where temp_isp_port_info.port_number=A.source_port_no
	and A.source_system_id not in(select edge_targetid from temp_cable_routes); 

	update temp_isp_port_info set is_valid=false
	from temp_route_connection A
	where temp_isp_port_info.port_number=A.source_port_no
	and A.destination_system_id not in(select edge_targetid from temp_cable_routes); 

    -- Loop through each record in temp_isp_port_info
    FOR rec IN (SELECT * FROM temp_isp_port_info where is_valid=true)
    LOOP
        -- Update temp_cable_routes for mismatched cable_ids
        UPDATE temp_cable_routes
        SET           
            avaiable_core_count = COALESCE(avaiable_core_count, 0) + 1
	   WHERE edge_targetid IN ( 
	   SELECT DISTINCT aci.cable_id FROM att_details_cable_info aci
       WHERE aci.cable_id IN ( SELECT edge_targetid FROM temp_cable_routes 
       WHERE user_id = p_user_id ) AND aci.fiber_number = rec.port_number 
       --AND (aci.a_end_status_id = 1 or (aci.is_a_end_through_connectivity = true and aci.a_end_status_id =2)) 
      -- AND (aci.b_end_status_id = 1 or (aci.is_b_end_through_connectivity = true and aci.b_end_status_id =2)
	   AND aci.link_system_id=0) and  user_id = p_user_id;

    if ( (select count(1) from (SELECT DISTINCT aci.cable_id FROM att_details_cable_info aci
    WHERE aci.cable_id IN ( SELECT edge_targetid FROM temp_cable_routes 
    WHERE user_id = p_user_id )
    AND aci.fiber_number = rec.port_number  
    --AND (aci.a_end_status_id = 1 or (aci.is_a_end_through_connectivity = true and aci.a_end_status_id =2)) 
    --AND (aci.b_end_status_id = 1 or (aci.is_b_end_through_connectivity = true and aci.b_end_status_id =2))
	   and aci.link_system_id =0
	   )a) 
	   = (SELECT count(1) FROM temp_cable_routes where  user_id = p_user_id))
   then
  
   IF  (p_source_system_id != 0 AND p_destination_system_id != 0 )
   then 
     update isp_port_info set is_valid_for_core_plan=true where parent_system_id=p_destination_system_id
     and parent_entity_type=rec.parent_entity_type and port_number = rec.port_number AND input_output = 'O'
     and link_system_id =0;
   else 
     update isp_port_info set is_valid_for_core_plan=true where parent_system_id=rec.parent_system_id
     and parent_entity_type=rec.parent_entity_type and port_number=rec.port_number AND input_output = 'O';
  end if;
 
  end if;
  END LOOP;

   IF  (p_source_system_id != 0 AND p_destination_system_id != 0 ) 
      then
     update isp_port_info set is_valid_for_core_plan=true where parent_system_id = p_source_system_id
     and parent_entity_type='FMS' and port_number in (SELECT port_number FROM isp_port_info
     WHERE parent_system_id = p_destination_system_id  AND parent_entity_type = 'FMS' AND 
     input_output = 'O'  AND is_valid_for_core_plan = true) AND input_output = 'O'
     and link_system_id =0;
   end if;

   update temp_cable_routes set message = 'Unavailable Core', is_valid = false where 
   avaiable_core_count  <  required_core and user_id = p_user_id;
 
 if (SELECT count(*) 
    FROM temp_cable_routes 
    WHERE avaiable_core_count >= required_core 
    AND user_id = p_user_id) = (select count(*) from temp_cable_routes)
    then
    IF p_source_system_id IS NOT NULL AND p_source_system_id != 0
      AND p_destination_system_id IS NOT NULL AND p_destination_system_id != 0 
      then
  if(SELECT COUNT(1) FROM isp_port_info 
    WHERE parent_system_id = p_destination_system_id 
      AND is_valid_for_core_plan = true 
      AND parent_entity_type = 'FMS'
      AND input_output = 'O' AND link_system_id =0) < required_core
      then 
       RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Required ports are unavailable on the ODF. Please check and update the port availability.' AS message) row;
RETURN;
      end if;
    end if;
   end if;
    -- Log valid cables
    INSERT INTO core_planner_logs(
        cable_id, cable_name, network_status, total_core, cable_length, 
		--a_system_id, b_system_id, a_entity_type, b_entity_type, 
		error_msg, is_valid, user_id, 
       -- a_network_id, b_network_id, 
		cable_network_id, avaiable, used_core
    )
    SELECT 
        att.system_id, att.cable_name, att.network_status, att.total_core, att.cable_calculated_length,
        --att.a_system_id, att.b_system_id, att.a_entity_type, att.b_entity_type, 
		info.message, info.is_valid,
        p_user_id, --att.a_network_id, att.b_network_id, 
		att.network_id, 
        (SELECT COUNT(*) FROM att_details_cable_info aci WHERE aci.cable_id = att.system_id
        -- AND aci.a_end_status_id = 1 AND aci.b_end_status_id = 1) AS available_cores,
		and (aci.link_system_id=0)) AS available_cores,
        (SELECT COUNT(*) FROM att_details_cable_info aci WHERE aci.cable_id = info.edge_targetid
        -- AND (aci.a_end_status_id > 1 or aci.b_end_status_id > 1 ) ) AS used_core
		 AND (aci.link_system_id > 0 ) ) AS used_core
    FROM att_details_cable att 
    LEFT JOIN temp_cable_routes info ON att.system_id = info.edge_targetid and  info.user_id = p_user_id
    WHERE att.system_id IN (SELECT edge_targetid FROM temp_cable_routes where user_id = p_user_id);

    -- Update system_id and entity_type for a and b ends
	
	with startCte as(
   SELECT pm.system_id,pm.entity_type,pm.common_name,cable_id, ST_ENDPOINT(lm.sp_geometry) as endPoint
                       FROM core_planner_logs
                       INNER JOIN line_master lm ON lm.system_id = core_planner_logs.cable_id 
                       AND lm.entity_type = 'Cable' 
						inner join  point_master pm 
                       on ST_WITHIN(pm.sp_geometry, ST_BUFFER_METERS(ST_STARTPOINT(lm.sp_geometry), 3)) 
                       AND pm.entity_type IN ('BDB','FDB','SpliceClosure','FMS') 
                       WHERE core_planner_logs.user_id = p_user_id )
    UPDATE core_planner_logs 
    SET 
        a_system_id = startCte.system_id,
        a_entity_type = startCte.entity_type,
		a_network_id=startCte.common_name
		from startCte
     WHERE core_planner_logs.user_id = p_user_id and core_planner_logs.cable_id=startCte.cable_id;

with endCte as(
SELECT pm.system_id,pm.entity_type,pm.common_name,cable_id 
                       FROM core_planner_logs
                       INNER JOIN line_master lm ON lm.system_id = core_planner_logs.cable_id 
                       AND lm.entity_type = 'Cable' 
						inner join  point_master pm 
                       on ST_WITHIN(pm.sp_geometry, ST_BUFFER_METERS(ST_ENDPOINT(lm.sp_geometry), 3)) 
                       AND pm.entity_type IN ('BDB','FDB','SpliceClosure','FMS') 
                       WHERE core_planner_logs.user_id = p_user_id )
    UPDATE core_planner_logs 
    SET 
        b_system_id = endCte.system_id,
        b_entity_type = endCte.entity_type,
		b_network_id=endCte.common_name
		from endCte
    WHERE core_planner_logs.user_id = p_user_id and core_planner_logs.cable_id=endCte.cable_id;

    -- Mark invalid records if no termination points
    UPDATE core_planner_logs 
    SET is_valid = false 
    WHERE coalesce(b_system_id, 0) = 0 OR coalesce(a_system_id, 0) = 0 
    AND user_id = p_user_id;

       -- Validate if both FMS points are found
    if NOT EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
   OR NOT EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) 
    then
       select logs.cable_network_id  into cableNetworkId from core_planner_logs logs inner join att_details_fms fms 
       on (fms.system_id = logs.a_system_id and logs.a_entity_type ='FMS') or (fms.system_id = logs.b_system_id and logs.b_entity_type ='FMS') 
       WHERE fms.system_id not in (p_source_system_id ,p_destination_system_id ) and user_id = p_user_id limit 1;
      if cableNetworkId is not null
      then 
       RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Extra ODF found between source and destination at the end of the cable '||cableNetworkId  AS message) row; 
      end if;
    end if ;
   
    -- Validate if both FMS points are found
    IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
    AND EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = destination_network_id) THEN
	IF (select COUNT(1)!= 2 from (
	SELECT *
        FROM core_planner_logs a WHERE a.a_entity_type = 'FMS' and a.user_id = p_user_id

        union all
        select * from core_planner_logs b
        WHERE b.b_entity_type = 'FMS' AND b.user_id = p_user_id)t)  
         then
             RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'One extra ODF found between source and destination. Please review and update the network path.'  AS message) row;
         
        -- Invalidate all records for the user
       /* UPDATE core_planner_logs
        SET is_valid = FALSE
        WHERE user_id = p_user_id;*/
    END IF; 
   end if;
  
    -- Validate if `a_system_id` or `b_system_id` is null
  UPDATE core_planner_logs log2
  SET 
  is_valid = FALSE,
  error_msg = 'Cable is not terminated properly'
  FROM core_planner_logs log1
  WHERE log2.user_id = p_user_id
  AND log2.user_id = log1.user_id
  AND (log2.a_system_id IS NULL OR log2.b_system_id IS NULL)
  AND log2.cable_id = log1.cable_id;

    -- Return result
    IF exists (SELECT 1 FROM core_planner_logs WHERE user_id = p_user_id AND is_valid = false )  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Required core is unavailable from '||source_network_id ||' to '||destination_network_id  AS message) row;
    ELSE
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT true AS status, 'Required Core available' AS message) row;
    END IF;

END;
$BODY$;

ALTER FUNCTION public.fn_get_core_planner_validation(character varying, character varying, integer, integer, integer)
    OWNER TO postgres;


