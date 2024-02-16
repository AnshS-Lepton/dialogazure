create trigger trg_ticket_type_master after
insert
or
update
or
delete
on
public.ticket_type_master for each row execute function fn_trg_ticket_type_master();
	
	
	
CREATE OR REPLACE FUNCTION public.fn_trg_ticket_type_master()
 RETURNS trigger
 LANGUAGE plpgsql
AS $function$

BEGIN

IF (TG_OP = 'INSERT' ) THEN  

INSERT INTO public.ticket_type_role_mapping (ticket_type_id, role_id, is_create, is_edit, is_view, is_approve, created_by, created_on, ticket_type) 
select distinct t.id,r.role_id,false,false,false,false,1,now(),t.ticket_type from ticket_type_master t
inner join role_module_mapping r on r.module_id= (select id from module_master where module_name='Network Ticket') 
and t.module='Network_Ticket' where t.id=new.id;

END IF;	

IF (TG_OP = 'UPDATE' ) THEN  

update ticket_type_role_mapping set ticket_type=(select ticket_type from ticket_type_master where id=new.id) where ticket_type_id=new.id;

END IF;	

IF (TG_OP = 'DELETE' ) THEN
  Delete from ticket_type_role_mapping where ticket_type_id=old.id;
END IF;
	

RETURN NEW;
END;
$function$
;