-- FUNCTION: public.fn_get_child_clone_list(character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_get_child_clone_list(character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_get_child_clone_list(
	p_entity_name character varying,
	p_system_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
AROW record;
 sql text='';
BEGIN

IF EXISTS(SELECT 1 FROM VW_LAYER_MAPPING WHERE UPPER(PARENT_LAYER_NAME)=UPPER(P_ENTITY_NAME) AND UPPER(CHILD_LAYER_NAME)!='EQUIPMENT')
THEN
FOR AROW IN select child_layer_table,child_layer_name from vw_layer_mapping where upper(parent_layer_name)=upper(p_entity_name) and upper(child_layer_name)!='EQUIPMENT'
LOOP
	IF(sql!='')
	THEN
		sql:=sql||' UNION ';
	END IF; 
	
	sql:=sql||'select network_id,pm.display_name,true is_child_entity,false is_associated_entity,'''||AROW.child_layer_name||''' as entity_type ,ent.system_id ,ld.layer_title
	from '||AROW.child_layer_table||' as ent
	inner join  point_master pm on ent.system_id=pm.system_id and upper(pm.entity_type)=upper('''||AROW.child_layer_name||''') 
	inner join layer_details ld on upper(ld.layer_name)=upper('''||AROW.child_layer_name||''')
	where parent_system_id='||p_system_id||' and upper(parent_entity_type)='''||upper(p_entity_name)||'''';	
	
END LOOP; 

sql:='SELECT A.*,COALESCE(ASS.parent_system_id,0)>0 AS IS_ACCESSORIES_PLACED from ('||sql||')A LEFT JOIN ATT_DETAILS_ACCESSORIES ASS ON ASS.PARENT_SYSTEM_ID=A.SYSTEM_ID 
AND UPPER(PARENT_ENTITY_TYPE)=UPPER(A.ENTITY_TYPE)
group by a.entity_type,a.network_id,a.display_name,a.is_child_entity,a.is_associated_entity,a.system_id,a.layer_title,ass.parent_system_id ';

RAISE INFO 'sql-1 %', sql;	

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||'order by layer_title asc) row';

ELSE

sql:=sql||' UNION select associated_network_id as network_id,associated_display_name as display_name,false is_child_entity,true is_associated_entity, associated_entity_type ,associated_system_id as system_id,ld.layer_title
from associate_entity_info
inner join layer_details ld on upper(ld.layer_name)=upper(associated_entity_type) and upper(ld.geom_type)=upper(''Point'') where entity_system_id='||p_system_id||' and upper(entity_type)='''||upper(p_entity_name)||'''
union
select entity_network_id as network_id,entity_display_name as display_name,false is_child_entity,true is_associated_entity,entity_type as entity_type,entity_system_id as system_id,ld.layer_title
from associate_entity_info 
inner join layer_details ld on upper(ld.layer_name)=upper(entity_type) and upper(ld.geom_type)=upper(''Point'')  where (associated_system_id='||p_system_id||' and upper(associated_entity_type)='''||upper(p_entity_name)||''')';

sql:='SELECT A.*,COALESCE(ASS.parent_system_id,0)>0 AS IS_ACCESSORIES_PLACED from ('||sql||')A LEFT JOIN ATT_DETAILS_ACCESSORIES ASS ON ASS.PARENT_SYSTEM_ID=A.SYSTEM_ID 
AND UPPER(PARENT_ENTITY_TYPE)=UPPER(A.ENTITY_TYPE)
group by a.entity_type,a.network_id,a.display_name,a.is_child_entity,a.is_associated_entity,a.system_id,a.layer_title,ass.parent_system_id ';

RAISE INFO 'sql-3 %', sql;	

	

RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||'order by layer_title asc) row';
end if;	
END;
$BODY$;

ALTER FUNCTION public.fn_get_child_clone_list(character varying, integer)
    OWNER TO postgres;