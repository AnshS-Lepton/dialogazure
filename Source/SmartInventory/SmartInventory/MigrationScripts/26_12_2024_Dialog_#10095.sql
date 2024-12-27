--Backup of the Duplicate link Id Data
CREATE TABLE att_details_fiber_link_duplicateLinkId_FreeData_bkp AS
SELECT * 
FROM att_details_fiber_link
WHERE link_id IN (
    SELECT link_id
    FROM att_details_fiber_link
    WHERE fiber_link_status = 'Free'
    GROUP BY link_id
    HAVING COUNT(*) > 1
);
---------------------------------------------------------------
--Delete all the duplicate data
DELETE FROM att_details_fiber_link
WHERE link_id IN (
    SELECT link_id
    FROM att_details_fiber_link
    WHERE fiber_link_status = 'Free'
    GROUP BY link_id
    HAVING COUNT(*) > 1
)
AND ctid NOT IN (
    SELECT MIN(ctid)
    FROM att_details_fiber_link
    WHERE fiber_link_status = 'Free'
    GROUP BY link_id
);
--------------------------------------------------------------------
--Create Unique Constraint on the Link_Id column
ALTER TABLE att_details_fiber_link
ADD CONSTRAINT unique_link_id UNIQUE (link_id);

