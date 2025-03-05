alter table audit_connection_info  add column action_by integer;

CREATE OR REPLACE FUNCTION public.fn_splicing_delete_connection(
	p_listconnection text,
    p_userid integer)
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

OPEN curgeommapping FOR select source_system_id,source_entity_type,source_port_no,destination_system_id,destination_entity_type,
destination_port_no,equipment_system_id,equipment_entity_type,is_source_cable_a_end,is_destination_cable_a_end 
from json_populate_recordset(null::temp_connection2,replace(p_listconnection,'\','')::json);
	LOOP
		FETCH  curgeommapping into v_source_system_id,v_source_entity_type,v_source_port_no,v_destination_system_id,v_destination_entity_type,v_destination_port_no,v_equipment_system_id,v_equipment_entity_type,v_is_source_cable_a_end,v_is_destination_cable_a_end;
		-- EXIT FOR LOOP
		IF NOT FOUND THEN
		EXIT;
		END IF;

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

INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO  WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
       
            -- --CABLE TO PARENT CONNECTOR
			DELETE FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=V_IS_SOURCE_CABLE_A_END)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));
		
        
            INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=V_IS_DESTINATION_CABLE_A_END)) 
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
        
        INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE
     ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			or (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)); 

			-- --CABLE TO PARENT CONNECTOR
			DELETE FROM CONNECTION_INFO 
			WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			or (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)); 
			
             INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER('EQUIPMENT') AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)
			or (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER('EQUIPMENT') AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
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
     
     INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE ((SOURCE_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND SOURCE_PORT_NO=V_SOURCE_PORT_NO 
			AND IS_CABLE_A_END=false)
			OR (DESTINATION_SYSTEM_ID=V_SOURCE_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_SOURCE_ENTITY_TYPE) AND DESTINATION_PORT_NO=V_SOURCE_PORT_NO
			AND IS_CABLE_A_END=false)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));



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
                 
                 INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE  source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type)
				and destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and is_cable_a_end=false;			
	
				delete from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type)
				and destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and is_cable_a_end=false;			
			end if;

		elsif(upper(v_destination_entity_type)='SPLITTER' or upper(v_destination_entity_type)='ONT')
		then
INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE  ((DESTINATION_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(DESTINATION_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) 
			AND DESTINATION_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)
			OR (SOURCE_SYSTEM_ID=V_DESTINATION_SYSTEM_ID AND UPPER(SOURCE_ENTITY_TYPE)=UPPER(V_DESTINATION_ENTITY_TYPE) AND SOURCE_PORT_NO=V_DESTINATION_PORT_NO
			AND IS_CABLE_A_END=false)) 
			and ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text));



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
            
            INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type)
				and destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
				and is_cable_a_end=false;	
            
				delete from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type)
				and destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
				and is_cable_a_end=false;				
			end if;
		
		elsif(upper(v_source_entity_type)=upper('FMS'))
		then
        
        INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE
    (source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) and source_port_no=v_source_port_no) 
			or (destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) and destination_port_no=v_source_port_no);

        
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
                
                INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='FMS';
                                              
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='FMS';
                    
                    INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO WHERE upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='FMS';

					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='FMS';
										
				end if;
			end if;		
		ELSIF(upper(v_destination_entity_type)=upper('FMS'))
		THEN
        INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where (source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and 
			source_port_no=v_destination_port_no) 
			or(destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) 
			and destination_port_no=v_destination_port_no);

        
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
                INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='FMS';
                
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' 
					and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='FMS';

INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_destination_port_no
					and destination_system_id=v_destination_system_id and upper(destination_entity_type)='FMS';

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
            INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO ci
				where ((lower(ci.destination_entity_type)||ci.destination_system_id)=(lower(ci.source_entity_type)||ci.source_system_id) 
				and ci.source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) and
				((ci.source_port_no=(-1*v_source_port_no) and ci.destination_port_no=v_source_port_no) or 
				(ci.destination_port_no=(-1*v_source_port_no) and ci.source_port_no=v_source_port_no)));
            
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
            
            INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO ci
				where ((lower(ci.destination_entity_type)||ci.destination_system_id)=(lower(ci.source_entity_type)||ci.source_system_id) 
				and ci.source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and
				((ci.source_port_no=(-1*v_destination_port_no) and ci.destination_port_no=v_destination_port_no) or 
				(ci.destination_port_no=(-1*v_destination_port_no) and ci.source_port_no=v_destination_port_no)));
    
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
					INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='EQUIPMENT';
    
    
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_source_port_no
					and source_system_id=v_source_system_id and upper(source_entity_type)='EQUIPMENT';

INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' and source_port_no=v_source_port_no
					and destination_system_id=v_source_system_id and upper(destination_entity_type)='EQUIPMENT';
    
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
					INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='EQUIPMENT';
                    
					delete from connection_info where upper(source_network_id)=upper(destination_network_id) 
					and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='FMS' and destination_port_no=v_destination_port_no
					and source_system_id=v_destination_system_id and upper(source_entity_type)='EQUIPMENT';

INSERT INTO public.audit_connection_info(
	 connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no, is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, action, action_date, action_by)
    select connection_id, source_system_id, source_network_id, source_entity_type, source_port_no, destination_system_id, destination_network_id, destination_entity_type, destination_port_no,is_customer_connected, created_on, created_by, approved_by, approved_on, splicing_source, is_cable_a_end, 'D', now(), p_userid
    FROM CONNECTION_INFO where upper(source_network_id)=upper(destination_network_id) 
					and source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='FMS' and source_port_no=v_destination_port_no
					and destination_system_id=v_destination_system_id and upper(destination_entity_type)='EQUIPMENT';
    
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
 equipment_system_id, coalesce(equipment_entity_type ,'') as equipment_entity_type
from json_populate_recordset(null::temp_connection2,replace(p_listconnection,'\','')::json);

--pass hardcoded status = 1 for update ports

Perform(fn_update_port_status_bulk_splicing(1));

RETURN QUERY  SELECT v_isvalid AS STATUS, v_message::CHARACTER VARYING AS MESSAGE;	  
END
$BODY$;


