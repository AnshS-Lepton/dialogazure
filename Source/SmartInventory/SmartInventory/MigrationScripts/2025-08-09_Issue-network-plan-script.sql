CREATE OR REPLACE FUNCTION public.fn_auto_network_plan_draft_update_loop_network(p_line_geom character varying, p_plan_id integer, p_user_id integer, p_loop_span double precision, p_looplength double precision)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_line_geom geometry;
    v_line_geom_length double precision;
    current_fraction double precision := 0.0;
    structure_location geometry;
BEGIN
    -- Temp table to hold results
    CREATE TEMP TABLE auto_network_plan_temp_loop
    ON COMMIT DROP AS
    SELECT * FROM temp_auto_network_plan WHERE false;
  
    -- Convert input to geometry
    SELECT ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
    INTO v_line_geom;

    -- Line length in meters
    SELECT ST_Length(v_line_geom::geography)
    INTO v_line_geom_length;

    -- Step along the line and insert matching loops
    WHILE current_fraction < 1.0 LOOP
        structure_location := ST_LineInterpolatePoint(
            v_line_geom,
            LEAST(current_fraction + (p_loop_span / v_line_geom_length), 1.0)
        );

        INSERT INTO auto_network_plan_temp_loop (
            system_id, plan_id, entity_type, entity_network_id, longitude, latitude, sp_geometry,
            province_id, region_id, created_by, line_sp_geometry, is_middle_point, fraction, loop_length,
            loop_parent_network_id, loop_parent_entity_type, loop_network_code, loop_sequence_id,
            loop_parent_system_id, cable_id, loop_associated_network_id, loop_associated_system_id,
            loop_entity_type, entity_system_id, display_name, cable_network_id, duct_id, trench_id,
            trench_network_id, duct_network_id
        )
        SELECT *
        FROM temp_auto_network_plan bpnd
        WHERE ST_Intersects(
                  ST_SetSRID(bpnd.sp_geometry, 4326),
                  st_buffer_meters(structure_location,1)
              )
          AND bpnd.plan_id = p_plan_id 
          ORDER BY ST_Distance(ST_SetSRID(bpnd.sp_geometry,4326), structure_location)
			LIMIT 1;
			
       -- ORDER BY bpnd.system_id;
raise INFO 'structure_location %', st_astext(structure_location);

        current_fraction := current_fraction + (p_loop_span / v_line_geom_length);
    END LOOP;
    
    UPDATE temp_auto_network_plan np
    SET loop_length = p_looplength
    FROM auto_network_plan_temp_loop lp
    WHERE np.system_id = lp.system_id;
   
 	UPDATE temp_auto_network_plan np
    SET loop_length = 0   
    WHERE np.system_id not in (select lp.system_id FROM auto_network_plan_temp_loop lp);
   
    RETURN;
END;
$function$
;

-------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_network_planning_get_plan_bom_list(p_plan_name character varying, p_plan_mode character varying, p_cable_type character varying, is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_cable_length double precision, p_distance double precision, p_user_id integer, p_temp_plan_id integer, p_is_loop_require boolean, p_is_loop_update boolean, p_loop_length double precision, p_polepecvendor character varying, p_manholepecvendor character varying, p_scspecvendor character varying, p_loop_span double precision)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
sql text;
v_line_total_length DOUBLE PRECISION;
v_point_entity character varying;
v_line_geom geometry;
v_total_point_entity integer;
v_is_osp_tp boolean;
v_total_line integer;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_cable_length double precision;
v_end_entity_count integer;
v_middle_entity_count integer; 
v_arrow_point record;
v_loop_length double precision;
v_outer_query text;
v_temp_planid integer;
p_site_id character varying;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
sql:='';
v_total_line:=0;
v_temp_planid:=0;


if(p_is_loop_update)
then
v_temp_planid:= p_temp_plan_id;
select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;

select count(plan_id)into v_total_point_entity from temp_auto_network_plan where plan_id=v_temp_planid and loop_length >0;

sql:=sql || 'select '||v_loop_length||' as length_qty,''Loop''::character varying as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
	raise info 'ssdfasdfql=%',sql;	
else

if(upper(p_cable_type)=upper('overhead'))
then
	v_point_entity:='Pole';
	else
	v_point_entity:='Manhole';
end if;


  -- First try with start point
SELECT pod.site_id into p_site_id 
FROM point_master pm
join att_details_pod pod on pod.system_id = pm.system_id 
WHERE pm.entity_type = 'POD'
  AND ST_Within(
        pm.sp_geometry,
        st_buffer_meters(
            ST_StartPoint(
                ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
            ), 2)
    ) limit 1;
  
 -- If not found, try with end point

 if p_site_id is null  
 then 
SELECT pod.site_id into p_site_id FROM point_master pm
join att_details_pod pod on pod.system_id = pm.system_id 
WHERE pm.entity_type = 'POD'
  AND ST_Within(
        pm.sp_geometry,
        st_buffer_meters(
            ST_endPoint(
                ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
            ), 2)
    ) limit 1;
  
  end if;
 
 raise info 'p_site_id =%',p_site_id;


	
select is_osp_tp into v_is_osp_tp from vw_termination_point_master where tp_layer_id=(select layer_id from layer_details where layer_name='SpliceClosure')and layer_id=(select layer_id from layer_details where layer_name='Cable');

  select validation.status,validation.message from  fn_network_planning_validate(p_plan_mode,p_cable_type,is_create_trench,
  is_create_duct,v_is_osp_tp,p_cable_length,v_point_entity,p_user_id,0,p_line_geom) as validation into V_IS_VALID,V_MESSAGE;

 IF(V_IS_VALID)
THEN

-- creating points entity according to line geom;
select * from fn_temp_network_planning(p_plan_mode,p_cable_type,is_create_trench,is_create_duct,p_line_geom,p_cable_length,p_distance,p_user_id,p_plan_name,'', '', '', 0, '', 0, '',p_temp_plan_id)into v_arrow_point;

v_temp_planid:=v_arrow_point.temp_plan_id;

-- count of middle entity
select count(plan_id)into v_middle_entity_count from temp_auto_network_plan where plan_id=v_arrow_point.temp_plan_id and is_middle_point='true';

-- end entity
select count(plan_id) into v_end_entity_count from temp_auto_network_plan where plan_id=v_arrow_point.temp_plan_id and is_middle_point='false';

-- total count of point entity
v_total_point_entity:=v_middle_entity_count+v_end_entity_count;

 select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
 select round(st_length(v_line_geom,false)::numeric, 6)::double precision into v_line_total_length;

-- get counts of cable 
 if(v_line_total_length<p_cable_length)
 then
 	v_total_line:=1;
 else
 	select CEIL(v_line_total_length/p_cable_length)::integer into v_total_line;		
 end if;

-- for cable
sql := 'select '||v_line_total_length||' as length_qty,* from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';

if(is_create_trench) 
then 
	sql:=sql || ' union select '||v_line_total_length||' as length_qty,* from 
	fn_network_planning_audit_template_detail(''trench'','||p_user_id||')';
end if;

if(is_create_duct) 
then 
	sql:=sql || ' union select '||v_line_total_length||' as length_qty,* from fn_network_planning_audit_template_detail(''duct'','||p_user_id||')';
end if;
 

if(upper(p_plan_mode)=upper('auto'))
then
	--sql:=sql || ' union select '||v_total_point_entity||' as length_qty,* from fn_network_planning_audit_template_detail('''||v_point_entity||''','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';	
sql := sql || ' UNION SELECT ' || v_total_point_entity || ' AS length_qty, * 
FROM fn_network_planning_audit_template_detail('
|| quote_literal(v_point_entity) || ',' 
|| p_user_id || ',' 
|| quote_literal(p_polepecvendor) || ',' 
|| quote_literal(p_manholepecvendor) || ',' 
|| quote_literal(p_scspecvendor) || ')';

else
	--sql:=sql || ' union select '||v_total_point_entity||' as length_qty,* from fn_network_planning_audit_template_detail('''||v_point_entity||''','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';
sql := sql || ' UNION SELECT ' || v_total_point_entity || ' AS length_qty, * 
FROM fn_network_planning_audit_template_detail('
|| quote_literal(v_point_entity) || ',' 
|| p_user_id || ',' 
|| quote_literal(p_polepecvendor) || ',' 
|| quote_literal(p_manholepecvendor) || ',' 
|| quote_literal(p_scspecvendor) || ')';

raise info 'inner query11 =%',sql;
end if;

if(v_is_osp_tp)
then 
	sql:=sql || ' union select '||v_end_entity_count||' as length_qty,* from fn_network_planning_audit_template_detail(''spliceclosure'','||p_user_id||','''|| p_polepecvendor||''','''|| p_manholepecvendor||''','''|| p_scspecvendor||''')';
end if;


if(p_is_loop_require)
	then 
		if(p_is_loop_update)
		then 
		 select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;
		else
			v_loop_length := v_total_point_entity * p_loop_length;
		end if;
		
		sql:=sql || ' union select '||v_loop_length||' as length_qty,''Loop'' as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';

	end if;
end if;


end if;

raise info 'inner query =%',sql;

v_outer_query:= 'select '||V_IS_VALID||'::boolean AS is_template_extis,'||v_temp_planid||' as temp_plan_id,
 case when upper(entity_type)=''CABLE'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''DUCT'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''TRENCH'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
 when upper(entity_type)=''LOOP'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_point_entity||')::character varying else length_qty ::character varying end as length_qty,
(round((length_qty)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((length_qty)::numeric, 2))as amount,entity_type,geom_type,cost_per_unit,service_cost_per_unit from ('||sql||')e';

 raise info 'inner query fin=al =%',v_outer_query;
perform (fn_auto_network_plan_draft_update_loop_network(p_line_geom, p_temp_plan_id, p_user_id, p_loop_span, p_loop_length));

IF (V_IS_VALID) THEN
    RETURN QUERY
    EXECUTE 'select row_to_json(row) 
             from (select *,'|| quote_literal(coalesce(p_site_id, '')) ||' as site_id 
                   from ('||v_outer_query||')t order by geom_type) row';
ELSE
    RETURN QUERY
    EXECUTE 'select row_to_json(row) 
             from (select '||V_IS_VALID||'::boolean AS is_template_extis, 
                          '||quote_literal(V_MESSAGE)||'::character varying AS msg, 
                          '||quote_literal(coalesce(p_site_id, ''))||'::character varying as site_id) row';
END IF;

            
 END
$function$
;

---------------------------------------------------------------------------------------------

