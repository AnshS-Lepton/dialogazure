CREATE OR REPLACE FUNCTION public.fn_delete_bulkoperation_entities(p_geom text, p_userid integer, p_selectiontype character varying,
p_radius double precision, p_network_status character varying, p_entity_type character varying,
p_entity_sub_type character varying, p_system_id integer)
 RETURNS TABLE(entity_name character varying, network_code character varying, is_deleted boolean, message character varying, deleted_status character varying)
 LANGUAGE plpgsql
AS $function$

 DECLARE
   V_AROW RECORD;
   V_STATUS BOOLEAN;
   V_MESSAGE CHARACTER VARYING(500);  
   v_success_count integer;
   v_role_id integer;
   v_entity_sub_type_column CHARACTER VARYING(500);
   v_table_name CHARACTER VARYING(500);
   v_sql TEXT;
   V_LAYER_TITLE CHARACTER VARYING(500);  
   v_is_parent_exist boolean;
   v_IsBulkDeleteSplicingAllowed integer;
   
BEGIN
v_status=true;
v_message= 'Entity Deleted Successfully!';

CREATE TEMP TABLE tempEntityStatusDetails(
 v_ENTITY_TYPE CHARACTER VARYING(500),
 v_NETWORK_ID CHARACTER VARYING(100),
 v_Is_deleted boolean,
 v_Error_Message CHARACTER VARYING(100),
 v_deleted_status character varying(100)
 )on commit drop;
 

CREATE TEMP TABLE TEMPENTITYDETAILS(
 SYSTEM_ID INTEGER,
 ENTITY_TYPE CHARACTER VARYING(500),
 NETWORK_STATUS CHARACTER VARYING(100),
 network_id character varying(100),
 project_id integer,
 created_by integer,
 Is_deleted boolean
) on commit drop;

select into v_role_id role_id from user_master where user_id = p_userid;
select value into v_IsBulkDeleteSplicingAllowed from global_settings where upper(key)='ISBULKDELETESPLICINGALLOWED';
 if(P_system_id>0)
 then
 INSERT INTO TEMPENTITYDETAILS  SELECT lst.*,false FROM 
 fn_get_bulk_entity_by_geom(P_GEOM, P_USERID, P_SELECTIONTYPE, P_RADIUS,P_ENTITY_TYPE,P_NETWORK_STATUS)lst 
 inner join layer_details lyr on upper(lyr.layer_name) = upper(lst.entity_type)
 inner join role_permission_entity rp on rp.layer_id = lyr.layer_id  and rp.role_id = v_role_id and rp.network_status = P_NETWORK_STATUS and rp.delete = true where lst.project_id=P_system_id;
 else
 INSERT INTO TEMPENTITYDETAILS  SELECT lst.*,false FROM 
 fn_get_bulk_entity_by_geom(P_GEOM, P_USERID, P_SELECTIONTYPE, P_RADIUS,P_ENTITY_TYPE,P_NETWORK_STATUS)lst 
 inner join layer_details lyr on upper(lyr.layer_name) = upper(lst.entity_type)
 inner join role_permission_entity rp on rp.layer_id = lyr.layer_id  and rp.role_id = v_role_id and rp.network_status = P_NETWORK_STATUS and rp.delete = true;
 end if;
raise info 'p_entity_type :%',p_entity_type;

----------------ankit

if(coalesce(p_entity_sub_type,'')!='')
then
	select  entity_sub_type_column , layer_table  into v_entity_sub_type_column,v_table_name
	from layer_details ld join bulk_operation_settings bos on bos.layer_id =ld.layer_id 
	where upper(ld.layer_name) =upper(p_entity_type) and bos.is_active=true;

		if(coalesce(v_entity_sub_type_column,'')!='')
	then 
			v_sql:= 'delete from TEMPENTITYDETAILS where system_id not in(select td.system_id from TEMPENTITYDETAILS td join  '||v_table_name||' t 
			on t.system_id=td.system_id where '||v_entity_sub_type_column||'  ='''||p_entity_sub_type||''') ';

		
if(v_sql<>'')
then
 EXECUTE v_sql;

end if;
	end if;

end if;

---------------ankit

 FOR V_AROW IN   SELECT DEO.LAYER_ID,DEO.LAYER_NAME AS ENTITY_TYPE ,DEO.ORDER_ID,DEO.SUB_ORDER_ID, TEMPENTITY.SYSTEM_ID,DEO.GEOM_TYPE,TEMPENTITY.network_id
		FROM VW_ATT_DETAILS_DELETE_ENTITY_ORDER DEO
		INNER JOIN TEMPENTITYDETAILS TEMPENTITY
		ON UPPER(TEMPENTITY.ENTITY_TYPE)=UPPER(DEO.LAYER_NAME)  ORDER BY DEO.ORDER_ID ,DEO.SUB_ORDER_ID   
	LOOP
	
	v_status=false;

--ADDED CASE FOR NEPAL TELECOM BY ANTRA
	IF(v_IsBulkDeleteSplicingAllowed=1) THEN
	
		SELECT A.STATUS,A.MESSAGE INTO V_STATUS,V_MESSAGE FROM FN_DELETE_ENTITY(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,V_AROW.GEOM_TYPE,p_userid) A LIMIT 1;

		
		IF(V_STATUS <> TRUE) THEN
		INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) 
		values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,false,V_MESSAGE,'No');

		ELSE
		
		IF NOT EXISTS(SELECT 1 FROM FN_GET_DEPENDENT_CHILD_ELEMENTS(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,V_AROW.GEOM_TYPE,''))
		THEN  
		--DELETE SPLICING::::----
		IF EXISTS(select 1 from connection_info where ((source_system_id=V_AROW.SYSTEM_ID and upper(source_entity_type)=upper(V_AROW.ENTITY_TYPE))
		or (destination_system_id=V_AROW.SYSTEM_ID and upper(destination_entity_type)=upper(V_AROW.ENTITY_TYPE)))
		and (((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))))
		THEN  			
			delete from connection_info  where (source_system_id=V_AROW.SYSTEM_ID and upper(source_entity_type)=upper(V_AROW.ENTITY_TYPE)) 
			or (destination_system_id=V_AROW.SYSTEM_ID and upper(destination_entity_type)=upper(V_AROW.ENTITY_TYPE));					 
		END IF;
		 
		--DELETE ASSOCIATION::::---
		IF EXISTS(select 1 from  associate_entity_info asso where ((asso.associated_system_id=V_AROW.SYSTEM_ID 
		and upper(asso.associated_entity_type)=UPPER(V_AROW.ENTITY_TYPE))
		OR (asso.entity_system_id=V_AROW.SYSTEM_ID and upper(asso.entity_type)=UPPER(V_AROW.ENTITY_TYPE))) and asso.is_termination_point=false)
		THEN	
			delete from associate_entity_info where (associated_system_id=V_AROW.SYSTEM_ID and upper(associated_entity_type)=upper(V_AROW.ENTITY_TYPE))
			or (entity_system_id=V_AROW.SYSTEM_ID and upper(entity_type)=upper(V_AROW.ENTITY_TYPE));
		END IF;
		   
		-- DELETE REFERENCES-::::
		DELETE FROM ATT_ENTITY_REFERENCE WHERE SYSTEM_ID=V_AROW.SYSTEM_ID AND Upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE LIBRARY_ATTACHMENTS-:::
		DELETE FROM LIBRARY_ATTACHMENTS WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ATT_ENTITY_AT_ACCEPTANCE-:::
		DELETE FROM ATT_ENTITY_AT_ACCEPTANCE WHERE SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ENTITY_AT_ACCEPTANCE_ATTACHMENTS-:::
		DELETE FROM ENTITY_AT_ACCEPTANCE_ATTACHMENTS WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND ENTITY_TYPE=UPPER(V_AROW.ENTITY_TYPE);
 			 
		-- DELETE ENTITY_MAINTAINENCE_CHARGES-:::
		DELETE FROM ENTITY_MAINTAINENCE_CHARGES WHERE ENTITY_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ENTITY_MAINTAINENCE_CHARGES_ATTACHMENTS-:::
		--DELETE FROM ENTITY_MAINTAINENCE_CHARGES_ATTACHMENTS WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ENTITY_NOTIFICATIONS_COMMENTS-:::
		DELETE FROM ENTITY_NOTIFICATIONS_COMMENTS WHERE NOTIFICATION_ID IN(SELECT ENC.NOTIFICATION_ID FROM ENTITY_NOTIFICATIONS_COMMENTS ENC 
		INNER JOIN ENTITY_NOTIFICATIONS EN ON ENC.NOTIFICATION_ID=EN.NOTIFICATION_ID 
		WHERE EN.ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(EN.ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE));
			 	
		-- DELETE ENTITY_NOTIFICATIONS-:::
		DELETE FROM ENTITY_NOTIFICATIONS WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ENTITY_NOTIFICATION_STATUS-:::
		DELETE FROM ENTITY_NOTIFICATION_STATUS WHERE SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		
		-- DELETE ENTITY_REGION_PROVINCE_MAPPING-:::
		DELETE FROM ENTITY_REGION_PROVINCE_MAPPING WHERE ENTITY_ID=V_AROW.SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE WCR_MAPPING-:::
		DELETE FROM WCR_MAPPING WHERE ENTITY_ID=V_AROW.SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 	
		-- DELETE ATT_DETAILS_LMC_CABLE_INFO-:::
		DELETE FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=V_AROW.SYSTEM_ID and UPPER(V_AROW.ENTITY_TYPE)='CABLE';
			 	
		-- DELETE ATT_DETAILS_ROW_ASSOCIATE_ENTITY_INFO-:::
		DELETE FROM ATT_DETAILS_ROW_ASSOCIATE_ENTITY_INFO WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 	
		-- DELETE ATT_DETAILS_SITE_INFO-:::
		DELETE FROM ATT_DETAILS_SITE_INFO WHERE PARENT_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(PARENT_ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
		
		DELETE FROM ATT_DETAILS_COMPETITOR WHERE SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(V_AROW.ENTITY_TYPE)=UPPER('COMPETITOR');	 	
		-- DELETE LOOP-::::
		DELETE FROM ATT_DETAILS_LOOP 
		where ((CABLE_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(V_AROW.ENTITY_TYPE)='CABLE') OR (ASSOCIATED_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(ASSOCIATED_ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE)));

		-- DELETE ATT_DETAILS_ROW_APPLY-::::
		DELETE FROM ATT_DETAILS_ROW_APPLY 
		WHERE (ROW_SYSTEM_ID in(select parent_system_id from att_details_row_pit where system_id=V_AROW.SYSTEM_ID) and UPPER(V_AROW.ENTITY_TYPE)=UPPER('PIT')
		OR (ROW_SYSTEM_ID=V_AROW.SYSTEM_ID and UPPER(V_AROW.ENTITY_TYPE)=UPPER('ROW')));
		 
		-- DELETE ATT_DETAILS_ROW_APPROVE_REJECT
		DELETE FROM ATT_DETAILS_ROW_APPROVE_REJECT 
		WHERE (ROW_SYSTEM_ID in(select parent_system_id from att_details_row_pit where system_id=V_AROW.SYSTEM_ID) and UPPER(V_AROW.ENTITY_TYPE)=UPPER('PIT')
		OR (ROW_SYSTEM_ID=V_AROW.SYSTEM_ID and UPPER(V_AROW.ENTITY_TYPE)=UPPER('ROW'))); 
		 
		-- DELETE TICKET_MASTER-::::
		IF(UPPER(V_AROW.ENTITY_TYPE)='BUILDING')
		THEN
			DELETE FROM TICKET_MASTER WHERE BUILDING_CODE IN(SELECT NETWORK_ID FROM ATT_DETAILS_BUILDING WHERE SYSTEM_ID=V_AROW.SYSTEM_ID);
		END IF; 

		 
			
		SELECT A.STATUS,A.MESSAGE INTO V_STATUS,V_MESSAGE FROM FN_DELETE_ENTITY(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,V_AROW.GEOM_TYPE,p_userid) A LIMIT 1;
		raise info'V_AROW.Network_id%',V_AROW.NETWORK_ID;
		raise info'GEOM_TYPE%',V_AROW.GEOM_TYPE;
		IF(V_STATUS=TRUE)
		THEN 
				--v_status=true;
				--v_message= 'Entity Deleted Successfully!'; 
				raise info'V_STATUS1%',V_STATUS;
				 
				INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) 
				values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,V_STATUS,V_MESSAGE,'Yes');
				INSERT INTO ATT_DETAILS_BULK_ENTITY_DELETE_HISTORY(ENTITY_SYSTEM_ID,ENTITY_TYPE ,DELETED_BY ,DELETED_ON)
				VALUES(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,p_userid,NOW());
				 
				--END IF;
				else
				raise info'V_AROW.Network_id%',V_AROW.NETWORK_ID;
				INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) 
				values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,false,V_MESSAGE,'No');
				end if;
			ELSE

 		INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) 
 		values(V_AROW.ENTITY_TYPE,V_AROW.Network_id,V_STATUS,' [SI_GBL_GBL_NET_FRM_210]','No');--There are some dependent elements, Please remove them first
 		
		--SELECT FALSE  AS STATUS, 'There are some dependent elements, Please remove them first'::CHARACTER VARYING AS MESSAGE;
		--RETURN;
		END IF;
 	 END IF;
	
 	ELSE
 	
 	IF NOT EXISTS(SELECT 1 FROM FN_GET_DEPENDENT_CHILD_ELEMENTS(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,V_AROW.GEOM_TYPE,''))
	THEN  
		--DELETE SPLICING::::----
		IF EXISTS(select 1 from connection_info where ((source_system_id=V_AROW.SYSTEM_ID and upper(source_entity_type)=upper(V_AROW.ENTITY_TYPE))
		or (destination_system_id=V_AROW.SYSTEM_ID and upper(destination_entity_type)=upper(V_AROW.ENTITY_TYPE)))
		and (((source_entity_type::text)||(source_system_id::text))!=((destination_entity_type::text)||(destination_system_id::text))))
		THEN  			
			delete from connection_info  where (source_system_id=V_AROW.SYSTEM_ID and upper(source_entity_type)=upper(V_AROW.ENTITY_TYPE)) 
			or (destination_system_id=V_AROW.SYSTEM_ID and upper(destination_entity_type)=upper(V_AROW.ENTITY_TYPE));					 
		END IF;
		 
		--DELETE ASSOCIATION::::---
		IF EXISTS(select 1 from  associate_entity_info asso where ((asso.associated_system_id=V_AROW.SYSTEM_ID and upper(asso.associated_entity_type)=UPPER(V_AROW.ENTITY_TYPE))
		OR (asso.entity_system_id=V_AROW.SYSTEM_ID and upper(asso.entity_type)=UPPER(V_AROW.ENTITY_TYPE))) and asso.is_termination_point=false)
		THEN	
			delete from associate_entity_info where (associated_system_id=V_AROW.SYSTEM_ID and upper(associated_entity_type)=upper(V_AROW.ENTITY_TYPE))
			or (entity_system_id=V_AROW.SYSTEM_ID and upper(entity_type)=upper(V_AROW.ENTITY_TYPE));
		END IF;
		   
		-- DELETE REFERENCES-::::
		DELETE FROM ATT_ENTITY_REFERENCE WHERE SYSTEM_ID=V_AROW.SYSTEM_ID AND Upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE LIBRARY_ATTACHMENTS-:::
		DELETE FROM LIBRARY_ATTACHMENTS WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ATT_ENTITY_AT_ACCEPTANCE-:::
		DELETE FROM ATT_ENTITY_AT_ACCEPTANCE WHERE SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ENTITY_AT_ACCEPTANCE_ATTACHMENTS-:::
		DELETE FROM ENTITY_AT_ACCEPTANCE_ATTACHMENTS WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND ENTITY_TYPE=UPPER(V_AROW.ENTITY_TYPE);
 			 
		-- DELETE ENTITY_MAINTAINENCE_CHARGES-:::
		DELETE FROM ENTITY_MAINTAINENCE_CHARGES WHERE ENTITY_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ENTITY_MAINTAINENCE_CHARGES_ATTACHMENTS-:::
		--DELETE FROM ENTITY_MAINTAINENCE_CHARGES_ATTACHMENTS WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ENTITY_NOTIFICATIONS_COMMENTS-:::
		DELETE FROM ENTITY_NOTIFICATIONS_COMMENTS WHERE NOTIFICATION_ID IN(SELECT ENC.NOTIFICATION_ID FROM ENTITY_NOTIFICATIONS_COMMENTS ENC 
		INNER JOIN ENTITY_NOTIFICATIONS EN ON ENC.NOTIFICATION_ID=EN.NOTIFICATION_ID 
		WHERE EN.ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(EN.ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE));
			 	
		-- DELETE ENTITY_NOTIFICATIONS-:::
		DELETE FROM ENTITY_NOTIFICATIONS WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE ENTITY_NOTIFICATION_STATUS-:::
		DELETE FROM ENTITY_NOTIFICATION_STATUS WHERE SYSTEM_ID=V_AROW.SYSTEM_ID AND upper(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		
		-- DELETE ENTITY_REGION_PROVINCE_MAPPING-:::
		DELETE FROM ENTITY_REGION_PROVINCE_MAPPING WHERE ENTITY_ID=V_AROW.SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 
		-- DELETE WCR_MAPPING-:::
		DELETE FROM WCR_MAPPING WHERE ENTITY_ID=V_AROW.SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 	
		-- DELETE ATT_DETAILS_LMC_CABLE_INFO-:::
		DELETE FROM ATT_DETAILS_LMC_CABLE_INFO WHERE CABLE_SYSTEM_ID=V_AROW.SYSTEM_ID and UPPER(V_AROW.ENTITY_TYPE)='CABLE';
			 	
		-- DELETE ATT_DETAILS_ROW_ASSOCIATE_ENTITY_INFO-:::
		DELETE FROM ATT_DETAILS_ROW_ASSOCIATE_ENTITY_INFO WHERE ENTITY_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
			 	
		-- DELETE ATT_DETAILS_SITE_INFO-:::
		DELETE FROM ATT_DETAILS_SITE_INFO WHERE PARENT_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(PARENT_ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE);
		
		DELETE FROM ATT_DETAILS_COMPETITOR WHERE SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(V_AROW.ENTITY_TYPE)=UPPER('COMPETITOR');	 	
		-- DELETE LOOP-::::
		DELETE FROM ATT_DETAILS_LOOP 
		where ((CABLE_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(V_AROW.ENTITY_TYPE)='CABLE') OR (ASSOCIATED_SYSTEM_ID=V_AROW.SYSTEM_ID AND UPPER(ASSOCIATED_ENTITY_TYPE)=UPPER(V_AROW.ENTITY_TYPE)));

		-- DELETE ATT_DETAILS_ROW_APPLY-::::
		DELETE FROM ATT_DETAILS_ROW_APPLY 
		WHERE (ROW_SYSTEM_ID in(select parent_system_id from att_details_row_pit where system_id=V_AROW.SYSTEM_ID) and UPPER(V_AROW.ENTITY_TYPE)=UPPER('PIT')
		OR (ROW_SYSTEM_ID=V_AROW.SYSTEM_ID and UPPER(V_AROW.ENTITY_TYPE)=UPPER('ROW')));
		 
		-- DELETE ATT_DETAILS_ROW_APPROVE_REJECT
		DELETE FROM ATT_DETAILS_ROW_APPROVE_REJECT 
		WHERE (ROW_SYSTEM_ID in(select parent_system_id from att_details_row_pit where system_id=V_AROW.SYSTEM_ID) and UPPER(V_AROW.ENTITY_TYPE)=UPPER('PIT')
		OR (ROW_SYSTEM_ID=V_AROW.SYSTEM_ID and UPPER(V_AROW.ENTITY_TYPE)=UPPER('ROW'))); 
		 
		-- DELETE TICKET_MASTER-::::
		IF(UPPER(V_AROW.ENTITY_TYPE)='BUILDING')
		THEN
			DELETE FROM TICKET_MASTER WHERE BUILDING_CODE IN(SELECT NETWORK_ID FROM ATT_DETAILS_BUILDING WHERE SYSTEM_ID=V_AROW.SYSTEM_ID);
		END IF; 

		 
			
		SELECT A.STATUS,A.MESSAGE INTO V_STATUS,V_MESSAGE FROM FN_DELETE_ENTITY(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,V_AROW.GEOM_TYPE,p_userid) A LIMIT 1;
		raise info'V_AROW.Network_id%',V_AROW.NETWORK_ID;
		raise info'GEOM_TYPE%',V_AROW.GEOM_TYPE;
		IF(V_STATUS=TRUE)
		THEN 
				--v_status=true;
				--v_message= 'Entity Deleted Successfully!'; 
				raise info'V_STATUS1%',V_STATUS;
				 
				INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,V_STATUS,V_MESSAGE,'Yes');
				INSERT INTO ATT_DETAILS_BULK_ENTITY_DELETE_HISTORY(ENTITY_SYSTEM_ID,ENTITY_TYPE ,DELETED_BY ,DELETED_ON)VALUES(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,p_userid,NOW());
				 
				--END IF;
				else
				raise info'V_AROW.Network_id%',V_AROW.NETWORK_ID;
				INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,false,V_MESSAGE,'No');
				end if;
		--END IF;
		ELSE
 		--RETURN QUERY
 		raise info'v_ENTITY_Type%',V_AROW.ENTITY_TYPE;
 		raise info'V_STATUS3%',V_STATUS;
 		raise info'V_AROW.Network_id%',V_MESSAGE;

 		INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) 
 		values(V_AROW.ENTITY_TYPE,V_AROW.Network_id,V_STATUS,' [SI_GBL_GBL_NET_FRM_210]','No');--There are some dependent elements, Please remove them first
 		
		--SELECT FALSE  AS STATUS, 'There are some dependent elements, Please remove them first'::CHARACTER VARYING AS MESSAGE;
		--RETURN;
		END IF;
 	   END IF;
END LOOP;
raise info'V_STATUS4%',V_STATUS;
--SELECT COALESCE(sum(CASE WHEN is_mobile_action THEN 1 ELSE 0 END),0)::integer  FROM layer_action_mapping where layer_id=9
--select count(*) into v_success_count from tempEntityStatusDetails where is_deleted=true;
RETURN QUERY (SELECT v_ENTITY_TYPE as Entity_Name,v_NETWORK_ID as network_code,v_Is_deleted as v_Is_Deleted,fn_Multilingual_Message_Convert('en',v_Error_Message)::character varying as Message,v_deleted_status as deleted_status  from tempEntityStatusDetails as a );
				RETURN;
 
END ;
$function$
;
