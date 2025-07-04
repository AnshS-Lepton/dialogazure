
 DROP FUNCTION IF EXISTS public.update_site_project_detail(integer, integer, character varying, character varying, character varying, character varying, character varying, integer, character varying, character varying, character varying, character varying, integer, double precision, double precision, integer, character varying, character varying, double precision, integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.update_site_project_detail(
	p_id integer,
	p_userid integer,
	p_site_name character varying,
	p_project_category character varying,
	p_cable_plan_cores character varying,
	p_comment character varying,
	p_site_owner character varying,
	p_maximum_cost integer,
	p_location_address character varying,
	p_ds_cmc_area character varying,
	p_destination_site_id character varying,
	p_destination_port_type character varying,
	p_no_of_cores integer,
	p_latitude double precision,
	p_longitude double precision,
	p_priority integer,
	p_fiber_link_type character varying,
	p_fiber_link_code character varying,
	p_total_fiber_distance double precision,
	p_plan_cost integer,
	p_site_id character varying,
	p_project_id character varying,
	OUT v_result boolean,
	OUT v_message character varying)
    RETURNS record
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN

update site_project_details set site_name=p_site_name,project_category=p_project_category,
cable_plan_cores=p_cable_plan_cores,comment=p_comment,site_owner=p_site_owner,maximum_cost=p_maximum_cost,
location_address=p_location_address,ds_cmc_area=p_ds_cmc_area,priority=p_priority,
modified_by=p_userId, modified_on=now() where id=p_id;

  --  IF FOUND THEN
	 update att_details_pod set destination_site_id=p_destination_site_id,
	destination_port_type=p_destination_port_type,
	no_of_cores=p_no_of_cores,
	plan_cost=p_plan_cost, 
	latitude=p_latitude,longitude=p_longitude,
	fiber_link_type=p_fiber_link_type,
	fiber_link_code=p_fiber_link_code,
	total_fiber_distance=p_total_fiber_distance,
	project_id=p_id 
	where LOWER(site_id) = LOWER(p_site_id);
	
	IF FOUND THEN
   RAISE NOTICE 'Row updated for site_id=%', p_site_id;
ELSE
   RAISE NOTICE 'No matching row for site_id=%', p_site_id;
END IF;

END;
$BODY$;

ALTER FUNCTION public.update_site_project_detail(integer, integer, character varying, character varying, character varying, character varying, character varying, integer, character varying, character varying, character varying, character varying, integer, double precision, double precision, integer, character varying, character varying, double precision, integer, character varying, character varying)
    OWNER TO postgres;

	---------------------------------------------------------------------------------


	DROP FUNCTION IF EXISTS public.get_site_project_detailsbyid(integer);

CREATE OR REPLACE FUNCTION public.get_site_project_detailsbyid(
	p_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT row_to_json(t)
    FROM (
        SELECT 
		sp.id,sp.project_id, sp.site_id,sp.site_name,sp.site_owner,sp.project_category, sp.cable_plan_cores,sp.maximum_cost,
 sp.comment,sp.location_address,sp.ds_cmc_area,sp.priority,pod.destination_site_id,pod.destination_port_type,pod.no_of_cores
		,pod.plan_cost,pod.latitude,pod.longitude,pod.fiber_link_type,pod.total_fiber_distance,pod.fiber_link_code,pod.fiber_link_type_linkid_prefix
        FROM site_project_details sp left join att_details_pod pod on pod.site_id=sp.site_id
        WHERE sp.id = p_id
    ) t;
END;
$BODY$;

ALTER FUNCTION public.get_site_project_detailsbyid(integer)
    OWNER TO postgres;

	-----------------------------------------------------------------------------


	CREATE OR REPLACE FUNCTION public.fn_update_site_bomboq_amount(
	p_id integer,
	p_userid integer,
	p_amount double precision,
	OUT v_result boolean,
	OUT v_message character varying)
    RETURNS record
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN

update att_details_pod set plan_cost=p_amount where system_id=p_id;

    v_result := TRUE;
    v_message := 'Record updated successfully.';

END;
$BODY$;

----------------------------------------------------------------------