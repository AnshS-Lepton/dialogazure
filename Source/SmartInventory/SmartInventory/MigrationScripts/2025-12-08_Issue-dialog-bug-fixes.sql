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
is_a_endpoint_entity_exists bool;
is_b_endpoint_entity_exists bool;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
sql:='';
v_total_line:=0;
v_temp_planid:=0;

p_scspecvendor = case when p_scspecvendor is null then '' else p_scspecvendor end;
p_manholepecvendor = case when p_manholepecvendor is null then '' else p_manholepecvendor end;

if(p_line_geom is null or p_line_geom = '') 
then 

SELECT substring(st_astext(line_sp_geometry), 12, length(st_astext(line_sp_geometry)) - 12) INTO p_line_geom from temp_auto_network_plan where plan_id = p_temp_plan_id and line_sp_geometry is not null
limit 1;

end if;

p_polepecvendor = case when p_polepecvendor is null then '' else p_polepecvendor end;

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
--- perform (fn_auto_network_plan_draft_update_loop_network(p_line_geom, p_temp_plan_id, p_user_id, p_loop_span, p_loop_length));

if(p_is_loop_require)
	then 	
		/*if(p_is_loop_update)
		then 
		 select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;
		else
			v_loop_length := v_total_point_entity * p_loop_length;
		end if;*/

 /*select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;
	
		sql:=sql || ' union select '||v_loop_length||' as length_qty,''Loop'' as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
*/
	end if;
end if;


end if;

raise info 'inner query =%',sql;

 SELECT count(*)>0 into is_a_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure','BDB','FDB', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = v_temp_planid and tp.fraction = 0
 ORDER BY tp.system_id LIMIT 1),4326),2));

 SELECT count(*)>0 into is_b_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure','BDB','FDB', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = v_temp_planid and tp.fraction = 1
 ORDER BY tp.system_id LIMIT 1),4326),2));

        -- First try with start point
        SELECT pod.site_id
        INTO p_site_id
        FROM point_master pm
        JOIN att_details_pod pod ON pod.system_id = pm.system_id 
        WHERE pm.entity_type = 'POD'
          AND ST_Within(
                pm.sp_geometry,
                st_buffer_meters(
                    ST_StartPoint(
                        ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
                    ), 3)
            )
        LIMIT 1;
       
      p_site_id = case when p_site_id is not null then p_site_id else '' end;
         -- Update Manhole names (forward order)
        UPDATE temp_auto_network_plan t
        SET entity_name = 
            CASE
                WHEN p_site_id IS null or p_site_id ='' THEN 'MH-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-MH-' || LPAD(x.seq::text, 2, '0')
            END
        FROM (
           SELECT system_id,
          ROW_NUMBER() OVER (ORDER BY system_id desc) AS seq
		FROM temp_auto_network_plan
		WHERE entity_type = 'Manhole'
		  AND plan_id = v_temp_planid
		  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
             
        ) x
        WHERE t.system_id = x.system_id
          AND t.plan_id = v_temp_planid;

        -- Update Pole names (forward order)
        UPDATE temp_auto_network_plan t
        SET entity_name = 
            CASE
                WHEN p_site_id IS null or p_site_id ='' THEN 'PL-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-PL-' || LPAD(x.seq::text, 2, '0')
            END
        FROM (
            SELECT system_id,
                   ROW_NUMBER() OVER (ORDER BY system_id desc) AS seq
            FROM temp_auto_network_plan
            WHERE entity_type = 'Pole'
              AND plan_id = v_temp_planid
              AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
        ) x
        WHERE t.system_id = x.system_id
          AND t.plan_id = v_temp_planid;

        -- If not found, try with end point
        IF p_site_id IS null or p_site_id = '' THEN
            SELECT pod.site_id 
            INTO p_site_id
            FROM point_master pm
            JOIN att_details_pod pod ON pod.system_id = pm.system_id 
            WHERE pm.entity_type = 'POD'
              AND ST_Within(
                    pm.sp_geometry,
                    st_buffer_meters(
                        ST_EndPoint(
                            ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
          ), 3) )
            LIMIT 1;
           
       

         IF p_site_id is Not NULL then
                  

            -- Update Manhole names (reverse order)
            UPDATE temp_auto_network_plan t
            SET entity_name =  CASE
                WHEN p_site_id IS NULL  or p_site_id ='' THEN 'MH-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-MH-' || LPAD(x.seq::text, 2, '0')
            END
            FROM (
                SELECT system_id,
                       ROW_NUMBER() OVER (ORDER BY system_id ) AS seq
                FROM temp_auto_network_plan 
                WHERE entity_type = 'Manhole'
                  AND plan_id = v_temp_planid
                  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
            ) x
            WHERE t.system_id = x.system_id
              AND t.plan_id = v_temp_planid;

            -- Update Pole names (reverse order)
            UPDATE temp_auto_network_plan t
            SET entity_name =  CASE
                WHEN p_site_id IS NULL or p_site_id ='' THEN 'PL-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-PL-' || LPAD(x.seq::text, 2, '0') 
            END
            FROM (
                SELECT system_id,
                       ROW_NUMBER() OVER (ORDER BY system_id) AS seq
                FROM temp_auto_network_plan
                WHERE entity_type = 'Pole'
                  AND plan_id = v_temp_planid
                  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
            ) x
            WHERE t.system_id = x.system_id
              AND t.plan_id = v_temp_planid;
             
            -- p_site_id = p_site_id|| '--ASC';
        END IF;
   END IF;
 
  perform (fn_auto_network_plan_draft_update_loop_network(p_line_geom, v_temp_planid, p_user_id, p_loop_span, p_loop_length));
 
 if(p_is_loop_require)
	then 
  select coalesce(sum(loop_length)::double precision,0) ,count(*)  from temp_auto_network_plan where plan_id=v_temp_planid AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      ) into v_loop_length,v_total_point_entity;
	
		sql:=sql || ' union select '||v_loop_length||' as length_qty,''Loop'' as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
end if;
	
v_outer_query:= 'select '||V_IS_VALID||'::boolean AS is_template_extis,'||v_temp_planid||' as temp_plan_id,
 case when upper(entity_type)=''CABLE'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''DUCT'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''TRENCH'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
 when upper(entity_type)=''LOOP'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_point_entity||')::character varying else length_qty ::character varying end as length_qty,
(round((length_qty)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((length_qty)::numeric, 2))as amount,entity_type,geom_type,cost_per_unit,service_cost_per_unit from ('||sql||')e';

 raise info 'inner query fin=al =%',v_outer_query;
  
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


CREATE OR REPLACE FUNCTION public.fn_backbone_convert_to_planned_network(p_plan_id integer, p_user_id integer)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_status    boolean;
    v_message   character varying;
    v_Geomtablename text;
    rec record;
BEGIN

    v_status := true;
    v_message := 'Success';

    FOR rec IN 
        SELECT layer_table, geom_type, layer_name
        FROM layer_details 
        WHERE layer_name IN ('Cable','SpliceClosure','Duct','Trench','Pole','Manhole')
    LOOP

        -- determine geometry table
        IF lower(rec.geom_type) = 'point' THEN
            v_Geomtablename := 'point_master';

        ELSIF lower(rec.geom_type) = 'line' THEN
            v_Geomtablename := 'line_master';

        ELSIF lower(rec.geom_type) = 'polygon' THEN
            v_Geomtablename := 'polygon_master';
        END IF;

        -- Update layer table
        EXECUTE
            'UPDATE ' || rec.layer_table ||
            ' SET network_status = ''P''' ||
            ' WHERE source_ref_id = ''' || p_plan_id || '''' ||
            ' AND source_ref_type = ''backbone planning''' ||
            ' AND created_by = ' || p_user_id || ';';

        -- Update geometry table
        EXECUTE
            'UPDATE ' || v_Geomtablename ||
            ' SET network_status = ''P''' ||
            ' WHERE source_ref_id = ''' || p_plan_id || '''' ||
            ' AND source_ref_type = ''backbone planning''' ||
            ' AND entity_type = ''' || rec.layer_name || '''' ||
            ' AND created_by = ' || p_user_id || ';';

    END LOOP;

    v_status := true;
    v_message := 'The Plan successfully converted from Planned to As-Built.';

    RETURN QUERY 
        SELECT v_status, v_message;

END
$function$
;

-- Permissions

--------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_auto_network_plan_draft_update_loop_network(p_line_geom character varying, p_plan_id integer, p_user_id integer, p_loop_span double precision, p_looplength double precision)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_line_geom geometry;
    v_line_length double precision;
    step_fraction double precision;
    v_fraction double precision := 0;
BEGIN

    -- Create temp loop table
    CREATE TEMP TABLE IF NOT EXISTS auto_network_plan_temp_loop
    ON COMMIT DROP AS
    SELECT * FROM temp_auto_network_plan WHERE false;

    -- Convert line
    SELECT ST_GeomFromText('LINESTRING(' || p_line_geom || ')', 4326)
    INTO v_line_geom;

    -- Get length
    SELECT ST_Length(v_line_geom::geography) INTO v_line_length;

    -- Compute fraction step
    IF v_line_length > 0 AND p_loop_span > 0 THEN
        step_fraction := p_loop_span / v_line_length;
    ELSE
        step_fraction := 1;
    END IF;

    -- First step (skip 0)
    v_fraction := step_fraction;

    -- Loop until below 1
    WHILE v_fraction < 1 LOOP
        INSERT INTO auto_network_plan_temp_loop
        SELECT t.*
        FROM temp_auto_network_plan t
        WHERE t.plan_id = p_plan_id
        ORDER BY ABS(t.fraction - v_fraction)
        LIMIT 1;

        v_fraction := v_fraction + step_fraction;
    END LOOP;

    -- Add endpoint (always include 1.0)
    INSERT INTO auto_network_plan_temp_loop
    SELECT *
    FROM temp_auto_network_plan t
    WHERE t.plan_id = p_plan_id
    ORDER BY ABS(t.fraction - 1.0)
    LIMIT 1;

    -- Remove duplicates BEFORE applying update
    DELETE FROM auto_network_plan_temp_loop a
    USING auto_network_plan_temp_loop b
    WHERE a.ctid < b.ctid
      AND a.system_id = b.system_id;

    -- Apply loop length to matched entities
    UPDATE temp_auto_network_plan np
    SET loop_length = p_looplength
    WHERE np.system_id IN (SELECT system_id FROM auto_network_plan_temp_loop);

    -- Reset loop length for non-selected entities
    UPDATE temp_auto_network_plan np
    SET loop_length = 0
    WHERE np.plan_id = p_plan_id
      AND np.system_id NOT IN (SELECT system_id FROM auto_network_plan_temp_loop);

END;
$function$
;


-- Permissions
-------------------------------------------------------------------------------------------------
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
is_a_endpoint_entity_exists bool;
is_b_endpoint_entity_exists bool;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
sql:='';
v_total_line:=0;
v_temp_planid:=0;

p_scspecvendor = case when p_scspecvendor is null then '' else p_scspecvendor end;
p_manholepecvendor = case when p_manholepecvendor is null then '' else p_manholepecvendor end;

if(p_line_geom is null or p_line_geom = '') 
then 

SELECT substring(st_astext(line_sp_geometry), 12, length(st_astext(line_sp_geometry)) - 12) INTO p_line_geom from temp_auto_network_plan where plan_id = p_temp_plan_id and line_sp_geometry is not null
limit 1;

end if;

p_polepecvendor = case when p_polepecvendor is null then '' else p_polepecvendor end;

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
--- perform (fn_auto_network_plan_draft_update_loop_network(p_line_geom, p_temp_plan_id, p_user_id, p_loop_span, p_loop_length));

if(p_is_loop_require)
	then 	
		/*if(p_is_loop_update)
		then 
		 select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;
		else
			v_loop_length := v_total_point_entity * p_loop_length;
		end if;*/

 /*select coalesce(sum(loop_length)::double precision,0)  from temp_auto_network_plan where plan_id=p_temp_plan_id into v_loop_length;
	
		sql:=sql || ' union select '||v_loop_length||' as length_qty,''Loop'' as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
*/
	end if;
end if;


end if;

raise info 'inner query =%',sql;

 SELECT count(*)>0 into is_a_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure','BDB','FDB', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = v_temp_planid and tp.fraction = 0
 ORDER BY tp.system_id LIMIT 1),4326),2));

 SELECT count(*)>0 into is_b_endpoint_entity_exists FROM point_master pm
 WHERE pm.entity_type IN ('Pole', 'SpliceClosure','BDB','FDB', 'Manhole', 'FMS','POD')
 AND ST_Within(pm.sp_geometry, st_buffer_meters(
 ST_SetSRID((SELECT sp_geometry FROM temp_auto_network_plan tp
 WHERE tp.plan_id = v_temp_planid and tp.fraction = 1
 ORDER BY tp.system_id LIMIT 1),4326),2));

        -- First try with start point
        SELECT pod.site_id
        INTO p_site_id
        FROM point_master pm
        JOIN att_details_pod pod ON pod.system_id = pm.system_id 
        WHERE pm.entity_type = 'POD'
          AND ST_Within(
                pm.sp_geometry,
                st_buffer_meters(
                    ST_StartPoint(
                        ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
                    ), 3)
            )
        LIMIT 1;
       
      p_site_id = case when p_site_id is not null then p_site_id else '' end;
         -- Update Manhole names (forward order)
        UPDATE temp_auto_network_plan t
        SET entity_name = 
            CASE
                WHEN p_site_id IS null or p_site_id ='' THEN 'MH-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-MH-' || LPAD(x.seq::text, 2, '0')
            END
        FROM (
           SELECT system_id,
          ROW_NUMBER() OVER (ORDER BY system_id desc) AS seq
		FROM temp_auto_network_plan
		WHERE entity_type = 'Manhole'
		  AND plan_id = v_temp_planid
		  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
             
        ) x
        WHERE t.system_id = x.system_id
          AND t.plan_id = v_temp_planid;

        -- Update Pole names (forward order)
        UPDATE temp_auto_network_plan t
        SET entity_name = 
            CASE
                WHEN p_site_id IS null or p_site_id ='' THEN 'PL-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-PL-' || LPAD(x.seq::text, 2, '0')
            END
        FROM (
            SELECT system_id,
                   ROW_NUMBER() OVER (ORDER BY system_id desc) AS seq
            FROM temp_auto_network_plan
            WHERE entity_type = 'Pole'
              AND plan_id = v_temp_planid
              AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
        ) x
        WHERE t.system_id = x.system_id
          AND t.plan_id = v_temp_planid;

        -- If not found, try with end point
        IF p_site_id IS null or p_site_id = '' THEN
            SELECT pod.site_id 
            INTO p_site_id
            FROM point_master pm
            JOIN att_details_pod pod ON pod.system_id = pm.system_id 
            WHERE pm.entity_type = 'POD'
              AND ST_Within(
                    pm.sp_geometry,
                    st_buffer_meters(
                        ST_EndPoint(
                            ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
          ), 3) )
            LIMIT 1;
           
       

         IF p_site_id is Not NULL then
                  

            -- Update Manhole names (reverse order)
            UPDATE temp_auto_network_plan t
            SET entity_name =  CASE
                WHEN p_site_id IS NULL  or p_site_id ='' THEN 'MH-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-MH-' || LPAD(x.seq::text, 2, '0')
            END
            FROM (
                SELECT system_id,
                       ROW_NUMBER() OVER (ORDER BY system_id ) AS seq
                FROM temp_auto_network_plan 
                WHERE entity_type = 'Manhole'
                  AND plan_id = v_temp_planid
                  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
            ) x
            WHERE t.system_id = x.system_id
              AND t.plan_id = v_temp_planid;

            -- Update Pole names (reverse order)
            UPDATE temp_auto_network_plan t
            SET entity_name =  CASE
                WHEN p_site_id IS NULL or p_site_id ='' THEN 'PL-' || LPAD(x.seq::text, 2, '0')
                ELSE p_site_id || '-PL-' || LPAD(x.seq::text, 2, '0') 
            END
            FROM (
                SELECT system_id,
                       ROW_NUMBER() OVER (ORDER BY system_id) AS seq
                FROM temp_auto_network_plan
                WHERE entity_type = 'Pole'
                  AND plan_id = v_temp_planid
                  AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      )
            ) x
            WHERE t.system_id = x.system_id
              AND t.plan_id = v_temp_planid;
             
            -- p_site_id = p_site_id|| '--ASC';
        END IF;
   END IF;
 
  perform (fn_auto_network_plan_draft_update_loop_network(p_line_geom, v_temp_planid, p_user_id, p_loop_span, p_loop_length));
 
 if(p_is_loop_require)
	then 
  select coalesce(sum(loop_length)::double precision,0) ,count(*)  from temp_auto_network_plan where plan_id=v_temp_planid AND (
		        CASE
		            WHEN is_a_endpoint_entity_exists THEN fraction > 0
		            ELSE TRUE
		        END
		      )
		  AND (
		        CASE
		            WHEN is_b_endpoint_entity_exists THEN fraction < 1
		            ELSE TRUE
		        END
		      ) into v_loop_length,v_total_point_entity;
	
		sql:=sql || ' union select '||v_loop_length||' as length_qty,''Loop'' as entity_type,geom_type,cost_per_unit,service_cost_per_unit
		from fn_network_planning_audit_template_detail(''cable'','||p_user_id||')';
end if;
	
v_outer_query:= 'select '||V_IS_VALID||'::boolean AS is_template_extis,'||v_temp_planid||' as temp_plan_id,
 case when upper(entity_type)=''CABLE'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''DUCT'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
when upper(entity_type)=''TRENCH'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_line||')::character varying
 when upper(entity_type)=''LOOP'' then concat(round((length_qty)::numeric, 2),''(m)/'','||v_total_point_entity||')::character varying else length_qty ::character varying end as length_qty,
(round((length_qty)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((length_qty)::numeric, 2))as amount,entity_type,geom_type,cost_per_unit,service_cost_per_unit from ('||sql||')e';

 raise info 'inner query fin=al =%',v_outer_query;
  
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


-- Permissions

