CREATE OR REPLACE FUNCTION public.fn_splicing_sync_connection_label(
	p_layer_id integer)
    RETURNS void
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
 declare v_display_column_name character varying;
 v_layer_name character varying;
 v_layer_table character varying;
 v_geom_type character varying;
 v_display_default_column_name character varying;
begin
	update display_name_settings set status='InProgress' where display_name_settings.layer_id=p_layer_id; 
	 PERFORM pg_sleep(50);

	select layer_name,layer_table,geom_type into v_layer_name,v_layer_table,v_geom_type from layer_details where layer_id=p_layer_id;
	select display_column_name,default_display_column_name into v_display_column_name,v_display_default_column_name from display_name_settings where layer_id=p_layer_id;
	--raise info '--------%',1;	
	execute 'update connection_info set source_display_name=(case when coalesce(ent.'||v_display_column_name||','''')='''' 
	then ent.'||v_display_default_column_name||' else ent.'||v_display_column_name||' end) from '||v_layer_table||' ent 
 	where connection_info.source_system_id=ent.system_id and upper(source_entity_type)=upper('''||v_layer_name||''')';
	--raise info '--------%',2;
 	execute 'update connection_info set destination_display_name=(case when coalesce(ent.'||v_display_column_name||','''')='''' 
 	then ent.'||v_display_default_column_name||' else ent.'||v_display_column_name||' end) from '||v_layer_table||' ent 
	where connection_info.destination_system_id=ent.system_id and upper(destination_entity_type)=upper('''||v_layer_name||''')';
	--raise info '--------%',3;
	execute 'update point_master  set display_name=(case when coalesce(ent.'||v_display_column_name||','''')='''' 
	then ent.'||v_display_default_column_name||' else ent.'||v_display_column_name||' end) from '||v_layer_table||' ent 
 	where point_master.system_id=ent.system_id and upper(point_master.entity_type)=upper('''||v_layer_name||''')';
	--raise info '--------%',4;
	execute 'update line_master  set display_name=(case when coalesce(ent.'||v_display_column_name||','''')='''' 
	then ent.'||v_display_default_column_name||' else ent.'||v_display_column_name||' end) from '||v_layer_table||' ent 
 	where line_master.system_id=ent.system_id and upper(line_master.entity_type)=upper('''||v_layer_name||''')';
	--raise info '--------%',5;
	execute 'update isp_line_master set display_name=(case when coalesce(ent.'||v_display_column_name||','''')='''' 
	then ent.'||v_display_default_column_name||' else ent.'||v_display_column_name||' end) from '||v_layer_table||' ent 
 	where isp_line_master.entity_id=ent.system_id and upper(isp_line_master.entity_type)=upper('''||v_layer_name||''')';
	--raise info '--------%',6;	
 	execute 'update polygon_master  set display_name=(case when coalesce(ent.'||v_display_column_name||','''')='''' 
 	then ent.'||v_display_default_column_name||' else ent.'||v_display_column_name||' end) from '||v_layer_table||' ent 
 	where polygon_master.system_id=ent.system_id and upper(polygon_master.entity_type)=upper('''||v_layer_name||''')';
	--raise info '--------%',7;
	execute 'update associate_entity_info set entity_display_name=(case when coalesce(ent.'||v_display_column_name||','''')='''' 
	then ent.'||v_display_default_column_name||' else ent.'||v_display_column_name||' end) from '||v_layer_table||' ent 
 	where associate_entity_info.entity_system_id=ent.system_id and upper(associate_entity_info.entity_type)=upper('''||v_layer_name||''')';
	--raise info '--------%',8;
	execute 'update associate_entity_info set associated_display_name=(case when coalesce(ent.'||v_display_column_name||','''')='''' 
	then ent.'||v_display_default_column_name||' else ent.'||v_display_column_name||' end) from '||v_layer_table||' ent 
 	where associate_entity_info.associated_system_id=ent.system_id and upper(associate_entity_info.associated_entity_type)=upper('''||v_layer_name||''')';
 	--raise info '--------%',9;       
	update display_name_settings set status='Completed' where display_name_settings.layer_id=p_layer_id;

EXCEPTION  
   WHEN OTHERS THEN 
   	--raise info '--------%',11;
 update display_name_settings set status='Failed' where display_name_settings.layer_id=p_layer_id;	
end;
$BODY$;