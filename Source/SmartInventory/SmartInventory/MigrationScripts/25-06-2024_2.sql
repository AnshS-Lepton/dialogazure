CREATE OR REPLACE FUNCTION public.fn_get_vector_layers_delta_by_geom(
	p_lastfetchtime character varying,
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



DECLARE   sql TEXT;
   v_entity_type character varying;
   v_entities character varying;
   lprovince_id int = p_prvinceIds::int;
   v_sql character varying;
   v_arow record;
   v_RADIUS int;
   v_col character varying;
   v_where_clause character varying;
begin
	v_sql:='';
	v_RADIUS = 1000;
	--select  * from fn_get_vector_layers_delta_by_geom('2024-06-25 13:53:57.174886','156',-1.27602,36.82734,0);
	if coalesce(p_lastFetchTime, '') <> '' then
		for v_arow in select layer_name,layer_table,delta_metadata_table, geom_type from layer_details where is_vector_layer_implemented=true
		loop
			if(v_sql!='')
			then
				v_sql:=v_sql||' 
			union all ';
			end if;

			v_sql:=v_sql||' 
			SELECT '''||v_arow.layer_name||''' as layer, e.system_id, e.geojson AS feature, '''||v_arow.geom_type||''' as geom_type FROM '||v_arow.layer_name||'_geojson_master e ';

			IF  p_ticket_id > 0 
			Then
				if(v_arow.layer_name='Network_Ticket')
				then 
					v_col='att.ticket_id';
					v_where_clause='';
				else
					v_col='att.system_id';
					v_where_clause='upper(att.source_ref_type)=''NETWORK_TICKET'' and 
				att.source_ref_id='''||(p_ticket_id)||''' and';
				end if;
				v_sql:=v_sql||' 
				inner join '||v_arow.layer_table||' att on e.system_id= '||v_col||' WHERE  '||v_where_clause||'
				EXISTS(SELECT 1 FROM '||v_arow.delta_metadata_table||' d WHERE d.province_id = e.province_id and 
				d.province_id in ('||p_prvinceids||')
				and d.last_delta_at >= '''||(p_lastFetchTime)||'''::timestamp) ';
			ELSE 
				v_sql:=v_sql||'  
				inner join '||v_arow.geom_type||'_master p on p.system_id=e.system_id and P.status=''A''
				WHERE EXISTS(SELECT 1 FROM '||v_arow.delta_metadata_table||' d 				
				WHERE d.province_id = e.province_id and 
				d.province_id in ('||p_prvinceids||')
				and d.last_delta_at >= '''||(p_lastFetchTime)||'''::timestamp) ';
			END IF;
		end loop;
		Raise info  ' Final Query - ,%',v_sql; 
			
		IF  p_ticket_id = 0 Then			
		
			RETURN QUERY EXECUTE 'select row_to_json(row) as geo from ('||v_sql||')row where 
			(row.geom_type = ''Point'' AND exists(select 1 from point_master p where p.status=''A'' and p.entity_type = row.layer and p.system_id = row.system_id AND
					 ST_INTERSECTS(P.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT(''POINT('|| p_longitude||' '||p_latitude||')'',4326),'||v_RADIUS||'))))
			OR
			(row.geom_type = ''Line'' AND exists(select 1 from line_master p where p.status=''A'' and  p.entity_type = row.layer and p.system_id = row.system_id AND
					 ST_INTERSECTS(P.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT(''POINT('|| p_longitude||' '||p_latitude||')'',4326),'||v_RADIUS||'))))
			OR
			((row.geom_type = ''Polygon'' OR row.layer = ''Building'')  AND exists(select 1 from Polygon_master p where p.status=''A'' and  p.entity_type = row.layer and p.system_id = row.system_id AND
					 ST_INTERSECTS(P.SP_GEOMETRY, ST_BUFFER_METERS(ST_GEOMFROMTEXT(''POINT('|| p_longitude||' '||p_latitude||')'',4326),'||v_RADIUS||'))))
			';
		Else
			RETURN QUERY EXECUTE 'select row_to_json(row) as geo from ('||v_sql||')row 
			';	
		End If;
	end if;
end 
$BODY$;

ALTER FUNCTION public.fn_get_vector_layers_delta_by_geom(character varying, character varying, double precision, double precision, integer)
    OWNER TO postgres;
	
	
	
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

--select * from public.fn_get_vector_layers_data_by_geom('156',-1.27602,36.82734,0);
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
					inner join ' ||v_geometry_table ||' g on g.status=''A'' and e.system_id = g.system_id and  g.entity_type = '''||v_arow.layer_name||'''
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
						v_where_col='upper(att.source_ref_type)=''NETWORK_TICKET'' and att.source_ref_id='''||p_ticket_id||'''';
					end if;
					v_sql:= v_sql||' SELECT '''||v_arow.layer_name||''' as layer, e.system_id, e.geojson AS feature FROM '||v_arow.entity_geojson_table||' e 
					inner join '||v_arow.layer_table||' att on '||v_layer_col||'=e.system_id
					where '||v_where_col||' ';
				End if;
		end loop;
		Raise info  ' Final Query - ,%',v_sql; 
		RAISE INFO '%', sql;        
		RETURN QUERY EXECUTE 'select row_to_json(row) as geo from ('||v_sql||')row';	
		
		
end 
$BODY$;

ALTER FUNCTION public.fn_get_vector_layers_data_by_geom(character varying, double precision, double precision, integer)
    OWNER TO postgres;