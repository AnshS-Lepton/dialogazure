-------------------first run this query----------------------------------
drop view vw_fe_tools_master;
--------------------Second step-----------------------------------------------

ALTER TABLE user_tools_mapping
DROP COLUMN is_accepted;

-----------------------Third Step--------------------------------------------

ALTER TABLE user_tools_mapping
ADD COLUMN is_accepted VARCHAR DEFAULT 'No';

-------------------------Final--------------------------------------------------

CREATE OR REPLACE VIEW public.vw_fe_tools_master
AS SELECT lgm.id,
    um.user_name,
    ftm.tool_name,
    lgm.barcode,
    lgm.serial_number,
    lgm.created_by,
    lgm.created_on,
    lgm.modified_by,
    lgm.modified_on,
    lgm.date_value,
    um1.user_name AS created_by_text,
    um2.user_name AS modified_by_text,
    ( SELECT
                CASE
                    WHEN count(user_tools_attachment.upload_type) > 1 THEN 'Both'::text
                    ELSE max(user_tools_attachment.upload_type::text)
                END AS upload_type
           FROM ( SELECT DISTINCT user_tools_mapping.id
                   FROM user_tools_mapping) all_types
             LEFT JOIN user_tools_attachment ON all_types.id = user_tools_attachment.tools_mapping_id
          WHERE all_types.id = lgm.id
          GROUP BY all_types.id) AS upload_type,
    lgm.user_id,
    lgm.is_accepted
   FROM user_tools_mapping lgm
     JOIN user_master um ON lgm.user_id = um.user_id
     LEFT JOIN user_master um1 ON lgm.created_by = um1.user_id
     LEFT JOIN user_master um2 ON um1.user_id = lgm.modified_by
     LEFT JOIN fe_tools_master ftm ON ftm.id = lgm.tool_id;