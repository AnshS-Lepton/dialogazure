/*------------------------------------------
CreatedBy: Chandra Shekhar Sahni
CreatedOn: 12 Dec 2024
Purpose: To Update the color codes of cables in Dialog Application
ModifiedOn: <>
ModifiedBy: <>
------------------------------------------*/

CREATE OR REPLACE FUNCTION fn_allocate_colors(p_cable_id INT) RETURNS void AS $$
DECLARE
    color_names TEXT[];
    color_codes TEXT[];
    color_count INT;
    core RECORD;
    color_idx INT := 1;
	previous_tube_id INT := NULL;
BEGIN
    -- Get all color codes and names into arrays
    SELECT array_agg(color_code ORDER BY color_id DESC), array_agg(color_character ORDER BY color_id DESC) INTO color_codes, color_names
    FROM cable_color_master
    WHERE UPPER(type) = UPPER('Core');
    
    -- Get the total number of color codes
    SELECT COUNT(*) INTO color_count 
    FROM cable_color_master 
    WHERE UPPER(type) = UPPER('Core');

    -- Loop through all cores for the given cable_id
    FOR core IN (SELECT * FROM att_details_cable_info WHERE cable_id = p_cable_id) LOOP

		-- Reset the color index if tube_id changes
        IF core.tube_number IS DISTINCT FROM previous_tube_id THEN
            color_idx := 1;
            previous_tube_id := core.tube_number;
        END IF;

        -- Set the core color code and color name
        UPDATE att_details_cable_info
        SET core_color_code = color_codes[color_idx],
            core_color = color_names[color_idx]
        WHERE id = core.id;

        -- Increment the color index and reset if necessary
        color_idx := color_idx + 1;
        IF color_idx > color_count THEN
            color_idx := 1;
        END IF;
    END LOOP;
END;
$$ LANGUAGE plpgsql;