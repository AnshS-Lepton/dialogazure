INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('en', 'SI_OSP_GBL_JQ_FRM_217', 'Chamber', true, true, 'English', 'Smart Inventory_Osp_Global_J Query_', 1, now(), now(), 1, true, false);
INSERT INTO public.res_resources
(culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key)
VALUES('hi', 'SI_OSP_GBL_JQ_FRM_217', 'Chamber', false, true, 'Hindi', 'Smart Inventory_Osp_Global_J Query_', 1, now(), now(), 1, true, false);


CREATE OR REPLACE FUNCTION public.fn_trg_update_core_port_status()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$



declare v_is_source_cpe_entity bool;
declare v_is_destination_cpe_entity bool;
declare v_display_name character varying;
declare v_layer_table character varying;
declare v_display_value character varying;
declare v_line_source_geometry geometry;
declare v_line_destination_geometry geometry;
v_source_point_geom geometry;
v_point_destination_geom geometry;

BEGIN
v_is_source_cpe_entity:=false;
v_is_destination_cpe_entity:=false;

IF (TG_OP = 'INSERT' and coalesce(new.fat_process_id,0)=0) 
THEN
select clable.display_column_name,ld.layer_table into v_display_name,v_layer_table from display_name_settings clable
inner join layer_details ld on clable.layer_id=ld.layer_id where upper(ld.layer_name)=upper(new.source_entity_type);
if(coalesce(v_display_name,'')!='')
then
execute 'select '||v_display_name||' from '||v_layer_table||' where system_id='||new.source_system_id||'' into v_display_value; 
update connection_info set source_display_name=v_display_value where connection_id=new.connection_id;
end if; 
select clable.display_column_name,ld.layer_table into v_display_name,v_layer_table from display_name_settings clable
inner join layer_details ld on clable.layer_id=ld.layer_id where upper(ld.layer_name)=upper(new.destination_entity_type);
if(coalesce(v_display_name,'')!='')
then
execute 'select '||v_display_name||' from '||v_layer_table||' where system_id='||new.destination_system_id||'' into v_display_value;
update connection_info set destination_display_name=v_display_value where connection_id=new.connection_id; 
end if; 

select clable.display_column_name,ld.layer_table into v_display_name,v_layer_table from display_name_settings clable
inner join layer_details ld on clable.layer_id=ld.layer_id where upper(ld.layer_name)=upper('Splicetray');
if(coalesce(v_display_name,'')!='' and new.source_tray_system_id>0)
then
execute 'select '||v_display_name||' from '||v_layer_table||' where system_id='||new.source_tray_system_id||'' into v_display_value; 
update connection_info set source_tray_display_name=v_display_value where connection_id=new.connection_id;
end if;
select clable.display_column_name,ld.layer_table into v_display_name,v_layer_table from display_name_settings clable
inner join layer_details ld on clable.layer_id=ld.layer_id where upper(ld.layer_name)=upper('Splicetray');
if(coalesce(v_display_name,'')!='' and new.destination_tray_system_id>0)
then
execute 'select '||v_display_name||' from '||v_layer_table||' where system_id='||new.destination_tray_system_id||'' into v_display_value; 
update connection_info set destination_tray_display_name=v_display_value where connection_id=new.connection_id;
end if;

---- REGION WHEN CABLE IS NOT TERMINATED & SPLICED

IF((UPPER(new.destination_entity_type)='CABLE')
and  exists(select 1 from att_details_cable where system_id=new.destination_system_id 
and ((a_system_id=0 and coalesce(a_entity_type,'')='') or(b_system_id=0 and coalesce(b_entity_type,'')=''))))
THEN

EXECUTE 'SELECT sp_geometry from point_master where system_id='||new.source_system_id||' and upper(entity_type)=upper('''||new.source_entity_type||''')' 
into v_source_point_geom;

EXECUTE 'SELECT sp_geometry from line_master where system_id='||new.destination_system_id||' and upper(entity_type)=upper('''||new.destination_entity_type||''')' 
into v_line_source_geometry;

IF(new.is_cable_a_end) THEN
     
UPDATE att_details_cable set a_system_id= new.source_system_id ,a_entity_type=new.source_entity_type,a_network_id= new.source_network_id 
,a_location=new.source_network_id where system_id=new.destination_system_id;
UPDATE LINE_MASTER  set SP_GEOMETRY= ST_SetPoint(v_line_source_geometry,0,v_source_point_geom) 
WHERE system_id=new.destination_system_id and upper(entity_type)=upper(new.destination_entity_type);
ELSE
UPDATE att_details_cable set b_system_id= new.source_system_id ,b_entity_type=new.source_entity_type,b_network_id= new.source_network_id 
,b_location=new.source_network_id where system_id=new.destination_system_id;
UPDATE LINE_MASTER  set SP_GEOMETRY= ST_SetPoint(v_line_source_geometry,-1,v_source_point_geom) 
WHERE system_id=new.destination_system_id and upper(entity_type)=upper(new.destination_entity_type);

END IF;

insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,
associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,
is_termination_point,entity_display_name,associated_display_name)
values(new.destination_system_id,new.destination_network_id,new.destination_entity_type,
new.source_system_id,new.source_network_id,new.source_entity_type,now(),
new.created_by,true,new.destination_display_name,new.source_display_name);

END IF;


IF((UPPER(new.source_entity_type)='CABLE')
and exists(select 1 from att_details_cable where system_id=new.source_system_id 
and ((a_system_id=0 and coalesce(a_entity_type,'')='') or(b_system_id=0 and coalesce(b_entity_type,'')=''))))
THEN

EXECUTE 'SELECT sp_geometry from point_master where system_id='||new.destination_system_id||' and upper(entity_type)=upper('''||new.destination_entity_type||''')' 
into v_point_destination_geom;

EXECUTE 'SELECT sp_geometry from line_master where system_id='||new.source_system_id||' and upper(entity_type)=upper('''||new.source_entity_type||''')' 
into v_line_destination_geometry;


   IF(new.is_cable_a_end) 
   THEN
   
UPDATE att_details_cable set a_system_id= new.destination_system_id ,a_entity_type=new.destination_entity_type,
a_network_id= new.destination_network_id
,a_location=new.destination_network_id  
where system_id=new.source_system_id;
UPDATE LINE_MASTER  set SP_GEOMETRY= ST_SetPoint(v_line_destination_geometry,0,v_point_destination_geom)
WHERE system_id=new.source_system_id and upper(entity_type)=upper(new.source_entity_type);

   ELSE
   
UPDATE att_details_cable set b_system_id= new.destination_system_id ,b_entity_type=new.destination_entity_type,b_network_id= new.destination_network_id
,b_location=new.destination_network_id where system_id=new.source_system_id;
UPDATE LINE_MASTER  set SP_GEOMETRY= ST_SetPoint(v_line_destination_geometry,-1,v_point_destination_geom)  
WHERE system_id=new.source_system_id and upper(entity_type)=upper(new.source_entity_type);


   END IF;

insert into associate_entity_info(entity_system_id,entity_network_id,entity_type,
associated_system_id,associated_network_id,associated_entity_type,created_on,created_by,
is_termination_point,entity_display_name,associated_display_name)
values(new.source_system_id,new.source_network_id,new.source_entity_type,
new.destination_system_id,new.destination_network_id,new.destination_entity_type,now(),
new.created_by,true,new.source_display_name,new.destination_display_name);

END IF;

IF ((concat(new.source_system_id::text,upper(new.source_entity_type::text))!=concat(new.destination_system_id::text,upper(new.destination_entity_type::text)))) 
THEN
select is_cpe_entity into v_is_source_cpe_entity from layer_details where upper(layer_name)=upper(new.source_entity_type);
select is_cpe_entity into v_is_destination_cpe_entity from layer_details where upper(layer_name)=upper(new.destination_entity_type);
------------UPDATE THE CABLE CORE STATUS---------
  if(upper(new.source_entity_type)='CABLE' and new.is_cable_a_end=true)
  then
update att_details_cable_info  set a_end_status_id=2   where cable_id=new.source_system_id and fiber_number=new.source_port_no;
update isp_port_info  set port_status_id=2   where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) 
and port_number=case when new.destination_port_no<0 then (-1*new.destination_port_no) 
else new.destination_port_no end  and input_output=case when new.destination_port_no<0 then 'I' else 'O' end;   
  elsif(upper(new.source_entity_type)='CABLE' and new.is_cable_a_end=false)
  then
update att_details_cable_info set b_end_status_id=2 where cable_id=new.source_system_id and fiber_number=new.source_port_no; 
update isp_port_info  set port_status_id=2   where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) and port_number=case when new.destination_port_no<0 then (-1*new.destination_port_no) else new.destination_port_no end and input_output=case when new.destination_port_no<0 then 'I' else 'O' end;   
  elsif(upper(new.destination_entity_type)='CABLE' and new.is_cable_a_end=true)
  then
update att_details_cable_info  set a_end_status_id=2   where cable_id=new.destination_system_id and fiber_number=new.destination_port_no;
update isp_port_info  set port_status_id=2   where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end 
and input_output=case when new.source_port_no<0 then 'I' else 'O' end;   
  elsif(upper(new.destination_entity_type)='CABLE' and new.is_cable_a_end=false)
  then
update att_details_cable_info set b_end_status_id=2 where cable_id=new.destination_system_id and fiber_number=new.destination_port_no;
update isp_port_info  set port_status_id=2   where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end and input_output=case when new.source_port_no<0 then 'I' else 'O' end;   
  end if;     

 
------------UPDATE THE SPLITTER PORT STATUS---------
if(upper(new.source_entity_type)='SPLITTER' or upper(new.source_entity_type)='FMS' or v_is_source_cpe_entity)
  then
  
update isp_port_info  set port_status_id=2   where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) 
and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end and input_output=case when new.source_port_no<0 then 'I' else 'O' end; 
 
elsif(upper(new.destination_entity_type)='SPLITTER' or upper(new.destination_entity_type)='FMS' or v_is_destination_cpe_entity)
then
update isp_port_info  set port_status_id=2   where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) 
and port_number=case when new.destination_port_no<0 then (-1*new.destination_port_no) else new.destination_port_no end and input_output=case when new.destination_port_no<0 then 'I' else 'O' end;   
end if;

------------UPDATE THE CUSTOMER STATUS---------
if(upper(new.source_entity_type)='CUSTOMER')
  then
update att_details_customer  set customer_status_id=2   where system_id=new.source_system_id;   
elsif(upper(new.destination_entity_type)='CUSTOMER')
then
update att_details_customer  set customer_status_id=2   where system_id=new.destination_system_id;   
end if;

END IF;
-- ----------------------------UPDATE THE EQUIPMENT PORT STATUS---------------------------------
-- 


IF( upper(new.source_entity_type)=upper('Equipment') 
and EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(NEW.SOURCE_NETWORK_ID)) and upper(new.splicing_source)='EQUIPMENT_SPLICING')
THEN
update isp_port_info set port_status_id=2 
where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) 
and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end 
and upper(input_output) in ('I','O');
END IF;

IF ( upper(new.source_entity_type)=upper('Equipment') 
and ((new.source_port_no<0 and new.destination_port_no<0) or (new.source_port_no>0 and new.destination_port_no>0)))
then
update isp_port_info set port_status_id=2 
where parent_system_id=new.source_system_id and upper(parent_entity_type)=upper(new.source_entity_type) 
and port_number=case when new.source_port_no<0 then (-1*new.source_port_no) else new.source_port_no end 
and upper(input_output)=case when new.source_port_no<0 then 'I' else 'O' end; 
end if;

IF (upper(new.destination_entity_type)=upper('Equipment')) and ((new.source_port_no<0 and new.destination_port_no<0) or (new.source_port_no>0 and new.destination_port_no>0))
then 
update isp_port_info set port_status_id=2 
where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) 
and port_number= case when new.destination_port_no<0 then (-1*new.destination_port_no) else new.destination_port_no end 
and upper(input_output)=case when new.destination_port_no<0 then 'I' else 'O' end; 
end if;

IF ( upper(new.destination_entity_type)=upper('Equipment') 
and EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(NEW.DESTINATION_NETWORK_ID)) and upper(new.splicing_source)='EQUIPMENT_SPLICING')
then 
update isp_port_info set port_status_id=2 
where parent_system_id=new.destination_system_id and upper(parent_entity_type)=upper(new.destination_entity_type) 
and port_number= case when new.destination_port_no<0 then (-1*new.destination_port_no) else new.destination_port_no end 
and upper(input_output)in ('I','O');
end if;
end if;


IF (TG_OP = 'DELETE' and coalesce(old.fat_process_id,0)=0) 
THEN  
select is_cpe_entity into v_is_source_cpe_entity from layer_details where upper(layer_name)=upper(old.source_entity_type);
select is_cpe_entity into v_is_destination_cpe_entity from layer_details where upper(layer_name)=upper(old.destination_entity_type);
--DELETE THE PATCH CORD IF SOURCE ENTITY TYPE IS PATCH CORD
IF(upper(old.source_entity_type)=UPPER('PATCHCORD'))
THEN
DELETE FROM ATT_DETAILS_PATCHCORD WHERE SYSTEM_ID=old.source_system_id;
ELSIF(upper(old.destination_entity_type)=UPPER('PATCHCORD'))
THEN
DELETE FROM ATT_DETAILS_PATCHCORD WHERE SYSTEM_ID=old.destination_system_id;
END IF;

--------UPDATE THE CABLE CORE STATUS---------
if(upper(old.source_entity_type)='CABLE' and old.is_cable_a_end=true)
   then
update att_details_cable_info  set a_end_status_id=1   where cable_id=old.source_system_id and fiber_number=old.source_port_no;
update isp_port_info  set port_status_id=1   where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) and port_number=case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end  and input_output=case when old.destination_port_no<0 then 'I' else 'O' end;   
   elsif(upper(old.source_entity_type)='CABLE' and old.is_cable_a_end=false)
   then
update att_details_cable_info set b_end_status_id=1 where cable_id=old.source_system_id and fiber_number=old.source_port_no;
update isp_port_info  set port_status_id=1   where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) and port_number=case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end  and input_output=case when old.destination_port_no<0 then 'I' else 'O' end;   
   elsif(upper(old.destination_entity_type)='CABLE' and old.is_cable_a_end=true)
   then
update att_details_cable_info  set a_end_status_id=1   where cable_id=old.destination_system_id and fiber_number=old.destination_port_no;
update isp_port_info  set port_status_id=1   where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) and port_number=case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end and input_output=case when old.source_port_no<0 then 'I' else 'O' end;   
   elsif(upper(old.destination_entity_type)='CABLE' and old.is_cable_a_end=false)
   then
update att_details_cable_info set b_end_status_id=1 where cable_id=old.destination_system_id and fiber_number=old.destination_port_no;
update isp_port_info  set port_status_id=1   where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) and port_number=case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end and input_output=case when old.source_port_no<0 then 'I' else 'O' end;   
   end if;

------------UPDATE THE SPLITTER PORT STATUS---------
if(upper(old.source_entity_type)='SPLITTER' or upper(old.source_entity_type)='FMS' or v_is_source_cpe_entity)
   then
update isp_port_info  set port_status_id=1   where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) and port_number=case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end and input_output=case when old.source_port_no<0 then 'I' else 'O' end;   
   elsif(upper(old.destination_entity_type)='SPLITTER' or upper(old.destination_entity_type)='FMS' or v_is_destination_cpe_entity)
   then
update isp_port_info  set port_status_id=1   where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) and port_number=case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end and input_output=case when old.destination_port_no<0 then 'I' else 'O' end;    
   end if;

   ------------UPDATE THE CUSTOMER STATUS---------
if(upper(old.source_entity_type)='CUSTOMER')
then
update att_details_customer  set customer_status_id=1   where system_id=old.source_system_id;   
elsif(upper(old.destination_entity_type)='CUSTOMER')
then
update att_details_customer  set customer_status_id=1   where system_id=old.destination_system_id;   
end if;


   


IF ( upper(old.source_entity_type)=upper('Equipment') 
and EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(old.SOURCE_NETWORK_ID)) and upper(old.splicing_source)='EQUIPMENT_SPLICING')
then
update isp_port_info set port_status_id=1 
where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) 
and port_number=(case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end) 
and upper(input_output)in ('I','O'); 
end if; 

IF ( upper(old.destination_entity_type)=upper('Equipment')
and EXISTS(SELECT 1 FROM ATT_DETAILS_FMS WHERE UPPER(NETWORK_ID)=UPPER(old.DESTINATION_NETWORK_ID)) and upper(old.splicing_source)='EQUIPMENT_SPLICING')
then 
update isp_port_info set port_status_id=1 
where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) 
and port_number=(case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end) 
and upper(input_output)in ('I','O');
end if; 

IF ( upper(old.source_entity_type)=upper('Equipment')) 
and ((old.source_port_no<0 and old.destination_port_no<0) or (old.source_port_no>0))
then
update isp_port_info set port_status_id=1   where parent_system_id=old.source_system_id and upper(parent_entity_type)=upper(old.source_entity_type) 
and port_number=(case when old.source_port_no<0 then (-1*old.source_port_no) else old.source_port_no end) and upper(input_output)=case when old.source_port_no<0 then 'I' else 'O' end; 
end if; 

IF ( upper(old.destination_entity_type)=upper('Equipment'))
and ((old.source_port_no<0 and old.destination_port_no<0) or (old.destination_port_no>0))
then 
update isp_port_info set port_status_id=1   where parent_system_id=old.destination_system_id and upper(parent_entity_type)=upper(old.destination_entity_type) 
and port_number=(case when old.destination_port_no<0 then (-1*old.destination_port_no) else old.destination_port_no end) and upper(input_output)=case when old.destination_port_no<0 then 'I' else 'O' end; 
end if; 
END IF; 

RETURN NEW;
END;
$BODY$;