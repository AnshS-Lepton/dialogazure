var connectionBuilder = function () {
    var app = this;
    var gMapObj = {};
    this.ParentModel = 'PARENT';
    this.isSwapped = false;
    this.mainJSPlumb = null;
    this.gMapObj = {};
    this.longitude = 0;
    this.latitude = 0;
    this.conObjects = [];
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
    this.connectionObjects = [];
    this.connection = null;
    this.info = null;
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
        "btnopticalExport": "#btnopticalExport",
        "chkSpliceTerminatedCablesOnly": "#chkSpliceTerminatedCablesOnly",
        "btnConnectedCustomerExport": "#btnConnectedCustomerExport",
        "linkConnection": "#linkConnection",
        "closeConEditor": "#closeConEditor",
        "Filter": "#btnConnectionFilter",
        "LeftPOD": "#ddlLeftPOD",
        "RightPOD": "#ddlRightPOD",
        "LeftRack": "#ddlLeftRack",
        "RightRack": "#ddlRightRack",
        "LeftEquipment": "#ddlLeftEquipment",
        "RightEquipment": "#ddlRightEquipment"
    }
    this.initApp = function () {
        this.bindEvents();
    }
    this.bindEvents = function () {
        //$(app.DE.linkConnection).on("click", function () {
        //    app.connectionEditor();
        //});
        $(document).on('change', app.DE.LeftPOD, function (e) {
            var systemId = $(this).val();
            $(this).next('.chosen-container').removeClass('NotValid');
            if (systemId == '') {
                //$(this).html('<option value="0">--Select--</option>').val('0').trigger('chosen:updated');
                $('#ddlLeftRack').html('<option value="0">--Select--</option>').trigger('chosen:updated');
                $(app.DE.LeftEquipment).html('<option value="0">--Select--</option>').val('0').trigger('chosen:updated');
            }
            else {
                app.getRackList(systemId, '#ddlLeftRack');
            }
        });
        $(document).on('change', app.DE.RightPOD, function (e) {
            var systemId = $(this).val();
            $(this).next('.chosen-container').removeClass('NotValid');
            if (systemId == '') {
                //$(this).html('<option value="0">--Select--</option>').val('0').trigger('chosen:updated');
                $('#ddlRightRack').html('<option value="0">--Select--</option>').val('0').trigger('chosen:updated');
                $(app.DE.RightEquipment).html('<option value="0">--Select--</option>').val('0').trigger('chosen:updated');
            }
            else {
                app.getRackList(systemId, '#ddlRightRack');
            }
        });
        $(document).on('change', app.DE.LeftRack, function (e) {
            var systemId = $(this).val();
            $(this).next('.chosen-container').removeClass('NotValid');
            if (systemId == '') {
                $(app.DE.LeftEquipment).html('<option value="0">--Select Equipment--</option>').val('0').trigger('chosen:updated');
            }
            else {
                $(this).next('.chosen-container').removeClass('NotValid');
                app.getEquipmentByRack(systemId,$(this).data("parent-id"),$(this).data("parent-type"), app.DE.LeftEquipment);
            }
        });
        $(document).on('change', app.DE.RightRack, function (e) {
            var systemId = $(this).val();
            $(this).next('.chosen-container').removeClass('NotValid');
            if (systemId == '') {
                $(app.DE.RightEquipment).html('<option value="0">--Select Equipment--</option>').val('0').trigger('chosen:updated');
            }
            else {
                $(this).next('.chosen-container').removeClass('NotValid');
                
                app.getEquipmentByRack(systemId,$(this).data("parent-id"),$(this).data("parent-type"), app.DE.RightEquipment);
            }
        });
        $(document).on('change', app.DE.LeftEquipment, function (e) {
            var systemId = $(this).val();
            $(app.DE.RightEquipment + ' option[data-is-middle-entity="true"]').attr('disabled', false).trigger("chosen:updated");
            if (systemId != '') {
                $(this).next('.chosen-container').removeClass('NotValid');
                $('#hdnSourceEqpmntId').val($(app.DE.LeftEquipment + ' option:selected').text());
                if ($(app.DE.LeftEquipment + ' option:selected').attr('data-is-middle-entity') == 'true') {
                    $(app.DE.RightEquipment + ' option[data-is-middle-entity="true"]').attr('disabled', true).trigger("chosen:updated");
                }
                $(app.DE.RightEquipment + " option").attr('disabled', false).trigger("chosen:updated");
                $(app.DE.RightEquipment + " option").each(function (i) {
                    if (systemId == $(this).val()) {
                        $(this).attr('disabled', true).trigger("chosen:updated");
                    }
                });
            }
        });
        $(document).on('change', app.DE.RightEquipment, function (e) {
            var systemId = $(this).val();
            $(app.DE.LeftEquipment + ' option[data-is-middle-entity="true"]').attr('disabled', false).trigger("chosen:updated");
            if (systemId != '') {
                $(this).next('.chosen-container').removeClass('NotValid');
                $('#hdnDestinationEqpmntId').val($(app.DE.RightEquipment + ' option:selected').text());
                if ($(app.DE.RightEquipment + ' option:selected').attr('data-is-middle-entity') == 'true') {
                    $(app.DE.LeftEquipment + ' option[data-is-middle-entity="true"]').attr('disabled', true).trigger("chosen:updated");
                }
                $(app.DE.LeftEquipment + " option").attr('disabled', false).trigger("chosen:updated");
                $(app.DE.LeftEquipment + " option").each(function (i) {
                    if (systemId == $(this).val()) {
                        $(this).attr('disabled', true).trigger("chosen:updated");
                    }
                });
            }
        });


    }
    this.validateFilter = function () {
        var _isValid = true;

        if ($(app.DE.LeftRack).val() == '' || $(app.DE.LeftRack).val() == '0')
        { $(app.DE.LeftRack).next('.chosen-container').addClass('NotValid'); _isValid = false; }

        if ($(app.DE.RightRack).val() == '' || $(app.DE.RightRack).val() == '0')
        { $(app.DE.RightRack).next('.chosen-container').addClass('NotValid'); _isValid = false; }

        if ($(app.DE.LeftEquipment).val() == '' || $(app.DE.LeftEquipment).val() == '0')
        { $(app.DE.LeftEquipment).next('.chosen-container').addClass('NotValid'); _isValid = false; }

        if ($(app.DE.RightEquipment).val() == '' || $(app.DE.RightEquipment).val() == '0')
        { $(app.DE.RightEquipment).next('.chosen-container').addClass('NotValid'); _isValid = false; }
        return _isValid;
    }
    this.resetFilter = function () {
        $('#ddlLeftRack').val('').trigger('chosen:updated');
        $('#ddlRightRack').val('').trigger('chosen:updated');
        $(app.DE.LeftRack + ' option').attr('disabled', false).trigger("chosen:updated");
        $(app.DE.RightRack + ' option').attr('disabled', false).trigger("chosen:updated");
        $(app.DE.LeftEquipment).html('<option value="0">--Select Equipment--</option>').val('0').trigger('chosen:updated');
        $(app.DE.RightEquipment).html('<option value="0">--Select Equipment--</option>').val('0').trigger('chosen:updated');
    }
    this.getRackList = function (_systemId, _updateTarget) {
        var entityType = $(app.DE.LeftPOD + ' option:selected').attr('data-entity-type');
        ajaxReq('ISP/getRackList', { systemId: parseInt(_systemId), entityType: entityType }, true, function (resp) {
            var option = '<option value="0">--Select--</option>';
            if (resp.length > 0) {
                $.each(resp, function (i, item) {
                    option += '<option value=' + item.dropdown_value + '>' + item.dropdown_key + '</option>';
                })
            }
            $(_updateTarget).html(option).trigger('chosen:updated');
        }, true, true);
    }
    this.getEquipmentByRack = function (_systemId,_parent_id,_parent_type, _updateTarget) {
    
        ajaxReq('ISP/getEquipmentByRack', { rackId: parseInt(_systemId),parent_id : _parent_id,parent_type :_parent_type }, true, function (resp) {
            var option = '<option value="0">--Select Equipment--</option>';
            if (resp.length > 0) {
                $.each(resp, function (i, item) {
                    option += '<option value=' + item.dropdown_value + '>' + item.dropdown_key + '</option>';
                })
            }
            $(_updateTarget).html(option).trigger('chosen:updated');
        }, true, true);
    }
    this.connectionEditor = function () {
         
        var dataURL = 'Splicing/ConnectionEditor';
        var titleText = MultilingualKey.SI_ISP_GBL_NET_FRM_036;
        popup.LoadModalDialog('PARENT', dataURL, {}, titleText, 'modal-xxl', function () {
            setTimeout(app.InitilizeJsPlumb, 500);
        });
    }
    this.conFilter = function () {
         
        var _sourcePODId = parseInt($('#hdnSourcePODId').val());
        var _sourceRackId = parseInt($('#hdnSourceRackId').val());
        var _sourceEquipmentId = parseInt($('#hdnSourceEqpmtId').val());
        var _destinationPODId = parseInt($('#hdnDestinationPODId').val());
        var _destinationRackId = parseInt($('#hdnDestinationRackId').val());
        var _destinationEquipmentId = parseInt($('#hdnDestinationEqpmtId').val());
        var _structureId = parseInt($('#hdnStructureId').val());

        var _data = {
            SourcePODId: ((_sourcePODId != null && _sourcePODId != '') ? parseInt(_sourcePODId) : 0),
            SourceRackId: ((_sourceRackId != null && _sourceRackId != '') ? parseInt(_sourceRackId) : 0),
            SourceEquipmentId: ((_sourceEquipmentId != null && _sourceEquipmentId != '') ? parseInt(_sourceEquipmentId) : 0),
            DestinationPODId: ((_destinationPODId != null && _destinationPODId != '') ? parseInt(_destinationPODId) : 0),
            DestinationRackId: ((_destinationRackId != null && _destinationRackId != '') ? parseInt(_destinationRackId) : 0),
            DestinationEquipmentId: ((_destinationEquipmentId != null && _destinationEquipmentId != '') ? parseInt(_destinationEquipmentId) : 0),
            structureId: _structureId
        }
        var dataURL = 'Splicing/ConnectionFilter';
        var titleText = 'Connection Filter';

        ajaxReq(dataURL, { entityId: parseInt($('#ispPOPId').val()), entityType: $('#ispPOPType').val() }, true, function (resp) {
            $('#divConnectionFilter').html(resp);
        }, false, false);

        //popup.LoadModalDialog('CHILD', dataURL, { structureId: parseInt($('#hdnStructureId').val()) }, titleText, 'modal-md', function () {
        //    if (_sourcePODId > 0) {
        //        $('#ddlLeftPOD').val(_sourcePODId).trigger('chosen:updated').change();
        //        setTimeout(function () { $('#ddlLeftRack').val(_sourceRackId).trigger('chosen:updated').change(); }, 1500)
        //        setTimeout(function () { $('#ddlLeftEquipment').val(_sourceEquipmentId).trigger('chosen:updated'); }, 2000)
        //    }
        //    if (_destinationPODId > 0) {
        //        $('#ddlRightPOD').val(_destinationPODId).trigger('chosen:updated').change();
        //        setTimeout(function () { $('#ddlRightRack').val(_destinationRackId).trigger('chosen:updated').change(); }, 1500)
        //        setTimeout(function () { $('#ddlRightEquipment').val(_destinationEquipmentId).trigger('chosen:updated'); }, 2000)
        //    }
        //});
    }
    this.InitilizeJsPlumb = function () {
        $('#closeChildPopup').trigger("click");
        var defaultOptions = app.getDefaultOptions();
        jsPlumb.importDefaults(defaultOptions);
        app.connectManyToMany();
        app.addEndPoint();
        app.connectOneToOne();
        app.setStatusColor();
        app.setModelNameColor();
        $('#divConnectionFilter').hide();
        if ($('#hdnInsideConnectivity').val() == 'True') { app.blockConnectedPorts(); }
    }
    this.getDefaultOptions = function () {
        var options = {};
        options.DragOptions = { cursor: 'pointer', zIndex: 2000 }
        options.PaintStyle = { strokeStyle: '#00ba8a', lineWidth: 1 };
        options.EndpointStyle = { width: 20, height: 16, strokeStyle: '#00ba8a', fillStyle: "#808080" };
        options.Endpoint = ["Dot", { radius: 3.5 }];
        options.connector = ["Bezier", { curviness: 50 }];
        options.MaxConnections = 1;
        return options;
    }
    this.addEndPoint = function () {
        try {
            var anchor = 'Center';
            if (app.endPointObject == null) {
                app.endPointObject = jsPlumb.getInstance();
            }
            if ($('#hdnInsideConnectivity').val() == 'True') { anchor = 'Right'; $('.src').css('background-color','#fff'); }
            app.endPointObject.addEndpoint($('.src[data-is-multiconnection="True"],.trg[data-is-multiconnection="True"]'), { anchor: anchor }, app.getEndPointOptions());
            var defaultOption = app.getEndPointOptions();
            defaultOption.maxConnections = 1;
            if ($('#hdnInsideConnectivity').val() == 'True') {
                app.endPointObject.addEndpoint($('.src[data-is-multiconnection="False"],.trg[data-is-multiconnection="False"]'), { anchor: anchor }, defaultOption);
            } else {
                app.endPointObject.addEndpoint($('.src[data-is-multiconnection="False"][data-status-id="1"],.trg[data-is-multiconnection="False"][data-status-id="1"]'), { anchor: anchor }, defaultOption);
            }



            app.bindjsPlumbEvent(app.endPointObject);
        } catch (err) {

        }
    }
    this.addSingleEndPoint = function () {
        if (app.endPointObject == null) {
            app.endPointObject = jsPlumb.getInstance();
        }
        var defaultOption = app.getEndPointOptions();
        defaultOption.maxConnections = 1;
        app.endPointObject.addEndpoint($('.src[data-is-multiconnection="False"],.trg[data-is-multiconnection="False"]'), { anchor: "Center" }, defaultOption);
        app.bindjsPlumbEvent(app.endPointObject);
    }
    this.getEndPointOptions = function () {
        try {
            var options = {};
            options.endpoint = ["Dot", { radius: 5 }];
            options.paintStyle = { fillStyle: "#808080" };
            options.isTarget = true;
            options.isSource = true;
            options.scope = "green dot";
            options.connectorStyle = { strokeStyle: "#00ba8a", lineWidth: 1 };
            options.connector = ["Bezier", { curviness: 50 }];
            options.maxConnections = 20;


            return options;
        } catch (err) {

        }
    }
    this.bindjsPlumbEvent = function (objJsPlumb) {
        objJsPlumb.bind("connection", function (info, originalEvent) {
            if (!app.isMoved) {
                var connection = app.getConnectionsData(info.sourceId, info.targetId, true);
                if ($('#hdnInsideConnectivity').val() == 'False' && !app.patching.isTemplateRequired()) {
                    $('#dvMainProgress').show();
                    app.connection = connection;
                    app.info = info;
                    app.createPatchCord();
                }
                else if ($('#hdnInsideConnectivity').val() == 'False' && app.patching.isTemplateRequired()) {
                    app.connection = connection;
                    app.info = info;
                    app.checkPatchTemplateExist(connection, info)
                }
                else {
                    var connectionId = app.saveConnection([connection], info.sourceId, info.targetId, info);
                }
                app.connectionObjects.push(info);
                AllConnectionObjects.push(info);
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

                app.deleteConnection([connection], source, target);

            }
        });
        objJsPlumb.bind("beforeDrop", function (params) {
            var connection = app.getConnectionsData(params.sourceId, params.targetId, true);
            var _filterConnection = AllConnectionData.filter(m=>((parseInt(m.source_port_no) == parseInt($('#' + params.sourceId).attr('data-port-no')) && parseInt(m.destination_port_no) == parseInt($('#' + params.targetId).attr('data-port-no')))
            || (parseInt(m.source_port_no) == parseInt($('#' + params.targetId).attr('data-port-no')) && parseInt(m.destination_port_no) == parseInt($('#' + params.sourceId).attr('data-port-no')))));
            if (_filterConnection.length > 0) {
                alert('Connection already created for these ports!');
                return false;
            } else
                if ($('#hdnInsideConnectivity').val() == 'True'
                   && $('#' + params.sourceId).attr('data-entity-type') == $('#' + params.targetId).attr('data-entity-type')
                   && parseInt($('#' + params.sourceId).attr('data-system-id')) == parseInt($('#' + params.targetId).attr('data-system-id'))
                   && parseInt($('#' + params.sourceId).attr('data-port-no')) == parseInt($('#' + params.targetId).attr('data-port-no'))) {
                    alert('Connection can not be created between two same ports!');
                    return false;
                }
                else if ($('#hdnInsideConnectivity').val() == 'False' && params.sourceId.indexOf('Left') == 0 && params.targetId.indexOf('Left') == 0) {
                    alert('Please open both side to same port connectivity!');
                    return false;
                }
                else if ($('#hdnInsideConnectivity').val() == 'False' && params.sourceId.indexOf('Right') == 0 && params.targetId.indexOf('Right') == 0) {
                    alert('Please open both side to same port connectivity!');
                    return false;
                }
                else { return true; }
        });
    }
    this.getConnectionsData = function (sourceId, targetId, isManualSplicing) {
         
        var source = $('#' + sourceId);
        var target = $('#' + targetId);
        var connection = {
            source_system_id: source.attr('data-system-id'),
            source_network_id: source.attr('data-network-id'),
            source_entity_type: source.attr('data-entity-type'),
            source_port_no: source.attr('data-port-no'),
            source_entity_sub_type: source.attr('data-entity-sub-type'),
            destination_system_id: target.attr('data-system-id'),
            destination_network_id: target.attr('data-network-id'),
            destination_entity_type: target.attr('data-entity-type'),
            destination_port_no: target.attr('data-port-no'),
            destination_entity_sub_type: target.attr('data-entity-sub-type'),
            splicing_source: 'EQUIPMENT_SPLICING',
            is_source_cable_a_end: false,
            is_destination_cable_a_end: false,
            equipment_system_id: 0,
            equipment_network_id: null,
            equipment_entity_type: null
        };


        return connection;
    }
    this.saveConnection = function (_data, sourceId, targetId, info) {
        ajaxReq('Splicing/SaveConnectionInfo', _data, true, function (resp) {
            if (resp.status) {
                $('#SlideMessage').text('Connection has saved successfully!');
                $(".alert-info").slideDown(300);
                setTimeout(function () {
                    $(".alert-info").slideUp(300);
                }, 2000)

                var connectedColor = $('#hdnConnectedColor').val();
                $('#' + sourceId).attr('data-is-connected', '1').attr('data-status-id', '2').siblings('._jsPlumb_endpoint').find('circle').attr('fill', connectedColor);//.css('background', connectedColor);
                $('#' + targetId).attr('data-is-connected', '1').attr('data-status-id', '2').siblings('._jsPlumb_endpoint').find('circle').attr('fill', connectedColor);;//.css('background', connectedColor);
                app.currentConnections.push(app.getConnectionsData(sourceId, targetId));
                var sibling = $('#' + sourceId).siblings('.connectionCount');
                var sourceConCount = sibling.html();
                if (sourceConCount != '') {
                    sourceConCount = parseInt(sourceConCount);
                    sourceConCount = sourceConCount + 1;
                    sibling.html(sourceConCount);
                    if (sourceConCount > 0) { sibling.removeClass('dvCountdisabled'); }
                }
                sibling = $('#' + targetId).siblings('.connectionCount');
                var DestConCount = sibling.html();
                if (DestConCount != '') {
                    DestConCount = parseInt(DestConCount);
                    DestConCount = DestConCount + 1;
                    sibling.html(DestConCount);
                    if (DestConCount > 0) { sibling.removeClass('dvCountdisabled'); }
                }
                app.connection = null;
                app.info = null;
            } else {
                jsPlumb.detach(info);
                alert(resp.message);
            }
        }, true, true);
    }
    this.patching = {
        isTemplateRequired: function () {
            return JSON.parse($('#hdnIsTemplateRequired').val().toLowerCase());
        },
        save: function (connection, info) {
            var sourceData = $('#' + info.sourceId).data();
            var targetData = $('#' + info.targetId).data();
            var _data = { geom: '', networkIdType: 'A', isDirectSave: true, a_system_id: sourceData.systemId, a_entity_type: sourceData.entityType, a_location: sourceData.networkId, b_system_id: targetData.systemId, b_entity_type: targetData.entityType, b_location: targetData.networkId, structure_id: (isp != null ? parseInt($(isp.DE.StructureId).val()) : 0) };
            ajaxReq('Library/SavePatchCord', _data, false, function (resp) {
                if (resp.objPM.status == 'OK') {
                    connection.equipment_entity_type = 'PatchCord';
                    connection.equipment_system_id = resp.system_id;
                    connection.equipment_network_id = resp.network_id;
                    var connectionId = app.saveConnection([connection], info.sourceId, info.targetId, info);
                    //info.connection.setPaintStyle({ strokeStyle: '#000000', lineWidth: 3 });
                    //info.connection.addclass('patchcord');
                    //info.connection.setparameters(resp.system_id);
                    //info.connection.bind('click', function (e) {
                    //    app.patchactions(info.connection.sourceid, e);
                    //})
                } else { alert(resp.objPM.message); }
                //$('#divSpliceWindow svg').tooltip({ title: resp.network_id, html: true, placement: "top" });
            }, true, true);
            //}, true, true, false);
        }

    }
    this.deleteConnection = function (connections, source, target) {
        ajaxReq('Splicing/deleteConnection', { objConnectionInfo: connections }, true, function (resp) {
             
            app.isMoved = false;
            if (resp.status == "OK") {
                source.attr('data-is-connected', '0').attr('data-status-id', '1');//.css('background', vacantColor);
                target.attr('data-is-connected', '0').attr('data-status-id', '1');//.css('background', vacantColor);
                if (app.info == null) {
                    $('#SlideMessage').text(resp.message);
                    $(".alert-info").slideDown(300);
                    setTimeout(function () {
                        $(".alert-info").slideUp(300);
                    }, 2000)

                    var vacantColor = $('#hdnVacantColor').val();

                    var sourceAttributes = source.data();
                    var targetAttributes = target.data();
                    var currentIndex = app.currentConnections.findIndex(function (m) {
                        return (parseInt(m.source_system_id) == parseInt(sourceAttributes.systemId) && m.source_entity_type.toUpperCase() == sourceAttributes.entityType.toUpperCase() && parseInt(m.source_port_no) == parseInt(sourceAttributes.portNo) && parseInt(m.destination_system_id) == parseInt(targetAttributes.systemId) && m.destination_entity_type.toUpperCase() == targetAttributes.entityType.toUpperCase() && parseInt(m.destination_port_no) == parseInt(targetAttributes.portNo))
                        || (parseInt(m.destination_system_id) == parseInt(targetAttributes.systemId) && m.destination_entity_type.toUpperCase() == targetAttributes.entityType.toUpperCase() && parseInt(m.source_port_no) == parseInt(targetAttributes.portNo) && parseInt(m.destination_system_id) == parseInt(sourceAttributes.systemId) && m.destination_entity_type.toUpperCase() == sourceAttributes.entityType.toUpperCase() && parseInt(m.destination_port_no) == parseInt(sourceAttributes.portNo))
                    });
                    app.currentConnections.splice(currentIndex, 1);
                    var sibling = source.siblings('.connectionCount');
                    var sourceConCount = sibling.html();
                    if (sourceConCount != '') {
                        sourceConCount = parseInt(sourceConCount);
                        sourceConCount = sourceConCount - 1;
                        sibling.html(sourceConCount);
                        if (sourceConCount == 0) {
                            sibling.addClass('dvCountdisabled');
                            app.setVacantColor(source);
                        }

                    }
                    sibling = target.siblings('.connectionCount');
                    var DestConCount = sibling.html();
                    if (DestConCount != '') {
                        DestConCount = parseInt(DestConCount);
                        DestConCount = DestConCount - 1;
                        sibling.html(DestConCount);
                        if (DestConCount == 0) {
                            sibling.addClass('dvCountdisabled');
                            app.setVacantColor(target);
                        }
                    }
                }
                //var jspl1 = jsPlumb.getInstance();
                //jspl1.addEndpoint($('.src[data-is-connected="0"]'), { anchor: "RightMiddle" }, app.getEndPointOptions());
                //jspl1.addEndpoint($('.trg[data-is-connected="0"]'), { anchor: "LeftMiddle" }, app.getEndPointOptions());
                app.connection = null;
                app.info = null;
            }
        }, true, true);
    }
    this.connectOneToOne = function () {
        $.each($('#dvLeftCable  .endPint[data-is-multiconnection="False"][data-is-connected="0"]'), function () {
            var sourceId = $(this).attr('id');
            var targetId = $(this).attr('data-target-id');
            if (sourceId.indexOf('EQUIPMENT') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                sourceId = (parseInt(sourceId.split('_')[0]) == parseInt($('#hdnSourceEqpmtId').val()) ? 'Left_' : 'Right_') + sourceId;
            }
            if (targetId.indexOf('EQUIPMENT') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                targetId = (parseInt(targetId.split('_')[0]) == parseInt($('#hdnDestinationEqpmtId').val()) ? 'Right_' : 'Left_') + targetId;
            }
            if ($('#hdnInsideConnectivity').val() == 'True') {
                sourceId = sourceId.replace('Right_', 'Left_'); targetId = targetId.replace('Right_', 'Left_');
            }
            if ($('#' + targetId).attr('data-is-connected') == '0' && $('#' + targetId).attr('data-is-multiconnection') == 'False') {
                app.createConnection(sourceId, targetId);
            }
        });      
        $.each($('#dvRightCable  .endPint[data-is-multiconnection="False"][data-is-connected="0"]'), function () {
            var sourceId = $(this).attr('id');
            var targetId = $(this).attr('data-target-id');            
            if (sourceId.indexOf('EQUIPMENT') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
            }
            if (targetId.indexOf('EQUIPMENT') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
            }
            if ($('#hdnInsideConnectivity').val() == 'True') {
                sourceId = sourceId.replace('Right_', 'Left_'); targetId = targetId.replace('Right_', 'Left_');
            }
            if ($('#' + targetId).attr('data-is-connected') == '0' && $('#' + targetId).attr('data-is-multiconnection') == 'False') {
                app.createConnection(sourceId, targetId);
            }
        });
        $('#dvProgressSplice').hide();
    }
    this.createOneToOneConnection = function (sourceId, targetId) {
        if ($('#' + targetId).length > 0 && $('#' + sourceId).length > 0) {
            var defaultOptions = app.getDefaultOptions();
            app.mainJSPlumb = jsPlumb.getInstance();
            app.mainJSPlumb.importDefaults(defaultOptions);
            var cableOptions = app.getOptions(sourceId, targetId, 'Left', '#00ba8a', '');
            cableOptions.anchors = app.getAnchors(sourceId, targetId);
            var conObject = app.mainJSPlumb.connect(cableOptions);
            app.connectionObjects.push(conObject);
            AllConnectionObjects.push(conObject);
            app.bindjsPlumbEvent(app.mainJSPlumb);
            var connectedColor = $('#hdnConnectedColor').val();
            $('#' + sourceId).attr('data-is-connected', '1');//.css('background', connectedColor);
            $('#' + targetId).attr('data-is-connected', '1');//.css('background', connectedColor);           
            app.currentConnections.push(app.getConnectionsData(sourceId, targetId));
        }
    }
    this.connectManyToMany = function () {
        $.each($('#dvLeftCable  .endPint[data-is-connected="0"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('EQUIPMENT') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $('#hdnSourceEqpmtId').val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('EQUIPMENT') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $('#hdnDestinationEqpmtId').val() ? 'Right_' : 'Left_') + targetId;
                }
                if ($('#hdnInsideConnectivity').val() == 'True') {
                    sourceId = sourceId.replace('Right_', 'Left_'); targetId = targetId.replace('Right_', 'Left_');
                }
                if (!($('#' + sourceId).attr('data-is-multiconnection') == 'False' && $('#' + targetId).attr('data-is-multiconnection') == 'False')) {
                    app.createConnection(sourceId, targetId);
                }
            }
        });

        $.each($('#dvRightCable  .endPint[data-is-connected="0"]'), function () {
            if ($(this).attr('data-is-connected') == '0') {
                var sourceId = $(this).attr('id');
                var targetId = $(this).attr('data-target-id');
                if (sourceId.indexOf('EQUIPMENT') > 0 && (sourceId.indexOf('Left_') < 0 && sourceId.indexOf('Right_') < 0)) {
                    sourceId = (parseInt(sourceId.split('_')[0]) == $(app.DE.SourceCable).val() ? 'Left_' : 'Right_') + sourceId;
                }
                if (targetId.indexOf('EQUIPMENT') > 0 && (targetId.indexOf('Left_') < 0 && targetId.indexOf('Right_') < 0)) {
                    targetId = (parseInt(targetId.split('_')[0]) == $(app.DE.DestinationCable).val() ? 'Right_' : 'Left_') + targetId;
                }
                if (!($('#' + sourceId).attr('data-is-multiconnection') == 'False' && $('#' + targetId).attr('data-is-multiconnection') == 'False')) {
                    app.createConnection(sourceId, targetId);
                }
            }
        });
        $('#dvProgressSplice').hide();
    }
    this.createConnection = function (sourceId, targetId) {
        if ($('#' + targetId).length > 0 && $('#' + sourceId).length > 0) {
            if ($('#' + sourceId).attr('data-is-multiconnection') == 'False') {
                if (app.endPointObject == null) {
                    app.endPointObject = jsPlumb.getInstance();
                }
                var defaultOption = app.getEndPointOptions();
                defaultOption.maxConnections = 1;
                app.endPointObject.addEndpoint($('#' + sourceId), { anchor: "Center" }, defaultOption);
                //app.bindjsPlumbEvent(app.endPointObject);
            }
            if ($('#' + targetId).attr('data-is-multiconnection') == 'False') {
                if (app.endPointObject == null) {
                    app.endPointObject = jsPlumb.getInstance();
                }
                var defaultOption = app.getEndPointOptions();
                defaultOption.maxConnections = 1;
                app.endPointObject.addEndpoint($('#' + targetId), { anchor: "Center" }, defaultOption);
                //app.bindjsPlumbEvent(app.endPointObject);
            }
            var defaultOptions = app.getDefaultOptions();
            app.mainJSPlumb = jsPlumb.getInstance();
            app.mainJSPlumb.importDefaults(defaultOptions);
            var cableOptions = app.getOptions(sourceId, targetId, 'Left', '#00ba8a', '');
            cableOptions.anchors = app.getAnchors(sourceId, targetId);
            var conObject = app.mainJSPlumb.connect(cableOptions);
            app.connectionObjects.push(conObject);
            AllConnectionObjects.push(conObject);
            app.bindjsPlumbEvent(app.mainJSPlumb);
            var connectedColor = $('#hdnConnectedColor').val();
            $('#' + sourceId).attr('data-is-connected', '1');//.css('background', connectedColor);
            $('#' + targetId).attr('data-is-connected', '1');//.css('background', connectedColor);           
            app.currentConnections.push(app.getConnectionsData(sourceId, targetId));
            AllConnectionData.push(app.getConnectionsData(sourceId, targetId));
        }
    }
    this.getOptions = function (sourceId, targetId, cablePosition, color, lineType) {
        try {
            var options = {};
            options.endpoint = ["Dot", { radius: 5 }];
            options.source = sourceId,
            options.target = targetId,
            options.connector = ["Bezier", { curviness: 50 }],
            options.paintStyle = { strokeStyle: color, lineWidth: 1 }
            options.MaxConnections = 20;
            //options.isTarget = true;
            //options.isSource = true;
            return options;
        }
        catch (err) { }
    }
    this.getAnchors = function (sourceId, targetId) {
        var anchors = [];
        if ($('#hdnInsideConnectivity').val() == 'True') { anchors.push('Right'); anchors.push('Right'); }
        else {           
            if ($('#' + sourceId).parent().hasClass('leftPorts')) {
                //anchors.push('Right');
                anchors.push('Center');
            } else if ($('#' + sourceId).parent().hasClass('rightPorts')) {
                //anchors.push('Left');
                anchors.push('Center');
            }
            if ($('#' + targetId).parent().hasClass('leftPorts')) {
                //anchors.push('Right');
                anchors.push('Center');
            } else if ($('#' + targetId).parent().hasClass('rightPorts')) {
                //anchors.push('Left');
                anchors.push('Center');
            }
        }
        return anchors;
    }
    this.setStatusColor = function () {
        $.each($('.endPint'), function (i, item) {
            var statusColor = $(this).attr('data-status-color');
            $(this).siblings('._jsPlumb_endpoint').find('circle').attr('fill', statusColor).attr('stroke', 'none');//.css('background', connectedColor);
        })
    }
    this.showConnections = function (_systemId, _networkId, _entityType, _portNo, obj) {
       if ($(obj).hasClass('dvCountdisabled')) { return false; }
        var titleText = 'Port Connections';
        var dataURL = 'Splicing/MultipleConnection';
        var _data = {
            source_system_id: _systemId, source_network_id: _networkId, source_entity_type: _entityType, source_port_no: _portNo,
            portType: (($('#hdnInsideConnectivity').val() == 'True') ? 'I' : 'O')
        };
        $('.dropbox').hide();
        // $('.connectionCount').next('.dropbox').hide();
        //var dataURL = 'Splicing/MultipleConnection';
        // popup.LoadModalDialog('CHILD', dataURL, _data, titleText, 'modal-md');
        $('#divMultiConnection_' + _systemId + '_' + _portNo).children('#divConnectionContainer').html('');
        if (!$('#divMultiConnection_' + _systemId + '_' + _portNo).hasClass('active')) {
            $('#divMultiConnection_' + _systemId + '_' + _portNo).addClass('active');
            $('#divMultiConnection_' + _systemId + '_' + _portNo).slideToggle();
            $('#divMultiConnection_' + _systemId + '_' + _portNo).children('#dvProgress').show();
            ajaxReq(dataURL, _data, true, function (resp) {
                $('#divMultiConnection_' + _systemId + '_' + _portNo).children('#divConnectionContainer').html(resp);
                $('#divMultiConnection_' + _systemId + '_' + _portNo);//.children('#dvProgress').hide();
                $('#divMultiConnection_' + _systemId + '_' + _portNo).css('background-image', 'none');
            }, false, false, false);
        } else {
            $('#divMultiConnection_' + _systemId + '_' + _portNo).removeClass('active');
            $('#divMultiConnection_' + _systemId + '_' + _portNo);//.children('#dvProgress').hide();
            $('#divMultiConnection_' + _systemId + '_' + _portNo).css('background-image', 'url(../content/images/loading_new.gif)');
        }
        // $(obj).next().slideToggle();
        //var objArrow = $(obj).find('i.fa');
        //if (objArrow != undefined && objArrow != null) {
        //    if (objArrow.hasClass('fa-chevron-up')) {
        //        objArrow.addClass('fa-chevron-down');
        //        objArrow.removeClass('fa-chevron-up');
        //    } else {
        //        objArrow.removeClass('fa-chevron-down');
        //        objArrow.addClass('fa-chevron-up');
        //    }
        //}
    }
    this.deleteMultiConnection = function (_ssystemId, _snetworkId, _sentityType, _sportNo, _dsystemId, _dnetworkId, _dentityType, _dportNo, _connectionId) {
         

        var source = _ssystemId + '_EQUIPMENT_' + _sportNo;
        var target = _dsystemId + '_EQUIPMENT_' + _dportNo;
        if ($('#hdnInsideConnectivity').val() == 'False') {
            if ($('#hdnSourceEqpmtId').val() == _ssystemId) {
                source = 'Left_' + source;
            } else { source = 'Right_' + source }

            if ($('#hdnDestinationEqpmtId').val() == _dsystemId) {
                target = 'Right_' + target;
            } else { target = 'Left_' + target }
        } else { source = 'Left_' + source; target = 'Left_' + target; }
        source = $('#' + source);
        target = $('#' + target);
        app.isAutoDeleteConnection = true;
        isAutoDeleteConnection = true;
        ajaxReq('Splicing/deleteMultiConnection', { connectionId: _connectionId }, true, function (resp) {
            if (resp.status == "OK") {
                if ($('#hdnInsideConnectivity').val() == 'True') { source.siblings('._jsPlumb_connector').remove(); }

                $('#SlideMessage').text(resp.message);
                $(".alert-info").slideDown(300);
                setTimeout(function () {
                    $(".alert-info").slideUp(300);
                }, 2000)

                var vacantColor = $('#hdnVacantColor').val();
                source.attr('data-is-connected', '0');//.css('background', vacantColor);
                target.attr('data-is-connected', '0');//.css('background', vacantColor);
                var sourceAttributes = source.data();
                var targetAttributes = target.data();
                var currentIndex = app.currentConnections.findIndex(function (m) {
                    return (parseInt(m.source_system_id) == parseInt(sourceAttributes.systemId) && m.source_entity_type.toUpperCase() == sourceAttributes.entityType.toUpperCase() && parseInt(m.source_port_no) == parseInt(sourceAttributes.portNo) && parseInt(m.destination_system_id) == parseInt(targetAttributes.systemId) && m.destination_entity_type.toUpperCase() == targetAttributes.entityType.toUpperCase() && parseInt(m.destination_port_no) == parseInt(targetAttributes.portNo))
                    || (parseInt(m.destination_system_id) == parseInt(targetAttributes.systemId) && m.destination_entity_type.toUpperCase() == targetAttributes.entityType.toUpperCase() && parseInt(m.source_port_no) == parseInt(targetAttributes.portNo) && parseInt(m.destination_system_id) == parseInt(sourceAttributes.systemId) && m.destination_entity_type.toUpperCase() == sourceAttributes.entityType.toUpperCase() && parseInt(m.destination_port_no) == parseInt(sourceAttributes.portNo))
                });
                app.currentConnections.splice(currentIndex, 1);
                $('#closeChildPopup').trigger("click");
                $.each(app.connectionObjects, function (index, item) {
                    if ((item.sourceId == source.attr('id') && item.targetId == target.attr('id'))
                        || (item.sourceId == target.attr('id') && item.targetId == source.attr('id'))) {
                        jsPlumb.detach(item);
                    }

                });
                $.each(AllConnectionObjects, function (index, item) {
                    if ((item.sourceId == source.attr('id') && item.targetId == target.attr('id'))
                        || (item.sourceId == target.attr('id') && item.targetId == source.attr('id'))) {
                        jsPlumb.detach(item);
                    }
                });
                app.isAutoDeleteConnection = false;
                var sibling = source.siblings('.connectionCount');
                var sourceConCount = sibling.html();
                if (sourceConCount != '') {
                    sourceConCount = parseInt(sourceConCount);
                    sourceConCount = sourceConCount > 0 ? sourceConCount - 1 : 0;
                    sibling.html(sourceConCount);
                    if (sourceConCount == 0) {
                        var vacantColor = $('#hdnVacantColor').val();
                        source.attr('data-status-id', '1');
                        sibling.addClass('dvCountdisabled');
                        app.setVacantColor(source);
                    }
                }
                sibling = target.siblings('.connectionCount');
                var DestConCount = sibling.html();
                if (DestConCount != '') {
                    DestConCount = parseInt(DestConCount);
                    DestConCount = DestConCount > 0 ? DestConCount - 1 : 0;
                    sibling.html(DestConCount);
                    if (DestConCount == 0) {
                        var vacantColor = $('#hdnVacantColor').val();
                        target.attr('data-status-id', '1');
                        sibling.addClass('dvCountdisabled');
                        app.setVacantColor(target);
                    }
                }
                $('#tr_' + _dsystemId + '_' + _dportNo).remove();
                if (source.siblings('.connectionCount ').find('.webgrid tbody tr').length == 0) {
                    source.siblings('.connectionCount ').next('.dropbox').hide();
                }
                if (target.siblings('.connectionCount ').find('.webgrid tbody tr').length == 0) {
                    target.siblings('.connectionCount ').next('.dropbox').hide();
                }
                var _findIndex = AllConnectionData.findIndex(m=>((parseInt(m.source_system_id) == parseInt(sourceAttributes.systemId) && parseInt(m.source_port_no) == parseInt(sourceAttributes.portNo) && parseInt(m.destination_port_no) == parseInt(targetAttributes.portNo) && parseInt(m.destination_system_id) == parseInt(targetAttributes.systemId))
            || (parseInt(m.source_system_id) == parseInt(targetAttributes.systemId) && parseInt(m.source_port_no) == parseInt(targetAttributes.portNo) && parseInt(m.destination_system_id) == parseInt(sourceAttributes.systemId) && parseInt(m.destination_port_no) == parseInt(sourceAttributes.portNo))));
                AllConnectionData.splice(_findIndex, 1);

            }
            isAutoDeleteConnection = false;
        }, true, true);
    }
    this.filterConnections = function (obj, _isSource) {
        var _isvalid = true;
        //$(obj).next('.chosen-container').removeClass('NotValid');
        //if ($('#ddlDestinationPort').val() == '' && $('#ddlSourcePort').val() == '') {
        //    _isvalid = false;
        //    $('#ddlSourcePort').next('.chosen-container').removeClass('NotValid');
        //    $('#ddlDestinationPort').next('.chosen-container').removeClass('NotValid');
        //} else if ($('#ddlSourcePort').val() == '' && !_isSource) {
        //    _isvalid = false;
        //    $('#ddlSourcePort').next('.chosen-container').addClass('NotValid'); return false;
        //} else if ($('#ddlDestinationPort').val() == '' && _isSource) {
        //    _isvalid = false;
        //    $('#ddlDestinationPort').next('.chosen-container').addClass('NotValid'); return false;
        //}
        var _sourcePortId = parseInt($('#hdnSourceEqpmtId').val());
        var _destinationPortId = parseInt($('#hdnDestinationEqpmtId').val());
        if ($('#ddlSourcePort').val() != '' && $('#ddlDestinationPort').val() != '') {
            _sourcePortId = parseInt($('#ddlSourcePort').val());
            _destinationPortId = parseInt($('#ddlDestinationPort').val());
        }
        if (_isvalid) {
            var _data = {
                SourceEquipmentId: parseInt($('#hdnSourceEqpmtId').val()),
                DestinationEquipmentId: parseInt($('#hdnDestinationEqpmtId').val()),
                SourcePortId: _sourcePortId,
                DestinationPortId: _destinationPortId,
                isInsideConnectivity: ($('#hdnInsideConnectivity').val() == 'True')
            };
            ajaxReq('Splicing/ModelConnections', _data, true, function (resp) {
                $('#divSpliceWindow').html(resp);
                app.connectManyToMany();
                app.addEndPoint();
                app.connectOneToOne();
                app.setStatusColor();
                app.setModelNameColor();
                if ($('#hdnInsideConnectivity').val() == 'False') {
                    conBuilder.bindSplicingTool();
                }
                if ($('#hdnInsideConnectivity').val() == 'True') { app.blockConnectedPorts(); }
            }, false, true);
        }
    }
    this.setModelNameColor = function () {
        $.each($('.modelName'), function (i, item) {
            var allItem = $(this).html().split('/');
            var modelName = '';
            if (allItem.length > 1) {
                for (i = 0; i < allItem.length; i++) {
                    var color = ((allItem.length - 1) == i) ? '' : 'green';
                    modelName = modelName + '<span style="color:' + color + ';">/ ' + allItem[i] + '</span>'
                }
                $(this).html(modelName);
            }
        })
    }
    this.redirectToCPF = function (_systemId, _networkId, _entityType, _portNo) {
        pageTitleText = 'Connection Path Finder';
        modalClass = 'modal-xxl';
        popup.LoadModalDialog('CHILD', 'splicing/ConnectionPathFinder',
            {
                equipment_id: _networkId,
                objFilterAttributes:
                    {
                        entity_type: _entityType,
                        entityid: _systemId,
                        port_no: _portNo
                    }, isControllEnable: false
            }, pageTitleText, modalClass);
    }
    this.redirectToCustomerDetails = function (networkId, _entityType, systemId, portNo) {
        pageTitleText = 'Connected Customer Details';
        modalClass = 'modal-xxl';
        _data = { equipment_id: networkId, objFilterAttributes: { entity_type: _entityType, entityid: systemId, port_no: portNo }, isControllEnable: false };
        popup.LoadModalDialog('CHILD', 'splicing/GetConnectedCustomerDetails', _data, pageTitleText, modalClass);
    }
    this.insideConnectivity = function (_systemId) {       
        let _networkId = '';
        var _data = {
            SourceEquipmentId: _systemId,
            DestinationEquipmentId: _systemId,
            structureId: 0,
            SourceEquipmentNWId: _networkId,
            DestinationEquipmentNWId: _networkId,
            isInsideConnectivity: true
        }
        var dataURL = 'Splicing/FilterConnection';
        var titleText = 'Internal Connectivity';
        popup.LoadModalDialog('PARENT', dataURL, _data, titleText, 'modal-xxl',
            function () {
                $('#btnConnectionFilter').hide();
                setTimeout(app.InitilizeJsPlumb, 500);
                $('body #dvProgress').hide();
            });
    }
    this.redirectToUpdateFiber = function (pEntitySystemId, pCorePortNumber, pEntityType, ptype) {
         
        if (pEntityType == undefined) {
            pCorePortNumber = pEntitySystemId.data.pCorePortNumber;
            pEntityType = pEntitySystemId.data.pEntityType;
            ptype = pEntitySystemId.data.ptype;
            pEntitySystemId = pEntitySystemId.data.pEntitySystemId;
        }
        pageTitleText = pEntityType.toUpperCase() == "CABLE" ? 'Update Core Status' : 'Update Port Status';
        modalClass = 'modal-sm';
        popup.LoadModalDialog('CHILD', 'splicing/UpdateCorePortStatus', { entitySystemId: pEntitySystemId, entityType: pEntityType, corePortNumber: pCorePortNumber, type: ptype }, pageTitleText, modalClass);
    }
    this.showPatchCord = function (_patchCordId) {
        var pageTitleText = 'PatchCord Details';
        popup.LoadModalDialog('CHILD', 'Library/AddPatchCord', { system_id: _patchCordId }, pageTitleText, 'modal-lg');
    }
    this.changePortStatus = function (resp) {
        //$('.dropbox').slideUp();
        var direction = '';
        var endPointId = resp.entity_system_id + '_' + resp.entity_type.toUpperCase() + '_' + resp.core_port_number;
        direction = ($('#ddlLeftEquipment').val() == resp.entity_system_id ? 'Left' : 'Right');
        endPointId = direction + '_' + resp.entity_system_id + '_' + resp.entity_type.toUpperCase() + '_' + resp.core_port_number;

        app.endPointObject.hide(endPointId, true);
        if (resp.portStatus > 1) {
            $('#' + endPointId).attr('data-status-id', resp.portStatus).attr('title', resp.comment).css('background', resp.portColor);
            $('#' + endPointId).tooltip({ title: resp.comment, html: true, placement: "left" });
        }
        if (resp.portStatus == 1) {
            app.endPointObject.addEndpoint($("#" + endPointId), { anchor: 'Center' }, app.getEndPointOptions());
            $('#' + endPointId).attr('data-status-id', resp.portStatus).attr('title', '').css('background', resp.portColor);
            $('#' + otherEnd).tooltip({ title: '', html: true, placement: "left" });
        }
    }
    this.setVacantColor = function (obj) {
        var vacantColor = $('#hdnVacantColor').val();
        obj.siblings('._jsPlumb_endpoint')
            .find('circle')
            .attr('fill', vacantColor)
            .attr('stroke', 'none');
    }
    this.unSpliceAll = function () {
        if (app.currentConnections.length > 0) {
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
                            setTimeout(function () { alert(data.message); }, 100);
                            var vacantColor = $('#hdnVacantColor').val();
                            app.isAutoDeleteConnection = false;
                            app.unSpliceResetEndPoint();
                        }
                        catch (err) { app.isAutoDeleteConnection = false; setTimeout(function () { alert(err.message); }, 200); }
                    }

                }, true, true);
            });
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_026);//Connections does not exist in current window!
        }
    }
    this.unSpliceResetEndPoint = function () {
        $.each($('.endPint[data-status-id="2"]'), function (i, item) {
             
            var _systemId = $(item).attr('data-system-id');
            var _portNo = $(item).attr('data-port-no');
            var _entityType = $(item).attr('data-entity-type');
            var sibling = $(item).siblings('.connectionCount');
            var currentPort = app.currentConnections.filter(function (m) {
                return (parseInt(m.source_system_id) == parseInt(_systemId) && m.source_entity_type.toUpperCase() == _entityType.toUpperCase() && parseInt(m.source_port_no) == parseInt(_portNo))
                || (parseInt(m.destination_system_id) == parseInt(_systemId) && m.destination_entity_type.toUpperCase() == _entityType.toUpperCase() && parseInt(m.destination_port_no) == parseInt(_portNo))
            });
            var conCount = sibling.html();
            if (conCount != '') {
                conCount = parseInt(conCount);
                conCount = conCount - currentPort.length;
                conCount = conCount <= 0 ? 0 : conCount;
                sibling.html(conCount);
                if (conCount == 0) {
                    sibling.addClass('dvCountdisabled');
                    app.setVacantColor($(item));
                }
            }
        })
        app.connectionObjects = [];
        app.currentConnections = [];
        $('div[data-is-connected="1"]').attr('data-is-connected', '0').attr('data-status-id', '1');
    }
    this.createPatchCord = function () {
        var sourceData = $('#' + app.info.sourceId).data();
        var targetData = $('#' + app.info.targetId).data();
        var _data = {
            geom: '', networkIdType: 'A', isDirectSave: false, a_system_id: sourceData.systemId, a_entity_type: sourceData.entityType,
            a_location: sourceData.networkId, b_system_id: targetData.systemId,
            b_entity_type: targetData.entityType, b_location: targetData.networkId,
            structure_id: 0,
            isEquipmentPatching: true
        };
        var pageTitleText = 'PatchCord Details';
        popup.LoadModalDialog('CHILD', 'Library/AddPatchCord', { objPatch: _data }, pageTitleText, 'modal-lg');
    }
    this.savePatchConnection = function (resp) {
        if (app.connection != null && resp != null) {
            app.connection.equipment_entity_type = 'PatchCord';
            app.connection.equipment_system_id = resp.system_id;
            app.connection.equipment_network_id = resp.network_id;
            var connectionId = app.saveConnection([app.connection], app.info.sourceId, app.info.targetId, app.info);
            //info.connection.setPaintStyle({ strokeStyle: '#000000', lineWidth: 3 });
            //app.info.connection.addClass('PatchCord');
            //app.info.connection.setParameters(resp.system_id);
            //app.info.connection.bind('click', function (e) {
            //    app.patchActions(app.info.connection.sourceId, e);
            //})
            app.connection = null;
            app.info = null;
            $('#ChildPopUp #closeChildPopup').trigger("click");
        }
    }
    this.bindSplicingTool = function () {
        $('.sourceEqpmtName,.destinationEqpmtName').unbind('click').on("click", function () {
            if (!$(this).hasClass('downDropbox')) {
                $('.dropbox').slideUp();
                $('.sourceEqpmtName,.destinationEqpmtName').removeClass('downDropbox');
                $(this).next().slideDown(); $(this).addClass('downDropbox');
            } else { $(this).next().slideUp(); $(this).removeClass('downDropbox'); }
            var portId = $(this).attr('data-port-id');
            var _equipmentId = $('#' + portId).attr('data-system-id');
            var _portNo = $('#' + portId).attr('data-port-no');
            if ($('#' + portId).attr('data-status-id') == 2) {
                var buttonId = 'btnUpdatePortStatus_' + _equipmentId + '_' + _portNo;
                $('#' + buttonId).hide();
            } else {
                var buttonId = 'btnUpdatePortStatus_' + _equipmentId + '_' + _portNo;
                $('#' + buttonId).show();
            }
        });
    }
    this.showPrevConnections = function (_systemId, _networkId, _entityType, _portNo) {
        var titleText = 'Port Connections';
        var dataURL = 'Splicing/MultipleConnection';
        var _data = {
            source_system_id: _systemId, source_network_id: _networkId, source_entity_type: _entityType, source_port_no: -1 * _portNo, portType: 'I', isPreviousConnection: true
        };
        $('.dropbox').hide();
        $('#divPrevMultiConnection_' + _systemId + '_' + _portNo).children('#divConnectionContainer').html('');
        if (!$('#divPrevMultiConnection_' + _systemId + '_' + _portNo).hasClass('active')) {
            $('#divPrevMultiConnection_' + _systemId + '_' + _portNo).addClass('active');
            $('#divPrevMultiConnection_' + _systemId + '_' + _portNo).slideToggle();
            $('#divPrevMultiConnection_' + _systemId + '_' + _portNo).children('#dvProgress').show();
            ajaxReq(dataURL, _data, true, function (resp) {
                $('#divPrevMultiConnection_' + _systemId + '_' + _portNo).children('#divConnectionContainer').html(resp);
                $('#divPrevMultiConnection_' + _systemId + '_' + _portNo);//.children('#dvProgress').hide();
                $('#divPrevMultiConnection_' + _systemId + '_' + _portNo).css('background-image','none');
            }, false, false);
        } else {
            $('#divPrevMultiConnection_' + _systemId + '_' + _portNo).removeClass('active');
            $('#divPrevMultiConnection_' + _systemId + '_' + _portNo);//.children('#dvProgress').hide();
            $('#divPrevMultiConnection_' + _systemId + '_' + _portNo).css('background-image', 'url(../content/images/loading_new.gif)');
        }
    }
    this.resetPatchConnection = function () {
        if (app.info != null) {
            jsPlumb.detach(app.info);
        }
    }
    this.checkPatchTemplateExist = function (connection, info) {
        ajaxReq('Main/ChkEntityTemplateExist', { entityType: 'PatchCord', entitysubType: '' }, true, function (resp) {
            if (resp.status == "OK") {
                var patchDetails = app.patching.save(connection, info);
            } else { app.resetPatchConnection(); alert('Patch cord ' + resp.message); }

        }, true, false);

    }
    this.blockConnectedPorts = function () {
        $.each($('#dvRightCable  .endPint[data-is-connected="1"]'), function () {
            // 
            //var endPointId = $(this).attr('data-target-id');
            //if (endPointId.indexOf('EQUIPMENT') > 0 && (endPointId.indexOf('Left_') < 0)) {
            //    endPointId = 'Left_' + endPointId;
            //}
            //app.endPointObject.hide(endPointId, true);
        });
    }
    this.AddConnection = function (systemId) {
        pageTitleText = 'Add Connection';
        modalClass = 'modal-sm';
        popup.LoadModalDialog('CHILD', 'splicing/AddConnection', { systemId: parseInt(systemId) }, pageTitleText, modalClass);
    }
    this.bindPortsList = function (obj, targetddl) {
        var _connectionClass = 'singlAllowed';
        var option = '<option value="0">--Select--</option>';
        $(targetddl).html(option).val('0').trigger('chosen:updated');
        if ($(obj).val() != '') {
            var _cardId = $('#ddlCards').val();
            ajaxReq('Splicing/getPortByEquipment', { systemId: parseInt(_cardId) }, true, function (resp) {
                if (resp.length > 0) {
                    if (resp.length > 0) {
                        $.each(resp, function (i, item) {
                            if (item.is_multi_connection) { _connectionClass = 'multiAllowed'; }
                            option += '<option value=' + item.dropdown_value + ' class="' + _connectionClass + '" data-is-multi-allowed="' + item.is_multi_connection + '" data-port-status="' + item.port_status_id + '">' + item.dropdown_key + '</option>';
                        })
                    }
                    $(targetddl).html(option).trigger('chosen:updated');
                    $(targetddl + ' option[data-is-multi-allowed="false"][data-port-status="2"]').attr('disabled', true).trigger("chosen:updated");
                }
            }, true, true);
        }
    }
    this.SaveManulConnection = function () {
        var _isValid = true;
        if ($('#ddlLeftPorts').val() == '0' && $('#ddlRightPorts').val() == '0') {
            $('#ddlLeftPorts').next('.chosen-container').addClass('NotValid');
            $('#ddlRightPorts').next('.chosen-container').addClass('NotValid');
            _isValid = false;
        } else if ($('#ddlLeftPorts').val() == '0') {
            $('#ddlLeftPorts').next('.chosen-container').addClass('NotValid');
            _isValid = false;
        } else if ($('#ddlRightPorts').val() == '0') {
            $('#ddlRightPorts').next('.chosen-container').addClass('NotValid');
            _isValid = false;
        }
        var _leftPort = $('#ddlLeftPorts').val();
        var _rightPort = $('#ddlRightPorts').val();
        var _filterConnection = AllConnectionData.filter(m=>((parseInt(m.source_port_no) == parseInt(_leftPort) && parseInt(m.destination_port_no) == parseInt(_rightPort))
            || (parseInt(m.source_port_no) == parseInt(_rightPort) && parseInt(m.destination_port_no) == parseInt(_leftPort))));
        if (_filterConnection.length > 0) {
            alert('Connection already created from <b>' + $('#ddlLeftPorts option:selected').text() + '</b> to <b>' + $('#ddlRightPorts option:selected').text() + '</b>!');
            _isValid = false;
        }
        if (_isValid) {
            confirm('Connection will create from <b>' + $('#ddlLeftPorts option:selected').text() + '</b> to <b>' + $('#ddlRightPorts option:selected').text() + '</b> do you want to continue?', function () {
                var _equipmentId = $('#hdnSourceEqpmtId').val();
                var _source = 'Left_' + _equipmentId + '_EQUIPMENT_' + _leftPort;
                var _target = 'Left_' + _equipmentId + '_EQUIPMENT_' + _rightPort;
                var connection = app.getConnectionsData(_source, _target, true);
                app.saveConnection([connection], _source, _target, null);
                app.createConnection(_source, _target);
                $('#closeChildPopup').trigger("click");
            });
        }
    }
}