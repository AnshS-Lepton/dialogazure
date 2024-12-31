--Remove the Garbage Data from link_prefix column 
update att_details_fiber_link set link_prefix='';

--Update the correct link_prefix  in the link_prefix column
UPDATE att_details_fiber_link
SET link_prefix = dd.link_prefix
FROM (
    SELECT dd.dropdown_value AS link_prefix
    FROM dropdown_master dd
    WHERE UPPER(dd.dropdown_type) = UPPER('FiberLinkPrefix')
      AND LOWER(dd.dropdown_type) NOT LIKE 'remark_%'
      AND dd.is_active = TRUE
) dd
WHERE att_details_fiber_link.link_id LIKE dd.link_prefix || '%';