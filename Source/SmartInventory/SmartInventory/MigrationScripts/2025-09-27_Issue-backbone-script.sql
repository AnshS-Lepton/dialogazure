alter table att_details_loop add column backbone_system_id integer null;
alter table att_details_pole add column backbone_system_id integer null;
alter table att_details_spliceclosure add column backbone_system_id integer null;
alter table att_details_manhole add column backbone_system_id integer null;
alter table att_details_cable add column backbone_system_id integer null;

-----------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_backbone_update_sprout_network(p_linegeom character varying, p_systemid integer, p_plan_id integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_line_geom geometry;
    v_line_geom_length double precision;
	current_fraction double precision := 0.0;
	structure_location geometry;
	p_pole_span integer;
	p_manhole_span integer;
	p_backbone_fiber_type character varying;
	line_geom geometry;
	p_is_looprequired bool;
	p_looplength  double precision;
	step_fraction double precision;
	P_SITE_NETWORK_ID character varying;
    V_closet_point character varying;
    V_is_update bool;
   sp_route_length double precision;
   sp_totalroute_length double precision;
   p_nearest_pole_manhole geometry;
	p_threshold double precision;
	p_cable_drum_length double precision;
    p_cable_type character varying;
   v_entity_type character varying;
   v_geometry geometry;
   updated_line_geom geometry;
begin
		 
      select bpd.pole_distance ,bpd.manhole_distance,bpd.loop_length ,bpd.is_loop_required,bpd.threshold,bpd.cable_length  into p_pole_span,p_manhole_span,p_looplength,p_is_looprequired,p_threshold,p_cable_drum_length from backbone_plan_details bpd where plan_id = p_plan_id;
		
	select fibertype,network_id,intersect_line_geom,is_update 
into p_backbone_fiber_type,P_SITE_NETWORK_ID, V_closet_point,V_is_update from backbone_plan_nearest_site bpns where id = p_systemid;	

 	UPDATE backbone_plan_nearest_site
    SET line_geom = p_linegeom
    WHERE id = p_systemid AND plan_id = p_plan_id;
   
	if(V_is_update)
	then 
	delete from backbone_plan_network_details bpnd 
	where site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id;

	end if;

	select ST_GeomFromText(concat('LINESTRING(', p_linegeom, ')'), 4326) into v_line_geom;
		
   select st_length(v_line_geom,false) into v_line_geom_length;
  
  v_geometry = ST_GeomFromText(
    concat('POINT(', trim(split_part(V_closet_point, ',', 2)), ' ', trim(split_part(V_closet_point, ',', 1)), ')'),
    4326
);
		-- Pole check for non-restricted area
	IF (v_line_geom_length < p_cable_drum_length) THEN

	if NOT EXISTS (
	        SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_geometry)
	          AND entity_type = 'RestrictedArea')
	 then 
	v_entity_type ='Pole';
	else 
	v_entity_type ='Manhole';
	end if;

	END IF;

	 IF (v_line_geom_length < p_manhole_span  ) 
	 THEN
      if not EXISTS (SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_geometry)
	          AND entity_type = 'RestrictedArea') then 
	          v_entity_type ='Pole';
			else 
			v_entity_type ='Manhole';
	          
	       end if;
	  end if;
	 
 	IF (v_line_geom_length < p_pole_span) 
	 THEN
      if not EXISTS (SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_geometry)
	          AND entity_type = 'RestrictedArea') then 
	          v_entity_type ='Pole';
			else 
			v_entity_type ='Manhole';
	          
	       end if;
	  end if;

    if v_entity_type in ('Pole','Manhole')
    then   
     IF EXISTS (select 1 FROM backbone_plan_network_details
    	WHERE st_within(sp_geometry,
            st_buffer_meters(v_geometry, p_threshold))
            AND plan_id = p_plan_id  and planned_cable_entity ='BackBone Cable' and entity_type in ('Pole','Manhole'))
     then
             raise info 'test1 %',st_astext(v_line_geom);
	          raise info 'v_line_geom %',v_line_geom;


        SELECT ST_AddPoint(v_line_geom,sp_geometry, 0)  into updated_line_geom       
		FROM backbone_plan_network_details bpn
		WHERE bpn.plan_id = p_plan_id
		  AND bpn.entity_type IN ('Pole','Manhole') and 
		  planned_cable_entity ='BackBone Cable'
		  AND ST_Within(bpn.sp_geometry, st_buffer_meters(v_geometry, p_threshold))
	   ORDER BY ST_Distance(bpn.sp_geometry, v_geometry)
	   LIMIT 1;
	          raise info 'v_line_geom %',updated_line_geom;

	 end if;

    -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type,
        loop_length, is_loop_required, planned_cable_entity, site_network_id,region_id ,province_id,sp_geometry ,longitude ,latitude  
    )
    VALUES (
        p_plan_id, v_entity_type,  p_user_id, p_backbone_fiber_type, 
        p_looplength, p_is_looprequired, 'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(updated_line_geom), ST_X(ST_startPoint(updated_line_geom)),ST_Y(ST_startPoint(updated_line_geom))
    );

     -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type,
        loop_length, is_loop_required, planned_cable_entity, site_network_id,region_id ,province_id ,sp_geometry ,longitude ,latitude 
    )
    VALUES (
        p_plan_id, 'SpliceClosure', p_user_id, p_backbone_fiber_type,
        p_looplength, p_is_looprequired, 'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(updated_line_geom), ST_X(ST_startPoint(updated_line_geom)),ST_Y(ST_startPoint(updated_line_geom))
    );

      -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type, line_sp_geometry,
        loop_length, is_loop_required, planned_cable_entity, site_network_id,region_id ,province_id ,sp_geometry ,longitude ,latitude 
    )
    VALUES (
        p_plan_id, 'Cable', p_user_id, p_backbone_fiber_type, updated_line_geom,
        p_looplength, p_is_looprequired, 'Sprout Cable', P_SITE_NETWORK_ID,0,0,ST_startPoint(updated_line_geom), ST_X(ST_startPoint(updated_line_geom)),ST_Y(ST_startPoint(updated_line_geom))
    );
      
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
   into sp_route_length from backbone_plan_network_details
   where site_network_id = P_SITE_NETWORK_ID and entity_type = 'Cable' and 
 	plan_id = P_plan_id and line_sp_geometry is not null;
 
   -- total sprout cable 
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where entity_type = 'Cable' and plan_id = p_plan_id;
   
   UPDATE backbone_plan_network_details AS np
SET province_id = pm.id,
    region_id   = rm.id
FROM region_boundary AS rm,
     province_boundary AS pm
WHERE rm.isvisible = TRUE
  AND pm.isvisible = TRUE
  AND ST_Intersects(
         pm.sp_geometry,
         np.sp_geometry 
     )
  AND ST_Intersects(
         rm.sp_geometry,
          np.sp_geometry 
     )
     and np.planned_cable_entity ='Sprout Cable'
  AND np.plan_id  = P_plan_id
  AND np.created_by = p_user_id and np.sp_geometry is not null and site_network_id = P_SITE_NETWORK_ID;
 
  RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;
return;

	END IF;
	   
  
      WHILE current_fraction < 1.0 LOOP
        -- Try pole first
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_pole_span / v_line_geom_length), 1.0));

        -- Check if it's in a restricted area
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));

            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,loop_length,planned_cable_entity,site_network_id ,province_id ,region_id 
            ) VALUES (
                p_plan_id, 'Manhole', ST_X(ST_EndPoint(line_geom)),ST_Y(ST_EndPoint(line_geom)),
                ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction,p_looplength,
                'Sprout Cable',P_SITE_NETWORK_ID,0,0
            );           
           
        else
        	line_geom = NULL;
            step_fraction := p_pole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));

            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,fraction ,planned_cable_entity,site_network_id ,province_id,region_id ,loop_length ,is_loop_required 
            ) 
            VALUES (p_plan_id, 'Pole', ST_X(ST_EndPoint(line_geom)) ,ST_Y(ST_EndPoint(line_geom)),ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,current_fraction,'Sprout Cable',P_SITE_NETWORK_ID,0,0,0,p_is_looprequired
            );

        END IF;

        -- Move to next step
        current_fraction := current_fraction + step_fraction;
        EXIT WHEN current_fraction >= 1.0;
    END LOOP;  
   
   	delete from backbone_plan_network_details where system_id =(
   select system_id from backbone_plan_network_details where plan_id = p_plan_id and 
   site_network_id =P_SITE_NETWORK_ID and entity_type in ('Manhole','Pole') order by system_id desc limit 1 );
  
   current_fraction := 0.0;

    WHILE current_fraction < 1.0 LOOP
       
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_cable_drum_length / v_line_geom_length), 1.0));         

        step_fraction := p_cable_drum_length / v_line_geom_length;
       
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
           

        -- cable type logic
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            p_cable_type := 'Underground';
           v_entity_type = 'Manhole';
        ELSE
            p_cable_type := 'Overhead';
           v_entity_type = 'Pole';
        END IF;
       
RAISE INFO 'line_geom = %', ST_AsText(line_geom);
RAISE INFO 'cable_type geom = %', p_cable_type;


        -- insert cable
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, created_by, fiber_type, fraction, planned_cable_entity, cable_type, line_sp_geometry, sp_geometry,site_network_id 
        )
        VALUES (
            p_plan_id, 'Cable', p_user_id, p_backbone_fiber_type,
            current_fraction, 'Sprout Cable', p_cable_type, line_geom, ST_STARTPoint(line_geom),P_SITE_NETWORK_ID
        );

        -- insert splice closure at segment end
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity,site_network_id 
        )
        VALUES (
            p_plan_id, 'SpliceClosure',
            ST_X(ST_STARTPoint(line_geom)), ST_Y(ST_STARTPoint(line_geom)),
            ST_STARTPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'Sprout Cable',P_SITE_NETWORK_ID
        );
       
        if not exists (SELECT 1 FROM backbone_plan_network_details
            WHERE plan_id = p_plan_id and ST_Intersects(sp_geometry, structure_location) and planned_cable_entity
='Sprout Cable' and site_network_id = P_SITE_NETWORK_ID AND entity_type = v_entity_type and sp_geometry is not null) 
              then 
		INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity,site_network_id 
        )
        VALUES (
            p_plan_id, v_entity_type , ST_X(ST_STARTPoint(line_geom)), ST_Y(ST_STARTPoint(line_geom)),
            ST_STARTPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'Sprout Cable',P_SITE_NETWORK_ID
        );
       end if;
      
        current_fraction := current_fraction + step_fraction;

        EXIT WHEN current_fraction >= 1.0;
    END LOOP;
    
  /*delete from backbone_plan_network_details where system_id =(
   select system_id from backbone_plan_network_details where plan_id = p_plan_id and 
   site_network_id =P_SITE_NETWORK_ID and entity_type in ('Manhole','Pole') order by system_id desc limit 1);*/
  
     IF EXISTS (select 1 FROM backbone_plan_network_details
    		WHERE st_within(sp_geometry,
            st_buffer_meters(v_geometry, p_threshold))
            AND plan_id = p_plan_id  and planned_cable_entity ='BackBone Cable' and entity_type in ('Pole','Manhole'))
     then
     raise info 'test %',st_astext(v_geometry);

        SELECT sp_geometry into p_nearest_pole_manhole       
		FROM backbone_plan_network_details bpn
		WHERE bpn.plan_id = p_plan_id
		  AND bpn.entity_type  IN ('Pole','Manhole') and 
		  planned_cable_entity ='BackBone Cable'
		  AND st_within(sp_geometry,
            st_buffer_meters(v_geometry, p_threshold))
	   ORDER BY ST_Distance(bpn.sp_geometry, v_geometry)
	   LIMIT 1;
	  RAISE INFO 'p_nearest_pole_manhole = %', ST_AsText(p_nearest_pole_manhole);

	  -- update  line geom if exists around pole, manhole	  	  
	  update backbone_plan_network_details set line_sp_geometry  = ST_AddPoint(line_sp_geometry,p_nearest_pole_manhole,0) 
	  where fraction = 0 and site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id
	  and entity_type  = 'Cable';
	
	  update backbone_plan_network_details set sp_geometry = v_geometry ,
	  latitude = ST_Y(v_geometry) , longitude = ST_X(v_geometry)
	  where fraction = 0 and site_network_id = P_SITE_NETWORK_ID and plan_id = p_plan_id 
	  and entity_type = 'SpliceClosure';
	 	 
	 
	  end if;
	  
	 select sum(round(ST_Length(line_sp_geometry, false)::numeric,3)) into sp_route_length 
    from backbone_plan_network_details where site_network_id = P_SITE_NETWORK_ID and entity_type ='Cable'
    and plan_id = p_plan_id;
   
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where planned_cable_entity = 'Sprout Cable' and entity_type ='Cable' and plan_id = p_plan_id;
   
UPDATE backbone_plan_network_details AS np
SET province_id = pm.id,
    region_id   = rm.id
FROM region_boundary AS rm,
     province_boundary AS pm
WHERE rm.isvisible = TRUE
  AND pm.isvisible = TRUE
  AND ST_Intersects(
         pm.sp_geometry,
         np.sp_geometry 
     )
  AND ST_Intersects(
         rm.sp_geometry,
          np.sp_geometry 
     )
     and np.planned_cable_entity ='Sprout Cable'
  AND np.plan_id  = P_plan_id
  AND np.created_by = p_user_id and np.sp_geometry is not null;


 RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;

END;
$function$
;

------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_bom(p_plan_id integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
sql TEXT := '';
v_loop_length double precision;
v_calculated_loop_length double precision;
v_loop_entity_count integer;
k_specification character  varying;
k_code character  varying;
is_create_loop boolean;
begin


	
select count(1)>0 into is_create_loop  from backbone_plan_network_details bpnd where  plan_id = p_plan_id and is_loop_required = true;

SELECT  substring(fiber_type FROM '\(([^)]+)\)'), split_part(fiber_type, ')', 2) into k_code, k_specification   
FROM backbone_plan_network_details
WHERE plan_id = p_plan_id
  AND entity_type = 'Manhole'
LIMIT 1;

    -- Cable
    sql :=  'SELECT
     entity_type,
     geom_type,
     SUM(cost_per_unit)         AS cost_per_unit,
     SUM(service_cost_per_unit) AS service_cost_per_unit,
     ROUND(SUM(measured_length)::numeric, 2) || ''(m)/'' || SUM(length_qty)::int AS length_qty,
     SUM(amount) AS amount
 FROM (
     SELECT
         (SELECT layer_title
            FROM layer_details
           WHERE layer_name ILIKE ''cable'')            AS entity_type,
         ''Line''                                        AS geom_type,
         COALESCE(item.cost_per_unit,        0)          AS cost_per_unit,
         COALESCE(item.service_cost_per_unit,0)          AS service_cost_per_unit,
         COUNT(att.system_id)                            AS length_qty,
         SUM(att.cable_measured_length)                  AS measured_length,
         -- total for this cable slice
         (SUM(att.cable_measured_length) * COALESCE(item.service_cost_per_unit,0) +
          COALESCE(item.cost_per_unit,0)  * SUM(att.cable_measured_length)) AS amount
       FROM att_details_cable  att
       join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Cable''
       JOIN item_template_master item
         ON item.audit_id = att.audit_item_master_id
      WHERE att.source_ref_id      = ' || quote_literal(p_plan_id) || '
        AND att.source_ref_type    = ''backbone planning''
        AND item.category_reference = ''Cable''
      GROUP BY item.cost_per_unit,
               item.service_cost_per_unit,
               att.specification,
               att.item_code, lm.sp_geometry
 ) sub
 GROUP BY entity_type, geom_type';

    -- Pole
    IF EXISTS (SELECT 1 FROM att_details_pole WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id ::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''Pole'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(1) AS length_qty
            FROM att_details_pole att
            join point_master pm on pm.system_id = att.system_id
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''Pole'' and item.specification = ''Generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;

    -- Manhole
    IF EXISTS (SELECT 1 FROM att_details_manhole WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''Manhole'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                item.cost_per_unit AS cost_per_unit,
                item.service_cost_per_unit AS service_cost_per_unit,
                COUNT(1) AS length_qty
            FROM att_details_manhole att
            join point_master pm on pm.system_id = att.system_id
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''Manhole'' and item.specification = ''Generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;

    -- SpliceClosure
    IF EXISTS (SELECT 1 FROM att_details_spliceclosure WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount 
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''SpliceClosure'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(att.system_id) AS length_qty
            FROM att_details_spliceclosure att
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''SpliceClosure'' and item.specification = ''generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name
        ) e';
    END IF;
 if is_create_loop = true then   
sql:=sql ||' UNION SELECT 
    ''Loop'', 
    ''Point'', 
    0 AS cost_per_unit, 
    0 AS service_cost_per_unit,
    (loop_data.loop_length || ''(m)/''||  loop_data.loop_count) ::character varying  AS length_qty,
    0 AS amount
FROM (
    SELECT sum(loop_length) as loop_length ,count(1) as loop_count from att_details_loop  where source_ref_type = ''backbone planning'' and source_ref_id='''||p_plan_id||''' 
        
) AS loop_data';
end if;

if exists (SELECT 1 FROM att_details_trench WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) 
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty ,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount
	 from (
	select (select layer_title from layer_details where layer_name ilike ''trench'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty, st_length(lm.sp_geometry,false) as measured_length 
from att_details_trench att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Trench''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where att.source_ref_type = ''backbone planning'' and att.source_ref_id='''||p_plan_id||'''  GROUP BY
	 ITEM.COST_PER_UNIT,ITEM.SERVICE_COST_PER_UNIT,ATT.SPECIFICATION,ATT.ITEM_CODE,VM.NAME) e';
end if;

if exists (SELECT 1 FROM att_details_duct WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar)
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount
	 from 
(select (select layer_title from layer_details where layer_name ilike ''duct'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty,sum(calculated_length) as measured_length
from att_details_duct att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Duct''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where att.source_ref_type = ''backbone planning'' and att.source_ref_id='''||p_plan_id||'''  GROUP BY
	 ITEM.COST_PER_UNIT,ITEM.SERVICE_COST_PER_UNIT,ATT.SPECIFICATION,ATT.ITEM_CODE,VM.NAME) e';
end if;

    -- Debug SQL
    RAISE INFO 'Final SQL: %', sql;

    -- Execute the full dynamic query and return as JSON
    RETURN QUERY EXECUTE '
        SELECT row_to_json(row) 
        FROM (
            SELECT * FROM (' || sql || ') t 
            ORDER BY geom_type
        ) row';

END;
$function$
;

-------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_loop_entity(p_plan_id integer)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
v_current_point_geom geometry;
v_temp_point_entity record;
v_loop_network_id record;
v_line_geom geometry;
v_arow_loop record;
BEGIN

	INSERT INTO public.att_details_loop(
	network_id, loop_length, associated_system_id, associated_network_id, 
	associated_entity_type, cable_system_id, created_by, created_on, 
	network_status, start_reading, end_reading, 
	parent_system_id, sequence_id, province_id, region_id, parent_network_id, 
	parent_entity_type, status, 
	is_visible_on_map, source_ref_type, source_ref_id, 
	source_ref_description, latitude, longitude, is_new_entity, backbone_system_id)
	select '',loop_length,entity_system_id,entity_network_id,
	entity_type,0,created_by,now(),
	'P',0,0,0, 0,province_id, region_id,'','',
	'A',true,'backbone planning', plan_id::varchar,'',latitude,longitude,true,tp.system_id 
	from backbone_plan_network_details tp where entity_type in ('Pole','Manhole') 
	and is_loop_required = true and loop_length >0 and sp_geometry is not null and plan_id = p_plan_id 
	order by system_id;

	INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,
	creator_remark,approver_remark,created_by,common_name, network_status,display_name)
	select lp.system_id , 'Loop',tp.longitude ,tp.latitude ,'A',tp.sp_geometry ,'','',tp.created_by ,'','P',''
	from att_details_loop lp
	join backbone_plan_network_details tp on tp.system_id = lp.backbone_system_id
	where lp.system_id not in (select system_id 
	from point_master where entity_type='Loop');

if exists(select 1 from backbone_plan_network_details where is_loop_required = true and plan_id = p_plan_id) then
PERFORM fn_backbone_update_network_code('att_details_loop', 'Loop',p_plan_id::text, 'backbone planning', 'loop_name' );
end if;

UPDATE att_details_loop lp
SET associated_system_id = pole.system_id,
    associated_network_id = pole.network_id
FROM att_details_pole pole
WHERE pole.backbone_system_id = lp.backbone_system_id
  AND pole.source_ref_id = lp.source_ref_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar;

 UPDATE att_details_loop lp
SET associated_system_id = mh.system_id,
    associated_network_id = mh.network_id
FROM att_details_manhole mh
WHERE mh.backbone_system_id = lp.backbone_system_id
  AND mh.source_ref_id = lp.source_ref_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar;

UPDATE att_details_loop lp
SET cable_system_id = cbl.system_id
FROM line_master cbl
WHERE cbl.entity_type = 'Cable'
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar
  AND cbl.source_ref_id = p_plan_id::varchar
  AND cbl.source_ref_type = 'backbone planning'
  AND ST_DWithin(
        cbl.sp_geometry::geography,
        ST_SetSRID(ST_MakePoint(lp.longitude, lp.latitude), 4326)::geography,
        0.5
      );

UPDATE att_details_cable lp
SET lp.loop_length = cbl.loop_length 
FROM att_details_loop cbl
WHERE cbl.system_id  = cbl.cable_system_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar
  AND cbl.source_ref_id = p_plan_id::varchar
  AND cbl.source_ref_type = 'backbone planning';
    

END
$function$
;

-------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_save_plan(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, backbone_plan_id integer)
 RETURNS TABLE(status boolean, message character varying, v_plan_id integer)
 LANGUAGE plpgsql
AS $function$

DECLARE
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
v_arow_cable record;
v_line_geom geometry;
v_region_province record;
v_arow_closure record;
v_arow_prev_closure record;
v_line_total_length DOUBLE PRECISION;
p_rfs_type character varying;
--backbone_plan_id integer;
v_temp_point_entity record;
v_geom_str character varying;
p_cable_type character varying;
p_skip_last_sc  integer;
p_skip_last_manhole_pole  integer;
v_geom geometry;
    v_result TEXT;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';

create temp table temp_generate_network_code(
status boolean,
network_id character varying,
entity_type character varying,
sequence_id integer,
province_id integer,
network_code character varying,
parent_entity_type character varying
) on commit drop;

create temp table temp_point_geom(
entity_type character varying,
sp_geometry character varying
) on commit drop;

select system_id into p_skip_last_manhole_pole  from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole','Pole') and planned_cable_entity = 'BackBone Cable' order by system_id desc limit 1;

select system_id into p_skip_last_sc  from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('SpliceClosure') and planned_cable_entity = 'BackBone Cable' order by system_id desc limit 1;
EXECUTE 'ALTER TABLE point_master DISABLE TRIGGER ALL';

----- insert manhole
INSERT INTO att_details_manhole(is_visible_on_map,ownership_type,
longitude, latitude, 
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, 
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,category,backbone_system_id)

SELECT true,'Own',
longitude,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id,region_id ,province_id,'Manhole' ,system_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and sp_geometry is not NULL order by system_id;
 
INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'Manhole',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.network_id,backbone_plan_id,'backbone planning'
    from att_details_manhole mh
	left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='Manhole' where pm.system_id is null;
   
	 UPDATE att_details_manhole adm
	SET 
	    specification = itm.specification,
	    item_code = itm.code,
	    audit_item_master_id = itm.audit_id,
	    vendor_id = itm.vendor_id
	FROM (
	    SELECT *
	    FROM item_template_master
	    WHERE category_reference = 'Manhole'
	      AND specification = 'Generic'
	    LIMIT 1
	) itm
	WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

---------insert sc

INSERT INTO att_details_spliceclosure (is_visible_on_map,ownership_type,
longitude, latitude, 
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, 
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,category,backbone_system_id)

SELECT true,'Own',
longitude ,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id,region_id ,province_id ,'SpliceClosure',system_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('SpliceClosure') and sp_geometry is not NULL order by system_id;

 
INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'SpliceClosure',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.network_id,backbone_plan_id,'backbone planning'
    from att_details_spliceclosure mh
	left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='SpliceClosure' where pm.system_id is null;

 
	 UPDATE att_details_spliceclosure adm
	SET 
	    specification = itm.specification,
	    item_code = itm.code,
	    audit_item_master_id = itm.audit_id,
	    vendor_id = itm.vendor_id,
	    no_of_ports = itm.no_of_port,
	    no_of_input_port= itm.no_of_input_port,
	    no_of_output_port = itm.no_of_output_port
	FROM (
	    SELECT *
	    FROM item_template_master
	    WHERE category_reference = 'SpliceClosure'
	      AND specification = 'generic'
	    LIMIT 1
	) itm
	WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

-----------insert pole

INSERT INTO att_details_pole(is_visible_on_map,ownership_type,
longitude, latitude, 
item_code, vendor_id, type, brand, model, construction, activation,accessibility, status,
created_by, created_on,network_status, 
project_id, planning_id, purpose_id, workorder_id,source_ref_type,source_ref_id,region_id,province_id,pole_type,category,backbone_system_id)

SELECT true,'Own',
longitude ,latitude ,
null, 0, 0, 0, 0, 0, 0,0, 'A',p_user_id, now(),'P'
, 0, 0, 0, 0,'backbone planning',backbone_plan_id::varchar,region_id ,province_id ,'Electrical','Pole',system_id 
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Pole') and sp_geometry is not NULL order by system_id;

 
INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select mh.system_id,'Pole',mh.longitude,mh.latitude, 'A', ST_GEOMFROMTEXT('POINT('||mh.longitude||' '||mh.latitude||')', 4326),
now(),'NA','NA',p_user_id,mh.network_id, 'P',mh.network_id,backbone_plan_id::varchar,'backbone planning' 
    from att_details_pole mh
	left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='Pole' where pm.system_id is null;

   
	 UPDATE att_details_pole adm
	SET 
	    specification = itm.specification,
	    item_code = itm.code,
	    audit_item_master_id = itm.audit_id,
	    vendor_id = itm.vendor_id
	FROM (
	    SELECT *
	    FROM item_template_master
	    WHERE category_reference = 'Pole'
	      AND specification = 'Generic'
	    LIMIT 1
	) itm
	WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

----------- insert cable
INSERT INTO ATT_DETAILS_CABLE(is_visible_on_map,total_core,no_of_tube,no_of_core_per_tube,cable_measured_length,cable_calculated_length,
	cable_type,specification,category,subcategory1,subcategory2,subcategory3,item_code,	
	vendor_id,network_status,status,created_by,created_on,cable_category,start_reading,	
	end_reading,structure_id,utilization,duct_id,trench_id,is_used,source_ref_type,source_ref_id,ownership_type,region_id,province_id,backbone_system_id,a_system_id,b_system_id)
select true,0,0,0,ST_Length(plan.line_sp_geometry,false),
ST_Length(plan.line_sp_geometry,false),plan.cable_type  ,plan.fiber_type,'Cable', null, null, null,null,
	0,'P','A',p_user_id,now(),case when plan.planned_cable_entity = 'BackBone Cable' then 'BackBone Cable' else 'Feeder Cable' end,
	0,0,0,'L',0,0,true,'backbone planning',backbone_plan_id,'OWN',region_id,province_id , plan.system_id,0,0 
from backbone_plan_network_details plan where plan.plan_id = backbone_plan_id and plan.created_by = p_user_id and plan.entity_type = 'Cable' and plan.line_sp_geometry  is not null order by system_id;

EXECUTE 'ALTER TABLE line_master DISABLE TRIGGER ALL';

INSERT INTO LINE_MASTER(system_id,common_name ,display_name ,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,network_status,is_virtual,source_ref_type,source_ref_id,sp_geometry,status)
select cbl.system_id,cbl.network_id,cbl.network_id,'Cable','A','','', p_user_id,0 ,'P',false,'backbone planning',backbone_plan_id , np.line_sp_geometry ,'A'
from ATT_DETAILS_CABLE cbl 
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id
where cbl.system_id not in (select system_id from line_master where entity_type='Cable') and np.entity_type = 'Cable' and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning' and np.line_sp_geometry is not null;

 UPDATE att_details_cable adm
	SET 
	    specification = itm.specification,
	    item_code = itm.code,
	    audit_item_master_id = itm.audit_id,
	    vendor_id = itm.vendor_id,
	    total_core = itm.other::integer,
	    no_of_tube = itm.no_of_tube,
	    no_of_core_per_tube = itm.no_of_core_per_tube
from    
	(select adm.system_id ,itm.* FROM att_details_cable adm
	join item_template_master itm
	    on category_reference = 'Cable'
	      AND '(' || code || ')' || other = adm.specification  
	      where adm.source_ref_id = backbone_plan_id::varchar
  AND adm.source_ref_type = 'backbone planning'
	) itm
	WHERE adm.system_id = itm.system_id;
	
----------- insert trench
if is_create_trench = true then
insert into att_details_trench(is_visible_on_map,trench_name,trench_width,trench_height,customer_count,trench_type,no_of_ducts,network_status,status,pin_code,province_id,region_id,construction,activation,accessibility,category,subcategory1,subcategory2,subcategory3,vendor_id,type,
	brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,mcgm_ward,strata_type,
	is_used,source_ref_type,source_ref_id,ownership_type,backbone_system_id,a_system_id,b_system_id)
	select true,'',0,0,0,'HDD',0,'P','A','',province_id,region_id,0,0,0,
	'Trench' ,null,null,null, 0,0,0,0,'',p_user_id,now(),0 ,0,0,0,'','',false,'backbone planning',backbone_plan_id,'OWN',tp.system_id ,0,0
	from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and tp.line_sp_geometry is not null and created_by = p_user_id;
	
	INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name,source_ref_id,source_ref_type,sp_geometry,status)
	select cbl.system_id,'Trench','A','','', p_user_id,0 ,'','P',false,'',backbone_plan_id,'backbone planning',np.line_sp_geometry,'A' 
from att_details_trench cbl 
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id where cbl.system_id not in (select system_id from line_master where entity_type='Trench') and np.entity_type in ('Manhole') and np.line_sp_geometry is not null and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';	

 UPDATE att_details_trench adm
	SET 
	    specification = itm.specification,
	    item_code = itm.code,
	    audit_item_master_id = itm.audit_id,
	    vendor_id = itm.vendor_id
	    FROM (
	    SELECT *
	    FROM item_template_master
	    WHERE category_reference = 'Trench'
	      AND specification = 'generic'
	    LIMIT 1
	) itm
	WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';

end if;

-------- insert duct
if is_create_duct = true then
insert into att_details_duct(is_visible_on_map,duct_name,calculated_length,manual_length,network_status,
	status,pin_code,province_id,region_id,utilization,no_of_cables,offset_value,construction,activation,accessibility,specification,category,subcategory1,subcategory2,
	subcategory3,item_code,vendor_id,type,brand,model,remarks,created_by,created_on,project_id,planning_id,purpose_id,workorder_id,inner_dimension,outer_dimension,
	is_used,source_ref_type,source_ref_id,trench_id,ownership_type,backbone_system_id,a_system_id,b_system_id)
	select true,'',ST_Length(tp.line_sp_geometry,false),ST_Length(tp.line_sp_geometry,false),'P','A','',province_id,region_id,'L',0,0,0,0,0,'','Duct',null,null,null,'',0,0,0,0,'',p_user_id,now(),0 ,0,0,0,0,0,false,'backbone planning',backbone_plan_id,0,'OWN',tp.system_id ,0,0
	from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and created_by = p_user_id;
	
	INSERT INTO LINE_MASTER(system_id,entity_type,approval_flag,creator_remark,approver_remark,created_by,approver_id,common_name,network_status,is_virtual,display_name,source_ref_id,source_ref_type,sp_geometry,status)
	select cbl.system_id,'Duct','A','','', p_user_id,0 ,'','P',false,'',backbone_plan_id,'backbone planning' ,np.line_sp_geometry ,'A'
from att_details_duct cbl 
join backbone_plan_network_details np on np.system_id = cbl.backbone_system_id where cbl.system_id not in (select system_id from line_master where entity_type='Duct') and np.entity_type ='Manhole' and np.line_sp_geometry is not null and source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';	

 UPDATE att_details_duct adm
	SET 
	    specification = itm.specification,
	    item_code = itm.code,
	    audit_item_master_id = itm.audit_id,
	    vendor_id = itm.vendor_id
	    FROM (
	    SELECT *
	    FROM item_template_master
	    WHERE category_reference = 'Duct'
	      AND specification = 'generic'
	    LIMIT 1
	) itm
	WHERE adm.source_ref_id = backbone_plan_id::varchar and source_ref_type = 'backbone planning';
end if;

   FOR v_temp_point_entity IN 
        SELECT province_id 
        FROM backbone_plan_network_details 
        WHERE plan_id = backbone_plan_id 
          AND created_by = p_user_id 
          AND province_id IS NOT NULL 
        GROUP BY province_id
    loop

	INSERT INTO temp_point_geom(entity_type, sp_geometry)
SELECT 'Manhole', geom_text FROM (
    SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
    FROM backbone_plan_network_details
    WHERE province_id = v_temp_point_entity.province_id
      AND plan_id = backbone_plan_id
      AND created_by = p_user_id
      AND entity_type = 'Manhole'
      and system_id not in (p_skip_last_manhole_pole)
      AND sp_geometry IS NOT NULL
    LIMIT 1
) AS manhole_geom

UNION ALL

SELECT 'Pole', geom_text FROM (
    SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
    FROM backbone_plan_network_details
    WHERE province_id = v_temp_point_entity.province_id
      AND plan_id = backbone_plan_id
      AND created_by = p_user_id
      AND entity_type = 'Pole'
      and system_id not in (p_skip_last_manhole_pole)
      AND sp_geometry IS NOT NULL
    LIMIT 1
) AS pole_geom

UNION ALL

SELECT 'Loop', geom_text FROM (
    SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
    FROM backbone_plan_network_details
    WHERE province_id = v_temp_point_entity.province_id
      AND plan_id = backbone_plan_id
      AND created_by = p_user_id and is_loop_required = true and loop_length > 0
      AND entity_type in ('Manhole','Pole')
      and system_id not in (p_skip_last_manhole_pole)
      AND sp_geometry IS NOT NULL
    LIMIT 1
) AS loop_geom

 UNION ALL

SELECT 'SpliceClosure', geom_text FROM (
    SELECT DISTINCT substring(left(ST_AsText(sp_geometry), -1), 7) AS geom_text
    FROM backbone_plan_network_details
    WHERE province_id = v_temp_point_entity.province_id
      AND plan_id = backbone_plan_id
      AND created_by = p_user_id
      AND entity_type IN ('SpliceClosure')
      and system_id not in (p_skip_last_sc)
      AND sp_geometry IS NOT NULL
    LIMIT 1
) AS spliceclosure_geom;

	 --- generaTE NETWORK CODE FOR POINT ENTITY   
	INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id, network_code, parent_entity_type)
	SELECT f.status, f.message, t.entity_type, f.o_sequence_id, v_temp_point_entity.province_id, f.o_p_network_id, f.o_p_entity_type
	FROM temp_point_geom t,
	LATERAL fn_get_clone_network_code(
	    t.entity_type,
	    'Point',
	    t.sp_geometry,
	    0,
	    ''
	) AS f
	WHERE t.sp_geometry IS NOT NULL;

delete from temp_generate_network_code cc where cc.status = false;
 
-------------------update network id 
PERFORM fn_backbone_update_network_code('att_details_manhole', 'Manhole',backbone_plan_id::text , 'backbone planning','manhole_name');
PERFORM fn_backbone_update_network_code('att_details_pole', 'Pole',backbone_plan_id::text, 'backbone planning', 'pole_name');
PERFORM fn_backbone_update_network_code('att_details_spliceclosure', 'SpliceClosure',backbone_plan_id::text, 'backbone planning', 'spliceclosure_name' );

---- generate network code cable

SELECT line_sp_geometry 
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
  AND plan_id = backbone_plan_id
  AND created_by = p_user_id
  AND entity_type in ('Cable')
  AND line_sp_geometry  IS NOT NULL
LIMIT 1;

if v_geom is not null
then 
  select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

 INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Cable', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Cable','','',v_geom_str,'OSP') AS f;

PERFORM fn_backbone_update_network_code('att_details_cable', 'Cable', backbone_plan_id::text, 'backbone planning','cable_name');

end if;

--- generate network codes for trench and duct
if is_create_trench = true then
SELECT line_sp_geometry 
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
  AND plan_id = backbone_plan_id
  AND created_by = p_user_id
  AND entity_type in ('Manhole')
  AND line_sp_geometry  IS NOT NULL
LIMIT 1;

if v_geom is not null
then 
  select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

 INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Trench', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Trench','','',v_geom_str,'OSP'::character varying,0,'') as f;

PERFORM fn_backbone_update_network_code('att_details_trench', 'Trench', backbone_plan_id::text, 'backbone planning','trench_name');
end if;
end if;

if is_create_duct = true
then
SELECT line_sp_geometry 
INTO v_geom
FROM backbone_plan_network_details
WHERE province_id = v_temp_point_entity.province_id
  AND plan_id = backbone_plan_id
  AND created_by = p_user_id
  AND entity_type in ('Manhole') 
  AND line_sp_geometry  IS NOT NULL
LIMIT 1;

if v_geom is not null
then 
  select substring(left(St_astext(v_geom),-1),12) ::character varying INTO v_geom_str;

 INSERT INTO temp_generate_network_code (status, network_id, entity_type, sequence_id, province_id,network_code,parent_entity_type)
SELECT true, f.network_code, 'Duct', f.sequence_id::integer, v_temp_point_entity.province_id,f.parent_network_id,f.parent_entity_type
FROM fn_get_line_network_code('Duct','','',v_geom_str,'OSP'::character varying,0,'') as f;

PERFORM fn_backbone_update_network_code('att_details_duct', 'Duct', backbone_plan_id::text, 'backbone planning','duct_name');

end if;
end if;

END LOOP;

 
   INSERT INTO routing_data_core_plan (id, system_id, source, target, cost, reverse_cost, geom)
    SELECT 
  lm.system_id ,lm.system_id,null as source,null as target,
        ST_Length(lm.sp_geometry::geography) / 1000 AS cost,
        ST_Length(lm.sp_geometry::geography) / 1000 AS reverse_cost,
        ST_Force2D(lm.sp_geometry) AS geom
    FROM line_master lm    
    WHERE lm.entity_type = 'Cable'
    AND lm.source_ref_id = backbone_plan_id::varchar and lm.source_ref_type ='backbone planning';

            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for INSERT' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for INSERT' AS message;
            END IF;
           
PERFORM(fn_backbone_get_loop_entity(backbone_plan_id));

 EXECUTE 'ALTER TABLE line_master ENABLE TRIGGER ALL';
 EXECUTE 'ALTER TABLE point_master ENABLE TRIGGER ALL';

V_MESSAGE:='BackBone network plan processed successfully!';
update backbone_plan_details t set status = 'Completed' where t.plan_id = backbone_plan_id and created_by = p_user_id;

RETURN QUERY SELECT V_IS_VALID::boolean AS STATUS, V_MESSAGE::CHARACTER VARYING AS MESSAGE,backbone_plan_id :: integer as v_plan_id ;

end;
$function$
;

---------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_loop_entity(p_plan_id integer)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
v_current_point_geom geometry;
v_temp_point_entity record;
v_loop_network_id record;
v_line_geom geometry;
v_arow_loop record;
BEGIN

	INSERT INTO public.att_details_loop(
	network_id, loop_length, associated_system_id, associated_network_id, 
	associated_entity_type, cable_system_id, created_by, created_on, 
	network_status, start_reading, end_reading, 
	parent_system_id, sequence_id, province_id, region_id, parent_network_id, 
	parent_entity_type, status, 
	is_visible_on_map, source_ref_type, source_ref_id, 
	source_ref_description, latitude, longitude, is_new_entity, backbone_system_id)
	select '',loop_length,entity_system_id,entity_network_id,
	entity_type,0,created_by,now(),
	'P',0,0,0, 0,province_id, region_id,'','',
	'A',true,'backbone planning', plan_id::varchar,'',latitude,longitude,true,tp.system_id 
	from backbone_plan_network_details tp where entity_type in ('Pole','Manhole') 
	and is_loop_required = true and loop_length >0 and sp_geometry is not null and plan_id = p_plan_id 
	order by system_id;

	INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,
	creator_remark,approver_remark,created_by,common_name, network_status,display_name)
	select lp.system_id , 'Loop',tp.longitude ,tp.latitude ,'A',tp.sp_geometry ,'','',tp.created_by ,'','P',''
	from att_details_loop lp
	join backbone_plan_network_details tp on tp.system_id = lp.backbone_system_id
	where lp.system_id not in (select system_id 
	from point_master where entity_type='Loop');

if exists(select 1 from backbone_plan_network_details where is_loop_required = true and plan_id = p_plan_id) then
PERFORM fn_backbone_update_network_code('att_details_loop', 'Loop',p_plan_id::text, 'backbone planning', 'loop_name' );
end if;

UPDATE att_details_loop lp
SET associated_system_id = pole.system_id,
    associated_network_id = pole.network_id
FROM att_details_pole pole
WHERE pole.backbone_system_id = lp.backbone_system_id
  AND pole.source_ref_id = lp.source_ref_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar;

 UPDATE att_details_loop lp
SET associated_system_id = mh.system_id,
    associated_network_id = mh.network_id
FROM att_details_manhole mh
WHERE mh.backbone_system_id = lp.backbone_system_id
  AND mh.source_ref_id = lp.source_ref_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar;

UPDATE att_details_loop lp
SET cable_system_id = cbl.system_id
FROM line_master cbl
WHERE cbl.entity_type = 'Cable'
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar
  AND cbl.source_ref_id = p_plan_id::varchar
  AND cbl.source_ref_type = 'backbone planning'
  AND ST_DWithin(
        cbl.sp_geometry::geography,
        ST_SetSRID(ST_MakePoint(lp.longitude, lp.latitude), 4326)::geography,
        0.2
      );
	  
 UPDATE att_details_cable lp
SET total_loop_length = sub.total_loop_length,
    total_loop_count  = sub.total_loop_count
FROM (
    SELECT cable_system_id,
           SUM(loop_length) AS total_loop_length,
           COUNT(*) AS total_loop_count
    FROM att_details_loop
    WHERE source_ref_id = p_plan_id::varchar
      AND source_ref_type = 'backbone planning'
    GROUP BY cable_system_id
) sub
WHERE lp.system_id = sub.cable_system_id
  AND lp.source_ref_id = p_plan_id::varchar
  AND lp.source_ref_type = 'backbone planning';


END
$function$
;

--------------------------------------------------------------------------------------------

alter table backbone_plan_network_details add column p_plan_name character varying null;

-------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_validate_entitycreation_area(
    p_geom text,
    p_userid integer,
    p_selectiontype character varying,
    p_ticket_id integer)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000
AS $BODY$
DECLARE
    result text;
    result_status bool;
    v_province_id integer;
    v_geom geometry;
BEGIN
    -- Convert geometry based on selection type
    IF lower(p_selectiontype) = 'polygon' OR lower(p_selectiontype) = 'circle' THEN
        v_geom := ST_GeomFromText('POLYGON(('||p_geom||'))', 4326);
    ELSIF lower(p_selectiontype) = 'line' THEN
        v_geom := ST_GeomFromText('LINESTRING('||p_geom||')', 4326);
    ELSE
        v_geom := ST_GeomFromText('POINT('||p_geom||')', 4326);
    END IF;

    -- Province permission check
    IF EXISTS (
        SELECT 1
        FROM province_boundary pb
        JOIN vw_user_permission_area pa ON pb.id = pa.province_id
        WHERE pa.user_id = p_userid
          AND ST_Intersects(pb.sp_geometry, v_geom)
    ) THEN
        result := 'Success';
        result_status := true;
    ELSE
        result := 'Selected area not exists in permitted region/province. Please contact your administrator !';
        result_status := false;
    END IF;

    -- Ticket-based validation
    IF result_status THEN
        IF p_ticket_id > 0 THEN
            -- If ticket geometry exists in polygon_master
            IF EXISTS (
                SELECT 1 FROM polygon_master
                WHERE system_id = p_ticket_id AND UPPER(entity_type) = 'NETWORK_TICKET'
            ) THEN
                -- ✅ Strict containment check (full geometry must be inside ticket)
                IF EXISTS (
                    SELECT 1
                    FROM polygon_master pm
                    WHERE pm.system_id = p_ticket_id
                      AND UPPER(pm.entity_type) = 'NETWORK_TICKET'
                      AND ST_CoveredBy(v_geom, pm.sp_geometry)  -- full containment
                ) THEN
                    result := 'Success';
                    result_status := true;
                ELSE
                    IF lower(p_selectiontype) = 'line' THEN
                        result := 'Trench line must be fully inside the assigned ticket area. Please contact your administrator !';
                    ELSE
                        result := 'Entity cannot be created outside the assigned ticket area. Please contact your administrator !';
                    END IF;
                    result_status := false;
                END IF;

            ELSE
                -- Province based ticket validation
                SELECT province_id INTO v_province_id
                FROM att_details_networktickets
                WHERE ticket_id = p_ticket_id;

                IF v_province_id = 0 THEN
                    result := 'Success';
                    result_status := true;
                ELSE
                    IF EXISTS (SELECT 1 FROM province_boundary WHERE id = v_province_id) THEN
                        IF EXISTS (
                            SELECT 1
                            FROM province_boundary pb
                            WHERE pb.id = v_province_id
                              AND ST_CoveredBy(v_geom, pb.sp_geometry) -- full containment
                        ) THEN
                            result := 'Success';
                            result_status := true;
                        ELSE
                            IF lower(p_selectiontype) = 'line' THEN
                                result := 'Line Entity must be fully inside the assigned ticket area. Please contact your administrator !';
                            ELSE
                                result := 'Entity cannot be created outside the assigned ticket area. Please contact your administrator !';
                            END IF;
                            result_status := false;
                        END IF;
                    END IF;
                END IF;
            END IF;
        END IF;
    END IF;

    RETURN QUERY
        SELECT result_status AS status, result::character varying AS message;
END;
$BODY$;

-------------------------------------------------------------------------------------------------------------



UPDATE layer_action_mapping
SET action_title='Add FTC', res_field_key='Add FTC'
WHERE layer_id= (select layer_id  from layer_details ld where layer_name = 'WallMount') and action_name = 'AddBDB';

UPDATE layer_action_mapping
SET action_title='Add FTC', res_field_key='Add FTC'
WHERE layer_id= (select layer_id  from layer_details ld where layer_name = 'Pole') and action_name = 'AddBDB';

-----------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_getnearbyentities(lat double precision, lng double precision, mtrbuffer integer, p_user_id integer, p_source_ref_id character varying, p_source_ref_type character varying)
 RETURNS TABLE(geom_type character varying, entity_type character varying, entity_title character varying, system_id integer, common_name character varying, geom text, centroid_geom text, network_status character varying, display_name character varying, total_core character varying)
 LANGUAGE plpgsql
AS $function$

declare 
v_role_id integer;
v_geometry_with_buffer geometry;
v_geometry geometry;
BEGIN
	v_geometry = ST_GEOMFROMTEXT('POINT('||LNG||' '||LAT||')',4326);
	v_geometry_with_buffer = ST_BUFFER_METERS(v_geometry,MTRBUFFER);
		Raise info 'v_geometry ->%',v_geometry;
		Raise info 'v_geometry_with_buffer ->%',v_geometry_with_buffer;

	SELECT ROLE_ID INTO V_ROLE_ID FROM USER_MASTER WHERE USER_ID=P_USER_ID;

	CREATE TEMP TABLE TEMP_REGION_PROVINCE(
	REGION_ID INTEGER,
	PROVINCE_ID INTEGER
	) ON COMMIT DROP;

	CREATE TEMP TABLE TEMP_POINT_MASTER(
	SYSTEM_ID INTEGER,ENTITY_TYPE CHARACTER VARYING,LONGITUDE DOUBLE PRECISION,LATITUDE DOUBLE PRECISION,APPROVAL_FLAG CHARACTER VARYING,SP_GEOMETRY geometry,APPROVAL_DATE TIMESTAMP WITHOUT TIME ZONE,CREATOR_REMARK TEXT,APPROVER_REMARK TEXT,CREATED_BY INTEGER,APPROVER_ID INTEGER,COMMON_NAME CHARACTER VARYING,DB_FLAG INTEGER,MODIFIED_ON TIME WITHOUT TIME ZONE,NETWORK_STATUS CHARACTER VARYING,NO_OF_PORTS CHARACTER VARYING,IS_VIRTUAL BOOLEAN,ENTITY_CATEGORY CHARACTER VARYING,IS_BURIED BOOLEAN,DISPLAY_NAME CHARACTER VARYING,MODIFIED_BY INTEGER,IS_PROCESSED BOOLEAN,GIS_DESIGN_ID CHARACTER VARYING,ST_X DOUBLE PRECISION,ST_Y DOUBLE PRECISION
	)ON COMMIT DROP;
	
	--CREATE INDEX IF NOT EXISTS TEMP_POINT_MASTER_system_id_entity_type_idx ON public.TEMP_POINT_MASTER USING btree( system_id, upper(entity_type) COLLATE pg_catalog."default" ASC NULLS LAST) TABLESPACE pg_default;
	

	CREATE TEMP TABLE TEMP_LINE_MASTER(
	system_id integer,entity_type character varying,approval_flag character varying,sp_geometry geometry,creator_remark text,approver_remark text,created_by integer,approver_id integer,common_name character varying,db_flag integer,approval_date timestamp without time zone,modified_on timestamp without time zone,network_status character varying,is_virtual boolean,display_name character varying,modified_by integer,is_processed boolean,gis_design_id character varying
	)ON COMMIT DROP;

	CREATE TEMP TABLE TEMP_POLYGON_MASTER(
	SYSTEM_ID INTEGER,APPROVAL_DATE TIMESTAMP WITHOUT TIME ZONE,MODIFIED_ON TIMESTAMP WITHOUT TIME ZONE,IS_VIRTUAL BOOLEAN,CENTER_LINE_GEOM geometry,BUFFER_WIDTH DOUBLE PRECISION,MODIFIED_BY INTEGER,SP_CENTROID geometry,IS_PROCESSED BOOLEAN,SP_GEOMETRY geometry,CREATED_BY INTEGER,APPROVER_ID INTEGER,DB_FLAG INTEGER,ENTITY_TYPE CHARACTER VARYING,APPROVAL_FLAG CHARACTER VARYING,DISPLAY_NAME CHARACTER VARYING,CREATOR_REMARK TEXT,APPROVER_REMARK TEXT,GIS_DESIGN_ID CHARACTER VARYING,NETWORK_STATUS CHARACTER VARYING,COMMON_NAME CHARACTER VARYING
	) ON COMMIT DROP;
	
	Raise info 'Query Start to Insert Point Entity ->%', clock_timestamp();
	INSERT INTO TEMP_POINT_MASTER(SYSTEM_ID,ENTITY_TYPE,LONGITUDE,LATITUDE,APPROVAL_FLAG,SP_GEOMETRY,APPROVAL_DATE,CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,APPROVER_ID,COMMON_NAME,DB_FLAG,MODIFIED_ON,NETWORK_STATUS,NO_OF_PORTS,IS_VIRTUAL,ENTITY_CATEGORY,IS_BURIED,DISPLAY_NAME,
	MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID,ST_X,ST_Y)
	
	SELECT PTM.SYSTEM_ID,PTM.ENTITY_TYPE,PTM.LONGITUDE,PTM.LATITUDE,APPROVAL_FLAG,coalesce(draftinfo.updated_geom,PTM.SP_GEOMETRY),APPROVAL_DATE,
	CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,APPROVER_ID,PTM.COMMON_NAME,DB_FLAG,MODIFIED_ON,PTM.NETWORK_STATUS,
	NO_OF_PORTS,IS_VIRTUAL,ENTITY_CATEGORY,IS_BURIED,PTM.DISPLAY_NAME,MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID,ST_X,ST_Y
	FROM POINT_MASTER PTM 
	LEFT JOIN --lateral
	(
		select draftinfo.* 
		from att_details_edit_entity_info draftinfo  
		inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id 
		and opr.description ='EL' --and draftinfo.entity_system_id = PTM.system_id and upper(draftinfo.entity_type)=upper(PTM.entity_type)
	) draftinfo	on draftinfo.entity_system_id = PTM.system_id and upper(draftinfo.entity_type)=upper(PTM.entity_type)
	WHERE ST_WITHIN(coalesce(draftinfo.updated_geom,PTM.SP_GEOMETRY), v_geometry_with_buffer) ;
   --Commented out since entities created from the application are not visible in Info----------
    --and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(PTM.status,'A') = 'A' end;
	--Commented out since entities created from the application are not visible in Info----------
	
	
	
	/*SELECT PTM.SYSTEM_ID,PTM.ENTITY_TYPE,PTM.LONGITUDE,PTM.LATITUDE,APPROVAL_FLAG,PTM.SP_GEOMETRY,APPROVAL_DATE,
	CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,APPROVER_ID,PTM.COMMON_NAME,DB_FLAG,MODIFIED_ON,PTM.NETWORK_STATUS,
	NO_OF_PORTS,IS_VIRTUAL,ENTITY_CATEGORY,IS_BURIED,PTM.DISPLAY_NAME,MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID,ST_X,ST_Y
	FROM POINT_MASTER PTM
	WHERE ST_WITHIN(PTM.SP_GEOMETRY,ST_BUFFER_METERS(ST_GEOMFROMTEXT('POINT('||LNG||' '||LAT||')',4326),MTRBUFFER));
	*/
	Raise info 'Query End to Insert Point Entity ->%', clock_timestamp();
	Raise info 'Query Start to Insert Line Entity ->%', clock_timestamp();
	
	INSERT INTO TEMP_LINE_MASTER(SYSTEM_ID,ENTITY_TYPE,APPROVAL_FLAG,SP_GEOMETRY,CREATOR_REMARK,
	APPROVER_REMARK,CREATED_BY,APPROVER_ID,COMMON_NAME,DB_FLAG,APPROVAL_DATE,MODIFIED_ON,NETWORK_STATUS,IS_VIRTUAL,DISPLAY_NAME,MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID)

	SELECT PTM.SYSTEM_ID,PTM.ENTITY_TYPE,PTM.APPROVAL_FLAG,coalesce(draftinfo.updated_geom,PTM.sp_geometry),CREATOR_REMARK,APPROVER_REMARK,CREATED_BY,APPROVER_ID,
	PTM.COMMON_NAME,DB_FLAG,APPROVAL_DATE,MODIFIED_ON,PTM.NETWORK_STATUS,IS_VIRTUAL,PTM.DISPLAY_NAME,MODIFIED_BY,IS_PROCESSED,GIS_DESIGN_ID 
	FROM LINE_MASTER PTM
	left join 
	(
		select draftinfo.* from att_details_edit_entity_info draftinfo  
		inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL'
	) draftinfo
	on draftinfo.entity_system_id=PTM.system_id and upper(draftinfo.entity_type)=upper(PTM.entity_type)
	WHERE ST_Intersects(st_makevalid(coalesce(draftinfo.updated_geom,PTM.sp_geometry)),v_geometry_with_buffer) ;
    
   -- and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(PTM.status,'A') = 'A' end;

	Raise info 'Query end to Insert Line Entity ->%', clock_timestamp();

	Raise info 'Query Start to Insert Polygon Entity ->%', clock_timestamp();
	INSERT INTO TEMP_POLYGON_MASTER(	
	SYSTEM_ID,APPROVAL_DATE,MODIFIED_ON,IS_VIRTUAL,CENTER_LINE_GEOM,BUFFER_WIDTH,
	MODIFIED_BY,SP_CENTROID,IS_PROCESSED,SP_GEOMETRY,CREATED_BY,APPROVER_ID,
	DB_FLAG,ENTITY_TYPE,APPROVAL_FLAG,DISPLAY_NAME,CREATOR_REMARK,APPROVER_REMARK,
	GIS_DESIGN_ID,NETWORK_STATUS,COMMON_NAME)
	SELECT POLY.SYSTEM_ID,APPROVAL_DATE,MODIFIED_ON,IS_VIRTUAL,CENTER_LINE_GEOM,
	BUFFER_WIDTH,MODIFIED_BY,SP_CENTROID,IS_PROCESSED,SP_GEOMETRY,CREATED_BY,APPROVER_ID,
	DB_FLAG,POLY.ENTITY_TYPE,APPROVAL_FLAG,POLY.DISPLAY_NAME,CREATOR_REMARK,APPROVER_REMARK,
	GIS_DESIGN_ID,POLY.NETWORK_STATUS,POLY.COMMON_NAME 
	FROM POLYGON_MASTER POLY
	WHERE ST_INTERSECTS(v_geometry_with_buffer,POLY.SP_GEOMETRY) ;
    --and case when coalesce(p_source_ref_id,'') !='' Then 1=1 else  coalesce(POLY.status,'A') = 'A' end;
    
	Raise info 'Query end to Insert Polygon Entity ->%', clock_timestamp();
	INSERT INTO TEMP_REGION_PROVINCE(REGION_ID,PROVINCE_ID)
	SELECT REGION_ID,ID FROM PROVINCE_BOUNDARY PROVINCE WHERE ST_WITHIN(v_geometry,PROVINCE.SP_GEOMETRY);

	  Raise info 'Query Start to return query ->%', clock_timestamp();
   RETURN QUERY
   select  tbl.geom_type::character varying as geom_type, tbl.entity_type,l.layer_title, tbl.system_id, tbl.common_name,tbl.geom,tbl.centroid_geom,
   case when l.is_network_entity  = true then tbl.network_status else '' end network_status,tbl.display_name::character varying,tbl.total_core from (
select *, row_number() over(partition by tbl1.system_id,tbl1.entity_type order by order_colum asc) as row_num 
       from (
    
	select 'Point' as geom_type, p.entity_type, p.system_id, p.common_name,
    ST_astext(p.sp_geometry) as geom,
	ST_astext(p.sp_geometry) as centroid_geom,
	coalesce(draftinfo1.ATTRIBUTE_INFO->>lower('curr_status'),p.network_status)::character varying as network_status,
	2 as order_colum,coalesce(draftinfo2.display_name,p.display_name) as display_name,
	coalesce(draftinfo2.ATTRIBUTE_INFO->>lower('no_of_core_per_tube'),p.no_of_ports)::character varying as total_core
	from TEMP_POINT_MASTER p
	inner join vw_user_permission_area upa on user_id=p_user_id
	inner join temp_region_province province on upa.region_id=province.region_id and upa.province_id=province.province_id 
	--inner join province_boundary province on ST_Within(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),province.sp_geometry) 	
	--left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	--inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL' 
	--and (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	--) draftinfo
	--on draftinfo.entity_system_id=p.system_id and upper(draftinfo.entity_type)=upper(p.entity_type)
	left join (select draftinfo1.* from att_details_edit_entity_info draftinfo1  
	inner join entity_operations_master opr1 on draftinfo1.entity_action_id=opr1.id and upper(opr1.description)='NS' 
	--and (draftinfo1.updated_by=p_user_id or (draftinfo1.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo1
	on draftinfo1.entity_system_id=p.system_id and upper(draftinfo1.entity_type)=upper(p.entity_type)
	left join (select draftinfo2.* from att_details_edit_entity_info draftinfo2  
	inner join entity_operations_master opr on draftinfo2.entity_action_id=opr.id and upper(opr.description)='ED') draftinfo2
	on draftinfo2.entity_system_id=p.system_id and upper(draftinfo2.entity_type)=upper(p.entity_type)
	where p.approval_flag <> 'U' and ST_WithIn(p.sp_geometry, ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer)) 
	--and lower(p.entity_type) != 'loop'
	union
	select 'Line' as geom_type, l.entity_type, l.system_id, l.common_name,ST_astext(l.sp_geometry) as geom,
	ST_astext(ST_Centroid(l.sp_geometry)) as centroid_geom,coalesce(draftinfo1.ATTRIBUTE_INFO->>lower('curr_status'),
	l.network_status)::character varying as network_status ,3 as order_colum,coalesce(draftinfo2.display_name,l.display_name) as display_name,
	coalesce(draftinfo2.ATTRIBUTE_INFO->>lower('total_core')||'F',cbl.total_core||'F')::character varying as total_core
        from temp_LINE_MASTER l 
        left join att_details_cable cbl on case when upper(l.entity_type)='CABLE' then l.system_id=cbl.system_id else 1=2 end
	inner join vw_user_permission_area upa on user_id=p_user_id
	inner join temp_region_province province on upa.region_id=province.region_id and upa.province_id=province.province_id 
	--left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	--inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL'  
	--and (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))

	--) draftinfo
	--on draftinfo.entity_system_id=l.system_id and upper(draftinfo.entity_type)=upper(l.entity_type)
	left join (select draftinfo1.* from att_details_edit_entity_info draftinfo1  
	inner join entity_operations_master opr1 on draftinfo1.entity_action_id=opr1.id and upper(opr1.description)='NS' 
	--and (draftinfo1.updated_by=p_user_id or (draftinfo1.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo1
	on draftinfo1.entity_system_id=l.system_id and upper(draftinfo1.entity_type)=upper(l.entity_type)
	left join (select draftinfo2.* from att_details_edit_entity_info draftinfo2  
	inner join entity_operations_master opr on draftinfo2.entity_action_id=opr.id and upper(opr.description)='ED') draftinfo2
	on draftinfo2.entity_system_id=l.system_id and upper(draftinfo2.entity_type)=upper(l.entity_type)
        where l.approval_flag <> 'U' and ST_Intersects(st_makevalid(l.sp_geometry), 
        ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer))
	union 
	select 'Polygon' as geom_type, poly.entity_type, poly.system_id, poly.common_name,ST_astext(coalesce(draftinfo.updated_geom,poly.sp_geometry)) as geom,
	ST_astext(ST_Centroid(coalesce(draftinfo.updated_geom,poly.sp_geometry))) as centroid_geom,
	coalesce(draftinfo1.ATTRIBUTE_INFO->>lower('curr_status'),poly.network_status)::character varying as network_status,1 as order_colum,
	coalesce(draftinfo2.display_name,poly.display_name) as display_name,null as total_core   from 
	TEMP_POLYGON_MASTER poly
	inner join vw_user_permission_area upa on user_id=p_user_id
	inner join temp_region_province province on upa.region_id=province.region_id and upa.province_id=province.province_id 
	left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL' 
	--and  (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo
	on draftinfo.entity_system_id=poly.system_id and upper(draftinfo.entity_type)=upper(poly.entity_type)
	left join (select draftinfo1.* from att_details_edit_entity_info draftinfo1  
	inner join entity_operations_master opr1 on draftinfo1.entity_action_id=opr1.id and upper(opr1.description)='NS' 
	--and (draftinfo1.updated_by=p_user_id or (draftinfo1.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo1
	on draftinfo.entity_system_id=poly.system_id and upper(draftinfo.entity_type)=upper(poly.entity_type)
	left join (select draftinfo2.* from att_details_edit_entity_info draftinfo2  
	inner join entity_operations_master opr on draftinfo2.entity_action_id=opr.id and upper(opr.description)='ED') draftinfo2
	on draftinfo2.entity_system_id=poly.system_id and upper(draftinfo2.entity_type)=upper(poly.entity_type)
	where poly.approval_flag <> 'U' and 
	ST_Intersects(ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer),coalesce(draftinfo.updated_geom,poly.sp_geometry)) 
	and case when upper(poly.entity_type)=upper('building') and poly.is_virtual=true then 1=2 else 1=1 end
	union 
	select 'Polygon' as geom_type,'Province' as entity_type, province.id as system_id, province.province_name||'('||province_abbreviation||')' as common_name,ST_astext(province.sp_geometry) as geom,
	ST_astext(ST_Centroid(province.sp_geometry)) as centroid_geom,
	'A' as network_status,1 as order_colum,province.province_name||'('||province_abbreviation||')' as display_name, null as total_core   
	from province_boundary province
	inner join temp_region_province tmpprovince on province.region_id=tmpprovince.region_id and province.id=tmpprovince.province_id
	inner join vw_user_permission_area upa on user_id=p_user_id and upa.region_id=tmpprovince.region_id and upa.province_id=tmpprovince.province_id
	 
	--left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	--inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL'  
	--and (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	--) draftinforo
	
	--on draftinfo.entity_system_id=province.id and upper(draftinfo.entity_type)=upper('Province')
	--inner join vw_user_permission_area upa on ST_Within(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),coalesce(draftinfo.updated_geom,province.sp_geometry)) and upa.region_id=province.region_id and upa.province_id=province.id and user_id=p_user_id	 
	union
	select 'Circle' as geom_type, poly.entity_type, poly.system_id, poly.common_name,ST_astext(coalesce(draftinfo.updated_geom,poly.sp_geometry)) as geom,
	ST_astext(ST_Centroid(coalesce(draftinfo.updated_geom,poly.sp_geometry))) as centroid_geom,
	coalesce(draftinfo1.ATTRIBUTE_INFO->>lower('curr_status'),poly.network_status)::character varying as network_status,1 as order_colum,poly.display_name||'-('||coalesce(draftinfo1.ATTRIBUTE_INFO->>'CURR_STATUS',poly.network_status)||')' as display_name, null as total_core   from circle_master poly 
	inner join vw_user_permission_area upa on user_id=p_user_id
	inner join temp_region_province province on upa.region_id=province.region_id and upa.province_id=province.province_id  
	left join (select draftinfo.* from att_details_edit_entity_info draftinfo  
	inner join entity_operations_master opr on draftinfo.entity_action_id=opr.id and upper(opr.description)='EL' 
	--and  (draftinfo.updated_by=p_user_id or (draftinfo.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo
	on draftinfo.entity_system_id=poly.system_id and upper(draftinfo.entity_type)=upper(poly.entity_type)
	left join (select draftinfo1.* from att_details_edit_entity_info draftinfo1  
	inner join entity_operations_master opr1 on draftinfo1.entity_action_id=opr1.id and upper(opr1.description)='NS' 
	--and (draftinfo1.updated_by=p_user_id or (draftinfo1.updated_by in (select user_id from user_master where manager_id=p_user_id)))
	) draftinfo1
	on draftinfo1.entity_system_id=poly.system_id and upper(draftinfo1.entity_type)=upper(poly.entity_type)
	where poly.approval_flag <> 'U' and 
	ST_Intersects(coalesce(draftinfo.updated_geom,poly.sp_geometry),ST_buffer_meters(St_Geomfromtext('POINT('||lng||' '||lat||')',4326),mtrBuffer)) 
	) tbl1
) tbl join layer_details l on upper(tbl.entity_type)=upper(l.layer_name)

 inner  join role_permission_entity rp on l.layer_id  = rp.layer_id and rp.role_id = v_role_id and rp.network_status = tbl.network_status and rp.viewonly = true
 and l.is_info_enabled=true and case when upper(l.layer_name)='PIT'  then 1=1 else l.isvisible=true end
 where row_num=1 order by tbl.geom_type asc
 , tbl.entity_type desc,tbl.display_name asc;
 --tbl.entity_type;
 Raise info 'Query end to return query ->%', clock_timestamp();
    
END
$function$
;
---------------------------------------------------------------------------------