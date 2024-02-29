INSERT INTO public.global_settings
("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES('isCDBAttributeEnabled', '1', '', 'Web', false, 'int', 0, 1, 1, now(), null, null, false, true, false, null, null);

------------------------------------------------------------------------------------------------------------------------------------------

-- DROP TABLE public.att_details_cable_cdb;

CREATE TABLE public.att_details_cable_cdb (
	circle_name varchar NULL,
	major_route_name varchar NULL,
	route_id varchar NULL,
	section_name varchar NULL,
	section_id varchar NULL,
	route_category varchar NULL,
	distance float8 NULL,
	fiber_pairs_laid int4 NULL,
	total_used_pair int4 NULL,
	fiber_pairs_used_by_vil int4 NULL,
	fiber_pairs_given_to_airtel int4 NULL,
	fiber_pairs_given_to_others int4 NULL,
	fiber_pairs_free int4 NULL,
	faulty_fiber_pairs int4 NULL,
	start_latitude float8 NULL,
	start_longitude float8 NULL,
	end_latitude float8 NULL,
	end_longitude float8 NULL,
	count_non_vil_tenancies_on_route int4 NULL,
	route_lit_up_date timestamp NULL,
	aerial_km float8 NULL,
	avg_loss_per_km float8 NULL,
	avg_last_six_months_fiber_cut float8 NULL,
	"row" float8 NULL,
	material float8 NULL,
	execution float8 NULL,
	row_availablity float8 NULL,
	iru_given_airtel float8 NULL,
	iru_given_jio float8 NULL,
	iru_given_ttsl_or_ttml float8 NULL,
	network_category varchar NULL,
	row_valid_or_exp timestamp NULL,
	remarks varchar NULL,
	cable_owner varchar NULL,
	route_type varchar NULL,
	operator_type varchar NULL,
	fiber_type varchar NULL,
	cable_id varchar NULL,
	iru_given_tcl float8 NULL,
	iru_given_others float8 NULL,
	id serial4 NOT NULL
);


----------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'Route_Type', 'Inter', true, 1, now(), NULL, NULL, 'Inter', NULL, 'route_type', true, true, 0);

INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'Route_Type', 'Intra', true, 1, now(), NULL, NULL, 'Intra', NULL, 'route_type', true, true, 0);

---------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'Operator_Type_LOV', 'E-Idea', true, 1, now(), NULL, NULL, 'E-Idea', NULL, 'operator_type', true, true, 0);

INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'Operator_Type_LOV', 'E-Vodafone', true, 1, now(), NULL, NULL, 'E-Vodafone', NULL, 'operator_type', true, true, 0);

INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'Operator_Type_LOV', 'VI', true, 1, now(), NULL, NULL, 'VI', NULL, 'operator_type', true, true, 0);

-----------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'Fiber_Type_LOV', 'Ribbon', true, 1, now(), NULL, NULL, 'Ribbon', NULL, 'fiber_type', true, true, 0);

INSERT INTO public.dropdown_master
( layer_id, dropdown_type, dropdown_value, dropdown_status, created_by, created_on, modified_by, modified_on, dropdown_key, parent_value, db_column_name, is_action_allowed, is_active, parent_id)
VALUES( 19, 'Fiber_Type_LOV', 'Single', true, 1, now(), NULL, NULL, 'Single', NULL, 'fiber_type', true, true, 0);

---------------------------------------------------------------------------------------------------------------------------------