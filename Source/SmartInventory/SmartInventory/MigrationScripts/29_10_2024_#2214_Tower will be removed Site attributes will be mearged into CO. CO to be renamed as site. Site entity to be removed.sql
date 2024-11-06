insert into module_master(module_name,module_description,type,is_active,module_abbr,parent_module_id,module_sequence)
	  values('Site Details','Site Details','Web',true,'SD',0,0);

insert into role_module_mapping(role_id,module_id)
	 values(2,(select id from module_master where module_name='Site Details'));

insert into user_module_mapping(user_id,module_id,modified_by,created_by)
	  values(5,(select id from module_master where module_name='Site Details'),0,1);

--------------------------------------------------------------------------------------
ALTER TABLE 
  att_details_pod 
ADD 
  COLUMN site_id VARCHAR NULL, 
ADD 
  COLUMN site_name VARCHAR null,
ADD 
  COLUMN on_air_date TIMESTAMP NULL, 
ADD 
  COLUMN removed_date TIMESTAMP NULL, 
ADD 
  COLUMN tx_type VARCHAR NULL, 
ADD 
  COLUMN tx_technology VARCHAR NULL, 
ADD 
  COLUMN tx_segment VARCHAR NULL, 
ADD 
  COLUMN tx_ring VARCHAR NULL, 
ADD 
  COLUMN province VARCHAR NULL, 
ADD 
  COLUMN district VARCHAR NULL, 
ADD 
  COLUMN depot VARCHAR NULL, 
ADD 
  COLUMN ds_division VARCHAR NULL, 
ADD 
  COLUMN local_authority VARCHAR NULL, 
ADD 
  COLUMN owner_name VARCHAR NULL, 
ADD 
  COLUMN access_24_7 VARCHAR NULL, 
ADD 
  COLUMN tower_type VARCHAR NULL, 
ADD 
  COLUMN tower_height INT4 NULL, 
ADD 
  COLUMN cabinet_type VARCHAR NULL, 
ADD 
  COLUMN solution_type VARCHAR NULL, 
ADD 
  COLUMN site_rank int4 NULL, 
ADD 
  COLUMN self_tx_traffic numeric NULL, 
ADD 
  COLUMN agg_tx_traffic  numeric NULL,
ADD 
  COLUMN csr_count int4 NULL, 
ADD 
  COLUMN dti_circuit int4 NULL,
ADD 
  COLUMN agg_01 VARCHAR NULL, 
ADD 
  COLUMN agg_02 VARCHAR NULL, 
ADD 
  COLUMN bandwidth int4 NULL, 
ADD 
  COLUMN ring_type VARCHAR NULL, 
ADD 
  COLUMN link_id VARCHAR NULL, 
ADD 
  COLUMN alias_name VARCHAR NULL;

------------------------------------------------------
update layer_details set isvisible =false,is_visible_in_ne_library=false  where layer_title ilike '%SITE%';
update layer_details set isvisible =false,is_visible_in_ne_library=false  where layer_title ilike '%TOWER%';
update layer_details set layer_title='Site' where layer_name = 'POD';