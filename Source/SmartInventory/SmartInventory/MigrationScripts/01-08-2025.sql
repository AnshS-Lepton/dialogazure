
---------------------------------------------- Update file size value while uploading files----------------------------------

update global_settings set value='20480' where key='MaxFileUploadSizeLimit';

----------------------------------Increase buffer range to retrieve details of nearby entities during segment creation.-------

DROP FUNCTION IF EXISTS public.fn_topology_get_selectedroute(character varying, integer, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_topology_get_selectedroute(
	route_geom character varying,
	p_agg1 integer,
	p_agg2 integer,
	p_user_id integer)
    RETURNS TABLE(route_id integer, route_name character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE 
    v_source_geom geometry;
	v_buffered_route geometry;
	v_agg1site_id integer;
	v_agg2site_id integer;
	v_Count integer;
	R_Count integer;
BEGIN

create temp table temp_cable
(

network_id character varying

) on commit drop;

    -- Convert input string to a valid LINESTRING geometry
    

 -- Buffer in meters: Convert to geography, buffer, then back to geometry
    v_buffered_route := st_buffer_meters(
                                ST_GeomFromText('LINESTRING(' || route_geom || ')', 4326),  
                                100
                            );
                        
			RAISE INFO 'v_buffered_route -> %', v_buffered_route;			
   
	insert into temp_cable (network_id)
    SELECT 
        cbl.network_id AS route_id
    FROM 
        line_master l
    INNER JOIN 
        att_details_cable cbl 
        ON cbl.system_id = l.system_id
    WHERE 
        UPPER(l.entity_type) = 'CABLE'
        AND ST_Within(
            l.sp_geometry,
            v_buffered_route
        );
		
		select count(1) into v_Count from temp_cable;
		
		RAISE INFO 'Row Count -> %', v_Count;
	 IF v_Count > 0 THEN
	 
	 select count(1) into R_Count from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2 and tr.route_name ILike '%Manual_Route%';
	 --delete from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2 and tr.route_name='Manual_Route';
	 
	insert into top_routes(route_name,cable_names,total_length,route_geom,agg1site_id,agg2site_id)
	
	SELECT 
  'Manual_Route_' || (R_Count + 1)::text AS route_name,
  (select '{' || string_agg(network_id, ',') || '}' from temp_cable) as cable_names,
  0,
  v_source_geom AS  route_geom,
 p_agg1,
 p_agg2
 ;
 END IF;
 
 
 -- Return matching cable lines within 3 meters
   
    RETURN QUERY
    select Max(tr.route_id) AS route_id,tr.route_name from top_routes tr where agg1site_id=p_agg1 and agg2site_id=p_agg2 and tr.route_name ILike '%Manual_Route%'
    GROUP BY tr.route_name order by 1 desc ;

END;
$BODY$;


--------------------------------------- Site owner related changes in project details--------------------------

CREATE OR REPLACE FUNCTION public.get_site_project_details(
	p_site_id text)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
    RETURN QUERY
    SELECT row_to_json(t)
    FROM (
        SELECT 
		sp.id,sp.project_id, sp.site_id,sp.site_name,pod.owner_name as site_owner,sp.project_category, sp.cable_plan_cores, sp.maximum_cost,
 sp.comment,sp.location_address,sp.ds_cmc_area,sp.priority,pod.destination_site_id,pod.destination_port_type,pod.no_of_cores
        FROM site_project_details sp left join att_details_pod pod on pod.site_id=sp.site_id
        WHERE sp.site_id::TEXT = p_site_id
    ) t;
END;
$BODY$;

-----------------------------------------------------------------------------------------------------------------------------

DROP FUNCTION IF EXISTS public.fn_site_get_export_report_nearest_data_kml(character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, character varying, double precision, integer);

CREATE OR REPLACE FUNCTION public.fn_site_get_export_report_nearest_data_kml(
	p_networkstatues character varying,
	p_provinceids character varying,
	p_regionids character varying,
	p_layer_name character varying,
	p_searchby character varying,
	p_searchbytext character varying,
	p_fromdate character varying,
	p_todate character varying,
	p_geom character varying,
	p_duration_based_column character varying,
	p_radius double precision,
	p_userid integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


   DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   LowerStart  character varying;
   LowerEnd  character varying;
   TotalRecords integer; 
   WhereCondition character varying;
   s_report_view_name character varying;
   s_geom_type character varying;
s_layer_id integer; 
s_layer_columns text;
TotalAppliedRecords integer; 
TotalRejectedRecords integer;
BEGIN

LowerStart:='';
LowerEnd:='';
WhereCondition:='';

drop table if exists temp_site_pit;
  create temp table temp_site_pit(
  system_id integer,
  entity_type character varying,
  network_id character varying,
  entity_title character varying,
  geom character varying,
  geom_type character varying,
  nearest_site_geometry character varying
  ) on commit drop;

-- GET LAYER ID AND REPORT VIEW NAME--
SELECT LAYER_ID, REPORT_VIEW_NAME, GEOM_TYPE INTO S_LAYER_ID, S_REPORT_VIEW_NAME,S_GEOM_TYPE FROM LAYER_DETAILS WHERE LOWER(LAYER_NAME) = LOWER(P_LAYER_NAME);

-- SELECT ALL ACTIVE LAYER FIELDS FROM LAYER COLUMN SETTINGS IN DEFINED ORDER..	
SELECT STRING_AGG(COLUMN_NAME||' as "'||DISPLAY_NAME||'"', ',') INTO S_LAYER_COLUMNS FROM (
SELECT  COLUMN_NAME,DISPLAY_NAME  FROM LAYER_COLUMNS_SETTINGS WHERE LAYER_ID=S_LAYER_ID AND UPPER(SETTING_TYPE)='REPORT' AND IS_ACTIVE=TRUE ORDER BY COLUMN_SEQUENCE) A;

-- DYNAMIC QUERY
IF ((p_geom) != '') THEN

	if(p_radius>0)
	then
		select substring(left(St_astext(ST_buffer_meters(ST_GeomFromText('POINT('||p_geom||')',4326),p_radius)),-2),10)  into p_geom;
	end if;
	
	sql:= 'select a.system_id, '''||p_layer_name||''' as entity_type,ST_ASTEXT(sp_geometry) as geom, a.network_id,''Point'' as geom_type,a.site_id as entity_title, ST_ASTEXT(nearest_site_geometry) as nearest_site_geometry from att_details_pod a 
	inner join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''') and ST_Intersects(m.sp_geometry, st_geomfromtext(''POINT(('||p_geom||'))'',4326)) 
	inner join user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and a.total_fiber_distance > 0 and  1=1 ';	

ELSE

	sql:= ' select a.system_id,''Site'' as entity_type,ST_ASTEXT(sp_geometry) as geom, a.network_id,''Point'' as geom_type,a.site_id as entity_title, ST_ASTEXT(nearest_site_geometry) as nearest_site_geometry from att_details_pod a 
		inner join '||s_geom_type||'_master m on a.system_id=m.system_id and upper(m.entity_type)=upper('''||p_layer_name||''')
		inner join user_permission_area pa on pa.province_id = a.province_id where pa.user_id = '||p_userid||' and  a.total_fiber_distance > 0  ';
		
END IF;

IF ((p_networkStatues) != '' and upper(S_LAYER_COLUMNS) like '%NETWORK_STATUS%') THEN
	sql:= sql ||' AND upper(Cast(a.network_status as TEXT)) in('||p_networkStatues||')';
END IF;

IF ((p_RegionIds) != '') THEN
	sql:= sql ||' AND a.region_id IN ('||p_RegionIds||')';
END IF;

IF ((P_ProvinceIds) != '') THEN
	sql:= sql ||' AND a.province_id IN ('||P_ProvinceIds||')';
END IF;

IF ((P_searchbytext) != '' and (P_searchby) != '') 
THEN
	if(substring(P_searchbytext from 1 for 1)='"' and  substring(P_searchbytext from length(P_searchbytext) for length(P_searchbytext))='"')
	then
		sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) = upper(replace('''||trim(P_searchbytext)||''',''"'','''')) ';
	else
		sql:= sql ||' AND upper(Cast(a.'||P_searchby||' as TEXT)) LIKE upper(replace(''%'||trim(P_searchbytext)||'%'',''"'',''''))';
	end if;
END IF;

IF(P_fromDate != '' and P_toDate != '' and coalesce(p_duration_based_column,'')!='') THEN
sql:= sql ||' AND a.'||p_duration_based_column||'::Date>= to_date('''||p_fromdate||''', ''DD-Mon-YYYY'') and a.'||p_duration_based_column||'::Date<=to_date('''||p_todate||''', ''DD-Mon-YYYY'')';

END IF;

execute 'insert into temp_site_pit(system_id,entity_type,network_id,entity_title,geom,geom_type,nearest_site_geometry) select system_id,entity_type::character varying,network_id::character varying,entity_title::character varying,geom::character varying,geom_type::character varying, nearest_site_geometry::character varying from ('||sql||')x';

RAISE INFO 'QUERY %', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from (select * from temp_site_pit ) row';

END ;
$BODY$;
