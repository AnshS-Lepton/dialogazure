
alter table backbone_plan_network_details add column cable_type character varying null;
alter table backbone_plan_details add column  is_create_plan bool default false;   
alter table backbone_plan_details add column  total_cable_length double precision null;
alter table backbone_plan_details add column  loop_span double precision null;

-------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_history_sites(planid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
BEGIN
    RETURN QUERY
    SELECT row_to_json(row)
    FROM (
        SELECT DISTINCT ON (bpns.network_id)
            bpns.network_id,
            bpns.site_id,
            bpns.site_name,
            bpns.is_selected,
            bpns.fibertype,
            bpns.intersect_line_geom AS cable_geom,
            bpns.site_geom,
            bpns.line_geom,
            COALESCE(
                ROUND(
                    ST_Length(
                        ST_GeomFromText('LINESTRING(' || bpns.line_geom || ')', 4326)::geography
                    )::numeric,
                    3
                ),
                0
            ) AS sprout_route_length,
            bpns.id
        FROM backbone_plan_nearest_site bpns
        WHERE bpns.plan_id = planid and bpns.is_selected = true
    ) row;
END;
$function$
;

---------------------------------------------------------------------------------------------

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

     RAISE INFO 'V_closet_point %', V_closet_point;

    RAISE INFO 'v_geometry %', st_astext(v_geometry);
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
    raise info ' v_entity_type %',v_entity_type;
   
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
           
      /*  line_geom := ST_LineSubstring(v_line_geom, current_fraction, current_fraction + step_fraction);
       
        structure_location := ST_LineInterpolatePoint(v_line_geom, current_fraction + step_fraction);*/

        -- cable type logic
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            p_cable_type := 'Underground';
        ELSE
            p_cable_type := 'Overhead';
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

        current_fraction := current_fraction + step_fraction;

        EXIT WHEN current_fraction >= 1.0;
    END LOOP;
    
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
	  
	 -- else 
	   -- remove the 
	 -- delete from backbone_plan_network_details where site_network_id = P_SITE_NETWORK_ID 
	 --and plan_id = p_plan_id and fraction = 0 and entity_type in ('Pole','Manhole');
	
	 -- cable type logic
       /* IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, v_geometry)
              AND entity_type = 'RestrictedArea'
        ) THEN
            p_cable_type := 'Manhole';
        ELSE
            p_cable_type := 'Pole';
        END IF;

            raise info 'testp_cable_type %',p_cable_type;

	 INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, planned_cable_entity,site_network_id 
        )
        VALUES (
            p_plan_id, p_cable_type,
            ST_X(v_geometry), ST_Y(v_geometry),
            v_geometry, p_user_id, p_backbone_fiber_type,'Sprout Cable',P_SITE_NETWORK_ID
        );
	   */
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

/*UPDATE backbone_plan_network_details AS np
SET province_id = pm.id,
    region_id   = rm.id
FROM region_boundary AS rm,
     province_boundary AS pm
WHERE rm.isvisible = TRUE
  AND pm.isvisible = TRUE
  AND ST_Intersects(
         pm.sp_geometry,
         st_endpoint(np.line_sp_geometry)  
     )
  AND ST_Intersects(
         rm.sp_geometry,
          st_endpoint(np.line_sp_geometry)
     )
     and np.planned_cable_entity ='Sprout Cable'
  AND np.plan_id  = P_plan_id
  AND np.created_by = p_user_id and np.intersect_line_geom is not null;
*/

 RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;

END;
$function$
;

--------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_draft_network(p_planid integer, p_userid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
   rec record; 
  v_site_geom record;
    v_sprout_intersect_geom geometry;
begin
   
DELETE FROM backbone_plan_network_details 
WHERE planned_cable_entity  = 'Sprout Cable'
  AND plan_id = p_planid ;

	
  FOR v_site_geom IN (SELECT distinct
  pm.longitude , pm.latitude,pm.sp_geometry  ,pm.common_name,bpns.fibertype,bpns.line_geom ,pm.system_id , bpns.id 
  FROM point_master pm 
  join att_details_pod pod on pm.system_id = pod.system_id and pm.entity_type = 'POD' 
  left join backbone_plan_nearest_site bpns on bpns.network_id = pod.network_id 
	  WHERE bpns.plan_id = p_planId and bpns.user_id = p_userid)
	loop
	
		SELECT ST_ClosestPoint(line.line_sp_geometry, v_site_geom.sp_geometry) 
		AS nearest_point INTO v_sprout_intersect_geom
		FROM backbone_plan_network_details line
		WHERE plan_id = p_planId
		ORDER BY ST_Distance(line.line_sp_geometry, v_site_geom.sp_geometry)
		LIMIT 1;

	UPDATE backbone_plan_nearest_site
	SET 
	    intersect_line_geom = 
	        round(ST_y(v_sprout_intersect_geom)::numeric, 6)::character varying  || ',' || 
	        round(ST_x(v_sprout_intersect_geom)::numeric, 6)::character varying ,
	       site_geom = round(ST_Y(v_site_geom.sp_geometry)::numeric, 6)::character varying  || ',' || round(ST_X(v_site_geom.sp_geometry)::numeric, 6)::character varying 
	WHERE id = v_site_geom.id 
	RETURNING id, plan_id, intersect_line_geom INTO rec;

	
	WITH threshold_closest_line AS (
    SELECT
       -- system_id,
        line_sp_geometry,
        sp_geometry,
        ST_DistanceSphere(
            sp_geometry,
            v_sprout_intersect_geom
        ) AS distance_meters
    FROM
        backbone_plan_network_details
    WHERE
        st_within(
            sp_geometry,
            st_buffer_meters(v_sprout_intersect_geom, (select threshold from backbone_plan_details bpd where plan_id = p_planId and created_by = p_userid and entity_type in ('Manhole','Pole')))
        )
        AND plan_id = p_planId
    ORDER BY
        distance_meters ASC
    LIMIT 1
)
UPDATE backbone_plan_nearest_site target
SET intersect_line_geom =  
    round(ST_Y(threshold_closest_line.sp_geometry)::numeric, 6)::varchar || ',' || 
    round(ST_X(threshold_closest_line.sp_geometry)::numeric, 6)::varchar
FROM threshold_closest_line
WHERE target.id = rec.id;

IF NOT EXISTS (
    select  1
    FROM
        backbone_plan_network_details
    WHERE
        st_within(
            sp_geometry,
            st_buffer_meters(v_sprout_intersect_geom, (select threshold from backbone_plan_details bpd where plan_id = p_planId and created_by = p_userid))
        )
        AND plan_id = p_planId  
) THEN
    UPDATE backbone_plan_nearest_site
    SET intersect_line_geom = round(ST_y(v_sprout_intersect_geom)::numeric, 6)::character varying  || ',' || 
	        round(ST_x(v_sprout_intersect_geom)::numeric, 6)::character varying,
	        sp_geometry = v_sprout_intersect_geom
    WHERE id = rec.id;
END IF;

END LOOP;

  if exists (select 1 from backbone_plan_nearest_site 
        where plan_id = p_planid and user_id = p_userId)
    then
  -- Return as JSON
    RETURN QUERY 
    SELECT row_to_json(t)
    FROM (
        SELECT id,plan_id, fibertype as fiber_type, intersect_line_geom, site_geom,ST_AsText(line_sp_geometry) as site_to_nearestcable_line_geom 
        FROM backbone_plan_nearest_site 
        WHERE user_id = p_userId and plan_id = p_planId 
    	) AS t;
   
  end if;   
END;
$function$
;

--------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_bom(p_plan_id integer, p_user_id integer, p_backbone_fiber_type character varying, p_sprout_fibertype character varying, p_backbone_line_geom character varying, p_iscreateduct boolean, p_iscreatetrench boolean)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_line_geom geometry;
    v_line_geom_length double precision;
    v_skip_ids integer;
BEGIN
    -- Geometry setup
    SELECT ST_GeomFromText('LINESTRING(' || p_backbone_line_geom || ')', 4326)
    INTO v_line_geom;
    SELECT ST_Length(v_line_geom, false)
    INTO v_line_geom_length;

    -- Add Backbone Cable system_id (1 latest record) to the same array
        SELECT system_id into v_skip_ids
        FROM backbone_plan_network_details
        WHERE entity_type IN ('Pole', 'Manhole')
          AND plan_id = p_plan_id
          AND planned_cable_entity = 'BackBone Cable'
        ORDER BY system_id DESC
        LIMIT 1;

    RETURN QUERY
    WITH bom_items AS (
        -- 1. Poles
        SELECT 'Pole' AS entity_type, COUNT(*)::numeric AS qty
        FROM backbone_plan_network_details
        WHERE plan_id = p_plan_id AND created_by = p_user_id
          AND entity_type = 'Pole'
          AND (system_id not in (v_skip_ids))

        UNION ALL

        -- 2. Manholes
        SELECT 'Manhole', COUNT(*)::numeric
        FROM backbone_plan_network_details
        WHERE plan_id = p_plan_id AND created_by = p_user_id
          AND entity_type = 'Manhole'
          AND (system_id not in (v_skip_ids))

        UNION ALL

        -- 3. SpliceClosures
        SELECT 'SpliceClosure', COUNT(*)::numeric
        FROM backbone_plan_network_details
        WHERE plan_id = p_plan_id AND created_by = p_user_id
          AND entity_type in ('SpliceClosure')
          
        UNION ALL

        -- 5. Duct (conditionally included)
        SELECT 'Duct', COUNT(*)::numeric
        FROM backbone_plan_network_details
        WHERE p_iscreateduct = true
          AND plan_id = p_plan_id AND created_by = p_user_id
          AND entity_type = 'Manhole'
          AND (system_id not in (v_skip_ids))

        UNION ALL

        -- 6. Trench (conditionally included)
        SELECT 'Trench', COUNT(*)::numeric
        FROM backbone_plan_network_details
        WHERE p_iscreatetrench = true
          AND plan_id = p_plan_id AND created_by = p_user_id
          AND entity_type = 'Manhole'
          AND (system_id not in (v_skip_ids))
    ),
   bom_pricing AS (
    -- Pricing for everything except Cable
    SELECT
        bi.entity_type,
        bi.qty::text AS cable_length_qty,
        itm.cost_per_unit,
         bi.qty,
        itm.service_cost_per_unit,
        ROUND((bi.qty::numeric * itm.cost_per_unit::numeric + bi.qty::numeric * itm.service_cost_per_unit::numeric)::numeric, 3) AS total_cost
    FROM bom_items bi
    JOIN item_template_master itm 
        ON itm.category_reference = bi.entity_type and itm.category_reference in ('Pole','Manhole','Duct')
    WHERE itm.specification = 'Generic'
 
    union all
     SELECT
        bi.entity_type,
        bi.qty::text AS cable_length_qty,
        itm.cost_per_unit,
         bi.qty,
        itm.service_cost_per_unit,
        ROUND((bi.qty::numeric * itm.cost_per_unit::numeric + bi.qty::numeric * itm.service_cost_per_unit::numeric)::numeric, 3) AS total_cost
    FROM bom_items bi
    JOIN item_template_master itm 
        ON itm.category_reference = bi.entity_type and itm.category_reference in ('SpliceClosure','Trench')
    WHERE itm.specification = 'generic'

    UNION ALL

 select 'Cable',
   concat( ROUND(SUM(cable_length_qty)::numeric, 3),'/',SUM(qty)) AS cable_length_qty,
     SUM(cost_per_unit) AS cost_per_unit,
    SUM(qty) AS total_qty,
    SUM(service_cost_per_unit) AS service_cost_per_unit,
    SUM(total_cost) AS total_cost
   
FROM (
    -- Place your current working query here
    SELECT
        ROUND(inner_bi.cbl_length::numeric, 3) AS cable_length_qty,
        inner_bi.cost_per_unit,
        inner_bi.qty,
        inner_bi.service_cost_per_unit,
        ROUND((inner_bi.cbl_length * inner_bi.cost_per_unit)::numeric, 3) +
        ROUND((inner_bi.cbl_length * inner_bi.service_cost_per_unit)::numeric, 3) AS total_cost
    FROM (
        SELECT
            COALESCE(SUM(ST_Length(line_sp_geometry, false)), 0)::numeric AS cbl_length,
            COUNT(t1.fiber_type) AS qty,
            itm.cost_per_unit,
            itm.service_cost_per_unit
        FROM backbone_plan_network_details t1
        JOIN item_template_master itm
            ON '(' || itm.code || ')' || itm.other = t1.fiber_type
        WHERE itm.category_reference = 'Cable'
          AND t1.plan_id = p_plan_id
          AND t1.created_by = p_user_id
        GROUP BY itm.cost_per_unit, itm.service_cost_per_unit
    ) AS inner_bi
) AS summary_data
)
    SELECT row_to_json(bp.*)
    FROM (
        SELECT * FROM bom_pricing WHERE qty > 0
    ) bp;

END;
$function$
;

--------------------------------------------------------------------------------------------

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
begin

	/*
select sum(loop_length) from att_details_loop where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and loop_length >0 into v_loop_length;

select count(1) into v_loop_entity_count from att_details_loop where  source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and loop_length >0;
*/
	
SELECT  substring(fiber_type FROM '\(([^)]+)\)'), split_part(fiber_type, ')', 2) into k_code, k_specification   
FROM backbone_plan_network_details
WHERE plan_id = p_plan_id
  AND entity_type = 'Manhole'
LIMIT 1;
		
/*if(v_loop_length>0)
then 
v_calculated_loop_length := v_loop_length;
end if;*/

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


if exists (SELECT 1 FROM att_details_trench WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) 
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty ,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount
	 from (
	select (select layer_title from layer_details where layer_name ilike ''trench'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty,sum(trench_length) as measured_length 
from att_details_trench att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Trench''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where source_ref_type = ''backbone planning'' and source_ref_id='''||p_plan_id||'''  GROUP BY
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
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where source_ref_type = ''backbone planning'' and source_ref_id='''||p_plan_id||'''  GROUP BY
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

----------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_backbone_draft_network(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, p_backbone_fiber_type character varying, p_pole_span double precision, p_manhole_span double precision, v_buffer double precision, p_threshold double precision, p_looplength double precision, p_is_looprequired boolean, cable_drum_length double precision, p_loop_span double precision)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_plan_id INTEGER;
    v_line_geom geometry;
    v_line_geom_length double precision;
    current_fraction double precision := 0.0;
    structure_location geometry;
    step_fraction double precision;
    line_geom geometry;
    manhole_fraction double precision;
    p_cable_type character varying;
BEGIN
    -- Create plan entry
    INSERT INTO backbone_plan_details (
        plan_name, start_point, end_point, is_create_trench, is_create_duct, 
        pole_distance, created_by, backbone_fiber_type, 
        threshold, buffer, is_loop_required, loop_length, manhole_distance, backbone_geometry,cable_length 
    )
    VALUES (
        p_plan_name, p_startpoint, p_endpoint, is_create_trench, is_create_duct,
        p_pole_span, p_user_id, p_backbone_fiber_type,
        p_threshold, v_buffer, p_is_looprequired, COALESCE(p_looplength,0),
        p_manhole_span, p_line_geom,cable_drum_length
    )
    RETURNING plan_id INTO v_plan_id;

    -- Convert geometry
    SELECT ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326) INTO v_line_geom;
    SELECT ST_Length(v_line_geom::geography) INTO v_line_geom_length;

    -------------------------------------------------------------------
    -- Structure placement (skip first + last point)
    -------------------------------------------------------------------
     WHILE current_fraction < 1.0 LOOP
        -- Try pole first
        structure_location := ST_LineInterpolatePoint(v_line_geom, LEAST(current_fraction + (p_pole_span / v_line_geom_length), 1.0));

        -- Check if it's in a restricted area
        IF NOT EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            step_fraction := p_pole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
         
            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,fraction,planned_cable_entity ,is_loop_required ,loop_length 
            ) VALUES (
                v_plan_id, 'Pole', ST_X(ST_EndPoint(line_geom)),  ST_Y(ST_EndPoint(line_geom)) , ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,current_fraction,'BackBone Cable',p_is_looprequired,0
            );           
           
        else
        	line_geom = NULL;
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
          
            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,loop_length ,planned_cable_entity 
            ) VALUES (
                v_plan_id, 'Manhole', ST_X(ST_EndPoint(line_geom)), ST_Y(ST_EndPoint(line_geom)), ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction,p_looplength,'BackBone Cable'
            );

        END IF;

        -- Move to next step
        current_fraction := current_fraction + step_fraction;

        -- Exit safety
        EXIT WHEN current_fraction >= 1.0;
    END LOOP;  

    -------------------------------------------------------------------
    -- Add SpliceClosure at start
    -------------------------------------------------------------------
  /*  INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity
    )
    VALUES (
        v_plan_id, 'SpliceClosure',
        ST_X(ST_StartPoint(v_line_geom)), ST_Y(ST_StartPoint(v_line_geom)),
        ST_StartPoint(v_line_geom), p_user_id, p_backbone_fiber_type, 0.0, 'BackBone Cable'
    );*/

    -------------------------------------------------------------------
    -- Cable placement
    -------------------------------------------------------------------
    current_fraction := 0.0;

    WHILE current_fraction < 1.0 LOOP
        step_fraction := cable_drum_length / v_line_geom_length;

        -- Prevent overshoot
        IF current_fraction + step_fraction > 1.0 THEN
            step_fraction := 1.0 - current_fraction;
        END IF;

        line_geom := ST_LineSubstring(v_line_geom, current_fraction, current_fraction + step_fraction);

        structure_location := ST_LineInterpolatePoint(v_line_geom, current_fraction + step_fraction);

        -- cable type logic
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            p_cable_type := 'Underground';
        ELSE
            p_cable_type := 'Overhead';
        END IF;

        -- insert cable
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, created_by, fiber_type, fraction, planned_cable_entity, cable_type, line_sp_geometry, sp_geometry
        )
        VALUES (
            v_plan_id, 'Cable', p_user_id, p_backbone_fiber_type,
            current_fraction, 'BackBone Cable', p_cable_type, line_geom, ST_EndPoint(line_geom)
        );

        -- insert splice closure at segment end
        INSERT INTO backbone_plan_network_details (
            plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type, fraction, planned_cable_entity
        )
        VALUES (
            v_plan_id, 'SpliceClosure',
            ST_X(ST_EndPoint(line_geom)), ST_Y(ST_EndPoint(line_geom)),
            ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type, current_fraction + step_fraction, 'BackBone Cable'
        );

        current_fraction := current_fraction + step_fraction;

        EXIT WHEN current_fraction >= 1.0;
    END LOOP;
   
   update backbone_plan_network_details set is_loop_required = false where system_id =
   (select system_id from backbone_plan_network_details where plan_id = v_plan_id and entity_type in ('Pole','Manhole') order by system_id desc limit 1);
   
   if (p_is_looprequired) then
  perform (fn_backbone_draft_update_loop_network(p_line_geom, v_plan_id, p_is_looprequired,p_user_id, p_loop_span,p_looplength));
  end if;
    -------------------------------------------------------------------
    -- Update region + province
    -------------------------------------------------------------------
    UPDATE backbone_plan_network_details AS np
    SET province_id = pm.id,
        region_id   = rm.id
    FROM region_boundary AS rm,
         province_boundary AS pm
    WHERE rm.isvisible = TRUE
      AND pm.isvisible = TRUE
      AND ST_Intersects(pm.sp_geometry, np.sp_geometry)
      AND ST_Intersects(rm.sp_geometry, np.sp_geometry)
      AND np.plan_id = v_plan_id
      AND np.created_by = p_user_id
      AND np.sp_geometry IS NOT NULL;

    RETURN QUERY
    SELECT row_to_json(t)
    FROM (SELECT v_plan_id AS plan_id) AS t;
END;
$function$
;

--------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_draft_update_loop_network(p_line_geom character varying, p_plan_id integer, p_is_loop_required boolean, p_user_id integer, p_loop_span double precision, p_looplength double precision)
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
    CREATE TEMP TABLE backbone_plan_temp_loop
	ON COMMIT DROP
	AS
	SELECT *
	FROM backbone_plan_network_details
	WHERE false;

	--truncate table backbone_plan_temp_loop;

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

        -- Insert directly if loop exists at this point
       INSERT INTO backbone_plan_temp_loop (
		    system_id, plan_id, entity_type, entity_network_id,
		    longitude, latitude, sp_geometry, province_id, region_id,
		    created_by, line_sp_geometry, is_middle_point, fraction,
		    cable_id, entity_system_id, display_name, cable_network_id,
		    duct_id, trench_id, trench_network_id, duct_network_id,
		    fiber_type, intersect_line_geom, site_geom, cable_length,
		    is_loop_required, planned_cable_entity, site_network_id,
		    is_update, cable_type, loop_length
		)
			SELECT bpnd.system_id, bpnd.plan_id, bpnd.entity_type, bpnd.entity_network_id,
		       bpnd.longitude, bpnd.latitude, bpnd.sp_geometry, bpnd.province_id, bpnd.region_id,
		       bpnd.created_by, bpnd.line_sp_geometry, bpnd.is_middle_point, bpnd.fraction,
		       bpnd.cable_id, bpnd.entity_system_id, bpnd.display_name, bpnd.cable_network_id,
		       bpnd.duct_id, bpnd.trench_id, bpnd.trench_network_id, bpnd.duct_network_id,
		       bpnd.fiber_type, bpnd.intersect_line_geom, bpnd.site_geom, bpnd.cable_length,
		       bpnd.is_loop_required, bpnd.planned_cable_entity, bpnd.site_network_id,
		       bpnd.is_update, bpnd.cable_type,p_looplength
        FROM backbone_plan_network_details bpnd
        WHERE ST_Intersects(bpnd.sp_geometry,st_buffer_meters(structure_location,2))
          AND bpnd.plan_id = p_plan_id and entity_type in ('Pole','Manhole')
          AND bpnd.is_loop_required = p_is_loop_required order by bpnd.system_id ;

        -- Next step
        current_fraction := current_fraction + (p_loop_span / v_line_geom_length);
    END LOOP;
   
    	INSERT INTO backbone_plan_temp_loop (
		    system_id, plan_id, entity_type, entity_network_id,
		    longitude, latitude, sp_geometry, province_id, region_id,
		    created_by, line_sp_geometry, is_middle_point, fraction,
		    cable_id, entity_system_id, display_name, cable_network_id,
		    duct_id, trench_id, trench_network_id, duct_network_id,
		    fiber_type, intersect_line_geom, site_geom, cable_length,
		    is_loop_required, planned_cable_entity, site_network_id,
		    is_update, cable_type, loop_length
		)
			SELECT bpnd.system_id, bpnd.plan_id, bpnd.entity_type, bpnd.entity_network_id,
		       bpnd.longitude, bpnd.latitude, bpnd.sp_geometry, bpnd.province_id, bpnd.region_id,
		       bpnd.created_by, bpnd.line_sp_geometry, bpnd.is_middle_point, bpnd.fraction,
		       bpnd.cable_id, bpnd.entity_system_id, bpnd.display_name, bpnd.cable_network_id,
		       bpnd.duct_id, bpnd.trench_id, bpnd.trench_network_id, bpnd.duct_network_id,
		       bpnd.fiber_type, bpnd.intersect_line_geom, bpnd.site_geom, bpnd.cable_length,
		       bpnd.is_loop_required, bpnd.planned_cable_entity, bpnd.site_network_id,
		       bpnd.is_update, bpnd.cable_type,bpnd.loop_length 
		FROM backbone_plan_network_details bpnd
		WHERE bpnd.plan_id = p_plan_id and entity_type in ('Pole','Manhole')
		  AND bpnd.is_loop_required = p_is_loop_required
		  AND bpnd.system_id NOT IN (
		        SELECT system_id FROM backbone_plan_temp_loop
		  );
		 
delete from backbone_plan_temp_loop where system_id = (select system_id FROM backbone_plan_network_details bpnd
		WHERE bpnd.entity_type in ('Pole','Manhole') order by system_id desc limit 1);

	update backbone_plan_network_details np set loop_length = lp.loop_length 
	from backbone_plan_temp_loop lp where np.system_id = lp.system_id ;
    -- Return all rows as JSON
    RETURN ;
END;
$function$
;

-------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_nearest_sites(line_geom character varying, buffer double precision, planid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_line geometry;
    v_buffer geometry;
    v_buffer_geojson json;
    v_sites json;
    v_total_length double precision := 0;
BEGIN
    IF line_geom IS NULL OR trim(line_geom) = '' THEN
        RETURN;
    END IF;

    -- Convert input text line into geometry
    SELECT ST_GeomFromText('LINESTRING(' || line_geom || ')', 4326)
    INTO v_line;

    -- Create a buffer around the line
    SELECT ST_Buffer(v_line::geography, buffer)::geometry
    INTO v_buffer;

    -- Convert buffer to GeoJSON
    SELECT ST_AsGeoJSON(v_buffer)::json
    INTO v_buffer_geojson;

    IF planid > 0 THEN
        IF EXISTS (
            SELECT 1
            FROM backbone_plan_nearest_site
            WHERE plan_id = planid
        ) THEN
            WITH s_all AS (
                SELECT DISTINCT ON (bpns.network_id)
                    bpns.network_id,
                    bpns.site_id,
                    bpns.site_name,
                    bpns.is_selected,
                    bpns.fibertype,
                    bpns.intersect_line_geom AS cable_geom,
                    bpns.site_geom,
                    bpns.line_geom,
                     COALESCE(ROUND(ST_Length(ST_GeomFromText('LINESTRING(' || bpns.line_geom || ')', 4326)::geography)::numeric, 3), 0) AS sprout_route_length,
                  bpns.id  
                FROM backbone_plan_nearest_site bpns
                WHERE bpns.plan_id = planid

                UNION

                SELECT
                    adp.network_id,
                    adp.site_id,
                    adp.site_name,
                    FALSE AS is_selected,
                    '' AS fibertype,
                    '' AS cable_geom,
                    '' AS site_geom,
                    '' AS line_geom,
                    0 AS sprout_route_length,
                    0 as id
                FROM point_master pm
                JOIN att_details_pod adp
                  ON pm.system_id = adp.system_id
                WHERE UPPER(pm.entity_type) = 'POD'
                  AND adp.status = 'A'
                  AND pm.network_status IN ('A')
                  AND ST_Within(pm.sp_geometry, v_buffer)
                  AND adp.network_id NOT IN (
                      SELECT network_id
                      FROM backbone_plan_nearest_site
                      WHERE plan_id = planid
                  )
                  --order by adp.network_id
            )
            SELECT 
                json_agg(
                    json_build_object(
                        'network_id', s_all.network_id,
                        'site_id', s_all.site_id,
                        'site_name', s_all.site_name,
                        'is_selected', s_all.is_selected,
                        'fibertype', s_all.fibertype,
                        'backbone_geom', s_all.cable_geom,
                        'geometry', s_all.site_geom,
                        'line_geom', s_all.line_geom,
                        'sprout_route_length', COALESCE(s_all.sprout_route_length, 0),
                        'id',s_all.id
                    )
                ),
                SUM(s_all.sprout_route_length)
            INTO v_sites, v_total_length
            FROM (select * from s_all order by s_all.network_id) s_all;

        ELSE
            SELECT json_agg(
                json_build_object(
                    'network_id', adp.network_id,
                    'site_id', adp.site_id,
                    'site_name', adp.site_name,
                    'is_selected', FALSE,
                    'fibertype', '',
                    'backbone_geom', '',
                    'geometry', '',
                    'line_geom', '',
                    'sprout_route_length', 0,
                    'id',0
                )
            )
            INTO v_sites
            FROM (
                SELECT
                    adp.network_id,
                    adp.site_id,
                    adp.site_name
                FROM point_master pm
                JOIN att_details_pod adp
                  ON pm.system_id = adp.system_id
                WHERE UPPER(pm.entity_type) = 'POD'
                  AND adp.status = 'A'
                  AND pm.network_status IN ('A')
                  AND ST_Within(pm.sp_geometry, v_buffer)
                ORDER BY adp.network_id
            ) adp;

          --  v_total_length := 0;
        END IF;
    END IF;

    RETURN NEXT json_build_object(
        'buffer_geometry', v_buffer_geojson,
        'sites', COALESCE(v_sites, '[]'::json),
        'total_sprout_length', ROUND(CAST(COALESCE(v_total_length, 0) AS numeric), 3)
    );
END;
$function$
;

-------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_network(p_plan_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
 lyrtitle record;
 v_entity_name character varying;
 v_new_entity_name character varying;
 sql text='';
 v_region_ids character varying='';
 v_province_ids character varying='';
 v_sub_sql text='';
 v_layer_id text;
BEGIN

	if not exists (select 1 from backbone_plan_details bpd where plan_id = p_plan_id and status = 'Completed') then 
	return ;
	end if;

SELECT string_agg(layer_id::text, ',') into v_layer_id
FROM layer_details 
WHERE upper(layer_name) IN ('POLE', 'MANHOLE', 'CABLE', 'SPLICECLOSURE','POD');


		FOR lyrtitle IN select distinct layer_title,layer_table,layer_name,geom_type from layer_details 
		where is_auto_plan_end_point=true or upper(layer_name) in (upper('cable'),upper('manhole'),upper('pole'))
LOOP
			v_entity_name:=lower(lyrtitle.layer_name)||'_name';
			
			select coalesce((SELECT column_name into v_new_entity_name FROM information_schema.columns
			WHERE table_name=lyrtitle.layer_table and column_name in('name',v_entity_name)),'Null');
IF EXISTS 		(SELECT 1  FROM information_schema.columns WHERE table_name=lyrtitle.layer_table and column_name ='source_ref_id')THEN
BEGIN

 IF(sql!='')THEN
 sql:=sql||' UNION ';
 END IF; 
			--execute 'insert into temp_nwt_entity_detalis
			sql:=sql||'Select lv.region_id,lv.province_id  from '||lyrtitle.layer_table||' as lv 
 			where lv.source_ref_id='||p_plan_id||'::character varying and Upper(source_ref_type)=''BACKBONE PLANNING''';
 END;
 END IF	;		
END LOOP;

 
 v_sub_sql:= 'select array_to_string(array_agg(row.region_id), '','')as region_id ,array_to_string(array_agg(row.province_id), '','')as province_id from('||sql||') row';

Execute v_sub_sql into v_region_ids,v_province_ids;

RAISE INFO ' v_sub_sql =%', v_region_ids;

v_sub_sql:='select '||v_region_ids||' as  region_ids, '||v_province_ids||' as province_ids, '|| quote_literal(v_layer_id)||' as layer_id ,* from backbone_plan_details where plan_id='||p_plan_id;

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||v_sub_sql||') row';
END ;

$function$
;

-------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_plan_network(p_plan_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
 lyrtitle record;
 v_entity_name character varying;
 v_new_entity_name character varying;
 sql text='';
 v_region_ids character varying='';
 v_province_ids character varying='';
 v_sub_sql text='';
 v_layer_id text;
BEGIN

SELECT string_agg(layer_id::text, ',') into v_layer_id
FROM layer_details 
WHERE upper(layer_name) IN ('POLE', 'MANHOLE', 'CABLE', 'SPLICECLOSURE','DUCT','TRENCH','LOOP');


		FOR lyrtitle IN select distinct layer_title,layer_table,layer_name,geom_type from layer_details 
		where is_auto_plan_end_point=true or upper(layer_name) in (upper('cable'),upper('manhole'),upper('pole'))
LOOP
			v_entity_name:=lower(lyrtitle.layer_name)||'_name';
			
			select coalesce((SELECT column_name into v_new_entity_name FROM information_schema.columns
			WHERE table_name=lyrtitle.layer_table and column_name in('name',v_entity_name)),'Null');
IF EXISTS 		(SELECT 1  FROM information_schema.columns WHERE table_name=lyrtitle.layer_table and column_name ='source_ref_id')THEN
BEGIN

 IF(sql!='')THEN
 sql:=sql||' UNION ';
 END IF; 
			--execute 'insert into temp_nwt_entity_detalis
			sql:=sql||'Select lv.region_id,lv.province_id  from '||lyrtitle.layer_table||' as lv 
 			where lv.source_ref_id='||p_plan_id||'::character varying and Upper(source_ref_type)=''BACKBONE PLANNING''';
 END;
 END IF	;		
END LOOP;

 
 v_sub_sql:= 'select array_to_string(array_agg(row.region_id), '','')as region_id ,array_to_string(array_agg(row.province_id), '','')as province_id from('||sql||') row';

RAISE INFO ' v_sub_sql =%', v_sub_sql;

Execute v_sub_sql into v_region_ids,v_province_ids;

RAISE INFO ' v_sub_sql =%', v_region_ids;
-- Final query combining everything
    v_sub_sql := format(
        'SELECT %L AS region_ids, %L AS province_ids, %L AS layer_id, start_point, end_point 
           FROM backbone_plan_details 
          WHERE plan_id = %s',
        v_region_ids, v_province_ids, v_layer_id, p_plan_id
    );

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||v_sub_sql||') row';
END ;

$function$
;

-----------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_list(p_user_id integer, p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_totalrecords integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

 DECLARE
    sql TEXT;
    SqlQueryCnt TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;

BEGIN

RAISE INFO '%', sql;
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '||CASE WHEN (P_SORTCOLNAME) ='' THEN 'edf.created_on' else P_SORTCOLNAME end||' ' || CASE WHEN (P_SORTTYPE) = '' THEN 'desc' ELSE P_SORTTYPE END||') AS S_No,um.user_name AS created_by_text, edf.* FROM backbone_plan_details edf
LEFT JOIN user_master um ON edf.created_by = um.user_id where edf.is_create_plan = true and ( edf.created_by='||p_user_id||') ';

IF (upper(p_searchtext) != '') THEN
-- change filter to display_name
	sql:= sql ||'AND upper(plan_name) LIKE upper(''%'||p_searchtext||'%'')';
END IF;

raise info 'sql %' ,sql;

SqlQueryCnt:= 'SELECT COUNT(1)  FROM ('||sql||') as a';
 EXECUTE   SqlQueryCnt INTO P_TOTALRECORDS;
 IF((P_PAGENO) <> 0) THEN
  
    StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
    EndSNo:= P_PAGENO * P_PAGERECORD;
    sql:= 'SELECT '||P_TOTALRECORDS||' as totalRecords, *
                
		FROM (' || sql || ' order by '|| CASE WHEN (P_SORTCOLNAME) ='' THEN 'edf.created_on' ELSE P_SORTCOLNAME END||' '||CASE WHEN (p_sorttype) ='' THEN 'desc' ELSE p_sorttype END ||') T               
                WHERE S_No BETWEEN ' || StartSNo || ' AND 100'; 
                -- WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 
                
     ELSE
	sql:= 'SELECT '||P_TOTALRECORDS||' as totalRecords, * FROM (' || sql || ' order by '||CASE WHEN (P_SORTCOLNAME) ='' THEN 'edf.created_on' ELSE P_SORTCOLNAME END||' '|| CASE WHEN (p_sorttype) ='' THEN 'desc' ELSE p_sorttype END ||') T';                  
  END IF; 


RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';


END ;
$function$
;

----------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_get_plan_kml_bom(p_plan_id integer, p_user_id integer)
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
begin

SELECT substring(fiber_type FROM '\(([^)]+)\)'), split_part(fiber_type, ')', 2) into k_code, k_specification   
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
     SUM(amount) AS amount,
     geom as sp_geometry
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
          COALESCE(item.cost_per_unit,0)  * SUM(att.cable_measured_length)) AS amount,
          ST_AsText(lm.sp_geometry) as geom
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
 GROUP BY entity_type, geom_type,sub.geom';

    -- Pole
    IF EXISTS (SELECT 1 FROM att_details_pole WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id ::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount , sp_geometry
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''Pole'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(1) AS length_qty,ST_AsText(pm.sp_geometry) as sp_geometry
            FROM att_details_pole att
            join point_master pm on pm.system_id = att.system_id
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''Pole'' and item.specification = ''Generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name, pm.sp_geometry
        ) e';
    END IF;

    -- Manhole
    IF EXISTS (SELECT 1 FROM att_details_manhole WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount ,sp_geometry
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''Manhole'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                item.cost_per_unit AS cost_per_unit,
                item.service_cost_per_unit AS service_cost_per_unit,
                COUNT(1) AS length_qty,ST_AsText(pm.sp_geometry) as sp_geometry
            FROM att_details_manhole att
            join point_master pm on pm.system_id = att.system_id
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''Manhole'' and item.specification = ''Generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name,pm.sp_geometry
        ) e';
    END IF;

    -- SpliceClosure
    IF EXISTS (SELECT 1 FROM att_details_spliceclosure WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) THEN
        sql := sql || '
        UNION 
        SELECT entity_type::character varying, geom_type, cost_per_unit, service_cost_per_unit, 
               length_qty::character varying,
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount , sp_geometry
        FROM (
            SELECT 
                (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''SpliceClosure'')::character varying AS entity_type,
                ''Point''::character varying AS geom_type,
                COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
                COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
                COUNT(att.system_id) AS length_qty, ST_AsText(sp_geometry) as sp_geometry
            FROM att_details_spliceclosure att
            join point_master pm on pm.system_id = att.system_id
            LEFT JOIN vendor_master vm ON att.vendor_id = vm.id
            LEFT JOIN item_template_master item ON item.category_reference = ''SpliceClosure'' and item.specification = ''generic''
            WHERE att.source_ref_type = ''backbone planning''
              AND att.source_ref_id = ' || quote_literal(p_plan_id) || '
            GROUP BY item.cost_per_unit, item.service_cost_per_unit, att.specification, att.item_code, vm.name,pm.sp_geometry
        ) e';
    END IF;   

if exists (SELECT 1 FROM att_details_trench WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) 
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty ,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount, sp_geometry
	 from (
	select (select layer_title from layer_details where layer_name ilike ''trench'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty,sum(trench_length) as measured_length , ST_AsText(lm.sp_geometry) as sp_geometry
from att_details_trench att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Trench''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where source_ref_type = ''backbone planning'' and source_ref_id='''||p_plan_id||'''  GROUP BY
	 ITEM.COST_PER_UNIT,ITEM.SERVICE_COST_PER_UNIT,ATT.SPECIFICATION,ATT.ITEM_CODE,VM.NAME,lm.sp_geometry) e';
end if;

if exists (SELECT 1 FROM att_details_duct WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar)
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount, sp_geometry
	 from 
(select (select layer_title from layer_details where layer_name ilike ''duct'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty,sum(calculated_length) as measured_length,ST_AsText(lm.sp_geometry) as sp_geometry
from att_details_duct att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Duct''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where source_ref_type = ''backbone planning'' and source_ref_id='''||p_plan_id||'''  GROUP BY
	 ITEM.COST_PER_UNIT,ITEM.SERVICE_COST_PER_UNIT,ATT.SPECIFICATION,ATT.ITEM_CODE,VM.NAME,lm.sp_geometry) e';
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


----------------------------------------------------------------------------------------------

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
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Manhole') and system_id not in (p_skip_last_manhole_pole) and sp_geometry is not NULL order by system_id;
 
INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select system_id,'Manhole',longitude,latitude, 'A', ST_GEOMFROMTEXT('POINT('||longitude||' '||latitude||')', 4326),
now(),'NA','NA',p_user_id,network_id, 'P',network_id,backbone_plan_id,'backbone planning'
    from att_details_manhole where system_id not in (select system_id from point_master where entity_type='Manhole');
   
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
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('SpliceClosure') and system_id not in (p_skip_last_sc) and sp_geometry is not NULL order by system_id;

 
INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select system_id,'SpliceClosure',longitude,latitude, 'A', ST_GEOMFROMTEXT('POINT('||longitude||' '||latitude||')', 4326),
now(),'NA','NA',p_user_id,network_id, 'P',network_id,backbone_plan_id,'backbone planning'
    from att_details_spliceclosure where system_id not in (select system_id from point_master where entity_type='SpliceClosure');
 
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
from backbone_plan_network_details tp where tp.plan_id = backbone_plan_id and entity_type in ('Pole') and system_id not in (p_skip_last_manhole_pole) and sp_geometry is not NULL order by system_id;

 
INSERT INTO point_master(system_id,entity_type,longitude,latitude,approval_flag,sp_geometry,approval_date,
creator_remark,approver_remark,created_by,common_name, network_status,display_name,source_ref_id ,source_ref_type)

select system_id,'Pole',longitude,latitude, 'A', ST_GEOMFROMTEXT('POINT('||longitude||' '||latitude||')', 4326),
now(),'NA','NA',p_user_id,network_id, 'P',network_id,backbone_plan_id::varchar,'backbone planning' 
    from att_details_pole where system_id not in (select system_id from point_master where entity_type='Pole');
   
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
	0,'P','A',p_user_id,now(),case when plan.planned_cable_entity = 'BackBone Cable' then 'BackBone Cable' else 'Feeder' end,
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
FROM fn_get_line_network_code('Trench','','',v_geom_str,'OSP'::character varying,0,'');

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
FROM fn_get_line_network_code('Duct','','',v_geom_str,'OSP'::character varying,0,'');

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


----------------------------------------------------------------------------------

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
	and is_loop_required = true and sp_geometry is not null and plan_id = p_plan_id 
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
FROM att_details_cable cbl
WHERE cbl.backbone_system_id = lp.backbone_system_id
  AND cbl.source_ref_id = lp.source_ref_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar;



END
$function$
;


-----------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_update_network_code(entity_table text, entity_type text, source_ref_id_val text, source_ref_type_val text, update_entity_name text)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
    sql_stmt text;
begin
	if(entity_type = 'Loop') then 
	 sql_stmt := format($sql$
        UPDATE %I AS t
        SET
            network_id = sub.prefix || LPAD((sub.base_number + sub.rn)::text, 6, '0'),
            parent_system_id = 0,
            parent_network_id = sub.network_code,
            sequence_id = LPAD((sub.base_number + sub.rn)::text, 6, '0')::integer,
            parent_entity_type = sub.parent_entity_type
        FROM (
            SELECT
                t.system_id,
                regexp_replace(c.network_id, '\d', '', 'g') AS prefix,
                COALESCE(NULLIF(regexp_replace(c.network_id, '\D', '', 'g'), ''), '0')::int AS base_number,
                ROW_NUMBER() OVER (PARTITION BY t.province_id ORDER BY t.system_id) - 1 AS rn,
                c.network_code,
                c.parent_entity_type
            FROM %I t
            JOIN temp_generate_network_code c
                ON c.province_id = t.province_id
                AND c.entity_type = %L
            WHERE t.source_ref_id = %L
              AND t.source_ref_type = %L
        ) AS sub
        WHERE t.system_id = sub.system_id;
    $sql$,
    entity_table,
    entity_table,
    entity_type,
    source_ref_id_val,
    source_ref_type_val
    );
	else 
    -- Base update for network codes
    sql_stmt := format($sql$
        UPDATE %I AS t
        SET
            network_id = sub.prefix || LPAD((sub.base_number + sub.rn)::text, 6, '0'),
            %I = sub.prefix || LPAD((sub.base_number + sub.rn)::text, 6, '0'),
            parent_system_id = 0,
            parent_network_id = sub.network_code,
            sequence_id = LPAD((sub.base_number + sub.rn)::text, 6, '0')::integer,
            parent_entity_type = sub.parent_entity_type
        FROM (
            SELECT
                t.system_id,
                regexp_replace(c.network_id, '\d', '', 'g') AS prefix,
                COALESCE(NULLIF(regexp_replace(c.network_id, '\D', '', 'g'), ''), '0')::int AS base_number,
                ROW_NUMBER() OVER (PARTITION BY t.province_id ORDER BY t.system_id) - 1 AS rn,
                c.network_code,
                c.parent_entity_type
            FROM %I t
            JOIN temp_generate_network_code c
                ON c.province_id = t.province_id
                AND c.entity_type = %L
            WHERE t.source_ref_id = %L
              AND t.source_ref_type = %L
        ) AS sub
        WHERE t.system_id = sub.system_id;
    $sql$,
    entity_table,
    update_entity_name,
    entity_table,
    entity_type,
    source_ref_id_val,
    source_ref_type_val
    );
end if;
    -- Append correct table update based on entity type
    IF entity_type IN ('Pole','Manhole','SpliceClosure','Loop') THEN
        sql_stmt := sql_stmt || format($sql$
            UPDATE point_master AS pm
            SET 
                common_name = m.network_id,
                display_name = m.network_id
            FROM %I AS m
            WHERE m.system_id = pm.system_id
              AND pm.entity_type = %L
              AND m.source_ref_id = %L
              AND m.source_ref_type = %L;
        $sql$,
        entity_table,
        entity_type,
        source_ref_id_val,
        'backbone planning'
        );
    ELSIF entity_type IN ('Duct','Cable','Trench') THEN
        sql_stmt := sql_stmt || format($sql$
            UPDATE line_master AS pm
            SET 
                common_name = m.network_id,
                display_name = m.network_id
            FROM %I AS m
            WHERE m.system_id = pm.system_id
              AND pm.entity_type = %L
              AND m.source_ref_id = %L
              AND m.source_ref_type = %L;
        $sql$,
        entity_table,
        entity_type,
        source_ref_id_val,
        'backbone planning'
        );
    END IF;

    -- Run everything in one go
    EXECUTE sql_stmt;
END;
$function$
;

-------------------------------------------------------------------------------------------













