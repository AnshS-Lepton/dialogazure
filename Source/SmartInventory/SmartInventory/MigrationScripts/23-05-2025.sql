
UPDATE data_uploader_template
SET template_column_name = 'site_id'
WHERE layer_id = (
    SELECT layer_id
    FROM layer_details
    WHERE layer_name = 'POD'
)
AND db_column_name = 'site_id';


UPDATE data_uploader_template
SET template_column_name = 'site_name'
WHERE layer_id = (
    SELECT layer_id
    FROM layer_details
    WHERE layer_name = 'POD'
)
AND db_column_name = 'site_name';

