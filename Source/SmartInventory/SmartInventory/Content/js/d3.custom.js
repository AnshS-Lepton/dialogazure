var customD3 = function () {
    var app = this;
    this.tempStartDot = null;
    this.tempEndDot = null;
    this.tempPathData = undefined;
    this.firstNLastPointGap = 10;// in pixels

    this.DE = {

        "tempPath": "#tempPath",
        "elementNodes": ".structureInfo .elementDotBox .dot",
        "elements": ".structureInfo .elementDotBox",
        "mainContainer": "#svgCable",
        "svgCable": "#svgCable",
        "actualMarkers": "#svgCable .actualMarker",
        "virtualMarkers": "#svgCable .virtualMarker",
        "EntitySelected": 'entitySelected',
        'svgPatchCable': '#svgPatchCable'
    }
    this.InitD3 = function () {
        app.InitializeTempPath();
        // add drag event to all element dots..
        d3.selectAll(app.DE.elementNodes).call(app.dragCable);

    }

    this.InitializeTempPath = function () {
        let pathData = $(app.DE.tempPath).data();
        if (pathData) {
            pathData.aSystemId = "";
            pathData.aEntityType = "";
            pathData.aLocation = "";
            pathData.aNodeType = "";
            pathData.bSystemId = "";
            pathData.bEntityType = "";
            pathData.bLocation = "";
            pathData.bNodeType = "";
        }
        app.tempPathData = [
        { x: 0, y: 0, isVirtual: false, isMidPoint: false, isEditAllowed: true },
        { x: 0, y: 0, isVirtual: false, isMidPoint: false, isEditAllowed: false },
        { x: 0, y: 0, isVirtual: false, isMidPoint: false, isEditAllowed: false },
        { x: 0, y: 0, isVirtual: false, isMidPoint: false, isEditAllowed: true }
        ];
    }


    this.drawTempPath = function () {
        var linePath = app.tempPathData.filter(function (x) {
            return x.isVirtual == false;
        });
        d3.select(app.DE.tempPath)
          .attr("d", app.lineFunction(linePath));

    }
    this.drawTempPathForEdit = function () {
        var linePath = app.tempPathData.filter(function (x) {
            return x.isVirtual == false;
        });
        d3.select(app.DE.tempPath)
          .attr("d", app.lineFunction(linePath));
        app.drawActualMarkers();
    }
    this.selectTerminationPoints = function () {

        let pathData = $(app.DE.tempPath).data();
        if (app.tempStartDot != null) {
            var startElementData = app.tempStartDot.parent().data();

            $('#ddlCableAEnd option[data-system-id="' + startElementData.systemId + '"][data-entity-type="' + startElementData.entityType + '"]').prop('selected', true);
            $('#ddlDuctAEnd option[data-system-id="' + startElementData.systemId + '"][data-entity-type="' + startElementData.entityType + '"]').prop('selected', true);
            pathData.aSystemId = startElementData.systemId;
            pathData.aEntityType = startElementData.entityType;
            pathData.aLocation = startElementData.networkId;
            pathData.aNodeType = app.tempStartDot.attr('data-node-type');

        }
        else {
            $('#ddlDuctAEnd').val(0);
            $('#ddlCableAEnd').val(0);
            pathData.aSystemId = "";
            pathData.aEntityType = "";
            pathData.aLocation = "";
            pathData.aNodeType = "";
            d3.select(app.DE.tempPath).attr("d", "");
        }

        if (app.tempEndDot != null) {
            var endElementData = app.tempEndDot.parent().data();
            var objOption = $('#ddlCableBEnd option[data-system-id="' + endElementData.systemId + '"][data-entity-type="' + endElementData.entityType + '"]').prop('selected', true);
            var objOption1 = $('#ddlDuctBEnd option[data-system-id="' + endElementData.systemId + '"][data-entity-type="' + endElementData.entityType + '"]').prop('selected', true);
            pathData.bSystemId = endElementData.systemId;
            pathData.bEntityType = endElementData.entityType;
            pathData.bLocation = endElementData.networkId;
            pathData.bNodeType = app.tempEndDot.attr('data-node-type');

        }
        else {
            $('#ddlCableBEnd').val(0);
            $('#ddlDuctBEnd').val(0);
            pathData.bSystemId = "";
            pathData.bEntityType = "";
            pathData.bLocation = "";
            pathData.bNodeType = "";
            d3.select(app.DE.tempPath).attr("d", "");
        }
        $('#ddlCableAEnd').trigger("chosen:updated");
        $('#ddlCableBEnd').trigger("chosen:updated");
        $('#ddlDuctAEnd').trigger("chosen:updated");
        $('#ddlDuctBEnd').trigger("chosen:updated");

        app.focusTempPath();
        setTimeout(app.isDiffEndPoints, 300);
    }
    this.removeTempPath = function () {
        app.tempEndDot = null;
        app.tempStartDot = null;

        app.InitializeTempPath();
        app.drawTempPath();
        app.removeActualMarkers();

    }
    this.lineFunction = d3.line().x(function (d) { return d.x; })
                                .y(function (d) { return d.y; })
                                .curve(d3.curveStepBefore);


    this.dragCable = d3.drag()
         .on("start", function () {
             $('.entityInfo').tooltip('disable');
             if (!$(app.DE.tempPath).data().isEditMode) {
                 app.removeTempPath();
                 app.manageTempEndPoints($(d3.select(this).node()), true);
                 app.tempStartDot = $(d3.select(this).node());
                 app.selectTerminationPoints();
             }
         })
         .on('drag', function () {
             //d3.select(this).attr("cx", d.x = d3.event.x).attr("cy", d.y = d3.event.y);
             $('.entityInfo').tooltip('disable');
             if (!$(app.DE.tempPath).data().isEditMode) {
                 //MOUSE POSITION FROM DRAG POINT
                 var pos = d3.mouse(this);
                 app.tempPathData[app.tempPathData.length - 1].x = app.tempPathData[0].x + pos[0];
                 app.tempPathData[app.tempPathData.length - 1].y = app.tempPathData[0].y + pos[1];
                 app.tempPathData[app.tempPathData.length - 2].x = app.tempPathData[0].x + pos[0];
                 app.tempPathData[app.tempPathData.length - 2].y = app.tempPathData[0].y + pos[1];
                 app.addMidPointToTempPath();
                 app.drawTempPath();
             }
         })
         .on("end", function () {
             app.resetFocus();
             if (!$(app.DE.tempPath).data().isEditMode) {
                 var contextPos = {
                     x: app.getOffsetToParent(app.tempStartDot, $(app.DE.mainContainer), "left") - ((app.tempStartDot.width() + 2)),
                     y: app.getOffsetToParent(app.tempStartDot, $(app.DE.mainContainer), "top") - ((app.tempStartDot.height() + 2))
                 }
                 let offsetX = 0, offsetY = 0;
                 if (Math.abs(d3.event.x) > 20) {
                     offsetX = 20;
                 }
                 if (d3.event.y > 50) {
                     offsetY = 8;
                 }
                 //if (d3.event.y < 0 && Math.abs(event.y) > 50)
                 //{
                 //    offsetY = -20;
                 //}
                 let $node = app.getNearestDot($(d3.select(d3.event.sourceEvent.target).node()), { x: contextPos.x + d3.event.x - offsetX, y: contextPos.y + d3.event.y - offsetY });
                 if (!$node || !$node.length)
                     $node = $(d3.select(d3.event.sourceEvent.target).node());

                 if ($node.hasClass("dot")) {
                     app.manageTempEndPoints($node, false);
                     app.addMidPointToTempPath();
                     app.drawTempPath();
                     app.drawActualMarkers();
                     app.tempEndDot = $node;
                     app.selectTerminationPoints();
                 }
                 else {
                     app.removeTempPath();
                     app.tempEndDot = null;
                     app.tempStartDot = null;
                     app.selectTerminationPoints();
                 }
             }

             $('.entityInfo').tooltip('enable');
         });

    this.addMidPointToTempPath = function () {

        //prev point value..
        var prevValue = app.tempPathData[1];
        //Next point value(in drag mode its mouse pointer position)
        var CurVal = app.tempPathData[app.tempPathData.length - 2];

        // prepare object to top and bottom mid point..
        var midPoint1 = { x: prevValue.x, y: CurVal.y, isVirtual: false, isMidPoint: true, isEditAllowed: true };
        var midPoint2 = { x: CurVal.x, y: prevValue.y, isVirtual: false, isMidPoint: true, isEditAllowed: true };

        //decide which one is top mid point and add the same into array..
        var midPoint = midPoint1.y < midPoint2.y ? midPoint1 : midPoint2;
        var index = app.tempPathData.findIndex(x=>x.isMidPoint == true);
        if (index > -1) {
            app.tempPathData[index] = midPoint;
        }
        else {
            app.tempPathData.splice(2, 0, midPoint);
        }
    }

    this.manageTempEndPoints = function (objNode, isStartPoint) {

        if (isStartPoint) {
            app.tempPathData[0].x = app.getOffsetToParent(objNode, $(app.DE.mainContainer), "left");
            app.tempPathData[0].y = app.getOffsetToParent(objNode, $(app.DE.mainContainer), "top");

            //add 2nd point value
            if (objNode.hasClass("top")) {
                app.tempPathData[1].x = app.tempPathData[0].x;
                app.tempPathData[1].y = app.tempPathData[0].y - app.firstNLastPointGap;
            }
            else if (objNode.hasClass("bottom")) {
                app.tempPathData[1].x = app.tempPathData[0].x;
                app.tempPathData[1].y = app.tempPathData[0].y + app.firstNLastPointGap;
            }
            else if (objNode.hasClass("left")) {

                app.tempPathData[1].x = app.tempPathData[0].x - app.firstNLastPointGap;
                app.tempPathData[1].y = app.tempPathData[0].y;
            }
            else if (objNode.hasClass("right")) {

                app.tempPathData[1].x = app.tempPathData[0].x + app.firstNLastPointGap;
                app.tempPathData[1].y = app.tempPathData[0].y;
            }

        }
        else {
            app.tempPathData[app.tempPathData.length - 1].x = app.getOffsetToParent(objNode, $(app.DE.mainContainer), "left");
            app.tempPathData[app.tempPathData.length - 1].y = app.getOffsetToParent(objNode, $(app.DE.mainContainer), "top");

            //add 2nd point value
            if (objNode.hasClass("top")) {
                app.tempPathData[app.tempPathData.length - 2].x = app.tempPathData[app.tempPathData.length - 1].x;
                app.tempPathData[app.tempPathData.length - 2].y = app.tempPathData[app.tempPathData.length - 1].y - app.firstNLastPointGap;
            }
            else if (objNode.hasClass("bottom")) {
                app.tempPathData[app.tempPathData.length - 2].x = app.tempPathData[app.tempPathData.length - 1].x;
                app.tempPathData[app.tempPathData.length - 2].y = app.tempPathData[app.tempPathData.length - 1].y + app.firstNLastPointGap;
            }
            else if (objNode.hasClass("left")) {

                app.tempPathData[app.tempPathData.length - 2].x = app.tempPathData[app.tempPathData.length - 1].x - app.firstNLastPointGap;
                app.tempPathData[app.tempPathData.length - 2].y = app.tempPathData[app.tempPathData.length - 1].y;
            }
            else if (objNode.hasClass("right")) {
                app.tempPathData[app.tempPathData.length - 2].x = app.tempPathData[app.tempPathData.length - 1].x + app.firstNLastPointGap;
                app.tempPathData[app.tempPathData.length - 2].y = app.tempPathData[app.tempPathData.length - 1].y;
            }
        }
    }

    this.setEndPoints = function (objNode, index) {
        let endNode = "";

        if (objNode.hasClass("top")) {
            endNode = "top";
        }
        else if (objNode.hasClass("bottom")) {
            endNode = "bottom";
        }
        else if (objNode.hasClass("left")) {
            endNode = "left";
        }
        else if (objNode.hasClass("right")) {
            endNode = "right";
        }

        app.tempPathData[index].endPoint = endNode
    }
    this.getOffsetToParent = function (objElement, objParent, offSetType) {

        //THIS FUNCTION RETURN OFFSET TO SPECIFIC PARENT ELEMENT.
        //IF NO PARENT THEN IT WILL RETURN OFFSET TO BODY/HTML.

        var offSet = 0;
        if (objElement && objElement.length) {
            if (offSetType.toUpperCase() == "LEFT") {
                if (objParent && objParent.length) {
                    offSet = (objElement.offset().left - objParent.offset().left) + ((objElement.width() + 2) / 2);
                }
                else {
                    offSet = objElement.offset().left + ((objElement.width() + 2) / 2);
                }
            }
            else if (offSetType.toUpperCase() == "TOP") {
                if (objParent && objParent.length) {
                    offSet = (objElement.offset().top - objParent.offset().top) + ((objElement.height() + 2) / 2);
                }
                else {
                    offSet = objElement.offset().top + ((objElement.height() + 2) / 2);
                }
            }
        }
        return offSet;
    }

    this.drawActualMarkers = function () {

        app.tempPathData = app.getPointArrayFromSvgPath($(app.DE.tempPath).attr('d'), $(app.DE.tempPath).data().cableType);
        app.renderMarkers()
        app.drawTempPath();
    }
    this.removeActualMarkers = function () {
        d3.select(app.DE.svgCable).selectAll(app.DE.actualMarkers).remove();
    }
    this.dragStartActualMarker = function (d) {
        d3.select(this).attr('pointer-events', 'none');
        $('.entityInfo').tooltip('disable');
        //app.addMarker();
    }
    this.onDragActualMarker = function (d) {
        $('.entityInfo').tooltip('disable');
        //update dragged marker position..
        var objCurCircle = d3.select(this);
        var curIndex = parseInt(objCurCircle.attr("circle-index"));
        app.tempPathData[curIndex].y = d3.event.y;
        app.tempPathData[curIndex].x = d3.event.x;
        d3.select(this).attr("transform", "translate(" + d3.event.x + "," + d3.event.y + ")");
        app.drawTempPath();
    }
    this.dragEndActualMarker = function (d) {

        var curIndex = parseInt(d3.select(this).attr("circle-index"));
        // check whether dragged point is First or Last or not...
        if (curIndex == 0 || curIndex == app.tempPathData.length - 1) {

            let $node = app.getNearestDot($(d3.select(d3.event.sourceEvent.target).node()), { x: d3.event.x, y: d3.event.y });
            if (!$node || !$node.length)
                $node = $(d3.select(d3.event.sourceEvent.target).node());

            if ($node.hasClass("dot")) {
                if ($(app.DE.tempPath).data().cableType == "OSP") {

                    let _cableData = $("#ispCable_" + $(app.DE.tempPath).data().systemId).data();
                    //check wheather a end exists in current structure...
                    let _isAEnd = $("#div_" + _cableData.aEntityType + "_" + _cableData.aSystemId).length > 0 ? true : false;
                    app.UpdateCableEndPointDetail($node, _isAEnd);
                }
                else {
                    let _isAEnd = curIndex == 0 ? true : false;
                    app.UpdateCableEndPointDetail($node, _isAEnd);
                }
                //update end point position..
                app.manageTempEndPoints($node, curIndex == 0 ? true : false);
                //app.addMidPointToTempPath();
                app.drawTempPath();
                app.drawActualMarkers();

            }
            else {

                if ($(app.DE.tempPath).data().cableType == "OSP") {
                    let _cableData = $("#ispCable_" + $(app.DE.tempPath).data().systemId).data();
                    //check wheather a end exists in current structure...
                    let _isAEnd = $("#div_" + _cableData.aEntityType + "_" + _cableData.aSystemId).length > 0 ? true : false;
                    app.UpdateCableEndPointDetail(null, _isAEnd);
                }
                else {
                    let _isAEnd = curIndex == 0 ? true : false;
                    app.UpdateCableEndPointDetail(null, _isAEnd);
                }
            }
            if (!$(app.DE.tempPath).data().isEditMode) {
                app.selectTerminationPoints();
            }
        }
        app.drawActualMarkers();
        d3.select(this).attr('pointer-events', 'visible');
        $('.entityInfo').tooltip('enable');
    }
    this.removeMarker = function (e) {

        if (app.tempPathData.length <= 4) {
            alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_047, 'warning');//Node can not be deleted. Minimum four nodes are required to draw a cable.
        }
        else {
            let len = app.tempPathData.length;
            //app.drawActualMarkers();
            let index = parseInt(d3.select(this).attr("circle-index"));
            if (index > 0 && index < (app.tempPathData.length - 1)) {

                app.tempPathData.splice(index, 1);
                app.renderMarkers();
                app.drawTempPath();
            }
        }
        d3.event.preventDefault();
        d3.event.stopPropagation();
        return false;
    }
    this.renderMarkers = function () {
        d3.select(app.DE.svgCable).selectAll(app.DE.actualMarkers).remove();
        d3.select(app.DE.svgCable).selectAll(app.DE.actualMarkers)
            .data(app.tempPathData)
            .enter().filter(function (d) { return d.isEditAllowed == true && d.isVirtual == false; })
            .append("circle")
            .attr("r", 4)
            .attr("pointer-events", "visible")
            .attr("class", function (d, i) { return app.tempPathData.indexOf(d) == 0 || app.tempPathData.indexOf(d) == app.tempPathData.length - 1 ? "actualMarker EndPointMarker" : "actualMarker"; })
            .attr("transform", function (d) { return "translate(" + d.x + ',' + d.y + ")"; })
            .attr("circle-index", function (d, i) { return app.tempPathData.indexOf(d); })
            .call(d3.drag()
            .on("start", app.dragStartActualMarker)
            .on("drag", app.onDragActualMarker)
            .on("end", app.dragEndActualMarker))
            .on("contextmenu", app.removeMarker);
    }
    this.UpdateCableEndPointDetail = function (objDot, isAEnd) {

        let tempData = $(app.DE.tempPath).data();
        if (isAEnd) {
            if (objDot != null && objDot != undefined) {
                app.tempStartDot = objDot;
                //for edit mode

                var startElementData = app.tempStartDot.parent().data();
                if (startElementData) {
                    tempData.aSystemId = startElementData.systemId;
                    tempData.aEntityType = startElementData.entityType;
                    tempData.aLocation = startElementData.networkId;
                    tempData.aNodeType = app.tempStartDot.attr('data-node-type');
                }
            }
            else {
                app.tempStartDot = null;
                tempData.aSystemId = "";
                tempData.aEntityType = "";
                tempData.aLocation = "";
                tempData.aNodeType = "";
            }
        }
        else {

            if (objDot != null && objDot != undefined) {
                app.tempEndDot = objDot;
                let endData = $(app.DE.tempPath).data();
                //for edit mode
                var endElementData = app.tempEndDot.parent().data();
                if (endElementData) {
                    tempData.bSystemId = endElementData.systemId;
                    tempData.bEntityType = endElementData.entityType;
                    tempData.bLocation = endElementData.networkId;
                    tempData.bNodeType = app.tempEndDot.attr('data-node-type');
                }
            }
            else {
                app.tempEndDot = null;
                tempData.bSystemId = "";
                tempData.bEntityType = "";
                tempData.bLocation = "";
                tempData.bNodeType = "";
            }
        }

        app.focusTempPath();
        setTimeout(app.isDiffEndPoints, 300);
    }

    this.drawDuct = function () {

        if (!$(app.DE.tempPath).data().isEditMode) {

            var _aEndVal, _bEndVal, _aEndData, _BEndData, _objAEndElement,
                _objBEndElement, _aEndX, _aEndY, _bEndX, _bEndY, _elementWidth, _elementHeight, _aEndDotType, _bEndDotType;

            //GET SELECTED TERMINATION POINT..
            _aEndVal = $('#ddlDuctAEnd').val();
            _bEndVal = $('#ddlDuctBEnd').val();
            _elementWidth = $(".Element").first().width();
            _elementHeight = $(".Element").first().height();


            if (_aEndVal > 0) {
                _aEndData = $('#ddlDuctAEnd option:selected').data();

                //FIND ELEMENT IN SHAFT AND GET POSITION RESPECTIVE TO MAIN CONTAINER..
                _objAEndElement = $('.entityInfo[data-system-id="' + _aEndData.systemId + '"][data-entity-type="' + _aEndData.entityType + '"]').first();
                if (_objAEndElement.length > 0) {
                    _aEndX = app.getOffsetToParent(_objAEndElement, $(app.DE.mainContainer), "left");
                    _aEndY = app.getOffsetToParent(_objAEndElement, $(app.DE.mainContainer), "top");

                }
            }

            if (_bEndVal > 0) {
                _bEndData = $('#ddlDuctBEnd option:selected').data();

                //FIND ELEMENT IN SHAFT AND GET POSITION RESPECTIVE TO MAIN CONTAINER..
                _objBEndElement = $('.entityInfo[data-system-id="' + _bEndData.systemId + '"][data-entity-type="' + _bEndData.entityType + '"]').first();
                if (_objBEndElement.length > 0) {
                    _bEndX = app.getOffsetToParent(_objBEndElement, $(app.DE.mainContainer), "left");
                    _bEndY = app.getOffsetToParent(_objBEndElement, $(app.DE.mainContainer), "top");

                }
            }

            if (_objAEndElement && _objBEndElement) {

                // decide A and B End nodes..

                if ((_aEndY - _bEndY) > (_elementHeight / 2)) // B element is on top..
                {
                    _aEndDotType = "top";

                    if ((_aEndX - _bEndX) > (_elementWidth / 2))// B element is on left side..
                    {
                        _bEndDotType = "right";
                    }
                    else if ((_aEndX - _bEndX) < -(_elementWidth / 2))// B element is on right side..
                    {
                        _bEndDotType = "left";
                    }
                    else //B element is just at top of  A Element
                    {
                        _bEndDotType = "bottom";
                    }
                }
                else if ((_aEndY - _bEndY) < -(_elementHeight / 2)) // A element is on top..
                {
                    _bEndDotType = "top";

                    if ((_bEndX - _aEndX) > (_elementWidth / 2))// A element is on left side..
                    {
                        _aEndDotType = "right";
                    }
                    else if ((_bEndX - _aEndX) < -(_elementWidth / 2))// A element is on right side..
                    {
                        _aEndDotType = "left";
                    }
                    else //B element is just at top of  A Element
                    {
                        _aEndDotType = "bottom";
                    }

                }
                else {
                    if (_bEndX > _aEndX) {
                        _bEndDotType = "left";
                        _aEndDotType = "right";
                    }
                    else {
                        _bEndDotType = "right";
                        _aEndDotType = "left";
                    }
                }

                // clean existing path..
                app.removeTempPath();

                // add start node position...
                var _objStartDot = _objAEndElement.find('.' + _aEndDotType).first();
                app.manageTempEndPoints(_objStartDot, true);
                // update start node type... 
                app.UpdateCableEndPointDetail(_objStartDot, true); //use same function for duct as well to update end points  Nikhil

                // add end node positions..
                var _objEndDot = _objBEndElement.find('.' + _bEndDotType).first();
                app.manageTempEndPoints(_objEndDot, false);
                // update end node type...
                app.UpdateCableEndPointDetail(_objEndDot, false); //use same function for duct as well to update end points  Nikhil
                app.addMidPointToTempPath();
                app.drawTempPath();
                app.drawActualMarkers();
                app.focusTempPath();

            }
            else {
                app.removeTempPath();
            }
        }
    }

    this.drawCable = function () {

        if (!$(app.DE.tempPath).data().isEditMode) {

            var _aEndVal, _bEndVal, _aEndData, _BEndData, _objAEndElement,
                _objBEndElement, _aEndX, _aEndY, _bEndX, _bEndY, _elementWidth, _elementHeight, _aEndDotType, _bEndDotType;

            //GET SELECTED TERMINATION POINT..
            _aEndVal = $('#ddlCableAEnd').val();
            _bEndVal = $('#ddlCableBEnd').val();
            _elementWidth = $(".Element").first().width();
            _elementHeight = $(".Element").first().height();


            if (_aEndVal > 0) {
                _aEndData = $('#ddlCableAEnd option:selected').data();

                //FIND ELEMENT IN SHAFT AND GET POSITION RESPECTIVE TO MAIN CONTAINER..
                _objAEndElement = $('.entityInfo[data-system-id="' + _aEndData.systemId + '"][data-entity-type="' + _aEndData.entityType + '"]').first();
                if (_objAEndElement.length > 0) {
                    _aEndX = app.getOffsetToParent(_objAEndElement, $(app.DE.mainContainer), "left");
                    _aEndY = app.getOffsetToParent(_objAEndElement, $(app.DE.mainContainer), "top");

                }
            }

            if (_bEndVal > 0) {
                _bEndData = $('#ddlCableBEnd option:selected').data();

                //FIND ELEMENT IN SHAFT AND GET POSITION RESPECTIVE TO MAIN CONTAINER..
                _objBEndElement = $('.entityInfo[data-system-id="' + _bEndData.systemId + '"][data-entity-type="' + _bEndData.entityType + '"]').first();
                if (_objBEndElement.length > 0) {
                    _bEndX = app.getOffsetToParent(_objBEndElement, $(app.DE.mainContainer), "left");
                    _bEndY = app.getOffsetToParent(_objBEndElement, $(app.DE.mainContainer), "top");

                }
            }

            if (_objAEndElement && _objBEndElement) {

                // decide A and B End nodes..

                if ((_aEndY - _bEndY) > (_elementHeight / 2)) // B element is on top..
                {
                    _aEndDotType = "top";

                    if ((_aEndX - _bEndX) > (_elementWidth / 2))// B element is on left side..
                    {
                        _bEndDotType = "right";
                    }
                    else if ((_aEndX - _bEndX) < -(_elementWidth / 2))// B element is on right side..
                    {
                        _bEndDotType = "left";
                    }
                    else //B element is just at top of  A Element
                    {
                        _bEndDotType = "bottom";
                    }
                }
                else if ((_aEndY - _bEndY) < -(_elementHeight / 2)) // A element is on top..
                {
                    _bEndDotType = "top";

                    if ((_bEndX - _aEndX) > (_elementWidth / 2))// A element is on left side..
                    {
                        _aEndDotType = "right";
                    }
                    else if ((_bEndX - _aEndX) < -(_elementWidth / 2))// A element is on right side..
                    {
                        _aEndDotType = "left";
                    }
                    else //B element is just at top of  A Element
                    {
                        _aEndDotType = "bottom";
                    }

                }
                else {
                    if (_bEndX > _aEndX) {
                        _bEndDotType = "left";
                        _aEndDotType = "right";
                    }
                    else {
                        _bEndDotType = "right";
                        _aEndDotType = "left";
                    }
                }

                // clean existing path..
                app.removeTempPath();

                // add start node position...
                var _objStartDot = _objAEndElement.find('.' + _aEndDotType).first();
                app.manageTempEndPoints(_objStartDot, true);
                // update start node type...
                app.UpdateCableEndPointDetail(_objStartDot, true);

                // add end node positions..
                var _objEndDot = _objBEndElement.find('.' + _bEndDotType).first();
                app.manageTempEndPoints(_objEndDot, false);
                // update end node type...
                app.UpdateCableEndPointDetail(_objEndDot, false);
                app.addMidPointToTempPath();
                app.drawTempPath();
                app.drawActualMarkers();
                app.focusTempPath();

            }
            else {
                app.removeTempPath();
            }
        }
    }

    this.getOSPCablePath = function (systemId, entityType, isEndPoint) {


        var _arrPath = [];

        // get termination point bottom dot position..

        var _objElementDot = $("#div_" + entityType + "_" + systemId).find('.bottom').first();

        var _elementDotX = app.getOffsetToParent(_objElementDot, $(app.DE.mainContainer), "left");
        var _elementDotY = app.getOffsetToParent(_objElementDot, $(app.DE.mainContainer), "top");


        //get OSP Cable icon center positon...
        var _ospCableX = app.getOffsetToParent($("#divOSPCable"), $(app.DE.mainContainer), "left");
        var _ospCableY = app.getOffsetToParent($("#divOSPCable"), $(app.DE.mainContainer), "top");

        var _ospCableY = $('path[data-cable-type="OSP"][d!=""]').length > 5 ? _ospCableY : ((_ospCableY - 8) + $('path[data-cable-type="OSP"][d!=""]').length * 4);
        console.log('Y:-- ' + _ospCableY);

        // Note: Element point should be in last of array.. 
        if (isEndPoint) {
            _arrPath.push({ x: _ospCableX, y: _ospCableY });
            _arrPath.push({ x: _elementDotX, y: _ospCableY });
            _arrPath.push({ x: _elementDotX, y: _elementDotY + 10 });
            _arrPath.push({ x: _elementDotX, y: _elementDotY });
        }
        else {
            _arrPath.push({ x: _elementDotX, y: _elementDotY });
            _arrPath.push({ x: _elementDotX, y: _elementDotY + 10 });
            _arrPath.push({ x: _elementDotX, y: _ospCableY });
            _arrPath.push({ x: _ospCableX, y: _ospCableY });
        }

        //    _arrPath.reverse();
        //return path..
        return app.lineFunction(_arrPath);
    }
    this.getISPCablePath = function ($nodeA, $nodeB) {
        let result = {
            positionA: {},
            positionB: {},
            path: '',
            pathArray:[]
        };
        let _arrPath = [];

        if ($nodeA && $nodeA.length && $nodeB && $nodeB.length) {
            let $mainContext = $(app.DE.mainContainer);
            let positionA = {
                x: app.getOffsetToParent($nodeA, $mainContext, "left"),
                y: app.getOffsetToParent($nodeA, $mainContext, "top"),
                dot: 'bottom',
                height: $nodeA.height(),
                width: $nodeA.width(),
                marginX: 0,
                marginY: 0,
            };
            let positionB = {
                x: app.getOffsetToParent($nodeB, $mainContext, "left"),
                y: app.getOffsetToParent($nodeB, $mainContext, "top"),
                dot: 'bottom',
                height: $nodeB.height(),
                width: $nodeB.width(),
                marginX: 0,
                marginY: 0,
            };

            if ((positionA.y - positionB.y) > (positionA.height / 2)) {
                positionA.dot = "top";
                positionA.marginY = -app.firstNLastPointGap;
                if ((positionA.x - positionB.x) > (positionA.width / 2)) {
                    positionB.dot = "right";
                    positionB.marginX = app.firstNLastPointGap;
                }
                else if ((positionA.x - positionB.x) < -(positionA.width / 2)) {
                    positionB.dot = "left";
                    positionB.marginX = -app.firstNLastPointGap;
                }
                else {
                    positionB.dot = "bottom";
                    positionB.marginY = app.firstNLastPointGap;
                }
            }
            else if ((positionA.y - positionB.y) < -(positionA.height / 2)) {
                positionB.dot = "top";
                positionB.marginY = -app.firstNLastPointGap;
                if ((positionB.x - positionA.x) > (positionA.width / 2))// A element is on left side..
                {
                    positionA.dot = "right";
                    positionA.marginX = app.firstNLastPointGap;
                }
                else if ((positionB.x - positionA.x) < -(positionA.width / 2))// A element is on right side..
                {
                    positionA.dot = "left";
                    positionA.marginX = -app.firstNLastPointGap;
                }
                else //B element is just at top of  A Element
                {
                    positionA.dot = "bottom";
                    positionA.marginY = app.firstNLastPointGap;
                }

            }
            else {
                if (positionB.x > positionA.x) {
                    positionB.dot = "left";
                    positionB.marginX = -app.firstNLastPointGap;
                    positionA.dot = "right";
                    positionA.marginX = app.firstNLastPointGap;
                }
                else {
                    positionB.dot = "right";
                    positionB.marginX = app.firstNLastPointGap;
                    positionA.dot = "left";
                    positionA.marginX = -app.firstNLastPointGap;
                }
            }


            let pointA = $nodeA.find('.' + positionA.dot).first();
            let pointB = $nodeB.find('.' + positionB.dot).first()
            _arrPath.push({
                x: app.getOffsetToParent(pointA, $mainContext, "left"),
                y: app.getOffsetToParent(pointA, $mainContext, "top")
            });
            _arrPath.push({
                x: app.getOffsetToParent(pointB, $mainContext, "left"),
                y: app.getOffsetToParent(pointB, $mainContext, "top")
            });
            _arrPath.splice(1, 0, {
                x: _arrPath[0].x + positionA.marginX,
                y: _arrPath[0].y + positionA.marginY
            });
            _arrPath.splice(2, 0, {
                x: _arrPath[2].x + positionB.marginX,
                y: _arrPath[2].y + positionB.marginY
            });
            result.positionA = positionA;
            result.positionB = positionB;
            result.path = app.lineFunction(_arrPath);
            result.pathArray = _arrPath;
        }

        return result;
    }

    this.getClosestJointPath = function ($node, point) {
        let result = {
            position: {},
            pathArray: []
        };
        let _arrPath = [];
        if ($node && $node.length) {
            let $mainContext = $(app.DE.mainContainer);
            let $dot = app.getNearestDot($node, point,9999);
            let position = {
                x: app.getOffsetToParent($dot, $mainContext, "left"),
                y: app.getOffsetToParent($dot, $mainContext, "top"),
                height: $dot.height(),
                width: $dot.width(),
                marginX: 0,
                marginY: 0,
                dot: 'bottom'
            };
            if ($dot.hasClass('top')) {
                position.dot = 'top';
                position.marginY = -app.firstNLastPointGap;
            }
            if ($dot.hasClass('right')) {
                position.dot = 'right';
                position.marginX = app.firstNLastPointGap;
            }
            if ($dot.hasClass('left')) {
                position.dot = 'left';
                position.marginX = -app.firstNLastPointGap;
            }
            if ($dot.hasClass('bottom')) {
                position.dot = 'bottom';
                position.marginY = app.firstNLastPointGap;
            }
            _arrPath.push({
                x: position.x,
                y: position.y
            });
            _arrPath.push({
                x: position.x + position.marginX,
                y: position.y + position.marginY
            });
            result.position = position;
            result.pathArray = _arrPath;
        }
        return result;
    }
   
    this.getPointArrayFromSvgPath = function (_path, _cableType) {
        let _finalPointArray = [];
        var tempPoints = [];
        let offset = 10;

        var nodes = _path.split(/(?=[LMC])/);
        if (!nodes || nodes == '' || nodes.length == 0) {
            return [];
        }
        let nodesLen = nodes.length;
        let count = 1;
        //let clipPoints = [];
        //let clipCountX = 0, clipCountY = 0;
        var _isEditAllowed = true; //_cableType.toUpperCase() == "ISP" ;
        tempPoints = app.filterDuplicatePoints(nodes, _isEditAllowed);
        nodesLen = tempPoints.length;

        _finalPointArray = this.filterPathPoints(tempPoints, offset, _isEditAllowed);
        _finalPointArray = app.adjustPathPoints(tempPoints, _finalPointArray, _isEditAllowed);
        return _finalPointArray;
    }
    this.adjustPathPoints = function (tempPoints, _finalPointArray, _isEditAllowed) {
        let nodesLen = tempPoints.length;
        if (nodesLen >= 4) {
            if (app.isEqualPoints(tempPoints[0], _finalPointArray[0], 3))
                _finalPointArray.splice(0, 1, tempPoints[0]);
            else
                _finalPointArray.splice(0, 0, tempPoints[0]);

            if (_finalPointArray[1] && !app.isEqualPoints(tempPoints[1], _finalPointArray[1], 3))
                if (!app.isEqualPoints(tempPoints[0], tempPoints[1], 3))
                    _finalPointArray.splice(1, 0, tempPoints[1]);

            if (_finalPointArray.length - 2 > 0
                && !app.isEqualPoints(tempPoints[nodesLen - 2], _finalPointArray[_finalPointArray.length - 2], 3)
                && !app.isEqualPoints(tempPoints[nodesLen - 2], _finalPointArray[_finalPointArray.length - 1], 3)
                && !app.isEqualPoints(tempPoints[nodesLen - 1], _finalPointArray[_finalPointArray.length - 2], 3))
                if (!app.isEqualPoints(tempPoints[nodesLen - 2], tempPoints[nodesLen - 1], 3))
                    _finalPointArray.splice(_finalPointArray.length - 1, 0, tempPoints[nodesLen - 2]);

            if (app.isEqualPoints(tempPoints[nodesLen - 1], _finalPointArray[_finalPointArray.length - 1], 3))
                _finalPointArray.splice(_finalPointArray.length, 1, tempPoints[nodesLen - 1]);
            else
                _finalPointArray.splice(_finalPointArray.length, 0, tempPoints[nodesLen - 1]);

            if (_finalPointArray.length == 2) {
                _finalPointArray.splice(1, 0, tempPoints[1]);
                _finalPointArray.splice(_finalPointArray.length - 1, 0, tempPoints[nodesLen - 2]);
            }
            if (_finalPointArray.length == 3) {

                if (_finalPointArray[1].x == _finalPointArray[2].x) {
                    _finalPointArray.splice(_finalPointArray.length - 2, 0, { x: tempPoints[1].x, y: tempPoints[1].y - 5, isVirtual: false, isMidPoint: false, isEditAllowed: true });

                }
                else if (_finalPointArray[1].y == _finalPointArray[2].y) {
                    _finalPointArray.splice(_finalPointArray.length - 2, 0, { x: tempPoints[1].x - 5, y: tempPoints[2].y, isVirtual: false, isMidPoint: false, isEditAllowed: true });
                }
                else {
                    _finalPointArray.splice(_finalPointArray.length - 2, 0, { x: tempPoints[1].x, y: tempPoints[2].y, isVirtual: false, isMidPoint: false, isEditAllowed: true });
                }

            }
        }
        else if (nodesLen == 2) {
            _finalPointArray = [];
            _finalPointArray.push(tempPoints[0]);
            if (tempPoints[0].x == tempPoints[1].x) {
                _finalPointArray.push({ x: tempPoints[0].x, y: tempPoints[0].y + 10, isVirtual: false, isMidPoint: false, isEditAllowed: true });
                _finalPointArray.push({ x: tempPoints[1].x, y: tempPoints[1].y - 10, isVirtual: false, isMidPoint: false, isEditAllowed: true });
            }
            else if (tempPoints[0].y == tempPoints[1].y) {
                _finalPointArray.push({ x: tempPoints[0].x + 10, y: tempPoints[0].y, isVirtual: false, isMidPoint: false, isEditAllowed: true });
                _finalPointArray.push({ x: tempPoints[1].x - 10, y: tempPoints[1].y, isVirtual: false, isMidPoint: false, isEditAllowed: true });
            }
            else {
                _finalPointArray.push({ x: tempPoints[0].x, y: tempPoints[1].y, isVirtual: false, isMidPoint: false, isEditAllowed: true });
                _finalPointArray.push({ x: tempPoints[1].x, y: tempPoints[1].y + 10, isVirtual: false, isMidPoint: false, isEditAllowed: true });
            }
            _finalPointArray.push(tempPoints[1]);
        }
        else if (nodesLen == 3) {
            _finalPointArray = [];
            _finalPointArray.push(tempPoints[0]);
            _finalPointArray.push(tempPoints[1]);
            if (tempPoints[1].x == tempPoints[2].x) {
                _finalPointArray.push({ x: tempPoints[1].x, y: tempPoints[1].y + 10, isVirtual: false, isMidPoint: false, isEditAllowed: true });
            }
            else if (tempPoints[1].y == tempPoints[2].y) {
                _finalPointArray.push({ x: tempPoints[1].x + 10, y: tempPoints[1].y, isVirtual: false, isMidPoint: false, isEditAllowed: true });

            }
            else {
                _finalPointArray.push({ x: tempPoints[1].x, y: tempPoints[2].y, isVirtual: false, isMidPoint: false, isEditAllowed: true });
            }
            _finalPointArray.push(tempPoints[2]);
        }
        else _finalPointArray = [];
        return _finalPointArray;
    }
    this.filterPathPoints = function (tempPoints, offset, _isEditAllowed) {
        let nodesLen = tempPoints.length;
        let _finalPointArray = [];
        let clipCountX = 0, clipCountY = 0;
        let count = 0;
        let clipPoints = [];
        for (var i = 0; i < nodesLen; i++) {
            if (!_finalPointArray.filter(p=> ((p.x <= offset + tempPoints[i].x && p.x >= -offset + tempPoints[i].x) && (p.y <= offset + tempPoints[i].y && p.y >= -offset + tempPoints[i].y))).length) {

                _finalPointArray.push({ x: tempPoints[i].x, y: tempPoints[i].y, isVirtual: false, isMidPoint: false, isEditAllowed: i == nodesLen - 1 ? _isEditAllowed : true });

                if (count > 0) {
                    let diffX = Math.abs(_finalPointArray[count - 1].x - _finalPointArray[count].x);
                    let diffY = Math.abs(_finalPointArray[count - 1].y - _finalPointArray[count].y);
                    if (diffX == 0 && diffY > 0) {
                        //console.log("Vertical Edge");
                        if (clipCountX)
                            clipPoints.push(count - 1);
                        clipCountX++;
                        clipCountY = 0;
                    }
                    else if (diffY == 0 && diffX > 0) {
                        //console.log("Horizontal Edge");
                        if (clipCountY)
                            clipPoints.push(count - 1);
                        clipCountY++;
                        clipCountX = 0;
                    }
                    else {
                        clipCountX = 0;
                        clipCountY = 0;
                    }
                }
                count++;

            }
        }


        return _finalPointArray.filter(function (val, index) { return clipPoints.indexOf(index) == -1; });


    }
    this.filterDuplicatePoints = function (nodes, _isEditAllowed) {

        let nodesLen = nodes.length;
        let count = 1;
        let tempPoints = [];

        let point = nodes[0].slice(1, nodes[0].length).split(',');
        tempPoints.push({ x: parseFloat(point[0]), y: parseFloat(point[1]), isVirtual: false, isMidPoint: false, isEditAllowed: _isEditAllowed });
        point = nodes[nodesLen - 1].slice(1, nodes[nodesLen - 1].length).split(',');
        tempPoints.push({ x: parseFloat(point[0]), y: parseFloat(point[1]), isVirtual: false, isMidPoint: false, isEditAllowed: true });
        //Remove Duplicate
        for (let i = 0; i < nodesLen - 1; i++) {
            let point = nodes[i].slice(1, nodes[i].length).split(',');
            point[0] = parseFloat(point[0]);
            point[1] = parseFloat(point[1]);
            if (!tempPoints.filter(p=> p.x == point[0] && p.y == point[1]).length) {
                tempPoints.splice(count++, 0, { x: point[0], y: point[1], isVirtual: false, isMidPoint: false, isEditAllowed: true });

            }
        }
        return tempPoints;
    }
    this.isEqualPoints = function (point1, point2, margin) {
        margin = margin || 0;
        // return point1.x == point2.x && point1.y == point2.y;
        return (point1.x <= margin + point2.x && point1.x >= -margin + point2.x) && (point1.y <= margin + point2.y && point1.y >= -margin + point2.y)
    }
    this.focusEndPoints = function ($start, $end) {
        if ($start && $start.attr("class")) {
            $start.attr("class", $start.attr("class").replace(app.DE.EntitySelected, ''));
            $start.attr("class", $start.attr("class").trim() + " " + app.DE.EntitySelected);
        }
        if ($end && $end.attr("class")) {
            $end.attr("class", $end.attr("class").replace(app.DE.EntitySelected, ''));
            $end.attr("class", $end.attr("class").trim() + " " + app.DE.EntitySelected);
        }
    }
    this.resetFocus = function () {
        $('.entityInfo').each(function (i, e) {
            $(e).attr("class", $(e).attr("class").replace(app.DE.EntitySelected, ''));
        });
    }
    this.focusTempPath = function () {
        app.resetFocus();
        let data = $(app.DE.tempPath).data();
        if (data && data.aEntityType && data.bEntityType && data.aSystemId && data.bSystemId) {
            let $start = $("#div_" + data.aEntityType + "_" + data.aSystemId);
            let $end = $("#div_" + data.bEntityType + "_" + data.bSystemId);
            //if($start && $end)
            app.focusEndPoints($start, $end);
        }
    }
    this.isDiffEndPoints = function () {
        let data = $(app.DE.tempPath).data();
        if (data) {
            if (data.aSystemId && data.bSystemId && data.aSystemId != '' && data.bSystemId != '' && data.aEntityType != '' && data.bEntityType != '' && (data.aSystemId === data.bSystemId && data.aEntityType == data.bEntityType)) {
                app.removeTempPath();
                app.resetFocus();
                $('#ddlCableAEnd').val(0).trigger("chosen:updated");
                $('#ddlCableBEnd').val(0).trigger("chosen:updated");
                $('#ddlDuctAEnd').val(0).trigger("chosen:updated");
                $('#ddlDuctBEnd').val(0).trigger("chosen:updated");
                alert( MultilingualKey.SI_ISP_GBL_JQ_GBL_048, 'warning');//Source and destination can not be same.
                return false;
            }
        }
        return true;
    }
    this.getNearestDot = function ($dotContainer, position,distanceLimit) {


        let $allDot = $dotContainer.find('span.dot');
        let $nearestDot, minDistance = Infinity;
        distanceLimit = distanceLimit || 25;
        $allDot.each(function (i, e) {
            let $e = $(e);
            let p = {
                x: app.getOffsetToParent($e, $(app.DE.mainContainer), "left"),
                y: app.getOffsetToParent($e, $(app.DE.mainContainer), "top")
            }
            let d = Math.sqrt(((position.x - p.x) * (position.x - p.x)) + ((position.y - p.y) * (position.y - p.y)));
            if (d < minDistance) {
                minDistance = d;
                $nearestDot = $e;
            }
        });
        if (distanceLimit >= minDistance)
            return $nearestDot;
        else
            return undefined;
    }
    this.CPEConnections = {
        show: function (arrConnections) {
            $.each(arrConnections, function (i, e) {
                var arrPoints = [];
                var objSourceElement = $("#div_" + arrConnections[i].source_entity_type + "_" + arrConnections[i].source_system_id);
                var objdestElement = $("#div_" + arrConnections[i].destination_entity_type + "_" + arrConnections[i].destination_system_id);

                if (arrConnections[i].destination_entity_type == 'CUSTOMER') {
                    arrPoints.push({ x: app.getOffsetToParent(objSourceElement, $(app.DE.mainContainer), "left"), y: app.getOffsetToParent(objSourceElement, $(app.DE.mainContainer), "top") });
                    arrPoints.push({ x: app.getOffsetToParent(objSourceElement, $(app.DE.mainContainer), "left"), y: app.getOffsetToParent(objSourceElement, $(app.DE.mainContainer), "top") + 20 });
                    arrPoints.push({ x: app.getOffsetToParent(objdestElement, $(app.DE.mainContainer), "left"), y: app.getOffsetToParent(objdestElement, $(app.DE.mainContainer), "top") });
                    d3.select(app.DE.svgPatchCable).append("path")
                        .attr("class", "CPEtoCustomerPath").attr("pointer-events", "visibleStroke")
                      .attr("d", app.CPEConnections.linePath(arrPoints)).attr("style", "fill: none;stroke:#08b514 !important;stroke-width:2px!important;");
                    objdestElement.addClass('customer-connected');
                }
                else {
                    arrPoints.push({ x: app.getOffsetToParent(objSourceElement, $(app.DE.mainContainer), "left"), y: app.getOffsetToParent(objSourceElement, $(app.DE.mainContainer), "top") });
                    arrPoints.push({ x: app.getOffsetToParent(objdestElement, $(app.DE.mainContainer), "left"), y: app.getOffsetToParent(objdestElement, $(app.DE.mainContainer), "top") });
                    d3.select(app.DE.svgPatchCable).append("path")
                        .attr("class", "CPEtoCPEPath").attr("data-entity-type", "PatchCord").attr("pointer-events", "visibleStroke")
                      .attr("d", app.CPEConnections.linePath(arrPoints)).attr("style", "fill: none;stroke:#5D5D5D !important;stroke-width:2px!important;");
                }


            });
        },
        hide: function () {
            $(app.DE.svgPatchCable + ' path[class^="CPE"]').remove();
            $('.element-CUSTOMER').removeClass('customer-connected');
        },
        linePath: d3.line().x(function (d) { return d.x; })
                            .y(function (d) { return d.y; })
                           .curve(d3.curveStepAfter)
    }
}