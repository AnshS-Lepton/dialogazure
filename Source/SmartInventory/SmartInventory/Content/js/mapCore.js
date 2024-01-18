var polyline = undefined;
var LayerFilters = [];
var MarkerList = [];
//PREVENT MAP SERVER HITS WHILE ZOOMING- START 1
var ZoomChangeDelay = 1.25;
var lastZoomNo = 0;
var lastZoomTime = new Date();
var zoomDiffSec = ZoomChangeDelay;
var isZoomChanged = false;

function checkLastZoom() {
    setInterval(function () {
        if (isZoomChanged) {
            zoomDiffSec = ((new Date() - lastZoomTime) / 1000);
            if (zoomDiffSec > ZoomChangeDelay || si.IsVecorLayerEnabled) {
                //console.log("lastZoomNo from interval= " + lastZoomNo + " time " + zoomDiffSec);
                isZoomChanged = false;
                //load layers on map.
                let arrActiveRegionlayers = si.getActiveRegionLayers();
                let arrActiveProvincelayers = si.getActiveProvinceLayers();
                let isValid = (arrActiveRegionlayers.length > 0 || arrActiveProvincelayers.length > 0);
                if (isValid) {
                    si.reqver++;
                    si.LoadLayersOnMap(false);
                    if (si.IsVecorLayerEnabled) {
                        si.fetchVectorDelta();
                        si.ShowHideVectorLayerByZoomSetting();
                    }
                }
            }
        }
    }, 30);
}
//PREVENT MAP SERVER HITS WHILE ZOOMING- END 1

function createOverlayLayer(objlayer, trackURLS, fn) {
    var overlayUrlsArr = overlayUrlsArr || {};
    if (trackURLS) {
        //var overlayUrlsArr = overlayUrlsArr || {};
        overlayUrlsArr[objlayer.DisplayName] = new Object();
        overlayUrlsArr[objlayer.DisplayName].initialDraw = true;
        overlayUrlsArr[objlayer.DisplayName].pendingUrls = new Array();
    }

    LayerFilters.push(objlayer);
    var mapTileWidth = 1024;
    var mapTileheight = 256;
    //removeLayerIFExists(objlayer.DisplayName);
    var newLayer = new google.maps.ImageMapType({
        getTileUrl: function (coord, zoom) {

            //PREVENT MAP SERVER HITS WHILE ZOOMING- START 3
            //if (zoomDiffSec < 2 && lastZoomNo > 0 && isZoomChanged) {
            if (lastZoomNo > 0 && isZoomChanged) {
                //console.log("last return zoom " + si.map.getZoom() + "time ====" + zoomDiffSec);
                return;
            }
            else {
                //lastZooTime = new Date();
                lastZoomTime = new Date();
                lastZoomNo = si.map.getZoom();
                //console.log("last success zoom" + lastZoomNo);
                isZoomChanged = false;
            }
            //PREVENT MAP SERVER HITS WHILE ZOOMING- END 3
           
          
            var proj = si.map.getProjection();
            var zfactor = Math.pow(2, zoom);
            var top = proj.fromPointToLatLng(new google.maps.Point(coord.x * mapTileWidth / zfactor, coord.y * mapTileheight / zfactor));
            var bot = proj.fromPointToLatLng(new google.maps.Point((coord.x + 1) * mapTileWidth / zfactor, (coord.y + 1) * mapTileheight / zfactor));
            var bbox = top.lng() + "," + bot.lat() + "," + bot.lng() + "," + top.lat();
            var currentLayers = '';

            var url = getMapserver();
            url += "MAP=" + objlayer.MapFilePath; //Map File
            url += "&srs=EPSG:4326";     //set WGS84 
            url += "&version=1.0.0";  //WMS version  
            url += "&REQUEST=GetMap"; //WMS operation
            url += "&LAYERS=" + objlayer.Name; //WMS layers  
            url += addFilters(objlayer.Filters);
            url += "&service=wms";    //WMS service         
            url += "&BBOX=" + bbox;      // set bounding box
            url += "&WIDTH=" + mapTileWidth;         //tile size in google hardcode:1024
            url += "&HEIGHT=" + mapTileheight;        //hardcode:256
            url += "&FORMAT=image/png"; //WMS format
            url += "&reqver=" + si.reqver; //WMS format

            if (trackURLS) {
                if (overlayUrlsArr[objlayer.DisplayName].initialDraw)
                    overlayUrlsArr[objlayer.DisplayName].pendingUrls.push(url);
            }

            if ($("#hdnMapUrl")) {
                if (objlayer.Name && objlayer.Name != 'Region,Province') {
                    $("#hdnMapUrl").val('');
                    $("#hdnMapUrl").val(url);
                }
            }
            if (objlayer.Name == '')
                return '';
            //console.log('Layer URL ---> '+url);
            return url;
        },
        tileSize: new google.maps.Size(mapTileWidth, mapTileheight),
        name: objlayer.DisplayName,
        opacity: 1,
        isPng: true
    });
    if (trackURLS) {
        $(newLayer).bind("overlay-idle", function () {
            overlayUrlsArr[objlayer.DisplayName].initialDraw = false;
            window.setTimeout(fn, 5000);
        });

        newLayer.baseGetTile = newLayer.getTile;
        newLayer.getTile = function (tileCoord, zoom, ownerDocument) {
            var node = newLayer.baseGetTile(tileCoord, zoom, ownerDocument);
            if (overlayUrlsArr[objlayer.DisplayName].initialDraw) {
                $("img", node).one("load", function () {
                    var index = $.inArray(this.__src__, overlayUrlsArr[objlayer.DisplayName].pendingUrls);
                    overlayUrlsArr[objlayer.DisplayName].pendingUrls.splice(index, 1);
                    if (overlayUrlsArr[objlayer.DisplayName].pendingUrls.length === 0) {
                        $(newLayer).trigger("overlay-idle");

                    }
                });
            }
            return node;
        };
        overlayUrlsArr[objlayer.DisplayName].initialDraw = false;
        window.setTimeout(fn, 5000);
    }
    return newLayer;
}
function createCovidOverlayLayer(objlayer, trackURLS, fn) {


    if (trackURLS) {
        var overlayUrlsArr = overlayUrlsArr || {};
        overlayUrlsArr[objlayer.DisplayName] = new Object();
        overlayUrlsArr[objlayer.DisplayName].initialDraw = true;
        overlayUrlsArr[objlayer.DisplayName].pendingUrls = new Array();
    }



    LayerFilters.push(objlayer);
    var newLayer = new google.maps.ImageMapType({
        getTileUrl: function (coord, zoom) {
            var proj = si.map.getProjection();
            var zfactor = Math.pow(2, zoom);
            var top = proj.fromPointToLatLng(new google.maps.Point(coord.x * 1024 / zfactor, coord.y * 256 / zfactor));
            var bot = proj.fromPointToLatLng(new google.maps.Point((coord.x + 1) * 1024 / zfactor, (coord.y + 1) * 256 / zfactor));
            var bbox = top.lng() + "," + bot.lat() + "," + bot.lng() + "," + top.lat();
            var currentLayers = '';
            var url = "http://networkaccess.leptonsoftware.com:199/cgi-bin/mapserv.exe?";
            url += "MAP=" + objlayer.MapFilePath; //Map File
            url += "&srs=EPSG:4326";     //set WGS84 
            url += "&version=1.0.0";  //WMS version  
            url += "&REQUEST=GetMap"; //WMS operation   
            url += "&LAYERS=" + objlayer.Name; //WMS layers  
            url += addFilters(objlayer.Filters);
            url += "&service=wms";    //WMS service         
            url += "&BBOX=" + bbox;      // set bounding box
            url += "&WIDTH=1024";         //tile size in google
            url += "&HEIGHT=256";
            url += "&FORMAT=image/png"; //WMS format
            url += "&reqver=" + si.reqver; //WMS format    
            if (trackURLS) {
                if (overlayUrlsArr[objlayer.DisplayName].initialDraw)
                    overlayUrlsArr[objlayer.DisplayName].pendingUrls.push(url);
            }

            if ($("#hdnMapUrl")) {
                if (objlayer.Name && objlayer.Name != 'Region,Province') {
                    $("#hdnMapUrl").val('');
                    $("#hdnMapUrl").val(url);
                }
            }
            if (objlayer.Name == '')
                return '';
            return url;
        },
        tileSize: new google.maps.Size(1024, 256),
        name: objlayer.DisplayName,
        opacity: 1,
        isPng: true
    });
    if (trackURLS) {
        $(newLayer).bind("overlay-idle", function () {
            overlayUrlsArr[objlayer.DisplayName].initialDraw = false;
            window.setTimeout(fn, 5000);
        });

        newLayer.baseGetTile = newLayer.getTile;
        newLayer.getTile = function (tileCoord, zoom, ownerDocument) {
            var node = newLayer.baseGetTile(tileCoord, zoom, ownerDocument);
            if (overlayUrlsArr[objlayer.DisplayName].initialDraw) {
                $("img", node).one("load", function () {
                    var index = $.inArray(this.__src__, overlayUrlsArr[objlayer.DisplayName].pendingUrls);
                    overlayUrlsArr[objlayer.DisplayName].pendingUrls.splice(index, 1);
                    if (overlayUrlsArr[objlayer.DisplayName].pendingUrls.length === 0) {
                        $(newLayer).trigger("overlay-idle");

                    }
                });
            }
            return node;
        };
        overlayUrlsArr[objlayer.DisplayName].initialDraw = false;
        window.setTimeout(fn, 5000);
    }
    return newLayer;
}
function getcurrentZoomEntity() {
    var lyrs = [];
    $.each($(si.DE.ulNetworkLayers + " li .mainLyr:checked"), function () {
        var minZoom = $('.mainlyr li[data-mapabbr="' + $(this).attr('data-mapabbr') + '"] #MinMaxZoom').attr('data-min-zoom');
        var maxZoom = $('.mainlyr li[data-mapabbr="' + $(this).attr('data-mapabbr') + '"] #MinMaxZoom').attr('data-max-zoom');
        var currentZoom = si.map.getZoom();
        if (currentZoom >= parseInt(minZoom) && currentZoom <= parseInt(maxZoom)) {
            lyrs.push($(this).attr('data-mapabbr'));
        }
    });
    return lyrs;
}

function getCurrentActiveLayer(networkStatus, isWithLabel) {
    var lyrs = [];
    var currentZoom = si.map.getZoom();
    //var layers = si.layerDetails.filter(p=>p.networkStatus==networkStatus && p.isWithLabel==isWithLabel && parseInt(p.minZoom)<=currentZoom && parseInt(p.maxZoom)>=currentZoom);
    //IE Solution
    var layers = si.layerDetails.filter(function (p) { return p.networkStatus == networkStatus && p.isWithLabel == isWithLabel && parseInt(p.minZoom) <= currentZoom && parseInt(p.maxZoom) >= currentZoom; });
    if (layers.length > 0) {
        $.each(layers, function (index, item) {
            lyrs.push(item.Name);
        })
    }
    return lyrs;
}

function getZoomEntity(_layers, _displayName, _CurrZoom) {
    if (_displayName != "Region_Province_Layer") {
        var _layer = [];
        var CurrLayerArray = _layers.split(",");
        $.each(CurrLayerArray, function (i) {
            if (CurrLayerArray[i] == 'BLD' || CurrLayerArray[i] == 'BLDP' || CurrLayerArray[i] == 'BLDC') {
                //var srchlayer = si._layerZoomEntity.find(p => p.layerType == 'BLD,BLDP,BLDC');
                //IE Solution
                var srchlayer = si._layerZoomEntity.find(function (p) { return p.layerType == 'BLD,BLDP,BLDC'; });
                if (_CurrZoom >= srchlayer.minZoomlvl && _CurrZoom <= srchlayer.maxZoomlvl) {
                    _layer.push(CurrLayerArray[i]);
                }
            }
            else {
                //var srchlayer = si._layerZoomEntity.find(p => p.layerType == CurrLayerArray[i]);
                //IE Solution
                var srchlayer = si._layerZoomEntity.find(function (p) { return p.layerType == CurrLayerArray[i]; });
                if (srchlayer != undefined && CurrLayerArray[i] == srchlayer.layerType && (_CurrZoom >= srchlayer.minZoomlvl && _CurrZoom <= srchlayer.maxZoomlvl)) {
                    _layer.push(CurrLayerArray[i]);
                }
            }


        });
        return _layer.length > 0 ? _layer.join(',') : '';
    }
    else {
        return _layers;
    }
}

function getMapserver() {
    var mapServer = ((si.mapServer == si.mapServers.length - 1) ? 0 : (si.mapServer + 1));
    return si.mapServers[mapServer];
}
function getLegendUrl(mapFile, layer) {
    return getMapserver() + 'MAP=' + mapFile + '&srs=EPSG:4326&version=1.0.0&REQUEST=GetLegendGraphic&LAYER=' + layer + '&service=wms&FORMAT=image/png';
}
function addFilters(filters) {
    var filterexp = '';
    $(filters).each(function (i) {
        filterexp += '&' + filters[i].Field + '=' + filters[i].value;
    });
    return filterexp;
}

function getCircle(mtrRadius, cntrPt) {
    var _circle = new google.maps.Circle({
        center: cntrPt,
        radius: mtrRadius,
        strokeColor: "#FF8800",
        strokeOpacity: 0.8,
        strokeWeight: 1,
        fillColor: "#FF8800",
        fillOpacity: 0.2
    });
    return _circle;
}

function getLatLongArr(pgString) {
    var latLngArr = [];
    pgString = pgString.substring(pgString.lastIndexOf('(') + 1, pgString.indexOf(')'));
    var longLatArr = pgString.split(',');
    for (var i = 0; i < longLatArr.length; i++) {
        var LongLatsingle = longLatArr[i].split(' ');
        latLngArr.push(new google.maps.LatLng(parseFloat(LongLatsingle[1]), parseFloat(LongLatsingle[0])));
    }
    return latLngArr;
}



// to switch active class from submenu - Deepak//
function switchAcitveMenuFooter(obj) {
     setTimeout(function () { $(obj).addClass("activeToolBar"); }, 100);
}




function initiateDrawingsBulkAsBuilt(obj, shapeFlag) {
  
    if ($(obj).hasClass('ActiveTool'))
    { $(obj).removeClass('ActiveTool') } else { $(obj).addClass('ActiveTool') }
    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    si.removeEventListnrs('click');
    si.gMapObj.shapeType = shapeFlag;
    si.gMapObj.purposeType = "BulkAsBuilt";
    si.gMapObj.shapeArr = [];
    // add or remove active class form footer icon menu
    switchAcitveMenuFooter(obj);



    google.maps.event.addListener(si.map, 'click', handleShapeEvents);
    $('#lengthAreaDiv').html('<div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}


function initiateDrawingsPotential(obj, shapeFlag) {

    if ($(obj).hasClass('ActiveTool'))
    { $(obj).removeClass('ActiveTool') } else { $(obj).addClass('ActiveTool') }
    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    si.removeEventListnrs('click');
    si.gMapObj.shapeType = shapeFlag;
    si.gMapObj.purposeType = "AreaPotential";
    si.gMapObj.shapeArr = [];



    google.maps.event.addListener(si.map, 'click', handleShapeAreaPotentialEvents);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}


function initiateDrawingsPotentialFORBOM(obj, shapeFlag) {

    if ($(obj).hasClass('ActiveTool'))
    { $(obj).removeClass('ActiveTool') } else { $(obj).addClass('ActiveTool') }
    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    si.removeEventListnrs('click');
    si.gMapObj.shapeType = shapeFlag;
    si.gMapObj.purposeType = "AreaPotential";
    si.gMapObj.shapeArr = [];

    google.maps.event.addListener(si.map, 'click', handleShapeAreaPotentialForBOMEvents);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}

function calculateDirection(obj) {
    //remove mesure distance events..
    
    if (si.gMapObj.RulerLine) {
        si.removeEventListnrs('click');
        si.gMapObj.RulerLine.setMap(null);
        si.gMapObj.RulerLine = null;
        $('#lengthAreaDiv').empty();
    }
    
    si.clearMarkers();
    ClearRuler();
    if (directionsDisplay) {
        directionsDisplay.setMap(null);
    }
    // add or remove active class form footer icon menu
    switchAcitveMenuFooter(obj);

    removeOldMarkers();
    alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_070);//Select source by clicking on map.
    $('.clearRout').removeClass('activeToolBar');
    window.setTimeout(function () { hideDiv('alertMsgBox'); }, 1000);
    si.map.setOptions({ draggableCursor: 'crosshair' });
    google.maps.event.addListener(si.map, 'click', function (e) {
        var src = si.createMarker(e.latLng, 'content/images/source.png');
        src.setMap(si.map);
        MarkerList.push(src);
        si.removeEventListnrs('click');
        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_071);//Now select destination by clicking on map.
        window.setTimeout(function () { hideDiv('alertMsgBox'); }, 1000);
        var request = {
            origin: e.latLng,
            travelMode: google.maps.DirectionsTravelMode.DRIVING,
            provideRouteAlternatives: true
        };
        google.maps.event.addListener(si.map, 'click', function (f) {
            si.removeEventListnrs('click');
            si.map.setOptions({ draggableCursor: '' });
            var dest = si.createMarker(f.latLng, 'content/images/destination.png');
            dest.setMap(si.map);
            MarkerList.push(dest);
            request.destination = f.latLng;
            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                    directionsDisplay.setMap(si.map);
                  //  si.sourcePoint = MarkerList[0].position;
                   // si.endPoint = MarkerList[1].position;
                    src.setMap(null);
                    dest.setMap(null);
                }
                else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_016);
                    window.setTimeout(function () {
                        src.setMap(null); dest.setMap(null);
                    }, 1000);
                }
            });
        });
    });
}


function addPolyPoint(e) {


    var vertices = si.gMapObj.RulerLine.getPath();
    vertices.push(e.latLng);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><b>Length :</b> <span>' + showFooterDist(vertices) + '</span></div>');
}

var polyDrLine;
var disLbl;
function toggleRuler(obj) {
    si.RulerFlag = si.RulerFlag || !0;
    ClearRuler();
    if (polyDrLine)
        polyDrLine.setMap(null);
    if (disLbl)
        disLbl.setMap(null);
    if (directionsDisplay) {
        directionsDisplay.setMap(null);
    }
    // add or remove active class form footer icon menu
    switchAcitveMenuFooter(obj);

    si.clearMarkers();
    var RulerPath = [];
    if (si.RulerFlag === !0) {
        si.Rlbl = new google.maps.Label({ visible: !1, map: si.map, position: si.map.getCenter(), text: '' });
        si.map.setOptions({ draggableCursor: 'url(../Content/images/hand.cur),crosshair' });
        si.Ruler = createPolyline(RulerPath);
        si.RulerTemp = createPolyline([]);
        si.RulerTemp.setOptions({ clickable: !1, strokeOpacity: 0.5, strokeColor: '#EF4B41' });
        si.Ruler.setOptions({ clickable: !1, strokeColor: '#EF4B41' });

        google.maps.event.addListener(si.map, 'click', function (e) {
            RulerPath.push(e.latLng);
            si.Ruler.setPath(RulerPath);
        });

        google.maps.event.addListenerOnce(si.map, 'dblclick', function (e) {
            si.RulerFlag = !1;
            si.RulerTemp.setMap(null);
            ClearMapListners();
            var rPath = si.Ruler.getPath().getArray();
            var distance = google.maps.geometry.spherical.computeLength(rPath);
            si.Rlbl.set('text', formatLength(distance));
        });

        google.maps.event.addListener(si.map, 'mousemove', function (e) {

            if (si.RulerFlag === !0 && si.Ruler) {
                var rPath = si.Ruler.getPath().getArray();
                if (rPath.length > 0) {
                    var TempPath = [rPath[rPath.length - 1], e.latLng];
                    si.RulerTemp = si.RulerTemp || createPolyline([]);
                    si.RulerTemp.setPath(TempPath);
                    var distance = google.maps.geometry.spherical.computeLength(rPath) + google.maps.geometry.spherical.computeLength(TempPath);
                    si.Rlbl.set('visible', !0);
                    si.Rlbl.set('text', formatLength(distance));
                    si.Rlbl.set('position', e.latLng);
                }
            }
        });
    }
  
}
function ClearRuler() {
    if (si.RulerTemp) si.RulerTemp.setMap(null);
    if (si.Ruler) si.Ruler.setMap(null);
    if (si.Rlbl) si.Rlbl.setMap(null);
    ClearMapListners();
}
function ClearMapListners() {
    google.maps.event.clearListeners(si.map, 'click');
    //google.maps.event.clearListeners(si.map, 'dblclick');
    //google.maps.event.clearListeners(si.map, 'mousemove');
    $('#lengthAreaDiv').empty();
    si.map.setOptions({ draggableCursor: '' });
}
function createPolyline(llArr) {
    var line = new google.maps.Polyline({
        path: llArr,
        strokeColor: '#050505',
        strokeWeight: 2,
        map: si.map
    });

    return line;
}

function formatLength(len) {
    var len = parseFloat(len);
    if (len > 999)
        len = (len / 1000.00).toLocaleString() + " " + MultilingualKey.SI_GBL_GBL_JQ_FRM_036;
    else
        len = Math.round(len).toLocaleString() + " " + MultilingualKey.SI_OSP_GBL_GBL_GBL_034;

    return len.toString();
}
function drawlinebtEntity() {
    var count = 0;
    if (polyDrLine)
        polyDrLine.setMap(null);
    if (disLbl)
        disLbl.setMap(null);
    ClearRuler();
    polyDrLine = new google.maps.Polyline({
        strokeColor: '#EF4B41',
        strokeOpacity: 0.5,
        strokeWeight: 3
    });
    polyDrLine.setMap(si.map);
    si.map.setOptions({ draggableCursor: 'url(images/crosshair.cur),crosshair' });
    google.maps.event.addListener(si.map, 'click', function (event) {
        //  alert("Latitude: " + event.latLng.lat() + " " + ", longitude: " + event.latLng.lng());
        var path = polyDrLine.getPath();
        if (count < 2) {
            count++;
            path.push(event.latLng);
            var rPath = polyDrLine.getPath().getArray();
            var distance = google.maps.geometry.spherical.computeLength(rPath);
            if (count == 2) {
                disLbl = new google.maps.Label({ visible: 1, map: si.map, position: polyDrLine.getPath().getArray()[1], text: '' });
                disLbl.set('text', formatLength(distance));
            }
        }
    });
}

function showFooterDist(_path) {

    var distance = google.maps.geometry.spherical.computeLength(_path);
    if (parseInt(distance) > 999)
        return (distance / 1000).toFixed(2) + ' km';
    else
        return distance.toFixed(2) + ' meter';
}


function removeVertex(poly, vertex) {
    poly.getPath().removeAt(vertex);
    si.gMapObj.shapeArr = poly.getPath().getArray();
}

function handleShapeAreaPotentialEvents(eventParam) {

    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polygon({
                paths: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });
            google.maps.event.addListener(si.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            si.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(si.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            si.gMapObj.shapeArr = [];
            si.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(si.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polyline({
                path: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(si.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });

            si.gMapObj.RulerFlag = true;
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(si.gMapObj.shapeObj, 'click', shapeClickInfoAreaPotential);
    }
    si.gMapObj.shapeObj.setMap(si.map);
}

function handleShapeAreaPotentialForBOMEvents(eventParam) {

    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polygon({
                paths: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });
            google.maps.event.addListener(si.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            si.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(si.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            si.gMapObj.shapeArr = [];
            si.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(si.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polyline({
                path: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(si.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });

            si.gMapObj.RulerFlag = true;
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(si.gMapObj.shapeObj, 'click', shapeClickInfoAreaForBOMPotential);
    }
    si.gMapObj.shapeObj.setMap(si.map);
}

function handleShapeEvents(eventParam) {
    ;
    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polygon({
                paths: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });



            google.maps.event.addListener(si.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            si.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(si.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            si.gMapObj.shapeArr = [];
            si.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(si.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polyline({
                path: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(si.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });



            //google.maps.event.addListener(si.gMapObj.shapeObj, 'click', addPolyPoint);

            //google.maps.event.addListener(si.gMapObj.RulerLine, 'click', addPolyPoint);

            //google.maps.event.addListenerOnce(si.gMapObj.RulerLine, "dblclick", function (event) {
            //    handleShapeEvents();
            //});
            si.gMapObj.RulerFlag = true;
            //si.removeEventListnrs('click');


            //if (si.gMapObj.RulerLine) {
            //    si.gMapObj.RulerLine.setMap(null);
            //    si.gMapObj.RulerLine = null;
            //    $('#lengthAreaDiv').empty();
            //}

            //if (si.gMapObj.ClickAction && si.gMapObj.ClickAction == "RULER") {
            //    si.map.setOptions({ draggableCursor: '' });
            //    si.gMapObj.ClickAction = '';
            //}

            //else {
            //    si.map.setOptions({ draggableCursor: 'crosshair' });
            //    //google.maps.event.addListener(si.map, 'click', function (LatLong) { triggerRuler(LatLong.latLng) });



            //    google.maps.event.addListener(si.gMapObj.shapeObj, 'click', function (LatLong) { triggerRuler(LatLong.latLng) });


            //    si.gMapObj.ClickAction = "RULER"
            //}



            //google.maps.event.addListener(si.gMapObj.shapeObj, 'click', addPolyPoint);

            //google.maps.event.addListenerOnce(si.gMapObj.shapeObj, "dblclick", function (event) {
            //    triggerRuler(event.latLng);
            //});
            //si.gMapObj.RulerFlag = true;
            //si.removeEventListnrs('click');
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(si.gMapObj.shapeObj, 'click', shapeClickInfo);
    }

    si.gMapObj.shapeObj.setMap(si.map);
}

function calculateArea() {


    $('#lengthAreaDiv').show();
    var area = '';
    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            area = getPolygonArea(si.gMapObj.shapeObj);
            break;
        case 'Rectangle':
            area = getRectArea(si.gMapObj.shapeObj);
            break;
        case 'Circle':
            area = getCircleArea(si.gMapObj.shapeObj);
            break;
        case 'Square':
            area = getRectArea(si.gMapObj.shapeObj);
            break;
    }
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span>' + area + ' sq. km</span></div>');
}

function getPolygonArea(objPoly) {
    var area = google.maps.geometry.spherical.computeArea(objPoly.getPath());
    return (area / 1000000).toFixed(1);
}

function getRectArea(objRect) {
    var area = google.maps.geometry.spherical.computeArea(getRectanglePath(objRect));
    return (area / 1000000).toFixed(1);
}

function getRectanglePath(objRect) {
    var rectBounds = objRect.getBounds();
    var ne = rectBounds.getNorthEast();
    var sw = rectBounds.getSouthWest();
    var nw = new google.maps.LatLng(ne.lat(), sw.lng());
    var se = new google.maps.LatLng(sw.lat(), ne.lng());

    return new Array(nw, ne, se, sw);
}

function getCircleArea(objCircle) {
    var _rad = objCircle.getRadius();
    var area = 22 / 7 * _rad * _rad;
    return (area / 1000000).toFixed(1);
}

function validateBounds() {
    var rectbound = new google.maps.LatLngBounds(si.gMapObj.shapeArr[0], si.gMapObj.shapeArr[si.gMapObj.shapeArr.length - 1]);
    if (si.gMapObj.shapeArr[0].lng() > si.gMapObj.shapeArr[si.gMapObj.shapeArr.length - 1].lng())
        rectbound = new google.maps.LatLngBounds(si.gMapObj.shapeArr[si.gMapObj.shapeArr.length - 1], si.gMapObj.shapeArr[0]);
    return rectbound;
}
function validateLayoutBounds() {
    let rectbound = new google.maps.LatLngBounds(si.gMapLayoutObj.shapeArr[0], si.gMapLayoutObj.shapeArr[si.gMapLayoutObj.shapeArr.length - 1]);
    if (si.gMapLayoutObj.shapeArr[0].lng() > si.gMapLayoutObj.shapeArr[si.gMapLayoutObj.shapeArr.length - 1].lng())
        rectbound = new google.maps.LatLngBounds(si.gMapLayoutObj.shapeArr[si.gMapLayoutObj.shapeArr.length - 1], si.gMapLayoutObj.shapeArr[0]);
    return rectbound;
}

function validateSquareBounds() {
    var rectbound = new google.maps.LatLngBounds(si.gMapObj.shapeArr[0], si.gMapObj.shapeArr[si.gMapObj.shapeArr.length - 1]);
    if (si.gMapObj.shapeArr[0].lng() > si.gMapObj.shapeArr[si.gMapObj.shapeArr.length - 1].lng())
        rectbound = new google.maps.LatLngBounds(si.gMapObj.shapeArr[si.gMapObj.shapeArr.length - 1], si.gMapObj.shapeArr[0]);
    return rectbound;
}
function getSquareBounds(center, size) {
    var n = google.maps.geometry.spherical.computeOffset(center, size.height / 2, 0).lat(),
    s = google.maps.geometry.spherical.computeOffset(center, size.height / 2, 180).lat(),
    e = google.maps.geometry.spherical.computeOffset(center, size.width / 2, 90).lng(),
    w = google.maps.geometry.spherical.computeOffset(center, size.width / 2, 270).lng();
    return new google.maps.LatLngBounds(new google.maps.LatLng(s, w),
                                          new google.maps.LatLng(n, e))
}
function convertToRad(x) {
    return x * Math.PI / 180;
};

function getDistance(p1, p2) {
    var R = 6378137; // Earth’s mean radius in meter
    var dLat = convertToRad(p2.lat() - p1.lat());
    var dLong = convertToRad(p2.lng() - p1.lng());
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(convertToRad(p1.lat())) * Math.cos(convertToRad(p2.lat())) *
      Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;
    return d; // returns the distance in meter
};
function shapeClickInfoAreaPotential() {

    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            showAreaPotentialDetail(si.gMapObj.shapeObj.getPath().getArray(), 'polygon', si.gMapObj.purposeType);
            break;
        case 'Rectangle':
            showAreaPotentialDetail(getRectanglePath(si.gMapObj.shapeObj), 'polygon', si.gMapObj.purposeType);
            break;
        case 'Circle':
            getCircleAreaPotential('circle', si.gMapObj.purposeType);

            break;
    }
}



function shapeClickInfoAreaForBOMPotential() {

    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            showAreaPotentialDetailBOM(si.gMapObj.shapeObj.getPath().getArray(), 'polygon', si.gMapObj.purposeType);
            break;
        case 'Rectangle':
            showAreaPotentialDetailBOM(getRectanglePath(si.gMapObj.shapeObj), 'polygon', si.gMapObj.purposeType);
            break;
        case 'Circle':
            //getCirclePotential('circle', si.gMapObj.purposeType);
            getCircleAreaPotentialBOM('circle', si.gMapObj.purposeType);

            break;
    }
}



function shapeClickInfo() {
    ;
    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            showPotential(si.gMapObj.shapeObj.getPath().getArray(), 'polygon', si.gMapObj.purposeType);
            break;
        case 'Rectangle':
            showPotential(getRectanglePath(si.gMapObj.shapeObj), 'polygon', si.gMapObj.purposeType);
            break;
        case 'Circle':
            getCirclePotential('circle', si.gMapObj.purposeType);
            break;
    }
}

function showAreaPotentialDetail(latLongArr, selectionType, purposeType) {
    var longLatArr = '';
    if (latLongArr.length > 0) {
        for (i = 0; i < latLongArr.length; i++) {
            longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[i].lng() + ' ' + latLongArr[i].lat();
        }
        longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[0].lng() + ' ' + latLongArr[0].lat();

        //si.BulkProcessInfo.geom = longLatArr;
        //si.BulkProcessInfo.selection_type = selectionType;
        //si.BulkProcessInfo.buff_Radius = 0;
        //si.BulkProcessInfo.ntk_type = 'P';
        ;
        //si.ShowAreaPotentialBOMBOQ(longLatArr, selectionType);
        //si.ShowAreaPotential(longLatArr, selectionType);

        si.ShowBomBoqWindow(longLatArr, selectionType);

    }
}

function getCircleAreaPotential(selectionType) {

    var lnglat = si.gMapObj.shapeObj.getCenter().lng() + ' ' + si.gMapObj.shapeObj.getCenter().lat();
    var circleRadius = si.gMapObj.shapeObj.getRadius();

    si.BulkProcessInfo.geom = lnglat;
    si.BulkProcessInfo.selection_type = selectionType;
    si.BulkProcessInfo.buff_Radius = circleRadius;
    si.BulkProcessInfo.ntk_type = 'P';
    //si.ShowEntityLstByGeom(lnglat, selectionType, circleRadius);
    //si.ShowAreaPotential(lnglat, selectionType, circleRadius); // old one for bomboq
    si.ShowBomBoqWindow(lnglat, selectionType, circleRadius);
}




function showAreaPotentialDetailBOM(latLongArr, selectionType, purposeType) {
    var longLatArr = '';
    if (latLongArr.length > 0) {
        for (i = 0; i < latLongArr.length; i++) {
            longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[i].lng() + ' ' + latLongArr[i].lat();
        }
        longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[0].lng() + ' ' + latLongArr[0].lat();

        //si.BulkProcessInfo.geom = longLatArr;
        //si.BulkProcessInfo.selection_type = selectionType;
        //si.BulkProcessInfo.buff_Radius = 0;
        //si.BulkProcessInfo.ntk_type = 'P';

        si.ShowAreaPotentialBOM(longLatArr, selectionType);


    }
}

function getCircleAreaPotentialBOM(selectionType) {

    var lnglat = si.gMapObj.shapeObj.getCenter().lng() + ' ' + si.gMapObj.shapeObj.getCenter().lat();
    var circleRadius = si.gMapObj.shapeObj.getRadius();

    si.BulkProcessInfo.geom = lnglat;
    si.BulkProcessInfo.selection_type = selectionType;
    si.BulkProcessInfo.buff_Radius = circleRadius;
    si.BulkProcessInfo.ntk_type = 'P';
    //si.ShowEntityLstByGeom(lnglat, selectionType, circleRadius);
    si.ShowAreaPotentialBOM(lnglat, selectionType, circleRadius);
}




function showPotential(latLongArr, selectionType, purposeType) {
    var longLatArr = '';
    if (latLongArr.length > 0) {
        for (i = 0; i < latLongArr.length; i++) {
            longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[i].lng() + ' ' + latLongArr[i].lat();
        }
        longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[0].lng() + ' ' + latLongArr[0].lat();

        si.BulkProcessInfo.geom = longLatArr;
        si.BulkProcessInfo.selection_type = selectionType;
        si.BulkProcessInfo.buff_Radius = 0;
        si.BulkProcessInfo.ntk_type = 'P';

        si.ShowEntityLstByGeom(longLatArr, selectionType);

    }
}





function getCirclePotential(selectionType) {

    var lnglat = si.gMapObj.shapeObj.getCenter().lng() + ' ' + si.gMapObj.shapeObj.getCenter().lat();
    var circleRadius = si.gMapObj.shapeObj.getRadius();

    si.BulkProcessInfo.geom = lnglat;
    si.BulkProcessInfo.selection_type = selectionType;
    si.BulkProcessInfo.buff_Radius = circleRadius;
    si.BulkProcessInfo.ntk_type = 'P';
    si.ShowEntityLstByGeom(lnglat, selectionType, circleRadius);
}
function setLayerOpacity(layerName, _opacity) {
    for (i = 0; i < si.map.overlayMapTypes.length; i++) {
        if (si.map.overlayMapTypes.getAt(i).isExistingLayersRequest) {
            si.map.overlayMapTypes.getAt(i).setOpacity(_opacity);
           // break;
        }
    }
}




/***** Export Report Functions ******/

function initiateDrawingsExportReport(obj, shapeFlag) {

    if ($(obj).hasClass('ActiveTool'))
    { $(obj).removeClass('ActiveTool') } else { $(obj).addClass('ActiveTool') }
    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    si.removeEventListnrs('click');
    si.gMapObj.shapeType = shapeFlag;
    si.gMapObj.purposeType = "ExportReport";
    si.gMapObj.shapeArr = [];

    google.maps.event.addListener(si.map, 'click', handleShapeExportReportEvents);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}

function handleShapeExportReportEvents(eventParam) {

    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polygon({
                paths: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });
            google.maps.event.addListener(si.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            si.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(si.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            si.gMapObj.shapeArr = [];
            si.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(si.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polyline({
                path: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(si.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });

            si.gMapObj.RulerFlag = true;
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(si.gMapObj.shapeObj, 'click', shapeClickInfoExportReport);
    }
    si.gMapObj.shapeObj.setMap(si.map);
}


function shapeClickInfoExportReport() {
    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            showExportReportDetail(si.gMapObj.shapeObj.getPath().getArray(), 'polygon', si.gMapObj.purposeType);
            break;
        case 'Rectangle':
            showExportReportDetail(getRectanglePath(si.gMapObj.shapeObj), 'polygon', si.gMapObj.purposeType);
            break;
        case 'Circle':
            //getCirclePotential('circle', si.gMapObj.purposeType);
            getCircleExportReport('circle', si.gMapObj.purposeType);

            break;
    }
}

function showExportReportDetail(latLongArr, selectionType, purposeType) {
    var longLatArr = '';
    if (latLongArr.length > 0) {
        for (i = 0; i < latLongArr.length; i++) {
            longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[i].lng() + ' ' + latLongArr[i].lat();
        }
        longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[0].lng() + ' ' + latLongArr[0].lat();

        //si.BulkProcessInfo.geom = longLatArr;
        //si.BulkProcessInfo.selection_type = selectionType;
        //si.BulkProcessInfo.buff_Radius = 0;
        //si.BulkProcessInfo.ntk_type = 'P';
        si.ShowExportReport(longLatArr, selectionType);
    }
}

function getCircleExportReport(selectionType) {

    var lnglat = si.gMapObj.shapeObj.getCenter().lng() + ' ' + si.gMapObj.shapeObj.getCenter().lat();
    var circleRadius = si.gMapObj.shapeObj.getRadius();

    si.BulkProcessInfo.geom = lnglat;
    si.BulkProcessInfo.selection_type = selectionType;
    si.BulkProcessInfo.buff_Radius = circleRadius;
    si.BulkProcessInfo.ntk_type = 'P';
    //si.ShowEntityLstByGeom(lnglat, selectionType, circleRadius);
    si.ShowExportReport(lnglat, selectionType, circleRadius);
}






//For Print Functionality
function handleShapeEventsPrint(eventParam) {
    ;
    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);

    switch (si.gMapObj.shapeType) {
        case 'Polygon':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polygon({
                paths: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });



            google.maps.event.addListener(si.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(si.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = si.gMapObj.shapeObj.getPath();
                si.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            si.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            si.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(si.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            si.gMapObj.shapeArr = [];
            si.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(si.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(si.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(si.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            si.gMapObj.shapeArr.push(eventParam.latLng);
            si.gMapObj.shapeObj = new google.maps.Polyline({
                path: si.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(si.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });



            //google.maps.event.addListener(si.gMapObj.shapeObj, 'click', addPolyPoint);

            //google.maps.event.addListener(si.gMapObj.RulerLine, 'click', addPolyPoint);

            //google.maps.event.addListenerOnce(si.gMapObj.RulerLine, "dblclick", function (event) {
            //    handleShapeEvents();
            //});
            si.gMapObj.RulerFlag = true;
            //si.removeEventListnrs('click');


            //if (si.gMapObj.RulerLine) {
            //    si.gMapObj.RulerLine.setMap(null);
            //    si.gMapObj.RulerLine = null;
            //    $('#lengthAreaDiv').empty();
            //}

            //if (si.gMapObj.ClickAction && si.gMapObj.ClickAction == "RULER") {
            //    si.map.setOptions({ draggableCursor: '' });
            //    si.gMapObj.ClickAction = '';
            //}

            //else {
            //    si.map.setOptions({ draggableCursor: 'crosshair' });
            //    //google.maps.event.addListener(si.map, 'click', function (LatLong) { triggerRuler(LatLong.latLng) });



            //    google.maps.event.addListener(si.gMapObj.shapeObj, 'click', function (LatLong) { triggerRuler(LatLong.latLng) });


            //    si.gMapObj.ClickAction = "RULER"
            //}



            //google.maps.event.addListener(si.gMapObj.shapeObj, 'click', addPolyPoint);

            //google.maps.event.addListenerOnce(si.gMapObj.shapeObj, "dblclick", function (event) {
            //    triggerRuler(event.latLng);
            //});
            //si.gMapObj.RulerFlag = true;
            //si.removeEventListnrs('click');
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(si.gMapObj.shapeObj, 'click', shapeClickInfoPrint);
    }

    si.gMapObj.shapeObj.setMap(si.map);
}

function shapeClickInfoPrint() {
    ;
    switch (si.gMapObj.shapeType) {
        //case 'Polygon':
        //    showPotential(si.gMapObj.shapeObj.getPath().getArray(), 'polygon', si.gMapObj.purposeType);
        //    break;
        case 'Rectangle':
            showPotentialPrint(getRectanglePath(si.gMapObj.shapeObj), 'polygon', si.gMapObj.purposeType, si.gMapObj.shapeObj.getBounds());
            break;
            //case 'Circle':
            //    getCirclePotential('circle', si.gMapObj.purposeType);
            //    break;
    }
}

function showPotentialPrint(latLongArr, selectionType, purposeType, centerLatLng) {
    var longLatArr = '';
    if (latLongArr.length > 0) {
        for (i = 0; i < latLongArr.length; i++) {
            longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[i].lng() + ' ' + latLongArr[i].lat();
        }
        longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[0].lng() + ' ' + latLongArr[0].lat();

        si.PrintMap.geom = longLatArr;
        si.PrintMap.selection_type = selectionType;
        si.PrintMap.buff_Radius = 0;
        //si.PrintMap.ntk_type = 'P';
        si.PrintMap.centerLat = centerLatLng.getCenter().lat();
        si.PrintMap.centerLng = centerLatLng.getCenter().lng();
        //si.ShowEntityLstByGeom(longLatArr, selectionType);
        si.PrintMap.area = google.maps.geometry.spherical.computeArea(latLongArr);

        //si.printMap2(si.PrintMap.centerLat, si.PrintMap.centerLng, centerLatLng);
        si.toggleMapPrint(si.PrintMap.centerLat, si.PrintMap.centerLng, centerLatLng, longLatArr);
        //alert(si.PrintMap.area);


    }
}

function initiateDrawingsPrint(obj, shapeFlag) {
    ;
    // if ($(obj).hasClass('ActiveTool')) { $(obj).removeClass('ActiveTool'); } else { $(obj).addClass('ActiveTool'); }
    //  if ($(obj).hasClass('activeToolBar')) { $(obj).removeClass('activeToolBar'); } else { $(obj).addClass('activeToolBar'); }
    $(obj).addClass('activeToolBar');
    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    si.removeEventListnrs('click');
    si.gMapObj.shapeType = shapeFlag;
    si.gMapObj.purposeType = "Print";
    si.gMapObj.shapeArr = [];



    google.maps.event.addListener(si.map, 'click', handleShapeEventsPrint);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}

function ShowLineLength(_path) {
    var arrLinePath = [];
    var arrLast2Points = [];
    if (si.gMapObj.lineflag === !0 && si.gMapObj.lineflag) {
        arrLinePath = _path || si.gMapObj.libPath.slice();
        // distance b/w last 2 vertax..
        arrLast2Points = arrLinePath.slice(Math.max(arrLinePath.length - 2, 0));
        $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><b>Length :</b> <span>' + showFooterDist(arrLast2Points) + ' / Total:' + showFooterDist(arrLinePath) + '</span></div>');
    }
}


