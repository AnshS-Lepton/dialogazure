var LandBase = function () {

    var app = this;
    this.pointCount = 0;
    this.firstPoint = null;
    this.mapListners = {};
    this.ParentModel = 'PARENT';
    this.lineBufferObj = null;
    this.gMapObj = {};
    var _isBufferEnable = false;
    var uploadId;
    var currentAction;
    var currentStep;//START,UPLOAD,VALIDATION,EXECUTION,DONE
    var isValidated;
    this.DE = {
        'chosen_select': '.chosen-select',
        'dvLandBaseLine': '#dvLandBaseLine',
        'dvlineHeader': '#dvlineHeader',
        "txtLandBaseLayerSearch": "#txtLandBaseLayerSearch",
        'DownloadLandBaseTemplate': '#btnDownloadLandBaseTemplate',
    }
    this.initApp = function () {
        app.bindLandBaseLayerEvents(); 
    }
    this.bindLandBaseLayerEvents = function () {
         
        $(app.DE.chosen_select).chosen({ width: "100%" });
        //$(app.DE.dvLandBaseLine).draggable({ scroll: false, containment: "window" });
        $(app.DE.dvLandBaseLine).draggable({ scroll: false, handle: "#dvlineHeader", containment: "window" });
        $('input[type=radio][name=chkLineBuffer]').change(function () {
             

            if (this.value == 'With') {
                $('#divLandBaseLinewidth').css('display', 'block');
                if ($('#ddlLandBaseLinewidth').val() == 0) {
                    $('#custLandBaseLinediv').css('display', 'block');
                }
                _isBufferEnable = true;
            }
            else if (this.value == 'Without') {
                $('#divLandBaseLinewidth').css('display', 'none');
                $('#custLandBaseLinediv').css('display', 'none');
                _isBufferEnable = false;
            }
        });

        $('#btnCancelLandBaseLineTemplae').on("click", function () {
             
            $('#dvLandBaseLine').css('display', 'none');
        });
        $('#closeLandBaseLineTemplate').on("click", function () {
             
            $('#dvLandBaseLine').css('display', 'none');

            if ($(".childtoolbar").find('.activeToolBar').hasClass('activeToolBar')) {
                $(".childtoolbar").find('.activeToolBar').removeClass('activeToolBar');
                //si.resetShapeTools();
                //si.removeEventListnrs('click');
            }
        });
       
       

    }
     

    //---------LAND BASE LAYERS------//
    this.ToolBar = {
        addLandbaseLayer: function (tbType, divObj) {
             
            var func = function () { };
            switch (tbType) {
                case 'AddLandBaseLayer':
                    LandBase.ToolBar.addRemoveActiveClass(divObj);
                    if ($('#dvLandBaseLine').css('display') == 'block') {
                        $('#dvLandBaseLine').css('display', 'none');
                    }
                    var offset = $('#dvAddLandBaseLayer').offset();
                    var posY = offset.top - $(window).scrollTop();
                    var posX = offset.left - $(window).scrollLeft(); 
                    $("#addLandbaseLayerToolBar").parent().css({ position: 'relative' });
                    $("#addLandbaseLayerToolBar").css({ position: 'absolute' });
                    LandBase.ToolBar.showhideChildMenu('#addLandbaseLayerToolBar', '.childtoolbar');
                    $("#InfoDiv").hide();
                    si.removeEventListnrs('click'); 
                    break; 
            }

        },
        showhideChildMenu: function (element, otherElements) {
            //console.log("running");
            $(otherElements).not(element).slideUp(100);
            if (element != '')
                $(element).slideToggle(200);
        },
        addRemoveActiveClass: function ($parentEle) {
             
            if (app.gMapObj.shapeObj)
                app.gMapObj.shapeObj.setMap(null);
            $(".LandBaseToolbar").find(".activeToolBar").removeClass("activeToolBar");
             $(".SurveyToolbar").find(".activeToolBar").removeClass("activeToolBar");
            if ($parentEle == '') {
                $(".iconBar").find(".activeToolBar").removeClass("activeToolBar");

            } else {
                $($parentEle).parent().find(".activeToolBar").not($parentEle).removeClass("activeToolBar");
                $($parentEle).toggleClass("activeToolBar");
            }

        }
    }

    this.AddLandBaseLayer = {
        initiateAddingLandBaseLayer: function (obj, shapeFlag) {
             
            app.resetShapeTools();
            si.clearTempNewEntity();
            app.clearTPRelatedObjects();

            $('#addLandbaseLayerToolBar .LandBaseIconBaricomoon').removeClass('activeToolBar');
            if ($(obj).hasClass('activeToolBar')) { $(obj).removeClass('activeToolBar'); } else { $('#addLandbaseLayerToolBar >.LandBaseIconBaricomoon >a').removeClass('activeToolBar'); $(obj).addClass('activeToolBar'); }

            if (si.gMapObj.shapeObj)
                si.gMapObj.shapeObj.setMap(null);
            si.removeEventListnrs('click');
            si.gMapObj.shapeType = shapeFlag;
            //si.gMapObj.purposeType = "Add LandBase Layer";
            si.gMapObj.purposeType = MultilingualKey.SI_OSP_GBL_GBL_GBL_157

            si.gMapObj.shapeArr = [];

            google.maps.event.addListener(si.map, 'click', LandBase.AddLandBaseLayer.handleShapeAddLandBaseLayer);
            $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
        },

        addEntity: function (obj, ShapeFlag) {
             
            if (app.gMapObj.shapeObj)
                app.gMapObj.shapeObj.setMap(null);
            $('#addLandbaseLayerToolBar .LandBaseIconBaricomoon').removeClass('activeToolBar');
            // }
            if ($(obj).hasClass('activeToolBar')) {

                $(obj).removeClass('activeToolBar');
                si.resetShapeTools();
                si.removeEventListnrs('click');


            } else {


                if (ShapeFlag == 'Line') {
                    var _isValidWidth = true;
                    var _bufferValue = 0;
                    _isValidWidth = app.ValidLineWidth(_isValidWidth, _bufferValue);
                    if (_isValidWidth) {
                        if ($('#dvLandBaseLine').css('display') == 'block') {
                            $('#landBaseLineCustomized').removeClass('NotValid');
                            $('#dvLandBaseLine').css('display', 'none');
                        } else {
                            $('#dvLandBaseLine').css('display', 'block');
                        }
                    }
                    else {
                        return false;
                    }
                }
                if (ShapeFlag != 'Line') {
                    if ($('#dvLandBaseLine').css('display') == 'block') {
                        $('#dvLandBaseLine').css('display', 'none');
                    }
                }

                var dragPT = new google.maps.Point(obj.pageX - 5, obj.pageY - 75);
                var libItem = this;
                si.resetShapeTools();
                app.clearTempNewEntity();
                si.clearTempNewEntity();
                app.clearTPRelatedObjects();
                app.pointCount = 0;
                app.firstPoint = null;
                si.gMapObj.libPath = [];
                google.maps.event.addListener(si.map, 'click', function (evt) {
                    if (ShapeFlag == 'Polygon') {
                        _isBufferEnable = false;
                        app.drawPolygonEntity(evt.latLng, libItem, ShapeFlag);
                    }
                    else if (ShapeFlag == 'Line') {
                          

                        app.drawAddLineEntity(evt.latLng, libItem, ShapeFlag, parseFloat($('#hdnbuffer_value').val()));
                    }
                    else {
                        _isBufferEnable = false;
                        app.createTempLibPt(evt.latLng, '', ShapeFlag); 
                    }

                });

                $('#addLandbaseLayerToolBar >.LandBaseIconBaricomoon >a').removeClass('activeToolBar');
                $(obj).addClass('activeToolBar');
            }
        }
    }
    this.createTempLibPt = function (latLng, libImg, ShapeFlag) {
        app.clearTempNewEntity();
        app.mapListners["TempLibItm"] = si.createMarker(latLng, libImg);
        app.mapListners["TempLibItm"].setDraggable(true);
        app.mapListners["TempLibItm"].setMap(si.map);
        app.mapListners["mapListners"] = google.maps.event.addListener(app.mapListners["TempLibItm"], 'click', function () {
            app.setLibToolEvents(ShapeFlag);
        });
    }
    this.drawPolygonEntity = function (latLng, libItem, ShapeFlag) {
        if (app.pointCount == 0 && app.firstPoint != null) { si.gMapObj.libPath = []; si.gMapObj.libPath.push(new google.maps.LatLng(app.firstPoint.getPath().getArray()[0].lat(), app.firstPoint.getPath().getArray()[0].lng())); }
        app.pointCount += 1;
        si.gMapObj.libPath.push(latLng);
        app.drawPolygonAndAttachEvents(ShapeFlag);
    }
    this.drawPolygonAndAttachEvents = function (ShapeFlag) {
        app.clearTempNewEntity();
        app.mapListners["TempLibItm"] = app.createPolygon(si.gMapObj.libPath);
        app.mapListners["TempLibItm"].setEditable(true);
        app.mapListners["TempLibItm"].setDraggable(false);
        app.mapListners["TempLibItm"].setMap(si.map);
        app.mapListners["mapListners"] = google.maps.event.addListener(app.mapListners["TempLibItm"], 'click', function (event) {
            if (event.vertex == undefined) {
                 
                // si.removeEventListnrs('click');
                si.map.setOptions({ draggableCursor: '' });
                app.setLibToolEvents(ShapeFlag);
            }
        });

        google.maps.event.addListener(app.mapListners["TempLibItm"], 'rightclick', function (event) {
            if (event.vertex == undefined) {
                return;
            } else {
                si.removeNodeNew(app.mapListners["TempLibItm"], event);
                app.drawPolygonAndAttachEvents();
            }
        });
        app.mapListners["TempLibItm"].getPaths().forEach(function (path, index) {
            google.maps.event.addListener(path, 'set_at', function (indx) {
                var newPath = path;
                si.gMapObj.libPath = newPath.getArray();
            });

            google.maps.event.addListener(path, 'insert_at', function (indx) {
                var newPath = path;
                si.gMapObj.libPath = newPath.getArray();
            });
        });
    }
    this.setLibToolEvents = function (ShapeFlag) {
         
        var centerLineGeom = '';  
        var eType = "LandBase";
        var pageTitleText = "LAND BASE LAYER";
        var isTemplate = false;
        var ntkIdType = "A";
        var txtGeom = null;
        txtGeom = si.getGeomObjType(ShapeFlag, app.mapListners["TempLibItm"]);

        if (txtGeom == false) {
            return false;
        }
        if (ShapeFlag == 'Line' && _isBufferEnable) {
            centerLineGeom = txtGeom;
            var _path = si.gMapObj.ROWPath;
            if (_path.length > 2) {
                _path.push(_path[0]);
                txtGeom = si.getGeomFromlatLngArr(_path);
            }
        }

        var modalClass = getPopUpModelClass(eType); 
        pageTitleText = "Landbase"; 
        ajaxReq('Main/ValidateLBEntityGeom', { geomType: ShapeFlag, enType: eType, txtGeom: txtGeom, isTemplate: isTemplate }, true,
            function (resp) {
                 
                if (resp.status == "OK") { 
                    si.showLibraryTools();
                    $('#LibraryTools a:nth-child(2)').off('click');
                    $('#LibraryTools a:nth-child(2)').on('click', function () {

                        var pageUrl = "LandBaseLayer/AddLandBase";

                        si.closeLibTools();
                        popup.LoadModalDialog(app.ParentModel, pageUrl, { geom: txtGeom, enType: ShapeFlag, networkIdType: ntkIdType, centerLineGeom: centerLineGeom, _isBufferEnable: _isBufferEnable, geomType: ShapeFlag }, pageTitleText, modalClass);

                    });
                }
                else {
                    alert(resp.error_message);
                    if (ShapeFlag == 'Polygon') {
                        google.maps.event.addListener(si.map, 'click', function (evt) {
                            si.drawPolygonEntity(evt.latLng, '');
                        });
                    }
                }
            }, true, true);
    }

    this.createPolygon = function (_path) {
        var tmpLine = new google.maps.Polygon({
            strokeColor: '#FF8800',
            fillColor: '#FF8800',
            fillOpacity: 0.3,
            strokeOpacity: 1,
            strokeWeight: 2,
            path: _path,
            editable: true,
            draggable: true
        });
        return tmpLine;
    }

    this.clearTempNewEntity = function () {
        if ($('#dvLandBaseLine').css('display') == 'block') {
            $('#dvLandBaseLine').css('display', 'none');
        }

        //app.clearTPRelatedObjects();
        if (app.mapListners["TempLibItm"]) {
            app.mapListners["TempLibItm"].setMap(null);
            app.mapListners["TempLibItm"] = null;
            si.gMapObj.lineflag = false;
            //$('#lengthAreaDiv').empty();

        }
        if (app.mapListners["mapListners"])
            app.mapListners["mapListners"] = null;
        if (si.entityOBJ.length > 0) {
            $.each(si.entityOBJ, function (key, value) { value.setMap(null); });
            si.entityOBJ = [];
            si.map.setOptions({ draggableCursor: '' });
            si.removeEventListnrs('click');

        }

    }
     

    this.drawAddLineEntity = function (latLng, libItem, ShapeFlag, _bufferValue) {
         // lb
        app.clearTempNewEntity();
        si.gMapObj.libPath.push(latLng);
        si.gMapObj.lineflag = true;
        app.mapListners["TempLibItm"] = si.createLine(si.gMapObj.libPath, _bufferValue > 0 ? _bufferValue : 0);
        app.mapListners["TempLibItm"].setEditable(true);
        app.mapListners["TempLibItm"].setMap(si.map);
        si.IsCreateLineExist();
        google.maps.event.addListener(app.mapListners["TempLibItm"], 'rightclick', function (event) {
             
            if (event.vertex == undefined) {
                return;
            } else {
                si.removeNode(app.mapListners["TempLibItm"], event.vertex);
            }
            si.IsCreateLineExist();
            if (_isBufferEnable) {
                app.createLineBuffer(app.mapListners["TempLibItm"].getPath().getArray(), _bufferValue);
            }
            ShowLineLength();
        });

        google.maps.event.addListener(app.mapListners["TempLibItm"].getPath(), 'set_at', function (indx) {
             
            var newPath = app.mapListners["TempLibItm"].getPath();
            si.gMapObj.libPath = newPath.getArray();
            si.IsCreateLineExist();
            if (_isBufferEnable) {
                app.createLineBuffer(si.gMapObj.libPath, _bufferValue);
            }
            ShowLineLength();
        });

        google.maps.event.addListener(app.mapListners["TempLibItm"].getPath(), 'insert_at', function (indx) {
             
            var newPath = app.mapListners["TempLibItm"].getPath();
            si.gMapObj.libPath = newPath.getArray();
            if (_isBufferEnable) {
                app.createLineBuffer(si.gMapObj.libPath, _bufferValue);
            }
            ShowLineLength();
        });


        app.mapListners["mapListners"] = google.maps.event.addListener(app.mapListners["TempLibItm"], 'click', function (evt) {
              
            si.EntityAttributeDetails.strokeWidth = _bufferValue;
            app.setLibToolEvents(ShapeFlag, _bufferValue);
             
        });
        if (_isBufferEnable) {
            app.createLineBuffer(app.mapListners["TempLibItm"].getPath().getArray(), _bufferValue);
        }

    }


    this.closeLibTools = function () {

        $('#LibraryTools').stop(true).animate({ 'margin-bottom': -30, 'opacity': '0' }, { queue: false, duration: 300, complete: function () { $(this).hide() } });
        $('#LibraryTools a:first-child').hide();
        $('#greyDiv').hide();
        if ($('#hdnGeomType').val() == 'Polygon') {
            google.maps.event.addListener(app.map, 'click', function (evt) {
                app.drawPolygonEntity(evt.latLng, '');
            });
        }

    }
    this.showHideCustomWidth = function () {
         
        if ($('#ddlLandBaseLinewidth').val() == "0") {
            //$('#custLandBaseLinediv').show();
            $('#custLandBaseLinediv').val('');
            $('#custLandBaseLinediv').css('display', 'block');
        } else {
            $('#custLandBaseLinediv').val('');
            $('#custLandBaseLinediv').css('display', 'none');
        }
    }
    this.LineTemplate = function (obj, ShapeFlag) {
          
        $('#addLandbaseLayerToolBar .LandBaseIconBaricomoon').removeClass('activeToolBar');
        if ($(obj).hasClass('activeToolBar')) { $(obj).removeClass('activeToolBar'); } else { $('#addLandbaseLayerToolBar >.LandBaseIconBaricomoon >a').removeClass('activeToolBar'); $(obj).addClass('activeToolBar'); }
        app.clearTempNewEntity();
        app.clearTPRelatedObjects();
        if ($(obj).hasClass('activeToolBar')) {
            $('#dvLandBaseLine').css('display', 'block');
        }
    }



    this.createLineBuffer = function (pointsArray, width) {
         
        var overviewPathGeo = []; 
        if (pointsArray.length >= 2) {
            if (app.lineBufferObj != null) {
                app.lineBufferObj.setMap(null);
            }
            for (var i = 0; i < pointsArray.length; i++) {
                overviewPathGeo.push([pointsArray[i].lng(), pointsArray[i].lat()]);
            }
            var geoInput = {
                type: "LineString",
                coordinates: overviewPathGeo
            };
            var geoReader = new jsts.io.GeoJSONReader();
            var geoWriter = new jsts.io.GeoJSONWriter();
            var geometry = geoReader.read(geoInput).buffer(((((parseFloat(width) + 0.2) / 2) / 1000) / 111.12), 0, 2);
            var polygon = geoWriter.write(geometry);
            var oLanLng = [];
            var oCoordinates;
            oCoordinates = polygon.coordinates[0];
             
            for (i = 0; i < oCoordinates.length; i++) {
                var oItem;
                oItem = oCoordinates[i];
                oLanLng.push(new google.maps.LatLng(oItem[1], oItem[0]));
            }

            app.lineBufferObj = new google.maps.Polygon({
                paths: oLanLng,
                map: si.map
            });
            si.gMapObj.ROWPath = app.lineBufferObj.getPath().getArray();
        }

    }


    this.resetShapeTools = function () { 
        if (app.gMapObj.shapeObj)
            app.gMapObj.shapeObj.setMap(null);
        app.gMapObj.shapeObj = null;

        app.clearTempNewEntity(); 
        if (app.lineBufferObj) {
            app.lineBufferObj.setMap(null);
            app.lineBufferObj = null;
        }
        var googlePolygon = new polygonProtoType();
        googlePolygon.clearEditableLines();
        googlePolygon.deleteProtoType();
    }


    this.clearTPRelatedObjects = function () {
        //landbase
         
        $("#TerminationPointDiv").hide();
        $("#lblTerminationPoint").text('Termination Point');
        $(app.DE.tblTerminationPoint + ' tbody tr').remove();
        si.removeInfoHoverItem();
        si.removeSrchHoverItem();
        si.lstTerminationPoint = [];
        si.EntityAttributeDetails = {};
        si.TerminationPoints = '';
        $("#searchTpoint").hide();
        $("#dvNoTerminatePoint").show();
        si.clearTempNewEntity();
        si.ClearMapAddressTool();
        si.ClearJobPackAddressTool();
        si.clearBulkEntityInfo();
        si.removeEventListnrs('click');
        si.map.setOptions({ draggableCursor: '' });

        $('#SplicingDiv').hide();
        si.clearEditObj();
        splicing.ClearSplicingRelatedObjs();
    }

    this.ValidLineWidth = function (_isValidWidth, _bufferValue) {
         
        var _chkRadioButton = $('input[name="chkLineBuffer"]:checked').val();
        var maxLimit = parseInt($('#hdnlBaseMaxWidthLimit').val());
        if ($('#ddlLandBaseLinewidth').val() != 0 && _chkRadioButton != 'Without' && _chkRadioButton != null && _chkRadioButton != undefined) {
            _bufferValue = $('#ddlLandBaseLinewidth').val()
            _isBufferEnable = true;
        }
        else if ($('#ddlLandBaseLinewidth').val() == 0 && _chkRadioButton != 'Without') {
            _bufferValue = $('#landBaseLineCustomized').val();
            _isBufferEnable = true;
            //-- Validate Buffer limit---
             

            if (_bufferValue.length == 0) {
                $('#landBaseLineCustomized').addClass('NotValid');
                _isValidWidth = false;
                $('#dvLandBaseLine').css('display', 'block');
                return false;
            }
            if (parseFloat(_bufferValue) > maxLimit) {
                $('#landBaseLineCustomized').addClass('NotValid');
                _isValidWidth = false;
                $('#dvLandBaseLine').css('display', 'block');
                alert('Maximum <b> ' + maxLimit + ' Mtr </b> width is allowed !');
                return false;
            }

            //----------
        }
        $('#hdnbuffer_value').val(parseFloat(_bufferValue));
        return _isValidWidth;
    }

    this.ValidateWidth = function () {
        var maxLimit = parseInt($('#hdnlBaseMaxWidthLimit').val());
        var _textValue = $('#landBaseLineCustomized').val();
        if ($('#landBaseLineCustomized').length > 0) {
            if (parseFloat(_textValue) > maxLimit) {
                $('#landBaseLineCustomized').addClass('NotValid');
            }
            else {
                $('#landBaseLineCustomized').removeClass('NotValid');
            }
        }
    }

    this.LandbaseLayerReport = {
        showMenu: function (divObj) {
            LandBase.ToolBar.addRemoveActiveClass(divObj);
            LandBase.ToolBar.showhideChildMenu('#LandBaseReportToolBar', '.childtoolbar');
            $("#InfoDiv").hide();
            si.removeEventListnrs('click');
            //app.gMapObj.shapeObj.setMap(null);
           
        },
        EntityExportReport: function (geom, modeType, radius, obj) {
             
            if (obj) {
                $('#dvNetworkActions .LandBaseIconBaricomoon').find(".activeToolBar").removeClass('activeToolBar');
            }
            if (geom != '' && geom != null) {
                ajaxReq('Report/ValidatePotentialArea', {
                    geom: geom, geomType: modeType, buff_Radius: radius
                }, true, function (resp) {
                    if (resp.status == 'FAILED' || resp.status == 'ERROR') {
                        alert(resp.message);
                        return false;
                    }
                    else {
                        popup.LoadModalDialog('PARENT', 'Report/LandBaseLayerExportReport', {
                            'objReportFilters.geom': geom, 'objReportFilters.geomType': modeType, 'objReportFilters.radius': radius, 'objReportFilters.layerName': ''
                        }, "Export Report", 'modal-md-new');
                    }
                }, true, true, true);
            }
            else {
                popup.LoadModalDialog('PARENT', 'Report/LandBaseLayerExportReport', {
                    eType: ''
                }, MultilingualKey.SI_OSP_GBL_GBL_GBL_043, 'modal-md-new');
            }

            $('#dvLandBaseLine').css('display', 'none');
           // $(".LandBaseToolbar").find(".activeToolBar").removeClass("activeToolBar");
           // $("#dvAddLandBaseLayer").removeClass("activeToolBar"); 
            $('#addLandbaseLayerToolBar').slideUp(100);
            $("#InfoDiv").hide();
            $(".infoSwitchLandBase").removeClass("activeToolBar");
            si.resetShapeTools();
            si.removeEventListnrs('click');
            si.clearTempNewEntity();
        },
        initiateDrawingsExportReport: function (obj, shapeFlag) {
             
            si.resetShapeTools();
            $('#LandBaseReportToolBar').find(".activeToolBar").removeClass('activeToolBar');
            if ($(obj).hasClass('activeToolBar')) { $(obj).removeClass('activeToolBar'); } else { $('#LandBaseReportToolBar >.LandBaseIconBaricomoon >a').removeClass('activeToolBar'); $(obj).addClass('activeToolBar'); }
            if (app.gMapObj.shapeObj)
                app.gMapObj.shapeObj.setMap(null);
            si.removeEventListnrs('click');
            app.gMapObj.shapeType = shapeFlag;
            app.gMapObj.purposeType = "ExportReport";
            app.gMapObj.shapeArr = [];

            google.maps.event.addListener(si.map, 'click', app.LandbaseLayerReport.handleShapeExportReportEvents);
        },

        handleShapeExportReportEvents: function (eventParam) {
             
            if (app.gMapObj.shapeObj)
                app.gMapObj.shapeObj.setMap(null);
            switch (app.gMapObj.shapeType) {
                case 'Polygon':
                    app.gMapObj.shapeArr.push(eventParam.latLng);
                    app.gMapObj.shapeObj = new google.maps.Polygon({
                        paths: app.gMapObj.shapeArr,
                        strokeColor: '#FF8800',
                        strokeOpacity: 0.6,
                        strokeWeight: 2,
                        fillColor: '#FF8800',
                        fillOpacity: 0.35,
                        editable: true,
                        draggable: true
                    });
                    google.maps.event.addListener(app.gMapObj.shapeObj, 'mouseup', app.calculateArea);
                    google.maps.event.addListener(app.gMapObj.shapeObj, 'rightclick', function (event) {
                        if (event.vertex == undefined) {
                            return;
                        } else {
                            removeVertex(app.gMapObj.shapeObj, event.vertex);
                        }
                    });

                    google.maps.event.addListener(app.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                        var newPath = app.gMapObj.shapeObj.getPath();
                        app.gMapObj.shapeArr = newPath.getArray();
                    });

                    google.maps.event.addListener(app.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                        var newPath = app.gMapObj.shapeObj.getPath();
                        app.gMapObj.shapeArr = newPath.getArray();
                    });
                    break;
                case 'Rectangle':
                    app.gMapObj.shapeArr.push(eventParam.latLng);
                    var rectBound = app.validateBounds();
                    app.gMapObj.shapeObj = new google.maps.Rectangle({
                        strokeColor: '#FF8800',
                        strokeOpacity: 0.6,
                        strokeWeight: 2,
                        fillColor: '#FF8800',
                        fillOpacity: 0.35,
                        editable: true,
                        draggable: true,
                        bounds: rectBound
                    });
                    google.maps.event.addListener(app.gMapObj.shapeObj, 'bounds_changed', app.calculateArea);

                    break;
                case 'Circle':
                    app.gMapObj.shapeArr = [];
                    app.gMapObj.shapeObj = new google.maps.Circle({
                        strokeColor: '#FF8800',
                        strokeOpacity: 0.6,
                        strokeWeight: 2,
                        fillColor: '#FF8800',
                        fillOpacity: 0.35,
                        center: eventParam.latLng, //new google.maps.LatLng(34.052234, -118.243684),   //event.latLng,     //circlArry[0],
                        radius: 200,
                        editable: true,
                        draggable: true
                    });
                    google.maps.event.addListener(app.gMapObj.shapeObj, 'radius_changed', app.calculateArea);
                    google.maps.event.addListener(app.gMapObj.shapeObj, 'rightclick', function (event) {
                        if (event.vertex == undefined) {
                            return;
                        } else {
                            removeVertex(app.gMapObj.shapeObj, event.vertex);
                        }
                    });

                    break;
                case 'PolyLine':

                    app.gMapObj.shapeArr.push(eventParam.latLng);
                    app.gMapObj.shapeObj = new google.maps.Polyline({
                        path: app.gMapObj.shapeArr,
                        strokeColor: '#FF8800',
                        strokeOpacity: 1,
                        strokeWeight: 2,
                        draggable: true

                    });

                    google.maps.event.addListener(app.gMapObj.RulerLine, 'click', function (event) {
                        addPolyPoint
                    });

                    app.gMapObj.RulerFlag = true;
                    break;
            }
            if (eventParam != 'PolyLine') {
                app.calculateArea();
                google.maps.event.addListener(app.gMapObj.shapeObj, 'click', app.LandbaseLayerReport.shapeClickInfoExportReport);
            }
            app.gMapObj.shapeObj.setMap(si.map);
        },
        shapeClickInfoExportReport: function () {

            switch (app.gMapObj.shapeType) {
                case 'Polygon':
                    app.LandbaseLayerReport.showExportReportDetail(app.gMapObj.shapeObj.getPath().getArray(), 'polygon', app.gMapObj.purposeType);
                    break;
                case 'Rectangle':
                    app.LandbaseLayerReport.showExportReportDetail(app.getRectanglePath(app.gMapObj.shapeObj), 'polygon', app.gMapObj.purposeType);
                    break;
                case 'Circle':
                    //getCirclePotential('circle', si.gMapObj.purposeType);
                    app.LandbaseLayerReport.getCircleExportReport('circle', app.gMapObj.purposeType);
                    break;
            }
        },
        showExportReportDetail: function (latLongArr, selectionType, purposeType) {
            var longLatArr = '';
            if (latLongArr.length > 0) {
                for (i = 0; i < latLongArr.length; i++) {
                    longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[i].lng() + ' ' + latLongArr[i].lat();
                }
                longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[0].lng() + ' ' + latLongArr[0].lat();
                app.LandbaseLayerReport.EntityExportReport(longLatArr, selectionType);
            }
        },

        getCircleExportReport: function (selectionType) {
            var lnglat = app.gMapObj.shapeObj.getCenter().lng() + ' ' + app.gMapObj.shapeObj.getCenter().lat();
            var circleRadius = app.gMapObj.shapeObj.getRadius();

            si.BulkProcessInfo.geom = lnglat;
            si.BulkProcessInfo.selection_type = selectionType;
            si.BulkProcessInfo.buff_Radius = circleRadius;
            si.BulkProcessInfo.ntk_type = 'P';
            app.LandbaseLayerReport.EntityExportReport(lnglat, selectionType, circleRadius);
        }
    }

    this.ExportLayerSummaryView = function (_eType) {
        popup.LoadModalDialog('CHILD', 'Report/ExportLBEntitySummaryView', {
            'objReportFilters.layerName': _eType
        }, "View Report", 'modal-xl');
    }
    this.clearToolbarSelection = function () {
         
        $(".LandBaseToolbar").find(".activeToolBar").removeClass("activeToolBar");
        $("#dvAddLandBaseLayer").removeClass("activeToolBar");
        app.clearTPRelatedObjects();
        LandBase.ToolBar.showhideChildMenu('#addLandbaseLayerToolBar', '.childtoolbar');
    }


    this.handleChClick = function (cb) {

         //lb
        var childId = $(cb).attr("data-layerid");
        var childnetworkClass = $(cb).attr("class").split(' ')[0];
        var childnetworkType = childnetworkClass.split('-')[1];
        var pAllId = app.getParentallid(childnetworkType);
        var parId = $(cb).attr("data-layerid").split('_')[1];
        var layerGroup = $(cb).attr("data-layergroup");
        var disCheckboxLength = $('input[type=checkbox]').filter('.' + childnetworkClass + '').length;


        if (childnetworkType != 'L') {

            if (cb.checked) {
                if ($('input[type=checkbox]:checked').filter('[data-layerid=' + childId + ']').filter("[data-networktype!='L']").length > 0) {
                    $('[data-layerid=' + parId + ']').not(":disabled").prop('checked', true);

                    if ($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').length == ($('.' + childnetworkClass + '').length - disCheckboxLength)) {
                        $('#' + pAllId).not(":disabled").prop('checked', true);
                    }
                }


            }
            else {
                if ($('input[type=checkbox]:checked').filter('[data-layerid=' + childId + ']').filter("[data-networktype!='L']").length == 0) {
                    $('[data-layerid=' + parId + ']').not(":disabled").prop('checked', false);
                }
                if (($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').length
                        - disCheckboxLength) < ($('.' + childnetworkClass + '').length - disCheckboxLength)) {
                    $('#' + pAllId).not(":disabled").prop('checked', false);
                }


            }
        }
        else {

            if (cb.checked) {
                //if ($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').length == ($('.' + childnetworkClass + '').length - disCheckboxLength)) {

                if ($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').length == disCheckboxLength) {
                    $('#' + pAllId).not(":disabled").prop('checked', true);
                }



                if ($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').length > 0) {
                    $('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').each(function (index) {
                        var targetLB = $(this).data("targetland")
                        $('#' + targetLB).prop('checked', true);
                    });
                }

            }
            else {
                if ($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').length < disCheckboxLength) {
                    $('#' + pAllId).not(":disabled").prop('checked', false);
                }
            }

        }
        //// Group layer checked unchecked
        // if ($('.layers .landbase li input[type=checkbox]:checked').filter('.checkbox-custom').filter('[data-layergroup=' + layerGroup + ']').length
        //== $('.layers .landbase li input[type=checkbox]').filter('.checkbox-custom').filter('[data-layergroup=' + layerGroup + ']').length) {
        //     $('#' + layerGroup).not(":disabled").prop('checked', true);

        // }
        // else {
        //     $('#' + layerGroup).not(":disabled").prop('checked', false);
        // }

        ///// Parent node checked/Unchecked

        if ($('.mainLBlyr:checkbox:checked').length == $('.mainLBlyr:checkbox').length) {
            $('#checkAll').prop('checked', true);
        }
        else {
            $('#checkAll').prop('checked', false);
        }

    }

    this.handlePrClick = function (cb) {
         
        var Pid = $(cb).attr("data-all");

        if (Pid != 'L') {

            if (cb.checked) {
                //$(".mainLBlyr .landbase .checkbox-custom").prop("checked", true);
                $(".landbase .checkbox-custom-LB").prop("checked", true);
                $(".checkbox-customgrp").not(":disabled").prop("checked", true);
                $(".chkLBnetwork-" + Pid).not(":disabled").prop('checked', true);

            }
            else {
                $(".chkLBnetwork-" + Pid).not(":disabled").prop('checked', false);

            }
        }
        else {

            $(".chkLBnetwork-" + Pid).not(":disabled").prop('checked', cb.checked);
        }


        if (Pid != 'L' && $('.checkbox-custom1:checkbox:unchecked').filter("[data-all!='L']").length == $('.checkbox-custom1').filter("[data-all!='L']").length) {
            $(".checkbox-customgrp").prop("checked", false);
            $(($('.layers .landbase li input[type=checkbox]:checked').filter('.checkbox-custom'))).attr('checked', false);
            $('#checkAll').prop('checked', false);
            $(".chkLBnetwork-L,#chkLBlabelAll").prop("checked", false);

        } else if (Pid == 'L' && $('.checkbox-custom1:checkbox:unchecked').filter("[data-all!='L']").length == $('.checkbox-custom1').filter("[data-all!='L']").length) {
            if (cb.checked) {
                //$(".mainLBlyr .landbase .checkbox-custom").prop("checked", true);
                $(".landbase .checkbox-custom-LB").prop("checked", true);
                $("#checkAllLandBaseLayers").prop("checked", true);
                console.log('1');
                $(".chkLBnetwork-" + Pid).not(":disabled").prop('checked', true);
                if ($('.checkbox-custom1:checkbox:unchecked').filter("[data-all!='L']").length == $('.checkbox-custom1').filter("[data-all!='L']").length) {
                    //$(".chknetwork-P,#chkPlannedAll,#checkAll").prop('checked', true);
                }

            }
        }

        if ($('.mainLBlyr:checkbox:checked').filter("[data-all!='L']").length > 0) {
            $('#checkAllLandBaseLayers').prop('checked', true);
            console.log('2');
        }
    }

    this.getParentallid = function (obj) {
        if (obj == 'P')
            return "chkLBPlannedAll";
        else if (obj == 'A')
            return "chkLBAsBuiltAll";
        else if (obj == 'D')
            return "chkLBDormentAll";
        else if (obj == 'L')
            return "chkLBlabelAll";
    }

    this.ExportLandBaseLayerSummaryView = function (_fileType) {
        window.location = appRoot + 'Report/DownloadLandBaselayerSummaryView?fileType=' + _fileType;
    }
    this.ExportLandBaseLayerReportSummary = function (_fileType, entityids,singleFileDownload) {
        window.location = appRoot + 'Report/DownloadLandBaseLayerReportSummary?fileType=' + _fileType + '&entityids=' + entityids+'&singleFileDownload='+ singleFileDownload;
    }


    $(app.DE.txtLandBaseLayerSearch).on('keyup', function (e) {
        
        app.loadSearchEngine();
    });

    this.loadSearchEngine = function () {        
       
        $(app.DE.txtLandBaseLayerSearch).autocomplete({

            source: function (request, response) {
                var res = ajaxReq('LandBaseLayer/GetLBLayerSearchResult', { SearchText: request.term }, true, function (data) {
                    if (data.geonames.length == 0) {
                        var result = [
                            {
                                label: MultilingualKey.SI_GBL_GBL_JQ_GBL_001
                            }
                        ];
                        response(result);
                    }
                    else {
                        response($.map(data.geonames, function (item) {                            
                            return {
                                label: item.label, value: item.label, geomType: item.geomType, entityName: item.entityName, systemID: item.systemId, entityId: item.entityID, region_id: item.region_id, province_id: item.province_id, landbase_layer_id: item.landbase_layer_id
                            }//
                        }))
                    }
                }, true, false);

            },
            minLength: 3,
            select: function (event, ui) {
                
                if (ui.item.label == MultilingualKey.SI_GBL_GBL_JQ_GBL_001) {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                }
                else {                    
                    event.preventDefault();
                    app.gtype = ui.item.geomType;
                    if (ui.item.entityName != null) {                        
                        $(app.DE.txtLandBaseLayerSearch).val(ui.item.entityName + ':' + ui.item.label);
                        app.ShowEntityOnMap(ui.item.systemID, ui.item.entityName, ui.item.geomType);

                        $(app.DE.txtEntitySearch).removeClass('ui-autocomplete-loading');
                        $('#chk_rLyr_' + ui.item.region_id).not(":disabled").prop('checked', true);
                        $('#chk_pLyr_' + ui.item.province_id).not(":disabled").prop('checked', true);
                        $('#chk_LBnLyr_' + ui.item.landbase_layer_id).not(":disabled").prop('checked', true);
                        $('#chk_nLBL_' + ui.item.landbase_layer_id).not(":disabled").prop('checked', true);
                        $(app.DE.lyrRefresh).trigger("click");
                    }
                    else {
                        $(app.DE.txtLandBaseLayerSearch).val(ui.item.label + ':');
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    }

    this.ShowEntityOnMap = function (systemID, eType, gType) {        
        ajaxReq('LandBaseLayer/getLBLayerGeometryDetail', { systemId: systemID, geomType: gType, entityType: eType }, false, function (resp) {
             
            if (resp.status = 'OK') {
                if (resp.result != null && resp.result != undefined) {
                    si.HighlightEntityOnMap(gType, resp.result);
                    //app.printPolygonEntityArea(resp.result);
                    si.fitElementOnMap(resp.result)
                }
            }
        }, true, false);
    }
    this.validateBounds=function() {
        var rectbound = new google.maps.LatLngBounds(app.gMapObj.shapeArr[0], app.gMapObj.shapeArr[app.gMapObj.shapeArr.length - 1]);
        if (app.gMapObj.shapeArr[0].lng() > app.gMapObj.shapeArr[app.gMapObj.shapeArr.length - 1].lng())
            rectbound = new google.maps.LatLngBounds(app.gMapObj.shapeArr[app.gMapObj.shapeArr.length - 1], app.gMapObj.shapeArr[0]);
        return rectbound;
    }
    this.getRectanglePath=function(objRect) {
        var rectBounds = objRect.getBounds();
        var ne = rectBounds.getNorthEast();
        var sw = rectBounds.getSouthWest();
        var nw = new google.maps.LatLng(ne.lat(), sw.lng());
        var se = new google.maps.LatLng(sw.lat(), ne.lng()); 
        return new Array(nw, ne, se, sw);
    }
    
this.calculateArea=function() {


    $('#lengthAreaDiv').show();
    var area = '';
    switch (app.gMapObj.shapeType) {
        case 'Polygon':
            area = app.getPolygonArea(app.gMapObj.shapeObj);
            break;
        case 'Rectangle':
            area = app.getRectArea(app.gMapObj.shapeObj);
            break;
        case 'Circle':
            area = getCircleArea(si.gMapObj.shapeObj);
            break;
        case 'Square':
            area = getRectArea(si.gMapObj.shapeObj);
            break;
    }
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>' + MultilingualKey.SI_OSP_GBL_JQ_FRM_159 + ':</b> ' + area + ' sq. km</span></div>');
}

this.getPolygonArea=function(objPoly) {
    var area = google.maps.geometry.spherical.computeArea(objPoly.getPath());
    return (area / 1000000).toFixed(1);
}

this.getRectArea=function(objRect) {
    var area = google.maps.geometry.spherical.computeArea(getRectanglePath(objRect));
    return (area / 1000000).toFixed(1);
}
    //this.toggleUpload = function () {
    //     
    //    var pageUrl = 'LandBaseDataUploader/UploadData';
    //    var modalClass = 'modal-xl';
    //    popup.LoadModalDialog(app.ParentModel, pageUrl, null, 'Upload Data', modalClass);
    //    //  app.addRemoveActiveClass('');
    //}

   
    //this.DownloadLandBaseTemplate = function () {
    //     
    //    isAuthenticate();
    //    let _entityType = $(app.DE.selEntity).val();
    //    let _templateType = $('#ddlTemplateType :selected').val();
    //    var geomType = $(app.DE.option_selected, $(app.DE.selEntity)).attr('label');
    //    if (_entityType == '0') {
    //        $(app.DE.lblMessage).html(app.StatusMessages.ENTITY_REQUIRED);
    //    }
    //    else if (_templateType == '0') {
    //        $(app.DE.lblMessage).html(app.StatusMessages.TEMPLATE_FILE_TYPE_REQUIRED);
    //    }
    //    else {
    //        $.ajax({
    //            url: applicationUrl + '/LandBaseDatauploader/CheckTemplateExist',
    //            type: "POST",
    //            processData: true, // Not to process data
    //            data: { entityType: _entityType },
    //            success: function (result) {
    //                if (result.status) {
    //                    window.location = appRoot + 'LandBaseDatauploader/downloadTemplate?entityType=' + _entityType + '&templateType=' + _templateType + '&geomType=' + geomType;
    //                } else {
    //                    alert(result.message);
    //                }
    //            },
    //            error: function (err) {
    //                alert(err.statusText);
    //            }
    //        });
    //    }
    //}

    //this.templateType = function (obj) {
    //     
    //    var _templateType = $('#ddlTemplateType :selected').val();
    //    // var _templateType = $(obj).attr('data-template-type');
    //    $(app.DE.divUpdKML).hide();
    //    $(app.DE.divUpdExcel).hide();
    //    $(app.DE.divUpdTab).hide();
    //    $(app.DE.divUpdDxf).hide();
    //    $(app.DE.divUpdShape).hide();
    //    switch (_templateType) {
    //        case 'KML': { $(app.DE.divUpdKML).show(); $(app.DE.divDxfSourceId).hide(); break; }
    //        case 'SHP': { $(app.DE.divUpdShape).show(); $(app.DE.divDxfSourceId).hide(); break; }
    //        case 'DXF': {


    //            $(app.DE.divUpdDxf).show();
    //            $(app.DE.divDxfSourceId).show();
    //            ajaxReq('datauploader/getDxfSourceList', null, true, function (response) {
    //                //$('#ddlSourceList').html('');
    //                 

    //                var result = $.parseJSON(response);

    //                $('#demo').inputpicker({
    //                    data: result,
    //                    //url: 'datauploader/getDxfSourceList',
    //                    fields: ['epsg', 'aoi', 'dsname'],
    //                    fieldText: 'aoi',
    //                    fieldValue: 'epsg',
    //                    filterOpen: true,
    //                    autoOpen: true,
    //                    width: '100%',
    //                    height: '200px'
    //                });
    //                // console.log(result.length);
    //                //console.log(result[0].aoi);
    //                //var options = "<option value>--Select--</option>";
    //                // for (var i = 0; i < result.length; i++) {
    //                //     options += '<option value="' + result[i].epsg + '">' + result[i].aoi + '</option>';
    //                // }
    //                // $('#ddlSourceList').append(options);
    //                // $('#ddlSourceList').trigger("chosen:updated");


    //            }, false, true);

    //            break;
    //        }
    //        case 'TAB': {
    //            $(app.DE.divUpdTab).show();
    //            $(app.DE.divDxfSourceId).hide();
    //            break;
    //        }
    //        default: { $(app.DE.divUpdExcel).show(); $(app.DE.divDxfSourceId).hide(); break; }
    //    }
    //}
    //this.fnDownloadUploadLogs = function (id, status) {
    //    isAuthenticate();
    //    app.checkLogFileExist(id, status)

    //}
    //this.DownloadLogs = function () {
    //    app.fnDownloadUploadLogs(app.uploadId, 'ALL')
    //}
    //this.checkLogFileExist = function (id, status) {
    //    ajaxReq('FileDownload/checkLogFileExist', { id: id, status: status }, false, function (resp) {
    //        if (resp.status == 'FAILED') {
    //            alert(resp.message);
    //        } else { window.location = applicationUrl + '/FileDownload/DownloadUploadLogs?id=' + id + '&status=' + status; }
    //    }, true, false);
    //}
    //this.ShowUploadSummary = function () {
    //    ajaxReq('LandBaseDataUploader/getUploadSummary', null, true, function (resp) {
    //        $(app.DE.UploadLogsContainer).html(resp);
    //    }, false, false);
    //}

    //this.showHideControls = function (action, message, result) {
    //    app.currentAction = action;
    //    if (action == app.Actions.PAGE_LOAD) {
    //        app.PageLoad();
    //    }
    //    else if (action == app.Actions.ENTITY_TYPE_CHANGE) {
    //        app.EntityTypeChange();
    //    }
    //    else if (action == app.Actions.FILE_UPLOAD_START) {
    //        app.FileUploadStart();
    //    }
    //    else if (action == app.Actions.FILE_UPLOADED_SUCCESS) {
    //        app.FileUploadedSuccess();
    //    }
    //    else if (action == app.Actions.FILE_UPLOADED_FAILED) {
    //        app.FileUploadedFailed(message)
    //    }
    //    else if (action == app.Actions.FILE_INVALID_INPUTS) {
    //        app.FileInvalidInputs(message);
    //    }
    //    else if (action == app.Actions.FILE_INVALID_FILE) {
    //        app.FileInvalidFile(message);
    //    }
    //    else if (action == app.Actions.VALIDATING) {
    //        app.Validating();
    //    }
    //    else if (action == app.Actions.VALIDATED) {
    //        app.Validated(result);
    //    }
    //    else if (action == app.Actions.EXECUTE) {
    //        app.Execute();
    //    }
    //    else if (action == app.Actions.EXECUTED) {
    //        app.Executed();
    //    }
    //    else if (action == app.Actions.EXECUTION_FAILED) {
    //        app.ExecutionFailed();
    //    }
    //    else if (action == app.Actions.DONE) {
    //        app.Done();
    //    }
    //    else if (action == app.Actions.PREVIOUS_1) {
    //        app.Previous1();
    //    }
    //    else if (action == app.Actions.PREVIOUS_2) {
    //        app.Previous2();
    //    }
    //    else if (action == app.Actions.PREVIOUS_3) {
    //        app.Previous3();
    //    }
    //    else if (action == app.Actions.COLUMN_MAPPING) {
    //        app.columnMapping();
    //    }
    //}

    //this.PageLoad = function () {
    //    $(app.DE.step1status).text('');
    //    $(app.DE.step2status).text('');
    //    $(app.DE.step3status).text('');
    //    $(app.DE.step4status).text('');
    //    app.hideSteps();
    //    app.showStep(app.DE.step0);
    //    $(app.DE.selEntity).val('0').change();
    //    $(app.DE.selEntity).trigger(app.DE.chosen_updated);
    //    $(app.DE.step1badge).removeClass(app.DE.badgeActive);
    //    $(app.DE.step2badge).removeClass(app.DE.badgeActive);
    //    $(app.DE.step3badge).removeClass(app.DE.badgeActive);
    //    $(app.DE.step4badge).removeClass(app.DE.badgeActive);
    //    $(app.DE.uploadinfo).hide();
    //    $(app.DE.step1imgcheck).attr('src', app.Images.loader);
    //    $(app.DE.uploadcontrols).hide();
    //    $(app.DE.btnPrevious1).removeAttr('disabled');
    //    $(app.DE.btnPrevious2).removeAttr('disabled');
    //    $(app.DE.btnPrevious3).removeAttr('disabled');
    //    $(app.DE.radioBtnNew).prop('checked', true);
    //}
    //this.EntityTypeChange = function () {
    //    app.hideSteps();
    //    app.showStep(app.DE.step0);
    //    $(app.DE.divUpdExcel).hide();
    //    $(app.DE.divUpdKML).hide();
    //    $(app.DE.btnUpload).show();
    //    $(app.DE.lblMessage).empty();
    //    $(app.DE.clearKMLFile).hide();
    //    $(app.DE.clearExcelFile).hide();
    //    $(app.DE.clearDxfFile).hide();
    //    $(app.DE.clearTabFile).hide();
    //    $(app.DE.updKML).val(null);
    //    $(app.DE.updExcel).val(null);
    //    $(app.DE.updDxf).val(null);
    //    $(app.DE.updTab).val(null);
    //    $(app.DE.updSHP).val(null);
    //    $(app.DE.divUpdShape).hide();
    //    $(app.DE.divUpdDxf).hide();
    //    $(app.DE.divUpdTab).hide();
    //    $(app.DE.clearShapeFile).hide();
    //}
    //this.FileUploadStart = function () {
    //    app.hideSteps();
    //    app.showStep(app.DE.step1);
    //    app.currentStep = app.Steps.step1;

    //    $('#' + app.currentStep + app.DE.status).text('Uploading file...');
    //    $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.loader);
    //    $(app.DE.btnPrevious1).attr('disabled', 'disabled');
    //    //$(app.DE.btnNext).attr('disabled', 'disabled');
    //    $(app.DE.btnNextStep2).attr('disabled', 'disabled');

    //}
    //this.FileUploadedSuccess = function () {
    //    app.hideSteps();
    //    app.showStep(app.DE.step1);
    //    $(app.DE.step1badge).addClass(app.DE.badgeActive);
    //    $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXCEL_UPLOADED_SUCCESSFULLY);
    //    $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
    //    //$(app.DE.btnNext).removeAttr('disabled');
    //    $(app.DE.btnNextStep2).removeAttr('disabled');
    //    $(app.DE.btnPrevious1).removeAttr('disabled');
    //}
    //this.FileUploadedFailed = function (message) {
    //    app.hideSteps();
    //    app.showStep(app.DE.step0);
    //    $(app.DE.lblMessage).html(message);

    //}

    //this.FileInvalidInputs = function (message) {
    //    let _result = $('<textarea />').html(message).text();
    //    app.hideSteps();
    //    app.showStep(app.DE.step0);
    //    alert(_result);
    //}
    //this.FileInvalidFile = function (message) {
    //    app.hideSteps();
    //    app.showStep(app.DE.step0);
    //    $(app.DE.lblMessage).html(message);
    //}
    //this.Validating = function () {
    //    app.currentStep = app.Steps.step3;
    //    $('#' + app.currentStep + app.DE.status).show();
    //    $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.VALIDATION_STARTED);
    //    app.hideSteps();
    //    app.showStep(app.DE.step3);
    //    $(app.DE.imgcheck).attr('src', app.Images.loader).show();
    //    $(app.DE.uploadinfo).hide();
    //    $(app.DE.TotalRecord).hide();
    //    $(app.DE.ValidRecord).hide();
    //    $(app.DE.InvalidRecord).hide();
    //    $(app.DE.btnExecute).attr('disabled', 'disabled').show();
    //    $(app.DE.btnPrevious3).attr('disabled', 'disabled').show();
    //}
    //this.Validated = function (result) {
    //    app.hideSteps();
    //    app.showStep(app.DE.step3);
    //    $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.VALIDATION_DONE);
    //    $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.check);
    //    $(app.DE.step3badge).addClass(app.DE.badgeActive);
    //    $(app.DE.uploadinfo).show();
    //    if (result != null) {
    //        $(app.DE.TotalRecord).text(result.total_record);
    //        $(app.DE.ValidRecord).text(result.success_record);
    //        $(app.DE.InvalidRecord).text(result.failed_record);
    //    }
    //    $(app.DE.TotalRecord).show();
    //    $(app.DE.ValidRecord).show();
    //    $(app.DE.InvalidRecord).show();
    //    $(app.DE.btnPrevious3).removeAttr('disabled');
    //    $(app.DE.btnExecute).removeAttr('disabled');
    //}
    //this.Execute = function () {
    //    app.currentStep = app.Steps.step4;
    //    $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXECUTING);
    //    $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.loader);
    //    app.hideSteps();
    //    app.showStep(app.DE.step4);
    //    //$(app.DE.btnPrevious2).hide();
    //    $(app.DE.btnPrevious3).hide();
    //    $(app.DE.btnExecute).attr('disabled', 'disabled');
    //    $(app.DE.btnDone).attr('disabled', 'disabled').show();
    //}
    //this.Executed = function () {
    //    $(app.DE.step4badge).addClass(app.DE.badgeActive);
    //    $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.check);
    //    $(app.DE.btnExecute).hide();
    //    $(app.DE.btnDone).removeAttr('disabled');
    //}
    //this.ExecutionFailed = function () {
    //    $(app.DE.step4badge).addClass(app.DE.badgeActive);
    //    $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXECUTION_FAILED);
    //    $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.failed);
    //    $(app.DE.btnDone).removeAttr('disabled');

    //}
    //this.Done = function () {
    //    app.showHideControls(app.Actions.PAGE_LOAD);
    //    $(app.DE.Uploadlogs).trigger("click");
    //    $(app.DE.btnExecute).hide();
    //}
    //this.Previous1 = function () {
    //    app.hideSteps();
    //    app.showStep(app.DE.step0);
    //    $(app.DE.step1badge).removeClass(app.DE.badgeActive);
    //    $(app.DE.btnUpload).show();
    //    $(app.DE.clearKMLFile).hide();
    //    $(app.DE.clearShapeFile).hide();
    //    //$(app.DE.clearExcelFile).hide();
    //}
    //this.Previous2 = function () {
    //    app.hideSteps();
    //    app.showStep(app.DE.step1);
    //    $(app.DE.step2badge).removeClass(app.DE.badgeActive);
    //    $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXCEL_UPLOADED_SUCCESSFULLY);
    //    $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
    //    //$(app.DE.btnNext).removeAttr('disabled');
    //    $(app.DE.btnNextStep2).removeAttr('disabled');
    //    $(app.DE.btnPrevious1).removeAttr('disabled');

    //}
    //this.Previous3 = function () {
    //    app.hideSteps();
    //    app.showStep(app.DE.step2);
    //    $(app.DE.step3badge).removeClass(app.DE.badgeActive);
    //    $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.COLUMN_MAPPED_SUCCESS);
    //    $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
    //    //$(app.DE.btnNext).removeAttr('disabled');
    //    $(app.DE.btnNextStep2).removeAttr('disabled');
    //    $(app.DE.btnPrevious1).removeAttr('disabled');

    //}
    //this.hideSteps = function () {
    //    $(app.DE.step0).hide();
    //    $(app.DE.step1).hide();
    //    $(app.DE.step2).hide();
    //    $(app.DE.step3).hide();
    //    $(app.DE.step4).hide();
    //}
    //this.columnMapping = function () {
    //    app.currentStep = app.Steps.step2;
    //    app.hideSteps();
    //    app.showStep(app.DE.step2);
    //    $(app.DE.step2badge).addClass(app.DE.badgeActive);
    //}
    //this.showStep = function (obj) {
    //    $(obj).show();
    //}

    //this.Steps = {
    //    step1: "step1",
    //    step2: "step2",
    //    step3: "step3",
    //    step4: "step4"
    //}
    //this.ExecuteData = function () {
    //    app.showHideControls(app.Actions.EXECUTE);
    //    ajaxReq('DataUploader/ProcessData', { uploadId: app.uploadId }, true, function (resp) {
    //        if (resp.status == app.StatusMessages.OK) {
    //            app.showHideControls(app.Actions.EXECUTED);
    //        }
    //        else {
    //            app.showHideControls(app.Actions.EXECUTION_FAILED);
    //        }
    //    }, false, false);
    //}

    //this.ValidateData = function () {
    //    app.showHideControls(app.Actions.VALIDATING);
    //    ajaxReq('DataUploader/ValidateData', { uploadId: app.uploadId }, true, function (resp) {
    //        if (resp.status == app.StatusMessages.OK) {
    //            app.isValidated = true;
    //            app.showHideControls(app.Actions.VALIDATED, null, resp);
    //            if (resp.failed_record > 0) {
    //                $(app.DE.downloadFailedLogs).show();
    //                $(app.DE.DownloadIcon).show();
    //            }
    //            if (resp.success_record == 0 && resp.failed_record > 0) {
    //                $(app.DE.btnExecute).attr('disabled', 'disabled');
    //            }
    //            else {
    //                $(app.DE.btnExecute).removeAttr('disabled');
    //            }
    //        }
    //        else {
    //            alert(app.StatusMessages.VALIDATION_ISSUE);
    //        }
    //    }, false, false);
    //}
    //this.Validatefiletype = function (filename, filecontrol) {
    //    var valid_extentions = filecontrol.get(0).accept.split(',');
    //    var position = filename.lastIndexOf('.');
    //    var extention = '';
    //    if (position != -1) {
    //        extention = filename.substr(position);
    //        return valid_extentions.includes(extention);
    //    }

    //}
    //this.ValidateExcelFile = function (excelFiles) {
    //    // check excel file selected or not.
    //    if (excelFiles.length == 0) {
    //        $(app.DE.lblMessage).html(app.StatusMessages.EXCEL_FILE_REQUIRED);
    //        return false;
    //    }
    //    // check file extension.
    //    if (excelFiles.length > 0) {
    //        if (!app.Validatefiletype(excelFiles[0].name.toLowerCase(), $(app.DE.updExcel))) {
    //            $(app.DE.lblMessage).html(app.StatusMessages.VALID_EXCEL_FILE_REQUIRED + $(app.DE.updExcel).get(0).accept);
    //            return false;
    //        }
    //    }
    //    return true;
    //}
    //this.ValidateKMLFile = function (kmlFiles) {
    //    // check KML file selected or not.
    //    if (kmlFiles.length == 0) {
    //        $(app.DE.lblMessage).html(app.StatusMessages.KML_FILE_REQUIRED);
    //        return false;
    //    }
    //    // check file extension.
    //    if (kmlFiles.length > 0) {
    //        if (!app.Validatefiletype(kmlFiles[0].name.toLowerCase(), $(app.DE.updKML))) {
    //            $(app.DE.lblMessage).html(app.StatusMessages.VALID_KML_FILE_REQUIRED + $(app.DE.updKML).get(0).accept);
    //            return false;
    //        }
    //    }
    //    return true;
    //}
    //this.ValidateSHPFile = function (shpFiles) {
    //    // check Shape file selected or not.
    //    if (shpFiles.length == 0) {
    //        $(app.DE.lblMessage).html(app.StatusMessages.SHP_FILE_REQUIRED);
    //        return false;
    //    }
    //    if (shpFiles.length > 4) {
    //        $(app.DE.lblMessage).html(app.StatusMessages.MAX_SHP_FILE);
    //        return false;
    //    }
    //    // check file extension.
    //    if (shpFiles.length > 0) {
    //        for (var i = 0; i < shpFiles.length; i++) {
    //            if (!app.Validatefiletype(shpFiles[i].name.toLowerCase(), $(app.DE.updSHP))) {
    //                $(app.DE.lblMessage).html(app.StatusMessages.VALID_SHP_FILE_REQUIRED + $(app.DE.updSHP).get(0).accept);
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //}
    //this.ValidateDxfFile = function (dxfFiles) {
    //    // check Shape file selected or not.
    //    if (dxfFiles.length == 0) {
    //        $(app.DE.lblMessage).html(app.StatusMessages.DXF_FILE_REQUIRED);
    //        return false;
    //    }

    //    // check file extension.
    //    if (dxfFiles.length > 0) {
    //        for (var i = 0; i < dxfFiles.length; i++) {
    //            if (!app.Validatefiletype(dxfFiles[i].name.toLowerCase(), $(app.DE.updDxf))) {
    //                $(app.DE.lblMessage).html(app.StatusMessages.VALID_DXF_FILE_REQUIRED + $(app.DE.updDxf).get(0).accept);
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //}
    //this.ValidateTabFile = function (tabFiles) {
    //    // check Shape file selected or not.
    //    if (tabFiles.length == 0) {
    //        $(app.DE.lblMessage).html(app.StatusMessages.TAB_FILE_REQUIRED);
    //        return false;
    //    }

    //    // check file extension.
    //    if (tabFiles.length > 0) {
    //        for (var i = 0; i < tabFiles.length; i++) {
    //            if (!app.Validatefiletype(tabFiles[i].name.toLowerCase(), $(app.DE.updTab))) {
    //                $(app.DE.lblMessage).html(app.StatusMessages.VALID_TAB_FILE_REQUIRED + $(app.DE.updTab).get(0).accept);
    //                return false;
    //            }
    //        }
    //    }
    //    return true;
    //}
    //this.UploadData = function () {

    //    if (window.FormData !== undefined) {
    //        // var _templateType = $('#divTemplateType>.radio-inline>input[type="radio"]:checked').attr('data-template-type');
    //        var _templateType = $('#ddlTemplateType :selected').val();
    //        // Create FormData object
    //        var fileData = new FormData();
    //        let entity = $(app.DE.selEntity).val();
    //        //read excel files
    //        var ExcelFile = $(app.DE.updExcel).get(0);
    //        var excel = ExcelFile.files;
    //        if (_templateType == "EXL" && !app.ValidateExcelFile(excel)) return;
    //        for (var i = 0; i < excel.length; i++) {
    //            fileData.append(excel[i].name, excel[i]);
    //        }

    //        // read kml files if geom type is Line or polygon
    //        var geomType = $(app.DE.option_selected, $(app.DE.selEntity)).attr('label');
    //        //if (geomType == "Line") {
    //        if (_templateType == "KML" || _templateType == "KMZ") {
    //            var KMLFile = $(app.DE.updKML).get(0);
    //            var kml = KMLFile.files;
    //            if (!app.ValidateKMLFile(kml)) return;
    //            for (var i = 0; i < kml.length; i++) {
    //                fileData.append(kml[i].name, kml[i]);
    //            }
    //        }
    //        if (_templateType == "SHP") {
    //            var SHPFile = $(app.DE.updSHP).get(0);
    //            var SHP = SHPFile.files;
    //            if (!app.ValidateSHPFile(SHP)) return;
    //            for (var i = 0; i < SHP.length; i++) {
    //                fileData.append(SHP[i].name, SHP[i]);
    //            }
    //        }
    //        if (_templateType == "DXF") {
    //            //if (!validate('demo')) { e.preventDefault(); return false; }
    //            var SourceId = $('table .inputpicker-active').attr('data-value');
    //            fileData.append('SourceId', SourceId);
    //            var DxfFile = $(app.DE.updDxf).get(0);
    //            var Dxf = DxfFile.files;
    //            if (!app.ValidateDxfFile(Dxf)) return;
    //            for (var i = 0; i < Dxf.length; i++) {
    //                fileData.append(Dxf[i].name, Dxf[i]);
    //            }

    //        }
    //        if (_templateType == "TAB") {
    //            var TABFile = $(app.DE.updTab).get(0);
    //            var Tab = TABFile.files;
    //            if (!app.ValidateTabFile(Tab)) return;
    //            for (var i = 0; i < Tab.length; i++) {
    //                fileData.append(Tab[i].name, Tab[i]);
    //            }

    //        }
    //        fileData.append('entity', entity);
    //        app.showHideControls(app.Actions.FILE_UPLOAD_START);
    //        ajaxReqforFileUpload('LandBaseDataUploader/UploadFiles', fileData, true, function (result) {
    //            let status = result.status;
    //            let message = result.status + " :" + (result.err_description || result.error_msg);
    //             
    //            switch (status) {

    //                case app.StatusMessages.OK:
    //                    app.uploadId = result.id;
    //                    app.showHideControls(app.Actions.FILE_UPLOADED_SUCCESS, null, result);
    //                    app.getMappingTemplate(0);
    //                    break;
    //                case app.StatusMessages.FAILED:
    //                    app.showHideControls(app.Actions.FILE_UPLOADED_FAILED, message);
    //                    break;
    //                case app.Actions.INVALID_INPUTS:
    //                    app.showHideControls(app.Actions.FILE_INVALID_INPUTS, message);
    //                    break;
    //                case app.Actions.INVALID_FILE:
    //                    app.showHideControls(app.Actions.FILE_INVALID_FILE, message);
    //                    break;
    //            }

    //        }, false);

    //    } else {
    //        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_057);//FormData is not supported.
    //    }
    //}
    //this.getMappingTemplate = function () {
    //    let _templateId = 0;
    //    if ($('#ddlColumTemplates').val() > 0)
    //    { _templateId = parseInt($('#ddlColumTemplates').val()); }
    //    let _layerName = $(app.DE.selEntity).val();
    //    ajaxReq('LandBaseDataUploader/getColumnMappping', { layerName: _layerName, templateId: _templateId }, true, function (resp) {
    //        $(app.DE.step2).html(resp);
    //    }, false, true);
    //}
    //this.setTemplateName = function () {
    //    var _isValid = true;
    //    $.each($('#step2 select[is-Required="1"]'), function (i, item) {
    //        if ($(item).val() == '') { _isValid = false; $(this).next('.chosen-container').addClass('notvalid'); }
    //    })
    //    if (_isValid) {
    //        popup.LoadModalDialog('CHILD', 'LandBaseDataUploader/templateName', null, 'Template Name', 'modal-sm');
    //    }
    //}
    //this.saveTemplate = function () {
    //    if ($('#txtTemplateName').val() == '') { $('#txtTemplateName').addClass('notvalid'); return false; }
    //    $('#hdnIsFinalMapping').val(false);
    //    $('#hdnTemplateName').val($('#txtTemplateName').val());
    //    var _isValid = true;
    //    $.each($('#ddlColumTemplates option'), function (i, item) {
    //        var _templateName = $(item).html();
    //        if (_templateName.toLowerCase() == $('#txtTemplateName').val().toLowerCase()) {
    //            _isValid = false;
    //            alert('Template name already exist!');
    //        }
    //    })
    //    if (_isValid) { app.resetSequence(); $('#frmMappColumns').submit(); }
    //}
    //this.successMapping = function (resp) {
    //    if ($('#hdnIsFinalMapping').val().toLowerCase() == 'true') {
    //        if (resp.status == app.Actions.INVALID_INPUTS) {
    //            let message = resp.status + " :" + (resp.err_description || resp.error_msg);
    //            app.showHideControls(app.Actions.FILE_INVALID_INPUTS, message);
    //        }
    //        else { app.ValidateData(); }
    //    } else {
    //        if (resp.objPM.status == 'OK') {
    //            alert(resp.objPM.message);
    //            $('#closeChildPopup').trigger("click");
    //            var option = '<option value=' + resp.id + '>' + resp.template_name + '</option>';
    //            $('#ddlColumTemplates').append(option).trigger('chosen:updated');
    //        }
    //    }
    //}

    //this.ChangeColumnMapping = function () {
    //     
    //    $('#hdnUploadId').val(app.uploadId);
    //    $('#hdnIsFinalMapping').val(true);
    //    var _isValid = true;
    //    $.each($('#step2 select[is-Required="1"]'), function (i, item) {
    //        if ($(item).val() == '') { _isValid = false; $(this).next('.chosen-container').addClass('notvalid'); }
    //    })
    //    if (_isValid) {
    //        app.resetSequence();
    //        $('#frmMappColumns').submit();
    //    }
    //}
    //this.getColumnsMapping = function () {
    //    var mappings = [];
    //    $.each($('#step2 select'), function (i, item) {
    //        if ($(item).val() != '') {
    //            let _templateColumnName = $(item).attr('template-column-name');
    //            let _templateDbColumnname = $(item).attr('template-db-column-name');
    //            let _importedColumnName = $(item).val();
    //            mappings.push({
    //                template_db_column_name: _templateDbColumnname,
    //                template_column_name: _templateColumnName,
    //                imported_column_name: _importedColumnName
    //            })
    //        }
    //    })
    //    return mappings;
    //}
    //this.resetSequence = function () {
    //    $.each($('#frmMappColumns select'), function (i, item) {
    //        if ($(this).attr('name') != undefined)
    //            $(this).attr('name', $(this).attr('name').replace(/\[\d+\]/, "[" + (i) + "]"));
    //        if ($(this).attr('id') != undefined)
    //            $(this).attr('id', $(this).attr('id').replace(/\_\d+\_/, "_" + (i) + "_"));

    //    })
    //}
    //this.checkLogFileExist = function (id, status) {
    //    ajaxReq('FileDownload/checkLogFileExist', { id: id, status: status }, false, function (resp) {
    //        if (resp.status == 'FAILED') {
    //            alert(resp.message);
    //        } else { window.location = applicationUrl + '/FileDownload/DownloadUploadLogs?id=' + id + '&status=' + status; }
    //    }, true, false);
    //}
    //---------LAND BASE LAYERS------//
}

