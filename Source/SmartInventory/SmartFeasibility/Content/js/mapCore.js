var polyline = undefined;
var LayerFilters = [];
var MarkerList = [];
//PREVENT MAP SERVER HITS WHILE ZOOMING- START 1
var lastZoomNo = 0;
var lastZoomTime = new Date();
var zoomDiffSec = 1;
var isZoomChanged = false;

function checkLastZoom() {
    setInterval(function () {
        if (isZoomChanged) {
            zoomDiffSec = ((new Date() - lastZoomTime) / 1000);
            if (zoomDiffSec > 2) {
                //console.log("lastZoomNo from interval= " + lastZoomNo + " time " + zoomDiffSec);
                isZoomChanged = false;
                //load layers on map.
                let arrActiveRegionlayers = sf.getActiveRegionLayers();
                let arrActiveProvincelayers = sf.getActiveProvinceLayers();
                let isValid = (arrActiveRegionlayers.length > 0 || arrActiveProvincelayers.length > 0);
                if (isValid) {
                    sf.reqver++;
                    sf.LoadLayersOnMap();
                }
            }
        }
    }, 30);
}
//PREVENT MAP SERVER HITS WHILE ZOOMING- END 1

function createOverlayLayer(objlayer, trackURLS, fn) {


    if (trackURLS) {
        var overlayUrlsArr = overlayUrlsArr || {};
        overlayUrlsArr[objlayer.DisplayName] = new Object();
        overlayUrlsArr[objlayer.DisplayName].initialDraw = true;
        overlayUrlsArr[objlayer.DisplayName].pendingUrls = new Array();
    }



    LayerFilters.push(objlayer);

    //removeLayerIFExists(objlayer.DisplayName);
    var newLayer = new google.maps.ImageMapType({
        getTileUrl: function (coord, zoom) {

            //PREVENT MAP SERVER HITS WHILE ZOOMING- START 3
            if (zoomDiffSec < 2 && lastZoomNo > 0 && isZoomChanged) {
                //console.log("last return zoom " + si.map.getZoom() + "time ====" + zoomDiffSec);
                return;
            }
            else {
                lastZoonTime = new Date();
                lastZoomNo = sf.map.getZoom();
                //console.log("last success zoom" + lastZoomNo);
                isZoomChanged = false;
            }
            //PREVENT MAP SERVER HITS WHILE ZOOMING- END 3
            //var layerNames=getZoomEntity(objlayer.Name,objlayer.DisplayName,zoom);
            // if(layerNames!=''){
            var proj = sf.map.getProjection();
            var zfactor = Math.pow(2, zoom);
            var top = proj.fromPointToLatLng(new google.maps.Point(coord.x * 1024 / zfactor, coord.y * 256 / zfactor));
            var bot = proj.fromPointToLatLng(new google.maps.Point((coord.x + 1) * 1024 / zfactor, (coord.y + 1) * 256 / zfactor));
            var bbox = top.lng() + "," + bot.lat() + "," + bot.lng() + "," + top.lat();
            var currentLayers = '';
            //if(objlayer.isNetworkLayer && objlayer.network_status!='')
            // {
            //    if(objlayer.isWithLabel)
            //   {
            //      currentLayers=sf.getActiveNetworkLayersWithLabel(objlayer.network_status).join(',');
            //  }else{
            //      currentLayers=sf.getActiveNetworkLayersNew(objlayer.network_status).join(',');
            // }
            // }

            //Code with array
            //if(objlayer.isNetworkLayer && objlayer.network_status!='')
            //{
            //    if(objlayer.isWithLabel)
            //    {
            //        currentLayers=getCurrentActiveLayer(objlayer.network_status,true).join(',');
            //    }
            //    else{
            //        currentLayers=getCurrentActiveLayer(objlayer.network_status,false).join(',');
            //    }
            //}

            var url = getMapserver();
            url += "MAP=" + objlayer.MapFilePath; //Map File
            url += "&srs=EPSG:4326";     //set WGS84 
            url += "&version=1.0.0";  //WMS version  
            url += "&REQUEST=GetMap"; //WMS operation
            //if(currentLayers!='')
            // { 
            //     url += "&LAYERS=" +currentLayers; //WMS layers  
            // }
            // else
            //{
            url += "&LAYERS=" + objlayer.Name; //WMS layers  
            //}

            url += addFilters(objlayer.Filters);
            url += "&service=wms";    //WMS service         
            url += "&BBOX=" + bbox;      // set bounding box
            url += "&WIDTH=1024";         //tile size in google
            url += "&HEIGHT=256";
            url += "&FORMAT=image/png"; //WMS format
            url += "&reqver=" + sf.reqver; //WMS format
            //url += "&BGCOLOR=0xFFFFFF";
            //url += "&TRANSPARENT=TRUE";    
            if (trackURLS) {
                if (overlayUrlsArr[objlayer.DisplayName].initialDraw)
                    overlayUrlsArr[objlayer.DisplayName].pendingUrls.push(url);
            }

            if ($("#hdnMapUrl")) {
                if (objlayer.Name && objlayer.Name != 'Region,Province') {
                    $("#hdnMapUrl").val('');
                    $("#hdnMapUrl").val(url);
                    //LayerFilters.push(url);
                }
            }
            // if(objlayer.isNetworkLayer && currentLayers=='')
            //return'';

            if (objlayer.Name == '')
                return '';

            // console.log(url);
            return url;
            //}
            //else{
            // return "";
            // }
            //return getMapserver() + 'MAP=' + objlayer.MapFilePath + '&mode=tile&tilemode=gmap&tile=' + coord.x + '+' + coord.y + '+' + zoom + '&LAYERS=' + objlayer.Name + addFilters(objlayer.Filters);
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
    $.each($(sf.DE.ulNetworkLayers + " li .mainLyr:checked"), function () {
        var minZoom = $('.mainlyr li[data-mapabbr="' + $(this).attr('data-mapabbr') + '"] #MinMaxZoom').attr('data-min-zoom');
        var maxZoom = $('.mainlyr li[data-mapabbr="' + $(this).attr('data-mapabbr') + '"] #MinMaxZoom').attr('data-max-zoom');
        var currentZoom = sf.map.getZoom();
        if (currentZoom >= parseInt(minZoom) && currentZoom <= parseInt(maxZoom)) {
            lyrs.push($(this).attr('data-mapabbr'));
        }
    });
    return lyrs;
}

function getCurrentActiveLayer(networkStatus, isWithLabel) {
    var lyrs = [];
    var currentZoom = sf.map.getZoom();
    //var layers = sf.layerDetails.filter(p=>p.networkStatus==networkStatus && p.isWithLabel==isWithLabel && parseInt(p.minZoom)<=currentZoom && parseInt(p.maxZoom)>=currentZoom);
    //IE Solution
    var layers = sf.layerDetails.filter(function (p) { return p.networkStatus == networkStatus && p.isWithLabel == isWithLabel && parseInt(p.minZoom) <= currentZoom && parseInt(p.maxZoom) >= currentZoom; });
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
                //var srchlayer = sf._layerZoomEntity.find(p => p.layerType == 'BLD,BLDP,BLDC');
                //IE Solution
                var srchlayer = sf._layerZoomEntity.find(function (p) { return p.layerType == 'BLD,BLDP,BLDC'; });
                if (_CurrZoom >= srchlayer.minZoomlvl && _CurrZoom <= srchlayer.maxZoomlvl) {
                    _layer.push(CurrLayerArray[i]);
                }
            }
            else {
                //var srchlayer = sf._layerZoomEntity.find(p => p.layerType == CurrLayerArray[i]);
                //IE Solution
                var srchlayer = sf._layerZoomEntity.find(function (p) { return p.layerType == CurrLayerArray[i]; });
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
    var mapServer = ((sf.mapServer == sf.mapServers.length - 1) ? 0 : (sf.mapServer + 1));
    return sf.mapServers[mapServer];
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


function initiateDrawingsBulkAsBuilt(obj, shapeFlag) {

    if ($(obj).hasClass('ActiveTool'))
    { $(obj).removeClass('ActiveTool') } else { $(obj).addClass('ActiveTool') }
    if (sf.gMapObj.shapeObj)
        sf.gMapObj.shapeObj.setMap(null);
    sf.removeEventListnrs('click');
    sf.gMapObj.shapeType = shapeFlag;
    sf.gMapObj.purposeType = "BulkAsBuilt";
    sf.gMapObj.shapeArr = [];



    google.maps.event.addListener(sf.map, 'click', handleShapeEvents);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}


function initiateDrawingsPotential(obj, shapeFlag) {

    if ($(obj).hasClass('ActiveTool'))
    { $(obj).removeClass('ActiveTool') } else { $(obj).addClass('ActiveTool') }
    if (sf.gMapObj.shapeObj)
        sf.gMapObj.shapeObj.setMap(null);
    sf.removeEventListnrs('click');
    sf.gMapObj.shapeType = shapeFlag;
    sf.gMapObj.purposeType = "AreaPotential";
    sf.gMapObj.shapeArr = [];



    google.maps.event.addListener(sf.map, 'click', handleShapeAreaPotentialEvents);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}



function initiateDrawingsPotentialFORBOM(obj, shapeFlag) {

    if ($(obj).hasClass('ActiveTool'))
    { $(obj).removeClass('ActiveTool') } else { $(obj).addClass('ActiveTool') }
    if (sf.gMapObj.shapeObj)
        sf.gMapObj.shapeObj.setMap(null);
    sf.removeEventListnrs('click');
    sf.gMapObj.shapeType = shapeFlag;
    sf.gMapObj.purposeType = "AreaPotential";
    sf.gMapObj.shapeArr = [];



    google.maps.event.addListener(sf.map, 'click', handleShapeAreaPotentialForBOMEvents);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}

function calculateDirection() {
    //remove mesure distance events..
    if (sf.gMapObj.RulerLine) {
        sf.removeEventListnrs('click');
        sf.gMapObj.RulerLine.setMap(null);
        sf.gMapObj.RulerLine = null;
        $('#lengthAreaDiv').empty();
    }
    sf.clearMarkers();
    ClearRuler();
    if (directionsDisplay) {
        directionsDisplay.setMap(null);
    }

    alert('Select source by clicking on map.');
    window.setTimeout(function () { hideDiv('alertMsgBox'); }, 1000);
    sf.map.setOptions({ draggableCursor: 'crosshair' });
    google.maps.event.addListener(sf.map, 'click', function (e) {
        var src = sf.createMarker(e.latLng, 'content/images/source.png');
        src.setMap(sf.map);
        MarkerList.push(src);
        sf.removeEventListnrs('click');
        alert('Now select destination by clicking on map.');
        window.setTimeout(function () { hideDiv('alertMsgBox'); }, 1000);
        var request = {
            origin: e.latLng,
            travelMode: google.maps.DirectionsTravelMode.DRIVING,
            provideRouteAlternatives: true
        };
        google.maps.event.addListener(sf.map, 'click', function (f) {
            sf.removeEventListnrs('click');
            sf.map.setOptions({ draggableCursor: '' });
            var dest = sf.createMarker(f.latLng, 'content/images/destination.png');
            dest.setMap(sf.map);
            MarkerList.push(dest);
            request.destination = f.latLng;
            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                    directionsDisplay.setMap(sf.map);
                    src.setMap(null);
                    dest.setMap(null);
                }
                else {
                    alert('Directions not found between source and destination.');
                    window.setTimeout(function () {
                        src.setMap(null); dest.setMap(null);
                    }, 1000);
                }
            });
        });
    });
}

function addPolyPoint(e) {


    var vertices = sf.gMapObj.RulerLine.getPath();
    vertices.push(e.latLng);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><b>Length :</b> <span>' + showFooterDist(vertices) + '</span></div>');
}

var polyDrLine;
var disLbl;
function toggleRuler() {
    sf.RulerFlag = sf.RulerFlag || !0;
    ClearRuler();
    if (polyDrLine)
        polyDrLine.setMap(null);
    if (disLbl)
        disLbl.setMap(null);
    if (directionsDisplay) {
        directionsDisplay.setMap(null);
    }
    sf.clearMarkers();
    var RulerPath = [];
    if (sf.RulerFlag === !0) {
        sf.Rlbl = new google.maps.Label({ visible: !1, map: sf.map, position: sf.map.getCenter(), text: '' });
        sf.map.setOptions({ draggableCursor: 'url(../Content/images/hand.cur),crosshair' });
        sf.Ruler = createPolyline(RulerPath);
        sf.RulerTemp = createPolyline([]);
        sf.RulerTemp.setOptions({ clickable: !1, strokeOpacity: 0.5, strokeColor: '#EF4B41' });
        sf.Ruler.setOptions({ clickable: !1, strokeColor: '#EF4B41' });

        google.maps.event.addListener(sf.map, 'click', function (e) {
            RulerPath.push(e.latLng);
            sf.Ruler.setPath(RulerPath);
        });

        google.maps.event.addListenerOnce(sf.map, 'dblclick', function (e) {
            sf.RulerFlag = !1;
            sf.RulerTemp.setMap(null);
            ClearMapListners();
            var rPath = sf.Ruler.getPath().getArray();
            var distance = google.maps.geometry.spherical.computeLength(rPath);
            sf.Rlbl.set('text', formatLength(distance));
        });

        google.maps.event.addListener(sf.map, 'mousemove', function (e) {

            if (sf.RulerFlag === !0 && sf.Ruler) {
                var rPath = sf.Ruler.getPath().getArray();
                if (rPath.length > 0) {
                    var TempPath = [rPath[rPath.length - 1], e.latLng];
                    sf.RulerTemp = sf.RulerTemp || createPolyline([]);
                    sf.RulerTemp.setPath(TempPath);
                    var distance = google.maps.geometry.spherical.computeLength(rPath) + google.maps.geometry.spherical.computeLength(TempPath);
                    sf.Rlbl.set('visible', !0);
                    sf.Rlbl.set('text', formatLength(distance));
                    sf.Rlbl.set('position', e.latLng);
                }
            }
        });
    }
}
function ClearRuler() {
    if (sf.RulerTemp) sf.RulerTemp.setMap(null);
    if (sf.Ruler) sf.Ruler.setMap(null);
    if (sf.Rlbl) sf.Rlbl.setMap(null);
    ClearMapListners();
}
function ClearMapListners() {
    google.maps.event.clearListeners(sf.map, 'click');
    google.maps.event.clearListeners(sf.map, 'dblclick');
    google.maps.event.clearListeners(sf.map, 'mousemove');
    $('#lengthAreaDiv').empty();
    sf.map.setOptions({ draggableCursor: '' });
}
function createPolyline(llArr) {
    var line = new google.maps.Polyline({
        path: llArr,
        strokeColor: '#050505',
        strokeWeight: 2,
        map: sf.map
    });

    return line;
}

function formatLength(len) {
    var len = parseFloat(len);
    if (len > 999)
        len = (len / 1000.00).toLocaleString() + ' km';
    else
        len = Math.round(len).toLocaleString() + ' mtr'

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
    polyDrLine.setMap(sf.map);
    sf.map.setOptions({ draggableCursor: 'url(images/crosshair.cur),crosshair' });
    google.maps.event.addListener(sf.map, 'click', function (event) {
        //  alert("Latitude: " + event.latLng.lat() + " " + ", longitude: " + event.latLng.lng());
        var path = polyDrLine.getPath();
        if (count < 2) {
            count++;
            path.push(event.latLng);
            var rPath = polyDrLine.getPath().getArray();
            var distance = google.maps.geometry.spherical.computeLength(rPath);
            if (count == 2) {
                disLbl = new google.maps.Label({ visible: 1, map: sf.map, position: polyDrLine.getPath().getArray()[1], text: '' });
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
        return distance.toFixed(2) + ' mtr';
}


function removeVertex(poly, vertex) {
    poly.getPath().removeAt(vertex);
    sf.gMapObj.shapeArr = poly.getPath().getArray();
}



function handleShapeAreaPotentialEvents(eventParam) {

    if (sf.gMapObj.shapeObj)
        sf.gMapObj.shapeObj.setMap(null);
    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            sf.gMapObj.shapeArr.push(eventParam.latLng);
            sf.gMapObj.shapeObj = new google.maps.Polygon({
                paths: sf.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(sf.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(sf.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = sf.gMapObj.shapeObj.getPath();
                sf.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(sf.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = sf.gMapObj.shapeObj.getPath();
                sf.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            sf.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            sf.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            sf.gMapObj.shapeArr = [];
            sf.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(sf.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            sf.gMapObj.shapeArr.push(eventParam.latLng);
            sf.gMapObj.shapeObj = new google.maps.Polyline({
                path: sf.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(sf.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });

            sf.gMapObj.RulerFlag = true;
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(sf.gMapObj.shapeObj, 'click', shapeClickInfoAreaPotential);
    }
    sf.gMapObj.shapeObj.setMap(sf.map);
}

function handleShapeAreaPotentialForBOMEvents(eventParam) {

    if (sf.gMapObj.shapeObj)
        sf.gMapObj.shapeObj.setMap(null);
    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            sf.gMapObj.shapeArr.push(eventParam.latLng);
            sf.gMapObj.shapeObj = new google.maps.Polygon({
                paths: sf.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(sf.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(sf.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = sf.gMapObj.shapeObj.getPath();
                sf.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(sf.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = sf.gMapObj.shapeObj.getPath();
                sf.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            sf.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            sf.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            sf.gMapObj.shapeArr = [];
            sf.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(sf.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            sf.gMapObj.shapeArr.push(eventParam.latLng);
            sf.gMapObj.shapeObj = new google.maps.Polyline({
                path: sf.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(sf.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });

            sf.gMapObj.RulerFlag = true;
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(sf.gMapObj.shapeObj, 'click', shapeClickInfoAreaForBOMPotential);
    }
    sf.gMapObj.shapeObj.setMap(sf.map);
}





function handleShapeEvents(eventParam) {

    if (sf.gMapObj.shapeObj)
        sf.gMapObj.shapeObj.setMap(null);
    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            sf.gMapObj.shapeArr.push(eventParam.latLng);
            sf.gMapObj.shapeObj = new google.maps.Polygon({
                paths: sf.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });



            google.maps.event.addListener(sf.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(sf.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(sf.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = sf.gMapObj.shapeObj.getPath();
                sf.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(sf.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = sf.gMapObj.shapeObj.getPath();
                sf.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            sf.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            sf.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            sf.gMapObj.shapeArr = [];
            sf.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(sf.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            sf.gMapObj.shapeArr.push(eventParam.latLng);
            sf.gMapObj.shapeObj = new google.maps.Polyline({
                path: sf.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(sf.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });



            //google.maps.event.addListener(sf.gMapObj.shapeObj, 'click', addPolyPoint);

            //google.maps.event.addListener(sf.gMapObj.RulerLine, 'click', addPolyPoint);

            //google.maps.event.addListenerOnce(sf.gMapObj.RulerLine, "dblclick", function (event) {
            //    handleShapeEvents();
            //});
            sf.gMapObj.RulerFlag = true;
            //sf.removeEventListnrs('click');


            //if (sf.gMapObj.RulerLine) {
            //    sf.gMapObj.RulerLine.setMap(null);
            //    sf.gMapObj.RulerLine = null;
            //    $('#lengthAreaDiv').empty();
            //}

            //if (sf.gMapObj.ClickAction && sf.gMapObj.ClickAction == "RULER") {
            //    sf.map.setOptions({ draggableCursor: '' });
            //    sf.gMapObj.ClickAction = '';
            //}

            //else {
            //    sf.map.setOptions({ draggableCursor: 'crosshair' });
            //    //google.maps.event.addListener(sf.map, 'click', function (LatLong) { triggerRuler(LatLong.latLng) });



            //    google.maps.event.addListener(sf.gMapObj.shapeObj, 'click', function (LatLong) { triggerRuler(LatLong.latLng) });


            //    sf.gMapObj.ClickAction = "RULER"
            //}



            //google.maps.event.addListener(sf.gMapObj.shapeObj, 'click', addPolyPoint);

            //google.maps.event.addListenerOnce(sf.gMapObj.shapeObj, "dblclick", function (event) {
            //    triggerRuler(event.latLng);
            //});
            //sf.gMapObj.RulerFlag = true;
            //sf.removeEventListnrs('click');
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(sf.gMapObj.shapeObj, 'click', shapeClickInfo);
    }

    sf.gMapObj.shapeObj.setMap(sf.map);
}
function calculateArea() {



    $('#lengthAreaDiv').show();
    var area = '';
    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            area = getPolygonArea(sf.gMapObj.shapeObj);
            break;
        case 'Rectangle':
            area = getRectArea(sf.gMapObj.shapeObj);
            break;
        case 'Circle':
            area = getCircleArea(sf.gMapObj.shapeObj);
            break;
    }
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> ' + area + ' sq. km</span></div>');
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
    var rectbound = new google.maps.LatLngBounds(sf.gMapObj.shapeArr[0], sf.gMapObj.shapeArr[sf.gMapObj.shapeArr.length - 1]);
    if (sf.gMapObj.shapeArr[0].lng() > sf.gMapObj.shapeArr[sf.gMapObj.shapeArr.length - 1].lng())
        rectbound = new google.maps.LatLngBounds(sf.gMapObj.shapeArr[sf.gMapObj.shapeArr.length - 1], sf.gMapObj.shapeArr[0]);
    return rectbound;
}



function shapeClickInfoAreaPotential() {

    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            showAreaPotentialDetail(sf.gMapObj.shapeObj.getPath().getArray(), 'polygon', sf.gMapObj.purposeType);
            break;
        case 'Rectangle':
            showAreaPotentialDetail(getRectanglePath(sf.gMapObj.shapeObj), 'polygon', sf.gMapObj.purposeType);
            break;
        case 'Circle':
            //getCirclePotential('circle', sf.gMapObj.purposeType);
            getCircleAreaPotential('circle', sf.gMapObj.purposeType);

            break;
    }
}



function shapeClickInfoAreaForBOMPotential() {

    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            showAreaPotentialDetailBOM(sf.gMapObj.shapeObj.getPath().getArray(), 'polygon', sf.gMapObj.purposeType);
            break;
        case 'Rectangle':
            showAreaPotentialDetailBOM(getRectanglePath(sf.gMapObj.shapeObj), 'polygon', sf.gMapObj.purposeType);
            break;
        case 'Circle':
            //getCirclePotential('circle', sf.gMapObj.purposeType);
            getCircleAreaPotentialBOM('circle', sf.gMapObj.purposeType);

            break;
    }
}



function shapeClickInfo() {
    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            showPotential(sf.gMapObj.shapeObj.getPath().getArray(), 'polygon', sf.gMapObj.purposeType);
            break;
        case 'Rectangle':
            showPotential(getRectanglePath(sf.gMapObj.shapeObj), 'polygon', sf.gMapObj.purposeType);
            break;
        case 'Circle':
            getCirclePotential('circle', sf.gMapObj.purposeType);
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

        //sf.BulkProcessInfo.geom = longLatArr;
        //sf.BulkProcessInfo.selection_type = selectionType;
        //sf.BulkProcessInfo.buff_Radius = 0;
        //sf.BulkProcessInfo.ntk_type = 'P';

        sf.ShowAreaPotential(longLatArr, selectionType);


    }
}

function getCircleAreaPotential(selectionType) {

    var lnglat = sf.gMapObj.shapeObj.getCenter().lng() + ' ' + sf.gMapObj.shapeObj.getCenter().lat();
    var circleRadius = sf.gMapObj.shapeObj.getRadius();

    sf.BulkProcessInfo.geom = lnglat;
    sf.BulkProcessInfo.selection_type = selectionType;
    sf.BulkProcessInfo.buff_Radius = circleRadius;
    sf.BulkProcessInfo.ntk_type = 'P';
    //sf.ShowEntityLstByGeom(lnglat, selectionType, circleRadius);
    sf.ShowAreaPotential(lnglat, selectionType, circleRadius);
}




function showAreaPotentialDetailBOM(latLongArr, selectionType, purposeType) {
    var longLatArr = '';
    if (latLongArr.length > 0) {
        for (i = 0; i < latLongArr.length; i++) {
            longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[i].lng() + ' ' + latLongArr[i].lat();
        }
        longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[0].lng() + ' ' + latLongArr[0].lat();

        //sf.BulkProcessInfo.geom = longLatArr;
        //sf.BulkProcessInfo.selection_type = selectionType;
        //sf.BulkProcessInfo.buff_Radius = 0;
        //sf.BulkProcessInfo.ntk_type = 'P';

        sf.ShowAreaPotentialBOM(longLatArr, selectionType);


    }
}

function getCircleAreaPotentialBOM(selectionType) {

    var lnglat = sf.gMapObj.shapeObj.getCenter().lng() + ' ' + sf.gMapObj.shapeObj.getCenter().lat();
    var circleRadius = sf.gMapObj.shapeObj.getRadius();

    sf.BulkProcessInfo.geom = lnglat;
    sf.BulkProcessInfo.selection_type = selectionType;
    sf.BulkProcessInfo.buff_Radius = circleRadius;
    sf.BulkProcessInfo.ntk_type = 'P';
    //sf.ShowEntityLstByGeom(lnglat, selectionType, circleRadius);
    sf.ShowAreaPotentialBOM(lnglat, selectionType, circleRadius);
}




function showPotential(latLongArr, selectionType, purposeType) {
    var longLatArr = '';
    if (latLongArr.length > 0) {
        for (i = 0; i < latLongArr.length; i++) {
            longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[i].lng() + ' ' + latLongArr[i].lat();
        }
        longLatArr += (longLatArr == '' ? '' : ',') + latLongArr[0].lng() + ' ' + latLongArr[0].lat();

        sf.BulkProcessInfo.geom = longLatArr;
        sf.BulkProcessInfo.selection_type = selectionType;
        sf.BulkProcessInfo.buff_Radius = 0;
        sf.BulkProcessInfo.ntk_type = 'P';

        sf.ShowEntityLstByGeom(longLatArr, selectionType);

    }
}



function getCirclePotential(selectionType) {

    var lnglat = sf.gMapObj.shapeObj.getCenter().lng() + ' ' + sf.gMapObj.shapeObj.getCenter().lat();
    var circleRadius = sf.gMapObj.shapeObj.getRadius();

    sf.BulkProcessInfo.geom = lnglat;
    sf.BulkProcessInfo.selection_type = selectionType;
    sf.BulkProcessInfo.buff_Radius = circleRadius;
    sf.BulkProcessInfo.ntk_type = 'P';
    sf.ShowEntityLstByGeom(lnglat, selectionType, circleRadius);
}
function setLayerOpacity(layerName, _opacity) {
    for (i = 0; i < sf.map.overlayMapTypes.length; i++) {
        if (sf.map.overlayMapTypes.getAt(i).name == layerName) {
            sf.map.overlayMapTypes.getAt(i).setOpacity(_opacity);
            break;
        }
    }
}




/***** Export Report Functions ******/

function initiateDrawingsExportReport(obj, shapeFlag) {

    if ($(obj).hasClass('ActiveTool'))
    { $(obj).removeClass('ActiveTool') } else { $(obj).addClass('ActiveTool') }
    if (sf.gMapObj.shapeObj)
        sf.gMapObj.shapeObj.setMap(null);
    sf.removeEventListnrs('click');
    sf.gMapObj.shapeType = shapeFlag;
    sf.gMapObj.purposeType = "ExportReport";
    sf.gMapObj.shapeArr = [];

    google.maps.event.addListener(sf.map, 'click', handleShapeExportReportEvents);
    $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><span><b>Area:</b> 0 sq. km</span></div>');
}

function handleShapeExportReportEvents(eventParam) {

    if (sf.gMapObj.shapeObj)
        sf.gMapObj.shapeObj.setMap(null);
    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            sf.gMapObj.shapeArr.push(eventParam.latLng);
            sf.gMapObj.shapeObj = new google.maps.Polygon({
                paths: sf.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true
            });
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'mouseup', calculateArea);
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(sf.gMapObj.shapeObj, event.vertex);
                }
            });

            google.maps.event.addListener(sf.gMapObj.shapeObj.getPath(), 'set_at', function (indx) {
                var newPath = sf.gMapObj.shapeObj.getPath();
                sf.gMapObj.shapeArr = newPath.getArray();
            });

            google.maps.event.addListener(sf.gMapObj.shapeObj.getPath(), 'insert_at', function (indx) {
                var newPath = sf.gMapObj.shapeObj.getPath();
                sf.gMapObj.shapeArr = newPath.getArray();
            });
            break;
        case 'Rectangle':
            sf.gMapObj.shapeArr.push(eventParam.latLng);
            var rectBound = validateBounds();
            sf.gMapObj.shapeObj = new google.maps.Rectangle({
                strokeColor: '#FF8800',
                strokeOpacity: 0.6,
                strokeWeight: 2,
                fillColor: '#FF8800',
                fillOpacity: 0.35,
                editable: true,
                draggable: true,
                bounds: rectBound
            });
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'bounds_changed', calculateArea);

            break;
        case 'Circle':
            sf.gMapObj.shapeArr = [];
            sf.gMapObj.shapeObj = new google.maps.Circle({
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
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'radius_changed', calculateArea);
            google.maps.event.addListener(sf.gMapObj.shapeObj, 'rightclick', function (event) {
                if (event.vertex == undefined) {
                    return;
                } else {
                    removeVertex(sf.gMapObj.shapeObj, event.vertex);
                }
            });

            break;
        case 'PolyLine':

            sf.gMapObj.shapeArr.push(eventParam.latLng);
            sf.gMapObj.shapeObj = new google.maps.Polyline({
                path: sf.gMapObj.shapeArr,
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                draggable: true

            });

            google.maps.event.addListener(sf.gMapObj.RulerLine, 'click', function (event) { addPolyPoint });

            sf.gMapObj.RulerFlag = true;
            break;
    }
    if (eventParam != 'PolyLine') {
        calculateArea();
        google.maps.event.addListener(sf.gMapObj.shapeObj, 'click', shapeClickInfoExportReport);
    }
    sf.gMapObj.shapeObj.setMap(sf.map);
}


function shapeClickInfoExportReport() {
    switch (sf.gMapObj.shapeType) {
        case 'Polygon':
            showExportReportDetail(sf.gMapObj.shapeObj.getPath().getArray(), 'polygon', sf.gMapObj.purposeType);
            break;
        case 'Rectangle':
            showExportReportDetail(getRectanglePath(sf.gMapObj.shapeObj), 'polygon', sf.gMapObj.purposeType);
            break;
        case 'Circle':
            //getCirclePotential('circle', sf.gMapObj.purposeType);
            getCircleExportReport('circle', sf.gMapObj.purposeType);

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

        //sf.BulkProcessInfo.geom = longLatArr;
        //sf.BulkProcessInfo.selection_type = selectionType;
        //sf.BulkProcessInfo.buff_Radius = 0;
        //sf.BulkProcessInfo.ntk_type = 'P';
        sf.ShowExportReport(longLatArr, selectionType);
    }
}

function getCircleExportReport(selectionType) {

    var lnglat = sf.gMapObj.shapeObj.getCenter().lng() + ' ' + sf.gMapObj.shapeObj.getCenter().lat();
    var circleRadius = sf.gMapObj.shapeObj.getRadius();

    sf.BulkProcessInfo.geom = lnglat;
    sf.BulkProcessInfo.selection_type = selectionType;
    sf.BulkProcessInfo.buff_Radius = circleRadius;
    sf.BulkProcessInfo.ntk_type = 'P';
    //sf.ShowEntityLstByGeom(lnglat, selectionType, circleRadius);
    sf.ShowExportReport(lnglat, selectionType, circleRadius);
}