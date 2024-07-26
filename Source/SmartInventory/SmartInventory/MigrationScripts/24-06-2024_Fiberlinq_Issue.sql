-- FUNCTION: public.fn_get_library_attachments_details(integer, character, character, character varying)

-- DROP FUNCTION IF EXISTS public.fn_get_library_attachments_details(integer, character, character, character varying);

CREATE OR REPLACE FUNCTION public.fn_get_library_attachments_details(
	p_entity_system_id integer,
	p_entity_type character,
	p_upload_type character,
	p_feature_name character varying)
    RETURNS TABLE(id integer, entity_system_id integer, entity_type character varying, org_file_name character varying, file_name character varying, file_extension character varying, file_location character varying, upload_type character varying, uploaded_by character varying, file_size integer, entity_feature_name character varying, uploaded_on timestamp without time zone, is_barcode_image boolean, is_meter_reading_image boolean, document_type character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


declare v_sql character varying;
BEGIN
v_sql:='select la.id,la.entity_system_id, la.entity_type,la.org_file_name,la.file_name,
la.file_extension,la.file_location,upload_type, um.user_name, la.file_size,la.entity_feature_name,
la.uploaded_on, la.is_barcode_image,la.is_meter_reading_image, la.document_type from library_attachments la 
inner join user_master um on la.uploaded_by = um.user_id
where entity_system_id='||p_entity_system_id||' and entity_type='''||p_entity_type||'''';
if(coalesce(p_feature_name,'')!='' and upper(p_entity_type)=upper('ROW'))
then
v_sql:=v_sql||' and upper(entity_feature_name)=upper('''||p_feature_name||''') ';
elsif(coalesce(p_feature_name,'')!='')
then
v_sql:=v_sql||' and upload_type='''||p_upload_type||''' and upper(entity_feature_name)=upper('''||p_feature_name||''')';
else
v_sql:=v_sql||' and upload_type='''||p_upload_type||'''';
end if;

RAISE INFO 'QUERY %', v_sql;
RETURN QUERY EXECUTE v_sql ;

END; 
$BODY$;


