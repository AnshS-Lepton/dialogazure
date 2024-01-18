-- 25-03-2020
-- Feasibility enable/disable  global setting
INSERT INTO global_settings(key, value, description, type, created_by)
VALUES('isFeasibilityEnabled', '0', 'Enable/Disable Smart Feasibility', 'Web', 1);

-- Feasibility Tables
CREATE TABLE feasibility_cable_type(
	cable_type_id SERIAL PRIMARY KEY,
	cores INT NOT NULL,
	display_name CHARACTER VARYING(10) NOT NULL,
	material_price_per_unit DOUBLE PRECISION NOT NULL DEFAULT 0,
	service_price_per_unit DOUBLE PRECISION NOT NULL DEFAULT 0,
	created_by INT NOT NULL,
	created_on TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
	modified_by INT,
	modified_on TIMESTAMP WITHOUT TIME ZONE
);


--INSERT INTO feasibility_cable_type(cores, display_name, created_by) VALUES
--	(4, '4F', 1), (6, '6F', 1), (12, '12F', 1), (24, '24F', 1), (48, '48F', 1);
	

CREATE TABLE feasibility_demarcation_type(
	type_id SERIAL PRIMARY KEY,
	name CHARACTER VARYING(20) NOT NULL UNIQUE,
	color CHARACTER VARYING(20) NOT NULL
);

--=======================================================================================================
-- 30-03-2020
INSERT INTO feasibility_demarcation_type(name, color) VALUES
	('Inside', 'green'), ('Outside', 'red'), ('LMC', 'blue');

CREATE TABLE feasibility_input(
	feasibility_id SERIAL PRIMARY KEY,
	feasibility_name CHARACTER VARYING NOT NULL,
	customer_id CHARACTER VARYING NOT NULL,
	customer_name CHARACTER VARYING,
	start_lat_lng CHARACTER VARYING(100) NOT NULL,
	end_lat_lng CHARACTER VARYING(100) NOT NULL,
	cores_required INT NOT NULL,
	cable_type_id INT NOT NULL,
	created_by INT NOT NULL,
	created_on TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
	modified_by INT,
	modified_on TIMESTAMP WITHOUT TIME ZONE,
	CONSTRAINT fk_feasInp_cbl FOREIGN KEY(cable_type_id)
		REFERENCES feasibility_cable_type(cable_type_id)
);


CREATE TABLE feasibility_history(
	history_id SERIAL PRIMARY KEY,
	feasibility_id INT NOT NULL,
	created_by INT NOT NULL,
	created_on TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
	modified_by INT,
	modified_on TIMESTAMP WITHOUT TIME ZONE,
	CONSTRAINT fk_feasHis_inp FOREIGN KEY(feasibility_id)
		REFERENCES feasibility_input(feasibility_id)
);

CREATE TABLE feasibility_geometry(
	id SERIAL PRIMARY KEY,
	history_id INT NOT NULL,
	type_id INT NOT NULL,
	cable_geometry GEOMETRY NOT NULL,
	cable_length DOUBLE PRECISION NOT NULL,
	created_by INT NOT NULL,
	created_on TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
	modified_by INT,
	modified_on TIMESTAMP WITHOUT TIME ZONE,
	CONSTRAINT fk_feasGeom_his FOREIGN KEY(history_id)
		REFERENCES feasibility_history(history_id),
	CONSTRAINT fk_feasGeom_dem FOREIGN KEY(type_id)
		REFERENCES feasibility_demarcation_type(type_id)
);

--===========================================================================================
-- 06-04-20
ALTER TABLE feasibility_geometry
ADD COLUMN material_cost DOUBLE PRECISION NOT NULL DEFAULT 0,
ADD COLUMN service_cost DOUBLE PRECISION NOT NULL DEFAULT 0;

alter table feasibility_geometry alter column material_cost drop not null;
alter table feasibility_geometry alter column service_cost drop not null;

CREATE TABLE feasibility_cable_type_history(
	id SERIAL PRIMARY KEY,
	cable_type_id INT NOT NULL,
	cores INT NOT NULL,
	display_name CHARACTER VARYING(10) NOT NULL,
	material_price_per_unit DOUBLE PRECISION NOT NULL,
	service_price_per_unit DOUBLE PRECISION NOT NULL,
	created_by INT NOT NULL,
	created_on TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
	CONSTRAINT fk_CblTyp_his FOREIGN KEY(cable_type_id)
		REFERENCES feasibility_cable_type(cable_type_id)
);

--=============================================================================================
-- 10-04-20
ALTER TABLE att_details_cable_info
ADD COLUMN is_available BOOL NOT NULL DEFAULT true;

--==============================================================================================
-- 13-04-20
CREATE OR REPLACE FUNCTION public.fn_sf_get_available_cores(p_cableid integer)
 RETURNS INTEGER
 LANGUAGE plpgsql
AS $function$
DECLARE
    available_cores INTEGER;
BEGIN
	select COUNT(*) into available_cores from att_details_cable_info
	where cable_id = p_cableid and is_available = true;

	return available_cores;
	
END; $function$;
--==============================================================================================
-- 16-04-20
update feasibility_demarcation_type
set name='inside', color = '#006400' where type_id = 1;

update feasibility_demarcation_type
set name='outside', color = '#FF0000' where type_id = 2;

update feasibility_demarcation_type
set name='inside_P', color = '#95a832' where type_id = 3;

INSERT INTO feasibility_demarcation_type(name, color) VALUES
	('inside_A', '#006400'), ('lmc_start', '#0000FF'), ('lmc_end', '#0000FF');
--===============================================================================================

--------------------====================== Sapna Start =================
-- fn_get_equipment renamed to fn_sf_get_feas_cable_types
CREATE OR REPLACE FUNCTION public.fn_sf_get_feas_cable_types(IN p_searchtext character varying)
  RETURNS TABLE(cable_type_id integer, cores integer, display_name character varying, material_price_per_unit double precision,service_price_per_unit double precision ) AS
$BODY$  
BEGIN
 RETURN QUERY
  select mn.cable_type_id,mn.cores,mn.display_name,mn.material_price_per_unit,mn.service_price_per_unit from feasibility_cable_type mn where mn.display_name like (''||p_searchtext||'%');

END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;


---------------==========================================================
alter table feasibility_cable_type_history add column  
 modified_by integer;
 
alter table feasibility_cable_type_history add column  
  modified_on timestamp without time zone;
  
--alter table feasibility_cable_type add column  
 --modified_by integer;
 
--alter table feasibility_cable_type add column  
  --modified_on timestamp without time zone;

  --alter table feasibility_cable_type add column  
 --created_by integer;
 
--alter table feasibility_cable_type add column  
  --created_on timestamp without time zone;


--------------=================================================
  
-- Function: public.fn_trg_history_feasibility_cable_type()
-- DROP FUNCTION public.fn_trg_history_feasibility_cable_type();

-- fn_trg_history_feasibility_cable_type renamed to fn_sf_trg_feas_cable_type_history

CREATE OR REPLACE FUNCTION public.fn_sf_trg_feas_cable_type_history()
  RETURNS trigger AS
$BODY$
BEGIN
IF (TG_OP = 'INSERT' ) THEN  
     INSERT INTO public.feasibility_cable_type_history(
            cable_type_id, cores, display_name, material_price_per_unit, 
            service_price_per_unit,created_on,created_by)
         select  new.cable_type_id, new.cores, new.display_name, new.material_price_per_unit, 
            new.service_price_per_unit,new.created_on,new.created_by from feasibility_cable_type where cable_type_id=new.cable_type_id;
END IF;
IF (TG_OP = 'UPDATE' ) THEN  
  INSERT INTO public.feasibility_cable_type_history(
            cable_type_id, cores, display_name, material_price_per_unit, 
            service_price_per_unit,created_by, created_on, modified_by, modified_on)
         select  new.cable_type_id, new.cores, new.display_name, new.material_price_per_unit, 
            new.service_price_per_unit,new.created_by, new.created_on, new.modified_by, new.modified_on from feasibility_cable_type where cable_type_id=new.cable_type_id;
END IF; 				
 IF (TG_OP = 'DELETE' ) THEN  
-- 
   INSERT INTO public.feasibility_cable_type_history(
             cable_type_id, cores, display_name, material_price_per_unit, 
             service_price_per_unit,created_by, created_on, modified_by, modified_on)
          values( old.cable_type_id, old.cores, old.display_name, old.material_price_per_unit, 
             old.service_price_per_unit,old.created_by, old.created_on, old.modified_by, old.modified_on);               
 
END IF; 		

RETURN NEW;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.fn_sf_trg_feas_cable_type_history()
  OWNER TO postgres; 
-------------------------======================================================================================
-- -- fn_trg_history_feasibility_cable_type renamed to fn_sf_trg_feas_cable_type_history
CREATE TRIGGER fn_sf_trg_feas_cable_type_history
  AFTER INSERT OR UPDATE OR DELETE
  ON public.feasibility_cable_type
  FOR EACH ROW
  EXECUTE PROCEDURE public.fn_sf_trg_feas_cable_type_history();
  
--===============================================================================================
-- fn_get_feasibility_history_Record renamed to fn_sf_get_feasibility_history_Record
CREATE OR REPLACE FUNCTION public.fn_sf_get_feasibility_history_Record(p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_system_id integer, p_entity_name character varying)
  RETURNS SETOF json AS
$BODY$

 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords integer; 
   WhereCondition character varying;
   V_HISTORY_VIEW_NAME character varying;
V_LAYER_COLUMNS text;
V_LAYER_ID  integer;
BEGIN
WhereCondition:='';

if(p_entity_name = 'FEASIBILITY_CABLE_TYPE')
then
V_HISTORY_VIEW_NAME ='vw_'||p_entity_name||'_history';


RAISE INFO '%', V_HISTORY_VIEW_NAME;
end if;

-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY audit_id desc) AS S_No, audit_id,cores,display_name,material_price_per_unit,service_price_per_unit from '||V_HISTORY_VIEW_NAME||' order by audit_id desc';


-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;

  ------------------===============================================================
  --------------------====================== Sapna End =================
  -- 17-04-20
ALTER TABLE feasibility_cable_type
ADD COLUMN is_deleted BOOL NOT NULL DEFAULT false;

--==============================================================================
CREATE OR REPLACE VIEW public.vw_feasibility_cable_type_history AS 
 SELECT fcth.id AS audit_id,
    fcth.cable_type_id,
    fcth.cores,
    fcth.display_name,
    fcth.material_price_per_unit,
    fcth.service_price_per_unit,
    fn_get_date(fcth.created_on) AS created_on,
    um.user_name AS created_by
   FROM feasibility_cable_type_history fcth
     JOIN user_master um ON fcth.created_by = um.user_id
  ORDER BY fcth.created_on;

ALTER TABLE public.vw_feasibility_cable_type_history
  OWNER TO postgres;

--===============================================================================
-- 27-04-2020
CREATE TABLE feasibility_inside_cable(
	id SERIAL PRIMARY KEY,
	history_id INT NOT NULL,
	system_id INT NOT NULL,
	network_status CHAR(1) NOT NULL,
	available_cores INT NOT NULL,
	cable_length DOUBLE PRECISION NOT NULL,
	cable_geometry GEOMETRY NOT NULL,
	created_by INT NOT NULL,
	created_on TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT now(),
	modified_by INT,
	modified_on TIMESTAMP WITHOUT TIME ZONE,
	CONSTRAINT fk_insd_cbl FOREIGN KEY(history_id)
		REFERENCES feasibility_history(history_id)
);

--===============================================================================
-- 30-04-2020
-- fixed on 26-05-2020
-- Function: public.fn_sf_get_past_feasibilities()

-- DROP FUNCTION public.fn_sf_get_past_feasibilities();

CREATE OR REPLACE FUNCTION public.fn_sf_get_past_feasibilities()
  RETURNS TABLE(feasibility_id integer, history_id integer, feasibility_name character varying, customer_id character varying, customer_name character varying, start_lat_lng character varying, end_lat_lng character varying, cores_required integer, exisiting_length double precision, new_length double precision, created_on character varying, core_level_result character varying, feasibility_result character varying, buffer_radius_a double precision, buffer_radius_b double precision, cable_type_id integer, cable_type character varying) AS
$BODY$
BEGIN
   RETURN QUERY
   SELECT fi.feasibility_id, fh.history_id, fi.feasibility_name, fi.customer_id, fi.customer_name, fi.start_lat_lng, fi.end_lat_lng, fi.cores_required,
	--ROUND(COALESCE(sum(fic.cable_length), 0)::numeric, 2)::double precision as exisiting_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id), 0)::numeric, 2)::double precision as exisiting_length, 
	ROUND(sum(fg.cable_length)::numeric, 2)::double precision as new_length,
	DATE(fh.created_on)::character varying as created_on, coalesce(fh.core_level_result, ' ') as core_level_result, 
	coalesce(fh.feasibility_result, ' ') as feasibility_result, fi.buffer_radius_a, fi.buffer_radius_b,
	fct.cable_type_id, fct.display_name 
	FROM feasibility_input fi join feasibility_history fh on fi.feasibility_id = fh.feasibility_id 
	JOIN feasibility_geometry fg on fh.history_id = fg.history_id
	JOIN feasibility_cable_type fct on fi.cable_type_id = fct.cable_type_id 
	--LEFT JOIN feasibility_inside_cable fic on fh.history_id = fic.history_id 
	GROUP BY fi.feasibility_id, fh.history_id, fct.cable_type_id;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_past_feasibilities()
  OWNER TO postgres;

--------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_sf_get_cable_geoms(hid integer)
  RETURNS TABLE(history_id integer, cable_geometry text, cable_length double precision,
  	cable_type character varying, system_id integer, cable_id character varying, cable_name character varying, 
	total_cores integer, available_cores integer, network_status character varying,
	material_cost double precision, service_cost double precision) AS
$BODY$
BEGIN
   RETURN QUERY
	select fh.history_id, ST_AsText(fg.cable_geometry) as cable_geometry, fg.cable_length, ft."name" as cable_type,
	-1 as system_id, '' as cable_id, '' as cable_name, -1 as total_cores, -1 as available_cores, CAST('' as character varying) as network_status,
	fg.material_cost, fg.service_cost
	from feasibility_geometry fg join feasibility_history fh on fg.history_id = fh.history_id 
	join feasibility_demarcation_type ft on fg.type_id = ft.type_id 
	where fh.history_id = hid
	union
	select fh.history_id, ST_AsText(fic.cable_geometry) as cable_geometry, fic.cable_length, 
	case when CAST(fic.network_status as character varying) = 'P' then 'inside_P'
		 when CAST(fic.network_status as character varying) = 'A' then 'inside_A'
		 else 'inside'
	end as cable_type, 
	fic.system_id, cd.network_id as cable_id, cd.cable_name, fic.total_cores, fic.available_cores, CAST(fic.network_status as character varying),
	-1 as material_cost, -1 as service_cost
	from feasibility_inside_cable fic join feasibility_history fh on fic.history_id = fh.history_id 
	join att_details_cable cd on fic.system_id = cd.system_id 
	where fh.history_id = hid
	order by cable_type;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_cable_geoms(hid integer)
  OWNER TO postgres;


--=============================================================================
-- 07-05-2020
ALTER TABLE feasibility_input
ADD COLUMN buffer_radius_A double precision NOT NULL DEFAULT 0,
ADD COLUMN buffer_radius_B double precision NOT NULL DEFAULT 0;

ALTER TABLE feasibility_history
ADD COLUMN core_level_result character varying,
ADD COLUMN feasibility_result character varying;

ALTER TABLE feasibility_inside_cable
ADD COLUMN total_cores integer not null default 0;

--==============================================================================
-- 08-05-2020 -- Nitya Function fix
-- Function: public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer, integer)

-- DROP FUNCTION public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public.sf_fn_insert_feasibility_geometry(in_history_id integer, latlng character varying, cable_type character varying, in_cable_length double precision, in_created_by integer, in_system_id integer, in_network_status character varying, in_avaliable_cores integer, in_total_cores integer)
  RETURNS SETOF integer AS
$BODY$
DECLARE
	in_Cable_GEOM 		GEOMETRY;
	in_type_id 		INTEGER;
	in_material_cost 	DOUBLE PRECISION;
	in_service_cost		DOUBLE PRECISION;
BEGIN
-- select * from feasibility_geometry;
-- select * from feasibility_demarcation_type;

select st_geomfromtext('LINESTRING(' || latlng ||')') into in_Cable_GEOM;
select type_id into in_type_id from feasibility_demarcation_type where name=cable_type;
if(UPPER(cable_type) = 'OUTSIDE' OR UPPER(cable_type) = 'LMC_START' OR UPPER(cable_type) = 'LMC_END') then
select (material_price_per_unit * in_cable_length), (service_price_per_unit * in_cable_length) into in_material_cost, in_service_cost from feasibility_cable_type
where cable_type_id = (select cable_type_id from feasibility_input where feasibility_id = (select feasibility_id from feasibility_history where history_id = in_history_id));	-- outside and lmc1 and lmc2
--select (service_price_per_unit * in_cable_length) into in_service_cost from feasibility_cable_type;
else
in_material_cost :=0;
in_service_cost :=0;
END IF;
if(UPPER(cable_type) = 'INSIDE' OR UPPER(cable_type) = 'INSIDE_A' OR UPPER(cable_type) = 'INSIDE_P') then
	-- select * from feasibility_inside_cable
	INSERT INTO public.feasibility_inside_cable(history_id, system_id, network_status, available_cores, total_cores, cable_length, cable_geometry, created_by, created_on)
	VALUES
	(in_history_id, in_system_id, in_network_status, in_avaliable_cores, in_total_cores, in_cable_length, in_Cable_GEOM, in_created_by, now());
ELSE
	INSERT INTO public.feasibility_geometry( history_id, type_id, cable_geometry, cable_length, created_by, created_on, material_cost, service_cost)
	VALUES
	 (in_history_id,in_type_id,in_Cable_GEOM,in_cable_length, in_created_by, now(), in_material_cost, in_service_cost);
END IF;
 RETURN QUERY 
	SELECT geom.id from feasibility_geometry geom where cable_geometry = in_Cable_GEOM and cable_length = in_cable_length;
END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer, integer)
  OWNER TO postgres;

--=========================================================================================================================================
-- Sapna - 07/05/20

  ------------------==============5_5_2020 =============================
alter table feasibility_cable_type_history add column  
  action character varying(10)


  
  -------------------=============================================================
  
  
                
CREATE OR REPLACE FUNCTION public.fn_sf_get_feasibility_history_record(p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_system_id integer, p_entity_name character varying)
  RETURNS SETOF json AS
$BODY$

 DECLARE
   sql TEXT;
   StartSNo    INTEGER;    
   EndSNo      INTEGER;
   TotalRecords integer; 
   WhereCondition character varying;
   V_HISTORY_VIEW_NAME character varying;
V_LAYER_COLUMNS text;
V_LAYER_ID  integer;
BEGIN
WhereCondition:='';

if(p_entity_name = 'FEASIBILITY_CABLE_TYPE')
then
V_HISTORY_VIEW_NAME ='vw_'||p_entity_name||'_history';


RAISE INFO '%', V_HISTORY_VIEW_NAME;
end if;

-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY audit_id desc) AS S_No, * from '||V_HISTORY_VIEW_NAME||' order by cores,audit_id desc';


-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_feasibility_history_record(integer, integer, character varying, character varying, integer, character varying)
  OWNER TO postgres;

  

----------------------------------=========================================================
alter table feasibility_cable_type_history add column
  is_deleted boolean NOT NULL DEFAULT false;
  

  -----------------=================================================================
  
CREATE OR REPLACE FUNCTION public.fn_get_sf_cabletype_details(p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying)
  RETURNS SETOF json AS
$BODY$

 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 

BEGIN

LowerStart:='';
LowerEnd:='';

 IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
    LowerStart:='LOWER(';
    LowerEnd:=')';
  END IF;


RAISE INFO '%', sql;


-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY cores) AS S_No,cable_type_id,cores,upper(display_name)::character varying as display_name,material_price_per_unit,service_price_per_unit,created_on,modified_on FROM feasibility_cable_type WHERE  is_deleted=false';
IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||p_searchtext||'%'')';
END IF;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' order by created_on desc) T';                  
 END IF; 


RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_get_sf_cabletype_details(character varying, character varying, integer, integer, character varying, character varying)
  OWNER TO postgres;


-------------==========================================================================


CREATE OR REPLACE FUNCTION public.fn_sf_trg_feas_cable_type_history()
  RETURNS trigger AS
$BODY$
BEGIN
IF (TG_OP = 'INSERT' ) THEN  
     INSERT INTO public.feasibility_cable_type_history(
            cable_type_id, cores, display_name, material_price_per_unit, 
            service_price_per_unit,created_on,created_by,is_deleted,action)
         select  new.cable_type_id, new.cores, new.display_name, new.material_price_per_unit, 
            new.service_price_per_unit,new.created_on,new.created_by,new.is_deleted,'I' as action from feasibility_cable_type where cable_type_id=new.cable_type_id;
END IF;
IF (TG_OP = 'UPDATE' ) THEN  
  INSERT INTO public.feasibility_cable_type_history(
            cable_type_id, cores, display_name, material_price_per_unit, 
            service_price_per_unit,created_by, created_on, modified_by, modified_on,action)
         select  old.cable_type_id, old.cores, old.display_name,old.material_price_per_unit, 
            old.service_price_per_unit,old.created_by, old.created_on, new.modified_by, new.modified_on,case when new.is_deleted =true then 'D' else 'U' end as action from feasibility_cable_type where cable_type_id=new.cable_type_id;
END IF; 				
 IF (TG_OP = 'DELETE' ) THEN  
-- 
   INSERT INTO public.feasibility_cable_type_history(
             cable_type_id, cores, display_name, material_price_per_unit, 
             service_price_per_unit,created_by, created_on, modified_by, modified_on,is_deleted,action)
          values( old.cable_type_id, old.cores, old.display_name, old.material_price_per_unit, 
             old.service_price_per_unit,old.created_by, old.created_on, old.modified_by, old.modified_on,old.is_deleted,'D');               
 
END IF; 		

RETURN NEW;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.fn_sf_trg_feas_cable_type_history()
  OWNER TO postgres;


----------------========================================================================
CREATE OR REPLACE FUNCTION public.fn_sf_get_feas_cable_types(IN p_searchtext character varying)
  RETURNS TABLE(cable_type_id integer, cores integer, display_name character varying, material_price_per_unit double precision, service_price_per_unit double precision) AS
$BODY$  
BEGIN
 RETURN QUERY
  select mn.cable_type_id,mn.cores,upper(mn.display_name)::character varying,mn.material_price_per_unit,mn.service_price_per_unit from feasibility_cable_type mn where mn.is_deleted=false and  mn.display_name like (''||p_searchtext||'%');

END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_feas_cable_types(character varying)
  OWNER TO postgres;

-----------------======================================================================


DROP VIEW public.vw_feasibility_cable_type_history;

CREATE OR REPLACE VIEW public.vw_feasibility_cable_type_history AS 
 SELECT fcth.id AS audit_id,
    fcth.cable_type_id,
    fcth.cores,
    upper(fcth.display_name)as display_name,
    fcth.material_price_per_unit,
    fcth.service_price_per_unit,
    um.user_name AS created_by,
    fn_get_date(fcth.created_on) AS created_on,
    mum.user_name AS modified_by,
    fn_get_date(fcth.modified_on) AS modified_on,
    fn_get_action(fcth.action) AS action
   FROM feasibility_cable_type_history fcth
     JOIN user_master um ON fcth.created_by = um.user_id
     LEFT JOIN user_master mum ON fcth.modified_by = mum.user_id
  ORDER BY fcth.created_on;

ALTER TABLE public.vw_feasibility_cable_type_history
  OWNER TO postgres;


-----------------=================================================================================
-- Sapna - 26/0/20

CREATE OR REPLACE FUNCTION public.fn_sf_get_feasibility_history_record( p_searchtext character varying,p_searchby character varying ,p_pageno integer, p_pagerecord integer,p_sortcolname character varying, p_sorttype character varying, p_system_id integer, p_entity_name character varying)
  RETURNS SETOF json AS
$BODY$

 DECLARE
   sql TEXT;
     StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
   V_HISTORY_VIEW_NAME character varying;
V_LAYER_COLUMNS text;
V_LAYER_ID  integer;
BEGIN


LowerStart:='';
LowerEnd:='';




  
WhereCondition:='';

if(p_entity_name = 'FEASIBILITY_CABLE_TYPE')
then
V_HISTORY_VIEW_NAME ='vw_'||p_entity_name||'_history';


RAISE INFO '%', V_HISTORY_VIEW_NAME;
end if;


-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY audit_id desc) AS S_No, * from '||V_HISTORY_VIEW_NAME||' where action!=''Inserted'' ';

IF ((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) ='cores') THEN
p_searchtext:=p_searchtext::integer;
sql:= sql ||' AND '||p_searchby||' = '||p_searchtext||'';
else if((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) !='cores')then
sql:= sql ||' AND lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
end if;
END IF;

sql:= sql ||'order by cores,audit_id desc ';





RAISE INFO '%sql', sql;


-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_feasibility_history_record(integer, integer, character varying, character varying, integer, character varying)
  OWNER TO postgres;


------------=====================================================================



CREATE OR REPLACE VIEW public.vw_feasibility_cable_type AS 
 SELECT
    fcth.cable_type_id,
    fcth.cores,
    upper(fcth.display_name::text) AS display_name,
    fcth.material_price_per_unit,
    fcth.service_price_per_unit,
    um.user_name AS created_user,
    fn_get_date(fcth.created_on) AS created_on,
    mum.user_name AS modified_user,
    fn_get_date(fcth.modified_on) AS modified_on,
    fcth.is_deleted
   FROM feasibility_cable_type fcth
     JOIN user_master um ON fcth.created_by = um.user_id
     LEFT JOIN user_master mum ON fcth.modified_by = mum.user_id
  ORDER BY fcth.created_on;





-----------------------===========================================================================



CREATE OR REPLACE FUNCTION public.fn_get_sf_cabletype_details(p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying)
  RETURNS SETOF json AS
$BODY$

 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 

BEGIN

LowerStart:='';
LowerEnd:='';

 IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
    LowerStart:='LOWER(';
    LowerEnd:=')';
  END IF;


RAISE INFO '%', sql;


-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY cores) AS S_No,* from vw_feasibility_cable_type WHERE  is_deleted=false';
IF ((p_searchtext) != '' and (p_searchby) != '') THEN
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||p_searchtext||'%'')';
END IF;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' order by created_on desc) T';                  
 END IF; 


RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_get_sf_cabletype_details(character varying, character varying, integer, integer, character varying, character varying)
  OWNER TO postgres;



---------------------================================================================================
-- Sapna - 27/05/20
CREATE OR REPLACE FUNCTION public.fn_get_sf_cabletype_details(p_searchby character varying, p_searchtext character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying)
  RETURNS SETOF json AS
$BODY$

DECLARE
   sql TEXT;
   StartSNo    INTEGER;  
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer;
BEGIN
LowerStart:='';
LowerEnd:='';

IF (coalesce(P_SORTCOLNAME,'')!='') THEN 
    LowerStart:='LOWER(';
    LowerEnd:=')';
  END IF;

RAISE INFO '%', sql;

-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY cores) AS S_No,* from vw_feasibility_cable_type WHERE  is_deleted=false';
IF ((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) ='cores') THEN
p_searchtext:=p_searchtext::integer;
sql:= sql ||' AND '||p_searchby||' = '||p_searchtext||'';
else if((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) !='cores')then
sql:= sql ||' AND lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
end if;
END IF;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
IF((P_PAGENO) <> 0) THEN
                StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
                EndSNo:= P_PAGENO * P_PAGERECORD;
                sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo;
ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' order by created_on desc) T';                 
 END IF;
RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';


END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_get_sf_cabletype_details(character varying, character varying, integer, integer, character varying, character varying)
  OWNER TO postgres;
--================================================================================================
-- 10/06
CREATE OR REPLACE FUNCTION public.fn_sf_get_routes(IN p_source character varying, IN p_destination character varying, IN p_start_buffer integer, IN p_end_buffer integer)
  RETURNS TABLE(seq integer, path_seq integer, edge_targetid integer, roadline_geomtext text) AS
$BODY$
DECLARE 
   v_start_vid integer;
   v_end_vid integer;
   v_start_vid_frwd integer;
   v_end_vid_frwd integer;
   v_start_vid_bkwd integer;
   v_end_vid_bkwd integer;
   v_length_frwd double precision;
   v_length_bkwd double precision;
BEGIN
	--select start_vid, end_vid INTO v_start_vid, v_end_vid
	--FROM pgr_dijkstra(
	--'SELECT id, source, target, cost, reverse_cost FROM new_poc_data',
	--(SELECT array_agg(source) FROM new_poc_data 
	--	WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), geom, p_start_buffer, true)),
	--(SELECT array_agg(target) FROM new_poc_data 
	--	WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), geom, p_end_buffer, true)),
	--directed:= True) as pt
	--JOIN new_poc_data_noded nd ON pt.edge = nd.old_id group by start_vid, end_vid 
	--order by (SUM(ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(geom), true) +
	--	ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(geom), true))) limit 1;

	select T.start_vid, T.end_vid, least((ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(single_geom), true) +
		ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(single_geom), true)),
		(ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(st_reverse(single_geom)), true) +
		ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(st_reverse(single_geom)), true))) as outside_length INTO v_start_vid_frwd, v_end_vid_frwd, v_length_frwd
	FROM(
	select start_vid, end_vid, ST_LineMerge(st_union(geom)) as single_geom
	FROM pgr_dijkstra(
	'SELECT id, source, target, cost, reverse_cost FROM new_poc_data',
	(SELECT array_agg(source) FROM new_poc_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), geom, p_start_buffer, true)),
	(SELECT array_agg(target) FROM new_poc_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), geom, p_end_buffer, true)),
	directed:= True) as pt
	JOIN new_poc_data_noded nd ON pt.edge = nd.old_id 
	group by start_vid, end_vid) T order by outside_length limit 1;

	-- if direction of cable is backwrds, it may leave boundary nodes, then we need to check lengths in reverse direction
	select T.start_vid, T.end_vid, least((ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(single_geom), true) +
		ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(single_geom), true)),
		(ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(st_reverse(single_geom)), true) +
		ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(st_reverse(single_geom)), true))) as outside_length INTO v_start_vid_bkwd, v_end_vid_bkwd, v_length_bkwd
	FROM(
	select start_vid, end_vid, ST_LineMerge(st_union(geom)) as single_geom
	FROM pgr_dijkstra(
	'SELECT id, source, target, cost, reverse_cost FROM new_poc_data',
	(SELECT array_agg(target) FROM new_poc_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), geom, p_start_buffer, true)),
	(SELECT array_agg(source) FROM new_poc_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), geom, p_end_buffer, true)),
	directed:= True) as pt
	JOIN new_poc_data_noded nd ON pt.edge = nd.old_id 
	group by start_vid, end_vid) T order by outside_length limit 1;

	RAISE NOTICE 'forward = % and backward = %', v_length_frwd,v_length_bkwd;
	
	if v_length_frwd <= v_length_bkwd then
		v_start_vid:= v_start_vid_frwd;
		v_end_vid:= v_end_vid_frwd;
	else
		v_start_vid:= v_end_vid_bkwd;
		v_end_vid:= v_start_vid_bkwd;
	end if;

	RAISE NOTICE 'start_vid = % and end_vid = %', v_start_vid, v_end_vid;
		

	RETURN QUERY	
	select pt.seq, 
	-- in case of considering reverse path, we will also have to reverse the path_seq.
	case when v_length_frwd <= v_length_bkwd then pt.path_seq::integer
		else ((count(*) OVER()) - pt.path_seq + 1)::integer
	end as path_seq, pt.edge::integer as edge_TargetID, st_astext(nd.geom) as roadLine_GeomText 
	FROM pgr_dijkstra(
	'SELECT id, source, target, cost, reverse_cost FROM new_poc_data', v_start_vid, v_end_vid,
	directed:= True) as pt
	JOIN new_poc_data_noded nd ON pt.edge = nd.old_id order by path_seq;

END; 
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_routes(character varying, character varying, integer, integer)
  OWNER TO postgres;

--====================================================================================================
-- 10/06
alter table feasibility_input
add start_point_name character varying; 
alter table feasibility_input
add end_point_name character varying;

--=====================================================================================================
-- Resource Scripts
-- Table: public.res_resources

-- DROP TABLE public.res_resources;

CREATE SEQUENCE public.resources_id_seq
  INCREMENT 1
  MINVALUE 1
  MAXVALUE 9223372036854775807
  START 358
  CACHE 1;
ALTER TABLE public.resources_id_seq
  OWNER TO postgres;
  
CREATE TABLE public.res_resources
(
  id integer NOT NULL DEFAULT nextval('resources_id_seq'::regclass),
  culture character varying(50) NOT NULL,
  key character varying(50) NOT NULL,
  value character varying(350) NOT NULL,
  is_default_lang boolean NOT NULL,
  is_visible boolean,
  language character varying(50),
  description character varying(500),
  modified_by integer,
  modified_on timestamp without time zone,
  created_on timestamp without time zone DEFAULT now(),
  created_by integer,
  CONSTRAINT resources_pkey PRIMARY KEY (culture, key)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.res_resources
  OWNER TO postgres;

  
  
--Insert into dropdown_master

INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Language_List', 'en', true, 1, 'English');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Language_List', 'hi', true, 1, 'Hindi');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Language_List', 'jr', true, 1, 'Jarmani');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Language_List', 'jp', true, 1, 'Japan');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Language_List', 'fr', true, 1, 'France');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Project', 'Smart Inventory', true, 1, 'SI');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Project', 'Smart Feasibility', true, 1, 'SF');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Project', 'Smart Planner', true, 1, 'SP');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Project', 'Global', true, 1, 'GBL');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Module', 'Admin', true, 1, 'ADM');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Module', 'Osp', true, 1, 'OSP');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Module', 'Isp', true, 1, 'ISP');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Module', 'Mdu', true, 1, 'MDU');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Module', 'Global', true, 1, 'GBL');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Sub_Module', 'Manhole', true, 1, 'MH');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Sub_Module', 'Splice Closure', true, 1, 'SC');
    
    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Sub_Module', 'Splitter', true, 1, 'SPL');
    
    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Sub_Module', 'Global', true, 1, 'GBL');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_DotNet_JQuery', 'Dot Net"', true, 1, 'NET');
    
    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_DotNet_JQuery', '"J Query', true, 1, 'JQ');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_DotNet_JQuery', 'Global', true, 1, 'GBL');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Purpose_Type', 'Form', true, 1, 'FRM');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Purpose_Type', 'Report', true, 1, 'RPT');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Purpose_Type', 'History', true, 1, 'HIS');

    INSERT INTO public.dropdown_master( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, dropdown_key)
    VALUES ( 0, 'Res_Purpose_Type', 'Global', true, 1, 'GNL');



  




-- Function: public.fn_res_get_resource_culture_dropdown()

-- DROP FUNCTION public.fn_res_get_resource_culture_dropdown();

CREATE OR REPLACE FUNCTION public.fn_res_get_resource_culture_dropdown()
  RETURNS TABLE(dropdown_type character varying, dropdown_value character varying, dropdown_key character varying) AS
$BODY$

 BEGIN
return query
select dm.dropdown_type,dm.dropdown_value,dm.dropdown_key from dropdown_master dm where dm.dropdown_type='Res_Language_List' and dm.dropdown_value  in(select distinct culture from res_resources);
   
END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_res_get_resource_culture_dropdown()
  OWNER TO postgres;
  
-- Function: public.fn_res_get_resource_dropdown()

-- DROP FUNCTION public.fn_res_get_resource_dropdown();

CREATE OR REPLACE FUNCTION public.fn_res_get_resource_dropdown()
  RETURNS TABLE(dropdown_type character varying, dropdown_value character varying, dropdown_key character varying) AS
$BODY$

 BEGIN
return query
select dm.dropdown_type,dm.dropdown_value,dm.dropdown_key from dropdown_master dm where dm.dropdown_type='Res_Language_List' and dm.dropdown_value not in(select distinct culture from res_resources);
   
END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_res_get_resource_dropdown()
  OWNER TO postgres;

  
  -- Function: public.fn_res_get_resource_list(character varying, character varying, character varying)

-- DROP FUNCTION public.fn_res_get_resource_list(character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_res_get_resource_list(IN searchtext character varying, IN searchvalue character varying, IN selectlang character varying)
  RETURNS TABLE(culture character varying, key character varying, value character varying, currentvalue character varying, language character varying) AS
$BODY$

 BEGIN


    IF(Searchtext = 'Name') THEN

        return query
		select rs.Culture::character varying,r.key::character varying,r.value::character varying ,rs.Value,rs.language
		from res_resources r join res_resources rs ON r.key   = rs.key and rs.Culture=SelectLang
		where r.Culture='en' AND r.key Like SearchValue;
     ELSEIF(Searchtext='Value') Then 
        return query
		select rs.Culture::character varying,r.key::character varying,r.value::character varying ,rs.Value,rs.language
		from res_resources r join res_resources rs ON r.key   = rs.key and rs.Culture=SelectLang
		where r.Culture='en' AND r.value Like SearchValue;

           ELSE
           
            return query
           	select rs.Culture::character varying,r.key::character varying,r.value::character varying ,rs.Value,rs.language
		from res_resources r join res_resources rs ON r.key   = rs.key and rs.Culture=SelectLang
		where r.Culture='en';	
 END IF;
END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_res_get_resource_list(character varying, character varying, character varying)
  OWNER TO postgres;
-- Function: public.fn_res_insert_new_resource(character varying)

-- DROP FUNCTION public.fn_res_insert_new_resource(character varying);

CREATE OR REPLACE FUNCTION public.fn_res_insert_new_resource(newculture character varying)
  RETURNS void AS
$BODY$

 BEGIN
IF(newculture!='' )then
BEGIN

insert into res_resources (id,culture,key,value,is_default_lang) 
select nextval('resources_id_seq'),newculture,rn.key, '',false from res_resources rn where rn.culture='en';
  END;
  END IF; 
  

END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.fn_res_insert_new_resource(character varying)
  OWNER TO postgres;
-- Function: public.fn_res_insert_resource_key(character varying, character varying)

-- DROP FUNCTION public.fn_res_insert_resource_key(character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_res_insert_resource_key(newkey character varying, newdescription character varying)
  RETURNS void AS
$BODY$
declare rec   record;
DECLARE res_language character varying;
	
BEGIN
		if not exists(select 1 from res_resources where culture='en') then
BEGIN
		select dm.dropdown_value into res_language from dropdown_master dm where dm.dropdown_key='en';
	
		insert into res_resources (id,culture,key,value,is_default_lang,language,description)
		VALUES( nextval('resources_id_seq'),'en',newkey,'',true,res_language,newdescription);	
END;
ELSE
BEGIN
		FOR rec IN select distinct culture from res_resources order by culture
LOOP
		select dm.dropdown_value into res_language from dropdown_master dm where dm.dropdown_key=rec.culture;
	
		insert into res_resources (id,culture,key,value,is_default_lang,language,description)
		VALUES( nextval('resources_id_seq'),rec.culture,newkey,'',CASE WHEN rec.culture='en' THEN true ELSE false END,res_language,newdescription);		
END LOOP;
END;
END IF;
END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.fn_res_insert_resource_key(character varying, character varying)
  OWNER TO postgres;
-- Function: public.fn_sf_get_feasibility_history_record(character varying, character varying, integer, integer, character varying, character varying, integer, character varying)

-- DROP FUNCTION public.fn_sf_get_feasibility_history_record(character varying, character varying, integer, integer, character varying, character varying, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_sf_get_feasibility_history_record(p_searchtext character varying, p_searchby character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_system_id integer, p_entity_name character varying)
  RETURNS SETOF json AS
$BODY$

 DECLARE
   sql TEXT;
     StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
   V_HISTORY_VIEW_NAME character varying;
V_LAYER_COLUMNS text;
V_LAYER_ID  integer;
BEGIN


LowerStart:='';
LowerEnd:='';




  
WhereCondition:='';

if(p_entity_name = 'FEASIBILITY_CABLE_TYPE')
then
V_HISTORY_VIEW_NAME ='vw_'||p_entity_name||'_history';


RAISE INFO '%', V_HISTORY_VIEW_NAME;
end if;


-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY audit_id desc) AS S_No,cores as SF_GBL_GBL_JQ_FRM_002,display_name as SF_GBL_GBL_NET_FRM_027,
                Material_Price_Per_Unit as SF_GBL_GBL_JQ_FRM_003,Service_Price_Per_Unit as SF_GBL_GBL_JQ_FRM_004,created_by as SF_GBL_GBL_JQ_FRM_005,created_on as SF_GBL_GBL_JQ_FRM_009,
                modified_by as SF_GBL_GBL_JQ_FRM_006,modified_on as SF_GBL_GBL_JQ_FRM_007,action as SF_GBL_GBL_JQ_FRM_008 from ' ||V_HISTORY_VIEW_NAME||' where action!=''Inserted'' ';

IF ((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) ='cores') THEN
p_searchtext:=p_searchtext::integer;
sql:= sql ||' AND '||p_searchby||' = '||p_searchtext||'';
else if((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) !='cores')then
sql:= sql ||' AND lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
end if;
END IF;

sql:= sql ||'order by cores,audit_id desc ';





RAISE INFO '%sql', sql;


-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_feasibility_history_record(character varying, character varying, integer, integer, character varying, character varying, integer, character varying)
  OWNER TO postgres;


-----------------===================================================================================================================






CREATE OR REPLACE FUNCTION public.fn_res_insert_resource_key(p_newkey character varying, p_newdescription character varying,p_value character varying)
  RETURNS void AS
$BODY$
DECLARE
 rec   record;
 res_language character varying;
 v_newKey character varying;
 v_keyCount integer;
 v_keyCountvalue integer;



	
BEGIN


select count(DISTINCT key)  as key into v_keyCount from res_resources where left(key,CHAR_LENGTH(p_newkey))=p_newkey;
v_keyCount:=v_keyCount+1;

	RAISE INFO 'v_keyCount: %',v_keyCount;

select CHAR_LENGTH(v_keyCount ::character varying) into v_keyCountvalue;

RAISE INFO 'v_keyCountvalue: %',v_keyCountvalue;


	if(v_keyCountvalue = 0)then
	RAISE INFO '1: %','1';
	v_newKey :=p_newkey||'_000'||v_keyCount;
	else if(v_keyCountvalue = 1)then
	RAISE INFO '1: %','1';
	v_newKey :=p_newkey||'_00'||v_keyCount;
	else if(v_keyCountvalue = 2)then
	RAISE INFO '2: %','2';
	v_newKey :=p_newkey||'_0'||v_keyCount;
	else
	RAISE INFO '3: %','3';
	v_newKey :=p_newkey||'_'||v_keyCount;
	end if;
	end if;
	end if;
	


	RAISE INFO 'v_newKey: %',v_newKey;

		if not exists(select 1 from res_resources where culture='en') then
BEGIN
		select dm.dropdown_key into res_language from dropdown_master dm where dm.dropdown_value='en';
	
		insert into res_resources (id,culture,key,value,is_default_lang,language,description)
		VALUES( nextval('resources_id_seq'),'en',v_newKey,p_value,true,res_language,p_newdescription);	
END;
ELSE
BEGIN
		FOR rec IN select distinct culture from res_resources order by culture
LOOP
		select dm.dropdown_key into res_language from dropdown_master dm where dm.dropdown_value=rec.culture;

		 insert into res_resources (id,culture,key,value,is_default_lang,language,description)
		 VALUES( nextval('resources_id_seq'),rec.culture,v_newKey,case WHEN rec.culture='en' THEN p_value ELSE '' END ,
		 CASE WHEN rec.culture='en' THEN true ELSE false END,res_language,p_newdescription);		
END LOOP;
END;
END IF;
END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.fn_res_insert_resource_key(character varying, character varying)
  OWNER TO postgres;

-----------------------------------------=================================================================================================
-- FUNCTION: public.fn_sf_get_past_feasibilities()

-- DROP FUNCTION public.fn_sf_get_past_feasibilities();

CREATE OR REPLACE FUNCTION public.fn_sf_get_past_feasibilities(
	)
RETURNS TABLE(feasibility_id integer, history_id integer, feasibility_name character varying, customer_id character varying, customer_name character varying, start_point_name character varying, start_lat_lng character varying, end_point_name character varying, end_lat_lng character varying, cores_required integer, exisiting_length double precision, new_length double precision, created_on character varying, core_level_result character varying, feasibility_result character varying, buffer_radius_a double precision, buffer_radius_b double precision, cable_type_id integer, cable_type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE 
    ROWS 1000
AS $BODY$
BEGIN
   RETURN QUERY
   SELECT fi.feasibility_id, fh.history_id, fi.feasibility_name, fi.customer_id, fi.customer_name, fi.start_point_name, fi.start_lat_lng, fi.end_point_name, fi.end_lat_lng, fi.cores_required,
	--ROUND(COALESCE(sum(fic.cable_length), 0)::numeric, 2)::double precision as exisiting_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id), 0)::numeric, 2)::double precision as exisiting_length, 
	ROUND(sum(fg.cable_length)::numeric, 2)::double precision as new_length,
	DATE(fh.created_on)::character varying as created_on, coalesce(fh.core_level_result, ' ') as core_level_result, 
	coalesce(fh.feasibility_result, ' ') as feasibility_result, fi.buffer_radius_a, fi.buffer_radius_b,
	fct.cable_type_id, fct.display_name 
	FROM feasibility_input fi join feasibility_history fh on fi.feasibility_id = fh.feasibility_id 
	JOIN feasibility_geometry fg on fh.history_id = fg.history_id
	JOIN feasibility_cable_type fct on fi.cable_type_id = fct.cable_type_id 
	--LEFT JOIN feasibility_inside_cable fic on fh.history_id = fic.history_id 
	GROUP BY fi.feasibility_id, fh.history_id, fct.cable_type_id;
END;
$BODY$;

ALTER FUNCTION public.fn_sf_get_past_feasibilities()
    OWNER TO postgres;

--==========================================================================================================================================
-- 16/06/20
insert into feasibility_demarcation_type(name, color)
values('outside_start', '#FF0000');
insert into feasibility_demarcation_type(name, color)
values('outside_end', '#FF0000');

-- Function: public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer)

-- DROP FUNCTION public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer);

CREATE OR REPLACE FUNCTION public.sf_fn_insert_feasibility_geometry(in_history_id integer, latlng character varying, cable_type character varying, in_cable_length double precision, in_created_by integer, in_system_id integer, in_network_status character varying, in_avaliable_cores integer)
  RETURNS SETOF integer AS
$BODY$
DECLARE
	in_Cable_GEOM 		GEOMETRY;
	in_type_id 		INTEGER;
	in_material_cost 	DOUBLE PRECISION;
	in_service_cost		DOUBLE PRECISION;
BEGIN
-- select * from feasibility_geometry;
-- select * from feasibility_demarcation_type;

select st_geomfromtext('LINESTRING(' || latlng ||')') into in_Cable_GEOM;
select type_id into in_type_id from feasibility_demarcation_type where name=cable_type;
if(UPPER(cable_type) = 'OUTSIDE' OR UPPER(cable_type) = 'OUTSIDE_START' OR UPPER(cable_type) = 'OUTSIDE_END' OR UPPER(cable_type) = 'LMC_START' OR UPPER(cable_type) = 'LMC_END') then
select (material_price_per_unit * in_cable_length), (service_price_per_unit * in_cable_length) into in_material_cost, in_service_cost from feasibility_cable_type
where cable_type_id = (select cable_type_id from feasibility_input where feasibility_id = (select feasibility_id from feasibility_history where history_id = in_history_id));	-- outside and lmc1 and lmc2
--select (service_price_per_unit * in_cable_length) into in_service_cost from feasibility_cable_type;
else
in_material_cost :=0;
in_service_cost :=0;
END IF;
if(UPPER(cable_type) = 'INSIDE' OR UPPER(cable_type) = 'INSIDE_A' OR UPPER(cable_type) = 'INSIDE_P') then
	-- select * from feasibility_inside_cable
	INSERT INTO public.feasibility_inside_cable(history_id, system_id, network_status, available_cores, cable_length, cable_geometry, created_by, created_on)
	VALUES
	(in_history_id, in_system_id, in_network_status, in_avaliable_cores, in_cable_length, in_Cable_GEOM, in_created_by, now());
ELSE
	INSERT INTO public.feasibility_geometry( history_id, type_id, cable_geometry, cable_length, created_by, created_on, material_cost, service_cost)
	VALUES
	 (in_history_id,in_type_id,in_Cable_GEOM,in_cable_length, in_created_by, now(), in_material_cost, in_service_cost);
END IF;
 RETURN QUERY 
	SELECT geom.id from feasibility_geometry geom where cable_geometry = in_Cable_GEOM and cable_length = in_cable_length;
END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer)
  OWNER TO postgres;

-- Function: public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer, integer)

-- DROP FUNCTION public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer, integer);

--======================================================================================================================================================================
CREATE OR REPLACE FUNCTION public.sf_fn_insert_feasibility_geometry(in_history_id integer, latlng character varying, cable_type character varying, in_cable_length double precision, in_created_by integer, in_system_id integer, in_network_status character varying, in_avaliable_cores integer, in_total_cores integer)
  RETURNS SETOF integer AS
$BODY$
DECLARE
	in_Cable_GEOM 		GEOMETRY;
	in_type_id 		INTEGER;
	in_material_cost 	DOUBLE PRECISION;
	in_service_cost		DOUBLE PRECISION;
BEGIN
-- select * from feasibility_geometry;
-- select * from feasibility_demarcation_type;

select st_geomfromtext('LINESTRING(' || latlng ||')') into in_Cable_GEOM;
select type_id into in_type_id from feasibility_demarcation_type where name=cable_type;
if(UPPER(cable_type) = 'OUTSIDE' OR UPPER(cable_type) = 'OUTSIDE_START' OR UPPER(cable_type) = 'OUTSIDE_END' OR UPPER(cable_type) = 'LMC_START' OR UPPER(cable_type) = 'LMC_END') then
select (material_price_per_unit * in_cable_length), (service_price_per_unit * in_cable_length) into in_material_cost, in_service_cost from feasibility_cable_type
where cable_type_id = (select cable_type_id from feasibility_input where feasibility_id = (select feasibility_id from feasibility_history where history_id = in_history_id));	-- outside and lmc1 and lmc2
--select (service_price_per_unit * in_cable_length) into in_service_cost from feasibility_cable_type;
else
in_material_cost :=0;
in_service_cost :=0;
END IF;
if(UPPER(cable_type) = 'INSIDE' OR UPPER(cable_type) = 'INSIDE_A' OR UPPER(cable_type) = 'INSIDE_P') then
	-- select * from feasibility_inside_cable
	INSERT INTO public.feasibility_inside_cable(history_id, system_id, network_status, available_cores, total_cores, cable_length, cable_geometry, created_by, created_on)
	VALUES
	(in_history_id, in_system_id, in_network_status, in_avaliable_cores, in_total_cores, in_cable_length, in_Cable_GEOM, in_created_by, now());
ELSE
	INSERT INTO public.feasibility_geometry( history_id, type_id, cable_geometry, cable_length, created_by, created_on, material_cost, service_cost)
	VALUES
	 (in_history_id,in_type_id,in_Cable_GEOM,in_cable_length, in_created_by, now(), in_material_cost, in_service_cost);
END IF;
 RETURN QUERY 
	SELECT geom.id from feasibility_geometry geom where cable_geometry = in_Cable_GEOM and cable_length = in_cable_length;
END
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.sf_fn_insert_feasibility_geometry(integer, character varying, character varying, double precision, integer, integer, character varying, integer, integer)
  OWNER TO postgres;
--===========================================================================================================================
-- Nitya - 17/06
-- FUNCTION: public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying)

-- DROP FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_sf_get_past_feasibilities(
	p_searchtext character varying,
	p_searchby character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying)
RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE 
    ROWS 1000
AS $BODY$
 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
  -- V_HISTORY_VIEW_NAME character varying;
-- V_LAYER_COLUMNS text;
-- V_LAYER_ID  integer;
BEGIN
LowerStart:='';
LowerEnd:=''; 
WhereCondition:='';

   sql := 'SELECT  ROW_NUMBER() OVER (ORDER BY fh.history_id desc) AS S_No, fi.feasibility_id, fh.history_id, fi.feasibility_name, fi.customer_id, fi.customer_name, fi.start_point_name, fi.start_lat_lng, fi.end_point_name, fi.end_lat_lng, fi.cores_required,
	--ROUND(COALESCE(sum(fic.cable_length), 0)::numeric, 2)::double precision as exisiting_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id), 0)::numeric, 2)::double precision as exisiting_length, 
	-- ROUND(sum(fg.cable_length)::numeric, 2)::double precision as new_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=7), 0)::numeric, 2)::double precision as outside_a_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=8), 0)::numeric, 2)::double precision as outside_b_end, 
	fh.created_on::character varying as created_on, coalesce(fh.core_level_result, '' '') as core_level_result, 
	coalesce(fh.feasibility_result, '' '') as feasibility_result, fi.buffer_radius_a, fi.buffer_radius_b,
	fct.cable_type_id, fct.display_name as cable_type
	FROM feasibility_input fi join feasibility_history fh on fi.feasibility_id = fh.feasibility_id 
	JOIN feasibility_geometry fg on fh.history_id = fg.history_id
	JOIN feasibility_cable_type fct on fi.cable_type_id = fct.cable_type_id where fh.history_id > 0 ';
	
IF ((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) ='fi.feasibility_name') THEN
p_searchtext:=p_searchtext::character varying;
sql:= sql ||' and '|| p_searchby||' = '''||p_searchtext||'''';
else if((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) !='fi.feasibility_name')then
sql:= sql ||' and lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
end if;
END IF;
	--LEFT JOIN feasibility_inside_cable fic on fh.history_id = fic.history_id 
sql:= sql ||' GROUP BY fi.feasibility_id, fh.history_id, fct.cable_type_id';
RAISE INFO '%sql', sql;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$;

ALTER FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying)
    OWNER TO postgres;


-------========================================================================================================================================
-- FUNCTION: public.fn_sf_get_export_Bom_data()

-- DROP FUNCTION public.fn_sf_get_export_Bom_data()

CREATE OR REPLACE FUNCTION public.fn_sf_get_export_Bom_data()

RETURNS SETOF json 
LANGUAGE 'plpgsql'
    COST 100
    VOLATILE 
    ROWS 1000
AS $BODY$
DECLARE
    sql TEXT;   
BEGIN 
   sql:='SELECT a.cores, a.display_name, a.material_price_per_unit, a.service_price_per_unit, a.created_user, a.created_on, a.modified_user, a.modified_on
   from vw_feasibility_cable_type a where is_deleted=false order by cores';
	RAISE INFO 'query : %',sql;
RETURN QUERY
	execute 'select row_to_json(row) from ( '||sql|| ') row';
END;
$BODY$;

ALTER FUNCTION public.fn_sf_get_export_Bom_data()
    OWNER TO postgres;

--========================================================================================================
-- FUNCTION: public.fn_sf_get_feasibility_history_record(character varying, character varying, integer, integer, character varying, character varying, integer, character varying)

-- DROP FUNCTION public.fn_sf_get_feasibility_history_record(character varying, character varying, integer, integer, character varying, character varying, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_sf_get_feasibility_history_record(
	p_searchtext character varying,
	p_searchby character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_system_id integer,
	p_entity_name character varying)
RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE 
    ROWS 1000
AS $BODY$

 DECLARE
   sql TEXT;
     StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
   V_HISTORY_VIEW_NAME character varying;
V_LAYER_COLUMNS text;
V_LAYER_ID  integer;
BEGIN

LowerStart:='';
LowerEnd:='';

  
WhereCondition:='';

if(p_entity_name = 'FEASIBILITY_CABLE_TYPE')
then
V_HISTORY_VIEW_NAME ='vw_'||p_entity_name||'_history';

RAISE INFO '%', V_HISTORY_VIEW_NAME;
end if;

-- DYNAMIC QUERY
sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY audit_id desc) AS S_No,cores as SF_GBL_GBL_JQ_FRM_002,display_name as SF_GBL_GBL_NET_FRM_027,
                Material_Price_Per_Unit as SF_GBL_GBL_JQ_FRM_003,Service_Price_Per_Unit as SF_GBL_GBL_JQ_FRM_004,created_by as SF_GBL_GBL_JQ_FRM_005,created_on as SF_GBL_GBL_JQ_FRM_009,
                modified_by as SF_GBL_GBL_JQ_FRM_006,modified_on as SF_GBL_GBL_JQ_FRM_007,action as SF_GBL_GBL_JQ_FRM_008 from ' ||V_HISTORY_VIEW_NAME||' where cable_type_id='|| p_system_id ||' and action!=''Inserted'' ';

IF ((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) ='cores') THEN
p_searchtext:=p_searchtext::integer;
sql:= sql ||' AND '||p_searchby||' = '||p_searchtext||'';
else if((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) !='cores')then
sql:= sql ||' AND lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
end if;
END IF;

sql:= sql ||'order by cores,audit_id desc ';

RAISE INFO '%sql', sql;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$;

ALTER FUNCTION public.fn_sf_get_feasibility_history_record(character varying, character varying, integer, integer, character varying, character varying, integer, character varying)
    OWNER TO postgres;
-------------------------
--=================================================================================================================================
-- Unique column to display -- 18/06
alter table feasibility_history 
add history_display_id VARCHAR(20)

update feasibility_history
set history_display_id = concat('FS', LPAD(history_id::varchar,  6, '0'))

alter table feasibility_history 
add constraint unq_feas_his
unique (history_display_id);

alter table feasibility_history 
alter column history_display_id set default concat('FS', LPAD(currval('feasibility_history_history_id_seq'::regclass)::varchar,  6, '0'));
--===============================================================================================================================
-- 18/6 - run this after resource insert execution
update res_resources set value = 'LMC A Length (m)'
where value = 'LMC_A Length (m)'

update res_resources set value = 'LMC B Length (m)'
where value = 'LMC_B Length (m)'

--===============================================================================================================================
-- left ones - 19/06
-- View: public.vw_feasibility_cable_type
-- DROP VIEW public.vw_feasibility_cable_type;

CREATE OR REPLACE VIEW public.vw_feasibility_cable_type AS 
 SELECT fcth.cable_type_id,
    fcth.cores,
    upper(fcth.display_name::text) AS display_name,
    fcth.material_price_per_unit,
    fcth.service_price_per_unit,
    um.user_name AS created_user,
    fn_get_date(fcth.created_on) AS created_on,
    mum.user_name AS modified_user,
    fn_get_date(fcth.modified_on) AS modified_on,
    fcth.is_deleted
   FROM feasibility_cable_type fcth
     JOIN user_master um ON fcth.created_by = um.user_id
     LEFT JOIN user_master mum ON fcth.modified_by = mum.user_id
  ORDER BY fcth.created_on;

ALTER TABLE public.vw_feasibility_cable_type
  OWNER TO postgres;

-- Function: public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying)
-- DROP FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_sf_get_past_feasibilities(p_searchtext character varying, p_searchby character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying)
  RETURNS SETOF json AS
$BODY$
 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
  -- V_HISTORY_VIEW_NAME character varying;
-- V_LAYER_COLUMNS text;
-- V_LAYER_ID  integer;
BEGIN
LowerStart:='';
LowerEnd:=''; 
WhereCondition:='';

   sql := 'SELECT  ROW_NUMBER() OVER (ORDER BY fh.history_id desc) AS S_No, fi.feasibility_id, fh.history_id, fh.history_display_id, fi.feasibility_name, fi.customer_id, fi.customer_name, fi.start_point_name, fi.start_lat_lng, fi.end_point_name, fi.end_lat_lng, fi.cores_required,
	--ROUND(COALESCE(sum(fic.cable_length), 0)::numeric, 2)::double precision as exisiting_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id), 0)::numeric, 2)::double precision as exisiting_length, 
	-- ROUND(sum(fg.cable_length)::numeric, 2)::double precision as new_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=7), 0)::numeric, 2)::double precision as outside_a_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=8), 0)::numeric, 2)::double precision as outside_b_end, 
	fh.created_on::character varying as created_on, coalesce(fh.core_level_result, '' '') as core_level_result, 
	coalesce(fh.feasibility_result, '' '') as feasibility_result, fi.buffer_radius_a, fi.buffer_radius_b,
	fct.cable_type_id, fct.display_name 
	FROM feasibility_input fi join feasibility_history fh on fi.feasibility_id = fh.feasibility_id 
	JOIN feasibility_geometry fg on fh.history_id = fg.history_id
	JOIN feasibility_cable_type fct on fi.cable_type_id = fct.cable_type_id where fh.history_id > 0 ';
	
IF ((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) ='fi.feasibility_name') THEN
p_searchtext:=p_searchtext::character varying;
sql:= sql ||' and '|| p_searchby||' = '''||p_searchtext||'''';
else if((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) !='fi.feasibility_name')then
sql:= sql ||' and lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
end if;
END IF;
	--LEFT JOIN feasibility_inside_cable fic on fh.history_id = fic.history_id 
sql:= sql ||' GROUP BY fi.feasibility_id, fh.history_id, fct.cable_type_id';
RAISE INFO '%sql', sql;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying)
  OWNER TO postgres;


-- View: public.vw_feasibility_cable_type_history

-- DROP VIEW public.vw_feasibility_cable_type_history;

CREATE OR REPLACE VIEW public.vw_feasibility_cable_type_history AS 
 SELECT fcth.id AS audit_id,
    fcth.cable_type_id,
    fcth.cores,
    upper(fcth.display_name::text) AS display_name,
    fcth.material_price_per_unit,
    fcth.service_price_per_unit,
    um.user_name AS created_by,
    fn_get_date(fcth.created_on) AS created_on,
    mum.user_name AS modified_by,
    fn_get_date(fcth.modified_on) AS modified_on,
    fn_get_action(fcth.action) AS action
   FROM feasibility_cable_type_history fcth
     JOIN user_master um ON fcth.created_by = um.user_id
     LEFT JOIN user_master mum ON fcth.modified_by = mum.user_id
  ORDER BY fcth.created_on;

ALTER TABLE public.vw_feasibility_cable_type_history
  OWNER TO postgres;
--=============================================================================================================================

-- 25/06
alter table feasibility_cable_type
add is_used boolean default false;

update feasibility_cable_type
set is_used = true 
where cable_type_id in(select cable_type_id from feasibility_input fi)
and is_deleted = false;

--================================
CREATE OR REPLACE FUNCTION public.fn_fs_trg_feas_input()
  RETURNS trigger AS
$BODY$
BEGIN
IF (TG_OP = 'INSERT' ) THEN  
	UPDATE feasibility_cable_type
	SET is_used = true
	WHERE cable_type_id = new.cable_type_id;
END IF;
RETURN NEW;
END;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.fn_fs_trg_feas_input()
  OWNER TO postgres;
--=================================
CREATE TRIGGER trg_fs_feas_input
  AFTER INSERT OR UPDATE OR DELETE
  ON public.feasibility_input
  FOR EACH ROW
  EXECUTE PROCEDURE public.fn_fs_trg_feas_input();
--=================================
CREATE OR REPLACE VIEW public.vw_feasibility_cable_type AS 
 SELECT fcth.cable_type_id,
    fcth.cores,
    upper(fcth.display_name::text) AS display_name,
    fcth.material_price_per_unit,
    fcth.service_price_per_unit,
    um.user_name AS created_user,
    fn_get_date(fcth.created_on) AS created_on,
    mum.user_name AS modified_user,
    fn_get_date(fcth.modified_on) AS modified_on,
    fcth.is_deleted,
    fcth.is_used
   FROM feasibility_cable_type fcth
     JOIN user_master um ON fcth.created_by = um.user_id
     LEFT JOIN user_master mum ON fcth.modified_by = mum.user_id
  ORDER BY fcth.created_on;

ALTER TABLE public.vw_feasibility_cable_type
  OWNER TO postgres;
--===================================================================================================
-- Nitya - 24/06
-- FUNCTION: public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying)

-- DROP FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_sf_get_past_feasibilities(
	p_searchtext character varying,
	p_searchby character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_fromdate character varying,
	p_todate character varying)
RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE 
    ROWS 1000
AS $BODY$
 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
  -- V_HISTORY_VIEW_NAME character varying;
-- V_LAYER_COLUMNS text;
-- V_LAYER_ID  integer;
BEGIN
LowerStart:='';
LowerEnd:=''; 
WhereCondition:='';

   sql := 'SELECT  ROW_NUMBER() OVER (ORDER BY fh.history_id desc) AS S_No, fi.feasibility_id, fh.history_id, fh.history_display_id, fi.feasibility_name, fi.customer_name, fi.customer_id, fi.start_point_name, fi.start_lat_lng, fi.end_point_name, fi.end_lat_lng, fi.cores_required,
	--ROUND(COALESCE(sum(fic.cable_length), 0)::numeric, 2)::double precision as exisiting_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id and fic.network_status=''P''), 0)::numeric, 2)::double precision as inside_p, 
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id and fic.network_status=''A''), 0)::numeric, 2)::double precision as inside_a, 
	-- ROUND(sum(fg.cable_length)::numeric, 2)::double precision as new_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=7), 0)::numeric, 2)::double precision as outside_a_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=8), 0)::numeric, 2)::double precision as outside_b_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=5), 0)::numeric, 2)::double precision as lmc_a, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=6), 0)::numeric, 2)::double precision as lmc_b, 
	fi.buffer_radius_a, fi.buffer_radius_b, fh.created_on::character varying as created_on, coalesce(fh.core_level_result, '' '') as core_level_result, 
	coalesce(fh.feasibility_result, '' '') as feasibility_result, 
	fct.cable_type_id, fct.display_name as cable_type
	FROM feasibility_input fi join feasibility_history fh on fi.feasibility_id = fh.feasibility_id 
	JOIN feasibility_geometry fg on fh.history_id = fg.history_id
	JOIN feasibility_cable_type fct on fi.cable_type_id = fct.cable_type_id where fh.history_id > 0 ';
	
IF ((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) ='fi.feasibility_name') THEN
p_searchtext:=p_searchtext::character varying;
sql:= sql ||' and '|| p_searchby||' = '''||p_searchtext||'''';
else if((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) !='fi.feasibility_name')then
sql:= sql ||' and lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
end if;
IF ((p_fromdate) != '' and (p_todate) != '') THEN
sql:= sql || ' and DATE(fh.created_on) BETWEEN '''|| p_fromdate ||''' and ''' || p_todate ||'''';
else 
sql:= sql;
end if;
END IF;
	--LEFT JOIN feasibility_inside_cable fic on fh.history_id = fic.history_id 
sql:= sql ||' GROUP BY fi.feasibility_id, fh.history_id, fct.cable_type_id';
RAISE INFO '%sql', sql;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$;

ALTER FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying)
    OWNER TO postgres;
--==============================================================================================================================================

-- Function: public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying)

-- DROP FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_sf_get_past_feasibilities(p_searchtext character varying, p_searchby character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_fromdate character varying, p_todate character varying)
  RETURNS SETOF json AS
$BODY$
 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
  -- V_HISTORY_VIEW_NAME character varying;
-- V_LAYER_COLUMNS text;
-- V_LAYER_ID  integer;
BEGIN
LowerStart:='';
LowerEnd:=''; 
WhereCondition:='';

   sql := 'SELECT  ROW_NUMBER() OVER (ORDER BY fh.history_id desc) AS S_No, fi.feasibility_id, fh.history_id, fh.history_display_id, fi.feasibility_name, fi.customer_name, fi.customer_id, fi.start_point_name, fi.start_lat_lng, fi.end_point_name, fi.end_lat_lng, fi.cores_required,
	--ROUND(COALESCE(sum(fic.cable_length), 0)::numeric, 2)::double precision as exisiting_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id and fic.network_status=''P''), 0)::numeric, 2)::double precision as inside_p, 
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id and fic.network_status=''A''), 0)::numeric, 2)::double precision as inside_a, 

	-- ROUND(sum(fg.cable_length)::numeric, 2)::double precision as new_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=7), 0)::numeric, 2)::double precision as outside_a_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=8), 0)::numeric, 2)::double precision as outside_b_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=5), 0)::numeric, 2)::double precision as lmc_a, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=6), 0)::numeric, 2)::double precision as lmc_b, 
	fi.buffer_radius_a, fi.buffer_radius_b, fh.created_on::character varying as created_on, coalesce(fh.core_level_result, '' '') as core_level_result, 
	coalesce(fh.feasibility_result, '' '') as feasibility_result, 
	fct.cable_type_id, fct.display_name as cable_type
	FROM feasibility_input fi join feasibility_history fh on fi.feasibility_id = fh.feasibility_id 
	JOIN feasibility_geometry fg on fh.history_id = fg.history_id
	JOIN feasibility_cable_type fct on fi.cable_type_id = fct.cable_type_id where fh.history_id > 0 ';
	
--IF ((p_searchtext) != '' and (p_searchby) != '' and (p_searchby) ='fi.feasibility_name') THEN
--p_searchtext:=p_searchtext::character varying;
--sql:= sql ||' and '|| p_searchby||' = '''||p_searchtext||'''';
--else 
if((p_searchtext) != '' and (p_searchby) != '') then -- and (p_searchby) !='fi.feasibility_name')then
sql:= sql ||' and lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
end if;
IF ((p_fromdate) != '' and (p_todate) != '') THEN
sql:= sql || ' and DATE(fh.created_on) BETWEEN '''|| p_fromdate ||''' and ''' || p_todate ||'''';
else 
sql:= sql;
end if;
--END IF;
	--LEFT JOIN feasibility_inside_cable fic on fh.history_id = fic.history_id 
sql:= sql ||' GROUP BY fi.feasibility_id, fh.history_id, fct.cable_type_id';
RAISE INFO '%sql', sql;

-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
 END IF; 

RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying)
  OWNER TO postgres;

--====================================================================================================================================================
-- 01/09/2020 - Hari
CREATE OR REPLACE FUNCTION public.fn_sf_get_past_feasibilities(p_history_ids character varying)
  RETURNS SETOF json AS
$BODY$
DECLARE
   sql TEXT;
BEGIN
	sql := 'select row_to_json(row) from (
	SELECT  fi.feasibility_id, fh.history_id, fh.history_display_id, fi.feasibility_name, fi.customer_name, fi.customer_id, fi.start_point_name, fi.start_lat_lng, fi.end_point_name, fi.end_lat_lng, fi.cores_required,
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id and fic.network_status=''P''), 0)::numeric, 2)::double precision as inside_p, 
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id and fic.network_status=''A''), 0)::numeric, 2)::double precision as inside_a, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=7), 0)::numeric, 2)::double precision as outside_a_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=8), 0)::numeric, 2)::double precision as outside_b_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=5), 0)::numeric, 2)::double precision as lmc_a, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=6), 0)::numeric, 2)::double precision as lmc_b, 
	fi.buffer_radius_a, fi.buffer_radius_b, coalesce(fh.core_level_result, '' '') as core_level_result, 
	coalesce(fh.feasibility_result, '' '') as feasibility_result, fct.display_name as cable_type
	FROM feasibility_input fi join feasibility_history fh on fi.feasibility_id = fh.feasibility_id 
	JOIN feasibility_geometry fg on fh.history_id = fg.history_id
	JOIN feasibility_cable_type fct on fi.cable_type_id = fct.cable_type_id where fh.history_id in (' || p_history_ids || ')
	GROUP BY fi.feasibility_id, fh.history_id, fct.cable_type_id) row';

	RETURN QUERY
	EXECUTE sql;

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_past_feasibilities(p_history_ids character varying)
  OWNER TO postgres;

--====================================================================================================================
-- Hari on 01/09/2020
CREATE OR REPLACE FUNCTION public.fn_sf_get_inside_cables(p_history_ids character varying)
  RETURNS SETOF json AS
$BODY$
DECLARE
   sql TEXT;
BEGIN

	sql := 'select row_to_json(row) from (
		select fh.history_display_id as "historyId", adc.network_id as cable_id, adc.cable_name, fic.network_status,
		fic.total_cores, fic.total_cores - fic.available_cores as used_cores, fic.available_cores,
		fic.cable_length 
		from 
		feasibility_inside_cable fic join att_details_cable adc on category = ''Cable'' and fic.system_id = adc.system_id 
		join feasibility_history fh on fic.history_id = fh.history_id 
		where fh.history_id in (' || p_history_ids || ')) row';

	RETURN QUERY
	EXECUTE sql;

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_inside_cables(p_history_ids character varying)
  OWNER TO postgres;

--====================================================================================================================
-- Hari on 15/09/2020 --- added comma separated historIDs filter to be used in bulk feasibility KML
-- Function: public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying)

-- DROP FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_sf_get_past_feasibilities(p_searchtext character varying, p_searchby character varying, p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_fromdate character varying, p_todate character varying)
  RETURNS SETOF json AS
$BODY$
 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
  -- V_HISTORY_VIEW_NAME character varying;
-- V_LAYER_COLUMNS text;
-- V_LAYER_ID  integer;
BEGIN
LowerStart:='';
LowerEnd:=''; 
WhereCondition:='';

   sql := 'SELECT  ROW_NUMBER() OVER (ORDER BY fh.history_id desc) AS S_No, fi.feasibility_id, fh.history_id, fh.history_display_id, fi.feasibility_name, fi.customer_name, fi.customer_id, fi.start_point_name, fi.start_lat_lng, fi.end_point_name, fi.end_lat_lng, fi.cores_required,
	--ROUND(COALESCE(sum(fic.cable_length), 0)::numeric, 2)::double precision as exisiting_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id and fic.network_status=''P''), 0)::numeric, 2)::double precision as inside_p, 
	ROUND(coalesce((select sum(cable_length) from feasibility_inside_cable fic where fic.history_id = fh.history_id and fic.network_status=''A''), 0)::numeric, 2)::double precision as inside_a, 

	-- ROUND(sum(fg.cable_length)::numeric, 2)::double precision as new_length,
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=7), 0)::numeric, 2)::double precision as outside_a_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=8), 0)::numeric, 2)::double precision as outside_b_end, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=5), 0)::numeric, 2)::double precision as lmc_a, 
	ROUND(coalesce((select sum(cable_length) from feasibility_geometry where history_id = fh.history_id and type_id=6), 0)::numeric, 2)::double precision as lmc_b, 
	fi.buffer_radius_a, fi.buffer_radius_b, fh.created_on::character varying as created_on, coalesce(fh.core_level_result, '' '') as core_level_result, 
	coalesce(fh.feasibility_result, '' '') as feasibility_result, 
	fct.cable_type_id, fct.display_name as cable_type
	FROM feasibility_input fi join feasibility_history fh on fi.feasibility_id = fh.feasibility_id 
	JOIN feasibility_geometry fg on fh.history_id = fg.history_id
	JOIN feasibility_cable_type fct on fi.cable_type_id = fct.cable_type_id where fh.history_id > 0 ';
	

	if(p_searchtext) != '' and (p_searchby) = 'historIDs' then 
		sql:= sql ||' and fh.history_id in ('||p_searchtext||')';
	else
		if((p_searchtext) != '' and (p_searchby) != '') then -- and (p_searchby) !='fi.feasibility_name')then
			sql:= sql ||' and lower('||p_searchby||') LIKE lower(''%'||p_searchtext||'%'')';
		end if;
		IF ((p_fromdate) != '' and (p_todate) != '') THEN
			sql:= sql || ' and DATE(fh.created_on) BETWEEN '''|| p_fromdate ||''' and ''' || p_todate ||'''';
		else 
			sql:= sql;
		end if;
	end if;

	sql:= sql ||' GROUP BY fi.feasibility_id, fh.history_id, fct.cable_type_id';
	RAISE INFO '%sql', sql;

	-- GET TOTAL RECORD COUNT
	EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;
	--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
	 IF((P_PAGENO) <> 0) THEN
		StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
		EndSNo:= P_PAGENO * P_PAGERECORD;
		sql:= 'SELECT '||TotalRecords||' as totalRecords, *
			FROM (' || sql || ' ) T 
			WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

	 ELSE
		 sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T';                  
	 END IF; 

	RAISE INFO '%', sql;
	RETURN QUERY
	EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_past_feasibilities(character varying, character varying, integer, integer, character varying, character varying, character varying, character varying)
  OWNER TO postgres;
--==============================================================================================

CREATE OR REPLACE FUNCTION public.fn_sf_get_available_cores(p_cableid integer)
  RETURNS integer AS
$BODY$
DECLARE
    available_cores INTEGER;
BEGIN
	select COUNT(*) into available_cores from att_details_cable_info
	where cable_id = p_cableid and a_end_status_id = 1 and b_end_status_id = 1; --is_available = true;

	return available_cores;
	
END; $BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION public.fn_sf_get_available_cores(integer)
  OWNER TO postgres;
  
--================================================================================================  