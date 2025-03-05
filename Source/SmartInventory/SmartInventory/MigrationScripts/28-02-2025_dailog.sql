INSERT INTO public.module_master
(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES('Item Vendor Cost', 'Item Vendor Cost', NULL, 'icon-Vendor', 1, now(), null, null, 'Admin', true, 'ICVNDR', 0, (SELECT MAX(module_sequence)+1 FROM MODULE_MASTER ), false, NULL, NULL);

 
INSERT INTO public.module_master
 (module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES( 'View Item Vendor Cost', 'View Item Vendor Cost', NULL, 'right-arrow', 1, now(), null,null, 'Admin', true, 'ICVNDR-VW', (select id from module_master where module_name='Item Vendor Cost'), 1, false, 'Vendor/ViewItemVendorCost', NULL);

 
INSERT INTO public.module_master
 (module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled, form_url, connection_id)
VALUES( 'Add Item Vendor Cost', 'Add Item Vendor Cost', NULL, 'right-arrow', 1, now(), null,null, 'Admin', true, 'ICVNDR-ADD', (select id from module_master where module_name='Item Vendor Cost'), 1, false, 'Vendor/AddItemVendorCost', NULL);

---------------------------------------------------------------------------------------------------------------------------------------------------
-- Table: public.item_vendor_cost_master

-- DROP TABLE IF EXISTS public.att_details_site;

CREATE TABLE IF NOT EXISTS public.item_vendor_cost_master
(
    id serial,
    layer_id integer,
    specification character varying COLLATE pg_catalog."default",  
    item_code character varying COLLATE pg_catalog."default",
    user_id integer,
    item_cost double precision,
    created_on timestamp without time zone,
    created_by integer,
    modified_on timestamp without time zone,
    modified_by integer,
	uom character varying COLLATE pg_catalog."default",
    CONSTRAINT item_vendor_cost_master_pk PRIMARY KEY(id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.item_vendor_cost_master
    OWNER to postgres;

-- Trigger: trg_audit_item_vendor_cost_master

-- DROP TRIGGER IF EXISTS trg_audit_item_vendor_cost_master ON public.item_vendor_cost_master;

CREATE TRIGGER trg_audit_item_vendor_cost_master
    AFTER INSERT OR DELETE OR UPDATE 
    ON public.item_vendor_cost_master
    FOR EACH ROW
    EXECUTE FUNCTION public.fn_audit_item_vendor_cost_master();
	------------------------------------------------------------------------------------------------------------------------------------------------
	-- Table: public.audit_item_vendor_cost_master

-- DROP TABLE IF EXISTS public.audit_item_vendor_cost_master;

CREATE TABLE IF NOT EXISTS public.audit_item_vendor_cost_master
(
    id serial,
    layer_id integer,
    specification character varying COLLATE pg_catalog."default",  
    item_code character varying COLLATE pg_catalog."default",
    user_id integer,
    item_cost double precision,
    created_on timestamp without time zone,
    created_by integer,
    modified_on timestamp without time zone,
    modified_by integer,
	uom character varying COLLATE pg_catalog."default",
	action character varying COLLATE pg_catalog."default",
	item_cost_id integer,
    CONSTRAINT audit_item_vendor_cost_master_pk PRIMARY KEY(id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.audit_item_vendor_cost_master
    OWNER to postgres;
	----------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS public.site_award_details
(
    id serial,   
    specification character varying COLLATE pg_catalog."default",  
    item_code character varying COLLATE pg_catalog."default",
    user_id integer,
    --item_cost double precision,
    created_on timestamp without time zone,
    created_by integer,
    modified_on timestamp without time zone,
    modified_by integer,
	uom character varying COLLATE pg_catalog."default",
	--action character varying COLLATE pg_catalog."default",
	item_cost_audit_id integer,
	site_plan_id integer,
    CONSTRAINT site_award_details_pk PRIMARY KEY(id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.site_award_details
    OWNER to postgres;
	-----------------------------------------------------------------------------------------------------------------------------------------
	-- FUNCTION: public.trg_audit_item_vendor_cost_master()

-- DROP FUNCTION IF EXISTS public.trg_audit_item_vendor_cost_master();

CREATE OR REPLACE FUNCTION public.trg_audit_item_vendor_cost_master()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$

BEGIN
 IF (TG_OP = 'INSERT' ) THEN  
 INSERT INTO public.audit_item_vendor_cost_master
(item_cost_id, layer_id, specification, item_code, user_id, item_cost, created_on, created_by,
 modified_on, modified_by,UOM,action)
 select  new.id,new.layer_id,new.specification,new.item_code,new.user_id,new.item_cost,
 new.created_on,new.created_by,new.modified_on,new.modified_by,new.UOM, 'I' as action 
 from item_vendor_cost_master where id=new.id;
END IF;
IF (TG_OP = 'UPDATE' ) THEN  

INSERT INTO public.audit_item_vendor_cost_master
(item_cost_id, layer_id, specification, item_code, user_id, item_cost, created_on, created_by,
 modified_on, modified_by,UOM,action)
 select  new.id,new.layer_id,new.specification,new.item_code,new.user_id,new.item_cost,
 new.created_on,new.created_by,new.modified_on,new.modified_by,new.UOM, 'U' as action 
 from item_vendor_cost_master where id=new.id;
END IF; 				
IF (TG_OP = 'DELETE' ) THEN  

INSERT INTO public.audit_item_vendor_cost_master(
item_cost_id, layer_id, specification, item_code, user_id, item_cost, created_on, created_by,
 modified_on, modified_by,UOM,action)
 select  old.id,old.layer_id,old.specification,old.item_code,old.user_id,old.item_cost,
 old.created_on,old.created_by,old.modified_on,old.modified_by,old.UOM, 'D' as action 
;           
END IF; 		

RETURN NEW;
END;

$BODY$;

ALTER FUNCTION public.trg_audit_item_vendor_cost_master()
    OWNER TO postgres;

-------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_vendor_spec_item_specification(integer)

-- DROP FUNCTION IF EXISTS public.fn_vendor_spec_item_specification(integer);

CREATE OR REPLACE FUNCTION public.fn_vendor_spec_item_specification(
	p_layer_id integer)
    RETURNS TABLE(key character varying, value character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    sql TEXT;
BEGIN
  
 RETURN QUERY  select specification as key , specification as value from item_template_master where is_master=true and layer_id=p_layer_id;
  
END;
$BODY$;

ALTER FUNCTION public.fn_vendor_spec_item_specification(integer)
    OWNER TO postgres;
-----------------------------------------------------------------------------------------------------------------------------------------------------
	-- FUNCTION: public.fn_item_vendor_spec_item_code(integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_item_vendor_spec_item_code(integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_item_vendor_spec_item_code(
	p_layer_id integer,
	p_specification character varying)
    RETURNS TABLE(key character varying, value character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    sql TEXT;
BEGIN
  
 RETURN QUERY  select code as key , code as value from item_template_master 
 where is_master=true and layer_id=p_layer_id and specification=p_specification;
  
END;
$BODY$;

ALTER FUNCTION public.fn_item_vendor_spec_item_code(integer, character varying)
    OWNER TO postgres;
-------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_item_vendor_spec_item_category(integer, character varyin)

-- DROP FUNCTION IF EXISTS public.fn_item_vendor_spec_item_category();

CREATE OR REPLACE FUNCTION public.fn_item_vendor_spec_item_category(
	)
    RETURNS TABLE(key int, value character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    sql TEXT;
BEGIN
  
 RETURN QUERY  select DISTINCT layer_id as key , category_reference as value from item_template_master 
 where is_master=true;
  
END;
$BODY$;

ALTER FUNCTION public.fn_item_vendor_spec_item_category()
    OWNER TO postgres;
	--------------------------------------------------------------------------------------------------------------------------------------------
	-- FUNCTION: public.fn_get_item_vendor_cost(character varying, character varying, integer, integer, character varying, character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_item_vendor_cost(character varying, character varying, integer, integer, character varying, character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_get_item_vendor_cost(
	searchby character varying,
	searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_totalrecords integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

 DECLARE
    sql TEXT;
    SqlQueryCnt TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying; 
begin
	
	if (searchby !~* '^[a-zA-Z0-9\s_]*$' or searchtext !~* '^[a-zA-Z0-9\s_]*$' or p_sortcolname !~* '^[a-zA-Z0-9\s_]*$' or
		p_sorttype !~* '^[a-zA-Z0-9\s_]*$') then
		
		RETURN QUERY
		EXECUTE 'select row_to_json(row) from (select 1 where 1 = 2) row';
	
	else
		BEGIN

			-- RAISE INFO '%', sql;  
			sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (P_SORTCOLNAME) = '' THEN 'i.id' ELSE P_SORTCOLNAME END ||' ' ||CASE WHEN 

			(P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||') AS S_No,
			 
    i.id,i.code, 
    i.specification, 
	i.unit_measurement,
    COALESCE(ivcm.item_cost, 0) as cost_per_unit,
	i.category_reference,
	i.layer_id,
    u.user_name, 
    u.user_id
FROM 
    item_template_master i
	
CROSS JOIN 
    user_master u
	left join item_vendor_cost_master ivcm on i.code=ivcm.item_code and u.user_id=ivcm.user_id
	
WHERE 
    
     u.user_type = ''partner''';

			IF (upper(searchText) != '' and (searchby) != '') THEN
			--sql:= sql ||' AND lower('||searchby||') LIKE lower(''%'||searchText||'%'')';
			sql:= sql ||' AND lower(i."'|| quote_ident($1) ||'") LIKE lower(''%'|| $2 ||'%'')';
			END IF;

			SqlQueryCnt:= 'SELECT COUNT(1)  FROM ('||sql||') as a';
			 EXECUTE   SqlQueryCnt INTO P_TOTALRECORDS;
			 IF((P_PAGENO) <> 0) THEN
			  
				StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
				EndSNo:= P_PAGENO * P_PAGERECORD;
				sql:= 'SELECT '||P_TOTALRECORDS||' as totalRecord, *
							FROM (' || sql || ' ) T 
							WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

				 ELSE
						   sql:= 'SELECT '||P_TOTALRECORDS||' as totalRecord, * FROM (' || sql || ' order by ivcm.item_cost desc) T';                  
			  END IF; 

			RAISE INFO '%', sql;
				
			RETURN QUERY
			EXECUTE 'select row_to_json(row) from ('||sql||') row';

		end;
	end if;
END ;
$BODY$;

ALTER FUNCTION public.fn_get_item_vendor_cost(character varying, character varying, integer, integer, character varying, character varying, integer)
    OWNER TO postgres;

------------------------------------------------------------------------------------------------------------------------------------------------------

--select * from fn_insert_site_award_details('167,POP001,CBL003,PL001','generic,Generic,48F Cable,7 M Light Pole','12,12,12,12')

CREATE OR REPLACE FUNCTION public.fn_insert_site_award_details(
    p_itemcode character varying,
    p_specification character varying,
    p_userid character varying)
RETURNS TABLE(status boolean, message character varying) 
LANGUAGE plpgsql
AS $BODY$
DECLARE
    v_status BOOLEAN := TRUE;
    v_message CHARACTER VARYING := 'Success';
    v_inserted_count INTEGER := 0;
    v_skipped_count INTEGER := 0;
BEGIN
    -- Step 1: Create a temporary table to hold input values
    CREATE TEMP TABLE temp_input_data (
        item_code VARCHAR,
        specification VARCHAR,
        user_id INTEGER
    ) ON COMMIT DROP;

    -- Step 2: Insert parsed input values into temp table
    INSERT INTO temp_input_data (item_code, specification, user_id)
    SELECT 
        UNNEST(string_to_array(p_itemcode, ',')) AS item_code,
        UNNEST(string_to_array(p_specification, ',')) AS specification,
        UNNEST(string_to_array(p_userid, ','))::INTEGER AS user_id;

    -- Step 3: Count existing records before insert
    SELECT COUNT(*)
    INTO v_skipped_count
    FROM temp_input_data id
    WHERE EXISTS (
        SELECT 1 FROM site_award_details sa 
        WHERE sa.item_code = id.item_code
        AND sa.specification = id.specification
        AND sa.user_id = id.user_id
    );

    -- Step 4: Insert only non-existing records and get count
    INSERT INTO site_award_details (item_code, specification, user_id, item_cost_audit_id)
    SELECT 
        id.item_code, 
        id.specification, 
        id.user_id,
        COALESCE(ai.id, NULL) -- Fetch auditid from audit_info
    FROM temp_input_data id
    LEFT JOIN audit_item_vendor_cost_master ai 
        ON ai.item_code = id.item_code 
        AND ai.specification = id.specification
        AND ai.user_id = id.user_id
    WHERE NOT EXISTS (
        SELECT 1 FROM site_award_details sa 
        WHERE sa.item_code = id.item_code
        AND sa.specification = id.specification
        AND sa.user_id = id.user_id
    );

    -- Step 5: Get the count of inserted rows
    GET DIAGNOSTICS v_inserted_count = ROW_COUNT;

    -- Step 6: Construct the final message
    IF v_inserted_count > 0 AND v_skipped_count > 0 THEN
        v_message := 'Site Awarded successfully';
    ELSIF v_inserted_count > 0 THEN
        v_message :='Site Awarded successfully';
    ELSIF v_skipped_count > 0 THEN
        v_message := 'Site already Awarded';
        v_status := FALSE;
    END IF;

    -- Step 7: Return the final status
	RETURN QUERY
    select v_status as status,v_message::character varying as message;
END;
$BODY$;

-----------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_nwt_insert_update_ticket_IVCM(integer, integer, character varying, character varying, integer, integer, character varying, character varying, integer, character varying, character varying, character varying, integer, character varying, character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_nwt_insert_update_ticket_IVCM(integer, integer, character varying, character varying, integer, integer, character varying, character varying, integer, character varying, character varying, character varying, integer, character varying, character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_nwt_insert_update_ticket_IVCM(
	p_ticket_id integer,
	p_ticket_type_id integer,
	p_reference_type character varying,
	p_reference_description character varying,
	p_regionid integer,
	p_provinceid integer,
	p_network_id character varying,
	p_name character varying,
	p_assigned_to integer,
	p_target_date character varying,
	p_network_status character varying,
	p_remarks character varying,
	p_created_by integer,
	p_geom character varying,
	p_reference_ticket_id character varying,
	p_project_code character varying,
	p_account_code character varying)
    RETURNS SETOF character varying 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


declare
 v_system_id   integer;
 v_network_id character varying;
 v_geom_query geometry;
BEGIN
 IF EXISTS (SELECT 1 FROM att_details_networktickets  WHERE  ticket_id=p_ticket_id) then
BEGIN
   Update att_details_networktickets set ticket_type_id=p_ticket_type_id,reference_type=p_reference_type,region_id=p_regionId,province_id=p_provinceId,name=p_name,assigned_to=p_assigned_to,
   target_date=p_target_date::date,for_network_type=p_network_status,remarks=p_remarks,reference_description=p_reference_description,modified_by=p_created_by,modified_on=now(),
   reference_ticket_id=p_reference_ticket_id,project_code=p_project_code,account_code=p_account_code where ticket_id=p_ticket_id and network_id=p_network_id;
  return query  select 'Update'::character varying as msg;
END;
 ELSE 
BEGIN
select * from fn_nwt_get_network_id(p_ticket_type_id,p_provinceId)into v_network_id;
--ticket_status_id 4 is Name "Assigned" module "NetworkTicket";
  insert into att_details_networktickets (ticket_type_id,reference_type,region_id,province_id,network_id,name,assigned_to,target_date,for_network_type,
  remarks,reference_description,created_by,created_on,ticket_status_id,reference_ticket_id,project_code,account_code) 
  values(p_ticket_type_id,p_reference_type,p_regionId,p_provinceId,v_network_id,p_name,p_assigned_to,p_target_date::date,p_network_status,p_remarks,
  p_reference_description,p_created_by,now(),4,p_reference_ticket_id,p_project_code,p_account_code);
  select ticket_id into v_system_id from att_details_networktickets where network_id=v_network_id;

IF(p_geom!='')Then
--select ticket_id into v_system_id from att_details_networktickets where network_id=v_network_id;
--v_geom_query = ST_FORCE2D(ST_GEOMFROMTEXT('POLYGON(('||p_geom||'))',4326));
v_geom_query = ST_Buffer(ST_FORCE2D(ST_GEOMFROMTEXT('POLYGON(('||p_geom||'))',4326)),10);
insert into polygon_master(system_id,entity_type,approval_flag,sp_geometry,created_by,common_name,approval_date,network_status,display_name)
values(v_system_id,'Network_Ticket','A',v_geom_query::geometry,p_created_by,v_network_id,now(),p_network_status,fn_get_display_name(v_system_id,'Network_Ticket'));

END IF;
 perform fn_geojson_update_entity_attribute(v_system_id, 'Network_Ticket',p_provinceId,0,false);
  return query  select 'Save'::character varying as msg;
END;
END IF;

END;
$BODY$;

ALTER FUNCTION public.fn_nwt_insert_update_ticket_IVCM(integer, integer, character varying, character varying, integer, integer, character varying, character varying, integer, character varying, character varying, character varying, integer, character varying, character varying, character varying, character varying)
    OWNER TO postgres;
--------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_site_bom_boq_report_new(character varying, integer, character varying, character varying, integer, integer, character varying, character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_site_bom_boq_report_new(character varying, integer, character varying, character varying, integer, integer, character varying, character varying, integer);
CREATE OR REPLACE FUNCTION public.fn_get_site_bom_boq_report_new(
	p_entitytype character varying,
	p_site_plan_id integer,
	searchby character varying,
	searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_totalrecords integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
V_SQLQUERY TEXT;
V_LAYER_ID integer;
V_LAYER_NAME CHARACTER VARYING;
V_LAYER_TITLE CHARACTER VARYING;
V_LAYER_TABLE CHARACTER VARYING;
V_GEOM_TYPE CHARACTER VARYING;
V_ENTITY_SUB_TYPE_COLUMN CHARACTER VARYING;
V_CALCULATED_LENGTH_COLUMN CHARACTER VARYING;
V_GIS_LENGTH_COLUMN CHARACTER VARYING;
V_ORDER_BY_STATEMENT CHARACTER VARYING;
V_Network_Status character varying;
CURBOMBOQLAYERS REFCURSOR;

arow record;
V_SqlFinalQuery text;
Q_SelectedNetworkStatus CHARACTER VARYING;
V_CommonCost text;
V_CommonServiceCost text;
V_CommonCount text;
V_CommonCLength text;
V_CommonGISLength text;
S_LAYER_COLUMNS_VAL text;

N_SQL_VAL text;
SqlQueryCnt TEXT;
 StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying; 
BEGIN
--DROP TABLE IF EXISTS Itemcost;

 create temp TABLE Rec
  (
  ID SERIAL,
  ENTITY_TYPE CHARACTER VARYING(100),
  GEOM_TYPE CHARACTER VARYING(100),
  
  COST_PER_UNIT NUMERIC,
  SERVICE_COST_PER_UNIT NUMERIC DEFAULT 0,
  SPECIFICATION CHARACTER VARYING(100),
  ITEM_CODE CHARACTER VARYING(100),
  USER_ID INTEGER,
	  UNIT_MEASUREMENT CHARACTER VARYING(100),
	  ITEM_COST NUMERIC DEFAULT 0,
  TOTAL_PLANNED_COUNT BIGINT,
  TOTAL_ASBUILT_COUNT BIGINT,
  TOTAL_DORMANT_COUNT BIGINT,
  TOTAL_PLANNED_SERVICE_COST NUMERIC DEFAULT 0,
  TOTAL_ASBUILT_SERVICE_COST NUMERIC DEFAULT 0,
  TOTAL_DORMANT_SERVICE_COST NUMERIC DEFAULT 0,
  TOTAL_PLANNED_COST NUMERIC,
  TOTAL_ASBUILT_COST NUMERIC,
  TOTAL_DORMANT_COST NUMERIC,
  TOTAL_PLANNED_CALC_LENGTH NUMERIC DEFAULT 0,
  TOTAL_ASBUILT_CALC_LENGTH NUMERIC DEFAULT 0,
  TOTAL_DORMANT_CALC_LENGTH NUMERIC DEFAULT 0,
  TOTAL_PLANNED_GIS_LENGTH NUMERIC DEFAULT 0,
  TOTAL_ASBUILT_GIS_LENGTH NUMERIC DEFAULT 0,
  TOTAL_DORMANT_GIS_LENGTH NUMERIC DEFAULT 0,
  IS_HEADER BOOLEAN DEFAULT FALSE,
  VENDOR_NAME  CHARACTER VARYING(100),
  USER_NAME  CHARACTER VARYING(100),
	  
  TOTAL_COST NUMERIC DEFAULT 0,
  TOTAL_QTY NUMERIC DEFAULT 0
 
  )ON COMMIT DROP; 	   
---------- Assing in case of multiple selection

OPEN CURBOMBOQLAYERS FOR SELECT L.LAYER_ID, L.LAYER_NAME,L.LAYER_TITLE,L.LAYER_TABLE,L.GEOM_TYPE,B.ENTITY_SUB_TYPE_COLUMN,B.CALCULATED_LENGTH_COLUMN,B.GIS_LENGTH_COLUMN,B.ORDER_BY_STATEMENT 
FROM BOM_BOQ_MASTER B INNER JOIN LAYER_DETAILS L ON B.LAYER_ID=L.LAYER_ID
 WHERE (L.ISVISIBLE=TRUE or L.is_visible_in_ne_library=true) AND B.IS_ENABLED=TRUE and L.Layer_Id in
 (select distinct layer_id from role_permission_entity pe
 where pe.layer_id = L.Layer_Id and pe.role_id = 2 and pe.viewonly = true and network_status in
  ('P','A','D')
   and L.layer_id in 
  (13,19,10,14) )and case when UPPER('POD')=upper('STRUCTURE') then is_isp_layer=true else 1=1 end;

 LOOP
 FETCH CURBOMBOQLAYERS  INTO V_LAYER_ID, V_LAYER_NAME,V_LAYER_TITLE,V_LAYER_TABLE,V_GEOM_TYPE,V_ENTITY_SUB_TYPE_COLUMN,
  V_CALCULATED_LENGTH_COLUMN,V_GIS_LENGTH_COLUMN,V_ORDER_BY_STATEMENT;

  IF NOT FOUND THEN
  EXIT;
 END IF;
 SELECT STRING_AGG(COLUMN_NAME, ',') INTO S_LAYER_COLUMNS_VAL FROM (
 SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE upper(TABLE_NAME) = UPPER(V_LAYER_TABLE)) A;

 --CHECK FOR NULL ALL COLUMN..
 V_ENTITY_SUB_TYPE_COLUMN:=CASE WHEN V_ENTITY_SUB_TYPE_COLUMN!='' THEN V_ENTITY_SUB_TYPE_COLUMN ELSE '''''' END;
 V_CALCULATED_LENGTH_COLUMN:=CASE WHEN V_CALCULATED_LENGTH_COLUMN!='' THEN V_CALCULATED_LENGTH_COLUMN ELSE '0' END;
 V_GIS_LENGTH_COLUMN:=CASE WHEN V_GIS_LENGTH_COLUMN!='' THEN V_GIS_LENGTH_COLUMN ELSE '0' END;
  V_SQLQUERY:='';
V_SQLQUERY:=V_SQLQUERY||' SELECT '''||V_LAYER_TITLE||''' AS ENTITY_TYPE,'''||V_GEOM_TYPE||''' AS GEOM_TYPE,
COALESCE(IVCM.ITEM_COST,0) as ITEM_COST,COALESCE(ITEM.SERVICE_COST_PER_UNIT,0) as SERVICE_COST_PER_UNIT,
ATT.SPECIFICATION,ATT.ITEM_CODE,COALESCE(IVCM.USER_ID,0) as USER_ID,ITEM.UNIT_MEASUREMENT AS UNIT_MEASUREMENT ,
UM.USER_NAME AS USER_NAME,
 0 AS TOTAL_COST_PLANNED,
 0 AS TOTAL_COST_ASBUILT,
 0 AS TOTAL_COST_DORMANT,
 SUM(CASE WHEN ATT.NETWORK_STATUS=''P'' THEN 1 ELSE 0 END) AS TOTAL_COUNT_PLANNED,
 SUM(CASE WHEN ATT.NETWORK_STATUS=''A'' THEN 1 ELSE 0 END) AS TOTAL_COUNT_ASBUILT,
 SUM(CASE WHEN ATT.NETWORK_STATUS=''D'' THEN 1 ELSE 0 END) AS TOTAL_COUNT_DORMANT';

 -- ADD CALCULATED AND GIS LENGTH COLUMNS FOR LINE ENTITY...
 V_SQLQUERY:=V_SQLQUERY||',COALESCE(SUM(CASE WHEN ATT.NETWORK_STATUS=''P'' THEN CAST('|| V_CALCULATED_LENGTH_COLUMN ||' AS DECIMAL(18,2)) ELSE 0 END),0) AS TOTAL_PLANNED_CALC_LENGTH,
 COALESCE(SUM(CASE WHEN ATT.NETWORK_STATUS=''A'' THEN CAST('|| V_CALCULATED_LENGTH_COLUMN ||' AS DECIMAL(18,2)) ELSE 0 END),0) AS TOTAL_ASBUILT_CALC_LENGTH,
 COALESCE(SUM(CASE WHEN ATT.NETWORK_STATUS=''D'' THEN CAST('|| V_CALCULATED_LENGTH_COLUMN ||' AS DECIMAL(18,2)) ELSE 0 END),0) AS TOTAL_DORMENT_CALC_LENGTH,
 COALESCE(SUM(CASE WHEN ATT.NETWORK_STATUS=''P'' THEN CAST('|| V_GIS_LENGTH_COLUMN ||' AS DECIMAL(18,2)) ELSE 0 END),0) AS TOTAL_PLANNED_GIS_LENGTH,
 COALESCE(SUM(CASE WHEN ATT.NETWORK_STATUS=''A'' THEN CAST('|| V_GIS_LENGTH_COLUMN ||' AS DECIMAL(18,2)) ELSE 0 END),0) AS TOTAL_ASBUILT_GIS_LENGTH,
 COALESCE(SUM(CASE WHEN ATT.NETWORK_STATUS=''D'' THEN CAST('|| V_GIS_LENGTH_COLUMN ||' AS DECIMAL(18,2)) ELSE 0 END),0) AS TOTAL_DORMENT_GIS_LENGTH';

V_SQLQUERY:=V_SQLQUERY||' FROM '||V_LAYER_TABLE||' ATT ';

 
V_SQLQUERY:=V_SQLQUERY||' LEFT JOIN ITEM_TEMPLATE_MASTER ITEM ON 
  UPPER(TRIM(ATT.ITEM_CODE))=UPPER(TRIM(ITEM.CODE)) AND ATT.specification=ITEM.specification AND ITEM.LAYER_ID='||V_LAYER_ID||'  
  Left JOIN item_vendor_cost_master IVCM on ITEM.specification=IVCM.specification and ITEM.code=IVCM.item_code
  Left JOIN user_master UM on IVCM.USER_ID=UM.USER_ID 
  where ATT.site_plan_id='||p_site_plan_id||'
GROUP BY 
  COALESCE(IVCM.ITEM_COST,0) ,COALESCE(ITEM.SERVICE_COST_PER_UNIT,0),ATT.SPECIFICATION,ATT.ITEM_CODE,IVCM.USER_ID,ITEM.UNIT_MEASUREMENT,UM.USER_NAME';
  

EXECUTE 'INSERT INTO Rec(ENTITY_TYPE,GEOM_TYPE,ITEM_COST,SERVICE_COST_PER_UNIT,SPECIFICATION,ITEM_CODE,
 USER_ID,UNIT_MEASUREMENT,USER_NAME,TOTAL_PLANNED_COST,TOTAL_ASBUILT_COST,
 TOTAL_DORMANT_COST,TOTAL_PLANNED_COUNT,TOTAL_ASBUILT_COUNT,TOTAL_DORMANT_COUNT,TOTAL_PLANNED_CALC_LENGTH,
 TOTAL_ASBUILT_CALC_LENGTH,
 TOTAL_DORMANT_CALC_LENGTH,
 TOTAL_PLANNED_GIS_LENGTH,TOTAL_ASBUILT_GIS_LENGTH,TOTAL_DORMANT_GIS_LENGTH) '||V_SQLQUERY||';';

--raise info 'FINAL QUERY : %',V_SQLQUERY; 
 END LOOP;
 close CURBOMBOQLAYERS;

--PERFORM fn_get_accessories_bom_boq_report_new(p_geom,p_network_status,p_radius,p_geom_type,p_provinceids,p_regionids, p_layerids, p_durationbasedon, p_fromdate, p_todate, p_userid, p_roleid, p_projectcodes);

 UPDATE Rec SET
 TOTAL_PLANNED_COST= CASE WHEN UPPER(GEOM_TYPE)='LINE' THEN COALESCE(ITEM_COST*TOTAL_PLANNED_CALC_LENGTH,0)
 ELSE COALESCE(ITEM_COST*TOTAL_PLANNED_COUNT,0) END,
 TOTAL_ASBUILT_COST= CASE WHEN UPPER(GEOM_TYPE)='LINE' THEN COALESCE(ITEM_COST*TOTAL_ASBUILT_CALC_LENGTH,0)
 ELSE COALESCE(ITEM_COST*TOTAL_ASBUILT_COUNT,0) END,
 TOTAL_DORMANT_COST= CASE WHEN UPPER(GEOM_TYPE)='LINE' THEN COALESCE(ITEM_COST*TOTAL_DORMANT_CALC_LENGTH,0)
 ELSE COALESCE(ITEM_COST*TOTAL_DORMANT_COUNT,0) END;
 
 UPDATE Rec SET
 TOTAL_COST=  COALESCE(TOTAL_PLANNED_COST,0)+COALESCE(TOTAL_ASBUILT_COST,0)+COALESCE(TOTAL_DORMANT_COST,0),
  TOTAL_QTY= CASE WHEN UPPER(GEOM_TYPE)='LINE' THEN COALESCE(TOTAL_PLANNED_CALC_LENGTH,0)+COALESCE(TOTAL_ASBUILT_CALC_LENGTH,0)+COALESCE(TOTAL_DORMANT_CALC_LENGTH,0)
 ELSE COALESCE(TOTAL_PLANNED_COUNT,0)+COALESCE(TOTAL_ASBUILT_COUNT,0)+COALESCE(TOTAL_DORMANT_COUNT,0) END;
 
 

 --V_SqlFinalQuery:='SELECT specification,ITEM_CODE as code,TOTAL_COST as cost_per_unit,TOTAL_QTY as totalqty,USER_ID,UNIT_MEASUREMENT,USER_NAME FROM Rec';
V_SqlFinalQuery := '
    WITH CTE AS (
        SELECT 
            specification, 
            ITEM_CODE AS code, 
            TOTAL_COST AS cost_per_unit, 
            TOTAL_QTY AS totalqty, 
            USER_ID, 
            UNIT_MEASUREMENT, 
            USER_NAME,
            COUNT(*) OVER() AS totalRecord,
            ROW_NUMBER() OVER (ORDER BY TOTAL_COST DESC) AS S_No
        FROM Rec
        WHERE 1=1';

-- Apply search filter
IF (upper(searchText) != '' AND searchby != '') THEN
if(searchby='code')then
    V_SqlFinalQuery := V_SqlFinalQuery || ' 
        AND  
		lower(item_' || quote_ident($3) || ') LIKE lower(''%' || $4 || '%'')';
		else
		 V_SqlFinalQuery := V_SqlFinalQuery || ' 
        AND  
		lower(' || quote_ident($3) || ') LIKE lower(''%' || $4 || '%'')';
		end if;
END IF;

V_SqlFinalQuery := V_SqlFinalQuery || ' )';

-- Apply pagination
IF (P_PAGENO <> 0) THEN
    StartSNo := P_PAGERECORD * (P_PAGENO - 1) + 1;
    EndSNo := P_PAGENO * P_PAGERECORD;

    V_SqlFinalQuery := V_SqlFinalQuery || '
        SELECT * FROM CTE WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo;
ELSE
    V_SqlFinalQuery := V_SqlFinalQuery || '
        SELECT * FROM CTE ORDER BY code DESC';
END IF;

raise info 'FINAL QUERY11 : %',V_SqlFinalQuery;
RETURN QUERY EXECUTE 'select row_to_json(row) from ('||V_SqlFinalQuery||')  row';

end;
$BODY$;

ALTER FUNCTION public.fn_get_site_bom_boq_report_new(character varying, integer, character varying, character varying, integer, integer, character varying, character varying, integer)
    OWNER TO postgres;
------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_combine_geom(integer)

-- DROP FUNCTION IF EXISTS public.fn_get_combine_geom(integer);

CREATE OR REPLACE FUNCTION public.fn_get_combine_geom(
	p_siteplanid integer)
    RETURNS TABLE(combine_geom character varying, network_id character varying, region_id integer, province_id integer, system_id integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

Declare sql TEXT;
v_combined_geom character varying;
v_network_id character varying;
v_region_id integer;
v_province_id integer;
v_system_id integer;
v_status bool;
BEGIN
	
	If(p_siteplanid >0  )THEN
-- 	SELECT 
--        string_agg(
--            ROUND(CAST(ST_X(geom) AS NUMERIC), 10) || ' ' || ROUND(CAST(ST_Y(geom) AS NUMERIC), 10), 
--            ','
--        ) AS combined_geom into v_combined_geom
-- 		FROM (
-- 			SELECT (ST_DumpPoints(ST_GeomFromWKB(sp_geometry))).geom AS geom
-- 			FROM line_master l
-- 			WHERE  l.system_id in(select a.system_id from att_details_cable a where site_plan_id=p_siteplanid)
-- 		) AS points;

WITH points AS (
    -- Extract points from the geometry
    SELECT (ST_DumpPoints(ST_GeomFromWKB(sp_geometry))).geom AS geom
    FROM line_master l  into v_combined_geom
    WHERE l.system_id IN (
        SELECT a.system_id FROM att_details_cable a WHERE site_plan_id = p_siteplanid
    )
),
formatted_points AS (
    -- Extract X and Y as separate columns
    SELECT 
        ROUND(CAST(ST_X(geom) AS NUMERIC), 10) AS x_coord,
        ROUND(CAST(ST_Y(geom) AS NUMERIC), 10) AS y_coord
    FROM points
),
first_point AS (
    -- Get the first point
    SELECT x_coord, y_coord FROM formatted_points LIMIT 1
)
SELECT 
    -- Concatenate all coordinates and append the first coordinate at the end
    string_agg(x_coord || ' ' || y_coord, ',') || ',' || 
    (SELECT x_coord || ' ' || y_coord FROM first_point)
    AS closed_combined_geom
FROM formatted_points;

	
	select p.network_id,p.region_id,p.province_id,p.system_id from att_details_pod p where site_plan_id=p_siteplanid 
	into v_network_id,v_region_id,v_province_id,v_system_id;
	end if;
	RETURN QUERY
    select v_combined_geom::character varying as combine_geom,v_network_id as network_id,v_region_id as region_id, v_province_id as province_id,v_system_id as system_id;	
END ;

$BODY$;

ALTER FUNCTION public.fn_get_combine_geom(integer)
    OWNER TO postgres;
-----------------------------------------------------------------------------------------------------------------------------------------

