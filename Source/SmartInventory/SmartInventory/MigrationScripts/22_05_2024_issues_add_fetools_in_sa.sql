INSERT INTO public.user_module_mapping
(user_id, module_id, created_by, created_on, modified_by, modified_on)
VALUES( (select user_id  from user_master where user_name ='sa'), (select id from module_master mm  where lower(module_name) =lower('FE Tools')), 1, now(), 0, NULL);
--------------------------------------------------------------------------------------------------------------------------------------------------------------
INSERT INTO public.role_module_mapping
( role_id, module_id)
VALUES( (select  role_id from role_master where  lower(role_name)='super admin'), (select id from module_master mm  where lower(module_name) =lower('FE Tools')));

---------------