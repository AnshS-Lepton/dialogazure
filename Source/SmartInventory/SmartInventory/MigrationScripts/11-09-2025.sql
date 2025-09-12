
---------------------- Add Site type in this view----------------------------------

CREATE OR REPLACE VIEW public.vw_att_details_pod_map
 AS
 SELECT
        CASE
            WHEN pod.agg_01 IS NULL THEN '0'::text
            ELSE '1'::text
        END AS agg_02,
        CASE
            WHEN dm.dropdown_value::text = 'Access'::text THEN '0'::text
            WHEN dm.dropdown_value::text = 'Aggregate'::text THEN '1'::text
            WHEN dm.dropdown_value::text = 'Core'::text THEN '2'::text
            ELSE '3'::text
        END AS agg,
    pod.system_id,
    pod.region_id,
    pod.province_id,
    COALESCE(adi.updated_geom, point.sp_geometry) AS sp_geometry,
    pod.pod_name,
    pod.project_id,
    pod.planning_id,
    pod.purpose_id,
    pod.workorder_id,
    pod.gis_design_id,
    pod.status,
    pod.is_new_entity,
    COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pod.network_status::text) AS network_status,
    pod.ownership_type,
    pod.third_party_vendor_id,
    pod.source_ref_id,
    pod.source_ref_type,
    lim.icon_path,
    (pod.site_id::text || '-'::text) || pod.site_name::text AS label_column,
    COALESCE(pod.is_agg_site, false) AS is_agg_site,
    pod.system_id AS site_system_id,
	 pod.pod_type
   FROM point_master point
     JOIN att_details_pod pod ON point.system_id = pod.system_id AND point.entity_type::text = 'POD'::text
     LEFT JOIN vw_att_details_edit_entity_info adi1 ON adi1.entity_system_id = pod.system_id AND adi1.entity_type::text = 'pod'::text AND adi1.description::text = 'NS'::text
     LEFT JOIN vw_layer_icon_map lim ON lim.network_abbreviation::text = COALESCE(adi1.attribute_info ->> lower('curr_status'::text), pod.network_status::text) AND lim.layer_name::text = 'POD'::text
     LEFT JOIN att_details_edit_entity_info adi ON adi.entity_system_id = pod.system_id AND adi.entity_type::text = 'POD'::text AND COALESCE(adi.updated_geom::character varying, ''::character varying)::text <> ''::text
     LEFT JOIN dropdown_master dm ON dm.layer_id = lim.layer_id;

ALTER TABLE public.vw_att_details_pod_map
    OWNER TO postgres;


    ------------------------------ Add ODF's used port in required format----------------------------------


CREATE OR REPLACE FUNCTION public.fn_get_fiber_link_detailsbyid(
	p_systemid integer,
	p_searchby character varying,
	p_searchtext character varying,
	p_pageno integer,
	p_pagerecord integer,
	p_sortcolname character varying,
	p_sorttype character varying,
	p_userid integer,
	p_searchfrom timestamp without time zone,
	p_searchto timestamp without time zone)
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
   TotalRecords INTEGER; 
   LowerStart  character varying;
   LowerEnd  character varying;
   v_user_role_id integer;
   s_layer_columns_val text; 
    main_link_Id  character varying;
	CableCount    INTEGER; 

BEGIN

sql:='';
LowerStart:='';
LowerEnd:='';

 --IF (coalesce(P_SORTCOLNAME,'')!='') THEN  
    -- LowerStart:='LOWER(';
    -- LowerEnd:=')';
--END IF;
  if(p_sortcolname='OP_ALIAS')then p_sortcolname:='Service Id'; end if;
select role_id into v_user_role_id from user_master where user_id=p_userId;

-- FETCH ALL COLUMNS FROM COLUMN SETTINGS TABLE
	 
	SELECT STRING_AGG(COLUMN_NAME||' as "'||case when COALESCE(res_field_key,'') ='' then DISPLAY_NAME else res_field_key  end||'"', ',') INTO S_LAYER_COLUMNS_VAL FROM(
	 SELECT  COLUMN_NAME,DISPLAY_NAME,res_field_key  FROM fiber_link_columns_settings WHERE is_active=true  and UPPER(column_name) not in('TOTAL_ROUTE_LENGTH','GIS_LENGTH','SUB_LINK_ID') order by Column_sequence) A;
	 

--- TO GET TOTAL_ROUTE_LENGTH & GIS_LENGTH
CREATE TEMP TABLE TEMP_CABLE_INFO(
 LINK_SYSTEM_ID INTEGER,
 CABLE_ID INTEGER
)ON COMMIT DROP;
 
 
 --- TO GET TOTAL_ROUTE_LENGTH & GIS_LENGTH
CREATE TEMP TABLE TEMP_SITE_INFO(
 CABLE_ID INTEGER,
	link_system_id INTEGER,
	siteA_id INTEGER,
	siteA character varying,
	siteA_Name character varying,
	siteB_id INTEGER,
	siteB character varying,
	siteB_Name character varying
)ON COMMIT DROP;

CREATE TEMP TABLE TEMP_FIBER_LINK_DETAILS(
  LINK_SYSTEM_ID INTEGER,
  TOTAL_ROUTE_LENGTH DOUBLE PRECISION,
  GIS_LENGTH DOUBLE PRECISION,
	a_network_id character varying
 )ON COMMIT DROP;

INSERT INTO TEMP_CABLE_INFO(LINK_SYSTEM_ID,CABLE_ID)
SELECT DISTINCT LINK_SYSTEM_ID,CABLE_ID FROM ATT_DETAILS_CABLE_INFO C
JOIN ATT_DETAILS_FIBER_LINK FIBER ON FIBER.SYSTEM_ID=C.LINK_SYSTEM_ID WHERE LINK_SYSTEM_ID>0;

 
INSERT INTO TEMP_FIBER_LINK_DETAILS(LINK_SYSTEM_ID,TOTAL_ROUTE_LENGTH,GIS_LENGTH)
 
SELECT DISTINCT T.LINK_SYSTEM_ID,ROUND((SUM(COALESCE(CABLE.cable_calculated_length,0)) + SUM(COALESCE(LOOP.LOOP_LENGTH,0)))::NUMERIC,2) AS TOTAL_ROUTE_LENGTH ,
ROUND((SUM(COALESCE(CABLE.CABLE_MEASURED_LENGTH,0)))::NUMERIC,2) AS CABLE_MEASURED_LENGTH
FROM TEMP_CABLE_INFO T
JOIN ATT_DETAILS_CABLE CABLE ON CABLE.SYSTEM_ID=T.CABLE_ID
LEFT JOIN( SELECT SUM(Loop_length) as loop_length,cable_system_id from ATT_DETAILS_LOOP LOOP group by LOOP.cable_system_id)loop ON LOOP.CABLE_SYSTEM_ID= CABLE.SYSTEM_ID
group by T.link_system_id
ORDER BY T.link_system_id DESC;

 CableCount = (select count(1) from TEMP_FIBER_LINK_DETAILS);
raise info'FIBER Count % ',CableCount;

insert into TEMP_SITE_INFO(CABLE_ID,link_system_id,siteA_id,siteB_id)
SELECT l.system_id,cl.link_system_id, ( SELECT p.system_id FROM point_master p
        WHERE ST_DWithin( p.sp_geometry::geography,ST_StartPoint(l.sp_geometry)::geography,5) and  p.entity_type ='POD'
        ORDER BY ST_Distance(p.sp_geometry::geography,ST_StartPoint(l.sp_geometry)::geography) LIMIT 1) AS siteA,
    ( SELECT p.system_id FROM point_master p WHERE ST_DWithin(
                  p.sp_geometry::geography,ST_EndPoint(l.sp_geometry)::geography,2) and p.entity_type ='POD'
        ORDER BY ST_Distance(p.sp_geometry::geography,ST_EndPoint(l.sp_geometry)::geography) LIMIT 1) AS siteB
FROM line_master l left join TEMP_CABLE_INFO cl on cl.CABLE_ID=l.system_id
where l.system_id in(
(select distinct cl.CABLE_ID from TEMP_CABLE_INFO cl
inner join TEMP_FIBER_LINK_DETAILS fl on fl.LINK_SYSTEM_ID=cl.link_system_id where (p_systemid IS NULL OR fl.LINK_SYSTEM_ID = p_systemid)
))
and  l.entity_type='Cable';

CableCount = (select count(1) from TEMP_SITE_INFO);

raise info'Site Count % ',CableCount;

-- Update SiteA_Name
UPDATE TEMP_SITE_INFO st
SET siteA_Name = sub.combined_name,siteA=sub.site_id
FROM (
    SELECT adp.system_id,
          adp.site_name ||
           CASE 
               WHEN COUNT(ci.source_port_no) > 0 
                    THEN '-ODF-' || STRING_AGG(ci.source_port_no::text, '-' ORDER BY ci.source_port_no)
               ELSE ''
           END AS combined_name,adp.site_id
    FROM att_details_pod adp
    LEFT JOIN att_details_fms af 
           ON af.parent_system_id = adp.system_id
    LEFT JOIN connection_info ci 
           ON ci.source_system_id = af.system_id and ci.destination_entity_type='FMS'
    GROUP BY adp.system_id, adp.site_name,adp.site_id
) sub
WHERE st.siteA_id = sub.system_id;

-- Update SiteB_Name
UPDATE TEMP_SITE_INFO st
SET siteB_Name = sub.combined_name,siteB=sub.site_id
FROM (
    SELECT adp.system_id,
           adp.site_name ||
           COALESCE(
               CASE 
                   WHEN COUNT(ci.source_port_no) > 0 
                        THEN '-ODF-' || STRING_AGG(ci.source_port_no::text, '-' ORDER BY ci.source_port_no)
                   ELSE ''
               END,
               ''
           ) AS combined_name,adp.site_id
    FROM att_details_pod adp
    LEFT JOIN att_details_fms af 
           ON af.parent_system_id = adp.system_id
    LEFT JOIN connection_info ci 
           ON ci.source_system_id = af.system_id and ci.destination_entity_type='FMS'
    GROUP BY adp.system_id, adp.site_name,adp.site_id
) sub
WHERE st.siteB_id = sub.system_id;
 
-- MANAGE SORT COLUMN NAME
IF (coalesce(TRIM(P_SORTCOLNAME,''))!='') THEN 

SELECT TRIM( trailing '	' from ''||P_SORTCOLNAME||'') into P_SORTCOLNAME;
select column_name into P_SORTCOLNAME from fiber_link_columns_settings WHERE UPPER(DISPLAY_NAME)=UPPER(P_SORTCOLNAME) ;
End IF;

 raise info'P_SORTCOLNAME% ',P_SORTCOLNAME;
  raise info'S_LAYER_COLUMNS_VAL% ',S_LAYER_COLUMNS_VAL;

-- DYNAMIC QUERY
sql:= 'SELECT ROW_NUMBER() OVER (ORDER BY '|| LowerStart || CASE WHEN (P_SORTCOLNAME) = '' THEN 'fl.system_id' ELSE  'fl.'|| P_SORTCOLNAME END ||LowerEnd|| ' ' ||CASE WHEN (P_SORTTYPE) ='' THEN 'desc' else P_SORTTYPE end||')::Integer AS S_No, 
fl.system_id,fl.network_id as "Network Id",'' '' as "Allocations/Project",'||S_LAYER_COLUMNS_VAL||', f2.TOTAL_ROUTE_LENGTH AS "Total Route Length(meter)",f2.GIS_LENGTH AS "GIS Length(meter)"
, (select siteA from TEMP_SITE_INFO st where st.link_system_id= fl.system_id and st.siteA_id is not null limit 1) AS "Site A",(select siteA_Name from TEMP_SITE_INFO st where st.link_system_id= fl.system_id and st.siteA_id is not null limit 1)  AS "Site Name"
,(select siteB from TEMP_SITE_INFO st where st.link_system_id= fl.system_id and st.siteB_id is not null limit 1)  AS "Site B",(select siteB_Name from TEMP_SITE_INFO st where st.link_system_id= fl.system_id and st.siteB_id is not null limit 1) AS "Site ID"
FROM vw_att_details_fiber_link fl
left join TEMP_FIBER_LINK_DETAILS f2 on fl.system_id=f2.link_system_id
WHERE 1=1  ';
	

 IF(p_systemid >0 )THEN 
	sql:= sql ||'and fl.system_id='||p_systemid||'';
 END IF;
--  IF(v_user_role_id!=1)THEN
-- 	sql:= sql ||'and fl.created_by_id='||p_userId||'';
--  END IF;
 raise info'sql2 % ',sql;
 raise info'p_searchby % ',p_searchby;

 IF ((p_searchtext) != '' and (p_searchby) != '') THEN
 if(p_searchby='sub_link_id')
 then 
 select fb.main_link_id into main_link_Id from vw_att_details_fiber_link fb where fb.link_id=p_searchtext;
 p_searchtext := main_link_Id;
 p_searchby := 'link_id';
 end if;
 if(p_searchby='OP_ALIAS')
 then 
 p_searchby := 'service_id';
 end if;
sql:= sql ||' AND lower('||p_searchby||'::text) LIKE lower(''%'||TRIM(p_searchtext)||'%'')';
END IF;
   
IF(p_searchfrom IS NOT NULL and p_searchto IS NOT NULL) THEN
sql:= sql ||' AND fl.created_on::Date>= to_date('''||p_searchfrom||''', ''YYYY-MM-DD'') and fl.created_on::Date<=to_date('''||p_searchto||''', ''YYYY-MM-DD'')';

END IF;

sql:= sql ||' group  by fl.system_id,fl.network_id,fl.link_id,fl.service_id,fl.otdr_distance
,fl.link_type,fl.created_by,fl.created_on,fl.fiber_link_status,fl.main_link_id,f2.a_network_id
,f2.total_route_length,f2.gis_length,fl.link_name';

	-- GET TOTAL RECORD COUNT
EXECUTE   'SELECT COUNT(1)  FROM ('||sql||') as a ' INTO TotalRecords;
 
--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((P_PAGENO) <> 0) THEN
	StartSNo:=  P_PAGERECORD * (P_PAGENO - 1 ) + 1;
	EndSNo:= P_PAGENO * P_PAGERECORD;
	sql:= 'SELECT '||TotalRecords||' as totalRecords,*
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ') T';                  
 END IF; 
RAISE INFO '%', sql;
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$BODY$;

ALTER FUNCTION public.fn_get_fiber_link_detailsbyid(integer, character varying, character varying, integer, integer, character varying, character varying, integer, timestamp without time zone, timestamp without time zone)
    OWNER TO postgres;
