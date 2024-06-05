-- FUNCTION: public.fn_delete_entity(integer, character varying, character varying, integer)

-- DROP FUNCTION IF EXISTS public.fn_delete_entity(integer, character varying, character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_delete_entity(
	p_system_id integer,
	p_entity_type character varying,
	p_geom_type character varying,
	p_user_id integer)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$



DECLARE 
v_layer_table character varying;
v_is_parent_exist boolean;
V_IS_VALID BOOLEAN;
V_MESSAGE CHARACTER VARYING;
V_LAYER_TITLE character varying;
v_parent_system_id integer;
v_building_system_id integer;
v_building_code character varying;
v_network_id character varying;
v_target_ref_id integer;

BEGIN
--GET THE LAYER TITLE
SELECT layer_title into V_LAYER_TITLE FROM layer_details where upper(layer_name)=upper(p_entity_type);

V_IS_VALID:=TRUE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_065]');--(V_LAYER_TITLE||' has deleted successfully!');
--CHECK THE STRUCTURE CHILD EXIST
if(upper(p_entity_type)='STRUCTURE' and (select count(1) from isp_entity_mapping where structure_id=p_system_id)>0)
then 
V_IS_VALID=FALSE;
V_MESSAGE:=('[SI_GBL_GBL_GBL_GBL_051]');--Structure can not be deleted as there are some dependent elements! 

--CHECK THE SPLICING(EXCLUDING THE INERNAL CONNECTIVITY OF THE DEVICE)
elsIF EXISTS(select 1 from connection_info 
where ((source_system_id=p_system_id and upper(source_entity_type)=upper(p_entity_type)) or ( destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type)))
and ( ((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))))
THEN 
v_is_parent_exist=false;
V_IS_VALID=FALSE;   

IF EXISTS(SELECT 1 FROM CONNECTION_INFO where ((source_system_id=p_system_id and upper(source_entity_type)=upper(p_entity_type)) or ( destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type))) )
THEN 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!');       
END IF;

IF(UPPER(p_entity_type)='SPLITTER') 
AND EXISTS(SELECT 1 FROM CONNECTION_INFO CON JOIN ATT_DETAILS_SPLITTER SPL 
ON  SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE') 
OR (CON.DESTINATION_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.DESTINATION_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.SOURCE_ENTITY_TYPE)='CABLE')) )
THEN
v_is_parent_exist=true; 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 
        ELSIF(UPPER(p_entity_type)='SPLITTER')THEN 
v_is_parent_exist=TRUE;
END IF;
        
IF(UPPER(p_entity_type)='SPLICECLOSURE') 
AND EXISTS(SELECT 1 FROM CONNECTION_INFO CON JOIN ATT_DETAILS_SPLICECLOSURE SPL 
ON SPL.SYSTEM_ID=p_system_id and ((CON.SOURCE_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.SOURCE_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.DESTINATION_ENTITY_TYPE)='CABLE') 
OR (CON.DESTINATION_SYSTEM_ID=SPL.PARENT_SYSTEM_ID AND UPPER(CON.DESTINATION_ENTITY_TYPE)=UPPER(SPL.PARENT_ENTITY_TYPE) AND UPPER(CON.SOURCE_ENTITY_TYPE)='CABLE')))
THEN 
        v_is_parent_exist=true;
        V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 

ELSIF((SELECT PARENT_SYSTEM_ID FROM ATT_DETAILS_SPLICECLOSURE SPL WHERE SPL.SYSTEM_ID=p_system_id and UPPER(p_entity_type)='SPLICECLOSURE')!=0)THEN 
v_is_parent_exist=true;
END IF;

IF(v_is_parent_exist=false)
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_052]');--(V_LAYER_TITLE||' can not be deleted as it is spliced with some entity!'); 

end if; 
--CHECK THE ROUTE ENTITY ASSOCIATION FIRST
elsif exists(select 1 from  ASSOCIATE_ROUTE_INFO asso where asso.entity_id=p_system_id and upper(entity_type)=upper(p_entity_type))
then 
delete from ASSOCIATE_ROUTE_INFO where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);

-- elsif exists(select 1 from  ASSOCIATE_ROUTE_INFO asso where asso.entity_id=p_system_id and upper(entity_type)=upper(p_entity_type))
-- then 
-- V_IS_VALID=FALSE;
-- V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as route is associated with some entity!');


--CHECK THE ENTITY ASSOCIATION FIRST
elsif exists(select 1 from  associate_entity_info asso where ((asso.associated_system_id=p_system_id and upper(asso.associated_entity_type)=UPPER(p_entity_type)) or 
(asso.entity_system_id=p_system_id and upper(asso.entity_type)=UPPER(p_entity_type))) and asso.is_termination_point=false)
and not exists(select 1 from layer_details where upper(layer_name)=upper(p_entity_type) and ne_class_name='SPLICE_CLOSURE')
then 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_152]');--(V_LAYER_TITLE||'can not be deleted as it is associated with some entity!'); 

--CHECK THE TERMINATION POINT DUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('duct') and is_enabled=true) 
and exists(select  1 from att_details_duct duct where (duct.a_system_id=p_system_id and upper(duct.a_entity_type)=upper(p_entity_type)) 
or (duct.b_system_id=p_system_id and upper(duct.b_entity_type)=upper(p_entity_type)))
then 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_053]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with duct!'); 

--CHECK THE TERMINATION POINT TRENCH
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('trench') and is_enabled=true) 
and exists(select  1 from att_details_trench trench where (trench.a_system_id=p_system_id and upper(trench.a_entity_type)=upper(p_entity_type)) 
or (trench.b_system_id=p_system_id and upper(trench.b_entity_type)=upper(p_entity_type)))
then   
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_054]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with trench!'); 

--CHECK THE TERMINATION POINT CABLE
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('cable') and is_enabled=true) 
  and exists(select 1 from att_details_cable cable where (cable.a_system_id=p_system_id and upper(cable.a_entity_type)=upper(p_entity_type)) 
  or (cable.b_system_id=p_system_id and upper(cable.b_entity_type)=upper(p_entity_type)))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_055]');--(V_LAYER_TITLE||' can not be deleted as it is used as termination point with cable!');

--CHECK THE TERMINATION POINT MICRODUCT
elsif exists(select 1 from vw_termination_point_master where upper(tp_layer_name)=upper(p_entity_type) and upper(layer_name)=upper('microduct') and is_enabled=true) 
and exists(select  1 from att_details_microduct microduct where (microduct.a_system_id=p_system_id and upper(microduct.a_entity_type)=upper(p_entity_type)) 
or (microduct.b_system_id=p_system_id and upper(microduct.b_entity_type)=upper(p_entity_type)))
then 
V_IS_VALID=FALSE;
V_MESSAGE:=(V_LAYER_TITLE ||' can not be deleted as it is used as termination point with microduct!'); --//have change the message for microduct 

elsif(upper(p_entity_type)='ROW') 
and exists(select 1 from att_details_row_apply where row_system_id=p_system_id) 
and not exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id)
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_056]');--(V_LAYER_TITLE||' can not be deleted as it is applied!');

elsif(upper(p_entity_type)='ROW') 
and exists(select 1 from att_details_row_apply where row_system_id=p_system_id) 
and  exists(select 1 from att_details_row_approve_reject where row_system_id=p_system_id and  upper(row_status)=upper('Approved'))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_057]');--(V_LAYER_TITLE||' can not be deleted as it has been Approved!');

elsif(upper(p_entity_type)='PIT')
then 
select parent_system_id into v_parent_system_id from att_details_row_pit where system_id=p_system_id;

IF EXISTS(SELECT 1 FROM ATT_DETAILS_ROW_APPLY WHERE ROW_SYSTEM_ID=v_parent_system_id) 
THEN
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as  ROW has been Applied!'); 
END IF;
ELSIF(upper(p_entity_type)='CABLE') AND EXISTS(SELECT 1 FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=p_system_id)
then

V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_058]');--(V_LAYER_TITLE||' can not be deleted as LMC is associated!');
ELSIF(upper(p_entity_type)='CABLE')
then
delete from att_details_loop where cable_system_id= p_system_id;

end if;

IF(upper(p_geom_type)='POLYGON') then

select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
raise info'V_LAYER_TABLE: %',V_LAYER_TABLE;

EXECUTE  'SELECT COALESCE(TARGET_REF_ID,0)  FROM '||V_LAYER_TABLE||' WHERE SYSTEM_ID='||P_SYSTEM_ID||'' INTO V_TARGET_REF_ID;

IF(upper(p_entity_type)!='SECTOR') THEN
IF exists (select 1 from 
(SELECT ptm.system_id ,ptm.entity_type,ptm.SP_GEOMETRY FROM point_master ptm  
UNION
SELECT lm.system_id ,lm.entity_type,lm.SP_GEOMETRY FROM line_master lm  
UNION
SELECT pm.system_id ,pm.entity_type,pm.SP_GEOMETRY FROM polygon_master pm  
) a WHERE ST_Within(a.SP_GEOMETRY, 
(select sp_geometry from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type))) 
and a.system_id!=p_system_id) 
then

V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as some child entities are present inside it!');

end if;

END IF;
-- IF(p_entity_type in('Area','Subarea','DSA') and
-- exists(select 1 from polygon_master pm 
-- where pm.system_id =p_system_id and upper(pm.entity_type) = upper(p_entity_type) and COALESCE(pm.gis_design_id ,'')!='') )
-- THEN 
-- V_IS_VALID=FALSE;
-- V_MESSAGE:='Reset the design ID to delete the entity';
-- END IF;
IF(V_TARGET_REF_ID>0 and (select COALESCE(value,'0')::INTEGER from global_settings gs where key='IS_DeleteEnabledForGISPush')=1)
THEN 
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as it is been pushed to GIS!');

END IF;

END IF;

IF(V_IS_VALID)
THEN
IF(UPPER(p_entity_type)=UPPER('SUBAREA')) 
THEN
select building_system_id,building_code into v_building_system_id,v_building_code from att_details_subarea where system_id=p_system_id;
delete from att_details_building where system_id=v_building_system_id and network_id=v_building_code;
delete from polygon_master where system_id=v_building_system_id and upper(entity_type)=upper('Building'); 
END IF;
IF(upper(p_entity_type)='FMS')
THEN
DELETE  FROM ATT_DETAILS_MODEL B  
USING ATT_DETAILS_FMS C 
WHERE UPPER(B.NETWORK_ID)=UPPER(C.NETWORK_ID)
AND C.SYSTEM_ID=P_SYSTEM_ID;     
END IF;

perform(fn_geojson_update_entity_attribute(p_system_id,p_entity_type,0,2,true));

select layer_table into v_layer_table from layer_details where upper(layer_name)=upper(p_entity_type);
execute  'select network_id from '||v_layer_table||' where system_id='||p_system_id||'' into v_network_id; 
execute  'delete from '||v_layer_table||' where system_id='||p_system_id||''; 
execute 'delete from '||p_geom_type||'_master where system_id='||p_system_id||' and upper(entity_type)=upper('''||p_entity_type||''')';
delete from polygon_master where system_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from isp_entity_mapping where entity_id=p_system_id and upper(entity_type)=upper(p_entity_type);
delete from isp_port_info where parent_system_id=p_system_id and upper(parent_entity_type)=upper(p_entity_type);
delete from associate_entity_info where (associated_system_id=p_system_id 
and upper(associated_entity_type)=upper(p_entity_type)) or (entity_system_id=p_system_id and upper(entity_type)=upper(p_entity_type)) and is_termination_point=true;
delete from connection_info  where (source_system_id=p_system_id 
and upper(source_entity_type)=upper(p_entity_type)) or (destination_system_id=p_system_id and upper(destination_entity_type)=upper(p_entity_type));
delete from isp_line_master  where entity_id=p_system_id  and upper(entity_type)=upper(p_entity_type);
delete from circle_master  where system_id=p_system_id  and upper(entity_type)=upper(p_entity_type);

IF EXISTS (select 1 from layer_details where upper(layer_name)=upper(p_entity_type) and is_reference_allowed=true) THEN
BEGIN
delete from att_entity_reference where system_id=p_system_id;
END;

IF(UPPER(p_entity_type)=UPPER('CUSTOMER'))
THEN
delete from wcr_mapping where CUSTOMER_ID=p_system_id;
END IF;

END IF;



RETURN QUERY 
SELECT true AS STATUS, fn_Multilingual_Message_Convert('en',Concat(V_LAYER_TITLE,' [SI_GBL_GBL_GBL_GBL_065]')::CHARACTER VARYING)::CHARACTER VARYING AS MESSAGE ; --V_LAYER_TITLE||' has deleted successfully!')
ELSE
RETURN QUERY 
SELECT V_IS_VALID AS STATUS, fn_Multilingual_Message_Convert('en',V_MESSAGE::CHARACTER VARYING) ::CHARACTER VARYING AS MESSAGE;
END IF;

if(coalesce(v_network_id,'')!='')
then
insert into entity_delete_log(system_id,entity_type,network_id,action_date,action_by)
values(p_system_id,p_entity_type,v_network_id,now(),p_user_id);
end if;

END
$BODY$;

ALTER FUNCTION public.fn_delete_entity(integer, character varying, character varying, integer)
    OWNER TO postgres;
