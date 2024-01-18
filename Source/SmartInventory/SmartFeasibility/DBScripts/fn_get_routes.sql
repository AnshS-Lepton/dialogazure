CREATE OR REPLACE FUNCTION public.fn_sf_get_routes(IN p_source character varying, IN p_destination character varying, IN p_start_buffer integer, IN p_end_buffer integer)
  RETURNS TABLE(seq integer, path_seq integer, edge_targetid integer, roadline_geomtext text) AS
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
BEGIN
	--select start_vid, end_vid INTO v_start_vid, v_end_vid
	--FROM pgr_dijkstra(
	--'SELECT id, source, target, cost, reverse_cost FROM new_poc_data',
	--(SELECT array_agg(source) FROM new_poc_data 
	--	WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), geom, p_start_buffer, true)),
	--(SELECT array_agg(target) FROM new_poc_data 
	--	WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), geom, p_end_buffer, true)),
	--directed:= True) as pt
	--JOIN new_poc_data_noded nd ON pt.edge = nd.old_id group by start_vid, end_vid 
	--order by (SUM(ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(geom), true) +
	--	ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(geom), true))) limit 1;

	select T.start_vid, T.end_vid, least((ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(single_geom), true) +
		ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(single_geom), true)),
		(ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(st_reverse(single_geom)), true) +
		ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(st_reverse(single_geom)), true))) as outside_length INTO v_start_vid_frwd, v_end_vid_frwd, v_length_frwd
	FROM(
	select start_vid, end_vid, ST_LineMerge(st_union(geom)) as single_geom
	FROM pgr_dijkstra(
	'SELECT id, source, target, cost, reverse_cost FROM new_poc_data',
	(SELECT array_agg(source) FROM new_poc_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), geom, p_start_buffer, true)),
	(SELECT array_agg(target) FROM new_poc_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), geom, p_end_buffer, true)),
	directed:= True) as pt
	JOIN new_poc_data_noded nd ON pt.edge = nd.old_id 
	group by start_vid, end_vid) T order by outside_length limit 1;

	-- if direction of cable is backwrds, it may leave boundary nodes, then we need to check lengths in reverse direction
	select T.start_vid, T.end_vid, least((ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(single_geom), true) +
		ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(single_geom), true)),
		(ST_Distance(ST_GeomFromText('POINT(' || p_source || ')', 4326), ST_StartPoint(st_reverse(single_geom)), true) +
		ST_Distance(ST_GeomFromText('POINT(' || p_destination || ')', 4326), ST_EndPoint(st_reverse(single_geom)), true))) as outside_length INTO v_start_vid_bkwd, v_end_vid_bkwd, v_length_bkwd
	FROM(
	select start_vid, end_vid, ST_LineMerge(st_union(geom)) as single_geom
	FROM pgr_dijkstra(
	'SELECT id, source, target, cost, reverse_cost FROM new_poc_data',
	(SELECT array_agg(target) FROM new_poc_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_source || ')', 4326), geom, p_start_buffer, true)),
	(SELECT array_agg(source) FROM new_poc_data 
		WHERE ST_DWithin(ST_GeomFromText('POINT(' || p_destination || ')', 4326), geom, p_end_buffer, true)),
	directed:= True) as pt
	JOIN new_poc_data_noded nd ON pt.edge = nd.old_id 
	group by start_vid, end_vid) T order by outside_length limit 1;

	RAISE NOTICE 'forward = % and backward = %', v_length_frwd,v_length_bkwd;
	
	if v_length_frwd <= v_length_bkwd then
		v_start_vid:= v_start_vid_frwd;
		v_end_vid:= v_end_vid_frwd;
	else
		v_start_vid:= v_end_vid_bkwd;
		v_end_vid:= v_start_vid_bkwd;
	end if;

	RAISE NOTICE 'start_vid = % and end_vid = %', v_start_vid, v_end_vid;
		

	RETURN QUERY	
	select pt.seq, 
	-- in case of considering reverse path, we will also have to reverse the path_seq.
	case when v_length_frwd <= v_length_bkwd then pt.path_seq::integer
		else ((count(*) OVER()) - pt.path_seq + 1)::integer
	end as path_seq, pt.edge::integer as edge_TargetID, st_astext(nd.geom) as roadLine_GeomText 
	FROM pgr_dijkstra(
	'SELECT id, source, target, cost, reverse_cost FROM new_poc_data', v_start_vid, v_end_vid,
	directed:= True) as pt
	JOIN new_poc_data_noded nd ON pt.edge = nd.old_id order by path_seq;

END; 
$BODY$
  LANGUAGE plpgsql VOLATILE
  COST 100
  ROWS 1000;
ALTER FUNCTION public.fn_sf_get_routes(character varying, character varying, integer, integer)
  OWNER TO postgres;


 -- drop function fn_sf_get_routes(IN p_source character varying, IN p_destination character varying, IN p_start_buffer integer, IN p_end_buffer integer)
-- select * from fn_sf_get_routes('77.137004 28.776205', '77.077008 28.729250', 1000, 1000)
-- select * from fn_sf_get_routes('77.077008 28.729250', '77.137004 28.776205', 1000, 1000)