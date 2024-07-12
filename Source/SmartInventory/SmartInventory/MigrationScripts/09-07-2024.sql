insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'execution_method',dropdown_value,dropdown_status,created_by,created_on,dropdown_key,'execution_method',is_action_allowed,is_active,parent_id
from dropdown_master where layer_id=(select layer_id from layer_details where upper(layer_name)='TRENCH')
and dropdown_type='Trench_Type' and upper(dropdown_value) not in  (select upper(dropdown_value) from dropdown_master where layer_id=(select layer_id from layer_details where upper(layer_name)='TRENCH')
and dropdown_type='execution_method');


Delete from dropdown_master where dropdown_type='Strata_Type' and layer_id=(select layer_id from layer_details where upper(layer_name)='TRENCH');
insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'Strata_Type','Hard Soil',true,1,now(),'Hard Soil','Strata_Type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'Strata_Type','Soft Soil',true,1,now(),'Soft Soil','Strata_Type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'Strata_Type','Generic',true,1,now(),'Generic','Strata_Type',true,true,0 from layer_details where upper(layer_name)='TRENCH'; 


Delete from dropdown_master where dropdown_type='trench_serving_type' and layer_id=(select layer_id from layer_details where upper(layer_name)='TRENCH');
insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'trench_serving_type','Back Bom',true,1,now(),'Back Bom','trench_serving_type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'trench_serving_type','Access',true,1,now(),'Access','trench_serving_type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'trench_serving_type','Back Bom LM',true,1,now(),'Back Bom LM','trench_serving_type',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'trench_serving_type','Access LM',true,1,now(),'Access LM','trench_serving_type',true,true,0 from layer_details where upper(layer_name)='TRENCH'; 

insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'Surface_Type','Damar Road',true,1,now(),'Damar Road','Surface_Type',true,true,0 from layer_details where upper(layer_name)='TRENCH'; 



insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'Model','Desktop design',true,1,now(),'Desktop design','Model',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'Model','NFA Approval Done',true,1,now(),'NFA Approval Done','Model',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'Model','BOQ',true,1,now(),'BOQ','Model',true,true,0 from layer_details where upper(layer_name)='TRENCH'
union
Select layer_id,'Model','Completed ',true,1,now(),'Completed','Model ',true,true,0 from layer_details where upper(layer_name)='TRENCH'; 


insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'Duct_Type','Microduct',true,1,now(),'Microduct','Duct_Type',true,true,0 from layer_details where upper(layer_name)='DUCT';

insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'Cable_Subcategory','ADSS',true,1,now(),'ADSS','Cable_Subcategory',true,true,0 from layer_details where upper(layer_name)='CABLE';

insert into dropdown_master (layer_id,dropdown_type,dropdown_value,dropdown_status,created_by,created_on,dropdown_key,db_column_name,is_action_allowed,is_active,parent_id)
Select layer_id,'Route_Type','OWN',true,1,now(),'OWN','Route_Type',true,true,0 from layer_details where upper(layer_name)='CABLE'
union
Select layer_id,'Route_Type','IRU',true,1,now(),'IRU','Route_Type',true,true,0 from layer_details where upper(layer_name)='CABLE';

update dropdown_master set dropdown_value='Aerial (Structured)',dropdown_key='Aerial (Structured)' where layer_id=(select layer_id from layer_details where upper(layer_name)='CABLE')
and dropdown_type='Cable_Type' and dropdown_value='Overhead';

update dropdown_master set dropdown_value='Aerial (Unstructured)',dropdown_key='Aerial (Unstructured)' where layer_id=(select layer_id from layer_details where upper(layer_name)='CABLE')
and dropdown_type='Cable_Type' and dropdown_value='Wall Clamped';


Delete from dropdown_master where layer_id=(select layer_id from layer_details where upper(layer_name)='CABLE')
and dropdown_type='Cable_Category' and dropdown_value in('Distribution','Drop','Feeder','Test');


-- select * from res_resources where key='SI_OSP_GBL_GBL_GBL_025' and culture<>'hi';
update res_resources set value ='IRU' where key='SI_GBL_GBL_GBL_GBL_151' and culture<>'hi';
update res_resources set value ='Installation Year' where key='SI_OSP_GBL_NET_FRM_037' and culture<>'hi';
update res_resources set value ='Owner:' where key='SI_GBL_GBL_GBL_GBL_148' and culture<>'hi';
update res_resources set value ='Met Code' where key='SI_OSP_GBL_NET_FRM_014' and culture<>'hi';
update res_resources set value ='Planning' where key='SI_OSP_GBL_GBL_GBL_025' and culture<>'hi';

insert into form_input_settings(form_name,form_feature_name,form_feature_type,is_active,is_required,created_by,created_on,is_readonly)
Values('Trench','trench_type','field',false,false,1,now(),false);



insert into system_spec_master(prop_name,prop_value)
values ('Construction','Completed')


CREATE OR REPLACE FUNCTION public.fn_get_item_specification(
	entitytype character varying,
	typeid integer,
	brandid integer,
	specification character varying)
    RETURNS TABLE(key character varying, value character varying, ddtype character varying, type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

DECLARE
sql TEXT;
v_layer_id integer;
BEGIN
select layer_id into v_layer_id from layer_details where upper(layer_name)=upper(entitytype);

sql:= 'select specification as key,specification as value, ''Specification''::character varying as dd_type,specify_type as type
from item_template_master where layer_id='||v_layer_id||' and is_active=true
union
select prop_value as key,id::character varying as value, ''Accessibility''::character varying as dd_type,''''::character varying as type
from system_spec_master where lower(prop_name)=lower(''accessibility'')
union
select prop_value as key,id::character varying as value, ''Construction''::character varying as dd_type,''''::character varying as type
from system_spec_master where lower(prop_name)=lower(''construction'')
union
select prop_value as key,id::character varying as value, ''Activation''::character varying as dd_type,''''::character varying as type
from system_spec_master where lower(prop_name)=lower(''activation'')
union
select dropdown_key as key,dropdown_value::character varying as value, ''TypeMaster''::character varying as dd_type ,''''::character varying as type
from dropdown_master where dropdown_type=''Splitter_Type''
union
select dropdown_key as key,dropdown_value::character varying as value, ''type''::character varying as dd_type ,''''::character varying as type
from dropdown_master where layer_id='||v_layer_id||' and dropdown_type=''type''    
union
select dropdown_key as key,dropdown_value::character varying as value, ''Hierarchy_type''::character varying as dd_type ,''''::character varying as type
from dropdown_master where layer_id='||v_layer_id||' and dropdown_type=''Hierarchy_type''
union
select dropdown_key as key,dropdown_value::character varying as value, ''Model''::character varying as dd_type ,''''::character varying as type
from dropdown_master where layer_id='||v_layer_id||' and dropdown_type=''Model''';

raise info 'V_SQLQUERY: %', sql;

if typeid !=0 then
sql:=sql ||' union select brand as key, id::character varying as value,''Brand''::character varying as dd_type,''''::character varying as type
from isp_brand_master where type_id='||typeid||' ';
end if;
if brandid !=0 then
sql:=sql ||' union select model as key, id::character varying as value,''Model''::character varying as dd_type,''''::character varying as type
from isp_base_model where brand_id='||brandid||' ';
end if;
if specification != '' then
sql:=sql ||' union select distinct i.vendor_id::character varying as key , v.name::character varying as value,''Vendor''::character varying as ddtype,''''::character varying as type
from item_template_master i inner join vendor_master v on i.vendor_id= v.id where lower(i.specification)=lower('''||specification||''' ) and i.is_active=true and i.is_master=true';
end if;

sql:= sql ||' order by dd_type,key';

RETURN QUERY
EXECUTE sql;
END;
$BODY$;