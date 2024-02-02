CREATE OR REPLACE FUNCTION public.fn_updatelabelvalue_map(
	entity_id character varying,
	label_column character varying)
    RETURNS TABLE(status character varying, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

	DECLARE
	from_position int;
	last_comma_position int;	
	before_from_query text;
	old_label_part text;
	new_label_part text;
	view_query text;
	view_name text;
	entity_type text;
	v_layer_table character varying;
	v_last_round_position int;
	

BEGIN

	select layer_name,layer_table into entity_type,v_layer_table from layer_details where lower(layer_id::character varying) = lower(''||entity_id||'');
		
	--raise info 'entity_type:%',entity_type;

	if not exists(select 1 from layer_details where upper(layer_name)=upper(entity_type) and is_label_change_allowed=true)
	then
	RAISE NOTICE 'This is a notice message';
		RETURN QUERY
		SELECT 'Falied'::character varying as status, ('Label change does not allowed for '||entity_type)::character varying as message;		
	else

	IF EXISTS(SELECT TRUE from INFORMATION_SCHEMA.VIEWS where lower(table_name)=lower('vw_att_details_'||entity_type||'_map'))
	THEN
		--VIEW NAME
	RAISE NOTICE 'This is a notice message11';
		view_name:='vw_att_details_'||entity_type||'_map';
		--raise info 'view_name:%',view_name;
		--GET VIEW DEFINATION
		select view_definition into view_query from INFORMATION_SCHEMA.views where upper(table_name) = upper(view_name);
		
		--raise info 'view_query:%',view_query;
		--PREPARE LABEL PART	
		--Comment the below code for new_label_part , and add condition for additional attributes	
		--new_label_part:=  entity_type||'.'||label_column||' AS label_column ';
		if not exists(select 1 from information_schema.columns WHERE upper(table_name) = upper(v_layer_table) AND upper(table_schema) = 'PUBLIC' 
		and upper(column_name)=upper(label_column))then
			new_label_part:=  entity_type||'.other_info::json ->>'''||label_column||''' AS label_column ';
			raise info 'label_column json:%',new_label_part;
		else

		     if exists(select 1 from information_schema.columns WHERE upper(table_name) = upper(v_layer_table) AND upper(table_schema) = 'PUBLIC' and
		      upper(column_name)=upper(label_column) and upper(data_type)='DOUBLE PRECISION')then

		      raise info'label_column1--%',label_column;

			new_label_part:=  'round('||entity_type||'.'||label_column||'::numeric,2) AS label_column '; 
		     else
			raise info'%label_column2222--',label_column;
		     new_label_part:=  entity_type||'.'||label_column||' AS label_column ';
		     end if;
			raise info 'label_column3 :%',new_label_part;
		end if;

		-- GET FROM POSITION  FROM VIEW DEFINATION
		SELECT   POSITION('FROM' in upper(view_query)) into from_position;

		raise info 'fromposition:%',from_position;
		-- GET BEFORE FROM STRING..
		SELECT Substring(view_query,1,from_position-1) into before_from_query;

		--raise info 'beforeFromString:%',before_from_query;
		--raise info 'beforeFromString:-----%',reverse(before_from_query);
		
		--GET COMMAN POSITION (LAST COMMA JUST BEFORE FROM KEYWORD
		if exists(select 1 from information_schema.columns WHERE upper(table_name) = upper(v_layer_table) AND upper(table_schema) = 'PUBLIC' and
		      upper(column_name) ilike upper('round%'))then
		      raise info 'XXXX--------%','111';
		select position('dnuor' in reverse(before_from_query)) +5 into v_last_round_position;
		
		raise info'last_round_position%----',v_last_round_position;

		 ELSE
		
		Select length(before_from_query) - position(',' in reverse(before_from_query)) + 1 into last_comma_position;

		END IF;  
		--else
		--Select length(before_from_query) - position(',' in reverse(before_from_query)) + 1 into last_comma_position;
		--end if;

		raise info 'commaposition:%',last_comma_position;
		
		--GET OLD LABEL PART FROM VIEW DEFINATION
		select substring(view_query,last_comma_position+1,(from_position-1)-last_comma_position) into old_label_part;

		raise info 'length:%',LENGTH(old_label_part);

		-- if(LENGTH(old_label_part) <> 0)
-- 		then
-- 		RETURN QUERY
-- 		SELECT 'failed'::character varying as status, 'label not found'::character varying as message;
-- 		end if;
		
		raise info 'queryApk1:%',view_query;
		raise info 'old_label_part:--------%',old_label_part;
		raise info 'new_label_part:%',new_label_part;
		
		-- REPLACE OLD LABEL PART WITH NEW LABEL PART
		--commented by pk--select replace (view_query,old_label_part,new_label_part) into view_query;
		
		--implemented by pk-- REPLACE OLD LABEL PART WITH NEW LABEL PART
        SELECT replace(view_query, quote_literal(old_label_part), quote_literal(new_label_part)) INTO view_query;

        raise info 'old_label_partpk1:%',old_label_part;
		raise info 'new_label_partpk2:%',new_label_part;


		raise info 'finalQuerypkkk:%',view_query;
		-- DROP EXISTING VIEW

		

		EXECUTE('DROP VIEW '||UPPER(view_name));
		
		--CREATE VIEW AGAIN WITH UPDATED DEFINATION
		
  		EXECUTE('CREATE OR REPLACE VIEW '||UPPER(view_name)||' AS '||view_query);	

		-- insertquery = 'insert into label_settings(layer_id, label_columns, created_by)values ('||entity_id||', '''||label_column||''', '''||user_id||''')';
-- 
-- 		raise info 'finalQuery:%',insertquery;
-- 
-- 		EXECUTE(insertquery);

		RETURN QUERY
		SELECT 'ok'::character varying as status, 'successfully'::character varying as message;

		
		
	
ELSE
RETURN QUERY
SELECT 'Falied'::character varying as status, 'View Not found'::character varying as message;		
END IF;	
   END IF;	
 exception 
 when others then 
 RETURN QUERY
  SELECT 'Exception'::character varying as status, SQLERRM ::character varying as message;
  
END;
$BODY$;

ALTER FUNCTION public.fn_updatelabelvalue_map(character varying, character varying)
    OWNER TO postgres;