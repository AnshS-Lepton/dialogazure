-- FUNCTION: public.fn_fs_create_routingdata(character varying)

-- DROP FUNCTION IF EXISTS public.fn_fs_create_routingdata(character varying);

CREATE OR REPLACE FUNCTION public.fn_fs_create_routingdata(
	p_desti_table character varying)
    RETURNS TABLE(status boolean, message text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
	execute 'DROP TABLE IF EXISTS ' || p_desti_table;
	execute 'create table ' || p_desti_table || ' as select c.system_id as id, c.system_id as system_id, c.system_id as ufi,c.system_id as sn,c.system_id as route_no, 50 as rc,
	''Road''::character varying as rt,40 as sr,0 as trf,c.province_name as city, c.region_name as state_name,''India''::character varying as country,
	ST_Length(ST_Transform(ST_GeomFromEWKT(''SRID=4326;''||st_astext(lm.sp_geometry)),26986))/1000 as length,0 as cost,0 as reverse_cost,
	ST_Force2D(lm.sp_geometry) as geom from line_master lm join  vw_att_details_cable c on lm.system_id=c.system_id where lm.entity_type=''Cable''';

	RETURN QUERY Select true as status, 'Routing Data created successfully!'::text  as message;
END; 

$BODY$;

ALTER FUNCTION public.fn_fs_create_routingdata(character varying)
    OWNER TO postgres;

------------------------------------------------------------------------------------

-- FUNCTION: public.fn_fs_create_routingdata(character varying, character varying, character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_fs_create_routingdata(character varying, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_fs_create_routingdata(
	p_desti_table character varying,
	p_src_host character varying,
	p_sc_db character varying,
	p_src_user character varying,
	p_src_pwd character varying)
    RETURNS TABLE(status boolean, message text) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
declare query character varying;
BEGIN
	query := 'CREATE TABLE ' || p_desti_table || ' AS select * from dblink(''hostaddr=' || p_src_host || ' dbname=' || p_sc_db || ' user=' || p_src_user || ' password=' || p_src_pwd || ''',
		''select c.system_id as id, c.system_id as system_id, c.system_id as ufi,c.system_id as sn,c.system_id as route_no, 50 as rc, ''''Road''''::character varying as rt,40 as sr,0 as trf,c.province_name as city, 
		c.region_name as state_name,''''India''''::character varying as country, ST_Length(ST_Transform(ST_GeomFromEWKT(''''SRID=4326;''''||st_astext(lm.sp_geometry)),26986))/1000 as length,
		0 as cost,0 as reverse_cost, ST_Force2D(lm.sp_geometry) as geom from line_master lm join vw_att_details_cable c on lm.system_id=c.system_id where lm.entity_type=''''Cable'''''')
		as t1(id integer, system_id integer, ufi integer, sn integer, route_no integer, rc integer, rt character varying, sr integer, trf integer, city character varying, state_name character varying, 
		   country character varying, length double precision, cost integer, reverse_cost integer, geom geometry)';
		   
	execute 'DROP TABLE IF EXISTS ' || p_desti_table;	   
	execute query;

	RETURN QUERY Select true as status, 'Routing Data created successfully!'::text  as message;
END; 

$BODY$;

ALTER FUNCTION public.fn_fs_create_routingdata(character varying, character varying, character varying, character varying, character varying)
    OWNER TO postgres;


