-- Function: fn_convertplannedtoasbuild(character varying)

-- DROP FUNCTION fn_convertplannedtoasbuild(character varying);

CREATE OR REPLACE FUNCTION fn_convertplannedtoasbuild(asbuilt_site_systemid character varying)
  RETURNS void AS
$BODY$


DECLARE	
	REC RECORD;
        query text;
        counter integer;
        AsBuildStaticValue character varying;	
        plan_system_id character varying;
        plan_buildingid character varying;
        asbuild_buildingid character varying;
        plan_room_id character varying;
        asbuilt_room_id character varying;
        plan_rack_id character varying;
        asbuilt_rack_id character varying;
        plan_rack_element_id character varying;
        asbuilt_rack_element_id character varying;
        plan_equipment character varying;
        plan_slot character varying;
        asbuilt_slot character varying;
        plan_card character varying;
        asbuild_card character varying;
        plan_port character varying;
        asbuild_port character varying;
        curBuilding refcursor;
	curRoom refcursor;
	cur_rack refcursor;
        cur_rack_element refcursor;
        cur_equipment refcursor;
        cur_slot refcursor;
        cur_card refcursor;
        cur_port refcursor;

        --
         temp_rack_element_id character varying;
         plan_odfsystem_id character varying;
         asbuild_odfsystem_id character varying;
         plan_odfid character varying;
         asbuilt_odfid character varying;



BEGIN

AsBuildStaticValue:='As Built';
counter:=0;
	-- BEGIN TRANSACTION
		
	
	select planned_refrence into plan_system_id from att_details_site where system_id = asbuilt_site_systemid;
	raise info ' Planned Site Id: %', plan_system_id;
	select count(*) into counter from isp_building_info where site_id = asbuilt_site_systemid;
	raise info 'Counter: %', counter;	
	
		 if(counter=0) then	
		 raise info 'Transaction Started.';
			OPEN curBuilding FOR select system_id from isp_building_info where site_id = plan_system_id;

			LOOP
				fetch  curBuilding into plan_buildingid;
				if not found then
				raise info 'Building Info not found';	
					exit ;
				END IF;

				raise info 'Planned Building Info: %', plan_buildingid;		 

				-- Insert Bulding As build
				raise info 'Building Info insert';
				asbuild_buildingid:='';
				SELECT 'BUILD' || lpad(nextval ('common_seq')::text, 11,'0') as nextval into asbuild_buildingid;
				raise info 'As Build Building ID: %', asbuild_buildingid;		
				insert into isp_building_info (
					system_id,site_id,site_name,floors,building_id,building_name,
					region_name,province_name,created_by,created_on,basements,roof,parent_pop,
					asset_id,network_id,network_stage,serial_no,customer_count,utilization
				) 
				select asbuild_buildingid,asbuilt_site_systemid,site_name,floors,replace(building_id, '(P)', ''),building_name,
					region_name,province_name,created_by,now(),basements,roof,parent_pop,
					replace(asset_id, '(P)', ''),replace(network_id, '(P)', ''),AsBuildStaticValue,
					serial_no,customer_count,utilization 
				from 	isp_building_info 
				where 	system_id=plan_buildingid;	
				raise info 'Building Info: inserted';		 
				
				-- Looping Rooms in Building
				OPEN curRoom FOR select system_id from isp_room_info where site_id = plan_buildingid;
				LOOP
					fetch  curRoom into plan_room_id;
					if not found then
						exit ;
					END IF;

					raise info 'Planned Room Id: %', plan_room_id;
					---- insert as built room from planned room with new building id
					----select * from isp_room_info where room_id = plan_room_id

					-- Insert Rooms As build 
					asbuilt_room_id:='';
					SELECT 'ROOM' || lpad(nextval ('common_seq')::text, 15,'0') as nextval into asbuilt_room_id;
					raise info 'As built Room Id %:', asbuilt_room_id;	
					insert into isp_room_info(
						system_id,room_uid,site_id,floor_id, room_name,
						room_type,room_rowcount,room_width,room_length,template_name, is_template,
						design,approval_status,region_name,province_name,created_by,
						created_on,room_location_type,dismantle, total_load, phase,current_load,engineer_name,date_of_dismantle_in_field,
						pop_customer_type,pop_customer_id,warehouse_name, hierarchy_type,parent_pop,
						asset_id,network_id,network_stage,serial_no,customer_count,utilization
					) 
					select asbuilt_room_id,replace(room_uid,'(P)',''),asbuild_buildingid,floor_id, room_name,
						room_type,room_rowcount,room_width,room_length,template_name, is_template,
						replace(design,plan_room_id,asbuilt_room_id),approval_status,region_name,province_name,created_by,
						now(),room_location_type,dismantle,total_load, phase,current_load, engineer_name,date_of_dismantle_in_field,
						pop_customer_type,pop_customer_id,warehouse_name,hierarchy_type,parent_pop,
						replace(asset_id, '(P)', ''),replace(network_id, '(P)', ''),AsBuildStaticValue,
						serial_no,customer_count,utilization 
					from 	isp_room_info 
					where 	system_id=plan_room_id; 	

							
					--   room genaral data   -------------------------------
					raise info 'Room General data insert';
					insert into isp_attr_general_data (construction_stage, activation_stage, accesibility, systemid) 
					select construction_stage, activation_stage, accesibility,asbuilt_room_id from isp_attr_general_data where systemid =plan_room_id;
					raise info 'Room General data insert END';
					--   room install data   -------------------------------
					raise info 'Room Install data insert';
					insert into isp_attr_install_data (
						installation_no, installation_year, production_year, installation_company, installation_technician,systemid )
					select installation_no, installation_year, production_year, installation_company, installation_technician,asbuilt_room_id
					  from isp_attr_install_data
					where systemid=plan_room_id;
					raise info 'Room Install data insert END';
					--   room Remark data   -------------------------------
					raise info 'Room Remark data insert';
					insert into isp_attr_remark_data (
						remark_1, remark_2, remark_3, remark_4, remark_5, remark_6, remark_7, remark_8, remark_9, remark_10, remark_11,
						remark_12, remark_13, remark_14, remark_15,system_id ) 
					select remark_1, remark_2, remark_3, remark_4,
						 remark_5, remark_6, remark_7, remark_8,
						remark_9, remark_10, remark_11,remark_12, remark_13, remark_14, remark_15,asbuilt_room_id  
					from isp_attr_remark_data 
					where system_id=plan_room_id;
					raise info 'Room Remark data insert END';
					-- Looping Racks in Room
					OPEN cur_rack FOR select system_id from isp_rack_info where parent_id = plan_room_id;
					LOOP
						fetch  cur_rack into plan_rack_id;
						if not found then
							exit ;
						END IF;			
						raise info 'Planned Rack Id: %', plan_rack_id;											 

						-- Insert Rack As build 	
						asbuilt_rack_id:=''; 
						SELECT 'RACK' || lpad(nextval ('common_seq')::text, 15,'0') as nextval into asbuilt_rack_id;
						raise info 'As built Rack Id %:', asbuilt_rack_id;
						insert into isp_rack_info( 
							system_id,rack_uid,parent_id,rack_unit,rack_width,rack_height,rack_row,
							rack_pos,rack_name,rack_type,rack_brand,parent_type,template_name,is_template,
							approval_status,province_name,created_by,created_on,rack_ref,dismantle,
							engineer_name,date_of_dismantle_in_field,pop_customer_type,pop_customer_id,
							pop_customer_name,warehouse_name,hierarchy_type,parent_pop,asset_id,
							network_id,network_stage,serial_no,utilization
						)
						select asbuilt_rack_id,replace(rack_uid,'(P)',''),asbuilt_room_id,rack_unit,rack_width,rack_height,rack_row,
							rack_pos,rack_name,rack_type,rack_brand,parent_type,template_name,is_template,
							approval_status,province_name,created_by,now(),rack_ref,dismantle,
							engineer_name,date_of_dismantle_in_field,pop_customer_type,pop_customer_id,
							pop_customer_name,warehouse_name,hierarchy_type,parent_pop,replace(asset_id,'(P)',''),
							replace(network_id,'(P)',''),AsBuildStaticValue,serial_no,utilization
						from 	isp_rack_info
						where 	system_id=plan_rack_id;			
						------Update Room Design------- 
						update isp_room_info 
							set design = replace(design, plan_rack_id, asbuilt_rack_id) 
						where system_id = asbuilt_room_id;
						------End Room Design----------

							
						------Insert Isp Attribute general data-------
						raise info 'Rack Remark data insert';
						insert into isp_attr_general_data (
							construction_stage, activation_stage, accesibility,systemid )  
						select construction_stage,activation_stage, accesibility, asbuilt_rack_id  
						from isp_attr_general_data 
						where systemid=plan_rack_id;
						raise info 'Rack Remark data insert END';
						------End Insert------------------------------
						------Insert Attribute Install Date--------------
						raise info 'Rack Install data insert';
						insert into isp_attr_install_data (
							installation_no, installation_year, production_year, installation_company, installation_technician,systemid)
						select installation_no, installation_year, production_year, installation_company, installation_technician,asbuilt_rack_id 
						from isp_attr_install_data 
						where systemid=plan_rack_id;
						raise info 'Rack Remark data insert END';
						-------End Insert-----------
						--------Insert Attr Dimension Data-----------
						raise info 'Rack dimension data insert';
						insert into isp_attr_dimension_data (
							calc_length, mannual_length, height, width, x_position, y_position, inner_diameter, outer_diameter,systemid)
						select calc_length, mannual_length, height, width, x_position, y_position, inner_diameter, outer_diameter,asbuilt_rack_id  
						from isp_attr_dimension_data 
						where systemid=plan_rack_id;
						raise info 'Rack dimension data insert END';
						--------End Attr Dimension Data----------------------------
						--------Insert ISP IP address------------------------------
						raise info 'Rack ID address data insert';
						insert into isp_ip_address (
							ip_address, subnet_mask,system_id ) 
						select ip_address, subnet_mask,asbuilt_rack_id  
						from isp_ip_address 
						where system_id=plan_rack_id;
						raise info 'Rack Address data insert END';
						---------End ISP adddress
						----------Insert Isp Attribute Remarks Data---------------
						raise info 'Rack remarks data insert';
						insert into isp_attr_remark_data (
							remark_1, remark_2, remark_3, remark_4, remark_5, remark_6, remark_7, remark_8, remark_9, remark_10,remark_11, remark_12, 
							remark_13, remark_14, remark_15, system_id) 
						select remark_1, remark_2, remark_3, remark_4, remark_5, remark_6,remark_7, remark_8, remark_9, remark_10, remark_11, remark_12,
							remark_13, remark_14, remark_15,asbuilt_rack_id 
						from isp_attr_remark_data
						where system_id=plan_rack_id;
						raise info 'Rack remarks data insert END';
						-----------End Isp Attribute--------------
							
						
						-- Looping Rack Elements in Rack
						OPEN cur_rack_element FOR select element_id from isp_rack_element_info where rack_id = plan_rack_id;
						--OPEN cur_rack_element FOR select ne_id from isp_rack_element_info where rack_id = plan_rack_id;
						LOOP
							fetch  cur_rack_element into plan_rack_element_id;
							if not found then
								exit ;
							END IF;				
							raise info 'Planned Rack Element Id: %', plan_rack_element_id;
							SELECT 'EQUP' || lpad(nextval ('common_seq')::text, 15,'0') as nextval into temp_rack_element_id;
						        IF (SUBSTR(plan_rack_element_id,1,3) = 'FMS') THEN
						          raise info '=============================  FMS As Build Start ==================================== ';
							ELSE IF (SUBSTR(plan_rack_element_id,1,3) = 'ODF') THEN
								   raise info '------------------------------ODF As Build START -----------------------';
									--temp_rack_element_id
								-----Insert into ODF Table Start Below
								-- plan_odfsystem_id character varying;
								-- asbuild_odfsystem_id character varying;
								-- plan_odfid character varying;
								-- asbuilt_odfid character varying;
								--select * from att_details_odf limit 10;
								SELECT 'ODF' || lpad(nextval ('common_seq')::text, 11,'0') as nextval into asbuild_odfsystem_id;
							       INSERT INTO att_details_odf(
								    system_id, odf_id, odf_name, odf_type, parent_id, parent_type, 
								    no_of_ports, odf_lat, odf_long, construction_stage, activation_stage, 
								    accessibility, width, height, x_position, y_position, manual_length, 
								    calculated_length, inner_diameter, outer_diameter, user_remark, 
								    remark_1, remark_2, remark_3, remark_4, remark_5, remark_6, remark_7, 
								    remark_8, remark_9, remark_10, remark_11, remark_12, remark_13, 
								    remark_14, remark_15, created_date, status, user_id, msid, province_name, 
								    region_name, parent_isp_id, db_flag, odf_brand, odf_model, mounting_type, 
								    units, installation_date, installation_number, installation_year, 
								    production_year, installation_company, installation_technician, 
								    hierarchy_type, sap_materialdesc, sap_serialnumber, sap_materialnumber, 
								    parent_pop, asset_id, network_id, network_stage, serial_no, customer_count, 
								    trail_id, utilization, isbuildready, schema_project_name, planned_refrence, 
								    address, mobile_no, fax, email_id, pincode, city, sublocality, 
								    locality, supervisor_name, execution_supervisor, supervisor_mobileno, 
								    supervisor_email, execution_date, acceptance_date, execution_company, 
								    start_point, end_point, customer_name, depth, maintenance_partner_name)
							  select asbuild_odfsystem_id, replace(odf_id,'(P)',''), odf_name, odf_type, parent_id, parent_type, 
								    no_of_ports, odf_lat, odf_long, construction_stage, activation_stage, 
								    accessibility, width, height, x_position, y_position, manual_length, 
								    calculated_length, inner_diameter, outer_diameter, user_remark, 
								    remark_1, remark_2, remark_3, remark_4, remark_5, remark_6, remark_7, 
								    remark_8, remark_9, remark_10, remark_11, remark_12, remark_13, 
								    remark_14, remark_15, now(), status, user_id, msid, province_name, 
								    region_name, asbuilt_rack_id, db_flag, odf_brand, odf_model, mounting_type, 
								    units, installation_date, installation_number, installation_year, 
								    production_year, installation_company, installation_technician, 
								    hierarchy_type, sap_materialdesc, sap_serialnumber, sap_materialnumber, 
								    parent_pop, replace(asset_id,'(P)',''), replace(network_id,'(P)',''), AsBuildStaticValue, serial_no, customer_count, 
								    trail_id, utilization, 'yes', schema_project_name, planned_refrence, 
								    address, mobile_no, fax, email_id, pincode, city, sublocality, 
								    locality, supervisor_name, execution_supervisor, supervisor_mobileno, 
								    supervisor_email, execution_date, acceptance_date, execution_company, 
								    start_point, end_point, customer_name, depth, maintenance_partner_name from att_details_odf where system_id = plan_rack_element_id;
							        
							         insert into isp_rack_element_info(
									element_id,rack_id,ne_id,ne_pos,slots,placement
								)
								select 	temp_rack_element_id,asbuilt_rack_id,asbuilt_rack_element_id,ne_pos,slots,placement 
								from 	isp_rack_element_info 
								where 	element_id=plan_rack_element_id;											
							  raise info '---------------------------------ODF As Build END: -----------------------------------';
							ELSE IF (SUBSTR(plan_rack_element_id,1,4) = 'EQUP') THEN
							 ----asbuilt_rack_element_id = sequence
							 ----select asbuilt_rack_element_id, asbuilt_rack_id
                                                         -- Insert Rack Element As build 
						     raise info '---------------------------------OLT START: -----------------------------------';
								 
							asbuilt_rack_element_id:=''; 
							SELECT 'EQUP' || lpad(nextval ('common_seq')::text, 15,'0') as nextval into asbuilt_rack_element_id;
							raise info 'As built Rack Element Id :%', asbuilt_rack_element_id;
							insert into isp_equipment_info ( 
								system_id,device_id,device_unit,equipment_type,common_name,asset_tag,explicit_tag,
								hardware_type,software_type,software_version,no_of_slots,device_length,device_width,
								device_height,brand,model,parent_type,product_name,template_name,is_template,
								design,region_name,province_name,created_by,created_on,approval_status,no_of_ports,
								category,dismantle,no_of_rear_slots,no_of_rear_ports,total_load,power_unit,serial_id,engineer_name,
								date_of_dismantle_in_field
								,pop_customer_type,pop_customer_id,pop_customer_name,hierarchy_type,warehouse_name,parent_pop,asset_id
								,network_id,network_stage,customer_count,utilization,ems_managed_element_name,ems_product_name,host_name,
								service_id,serial_no
							)
							select 	asbuilt_rack_element_id,replace(device_id,'(P)',''),device_unit,equipment_type,common_name,asset_tag,explicit_tag,
								hardware_type,software_type,software_version,no_of_slots,device_length,device_width,
								device_height,brand,model,parent_type,product_name,template_name,is_template,
								design,region_name,province_name,created_by,now(),approval_status,no_of_ports,
								category,dismantle,no_of_rear_slots,no_of_rear_ports,total_load,power_unit,serial_id,engineer_name,
								date_of_dismantle_in_field
								,pop_customer_type,pop_customer_id,pop_customer_name,hierarchy_type,warehouse_name,parent_pop,replace(asset_id,'(P)','')
								,replace(network_id,'(P)',''),AsBuildStaticValue,customer_count,utilization,ems_managed_element_name,
								ems_product_name,host_name,service_id,serial_no
							from isp_equipment_info
							where system_id = plan_rack_element_id;						


							---------------------------OLT REFRENCE TABLE --------------------------------------------------------
							--   To Create Two OLT first system_id store in element_id and second Store in ne_id column
							insert into isp_rack_element_info(
								element_id,rack_id,ne_id,ne_pos,slots,placement
							)
							select 	temp_rack_element_id,asbuilt_rack_id,asbuilt_rack_element_id,ne_pos,slots,placement 
							from 	isp_rack_element_info
							where 	rack_id = plan_rack_id and ne_id = plan_rack_element_id;



								
							------------Isp attribute General data---------------------------------------------------------
							raise info 'Equipment general insert';
							insert into isp_attr_general_data (construction_stage, activation_stage, accesibility,systemid )
							select construction_stage,activation_stage, accesibility,asbuilt_rack_element_id   
							from isp_attr_general_data 
							where systemid=plan_rack_element_id;
							raise info 'Equipment general insert END';
							-------------End attribute General data-------------------------------------------------------
							
							------------- Insert Dimention data
							raise info 'Equipment Dimention insert';
							insert into isp_attr_dimension_data (
								calc_length, mannual_length, height, width, x_position, y_position, inner_diameter, outer_diameter,systemid)
							select calc_length, mannual_length, height, width, x_position, y_position, inner_diameter, outer_diameter,asbuilt_rack_element_id  
							from isp_attr_dimension_data 
							where systemid=plan_rack_element_id;
							raise info 'Equipment Dimention insert END';
							--------------End general data------------------------------
							
							------Insert Attribute Install Date--------------
							raise info 'Equipment Install insert';
							insert into isp_attr_install_data (
								installation_no, installation_year, production_year, installation_company, installation_technician,systemid )
							select installation_no, installation_year, production_year, installation_company, installation_technician,asbuilt_rack_element_id
							from isp_attr_install_data 
							where systemid=plan_rack_element_id;
							raise info 'Equipment Install insert END';
							--------End Insert-------------------------------
							
							--------Insert ISP IP address--------------------
							raise info 'Equipment Ip address insert';
							insert into isp_ip_address (
								ip_address, subnet_mask, system_id ) 
							select ip_address, subnet_mask,asbuilt_rack_element_id 
							from isp_ip_address 
							where system_id=plan_rack_element_id;
							raise info 'Equipment Ip address insert END';
							---------End ISP adddress---------------------------------

							---------Insert Isp Attr EMS NMS data-------------------------------
							raise info 'Equipment EMSNMS insert';
							insert into isp_attr_ems_nms_data (ems_type, ems_brand, ems_model, nms_type, nms_brand, nms_model,systemid )
							select ems_type, ems_brand, ems_model, nms_type, nms_brand, nms_model, asbuilt_rack_element_id  from isp_attr_ems_nms_data
							 where systemid=plan_rack_element_id;
							
							raise info 'Equipment EMSNMS insert END';
							---------End Insert

							----------Insert Isp Attribute Remarks Data------
							raise info 'Equipment remarks insert';
							insert into isp_attr_remark_data (
								remark_1, remark_2, remark_3, remark_4, remark_5, remark_6, remark_7, remark_8, remark_9, remark_10,remark_11, remark_12, 
								remark_13, remark_14, remark_15,system_id ) 
							select remark_1, remark_2, remark_3, remark_4, remark_5, remark_6,remark_7, remark_8, remark_9, remark_10, remark_11, remark_12,
								remark_13, remark_14, remark_15,asbuilt_rack_element_id  
							from isp_attr_remark_data
							where system_id=plan_rack_element_id;
							raise info 'Equipment remarks insert END';
							-----------End Isp Attribute----------------------

							
							raise info '---------------------------------OLT END Id: -----------------------------------';
							-- Looping Slots in Element
							
							OPEN cur_slot FOR select system_id from isp_slot_info where element_id  = plan_rack_element_id;
							LOOP
								fetch  cur_slot into plan_slot;
								if not found then
									exit ;
								END IF;
								 raise info '---------------------------------SLOT START: -----------------------------------';
								raise info 'Planned Slot Info: %', plan_slot;

								-- Inserting Slot as Build
								asbuilt_slot:=''; 
								SELECT 'SLOT' || lpad(nextval ('common_seq')::text, 15,'0') as nextval into asbuilt_slot;
								raise info 'ASBUILD Slot Info: %', asbuilt_slot;
								
								insert into isp_slot_info (
									system_id,slot_id,slot_name,element_id,slot_length,slot_width,no_of_ports,
									slot_type,parent_type,template_name,is_template,approval_status,created_by,
									created_on,slot_ref,region_name,province_name,dismantle,placement,pop_customer_name,
									pop_customer_id,pop_customer_type,hierarchy_type,parent_pop,asset_id,network_id,
									network_stage,serial_no,customer_count,utilization
								)
								select 	asbuilt_slot,replace(slot_id,'(P)',''),slot_name,asbuilt_rack_element_id,
									slot_length,slot_width,no_of_ports,slot_type,parent_type,
									template_name,is_template,approval_status,created_by,
									now(),slot_ref,region_name,province_name,dismantle,placement,pop_customer_name,
									pop_customer_id,pop_customer_type,hierarchy_type,parent_pop,replace(asset_id,'(P)',''),replace(network_id,'(P)',''),
									AsBuildStaticValue,serial_no,customer_count,utilization 
								from 	isp_slot_info
								--where 	element_id=plan_rack_element_id;--plan_slot
								where 	system_id=plan_slot;--plan_slot

								update isp_equipment_info set design = replace(design,plan_slot,asbuilt_slot) where system_id = asbuilt_rack_element_id;

								
								------------- Insert general data------
								raise info 'Slot isp_attr_general_data insert';
								insert into isp_attr_general_data (construction_stage, activation_stage, accesibility,systemid )
								select construction_stage,activation_stage, accesibility,asbuilt_slot   
								from isp_attr_general_data 
								where systemid=plan_slot;
								raise info 'Slot isp_attr_general_data insert END';
								--------------End general data-------------
								raise info 'Slot isp_attr_dimension_data insert';
								insert into isp_attr_dimension_data (
									calc_length, mannual_length, height, width, x_position, y_position, inner_diameter,
									 outer_diameter,systemid)
								select calc_length, mannual_length, height, width, x_position, y_position, inner_diameter, outer_diameter,asbuilt_slot  
								from isp_attr_dimension_data 
								where systemid=plan_slot;
								raise info 'Slot isp_attr_dimension_data insert END';
						
								--------End Attr Dimension Data----------------------------
								
								------Insert Attribute Install Date--------------
								raise info 'Slot isp_attr_install_data insert';
								insert into isp_attr_install_data (
									installation_no, installation_year, production_year, installation_company, 
									installation_technician,systemid)
								select installation_no, installation_year, production_year, installation_company, installation_technician,asbuilt_slot 
								from isp_attr_install_data 
								where systemid=plan_slot;
								raise info 'Slot isp_attr_install_data insert END';
								--------End Insert-------------------------------
								
								--------Insert Remark Data-----------------------
								raise info 'Slot isp_attr_remark_data insert';
								insert into isp_attr_remark_data (
									remark_1, remark_2, remark_3, remark_4, remark_5, remark_6, remark_7, remark_8, 
									remark_9, remark_10,remark_11, remark_12, remark_13, remark_14, remark_15,system_id ) 
								select remark_1, remark_2, remark_3, remark_4, remark_5, remark_6,remark_7, remark_8, 
									remark_9, remark_10, 
									remark_11, remark_12,remark_13, remark_14, remark_15,asbuilt_slot  
								from isp_attr_remark_data
								where system_id=plan_slot;
								--where system_id=plan_rack_element_id;
								raise info 'Slot isp_attr_remark_data insert END';
								--------End Remark Data-----------------------

								
							raise info '---------------------------------SLOT END: -----------------------------------';
								-- Looping Cards in Slot
								OPEN cur_card FOR select system_id from isp_card_info where slot_id = plan_slot;
								LOOP
									fetch  cur_card into plan_card;
									if not found then
										exit ;
									END IF;
									raise info 'Planned Card Info: %', plan_card;

									-- Inserting Card as Build
									asbuild_card:=''; 
									SELECT 'CARD' || lpad(nextval ('common_seq')::text, 15,'0') as nextval into asbuild_card;
									raise info 'As built Card Id %:', asbuild_card;
									insert into  isp_card_info (
										system_id,card_id,slot_id,card_name,no_of_ports,card_type,
										card_brand,card_model,parent_type,technology_type,template_name,
										is_template,design,approval_status,created_by,created_on,region_name,
										province_name,dismantle,engineer_name,date_of_dismantle_in_field,link_type,
										pop_customer_type,pop_customer_id,pop_customer_name,warehouse_name,hierarchy_type,
										parent_pop,asset_id,network_id,network_stage,serial_no,customer_count,utilization
									)
									select	asbuild_card,replace(card_id,'(P)',''),asbuilt_slot,card_name,no_of_ports,card_type,
										card_brand,card_model,parent_type,technology_type,template_name,
										is_template,replace(design,plan_card,asbuild_card),approval_status,created_by,now(),region_name,
										province_name,dismantle,engineer_name,date_of_dismantle_in_field,link_type,
										pop_customer_type,pop_customer_id,pop_customer_name,warehouse_name,hierarchy_type,
										parent_pop,replace(asset_id,'(P)',''),replace(network_id,'(P)',''),AsBuildStaticValue,serial_no,
										customer_count,utilization
									from 	isp_card_info
									where 	slot_id=plan_slot;

									update isp_card_info set design = replace(design,plan_card,asbuild_card) where system_id = asbuild_card;

									-- Looping Ports in Card
									OPEN cur_port FOR select system_id from isp_port_info where card_ne_id = plan_card;
									LOOP
										fetch  cur_port into plan_port;
										if not found then
											exit ;
										END IF;
										raise info 'Planned Port Info: %', plan_port;

										-- inserting port as build
										asbuild_port:=''; 
										SELECT 'PORT' || lpad(nextval ('common_seq')::text, 15,'0') as nextval into asbuild_port;
										raise info 'As built Port Id %:', asbuild_port;
										insert into  isp_port_info (
											system_id,port_id,port_name,card_ne_id,port_number,sfp_type,
											port_type,parent_type,template_name,is_template,design,approval_status,
											created_by,created_on,port_ref,region_name,province_name,dismantle,
											placement,pop_customer_type,pop_customer_id,pop_customer_name,
											technology_type,hierarchy_type,parent_pop,network_id,network_stage,
											serial_no,customer_count,utilization

										)
										select 	asbuild_port,replace(port_id,'(P)',''),port_name,asbuild_card,port_number,sfp_type,
											port_type,parent_type,template_name,is_template,design,approval_status,
											created_by,now(),port_ref,region_name,province_name,dismantle,
											placement,pop_customer_type,pop_customer_id,pop_customer_name,
											technology_type,hierarchy_type,parent_pop,replace(network_id,'(P)',''),AsBuildStaticValue,
											serial_no,customer_count,utilization
										from isp_port_info
										where system_id=plan_port;
																	
										update isp_card_info set design = replace(design,plan_port,asbuild_port) where system_id = asbuild_card;


										
										------------- Insert general data------
										raise info 'PORT  isp_attr_dimension_data insert';
										insert into isp_attr_dimension_data (
											calc_length, mannual_length, height, width, x_position, y_position, inner_diameter,
											outer_diameter,systemid)
										select calc_length, mannual_length, height, width, x_position, y_position, inner_diameter,
											outer_diameter,asbuild_port  
										from isp_attr_dimension_data 
										where systemid=plan_port;
										raise info 'PORT  isp_attr_dimension_data insert END';
										--------------End general data-------------
										--------Insert ISP IP address--------------------
										raise info 'PORT  isp_ip_address insert';
										insert into isp_ip_address (
											ip_address, subnet_mask,system_id ) 
										select ip_address, subnet_mask,asbuild_port  
										from isp_ip_address 
										where system_id=plan_port;
										raise info 'PORT  isp_ip_address insert END';
										---------End ISP adddress------------------
										--------Insert Remark Data-----------------------
										raise info 'PORT  isp_attr_remark_data insert';
										insert into isp_attr_remark_data (
											remark_1, remark_2, remark_3, remark_4, remark_5, remark_6, remark_7, remark_8, 
											remark_9, remark_10,remark_11, remark_12, remark_13, remark_14, remark_15,system_id ) 
										select remark_1, remark_2, remark_3, remark_4, remark_5, remark_6,remark_7, remark_8, 
											remark_9, remark_10, 
											remark_11, remark_12,remark_13, remark_14, remark_15,asbuild_port  
										from isp_attr_remark_data
										where system_id=plan_port;
										raise info 'PORT  isp_attr_remark_data insert END';
										--------End Remark Data-----------------------

										------Insert Attribute Install Date--------------
										raise info 'PORT  isp_attr_install_data insert';
										insert into isp_attr_install_data (
											installation_no, installation_year, production_year, installation_company, 
										installation_technician,systemid)
										select installation_no, installation_year, production_year, installation_company,
										 installation_technician,asbuild_port 
										from isp_attr_install_data 
										where systemid=plan_port;
										raise info 'PORT  isp_attr_install_data insert END';
										--------End Insert-------------------------------
										
												
									END LOOP;  
									close cur_port;

								END LOOP;        
								close cur_card;

							END LOOP;  
							close cur_slot;

						END IF;
						END IF;
						END IF;

					    END LOOP;  
					    close cur_rack_element;

					END LOOP;  
					close cur_rack;
				
				END LOOP;
				close curRoom;
				  
			END LOOP;
			close curBuilding;	
			 RAISE NOTICE 'Transaction End  sucessfully.';
		 ELSE		
		 RAISE NOTICE 'Already ISP Elements are As-Build.';
		 END IF;


	
END; 
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100;
ALTER FUNCTION fn_convertplannedtoasbuild(character varying)
  OWNER TO postgres;
