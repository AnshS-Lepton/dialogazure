CREATE OR REPLACE FUNCTION public.fn_get_audit_entity_geometry_detail(p_pageno integer, p_pagerecord integer, p_sortcolname character varying, p_sorttype character varying, p_system_id integer, p_entity_name character varying)
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$



 DECLARE
   sql TEXT;
   StartSNo    INTEGER;   
   EndSNo      INTEGER;
   TotalRecords integer; 
   v_geom_type character varying;
   v_layer_table character varying;
   v_entity_title character varying;

BEGIN

-- DYNAMIC QUERY
select geom_type,layer_table,layer_title into v_geom_type,v_layer_table,v_entity_title   from layer_details where upper(layer_name)=upper(p_entity_name);
 
 if(upper(p_entity_name) = 'BUILDING') and (select 1 from att_details_building where system_id = p_system_id and building_status = 'Approved' ) = 1 then
    v_geom_type = 'Polygon';
  end if;
 
 if(upper(coalesce(p_sortcolname,''))=upper('Area(m²)'))THEN
p_sortcolname='area';
else if(upper(coalesce(p_sortcolname,''))=upper('gis length(mtr)'))THEN
p_sortcolname='length';
else if(upper(coalesce(p_sortcolname,''))=upper('GIS Length(meter)'))THEN
p_sortcolname='length'; 
else IF (coalesce(p_sortcolname,'')!='') THEN 
 	select replace(p_sortcolname , ' ', '_') into p_sortcolname;
 	RAISE INFO '%', p_sortcolname;
 End IF;
End IF; 
End IF;
End IF;

sql:= 'SELECT  ROW_NUMBER() OVER (ORDER BY '|| CASE WHEN (p_sortcolname) = '' THEN 'nt.audit_id' ELSE p_sortcolname END || ' ' ||CASE WHEN (p_sorttype) ='' THEN 'asc' else p_sorttype end||') AS S_No
  ,nt.system_id,nt.entity_type,nt.geom,nt.audit_id,'''||v_geom_type||''' as geom_type,nt.network_id as SI_ISP_GBL_GBL_GBL_008,'||case when v_geom_type='Point' then 'nt.latitude as SI_OSP_GBL_GBL_GBL_106,nt.longitude as SI_OSP_GBL_GBL_GBL_012 '
  when v_geom_type='Line' then 'nt.length  as SI_OSP_CAB_NET_HIS_006'  ELSE 'nt.area as SI_ISP_UNT_NET_FRM_007' end ||',created_by as SI_OSP_GBL_GBL_GBL_056,nt.created_on as SI_OSP_GBL_GBL_GBL_055,'''||v_entity_title||''' as entity_title ,modified_by as SI_OSP_GBL_GBL_GBL_058  ,modified_on as SI_OSP_GBL_GBL_GBL_057   from vw_audit_'||v_geom_type||'_master nt
  where nt.system_id='||p_system_id||' and upper(nt.entity_type)='''||upper(p_entity_name)||'''limit 2 ';
-- GET TOTAL RECORD COUNT
EXECUTE  'SELECT COUNT(1)  FROM ('||sql||') as a' INTO TotalRecords;

--GET PAGE WISE RECORD (FOR PARTICULAR PAGE)
 IF((p_pageno) <> 0) THEN
	StartSNo:=  p_pagerecord * (p_pageno - 1 ) + 1;
	EndSNo:= p_pageno * p_pagerecord;
	sql:= 'SELECT '||TotalRecords||' as totalRecords, *
                FROM (' || sql || ' ) T 
                WHERE S_No BETWEEN ' || StartSNo || ' AND ' || EndSNo; 

 ELSE
         sql:= 'SELECT '||TotalRecords||' as totalRecords, * FROM (' || sql || ' ) T ';                  
 END IF; 

RAISE INFO '%', sql;
	
RETURN QUERY
EXECUTE 'select row_to_json(row) from ('||sql||') row';

END ;

$function$
;
