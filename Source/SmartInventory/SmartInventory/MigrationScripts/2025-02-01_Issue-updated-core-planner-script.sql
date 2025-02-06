CREATE OR REPLACE FUNCTION public.fn_trg_audit_line_master()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$



declare

v_served_by_ring text;

v_dsa_system_id integer;

v_layer_table text;
v_subarea_system_id integer;

begin

                v_dsa_system_id:=0;

IF (TG_OP = 'INSERT' ) THEN 

 

				INSERT INTO public.audit_line_master(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,is_virtual,action,modified_by,gis_design_id)

				values(new.system_id,new.entity_type,new.approval_flag,new.sp_geometry,new.creator_remark,new.approver_remark,new.created_by,new.approver_id,new.common_name,new.db_flag,new.approval_date,now(),new.network_status,new.is_virtual,'I',new.modified_by,new.gis_design_id);

 

                select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(new.entity_type);

                select system_id into v_dsa_system_id  from polygon_master where entity_type='DSA' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1;                    
                update att_details_subarea set is_association_completed=false where system_id in(select system_id from polygon_master where entity_type='SubArea' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1);

				if(coalesce(v_dsa_system_id,0)>0)

                then

                                select served_by_ring into v_served_by_ring from att_details_dsa where system_id=v_dsa_system_id;

                                execute 'update '||v_layer_table||' set served_by_ring='''||v_served_by_ring||''' where system_id='||new.system_id||' ';

                end if;
				-- IF (new.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 				THEN
-- 					PERFORM fn_update_entity_geojson('INSERT',new.entity_type,'Line',new.SYSTEM_ID, new.sp_geometry);
-- 				END IF;
    PERFORM fn_update_routing_topology(new.system_id,'INSERT');

END IF;

 

IF (TG_OP = 'UPDATE' ) THEN 

IF((select st_astext(new.sp_geometry)) !=(select st_astext(old.sp_geometry))) then

 

                INSERT INTO public.audit_line_master(system_id,entity_type,approval_flag,sp_geometry,creator_remark,approver_remark,created_by,approver_id,common_name,db_flag,approval_date,modified_on,network_status,is_virtual,action,modified_by,gis_design_id)

                values(new.system_id,new.entity_type,new.approval_flag,new.sp_geometry,new.creator_remark,new.approver_remark,new.created_by,new.approver_id,new.common_name,new.db_flag,new.approval_date,now(),new.network_status,new.is_virtual,'U',new.modified_by,new.gis_design_id); 

 

                select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(new.entity_type);

                select system_id into v_dsa_system_id  from polygon_master where entity_type='DSA' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1;                    
                update att_details_subarea set is_association_completed=false where system_id in(select system_id from polygon_master where entity_type='SubArea' and (st_within(st_endpoint(new.sp_geometry),sp_geometry) or st_within(st_startpoint(new.sp_geometry),sp_geometry)) limit 1);

                if(coalesce(v_dsa_system_id,0)>0)

                then

                                select served_by_ring into v_served_by_ring from att_details_dsa where system_id=v_dsa_system_id;

                                execute 'update '||v_layer_table||' set served_by_ring='''||v_served_by_ring||''' where system_id='||new.system_id||' ';

                end if;
				
				-- IF (new.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 				THEN
-- 					PERFORM fn_update_entity_geojson('UPDATE', new.entity_type, 'Line', new.SYSTEM_ID, new.sp_geometry);
-- 				END IF;
END IF;

perform fn_update_routing_topology(new.system_id,'UPDATE');

END IF;

 
IF (TG_OP = 'DELETE' ) THEN 
-- 		IF (old.entity_type = ANY('{Cable,Trench}'::character varying[]))
-- 		THEN
-- 			PERFORM fn_update_entity_geojson('DELETE', old.entity_type, 'Line', old.SYSTEM_ID, old.sp_geometry);
-- 		END IF;

 PERFORM fn_update_routing_topology(old.system_id,'DELETE');

	RETURN OLD;
END IF;
RETURN NEW;

END;

$function$
;

-----------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_refresh_routing_data()
 RETURNS TABLE(status boolean, message text)
 LANGUAGE plpgsql
AS $function$
declare

begin

	TRUNCATE TABLE routing_data_core_plan RESTART IDENTITY;

	insert into routing_data_core_plan (id,system_id,cost,reverse_cost,geom)
	SELECT 
    c.system_id AS id, 
    c.system_id AS system_id, 
    ST_Length(lm.sp_geometry::geography)/1000 AS cost,
    ST_Length(lm.sp_geometry::geography)/1000 AS reverse_cost,
    ST_Force2D(lm.sp_geometry) AS geom 
FROM  line_master lm  JOIN  vw_att_details_cable c ON 
    lm.system_id = c.system_id 
WHERE 
    lm.entity_type = 'Cable';

	RETURN QUERY Select true as status, 'Routing Data created successfully!'::text  as message;


END; 

$function$
;

---------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_update_routing_topology(p_cable_id integer, p_operation text)
 RETURNS TABLE(status boolean, message text)
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_existing_count INTEGER;
    v_result TEXT;
    v_geom GEOMETRY;
    v_cost DOUBLE PRECISION;
    v_reverse_cost DOUBLE PRECISION;
BEGIN
    SELECT 
        ST_Force2D(lm.sp_geometry) AS geom,
        ST_Length(lm.sp_geometry::geography) / 1000 AS cost,
        ST_Length(lm.sp_geometry::geography) / 1000 AS reverse_cost
    INTO v_geom, v_cost, v_reverse_cost
    FROM line_master lm
    JOIN vw_att_details_cable c 
        ON lm.system_id = c.system_id
    WHERE lm.entity_type = 'Cable'
    AND c.system_id = p_cable_id;

    
   
IF p_cable_id = 0 THEN

            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph' AS message;
            END IF;

            RETURN QUERY SELECT TRUE AS status, 'Routing topology updated' AS message;
    
else
	IF v_geom IS NULL THEN
	        RETURN QUERY SELECT FALSE AS status, 'Cable ID not found or no geometry data available.' AS message;
	    END IF;
   
    IF p_operation = 'INSERT' THEN
        SELECT COUNT(*) INTO v_existing_count
        FROM routing_data_core_plan
        WHERE id = p_cable_id;

        IF v_existing_count = 0 THEN
            INSERT INTO routing_data_core_plan (id, system_id, source, target, cost, reverse_cost, geom)
            VALUES (p_cable_id, p_cable_id, NULL, NULL, v_cost, v_reverse_cost, v_geom);
            
            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for INSERT' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for INSERT' AS message;
            END IF;

            RETURN QUERY SELECT TRUE AS status, 'Cable inserted successfully and graph updated' AS message;
        ELSE
            RETURN QUERY SELECT FALSE AS status, 'Cable ID already exists for INSERT operation' AS message;
        END IF;

    ELSIF p_operation = 'UPDATE' THEN
        SELECT COUNT(*) INTO v_existing_count
        FROM routing_data_core_plan
        WHERE id = p_cable_id;

        IF v_existing_count > 0 THEN
            UPDATE routing_data_core_plan
            SET geom = v_geom, cost = v_cost, reverse_cost = v_reverse_cost, source = NULL, target = NULL
            WHERE id = p_cable_id;
            
            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for UPDATE' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for UPDATE' AS message;
            END IF;

            RETURN QUERY SELECT TRUE AS status, 'Cable updated successfully and graph updated' AS message;
        ELSE
            RETURN QUERY SELECT FALSE AS status, 'Cable ID does not exist for UPDATE operation' AS message;
        END IF;

    ELSIF p_operation = 'DELETE' THEN
        SELECT COUNT(*) INTO v_existing_count
        FROM routing_data_core_plan
        WHERE id = p_cable_id;

        IF v_existing_count > 0 THEN
            DELETE FROM routing_data_core_plan
            WHERE id = p_cable_id;

            SELECT pgr_createTopology('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in creating topology for DELETE' AS message;
            END IF;
            SELECT pgr_analyzegraph('routing_data_core_plan', 0.000025, 'geom') INTO v_result;
            IF v_result <> 'OK' THEN
                RETURN QUERY SELECT FALSE AS status, 'Error in analyzing graph for DELETE' AS message;
            END IF;

            RETURN QUERY SELECT TRUE AS status, 'Cable deleted successfully and graph updated' AS message;
        ELSE
            RETURN QUERY SELECT FALSE AS status, 'Cable ID does not exist for DELETE operation' AS message;
        END IF;
    ELSE
        RETURN QUERY SELECT FALSE AS status, 'Invalid operation type. Please use INSERT, UPDATE, or DELETE.' AS message;
    END IF;
   END IF;
END;
$function$
;


-----------------------------------------------------------------------------------------------------

-- public.routing_data_core_plan definition

-- Drop table

-- DROP TABLE public.routing_data_core_plan;

CREATE TABLE public.routing_data_core_plan (
	id int4 NULL,
	system_id int4 NULL,
	"source" int4 NULL,
	target int4 NULL,
	"cost" float8 NULL,
	reverse_cost float8 NULL,
	geom public.geometry NULL
);
CREATE INDEX routing_data_core_plan_geom_idx ON public.routing_data_core_plan USING gist (geom);
CREATE INDEX routing_data_core_plan_id_idx ON public.routing_data_core_plan USING btree (id);
CREATE INDEX routing_data_core_plan_source_idx ON public.routing_data_core_plan USING btree (source);
CREATE INDEX routing_data_core_plan_target_idx ON public.routing_data_core_plan USING btree (target);

---------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_validation(source_network_id character varying, destination_network_id character varying, buffer integer, required_core integer, p_user_id integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
DECLARE 
    rec RECORD;
    total_core integer;
    p_source character varying;
    p_destination character varying;
    availableCoreDestination integer;
    usedCore integer;
    source_system_id integer;
    p_destination_system_id integer;
    availableCoreSource integer;
   t_count integer;
  cableNetworkId character varying;
BEGIN
    availableCoreDestination := 0;
    usedCore := 0;
    t_count :=0;
    -- Create temporary tables
    CREATE TEMP TABLE temp_cable_routes(
        seq integer,
        path_seq integer, 
        edge_targetid integer, 
        roadline_geomtext text, 
        start_point character varying,
        end_point character varying,
        message varchar NULL,
        is_valid boolean DEFAULT true,
        avaiable_core_count integer DEFAULT 0,
        user_id integer
    ) ON COMMIT DROP;

    CREATE TEMP TABLE temp_isp_port_info(
        system_id int4 NULL,
        network_id varchar NULL,
        parent_system_id int4 NULL,
        parent_network_id varchar NULL,
        port_status_id int4 NULL,
        is_valid boolean DEFAULT true,
        port_number int4 null,
        parent_entity_type varchar NULL
    ) ON COMMIT DROP;

    -- Clear previous logs for the user
    DELETE FROM core_planner_logs WHERE user_id = p_user_id;

    -- Get source and destination points
    SELECT longitude || ' ' || latitude AS geom, system_id
    INTO p_source, source_system_id
    FROM att_details_fms
    WHERE network_id = source_network_id;

    SELECT longitude || ' ' || latitude AS geom, system_id
    INTO p_destination, p_destination_system_id
    FROM att_details_fms
    WHERE network_id = destination_network_id;

    -- Calculate the available cores for the source system
    SELECT  
        COUNT(CASE WHEN port_status_id = 1 and input_output = 'O' THEN 1 END) AS availableCore,
        COUNT(CASE WHEN port_status_id > 1 and input_output = 'O' THEN 1 END) AS usedCore
    INTO availableCoreSource, usedCore
    FROM isp_port_info
    WHERE parent_system_id = source_system_id 
      AND parent_entity_type = 'FMS';

    -- Calculate the available cores for the destination system
    SELECT COUNT(*)
    INTO availableCoreDestination
    FROM isp_port_info
    WHERE parent_system_id = p_destination_system_id 
      AND parent_entity_type = 'FMS' 
      AND port_status_id = 1 
      AND input_output = 'O';

   if not exists (select 1 from att_details_fms where network_id = source_network_id) THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'The ODF1 does not exist. Please enter a valid ODF.' AS message) row;
    END IF;
   
    if not exists (select 1 from att_details_fms where network_id = destination_network_id) THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'The ODF2 does not exist. Please enter a valid ODF.' AS message) row;
    END IF;
	
    -- Validate if both counts match the requiredCore
    IF availableCoreSource < required_core AND availableCoreDestination < required_core THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Required port is not available in ODF' AS message) row;
    END IF;

	    -- Populate temporary cable routes table
		  INSERT INTO temp_cable_routes (seq, path_seq, edge_targetid, user_id)
		   
		SELECT seq, path_seq, edge,p_user_id FROM pgr_dijkstra('SELECT id, source, target, cost, reverse_cost 
	    FROM routing_data_core_plan', (SELECT id
		FROM routing_data_core_plan_vertices_pgr
		WHERE ST_Within(
		    the_geom, ST_BUFFER_METERS(ST_GeomFromText('POINT(' || p_source || ')', 4326), 2)
		)limit 1 ),
		(SELECT id
		FROM routing_data_core_plan_vertices_pgr
		WHERE ST_Within(
		    the_geom, 
		    ST_BUFFER_METERS(ST_GeomFromText('POINT('|| p_destination ||')', 4326), 2)
		) limit 1 )
		);
	
delete from temp_cable_routes where edge_targetid = -1 and user_id = p_user_id;

if not exists (select 1 from temp_cable_routes where user_id = p_user_id) 
then

 RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'There is no existing route between the ODFs.' AS message) row;
RETURN;
 
end if;
    -- Populate temporary ISP port info table
    INSERT INTO temp_isp_port_info (system_id, network_id, parent_system_id, parent_network_id, port_status_id, port_number,parent_entity_type)
    SELECT system_id, network_id, parent_system_id, parent_network_id, port_status_id, port_number,parent_entity_type
    FROM isp_port_info a 
    WHERE a.parent_system_id = source_system_id  
    AND a.parent_entity_type = 'FMS'
    AND port_status_id = 1 
    AND input_output = 'O'
    ORDER BY system_id;

    -- Loop through each record in temp_isp_port_info
 FOR rec IN (SELECT * FROM temp_isp_port_info) LOOP

        -- Update temp_cable_routes for mismatched cable_ids
        UPDATE temp_cable_routes
        SET           
            avaiable_core_count = COALESCE(avaiable_core_count, 0) + 1
			
        WHERE edge_targetid IN (
           SELECT DISTINCT aci.cable_id
          FROM att_details_cable_info aci
         WHERE aci.cable_id IN (SELECT edge_targetid FROM temp_cable_routes where  user_id = p_user_id)
        AND aci.cable_id IN (
      SELECT cable_id
      FROM att_details_cable_info
      WHERE fiber_number = rec.port_number)
  AND aci.a_end_status_id = 1
  AND aci.b_end_status_id = 1) and  user_id = p_user_id;

if ((select count(1) from(SELECT DISTINCT aci.cable_id
          FROM att_details_cable_info aci
         WHERE aci.cable_id IN (SELECT edge_targetid FROM temp_cable_routes where  user_id = p_user_id)
        AND aci.cable_id IN (
      SELECT cable_id
      FROM att_details_cable_info
      WHERE fiber_number = rec.port_number)
  AND aci.a_end_status_id = 1
  AND aci.b_end_status_id = 1)a) = (SELECT count(1) FROM temp_cable_routes where  user_id = p_user_id))
then

update isp_port_info set is_valid_for_core_plan=true where parent_system_id=rec.parent_system_id
and parent_entity_type=rec.parent_entity_type and port_number=rec.port_number AND input_output = 'O';
end if;

END LOOP;

  update temp_cable_routes set message = 'Required core is not available', is_valid = false where 
  avaiable_core_count  <  required_core and user_id = p_user_id;

    -- Log valid cables
    INSERT INTO core_planner_logs(
        cable_id, cable_name, network_status, total_core, cable_length, 
		--a_system_id, b_system_id, a_entity_type, b_entity_type, 
		error_msg, is_valid, user_id, 
       -- a_network_id, b_network_id, 
		cable_network_id, avaiable, used_core
    )
    SELECT 
        att.system_id, att.cable_name, att.network_status, att.total_core, att.cable_calculated_length,
        --att.a_system_id, att.b_system_id, att.a_entity_type, att.b_entity_type, 
		info.message, info.is_valid,
        p_user_id, --att.a_network_id, att.b_network_id, 
		att.network_id, 
        (SELECT COUNT(*) FROM att_details_cable_info aci WHERE aci.cable_id = att.system_id
         AND aci.a_end_status_id = 1 AND aci.b_end_status_id = 1) AS available_cores,
        (SELECT COUNT(*) FROM att_details_cable_info aci WHERE aci.cable_id = info.edge_targetid
         AND (aci.a_end_status_id > 1 or aci.b_end_status_id > 1 ) ) AS used_core
    FROM att_details_cable att 
    LEFT JOIN temp_cable_routes info ON att.system_id = info.edge_targetid and  info.user_id = p_user_id
    WHERE att.system_id IN (SELECT edge_targetid FROM temp_cable_routes where user_id = p_user_id);

    -- Update system_id and entity_type for a and b ends
	
	with startCte as(
   SELECT pm.system_id,pm.entity_type,pm.common_name,cable_id 
                       FROM core_planner_logs
                       INNER JOIN line_master lm ON lm.system_id = core_planner_logs.cable_id 
                       AND lm.entity_type = 'Cable' 
						inner join  point_master pm 
                       on ST_WITHIN(pm.sp_geometry, ST_BUFFER_METERS(ST_STARTPOINT(lm.sp_geometry), 3)) 
                       AND pm.entity_type IN ('BDB','FDB','SpliceClosure','FMS') 
                       WHERE core_planner_logs.user_id = p_user_id )
    UPDATE core_planner_logs 
    SET 
        a_system_id = startCte.system_id,
        a_entity_type = startCte.entity_type,
		a_network_id=startCte.common_name
		from startCte
     WHERE core_planner_logs.user_id = p_user_id and core_planner_logs.cable_id=startCte.cable_id;

with endCte as(
SELECT pm.system_id,pm.entity_type,pm.common_name,cable_id 
                       FROM core_planner_logs
                       INNER JOIN line_master lm ON lm.system_id = core_planner_logs.cable_id 
                       AND lm.entity_type = 'Cable' 
						inner join  point_master pm 
                       on ST_WITHIN(pm.sp_geometry, ST_BUFFER_METERS(ST_ENDPOINT(lm.sp_geometry), 3)) 
                       AND pm.entity_type IN ('BDB','FDB','SpliceClosure','FMS') 
                       WHERE core_planner_logs.user_id = p_user_id )
    UPDATE core_planner_logs 
    SET 
        b_system_id = endCte.system_id,
        b_entity_type = endCte.entity_type,
		b_network_id=endCte.common_name
		from endCte
    WHERE core_planner_logs.user_id = p_user_id and core_planner_logs.cable_id=endCte.cable_id;

    -- Mark invalid records if no termination points
    UPDATE core_planner_logs 
    SET is_valid = false 
    WHERE coalesce(b_system_id, 0) = 0 OR coalesce(a_system_id, 0) = 0 
    AND user_id = p_user_id;

    -- Validate if both FMS points are found

	IF (SELECT COUNT(*) 
        FROM core_planner_logs logs 
        WHERE (logs.a_entity_type = 'FMS' OR logs.b_entity_type = 'FMS') 
          AND logs.user_id = p_user_id) != 2 THEN
        -- Invalidate all records for the user
        UPDATE core_planner_logs
        SET is_valid = FALSE
        WHERE user_id = p_user_id;
    END IF;

    -- Validate if `a_system_id` or `b_system_id` is null
  UPDATE core_planner_logs log2
  SET 
  is_valid = FALSE,
  error_msg = 'Cable is not terminated properly'
  FROM core_planner_logs log1
  WHERE log2.user_id = p_user_id
  AND log2.user_id = log1.user_id
  AND (log2.a_system_id IS NULL OR log2.b_system_id IS NULL)
  AND log2.cable_id = log1.cable_id;

    -- Return result
    IF exists (SELECT 1 FROM core_planner_logs WHERE user_id = p_user_id AND is_valid = false )  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT false AS status, 'Required core is not available from '||source_network_id ||' to '||destination_network_id  AS message) row;
    ELSE
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT true AS status, 'Required Core available' AS message) row;
    END IF;

END;
$function$
;

-------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_core_planner_splicing(required_core integer, p_user_id integer, fiber_link_network_id character varying, source_network_id character varying, destination_network_id character varying, buffer integer)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE 
    rec RECORD;
   v_arow RECORD;
    v_network_id character varying;
    v_system_id integer;
    v_cable_details_left record;
    v_cable_details_right record;
    v_cable_details record;
   p_link_system_id integer;
   start_point integer;
  v_status bool;
  v_message  character varying;
V_IS_CABLE_A_END BOOLEAN;
BEGIN
V_IS_CABLE_A_END:=FALSE;
CREATE TEMP TABLE TEMP_LINE_MASTER
(
SYSTEM_ID INTEGER,
entity_type character varying,
network_id character varying,
SP_GEOMETRY GEOMETRY
)ON COMMIT DROP;

CREATE TEMP TABLE TEMP_POINT_MASTER
(
A_SYSTEM_ID INTEGER,
A_entity_type character varying,
A_network_id character varying,
SP_GEOMETRY GEOMETRY
)ON COMMIT DROP;

INSERT INTO TEMP_POINT_MASTER(A_SYSTEM_ID,A_network_id,A_entity_type,SP_GEOMETRY)
select A.a_system_id,P.COMMON_NAME,A.a_entity_type,P.SP_GEOMETRY from (      
         SELECT a_system_id,a_entity_type FROM core_planner_logs  where user_id = p_user_id and is_valid =TRUE
        union
        SELECT b_system_id,b_entity_type FROM core_planner_logs where user_id = p_user_id and is_valid =TRUE )a
		INNER JOIN point_master P ON P.SYSTEM_ID=A.a_system_id AND P.entity_type=A.a_entity_type;

INSERT INTO TEMP_LINE_MASTER(SYSTEM_ID,network_id,entity_type,SP_GEOMETRY)       
SELECT CABLE_id,CABLE_network_id,'Cable',P.SP_GEOMETRY FROM core_planner_logs A 
INNER JOIN LINE_master P ON P.SYSTEM_ID=A.CABLE_id AND P.entity_type='Cable'
where user_id = p_user_id and is_valid =TRUE ;




		
    v_network_id := '';
    v_system_id := 0;
-- truncate table temp_connection restart identity;
    -- Create a temporary table for connection data
    CREATE TEMP TABLE temp_connection (
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
    ) ON COMMIT DROP;
     
  SELECT 
        (result_json->>'status')::BOOLEAN, 
        result_json->>'message'
    INTO v_status, v_message
    FROM fn_get_core_planner_validation(source_network_id, destination_network_id, buffer, required_core, p_user_id) AS result_json;
   
   IF v_status = false  THEN
        RETURN QUERY SELECT row_to_json(row) 
        FROM (SELECT v_status AS status, v_message AS message) row;
    ELSE
   select system_id into p_link_system_id from att_details_fiber_link adfl 
   where upper(network_id) = upper(fiber_link_network_id)
   or upper(link_id)=upper(fiber_link_network_id);        
        FOR rec IN select * from TEMP_POINT_MASTER order by a_entity_type
        LOOP									
        raise info 'rec 1 : %',rec;
		if(rec.a_entity_type='FMS')
		then
		
			SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) AS IS_CABLE_A_END INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
			WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) or
			ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)));				    

					INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source)
					
					select rec.a_system_id, rec.a_network_id, rec.a_entity_type, 
                    port_number, 
                    v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    port_number,false,v_cable_details_left.IS_CABLE_A_END, 
                    p_user_id, 'CorePlanning' 
					from isp_port_info 
					where is_valid_for_core_plan=true 
					and parent_network_id =source_network_id
					and input_output='O' and port_status_id=1
					order by port_number limit required_core; 		   							           		   
		else

		

	SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) AS IS_CABLE_A_END 
	INTO v_cable_details_left FROM TEMP_LINE_MASTER A				
	WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
	OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) LIMIT 1;
		
		
	SELECT a.*,ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2)) AS IS_CABLE_A_END 
	INTO v_cable_details_right FROM TEMP_LINE_MASTER A				
	WHERE (ST_Within(ST_STARTPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))
	OR ST_Within(ST_ENDPOINT(A.SP_GEOMETRY), ST_BUFFER_METERS(REC.SP_GEOMETRY, 2))) 
	AND SYSTEM_ID NOT IN(v_cable_details_left.SYSTEM_ID) LIMIT 1;
		
        

		INSERT INTO temp_connection (
                    source_system_id, source_network_id, source_entity_type, source_port_no, 
                    destination_system_id, destination_network_id, destination_entity_type,
                    destination_port_no, is_source_cable_a_end, is_destination_cable_a_end, 
                    created_by, splicing_source,equipment_system_id, equipment_network_id, equipment_entity_type)
				
		select v_cable_details_left.system_id, v_cable_details_left.network_id, 'Cable', 
                    port_number, 
                    v_cable_details_right.system_id, v_cable_details_right.network_id, 'Cable', 
                    port_number,
                    v_cable_details_left.IS_CABLE_A_END,
                    v_cable_details_right.IS_CABLE_A_END, 
                    p_user_id, 'CorePlanning',rec.a_system_id,rec.a_network_id,rec.a_entity_type 
					from isp_port_info 
					-- inner join att_details_cable_info c 
-- 					on c.cable_id=v_cable_details_left.system_id and c.fiber_number=port_number
-- 					and (case when v_cable_details_left.IS_CABLE_A_END then is_a_end_through_connectivity=false
-- 							  when v_cable_details_left.IS_CABLE_A_END=false then is_b_end_through_connectivity=false
-- 						else 1=1 end)
					where is_valid_for_core_plan=true 
					and parent_network_id =source_network_id
					and input_output='O' and port_status_id=1					
					order by port_number limit required_core;							    		                 	
		end if;

            
        END LOOP;
		
		update isp_port_info set is_valid_for_core_plan=false 
		where parent_network_id =source_network_id
		and parent_entity_type='FMS' 
		--and port_number=v_arow.port_number 
		AND input_output = 'O' 
		and is_valid_for_core_plan=true;

           raise info 'end loop 1 : %',p_user_id;
          
     -- UPDATE temp_connection tmp
     -- SET source_network_id = pm.common_name
     -- FROM point_master pm
     -- WHERE pm.entity_type=tmp.source_entity_type and tmp.source_entity_type IN ('SpliceClosure', 'FMS','FDB','BDB')
     -- AND pm.system_id = tmp.source_system_id ;

     -- UPDATE temp_connection tmp
     -- SET equipment_network_id  = pm.common_name
     -- FROM point_master pm
     -- WHERE pm.entity_type=tmp.equipment_entity_type and tmp.equipment_entity_type IN ('SpliceClosure','FDB','BDB')
     -- AND pm.system_id = tmp.equipment_system_id ;
 
 
if (SELECT COUNT(*) FROM temp_connection) > 1
then 
 raise info 'end splicing 1 : %',p_user_id;
perform(fn_auto_provisioning_save_connections());

FOR rec in (select destination_system_id,destination_port_no from temp_connection where destination_entity_type = 'Cable' and created_by = p_user_id
union 
select source_system_id, source_port_no from temp_connection where source_entity_type = 'Cable' and created_by = p_user_id)
loop 

perform fn_associate_fiber_link_cable(rec.destination_system_id,coalesce(p_link_system_id,0),rec.destination_port_no,'A' );
	
end loop;
--UPDATE att_details_fiber_link
--SET fiber_link_status = 'Associated' where system_id=p_link_system_id;
end if;	

  --    update core_planner_logs set used_core = used_core + required_core,avaiable= case when avaiable - required_core < 0 
   --   then 0 else avaiable - required_core end where user_id = p_user_id and is_valid =true;
     
       delete from core_planner_logs where user_id = p_user_id;

       return query select row_to_json(row) from ( select true as status, 'ODF to ODF splicing successfully completed' as message ) row;
      end if;
  END;
 
$function$
;

-----------------------------------------------------------------------------------
	select * from fn_refresh_routing_data();
    select * from fn_update_routing_topology(0,'INSERT');