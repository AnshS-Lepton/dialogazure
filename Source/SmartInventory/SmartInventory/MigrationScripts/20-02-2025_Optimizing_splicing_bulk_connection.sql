
CREATE OR REPLACE FUNCTION public.fn_splicing_save_connections(
	p_connection text)
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
  v_equipment_tray_system_id integer;
  v_source_port_no integer;
  v_destination_system_id integer;
  v_destination_network_id character varying(100);
  v_destination_entity_type character varying(100);
  v_destination_port_no integer;
  v_is_source_cable_a_end boolean  DEFAULT false;
  v_is_destination_cable_a_end boolean  DEFAULT false;
  v_created_by integer;
  v_splicing_source character varying(100);
  v_is_through_connection boolean  DEFAULT false;
  curgeommapping refcursor;

  counter INTEGER;
  V_SPL_INPUT_PORT INTEGER;
  V_SPL_OUTPUT_PORT INTEGER;
  v_fms_mapping record;
  v_source_entity_sub_type character varying;
  v_destination_entity_sub_type character varying;
  v_layer_table character varying;
  v_is_mapped boolean;
BEGIN
v_is_mapped:=false;
v_equipment_port_no:=1;
v_is_source_cable_a_end:=false;
v_is_destination_cable_a_end:=false;
counter:=0;

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

CREATE temp TABLE temp_connection
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
  is_through_connection BOOLEAN DEFAULT FALSE 			
)on commit drop;
  	
OPEN curgeommapping FOR select source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
destination_network_id,destination_entity_type,destination_port_no,coalesce(is_source_cable_a_end,false) as is_source_cable_a_end,coalesce(is_destination_cable_a_end,false) as is_destination_cable_a_end,equipment_system_id,equipment_network_id,
equipment_entity_type,equipment_tray_system_id,created_by,splicing_source,source_entity_sub_type,destination_entity_sub_type,is_through_connection 
 from json_populate_recordset(null::temp_connection,replace(p_connection,'\','')::json);
	LOOP

		truncate table temp_connections;
	
		FETCH  curgeommapping into v_source_system_id,v_source_network_id,v_source_entity_type,v_source_port_no,v_destination_system_id,
		v_destination_network_id,v_destination_entity_type,v_destination_port_no,v_is_source_cable_a_end,v_is_destination_cable_a_end,
		v_equipment_system_id,v_equipment_network_id,v_equipment_entity_type,v_equipment_tray_system_id,v_created_by,v_splicing_source, 
		v_source_entity_sub_type,v_destination_entity_sub_type,v_is_through_connection;
		-- EXIT FOR LOOP
		IF NOT FOUND THEN
		EXIT;
		END IF;

v_equipment_tray_system_id:=coalesce(v_equipment_tray_system_id,0);

raise info'%v_source_entity_type',v_source_entity_type;

IF(upper(v_source_entity_type)='SPLITTER')
THEN
	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type))=0)
	then
		select split_part(splitter_ratio, ':', 1)::INTEGER  ,split_part(splitter_ratio, ':', 2)::INTEGER  INTO V_SPL_INPUT_PORT,V_SPL_OUTPUT_PORT from att_details_splitter WHERE SYSTEM_ID=v_source_system_id;
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,source_tray_system_id,destination_tray_system_id,is_through_connection)
		SELECT v_source_system_id,v_source_network_id,v_source_entity_type,-1*input,v_source_system_id,v_source_network_id,v_source_entity_type,output,false,v_splicing_source,0,0,v_is_through_connection from generate_series(1,V_SPL_INPUT_PORT) input  
		cross join generate_series(1,V_SPL_OUTPUT_PORT) output;
	end if;

elsIF(upper(v_destination_entity_type)='SPLITTER')
THEN
	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type))=0)
	then
		select split_part(splitter_ratio, ':', 1)::INTEGER  ,split_part(splitter_ratio, ':', 2)::INTEGER  INTO V_SPL_INPUT_PORT,V_SPL_OUTPUT_PORT from att_details_splitter WHERE SYSTEM_ID=v_destination_system_id;
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,source_tray_system_id,destination_tray_system_id,is_through_connection)
		SELECT v_destination_system_id,v_destination_network_id,v_destination_entity_type,-1*input,v_destination_system_id,v_destination_network_id,v_destination_entity_type,output,false,v_splicing_source,0,0,v_is_through_connection from generate_series(1,V_SPL_INPUT_PORT) input  
		cross join generate_series(1,V_SPL_OUTPUT_PORT) output;
	end if;

elsif(upper(v_source_entity_type)='ONT')
then
	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type))=0)
	then
		select no_of_input_port,no_of_output_port  INTO V_SPL_INPUT_PORT,V_SPL_OUTPUT_PORT from att_details_ont WHERE SYSTEM_ID=v_source_system_id;
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,source_tray_system_id,destination_tray_system_id,is_through_connection)
		SELECT v_source_system_id,v_source_network_id,v_source_entity_type,-1*input,v_source_system_id,v_source_network_id,v_source_entity_type,output,false,v_splicing_source,0,0,v_is_through_connection from generate_series(1,V_SPL_INPUT_PORT) input  
		cross join generate_series(1,V_SPL_OUTPUT_PORT) output;
	end if;
	
elsif(upper(v_destination_entity_type)='ONT')
then	
	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type))=0)
	then
		select no_of_input_port,no_of_output_port INTO V_SPL_INPUT_PORT,V_SPL_OUTPUT_PORT from att_details_ont WHERE SYSTEM_ID=v_destination_system_id;
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,source_tray_system_id,destination_tray_system_id,is_through_connection)
		SELECT v_destination_system_id,v_destination_network_id,v_destination_entity_type,-1*input,v_destination_system_id,v_destination_network_id,v_destination_entity_type,output,false,v_splicing_source,0,0,v_is_through_connection from generate_series(1,V_SPL_INPUT_PORT) input  
		cross join generate_series(1,V_SPL_OUTPUT_PORT) output;
	end if;	
elsif(upper(v_source_entity_type)='FMS' or upper(v_source_entity_type)='PATCHPANEL')
then
	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) 
	and source_port_no=case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end 
	and  destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type))=0)
	then			
		insert into connection_info
		(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,source_tray_system_id,destination_tray_system_id,is_through_connection)
		values(v_source_system_id,v_source_network_id,v_source_entity_type,
		(case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end),v_source_system_id,v_source_network_id,v_source_entity_type,
		(case when v_source_port_no<0 then -1*v_source_port_no else v_source_port_no end),false,v_splicing_source,0,0,v_is_through_connection);
	end if;	

			
elsif(upper(v_destination_entity_type)='FMS' or upper(v_source_entity_type)='PATCHPANEL')
then	
	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) 
	and source_port_no=case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end 
	and  destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type))=0)
	then	
				
		insert into connection_info
		(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,source_tray_system_id,destination_tray_system_id,is_through_connection)
		values(v_destination_system_id,v_destination_network_id,v_destination_entity_type,
		(case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end ),v_destination_system_id,v_destination_network_id,
		v_destination_entity_type,(case when v_destination_port_no<0 then -1*v_destination_port_no else v_destination_port_no end ),false,v_splicing_source,0,0,v_is_through_connection);
	end if;	

		
elsif(upper(v_source_entity_type)='HTB' or upper(v_source_entity_type)='OPTICALREPEATER')
then
	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) 
	and source_port_no=case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end 
	and  destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type))=0)
	then			
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,source_tray_system_id,destination_tray_system_id,is_through_connection)
		values(v_source_system_id,v_source_network_id,v_source_entity_type,(case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end),v_source_system_id,v_source_network_id,v_source_entity_type,
		(case when v_source_port_no<0 then -1*v_source_port_no else v_source_port_no end),false,v_splicing_source,0,0,v_is_through_connection);
	end if;	
elsif(upper(v_destination_entity_type)='HTB' or upper(v_destination_entity_type)='OPTICALREPEATER')
then	
	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) 
	and source_port_no=case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end 
	and  destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type))=0)
	then			
		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,source_tray_system_id,destination_tray_system_id,is_through_connection)
		values(v_destination_system_id,v_destination_network_id,v_destination_entity_type,(case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end ),v_destination_system_id,v_destination_network_id,
		v_destination_entity_type,(case when v_destination_port_no<0 then -1*v_destination_port_no else v_destination_port_no end ),false,v_splicing_source,0,0,v_is_through_connection);
	end if;	
 elsif(upper(v_source_entity_type)='EQUIPMENT' and (upper(v_destination_entity_type)='EQUIPMENT'))
 then
 	if((select count(1) from connection_info where source_system_id=v_source_system_id and upper(source_entity_type)=upper(v_source_entity_type) 
 	and  destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(v_source_entity_type) 
 	and ABS(source_port_no)=ABS(v_source_port_no) and ABS(destination_port_no)=ABS(v_source_port_no)
 	and (source_port_no+destination_port_no)=0)=0)
 	then			
 		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,
		destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,created_by,source_tray_system_id,destination_tray_system_id,is_through_connection)
 		values(v_source_system_id,v_source_network_id,v_source_entity_type,
 		(case when v_source_port_no<0 then v_source_port_no else -1*v_source_port_no end),v_source_system_id,v_source_network_id,v_source_entity_type,
 		(case when v_source_port_no<0 then -1*v_source_port_no else v_source_port_no end),false,v_splicing_source,v_created_by,0,0,v_is_through_connection);
 	end if;	

 	
 	if((select count(1) from connection_info where source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) 
 	and  destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type)
 	and ABS(source_port_no)=ABS(v_destination_port_no) and ABS(destination_port_no)=ABS(v_destination_port_no)
	and (source_port_no+destination_port_no)=0)=0)
 	then			
 		insert into connection_info(source_system_id,source_network_id,source_entity_type,source_port_no,
		destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,splicing_source,created_by,source_tray_system_id,destination_tray_system_id,is_through_connection)
 		values(v_destination_system_id,v_destination_network_id,v_destination_entity_type,
 		(case when v_destination_port_no<0 then v_destination_port_no else -1*v_destination_port_no end),v_destination_system_id,v_destination_network_id,v_destination_entity_type,
 		(case when v_destination_port_no<0 then -1*v_destination_port_no else v_destination_port_no end),false,v_splicing_source,v_created_by,0,0,v_is_through_connection);
 	end if; 		 			 	
end if;
--CHECK IF SOURCE IS MIDDLEWARE ENTITY
IF EXISTS(SELECT 1 FROM LAYER_DETAILS WHERE UPPER(LAYER_NAME)=UPPER(V_SOURCE_ENTITY_TYPE) and is_middleware_entity=true)
then

RAISE INFO '%', 'sssss-0';
	IF EXISTS(select 1 from att_details_model where upper(network_id)=upper(v_source_network_id))
	then
		select * into v_fms_mapping from att_details_model where upper(network_id)=upper(v_source_network_id);
		
		if not exists(select * from connection_info 
		where (source_system_id=v_source_system_id and upper(source_entity_type)=upper(V_SOURCE_ENTITY_TYPE) and source_port_no=-1*v_source_port_no
		and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' and destination_port_no=v_source_port_no)
		or (source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_source_port_no 
		and destination_system_id=v_source_system_id and upper(destination_entity_type)=upper(V_SOURCE_ENTITY_TYPE) and destination_port_no=-1*v_source_port_no))
		then	
			insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
 			destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type,source_tray_system_id,destination_tray_system_id,is_through_connection)
 			values(v_fms_mapping.system_id,v_source_network_id,'EQUIPMENT',v_source_port_no,v_source_system_id,v_source_network_id,v_source_entity_type,
 			-1*v_source_port_no,false,v_created_by,v_splicing_source,UPPER(V_SOURCE_ENTITY_TYPE),0,0,v_is_through_connection);
		end if; 	
	end if;
end if;
--CHECK IF DESTINATION IS MIDDLEWARE ENTITY
IF EXISTS(SELECT 1 FROM LAYER_DETAILS WHERE UPPER(LAYER_NAME)=UPPER(v_destination_entity_type) and is_middleware_entity=true)
then

RAISE INFO '%', 'sssss-1';
	IF EXISTS(select 1 from att_details_model where upper(network_id)=upper(v_destination_network_id))
	 then
		select * into v_fms_mapping from att_details_model where upper(network_id)=upper(v_destination_network_id);
	 
		if not exists(select * from connection_info 
		where (source_system_id=v_destination_system_id and upper(source_entity_type)=upper(v_destination_entity_type) and source_port_no=-1*v_destination_port_no
		and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)='EQUIPMENT' and destination_port_no=v_destination_port_no)
		or (source_system_id=v_fms_mapping.system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=v_destination_port_no 
		and destination_system_id=v_destination_system_id and upper(destination_entity_type)=upper(v_destination_entity_type) and destination_port_no=-1*v_destination_port_no))
		then
			insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
			destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,destination_entity_sub_type,source_tray_system_id,destination_tray_system_id,is_through_connection)
			values(v_fms_mapping.system_id,v_destination_network_id,'EQUIPMENT',v_destination_port_no,v_destination_system_id,v_destination_network_id,v_destination_entity_type,
			-1*v_destination_port_no,false,v_created_by,v_splicing_source,UPPER(v_destination_entity_type),0,0,v_is_through_connection);
		 end if;		
	 end if;	
 end if;
	
if(v_equipment_entity_type is not null)
then

if (upper(v_source_entity_type)=upper('equipment') and upper(v_destination_entity_type)=upper('equipment')) 
then
	if exists(select 1 from layer_details where upper(layer_name)=upper(v_source_entity_sub_type) and is_middleware_entity=true)
	then
		select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(v_source_entity_sub_type);
		execute 'select * from '||v_layer_table||' where upper(network_id)=upper('''||v_source_network_id||''') ' into v_fms_mapping;
		execute 'select count(1)>0 from '||v_layer_table||' where upper(network_id)=upper('''||v_source_network_id||''') ' into v_is_mapped;
		if(v_is_mapped)
		then
			--select * into v_fms_mapping from att_details_fms where network_id=v_source_network_id;
			if not exists(select * from connection_info 
			where (source_system_id=v_source_system_id and upper(source_entity_type)='EQUIPMENT' and source_port_no=-1*v_source_port_no 
			and destination_system_id=v_fms_mapping.system_id  and upper(destination_entity_type)=upper(v_source_entity_sub_type) and destination_port_no=v_source_port_no)
			or (source_system_id=v_fms_mapping.system_id  and source_entity_type=upper(v_source_entity_sub_type) and source_port_no=v_source_port_no
			and destination_system_id=v_source_system_id and upper(destination_entity_type)='EQUIPMENT' and destination_port_no=-1*v_source_port_no)
			)
			then
				insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
				destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type,source_tray_system_id,destination_tray_system_id,is_through_connection)
				values(v_source_system_id,v_source_network_id,v_source_entity_type,-1*v_source_port_no, v_fms_mapping.system_id,v_source_network_id,v_source_entity_sub_type,
				v_source_port_no,false,v_created_by,v_splicing_source,v_source_entity_sub_type,0,0,v_is_through_connection);
			end if;	
		end if;
	end if;

	if exists(select 1 from layer_details where upper(layer_name)=upper(v_destination_entity_sub_type) and is_middleware_entity=true)
	then
		select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(v_destination_entity_sub_type);
		execute 'select * from '||v_layer_table||' where upper(network_id)=upper('''||v_destination_network_id||''') ' into v_fms_mapping;
		execute 'select count(1)>0 from '||v_layer_table||' where upper(network_id)=upper('''||v_destination_network_id||''') ' into v_is_mapped;
	if (v_is_mapped)
	then
		--select * into v_fms_mapping from att_details_fms where network_id=v_destination_network_id;

	if not exists(select * from connection_info 
 	where (source_system_id=v_destination_system_id  and upper(source_entity_type)='EQUIPMENT' and source_port_no=-1*v_destination_port_no 
	and destination_system_id=v_fms_mapping.system_id and upper(destination_entity_type)=upper(v_destination_entity_sub_type) and destination_port_no=v_destination_port_no)
	or (source_system_id=v_fms_mapping.system_id and source_entity_type=upper(v_destination_entity_sub_type) and source_port_no=-1*v_destination_port_no
	and destination_system_id=v_destination_system_id  and upper(destination_entity_type)='EQUIPMENT' and destination_port_no=v_destination_port_no))
	then
		insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
		destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type,source_tray_system_id,destination_tray_system_id,is_through_connection)
		values(v_destination_system_id,v_destination_network_id,v_destination_entity_type,-1*v_destination_port_no,v_fms_mapping.system_id,v_destination_network_id,v_destination_entity_type,
		v_destination_port_no,false,v_created_by,v_splicing_source,upper(v_destination_entity_sub_type),0,0,v_is_through_connection);
	end if;	
	end if;	
end if;	
end if;

	if( (SELECT COUNT(1) FROM LAYER_DETAILS WHERE IS_VIRTUAL_PORT_ALLOWED=TRUE  AND ISVISIBLE=TRUE AND UPPER(LAYER_NAME)=UPPER(v_equipment_entity_type) 
	AND (IS_SPLICER=TRUE or IS_ISP_SPLICER=TRUE))>0 or UPPER(v_equipment_entity_type)=UPPER('PatchCord'))
	then	

	
		if(UPPER(v_equipment_entity_type)!=UPPER('PatchCord'))
		then
			--GET THE NEXT PORT OF THE PARENT CONNECTOR	
			select (max(coalesce(destination_port_no))+1) into v_equipment_port_no from connection_info where destination_system_id=v_equipment_system_id and upper(destination_entity_type)=upper(v_equipment_entity_type);	
			v_equipment_port_no=coalesce(v_equipment_port_no,1);
		end if;

		insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
		destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type,destination_entity_sub_type,source_tray_system_id,destination_tray_system_id,is_through_connection)

		values(v_source_system_id,v_source_network_id,v_source_entity_type,v_source_port_no,v_equipment_system_id,
		v_equipment_network_id,v_equipment_entity_type,v_equipment_port_no,v_is_source_cable_a_end,v_created_by,
		v_splicing_source,v_source_entity_sub_type,null,0,v_equipment_tray_system_id,v_is_through_connection),

		(v_equipment_system_id,v_equipment_network_id,v_equipment_entity_type,v_equipment_port_no,v_destination_system_id,
		v_destination_network_id,v_destination_entity_type,v_destination_port_no,v_is_destination_cable_a_end,v_created_by,
		v_splicing_source,null,v_destination_entity_sub_type,v_equipment_tray_system_id,0,v_is_through_connection);
		 
		perform(fn_splicing_insert_into_connection_info());		
	end if;

if( (SELECT COUNT(1) FROM LAYER_DETAILS WHERE IS_VIRTUAL_PORT_ALLOWED=false  AND ISVISIBLE=TRUE AND UPPER(LAYER_NAME)=UPPER(v_equipment_entity_type) 
	AND (IS_SPLICER=TRUE or IS_ISP_SPLICER=TRUE))>0)
	then
	insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type
	,destination_entity_sub_type,source_tray_system_id,destination_tray_system_id,is_through_connection)
	values(v_source_system_id,v_source_network_id,v_source_entity_type,v_source_port_no,v_destination_system_id,
	v_destination_network_id,v_destination_entity_type,v_destination_port_no,
	case when upper(v_destination_entity_type)='CABLE' then v_is_destination_cable_a_end else v_is_source_cable_a_end end,v_created_by,v_splicing_source,v_source_entity_sub_type,v_destination_entity_sub_type,0,0,v_is_through_connection);

	perform(fn_splicing_insert_into_connection_info());
end if;
	
elsif (upper(v_source_entity_type)=upper('equipment') and upper(v_destination_entity_type)=upper('equipment')) 
then

	insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type
	,destination_entity_sub_type,source_tray_system_id,destination_tray_system_id,is_through_connection)
	values(v_source_system_id,v_source_network_id,v_source_entity_type,v_source_port_no,v_destination_system_id,v_destination_network_id,v_destination_entity_type,v_destination_port_no,false,v_created_by,v_splicing_source,v_source_entity_sub_type,v_destination_entity_sub_type,0,0,v_is_through_connection);
	perform(fn_splicing_insert_into_connection_info());	
else

	insert into temp_connections(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type
	,destination_entity_sub_type,source_tray_system_id,destination_tray_system_id,is_through_connection)
	values(v_source_system_id,v_source_network_id,v_source_entity_type,v_source_port_no,v_destination_system_id,
	v_destination_network_id,v_destination_entity_type,v_destination_port_no,
	case when upper(v_destination_entity_type)='CABLE' then v_is_destination_cable_a_end else v_is_source_cable_a_end end,v_created_by,v_splicing_source,v_source_entity_sub_type,v_destination_entity_sub_type,0,0,v_is_through_connection);

	perform(fn_splicing_insert_into_connection_info());

-- insert into temp_connections2(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type,destination_entity_sub_type)
-- select source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,is_cable_a_end,created_by,splicing_source,source_entity_sub_type,destination_entity_sub_type
-- from temp_connections;

		
end if;
--Raise info 'v_equipment_system_id::::: %',v_equipment_system_id;
--Raise info 'v_equipment_entity_type::::: %',v_equipment_entity_type;
--Raise info 'v_source_entity_type::::: %',v_source_entity_type;
--Raise info 'v_destination_entity_type::::: %',v_destination_entity_type;
--Raise info 'v_source_system_id::::: %',v_source_system_id;
--Raise info 'v_destination_system_id::::: %',v_destination_system_id;
--Raise info 'v_is_destination_cable_a_end::::: %',v_is_destination_cable_a_end;
--Raise info 'v_is_source_cable_a_end::::: %',v_is_source_cable_a_end;
--Raise info 'v_created_by::::: %',v_created_by;

perform(fn_splicing_delete_junk_connection(v_equipment_system_id,v_equipment_entity_type));

perform(fn_splicing_snap_cable_end_points(coalesce(v_source_entity_type,''),coalesce(v_destination_entity_type,''),coalesce(v_equipment_entity_type,''),v_source_system_id,v_destination_system_id,v_equipment_system_id,v_is_destination_cable_a_end , v_is_source_cable_a_end,v_created_by ));

END LOOP;
close curgeommapping;
	

insert into temp_port_status_table (
  source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
    destination_network_id,destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,equipment_system_id,equipment_network_id,
    equipment_entity_type,equipment_tray_system_id,created_by,splicing_source,source_entity_sub_type,destination_entity_sub_type,is_through_connection
) select source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
destination_network_id,destination_entity_type,destination_port_no,coalesce(is_source_cable_a_end,false) as is_source_cable_a_end,coalesce(is_destination_cable_a_end,false) as is_destination_cable_a_end,equipment_system_id,equipment_network_id,
equipment_entity_type,equipment_tray_system_id,created_by,splicing_source,source_entity_sub_type,destination_entity_sub_type,is_through_connection 
from json_populate_recordset(null::temp_connection,replace(p_connection,'\','')::json);

--pass hardcoded status = 2 for update ports

Perform(fn_update_port_status_bulk_splicing(2));

return query
select row_to_json(row) from (
select true as status, 'success'::character varying as message 
 ) row;
 
END
$BODY$;







CREATE OR REPLACE FUNCTION public.fn_update_port_status_bulk_splicing(
	p_status_id integer)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE v_display_name character varying;
v_layer_table character varying;

BEGIN

update att_details_cable_info  set a_end_status_id=p_status_id
from temp_port_status_table b
where cable_id=b.source_system_id and b.source_entity_type='Cable'
and fiber_number=b.source_port_no
and b.is_source_cable_a_end=true;

update att_details_cable_info  set b_end_status_id=p_status_id 
from temp_port_status_table b
where cable_id=b.source_system_id and b.source_entity_type='Cable'
and fiber_number=b.source_port_no
and b.is_source_cable_a_end=false;

update att_details_cable_info  set a_end_status_id=p_status_id 
from temp_port_status_table b
where cable_id=b.destination_system_id and b.destination_entity_type='Cable'
and fiber_number=b.destination_port_no
and b.is_destination_cable_a_end=true;

update att_details_cable_info  set b_end_status_id=p_status_id 
from temp_port_status_table b
where cable_id=b.destination_system_id and b.destination_entity_type='Cable'
and fiber_number=b.destination_port_no
and b.is_destination_cable_a_end=false;

update isp_port_info  set port_status_id=p_status_id 
from temp_port_status_table b
where parent_system_id=b.destination_system_id and upper(parent_entity_type)=upper(b.destination_entity_type)
and port_number=case when b.destination_port_no<0 then (-1*b.destination_port_no) else b.destination_port_no end 
and input_output=case when b.destination_port_no<0 then 'I' else 'O' end;

update isp_port_info  set port_status_id=p_status_id 
from temp_port_status_table b
where parent_system_id=b.source_system_id and upper(parent_entity_type)=upper(b.source_entity_type)
and port_number=case when b.source_port_no<0 then (-1*b.source_port_no) else b.source_port_no end 
and input_output=case when b.source_port_no<0 then 'I' else 'O' end;

---Customer
update att_details_customer  set customer_status_id=p_status_id 
from temp_port_status_table b
where (system_id=b.source_system_id and b.source_entity_type='Customer')
or (system_id=b.destination_system_id and b.destination_entity_type='Customer'); 

--Equipment
update temp_port_status_table set SOURCE_NETWORK_ID=b.network_id
from att_details_model b where upper(temp_port_status_table.source_entity_type)=upper('Equipment') and
b.system_id=source_system_id;

update temp_port_status_table set DESTINATION_NETWORK_ID=b.network_id
from att_details_model b where upper(temp_port_status_table.destination_entity_type)=upper('Equipment') and 
b.system_id=destination_system_id;

update isp_port_info set port_status_id=p_status_id 
from temp_port_status_table b
where parent_system_id=b.source_system_id and upper(parent_entity_type)=upper(b.source_entity_type) 
and port_number=case when b.source_port_no<0 then (-1*b.source_port_no) else b.source_port_no end 
and upper(input_output)=case when b.source_port_no<0 then 'I' else 'O' end
and upper(b.source_entity_type)=upper('Equipment')
and  ((b.source_port_no<0 and b.destination_port_no<0) or (b.source_port_no>0 and b.destination_port_no>0));

if(p_status_id = 2) then
update isp_port_info set port_status_id=p_status_id
from temp_port_status_table b
where parent_system_id=b.destination_system_id and upper(parent_entity_type)=upper(b.destination_entity_type) 
and port_number=case when b.destination_port_no<0 then (-1*b.destination_port_no) else b.destination_port_no end 
and upper(input_output) in ('I','O')
and upper(b.destination_entity_type)=upper('Equipment')
and  EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(b.destination_NETWORK_ID)) 
and upper(b.splicing_source)='EQUIPMENT_SPLICING';

update isp_port_info set port_status_id=p_status_id
from temp_port_status_table b
where parent_system_id=b.source_system_id and upper(parent_entity_type)=upper(b.source_entity_type) 
and port_number=case when b.source_port_no<0 then (-1*b.source_port_no) else b.source_port_no end 
and upper(input_output) in ('I','O')
and upper(b.source_entity_type)=upper('Equipment')
and  EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(b.SOURCE_NETWORK_ID)) 
and upper(b.splicing_source)='EQUIPMENT_SPLICING';

update connection_info ci set source_display_name=display_name from
(select display_name,system_id,entity_type from line_master lm
 inner join temp_port_status_table b on lm.entity_type=b.source_entity_type) b
where ci.source_system_id= b.system_id and ci.source_entity_type = b.entity_type ;

update connection_info ci set source_display_name=display_name from
(select display_name,system_id,entity_type from line_master lm
 inner join temp_port_status_table b on lm.entity_type=b.destination_entity_type) b
where ci.destination_system_id= b.system_id and ci.destination_entity_type = b.entity_type ;

update connection_info ci set source_display_name=display_name from
(select display_name,system_id,entity_type from point_master pm 
 inner join temp_port_status_table b on pm.entity_type=b.source_entity_type) b
where ci.source_system_id= b.system_id and ci.source_entity_type = b.entity_type ;

update connection_info ci set source_display_name=display_name from
(select display_name,system_id,entity_type from point_master pm
 inner join temp_port_status_table b on pm.entity_type=b.destination_entity_type) b
where ci.destination_system_id= b.system_id and ci.destination_entity_type = b.entity_type ;

End if;

if(p_status_id = 1) then
update isp_port_info set port_status_id=p_status_id
from temp_port_status_table b
where parent_system_id=b.destination_system_id and upper(parent_entity_type)=upper(b.destination_entity_type) 
and port_number=case when b.destination_port_no<0 then (-1*b.destination_port_no) else b.destination_port_no end 
and upper(input_output) in ('I','O')
and upper(b.destination_entity_type)=upper('Equipment');

update isp_port_info set port_status_id=p_status_id
from temp_port_status_table b
where parent_system_id=b.source_system_id and upper(parent_entity_type)=upper(b.source_entity_type) 
and port_number=case when b.source_port_no<0 then (-1*b.source_port_no) else b.source_port_no end 
and upper(input_output) in ('I','O')
and upper(b.source_entity_type)=upper('Equipment');

DELETE FROM ATT_DETAILS_PATCHCORD a
USING temp_port_status_table b
WHERE a.SYSTEM_ID = b.source_system_id 
AND UPPER(b.source_entity_type) = UPPER('PATCHCORD');

DELETE FROM ATT_DETAILS_PATCHCORD a
USING temp_port_status_table b
WHERE a.SYSTEM_ID = b.destination_system_id 
AND UPPER(b.destination_entity_type) = UPPER('PATCHCORD');

End if;

update isp_port_info set port_status_id=p_status_id
from temp_port_status_table b
where parent_system_id=b.destination_system_id and upper(parent_entity_type)=upper(b.destination_entity_type) 
and port_number=case when b.destination_port_no<0 then (-1*b.destination_port_no) else b.destination_port_no end 
and upper(input_output)=case when b.destination_port_no<0 then 'I' else 'O' end
and upper(b.destination_entity_type)=upper('Equipment')
and  ((b.source_port_no<0 and b.destination_port_no<0) or (b.source_port_no>0 and b.destination_port_no>0));

   
END;
$BODY$;




CREATE OR REPLACE FUNCTION public.fn_trg_update_core_port_status()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$

Declare v_is_source_cpe_entity bool;
v_is_destination_cpe_entity bool;
v_display_name character varying;
v_layer_table character varying;
v_display_value character varying;
v_line_source_geometry geometry;
v_line_destination_geometry geometry;
v_source_point_geom geometry;
v_point_destination_geom geometry;
v_line_new_geom geometry;

BEGIN
v_is_source_cpe_entity:=false;
v_is_destination_cpe_entity:=false;

IF (TG_OP = 'INSERT' and coalesce(new.fat_process_id,0)=0) 
THEN
-- select clable.display_column_name,ld.layer_table into v_display_name,v_layer_table from display_name_settings clable
-- inner join layer_details ld on clable.layer_id=ld.layer_id where upper(ld.layer_name)=upper(new.source_entity_type);
-- if(coalesce(v_display_name,'')!='')
-- then
-- execute 'select '||v_display_name||' from '||v_layer_table||' where system_id='||new.source_system_id||'' into v_display_value; 
-- update connection_info set source_display_name=v_display_value where connection_id=new.connection_id;
-- end if; 
-- select clable.display_column_name,ld.layer_table into v_display_name,v_layer_table from display_name_settings clable
-- inner join layer_details ld on clable.layer_id=ld.layer_id where upper(ld.layer_name)=upper(new.destination_entity_type);
-- if(coalesce(v_display_name,'')!='')
-- then
-- execute 'select '||v_display_name||' from '||v_layer_table||' where system_id='||new.destination_system_id||'' into v_display_value;
-- update connection_info set destination_display_name=v_display_value where connection_id=new.connection_id; 
-- end if; 

-- select clable.display_column_name,ld.layer_table into v_display_name,v_layer_table from display_name_settings clable
-- inner join layer_details ld on clable.layer_id=ld.layer_id where upper(ld.layer_name)=upper('Splicetray');
-- if(coalesce(v_display_name,'')!='' and new.source_tray_system_id>0)
-- then
-- execute 'select '||v_display_name||' from '||v_layer_table||' where system_id='||new.source_tray_system_id||'' into v_display_value; 
-- update connection_info set source_tray_display_name=v_display_value where connection_id=new.connection_id;
-- end if;
-- select clable.display_column_name,ld.layer_table into v_display_name,v_layer_table from display_name_settings clable
-- inner join layer_details ld on clable.layer_id=ld.layer_id where upper(ld.layer_name)=upper('Splicetray');
-- if(coalesce(v_display_name,'')!='' and new.destination_tray_system_id>0)
-- then
-- execute 'select '||v_display_name||' from '||v_layer_table||' where system_id='||new.destination_tray_system_id||'' into v_display_value; 
-- update connection_info set destination_tray_display_name=v_display_value where connection_id=new.connection_id;
-- end if;

---- REGION WHEN CABLE IS NOT TERMINATED & SPLICED

-- IF((UPPER(new.destination_entity_type)='CABLE')
-- and  exists(select 1 from att_details_cable where system_id=new.destination_system_id 
-- and ((a_system_id=0 and coalesce(a_entity_type,'')='') or(b_system_id=0 and coalesce(b_entity_type,'')=''))))
-- THEN

-- EXECUTE 'SELECT sp_geometry from point_master where system_id='||new.source_system_id||' and upper(entity_type)=upper('''||new.source_entity_type||''')' 
-- into v_source_point_geom;

-- EXECUTE 'SELECT sp_geometry from line_master where system_id='||new.destination_system_id||' and upper(entity_type)=upper('''||new.destination_entity_type||''')' 
-- into v_line_source_geometry;

-- IF(new.is_cable_a_end) THEN
-- UPDATE att_details_cable set a_system_id= new.source_system_id ,a_entity_type=new.source_entity_type,a_network_id= new.source_network_id 
-- ,a_location=new.source_network_id where system_id=new.destination_system_id;

-- v_line_new_geom:=ST_SetPoint(v_line_source_geometry,0,v_source_point_geom) ;
-- if(v_line_new_geom is not null )
-- then
--     UPDATE LINE_MASTER  set SP_GEOMETRY= v_line_new_geom
--     WHERE system_id=new.destination_system_id and upper(entity_type)=upper(new.destination_entity_type);
-- end if;

-- ELSE

-- UPDATE att_details_cable set b_system_id= new.source_system_id ,
-- b_entity_type=new.source_entity_type,b_network_id= new.source_network_id 
-- ,b_location=new.source_network_id where system_id=new.destination_system_id;
 
-- v_line_new_geom:=ST_SetPoint(v_line_source_geometry,-1,v_source_point_geom) ;
-- if(v_line_new_geom is not null )
-- then
--     UPDATE LINE_MASTER  set SP_GEOMETRY= v_line_new_geom
--     WHERE system_id=new.destination_system_id and upper(entity_type)=upper(new.destination_entity_type);
-- end if;
  
-- END IF;

-- insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,
-- associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,
-- is_termination_point,entity_display_name,associated_display_name)
-- values(new.destination_system_id,new.destination_network_id,new.destination_entity_type,
-- new.source_system_id,new.source_network_id,new.source_entity_type,now(),
-- new.created_by,true,new.destination_display_name,new.source_display_name);

-- END IF;

-- IF((UPPER(new.source_entity_type)='CABLE')
-- and exists(select 1 from att_details_cable where system_id=new.source_system_id 
-- and ((a_system_id=0 and coalesce(a_entity_type,'')='') or(b_system_id=0 and coalesce(b_entity_type,'')=''))))
-- THEN

-- EXECUTE 'SELECT sp_geometry from point_master where system_id='||new.destination_system_id||' and upper(entity_type)=upper('''||new.destination_entity_type||''')' 
-- into v_point_destination_geom;

-- EXECUTE 'SELECT sp_geometry from line_master where system_id='||new.source_system_id||' and upper(entity_type)=upper('''||new.source_entity_type||''')' 
-- into v_line_destination_geometry;

--    IF(new.is_cable_a_end) 
--    THEN
   
-- UPDATE att_details_cable set a_system_id= new.destination_system_id ,a_entity_type=new.destination_entity_type,
-- a_network_id= new.destination_network_id
-- ,a_location=new.destination_network_id  
-- where system_id=new.source_system_id;

-- v_line_new_geom:=ST_SetPoint(v_line_destination_geometry,0,v_point_destination_geom) ;
-- if(v_line_new_geom is not null )
-- then
--     UPDATE LINE_MASTER  set SP_GEOMETRY= v_line_new_geom
--     WHERE system_id=new.source_system_id and upper(entity_type)=upper(new.source_entity_type);
-- end if;

--    ELSE
-- UPDATE att_details_cable set b_system_id= new.destination_system_id ,b_entity_type=new.destination_entity_type,b_network_id= new.destination_network_id
-- ,b_location=new.destination_network_id where system_id=new.source_system_id;

-- v_line_new_geom:=ST_SetPoint(v_line_destination_geometry,-1,v_point_destination_geom) ;
-- if(v_line_new_geom is not null )
-- then
--     UPDATE LINE_MASTER  set SP_GEOMETRY= v_line_new_geom
--     WHERE system_id=new.source_system_id and upper(entity_type)=upper(new.source_entity_type);
-- end if;

-- END IF;

-- insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,
-- associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,
-- is_termination_point,entity_display_name,associated_display_name)
-- values(new.source_system_id,new.source_network_id,new.source_entity_type,
-- new.destination_system_id,new.destination_network_id,new.destination_entity_type,now(),
-- new.created_by,true,new.source_display_name,new.destination_display_name);

-- END IF;

-- IF ((concat(new.source_system_id::text,upper(new.source_entity_type::text))!=concat(new.destination_system_id::text,upper(new.destination_entity_type::text)))) 
-- THEN
-- select is_cpe_entity into v_is_source_cpe_entity from layer_details where upper(layer_name)=upper(new.source_entity_type);
-- select is_cpe_entity into v_is_destination_cpe_entity from layer_details where upper(layer_name)=upper(new.destination_entity_type);
-- ------------UPDATE THE CABLE CORE STATUS---------
--   if(upper(new.source_entity_type)='CABLE' and new.is_cable_a_end=true)
--   then
--    if(new.is_through_connection) then
--        update att_details_cable_info  set is_a_end_through_connectivity=true 
--        where cable_id=new.source_system_id    and fiber_number=new.source_port_no;
--   end if;

--update att_details_cable_info  set a_end_status_id=2 where cable_id=new.source_system_id and fiber_number=new.source_port_no;
--update isp_port_info  set port_status_id=2   where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) 
--and port_number=case when new.destination_port_no<0 then (-1*new.destination_port_no) 
--else new.destination_port_no end  and input_output=case when new.destination_port_no<0 then 'I' else 'O' end;   
--   elsif(upper(new.source_entity_type)='CABLE' and new.is_cable_a_end=false)
--   then
  
--   if(new.is_through_connection) then
--   update att_details_cable_info set is_b_end_through_connectivity=true where cable_id=new.source_system_id and fiber_number=new.source_port_no; 
--  end if;

--update att_details_cable_info set b_end_status_id=2 where cable_id=new.source_system_id and fiber_number=new.source_port_no; 
--update isp_port_info  set port_status_id=2   where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) and port_number=case when new.destination_port_no<0 then (-1*new.destination_port_no) else new.destination_port_no end and input_output=case when new.destination_port_no<0 then 'I' else 'O' end;   
--   elsif(upper(new.destination_entity_type)='CABLE' and new.is_cable_a_end=true)
--   then
  
--   if(new.is_through_connection) then
--      update att_details_cable_info  set is_a_end_through_connectivity=true 
--      where cable_id=new.destination_system_id 
--      and fiber_number=new.destination_port_no;
--   end if;
 
--update att_details_cable_info  set a_end_status_id=2 where cable_id=new.destination_system_id and fiber_number=new.destination_port_no;
--update isp_port_info  set port_status_id=2   where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end 
--and input_output=case when new.source_port_no<0 then 'I' else 'O' end;   
--   elsif(upper(new.destination_entity_type)='CABLE' and new.is_cable_a_end=false)
--   then
--     if(new.is_through_connection) then
--        update att_details_cable_info set is_b_end_through_connectivity=true 
--       where cable_id=new.destination_system_id and fiber_number=new.destination_port_no;
--  end if;

--update att_details_cable_info set b_end_status_id=2 where cable_id=new.destination_system_id and fiber_number=new.destination_port_no;
--update isp_port_info  set port_status_id=2   where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end and input_output=case when new.source_port_no<0 then 'I' else 'O' end;   
--  end if;     

 
------------UPDATE THE SPLITTER PORT STATUS---------
-- if(upper(new.source_entity_type)='SPLITTER' or upper(new.source_entity_type)='FMS' or v_is_source_cpe_entity)
--   then
  
-- update isp_port_info  set port_status_id=2   where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) 
-- and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end and input_output=case when new.source_port_no<0 then 'I' else 'O' end; 
 
-- elsif(upper(new.destination_entity_type)='SPLITTER' or upper(new.destination_entity_type)='FMS' or v_is_destination_cpe_entity)
-- then
-- update isp_port_info  set port_status_id=2   where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) 
-- and port_number=case when new.destination_port_no<0 then (-1*new.destination_port_no) 
-- else new.destination_port_no end and input_output=case when new.destination_port_no<0 then 'I' else 'O' end;   
-- end if;

------------UPDATE THE CUSTOMER STATUS---------
-- if(upper(new.source_entity_type)='CUSTOMER')
--   then
-- update att_details_customer  set customer_status_id=2   where system_id=new.source_system_id;   
-- elsif(upper(new.destination_entity_type)='CUSTOMER')
-- then
-- update att_details_customer  set customer_status_id=2   where system_id=new.destination_system_id;   
-- end if;

END IF;
-- ----------------------------UPDATE THE EQUIPMENT PORT STATUS---------------------------------
-- 

-- IF( upper(new.source_entity_type)=upper('Equipment') 
-- and EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(NEW.SOURCE_NETWORK_ID)) and upper(new.splicing_source)='EQUIPMENT_SPLICING')
-- THEN
-- update isp_port_info set port_status_id=2 
-- where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) 
-- and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end 
-- and upper(input_output) in ('I','O');
-- END IF;

-- IF ( upper(new.source_entity_type)=upper('Equipment') 
-- and ((new.source_port_no<0 and new.destination_port_no<0) or (new.source_port_no>0 and new.destination_port_no>0)))
-- then
-- update isp_port_info set port_status_id=2 
-- where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) 
-- and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end 
-- and upper(input_output)=case when new.source_port_no<0 then 'I' else 'O' end; 
-- end if;

-- IF (upper(new.destination_entity_type)=upper('Equipment')) and ((new.source_port_no<0 and new.destination_port_no<0) or (new.source_port_no>0 and new.destination_port_no>0))
-- then 
-- update isp_port_info set port_status_id=2 
-- where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) 
-- and port_number= case when new.destination_port_no<0 then (-1*new.destination_port_no) else new.destination_port_no end 
-- and upper(input_output)=case when new.destination_port_no<0 then 'I' else 'O' end; 
-- end if;

-- IF ( upper(new.destination_entity_type)=upper('Equipment') 
-- and EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(NEW.DESTINATION_NETWORK_ID)) and upper(new.splicing_source)='EQUIPMENT_SPLICING')
-- then 
-- update isp_port_info set port_status_id=2 
-- where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) 
-- and port_number= case when new.destination_port_no<0 then (-1*new.destination_port_no) else new.destination_port_no end 
-- and upper(input_output)in ('I','O');
-- end if;

--end if;

-- IF (TG_OP = 'DELETE' and coalesce(old.fat_process_id,0)=0) 
-- THEN  
-- select is_cpe_entity into v_is_source_cpe_entity from layer_details where upper(layer_name)=upper(old.source_entity_type);
-- select is_cpe_entity into v_is_destination_cpe_entity from layer_details where upper(layer_name)=upper(old.destination_entity_type);
-- --DELETE THE PATCH CORD IF SOURCE ENTITY TYPE IS PATCH CORD
-- IF(upper(old.source_entity_type)=UPPER('PATCHCORD'))
-- THEN
-- DELETE FROM ATT_DETAILS_PATCHCORD WHERE SYSTEM_ID=old.source_system_id;
-- ELSIF(upper(old.destination_entity_type)=UPPER('PATCHCORD'))
-- THEN
-- DELETE FROM ATT_DETAILS_PATCHCORD WHERE SYSTEM_ID=old.destination_system_id;
-- END IF;

--------UPDATE THE CABLE CORE STATUS---------
-- if(upper(old.source_entity_type)='CABLE' and old.is_cable_a_end=true)
--    then
-- update att_details_cable_info  set a_end_status_id=1,is_a_end_through_connectivity=false   where cable_id=old.source_system_id and fiber_number=old.source_port_no;
-- update isp_port_info  set port_status_id=1   where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) and port_number=case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end  and input_output=case when old.destination_port_no<0 then 'I' else 'O' end;   
--    elsif(upper(old.source_entity_type)='CABLE' and old.is_cable_a_end=false)
--    then
-- update att_details_cable_info set b_end_status_id=1,is_b_end_through_connectivity=false where cable_id=old.source_system_id and fiber_number=old.source_port_no;
-- update isp_port_info  set port_status_id=1   where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) and port_number=case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end  and input_output=case when old.destination_port_no<0 then 'I' else 'O' end;   
--    elsif(upper(old.destination_entity_type)='CABLE' and old.is_cable_a_end=true)
--    then
-- update att_details_cable_info  set a_end_status_id=1 ,is_a_end_through_connectivity=false  where cable_id=old.destination_system_id and fiber_number=old.destination_port_no;
-- update isp_port_info  set port_status_id=1   where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) and port_number=case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end and input_output=case when old.source_port_no<0 then 'I' else 'O' end;   
--    elsif(upper(old.destination_entity_type)='CABLE' and old.is_cable_a_end=false)
--    then
-- update att_details_cable_info set b_end_status_id=1,is_b_end_through_connectivity=false where cable_id=old.destination_system_id and fiber_number=old.destination_port_no;
-- update isp_port_info  set port_status_id=1   where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) and port_number=case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end and input_output=case when old.source_port_no<0 then 'I' else 'O' end;   
--    end if;

------------UPDATE THE SPLITTER PORT STATUS---------
-- if(upper(old.source_entity_type)='SPLITTER' or upper(old.source_entity_type)='FMS' or v_is_source_cpe_entity)
--    then
-- update isp_port_info  set port_status_id=1   where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) and port_number=case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end and input_output=case when old.source_port_no<0 then 'I' else 'O' end;   
--    elsif(upper(old.destination_entity_type)='SPLITTER' or upper(old.destination_entity_type)='FMS' or v_is_destination_cpe_entity)
--    then
-- update isp_port_info  set port_status_id=1   where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) and port_number=case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end and input_output=case when old.destination_port_no<0 then 'I' else 'O' end;    
--    end if;

   ------------UPDATE THE CUSTOMER STATUS---------
-- if(upper(old.source_entity_type)='CUSTOMER')
-- then
-- update att_details_customer  set customer_status_id=1   where system_id=old.source_system_id;   
-- elsif(upper(old.destination_entity_type)='CUSTOMER')
-- then
-- update att_details_customer  set customer_status_id=1   where system_id=old.destination_system_id;   
-- end if;

   

-- IF ( upper(old.source_entity_type)=upper('Equipment') 
-- and EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(old.SOURCE_NETWORK_ID)) and upper(old.splicing_source)='EQUIPMENT_SPLICING')
-- then
-- update isp_port_info set port_status_id=1 
-- where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) 
-- and port_number=(case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end) 
-- and upper(input_output)in ('I','O'); 
-- end if; 

-- IF ( upper(old.destination_entity_type)=upper('Equipment')
-- and EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(old.DESTINATION_NETWORK_ID)) and upper(old.splicing_source)='EQUIPMENT_SPLICING')
-- then 
-- update isp_port_info set port_status_id=1 
-- where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) 
-- and port_number=(case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end) 
-- and upper(input_output)in ('I','O');
-- end if; 

-- IF ( upper(old.source_entity_type)=upper('Equipment')) 
-- and ((old.source_port_no<0 and old.destination_port_no<0) or (old.source_port_no>0))
-- then
-- update isp_port_info set port_status_id=1   where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) 
-- and port_number=(case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end) and upper(input_output)=case when old.source_port_no<0 then 'I' else 'O' end; 
-- end if; 

-- IF ( upper(old.destination_entity_type)=upper('Equipment'))
-- and ((old.source_port_no<0 and old.destination_port_no<0) or (old.destination_port_no>0))
-- then 
-- update isp_port_info set port_status_id=1   where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) 
-- and port_number=(case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end) and upper(input_output)=case when old.destination_port_no<0 then 'I' else 'O' end; 
-- end if; 

--END IF; 

RETURN NEW;
END;
$BODY$;




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
  created_by integer,
  splicing_source character varying(100)
) ON COMMIT DROP ;

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

insert into temp_port_status_table (
  source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
    destination_network_id,destination_entity_type,destination_port_no,is_source_cable_a_end,is_destination_cable_a_end,equipment_system_id,equipment_network_id,
    equipment_entity_type,created_by,splicing_source
) select source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,
destination_network_id,destination_entity_type,destination_port_no,coalesce(is_source_cable_a_end,false) as is_source_cable_a_end,coalesce(is_destination_cable_a_end,false) as is_destination_cable_a_end,equipment_system_id,equipment_network_id,
equipment_entity_type,created_by, coalesce(splicing_source,'') as splicing_source from temp_connection; 

Perform(fn_update_port_status_bulk_splicing(2));

	
return query
select row_to_json(row) from (
select true as status, 'success'::character varying as message 
 ) row;
 
END
$BODY$;


CREATE OR REPLACE FUNCTION public.fn_splicing_snap_cable_end_points(
	p_source_entity_type character varying,
	p_destination_entity_type character varying,
	p_equipment_entity_type character varying,
	p_source_system_id integer,
	p_destination_system_id integer,
	p_equipment_system_id integer,
	p_destination_cable_a_end boolean,
	p_source_cable_a_end boolean,
	p_created_by integer)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
Declare v_source_point_geom geometry;
v_line_source_geometry geometry;
v_line_new_geom geometry;
v_line_destination_geometry geometry;
v_point_destination_geom geometry;
v_source_network_id character varying;
v_destination_network_id character varying;
v_equipment_network_id character varying;
v_equipments_network_id character varying;
v_destination_display_name character varying;
v_source_display_name character varying;
BEGIN
---- REGION WHEN CABLE IS NOT TERMINATED & SPLICED

IF((UPPER(p_destination_entity_type)='CABLE')
and  exists(select 1 from att_details_cable where system_id=p_destination_system_id 
and ((a_system_id=0 and coalesce(a_entity_type,'')='') or(b_system_id=0 and coalesce(b_entity_type,'')=''))))
THEN
Execute 'Select network_id from att_details_cable where system_id='||p_destination_system_id||' ' into  v_destination_network_id;

EXECUTE 'SELECT sp_geometry,common_name,display_name from point_master where system_id='||p_equipment_system_id||' and upper(entity_type)=upper('''||p_equipment_entity_type||''')' 
into v_source_point_geom,v_equipment_network_id,v_source_display_name ;

EXECUTE 'SELECT sp_geometry,display_name from line_master where system_id='||p_destination_system_id||' and upper(entity_type)=upper('''||p_destination_entity_type||''')' 
into v_line_source_geometry,v_destination_display_name;

IF(p_destination_cable_a_end) THEN
UPDATE att_details_cable set a_system_id= p_equipment_system_id ,a_entity_type=p_equipment_entity_type,a_network_id= v_equipment_network_id 
,a_location=v_equipment_network_id where system_id=p_destination_system_id;

v_line_new_geom:=ST_SetPoint(v_line_source_geometry,0,v_source_point_geom) ;
if(v_line_new_geom is not null )
then
    UPDATE LINE_MASTER  set SP_GEOMETRY= v_line_new_geom
    WHERE system_id=p_destination_system_id and upper(entity_type)=upper(p_destination_entity_type);
end if;

ELSE

UPDATE att_details_cable set b_system_id= p_equipment_system_id ,
b_entity_type=p_equipment_entity_type,b_network_id= v_equipment_network_id 
,b_location=v_equipment_network_id where system_id=p_destination_system_id;
 
v_line_new_geom:=ST_SetPoint(v_line_source_geometry,-1,v_source_point_geom) ;
if(v_line_new_geom is not null )
then
    UPDATE LINE_MASTER  set SP_GEOMETRY= v_line_new_geom
    WHERE system_id=p_destination_system_id and upper(entity_type)=upper(p_destination_entity_type);
end if;
  
END IF;

insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,
associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,
is_termination_point,entity_display_name,associated_display_name)
values(p_destination_system_id,v_destination_network_id,p_destination_entity_type,
p_equipment_system_id,v_equipment_network_id,p_equipment_entity_type,now(),
p_created_by,true,v_destination_display_name,v_source_display_name);

END IF;    

IF((UPPER(p_source_entity_type)='CABLE')
and exists(select 1 from att_details_cable where system_id=p_source_system_id 
and ((a_system_id=0 and coalesce(a_entity_type,'')='') or(b_system_id=0 and coalesce(b_entity_type,'')=''))))
THEN

Execute 'Select network_id from att_details_cable where system_id='||p_source_system_id||' ' into v_source_network_id;

EXECUTE 'SELECT sp_geometry,common_name,display_name from point_master where system_id='||p_equipment_system_id||' and upper(entity_type)=upper('''||p_equipment_entity_type||''')' 
into v_point_destination_geom,v_equipments_network_id,v_destination_display_name;

EXECUTE 'SELECT sp_geometry,display_name from line_master where system_id='||p_source_system_id||' and upper(entity_type)=upper('''||p_source_entity_type||''')' 
into v_line_destination_geometry,v_source_display_name;

   IF(p_source_cable_a_end) 
   THEN
   
UPDATE att_details_cable 
set a_system_id= p_equipment_system_id ,
a_entity_type=p_equipment_entity_type,
a_network_id= v_equipments_network_id, 
a_location=v_equipments_network_id  
where system_id=p_source_system_id;

v_line_new_geom:=ST_SetPoint(v_line_destination_geometry,0,v_point_destination_geom) ;
if(v_line_new_geom is not null )
then
    UPDATE LINE_MASTER  set SP_GEOMETRY= v_line_new_geom
    WHERE system_id=p_source_system_id and upper(entity_type)=upper(p_source_entity_type);
end if;

   ELSE
UPDATE att_details_cable set b_system_id= p_equipment_system_id ,b_entity_type=p_equipment_entity_type,b_network_id= v_equipments_network_id
,b_location=v_equipments_network_id where system_id=p_source_system_id;

v_line_new_geom:=ST_SetPoint(v_line_destination_geometry,-1,v_point_destination_geom) ;
if(v_line_new_geom is not null )
then
    UPDATE LINE_MASTER  set SP_GEOMETRY= v_line_new_geom
    WHERE system_id=p_source_system_id and upper(entity_type)=upper(p_source_entity_type);
end if;

END IF;

insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,
associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,
is_termination_point,entity_display_name,associated_display_name)
values(p_source_system_id,v_source_network_id,p_source_entity_type,
p_equipment_system_id,v_equipments_network_id,p_equipment_entity_type,now(),
p_created_by,true,v_source_display_name,v_destination_display_name);

END IF;
    
    
END;
$BODY$;


	