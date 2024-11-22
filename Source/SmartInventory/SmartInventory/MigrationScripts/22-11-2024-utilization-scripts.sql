UPDATE public.layer_details
	SET is_utilization_enabled=false
	WHERE layer_name='FiberLink';
	
	------------------------------
	
UPDATE public.module_master
	SET is_active=false
	WHERE module_abbr ='EXASSRPT';
	