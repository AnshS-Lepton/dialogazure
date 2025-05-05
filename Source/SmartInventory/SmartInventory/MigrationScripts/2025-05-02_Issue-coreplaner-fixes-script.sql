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
  v_source_ports character varying;
  v_destination_ports character varying;
 
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
ID serial4 NOT NULL,
A_SYSTEM_ID INTEGER,
A_entity_type character varying,
A_network_id character varying,
SP_GEOMETRY GEOMETRY,
SEQ INTEGER
)ON COMMIT DROP;
		
     v_network_id := '';
     v_system_id := 0;
    p_destination_network_id = destination_network_id;
    --truncate table temp_connection restart identity;
    --truncate table connection_log_details;
    -- Create a temporary table for connection data
   CREATE temp TABLE temp_connection (
        id serial4 NOT null,
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
        splicing_source character varying(100),
        is_selected boolean NOT NULL DEFAULT true
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

		create temp table connection_log_details
		(
		id serial,
		destination_systemid integer,		
		destination_networkid character varying,source_no integer,
		destination_port_no integer,
		destination_entitytype character varying
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
      INSERT INTO TEMP_POINT_MASTER(SEQ,A_SYSTEM_ID,A_network_id,A_entity_type,SP_GEOMETRY  )
      select A.seq, A.a_system_id,P.COMMON_NAME,A.a_entity_type,P.SP_GEOMETRY from (      
      SELECT seq,a_system_id,a_entity_type  FROM core_planner_logs  where user_id = p_user_id and is_valid =TRUE
      union
      SELECT seq,b_system_id,b_entity_type FROM core_planner_logs where user_id = p_user_id and is_valid =TRUE )a
      INNER JOIN point_master P ON P.SYSTEM_ID=A.a_system_id AND P.entity_type=A.a_entity_type;
     
    delete from TEMP_POINT_MASTER t1 WHERE seq > (
    SELECT MIN(seq)
    FROM TEMP_POINT_MASTER t2
    WHERE t1.a_system_id = t2.a_system_id
    AND t1.a_network_id = t2.a_network_id
    );    

		if((SELECT COUNT(*) FROM core_planner_logs where user_id = p_user_id) = 1 AND 
       (SELECT COUNT(*) FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') = 2)
		   then 
		UPDATE TEMP_POINT_MASTER
		SET seq = 
		  case when a_network_id = source_network_id THEN 0     
		    ELSE 1 end  WHERE a_entity_type = 'FMS';
		else 
		UPDATE TEMP_POINT_MASTER
		SET seq = 0
		  where a_entity_type = 'FMS' AND seq = 1 and (SELECT COUNT(*) FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') = 2;
		end if;

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
   	
        FOR rec IN (SELECT * FROM TEMP_POINT_MASTER ORDER BY 
    CASE WHEN (SELECT COUNT(*) FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') = 1
             AND (SELECT seq FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') <> 1
        THEN -seq
        ELSE seq
	    END
	 )
        LOOP									
		    if(rec.a_entity_type='FMS')
		    then

			 SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) 
			  AS IS_CABLE_A_END INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
			  WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) or
			  ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)));				    
										
            raise info 'v_cable_details_left A : %',v_cable_details_left;
                   		    
		    
			WITH recursive table1 AS (
			    -- Simulating Table-1			    
			   ( SELECT parent_system_id,parent_network_id, port_number FROM temp_isp_port_info 
			    where parent_system_id=rec.a_system_id --and port_status_id =1 
			    and is_valid = true and p_odf_network_id = parent_network_id 
			    ORDER BY port_number limit required_core
			    )
			    union all
			    
			    (SELECT parent_system_id,parent_network_id, port_number FROM temp_isp_port_info 
			    where parent_system_id=rec.a_system_id --and port_status_id =1 
			    and is_valid = true and p_odf_network_id <> parent_network_id
			    ORDER BY port_number )
			    
			),
		   fiber_pairs AS (
		   
		   	select * from(SELECT 
			    t1.id AS id, 
			    t1.cable_id,
			    t1.fiber_number AS fiber1,
			    COALESCE(t2.fiber_number, 0) AS fiber2,  -- If no pair, set fiber2 to 0
			    LEAD(t1.cable_id) OVER (PARTITION BY t1.fiber_number ORDER BY t1.cable_id) AS next_cable,
			    CASE 
			        WHEN t2.fiber_number IS NULL THEN 0  -- If pair is missing (no even-numbered fiber for odd)
			        WHEN t2.fiber_number != t1.fiber_number + 1 THEN 0  -- If next fiber is not a valid pair
			        ELSE t1.seq
			    END AS seq 
			FROM core_planner_fiber_info t1
			LEFT JOIN core_planner_fiber_info t2 
			    ON t1.cable_id = t2.cable_id 
			    AND t2.fiber_number = t1.fiber_number + 1  -- Pair with the next fiber (even-numbered)
			    AND t2.user_id = p_user_id
			WHERE t1.user_id = p_user_id 
			ORDER BY t1.id, t1.fiber_number) t where NOT (fiber2 = 0 AND fiber1 % 2 = 0)
			    
			),
			-- Start the chain from every valid fiber pair
			chain_start AS (
			    SELECT 
			        seq,fiber1, fiber2, cable_id,
			        1 AS depth,
			        cable_id AS first_cable
			    FROM fiber_pairs where cable_id = v_cable_details_left.system_id   
			),
			-- Recursively add more cables in the same fiber pair
			recursive_chain AS (
			    SELECT * FROM chain_start
			    UNION ALL
			    SELECT 
			        fs.seq,fs.fiber1, fs.fiber2, fs.cable_id,
			        rc.depth + 1,
			        rc.first_cable
			    FROM fiber_pairs fs
			    JOIN recursive_chain rc
			        ON fs.fiber1 = rc.fiber1
			        AND fs.fiber2 = rc.fiber2
			        AND fs.seq = rc.seq+1  -- move "downward" in cable_id
			),
     	   existing_connections AS (
		    -- Find already connected fibers in TEMP_ROUTE_CONNECTION
		      SELECT SOURCE_SYSTEM_ID,
		        SOURCE_ENTITY_TYPE,
		        SOURCE_PORT_NO,
		        DESTINATION_SYSTEM_ID , 
		        DESTINATION_ENTITY_TYPE , 
		        DESTINATION_PORT_NO, 
		        IS_CABLE_A_END FROM (
		        SELECT  
		        SOURCE_SYSTEM_ID,
		        SOURCE_ENTITY_TYPE,
		        SOURCE_PORT_NO,
		        DESTINATION_SYSTEM_ID , 
		        DESTINATION_ENTITY_TYPE , 
		        DESTINATION_PORT_NO, 
		        IS_CABLE_A_END
		        FROM TEMP_ROUTE_CONNECTION
		        WHERE SOURCE_ENTITY_TYPE = 'FMS' and SOURCE_SYSTEM_ID = rec.a_system_id 
		        and destination_ENTITY_TYPE = 'Cable' and destination_system_id= 
		        v_cable_details_left.SYSTEM_ID and IS_CABLE_A_END = v_cable_details_left.IS_CABLE_A_END 
		        UNION
		        SELECT 
		        DESTINATION_SYSTEM_ID , 
		        DESTINATION_ENTITY_TYPE , 
		        DESTINATION_PORT_NO,
		        SOURCE_SYSTEM_ID,
		        SOURCE_ENTITY_TYPE,
		        SOURCE_PORT_NO,
		        IS_CABLE_B_END	       
		        FROM TEMP_ROUTE_CONNECTION
		        WHERE DESTINATION_ENTITY_TYPE = 'FMS' and DESTINATION_SYSTEM_ID = rec.a_system_id 
		        and SOURCE_ENTITY_TYPE = 'Cable' and source_system_id= 
		        v_cable_details_left.SYSTEM_ID  and IS_CABLE_B_END = v_cable_details_left.IS_CABLE_A_END 
		     ) AS existing
	      ),
			    right_existing_connection as(
			    select * from existing_connections ec where 
			    ec.source_system_id = rec.a_system_id  and  ec.source_port_no not in (select port_number from table1 order by port_number limit required_core) and ec.DESTINATION_SYSTEM_ID = v_cable_details_left.system_id  and p_odf_network_id = (select parent_network_id from table1 limit 1)  			    			
			    ),
			    checking_right_existing_connection as(
			    SELECT port_number
					FROM (
						SELECT b.port_number, 1 AS priority
						FROM existing_connections ec
						JOIN table1 b ON ec.source_system_id = b.parent_system_id
						WHERE ec.DESTINATION_SYSTEM_ID = v_cable_details_LEFT.system_id
						  AND ec.DESTINATION_ENTITY_TYPE = 'Cable'
						  AND ec.DESTINATION_PORT_NO IN (
							  SELECT destination_port_no 
							  FROM connection_log_details 
							  ORDER BY id DESC 
							  LIMIT required_core
						  )
						  AND ec.source_port_no = b.port_number

						UNION ALL

						SELECT b2.port_number, 2 AS priority
						FROM table1 b2
						WHERE NOT EXISTS (
							SELECT 1
							FROM existing_connections ec
							WHERE ec.source_system_id = b2.parent_system_id
							  AND ec.DESTINATION_SYSTEM_ID = v_cable_details_LEFT.system_id
							  AND ec.DESTINATION_ENTITY_TYPE = 'Cable'
							  AND ec.DESTINATION_PORT_NO IN (
								  SELECT destination_port_no 
								  FROM connection_log_details 
								  ORDER BY id DESC 
								  LIMIT required_core
							  )
							  AND ec.source_port_no = b2.port_number
						) and port_number not in (select source_port_no from existing_connections ec where 
			    ec.source_system_id =  rec.a_system_id and ec.DESTINATION_port_no not in (
		                    SELECT destination_port_no 
		                    FROM connection_log_details 
		                    ORDER BY id DESC 
		                    LIMIT required_core ) and ec.DESTINATION_SYSTEM_ID = v_cable_details_LEFT.system_id  ) ) AS combined
					ORDER BY priority, port_number
					LIMIT required_core

			    ),
			    
			    
			fiber_availability AS (
		    SELECT  fiber1,fiber2, first_cable, continuous_count from 
		   (SELECT  fiber1,fiber2, first_cable, COUNT(*) AS continuous_count
            FROM recursive_chain
            GROUP BY fiber1, fiber2, first_cable
            ORDER BY continuous_count DESC, fiber1 
			) t1 where (t1.fiber1 in				
				(
				select b.DESTINATION_port_no from table1 a
				inner join right_existing_connection b on (a.port_number=b.source_port_no
				and a.parent_network_id = p_odf_network_id 	)
				)
				or 				
				t1.fiber1 not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b 
				)
				)
				AND 
				( ( required_core = 1 AND (t1.fiber2 IS NULL OR t1.fiber2 = 0) ) or 
				t1.fiber2 in				
				(
				(select b.DESTINATION_port_no from table1 a
				inner join right_existing_connection b on (a.port_number=b.source_port_no
				and a.parent_network_id = p_odf_network_id 	)) 
				) 
				or 				
				t1.fiber2 not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b
				)
				)
				ORDER BY t1.continuous_count DESC, t1.fiber1
           ),
           
			match_avialibility AS (
			select COALESCE( (select inf.continuous_count from (select * from table1 
			order by port_number limit 1) t1 join 
			fiber_availability inf on inf.fiber1 = t1.port_number or inf.fiber2 = t1.port_number+1
			where t1.parent_network_id = p_odf_network_id and inf.fiber1 % 2 = 1),0) >=
			 COALESCE((SELECT MAX(continuous_count) FROM fiber_availability ), 0) 
			 as higher_match_avialibility
			),
			exact_match AS (
			SELECT COUNT(*) = required_core AS is_match
			FROM (
			    SELECT distinct *
			    FROM table1 t1
			    JOIN core_planner_fiber_info inf 
			        ON inf.fiber_number = t1.port_number
			    WHERE inf.cable_id = v_cable_details_left.system_id and inf.user_id = p_user_id
			      and true = (select higher_match_avialibility from match_avialibility)
			 ) AS selected_fibers_match
			),
			downward_match AS (
			   -- Rank fiber pairs based on availability (highest first), then by fiber1 descending
			  SELECT  fiber1, fiber2, continuous_count,
			  RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
			  FROM fiber_availability F
			  JOIN table1 d  -- Properly referencing the threshold
		      on f.fiber1 > d.port_number where f.fiber1 % 2 = 1 
			 ),	
			upward_match AS (
				   -- Rank fiber pairs based on availability (highest first), then by fiber1 descending
		      SELECT  fiber1, fiber2, continuous_count,
			  RANK() OVER (ORDER BY continuous_count DESC, fiber1 asc) AS rank
			  FROM fiber_availability F
			  JOIN table1 d  -- Properly referencing the threshold
              on f.fiber1 <= d.port_number where f.fiber1 % 2 = 1  and  NOT EXISTS (
	        -- Check if an exact match is available
	        SELECT 1
	        FROM downward_match 
	        ) ),
	  cable_connection_squence as(
				    (select source_port_no,DESTINATION_PORT_NO from existing_connections where source_port_no in (select port_number from table1) and  MOD(source_port_no, 2) <> MOD(destination_port_no, 2) and destination_system_id = v_cable_details_left.system_id and (select parent_network_id from table1 limit 1)= p_odf_network_id )
			 union 
		(select source_port_no,DESTINATION_PORT_NO from existing_connections where destination_port_no in 	(select destination_port_no from connection_log_details order by id desc limit required_core)
		 and  MOD(source_port_no, 2) <> MOD(destination_port_no, 2) and destination_system_id = v_cable_details_left.system_id and (select parent_network_id from table1 limit 1) <> p_odf_network_id)
				    
				    ),

		   matched_fibers AS (			
			SELECT DISTINCT 
			    t1.parent_system_id, 
			    t1.parent_network_id, 
			    t1.port_number,
			    t2.cable_id, 
			    v_cable_details_left.network_id AS network_id,
			    t2.fiber_number, 
			    v_cable_details_left.IS_CABLE_A_END AS IS_CABLE_A_END,
			    ROW_NUMBER() OVER (
			        PARTITION BY t1.port_number 
			        ORDER BY t2.fiber_number
			    ) AS port_rank
				from (SELECT * FROM table1 WHERE
                CASE 
                WHEN p_odf_network_id <> parent_network_id THEN 
                port_number in (select * from checking_right_existing_connection)
                ELSE TRUE
            END
        ORDER BY port_number
        LIMIT required_core) t1
					JOIN core_planner_fiber_info t2 
					    ON t2.cable_id = v_cable_details_left.system_id 
					     and t2.user_id = p_user_id
					LEFT JOIN exact_match em ON TRUE
					WHERE (  case 
					            WHEN p_odf_network_id <> t1.parent_network_id THEN 
		                t2.fiber_number IN (
		                    SELECT destination_port_no 
		                    FROM connection_log_details 
		                    ORDER BY id DESC 
		                    LIMIT required_core )
		            WHEN em.is_match = TRUE THEN 
		                t2.fiber_number = t1.port_number
		                else
		                t2.fiber_number IN (
		                    SELECT fiber1 FROM downward_match WHERE rank = 1
		                    UNION
		                    SELECT fiber2 FROM downward_match WHERE rank = 1
		                    UNION 
		                    SELECT fiber1 FROM upward_match WHERE rank = 1
		                    UNION
		                    SELECT fiber2 FROM upward_match WHERE rank = 1 )
		        END )
		
				ORDER BY t1.port_number		
				--LIMIT required_core
              ),
              
             final_result AS (
               SELECT * from (SELECT *, 
               ROW_NUMBER() OVER (
               PARTITION BY fiber_number 
               ORDER BY port_number
                ) AS fiber_rank
                FROM matched_fibers )t WHERE case when (select count(1) from cable_connection_squence) <> 1
                then
                t.port_rank =  t.fiber_rank 
                else 
                (t.port_number = (select source_port_no from cable_connection_squence)  and t.fiber_number = (select destination_port_no from cable_connection_squence))
    	           or 
    	          ( t.port_number <> (select source_port_no from cable_connection_squence) and 
    	           t.fiber_number <> (select destination_port_no from cable_connection_squence)) 
                end                             
             ),	
          
			check_existing AS (
		    -- Check if ANY fiber in final_result is already in TEMP_ROUTE_CONNECTION
	        SELECT  source_system_id,source_entity_type,source_port_no,is_cable_a_end,
		    destination_system_id, DESTINATION_PORT_NO from 
		    (SELECT e.source_system_id,e.source_entity_type,e.source_port_no,e.is_cable_a_end,
		    e.destination_system_id, e.DESTINATION_PORT_NO
		    FROM (select * from final_result ) fr1
		    JOIN existing_connections e
		      ON e.source_system_id = fr1.parent_system_id and
				    e.destination_system_id = v_cable_details_left.system_id and 
				    (e.DESTINATION_PORT_NO = fr1.fiber_number or e.source_port_no = fr1.port_number)
			)t	   
			
			),			
			filtered_final_result as(
			 SELECT fr.* FROM (select * from final_result ) fr
				LEFT JOIN check_existing ce 
				    ON ce.source_system_id = fr.parent_system_id AND ce.source_port_no = fr.port_number				    
				WHERE ce.source_port_no IS null 
				limit required_core
			),
				final_insert AS (
		    -- Insert into temp_connection only if NOT in check_existing
		    INSERT INTO temp_connection (
		        source_system_id, source_network_id, source_entity_type, source_port_no, 
		        destination_system_id, destination_network_id, destination_entity_type,
		        destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
		        created_by, splicing_source
		    ) 
			    SELECT 
			        fr.parent_system_id, fr.parent_network_id, 'FMS', fr.port_number, 
			        fr.cable_id, fr.network_id, 'Cable', fr.fiber_number, 
			        FALSE, fr.IS_CABLE_A_END, p_user_id, 'CorePlanning'
			    FROM filtered_final_result fr
			    ORDER BY fr.port_number
			    LIMIT required_core
			),
			insert_log_filtered AS (
		    -- Insert records from filtered_final_result
		   INSERT INTO connection_log_details (
			    destination_systemid, destination_networkid, destination_port_no, destination_entitytype
			)
			SELECT 
			    fr.cable_id, fr.network_id, fr.fiber_number, 'Cable'
			FROM filtered_final_result fr
			 where fr.cable_id = v_cable_details_left.SYSTEM_ID
		    LIMIT required_core
		  )
     	  INSERT INTO connection_log_details (
       		 destination_systemid, destination_networkid, destination_port_no, destination_entitytype
		    )
		    SELECT 
		        distinct destination_system_id, NULL, DESTINATION_PORT_NO, 'Cable'
		    FROM check_existing ce
		    where ce.destination_system_id = v_cable_details_left.system_id	 ;
		   -- ORDER BY source_port_no;
	  
		else		

			SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
			AS IS_CABLE_A_END 
			INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
			WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
			OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) 
			and a.system_id in
		   (select destination_systemid from connection_log_details order by id desc limit 1)  LIMIT 1;
				
			SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) 
			AS IS_CABLE_A_END 
			INTO v_cable_details_right FROM TEMP_LINE_MASTER A				
			WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
			OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) 
			AND SYSTEM_ID NOT IN(v_cable_details_left.SYSTEM_ID)  LIMIT 1;

		 raise info 'v_cable_details_left 1 : %',v_cable_details_left;


		if (v_cable_details_left is not null and v_cable_details_right is not null)
		then
            raise info 'v_cable_details_right 1 : %',v_cable_details_right;
     

			WITH recursive table1 AS (
				SELECT cable_id, fiber_number,v_cable_details_left.network_id as network_id,
				v_cable_details_left.IS_CABLE_A_END as IS_CABLE_A_END
				FROM core_planner_fiber_info where cable_id = v_cable_details_left.system_id				
				and is_valid = true  and fiber_number IN 
				(SELECT destination_port_no FROM connection_log_details t1 
				order by t1.id  desc limit required_core) and core_planner_fiber_info.user_id =p_user_id order by fiber_number  
				limit required_core 				
			) ,
			  fiber_pairs AS (			
			select * from(SELECT 
			    t1.id AS id, 
			    t1.cable_id,
			    t1.fiber_number AS fiber1,
			    COALESCE(t2.fiber_number, 0) AS fiber2,  -- If no pair, set fiber2 to 0
			    LEAD(t1.cable_id) OVER (PARTITION BY t1.fiber_number ORDER BY t1.cable_id) AS next_cable,
			    CASE 
			        WHEN t2.fiber_number IS NULL THEN 0  -- If pair is missing (no even-numbered fiber for odd)
			        WHEN t2.fiber_number != t1.fiber_number + 1 THEN 0  -- If next fiber is not a valid pair
			        ELSE t1.seq
			    END AS seq 
			FROM core_planner_fiber_info t1
			LEFT JOIN core_planner_fiber_info t2 
			    ON t1.cable_id = t2.cable_id 
			    AND t2.fiber_number = t1.fiber_number + 1  -- Pair with the next fiber (even-numbered)
			    AND t2.user_id = p_user_id
			WHERE t1.user_id = p_user_id 
			ORDER BY t1.id, t1.fiber_number) t where NOT (fiber2 = 0 AND fiber1 % 2 = 0)
		),
				-- Start the chain from every valid fiber pair
				chain_start AS (
				    SELECT 
				        seq,fiber1, fiber2, cable_id,
				        1 AS depth,
				        cable_id AS first_cable
				    FROM fiber_pairs where cable_id = v_cable_details_right.system_id  
				),
				-- Recursively add more cables in the same fiber pair
				recursive_chain AS (
					SELECT * FROM chain_start
					UNION ALL
					SELECT fs.seq,fs.fiber1, fs.fiber2, fs.cable_id,
					rc.depth + 1,rc.first_cable
					FROM fiber_pairs fs
					JOIN recursive_chain rc
					ON fs.fiber1 = rc.fiber1
					AND fs.fiber2 = rc.fiber2
					AND fs.seq = rc.seq+1 
					),	
				  existing_connections AS (
			    -- Find already connected fibers in TEMP_ROUTE_CONNECTION
			      SELECT SOURCE_SYSTEM_ID,
			        SOURCE_ENTITY_TYPE,
			        SOURCE_PORT_NO,
			        DESTINATION_SYSTEM_ID , 
			        DESTINATION_ENTITY_TYPE , 
			        DESTINATION_PORT_NO, 
			        IS_CABLE_A_END FROM (
			        SELECT  
			        SOURCE_SYSTEM_ID,
			        SOURCE_ENTITY_TYPE,
			        SOURCE_PORT_NO,
			        DESTINATION_SYSTEM_ID , 
			        DESTINATION_ENTITY_TYPE , 
			        DESTINATION_PORT_NO, 
			        IS_CABLE_A_END
			        FROM TEMP_ROUTE_CONNECTION
			        WHERE SOURCE_ENTITY_TYPE = 'Cable' and SOURCE_SYSTEM_ID = v_cable_details_left.system_id  and IS_CABLE_A_END = v_cable_details_left.IS_CABLE_A_END 
			        and destination_system_id=  v_cable_details_right.system_id 
			        UNION
			        SELECT 
			        DESTINATION_SYSTEM_ID , 
			        DESTINATION_ENTITY_TYPE , 
			        DESTINATION_PORT_NO,
			        SOURCE_SYSTEM_ID,
			        SOURCE_ENTITY_TYPE,
			        SOURCE_PORT_NO,
			        IS_CABLE_B_END	       
			        FROM TEMP_ROUTE_CONNECTION
			        WHERE DESTINATION_ENTITY_TYPE = 'Cable' and DESTINATION_SYSTEM_ID = v_cable_details_left.system_id  and IS_CABLE_B_END = v_cable_details_left.IS_CABLE_A_END 
			        and SOURCE_SYSTEM_ID= v_cable_details_right.system_id
			     ) AS existing
			    ),
			    right_existing_connection as(
			    select * from existing_connections  ec where ec.SOURCE_SYSTEM_ID = v_cable_details_left.system_id and SOURCE_ENTITY_TYPE ='Cable' AND source_PORT_NO not in (SELECT destination_port_no FROM connection_log_details t1 
				order by t1.id  desc limit required_core) and ec.DESTINATION_SYSTEM_ID = v_cable_details_right.system_id
			    ),			     
				fiber_availability AS (
			    SELECT distinct  fiber1,fiber2, first_cable, continuous_count from (  
			    SELECT  fiber1,fiber2, first_cable, COUNT(*) AS continuous_count
	            FROM recursive_chain
	            GROUP BY fiber1, fiber2, first_cable
	            ORDER BY continuous_count DESC, fiber1 
				) t1 where (t1.fiber1 in				
				(
				select b.DESTINATION_port_no from table1 a
				inner join right_existing_connection b on a.fiber_number=b.SOURCE_PORT_NO 		
				)
				or 				
				t1.fiber1 not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b
				)
				)
				AND 
				(t1.fiber2 in				
				(
				select b.DESTINATION_port_no from table1 a
				inner join right_existing_connection b on a.fiber_number=b.SOURCE_PORT_NO			
				) 
				or 				
				t1.fiber2 not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b
				)
				) or t1.fiber2 IS NULL OR t1.fiber2 = 0
				ORDER BY t1.continuous_count DESC, t1.fiber1  
				),
				selected_fibers_match AS (
			    -- Step 3: Select the fiber availability where fiber1 is odd
			    SELECT distinct t2.FIBER_number,t1.FIBER_number as C1_FIBER_NUMBER
			    FROM table1 t1  
			    JOIN core_planner_fiber_info t2  
			        ON t2.FIBER_number = t1.FIBER_number and t2.cable_id  = v_cable_details_right.system_id and t2.user_id = p_user_id
			    WHERE  t2.FIBER_number in				
				(
				select a.fiber_number from table1 a
				inner join right_existing_connection b on a.fiber_number=b.source_port_no			
				)
				or 				
				t2.FIBER_number not in				
				(
				select b.DESTINATION_port_no from right_existing_connection b
				)
					
			    order by t1.FIBER_number 
			   ),
				exact_match AS (
		        SELECT COUNT(*) = required_core AS is_match
		        FROM selected_fibers_match
		        ),
					downward_match AS (						
				    SELECT distinct fiber1, fiber2, continuous_count,
				    RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
				    FROM fiber_availability f
                    JOIN table1 d  -- Properly referencing the threshold
                   on f.fiber1 > d.fiber_number where f.fiber1 % 2 = 1 
				),
					upward_match AS (
				   -- Rank fiber pairs based on availability (highest first), then by fiber1 descending
				    SELECT distinct fiber1, fiber2, continuous_count,
				    RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
				    FROM fiber_availability F
				    JOIN table1 d  -- Properly referencing the threshold
                    on f.fiber1 <= d.fiber_number where f.fiber1 % 2 = 1  and NOT EXISTS (
				        SELECT 1
				        FROM downward_match 
				    )) ,
				    cable_connection_squence as(
				    select source_port_no,DESTINATION_PORT_NO from existing_connections where SOURCE_PORT_NO in (select destination_port_no from connection_log_details order by id desc limit required_core) and  (MOD(source_port_no, 2) <> MOD(destination_port_no, 2))
				    ),
		 
		      matched_fibers AS (
				SELECT t1.cable_id as c1_cable_id, t1.network_id as c1_network_id,
				t1.fiber_number as c1_fiber_number,t1.IS_CABLE_A_END as C1_IS_CABLE_A_END,
				t2.cable_id as c2_cable_id,v_cable_details_right.network_id as c2_network_id,
				t2.fiber_number as c2_fiber_number, v_cable_details_right.IS_CABLE_A_END  
				as C2_IS_CABLE_A_END ,
				ROW_NUMBER() OVER (
	            PARTITION BY t1.fiber_number 
	            ORDER BY t2.fiber_number
	            ) AS fiber1_rank
			    FROM table1 t1
			    JOIN core_planner_fiber_info t2 ON  t2.cable_id = v_cable_details_right.system_id  			   
				and t2.user_id = p_user_id 		
			    LEFT JOIN exact_match em 
                    ON TRUE
                WHERE 
       		     (CASE 
        		    WHEN em.is_match = TRUE THEN 
        		    case when (select count(1) from cable_connection_squence) = 1 then 
        		     (t1.fiber_number = (select source_port_no from cable_connection_squence)  and t2.fiber_number = (select destination_port_no from cable_connection_squence))
    	           or 
    	           (t2.fiber_number = (select source_port_no from cable_connection_squence) and 
    	           t1.fiber_number = (select destination_port_no from cable_connection_squence))
    	           else
    	            t2.fiber_number = t1.fiber_number end
            	    --AND t2.fiber_number = t1.fiber_number + 1
           		 ELSE 
                t2.fiber_number IN (
                    SELECT fiber1 FROM downward_match WHERE rank = 1
                    UNION
                    SELECT fiber2 FROM downward_match WHERE rank = 1
                    union 
                    SELECT fiber1 FROM upward_match WHERE rank = 1
                    UNION
                    SELECT fiber2 FROM upward_match WHERE rank = 1
                ) 
       			 END  
       			 )
                order by t1.fiber_number --limit required_core
			    
	    	),
	    	
	     final_result AS (
               SELECT * from (SELECT *, 
               ROW_NUMBER() OVER (
               PARTITION BY c2_fiber_number 
               ORDER BY c1_fiber_number
                ) AS fiber2_rank
                FROM matched_fibers)t  WHERE t.fiber1_rank =  t.fiber2_rank
                ),

			check_existing AS (
		    -- Check if ANY fiber in final_result is already in TEMP_ROUTE_CONNECTION
		    SELECT source_system_id,source_entity_type,source_port_no,is_cable_a_end,
		    destination_system_id, DESTINATION_PORT_NO from (SELECT e.source_system_id,e.source_entity_type,e.source_port_no,e.is_cable_a_end,
		    e.destination_system_id, e.DESTINATION_PORT_NO
		    FROM (select * from final_result LIMIT required_core) fr1
		    JOIN existing_connections e
		      ON e.source_system_id = fr1.c1_cable_id 
		     where e.source_port_no = fr1.c1_fiber_number 
		     AND e.is_cable_a_end = fr1.C1_IS_CABLE_A_END) t where t.destination_system_id = v_cable_details_right.system_id --and t.DESTINATION_PORT_NO = c2_fiber_number
			),			
			
			filtered_final_result as(
			 SELECT fr.* FROM (select * from final_result limit required_core ) fr
				LEFT JOIN check_existing ce 
				    ON (ce.source_system_id = fr.c1_cable_id AND ce.source_port_no = fr.c1_fiber_number) 				    
				WHERE ce.source_port_no IS NULL
			),

			final_insert AS (
			-- Final selection: prioritize exact match, then downward, then upward
			INSERT INTO temp_connection (
            source_system_id, source_network_id, source_entity_type, source_port_no, 
            destination_system_id, destination_network_id, destination_entity_type,
            destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
            created_by, splicing_source,equipment_system_id, equipment_network_id, equipment_entity_type)
				
			SELECT c1_cable_id,c1_network_id,'Cable', c1_fiber_number,
			c2_cable_id ,c2_network_id,'Cable',c2_fiber_number,c1_IS_CABLE_A_END,c2_IS_CABLE_A_END,
			p_user_id, 'CorePlanning',rec.a_system_id,rec.a_network_id,rec.a_entity_type
			FROM filtered_final_result				  
			ORDER BY c1_fiber_number 
		),
		insert_log_filtered AS (
		    -- Insert records from filtered_final_result
		    INSERT INTO connection_log_details (
		        destination_systemid, destination_networkid, destination_port_no, destination_entitytype
		    )
		    SELECT 
		        fr.c2_cable_id, fr.c2_network_id, fr.c2_fiber_number, 'Cable'
		    FROM filtered_final_result fr
		    ORDER BY c2_fiber_number 
		    LIMIT required_core
		)
     	  INSERT INTO connection_log_details (
       		 destination_systemid, destination_networkid, destination_port_no, destination_entitytype
		    )
		    SELECT 
		        distinct destination_system_id, NULL, DESTINATION_PORT_NO, 'Cable'
		    FROM existing_connections ce
		    where ce.source_system_id = v_cable_details_left.system_id	and source_port_no in (select destination_port_no from connection_log_details order by id desc limit required_core)
		    and ce.destination_system_id = v_cable_details_right.system_id;    
		    --ORDER BY source_port_no;
		end if;	  
	   
	end if;
        END LOOP;

		if (SELECT COUNT(*) FROM temp_connection) > 0
		then 
		 raise info 'end splicing 1 : %',p_user_id;
	    perform(fn_auto_provisioning_save_connections());
		
		
		end if;	
	
		update att_details_cable_info  set a_end_status_id=2
		from temp_connection tc
		where cable_id=tc.source_system_id and tc.source_entity_type='Cable'
		and fiber_number=tc.source_port_no
		and tc.is_source_cable_a_end =true;
		
		update att_details_cable_info  set b_end_status_id=2 
		from temp_connection tc
		where cable_id=tc.source_system_id and tc.source_entity_type='Cable'
		and fiber_number=tc.source_port_no
		and tc.is_source_cable_a_end=false;
		
		update att_details_cable_info set a_end_status_id=2 
		from temp_connection tc
		where cable_id=tc.destination_system_id and tc.destination_entity_type='Cable'
		and fiber_number=tc.destination_port_no
		and tc.is_destination_cable_a_end=true;
		
		update att_details_cable_info  set b_end_status_id=2 
		from temp_connection tc
		where cable_id=tc.destination_system_id and tc.destination_entity_type='Cable'
		and fiber_number=tc.destination_port_no
		and tc.is_destination_cable_a_end=false;
	
	    update isp_port_info  set port_status_id=2 
		from temp_connection tc
		where parent_system_id=tc.destination_system_id 
		and upper(parent_entity_type)=upper(tc.destination_entity_type)
		and port_number = tc.destination_port_no ;
	
	    update isp_port_info  set port_status_id=2 
		from temp_connection tc
		where parent_system_id=tc.source_system_id 
		and upper(parent_entity_type)=upper(tc.source_entity_type)
		and port_number = tc.source_port_no ;

		insert into temp_connected_cables(cable_id,fiber_number)
		select destination_systemid, destination_port_no from connection_log_details;					
					
		update att_details_cable_info set  fiber_status ='Reserved',
		link_system_id=coalesce(p_link_system_id,0)
		from temp_connected_cables b
		where att_details_cable_info.cable_id=b.cable_id 
	    and b.fiber_number=att_details_cable_info.fiber_number;


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
		

       return query select row_to_json(row) from ( select true as status, 'The required core has been spliced, and the fiber link has been successfully attached.' as message ) row;
      end if;
  END;
 
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_core_planner_splicing(int4, int4, varchar, varchar, varchar, int4) TO postgres;


------------------------------------------------------------------------------------------------------------------



CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_details(p_systemid integer, p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_userid integer, p_searchfrom timestamp without time zone, p_searchto timestamp without time zone)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$


 DECLARE
   sql TEXT; 
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords INTEGER; 
   LowerStart  character varying;
   LowerEnd  character varying;
   v_user_role_id integer;
   s_layer_columns_val text; 

BEGIN

sql:='';
LowerStart:='';
LowerEnd:='';

 --IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
    -- LowerStart:='LOWER(';
    -- LowerEnd:=')';
--END IF;
  if(p_sortcolname='OP_ALIAS')then p_sortcolname:='Service Id'; end if;
select role_id into v_user_role_id from user_master where user_id=p_userId;

-- FETCH ALL COLUMNS FROM COLUMN SETTINGS TABLE
	 
	SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM(
	 SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM fiber_link_columns_settings WHERE is_active=true and UPPER(column_name) not in('TOTAL_ROUTE_LENGTH','GIS_LENGTH') order by Column_sequence) A;
	 

--- TO GET TOTAL_ROUTE_LENGTH & GIS_LENGTH
CREATE TEMP TABLE TEMP_CABLE_INFO(
 LINK_SYSTEM_ID INTEGER,
 CABLE_ID INTEGER
)ON COMMIT DROP;
 
CREATE TEMP TABLE TEMP_FIBER_LINK_DETAILS(
  LINK_SYSTEM_ID INTEGER,
  TOTAL_ROUTE_LENGTH DOUBLE PRECISION,
  GIS_LENGTH DOUBLE PRECISION
 )ON COMMIT DROP;

INSERT INTO TEMP_CABLE_INFO(LINK_SYSTEM_ID,CABLE_ID)
SELECT DISTINCT LINK_SYSTEM_ID,CABLE_ID FROM ATT_DETAILS_CABLE_INFO C
JOIN ATT_DETAILS_FIBER_LINK FIBER ON FIBER.SYSTEM_ID=C.LINK_SYSTEM_ID WHERE LINK_SYSTEM_ID>0;

 
INSERT INTO TEMP_FIBER_LINK_DETAILS(LINK_SYSTEM_ID,TOTAL_ROUTE_LENGTH,GIS_LENGTH)
 
SELECT DISTINCT T.LINK_SYSTEM_ID,ROUND((SUM(COALESCE(CABLE.cable_calculated_length,0)) + SUM(COALESCE(LOOP.LOOP_LENGTH,0)))::NUMERIC,2) AS TOTAL_ROUTE_LENGTH ,
ROUND((SUM(COALESCE(CABLE.CABLE_MEASURED_LENGTH,0)))::NUMERIC,2) AS CABLE_MEASURED_LENGTH
FROM TEMP_CABLE_INFO T
JOIN ATT_DETAILS_CABLE CABLE ON CABLE.SYSTEM_ID=T.CABLE_ID
LEFT JOIN( SELECT SUM(Loop_length) as loop_length,cable_system_id from ATT_DETAILS_LOOP LOOP group by LOOP.cable_system_id)loop ON LOOP.CABLE_SYSTEM_ID= CABLE.SYSTEM_ID
group by T.link_system_id
ORDER BY T.link_system_id DESC;

 
-- MANAGE SORT COLUMN NAME
IF (coalesce(TRIM(P_SORTCOLNAME,''))!='') THEN 

SELECT TRIM( trailing '	' from ''||P_SORTCOLNAME||'') into P_SORTCOLNAME;
select column_name into P_SORTCOLNAME from fiber_link_columns_settings WHERE UPPER(DISPLAY_NAME)=UPPER(P_SORTCOLNAME);
End IF;

 raise info'P_SORTCOLNAME% ',P_SORTCOLNAME;
  raise info'S_LAYER_COLUMNS_VAL% ',S_LAYER_COLUMNS_VAL;

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'fl.system_id' ELSE  'fl.'|| P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS S_No, 
fl.system_id,fl.network_id as "Network Id",'||S_LAYER_COLUMNS_VAL||', f2.TOTAL_ROUTE_LENGTH AS "Total Route Length(meter)",f2.GIS_LENGTH AS "GIS Length(meter)"
FROM vw_att_details_fiber_link fl
left join TEMP_FIBER_LINK_DETAILS f2 on fl.system_id=f2.link_system_id WHERE 1=1  ';
	

 IF(p_systemid >0 )THEN 
	sql:= sql ||'and fl.system_id='||p_systemid||'';
 END IF;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2% ',sql;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;
	-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a ' INTO TotalRecords;
 
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords,*
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 
RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_fiber_link_details(int4, varchar, varchar, int4, int4, varchar, varchar, int4, timestamp, timestamp) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_fiber_link_details(int4, varchar, varchar, int4, int4, varchar, varchar, int4, timestamp, timestamp) TO public;
GRANT ALL ON FUNCTION public.fn_get_fiber_link_details(int4, varchar, varchar, int4, int4, varchar, varchar, int4, timestamp, timestamp) TO postgres;


------------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_get_fiberlink_schematicview(p_link_system_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
 
DECLARE 
v_Query text;
Query text; 
rec record;
v_from_id integer;
v_from_entity_type character varying;
nodes text;
edges text;
nodesDownstream text;
edgesDownstream text;
legends text;
cables text;
BEGIN
--truncate table temp_edges;
--truncate table cpf_temp_result;
--truncate table temp_nodes;

 create temp table cpf_temp_result
 (
 id serial,
 source_system_id integer,
 source_network_id character varying,
 source_port_no integer, 
 source_entity_type character varying,
 source_entity_title character varying,
 destination_system_id integer,
 destination_network_id character varying,
 destination_port_no integer,
 destination_entity_type character varying,
 destination_entity_title character varying,
 via_system_id integer,
 via_network_id character varying,
 via_entity_type character varying,
 via_port_no integer
 )on commit drop;
 
 create temp table temp_nodes
 (
 id serial,
 source_system_id integer,
 source_network_id character varying,
 connected_ports character varying, 
 source_entity_type character varying,
 source_entity_title character varying,
 node_name 	character varying
 )on commit drop;
 
 create temp table temp_edges
 (
 id serial,
 from_id integer,
 from_entity_type character varying,
 from_network_id character varying,
 to_id integer,
 to_network_id character varying,
 to_entity_type character varying,
 label character varying,
 via_ports character varying,
 network_status character varying,
 cable_type character varying,
 color_code character varying,
 total_core integer,
 cable_category character varying,
 	cable_calculated_length double precision
 )on commit drop;

insert into cpf_temp_result(source_system_id ,source_network_id,source_port_no , source_entity_type 
,destination_system_id ,destination_network_id ,destination_port_no ,destination_entity_type,
via_system_id ,via_network_id ,via_port_no ,via_entity_type)
select c1.source_system_id ,c1.source_network_id,c1.source_port_no , c1.source_entity_type 
,c2.destination_system_id ,c2.destination_network_id ,c2.destination_port_no ,c2.destination_entity_type,
c1.destination_system_id,c1.destination_network_id,c1.destination_port_no ,c1.destination_entity_type
from att_details_cable_info ci
inner join connection_info c1 on ci.cable_id=c1.destination_system_id and c1.destination_entity_type='Cable' and c1.destination_port_no=ci.fiber_number
left join connection_info c2 on c2.source_system_id=c1.destination_system_id and c2.source_entity_type=c1.destination_entity_type and c2.source_port_no=c1.destination_port_no
where ci.link_system_id=p_link_system_id
union
select c1.source_system_id ,c1.source_network_id,c1.source_port_no , c1.source_entity_type 
,c2.destination_system_id ,c2.destination_network_id ,c2.destination_port_no ,c2.destination_entity_type,
c1.destination_system_id,c1.destination_network_id,c1.destination_port_no ,c1.destination_entity_type
from att_details_cable_info ci
inner join connection_info c1 on ci.cable_id=c1.destination_system_id and c1.destination_entity_type='Cable' and c1.destination_port_no=ci.fiber_number
and c1.source_entity_type='FMS'
left join connection_info c2 on c2.source_system_id=c1.destination_system_id 
and c2.source_entity_type=c1.source_entity_type and c2.source_port_no=c1.destination_port_no
where ci.link_system_id=p_link_system_id;

insert into cpf_temp_result(source_system_id ,source_network_id,source_port_no , source_entity_type 
,destination_system_id ,destination_network_id ,destination_port_no ,destination_entity_type,
via_system_id ,via_network_id ,via_port_no ,via_entity_type)
select  pm.system_id ,pm.common_name,null , coalesce(pm.entity_type,'SpliceClosure') 
,pm2.system_id ,pm2.common_name ,null ,coalesce(pm2.entity_type,'SpliceClosure') ,
lm.system_id,lm.common_name,ci.fiber_number ,lm.entity_type
from att_details_cable_info ci
inner join line_master lm on ci.cable_id=lm.system_id and lm.entity_type='Cable'
left join point_master pm on st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2)) and pm.entity_type in('SpliceClosure')
left join point_master pm2 on st_within(pm2.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2))
and pm2.entity_type in('SpliceClosure')
where ci.link_system_id=p_link_system_id and ci.cable_id not in(select via_system_id from cpf_temp_result);

update cpf_temp_result set source_system_id=a.system_id,
source_entity_type=a.entity_type,
source_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on (st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2))
or st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2)))
and pm.entity_type in('SpliceClosure')
where a.source_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and source_system_id is null;

update cpf_temp_result set destination_system_id=a.system_id,
destination_entity_type=a.entity_type,
destination_network_id=a.network_id
from(
select distinct pm.system_id,lm.system_id as cable_id ,pm.entity_type,pm.common_name as network_id
from cpf_temp_result a
inner join line_master lm on a.via_system_id=lm.system_id and lm.entity_type='Cable'
inner join point_master pm on (st_within(pm.sp_geometry,st_buffer_meters(st_endpoint(lm.sp_geometry),2))
or st_within(pm.sp_geometry,st_buffer_meters(st_startpoint(lm.sp_geometry),2)))
and pm.entity_type in('SpliceClosure')
where a.destination_system_id is null
)a where cpf_temp_result.via_system_id=a.cable_id and destination_system_id is null;

delete from cpf_temp_result where source_network_id=destination_network_id;

 --delete from cpf_temp_result where id in(
--select B.id from cpf_temp_result A
	-- inner join cpf_temp_result b on A.source_system_id=b.destination_system_id
	 --and A.source_entity_type=b.destination_entity_type and a.via_system_id=b.via_system_id);
	

WITH duplicates1 AS (
    SELECT id FROM (
        SELECT id, 
               ROW_NUMBER() OVER (PARTITION BY source_system_id , source_entity_type ,destination_system_id ,destination_entity_type 
,via_network_id,via_port_no
								  ORDER BY id) AS row_num
        FROM cpf_temp_result
    ) subquery
    WHERE row_num > 1
)
delete from cpf_temp_result where id in(select id from duplicates1);

WITH duplicates2 AS (
    SELECT id FROM (
        SELECT id, source_network_id ,source_port_no,via_network_id,via_port_no,
               ROW_NUMBER() OVER (PARTITION BY source_network_id ,source_port_no
,via_network_id,via_port_no
								  ORDER BY id) AS row_num
        FROM cpf_temp_result where destination_system_id is null
    ) subquery
    WHERE row_num = 1
)
delete from cpf_temp_result where id in(select id from duplicates2);

insert into temp_nodes(source_system_id,source_network_id,source_entity_type,connected_ports)
select source_system_id,source_network_id,source_entity_type,string_agg(source_port_no::text,',')  from(
select * from(
select source_system_id,source_network_id,source_entity_type,source_port_no 
	from cpf_temp_result 
union
select destination_system_id,destination_network_id,destination_entity_type,destination_port_no 
	from cpf_temp_result)b
	order by b.source_port_no
)t group by source_system_id,source_network_id,source_entity_type;

update temp_nodes set node_name=fms.fms_name
from att_details_fms fms where source_system_id=fms.system_id and source_entity_type='FMS';

update temp_nodes set node_name=fms.spliceclosure_name
from att_details_spliceclosure fms where source_system_id=fms.system_id and source_entity_type='SpliceClosure';

insert into temp_edges(from_id,from_entity_type,
to_id,to_entity_type,label,via_ports,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length)

	select source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,string_agg(via_port_no::text,','),
network_status,cable_type,color_code,total_core,cable_category,
cable_calculated_length
from(
	select coalesce(source_system_id,cbl.a_system_id)as source_system_id, 
	coalesce(source_entity_type,a_entity_type) as source_entity_type,
coalesce(destination_system_id,b_system_id) as destination_system_id,
coalesce(destination_entity_type,b_entity_type) as destination_entity_type
,via_network_id,via_port_no,
cbl.network_status,cbl.cable_type,cbl.total_core,cbl.cable_category,cbls.color_code,
Cast(cbl.cable_calculated_length as decimal(10,2))  as cable_calculated_length
from cpf_temp_result cpf
inner join att_details_cable cbl on cbl.system_id=cpf.via_system_id
left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type 
and cbls.cable_category=cbl.cable_category and cbls.fiber_count=cbl.total_core
order by via_port_no
)a
group by source_system_id, source_entity_type,
destination_system_id,destination_entity_type
,via_network_id,network_status,cable_type,color_code,total_core,cable_category,cable_calculated_length;

update temp_edges set via_ports=b.via_ports
from
(
select label,string_agg(via_ports,',') as via_ports from (select * from temp_edges order by via_ports)a group by label)
b where temp_edges.label=b.label;

WITH duplicates AS (
    SELECT id FROM (
        SELECT id, 
               ROW_NUMBER() OVER (PARTITION BY label
								  ORDER BY id) AS row_num
        FROM temp_edges
    ) subquery
    WHERE row_num > 1
)
delete from temp_edges where id in(select id from duplicates);

   
   -- NODES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select * from(
select row_number() over(partition by id) rn,* from(
select distinct * from (
select CONCAT(source_system_id,source_entity_type) as id,
CONCAT('<b>',replace(node_name,E'\t',''),'</b>','\n',case when ld.is_virtual_port_allowed=false then concat('(',connected_ports,')') end) as label

,source_entity_type as group, null as title from temp_nodes t
	inner join layer_details ld on ld.layer_name=t.source_entity_type where source_entity_type!='Splitter'

) a)b)b where rn=1
) x) into nodesDownstream;

-- EDGES DOWNSTREAM JSON--
Select (select array_to_json(array_agg(row_to_json(x)))from (
select CONCAT(t.from_id,t.from_entity_type) as from,
CONCAT(t.to_id,t.to_entity_type) as to,'<b>'
||(case when t.total_core is not null then '('||t.total_core||'F)' else '' end)||
concat(' (',t.cable_calculated_length::text,' m)')||
concat(' (',t.via_ports,')')||
(CASE WHEN t.via_ports!='' THEN '\n'||'\n' else '' end) as label,

case WHEN t.color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '{"color": "#FF0000"}'::json
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '{"color": "#0000FF"}'::json
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '{"color": "#FD10FD"}'::json
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '{"color": "#000"}'::json
ELSE '{"color": "#0000FF"}'::json
END
ELSE '{"color": "#000"}'::json
END as color,
0 as length,
CASE
WHEN upper(t.network_status::text) = 'P'::text THEN true
WHEN upper(t.network_status::text) = 'A'::text THEN false
ELSE true
END as dashes,
1.5  as width,
concat('{"type": "''curvedCCW''", "roundness":"0.5"}')::json as smooth
from temp_edges t
) x) into edgesDownstream;
   
   --LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (

select distinct * from (
select source_entity_type as entity_type, 
(select layer_title from layer_details where layer_name=source_entity_type) as entity_title,false as upstream 
from temp_nodes
) a where a.entity_type not in ('Cable','Splitter') order by entity_title
) x) into legends;

--CABLE LEGEND IN JSON FORMAT--
select (select array_to_json(array_agg(row_to_json(x)))from (
Select case WHEN color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '#FF0000'::text
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '#0000FF'::text
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '#FD10FD'::text
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '#000'::text
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
(case when total_core is not null then '('||t.total_core||'F)' else '' end))as text,false as upstream
from(
select color_code,cable_category,cable_type,total_core from temp_edges
group by color_code,cable_category,cable_type,total_core )t where t.cable_type is not null
) x) into cables;
   
	return query select row_to_json(result) from (
select '' as entityType, '' as entityTitle,'' as entityDisplayText,
0 as latitude,0 as longitude,
now(),null as nodes,null as edges,nodesDownstream,edgesDownstream,legends,cables
) result;
 

END 
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_fiberlink_schematicview(int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_fiberlink_schematicview(int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_fiberlink_schematicview(int4) TO postgres;


