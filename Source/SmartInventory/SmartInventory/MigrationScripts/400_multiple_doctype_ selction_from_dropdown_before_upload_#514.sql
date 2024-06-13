 alter table Library_Attachments
 add column document_type character varying(100); 

--------------------------------------------------------------------------------------
INSERT INTO public.global_settings(
	key, value, description, type, is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
	VALUES ('AdddocumentRowNumber', 4, 'Add document Row Number', 'Web', false, 'int', 1, 10, 5, now(), 5, now(), false, true, false, null, null);

---------------------------------------------------------------------------------------------
-- FUNCTION: public.fn_get_upload_dropdownlist(character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_upload_dropdownlist(character varying);

CREATE OR REPLACE FUNCTION public.fn_get_upload_dropdownlist(
	dropdowntype character varying)
    RETURNS TABLE(dropdown_type character varying, dropdown_value character varying, dropdown_key character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
 DECLARE
    sql TEXT;
BEGIN

return query
select a.dropdown_type,a.dropdown_value,a.dropdown_key from(
select dd.id,dd.dropdown_type,dd.dropdown_value,dd.dropdown_key FROM dropdown_master dd 
where lower(dd.dropdown_type)=lower(dropdowntype))a order by id;
	
	
  END; 
$BODY$;

ALTER FUNCTION public.fn_get_upload_dropdownlist(character varying)
    OWNER TO postgres;

-----------------------------------------------------------------------------------------


-- FUNCTION: public.fn_get_library_attachments_details_withtype_opsisp(integer, character, character, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_library_attachments_details_withtype_opsisp(integer, character, character, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_library_attachments_details_withtype_opsisp(
	p_entity_system_id integer,
	p_entity_type character,
	p_upload_type character,
	p_feature_name character varying)
    RETURNS TABLE(id integer, entitysystemid integer, entitytype character varying, orgfilename character varying, filename character varying, fileextension character varying, filelocation character varying, uploadtype character varying, uploadedby character varying, file_size character varying, entity_feature_name character varying, uploaded_on timestamp without time zone, categorytype character varying, delete_action boolean, document_type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

declare v_sql character varying;
declare v_tblName character varying;
declare v_geom_type character varying;

BEGIN

if(upper(p_feature_name)=upper('SPECIFICATION')) THEN

v_sql:='select sa.id as Id,0::integer as EntitySystemId, ''''::character varying as EntityType,sa.org_file_name as OrgFileName ,sa.file_name as FileName ,sa.file_extension as FileExtension ,sa.file_location as FileLocation, sa.upload_type as UploadType, um.user_name as UploadedBy, sa.file_size::character varying ,''''::character varying as entity_feature_name, sa.uploaded_on, ''Specification''::character varying as categorytype,false as delete_action,sa.document_type from specification_attachments sa left join user_master um on sa.uploaded_by = um.user_id where upper(sa.upload_type)=upper('''||p_upload_type||''') and sa.id='||p_entity_system_id||'';
RAISE INFO '1 %', v_sql;

elsif(upper(p_feature_name)=upper('ENTITY')) THEN

v_sql:='select la.id as Id,la.entity_system_id as EntitySystemId, la.entity_type as EntityType,la.org_file_name as OrgFileName ,la.file_name as FileName ,la.file_extension as FileExtension ,la.file_location as FileLocation,upload_type as UploadType, um.user_name as UploadedBy, la.file_size::character varying ,la.entity_feature_name,la.uploaded_on, ''Entity''::character varying as categorytype,false as delete_action,la.document_type from library_attachments la inner join user_master um on la.uploaded_by = um.user_id where la.id='||p_entity_system_id||' and entity_type='''||p_entity_type||''' and upper(upload_type)=upper('''||p_upload_type||''')';

else
RAISE INFO '2 %', v_sql;
select layer_table,geom_type into v_tblName,v_geom_type from layer_details where upper(layer_name)=upper(p_entity_type) limit 1;
RAISE INFO 'v_tblName %', v_tblName;

IF(coalesce(v_tblName,'')='')THEN

RAISE INFO '22%', v_sql;
v_sql:='select la.id as Id,la.entity_system_id as EntitySystemId, la.entity_type as EntityType,la.org_file_name as OrgFileName ,la.file_name as FileName ,la.file_extension as FileExtension ,la.file_location as FileLocation,upload_type as UploadType, um.user_name as UploadedBy, la.file_size::character varying ,la.entity_feature_name,la.uploaded_on, ''Entity''::character varying as categorytype,true as delete_action,la.document_type from library_attachments la inner join user_master um on la.uploaded_by = um.user_id where la.entity_system_id='||p_entity_system_id||' and entity_type='''||p_entity_type||''' and upper(upload_type)=upper('''||p_upload_type||''')';
ELSE
RAISE INFO '33 %', v_sql;
v_sql:='select la.id as Id,la.entity_system_id as EntitySystemId, la.entity_type as EntityType,la.org_file_name as OrgFileName ,la.file_name as FileName ,la.file_extension as FileExtension ,la.file_location as FileLocation,upload_type as UploadType, 
um.user_name as UploadedBy, la.file_size::character varying ,la.entity_feature_name,la.uploaded_on, ''Entity''::character varying as categorytype,case when upper(att.network_status)=''P'' then planned_delete when upper(att.network_status)=''A'' then asbuild_delete else dormant_delete end as delete_action,la.document_type
 from library_attachments la inner join user_master um
 on la.uploaded_by = um.user_id inner join '||v_tblName||' att on att.system_id=la.entity_system_id 
inner join vw_network_layers vnl on  vnl.layer_id=(select layer_id from layer_details where upper(layer_name)=upper('''||p_entity_type||''')) and vnl.role_id=um.role_id
  where la.entity_system_id='||p_entity_system_id||' and upper(entity_type)=upper('''||p_entity_type||''') and upper(upload_type)=upper('''||p_upload_type||''')';

END IF;

if upper(v_geom_type) != 'POLYGON' and exists(SELECT column_name FROM information_schema.columns WHERE upper(table_name)=upper(v_tblName) and column_name in('vendor_id')) then
v_sql:=v_sql||'
union all
select sa.id as Id,0 as EntitySystemId, '''' as EntityType,sa.org_file_name as OrgFileName ,sa.file_name as FileName ,sa.file_extension as FileExtension ,sa.file_location as FileLocation,sa.upload_type as UploadType, um.user_name as UploadedBy,
sa.file_size::character varying ,'''' as entity_feature_name,sa.uploaded_on, ''Specification''::character varying as categorytype,case when upper(att.network_status)=''P'' then planned_delete when upper(att.network_status)=''A'' then asbuild_delete else dormant_delete end as delete_action,sa.document_type 
from '||v_tblName||' att
inner join item_template_master itm on att.vendor_id=itm.vendor_id and itm.code=att.Item_Code
inner join specification_attachments sa on sa.specification_id=itm.id
left join user_master um on sa.uploaded_by = um.user_id
inner join vw_network_layers vnl on  vnl.layer_id=(select layer_id from layer_details where upper(layer_name)=upper('''||p_entity_type||''')) and vnl.role_id=um.role_id
where att.system_id='||p_entity_system_id||' and upper(upload_type)=upper('''||p_upload_type||''')';

end if;
end if;

RAISE INFO 'QUERY %', v_sql;
RETURN QUERY EXECUTE v_sql ;

END; 
$BODY$;

---------------------------------------------------------------------------------------------------------------------

-- FUNCTION: public.fn_get_library_attachments_details_byid(integer, character, character, character, integer, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_library_attachments_details_byid(integer, character, character, character, integer, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_library_attachments_details_byid(
	p_entity_system_id integer,
	p_entity_type character,
	p_upload_type character,
	p_filename character,
	p_userid integer,
	p_feature_name character varying)
    RETURNS TABLE(id integer, entity_system_id integer, entity_type character varying, org_file_name character varying, file_name character varying, file_extension character varying, file_location character varying, upload_type character varying, uploaded_by character varying, file_size integer, entity_feature_name character varying, uploaded_on timestamp without time zone, is_barcode_image boolean, is_meter_reading_image boolean, document_type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$

declare v_sql character varying;
BEGIN
v_sql:='select la.id,la.entity_system_id, la.entity_type,la.org_file_name,la.file_name,la.file_extension,la.file_location,upload_type, um.user_name,
 la.file_size,la.entity_feature_name,la.uploaded_on,la.is_barcode_image,la.is_meter_reading_image,la.document_type from library_attachments la inner join user_master um on la.uploaded_by = um.user_id 
 where entity_system_id='||p_entity_system_id||' and um.user_id='||p_userId||' and entity_type='''||p_entity_type||'''';
if(coalesce(p_feature_name,'')!='' and upper(p_entity_type)=upper('ROW'))
then
	v_sql:=v_sql||' and upper(entity_feature_name)=upper('''||p_feature_name||''') ';
elsif(coalesce(p_feature_name,'')!='')
then
	v_sql:=v_sql||' and upload_type='''||p_upload_type||''' and org_file_name='''||p_fileName||'''  and upper(entity_feature_name)=upper('''||p_feature_name||''')';
else
	v_sql:=v_sql||' and org_file_name='''||p_fileName||'''  and upload_type='''||p_upload_type||'''';
end if;

RAISE INFO 'QUERY %', v_sql;
 RETURN QUERY EXECUTE v_sql ;

END; 
$BODY$;

ALTER FUNCTION public.fn_get_library_attachments_details_byid(integer, character, character, character, integer, character varying)
    OWNER TO postgres;


--------------------------------------------------------------------------------------------------------------------------------------------