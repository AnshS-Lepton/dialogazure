CREATE OR REPLACE VIEW public.vw_att_details_fiber_link
AS SELECT link.system_id,
    link.link_id,
    link.status,
    link.link_name,
    link.link_type,
    link.network_id,
    link.start_point_type,
    link.start_point_network_id,
    link.start_point_location,
    link.end_point_type,
    link.end_point_network_id,
    link.end_point_location,
    link.no_of_lmc,
    link.each_lmc_length,
    link.total_route_length,
    link.gis_length,
    link.otdr_distance,
    link.no_of_pair,
    link.tube_and_core_details,
    link.existing_route_length_otdr,
    link.new_building_route_length,
    link.otm_length,
    link.otl_length,
        CASE
            WHEN link.any_row_portion = true THEN 'Yes'::text
            ELSE 'No'::text
        END AS any_row_portion,
    link.row_authority,
    link.total_row_segments,
    link.total_row_length,
    link.total_row_reccuring_charges,
    fn_get_date(link.handover_date, false) AS handover_date,
    fn_get_date(link.hoto_signoff_date, false) AS hoto_signoff_date,
    link.fiber_link_status,
    link.remarks,
    um.user_name AS created_by,
    fn_get_date(link.created_on) AS created_on,
    fn_get_date(link.modified_on) AS modified_on,
    um2.user_name AS modified_by,
    um.user_id AS created_by_id,
    link.service_id,
    link.main_link_type,
    link.main_link_id,
    link.redundant_link_type,
    link.redundant_link_id
    --pm1.latitude AS start_point_latitude,
    --pm1.longitude AS start_point_longitude,
    --pm2.latitude AS end_point_latitude,
    --pm2.longitude AS end_point_longitude,
    --cbl.status,
    --cbl.region_id,
    --cbl.province_id
    --fn_get_network_status(cbl.network_status) AS network_status
   FROM att_details_fiber_link link
     JOIN user_master um ON link.created_by = um.user_id
     LEFT JOIN user_master um2 ON link.modified_by = um2.user_id
     --LEFT JOIN point_master pm1 ON replace(link.start_point_type::text, ' '::text, ''::text) = pm1.entity_type::text AND link.start_point_network_id::text = pm1.common_name::text
     --LEFT JOIN point_master pm2 ON replace(link.end_point_type::text, ' '::text, ''::text) = pm1.entity_type::text AND link.end_point_network_id::text = pm1.common_name::text
     --LEFT join (select distinct link_system_id,cable_id from att_details_cable_info) ci ON ci.link_system_id = link.system_id
     --LEFT join att_details_cable cbl ON ci.cable_id = cbl.system_id;
     --where link.network_id='LNK005482'
	 --------------------------------------------------------------------------------------------------------------------------------------------
	 CREATE OR REPLACE FUNCTION public.fn_get_search_entity_result(p_entityname text, p_searchtext character varying, p_user_id integer, p_columnname character varying)
 RETURNS TABLE(systemid integer, networkid character varying, networkstatus character varying, status character varying, entityname text, entitytitle text, label character varying, value character varying, geomtype text, labellength integer)
 LANGUAGE plpgsql
AS $function$
DECLARE s_columns character varying ; 
	s_tablename character varying;
	s_geomtype character varying;
	s_colname character varying;
s_entitytype character varying;
s_entitytitle character varying;
        sql text;
       curSearchColumns refcursor;  
       v_role_id integer;       
BEGIN

select role_id into v_role_id from user_master where user_id=p_user_id;

sql:='';
 --select s.search_columns,s.table_name,s.geo_type  into s_columns,s_tablename,s_geomtype from   search_entity s where lower(s.entity_name)=lower(p_entityname);

--Commented the below select line and add new one with changes to use REPORT VIEW NAME not LAYER VIEW because from now on we use report view due to the implementation of additional attributes
--select srch_setting.search_columns,lyr_detail.layer_view, lyr_detail.geom_type,lyr_detail.layer_name,lyr_detail.layer_title 
select srch_setting.search_columns,lyr_detail.report_view_name, lyr_detail.geom_type,lyr_detail.layer_name,lyr_detail.layer_title 
into s_columns,s_tablename,s_geomtype,s_entitytype,s_entitytitle  from search_settings as srch_setting
inner join layer_details as lyr_detail on lyr_detail.layer_id = srch_setting.layer_id
where (lower(lyr_detail.layer_title) = lower(p_entityname) OR lower(lyr_detail.layer_name) = lower(p_entityname));-- p_entityname contain layer_title

if(p_columnName=null or p_columnName='')
THEN
if s_columns is not null and trim(s_columns)!='' then

		OPEN curSearchColumns FOR SELECT regexp_split_to_table(s_columns,E',');

			LOOP
				fetch  curSearchColumns into s_colname;
				-- exist from loop
				if not found then
					exit ;
				end if;	
				-------
				if upper(p_entityname)='BUILDING' then
					-- AS PER CURRENT LOGIC BUIDLING GEOMETERY IS OF TWO TYPE POLYGON FOR  APPROVED BUILDING AND POINT FOR OTHERS.
					sql:=sql||' select system_id as systemid ,network_id as networkId,network_status as network_status, building_status as status,'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					case when building_status=''Approved'' then ''Polygon''::text else  ''Point''::text  end as geomtype,region_id,province_id from '||s_tablename||' 
					where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
				elsif upper(p_entityname)='ROW' then
					sql:=sql||' select system_id as systemid ,network_id as networkId,''''::character varying as network_status,''''::character varying as status,
					'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					'''||s_geomtype||'''::text  as geomtype,region_id,province_id  from '||s_tablename||' 
					where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
				elsif upper(p_entityname)='CABLE' then
					sql:=sql||' select system_id as systemid ,network_id as networkId,network_status, status,
					'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					'''||s_geomtype||'''::text  as geomtype,region_id,province_id from '||s_tablename||' 
					where  upper('||s_tablename||'.cable_type::character varying)!=''ISP'' and  upper('||s_colname||'::text)  like ''%'||upper(p_searchtext)|| '%'' union';
						
				else
					IF Exists(SELECT 1 FROM information_schema.columns WHERE upper(table_name) = upper(s_tablename) AND upper(column_name) = 'NETWORK_STATUS') THEN

						sql:=sql||' select system_id as systemid ,network_id as networkId,network_status::character varying as network_status, status,
						'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
						('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
						'''||s_geomtype||'''::text  as geomtype,region_id,province_id from '||s_tablename||' 
						where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
					 
					else

			IF Exists(SELECT 1 FROM information_schema.columns WHERE upper(table_name) = upper(s_tablename) AND upper(column_name) in('REGION_ID','PROVINCE_ID')) 
			THEN

				sql:=sql||' select system_id as systemid ,network_id as networkId,''''::character varying as network_status, status,
					'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					'''||s_geomtype||'''::text  as geomtype,region_id,province_id from '||s_tablename||' 
					where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
			else
				sql:=sql||' select system_id as systemid ,network_id as networkId,''''::character varying as network_status, status,
					'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					'''||s_geomtype||'''::text  as geomtype from '||s_tablename||' 
					where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
			end if;

					
					end if;
				end if;
				
				
			END LOOP;
		close curSearchColumns;
end if;

if s_columns is not null and trim(s_columns)!='' then
sql:=RTRIM(sql, 'union');

--comment the below sql ,and change the condition for network_status to use with report view

--sql:='select  a.systemid,a.networkId,a.network_status,a.status,a.entityname,a.entitytitle,a.label,a.value,a.geomtype from ('||sql||') a 
--inner join layer_details l on upper(a.entityname)=upper(l.layer_name)
--inner  join role_permission_entity rp  on l.layer_id  = rp.layer_id and rp.role_id ='||v_role_id||' and rp.network_status =(case when a.network_Status='''' then ''A'' else a.network_Status end) and rp.viewonly = true
--inner join vw_user_permission_area upa on upa.region_id=a.region_id and upa.province_id=a.province_id and user_id='||p_user_id||' limit 10';

sql:='select distinct a.systemid,a.networkId,a.network_status::character varying,a.status,a.entityname,a.entitytitle,a.label,a.value,a.geomtype,length(a.label::text) labellength from ('||sql||') a 
inner join layer_details l on upper(a.entityname)=upper(l.layer_name)
inner  join role_permission_entity rp  on l.layer_id  = rp.layer_id and rp.role_id ='||v_role_id||' and rp.network_status =(case when a.network_Status='''' then ''A'' when upper(a.network_Status)=''PLANNED'' then ''P'' when upper(a.network_Status)=''AS BUILT'' then ''A'' when upper(a.network_Status)=''DORMANT'' then ''D'' end) and rp.viewonly = true';

IF Exists(SELECT 1 FROM information_schema.columns WHERE upper(table_name) = upper(s_tablename) AND upper(column_name) in('REGION_ID','PROVINCE_ID')) 
THEN
sql:=sql||' inner join vw_user_permission_area upa on upa.region_id=a.region_id and upa.province_id=a.province_id and user_id='||p_user_id||' order by length(a.label::text) ';
end if;

sql:=sql||'  limit 10 ';
raise info '%',sql;
   RETURN QUERY EXECUTE sql;
end if;

else
s_columns=p_columnName;
 

		OPEN curSearchColumns FOR SELECT regexp_split_to_table(s_columns,E',');

			LOOP
				fetch  curSearchColumns into s_colname;
				-- exist from loop
				if not found then
					exit ;
				end if;	
				-------
				if upper(p_entityname)='BUILDING' then
					-- AS PER CURRENT LOGIC BUIDLING GEOMETERY IS OF TWO TYPE POLYGON FOR  APPROVED BUILDING AND POINT FOR OTHERS.
					sql:=sql||' select system_id as systemid ,network_id as networkId,network_status as network_status, building_status as status,'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					case when building_status=''Approved'' then ''Polygon''::text else  ''Point''::text  end as geomtype,region_id,province_id from '||s_tablename||' 
					where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
				elsif upper(p_entityname)='ROW' then
					sql:=sql||' select system_id as systemid ,network_id as networkId,''''::character varying as network_status,''''::character varying as status,
					'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					'''||s_geomtype||'''::text  as geomtype,region_id,province_id  from '||s_tablename||' 
					where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
				elsif upper(p_entityname)='CABLE' then
					sql:=sql||' select system_id as systemid ,network_id as networkId,network_status, status,
					'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					'''||s_geomtype||'''::text  as geomtype,region_id,province_id from '||s_tablename||' 
					where  upper('||s_tablename||'.cable_type::character varying)!=''ISP'' and  upper('||s_colname||'::text)  like ''%'||upper(p_searchtext)|| '%'' union';
						
				else
					IF Exists(SELECT 1 FROM information_schema.columns WHERE upper(table_name) = upper(s_tablename) AND upper(column_name) = 'NETWORK_STATUS') THEN

						sql:=sql||' select system_id as systemid ,network_id as networkId,network_status::character varying as network_status, status,
						'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
						('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
						'''||s_geomtype||'''::text  as geomtype,region_id,province_id from '||s_tablename||' 
						where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
					 
					else
			
					sql:=sql||' select system_id as systemid ,network_id as networkId,''''::character varying as network_status, status,
					'''||s_entitytype||'''::text as entityname,'''||s_entitytitle||'''::text as entitytitle,
					('||s_colname||'::character varying) as label, ('||s_colname||'::character varying) as value, 
					'''||s_geomtype||'''::text  as geomtype,region_id,province_id from '||s_tablename||' 
					where  upper('||s_colname||'::text) like ''%'||upper(p_searchtext)|| '%'' union';
					end if;
				end if;
				
				
			END LOOP;
		close curSearchColumns;

 
 
sql:=RTRIM(sql, 'union');

--comment the below sql ,and change the condition for network_status to use with report view

--sql:='select  a.systemid,a.networkId,a.network_status,a.status,a.entityname,a.entitytitle,a.label,a.value,a.geomtype from ('||sql||') a 
--inner join layer_details l on upper(a.entityname)=upper(l.layer_name)
--inner  join role_permission_entity rp  on l.layer_id  = rp.layer_id and rp.role_id ='||v_role_id||' and rp.network_status =(case when a.network_Status='''' then ''A'' else a.network_Status end) and rp.viewonly = true
--inner join vw_user_permission_area upa on upa.region_id=a.region_id and upa.province_id=a.province_id and user_id='||p_user_id||' limit 10';

sql:='select  a.systemid,a.networkId,a.network_status::character varying,a.status,a.entityname,a.entitytitle,a.label,a.value,a.geomtype,length(a.label::text) labellength from ('||sql||') a 
inner join layer_details l on upper(a.entityname)=upper(l.layer_name)
inner  join role_permission_entity rp  on l.layer_id  = rp.layer_id and rp.role_id ='||v_role_id||' and rp.network_status =(case when a.network_Status='''' then ''A'' when upper(a.network_Status)=''PLANNED'' then ''P'' when upper(a.network_Status)=''AS BUILT'' then ''A'' when upper(a.network_Status)=''DORMANT'' then ''D'' end) and rp.viewonly = true';

IF Exists(SELECT 1 FROM information_schema.columns WHERE upper(table_name) = upper(s_tablename) AND upper(column_name) in('REGION_ID','PROVINCE_ID')) 
THEN
sql:=sql||' inner join vw_user_permission_area upa on upa.region_id=a.region_id and upa.province_id=a.province_id and user_id='||p_user_id||' order by length(a.label::text) ';
end if;

sql:=sql||'  limit 10 ';

raise info '%',s_columns;
   RETURN QUERY EXECUTE sql;
 
end IF;
END
$function$
;
-------------------------------------------------------------------------------------------------------------------------------------------