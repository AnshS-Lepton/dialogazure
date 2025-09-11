-- FUNCTION: public.fn_backbone_save_plan(boolean, boolean, character varying, integer, integer)

-- DROP FUNCTION IF EXISTS public.fn_backbone_save_plan(boolean, boolean, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_backbone_save_plan(
	is_create_trench boolean,
	is_create_duct boolean,
	p_line_geom character varying,
	p_user_id integer,
	backbone_plan_id integer)
    RETURNS TABLE(status boolean, message character varying, v_plan_id integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

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
	left join point_master pm on pm.system_id = mh.system_id and pm.entity_type='Pole' where pm.system_id is nul;

   
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
$BODY$;

ALTER FUNCTION public.fn_backbone_save_plan(boolean, boolean, character varying, integer, integer)
    OWNER TO postgres;

--------------------------------------------------------------------------------------

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
FROM att_details_cable cbl
WHERE cbl.backbone_system_id = lp.backbone_system_id
  AND cbl.source_ref_id = lp.source_ref_id
  AND lp.source_ref_type = 'backbone planning'
  AND lp.source_ref_id = p_plan_id::varchar;



END
$function$
;

--------------------------------------------------------------------------------------

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
SELECT pod.site_id||'--DESC'  into p_site_id FROM point_master pm
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
----------------------------------------------------------------------------------------------------

-- FUNCTION: public.fn_auto_network_plan_draft_update_loop_network(character varying, integer, integer, double precision, double precision)

-- DROP FUNCTION IF EXISTS public.fn_auto_network_plan_draft_update_loop_network(character varying, integer, integer, double precision, double precision);

CREATE OR REPLACE FUNCTION public.fn_auto_network_plan_draft_update_loop_network(
	p_line_geom character varying,
	p_plan_id integer,
	p_user_id integer,
	p_loop_span double precision,
	p_looplength double precision)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_line_geom geometry;
    v_line_length double precision;
    step_fraction double precision;
BEGIN
    -- Create a temp table for loop results
    CREATE TEMP TABLE auto_network_plan_temp_loop
    ON COMMIT DROP AS
    SELECT * FROM temp_auto_network_plan WHERE false;

    -- Convert input line string to geometry
    SELECT ST_GeomFromText(CONCAT('LINESTRING(', p_line_geom, ')'), 4326)
    INTO v_line_geom;

    -- Total line length in meters
    SELECT ST_Length(v_line_geom::geography)
    INTO v_line_length;

    -- Compute fraction step size
    IF v_line_length > 0 AND p_loop_span > 0 THEN
        step_fraction := p_loop_span / v_line_length;
    ELSE
        step_fraction := 1; -- default: just start and end
    END IF;

    -- Insert nearest network points for each fraction
    IF step_fraction > 0 AND step_fraction <= 1 THEN
        INSERT INTO auto_network_plan_temp_loop
        SELECT t.*
        FROM generate_series(0::numeric, 1::numeric, step_fraction::numeric) AS frac
        JOIN LATERAL (
            SELECT *
            FROM temp_auto_network_plan t
            WHERE t.plan_id = p_plan_id
            ORDER BY ABS(t.fraction - frac)
            LIMIT 1
        ) t ON true;
    END IF;

    -- Always include endpoint at fraction = 1
   /* INSERT INTO auto_network_plan_temp_loop
    SELECT *
    FROM temp_auto_network_plan t
    WHERE t.plan_id = p_plan_id
    ORDER BY ABS(t.fraction - 1.0)
    LIMIT 1;*/

    -- Update loop_length for matched entities
    UPDATE temp_auto_network_plan np
    SET loop_length = p_looplength
    FROM auto_network_plan_temp_loop lp
    WHERE np.system_id = lp.system_id;

    -- Reset loop_length for others
    UPDATE temp_auto_network_plan np
    SET loop_length = 0
    WHERE np.plan_id = p_plan_id
      AND np.system_id NOT IN (
          SELECT lp.system_id FROM auto_network_plan_temp_loop lp
      );

END;
$BODY$;
