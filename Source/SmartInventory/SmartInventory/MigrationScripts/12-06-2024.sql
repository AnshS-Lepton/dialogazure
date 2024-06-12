alter table att_details_model add column  source_ref_type character varying
,add column  source_ref_id character varying;


CREATE OR REPLACE FUNCTION public.fn_get_vector_layers_data_by_geom(
	p_prvinceids character varying,
	p_longitude double precision,
	p_latitude double precision,
	p_ticket_id integer)
    RETURNS SETOF json 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$


DECLARE

--select * from public.fn_get_vector_layers_data_by_geom('156',-1.27602,36.82734,53);
	--lprovince_id int = p_prvinceIds::int; --24;--3
   sql TEXT;
   v_entity_type character varying;
   v_entity_geojson_table character varying; 
   v_layer_name character varying;
   v_layer_col character varying;
   v_where_col character varying;
   v_arow record;
   v_sql character varying;
   v_geometry_table character varying;
   v_geom geometry;
   v_RADIUS int;
BEGIN
		--select  * from point_master limit 10
		v_sql:= '';		
		v_RADIUS = 1000;
		
		for v_arow in select layer_name,layer_table, delta_metadata_table, geom_type, entity_geojson_table from layer_details where is_vector_layer_implemented=true
		loop
			if(v_arow.geom_type ='Line')
			then
				v_geometry_table = 'line_master';
		    else if(v_arow.geom_type ='Point') Then
				v_geometry_table = 'point_master';
			else
				v_geometry_table = 'polygon_master';			
			end if;
			end if;
		
			if(v_sql!='')
			then
				v_sql:=v_sql||'
				union all
				';
			end if;
				If p_ticket_id = 0 Then
					v_sql:= v_sql||' SELECT '''||v_arow.layer_name||''' as layer, e.system_id, e.geojson AS feature FROM '||v_arow.entity_geojson_table||' e 
					inner join ' ||v_geometry_table ||' g on e.system_id = g.system_id and  g.entity_type = '''||v_arow.layer_name||'''
					where 
					--e.province_id ='||p_prvinceids|| ' AND
					 ST_INTERSECTS(g.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT(''POINT('|| p_longitude||' '||p_latitude||')'',4326),'||v_RADIUS||'))';
				Else
					-- v_sql:= v_sql||' SELECT '''||v_arow.layer_name||''' as layer, e.system_id, e.geojson AS feature FROM '||v_arow.entity_geojson_table||' e 
					-- 
					-- inner join ' ||v_geometry_table ||' g on e.system_id = g.system_id and  g.entity_type = '''||v_arow.layer_name||'''
					-- inner join polygon_master n on n.entity_type =''Network_Ticket'' AND ST_INTERSECTS(g.SP_GEOMETRY, n.SP_GEOMETRY) 
					-- where  
					-- --e.province_id ='||p_prvinceids|| ' AND
					-- n.system_id = '|| p_ticket_id;

					

					if(v_arow.layer_name='Network_Ticket')
					then
						v_layer_col='att.ticket_id';
						v_where_col='att.ticket_id='''||p_ticket_id||'''';
					else
						v_layer_col='att.system_id';
						v_where_col='att.source_ref_type=''Network_Ticket'' and att.source_ref_id='''||p_ticket_id||'''';
					end if;
					v_sql:= v_sql||' SELECT '''||v_arow.layer_name||''' as layer, e.system_id, e.geojson AS feature FROM '||v_arow.entity_geojson_table||' e 
					
					inner join ' ||v_geometry_table ||' g on e.system_id = g.system_id and  g.entity_type = '''||v_arow.layer_name||'''
					inner join polygon_master n on n.entity_type =''Network_Ticket'' AND ST_INTERSECTS(g.SP_GEOMETRY, n.SP_GEOMETRY) 
					inner join '||v_arow.layer_table||' att on '||v_layer_col||'=e.system_id
					where '||v_where_col||' and n.system_id = '|| p_ticket_id;
				End if;
		end loop;
		Raise info  ' Final Query - ,%',v_sql; 
		RAISE INFO '%', sql;        
		RETURN QUERY EXECUTE 'select row_to_json(row) as geo from ('||v_sql||')row';	
		
		
end 
$BODY$;

ALTER FUNCTION public.fn_get_vector_layers_data_by_geom(character varying, double precision, double precision, integer)
    OWNER TO postgres;