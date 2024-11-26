UPDATE public.module_master
SET  is_active=true
WHERE module_name='Approve/Reject Building';

----------------------------------------------------------------------------

ALTER TABLE connection_info
ENABLE TRIGGER trg_update_core_port_status; 

---------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_associate_fiber_link_cable(p_cable_id integer, p_link_system_id integer, p_fiber_number integer, p_action character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$
 
DECLARE
rec record;
p_a_end_id integer;
p_b_end_id integer;
v_count integer;
v_fms_count integer;
BEGIN

v_fms_count:= 0;
 --TEMP TABLE TO STORE CONNECTION INFO RESULT..
create temp table cpf_temp_bkp
(
	id integer, 
	connection_id integer,
	rowid integer,
	parent_connection_id integer,
	source_system_id integer, 
	source_network_id character varying,
	source_entity_type character varying, 
	source_port_no integer, 
	is_source_virtual boolean,
	destination_system_id integer, 
	destination_network_id character varying,
	destination_entity_type character varying,
	destination_port_no integer, 
	is_destination_virtual boolean, 
	is_customer_connected boolean, 
	created_on timestamp without time zone, 
	created_by integer, 
	approved_by integer, 
	approved_on timestamp without time zone, 
	cable_calculated_length double precision,
	cable_measured_length double precision,
	splitter_ratio character varying,
	is_backward_path boolean,
	trace_end character varying,
	source_display_name character varying,
	destination_display_name character varying
	 
	) on commit drop;

	create temp table cpf_temp_route
	(
	route_id character varying,
	fiber_number integer
	) on commit drop;
	--select cable_measured_length from att_details_cable 
	
	insert into cpf_temp_bkp(id, connection_id,rowid,parent_connection_id,source_system_id, source_network_id,source_entity_type, source_port_no, is_source_virtual,destination_system_id, destination_network_id,
		destination_entity_type,destination_port_no, is_destination_virtual, is_customer_connected, created_on, created_by, approved_by, approved_on, cable_calculated_length ,
		cable_measured_length,splitter_ratio,is_backward_path,trace_end,source_display_name,destination_display_name)
		select a.id, a.connection_id,a.rowid,a.parent_connection_id,a.source_system_id, a.source_network_id,a.source_entity_type, a.source_port_no, a.is_source_virtual,a.destination_system_id, a.destination_network_id,a.destination_entity_type,a.destination_port_no, a.is_destination_virtual, a.is_customer_connected, a.created_on, a.created_by, a.approved_by, a.approved_on, a.cable_calculated_length ,
		a.cable_measured_length,a.splitter_ratio,a.is_backward_path,a.trace_end,a.source_display_name,a.destination_display_name from fn_get_connection_info('', '', 1, 10,null, null, p_cable_id, p_fiber_number,'Cable') a;

insert into cpf_temp_route(route_id,fiber_number)
select route_id,fiber_number
from att_details_cable_info inf
inner join 
att_details_cable cbl on cbl.system_id=inf.cable_id
where link_system_id=p_link_system_id
group by cbl.route_id,inf.fiber_number;

select id into p_a_end_id from cpf_temp_bkp where trace_end='A' and (lower(source_entity_type) in ('splitter','ont','fms') or(lower(destination_entity_type) 
in ('splitter','ont','fms'))) order by id limit 1;

select id into p_b_end_id from cpf_temp_bkp where trace_end='B' and (lower(source_entity_type) in ('splitter','ont','fms') or(lower(destination_entity_type) 
in ('splitter','ont','fms'))) order by id limit 1;


CREATE TEMP TABLE temp_cable_info AS
select  distinct a.* from (
select source_system_id as cable_system_id,source_network_id as cable_network_id,source_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='A'  and case when coalesce(p_a_end_id,0)>0 then id<p_a_end_id else true end and upper(source_entity_type)=upper('cable')
union
select destination_system_id as cable_system_id,destination_network_id as cable_network_id,destination_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='A'  and case when coalesce(p_a_end_id,0)>0 then id<p_a_end_id else true end and upper(destination_entity_type)=upper('Cable')
union
select source_system_id as cable_system_id,source_network_id as cable_network_id,source_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='B'  and case when coalesce(p_b_end_id,0)>0 then id<p_b_end_id else true end and upper(source_entity_type)=upper('cable')
union
select destination_system_id as cable_system_id,destination_network_id as cable_network_id,destination_port_no as cable_fiber_number  from cpf_temp_bkp where trace_end='B'  and case when coalesce(p_b_end_id,0)>0 then id<p_b_end_id else true end  and upper(destination_entity_type)=upper('Cable')
) a;
  IF EXISTS(SELECT 1 from ATT_DETAILS_CABLE_INFO where cable_id=p_cable_id)THEN
	IF(p_action='A')THEN 
		UPDATE ATT_DETAILS_CABLE_INFO set link_system_id=p_link_system_id where cable_id=p_cable_id and fiber_number=p_fiber_number;
		-- UPDATE FIBER LINK STATUS TO ASSOCIATE 
IF (SELECT COUNT(*) FROM cpf_temp_bkp) > 0 then

 UPDATE att_details_fiber_link
SET fiber_link_status = 'Associated',
    gis_length = ROUND(
        COALESCE(
            (SELECT SUM(ctb.cable_measured_length) FROM cpf_temp_bkp ctb), 0
        )::NUMERIC, 3
    ),
    total_route_length = ROUND(
        COALESCE(
            (SELECT SUM(ctb.cable_calculated_length) FROM cpf_temp_bkp ctb), 0
        )::NUMERIC, 3
    )
WHERE system_id = p_link_system_id;

else

  UPDATE att_details_fiber_link
SET fiber_link_status = 'Associated',
    gis_length = ROUND(
        (COALESCE(
            (SELECT cable_measured_length FROM att_details_cable WHERE system_id = p_cable_id), 0
        ) + COALESCE(gis_length, 0))::NUMERIC, 3
    ),
    total_route_length = ROUND(
        (COALESCE(
            (SELECT cable_calculated_length FROM att_details_cable WHERE system_id = p_cable_id), 0
        ) + COALESCE(total_route_length, 0))::NUMERIC, 3
    )
WHERE system_id = p_link_system_id;

           
           
END IF;

		UPDATE att_details_cable_info A SET link_system_id= p_link_system_id
		FROM temp_cable_info B WHERE  B.cable_system_id=A.cable_id and B.cable_fiber_number=A.fiber_number;
		drop table temp_cable_info;
	
	ELSIF(p_action='D')THEN
	raise info'Delete1%',p_link_system_id;
	select count(*) into v_count from att_details_cable_info where link_system_id=p_link_system_id;
	raise info'Delete1%',v_count;
		
		UPDATE ATT_DETAILS_CABLE_INFO set link_system_id=0 where cable_id=p_cable_id and fiber_number=p_fiber_number;
		
		UPDATE att_details_cable_info A SET link_system_id= 0
		FROM temp_cable_info B WHERE  B.cable_system_id=A.cable_id and B.cable_fiber_number=A.fiber_number;
		drop table temp_cable_info;
		IF NOT Exists(select 1 from att_details_cable_info where link_system_id=p_link_system_id)THEN
			UPDATE att_details_fiber_link set fiber_link_status='Free',gis_length = 0,total_route_length= 0 where system_id=p_link_system_id;
			raise info'Delete2%',45;
		END IF;
		 
	END IF;
drop table if exists cpf_temp_bkp;
drop table if exists cpf_temp_route;
drop table if exists cpf_temp;

		RETURN QUERY 
		SELECT TRUE AS status,'Success!'::CHARACTER VARYING AS message;
		RETURN;
	ELSE
		RETURN QUERY
		SELECT FALSE AS status,'FAIL!'::CHARACTER VARYING AS message;
		RETURN;
	END IF;
	  
END
$function$
;


----------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_update_entity_wise_network_status(p_geom text, p_userid integer, p_selectiontype character varying, p_radius double precision, p_current_status character varying, p_entity_type character varying, p_change_network_status character varying)
 RETURNS TABLE(status boolean, message character varying)
 LANGUAGE plpgsql
AS $function$

DECLARE

 

v_Query text;

v_tablename character varying;

v_Geomtype character varying;

v_Geomtablename character varying;

 v_entity_type character varying;

REC RECORD;

BEGIN
DROP TABLE IF EXISTS temp_entity;
CREATE  TABLE temp_entity
(
  id serial,
  system_id integer,
  entity_type character varying   
);
--ON COMMIT DROP ; 

            select layer_table,geom_type,layer_name from  layer_details where  upper(layer_title)=''||upper(p_entity_type)||''  into v_tablename,v_Geomtype,v_entity_type;
          
           v_Geomtablename:=CONCAT(''||lower(v_Geomtype)||'', '_master');

              raise info '%',v_tablename;

        if(p_change_network_status='D')
        then
		insert into temp_entity(system_id,entity_type)
		select tblmain2.system_id,tblmain2.entity_type from (
		select  ent.system_id ,tblmain.system_id as con_system_id,ent.entity_type from fn_get_bulk_entity_by_geom( p_geom, p_userId, p_selectiontype, p_radius,v_entity_type,p_current_status ) ent
		left join 
		(
		select source_system_id as system_id from connection_info where upper(source_entity_type)=upper(v_entity_type)
		union
		select destination_system_id  as  system_id from connection_info where upper(destination_entity_type)=upper(v_entity_type)
		) tblmain
		on ent.system_id=tblmain.system_id) tblmain2 where tblmain2.system_id!=tblmain2.con_system_id or tblmain2.con_system_id is null;
	else
		insert into temp_entity(system_id,entity_type)
		select tblmain2.system_id,tblmain2.entity_type from (
		select  ent.system_id ,tblmain.system_id as con_system_id,ent.entity_type from fn_get_bulk_entity_by_geom( p_geom, p_userId, p_selectiontype, p_radius,v_entity_type,p_current_status ) ent
		left join 
		(
		select source_system_id as system_id from connection_info where upper(source_entity_type)=upper(v_entity_type)
		union
		select destination_system_id  as  system_id from connection_info where upper(destination_entity_type)=upper(v_entity_type)
		) tblmain
		on ent.system_id=tblmain.system_id) tblmain2 ;
	end if;

	EXECUTE 'update  '||v_tablename||' set network_status='''||p_change_network_status||''', modified_by='||p_userid||',modified_on=now() where system_id  in  (select system_id from temp_entity) ';

	EXECUTE 'update  '||v_Geomtablename||' set network_status='''||p_change_network_status||''', modified_on=now() where system_id in  (select system_id from temp_entity) and upper(entity_type)='''||upper(v_entity_type)||'''';

FOR REC IN
    SELECT system_id, entity_type FROM temp_entity
LOOP
    -- Call the function for each record
    PERFORM fn_geojson_update_entity_attribute(REC.system_id, REC.entity_type, 0, 1, false);
END LOOP;

	--perform(select fn_geojson_update_entity_attribute(system_id,entity_type,0,1,false) from temp_entity);

	if(upper(v_entity_type)=upper('ADB') or upper(v_entity_type)=upper('BDB') or upper(v_entity_type)=upper('CDB'))
	then	
		update att_details_splitter set network_status=p_change_network_status,modified_by=p_userid,modified_on=now() 
		where parent_system_id in (select system_id from temp_entity where entity_type=v_entity_type) and upper(parent_entity_type)=upper(v_entity_type);
		update point_master set network_status=p_change_network_status from att_details_splitter where point_master.system_id=att_details_splitter.system_id and upper(entity_type)=upper('splitter')
		and  parent_system_id in (select system_id from temp_entity where entity_type=v_entity_type) and upper(parent_entity_type)=upper(v_entity_type);

		perform(select fn_geojson_update_entity_attribute(system_id,'Splitter',0,1,false) from att_details_splitter where parent_system_id in (select system_id from temp_entity where entity_type=v_entity_type) and upper(parent_entity_type)=upper(v_entity_type));

		update att_details_fms set network_status=p_change_network_status,modified_by=p_userid,modified_on=now() 
		where parent_system_id in (select system_id from temp_entity where entity_type=v_entity_type) and upper(parent_entity_type)=upper(v_entity_type);
		update point_master set network_status=p_change_network_status from att_details_fms where point_master.system_id=att_details_fms.system_id and upper(entity_type)=upper('FMS')
		and  parent_system_id in (select system_id from temp_entity where entity_type=v_entity_type) and upper(parent_entity_type)=upper(v_entity_type);
		
		perform(select fn_geojson_update_entity_attribute(system_id,'FMS',0,1,false) from att_details_fms where parent_system_id in (select system_id from temp_entity where entity_type=v_entity_type) and upper(parent_entity_type)=upper(v_entity_type));
	elsif(upper(v_entity_type)=upper('structure')) and exists (select 1 from polygon_master where system_id in (select system_id from temp_entity where entity_type=v_entity_type) and upper(entity_type)=upper(v_entity_type))
	then
		update polygon_master set network_status=p_change_network_status  where system_id in (select system_id from temp_entity where entity_type=v_entity_type) and upper(entity_type)=upper(v_entity_type);		
	
	end if;
 

 --drop table temp_entity;

---For Excuting Query---

      RETURN QUERY

           select true as status, 'Network entity status updated successfully'::character varying as message;

END

$function$
;

-------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_child_clone_list(p_entity_name character varying, p_system_id integer)
RETURNS SETOF json
LANGUAGE plpgsql
AS $function$
DECLARE
    AROW record;
    sql text = '';
BEGIN
    -- Create a temporary table for storing results
    CREATE Temp TABLE temp_associate_entity (
        network_id character varying,
        display_name character varying,
        is_child_entity bool,
        is_associated_entity bool,
        entity_type character varying,
        system_id integer,
        layer_title character varying,
        is_accessories_placed bool
    ) ON COMMIT DROP;
    -- Check if there are relevant mappings for the entity
    IF EXISTS(SELECT 1 FROM VW_LAYER_MAPPING WHERE UPPER(PARENT_LAYER_NAME) = UPPER(p_entity_name) AND UPPER(CHILD_LAYER_NAME) != 'EQUIPMENT') THEN
        -- Loop over child layer mappings and construct SQL dynamically
        FOR AROW IN 
            SELECT child_layer_table, child_layer_name
            FROM vw_layer_mapping
            WHERE upper(parent_layer_name) = upper(p_entity_name) AND upper(child_layer_name) != 'EQUIPMENT'
        LOOP
            IF (sql != '') THEN
                sql := sql || ' UNION ';
            END IF;
            
            sql := sql || 'SELECT network_id, pm.display_name, true AS is_child_entity, false AS is_associated_entity, ''' || AROW.child_layer_name || ''' AS entity_type, ent.system_id, ld.layer_title
                           FROM ' || AROW.child_layer_table || ' AS ent
                           INNER JOIN point_master pm ON ent.system_id = pm.system_id AND upper(pm.entity_type) = upper(''' || AROW.child_layer_name || ''')
                           INNER JOIN layer_details ld ON upper(ld.layer_name) = upper(''' || AROW.child_layer_name || ''')
                           WHERE parent_system_id = ' || p_system_id || ' AND upper(parent_entity_type) = ''' || upper(p_entity_name) || '''';
        END LOOP;
        
        -- Append the rest of the SQL for associated entities
        sql := sql || ' UNION 
                        SELECT associated_network_id AS network_id, associated_display_name AS display_name, false AS is_child_entity, true AS is_associated_entity, associated_entity_type, associated_system_id AS system_id, ld.layer_title
                        FROM associate_entity_info
                        INNER JOIN layer_details ld ON upper(ld.layer_name) = upper(associated_entity_type) AND upper(ld.geom_type) = upper(''Point'')
                        WHERE entity_system_id = ' || p_system_id || ' AND upper(entity_type) = ''' || upper(p_entity_name) || '''';

        sql := sql || ' UNION
                        SELECT entity_network_id AS network_id, entity_display_name AS display_name, false AS is_child_entity, true AS is_associated_entity, entity_type AS entity_type, entity_system_id AS system_id, ld.layer_title
                        FROM associate_entity_info
                        INNER JOIN layer_details ld ON upper(ld.layer_name) = upper(entity_type) AND upper(ld.geom_type) = upper(''Point'')
                        WHERE associated_system_id = ' || p_system_id || ' AND upper(associated_entity_type) = ''' || upper(p_entity_name) || '''';
        
        -- Add the accessories check logic
        sql := 'SELECT A.*, COALESCE(ASS.parent_system_id, 0) > 0 AS is_accessories_placed
                FROM (' || sql || ') A
                LEFT JOIN ATT_DETAILS_ACCESSORIES ASS ON ASS.PARENT_SYSTEM_ID = A.SYSTEM_ID
                AND UPPER(PARENT_ENTITY_TYPE) = UPPER(A.ENTITY_TYPE)
                GROUP BY A.entity_type, A.network_id, A.display_name, A.is_child_entity, A.is_associated_entity, A.system_id, A.layer_title, ASS.parent_system_id';
        
        -- Execute the SQL and insert the results into temp_associate_entity
        EXECUTE 'INSERT INTO temp_associate_entity (network_id, display_name, is_child_entity, is_associated_entity, entity_type, system_id, layer_title, is_accessories_placed) ' || sql;

        -- Remove duplicates based on network_id and entity_type for associated entities
        DELETE FROM temp_associate_entity
        WHERE ctid IN (
            SELECT ctid
            FROM (
                SELECT ctid,is_associated_entity, ROW_NUMBER() OVER (PARTITION BY network_id, entity_type ORDER BY layer_title) AS duplicate_row
                FROM temp_associate_entity
            ) AS subquery
            WHERE duplicate_row > 1 and is_associated_entity = true
        );

        -- Return the result as JSON
        RETURN QUERY
        EXECUTE 'SELECT row_to_json(row) FROM (SELECT * FROM temp_associate_entity ORDER BY layer_title ASC) row';

    END IF;

END;
$function$;
