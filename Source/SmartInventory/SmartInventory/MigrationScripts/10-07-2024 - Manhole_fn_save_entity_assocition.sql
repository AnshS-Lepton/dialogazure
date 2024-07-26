-- FUNCTION: public.fn_save_entity_assocition(character varying, integer, character varying, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_save_entity_assocition(character varying, integer, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_save_entity_assocition(
	p_line_associate_info character varying,
	p_parent_system_id integer,
	p_parent_entity_type character varying,
	p_user_id integer,
	p_manhole_count integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

 
DECLARE 
 curgeommapping refcursor;  
 v_system_id integer;
 v_network_id character varying;
 v_entity_type character varying;
 v_parent_netwok_id character varying;
 v_is_associated bool;
 v_line_associate_info json;
 v_snapped_geom geometry;
 v_point_geom geometry;
 v_CableExtraLengthPercentage integer;
 v_cablemeasured_length double precision;
 v_total_loop_length integer;

 v_start_point geometry;
 v_end_point geometry;
 v_total_points integer;
 v_old_measured_length double precision;
 v_new_measured_length double precision;
 v_old_calculated_length double precision;

 v_old_line_geom geometry;
 v_parent_display_name character varying;
 v_parent_display_value character varying;
 v_layer_table character varying;
 v_display_value character varying;
 v_associated_system_id integer;
 v_associated_entity_type character varying;
 b_arow record;
 V_BROW record;
 v_is_geomtryedit_allowed integer;
BEGIN

create temp table tmp_association
(
system_id integer,
entity_type character varying,
entity_network_id character varying,
is_associated bool
) on commit drop;

select replace(p_line_associate_info,'\','') into v_line_associate_info;
EXECUTE 'SELECT network_id  from att_details_'||p_parent_entity_type||' where system_id='||p_parent_system_id||'' into v_parent_netwok_id;
OPEN curgeommapping FOR select system_id,entity_network_id,entity_type,is_associated  from json_populate_recordset(null::tmp_association,v_line_associate_info);
	LOOP
		FETCH  curgeommapping into v_system_id,v_network_id,v_entity_type,v_is_associated;
		-- EXIT FOR LOOP
		IF NOT FOUND THEN
		EXIT;
		END IF;

	if(v_is_associated=true)
	then
		raise info '%11',11;
-- 		raise info '%v_system_id',v_system_id;
-- 		raise info '%v_entity_type',v_entity_type;
-- 		raise info '%p_parent_system_id',p_parent_system_id;
-- 		raise info '%p_parent_entity_type',p_parent_entity_type;
-- 		
		if(not exists(select entity_system_id from associate_entity_info 
		where (associated_system_id=v_system_id and upper(associated_entity_type)=upper(v_entity_type) and entity_system_id=p_parent_system_id and 
		upper(entity_type)=upper(p_parent_entity_type))
		or (entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and associated_system_id=p_parent_system_id and 
		upper(associated_entity_type)=upper(p_parent_entity_type))))
		then
			raise info '%12',12;
			v_is_geomtryedit_allowed=(select (coalesce((select value::integer from global_settings where key='IsGeometryUpdateOnAssociationAllowed'),0))::integer);
			
			insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,entity_display_name,associated_system_id,associated_network_id,
			associated_entity_type,associated_display_name,created_on,created_by) 
			values(p_parent_system_id,v_parent_netwok_id,p_parent_entity_type,fn_get_display_name(p_parent_system_id,p_parent_entity_type),v_system_id,
			v_network_id,v_entity_type,fn_get_display_name(v_system_id,v_entity_type),now(),p_user_id);

			

					
			if((upper(p_parent_entity_type)='CABLE' or lower(p_parent_entity_type)='duct' or lower(p_parent_entity_type)='trench'))
			then			
				raise info '%13',13;
				--select LM.sp_geometry into v_old_line_geom from line_master LM where WHERE LM.system_id = p_parent_system_id 
				--and lower(entity_type)=lower(p_parent_entity_type);

				if(upper(p_parent_entity_type)='TRENCH' and upper(v_entity_type)='DUCT' and v_is_geomtryedit_allowed=1)
					then
						update att_details_duct set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
						where system_id=v_system_id;
						update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Trench' and system_id=p_parent_system_id)
						where entity_type=v_entity_type and system_id=v_system_id;
						delete from associate_entity_info where entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and 
						UPPER(ASSOCIATED_ENTITY_TYPE) NOT IN ('CABLE','DUCT');
						perform(fn_geojson_update_entity_attribute(v_system_id,v_entity_type,0,1,true));

						FOR V_BROW IN SELECT * FROM ASSOCIATE_ENTITY_INFO WHERE ENTITY_SYSTEM_ID=v_system_id AND UPPER(ENTITY_TYPE)=UPPER(v_entity_type) and 
						UPPER(ASSOCIATED_ENTITY_TYPE)='CABLE'
						LOOP

							update att_details_cable set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
							
							where system_id=V_BROW.associated_system_id;
							update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Duct' and system_id=v_system_id)
							where entity_type=V_BROW.associated_entity_type and system_id=V_BROW.associated_system_id;
							delete from associate_entity_info where entity_system_id=V_BROW.associated_system_id and upper(entity_type)=
							upper(V_BROW.associated_entity_type) and UPPER(ASSOCIATED_ENTITY_TYPE) NOT IN ('CABLE','DUCT');
							
							delete from connection_info where (upper(source_entity_type)=upper(V_BROW.associated_entity_type) and 
							source_system_id=V_BROW.associated_system_id) or (upper(destination_entity_type)=upper(V_BROW.associated_entity_type) and 
							destination_system_id=V_BROW.associated_system_id);
							perform(fn_geojson_update_entity_attribute(V_BROW.associated_system_id,V_BROW.associated_entity_type,0,1,true));
						END LOOP;
				end if;
				if(upper(p_parent_entity_type)='DUCT' and upper(v_entity_type)='CABLE' and v_is_geomtryedit_allowed=1)
					then
						update att_details_cable set a_system_id=0,a_location=null,a_entity_type=null,b_system_id=0,b_location=null,b_entity_type=null
						where system_id=v_system_id;
						update line_master set sp_geometry=(select sp_geometry from line_master where entity_type='Duct' and system_id=p_parent_system_id)
						where entity_type=v_entity_type and system_id=v_system_id;
						delete from associate_entity_info where entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and 
						UPPER(ASSOCIATED_ENTITY_TYPE) NOT IN ('CABLE','DUCT');

						delete from connection_info where (upper(source_entity_type)=upper(v_entity_type) and source_system_id=v_system_id) or 
						(upper(destination_entity_type)=upper(v_entity_type) and destination_system_id=v_system_id);
						perform(fn_geojson_update_entity_attribute(v_system_id,v_entity_type,0,1,true));
				end if;
				
				SELECT ST_RemoveRepeatedPoints(ST_AddPoint(
				ST_AddPoint(ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01),st_startpoint(LM.SP_GEOMETRY),0)
				,st_endpoint(LM.SP_GEOMETRY),ST_NPoints(ST_AddPoint(ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01),
				st_startpoint(LM.SP_GEOMETRY),0)))) into v_snapped_geom
				FROM (SELECT SP_GEOMETRY AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = v_system_id and lower(entity_type)=lower(v_entity_type)) AS point,
				LINE_MASTER AS LM  WHERE LM.system_id = p_parent_system_id and lower(entity_type)=lower(p_parent_entity_type);

				
				if(v_snapped_geom is not null) then
					update LINE_MASTER set SP_GEOMETRY=v_snapped_geom  WHERE system_id = p_parent_system_id and lower(entity_type)=lower(p_parent_entity_type);
									
				if(upper(p_parent_entity_type)='CABLE')
				then    select value  into v_CableExtraLengthPercentage from global_settings where key='CableExtraLengthPercentage';
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER 
					WHERE system_id = p_parent_system_id and lower(entity_type)=lower(p_parent_entity_type))) into v_cablemeasured_length;
					select total_loop_length into v_total_loop_length from att_details_cable where system_id=p_parent_system_id;

					
					 
					update att_details_cable set manhole_count=p_manhole_count , cable_measured_length=v_cablemeasured_length, 
					cable_calculated_length=v_cablemeasured_length+v_total_loop_length+(((v_CableExtraLengthPercentage*v_cablemeasured_length)/100)-
					v_total_loop_length)
					where system_id=p_parent_system_id;
					
					
				end if; 

				if(upper(p_parent_entity_type)='DUCT')
				then
					select calculated_length,manual_length into v_old_measured_length,v_old_calculated_length from att_details_duct WHERE  
					system_id=p_parent_system_id; 
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER 
					WHERE  system_id=p_parent_system_id and upper(entity_type)=upper(p_parent_entity_type))) into v_new_measured_length;
					
					if(v_new_measured_length!=v_old_measured_length)
					then
					update att_details_duct set calculated_length=v_new_measured_length,
					manual_length= 
					case when coalesce(v_old_calculated_length,0)>0 and coalesce(v_new_measured_length,0)>coalesce(v_old_measured_length,0) 
					then (v_old_calculated_length+(v_new_measured_length-v_old_measured_length))
					when coalesce(v_old_calculated_length,0)>0 and coalesce(v_new_measured_length,0)<coalesce(v_old_measured_length,0) 
					then (v_old_calculated_length-(v_old_measured_length-v_new_measured_length)) end
					WHERE  system_id=p_parent_system_id;
					end if;
				end if;

				if(upper(p_parent_entity_type)='TRENCH')
				then
				raise info '%19',19;
					select trench_length into v_old_measured_length from att_details_trench WHERE  system_id=p_parent_system_id; 
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE  system_id=p_parent_system_id 
					and upper(entity_type)=upper(p_parent_entity_type))) into v_new_measured_length;

					if(v_new_measured_length!=v_old_measured_length)
					then
						update att_details_trench set 
						trench_length= 
						case 
						when coalesce(v_old_measured_length,0)>0 and coalesce(v_new_measured_length,0)>coalesce(v_old_measured_length,0) 
						then (v_old_measured_length+(v_new_measured_length-v_old_measured_length))
						when coalesce(v_old_measured_length,0)>0 and coalesce(v_new_measured_length,0)<coalesce(v_old_measured_length,0) 
						then (v_old_measured_length-(v_old_measured_length-v_new_measured_length)) end
						WHERE  system_id=p_parent_system_id;
					end if;
					
				end if;
				end if;
							
			end if;

			end if;

-- select 1 from vw_associate_entity_master 
-- where ((upper(layer_name)=upper(p_parent_entity_type) and upper(associate_layer_name)=upper(v_entity_type) and upper(associated_layer_geom_type)='LINE')
-- or (upper(associate_layer_name)=upper(p_parent_entity_type) and upper(layer_name)=upper(v_entity_type)) and upper(layer_geom_type)='LINE')						
-- and is_snapping_enabled=true
--IF(upper(p_parent_entity_type)='POLE')

			
			if exists(select 1 from vw_associate_entity_master 
			where ((upper(layer_name)=upper(p_parent_entity_type) and upper(associate_layer_name)=upper(v_entity_type) and upper(associated_layer_geom_type)='LINE')
			or (upper(associate_layer_name)=upper(p_parent_entity_type) and upper(layer_name)=upper(v_entity_type)) and upper(layer_geom_type)='LINE')						
			and is_snapping_enabled=true)
			then
				raise info '%14',14;
				SELECT ST_RemoveRepeatedPoints(ST_AddPoint(
				ST_AddPoint(ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01),st_startpoint(LM.SP_GEOMETRY),0)
				,st_endpoint(LM.SP_GEOMETRY),ST_NPoints(ST_AddPoint(ST_Snap(LM.SP_GEOMETRY, point.THE_POINT, ST_Distance(point.THE_POINT,LM.SP_GEOMETRY)*1.01),
				st_startpoint(LM.SP_GEOMETRY),0)))) into v_snapped_geom
				FROM (SELECT SP_GEOMETRY AS THE_POINT FROM POINT_MASTER as p WHERE p.system_id = p_parent_system_id 
				and lower(entity_type)=lower(p_parent_entity_type)) AS point,
				LINE_MASTER AS LM  WHERE LM.system_id = v_system_id and lower(entity_type)=lower(v_entity_type);

				raise info '%v_snapped_geom',v_snapped_geom;
				
				if(v_snapped_geom is not null) then
					update LINE_MASTER set SP_GEOMETRY=v_snapped_geom  WHERE system_id = v_system_id and lower(entity_type)=lower(v_entity_type);
				--BY ANTRA--	
				raise info '%v_system_id',v_system_id;
				raise info '%v_entity_type',v_entity_type;
				raise info '%p_parent_entity_type',p_parent_entity_type;
				
			for b_arow in	select associated_system_id,associated_entity_type  from associate_entity_info 
				where (entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) and is_termination_point=false 
				and lower(entity_type) !=lower(p_parent_entity_type))		
				union 
				select entity_system_id,entity_type from associate_entity_info where (associated_system_id=v_system_id 
				and upper(associated_entity_type)=upper(v_entity_type) and is_termination_point=false and lower(entity_type) 
				!=lower(p_parent_entity_type))
				
				LOOP

				if(v_snapped_geom is not null) then
					update LINE_MASTER set SP_GEOMETRY=v_snapped_geom  WHERE system_id = b_arow.associated_system_id
					and lower(entity_type)=lower( b_arow.associated_entity_type);
				end if;
				END LOOP;
				--END					
				if(upper(p_parent_entity_type)='CABLE')
				then    select value  into v_CableExtraLengthPercentage from global_settings where key='CableExtraLengthPercentage';
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER 
					WHERE system_id = v_system_id and lower(entity_type)=lower(v_entity_type))) into v_cablemeasured_length;
					select total_loop_length into v_total_loop_length from att_details_cable where system_id=v_system_id;					
				
					update att_details_cable set  cable_measured_length=v_cablemeasured_length, 
					cable_calculated_length=v_cablemeasured_length+v_total_loop_length+(((v_CableExtraLengthPercentage*v_cablemeasured_length)/100)-
					v_total_loop_length) where system_id=v_system_id;				
					
				end if; 

				if(upper(p_parent_entity_type)='DUCT')
				then
					select calculated_length,manual_length into v_old_measured_length,v_old_calculated_length from att_details_duct WHERE  system_id=v_system_id; 
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER 
					WHERE  system_id=v_system_id and upper(entity_type)=upper(v_entity_type))) into v_new_measured_length;
					
					if(v_new_measured_length!=v_old_measured_length)
					then
					update att_details_duct set calculated_length=v_new_measured_length,
					manual_length= 
					case when coalesce(v_old_calculated_length,0)>0 and coalesce(v_new_measured_length,0)>coalesce(v_old_measured_length,0) 
					then (v_old_calculated_length+(v_new_measured_length-v_old_measured_length))
					when coalesce(v_old_calculated_length,0)>0 and coalesce(v_new_measured_length,0)<coalesce(v_old_measured_length,0) 
					then (v_old_calculated_length-(v_old_measured_length-v_new_measured_length)) end
					WHERE  system_id=v_system_id;
					end if;
				end if;

				if(upper(p_parent_entity_type)='TRENCH')
				then
					select trench_length into v_old_measured_length from att_details_trench WHERE  system_id=v_system_id; 
					select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE  system_id=v_system_id 
					and upper(entity_type)=upper(v_entity_type))) into v_new_measured_length;

					if(v_new_measured_length!=v_old_measured_length)
					then
						update att_details_trench set 
						trench_length= 
						case 
						when coalesce(v_old_measured_length,0)>0 and coalesce(v_new_measured_length,0)>coalesce(v_old_measured_length,0) 
						then (v_old_measured_length+(v_new_measured_length-v_old_measured_length))
						when coalesce(v_old_measured_length,0)>0 and coalesce(v_new_measured_length,0)<coalesce(v_old_measured_length,0) 
						then (v_old_measured_length-(v_old_measured_length-v_new_measured_length)) end
						WHERE  system_id=v_system_id;
					end if;
				end if;
				end if;
			
		end if;

	else
		if(exists(
		select entity_system_id from associate_entity_info 
		where (associated_system_id=v_system_id and upper(associated_entity_type)=upper(v_entity_type)and entity_system_id=p_parent_system_id and 
		upper(entity_type)=upper(p_parent_entity_type)) 
		or (associated_system_id=p_parent_system_id  and upper(associated_entity_type)=upper(p_parent_entity_type) and entity_system_id=v_system_id and 
		upper(entity_type)=upper(v_entity_type))
		))
		then
		
		update att_details_cable set manhole_count=p_manhole_count						
			where system_id=p_parent_system_id;
		
			delete from associate_entity_info 
			where (associated_system_id=v_system_id and  upper(associated_entity_type)=upper(v_entity_type) and entity_system_id=p_parent_system_id and 
			upper(entity_type)=upper(p_parent_entity_type))	or (associated_system_id=p_parent_system_id  and  upper(associated_entity_type)=
			upper(p_parent_entity_type)  and entity_system_id=v_system_id and upper(entity_type)=upper(v_entity_type) );
				
		end if;
	end if;
END LOOP;
close curgeommapping;
	--if(upper(p_parent_entity_type)='CABLE')
	--then
		--select value  into v_CableExtraLengthPercentage from global_settings where key='CableExtraLengthPercentage';
		--select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE system_id = p_parent_system_id and lower(entity_type)=lower('cable'))) into v_cablemeasured_length;
		--select total_loop_length into v_total_loop_length from att_details_cable where system_id=p_parent_system_id; 
		--update att_details_cable set cable_measured_length=v_cablemeasured_length, cable_calculated_length=v_cablemeasured_length+v_total_loop_length+(((v_CableExtraLengthPercentage*v_cablemeasured_length)/100)-v_total_loop_length) where system_id=p_parent_system_id; 
	--end if;
return query
select row_to_json(row) from (
select true as status, 'Success' as message 
 ) row;
 
END
$BODY$;

ALTER FUNCTION public.fn_save_entity_assocition(character varying, integer, character varying, integer, integer)
    OWNER TO postgres;
