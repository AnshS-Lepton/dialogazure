INSERT INTO public.global_settings ("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES ('isAADEnabled', true, 'Azure Active Directory Login Enabled for Both', 'both', false, 'Boolean', 1.0, 1.0, 1, Now(), NULL, NULL, true, true, false, NULL, NULL);

INSERT INTO
public.global_settings(key, value, description, type, is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES ('product_lisence_web', 'pLQLod1nty5iQ3XsnCJ361/UKHUFV5Ogb7kMpuaf8yMBN4ZoLlrowaoqrQzLchDV', 'Product Lisence Validity Details', 'Web', false, 'string', 1, 30, 5, CURRENT_TIMESTAMP, 5, CURRENT_TIMESTAMP, true, true, false, null, null);

INSERT INTO
public.global_settings(key, value, description, type, is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic)
VALUES ('product_lisence_mobile', 'pLQLod1nty5iQ3XsnCJ361/UKHUFV5Ogb7kMpuaf8yMBN4ZoLlrowaoqrQzLchDV', 'Product Lisence Validity Details', 'Mobile', false, 'string', 1, 30, 5, CURRENT_TIMESTAMP, 5, CURRENT_TIMESTAMP, true, true, false, null, null);

update global_settings 
set value='W56FjZgNMRDcMR+GX9IKVAbt8hL26yz8bCT0Da7WgWn2FUY2AiLXSBAwJWh/+gQXN+lbRdiRgnLJ2Jp15lDQPJdp8j8QcigJy0fni69FfCskDp0vFx8fUSV8dUuBAS3M+q/llOgKVfRC3raC5AAV4UoAp7ME6x6FjHFTye09quSoKgtDtnAdhoNEzZpIWfdax2oK9cjf3jOHiTi5UDDZU9OySt1aYy1enQNBhnVbtYIIplkgMN7fEkQLx9gViU+mThJjLTHHPGtzaOQkh24kH6pwY6ddthkMo8GFOyUXbfESVFjbUukJKw24K4ddLDOtmIBDK8V1XAXlRK5CGrlInQ==' 
where key ilike 'product_lisence_%';

INSERT INTO public.global_settings ("key", value, description, "type", is_edit_allowed, data_type, min_value, max_value, created_by, created_on, modified_by, modified_on, is_mobile_key, is_web_key, is_edit_allowed_for_sa, min_value_logic, max_value_logic) 
VALUES('isLicenseExpAlrtAllowed', 'true', 'License Expiry Alert message', 'Web', false, 'bool', 0.0, 0.0, 5, '2024-09-12 13:05:28.536', 5, '2024-09-12 13:05:28.536', true, true, false, NULL, NULL);

INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('en', 'SI_GBL_GBL_NET_FRM_442', 'Hello, Your license will expire in <PExpTDays> days. Please renew at the earliest to avoid any service disruption.', true, true, 'English', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2024-09-11 16:38:29.452', 1, false, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('hi', 'SI_GBL_GBL_NET_FRM_442', 'हैलो, आपका लाइसेंस <PExpTDays> दिनों में समाप्त हो जाएगा। कृपया किसी भी सेवा व्यवधान से बचने के लिए जल्द से जल्द नवीनीकरण करें।', true, true, 'Hindi', 'Smart Inventory_Global_Global_Dot Net_', NULL, NULL, '2024-09-11 16:38:29.452', 1, false, false);



INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('de-DE', 'GBL_GBL_GBL_GBL_GBL_034', 'Einloggen auf', false, true, 'German', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 14:50:19.212', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('en', 'GBL_GBL_GBL_GBL_GBL_034', 'Login to', true, true, 'English', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 14:50:19.212', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('fr', 'GBL_GBL_GBL_GBL_GBL_034', 'Connectez-vous à', false, true, 'French', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 14:50:19.212', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('hi', 'GBL_GBL_GBL_GBL_GBL_034', 'लॉग इन करें', false, true, 'Hindi', 'Global_Global_Global_Global_', 287, '2024-09-19 14:52:04.843', '2024-09-19 14:50:19.212', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('ja-JP', 'GBL_GBL_GBL_GBL_GBL_034', 'ログイン', false, true, 'Japanese', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 14:50:19.212', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('ru-RU', 'GBL_GBL_GBL_GBL_GBL_034', 'Войти в', false, true, 'Russian', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 14:50:19.212', 287, true, false);


INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('de-DE', 'SI_OSP_GBL_JQ_GBL_117', 'Indem Sie fortfahren, erklären Sie sich mit unseren Servicebedingungen und der Datenschutz- und Rechtsrichtlinie einverstanden.', false, true, 'German', 'Smart Inventory_Osp_Global_J Query_', NULL, NULL, '2024-09-13 17:25:49.103', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('en', 'SI_OSP_GBL_JQ_GBL_117', 'By continuing, you agree to our Terms of Services and Privacy & Legal Policy.', true, true, 'English', 'Smart Inventory_Osp_Global_J Query_', NULL, NULL, '2024-09-13 17:25:49.103', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('fr', 'SI_OSP_GBL_JQ_GBL_117', 'En continuant, vous acceptez nos conditions d’utilisation et notre politique de confidentialité et juridique.', false, true, 'French', 'Smart Inventory_Osp_Global_J Query_', 287, '2024-09-16 10:04:54.407', '2024-09-13 17:25:49.103', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('hi', 'SI_OSP_GBL_JQ_GBL_117', 'जारी रखते हुए, आप हमारी सेवाओं की शर्तों और गोपनीयता और कानूनी नीति से सहमत होते हैं।', false, true, 'Hindi', 'Smart Inventory_Osp_Global_J Query_', 287, '2024-09-16 10:01:06.641', '2024-09-13 17:25:49.103', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('ja-JP', 'SI_OSP_GBL_JQ_GBL_117', '続行すると、当社の利用規約およびプライバシーおよび法的ポリシーに同意したことになります。', false, true, 'Japanese', 'Smart Inventory_Osp_Global_J Query_', NULL, NULL, '2024-09-13 17:25:49.103', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('ru-RU', 'SI_OSP_GBL_JQ_GBL_117', 'Продолжая, вы соглашаетесь с нашими Условиями предоставления услуг и Политикой конфиденциальности и права.', false, true, 'Russian', 'Smart Inventory_Osp_Global_J Query_', NULL, NULL, '2024-09-13 17:25:49.103', 287, true, false);

INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('de-DE', 'GBL_GBL_GBL_GBL_GBL_033', 'Benutzername', false, true, 'German', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 10:51:24.489', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('en', 'GBL_GBL_GBL_GBL_GBL_033', 'Username', true, true, 'English', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 10:51:24.489', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('fr', 'GBL_GBL_GBL_GBL_GBL_033', 'Nom d''utilisateur', false, true, 'French', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 10:51:24.489', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('hi', 'GBL_GBL_GBL_GBL_GBL_033', 'उपयोगकर्ता नाम', false, true, 'Hindi', 'Global_Global_Global_Global_', 287, '2024-09-19 11:02:06.241', '2024-09-19 10:51:24.489', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('ja-JP', 'GBL_GBL_GBL_GBL_GBL_033', 'ユーザー名', false, true, 'Japanese', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 10:51:24.489', 287, true, false);
INSERT INTO public.res_resources (culture, "key", value, is_default_lang, is_visible, "language", description, modified_by, modified_on, created_on, created_by, is_jquery_used, is_mobile_key) 
VALUES('ru-RU', 'GBL_GBL_GBL_GBL_GBL_033', 'Имя пользователя', false, true, 'Russian', 'Global_Global_Global_Global_', NULL, NULL, '2024-09-19 10:51:24.489', 287, true, false);