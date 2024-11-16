-- public.core_planner_logs definition

-- Drop table

-- DROP TABLE public.core_planner_logs;

CREATE TABLE public.core_planner_logs (
	cable_id int4 NULL,
	cable_name varchar NULL,
	network_status varchar NULL,
	total_core int4 NULL,
	used_core int4 NULL,
	avaiable int4 NULL,
	cable_length varchar NULL,
	a_system_id int4 NULL,
	b_system_id int4 NULL,
	a_entity_type varchar NULL,
	b_entity_type varchar NULL,
	error_msg varchar NULL,
	user_id int4 NULL,
	is_valid bool NULL,
	id serial4 NOT NULL,
	a_network_id varchar NULL,
	b_network_id varchar NULL,
	cable_network_id varchar NULL
);

----------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_splicing(required_core integer, p_user_id integer, fiber_link_network_id character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
    rec RECORD;
    v_network_id character varying;
    v_system_id integer;
    v_cable_details_left record;
    v_cable_details_right record;
    v_cable_details record;
   p_link_system_id integer;
BEGIN
    v_network_id := '';
    v_system_id := 0;
    p_link_system_id =0;
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
       
   select system_id into p_link_system_id from att_details_fiber_link adfl where network_id = fiber_link_network_id;
        -- Loop through records to populate temp_connection for valid routes
        FOR rec IN select * from (      
         SELECT a_system_id,a_network_id,a_entity_type FROM core_planner_logs  where user_id = p_user_id and is_valid =TRUE
        union
        SELECT b_system_id,b_network_id,b_entity_type FROM core_planner_logs where user_id = p_user_id and is_valid =TRUE )a
        LOOP
    raise info 'rec 1 : %',rec;
		if(rec.a_entity_type='FMS')
		then

		select b.* into v_cable_details_left 
		from core_planner_logs a
		inner join att_details_cable b on a.cable_id=b.system_id where (b.a_system_id=rec.a_system_id or b.b_system_id=rec.a_system_id ) and (a.a_entity_type=rec.a_entity_type or b.b_entity_type = rec.a_entity_type);
   raise info 'v_cable_details_left 1 : %',v_cable_details_left;
  
		INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source
                )
		    SELECT 
                   rec.a_system_id, rec.a_network_id, rec.a_entity_type, 
                    a.port_number, 
                    v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    a.port_number,false, (case when v_cable_details_left.a_entity_type=rec.a_entity_type then true else false end), 
                    p_user_id, 'CorePlanning'
		    FROM  isp_port_info a where a.parent_system_id=rec.a_system_id and a.parent_entity_type=rec.a_entity_type and a.input_output='O' and port_status_id=1 limit required_core;
       
		   

		else


		select b.* into v_cable_details_left 
		from core_planner_logs a
		inner join att_details_cable b on a.cable_id=b.system_id where (a.a_system_id=rec.a_system_id or a.b_system_id =rec.a_system_id )  and (a.a_entity_type=rec.a_entity_type or a.b_entity_type=rec.a_entity_type) and a.user_id = p_user_id and a.is_valid =true order by a.id asc limit 1;
   raise info 'v_cable_details_left 2 : %',v_cable_details_left;

		select b.* into v_cable_details_right 
		from core_planner_logs a
		inner join att_details_cable b on a.cable_id=b.system_id where (a.b_system_id=rec.a_system_id or a.a_system_id = rec.a_system_id) and (a.a_entity_type=rec.a_entity_type or a.b_entity_type = rec.a_entity_type) and a.user_id = p_user_id and a.is_valid =true order by a.id desc limit 1;
   raise info 'v_cable_details_right 3 : %',v_cable_details_left;

		INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source,equipment_system_id, equipment_network_id, equipment_entity_type 
                )
		    SELECT 
                    v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    a.fiber_number, 
                    v_cable_details_right.system_id, v_cable_details_right.network_id, 'Cable', 
                    a.fiber_number,
                    (case when v_cable_details_left.a_entity_type=logs.a_entity_type then true else false end),
                    (case when v_cable_details_right.a_entity_type=logs.a_entity_type then true else false end), 
                    p_user_id, 'CorePlanning',rec.a_system_id,rec.a_network_id,rec.a_entity_type
		    FROM  att_details_cable_info a join core_planner_logs logs on logs.cable_id = a.cable_id  where a.cable_id=v_cable_details_left.system_id and a.a_end_status_id=1 and a.b_end_status_id=1 limit required_core;
               

		
		end if;

            
        END LOOP;

           raise info 'end loop 1 : %',p_user_id;

if (SELECT COUNT(*) FROM temp_connection) > 1
then 
 raise info 'end splicing 1 : %',p_user_id;
perform(fn_auto_provisioning_save_connections());

FOR i in 1..required_core
loop 
	 raise info 'end splicing 2 1 : %',p_user_id;

perform fn_associate_fiber_link_cable((select cable_id from core_planner_logs where user_id = p_user_id limit 1),p_link_system_id,i,'A' );
	
end loop;
 raise info 'end loop fiberlink 2 : %',p_user_id;

end if;	


return query select row_to_json(row) from ( select true as status, 'Success' as message ) row;



END;
$function$
;



----------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation(source_network_id character varying, destination_network_id character varying, buffer integer, required_core integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
    rec RECORD;
    total_core integer;
   p_source character varying;
  p_destination character varying;
 avaiableCore  integer;
 usedCore  integer;
source_system_id integer;
begin
	avaiableCore = 0;
    usedCore = 0;
    -- Create temporary table for cable routes
    CREATE TEMP TABLE temp_cable_routes(
        seq integer,
        path_seq integer, 
        edge_targetid integer, 
        roadline_geomtext text, 
        start_point character varying,
        end_point character varying
    ) ON COMMIT DROP;

   select longitude ||' '||latitude as geom,system_id  into p_source,source_system_id from att_details_fms where network_id = source_network_id;
   select longitude ||' '||latitude as geom into p_destination from att_details_fms where network_id = destination_network_id;

    -- Insert shortest route cables into the temporary table
    INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, roadline_geomtext, start_point, end_point)
    SELECT * FROM fn_sf_get_routes(p_source, p_destination, buffer, buffer);

   delete from core_planner_logs where user_id = p_user_id;
    -- Loop through each record in the temporary table
    FOR rec IN (SELECT * FROM temp_cable_routes order by seq asc)
    loop
	    raise info 'rec 1 : %',rec;
        -- Check if the required core count is available for a cable
        SELECT COUNT(*) INTO total_core
        FROM att_details_cable_info adci 
        WHERE adci.cable_id = rec.edge_targetid 
          AND a_end_status_id = 1 
          AND b_end_status_id = 1;
 raise info 'total_core 1 : %',total_core;

   SELECT COUNT(*) INTO avaiableCore
  from isp_port_info a where a.parent_system_id=source_system_id and a.parent_entity_type='FMS' and port_status_id=1 ; 
 raise info 'avaiableCore 1 : %',avaiableCore;

SELECT COUNT(*) INTO usedCore
from isp_port_info a where a.parent_system_id=source_system_id and a.parent_entity_type='FMS' and port_status_id >1 ; 
     raise info 'usedCore 1 : %',usedCore;

        IF total_core = required_core THEN
            -- Insert record with available ports
            INSERT INTO core_planner_logs(
                cable_id, cable_name, network_status, total_core, cable_length, 
                a_system_id, b_system_id, a_entity_type, b_entity_type, 
                error_msg, is_valid, user_id, a_network_id, b_network_id,cable_network_id,avaiable,used_core  
            )     
            SELECT att.system_id, att.cable_name, att.network_status, att.total_core, att.cable_calculated_length, 
                   att.a_system_id, att.b_system_id, att.a_entity_type, att.b_entity_type, 
                   'Available Port', TRUE, p_user_id, att.a_network_id, att.b_network_id,att.network_id ,avaiableCore,usedCore
            FROM att_details_cable att 
            WHERE att.system_id = rec.edge_targetid;
        ELSE
            -- Insert record with error message if required core is not available
            INSERT INTO core_planner_logs(
                cable_id, cable_name, network_status, total_core, cable_length, 
                error_msg, is_valid, user_id, a_system_id, b_system_id, a_entity_type, b_entity_type, a_network_id, b_network_id,cable_network_id,avaiable,used_core
            )     
            SELECT att.system_id, att.cable_name, att.network_status, att.total_core, att.cable_calculated_length, 
                   'Required core is not available', FALSE, p_user_id, att.a_system_id, att.b_system_id, 
                   att.a_entity_type, att.b_entity_type, att.a_network_id, att.b_network_id,att.network_id,avaiableCore,usedCore 
            FROM att_details_cable att 
            WHERE att.system_id = rec.edge_targetid;
        END IF;
    END LOOP;

    -- Validate if any record has 'FMS' in either `a_entity_type` or `b_entity_type`
    IF NOT EXISTS (
        SELECT 1
        FROM core_planner_logs logs  
        WHERE (logs.a_entity_type = 'FMS' OR logs.b_entity_type = 'FMS') 
          AND logs.user_id = p_user_id
        ORDER BY logs.id ASC 
        LIMIT 1
    ) THEN 
        UPDATE core_planner_logs 
        SET is_valid = FALSE 
        WHERE user_id = p_user_id;
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM core_planner_logs logs  
        WHERE (logs.a_entity_type = 'FMS' OR logs.b_entity_type = 'FMS') 
          AND logs.user_id = p_user_id
        ORDER BY logs.id DESC 
        LIMIT 1
    ) THEN 
        UPDATE core_planner_logs 
        SET is_valid = FALSE 
        WHERE user_id = p_user_id;
    END IF;

    -- Validate if `a_system_id` or `b_system_id` is null
    IF EXISTS (
        SELECT 1
        FROM core_planner_logs logs  
        WHERE logs.a_system_id IS NULL OR logs.b_system_id IS NULL
          AND logs.user_id = p_user_id
    ) THEN 
        UPDATE core_planner_logs log2
SET is_valid = FALSE 
WHERE user_id = p_user_id 
  AND (a_system_id IS NULL OR b_system_id IS NULL)
  AND EXISTS (
    SELECT 1 
    FROM core_planner_logs log1 
    WHERE log1.cable_id = log2.cable_id 
      AND log1.id = log2.id
  );
    END IF;

   if (SELECT count(*) 
    FROM core_planner_logs 
    WHERE user_id = p_user_id AND is_valid = TRUE) > 0 THEN
    -- Return the cable routes log records as JSON for the specified user_id
    return query select row_to_json(row) from ( select true as status, 'Success' as message ) row;
   else 
   return query select row_to_json(row) from ( select false as status, 'Failed' as message ) row;
   end if;
END;
$function$
;


---------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_associate_fiber_link_cable(p_cable_id integer, p_link_system_id integer, p_fiber_number integer, p_action character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
 
DECLARE
rec record;
p_a_end_id integer;
p_b_end_id integer;
v_count integer;
v_fms_count integer;
BEGIN

v_fms_count:= 0;
 --TEMP TABLE TO STORE CONNECTION INFO RESULT..
create temp table cpf_temp_bkp
(
	id integer, 
	connection_id integer,
	rowid integer,
	parent_connection_id integer,
	source_system_id integer, 
	source_network_id character varying,
	source_entity_type character varying, 
	source_port_no integer, 
	is_source_virtual boolean,
	destination_system_id integer, 
	destination_network_id character varying,
	destination_entity_type character varying,
	destination_port_no integer, 
	is_destination_virtual boolean, 
	is_customer_connected boolean, 
	created_on timestamp without time zone, 
	created_by integer, 
	approved_by integer, 
	approved_on timestamp without time zone, 
	cable_calculated_length double precision,
	cable_measured_length double precision,
	splitter_ratio character varying,
	is_backward_path boolean,
	trace_end character varying,
	source_display_name character varying,
	destination_display_name character varying
	 
	) on commit drop;

	create temp table cpf_temp_route
	(
	route_id character varying,
	fiber_number integer
	) on commit drop;
	--select cable_measured_length from att_details_cable 
	
	insert into cpf_temp_bkp(id, connection_id,rowid,parent_connection_id,source_system_id, source_network_id,source_entity_type, source_port_no, is_source_virtual,destination_system_id, destination_network_id,
		destination_entity_type,destination_port_no, is_destination_virtual, is_customer_connected, created_on, created_by, approved_by, approved_on, cable_calculated_length ,
		cable_measured_length,splitter_ratio,is_backward_path,trace_end,source_display_name,destination_display_name)
		select a.id, a.connection_id,a.rowid,a.parent_connection_id,a.source_system_id, a.source_network_id,a.source_entity_type, a.source_port_no, a.is_source_virtual,a.destination_system_id, a.destination_network_id,a.destination_entity_type,a.destination_port_no, a.is_destination_virtual, a.is_customer_connected, a.created_on, a.created_by, a.approved_by, a.approved_on, a.cable_calculated_length ,
		a.cable_measured_length,a.splitter_ratio,a.is_backward_path,a.trace_end,a.source_display_name,a.destination_display_name from fn_get_connection_info('', '', 1, 10,null, null, p_cable_id, p_fiber_number,'Cable') a;

insert into cpf_temp_route(route_id,fiber_number)
select route_id,fiber_number
from att_details_cable_info inf
inner join 
att_details_cable cbl on cbl.system_id=inf.cable_id
where link_system_id=p_link_system_id
group by cbl.route_id,inf.fiber_number;

select id into p_a_end_id from cpf_temp_bkp where trace_end='A' and (lower(source_entity_type) in ('splitter','ont','fms') or(lower(destination_entity_type) 
in ('splitter','ont','fms'))) order by id limit 1;

select id into p_b_end_id from cpf_temp_bkp where trace_end='B' and (lower(source_entity_type) in ('splitter','ont','fms') or(lower(destination_entity_type) 
in ('splitter','ont','fms'))) order by id limit 1;


CREATE TEMP TABLE temp_cable_info AS
select  distinct a.* from (
select source_system_id as cable_system_id,source_network_id as cable_network_id,source_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='A'  and case when coalesce(p_a_end_id,0)>0 then id<p_a_end_id else true end and upper(source_entity_type)=upper('cable')
union
select destination_system_id as cable_system_id,destination_network_id as cable_network_id,destination_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='A'  and case when coalesce(p_a_end_id,0)>0 then id<p_a_end_id else true end and upper(destination_entity_type)=upper('Cable')
union
select source_system_id as cable_system_id,source_network_id as cable_network_id,source_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='B'  and case when coalesce(p_b_end_id,0)>0 then id<p_b_end_id else true end and upper(source_entity_type)=upper('cable')
union
select destination_system_id as cable_system_id,destination_network_id as cable_network_id,destination_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='B'  and case when coalesce(p_b_end_id,0)>0 then id<p_b_end_id else true end  and upper(destination_entity_type)=upper('Cable')
) a;
  IF EXISTS(SELECT 1 from ATT_DETAILS_CABLE_INFO where cable_id=p_cable_id)THEN
	IF(p_action='A')THEN 
		UPDATE ATT_DETAILS_CABLE_INFO set link_system_id=p_link_system_id where cable_id=p_cable_id and fiber_number=p_fiber_number;
		-- UPDATE FIBER LINK STATUS TO ASSOCIATE 
IF (SELECT COUNT(*) FROM cpf_temp_bkp) > 0 then

    UPDATE att_details_fiber_link SET  fiber_link_status = 'Associated',gis_length = COALESCE(
            (SELECT SUM(ctb.cable_measured_length) FROM cpf_temp_bkp ctb), 0 ), total_route_length = COALESCE(
            (SELECT SUM(ctb.cable_calculated_length) FROM cpf_temp_bkp ctb), 0) WHERE system_id = p_link_system_id;
else

   UPDATE att_details_fiber_link SET fiber_link_status = 'Associated',
    gis_length = COALESCE(
        (SELECT cable_measured_length FROM att_details_cable WHERE system_id = p_cable_id), 0) + COALESCE(gis_length, 0),
    total_route_length = COALESCE(
        (SELECT cable_calculated_length FROM att_details_cable WHERE system_id = p_cable_id), 0) + COALESCE(total_route_length, 0)
   WHERE  system_id = p_link_system_id;
           
           
END IF;

		UPDATE att_details_cable_info A SET link_system_id= p_link_system_id
		FROM temp_cable_info B WHERE  B.cable_system_id=A.cable_id and B.cable_fiber_number=A.fiber_number;
		drop table temp_cable_info;
	
	ELSIF(p_action='D')THEN
	raise info'Delete1%',p_link_system_id;
	select count(*) into v_count from att_details_cable_info where link_system_id=p_link_system_id;
	raise info'Delete1%',v_count;
		
		UPDATE ATT_DETAILS_CABLE_INFO set link_system_id=0 where cable_id=p_cable_id and fiber_number=p_fiber_number;
		
		UPDATE att_details_cable_info A SET link_system_id= 0
		FROM temp_cable_info B WHERE  B.cable_system_id=A.cable_id and B.cable_fiber_number=A.fiber_number;
		drop table temp_cable_info;
		IF NOT Exists(select 1 from att_details_cable_info where link_system_id=p_link_system_id)THEN
			UPDATE att_details_fiber_link set fiber_link_status='Free',gis_length = 0,total_route_length= 0 where system_id=p_link_system_id;
			raise info'Delete2%',45;
		END IF;
		 
	END IF;
drop table if exists cpf_temp_bkp;
drop table if exists cpf_temp_route;
drop table if exists cpf_temp;

		RETURN QUERY 
		SELECT TRUE AS status,'Success!'::CHARACTER VARYING AS message;
		RETURN;
	ELSE
		RETURN QUERY
		SELECT FALSE AS status,'FAIL!'::CHARACTER VARYING AS message;
		RETURN;
	END IF;
	  
END
$function$
;


--------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_logs(p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
    rec RECORD;
BEGIN   
    -- Return the cable routes log records as JSON for the specified user_id
    RETURN QUERY 
    SELECT row_to_json(core_planner_logs.*) 
    FROM core_planner_logs 
    WHERE user_id = p_user_id ;

END;
$function$
;


