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
   
if(v_loop_length>0)
 then 
sql:=sql ||' UNION SELECT 
    entity_type, 
    geom_type, 
    SUM(cost_per_unit) AS cost_per_unit, 
    SUM(service_cost_per_unit) AS service_cost_per_unit,
    loop_length || ''(m)/'' || loop_count AS length_qty,
    (loop_length * SUM(service_cost_per_unit) +
     SUM(cost_per_unit) * loop_length) AS amount
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

--------------------------------------------------------------------------------------------

alter table backbone_plan_network_details add column planned_cable_entity character varying null;
alter table backbone_plan_network_details add column site_network_id character varying null;
alter table backbone_plan_network_details add column is_update bool default false;
alter table backbone_plan_details add column sprout_route_length double precision null;

alter table backbone_plan_nearest_site add column sp_geometry geometry;
alter table backbone_plan_nearest_site add column line_sp_geometry geometry;
alter table backbone_plan_nearest_site add column created_by integer;
alter table backbone_plan_nearest_site add column system_id integer;
alter table backbone_plan_nearest_site add column is_update bool default false;


-------------------------------------------------------------------------------------------
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

-------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_backbone_draft_network(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, p_plan_name character varying, p_startpoint character varying, p_endpoint character varying, p_sprout_fiber_type character varying, p_backbone_fiber_type character varying, p_pole_span double precision, p_manhole_span double precision, v_buffer double precision, p_threshold double precision, p_looplength double precision, p_is_looprequired boolean)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_plan_id INTEGER;
    rec record;
    v_sprout_line_geom geometry;
    v_site_geom record;
    v_common_names text[];
   geomtxt character varying;
  	v_line_geom geometry;
 	v_line_geom_length double precision;
   current_fraction double precision := 0.0;
   structure_location geometry;
   step_fraction double precision;
    line_geom geometry;
begin
	

    INSERT INTO backbone_plan_details(plan_name, start_point, end_point, is_create_trench, is_create_duct, pole_distance, created_by, sprout_fiber_type, backbone_fiber_type,threshold,buffer,is_loop_required, loop_length,manhole_distance)
    VALUES (p_plan_name, p_startpoint, p_endpoint, is_create_trench, is_create_duct, p_pole_span, p_user_id, p_sprout_fiber_type, p_backbone_fiber_type,p_threshold,v_buffer,p_is_looprequired,COALESCE(p_looplength, 0),p_manhole_span)
    RETURNING plan_id INTO v_plan_id;
   
select ST_GeomFromText(concat('LINESTRING(', p_line_geom, ')'), 4326) into v_line_geom;
select st_length(v_line_geom,false) into v_line_geom_length;

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
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,planned_cable_entity 
            ) VALUES (
                v_plan_id, 'Pole', ST_X(ST_EndPoint(line_geom)),  ST_Y(ST_EndPoint(line_geom)) , ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction,'BackBone Cable'
            );           
           
        else
        	line_geom = NULL;
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
            
            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,loop_length,is_loop_required ,planned_cable_entity
            ) VALUES (
                v_plan_id, 'Manhole', ST_X(ST_EndPoint(line_geom)), ST_Y(ST_EndPoint(line_geom)), ST_EndPoint(line_geom), p_user_id, p_backbone_fiber_type,line_geom,current_fraction,p_looplength,p_is_looprequired,'BackBone Cable'
            );

        END IF;

        -- Move to next step
        current_fraction := current_fraction + step_fraction;

        -- Exit safety
        EXIT WHEN current_fraction >= 1.0;
    END LOOP;  
 /*     
   FOR v_site_geom IN 
   (SELECT sp_geometry,common_name  FROM point_master WHERE common_name in ( SELECT unnest(string_to_array(p_nearest_sites, ',')) )
   )
loop
		
SELECT 
    ST_ClosestPoint(line.line_sp_geometry, v_site_geom.sp_geometry) AS nearest_point INTO v_sprout_line_geom
FROM 
    backbone_plan_network_details line
WHERE 
    ST_Intersects(
        line.line_sp_geometry,
        ST_Buffer(v_site_geom.sp_geometry, v_buffer)  -- ~5 meters buffer (in degrees)
    )
    AND plan_id = v_plan_id
ORDER BY 
    ST_Distance(line.line_sp_geometry, v_site_geom.sp_geometry)
LIMIT 1;

    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type,intersect_line_geom,site_geom 
    )
    VALUES (
        v_plan_id,'Cable', p_user_id, p_sprout_fiber_type,
         round(ST_Y(v_sprout_line_geom)::numeric, 6)::character varying || ',' || round(ST_X(v_sprout_line_geom)::numeric, 6)::character varying ,
   round(ST_Y(v_site_geom.sp_geometry)::numeric, 6)::character varying || ',' || round(ST_X(v_site_geom.sp_geometry)::numeric, 6)::character varying
    ) RETURNING 
    intersect_line_geom,
    system_id,
    plan_id 
INTO rec;

WITH threshold_closest_line AS (
    SELECT
        system_id,
        line_sp_geometry,
        sp_geometry,
        ST_DistanceSphere(
            sp_geometry,
            v_sprout_line_geom
        ) AS distance_meters
    FROM
        backbone_plan_network_details
    WHERE
        st_within(
            sp_geometry,
            st_buffer_meters(v_sprout_line_geom, p_threshold)
        )
        AND plan_id = v_plan_id
    ORDER BY
        distance_meters ASC
    LIMIT 1
)
UPDATE backbone_plan_network_details target
SET intersect_line_geom =  
    round(ST_Y(threshold_closest_line.sp_geometry)::numeric, 6)::varchar || ',' || 
    round(ST_X(threshold_closest_line.sp_geometry)::numeric, 6)::varchar
FROM threshold_closest_line
WHERE target.system_id = rec.system_id;

IF NOT EXISTS (
    select  1
    FROM
        backbone_plan_network_details
    WHERE
        st_within(
            sp_geometry,
            st_buffer_meters(v_sprout_line_geom, p_threshold)
        )
        AND plan_id = v_plan_id  
) THEN
    UPDATE backbone_plan_network_details
    SET sp_geometry = v_sprout_line_geom
    WHERE system_id = rec.system_id;
END IF;


END LOOP;

 UPDATE backbone_plan_network_details
 SET loop_length = 0,is_loop_required=false
 WHERE system_id = (select system_id  from backbone_plan_network_details where entity_type in ('Manhole','Pole') and plan_id = v_plan_id order by system_id desc limit 1);
   


	if exists (select 1 from backbone_plan_network_details where plan_id = v_plan_id and entity_type = 'Cable' )
    then
  -- Return as JSON
    RETURN QUERY 
    SELECT row_to_json(t)
    FROM (
        SELECT system_id,plan_id, fiber_type, intersect_line_geom, site_geom, created_by 
        FROM backbone_plan_network_details 
        WHERE created_by = p_user_id and plan_id = v_plan_id and fiber_type = p_sprout_fiber_type and entity_type = 'Cable' 
    ) AS t;
   else 
   */
   RETURN QUERY 
    SELECT row_to_json(t)
    FROM (
        SELECT v_plan_id as plan_id
    ) AS t;
  -- end if;
   
END;
$function$
;

------------------------------------------------------------------------------------------

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
	
		select ST_ClosestPoint(line.line_sp_geometry, v_site_geom.sp_geometry) 
		AS nearest_point INTO v_sprout_intersect_geom
		FROM 
		    backbone_plan_network_details line
		WHERE 
		    ST_Intersects(
		        line.line_sp_geometry,
		        ST_Buffer(v_site_geom.sp_geometry, (select buffer from backbone_plan_details bpd where plan_id = p_planId and created_by = p_userid)) 
		    )
		    AND plan_id = p_planId
		ORDER BY 
		    ST_Distance(line.line_sp_geometry, v_site_geom.sp_geometry)
		LIMIT 1;


	UPDATE backbone_plan_nearest_site
	SET 
	    intersect_line_geom = 
	        round(ST_Y(v_sprout_intersect_geom)::numeric, 6)::character varying  || ',' || 
	        round(ST_X(v_sprout_intersect_geom)::numeric, 6)::character varying ,
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
            st_buffer_meters(v_sprout_intersect_geom, (select threshold from backbone_plan_details bpd where plan_id = p_planId and created_by = p_userid))
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
    SET intersect_line_geom = round(ST_Y(v_sprout_intersect_geom)::numeric, 6)::character varying  || ',' || 
	        round(ST_X(v_sprout_intersect_geom)::numeric, 6)::character varying,
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
    v_is_first boolean := true;
    v_is_last boolean;
    V_closet_point character varying;
   V_is_update bool;
  sp_route_length double precision;
 sp_totalroute_length double precision;
begin
	
      select pole_distance ,manhole_distance into p_pole_span,p_manhole_span,p_looplength,p_is_looprequired from backbone_plan_details bpd where plan_id = p_plan_id;
		
	select fibertype,network_id,intersect_line_geom,is_update into p_backbone_fiber_type,P_SITE_NETWORK_ID, V_closet_point,V_is_update from backbone_plan_nearest_site bpns where id = p_systemid;	

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
	
	    -- Check if the line is within restricted area
	IF (v_line_geom_length < p_manhole_span AND EXISTS (
	        SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_line_geom)
	          AND entity_type = 'RestrictedArea')
	) THEN

    -- Insert Manhole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type, line_sp_geometry,
        loop_length, is_loop_required, planned_cable_entity  , site_network_id
    )
    VALUES (
        p_plan_id, 'Manhole', p_user_id, p_backbone_fiber_type, v_line_geom,
        p_looplength, p_is_looprequired, 'Sprout Cable', P_SITE_NETWORK_ID
    );

    -- Update intersect_line_geom if POD point not present nearby
    UPDATE backbone_plan_network_details b
    SET intersect_line_geom = V_closet_point
    WHERE b.plan_id = P_plan_id
     AND b.planned_cable_entity = 'Sprout Cable'
     AND b.site_network_id = P_SITE_NETWORK_ID;

    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3)) into sp_route_length 
    from backbone_plan_network_details where site_network_id = P_SITE_NETWORK_ID 
    and plan_id = p_plan_id;
   
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where planned_cable_entity = 'Sprout Cable' and plan_id = p_plan_id;
   
    -- RETURN select json sp_route_length::varchar;
RETURN QUERY SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;

	   -- return ;
	END IF;


	-- Pole check for non-restricted area
	IF (v_line_geom_length < p_pole_span AND NOT EXISTS (
	        SELECT 1 FROM polygon_master
	        WHERE ST_Intersects(sp_geometry, v_line_geom)
	          AND entity_type = 'RestrictedArea')
	) THEN

    -- Insert Pole entity
    INSERT INTO backbone_plan_network_details (
        plan_id, entity_type, created_by, fiber_type, line_sp_geometry,
        loop_length, is_loop_required, planned_cable_entity, site_network_id
    )
    VALUES (
        p_plan_id, 'Pole', p_user_id, p_backbone_fiber_type, v_line_geom,
        p_looplength, p_is_looprequired, 'Sprout Cable', P_SITE_NETWORK_ID
    );

    -- Update intersect_line_geom if POD point not present nearby
    UPDATE backbone_plan_network_details b
    SET intersect_line_geom = V_closet_point
    WHERE b.plan_id = P_plan_id
     AND b.planned_cable_entity = 'Sprout Cable'
     AND b.site_network_id = P_SITE_NETWORK_ID;
    
	 --RETURN;
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3)) into sp_route_length from backbone_plan_network_details where site_network_id = P_SITE_NETWORK_ID  and plan_id = P_plan_id;
   
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where planned_cable_entity = 'Sprout Cable' and plan_id = p_plan_id;
  RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;
	END IF;
	   
  
      WHILE current_fraction < 1.0 LOOP
        -- Try pole first
        structure_location := ST_LineInterpolatePoint(v_line_geom, 		LEAST(current_fraction + (p_pole_span / v_line_geom_length), 1.0));

        -- Check if it's in a restricted area
        IF EXISTS (
            SELECT 1 FROM polygon_master
            WHERE ST_Intersects(sp_geometry, structure_location)
              AND entity_type = 'RestrictedArea'
        ) THEN
            step_fraction := p_manhole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
             v_is_last := LEAST(current_fraction + step_fraction, 1.0) = 1.0;

            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction,loop_length,is_loop_required,planned_cable_entity,site_network_id 
            ) VALUES (
                p_plan_id, 'Manhole', CASE WHEN v_is_first OR v_is_last THEN NULL ELSE ST_X(ST_EndPoint(line_geom)) END,
                CASE WHEN v_is_first OR v_is_last THEN NULL ELSE ST_Y(ST_EndPoint(line_geom)) END,
                CASE WHEN v_is_first OR v_is_last THEN NULL ELSE ST_EndPoint(line_geom) END, p_user_id, p_backbone_fiber_type,line_geom,current_fraction,p_looplength,p_is_looprequired,
                'Sprout Cable',P_SITE_NETWORK_ID
            );           
           
        else
        	line_geom = NULL;
            step_fraction := p_pole_span / v_line_geom_length;
            line_geom := ST_LineSubstring(v_line_geom, current_fraction, LEAST(current_fraction + step_fraction, 1.0));
            v_is_last := LEAST(current_fraction + step_fraction, 1.0) = 1.0;

            INSERT INTO backbone_plan_network_details (
                plan_id, entity_type, longitude, latitude, sp_geometry, created_by, fiber_type,line_sp_geometry,fraction ,planned_cable_entity,site_network_id 
            ) VALUES (
                p_plan_id, 'Pole', CASE WHEN v_is_first OR v_is_last THEN NULL ELSE ST_X(ST_EndPoint(line_geom)) END,
                CASE WHEN v_is_first OR v_is_last THEN NULL ELSE ST_Y(ST_EndPoint(line_geom)) END,
                CASE WHEN v_is_first OR v_is_last THEN NULL ELSE ST_EndPoint(line_geom) END, p_user_id, p_backbone_fiber_type,line_geom,current_fraction,'Sprout Cable',P_SITE_NETWORK_ID
            );

        END IF;

        -- Move to next step
        current_fraction := current_fraction + step_fraction;
  		v_is_first := false;
        -- Exit safety
        EXIT WHEN current_fraction >= 1.0;
    END LOOP;  
   	
	-- Update rows where current_fraction = 0 (start point)
	UPDATE backbone_plan_network_details b
	SET intersect_line_geom = V_closet_point
	WHERE b.plan_id = P_plan_id
	  AND b.planned_cable_entity = 'Sprout Cable'
	  AND b.site_network_id = P_SITE_NETWORK_ID
	  AND b.fraction  = 0
	  AND NOT EXISTS (
	    SELECT 1
	    FROM point_master pm
	    WHERE pm.common_name = P_SITE_NETWORK_ID
	      AND pm.entity_type = 'POD'
	      AND ST_intersects(
	            pm.sp_geometry,
	            st_buffer_meters(
	            ST_StartPoint(b.line_sp_geometry),
	            2)
	          )
	  );

	 --Update rows where end point
	UPDATE backbone_plan_network_details b
	SET intersect_line_geom = V_closet_point
	WHERE b.plan_id = P_plan_id
	  AND b.planned_cable_entity = 'Sprout Cable'
	  AND b.site_network_id = P_SITE_NETWORK_ID
	  AND b.fraction != 0
	  AND b.sp_geometry IS NULL
	  AND NOT EXISTS (
	    SELECT 1
	    FROM point_master pm
	    WHERE pm.common_name = P_SITE_NETWORK_ID
	      AND pm.entity_type = 'POD'
	      AND ST_Intersects(
	            pm.sp_geometry,
	              st_buffer_meters(
	            ST_EndPoint(b.line_sp_geometry),
	            2
	          )
	          )
	  );
 
	 select sum(round(ST_Length(line_sp_geometry, false)::numeric,3)) into sp_route_length 
    from backbone_plan_network_details where site_network_id = P_SITE_NETWORK_ID 
    and plan_id = p_plan_id;
   
    select sum(round(ST_Length(line_sp_geometry, false)::numeric,3))
    into sp_totalroute_length from backbone_plan_network_details 
    where planned_cable_entity = 'Sprout Cable' and plan_id = p_plan_id;
   

 RETURN QUERY  SELECT row_to_json(u)
FROM (
    SELECT sp_route_length as sprout_route_length, sp_totalroute_length as total_sp_route_length
) AS u;

END;
$function$
;

----------------------------------------------------------------------------------


---------------------------------------------------------------------------------------

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

--------------------------------------------------------------------------------------

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

------------------------------------------------------------------------------------

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
LEFT JOIN user_master um ON edf.created_by = um.user_id where ( edf.created_by='||p_user_id||') ';

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


----------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_backbone_save_plan(is_create_trench boolean, is_create_duct boolean, p_line_geom character varying, p_user_id integer, p_plan_name character varying, plan_id integer)
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
p_plan_id integer;
v_temp_point_entity record;
v_geom_str character varying;
p_cable_type character varying;
p_skip_cable_end_point_entity  integer;
BEGIN
v_is_valid:=true;
v_message:='[SI_GBL_GBL_GBL_GBL_066]';
p_rfs_type:='A-RFS';
p_plan_id=plan_id;

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
created_by integer,
splicing_source character varying(100)
) on commit drop;


IF(V_IS_VALID)
THEN

select ST_GeomFromText('LINESTRING('||p_line_geom||')',4326) into v_line_geom;
select st_length(v_line_geom,false)::double precision into v_line_total_length;

IF(V_IS_VALID)
THEN

select system_id into p_skip_cable_end_point_entity  from backbone_plan_network_details tp where tp.plan_id = p_plan_id and entity_type in ('Manhole','Pole') and planned_cable_entity  ='BackBone Cable' order by system_id desc limit 1;

select ''::character varying as network_id,''::character varying as entity_type ,0::integer as system_id  into v_arow_prev_closure; 

FOR v_temp_point_entity IN select * from backbone_plan_network_details tp where tp.plan_id = p_plan_id and entity_type in ('Pole','Manhole') and planned_cable_entity = 'BackBone Cable' order by system_id
LOOP	

select * into v_region_province from fn_getregionprovince(substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),'Point');

update backbone_plan_network_details set province_id =v_region_province.province_id,region_id=v_region_province.region_id where system_id=v_temp_point_entity.system_id;
RAISE INFO 'p_skip_point_id p: %', p_skip_cable_end_point_entity;

RAISE INFO 'v_temp_point_entity system_id p1: %', v_temp_point_entity.system_id;

if v_temp_point_entity.system_id != p_skip_cable_end_point_entity 
then
RAISE INFO 'v_temp_point_entity p1: %', v_temp_point_entity.system_id;

SELECT *
INTO v_arow_closure from fn_backbone_save_entity(is_create_trench, is_create_duct, p_user_id, plan_id ,v_temp_point_entity.entity_type::character varying,v_temp_point_entity.sp_geometry ::geometry,v_temp_point_entity.line_sp_geometry::geometry, v_temp_point_entity.system_id::integer,v_temp_point_entity.longitude ::double precision,v_temp_point_entity.latitude ::double precision,v_region_province.province_id::integer, v_region_province.region_id ::integer) AS result(system_id integer, network_id varchar, entity_type varchar);

RAISE INFO 'v_arow_closure p1: %', v_arow_closure;

end if;
--------------------- insert cable --------------------------
if(p_skip_cable_end_point_entity = v_temp_point_entity.system_id:: integer) 
then
v_arow_prev_closure = null;
select ''::character varying as network_id,''::character varying as entity_type ,0::integer as system_id  into v_arow_prev_closure; 

end if;

v_geom_str = null;
select substring(left(St_astext(v_temp_point_entity.line_sp_geometry),-1),12) ::character varying INTO v_geom_str;

 IF EXISTS (SELECT 1 FROM polygon_master 
      WHERE ST_Intersects(
        sp_geometry,
        ST_GeomFromText('LINESTRING(' || v_geom_str || ')', 4326))
        AND entity_type = 'RestrictedArea')
      then 
        p_cable_type = 'Underground';
        else 
        p_cable_type = 'Overhead';
  end if;
    
select * into v_arow_cable from fn_backbone_auto_create_cable(v_arow_prev_closure.system_id,
v_arow_prev_closure.entity_type,v_arow_prev_closure.network_id,
v_arow_closure.system_id::integer,v_arow_closure.entity_type::character varying,v_arow_closure.network_id::character varying, 
v_geom_str::character varying,
0,v_region_province.region_id::integer ,v_region_province.province_id::integer,p_user_id,p_rfs_type ::character varying,500,v_temp_point_entity.fiber_type::character varying,p_plan_id,
p_cable_type::character varying,0,''::character varying)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

RAISE INFO 'v_arow_cABLE p: %', v_arow_cable;

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id::integer,'Cable'::character varying ,v_region_province.province_id,0,false));

 update backbone_plan_network_details set 
cable_network_id = v_arow_cable.network_id, cable_id = v_arow_cable.system_id
where system_id = v_temp_point_entity.system_id;

RAISE INFO 'v_arow_closure p: %', v_arow_closure;

v_arow_prev_closure.system_id := v_arow_closure.system_id;
v_arow_prev_closure.entity_type := v_arow_closure.entity_type::varchar;
v_arow_prev_closure.network_id := v_arow_closure.network_id::varchar;

end loop;
--------------   end backbone data query  -----------------------------------------


--------------   start sproutplan data query  -----------------------------------------
FOR v_temp_point_entity IN (select * from backbone_plan_network_details tp where tp.plan_id = p_plan_id and planned_cable_entity = 'Sprout Cable' order by system_id)
loop 

select substring(left(St_astext(v_temp_point_entity.line_sp_geometry),-1),12) ::character varying INTO v_geom_str;

    IF EXISTS (SELECT 1 FROM polygon_master 
      WHERE ST_Intersects(
        sp_geometry,
        ST_GeomFromText('LINESTRING(' || v_geom_str || ')', 4326))
        AND entity_type = 'RestrictedArea')
      then 
        p_cable_type = 'Overhead';
        else 
        p_cable_type = 'Underground';
    end if;
       

if (v_temp_point_entity.sp_geometry is not null)
then 

select * into v_region_province from fn_getregionprovince(substring(left(St_astext(v_temp_point_entity.sp_geometry),-1),7),'Point');
	
update backbone_plan_network_details set province_id =v_region_province.province_id,region_id=v_region_province.region_id where system_id=v_temp_point_entity.system_id;

SELECT * INTO v_arow_closure 
from fn_backbone_save_entity (is_create_trench, is_create_duct, p_user_id, plan_id ,v_temp_point_entity.entity_type::character varying,v_temp_point_entity.sp_geometry ::geometry,v_temp_point_entity.line_sp_geometry::geometry, v_temp_point_entity.system_id::integer,v_temp_point_entity.longitude ::double precision,v_temp_point_entity.latitude ::double precision,v_region_province.province_id::integer, v_region_province.region_id ::integer) 
AS result(system_id integer, network_id varchar, entity_type varchar); 
 
  RAISE NOTICE 'start backbone cable - closure: %',v_arow_closure;

end if;

if (v_temp_point_entity.intersect_line_geom is null and v_temp_point_entity.sp_geometry is null)
then
  RAISE NOTICE 'site 1: %',v_arow_closure;

v_arow_prev_closure = v_arow_closure;
v_arow_closure = null;
select common_name as network_id,entity_type,system_id  into v_arow_closure from point_master pm where common_name = v_temp_point_entity.site_network_id and entity_type ='POD' limit 1;
  RAISE NOTICE 'site2 : %',v_arow_prev_closure;
  RAISE NOTICE 'site 3: %',v_arow_closure;

 end if;

if v_temp_point_entity.intersect_line_geom is not null
 then
select '' ::character varying  as network_id , ''::character varying as entity_type  ,0::integer as system_id into v_arow_prev_closure;

end if;

  RAISE NOTICE 'start backbone cable';
 
select * into v_arow_cable from fn_backbone_auto_create_cable(v_arow_prev_closure.system_id::integer,
v_arow_prev_closure.entity_type::character varying,v_arow_prev_closure.network_id::character varying,
v_arow_closure.system_id::integer,v_arow_closure.entity_type::character varying,v_arow_closure.network_id::character varying, 
v_geom_str::character varying,
0,v_region_province.region_id::integer,v_region_province.province_id::integer,p_user_id,p_rfs_type ::character varying,500,v_temp_point_entity.fiber_type::character varying,p_plan_id,
p_cable_type,0,''::character varying)
as x(system_id integer,network_id character varying,no_of_tube integer,total_core integer);

perform (fn_geojson_update_entity_attribute(v_arow_cable.system_id::integer,'Cable'::character varying ,v_region_province.province_id,0,false));	

update backbone_plan_network_details set 
cable_network_id = v_arow_cable.network_id, cable_id = v_arow_cable.system_id
where system_id = v_temp_point_entity.system_id;

v_arow_prev_closure.system_id := v_arow_closure.system_id;
v_arow_prev_closure.entity_type := v_arow_closure.entity_type::varchar;
v_arow_prev_closure.network_id := v_arow_closure.network_id::varchar;

end loop;
--------------   end sproutplan data query  -----------------------------------------

if ((SELECT COUNT(1) FROM temp_connection)>0)
then
 
perform(fn_auto_provisioning_save_connections());

END IF;

PERFORM(fn_backbone_get_loop_entity(p_plan_id));


END IF;

IF(V_IS_VALID)
THEN
V_MESSAGE:='BackBone network plan processed successfully!';
update backbone_plan_details t set status = 'Completed' where t.plan_id = p_plan_id and created_by = p_user_id;
END IF;
end if;
RETURN QUERY SELECT V_IS_VALID::boolean AS STATUS, V_MESSAGE::CHARACTER VARYING AS MESSAGE,plan_id :: integer as v_plan_id ;

end;
$function$
;

---------------------------------------------------------------------------------

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
                  AND pm.network_status IN ('P')
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
                  AND pm.network_status IN ('P')
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

-----------------------------------------------------------------------------------