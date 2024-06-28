CREATE OR REPLACE FUNCTION public.fn_save_group_library_entity(p_system_id integer, p_entity_type character varying, p_parent_id integer, p_is_accessories_required boolean, p_is_associated_entity boolean, p_is_child_entity boolean, p_name character varying, p_description character varying, p_created_by integer)

RETURNS SETOF integer

LANGUAGE plpgsql

AS $function$

DECLARE

v_entity_data character varying;

v_allcolumns  character varying;

v_layer_table character varying;

V_ALLACCESSCOLUMNS character varying;

V_ACCESSORIES_DATE character varying;

V_TRAY_INFO character varying;

V_TRAY_INFO_DATA character varying;

v_geom_type character varying;

BEGIN

select layer_table,geom_type into v_layer_table,v_geom_type  from layer_details where lower(layer_name)=lower(p_entity_type);

                if(upper(v_geom_type)=upper('Point'))

                THEN


                                EXECUTE 'SELECT string_agg( column_name, '','' ) 

                                FROM information_schema.columns WHERE table_name   = '''||v_layer_table||''' and lower(column_name) not in

                                (''system_id'',''latitude'',''longitude'',''modified_on'',''created_on'',''network_id'',''created_by'',''parent_system_id'',

                                ''parent_network_id'',''parent_entity_type'',''sequence_id'',lower('''||p_entity_type||'_name''),''region_id'',''province_id'',''csa_id'',''dsa_id'',

                                ''area_id'',''subarea_id'',''csa_system_id'',''dsa_system_id'',''area_system_id'',''subarea_system_id'',''gis_design_id'',''status_updated_by'',''status_updated_on'',''structure_id'',''shaft_id'',''floor_id'',''project_id'',''planning_id'',''purpose_id'',''workorder_id'',''st_x'',''st_y'')'

                                INTO v_allcolumns;

                                EXECUTE'select row_to_json(row) from (select '||v_allcolumns||' from '||v_layer_table||' where system_id='||p_system_id||') row' into v_entity_data;


                                EXECUTE 'SELECT STRING_AGG(COLUMN_NAME, '','' ) 

                                FROM INFORMATION_SCHEMA.COLUMNS WHERE UPPER(TABLE_NAME)=UPPER(''ATT_DETAILS_ACCESSORIES'') AND UPPER(COLUMN_NAME)

                                NOT IN (''SYSTEM_ID'',''MODIFIED_ON'',''CREATED_ON'',''CREATED_BY'',''PARENT_SYSTEM_ID'',''PARENT_NETWORK_ID'',''PARENT_ENTITY_TYPE'',''REMARKS'',''MODIFIED_BY'')'

                                INTO V_ALLACCESSCOLUMNS;


                                EXECUTE'select json_agg(row) from (select '||V_ALLACCESSCOLUMNS||',vm.name as vendor_name,ADA.REMARKS,ADA.MODIFIED_BY,am.name as accessories_name

                                from ATT_DETAILS_ACCESSORIES ADA join vendor_master vm on ada.vendor_id=vm.id join accessories_master am on am.id=ada.accessories_id 

                                where PARENT_SYSTEM_ID = '||p_system_id||' AND UPPER(PARENT_ENTITY_TYPE)=UPPER('''||p_entity_type||''')) row' into V_ACCESSORIES_DATE;

                                raise info '%','new_status'|| V_ACCESSORIES_DATE;


                                EXECUTE 'SELECT STRING_AGG(COLUMN_NAME, '','' ) 

                                FROM INFORMATION_SCHEMA.COLUMNS WHERE UPPER(TABLE_NAME)=UPPER(''ATT_DETAILS_SPLICE_TRAY'') AND UPPER(COLUMN_NAME)

                                NOT IN (''SYSTEM_ID'',''MODIFIED_ON'',''CREATED_ON'',''CREATED_BY'',''PARENT_SYSTEM_ID'',''PARENT_NETWORK_ID'',''PARENT_ENTITY_TYPE''

                                ,''REMARKS'',''MODIFIED_BY'',''NETWORK_ID'')'

                                INTO V_TRAY_INFO;


                                EXECUTE'select json_agg(row) from (select '||V_TRAY_INFO||' from ATT_DETAILS_SPLICE_TRAY ADA  

                                where PARENT_SYSTEM_ID = '||p_system_id||' AND UPPER(PARENT_ENTITY_TYPE)=UPPER('''||p_entity_type||''')) row'

                                into V_TRAY_INFO_DATA;          

                ELSIF(upper(v_geom_type)=upper('Line'))        

                THEN

                                EXECUTE 'SELECT string_agg( column_name, '','' ) 

                                FROM information_schema.columns WHERE table_name   = '''||v_layer_table||''' and lower(column_name) not in

                                (''system_id'',''modified_on'',''created_on'',''network_id'',''created_by'',''parent_system_id''

                                ,''parent_network_id'',''parent_entity_type'',''sequence_id'',lower('''||p_entity_type||'_name''),

                                ''region_id'',''province_id'',''a_location'',''a_system_id'',''a_network_id'',''a_entity_type'',''b_system_id'',''b_network_id'',''b_entity_type'',''b_location'',''status_updated_by'',''status_updated_on'',''structure_id'',''shaft_id'',''floor_id'',''project_id'',''planning_id'',''purpose_id'',''workorder_id'',''st_x'',''st_y'')'

                                INTO v_allcolumns;

                                EXECUTE'select row_to_json(row) from (select '||v_allcolumns||' from '||v_layer_table||' where system_id='||p_system_id||') row' into v_entity_data;


 

 

                                EXECUTE 'SELECT STRING_AGG(COLUMN_NAME, '','' ) 

                                FROM INFORMATION_SCHEMA.COLUMNS WHERE UPPER(TABLE_NAME)=UPPER(''ATT_DETAILS_ACCESSORIES'') AND UPPER(COLUMN_NAME)

                                NOT IN (''SYSTEM_ID'',''MODIFIED_ON'',''CREATED_ON'',''CREATED_BY'',''PARENT_SYSTEM_ID'',''PARENT_NETWORK_ID'',''PARENT_ENTITY_TYPE'',''REMARKS'',''MODIFIED_BY'')'

                                INTO V_ALLACCESSCOLUMNS;


                                EXECUTE'select json_agg(row) from (select '||V_ALLACCESSCOLUMNS||',vm.name as vendor_name,ADA.REMARKS,ADA.MODIFIED_BY,am.name as accessories_name

                                from ATT_DETAILS_ACCESSORIES            ADA join vendor_master vm on ada.vendor_id=vm.id join accessories_master am on am.id=ada.accessories_id 

                                where PARENT_SYSTEM_ID = '||p_system_id||' AND UPPER(PARENT_ENTITY_TYPE)=UPPER('''||p_entity_type||''')) row' into V_ACCESSORIES_DATE;

                

                END IF;

                                

 

insert into entity_group_library(system_id,entity_type,is_accessories_required,parent_id,name,description,is_child_entity,is_associated_entity,is_active,created_by,created_on,entity_data,accessories_data,tray_info_data)

values(p_system_id,p_entity_type,p_is_accessories_required,p_parent_id,p_name,p_description,p_is_child_entity,p_is_associated_entity,true,p_created_by,now(),v_entity_data:: json,v_ACCESSORIES_DATE::json,V_TRAY_INFO_DATA::json);

RETURN QUERY

select egl.id from entity_group_library egl where egl.system_id=p_system_id and egl.entity_type=p_entity_type;                      

END

$function$

;