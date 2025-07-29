


CREATE OR REPLACE FUNCTION public.fn_get_update_site_fiber_details(linestring character varying, nearestsite_system_id integer, p_system_id integer, nearestsite_distance double precision, p_nearest_cable_geom character varying, p_nearest_cable_system_id integer, p_cable_end_to_site_geom character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$


DECLARE
   v_full_geom geometry;
   v_length double precision := 0;
   ug_distance double precision := 0;
   oh_distance double precision := 0;
   v_manhole_span integer;
   v_pole_span integer;
   v_restricted_intersect_count integer := 0;
   POLE_INTERSECT_COUNT INTEGER = 0;
   MANHOLE_INTERSECT_COUNT INTEGER = 0;
   v_intersect_point geometry;
   v_substring_geom geometry;
   fracPoint double precision := 0; 
  start_frac double precision;
  end_frac double precision;
BEGIN

	select value:: integer from global_settings  where key ='NearBySitePoleSpan' 
    into v_pole_span ;
	select value:: integer from global_settings  where key ='NearBySiteManholeSpan' 
    into v_manhole_span ;

    v_full_geom := ST_GeomFromText('LINESTRING(' || p_cable_end_to_site_geom || ')', 4326);
    v_length = ST_Length(v_full_geom::geography);
   
	SELECT 
	  (ST_Intersection(lm.sp_geometry , v_full_geom)) into v_intersect_point
	FROM line_master lm
	  where system_id = p_nearest_cable_system_id;
 
   IF v_intersect_point IS NOT null AND NOT ST_IsEmpty(v_intersect_point)  then
   IF ST_GeometryType(v_intersect_point) = 'ST_LineString' or  ST_GeometryType(v_intersect_point) = 'ST_MultiLineString'  
   then 
  start_frac := ST_LineLocatePoint(v_full_geom, 							ST_StartPoint(v_intersect_point));
  end_frac := ST_LineLocatePoint(v_full_geom, 							ST_EndPoint(v_intersect_point));
   fracPoint := LEAST(start_frac, end_frac);
   v_substring_geom := ST_LineSubstring(v_full_geom, 0.0::double precision, 	fracPoint::double precision);
   else
    fracPoint := ST_LineLocatePoint(v_full_geom, v_intersect_point);
	v_substring_geom := ST_LineSubstring(v_full_geom, 0.0::double precision, 	fracPoint::double precision);

  END IF;
  END IF;

   SELECT COUNT(*) into v_restricted_intersect_count FROM polygon_master
   WHERE ST_Intersects(sp_geometry, v_full_geom)
   AND entity_type = 'RestrictedArea';
           
        IF  v_restricted_intersect_count > 0
        then
          SELECT 
          SUM(ST_Length(ST_Intersection(v_full_geom, poly.sp_geometry)::geography)) 
          into ug_distance
          FROM polygon_master poly
           WHERE 
            ST_Intersects(v_full_geom, poly.sp_geometry)
            AND poly.entity_type = 'RestrictedArea';
   
           oh_distance = v_length - ug_distance ; 

        else 
          oh_distance  = v_length;
          ug_distance = 0;

        END IF;
       
        MANHOLE_INTERSECT_COUNT = (v_restricted_intersect_count * 2);        
       
        UPDATE att_details_pod
        SET fiber_oh_distance_to_network = oh_distance,
            fiber_ug_distance_to_network = ug_distance,
            total_fiber_distance = v_length,
            fiber_distance_to_nearest_site = nearestsite_distance + v_length,
            pole_count = CEIL(oh_distance/v_pole_span::double precision):: int,
            manhole_count = CEIL(ug_distance/v_manhole_span::double precision):: int,
            spliceclosure_count = FLOOR(v_length / 2000.0) + 1 
                                  + (CASE WHEN MOD(v_length,  2000.0) > 0 THEN 1 ELSE 0 END) 
                                  + (v_restricted_intersect_count * 2) ,
            manhole_span = v_manhole_span,
            pole_span =  v_pole_span,
            cable_intersection_count = MANHOLE_INTERSECT_COUNT,
            nearest_site = (select network_id from att_details_pod 
            where system_id = nearestsite_system_id),
	nearest_site_geometry = case when ST_IsEmpty(v_intersect_point) 
    then ST_GeomFromText('LINESTRING(' || p_cable_end_to_site_geom || ')', 4326)
    else v_substring_geom 
	end WHERE system_id = p_system_id;

	WITH filtered_items AS (
		SELECT cost_per_unit, category_reference
		FROM item_template_master
		WHERE category_reference IN ('Pole', 'Manhole', 'SpliceClosure')
		  AND specification IN ('Generic', '48 Port SpliceClouser')
	),
	cost_calc AS (
		SELECT
			a.system_id,
			SUM(
				CASE 
					WHEN f.category_reference = 'Pole' THEN a.pole_count * f.cost_per_unit
					WHEN f.category_reference = 'Manhole' THEN a.manhole_count * f.cost_per_unit
					WHEN f.category_reference = 'SpliceClosure' THEN a.spliceclosure_count * f.cost_per_unit
					ELSE 0
				END
			) AS total_cost
		FROM att_details_pod a
		JOIN filtered_items f
		  ON f.category_reference IN ('Pole', 'Manhole', 'SpliceClosure')
		WHERE a.system_id = p_system_id
		GROUP BY a.system_id
	)
	UPDATE att_details_pod a
	SET plan_cost = c.total_cost
	FROM cost_calc c
	WHERE a.system_id = c.system_id;

	UPDATE site_project_details sp
	SET maximum_cost = pod.plan_cost
	FROM att_details_pod pod
	WHERE sp.site_id = pod.site_id
	 AND pod.system_id = p_system_id;

END;
$function$
;

-------------------------------------------------------------------------------------------

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
	
select sum(loop_length) from att_details_loop where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and loop_length >0 into v_loop_length;

select count(1) into v_loop_entity_count from att_details_loop where  source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and loop_length >0;

SELECT  substring(fiber_type FROM '\(([^)]+)\)'), split_part(fiber_type, ')', 2) into k_code, k_specification   
FROM backbone_plan_network_details
WHERE plan_id = p_plan_id
  AND entity_type = 'Manhole'
LIMIT 1;
		
if(v_loop_length>0)
then 
v_calculated_loop_length := v_loop_length;
end if;

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
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount , NULL AS sp_geometry
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
   
if(v_loop_length>0)
 then 
sql:=sql ||' UNION SELECT 
    entity_type, 
    geom_type, 
    SUM(cost_per_unit) AS cost_per_unit, 
    SUM(service_cost_per_unit) AS service_cost_per_unit,
    loop_length || ''(m)/'' || loop_count AS length_qty,
    (loop_length * SUM(service_cost_per_unit) +
     SUM(cost_per_unit) * loop_length) AS amount, NULL AS sp_geometry
FROM (
    SELECT 
        (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''loop'') AS entity_type,
        ''Line'' AS geom_type,
        COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
        COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
        ' || v_calculated_loop_length || ' AS loop_length,
        ' || v_loop_entity_count || ' AS loop_count
    FROM item_template_master item 
    WHERE item.code = ' || quote_literal(k_code) || ' AND item.other = ' || quote_literal(k_specification) || '
    GROUP BY item.cost_per_unit, item.service_cost_per_unit
) AS loop_data 
GROUP BY entity_type, geom_type, loop_length, loop_count';

 end if;

if exists (SELECT 1 FROM att_details_trench WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) 
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty ,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount, sp_geometry
	 from (
	select (select layer_title from layer_details where layer_name ilike ''trench'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty,sum(trench_length) as measured_length , ST_AsText(lm.sp_geometry) as sp_geometry
from att_details_trench att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Trench''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where att.source_ref_type = ''backbone planning'' and att.source_ref_id='''||p_plan_id||'''  GROUP BY
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
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where att.source_ref_type = ''backbone planning'' and att.source_ref_id='''||p_plan_id||'''  GROUP BY
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
	
select sum(loop_length) from att_details_loop where source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and loop_length >0 into v_loop_length;

select count(1) into v_loop_entity_count from att_details_loop where  source_ref_id=''||p_plan_id||'' and source_ref_type='backbone planning' and loop_length >0;

SELECT  substring(fiber_type FROM '\(([^)]+)\)'), split_part(fiber_type, ')', 2) into k_code, k_specification   
FROM backbone_plan_network_details
WHERE plan_id = p_plan_id
  AND entity_type = 'Manhole'
LIMIT 1;
		
if(v_loop_length>0)
then 
v_calculated_loop_length := v_loop_length;
end if;

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
               (cost_per_unit * length_qty + service_cost_per_unit * length_qty) AS amount , NULL AS sp_geometry
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
   
if(v_loop_length>0)
 then 
sql:=sql ||' UNION SELECT 
    entity_type, 
    geom_type, 
    SUM(cost_per_unit) AS cost_per_unit, 
    SUM(service_cost_per_unit) AS service_cost_per_unit,
    loop_length || ''(m)/'' || loop_count AS length_qty,
    (loop_length * SUM(service_cost_per_unit) +
     SUM(cost_per_unit) * loop_length) AS amount, NULL AS sp_geometry
FROM (
    SELECT 
        (SELECT layer_title FROM layer_details WHERE layer_name ILIKE ''loop'') AS entity_type,
        ''Line'' AS geom_type,
        COALESCE(item.cost_per_unit, 0) AS cost_per_unit,
        COALESCE(item.service_cost_per_unit, 0) AS service_cost_per_unit,
        ' || v_calculated_loop_length || ' AS loop_length,
        ' || v_loop_entity_count || ' AS loop_count
    FROM item_template_master item 
    WHERE item.code = ' || quote_literal(k_code) || ' AND item.other = ' || quote_literal(k_specification) || '
    GROUP BY item.cost_per_unit, item.service_cost_per_unit
) AS loop_data 
GROUP BY entity_type, geom_type, loop_length, loop_count';

 end if;

if exists (SELECT 1 FROM att_details_trench WHERE source_ref_type = 'backbone planning' AND source_ref_id = p_plan_id::varchar) 
then 
	sql:=sql || ' union select entity_type::character varying,geom_type,cost_per_unit,service_cost_per_unit,round((measured_length)::numeric, 2) || ''(m)/''|| length_qty ::character varying as length_qty ,
	(round((measured_length)::numeric, 2) * service_cost_per_unit +  cost_per_unit * round((measured_length)::numeric, 2))as amount, sp_geometry
	 from (
	select (select layer_title from layer_details where layer_name ilike ''trench'')::character varying as entity_type,''Line''::character varying as geom_type,COALESCE(COST_PER_UNIT,0)as COST_PER_UNIT,COALESCE(SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,count(att.system_id)as length_qty,sum(trench_length) as measured_length , ST_AsText(lm.sp_geometry) as sp_geometry
from att_details_trench att 
join line_master lm on lm.system_id = att.system_id and lm.entity_type =''Trench''
LEFT JOIN VENDOR_MASTER VM ON ATT.VENDOR_ID = VM.ID
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where att.source_ref_type = ''backbone planning'' and att.source_ref_id='''||p_plan_id||'''  GROUP BY
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
LEFT JOIN AUDIT_ITEM_TEMPLATE_MASTER ITEM ON ITEM.AUDIT_ID=ATT.AUDIT_ITEM_MASTER_ID where att.source_ref_type = ''backbone planning'' and att.source_ref_id='''||p_plan_id||'''  GROUP BY
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

-- Permissions


------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_sr_get_export_report_data(p_networkstatues character varying, p_provinceids character varying, p_regionids character varying, p_layer_name character varying, p_searchby character varying, p_searchbytext character varying, p_fromdate character varying, p_todate character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_geom character varying, p_duration_based_column character varying, p_radius double precision, p_userid integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

   DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
   s_report_view_name character varying;
   s_geom_type character varying;
s_layer_id integer; 
s_layer_columns text;
TotalAppliedRecords integer; 
TotalRejectedRecords integer;
BEGIN

LowerStart:='';
LowerEnd:='';
WhereCondition:='';

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..	
-- SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS FROM (
-- SELECT  COLUMN_NAME,res_field_key,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

--SELECT (SPECIFIC) ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..
SELECT STRING_AGG(COLUMN_NAME ||' as "'|| CASE WHEN COALESCE(res_field_key, '') = '' THEN DISPLAY_NAME ELSE res_field_key END || '"', ',') INTO S_LAYER_COLUMNS 
FROM (SELECT COLUMN_NAME, res_field_key, DISPLAY_NAME FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID = S_LAYER_ID AND UPPER(SETTING_TYPE) = 'REPORT' AND IS_ACTIVE = TRUE
       AND COLUMN_NAME IN ('system_id','site_id','site_name','latitude','longitude',
                           'fiber_oh_distance_to_network','fiber_ug_distance_to_network','total_fiber_distance',
                           'fiber_distance_to_nearest_site','cost_based_on_rate_card_lkr','is_site_imported','nearest_site',
                          'manhole_count','pole_count','spliceclosure_count','owner_name') ORDER BY COLUMN_SEQUENCE ) A;

-- DYNAMIC QUERY
IF ((p_geom) != '') 
THEN
	if(p_radius>0)
	then
		select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10)  into p_geom;
	end if;
RAISE INFO '------------------------------------S_LAYER_COLUMNS%', S_LAYER_COLUMNS;

	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id, '||S_LAYER_COLUMNS||' from (select 
     a.system_id,a.site_id,a.site_name,a.latitude,a.longitude, 
     a.fiber_oh_distance_to_network,ROUND(a.fiber_ug_distance_to_network::NUMERIC, 2) as fiber_ug_distance_to_network,ROUND(a.total_fiber_distance::NUMERIC, 2) as total_fiber_distance,
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported,(select case pod.site_id is null then pod.network_id else pod.site_id end from att_details_pod pod where pod.network_id=a.nearest_site limit 1) as nearest_site,
     a.manhole_count,a.pole_count,a.spliceclosure_count,a.owner_name
   from '||S_REPORT_VIEW_NAME||' a inner join '||s_geom_type||'_master m
	on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') 
	and ST_Intersects(m.sp_geometry, st_geomfromtext(''POLYGON(('||p_geom||'))'',4326)) 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
	RAISE INFO '------------------------------------sql%', sql;
ELSE
     -- a.system_id, a.design_id,a.network_id,a.port_type

	sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY system_id desc) AS S_No,system_id, '||S_LAYER_COLUMNS||' from (select 
     a.system_id, a.site_id,a.site_name,a.latitude,a.longitude, 
     a.fiber_oh_distance_to_network,ROUND(a.fiber_ug_distance_to_network::NUMERIC, 2) as fiber_ug_distance_to_network,ROUND(a.total_fiber_distance::NUMERIC, 2) as total_fiber_distance,
     a.fiber_distance_to_nearest_site,a.cost_based_on_rate_card_lkr,a.is_site_imported,(select site_id from att_details_pod pod where pod.network_id=a.nearest_site limit 1) as nearest_site,
     a.manhole_count,a.pole_count,a.spliceclosure_count,a.owner_name
    from '||S_REPORT_VIEW_NAME||' a 
	inner join vw_user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and 1=1 ';
		RAISE INFO '------------------------------------sql1%', sql;
END IF;

IF ((p_networkStatues) != '' and upper(S_LAYER_COLUMNS) like '%NETWORK_STATUS%') THEN
	sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in('||p_networkStatues||')';
END IF;

IF ((p_RegionIds) != '') THEN
	sql:= sql ||' AND a.region_id IN ('||p_RegionIds||')';
END IF;

IF ((P_ProvinceIds) != '') THEN
	sql:= sql ||' AND a.province_id IN ('||P_ProvinceIds||')';
END IF;

IF ((P_searchbytext) != '' and (P_searchby) != '') THEN
--sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(''%'||trim(P_searchbytext)||'%'')';

	if(substring(P_searchbytext from 1 for 1)='"' and  substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
	then
		sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
	else
		sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
	end if;

END IF;

IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND a.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

sql:= sql ||' )X';
RAISE INFO '%', sql;
-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
 RAISE INFO 'page%',P_PAGENO;
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords,* FROM (' || sql || ') T';                  
 END IF; 

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$function$
;
