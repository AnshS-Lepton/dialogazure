CREATE OR REPLACE FUNCTION public.fn_get_dependent_child_elements(
	p_systemid integer,
	p_entitytype character varying,
	p_geomtype character varying,
	p_impacttype character varying)
    RETURNS TABLE(entity_type text, network_id character varying, display_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


DECLARE

    sql TEXT;

    v_building_system_id INTEGER;

    v_building_code CHARACTER VARYING(90);

    v_arow record;

    v_is_shifting_allowed boolean;

    v_associated_entity_type CHARACTER VARYING;

    V_IS_ASSOCIATED bool;

    v_assc_layer_table CHARACTER VARYING;

    v_assc_geom_type CHARACTER VARYING;

BEGIN

V_IS_ASSOCIATED:=false;

sql:= '';

 

----REGION LOCATION UPDATION FOR Associated Entity Condition By ANTRA  

select case when count(1)>0 then  true else false end into V_IS_ASSOCIATED

from associate_entity_info where ((entity_system_id=p_systemid and upper(associate_entity_info.entity_type)=upper(p_entitytype))

or (associated_system_id=p_systemid and upper(associated_entity_type)=upper(p_entitytype))) and is_termination_point=false;

 

raise info 'V_IS_ASSOCIATED% ',V_IS_ASSOCIATED;

 

if exists(select 1 from layer_details where upper(layer_name)=upper(p_entitytype) and ne_class_name='SPLICE_CLOSURE')

then

V_IS_ASSOCIATED:=false;

end if;

 

IF (V_IS_ASSOCIATED) THEN

 

                                FOR v_arow IN SELECT distinct associated_entity_type,associated_network_id,associated_display_name,associated_system_id from associate_entity_info aei

                                WHERE entity_system_id=p_systemid and upper(aei.entity_type)=upper(p_entitytype) and is_termination_point=false

                                UNION

                                SELECT distinct aei.entity_type,aei.entity_network_id,aei.entity_display_name,aei.entity_system_id from associate_entity_info aei

                                WHERE aei.associated_system_id=p_systemid and upper(aei.associated_entity_type)=upper(p_entitytype) and aei.is_termination_point=false order by associated_entity_type

 

                               

                               

                                LOOP

                                               

                                if(sql!='') then

                                sql:=sql||' union ';

                                end if;

                               

--                             raise info 'associated_entity_type% ',v_arow.associated_entity_type;

--                             raise info 'associated_network_id% ',v_arow.associated_network_id;

                               raise info 'associated_display_name% ',v_arow.associated_display_name;

                               

                                select layer_table,geom_type into v_assc_layer_table,v_assc_geom_type from layer_details where upper(layer_name)=upper(v_arow.associated_entity_type);                              

                                raise info 'v_assc_geom_type% ',v_assc_geom_type;                              

                               

                       --  sql:=sql||'select '''||v_arow.associated_entity_type||'''::text as entity_type,'''||v_arow.associated_network_id||'''::character varying as network_id,

--                             CASE WHEN (aei.associated_display_name !='''') THEN aei.associated_display_name ELSE (select display_name from '||v_assc_geom_type||'_master

--                             where system_id='||v_arow.associated_system_id||' and entity_type='''||v_arow.associated_entity_type||''') END from associate_entity_info aei

--                             where aei.entity_system_id='||p_systemid||' and upper(aei.entity_type)=upper('''||p_entitytype||''') and aei.associated_system_id='||v_arow.associated_system_id||'';

 

                                sql:=sql||'select associated_entity_type as entity_type,associated_network_id as network_id,associated_system_id as system_id, case when coalesce(aei.associated_display_name,'''')!=''''

                                 then aei.associated_display_name else (select common_name from '||v_assc_geom_type||'_master
                                 where system_id='||v_arow.associated_system_id||' and entity_type='''||v_arow.associated_entity_type||''' ) end as display_name from associate_entity_info aei
                                where (aei.entity_system_id='||p_systemid||' and upper(aei.entity_type)=upper('''||p_entitytype||''')
                                and aei.associated_system_id='||v_arow.associated_system_id||' and aei.associated_entity_type='''||v_arow.associated_entity_type||''')
                                union
                                select entity_type ,entity_network_id,entity_system_id, case when coalesce(aei.entity_display_name,'''')!=''''
                                 then aei.entity_display_name else (select common_name from '||v_assc_geom_type||'_master
                                 where system_id='||v_arow.associated_system_id||' and entity_type='''||v_arow.associated_entity_type||''' ) end as display_name from associate_entity_info aei
                                where (aei.associated_system_id='||p_systemid||' and upper(aei.associated_entity_type)=upper('''||p_entitytype||''')
                                and aei.entity_system_id='||v_arow.associated_system_id||' and aei.entity_type='''||v_arow.associated_entity_type||''')'; 

				if(p_entitytype='Trench')
				then
					 sql:=sql||' union select associated_entity_type as entity_type,associated_network_id as network_id,associated_system_id as system_id, case when coalesce(aei.associated_display_name,'''')!=''''
then aei.associated_display_name else (select common_name from Line_master
where system_id='||v_arow.associated_system_id||' and entity_type='''||v_arow.associated_entity_type||''' ) end as display_name from associate_entity_info aei
where (aei.entity_system_id='||v_arow.associated_system_id||' and upper(aei.entity_type)=upper('''||v_arow.associated_entity_type||'''))';
				end if;
				
                                raise info 'sql:-%',sql; 

                END LOOP;

 

                sql:='select ld.layer_title::text,tbl.network_id,tbl.display_name from('||sql||')tbl

inner join layer_details ld on upper(ld.layer_name)=upper(tbl.entity_type)'; 

 

ELSIF (V_IS_ASSOCIATED = false) THEN

if exists(select 1 from vw_layer_mapping where upper(parent_layer_name)=upper(p_entitytype) and coalesce(child_geom_type,'')!='')

then

for v_arow in select * from vw_layer_mapping where upper(parent_layer_name)=upper(p_entitytype) and coalesce(child_geom_type,'')!=''

loop

                if(sql!='')

                then

                                sql:=sql||' union ';

                end if;

 

                sql:=sql||' select '''||v_arow.child_layer_name||'''::text as entity_type, ent.network_id,pm.display_name from '||v_arow.child_layer_table||' ent

                inner join '||v_arow.child_geom_type||'_master pm on pm.system_id=ent.system_id and upper(pm.entity_type) = upper('''||v_arow.child_layer_name||''')

                where ent.parent_system_id='||p_systemid||' and upper(ent.parent_entity_type)=upper('''||p_entitytype||''')';

                                    

end loop; 

sql:='select ld.layer_title::text,tbl.network_id,tbl.display_name from('||sql||')tbl

inner join layer_details ld on upper(ld.layer_name)=upper(tbl.entity_type)'; 

else

sql:='select ''''::text,''''::character varying as network_id,''''::character varying as display_name where 1=2 ';

end if;

end if;

raise info '%',sql;

 

                RETURN QUERY  EXECUTE sql;

END;

$BODY$;

ALTER FUNCTION public.fn_get_dependent_child_elements(integer, character varying, character varying, character varying)
    OWNER TO postgres;