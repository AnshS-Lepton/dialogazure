CREATE OR REPLACE VIEW public.vw_associate_entity_master
AS
SELECT asm.layer_id,
ld.layer_title AS layer_name,
asm.associate_layer_id,
ld2.layer_title AS associate_layer_name,
asm.layer_subtype,
asm.is_enabled,
asm.is_snapping_enabled,
asm.created_on,
asm.created_by,
asm.modified_on,
asm.modified_by,
ld.geom_type AS layer_geom_type,
ld2.geom_type AS associated_layer_geom_type,
asm.system_id,
um.user_name AS created_by_text,
um1.user_name AS modified_by_text
FROM associate_entity_master asm
JOIN layer_details ld ON ld.layer_id = asm.layer_id
JOIN layer_details ld2 ON ld2.layer_id = asm.associate_layer_id
LEFT JOIN user_master um ON um.user_id = asm.created_by
LEFT JOIN user_master um1 ON um1.user_id = asm.modified_by;

ALTER TABLE public.vw_associate_entity_master
OWNER TO postgres;