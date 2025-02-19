update res_resources set is_jquery_used=true where key='SI_OSP_ONT_NET_RPT_012';

/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: 
ModifiedOn: 31 Jan 2025
ModifiedBy: Chandra Shekhar Sahni
Purpose: Removed commented code also handled if p_user_id is coming as null or empty then return empty result set also implemented Exception handling
------------------------------------------*/
-- DROP FUNCTION public.fn_get_report_users(varchar, bool);

CREATE OR REPLACE FUNCTION public.fn_get_report_users(p_user_id character varying, p_isselected boolean)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE
   sql TEXT;
   userGroupName character varying;
BEGIN
   -- Check if p_user_id is null or empty and return empty result set if true
   IF p_user_id IS NULL OR p_user_id = '' THEN
      RETURN QUERY SELECT row_to_json(row) FROM (SELECT NULL AS user_id, NULL AS user_name, NULL AS name, NULL AS user_email, NULL AS role_name, NULL AS groupUser, NULL AS isSelected, NULL AS mobile_number) row WHERE false;
      RETURN;
   END IF;

   BEGIN
      sql := 'SELECT * from ( select  distinct um.user_id,um.user_name,um.name,um.user_email,rm.role_name,case when
 umm.manager_id in (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)  or
 um.user_id in
  (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number) then ''same'' else ''Others'' end as groupUser,
 case when umm.manager_id in (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)  
 or (select count(1)>0 from user_report_mapping
 where parent_user_id in (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)  and child_user_id = um.user_id)=true
 or um.user_id in (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)  then true else false end as isSelected,um.mobile_number 
 from user_master um
 left join user_manager_mapping umm on um.user_id = umm.user_id
 inner join
 role_master rm on rm.role_id = um.role_id
 inner join user_permission_area upa1 on upa1.user_id in(SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)
 inner join user_permission_area upa on upa.user_id=um.user_id
  and upa.province_id=upa1.province_id and upa.region_id=upa1.region_id
 where   um.role_id <>1 and um.is_Active = true)a';
      IF p_isselected = true THEN
         sql := sql || ' where isSelected = true'; 
      END IF;

      sql := sql || ' ORDER BY isSelected desc,groupuser desc, CASE WHEN user_id in (SELECT number::INTEGER FROM 
regexp_split_to_table('''||p_user_id||''', '','') AS number) THEN ''1'' Else user_name END';

      --RAISE INFO '%', sql;
      RETURN QUERY EXECUTE 'select row_to_json(row) from (' || sql || ')row';

   EXCEPTION
      WHEN others THEN
         -- Insert error details into error_log table
         INSERT INTO public.error_log (
            user_id, server_ip, controller_name, action_name, err_message, err_description, created_on
         ) VALUES (
            0, inet_server_addr(),
            'Report', 'fn_get_report_users()', SQLERRM, 
            'Exception Message: ' || SQLERRM || ' | Exception Type: ' || 'Export Report Functionality', now()
         );

         -- Return empty result set
         RETURN QUERY SELECT row_to_json(row) FROM (SELECT NULL AS user_id, NULL AS user_name, NULL AS name, NULL AS user_email, NULL AS role_name, NULL AS groupUser, NULL AS isSelected, NULL AS mobile_number) row WHERE false;
   END;
END
$function$
;
--------------------------------------------------------------------------------------------
-- public.fiberlink_routing_data definition

-- Drop table

-- DROP TABLE public.fiberlink_routing_data;

CREATE TABLE public.fiberlink_routing_data (
	id int4 NULL,
	system_id int4 NULL,
	ufi int4 NULL,
	sn int4 NULL,
	route_no int4 NULL,
	rc int4 NULL,
	rt varchar DEFAULT 'Road'::character varying NULL,
	sr int4 NULL,
	trf int4 DEFAULT 0 NULL,
	city varchar NULL,
	state_name varchar NULL,
	country varchar DEFAULT 'India'::character varying NULL,
	length float8 NULL,
	"cost" float8 DEFAULT 0 NULL,
	reverse_cost float8 DEFAULT 0 NULL,
	geom public.geometry NULL,
	"source" int4 NULL,
	target int4 NULL,
	user_id int4 NULL
);
CREATE INDEX fiberlink_routing_data_geom_idx ON public.fiberlink_routing_data USING gist (geom);
CREATE INDEX fiberlink_routing_data_id_idx ON public.fiberlink_routing_data USING btree (id);
CREATE INDEX fiberlink_routing_data_source_idx ON public.fiberlink_routing_data USING btree (source);
CREATE INDEX fiberlink_routing_data_target_idx ON public.fiberlink_routing_data USING btree (target);

-- Permissions

ALTER TABLE public.fiberlink_routing_data OWNER TO postgres;
GRANT ALL ON TABLE public.fiberlink_routing_data TO postgres;
--------------------------------------------------------------------------------------------
/*------------------------------------------
CreatedBy: Navika Jaiswal
CreatedOn: 28-01-2025
Description: This is generating result of odf to odf connectivity based on fiber link 
ModifiedOn: 
ModifiedBy: 
Purpose: 
------------------------------------------*/
-- DROP FUNCTION public.fn_get_fiberlink_schematic_view(int4, int4);

CREATE OR REPLACE FUNCTION public.fn_get_fiberlink_schematic_view(fiber_link_system_id integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE RESULT text;
DECLARE 
p_link_system_id integer;
odf_rec RECORD;
p_source character varying;
p_destination character varying;
max_id integer;
nodes text;
cables text;
legends text;
edges text;
v_odf_1_system_id integer;
v_odf_2_system_id integer;
via_port_no integer;
REC RECORD;
BEGIN
via_port_no = 1;
--drop table if exists fiberlink_routing_data;
delete from fiberlink_routing_data where user_id = p_user_id;
   

 CREATE temp TABLE temp_cable_routes(
        seq integer,
        path_seq integer, 
        edge_targetid integer, 
        roadline_geomtext text, 
        start_point character varying,
        end_point character varying,
        message varchar NULL,
        is_valid boolean DEFAULT true,
        avaiable_core_count integer DEFAULT 0,
        user_id integer,
        a_system_id int4 NULL,
	   b_system_id int4 NULL,
	  a_entity_type varchar NULL,
	   b_entity_type varchar null,
	   color_code varchar null,
	   cable_category varchar null ,
	   cable_type varchar null,
network_status varchar null,
total_core integer
)ON COMMIT DROP;

-- Create temporary tables

   create TEMP TABLE temp_odf_lst(
        odf1 integer,
        odf2 integer,
        user_id integer
        ) ON COMMIT DROP;

select system_id into p_link_system_id from att_details_fiber_link where system_id = fiber_link_system_id;

-- insert odf into temp table which is connected to fiberlink  
insert into temp_odf_lst ( odf1,odf2, user_id)    
SELECT DISTINCT ipi1.parent_system_id AS odf1, ipi2.parent_system_id AS odf2, p_user_id
FROM isp_port_info ipi1
JOIN isp_port_info ipi2 
    ON ipi1.link_system_id = ipi2.link_system_id
WHERE ipi1.parent_system_id < ipi2.parent_system_id
AND ipi1.link_system_id = p_link_system_id  
ORDER BY odf1, odf2;

--- insert all cable into temp table which is connected to fiberlink  
   insert into fiberlink_routing_data(id, system_id, ufi, sn, route_no, rc, rt, sr, trf, city, state_name, country, length, cost, reverse_cost, geom, user_id) 
select DISTINCT  c.system_id as id, c.system_id as system_id, c.system_id as ufi,c.system_id as sn,c.system_id as route_no, 50 as rc,
	'Road'::character varying as rt,40 as sr,0 as trf,c.province_name as city, c.region_name as state_name,'India'::character varying as country,
	ST_Length(ST_Transform(ST_GeomFromEWKT('SRID=4326;'||st_astext(lm.sp_geometry)),26986))/1000 as length,0 as cost,0 as reverse_cost,
	ST_Force2D(lm.sp_geometry) as geom,p_user_id from line_master lm join  vw_att_details_cable c on lm.system_id=c.system_id 
inner join att_details_cable_info info on info.cable_id = c.system_id where lm.entity_type='Cable' and info.link_system_id  = p_link_system_id ;

	-- remove the invalid rows
	delete from fiberlink_routing_data where length is null;
   
	-- insert same records again with reverse geometry
	insert into fiberlink_routing_data(id, ufi, sn, route_no, rc, rt, sr, trf, city, state_name, country, 
	length, cost, reverse_cost, geom, source, target, system_id)
	select nextval('seq_routing'), currval('seq_routing'), currval('seq_routing'), currval('seq_routing'), rc, rt, sr, trf, city, state_name, country, length, cost, reverse_cost, st_reverse(geom), source, target, id from fiberlink_routing_data;

	-- create routing topology
	SELECT pgr_createTopology('fiberlink_routing_data', 0.000001, 'geom') INTO RESULT;
   if RESULT = 'OK' then

  for REC in (select * from temp_odf_lst where user_id = p_user_id ) 
  loop
		      
    -- Get source and destination points
    SELECT longitude || ' ' || latitude AS geom
    INTO p_source
    FROM att_details_fms
    WHERE system_id  = REC.odf1;

    SELECT longitude || ' ' || latitude AS geom
    INTO p_destination
    FROM att_details_fms
    WHERE system_id  = REC.odf2;
   
     RAISE NOTICE 'geom1:%',p_source;
     RAISE NOTICE 'geom2:%',p_destination;

    -- Populate temporary cable routes table
    INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, roadline_geomtext, start_point, end_point ,user_id)
    SELECT seq, path_seq, edge_targetid, roadline_geomtext, start_point, end_point,p_user_id
    FROM fn_get_fiberlink_route_data(p_source, p_destination, 5, 5);
   drop table temp_routes;
    end loop;
  
   -- update the nodes of cable details
   UPDATE temp_cable_routes set color_code =cbls.color_code,cable_category=cbl.cable_category ,cable_type =cbl.cable_type,
network_status=cbl.network_status,total_core=cbl.total_core 
from att_details_cable cbl
left join cable_color_settings cbls on cbls.cable_type=cbl.cable_type and cbls.cable_category=cbl.cable_category
where cbl.system_id=temp_cable_routes.edge_targetid;


   -- update the nodes of cable 
   	with startCte as(
SELECT pm.system_id,pm.entity_type,edge_targetid 
                       FROM temp_cable_routes
                       INNER JOIN line_master lm ON lm.system_id = temp_cable_routes.edge_targetid 
                       AND lm.entity_type = 'Cable' 
						inner join  point_master pm 
                       on ST_WITHIN(pm.sp_geometry, ST_BUFFER_METERS(ST_STARTPOINT(lm.sp_geometry), 2)) 
                       AND pm.entity_type IN ('BDB','FDB','SpliceClosure','FMS') 
                       WHERE temp_cable_routes.user_id = p_user_id )
    UPDATE temp_cable_routes 
    SET 
        a_system_id = startCte.system_id,
        a_entity_type = startCte.entity_type
		from startCte
     WHERE temp_cable_routes.user_id = p_user_id and temp_cable_routes.edge_targetid=startCte.edge_targetid;

with endCte as(
SELECT pm.system_id,pm.entity_type,edge_targetid 
                       FROM temp_cable_routes
                       INNER JOIN line_master lm ON lm.system_id = temp_cable_routes.edge_targetid 
                       AND lm.entity_type = 'Cable' 
						inner join  point_master pm 
                       on ST_WITHIN(pm.sp_geometry, ST_BUFFER_METERS(ST_ENDPOINT(lm.sp_geometry), 2)) 
                       AND pm.entity_type IN ('BDB','FDB','SpliceClosure','FMS') 
                       WHERE temp_cable_routes.user_id = p_user_id )
    UPDATE temp_cable_routes 
    SET 
        b_system_id = endCte.system_id,
        b_entity_type = endCte.entity_type
		from endCte
    WHERE temp_cable_routes.user_id = p_user_id and temp_cable_routes.edge_targetid=endCte.edge_targetid;
   
 --- downstreamnodes
       
      select( SELECT array_to_json(array_agg(row_to_json(x)))
    FROM (
        SELECT a_system_id AS id, a_entity_type AS label, a_entity_type AS group, NULL AS title 
        FROM temp_cable_routes 
        WHERE user_id = p_user_id 

        UNION 

        SELECT b_system_id, b_entity_type, b_entity_type, NULL 
        FROM temp_cable_routes 
        WHERE user_id = p_user_id 
    ) x  )   INTO nodes;
       
--- upstreamedges
       
Select (select array_to_json(array_agg(row_to_json(x)))from (
select routes.a_system_id as from, routes.b_system_id as to,
CONCAT('<b>','','\n <b>',att.network_id,
case when via_port_no is not null then CONCAT('(',via_port_no,')') else '' end,''||'\n'||'\n') as label,'{"type": "curvedCCW","roundness": 0.1}'::json as smooth,
case WHEN color_code IS NULL THEN
CASE
WHEN upper(att.cable_type::text) = 'OVERHEAD'::text THEN '{"color": "#FF0000"}'::json
WHEN upper(att.cable_type::text) = 'UNDERGROUND'::text THEN '{"color": "#0000FF"}'::json
WHEN upper(att.cable_type::text) = 'WALL CLAMPED'::text THEN '{"color": "#FD10FD"}'::json
WHEN upper(att.cable_type::text) = 'ISP'::text THEN '{"color": "#000"}'::json
ELSE '{"color": "#0000FF"}'::json
END
ELSE concat('{"color": "',color_code,'"}')::json
END as color,
500 as length,'{"from": {"enabled": false,"scaleFactor": 0.3,"type": "circle"},"to": null}'::json as arrows,
CASE
WHEN upper(att.network_status::text) = 'P'::text THEN true
WHEN upper(att.network_status::text) = 'A'::text THEN false
ELSE true
END as dashes,
3 as width from temp_cable_routes routes inner join att_details_cable att 
on routes.edge_targetid = att.system_id 
) x) into edges;

--- legends
select (select array_to_json(array_agg(row_to_json(x)))from (
select distinct a_entity_type as entity_type,a_entity_type as entity_title, false as upstream 
from ( SELECT a_system_id,a_entity_type FROM temp_cable_routes  where user_id = p_user_id 
        union
        SELECT b_system_id,b_entity_type FROM temp_cable_routes where user_id = p_user_id ) t
) x) into legends;

-- cables

select (select array_to_json(array_agg(row_to_json(x)))from (
Select case WHEN color_code IS NULL THEN
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN '#FF0000'::text
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN '#0000FF'::text
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN '#FD10FD'::text
WHEN upper(t.cable_type::text) = 'ISP'::text THEN '#000'::text
ELSE '#0000FF'::text
END
ELSE t.color_code::text
END as color_code,CONCAT(COALESCE(t.cable_category,''),
CASE
WHEN upper(t.cable_type::text) = 'OVERHEAD'::text THEN ' OH'
WHEN upper(t.cable_type::text) = 'UNDERGROUND'::text THEN ' UG'
WHEN upper(t.cable_type::text) = 'WALL CLAMPED'::text THEN ' WC'
WHEN upper(t.cable_type::text) = 'ISP'::text THEN ' ISP'
ELSE ''
END ,
'(',t.total_core,'F)')as text from(
select color_code,cable_category,cable_type,total_core
from temp_cable_routes
group by color_code,cable_category,cable_type,total_core
)t where t.cable_type is not null
) x) into cables;
   
   ELSE
        RESULT := 'ERROR IN pgr_createTopology';
        RETURN QUERY SELECT json_build_object('status', FALSE, 'message', RESULT);
        RETURN;
    END IF;

    RETURN QUERY 
  select row_to_json(result1) 
from (select  nodes,edges , legends, cables
)result1	;
	
END; 


$function$
;
