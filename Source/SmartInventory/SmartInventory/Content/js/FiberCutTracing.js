const urls = {
    getFiberTracing: 'FiberCutTracing/index',
    getEntities: 'FiberCutTracing/GetEquipmentSearchResult',
    getEntityPortInfo: 'Splicing/GetEquipmentPortInfo',
    getFiberTracingPath: 'FiberCutTracing/getFiberTracingPath',
    getFiberCutDetails: 'FiberCutTracing/getFiberCutDetails',
    getFiberNodeTypes: 'FiberCutTracing/getFiberNodeType',
    getGeometryDetail: 'Main/getGeometryDetail'
};
let DE = {
    FiberCut: '#dvFiberCut',
    ConnectivityBasedTracing: "#dvConnectivityFiberCut",
    RouteBasedTracing: "#dvRouteFiberCut",
    Export: "btnFiberExport",
    TracingEquipment: "#tracingEquipment",
    ddlCore: "#ddlCore",
    TracingPtahContainer: '#dvFiberCutTracingPath',
    entityType: "#hdnEntityType",
    entityId: 'hdnEntityId',
    btnTracing: '#btnShowTracingRoutes',
    entityid: '#hdnEntityId',
    ddlStreamType: '#ddlStreamType',
    tracingDistance: '#tracingDistance',
    dvFuberCutDetails: '#dvFuberCutDetails',
    reset: '#btnReset',
    viewOnMap: '#btnViewOnMap',
    exportDetails: '#btnFiberExport',
    lastTPLat: '#hdnLastTPLat',
    lastTPLong: '#hdnLastTPLong',
    lastTPType: '#hdnLastTPType',
    faultyFiberGeom: '#hdnFaultyFiberGeom',
    faultLat: '#hdnFaultLat',
    faultLong: '#hdnFaultLong',
    NoRecordFound: '#divNoRecordFound',
    NodeType: '#ddlNodeType',
    nodeTypeContainer: '#dvNodeType',
    FaultyFiberId: '#hdnFaultyFiberId'

}
var FiberCutTracing = (function () {
    var lineObj = null;
    var _faultObj = null;
    var _lastTpPoint = null;
    var _startGeom = null;
    var _endGeom = null;
    var _selectedEntity = null;
    var defaultHTMl = '<div id="divNoRecordFound" style="text-align: center; margin-top: 80px; padding: 30px; font-size: 15px;">';
    defaultHTMl+='<span>Use the above parameters to get the fiber cut details!</span>';
    defaultHTMl+='</div>';
    var action = {
        createFault: function (_faultgeometry) {
            if (_faultObj != null) { _faultObj.setMap(null); }
            //if (_lastTpPoint != null) { _lastTpPoint.setMap(null); }
            _faultObj = si.createMarker(_faultgeometry, 'Content/images/icons/lib/circle/FAULT.png');
            // get layer Detail...
            var lyrDetail = getLayerDetail("Fault");
            var pageUrl = lyrDetail['layer_form_url'];
            var titleText = lyrDetail['layer_title'];
            var eType = lyrDetail['layer_name'];
            var gType = lyrDetail['geom_type'];
            var saveUrl = lyrDetail['save_entity_url'];
            var isDirectSaveAllowed = (lyrDetail['is_direct_save'].toUpperCase() == "TRUE");
            var isTemplateRequired = (lyrDetail['is_template_required'].toUpperCase() == "TRUE");
            var ntkIdType = lyrDetail['network_id_type'];
            var pageTitleText = lyrDetail['layer_title'];
            let _faultyCabeId = $(DE.FaultyFiberId).val();
            si.clearTempNewEntity();
            _faultObj.setMap(si.map);
            _faultObj.setDraggable(false);
            google.maps.event.addListener(_faultObj, 'click', function () {
                var txtGeom = si.getGeomObjType(gType, _faultObj);
                var modalClass = getPopUpModelClass(eType);
                ajaxReq('Main/ValidateEntityGeom', { geomType: gType, enType: eType, txtGeom: txtGeom, isTemplate: isTemplateRequired }, false,
                       function (resp) {
                           $('#ModalPopUp .minmizeModel>i').trigger("click");
                           if (resp.status == "OK") {
                               if (isDirectSaveAllowed == true) {
                                   ajaxReq(saveUrl, { geom: txtGeom, networkIdType: ntkIdType, isDirectSave: true }, true, function (resp) {
                                       if (resp.status == "OK") {
                                           si.loadLayerOnEntity();
                                       }
                                       alert(resp.message);
                                   }, true, true);
                               }
                               else {                                   
                                   popup.LoadModalDialog('CHILD', pageUrl, { geom: txtGeom, networkIdType: ntkIdType, faultyFiber: _faultyCabeId }, pageTitleText, modalClass);
                               }
                           }
                           else {
                               alert(resp.error_message);
                           }
                       }, true, true);
            });
        },
        createEndPoint: function (geometry, noteType) {
            var latLngArr = si.getLatLongArr(geometry);
            _endGeom = si.createMarker(latLngArr[0], 'Content/images/icons/lib/KMLICONS/' + (noteType == 'A' ? 'Actual_Start' : 'End') + '.png');
            _endGeom.setMap(si.map);
        },
        getFiberNodes: function () {
            let _entityType = $(DE.entityType).val();
            let _entityId = $(DE.entityid).val();
            let _request = { systemId: _entityId, entityType: _entityType };
            API.call(urls.getFiberNodeTypes, _request, function (resp) {
                render.nodeTypeDropDown(resp);
                if (_entityType.toUpperCase() == 'CABLE') {
                    render.enableNodeType(true);
                    action.displayNode();
                }
            }, false, true, true);
        },
        getStreamTracing: function () {
            let _entityId = $(DE.entityid).val();
            let _entityType = $(DE.entityType).val();
            let _portNo = $('#ddlCore option:selected').val();
            let _nodeType = $(DE.NodeType).val();
            _nodeType = _entityType.toUpperCase() == 'CABLE' ? _nodeType : '';
            let _request = { systemId: parseInt(_entityId), entityType: _entityType, portNo: parseInt(_portNo), nodeType: _nodeType }
            API.call(urls.getFiberTracingPath, _request, function (resp) {
                render.streamDropDown(resp);
            }, false, true, true);
        },
        reset: function () {
            render.resetPathAnimation();
            $(DE.TracingEquipment).val('');
            $(DE.ddlCore).empty();
            $(DE.ddlCore).append($("<option></option>").val('0').html('--Select--')).trigger('chosen:updated');
            $(DE.ddlStreamType).empty();
            $(DE.ddlStreamType).append($("<option></option>").val('0').html('--Select--')).trigger('chosen:updated');
            $(DE.ddlStreamType).val('0').trigger('chosen:updated');
            $(DE.tracingDistance).val('');
            $(DE.viewOnMap).addClass('dvdisabled');
            $(DE.exportDetails).addClass('dvdisabled');
            $(DE.NoRecordFound).html('Use the above parameters to get the fiber cut details!');
            if (_faultObj != null) { _faultObj.setMap(null); }
            if (_lastTpPoint != null) { _lastTpPoint.setMap(null); }
            if (_startGeom != null) { _startGeom.setMap(null); }
            if (_endGeom != null) { _endGeom.setMap(null); }
            $(DE.nodeTypeContainer).addClass('dvdisabled');
            $(DE.NodeType).append($("<option></option>").val('0').html('--Select--')).trigger('chosen:updated');
            if (_selectedEntity != null) { _selectedEntity.setMap(null); }
            $(DE.dvFuberCutDetails).html(defaultHTMl);
        },
        displayNode: function () {
            if (_startGeom != null) { _startGeom.setMap(null); }
            if (_endGeom != null) { _endGeom.setMap(null); }
            let _nodeType = $(DE.NodeType).val();
            let _endPointGeom = $(DE.NodeType + ' option:selected').attr('data-node-geom');
            action.createEndPoint(_endPointGeom, _nodeType);
            action.getStreamTracing();
        },
        getGeometryDetail: function (_entityId, _entityType) {
            let _geomType = (_entityType == 'Cable' ? 'Line' : 'Point');
            let _request = { systemId: _entityId, geomType: _geomType, entityType: _entityType }
            API.call(urls.getGeometryDetail, _request, function (resp) {
                si.fitElementOnMap(resp.result);
                if (_entityType == 'Cable') { si.HighlightEntityOnMap(_geomType, resp.result); }
                else { render.displayEntity(resp.result.sp_geometry, _entityType); }
            }, false, true, true);
        }
    };
    var validation = {
        validateInputs: function () {
            let _isValid = true;
            if ($(DE.TracingEquipment).val() == '') { _isValid = false; $(DE.TracingEquipment).addClass('notvalid'); }
            else if ($(DE.ddlCore).val() == '0') { _isValid = false; $(DE.ddlCore).next('.chosen-container').addClass('notvalid'); }
            else if ($(DE.ddlStreamType).val() == '0') { _isValid = false; $(DE.ddlStreamType).next('.chosen-container').addClass('notvalid'); }
            else if ($(DE.tracingDistance).val() == '' || $(DE.tracingDistance).val() == '0') { _isValid = false; $(DE.tracingDistance).addClass('notvalid'); }
            else if ($(DE.tracingDistance).val() > parseFloat($(DE.ddlStreamType + " option:selected").attr('data-route-length')) && $(DE.ddlStreamType).val() == 'true') {
                alert('Distance can not be greater than Up stream route length!');
                _isValid = false;
            } else if ($(DE.tracingDistance).val() > parseFloat($(DE.ddlStreamType + " option:selected").attr('data-route-length')) && $(DE.ddlStreamType).val() == 'false') {
                alert('Distance can not be greater than Down stream route length!');
                _isValid = false;
            }
            return _isValid;
        }
    };
    var render = {
        autocomplete: function (data, response) {
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
                        label: item.display_name, value: item.network_id, entity_type: item.entity_type, entity_id: item.system_id, node_type: item.node_type, end_point_geom: item.end_point_geom
                    }//
                }))
            }
        },
        entitiyPorts: function (resp) {
            if (resp.status = 'OK') {
                $(DE.TracingEquipment).removeClass('input-validation-error');
                $("#ddlCore_chosen").removeClass('input-validation-error');
                $(DE.ddlCore).empty();
                $(DE.ddlCore).append($("<option></option>").val('0').html('--Select--'));
                $.each(resp.result, function (data, value) {
                    $(DE.ddlCore).append($("<option></option>").val(value.port_value).html(value.port_text));
                });
                $(DE.ddlCore).trigger('chosen:updated');
            }
            else {
                $("#ddlCore_chosen").addClass('input-validation-error');
                alert(resp.message);
            }
        },
        choosen: function () {
            $(' .chosen-select').chosen({ width: '100%', max_selected_options: 5, search_contains: true });
        },
        streamDropDown: function (resp) {
            $(DE.ddlStreamType).empty();
            $(DE.ddlStreamType).append($("<option></option>").val('0').html('--Select--'));
            $.each(resp, function (data, value) {
                $(DE.ddlStreamType).append($("<option data-geom='" + value.fiber_path_geom + "' data-route-length='" + value.fiber_length.toFixed(2) + "' style='background-color:" + (value.is_backword_path == false ? "rgb(239, 168, 168)" : "rgb(197, 200, 236)") + "'></option>").val(value.is_backword_path).html(value.stream_type + '(' + value.fiber_length.toFixed(2) + ' meter)'));
            });
            $(DE.ddlStreamType + ' option[data-geom="null"]').css({ 'color': '#000', 'cursor': 'not-allowed' }).attr('disabled', true).trigger("chosen:updated");
            $(DE.ddlStreamType).trigger('chosen:updated');
        },
        showTracingOnMap: function () {
            var geometry = $(DE.ddlStreamType + " option:selected").attr('data-geom');
            render.resetPathAnimation();
            if (geometry != '' && geometry != undefined && geometry != null && geometry != 'null') {
                render.showPathOnMap(geometry);
            }
        },
        resetPathAnimation: function () {
            if (lineObj != null) { lineObj.setMap(null); }
        },
        loadFiberCutDetails: function (resp) {
            $(DE.dvFuberCutDetails).html(resp);
            $(DE.viewOnMap).removeClass('dvdisabled');
            $(DE.exportDetails).removeClass('dvdisabled');
        },
        showPathOnMap: function (geometry) {
            geometry = getLatLongArr(geometry);
            var _isBackWord = $(DE.ddlStreamType).val();
            if (geometry.length > 0) {
                lineObj = si.createLine(geometry);
                var _color = '#ff0000';
                if (_isBackWord == 'true') {
                    _color = 'blue';
                }
                lineObj.strokeColor = _color;
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
                lineObj.setMap(si.map);
                setTimeout(function () { animateLine(lineObj, 0) }, 20);
            }
        },
        nodeTypeDropDown: function (resp) {
            $(DE.NodeType).empty();
            $.each(resp, function (data, value) {
                $(DE.NodeType).append($("<option data-node-geom='" + value.node_geom + "'></option>").val(value.node_value).html(value.node_text));
            });
            $(DE.NodeType).trigger('chosen:updated');
        },
        enableNodeType: function (_flag) {
            if (_flag) {
                $(DE.nodeTypeContainer).removeClass('dvdisabled');
            }
            else {
                $(DE.nodeTypeContainer).addClass('dvdisabled');
            }
        },
        displayEntity: function (geometry, entityType) {
            var latLngArr = si.getLatLongArr(geometry);
            _selectedEntity = si.createMarker(latLngArr[0], 'Content/images/icons/lib/KMLICONS/' + entityType + '.png');
            _selectedEntity.setMap(si.map);
        },
    };
    var event = {
        onFiberTracing: function () {
            var pageUrl = urls.getFiberTracing;
            var modalClass = 'modal-md';
            popup.LoadModalDialog('PARENT', pageUrl, {}, MultilingualKey.SI_OSP_GBL_GBL_GBL_277, modalClass);
        },
        onExportMenu: function () {
            $(this).next().slideToggle();
            var objArrow = $(this).find('i.fa');
            if (objArrow != undefined && objArrow != null) {
                if (objArrow.hasClass('fa-chevron-up')) {
                    objArrow.addClass('fa-chevron-down');
                    objArrow.removeClass('fa-chevron-up');
                } else {
                    objArrow.removeClass('fa-chevron-down');
                    objArrow.addClass('fa-chevron-up');
                }
            }
        },
        onAutoComplete: function (request, response) {
            API.call(urls.getEntities, { SearchText: $.trim(request.term) }, function (resp) {
                render.autocomplete(resp, response);
            }, false, false, true);
        },
        onApply: function () {
            render.resetPathAnimation();
            if (validation.validateInputs()) {
                let _entityId = $(DE.entityid).val();
                let _entityType = $(DE.entityType).val();
                let _portNo = $('#ddlCore option:selected').val();
                let _isBackWordPath = $(DE.ddlStreamType + " option:selected").val() == 'true';
                let _distance = $(DE.tracingDistance).val();
                let _nodeType = $(DE.NodeType).val();
                _nodeType = _entityType.toUpperCase() == 'CABLE' ? _nodeType : '';
                let _request = { systemId: parseInt(_entityId), entityType: _entityType, portNo: parseInt(_portNo), distance: parseFloat(_distance), nodeType: _nodeType, isBackWordPath: _isBackWordPath }
                API.call(urls.getFiberCutDetails, _request, function (resp) {
                    render.loadFiberCutDetails(resp);
                    let $btnViewOnMap = $(DE.viewOnMap);
                    $btnViewOnMap.on('click', event.onViewOnMap);
                }, false, true, true);
            }
        },
        onPortChange: function () {
            if ($(DE.ddlCore).val() != '0') { $(DE.ddlCore).next('.chosen-container').removeClass('notvalid'); }
            let _entityType = $(DE.entityType).val();
            if (_entityType.toUpperCase() == 'CABLE') {
                action.getFiberNodes();
            }
            else {
                action.getStreamTracing();
            }
        },
        onStreamTypeChange: function () {
            if ($(DE.ddlStreamType).val() != '0') { $(DE.ddlStreamType).next('.chosen-container').removeClass('notvalid'); }
            render.showTracingOnMap();
        },
        onReset: function () {
            action.reset();
        },
        onViewOnMap: function () {
            var bounds = new google.maps.LatLngBounds();
            $(popup.DE.MinimizeModel).trigger("click");
            let _lastTPLat = $(DE.lastTPLat).val();
            let _lastTPLong = $(DE.lastTPLong).val();
            let _lastTPType = $(DE.lastTPType).val();
            let _faultyFiberGeom = $(DE.faultyFiberGeom).val();
            let _faultLat = $(DE.faultLat).val();
            let _faultLong = $(DE.faultLong).val();
            render.resetPathAnimation();
            render.showPathOnMap(_faultyFiberGeom);
            var _tpgeometry = new google.maps.LatLng(parseFloat(_lastTPLat), parseFloat(_lastTPLong));
            var _faultgeometry = new google.maps.LatLng(parseFloat(_faultLat), parseFloat(_faultLong));
            _lastTpPoint = si.createMarker(_tpgeometry, 'Content/images/icons/lib/circle/' + _lastTPType + '.png');
            _lastTpPoint.setMap(si.map);
            action.createFault(_faultgeometry);
            var geometry = getLatLongArr(_faultyFiberGeom);
            for (var z = 0; z < geometry.length; z++) {
                bounds.extend(geometry[z]);
            }
            si.map.fitBounds(bounds);
        },
        onNodeChange: function () {
            action.displayNode();
        },
        onDistanceChange: function () {
            $(DE.tracingDistance).removeClass('notvalid');
        }
    };
    var bind = {
        mainToolBar: function () {
            let $connectivityTracing = $(DE.ConnectivityBasedTracing);
            $connectivityTracing.on('click', event.onFiberTracing);

            let $routeTracing = $(DE.RouteBasedTracing);
            $routeTracing.on('click', event.onFiberTracing);

            let $fiberCut = $(DE.FiberCut);
            $fiberCut.on('click', event.onFiberTracing);
        },
        commonActions: function () {
            let $exportFiberDetails = $(DE.Export);
            $exportFiberDetails.on('click', event.onExportMenu);

            let $ddlCore = $(DE.ddlCore);
            $ddlCore.on('change', event.onPortChange);

            let $ddlStreamType = $(DE.ddlStreamType);
            $ddlStreamType.on('change', event.onStreamTypeChange);

            let $fiberApply = $(DE.btnTracing);
            $fiberApply.on('click', event.onApply);

            let $btnReset = $(DE.reset);
            $btnReset.on('click', event.onReset);

            let $ddlNodeType = $(DE.NodeType);
            $ddlNodeType.on('change', event.onNodeChange);

            let $inputDistance = $(DE.tracingDistance);
            $inputDistance.on('keypress', event.onDistanceChange);
            
        },
        autocomplete: function () {
            let $TracingEquipment = $(DE.TracingEquipment);
            $TracingEquipment.autocomplete({
                source: function (request, response) {
                    event.onAutoComplete(request, response);
                },
                minLength: 3,
                select: function (e, ui) {
                    $('#dvViewConnectionPathFinder,#dvSchematicViewContainer').html('');
                    $('#divNoRecordExist').show();
                    var label = ui.item.value;
                    if (ui.item.entity_id == 0) {
                        e.preventDefault();
                    }
                    else {
                        e.preventDefault();
                        $(DE.TracingEquipment).val(ui.item.label);
                        $(DE.entityType).val(ui.item.entity_type);
                        $(DE.entityid).val(ui.item.entity_id);
                        API.call(urls.getEntityPortInfo, { entity_id: ui.item.entity_id, entity_type: ui.item.entity_type }, function (resp) {
                            render.entitiyPorts(resp);
                        }, false, true, true);
                    }
                    if (_selectedEntity != null) { _selectedEntity.setMap(null); }
                    action.getGeometryDetail(ui.item.entity_id, ui.item.entity_type);
                    // si.ShowEntityOnMap(ui.item.entity_id, ui.item.entity_type, (ui.item.entity_type == 'Cable' ? 'Line' : 'Point'));
                }
            });
        }
    };
    var load = {
        convertChoosen: function () { render.choosen(); }
    };
    var API = {
        call: function (url, param, callback, is_request_JSON, isLoaderRequired, is_response_JSON) {
            ajaxReq(url, param, true, function (res) {
                hideProgress();
                if (callback && typeof callback == 'function')
                    callback(res);
            }, is_request_JSON, isLoaderRequired, is_response_JSON);
        },
    };
    var setup = function () {

        //Load Data
        for (let e in load) {
            if (typeof load[e] == 'function')
                load[e]();
        }
        //Bind events
        for (let e in bind) {
            if (typeof bind[e] == 'function')
                bind[e]();
        }
    };
    var init = function (configs) {
        $.extend(true, DE, configs);
        setup();
    };
    return {
        init: init,
        reset: event.onReset
    };
})();