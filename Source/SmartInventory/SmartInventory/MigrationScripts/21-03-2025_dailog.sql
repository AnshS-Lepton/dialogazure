update layer_details set is_dynamic_control_enable= true where layer_name='Building';
----------------------------------------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_bulk_upload_fiberlink_insert(integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_bulk_upload_fiberlink_insert(integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_bulk_upload_fiberlink_insert(
	p_userid integer,
	v_network_id character varying)
    RETURNS TABLE(status boolean, message character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE  
V_BLD_RFS_TYPE CHARACTER VARYING;
V_created_by INTEGER;
V_USER_ID INTEGER;

r RECORD;
BEGIN

	--VALIDATE FOR NULL OR BLANK VALUES FOR ALL FIELDS
	--select link_type into V_link_type from temp_fiber_link;
   
	    -- INSERT INTO MAIN TABLE
  INSERT INTO att_details_fiber_link(Network_id,link_id,Link_Type,link_name,start_point_type,end_point_type,
  start_point_network_id,end_point_network_id ,
  
  no_of_lmc,CREATED_BY, CREATED_ON,handover_date,hoto_signoff_date,Redundant_Link_Type,Redundant_Link_Id,
  fiber_link_status,main_link_type,main_link_id,link_prefix,service_id)
  
 SELECT Network_id:: character VARYING,link_id,Link_Type,link_name,start_point_type,end_point_type,start_point_network_id,
 end_point_network_id ,
 no_of_lmc,CREATED_BY, CREATED_ON,handover_date,hoto_signoff_date,Redundant_Link_Type,Redundant_Link_Id,fiber_link_status,main_link_type,main_link_id
,link_prefix,service_id
    				FROM temp_Fiber_Link
    				WHERE  created_by =5  AND IS_VALID =true and network_id  =v_network_id;

      --UPDATE RECORDS WHICH SAVED IN Fiber
     UPDATE temp_Fiber_Link SET 
   -- IS_VALID =  FALSE ELSE TRUE ,
    ERROR_MSG='Fiber INSERTED SUCCESSFULLY!' 
     WHERE created_by =P_USERID  AND IS_VALID =TRUE;
 
 
RETURN QUERY SELECT TRUE AS STATUS, 'SUCCESSFULLY'::CHARACTER VARYING AS MESSAGE;
EXCEPTION  
                WHEN OTHERS THEN 
                RETURN QUERY
                SELECT FALSE AS STATUS, SQLERRM ::CHARACTER VARYING AS MESSAGE;              
END
$BODY$;

ALTER FUNCTION public.fn_bulk_upload_fiberlink_insert(integer, character varying)
    OWNER TO postgres;
