update res_resources set is_jquery_used=true where key='SI_OSP_ONT_NET_RPT_012';

/*------------------------------------------
CreatedBy: 
CreatedOn: 
Description: 
ModifiedOn: 31 Jan 2025
ModifiedBy: Chandra Shekhar Sahni
Purpose: Removed commented code also handled if p_user_id is coming as null or empty then return empty result set also implemented Exception handling
------------------------------------------*/
-- DROP FUNCTION public.fn_get_report_users(varchar, bool);

CREATE OR REPLACE FUNCTION public.fn_get_report_users(p_user_id character varying, p_isselected boolean)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

DECLARE
   sql TEXT;
   userGroupName character varying;
BEGIN
   -- Check if p_user_id is null or empty and return empty result set if true
   IF p_user_id IS NULL OR p_user_id = '' THEN
      RETURN QUERY SELECT row_to_json(row) FROM (SELECT NULL AS user_id, NULL AS user_name, NULL AS name, NULL AS user_email, NULL AS role_name, NULL AS groupUser, NULL AS isSelected, NULL AS mobile_number) row WHERE false;
      RETURN;
   END IF;

   BEGIN
      sql := 'SELECT * from ( select  distinct um.user_id,um.user_name,um.name,um.user_email,rm.role_name,case when
 umm.manager_id in (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)  or
 um.user_id in
  (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number) then ''same'' else ''Others'' end as groupUser,
 case when umm.manager_id in (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)  
 or (select count(1)>0 from user_report_mapping
 where parent_user_id in (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)  and child_user_id = um.user_id)=true
 or um.user_id in (SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)  then true else false end as isSelected,um.mobile_number 
 from user_master um
 left join user_manager_mapping umm on um.user_id = umm.user_id
 inner join
 role_master rm on rm.role_id = um.role_id
 inner join user_permission_area upa1 on upa1.user_id in(SELECT number::INTEGER FROM regexp_split_to_table('''||p_user_id||''', '','') AS number)
 inner join user_permission_area upa on upa.user_id=um.user_id
  and upa.province_id=upa1.province_id and upa.region_id=upa1.region_id
 where   um.role_id <>1 and um.is_Active = true)a';
      IF p_isselected = true THEN
         sql := sql || ' where isSelected = true'; 
      END IF;

      sql := sql || ' ORDER BY isSelected desc,groupuser desc, CASE WHEN user_id in (SELECT number::INTEGER FROM 
regexp_split_to_table('''||p_user_id||''', '','') AS number) THEN ''1'' Else user_name END';

      --RAISE INFO '%', sql;
      RETURN QUERY EXECUTE 'select row_to_json(row) from (' || sql || ')row';

   EXCEPTION
      WHEN others THEN
         -- Insert error details into error_log table
         INSERT INTO public.error_log (
            user_id, server_ip, controller_name, action_name, err_message, err_description, created_on
         ) VALUES (
            0, inet_server_addr(),
            'Report', 'fn_get_report_users()', SQLERRM, 
            'Exception Message: ' || SQLERRM || ' | Exception Type: ' || 'Export Report Functionality', now()
         );

         -- Return empty result set
         RETURN QUERY SELECT row_to_json(row) FROM (SELECT NULL AS user_id, NULL AS user_name, NULL AS name, NULL AS user_email, NULL AS role_name, NULL AS groupUser, NULL AS isSelected, NULL AS mobile_number) row WHERE false;
   END;
END
$function$
;