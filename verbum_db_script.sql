--
-- PostgreSQL database dump
--

-- Dumped from database version 16.4
-- Dumped by pg_dump version 16.4

-- Started on 2024-12-02 22:28:59

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 5 (class 2615 OID 17545)
-- Name: public; Type: SCHEMA; Schema: -; Owner: avnadmin
--

CREATE SCHEMA public;


ALTER SCHEMA public OWNER TO avnadmin;

--
-- TOC entry 250 (class 1255 OID 17832)
-- Name: set_order_name(); Type: FUNCTION; Schema: public; Owner: avnadmin
--

CREATE FUNCTION public.set_order_name() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Generate the order name if it's not provided
    IF NEW.order_name IS NULL THEN
        NEW.order_name := 'ORD-' || LPAD(nextval('order_name_seq')::text, 5, '0');
    END IF;
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.set_order_name() OWNER TO avnadmin;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 236 (class 1259 OID 17834)
-- Name: assignee_job; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.assignee_job (
    assignee_id uuid NOT NULL,
    job_id uuid NOT NULL
);


ALTER TABLE public.assignee_job OWNER TO avnadmin;

--
-- TOC entry 216 (class 1259 OID 17546)
-- Name: category; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.category (
    category_name character varying,
    category_id integer NOT NULL
);


ALTER TABLE public.category OWNER TO avnadmin;

--
-- TOC entry 217 (class 1259 OID 17551)
-- Name: category_category_id_seq; Type: SEQUENCE; Schema: public; Owner: avnadmin
--

ALTER TABLE public.category ALTER COLUMN category_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.category_category_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 238 (class 1259 OID 17878)
-- Name: client_transaction; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.client_transaction (
    client_id uuid NOT NULL,
    transaction_id character varying NOT NULL,
    orderid uuid NOT NULL,
    is_deposit boolean DEFAULT false NOT NULL
);


ALTER TABLE public.client_transaction OWNER TO avnadmin;

--
-- TOC entry 234 (class 1259 OID 17775)
-- Name: discount; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.discount (
    discount_id uuid NOT NULL,
    discount_percent numeric,
    discount_name character varying
);


ALTER TABLE public.discount OWNER TO avnadmin;

--
-- TOC entry 218 (class 1259 OID 17552)
-- Name: issue; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.issue (
    issue_id uuid NOT NULL,
    issue_name character varying,
    created_at timestamp without time zone,
    updated_at timestamp without time zone,
    status character varying,
    client_id uuid,
    issue_description character varying,
    assignee_id uuid,
    cancel_response character varying,
    reject_response character varying,
    job_id uuid NOT NULL,
    src_document_url character varying NOT NULL
);


ALTER TABLE public.issue OWNER TO avnadmin;

--
-- TOC entry 4550 (class 0 OID 0)
-- Dependencies: 218
-- Name: TABLE issue; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON TABLE public.issue IS 'tạo issue, job tạo ra bởi issue : OPEN

SM accept issue from client: IN_PROGRESS
SM reject issue from client: CANCEL

Linguist đánh: SUBMITTED

SM review linguist ok: RESOLVED 
SM reject resolve from linguist: IN_PROGRESS

client cancel lúc nào cx đc: CANCEL';


--
-- TOC entry 4551 (class 0 OID 0)
-- Dependencies: 218
-- Name: COLUMN issue.status; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON COLUMN public.issue.status IS 'CANCEL, OPEN, RESOLVED, ACCEPTED, IN-PROGRESS';


--
-- TOC entry 219 (class 1259 OID 17557)
-- Name: issue_attachments; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.issue_attachments (
    issue_id uuid NOT NULL,
    attachment_url character varying NOT NULL,
    is_deleted boolean DEFAULT false NOT NULL,
    tag character varying
);


ALTER TABLE public.issue_attachments OWNER TO avnadmin;

--
-- TOC entry 4552 (class 0 OID 0)
-- Dependencies: 219
-- Name: TABLE issue_attachments; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON TABLE public.issue_attachments IS 'tag lưu file solution và reference file: SOLUTION, ATTACHMENT';


--
-- TOC entry 230 (class 1259 OID 17609)
-- Name: job; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.job (
    id uuid NOT NULL,
    name text NOT NULL,
    status text NOT NULL,
    due_date timestamp(0) without time zone,
    created_at timestamp(0) without time zone,
    updated_at timestamp(0) without time zone,
    word_count integer NOT NULL,
    document_url text NOT NULL,
    target_language_id character varying NOT NULL,
    work_id uuid,
    deliverable_url character varying,
    reject_reason character varying
);


ALTER TABLE public.job OWNER TO avnadmin;

--
-- TOC entry 4553 (class 0 OID 0)
-- Dependencies: 230
-- Name: TABLE job; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON TABLE public.job IS 'chưa assign:NEW

asign linguists: IN_PROGRESS
 
linguist làm xong: SUBMITTED

SM review ok: APPROVED
SM reject: IN_PROGRESS

client tạo issue thì tạo thêm 1 job mới ở sevice cuối cùng(TL-> ED -> EV)';


--
-- TOC entry 4554 (class 0 OID 0)
-- Dependencies: 230
-- Name: COLUMN job.status; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON COLUMN public.job.status IS 'NEW, IN_PROGRESS, SUBMITTED, ACCEPTED';


--
-- TOC entry 220 (class 1259 OID 17562)
-- Name: language; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.language (
    language_name text NOT NULL,
    language_id character varying NOT NULL,
    support boolean DEFAULT false NOT NULL
);


ALTER TABLE public.language OWNER TO avnadmin;

--
-- TOC entry 221 (class 1259 OID 17567)
-- Name: order; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public."order" (
    order_id uuid NOT NULL,
    client_id uuid NOT NULL,
    due_date timestamp without time zone,
    source_language_id character varying NOT NULL,
    order_status character varying,
    order_price numeric,
    discount_id uuid,
    has_translate_service boolean DEFAULT false NOT NULL,
    has_edit_service boolean DEFAULT false NOT NULL,
    has_evaluate_service boolean DEFAULT false NOT NULL,
    created_date timestamp without time zone,
    order_name character varying,
    reject_reason character varying,
    order_note character varying
);


ALTER TABLE public."order" OWNER TO avnadmin;

--
-- TOC entry 4555 (class 0 OID 0)
-- Dependencies: 221
-- Name: TABLE "order"; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON TABLE public."order" IS 'tạo: NEW

staff: ACCEPTED, REJECTED, 

client CANCELED

deposit xong:IN_PROGRESS, 

xong tất cả các job: COMPLETED.

client tạo isue: IN_PROGRESS

resolve xong isue: COMPLETED

pay xong: DELIVERED';


--
-- TOC entry 4556 (class 0 OID 0)
-- Dependencies: 221
-- Name: COLUMN "order".order_note; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON COLUMN public."order".order_note IS '255 char';


--
-- TOC entry 235 (class 1259 OID 17831)
-- Name: order_name_seq; Type: SEQUENCE; Schema: public; Owner: avnadmin
--

CREATE SEQUENCE public.order_name_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.order_name_seq OWNER TO avnadmin;

--
-- TOC entry 222 (class 1259 OID 17573)
-- Name: order_references; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.order_references (
    order_id uuid NOT NULL,
    reference_file_url character varying NOT NULL,
    tag character varying,
    is_deleted boolean DEFAULT false NOT NULL
);


ALTER TABLE public.order_references OWNER TO avnadmin;

--
-- TOC entry 4557 (class 0 OID 0)
-- Dependencies: 222
-- Name: TABLE order_references; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON TABLE public.order_references IS 'TRANSLATION, REFERENCES';


--
-- TOC entry 223 (class 1259 OID 17578)
-- Name: order_target_language; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.order_target_language (
    order_id uuid NOT NULL,
    target_language_id character varying NOT NULL
);


ALTER TABLE public.order_target_language OWNER TO avnadmin;

--
-- TOC entry 224 (class 1259 OID 17583)
-- Name: rating; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.rating (
    rating_id uuid NOT NULL,
    order_id uuid,
    in_time numeric,
    expectation numeric,
    issue_resolved numeric,
    more_thought character varying
);


ALTER TABLE public.rating OWNER TO avnadmin;

--
-- TOC entry 237 (class 1259 OID 17866)
-- Name: receipt; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.receipt (
    receipt_id uuid NOT NULL,
    pay_date timestamp without time zone,
    order_id uuid,
    deposite_or_payment boolean NOT NULL,
    amount numeric,
    done boolean DEFAULT false NOT NULL,
    payment_id character varying NOT NULL
);


ALTER TABLE public.receipt OWNER TO avnadmin;

--
-- TOC entry 4558 (class 0 OID 0)
-- Dependencies: 237
-- Name: TABLE receipt; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON TABLE public.receipt IS 'true is deposit, false is payment';


--
-- TOC entry 225 (class 1259 OID 17588)
-- Name: refresh_token; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.refresh_token (
    token_id integer NOT NULL,
    issued_at timestamp without time zone NOT NULL,
    expire_at timestamp without time zone NOT NULL,
    token_content character varying NOT NULL
);


ALTER TABLE public.refresh_token OWNER TO avnadmin;

--
-- TOC entry 226 (class 1259 OID 17593)
-- Name: refresh_token_token_id_seq; Type: SEQUENCE; Schema: public; Owner: avnadmin
--

ALTER TABLE public.refresh_token ALTER COLUMN token_id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.refresh_token_token_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 227 (class 1259 OID 17594)
-- Name: revelancy; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.revelancy (
    revelancy_id uuid NOT NULL,
    user_id uuid,
    source_language_id character varying,
    target_language_id character varying,
    category_id integer,
    service_code character varying
);


ALTER TABLE public.revelancy OWNER TO avnadmin;

--
-- TOC entry 228 (class 1259 OID 17599)
-- Name: role; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.role (
    role_id text NOT NULL,
    role_name text NOT NULL
);


ALTER TABLE public.role OWNER TO avnadmin;

--
-- TOC entry 229 (class 1259 OID 17604)
-- Name: services; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.services (
    service_name character varying,
    service_code character varying NOT NULL,
    service_price numeric,
    service_order integer NOT NULL
);


ALTER TABLE public.services OWNER TO avnadmin;

--
-- TOC entry 231 (class 1259 OID 17614)
-- Name: user; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public."user" (
    id uuid NOT NULL,
    name text NOT NULL,
    email text NOT NULL,
    email_verified timestamp(3) without time zone,
    image_id integer,
    password text,
    created_at timestamp(3) without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    updated_at timestamp(3) without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    status text DEFAULT 'ACTIVE'::text NOT NULL,
    token_id integer,
    role_code character varying NOT NULL
);


ALTER TABLE public."user" OWNER TO avnadmin;

--
-- TOC entry 4559 (class 0 OID 0)
-- Dependencies: 231
-- Name: TABLE "user"; Type: COMMENT; Schema: public; Owner: avnadmin
--

COMMENT ON TABLE public."user" IS 'confirm mail: ACTIVE
tạo tài khoản: INACTIVE';


--
-- TOC entry 232 (class 1259 OID 17622)
-- Name: work; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.work (
    work_id uuid NOT NULL,
    order_id uuid,
    created_date timestamp without time zone,
    due_date timestamp without time zone,
    service_code character varying,
    work_name character varying
);


ALTER TABLE public.work OWNER TO avnadmin;

--
-- TOC entry 233 (class 1259 OID 17627)
-- Name: work_category; Type: TABLE; Schema: public; Owner: avnadmin
--

CREATE TABLE public.work_category (
    work_id uuid NOT NULL,
    category_id integer NOT NULL
);


ALTER TABLE public.work_category OWNER TO avnadmin;

--
-- TOC entry 4541 (class 0 OID 17834)
-- Dependencies: 236
-- Data for Name: assignee_job; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.assignee_job (assignee_id, job_id) FROM stdin;
5cfe0fea-44fc-45eb-b999-c43aa9590820	0c48b860-a1cd-4069-9d5d-9191dc1e779b
ee40039b-5c1d-4ab0-8731-e9fd458b196a	0c48b860-a1cd-4069-9d5d-9191dc1e779b
926cfb71-d9f4-4d21-a87a-4191bd3b7c59	c07d8cb3-1368-49cd-a7ac-38b58f41931c
926cfb71-d9f4-4d21-a87a-4191bd3b7c59	bf187597-2705-4ebe-bc65-44e00aeeed82
926cfb71-d9f4-4d21-a87a-4191bd3b7c59	82bc70ce-50bb-4a5a-8a07-61a3c5568564
926cfb71-d9f4-4d21-a87a-4191bd3b7c59	16e826af-816e-4b64-876b-4ac3dd4ccd72
ee40039b-5c1d-4ab0-8731-e9fd458b196a	16e826af-816e-4b64-876b-4ac3dd4ccd72
\.


--
-- TOC entry 4521 (class 0 OID 17546)
-- Dependencies: 216
-- Data for Name: category; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.category (category_name, category_id) FROM stdin;
General	6
Sport	1
Cooking	5
Gaming	8
\.


--
-- TOC entry 4543 (class 0 OID 17878)
-- Dependencies: 238
-- Data for Name: client_transaction; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.client_transaction (client_id, transaction_id, orderid, is_deposit) FROM stdin;
\.


--
-- TOC entry 4539 (class 0 OID 17775)
-- Dependencies: 234
-- Data for Name: discount; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.discount (discount_id, discount_percent, discount_name) FROM stdin;
fd8d7e9d-064b-440a-bf65-2bf8a91e1daa	30	TRANS30
abee2843-376b-4e2b-9275-6b002b261c74	70	TRANS13
30812060-0c3c-47e4-899e-601d52b10a55	50	TRANS50
9ef71cf1-0984-4e7f-a978-df333d876b90	23	DISDIS
ad81abf3-7279-43ce-b0e8-7892a6d2ef31	32	DISCOUNT 325
\.


--
-- TOC entry 4523 (class 0 OID 17552)
-- Dependencies: 218
-- Data for Name: issue; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.issue (issue_id, issue_name, created_at, updated_at, status, client_id, issue_description, assignee_id, cancel_response, reject_response, job_id, src_document_url) FROM stdin;
75d0ee79-c6ce-4fc6-93ef-6188c8d3cd84	string	2024-11-24 16:06:16.269993	2024-11-26 06:18:42.911591	SUBMITTED	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	string	926cfb71-d9f4-4d21-a87a-4191bd3b7c59	\N	\N	941ff631-737d-4e64-8782-6b0327aafd7d	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftt-08-3.doc?alt=media&token=8e0a50b5-3721-4bf0-b11b-4689b7b34ff5
79d06a12-efc7-490d-9f54-61e8edccce06	bruh	2024-12-02 04:25:29.173719	2024-12-02 06:11:21.678612	OPEN	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	huehuehuehuehue	\N	\N	\N	82bc70ce-50bb-4a5a-8a07-61a3c5568564	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FHCM202_MKT1706%20(1).xlsx?alt=media&token=58b78f48-8a1d-47e0-a728-1451af3355a3
d542d75d-9be6-415b-8d90-241a99f8b161	kekw	2024-11-25 16:29:39.086246	2024-11-25 17:01:15.819519	RESOLVED	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	KEKW MONKA S ASDASDA	ee40039b-5c1d-4ab0-8731-e9fd458b196a	\N	\N	0c48b860-a1cd-4069-9d5d-9191dc1e779b	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FEmperor_Gia_Long.jpg?alt=media&token=24a646e8-7f03-4588-9eb1-b51464463a11
\.


--
-- TOC entry 4524 (class 0 OID 17557)
-- Dependencies: 219
-- Data for Name: issue_attachments; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.issue_attachments (issue_id, attachment_url, is_deleted, tag) FROM stdin;
d542d75d-9be6-415b-8d90-241a99f8b161	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FSCP%20Reading%20Consent%20Form%2024-25.docx?alt=media&token=a0806aa4-2e13-4c3b-bccd-8c3405e43995	f	SOLUTION
75d0ee79-c6ce-4fc6-93ef-6188c8d3cd84	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FSEP490%20StudentGuide_Fall%202023.docx?alt=media&token=b9b19912-6325-4b7e-a97c-5deca5396eff	f	ATTACHMENT
\.


--
-- TOC entry 4535 (class 0 OID 17609)
-- Dependencies: 230
-- Data for Name: job; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.job (id, name, status, due_date, created_at, updated_at, word_count, document_url, target_language_id, work_id, deliverable_url, reject_reason) FROM stdin;
13828476-60af-46ad-8529-27906cfbf5e1	GL_Evaluate_uploads%2Fcreate-orders.vue?alt=media&token=90de1ae2-6399-4843-8e3a-474fe35712de	NEW	\N	2024-11-25 16:52:14	2024-11-25 16:52:14	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=90de1ae2-6399-4843-8e3a-474fe35712de	GL	47a7d7f9-f710-4103-99ad-0d6a1dff038f	\N	\N
6584e8f6-17a1-497b-98df-e9d505772d94	GL_Evaluate_uploads%2Fpackage (1).json?alt=media&token=b78c9e0a-f5ba-4064-a0c1-d245846c18f9	NEW	\N	2024-11-25 16:52:14	2024-11-25 16:52:14	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=b78c9e0a-f5ba-4064-a0c1-d245846c18f9	GL	47a7d7f9-f710-4103-99ad-0d6a1dff038f	\N	\N
a247ed21-a6ab-4e42-a857-5952206ea79a	GL_Evaluate_uploads%2Ftailwind (1).css?alt=media&token=02bbbff7-ef1c-4797-a1ae-434f315c0b95	NEW	\N	2024-11-25 16:52:14	2024-11-25 16:52:14	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftailwind%20(1).css?alt=media&token=02bbbff7-ef1c-4797-a1ae-434f315c0b95	GL	47a7d7f9-f710-4103-99ad-0d6a1dff038f	\N	\N
0c48b860-a1cd-4069-9d5d-9191dc1e779b	HY_Translate_uploads%2FCRT 11686 Final - Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	APPROVED	2024-11-25 23:14:42	2024-11-25 15:10:34	2024-11-25 15:10:34	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	HY	787c67d3-c5d4-4463-a038-62fc408b19c1	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FSCP%20Reading%20Consent%20Form%2024-25.docx?alt=media&token=a0806aa4-2e13-4c3b-bccd-8c3405e43995	\N
c07d8cb3-1368-49cd-a7ac-38b58f41931c	ID_Translate_uploads%2FCRT 11686 Final - Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	APPROVED	2024-11-25 23:24:05	2024-11-25 15:10:34	2024-11-25 15:10:34	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	ID	787c67d3-c5d4-4463-a038-62fc408b19c1	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FVIETNAMESE%20CACEO-SDROV%20GLOSSARY_final_REVISED%20_Su%2003-05-2024_Final%2011-19-2023_2024%2011-05_06-03-2024-English.mtf?alt=media&token=036512fe-0792-42c9-a282-bfb005be0bdf	\N
bf187597-2705-4ebe-bc65-44e00aeeed82	VI_Evaluate_%2Fnote	APPROVED	2024-11-26 02:26:32	2024-11-25 19:25:21	2024-11-25 19:25:21	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fnote.txt?alt=media&token=8948327e-fc92-4c9a-b789-f5825d0288a9	VI	daea43de-041f-4073-a1ca-89a63e2d7204	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fnote.txt?alt=media&token=052fc693-94bc-4025-873d-b56ca901074c	\N
3603f5e2-6058-43ad-81fb-40a5b3ae7569	GL_Evaluate_%2Fpackage%20(1)	NEW	\N	2024-11-25 19:37:26	2024-11-25 19:37:26	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=b78c9e0a-f5ba-4064-a0c1-d245846c18f9	GL	8a6d8a19-6524-4d70-a144-342c6951850c	\N	\N
6dab4b17-ab0b-4fde-be59-85f24d4df63b	GL_Evaluate_%2Ftailwind%20(1)	NEW	\N	2024-11-25 19:37:26	2024-11-25 19:37:26	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftailwind%20(1).css?alt=media&token=02bbbff7-ef1c-4797-a1ae-434f315c0b95	GL	8a6d8a19-6524-4d70-a144-342c6951850c	\N	\N
b969e9d3-8589-483e-b1fc-5cfc071a6e9f	GL_Evaluate_%2Fcreate-orders	NEW	\N	2024-11-25 19:37:26	2024-11-25 19:37:26	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=90de1ae2-6399-4843-8e3a-474fe35712de	GL	8a6d8a19-6524-4d70-a144-342c6951850c	\N	\N
008c0662-b914-4456-9cb2-637726f2f5d4	GL_Evaluate_%2Ftailwind%20(1)	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftailwind%20(1).css?alt=media&token=02bbbff7-ef1c-4797-a1ae-434f315c0b95	GL	3266af4b-298a-4d4d-8353-1303b7fa10b0	\N	\N
941ff631-737d-4e64-8782-6b0327aafd7d	BG_Evaluate_uploads%2Ftt-08-3.doc?alt=media&token=8e0a50b5-3721-4bf0-b11b-4689b7b34ff5	APPROVED	\N	2024-11-24 15:56:50	2024-11-24 15:56:50	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftt-08-3.doc?alt=media&token=8e0a50b5-3721-4bf0-b11b-4689b7b34ff5	BG	af85556e-b3a2-406b-abd5-1e997311c339	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FProject%20Weekly%20Report_GroupName.xlsx?alt=media&token=ec15de59-4613-4959-b989-d8a33f4213d9	\N
439af652-6c48-4359-b90d-55987d645ef6	GL_Evaluate_%2Fpackage%20(1)	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=b78c9e0a-f5ba-4064-a0c1-d245846c18f9	GL	8a6d8a19-6524-4d70-a144-342c6951850c	\N	\N
82bc70ce-50bb-4a5a-8a07-61a3c5568564	ID_Translate_uploads%2FHCM202_MKT1706	APPROVED	2024-11-26 14:26:45	2024-11-25 17:06:57	2024-11-25 17:06:57	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FHCM202_MKT1706.xlsx?alt=media&token=4e91ec36-a72a-4a6d-8f60-e0d68fae00a6	ID	fae190d3-1308-43d9-a1bf-5804bfec4fe0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FHCM202_MKT1706%20(1).xlsx?alt=media&token=58b78f48-8a1d-47e0-a728-1451af3355a3	\N
63f03806-1b22-4c36-85f3-0b131e4725c2	GL_Evaluate_%2Ftailwind%20(1)	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftailwind%20(1).css?alt=media&token=02bbbff7-ef1c-4797-a1ae-434f315c0b95	GL	8a6d8a19-6524-4d70-a144-342c6951850c	\N	\N
66966ebf-2b0b-4c9c-8ede-3ad66c5fe134	GL_Evaluate_%2Fpackage%20(1)	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=b78c9e0a-f5ba-4064-a0c1-d245846c18f9	GL	47a7d7f9-f710-4103-99ad-0d6a1dff038f	\N	\N
900bd48a-19a5-4110-8a35-1c9be7b0d67b	GL_Evaluate_%2Fcreate-orders	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=90de1ae2-6399-4843-8e3a-474fe35712de	GL	3266af4b-298a-4d4d-8353-1303b7fa10b0	\N	\N
a6299ac7-f32b-4c35-b0eb-6321df1990f1	GL_Evaluate_%2Fcreate-orders	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=90de1ae2-6399-4843-8e3a-474fe35712de	GL	8a6d8a19-6524-4d70-a144-342c6951850c	\N	\N
bc2806bf-2086-4de4-a896-0acd577ac9de	GL_Evaluate_%2Ftailwind%20(1)	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftailwind%20(1).css?alt=media&token=02bbbff7-ef1c-4797-a1ae-434f315c0b95	GL	47a7d7f9-f710-4103-99ad-0d6a1dff038f	\N	\N
e2d7f44e-f106-43cb-8773-05b51addd524	GL_Evaluate_%2Fcreate-orders	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=90de1ae2-6399-4843-8e3a-474fe35712de	GL	47a7d7f9-f710-4103-99ad-0d6a1dff038f	\N	\N
fd9736c2-7041-439d-8703-9eca6a4cec33	GL_Evaluate_%2Fpackage%20(1)	NEW	\N	2024-11-29 00:16:51	2024-11-29 00:16:51	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=b78c9e0a-f5ba-4064-a0c1-d245846c18f9	GL	3266af4b-298a-4d4d-8353-1303b7fa10b0	\N	\N
25fb1282-ac97-42e7-a903-a46589d7ecfe	BG_Evaluate_%2FAED%20School%20Accreditation_EN	NEW	\N	2024-11-29 00:34:09	2024-11-29 00:34:09	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FAED%20School%20Accreditation_EN.docx?alt=media&token=11061b9c-70a5-4840-b1b6-3227fe3ae9c8	BG	f81dc667-e094-4d28-af04-71bae9ba3880	\N	\N
b46db97d-2f04-487b-85cd-431f19608697	BG_Translate_%2FAED%20School%20Accreditation_EN	NEW	\N	2024-11-29 00:34:09	2024-11-29 00:34:09	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FAED%20School%20Accreditation_EN.docx?alt=media&token=11061b9c-70a5-4840-b1b6-3227fe3ae9c8	BG	655edb80-39bb-4679-b072-e80950b23325	\N	\N
5587c947-749c-4191-a35e-2f7c2639c76a	FR_Translate_VERBUM_%2FCRT%2011686%20Final%20-%20Application-en-vi-PE	NEW	\N	2024-12-02 10:08:21	2024-12-02 10:08:21	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	FR	74bffa3c-62e5-4940-8344-aa42d0e3ad5f	\N	\N
6db8dcf9-f18b-493c-90a1-3742eb3367fc	VI_Translate_VERBUM_%2FCRT%2011686%20Final%20-%20Application-en-vi-PE	NEW	\N	2024-12-02 10:08:21	2024-12-02 10:08:21	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	VI	74bffa3c-62e5-4940-8344-aa42d0e3ad5f	\N	\N
707d608d-bab5-4f2b-956f-384f3fa33688	VI_Evaluate_VERBUM_%2FCRT%2011686%20Final%20-%20Application-en-vi-PE	NEW	\N	2024-12-02 10:08:21	2024-12-02 10:08:21	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	VI	3009dad4-d59a-4b3c-adaa-31d46e709a09	\N	\N
ac35ac4e-1d4d-429f-9f09-983f57fc38a7	VI_Edit_VERBUM_%2FCRT%2011686%20Final%20-%20Application-en-vi-PE	NEW	\N	2024-12-02 10:08:21	2024-12-02 10:08:21	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	VI	e4815fda-ae77-4d54-93c9-eb208aae7232	\N	\N
e298d357-d8e8-4be9-8196-28c8e727edd9	FR_Edit_VERBUM_%2FCRT%2011686%20Final%20-%20Application-en-vi-PE	NEW	\N	2024-12-02 10:08:21	2024-12-02 10:08:21	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	FR	e4815fda-ae77-4d54-93c9-eb208aae7232	\N	\N
16e826af-816e-4b64-876b-4ac3dd4ccd72	FR_Evaluate_VERBUM_%2FCRT%2011686%20Final%20-%20Application-en-vi-PE	IN_PROGRESS	2024-12-11 17:44:21	2024-12-02 10:08:21	2024-12-02 10:08:21	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	FR	3009dad4-d59a-4b3c-adaa-31d46e709a09	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FHCM202_MKT1706%20(1).xlsx?alt=media&token=4d98048b-4798-4a8a-9343-bda6083d2d05	testing
6efe4d3a-a8c6-4940-8eaf-8099241fe819	VI_Edit_VERBUM_%2FAED-R%20School-Level%20Accountability%20Committee_EN	NEW	\N	2024-12-02 13:58:44	2024-12-02 13:58:44	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FAED-R%20School-Level%20Accountability%20Committee_EN.docx?alt=media&token=b6c9382a-b0e1-49ad-9959-34eb12f10526	VI	c867db3c-4775-468e-9893-1e4dd080b157	\N	\N
bda4dba7-bcd2-4340-8863-4184e32fe513	VI_Translate_VERBUM_%2FAED-R%20School-Level%20Accountability%20Committee_EN	NEW	\N	2024-12-02 13:58:44	2024-12-02 13:58:44	0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FAED-R%20School-Level%20Accountability%20Committee_EN.docx?alt=media&token=b6c9382a-b0e1-49ad-9959-34eb12f10526	VI	4c9cdd8f-dab3-4f0d-bd45-c8d6c1d4fcab	\N	\N
\.


--
-- TOC entry 4525 (class 0 OID 17562)
-- Dependencies: 220
-- Data for Name: language; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.language (language_name, language_id, support) FROM stdin;
Armenian	HY	f
Galician	GL	f
Indonesian	ID	f
Bulgarian	BG	f
Portuguese	PT	f
Hindi	HI	f
Bengali	BN	f
Punjabi	PA	f
Javanese	JV	f
Amharic	AM	f
Italian	IT	f
Polish	PL	f
Ukrainian	UK	f
Dutch	NL	f
Greek	EL	f
Turkish	TR	f
Persian	FA	f
Hungarian	HU	f
Czech	CS	f
Swedish	SV	f
Danish	DA	f
Finnish	FI	f
Norwegian	NO	f
Hebrew	HE	f
Thai	TH	f
Arabic	AR	f
Malay	MS	f
Romanian	RO	f
Serbian	SR	f
Slovak	SK	f
Lithuanian	LT	f
Latvian	LV	f
Estonian	ET	f
Slovenian	SL	f
Icelandic	IS	f
Maltese	MT	f
Irish	GA	f
Welsh	CY	f
Catalan	CA	f
Basque	EU	f
English	EN	t
Swahili	SW	f
Zulu	ZU	f
Xhosa	XH	f
Macedonian	MK	f
Georgian	KA	f
Azerbaijani	AZ	f
Uzbek	UZ	f
Kazakh	KK	f
Mongolian	MN	f
Sinhala	SI	f
Tamil	TA	f
Telugu	TE	f
Malayalam	ML	f
Kannada	KN	f
Marathi	MR	f
Gujarati	GU	f
Odia	OR	f
Assamese	AS	f
Burmese	MY	f
Khmer	KM	f
Lao	LO	f
Chichewa	NY	f
Haitian Creole	HT	f
Yiddish	YI	f
Luxembourgish	LB	f
Bosnian	BS	f
Tajik	TG	f
Kyrgyz	KY	f
Turkmen	TK	f
Dhivehi	DV	f
Nepali	NE	f
Pashto	PS	f
Urdu	UR	f
Tagalog	TL	f
Hawaiian	HAW	f
Samoan	SM	f
Maori	MI	f
Russian	RU	f
Albanian	SQ	f
Afrikaans	AF	f
German	DE	f
Spanish	ES	f
French	FR	f
Japanese	JA	f
Chinese	ZH	f
Croatian	HR	f
Korean	KO	f
Faroese	FO	f
Yoruba	YO	f
Igbo	IG	f
Hausa	HA	f
Kirundi	RN	f
Vietnamese	VI	t
Kinyarwanda	RW	f
\.


--
-- TOC entry 4526 (class 0 OID 17567)
-- Dependencies: 221
-- Data for Name: order; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public."order" (order_id, client_id, due_date, source_language_id, order_status, order_price, discount_id, has_translate_service, has_edit_service, has_evaluate_service, created_date, order_name, reject_reason, order_note) FROM stdin;
2ba3cb40-daaa-44a7-8428-cf042da0d99c	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:25:05.522198	ORD-00066	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
66b7f9a6-fdfa-470f-a1ee-993d9a906d58	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:25:27.675912	ORD-00067	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
bc91dca5-9219-4a68-970b-190b8dab8a35	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:31:26.295209	ORD-00069	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
7c82ccae-387e-4d12-90a2-0be51ea85c9a	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:33:19.078738	ORD-00070	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
26aa4a65-98c5-4b8d-9bd3-c48b13450c4d	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:40:12.778706	ORD-00071	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
95e40995-6624-4d28-93f2-b504de14d333	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:40:13.666213	ORD-00072	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
a6e017c9-12e8-46a0-a6d1-6a034c393e9a	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:40:17.140039	ORD-00073	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
d338c79e-7d80-4ecc-bb29-b91742b83e8b	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:40:18.162039	ORD-00074	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
2779fc5c-4f6e-4629-9d41-205c2d2ee7e9	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	t	t	2024-11-29 10:40:19.24199	ORD-00075	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
c0d2e350-d57f-4026-a548-aa8e5fd7f77c	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-11-30 00:00:00	MT	DELIVERED	120	\N	f	f	t	2024-11-25 19:03:43.690206	ORD-00055	\N	\N
cee58571-73b1-4d0e-8b30-47d20514adcc	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-31 00:00:00	EN	NEW	\N	\N	t	f	f	2024-11-29 09:07:57.308913	ORD-00060	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
7aa54ee1-4058-40fa-a7a8-404ed5a82c2d	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-01 00:00:00	GL	IN_PROGRESS	\N	\N	t	f	f	2024-11-25 17:06:28.814486	ORD-00053	\N	\N
e6182e64-4961-4fda-8f4f-f67c2290a204	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-31 03:59:00	EN	IN_PROGRESS	400	\N	t	t	t	2024-11-29 09:08:00.56783	ORD-00061	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
3f29a214-f2dc-4b67-bc8c-d0997e74b5ab	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-11-09 00:00:00	HY	IN_PROGRESS	600	\N	f	f	t	2024-11-24 15:56:10.557279	ORD-00048	\N	\N
40aff376-157f-45c4-b88f-aa19f79a93e8	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-11-29 00:00:00	GL	NEW	\N	\N	f	f	t	2024-11-24 16:33:39.022921	ORD-00051	\N	\N
3bdafae2-d30b-4c5d-8462-942265626faa	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-11-29 00:00:00	EN	IN_PROGRESS	700	\N	t	f	f	2024-11-25 15:09:38.212109	ORD-00052	\N	\N
f17cee86-fdee-44f8-8a76-6a2a1e6fc785	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-11-30 00:00:00	GL	CANCELLED	\N	\N	f	f	t	2024-11-24 16:19:55.207119	ORD-00049	\N	\N
43c21b33-25d9-4f84-a9e8-2be04d139738	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-11-30 00:00:00	HY	IN_PROGRESS	200	\N	f	f	t	2024-11-24 16:24:25.622798	ORD-00050	\N	\N
600687a6-c54f-4254-b262-4651899cefae	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-11-30 00:00:00	TH	IN_PROGRESS	50	\N	t	f	t	2024-11-25 17:49:53.7957	ORD-00054	\N	\N
055d648a-f7ec-4772-b084-2fc6c8256f68	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 04:06:50.423542	ORD-00056	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
0ad6050c-b90f-4714-a48e-3d4ef7f47fa8	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-31 03:59:00	EN	NEW	\N	\N	t	t	t	2024-11-29 06:48:27.916199	ORD-00057	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
595f294b-d0bb-4e76-8724-f7bcfa61c1e1	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-31 03:59:00	EN	NEW	\N	\N	t	t	t	2024-11-29 06:48:56.085763	ORD-00058	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
b7e3e25c-f13e-4b75-932f-9f9b2bc2649c	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 06:49:08.085293	ORD-00059	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
a4084a36-35da-40f6-a3be-aa0e073ed2f7	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 09:09:19.692144	ORD-00062	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
322464fb-3296-44c6-8d37-3365550a9a89	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	t	t	2024-11-29 09:09:20.686111	ORD-00063	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
526ca05c-6d6a-43d2-96dd-1bc750347dc4	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 09:25:18.112653	ORD-00064	\N	
b411b749-0c5f-4792-8463-13c507b7a571	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 09:46:33.829816	ORD-00065	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
a5f511ff-2552-4007-a2e3-5752b2f85aa5	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:40:22.481869	ORD-00076	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
4d5e3913-37ca-4504-ae2d-0549fe66d441	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:40:25.801347	ORD-00077	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
02917f41-453a-4ed9-8f51-3e763f29cc81	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-30 00:00:00	ZU	NEW	\N	\N	t	f	f	2024-12-02 13:14:37.382598	ORD-00098	\N	\N
15ee0977-d156-45ea-9566-81d517903a47	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-29 10:25:53.980479	ORD-00068	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
ee062a85-4583-4022-bbdb-cde23a00ed1a	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-30 14:24:43.218141	ORD-00084	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
bd956c13-e41d-4c1b-9c34-636c00bc1fb5	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-30 14:24:44.164863	ORD-00085	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
3cff2713-fbeb-40cf-8417-f4c5dd196d74	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-30 14:24:47.48871	ORD-00086	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
9c2de7f6-be43-4b42-a4ce-7f61c9a10500	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-30 14:24:50.503766	ORD-00087	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
d16adee2-7f1b-4367-bcdb-1bd9212ddf7b	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	t	t	2024-11-30 14:24:51.60798	ORD-00088	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
3a3584fb-fb95-496d-980b-7bb99ff2291d	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	t	t	2024-11-30 14:24:52.802838	ORD-00089	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
3520bce7-85a1-4a47-80f7-bdcfdd732505	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	REJECTED	\N	\N	t	t	t	2024-11-29 10:40:26.589794	ORD-00078	bruh	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
2701de74-78d6-4914-af63-151dedc639d5	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-30 14:20:07.358503	ORD-00079	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
9d1fdf08-3ef8-4670-9d55-244692281a3e	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-30 14:20:10.316032	ORD-00080	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
112a8f9f-b52f-45ca-a809-1945469d0755	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-30 14:20:14.230979	ORD-00081	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
df8eabee-0b88-4547-85cb-1b17a6eb54cf	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	f	f	2024-11-30 14:20:15.052657	ORD-00082	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
3d0025e7-4db3-4ed4-894a-fa4005bc9fc0	baa8ffbe-6495-492c-91ee-e33e9d187e28	2024-12-31 03:59:00	EN	NEW	\N	\N	t	t	t	2024-11-30 14:20:16.185081	ORD-00083	\N	Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.
2801c823-f9a6-4dce-8c9b-71b5773cc197	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-31 00:00:00	HY	CANCELLED	\N	fd8d7e9d-064b-440a-bf65-2bf8a91e1daa	t	f	f	2024-12-02 12:33:19.900892	ORD-00092	\N	\N
6b6c5ee8-bbc3-4212-87a1-77444bcd062d	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-28 00:00:00	HY	NEW	\N	\N	t	f	f	2024-12-02 12:56:02.508793	ORD-00093	\N	\N
f5298942-16a5-4169-a302-3b8e7e6beab9	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-28 00:00:00	BG	CANCELLED	\N	\N	t	f	f	2024-12-02 12:56:59.92247	ORD-00094	\N	\N
92c1901c-0890-449f-9e30-31f2b088318d	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-31 00:00:00	PA	NEW	\N	\N	t	f	f	2024-12-02 12:57:55.019786	ORD-00095	\N	\N
f8453b99-cf69-48e9-84c1-ffccf487a06f	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-21 00:00:00	HY	NEW	\N	\N	t	f	f	2024-12-02 13:09:14.24518	ORD-00096	\N	\N
f368eee7-bb30-4292-b97f-022a4c71f062	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2024-12-26 00:00:00	GL	NEW	\N	30812060-0c3c-47e4-899e-601d52b10a55	t	f	f	2024-12-02 13:13:54.632589	ORD-00097	\N	\N
c91826f4-3c9a-4c51-b082-9a1e7e3d8ea4	3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	2025-01-01 00:00:00	EN	IN_PROGRESS	99	\N	t	t	f	2024-12-02 13:54:20.217134	ORD-00099	\N	\N
\.


--
-- TOC entry 4527 (class 0 OID 17573)
-- Dependencies: 222
-- Data for Name: order_references; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.order_references (order_id, reference_file_url, tag, is_deleted) FROM stdin;
f17cee86-fdee-44f8-8a76-6a2a1e6fc785	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=541ecf05-3928-4547-9654-fc8f42b47849	TRANSLATION	f
f17cee86-fdee-44f8-8a76-6a2a1e6fc785	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftt-08-3.doc?alt=media&token=0c1e159c-1016-4cb6-bdfc-8d2771b3f9e7	TRANSLATION	f
f17cee86-fdee-44f8-8a76-6a2a1e6fc785	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Flogo.png?alt=media&token=301c13a9-aa41-4982-92b6-8b1642d81553	REFERENCES	f
f17cee86-fdee-44f8-8a76-6a2a1e6fc785	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FOne.vue?alt=media&token=cb4cf9b2-8dcf-4fcc-a01d-c70d98d6dbf8	REFERENCES	f
f17cee86-fdee-44f8-8a76-6a2a1e6fc785	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FTwo.vue?alt=media&token=654b8ff8-b061-4056-ae0d-aa11778dc1ca	REFERENCES	f
f17cee86-fdee-44f8-8a76-6a2a1e6fc785	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=5c61f5a6-3463-4eae-8676-938faaa68a9f	TRANSLATION	t
40aff376-157f-45c4-b88f-aa19f79a93e8	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=7ce06e59-8677-4ae5-ae67-04dad148cad1	TRANSLATION	f
40aff376-157f-45c4-b88f-aa19f79a93e8	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=4d4279b3-ac2f-46d8-bfc5-9183ec135c8c	TRANSLATION	f
40aff376-157f-45c4-b88f-aa19f79a93e8	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftailwind%20(1).css?alt=media&token=081c996b-b16d-4e6c-b212-81cde460d4f3	TRANSLATION	f
40aff376-157f-45c4-b88f-aa19f79a93e8	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=cfff016c-2594-4cf8-a279-d58d124231e7	REFERENCES	f
40aff376-157f-45c4-b88f-aa19f79a93e8	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=b465cfbd-433e-4fc4-a4e5-dd8cf61efe59	REFERENCES	f
40aff376-157f-45c4-b88f-aa19f79a93e8	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftailwind%20(1).css?alt=media&token=576bb3ba-51d6-4570-9be8-29f75ee8d6d3	REFERENCES	f
3bdafae2-d30b-4c5d-8462-942265626faa	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
600687a6-c54f-4254-b262-4651899cefae	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FAED%20School%20Accreditation_EN.docx?alt=media&token=11061b9c-70a5-4840-b1b6-3227fe3ae9c8	TRANSLATION	f
055d648a-f7ec-4772-b084-2fc6c8256f68	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
055d648a-f7ec-4772-b084-2fc6c8256f68	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
cee58571-73b1-4d0e-8b30-47d20514adcc	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
e6182e64-4961-4fda-8f4f-f67c2290a204	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
e6182e64-4961-4fda-8f4f-f67c2290a204	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
526ca05c-6d6a-43d2-96dd-1bc750347dc4	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
526ca05c-6d6a-43d2-96dd-1bc750347dc4	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
15ee0977-d156-45ea-9566-81d517903a47	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
15ee0977-d156-45ea-9566-81d517903a47	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	TRANSLATION	f
7c82ccae-387e-4d12-90a2-0be51ea85c9a	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
26aa4a65-98c5-4b8d-9bd3-c48b13450c4d	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
95e40995-6624-4d28-93f2-b504de14d333	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
95e40995-6624-4d28-93f2-b504de14d333	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
a6e017c9-12e8-46a0-a6d1-6a034c393e9a	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
a6e017c9-12e8-46a0-a6d1-6a034c393e9a	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
d338c79e-7d80-4ecc-bb29-b91742b83e8b	{{referenceFileUrls2}}	TRANSLATION	f
d338c79e-7d80-4ecc-bb29-b91742b83e8b	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
d338c79e-7d80-4ecc-bb29-b91742b83e8b	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
3f29a214-f2dc-4b67-bc8c-d0997e74b5ab	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftt-08-3.doc?alt=media&token=8e0a50b5-3721-4bf0-b11b-4689b7b34ff5	TRANSLATION	f
43c21b33-25d9-4f84-a9e8-2be04d139738	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fcreate-orders.vue?alt=media&token=90de1ae2-6399-4843-8e3a-474fe35712de	TRANSLATION	f
43c21b33-25d9-4f84-a9e8-2be04d139738	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fpackage%20(1).json?alt=media&token=b78c9e0a-f5ba-4064-a0c1-d245846c18f9	TRANSLATION	f
43c21b33-25d9-4f84-a9e8-2be04d139738	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftailwind%20(1).css?alt=media&token=02bbbff7-ef1c-4797-a1ae-434f315c0b95	TRANSLATION	f
43c21b33-25d9-4f84-a9e8-2be04d139738	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Ftt-08-3.doc?alt=media&token=f9283c4a-4d29-431e-b9e0-33081e35dfc8	REFERENCES	f
3bdafae2-d30b-4c5d-8462-942265626faa	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
7aa54ee1-4058-40fa-a7a8-404ed5a82c2d	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FHCM202_MKT1706.xlsx?alt=media&token=4e91ec36-a72a-4a6d-8f60-e0d68fae00a6	TRANSLATION	f
c0d2e350-d57f-4026-a548-aa8e5fd7f77c	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fnote.txt?alt=media&token=8948327e-fc92-4c9a-b789-f5825d0288a9	TRANSLATION	f
0ad6050c-b90f-4714-a48e-3d4ef7f47fa8	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
0ad6050c-b90f-4714-a48e-3d4ef7f47fa8	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
595f294b-d0bb-4e76-8724-f7bcfa61c1e1	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
595f294b-d0bb-4e76-8724-f7bcfa61c1e1	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
b7e3e25c-f13e-4b75-932f-9f9b2bc2649c	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
b7e3e25c-f13e-4b75-932f-9f9b2bc2649c	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
a4084a36-35da-40f6-a3be-aa0e073ed2f7	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
a4084a36-35da-40f6-a3be-aa0e073ed2f7	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
322464fb-3296-44c6-8d37-3365550a9a89	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
322464fb-3296-44c6-8d37-3365550a9a89	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
b411b749-0c5f-4792-8463-13c507b7a571	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
bc91dca5-9219-4a68-970b-190b8dab8a35	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
bc91dca5-9219-4a68-970b-190b8dab8a35	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
7c82ccae-387e-4d12-90a2-0be51ea85c9a	{{referenceFileUrls2}}	TRANSLATION	f
7c82ccae-387e-4d12-90a2-0be51ea85c9a	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
2779fc5c-4f6e-4629-9d41-205c2d2ee7e9	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
2779fc5c-4f6e-4629-9d41-205c2d2ee7e9	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
a5f511ff-2552-4007-a2e3-5752b2f85aa5	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
4d5e3913-37ca-4504-ae2d-0549fe66d441	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
4d5e3913-37ca-4504-ae2d-0549fe66d441	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
3520bce7-85a1-4a47-80f7-bdcfdd732505	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
3520bce7-85a1-4a47-80f7-bdcfdd732505	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
2701de74-78d6-4914-af63-151dedc639d5	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
9d1fdf08-3ef8-4670-9d55-244692281a3e	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
9d1fdf08-3ef8-4670-9d55-244692281a3e	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
112a8f9f-b52f-45ca-a809-1945469d0755	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
112a8f9f-b52f-45ca-a809-1945469d0755	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
df8eabee-0b88-4547-85cb-1b17a6eb54cf	{{referenceFileUrls2}}	TRANSLATION	f
df8eabee-0b88-4547-85cb-1b17a6eb54cf	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
df8eabee-0b88-4547-85cb-1b17a6eb54cf	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
3d0025e7-4db3-4ed4-894a-fa4005bc9fc0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
3d0025e7-4db3-4ed4-894a-fa4005bc9fc0	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
ee062a85-4583-4022-bbdb-cde23a00ed1a	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
bd956c13-e41d-4c1b-9c34-636c00bc1fb5	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
bd956c13-e41d-4c1b-9c34-636c00bc1fb5	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
3cff2713-fbeb-40cf-8417-f4c5dd196d74	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
3cff2713-fbeb-40cf-8417-f4c5dd196d74	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
9c2de7f6-be43-4b42-a4ce-7f61c9a10500	{{referenceFileUrls2}}	TRANSLATION	f
9c2de7f6-be43-4b42-a4ce-7f61c9a10500	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
9c2de7f6-be43-4b42-a4ce-7f61c9a10500	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
d16adee2-7f1b-4367-bcdb-1bd9212ddf7b	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
d16adee2-7f1b-4367-bcdb-1bd9212ddf7b	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
3a3584fb-fb95-496d-980b-7bb99ff2291d	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	f
3a3584fb-fb95-496d-980b-7bb99ff2291d	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FIC-Project-Management-with-Gantt-Schedule-Template-10689.xlsx?alt=media&token=66d0f7b7-a139-4850-b025-3e43e8ea0f62	REFERENCES	f
cee58571-73b1-4d0e-8b30-47d20514adcc	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FCRT%2011686%20Final%20-%20Application-en-vi-PE.mxliff?alt=media&token=3e70a72d-e4ea-443d-adc1-972cbd3aef44	TRANSLATION	t
2801c823-f9a6-4dce-8c9b-71b5773cc197	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FVERBUM.postman_collection.json?alt=media&token=16d49daf-f0f6-4930-be6d-758c75fd09cd	TRANSLATION	f
6b6c5ee8-bbc3-4212-87a1-77444bcd062d	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Flogo.png?alt=media&token=1aac25c8-55b3-4ce1-a824-78da5f536f37	TRANSLATION	f
f5298942-16a5-4169-a302-3b8e7e6beab9	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Fworkspace.postman_globals.json?alt=media&token=890fcc75-320e-4857-b250-05d4c5ec1e87	TRANSLATION	f
92c1901c-0890-449f-9e30-31f2b088318d	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Flogo.png?alt=media&token=14b07da2-705d-4184-aef3-e50db42edc17	TRANSLATION	f
f8453b99-cf69-48e9-84c1-ffccf487a06f	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2Flogo.png?alt=media&token=fe847759-78cf-4ad7-8f46-013de9ef8b2a	TRANSLATION	f
f368eee7-bb30-4292-b97f-022a4c71f062	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FLinguist%20Workflow.png?alt=media&token=35467fe1-abfe-43ee-a44d-cd1179f0f7a1	TRANSLATION	f
02917f41-453a-4ed9-8f51-3e763f29cc81	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FClient%20Workflow.png?alt=media&token=40679bbf-b671-4c51-a0df-f8f240830089	TRANSLATION	f
c91826f4-3c9a-4c51-b082-9a1e7e3d8ea4	https://firebasestorage.googleapis.com/v0/b/verbum-sep490.appspot.com/o/uploads%2FAED-R%20School-Level%20Accountability%20Committee_EN.docx?alt=media&token=b6c9382a-b0e1-49ad-9959-34eb12f10526	TRANSLATION	f
\.


--
-- TOC entry 4528 (class 0 OID 17578)
-- Dependencies: 223
-- Data for Name: order_target_language; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.order_target_language (order_id, target_language_id) FROM stdin;
3f29a214-f2dc-4b67-bc8c-d0997e74b5ab	BG
f17cee86-fdee-44f8-8a76-6a2a1e6fc785	HY
43c21b33-25d9-4f84-a9e8-2be04d139738	GL
40aff376-157f-45c4-b88f-aa19f79a93e8	JV
3bdafae2-d30b-4c5d-8462-942265626faa	HY
3bdafae2-d30b-4c5d-8462-942265626faa	ID
7aa54ee1-4058-40fa-a7a8-404ed5a82c2d	ID
600687a6-c54f-4254-b262-4651899cefae	BG
c0d2e350-d57f-4026-a548-aa8e5fd7f77c	VI
055d648a-f7ec-4772-b084-2fc6c8256f68	HY
055d648a-f7ec-4772-b084-2fc6c8256f68	ID
0ad6050c-b90f-4714-a48e-3d4ef7f47fa8	FR
0ad6050c-b90f-4714-a48e-3d4ef7f47fa8	VI
595f294b-d0bb-4e76-8724-f7bcfa61c1e1	FR
595f294b-d0bb-4e76-8724-f7bcfa61c1e1	VI
b7e3e25c-f13e-4b75-932f-9f9b2bc2649c	HY
b7e3e25c-f13e-4b75-932f-9f9b2bc2649c	ID
e6182e64-4961-4fda-8f4f-f67c2290a204	FR
e6182e64-4961-4fda-8f4f-f67c2290a204	VI
a4084a36-35da-40f6-a3be-aa0e073ed2f7	HY
a4084a36-35da-40f6-a3be-aa0e073ed2f7	ID
322464fb-3296-44c6-8d37-3365550a9a89	FR
322464fb-3296-44c6-8d37-3365550a9a89	VI
526ca05c-6d6a-43d2-96dd-1bc750347dc4	HY
526ca05c-6d6a-43d2-96dd-1bc750347dc4	ID
b411b749-0c5f-4792-8463-13c507b7a571	HY
b411b749-0c5f-4792-8463-13c507b7a571	ID
2ba3cb40-daaa-44a7-8428-cf042da0d99c	HY
2ba3cb40-daaa-44a7-8428-cf042da0d99c	ID
66b7f9a6-fdfa-470f-a1ee-993d9a906d58	HY
66b7f9a6-fdfa-470f-a1ee-993d9a906d58	ID
bc91dca5-9219-4a68-970b-190b8dab8a35	HY
bc91dca5-9219-4a68-970b-190b8dab8a35	ID
7c82ccae-387e-4d12-90a2-0be51ea85c9a	HY
7c82ccae-387e-4d12-90a2-0be51ea85c9a	ID
26aa4a65-98c5-4b8d-9bd3-c48b13450c4d	HY
26aa4a65-98c5-4b8d-9bd3-c48b13450c4d	ID
95e40995-6624-4d28-93f2-b504de14d333	HY
95e40995-6624-4d28-93f2-b504de14d333	ID
a6e017c9-12e8-46a0-a6d1-6a034c393e9a	HY
a6e017c9-12e8-46a0-a6d1-6a034c393e9a	ID
d338c79e-7d80-4ecc-bb29-b91742b83e8b	HY
d338c79e-7d80-4ecc-bb29-b91742b83e8b	ID
2779fc5c-4f6e-4629-9d41-205c2d2ee7e9	FR
2779fc5c-4f6e-4629-9d41-205c2d2ee7e9	VI
a5f511ff-2552-4007-a2e3-5752b2f85aa5	HY
a5f511ff-2552-4007-a2e3-5752b2f85aa5	ID
4d5e3913-37ca-4504-ae2d-0549fe66d441	HY
4d5e3913-37ca-4504-ae2d-0549fe66d441	ID
3520bce7-85a1-4a47-80f7-bdcfdd732505	FR
3520bce7-85a1-4a47-80f7-bdcfdd732505	VI
2701de74-78d6-4914-af63-151dedc639d5	HY
2701de74-78d6-4914-af63-151dedc639d5	ID
9d1fdf08-3ef8-4670-9d55-244692281a3e	HY
9d1fdf08-3ef8-4670-9d55-244692281a3e	ID
112a8f9f-b52f-45ca-a809-1945469d0755	HY
112a8f9f-b52f-45ca-a809-1945469d0755	ID
df8eabee-0b88-4547-85cb-1b17a6eb54cf	HY
df8eabee-0b88-4547-85cb-1b17a6eb54cf	ID
3d0025e7-4db3-4ed4-894a-fa4005bc9fc0	FR
3d0025e7-4db3-4ed4-894a-fa4005bc9fc0	VI
15ee0977-d156-45ea-9566-81d517903a47	HY
15ee0977-d156-45ea-9566-81d517903a47	ID
ee062a85-4583-4022-bbdb-cde23a00ed1a	HY
ee062a85-4583-4022-bbdb-cde23a00ed1a	ID
bd956c13-e41d-4c1b-9c34-636c00bc1fb5	HY
bd956c13-e41d-4c1b-9c34-636c00bc1fb5	ID
3cff2713-fbeb-40cf-8417-f4c5dd196d74	HY
3cff2713-fbeb-40cf-8417-f4c5dd196d74	ID
9c2de7f6-be43-4b42-a4ce-7f61c9a10500	HY
9c2de7f6-be43-4b42-a4ce-7f61c9a10500	ID
d16adee2-7f1b-4367-bcdb-1bd9212ddf7b	FR
d16adee2-7f1b-4367-bcdb-1bd9212ddf7b	VI
3a3584fb-fb95-496d-980b-7bb99ff2291d	FR
3a3584fb-fb95-496d-980b-7bb99ff2291d	VI
cee58571-73b1-4d0e-8b30-47d20514adcc	HY
cee58571-73b1-4d0e-8b30-47d20514adcc	ID
2801c823-f9a6-4dce-8c9b-71b5773cc197	HY
6b6c5ee8-bbc3-4212-87a1-77444bcd062d	ID
f5298942-16a5-4169-a302-3b8e7e6beab9	SV
92c1901c-0890-449f-9e30-31f2b088318d	ET
92c1901c-0890-449f-9e30-31f2b088318d	IS
f8453b99-cf69-48e9-84c1-ffccf487a06f	ID
f368eee7-bb30-4292-b97f-022a4c71f062	PA
02917f41-453a-4ed9-8f51-3e763f29cc81	HE
c91826f4-3c9a-4c51-b082-9a1e7e3d8ea4	VI
\.


--
-- TOC entry 4529 (class 0 OID 17583)
-- Dependencies: 224
-- Data for Name: rating; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.rating (rating_id, order_id, in_time, expectation, issue_resolved, more_thought) FROM stdin;
2b86fa98-8b51-4143-95d5-4370501f98b2	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	5	5	5	gud job !! 👍
\.


--
-- TOC entry 4542 (class 0 OID 17866)
-- Dependencies: 237
-- Data for Name: receipt; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.receipt (receipt_id, pay_date, order_id, deposite_or_payment, amount, done, payment_id) FROM stdin;
760dec15-572f-45a4-b05e-117480b83ee5	2024-11-24 23:34:26.969585	3f29a214-f2dc-4b67-bc8c-d0997e74b5ab	t	300.00	f	PAYID-M5BVLEQ5U784141KJ729161G
b2627bfb-2428-408f-b76e-51196f8b1faa	2024-11-25 11:22:51.055357	3f29a214-f2dc-4b67-bc8c-d0997e74b5ab	f	300.00	f	PAYID-M5CF4CQ1C202443V9522810H
f33a644b-c0d0-4932-95c0-240b06f6d722	2024-11-25 15:40:38.401786	3bdafae2-d30b-4c5d-8462-942265626faa	t	350.00	f	PAYID-M5CJU5Q1CD98804FS994953L
0fa1b673-ebf0-4c8c-aecb-639039ea95e7	2024-11-25 19:08:15.442759	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	f	60.00	t	PAYID-M5CMWHI5T393274AL499043V
4ef2b216-1fd1-4d3c-895b-ebce70016b83	2024-11-25 19:12:12.940717	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	t	60.00	t	PAYID-M5CMYCY36J86906R28461941
096b04b3-e166-4ff9-884c-2429247b170f	2024-11-25 19:18:20.467311	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	t	60.00	f	PAYID-M5CM26Q85H42180GU7890723
bbea40e4-2b90-4c44-8d3e-fbe26aa27c8d	2024-11-25 19:19:10.919299	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	t	60.00	f	PAYID-M5CM3LI14E609572T451950E
888533b3-d73f-4f8e-93e3-65426af67303	2024-11-25 19:20:23.434601	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	t	60.00	f	PAYID-M5CM35I96R80387BK927901M
f22ab61e-4807-4ac6-a39b-a6b5174b3e5c	2024-11-25 19:22:35.506389	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	t	60.00	f	PAYID-M5CM46I1NU578902L007053E
4e50348d-0dbc-48eb-ad7b-4f7e4f4e6986	2024-11-25 19:25:23.259279	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	t	60.00	t	PAYID-M5CM6II2DX22043KU571032E
b50aed44-ac5c-4ff4-810a-49f8f53941b8	2024-11-25 19:28:41.992176	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	t	60.00	t	PAYID-M5CM72A72357447NK425334M
919b17c1-c4a8-40d3-b519-a22dcac29a29	2024-11-25 19:36:29.629715	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	f	60.00	t	PAYID-M5CNDOY3GE29923B2890052V
d5cab941-48cc-42f5-803c-3d00d472f63e	2024-11-25 19:37:29.055774	43c21b33-25d9-4f84-a9e8-2be04d139738	f	100.00	t	PAYID-M5CND5Y7K20287798167334U
c3883e2b-5059-4042-a3f2-8804d5daf2e7	2024-11-29 00:15:17.237619	43c21b33-25d9-4f84-a9e8-2be04d139738	t	100.00	t	PAYID-M5EKKJI0XK551979K815720E
24c7f788-0f97-486d-b5c7-55c6fc6bc333	2024-11-29 00:32:12.116477	600687a6-c54f-4254-b262-4651899cefae	t	25.00	t	PAYID-M5EKSGY8X945980FN0701435
681435d0-7bed-42a7-a98c-caaaa29b7281	2024-12-02 08:41:16.511884	e6182e64-4961-4fda-8f4f-f67c2290a204	t	200.00	f	PAYID-M5GXFLA3XJ103516K592620K
c473549f-b246-492e-928f-07d5613b389a	2024-12-02 10:07:13.408016	e6182e64-4961-4fda-8f4f-f67c2290a204	t	200.00	t	PAYID-M5GYNUQ8WK592349V365352L
38e9ac48-dfec-4ca5-8a85-b3b47f52b3e1	2024-12-02 13:57:22.66239	c91826f4-3c9a-4c51-b082-9a1e7e3d8ea4	t	49.50	t	PAYID-M5G3ZQY4U6140018P036043J
\.


--
-- TOC entry 4530 (class 0 OID 17588)
-- Dependencies: 225
-- Data for Name: refresh_token; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.refresh_token (token_id, issued_at, expire_at, token_content) FROM stdin;
26	2024-10-13 21:28:57.046723	2025-12-29 05:46:16.53737	R2ol5au4DWwbDw5DGDQ9bTPYRJYn/fuHMANMQqPFScE=
24	2024-10-08 16:50:11.404279	2025-12-26 14:29:26.14499	ggqQTXa3oOV9ePXWH1PGEg3/YCDVlzqQhDkGoXcuGbk=
8	2024-10-07 03:53:10.555596	2026-01-27 14:45:41.561019	ikxhMVXmOrMvGrb/Zr/5Qn3+Wio3xjJyNwPi130o2QQ=
44	2024-10-24 20:44:10.769785	2026-02-02 11:24:38.812104	oi1r/jtl+pi6KSLwgJGW/lzL9uufEkNaIqjWptvZA+0=
40	2024-10-18 15:32:33.931901	2026-02-02 13:37:56.613465	NipuHoud2SpiMIWZXjlxLE67KwnPi1LSf8eu3wRMam4=
41	2024-10-18 15:47:34.527128	2026-02-02 13:41:07.039589	c4X76rDsDEoCtoAL2HqBOBtdZnH8xPiQdF1niD3if68=
46	2024-10-28 14:15:02.161983	2024-11-11 14:15:02.162015	tg8Qo+s9qArI6NVQowW95ymtg0PHWjVDJo+jdWjjVuc=
42	2024-10-21 14:52:06.686318	2026-02-02 13:54:51.167855	X3DwvZ5G6aCm1hjp8VTdSnrLKab/Qu51Rgpfbn45GSw=
22	2024-10-07 20:34:11.126085	2026-02-02 13:56:08.68407	NuXItQvF7oLwmEUstHn7Bc0m0d0t2qpCwnw0rXxH05k=
47	2024-11-29 07:27:34.67306	2026-01-30 14:05:28.436843	vtnAGoeco6MSsvASpuhtro0KK0NVZQyzvSTgUjlExlk=
43	2024-10-21 14:59:25.749688	2026-02-02 13:56:46.6128	+FxBdsOBPQzvWYGA8jgTd/iMDY+I3aiee1qZkUwqQRg=
45	2024-10-24 21:24:13.839015	2026-02-02 13:59:21.584512	yikkb/oh0QvFTDB7WXx7KlViAWjWS/lWa1QMBGHqv/c=
39	2024-10-15 06:46:00.114957	2026-01-30 15:07:00.331871	7BmaXPGIELeucUqTvFwwUAEwN86jN61m/B7yOCVqUMA=
\.


--
-- TOC entry 4532 (class 0 OID 17594)
-- Dependencies: 227
-- Data for Name: revelancy; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.revelancy (revelancy_id, user_id, source_language_id, target_language_id, category_id, service_code) FROM stdin;
19fcb202-89d3-40b8-b2bf-445869e09303	5cfe0fea-44fc-45eb-b999-c43aa9590820	EN	GL	6	ED
\.


--
-- TOC entry 4533 (class 0 OID 17599)
-- Dependencies: 228
-- Data for Name: role; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.role (role_id, role_name) FROM stdin;
ADMIN	Administrator
STAFF	Staff
TRANSLATE_MANAGER	Translate Manager
EDIT_MANAGER	Edit Manager
EVALUATE_MANAGER	Evaluate Manager
CLIENT	Client
DIRECTOR	Center Director
LINGUIST	Linguist
\.


--
-- TOC entry 4534 (class 0 OID 17604)
-- Dependencies: 229
-- Data for Name: services; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.services (service_name, service_code, service_price, service_order) FROM stdin;
Translate	TL	\N	1
Edit	ED	\N	2
Evaluate	EV	\N	3
\.


--
-- TOC entry 4536 (class 0 OID 17614)
-- Dependencies: 231
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public."user" (id, name, email, email_verified, image_id, password, created_at, updated_at, status, token_id, role_code) FROM stdin;
ee40039b-5c1d-4ab0-8731-e9fd458b196a	Lê Đại Nguyên	yolo1503gg@gmail.com	2024-10-24 20:44:10.534	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-10-24 20:44:10.534	2024-10-24 20:44:10.534	ACTIVE	44	LINGUIST
b0059b6c-8220-422a-8cf0-2f21d0e0590b	Nguyen Minh Tuan	tuannmhe170202@fpt.edu.vn	\N	\N	\N	2024-10-08 16:50:11.031	2024-10-08 16:50:11.031	ACTIVE	24	CLIENT
632e6857-e108-4420-8e51-a154ab151b37	manager	yelotoh510@regishub.com	2024-10-24 21:24:13.838	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-10-24 21:23:54.737	2024-10-24 21:23:54.737	ACTIVE	45	TRANSLATE_MANAGER
926cfb71-d9f4-4d21-a87a-4191bd3b7c59	Lâm Phùng	lamphung213.phuc@gmail.com	2024-10-24 20:44:10.534	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-10-15 06:45:59.617	2024-10-15 06:45:59.617	ACTIVE	39	LINGUIST
94cdfcad-0e70-40fc-8cb9-bdd16636171e	Phùng Phúc Lâm	lampphe172382@fpt.edu.vn	2024-10-24 20:44:10.534	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-10-18 15:32:33.64	2024-10-18 15:32:33.64	ACTIVE	40	EVALUATE_MANAGER
baa8ffbe-6495-492c-91ee-e33e9d187e28	client2	kofemoj497@bflcafe.com	2024-11-29 07:27:34.671	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-11-29 07:27:18.075	2024-11-29 07:27:18.075	ACTIVE	47	CLIENT
2a48edc2-b2b3-4098-afd8-26a2a3238e66	lamphung	besago8363@nausard.com	\N	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-11-30 14:44:12.002	2024-11-30 14:44:12.002	DEACTIVATE	\N	CLIENT
438371ec-121c-4b11-8870-440158f7118b	Tuấn Nguyễn Minh	nguyenminhtuan271203@gmail.com	\N	\N	\N	2024-10-07 03:53:10.264	2024-10-07 03:53:10.264	ACTIVE	8	CLIENT
89d95db3-ebc5-4373-9d94-ae7596cd10eb	staff	dnsc1503@gmail.com	2024-10-21 14:52:06.685	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-10-21 14:50:29.668	2024-10-21 14:50:29.668	ACTIVE	42	STAFF
232fadf5-334b-494b-bc23-23ce9725f92d	admin	nguyenldhe176088@fpt.edu.vn	2024-10-07 20:34:11.126	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-10-07 20:31:15.857	2024-10-07 20:31:15.857	ACTIVE	22	ADMIN
53ff78f3-6179-4bc7-bbd0-ea4cbbfb6110	director	diunt.se03@gmail.com	2024-10-21 14:59:25.748	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-10-21 14:59:11.539	2024-10-21 14:59:11.539	ACTIVE	43	DIRECTOR
3a97abe8-35fe-421b-b0e0-5147d4b2c4b8	client	witelights213@gmail.com	2024-10-18 15:47:34.526	\N	B23E751164A8F0938B9489961B97D6D1C85C5AD11065FB12091B5D964362D49D	2024-10-18 15:45:35.819	2024-10-18 15:45:35.819	ACTIVE	41	CLIENT
c947a33b-2da8-4104-a0be-d8ed37fdf0e0	helen ng	nguyenthudiu03122003@gmail.com	\N	\N	\N	2024-10-28 14:15:01.983	2024-10-28 14:15:01.983	ACTIVE	46	CLIENT
5cfe0fea-44fc-45eb-b999-c43aa9590820	Trần Thị B	hiepnhhe171696@fpt.edu.vn	2024-10-13 21:28:57.046	\N	E1EE016E65C119ABE302B99107FBD225ADFD2EAD60512748F0709175292C7B8A	2024-10-13 21:24:49.033	2024-10-13 21:24:49.033	ACTIVE	26	LINGUIST
\.


--
-- TOC entry 4537 (class 0 OID 17622)
-- Dependencies: 232
-- Data for Name: work; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.work (work_id, order_id, created_date, due_date, service_code, work_name) FROM stdin;
655edb80-39bb-4679-b072-e80950b23325	600687a6-c54f-4254-b262-4651899cefae	2024-11-29 00:33:38.489702	2024-11-30 00:00:00	TL	ORD-00054
f81dc667-e094-4d28-af04-71bae9ba3880	600687a6-c54f-4254-b262-4651899cefae	2024-11-29 00:33:38.489923	2024-11-30 00:00:00	EV	ORD-00054
3009dad4-d59a-4b3c-adaa-31d46e709a09	e6182e64-4961-4fda-8f4f-f67c2290a204	2024-12-02 10:08:20.150808	2024-12-31 03:59:00	EV	ORD-00061
74bffa3c-62e5-4940-8344-aa42d0e3ad5f	e6182e64-4961-4fda-8f4f-f67c2290a204	2024-12-02 10:08:20.150745	2024-12-31 03:59:00	TL	ORD-00061
e4815fda-ae77-4d54-93c9-eb208aae7232	e6182e64-4961-4fda-8f4f-f67c2290a204	2024-12-02 10:08:20.150807	2024-12-31 03:59:00	ED	ORD-00061
4c9cdd8f-dab3-4f0d-bd45-c8d6c1d4fcab	c91826f4-3c9a-4c51-b082-9a1e7e3d8ea4	2024-12-02 13:58:43.967427	2025-01-01 00:00:00	TL	ORD-00099
c867db3c-4775-468e-9893-1e4dd080b157	c91826f4-3c9a-4c51-b082-9a1e7e3d8ea4	2024-12-02 13:58:43.967477	2025-01-01 00:00:00	ED	ORD-00099
af85556e-b3a2-406b-abd5-1e997311c339	3f29a214-f2dc-4b67-bc8c-d0997e74b5ab	2024-11-24 15:56:49.843373	2024-11-08 17:00:00	EV	ORD-00048
787c67d3-c5d4-4463-a038-62fc408b19c1	3bdafae2-d30b-4c5d-8462-942265626faa	2024-11-25 15:10:33.27132	2024-11-28 17:00:00	TL	ORD-00052
47a7d7f9-f710-4103-99ad-0d6a1dff038f	43c21b33-25d9-4f84-a9e8-2be04d139738	2024-11-25 16:52:13.357568	2024-11-29 17:00:00	EV	ORD-00050
fae190d3-1308-43d9-a1bf-5804bfec4fe0	7aa54ee1-4058-40fa-a7a8-404ed5a82c2d	2024-11-25 17:06:57.081516	2024-11-30 17:00:00	TL	ORD-00053
daea43de-041f-4073-a1ca-89a63e2d7204	c0d2e350-d57f-4026-a548-aa8e5fd7f77c	2024-11-25 19:25:20.912133	2024-11-29 17:00:00	EV	ORD-00055
8a6d8a19-6524-4d70-a144-342c6951850c	43c21b33-25d9-4f84-a9e8-2be04d139738	2024-11-25 19:37:25.616757	2024-11-29 17:00:00	EV	ORD-00050
3266af4b-298a-4d4d-8353-1303b7fa10b0	43c21b33-25d9-4f84-a9e8-2be04d139738	2024-11-29 00:16:47.485792	2024-11-30 00:00:00	EV	ORD-00050
\.


--
-- TOC entry 4538 (class 0 OID 17627)
-- Dependencies: 233
-- Data for Name: work_category; Type: TABLE DATA; Schema: public; Owner: avnadmin
--

COPY public.work_category (work_id, category_id) FROM stdin;
af85556e-b3a2-406b-abd5-1e997311c339	6
787c67d3-c5d4-4463-a038-62fc408b19c1	6
47a7d7f9-f710-4103-99ad-0d6a1dff038f	6
fae190d3-1308-43d9-a1bf-5804bfec4fe0	6
daea43de-041f-4073-a1ca-89a63e2d7204	6
8a6d8a19-6524-4d70-a144-342c6951850c	6
3266af4b-298a-4d4d-8353-1303b7fa10b0	6
655edb80-39bb-4679-b072-e80950b23325	6
f81dc667-e094-4d28-af04-71bae9ba3880	6
3009dad4-d59a-4b3c-adaa-31d46e709a09	6
74bffa3c-62e5-4940-8344-aa42d0e3ad5f	6
e4815fda-ae77-4d54-93c9-eb208aae7232	6
4c9cdd8f-dab3-4f0d-bd45-c8d6c1d4fcab	6
c867db3c-4775-468e-9893-1e4dd080b157	6
\.


--
-- TOC entry 4560 (class 0 OID 0)
-- Dependencies: 217
-- Name: category_category_id_seq; Type: SEQUENCE SET; Schema: public; Owner: avnadmin
--

SELECT pg_catalog.setval('public.category_category_id_seq', 8, true);


--
-- TOC entry 4561 (class 0 OID 0)
-- Dependencies: 235
-- Name: order_name_seq; Type: SEQUENCE SET; Schema: public; Owner: avnadmin
--

SELECT pg_catalog.setval('public.order_name_seq', 99, true);


--
-- TOC entry 4562 (class 0 OID 0)
-- Dependencies: 226
-- Name: refresh_token_token_id_seq; Type: SEQUENCE SET; Schema: public; Owner: avnadmin
--

SELECT pg_catalog.setval('public.refresh_token_token_id_seq', 47, true);


--
-- TOC entry 4324 (class 2606 OID 17631)
-- Name: role Role_pkey; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.role
    ADD CONSTRAINT "Role_pkey" PRIMARY KEY (role_id);


--
-- TOC entry 4334 (class 2606 OID 17633)
-- Name: user User_pkey; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT "User_pkey" PRIMARY KEY (id);


--
-- TOC entry 4345 (class 2606 OID 17838)
-- Name: assignee_job assignee_job_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.assignee_job
    ADD CONSTRAINT assignee_job_pk PRIMARY KEY (assignee_id, job_id);


--
-- TOC entry 4302 (class 2606 OID 17635)
-- Name: category category_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.category
    ADD CONSTRAINT category_pk PRIMARY KEY (category_id);


--
-- TOC entry 4349 (class 2606 OID 17911)
-- Name: client_transaction client_transaction_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.client_transaction
    ADD CONSTRAINT client_transaction_pk PRIMARY KEY (transaction_id, client_id);


--
-- TOC entry 4351 (class 2606 OID 17907)
-- Name: client_transaction client_transaction_unique; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.client_transaction
    ADD CONSTRAINT client_transaction_unique UNIQUE (orderid);


--
-- TOC entry 4343 (class 2606 OID 17781)
-- Name: discount discount_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.discount
    ADD CONSTRAINT discount_pk PRIMARY KEY (discount_id);


--
-- TOC entry 4308 (class 2606 OID 17637)
-- Name: issue_attachments issue_attachments_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.issue_attachments
    ADD CONSTRAINT issue_attachments_pk PRIMARY KEY (issue_id, attachment_url);


--
-- TOC entry 4304 (class 2606 OID 17639)
-- Name: issue issue_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.issue
    ADD CONSTRAINT issue_pk PRIMARY KEY (issue_id);


--
-- TOC entry 4306 (class 2606 OID 17884)
-- Name: issue issue_unique; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.issue
    ADD CONSTRAINT issue_unique UNIQUE (job_id);


--
-- TOC entry 4329 (class 2606 OID 17641)
-- Name: job job_pkey; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.job
    ADD CONSTRAINT job_pkey PRIMARY KEY (id);


--
-- TOC entry 4331 (class 2606 OID 17898)
-- Name: job job_unique; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.job
    ADD CONSTRAINT job_unique UNIQUE (deliverable_url);


--
-- TOC entry 4310 (class 2606 OID 17645)
-- Name: language language_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.language
    ADD CONSTRAINT language_pk PRIMARY KEY (language_id);


--
-- TOC entry 4312 (class 2606 OID 17647)
-- Name: order order_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public."order"
    ADD CONSTRAINT order_pk PRIMARY KEY (order_id);


--
-- TOC entry 4314 (class 2606 OID 17649)
-- Name: order_references order_references_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.order_references
    ADD CONSTRAINT order_references_pk PRIMARY KEY (order_id, reference_file_url);


--
-- TOC entry 4316 (class 2606 OID 17651)
-- Name: order_target_language order_target_language_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.order_target_language
    ADD CONSTRAINT order_target_language_pk PRIMARY KEY (target_language_id, order_id);


--
-- TOC entry 4318 (class 2606 OID 17655)
-- Name: rating rating_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.rating
    ADD CONSTRAINT rating_pk PRIMARY KEY (rating_id);


--
-- TOC entry 4347 (class 2606 OID 17932)
-- Name: receipt receipt_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.receipt
    ADD CONSTRAINT receipt_pk PRIMARY KEY (receipt_id);


--
-- TOC entry 4320 (class 2606 OID 17657)
-- Name: refresh_token refreshtoken_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.refresh_token
    ADD CONSTRAINT refreshtoken_pk PRIMARY KEY (token_id);


--
-- TOC entry 4322 (class 2606 OID 17659)
-- Name: revelancy revelancy_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.revelancy
    ADD CONSTRAINT revelancy_pk PRIMARY KEY (revelancy_id);


--
-- TOC entry 4326 (class 2606 OID 17665)
-- Name: services services_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.services
    ADD CONSTRAINT services_pk PRIMARY KEY (service_code);


--
-- TOC entry 4337 (class 2606 OID 17667)
-- Name: user user_unique; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_unique UNIQUE (token_id);


--
-- TOC entry 4341 (class 2606 OID 17669)
-- Name: work_category work_category_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.work_category
    ADD CONSTRAINT work_category_pk PRIMARY KEY (work_id, category_id);


--
-- TOC entry 4339 (class 2606 OID 17671)
-- Name: work work_pk; Type: CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.work
    ADD CONSTRAINT work_pk PRIMARY KEY (work_id);


--
-- TOC entry 4332 (class 1259 OID 17672)
-- Name: User_email_key; Type: INDEX; Schema: public; Owner: avnadmin
--

CREATE UNIQUE INDEX "User_email_key" ON public."user" USING btree (email);


--
-- TOC entry 4327 (class 1259 OID 17908)
-- Name: job_document_url_idx; Type: INDEX; Schema: public; Owner: avnadmin
--

CREATE INDEX job_document_url_idx ON public.job USING btree (document_url);


--
-- TOC entry 4335 (class 1259 OID 17673)
-- Name: user_login_idx; Type: INDEX; Schema: public; Owner: avnadmin
--

CREATE INDEX user_login_idx ON public."user" USING btree (email, password);


--
-- TOC entry 4377 (class 2620 OID 17833)
-- Name: order trigger_set_order_name; Type: TRIGGER; Schema: public; Owner: avnadmin
--

CREATE TRIGGER trigger_set_order_name BEFORE INSERT ON public."order" FOR EACH ROW EXECUTE FUNCTION public.set_order_name();


--
-- TOC entry 4374 (class 2606 OID 17839)
-- Name: assignee_job assignee_job_job_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.assignee_job
    ADD CONSTRAINT assignee_job_job_fk FOREIGN KEY (job_id) REFERENCES public.job(id) ON DELETE CASCADE;


--
-- TOC entry 4375 (class 2606 OID 17844)
-- Name: assignee_job assignee_job_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.assignee_job
    ADD CONSTRAINT assignee_job_user_fk FOREIGN KEY (assignee_id) REFERENCES public."user"(id) ON DELETE CASCADE;


--
-- TOC entry 4352 (class 2606 OID 17674)
-- Name: issue issue_assign_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.issue
    ADD CONSTRAINT issue_assign_fk FOREIGN KEY (assignee_id) REFERENCES public."user"(id) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4355 (class 2606 OID 17679)
-- Name: issue_attachments issue_attachments_issue_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.issue_attachments
    ADD CONSTRAINT issue_attachments_issue_fk FOREIGN KEY (issue_id) REFERENCES public.issue(issue_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4353 (class 2606 OID 17890)
-- Name: issue issue_job_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.issue
    ADD CONSTRAINT issue_job_fk FOREIGN KEY (job_id) REFERENCES public.job(id) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4354 (class 2606 OID 17689)
-- Name: issue issue_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.issue
    ADD CONSTRAINT issue_user_fk FOREIGN KEY (client_id) REFERENCES public."user"(id) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4356 (class 2606 OID 17782)
-- Name: order order_discount_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public."order"
    ADD CONSTRAINT order_discount_fk FOREIGN KEY (discount_id) REFERENCES public.discount(discount_id) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4357 (class 2606 OID 17694)
-- Name: order order_language_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public."order"
    ADD CONSTRAINT order_language_fk FOREIGN KEY (source_language_id) REFERENCES public.language(language_id) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4359 (class 2606 OID 17699)
-- Name: order_references order_references_order_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.order_references
    ADD CONSTRAINT order_references_order_fk FOREIGN KEY (order_id) REFERENCES public."order"(order_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4360 (class 2606 OID 17704)
-- Name: order_target_language order_target_language_language_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.order_target_language
    ADD CONSTRAINT order_target_language_language_fk FOREIGN KEY (target_language_id) REFERENCES public.language(language_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4361 (class 2606 OID 17709)
-- Name: order_target_language order_target_language_order_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.order_target_language
    ADD CONSTRAINT order_target_language_order_fk FOREIGN KEY (order_id) REFERENCES public."order"(order_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4358 (class 2606 OID 17714)
-- Name: order order_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public."order"
    ADD CONSTRAINT order_user_fk FOREIGN KEY (client_id) REFERENCES public."user"(id) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4362 (class 2606 OID 17849)
-- Name: rating rating_order_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.rating
    ADD CONSTRAINT rating_order_fk FOREIGN KEY (order_id) REFERENCES public."order"(order_id) ON DELETE CASCADE;


--
-- TOC entry 4376 (class 2606 OID 17871)
-- Name: receipt receipt_order_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.receipt
    ADD CONSTRAINT receipt_order_fk FOREIGN KEY (order_id) REFERENCES public."order"(order_id);


--
-- TOC entry 4363 (class 2606 OID 17755)
-- Name: revelancy revelancy_category_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.revelancy
    ADD CONSTRAINT revelancy_category_fk FOREIGN KEY (category_id) REFERENCES public.category(category_id) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4364 (class 2606 OID 17765)
-- Name: revelancy revelancy_language_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.revelancy
    ADD CONSTRAINT revelancy_language_fk FOREIGN KEY (source_language_id) REFERENCES public.language(language_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4365 (class 2606 OID 17770)
-- Name: revelancy revelancy_language_fk_1; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.revelancy
    ADD CONSTRAINT revelancy_language_fk_1 FOREIGN KEY (target_language_id) REFERENCES public.language(language_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4366 (class 2606 OID 17760)
-- Name: revelancy revelancy_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.revelancy
    ADD CONSTRAINT revelancy_user_fk FOREIGN KEY (user_id) REFERENCES public."user"(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4367 (class 2606 OID 17719)
-- Name: job task_work_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.job
    ADD CONSTRAINT task_work_fk FOREIGN KEY (work_id) REFERENCES public.work(work_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4368 (class 2606 OID 17724)
-- Name: user user_refresh_token_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_refresh_token_fk FOREIGN KEY (token_id) REFERENCES public.refresh_token(token_id) ON DELETE CASCADE;


--
-- TOC entry 4369 (class 2606 OID 17729)
-- Name: user user_role_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_role_fk FOREIGN KEY (role_code) REFERENCES public.role(role_id) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4372 (class 2606 OID 17734)
-- Name: work_category work_category_category_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.work_category
    ADD CONSTRAINT work_category_category_fk FOREIGN KEY (category_id) REFERENCES public.category(category_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4373 (class 2606 OID 17739)
-- Name: work_category work_category_work_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.work_category
    ADD CONSTRAINT work_category_work_fk FOREIGN KEY (work_id) REFERENCES public.work(work_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4370 (class 2606 OID 17744)
-- Name: work work_order_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.work
    ADD CONSTRAINT work_order_fk FOREIGN KEY (order_id) REFERENCES public."order"(order_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- TOC entry 4371 (class 2606 OID 17749)
-- Name: work work_services_fk; Type: FK CONSTRAINT; Schema: public; Owner: avnadmin
--

ALTER TABLE ONLY public.work
    ADD CONSTRAINT work_services_fk FOREIGN KEY (service_code) REFERENCES public.services(service_code) ON UPDATE SET NULL ON DELETE SET NULL;


--
-- TOC entry 4549 (class 0 OID 0)
-- Dependencies: 5
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: avnadmin
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;


-- Completed on 2024-12-02 22:29:10

--
-- PostgreSQL database dump complete
--

