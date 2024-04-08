CREATE OR REPLACE FUNCTION public.fn_uploader_get_template_sample_records(p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$ 
declare v_columns character varying;
v_values character varying;
v_layer_title character varying;
BEGIN

select layer_title into v_layer_title from  layer_details ld  where upper(ld.layer_name)=upper(p_entity_type);
p_entity_type:=upper(p_entity_type);
select string_agg( 'null as "'||template_column_name||'"',','),string_agg(''''||example_value||'''',',') into v_columns,v_values dutemp
from(select template_column_name,example_value from data_uploader_template dutemp 
left join layer_details ld on dutemp.layer_id=ld.layer_id where upper(ld.layer_name)=upper(p_entity_type)  and  dutemp.db_column_name not in ('client_id', 'parent_client_id')
and  dutemp.is_enable_for_template_column=true and is_cdb_attributes = false
--and dutemp.is_excel_attribute=true 
order by column_sequence) a; 

RAISE INFO 'v_columns:%',v_columns;

RETURN QUERY
execute 'select row_to_json(row) from (select *,''Very IMP: Do not Delete/Update any column header in '||v_layer_title||'_Details Sheet'' as Heading1,
''                               GUIDELINE FOR FILLING '||v_layer_title||' DETAILS                               '' as Heading2 from (select '||v_columns||' union select '||v_values||')a limit 1) row';


END ;
$function$
;


----------------------------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_uploader_get_template_cdb_attributes_records(p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$ 
declare 
v_columns character varying;
v_values character varying;
BEGIN

select string_agg( 'null as "'||template_column_name||'"',','),string_agg(''''||example_value||'''',',') into v_columns,v_values dutemp
from(select template_column_name,example_value from data_uploader_template dutemp 
left join layer_details ld on dutemp.layer_id=ld.layer_id where upper(ld.layer_name)=upper(p_entity_type)  and  dutemp.db_column_name not in ('client_id', 'parent_client_id')
and  dutemp.is_enable_for_template_column=true and is_cdb_attributes = true
--and dutemp.is_excel_attribute=true 
order by column_sequence) a; 

RAISE INFO 'v_columns:%',v_columns;

RETURN QUERY
execute 'select row_to_json(row) from (select * from (select '||v_columns||' union select '||v_values||')a limit 1) row';


END ;
$function$
;


----------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_uploader_get_entity_template_for_cdb_attributes(p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$ 
BEGIN

RETURN QUERY
select row_to_json(row) from (select dutemp.id, ld.layer_id,ld.layer_name,db_column_name,db_column_data_type,column_sequence,template_column_name,is_mandatory,max_length,is_dropdown,is_nullable,is_template_column_required,default_value,min_value,max_value from data_uploader_template dutemp 
left join layer_details ld on dutemp.layer_id=ld.layer_id where upper(ld.layer_name)=upper(p_entity_type) 
and  dutemp.is_enable_for_template_column=true and is_cdb_attributes = true
--and dutemp.is_kml_attribute=false 
order by column_sequence
) row;

END ;
$function$
;


------------------------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_uploader_validation_cdb_attributes(p_upload_id integer)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
BEGIN
    UPDATE temp_du_att_details_cable_cdb t1 
    SET error_msg = 'duplicate records exists', is_valid = false
    FROM (
        SELECT cable_id
        FROM temp_du_att_details_cable_cdb
        WHERE upload_id = p_upload_id AND is_valid = true
        GROUP BY cable_id
        HAVING COUNT(*) > 1
    ) AS sub_cdb
    WHERE t1.upload_id = p_upload_id AND t1.is_valid = true AND t1.cable_id = sub_cdb.cable_id;
   
update temp_du_cable tb set is_valid = false,error_msg = 'Duplicate records exists in cdb attributes' where upload_id = p_upload_id and network_id  in (select (string_to_array(cable_id, '-'))[2] from temp_du_att_details_cable_cdb tt where tt.upload_id = p_upload_id and tt.is_valid =false );
   
END;
$function$
;

-----------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_uploader_get_template_guideline(p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
declare v_columns character varying;
v_values character varying;
BEGIN

RETURN QUERY
select row_to_json(row) from (
select template_column_name as "Field Name",description as "Description",display_column_data_type as "Data Type", case when is_mandatory then 'YES' else 'NO' end as "Is Mandatory",example_value as "Example value",default_value as "Default value",max_length as "Max length",min_value as "Min value",max_value as "Max value" from data_uploader_template dutemp
left join layer_details ld on dutemp.layer_id=ld.layer_id where upper(ld.layer_name)=upper(p_entity_type) and dutemp.db_column_name not in ('client_id', 'parent_client_id') and dutemp.is_cdb_attributes =false
--and is_kml_attribute=false
order by column_sequence
) row;


END ;
$function$
;

--------------------------------------------------------------------------------------------------------------------



CREATE TABLE public.temp_du_att_details_cable_cdb (
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
	count_non_vil_tenancies_on_route varchar NULL,
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
	id serial4 NOT NULL,
	upload_id int4 NULL,
	created_by int4 NULL,
	batch_id int4 NULL,
	error_msg varchar NULL,
	is_valid bool NULL DEFAULT true,
	uploaded_on timestamp NULL DEFAULT now()
);



--------------------------------------------------------------------------------------------------------------------


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
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category,structure_id)

SELECT v_network_code,v_network_name, v_a_network_id, v_b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),v_a_system_id,
v_a_network_id,
v_a_entity_type,v_b_system_id,v_b_network_id,v_b_entity_type,cable_category,sub_category,
route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed',v_structure_id
--case when v_structure_id is null or v_structure_id ='' then 
--0 --else v_structure_id end
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
origin_Ref_description,request_ref_id,requested_by,request_approved_by,bom_sub_category)
SELECT v_network_code,v_network_name, a_network_id, b_network_id,total_core,no_of_tube,core_per_tube,cable_measured_length,
rec.cable_calculated_length,cable_type, specification, category,subcategory1, subcategory2,
subcategory3,item_code, vendor_id,coalesce(rec.network_status,'P'),'A',province_id, region_id,created_by, now(),a_system_id,a_network_id,
a_entity_type,b_system_id,b_network_id,b_entity_type,cable_category,sub_category,
route_id,rec.parent_system_id, v_parent_network_id,REC.PARENT_ENTITY_TYPE,v_sequence_id,0,0,0,0,0,0,0,'L','Own', 'DU',
p_upload_id, rec.remarks, rec.audit_item_master_id,rec.origin_from,rec.origin_Ref_id,
rec.origin_Ref_code,rec.origin_Ref_description,rec.request_ref_id,rec.requested_by,rec.request_approved_by,'Proposed'
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


---------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'circle_name', 'varchar', 'Circle_Name', false, NULL, 'Circle Name', 'GGN', 1, 100, 1,now(), NULL, NULL, false, 'character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
(layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'major_route_name', 'varchar', 'Major_Route_Name', false, NULL, 'Major Route Name', 'Pan Punjab-BB', 1, 100, 1, now(), NULL, NULL, false, 'character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
(layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'route_id', 'varchar', 'Route_Id', false, NULL, 'Route Id', '122', 1, 100, 1, now(), NULL, NULL, false, 'character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'section_name', 'varchar', 'Section_Name', false, NULL, 'Section Name', 'Pan Punjab-BB', 1, 100, 1, now(), NULL, NULL, false, 'character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'section_id', 'varchar', 'Section_Id', false, NULL, 'Section Id', '123', 5, 100, 1,now(), NULL, NULL, false, 'character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'route_category', 'varchar', 'Route_Category', false, NULL, 'Route Category', 'IRU', 6, 100, 1, now(), NULL, NULL, false, 'character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'distance', 'float8', 'Distance', false, NULL, 'Distance', '1.236', 7, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'fiber_pairs_laid', 'int4', 'Fiber_Pairs_Laid', false, NULL, 'Fiber Pairs Laid', '1', 8, 10, 1, now(), NULL, NULL, false, 'integer', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'total_used_pair', 'int4', 'Total_Used_Pair', false, NULL, 'Total Used Pair', '1', 9, 10, 1,now(), NULL, NULL, false, 'integer', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'fiber_pairs_used_by_vil', 'int4', 'Fiber_Pairs_Used_by_VIL', false, NULL, 'Fiber Pairs Used by VIL', '1', 10, 10, 1, now(), NULL, NULL, false, 'integer', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'fiber_pairs_given_to_airtel', 'int4', 'Fiber_Pairs_Given_To_Airtel', false, NULL, 'Fiber Pairs Given To Airtel', '1', 10, 10, 1, now(), NULL, NULL, false, 'integer', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'fiber_pairs_given_to_others', 'int4', 'Fiber_Pairs_Given_To_Others', false, NULL, 'Fiber Pairs Given To Others', '1', 12, 10, 1, now(), NULL, NULL, false, 'integer', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'fiber_pairs_free', 'int4', 'Fiber_Pairs_Free', false, NULL, 'Fiber Pairs Free', '1', 13, 10, 1, now(), NULL, NULL, false, 'integer', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'faulty_fiber_pairs', 'int4', 'Faulty_Fiber_Pairs', false, NULL, 'Faulty Fiber Pairs', '1', 14, 10, 1, now(), NULL, NULL, false, 'integer', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'start_latitude', 'float8', 'Start_Latitude', false, NULL, 'Start Latitude', '28.2221', 15, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'start_longitude', 'float8', 'Start_Longitude', false, NULL, 'Start Longitude', '0', 16, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'end_latitude', 'float8', 'End Latitude', false, NULL, 'End Latitude', '0', 17, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'end_longitude', 'float8', 'End_Longitude', false, NULL, 'End Longitude', '0', 18, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'count_non_vil_tenancies_on_route', 'varchar', 'Count_Non_VIL_Tenancies_on_Route', false, NULL, 'Count Non VIL Tenancies on Route', '1', 19, 10, 1,now(), NULL, NULL, false, 'character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'route_lit_up_date', 'timestamp', 'Route_lit_up_Date', false, NULL, 'Route lit up Date', '', 20, 10, 1, now(), NULL, NULL, false, 'DateTime', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'aerial_km', 'float8', 'Aerial_Km', false, NULL, 'Aerial kms', '1', 21, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'avg_loss_per_km', 'float8', 'Avg_Loss_per_km', false, NULL, 'Avg Loss Per km', '1', 22, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'avg_last_six_months_fiber_cut', 'float8', 'Avg_Last_Six_Months_Fiber_Cut', false, NULL, 'Avg Last Six Months Fiber Cut', '1', 23, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'row', 'float8', 'Row', false, NULL, 'Row', '1', 24, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'material', 'float8', 'Material', false, NULL, 'Material', '1', 24, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'execution', 'float8', 'Execution', false, NULL, 'Execution', '1', 26, 10, 1,now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'row_availablity', 'float8', 'Row_Availablity', false, NULL, 'Row Availablity', '1', 27, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'iru_given_airtel', 'float8', 'IRU_Given_Airtel', false, NULL, 'IRU Given Airtel', '1', 28, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'iru_given_jio', 'float8', 'IRU_Given_JIO', false, NULL, 'IRU Given JIO', '1', 29, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'iru_given_ttsl_or_ttml', 'float8', 'IRU_Given_TTSL_or_TTML', false, NULL, 'IRU Given TTSL or TTML', '1', 30, 10, 1, now(), NULL, NULL, false, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'network_category', 'varchar', 'Network_Category', false, NULL, 'Network Category', '1', 31, 10, 1, now(), NULL, NULL, false, 'character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'row_valid_or_exp', 'timestamp', 'Row_Valid_or_Exp', false, NULL, 'ROW Valid/Exp By', '', 33, 10, 1, now(), NULL, NULL, false, 'DateTime', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'remarks', 'varchar', 'Remarks', false, NULL, 'Remarks', '', 34, 10, 1, now(), NULL, NULL, false, 'Character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'cable_owner', 'varchar', 'Cable_Owner', false, NULL, 'Cable Owner', '', 35, 10, 1, now(), NULL, NULL, false, 'Character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'route_type', 'varchar', 'Route_Type', false, NULL, 'Route Type', '', 36, 10, 1, now(), NULL, NULL, true, 'Character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'operator_type', 'varchar', 'Operator_Type', false, NULL, 'Operator Type', '', 37, 10, 1,now(), NULL, NULL, true, 'Character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'fiber_type', 'varchar', 'Fiber_Type', false, NULL, 'Fiber Type', '', 38, 10, 1, now(), NULL, NULL, true, 'Character varying', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'iru_given_tcl', 'float8', 'IRU_Given_TCL', false, NULL, 'IRU Given TCL', '', 40, 10, 1, now(), NULL, NULL, true, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'iru_given_others', 'float8', 'IRU_Given_Others', false, NULL, 'IRU Given Others', '', 41, 10, 1, now(), NULL, NULL, true, 'double', true, false, true, NULL, false, true, NULL, NULL, NULL, true, false, true);

INSERT INTO public.data_uploader_template
( layer_id, db_column_name, db_column_data_type, template_column_name, is_mandatory, udtname, description, example_value, column_sequence, max_length, created_by, created_on, modified_by, modified_on, is_dropdown, display_column_data_type, is_kml_attribute, is_nullable, is_excel_attribute, boundary_type, is_template_column_required, is_enable_for_template_column, default_value, min_value, max_value, is_allowed_for_update, is_allowed_for_update_admin, is_cdb_attributes)
VALUES( 19, 'cable_id', 'varchar', 'Cable_Id', true, NULL, 'cable Id', '', 39, 10, 1, now(), NULL, NULL, true, 'Character varying', true, false, true, NULL, true, true, NULL, NULL, NULL, true, false, true);

----------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_uploader_get_entity_template(p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$ 
BEGIN

RETURN QUERY
select row_to_json(row) from (select dutemp.id, ld.layer_id,ld.layer_name,db_column_name,db_column_data_type,column_sequence,template_column_name,is_mandatory,max_length,is_dropdown,is_nullable,is_template_column_required,default_value,min_value,max_value from data_uploader_template dutemp 
left join layer_details ld on dutemp.layer_id=ld.layer_id where upper(ld.layer_name)=upper(p_entity_type) 
and  dutemp.is_enable_for_template_column=true and is_cdb_attributes = false
--and dutemp.is_kml_attribute=false 
order by column_sequence
) row;

END ;
$function$
;

-------------------------------------------------------------------------------------------------------------------------------------

alter table data_uploader_template add column is_allowed_for_update bool;

alter table data_uploader_template add column is_allowed_for_update_admin bool;

alter table data_uploader_template add column is_cdb_attributes bool NOT NULL DEFAULT false;

----------------------------------------------------------------------------------------------------------------------------------------


CREATE OR REPLACE FUNCTION public.fn_uploader_get_template_cdbattributes_guideline(p_entity_type character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$
declare v_columns character varying;
v_values character varying;
BEGIN

RETURN QUERY
select row_to_json(row) from (
select template_column_name as "Field Name",description as "Description",display_column_data_type as "Data Type", case when is_mandatory then 'YES' else 'NO' end as "Is Mandatory",example_value as "Example value",default_value as "Default value",max_length as "Max length",min_value as "Min value",max_value as "Max value" from data_uploader_template dutemp
left join layer_details ld on dutemp.layer_id=ld.layer_id where upper(ld.layer_name)=upper(p_entity_type) and dutemp.db_column_name not in ('client_id', 'parent_client_id') and dutemp.is_cdb_attributes =true
--and is_kml_attribute=false
order by column_sequence
) row;


END ;
$function$
;
