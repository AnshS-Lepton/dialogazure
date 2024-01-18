-- Function: public.fn_sf_get_routes(character varying, character varying, integer, integer)

-- DROP FUNCTION public.fn_sf_get_routes(character varying, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public.fn_sf_get_routes(IN p_source character varying, IN p_destination character varying, IN p_start_buffer integer, IN p_end_buffer integer)
  RETURNS TABLE(seq integer, path_seq integer, edge_targetid integer, roadline_geomtext text, start_point character varying, end_point character varying) AS
$BODY$
DECLARE 
   v_start_vid integer;
   v_end_vid integer;
   v_start_vid_frwd integer;
   v_end_vid_frwd integer;
   v_start_vid_bkwd integer;
   v_end_vid_bkwd integer;
   v_length_frwd double precision;
   v_length_bkwd double precision;
   v_start_point character varying;
   v_end_point character varying;
   v_temp_point character varying;
   v_geom character varying;
   rec RECORD;
BEGIN

	CREATE TEMP TABLE temp_routes(
		start_vid integer,
		end_vid integer,
		single_geom geometry
	)
	ON COMMIT DROP;

	INSERT INTO temp_routes(start_vid, end_vid)
	(select start_vid, end_vid
	FROM pgr_dijkstra(
	'SELECT id, source, target, cost, reverse_cost FROM routing_data',
	(SELECT array_agg(source) FROM routing_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), geom, p_start_buffer, true)),
	(SELECT array_agg(target) FROM routing_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), geom, p_end_buffer, true)),
	directed:= True) as pt
	JOIN routing_data_noded nd ON pt.edge = nd.old_id 
	group by start_vid, end_vid);

	RAISE INFO 'Data inserted in temp_routes'; 

	UPDATE temp_routes tr
	SET single_geom = 
		(
			SELECT ST_LineMerge(st_collect(nd.geom))
			FROM pgr_dijkstra(
			'SELECT id, source, target, cost, reverse_cost FROM routing_data', tr.start_vid, tr.end_vid,
			directed:= True) as pt
			JOIN routing_data_noded nd ON pt.edge = nd.old_id 
			group by start_vid, end_vid
		);

	select T.start_vid, T.end_vid, st_astext(single_geom)
		INTO v_start_vid_frwd, v_end_vid_frwd, v_geom
	FROM temp_routes T 
	WHERE 
	-- forward case
	((ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_StartPoint(single_geom), true) >= 
	ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(single_geom), true))
	and
	(ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(single_geom), p_start_buffer, true)
	and ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(single_geom), p_end_buffer, true)))
	OR
	-- reverse case
	((ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_StartPoint(single_geom), true) < 
	ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(single_geom), true))
	and
	(ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_EndPoint(single_geom), p_start_buffer, true)
	and ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_StartPoint(single_geom), p_end_buffer, true)))
	order by ST_Length(single_geom) limit 1;
	
	v_start_vid:= v_start_vid_frwd;
	v_end_vid:= v_end_vid_frwd;

	RAISE NOTICE 'v_geom = %', ST_StartPoint(ST_GeomFromText(v_geom, 4326));

	v_start_point := st_astext(ST_StartPoint(ST_GeomFromText(v_geom, 4326)));
	v_end_point := st_astext(ST_EndPoint(ST_GeomFromText(v_geom, 4326)));

	-- check for correct start & end points 
	if ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_GeomFromText(v_start_point, 4326), true) < ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_GeomFromText(v_start_point, 4326), true) then
		v_temp_point := v_start_point;
		v_start_point := v_end_point;
		v_end_point := v_temp_point;
	end if;

	RAISE NOTICE 'start_vid = % and end_vid = %', v_start_vid, v_end_vid;
		
	RETURN QUERY	
	select pt.seq, 
	pt.path_seq::integer as path_seq,
	main.system_id::integer as edge_TargetID, 
	st_astext(nd.geom) as roadLine_GeomText, v_start_point as start_point, v_end_point as end_point
	FROM pgr_dijkstra(
	'SELECT id, source, target, cost, reverse_cost FROM routing_data', v_start_vid, v_end_vid,
	directed:= True) as pt
	JOIN routing_data_noded nd ON pt.edge = nd.old_id 
	JOIN routing_data main ON pt.edge = main.id
	order by path_seq;
END; 
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_routes(character varying, character varying, integer, integer)
  OWNER TO postgres;
