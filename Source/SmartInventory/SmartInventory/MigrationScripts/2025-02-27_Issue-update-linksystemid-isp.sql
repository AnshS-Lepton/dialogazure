
update isp_port_info p set link_system_id = 0;


update isp_port_info p set link_system_id=c.link_system_id
from (
select a.*,c.link_system_id from connection_info  a 
inner join att_details_cable_info c on source_entity_type='FMS'  and a.destination_system_id=c.cable_id and a.destination_port_no=c.fiber_number
and destination_entity_type='Cable' and c.link_system_id>0)c
where p.parent_system_id=c.source_system_id and p.parent_entity_type=c.source_entity_type and  p.parent_entity_type='FMS' and p.port_number=c.source_port_no
and p.input_output='O';

update isp_port_info p set link_system_id=c.link_system_id
from (
select a.*,c.link_system_id from connection_info  a 
inner join att_details_cable_info c on destination_entity_type='FMS'  and a.source_system_id=c.cable_id and a.source_port_no=c.fiber_number
and source_entity_type='Cable' and c.link_system_id>0)c
where p.parent_system_id=c.destination_system_id and p.parent_entity_type=c.destination_entity_type and  p.parent_entity_type='FMS' and p.port_number=c.destination_port_no
and p.input_output='O';