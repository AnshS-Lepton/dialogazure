CREATE OR REPLACE FUNCTION public.fn_get_rootid_list(
   p_userId int, p_geom text,p_selectiontype character varying,p_radius double precision,p_network_status character varying)
RETURNS SETOF json 
LANGUAGE 'plpgsql'
COST 100
VOLATILE PARALLEL UNSAFE
ROWS 1000
AS $BODY$
DECLARE 
    v_Query text;
    provinceid TEXT;
BEGIN
    CREATE temp TABLE Temp_Entity_by_geom
            		   (
            		      system_id integer,
            		      entity_type VARCHAR(80),
            		      network_status VARCHAR(80),
            		      network_id varchar(100)
            		   ) on commit  drop ;
    
	
	
	
 EXECUTE   'insert into Temp_Entity_by_geom(system_id,entity_type,network_status,network_id) 
select system_id,entity_type,network_status,network_id from fn_get_bulk_entity_by_geom
( '''||p_geom||''', '||p_userId||', '''||p_selectiontype||''', '||p_radius||',''Cable'','''||p_network_status||''' )
where created_by in('||p_userId||')';
	
    -- Constructing and returning JSON
    RETURN QUERY 
    SELECT row_to_json(row) 
    FROM (
        SELECT LM.system_id AS value, 
               CONCAT(LM.route_id, '-', '(', LM.NETWORK_ID, ')') AS key 
        FROM att_details_cable LM 
         inner join Temp_Entity_by_geom b on b.system_id=LM.system_id
    ) row;
END
$BODY$;
