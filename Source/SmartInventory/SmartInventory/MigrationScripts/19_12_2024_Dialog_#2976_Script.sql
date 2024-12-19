-- DROP FUNCTION public.fn_splicing_save_osp_split_connection(int4, int4, int4, int4, varchar, varchar, int4, varchar);

CREATE OR REPLACE FUNCTION public.fn_splicing_save_osp_split_connection(p_cable_one_system__id integer, p_cable_two_system_id integer, p_old_cable_system_id integer, p_split_entity_system_id integer, p_split_entity_network_id character varying, p_split_entity_type character varying, p_user_id integer, p_splicing_source character varying)
 RETURNS boolean
 LANGUAGE plpgsql
AS $function$
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
			

--end if;
    return true;
end;
$function$
;