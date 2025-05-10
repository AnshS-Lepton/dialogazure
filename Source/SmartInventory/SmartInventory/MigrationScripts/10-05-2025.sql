------------------------------------------Insert data-------------------------------------------------------------------

INSERT INTO public.module_master
(module_name, module_description, icon_content, icon_class, created_by, created_on, modified_by, modified_on, "type", is_active, module_abbr, parent_module_id, module_sequence, is_offline_enabled)
VALUES('Create Segment', 'Create Segment', '', '', NULL, now(), 287, now(), 'Web', true, 'TPCS', (select id from module_master m where module_abbr='TPL' and module_name='Topology Details' ), 0, false, NULL, NULL);

-----------------------------------------Create new function to get route connected element details--------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_route_connectedelement_details(
	p_route_id integer,
	p_user_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 
DECLARE 
v_Query text;
p_site1_geom character varying;
	v_geometry_with_buffer geometry;
	 p_site2_geom character varying;
	 startroute integer default 1;
	 geom_text TEXT;
Query text; 
rec record;

BEGIN
	create temp table temp_connectedElement(
		connected_system_id integer, 
		connected_network_id character varying(100), 
		connected_entity_type character varying(100),
		connected_entity_geom character varying ,
		is_virtual boolean,
		is_agg_site boolean
	)on commit drop;
	
	 create temp table temp_cableInfo(
		
	        cable_system_id integer,
	        cable_network_id character varying(100),
	        cable_type character varying(100),
	        cable_geom character varying
	       
	)on commit drop;
	
 -- Create a temporary table to store site geometries
    CREATE TEMP TABLE temp_site_geom_route( 
        site_id integer, 
        site_name character varying, 
        geom geometry,
		ringid integer,
        user_id integer,
		 cbl_distance numeric
    ) ON COMMIT DROP; 
	

Raise info 'p_route_id ->%',p_route_id;
 
 -- Insert site geometries for the given ring_id
    INSERT INTO temp_site_geom_route(site_id, site_name, geom,ringid, user_id,cbl_distance)
     select distinct sitedetails.system_id , sitedetails.site_name , ST_GeomFromText('POINT(' || sitedetails.longitude || ' ' || sitedetails.latitude || ')', 4326),sitedetails.ring_id,p_user_id,sitedetails.length_meters 
	 from  (select pod.system_id,pod.site_name,pod.longitude,pod.latitude,pod.ring_id,0 AS length_meters  
			from att_details_pod pod
    where pod.system_id in (SELECT agg1site_id AS site_id FROM top_routes WHERE route_id = p_route_id
UNION ALL
SELECT agg2site_id AS site_id FROM top_routes WHERE route_id = p_route_id)) sitedetails;
	

insert into temp_cableInfo(cable_system_id,cable_network_id,cable_type,cable_geom)
select distinct lm.system_id,cbl.network_id,lm.entity_type,st_astext(lm.sp_geometry) as connected_entity_geom 
from att_details_cable cbl 
		inner join line_master lm ON  upper(lm.entity_type)='CABLE' and cbl.system_id=lm.system_id
		where cbl.network_id in (select unnest(string_to_array(regexp_replace(cable_names, '[\{\}\n\r]+', ',', 'g'), ',')) AS code from top_routes where route_id=p_route_id) ;

Raise info 'Temp table row ->%',(select count(1) from temp_site_geom);
Raise info 'temp_cableInfo row ->%',(select count(1) from temp_cableInfo);
		
insert into temp_connectedElement(connected_system_id,connected_network_id,connected_entity_type,connected_entity_geom,is_virtual,is_agg_site)
		select pm.system_id,pod.network_id,pm.entity_type,st_astext(pm.sp_geometry) as connected_entity_geom,pm.is_virtual,pod.is_agg_site from temp_site_geom_route ci
		inner join att_details_pod pod on pod.system_id=ci.site_id
		inner join point_master pm ON  upper(pm.entity_type)='POD' and pod.system_id=pm.system_id;
		
Raise info 'temp_connectedElement row ->%',(select count(1) from temp_connectedElement);

   
	RETURN QUERY select row_to_json(result) 
	from (
		select (
			select array_to_json(array_agg(row_to_json(x)))
			from (
				select * from temp_cableInfo
				
			) x
		) as lstCableInfo,
		(
			select array_to_json(array_agg(row_to_json(x)))
			from (
				select distinct connected_system_id,connected_network_id,connected_entity_type
				 ,connected_entity_geom ,is_virtual,COALESCE(is_agg_site, false) AS is_agg_site from temp_connectedElement temp
				inner join layer_details ld on upper(temp.connected_entity_type)=upper(ld.layer_name)
				
			) x
		) as lstConnectedElements
	)result;
 

END 
$BODY$;

-----------------------------------------------Modify delete entity function--------------------------------------------
 DROP FUNCTION IF EXISTS public.fn_delete_entity(integer, character varying, character varying, integer);

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

IF(V_IS_VALID and p_entity_type='Cable' and(exists(select 1 from top_ring_cable_mapping where cable_id=p_system_id ) or exists(select 1 from top_segment_cable_mapping where cable_id=p_system_id )) )
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as cable already in a topology so please rearrange it!');
end if;

IF(V_IS_VALID and upper(p_entity_type)='POD' and exists(select 1 from top_ring_site_mapping where site_id=p_system_id ))
then
V_IS_VALID=FALSE;
V_MESSAGE:=Concat(V_LAYER_TITLE,' can not be deleted as site already in a topology so please rearrange it!');
end if;

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

if(UPPER(p_entity_type)=UPPER('POD') and (select count(1) from top_ring_site_mapping where site_id=p_system_id)>0) then

 delete from top_ring_site_mapping where site_id=p_system_id;
END IF;
if(UPPER(p_entity_type)=UPPER('Cable') and (select count(1) from top_ring_cable_mapping where cable_id=p_system_id)>0) then

 delete from top_ring_cable_mapping where cable_id=p_system_id;
END IF;
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

	------------------------------------------------------ Modify function-----------------------------------

	 DROP FUNCTION IF EXISTS public.fn_get_segment_report_data(character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer);

CREATE OR REPLACE FUNCTION public.fn_get_segment_report_data(
	p_searchby character varying,
	p_searchbytext character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_duration_based_column character varying,
	p_userid integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

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

-- DYNAMIC QUERY
	sql := '
SELECT ROW_NUMBER() OVER (ORDER BY ' || 
       COALESCE(NULLIF(p_sortcolname, ''), 'system_id') || ' ' ||
       COALESCE(NULLIF(p_sorttype, ''), 'desc') || ')::Integer AS S_No,
       system_id,
       region_name,
       segment_code,
       description,
       sequence,
       agg1,
       agg2,
       route_name
FROM (
    SELECT ts.route_id AS system_id,
           tr2.region_name,
           ts.segment_code,
           ts.description,
           ts."sequence",
           adp.site_id || '' ('' || adp.network_id || '')'' AS agg1,
           adp2.site_id || '' ('' || adp2.network_id || '')'' AS agg2,
           tr.route_name
           
    FROM top_segment ts
    INNER JOIN top_region tr2 ON tr2.id = ts.region_id 
    LEFT JOIN vw_att_details_pod adp ON ts.agg1_site_id = adp.system_id 
    LEFT JOIN vw_att_details_pod adp2 ON adp2.system_id = ts.agg2_site_id
    LEFT JOIN top_routes tr ON tr.route_id = ts.route_id where 1=1 ';

		RAISE INFO '------------------------------------sql1%', sql;

IF ((P_searchbytext) != '' and (P_searchby) != '') THEN
sql:= sql ||' AND upper(Cast(ts.'||P_searchby||' as TEXT)) LIKE upper(''%'||trim(P_searchbytext)||'%'')';

	--if(substring(P_searchbytext from 1 for 1)='"' and  substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
--	then
--		sql:= sql ||' AND upper(Cast(ts.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
--	else
		--sql:= sql ||' AND upper(Cast(ts.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
--	end if;

END IF;

IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND ts.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and ts.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

sql:= sql ||' )X';
RAISE INFO '%', sql;
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
         sql:= 'SELECT '||TotalRecords||' as totalRecords,* FROM (' || sql || ') T';                  
 END IF; 

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;
$BODY$;

ALTER FUNCTION public.fn_get_segment_report_data(character varying, character varying, character varying, character varying, integer, integer, character varying, character varying, character varying, integer)
    OWNER TO postgres;
