DROP TABLE IF EXISTS public.att_entity_execution_method;

CREATE TABLE IF NOT EXISTS public.att_entity_execution_method
(
    id integer NOT NULL DEFAULT nextval('att_entity_execution_method_id_seq'::regclass),
    system_id integer NOT NULL,
    entity_type character varying(100) COLLATE pg_catalog."default",
    execution_method character varying COLLATE pg_catalog."default",
    execution_length integer,
    created_by integer DEFAULT 0,
    created_on timestamp without time zone DEFAULT now(),
    modified_by integer,
    modified_on timestamp without time zone,
    CONSTRAINT att_entity_execution_method_pkey PRIMARY KEY (id)
)