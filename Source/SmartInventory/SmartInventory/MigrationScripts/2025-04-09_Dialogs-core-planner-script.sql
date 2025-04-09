
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
-- truncate table TEMP_LINE_MASTER;
-- truncate table TEMP_POINT_MASTER;


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
   -- truncate table temp_connection restart identity;
  -- truncate table connection_log_details;
    -- Create a temporary table for connection data
   CREATE TEMP TABLE temp_connection (
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
   	
        FOR rec IN (select * from TEMP_POINT_MASTER  ORDER BY  CASE WHEN a_entity_type = 'FMS' 
        AND seq = 1 THEN 0  ELSE 1 END, CASE 
       WHEN (SELECT COUNT(*) FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') = 1 and 
       (SELECT seq FROM TEMP_POINT_MASTER WHERE a_entity_type = 'FMS') <> 1
        THEN -seq  -- simulates DESC
       ELSE seq     -- ASC
  END)
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
			    SELECT parent_system_id,parent_network_id, port_number FROM temp_isp_port_info 
			    where parent_system_id=rec.a_system_id and port_status_id =1 and is_valid = true 
			    ORDER BY port_number limit required_core
			),
		   fiber_pairs AS (
			     select t1.id as id, t1.cable_id,t1.fiber_number AS fiber1,t2.fiber_number AS fiber2,
				LEAD(t1.cable_id) OVER (PARTITION BY t1.fiber_number ORDER BY t1.cable_id)
				 AS next_cable, t1.seq
				FROM core_planner_fiber_info t1
				inner join core_planner_fiber_info t2 on t1.cable_id=t2.cable_id 
				and t1.fiber_number=t2.fiber_number-1 where t1.is_valid = true and t2.is_valid = true 
			    order by t1.id,t1.fiber_number
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
     
			fiber_availability AS (
		    SELECT  fiber1,fiber2, first_cable, COUNT(*) AS continuous_count
            FROM recursive_chain
            GROUP BY fiber1, fiber2, first_cable
            ORDER BY continuous_count DESC, fiber1 
			),
			match_avialibility AS (
			select COALESCE( (select inf.continuous_count from (select * from table1 
			order by port_number limit 1) t1 join 
			fiber_availability inf on inf.fiber1 = t1.port_number and inf.fiber2 = t1.port_number+1
			where t1.parent_network_id = p_odf_network_id and inf.fiber1 % 2 = 1),0) >=
			 COALESCE((SELECT MAX(continuous_count) FROM fiber_availability ), 0) 
			 as higher_match_avialibility
			),
			exact_match AS (
			SELECT COUNT(*) = required_core AS is_match
			FROM (
			    SELECT *
			    FROM table1 t1
			    JOIN core_planner_fiber_info inf 
			        ON inf.fiber_number = t1.port_number
			    WHERE inf.cable_id = v_cable_details_left.system_id
			      AND (SELECT higher_match_avialibility FROM match_avialibility) = TRUE
			) AS selected_fibers_match
			),
			downward_match AS (
			   -- Rank fiber pairs based on availability (highest first), then by fiber1 descending
			  SELECT  fiber1, fiber2, continuous_count,
			  RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
			  FROM fiber_availability F
			  JOIN table1 d  -- Properly referencing the threshold
		      on f.fiber1 > d.port_number where f.fiber1 % 2 = 1 and NOT EXISTS (
		        -- Check if an exact match is available
		       SELECT 1
		       FROM exact_match em
		       WHERE em.is_match = TRUE
		     ) 
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
		   matched_fibers AS (
           -- Step 5: Match the highest-ranked fiber pairs
          select distinct t1.parent_system_id, t1.parent_network_id, t1.port_number,
           t2.cable_id, v_cable_details_left.network_id as network_id,
           t2.fiber_number, v_cable_details_left.IS_CABLE_A_END as IS_CABLE_A_END,
           (t1.port_number+t2.fiber_number) as remove_duplicate_seq,
            ROW_NUMBER() OVER (
            PARTITION BY t1.port_number 
            ORDER BY t2.fiber_number
            ) AS port_rank
          FROM table1 t1
          JOIN core_planner_fiber_info t2 
          ON t2.cable_id = v_cable_details_left.system_id and 
		  ((v_cable_details_left.IS_CABLE_A_END=true) --and a_end_status_id = 1)
		  or(v_cable_details_left.IS_CABLE_A_END =false))-- and b_end_status_id = 1))	
          LEFT JOIN exact_match em 
           ON TRUE
            WHERE (CASE WHEN p_odf_network_id <> t1.parent_network_id 
             THEN 
               t2.fiber_number IN (
                SELECT destination_port_no FROM connection_log_details t1 
                ORDER BY t1.id DESC LIMIT required_core) 
           WHEN em.is_match = TRUE 
           THEN 
            t2.fiber_number = t1.port_number               
           ELSE 
            t2.fiber_number IN (
                SELECT fiber1 FROM downward_match WHERE rank = 1
                UNION
                SELECT fiber2 FROM downward_match WHERE rank = 1
                UNION 
                SELECT fiber1 FROM upward_match WHERE rank = 1
                UNION
                SELECT fiber2 FROM upward_match WHERE rank = 1
            )  
    END)
                   order by t1.port_number
                   -- LIMIT required_core
                   ),
                   final_result AS (
               SELECT * from (SELECT *, 
               ROW_NUMBER() OVER (
               PARTITION BY fiber_number 
               ORDER BY port_number
                ) AS fiber_rank
                FROM matched_fibers)t  WHERE t.port_rank =  t.fiber_rank
                ),	
            /*    existing_connections AS (
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
	        WHERE SOURCE_ENTITY_TYPE = 'FMS' and SOURCE_SYSTEM_ID = rec.a_system_id and destination_system_id= v_cable_details_left.SYSTEM_ID and IS_CABLE_A_END = v_cable_details_left.IS_CABLE_A_END 
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
	        WHERE DESTINATION_ENTITY_TYPE = 'FMS' and DESTINATION_SYSTEM_ID = rec.a_system_id and source_system_id= v_cable_details_left.SYSTEM_ID and IS_CABLE_B_END = v_cable_details_left.IS_CABLE_A_END 
	     ) AS existing
	    ),	    
			check_existing AS (
		    -- Check if ANY fiber in final_result is already in TEMP_ROUTE_CONNECTION
		    SELECT source_system_id,source_entity_type,source_port_no,is_cable_a_end,
		    destination_system_id, DESTINATION_PORT_NO from (SELECT e.source_system_id,e.source_entity_type,e.source_port_no,e.is_cable_a_end,
		    e.destination_system_id, e.DESTINATION_PORT_NO
		    FROM (select * from final_result LIMIT required_core) fr1
		    JOIN existing_connections e
		      ON e.source_system_id = fr1.parent_system_id 
		     where e.source_port_no = fr1.port_number and e.IS_CABLE_A_END = v_cable_details_left.IS_CABLE_A_END )t where t.destination_system_id = v_cable_details_left.system_id   
			),			
			
			filtered_final_result as(
			 SELECT fr.* FROM (select * from final_result limit required_core ) fr
				LEFT JOIN check_existing ce 
				    ON (ce.source_system_id = fr.parent_system_id AND ce.source_port_no = fr.port_number) 				    
				WHERE ce.source_port_no IS NULL
			),*/
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
			    FROM final_result fr
			    ORDER BY fr.port_number
			    LIMIT required_core
			)
			--insert_log_filtered AS (
		    -- Insert records from filtered_final_result
		   INSERT INTO connection_log_details (
			    destination_systemid, destination_networkid, destination_port_no, destination_entitytype
			)
			SELECT 
			    fr.cable_id, fr.network_id, fr.fiber_number, 'Cable'
			FROM final_result fr
			 where fr.cable_id = v_cable_details_left.SYSTEM_ID
		    LIMIT required_core;
		 --  )
     	 /* INSERT INTO connection_log_details (
       		 destination_systemid, destination_networkid, destination_port_no, destination_entitytype
		    )
		    SELECT 
		        destination_system_id, NULL, DESTINATION_PORT_NO, 'Cable'
		    FROM check_existing ce
		    where ce.destination_system_id = v_cable_details_left.system_id	 
		    ORDER BY source_port_no;*/
	  
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
				and ((v_cable_details_left.IS_CABLE_A_END=true)-- and a_end_status_id = 1)
				or(v_cable_details_left.IS_CABLE_A_END =false))-- and b_end_status_id = 1)) 
				and is_valid = true  and fiber_number IN 
				(SELECT destination_port_no FROM connection_log_details t1 
				order by t1.id  desc limit required_core) order by fiber_number  
				limit required_core 				
			),
			  fiber_pairs AS (
		     select t1.id as id, t1.cable_id,t1.fiber_number AS fiber1,t2.fiber_number AS fiber2,
		    LEAD(t1.cable_id) OVER (PARTITION BY t1.fiber_number ORDER BY t1.cable_id) 
		    AS next_cable,  t1.seq
			FROM core_planner_fiber_info t1
		    inner join core_planner_fiber_info t2 on t1.cable_id=t2.cable_id 
			and t1.fiber_number=t2.fiber_number-1 where t1.is_valid = true and t2.is_valid = true 
			order by t1.id,t1.fiber_number
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
			   fiber_availability AS (
					SELECT fiber1,fiber2, first_cable,COUNT(*) AS continuous_count
					FROM recursive_chain
					GROUP BY fiber1, fiber2, first_cable
					ORDER BY continuous_count DESC, fiber1 
				),
				selected_fibers_match AS (
			    -- Step 3: Select the fiber availability where fiber1 is odd
			    SELECT t2.*
			    FROM table1 t1  
			    JOIN core_planner_fiber_info t2  
			        ON t2.FIBER_number = t1.FIBER_number 
			    WHERE t2.cable_id  = v_cable_details_right.system_id
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
                   on f.fiber1 > d.fiber_number where f.fiber1 % 2 = 1 and
				    NOT EXISTS (
				        -- Check if an exact match is available
				        SELECT 1
				        FROM exact_match em
				        WHERE em.is_match = TRUE
				    ) 
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
		 
		      matched_fibers AS (
				SELECT t1.cable_id as c1_cable_id, t1.network_id as c1_network_id,
				t1.fiber_number as c1_fiber_number,t1.IS_CABLE_A_END as C1_IS_CABLE_A_END,
				t2.cable_id as c2_cable_id,v_cable_details_right.network_id as c2_network_id,
				t2.fiber_number as c2_fiber_number, v_cable_details_right.IS_CABLE_A_END  
				as C2_IS_CABLE_A_END ,(t1.fiber_number + t2.fiber_number) as remove_duplicate_seq,
				ROW_NUMBER() OVER (
	            PARTITION BY t1.fiber_number 
	            ORDER BY t2.fiber_number
	            ) AS fiber1_rank
			    FROM table1 t1
			    JOIN core_planner_fiber_info t2 ON  t2.cable_id = v_cable_details_right.system_id and 
			    ((v_cable_details_right.IS_CABLE_A_END=true) -- and a_end_status_id = 1)
				or(v_cable_details_right.IS_CABLE_A_END =false))-- and b_end_status_id = 1)) 		
			    LEFT JOIN exact_match em 
                    ON TRUE
                WHERE 
       		     CASE 
        		    WHEN em.is_match = TRUE THEN 
    	            t2.fiber_number = t1.fiber_number 
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
                order by t1.fiber_number --limit required_core
			    
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
	        WHERE SOURCE_ENTITY_TYPE = 'Cable' and SOURCE_SYSTEM_ID = v_cable_details_left.system_id and IS_CABLE_A_END = v_cable_details_left.IS_CABLE_A_END 
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
	        WHERE DESTINATION_ENTITY_TYPE = 'Cable' and DESTINATION_SYSTEM_ID = v_cable_details_left.system_id and IS_CABLE_B_END = v_cable_details_left.IS_CABLE_A_END
	     ) AS existing
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
		     AND e.is_cable_a_end = fr1.C1_IS_CABLE_A_END)t where t.destination_system_id = v_cable_details_right.system_id  
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
		        destination_system_id, NULL, DESTINATION_PORT_NO, 'Cable'
		    FROM existing_connections ce
		    where ce.source_system_id = v_cable_details_left.system_id	and source_port_no in (select destination_port_no from connection_log_details order by id desc limit required_core)
		    and ce.destination_system_id = v_cable_details_right.system_id    
		    ORDER BY source_port_no;
		end if;	  
	   
	end if;
        END LOOP;

		if (SELECT COUNT(*) FROM temp_connection) > 0
		then 
		 raise info 'end splicing 1 : %',p_user_id;
		  perform(fn_auto_provisioning_save_connections());
		
		
		end if;	


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

ALTER FUNCTION public.fn_get_core_planner_splicing_navika(int4, int4, varchar, varchar, varchar, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_core_planner_splicing_navika(int4, int4, varchar, varchar, varchar, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_core_planner_splicing_navika(int4, int4, varchar, varchar, varchar, int4) TO postgres;


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