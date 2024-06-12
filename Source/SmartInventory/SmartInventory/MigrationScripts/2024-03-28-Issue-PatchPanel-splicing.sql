CREATE OR REPLACE FUNCTION public.fn_splicing_delete_junk_connection(p_connector_system_id integer, p_connector_type character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
declare
v_arow record;
v_count integer;
BEGIN

--insert into temp_equipment(id,etype) values(p_connector_system_id,p_connector_type);

v_count:=0;
for v_arow in 
select * from (
select *,count(1) as total_record
from (
select destination_system_id,destination_entity_type,destination_port_no 
from connection_info where upper(source_entity_type) in ('CABLE','SPLITTER','FMS','PATCHPANEL') and upper(destination_entity_type) in ('ADB','BDB','CDB','FDB','SPLICECLOSURE')
union all
select source_system_id,source_entity_type,source_port_no 
from connection_info where upper(destination_entity_type) in ('CABLE','SPLITTER','FMS','PATCHPANEL') 
and upper(source_entity_type) in ('ADB','BDB','CDB','FDB','SPLICECLOSURE')
) a group by destination_system_id,destination_entity_type,destination_port_no 
    order by destination_system_id) b 
where total_record=1 and destination_system_id=p_connector_system_id and upper(destination_entity_type)=upper(p_connector_type)
loop

delete from connection_info 
where (source_system_id=v_arow.destination_system_id 
       and upper(source_entity_type)=upper(v_arow.destination_entity_type) 
       and  source_port_no=v_arow.destination_port_no) 
or (destination_system_id=v_arow.destination_system_id 
    and upper(destination_entity_type)=upper(v_arow.destination_entity_type)  
    and destination_port_no=v_arow.destination_port_no);
    
RAISE INFO '%', v_arow.destination_system_id||'  <---Port Number --->  '||v_arow.destination_port_no||'  <---con count --->  '||v_arow.total_record;

end loop;

END;
$function$
;