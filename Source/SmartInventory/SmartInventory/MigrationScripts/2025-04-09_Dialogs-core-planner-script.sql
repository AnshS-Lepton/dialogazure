CREATE OR REPLACE FUNCTION public.fn_get_core_planner_splicing(required_core integer, p_user_id integer, fiber_link_network_id character varying, source_network_id character varying, destination_network_id character varying, buffer integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

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
	
	-- TEMP TABLE FOR CPF RESULT --
create temp table temp_cpf_result
(
id serial,
connection_id integer,
source_system_id integer,
source_network_id character varying(100),
source_entity_type character varying(100),
source_port_no integer,
destination_system_id integer,
destination_network_id character varying(100),
destination_entity_type character varying(100),
destination_port_no integer
) on commit drop;

create temp table temp_connected_cables
(
id serial,
cable_id integer,
fiber_number integer
) on commit drop;

create temp table temp_connected_FMS
(
id serial,
fms_id integer,
port_number integer
) on commit drop;

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
    parent_network_id, port_status_id, port_number,parent_entity_type)
    SELECT system_id, network_id, parent_system_id, parent_network_id, 
    port_status_id, port_number, parent_entity_type 
    FROM isp_port_info ipt
    WHERE ipt.parent_network_id = p_odf_network_id AND ipt.parent_entity_type = 'FMS'
    AND ipt.input_output = 'O'
    AND ipt.port_status_id = 1 AND ipt.port_number >= (
      SELECT MIN(port_number) 
      FROM isp_port_info 
      WHERE parent_network_id = p_odf_network_id  AND parent_entity_type = 'FMS' AND input_output = 'O'
        AND port_status_id = 1
        AND port_number % 2 = 1 
   ) ORDER BY ipt.port_number ASC  LIMIT required_core;
	
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
									
                    SELECT leftcbl.parent_system_id, leftcbl.parent_network_id, 'FMS', 
                    leftcbl.port_number, 
                    rightcbl.cable_id, rightcbl.network_id, 'Cable', 
                    rightcbl.fiber_number,false,rightcbl.IS_CABLE_A_END, 
                    p_user_id, 'CorePlanning'
                FROM (
			    SELECT parent_system_id,parent_network_id,
			    false as IS_CABLE_A_END,
			    port_number, ROW_NUMBER() OVER (ORDER BY port_number) AS rn
			    FROM isp_port_info where parent_system_id=rec.a_system_id
   			    AND parent_entity_type = 'FMS'  
   			    AND input_output = 'O' and link_system_id =0
				and port_status_id=1 
    			ORDER BY port_number limit required_core 
			) leftcbl
			JOIN (
			    SELECT cable_id,v_cable_details_left.network_id as network_id,
			    v_cable_details_left.IS_CABLE_A_END as IS_CABLE_A_END,fiber_number, 
			    ROW_NUMBER() OVER (ORDER BY fiber_number) AS rn
			    FROM core_planner_fiber_info where cable_id = v_cable_details_left.system_id
			    and ((v_cable_details_left.IS_CABLE_A_END=true and a_end_status_id = 1)
			 	or(v_cable_details_left.IS_CABLE_A_END =false and b_end_status_id = 1)) and is_valid = true
			) rightcbl
			ON leftcbl.rn = rightcbl.rn limit required_core;
		
						   							           		   
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
				
		
					
			SELECT leftcbl.cable_id, leftcbl.network_id, 'Cable', 
            leftcbl.fiber_number,rightcbl.cable_id,  rightcbl.network_id , 'Cable', 
            rightcbl.fiber_number,leftcbl.IS_CABLE_A_END,rightcbl.IS_CABLE_A_END, 
            p_user_id, 'CorePlanning',rec.a_system_id,rec.a_network_id,rec.a_entity_type
                FROM (
			    SELECT cable_id,v_cable_details_left.network_id as network_id,
			    v_cable_details_left.IS_CABLE_A_END as IS_CABLE_A_END,
			    fiber_number, ROW_NUMBER() OVER (ORDER BY fiber_number) AS rn
			    FROM core_planner_fiber_info where cable_id = v_cable_details_left.system_id
			    and ((v_cable_details_left.IS_CABLE_A_END=true and a_end_status_id = 1)
				or(v_cable_details_left.IS_CABLE_A_END =false and b_end_status_id = 1)) and is_valid = true
			) leftcbl
			JOIN (
			    SELECT cable_id,v_cable_details_right.network_id as network_id,
			    v_cable_details_right.IS_CABLE_A_END as IS_CABLE_A_END,fiber_number, 
			    ROW_NUMBER() OVER (ORDER BY fiber_number) AS rn
			    FROM core_planner_fiber_info where cable_id = v_cable_details_right.system_id
			    and ((v_cable_details_right.IS_CABLE_A_END=true and a_end_status_id = 1)
			 	or(v_cable_details_right.IS_CABLE_A_END =false and b_end_status_id = 1)) and is_valid = true
			) rightcbl
			ON leftcbl.rn = rightcbl.rn limit required_core;							    		                 	
		end if;   
	end if;
        END LOOP;
	

	  /*
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
	
	*/
	
	--- update odf is_valid_for_core_plan,link_system_id
      /*  IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
		then
		 
		update isp_port_info set link_system_id=coalesce(p_link_system_id,0)
		where parent_network_id =source_network_id
		and input_output='O'-- and port_status_id=1 
		and port_number in (select port_number 
		from isp_port_info_tbl 
		---where parent_network_id =source_network_id
		--and input_output='O' -- and port_status_id=1					
		order by port_number limit required_core);
		/*
		update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id =source_network_id
		and parent_entity_type='FMS' 
		--and port_number=v_arow.port_number 
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;
	*/
	
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
		/*
	   update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id = p_destination_network_id
		and parent_entity_type='FMS' 
		--and port_number=v_arow.port_number 
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;	
			*/
       end if;*/
 
 
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

/*
update isp_port_info set link_system_id=coalesce(p_link_system_id,0)
WHERE parent_network_id=p_odf_network_id
    AND parent_entity_type = 'FMS'  
    AND input_output = 'O' and is_valid_for_core_plan=true; 
	
	
update isp_port_info set link_system_id=coalesce(p_link_system_id,0),is_valid_for_core_plan=true
from isp_port_info_tbl b
where isp_port_info.parent_entity_type=b.parent_entity_type
and isp_port_info.parent_system_id=b.parent_system_id
and isp_port_info.port_number=b.port_number;
*/


-- GET CONNECTION DATA BASED ON EQUIPMENT OR DEVICE
insert into temp_cpf_result(connection_id,source_system_id,source_network_id,source_entity_type,
source_port_no,destination_system_id,
destination_network_id,destination_entity_type,destination_port_no
)
select con.connection_id,con.source_system_id,con.source_network_id,con.source_entity_type,
con.source_port_no,con.destination_system_id,
con.destination_network_id,con.destination_entity_type,con.destination_port_no
from fn_get_fiberlink_schematicview_date('','',1,100,'','',(select parent_system_id from isp_port_info
where parent_network_id=p_odf_network_id limit 1),
(select string_agg(port_number::character varying,',')::character varying from isp_port_info
where parent_network_id=p_odf_network_id and upper(parent_entity_type)=upper('FMS') and upper(input_output)=upper('O')
and is_valid_for_core_plan=true and port_number in(select port_number from isp_port_info_tbl))
,'FMS') con;

insert into temp_connected_cables(cable_id,fiber_number)
select source_system_id,source_port_no from temp_cpf_result where source_entity_type='Cable'
union
select destination_system_id,destination_port_no from temp_cpf_result where destination_entity_type='Cable';

-- insert into temp_connected_FMS(fms_id,port_number)
-- select source_system_id,source_port_no from temp_cpf_result where source_entity_type='FMS' and source_network_id!=destination_network_id
-- union
-- select destination_system_id,destination_port_no from temp_cpf_result where destination_entity_type='FMS' 
-- and source_network_id!=destination_network_id;

-- update isp_port_info set link_system_id=coalesce(p_link_system_id,0),is_valid_for_core_plan=true
-- from temp_connected_FMS b
-- where isp_port_info.parent_entity_type='FMS'
-- and isp_port_info.parent_system_id=b.source_system_id
-- and isp_port_info.port_number=b.source_port_no;

/*update att_details_cable_info set  fiber_status ='Reserved',
link_system_id=coalesce(p_link_system_id,0)
from temp_connected_cables b
where att_details_cable_info.cable_id=b.cable_id and b.fiber_number=att_details_cable_info.fiber_number;*/

		UPDATE att_details_cable_info 
		SET fiber_status = 'Reserved',
		    link_system_id = COALESCE(p_link_system_id, 0)
		FROM (
		    SELECT b.cable_id, b.fiber_number, 
		           ROW_NUMBER() OVER (PARTITION BY b.cable_id ORDER BY b.fiber_number asc) AS rn
		    FROM core_planner_fiber_info b  
		    INNER JOIN TEMP_LINE_MASTER tmp 
		        ON tmp.system_id = b.cable_id 
		    WHERE b.is_valid = true and link_system_id =0
		) b
		WHERE b.rn <= required_core  -- Ensure required_core is properly assigned
		AND att_details_cable_info.cable_id = b.cable_id 
		AND att_details_cable_info.fiber_number = b.fiber_number;

    /*   update att_details_cable_info set  fiber_status ='Reserved',
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
		and fiber_number=b.port_number;*/

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
  /*      IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = source_network_id) 
		then		
		
		update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id =source_network_id
		and parent_entity_type='FMS' 
		
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;
	
	
       end if;
     
      IF EXISTS (SELECT 1 FROM att_details_fms WHERE network_id = p_destination_network_id) 
	  then	   
	   update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id = p_destination_network_id
		and parent_entity_type='FMS' 		
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;			
       end if;*/
	      
     --  delete from core_planner_logs where user_id = p_user_id;

       return query select row_to_json(row) from ( select true as status, 'The required core has been spliced, and the fiber link has been successfully attached.' as message ) row;
      end if;
  END;
 
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) TO postgres;


--------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation(source_network_id character varying, destination_network_id character varying, buffer integer, required_core integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE     
    P_SOURCE CHARACTER VARYING;
    P_DESTINATION CHARACTER VARYING;    
    P_SOURCE_SYSTEM_ID INTEGER;
    P_DESTINATION_SYSTEM_ID INTEGER;
    P_SOURCE_NETWORK_ID CHARACTER VARYING;
	P_DESTINATION_NETWORK_ID CHARACTER VARYING;
	V_IS_VALID BOOLEAN;
	V_MESSAGE CHARACTER VARYING;
	FIBER_ODF_COUNT INTEGER;
    FIBER_CABLE_COUNT INTEGER;
   Is_AvailableODF BOOLEAN;
BEGIN
	V_IS_VALID:=TRUE;
	V_MESSAGE:='Required Core available';
	P_SOURCE_NETWORK_ID:=SOURCE_NETWORK_ID;
	P_DESTINATION_NETWORK_ID:=DESTINATION_NETWORK_ID;
	P_SOURCE_SYSTEM_ID:=0;
	P_DESTINATION_SYSTEM_ID:=0;
	FIBER_ODF_COUNT:=0;
    FIBER_CABLE_COUNT:=0;
     --TRUNCATE TABLE TEMP_CABLE_ROUTES; 
     --TRUNCATE TABLE TEMP_ISP_PORT_INFO;
     --TRUNCATE TABLE TEMP_ISP_PORT_INFO; 
     --TRUNCATE TABLE TEMP_ROUTE_CONNECTION; 

    
-- CREATE TEMPORARY TABLES
       CREATE TEMP TABLE  TEMP_CABLE_ROUTES(
        SEQ INTEGER,
        PATH_SEQ INTEGER, 
        EDGE_TARGETID INTEGER, 
        ROADLINE_GEOMTEXT TEXT, 
        START_POINT CHARACTER VARYING,
        END_POINT CHARACTER VARYING,
        MESSAGE VARCHAR NULL,
        IS_VALID BOOLEAN DEFAULT TRUE,
        AVAIABLE_CORE_COUNT INTEGER DEFAULT 0,
        USER_ID INTEGER,
        A_SYSTEM_ID INTEGER,
        A_ENTITY_TYPE CHARACTER VARYING,
        A_NETWORK_ID CHARACTER VARYING,
        B_SYSTEM_ID INTEGER,
        B_ENTITY_TYPE CHARACTER VARYING,
        B_NETWORK_ID CHARACTER VARYING
    ) ON COMMIT DROP;

    CREATE TEMP TABLE TEMP_ISP_PORT_INFO(
        SYSTEM_ID INT4 NULL,
        NETWORK_ID VARCHAR NULL,
        PARENT_SYSTEM_ID INT4 NULL,
        PARENT_NETWORK_ID VARCHAR NULL,
        PORT_STATUS_ID INT4 NULL,
       -- IS_VALID BOOLEAN DEFAULT TRUE,
        PORT_NUMBER INT4 NULL,
        PARENT_ENTITY_TYPE VARCHAR NULL,
		IS_VALID BOOLEAN NOT NULL DEFAULT TRUE
    ) ON COMMIT DROP;

	CREATE TEMP TABLE TEMP_ROUTE_CONNECTION (
		ID SERIAL,
		CONNECTION_ID INT4,
		SOURCE_SYSTEM_ID INT4 NULL,
		SOURCE_NETWORK_ID VARCHAR(100) NULL,
		SOURCE_ENTITY_TYPE VARCHAR(100) NULL,
		SOURCE_PORT_NO INT4 NULL,
		DESTINATION_SYSTEM_ID INT4 NULL,
		DESTINATION_NETWORK_ID VARCHAR(100) NULL,
		DESTINATION_ENTITY_TYPE VARCHAR(100) NULL,
		DESTINATION_PORT_NO INT4 null,
		IS_CABLE_A_END BOOLEAN,
		IS_CABLE_B_END BOOLEAN
	)ON COMMIT DROP;


    -- CLEAR PREVIOUS LOGS FOR THE USER
    DELETE FROM CORE_PLANNER_LOGS WHERE USER_ID = P_USER_ID;
    DELETE  FROM CORE_PLANNER_FIBER_INFO WHERE USER_ID = P_USER_ID;
    
    IF V_IS_VALID and REQUIRED_CORE > 2
    THEN 
	V_IS_VALID:=FALSE;
	V_MESSAGE:='A maximum of two ports is permitted!';        
    END IF;
   
    IF V_IS_VALID and NOT EXISTS(SELECT 1 FROM POINT_MASTER WHERE COMMON_NAME IN(SOURCE_NETWORK_ID) and entity_type in('FMS','SpliceClosure'))
    THEN 
	V_IS_VALID:=FALSE;
	V_MESSAGE:='Please enter a valid ODF/Splice Closure at start!';        
    END IF;

    IF V_IS_VALID and NOT EXISTS(SELECT 1 FROM POINT_MASTER WHERE COMMON_NAME IN(DESTINATION_NETWORK_ID) and entity_type in('FMS','SpliceClosure'))
    THEN 
	V_IS_VALID:=FALSE;
	V_MESSAGE:='Please enter a valid ODF/Splice Closure at end!';        
    END IF;

    IF V_IS_VALID AND (SELECT COUNT(1)=0 FROM ATT_DETAILS_FMS WHERE NETWORK_ID IN(SOURCE_NETWORK_ID,DESTINATION_NETWORK_ID))
    THEN
	V_IS_VALID:=FALSE;
	V_MESSAGE:='AT LEAST ONE ODF IS REQUIRED. PLEASE PROVIDE A VALID ODF.!'  ;      
    END IF;
        
  if(V_IS_VALID)
  then           
	  SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
	  INTO P_SOURCE, P_SOURCE_SYSTEM_ID
	  FROM POINT_MASTER
	  WHERE COMMON_NAME IN(SOURCE_NETWORK_ID) AND ENTITY_TYPE in('FMS');

	  SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
	  INTO P_DESTINATION, P_DESTINATION_SYSTEM_ID 
	  FROM POINT_MASTER
	  WHERE COMMON_NAME IN(DESTINATION_NETWORK_ID) AND ENTITY_TYPE in('FMS');

	IF COALESCE(P_SOURCE_SYSTEM_ID,0)=0
	THEN		
		SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
		INTO P_SOURCE
		FROM POINT_MASTER
		WHERE COMMON_NAME IN(SOURCE_NETWORK_ID) AND ENTITY_TYPE in('SpliceClosure');
	end if;

	IF COALESCE(P_DESTINATION_SYSTEM_ID,0)=0
	THEN
		SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
		INTO P_DESTINATION
		FROM POINT_MASTER
		WHERE COMMON_NAME IN(DESTINATION_NETWORK_ID) AND ENTITY_TYPE in('SpliceClosure');
	end if;
	RAISE NOTICE 'P_DESTINATION_SYSTEM_ID1: %', P_DESTINATION_SYSTEM_ID;
RAISE NOTICE 'P_SOURCE_SYSTEM_ID1: %', P_SOURCE_SYSTEM_ID;

      IF P_DESTINATION_SYSTEM_ID <> 0 and P_SOURCE_SYSTEM_ID <> 0 then
      	RAISE NOTICE 'P_DESTINATION_SYSTEM_ID2: %', P_DESTINATION_SYSTEM_ID;
        RAISE NOTICE 'P_SOURCE_SYSTEM_ID2: %', P_SOURCE_SYSTEM_ID;
       
	    INSERT INTO TEMP_ISP_PORT_INFO (SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, 
	    PARENT_NETWORK_ID,   PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE)
	    SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID, 
	    PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE
	    FROM ISP_PORT_INFO A 
	    WHERE A.PARENT_SYSTEM_ID = P_DESTINATION_SYSTEM_ID 
	    AND A.PARENT_ENTITY_TYPE = 'FMS' AND INPUT_OUTPUT = 'O' and port_status_id = 1 
	   ORDER BY SYSTEM_ID;
	 end if;
	 
	  WITH SORTED_DATA AS (
		            SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID, 
	    PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE,
		            LAG(PORT_NUMBER) OVER (PARTITION BY PARENT_SYSTEM_ID ORDER BY PORT_NUMBER) AS PREV_FIBER
		            FROM ISP_PORT_INFO 
		            WHERE  PARENT_SYSTEM_ID = (CASE WHEN P_SOURCE_SYSTEM_ID <> 0 THEN P_SOURCE_SYSTEM_ID
                   ELSE P_DESTINATION_SYSTEM_ID END) AND 
		            PORT_NUMBER % 2 = 1 AND	INPUT_OUTPUT = 'O' and PARENT_ENTITY_TYPE = 'FMS' 
		            and port_status_id = 1
		        ),
		        FILTERED_DATA AS (
		            SELECT A.SYSTEM_ID, A.NETWORK_ID, A.PARENT_SYSTEM_ID, A.PARENT_NETWORK_ID, 
	    A.PORT_STATUS_ID, A.PORT_NUMBER,A.PARENT_ENTITY_TYPE
		            FROM ISP_PORT_INFO A
		            INNER JOIN SORTED_DATA B 
		                ON A.PARENT_SYSTEM_ID = B.PARENT_SYSTEM_ID 
		                AND (A.PORT_NUMBER = B.PORT_NUMBER + 1 OR A.PORT_NUMBER = B.PORT_NUMBER)
		            WHERE A.INPUT_OUTPUT = 'O' and A.PARENT_ENTITY_TYPE = 'FMS'
		            and A.port_status_id = 1
		        ),
		        FIBER_DATA AS (
		            SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID, 
	                 PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE,
		            LAG(PORT_NUMBER) OVER (PARTITION BY PARENT_SYSTEM_ID ORDER BY PORT_NUMBER) AS PREV_FIBER,
		            LEAD(PORT_NUMBER) OVER (PARTITION BY PARENT_SYSTEM_ID ORDER BY PORT_NUMBER) AS NEXT_FIBER        
		            FROM FILTERED_DATA 
		            WHERE PARENT_SYSTEM_ID = (CASE WHEN P_SOURCE_SYSTEM_ID <> 0 THEN P_SOURCE_SYSTEM_ID
                   ELSE P_DESTINATION_SYSTEM_ID END)
		        )		        
       INSERT INTO TEMP_ISP_PORT_INFO (SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, 
	   PARENT_NETWORK_ID,   PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE)
	   
	    SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID, 
	                 PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE FROM FIBER_DATA
		WHERE PORT_NUMBER = PREV_FIBER + 1 OR PORT_NUMBER = NEXT_FIBER - 1;
		        		        
          -- Populate temporary cable routes table
	  INSERT INTO TEMP_CABLE_ROUTES(SEQ, PATH_SEQ, EDGE_TARGETID, USER_ID)		  
	  SELECT SEQ, PATH_SEQ, EDGE,P_USER_ID FROM PGR_DIJKSTRA('SELECT ID, SOURCE, TARGET, COST, REVERSE_COST 
	  FROM ROUTING_DATA_CORE_PLAN', (SELECT ID
	  FROM ROUTING_DATA_CORE_PLAN_VERTICES_PGR
	  WHERE ST_WITHIN( THE_GEOM, ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT(' || P_SOURCE || ')', 4326), 3))
	  LIMIT 1 ),(SELECT ID FROM ROUTING_DATA_CORE_PLAN_VERTICES_PGR
	  WHERE ST_WITHIN( THE_GEOM, 
	  ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('|| P_DESTINATION ||')', 4326), 3)) 
	  LIMIT 1));
	
	  DELETE FROM TEMP_CABLE_ROUTES WHERE EDGE_TARGETID = -1 AND USER_ID = P_USER_ID;

	IF NOT EXISTS(SELECT 1 FROM TEMP_CABLE_ROUTES)
	THEN
		V_IS_VALID:=false;
		V_MESSAGE:='Route not found between ODF/Splice Closure!';
	END IF;

	-----------UPDATE THE A END ID
	WITH STARTCTE AS(
	SELECT PM.SYSTEM_ID,PM.ENTITY_TYPE,PM.COMMON_NAME,EDGE_TARGETID as CABLE_ID,
	ST_ENDPOINT(LM.SP_GEOMETRY) AS ENDPOINT
	FROM TEMP_CABLE_ROUTES RS
	INNER JOIN LINE_MASTER LM 
	ON LM.SYSTEM_ID = RS.EDGE_TARGETID AND UPPER(LM.ENTITY_TYPE) = 'CABLE' 
	INNER JOIN  POINT_MASTER PM  
	ON ST_WITHIN(PM.SP_GEOMETRY, ST_BUFFER_METERS(ST_STARTPOINT(LM.SP_GEOMETRY), 3)) 
	AND UPPER(PM.ENTITY_TYPE) IN ('BDB','FDB','SPLICECLOSURE','FMS'))
			
	UPDATE TEMP_CABLE_ROUTES 
	SET 
	A_SYSTEM_ID = STARTCTE.SYSTEM_ID,
	A_ENTITY_TYPE = STARTCTE.ENTITY_TYPE,
	A_NETWORK_ID=STARTCTE.COMMON_NAME
	FROM STARTCTE
	WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID=STARTCTE.CABLE_ID;

	-----------UPDATE THE B END ID
	WITH ENDCTE AS(
	SELECT PM.SYSTEM_ID,PM.ENTITY_TYPE,PM.COMMON_NAME,EDGE_TARGETID as CABLE_ID, ST_ENDPOINT(LM.SP_GEOMETRY) AS ENDPOINT
	FROM TEMP_CABLE_ROUTES RS
	INNER JOIN LINE_MASTER LM 
	ON LM.SYSTEM_ID = RS.EDGE_TARGETID AND UPPER(LM.ENTITY_TYPE) = 'CABLE' 
	INNER JOIN  POINT_MASTER PM  
	ON ST_WITHIN(PM.SP_GEOMETRY, ST_BUFFER_METERS(ST_ENDPOINT(LM.SP_GEOMETRY), 3)) 
	AND UPPER(PM.ENTITY_TYPE) IN ('BDB','FDB','SPLICECLOSURE','FMS'))
			
	UPDATE TEMP_CABLE_ROUTES 
	SET 
	B_SYSTEM_ID = ENDCTE.SYSTEM_ID,
	B_ENTITY_TYPE = ENDCTE.ENTITY_TYPE,
	B_NETWORK_ID=ENDCTE.COMMON_NAME
	FROM ENDCTE
	WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID=ENDCTE.CABLE_ID;

	IF V_IS_VALID AND EXISTS(SELECT 1 FROM TEMP_CABLE_ROUTES WHERE (A_ENTITY_TYPE = 'FMS' AND A_NETWORK_ID 		NOT IN (SOURCE_NETWORK_ID,DESTINATION_NETWORK_ID)) 
   		OR (B_ENTITY_TYPE = 'FMS' AND B_NETWORK_ID NOT IN (SOURCE_NETWORK_ID,DESTINATION_NETWORK_ID)))
	THEN
		V_IS_VALID:=FALSE;
		V_MESSAGE:='MULTIPLE ODF FOUND ON THIS ROUTE!';
	END IF;

	IF V_IS_VALID AND (SELECT COUNT(1)>0 FROM TEMP_CABLE_ROUTES WHERE COALESCE(B_ENTITY_TYPE,'')='' OR COALESCE(A_ENTITY_TYPE,'')='')
	THEN
		UPDATE TEMP_CABLE_ROUTES SET IS_VALID=false,
		MESSAGE='Splice Closure is missing at end point of cables'
		WHERE COALESCE(B_ENTITY_TYPE,'')='' OR COALESCE(A_ENTITY_TYPE,'')='';
	
		V_IS_VALID:=FALSE;
		V_MESSAGE:='Splice Closure is missing at end point of cables';
	END IF;

	IF(V_IS_VALID)
	THEN
		--GET THE ALL CABLE FIBER DETAILS	
		WITH SORTED_DATA AS (
		    SELECT CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, 
		           IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, LINK_SYSTEM_ID,
		           LAG(FIBER_NUMBER) OVER (PARTITION BY CABLE_ID ORDER BY FIBER_NUMBER) AS PREV_FIBER
		    FROM ATT_DETAILS_CABLE_INFO 
		    WHERE CABLE_ID IN (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) 
		          AND FIBER_NUMBER % 2 = 1
		          AND LINK_SYSTEM_ID = 0  ORDER BY FIBER_NUMBER
		),
		FILTERED_DATA AS (
		    SELECT A.CABLE_ID, A.FIBER_NUMBER, A.A_END_STATUS_ID, A.B_END_STATUS_ID, 
		           A.IS_A_END_THROUGH_CONNECTIVITY, A.IS_B_END_THROUGH_CONNECTIVITY, 
		            A.LINK_SYSTEM_ID
		    FROM ATT_DETAILS_CABLE_INFO A
		    INNER JOIN SORTED_DATA B 
		        ON A.CABLE_ID = B.CABLE_ID 
		        AND (A.FIBER_NUMBER = B.FIBER_NUMBER + 1 OR A.FIBER_NUMBER = B.FIBER_NUMBER)
		    WHERE A.LINK_SYSTEM_ID = 0 
		),
		FIBER_DATA AS (
		    SELECT CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, 
		           IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, 
		            LINK_SYSTEM_ID,  -- Fixed missing comma
		           LAG(FIBER_NUMBER) OVER (PARTITION BY CABLE_ID ORDER BY FIBER_NUMBER) AS PREV_FIBER,
		           LEAD(FIBER_NUMBER) OVER (PARTITION BY CABLE_ID ORDER BY FIBER_NUMBER) AS NEXT_FIBER        
		    FROM FILTERED_DATA 
		    WHERE CABLE_ID IN (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) ORDER BY FIBER_NUMBER
		)		
		-- Use correct INSERT INTO format
		INSERT INTO CORE_PLANNER_FIBER_INFO 
		    (CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, 
		     IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, USER_ID, LINK_SYSTEM_ID,seq)
		SELECT CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, 
		       IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, P_USER_ID, LINK_SYSTEM_ID, tcr.SEQ
		FROM FIBER_DATA
		JOIN TEMP_CABLE_ROUTES tcr 
    ON CABLE_ID = tcr.EDGE_TARGETID
WHERE 
    FIBER_NUMBER = PREV_FIBER + 1 OR FIBER_NUMBER = NEXT_FIBER - 1
ORDER BY 
    tcr.SEQ, FIBER_NUMBER
		;

		/*SELECT CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID, IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, P_USER_ID ,LINK_SYSTEM_ID
		FROM ATT_DETAILS_CABLE_INFO WHERE CABLE_ID IN (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES WHERE USER_ID = P_USER_ID) AND LINK_SYSTEM_ID = 0
		ORDER BY FIBER_NUMBER;*/

		--GET THE CONNECTION OF CABLE FROM THIS ROUTE
		INSERT INTO TEMP_ROUTE_CONNECTION(
		SOURCE_SYSTEM_ID,SOURCE_NETWORK_ID,SOURCE_ENTITY_TYPE,SOURCE_PORT_NO,DESTINATION_SYSTEM_ID,
		DESTINATION_NETWORK_ID,DESTINATION_ENTITY_TYPE,DESTINATION_PORT_NO,IS_CABLE_A_END,IS_CABLE_B_END)

		select A.SOURCE_SYSTEM_ID,A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,
		B.DESTINATION_SYSTEM_ID,B.DESTINATION_NETWORK_ID,B.DESTINATION_ENTITY_TYPE,
		B.DESTINATION_PORT_NO,A.IS_CABLE_A_END,false 
		FROM CONNECTION_INFO A
		INNER JOIN CONNECTION_INFO B 
		ON A.DESTINATION_SYSTEM_ID=B.SOURCE_SYSTEM_ID AND A.DESTINATION_ENTITY_TYPE=B.SOURCE_ENTITY_TYPE
		AND A.DESTINATION_PORT_NO=B.SOURCE_PORT_NO WHERE A.SOURCE_SYSTEM_ID 
		in (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) 
		AND A.SOURCE_ENTITY_TYPE='Cable'

		UNION 
		SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
		A.DESTINATION_PORT_NO,B.SOURCE_SYSTEM_ID, B.SOURCE_NETWORK_ID,B.SOURCE_ENTITY_TYPE,
		B.SOURCE_PORT_NO,false,A.is_cable_a_end
		FROM CONNECTION_INFO A
		INNER JOIN CONNECTION_INFO B 
		ON B.DESTINATION_SYSTEM_ID=A.SOURCE_SYSTEM_ID AND B.DESTINATION_ENTITY_TYPE=A.SOURCE_ENTITY_TYPE
		AND B.DESTINATION_PORT_NO=A.SOURCE_PORT_NO WHERE A.DESTINATION_SYSTEM_ID in 
		(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) AND A.DESTINATION_ENTITY_TYPE='Cable'	
		UNION 
		SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
		A.DESTINATION_PORT_NO,
		A.SOURCE_SYSTEM_ID, A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,false,A.IS_CABLE_A_END
		FROM CONNECTION_INFO A
		WHERE A.SOURCE_ENTITY_TYPE='FMS' and
		A.SOURCE_NETWORK_ID IN(P_SOURCE_NETWORK_ID,P_DESTINATION_NETWORK_ID) 
		AND A.DESTINATION_ENTITY_TYPE='Cable'
		UNION 
		SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
		A.DESTINATION_PORT_NO,
		A.SOURCE_SYSTEM_ID, A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,A.IS_CABLE_A_END,false
		FROM CONNECTION_INFO A
		WHERE A.DESTINATION_ENTITY_TYPE='FMS' AND 
		A.DESTINATION_NETWORK_ID IN(P_SOURCE_NETWORK_ID,P_DESTINATION_NETWORK_ID)
		AND A.SOURCE_ENTITY_TYPE='Cable';

		--VALIDAT THE CONNECTION IS EXIST IN SAME ROUTE OR NOT
		UPDATE core_planner_fiber_info SET IS_VALID=FALSE
		FROM TEMP_ROUTE_CONNECTION A
		WHERE core_planner_fiber_info.FIBER_NUMBER=A.SOURCE_PORT_NO AND A.SOURCE_ENTITY_TYPE='Cable'
		AND core_planner_fiber_info.CABLE_ID = A.SOURCE_SYSTEM_ID
		AND A.SOURCE_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES); 

		UPDATE core_planner_fiber_info SET IS_VALID=FALSE
		FROM TEMP_ROUTE_CONNECTION A
		WHERE core_planner_fiber_info.FIBER_NUMBER=A.DESTINATION_PORT_NO 
		AND A.DESTINATION_ENTITY_TYPE='Cable' AND core_planner_fiber_info.CABLE_ID = A.DESTINATION_SYSTEM_ID
		AND A.DESTINATION_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES); 
	

		UPDATE TEMP_ISP_PORT_INFO SET IS_VALID=FALSE
		FROM TEMP_ROUTE_CONNECTION A
		WHERE TEMP_ISP_PORT_INFO.PORT_NUMBER = A.SOURCE_PORT_NO AND A.SOURCE_ENTITY_TYPE='FMS' AND A.DESTINATION_ENTITY_TYPE='Cable' AND TEMP_ISP_PORT_INFO.parent_system_id = A.SOURCE_SYSTEM_ID
		AND A.DESTINATION_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES); 

		UPDATE TEMP_ISP_PORT_INFO SET IS_VALID=FALSE
		FROM TEMP_ROUTE_CONNECTION A
		WHERE  TEMP_ISP_PORT_INFO.PORT_NUMBER = A.DESTINATION_PORT_NO AND A.DESTINATION_ENTITY_TYPE='FMS'
		AND A.SOURCE_ENTITY_TYPE='Cable' AND TEMP_ISP_PORT_INFO.parent_system_id = A.DESTINATION_SYSTEM_ID
		AND A.SOURCE_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES); 	

      SELECT (SELECT COUNT(*)FROM (
       SELECT parent_system_id  FROM TEMP_ISP_PORT_INFO 
       WHERE IS_VALID = true GROUP BY parent_system_id HAVING COUNT(*) >= REQUIRED_CORE
        ) AS t1) <> (SELECT COUNT(distinct PARENT_SYSTEM_ID) FROM ISP_PORT_INFO 
       WHERE PARENT_SYSTEM_ID IN (P_SOURCE_SYSTEM_ID, P_DESTINATION_SYSTEM_ID)) into Is_AvailableODF;
       
		IF V_IS_VALID and Is_AvailableODF
		THEN
			V_IS_VALID:=FALSE;
			V_MESSAGE:='All the ports of the ODF are connected to different routes, or adjacent ports are not available in the source/destination ODF!';
		END IF;		   		 		   
          
		/* IF V_IS_VALID AND ((select COUNT(DISTINCT PARENT_SYSTEM_ID) FROM 
		 ISP_PORT_INFO WHERE PARENT_SYSTEM_ID IN (P_SOURCE_SYSTEM_ID,P_DESTINATION_SYSTEM_ID)) 
		 != (SELECT COUNT(DISTINCT parent_system_id) 
              FROM TEMP_ISP_PORT_INFO  WHERE is_valid = TRUE))
		 THEN
		        V_IS_VALID := FALSE;
		        V_MESSAGE := 'Adjacent Ports are not available in Source ODF!';
		 END IF;	*/	
	 
		WITH CABLECTE AS(
		SELECT CABLE_ID,SUM(CASE WHEN link_system_id = 0 THEN 1 ELSE 0 END) AS AVAILABLE_CORE
		FROM CORE_PLANNER_FIBER_INFO WHERE USER_ID = P_USER_ID GROUP BY CABLE_ID
		)
		UPDATE TEMP_CABLE_ROUTES 
		SET IS_VALID = FALSE, 
   		 MESSAGE = 'Unavailable Core'
		FROM CABLECTE
		WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID = CABLECTE.CABLE_ID 
  		AND CABLECTE.AVAILABLE_CORE < REQUIRED_CORE;

		IF V_IS_VALID AND EXISTS(SELECT 1 FROM TEMP_CABLE_ROUTES WHERE IS_VALID = FALSE AND
		MESSAGE = 'Unavailable Core')
		THEN
			V_IS_VALID:=FALSE;
			V_MESSAGE:='Required Core is not available';
		END IF;			
	  	   
	 IF V_IS_VALID AND ((select COUNT(DISTINCT cable_id) from CORE_PLANNER_FIBER_INFO ) != (SELECT COUNT(*) FROM TEMP_CABLE_ROUTES )) 
	 THEN
	        V_IS_VALID := FALSE;
	        V_MESSAGE := 'ADJACENT CORES ARE NOT AVAILABLE IN C1 AND C2!';
	 END IF;

		INSERT INTO 
		CORE_PLANNER_LOGS(CABLE_ID, CABLE_NAME, NETWORK_STATUS, TOTAL_CORE, CABLE_LENGTH,ERROR_MSG, IS_VALID, USER_ID,CABLE_NETWORK_ID, AVAIABLE, USED_CORE,B_SYSTEM_ID,A_SYSTEM_ID,a_entity_type ,b_entity_type,SEQ)
		
		SELECT  ATT.SYSTEM_ID, ATT.CABLE_NAME, ATT.NETWORK_STATUS, ATT.TOTAL_CORE, ATT.CABLE_CALCULATED_LENGTH,INFO.MESSAGE, INFO.IS_VALID,
		P_USER_ID,ATT.NETWORK_ID, 
		(SELECT COUNT(*) FROM ATT_DETAILS_CABLE_INFO ACI WHERE ACI.CABLE_ID = ATT.SYSTEM_ID AND (ACI.LINK_SYSTEM_ID=0)) AS AVAILABLE_CORES,
		(SELECT COUNT(*) FROM ATT_DETAILS_CABLE_INFO ACI WHERE ACI.CABLE_ID = INFO.EDGE_TARGETID AND (ACI.LINK_SYSTEM_ID > 0 ) ) AS USED_CORE,
		INFO.B_SYSTEM_ID,INFO.A_SYSTEM_ID,
		INFO.a_entity_type ,INFO.b_entity_type ,INFO.seq 
		FROM ATT_DETAILS_CABLE ATT 
		INNER JOIN TEMP_CABLE_ROUTES INFO ON ATT.SYSTEM_ID = INFO.EDGE_TARGETID AND  INFO.USER_ID = P_USER_ID order by INFO.seq ;		
				 				 		 
	END IF;
END IF;

RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT V_IS_VALID AS status, V_MESSAGE AS message) row;
END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) TO postgres;


---------------------------------------------------------------------------------------------------------


CREATE TABLE public.core_planner_fiber_info (
	cable_id int4 NULL,
	fiber_number int4 NULL,
	is_a_end_through_connectivity bool NULL DEFAULT false,
	is_b_end_through_connectivity bool NULL DEFAULT false,
	error_msg varchar NULL,
	is_valid bool NOT NULL DEFAULT true,
	user_id int4 NULL,
	a_end_status_id int4 NULL DEFAULT 1,
	b_end_status_id int4 NULL DEFAULT 1,
	link_system_id int4 NULL,
	id serial4 NOT NULL,
	seq int4 NULL,
	CONSTRAINT core_planner_fiber_info_pkey PRIMARY KEY (id)
);