update LAYER_COLUMNS_SETTINGS set IS_ACTIVE =false where layer_id =(select layer_id FROM LAYER_DETAILS WHERE UPPER(LAYER_NAME)=UPPER('SPLICECLOSURE')) and UPPER(SETTING_TYPE)='HISTORY' and  column_name ='is_plice'
-----------------------------------------------------------------------------------
alter table att_details_coupler add column own_vendor_id character varying(50);
----------------------------------------------------------------------------------------------------
alter table att_details_coupler add column hierarchy_type character varying(50);

------------------------------------------------------------
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'origin_ref_id', 133974, 'Origin Ref Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES( (select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'province_name', 133976, 'Province Name', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES( (select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'mzone_id', 133977, 'Mzone Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'created_by_id', 133978, 'Created By Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'origin_from', 133979, 'Origin From', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'modified_by', 133980, 'Modified By', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, now(), false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'country_name', 133981, 'Country Name', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'served_by_ring', 133982, 'Served By Ring', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'status_updated_by', 133983, 'Status Updated By', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'network_status', 133984, 'Network Status', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'due_date', 133985, 'Due Date', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'area_id', 133986, 'Area Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'province_id', 133987, 'Province Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'remarks', 133988, 'Remarks', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'parent_code', 133989, 'Parent Code', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'surveyarea_name', 133990, 'Surveyarea Name', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, now(), false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'request_ref_id', 133991, 'Request Ref Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'created_on', 133992, 'Created On', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1,null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'status', 133993, 'Status', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'modified_on', 133994, 'Modified On', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);
INSERT INTO public.layer_columns_settings
( layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_duration_based_column, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Report', 'csa_id', 133995, 'Csa Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, false, true, NULL);

INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'origin_ref_id', 134011, 'Origin Ref Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'created_by_id', 134012, 'Created By Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'requested_by', 134013, 'Requested By', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'subarea_id', 134014, 'Subarea Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'jc_id', 134015, 'JC Id', 'vw_att_details_surveyarea_report', false, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'created_on', 134016, 'Created On', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'parent_type', 134017, 'Parent Type', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'status_updated_on', 134018, 'Status Updated On', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'province_name', 134019, 'Province Name', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'status_updated_by', 134020, 'Status Updated By', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'parent_code', 134021, 'Parent Code', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'status_remark', 134022, 'Status Remark', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'country_name', 134023, 'Country Name', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'network_id', 134024, 'Network Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'design_id', 134025, 'Design Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'created_by', 134026, 'Created By', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'mzone_id', 134027, 'Mzone Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'prms_id', 134028, 'PRMS Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'status', 134029, 'Status', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'csa_id', 134030, 'Csa Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'served_by_ring', 134031, 'Served By Ring', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'area_id', 134032, 'Area Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'region_id', 134033, 'Region Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'surveyarea_status', 134034, 'Surveyarea Status', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'province_id', 134035, 'Province Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'request_ref_id', 134036, 'Request Ref Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'request_approved_by', 134037, 'Request Approved By', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'origin_from', 134038, 'Origin From', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'is_visible_on_map', 134039, 'Is Visible On Map', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'due_date', 134040, 'Due Date', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'origin_ref_description', 134041, 'Origin Ref Description', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'modified_by', 134042, 'Modified By', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'origin_ref_code', 134043, 'Origin Ref Code', 'vw_att_details_surveyarea_report', true, false, 1,now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'remarks', 134044, 'Remarks', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'modified_on', 134045, 'Modified On', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'surveyarea_name', 134046, 'Surveyarea Name', 'vw_att_details_surveyarea_report', true, false, 1,now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'region_name', 134047, 'Region Name', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'network_status', 134048, 'Network Status', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
INSERT INTO public.layer_columns_settings
(layer_id, setting_type, column_name, column_sequence, display_name, table_name, is_active, is_required, created_by, created_on, modified_by, modified_on, is_kml_column_required, res_field_key)
VALUES((select layer_id from layer_details where layer_name ='SurveyArea'), 'Info', 'dsa_id', 134049, 'Dsa Id', 'vw_att_details_surveyarea_report', true, false, 1, now(), 1, null, true, NULL);
----------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_uploader_insert_cable(p_upload_id integer, p_batch_id integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$

		declare
		result int:=0;
		cnt int:=0;
		texttoappend character varying;
		REC RECORD;
		current_system_id integer;
		v_parent_system_id integer;
		v_parent_network_id character varying;
		v_parent_entity_type character varying;
		v_network_code character varying;
		v_sequence_id integer;
		v_geom character varying;
		v_entity_name character varying;
		v_network_Id_type character varying;
		v_network_code_seperator character varying;
		v_a_display_value character varying;
		v_b_display_value character varying;
		v_auto_character_count integer;
		v_network_name character varying;
		v_a_system_id integer;
		v_a_entity_type character varying;
		v_b_system_id integer;
		v_b_entity_type character varying;
		v_structure_id integer;
		v_a_network_id character varying;
		v_b_network_id character varying;
		v_cable_a_primary_table character varying;
		v_cable_a_secondary_table character varying;
		v_cable_b_primary_table character varying;
		v_cable_b_secondary_table character varying;
		v_a_connectivity character varying;
		v_b_connectivity character varying;
		sql text;
		BEGIN
		select entity_type into v_entity_name from upload_summary where id=p_upload_id;
		select network_id_type,network_code_seperator into v_network_Id_type,v_network_code_seperator from layer_details where upper(layer_name)=upper(v_entity_name);

		FOR REC IN select * from temp_du_cable t
		--inner join kml_attributes k on upper(k.cable_name)=upper(t.cable_name) and t.upload_id=k.uploaded_id
		where upload_id=p_upload_id and is_valid=true and batch_id=p_batch_id
		LOOP
		BEGIN
		-- v_geom:=replace(replace(rec.sp_geometry,'LINESTRING(',''),')','');
		-- v_geom:=replace(replace(rec.sp_geometry,'LINESTRING (',''),')','');

		v_geom:=replace(replace(upper(rec.sp_geometry),'LINESTRING(',''),')','');
		v_geom:=replace(v_geom,'LINESTRING (','');
		v_geom:=replace(v_geom,'LINESTRINGZ (','');
		v_geom:=replace(v_geom,'LINESTRINGZ(','');

		--v_geom:=replace(v_geom,'LINESTRING (','');
		v_network_name:=rec.cable_name;
		v_sequence_id:=0;

		-- IF MANUAL THEN ACCEPT THE NETWORK CODE ENTERED BY USER.
		if (v_network_id_type='M' and coalesce(REC.network_id,'')!='')
		then
		v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		elsif((v_network_id_type='A' and coalesce(REC.network_id,'')!=''))
		then

		v_network_code=rec.parent_network_id||v_network_code_seperator||rec.network_id;
		select (length(network_code_format)-length(replace(network_code_format,RIGHT(network_code_format, 1),''))) into v_auto_character_count
		from vw_layer_mapping where upper(child_layer_name)=upper('Cable') and upper(parent_layer_name)=UPPER(REC.PARENT_ENTITY_TYPE) and is_used_for_network_id=true;
		v_sequence_id:=RIGHT(rec.network_id, v_auto_character_count);
		else
		-- GET NETWORK CODE & PARENT DETAILS..
		-- select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
		-- from fn_get_clone_network_code('Cable', 'Line',''||v_longitude||' '||v_latitude||'', rec.parent_system_id, rec.parent_entity_type) into
		-- v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;
		raise info 'REC.network_id%',v_geom;
		raise info 'rec.parent_entity_type%',rec.parent_entity_type;
		select o_p_system_id,o_p_network_id,o_p_entity_type,message,o_sequence_id
		from fn_get_clone_network_code('Cable', 'Line',''||v_geom||'', rec.parent_system_id, rec.parent_entity_type) into
		v_parent_system_id,v_parent_network_id,v_parent_entity_type,v_network_code,v_sequence_id;

		end if;

		IF(coalesce(v_network_name,'')='')
		then
		v_network_name=v_network_code;
		END IF;

		RAISE INFO 'v_parent_system_id : %',v_parent_system_id;
		RAISE INFO 'v_parent_network_id : %',v_parent_network_id;
		RAISE INFO 'v_parent_entity_type : %',v_parent_entity_type;
		RAISE INFO 'v_network_code : %',v_network_code;
		RAISE INFO 'v_sequence_id : %',v_sequence_id;

		If(upper(coalesce(v_parent_network_id, ''))!= 'NLD')
		then
		v_parent_network_id=rec.parent_network_id;
		END IF;
		--INSERT INTO MAIN TABLE

		raise info 'Result of rec :%',rec.origin_ref_id;
		if(rec.origin_ref_id is not null and rec.origin_from ='SP') then 
		if(rec.cable_type ='ISP') then
		Select  adb2.system_id as a_system_id,adb2.network_id, 'BDB' as a_entity_type ,
				ifi2.system_id as b_system_id ,ifi2.network_id ,'FDB' as b_entity_type,ifi2.parent_system_id 
				into v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type,v_structure_id
				from 
				(select * from temp_du_cable where system_id =rec.system_id) cable 
		inner join att_details_splitter adb   
		on cast(cable.a_end as character varying) = adb.origin_Ref_id 
		inner join att_details_splitter ifi 
		on cast(cable.b_end as character varying) = ifi.origin_ref_id
		inner join att_details_bdb adb2 
		on adb2.system_id = adb.parent_system_id 
		inner join isp_fdb_info ifi2  
		on ifi2.system_id = ifi.parent_system_id order by 1 desc,4 desc  limit 1;
		else
		raise info 'Processed';
		 if(rec.a_entity_type ='BDB') then 
		 
		 v_cable_a_primary_table :='att_details_splitter';
		 v_cable_a_secondary_table := 'att_details_bdb';
		v_a_connectivity :='on a_secondary.system_id = a_primary.parent_system_id' ;
		elseif (rec.a_entity_type='FMS') then
		 
		 v_cable_a_primary_table :='att_details_pod';
		 v_cable_a_secondary_table := 'att_details_fms';
		v_a_connectivity :='on a_secondary.parent_system_id = a_primary.system_id' ;
		end if;
		if ((rec.b_entity_type ='BDB')) then 

		v_cable_b_primary_table :='att_details_splitter';
		 v_cable_b_secondary_table := 'att_details_bdb';
		v_b_connectivity :='on b_secondary.system_id = b_primary.parent_system_id' ;
		elseif ((rec.b_entity_type ='FMS')) then 
		v_cable_b_primary_table :='att_details_pod';
		 v_cable_b_secondary_table := 'att_details_fms';
		v_b_connectivity :='on b_secondary.parent_system_id = b_primary.system_id' ;
		end if;
		raise info 'Processed :% ,:%, :% ,:%',v_cable_a_primary_table,v_cable_b_primary_table,v_cable_a_secondary_table,v_cable_b_secondary_table;
		sql := 'Select  a_secondary.system_id as a_system_id,a_secondary.network_id, cable.a_entity_type ,
				b_secondary.system_id as b_system_id ,b_secondary.network_id ,cable.b_entity_type
				from 
				(select * from temp_du_cable where system_id ='||rec.system_id ||') cable 
		inner join '|| v_cable_a_primary_table ||' a_primary   
		on cast(cable.a_end as character varying) = a_primary.origin_Ref_id 
		inner join '|| v_cable_b_primary_table ||' b_primary 
		on cast(cable.b_end as character varying) = b_primary.origin_ref_id
		inner join '|| v_cable_a_secondary_table || ' a_secondary ' ||
		v_a_connectivity ||'
		inner join '|| v_cable_b_secondary_table ||' b_secondary  
		' ||
		v_b_connectivity ||' order by 1 desc,4 desc  limit 1;';
		 raise info 'Sql:% ',sql;
		 execute sql  into v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type;
		v_structure_id :=0;
		end if;
		raise info 'Values :%,:%,:%,:%,%,%',v_a_system_id,v_a_network_id, v_a_entity_type, v_b_system_id,v_b_network_id, v_b_entity_type;
		INSERT INTO public.att_details_cable(network_id, cable_name, a_location, b_location, total_core,
		no_of_tube, no_of_core_per_tube,
		cable_measured_length, cable_calculated_length,
		cable_type, specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id, network_status,
		status,province_id,
		region_id,created_by,created_on, a_system_id,a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type,
		cable_category,cable_sub_category,route_id, parent_system_id, parent_network_id, parent_entity_type,sequence_id,
		type,brand,model,construction,activation,accessibility,duct_id,utilization,ownership_type, source_ref_type,
		source_ref_id, remarks,audit_item_master_id,origin_from,origin_Ref_id,origin_Ref_code,
		origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,structure_id,
		section_name, generic_section_name,hierarchy_type,aerial_location)

		SELECT v_network_code,v_network_name, v_a_network_id, v_b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
		rec.cable_calculated_length,coalesce(cable_type,'Overhead'), specification, category,subcategory1, subcategory2,
		subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),v_a_system_id,
		v_a_network_id,
		v_a_entity_type,v_b_system_id,v_b_network_id,v_b_entity_type,cable_category,sub_category,
		route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
		p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',v_structure_id,
		section_name, generic_section_name,hierarchy_type,aerial_location
		from temp_du_cable where system_id=rec.system_id
		returning system_id into current_system_id;
		else
		INSERT INTO public.att_details_cable(network_id, cable_name, a_location, b_location, total_core,no_of_tube, no_of_core_per_tube,
		cable_measured_length, cable_calculated_length,
		cable_type, specification, category, subcategory1, subcategory2, subcategory3, item_code, vendor_id, network_status, status,province_id,
		region_id,created_by,created_on, a_system_id,a_network_id, a_entity_type, b_system_id, b_network_id, b_entity_type,
		cable_category,cable_sub_category,route_id, parent_system_id, parent_network_id, parent_entity_type,sequence_id,
		type,brand,model,construction,activation,accessibility,duct_id,utilization,ownership_type, source_ref_type,
		source_ref_id, remarks,audit_item_master_id,origin_from,origin_Ref_id,origin_Ref_code,
		origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,section_name, generic_section_name,hierarchy_type,aerial_location,a_location_code,b_location_code)
		SELECT v_network_code,v_network_name, a_network_id, b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
		rec.cable_calculated_length,coalesce(cable_type,'Overhead'), specification, category,subcategory1, subcategory2,
		subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),a_system_id,a_network_id,
		a_entity_type,b_system_id,b_network_id,b_entity_type,cable_category,sub_category,
		route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
		p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
		rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',
		section_name, generic_section_name,hierarchy_type,aerial_location,a_location_code,b_location_code
		from temp_du_cable where system_id=rec.system_id
		returning system_id into current_system_id;
		end if;

		select display_name into v_a_display_value from point_master where system_id=REC.a_system_id and upper(entity_type)=upper(REC.a_entity_type);
		select display_name into v_b_display_value from point_master where system_id=REC.b_system_id and upper(entity_type)=upper(REC.b_entity_type);

		--INSERT INTO LINE MASTER

		if (coalesce(rec.cable_type,'') !='ISP') then 
		Raise info '------------------------------------rec.cable_type123456%',rec.cable_type;
		INSERT INTO line_master(system_id, entity_type, approval_flag,sp_geometry,approval_date,creator_remark,approver_remark, created_by,
		approver_id, common_name, db_flag, network_status,display_name)
		values(current_system_id, 'Cable','A',st_geomfromtext(rec.sp_geometry,4326),
		now(), 'NA', 'NA', rec.created_by,0, v_network_code, p_upload_id, coalesce(rec.network_status,'P'),fn_get_display_name(current_system_id, 'Cable'));
		--from temp_du_cable t join kml_attributes k on upper(k.cable_name)=upper(k.cable_name) and upload_id=k.uploaded_id
		--where t.system_id = rec.system_id and t.upload_id=p_upload_id;

		end if;

		perform(fn_isp_create_OSP_Cable(current_system_id));
		perform(fn_cable_set_end_point(current_system_id));
		perform fn_set_cable_color_info(current_system_id, rec.created_by, rec.no_of_tube, rec.core_per_tube);
		PERFORM (FN_CREATE_ROUTE_ID(current_system_id,'Cable',0,'Cable'));
		perform (fn_geojson_update_entity_attribute(current_system_id::integer,'Cable'::character varying ,rec.province_id::integer,0,false));

		insert into associate_entity_info(entity_system_id,entity_type,entity_network_id,entity_display_name,associated_system_id,associated_entity_type,associated_network_id,associated_display_name,created_by,created_on,is_termination_point)
		values(current_system_id,'Cable',v_network_code,fn_get_display_name(current_system_id, 'Cable'),REC.a_system_id,REC.a_entity_type,REC.a_network_id,v_a_display_value,rec.created_by,now(),true),
		(current_system_id,'Cable',v_network_code,fn_get_display_name(current_system_id, 'Cable'),REC.b_system_id,REC.b_entity_type,REC.b_network_id,v_b_display_value,rec.created_by,now(),true);

		insert into att_details_cable_cdb (circle_name, major_route_name, route_id, section_name, section_id, route_category, distance, fiber_pairs_laid, total_used_pair, fiber_pairs_used_by_vil, fiber_pairs_given_to_airtel, fiber_pairs_given_to_others, fiber_pairs_free, faulty_fiber_pairs, start_latitude, start_longitude, end_latitude, end_longitude, count_non_vil_tenancies_on_route, route_lit_up_date, aerial_km, avg_loss_per_km, avg_last_six_months_fiber_cut, row, material, execution, row_availablity, iru_given_airtel, iru_given_jio, iru_given_ttsl_or_ttml, network_category, row_valid_or_exp, remarks, cable_owner, route_type, operator_type, fiber_type, cable_id, iru_given_tcl, iru_given_others)
		select circle_name, major_route_name, route_id, section_name, section_id, route_category, distance, fiber_pairs_laid, total_used_pair, fiber_pairs_used_by_vil, fiber_pairs_given_to_airtel, fiber_pairs_given_to_others, fiber_pairs_free, faulty_fiber_pairs, start_latitude, start_longitude, end_latitude, end_longitude, count_non_vil_tenancies_on_route, route_lit_up_date, aerial_km, avg_loss_per_km, avg_last_six_months_fiber_cut, row, material, execution, row_availablity, iru_given_airtel, iru_given_jio, iru_given_ttsl_or_ttml, network_category, row_valid_or_exp, remarks, cable_owner, route_type, operator_type, fiber_type, (SELECT att.system_id FROM  att_details_cable AS att JOIN  temp_du_att_details_cable_cdb AS tempcdb ON  att.network_id  = tempcdb.cable_id WHERE  tempcdb.cable_id = v_network_code  AND tempcdb.upload_id = p_upload_id) as cable_id, iru_given_tcl, iru_given_others from temp_du_att_details_cable_cdb where cable_id = v_network_code and upload_id = p_upload_id and is_valid=true;
		raise info '--------------network id :%',v_network_code;

		END;
		END LOOP;

		return 1;

		END;
		
$function$
;
---------------------------------------------------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION public.fn_get_parent_type(p_parenttype character varying)
 RETURNS character varying
 LANGUAGE plpgsql
AS $function$
  begin  
	
	if exists(select layer_title from layer_details where upper(layer_name)=upper(p_parenttype))
	then
		select layer_title into p_parenttype from layer_details where upper(layer_name)=upper(p_parenttype);
	end if;

 return p_parenttype;
  end;
  $function$
;
----------------------------------------------------------------------------------------------------------
 drop view public.vw_att_details_rack_report ;
 -------------------------------------------------------
 CREATE OR REPLACE VIEW public.vw_att_details_rack_report
AS SELECT rack.system_id,
    rack.network_id,
    rack.rack_name,
    rack.rack_type,
    round(rack.length::numeric, 2) AS length,
    round(rack.width::numeric, 2) AS width,
    round(rack.height::numeric, 2) AS height,
    round(rack.border_width::numeric, 2) AS border_width,
    rack.no_of_units,
    round(rack.latitude::numeric, 6) AS latitude,
    round(rack.longitude::numeric, 6) AS longitude,
    rack.parent_network_id,
    fn_get_parent_type(rack.parent_entity_type) as parent_entity_type,
    COALESCE(fn_get_entity_status(rack.status), rack.status) AS status,
    rack.pos_x,
    rack.pos_y,
    rack.pos_z,
    rack.item_code,
    rack.specification,
    rack.category,
    rack.subcategory1,
    rack.subcategory2,
    rack.subcategory3,
    rack.sequence_id,
    prov.province_name,
    reg.region_name,
    fn_get_network_status(rack.network_status) AS network_status,
    um.user_name AS created_by_user,
    um2.user_name AS modified_by_user,
    fn_get_date(rack.created_on) AS created_on,
    fn_get_date(rack.modified_on) AS modified_on,
    vm.name AS vendor_name,
    rack.is_visible_on_map,
    rack.remarks,
    rack.region_id,
    rack.province_id,
    rack.created_by AS created_by_id,
    reg.country_name,
    rack.origin_from,
    rack.origin_ref_id,
    rack.origin_ref_code,
    rack.origin_ref_description,
    rack.request_ref_id,
    rack.requested_by,
    rack.request_approved_by,
    rack.subarea_id,
    rack.area_id,
    rack.dsa_id,
    rack.csa_id,
    rack.bom_sub_category,
    rack.gis_design_id AS design_id,
    rack.st_x,
    rack.st_y,
    rack.ne_id,
    rack.prms_id,
    rack.jc_id,
    rack.mzone_id,
    rack.served_by_ring
   FROM att_details_rack rack
     JOIN province_boundary prov ON rack.province_id = prov.id
     JOIN region_boundary reg ON rack.region_id = reg.id
     JOIN user_master um ON rack.created_by = um.user_id
     JOIN vendor_master vm ON rack.vendor_id = vm.id
     LEFT JOIN user_master um2 ON rack.modified_by = um2.user_id;

