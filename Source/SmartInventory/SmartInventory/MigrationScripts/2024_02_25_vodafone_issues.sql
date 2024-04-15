INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'network_id', 'varchar', 'network_id', false, 'Unique row network id', 'ROW000000001', '20', '1', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'row_name', 'varchar', 'row_name', false, 'Name of Row', 'KIA-ROW000000001', '20', '2', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'network_status', 'varchar', 'network_status', false, 'Network status of entity', 'P', '1', '3', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'row_stage', 'varchar', 'row_stage', false, 'Stage of Row', 'New', '50', '4', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'row_type', 'varchar', 'row_type', false, 'Type of Row', 'Line', '50', '5', false);

INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'parent_network_id', 'varchar', 'parent_network_id', false, 'Parent network id of ROW', 'KIA-ARA000004', '20', '7', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'parent_entity_type', 'varchar', 'parent_entity_type', false, 'Parent entity type of ROW', 'Area', '20', '8', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'remarks', 'varchar', 'Remarks', false, 'Remarks', 'Remarks', '500', '9', false);

INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'sp_geometry', 'varchar', 'sp_geometry', true, 'Unknown Line geom', '36.821290618574665 -1.169242318989201,36.82129131581691 -1.169242313068906,36.821865308546094 -1.169301309379887,36.82186771863438 -1.169306009904749,36.82165850633122 -1.169670714345082,36.82165564125943 -1.169672294002846,36.8208938938992 -1.169640114201192,36.82089415978249 -1.16963382031873,36.82165399458803 -1.169665919325052,36.821859842245736 -1.169307080224005,36.82129102068399 -1.169248615417671,36.82102314807395 -1.169280753437544,36.821022397673175 -1.169274498795345,36.821290618574665 -1.169242318989201,36.821290618574665 -1.169242318989201', '100000', '6', false);



INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'requested_by', 'varchar', 'requested_by', false, 'Req By', 'Admin', '50', '10', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'origin_ref_code', 'varchar', 'origin_ref_code', false, 'entity code from where it has been created from', 'A02', '50', '11', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'request_approved_by', 'varchar', 'request_approved_by', false, 'Req Approved By', 'Admin', '20', '12', false);

INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'origin_from', 'varchar', 'origin_from', false, 'Where entity has been created from', 'DU', '20', '14', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'origin_ref_id', 'varchar', 'origin_ref_id', false, 'Origin entity Id', '12', '20', '15', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'origin_ref_description', 'varchar', 'origin_ref_description', false, 'entity description', 'DU', '200', '16', false);
INSERT INTO data_uploader_template (layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, description, example_value, max_length, column_sequence, is_dropdown) VALUES (39, 'request_ref_id', 'varchar', 'request_ref_id', false, 'Req entity has been created from', '19', '20', '17', false);



CREATE TABLE IF NOT EXISTS public.temp_du_row
(
    system_id serial,
    network_id character varying COLLATE pg_catalog."default",
    row_name character varying COLLATE pg_catalog."default",
    row_stage character varying COLLATE pg_catalog."default",	
    row_type character varying COLLATE pg_catalog."default",
    province_id integer DEFAULT 0,
    region_id integer DEFAULT 0,
    created_by integer,
    created_on timestamp without time zone DEFAULT now(),  
    parent_network_id character varying COLLATE pg_catalog."default",
    parent_entity_type character varying COLLATE pg_catalog."default",
    parent_system_id integer DEFAULT 0,  
    is_valid boolean,
    error_msg character varying COLLATE pg_catalog."default",
    upload_id integer,
    batch_id integer,   
    row_order integer DEFAULT 0,
    sp_geometry character varying COLLATE pg_catalog."default",
    is_processed boolean NOT NULL DEFAULT false,
    audit_item_master_id integer,
    remarks character varying COLLATE pg_catalog."default",
    origin_from character varying COLLATE pg_catalog."default",
    origin_ref_id character varying COLLATE pg_catalog."default",
    origin_ref_code character varying COLLATE pg_catalog."default",
    origin_ref_description character varying COLLATE pg_catalog."default",
    request_ref_id character varying COLLATE pg_catalog."default",
    requested_by character varying COLLATE pg_catalog."default",
    request_approved_by character varying COLLATE pg_catalog."default",
    network_status character varying COLLATE pg_catalog."default",
    CONSTRAINT temp_du_row_pkey PRIMARY KEY (system_id)
);

update layer_details set data_upload_table='temp_du_row' where layer_name='ROW';
update layer_details set is_data_upload_enabled=true where layer_name='ROW';

INSERT INTO LAYER_MAPPING (layer_id,parent_layer_id,parent_sequence,is_enable_inside_parent_info,is_used_for_network_id,network_code_format,	is_default_code_format,is_default_parent)
VALUES(39,1,2,FALSE,TRUE,'ARAxxxxxx',true,false);