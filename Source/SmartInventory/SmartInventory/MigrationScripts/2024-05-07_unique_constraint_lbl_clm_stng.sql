delete from label_column_settings where table_column_name = 'gis_design_id' and column_name = 'gis_design_id';

ALTER TABLE label_column_settings ADD CONSTRAINT unq_lyr_col UNIQUE (layer_id, table_column_name);