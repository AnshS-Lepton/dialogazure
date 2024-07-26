-- FUNCTION: public.fn_get_audit_log_details(text, integer, text)

-- DROP FUNCTION IF EXISTS public.fn_get_audit_log_details(text, integer, text);

CREATE OR REPLACE FUNCTION public.fn_get_audit_log_details(
	dynamic_table_name text,
	dynamic_system_id integer,
	dynamic_category_name text)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$

DECLARE 
    v_column_name text; 
    query text := '';
    query_part text;
BEGIN
    
    
    FOR v_column_name IN 
        SELECT column_name 
        FROM information_schema.columns 
        WHERE table_name = dynamic_table_name 
        AND table_schema = 'public' 
         AND column_name NOT IN('modified_on', 'modified_by', 'created_by', 'created_on','audit_id','system_id')
    LOOP        
        query_part := '
            INSERT INTO temp_comparisons (user_name, action, log_table_name, category, system_id,network_id, colname, oldvalue, newvalue, modified_on)
            SELECT
                (SELECT user_name FROM user_master WHERE user_id = t1.created_by::integer) AS user_name,
                CASE WHEN COALESCE(t1.action, '''') = ''U'' THEN ''UPDATE'' ELSE ''CREATE'' END AS action,
                ''' || dynamic_table_name || ''' AS log_table_name,
                '''||dynamic_category_name||''' as category,
                t1.system_id::text AS system_id,
                COALESCE(t1.network_id, '''') AS network_id,
                ''' || v_column_name || ''' AS colname, 
                COALESCE(t1."' || v_column_name || '"::text, '''') AS oldvalue,
                COALESCE(t2."' || v_column_name || '"::text, '''') AS newvalue,		        
                TO_CHAR(t1.modified_on, ''DD-MM-YYYY HH24:MI'') AS modified_on
            FROM 
                (SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS row_num 
                 FROM ' || dynamic_table_name || ' a
inner join temp_all_entity b on a.system_id=b.entity_id) t1
            FULL OUTER JOIN 
                (SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS row_num 
                 FROM ' || dynamic_table_name || ' a inner join temp_all_entity b on a.system_id=b.entity_id) t2 
            ON t1.row_num = t2.row_num - 1 
            WHERE 
                ( t1."' || v_column_name || '" <> t2."' || v_column_name || '");';
        
        query := query || query_part || ' ';
    END LOOP;

    EXECUTE query;
    --RETURN QUERY SELECT * FROM temp_comparisons tc WHERE tc.system_id IS NOT NULL AND tc.oldvalue <> ''; -- AND tc.newvalue <> '';
END 
$BODY$;

ALTER FUNCTION public.fn_get_audit_log_details(text, integer, text)
    OWNER TO postgres;

-- select * from fn_get_audit_log_report_allexcel('','','','0','','Cable','','','','','Created_On','','','',0,0,'','','',5,2,'','','',0.0,'');

