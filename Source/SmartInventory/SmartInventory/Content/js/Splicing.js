var splicing = function () {
    var app = this;
    var gMapObj = {};
    this.ParentModel = 'PARENT';
    this.isSwapped = false;
    this.mainJSPlumb = null;
    this.gMapObj = {};
    this.longitude = 0;
    this.latitude = 0;
    this.connectionObjects = [];
    this.currentConnections = [];
    this.isAutoDeleteConnection = false;
    this.isMoved = false;
    this.entityInfo = null;
    this.upStream = null;
    this.downStream = null;
    this.isCPEWindow = false;
    this.filteredConnections = [];
    this.leftRangeCon = [];
    this.filteredConnObjects = [];
    this.endPointObject = null;
    this.coreColor = '';
    this.statusId = 0;
    this.PortComment = '';
    this.apptestvalue = false;
    var oms = null;
    var color = '#00ba8a';
    this.DE = {
        "SourceCable": "#ddlSourceCable",
        "DestinationCable": "#ddlDestinationCable",
        "ConnectingEntity": "#ddlConnectingEntity",
        "SplicingDiv": "#SplicingDiv",
        "splicingMain": "#SplicingDiv .splicingMain",
        "btnopticalExport": "#btnopticalExport",
        "divLeftCable": "#divLeftCable",
        "divRightCable": "#divRightCable",
        "lblLeftCable": "#lblLeftCable",
        "lblRightCable": "#lblRightCable",
        "ddlCPEEntity": "#ddlCPEEntity",
        "ddlcustomer": "#ddlCustomer",
        "leftStartRange": "#leftStartRange",
        "leftEndRange": "#leftEndRange",
        "rightStartRange": "#rightStartRange",
        "rightEndRange": "#rightEndRange",
        "splicingInfowindow": "#splicingInfowindow",
        "equipmentid": "#equipment_id",
        "ddlCore": "#ddlCore",
        "btnCPFexport": "#btncpfExport",
        "btnCPFexportToKML": "#btnCPFexportToKML",
        "btnCPFClear": "#btncpfClear",
        "btnShowRoute": "#btnShowRoute",
        "entityid": "#objFilterAttributes_entityid",
        "port_no": "#objFilterAttributes_port_no",
        "entity_type": "#objFilterAttributes_entity_type",
        "showOnMap": "#btnShowOnMap",
        "chkSpliceTerminatedCablesOnly": "#chkSpliceTerminatedCablesOnly",
        "btnConnectedCustomerExport": "#btnConnectedCustomerExport",
        "btnSchView": "#btnSchView",
        "ddlSpliceTray": "#ddl_SpliceTray"
    }
    this.initApp = function () {
        this.bindEvents();
    }
    this.bindEvents = function () {
        $(app.DE.equipmentid).autocomplete({
            source: function (request, response) {
                var res = ajaxReq('Splicing/GetEquipmentSearchResult', { SearchText: $.trim(request.term) }, true, function (data) {
                    if (data.length == 0) {
                        var result = [
                            {
                                label: MultilingualKey.SI_GBL_GBL_JQ_GBL_001,
                                entity_id: 0
                            }
                        ];
                        response(result);
                    }
                    else {
                        response($.map(data, function (item) {
                            return {
                                label: item.display_name, value: item.network_id, entity_type: item.entity_type, entity_id: item.system_id
                            }//
                        }))
                    }
                }, true, false);
            },
            minLength: 3,
            select: function (event, ui) {
                $('#dvViewConnectionPathFinder,#dvSchematicViewContainer').html('');
                $('#divNoRecordExist').show();
                var label = ui.item.value;
                if (ui.item.entity_id == 0) {
                    event.preventDefault();
                }
                else {
                    event.preventDefault();
                    $(app.DE.equipmentid).val(ui.item.label);
                    $(app.DE.entity_type).val(ui.item.entity_type);
                    $(app.DE.entityid).val(ui.item.entity_id);
                    app.BindEquipementPort(ui.item.entity_id, ui.item.entity_type);//
                }

            }

        });
        $(app.DE.btnShowRoute).on("click", function () {
            var srchrtxt = $(app.DE.equipmentid).val();
            if (srchrtxt == '') {
                $(app.DE.equipmentid).addClass('input-validation-error');
                return false;
            }
            else {
                $(app.DE.equipmentid).removeClass('input-validation-error');
            }
            var selectedValue = $('#ddlCore option:selected').val();
            if (selectedValue == '0') {
                $(app.DE.port_no).val('0');
                $("#ddlCore_chosen").addClass('input-validation-error');
                return false;
            }
            else {
                $(app.DE.port_no).val(selectedValue);
                $("#ddlCore_chosen").removeClass('input-validation-error');
            }
        });
        $(app.DE.showOnMap).on("click", function () {
            app.showPath();
        });
        $(app.DE.btnCPFexport).on("click", function () {
             
            app.downloadCPE();
            $("#PathTrack .dropfiles").trigger("click");
        });

        $(app.DE.btnConnectedCustomerExport).on("click", function () {
            app.downloadConnectedCustomerDetails();
            $("#PathTrack .dropfiles").trigger("click");
        })
        $(app.DE.btnCPFexportToKML).on("click", function () {
            app.downloadCPEKML();
            $("#PathTrack .dropfiles").trigger("click");
        });
        $(app.DE.btnCPFClear).on("click", function () {

            if (isp != null) {
                $('#btnShowOnMap').hide();
            }
            app.clearCFPGrid();
        });
        $(app.DE.btnopticalExport).on("click", function () {
            app.downloadOpticalLink();
        });
        $(app.DE.btnSchView).on("click", function () {
            app.showSchematicView();
        });
    }
    this.cancelSplice = function () {
        $(app.DE.SplicingDiv).hide();
        if (app.gMapObj.pointObj != null) { app.gMapObj.pointObj.setMap(null); app.gMapObj.pointObj = null; }
        if (app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
        if (app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; }
        // si.clearTPRelatedObjects();
        $('#ModalPopUp div').removeClass('modal-xxl');
    }
    this.spliceHere = function (dvObj) {
        debugger;
        si.clearTPRelatedObjects();
        $('#InfoDiv').hide();
        si.removeEventListnrs('click');
        //si.makeTempOrange(dvObj, 'spliceToolClick');
        if (si.map.ClickAction && si.map.ClickAction == "Splicing") {
            si.map.setOptions({ draggableCursor: '' });
            si.map.ClickAction = '';
            $(dvObj).removeClass('activeToolBar');
        }
        else {
            si.map.setOptions({ draggableCursor: 'crosshair' });
            google.maps.event.addListener(si.map, 'click', function (e) {
                app.ClearSplicingRelatedObjs();
                app.initiateSplicing(e.latLng);
            });
            si.map.ClickAction = "Splicing";
            $(dvObj).addClass('activeToolBar');
        }
    }
    this.initiateSplicing = function (e) {
        si.collapseRemove();
        var _zoom = si.map.getZoom();
        if (_zoom > 18) {
            app.longitude = e.lng();
            app.latitude = e.lat();
            si.showTempBufferforInfo(e);
            var radius = getMeterDistanceFromZoom(_zoom);
            var dataURL = 'Splicing/Index';
            ajaxReq('Splicing/Index', { latitude: e.lat(), longitude: e.lng(), bufferRadius: radius }, true, function (resp) {
                $(app.DE.splicingMain).html(resp);
                $(app.DE.SplicingDiv).show();
            }, false, true);
        }
        else {
            //Splicing tools works on 19 or greater zoom level.//Your current map zoom is//Do you want to zoom?
            var func = function () { si.map.setCenter(e); si.map.setZoom(19); };
             
            confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_FRM_020, _zoom)), func);
        }
    }
    this.splicingWindow = function () {
        //var connectorType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        let _isMiddlewareEntity = ($(app.DE.ConnectingEntity + ' :selected').attr('data-is-middleware-entity').toLowerCase() == 'true');
        //if (connectorType == 'ODF' || connectorType == 'FMS')
        if (_isMiddlewareEntity) {
            app.FMSSplicingWindow();
        } else { app.cableToCableSplicing(); }
    }
    this.cableToCableSplicing = function () {
        SetThroughConnValue();
        let _isMiddlewareEntity = ($(app.DE.ConnectingEntity + ' :selected').attr('data-is-middleware-entity').toLowerCase() == 'true');
        app.isCPEWindow = false;
        var connectors = [];
        if ($(app.DE.ConnectingEntity).val() == '0') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_013);//Please select connecting element!
            return false;
        }
        if ($(app.DE.SourceCable).val() == '0' && $(app.DE.DestinationCable).val() == '0') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_020);//Please select left or right cable!
            return false;
        }
        if ($(app.DE.ConnectingEntity + ' :selected').attr('data-total-port') == '' || $(app.DE.ConnectingEntity + ' :selected').attr('data-total-port') && parseInt($(app.DE.ConnectingEntity + ' :selected').attr('data-total-port')) == 0) {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_021);//Zero port does not allowed!
            return false;
        }
        connectors.push({ system_id: parseInt($(app.DE.ConnectingEntity).val()), entity_type: $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type'), is_virtual: $(app.DE.ConnectingEntity + ' :selected').attr('data-is-virtual'), is_virtual_entity: $(app.DE.ConnectingEntity + ' :selected').attr('data-is-virtual-entity') });
        var leftStartPoint = JSON.parse($(app.DE.SourceCable + ' :selected').val() != '0' ? $(app.DE.SourceCable + ' :selected').attr('data-is-start-point').toLowerCase() : 'false');
        var rightStartPoint = JSON.parse($(app.DE.DestinationCable + ' :selected').val() != '0' ? $(app.DE.DestinationCable + ' :selected').attr('data-is-start-point').toLowerCase() : 'false');
        var dataURL = 'Splicing/CableToCable';
        var titleText = 'Cable to Cable';
        if ($(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') == 'HTB') { titleText = 'Cable to HTB'; }
        var objSplicingInput = {
            source_system_id: parseInt($(app.DE.SourceCable).val()),
            source_entity_type: 'Cable',
            destination_system_id: parseInt($(app.DE.DestinationCable).val()),
            destination_entity_type: 'Cable',
            //connector_system_id: parseInt($(app.DE.ConnectingEntity).val()),
            connector_entity_type: $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type'),
            listConnector: connectors,
            is_source_start_point: (leftStartPoint ? leftStartPoint : false),
            is_destination_start_point: (rightStartPoint ? rightStartPoint : false),
            splicing_type: 'CABLE2CABLE',
            is_middleware_entity: _isMiddlewareEntity
        };
        popup.LoadModalDialog(app.ParentModel, dataURL, objSplicingInput, titleText, 'modal-xxxl', function () {
            $('#dvProgressSplice').show();
            setTimeout(app.InitilizeJsPlumb, 300);
            app.currentConnections = [];
            $(app.DE.leftStartRange).val(1);
            $(app.DE.leftEndRange).val($('#dvLeftCable .leftFiber').length);
            $(app.DE.rightStartRange).val(1);
            $(app.DE.rightEndRange).val($('#dvRightCable .rightFiber').length);
            $(app.DE.leftStartRange).on('keyup', function (e) {
                $(this).removeClass('NotValid').addClass('valid');
            });
            $(app.DE.leftEndRange).on('keyup', function (e) {
                $(this).removeClass('NotValid').addClass('valid');
            });
            $(app.DE.rightStartRange).on('keyup', function (e) {
                $(this).removeClass('NotValid').addClass('valid');
            });
            $(app.DE.rightEndRange).on('keyup', function (e) {
                $(this).removeClass('NotValid').addClass('valid');
            });
            app.bindCoreInfoWindow();
        });
    }
    this.FMSSplicingWindow = function () {
        let _isMiddlewareEntity = ($(app.DE.ConnectingEntity + ' :selected').attr('data-is-middleware-entity').toLowerCase() == 'true');
        app.isCPEWindow = false;
        var connectors = [];
        if ($(app.DE.ConnectingEntity).val() == '0') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_013);//Please select connecting element!
            return false;
        }
        if ($(app.DE.DestinationCable).val() == '0') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_022);//Please select cable!
            return false;
        }
        var entityName = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        connectors.push({ system_id: parseInt($(app.DE.ConnectingEntity).val()), entity_type: $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') });
        var isCableAEnd = JSON.parse($(app.DE.DestinationCable + ' :selected').val() != '0' ? $(app.DE.DestinationCable + ' :selected').attr('data-is-start-point').toLowerCase() : 'false');
        var titleText = 'ODF/FMS';
        //if (entityName != 'FMS') {
        //    titleText = entityName;
        //}
        titleText = titleText + " to Cable";
        var dataURL = 'Splicing/ODFToCable';
        // var titleText = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') + ' to Cable';
        var objSplicingInput = {
            source_system_id: 0,
            source_entity_type: '',
            destination_system_id: parseInt($(app.DE.DestinationCable).val()),
            destination_entity_type: 'Cable',
            listConnector: connectors,
            //connector_system_id: parseInt($(app.DE.ConnectingEntity).val()),
            connector_entity_type: $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type'),
            is_source_start_point: false,
            is_destination_start_point: (isCableAEnd ? isCableAEnd : false),
            is_middleware_entity: _isMiddlewareEntity
        };
        popup.LoadModalDialog(app.ParentModel, dataURL, objSplicingInput, 'ODF to Cable', 'modal-lg', function () {
            $('#dvProgressSplice').show();
            setTimeout(app.InitilizeJsPlumb, 300);
            app.currentConnections = [];
            $('#dvProgressSplice').hide();
            $(app.DE.leftStartRange).val(1);
            $(app.DE.leftEndRange).val($('.DeviceView .leftFiber .innerFiber').length);
            $(app.DE.rightStartRange).val(1);
            $(app.DE.rightEndRange).val($('#dvRightCable .rightFiber').length);
            $(app.DE.leftStartRange).on('keyup', function (e) {
                $(this).removeClass('NotValid').addClass('valid');
            });
            $(app.DE.leftEndRange).on('keyup', function (e) {
                $(this).removeClass('NotValid').addClass('valid');
            });
            $(app.DE.rightStartRange).on('keyup', function (e) {
                $(this).removeClass('NotValid').addClass('valid');
            });
            $(app.DE.rightEndRange).on('keyup', function (e) {
                $(this).removeClass('NotValid').addClass('valid');
            });
            app.bindCoreInfoWindow();
        });
    }
    this.CPESplicingWindow = function () {
        app.isCPEWindow = true;
        var connectors = [];
        if ($(app.DE.ddlCPEEntity).val() == '' || $(app.DE.ddlCPEEntity).val() == null) {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_028);//Please select CPE!
            return false;
        }
        if ($(app.DE.ddlcustomer).val() == null) {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_029);//Please select customer!
            return false;
        }
        $.each($('#ddlCPEEntity :selected'), function (index, item) {
            connectors.push({ system_id: parseInt($(item).val()), entity_type: $(item).data().entityType });
        })
        var index = connectors.findIndex(function (m) { return m.entity_type == 'ONT' });
        if (index < 0) {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_030);//Please select ONT!
            return false;
        }
        var dataURL = 'Splicing/CPEToCustomer';
        var titleText = 'Manual Splicing';
        var objSplicingInput = {
            source_system_id: 0,
            source_entity_type: '',
            destination_system_id: 0,
            destination_entity_type: '',
            //connector_system_id: $(app.DE.ddlCPEEntity).val().join(','),
            //connector_entity_type: entityTypes.join(','), //$(app.DE.ddlCPEEntity + ' :selected').attr('data-entity-type'),
            listConnector: connectors,
            is_source_start_point: false,
            is_destination_start_point: false,
            customer_ids: $(app.DE.ddlcustomer).val().join(',')
        };
        popup.LoadModalDialog(app.ParentModel, dataURL, objSplicingInput, titleText, 'modal-lg', function () {
            $('#dvProgressSplice').show();
            setTimeout(app.InitilizeJsPlumb, 300);
            $('#dvProgressSplice').hide();
            app.currentConnections = [];
            app.bindCoreInfoWindow();
        });
    }
    this.InitilizeJsPlumb = function () {
        var defaultOptions = app.getDefaultOptions();
        jsPlumb.importDefaults(defaultOptions);
        app.addEndPoint();
        app.getConnections();
        if ($('#hdnIsSplicingEditEnabled').val() == 'False') { $('._jsPlumb_endpoint').remove(); }
        $('.linkNumber[data-link-id!="0"]').on("click", function () {
            var _networkId = $(this).attr('data-network-id');
            var _entityType = $(this).attr('data-entity-type');
            var _systemId = parseInt($(this).attr('data-system-id'));
            var _portNo = parseInt($(this).attr('data-port-no'));
            app.redirectToLinkInfo(null, _networkId, _entityType, _systemId, _portNo, false)
        });
    }
    this.addEndPoint = function () {
        try {
            var jspl1 = jsPlumb.getInstance();
            jspl1.addEndpoint($('.src[data-status-id="1"],.src[data-status-id="2"]'), { anchor: "RightMiddle" }, app.getEndPointOptions());
            jspl1.addEndpoint($('.trg[data-status-id="1"],.trg[data-status-id="2"]'), { anchor: "LeftMiddle" }, app.getEndPointOptions());
            app.bindjsPlumbEvent(jspl1);
            app.endPointObject = jspl1;
        } catch (err) {

        }
    }
    this.getEndPointOptions = function () {
        try {
            var options = {};
            options.endpoint = ["Dot", { radius: 3.5 }];
            options.paintStyle = { fillStyle: "black" };
            options.isTarget = true;
            options.isSource = true;
            options.scope = "green dot";
            options.connectorStyle = { strokeStyle: '#00ba8a', lineWidth: 1 };
            options.connector = ["Bezier", { curviness: 100 }];
            options.maxConnections = 1;
            return options;
        } catch (err) {

        }
    }
    this.spliceAll = function () {
        if (app.isSplicingRangeValid()) {
            var leftStartRange = parseInt($(app.DE.leftStartRange).val());
            var leftEndRange = parseInt($(app.DE.leftEndRange).val());
            var rightStartRange = parseInt($(app.DE.rightStartRange).val());
            var rightEndRange = parseInt($(app.DE.rightEndRange).val());
            var allConnections = [];
            var connectorType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
            if (connectorType == 'ODF' || connectorType == 'FMS' || connectorType == 'HTB') {
                if ($(app.DE.DestinationCable).val() == '0') {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_022);//Please select cable!
                    return false;
                }
                for (var i = leftStartRange; i <= leftEndRange; i++) {
                    if ($('.DeviceView .leftFiber .src[data-is-connected="0"][data-status-id="1"][data-port-no="' + i + '"]').length > 0
                        && $('#dvRightCable .rightFiber .trg[data-is-connected="0"][data-status-id="1"][data-port-no="' + rightStartRange + '"]').length > 0) {
                        var sourceId = $('.DeviceView .leftFiber .src[data-is-connected="0"][data-status-id="1"][data-port-no="' + i + '"]').attr('id');
                        var targetId = $('#dvRightCable .rightFiber .trg[data-is-connected="0"][data-status-id="1"][data-port-no="' + rightStartRange + '"]').attr('id');
                        var connectionRequiest = app.getConnectionsData(sourceId, targetId, false);
                        allConnections.push(connectionRequiest);
                    }
                    if (rightStartRange == rightEndRange) { break; }
                    rightStartRange++;
                }
                //$.each($('.DeviceView .leftFiber .src[data-is-connected="0"]'), function () {
                //    var sourceId = $(this).attr('id');
                //    var sourcePortNo = $(this).attr('data-port-no');
                //    var targetId = 'Right_' + $(app.DE.DestinationCable).val() + '_CABLE_' + sourcePortNo;
                //    if ($('#' + targetId).length > 0) {
                //        if ($('#' + targetId).hasClass('trg') && $('#' + targetId).attr('data-is-connected') == '0') { allConnections.push(app.getConnectionsData(sourceId, targetId)); }
                //    } else { return false; }
                //});
                if (allConnections.length > 0) {
                    if (app.validateConnections(allConnections)) {
                        confirm(getMultilingualStringValue(MultilingualKey.SI_OSP_GBL_JQ_GBL_034), function () {
                            app.savebulkConnection(allConnections);
                        });
                    }
                } else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_023);//Already connected for this range!
                }
            } else {
                if ($(app.DE.SourceCable).val() == '0' || $(app.DE.DestinationCable).val() == '0') {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_148);//Please select left and right cable!
                    return false;
                }
                for (var i = leftStartRange; i <= leftEndRange; i++) {
                    if ($('#dvLeftCable .leftFiber .src[data-is-connected="0"][data-status-id="1"][data-port-no="' + i + '"]').length > 0 && $('#dvRightCable .rightFiber .trg[data-is-connected="0"][data-port-no="' + rightStartRange + '"]').length > 0) {
                        var sourceId = $('#dvLeftCable .leftFiber .src[data-is-connected="0"][data-status-id="1"][data-port-no="' + i + '"]').attr('id');
                        var targetId = $('#dvRightCable .rightFiber .trg[data-is-connected="0"][data-status-id="1"][data-port-no="' + rightStartRange + '"]').attr('id');
                        allConnections.push(app.getConnectionsData(sourceId, targetId));
                    }
                    if (rightStartRange == rightEndRange) { break; }
                    rightStartRange++;
                }
                //$.each($('#dvLeftCable .leftFiber .src[data-is-connected="0"]'), function () {
                //    var sourceId = $(this).attr('id');
                //    var sourcePortNo = $(this).attr('data-port-no');
                //    var targetId = 'Right_' + $(app.DE.DestinationCable).val() + '_CABLE_' + sourcePortNo;
                //    if ($('#' + targetId).length > 0) {
                //        if ($('#' + targetId).hasClass('trg') && $('#' + targetId).attr('data-is-connected') == '0') { allConnections.push(app.getConnectionsData(sourceId, targetId)); }
                //    } else { return false; }
                //});
                if (allConnections.length > 0) {
                    if (app.validateConnections(allConnections)) {
                        confirm(getMultilingualStringValue(MultilingualKey.SI_OSP_GBL_JQ_GBL_034), function () {
                            app.savebulkConnection(allConnections);
                        });
                    }
                } else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_023);//Already connected for this range!
                }
            }
        }
    }
    this.CPESpliceAll = function () {
        var allConnections = [];
        $.each($('#dvCPEContainer .leftFiber .src[data-is-connected="0"][data-status-id="1"]'), function () {
            var sourceId = $(this).attr('id');
            $.each($('#divCustomerContainer .trg[data-is-connected="0"]'), function (index, item) {
                var taegetSystemid = $(this).attr('data-system-id');
                var targetId = $(this).attr('data-system-id') + '_' + $(this).attr('data-entity-type').toUpperCase() + '_' + 1;
                var index = allConnections.findIndex(function (m) { return parseInt(m.destination_system_id) == parseInt(taegetSystemid) });
                if ($('#' + targetId).length > 0 && index < 0) {
                    if ($('#' + targetId).hasClass('trg') && $('#' + targetId).attr('data-is-connected') == '0') { allConnections.push(app.getConnectionsData(sourceId, targetId, false)); }
                    return false;
                }
            })
        });
        if (allConnections.length > 0) {
            if (app.validateConnections(allConnections)) {
                confirm(getMultilingualStringValue(MultilingualKey.SI_OSP_GBL_JQ_GBL_034), function () {
                    app.savebulkConnection(allConnections);
                });
            }
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_024);//All ports are occupied!
        }
    }
    this.unSpliceAll = function () {
        if (app.currentConnections.length > 0) {
            if (($('#leftStartRange').length > 0 && $('#leftEndRange').length > 0) && ($('#rightStartRange').length > 0 && $('#rightEndRange').length > 0)) {
                app.rangeBasedUnsplice();
                return false;
            }
            //Are you sure to un-splice all connections?
            confirm(MultilingualKey.SI_OSP_GBL_JQ_GBL_025, function () {
                app.isAutoDeleteConnection = true;
                var res = ajaxReq('Splicing/deleteConnection', { objConnectionInfo: app.currentConnections }, true, function (data) {
                    if (data.status == 'OK') {
                        try {
                            $.each(app.connectionObjects, function (index, item) {
                                if ($('#' + item.sourceId).attr('data-is-connected') == '1' && $('#' + item.targetId).attr('data-is-connected') == '1') {
                                    jsPlumb.detach(item);
                                }
                            });
                            app.connectionObjects = [];
                            app.currentConnections = [];
                            setTimeout(function () { alert(data.message); }, 100);
                            var vacantColor = $('#hdnVacantColor').val();
                            $('div[data-is-connected="1"]').attr('data-is-connected', '0').attr('data-status-id', '1').css('background', vacantColor);
                            $('.customer').css('background-color', vacantColor);
                            app.isAutoDeleteConnection = false;
                            if ($(app.DE.ConnectingEntity).val() != '0' && $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') != 'FMS') {
                                app.getAvailablePorts();
                            }
                            app.updateTrayPort(true);
                        }
                        catch (err) { app.isAutoDeleteConnection = false; setTimeout(function () { alert(err.message); }, 200); }
                    }

                }, true, true);
            });
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_026);//Connections does not exist in current window!
        }
    }


    this.rangeBasedUnsplice = function () {
        if (app.currentConnections.length > 0) {
            var Msg = '';
            if (($('#leftStartRange').length > 0 && $('#leftEndRange').length > 0) && ($('#rightStartRange').length > 0 && $('#rightEndRange').length > 0)) {

                //Below connection will be removed,Are you sure to un-splice all connections?//Entity//From//To
                Msg = MultilingualKey.SI_OSP_GBL_JQ_GBL_053 + ' :<br/><div class="table-responsive tble_cls"><table border="1" style="width:100%;"class="alertgrid" id="tblAlertDependendEntity"><thead><tr><td><b> ' + MultilingualKey.SI_OSP_GBL_JQ_FRM_149 + ' </b></td><td><b> ' + MultilingualKey.SI_OSP_GBL_JQ_FRM_150 + ' </b></td><td><b> ' + MultilingualKey.SI_GBL_GBL_JQ_FRM_025 + ' </b></td></tr></thead><tbody>';
                if ($('#leftStartRange').val() != '' && $('#leftEndRange').val() != '') {
                    Msg += '<tr><td>' + (($('#ddlSourceCable').val() != '0' ? $('#ddlSourceCable option:selected').text() : $('#ddlConnectingEntity option:selected').text())) + ' </td><td> ' + $('#leftStartRange').val() + '</td><td> ' + $('#leftEndRange').val() + '</td></tr>';
                }
                if ($('#rightStartRange').val() != '' && $('#rightEndRange').val() != '') {
                    Msg += '<tr><td>' + $('#ddlDestinationCable option:selected').text() + ' </td><td> ' + $('#rightStartRange').val() + '</td><td> ' + $('#rightEndRange').val() + '</td></tr>';
                }
                Msg += '</tbody></table></div>';
            }
            if (!app.isUnSplicingRangeValid()) {
                return false;
            }
            var leftStartRange = parseInt($('#leftStartRange').val());
            var leftEndRange = parseInt($('#leftEndRange').val());
            var rightStartRange = parseInt($('#rightStartRange').val());
            var rightEndRange = parseInt($('#rightEndRange').val());
            var sourceEntity = 'Cable';
            var sourceSystemId = $('#ddlSourceCable').val();
            if ($('#ddlConnectingEntity option:selected').attr('data-entity-type') == 'FMS' || $('#ddlConnectingEntity option:selected').attr('data-entity-type') == 'ODF' || $('#ddlConnectingEntity option:selected').attr('data-entity-type') == 'HTB') {
                sourceEntity = $('#ddlConnectingEntity option:selected').attr('data-entity-type');
                sourceSystemId = $('#ddlConnectingEntity').val();
            }
            for (var i = leftStartRange; i <= leftEndRange; i++) {
                var index = app.currentConnections.findIndex(function (item) {
                    return (item.source_port_no == i && item.source_entity_type == sourceEntity && item.source_system_id == sourceSystemId)
                        || (item.destination_port_no == i && item.destination_entity_type == sourceEntity && item.destination_system_id == sourceSystemId)
                });
                if (index >= 0) {
                    app.filteredConnections.push(app.currentConnections[index]);
                    app.currentConnections.splice(index, 1);
                }
            }
            for (var i = rightStartRange; i <= rightEndRange; i++) {
                var index = app.currentConnections.findIndex(function (item) {
                    return (item.source_port_no == i && item.source_entity_type == 'Cable' && item.source_system_id == $('#ddlDestinationCable').val())
                        || (item.destination_port_no == i && item.destination_entity_type == 'Cable' && item.destination_system_id == $('#ddlDestinationCable').val())
                });
                if (index >= 0) {
                    app.filteredConnections.push(app.currentConnections[index]);
                    app.currentConnections.splice(index, 1);
                }
            }
            if (app.filteredConnections.length > 0) {
                confirm(Msg, function () {

                    app.isAutoDeleteConnection = true;
                    var res = ajaxReq('Splicing/deleteConnection', { objConnectionInfo: app.filteredConnections }, true, function (data) {
                        if (data.status == 'OK') {
                            try {
                                $.each(app.filteredConnections, function (index, item) {
                                    var sourceConId = item.source_system_id + '_' + item.source_entity_type.toUpperCase() + '_' + item.source_port_no;
                                    var destinationConId = item.destination_system_id + '_' + item.destination_entity_type.toUpperCase() + '_' + item.destination_port_no;
                                    if (item.source_entity_type.toUpperCase() == 'CABLE') {
                                        if ($('#ddlSourceCable').val() == item.source_system_id) { sourceConId = 'Left_' + sourceConId; }
                                        if ($('#ddlDestinationCable').val() == item.source_system_id) { sourceConId = 'Right_' + sourceConId; }
                                    }
                                    if (item.destination_entity_type.toUpperCase() == 'CABLE') {
                                        if ($('#ddlSourceCable').val() == item.destination_system_id) { destinationConId = 'Left_' + destinationConId; }
                                        if ($('#ddlDestinationCable').val() == item.destination_system_id) { destinationConId = 'Right_' + destinationConId; }
                                    }
                                    var filerObject = app.connectionObjects.filter(function (item) {
                                        return item.sourceId == sourceConId
                                            || item.targetId == sourceConId
                                            || item.sourceId == destinationConId
                                            || item.targetId == destinationConId
                                    });
                                    if (filerObject.length > 0) {
                                        $.each(filerObject, function (index, item) {
                                            jsPlumb.detach(item);
                                            var vacantColor = $('#hdnVacantColor').val();
                                            $('#' + item.sourceId).attr('data-is-connected', '0').attr('data-status-id', '1').css('background', vacantColor);
                                            $('#' + item.targetId).attr('data-is-connected', '0').attr('data-status-id', '1').css('background', vacantColor);
                                            // app.connectionObjects.splice(index, 1);
                                        })
                                    }
                                })
                                app.filteredConnections = [];
                                setTimeout(function () { alert(data.message); }, 100);
                                var vacantColor = $('#hdnVacantColor').val();
                                //$('div[data-is-connected="1"]').attr('data-is-connected', '0').css('background', vacantColor);
                                $('.customer').css('background-color', vacantColor);
                                app.isAutoDeleteConnection = false;
                                if ($(app.DE.ConnectingEntity).val() != '0' && $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') != 'FMS') {
                                    app.getAvailablePorts();
                                }
                                app.updateTrayPort(true);
                            }
                            catch (err) {
                                app.isAutoDeleteConnection = false;
                                setTimeout(function () { alert(err.message); }, 200);
                            }
                        }
                    }, true, true);
                });
            } else { alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_027); }//Connections does not exist for this range!
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_026);//Connections does not exist in current window!
        }
    }
    this.swapCables = function () {
        $("#dvLeftCable .cableContainer").swap({
            target: "dvRightCable .cableContainer", // Mandatory. The ID of the element we want to swap with
            opacity: "0.5", // Optional. If set will give the swapping elements a translucent effect while in motion
            speed: 1000, // Optional. The time taken in milliseconds for the animation to occur
            callback: function () { // Optional. Callback function once the swap is complete
                var leftCableHtml = $('#dvLeftCable').html();
                var rightCableHtml = $("#dvRightCable").html();
                $('#dvLeftCable').html(rightCableHtml);
                $('#dvRightCable').html(leftCableHtml);
                $('#dvLeftCable .cableContainer,#dvRightCable .cableContainer').attr('style', 'direction:' + (app.isSwapped ? 'ltr' : 'rtl'));
                app.isSwapped = !app.isSwapped;
                $('#dvRightCable .fiber').removeClass('leftFiber').addClass('rightFiber');
                $('#dvLeftCable .fiber').removeClass('rightFiber').addClass('leftFiber');
            }
        });
    }
    this.bindjsPlumbEvent = function (objJsPlumb) {
        objJsPlumb.bind("connection", function (info, originalEvent) {
             
            if (!app.isMoved) {
                var connection = app.getConnectionsData(info.sourceId, info.targetId, true);
                if ((connection.source_entity_type == 'HTB' && connection.destination_entity_type == 'ONT') || (connection.source_entity_type == 'ONT' && connection.destination_entity_type == 'HTB')) {
                    app.checkPatchTemplateExist(connection, info)
                } else { var connectionId = app.saveConnection([connection], info.sourceId, info.targetId, info); }
                app.connectionObjects.push(info);
            } else {
                jsPlumb.detach(info);
            }
        });
        objJsPlumb.bind("connectionDetached", function (info, originalEvent) {
            var source = $('#' + info.sourceId);
            var target = $('#' + info.targetId);
            var connection = app.getConnectionsData(info.sourceId, info.targetId, true);
            if (!app.isAutoDeleteConnection) {
                app.isMoved = true;
                if (app.isCPEWindow && ((source.attr('data-entity-type') == 'HTB' && target.attr('data-entity-type') == 'ONT')
                    || (target.attr('data-entity-type') == 'HTB' && source.attr('data-entity-type') == 'ONT'))) {
                    // Patch cord will be deleted,do you want to continue?
                    confirm(MultilingualKey.SI_OSP_GBL_JQ_GBL_028, function () {
                        app.deleteConnection([connection], source, target);
                    }, function () { app.createConnection(info.sourceId, info.targetId); });
                } else { app.deleteConnection([connection], source, target); }

            }
        });
        objJsPlumb.bind("beforeDrop", function (params) {
             
            SetThroughConnValue();
            var connection = app.getConnectionsData(params.sourceId, params.targetId, true);

            var connectorEtype = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
            if (connection.is_through_connection.toLowerCase() == 'true' && connection.source_port_no != connection.destination_port_no) {
                alert("Through connection can be performed with same tube/core number only!");
                return false;
            }
            if ($('#' + params.sourceId).attr('data-entity-type') == $('#' + params.targetId).attr('data-entity-type') && parseInt($('#' + params.sourceId).attr('data-system-id')) == parseInt($('#' + params.targetId).attr('data-system-id')) && $('#' + params.sourceId).attr('data-entity-type').toLowerCase() != 'cable') {
                //Connection can not be created between two port for same
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_029 + '  <b>' + $('#' + params.sourceId).attr('data-entity-type') + '</b>.');
                return false;
            } else if ($('#' + params.sourceId).attr('data-entity-type') == $('#' + params.targetId).attr('data-entity-type') && parseInt($('#' + params.sourceId).attr('data-system-id')) == parseInt($('#' + params.targetId).attr('data-system-id')) && $('#' + params.sourceId).attr('data-entity-type').toLowerCase() != 'cable') {
                //Connection can not be created between two port for same
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_029 + '  <b>' + $('#' + params.sourceId).attr('data-entity-type') + '</b>.');
                return false;
            }
            else if ($('#' + params.sourceId).attr('data-entity-type') == $('#' + params.targetId).attr('data-entity-type')
                && parseInt($('#' + params.sourceId).attr('data-system-id')) != parseInt($('#' + params.targetId).attr('data-system-id'))
                && ((parseInt($('#' + params.sourceId).attr('data-port-no')) > 0 && parseInt($('#' + params.targetId).attr('data-port-no')) > 0) || (parseInt($('#' + params.sourceId).attr('data-port-no')) < 0 && parseInt($('#' + params.targetId).attr('data-port-no')) < 0))
                && $('#' + params.sourceId).attr('data-entity-type').toLowerCase() != 'cable') {
                //Connection can not be created between two output/input for different
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_030 + '  <b>' + $('#' + params.sourceId).attr('data-entity-type') + '</b>.');
                return false;
            } else if ((connectorEtype == 'ODF' || connectorEtype == 'FMS' || connectorEtype == 'ONT') && $('#' + params.sourceId).attr('data-entity-type') == $('#' + params.targetId).attr('data-entity-type')) {
                //Connection can not be created between two core for cable.
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_031);
                return false;
            } else if ($('#' + params.targetId).attr('data-is-connected') == '1') {
                return false;
            } else if (($('#' + params.sourceId).attr('data-entity-type') == 'HTB' && $('#' + params.targetId).attr('data-entity-type') == 'Customer')
                || ($('#' + params.targetId).attr('data-entity-type') == 'HTB' && $('#' + params.sourceId).attr('data-entity-type') == 'Customer')) {
                //Connection can not be created between HTB and Customer
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_032);
                return false;
            } else if ((($('#' + params.sourceId).attr('data-entity-type') == 'HTB' && $('#' + params.targetId).attr('data-entity-type') == 'ONT')
                || ($('#' + params.targetId).attr('data-entity-type') == 'HTB' && $('#' + params.sourceId).attr('data-entity-type') == 'ONT'))
                && parseInt($('#' + params.targetId).attr('data-port-no')) > 0 && parseInt($('#' + params.sourceId).attr('data-port-no')) > 0) {
                //Connection can not be created between two output port.
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_033);
                return false;
            }
            else if (app.hexc($('#' + params.sourceId).css('background-color')) != $('#hdnConnectedColor').val() && !app.validateConnections([connection])) {
                return false;
            }
            else if (!app.validateTray()) {
                alert('All ports has been occupied for selected tray!');
                return false;
            }
            else { return true; }
        });

    }
    this.getConnectionsData = function (sourceId, targetId, isManualSplicing) {
        var is_through = $("input[name='ConnInfoMaster.is_through_connection']:checked").val();
        var source = $('#' + sourceId);
        var target = $('#' + targetId);
        var connectorEtype = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        var connection = {
            source_system_id: source.attr('data-system-id'),
            source_network_id: source.attr('data-network-id'),
            source_entity_type: source.attr('data-entity-type'),
            source_port_no: source.attr('data-port-no'),
            destination_system_id: target.attr('data-system-id'),
            destination_network_id: target.attr('data-network-id'),
            destination_entity_type: target.attr('data-entity-type'),
            destination_port_no: target.attr('data-port-no'),
            splicing_source: (si != null ? 'OSP_SPLICING' : 'ISP_SPLICING'),
            is_source_cable_a_end: JSON.parse(source.attr('data-is-cable-a-end').toLowerCase()),
            is_destination_cable_a_end: JSON.parse(target.attr('data-is-cable-a-end').toLowerCase()),
            equipment_system_id: $(app.DE.ConnectingEntity).val(),
            equipment_network_id: $(app.DE.ConnectingEntity + ' :selected').attr('data-network-id'),
            equipment_entity_type: $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type'),
            equipment_tray_system_id: $(app.DE.ddlSpliceTray + ' :selected').attr('data-splicetrayid'),
            is_through_connection: is_through
        };

        if (connectorEtype == 'ODF' || connectorEtype == 'FMS' || connectorEtype == 'ONT' || connectorEtype == 'HTB' ||
            (connection.source_entity_type.toUpperCase() == 'FMS' && connection.destination_entity_type.toUpperCase() == 'SPLITTER') ||
            (connection.source_entity_type.toUpperCase() == 'SPLITTER' && connection.destination_entity_type.toUpperCase() == 'FMS') ||
            (connection.source_entity_type.toUpperCase() == 'SPLITTER' && connection.destination_entity_type.toUpperCase() == 'SPLITTER')) {
            connection.equipment_system_id = 0;
            connection.equipment_network_id = null;
            connection.equipment_entity_type = null;
        }
        return connection;
    }
    this.saveConnection = function (_data, sourceId, targetId, info) {
         
        ajaxReq('Splicing/SaveConnectionInfo', _data, true, function (resp) {
            if (resp.status) {
                $('#SlideMessage').text(MultilingualKey.SI_OSP_GBL_JQ_GBL_052);//Connection has saved successfully!

                if ($(app.DE.ConnectingEntity).val() != '0' && $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') != 'FMS') {
                    app.getAvailablePorts();
                }
                $(".alert-info").slideDown(300);
                setTimeout(function () {
                    $(".alert-info").slideUp(300);
                }, 3000)
                var connectedColor = $('#hdnConnectedColor').val();
                $('#' + sourceId).attr('data-is-connected', '1').attr('data-status-id', '2').css('background', connectedColor);
                $('#' + targetId).attr('data-is-connected', '1').attr('data-status-id', '2').css('background', connectedColor);
                $('#' + targetId).parent().parent().children('.customer').css('background-color', connectedColor);
                $('#' + sourceId).parent().parent().children('.customer').css('background-color', connectedColor);
                app.currentConnections.push(app.getConnectionsData(sourceId, targetId));
                if (isp != null && app.isCPEWindow) {
                    isp.getCPEConnections();
                }
                app.updateTrayPort(true);
            } else {
                jsPlumb.detach(info);
                alert(resp.message);
            }
        }, true, true);
    }
    this.savebulkConnection = function (_data) {
        //Splice all will perform straight connectivity except the cores/ports which are already connected or connected to other// Do you want to continue?
         
        var IsSamePortConnForThrough = true;
        var is_through = $("input[name='ConnInfoMaster.is_through_connection']:checked").val();
        $.each(_data, function (index, item) {
             
            _data[index].splicing_source = 'Splice All';
            if (is_through != undefined) {
                if (is_through.toLowerCase() == 'true' && item.source_port_no != item.destination_port_no) {
                    alert("Through connection can be performed with same tube/core number only!");
                    IsSamePortConnForThrough = false;
                    return false;
                }
            }
        });
        if (IsSamePortConnForThrough) {
            ajaxReq('Splicing/SaveConnectionInfo', _data, true, function (resp) {
                $('#SlideMessage').text(MultilingualKey.SI_OSP_GBL_JQ_GBL_052);
                $.each(_data, function (index, item) {
                    var sourceId = item.source_system_id + '_' + item.source_entity_type.toUpperCase() + '_' + item.source_port_no;
                    var targetId = item.destination_system_id + '_' + item.destination_entity_type.toUpperCase() + '_' + item.destination_port_no;
                    if (item.source_entity_type == 'Cable' && parseInt(item.source_system_id) == parseInt($(app.DE.SourceCable).val())) {
                        sourceId = 'Left_' + sourceId;
                    }
                    if (item.destination_entity_type == 'Cable' && parseInt(item.destination_system_id) == parseInt($(app.DE.DestinationCable).val())) {
                        targetId = 'Right_' + targetId;
                    }
                    if ($('#' + targetId).hasClass('trg')) {
                        app.createConnection(sourceId, targetId);
                    }
                });
                if ($(app.DE.ConnectingEntity).val() != '0' && $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') != 'FMS') {
                    app.getAvailablePorts();
                }
                if ($(app.DE.ddlSpliceTray).val() != '') {
                    app.updateTrayPort(true);
                }
                $(".alert-info").slideDown(300);
                setTimeout(function () {
                    $(".alert-info").slideUp(300);
                }, 3000)
            }, true, true)
        }
        //});
    }
    this.ClearSplicingRelatedObjs = function (tabType) {
        $(app.DE.DestinationCable).val('0').trigger("chosen:updated");
        $(app.DE.SourceCable).val('0').trigger("chosen:updated");
        $(app.DE.ConnectingEntity).val('0').trigger("chosen:updated");
        $(app.DE.ddlcustomer).val('0').trigger("chosen:updated");
        $(app.DE.ddlCPEEntity).val('').trigger("chosen:updated");
        if (app.gMapObj.pointObj != null) { app.gMapObj.pointObj.setMap(null); app.gMapObj.pointObj = null; }
        if (app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
        if (app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; }
    }
    this.focusCPEToCustomer = function (ddlId) {
        var systemID = $(ddlId).val();
        var eType = $(ddlId + ' :selected').attr('data-entity-type');
        var gType = $(ddlId + ' :selected').attr('data-geom-type');

        if (systemID != '0') {
            // HIGHLIGHT EQUIPMENT/CABLE ON MAP
            ajaxReq('main/getGeometryDetail', { systemId: systemID, entityType: eType, geomType: gType }, true, function (resp) {
                if (resp.status = 'OK') {
                    if (resp.result != null && resp.result != undefined) {
                        var latLngArr = si.getLatLongArr(resp.result.sp_geometry);
                        app._focusMe(gType, latLngArr);
                    }
                }
            }, true, false);
        }
    }
    this.onChangeSpliceTerminatedCablesOnly = function () {
        $(app.DE.ConnectingEntity).val('0').trigger("chosen:updated");
        $(app.DE.SourceCable + ' option:not([value="0"])').show();
        $(app.DE.DestinationCable + ' option:not([value="0"])').show();
        $(app.DE.DestinationCable).val('0').trigger("chosen:updated");
        $(app.DE.SourceCable).val('0').trigger("chosen:updated");
    }
    this.ShowCablesTerminatedOnDeviceOnly = function (sysId, eType) {
         
        if (sysId > 0) {
            $(app.DE.SourceCable + ' option:not([value="0"])').hide();
            $(app.DE.SourceCable + ' option[data-a-system-id="' + sysId + '"][data-a-entity-type="' + eType + '" i][data-is-both-ends="False"]').show();
            $(app.DE.SourceCable + ' option[data-b-system-id="' + sysId + '"][data-b-entity-type="' + eType + '" i][data-is-both-ends="False"]').show();

            $(app.DE.DestinationCable + ' option:not([value="0"])').hide();
            $(app.DE.DestinationCable + ' option[data-a-system-id="' + sysId + '"][data-a-entity-type="' + eType + '" i][data-is-both-ends="False"]').show();
            $(app.DE.DestinationCable + ' option[data-b-system-id="' + sysId + '"][data-b-entity-type="' + eType + '" i][data-is-both-ends="False"]').show();

            // for both ends..

            var arrCableSystemId = [];
            $(app.DE.SourceCable + ' option[data-is-both-ends="True"').each(function () {
                if (arrCableSystemId.indexOf($(this).val()) < 0) {
                    arrCableSystemId.push($(this).val());
                }
            });
            $.each(arrCableSystemId, function (index, value) {
                arrSourceOptions = $(app.DE.SourceCable + ' option[value="' + value + '"][data-is-both-ends="True"]');
                arrDestinationOptions = $(app.DE.DestinationCable + ' option[value="' + value + '"][data-is-both-ends="True"]');
                if (arrSourceOptions.length == 2) {
                    if ($(arrSourceOptions[0]).attr("data-a-system-id") == sysId &&
                        $(arrSourceOptions[0]).attr("data-a-entity-type").toUpperCase() == eType.toUpperCase() &&
                        $(arrSourceOptions[0]).attr("data-b-system-id") == sysId &&
                        $(arrSourceOptions[0]).attr("data-b-entity-type").toUpperCase() == eType.toUpperCase()
                    ) {
                        $(arrSourceOptions[0]).show();
                        $(arrSourceOptions[1]).show();
                        $(arrDestinationOptions[0]).show();
                        $(arrDestinationOptions[1]).show();
                    }
                    else if ($(arrSourceOptions[0]).attr("data-a-system-id") == sysId &&
                        $(arrSourceOptions[0]).attr("data-a-entity-type").toUpperCase() == eType.toUpperCase()) {
                        $(app.DE.SourceCable + ' option[value="' + value + '"][data-is-both-ends="True"][data-is-start-point="True"]').show();
                        $(app.DE.DestinationCable + ' option[value="' + value + '"][data-is-both-ends="True"][data-is-start-point="True"]').show();
                    }
                    else if ($(arrSourceOptions[0]).attr("data-b-system-id") == sysId &&
                        $(arrSourceOptions[0]).attr("data-b-entity-type").toUpperCase() == eType.toUpperCase()) {

                        $(app.DE.SourceCable + ' option[value="' + value + '"][data-is-both-ends="True"][data-is-end-point="True"]').show();
                        $(app.DE.DestinationCable + ' option[value="' + value + '"][data-is-both-ends="True"][data-is-end-point="True"]').show();
                    }
                }
            });
        }
        else {
            $(app.DE.SourceCable + ' option:not([value="0"])').show();
            $(app.DE.DestinationCable + ' option:not([value="0"])').show();
        }
    }
    this.focusCableToEquipment = function (ddlId) {
        let _isMiddlewareEntity = ($(app.DE.ConnectingEntity + ' :selected').attr('data-is-middleware-entity').toLowerCase() == 'true');
        var systemID = $(ddlId).val();
        var eType = $(ddlId + ' :selected').attr('data-entity-type');
        var gType = $(ddlId + ' :selected').attr('data-geom-type');
        var cableDirection = $(ddlId + ' :selected').attr('data-cable-direction');
        var IsVirtual = $(ddlId + ' :selected').attr('data-is-virtual');

        if (gType == 'Point') {
            if ($(app.DE.chkSpliceTerminatedCablesOnly).is(":checked")) {
                //SHOW ONLY CABLES WHICH ARE TERMINATED ON SELECTED DEVICE..
                app.ShowCablesTerminatedOnDeviceOnly(systemID, eType);
            }
            ///////////////////////////////
            $(app.DE.DestinationCable).val('0').trigger("chosen:updated");
            $(app.DE.SourceCable).val('0').trigger("chosen:updated");
            $(app.DE.DestinationCable + ' option:not([data-is-isp-entity="True"])').attr('disabled', false).trigger("chosen:updated");
            $(app.DE.SourceCable + ' option:not([data-is-isp-entity="True"])').attr('disabled', false).trigger("chosen:updated");
            if (app.gMapObj.pointObj != null) { app.gMapObj.pointObj.setMap(null); app.gMapObj.pointObj = null; }
            if (app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
            if (app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; }
        }
        //IF EQUIPMENT IS FMS/ODF, THEN ONLY ONE CABLE DROPDOWN SHOULD BE SHOWN..
        var connectorEtype = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        if (_isMiddlewareEntity) {
            $(app.DE.lblRightCable).text('Cable');
            $(app.DE.divLeftCable).hide();
        }
        else {
            $(app.DE.divLeftCable).show();
            $(app.DE.lblRightCable).text(MultilingualKey.SI_ISP_GBL_GBL_FRM_008);
             // update multilingual from and to           
            //$(app.DE.lblLeftCable).text(MultilingualKey.SI_OSP_GBL_GBL_GBL_322);
            //$(app.DE.lblRightCable).text(MultilingualKey.SI_OSP_GBL_GBL_GBL_323);
            //$(app.DE.lblLeftCable).text(MultilingualKey.SI_OSP_GBL_GBL_FRM_011 == "IN" ? MultilingualKey.SI_OSP_CAB_NET_FRM_054 : MultilingualKey.SI_OSP_GBL_NET_FRM_130);
            //$(app.DE.lblRightCable).text(MultilingualKey.SI_OSP_GBL_GBL_FRM_012 == "OUT" ? MultilingualKey.SI_OSP_CAB_NET_FRM_055 : MultilingualKey.SI_OSP_GBL_NET_FRM_131);
            }

        if (cableDirection == 'Source') {
            if (systemID == '0') {
                $(app.DE.DestinationCable + ' option:not([data-is-isp-entity="True"])').attr('disabled', false).trigger("chosen:updated");
                if (app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
                return false;
            }
            else {
                //DISABLE THE SAME CABLE ID FROM DESTINATION DROPDOWN..
                $(app.DE.DestinationCable + ' option:not([data-is-isp-entity="True"])').attr('disabled', false).trigger("chosen:updated");
                var destOpt = app.DE.DestinationCable + ' option[data-text="' + $(app.DE.SourceCable + ' :selected').text() + '"]';
                $(destOpt).attr('disabled', true).trigger("chosen:updated");
            }
        }
        else if (cableDirection == 'Destination') {
            if (systemID == '0') {
                $(app.DE.SourceCable + ' option:not([data-is-isp-entity="True"])').attr('disabled', false).trigger("chosen:updated");
                if (app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; }
                return false;
            }
            else {
                //DISABLE THE SAME CABLE ID FROM SOURCE DROPDOWN..
                $(app.DE.SourceCable + ' option:not([data-is-isp-entity="True"])').attr('disabled', false).trigger("chosen:updated");
                var destOpt = app.DE.SourceCable + ' option[data-text="' + $(app.DE.DestinationCable + ' :selected').text() + '"]';
                $(destOpt).attr('disabled', true).trigger("chosen:updated");
            }
        }
        if (systemID != '0') {
            // HIGHLIGHT EQUIPMENT/CABLE ON MAP
            ajaxReq('main/getGeometryDetail', { systemId: systemID, entityType: eType, geomType: gType }, true, function (resp) {
                if (resp.status = 'OK') {
                    if (resp.result != null && resp.result != undefined) {
                        var latLngArr = si.getLatLongArr(resp.result.sp_geometry);
                        app._focusMe(gType, latLngArr, cableDirection);
                    }
                }
            }, true, false);
        }
    }
    this._focusMe = function (geomType, latLngArr, cableDirection) {
        switch (geomType) {
            case 'Point':
                if (app.gMapObj.pointObj != null) { app.gMapObj.pointObj.setMap(null); app.gMapObj.pointObj = null; }
                app.gMapObj.pointObj = si.createMarker(latLngArr[0], 'Content/images/dwnArrow.png');
                app.gMapObj.pointObj.setAnimation(google.maps.Animation.BOUNCE);
                app.gMapObj.pointObj.setMap(si.map);
                break;
            case 'Line':
                var lineObj = null;

                if (cableDirection == 'Source') {

                    if (app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
                    app.gMapObj.sourceLineObj = si.createLine(latLngArr);
                    lineObj = app.gMapObj.sourceLineObj;

                }
                else if (cableDirection == 'Destination') {
                    if (app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; }
                    app.gMapObj.destLineObj = si.createLine(latLngArr);
                    lineObj = app.gMapObj.destLineObj;
                }
                lineObj.strokeOpacity = 0;
                lineObj.strokeWeight = 4;
                lineObj.set('icons', [{ icon: { path: 'M -.5,-.5 .5,-.5, .5,.5 -.5,.5', fillOpacity: 1, fillColor: 'blue' }, repeat: '8px' }]);
                setTimeout(function () { animateLine(lineObj, 0) }, 20);
                lineObj.setMap(si.map);
                break;
        }
        //window.setTimeout(function () { if (app.gMapObj.infoHoverObj) { app.gMapObj.infoHoverObj.setMap(null) } app.gMapObj.infoHoverObj = null; }, 25000);
    }
    this.getConnections = function () {
        $.each($('#dvLeftCable .leftFiber .endPint[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });

        $.each($('#dvRightCable .rightFiber .endPint[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });
        $.each($('#divSplitter .rightFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('data-target-id');
                var targetId = $(this).attr('id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });
        $.each($('#divSplitter .leftFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });

        $.each($('#divFMS .rightFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('data-target-id');
                var targetId = $(this).attr('id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });
        $.each($('#divFMS .leftFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });

        $.each($('#divONT .rightFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('data-target-id');
                var targetId = $(this).attr('id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });
        $.each($('#divONT .leftFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });

        if ($(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') == 'FMS') {
            $.each($('.DeviceView .leftFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
                if ($(this).attr('data-is-connected') == '0') {
                    var sourceId = $(this).attr('id');
                    var targetId = $(this).attr('data-target-id');
                    if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                        sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                    }
                    if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                        targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                    }
                    app.createConnection(sourceId, targetId);
                }
            });
            $.each($('#dvRightCable .rightFiber .endPint[data-is-connected-to-same="True"]'), function () {
                if ($(this).attr('data-is-connected') == '0') {
                    var sourceId = $(this).attr('data-target-id');
                    var targetId = $(this).attr('id');
                    if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                        sourceId = 'Right_' + sourceId;
                    }
                    if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                        targetId = 'Right_' + targetId;
                    }
                    app.createConnection(sourceId, targetId);
                }
            });
        }
        //LEFT TO RIGHT CONNECTION
        $.each($('.leftConPoint[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });
        //REIGHT TO LEFT CONNECTION
        $.each($('.rightConPoint[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {

                var sourceId = $(this).attr('data-target-id');
                var targetId = $(this).attr('id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });

        $.each($('#divHTB .rightFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('data-target-id');
                var targetId = $(this).attr('id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });
        $.each($('#divHTB .leftFiber .innerFiber[data-is-connected-to-same="True"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('CABLE') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('CABLE') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                app.createConnection(sourceId, targetId);
            }
        });
        $('#dvProgressSplice').hide();
    }
    this.createConnection = function (sourceId, targetId) {
         
        var colorCode = '#00ba8a';
        if ($('#' + sourceId).attr('data-through-conn') != undefined && $('#' + sourceId).attr('data-through-conn').toUpperCase() == "TRUE")
        { colorCode = '#808080'; }

        if ($('#' + targetId).length > 0 && $('#' + sourceId).length > 0) {
            var datalinkid = $('#' + sourceId).attr("data-link-id");
            var defaultOptions = app.getDefaultOptions();
            app.mainJSPlumb = jsPlumb.getInstance();
            app.mainJSPlumb.importDefaults(defaultOptions);
            var cableOptions = app.getOptions(sourceId, targetId, 'Left', colorCode, '');
            cableOptions.anchors = app.getAnchors(sourceId, targetId);
            var conObject = app.mainJSPlumb.connect(cableOptions);
            app.connectionObjects.push(conObject);
            app.bindjsPlumbEvent(app.mainJSPlumb);
            var connectedColor = $('#hdnConnectedColor').val();
            $('#' + sourceId).attr('data-is-connected', '1').attr('data-status-id', '2').css('background', connectedColor);
            $('#' + targetId).attr('data-is-connected', '1').attr('data-status-id', '2').css('background', connectedColor);
            $('#' + targetId).parent().parent().children('.customer').css('background-color', connectedColor);
            $('#' + sourceId).parent().parent().children('.customer').css('background-color', connectedColor);
            app.currentConnections.push(app.getConnectionsData(sourceId, targetId));

            if (datalinkid > 0) {
                 
                conObject.setPaintStyle({ strokeStyle: colorCode, lineWidth: 2 });
                conObject.setParameters($('#' + conObject.sourceId).data().viaSystemId);
                 $('#divSpliceWindow #' + sourceId).siblings('._jsPlumb_connector').tooltip({ title: $('#' + sourceId).attr("data-name"), html: true, placement: "top" });
            }

            if (($('#' + sourceId).data().entityType == 'HTB' && $('#' + targetId).data().entityType == 'ONT')
                || ($('#' + sourceId).data().entityType == 'ONT' && $('#' + targetId).data().entityType == 'HTB')) {
                conObject.setPaintStyle({ strokeStyle: '#000000', lineWidth: 3 });
                conObject.setParameters($('#' + conObject.sourceId).data().viaSystemId);
                conObject.bind('click', function (e) {
                    app.patchActions(conObject.sourceId, e);
                })
                $('#divSpliceWindow #' + sourceId).siblings('._jsPlumb_connector').tooltip({ title: $('#' + sourceId).data().viaNetworkId, html: true, placement: "top" });
            }
        }
    }
    this.getAnchors = function (sourceId, targetId) {
        var anchors = [];
        if ($('#' + sourceId).parent().hasClass('leftFiber')) {
            anchors.push('Right');
        } else if ($('#' + sourceId).parent().hasClass('rightFiber')) { anchors.push('Left'); }
        if ($('#' + targetId).parent().hasClass('leftFiber')) {
            anchors.push('Right');
        } else if ($('#' + targetId).parent().hasClass('rightFiber')) { anchors.push('Left'); }

        return anchors;

    }
    this.getDefaultOptions = function () {
        var options = {};
        options.DragOptions = { cursor: 'pointer', zIndex: 2000 }
        options.PaintStyle = { strokeStyle: '#00ba8a', lineWidth: 1 };
        options.EndpointStyle = { width: 20, height: 16, strokeStyle: 'black', fillStyle: "black" };
        options.Endpoint = ["Dot", { radius: 3.5 }];
        options.connector = ["Bezier", { curviness: 100 }];
        options.MaxConnections = 1;
        return options;
    }
    this.getOptions = function (sourceId, targetId, cablePosition, color, lineType) {
        try {
            var options = {};
            options.source = sourceId,
                options.target = targetId,
                options.connector = ["Bezier", { curviness: 50 }],
                options.paintStyle = { strokeStyle: color, lineWidth: 1 }
            options.MaxConnections = 1;
            return options;
        }
        catch (err) { }
    }
    this.deleteConnection = function (connections, source, target) {
        var source = source;
        var target = target;
        ajaxReq('Splicing/deleteConnection', { objConnectionInfo: connections }, true, function (resp) {
            app.isMoved = false;
            if (resp.status == "OK") {
                $('#SlideMessage').text(resp.message);
                var vacantColor = $('#hdnVacantColor').val();
                source.attr('data-is-connected', '0').attr('data-status-id', '1').css('background', vacantColor);
                target.attr('data-is-connected', '0').attr('data-status-id', '1').css('background', vacantColor);
                source.parent().parent().children('.customer').css('background-color', vacantColor);
                target.parent().parent().children('.customer').css('background-color', vacantColor);
                var sourceAttributes = source.data();
                var targetAttributes = target.data();
                var currentIndex = app.currentConnections.findIndex(function (m) {
                    return (parseInt(m.source_system_id) == parseInt(sourceAttributes.systemId) && m.source_entity_type.toUpperCase() == sourceAttributes.entityType.toUpperCase() && parseInt(m.source_port_no) == parseInt(sourceAttributes.portNo) && parseInt(m.destination_system_id) == parseInt(targetAttributes.systemId) && m.destination_entity_type.toUpperCase() == targetAttributes.entityType.toUpperCase() && parseInt(m.destination_port_no) == parseInt(targetAttributes.portNo))
                        || (parseInt(m.destination_system_id) == parseInt(targetAttributes.systemId) && m.destination_entity_type.toUpperCase() == targetAttributes.entityType.toUpperCase() && parseInt(m.source_port_no) == parseInt(targetAttributes.portNo) && parseInt(m.destination_system_id) == parseInt(sourceAttributes.systemId) && m.destination_entity_type.toUpperCase() == sourceAttributes.entityType.toUpperCase() && parseInt(m.destination_port_no) == parseInt(sourceAttributes.portNo))
                });
                app.currentConnections.splice(currentIndex, 1);
                //$.each(connections, function (index, item) {
                //    var sourceId = item.source_system_id + '_' + item.source_entity_type.toUpperCase() + '_' + item.source_port_no;
                //    var targetId = item.destination_system_id + '_' + item.destination_entity_type.toUpperCase() + '_' + item.destination_port_no;
                //    if (app.isODFTOCableWindow) {
                //        if (sourceId.indexOf('CABLE') > 0) {
                //            sourceId = 'Right_' + sourceId;
                //        }
                //        if (targetId.indexOf('CABLE') > 0) {
                //            targetId = 'Right_' + targetId;
                //        }
                //    } else {
                //        if (item.source_entity_type == 'Cable' && parseInt(item.source_system_id) == parseInt($(app.DE.SourceCable).val())) {
                //            sourceId = 'Left_' + sourceId;
                //        } else if (item.source_entity_type == 'Cable' && parseInt(item.source_system_id) == parseInt($(app.DE.DestinationCable).val())) {
                //            sourceId = 'Right_' + sourceId;
                //        }
                //        if (item.destination_entity_type == 'Cable' && parseInt(item.destination_system_id) == parseInt($(app.DE.DestinationCable).val())) {
                //            targetId = 'Right_' + targetId;
                //        } else if (item.destination_entity_type == 'Cable' && parseInt(item.destination_system_id) == parseInt($(app.DE.SourceCable).val())) {
                //            targetId = 'Left_' + targetId;
                //        }
                //    }


                //    $('#' + sourceId).attr('data-is-connected', '0');
                //    $('#' + targetId).attr('data-is-connected', '0');
                //    var currentIndex = app.currentConnections.findIndex(function (m) {
                //        return (parseInt(m.source_system_id) == parseInt(item.source_system_id) && m.source_entity_type.toUpperCase() == item.source_entity_type.toUpperCase() && m.source_port_no == item.source_port_no && parseInt(m.destination_system_id) == parseInt(item.destination_system_id) && m.destination_entity_type.toUpperCase() == item.destination_entity_type.toUpperCase() && m.destination_port_no == item.destination_port_no)
                //        || (parseInt(m.destination_system_id) == parseInt(item.source_system_id) && m.destination_entity_type.toUpperCase() == item.source_entity_type.toUpperCase() && m.source_port_no == item.destination_port_no && parseInt(m.destination_system_id) == parseInt(item.source_system_id) && m.destination_entity_type.toUpperCase() == item.source_entity_type.toUpperCase() && m.destination_port_no == item.source_port_no)
                //    });
                //    app.currentConnections.splice(currentIndex, 1);
                //})

                if ($(app.DE.ConnectingEntity).val() != '0' && $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') != 'FMS') {
                    app.getAvailablePorts();
                }
                $(".alert-info").slideDown(300);
                setTimeout(function () {
                    $(".alert-info").slideUp(300);
                }, 3000)
                app.updateTrayPort(false);
                if (isp != null && app.isCPEWindow) {
                    isp.getCPEConnections();
                }
            }
        }, true, true);
    }
    this.getAvailablePorts = function () {
        $('#divConnectionAlert').removeClass('alert-danger');
        $('#OverConnectionMessage').hide();
        var systemId = $(app.DE.ConnectingEntity).val();
        var entityType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        let _displayText = $(app.DE.ConnectingEntity + ' :selected').attr('data-loading-text');
        ajaxReq('Splicing/GetAvailabePorts', { systemId: systemId, entityType: entityType }, true, function (resp) {
            $('#spnTotalPorts').text('Total - ' + resp.total_ports);
            $('#spnAvailablePorts').text('Available - ' + (resp.available_ports < 0 ? 0 : resp.available_ports));
            $('#spnAdditionalPorts').text('Additional - ' + (resp.available_ports < 0 ? Math.abs(resp.available_ports) : 0))
            if (resp.available_ports < 0) {
                $('.DeviceView').css('background', '#ff00001c');
                $('#divConnectionAlert').addClass('alert-danger');
                $('#OverConnectionMessage').html('<div>All the available ports has been occupied for this entity (' + _displayText + ')</div> <div style="padding: 3px;"> <b>(Ports: Total-' + resp.total_ports + ', Available-' + (resp.available_ports < 0 ? 0 : resp.available_ports) + ', Additional-' + (resp.available_ports < 0 ? Math.abs(resp.available_ports) : 0) + ')</b></div>');
                $('#OverConnectionMessage').show();
            } else {
                $('.DeviceView').css('background', '#fff');
                $('#OverConnectionMessage').hide();
            }
        }, false, true, false);
    }
    this.isSplicingRangeValid = function () {
        var connectorType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        if ($(app.DE.leftStartRange).val() == '') {
            $(app.DE.leftStartRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_036);//Please enter the start range!
            return false;
        } else if (parseInt($(app.DE.leftStartRange).val()) == 0) {
            $(app.DE.leftStartRange).addClass('NotValid');
            //
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_037);//Start range  can not be zero!  
            return false;
        } else if (parseInt($(app.DE.leftStartRange).val()) > parseInt($(app.DE.leftEndRange).val())) {
            $(app.DE.leftStartRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_038);//Start range can not be greater than end range!
            return false;
        }

        if ($(app.DE.leftEndRange).val() == '') {
            $(app.DE.leftEndRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_039);//Please enter the end range!
            return false;
        } else if (parseInt($(app.DE.leftEndRange).val()) == 0) {
            $(app.DE.leftEndRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_040);//End range can not be zero!
            return false;
        }

        if ($(app.DE.rightStartRange).val() == '') {
            $(app.DE.rightStartRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_036);//Please enter the start range!
            return false;
        } else if (parseInt($(app.DE.rightStartRange).val()) == 0) {
            $(app.DE.rightStartRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_037);//Start range  can not be zero!
            return false;
        } else if (parseInt($(app.DE.rightStartRange).val()) > parseInt($(app.DE.rightEndRange).val())) {
            $(app.DE.rightStartRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_038);//Start range can not be greater than end range!
            return false;
        }

        if ($(app.DE.rightEndRange).val() == '') {
            $(app.DE.rightEndRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_039);//Please enter the end range!
            return false;
        } else if (parseInt($(app.DE.rightEndRange).val()) == 0) {
            $(app.DE.rightEndRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_040);//End range can not be zero!
            return false;
        }

        if (parseInt($(app.DE.rightStartRange).val()) > $('#dvRightCable .rightFiber').length) {
            $(app.DE.rightStartRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_041);//Start range can not be greater than  total cable cores!
            return false;
        } else if (parseInt($(app.DE.rightEndRange).val()) > $('#dvRightCable .rightFiber').length) {
            $(app.DE.rightEndRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_042);//End range can not be greater than  total cable cores!
            return false;
        }

        if (connectorType == 'ODF' || connectorType == 'FMS' || connectorType == 'HTB') {
            if (parseInt($(app.DE.leftEndRange).val()) > $('.DeviceView .leftFiber').length) {
                $(app.DE.leftEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_043);//Start range can not be greater than total ports!
                return false;
            } else if (parseInt($(app.DE.leftEndRange).val()) > $('.DeviceView .leftFiber').length) {
                $(app.DE.leftEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_044);//End range can not be greater than  total ports!
                return false;
            } else if ($('.DeviceView .leftFiber .src[data-is-connected="0"]').length == 0) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_045);//All ODF/FMS ports are occupied!
                return false;

            } else if ($('#dvRightCable .rightFiber .trg[data-is-connected="0"]').length == 0) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_046);//All cable cores are occupied!
                return false;
            }
        } else {
            if (parseInt($(app.DE.leftStartRange).val()) > $('#dvLeftCable .leftFiber').length) {
                $(app.DE.leftStartRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_041);//Start range can not be greater than  total cable cores!
                return false;
            } else if (parseInt($(app.DE.leftEndRange).val()) > $('#dvLeftCable .leftFiber').length) {
                $(app.DE.leftEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_042);//End range can not be greater than  total cable cores!
                return false;
            } else if ($('#dvLeftCable .leftFiber .src[data-is-connected="0"]').length == 0) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_047);//All cores are occupied of left cable!
                return false;

            } else if ($('#dvRightCable .rightFiber .trg[data-is-connected="0"]').length == 0) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_048);//All cores are occupied of right cable!
                return false;
            }
        }

        if ($(app.DE.ddlSpliceTray).val() != '') {
            var counter = 0;
            var leftStartRange = parseInt($(app.DE.leftStartRange).val());
            var leftEndRange = parseInt($(app.DE.leftEndRange).val());
            var rightStartRange = parseInt($(app.DE.rightStartRange).val());
            var rightEndRange = parseInt($(app.DE.rightEndRange).val());
            for (var i = leftStartRange; i <= leftEndRange; i++) {
                if ($('#dvLeftCable .leftFiber .src[data-is-connected="0"][data-status-id="1"][data-port-no="' + i + '"]').length > 0 && $('#dvRightCable .rightFiber .trg[data-is-connected="0"][data-port-no="' + rightStartRange + '"]').length > 0) {
                    counter++;
                }
                if (rightStartRange == rightEndRange) { break; }
                rightStartRange++;
            }
            var totalTrayPort = parseInt($(app.DE.ddlSpliceTray + ' :selected').attr('data-total-ports'));
            var usedPort = parseInt($(app.DE.ddlSpliceTray + ' :selected').attr('data-used-ports'));
            if (counter > (totalTrayPort - usedPort)) {
                var Msg = 'Selected tray does not have the sufficient ports for this range! <br/><table border="1" class="alertgrid">';
                Msg += '<tr><td>Total Ports </td><td> ' + totalTrayPort + '</td></tr>';
                Msg += '<tr><td>Used Ports </td><td> ' + usedPort + '</td></tr>';
                Msg += '<tr><td>Available Ports </td><td> ' + (totalTrayPort - usedPort) + '</td></tr>';
                Msg += '</table>';
                alert(Msg);

                return false;
            }
        }
        return true;
    }
    this.isUnSplicingRangeValid = function () {
        var connectorType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');

        if ($(app.DE.leftStartRange).val() == '' && $(app.DE.leftEndRange).val() == '' && $(app.DE.rightStartRange).val() == '' && $(app.DE.rightEndRange).val() == '') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_049);//Please enter the valid range to un-splice the connections!
            return false;

        } else if (parseInt($(app.DE.leftStartRange).val()) == 0 && parseInt($(app.DE.leftEndRange).val()) == 0 && parseInt($(app.DE.rightStartRange).val()) == 0 && parseInt($(app.DE.rightEndRange).val()) == 0) {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_049);//Please enter the valid range to un-splice the connections!
            return false;
        }

        if ($(app.DE.leftStartRange).val() != '' || $(app.DE.leftEndRange).val() != '') {
            if ($(app.DE.leftStartRange).val() == '') {
                $(app.DE.leftStartRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_036);//Please enter the start range!
                return false;
            } else if ($(app.DE.leftEndRange).val() == '') {
                $(app.DE.leftEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_039);//Please enter the end range!
                return false;
            } else if (parseInt($(app.DE.leftStartRange).val()) == 0) {
                $(app.DE.leftStartRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_037);//Start range  can not be zero!  
                return false;
            } else if (parseInt($(app.DE.leftEndRange).val()) == 0) {
                $(app.DE.leftEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_040);//End range can not be zero!
                return false;
            } else if (parseInt($(app.DE.leftStartRange).val()) > parseInt($(app.DE.leftEndRange).val())) {
                $(app.DE.leftStartRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_038);//Start range can not be greater than end range!
                return false;
            }
        }

        if ($(app.DE.rightStartRange).val() != '' || $(app.DE.rightEndRange).val() != '') {
            if ($(app.DE.rightStartRange).val() == '') {
                $(app.DE.rightStartRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_036);//Please enter the start range!
                return false;
            }
            else if ($(app.DE.rightEndRange).val() == '') {
                $(app.DE.rightEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_039);//Please enter the end range!
                return false;
            }
            else if (parseInt($(app.DE.rightStartRange).val()) == 0) {
                $(app.DE.rightStartRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_037);//Start range  can not be zero!  
                return false;
            } else if (parseInt($(app.DE.rightEndRange).val()) == 0) {
                $(app.DE.rightEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_040);//End range can not be zero!
                return false;
            } else if (parseInt($(app.DE.rightStartRange).val()) > parseInt($(app.DE.rightEndRange).val())) {
                $(app.DE.rightStartRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_038);//Start range can not be greater than end range!
                return false;
            }
        }

        if (parseInt($(app.DE.rightStartRange).val()) > $('#dvRightCable .rightFiber').length) {
            $(app.DE.rightStartRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_041);//Start range can not be greater than  total cable cores!
            return false;
        } else if (parseInt($(app.DE.rightEndRange).val()) > $('#dvRightCable .rightFiber').length) {
            $(app.DE.rightEndRange).addClass('NotValid');
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_042);//End range can not be greater than  total cable cores!
            return false;
        }

        if (connectorType == 'ODF' || connectorType == 'FMS' || connectorType == 'HTB') {
            if (parseInt($(app.DE.leftEndRange).val()) > $('.DeviceView .leftFiber').length) {
                $(app.DE.leftEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_043);//Start range can not be greater than total ports!
                return false;
            } else if (parseInt($(app.DE.leftEndRange).val()) > $('.DeviceView .leftFiber').length) {
                $(app.DE.leftEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_044);//End range can not be greater than  total ports!
                return false;
            }
        } else {
            if (parseInt($(app.DE.leftStartRange).val()) > $('#dvLeftCable .leftFiber').length) {
                $(app.DE.leftStartRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_041);//Start range can not be greater than  total cable cores!
                return false;
            } else if (parseInt($(app.DE.leftEndRange).val()) > $('#dvLeftCable .leftFiber').length) {
                $(app.DE.leftEndRange).addClass('NotValid');
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_042);//End range can not be greater than  total cable cores!
                return false;
            }
        }
        return true;
    }
    this.bindCoreInfoWindow = function () {
         
        if ($('#hdnIsSplicingEditEnabled').val() == 'True') {
            //document.addEventListener('contextmenu', event => event.preventDefault());
            $('div[data-is-connected-to-same]').bind("click", function (e) {
                $('.rel_pop').remove();
                $(this).parent().append($(app.DE.splicingInfowindow).html());
                $('#btnCloseSplicingInfo').on("click", function () { $('.rel_pop').hide(); })
                $('.rel_pop').show();
                if ($('.rel_pop').parent().hasClass('rightFiber')) {
                    var entityType = $(this).attr('data-entity-type')
                    $('.rel_pop').css('margin-left', entityType == 'Customer' ? '-95px' : '-65px');
                    $('.fa-caret-left').addClass('fa-caret-right').css({ 'left': '95%' });
                }
                 
                app.entityInfo = $(e.currentTarget).data();
                var PortStatusId = parseInt($(this).attr('data-status-id'));
                var portcommont = $(this).attr('data-port-comment');
                var portStatus = $(this).attr('data-port-status');
                $('#btnConnectionPath').click({ networkId: app.entityInfo.networkId, entityType: app.entityInfo.entityType, systemId: app.entityInfo.systemId, portNo: app.entityInfo.portNo, isControllEnable: false, isParentPopup: false, entityName: null }, app.redirectToCPF)
                $('#btnConnectedCustomerDetails').click({ networkId: app.entityInfo.networkId, entityType: app.entityInfo.entityType, systemId: app.entityInfo.systemId, portNo: app.entityInfo.portNo, isControllEnable: false, isParentPopup: false, entityName: null }, app.redirectToCustomerDetails)
                //$('#btnConnectedlinkInfo').click({ networkId: app.entityInfo.networkId, entityType: app.entityInfo.entityType, systemId: app.entityInfo.systemId, portNo: app.entityInfo.portNo, isControllEnable: false, isParentPopup: false, entityName: null }, app.redirectToLinkInfo)
                $('#btncoreportupdate').click(
                    //app.redirectToUpdateFiber(app.entityInfo.systemId,app.entityInfo.portNo,app.entityInfo.entityType,"")
                    { pEntitySystemId: parseInt(app.entityInfo.systemId), pCorePortNumber: app.entityInfo.portNo, pEntityType: app.entityInfo.entityType, ptype: "SPLICING", isGridCalling: false, pPortStatusId: PortStatusId, pPortComment: portcommont, pPortStatus: portStatus }, app.redirectToUpdateFiber
                )
                //this.redirectToUpdateFiber = function (pEntitySystemId, pCorePortNumber, pEntityType,ptype) {
                $('.rel_pop').css({ 'top': ((e.pageY) - 50) });
                //if (parseInt($(this).attr('data-status-id')) == 2) { $('#btncoreportupdate').hide(); }
                if (app.entityInfo.entityType.toUpperCase() == 'CABLE') {
                    var varotherEndid = '#Other_' + app.entityInfo.systemId + '_CABLE_' + app.entityInfo.portNo;
                    //if (parseInt($(varotherEndid).attr('data-is-connected')) == 1) { $('#btncoreportupdate').hide(); }
                }
            })
        }
    }
    this.toggleConnectionUpload = function () {
        var pageUrl = 'Splicing/UploadConnection';
        var modalClass = 'modal-sm';
        popup.LoadModalDialog('PARENT', pageUrl, {}, 'Bulk Splicing', modalClass);

    }

    this.ExportConnections = function (exportType, exportKey) {
        showProgress();
        if (exportKey == "C2C") {
            //var sourceId = $(app.DE.SourceCable).val();

            //var connectorId = $(app.DE.ConnectingEntity).val();
            //var destinationId = $(app.DE.DestinationCable).val();
            //var eType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');   //SpliceClosure

            //var source_type = $(app.DE.SourceCable + ' :selected').attr('data-entity-type');
            //var is_source_connected = $(app.DE.SourceCable + ' :selected').attr('data-is-start-point');
            //var connecting_entity = '[{"system_id":' + connectorId + ',"entity_type":"' + eType + '"}]';
            //var destination_system_id = destinationId;
            //var destination_type = $(app.DE.DestinationCable + ' :selected').attr('data-entity-type');
            //var is_destination_connected = $(app.DE.SourceCable + ' :selected').attr('data-is-start-point');
            //var customer_Ids = '';
            // 
            ////Get Image from div and save it in file system and then send it for export excel file with image
            ////document.body.style.zoom = "40%"; //app.GetZoomPercentage($('.mainContainer').width());
            //var targetElem = $('.mainContainer');
            //var elements = targetElem.find('svg').map(function () {
            //    var svg = $(this);
            //    var canvas = $('<canvas></canvas>').css({ position: 'absolute', fill: 'none', left: svg.css('left'), top: svg.css('top') });
            //    svg.replaceWith(canvas);
            //    //  Get the raw SVG string and curate it
            //    var content = svg.wrap('<p></p>').parent().html();
            //    svg.unwrap();
            //    canvg(canvas[0], content);
            //    return {
            //        svg: svg,
            //        canvas: canvas
            //    };
            //});

            //// At this point the container has no SVG, it only has HTML and Canvases.
            //html2canvas($(targetElem)[0], {
            //    width: $(targetElem)[0].scrollWidth, height: $(targetElem)[0].scrollHeight, quality: 2, scale: 2
            //}, { allowTaint: true, useCORS: true }).then(function (canvas) {
            //    // Put the SVGs back in place
            //    elements.each(function () {
            //        this.canvas.replaceWith(this.svg);
            //    });
            //    //document.body.style.zoom ="100%";
            //    var base64encodedstring = canvas.toDataURL("image/png", 1);
            //    var canvasimgdata = base64encodedstring.replace(/^data:image\/(png|jpg);base64,/, "");
            //    //Save Image in filesystem and get fullpath of image
            //    ajaxReq('Splicing/SaveCaptureImage', { 'imgdata': canvasimgdata }, true, function (resp) {
            //        if (resp != null && resp.status == true) {

            //            //Get Excel file with Image
            //            window.location = appRoot + 'Splicing/ExportConnections?source_system_id='
            //                + sourceId + '&source_type='
            //                + source_type + '&is_source_connected='
            //                + is_source_connected + '&connecting_entity='
            //                + connecting_entity + '&destination_system_id='
            //                + destination_system_id + '&destination_type='
            //                + destination_type + '&is_destination_connected='
            //                + is_destination_connected + '&image_path='
            //                + resp.file + '&exportType=' + exportType
            //                + '&exportKey=' + exportKey
            //                + '&customer_Ids=' + customer_Ids;

            //            hideProgress();
            //        }
            //    }, false, true, false);
            //});

            var controlId = "mainContainer";
            var targetElem = $("#" + controlId);
            //this is for handle svg image
            var elements = targetElem.find('svg').map(function () {
                var svg = $(this);
                var canvas = $('<canvas></canvas>').css({ position: 'absolute', fill: 'none', left: svg.css('left'), top: svg.css('top') });
                svg.replaceWith(canvas);
                // Get the raw SVG string and curate it
                var content = svg.wrap('<p></p>').parent().html();
                svg.unwrap();
                canvg(canvas[0], content);
                return {
                    svg: svg,
                    canvas: canvas
                };
            });
            //end
            var HTML_Width = $(targetElem).width();
            var HTML_Height = $(targetElem)[0].scrollHeight;

            var top_left_margin = 15;
            var PDF_Width = HTML_Width + (top_left_margin * 2);
            var PDF_Height = HTML_Height + (top_left_margin * 2);
            var canvas_image_width = HTML_Width;
            var canvas_image_height = HTML_Height;

            var totalPDFPages = Math.ceil(HTML_Height / PDF_Height) - 1;
             
            // At this point the container has no SVG, it only has HTML and Canvases.
            html2canvas($(targetElem)[0], {
                width: $(targetElem)[0].scrollWidth, height: $(targetElem)[0].scrollHeight, quality: 2, scale: 2
            }, { allowTaint: true, useCORS: true }).then(function (canvas) {
                // Put the SVGs back in place
                elements.each(function () {
                    this.canvas.replaceWith(this.svg);
                });
                //console.log(canvas.height + " " + canvas.width);
                canvas.getContext('2d');
                var imgData = canvas.toDataURL("image/png", 1.0);
                //window.location.href = imgData.replace("image/png", "image/octet-stream"); // it will save locally
                var sourceId = $(app.DE.SourceCable).val();

                var connectorId = $(app.DE.ConnectingEntity).val();
                var destinationId = $(app.DE.DestinationCable).val();
                var eType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');   //SpliceClosure

                var source_type = $(app.DE.SourceCable + ' :selected').attr('data-entity-type');
                var is_source_connected = $(app.DE.SourceCable + ' :selected').attr('data-is-start-point');
                var connecting_entity = '[{"system_id":' + connectorId + ',"entity_type":"' + eType + '"}]';
                var destination_system_id = destinationId;
                var destination_type = $(app.DE.DestinationCable + ' :selected').attr('data-entity-type');
                var is_destination_connected = $(app.DE.SourceCable + ' :selected').attr('data-is-start-point');
                var customer_Ids = '';
                 

                var canvasimgdata = imgData.replace(/^data:image\/(png|jpg);base64,/, "");
                //Save Image in filesystem and get fullpath of image
                ajaxReq('Splicing/SaveCaptureImage', { 'imgdata': canvasimgdata }, true, function (resp) {
                    if (resp != null && resp.status == true) {

                        //Get Excel file with Image
                        window.location = appRoot + 'Splicing/ExportConnections?source_system_id='
                            + sourceId + '&source_type='
                            + source_type + '&is_source_connected='
                            + is_source_connected + '&connecting_entity='
                            + connecting_entity + '&destination_system_id='
                            + destination_system_id + '&destination_type='
                            + destination_type + '&is_destination_connected='
                            + is_destination_connected + '&image_path='
                            + resp.file + '&exportType=' + exportType
                            + '&exportKey=' + exportKey
                            + '&customer_Ids=' + customer_Ids;

                        hideProgress();
                    }
                }, false, true, false);

            });
        }

        else if (exportKey == "CPE2C") {
            var sourceId = 0;

            var source_type = '';
            var is_source_connected = false;
            var connecting_entity = '[';
            $.each($('#ddlCPEEntity :selected'), function (index, item) {
                connecting_entity += '{"system_id":' + parseInt($(item).val()) + ',"entity_type":"' + $(item).data().entityType + '"}';
                connecting_entity += ',';
            })
            connecting_entity = connecting_entity.substring(0, connecting_entity.length - 1) + ']';
            var destination_system_id = 0;
            var destination_type = '';
            var is_destination_connected = false;
            var customer_Ids = $(app.DE.ddlcustomer).val().join(',');

            //Get Image from div and save it in file system and then send it for export excel file with image
            var targetElem = $('.mainContainer');
            var elements = targetElem.find('svg').map(function () {
                var svg = $(this);
                var canvas = $('<canvas></canvas>').css({ position: 'absolute', fill: 'none', left: svg.css('left'), top: svg.css('top') });
                svg.replaceWith(canvas);
                //  Get the raw SVG string and curate it
                var content = svg.wrap('<p></p>').parent().html();
                svg.unwrap();
                canvg(canvas[0], content);
                return {
                    svg: svg,
                    canvas: canvas
                };
            });

            // At this point the container has no SVG, it only has HTML and Canvases.
            html2canvas($(targetElem)[0], {
                allowTaint: true, useCORS: true, logging: false, height: window.outerHeight + window.innerHeight, windowHeight: window.outerHeight + window.innerHeight
            }).then(function (canvas) {
                // Put the SVGs back in place
                elements.each(function () {
                    this.canvas.replaceWith(this.svg);
                });

                var base64encodedstring = canvas.toDataURL("image/png", 1);

                var canvasimgdata = base64encodedstring.replace(/^data:image\/(png|jpg);base64,/, "");
                //window.location.href = canvasimgdata;
                //Save Image in filesystem and get fullpath of image
                ajaxReq('Splicing/SaveCaptureImage', { 'imgdata': canvasimgdata }, true, function (resp) {
                    if (resp != null && resp.status == true) {

                        //Get Excel file with Image
                        window.location = appRoot + 'Splicing/ExportConnections?source_system_id='
                            + sourceId + '&source_type='
                            + source_type + '&is_source_connected='
                            + is_source_connected + '&connecting_entity='
                            + connecting_entity + '&destination_system_id='
                            + destination_system_id + '&destination_type='
                            + destination_type + '&is_destination_connected='
                            + is_destination_connected + '&image_path='
                            + resp.file + '&exportType=' + exportType
                            + '&exportKey=' + exportKey
                            + '&customer_Ids=' + customer_Ids;

                        hideProgress();
                    }
                }, false, true, false);
            });
        }
        else if (exportKey == "FMS2C") {
            var sourceId = 0;
            var source_type = '';
            var is_source_connected = false;
            var isCableAEnd = JSON.parse($(app.DE.DestinationCable + ' :selected').val() != '0' ? $(app.DE.DestinationCable + ' :selected').attr('data-is-start-point').toLowerCase() : 'false');
            var connecting_entity = '[{"system_id":' + parseInt($(app.DE.ConnectingEntity).val()) + ',"entity_type":"' + $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type') + '"}]';
            var destination_system_id = parseInt($(app.DE.DestinationCable).val());
            var destination_type = 'Cable';
            var is_destination_connected = (isCableAEnd ? isCableAEnd : false);
            var customer_Ids = '';

            //Get Image from div and save it in file system and then send it for export excel file with image
            var targetElem = $('.mainContainer');
            var elements = targetElem.find('svg').map(function () {
                var svg = $(this);
                var canvas = $('<canvas></canvas>').css({ position: 'absolute', fill: 'none', left: svg.css('left'), top: svg.css('top') });
                svg.replaceWith(canvas);
                //  Get the raw SVG string and curate it
                var content = svg.wrap('<p></p>').parent().html();
                svg.unwrap();
                canvg(canvas[0], content);
                return {
                    svg: svg,
                    canvas: canvas
                };
            });

            // At this point the container has no SVG, it only has HTML and Canvases.
            html2canvas($(targetElem)[0], {
                allowTaint: true, useCORS: true, logging: false, height: window.outerHeight + window.innerHeight + $(targetElem)[0].scrollHeight, windowHeight: window.outerHeight + window.innerHeight + $(targetElem)[0].scrollHeight
            }).then(function (canvas) {
                // Put the SVGs back in place
                elements.each(function () {
                    this.canvas.replaceWith(this.svg);
                });

                var base64encodedstring = canvas.toDataURL("image/png", 1);
                var canvasimgdata = base64encodedstring.replace(/^data:image\/(png|jpg);base64,/, "");
                //Save Image in filesystem and get fullpath of image
                ajaxReq('Splicing/SaveCaptureImage', { 'imgdata': canvasimgdata }, true, function (resp) {
                    if (resp != null && resp.status == true) {

                        //Get Excel file with Image
                        window.location = appRoot + 'Splicing/ExportConnections?source_system_id='
                            + sourceId + '&source_type='
                            + source_type + '&is_source_connected='
                            + is_source_connected + '&connecting_entity='
                            + connecting_entity + '&destination_system_id='
                            + destination_system_id + '&destination_type='
                            + destination_type + '&is_destination_connected='
                            + is_destination_connected + '&image_path='
                            + resp.file + '&exportType=' + exportType
                            + '&exportKey=' + exportKey
                            + '&customer_Ids=' + customer_Ids;

                        hideProgress();
                    }
                }, false, true, false);
            });
        }
    }


    //this.ExportConnections = function (exportType, exportKey) {
    //    showProgress();
    //    if (exportKey == "C2C") {
    //        var controlId = "mainContainer";
    //        var targetElem = $("#" + controlId);
    //        //this is for handle svg image
    //        var elements = targetElem.find('svg').map(function () {
    //            var svg = $(this);
    //            var canvas = $('<canvas></canvas>').css({ position: 'absolute', fill: 'none', left: svg.css('left'), top: svg.css('top') });
    //            svg.replaceWith(canvas);
    //            // Get the raw SVG string and curate it
    //            var content = svg.wrap('<p></p>').parent().html();
    //            svg.unwrap();
    //            canvg(canvas[0], content);
    //            return {
    //                svg: svg,
    //                canvas: canvas
    //            };
    //        });
    //        //end
    //        var HTML_Width = $(targetElem).width();
    //        var HTML_Height = $(targetElem)[0].scrollHeight;

    //        var top_left_margin = 15;
    //        var PDF_Width = HTML_Width + (top_left_margin * 2);
    //        var PDF_Height = HTML_Height + (top_left_margin * 2);
    //        var canvas_image_width = HTML_Width;
    //        var canvas_image_height = HTML_Height;

    //        var totalPDFPages = Math.ceil(HTML_Height / PDF_Height) - 1;
    //         
    //        // At this point the container has no SVG, it only has HTML and Canvases.
    //        html2canvas($(targetElem)[0], {
    //            width: $(targetElem)[0].scrollWidth, height: $(targetElem)[0].scrollHeight, quality: 2, scale: 2
    //        }, { allowTaint: true, useCORS: true }).then(function (canvas) {
    //            // Put the SVGs back in place
    //            elements.each(function () {
    //                this.canvas.replaceWith(this.svg);
    //            });
    //            //console.log(canvas.height + " " + canvas.width);
    //            canvas.getContext('2d');
    //            var imgData = canvas.toDataURL("image/png", 1.0);
    //            //window.location.href = imgData.replace("image/png", "image/octet-stream"); // it will save locally
    //            var sourceId = $(app.DE.SourceCable).val();

    //            var connectorId = $(app.DE.ConnectingEntity).val();
    //            var destinationId = $(app.DE.DestinationCable).val();
    //            var eType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');   //SpliceClosure

    //            var source_type = $(app.DE.SourceCable + ' :selected').attr('data-entity-type');
    //            var is_source_connected = $(app.DE.SourceCable + ' :selected').attr('data-is-start-point');
    //            var connecting_entity = '[{"system_id":' + connectorId + ',"entity_type":"' + eType + '"}]';
    //            var destination_system_id = destinationId;
    //            var destination_type = $(app.DE.DestinationCable + ' :selected').attr('data-entity-type');
    //            var is_destination_connected = $(app.DE.SourceCable + ' :selected').attr('data-is-start-point');
    //            var customer_Ids = '';
    //             

    //            var canvasimgdata = imgData.replace(/^data:image\/(png|jpg);base64,/, "");
    //            //Save Image in filesystem and get fullpath of image
    //            ajaxReq('Splicing/SaveCaptureImage', { 'imgdata': canvasimgdata }, true, function (resp) {
    //                if (resp != null && resp.status == true) {

    //                    //Get Excel file with Image
    //                    window.location = appRoot + 'Splicing/ExportConnections?source_system_id='
    //                        + sourceId + '&source_type='
    //                        + source_type + '&is_source_connected='
    //                        + is_source_connected + '&connecting_entity='
    //                        + connecting_entity + '&destination_system_id='
    //                        + destination_system_id + '&destination_type='
    //                        + destination_type + '&is_destination_connected='
    //                        + is_destination_connected + '&image_path='
    //                        + resp.file + '&exportType=' + exportType
    //                        + '&exportKey=' + exportKey
    //                        + '&customer_Ids=' + customer_Ids;

    //                    hideProgress();
    //                }
    //            }, false, true, false);

    //        });
    //    }
    //};
    this.GetZoomPercentage = (svgSize) => {
         
        var windowWidth = window.innerWidth;
        var percentage = (svgSize / windowWidth) * 100;
        if (percentage > 100)
            return 100 + "%";
        return percentage - 10 + "%";
    }
    /*CPF AND END 2 END SCHEMATIC*/
    this.SinglePathFind = function (fromISP) {
        $("#wLineTBar").find(".activeToolBar").removeClass('activeToolBar');
        popup.LoadModalDialog('PARENT', 'Splicing/ConnectionPathFinder', { eType: '' }, 'Connection Path Finder', 'modal-xxl', function () {
            if (fromISP) {
                $('#btnShowOnMap').hide();
            }
        });
    }
    this.BindEquipementPort = function (entityid, entitytype) {
        var ddlCore = $(app.DE.ddlCore);
        ajaxReq('Splicing/GetEquipmentPortInfo', { entity_id: entityid, entity_type: entitytype }, false, function (resp) {
            if (resp.status = 'OK') {
                $(app.DE.equipmentid).removeClass('input-validation-error');
                $("#ddlCore_chosen").removeClass('input-validation-error');
                ddlCore.empty();
                ddlCore.append($("<option></option>").val('0').html('--Select--'));
                $.each(resp.result, function (data, value) {
                    ddlCore.append($("<option></option>").val(value.port_value).html(value.port_text));
                });
            }
            else {
                $("#ddlCore_chosen").addClass('input-validation-error');
                alert(resp.message);
            }
        }, true, false);
        ddlCore.trigger("chosen:updated");
    }
    this.clearCPFMarker = function () {
        app.clearUserTempOverlay(gMapObj.TraceRoute);
        gMapObj.TraceRoute = [];
        if (app.oms != null) { app.oms.removeAllMarkers(); }
    }
    this.clearUserTempOverlay = function (lyrObj) {
        if (lyrObj) {
            for (var i = 0; i < lyrObj.length; i++)
                lyrObj[i].setMap(null);
        }
        lyrObj = [];
    }
    this.ddlcpfCore = function ddlcpfCore(obj) {
        var selectedValue = $('#ddlCore option:selected').val();
        if (selectedValue == '0') {
            $(app.DE.port_no).val('0');
            $("#ddlCore_chosen").addClass('input-validation-error');
        }
        else {
            $(app.DE.port_no).val(selectedValue);
            $("#ddlCore_chosen").removeClass('input-validation-error');
        }
    }


    this.showFiberLinkOnMap = function (_linkSystemID) {
         

        ajaxReq('FiberLink/showFiberLinkOnMap', { linkSystemID: _linkSystemID }, true, function (resp) {
            if (resp.status = 'OK') {
                 
                if (resp.result != null && resp.result != undefined) {
                    $(popup.DE.MinimizeModel).trigger("click");
                    app.clearUserTempOverlay(gMapObj.TraceRoute);
                    gMapObj.TraceRoute = [];
                    app.gMapObj.pointMarkers = [];
                    var bounds = new google.maps.LatLngBounds();
                    var highlightLineList = [];
                    var oms = new OverlappingMarkerSpiderfier(si.map, {
                        markersWontMove: true,
                        markersWontHide: true,
                        keepSpiderfied: true,
                        circleFootSeparation: 35, // radius of circle 
                        nearbyDistance: 30, // distance in which it include the markers in spiderfier..
                        circleSpiralSwitchover: Infinity,//infinity= circle format, 0= spiral format with 9 element in one spiral round
                        legWeight: 2.4
                    });
                    var objInfoWindow = new google.maps.InfoWindow();
                    var infowindow = new google.maps.InfoWindow({
                        content: contentString
                    });
                    if (resp.result.lstCableInfo != null || resp.result.lstCableInfo != undefined) {

                        for (var i = 0; i < resp.result.lstCableInfo.length; i++) {

                            if (resp.result.lstCableInfo[i].cable_geom != null) {
                                var geometry = getLatLongArr(resp.result.lstCableInfo[i].cable_geom);
                                for (var z = 0; z < geometry.length; z++) {
                                    bounds.extend(geometry[z]);
                                }
                                var lineObj = si.createLine(geometry);
                                lineObj.strokeColor = 'blue';
                                var _lineIcon = [{
                                    icon: {
                                        path: 'M -.5,-.5 .5,-.5, .5,.5 -.5,.5',
                                        fillOpacity: 1
                                    },
                                    repeat: '6px'
                                }];

                                lineObj.strokeOpacity = 0;
                                lineObj.strokeWeight = 4;
                                lineObj.set('icons', _lineIcon);
                                highlightLineList.push(lineObj);

                                var contentString = '<div id="content">' +
                                    '<input type="button" id="export" enType="' + 'Cable' + '" systemId="' + resp.result.lstCableInfo[i].cable_system_id + '"  value="export"/>' +
                                    '</div>';

                                google.maps.event.addListener(lineObj, 'click', (function (lineObj, i) {

                                    var _cableNetworkId = resp.result.lstCableInfo[i].cable_network_id;
                                    var _cableFiberNumber = resp.result.lstCableInfo[i].fiber_number;
                                    return function () {

                                        var content = '<div><h4>' + 'Cable' + ' Detail </h3><p><span  class="Info-content">Network Id : </span>' + _cableNetworkId + '</p>';
                                        content += '<p><span  class="Info-content">Port No :</span> ' + _cableFiberNumber + '</p>';
                                        objInfoWindow.setContent(content);
                                        objInfoWindow.open(si.map, lineObj);
                                    }
                                })(lineObj, i));

                                //si.map.data.addListener("mouseover", event => {
                                //    // 
                                //    app._focusMe('Line', event.feature.getGeometry().i, 'Cable', null);
                                //});

                                //si.map.data.addListener("mouseout", event => {
                                //    si.RemoveOldInfoWindow();
                                //    si.removeInfoHoverItem();
                                //});

                                lineObj.setMap(si.map);
                                gMapObj.TraceRoute.push(lineObj);
                            }
                        }

                    }
                    if (resp.result.lstConnectedElements != null || resp.result.lstConnectedElements != undefined) {
                        for (var i = 0; i < resp.result.lstConnectedElements.length; i++) {

                            var en_type = resp.result.lstConnectedElements[i].connected_entity_type;
                            var system_id = resp.result.lstConnectedElements[i].connected_system_id;
                            var port_no = resp.result.lstConnectedElements[i].connected_port_no;
                            var network_id = resp.result.lstConnectedElements[i].connected_network_id;
                            var is_virtual_port_allowed = resp.result.lstConnectedElements[i].is_virtual_port_allowed;
                            var geometry = getLatLongArr(resp.result.lstConnectedElements[i].connected_entity_geom);

                            var ptObj = null;
                            if (resp.result.lstConnectedElements[i].connected_entity_category != null && resp.result.lstConnectedElements[i].connected_entity_category != '') {
                                ptObj = si.createMarkerForPathFinder(geometry[0], 'Content/images/icons/lib/circle/' + resp.result.lstConnectedElements[i].connected_entity_category + '_' + resp.result.lstConnectedElements[i].connected_entity_type.toUpperCase() + '.png', system_id, en_type, port_no, network_id, is_virtual_port_allowed);
                            }
                            else {
                                ptObj = si.createMarkerForPathFinder(geometry[0], 'Content/images/icons/lib/circle/' + (resp.result.lstConnectedElements[i].is_virtual ? 'v_' : '') + resp.result.lstConnectedElements[i].connected_entity_type + '.png', system_id, en_type, port_no, network_id, is_virtual_port_allowed);
                            }
                            // add info window
                            var contentString = '<div id="content">' +
                                '<input type="button" id="export" enType="' + en_type + '" systemId="' + system_id + '"  value="export"/>' +
                                '</div>';
                            google.maps.event.addListener(ptObj, 'click', (function (ptObj, i) {
                                if (ptObj.portNo != null) {
                                    var PortNo = ptObj.portNo > 0 ? ptObj.portNo.toString() + " OUT" : ptObj.portNo.toString().replace('-', '') + " IN";
                                }
                                return function () {

                                    var content = '<div><h4>' + ptObj.eType + ' Detail </h3><p><span  class="Info-content">Network Id : </span>' + ptObj.networkId + '</p>';
                                    if (!ptObj.isVirtualPortAllowed) {
                                        content += '<p><span  class="Info-content">Port No :</span> ' + PortNo + '</p>';
                                    }
                                    objInfoWindow.setContent(content);
                                    objInfoWindow.open(si.map, ptObj);
                                }
                            })(ptObj, i));

                            ptObj.setMap(si.map);
                            gMapObj.TraceRoute.push(ptObj);
                            app.gMapObj.pointMarkers.push(ptObj);
                            oms.addMarker(ptObj);

                        }
                    }
                    // for highlight the  Path..
                    $.each(highlightLineList, function (index, item) {
                        setTimeout(function () { animateLine(item, 0) }, 20);
                    });

                    si.map.fitBounds(bounds);
                }
            }
        }, true, true);
    }

    this.showPath = function () {
         
        //alert('Entry Splice without osp');
        ajaxReq('Splicing/GetCPFelementPath', {}, true, function (resp) {
            if (resp.status = 'OK') {
                if (resp.result != null && resp.result != undefined) {
                    $(popup.DE.MinimizeModel).trigger("click");
                    //  plotSinglePath(resp, '#ff0000', true)
                    var isBoth = true;
                    var _color = '#ff0000';
                    //if (gMapObj.TraceRoute && !isBoth)
                    app.clearUserTempOverlay(gMapObj.TraceRoute);

                    //if (!isBoth)    // for both path not clear TraceRoute and add into it..
                    gMapObj.TraceRoute = [];
                    app.gMapObj.pointMarkers = [];
                    var bounds = new google.maps.LatLngBounds();


                    var highlightLineList = [];
                    app.oms = new OverlappingMarkerSpiderfier(si.map, {
                        markersWontMove: true,
                        markersWontHide: true,
                        keepSpiderfied: true,
                        circleFootSeparation: 35, // radius of circle 
                        nearbyDistance: 30, // distance in which it include the markers in spiderfier..
                        circleSpiralSwitchover: Infinity,//infinity= circle format, 0= spiral format with 9 element in one spiral round
                        legWeight: 2.4
                    });
                    for (var i = 0; i < resp.result.length; i++) {
                        _color = '#ff0000';
                         
                        var entityCount = resp.result.filter(function (e) { return e.system_id == resp.result[i].system_id; }).length;
                        //if (entityCount <= 1) {
                        if (resp.result[i].en_type == 'Cable') {


                            //backward_path
                            var geometry = getLatLongArr(resp.result[i].sp_geometry);
                            for (var z = 0; z < geometry.length; z++) {
                                bounds.extend(geometry[z]);
                            }
                            var lineObj = si.createLine(geometry);
                            if (resp.result[i].backward_path == true) {
                                _color = 'blue';
                            }
                            lineObj.strokeColor = _color;
                            var _lineIcon = [{
                                icon: {
                                    path: 'M -.5,-.5 .5,-.5, .5,.5 -.5,.5',
                                    fillOpacity: 1
                                    //fillColor: 'blue'
                                },
                                repeat: '6px'
                            }];

                            lineObj.strokeOpacity = 0;
                            lineObj.strokeWeight = 4;
                            lineObj.set('icons', _lineIcon);
                            highlightLineList.push(lineObj);
                            lineObj.setMap(si.map);
                            gMapObj.TraceRoute.push(lineObj);
                        }
                        else {

                            var en_type = resp.result[i].en_type;
                            var system_id = resp.result[i].system_id;
                            var port_no = resp.result[i].port_no;
                            var network_id = resp.result[i].network_id;
                            var display_name = resp.result[i].display_name;
                            var is_virtual_port_allowed = resp.result[i].is_virtual_port_allowed;
                            let _layerTitle = resp.result[i].entity_title;
                            //var ptObj=undefined;
                            //var _points = jResp.Data.point;
                            //for (var i = 0; i < _points.length; i++) {
                            var geometry = getLatLongArr(resp.result[i].sp_geometry);
                            //check if marker already exist
                            //if(app.gMapObj.pointMarkers.filter(obj => { return obj.id === system_id && obj.eType==en_type }).length==0)
                            //IE Solution
                            var ptObj = null;
                            if (resp.result[i].entity_category != null && resp.result[i].entity_category != '') {
                                ptObj = si.createMarkerForPathFinder(geometry[0], 'Content/images/icons/lib/circle/' + resp.result[i].entity_category + '_' + resp.result[i].en_type.toUpperCase() + '.png', system_id, en_type, port_no, display_name, is_virtual_port_allowed);
                            }
                            else {
                                ptObj = si.createMarkerForPathFinder(geometry[0], 'Content/images/icons/lib/circle/' + (resp.result[i].is_virtual ? 'v_' : '') + resp.result[i].en_type + '.png', system_id, en_type, port_no, display_name, is_virtual_port_allowed);
                            }
                            var SourceEquipmentId = $('#equipment_id').val().split(" ")[0];
                            //JIRA- SSSI-452 bug fix change done by Ram
                            bounds.extend(geometry[0]);
                            if (network_id == SourceEquipmentId) {

                                // current maps duration of one bounce (v3.13)
                                ptObj.setAnimation(google.maps.Animation.BOUNCE);
                                setTimeout(function () {
                                    ptObj.setAnimation(null);
                                }, 700);
                            }
                            //ptObj.draggable=false;
                            // add info window
                            var contentString = '<div id="content">' +
                                '<input type="button" id="export" enType="' + en_type + '" systemId="' + system_id + '"  value="export"/>' +
                                '</div>';
                            var objInfoWindow = new google.maps.InfoWindow();
                            var infowindow = new google.maps.InfoWindow({
                                content: contentString
                            });

                            google.maps.event.addListener(ptObj, 'click', (function (ptObj, i) {
                                var PortNo = ptObj.portNo > 0 ? ptObj.portNo.toString() + " OUT" : ptObj.portNo.toString().replace('-', '') + " IN";
                                return function () {

                                    var content = '<div><h4>' + _layerTitle + ' Detail </h3><p><span  class="Info-content"></span>' + ptObj.networkId + '</p>';
                                    if (!ptObj.isVirtualPortAllowed) {
                                        content += '<p><span  class="Info-content">Port No :</span> ' + PortNo + '</p>';
                                    }
                                    content += '<input class="btn btn-primary btn-xs" type="button" id="export" networkId="' + ptObj.networkId + '" portNo="' + ptObj.portNo + '" enType="' + ptObj.eType + '" systemId="' + ptObj.id + '"  value="Export connection detail "/></div>';
                                    objInfoWindow.setContent(content);
                                    objInfoWindow.open(si.map, ptObj);
                                }
                            })(ptObj, i));

                            ptObj.setMap(si.map);
                            gMapObj.TraceRoute.push(ptObj);
                            app.gMapObj.pointMarkers.push(ptObj);
                            if (resp.result[i].en_type.toUpperCase() != 'PATCHCORD') { app.oms.addMarker(ptObj); }


                            //}
                        }
                        // }
                    }

                    // for highlight the  Path..
                    $.each(highlightLineList, function (index, item) {
                        setTimeout(function () { animateLine(item, 0) }, 20);
                    });
                    if ($('#chkFittomap').is(':checked')) {
                        
                        si.map.fitBounds(bounds);

                    } else {
                        var systemId = $('#objFilterAttributes_entityid').val();
                        var entityType = $('#objFilterAttributes_entity_type').val();
                        si.ShowEntityOnMap(parseInt(systemId), entityType, (entityType == 'Cable' ? 'Line' : 'Point'));
                    }
                }
            }
        }, true, true);
    }
    this.downloadCPE = function () {
        window.location = appRoot + 'Splicing/DownloadConnectionPathFinderReport';
    }

    this.downloadConnectedCustomerDetails = function () {
         
        window.location = appRoot + 'Splicing/DownloadConnectedCustomerReport';
    }
    this.downloadCPEKML = function () {
        window.location = appRoot + 'Splicing/DownloadCPFIntoKML';
    }
    //pk
    this.clearCFPGrid = function () {
        if (app.apptestvalue == false) {
            $(app.DE.equipmentid).val('');            
            $(app.DE.ddlCore).html('').html('<option value="0">-Select-</option>').val("0").trigger("chosen:updated");
            $(app.DE.entityid).val(0);
            $('#objFilterAttributes_port_no').val(0);
            $('#objFilterAttributes_entity_type').val('');
            $(app.DE.btnShowRoute).trigger("click");
        }                
        app.clearCPFMarker();        
        $('#divNoRecordExist').show();        
        $('#tblConnectionPathFinderInfo,#dvHeader').hide();
        $('.libTab--dis').hide();
        $(app.DE.ddlCore).html('').html('<option value="0">-Select-</option>').val("0").trigger("chosen:updated");

    }
    this.downloadOpticalLink = function () {
        window.location = appRoot + 'Splicing/DownloadOpticalLinkBudgetReport';
    }

    this.showSchematicView = function () {
        var entityid = $('#objFilterAttributes_entityid').val();
        var entity_type = $('#objFilterAttributes_entity_type').val();
        var port_no = $('#objFilterAttributes_port_no').val();
        //var entityid = $('#objFilterAttributes_entityid').val();
        var systemId = $(app.DE.ConnectingEntity).val();
         
        ajaxReq('main/EncryptMultiple', { entityid: entityid, entity_type: entity_type, port_no: port_no }, false, function (resp) {
            window.open(appRoot + 'Splicing/SchematicView?val='
                + resp.entityid + ','
                + resp.entity_type + ','
                + resp.port_no, '_blank');
        }, false, false);
    }

    $(document).on("click", "#export", function () {
         
        var enType = $(this).attr('enType');
        var systemId = $(this).attr('systemId');
        var portNo = $(this).attr('portNo');;
        window.location = 'Splicing/exportConnectionPathForMarker?entity_type=' + enType + '&system_id=' + systemId + '&port_no=' + portNo;
    });
    this.OpticalLinkClearButton = function () {
        $('#ddlSourceNetworkid, #ddlDestinationNewtorkid, #ddl_waveLength').val('0').trigger('chosen:updated');
        $('#txt_transmit').val(0);
        $('#txt_receving').text('');
        $('#txt_totallossdetail').text('');
        $('#dvViewOpticalLinkBudgetDetails').empty();
        $('#btnopticalExport').attr('disabled', 'disabled');
    }
    this.CalculateLinkBudgetDetail = function () {
        var _entitysystemid = $(app.DE.entityid).val();
        var SourceEquipmentId = $('#equipment_id').val().split(" ")[0];
        var _equipmentid = SourceEquipmentId
        var _equipmentportid = $("#ddlCore").val();
        var _sourcenetworkid = $("#ddlSourceNetworkid option:selected").text();

        var _sourcesystemid = $("#ddlSourceNetworkid option:selected").val();
        var _sourceportno = $("#ddlSourceNetworkid option:selected").attr('data-portno');
        var _sourceentitytype = $("#ddlSourceNetworkid option:selected").attr('data-entityType');

        var _destinationnetworkid = $("#ddlDestinationNewtorkid option:selected").text();


        var _destinationsystemid = $("#ddlDestinationNewtorkid option:selected").val();
        var _destinationportno = $("#ddlDestinationNewtorkid option:selected").attr('data-portno');
        var _destinationentitytype = $("#ddlDestinationNewtorkid option:selected").attr('data-entityType');

        var _Wavelengthid = $("#ddl_waveLength option:selected").val();
        var _transmitpower = $("#txt_transmit").val();
        var _equipmenttype = $(app.DE.entity_type).val();

        if (_sourcenetworkid.toLowerCase() == '--select--') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_008);//Please select Source Id!
            return false;
        }
        else if (_destinationnetworkid.toLowerCase() == '--select--') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_009);//Please select Destination Id!
            return false;
        }
        else if (_Wavelengthid.toLowerCase() == '--select--') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_010);//Please select wavelength!
            return false;
        }

        else if (_transmitpower.length <= 0) {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_011);//Please enter transmit power!
            return false;
        }
        var data = {
            equipmentsystemid: _equipmentid,
            equipmentPortid: _equipmentportid,
            sourcesystemid: _sourcesystemid,
            sourceportno: _sourceportno,
            sourceentitytype: _sourceentitytype,
            destinationsystemid: _destinationsystemid,
            destinationportno: _destinationportno,
            destinationentitytype: _destinationentitytype,
            wavelengthid: _Wavelengthid,
            transmitpower: _transmitpower,
            equipmenttype: _equipmenttype,
            equipmentsystemid: _entitysystemid
        }
        ajaxReq('Splicing/GetLinkBudgetDetails', data, true,
            function (resp) {
                $("#div_bindOpticalLinkGrid").html(resp);
                $('#txt_receving').text($("#hdnLastTransmitPower").val());
                $('#txt_totallossdetail').text($("#hdnTotalLoss").val());
                if ($("#tblOpticalLinkBudgetDetails tbody tr").length > 0) {
                    $("#btnopticalExport").removeAttr('disabled');
                }
            }, false, true);
    }
    this.redirectToCPF = function (event) {
        pageTitleText = 'Connection Path Finder';
        modalClass = 'modal-xxl';
        popup.LoadModalDialog(event.data.isParentPopup ? 'PARENT' : 'CHILD', 'splicing/ConnectionPathFinder', { equipment_id: event.data.networkId, objFilterAttributes: { entity_type: event.data.entityType, entityid: event.data.systemId, port_no: event.data.portNo }, isControllEnable: event.data.isControllEnable }, pageTitleText, modalClass);
        app.apptestvalue = event.data.isControllEnable;
    }
    this.redirectToCustomerDetails = function (event, networkId, entity_type, systemId, portNo, isParentPopup) {
         
        pageTitleText = 'Connected Customer Details';
        modalClass = 'modal-xxl';
        var _data = null;
        if (event != null) {
            _data = { equipment_id: event.data.networkId, objFilterAttributes: { entity_type: event.data.entityType, entityid: event.data.systemId, port_no: event.data.portNo }, isControllEnable: event.data.isControllEnable };
            popup.LoadModalDialog(event.data.isParentPopup ? 'PARENT' : 'CHILD', 'splicing/GetConnectedCustomerDetails', _data, pageTitleText, modalClass);
        }
        else {
            _data = { equipment_id: networkId, objFilterAttributes: { entity_type: entity_type, entityid: systemId, port_no: portNo }, isControllEnable: false };
            popup.LoadModalDialog(isParentPopup ? 'PARENT' : 'CHILD', 'splicing/GetConnectedCustomerDetails', _data, pageTitleText, modalClass);
        }

    }

    this.redirectToLinkInfo = function (event, networkId, entity_type, systemId, portNo, isParentPopup) {
         
        pageTitleText = 'Link Info';
        modalClass = 'modal-md';
        var _data = null;
        if (event != null) {
            _data = { equipment_id: event.data.networkId, objFilterAttributes: { entity_type: event.data.entityType, entityid: event.data.systemId, port_no: event.data.portNo }, isControllEnable: event.data.isControllEnable };
            popup.LoadModalDialog(event.data.isParentPopup ? 'PARENT' : 'CHILD', 'splicing/GetAssociatedLinkInfo', _data, pageTitleText, modalClass);
        }
        else {
            _data = { equipment_id: networkId, objFilterAttributes: { entity_type: entity_type, entityid: systemId, port_no: portNo }, isControllEnable: false };
            popup.LoadModalDialog(isParentPopup ? 'PARENT' : 'CHILD', 'splicing/GetAssociatedLinkInfo', _data, pageTitleText, modalClass);
        }

    }
    this.changeUpDownStream = function (controllId, streamClass) {
        $('#divNoRecordExist').css('visibility', 'hidden')
        if ($(controllId).is(':checked')) { $(streamClass).show(); } else { $(streamClass).hide(); }
        if ($('#chkUpStream').is(':checked') && $('#chkDownStream').is(':checked')) {
            var downLength = app.getDownStreamLength();
            var upLength = app.getUpStreamLength();
            $('#spnCalculatedLength').text(roundNumber((downLength.calculatedLength + upLength.calculatedLength), 2) + ' Mtr');
            $('#spnMeasuredLength').text(roundNumber((downLength.measuredLength + upLength.measuredLength), 2) + ' Mtr');
        } else if ($('#chkUpStream').is(':checked') && !$('#chkDownStream').is(':checked')) {
            var upLength = app.getUpStreamLength();
            $('#spnCalculatedLength').text(roundNumber(upLength.calculatedLength, 2) + ' Mtr');
            $('#spnMeasuredLength').text(roundNumber(upLength.measuredLength, 2) + ' Mtr');
        } else if ($('#chkDownStream').is(':checked') && !$('#chkUpStream').is(':checked')) {
            var downLength = app.getDownStreamLength();
            $('#spnCalculatedLength').text(roundNumber(downLength.calculatedLength, 2) + ' Mtr');
            $('#spnMeasuredLength').text(roundNumber(downLength.measuredLength, 2) + ' Mtr');
        }
        if (controllId == '#chkDownStream' && !$(controllId).is(':checked') && !$('#chkUpStream').is(':checked')) { $('#chkUpStream').trigger("click"); }
        else if (controllId == '#chkUpStream' && !$(controllId).is(':checked') && !$('#chkDownStream').is(':checked')) { $('#chkDownStream').trigger("click"); }
        $('#spnTotalRecord').text($('#tblConnectionPathFinderInfo  tbody tr[style$="display: table-row;"]').length);
        if ($('#tblConnectionPathFinderInfo  tbody tr[style$="display: table-row;"]').length == 0) {
            $('#divNoRecordExist').css('visibility', 'visible')
        }
    }
    this.getDownStreamLength = function () {
        var totalCalculatedLength = 0;
        var totalMeasuredLength = 0;
        $('.downStream').each(function () {
            var data = $(this).data();
            if (data.calculatedLength != '') { totalCalculatedLength += parseFloat(data.calculatedLength); }
            if (data.measuredLength != '') { totalMeasuredLength += parseFloat(data.measuredLength) };
        })
        return { calculatedLength: totalCalculatedLength, measuredLength: totalMeasuredLength }
    }
    this.getUpStreamLength = function () {
        var totalCalculatedLength = 0;
        var totalMeasuredLength = 0;
        $('.upStream').each(function () {
            var data = $(this).data();
            if (data.calculatedLength != '') { totalCalculatedLength += parseFloat(data.calculatedLength); }
            if (data.measuredLength != '') { totalMeasuredLength += parseFloat(data.measuredLength) };
        })
        return { calculatedLength: totalCalculatedLength, measuredLength: totalMeasuredLength }
    }
    //this.swithchView = function (isTabulerView, isSchematicView) {
    //    $('#divSchematicParentStream').hide();
    //    $('#divNoRecordExist').css('visibility', 'hidden');
    //    $('#dvSchematicView').parent().css('border', '1px solid grey');
    //    if (isTabulerView) {
    //        $('#divParentStream').show();
    //        if ($('#tblConnectionPathFinderInfo  tbody tr[style$="display: table-row;"]').length == 0) {
    //            $('#divNoRecordExist').css('visibility', 'visible')
    //            $('#dvSchematicView').parent().css('border', 'none');
    //        }
    //    }
    //    if (isSchematicView) {
    //        $('#divParentStream').hide();
    //        $('#divSchematicParentStream').show();
    //        if ($('#chkSchematicDownStream').is(':checked') && splicing.downStream == null) {
    //            $('#divNoRecordExist').css('visibility', 'visible');
    //            $('#dvSchematicView').parent().css('border', 'none');
    //        }
    //        if ($('#chkSchematicUpStream').is(':checked') && splicing.upStream == null) {
    //            $('#divNoRecordExist').css('visibility', 'visible');
    //            $('#dvSchematicView').parent().css('border', 'none');
    //        }
    //    }
    //}
    //this.changeSchematicStream = function (obj) {
    //     
    //    $('#divNoRecordExist').css('visibility', 'hidden');
    //    $('#dvSchematicView').parent().css('border', '1px solid grey');
    //    $('#dvSchematicView').html('');
    //    if ($(obj).is(':checked') && $(obj).attr('id') == 'chkSchematicDownStream') {

    //            if (splicing.downStream != null) { SchematicView(app.downStream, "#dvSchematicView", 870, 320); } else { $('#divNoRecordExist').css('visibility', 'visible'); $('#dvSchematicView').parent().css('border', 'none'); }

    //    }
    //    if ($(obj).is(':checked') && $(obj).attr('id') == 'chkSchematicUpStream') {
    //        if (splicing.upStream != null) { SchematicView(app.upStream, "#dvSchematicView", 870, 320); } else { $('#divNoRecordExist').css('visibility', 'visible'); $('#dvSchematicView').parent().css('border', 'none'); }
    //    }
    //}
    this.patching = {
        save: function (connection, info) {
             
            //ajaxReq('ISP/checkTemplateExist', { enType: 'PatchCord' }, true, function (data) {
            //    if (data.status == 'FAILED') {
            //        jsPlumb.detach(info);
            //        alert(data.message, 'warning');
            //        return false;
            //    }
            var sourceData = $('#' + info.sourceId).data();
            var targetData = $('#' + info.targetId).data();
            var _data = { geom: '', networkIdType: 'A', isDirectSave: true, a_system_id: sourceData.systemId, a_entity_type: sourceData.entityType, a_location: sourceData.networkId, b_system_id: targetData.systemId, b_entity_type: targetData.entityType, b_location: targetData.networkId, structure_id: (isp != null ? parseInt($(isp.DE.StructureId).val()) : 0) };
            ajaxReq('Library/SavePatchCord', _data, false, function (resp) {
                if (resp.objPM.status == 'OK') {
                    connection.equipment_entity_type = 'PatchCord';
                    connection.equipment_system_id = resp.system_id;
                    connection.equipment_network_id = resp.network_id;
                    var connectionId = app.saveConnection([connection], info.sourceId, info.targetId, info);
                    info.connection.setPaintStyle({ strokeStyle: '#000000', lineWidth: 3 });
                    info.connection.addClass('PatchCord');
                    info.connection.setParameters(resp.system_id);
                    info.connection.bind('click', function (e) {
                        app.patchActions(info.connection.sourceId, e);
                    })
                    $('#divSpliceWindow svg').tooltip({ title: resp.network_id, html: true, placement: "top" });
                } else { alert(resp.objPM.message); }
            }, true, true);
            //}, true, true, false);
        }

    }
    this.validateEntity = function () {
        if ($(app.DE.ddlCPEEntity).val() != null && $(app.DE.ddlCPEEntity).val().length > 2) {
            $(app.DE.ddlCPEEntity).val('').trigger('chosen:updated'); alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_051)//Maximum 2 entities can be selected!
        }
        if ($(app.DE.ddlCPEEntity).val() != null && $(app.DE.ddlCPEEntity).val().length == 2) {
            var firstEntity = '';
            $.each($(app.DE.ddlCPEEntity + ' :selected'), function (index, item) {
                if (firstEntity != '' && firstEntity.toUpperCase() == $(item).data().entityType.toUpperCase()) {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_050);//Same entities can not be selected!
                    $(app.DE.ddlCPEEntity).val('').trigger('chosen:updated');
                } else { firstEntity = $(item).data().entityType; }
            })
        }
    }
    this.redirectToPatchCord = function (event) {
        var titleText = 'Patch Cord';
        var modelClass = getPopUpModelClass(event.data.entityType);
        popup.LoadModalDialog('CHILD', 'Library/AddPatchCord', { system_id: event.data.systemId }, titleText, modelClass);
    }
    this.patchSuccess = function (resp) {
        $('.rel_pop').remove();
        $('#ChildPopUp #closeChildPopup').trigger("click");
        var type = resp.objPM.status == 'VALIDATION_FAILED' ? 'warning' : 'success';
        var header = resp.objPM.status == 'VALIDATION_FAILED' ? 'Warning' : 'Information';
        alert(resp.objPM.message);
    }
    this.patchActions = function (sourceId, e) {
        $('.rel_pop').remove();
        $('#' + sourceId).parent().append($(app.DE.splicingInfowindow).html());
        $('#btnConnectionPath').hide();
        $('#btnPatchCordEdit').show();
        $('#btnCloseSplicingInfo').on("click", function () { $('.rel_pop').hide(); })
        $('.rel_pop').show();//.css({ 'top': ((e.pageY)), 'left': (e.pageX) });
        $('#btnPatchCordEdit').click({ entityType: 'PatchCord', systemId: e.getParameters() }, app.redirectToPatchCord);
    }
    this.validateConnections = function (Connections) {
        var isValidConnection = true;
        var res = ajaxReq('Splicing/ValidtaeConnections', { connections: Connections }, false, function (data) {
            if (!data.status) {
                isValidConnection = false;
                alert(data.message);
            }
        }, true, true);
        return isValidConnection;
    }
    this.hexc = function (colorval) {
        var parts = colorval.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
        delete (parts[0]);
        for (var i = 1; i <= 3; ++i) {
            parts[i] = parseInt(parts[i]).toString(16);
            if (parts[i].length == 1) parts[i] = '0' + parts[i];
        }
        return '#' + parts.join('');
    }
    this.redirectToUpdateFiber = function (pEntitySystemId, pCorePortNumber, pEntityType, ptype, isGridCalling, pPortStatusId, pPortComment, pPortStatus) {
         
        if (pEntityType == undefined) {
            pCorePortNumber = pEntitySystemId.data.pCorePortNumber;
            pEntityType = pEntitySystemId.data.pEntityType;
            ptype = pEntitySystemId.data.ptype;
            pPortStatusId = pEntitySystemId.data.pPortStatusId;
            pPortComment = pEntitySystemId.data.pPortComment;
            pPortStatus = pEntitySystemId.data.pPortStatus;
            pEntitySystemId = pEntitySystemId.data.pEntitySystemId;
        }
        pageTitleText = pEntityType.toUpperCase() == "CABLE" ? 'Update Core Status' : 'Update Port Status';
        modalClass = 'modal-sm';
        popup.LoadModalDialog('CHILD', 'splicing/UpdateCorePortStatus', { entitySystemId: pEntitySystemId, entityType: pEntityType, corePortNumber: pCorePortNumber, type: ptype, isGridCalling: isGridCalling, PortStatusId: pPortStatusId, PortComment: pPortComment, Status: pPortStatus}, pageTitleText, modalClass);
    }
    this.viewPortHistory = function (system_id) {
        pageTitleText = 'Port History';
        modalClass = 'modal-xxl';
        popup.LoadModalDialog('CHILD', 'splicing/ViewPortHistory', { system_id: system_id }, pageTitleText, modalClass);
    }
    this.resetEndPoint = function (resp) {
        $('#closeChildPopup').trigger("click");
        alert(resp.pageMsg.message);
        if ($('#ddlSourceCable').length > 0 || $('#ddlDestinationCable').length > 0) {
            var direction = '';
            var endPointId = resp.entity_system_id + '_' + resp.entity_type.toUpperCase() + '_' + resp.core_port_number;
            if (resp.entity_type.toLowerCase() == 'cable') {
                direction = ($('#ddlSourceCable').val() == resp.entity_system_id ? 'Left' : 'Right');
                endPointId = direction + '_' + resp.entity_system_id + '_' + resp.entity_type.toUpperCase() + '_' + resp.core_port_number;
            }

            if (resp.entity_type.toLowerCase() == 'cable') {
                app.endPointObject.hide(endPointId, true);
                if (resp.portStatus > 1) {
                    $('#' + endPointId).attr('data-status-id', resp.portStatus).attr('title', resp.comment).css('background', resp.portColor);
                    $('#' + endPointId).tooltip({ title: resp.comment, html: true, placement: "left" });
                }
                if (resp.portStatus == 1) {
                    app.endPointObject.addEndpoint($("#" + endPointId), { anchor: ($('#' + endPointId).hasClass('src') ? 'Right' : 'Left') + "Middle" }, app.getEndPointOptions());
                    $('#' + endPointId).attr('data-status-id', resp.portStatus).attr('title', '').css('background', resp.portColor);
                }
                var otherEnd = 'Other_' + resp.entity_system_id + '_' + resp.entity_type.toUpperCase() + '_' + resp.core_port_number;
                if (resp.portStatus > 1) {
                    $('#' + otherEnd).attr('data-status-id', resp.portStatus).attr('title', resp.comment).css('background', resp.portColor);
                    $('#' + otherEnd).attr('data-status-id', resp.portStatus).tooltip({ title: resp.comment, html: true, placement: "left" });
                }
                if (resp.portStatus == 1) {
                    $('#' + otherEnd).attr('data-status-id', resp.portStatus).css('background', resp.portColor);
                    $('#' + otherEnd).attr('data-status-id', resp.portStatus).tooltip({ title: '', html: true, placement: "left" });
                }
                if ($('#ddlSourceCable').val() == $('#ddlSourceCable').val()) {
                    endPointId = (direction == 'Left' ? 'Right' : 'Left') + '_' + resp.entity_system_id + '_' + resp.entity_type.toUpperCase() + '_' + resp.core_port_number;
                    app.endPointObject.hide(endPointId, true);
                    if (resp.portStatus > 1) {
                        $('#' + endPointId).attr('data-status-id', resp.portStatus).attr('title', resp.comment).css('background', resp.portColor);
                        $('#' + otherEnd).attr('data-status-id', resp.portStatus).tooltip({ title: resp.comment, html: true, placement: (direction == 'Left' ? 'right' : 'left') });
                    }
                    if (resp.portStatus == 1) {
                        app.endPointObject.addEndpoint($("#" + endPointId), { anchor: ($('#' + endPointId).hasClass('src') ? 'Right' : 'Left') + "Middle" }, app.getEndPointOptions());
                        $('#' + endPointId).attr('data-status-id', resp.portStatus).attr('title', '').css('background', resp.portColor);
                        $('#' + otherEnd).attr('data-status-id', resp.portStatus).tooltip({ title: '', html: true, placement: 'left' });
                    }
                }
            } else {
                app.endPointObject.hide(endPointId, true);
                if (resp.portStatus > 1) {
                    $('#' + endPointId).attr('data-status-id', resp.portStatus).attr('title', resp.comment).css('background', resp.portColor);
                    $('#' + endPointId).tooltip({ title: resp.comment, html: true, placement: "left" });
                }
                if (resp.portStatus == 1) {
                    app.endPointObject.addEndpoint($("#" + endPointId), { anchor: ($('#' + endPointId).hasClass('src') ? 'Right' : 'Left') + "Middle" }, app.getEndPointOptions());
                    $('#' + endPointId).attr('data-status-id', resp.portStatus).attr('title', '').css('background', resp.portColor);
                    $('#' + otherEnd).tooltip({ title: '', html: true, placement: "left" });
                }
            }
        }

    }
    this.statusColor = function () {
        var color = $('#ddlCoreStatus :selected').attr('data-status-color');
        $('#hdnPortStatus').val($('#ddlCoreStatus').val());
        $('#hdnPortColor').val(color);
    }
    this.connectionEditor = function () {
        var dataURL = 'Splicing/ConnectionEditor';
        var titleText = 'Connection Editor';
        popup.LoadModalDialog('PARENT', dataURL, {}, titleText, 'modal-xxl', function () {
            var conBuilder = new connectionBuilder();
            setTimeout(conBuilder.InitilizeJsPlumb, 300);
            var dataURL = 'Splicing/ConnectionFilter';
            var titleText = 'Connection Filter';
            ajaxReq(dataURL, { entityId: parseInt($('#ispPOPId').val()), entityType: $('#ispPOPType').val() }, true, function (resp) {
                setTimeout(function () {
                    $('#divConnectionFilter').html(resp).show();
                }, 300);
            }, false, false);
        });
    }
    this.crossConnect = function () {
        if (app.isSplicingRangeValid()) {
            var leftStartRange = parseInt($(app.DE.leftStartRange).val());
            var leftEndRange = parseInt($(app.DE.leftEndRange).val());
            var rightStartRange = parseInt($(app.DE.rightStartRange).val());
            var rightEndRange = parseInt($(app.DE.rightEndRange).val());
            var allConnections = [];
            var connectorType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
            if ($(app.DE.SourceCable).val() == '0' || $(app.DE.DestinationCable).val() == '0') {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_148);//Please select left and right cable!
                return false;
            }
            if ($('#dvLeftCable .leftFiber .src[data-link-id!="0"]').length == 0 && $('#dvRightCable .rightFiber .trg[data-link-id!="0"]').length == 0) { alert('Link not available on both cables!'); return false; }
            for (var i = leftStartRange; i <= leftEndRange; i++) {
                if ($('#dvLeftCable .leftFiber .src[data-status-id="1"][data-link-id!="0"][data-port-no="' + i + '"]').length > 0
                    && $('#dvRightCable .rightFiber .trg[data-status-id="1"][data-link-id!="0"]').length > 0) {
                    var sourceId = $('#dvLeftCable .leftFiber .src[data-status-id="1"][data-port-no="' + i + '"][data-link-id!="0"]').attr('id');
                    var linkId = $('#' + sourceId).attr('data-link-id');
                    var targetId = $('#dvRightCable .rightFiber .trg[data-status-id="1"][data-link-id="' + linkId + '"]').attr('id');
                    var tagetSystemId = $('#' + targetId).attr('data-system-id');
                    var tagetPortNo = $('#' + targetId).attr('data-port-no')
                    var filterConnection = allConnections.filter(m => (m.source_system_id == tagetSystemId && m.source_entity_type == 'Cable' && m.source_port_no == tagetPortNo)
                        || (m.destination_system_id == tagetSystemId && m.destination_port_no == tagetPortNo && m.destination_entity_type == 'Cable'));
                    if (sourceId && targetId && filterConnection.length == 0) {
                        allConnections.push(app.getConnectionsData(sourceId, targetId));
                    }
                }
                if (rightStartRange == rightEndRange) { break; }
                rightStartRange++;
            }



            if (allConnections.length > 0) {
                if (app.validateConnections(allConnections)) {
                    confirm(getMultilingualStringValue('Cross connectivity will perform based on same link except the cores which are already connected or connected to other,Do you want to continue?'), function () {
                        app.savebulkConnection(allConnections);
                    });
                }
            } else {
                alert('All links already connected for this range!');
            }

        }
    }
    this.resetPatchConnection = function (info) {
        jsPlumb.detach(info);
        app.isAutoDeleteConnection = false;
    }
    this.checkPatchTemplateExist = function (connection, info) {
        ajaxReq('Main/ChkEntityTemplateExist', { entityType: 'PatchCord', entitysubType: '' }, true, function (resp) {
            if (resp.status == "OK") {
                var patchDetails = app.patching.save(connection, info);
            } else {
                app.isAutoDeleteConnection = true;
                app.resetPatchConnection(info);
                var msga = resp.message.split('.');
                var msg = resp.message;
                if (msga.length > 1)
                    msg = msga[0] + ' Patch Cord ' + msga[1];

                alert(msg);
            }

        }, true, false);

    }
    this.validateTray = function () {
        let _isValid = true;
        let _totalPorts = $(app.DE.ddlSpliceTray + ' :selected').attr('data-total-ports');
        let _usedPorts = $(app.DE.ddlSpliceTray + ' :selected').attr('data-used-ports');
        if (parseInt(_usedPorts) == parseInt(_totalPorts)) { _isValid = false; }
        return _isValid;
    }
    this.updateTrayPort = function (_isNewConnection) {
        if ($(app.DE.ddlSpliceTray + ' :selected').val() != '') {
            let _traySystemId = $(app.DE.ddlSpliceTray).val();
            ajaxReq('Splicing/GetTrayUsedPort', { systemId: _traySystemId }, true, function (resp) {
                $(app.DE.ddlSpliceTray + ' :selected').attr('data-used-ports', resp);
            }, false, true, false);
        }
    }
    function SetThroughConnValue() {
         
        var _isthrough = $(app.DE.ConnectingEntity + ' :selected').attr('data-is-virtual');
        if (_isthrough.toLowerCase() == 'false') {
            app.color = '#00ba8a';
        }
        else {
            app.color = '#808080';
        }
    }

    this.ResetSpliceTrayThroughInfo = function () {
        if ($("input[name='ConnInfoMaster.is_through_connection']:checked").val().toUpperCase() == "TRUE") {
            $("#ddl_SpliceTray")[0].selectedIndex = 0;
            $('#ddl_SpliceTray').prop('disabled', true).trigger("chosen:updated")
        }

    }
    this.ResetSpliceTraySpliceInfo = function () {
        if ($("input[name='ConnInfoMaster.is_through_connection']:checked").val().toUpperCase() == "FALSE") {
            $('#ddl_SpliceTray').prop('disabled', false).trigger("chosen:updated")
        }

    }

}
