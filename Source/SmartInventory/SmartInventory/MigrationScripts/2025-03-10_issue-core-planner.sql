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

    /* SELECT 
        (result_json->>'status')::BOOLEAN, 
        result_json->>'message'
     INTO v_status, v_message
     FROM fn_get_core_planner_validation(source_network_id, destination_network_id, buffer, 
     required_core, p_user_id) AS result_json;
   
     IF v_status = false  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT v_status AS status, v_message AS message) row;
       
     else*/
     
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
					inner join att_details_cable_info c 
					on c.cable_id=v_cable_details_left.system_id and c.fiber_number=port_number
					and ((v_cable_details_left.IS_CABLE_A_END=true and a_end_status_id = 1)
 					  or(v_cable_details_left.IS_CABLE_A_END =false and b_end_status_id = 1))
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
 					  or(v_cable_details_left.IS_CABLE_A_END =false and b_end_status_id = 1))
 				    inner join att_details_cable_info c2 
					on c2.cable_id=v_cable_details_right.system_id and c2.fiber_number=port_number
					and ((v_cable_details_right.IS_CABLE_A_END=true and c2.a_end_status_id = 1)
 					  or(v_cable_details_right.IS_CABLE_A_END =false and c2.b_end_status_id = 1))
 				    
					where is_valid_for_core_plan=true 
					and parent_network_id = p_odf_network_id
					and input_output='O' --and port_status_id=1					
					order by port_number limit required_core;							    		                 	
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

update isp_port_info set link_system_id=coalesce(p_link_system_id,0),is_valid_for_core_plan=true
from(
select c.* from core_planner_logs A
inner join isp_port_info_tbl p on a.user_id=p_user_id and 
((p.parent_system_id=A.b_system_id and p.parent_entity_type=A.b_entity_type)
 or (p.parent_system_id=A.a_system_id and p.parent_entity_type=A.a_entity_type))
inner join connection_info c on c.source_system_id=a.cable_id and c.source_entity_type='Cable' 
and C.source_port_no=p.port_number)b
where isp_port_info.port_number=b.destination_port_no and isp_port_info.parent_system_id=b.destination_system_id
and isp_port_info.parent_entity_type=b.destination_entity_type and isp_port_info.input_output='O';

update isp_port_info set link_system_id=coalesce(p_link_system_id,0),is_valid_for_core_plan=true
from(
select c.* from core_planner_logs A
inner join isp_port_info_tbl p on a.user_id=p_user_id and 
((p.parent_system_id=A.b_system_id and p.parent_entity_type=A.b_entity_type)
 or (p.parent_system_id=A.a_system_id and p.parent_entity_type=A.a_entity_type))
inner join connection_info c on c.destination_system_id=a.cable_id and c.destination_entity_type='Cable' 
and C.destination_port_no=p.port_number)b
where isp_port_info.port_number=b.source_port_no and isp_port_info.parent_system_id=b.source_system_id
and isp_port_info.parent_entity_type=b.source_entity_type and isp_port_info.input_output='O';


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

update att_details_cable_info set  fiber_status ='Reserved',
link_system_id=coalesce(p_link_system_id,0)
from temp_connected_cables b
where att_details_cable_info.cable_id=b.cable_id and b.fiber_number=att_details_cable_info.fiber_number;


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
       end if;
	   
	
	   
	   
	   
	   
       delete from core_planner_logs where user_id = p_user_id;

       return query select row_to_json(row) from ( select true as status, 'The required core has been spliced, and the fiber link has been successfully attached.' as message ) row;
      -- end if;
  END;
 
$function$
;
