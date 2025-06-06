

-------------------------------------------------- Insert resource key--------------------------------------------------

INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('de-DE', 'SI_GBL_GBL_NET_FRM_443', 'Validate Route', false, true, 'German', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:11:28.885', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_GBL_GBL_NET_FRM_443', 'Validate Route', true, true, 'English', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:11:28.885', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en-US', 'SI_GBL_GBL_NET_FRM_443', 'Validate Route', false, true, NULL, 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:11:28.885', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('fr', 'SI_GBL_GBL_NET_FRM_443', 'Validate Route', false, true, 'French', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:11:28.885', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_GBL_GBL_NET_FRM_443', 'Validate Route', false, true, 'Hindi', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:11:28.885', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ja-JP', 'SI_GBL_GBL_NET_FRM_443', 'Validate Route', false, true, 'Japanese', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:11:28.885', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ru-RU', 'SI_GBL_GBL_NET_FRM_443', 'Validate Route', false, true, 'Russian', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:11:28.885', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('de-DE', 'SI_GBL_GBL_NET_FRM_444', 'Delete Route', false, true, 'German', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:12:38.904', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_GBL_GBL_NET_FRM_444', 'Delete Route', true, true, 'English', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:12:38.904', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en-US', 'SI_GBL_GBL_NET_FRM_444', 'Delete Route', false, true, NULL, 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:12:38.904', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('fr', 'SI_GBL_GBL_NET_FRM_444', 'Delete Route', false, true, 'French', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:12:38.904', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_GBL_GBL_NET_FRM_444', 'Delete Route', false, true, 'Hindi', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:12:38.904', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ja-JP', 'SI_GBL_GBL_NET_FRM_444', 'Delete Route', false, true, 'Japanese', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:12:38.904', 5, false, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('ru-RU', 'SI_GBL_GBL_NET_FRM_444', 'Delete Route', false, true, 'Russian', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2025-06-05 12:12:38.904', 5, false, false);

-------------------------------------------------- Create function for route validation--------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_getvalidatetoplogyroute(
	p_agg1 integer,
	p_agg2 integer,
	p_geom character varying,
	p_user_id integer)
    RETURNS boolean
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    v_role_id INTEGER;
    v_result BOOLEAN := false;
    v_geom GEOMETRY;
	agg1_point GEOMETRY ;
    agg2_point GEOMETRY ;
BEGIN
    -- Optional: Fetch role ID
    SELECT role_id INTO v_role_id 
    FROM user_master 
    WHERE user_id = p_user_id;
 
 
 select ST_SetSRID(ST_MakePoint(longitude, latitude), 4326) into agg1_point from att_details_pod adp  where system_id=p_agg1; 
 select ST_SetSRID(ST_MakePoint(longitude, latitude), 4326) into agg2_point from att_details_pod adp  where system_id=p_agg2;
    -- Convert raw coordinate list into proper LINESTRING geometry
    v_geom := ST_GeomFromText('LINESTRING(' || p_geom || ')', 4326);

RAISE INFO 'agg1_point -> %', agg1_point;
RAISE INFO 'agg2_point -> %', agg2_point;
RAISE INFO 'StartPoint -> %', ST_AsText(ST_StartPoint(v_geom));
RAISE INFO 'EndPoint -> %', ST_AsText(ST_EndPoint(v_geom));
RAISE INFO 'v_geom -> %', ST_AsText(v_geom);

   -- Create 2m buffers around start and end points
 IF ST_Within(agg1_point, st_buffer_meters(ST_StartPoint(v_geom), 2))
    and ST_Within(agg2_point, st_buffer_meters(ST_EndPoint(v_geom), 2)) THEN
    v_result := true;
END IF;

    RETURN v_result;
END;
$BODY$;