CREATE OR REPLACE FUNCTION public.fn_delete_bulkoperation_entities(p_geom text, p_userid integer, p_selectiontype character varying, p_radius double precision, p_network_status character varying, p_entity_type character varying, p_entity_sub_type character varying, p_system_id integer, p_root_id integer, p_selectedusers character varying)
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
   v_users Integer;
   
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

select into v_role_id role_id from user_master where user_id = p_userid;
raise info'v_role_id 1%',v_role_id;
		--raise info'role_id%',role_id;
	
select value into v_IsBulkDeleteSplicingAllowed from global_settings where upper(key)='ISBULKDELETESPLICINGALLOWED';

   if(P_system_id>0)
	then
	raise info'2nd IF';
		 INSERT INTO TEMPENTITYDETAILS  SELECT lst.*,false FROM 
		 fn_get_bulk_entity_by_geom(P_GEOM, P_USERID, P_SELECTIONTYPE, P_RADIUS,P_ENTITY_TYPE,P_NETWORK_STATUS)lst 
		 inner join layer_details lyr on upper(lyr.layer_name) = upper(lst.entity_type)
		 inner join role_permission_entity rp on rp.layer_id = lyr.layer_id  and rp.role_id = v_role_id and rp.network_status = P_NETWORK_STATUS and rp.delete = true where lst.project_id=P_system_id;
	else
	raise info'2nd else';

    if (SELECT COUNT(*)
FROM unnest(string_to_array(p_selectedUsers, ','))) = 1 and (SELECT CAST(trim(user_id) AS INTEGER)
    FROM unnest(string_to_array(p_selectedUsers, ',')) as user_id limit 1) = p_userid 
    then 
     INSERT INTO TEMPENTITYDETAILS  SELECT lst.*,false FROM 
		 fn_get_bulk_entity_by_geom(P_GEOM, p_userid, P_SELECTIONTYPE, P_RADIUS,P_ENTITY_TYPE,P_NETWORK_STATUS)lst 
		 inner join layer_details lyr on upper(lyr.layer_name) = upper(lst.entity_type)
		 inner join role_permission_entity rp on rp.layer_id = lyr.layer_id  and rp.role_id = v_role_id and rp.network_status = P_NETWORK_STATUS and rp.delete = true;
		
    else 
	FOR V_users IN SELECT CAST(trim(user_id) AS INTEGER)
    FROM unnest(string_to_array(p_selectedUsers, ',')) AS user_id
    loop 
		 INSERT INTO TEMPENTITYDETAILS  SELECT lst.*,false FROM 
		 fn_get_bulk_entity_by_userid_geom(P_GEOM, v_users, P_SELECTIONTYPE, P_RADIUS,P_ENTITY_TYPE,P_NETWORK_STATUS)lst 
		 inner join layer_details lyr on upper(lyr.layer_name) = upper(lst.entity_type)
		 inner join role_permission_entity rp on rp.layer_id = lyr.layer_id  and rp.role_id = v_role_id and rp.network_status = P_NETWORK_STATUS and rp.delete = true;
	END LOOP;
	end if;
	end if;
 IF (p_root_id != 0)
THEN
raise info'1st IF';
   delete from TEMPENTITYDETAILS where SYSTEM_ID NOT IN(select Entity_Id from associate_route_info where cable_id=p_root_id)
   and entity_type <>'Cable';
   
      delete from TEMPENTITYDETAILS where SYSTEM_ID NOT IN(select SYSTEM_ID from att_details_cable where SYSTEM_ID=p_root_id)
   and entity_type='Cable';
   
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
	
------------------------
		-- delete from associate_entity_info where entity_system_id=V_AROW.SYSTEM_ID and upper(entity_type)=upper(V_AROW.ENTITY_TYPE) and 
-- 		associated_system_id=V_AROW.SYSTEM_ID and upper(associated_entity_type)=upper(V_AROW.ENTITY_TYPE);	
-- 
-- 
-- 	delete from connection_info  where (source_system_id=V_AROW.SYSTEM_ID and upper(source_entity_type)=upper(V_AROW.ENTITY_TYPE)) 
-- 	or (destination_system_id=V_AROW.SYSTEM_ID and upper(destination_entity_type)=upper(V_AROW.ENTITY_TYPE));					 
-------------------	

--ADDED CASE FOR NEPAL TELECOM BY ANTRA
	IF(v_IsBulkDeleteSplicingAllowed=1) THEN
	
		FOR v_users IN SELECT CAST(trim(user_id) AS INTEGER)
    FROM unnest(string_to_array(p_selectedUsers, ',')) AS user_id
    loop  
		SELECT A.STATUS,A.MESSAGE INTO V_STATUS,V_MESSAGE FROM FN_DELETE_ENTITY(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,V_AROW.GEOM_TYPE,v_users) A LIMIT 1;

		
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
		raise info 'delet1%',V_AROW.SYSTEM_ID;
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

	FOR v_users IN SELECT CAST(trim(user_id) AS INTEGER)
    FROM unnest(string_to_array(p_selectedUsers, ',')) AS user_id
    loop  
			
		SELECT A.STATUS,A.MESSAGE INTO V_STATUS,V_MESSAGE FROM FN_DELETE_ENTITY(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,V_AROW.GEOM_TYPE,v_users) A LIMIT 1;
		
		IF(V_STATUS=TRUE)
		THEN 
				--v_status=true;
				--v_message= 'Entity Deleted Successfully!'; 
				raise info'V_STATUS1%',V_STATUS;
				 
				INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) 
				values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,V_STATUS,V_MESSAGE,'Yes');
				INSERT INTO ATT_DETAILS_BULK_ENTITY_DELETE_HISTORY(ENTITY_SYSTEM_ID,ENTITY_TYPE ,DELETED_BY ,DELETED_ON)
				VALUES(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,v_users,NOW());
				 
				--END IF;
				else
				raise info'V_AROW.Network_id%',V_AROW.NETWORK_ID;
				INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) 
				values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,false,V_MESSAGE,'No');
				end if;
		end loop;
			ELSE

 		INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) 
 		values(V_AROW.ENTITY_TYPE,V_AROW.Network_id,V_STATUS,' [SI_GBL_GBL_NET_FRM_210]','No');--There are some dependent elements, Please remove them first
 		
		--SELECT FALSE  AS STATUS, 'There are some dependent elements, Please remove them first'::CHARACTER VARYING AS MESSAGE;
		--RETURN;
		END IF;
 	 END IF;
	end loop;
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
	FOR v_users IN SELECT CAST(trim(user_id) AS INTEGER)
    FROM unnest(string_to_array(p_selectedUsers, ',')) AS user_id
    loop 
		 			
		SELECT A.STATUS,A.MESSAGE INTO V_STATUS,V_MESSAGE FROM FN_DELETE_ENTITY(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,V_AROW.GEOM_TYPE,v_users) A LIMIT 1;
		raise info'V_AROW.Network_id%',V_AROW.NETWORK_ID;
		raise info'GEOM_TYPE%',V_AROW.GEOM_TYPE;
		IF(V_STATUS=TRUE)
		THEN 
				--v_status=true;
				--v_message= 'Entity Deleted Successfully!'; 
				raise info'V_STATUS1%',V_STATUS;
				 
				INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,V_STATUS,V_MESSAGE,'Yes');
				INSERT INTO ATT_DETAILS_BULK_ENTITY_DELETE_HISTORY(ENTITY_SYSTEM_ID,ENTITY_TYPE ,DELETED_BY ,DELETED_ON)VALUES(V_AROW.SYSTEM_ID,V_AROW.ENTITY_TYPE,v_users,NOW());
				 
				--END IF;
				else
				raise info'V_AROW.Network_id%',V_AROW.NETWORK_ID;
				INSERT INTO tempEntityStatusDetails(V_ENTITY_TYPE,v_NETWORK_ID,v_Is_deleted,v_Error_Message,v_deleted_status) values(V_AROW.ENTITY_TYPE,V_AROW.NETWORK_ID,false,V_MESSAGE,'No');
				end if;
				
		end loop;
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
	   
	---
	
  
  

END LOOP;

 DELETE FROM ASSOCIATE_ROUTE_INFO WHERE CABLE_ID= P_ROOT_ID;
 
raise info'V_STATUS4%',V_STATUS;
--SELECT COALESCE(sum(CASE WHEN is_mobile_action THEN 1 ELSE 0 END),0)::integer  FROM layer_action_mapping where layer_id=9
--select count(*) into v_success_count from tempEntityStatusDetails where is_deleted=true;
RETURN QUERY (SELECT v_ENTITY_TYPE as Entity_Name,v_NETWORK_ID as network_code,v_Is_deleted as v_Is_Deleted,fn_Multilingual_Message_Convert('en',v_Error_Message)::character varying as Message,v_deleted_status as deleted_status  from tempEntityStatusDetails as a );
				RETURN;
 
END ;
$function$
;

-- Permissions

ALTER FUNCTION public.fn_delete_bulkoperation_entities(text, int4, varchar, float8, varchar, varchar, varchar, int4, int4, varchar) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_delete_bulkoperation_entities(text, int4, varchar, float8, varchar, varchar, varchar, int4, int4, varchar) TO public;
GRANT ALL ON FUNCTION public.fn_delete_bulkoperation_entities(text, int4, varchar, float8, varchar, varchar, varchar, int4, int4, varchar) TO postgres;


---------------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_get_bulk_entity_by_userid_geom(p_geom text, p_userid integer, p_selectiontype character varying, p_radius double precision, p_entity_type character varying, p_network_status character varying)
 RETURNS TABLE(system_id integer, entity_type character varying, network_status character varying, network_id character varying, project_id integer, created_by integer)
 LANGUAGE plpgsql
AS $function$

Declare
 v_role_id integer;
  v_geometry geometry;
BEGIN

select into v_role_id role_id from user_master where user_id = p_userid;
if not exists(select 1 from information_schema.tables where upper(table_name)='TEMP_REGION_PROV')
then
	create temp table temp_region_prov(
	region_id integer,
	id integer
	)on commit drop;
end if;

IF(p_selectiontype <> 'circle') 
THEN
	select st_geomfromtext('POLYGON(('||p_geom||'))',4326) into v_geometry;
else 
	select st_buffer_meters(St_Geomfromtext('Point('||p_geom||')',4326), p_radius) into v_geometry;
end if;

truncate table temp_region_prov;
insert into temp_region_prov(region_id,id)
select region_id,id from province_boundary prov where ST_Intersects(v_geometry,prov.sp_geometry);

RETURN QUERY
select bulkentity.system_id, bulkentity.entity_type, bulkentity.network_status, bulkentity.network_id,bulkentity.project_id,bulkentity.created_by from (
select pm.system_id, pm.entity_type, pm.network_status,pm.common_name as network_id,pm.project_id,pm.created_by from point_master as pm
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id 
where case when coalesce(p_entity_type,'')!='' then upper(pm.entity_type)=upper(p_entity_type) else 1=1 end and 
st_within(pm.sp_geometry,v_geometry)
union all
select lm.system_id, lm.entity_type, lm.network_status, lm.common_name as network_id,lm.project_id,lm.created_by from line_master as lm
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id 
where case when coalesce(p_entity_type,'')!='' then upper(lm.entity_type)=upper(p_entity_type) else 1=1 end and 
st_intersects(lm.sp_geometry, v_geometry)
union all
select poly.system_id, poly.entity_type, poly.network_status,poly.common_name as network_id,poly.project_id,poly.created_by from polygon_master poly
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id
where case when coalesce(p_entity_type,'')!='' then upper(poly.entity_type)=upper(p_entity_type) else 1=1 end 
and st_within(poly.sp_geometry, v_geometry)
union  
select room.system_id, 'Unit' as entity_type, room.network_status,room.network_id,room.project_id,null as created_by
	from point_master pm 
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id
inner join  isp_entity_mapping mapp on pm.system_id=mapp.structure_id and pm.entity_type='Structure'
inner join isp_room_info room On mapp.structure_id=room.parent_system_id  
where case when (upper(p_entity_type)='UNIT' or upper(p_entity_type)='') then upper(pm.entity_type)=upper('structure') end 
and st_within(pm.sp_geometry, v_geometry)
union
select cbl.system_id, 'Cable' as entity_type, cbl.network_status,cbl.network_id,cbl.project_id,cbl.created_by  from isp_line_master lm  
join att_details_cable cbl on cbl.system_id=lm.entity_id and lm.cable_type='ISP'
join point_master pm
on pm.system_id=lm.structure_id and upper(pm.entity_type)='STRUCTURE'
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id
where case when (upper(p_entity_type)=upper('Cable') or upper(p_entity_type)='') then 1=1 end 
and st_within(pm.sp_geometry,v_geometry)
union 
select pit.system_id, 'PIT' as entity_type, 'P' as network_status,pit.network_id,0 as project_id,null as created_by  from circle_master cm
 inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id  
join att_details_row_pit pit
on pit.system_id=cm.system_id  
join att_details_row row
on row.system_id=pit.parent_system_id and upper(pit.parent_entity_type)='ROW'
where case when (upper(p_entity_type)=upper('PIT') or upper(p_entity_type)='') then 1=1  end 
and st_within(cm.sp_geometry, v_geometry)
Union
select rack.system_id, 'Rack' as entity_type, rack.network_status,rack.network_id,0 as project_id,null as created_by  from point_master pm 
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id 
inner join  isp_entity_mapping mapp on pm.system_id=mapp.entity_id and upper(pm.entity_type)=upper('POD')
inner join att_details_rack rack 
On mapp.entity_id=rack.parent_system_id  
where case when (upper(p_entity_type)='RACK' or upper(p_entity_type)='') then upper(pm.entity_type)=upper('POD') end 
and st_within(pm.sp_geometry, v_geometry)
Union
select model.system_id, 'Equipment' as entity_type, model.network_status,model.network_id,0 as project_id,null as created_by  from point_master pm
inner join user_permission_area upa on  upa.user_id=p_userid
inner join temp_region_prov province on upa.region_id=province.region_id and upa.province_id=province.id 
inner join  isp_entity_mapping mapp on pm.system_id=mapp.entity_id and upper(pm.entity_type)=upper('POD')
inner join att_details_model model 
On mapp.entity_id=model.parent_system_id  
where case when (upper('Equipment')='EQUIPMENT' or upper('Equipment')='') then upper(pm.entity_type)=upper('POD') end 
and st_within(pm.sp_geometry, v_geometry)
) as bulkentity 
left join layer_details l on upper(bulkentity.entity_type)=upper(l.layer_name)   
inner join vw_network_layers nl on nl.layer_id = l.layer_id and nl.role_id = v_role_id
and (COALESCE(nl.planned_view,false) = true or COALESCE(nl.asbuild_view,false)  = true or COALESCE(nl.dormant_view,false)  = true )
where upper(p_network_status) =upper(bulkentity.network_status) and bulkentity.created_by = p_userid order by bulkentity.entity_type  ;
END; 
$function$
;

-- Permissions

ALTER FUNCTION public.fn_get_bulk_entity_by_userid_geom(text, int4, varchar, float8, varchar, varchar) OWNER TO postgres;
GRANT ALL ON FUNCTION public.fn_get_bulk_entity_by_userid_geom(text, int4, varchar, float8, varchar, varchar) TO postgres;
