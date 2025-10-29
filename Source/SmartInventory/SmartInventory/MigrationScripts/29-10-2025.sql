
------------Create table to store request logs------------

CREATE TABLE IF NOT EXISTS public.projectwise_report_request_logs
(
    id SERIAL PRIMARY KEY,
    report_type character varying(100) COLLATE pg_catalog."default" NOT NULL,
    file_name character varying(100) COLLATE pg_catalog."default",
    created_by integer NOT NULL,
    created_on timestamp without time zone NOT NULL,
    completed_on timestamp without time zone,
    status character varying(50) COLLATE pg_catalog."default",
    block_code integer,
    filepath text COLLATE pg_catalog."default",
    error_description text COLLATE pg_catalog."default"
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.projectwise_report_request_logs
    OWNER to postgres;

    ------------------- Insert data for module permissions -------------------

    INSERT INTO public.module_master
(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled)
VALUES('Projectwise Fiber Distance', 'Projectwise Fiber Distance', '', '', NULL, now(), 5, now(), 'Web', true, 'PFD', 0, 0, false, NULL, NULL);

INSERT INTO public.module_master
(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled)
VALUES('Projectwise Fiber Distance Report', 'Projectwise Fiber Distance Details', '', '', NULL, now(), 5, now(), 'Web', true, 'PFDR', (select id from module_master m where module_abbr='PFD' and module_name='Projectwise Fiber Distance' ), 1, false);

----------------------------- Function to get projectwise region details -----------------------------

CREATE OR REPLACE FUNCTION public.fn_get_projectwise_regionlist(
	)
    RETURNS TABLE(key text, value text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

BEGIN
   RETURN QUERY
   
   select r.ID::text as key ,r.REGION_NAME::text as value from REGION_BOUNDARY r where r.Id in (select distinct p.region_id from att_details_pod p) and r.ISVISIBLE=TRUE
  	order by r.REGION_NAME asc;
    
END;
$BODY$;

----------------------------- Function to get projectwise province details -----------------------------

CREATE OR REPLACE FUNCTION public.fn_get_projectwise_province_data(
	p_region_id character varying)
    RETURNS TABLE(key text, value text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

BEGIN
	RETURN QUERY
   	
	select p.Id::text as key, p.province_name::text as value from province_boundary p where p.Id in (select distinct province_id from att_details_pod p)
		and p.region_id = p_region_id::int and p.isvisible=true order by p.province_name asc;
END;
$BODY$;

----------------------------- Function to get projectwise district details -----------------------------

CREATE OR REPLACE FUNCTION public.fn_get_projectwise_fiber_distance_report(
	p_region_id integer,
	p_province_id integer)
    RETURNS json
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE
    result JSON;
BEGIN
    SELECT json_agg(t)
    INTO result
    FROM (
        select
		(select project_Id from site_project_details s where s.id=adp.project_Id) as Project_code,
            ''  AS City,
            (select province_name from province_boundary r where id=adp.province_id) as District,
		(select region_name from region_boundary r where id=adp.region_id) as Region,
            adp.category as Catergory,
		adp.site_id,
		adp.site_name, 
            adp.address ,
            adp.total_fiber_distance AS OSP_Distance,
           0 as IBW_Distance,
		0 as Micro_Trench_Distance,
		0 as Distance_without_MT,
		0 as Total_Distance,
            'NA' AS Network_Status,
            '2016' AS Year,
		'July' as Month,
		'2021' as Removed_Year,
		'February' as Removed_Month 
        FROM att_details_pod adp
        WHERE adp.region_id = p_region_id and adp.province_id=p_province_id
    ) t;
	
   	if result is null 
   	then
   	return json_agg(jsonb_build_object('No Data','No Data')); 
   	else
   	RETURN result;
   	end if;
    
END;
$BODY$;
