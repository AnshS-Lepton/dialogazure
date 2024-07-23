
-- Select * from fn_api_get_fault_location_detail('dszrefdsxc','GGN-P000041-FAT0001','',1,300);

CREATE OR REPLACE FUNCTION public.fn_api_get_fault_location_detail(
    p_fiber_link_id character varying,
	p_equipment_id  character varying,
	p_site_code character varying,
	p_entity_port_no integer,
	p_distance double precision)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
declare 
v_main_path geometry;
v_sub_line_start_point geometry;
v_sub_line_end_point geometry;
v_sub_line_geom geometry;
v_line_total_length double precision;
v_faulty_record record;
v_last_termination_point record;
v_input_cable_end_point geometry;
v_main_fiber_end_point geometry;
v_test geometry;
v_arow record;
v_main_path_geom text;
results text;
affected_cable_segment text;
associated_fiber_links text;
status text;
error_message text;
v_node_type character varying;
v_is_backword_path boolean;
	p_entity_type character varying;
	p_is_a_end boolean;
	p_is_backword_path character varying;
	p_action_code integer;
	p_entity_system_id integer;
	p_network_id character varying;
	p_fiber_cable_id integer;

BEGIN
v_last_termination_point:=null;
status:= 'Ok';
error_message:=null;
p_is_a_end:=true;
p_is_backword_path='DownStream';
p_action_code:=1001;
IF UPPER(p_is_backword_path)='UPSTREAM'
THEN
v_is_backword_path:= true;
ELSE 
v_is_backword_path:= false;
END IF;

Select entity_system_id, entity_network_id, entity_type into p_entity_system_id, p_network_id, p_entity_type  From associate_entity_info 
where associated_network_id = p_equipment_id and entity_type ='Cable' limit 1;

IF(coalesce(p_entity_system_id, 0) != 0 ) THEN

 SELECT ADB.CABLE_ID INTO p_fiber_cable_id FROM ATT_DETAILS_FIBER_LINK ATT
 INNER JOIN ATT_DETAILS_CABLE_INFO ADB ON ADB.LINK_SYSTEM_ID = ATT.SYSTEM_ID WHERE LINK_ID = p_fiber_link_id LIMIT 1;

IF(coalesce(p_fiber_cable_id, 0) != 0 ) THEN

IF(p_entity_system_id = p_fiber_cable_id ) THEN

IF(p_network_id!='') THEN
	Select p.system_id into p_entity_system_id from (
	select system_id from point_master where common_name=p_network_id AND UPPER(entity_type)=UPPER(p_entity_type)
	UNION
	select system_id from LINE_master where common_name=p_network_id AND UPPER(entity_type)=UPPER(p_entity_type)) p;
END IF;


	IF (p_is_a_end = true)
	THEN v_node_type:='A';
	ELSE
	    v_node_type:='B';
	END IF;	
		
		--TEMP TABLE TO STORE CONNECTION INFO RESULT..
		create temp table final_table
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
		cable_measured_length double precision,
		is_backward_path boolean default false		
		) on commit drop;

		create temp table temp_fibers
		(
		id serial,		
		cable_id integer,	
		sp_geometry text,
		network_id character varying(100),
		port_no integer	
		) on commit drop;
		
		insert into final_table(source_system_id,source_network_id,source_entity_type,source_port_no,destination_system_id,destination_network_id,destination_entity_type,destination_port_no,cable_measured_length,is_backward_path)
		select  a.source_system_id,a.source_network_id,a.source_entity_type,a.source_port_no,a.destination_system_id,a.destination_network_id,a.destination_entity_type,a.destination_port_no,a.cable_measured_length,a.is_backward_path
		from fn_otdr_get_fiber_list(p_entity_system_id, p_entity_port_no,p_entity_type,v_node_type) a;

		-- Dropping table because same cpf_temp used in 2 functions: fn_otdr_get_fiber_list, fn_get_connection_info
		drop table cpf_temp;

		
		insert into temp_fibers(cable_id,sp_geometry,network_id,port_no)
		select source_system_id,lm.sp_geometry,network_id,port_no from(
		select id, source_system_id,source_network_id as network_id,source_port_no as port_no from final_table cpf where upper(cpf.source_entity_type)=upper('CABLE') and is_backward_path=v_is_backword_path
		and case when v_is_backword_path=true  and upper(p_entity_type)=upper('CABLE') and coalesce(v_node_type,'')='A' then source_system_id!=p_entity_system_id 
		when v_is_backword_path=false  and upper(p_entity_type)=upper('CABLE') and coalesce(v_node_type,'')='B' then source_system_id!=p_entity_system_id  else 1=1 end
		union
		select id, destination_system_id,destination_network_id as network_id,destination_port_no as port_no from final_table cpf where upper(cpf.destination_entity_type)=upper('CABLE') and is_backward_path=v_is_backword_path 
		and case when v_is_backword_path=true  and upper(p_entity_type)=upper('CABLE') and coalesce(v_node_type,'')='A' then destination_system_id!=p_entity_system_id 
		when v_is_backword_path=false  and upper(p_entity_type)=upper('CABLE') and coalesce(v_node_type,'')='B' then destination_system_id!=p_entity_system_id  else 1=1 end
		)a inner join line_master lm on a.source_system_id=lm.system_id and upper(lm.entity_type)=upper('CABLE') order by a.id ;

	DELETE FROM temp_fibers T1 USING   temp_fibers T2
	WHERE   T1.id < T2.id and t1.cable_id=t2.cable_id;

	select  st_astext(ST_LineMerge(ST_Collect(sp_geometry))) 
	into v_main_path_geom from temp_fibers;

	RAISE INFO '%v_main_path', v_main_path;

		IF(UPPER(LEFT(v_main_path_geom,5)) = 'MULTI')
		THEN

			for v_arow in select * from temp_fibers
			loop
				if(v_main_path is null)
				then
					v_main_path:=v_arow.sp_geometry;
				else
					v_main_path:=ST_MakeLine(v_main_path,v_arow.sp_geometry);
				end if;	
			end loop;
		else
			select  ST_LineMerge(ST_Collect(sp_geometry)) into v_main_path from temp_fibers;
		END IF;

		
		select Cast(st_length(v_main_path,false)as decimal(10,2)) into v_line_total_length;
		

		if(upper(p_entity_type)=upper('CABLE'))
		then
			if(v_node_type='B')
			then
				select st_endpoint(sp_geometry) into v_input_cable_end_point from line_master where system_id=p_entity_system_id and upper(entity_type)=upper('CABLE');
				v_main_fiber_end_point:=st_endpoint(v_main_path);
				if(Cast(st_distance(v_main_fiber_end_point,v_input_cable_end_point,false)as decimal(10,2))=0)
				then
					v_main_path:=st_reverse(v_main_path);
				end if;
			elsif(v_node_type='A')
			then
				select st_startpoint(sp_geometry) into v_input_cable_end_point from line_master where system_id=p_entity_system_id and upper(entity_type)=upper('CABLE');
				v_main_fiber_end_point:=st_startpoint(v_main_path);
				if(Cast(st_distance(v_main_fiber_end_point,v_input_cable_end_point,false)as decimal(10,2))>0)
				then
					v_main_path:=st_reverse(v_main_path);
				end if;			
			end if;	
		end if;

		select st_startpoint(v_main_path) into v_sub_line_start_point;
		select ST_LineInterpolatePoint(v_main_path, p_distance /v_line_total_length) into v_sub_line_end_point;
		select ST_Line_Substring(v_main_path, ST_Line_Locate_Point(v_main_path, v_sub_line_start_point), ST_Line_Locate_Point(v_main_path, v_sub_line_end_point)) into v_sub_line_geom;


		select * into v_faulty_record from temp_fibers where ST_Contains(ST_Snap(sp_geometry, st_endpoint(v_sub_line_geom), 1e-14), st_endpoint(v_sub_line_geom));		
					
		

		IF v_faulty_record.cable_id is not null
		THEN
			return query select  row_to_json(x) from (
			select json_build_object('LATITUDE',cast(st_y(v_sub_line_end_point)as decimal(10,6)),'LONGITUDE',cast(st_x(v_sub_line_end_point)as decimal(10,6)),
		    'BLOCK_CODE', SPLIT_PART(PB.PROVINCE_ABBREVIATION, '/', 3),
        	'BLOCK_NAME', PB.PROVINCE_NAME ,   'PROVINCE_CODE', SPLIT_PART(PB.PROVINCE_ABBREVIATION, '/', 2),
        	'PROVINCE_NAME', RB.REGION_NAME , 'REGION_CODE',SPLIT_PART(PB.PROVINCE_ABBREVIATION, '/', 1) ,'REGION_NAME',RB.COUNTRY_NAME)
	  		AS fault_Location, 
			
			json_build_object('cable_system_id',CB.system_id ,'cable_network_id',CB.network_id,
			'cable_length', CB.cable_calculated_length,
	       'a_end_entity_type', CB.a_entity_type,   'a_end_entity_id', CB.a_network_id,
	       'z_end_entity_type', CB.b_entity_type , 'z_end_entity_id',CB.b_network_id ,'a_end_distance',Cast(st_distance(v_sub_line_end_point,v_sub_line_start_point,false)as decimal(10,2)),'z_end_distance',Cast(st_distance(st_endpoint(v_main_path) ,v_sub_line_end_point,false)as decimal(10,2)))
			AS affected_cable_segment, 
            ADFL.NETWORK_ID AS FIBER_LINK_NETWORK_ID,  ADFL.LINK_NAME AS FIBER_LINK_NAME
            From ATT_DETAILS_CABLE CB 
            INNER JOIN PROVINCE_BOUNDARY PB ON CB.PROVINCE_ID = PB.ID 
            INNER JOIN REGION_BOUNDARY RB ON CB.REGION_ID = RB.ID
		    LEFT JOIN ATT_DETAILS_CABLE_INFO ADCI ON ADCI.CABLE_ID = CB.SYSTEM_ID
            LEFT JOIN ATT_DETAILS_FIBER_LINK ADFL ON ADCI.LINK_SYSTEM_ID = ADFL.SYSTEM_ID 
            WHERE CB.SYSTEM_ID= v_faulty_record.cable_id
             )x; --into results;		
		ELSE
				return query select row_to_json(x)
	            from (select 'Failed' AS status, 'No data found!' AS error_message)x;	
		END IF;
		ELSE
	    return query select row_to_json(x)
	    from (select 'Failed' AS status, 'Cable not match with fiber_link_id and equipment_id!!' AS error_message)x;	
	    END IF;
		ELSE

 return query select row_to_json(x)
	            from (select 'Failed' AS status, 'Cable not found again Fiber link Id!' AS error_message)x;
END IF;	

ELSE

 return query select row_to_json(x)
	            from (select 'Failed' AS status, 'Cable not found again Equipment!' AS error_message)x;
END IF;	
	
		
END; 
$BODY$;


