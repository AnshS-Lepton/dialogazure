update res_resources set value = 'Route Name' where key ilike '%SI_GBL_GBL_GBL_GBL_164%' and culture ='en'

update res_resources set value = 'EE Area:' where key ilike '%SI_OSP_GBL_NET_FRM_545%' and culture ='en'


-------------------------------------------------------------------------------------------------------

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Handhole', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Handhole', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Pole', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Pole', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Manhole', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Manhole', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'WallMount', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'WallMount', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Tree', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Tree', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'POD', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'POD', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);


INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'MPOD', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'MPOD', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);


INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'ONT', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'ONT', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'FDB', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'FDB', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'BDB', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'BDB', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'HTB', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'HTB', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'SpliceClosure', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'SpliceClosure', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);


INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Vault', 'latitude', 'field', true, true, NULL, NULL, NULL, NULL, false);

INSERT INTO public.form_input_settings
( form_name, form_feature_name, form_feature_type, is_active, is_required, created_by,  modified_by, modified_on, feature_description, is_readonly)
VALUES( 'Vault', 'longitude', 'field', true, true, NULL, NULL, NULL, NULL, false);


------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.delete_duplicates_records()
 RETURNS void
 LANGUAGE plpgsql
AS $function$
DECLARE
    v_record RECORD;
BEGIN
    FOR v_record IN 
        SELECT layer_id 
        FROM layer_details 
    LOOP
        WITH cte AS (
            SELECT ctid, db_column_name, layer_id,
                   ROW_NUMBER() OVER (PARTITION BY db_column_name, layer_id ORDER BY ctid) AS row_num
            FROM data_uploader_template
            WHERE layer_id = v_record.layer_id
        )
        DELETE FROM data_uploader_template
        WHERE ctid IN (
            SELECT ctid
            FROM cte
            WHERE row_num > 1
        );
    END LOOP;
END;
$function$
;

-------------------------------------------------------------------------------------------------


insert into cable_color_master(color_character,color_name,color_code,type,created_by,created_on)
values('BLUE','','#0000ff','Core',1,now())
,('ORANGE','','#ffa500','Core',1,now())
,('BLUE-1','','#0000ff','Tube',1,now())
,('ORANGE-2','','#ffa500','Tube',1,now())
,('GREEN-3','','#008000','Tube',1,now())
,('BROWN-4','','#a52a2a','Tube',1,now())
,('SLATE-5','','#808080','Tube',1,now())
,('WHITE-6','','#ffffff','Tube',1,now())
,('RED-7','','#ff0000','Tube',1,now())
,('BLACK-8','','#000000','Tube',1,now())
,('YELLOW-9','','#ffff00','Tube',1,now())
,('VIOLET-10','','#ee82ee','Tube',1,now())
,('ROSE-11','','#ffc0cb','Tube',1,now())
,('AQUA-12','','#00ffff','Tube',1,now())
,('BLUE-13','','#0000ff','Tube',1,now())
,('ORANGE-14','','#ffa500','Tube',1,now())
,('GREEN-15','','#008000','Tube',1,now())
,('BROWN-16','','#a52a2a','Tube',1,now())
,('SLATE-17','','#808080','Tube',1,now())
,('WHITE-18','','#ffffff','Tube',1,now())
,('RED-19','','#ff0000','Tube',1,now())
,('BLACK-20','','#000000','Tube',1,now())
,('YELLOW-21','','#ffff00','Tube',1,now())
,('VIOLET-22','','#ee82ee','Tube',1,now())
,('ROSE-23','','#ffc0cb','Tube',1,now())
,('AQUA-24','','#00ffff','Tube',1,now())
,('GREEN','','#008000','Core',1,now())
,('BROWN','','#a52a2a','Core',1,now())
,('SLATE','','#808080','Core',1,now())
,('WHITE','','#ffffff','Core',1,now())
,('RED','','#ff0000','Core',1,now())
,('BLACK','','#000000','Core',1,now())
,('YELLOW','','#ffff00','Core',1,now())
,('VIOLET','','#ee82ee','Core',1,now())
,('ROSE','','#ffc0cb','Core',1,now())
,('AQUA','','#00ffff','Core',1,now());


, 