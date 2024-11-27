INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'ddlDocumentType', 'doc1', true, 1, '2024-06-12 10:26:14.839', 1, '2024-06-12 10:26:14.839', 'doc1', NULL, 'document_type', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'ddlDocumentType', 'doc2', true, 1, '2024-06-12 10:26:14.839', 1, '2024-06-12 10:26:14.839', 'doc2', NULL, 'document_type', true, true, 0);
-----------------------------------------------------------------------------------------------
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( (select layer_id from layer_details where layer_name='FiberLink'), 'FiberLinkPrefix', 'TX', true, 1, now(), 1, now(), 'TX', NULL, 'link_prefix', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='FiberLink'), 'FiberLinkPrefix', 'CX', true, 1, now(), 1, now(), 'CX', NULL, 'link_prefix', true, true, 0);
INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES((select layer_id from layer_details where layer_name='FiberLink'), 'FiberLinkPrefix', 'IP', true, 1, now(), 1, now(), 'IP', NULL, 'link_prefix', true, true, 0);

alter table att_details_fiber_link 
add column link_prefix character varying;

alter table audit_att_details_fiber_link 
add column link_prefix character varying;

update dropdown_master set layer_id=(select layer_id from layer_details where layer_name='FiberLink') where dropdown_type='FiberLinkPrefix'


UPDATE att_details_fiber_link SET link_prefix='TX00000000' WHERE SYSTEM_ID=57;
UPDATE att_details_fiber_link SET link_prefix='CX00000000' WHERE SYSTEM_ID=55;
UPDATE att_details_fiber_link SET link_prefix='IP00000000' WHERE SYSTEM_ID=58;
--------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_dropdownlist(character varying, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_dropdownlist(character varying, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_dropdownlist(
	entitytype character varying,
	dropdowntype character varying)
    RETURNS TABLE(dropdown_type character varying, dropdown_value character varying, dropdown_key character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

 DECLARE
    sql TEXT;
BEGIN

IF(entitytype != '' and dropdownType != '' and dropdownType ='splitter_ratio' ) THEN

	sql:= 'select dropdown_type,dropdown_value,dropdown_key from (select SPLIT_PART(dd.dropdown_value,'':'', 1)::integer as input,SPLIT_PART(dd.dropdown_value,'':'', 2)::integer as output, dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id where UPPER(l.layer_name)= upper('''||entitytype||''') 
	and UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
	and lower(dd.dropdown_type) not like ''remark_%'' order by dd.dropdown_value)t order by t.input,t.output' ;
elsIF(dropdownType = 'FiberLinkPrefix') THEN

 sql:= 'select dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd where UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
  and lower(dd.dropdown_type) not like ''remark_%''  and dd.is_active=true order by dd.id';


elsIF(entitytype != '' and dropdownType != '' and upper(entitytype) !=upper('entity_type')) THEN

	sql:= 'select dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id where UPPER(l.layer_name)= upper('''||entitytype||''') and UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
	and lower(dd.dropdown_type) not like ''remark_%'' and dd.is_active=true order by dd.id';

	
elsIF(entitytype != '' and dropdownType != '' and upper(entitytype) !=upper('landbase')) THEN

	sql:= 'select dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id where UPPER(l.layer_name)= upper('''||entitytype||''') and UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
	and lower(dd.dropdown_type) not like ''remark_%'' order by dd.id';

elsIF(entitytype != '' and upper(entitytype) !=upper('landbase')) THEN  
     -- sql:= 'SELECT dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id
--       WHERE UPPER(l.layer_name)= upper('''||entitytype||''') and lower(dd.dropdown_type) not like ''remark_%'' order by dd.id';
      sql:= 'SELECT dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd inner join layer_details l on dd.layer_id=l.layer_id
      WHERE UPPER(l.layer_name)= upper('''||entitytype||''') and lower(dd.dropdown_type) not like ''remark_%'' order by dd.id';




elsIF(dropdownType != '' and upper(entitytype) !=upper('landbase')) THEN

 sql:= 'select dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd where UPPER(dd.dropdown_type)= upper('''||dropdownType||''')
  and lower(dd.dropdown_type) not like ''remark_%'' and layer_id=(select coalesce((select COALESCE(layer_id,0) from layer_details where Upper(layer_name)=upper('''||entitytype||''')),0)) and dd.is_active=true order by dd.id';

 elseIF(upper(entitytype)=upper('landbase'))THEN
 sql:='SELECT dd.type as dropdown_type,dd.value as dropdown_value,dd.value as dropdown_key FROM landbase_dropdown_master dd
	UNION
	SELECT  ''Layer Type'' as dropdown_type, dd.layer_name as dropdown_value,dd.layer_name as dropdown_key  FROM landbase_layer_master dd  ' ;

elseIF(coalesce(entitytype,'')='') THEN
sql:= 'SELECT dropdown_type,dropdown_value,dropdown_key FROM dropdown_master where layer_id=0 and is_active=true';
end if; 
raise info 'sql:%',sql;

RETURN QUERY EXECUTE sql;	
  END; 
$BODY$;

ALTER FUNCTION public.fn_get_dropdownlist(character varying, character varying)
    OWNER TO postgres;
------------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_fiber_link_prefix(character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_fiber_link_prefix(character varying);

CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_prefix(
	p_link_prefix character varying)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    sql TEXT;
BEGIN
    -- Construct the dynamic SQL
    sql := '
    SELECT 
        ' || quote_literal(p_link_prefix) || ' || LPAD((SUBSTRING(MAX(link_ID), ' || (LENGTH(p_link_prefix) + 1) || ')::INTEGER + 1)::TEXT, 8, ''0'') AS link_prefix
    FROM 
        att_details_fiber_link
    WHERE 
        link_ID ILIKE ' || quote_literal(p_link_prefix || '%') || ' 
    ';

    -- Debugging Information
    RAISE INFO '%', sql;

    -- Execute the query and return the results
    RETURN QUERY EXECUTE 'SELECT row_to_json(row) FROM (' || sql || ') row';

END;
$BODY$;

ALTER FUNCTION public.fn_get_fiber_link_prefix(character varying)
    OWNER TO postgres;
-------------------------------------------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_validate_linkids(character varying)

-- DROP FUNCTION IF EXISTS public.fn_validate_linkids(character varying);

CREATE OR REPLACE FUNCTION public.fn_validate_linkids(
    p_link_ids character varying
)
RETURNS SETOF json
LANGUAGE 'plpgsql'
COST 100
VOLATILE PARALLEL UNSAFE
ROWS 1000
AS $BODY$
DECLARE
    sql TEXT;
BEGIN
    -- Construct the dynamic SQL query
    sql := 'WITH temp_ids AS (
        SELECT unnest(string_to_array(''' || p_link_ids || ''', '','')) AS link_id
    )
    SELECT string_agg(temp_ids.link_id, '','') AS invalidLinkIds  -- Aggregate invalid ids into a comma-separated list
    FROM temp_ids
    LEFT JOIN att_details_fiber_link ON temp_ids.link_id = att_details_fiber_link.link_id
    WHERE att_details_fiber_link.link_id IS NULL';
    RAISE INFO 'Executing SQL: %', sql;
    RETURN QUERY EXECUTE 'SELECT row_to_json(row) FROM (' || sql || ') row';
END;
$BODY$;
ALTER FUNCTION public.fn_validate_linkids(character varying)
    OWNER TO postgres;
