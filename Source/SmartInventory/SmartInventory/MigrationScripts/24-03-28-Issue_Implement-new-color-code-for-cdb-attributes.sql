-- public.vw_att_details_cable_vector source

CREATE OR REPLACE VIEW public.vw_att_details_cable_vector
AS SELECT cable.system_id,
    cable.region_id,
    cable.province_id,
    cable.cable_name,
    cable.cable_type,
    cable.cable_category,
    cable.network_status,
    lm.sp_geometry,
    ((cable.total_core || 'F'::text) ||
        CASE
            WHEN cable.cable_type::text = 'Overhead'::text THEN ' OH'::text
            WHEN cable.cable_type::text = 'Underground'::text THEN ' UG'::text
            WHEN cable.cable_type::text = 'Wall Clamped'::text THEN ' WC'::text
            ELSE ''::text
        END) ||
        CASE
            WHEN COALESCE(cable.ownership_type, ''::character varying)::text = 'Third Party'::text THEN 'Third Party'::text
            ELSE ''::text
        END AS cable_cores,
    cable.gis_design_id AS label_column,
    cable.network_id,
        CASE
            WHEN COALESCE(cable.gis_design_id, ''::character varying)::text = ''::text THEN cable.network_id
            ELSE cable.gis_design_id
        END AS display_name,
    COALESCE(cable.cable_category, ''::character varying) AS entity_category,
    vm.name AS vendor_name,
    ssm.prop_name AS activation_stage,
    cable.ownership_type 
   FROM line_master lm
     JOIN att_details_cable cable ON lm.system_id = cable.system_id AND lm.entity_type::text = 'Cable'::text
     LEFT JOIN vendor_master vm ON vm.id = cable.third_party_vendor_id
     LEFT JOIN system_spec_master ssm ON ssm.id = cable.activation;
     
 
------------------------------------------------------------------------------------------------------------------------------------

INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category,  created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#32CD32', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'Own', NULL,  now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'ownership_type', 'feature.properties.activation_stage ==''COMPLETED''');
 
    
 INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression,  layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#000080', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'Other', NULL,   now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]',  11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');
   
    
 INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression,  layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#000000', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'Jio', NULL,   now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');
   
    
INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category,  created_on, created_by, modified_on, modified_by, label_expression,  layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#ff99ff', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'TCL', NULL,  now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');


INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, id, created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#67ff67', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'VTL', NULL,   now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');



INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression,  layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#a52a2a', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'Depl', NULL,  now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]',  11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');


 INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category,  created_on, created_by, modified_on, modified_by, label_expression,  layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#4169e1', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'Rcom', NULL,  now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');
   
   

INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#f20207', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'Bharti', NULL, now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');
   
   
INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#ff00ff', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'TTML', NULL,  now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');


INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category,  created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#66ccff', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'Approved', NULL, now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]',  11, 'Solid', 'Solid', 'Dash', 'activation_stage', '');


INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category,  created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#008000', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'Planning', NULL, now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]',  11, 'Solid', 'Solid', 'Dash', 'activation_stage', '');


INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#ff00ff', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'TTSL', NULL,  now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'vendor_name', 'feature.properties.activation_stage ==''COMPLETED''');



INSERT INTO public.layer_style_master
(layer_id, color_code_hex, outline_color_hex, opacity, label_font_size, label_color_hex, label_bg_color_hex, icon_base_path, icon_file_name, line_width, entity_category, entity_sub_category, created_on, created_by, modified_on, modified_by, label_expression, layer_sequence, planned, asbuild, dormant, style_column_name, expressions)
VALUES(19, '#800000', NULL, 0.26, 21, '#c80bd5', '#cca519', 'Content/images/icons/map/', 'cable.png', 5, 'TTSL', NULL,  now(), 5, null, 1, '[
  {
    "type": "UD",
    "Value": "Distribution:"
  },
  {
    "type": "Col",
    "Value": "cable_name"
  },
  {
    "type": "UD",
    "Value": "cable"
  },
  {
    "type": "Col",
    "Value": null
  }
]', 11, 'Solid', 'Solid', 'Dash', 'activation_stage', 'feature.properties.activation_stage ==''REJECTED''');


-----------------------------------------------------------------------------------------------------------------------------------

alter table  layer_style_master add column expressions varchar null;

alter table  layer_style_master add column style_column_name varchar null;

-----------------------------------------------------------------------------------------------------------------------------------

CREATE OR REPLACE FUNCTION public.fn_get_vector_layer_properties()
 RETURNS SETOF json
 LANGUAGE plpgsql
AS $function$

BEGIN
	

RETURN QUERY 
SELECT Json_build_object('layer_name', ld.layer_name, 'layer_title',ld.layer_title,'layer_abbr', ld.layer_abbr, 
	'LayerStyle',
 	(
		SELECT (select json_agg(row_to_json(t)) FROM 
				(SELECT 
					entity_category,entity_sub_category,color_code_hex,outline_color_hex,opacity,line_width,label_font_size,
					label_color_hex,label_bg_color_hex,icon_base_path,icon_file_name,color_code_alpha_hex,outline_color_code_alpha_hex,label_expression
				 ,planned,asbuild,dormant,style_column_name,expressions
					from layer_style_master 
					where layer_id = ld.layer_id
				)t)as LayerStyle
	)
)
FROM layer_details ld 
inner join layer_style_master ls on ld.layer_id=ls.layer_id
group by ld.layer_name,ld.layer_title,ld.layer_abbr,ld.layer_id,layer_sequence order by ls.layer_sequence;
end 
$function$
;
