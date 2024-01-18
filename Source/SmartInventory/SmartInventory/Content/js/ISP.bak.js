var ISPMain = function () {
    var app = this;
    app.defaultEntity = '27';
    app.layerDetails = null;
    app.currentTarget = null;
    app.HTML = '<div class="rel_pop animated rotateIn" id="divmduInfoWindow" style="position: fixed;">';
    app.HTML += '<span class="fa fa-caret-left pointer"></span>';
    app.HTML += '<span class="fa fa-times times-cross closeRightPop"></span>';
    app.HTML += '<div class="row">';
    app.HTML += '<div class="col-md-12">';
    app.HTML += '<div class="span_box_menu dvSplitElement" style="display:none;padding-left:10px;" elementid="" data-networkid="" elementtype="" style="padding-left:10px;">Split</div>'; //sapna
    app.HTML += '<div class="span_box_menu dvViewElement" elementid="" elementtype="" style="padding-left:10px;">Update</div>';
    app.HTML += '<div class="span_box_menu dvDeleteElement" elementid="" elementtype="" style="padding-left:10px;">Delete</div>';
    app.HTML += '<div class="span_box_menu dvAddCable" elementid="" data-shaftid="" data-floorid="" data-networkid="" elementtype="" style="padding-left:10px;">Add TP</div>';
    app.HTML += '<div class="span_box_menu dvEditgeom" style="display:none;padding-left:10px;" elementid="" data-shaftid="" data-floorid="" data-networkid="" elementtype="" >Edit Location</div>';
    app.HTML += '<div class="span_box_menu dvSavegeom" style="display:none;padding-left:10px;" elementid="" data-shaftid="" data-floorid="" data-networkid="" elementtype="" >Save Location</div>';
    app.HTML += '</div>';
    app.HTML += '</div>';
    app.HTML += '</div>';
    this.lstISPCablePt = [];
    var mainCables = [];
    this.ParentModel = 'PARENT';
    this.ChildModel = 'CHILD';

    this.prevordX = null;
    this.prevordY = null;

    this.lineLatLong = [];
    this.Line = '';
    this.lineGeom = '';
    this.editedCableGeom = '';
    this.isEditMode = false;
    this.editCableId = '';
    this.prevScroll = 0;
    this.allCables = [];
    this.nearByentityDistanceBuffer = 20;
    this.cableNearPoint = [];
    this.entityNearPoint = [];
    this.sourceCableId = null;
    this.destinationCableId = null;
    this.SplicerEntity = [];
    this.refreshlineLatLong = [];
    this.EndPointSnappedCables = [];
    this.DE = {
        "ElementTemplate": "#ElementList .Element",
        "Elements": "#divISPView .Element,#divISPView .infotool",
        "ddlLevel": "#ddlLevel",
        "ddlShaft": "#ddlShaft",
        "StructureId": "#StructureId",
        "ElementType": "#ElementType :selected",
        "chkShowLibrary": "#chkShowLibrary",
        "divLibrary": "#divLibrary",
        "divISPView": "#divISPView",
        "divShaft": "#divShaft",
        'divFloor': '#divFloor',
        "BaseLocation": "#ddlBaseLocation",
        "divBaseLocation": "#divBaseLocation",
        "DeleteElement": "#divISPView .dvDeleteElement",
        "ViewElement": "#divISPView .dvViewElement",
        "SplitterParent": "#ddlSplitterParent",
        "divSplitterParent": "#divSplitterParent",
        "AddCableEvent": "#divISPView .dvAddCable",
        "tblISPTerminationPt": "#tblISPTerminationPoint",
        "dvElementType": "#dvElementType",
        "ItemTemplate": ".clsTemplateIcon",
        "dvISPNoTerminatePoint": "#dvISPNoTerminatePoint",
        "LayerAccordin": ".layers h2 .lyracrdn",
        "OnLyrs": 'input[type="checkbox"]',
        "CustomerParent": "#ddlCustomerParent",
        "divCustomerParent": "#divCustomerParent",
        "SplitElement": "#divISPView .dvSplitElement",
        "SplicingDiv": "#SplicingDiv",
        "splicingMain": "#SplicingDiv .splicingMain",
        "SourceCable": "#ddlSourceCable",
        "DestinationCable": "#ddlDestinationCable",
        "ConnectingEntity": "#ddlConnectingEntity",
        "startReading": "#start_reading",
        "endReading": "#end_reading",
        "cableCalculatedLength": "#cable_calculated_length",
        "cableMeasuredLength": "#cable_measured_length",
        "FMSParent": "#ddlFMSParent",
        "divFMSParent": "#divFMSParent"
    }
    this.initApp = function () {
        $(app.DE.BaseLocation).val(2).trigger('change');
        $('#ElementType').val(app.defaultEntity);
        //$(app.DE.divFloor).show();
        //$(document).bind("contextmenu", function (e) { e.preventDefault(); });        
        if (parseInt($('#totalOSPCable').val()) > $('.StructureInfo svg[data-cable-type="OSP"]').length) {
            app.createOspCable();
        }
        this.bindEvents();
        //app.refreshCable();
    }
    this.bindEvents = function () {

        //$(app.DE.Elements).mousedown(function (event) {
        //    app.currentTarget = event;
        //    app.updateElement(event);
        //});
        $(document).on('click', app.DE.LayerAccordin, function (e) {

            $(this).toggleClass('fa-plus').toggleClass('fa-minus');
            $(this).closest('div.Layers').find('.mainlyr').slideToggle();
            $(this).closest('div.Layers').find('#dvNetworkActions').slideToggle();
        });
        $(app.DE.ElementTemplate).on('click', function (e) {
            app.setElement(e);
        });



        $(app.DE.DeleteElement).unbind('click').bind('click', function (e) { app.DeleteElement(e); });
        $(app.DE.AddCableEvent).unbind('click').bind('click', function (e) { app.currentTarget = e; app.AddCableEvent(e); });
        $(app.DE.ViewElement).bind('click', function (e) { app.currentTarget = e; app.updateElement(e); });
        $(app.DE.chkShowLibrary).change(function () {
            $(app.DE.divLibrary).toggle("slide");
            $(app.DE.divISPView).toggleClass("fullWidth", "slow");
            //fullWidth
            //if (this.checked) {
            //    $(app.DE.divISPView).animate({ 'width': 'calc(100% - 250px)' }, 'slow');
            //}
            //else {
            //    $(app.DE.divISPView).animate({ 'width': '100%' }, 'slow');
            //}
        });

        $(app.DE.BaseLocation).change(function () {
            $(app.DE.ddlShaft).val('').trigger("chosen:updated");
            $(app.DE.ddlLevel).val('').trigger("chosen:updated");
            if ($(this).val() == 1)
            { $(app.DE.divShaft + ',' + app.DE.divFloor).show(); } else { $(app.DE.divShaft).hide(); $(app.DE.divFloor).show(); }
        });

        $(document).on("click", "#btnCreateISPCable", function () {
            if (app.lstISPCablePt.length < 2) {
                app.showMessageOverlay();
                alert('Please select  2 termination point!');
                return false;
            }

            var strucIdVal = $("#StructureId").val();
            var lyrDetail = getLayerDetail("Cable");
            var ntkIdType = lyrDetail['network_id_type'];



            var _data = {
                geom: '0', enType: 'Cable', cableType: 'ISP', lstTP: app.lstISPCablePt, networkIdType: ntkIdType,
                ModelInfo: { structureid: strucIdVal }
            };
            app.addCable(_data);

        });
        $(document).on("click", "#btnEditISPCableTemplate", function () {
            popup.LoadModalDialog(app.ParentModel, 'ItemTemplate/CableTemplate', { cblType: 'ISP' }, 'ISP Cable Template', 'modal-lg');

        });

        //$(document).on("click", app.DE.AddCableEvent, function () {
        //    alert(1);
        //});
        $(document).on('click', app.DE.ItemTemplate, function (e) {
            var dataURL = $(this).attr('data-href');
            var layerName = $(this).attr('data-lyrname');
            var layerTitle = $(this).attr('data-title');
            var titleText = layerTitle + ' Template';
            if (dataURL !== '' && dataURL != null) {
                //if (layerName == "Cable") {
                //    layerName = $(this).attr('data-cabletype');
                //    popup.LoadModalDialog(app.ParentModel, app.ParentModel, dataURL, { cblType: layerName }, layerName + ' Template', 'modal-lg');
                //}
                //else {
                popup.LoadModalDialog(app.ParentModel, dataURL, { eType: layerName }, titleText, 'modal-lg');
                //}
                //e.stopPropagation();
            } else { app.showMessageOverlay(); alert('Redirect url not found!') }
        });

        $(document).on('change', app.DE.OnLyrs, function (e) {
            var mapAbr = $(this).attr('data-mapabbr');
            if (this.checked) {
                app.OnLayer(mapAbr);
            } else { app.OffLayer(mapAbr); }
        });
        $(document).on('change', '#checkAll', function (e) {
            if (this.checked) {
                app.OnAllLayer();
            } else { app.OffAllLayer(); }
        });
        $(document).on('change', app.DE.SplitterParent, function (e) {
            $('#hdnSelectedParent').val($(this).val());
        });
        document.addEventListener('contextmenu', event => event.preventDefault());
        $(app.DE.SplicingDiv).draggable({ scroll: false, handle: "#splicingHeader", containment: "window" });
    }
    this.saveElementInfo = function (event, layerDetails, templateId) {

        app.currentTarget = event;
        if (layerDetails != null) {
            var elementType = $(app.DE.ElementType).attr('data-layer-name').toUpperCase().replace(/ /g, "");
            if (elementType == "UNIT") {
                //var systemId=$(app.DE.ddlLevel).val();
                //var unitCount=$('#FLOOR_'+systemId).attr('data-total-unit'); 
                //var existUnitCount=$('#FLOOR_'+systemId+' span.UNIT').length;
                //if(parseInt(existUnitCount)<parseInt(unitCount))
                //{
                app.addRoom(layerDetails, templateId);
                //}
                //else{
                //    app.showMessageOverlay();
                //    alert('Only '+unitCount+' units are allowed!');
                //}
            }
            if (elementType == "HTB") { app.addHTB(layerDetails, templateId); }
            if (elementType == "FDB") { app.addFDB(layerDetails, templateId); }
            if (elementType == "BDB") { app.addBDB(layerDetails, templateId); }
            if (elementType == "SPLITTER") { app.addSplitter(layerDetails, templateId); }
            if (elementType == "SPLICECLOSURE") { app.addSC(layerDetails, templateId); }
            if (elementType == "ONT") { app.addONT(layerDetails, templateId); }
            if (elementType == "CUSTOMER") { app.addCustomer(layerDetails, templateId); }
            if (elementType == "FMS") { app.addFMS(layerDetails, templateId); }

        }
    }
    this.updateElement = function (e) {

        var elementType = $(e.currentTarget).attr('elementtype').toUpperCase().replace(/ /g, "");
        elementType = elementType.toUpperCase() == 'ISP' ? 'CABLE' : elementType.toUpperCase();
        if (elementType == 'UNIT') { app.updateRoom(e); }
        else if (elementType == 'HTB') { app.updateHTB(e); }
        else if (elementType == 'FDB') { app.updateFDB(e) }
        else if (elementType == 'BDB') { app.updateBDB(e) }
        else if (elementType == 'SPLITTER') { app.updateSplitter(e) }
        else if (elementType == 'SPLICECLOSURE') { app.updateSC(e) }
        else if (elementType == 'ONT') { app.updateONT(e) }
        else if (elementType == 'CUSTOMER') { app.updateCustomer(e) }
        else if (elementType == 'POD') { app.updatePOD(e) }
        else if (elementType == 'MPOD') { app.updateMPOD(e) }
        else if (elementType == 'CABLE') { app.updateCable(e) }
        else if (elementType == 'FMS') { app.updateFMS(e) }
    }
    this.SplitCable = function (e) {


        var elementType = $(e.currentTarget).attr('elementtype').toUpperCase().replace(/ /g, "");
        elementType = elementType.toUpperCase() == 'ISP' ? 'CABLE' : elementType.toUpperCase();
        app.splitCaple(e)

    }
    this.DeleteElement = function (e) {
        app.showMessageOverlay();
        var elementId = $(e.currentTarget).attr('elementid');
        var elementType = $(e.currentTarget).attr('elementtype').replace(/ /g, "");
        elementType = elementType.toUpperCase() == 'ISP' ? 'Cable' : elementType.toUpperCase();

        var layerTitle = getLayerTltle(elementType);
        if (elementType.toUpperCase() == 'ONT') { app.deleteONT(elementId); return; }
        else if (elementType.toUpperCase() == 'BDB') { app.deleteBDB(elementId); return; }

        confirm('Are you sure,do you want to delete this ' + layerTitle + '?', function () {
            app.showMessageOverlay();
            if (elementType.toUpperCase() == 'UNIT') { app.deleteRoom(elementId); }
            else if (elementType.toUpperCase() == 'HTB') { app.deleteHTB(elementId); }
            else if (elementType.toUpperCase() == 'FDB') { app.deleteFDB(elementId); }

            else if (elementType.toUpperCase() == 'SPLITTER') { app.deleteSplitter(elementId); }
            else if (elementType.toUpperCase() == 'SPLICECLOSURE') { app.deleteSC(elementId); }

            else if (elementType.toUpperCase() == 'CUSTOMER') { app.deleteCustomer(elementId); }
            else if (elementType.toUpperCase() == 'POD') { app.deletePOD(elementId); }
            else if (elementType.toUpperCase() == 'MPOD') { app.deleteMPOD(elementId); }
            else if (elementType.toUpperCase() == 'CABLE') { app.deleteCable(elementId); }
            else if (elementType.toUpperCase() == 'ADB') { app.deleteADB(elementId); }
            else if (elementType.toUpperCase() == 'CDB') { app.deleteCDB(elementId); }
            else if (elementType.toUpperCase() == 'FMS') { app.deleteFMS(elementId); } $('.rel_pop').remove();

        })

    }
    this.AddCableEvent = function (e) {
        if (app.isEditMode == true) {
            app.clearDragableObject();
            app.isEditMode = false;
            app.editCableId = '';
        }

        //var elem = $(e.currentTarget).parents(4).children('span')[2];
        //var ntkId = $(elem).attr("data-networkid");
        //var sysId = $(elem).attr("data-systemid");
        //var enType = $(elem).attr("data-entitytype");
        //var enShaftId = $(elem).attr("data-shaftid");

        var ntkId = $(e.currentTarget).attr("data-networkid");
        var sysId = $(e.currentTarget).attr("elementid");
        var enType = $(e.currentTarget).attr("elementtype");
        var enShaftId = $(e.currentTarget).attr("data-shaftid");
        var enFloorId = $(e.currentTarget).attr("data-floorid");
        if (!(enType != "" && (enType.toUpperCase() == 'BDB' || enType.toUpperCase() == 'FDB' || enType.toUpperCase() == 'CUSTOMER' || enType.toUpperCase() == 'HTB' || enType.toUpperCase() == 'SPLITTER' || enType.toUpperCase() == 'SPLICECLOSURE' || enType.toUpperCase() == 'ONT' || enType.toUpperCase() == 'FMS'))) {
            app.showMessageOverlay();
            alert(enType + ' can not be the termination point for cable.');
            return false;
        }


        var chkflag = false;
        var message = 'This termination point already added!';
        if (app.lstISPCablePt.length < 2) {
            $.map(app.lstISPCablePt, function (value, index) {
                if (value.network_name == enType && value.network_id == ntkId && value.system_id == sysId) {
                    chkflag = true;
                }
                if ((value.shaftId == 0 && enShaftId == 0) && value.floorId != enFloorId && (value.floorId != 0 && enFloorId != 0)) { chkflag = true; message = 'Floor to Floor Connectivity is not available!'; }
                // else if (value.shaftId != enShaftId && (value.shaftId != 0 && enShaftId != 0)) { chkflag = true; message = 'Shaft to Shaft Connectivity is not available!'; }

            });
            if (!chkflag) {
                app.lstISPCablePt.push(
                                           {
                                               network_name: enType, network_id: ntkId, system_id: sysId, shaftId: enShaftId, floorId: enFloorId, x: e.pageX, y: e.pageY
                                           });
                $(app.DE.tblISPTerminationPt + ' tbody').append('<tr class="child"><td>' + ntkId + '</td><td style="font-weight: bold;color: red;    cursor: pointer;" class="remove" onclick="isp.deleteISPTerminationPointRow(this);" >X</td></tr>');
                $('#dvISPNoTerminatePoint').hide();
            } else { app.showMessageOverlay(); alert(message); }

            // if two points selected..
            if (app.lstISPCablePt.length == 1) {
                //app.createCableSVG(app.lstISPCablePt[0].x, app.lstISPCablePt[0].y);
                var path = '';//'M ' + startX + ' ' + startY + ' L ' + startX + ' ' + startY + 'M ' + startX + ' ' + startY + ' L ' + endX + ' ' + endY;
                //app.prevordX=(app.lstISPCablePt[0].x -($('#svgContainer').offset().left+80));
                //app.prevordY= (app.lstISPCablePt[0].y - ($('#svgContainer').offset().top+80));



                app.prevordX = ($('#' + enType.toUpperCase() + '_' + sysId).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                app.prevordY = ($('#' + enType.toUpperCase() + '_' + sysId).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
                //GET MARKER KEY
                var mKey = getMarkerKey();
                var color = app.getColor();
                addMarkerToLineArray(app.prevordX, app.prevordY, app.prevordX, app.prevordY, false, mKey);
                var svgHtml = app.parseSVG('<svg style="position: absolute;z-index: 99;max-width: 1150px;width:1150px;height:1095px;max-height:1095px;"   pointer-events="none"  version="1.1" xmlns="http://www.w3.org/1999/xhtml" > <path d="' + path + '"  pointer-events="visibleStroke" version="1.1" xmlns="http://www.w3.org/1999/xhtml"  fill="none" stroke="' + color + '" stroke-width="3"></path> </svg>');
                //GET SVG FOR MARKER AND APPEND
                //$('#divMarker').append(app.parseSVG(getMarkerSVG(mKey, app.prevordX,app.prevordY, false)));
                $('#svgContainer').append(svgHtml);
                app.setSVGWIdthHeight();
            }
            if (app.lstISPCablePt.length == 2) {
                var x = ($('#' + enType.toUpperCase() + '_' + sysId).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                var y = ($('#' + enType.toUpperCase() + '_' + sysId).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));

                if (app.lstISPCablePt[0].shaftId == enShaftId) {

                    var cableCount = $('#' + app.lstISPCablePt[0].network_name + '_' + app.lstISPCablePt[0].system_id).attr('data-cable-count-v');
                    app.lineLatLong[0].moveTo = 'M ' + (app.prevordX + cableCount * 4) + ' ' + app.prevordY;
                    app.lineLatLong[0].lineTo = 'L ' + (app.prevordX + cableCount * 4) + ' ' + app.prevordY;
                    x = x + cableCount * 4;
                    app.prevordX = (app.prevordX + cableCount * 4)




                } else {
                    var cableCount = $('#' + app.lstISPCablePt[0].network_name + '_' + app.lstISPCablePt[0].system_id).attr('data-cable-count-h');
                    app.lineLatLong[0].moveTo = 'M ' + (app.prevordX) + ' ' + (app.prevordY + cableCount * 4);
                    app.lineLatLong[0].lineTo = 'L ' + (app.prevordX) + ' ' + (app.prevordY + cableCount * 4);
                    y = y + cableCount * 4;
                    app.prevordY = (app.prevordY + cableCount * 4)
                }

                //GET MARKER KEY
                var mKey = getMarkerKey();
                //PUSH MARKER POSITION INTO GLOBAL ARRAY..
                if (app.prevordX != null && app.prevordY != null) {
                    var distanceX = app.prevordX - x;
                    var distanceY = app.prevordY - y;
                    var distance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
                    if (distance < 20) return false;
                    addMarkerToLineArray(app.prevordX, app.prevordY, x, y, false, mKey);

                    //GET SVG FOR MARKER AND APPEND
                    //$('#divMarker').append(app.parseSVG(getMarkerSVG(mKey, x, y, false)));

                    // CREATE VIRTUAL MARKER BETWEEN NEW CLICKED POINT AND LAST POINT..
                    var objLeftVirtualPoint = createleftVMarker(app.lineLatLong.length - 1, x, y);

                    if (objLeftVirtualPoint != null) {
                        app.lineLatLong.splice(app.lineLatLong.length - 1, 0, objLeftVirtualPoint);
                    }

                    // REDRAWA LINE USING GLOBAL LINE ARRAY..
                    RedrawLine();

                    //UPDATE PREVCLICK VALUE..
                    app.prevordX = x;
                    app.prevordY = y;
                    //MAKE MARKER DRAGABLE..
                    makeDraggable('#divMarker')

                }

                //$(window).unbind("click");
                //$(window).bind("click", function (e) {
                //    app.drawline(e);
                //});

                $('#svgVMarker' + app.lineLatLong[0].markerKey).remove();
                $('#svgVMarker' + app.lineLatLong[app.lineLatLong.length - 1].markerKey).remove();
            }
        }
        else {
            app.showMessageOverlay(); alert('Only 2 termination points can be selected!');
        }
        $('.rel_pop').remove();
    }
    this.drawline = function (e) {
        var x = (e.pageX - 360);
        var y = (e.pageY - 190);
        //GET MARKER KEY
        var mKey = getMarkerKey();
        //PUSH MARKER POSITION INTO GLOBAL ARRAY..
        if (app.prevordX != null && app.prevordY != null) {
            var distanceX = app.prevordX - x;
            var distanceY = app.prevordY - y;
            var distance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
            if (distance < 20) return false;
            addMarkerToLineArray(app.prevordX, app.prevordY, x, y, false, mKey);



        } else {
            addMarkerToLineArray(x, y, x, y, false, mKey);
        }
        //GET SVG FOR MARKER AND APPEND
        $('#divMarker').append(app.parseSVG(getMarkerSVG(mKey, x, y, false)));

        // CREATE VIRTUAL MARKER BETWEEN NEW CLICKED POINT AND LAST POINT..
        var objLeftVirtualPoint = createleftVMarker(app.lineLatLong.length - 1, x, y);

        if (objLeftVirtualPoint != null) {
            app.lineLatLong.splice(app.lineLatLong.length - 1, 0, objLeftVirtualPoint);
        }

        // REDRAWA LINE USING GLOBAL LINE ARRAY..
        RedrawLine();

        //UPDATE PREVCLICK VALUE..
        app.prevordX = x;
        app.prevordY = y;
        //MAKE MARKER DRAGABLE..
        makeDraggable('#divMarker')
    }
    this.createCableSVG = function (startX, startY) {
        //startX = startX - 416;
        //endX = endX - 416;
        //startY = startY - 230;
        //endY = endY - 230;
        var path = '';//'M ' + startX + ' ' + startY + ' L ' + startX + ' ' + startY + 'M ' + startX + ' ' + startY + ' L ' + endX + ' ' + endY;
        app.prevordX = startX - 416;
        app.prevordY = startY - 230;
        //GET MARKER KEY
        var mKey = getMarkerKey();
        addMarkerToLineArray(app.prevordX, app.prevordY, app.prevordX, app.prevordY, false, mKey);
        var svgHtml = app.parseSVG('<svg style="position: absolute;z-index: 99;"   pointer-events="none"  version="1.1" xmlns="http://www.w3.org/1999/xhtml" > <path d="' + path + '" transform="translate(1.5,38.5)" pointer-events="visibleStroke" version="1.1" xmlns="http://www.w3.org/1999/xhtml"  fill="none" stroke="#FF8800" stroke-width="3"></path> </svg>');
        //GET SVG FOR MARKER AND APPEND
        $('#divMarker').append(app.parseSVG(getMarkerSVG(mKey, app.prevordX, app.prevordY, false)));
        $('#svgContainer').append(svgHtml);
        //$('#svgContainer svg').off('click')
        //$('#svgContainer :last-child svg').on("click", function(){
        //    console.log('1');
        //})
        $(window).unbind("click");
        $(window).bind("click", function () {

            var x = (event.pageX - 416);
            var y = (event.pageY - 230);
            //GET MARKER KEY
            var mKey = getMarkerKey();
            //PUSH MARKER POSITION INTO GLOBAL ARRAY..
            if (app.prevordX != null && app.prevordY != null) {
                var distanceX = app.prevordX - x;
                var distanceY = app.prevordY - y;
                var distance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
                if (distance < 20) return false;
                addMarkerToLineArray(app.prevordX, app.prevordY, x, y, false, mKey);



            } else {
                addMarkerToLineArray(x, y, x, y, false, mKey);
            }
            //GET SVG FOR MARKER AND APPEND
            $('#divMarker').append(app.parseSVG(getMarkerSVG(mKey, x, y, false)));

            // CREATE VIRTUAL MARKER BETWEEN NEW CLICKED POINT AND LAST POINT..
            var objLeftVirtualPoint = createleftVMarker(app.lineLatLong.length - 1, x, y);

            if (objLeftVirtualPoint != null) {
                app.lineLatLong.splice(app.lineLatLong.length - 1, 0, objLeftVirtualPoint);
            }

            // REDRAWA LINE USING GLOBAL LINE ARRAY..
            RedrawLine();

            //UPDATE PREVCLICK VALUE..
            app.prevordX = x;
            app.prevordY = y;
            //MAKE MARKER DRAGABLE..
            makeDraggable('#divMarker')
        });
    }

    this.parseSVG = function (s) {
        var div = document.createElementNS('http://www.w3.org/1999/xhtml', 'div');
        div.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg">' + s + '</svg>';
        var frag = document.createDocumentFragment();
        while (div.firstChild.firstChild)
            frag.appendChild(div.firstChild.firstChild);
        return frag;
    }
    this.deleteISPTerminationPointRow = function (selectedRow) {

        var p_network_id = $(selectedRow).closest("td").parent().children('td')[0];
        p_network_id = $(p_network_id).text();
        var removeIndex;
        var chkRemove = false;
        var lstTP = app.lstISPCablePt;
        $.map(lstTP, function (value, index) {

            if (value.network_id == p_network_id) {
                removeIndex = index;
                chkRemove = true;
            }
            lstTP = app.lstISPCablePt;
        });
        if (removeIndex >= 0 && chkRemove)
            app.lstISPCablePt.splice(removeIndex, 1);

        $(selectedRow).closest("tr").remove();

        if ($(app.DE.tblISPTerminationPt + ' tbody tr').length == 0) {
            $("#dvISPNoTerminatePoint").show();
            $('#svgContainer svg').last().remove();
            app.clearDragableObject();
        }

    }
    this.getElementTemplate = function () {

        $('.dvAddCable').hide();
        $('#dvElementList').html('');
        var selectionType = $(app.DE.ElementType).attr('data-layer-name');
        $(app.DE.divSplitterParent).hide();
        $(app.DE.divCustomerParent).hide();
        $(app.DE.divFMSParent).hide();
        $(app.DE.divBaseLocation).show();
        if (selectionType == "Cable") {

            var childElement = $(app.DE.ElementType).attr('data-layer-child');
            // $(app.DE.SplitterParent).val('').trigger("chosen:updated");
            //if (childElement == '1') {
            $(app.DE.BaseLocation).val('0').trigger("chosen:updated");
            $(app.DE.ddlShaft).val('').trigger("chosen:updated");
            $(app.DE.ddlLevel).val('').trigger("chosen:updated");
            // $(app.DE.divSplitterParent).show();
            $(app.DE.divShaft).hide();
            $(app.DE.divFloor).hide();
            // }            
            $(app.DE.divShaft).hide();
            $(app.DE.divFloor).hide();
            app.lstISPCablePt = [];
            $(".dvAddCable").show();
            $(app.DE.tblISPTerminationPt + ' tbody tr').html('');
            $("#dvIspCable,#tblISPTerminationPoint").show();
            $("#ParentElement").parent().hide();
        } else if (selectionType.toLowerCase() == "customer") {
            $(app.DE.SplitterParent).val('').trigger("chosen:updated");
            $(app.DE.FMSParent).val('').trigger("chosen:updated");
            var childElement = $(app.DE.ElementType).attr('data-layer-child');
            if (childElement == '1') {
                $(app.DE.BaseLocation).val('0').trigger("chosen:updated");
                $(app.DE.ddlShaft).val('').trigger("chosen:updated");
                $(app.DE.ddlLevel).val('').trigger("chosen:updated");
                $(app.DE.divCustomerParent).show();
                $(app.DE.divShaft).hide();
                $(app.DE.divFloor).hide();
            } else if ($(app.DE.BaseLocation).val() == 1) { $(app.DE.divShaft).show(); $(app.DE.divFloor).show(); }
            else if ($(app.DE.BaseLocation).val() == 2) { $(app.DE.divShaft).hide(); $(app.DE.divFloor).show(); }
            $("#dvIspCable,#tblISPTerminationPoint").hide();
            var customerHTML = '<label class="control-label">Templates:</label><div id="ParentElement"><div id="l_e_A3" class="e_A"><div id="ElementList" style="float:left;"><div id="ParentElement"><div data-ref="RO" class="e_B_rooms Element infotool" title="Customer" >';
            customerHTML += '<span class="CUSTOMER rightInfo"></span>';
            customerHTML += '<span class="mls roomlabel">Customer</span>'
            customerHTML += '</div></div></div></div></div>';
            $('#dvElementList').html(customerHTML);
            //var baseLocation = $(app.DE.ElementType).attr('data-layer-base-location');
            $(app.DE.ElementTemplate).on('click', function (e) {

                app.setElement(e);
            });
        }
        else {

            $("#ParentElement").parent().show();
            $("#dvIspCable,#tblISPTerminationPoint").hide();
            $(app.DE.SplitterParent).val('').trigger("chosen:updated");
            $(app.DE.CustomerParent).val('').trigger("chosen:updated");
            $(app.DE.FMSParent).val('').trigger("chosen:updated");
            var childElement = $(app.DE.ElementType).attr('data-layer-child');
            if (childElement == '1') {
                //$(app.DE.divBaseLocation).hide();
                $(app.DE.BaseLocation).val('0').trigger("chosen:updated");
                $(app.DE.ddlShaft).val('').trigger("chosen:updated");
                $(app.DE.ddlLevel).val('').trigger("chosen:updated");
                if (selectionType.toLowerCase() == "fms")
                { $(app.DE.divFMSParent).show(); }
                else { $(app.DE.divSplitterParent).show(); }

                $(app.DE.divShaft).hide();
                $(app.DE.divFloor).hide();
            }
            else if ($(app.DE.BaseLocation).val() == 1) { $(app.DE.divShaft).show(); $(app.DE.divFloor).show(); }
            else if ($(app.DE.BaseLocation).val() == 2) { $(app.DE.divShaft).hide(); $(app.DE.divFloor).show(); }
            $('#dvElementList').load(appRoot + 'ISP/getElementTemplate', { ElementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase().replace(/ /g, "") }, function (response, status, xhr) {
                try {
                    if (status == "error") {
                        $('#ParentElement').html("<h6 style='color:red'>Error: " + xhr.status + " " + xhr.statusText + "</h6>");
                    }
                    $(app.DE.ElementTemplate).on('click', function (e) {
                        app.setElement(e);
                    });
                } catch (e) {

                }
            });
        }

    }
    this.SetDropDownSequence = function () {
        $(app.DE.ddlShaft).val('').trigger("chosen:updated");
        $(app.DE.ddlLevel).val('').trigger("chosen:updated");
        $(app.DE.SplitterParent).val('').trigger("chosen:updated");
        $(app.DE.divShaft).hide();
        $(app.DE.divFloor).hide();
        $(app.DE.divSplitterParent).hide();
        $(app.DE.dvElementType).hide();
        $(app.DE.divSplitterParent).hide();
        var baseLocation = $(app.DE.BaseLocation).val();
        if (baseLocation == 1) {
            $(app.DE.divShaft).show();
            $(app.DE.dvElementType).show();
            $(app.DE.divFloor).show();
            $("option[data-layer-base-location$='SHAFT']").show()
            $("option[data-layer-base-location$='FLOOR']").hide()

        }
        else if (baseLocation == 2) {
            $(app.DE.divFloor).show();
            $(app.DE.dvElementType).show();
            $("option[data-layer-base-location$='FLOOR']").show()
            $("option[data-layer-base-location$='SHAFT']").hide()
        }
        $("#ElementType").val('0').trigger("chosen:updated");
        setTimeout(function () {
            if ($("#ElementType").val() === '0')
            { $('#dvElementList').html(''); }
        }, 200)

    }
    this.bindElement = function (e, SystemId) {

        var selectedElementType = $(app.DE.ElementType).attr('data-layer-name').toUpperCase().replace(/ /g, "");;
        //var baseLocation = $(app.DE.ElementType).attr('data-layer-base-location');
        var floorId = $(app.DE.ddlLevel).val();
        var shaftId = $(app.DE.ddlShaft).val();
        var elementid = $(e.currentTarget).attr('elementid');
        var targetId = null;
        if (baseLocation.toUpperCase() == 'SHAFT' && $(app.DE.BaseLocation).val() == 1) {

            targetId = $('#SHAFT_' + shaftId + '_FLOOR_' + floorId);
            $(e.currentTarget).clone().appendTo($('#SHAFT_' + shaftId + '_FLOOR_' + floorId));
            targetId.children().last().attr('id', 'Parent_' + SystemId);
            targetId.children().last().attr('elementid', SystemId);
            targetId.children().last().children().attr('id', selectedElementType + '_' + SystemId);
        } else if (selectedElementType == 'SPLITTER') {
            var SpParentId = $(app.DE.SplitterParent).val();
            targetId = $('#Parent_' + SpParentId.split('_')[1]);
            //$(e.currentTarget).clone().appendTo(targetId).fadeIn(500);
            $(e.currentTarget).children().first().clone().appendTo(targetId).fadeIn(500);
            targetId.children().last().attr('id', 'SPLITTER_' + SystemId);
            targetId.children().last().attr('elementid', SystemId);
            targetId.children().last().children().attr('id', selectedElementType + '_' + SystemId);

        }
        else if (selectedElementType == 'SPLICECLOSURE') {
            var SPCParentId = $(app.DE.SplitterParent).val();
            targetId = $('#Parent_' + SPCParentId.split('_')[1]);
            $(e.currentTarget).children().first().clone().appendTo(targetId).fadeIn(500);
            targetId.children().last().attr('id', 'SPLICECLOSURE_' + SystemId);
            targetId.children().last().attr('elementid', SystemId);
            targetId.children().last().children().attr('id', selectedElementType + '_' + SystemId);

        } else if (selectedElementType == "CABLE") {
            app.lstISPCablePt = [];
            $(app.DE.tblISPTerminationPt + ' tbody tr').html('');
            $("#dvIspCable,#tblISPTerminationPoint,#dvISPNoTerminatePoint").show();
        }
        else {



            targetId = $('#FLOOR_' + floorId);
            $(e.currentTarget).clone().appendTo(targetId).fadeIn(500);
            targetId.children().last().attr('id', 'Parent_' + SystemId);
            targetId.children().last().attr('elementid', SystemId);
            targetId.children().last().children().attr('id', selectedElementType + '_' + SystemId);
        }


        $(app.DE.DeleteElement).unbind('click').bind('click', function (e) { app.DeleteElement(e); });
        $(app.DE.ViewElement).unbind('click').bind('click', function (e) {

            app.updateElement(e)
        });
        $(app.DE.AddCableEvent).unbind('click').bind('click', function (e) { app.AddCableEvent(e) });
        //targetId.children().last().children().last().find('.dvDeleteElement,.dvViewElement').attr('elementid', SystemId).attr('elementtype', $(app.DE.ElementType + ' :selected').text());
        targetId.children().last().children().last().find('.dvDeleteElement,.dvViewElement').attr('elementid', SystemId).attr('elementtype', $(app.DE.ElementType).attr('data-layer-name'));
        $('.closeRightPop').unbind('click').on("click", function () { $('.rel_pop').hide(); })
        app.showRightPop();
        targetId.children().last().children('.roomlabel').remove();
        app.RefreshShaftHeight();
        $('.dvViewElement,.rel_pop,.dvDeleteElement').remove();

    }
    this.setElement = function (e) {

        var templateId = $(e.currentTarget).attr('ElementId');
        var isTemplateFilled = $(e.currentTarget).attr('data-is-template-filled');
        ajaxReq('ISP/checkTemplateExist', { enType: $(app.DE.ElementType).attr('data-layer-name').replace(/ /g, "") }, false, function (data) {
            if (data.status == 'FAILED' && $(app.DE.ElementType).attr('data-layer-name').replace(/ /g, "").toUpperCase() != 'CUSTOMER') {
                app.showMessageOverlay();
                alert(data.message);
                return false;
            } else {
                //var baseLocation = $(app.DE.ElementType).attr('data-layer-base-location');
                var childElement = $(app.DE.ElementType).attr('data-layer-child');
                var shaft = $(app.DE.ddlShaft).val();
                var entityType = $(app.DE.SplitterParent).val();
                var baseLocation = $(app.DE.BaseLocation).val();
                var floorId = $(app.DE.ddlLevel).val();
                if ($(app.DE.divBaseLocation).css('display') == 'block' && (baseLocation == '' || baseLocation == null || baseLocation == '0') && childElement == '0') {
                    app.showMessageOverlay();
                    alert('Please select base location first!');
                    return false;
                }
                //if (baseLocation != "" && $(app.DE.BaseLocation).val() == 1 && baseLocation.toUpperCase() == 'SHAFT' && (shaft == '' || shaft == null)) {
                //    app.showMessageOverlay();
                //    alert('Please select shaft!');
                //    return false;
                //}
                if ($(app.DE.divShaft).css('display') == 'block' && (shaft == '' || shaft == null)) {
                    app.showMessageOverlay();
                    alert('Please select shaft!');
                    return false;
                } else if ($(app.DE.ElementType).css('display') == 'block' && ($(app.DE.ElementType).val() == '0' || $(app.DE.ElementType).val() == null)) {
                    app.showMessageOverlay();
                    alert('Please select element!');
                    return false;
                }
                else if ($(app.DE.divFloor).css('display') == 'block' && (floorId == '' || floorId == null)) {
                    app.showMessageOverlay();
                    alert('Please select floor!');
                    return false;
                }
                else if ($(app.DE.divSplitterParent).css('display') == 'block' && (entityType == '' || entityType == null)) {
                    app.showMessageOverlay();
                    alert('Please select parent entity!');
                    return false;
                } else if ($(app.DE.divCustomerParent).css('display') == 'block' && ($(app.DE.CustomerParent).val() == '' || $(app.DE.CustomerParent).val() == null)) {
                    app.showMessageOverlay();
                    alert('Please select parent entity!');
                    return false;
                } else if ($(app.DE.divFMSParent).css('display') == 'block' && ($(app.DE.FMSParent).val() == '' || $(app.DE.FMSParent).val() == null)) {
                    app.showMessageOverlay();
                    alert('Please select parent entity!');
                    return false;
                }
                else { app.getLayerDetails(e, $(app.DE.ElementType).attr('data-layer-name'), templateId, e); }
            }
        }, true, false);
        //if (isTemplateFilled == 'False')
        //{
        //    app.showMessageOverlay();
        //    alert('Template is not filled,Please fill the template first!');
        //    return false;
        //}


    }

    this.SuccessSave = function (response) {
        if (response.Status == 'OK' || response.Status == 'VALIDATION_FAILED')
        { $(popup.DE.closeModalPopup).trigger("click"); }
        app.showMessageOverlay();
        app.refreshStructureInfo();
        app.lstISPCablePt = [];
        $(app.DE.tblISPTerminationPt + ' tbody').html('');
        $(app.DE.dvISPNoTerminatePoint).show();
        if (response.Data != null && response.Data.entityType != null) {
            app.refreshNLayers(response.Data.entityType);
        } else { if (!($('.clsTemplateIcon').hasClass('greenTemplate'))) { $('.clsTemplateIcon').addClass('greenTemplate'); } }
        alert(response.Message)
    }
    this.RefreshShaftHeight = function () {
        $.each($('#dvFloors div[id^="FLOOR_"]'), function (indx, itm) {
            var floor_id = $(this).attr("id");
            var floorHeight = $(this).height();
            $("div[id$='_" + floor_id + "']").height(floorHeight);
        });
    }
    this.OnAllLayer = function () {
        $('path').css('visibility', 'visible'); $('._jsPlumb_endpoint').css('visibility', 'visible');
        $('.e_B_first').css('visibility', 'visible');
        $.each($(".network  input[type='checkbox']"), function (indx, itm) {
            $(this).prop('checked', true);
            var lyrName = $(this).attr('data-mapabbr');
            $('.ISPViewContainer .' + lyrName).css('visibility', 'visible');
            if (lyrName == 'BDB') {
                $('.ISPViewContainer .BDB,.ISPViewContainer .BDB2').css('visibility', 'visible');
            }
            if (lyrName == 'SPLITTER') {
                $('.ISPViewContainer .SPLITTER,.ISPViewContainer .SPLITTER2').css('visibility', 'visible');
            }
        });
    }
    this.OffAllLayer = function () {
        $('path').css('visibility', 'hidden'); $('._jsPlumb_endpoint').css('visibility', 'hidden');
        $('.e_B_first').css('visibility', 'hidden');
        $.each($(".network  input[type='checkbox']"), function (indx, itm) {
            $(this).prop('checked', false);
            var lyrName = $(this).attr('data-mapabbr');
            $('.ISPViewContainer .' + lyrName).css('visibility', 'hidden');
            if (lyrName == 'BDB') {
                $('.ISPViewContainer .BDB,.ISPViewContainer .BDB2').css('visibility', 'hidden');
            }
            if (lyrName == 'SPLITTER') {
                $('.ISPViewContainer .SPLITTER,.ISPViewContainer .SPLITTER2').css('visibility', 'hidden');
            }
        });
    }
    this.OnLayer = function (enType) {
        var checkedCount = $("input[type='checkbox']:not(:checked)");
        if (checkedCount.length == 1) { $('#checkAll').prop('checked', true); }
        if (enType == 'CABLE') { $('path').css('visibility', 'visible'); $('._jsPlumb_endpoint').css('visibility', 'visible'); }
        $('.ISPViewContainer .' + enType).css('visibility', 'visible');

        if (enType == 'BDB') {
            $('.ISPViewContainer .BDB').css('visibility', 'visible');
            $('.ISPViewContainer .BDB2').css('visibility', 'visible');
            $('.ISPViewContainer .BDB').parent().css('visibility', 'visible');
            $('.ISPViewContainer .BDB2').parent().css('visibility', 'visible');
        }
        if (enType == 'SPLITTER') {
            $('.ISPViewContainer .SPLITTER,.ISPViewContainer .SPLITTER2').css('visibility', 'visible');
        }

        if (enType == 'ONT') {
            $('.ISPViewContainer .' + enType).parent().css('visibility', 'visible');
        }
    }
    this.OffLayer = function (enType) {
        $('#checkAll').prop('checked', false);
        if (enType == 'CABLE') { $('path').css('visibility', 'hidden'); $('._jsPlumb_endpoint').css('visibility', 'hidden'); }
        if (enType == 'Splitter') { $('.SPLITTER2').css('visibility', 'hidden'); }

        $('.ISPViewContainer .' + enType).css('visibility', 'hidden');
        //Old Code
        if (enType == 'BDB') {
            $('.ISPViewContainer .BDB').css('visibility', 'hidden');
            $('.ISPViewContainer .BDB2').css('visibility', 'hidden');
            $('.ISPViewContainer .BDB').parent().css('visibility', 'hidden');
            $('.ISPViewContainer .BDB2').parent().css('visibility', 'hidden');
        }
        if (enType == 'SPLITTER') {
            $('.ISPViewContainer .SPLITTER,.ISPViewContainer .SPLITTER2').css('visibility', 'hidden');
        }

        if (enType == 'ONT') {
            $('.ISPViewContainer .' + enType).parent().css('visibility', 'hidden');
        }

    }
    this.getLayerDetails = function (event, elementType, templateId) {
        elementType = elementType.toUpperCase().replace(/ /g, "");
        if (elementType == "UNIT") {
            var systemId = $(app.DE.ddlLevel).val();
            var unitCount = $('#FLOOR_' + systemId).attr('data-total-unit');
            var existUnitCount = $('#FLOOR_' + systemId + ' span.UNIT').length;
            if (!(parseInt(existUnitCount) < parseInt(unitCount))) {
                app.showMessageOverlay();
                alert('Only ' + unitCount + ' units are allowed in this floor!');
                return false;
            }
            var totalUnits = $('#dvFloors  .UNIT').length;
            var maxAllowedUnits = $('#total_unit').val();
            //if(parseInt(totalUnits)==parseInt(maxAllowedUnits))
            //{
            //    alert('Only '+maxAllowedUnits+' units are allowed in this structure!');
            //    return false;
            //}
        }
        ajaxReq('ISP/getLayerDetails', { layerName: elementType.replace(/ /g, "") }, false, function (data) {
            app.saveElementInfo(event, data, templateId);
        }, true, false);
    }
    this.isUnitValid = function (systemId, obj) {
        var structureTotalUnits = parseInt($('#total_unit').val());
        var totalUsedUnits = 0;
        $.each($('#dvFloors .FloorBox'), function () {
            var floorId = $(this).attr('id');
            if (floorId != 'FLOOR_' + systemId) {
                totalUsedUnits = totalUsedUnits + parseInt($(this).attr('data-total-unit'));
            }
        })
        if (parseInt($(obj).val()) > (structureTotalUnits - totalUsedUnits)) {
            alert('Maximum ' + structureTotalUnits + ' units are allowed in this structure!');
            $(obj).val($('#FLOOR_' + systemId).attr('data-total-unit'));
            return false;
        }
    }
    this.updateFloorInfo = function (obj) {
        var floorId = $(obj).attr('floorId');
        var pageUrl = 'ISP/FloorInfo';
        var modalClass = 'modal-sm';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { floorId: floorId }, 'Floor Details', modalClass);
    }
    this.SuccessFloorInfo = function (response) {
        $(popup.DE.closeModalPopup).trigger("click");
        $('#ddlLevel option[value="' + response.Data.system_id + '"]').html(response.Data.floor_name);
        $('#ddlLevel').trigger("chosen:updated");
        app.refreshStructureInfo();
        alert(response.Message)
    }
    this.allowNumberOnly = function (event) {
        var regex = new RegExp("^[\.0-9/]$");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    }
    this.showRightPop = function () {
        $("#divISPView .rightInfo").unbind('click').on("click", function (e) {
             
            app.clearEditMode();

            var entityType = $(this).attr('id').split('_')[0];

            $('.rel_pop').remove();
            var objHTML = $.parseHTML(app.HTML);
            if (entityType == 'FDB') {
                $(objHTML).find('.dvSplitElement').attr("style", "padding-left:10px;")
            }
            $(objHTML).find('.dvDeleteElement,.dvViewElement,.dvAddCable,.dvSplitElement').attr('elementid', $(this).attr('id').split('_')[1]);
            $(objHTML).find('.dvDeleteElement,.dvViewElement,.dvAddCable,.dvSplitElement').attr('elementtype', $(this).attr('id').split('_')[0]);
            $(objHTML).find('.dvAddCable').attr('data-shaftid', $(this).attr('data-shaftid'));
            $(objHTML).find('.dvAddCable,.dvSplitElement').attr('data-networkid', $(this).attr('data-networkid'));
            $(objHTML).find('.dvAddCable').attr('data-floorid', $(this).attr('data-floorid'));
            $(this).parent().append(objHTML);
            $(isp.DE.DeleteElement).unbind('click').bind('click', function (e) { isp.DeleteElement(e); });
            $(isp.DE.ViewElement).bind('click', function (e) { isp.currentTarget = e; isp.updateElement(e); });
            //SplitElement
            $(isp.DE.SplitElement).bind('click', function (e) { isp.SplitCable(e); });
            $(isp.DE.AddCableEvent).bind('click', function (e) { isp.currentTarget = e; app.AddCableEvent(e); });
            $('.closeRightPop').unbind('click').on("click", function () { $('.rel_pop').hide(); })
            $('.rel_pop').hide();
            $(this).siblings('.rel_pop').show();
            if ($(app.DE.ElementType).attr('data-layer-name')!='' && $(app.DE.ElementType).attr('data-layer-name').toUpperCase() == 'CABLE') { $('.dvAddCable').show(); }
            $('.rel_pop').draggable({ scroll: false, containment: ".StructureInfo" });
            if (entityType == 'SPLITTER' || entityType == 'CUSTOMER' || entityType == 'SPLICECLOSURE') {
                $('.dvAddCable').hide();
            }
            $('.rel_pop').css({ 'top': ((e.pageY)), 'left': (e.pageX) });
        });

    }
    this.refreshParentElementddl = function () {

        var HTML = '<option value="">-- Select Parent --</option>';
        $.each($('span[data-entitytype="BDB"]'), function (indx, itm) {
            var ID = $(itm).attr('id');
            var netWorkId = $(itm).attr('data-networkid');
            HTML += '<option value=' + ID + '>' + netWorkId + '</option>';
        })
        $('#ddlSplitterParent').html(HTML).val($('#hdnSelectedParent').val()).trigger("chosen:updated");

        var HTML = '<option value="">-- Select ONT --</option>';
        $.each($('span[data-entitytype="ONT"]'), function (indx, itm) {
            var ID = $(itm).attr('id');
            var netWorkId = $(itm).attr('data-networkid');
            HTML += '<option value=' + ID + '>' + netWorkId + '</option>';
        })
        $('#ddlCustomerParent').html(HTML).val($('#hdnSelectedParent').val()).trigger("chosen:updated");
    }
    /*ROOM*/
    this.addRoom = function (layerDetails, templateId) {
        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq(layerDetails.Data.save_entity_url, { networkIdType: layerDetails.Data.network_id_type, templateId: templateId, objIspEntityMap: { floor_id: $(app.DE.ddlLevel).val(), structure_id: $(app.DE.StructureId).val() }, isDirectSave: true }, false, function (response) {
                $(popup.DE.closeModalPopup).trigger("click");
                app.showMessageOverlay();
                if (response.Status == 'OK') {
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else { alert(response.Message); }
            }, true, false);
        } else {
            var pageUrl = layerDetails.Data.layer_form_url;
            var modalClass = 'modal-sm';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, floorid: $(app.DE.ddlLevel).val(), shaftid: $(app.DE.ddlShaft).val(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateRoom = function (e) {
        var pageUrl = 'ISP/AddRoom';
        var modalClass = 'modal-sm';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'Unit Details', modalClass);
    }
    this.deleteRoom = function (systemId) {
        ajaxReq('ISP/DeleteRoomById', { systemId: systemId }, false, function (data) {
            if (data.status == 'OK') {
                //$('#Parent_' + systemId).remove();
                app.refreshStructureInfo();
                alert(data.message);
            }
        }, true, false);
    }

    /*HTB*/
    this.addHTB = function (layerDetails, templateId) {
        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq(layerDetails.Data.save_entity_url, { networkIdType: layerDetails.Data.network_id_type, templateId: templateId, objIspEntityMap: { floor_id: $(app.DE.ddlLevel).val(), structure_id: $(app.DE.StructureId).val(), shaft_id: $(app.DE.ddlShaft).val() }, isDirectSave: true }, false, function (response) {
                app.showMessageOverlay();
                $(popup.DE.closeModalPopup).trigger("click");
                if (response.Status == 'OK') {
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else { alert(response.Message); }

            }, true, false);
        }
        else {
            var pageUrl = layerDetails.Data.layer_form_url;
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, floorid: $(app.DE.ddlLevel).val(), shaftid: $(app.DE.ddlShaft).val(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateHTB = function (e) {
        var pageUrl = 'ISP/AddHTB';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'HTB Details', modalClass);
    }
    this.deleteHTB = function (systemId) {
        ajaxReq('ISP/DeleteHTBById', { systemId: systemId }, false, function (data) {
            //if (data.status == 'OK') {
            // $('#Parent_' + systemId).remove();
            app.refreshStructureInfo();
            alert(data.message);
            //}
        }, true, false);
    }

    /*FDB*/
    this.addFDB = function (layerDetails, templateId) {
        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq('ISP/SaveFDB', { networkIdType: layerDetails.Data.network_id_type, templateId: templateId, objIspEntityMap: { floor_id: $(app.DE.ddlLevel).val(), structure_id: $(app.DE.StructureId).val(), shaft_id: $(app.DE.ddlShaft).val() }, isDirectSave: true }, false, function (response) {
                app.showMessageOverlay();
                if (response.Status == 'OK') {
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else { alert(response.Message); }
            }, true, false)
        }
        else {
            var pageUrl = 'ISP/AddFDB';
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, floorid: $(app.DE.ddlLevel).val(), shaftid: $(app.DE.ddlShaft).val(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateFDB = function (e) {

        var pageUrl = 'ISP/AddFDB';
        var modalClass = 'modal-lg';
        var layerTitle = getLayerTltle('FDB');
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, layerTitle, modalClass);
    }
    this.deleteFDB = function (systemId) {
        ajaxReq('ISP/DeleteFDBById', { systemId: systemId }, false, function (data) {
            app.refreshStructureInfo();
            alert(data.message);
        }, true, false);
    }

    /*BDB*/
    this.addBDB = function (layerDetails, templateId) {

        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq('ISP/SaveBDB', {
                networkIdType: layerDetails.Data.network_id_type, parent_system_id: $(app.DE.StructureId).val(), templateId: templateId, objIspEntityMap: { floor_id: $(app.DE.ddlLevel).val(), structure_id: $(app.DE.StructureId).val(), shaft_id: $(app.DE.ddlShaft).val() }, isDirectSave: true
            }, false, function (response) {
                app.showMessageOverlay();
                if (response.Status == 'OK') {
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else {
                    alert(response.Message);
                }
            }, true, false);
        }
        else {
            var pageUrl = 'ISP/AddBDB';// layerDetails.Data.layer_form_url;
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, floorid: $(app.DE.ddlLevel).val(), shaftid: $(app.DE.ddlShaft).val(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateBDB = function (e) {
        var pageUrl = 'ISP/AddBDB';
        var modalClass = 'modal-lg';
        var layerTitle = getLayerTltle('BDB');
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, layerTitle, modalClass);
    }


    this.splitCaple = function (e) {
        mainCables = [];
        var lineLatLongValue = [];
        var cableStartEndPoint = [];
        var cablearr = [];
        var cablearr2 = [];
        var isVirtual = false;

        var sysId = $(e.currentTarget).attr('elementid');
        var enType = $(e.currentTarget).attr('elementtype');

        var x = ($('#' + enType.toUpperCase() + '_' + sysId).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
        var y = ($('#' + enType.toUpperCase() + '_' + sysId).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));

        var CableList = $('#svgContainer').children();

        $(CableList).each(function () {

            var cableId = $(this).attr('id');
            var path = $('#' + cableId + ' path').attr('d');
            var networkId = $(this).attr('networkid');
            var cablePathWithId = {
                id: cableId, Path: path, NetworkId: networkId
            };

            cablearr.push(cablePathWithId);
        });


        $.each(cablearr, function (index, item) {
            var cablePoints = item.Path.split('M');
            $.each(cablePoints, function (index, itemChild) {
                var latLong = null;
                if (index > 0) {
                    var moveTo = itemChild.split('L')[0];
                    var lineTo = itemChild.split('L')[1];
                    var lineToX = $.trim(lineTo).split(" ")[0];
                    var lineToY = $.trim(lineTo).split(" ")[1];

                    var distanceX = lineToX - x;
                    var distanceY = lineToY - y;
                    distanceY = parseInt(distanceY.toString().replace('-', ' '));

                    var cabledistance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);

                    latLong = {
                        id: item.id, moveTo: 'M ' + moveTo.split(' ')[1] + ' ' + moveTo.split(' ')[2], lineTo: 'L ' + lineTo.split(' ')[1] + ' ' + lineTo.split(' ')[2], isVirtual: isVirtual, markerKey: index, distance: cabledistance, NetworkId: item.NetworkId
                    };
                    lineLatLongValue.push(latLong);

                    if (index == 1 || cablePoints.length - 1 == index) {
                        cableStartEndPoint.push(latLong);
                    }
                }

            });

            var result = lineLatLongValue.reduce(function (res, obj) {
                return (obj.distance < res.distance) ? obj : res;
            });

            cablearr2.push(result);
            lineLatLongValue = [];
        });


        $.each(cablearr2, function (index, item) {

            if (item.distance < 100) {

                var cableIndex = 0;
                cableIndex = cableStartEndPoint.findIndex(p => p.id == item.id && p.markerKey == item.markerKey);
                if (cableIndex < 0) {
                    mainCables.push(item)
                }
            }
        });

        var pageUrl = 'ISP/getSplitCable';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: $(e.currentTarget).attr('elementtype'), systemId: $(e.currentTarget).attr('elementid') }, 'Split Cable Details', modalClass);

    }

    this.NearCable = function () {

        var rows = '';
        if (mainCables != 0) {
            $.each(mainCables, function (index, item) {

                var cableId = item.id;
                if (item.distance != 0) {
                    rows = '<tr><td><input type="radio" name="Cable" value="' + item.NetworkId.toString() + '" onchange="isp.showSplitCable(this);" s_id=' + item.id.toString().replace('ispCable_', '') + ' /><span>' + item.NetworkId.toString() + '</span><span style="float: right;"><a class="btn btn-small btn-primary" href="#FiberDetail" onclick="isp.GetSliptCableFiberDetail(' + item.id.toString().replace('ispCable_', '') + ');" data-toggle="tab">Fiber Details</a></span></td></tr>'; $('#nearCables tbody').append(rows);
                }
            });
        }
        else {
            $('#splitCableContent').css({
                "display": "none"
            });
            $('#dvBtnAction').css({
                "display": "none"
            });
            $('#NoContent').css({
                "display": "block"
            });

        }
    };

    this.GetSliptCableFiberDetail = function (cableid) {
        var formURL = 'Library/getFiberDetail';
        cable_id = cableid;
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ChildModel, formURL, { cableId: cableid }, 'Fiber Details', modalClass);
    }

    this.getNewCableDetails = function () {

        var splitcablesystemid = $("input[name='Cable']:checked").attr('s_id');
        $('#split_cable_system_id').val(splitcablesystemid);
        var splitEnityNetworkId = $('#split_entity_networkId').val();
        var splitEntity = $('#split_entity').val();
        var splitEntitySystem_id = $('#split_entity_system_id').val();
        var networkStatus = $('#ispCable_' + splitcablesystemid).attr('data-network-status');
        var CableValue = $("input[name='Cable']:checked").val();

        if (CableValue == undefined) {
            alert("	Please select cable to Process!");
        }
        else {
            if ($('#cable_one_start_reading').length > 0 && $('#cable_one_end_reading').length > 0) {
                $('#cable_one_start_reading,#cable_one_end_reading').keyup(function () {
                    app.calculateCableLength('', '' + networkStatus + '', 'cable_one_start_reading', 'cable_one_end_reading', 'cable_one_calculated_length', 'cable_one_measured_length')
                });
                $('#cable_two_start_reading,#cable_two_end_reading').keyup(function () {
                    app.calculateCableLength('', '' + networkStatus + '', 'cable_two_start_reading', 'cable_two_end_reading', 'cable_two_calculated_length', 'cable_two_measured_length')
                });
                if (networkStatus == 'P') {
                    $('#cable_one_start_reading,#cable_one_end_reading,#cable_one_calculated_length,#cable_one_measured_length').val(0);
                    $('#cable_two_start_reading,#cable_two_end_reading,#cable_two_calculated_length,#cable_two_measured_length').val(0);
                } else {
                    $('#cable_one_start_reading,#cable_one_end_reading').attr('required', true).val(null);
                    $('#cable_two_start_reading,#cable_two_end_reading').attr('required', true).val(null);
                }
            }
            var aLocation = $('#ispCable_' + splitcablesystemid).attr('data-a-location');
            var bLocation = $('#ispCable_' + splitcablesystemid).attr('data-b-location');
            var parentCable = CableValue;
            var firstCableNetworkId = parentCable + '_01';
            $('#cable_one_network_id').val(firstCableNetworkId);
            $('#cable_one_name').val(firstCableNetworkId);
            $('#cable_one_a_location').val(aLocation);
            $('#cable_one_b_location').val(splitEnityNetworkId);

            var spliEntityId = $('#split_entity_type').val() + '_' + splitEntitySystem_id;

            var secondCableNetworkId = parentCable + '_02';
            $('#cable_two_network_id').val(secondCableNetworkId);
            $('#cable_two_name').val(secondCableNetworkId);
            $('#cable_two_a_location').val(splitEnityNetworkId);
            $('#cable_two_b_location').val(bLocation);
            $('#split_entity_x').val($('#' + spliEntityId).position().left);
            $('#split_entity_y').val($('#' + spliEntityId).position().top);

            //OLD CODE
            //var splitCableValue = CableValue.split(':');
            //var cableTwo = splitCableValue[1].split('-CBL');

            //var cableOneValue = splitCableValue[0] + ':' + splitEnityNetworkId + '-CBL_01';

            //$('#cable_one_network_id').val(cableOneValue);
            //$('#cable_one_name').val(cableOneValue);
            //$('#cable_one_a_location').val(splitCableValue[0]);
            //$('#cable_one_b_location').val(splitEnityNetworkId);


            //var cableTwoValue = splitEnityNetworkId + ':' + cableTwo[0] + '-CBL_01';

            //$('#cable_two_network_id').val(cableTwoValue);
            //$('#cable_two_name').val(cableOneValue);
            //$('#cable_two_a_location').val(splitEnityNetworkId);
            //$('#cable_two_b_location').val(cableTwo[0]);
        }
    }

    this.SaveSplitCable = function () {
        var lineLatLongValue = [];

        $('#cable_one_network_id').val();
        $('#cable_two_network_id').val();
        var isVirtual = false;
        if ($('#cable_one_network_id').val() == "" && $('#cable_one_network_id').val() == "") {
            alert('Please click on process cable !');
        }
        else {
            var sysId = $('#split_entity_system_id').val();
            var enType = $('#split_entity_type').val();
            var x = ($('#' + enType.toUpperCase() + '_' + sysId).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
            var y = ($('#' + enType.toUpperCase() + '_' + sysId).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
            var splitcablesystemid = $("input[name='Cable']:checked").attr('s_id');
            var path = $('#ispCable_' + splitcablesystemid + ' path').attr('d');
            var cableone = '';
            var cableTwo = '';

            $.each(path.split('M'), function (index, item) {
                if (index > 0) {
                    var moveTo = item.split('L')[0];
                    var lineTo = item.split('L')[1];
                    var lineToX = $.trim(lineTo).split(" ")[0];
                    var lineToY = $.trim(lineTo).split(" ")[1];

                    var distanceX = lineToX - x;
                    var distanceY = lineToY - y;
                    distanceY = parseInt(distanceY.toString().replace('-', ' '));

                    var cabledistance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
                    var latLong = {
                        moveTo: 'M ' + moveTo.split(' ')[1] + ' ' + moveTo.split(' ')[2], lineTo: 'L ' + lineTo.split(' ')[1] + ' ' + lineTo.split(' ')[2], isVirtual: isVirtual, markerKey: index, distance: cabledistance
                    };
                    lineLatLongValue.push(latLong);
                }
            });

            var result = lineLatLongValue.reduce(function (res, obj) {
                return (obj.distance < res.distance) ? obj : res;
            });


            var indexToSplit = (result.markerKey) - 1;

            var firstCable = lineLatLongValue.slice(0, indexToSplit);
            var finalSecondCable = [];

            var secondCable = lineLatLongValue.slice(indexToSplit);

            //set the first cable end point 
            // firstCable[firstCable.length - 1].lineTo = "L " + x + " " + y;

            //if (firstCable.length != indexToSplit) {

            if (firstCable.length == 1) {

                finalSecondCable.push({
                    moveTo: firstCable[0].moveTo, lineTo: firstCable[0].lineTo
                });
                finalSecondCable.push({
                    moveTo: firstCable[0].moveTo, lineTo: "L " + x + " " + y
                });
            }
            else {

                firstCable[firstCable.length - 1].lineTo = "L " + x + " " + y;

                $.each(firstCable, function (index, item) {
                    finalSecondCable.push({
                        moveTo: item.moveTo, lineTo: item.lineTo
                    });
                });

            }

            //set the second cable start point 
            secondCable[0].moveTo = "M " + x + " " + y;

            var FinalSecondCable = [];
            var FirstArrayForSecendCable = {
                moveTo: "M " + x + " " + y, lineTo: "L " + x + " " + y
            };
            FinalSecondCable.push(FirstArrayForSecendCable);

            $.each(secondCable, function (index, item) {
                FinalSecondCable.push({
                    moveTo: item.moveTo, lineTo: item.lineTo
                });
            });


            $.each(finalSecondCable, function (index, item) {
                cableone = cableone.concat(item.moveTo + ' ' + item.lineTo)

            });

            $.each(FinalSecondCable, function (index, item) {
                cableTwo = cableTwo.concat(item.moveTo + ' ' + item.lineTo)
            });


            $('#ispLineGeomCableOne').val(cableone);
            $('#ispLineGeomCableTwo').val(cableTwo);

            if ((parseFloat($('#cable_one_end_reading').val()) != 0 || parseFloat($('#cable_one_start_reading').val()) != 0) && (parseFloat($('#cable_one_end_reading').val()) <= parseFloat($('#cable_one_start_reading').val()))) {
                alert('Cable 1 end reading can not be less then or equal to Cable 1 start reading!');
                return false;
            }
            if ((parseFloat($('#cable_two_end_reading').val()) != 0 || parseFloat($('#cable_two_start_reading').val()) != 0) && (parseFloat($('#cable_two_end_reading').val()) <= parseFloat($('#cable_two_start_reading').val()))) {
                alert('Cable 2 end reading can not be less then or equal to Cable 2 start reading!');
                return false;
            }


            //confirm('Splitting of this cable would result two new cable with same configuration. Splicing would be done automatically at both ends, if applicable. Are you sure to contiue ?', function () {
            //    $('#frmSplitCableMDU').submit();
            //});

            //}
            //else {
            //    alert("You can not split this cable because of the spliting point is very near");
            //}
        }

    };




    this.deleteBDB = function (systemId) {
        var parentId = '#Parent_' + systemId;
        //if ($(parentId + ' .SPLITTER, ' + parentId + ' .SPLICECLOSURE').length > 0) {
        //  alert('This Entity is associated with other entity,So you can not delete this entity!');
        //  return false;
        //} else {

        var postData = {
            systemId: systemId, entityType: 'BDB', geomType: 'Point', impactType: 'Deletion'
        };

        ajaxReq('Main/getDependentChildElements', postData, true, function (resp) {
            if (resp.status == "OK") {

                if (resp.results.ChildElements.length > 0) {
                    var Msg = '<table border="1" class="alertgrid"><tr><td><b>Entity</b></td><td><b>Network Id<b/></td></tr>';
                    $.each(resp.results.ChildElements, function (index, item) {
                        Msg += '<tr><td>' + item.Entity_Type + ' </td><td> ' + item.Network_Id + '</td></tr>';
                    });
                    Msg += '</table>';
                    alert('Following are the dependent elements. You need to remove them first:<br/>' + Msg);
                }
                else {
                    var layerTitle = getLayerTltle(postData["entityType"]);

                    confirm('Are you sure,do you want to delete this ' + layerTitle + '?', function () {
                        ajaxReq('ISP/DeleteBDBById', {
                            systemId: systemId
                        }, false, function (data) {
                            app.refreshStructureInfo();
                            alert(data.message);
                        }, true, false);
                    });
                }
            }
            else {
                alert(resp.message);

            }
        }, true, true);
        //}
    }

    /*SPLITTER*/
    this.addSplitter = function (layerDetails, templateId) {
        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq('ISP/SaveSplitter', {
                networkIdType: layerDetails.Data.network_id_type, objIspEntityMap: { structure_id: $(app.DE.StructureId).val() }, templateId: templateId, pSystemId: $(app.DE.SplitterParent).val().split('_')[1], pEntityType: $(app.DE.SplitterParent).val().split('_')[0].toUpperCase(), isDirectSave: true
            }, false, function (response) {
                app.showMessageOverlay();
                if (response.Status == 'OK'){
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else {
                    alert(response.Message);
                }
            }, true, false);
        }
        else {
            var pageUrl = 'ISP/AddSplitter';
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, pSystemId: $(app.DE.SplitterParent).val().split('_')[1], pEntityType: $(app.DE.SplitterParent).val().split('_')[0].toUpperCase(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateSplitter = function (e) {
        var pageUrl = 'ISP/AddSplitter';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'Splitter Details', modalClass);
    }
    this.deleteSplitter = function (systemId) {
        ajaxReq('ISP/DeleteSplitterById', {
            systemId: systemId
        }, false, function (data) {

            // $('#SPLITTER_' + systemId).remove();
            app.refreshStructureInfo();
            alert(data.message);

        }, true, false);
    }

    /*SPLICE CLOSURE*/
    this.addSC = function (layerDetails, templateId) {



        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq('ISP/SaveSC', {
                networkIdType: layerDetails.Data.network_id_type, objIspEntityMap: { structure_id: $(app.DE.StructureId).val() }, templateId: templateId, parent_system_id: $(app.DE.SplitterParent).val().split('_')[1], parent_entity_type: $(app.DE.SplitterParent).val().split('_')[0].toUpperCase(), pNetworkId: $(app.DE.SplitterParent + ' :selected').text(), isDirectSave: true
            }, false, function (response) {
                app.showMessageOverlay();
                if (response.Status == 'OK') {
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else {
                    alert(response.Message);
                }
            }, true, false);
        }
        else {
            var pageUrl = 'ISP/AddSpliceClosure';
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, pSystemId: $(app.DE.SplitterParent).val().split('_')[1], pEntityType: $(app.DE.SplitterParent).val().split('_')[0].toUpperCase(), pNetworkId: $(app.DE.SplitterParent + ' :selected').text(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateSC = function (e) {
        var pageUrl = 'ISP/AddSpliceClosure';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'Splice Closure Details', modalClass);
    }
    this.deleteSC = function (systemId) {
        ajaxReq('ISP/DeleteSCById', {
            systemId: systemId
        }, false, function (data) {

            //$('#SPLICECLOSURE_' + systemId).remove();
            app.refreshStructureInfo();
            alert(data.message);

        }, true, false);
    }

    /*ONT*/
    this.addONT = function (layerDetails, templateId) {
        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq('ISP/SaveONT', {
                networkIdType: layerDetails.Data.network_id_type, objIspEntityMap: { floor_id: $(app.DE.ddlLevel).val(), structure_id: $(app.DE.StructureId).val(), shaft_id: $(app.DE.ddlShaft).val() }, templateId: templateId, isDirectSave: true
            }, false, function (response) {
                app.showMessageOverlay();
                if (response.Status == 'OK') {
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else {
                    alert(response.Message);
                }
            }, true, false);
        }
        else {
            var pageUrl = 'ISP/AddONT';
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, floorid: $(app.DE.ddlLevel).val(), shaftid: $(app.DE.ddlShaft).val(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateONT = function (e) {
        var pageUrl = 'ISP/AddONT';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'ONT Details', modalClass);
    }
    this.deleteONT = function (systemId) {
        var postData = {
            systemId: systemId, entityType: 'ONT', geomType: 'Point', impactType: 'Deletion'
        };

        ajaxReq('Main/getDependentChildElements', postData, true, function (resp) {
            if (resp.status == "OK") {

                if (resp.results.ChildElements.length > 0) {
                    var Msg = '<table border="1" class="alertgrid"><tr><td><b>Entity</b></td><td><b>Network Id<b/></td></tr>';
                    $.each(resp.results.ChildElements, function (index, item) {
                        Msg += '<tr><td>' + item.Entity_Type + ' </td><td> ' + item.Network_Id + '</td></tr>';
                    });
                    Msg += '</table>';
                    alert('Following are the dependent elements. You need to remove them first:<br/>' + Msg);
                }
                else {
                    confirm('Are you sure,do you want to delete this ONT?', function () {
                        ajaxReq('ISP/DeleteONTById', {
                            systemId: systemId
                        }, false, function (data) {
                            app.refreshStructureInfo();
                            alert(data.message);
                        }, true, false);
                    });
                }
            }
            else {
                alert(resp.message);

            }
        }, true, true);

    }


    /*ADB*/
    this.deleteADB = function (systemId) {
        ajaxReq('Library/DeleteADBById', {
            systemId: systemId
        }, false, function (data) {
            app.refreshStructureInfo();
            alert(data.message);
        }, true, false);
    }


    /*CDB*/
    this.deleteCDB = function (systemId) {
        ajaxReq('Library/DeleteCDBById', {
            systemId: systemId
        }, false, function (data) {
            app.refreshStructureInfo();
            alert(data.message);
        }, true, false);
    }


    /*CUSTOMER*/
    this.addCustomer = function (layerDetails, templateId) {
        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq('ISP/SaveCustomer', {
                networkIdType: layerDetails.Data.network_id_type, objIspEntityMap: { floor_id: $(app.DE.ddlLevel).val(), structure_id: $(app.DE.StructureId).val(), shaft_id: $(app.DE.ddlShaft).val() }, pSystemId: $(app.DE.CustomerParent).val().split('_')[1], pEntityType: $(app.DE.CustomerParent).val().split('_')[0].toUpperCase(), pNetworkId: $(app.DE.CustomerParent + ' :selected').text(), isDirectSave: true
            }, false, function (response) {
                app.showMessageOverlay();
                if (response.Status == 'OK') {
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else {
                    alert(response.Message);
                }
            }, true, false);
        } else {
            var pageUrl = 'ISP/AddCustomer';
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, floorid: $(app.DE.ddlLevel).val(), shaftid: $(app.DE.ddlShaft).val(), pSystemId: $(app.DE.CustomerParent).val().split('_')[1], pEntityType: $(app.DE.CustomerParent).val().split('_')[0].toUpperCase(), pNetworkId: $(app.DE.CustomerParent + ' :selected').text(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateCustomer = function (e) {
        var pageUrl = 'ISP/AddCustomer';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'Customer Details', modalClass);
    }
    this.deleteCustomer = function (systemId) {
        ajaxReq('ISP/DeleteCustomerById', {
            systemId: systemId
        }, false, function (data) {
            // if (data.status == 'OK') {
            app.refreshStructureInfo();
            alert(data.message);
            //}
        }, true, false);
    }

    /*POD*/
    this.updatePOD = function (e) {
        var pageUrl = 'Library/AddPOD';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'POD', modalClass);
    }
    this.deletePOD = function (systemId) {

        ajaxReq('Library/DeletePODById', {
            systemId: systemId
        }, false, function (data) {

            app.refreshStructureInfo();
            alert(data.message);

        }, true, false);
    }
    /*MPOD*/
    this.updateMPOD = function (e) {
        var pageUrl = 'Library/AddMPOD';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'MPOD', modalClass);
    }
    this.deleteMPOD = function (systemId) {
        ajaxReq('Library/DeleteMPODById', {
            systemId: systemId
        }, false, function (data) {
            //if (data.status == 'OK') {
            //    app.refreshStructureInfo();
            //    alert(data.message);
            //}


            app.refreshStructureInfo();
            alert(data.message);


        }, true, false);
    }
    /*Cable*/
    this.addCable = function (data) {
        popup.LoadModalDialog(app.ParentModel, "ISP/AddISPCable", data, "Add ISP Cable", "modal-lg", function () {
            $('#hdnLineGeom').val(app.lineGeom);
            app.isEditMode = false;
            app.editCableId = '';
        });
    }
    this.updateCable = function (e) {
        var pageUrl = 'ISP/AddISPCable';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid'), ModelInfo: { structureid: $(app.DE.StructureId).val() } }, 'Update ISP Cable', modalClass);
    }
    this.deleteCable = function (systemId) {
        var postData = {
            systemId: systemId, entityType: 'Cable', geomType: 'Line'
        };
        ajaxReq('Main/DeleteEntityFromInfo', postData, false, function (data) {
            //if (data.status == 'OK') {
            app.refreshStructureInfo();
            app.clearDragableObject();
            alert(data.message);
            //}
        }, true, false);
    }
    this.calculateCableLength = function (obj, networkStatus, objStartReading, objEndReading, objCalLength, objMeasuredLength) {
        var startReading = parseFloat($('#' + objStartReading).val());
        var endReading = parseFloat($('#' + objEndReading).val());
        if (networkStatus == 'A' && startReading == 0) {
            $('#' + objStartReading).val('');
            return false;
        }
        if (networkStatus == 'A' && endReading == 0) {
            $('#' + objEndReading).val('');
            return false;
        }
        var length = endReading - startReading;
        if (length > 0) {
            $('#' + objCalLength).val(Math.round(length * 100) / 100);
            $('#' + objMeasuredLength).val(Math.round(length * 100) / 100);
        } else {
            $('#' + objCalLength).val(0);
            $('#' + objMeasuredLength).val(0);
        }
    }

    /*FMS*/
    this.addFMS = function (layerDetails, templateId) {
        if (layerDetails.Data.is_direct_save == true) {
            ajaxReq('ISP/SaveFMS', {
                networkIdType: layerDetails.Data.network_id_type, objIspEntityMap: { structure_id: $(app.DE.StructureId).val() }, templateId: templateId, parent_system_id: $(app.DE.FMSParent).val().split('_')[1], parent_entity_type: $(app.DE.FMSParent).val().split('_')[0].toUpperCase(), pNetworkId: $(app.DE.FMSParent + ' :selected').text(), isDirectSave: true
            }, false, function (response) {
                app.showMessageOverlay();
                if (response.Status == 'OK') {
                    app.refreshStructureInfo();
                    if (response.Data != null && response.Data.entityType != null) {
                        app.refreshNLayers(response.Data.entityType);
                    }
                    alert(response.Message);
                } else {
                    alert(response.Message);
                }
            }, true, false);
        }
        else {
            var pageUrl = 'ISP/AddFMS';
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: layerDetails.Data.network_id_type, elementType: $(app.DE.ElementType).attr('data-layer-name').toUpperCase(), structureid: $(app.DE.StructureId).val(), templateId: templateId, pSystemId: $(app.DE.FMSParent).val().split('_')[1], pEntityType: $(app.DE.FMSParent).val().split('_')[0].toUpperCase(), pNetworkId: $(app.DE.FMSParent + ' :selected').text(), systemId: 0 }, layerDetails.Data.layer_title, modalClass);
        }
    }
    this.updateFMS = function (e) {
        var pageUrl = 'ISP/AddFMS';
        var modalClass = 'modal-lg';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { systemId: $(e.currentTarget).attr('elementid') }, 'FMS', modalClass);
    }
    this.deleteFMS = function (systemId) {
        ajaxReq('ISP/DeleteFMSById', {
            systemId: systemId
        }, false, function (data) {
            app.refreshStructureInfo();
            alert(data.message);

        }, true, false);
    }
    this.validateFloorDimension = function () {
        var floorId = '#FLOOR_' + $('#floor_id').val();
        var floorHeight = parseFloat($(floorId).attr('height'));
        var floorWidth = parseFloat($(floorId).attr('width'));
        var floorLength = parseFloat($(floorId).attr('length'));


        var roomHeight = parseFloat($('#txtRoomHeight').val());
        var roomWidth = parseFloat($('#txtRoomWidth').val());
        var roomLength = parseFloat($('#txtRoomLength').val());
        if ($('#system_id').val() != 0) {
            $('#Parent_' + $('#system_id').val()).attr('length', roomLength)
        }
        if (roomHeight > floorHeight) {
            app.showMessageOverlay(); alert('UNIT hieght can not be greater then floor height!'); return false;
        }
        if (roomWidth > floorWidth) {
            app.showMessageOverlay(); alert('UNIT width can not be greater then floor width!'); return false;
        }
        if (roomLength > floorLength) {
            app.showMessageOverlay(); alert('UNIT length can not be greater then floor length!'); return false;
        }

        var allRoomLength = 0;

        $(floorId).children('[elementtype="UNIT"]').each(function () {
            allRoomLength += parseFloat($(this).attr('data-additional-attributes').split('|')[2].replace('length=', ''));
        })
        if ($('#system_id').val() == 0) {
            allRoomLength = allRoomLength + roomLength;
        }
        if (allRoomLength > floorLength) {
            app.showMessageOverlay(); alert('selected UNIT can not be added into <b> ' + $(floorId).text() + ' </b> as UNIT dimension is greater than the available space!'); return false;
        }
        $('#frmAddRoom').submit();

    }
    //textbox can shown only numeric datatype
    this.allowOnlyNumber = function (evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }

    this.roundNumber = function (num, dec) {
        var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec) + '';
        var r = result.indexOf('.');
        return result;
    }
    this.GetISPCableTubeCoreInfo = function (cableid) {
        ajaxReq('Library/GetCableTubeCoreDetail', {
            cableId: cableid
        }, false, function (resp) {
            $("#TubeColor").html(resp);
        }, false, false);

    }
    this.refreshStructureInfo = function () {
        $('#ancrStructureInfo').trigger("click");
    }
    this.showMessageOverlay = function () {
        $('#alertBackOverLay').fadeIn();
    }

    this.refreshNLayers = function (enType) {
        if (enType != '' && enType != null) {
            enType = enType.toUpperCase();
            var cableCHKCount = $('#layersContainer ul input[data-mapabbr="' + enType + '"]').length;
            if (cableCHKCount == 0) {
                $('#liNoLayer').remove(); $('#layersContainer ul').append('<li class="ispLayerTree"><input id="chk_nLyr_' + enType + '" type="checkbox" data-mapabbr="' + enType + '"  checked="checked" class="checkbox-custom"><b><label for="chk_nLyr_' + enType + '" class="checkbox-custom-label"><span>' + enType + '</span></label></b></li>');
            }
        }
    }

    /*Line Drawing Code*/
    function makeDraggable(divId) {
        $(divId + ' svg').unbind('dragstop');
        $(divId + ' svg').attr('pointer-events', '');
        $(divId + ' svg')
          .draggable()
          .bind('drag', function (event, ui) {

              var x = (event.pageX - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
              var y = (event.pageY - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));


              //GET INDEX BASED ON MARKER KEY FROM LINE ARRAY
              var removeIndex = app.lineLatLong.findIndex(p => p.markerKey == $(this).attr('data-svg-marker-key'));
              if (removeIndex > 0) {
                  updateVMarkerPositionNew((removeIndex), x, y);
              }
              if (removeIndex != undefined && removeIndex >= 0) {

                  if (removeIndex == app.lineLatLong.length - 1) {
                      //UPDATE PREVCLICK VALUE..
                      app.prevordX = x;
                      app.prevordX = y;
                  }


                  if (app.lineLatLong[removeIndex].isVirtual == true) {

                      //convert virtual point to real point..
                      app.lineLatLong[removeIndex].isVirtual = false;

                      //bind right click event
                      $(this).bind("contextmenu", function (e) {
                          app.removePoint('svgVMarker', $(this).attr('data-svg-marker-key'))
                      });

                      //create left virtual point..
                      var objLeftVirtualPoint = createleftVMarker(removeIndex, x, y);
                      //create right virtual point..
                      var objRightVirtualPoint = createRightVMarker(removeIndex, x, y, objLeftVirtualPoint != null ? true : false);

                      //push left virtual point into array..
                      if (objLeftVirtualPoint != null) {
                          app.lineLatLong.splice(removeIndex, 0, objLeftVirtualPoint);
                      }

                      //push right virtual point into array..
                      if (objRightVirtualPoint != null) {
                          app.lineLatLong.splice(removeIndex + 2, 0, objRightVirtualPoint);
                      }



                      $(this).children('#marker-layer').children('circle').css({
                          'opacity': '1', 'fill': 'blue'
                      });
                      //$(this).children('#handle-layer').children('circle').css('fill','black');
                      //$(this).children('#handle-layer').children('circle').css('stroke','none');

                      makeDraggable('#divMarker');

                      //$(this).css('left',x+83);
                      //$(this).css('top',y+88);

                  }

                  else  //virtual false
                  {
                      // left side
                      // get left mid point

                      if (removeIndex > 1) {
                          var leftMidX = (parseFloat(app.lineLatLong[removeIndex - 2].lineTo.split(' ')[1]) + x) / 2;
                          var leftMidY = (parseFloat(app.lineLatLong[removeIndex - 2].lineTo.split(' ')[2]) + y) / 2;
                          //update virutal point position
                          app.lineLatLong[removeIndex - 1].moveTo = app.lineLatLong[removeIndex - 2].lineTo.replace('L', 'M');
                          app.lineLatLong[removeIndex - 1].lineTo = 'L ' + leftMidX + ' ' + leftMidY;
                          //update current element position

                          app.lineLatLong[removeIndex].moveTo = 'M ' + leftMidX + ' ' + leftMidY;
                          app.lineLatLong[removeIndex].lineTo = 'L ' + x + ' ' + y;

                          updateVMarkerPositionNew((removeIndex - 1), leftMidX, leftMidY);
                      }

                      if (removeIndex <= app.lineLatLong.length - 3) {

                          var rightMidX = (parseFloat(app.lineLatLong[removeIndex + 2].lineTo.split(' ')[1]) + x) / 2;
                          var rightMidY = (parseFloat(app.lineLatLong[removeIndex + 2].lineTo.split(' ')[2]) + y) / 2;

                          //update virutal point position
                          app.lineLatLong[removeIndex + 1].moveTo = 'M ' + x + ' ' + y;
                          app.lineLatLong[removeIndex + 1].lineTo = 'L ' + rightMidX + ' ' + rightMidY;
                          //update next element move to only

                          app.lineLatLong[removeIndex + 2].moveTo = 'M ' + rightMidX + ' ' + rightMidY;

                          updateVMarkerPositionNew((removeIndex + 1), rightMidX, rightMidY);

                      }

                      //if first element
                      if (removeIndex == 0) {
                          app.lineLatLong[removeIndex].moveTo = 'M ' + x + ' ' + y;
                          app.lineLatLong[removeIndex].lineTo = 'L ' + x + ' ' + y;
                      }

                  }
              }
              RedrawLine();
              //if(lineLatLong[removeIndex].isVirtual==false){
              //updateVMarkerPosition(removeIndex,x,y);
              //}
              //setTimeout(function(){
              //    $(window).bind("click", function (e) {
              //        app.drawline(e);
              //    });
              //},1000)
          });

    }

    function addMarkerToLineArray(moveX, moveY, lineX, lineY, _isVirtual, _markerKey) {

        var latLong = {
            moveTo: 'M ' + moveX + ' ' + moveY, lineTo: 'L ' + lineX + ' ' + lineY, isVirtual: _isVirtual, markerKey: _markerKey
        };
        app.lineLatLong.push(latLong);
        return app.lineLatLong.length > 0 ? app.lineLatLong.length - 1 : 0;
    }

    function getMarkerKey() {
        if (app.lineLatLong.length > 0) {
            return (app.lineLatLong.reduce((max, p) => p.markerKey > max ? p.markerKey : max, app.lineLatLong[0].markerKey)) + 1;
        }
        else {
            return 1;
        }
    }

    this.getMarkerSVG = function (markerKey, posX, posY, isVirtual) {
        var markerLayer;
        var leftPos = (parseFloat(posX) + $(".StructureInfo").offset().left) + 2;
        var topPos = (parseFloat(posY) + $(".StructureInfo").offset().top) + 2;
        var topScroll = $('#divISPView').scrollTop();
        var leftScroll = $('.ISPViewContainer').scrollLeft();
        if (isVirtual) {
            //var markerLayer = "<svg id=\"svgVMarker" + markerKey + "\" data-svg-marker-key=" + markerKey + " style=\"position:fixed;left:" + leftPos + "px;top:" + topPos + "px;\" width=\"30\" height=\"30\" pointer-events=\"none\" position=\"absolute\"  version=\"1.1\"xmlns=\"http://www.w3.org/1999/xhtml\" class=\"_jsPlumb_connector\"><g id=\"marker-layer\"><circle  class=\"vmarker\" r=\"4\" cx=\"15\" cy=\"15\"></circle></g>";            
            var markerLayer = "<svg id=\"svgVMarker" + markerKey + "\" data-svg-marker-key=" + markerKey + " data-svg-marker-left=" + leftPos + "  data-svg-marker-top=" + topPos + " data-svg-prev-scroll=" + topScroll + " data-svg-prev-hscroll=" + leftScroll + " style=\"position:fixed;left:" + leftPos + "px;top:" + (topPos) + "px;\" width=\"30\" height=\"30\" pointer-events=\"none\" position=\"absolute\"  version=\"1.1\"xmlns=\"http://www.w3.org/1999/xhtml\" class=\"_jsPlumb_connector\"><g id=\"marker-layer\"><circle  class=\"vmarker\" r=\"5\" cx=\"15\" cy=\"15\"></circle></g>";
            markerLayer += "</svg>";

        }
        else {
            //markerLayer = "<svg id=\"svgMarker" + markerKey + "\" oncontextmenu=\"isp.removePoint('svgMarker',"+markerKey+");\" data-svg-marker-key=" + markerKey + "  style=\"position:fixed;left:" + leftPos + "px;top:" + topPos + "px;\" width=\"30\" height=\"30\" pointer-events=\"none\" position=\"absolute\"  version=\"1.1\"xmlns=\"http://www.w3.org/1999/xhtml\" class=\"_jsPlumb_connector\"><g id=\"marker-layer\"><circle  class=\"marker\" r=\"4\" cx=\"15\" cy=\"15\"></circle></g>";            
            markerLayer = "<svg id=\"svgMarker" + markerKey + "\" oncontextmenu=\"isp.removePoint('svgMarker'," + markerKey + ");\" data-svg-marker-key=" + markerKey + "  data-svg-marker-left=" + leftPos + "  data-svg-marker-top=" + topPos + " data-svg-prev-scroll=" + topScroll + " data-svg-prev-hscroll=" + leftScroll + "  style=\"position:fixed;left:" + leftPos + "px;top:" + (topPos) + "px;\" width=\"30\" height=\"30\" pointer-events=\"none\" position=\"absolute\"  version=\"1.1\"xmlns=\"http://www.w3.org/1999/xhtml\" class=\"_jsPlumb_connector\"><g id=\"marker-layer\"><circle  class=\"marker\" r=\"5\" cx=\"15\" cy=\"15\"></circle></g>";
            markerLayer += "</svg>";
        }
        return markerLayer;
    }

    function RedrawLine() {
        var dPath = '';
        $.each(app.lineLatLong, function (indx, item) {           
            dPath += item.moveTo + ' ' + item.lineTo;            
        });
        if (app.isEditMode == true) {
            app.editCableId;
            app.editedCableGeom = dPath;
            $(app.editCableId + ' path').attr('d', dPath);
        } else {
            $('#svgContainer svg').last().children('path').attr('d', dPath);
            app.lineGeom = dPath;
        }
    }
    function createleftVMarker(removeIndex, x, y) {

        var distanceX = app.lineLatLong[removeIndex].moveTo.split(' ')[1] - x;
        var distanceY = app.lineLatLong[removeIndex].moveTo.split(' ')[2] - y;
        var distance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
        if (distance > 0) {

           //var firstPoint = app.lineLatLong[removeIndex - 1].moveTo.split(' ')[1];
          //var currentPoint = app.lineLatLong[removeIndex].lineTo.split(' ')[2];

            // get mid points..
            var leftMidX =(parseFloat(app.lineLatLong[removeIndex].moveTo.split(' ')[1]) + x) / 2;
            var leftMidY = (parseFloat(app.lineLatLong[removeIndex].moveTo.split(' ')[2]) + y) / 2;

            //prepare array object for new mid point
            var mKey = getMarkerKey();
            var latLong = {
                moveTo: app.lineLatLong[removeIndex].moveTo, lineTo: 'L ' + leftMidX + ' ' + leftMidY, isVirtual: true, markerKey: mKey
            };

            //update current index position
            app.lineLatLong[removeIndex].moveTo = 'M ' + leftMidX + ' ' + leftMidY;
            app.lineLatLong[removeIndex].lineTo = 'L ' + x + ' ' + y;

            //create marker for left virtual point..
            $('#divMarker').append(app.parseSVG(app.getMarkerSVG(mKey, leftMidX, leftMidY, true)));
            return latLong;
        }
        return null;
    }

    function createRightVMarker(removeIndex, x, y, isleftVPointCreated) {

        var distanceX = app.lineLatLong[removeIndex + 1].lineTo.split(' ')[1] - x;
        var distanceY = app.lineLatLong[removeIndex + 1].lineTo.split(' ')[2] - y;
        var distance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
        if (distance > 0) {
            // get mid points..
            var rightMidX = (parseFloat(app.lineLatLong[removeIndex + 1].lineTo.split(' ')[1]) + x) / 2;
            var rightMidY = (parseFloat(app.lineLatLong[removeIndex + 1].lineTo.split(' ')[2]) + y) / 2;

            //prepare array object for new mid point
            var mKey = isleftVPointCreated == true ? getMarkerKey() + 1 : getMarkerKey();
            var latLong = {
                moveTo: 'M ' + x + ' ' + y, lineTo: 'L ' + rightMidX + ' ' + rightMidY, isVirtual: true, markerKey: mKey
            };

            //update next index move to only
            app.lineLatLong[removeIndex + 1].moveTo = 'M ' + rightMidX + ' ' + rightMidY;

            //create marker for right virtual point..
            $('#divMarker').append(app.parseSVG(app.getMarkerSVG(mKey, rightMidX, rightMidY, true)));

            return latLong;

        }

        return null;
    }

    function createMidVMarker(removeIndex) {


        // get mid points..
        var midX = (parseFloat(app.lineLatLong[removeIndex + 2].lineTo.split(' ')[1]) + parseFloat(app.lineLatLong[removeIndex - 2].lineTo.split(' ')[1])) / 2;
        var midY = (parseFloat(app.lineLatLong[removeIndex + 2].lineTo.split(' ')[2]) + parseFloat(app.lineLatLong[removeIndex - 2].lineTo.split(' ')[2])) / 2;

        //prepare array object for new mid point
        var mKey = getMarkerKey();
        var latLong = {
            moveTo: app.lineLatLong[removeIndex - 2].lineTo.replace('L', 'M'), lineTo: 'L ' + midX + ' ' + midY, isVirtual: true, markerKey: mKey
        };

        //update current index position
        app.lineLatLong[removeIndex + 2].moveTo = 'M ' + midX + ' ' + midY;

        //create marker for left virtual point..
        $('#divMarker').append(app.parseSVG(app.getMarkerSVG(mKey, midX, midY, true)));
        // var objMarker={latLong:latLong,leftvMkey:lineLatLong[removeIndex-1].marekrKey,rightvMkey:''};
        return latLong;


    }

    function updateVMarkerPositionNew(index, x, y) {
        var markerKey = app.lineLatLong[index].markerKey;



        var prevScroll = $('#svgVMarker' + (markerKey)).attr('data-svg-prev-scroll');
        var prevLeftScroll = $('#svgVMarker' + (markerKey)).attr('data-svg-prev-hscroll');
        //var leftPos=(parseFloat(x))+$("#svgContainer").offset().left-($('#divISPView').scrollLeft()-prevLeftScroll);
        //var topPos=((parseFloat(y))+$("#svgContainer").offset().top)-($('#divISPView').scrollTop()-app.prevScroll);


        var leftPos = (parseFloat(x) + $("#svgContainer").offset().left) - parseInt($(".StructureInfo").css("padding-left"));
        var topPos = (parseFloat(y) + $("#svgContainer").offset().top) - parseInt($(".StructureInfo").css("padding-top"));

        $('#svgVMarker' + (markerKey)).css('left', leftPos).css('top', topPos);
        $('#svgVMarker' + (markerKey)).attr('data-svg-marker-top', topPos);
        $('#svgVMarker' + (markerKey)).attr('data-svg-marker-left', leftPos);
    }

    this.removePoint = function (markerId, marekrKey) {
        //GET INDEX BASED ON MARKER KEY FROM LINE ARRAY
        var removeIndex = app.lineLatLong.findIndex(p => p.markerKey == marekrKey);
        //if(removeIndex==0)//Stop Start Point Remove
        //{
        //var vmarkerKey=app.lineLatLong[removeIndex+1].markerKey;
        //app.lineLatLong.splice(removeIndex, 2);              
        //app.lineLatLong[removeIndex].moveTo=app.lineLatLong[removeIndex].lineTo.replace('L','M');
        //    $('#svgVMarker'+vmarkerKey).remove();

        //}else if(removeIndex==app.lineLatLong.length-1)//Stop Start Point Remove
        //{
        //    var vmarkerKey=app.lineLatLong[removeIndex-1].markerKey;
        //    app.lineLatLong.splice(removeIndex, 1);
        //    app.lineLatLong.splice(removeIndex-1, 1);
        //    $('#svgVMarker'+vmarkerKey).remove();
        //    app.prevordX = app.lineLatLong[removeIndex-2].lineTo.split(' ')[1];
        //    app.prevordY = app.lineLatLong[removeIndex-2].lineTo.split(' ')[2];
        //}
        //else
        if (removeIndex != 0 && removeIndex != app.lineLatLong.length - 1) {
            //lineLatLong[removeIndex+1].moveTo=lineLatLong[removeIndex].moveTo;

            var leftVmarkerKey = app.lineLatLong[removeIndex - 1].markerKey;
            var rightVmarkerKey = app.lineLatLong[removeIndex + 1].markerKey;
            var currentMarkerKey = app.lineLatLong[removeIndex].markerKey;
            var midVmarker = createMidVMarker(removeIndex);
            if (midVmarker != null) {
                app.lineLatLong.splice(removeIndex, 0, midVmarker);
            }
            app.lineLatLong.splice(app.lineLatLong.findIndex(p => p.markerKey == leftVmarkerKey), 1);
            app.lineLatLong.splice(app.lineLatLong.findIndex(p => p.markerKey == rightVmarkerKey), 1);
            app.lineLatLong.splice(app.lineLatLong.findIndex(p => p.markerKey == currentMarkerKey), 1);
            $('#svgVMarker' + leftVmarkerKey).remove();
            $('#svgVMarker' + rightVmarkerKey).remove();
        }
        $('#' + markerId + marekrKey).remove();
        RedrawLine();
        makeDraggable('#divMarker');
    }

    this.editCable = function (system_id) {
        app.clearDragableObject();
        var path = $('#ispCable_' + system_id + ' path').attr('d');
        app.isEditMode = true;
        app.editCableId = '#ispCable_' + system_id;
        var isVirtual = false;
        $(app.editCableId + ' path').attr('stroke-width', 5);
        $(app.editCableId + ' path').attr('stroke', 'red');
        $.each(path.split('M'), function (index, item) {
            if (index > 0) {
                var moveTo = item.split('L')[0];
                var lineTo = item.split('L')[1];
                var latLong = {
                    moveTo: 'M ' + moveTo.split(' ')[1] + ' ' + moveTo.split(' ')[2], lineTo: 'L ' + lineTo.split(' ')[1] + ' ' + lineTo.split(' ')[2], isVirtual: isVirtual, markerKey: index
                };
                isp.lineLatLong.push(latLong);
                //create marker for left virtual point..
                $('#divMarker').append(isp.parseSVG(isp.getMarkerSVG(index, (item.split('L')[1]).split(' ')[1], (item.split('L')[1]).split(' ')[2], index)));
                if (isVirtual == false) {
                    isVirtual = true;
                    //bind right click event
                    $('#svgVMarker' + index).bind("contextmenu", function (e) {
                        app.removePoint('svgVMarker', index)
                    });
                } else {
                    isVirtual = false;
                }
            }
        });
        makeDraggable('#divMarker');
        $('#svgVMarker' + app.lineLatLong[0].markerKey).remove();
        $('#svgVMarker' + app.lineLatLong[app.lineLatLong.length - 1].markerKey).remove();

    }
    this.saveEdtitCablegeom = function (system_id) {
        if (app.editedCableGeom != '') {
            ajaxReq('ISP/SaveCablegeom', {
                systemId: system_id, cableGeom: app.editedCableGeom, structureId: $('#StructureId').val()
            }, false, function (data) {
                if (data.Status == 'OK') {
                    app.showMessageOverlay();
                    alert(data.Message);
                    app.clearDragableObject();
                    app.isEditMode = false;
                    app.editCableId = '';
                    $('.rel_pop').hide();
                    app.clearEditMode();
                }
            }, true, false);
        } else {
            app.showMessageOverlay();
            alert('No changes found in cable location!');
        }
    }
    this.clearDragableObject = function () {
        app.lineLatLong = [];
        app.editedCableGeom = '';
        $('#divMarker').html('');
    }
    this.setSVGWIdthHeight = function () {

        $('#svgContainer svg').css({
            'width': $('.StructureInfo').width(), 'height': $('.StructureInfo').height(), 'max-width': $('.StructureInfo').width(), 'max-height': $('.StructureInfo').height()
        });
    }
    this.getColor = function () {
        //
        //var color_arr = ['#000000', '#f12957', '#7d0031', '#313aa0', '#313aa0', '#171d5b', '#7d12d0', '#060329', '#d518d6', '#b5181d'];
        //var color_arr = [ '#313aa0']#0000FF;
        var color_arr = ['#0000FF'];
        var arr_ind = Math.floor(Math.random() * 10);
        var a = color_arr[arr_ind];
        return '#0000FF';
    }
    //this.showWLTB = function (tbType, divObj) {
    //    var func = function () { };
    //    switch (tbType) {

    //        case 'Survey':
    //            app.makeTempOrange(divObj, 'toolMiscClick');
    //            func = function () { $('#surveyTBar').toggle('slide', { direction: 'up' }, 500); };
    //            if ($('#wLineTBar').is(":visible")) { $('#wLineTBar').toggle('slide', { direction: 'up' }, 500, func()); }
    //            else if ($('#wLessTBar').is(":visible")) { $('#wLessTBar').toggle('slide', { direction: 'up' }, 500, func()); }
    //            else if ($('#dvRadiusTools').is(":visible")) { $('#dvRadiusTools').toggle('slide', { direction: 'up' }, 500, func()); }
    //            else if ($('#dvMiscTools').is(":visible")) { $('#dvMiscTools').toggle('slide', { direction: 'up' }, 500, func()); }
    //            else
    //                func();

    //            //Set width auto..
    //            $('#surveyTBar').css('width', 'auto !important');
    //            break;
    //        case 'WireLine':
    //            app.makeTempOrange(divObj, 'toolWirelineClick');
    //            func = function () { $('#wLineTBar').toggle('slide', { direction: 'up' }, 500); app.clearTPRelatedObjects();app.showRightPop(); $(app.DE.SplicingDiv).hide(); };
    //            if ($('#wLessTBar').is(":visible")) { $('#wLessTBar').toggle('slide', { direction: 'up' }, 500, func()); }
    //            else if ($('#dvMiscTools').is(":visible")) { $('#dvMiscTools').toggle('slide', { direction: 'up' }, 500, func()); }
    //            else if ($('#dvRadiusTools').is(":visible")) { $('#dvRadiusTools').toggle('slide', { direction: 'up' }, 500, func()); }
    //            else
    //                func();
    //            break;


    //    }
    //}

    this.showWLTB = function (tbType, divObj) {
        var func = function () {
        };
        switch (tbType) {

            case 'Survey':
                //app.makeTempOrange(divObj, 'toolMiscClick');
                $(divObj).toggleClass('activeToolBar');
                func = function () {
                    $('#surveyTBar').toggle('slide', {
                        direction: 'up'
                    }, 500);
                };
                if ($('#wLineTBar').is(":visible")) {
                    $('#wLineTBar').toggle('slide', { direction: 'up' }, 500, func());
                    $('.clsWireLine').removeClass('activeToolBar');
                }
                else
                    func();

                //Set width auto..
                $('#surveyTBar').css('width', 'auto !important');
                break;
            case 'WireLine':
                $(divObj).toggleClass('activeToolBar');
                //app.makeTempOrange(divObj, 'toolWirelineClick');
                func = function () {
                    $('#wLineTBar').toggle('slide', {
                        direction: 'up'
                    }, 500);
                };
                if ($('#surveyTBar').is(":visible")) {
                    $('#surveyTBar').toggle('slide', { direction: 'up' }, 500, func());
                    $('.clsSurvey').removeClass('activeToolBar');
                }
                else
                    func();
                break;
        }
    }

    this.makeTempOrange = function (obj, className) {
        $(obj).addClass(className);
        window.setTimeout(function () {
            $(obj).removeClass(className);
        }, 500);
    }
    this.showSplitCable = function (obj) {
        app.clearTPRelatedObjects();
        if ($(obj).is(':checked')) {
            var cableId = $(obj).attr('s_id');
            var dpathHtml = $('#ispCable_' + cableId).html();
            $('#ispCable_' + cableId).html(app.parseSVG(dpathHtml.replace('tempcss', 'pathAnimation')));
        }
    }
    this.setParentTitle = function (obj) {
        $(obj).parent().attr('title', $(obj).attr('data-networkid'));
    }
    this.resetTitle = function (obj) {
        $(obj).parent().attr('title', $(obj).parent().attr('data-title'));
    }
    /*Splicing*/
    this.spliceHere = function (dvObj) {
         
        $('#buffersvgContainer').html('');
        $("#divISPView .rightInfo").unbind('click');
        if ($(dvObj).hasClass('Splicing') == true) {
            app.clearTPRelatedObjects();
            app.showRightPop();
            app.bindCableRightPop();
            $(dvObj).removeClass('activeToolBar');
            $(app.DE.SplicingDiv).hide();
        }
        else {

            $(dvObj).addClass('Splicing');
            $('.StructureInfo').on("click", function (e) {
                app.initiateSplicing(e);
                $('#pointervgContainer').html('');
                //$('path').attr('stroke-width','1.5');
                app.clearLineAnimation();
                var x = (e.pageX - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                var y = (e.pageY - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
                var svgHtml = app.parseSVG('<svg style="opacity: 0.5;position: absolute; z-index: 99999; width: ' + $('.StructureInfo').width() + 'px' + '; height: ' + $('.StructureInfo').height() + 'px' + '; max-width: ' + $('.StructureInfo').width() + 'px' + '; max-height: ' + $('.StructureInfo').height() + 'px' + ';" pointer-events="none" version="1.1" xmlns="http://www.w3.org/1999/xhtml"><circle id="cirlcePointer" class="marker" r=' + app.nearByentityDistanceBuffer + ' cx=' + x + ' cy=' + y + '></circle></svg>');
                $('#buffersvgContainer').html(svgHtml);
            });
            $('.StructureInfo').css({
                'cursor': 'url(' + baseUrl + appRoot + 'Content/images/hand.cur' + '), default'
            });
            $(dvObj).addClass('activeToolBar');
        }
    }



    this.initiateSplicing = function (e) {
        var titleText = 'Splicing';
        var dataURL = 'Splicing/Index';
        ajaxReq('Splicing/Index', {
            latitude: 0, longitude: 0, bufferRadius: 0
        }, true, function (resp) {
            $(app.DE.splicingMain).html(resp);
            $(app.DE.SplicingDiv).show();
            app.getNearByEntity(e);
        }, false, true);
    }
    this.cancelSplice = function () {
        $(app.DE.SplicingDiv).hide();
        $('.spliceTool').removeClass('Splicing');
        app.showRightPop();
        $('#buffersvgContainer').html('');
        app.destinationCableId = null;
        app.sourceCableId = null;
        $('#pointervgContainer').html('');
        app.clearLineAnimation();
        app.bindCableRightPop();
    }
    this.clearTPRelatedObjects = function () {
        $('.spliceTool').removeClass('Splicing');
        $('.StructureInfo').unbind('click');
        $('.StructureInfo').css({
            'cursor': 'url(), default'
        });
        $('#buffersvgContainer').html('');
        app.destinationCableId = null;
        app.sourceCableId = null;
        $('#pointervgContainer').html('');
        app.clearLineAnimation();
        app.bindCableRightPop();
    }
    this.getNearByEntity = function (e) {
        var lineLatLongValue = [];
        var cableStartEndPoint = [];
        app.cableNearPoint = [];
        app.entityNearPoint = [];
        var pointLatLongValue = [];
        var isVirtual = false;
        var x = (e.pageX - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
        var y = (e.pageY - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
        $('#svgContainer').children().each(function () {
            var cableId = $(this).attr('id');
            var path = $('#' + cableId + ' path').attr('d');
            var networkId = $(this).attr('networkid');
            var cablePathWithId = {
                id: cableId, Path: path, NetworkId: networkId
            };
            var system_id = $(this).attr('data-system-id');
            var total_core = $(this).attr('data-total-core');
            var networkStatus = $(this).attr('data-network-status');
            //var cablePoints=[];
            var cablePoints = path.split('M');
            //if(cableAllPoints.length>0)
            //{
            //    cablePoints.push(cableAllPoints[1]);
            //    cablePoints.push(cableAllPoints[cableAllPoints.length-1]);
            //}
            //OLD CODE
            //$.each(cablePoints, function (index, itemChild) {
            //    var latLong = null;
            //    if (index > 0) {
            //        var moveTo = itemChild.split('L')[0];
            //        var lineTo = itemChild.split('L')[1];
            //        var lineToX = $.trim(lineTo).split(" ")[0];
            //        var lineToY = $.trim(lineTo).split(" ")[1];
            //        var distanceX = lineToX - x;
            //        var distanceY = lineToY - y;
            //        var cabledistance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
            //        latLong = {
            //            id: cableId, moveTo: 'M ' + moveTo.split(' ')[1] + ' ' + moveTo.split(' ')[2], lineTo: 'L ' + lineTo.split(' ')[1] + ' ' + lineTo.split(' ')[2], isVirtual: isVirtual, markerKey: index, distance: cabledistance, NetworkId: networkId, systemId: system_id, totalCore: total_core, networkStatus: networkStatus, isCableStartPoint: (index == 1 ? true : false)
            //        };
            //        lineLatLongValue.push(latLong);
            //        if (index == 1 || cablePoints.length - 1 == index) {
            //            cableStartEndPoint.push(latLong);
            //        }
            //    }
            //});
            cablePoints.splice(0, 1);
            if (cablePoints.length > 0)
            {
                var startPoint = cablePoints[0];              
                var moveTo = startPoint.split('L')[0];
                var lineTo = startPoint.split('L')[1];
                var lineToX = $.trim(lineTo).split(" ")[0];
                var lineToY = $.trim(lineTo).split(" ")[1];
                var distanceX = lineToX - x;
                var distanceY = lineToY - y;
                var cabledistance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
                latLong = {
                    id: cableId, moveTo: 'M ' + moveTo.split(' ')[1] + ' ' + moveTo.split(' ')[2], lineTo: 'L ' + lineTo.split(' ')[1] + ' ' + lineTo.split(' ')[2], isVirtual: isVirtual, markerKey: 1, distance: cabledistance, NetworkId: networkId, systemId: system_id, totalCore: total_core, networkStatus: networkStatus, isCableStartPoint: true
                };
                lineLatLongValue.push(latLong);
              
                endPoint = cablePoints[cablePoints.length - 1];
                moveTo = endPoint.split('L')[0];
                lineTo = endPoint.split('L')[1];
                lineToX = $.trim(lineTo).split(" ")[0];
                lineToY = $.trim(lineTo).split(" ")[1];
                distanceX = lineToX - x;
                distanceY = lineToY - y;
                cabledistance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
                latLong = {
                    id: cableId, moveTo: 'M ' + moveTo.split(' ')[1] + ' ' + moveTo.split(' ')[2], lineTo: 'L ' + lineTo.split(' ')[1] + ' ' + lineTo.split(' ')[2], isVirtual: isVirtual, markerKey: cablePoints[cablePoints.length - 1], distance: cabledistance, NetworkId: networkId, systemId: system_id, totalCore: total_core, networkStatus: networkStatus, isCableStartPoint: false
                };
                lineLatLongValue.push(latLong);
            }

            var result = lineLatLongValue.reduce(function (res, obj) {
                return (obj.distance < res.distance) ? obj : res;
            });

            app.cableNearPoint.push(result);
            lineLatLongValue = [];
        });

        $.each(app.cableNearPoint, function (index, item) {
            if (item.distance < app.nearByentityDistanceBuffer) {
                $('#ddlSourceCable').append('<option value=' + item.systemId + ' data-geom-type="Line" data-entity-type="Cale" data-cable-direction="Source" data-is-start-point="' + (item.isCableStartPoint == true ? true : false) + '">' + item.NetworkId + '-(' + item.totalCore + 'F)' + '-(' + item.networkStatus + ')' + (item.isCableStartPoint == true ? '-(A End)' : '-(B End)') + '</option>');
                $('#ddlDestinationCable').append('<option value=' + item.systemId + ' data-geom-type="Line" data-entity-type="Cale" data-cable-direction="Destination" data-is-start-point="' + (item.isCableStartPoint == true ? true : false) + '">' + item.NetworkId + '-(' + item.totalCore + 'F)' + '-(' + item.networkStatus + ')' + (item.isCableStartPoint == true ? '-(A End)' : '-(B End)') + '</option>');
            }
        });
        $('#ddlSourceCable,#ddlDestinationCable').trigger("chosen:updated");
        $.each(isp2.SplicerEntity, function (index, item) {
            $('.StructureInfo span[data-entitytype=' + item.entityType + ']').each(function () {
                var cabledistance = 0;
                var networkId = $(this).attr('data-networkid');
                var systemId = $(this).attr('data-systemid');
                var entityType = $(this).attr('data-entitytype');
                var ports = $(this).attr('data-ports');
                var networkStatus = $(this).attr('data-network-status');

                var BRX = ($(this).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left")))) + $(this).width();
                var BRY = ($(this).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top")))) + $(this).height();
                var distanceBRX = BRX - x;
                var distanceBRY = BRY - y;
                var distanceBR = Math.sqrt(distanceBRX * distanceBRX + distanceBRY * distanceBRY);


                var TRX = ($(this).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left")))) + $(this).width();
                var TRY = ($(this).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
                var distanceTRX = TRX - x;
                var distanceTRY = TRY - y;
                var distanceTR = Math.sqrt(distanceTRX * distanceTRX + distanceTRY * distanceTRY);


                var BLX = ($(this).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                var BLY = ($(this).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top")))) + $(this).height();
                var distanceBLX = BLX - x;
                var distanceBLY = BLY - y;
                var distanceBL = Math.sqrt(distanceBLX * distanceBLX + distanceBLY * distanceBLY);


                var TLX = ($(this).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                var TLY = ($(this).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
                var distanceTLX = TLX - x;
                var distanceTLY = TLY - y;
                var distanceTL = Math.sqrt(distanceTLX * distanceTLX + distanceTLY * distanceTLY);


                var MTX = ($(this).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left")))) + ($(this).width() / 2);
                var MTY = ($(this).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
                var distanceMTX = MTX - x;
                var distanceMTY = MTY - y;
                var distanceMT = Math.sqrt(distanceMTX * distanceMTX + distanceMTY * distanceMTY);


                var MBX = ($(this).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left")))) + ($(this).width() / 2);
                var MBY = ($(this).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top")))) + $(this).height();
                var distanceMBX = MBX - x;
                var distanceMBY = MBY - y;
                var distanceMB = Math.sqrt(distanceMBX * distanceMBX + distanceMBY * distanceMBY);





                if (distanceBR < app.nearByentityDistanceBuffer) {
                    latLong = {
                        systemId: systemId, distance: distanceBR, NetworkId: networkId, totalPort: (ports == '' ? '' : '-(' + ports + ')'), entityType: entityType, networkStatus: (networkStatus == '' ? '' : '-(' + networkStatus + ')')
                    };
                    pointLatLongValue.push(latLong);
                    var result = pointLatLongValue.reduce(function (res, obj) {
                        return (obj.distance < res.distance) ? obj : res;
                    });
                    app.entityNearPoint.push(result);
                    pointLatLongValue = [];
                } else if (distanceTR < app.nearByentityDistanceBuffer) {
                    latLong = {
                        systemId: systemId, distance: distanceTR, NetworkId: networkId, totalPort: (ports == '' ? '' : '-(' + ports + ')'), entityType: entityType, networkStatus: (networkStatus == '' ? '' : '-(' + networkStatus + ')')
                    };
                    pointLatLongValue.push(latLong);
                    var result = pointLatLongValue.reduce(function (res, obj) {
                        return (obj.distance < res.distance) ? obj : res;
                    });
                    app.entityNearPoint.push(result);
                    pointLatLongValue = [];
                } else if (distanceBL < app.nearByentityDistanceBuffer) {
                    latLong = {
                        systemId: systemId, distance: distanceBL, NetworkId: networkId, totalPort: (ports == '' ? '' : '-(' + ports + ')'), entityType: entityType, networkStatus: (networkStatus == '' ? '' : '-(' + networkStatus + ')')
                    };
                    pointLatLongValue.push(latLong);
                    var result = pointLatLongValue.reduce(function (res, obj) {
                        return (obj.distance < res.distance) ? obj : res;
                    });
                    app.entityNearPoint.push(result);
                    pointLatLongValue = [];

                } else if (distanceTL < app.nearByentityDistanceBuffer) {
                    latLong = {
                        systemId: systemId, distance: distanceTL, NetworkId: networkId, totalPort: (ports == '' ? '' : '-(' + ports + ')'), entityType: entityType, networkStatus: (networkStatus == '' ? '' : '-(' + networkStatus + ')')
                    };
                    pointLatLongValue.push(latLong);
                    var result = pointLatLongValue.reduce(function (res, obj) {
                        return (obj.distance < res.distance) ? obj : res;
                    });
                    app.entityNearPoint.push(result);
                    pointLatLongValue = [];
                } else if (distanceMT < app.nearByentityDistanceBuffer) {
                    latLong = {
                        systemId: systemId, distance: distanceMT, NetworkId: networkId, totalPort: (ports == '' ? '' : '-(' + ports + ')'), entityType: entityType, networkStatus: (networkStatus == '' ? '' : '-(' + networkStatus + ')')
                    };
                    pointLatLongValue.push(latLong);
                    var result = pointLatLongValue.reduce(function (res, obj) {
                        return (obj.distance < res.distance) ? obj : res;
                    });
                    app.entityNearPoint.push(result);
                    pointLatLongValue = [];

                } else if (distanceMB < app.nearByentityDistanceBuffer) {
                    latLong = {
                        systemId: systemId, distance: distanceMB, NetworkId: networkId, totalPort: (ports == '' ? '' : '-(' + ports + ')'), entityType: entityType, networkStatus: (networkStatus == '' ? '' : '-(' + networkStatus + ')')
                    };
                    pointLatLongValue.push(latLong);
                    var result = pointLatLongValue.reduce(function (res, obj) {
                        return (obj.distance < res.distance) ? obj : res;
                    });
                    app.entityNearPoint.push(result);
                    pointLatLongValue = [];
                }
            })
        });
        $.each(app.entityNearPoint, function (index, item) {
            $('#ddlConnectingEntity').append('<option value=' + item.systemId + ' data-geom-type="Point" data-entity-type=' + item.entityType + ' data-total-port=' + item.totalPort.replace('(', '').replace(')', '') + ' data-network-id=' + item.NetworkId + '>' + item.NetworkId + item.totalPort + item.networkStatus + '</option>');
            if (item.entityType == 'ONT') {
                $('#ddlCPEEntity').append('<option value=' + item.systemId + ' data-geom-type="Point" data-entity-type=' + item.entityType + ' data-total-port=' + item.totalPort.replace('(', '').replace(')', '') + '>' + item.NetworkId + item.totalPort + item.networkStatus + '</option>');
                $.each($('#ONT_' + item.systemId).parent().find('.CUSTOMER'), function (inex, item) {
                    var attr = $(this).data();
                    $('#ddlCustomer').append('<option value=' + attr.systemid + ' data-geom-type="Point" data-entity-type=' + attr.entitytype + ' data-total-port="1">' + attr.networkid + '(' + attr.entityname + ')' + '</option>');
                })
            }
        });
        $('#ddlConnectingEntity').trigger("chosen:updated");
        $('#ddlCPEEntity').trigger("chosen:updated");
        $('#ddlCustomer').trigger("chosen:updated");
    }
    this.splicingWindow = function () {

        if ($(app.DE.ConnectingEntity).val() == '0') {
            app.showMessageOverlay();
            alert('Please select connecting element!');
            return false;
        } else if ($(app.DE.SourceCable).val() == '0' && $(app.DE.DestinationCable).val() == '0') {
            app.showMessageOverlay();
            alert('Please select source or destination cable!');
            return false;
        } else if ($(app.DE.ConnectingEntity + ' :selected').attr('data-total-port') == '' || parseInt($(app.DE.ConnectingEntity + ' :selected').attr('data-total-port')) == 0) {
            app.showMessageOverlay();
            alert('No port exist for selected equipment!');
            return false;
        }
        var dataURL = 'Splicing/CableToCable';
        var titleText = 'Splicing';
        var leftStartPoint = JSON.parse($(app.DE.SourceCable + ' :selected').val() != '0' ? $(app.DE.SourceCable + ' :selected').attr('data-is-start-point').toLowerCase() : 'false');
        var rightStartPoint = JSON.parse($(app.DE.DestinationCable + ' :selected').val() != '0' ? $(app.DE.DestinationCable + ' :selected').attr('data-is-start-point').toLowerCase() : 'false');
        var dataURL = 'Splicing/CableToCable';
        var titleText = 'Cable to Cable';
        var objSplicingInput = {
            source_system_id: parseInt($(app.DE.SourceCable).val()),
            source_entity_type: 'Cable',
            destination_system_id: parseInt($(app.DE.DestinationCable).val()),
            destination_entity_type: 'Cable',
            connector_system_id: parseInt($(app.DE.ConnectingEntity).val()),
            connector_entity_type: $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type'),
            is_source_start_point: (leftStartPoint ? leftStartPoint : false),
            is_destination_start_point: (rightStartPoint ? rightStartPoint : false),
            splicing_type: 'CABLE2CABLE'
        };
        popup.LoadModalDialog(app.ParentModel, dataURL, objSplicingInput, titleText, 'modal-xxl', function () {
            $('#dvProgressSplice').show();
            setTimeout(osp.InitilizeJsPlumb, 300); osp.isAutoDeleteConnection = false;
            app.destinationCableId = null;
            app.sourceCableId = null;
            if ($('#ddlSourceCable').val() != '0') {
                isp.focusOnItem('#ddlSourceCable');
            }
            else if ($('#ddlDestinationCable').val() != '0') {
                isp.focusOnItem('#ddlDestinationCable');
            }
        });

    }
    this.focusOnItem = function (ddlId) {
        if ($(app.DE.DestinationCable).val() != '0' && $(app.DE.SourceCable).val() == $(app.DE.DestinationCable).val()) {
            $(app.DE.DestinationCable).val('0').trigger("chosen:updated");
            alert('Please select diffirent cable!');
            return false;
        }

        var systemID = $(ddlId).val();
        var geomType = $(ddlId + ' :selected').attr('data-geom-type');
        var entityType = $(ddlId + ' :selected').attr('data-entity-type');
        var cableDirection = $(ddlId + ' :selected').attr('data-cable-direction');
        if (cableDirection == 'Source' && systemID != '0') {
            app.clearLineAnimation();
            var cableId = 'ispCable_' + systemID;
            app.sourceCableId = cableId;
            if (app.destinationCableId != null) {
                var dpathHtml = $('#' + app.destinationCableId).html();
                $('#' + app.destinationCableId).html(app.parseSVG(dpathHtml.replace('tempcss', 'pathAnimation')));
            }
            var spathHtml = $('#' + cableId).html();
            $('#' + cableId).html(app.parseSVG(spathHtml.replace('tempcss', 'pathAnimation')));

        }
        var connectorEtype = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        if (connectorEtype == 'ODF' || connectorEtype == 'FMS') {
            $(splicing.DE.lblRightCable).text('Cable');
            $(splicing.DE.divLeftCable).hide();
        }
        else {
            $(splicing.DE.divLeftCable).show();
            $(splicing.DE.lblRightCable).text('Right Cable');
        }
        if (cableDirection == 'Destination' && systemID != '0') {
            app.clearLineAnimation();
            if (app.sourceCableId != null) {
                var spathHtml = $('#' + app.sourceCableId).html();
                $('#' + app.sourceCableId).html(app.parseSVG(spathHtml.replace('tempcss', 'pathAnimation')));
            }
            var cableId = 'ispCable_' + systemID;
            app.destinationCableId = cableId;

            var dpathHtml = $('#' + cableId).html();
            $('#' + cableId).html(app.parseSVG(dpathHtml.replace('tempcss', 'pathAnimation')));
        }
        if (geomType == 'Point' && systemID != '0') {
            $('#pointervgContainer').html('');
            var left = $('#' + entityType + '_' + systemID).position().left + $('.ISPViewContainer').scrollLeft();
            var top = $('#' + entityType + '_' + systemID).position().top + $('#divISPView').scrollTop();
            var svgHtml = app.parseSVG('<svg height="24" class="bounce" version="1.1" width="24" xmlns="http://www.w3.org/2000/svg" xmlns:cc="http://creativecommons.org/ns#" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" style="width: 24px;position: absolute;z-index: 999;top:' + (top - 15) + 'px;' + 'left:' + left + 'px;' + '"><g transform="translate(0 -1028.4)"><path d="m12 0c-4.4183 2.3685e-15 -8 3.5817-8 8 0 1.421 0.3816 2.75 1.0312 3.906 0.1079 0.192 0.221 0.381 0.3438 0.563l6.625 11.531 6.625-11.531c0.102-0.151 0.19-0.311 0.281-0.469l0.063-0.094c0.649-1.156 1.031-2.485 1.031-3.906 0-4.4183-3.582-8-8-8zm0 4c2.209 0 4 1.7909 4 4 0 2.209-1.791 4-4 4-2.2091 0-4-1.791-4-4 0-2.2091 1.7909-4 4-4z" fill="#e74c3c" transform="translate(0 1028.4)"></path><path d="m12 3c-2.7614 0-5 2.2386-5 5 0 2.761 2.2386 5 5 5 2.761 0 5-2.239 5-5 0-2.7614-2.239-5-5-5zm0 2c1.657 0 3 1.3431 3 3s-1.343 3-3 3-3-1.3431-3-3 1.343-3 3-3z" fill="#c0392b" transform="translate(0 1028.4)"></path></g></svg>');
            $('#pointervgContainer').html(svgHtml);
        } else if (geomType == 'Point' && systemID == '0') {
            $('#pointervgContainer').html('');
        }
        if (cableDirection == 'Source' && systemID == '0') {
            $(app.DE.DestinationCable + ' option').attr('disabled', false).trigger("chosen:updated");
            var spathHtml = $('#' + app.sourceCableId).html();
            $('#' + app.sourceCableId).html(app.parseSVG(spathHtml.replace('pathAnimation', 'tempcss')));
            return false;
        }
        else if (cableDirection == 'Destination' && systemID == '0') {
            $(app.DE.SourceCable + ' option').attr('disabled', false).trigger("chosen:updated");
            var dpathHtml = $('#' + app.destinationCableId).html();
            $('#' + app.destinationCableId).html(app.parseSVG(dpathHtml.replace('pathAnimation', 'tempcss')));
            return false;
        }
        if (cableDirection == 'Source') {
            $(app.DE.DestinationCable + ' option').attr('disabled', false).trigger("chosen:updated");
            var destOpt = app.DE.DestinationCable + ' option[value="' + $(app.DE.SourceCable).val() + '"]';
            $(destOpt).attr('disabled', true).trigger("chosen:updated");
        } else if (cableDirection == 'Destination') {
            $(app.DE.SourceCable + ' option').attr('disabled', false).trigger("chosen:updated");
            var sourceOpt = app.DE.SourceCable + ' option[value="' + $(app.DE.DestinationCable).val() + '"]';
            $(sourceOpt).attr('disabled', true).trigger("chosen:updated");
        }
        if (systemID == '0') return false;
    }
    this.ClearSplicingRelatedObjs = function () {
        var sourceId = $(app.DE.SourceCable).val();
        var destinationId = $(app.DE.DestinationCable).val();
        if (sourceId != '0')
        {
            var cableId = 'ispCable_' + sourceId;
            var spathHtml = $('#' + cableId).html();
            $('#' + cableId).html(app.parseSVG(spathHtml.replace('pathAnimation', 'tempcss')));
        }
        if (destinationId != '0') {
            var cableId = 'ispCable_' + destinationId;
            var spathHtml = $('#' + cableId).html();
            $('#' + cableId).html(app.parseSVG(spathHtml.replace('pathAnimation', 'tempcss')));
        }                      
        $('#pointervgContainer').html('');
        $(app.DE.DestinationCable).val('0').trigger("chosen:updated");
        $(app.DE.SourceCable).val('0').trigger("chosen:updated");
        $(app.DE.ConnectingEntity).val('0').trigger("chosen:updated");
        $(app.DE.ddlcustomer).val('0').trigger("chosen:updated");
        $(app.DE.ddlCPEEntity).val('0').trigger("chosen:updated");
    }
    this.toggleConnectionUpload = function () {
        var pageUrl = 'Splicing/UploadConnection';
        var modalClass = 'modal-sm';
        popup.LoadModalDialog('PARENT', pageUrl, {}, 'Bulk Splicing', modalClass);

    }

    this.clearLineAnimation = function () {
        $('#svgContainer').children().each(function () {
            var pathHtml = $(this).html();
            $(this).html(app.parseSVG(pathHtml.replace('pathAnimation', 'tempcss')));
        });

    }
    this.animateSplitCable = function (obj) {
        app.clearLineAnimation();
        var cableId = $(obj).attr('s_id');
        var spathHtml = $('#ispCable_' + cableId).html();
        $('#ispCable_' + cableId).html(app.parseSVG(spathHtml.replace('tempcss', 'pathAnimation')));
    }
    this.createOspCable = function () {
        var ospEntity = [];
        ospEntity.push('BDB');
        ospEntity.push('FDB');
        ospEntity.push('FMS');
        ospEntity.push('ONT');
        var ospISPCable = [];
        $.each(ospEntity, function (index, item) {
            $.each($('.StructureInfo span[data-entitytype="' + item + '"]'), function (indx, itm) {
                var cableId = $(this).attr("data-osp-cable-id");
                var systemId = $(this).attr('data-systemid');

                var moveX = ($('#ospCableEndPoint').offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                var moveY = ($('#ospCableEndPoint').offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top")))) + 30;


                var lineX = $(this).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))) + 5;
                var lineY = $(this).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top")));


                var midX = (parseFloat(lineX) + moveX) / 2;
                var midY = (parseFloat(lineY) + moveY) / 2;

                var path = 'M ' + moveX + ' ' + moveY + ' L ' + moveX + ' ' + moveY + ' M ' + moveX + ' ' + moveY + ' L ' + midX + ' ' + midY + ' M ' + midX + ' ' + midY + ' L ' + lineX + ' ' + lineY;
                ospISPCable.push({
                    line_geom: path, StructureId: $("#StructureId").val(), system_id: parseInt(systemId), entity_type: item,
                    startX: moveX,
                    startY: moveY,
                    midX: midX,
                    midY: midY,
                    endX: lineX,
                    endY: lineY
                });
                //var svgHtml=app.parseSVG('<svg style="position: absolute;z-index: 99;max-width: 1150px;width:1150px;height:1095px;max-height:1095px;"   pointer-events="none"  version="1.1" xmlns="http://www.w3.org/1999/xhtml" > <path d="'+path+'"  pointer-events="visibleStroke" version="1.1" xmlns="http://www.w3.org/1999/xhtml"  fill="none" stroke="#7d12d0" stroke-width="1.5"></path> </svg>');    
                //$('#svgContainer').append(svgHtml); 
                //app.setSVGWIdthHeight();


            });
        });
        ajaxReq('ISP/createOSPISPCable', {
            ospISPCables: ospISPCable
        }, false, function (response) {
            app.refreshStructureInfo();
        }, true, false);
    }
    this.bindCableRightPop = function () {
        var CableList = $('#svgContainer').children();
        $(CableList).each(function () {
            var cableId = $(this).attr('id');
            var systemId = $(this).attr('data-system-id');
            var a_system_id = $(this).attr('data-a-system-id');
            var a_entity_type = $(this).attr('data-a-entity-type');
            $('#' + cableId + ' path').unbind('click').on("click", function (e) {
                showCableRightPop(a_entity_type + '_' + a_system_id, systemId, e);
            });
        });
    }
    this.clearEditMode = function () {
        $('#svgContainer').children().each(function () {
            var cableID = $(this).attr('id');
            $('#' + cableID + ' path').attr('stroke-width', 3);
            $('#' + cableID + ' path').attr('stroke', '#0000FF');
        });
        $('#divMarker').html('');
    }
    this.refreshCable = function () {
        var CableList = $('#svgContainer').children();
        var ISPCableMaster = [];
        var cableDetails = [];
        $(CableList).each(function () {
            app.refreshlineLatLong = [];
            var cableId = $(this).attr('id');
            var systemId = $(this).attr('data-system-id');
            var path = $('#' + cableId + ' path').attr('d');
            var a_system_id = $(this).attr('data-a-system-id');
            var a_entity_type = $(this).attr('data-a-entity-type');
            var b_system_id = $(this).attr('data-b-system-id');
            var b_entity_type = $(this).attr('data-b-entity-type');
            if ($('#' + b_entity_type + '_' + b_system_id).length > 0) {

                //Snap the end point of the cable
                $.each(path.split('M'), function (index, item) {
                    if (index > 0) {
                        var moveTo = item.split('L')[0];
                        var lineTo = item.split('L')[1];
                        var latLong = {
                            moveTo: 'M ' + moveTo.split(' ')[1] + ' ' + moveTo.split(' ')[2], lineTo: 'L ' + lineTo.split(' ')[1] + ' ' + lineTo.split(' ')[2], isVirtual: true, markerKey: index
                        };
                        app.refreshlineLatLong.push(latLong);
                    }
                });
                var x = ($('#' + b_entity_type + '_' + b_system_id).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                var y = ($('#' + b_entity_type + '_' + b_system_id).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
                var distanceX = x - parseInt(app.refreshlineLatLong[app.refreshlineLatLong.length - 1].lineTo.split(' ')[1]);
                var distanceY = y - parseInt(app.refreshlineLatLong[app.refreshlineLatLong.length - 1].lineTo.split(' ')[2]);
                var distance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
                console.log('distance : ' + distance);
                if (distance > 20) {
                    app.refreshlineLatLong[app.refreshlineLatLong.length - 1].lineTo = 'L ' + (x + 10 + (app.EndPointSnappedCables.length * 4)) + ' ' + (y);
                    var dPath = '';
                    $.each(app.refreshlineLatLong, function (indx, item) {
                        dPath += item.moveTo + ' ' + item.lineTo;
                    });

                    ISPCableMaster.push({
                        entity_id: systemId, line_geom: dPath, structure_id: $('#StructureId').val()
                    });
                    cableDetails.push({
                        cableId: cableId, path: dPath, systemId: systemId
                    });
                    app.EndPointSnappedCables.push({
                        b_entity_type: b_entity_type, b_system_id: b_system_id, a_entity_type: a_entity_type, a_system_id: a_system_id
                    });
                }
            }
        });

        if (ISPCableMaster.length > 0) {
            ajaxReq('ISP/updateIspCableGeom', {
                cableDetails: ISPCableMaster
            }, false, function (response) {
                $.each(cableDetails, function (index, item) {
                    $('#' + item.cableId + ' path').attr('d', item.path);
                });
            }, true, false);
        }

        var ISPCableMaster = [];
        var cableDetails = [];
        $(CableList).each(function () {
            app.refreshlineLatLong = [];
            var cableId = $(this).attr('id');
            var systemId = $(this).attr('data-system-id');
            var path = $('#' + cableId + ' path').attr('d');
            var a_system_id = $(this).attr('data-a-system-id');
            var a_entity_type = $(this).attr('data-a-entity-type');
            var b_system_id = $(this).attr('data-b-system-id');
            var b_entity_type = $(this).attr('data-b-entity-type');
            var cableType = $(this).attr('data-cable-type');
            if ($('#' + a_entity_type + '_' + a_system_id).length > 0 || cableType == 'OSP') {

                //Snap the Start Point of the cable
                $.each(path.split('M'), function (index, item) {
                    if (index > 0) {
                        var moveTo = item.split('L')[0];
                        var lineTo = item.split('L')[1];
                        var latLong = {
                            moveTo: 'M ' + moveTo.split(' ')[1] + ' ' + moveTo.split(' ')[2], lineTo: 'L ' + lineTo.split(' ')[1] + ' ' + lineTo.split(' ')[2], isVirtual: true, markerKey: index
                        };
                        app.refreshlineLatLong.push(latLong);
                    }
                });
                var x = 0;
                var y = 0;
                if (cableType == 'OSP') {
                    x = ($('#ospCableEndPoint').offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                    y = ($('#ospCableEndPoint').offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
                } else {
                    x = ($('#' + a_entity_type + '_' + a_system_id).offset().left - ($('.StructureInfo').offset().left + parseInt($(".StructureInfo").css("padding-left"))));
                    y = ($('#' + a_entity_type + '_' + a_system_id).offset().top - ($('.StructureInfo').offset().top + parseInt($(".StructureInfo").css("padding-top"))));
                }


                var distanceX = x - parseInt(app.refreshlineLatLong[app.refreshlineLatLong.length - 1].lineTo.split(' ')[1]);
                var distanceY = y - parseInt(app.refreshlineLatLong[app.refreshlineLatLong.length - 1].lineTo.split(' ')[2]);
                var distance = Math.sqrt(distanceX * distanceX + distanceY * distanceY);
                if (distance > 20) {
                    app.refreshlineLatLong[0].moveTo = 'M ' + (x + 10 + (app.EndPointSnappedCables.length * 4)) + ' ' + y;
                    app.refreshlineLatLong[0].lineTo = 'L ' + (x + 10 + (app.EndPointSnappedCables.length * 4)) + ' ' + y;
                    app.refreshlineLatLong[1].moveTo = 'M ' + (x + 10 + (app.EndPointSnappedCables.length * 4)) + ' ' + y;
                    var dPath = '';
                    $.each(app.refreshlineLatLong, function (indx, item) {
                        dPath += item.moveTo + ' ' + item.lineTo;
                    });

                    ISPCableMaster.push({
                        entity_id: systemId, line_geom: dPath, structure_id: $('#StructureId').val()
                    });
                    cableDetails.push({
                        cableId: cableId, path: dPath, systemId: systemId
                    });
                    app.EndPointSnappedCables.push({
                        b_entity_type: b_entity_type, b_system_id: b_system_id, a_entity_type: a_entity_type, a_system_id: a_system_id
                    });
                }
            }
        });

        if (ISPCableMaster.length > 0) {
            ajaxReq('ISP/updateIspCableGeom', {
                cableDetails: ISPCableMaster
            }, false, function (response) {
                $.each(cableDetails, function (index, item) {
                    $('#' + item.cableId + ' path').attr('d', item.path);
                });
            }, true, false);
        }
    }
    // ## Get the Reference details
    this.getEntityReference = function (_entityId, _entityType) {
        if ($("#dvRefrence").html().trim() == '') {
            ajaxReq('Library/GetReferenceEntity', {
                entityId: _entityId, entityType: _entityType
            }, true, function (resp) {
                $("#dvRefrence").html(resp);
                $("#dvRefrence").css('background-image', 'none');
            }, false, false);
        }
    }

    // ##End Get Reference details

}





