create trigger trg_ticket_type_master after
insert
    on
    public.ticket_type_master for each row execute function fn_trg_ticket_type_master();
	
	
CREATE OR REPLACE FUNCTION public.fn_trg_ticket_type_master()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$

DECLARE
    ROW_RECORD RECORD;

BEGIN

IF (TG_OP = 'INSERT' ) THEN  

FOR ROW_RECORD IN SELECT distinct role_id from role_master 

LOOP

INSERT INTO public.ticket_type_role_mapping (ticket_type_id, role_id, is_create, is_edit, is_view, is_approve, created_by, created_on, ticket_type) 
select id,ROW_RECORD.role_id,false,false,false,false,1,now(),ticket_type from ticket_type_master where id=new.id;

END LOOP;

END IF;		

RETURN NEW;
END;
$function$
;