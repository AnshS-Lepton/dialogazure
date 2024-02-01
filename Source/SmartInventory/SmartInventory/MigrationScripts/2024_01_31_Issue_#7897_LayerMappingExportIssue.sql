drop view vw_layer_group_mapping;

CREATE OR REPLACE VIEW public.vw_layer_group_mapping
AS SELECT lm.mapping_id,
    lm.layer_id,
    ch.layer_name,
    lgm.group_name,
    lgm.group_description,
    lm.group_id,
    lm.created_by,
    lm.created_on,
    lm.modified_by,
    lm.modified_on,
    um.user_name AS created_by_text,
    um1.user_name AS modified_by_text,
    lm.layer_seq,
    ch.layer_title,
    ch.layer_table,
    ch.category_name,
    ch.type_name
   FROM layer_group_mapping lm
     LEFT JOIN layer_details ch ON lm.layer_id = ch.layer_id
     LEFT JOIN layer_group_master lgm ON lm.group_id = lgm.group_id
     LEFT JOIN user_master um ON lm.created_by = um.user_id
     LEFT JOIN user_master um1 ON um1.user_id = lm.modified_by
  WHERE (ch.isvisible = true OR ch.is_visible_in_ne_library = true) AND ch.is_osp_layer = true;