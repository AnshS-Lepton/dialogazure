delete from LABEL_COLUMN_SETTINGS where column_name='ownership_type' ;
delete from LABEL_COLUMN_SETTINGS where column_name='third_party_vendor_id';


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Cable')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Cable')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('ADB')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('ADB')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');



insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('BDB')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('BDB')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('FDB')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('FDB')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('FMS')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('FMS')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Handhole')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Handhole')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');

insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('htb')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('htb')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');

insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Manhole')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Manhole')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Cabinet')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Cabinet')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');

insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('ONT')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('ONT')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');

insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('POD')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('POD')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');

insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Pole')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Pole')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('SPLICECLOSURE')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('SPLICECLOSURE')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Tower')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Tower')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Trench')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Trench')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('WallMount')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('WallMount')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Antenna')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Antenna')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Duct')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Duct')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('PATCHPANEL')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('PATCHPANEL')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Microduct')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('Microduct')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('SECTOR')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('SECTOR')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');


insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('CDB')),'ownership_type','ownership type',0,true,'ownership_type');
insert into LABEL_COLUMN_SETTINGS (layer_id,column_name,display_column_name,column_sequence,is_active,table_column_name)
values((select layer_id from layer_details where upper(layer_name)=upper('CDB')),'third_party_vendor_id','third party vendor id',0,true,'third_party_vendor_id');




