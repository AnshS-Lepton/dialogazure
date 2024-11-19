-- public.vw_att_details_row_report source

CREATE OR REPLACE VIEW public.vw_att_details_row_report
AS SELECT enrow.system_id,
    enrow.network_id,
    enrow.row_type,
    enrow.row_name,
    reg.region_name,
    prov.province_name,
    enrow.parent_network_id AS parent_code,
    enrow.parent_entity_type AS parent_type,
    enrow.remarks,
    fn_get_date(enrow.created_on) AS created_on,
    fn_get_date(enrow.modified_on) AS modified_on,
    um.user_name AS created_by,
    um2.user_name AS modified_by,
    enrow.region_id,
    enrow.province_id,
    enrow.row_stage,
    poly.buffer_width AS row_width,
    st_length(poly.center_line_geom::geography, false)::numeric(30,2) AS row_length,
    (poly.buffer_width * st_length(poly.center_line_geom::geography, false))::numeric(30,2) AS row_area,
    _row.road_name,
    _row.radius,
    _row.route_length,
    _row.supervision,
    _row.no_of_hh,
    _row.ri_rate::numeric(30,2) AS ri_rate,
    _row.ri_amount::numeric(30,2) AS ri_amount,
    _row.local_management::numeric(30,2) AS local_management,
    _row.dit_liaisoning::numeric(30,2) AS dit_liaisoning,
    _row.row_liaisoning::numeric(30,2) AS row_liaisoning,
    _row.otm_liasoning::numeric(30,2) AS otm_liasoning,
    _row.autocad_drawing::numeric(30,2) AS autocad_drawing,
    _row.total_amount AS applied_total_amount,
    _row.start_point,
    _row.end_point,
    _row.cl_rly_fly_brg_laisioning::numeric(30,2) AS cl_rly_fly_brg_laisioning,
    _row.cl_ft_brg_laisioning::numeric(30,2) AS cl_ft_brg_laisioning,
    _row.refundable_amount::numeric(30,2) AS refundable_amount,
    _row.non_refundable_amount::numeric(30,2) AS non_refundable_amount,
    _row.mcgm_ward,
    _row.applied_pit,
    _row.pit_charges,
    _row.access_charges::numeric(30,2) AS access_charges,
    _row.considered_dlp_factor::numeric(30,2) AS considered_dlp_factor,
    _row.starta_type,
    um3.user_name AS applied_by,
    fn_get_date(_row.apply_date, false) AS apply_date,
        CASE
            WHEN upper(enrow.row_type::text) = upper('PIT'::text) THEN _row.row_area
            ELSE NULL::double precision
        END AS total_pit_area,
    _row.budget_material_cost::numeric(30,2) AS budget_material_cost,
    _row.budget_contractor_cost::numeric(30,2) AS budget_contractor_cost,
    _row.no_of_pulled_cables,
    _row.cable_pulling_rate,
    _row.bdotm_otm_trenching_ducting_rate,
    _row.bdotm_otm_trenching_ducting_length,
    _row.bdotm_otm_ri_rate,
    _row.bdotm_otm_ri_length,
    _row.bdotm_otm_liasioning_rate,
    _row.bdotm_otm_liasioning_length,
    _row.bdotl_otl_trenching_ducting_rate,
    _row.bdotl_otl_trenching_ducting_length,
    _row.bdotl_otl_ri_rate,
    _row.bdotl_otl_ri_length,
    _row.cl_rly_fly_brg_length,
    _row.cl_ft_brg_length,
    _row.handhole_rate,
    row_stage.demand_note,
    row_stage.row_status,
    row_stage.actual_row_route::numeric(30,2) AS actual_row_route,
    row_stage.dlp_factor::numeric(30,2) AS dlp_factor,
    row_stage.actual_rate::numeric(30,2) AS actual_rate,
    row_stage.actual_deposit_amount::numeric(30,2) AS actual_deposit_amount,
    row_stage.actual_ri_amount::numeric(30,2) AS actual_ri_amount,
    row_stage.actual_access_charge_amount::numeric(30,2) AS actual_access_charge_amount,
    row_stage.actual_refundable_amount::numeric(30,2) AS actual_refundable_amount,
    row_stage.penalty_amount::numeric(30,2) AS penalty_amount,
    row_stage.total_paid_amount::numeric(30,2) AS total_paid_amount,
    fn_get_date(row_stage.payment_date, false) AS payment_date,
    fn_get_date(row_stage.start_date, false) AS start_date,
    fn_get_date(row_stage.end_date, false) AS end_date,
    fn_get_date(row_stage.access_charges_start_date, false) AS access_charges_start_date,
    fn_row_get_access_charge_due_date(row_stage.row_system_id) AS recurring_access_charge_due_date,
    date_part('year'::text, age(now(), row_stage.access_charges_start_date::timestamp with time zone)) * row_stage.actual_access_charge_amount AS total_recurring_charge,
        CASE
            WHEN (EXISTS ( SELECT 1
               FROM att_details_row_associate_entity_info ass
              WHERE ass.parent_system_id = enrow.system_id)) THEN 'Yes'::text
            ELSE 'No'::text
        END AS is_entity_associated,
    um.user_id AS created_by_id,
    pm.project_name,
    pm.project_code,
    plningm.planning_name,
    plningm.planning_code,
    workorm.workorder_name,
    workorm.workorder_code,
    purposem.purpose_name,
    purposem.purpose_code,
    enrow.status_remark,
    enrow.status_updated_by,
    fn_get_date(enrow.status_updated_on) AS status_updated_on,
    enrow.is_visible_on_map,
    reg.country_name,
    enrow.origin_from,
    enrow.origin_ref_id,
    enrow.origin_ref_code,
    enrow.origin_ref_description,
    enrow.request_ref_id,
    enrow.requested_by,
    enrow.request_approved_by,
    enrow.gis_design_id AS design_id,
    enrow.ne_id,
    enrow.prms_id,
    enrow.jc_id,
    enrow.mzone_id,
    enrow.served_by_ring
   FROM att_details_row enrow
     JOIN province_boundary prov ON enrow.province_id = prov.id
     JOIN region_boundary reg ON enrow.region_id = reg.id
     LEFT JOIN user_master um ON enrow.created_by = um.user_id
     LEFT JOIN user_master um2 ON enrow.modified_by = um2.user_id
     LEFT JOIN att_details_row_apply _row ON _row.row_system_id = enrow.system_id
     LEFT JOIN user_master um3 ON _row.created_by = um3.user_id
     LEFT JOIN att_details_row_approve_reject row_stage ON row_stage.row_system_id = enrow.system_id
     LEFT JOIN user_master um4 ON row_stage.created_by = um4.user_id
     LEFT JOIN polygon_master poly ON poly.system_id = enrow.system_id AND upper(poly.entity_type::text) = upper('ROW'::text)
     LEFT JOIN att_details_project_master pm ON enrow.project_id = pm.system_id
     LEFT JOIN att_details_planning_master plningm ON enrow.planning_id = plningm.system_id
     LEFT JOIN att_details_workorder_master workorm ON enrow.workorder_id = workorm.system_id
     LEFT JOIN att_details_purpose_master purposem ON enrow.purpose_id = purposem.system_id
     LEFT JOIN att_details_csa csa ON csa.system_id = enrow.system_id;
	 
-------------------
UPDATE public.res_resources SET value='kann nicht in Dormant umgewandelt werden, da es beim Spleißen verwendet wird' WHERE culture='de-DE' AND "key"='SI_GBL_GBL_GBL_GBL_141';
UPDATE public.res_resources SET value='can not convert into dormant,as it is used in splicing' WHERE culture='en' AND "key"='SI_GBL_GBL_GBL_GBL_141';
UPDATE public.res_resources SET value='can not convert into dormant,as it is used in splicing' WHERE culture='ja-JP' AND "key"='SI_GBL_GBL_GBL_GBL_141';

UPDATE public.res_resources SET value='kann nicht in „dormant“ umgewandelt werden, da es mit einer Entität verknüpft ist!' WHERE culture='de-DE' AND "key"='SI_GBL_GBL_GBL_GBL_142';
UPDATE public.res_resources SET value='can not convert into dormant,as it is associated with some entity!' WHERE culture='en' AND "key"='SI_GBL_GBL_GBL_GBL_142';
UPDATE public.res_resources SET value='can not convert into dormant,as it is associated with some entity!' WHERE culture='ja-JP' AND "key"='SI_GBL_GBL_GBL_GBL_142';