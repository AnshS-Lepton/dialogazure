CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation_splicing(required_core integer, p_user_id integer, source_network_id character varying, destination_network_id character varying, buffer integer)
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

truncate table temp_connection restart identity;
truncate table connection_log_details restart identity;

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
COALESCE(t2.fiber_number, 0) AS fiber2, -- If no pair, set fiber2 to 0
LEAD(t1.cable_id) OVER (PARTITION BY t1.fiber_number ORDER BY t1.cable_id) AS next_cable,
CASE
WHEN t2.fiber_number IS NULL THEN 0 -- If pair is missing (no even-numbered fiber for odd)
WHEN t2.fiber_number != t1.fiber_number + 1 THEN 0 -- If next fiber is not a valid pair
ELSE t1.seq
END AS seq
FROM core_planner_fiber_info t1
LEFT JOIN core_planner_fiber_info t2
ON t1.cable_id = t2.cable_id
AND t2.fiber_number = t1.fiber_number + 1 -- Pair with the next fiber (even-numbered)
AND t2.user_id = p_user_id and t2.is_valid = true
WHERE t1.user_id = p_user_id and t1.is_valid = true
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
AND fs.seq = rc.seq+1 -- move "downward" in cable_id
),

-- to get existing connections
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
v_cable_details_left.SYSTEM_ID and IS_CABLE_B_END = v_cable_details_left.IS_CABLE_A_END
) AS existing
),
-- to get ODF source connections that are already existing, excluding those connections that will be used for further connections
source_existing_connection as(
select * from existing_connections ec where
ec.source_system_id = rec.a_system_id and ec.source_port_no not in (select port_number from table1 order by port_number limit required_core) and ec.DESTINATION_SYSTEM_ID = v_cable_details_left.system_id and p_odf_network_id = (select parent_network_id from table1 limit 1)
),

-- to get  the port_number of the ODF destination, checking for either existing connections or other connections that are not further connected
destination_connection as(
SELECT port_number
FROM (
SELECT b.port_number, 1 AS priority
FROM existing_connections ec
JOIN table1 b ON ec.source_system_id = b.parent_system_id
WHERE ec.DESTINATION_SYSTEM_ID = v_cable_details_LEFT.system_id
AND ec.DESTINATION_ENTITY_TYPE = 'Cable'
AND ec.DESTINATION_PORT_NO IN (
SELECT destination_port_no
FROM connection_log_details where destination_systemid = v_cable_details_LEFT.system_id
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
FROM connection_log_details where destination_systemid = v_cable_details_LEFT.system_id
ORDER BY id DESC
LIMIT required_core
)
AND ec.source_port_no = b2.port_number
) and port_number not in (select source_port_no from existing_connections ec where
ec.source_system_id = rec.a_system_id and ec.DESTINATION_port_no not in (
SELECT destination_port_no
FROM connection_log_details where destination_systemid = v_cable_details_LEFT.system_id
ORDER BY id DESC
LIMIT required_core ) and ec.DESTINATION_SYSTEM_ID = v_cable_details_LEFT.system_id ) ) AS combined
ORDER BY priority, port_number
LIMIT required_core
),

-- Check the fiber availability across all cables in the routes
fiber_availability AS (
SELECT fiber1,fiber2, first_cable, continuous_count from
(SELECT fiber1,fiber2, first_cable, COUNT(*) AS continuous_count
FROM recursive_chain
GROUP BY fiber1, fiber2, first_cable
ORDER BY continuous_count DESC, fiber1
) t1 where (t1.fiber1 in
(
select b.DESTINATION_port_no from table1 a
inner join source_existing_connection b on (a.port_number=b.source_port_no
and a.parent_network_id = p_odf_network_id )
)
or
t1.fiber1 not in
(
select b.DESTINATION_port_no from source_existing_connection b
)
)
AND
( ( required_core = 1 AND (t1.fiber2 IS NULL OR t1.fiber2 = 0) ) or
t1.fiber2 in
(
(select b.DESTINATION_port_no from table1 a
inner join source_existing_connection b on (a.port_number=b.source_port_no
and a.parent_network_id = p_odf_network_id ))
)
or
t1.fiber2 not in
(
select b.DESTINATION_port_no from source_existing_connection b
)
)
ORDER BY t1.continuous_count DESC, t1.fiber1
),

-- Check if the cores match at the same level.
match_avialibility AS (
select COALESCE( (select inf.continuous_count from (select * from table1
order by port_number limit 1) t1 join
fiber_availability inf on inf.fiber1 = t1.port_number or inf.fiber2 = t1.port_number+1
where t1.parent_network_id = p_odf_network_id and inf.fiber1 % 2 = 1),0) >=
COALESCE((SELECT MAX(continuous_count) FROM fiber_availability ), 0)
as higher_match_avialibility
),

-- If matching availability is found, then check if the number of available fibers at the same level is equal to or greater than the required core count
exact_match AS (
SELECT COUNT(*) = required_core AS is_match
FROM (
SELECT distinct *
FROM table1 t1
JOIN core_planner_fiber_info inf
ON inf.fiber_number = t1.port_number
WHERE inf.cable_id = v_cable_details_left.system_id and inf.user_id = p_user_id and inf.is_valid = true
and true = (select higher_match_avialibility from match_avialibility)
) AS selected_fibers_match
),

-- If no matching availability is found, then retrieve a fiber from the downward side.
downward_match AS (
-- Rank fiber pairs based on availability (highest first), then by fiber1 descending
SELECT fiber1, fiber2, continuous_count,
RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
FROM fiber_availability F
JOIN table1 d -- Properly referencing the threshold
on f.fiber1 > d.port_number where f.fiber1 % 2 = 1
),
-- If there is no availability on the downward side, then get a fiber from the upward side

upward_match AS (
-- Rank fiber pairs based on availability (highest first), then by fiber1 descending
SELECT fiber1, fiber2, continuous_count,
RANK() OVER (ORDER BY continuous_count DESC, fiber1 asc) AS rank
FROM fiber_availability F
JOIN table1 d -- Properly referencing the threshold
on f.fiber1 <= d.port_number where f.fiber1 % 2 = 1 and NOT EXISTS (
-- Check if an exact match is available
SELECT 1
FROM downward_match
) ),

-- to get cables that are already connected, in order to map their sequence for further connectivity
cable_connection_squence as(
(select source_port_no,DESTINATION_PORT_NO from existing_connections where source_port_no in (select port_number from table1) and destination_system_id = v_cable_details_left.system_id and (select parent_network_id from table1 limit 1)= p_odf_network_id )
union
(select source_port_no,DESTINATION_PORT_NO from existing_connections where destination_port_no in (select destination_port_no from connection_log_details where destination_systemid = v_cable_details_LEFT.system_id order by id desc limit required_core)
and destination_system_id = v_cable_details_left.system_id and (select parent_network_id from table1 limit 1) <> p_odf_network_id)
),

-- to preapare connectivity data between odf to cable
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
port_number in (select * from destination_connection)
ELSE TRUE
END
ORDER BY port_number
LIMIT required_core) t1
JOIN core_planner_fiber_info t2
ON t2.cable_id = v_cable_details_left.system_id
and t2.user_id = p_user_id and t2.is_valid = true
LEFT JOIN exact_match em ON TRUE
WHERE ( case
WHEN p_odf_network_id <> t1.parent_network_id THEN
t2.fiber_number IN (
SELECT destination_port_no
FROM connection_log_details where destination_systemid = v_cable_details_LEFT.system_id
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
-- prepare data using a cross join at the same level — for example, (1, 2) in ODF and (1, 2) in cable — but exists any cross-connections (e.g., if ODF 1 is connected to cable 2). The goal is to map cables in the correct order, resulting in data like (ODF: 1, 1, 2, 2 to Cable: 1, 2, 1, 2).

 matched_fibers_cross AS (
    SELECT t.parent_system_id, 
           t.parent_network_id, 
           t.port_number, 
           t.cable_id,
           t.network_id,
           t.fiber_number,
          t.IS_CABLE_A_END, 
           ROW_NUMBER() OVER (PARTITION BY t.port_number ORDER BY t.fiber_number) AS port_rank
    FROM (
        SELECT a.parent_system_id, 
               a.parent_network_id, 
               a.port_number, 
               a.cable_id,
               a.network_id,
               b.fiber_number,
               a.IS_CABLE_A_END
        FROM matched_fibers a
        CROSS JOIN (SELECT DISTINCT fiber_number FROM matched_fibers) b
    ) t
),
-- to get the final records with mapping cables (existing or not connection)
final_result AS (
(SELECT * from (SELECT *,
ROW_NUMBER() OVER (
PARTITION BY fiber_number
ORDER BY port_number
) AS fiber_rank
FROM matched_fibers )t WHERE case when (select count(1) from matched_fibers) <> required_core
then 
case when (select count(1) from cable_connection_squence) <> 1 
then
t.port_rank = t.fiber_rank
 WHEN (SELECT COUNT(1) FROM cable_connection_squence) = 1 
                 AND not exists(SELECT 1 FROM matched_fibers where fiber_number in (SELECT destination_port_no FROM cable_connection_squence))
THEN t.port_rank = t.fiber_rank
else
(t.port_number = (select source_port_no from cable_connection_squence) and t.fiber_number = (select destination_port_no from cable_connection_squence)) 
or
( t.port_number <> (select source_port_no from cable_connection_squence) and
t.fiber_number <> (select destination_port_no from cable_connection_squence))
end
else
 1=2 end)

 UNION 
    (SELECT * 
    FROM (
        SELECT *,
            ROW_NUMBER() OVER (
                PARTITION BY fiber_number
                ORDER BY port_number
            ) AS fiber_rank
        FROM matched_fibers_cross
    ) t 
    WHERE 
        CASE when required_core = (SELECT COUNT(1) FROM matched_fibers) then
          case WHEN (SELECT COUNT(1) FROM cable_connection_squence) <> 1 
            THEN t.port_rank = t.fiber_rank
            WHEN (SELECT COUNT(1) FROM cable_connection_squence) = 1 
                 AND not exists(SELECT 1 FROM matched_fibers where fiber_number in (SELECT destination_port_no FROM cable_connection_squence) )
            THEN t.port_rank = t.fiber_rank
            ELSE 
                (t.port_number = (SELECT source_port_no FROM cable_connection_squence) 
                 AND t.fiber_number = (SELECT destination_port_no FROM cable_connection_squence))
                 OR
                (t.port_number <> (SELECT source_port_no FROM cable_connection_squence) 
                 AND t.fiber_number <> (SELECT destination_port_no FROM cable_connection_squence))
        end else 1=2  end 
)
),
--  to get existing connections using final result the connection 
check_existing AS (
-- Check if ANY fiber in final_result is already in TEMP_ROUTE_CONNECTION
SELECT source_system_id,source_entity_type,source_port_no,is_cable_a_end,
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

--  to get those connections that are not already connected 
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
where ce.destination_system_id = v_cable_details_left.system_id ;
-- ORDER BY source_port_no;

else

SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
AS IS_CABLE_A_END
INTO v_cable_details_left FROM TEMP_LINE_MASTER A
WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)))
and a.system_id in
(select destination_systemid from connection_log_details order by id desc limit 1) LIMIT 1;

SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
AS IS_CABLE_A_END
INTO v_cable_details_right FROM TEMP_LINE_MASTER A
WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)))
AND SYSTEM_ID NOT IN(v_cable_details_left.SYSTEM_ID) LIMIT 1;

raise info 'v_cable_details_left 1 : %',v_cable_details_left;


if (v_cable_details_left is not null and v_cable_details_right is not null)
then
raise info 'v_cable_details_right 1 : %',v_cable_details_right;


WITH recursive table1 AS (
SELECT cable_id, fiber_number,v_cable_details_left.network_id as network_id,
v_cable_details_left.IS_CABLE_A_END as IS_CABLE_A_END
FROM core_planner_fiber_info where cable_id = v_cable_details_left.system_id
and is_valid = true and fiber_number IN
(SELECT destination_port_no FROM connection_log_details t1 where t1.destination_systemid = v_cable_details_LEFT.system_id
order by t1.id desc limit required_core) and core_planner_fiber_info.user_id =p_user_id order by fiber_number
limit required_core
) ,
fiber_pairs AS (
select * from(SELECT
t1.id AS id,
t1.cable_id,
t1.fiber_number AS fiber1,
COALESCE(t2.fiber_number, 0) AS fiber2, -- If no pair, set fiber2 to 0
LEAD(t1.cable_id) OVER (PARTITION BY t1.fiber_number ORDER BY t1.cable_id) AS next_cable,
CASE
WHEN t2.fiber_number IS NULL THEN 0 -- If pair is missing (no even-numbered fiber for odd)
WHEN t2.fiber_number != t1.fiber_number + 1 THEN 0 -- If next fiber is not a valid pair
ELSE t1.seq
END AS seq
FROM core_planner_fiber_info t1
LEFT JOIN core_planner_fiber_info t2
ON t1.cable_id = t2.cable_id
AND t2.fiber_number = t1.fiber_number + 1 -- Pair with the next fiber (even-numbered)
AND t2.user_id = p_user_id and t2.is_valid = true
WHERE t1.user_id = p_user_id and t1.is_valid = true
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
WHERE SOURCE_ENTITY_TYPE = 'Cable' and SOURCE_SYSTEM_ID = v_cable_details_left.system_id and IS_CABLE_A_END = v_cable_details_left.IS_CABLE_A_END
and destination_system_id= v_cable_details_right.system_id
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
and SOURCE_SYSTEM_ID= v_cable_details_right.system_id
) AS existing
),
right_existing_connection as(
select * from existing_connections ec where ec.SOURCE_SYSTEM_ID = v_cable_details_left.system_id and SOURCE_ENTITY_TYPE ='Cable' AND source_PORT_NO not in (SELECT destination_port_no FROM connection_log_details t1 where t1.destination_systemid = v_cable_details_LEFT.system_id
order by t1.id desc limit required_core) and ec.DESTINATION_SYSTEM_ID = v_cable_details_right.system_id
),
fiber_availability AS (
SELECT distinct fiber1,fiber2, first_cable, continuous_count from (
SELECT fiber1,fiber2, first_cable, COUNT(*) AS continuous_count
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
ON t2.FIBER_number = t1.FIBER_number and t2.cable_id = v_cable_details_right.system_id and t2.user_id = p_user_id and t2.is_valid = true
WHERE t2.FIBER_number in
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
JOIN table1 d -- Properly referencing the threshold
on f.fiber1 > d.fiber_number where f.fiber1 % 2 = 1
),
upward_match AS (
-- Rank fiber pairs based on availability (highest first), then by fiber1 descending
SELECT distinct fiber1, fiber2, continuous_count,
RANK() OVER (ORDER BY continuous_count DESC, fiber1 ASC) AS rank
FROM fiber_availability F
JOIN table1 d -- Properly referencing the threshold
on f.fiber1 <= d.fiber_number where f.fiber1 % 2 = 1 and NOT EXISTS (
SELECT 1
FROM downward_match
)) ,
cable_connection_squence as(
select source_port_no,DESTINATION_PORT_NO from existing_connections where SOURCE_PORT_NO in (select destination_port_no from connection_log_details where destination_systemid = v_cable_details_LEFT.system_id order by id desc limit required_core) and (MOD(source_port_no, 2) <> MOD(destination_port_no, 2))
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
JOIN core_planner_fiber_info t2 ON t2.cable_id = v_cable_details_right.system_id
and t2.user_id = p_user_id and t2.is_valid = true
LEFT JOIN exact_match em
ON TRUE
WHERE
(CASE
WHEN em.is_match = TRUE THEN
case when (select count(1) from cable_connection_squence) = 1 then
(t1.fiber_number = (select source_port_no from cable_connection_squence) and t2.fiber_number = (select destination_port_no from cable_connection_squence))
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
FROM matched_fibers)t WHERE t.fiber1_rank = t.fiber2_rank
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
where ce.source_system_id = v_cable_details_left.system_id and source_port_no in (select destination_port_no from connection_log_details order by id desc limit required_core)
and ce.destination_system_id = v_cable_details_right.system_id;
--ORDER BY source_port_no;
end if;

end if;
END LOOP;	

       return;
    
  END;
 
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_core_planner_validation_splicing(int4, int4, varchar, varchar, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_core_planner_validation_splicing(int4, int4, varchar, varchar, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_core_planner_validation_splicing(int4, int4, varchar, varchar, int4) TO postgres;

----------------------------------------------------------------------------------------------
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
v_rec RECORD;
SC_NETWORK_ID CHARACTER VARYING;
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
--TRUNCATE table connected_fiberlink_cable;
--TRUNCATE table connection_log_details;
--TRUNCATE table TMP_MISSING_FIBERS_INFO_CABLE;
--TRUNCATE table temp_connection;

-- CREATE TEMPORARY TABLES
CREATE TEMP TABLE TEMP_CABLE_ROUTES(
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
B_NETWORK_ID CHARACTER VARYING,
A_POINT_Geom geometry,
B_POINT_Geom geometry,
IS_MULTIPLE_ODF BOOLEAN DEFAULT TRUE
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
IS_CABLE_B_END BOOLEAN,
IS_VALID boolean default true,
connection_network_id VARCHAR NULL,
Connection_entity_type VARCHAR NULL
)ON COMMIT DROP;

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

create temp table connection_log_details
(
id serial,
destination_systemid integer,
destination_networkid character varying,source_no integer,
destination_port_no integer,
destination_entitytype character varying
) on commit drop;

create TEMP TABLE connected_fiberlink_cable (
id SERIAL PRIMARY KEY,
cable_id INTEGER NOT NULL,
fiber_number INTEGER NOT NULL,
is_valid boolean default true,
link_system_id INTEGER,
a_end_status_id INTEGER,
b_end_status_id INTEGER,
error_msg VARCHAR null,
fiber_status VARCHAR null
) ON COMMIT DROP;


CREATE temp TABLE TMP_MISSING_FIBERS_INFO_CABLE (
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
	id serial4 NOT NULL
) ON COMMIT DROP;

-- CLEAR PREVIOUS LOGS FOR THE USER
DELETE FROM CORE_PLANNER_LOGS WHERE USER_ID = P_USER_ID;
DELETE FROM CORE_PLANNER_FIBER_INFO WHERE USER_ID = P_USER_ID;

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
V_MESSAGE:='AT LEAST ONE ODF IS REQUIRED. PLEASE PROVIDE A VALID ODF.!' ;
END IF;

if(V_IS_VALID)
then
SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
INTO P_SOURCE, P_SOURCE_SYSTEM_ID
FROM att_details_fms
WHERE NETWORK_ID IN(SOURCE_NETWORK_ID);

SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
INTO P_DESTINATION, P_DESTINATION_SYSTEM_ID
FROM att_details_fms
WHERE NETWORK_ID IN(DESTINATION_NETWORK_ID);

IF COALESCE(P_SOURCE_SYSTEM_ID,0)=0
THEN
SC_NETWORK_ID = SOURCE_NETWORK_ID;
SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
INTO P_SOURCE
FROM att_details_spliceclosure
WHERE network_id IN(SOURCE_NETWORK_ID);
end if;

IF COALESCE(P_DESTINATION_SYSTEM_ID,0)=0
then
SC_NETWORK_ID = DESTINATION_NETWORK_ID;
SELECT LONGITUDE || ' ' || LATITUDE AS GEOM, SYSTEM_ID
INTO P_DESTINATION
FROM att_details_spliceclosure
WHERE network_id IN(DESTINATION_NETWORK_ID);
end if;


IF P_DESTINATION_SYSTEM_ID <> 0 and P_SOURCE_SYSTEM_ID <> 0 then

INSERT INTO TEMP_ISP_PORT_INFO (SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID,
PARENT_NETWORK_ID, PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE)
SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID,
PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE
FROM ISP_PORT_INFO A
WHERE A.PARENT_SYSTEM_ID = P_DESTINATION_SYSTEM_ID
AND A.PARENT_ENTITY_TYPE = 'FMS' AND INPUT_OUTPUT = 'O' and A.port_status_id in (1,2)
ORDER BY SYSTEM_ID;
end if;

WITH SORTED_DATA AS (
SELECT SYSTEM_ID, NETWORK_ID, PARENT_SYSTEM_ID, PARENT_NETWORK_ID,
PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE,
LAG(PORT_NUMBER) OVER (PARTITION BY PARENT_SYSTEM_ID ORDER BY PORT_NUMBER) AS PREV_FIBER
FROM ISP_PORT_INFO
WHERE PARENT_SYSTEM_ID = (CASE WHEN P_SOURCE_SYSTEM_ID <> 0 THEN P_SOURCE_SYSTEM_ID
ELSE P_DESTINATION_SYSTEM_ID END) AND
PORT_NUMBER % 2 = 1 AND INPUT_OUTPUT = 'O' and PARENT_ENTITY_TYPE = 'FMS'
and port_status_id in (1,2)
),
FILTERED_DATA AS (
SELECT A.SYSTEM_ID, A.NETWORK_ID, A.PARENT_SYSTEM_ID, A.PARENT_NETWORK_ID,
A.PORT_STATUS_ID, A.PORT_NUMBER,A.PARENT_ENTITY_TYPE
FROM ISP_PORT_INFO A
INNER JOIN SORTED_DATA B
ON A.PARENT_SYSTEM_ID = B.PARENT_SYSTEM_ID
AND (A.PORT_NUMBER = B.PORT_NUMBER + 1 OR A.PORT_NUMBER = B.PORT_NUMBER)
WHERE A.INPUT_OUTPUT = 'O' and A.PARENT_ENTITY_TYPE = 'FMS'
and A.port_status_id in (1,2)
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
PARENT_NETWORK_ID, PORT_STATUS_ID, PORT_NUMBER,PARENT_ENTITY_TYPE)

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
ST_ENDPOINT(LM.SP_GEOMETRY) AS ENDPOINT,PM.sp_geometry as point_geom
FROM TEMP_CABLE_ROUTES RS
INNER JOIN LINE_MASTER LM
ON LM.SYSTEM_ID = RS.EDGE_TARGETID AND UPPER(LM.ENTITY_TYPE) = 'CABLE'
INNER JOIN POINT_MASTER PM
ON ST_WITHIN(PM.SP_GEOMETRY, ST_BUFFER_METERS(ST_STARTPOINT(LM.SP_GEOMETRY), 3))
AND (UPPER(PM.ENTITY_TYPE) in ('BDB','FDB','SPLICECLOSURE')
OR (
UPPER(PM.ENTITY_TYPE) = 'FMS'
AND PM.COMMON_NAME IN (SOURCE_NETWORK_ID, DESTINATION_NETWORK_ID)
)) )

UPDATE TEMP_CABLE_ROUTES
SET
A_SYSTEM_ID = STARTCTE.SYSTEM_ID,
A_ENTITY_TYPE = STARTCTE.ENTITY_TYPE,
A_NETWORK_ID=STARTCTE.COMMON_NAME,
A_POINT_Geom = STARTCTE.point_geom
FROM STARTCTE
WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID=STARTCTE.CABLE_ID;

-----------UPDATE THE B END ID
WITH ENDCTE AS(
SELECT PM.SYSTEM_ID,PM.ENTITY_TYPE,PM.COMMON_NAME,EDGE_TARGETID as CABLE_ID, ST_ENDPOINT(LM.SP_GEOMETRY) AS ENDPOINT ,PM.sp_geometry as point_geom
FROM TEMP_CABLE_ROUTES RS
INNER JOIN LINE_MASTER LM
ON LM.SYSTEM_ID = RS.EDGE_TARGETID AND UPPER(LM.ENTITY_TYPE) = 'CABLE'
INNER JOIN POINT_MASTER PM
ON ST_WITHIN(PM.SP_GEOMETRY, ST_BUFFER_METERS(ST_ENDPOINT(LM.SP_GEOMETRY),3))
AND (UPPER(PM.ENTITY_TYPE) in ('BDB','FDB','SPLICECLOSURE')
OR (
UPPER(PM.ENTITY_TYPE) = 'FMS'
AND PM.COMMON_NAME IN (SOURCE_NETWORK_ID, DESTINATION_NETWORK_ID)
)) )

UPDATE TEMP_CABLE_ROUTES
SET
B_SYSTEM_ID = ENDCTE.SYSTEM_ID,
B_ENTITY_TYPE = ENDCTE.ENTITY_TYPE,
B_NETWORK_ID=ENDCTE.COMMON_NAME,
B_POINT_Geom = ENDCTE.point_geom
FROM ENDCTE
WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID=ENDCTE.CABLE_ID;


IF V_IS_VALID AND (SELECT COUNT(1)>0 FROM TEMP_CABLE_ROUTES WHERE COALESCE(B_ENTITY_TYPE,'')=''
OR COALESCE(A_ENTITY_TYPE,'')='')
THEN
UPDATE TEMP_CABLE_ROUTES SET IS_VALID=false,
MESSAGE='Multiple ODFs found along this route, or the Splice Closure is missing at the end of the cable.'
WHERE COALESCE(B_ENTITY_TYPE,'')='' OR COALESCE(A_ENTITY_TYPE,'')='';

V_IS_VALID:=FALSE;
V_MESSAGE:='Multiple ODFs found along this route, or the Splice Closure is missing at the end of the cable.';
END IF;

IF(V_IS_VALID = FALSE)
THEN

INSERT INTO CORE_PLANNER_LOGS (cable_network_id,CABLE_ID,CABLE_NAME,NETWORK_STATUS,
TOTAL_CORE,CABLE_LENGTH,ERROR_MSG,USER_ID,IS_VALID,a_system_id,b_system_id)
SELECT AC.NETWORK_ID,trc.edge_targetid ,AC.CABLE_NAME ,AC.NETWORK_STATUS ,
AC.TOTAL_CORE,AC.CABLE_CALCULATED_LENGTH,TRC.MESSAGE,P_USER_ID,TRC.is_valid,0,0 FROM TEMP_CABLE_ROUTES TRC
JOIN ATT_DETAILS_CABLE AC ON AC.SYSTEM_ID = TRC.EDGE_TARGETID AND TRC.USER_ID = P_USER_ID;

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
AND LINK_SYSTEM_ID = 0 and (a_end_status_id in(1,2) and b_end_status_id in(1,2) and fiber_status is null) 
ORDER BY FIBER_NUMBER
),
FILTERED_DATA AS (
SELECT A.CABLE_ID, A.FIBER_NUMBER, A.A_END_STATUS_ID, A.B_END_STATUS_ID,
A.IS_A_END_THROUGH_CONNECTIVITY, A.IS_B_END_THROUGH_CONNECTIVITY,
A.LINK_SYSTEM_ID
FROM ATT_DETAILS_CABLE_INFO A
INNER JOIN SORTED_DATA B
ON A.CABLE_ID = B.CABLE_ID
AND (A.FIBER_NUMBER = B.FIBER_NUMBER + 1 OR A.FIBER_NUMBER = B.FIBER_NUMBER)
WHERE  A.LINK_SYSTEM_ID = 0 and (A.a_end_status_id in(1,2) and A.b_end_status_id in(1,2) and A.fiber_status is null) 
),
FIBER_DATA AS (
SELECT CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID,
IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY,
LINK_SYSTEM_ID, -- Fixed missing comma
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
--WHERE
-- FIBER_NUMBER = PREV_FIBER + 1 OR FIBER_NUMBER = NEXT_FIBER - 1
ORDER BY
tcr.SEQ, FIBER_NUMBER;


INSERT INTO TMP_MISSING_FIBERS_INFO_CABLE
(CABLE_ID, FIBER_NUMBER, A_END_STATUS_ID, B_END_STATUS_ID,
IS_A_END_THROUGH_CONNECTIVITY, IS_B_END_THROUGH_CONNECTIVITY, USER_ID, LINK_SYSTEM_ID)

select distinct A.CABLE_ID, A.FIBER_NUMBER, A.A_END_STATUS_ID, A.B_END_STATUS_ID,
A.IS_A_END_THROUGH_CONNECTIVITY, A.IS_B_END_THROUGH_CONNECTIVITY, P_USER_ID, A.LINK_SYSTEM_ID FROM 
    ATT_DETAILS_CABLE_INFO A 
WHERE  -- Only include cables that exist in CORE_PLANNER_FIBER_INFO for this user
    A.CABLE_ID IN (
        SELECT DISTINCT CABLE_ID 
        FROM CORE_PLANNER_FIBER_INFO 
        WHERE USER_ID = P_USER_ID
    ) and
    NOT EXISTS (
        SELECT 1 
        FROM CORE_PLANNER_FIBER_INFO B
        WHERE 
            B.CABLE_ID = A.CABLE_ID AND 
            B.FIBER_NUMBER = A.FIBER_NUMBER AND
            B.USER_ID = P_USER_ID
    );

--GET THE CONNECTION OF CABLE FROM THIS ROUTE
INSERT INTO TEMP_ROUTE_CONNECTION(
SOURCE_SYSTEM_ID,SOURCE_NETWORK_ID,SOURCE_ENTITY_TYPE,SOURCE_PORT_NO,DESTINATION_SYSTEM_ID,
DESTINATION_NETWORK_ID,DESTINATION_ENTITY_TYPE,DESTINATION_PORT_NO,IS_CABLE_A_END,IS_CABLE_B_END,
Connection_network_id, Connection_entity_type)

select A.SOURCE_SYSTEM_ID,A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,
B.DESTINATION_SYSTEM_ID,B.DESTINATION_NETWORK_ID,B.DESTINATION_ENTITY_TYPE,
B.DESTINATION_PORT_NO,A.IS_CABLE_A_END,B.IS_CABLE_A_END,A.DESTINATION_NETWORK_ID,
A.DESTINATION_ENTITY_TYPE
FROM CONNECTION_INFO A
INNER JOIN CONNECTION_INFO B
ON A.DESTINATION_SYSTEM_ID=B.SOURCE_SYSTEM_ID AND A.DESTINATION_ENTITY_TYPE=B.SOURCE_ENTITY_TYPE
AND A.DESTINATION_PORT_NO=B.SOURCE_PORT_NO WHERE A.SOURCE_SYSTEM_ID
in (SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES)
AND A.SOURCE_ENTITY_TYPE='Cable' AND B.DESTINATION_ENTITY_TYPE='Cable'

UNION
SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
A.DESTINATION_PORT_NO,B.SOURCE_SYSTEM_ID, B.SOURCE_NETWORK_ID,B.SOURCE_ENTITY_TYPE,
B.SOURCE_PORT_NO,A.IS_CABLE_A_END,B.is_cable_a_end,A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE
FROM CONNECTION_INFO A
INNER JOIN CONNECTION_INFO B
ON B.DESTINATION_SYSTEM_ID=A.SOURCE_SYSTEM_ID AND B.DESTINATION_ENTITY_TYPE=A.SOURCE_ENTITY_TYPE
AND B.DESTINATION_PORT_NO=A.SOURCE_PORT_NO WHERE A.DESTINATION_SYSTEM_ID in
(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) AND A.DESTINATION_ENTITY_TYPE='Cable' AND B.SOURCE_ENTITY_TYPE='Cable'

UNION
SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
A.DESTINATION_PORT_NO,
A.SOURCE_SYSTEM_ID,A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,false,A.IS_CABLE_A_END,
null,null
FROM CONNECTION_INFO A
WHERE A.SOURCE_ENTITY_TYPE='FMS' and
A.SOURCE_NETWORK_ID IN(P_SOURCE_NETWORK_ID,P_DESTINATION_NETWORK_ID)
AND A.DESTINATION_ENTITY_TYPE='Cable'
UNION
SELECT A.DESTINATION_SYSTEM_ID,A.DESTINATION_NETWORK_ID,A.DESTINATION_ENTITY_TYPE,
A.DESTINATION_PORT_NO,
A.SOURCE_SYSTEM_ID, A.SOURCE_NETWORK_ID,A.SOURCE_ENTITY_TYPE,A.SOURCE_PORT_NO,A.IS_CABLE_A_END,
false,null,null
FROM CONNECTION_INFO A
WHERE A.DESTINATION_ENTITY_TYPE='FMS' AND
A.DESTINATION_NETWORK_ID IN(P_SOURCE_NETWORK_ID,P_DESTINATION_NETWORK_ID)
AND A.SOURCE_ENTITY_TYPE='Cable';

WITH combined AS (
SELECT
t1.id, t1.source_system_id, t1.source_network_id ,t1.source_entity_type,t1.source_port_no,
t1.destination_system_id, t1.destination_network_id, t1.destination_entity_type, t1.destination_port_no,t1.is_cable_a_end, t1.is_cable_b_end
FROM TEMP_ROUTE_CONNECTION t1
UNION ALL

SELECT
t2.id, t2.destination_system_id AS source_system_id, t2.destination_network_id AS source_network_id,
t2.destination_entity_type AS source_entity_type, t2.destination_port_no AS source_port_no,
t2.source_system_id AS destination_system_id,t2.source_network_id AS destination_network_id,
t2.source_entity_type AS destination_entity_type,t2.source_port_no AS destination_port_no,
t2.is_cable_b_end AS is_cable_a_end,t2.is_cable_a_end AS is_cable_b_end
FROM TEMP_ROUTE_CONNECTION t2
),
ranked AS (
SELECT *,
ROW_NUMBER() OVER (
PARTITION BY
combined.source_system_id,combined.source_network_id, combined.source_entity_type, combined.source_port_no,combined.destination_system_id,combined.destination_network_id,
combined.destination_entity_type,combined.destination_port_no,
combined.is_cable_a_end,combined.is_cable_b_end
ORDER by combined.id
) AS rn
FROM combined
),
removeDuplicateConnections AS (
SELECT id
FROM ranked
WHERE rn > 1
)
DELETE FROM TEMP_ROUTE_CONNECTION where id in (SELECT * FROM removeDuplicateConnections);

--VALIDAT THE CONNECTION IS EXIST IN SAME ROUTE OR NOT
UPDATE core_planner_fiber_info SET IS_VALID=false, error_msg = 'Other Route Connection'
FROM TEMP_ROUTE_CONNECTION A
WHERE core_planner_fiber_info.FIBER_NUMBER=A.SOURCE_PORT_NO AND A.SOURCE_ENTITY_TYPE='Cable'
AND core_planner_fiber_info.CABLE_ID = A.SOURCE_SYSTEM_ID
AND A.DESTINATION_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) and A.DESTINATION_ENTITY_TYPE='Cable' and core_planner_fiber_info.user_id = p_user_id and case when SC_NETWORK_ID is not null then A.connection_network_id not in (SC_NETWORK_ID) else 1=1 end;

UPDATE core_planner_fiber_info SET IS_VALID=false, error_msg = 'Other Route Connection'
FROM TEMP_ROUTE_CONNECTION A
WHERE core_planner_fiber_info.FIBER_NUMBER=A.DESTINATION_PORT_NO
AND A.DESTINATION_ENTITY_TYPE='Cable' AND core_planner_fiber_info.CABLE_ID = A.DESTINATION_SYSTEM_ID
AND A.SOURCE_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) and A.SOURCE_ENTITY_TYPE='Cable' and core_planner_fiber_info.user_id = p_user_id and case when SC_NETWORK_ID is not null then A.connection_network_id not in (SC_NETWORK_ID) else 1=1 end;

--IN TMP_MISSING_FIBERS_INFO_CABLE TABLE, VALIDAT THE CONNECTION IS EXIST IN SAME ROUTE OR NOT 
UPDATE TMP_MISSING_FIBERS_INFO_CABLE SET IS_VALID=false, error_msg = 'Other Route Connection'
FROM TEMP_ROUTE_CONNECTION A
where TMP_MISSING_FIBERS_INFO_CABLE.link_system_id = 0 and TMP_MISSING_FIBERS_INFO_CABLE.FIBER_NUMBER=A.SOURCE_PORT_NO AND A.SOURCE_ENTITY_TYPE='Cable'
AND TMP_MISSING_FIBERS_INFO_CABLE.CABLE_ID = A.SOURCE_SYSTEM_ID
AND A.DESTINATION_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) and A.DESTINATION_ENTITY_TYPE='Cable' and TMP_MISSING_FIBERS_INFO_CABLE.user_id = p_user_id and case when SC_NETWORK_ID is not null then A.connection_network_id not in (SC_NETWORK_ID) else 1=1 end;

UPDATE TMP_MISSING_FIBERS_INFO_CABLE SET IS_VALID=false, error_msg = 'Other Route Connection'
FROM TEMP_ROUTE_CONNECTION A
where TMP_MISSING_FIBERS_INFO_CABLE.link_system_id = 0 and TMP_MISSING_FIBERS_INFO_CABLE.FIBER_NUMBER=A.DESTINATION_PORT_NO
AND A.DESTINATION_ENTITY_TYPE='Cable' AND TMP_MISSING_FIBERS_INFO_CABLE.CABLE_ID = A.DESTINATION_SYSTEM_ID
AND A.SOURCE_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES) and A.SOURCE_ENTITY_TYPE='Cable' and TMP_MISSING_FIBERS_INFO_CABLE.user_id = p_user_id and case when SC_NETWORK_ID is not null then A.connection_network_id not in (SC_NETWORK_ID) else 1=1 end;

--update TEMP_ISP_PORT_INFO is_valid FALSE which already connected to others cable

UPDATE TEMP_ISP_PORT_INFO SET IS_VALID=FALSE
FROM TEMP_ROUTE_CONNECTION A
WHERE TEMP_ISP_PORT_INFO.PORT_NUMBER = A.SOURCE_PORT_NO AND A.SOURCE_ENTITY_TYPE='FMS'
AND A.DESTINATION_ENTITY_TYPE='Cable' AND
TEMP_ISP_PORT_INFO.parent_system_id = A.SOURCE_SYSTEM_ID
AND A.DESTINATION_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES);

UPDATE TEMP_ISP_PORT_INFO SET IS_VALID=FALSE
FROM TEMP_ROUTE_CONNECTION A
WHERE TEMP_ISP_PORT_INFO.PORT_NUMBER = A.DESTINATION_PORT_NO
AND A.DESTINATION_ENTITY_TYPE='FMS'
AND A.SOURCE_ENTITY_TYPE='Cable' AND TEMP_ISP_PORT_INFO.parent_system_id
= A.DESTINATION_SYSTEM_ID
AND A.SOURCE_SYSTEM_ID NOT IN(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES);

UPDATE TEMP_ISP_PORT_INFO tpi SET IS_VALID = FALSE
FROM (
SELECT adi.fiber_number, subqu.parent_system_id,subqu.DESTINATION_port_no
FROM (
SELECT distinct A.SOURCE_SYSTEM_ID,A.source_port_no ,
A.DESTINATION_SYSTEM_ID as parent_system_id,A.DESTINATION_port_no
FROM TEMP_ROUTE_CONNECTION A
JOIN TEMP_ISP_PORT_INFO
ON --TEMP_ISP_PORT_INFO.PORT_NUMBER = A.DESTINATION_PORT_NO
TEMP_ISP_PORT_INFO.parent_system_id = A.DESTINATION_SYSTEM_ID
AND A.DESTINATION_ENTITY_TYPE = 'FMS'
AND A.SOURCE_ENTITY_TYPE = 'Cable'
WHERE A.SOURCE_SYSTEM_ID IN (
SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES
)
) subqu
JOIN att_details_cable_info adi
ON subqu.SOURCE_SYSTEM_ID = adi.cable_id and subqu.source_port_no = adi.fiber_number
WHERE adi.link_system_id <> 0 
) outqu
WHERE tpi.parent_system_id = outqu.parent_system_id
AND tpi.PORT_NUMBER = outqu.DESTINATION_port_no;

UPDATE TEMP_ISP_PORT_INFO tpi SET IS_VALID = FALSE
FROM (
SELECT adi.fiber_number, subqu.parent_system_id,subqu.source_port_no
FROM (
SELECT distinct A.DESTINATION_SYSTEM_ID,A.DESTINATION_port_no,
A.SOURCE_SYSTEM_ID as parent_system_id,A.source_port_no
FROM TEMP_ROUTE_CONNECTION A
JOIN TEMP_ISP_PORT_INFO
ON TEMP_ISP_PORT_INFO.parent_system_id = A.SOURCE_SYSTEM_ID
AND A.SOURCE_ENTITY_TYPE = 'FMS'
AND A.DESTINATION_ENTITY_TYPE = 'Cable'
WHERE A.DESTINATION_SYSTEM_ID IN (
SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES
)
) subqu
JOIN att_details_cable_info adi
ON subqu.DESTINATION_SYSTEM_ID = adi.cable_id and subqu.DESTINATION_port_no = adi.fiber_number
WHERE adi.link_system_id <> 0 
) outqu
WHERE tpi.parent_system_id = outqu.parent_system_id
AND tpi.PORT_NUMBER = outqu.source_port_no;

UPDATE TEMP_ISP_PORT_INFO tpi SET IS_VALID = false
from (SELECT t1.*
FROM (
SELECT DISTINCT
A.DESTINATION_SYSTEM_ID,
A.DESTINATION_port_no,
A.SOURCE_SYSTEM_ID AS parent_system_id,
A.source_port_no
FROM TEMP_ROUTE_CONNECTION A
JOIN TEMP_ISP_PORT_INFO
ON TEMP_ISP_PORT_INFO.parent_system_id = A.SOURCE_SYSTEM_ID
WHERE A.SOURCE_ENTITY_TYPE = 'FMS'
AND A.DESTINATION_ENTITY_TYPE = 'Cable'
AND A.DESTINATION_SYSTEM_ID IN (
SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES
)
) t1
LEFT JOIN CORE_PLANNER_FIBER_INFO inf
ON inf.fiber_number = t1.DESTINATION_port_no
AND inf.cable_id = t1.DESTINATION_SYSTEM_ID
WHERE inf.fiber_number IS null) tp where tp.parent_system_id = tpi.parent_system_id and tp.source_port_no = tpi.port_number;

UPDATE TEMP_ISP_PORT_INFO tpi SET IS_VALID = false
from (SELECT t1.*
FROM (
SELECT distinct A.SOURCE_SYSTEM_ID,A.source_port_no ,
A.DESTINATION_SYSTEM_ID as parent_system_id,A.DESTINATION_port_no
FROM TEMP_ROUTE_CONNECTION A
JOIN TEMP_ISP_PORT_INFO
ON --TEMP_ISP_PORT_INFO.PORT_NUMBER = A.DESTINATION_PORT_NO
TEMP_ISP_PORT_INFO.parent_system_id = A.DESTINATION_SYSTEM_ID
AND A.DESTINATION_ENTITY_TYPE = 'FMS'
AND A.SOURCE_ENTITY_TYPE = 'Cable'
WHERE A.SOURCE_SYSTEM_ID IN (
SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES
)
) t1
LEFT JOIN CORE_PLANNER_FIBER_INFO inf
ON inf.fiber_number = t1.source_port_no
AND inf.cable_id = t1.SOURCE_SYSTEM_ID
WHERE inf.fiber_number IS null) tp where tp.parent_system_id = tpi.parent_system_id and tp.DESTINATION_port_no = tpi.port_number;

UPDATE TEMP_ISP_PORT_INFO AS even
SET is_valid = false
FROM TEMP_ISP_PORT_INFO AS odd
WHERE
odd.port_number % 2 = 1
AND odd.is_valid = false
AND even.port_number = odd.port_number + 1
AND even.parent_system_id = odd.parent_system_id
AND even.is_valid = true AND (
  SC_NETWORK_ID IS NOT NULL
  OR even.parent_network_id = source_network_id
);

UPDATE TEMP_ISP_PORT_INFO AS odd
SET is_valid = false
FROM TEMP_ISP_PORT_INFO AS even
WHERE
even.port_number % 2 = 0
AND even.is_valid = false
AND odd.port_number = even.port_number - 1
AND odd.parent_system_id = even.parent_system_id
AND odd.is_valid = true AND (
  SC_NETWORK_ID IS NOT NULL
  OR even.parent_network_id = source_network_id
);

INSERT INTO connected_fiberlink_cable (cable_id, fiber_number,link_system_id,
a_end_status_id,b_end_status_id ,fiber_status)
select cable_id ,fiber_number,link_system_id,a_end_status_id,b_end_status_id,fiber_status
from att_details_cable_info where cable_id in
(SELECT EDGE_TARGETID FROM TEMP_CABLE_ROUTES);

update connected_fiberlink_cable set is_valid = false where link_system_id > 0
and ( a_end_status_id = 2 or b_end_status_id =2 ) and fiber_status is not null ;

for v_rec in (select edge_targetid from TEMP_CABLE_ROUTES order by seq)
loop

WITH invalid_sources AS (
SELECT ci.destination_system_id,
ci.destination_port_no,
ci.destination_entity_type
FROM temp_route_connection ci
JOIN connected_fiberlink_cable src
ON src.cable_id = ci.source_system_id
AND src.fiber_number = ci.source_port_no
AND src.is_valid = false
WHERE ci.source_entity_type = 'Cable' and ci.source_system_id = v_rec.edge_targetid
)
UPDATE connected_fiberlink_cable dst
SET is_valid = false, error_msg ='Occupied Connection'
FROM invalid_sources t
WHERE dst.cable_id = t.destination_system_id
AND dst.fiber_number = t.destination_port_no
AND t.destination_entity_type = 'Cable';

WITH invalid_destinations AS (
SELECT ci.source_system_id,
ci.source_port_no,
ci.source_entity_type
FROM temp_route_connection ci
JOIN connected_fiberlink_cable src
ON src.cable_id = ci.destination_system_id
AND src.fiber_number = ci.destination_port_no
AND src.is_valid = false
WHERE ci.destination_entity_type = 'Cable' and ci.destination_system_id = v_rec.edge_targetid
)
UPDATE connected_fiberlink_cable dst
SET is_valid = false,error_msg ='Occupied Connection'
FROM invalid_destinations t
WHERE dst.cable_id = t.source_system_id
AND dst.fiber_number = t.source_port_no
AND t.source_entity_type = 'Cable';

end loop;

update CORE_PLANNER_FIBER_INFO t1 set is_valid = false ,error_msg = t2.error_msg
from connected_fiberlink_cable t2 where t1.cable_id = t2.cable_id and
t1.fiber_number = t2.fiber_number and t2.is_valid = false
and t1.user_id =p_user_id;


SELECT (SELECT COUNT(*)FROM (
SELECT parent_system_id FROM TEMP_ISP_PORT_INFO
WHERE IS_VALID = true GROUP BY parent_system_id HAVING COUNT(*) >= REQUIRED_CORE
) AS t1) <> (SELECT COUNT(distinct PARENT_SYSTEM_ID) FROM ISP_PORT_INFO
WHERE PARENT_SYSTEM_ID IN (P_SOURCE_SYSTEM_ID, P_DESTINATION_SYSTEM_ID)) into Is_AvailableODF;

IF V_IS_VALID and Is_AvailableODF
THEN
V_IS_VALID:=FALSE;
V_MESSAGE:='All ports of the ODF are either connected to different routes, marked as reserved or faulty, or no adjacent ports are available in the source or destination ODF.';
END IF;

if (REQUIRED_CORE = 2) then
WITH CABLECTE AS (
SELECT
tcr.edge_targetid AS CABLE_ID,
COALESCE(2 * COUNT(next_fiber.fiber_number), 0) AS AVAILABLE_CORE
FROM TEMP_CABLE_ROUTES tcr
LEFT JOIN CORE_PLANNER_FIBER_INFO adi
ON tcr.edge_targetid = adi.cable_id
AND adi.USER_ID = P_USER_ID
AND adi.link_system_id = 0
AND adi.fiber_number % 2 = 1
LEFT JOIN CORE_PLANNER_FIBER_INFO next_fiber
ON next_fiber.cable_id = adi.cable_id
AND next_fiber.fiber_number = adi.fiber_number + 1
AND next_fiber.link_system_id = 0
GROUP BY tcr.edge_targetid
)
UPDATE TEMP_CABLE_ROUTES
SET IS_VALID = FALSE,
MESSAGE =  'Unavailable core: either reserved, faulty, or connected to other routes'
FROM CABLECTE
WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID = CABLECTE.CABLE_ID
AND CABLECTE.AVAILABLE_CORE < REQUIRED_CORE;
end if;

if (REQUIRED_CORE = 1) then
WITH CABLECTE AS (
SELECT
tcr.edge_targetid AS CABLE_ID,
SUM(CASE WHEN adi.link_system_id = 0 THEN 1 ELSE 0 END) AS AVAILABLE_CORE
FROM CORE_PLANNER_FIBER_INFO adi
JOIN TEMP_CABLE_ROUTES tcr ON tcr.edge_targetid = adi.cable_id
WHERE adi.USER_ID = P_USER_ID
GROUP BY tcr.edge_targetid
)
UPDATE TEMP_CABLE_ROUTES
SET IS_VALID = FALSE,
MESSAGE = 'Unavailable core: either reserved, faulty, or connected to other routes'
FROM CABLECTE
WHERE TEMP_CABLE_ROUTES.EDGE_TARGETID = CABLECTE.CABLE_ID
AND CABLECTE.AVAILABLE_CORE < REQUIRED_CORE;
end if;


IF V_IS_VALID AND EXISTS(SELECT 1 FROM TEMP_CABLE_ROUTES WHERE IS_VALID = FALSE AND
MESSAGE = 'Unavailable core: either reserved, faulty, or connected to other routes')
THEN
V_IS_VALID:=FALSE;
V_MESSAGE:='Required/Adjacent Cores are unavailable or currently connected to other routes/reserved!';
END IF;

UPDATE TEMP_CABLE_ROUTES tcr
SET is_valid = false,
MESSAGE = 'Required/Adjacent Cores are unavailable or currently connected to other routes/reserved!'
FROM att_details_cable_info adi
LEFT JOIN CORE_PLANNER_FIBER_INFO cpfi
ON cpfi.cable_id = adi.cable_id AND cpfi.is_valid = true and cpfi.user_id = p_user_id
WHERE tcr.edge_targetid = adi.cable_id
AND cpfi.cable_id IS NULL ;

if V_IS_VALID AND EXISTS(SELECT 1 FROM TEMP_CABLE_ROUTES WHERE IS_VALID = false)
then
V_IS_VALID := FALSE;
V_MESSAGE := 'Required/Adjacent Cores are unavailable or currently connected to other routes/reserved!';
end if;

IF V_IS_VALID AND EXISTS(SELECT cable_id FROM CORE_PLANNER_FIBER_INFO
WHERE user_id = p_user_id
GROUP BY cable_id
HAVING SUM(CASE
WHEN is_valid = false AND error_msg = 'Occupied Connection' THEN 1
ELSE 0
END) = COUNT(*))
then

V_IS_VALID:=FALSE;
V_MESSAGE:='Unable to Find Connectivity Cores in the Specified Cables!';
END IF;


INSERT INTO
CORE_PLANNER_LOGS(CABLE_ID, CABLE_NAME, NETWORK_STATUS, TOTAL_CORE, CABLE_LENGTH,ERROR_MSG, IS_VALID, USER_ID,CABLE_NETWORK_ID, AVAIABLE, USED_CORE,B_SYSTEM_ID,A_SYSTEM_ID,a_entity_type ,b_entity_type,SEQ)

SELECT ATT.SYSTEM_ID, ATT.CABLE_NAME, ATT.NETWORK_STATUS, ATT.TOTAL_CORE, ATT.CABLE_CALCULATED_LENGTH,INFO.MESSAGE, INFO.IS_VALID,
P_USER_ID,ATT.NETWORK_ID,
(SELECT COUNT(*) FROM ATT_DETAILS_CABLE_INFO ACI WHERE ACI.CABLE_ID = ATT.SYSTEM_ID and ACI.LINK_SYSTEM_ID=0 ) AS AVAILABLE_CORES,
(SELECT COUNT(*) FROM ATT_DETAILS_CABLE_INFO ACI WHERE ACI.CABLE_ID = INFO.EDGE_TARGETID AND ACI.LINK_SYSTEM_ID > 0) AS USED_CORE,
INFO.B_SYSTEM_ID,INFO.A_SYSTEM_ID,
INFO.a_entity_type ,INFO.b_entity_type ,INFO.seq
FROM ATT_DETAILS_CABLE ATT
INNER JOIN TEMP_CABLE_ROUTES INFO ON ATT.SYSTEM_ID = INFO.EDGE_TARGETID AND INFO.USER_ID = P_USER_ID order by INFO.seq ;

UPDATE TMP_MISSING_FIBERS_INFO_CABLE SET IS_VALID=false, error_msg = 'Other Route Connection'
where (a_end_status_id not in (1,2) and a_end_status_id not in (1,2)) and user_id = P_USER_ID;

UPDATE CORE_PLANNER_LOGS cpl
SET used_core = cpl.used_core + t.used_core_count,
avaiable = avaiable - t.used_core_count,
error_msg =case WHEN cpl.error_msg IS NULL 
     THEN 'Connected to other routes/reserved' else error_msg end
FROM (
SELECT
cable_id,
COUNT(*) AS used_core_count
FROM CORE_PLANNER_FIBER_INFO
WHERE
user_id = P_USER_ID
AND error_msg = 'Other Route Connection' and is_valid = false
AND (a_end_status_id = 2 OR b_end_status_id = 2)
GROUP BY cable_id
) t
WHERE cpl.cable_id = t.cable_id
AND cpl.user_id = P_USER_ID;

-- update missing used_core and avaiable core 
UPDATE CORE_PLANNER_LOGS cpl
SET used_core = cpl.used_core + t.used_core_count,
avaiable = avaiable - t.used_core_count,
error_msg =case WHEN cpl.error_msg IS NULL 
     THEN 'Connected to other routes/reserved' else error_msg end
FROM (
SELECT
cable_id,
COUNT(*) AS used_core_count
FROM TMP_MISSING_FIBERS_INFO_CABLE
WHERE
user_id = P_USER_ID
AND error_msg = 'Other Route Connection' and is_valid = false
AND link_system_id = 0
GROUP BY cable_id
) t
WHERE cpl.cable_id = t.cable_id
AND cpl.user_id = P_USER_ID;

END IF;
END IF;

	if(V_IS_VALID) 
	then 
	perform fn_get_core_planner_validation_splicing(required_core, p_user_id, source_network_id, 	destination_network_id, buffer);

    update CORE_PLANNER_LOGS set 
    error_msg = 'Fibers are not available or already occupied with diffirent route/network!', is_valid = false 
	where cable_id in (select edge_targetid  from temp_cable_routes tcr where edge_targetid not in (select 	destination_systemid from connection_log_details GROUP BY destination_systemid
    HAVING COUNT(*) >= required_core)) and user_id = P_USER_ID;
	if exists (select 1 from CORE_PLANNER_LOGS where is_valid= false and error_msg = 'Fibers are not available or already occupied with diffirent route/network!' and user_id= P_USER_ID)
	then 
	V_IS_VALID:=FALSE;
	V_MESSAGE:='Fibers are not available or already occupied with diffirent route/network!';
	end if;

end if;

RETURN QUERY SELECT row_to_json(row)
FROM (SELECT V_IS_VALID AS status, V_MESSAGE AS message) row;
END;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) TO public;
GRANT ALL ON FUNCTION public.fn_get_core_planner_validation(varchar, varchar, int4, int4, int4) TO postgres;



---------------------------------------------------------------------------------------------------------------

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
v_message character varying;
V_IS_CABLE_A_END BOOLEAN;
p_destination_network_id character varying;
p_odf_network_id character varying;
v_source_ports character varying;
v_destination_ports character varying;

BEGIN
V_IS_CABLE_A_END:=FALSE;

p_destination_network_id = destination_network_id;
--truncate table temp_connection restart identity;
--truncate table connection_log_details;
-- Create a temporary table for connection data

create temp table temp_connected_cables
(
id serial,
cable_id integer,
fiber_number integer
) on commit drop;

SELECT
(result_json->>'status')::BOOLEAN,
result_json->>'message'
INTO v_status, v_message
FROM fn_get_core_planner_validation(source_network_id, destination_network_id, buffer,
required_core, p_user_id) AS result_json;

IF v_status = false THEN
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
--- get fiberlink into p_link_system_id
select system_id into p_link_system_id from att_details_fiber_link adfl
where upper(network_id) = upper(fiber_link_network_id)
or upper(link_id)=upper(fiber_link_network_id);
 
-- perform fn_get_core_planner_validation_splicing(required_core, p_user_id, source_network_id, 	destination_network_id, buffer);

if (SELECT COUNT(*) FROM temp_connection) > 0
then
raise info 'end splicing 1 : %',p_user_id;
perform(fn_auto_provisioning_save_connections());


end if;

update att_details_cable_info set a_end_status_id=2
from temp_connection tc
where cable_id=tc.source_system_id and tc.source_entity_type='Cable'
and fiber_number=tc.source_port_no
and tc.is_source_cable_a_end =true;

update att_details_cable_info set b_end_status_id=2
from temp_connection tc
where cable_id=tc.source_system_id and tc.source_entity_type='Cable'
and fiber_number=tc.source_port_no
and tc.is_source_cable_a_end=false;

update att_details_cable_info set a_end_status_id=2
from temp_connection tc
where cable_id=tc.destination_system_id and tc.destination_entity_type='Cable'
and fiber_number=tc.destination_port_no
and tc.is_destination_cable_a_end=true;

update att_details_cable_info set b_end_status_id=2
from temp_connection tc
where cable_id=tc.destination_system_id and tc.destination_entity_type='Cable'
and fiber_number=tc.destination_port_no
and tc.is_destination_cable_a_end=false;

update isp_port_info set port_status_id=2
from temp_connection tc
where parent_system_id=tc.destination_system_id
and upper(parent_entity_type)=upper(tc.destination_entity_type)
and port_number = tc.destination_port_no ;

update isp_port_info set port_status_id=2
from temp_connection tc
where parent_system_id=tc.source_system_id
and upper(parent_entity_type)=upper(tc.source_entity_type)
and port_number = tc.source_port_no ;

insert into temp_connected_cables(cable_id,fiber_number)
select destination_systemid, destination_port_no from connection_log_details;

update att_details_cable_info set fiber_status ='Reserved',
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
