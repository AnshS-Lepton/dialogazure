
CREATE OR REPLACE FUNCTION public.fn_splicing_get_query(
	p_source_system_id integer,
	p_source_entity_type character varying,
	p_is_source_a_end boolean,
	p_destination_system_id integer,
	p_destination_entity_type character varying,
	p_is_destination_a_end boolean,
	p_customer_ids character varying)
    RETURNS text
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$


DECLARE 
v_Query text;
v_subQuery text;
v_layer_table character varying; 
v_is_virtual_port_allowed boolean;
v_is_middleware_entity boolean;
connector record;
BEGIN

v_subQuery:='';		
	
-- QUERY TO FETCH CABLE CORE INFO FROM ATT_DETAILS_CABLE_INFO..
-- start cable core detail
v_Query:='select tbl1.system_id as system_id,tbl1.network_id,fn_get_display_name(tbl1.system_id, ''Cable'') as display_name, ''Cable'' as entity_type,tbl1.no_of_tube,tbl2.tube_number,tbl2.tube_color_code,tbl2.fiber_number as core_port_number,tbl2.core_color_code,portm.status as core_port_status,portm.color_code as core_port_status_color,'||p_is_source_a_end||' as is_cable_a_end,0 as is_parent,null as input_output,tbl2.link_system_id,tbl2.core_comment,
(case when '||p_is_source_a_end||' then a_end_status_id else b_end_status_id end) as port_status_id,lk.link_id as name
from att_details_cable tbl1 join att_details_cable_info tbl2  on tbl1.system_id=tbl2.cable_id 
left join port_status_master portm on portm.system_id=case when true='||p_is_source_a_end||' then tbl2.a_end_status_id else tbl2.b_end_status_id end
left join att_details_fiber_link lk on lk.system_id=tbl2.link_system_id
where tbl1.system_id='||p_source_system_id;
-- end cable core detail
v_Query:=v_Query||' union select tbl1.system_id as system_id,tbl1.network_id,fn_get_display_name(tbl1.system_id, ''Cable'') as display_name, ''Cable'' as entity_type,tbl1.no_of_tube,tbl2.tube_number,tbl2.tube_color_code,tbl2.fiber_number as core_port_number,tbl2.core_color_code,portm.status as core_port_status,portm.color_code as core_port_status_color,'||p_is_destination_a_end||' as is_cable_a_end,0 as is_parent,null as input_output,tbl2.link_system_id,tbl2.core_comment,
(case when '||p_is_destination_a_end||' then a_end_status_id else b_end_status_id end) as port_status_id ,lk.link_id as name
from att_details_cable tbl1 join att_details_cable_info tbl2  on tbl1.system_id=tbl2.cable_id 
left join port_status_master portm on portm.system_id=case when true='||p_is_destination_a_end||' then tbl2.a_end_status_id else tbl2.b_end_status_id end
left join att_details_fiber_link lk on lk.system_id=tbl2.link_system_id
where tbl1.system_id='||p_destination_system_id;

v_Query:=v_Query||' union select tblMain.system_id,tblMain.network_id, display_name,tblMain.entity_type,null as no_of_tube,null as tube_number,
null as tube_color_code,tblMain.core_port_number,null as core_color_code,tblMain.core_port_status,tblMain.core_port_status_color, false as is_cable_a_end, tblMain.is_parent,tblMain.input_output,
0 as link_system_id,tblMain.core_comment,tblMain.port_status_id,'''' as name  from(';


FOR connector IN select system_id,entity_type  from temp_connectors
LOOP

-- QUERY TO FETCH CONNECTOR PORT INFO AND DETAIL-- 
if(connector.system_id>0 and coalesce(connector.entity_type,'')!='') then

if(v_subQuery!='')
then 
	v_subQuery:=v_subQuery||' union ';
end if;

	-- GET CONNECTOR LAYER DETAIL..
	select layer_table,is_virtual_port_allowed,is_middleware_entity into v_layer_table,v_is_virtual_port_allowed,v_is_middleware_entity from layer_details where upper(layer_name)=upper(connector.entity_type);

	-- CHECK WHETHER VIRTUAL PORT INS ALLOWED--
	if(v_is_virtual_port_allowed) then

		-- IS VIRTUAL PORTS, THEN  FETCH ONLY CONNECTOR DETAIL--
		v_subQuery:=v_subQuery||' select en.system_id,en.network_id,fn_get_display_name(en.system_id, '''||connector.entity_type||''') as display_name,'''||connector.entity_type||''' as  entity_type,'||(case when v_is_middleware_entity then 0 else 1 end)||' as is_parent,null as core_port_number,null as input_output,
		null as core_port_status,null as core_port_status_color,null as core_comment,1 as port_status_id   from  '||v_layer_table||' en where en.system_id='||connector.system_id||' union';
	else
		-- IF PHYSICAL PORTS, THEN FETCH ITS  PORT INFO FROM ISP PORT INFO TABLE..
		v_subQuery:=v_subQuery||' select en.system_id,en.network_id,fn_get_display_name(en.system_id, '''||connector.entity_type||''') as display_name,'''||connector.entity_type||''' as  entity_type,'||(case when v_is_middleware_entity then 0 else 1 end)||' as is_parent,
		case when pInfo.input_output=''I'' then (-1*pInfo.port_number) else pInfo.port_number end as  core_port_number,
		pInfo.input_output,portm.status as core_port_status,portm.color_code as core_port_status_color,
		pInfo.port_comment as core_comment,pInfo.port_status_id from  '||v_layer_table||' en inner join isp_port_info pInfo on pInfo.parent_system_id=en.system_id
		and upper(pInfo.parent_entity_type)=upper('''||connector.entity_type||''') left join port_status_master portm on portm.system_id=pInfo.port_status_id 
		where en.system_id='||connector.system_id||' union';
	end if;


	--  QUERY TO FTECH CONNECTOR CHILD ELEMENTS PORT INFORMATION..

	--*****FETCH SPLITTER PORT INFO********---
	v_subQuery:=v_subQuery||' select spl.system_id,spl.network_id,fn_get_display_name(spl.system_id, ''Splitter'') as display_name,''Splitter'' as entity_type,0 as is_parent,case when pInfo.input_output=''I'' 
	then (-1*pInfo.port_number) else pInfo.port_number end as  core_port_number,pInfo.input_output,portm.status as core_port_status,portm.color_code as core_port_status_color,pInfo.port_comment as core_comment,
	pInfo.port_status_id 
	from att_details_splitter spl inner join isp_port_info pInfo on spl.system_id=pInfo.parent_system_id and upper(pInfo.parent_entity_type)=upper(''Splitter'') left join 
	port_status_master portm on portm.system_id=pInfo.port_status_id
	where spl.parent_system_id='||connector.system_id||' and upper(spl.parent_entity_type)=upper('''||connector.entity_type||''')';
	
	--*****FETCH FMS PORT INFO********---
	--query is pending..

	v_subQuery:=v_subQuery||' union select fms.system_id,fms.network_id,fn_get_display_name(fms.system_id,''FMS'') as display_name,''FMS'' as entity_type,0 as is_parent,case when pInfo.input_output=''I'' 
	then (-1*pInfo.port_number) else pInfo.port_number end as  core_port_number,pInfo.input_output,portm.status as core_port_status,portm.color_code as core_port_status_color,
	pInfo.port_comment,pInfo.port_status_id from att_details_fms fms 
	inner join isp_port_info pInfo on fms.system_id=pInfo.parent_system_id and upper(pInfo.parent_entity_type)=upper(''FMS'') left join 
	port_status_master portm on portm.system_id=pInfo.port_status_id
	where fms.parent_system_id='||connector.system_id||' and upper(fms.parent_entity_type)=upper('''||connector.entity_type||''')';


	--*****FETCH Repeater PORT INFO********---
	--query is pending..

	v_subQuery:=v_subQuery||' union select repeater.system_id,repeater.network_id,fn_get_display_name(repeater.system_id,''OpticalRepeater'') as display_name,''OpticalRepeater'' as entity_type,0 as is_parent,
	case when pInfo.input_output=''I'' 
	then (-1*pInfo.port_number) else pInfo.port_number end as  core_port_number,pInfo.input_output,portm.status as core_port_status,
	portm.color_code as core_port_status_color,
	pInfo.port_comment,pInfo.port_status_id from isp_opticalrepeater_info repeater 
	inner join isp_port_info pInfo on repeater.system_id=pInfo.parent_system_id and upper(pInfo.parent_entity_type)=upper(''OpticalRepeater'')
	left join port_status_master portm on portm.system_id=pInfo.port_status_id
	where repeater.parent_system_id='||connector.system_id||' and upper(repeater.parent_entity_type)=upper('''||connector.entity_type||''')'; 

	--*****FETCH PatchPanel PORT INFO********---
	--query is pending..

	v_subQuery:=v_subQuery||' union select  patch.system_id,patch.network_id,patchpanel_name as display_name,''PatchPanel'' as entity_type,0 as is_parent,case when pInfo.input_output=''I'' 
	then (-1*pInfo.port_number) else pInfo.port_number end as  core_port_number,pInfo.input_output,portm.status as core_port_status,portm.color_code as core_port_status_color,
	pInfo.port_comment,pInfo.port_status_id from att_details_patchpanel patch 
	inner join isp_port_info pInfo on patch.system_id=pInfo.parent_system_id and upper(pInfo.parent_entity_type)=upper(''PatchPanel'') left join 
	port_status_master portm on portm.system_id=pInfo.port_status_id
	where patch.parent_system_id='||connector.system_id||' and upper(patch.parent_entity_type)=upper('''||connector.entity_type||''')';
	
end if;

END LOOP;

v_Query:=v_Query||v_subQuery;

-- QUERY TO FETCH CUSTOMER INFORMATION--
if(coalesce(p_customer_ids,'')!='') then
	v_Query:=v_Query||' union select cust.system_id,cust.network_id||''(''||cust.customer_name||'')'' as network_id,'''' as display_name,''Customer'' as entity_type,0 as is_parent,1 core_port_number,''I'' as input_output,
	portm.status core_port_status,portm.color_code as core_port_status_color,null as core_comment,1 as port_status_id from att_details_customer cust left join 
	port_status_master portm on portm.system_id=cust.customer_status_id where cust.system_id in('||p_customer_ids||')';
end if;

v_Query:=v_Query||') tblMain order by system_id,tube_number ,core_port_number';

 return v_Query;                        
END
$BODY$;

ALTER FUNCTION public.fn_splicing_get_query(integer, character varying, boolean, integer, character varying, boolean, character varying)
    OWNER TO postgres;
-