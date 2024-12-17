/*------------------------------------------
CreatedBy: Chandra Shekhar Sahni
CreatedOn: 12 Dec 2024
Description: This function updates every cable core color available against cable_id 
Purpose: To Update the core color codes of cables in Dialog Application
ModifiedOn: <>
ModifiedBy: <>
------------------------------------------*/

CREATE OR REPLACE FUNCTION fn_allocate_core_colors(p_cable_id INT) RETURNS void AS $$
DECLARE
    color_names TEXT[];
    color_codes TEXT[];
    color_count INT;
    core RECORD;
    color_idx INT := 1;
	previous_tube_id INT := NULL;
BEGIN
    -- Get all color codes and names into arrays
    SELECT array_agg(color_code ORDER BY color_id ASC), array_agg(color_character ORDER BY color_id ASC) INTO color_codes, color_names
    FROM cable_color_master
    WHERE UPPER(type) = UPPER('Core');
    
    -- Get the total number of color codes
    SELECT COUNT(*) INTO color_count 
    FROM cable_color_master 
    WHERE UPPER(type) = UPPER('Core');

    -- Loop through all cores for the given cable_id
    FOR core IN (SELECT * FROM att_details_cable_info WHERE cable_id = p_cable_id ORDER BY tube_number,core_number ASC) LOOP

		-- Reset the color index if tube_id changes
        IF core.tube_number IS DISTINCT FROM previous_tube_id THEN
            color_idx := 1;
            previous_tube_id := core.tube_number;
        END IF;

        -- Set the core color code and color name
        UPDATE att_details_cable_info
        SET core_color_code = color_codes[color_idx],
            core_color = color_names[color_idx]
        WHERE core_number=core.core_number and cable_id=core.cable_id; --and id = core.id

        -- Increment the color index and reset if necessary
        color_idx := color_idx + 1;
        IF color_idx > color_count THEN
            color_idx := 1;
        END IF;
    END LOOP;
END;
$$ LANGUAGE plpgsql;


---------------------------------------------------------------------------------------------------------
/*------------------------------------------
CreatedBy: Chandra Shekhar Sahni
CreatedOn: 13 Dec 2024
Description: This function updates every cable tube color available against cable_id 
Purpose: To Update the tube color codes of cables in Dialog Application
ModifiedOn: <>
ModifiedBy: <>
------------------------------------------*/

CREATE OR REPLACE FUNCTION fn_allocate_tube_colors(p_cable_id INT) RETURNS void AS $$
DECLARE
    color_names TEXT[];
    color_codes TEXT[];
    color_count INT;
    tube RECORD;
    color_idx INT := 1;
	previous_tube_id INT := NULL;
BEGIN
    -- Get all color codes and names into arrays
    SELECT array_agg(color_code ORDER BY color_id ASC), array_agg(color_character ORDER BY color_id ASC) INTO color_codes, color_names
    FROM cable_color_master
    WHERE UPPER(type) = UPPER('Tube');
    
    -- Get the total number of color codes
    SELECT COUNT(*) INTO color_count 
    FROM cable_color_master 
    WHERE UPPER(type) = UPPER('Tube');

    -- Loop through all tubes for the given cable_id
    FOR tube IN (SELECT distinct cable_id,tube_number FROM att_details_cable_info WHERE cable_id = p_cable_id ORDER BY tube_number ASC) LOOP

        -- Set the tube color code and color name
        UPDATE att_details_cable_info
        SET tube_color_code = color_codes[color_idx],
            tube_color = color_names[color_idx]
        WHERE tube_number = tube.tube_number and cable_id=tube.cable_id;

        -- Increment the color index and reset if necessary
        color_idx := color_idx + 1;
        IF color_idx > color_count THEN
            color_idx := 1;
        END IF;
    END LOOP;
END;
$$ LANGUAGE plpgsql;

---------------------------------------------------------------------------------------------
/*------------------------------------------
CreatedBy: Chandra Shekhar Sahni
CreatedOn: 13 Dec 2024
Description: This function updates every cable core or tube color code and name available against cable_id 
Purpose: To Update the core or tube color codes and color name of cables in Dialog Application
ModifiedOn: <>
ModifiedBy: <>
------------------------------------------*/
CREATE OR REPLACE FUNCTION fn_set_core_and_tube_color_for_all_cables(p_type TEXT) RETURNS void AS $$
DECLARE
    cable RECORD;
BEGIN
    -- Capitalize the type parameter
    p_type := UPPER(p_type);

    -- Loop through all system_id in att_details_cable
    FOR cable IN SELECT system_id FROM att_details_cable LOOP
        -- Check the type and call the appropriate function
        IF p_type = 'CORE' THEN
            PERFORM fn_allocate_core_colors(cable.system_id);
        ELSIF p_type = 'TUBE' THEN
            PERFORM fn_allocate_tube_colors(cable.system_id);
        ELSE
            RAISE EXCEPTION 'Invalid type: %', p_type;
        END IF;
    END LOOP;
END;
$$ LANGUAGE plpgsql;

-------------------------------------------------------------------------------------------------------
select public.fn_set_core_and_tube_color_for_all_cables('core');
select public.fn_set_core_and_tube_color_for_all_cables('tube');

---------------------------------------Picked from SIT Env--------------------------------------------------------------------
Truncate table cable_color_master
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BLUE', NULL, '#0000ff', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.176');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('ORANGE', NULL, '#ffa500', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.176');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BLUE-1', NULL, '#0000ff', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:08.146');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('ORANGE-2', NULL, '#ffa500', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:08.250');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('GREEN-3', NULL, '#008000', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:08.344');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BROWN-4', NULL, '#a52a2a', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:08.481');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('SLATE-5', NULL, '#808080', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:08.618');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('WHITE-6', NULL, '#ffffff', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:08.784');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('RED-7', NULL, '#ff0000', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:08.911');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BLACK-8', NULL, '#000000', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:09.059');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('YELLOW-9', NULL, '#ffff00', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:09.272');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('VIOLET-10', NULL, '#ee82ee', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:09.453');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('ROSE-11', NULL, '#ffc0cb', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:09.594');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('AQUA-12', NULL, '#00ffff', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:09.719');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BLUE-13', NULL, '#0000ff', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:09.976');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('ORANGE-14', NULL, '#ffa500', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.119');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('GREEN-15', NULL, '#008000', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.294');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BROWN-16', NULL, '#a52a2a', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.432');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('SLATE-17', NULL, '#808080', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.529');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('WHITE-18', NULL, '#ffffff', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.639');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('RED-19', NULL, '#ff0000', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.764');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BLACK-20', NULL, '#000000', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.820');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('YELLOW-21', NULL, '#ffff00', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.906');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('VIOLET-22', NULL, '#ee82ee', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:10.986');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('ROSE-23', NULL, '#ffc0cb', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:11.085');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('AQUA-24', NULL, '#00ffff', 'Tube', 1, '2024-11-08 14:09:32.922', 5, '2024-11-22 14:44:11.197');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('GREEN', NULL, '#008000', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.193');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BROWN', NULL, '#a52a2a', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.193');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('SLATE', NULL, '#808080', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.193');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('WHITE', NULL, '#ffffff', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.208');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('RED', NULL, '#ff0000', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.224');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('BLACK', NULL, '#000000', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.224');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('YELLOW', NULL, '#ffff00', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.224');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('VIOLET', NULL, '#ee82ee', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.239');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('ROSE', NULL, '#ffc0cb', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.239');
INSERT INTO public.cable_color_master (color_character, color_name, color_code, "type", created_by, created_on, modified_by, modified_on) VALUES('AQUA', NULL, '#00ffff', 'Core', 1, '2024-11-08 14:09:32.922', 1, '2024-11-21 10:01:38.239');