-- FUNCTION: public.fn_splicing_delete_connection(text)

-- DROP FUNCTION IF EXISTS public.fn_splicing_delete_connection(text);

CREATE OR REPLACE FUNCTION public.fn_splicing_delete_connection(
	p_listconnection text)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE geo_data text;
v_connection_info json;
curgeommapping refcursor;  
  v_source_system_id integer;  
  v_source_entity_type character varying(100);
  v_source_port_no integer;
  v_destination_system_id integer;
  v_destination_entity_type character varying(100);
  v_destination_port_no integer;
  v_is_left_cable_start_point boolean NOT NULL DEFAULT false;
  v_is_right_cable_start_point boolean NOT NULL DEFAULT false;
  v_is_source_cable_a_end boolean;
  v_is_destination_cable_a_end boolean;
  v_equipment_system_id integer;
  v_equipment_entity_type character varying(100);  
  v_source_network_id character varying;
  v_isvalid boolean;
  v_message character varying;
  v_count integer;
  v_isvalid_request boolean;
   v_fms_mapping record;
   destionation_linksystemId integer;
   source_linksystemId integer;
BEGIN
v_isvalid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_062]';--Connection has deleted successfully!
v_count:=0;

raise info 'p_listconnection %',p_listconnection;

CREATE temp TABLE temp_port_status_table 
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
  equipment_tray_system_id integer,
  created_by integer,
  splicing_source character varying(100),
  source_entity_sub_type character varying,
  destination_entity_sub_type character varying,
  is_through_connection BOOLEAN DEFAULT FALSE
) ON COMMIT DROP ;

CREATE temp TABLE temp_connection2
(  
  source_system_id integer,  
  source_entity_type character varying(100),
  source_port_no integer,
  destination_system_id integer, 
  destination_entity_type character varying(100),
  destination_port_no integer,  
  is_source_cable_a_end boolean NOT NULL DEFAULT false,
  is_destination_cable_a_end boolean NOT NULL DEFAULT false,
  equipment_system_id  integer,  
  equipment_entity_type character varying(100)
) ON COMMIT DROP ;

OPEN curgeommapping FOR select source_system_id,source_entity_type,source_port_no,destination_system_id,destination_entity_type,destination_port_no,equipment_system_id,equipment_entity_type,is_source_cable_a_end,is_destination_cable_a_end  from json_populate_recordset(null::temp_connection2,replace(p_listconnection,'\','')::json);
	LOOP
		FETCH  curgeommapping into v_source_system_id,v_source_entity_type,v_source_port_no,v_destination_system_id,v_destination_entity_type,v_destination_port_no,v_equipment_system_id,v_equipment_entity_type,v_is_source_cable_a_end,v_is_destination_cable_a_end;
		-- EXIT FOR LOOP
		IF NOT FOUND THEN
		EXIT;
		END IF;

    -- check link_system_id given cable_id 
    select distinct link_system_id into destionation_linksystemId from att_details_cable_info 
    where cable_id = v_destination_system_id and v_destination_entity_type='Cable';
	
	  select distinct link_system_id into source_linksystemId from att_details_cable_info 
    where cable_id = v_source_system_id and v_source_entity_type='Cable';
	
update isp_port_info set link_system_id=0 
where parent_system_id=v_source_system_id
and parent_entity_type=v_source_entity_type
and port_number=v_source_port_no;

update isp_port_info set link_system_id=0 
where parent_system_id=v_destination_system_id
and parent_entity_type=v_destination_entity_type
and port_number=v_destination_port_no;

update att_details_cable_info set link_system_id=0 
where cable_id=v_source_system_id
and v_source_entity_type='Cable'
and fiber_number=v_source_port_no;

update att_details_cable_info set link_system_id=0 
where cable_id=v_destination_system_id
and v_destination_entity_type='Cable'
and fiber_number=v_destination_port_no;


  
	if not exists (SELECT 1 FROM att_details_cable_info WHERE link_system_id= destionation_linksystemId)
    then
	-- update fiber_link_status free when no fiberlink attach at any cable 
    update att_details_fiber_link set fiber_link_status ='Free' 
    where system_id = destionation_linksystemId
    and fiber_link_status ='Associated'; 	
	end if;
	
	if not exists (SELECT 1 FROM att_details_cable_info WHERE link_system_id= source_linksystemId)
    then
	-- update fiber_link_status free when no fiberlink attach at any cable 
    update att_details_fiber_link set fiber_link_status ='Free' 
    where system_id = source_linksystemId
    and fiber_link_status ='Associated'; 	
	end if;
	
			v_isvalid_request:=true;
			
			select count(1) into v_count from (
			select * FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))
			union
			--PARENT CONNECTOR TO CABLE
			select * FROM CONNECTION_INFO 
			WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))
			) a;

		--if(upper(v_source_entity_type)!=upper('equipment') and v_count=2 )
		if(upper(v_source_entity_type)!=upper('equipment')  )
		then
			-- --CABLE TO PARENT CONNECTOR
			DELETE FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
			--PARENT CONNECTOR TO CABLE
			DELETE FROM CONNECTION_INFO 
			WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
		elsif(upper(v_source_entity_type)=upper('EQUIPMENT') and upper(V_DESTINATION_ENTITY_TYPE)=upper('EQUIPMENT'))
		then
			-- --CABLE TO PARENT CONNECTOR
			DELETE FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			or (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)); 
			
			--PARENT CONNECTOR TO CABLE
			DELETE FROM CONNECTION_INFO 
			WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)
			or (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false));
		else 	
			v_isvalid_request:=false;
			v_message:='[SI_GBL_GBL_GBL_GBL_063]';--Invalid delete request!
			insert into temp_invalid_delete_request(request,created_on) values(p_listconnection,now());		
		end if;

if(v_isvalid_request)
then
		--CHECK THE SPLITTER CONNECTION IN DESTINATION
		if(upper(v_source_entity_type)='SPLITTER' or upper(v_source_entity_type)='ONT')
		then

		       DELETE FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
			
			
			--SPLITTER TO SPLITTER or ONT TO ONT
			if((select count(1) from connection_info where (source_system_id=v_source_system_id and 
			upper(source_entity_type)=upper(v_source_entity_type) and upper(destination_entity_type)!=upper(v_source_entity_type))
			 or (destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) 
			and upper(source_entity_type)!=upper(v_source_entity_type)))=0)
			then
				delete from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type)
				and destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and is_cable_a_end=false;			
			end if;

		elsif(upper(v_destination_entity_type)='SPLITTER' or upper(v_destination_entity_type)='ONT')
		then

			DELETE FROM CONNECTION_INFO 
			WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));

			--SPLITTER TO SPLITTER or ONT TO ONT
			if((select count(1) from connection_info where (source_system_id=v_destination_system_id and 
			upper(source_entity_type)=upper(v_destination_entity_type) and upper(destination_entity_type)!=upper(v_destination_entity_type)) 
			or (destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
			and upper(source_entity_type)!=upper(v_destination_entity_type)))=0)
			then			
				delete from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type)
				and destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
				and is_cable_a_end=false;				
			end if;
		
		elsif(upper(v_source_entity_type)=upper('FMS'))
		then
			delete from connection_info 
			where (source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) and source_port_no=v_source_port_no) 
			or (destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and destination_port_no=v_source_port_no);

			select network_id into v_source_network_id from att_details_fms where system_id=v_source_system_id;

			--CHECK THE MIDDLEWARE CONNECTION EXIST AT SOURCE
			
			if exists(select 1 from att_details_model where network_id=v_source_network_id)
			then
				select * into v_fms_mapping from att_details_model where network_id=v_source_network_id;
				
				if not exists(select 1 from connection_info where 
				(source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' 
				and source_port_no=(-1*v_source_port_no) and upper(destination_entity_type)='PATCHCORD')
				or (destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
				and destination_port_no=(-1*v_source_port_no) and upper(source_entity_type)='PATCHCORD')) 				
				then					
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='FMS';

					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='FMS';
										
				end if;
			end if;		
		ELSIF(upper(v_destination_entity_type)=upper('FMS'))
		THEN
			delete from connection_info 
			where (source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and 
			source_port_no=v_destination_port_no) 
			or(destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
			and destination_port_no=v_destination_port_no);

			select network_id into v_source_network_id from att_details_fms where system_id=v_destination_system_id;

			if exists(select 1 from att_details_model where network_id=v_source_network_id)
			then			
				select * into v_fms_mapping from att_details_model where network_id=v_source_network_id;

				if not exists(select 1 from connection_info 
				where  
				(source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' 
				and source_port_no=(-1*v_destination_port_no) and upper(destination_entity_type)='PATCHCORD')
				or (destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
				and destination_port_no=(-1*v_destination_port_no) and upper(source_entity_type)='PATCHCORD'))
				then					
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='FMS';

					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_destination_port_no
					and destination_system_id=v_destination_system_id and upper(destination_entity_type)='FMS';
				end if;	
			end if;	
		elsif(upper(v_source_entity_type)=upper('EQUIPMENT') and upper(V_DESTINATION_ENTITY_TYPE)=upper('EQUIPMENT'))
		then
			if not exists(select * from connection_info where ((destination_system_id=v_source_system_id 
			and upper(destination_entity_type)=upper(v_source_entity_type) and destination_port_no=v_source_port_no)
			or (source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) 
			and source_port_no=v_source_port_no)) and (source_port_no>0 and destination_port_no>0))
			then
				delete from connection_info ci
				where ((lower(ci.destination_entity_type)||ci.destination_system_id)=(lower(ci.source_entity_type)||ci.source_system_id) 
				and ci.source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) and
				((ci.source_port_no=(-1*v_source_port_no) and ci.destination_port_no=v_source_port_no) or 
				(ci.destination_port_no=(-1*v_source_port_no) and ci.source_port_no=v_source_port_no)));

			end if;
				
			if not exists(select * from connection_info where ((destination_system_id=v_destination_system_id and 
			upper(destination_entity_type)=upper(v_destination_entity_type) and destination_port_no=v_destination_port_no)
			or (source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and source_port_no=v_destination_port_no)) 
			and (source_port_no>0 and destination_port_no>0))
			then
				delete from connection_info ci
				where ((lower(ci.destination_entity_type)||ci.destination_system_id)=(lower(ci.source_entity_type)||ci.source_system_id) 
				and ci.source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and
				((ci.source_port_no=(-1*v_destination_port_no) and ci.destination_port_no=v_destination_port_no) or 
				(ci.destination_port_no=(-1*v_destination_port_no) and ci.source_port_no=v_destination_port_no)));
				
			end if;	

			--DELETE THE MIDDLE WARE CONNECTION IF SOURCE IS MIDDLEWARE ENTITY
			select network_id into v_source_network_id from att_details_model where system_id=v_source_system_id;

			if  v_source_network_id is not null
			then
				select * into v_fms_mapping from att_details_fms where network_id=v_source_network_id;
				
				if not exists(select 1 from connection_info where 
				(source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' 
				and source_port_no=ABS(v_source_port_no) and upper(destination_entity_type)='CABLE')
				or (destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' 
				and destination_port_no=ABS(v_source_port_no) and upper(source_entity_type)='CABLE')) 				
				then					
										
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='EQUIPMENT';

					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='EQUIPMENT';

				end if;
			end if;	

			
			--DELETE THE MIDDLE WARE CONNECTION IF DESTINATION IS MIDDLEWARE ENTITY
			select network_id into v_source_network_id from att_details_model where system_id=v_destination_system_id;

			if  v_source_network_id is not null
			then
				select * into v_fms_mapping from att_details_fms where network_id=v_source_network_id;
				
				if not exists(select 1 from connection_info where 
				(source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' 
				and source_port_no=ABS(v_destination_port_no) and upper(destination_entity_type)='CABLE')
				or (destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' 
				and destination_port_no=ABS(v_destination_port_no) and upper(source_entity_type)='CABLE')) 				
				then					
										
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='EQUIPMENT';

					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' and source_port_no=v_destination_port_no
					and destination_system_id=v_destination_system_id and upper(destination_entity_type)='EQUIPMENT';

				end if;
			end if;
								
		end if;
end if;
	END LOOP;
	close curgeommapping;
  insert into temp_port_status_table (
  source_system_id,source_entity_type,source_port_no,destination_system_id,
  destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,
  equipment_system_id, equipment_entity_type
) select source_system_id,source_entity_type,source_port_no,destination_system_id,
 destination_entity_type,destination_port_no,coalesce(is_source_cable_a_end,false) as is_source_cable_a_end,coalesce(is_destination_cable_a_end,false) as is_destination_cable_a_end,
 equipment_system_id, equipment_entity_type 
from json_populate_recordset(null::temp_connection2,replace(p_listconnection,'\','')::json);

--pass hardcoded status = 1 for update ports

Perform(fn_update_port_status_bulk_splicing(1));

RETURN QUERY  SELECT v_isvalid AS STATUS, v_message::CHARACTER VARYING AS MESSAGE;	  
END
$BODY$;

ALTER FUNCTION public.fn_splicing_delete_connection(text)
    OWNER TO postgres;
	
	
	
	------------------------------------------------------------------------------------------------------------------------
	
	CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation(source_network_id character varying, destination_network_id character varying, buffer integer, required_core integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

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
  p_source_network_id character varying;
	p_destination_network_id character varying;
BEGIN
	p_source_network_id:=source_network_id;
	p_destination_network_id:=destination_network_id;
	
    availableCoreDestination := 0;
    usedCore := 0;
    t_count :=0;
   
     -- truncate table temp_cable_routes; 
     --truncate table temp_isp_port_info; 
     -- truncate table temp_route_connection; 
       
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
	and A.destination_port_no=B.source_port_no where A.source_system_id in(select edge_targetid from temp_cable_routes) 
and A.source_entity_type='Cable'

	union 
	select A.destination_system_id,A.destination_network_id,A.destination_entity_type,A.destination_port_no,
	B.source_system_id, B.source_network_id,B.source_entity_type,B.source_port_no
	from connection_info A
	inner join connection_info B 
	on B.destination_system_id=A.Source_system_id and B.destination_entity_type=A.Source_entity_type
	and B.destination_port_no=A.Source_port_no where A.destination_system_id in(select edge_targetid from temp_cable_routes) and A.destination_entity_type='Cable'	
	union 
	select A.destination_system_id,A.destination_network_id,A.destination_entity_type,A.destination_port_no,
	A.source_system_id, A.source_network_id,A.source_entity_type,A.source_port_no
	from connection_info A
	where A.Source_entity_type='FMS' and A.source_network_id in(p_source_network_id,p_destination_network_id) 
	and A.destination_entity_type='Cable'
	union 
	select A.destination_system_id,A.destination_network_id,A.destination_entity_type,A.destination_port_no,
	A.source_system_id, A.source_network_id,A.source_entity_type,A.source_port_no
	from connection_info A
	where A.destination_entity_type='FMS' and A.destination_network_id in(p_source_network_id,p_destination_network_id)
	and A.source_entity_type='Cable';

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
	where temp_isp_port_info.port_number=A.source_port_no and A.Source_entity_type='Cable'
	and A.source_system_id not in(select edge_targetid from temp_cable_routes); 

	update temp_isp_port_info set is_valid=false
	from temp_route_connection A
	where temp_isp_port_info.port_number=A.source_port_no and A.destination_entity_type='Cable'
	and A.destination_system_id not in(select edge_targetid from temp_cable_routes); 

	update temp_isp_port_info set is_valid=false
	from temp_route_connection A
	where temp_isp_port_info.port_number=A.source_port_no and A.Source_entity_type='FMS' and A.destination_entity_type='Cable'
	and A.destination_system_id not in(select edge_targetid from temp_cable_routes); 

	update temp_isp_port_info set is_valid=false
	from temp_route_connection A
	where temp_isp_port_info.port_number=A.source_port_no and A.destination_entity_type='FMS' and A.Source_entity_type='Cable'
	and A.source_system_id not in(select edge_targetid from temp_cable_routes); 
	
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
                       WHERE core_planner_logs.user_id = p_user_id and pm.common_name in (source_network_id ,destination_network_id ) )
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
                       WHERE core_planner_logs.user_id = p_user_id and pm.common_name in (source_network_id ,destination_network_id))
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
$function$
;
