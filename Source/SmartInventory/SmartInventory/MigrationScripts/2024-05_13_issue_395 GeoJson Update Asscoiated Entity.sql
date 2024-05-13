CREATE OR REPLACE FUNCTION public.fn_snap_end_point(
	p_system_id integer,
	p_entity_type character varying,
	p_longlat character varying)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE  
 v_longlat character varying;
 v_point_index integer;
 v_point_old_geometry geometry;
 v_last_point_geometry geometry;
 v_second_last_point_geometry geometry;
 v_first_point_geometry geometry;
 v_second_point_geometry geometry;
 v_distance_in_last_two_point double precision;
 v_old_measured_length double precision;
 v_new_measured_length double precision;
 v_old_calculated_length double precision;
 v_new_calculated_length double precision; 
 arow record;
 brow record;
 v_total_point integer;
 v_total_left_point integer;
BEGIN    
execute 'select  longitude||'' ''||latitude,sp_geometry from point_master where system_id='||p_system_id||' and upper(entity_type)=upper('''||p_entity_type||''')' into v_longlat,v_point_old_geometry;
FOR arow IN 
	select cable.system_id,'Cable' as entity_type from att_details_cable cable 
	where ((cable.a_system_id=p_system_id and upper(cable.a_entity_type)=upper(p_entity_type)) or (cable.b_system_id=p_system_id and upper(cable.b_entity_type)=upper(p_entity_type)))
	union
	select duct.system_id,'Duct' as entity_type from att_details_duct duct  
	where ((duct.a_system_id=p_system_id and upper(duct.a_entity_type)=upper(p_entity_type)) or (duct.b_system_id=p_system_id and upper(duct.b_entity_type)=upper(p_entity_type)))
	union
	select mduct.system_id,'Microduct' as entity_type from att_details_microduct mduct  
	where ((mduct.a_system_id=p_system_id and upper(mduct.a_entity_type)=upper(p_entity_type)) or (mduct.b_system_id=p_system_id and upper(mduct.b_entity_type)=upper(p_entity_type)))
	union
	select trench.system_id,'Trench' as entity_type from att_details_trench trench 
	where ((trench.a_system_id=p_system_id and upper(trench.a_entity_type)=upper(p_entity_type)) or (trench.b_system_id=p_system_id and upper(trench.b_entity_type)=upper(p_entity_type)))	
	union
	select cable.system_id,'Cable' as entity_type from att_details_cable cable  
	inner join att_details_fms FMS on ((cable.a_system_id=FMS.system_id and upper(cable.a_entity_type)=upper('FMS')) or (cable.b_system_id=FMS.system_id and upper(cable.b_entity_type)=upper('FMS'))) 
	where FMS.parent_system_id=p_system_id and upper(FMS.parent_entity_type)=upper(p_entity_type)	
	LOOP	
		
		for brow in SELECT (dp).path[1] As index FROM (select  ST_DumpPoints(sp_geometry) AS dp from line_master where system_id=arow.system_id and upper(entity_type)=upper(arow.entity_type)) AS dp
	        where (dp).geom=v_point_old_geometry 
	        loop		          	       
				
			update LINE_MASTER  set SP_GEOMETRY=ST_SetPoint(sp_geometry,brow.index-1,ST_GeomFromText('POINT('||p_longlat||')',4326)) where system_id=arow.system_id and upper(entity_type)=upper(arow.entity_type);	            									
																	
			if(upper(arow.entity_type)='CABLE')
			then
				select cable_measured_length,cable_calculated_length into v_old_measured_length,v_old_calculated_length from att_details_cable WHERE  system_id=arow.system_id; 
				select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE  system_id=arow.system_id and entity_type='Cable')) into v_new_measured_length;
				v_new_measured_length:=coalesce(v_new_measured_length,0);
				v_old_measured_length:=coalesce(v_old_measured_length,0);
				if(v_new_measured_length!=v_old_measured_length)
				then
					update att_details_cable set cable_measured_length=v_new_measured_length,
					cable_calculated_length= case 
					when v_old_calculated_length>0 and v_new_measured_length>v_old_measured_length 
					then (v_old_calculated_length+(v_new_measured_length-v_old_measured_length))
					when v_old_calculated_length>0 and v_new_measured_length<v_old_measured_length 
					then (v_old_calculated_length-(v_old_measured_length-v_new_measured_length)) 
					else cable_calculated_length end
					WHERE  system_id=arow.system_id;
				end if;
			end if;

			if(upper(arow.entity_type)='DUCT')
			then
				select calculated_length,manual_length into v_old_measured_length,v_old_calculated_length from att_details_duct WHERE  system_id=arow.system_id; 
				select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE  system_id=arow.system_id and entity_type='Duct')) into v_new_measured_length;
				v_new_measured_length:=coalesce(v_new_measured_length,0);
				v_old_measured_length:=coalesce(v_old_measured_length,0);
				if(v_new_measured_length!=v_old_measured_length)
				then
					update att_details_duct 
					set calculated_length=v_new_measured_length,
					manual_length= case when v_old_calculated_length>0 and v_new_measured_length>v_old_measured_length 
					then (v_old_calculated_length+(v_new_measured_length-v_old_measured_length))
					when v_old_calculated_length>0 and v_new_measured_length<v_old_measured_length 
					then (v_old_calculated_length-(v_old_measured_length-v_new_measured_length)) 
					else
					manual_length end
					WHERE  system_id=arow.system_id;
				end if;
			end if;

			if(upper(arow.entity_type)='TRENCH')
			then
				select trench_length into v_old_measured_length from att_details_trench WHERE  system_id=arow.system_id; 
				select * from getgeometrylength((SELECT substring(left(St_astext(sp_geometry),-1),12)  from LINE_MASTER WHERE  system_id=arow.system_id and entity_type='Trench')) into v_new_measured_length;
				v_new_measured_length:=coalesce(v_new_measured_length,0);
				v_old_measured_length:=coalesce(v_old_measured_length,0);
				if(v_new_measured_length!=v_old_measured_length)
				then
					update att_details_trench set 
					trench_length= case when v_old_measured_length>0 and v_new_measured_length>v_old_measured_length then (v_old_measured_length+(v_new_measured_length-v_old_measured_length))
					when v_old_measured_length>0 and v_new_measured_length<v_old_measured_length 
					then (v_old_measured_length-(v_old_measured_length-v_new_measured_length)) 
					else
					trench_length
					end
					WHERE  system_id=arow.system_id;
				end if;
			end if;
						perform(fn_geojson_update_entity_attribute(arow.system_id,arow.entity_type,0,1,true));


	        end loop;		
	END LOOP;

    RETURN QUERY
     select false as status, 'Success'::character varying as message;
END
$BODY$;