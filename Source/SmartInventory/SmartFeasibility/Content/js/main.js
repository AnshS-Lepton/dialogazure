
var isFtth = false;
var map;
var app;
var isFirst = true;
var main = function () {
    app = this;
    this.lyrTbls = [],     // new added
        this.lyrNames = [],  // new added
        this.lyrMarkers = [],   // new added
        this.DirRoutes = [],   // new added
        this.distanceWidget_Ftth = undefined;//new added
    this.distanceWidget_F = undefined;
    this.FtthMarker = undefined;//new added
    this.FtthMarker_saved = undefined;//new added
    this.radiuswidget_ftth = undefined;//new added
    this.detailsMenuFTTH = undefined;//new added
    this.FtthLatLng = [];//new added
    this.ftthEntityId = undefined,//new added
        this.NeDeviceDist = undefined,//new added
        this.tbhtml = undefined,//new added
        this.neElmLoc = undefined//new added
    this.iRadiusInKiloMeter = 0.05//new added
    this.stepPolyline = undefined;//new added
    this.pastKMLDataFtth = [];
    this.ExportKMLDataFtth = [];
    this.polylines = [];
    this.allActiveLayers = [];
    this.fadedmap = [new google.maps.LatLng(85, 180), new google.maps.LatLng(85, 0), new google.maps.LatLng(85, -180), new google.maps.LatLng(-85, -180), new google.maps.LatLng(-85, 0), new google.maps.LatLng(-85, 180)],    // new added
        this.ActivePlannedlayers = [];
    this.ActiveAsBuiltlayers = [];
    this.ActiveDormantlayers = [];
    this.ActiveRegionlayers = [];
    this.ActiveProvincelayers = [];
    this.wayPoints = [];
    this.map = undefined;
    this.mapServers = [];
    this.mapDirPath = undefined;
    this.reqver = 0;
    this.mapServer = 0;
    this.filterprojectvalue = '';
    this.filterOwnershipvalue = '';
    this.filterFaultvalue = '';
    this.ownership = '0';
    this._showLabel = false;
    this.layerManager = [];
    this.gMapObj = {};
    this.startLatLng = {};
    this.endLatLng = {};
    this.cores_required = 0;
    this.startMarker = undefined;
    this.endMarker = undefined;
    this.startMarker_saved = undefined;
    this.endMarker_saved = undefined;
    this.directionsService = undefined;
    this.feasiblePaths = [];
    this.insidePaths = [];
    this.savedPaths = [];
    this.FeasibilityDistances = {};
    this.infowindow = undefined;
    this.editMenu = undefined;
    this.detailsMenu = undefined;
    this.color_LMC = undefined;
    this.color_Outside = undefined;
    this.color_inside = undefined;
    this.feasibilityDetailsInfo = [];
    this.feasibilityGeometry = [];
    this.feasibility_saved = [];
    this.is_core_feasibile = false;
    this.distanceWidget_A = undefined;
    this.distanceWidget_B = undefined;
    this.ParentModel = 'PARENT';
    this.ChildModel = 'CHILD';
    this.geomTypes = [];
    this.pastKMLData = [];
    this.pastBOMData = [];
    this.bulkFeasibility_popUp = undefined;
    this.pastFeasibility_popUp = undefined;
    this.toggleBaseMap_popUp = undefined;
    this.sliderDefaults = { MIN: 100, MAX: 2000, VALUE: 500, STEP: 10 };
    this.sliderDefaults_Ftth = { MIN: 100, MAX: 2000, VALUE: 50, STEP: 10 };
    this.DEMO_VERSION = undefined;
    this.ROUTING_API_URL = undefined;
    this.feasibilityStatus = "";
    this.pastFeasibilities = [];
    this.bulkFeasibilityData = undefined;
    this.LAT_LNG_FORMAT_TYPES = {
        "DEGREE_DECIMAL": "DD",
        "DEGREES_MINUTES_SECONDS": "DMS"
    };
    this.LAT_LNG_FORMAT = app.LAT_LNG_FORMAT_TYPES.DEGREE_DECIMAL;
    this.cable_types = {
        "INSIDE_P": "inside_P",
        "INSIDE_A": "inside_A",
        "INSIDE": "inside",
        "OUTSIDE_A_END": "outside_start",
        "OUTSIDE_B_END": "outside_end",
        "LMC_A_END": "lmc_start",
        "LMC_B_END": "lmc_end"
    };
    this.DE = {
        "chkAll": "#checkAll",
        "ulNetworkLayers": ".layers .network",
        "ulRegionLayers": ".layers .region",
        "ulProvinceLayers": ".layers .region .province",
        "divLayers": "#divLayers",
        "lyrRefresh": ".lyrRefresh",
        "spnCoords": "#spnCoords",
        "mapScale": "#mapScale",
        "hdnMapCordinates": "#hdnMapCordinates",
        "leftPanelOpenClose": '#open-close',
        "rightPanelOpenClose": '#open-close-right',
        "leftPanel": "#floating-panel-left",
        "rightPanel": "#floating-panel-right",
        "gSearchTxtBx": "txtGoogleSearch",
        "gSearch": "#txtGoogleSearch",
        "selectStartLatLng": "#selectStartLatLng",
        "selectEndLatLng": "#selectEndLatLng",
        "coresTextBox": "#cores",
        "cableTypeddl": "#cableType",
        "txtStartLat": "#startLat",
        "txtStartLng": "#startLng",
        "txtEndLat": "#endLat",
        "txtEndLng": "#endLng",
        "txtStartPointName": "#startPointName",
        "txtEndPointName": "#endPointName",
        "txtLatLng": ".txtLatLng",
        "txtBufferRadius": ".txtBufferRadius",
        "txtCustomerID": "#customerID",
        "txtCustomerName": "#customerName",
        "txtFseasibilityID": "#feasibilityID",
        "btnCompute": "#btnCompute",
        "btnReset": "#btnReset",
        "btnfeasibilitySubmit": "#btnfeasibilitySubmit",
        //"selectedColor": "#0326cc",
        "selectedColor": "#fb230d",
        "dullColor": "#adc3ca",
        'multicons': ".multicons span",
        "feasTypeWrapper": ".feasTypeWrapper div",
        "downloadFeasReport": ".downloadFeasStatus",
        "cableType": "#cableType option:selected",
        "slider_A": "#slider1",
        "slider_B": "#slider2",
        "sliderHandle_A": "#custom-handle1",
        "sliderHandle_B": "#custom-handle2",
        "txtBufferRadius_A": "#startBuffer",
        "btnComputeFtth": "#btnComputeFtth",
        "slider_Ftth": "#sliderFtth",//new
        "sliderHandle_Ftth": "#custom-handleFtth",//new
        "txtBufferRadius_Ftth": "#ftthBuffer",//new
        "ftthStartLatLng": "#FtthLatLng",//new
        "ftthLat": "#FtthLat",//new
        "ftthLng": "#FtthLng",//new
        "iconPastFeasFtth": "#iconPastFeasFtth",//new
        "btnResetFtth": "#btnResetFtth",
        "txtEntitySearch": "#txtEntitySearch",
        "txtBufferRadius_B": "#endBuffer",
        "end_type": { 'START': 'A', 'END': 'B', 'ftth': 'F' },
        "iconBulkFeas": "#iconBulkFeas",
        "iconBulkFeasFtth": "#iconBulkFeasFtth",
        "iconPastFeas": "#iconPastFeas",
        "LayerAccordin": ".layers h2 .lyracrdn",
        "spnLogout": ".spnLogout",
        "spnCableType": ".spnCableType",
        "logoutpannel": ".logoutpannel",
        "sidectrlright": ".sidectrlright",
        "sidectrlleft": ".sidectrl",
        "btnlist": "#btnlist",
        "insidePLine": ".insidePLine",
        "insideALine": ".insideALine",
        "outsideLine": ".outsideLine",
        "lmcLine": ".lmcLine",
        "divLegends": ".legends",
        "divCableType": ".divCableType",
        "closeModalPopup": "#closeModalPopup",
        "hdn_feasibility_id": "#hdn_feasibility_id"
    }

    this.authAPICredentials = {
        "Username": "avanish",
        "Password": "lepton@123"
    }

    this.SignOut = function () {
        showConfirm(MultilingualKey.Sign_Out, function () {
            window.location = baseUrl + appRoot + '/login/logout';
        });
    }

    this.initApp = function () {
        this.initGlobalSettings();
       this.LoadMap();
        this.bindEvents();
        this.initSliders();
        this.initBulkFeasPopup();
        this.initPastFeasPopup();
        this.initToggleBaseMapPopup();
        this.disableBufferRadius(true, 'A');
        this.disableBufferRadius(true, 'B');
        this.disableBufferRadius(true, 'F');
        app.geomTypes.push('Polygon');
        app.geomTypes.push('Line');
        app.geomTypes.push('Point');
        $(app.DE.insidePLine).css('color', app.color_inside_P);
        $(app.DE.insideALine).css('color', app.color_inside_A);
        $(app.DE.outsideLine).css('color', app.color_Outside);
        $(app.DE.lmcLine).css('color', app.color_LMC);

    }

    this.initGlobalSettings = function () {
        app.mapServers.push($("#hdnMapServerURL").val());
        app.mapDirPath = $("#hdnMapDirPath").val();
        app.color_inside = $("#hdn_color_inside").val();
        app.color_inside_A = $("#hdn_color_inside_A").val();
        app.color_inside_P = $("#hdn_color_inside_P").val();
        app.color_Outside = $("#hdn_color_Outside").val();
        app.color_LMC = $("#hdn_color_LMC_Start").val();
        app.DEMO_VERSION = $("#hdnDemoVersion").val();
        app.ROUTING_API_URL = $("#hdnRoutingAPIUrl").val();
        app.DE.selectedColor = app.color_Outside;
    }

    this.initSliders = function () {
        var handle1 = $(app.DE.sliderHandle_A);
        $(app.DE.slider_A).slider({
            max: app.sliderDefaults.MAX,
            min: app.sliderDefaults.MIN,
            value: app.sliderDefaults.VALUE,
            step: app.sliderDefaults.STEP,
            create: function () {
                handle1.text($(this).slider("value"));
                $(app.DE.txtBufferRadius_A).val($(this).slider("value"));
            },
            slide: function (event, ui) {
                app.updateBufferRadius(app.DE.end_type.START, ui.value);
            },
            stop: function (event, ui) {
                app.fitBoundsToBuffer();
            }
        });

        var handle2 = $(app.DE.sliderHandle_B);
        $(app.DE.slider_B).slider({
            max: app.sliderDefaults.MAX,
            min: app.sliderDefaults.MIN,
            value: app.sliderDefaults.VALUE,
            step: app.sliderDefaults.STEP,
            create: function () {
                handle2.text($(this).slider("value"));
                $(app.DE.txtBufferRadius_B).val($(this).slider("value"));
            },
            slide: function (event, ui) {
                app.updateBufferRadius(app.DE.end_type.END, ui.value);
            },
            stop: function (event, ui) {
                app.fitBoundsToBuffer();
            }
        });

        var handleftth = $(app.DE.sliderHandle_Ftth);
        $(app.DE.slider_Ftth).slider({
            
            max: app.sliderDefaults_Ftth.MAX,
            min: 50,
            value: 50,
            step: app.sliderDefaults_Ftth.STEP,
            create: function () {
                handleftth.text($(this).slider("value"));
                $(app.DE.txtBufferRadius_Ftth).val($(this).slider("value"));
            },
            slide: function (event, ui) {
                var cradius = ui.value / 1000;
                fadeOuter(sf.map.getCenter(), cradius);
                if (app.polylines.length) {
                    for (var i = 0; i < app.polylines.length; i++) {
                        app.polylines[i].setMap(null);
                    }
                }

               
                app.updateBufferRadius(app.DE.end_type.ftth, ui.value);//RadiusWidget_Ftth

              

            },
          
            stop: function (event, ui) {
               
                var cradius = ui.value / 1000;
                fadeOuter(sf.map.getCenter(), cradius);
                if (app.polylines.length) {
                    for (var i = 0; i < app.polylines.length; i++) {
                        app.polylines[i].setMap(null);
                    }
                }
             
                $(".details-menu").remove();
                if (app.distanceWidget) {
                   
                    getNEDetails(app.distanceWidget);
                }
                app.updateBufferRadius(app.DE.end_type.ftth, ui.value);//RadiusWidget_Ftth

                getNEDetails(app.distanceWidget);
               // setPopupContent('Showing items within <b>' + (ui.value + ' mtr</b> <br /> Drag to change circle radius'));
            }
        });
    }

    this.initBulkFeasPopup = function () {
        app.bulkFeasibility_popUp = new ModalPopUp();
        if (app.bulkFeasibility_popUp) {
            app.bulkFeasibility_popUp.InitPopUp();
        }
    }

    this.initPastFeasPopup = function () {
        app.pastFeasibility_popUp = new ModalPopUp();
        if (app.pastFeasibility_popUp) {
            app.pastFeasibility_popUp.InitPopUp();
        }
    }

    this.initToggleBaseMapPopup = function () {
        app.toggleBaseMap_popUp = new ModalPopUp();
        if (app.toggleBaseMap_popUp) {
            app.toggleBaseMap_popUp.InitPopUp();
        }
    }

    this.roundNumber = function (num, dec) {
        var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec) + '';
        var r = result.indexOf('.');
        return result;
    }

    this.getNewDistanceWidget = function (radius, center, marker) {
        return new DistanceWidget({
            map: app.map,
            distance: radius,
            center: center,
            marker: marker
        });
    }
    this.getNewDistanceWidget_ftth = function (radius, center, marker) {
        return new DistanceWidget_FTTH({
            map: sf.map,
            distance: radius, // Starting distance in km.
            maxDistance: 1, // Twitter has a max distance of 2500km.
            color: '#3a79c8',
            activeColor: '#3a79c8',
            marker: marker
            //sizerIcon: {
            //    url: baseUrl + appRoot + 'Content/Images/resize.png',
            //    size: new google.maps.Size(24, 24),
            //    origin: new google.maps.Point(0, 0),
            //    anchor: new google.maps.Point(12, 12)
            //},
            //icon: {
            //    url: baseUrl + appRoot + 'Content/Images/center.png',
            //    size: new google.maps.Size(21, 21),
            //    origin: new google.maps.Point(0, 0),
            //    anchor: new google.maps.Point(10, 10)
            //}
        });
    }

    this.initDistanceWidget_A = function (radius, center, marker) {
        if (app.distanceWidget_A) {
            app.distanceWidget_A.set("map", null);
            app.distanceWidget_A = null;
        }

        app.distanceWidget_A = app.getNewDistanceWidget(radius, center, marker);
    }

    this.initDistanceWidget_B = function (radius, center, marker) {
        if (app.distanceWidget_B) {
            app.distanceWidget_B.set("map", null);
            app.distanceWidget_B = null;
        }

        app.distanceWidget_B = app.getNewDistanceWidget(radius, center, marker);
    }

    this.initDistanceWidget_Ftth = function (radius, center, marker) {
        if (app.distanceWidget_Ftth) {
            app.distanceWidget_Ftth.set("map", null);
            app.distanceWidget_Ftth = null;
        }
        //startWidget();
        app.distanceWidget_Ftth = app.getNewDistanceWidget_ftth(radius, center, marker);
    }

    this.addDistanceBuffer_A = function (radius, center, marker) {
        app.initDistanceWidget_A(radius, center, marker);
    }

    this.addDistanceBuffer_B = function (radius, center, marker) {
        app.initDistanceWidget_B(radius, center, marker);
    }

    this.addDistanceBuffer_Ftth = function (radius, center, marker) {
        app.initDistanceWidget_Ftth(radius, center, marker);
    }

    this.LoadMap = function () {
        google.maps.visualRefresh = true;
        var DefaultMapZoomLevel = parseInt($('#hdnDefaultMapZoomLevel').val());
        var CountryCenterlatLong = $('#hdnCountryCenterLatLong').val().split(',');
        var mapSettings
        if (CountryCenterlatLong.length == 2) {
            mapSettings = { Zoom: DefaultMapZoomLevel, lat: CountryCenterlatLong[0], lng: CountryCenterlatLong[1] };
            var center_pt = new google.maps.LatLng(mapSettings.lat, mapSettings.lng);
            var mapOptions = {
                zoom: mapSettings.Zoom,
                center: center_pt,
                minZoom: 5,
                maxZoom: 21,
                mapTypeControl: false,
                scaleControl: true,
                clickableIcons: false,
                rotateControl: false,
                styles: [{ featureType: 'poi.business', stylers: [{ visibility: 'on' }] }]
            };
            app.map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
            app.map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
            app.map.setTilt(0);
            app.initilizeOverlay(); //for getting projection required for point to LatLong Conversion
            app.ShowMapZoomLevelInFooter();//Set map scale
            app.map.enableKeyDragZoom({
                key: 'shift',
                visualEnabled: true,
                visualSprite: window.location.protocol + "//maps.gstatic.com/mapfiles/ftr/controls/dragzoom_btn.png",
                visualPosition: google.maps.ControlPosition.RIGHT_BOTTOM,
                visualPositionOffset: new google.maps.Size(20, 20, 20, 20),
                boxStyle: {
                    border: "2px solid #DE4949",
                    backgroundColor: "transparent"
                },
                veilStyle: {
                    backgroundColor: "rgba(0,0,0,0.7)"
                }
            });
            var service = new google.maps.places.AutocompleteService();
        }
        
       

        //Autocomplete function...



        $("#txtGoogleSearch").autocomplete({

            source: function (request, response) {
                var gcFlag = true;
                var sepVals = request.term.split(',');
                if (sepVals.length == 2) {
                    var lat = parseFloat(sepVals[0]);
                    var lng = parseFloat(sepVals[1]);

                    if (!isNaN(lat) && !isNaN(lng))
                        gcFlag = false;
                    if (lat > -90 && lat < 90 && lng > -180 && lng < 180) {
                        $("#txtGoogleSearch").removeClass('ui-autocomplete-loading');
                        $("#ui-id-1").removeClass('ui-autocomplete');
                        $(document).on('keypress', function (e) {
                            if (e.which == 13) {
                                var addrLocation = new google.maps.LatLng(lat, lng);
                                app.map.setCenter(addrLocation);
                                app.map.setZoom(18);
                                app.showTempDownArrow(addrLocation, true);
                                $("#txtGoogleSearch").removeClass('ui-autocomplete-loading');
                            }
                        });

                    }
                }
                if (gcFlag) {


                    $("#ui-id-1").addClass('ui-autocomplete');

                    service.getQueryPredictions({ input: request.term, fields: ['formatted_address', 'geometry', 'icon', 'name', 'permanently_closed', 'place_id', 'plus_code', 'types'] },
                        function (predictions, status) {

                            var result = [];
                            if (status == google.maps.places.PlacesServiceStatus.OK) {
                                if (predictions.length == 0) {
                                    result.push({ label: 'No match Found' });
                                }
                                else {

                                    predictions.forEach(function (prediction) {
                                        result.push({ label: truncateText(prediction.description), value: prediction.description, placeId: prediction.place_id });
                                        $("#txtGoogleSearch").removeClass('ui-autocomplete-loading');
                                    })
                                }
                            }
                            else if (status = "ZERO_RESULTS") { result.push({ label: 'No match Found' }); $("#txtGoogleSearch").removeClass('ui-autocomplete-loading'); }
                            else {
                                result.push({ label: status });
                            }
                            if (!isFtth)
                                $(".pac-container").hide();
                            response(result);

                        }
                    );
                }


            },
            minLength: 3, // minimum 3 character to hit api
            delay: 1000,// 1sec delay to hit api
            select: function (event, ui) {
                //alert("Selected: " + ui.item.placeId);
                if (ui.item.label == 'No match Found') {
                    event.preventDefault();
                }
                else {

                    var geocoder = new google.maps.Geocoder();
                    geocoder.geocode({ 'placeId': ui.item.placeId }, function (results, status) {
                        if (status == google.maps.GeocoderStatus.OK) {
                            var addrLocation = results[0].geometry.location;
                            app.map.setCenter(addrLocation);
                            app.map.setZoom(18);
                            app.showTempDownArrow(addrLocation, true);
                            $("#txtGoogleSearch").removeClass('ui-autocomplete-loading');
                        }
                    });
                }

            }

        });

        //new code section

        this.populateInputFieldsFtth = function (data) {

            var startLat = data.lat;
            var startLng = data.lng;
            $(app.DE.ftthLat).val(startLat);
            $(app.DE.ftthLng).val(startLng);
            $(app.DE.txtBufferRadius_Ftth).val(data.buffer_radius);
            $(app.DE.slider_Ftth).slider('value', data.buffer_radius);
            $(app.DE.sliderHandle_Ftth).text(data.buffer_radius);
        }

        $(app.DE.txtEntitySearch).on('keyup', function (e) {
          
            app.loadSearchEngine();
        });

        this.loadSearchEngine = function () {
            $(app.DE.txtEntitySearch).autocomplete({

                source: function (request, response) {
                    var res = ajaxReq('main/GetEntitySearchResult', { SearchText: request.term }, true, function (data) {
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
                                    label: item.label, value: item.label, geomType: item.geomType, entityName: item.entityName, entityTitle: item.entityTitle, systemID: item.systemId, status: item.status, entityId: item.entityID
                                }
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
                            $(app.DE.txtEntitySearch).val(ui.item.entityTitle + ':' + ui.item.label);
                            app.ShowEntityOnMap(ui.item.systemID, ui.item.entityName, ui.item.geomType);
                        }
                        else {
                            $(app.DE.txtEntitySearch).val(ui.item.label + ':');
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
            ajaxReq('main/getGeometryDetail', { systemId: systemID, geomType: gType, entityType: eType }, false, function (resp) {
               
                if (resp.status = 'OK') {
                    if (resp.result != null && resp.result != undefined) {
                        app.HighlightEntityOnMap(gType, resp.result);
                        //app.printPolygonEntityArea(resp.result);
                        app.fitElementOnMap(resp.result)
                    }
                }
            }, true, false);
        }

        this.fitElementOnMap = function (elemntExtant) {
            var southWest = new google.maps.LatLng(elemntExtant.southWest["Lat"], elemntExtant.southWest["Long"]);
            var northEast = new google.maps.LatLng(elemntExtant.northEast["Lat"], elemntExtant.northEast["Long"]);
            var bounds = new google.maps.LatLngBounds(southWest, northEast);
            app.map.fitBounds(bounds);
        }

        this.HighlightEntityOnMap = function (geomType, jResp) {
            if (app.gMapObj.entitySrchObj) {
                if (app.gMapObj.entitySrchObj.map)
                    app.gMapObj.entitySrchObj.setMap(null);
            }
            var latLngArr = app.getLatLongArr(jResp.sp_geometry);
            switch (geomType) {
                case 'Point':
                    app.gMapObj.entitySrchObj = app.createMarker(latLngArr[0], 'Content/images/dwnArrow.png');
                    app.gMapObj.entitySrchObj.setAnimation(google.maps.Animation.BOUNCE);
                    app.gMapObj.entitySrchObj.setDraggable(false);
                    break;
                case 'Line':
                // PENDING : NEED TO CHECK THE SAME
                case 'Polygon':
                    app.gMapObj.entitySrchObj = app.createLine(latLngArr);
                    var _lineIcon = [{
                        icon: {
                            path: 'M -.5,-.5 .5,-.5, .5,.5 -.5,.5',
                            fillOpacity: 1,
                            fillColor: 'blue'
                        },
                        repeat: '8px'
                    }];

                    app.gMapObj.entitySrchObj.strokeOpacity = 0;
                    app.gMapObj.entitySrchObj.strokeWeight = 4;
                    app.gMapObj.entitySrchObj.set('icons', _lineIcon);
                    setTimeout(function () { app.animateLine(app.gMapObj.entitySrchObj, 0) }, 20);
                    break;
                case 'Circle':
                    app.gMapObj.entitySrchObj = app.createLine(latLngArr); //app.createCircle(jResp.radious, app.getLatLongArr(jResp.sp_centroid)[0]);
                    var _lineIcon = [{
                        icon: {
                            path: 'M -.5,-.5 .5,-.5, .5,.5 -.5,.5',
                            fillOpacity: 1,
                            fillColor: 'blue'
                        },
                        repeat: '8px'
                    }];

                    app.gMapObj.entitySrchObj.strokeOpacity = 0;
                    app.gMapObj.entitySrchObj.strokeWeight = 4;
                    app.gMapObj.entitySrchObj.set('icons', _lineIcon);
                    setTimeout(function () { app.animateLine(app.gMapObj.entitySrchObj, 0) }, 20);
                    break;
            }
            app.gMapObj.entitySrchObj.setMap(app.map);
            window.setTimeout(function () { if (app.gMapObj.entitySrchObj) { app.gMapObj.entitySrchObj.setMap(null) } app.gMapObj.entitySrchObj = null; }, 25000);
        }

        this.createLine = function (_path, strokeWdth) {      
            strokeWdth = (strokeWdth == undefined || strokeWdth == null || strokeWdth == '' || strokeWdth == 0) ? 2 : strokeWdth * 10;
            var tmpLine = new google.maps.Polyline({
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                path: _path,
                //editable: true
            });
            ShowLineLength(_path);
            return tmpLine;
        }
        function ShowLineLength(_path) {      
            var arrLinePath = [];
            var arrLast2Points = [];
            if (sf.gMapObj.lineflag === !0 && sf.gMapObj.lineflag) {
                arrLinePath = _path || sf.gMapObj.libPath.slice();
                // distance b/w last 2 vertax..
                arrLast2Points = arrLinePath.slice(Math.max(arrLinePath.length - 2, 0));
                $('#lengthAreaDiv').html('<div class="bottom_line">&nbsp;</div><div class="bottom_txt"><b>Length :</b> <span>' + showFooterDist(arrLast2Points) + ' / Total:' + showFooterDist(arrLinePath) + '</span></div>');
            }
        }
        this.animateLine = function (_lineObj, offset) {
            if (_lineObj) {
                if (_lineObj.map) {
                    var icons = _lineObj.get('icons');
                    if (icons) {
                        if (offset == 5)
                            offset = 0;
                        else
                            offset++;
                        icons[0].offset = offset + 'px';
                        _lineObj.set('icons', icons);
                        setTimeout(function () { app.animateLine(_lineObj, offset) }, 50);
                    }
                }
            }
        }

        this.GeoCodeIt = function (address) {
            var gcFlag = true;
            var sepVals = address.split(',');
            if (sepVals.length == 2) {
                var lat = parseFloat(sepVals[0]);
                var lng = parseFloat(sepVals[1]);

                if (!isNaN(lat) && !isNaN(lng))
                    gcFlag = false;

                if (lat > -90 && lat < 90 && lng > -180 && lng < 180) {
                    var addrLocation = new google.maps.LatLng(lat, lng);
                    sf.map.setCenter(addrLocation);
                    sf.map.setZoom(17);
                    startWidget();
                    // $('.modal-title').text(address);
                    toggleSearch();
                }

                if (gcFlag)
                    app.geocoder.geocode({ 'address': address }, function (results, status) {
                        if (status == google.maps.GeocoderStatus.OK) {
                            var addrLocation = results[0].geometry.location;
                            sf.map.setCenter(addrLocation);
                            sf.map.setZoom(17);
                            startWidget();
                            //  $('.modal-title').text(results[0].formatted_address);
                            toggleSearch();
                        }
                    });
            }
        }



        this.getFeasibilityDetailsFtth = function () {
            var pathdist = app.NeDeviceDist;
            if (app.NeDeviceDist.split(' ')[1] == "m") {
                pathdist = parseInt(app.NeDeviceDist.split(' ')[0]) / 1000 + " km";
            }
            var feasibilityDetailsFtthInfo = {
                "lat": app.DMStoDD($("#FtthLat").val()),
                "lng": app.DMStoDD($("#FtthLng").val()),
                "buffer_radius": $(app.DE.txtBufferRadius_Ftth).val(),
                "path_geometry": app.FtthLatLng,
                "entity_id": app.ftthEntityId,
                "path_distance": pathdist,
                "entity_location": app.neElmLoc.lat() + "," + app.neElmLoc.lng()
            };
            // feasibilityDetailsInfo.lstFeasibilityDetails = app.feasibilityDetailsInfo;
            // feasibilityDetailsInfo.lstFeasibilityGeometry = app.feasibilityGeometry;
            /*app.feasibilityGeometry.length = app.feasiblePaths.length;
            for (var i = 0; i < app.feasiblePaths.length; i++) {
                for (subPaths of app.feasiblePaths[i]) {
                    subPaths.setMap(null);
                }
            }
            */

            popup.LoadModalDialog('ModalPopUp_SaveFtth', 'feasibilitydetails/Ftth', { 'feasibilityDetailsFtthInfo': feasibilityDetailsFtthInfo }, 'Feasibility Details Ftth', 'modal-xxl');
        }
        //this.fillFeasibilityDetailsFtth = function (totalLength) {
        //    // $($('.libTabs ul li a')[1]).trigger('click');
        //    $("#feasibilitybtnDv").css("display", "block");
        //    $("#totalLength").val(totalLength);
        //    $("#ExistingLength_P").val(ExistingLength_P);
        //    $("#ExistingLength_A").val(ExistingLength_A);
        //    $("#NewOutside_A_Length").val(NewOutside_A_Length);
        //    $("#NewOutside_B_Length").val(NewOutside_B_Length);
        //    $("#lmc_A_End_Path").val(lmc_A_End_Path);
        //    $("#lmc_B_End_Path").val(lmc_B_End_Path);
        //    $("#buffer_radius_a").val(buffer_radius_a);
        //    $("#buffer_radius_b").val(buffer_radius_b);

        //    $("#cableType_id").val(parseInt($("#cableType option:selected").val()));
        //    if (bCalcGeom) {
        //        // app.getCableGeometry(selected_id);

        //    }
        //    else {
        //        $.ajax({
        //            type: "POST",
        //            url: "feasibilitydetails/getfeasibilityHistoryDetails",
        //            data: JSON.stringify({ 'selectedFeasibilityID': selected_id }),
        //            contentType: "application/json; charset=utf-8",
        //            dataType: JSON,
        //            success: function (response) {
        //                //alert("Hello:");
        //                response = JSON.parse(response.responseText);
        //                $("#feasibility_id").val(response.feasibility_name);
        //                $("#customer_name").val(response.customer_name);
        //                $("#customer_id").val(response.customer_id);
        //                $("#cores_free").val(response.cores_required);
        //                $("#start_lat").val(response.start_lat_lng);
        //                $("#end_lat").val(response.end_lat_lng);
        //                // $("#feasibilitydetailsDv").html(response);
        //            },
        //            failure: function (response) {

        //                alert(response.responseText);
        //            },
        //            error: function (response) {
        //                response = JSON.parse(response.responseText);
        //                $("#feasibility_id").val(response.feasibility_name);
        //                $("#customer_name").val(response.customer_name);
        //                $("#customer_id").val(response.customer_id);
        //                $("#cores_free").val(response.cores_required);
        //                $("#start_lat").val(response.start_lat_lng);
        //                $("#end_lat").val(response.end_lat_lng);
        //                // alert(response.responseText);
        //            }
        //        });
        //    }
        //}








        //end new code section


        //$("#txtGoogleSearch").autocomplete({
        //    source: function (request, response) {
        //        service.getQueryPredictions({ input: request.term, fields: ['formatted_address', 'geometry', 'icon', 'name', 'permanently_closed', 'place_id', 'plus_code', 'types'] },
        //            function (predictions, status) {
        //                var result = [];
        //                if (status == google.maps.places.PlacesServiceStatus.OK) {
        //                    if (predictions.length == 0) {
        //                        result.push({ label: 'No match Found' });
        //                    }
        //                    else {
        //                        predictions.forEach(function (prediction) {
        //                            result.push({ label: truncateText(prediction.description), value: prediction.description, placeId: prediction.place_id });
        //                        })
        //                    }
        //                }
        //                else if (status = "ZERO_RESULTS") { result.push({ label: 'No match Found' }); }
        //                else {
        //                    result.push({ label: status });
        //                }
        //                response(result);
        //            }
        //            );
        //    },
        //    minLength: 3, // minimum 3 character to hit api
        //    delay: 1000,// 1sec delay to hit api
        //    select: function (event, ui) {
        //        //alert("Selected: " + ui.item.placeId);
        //        if (ui.item.label == 'No match Found') {
        //            event.preventDefault();
        //        }
        //        else {
        //            var geocoder = new google.maps.Geocoder();
        //            geocoder.geocode({ 'placeId': ui.item.placeId }, function (results, status) {
        //                if (status == google.maps.GeocoderStatus.OK) {
        //                    var addrLocation = results[0].geometry.location;
        //                    app.map.setCenter(addrLocation);
        //                    app.map.setZoom(18);
        //                    app.showTempDownArrow(addrLocation, true);
        //                }
        //            });
        //        }

        //    }
        //});

        //var input = document.getElementById(app.DE.gSearchTxtBx);
        //var autocomplete = new google.maps.places.Autocomplete(input);
        //autocomplete.bindTo('bounds', app.map);
        //google.maps.event.addListener(autocomplete, 'place_changed', function () {
        //    var place = autocomplete.getPlace();
        //    if (place.geometry) {
        //        // If the place has a geometry, then present it on a map.
        //        if (place.geometry.viewport) {
        //            app.showTempDownArrow(place.geometry.location, true);
        //            app.map.fitBounds(place.geometry.viewport);
        //        } else {
        //            app.showTempDownArrow(place.geometry.location, true);
        //            app.map.setCenter(place.geometry.location);
        //            app.map.setZoom(17);  // Why 17? Because it looks good.
        //        }
        //    }
        //});

        google.maps.event.addListener(app.map, 'mousemove', function (event) {
            app.ShowMapCordinateInFooter(event.latLng);

        });
        google.maps.event.addListener(app.map, 'zoom_changed', function (event) {
            var _zoom = app.map.getZoom();
            if (_zoom > 21) _zoom = 21;
            if (_zoom < 5) _zoom = 5;
            $(app.DE.mapScale).text(_zoom);

            //SetMapScaleRatio(_zoom);//// Zoom scale ratio
            //app.ManageGoogleElementLanguage();
            //PREVENT MAP SERVER HITS WHILE ZOOMING- START 2
            checkLastZoom();
            if (_zoom != lastZoomNo) {
                zoomDiffSec = ((new Date() - lastZoomTime) / 1000)
                lastZoomTime = new Date();
                isZoomChanged = true;
            }
            //PREVENT MAP SERVER HITS WHILE ZOOMING- END 2

        });

        window.addEventListener('load', function (event) {
            //set coordinates on map load...
            //app.ShowMapCordinateInFooter(app.map.getCenter());
            //app.ShowMapZoomLevelInFooter();
        });

        google.maps.event.addListener(app.map, 'bounds_changed', function () {
            //app.setMapCordinates(app.map);
        });

        app.directionsService = new google.maps.DirectionsService();
        app.infowindow = new google.maps.InfoWindow();
        app.editMenu = new EditMenu();
        app.detailsMenu = new DetailsMenu();
        //directionsDisplay = new google.maps.DirectionsRenderer({ draggable: true, polylineOptions: { strokeColor: '#FF8800' } });
        //directionsService = new google.maps.DirectionsService();
        //google.maps.event.addListener(directionsDisplay, 'directions_changed', function () {
        //    app.computeTotalDistance(directionsDisplay.directions);
        //});


        $('#map_canvas').append('<div class="dvToggleMap" onclick="sf.toggleBaseMap()" title="Toggle Base Map"></div>');
        //$('#map_canvas').append('<div class="dvInfoMapAddress" title="" ><span id="spInfoMapAddress" ></span><span class="closeInfoMapAddress" title="Close">x</span></div>')
    }

    this.showTempDownArrow = function (position, autohide) {
        app.clearTempSrchObj();
        app.gMapObj.tempSrchObj = app.createMarker(position, 'Content/images/dwnArrow.png');
        app.gMapObj.tempSrchObj.setAnimation(google.maps.Animation.BOUNCE);
        app.gMapObj.tempSrchObj.setMap(app.map);
        if (autohide)
            window.setTimeout(function () { app.gMapObj.tempSrchObj.setMap(null) }, 25000);
    }

    this.clearTempSrchObj = function () {
        if (app.gMapObj.tempSrchObj && app.gMapObj.tempSrchObj.map)
            app.gMapObj.tempSrchObj.setMap(null)
    }

    this.fillLocationftth = function (LatLong, end) {
        if (end == app.DE.end_type.ftth) {
            var latitude;
            var longitude;
            if ($(app.DE.ftthLat).val() != "")
                latitude = $(app.DE.ftthLat).val();
            else {
                latitude = LatLong.latLng.lat();
                $(app.DE.ftthLat).val(latitude.toFixed(6));
                $(app.DE.ftthLng).val(longitude.toFixed(6));
            }
            if ($(app.DE.ftthLng).val() != "")
                longitude = $(app.DE.ftthLng).val();
            else {
                longitude = LatLong.latLng.lng();
                $(app.DE.ftthLat).val(latitude.toFixed(6));
                $(app.DE.ftthLng).val(longitude.toFixed(6));
            }
            //$("#txtGoogleSearch").val($(app.DE.ftthLat).val() + ',' + $(app.DE.ftthLng).val());
            app.GeoCodeIt($(app.DE.ftthLat).val() + ',' + $(app.DE.ftthLng).val());
            $(app.DE.ftthLat).parent().removeClass("has-error");
            $(app.DE.ftthLng).removeClass("input-validation-error");
            $(app.DE.ftthLat).parent().removeClass("has-error");
            $(app.DE.ftthLng).removeClass("input-validation-error");
            app.disableBufferRadius(false, app.DE.end_type.ftth);
            var handleftth = $(app.DE.txtBufferRadius_Ftth).val();
            var cradius = handleftth / 1000;
            fadeOuter(sf.map.getCenter(), cradius);
            if (app.polylines.length) {
                for (var i = 0; i < app.polylines.length; i++) {
                    app.polylines[i].setMap(null);
                }
            }
            $(".details-menu").remove();
            if (app.distanceWidget) {

                getNEDetails(app.distanceWidget);
            }
            app.updateBufferRadius(app.DE.end_type.ftth, handleftth);//RadiusWidget_Ftth
            getNEDetails(app.distanceWidget);
        }

    }
    this.fillLocation = function (LatLong, end) {
        if (end == app.DE.end_type.START) {
            app.startLatLng.lat = LatLong.latLng.lat();
            app.startLatLng.lng = LatLong.latLng.lng();
            $(app.DE.txtStartLat).val(app.startLatLng.lat.toFixed(6));
            $(app.DE.txtStartLng).val(app.startLatLng.lng.toFixed(6));
            $(app.DE.txtStartLat).parent().removeClass("has-error");
            $(app.DE.txtStartLat).removeClass("input-validation-error");
            $(app.DE.txtStartLng).parent().removeClass("has-error");
            $(app.DE.txtStartLng).removeClass("input-validation-error");         
           app.updateBufferRadius(app.DE.end_type.START, $(app.DE.txtBufferRadius_A).val())
            app.disableBufferRadius(false, app.DE.end_type.START);
        }
        else if (end == app.DE.end_type.END) {
            app.endLatLng.lat = LatLong.latLng.lat();
            app.endLatLng.lng = LatLong.latLng.lng();
            $(app.DE.txtEndLat).val(app.endLatLng.lat.toFixed(6));
            $(app.DE.txtEndLng).val(app.endLatLng.lng.toFixed(6));
            $(app.DE.txtEndLat).parent().removeClass("has-error");
            $(app.DE.txtEndLat).removeClass("input-validation-error");
            $(app.DE.txtEndLng).parent().removeClass("has-error");
            $(app.DE.txtEndLng).removeClass("input-validation-error");
           
            app.updateBufferRadius(app.DE.end_type.END, $(app.DE.txtBufferRadius_B).val())
            app.disableBufferRadius(false, app.DE.end_type.END);
        }

        else if (end == app.DE.end_type.ftth) {
            
            var latitude = LatLong.latLng.lat();
            var longitude= LatLong.latLng.lng();
            $(app.DE.ftthLat).val(latitude.toFixed(6));
            $(app.DE.ftthLng).val(longitude.toFixed(6));            
            //$("#txtGoogleSearch").val($(app.DE.ftthLat).val() + ',' + $(app.DE.ftthLng).val());
            app.GeoCodeIt($(app.DE.ftthLat).val() + ',' + $(app.DE.ftthLng).val());
            $(app.DE.ftthLat).parent().removeClass("has-error");
            $(app.DE.ftthLng).removeClass("input-validation-error");
            $(app.DE.ftthLat).parent().removeClass("has-error");
            $(app.DE.ftthLng).removeClass("input-validation-error");
            app.disableBufferRadius(false, app.DE.end_type.ftth);
            app.updateBufferRadius(app.DE.end_type.ftth, '50')
            app.updateBufferRadius(app.DE.end_type.ftth, $(app.DE.txtBufferRadius_Ftth).val())
        }
    }

    this.initilizeOverlay = function () {
        app.overlay = new google.maps.OverlayView();
        app.overlay.draw = function () { };
        app.overlay.setMap(app.map);
    }

    this.DDtoDMS = function (dd) {
        var degrees = parseInt(dd);
        var minutes = (dd - degrees) * 60;
        var seconds = minutes - parseInt(minutes);
        minutes = parseInt(minutes);
        seconds = seconds * 60;
        var dms = degrees.toString() + '°' + minutes.toString() + '\'' + app.roundNumber(seconds, 2).toString() + '"';
        return dms;
    }

    this.ShowMapCordinateInFooter = function (latLng) {
        $(app.DE.spnCoords).html(app.roundNumber(latLng.lat(), 5) + "," + app.roundNumber(latLng.lng(), 5));
    }

    this.ShowMapZoomLevelInFooter = function () {
        var _zoom = app.map.getZoom();
        if (_zoom > 21) _zoom = 21;
        if (_zoom < 5) _zoom = 5;
        $(app.DE.mapScale).text(_zoom);
    }

    this.setMapCordinates = function (map) {
        var cordinates = map ? map.getBounds() : undefined;
        var ne = cordinates ? cordinates.getNorthEast() : undefined;
        var sw = cordinates ? cordinates.getSouthWest() : undefined;

        var cord = (ne && sw) ? sw.lng() + "," + sw.lat() + "," + ne.lng() + "," + ne.lat() : undefined;

        if ($(app.DE.hdnMapCordinates) && cord) {
            $(app.DE.hdnMapCordinates).val(cord);
        }
    }

    this.createMarker = function (mrkrLatlng, imageUrl, label) {
        var gmarkernew = new google.maps.Marker({
            position: mrkrLatlng,
            icon: imageUrl,
            draggable: true,
            title: ((label == 'A') ? 'Start Point' : 'End Point'),
            //label: label,
            map: app.map
        });
      
        return gmarkernew;
    }

    this.clearLocationObjects = function () {
        app.map.setOptions({ draggableCursor: '' });
        app.removeEventListeners('click');
    }

    this.removeEventListeners = function (evt) {
        google.maps.event.clearListeners(app.map, evt);
    }


  

    this.selectLocation = function (end) { 
        app.map.setOptions({ draggableCursor: 'crosshair' });
        google.maps.event.clearListeners(app.map, 'click');
        google.maps.event.addListener(app.map, 'click', function (LatLong) {
             
            app.clearLocationObjects();
            var currentMarker = undefined;
            if (end == 'A') {
              
                if (app.startMarker)
                    app.startMarker.setMap(null);
                app.startMarker = app.createMarker(LatLong.latLng, 'Content/images/StartPoint.png', end);
                app.startMarker.addListener('drag', function (LatLong) {
                    app.fillLocation(LatLong, end);
                   
                });
                app.addDistanceBuffer_A(app.sliderDefaults.VALUE / 1000, LatLong.latLng, app.startMarker);
                $(app.DE.selectStartLatLng).css('color', '');
                app.startMarker.addListener('dragend', function () {
                    app.fitBoundsToBuffer();
                });
                app.fitBoundsToBuffer();
            
            }
            else if (end == 'B') {
                if (app.endMarker)
                    app.endMarker.setMap(null);
                app.endMarker = app.createMarker(LatLong.latLng, 'Content/images/EndPoint.png', end);
                app.endMarker.addListener('drag', function (LatLong) {
                    app.fillLocation(LatLong, end);
                    app.fitBoundsToBuffer();
                });

                app.addDistanceBuffer_B(app.sliderDefaults.VALUE / 1000, LatLong.latLng, app.endMarker);
                $(app.DE.selectEndLatLng).css('color', '');
                app.endMarker.addListener('dragend', function () {
                    app.fitBoundsToBuffer();
                });

                app.fitBoundsToBuffer();

            }

            else if (end == 'F') {


                //if (app.FtthMarker)
                //    app.FtthMarker.setMap(null);
                //   app.FtthMarker = app.createMarker(LatLong.latLng, 'Content/images/center.png', end);
                //app.FtthMarker.addListener('drag', function (LatLong) {
                //    app.fillLocation(LatLong, end);
                //});

                // app.addDistanceBuffer_Ftth(app.sliderDefaults.VALUE / 1000, LatLong.latLng, app.endMarker);
                $(app.DE.ftthStartLatLng).css('color', '');
            }

            app.fillLocation(LatLong, end);
        });
    }

    this.moveMarker = function (end) {    
        if (end == 'A' && app.startLatLng.lat && app.startLatLng.lng) {
            var latlng = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
            if (app.startMarker)
                app.startMarker.setPosition(latlng);
            else {
                app.startMarker = app.createMarker(latlng, 'Content/images/StartPoint.png', end);
                app.startMarker.addListener('drag', function (LatLong) {
                    app.fillLocation(LatLong, end);
                });
                app.addDistanceBuffer_A(app.sliderDefaults.VALUE / 1000, latlng, app.startMarker);
                app.disableBufferRadius(false, app.DE.end_type.START);

            }
        }
        else if (end == 'B' && app.endLatLng.lat && app.endLatLng.lng) {
            var latlng = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
            if (app.endMarker)
                app.endMarker.setPosition(latlng);
            else {
                app.endMarker = app.createMarker(latlng, 'Content/images/EndPoint.png', end);
                app.endMarker.addListener('drag', function (LatLong) {
                    app.fillLocation(LatLong, end);
                });
                app.addDistanceBuffer_B(app.sliderDefaults.VALUE / 1000, latlng, app.endMarker);
                app.disableBufferRadius(false, app.DE.end_type.END);
            }
        }
        app.fitBoundsToBuffer();
    }

    this.fitBoundsToBuffer = function () {

        var bounds = new google.maps.LatLngBounds();
        if (!isFtth) {
            if (app.distanceWidget_A)
                bounds.union(app.distanceWidget_A.gCircle.getBounds());
            if (app.distanceWidget_B)
                bounds.union(app.distanceWidget_B.gCircle.getBounds());
            if (app.distanceWidget_A != undefined && app.distanceWidget_B != undefined)
                app.map.fitBounds(bounds);
        }
        else {
            if (app.distanceWidget)
                bounds.union(app.distanceWidget.gCircle.getBounds());
            if (app.distanceWidget != undefined) {
                app.map.fitBounds(bounds);

            }
        }
    }

    this.setWayPoints = function (start_point, end_point) {
        app.wayPoints = [];
        if (start_point && end_point && start_point.length && end_point.length) {
            start_point = start_point.replace('POINT(', '').replace(')', '').split(' ');
            end_point = end_point.replace('POINT(', '').replace(')', '').split(' ');
            if (start_point.length == 2 && end_point.length == 2) {
                app.wayPoints = [{ location: app.gMapsLatLng(start_point[1], start_point[0]) }, { location: app.gMapsLatLng(end_point[1], end_point[0]) }];
            }
        }
    }

    this.getWaypoints = function () {
        //let pathCount = app.insidePaths.length;
        //var waypoints = [];
        //if (pathCount) {
        //    let firstPath = app.insidePaths[0];
        //    let lastPath = app.insidePaths[pathCount - 1];

        //    waypoints = [{ location: firstPath.path[0] }, { location: lastPath.path[lastPath.path.length - 1] }];
        //}
        return app.wayPoints;
    }

    this.clearAllPaths = function () {
        for (var i = 0; i < app.feasiblePaths.length; i++) {
            for (subPaths of app.feasiblePaths[i]) {
                subPaths.setMap(null);
            }
        }
        app.feasiblePaths = [];
    }

    this.getDistanceString = function (distance) {
        if (parseInt(distance) > 999)
            return (distance / 1000).toFixed(2) + ' km';
        else
            return distance.toFixed(2) + ' m';
    }

    this.selectPath = function (paths) {
        app.unselectAllPaths();

        for (path of paths) {
            if (path)
                path.setOptions({ strokeColor: app.getColorByType(path.type), zIndex: 10, isSelected: true });
        }

        app.feasibilityDetailsInformation();
    }

    this.selectShortestPath = function () {
        if (app.feasiblePaths.length) {
            var shortestIdx = 0;

            var subPaths = app.feasiblePaths[0];
            var minDistance = subPaths[0].totalDistance;

            for (var i = 1; i < app.feasiblePaths.length; i++) {
                totalDistance = 0;
                var subPaths = app.feasiblePaths[i];
                if (subPaths[i].totalDistance < minDistance) {
                    minDistance = subPaths[i].totalDistance;
                    shortestIdx = i;
                }
            }

            app.selectPath(app.feasiblePaths[shortestIdx]);
        }
    }

    this.getColorByType = function (type) {
        let clr = app.DE.dullColor;
        if (type == app.cable_types.OUTSIDE_A_END || type == app.cable_types.OUTSIDE_B_END)
            clr = app.color_Outside;
        else if (type == app.cable_types.INSIDE_P)
            clr = app.color_inside_P;
        else if (type == app.cable_types.INSIDE_A || type == app.cable_types.INSIDE)
            clr = app.color_inside_A;
        if (type == app.cable_types.LMC_A_END || type == app.cable_types.LMC_B_END)
            clr = app.color_LMC;
        return clr;
    }

    this.updateDistances = function (subPath) {
        var distances = { totalDistance: 0 };
        let paths = app.feasiblePaths[subPath.pathIndex];
        if (paths.length) {
            for (sub_path of paths) {
                if (sub_path) {
                    var length = google.maps.geometry.spherical.computeLength(sub_path.getPath());
                    sub_path.self_length = length;
                    switch (sub_path.type) {
                        case app.cable_types.INSIDE_P:
                            distances.inside_P_Distance = distances.inside_P_Distance ? distances.inside_P_Distance + length : length;
                            break;
                        case app.cable_types.INSIDE_A:
                            distances.inside_A_Distance = distances.inside_A_Distance ? distances.inside_A_Distance + length : length;
                            break;
                        case app.cable_types.INSIDE:
                            distances.insideDistance = distances.insideDistance ? distances.insideDistance + length : length;
                            break;
                        case app.cable_types.OUTSIDE_A_END:
                            distances.outside_A_Distance = distances.outside_A_Distance ? distances.outside_A_Distance + length : length;
                            break;
                        case app.cable_types.OUTSIDE_B_END:
                            distances.outside_B_Distance = distances.outside_B_Distance ? distances.outside_B_Distance + length : length;
                            break;
                        case app.cable_types.LMC_A_END:
                            distances.lmc_A_Distance = distances.lmc_A_Distance ? distances.lmc_A_Distance + length : length;
                            break;
                        case app.cable_types.LMC_B_END:
                            distances.lmc_B_Distance = distances.lmc_B_Distance ? distances.lmc_B_Distance + length : length;
                            break;
                    }
                    distances.totalDistance += length;
                }
            }

            app.FeasibilityDistances = distances;

            for (sub_path of paths) {
                if (sub_path) {
                    switch (sub_path.type) {
                        case app.cable_types.INSIDE_P:
                            sub_path.distance = distances.inside_P_Distance;
                            break;
                        case app.cable_types.INSIDE_A:
                            sub_path.distance = distances.inside_A_Distance;
                            break;
                        case app.cable_types.INSIDE:
                            sub_path.distance = distances.insideDistance;
                            break;
                        case app.cable_types.OUTSIDE_A_END:
                            sub_path.distance = distances.outside_A_Distance;
                            break;
                        case app.cable_types.OUTSIDE_B_END:
                            sub_path.distance = distances.outside_B_Distance;
                            break;
                        case app.cable_types.LMC_A_END:
                            sub_path.distance = distances.lmc_A_Distance;
                            break;
                        case app.cable_types.LMC_B_END:
                            sub_path.distance = distances.lmc_B_Distance;
                            break;
                    }
                    sub_path.totalDistance = distances.totalDistance;
                }
            }
        }
    }

    this.gMapsLatLng = function (lat, lng) {
        return new google.maps.LatLng(lat, lng);
    }

    this.getRouteLegPath = function (leg) {
        var path = [];
        let steps = leg.steps;
        if (steps.length) {
            for (step of steps) {
                path = path.concat(step.lat_lngs);
            }
        }
        return path;
    }

    this.getInsidePolylines = function () {
        let polylines = [];
        if (app.insidePaths.length) {
            for (pathObj of app.insidePaths) {
                var polyline = new google.maps.Polyline({
                    path: pathObj.path,
                    strokeColor: app.DE.dullColor,
                    strokeOpacity: 1.0,
                    strokeWeight: 5,
                    zIndex: 0,
                    isSelected: false,
                    distance: 0,
                    totalDistance: 0,
                    cable_id: pathObj.cable_id,
                    cable_name: pathObj.cable_name,
                    type: pathObj.network_status == 'P' ? app.cable_types.INSIDE_P : (pathObj.network_status == 'A' ? app.cable_types.INSIDE_A : app.cable_types.INSIDE),
                    system_id: pathObj.system_id
                    //pathIndex: i
                });
                polylines.push(polyline);
            }
        }
        return polylines;
    }

    this.setMapForAll = function () {
        var distances = { totalDistance: 0 };
        if (app.feasiblePaths.length) {
            for (f_path of app.feasiblePaths) {
                if (f_path.length) {
                    for (sub_path of f_path) {
                        if (sub_path) {
                            sub_path.setMap(app.map);   // set the map for subpath
                            var length = google.maps.geometry.spherical.computeLength(sub_path.getPath());
                            sub_path.self_length = length;
                            switch (sub_path.type) {
                                case app.cable_types.INSIDE_A:
                                    distances.inside_A_Distance = distances.inside_A_Distance ? distances.inside_A_Distance + length : length;
                                    break;
                                case app.cable_types.INSIDE_P:
                                    distances.inside_P_Distance = distances.inside_P_Distance ? distances.inside_P_Distance + length : length;
                                    break;
                                case app.cable_types.INSIDE:
                                    distances.insideDistance = distances.insideDistance ? distances.insideDistance + length : length;
                                    break;
                                case app.cable_types.OUTSIDE_A_END:
                                    distances.outside_A_Distance = distances.outside_A_Distance ? distances.outside_A_Distance + length : length;
                                    break;
                                case app.cable_types.OUTSIDE_B_END:
                                    distances.outside_B_Distance = distances.outside_B_Distance ? distances.outside_B_Distance + length : length;
                                    break;
                                case app.cable_types.LMC_A_END:
                                    distances.lmc_A_Distance = distances.lmc_A_Distance ? distances.lmc_A_Distance + length : length;
                                    break;
                                case app.cable_types.LMC_B_END:
                                    distances.lmc_B_Distance = distances.lmc_B_Distance ? distances.lmc_B_Distance + length : length;
                                    break;
                            }
                            distances.totalDistance += length;
                        }
                    }
                    //}
                    //}

                    app.FeasibilityDistances = distances;

                    // set distances
                    //for(f_path of app.feasiblePaths) {
                    //  if (f_path.length) {
                    for (sub_path of f_path) {
                        if (sub_path) {
                            switch (sub_path.type) {
                                case app.cable_types.INSIDE_A:
                                    sub_path.distance = distances.inside_A_Distance;
                                    break;
                                case app.cable_types.INSIDE_P:
                                    sub_path.distance = distances.inside_P_Distance;
                                    break;
                                case app.cable_types.INSIDE:
                                    sub_path.distance = distances.insideDistance;
                                    break;
                                case app.cable_types.OUTSIDE_A_END:
                                    sub_path.distance = distances.outside_A_Distance;
                                    break;
                                case app.cable_types.OUTSIDE_B_END:
                                    sub_path.distance = distances.outside_B_Distance;
                                    break;
                                case app.cable_types.LMC_A_END:
                                    sub_path.distance = distances.lmc_A_Distance;
                                    break;
                                case app.cable_types.LMC_B_END:
                                    sub_path.distance = distances.lmc_B_Distance;
                                    break;
                            }
                            sub_path.totalDistance = distances.totalDistance;
                        }
                    }
                }
            }
        }
    }

    this.setMapForSavedPaths = function () {
        if (app.savedPaths.length) {
            for (path of app.savedPaths) {
                if (path) {
                    path.setMap(app.map);
                }
            }
        }
    }

    this.isSameGLocation = function (loc1, loc2) {
        return loc1.lat() == loc2.lat() && loc1.lng() == loc2.lng();
    }

    this.orderTest = function () {
        if (app.insidePaths.length > 1) {
            var temp_insides = app.insidePaths;
            var ordered_insides = [];
            var nextTarget = undefined;

            for (var i = 0; i < temp_insides.length; i++) {
                // find first end point
                let curr_path = temp_insides[i].path;
                let curr_startPt = curr_path[0];
                let curr_endPt = curr_path[curr_path.length - 1];

                // check curr_startPt for end point
                let start_found = false;
                let end_found = false;
                for (var j = 0; j < temp_insides.length && !start_found; j++) {
                    if (j != i) {
                        if (app.isSameGLocation(temp_insides[j].path[0], curr_startPt) ||
                            app.isSameGLocation(temp_insides[j].path[temp_insides[j].path.length - 1], curr_startPt)) {
                            start_found = true;
                        }
                    }
                }
                // check curr_endPt for end point
                if (start_found) {
                    if (j != i) {
                        for (var j = 0; j < temp_insides.length && !end_found; j++) {
                            if (app.isSameGLocation(temp_insides[j].path[0], curr_endPt) ||
                                app.isSameGLocation(temp_insides[j].path[temp_insides[j].path.length - 1], curr_endPt)) {
                                end_found = true;
                            }
                        }
                    }
                }
                if (!start_found) {
                    ordered_insides.push(temp_insides[i]);
                    // remove path from array
                    temp_insides.splice(i, 1);
                    nextTarget = curr_endPt;
                    break;
                }
                else if (!end_found) {
                    temp_insides[i].path.reverse();
                    ordered_insides.push(temp_insides[i]);
                    // remove path from array
                    temp_insides.splice(i, 1);
                    nextTarget = curr_startPt;
                    break;
                }
            }

            while (nextTarget && temp_insides.length) {
                let tgtFound = false;
                for (var i = 0; i < temp_insides.length && !tgtFound; i++) {
                    let curr_path = temp_insides[i].path;
                    let curr_startPt = curr_path[0];
                    let curr_endPt = curr_path[curr_path.length - 1];

                    // if first point is target
                    if (app.isSameGLocation(curr_startPt, nextTarget)) {
                        ordered_insides.push(temp_insides[i]);
                        temp_insides.splice(i, 1);
                        nextTarget = curr_endPt;
                        tgtFound = true;
                    }
                    // if last point is target
                    else if (app.isSameGLocation(curr_endPt, nextTarget)) {
                        temp_insides[i].path.reverse();
                        ordered_insides.push(temp_insides[i]);
                        temp_insides.splice(i, 1);
                        nextTarget = curr_startPt;
                        tgtFound = true;
                    }
                }
            }
        }
    }

    this.orderInsidePaths = function (flag) {
        if (app.insidePaths.length > 1) {
            for (var i = 0; i < app.insidePaths.length - 1; i++) {
                let currentPath = app.insidePaths[i].path;
                let nextPath = app.insidePaths[i + 1].path;
                let currFirst = currentPath[0];
                let currLast = currentPath[currentPath.length - 1];
                let nextFirst = nextPath[0];
                let nextLast = nextPath[nextPath.length - 1];

                if (!(app.isSameGLocation(currLast, nextFirst) || app.isSameGLocation(currLast, nextLast))) {
                    // reverse the direction
                    app.insidePaths[i].path.reverse();
                }
            }

            // last path
            let lastPath = app.insidePaths[app.insidePaths.length - 1].path;
            let secondLastPath = app.insidePaths[app.insidePaths.length - 2].path;
            if (app.isSameGLocation(lastPath[lastPath.length - 1], secondLastPath[secondLastPath.length - 1])) {
                app.insidePaths[app.insidePaths.length - 1].path.reverse();
            }
        }

        // When all path-seq are same, our ordering of paths by path-seq have no impact
        // then we have to order them using nearest ends
        if (flag) {
            let path1 = app.insidePaths[0];
            let path2 = app.insidePaths[app.insidePaths.length - 1];

            let waypoints = app.getWaypoints();
            let start = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
            let end = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);

            let radius_A = parseFloat($(app.DE.txtBufferRadius_A).val());
            let radius_B = parseFloat($(app.DE.txtBufferRadius_B).val());

            if (radius_A && radius_B) {
                // distance between point A and its nearest end-point of Inside Path
                let distance_A = google.maps.geometry.spherical.computeDistanceBetween(start, path1.path[0]);
                // distance between point B and its nearest end-point of Inside Path
                let distance_B = google.maps.geometry.spherical.computeDistanceBetween(start, path2.path[path2.path.length - 1]);

                if (distance_B < distance_A) {
                    for (path of app.insidePaths) {
                        path.path.reverse();
                    }
                    app.insidePaths.reverse();
                }
            }
        }
    }

    this.updateCableDetails = function (cables) {
        app.is_core_feasibile = true;
        if (cables.length) {
            for (cable of cables) {
                var cable_id = cable.cable_id;
                if (cable_id) {
                    for (insidePath of app.insidePaths) {
                        if (insidePath && insidePath.system_id == cable_id) {
                            insidePath.cable_id = cable.network_id;
                            insidePath.cable_name = cable.cable_name;
                            insidePath.total_cores = cable.total_cores;
                            insidePath.used_cores = cable.total_cores - cable.available_cores;
                            insidePath.network_status = cable.cable_status;
                            insidePath.available_cores = cable.available_cores;
                            if (cable.available_cores < app.cores_required) {
                                app.is_core_feasibile = false;
                            }
                        }
                    }
                }
            }
        }
    }

    this.updateNetworkStatus = function () {
        let allCables = '';
        for (path of app.insidePaths) {
            allCables += path.system_id + ',';
        }

        if (allCables.length) {
            $.ajax({
                type: "POST",
                url: "Main/GetCablesDetails",
                data: JSON.stringify({ systemIds: allCables.slice(0, -1) }),
                dataType: "json",
                contentType: "application/json",
                success: function (result, status, xhr) {
                    if (status == "success") {
                        app.updateCableDetails(result.cableStatuses);
                        app.processPaths();
                    }
                },
                error: function (xhr, status, error) {
                    alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
                }
            });
        }
    }

    this.sort_by_key = function (array, key) {
        return array.sort(function (a, b) {
            var x = a[key]; var y = b[key];
            return ((x < y) ? -1 : ((x > y) ? 1 : 0));
        });
    }

    this.isSamePathSeq = function (res) {
        if (res.length) {
            let seq = res[0].path_seq;
            for (item of res) {
                if (item.path_seq != seq)
                    return false;
            }
            return true;
        }
        return false;
    }

    this.isWithinBufferRegion = function () {
        let waypoints = app.getWaypoints();
        let start = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
        let end = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);

        let radius_A = parseFloat($(app.DE.txtBufferRadius_A).val());
        let radius_B = parseFloat($(app.DE.txtBufferRadius_B).val());

        if (waypoints.length && radius_A && radius_B) {
            // distance between point A and its nearest end-point of Inside Path
            let distance_A = google.maps.geometry.spherical.computeDistanceBetween(start, waypoints[0].location);
            if (distance_A <= radius_A) {
                // distance between point B and its nearest end-point of Inside Path
                let distance_B = google.maps.geometry.spherical.computeDistanceBetween(end, waypoints[1].location);
                if (distance_B <= radius_B) {
                    return true;
                }
            }
        }

        return false;
    }
    this.DisableInputfields = function () {
        $(app.DE.txtCustomerName).attr("disabled", "disabled");;
        $(app.DE.txtCustomerID).attr("disabled", "disabled");
        $(app.DE.txtFseasibilityID).attr("disabled", "disabled");
        $(app.DE.txtStartLat).attr("disabled", "disabled");
        $(app.DE.txtStartLng).attr("disabled", "disabled");
        $(app.DE.txtBufferRadius_A).attr("disabled", "disabled");
        $(app.DE.txtEndLat).attr("disabled", "disabled");
        $(app.DE.txtEndLng).attr("disabled", "disabled");
        $(app.DE.txtStartPointName).attr("disabled", "disabled");
        $(app.DE.txtEndPointName).attr("disabled", "disabled");
        $(app.DE.txtBufferRadius_B).attr("disabled", "disabled");
        $(app.DE.txtEndLat).attr("disabled", "disabled");
        $(app.DE.txtEndLng).attr("disabled", "disabled");
        $(app.DE.txtBufferRadius_B).attr("disabled", "disabled");
        $(app.DE.coresTextBox).attr("disabled", "disabled");
        $(app.DE.cableTypeddl).attr("disabled", "disabled");
        app.disableBufferRadius(true, 'A');
        app.disableBufferRadius(true, 'B');
        app.startMarker.draggable = false;
        app.endMarker.draggable = false;
        $(app.DE.selectStartLatLng).addClass('disabled');
        $(app.DE.selectEndLatLng).addClass('disabled');
        $(app.DE.ftthStartLatLng).addClass('disabled');
    }
    this.EnableInputfields = function () {
        $(app.DE.txtCustomerName).removeAttr("disabled");
        $(app.DE.txtCustomerID).removeAttr("disabled");
        $(app.DE.txtFseasibilityID).removeAttr("disabled");
        $(app.DE.txtStartLat).removeAttr("disabled");
        $(app.DE.txtStartLng).removeAttr("disabled");
        $(app.DE.txtEndLat).removeAttr("disabled");
        $(app.DE.txtEndLng).removeAttr("disabled");
        $(app.DE.txtEndLat).removeAttr("disabled");
        $(app.DE.txtEndLng).removeAttr("disabled");
        $(app.DE.txtBufferRadius_B).removeAttr("disabled");
        $(app.DE.coresTextBox).removeAttr("disabled");
        $(app.DE.cableTypeddl).removeAttr("disabled");
        $(app.DE.txtStartPointName).removeAttr("disabled");
        $(app.DE.txtEndPointName).removeAttr("disabled");
        $(app.DE.selectStartLatLng).removeClass('disabled');
        $(app.DE.selectEndLatLng).removeClass('disabled');
        $(app.DE.ftthStartLatLng).removeClass('disabled');
    }
    this.populateInputFields = function (data) {

        var startLat = data.start_lat;
        var startLng = data.start_lng;
        var endLat = data.end_lat;
        var endLng = data.end_lng;
        $(app.DE.txtCustomerName).val(data.customer_name);
        $(app.DE.txtCustomerID).val(data.customer_id);
        $(app.DE.txtFseasibilityID).val(data.feasibility_id);
        $(app.DE.txtStartPointName).val(data.start_point_name);
        $(app.DE.txtEndPointName).val(data.end_point_name);

        $(app.DE.txtStartLat).val(startLat);
        $(app.DE.txtStartLng).val(startLng);
        $(app.DE.txtBufferRadius_A).val(data.start_buffer);
        $(app.DE.txtEndLat).val(endLat);
        $(app.DE.txtEndLng).val(endLng);
        $(app.DE.txtBufferRadius_B).val(data.end_buffer);
        $(app.DE.coresTextBox).val(data.cores_required);
        $(app.DE.cableTypeddl).val(data.cable_type_id);

        app.startLatLng = { lat: parseFloat(startLat), lng: parseFloat(startLng) };
        app.endLatLng = { lat: parseFloat(endLat), lng: parseFloat(endLng) };

        var startGlatlng = new google.maps.LatLng(parseFloat(startLat), parseFloat(startLng));
        var endGlatlng = new google.maps.LatLng(parseFloat(endLat), parseFloat(endLng));

        app.disableBufferRadius(false, app.DE.end_type.START);
        app.disableBufferRadius(false, app.DE.end_type.END);


        if (app.startMarker)
            app.startMarker.setMap(null);
        app.startMarker = app.createMarker(startGlatlng, 'Content/images/StartPoint.png', app.DE.end_type.START);
        app.startMarker.addListener('drag', function (LatLong) {
            app.fillLocation(LatLong, app.DE.end_type.START);
        });
        app.addDistanceBuffer_A(app.sliderDefaults.VALUE / 1000, startGlatlng, app.startMarker);

        if (app.endMarker)
            app.endMarker.setMap(null);
        app.endMarker = app.createMarker(endGlatlng, 'Content/images/EndPoint.png', app.DE.end_type.END);
        app.endMarker.addListener('drag', function (LatLong) {
            app.fillLocation(LatLong, app.DE.end_type.END);
        });
        app.addDistanceBuffer_B(1, endGlatlng, app.endMarker);

        $(app.DE.txtBufferRadius_A).change();
        $(app.DE.txtBufferRadius_B).change();
    }

    this.validateFileData = function (data) {
        var result = { status: false, message: '' };
        if (data.length == 14) {
            let customerName = data[0];
            let customerID = data[1];
            let feasibilityID = data[2];
            let startPointName = data[3];
            let startLat = data[4];
            let startLng = data[5];
            let txtstartLat = data[4];
            let txtstartLng = data[5];

            let startBuffer = data[6];
            let endPointName = data[7];
            let endLat = data[8];
            let endLng = data[9];
            let txtendLat = data[8];
            let txtendLng = data[9];
            let endBuffer = data[10];
            let coresRequired = data[11];
            let cableType = data[12];

            var decimal = /^\d+\.\d+$/;

            if (customerName == undefined) {
                result.status = false;
                result.message = MultilingualKey.Customer_Name_cannot_be_empty;
                return result;
            }

            if (customerID == undefined) {
                result.status = false;
                result.message = MultilingualKey.Customer_ID_cannot_be_empty;
                return result;
            }

            if (feasibilityID == undefined) {
                result.status = false;
                result.message = MultilingualKey.Feasibility_ID_cannot_be_empty;
                return result;
            }

            var dd = app.DMStoDD(startLat);
            if (dd != -1) {
                startLat = dd;
            }
            else {
                result.status = false;
                result.message = MultilingualKey.Invalid_Start_Latitude;
                return result;
            }

            dd = app.DMStoDD(startLng);
            if (dd != -1) {
                startLng = dd;
            }
            else {
                result.status = false;
                result.message = MultilingualKey.Invalid_Start_Longitude;
                return result;
            }

            dd = app.DMStoDD(endLat);
            if (dd != -1) {
                endLat = dd;
            }
            else {
                result.status = false;
                result.message = MultilingualKey.Invalid_End_Latitude;
                return result;
            }

            dd = app.DMStoDD(endLng);
            if (dd != -1) {
                endLng = dd;
            }
            else {
                result.status = false;
                result.message = MultilingualKey.Invalid_End_Longitude;
                return result;
            }

            if (startBuffer == undefined || isNaN(startBuffer) || startBuffer < 0 || startBuffer > 20000) {
                result.status = false;
                result.message = MultilingualKey.Invalid_Start_Point_Buffer_Radius;
                return result;
            }

            if (endBuffer == undefined || isNaN(endBuffer) || endBuffer < 0 || endBuffer > 20000) {
                result.status = false;
                result.message = MultilingualKey.Invalid_End_Point_Buffer_Radius;
                return result;
            }

            if (coresRequired == undefined || isNaN(coresRequired) || coresRequired <= 0 || coresRequired > 288) {
                result.status = false;
                result.message = MultilingualKey.Invalid_Cores;
                return result;
            }

            if (cableType == undefined || cableType.length == 0) {
                result.status = false;
                result.message = MultilingualKey.Cable_Type_should_not_be_empty;
                return result;
            }
            var cableTypeDDL = $("#cableType option").filter(function () { return $(this).text() === cableType.toUpperCase(); });
            if (cableTypeDDL.val() != undefined && !isNaN(cableTypeDDL.text().slice(0, -1).trim())) {
                $("#cableType").val(cableTypeDDL.val());
                if (parseInt(coresRequired) > parseInt(cableTypeDDL.text().slice(0, -1).trim())) {
                    result.status = false;
                    result.message = 'Required cores must be less than or equal to Cable Type!';
                    return result;
                }
            }
            else {
                result.status = false;
                result.message = MultilingualKey.Cable_Type_does_not_exist;
                return result;
            }
            // fill values
            $(app.DE.txtCustomerName).val(customerName);
            $(app.DE.txtCustomerID).val(customerID);
            $(app.DE.txtFseasibilityID).val(feasibilityID);
            $(app.DE.txtStartLat).val(txtstartLat);
            $(app.DE.txtStartLng).val(txtstartLng);
            $(app.DE.txtBufferRadius_A).val(startBuffer);
            $(app.DE.txtEndLat).val(txtendLat);
            $(app.DE.txtEndLng).val(txtendLng);
            $(app.DE.txtBufferRadius_B).val(endBuffer);
            $(app.DE.coresTextBox).val(coresRequired);
            $(app.DE.txtStartPointName).val(startPointName);
            $(app.DE.txtEndPointName).val(endPointName);

            app.startLatLng = { lat: parseFloat(startLat), lng: parseFloat(startLng) };
            app.endLatLng = { lat: parseFloat(endLat), lng: parseFloat(endLng) };

            var startGlatlng = new google.maps.LatLng(parseFloat(startLat), parseFloat(startLng));
            var endGlatlng = new google.maps.LatLng(parseFloat(endLat), parseFloat(endLng));

            app.disableBufferRadius(false, app.DE.end_type.START);
            app.disableBufferRadius(false, app.DE.end_type.END);


            if (app.startMarker)
                app.startMarker.setMap(null);
            app.startMarker = app.createMarker(startGlatlng, 'Content/images/StartPoint.png', app.DE.end_type.START);
            app.startMarker.addListener('drag', function (LatLong) {
                app.fillLocation(LatLong, app.DE.end_type.START);
            });
            app.addDistanceBuffer_A(app.sliderDefaults.VALUE / 1000, startGlatlng, app.startMarker);

            if (app.endMarker)
                app.endMarker.setMap(null);
            app.endMarker = app.createMarker(endGlatlng, 'Content/images/EndPoint.png', app.DE.end_type.END);
            app.endMarker.addListener('drag', function (LatLong) {
                app.fillLocation(LatLong, app.DE.end_type.END);
            });
            app.addDistanceBuffer_B(app.sliderDefaults.VALUE / 1000, endGlatlng, app.endMarker);

            $(app.DE.txtBufferRadius_A).change();
            $(app.DE.txtBufferRadius_B).change();

            result.status = true;
            result.message = '';
        }
        else {
            result.status = false;
            result.message = MultilingualKey.Invalid_columns;
        }
        return result;
    }


    this.validateFileDataForFtth = function (data) {
        var result = { status: false, message: '' };
        if (data.length == 7) {
            let customerName = data[0];
            let customerID = data[1];
            let feasibilityID = data[2];
            let startLat = data[3];
            let startLng = data[4];

            let startBuffer = data[5];
            let txtstartLat = data[3];
            let txtstartLng = data[4];
            var decimal = /^\d+\.\d+$/;

            if (customerName == undefined) {
                result.status = false;
                result.message = MultilingualKey.Customer_Name_cannot_be_empty;
                return result;
            }

            if (customerID == undefined) {
                result.status = false;
                result.message = MultilingualKey.Customer_ID_cannot_be_empty;
                return result;
            }

            if (feasibilityID == undefined) {
                result.status = false;
                result.message = MultilingualKey.Feasibility_ID_cannot_be_empty;
                return result;
            }

            var dd = app.DMStoDD(startLat);
            if (dd != -1) {
                startLat = dd;
            }
            else {
                result.status = false;
                result.message = MultilingualKey.Invalid_Start_Latitude;
                return result;
            }

            dd = app.DMStoDD(startLng);
            if (dd != -1) {
                startLng = dd;
            }
            else {
                result.status = false;
                result.message = MultilingualKey.Invalid_Start_Longitude;
                return result;
            }



            if (startBuffer == undefined || isNaN(startBuffer) || startBuffer < 0 || startBuffer > 2000) {
                result.status = false;
                result.message = MultilingualKey.Invalid_Start_Point_Buffer_Radius;
                return result;
            }


            $(app.DE.txtCustomerName).val(customerName);
            $(app.DE.txtCustomerID).val(customerID);
            $(app.DE.txtFseasibilityID).val(feasibilityID);
            $(app.DE.ftthLat).val(txtstartLat);
            $(app.DE.ftthLng).val(txtstartLng);
            $(app.DE.txtBufferRadius_Ftth).val(startBuffer);
            $(app.DE.slider_Ftth).slider('value', startBuffer);
            $(app.DE.sliderHandle_Ftth).text(startBuffer);
            sf.iRadiusInKiloMeter = startBuffer / 1000;
            if ($('#FFTH-layer-panel :checked').val() == undefined) {

                $('#FFTH-layer-panel :checkbox').prop('checked', true);
                CheckAllLayrs('Layers', true);
            }
            app.startLatLng = { lat: parseFloat(startLat), lng: parseFloat(startLng) };
            var startGlatlng = new google.maps.LatLng(parseFloat(startLat), parseFloat(startLng));

            //get widget
            app.GeoCodeIt($(app.DE.ftthLat).val() + ',' + $(app.DE.ftthLng).val());


            result.status = true;
            result.message = '';


        }
        else {
            result.status = false;
            result.message = MultilingualKey.Invalid_columns;
        }
        return result;
    }

    this.bulkAPIRequest = function (requestData) {
        apiAuthUrl = app.ROUTING_API_URL + "api/users/authenticate";
        apiUrl = app.ROUTING_API_URL + "api/roadnetwork/bulkdirections";
        requestObj = requestData;

        $.ajax({
            type: "POST",
            url: apiAuthUrl,
            data: JSON.stringify(app.authAPICredentials),
            dataType: "json",
            contentType: "application/json",
            headers: {
                "Content-Type": "application/json",
            },
            success: function (result, status, xhr) {
                if (status == "success") {
                    let authToken = 'Bearer ' + result.token;
                    var apiRequest = requestObj;
                    $.ajax({
                        type: "POST",
                        url: apiUrl,
                        data: JSON.stringify(apiRequest),
                        dataType: "json",
                        contentType: "application/json",
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": authToken
                        },
                        success: function (result, status, xhr) {
                            if (status == "success") {
                                //console.log(result);
                            }
                        },
                        error: function (xhr, status, error) {
                            alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
                        }
                    });
                }
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
            }
        });
    }

    this.showPaths = function () {

        //----------------------------------------------------
        var apiAuthUrl = '';
        var apiUrl = '';
        var requestObj = {}

        //if (app.DEMO_VERSION == 1) {
        //    apiAuthUrl = app.ROUTING_API_URL + "users/authenticate";
        //    apiUrl = app.ROUTING_API_URL + "roadnetwork/roaddirections";
        //    requestObj = {
        //        "Src_Lat": 38.9899,
        //        "Src_Long": 38.9899,
        //        "Dest_Lat": 38.9899,
        //        "Dest_Long": 38.9899,
        //        "Source": app.startLatLng.lng.toString() + ' ' + app.startLatLng.lat.toString(),
        //        "Destination": app.endLatLng.lng.toString() + ' ' + app.endLatLng.lat.toString(),
        //        "Waypoints": [],
        //        "IsDirected": true
        //    };
        //}
        //else if (app.DEMO_VERSION == 2 || app.DEMO_VERSION == 3) {
        apiAuthUrl = app.ROUTING_API_URL + "api/users/authenticate";
        apiUrl = app.ROUTING_API_URL + "api/roadnetwork/roaddirections";
        requestObj = {
            "Src_Lat": 38.9899,
            "Src_Long": 38.9899,
            "Dest_Lat": 38.9899,
            "Dest_Long": 38.9899,
            "Source": app.startLatLng.lng.toString() + ' ' + app.startLatLng.lat.toString(),
            "Destination": app.endLatLng.lng.toString() + ' ' + app.endLatLng.lat.toString(),
            "StartPointRadius": parseFloat($(app.DE.txtBufferRadius_A).val()),
            "EndPointRadius": parseFloat($(app.DE.txtBufferRadius_B).val()),
            "Waypoints": [],
            "IsDirected": true
        };
        //}

        ajaxReq('Main/getRoutes', { src: requestObj.Source, desti: requestObj.Destination, sourceBuffer: requestObj.StartPointRadius, destiBuffer: requestObj.EndPointRadius, p_core_required: app.cores_required }, true, function (result, status, xhr) {
            if (status == "success" && result.status == "success") {
                 
                app.insidePaths = [];
                var path = [];
                result = result.directions;
                if (result.length) {
                    // sort the result by path_Seq
                    result = app.sort_by_key(result, 'path_Seq');
                    app.is_core_feasibile = true;
                    for (item of result) {
                        path = [];
                        var geom = item.roadLine_GeomText.replace('LINESTRING(', '').replace(')', '').split(',');
                        var system_id = item.edge_TargetID
                        for (g of geom) {
                            var latLng = g.trim().split(' ');
                            path.push(new google.maps.LatLng(latLng[1], latLng[0]));
                        }
                        app.insidePaths.push(
                            {
                                path: path,
                                system_id: system_id,
                                length: google.maps.geometry.spherical.computeLength(path),
                                cable_id: item.network_id,
                                cable_name: item.cable_name,
                                total_cores: item.total_cores,
                                used_cores: item.total_cores - item.available_cores,
                                network_status: item.cable_status,
                                available_cores: item.available_cores,
                                is_core_feasibile: item.available_cores >= app.cores_required
                            });
                        if (item.available_cores < app.cores_required) {
                            app.is_core_feasibile = false;
                        }
                    }

                    // order the paths in single direction from start to end 
                    //app.orderInsidePaths(app.isSamePathSeq(result));
                    app.setWayPoints(result[0].start_point, result[0].end_point);

                    // process inside paths only if both end-points are within buffer range
                    if (!app.isWithinBufferRegion()) {
                        app.waypoints = [];
                        app.insidePaths = [];
                    }
                    app.processPaths();

                    //if (app.DEMO_VERSION == 1 || app.DEMO_VERSION == 2) {
                    //    // process inside paths only if both end-points are within buffer range
                    //    if (app.isWithinBufferRegion()) {
                    //        app.updateNetworkStatus();
                    //    }
                    //    else {
                    //        app.insidePaths = [];
                    //        app.processPaths();
                    //    }
                    //}
                    //else {
                    //    app.updateNetworkStatus();
                    //}
                }
                else {
                    app.is_core_feasibile = false;
                    app.wayPoints = [];
                    app.processPaths();
                }
            }
            else {
                if (result.status == 'error') {
                    alert('Error: ' + result.message);
                }
                app.processPaths();
            }
        }, true, true);

        //$.ajax({
        //    type: "POST",
        //    url: "Main/getRoutes",
        //    data: JSON.stringify({ src: requestObj.Source, desti: requestObj.Destination, sourceBuffer: requestObj.StartPointRadius, destiBuffer: requestObj.EndPointRadius }),
        //    dataType: "json",
        //    contentType: "application/json",
        //    success: function (result, status, xhr) {
        //        if (status == "success" && result.status == "success") {
        //            app.insidePaths = [];
        //            var path = [];
        //            result = result.directions;
        //            if (result.length) {
        //                // sort the result by path_Seq
        //                result = app.sort_by_key(result, 'path_Seq');

        //                for(item of result) {
        //                    path = [];
        //                    var geom = item.roadLine_GeomText.replace('LINESTRING(', '').replace(')', '').split(',');
        //                    var system_id = item.edge_TargetID
        //                    for(g of geom) {
        //                        var latLng = g.trim().split(' ');
        //                        path.push(new google.maps.LatLng(latLng[1], latLng[0]));
        //                    }
        //                    app.insidePaths.push({ path: path, system_id: system_id, length: google.maps.geometry.spherical.computeLength(path) });
        //                }

        //                // order the paths in single direction from start to end 
        //                app.orderInsidePaths();

        //                if (app.DEMO_VERSION == 1 || app.DEMO_VERSION == 2) {
        //                    // process inside paths only if both end-points are within buffer range
        //                    if (app.isWithinBufferRegion()) {
        //                        app.updateNetworkStatus();
        //                    }
        //                    else {
        //                        app.insidePaths = [];
        //                        app.processPaths();
        //                    }
        //                }
        //                else {
        //                    app.updateNetworkStatus();
        //                }
        //            }
        //            else {
        //                app.processPaths();
        //            }
        //        }
        //        else {
        //            if (result.status == 'error') {
        //                alert('Error: ' + result.message);
        //            }
        //            app.processPaths();
        //        }
        //    },
        //    error: function (xhr, status, error) {
        //        alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
        //    }
        //});

        /*$.ajax({
            type: "POST",
            url: apiAuthUrl,
            data: JSON.stringify(app.authAPICredentials),
            dataType: "json",
            contentType: "application/json",
            headers: {
                "Content-Type": "application/json",
            },
            success: function (result, status, xhr) {
                if (status == "success") {
                    let authToken = 'Bearer ' + result.token;
                    var apiRequest = requestObj;
    
                    $.ajax({
                        type: "POST",
                        url: apiUrl,
                        data: JSON.stringify(apiRequest),
                        dataType: "json",
                        contentType: "application/json",
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": authToken
                        },
                        success: function (result, status, xhr) {
                            app.insidePaths = [];
                            var path = [];
                            if (status == "success") {
                                if (result.length) {
                                    // sort the result by path_Seq
                                    result = app.sort_by_key(result, 'path_Seq');
    
                                    for(item of result) {
                                        path = [];
                                        var geom = item.roadLine_GeomText.replace('LINESTRING(', '').replace(')', '').split(',');
                                        var system_id = item.edge_TargetID
                                        for(g of geom) {
                                            var latLng = g.trim().split(' ');
                                            path.push(new google.maps.LatLng(latLng[1], latLng[0]));
                                        }
                                        app.insidePaths.push({ path: path, system_id: system_id, length: google.maps.geometry.spherical.computeLength(path) });
                                    }
    
                                    // order the paths in single direction from start to end 
                                    app.orderInsidePaths();
    
                                    if (app.DEMO_VERSION == 1 || app.DEMO_VERSION == 2) {
                                        // process inside paths only if both end-points are within buffer range
                                        if (app.isWithinBufferRegion()) {
                                            app.updateNetworkStatus();
                                        }
                                        else {
                                            app.insidePaths = [];
                                            app.processPaths();
                                        }
                                    }
                                    else {
                                        app.updateNetworkStatus();
                                    }
                                }
                                else {
                                    app.processPaths();
                                }
                            }
                            else {
                                //app.processPaths();
                            }
                        },
                        error: function (xhr, status, error) {
                            alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
                            app.processPaths();
                        }
                    });
                }
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
                app.processPaths();
            }
        });*/


        //----------------------------------------------------


    }


    // to do ==> rename
    this.processPaths = function () {
        var start = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
        var end = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);

        app.clearAllPaths();

        var request = {
            origin: start,
            destination: end,
            travelMode: 'WALKING',
            waypoints: app.getWaypoints(),
            provideRouteAlternatives: true
        };

        app.directionsService.route(request, function (result, status) {
            if (status == 'OK') {
                if (result.routes) {
                    // app.feasibilityDetailsInfo = [];
                    app.feasibilityDetailsInfo = [];
                    app.feasibilityGeometry = [];
                    app.feasiblePaths = [];
                    // show at most 3 paths
                    for (var i = 0; i < 3 && i < result.routes.length; i++) {

                        app.feasiblePaths[i] = [];

                        // if there are 2 waypoints, there would be 3 legs
                        // we would remove middle leg, and it would be replaced by the inside path (existing network)
                        if (result.routes[i].legs.length == 3 && app.insidePaths.length) {
                            result.routes[i].legs.splice(1, 1);
                            var outsidePath1 = app.getRouteLegPath(result.routes[i].legs[0]);
                            var outsidePath2 = app.getRouteLegPath(result.routes[i].legs[1]);

                            let wp = request.waypoints;
                            if (wp.length) {
                                // connect with inside path from both sides
                                outsidePath1.push(wp[0].location);
                                outsidePath2.unshift(wp[1].location);
                            }

                            // LMC path
                            var lmcPath_Start = [start, outsidePath1[0]];
                            var lmcPath_End = [outsidePath2[outsidePath2.length - 1], end];

                            var outside_polyline1 = new google.maps.Polyline({
                                path: outsidePath1,
                                strokeColor: app.DE.dullColor,
                                strokeOpacity: 1.0,
                                strokeWeight: 5,
                                zIndex: 0,
                                isSelected: false,
                                type: app.cable_types.OUTSIDE_A_END,
                                pathIndex: i
                            });

                            var outside_polyline2 = new google.maps.Polyline({
                                path: outsidePath2,
                                strokeColor: app.DE.dullColor,
                                strokeOpacity: 1.0,
                                strokeWeight: 5,
                                zIndex: 0,
                                isSelected: false,
                                type: app.cable_types.OUTSIDE_B_END,
                                pathIndex: i
                            });

                            var insidePolylines = app.getInsidePolylines();
                            if (insidePolylines.length) {
                                for (line of insidePolylines) {
                                    line.pathIndex = i;
                                    app.feasiblePaths[i].push(line);
                                }
                            }

                            app.feasiblePaths[i].push(outside_polyline1);
                            app.feasiblePaths[i].push(outside_polyline2);
                        }
                        else {
                            var path = google.maps.geometry.encoding.decodePath(result.routes[i].overview_polyline);
                            lmcPath_Start = [start, path[0]];
                            lmcPath_End = [path[path.length - 1], end];

                            let outside_polyline = new google.maps.Polyline({
                                path: path,
                                strokeColor: app.DE.dullColor,
                                strokeOpacity: 1.0,
                                strokeWeight: 5,
                                zIndex: 0,
                                isSelected: false,
                                type: app.cable_types.OUTSIDE_A_END,
                                pathIndex: i
                            });

                            app.feasiblePaths[i].push(outside_polyline);
                        }

                        var lmc_Polyline_Start = new google.maps.Polyline({
                            path: lmcPath_Start,
                            strokeColor: app.color_LMC,
                            strokeOpacity: 1.0,
                            strokeWeight: 5,
                            zIndex: 0,
                            isSelected: false,
                            type: app.cable_types.LMC_A_END,
                            pathIndex: i
                        });

                        var lmc_Polyline_End = new google.maps.Polyline({
                            path: lmcPath_End,
                            strokeColor: app.color_LMC,
                            strokeOpacity: 1.0,
                            strokeWeight: 5,
                            zIndex: 0,
                            isSelected: false,
                            type: app.cable_types.LMC_B_END,
                            pathIndex: i
                        });

                        app.feasiblePaths[i].push(lmc_Polyline_Start);
                        app.feasiblePaths[i].push(lmc_Polyline_End);

                        // attach events to all sub-paths
                        for (subPath of app.feasiblePaths[i]) {
                            if (subPath) {

                                google.maps.event.addListener(subPath, 'mouseover', function (event) {
                                    app.resetPaths();

                                    // highlight the complete path
                                    let all_subPaths = app.feasiblePaths[this.pathIndex];
                                    for (path of all_subPaths) {
                                        if (path) {
                                            path.setOptions({ strokeColor: app.getColorByType(path.type), zIndex: 10 });
                                        }
                                    }

                                    if (app.infowindow)
                                        app.infowindow.setMap(null);

                                    if (app.detailsMenu && !this.editable) {
                                        app.detailsMenu.open(app.map, this, event.latLng);
                                    }

                                    app.updateDistances(this);  // need to find better way
                                });

                                google.maps.event.addListener(subPath, 'mouseout', function (event) {
                                    app.resetPaths();
                                    app.highlightSelectedPath();
                                    if (app.infowindow)
                                        app.infowindow.setMap(null);
                                    if (app.detailsMenu) {
                                        //app.detailsMenu.setMap(null);
                                    }
                                });

                                google.maps.event.addListener(subPath, 'click', function (event) {
                                    app.selectPath(app.feasiblePaths[this.pathIndex]);
                                });

                                google.maps.event.addListener(subPath, 'rightclick', function (event) {
                                    if (app.infowindow)
                                        app.infowindow.setMap(null);

                                    if (event.vertex) {
                                        this.getPath().removeAt(event.vertex);
                                        return;
                                    }

                                    if (app.editMenu) {
                                        if (app.detailsMenu)
                                            app.detailsMenu.setMap(null);

                                        if (this.type != app.cable_types.INSIDE && this.type != app.cable_types.INSIDE_A && this.type != app.cable_types.INSIDE_P)
                                            app.editMenu.open(app.map, this, event.latLng);
                                    }
                                });
                            }
                        }
                    }

                    app.setMapForAll();
                    app.selectShortestPath();
                    app.feasibilityDetailsInformation();
                    // $(app.DE.divLegends).show();
                }
            }
        });
    }

    this.highlightSelectedPath = function () {
        for (var i = 0; i < app.feasiblePaths.length; i++) {
            var subPath = app.feasiblePaths[i];
            if (subPath.length) {
                if (subPath[0].isSelected) {
                    for (path of subPath) {
                        if (path) {
                            path.setOptions({ zIndex: 10, strokeColor: app.getColorByType(path.type) });
                        }
                    }
                    break;
                }
            }
        }
    }

    this.resetPathEdits = function () {
        for (var i = 0; i < app.feasiblePaths.length; i++) {
            let all_subPaths = app.feasiblePaths[i];
            for (path of all_subPaths) {
                if (path) {
                    path.setOptions({ zIndex: 0, strokeColor: app.DE.dullColor, editable: false, isSelected: false });
                }
            }
        }
    }

    this.getDescriptionHTML = function (subPath) {
        let title = app.getTitle(subPath.type) + ' Network';
        let distance = app.getDistanceString(subPath.self_length);

        let desc = '<![CDATA[<p><b>Distance:</b> ' + distance + '<br/>';
        desc += (subPath.cable_id ? ('<b>Cable ID: </b>' + subPath.cable_id) + '<br/>' : '');
        desc += (subPath.cable_name ? ('<b>Cable Name: </b>' + subPath.cable_name) : '');
        desc += '</p>]]>';

        return desc;
    }

    this.getTitle = function (type) {
        if (type == 'inside_A')
            return 'Inside As-built';
        else if (type == 'inside_P')
            return 'Inside Planned';
        else if (type == 'inside')
            return 'Inside';
        else if (type == 'outside_start')
            return 'Outside A';
        else if (type == 'outside_end')
            return 'Outside B';
        else if (type == 'lmc_start')
            return 'LMC A';
        else if (type == 'lmc_end')
            return 'LMC B';
    }

    app.getInsideCableData = function () {
         
        var data = [];
        if (app.insidePaths.length) {
            for (path of app.insidePaths) {
                if (path) {
                    data.push(
                        {
                            cable_id: path.cable_id,
                            //system_id: path.system_id,
                            cable_name: path.cable_name,
                            network_status: path.network_status,
                            total_cores: path.total_cores,
                            used_cores: path.used_cores,
                            available_cores: path.available_cores,
                            cable_length: path.length.toFixed(2)
                        });
                }
            }
        }
        return data;
    }
    this.CalcfeasibilityStatus = function (totalInsideLength, totalDistance) {
        let percentInside = (totalInsideLength * 100 / totalDistance).toFixed(2);
        var feasibilityStatus = 'Not Feasible';
        if (percentInside > 80) {
            feasibilityStatus = 'Feasible';
        }
        else if (percentInside >= 50) {
            feasibilityStatus = 'Partially Feasible';
        }
        return feasibilityStatus;
    }
    this.getXLSData = function (isPast) {
        var start = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
        var end = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
        var feasibilityStatus = '';
        var is_core_feasibile = '';
        let percentInside = 0;
        let insideCableData = {};
        var startPointName = '';
        var endPointName = '';
        if (!isPast) {
            let totalInsideLength = (app.FeasibilityDistances.insideDistance ? app.FeasibilityDistances.insideDistance : 0) +
                (app.FeasibilityDistances.inside_P_Distance ? app.FeasibilityDistances.inside_P_Distance : 0) +
                (app.FeasibilityDistances.inside_A_Distance ? app.FeasibilityDistances.inside_A_Distance : 0);
            percentInside = (totalInsideLength * 100 / app.FeasibilityDistances.totalDistance).toFixed(2);
            feasibilityStatus = app.CalcfeasibilityStatus(totalInsideLength, app.FeasibilityDistances.totalDistance);

            is_core_feasibile = app.is_core_feasibile ? 'Feasible' : 'Not Feasible';

            insideCableData = app.getInsideCableData();
            startPointName = $(app.DE.txtStartPointName).val();
            endPointName = $(app.DE.txtEndPointName).val();
        }
        else {
            feasibilityStatus = app.feasibility_result;
            is_core_feasibile = app.core_level_result;
            insideCableData = app.pastInsideCables;
            percentInside = app.percentInside;
            startPointName = $("#_startPointName").val();
            endPointName = $("#_endPointName").val();
        }


        return [{
            Start_Point_Name: startPointName.length ? startPointName : ' ',
            Start_Lat: app.startLatLng.lat,
            Start_Lng: app.startLatLng.lng,
            End_Point_Name: endPointName.length ? endPointName : ' ',
            End_Lat: app.endLatLng.lat,
            End_Lng: app.endLatLng.lng,
            Cores_Required: app.cores_required,
            Total_Length: app.FeasibilityDistances.totalDistance ? app.FeasibilityDistances.totalDistance.toFixed(2) : 0,
            Inside_Planned_Length: app.FeasibilityDistances.inside_P_Distance ? app.FeasibilityDistances.inside_P_Distance.toFixed(2) : 0,
            Inside_AsBuilt_Length: app.FeasibilityDistances.inside_A_Distance ? app.FeasibilityDistances.inside_A_Distance.toFixed(2) : 0,
            //Inside_Length: app.FeasibilityDistances.insideDistance,
            Outside_A_Length: app.FeasibilityDistances.outside_A_Distance ? app.FeasibilityDistances.outside_A_Distance.toFixed(2) : 0,
            Outside_B_Length: app.FeasibilityDistances.outside_B_Distance ? app.FeasibilityDistances.outside_B_Distance.toFixed(2) : 0,
            LMC_A_Length: app.FeasibilityDistances.lmc_A_Distance ? app.FeasibilityDistances.lmc_A_Distance.toFixed(2) : 0,
            LMC_B_Length: app.FeasibilityDistances.lmc_B_Distance ? app.FeasibilityDistances.lmc_B_Distance.toFixed(2) : 0,
            Inside_Length_Percent: percentInside,
            Status: feasibilityStatus,
            is_core_feasibile: is_core_feasibile,
            //cable_type_cores: $(app.DE.cableType).val(),
            insideCables: insideCableData
        }];
    }

    this.getBOMData = function () {
        let totalLength = (app.FeasibilityDistances.outside_A_Distance ? app.FeasibilityDistances.outside_A_Distance : 0) +
            (app.FeasibilityDistances.outside_B_Distance ? app.FeasibilityDistances.outside_B_Distance : 0) +
            (app.FeasibilityDistances.lmc_A_Distance ? app.FeasibilityDistances.lmc_A_Distance : 0) +
            (app.FeasibilityDistances.lmc_B_Distance ? app.FeasibilityDistances.lmc_B_Distance : 0);
        return [{
            cable_type_id: $(app.DE.cableType).val(),
            cable_length: totalLength,
            cable_type: $(app.DE.cableType).text(),
            material_unit_price: 0,
            service_unit_price: 0,
            total_material_cost: 0,
            total_service_cost: 0

        }];
    }

    this.getKMLHexCode = function (code_6) {
        let Hex_8_dig = '#FF0000FF';
        if (code_6.length) {
            // KML expects color in AABBGGRR format where AA is Alpha (opaqueness), 00 is transparent, FF is full opaque
            Hex_8_dig = ('#FF' + code_6.substring(5) + code_6.substring(3, 5) + code_6.substring(1, 3)).toUpperCase();
        }
        return Hex_8_dig;
    }

    this.getKMLData = function () {
        var KMLData = [];

        // process cables
        for (path of app.feasiblePaths) {
            if (path) {
                for (sub_path of path) {
                    if (sub_path.isSelected) {
                        var geom = '';
                        for (ll of sub_path.getPath().getArray()) {
                            geom += ll.lng() + ',' + ll.lat() + ',0 ';
                        }
                        if (geom.length) {
                            KMLData.push({
                                cable_id: sub_path.cable_id,
                                cable_name: sub_path.cable_name,
                                geom: geom,
                                description: app.getDescriptionHTML(sub_path),
                                geom_type: 'LINE',
                                entity_name: sub_path.type,
                                entity_title: app.getTitle(sub_path.type),
                                entity_type: sub_path.type,
                                distance: sub_path.distance,
                                colorHex_8: app.getKMLHexCode(sub_path.strokeColor)
                            });
                        }
                    }
                }
            }
        }

        // proces start and end points
        if (app.startLatLng) {
            KMLData.push({
                geom: app.startLatLng.lng + ',' + app.startLatLng.lat + ',0 ',
                description: '<![CDATA[<h2>(' + app.startLatLng.lng + ',' + app.startLatLng.lat + ')</h2>]]>',
                geom_type: 'POINT',
                entity_name: 'Start Point',
                entity_title: ($(app.DE.txtStartPointName).val() != null && $(app.DE.txtStartPointName).val().length > 0) ? $(app.DE.txtStartPointName).val() : 'Start Point'
            });
        }
        if (app.endLatLng) {
            KMLData.push({
                geom: app.endLatLng.lng + ',' + app.endLatLng.lat + ',0 ',
                description: '<![CDATA[<h2>(' + app.startLatLng.lng + ',' + app.startLatLng.lat + ')</h2>]]>',
                geom_type: 'POINT',
                entity_name: 'End Point',
                entity_title: ($(app.DE.txtEndPointName).val() != null && $(app.DE.txtEndPointName).val().length > 0) ? $(app.DE.txtEndPointName).val() : 'End Point'
            });
        }

        return KMLData;
    }
    this.downloadHistoryReport = function (reportType) {
        app.FeasibilityDistances.insideDistance = parseFloat($("#_insideLength").val());
        app.FeasibilityDistances.outsideDistance = parseFloat($("#_outsideLength").val());
        app.FeasibilityDistances.lmc_A_Distance = parseFloat($("#_lmcA").val());
        app.FeasibilityDistances.lmc_B_Distance = parseFloat($("#_lmcB").val())
        app.FeasibilityDistances.totalDistance = (app.FeasibilityDistances.insideDistance + app.FeasibilityDistances.outsideDistance + app.FeasibilityDistances.lmc_A_Distance + app.FeasibilityDistances.lmc_B_Distance);

        app.dowloadFeasibilityReport(reportType, false);

        // $("#resultas").append($item);       // Outputs the answer
    }
    this.getLatLongArr = function (pgString) {
        var latLngArr = [];
        pgString = pgString.substring(pgString.lastIndexOf('(') + 1, pgString.indexOf(')'));
        var longLatArr = pgString.split(',');
        for (var i = 0; i < longLatArr.length; i++) {
            var LongLatsingle = longLatArr[i].split(' ');
            latLngArr.push(new google.maps.LatLng(parseFloat(LongLatsingle[1]), parseFloat(LongLatsingle[0])));
        }
        return latLngArr;
    }
    this.createLine = function (_path) {
        var tmpLine = new google.maps.Polyline({
            strokeColor: '#FF8800',
            strokeOpacity: 1,
            strokeWeight: 2,
            path: _path,
            //editable: true
        });
        return tmpLine;
    }
    this.ComputeOldFeasibilityAgain = function () {
        $("#FeasibilityHistory tr").on("click", function () {
            var $item = $(this).closest("tr");   // Finds the closest row <tr>
            app.startLatLng.lat = $item[0].cells[8].innerText.trim();
            app.startLatLng.lng = $item[0].cells[9].innerText.trim();
            app.endLatLng.lat = $item[0].cells[10].innerText.trim();
            app.endLatLng.lng = $item[0].cells[11].innerText.trim();
            app.showPaths();
            var latlng = new google.maps.LatLng(app.startLatLng.lat, app.endLatLng.lng);
            app.map.setCenter(latlng);
            app.map.setZoom(14);
        })

    }
    this.ShowfeasibilityOnMap = function (cable_geom) {
        var lineobj = [];
        var latLngArr = cable_geom.split(',');
        // var latLngArr = app.getLatLongArr(cable_geom);
        $(latLngArr).each(function (i, item) {
            if (i == 0) {
                c = { lat: parseFloat(item.split(' ')[1]), lng: parseFloat(item.split(' ')[0]) };
                // c = { lat: 37.772, lng: -122.214 };
            }
            lineobj.push({ lat: parseFloat(item.split(' ')[1]), lng: parseFloat(item.split(' ')[0]) })
        });
        app.gMapObj.tempSrchObj = app.createLine(lineobj);

        app.gMapObj.tempSrchObj.strokeColor = '#FF0000';
        app.gMapObj.tempSrchObj.strokeOpacity = 0.6;
        app.gMapObj.tempSrchObj.strokeWeight = '2';
        app.gMapObj.tempSrchObj.setMap(app.map);
    }
    this.dowloadFeasibilityReport = function (reportType, isPast) {
         
        if (app.FeasibilityDistances) {
            var kmlData = [];
            var xlsData = [];
            var bomData = [];

            if (reportType == 'KML') {
                if (!isPast)
                    kmlData = app.getKMLData();
                else
                    kmlData = app.pastKMLData;
            }
            else if (reportType == 'XLS' || reportType == 'PDF')
                xlsData = app.getXLSData(isPast);
            else if (reportType == 'BOM') {
                if (!isPast)
                    bomData = app.getBOMData(isPast);
                else
                    bomData = app.pastBOMData;
            }
            else
                return;
            $.ajax({
                type: "POST",
                url: "Report/SetFeasibiltyData",
                data: JSON.stringify({ reportData: xlsData, kmlData: kmlData, bomData: bomData }),
                dataType: "json",
                contentType: "application/json",
                success: function (result, status, xhr) {
                    if (status == "success") {
                        window.location = appRoot + 'Report/DownloadFeasibilityReport?reportType=' + reportType;
                    }
                },
                error: function (xhr, status, error) {
                    alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
                }
            });
        }
    }

    //FTTH FEASIBILITY REPORT

    this.getXLSDataFtth = function () {
        if ($('#hdnFeasibilityInput').val()) {
            return [{

                feasibility_id: $("#hdn_feasibility_id").val(),
                feasibility_name: $(app.DE.txtFseasibilityID).val(),
                customer_id: $(app.DE.txtCustomerID).val(),
                customer_name: $(app.DE.txtCustomerName).val(),
                entity_id: app.ftthEntityId,
                lat_lng: app.DMStoDD($(app.DE.ftthLat).val()) + "," + app.DMStoDD($(app.DE.ftthLng).val()),
                path_distance: app.NeDeviceDist,
                buffer_radius: $(app.DE.txtBufferRadius_Ftth).val()

            }];

        } else {
            return [{
                feasibility_id: $("#_feasibilityID").val(),
                feasibility_name: $("#_feasibilityName").val(),
                customer_id: $("#_customerID").val(),
                customer_name: $("#_customerName").val(),
                entity_id: $("#_entityId").val(),
                lat_lng: $("#_Location").val(),
                path_distance: $("#_pathDistance").val().split(' ')[0],
                buffer_radius: $("#_bufferRadius").val().split(' ')[0]

            }];
        }
    }


    this.dowloadFeasibilityReportFTTH = function (reportType) {

        var kmlData = [];
        var xlsData = [];
        var bomData = [];

        if (reportType == 'KML') {
            if ($('#hdnFeasibilityInput').val() == "ftth") {
                app.ExportKMLReportFtth();
                kmlData = app.ExportKMLDataFtth;

            } else {
                kmlData = app.pastKMLDataFtth;
            }

        }
        else if (reportType == 'XLS' || reportType == 'PDF')
            xlsData = app.getXLSDataFtth();
        else if (reportType == 'BOM') {
            if (!isPast)
                bomData = app.getBOMData(isPast);
            else
                bomData = app.pastBOMData;
        }
        else
            return;
        $.ajax({
            type: "POST",
            url: "Report/SetFeasibiltyDataFTTH",
            data: JSON.stringify({ reportData: xlsData, kmlData: kmlData, bomData: bomData }),
            dataType: "json",
            contentType: "application/json",
            success: function (result, status, xhr) {
                if (status == "success") {
                    window.location = appRoot + 'Report/DownloadFeasibilityReportFTTH?reportType=' + reportType;
                }
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        });

    }
    this.ExportKMLReportFtth = function () {

        let KMLData = [];
        var data =
        {
            "feasibility_id": $("#hdn_feasibility_id").val(),
            "feasibility_name": $(app.DE.txtFseasibilityID).val(),
            "customer_name": $(app.DE.txtCustomerName).val(),
            "customer_id": $(app.DE.txtCustomerID).val(),
            "lat": app.DMStoDD($(app.DE.ftthLat).val()),
            "lng": app.DMStoDD($(app.DE.ftthLng).val()),
            "path_geometry": app.FtthLatLng,
            "entity_id": app.ftthEntityId,
            "path_distance": app.NeDeviceDist,
            "entity_location": app.neElmLoc.lat() + "," + app.neElmLoc.lng(),
            "buffer_radius": $(app.DE.txtBufferRadius_Ftth).val(),
        };

        var startPoint = app.DMStoDD($(app.DE.ftthLat).val()) + "," + app.DMStoDD($(app.DE.ftthLng).val());
        var endPoint = app.neElmLoc.lat() + "," + app.neElmLoc.lng();
        var ll1 = startPoint.trim().split(',');
        var ll2 = endPoint.trim().split(',');


        var geom = '';
        var geomArray = data.path_geometry;
        for (g of geomArray) {
            var latLng = g.trim().split(',');
            geom += latLng[1] + ',' + latLng[0] + ',0 ';
        }
        let distance = data.path_distance;

        let desc = '<![CDATA[<p><b>Distance:</b> ' + distance + '<br/>';
        desc += ('<b>Feasibility ID: </b>' + data.feasibility_id);
        desc += ('<b>Feasibility Name: </b>' + data.feasibility_name);
        desc += '</p>]]>';

        if (geom.length) {
            KMLData.push({
                feasibility_id: data.feasibility_id,
                feasibility_name: data.feasibility_name,
                geometry: geom,
                description: desc,
                geom_type: 'LINE',
                entity_id: data.entity_id,
                path_distance: data.path_distance,
                colorHex_8: sf.getKMLHexCode(sf.getColorByType())
            });
        }


        if (ll1.length) {
            KMLData.push({
                geometry: ll1[1] + ',' + ll1[0] + ',0 ',
                description: '<![CDATA[<h2>(' + ll1[1] + ',' + ll1[0] + ')</h2>]]>',
                geom_type: 'POINT',
                feasibility_title: 'Customer Location'
                // entity_title: (tableData.start_point_name != null && tableData.start_point_name.length > 0) ? //tableData.start_point_name : 'Start Point'
            });
        }
        if (ll2.length) {
            KMLData.push({
                geometry: ll2[1] + ',' + ll2[0] + ',0 ',
                description: '<![CDATA[<h2>(' + ll2[1] + ',' + ll2[0] + ')</h2>]]>',
                geom_type: 'POINT',
                feasibility_title: 'Entity Location'
                // entity_title: (tableData.end_point_name != null && tableData.end_point_name.length > 0) ? //tableData.end_point_name : 'End Point'
            });
        }

        app.ExportKMLDataFtth = KMLData;
    }

    //END FTTH FEASIBILITY REPORT

    this.resetPaths = function () {
        for (var i = 0; i < app.feasiblePaths.length; i++) {
            let all_subPaths = app.feasiblePaths[i];
            for (path of all_subPaths) {
                if (path) {
                    path.setOptions({ zIndex: 0, strokeColor: app.DE.dullColor });
                }
            }
        }
    }

    this.unselectAllPaths = function () {
        for (var i = 0; i < app.feasiblePaths.length; i++) {
            let all_subPaths = app.feasiblePaths[i];
            for (path of all_subPaths) {
                if (path) {
                    path.setOptions({ isSelected: false, zIndex: 0, strokeColor: app.DE.dullColor });
                }
            }
        }
    }

    this.updateBufferRadius = function (type, radius) {
        if (type == app.DE.end_type.START) {
            $(app.DE.slider_A).slider('value', radius);
            $(app.DE.sliderHandle_A).text(radius);
            $(app.DE.txtBufferRadius_A).val(radius);
            app.distanceWidget_A.set('distance', radius / 1000);
        }
        else if (type == app.DE.end_type.END) {
            $(app.DE.slider_B).slider('value', radius);
            $(app.DE.sliderHandle_B).text(radius);
            $(app.DE.txtBufferRadius_B).val(radius);
            app.distanceWidget_B.set('distance', radius / 1000);
        }
        else if (type == app.DE.end_type.ftth) {
         
            $(app.DE.slider_Ftth).slider('value', radius);
            $(app.DE.sliderHandle_Ftth).text(radius);
            $(app.DE.txtBufferRadius_Ftth).val(radius);
            app.distanceWidget.set('distance', radius / 1000);
            //getNEDetails(app.distanceWidget);
            app.fitBoundsToBuffer();
        }
     
    }

    this.disableBufferRadius = function (disable, type) {
        if (type == app.DE.end_type.START) {
            $(app.DE.slider_A).slider("option", "disabled", disable);
            $(app.DE.txtBufferRadius_A).attr('disabled', true);
        }
        else if (type == app.DE.end_type.END) {
            $(app.DE.slider_B).slider("option", "disabled", disable);
            $(app.DE.txtBufferRadius_B).attr('disabled', true);
        }
        else if (app.DE.end_type.ftth) {
            $(app.DE.slider_Ftth).slider("option", "disabled", disable);
            $(app.DE.txtBufferRadius_Ftth).attr('disabled', true);
        }
    }

    this.compute = function () {
        //if (!app.validateInputFields()) {
        //    alert('Fill the Required Fields!');
        //    return;
        //}

        app.clearMap();

        if (app.detailsMenu) {
            app.detailsMenu.setMap(null);
        }
        if ($(app.DE.coresTextBox).val().length) {
            app.cores_required = $(app.DE.coresTextBox).val();
        }

        if (app.startMarker)
            app.startMarker.setMap(null);
        let startLL = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
        app.startMarker = app.createMarker(startLL, 'Content/images/StartPoint.png', app.DE.end_type.START);
        app.startMarker.addListener('drag', function (LatLong) {
            app.fillLocation(LatLong, app.DE.end_type.START);
        });
        app.addDistanceBuffer_A(app.sliderDefaults.VALUE / 1000, startLL, app.startMarker);

        app.startMarker.addListener('dragend', function () {
            app.fitBoundsToBuffer();
        });


        if (app.endMarker)
            app.endMarker.setMap(null);
        let endLL = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
        app.endMarker = app.createMarker(endLL, 'Content/images/EndPoint.png', app.DE.end_type.END);
        app.endMarker.addListener('drag', function (LatLong) {
            app.fillLocation(LatLong, app.DE.end_type.END);
        });
        app.addDistanceBuffer_B(app.sliderDefaults.VALUE / 1000, endLL, app.endMarker);
        app.endMarker.addListener('dragend', function () {
            app.fitBoundsToBuffer();
        });

        $(app.DE.txtBufferRadius_A).change();
        $(app.DE.txtBufferRadius_B).change();

        app.showPaths();

        var bounds = new google.maps.LatLngBounds();
        bounds.extend(new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng));
        bounds.extend(new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng));
        sf.map.fitBounds(bounds);
        // var zoom = sf.map.getZoom();
        sf.map.setZoom(sf.map.getZoom() - 1);
        $("#btnlist").removeAttr("disabled");
    }

    this.computeFtth = function () {
        //if (!app.validateInputFields()) {
        //    alert('Fill the Required Fields!');
        //    return;
        //}

        $(app.DE.btnComputeFtth).trigger("click");

        //if (app.startMarker)
        //    app.startMarker.setMap(null);
        //let startLL = new google.maps.LatLng($(app.DE.ftthLat).val(), $(app.DE.ftthLng).val());
        //app.startMarker = app.createMarker(startLL, '', app.DE.end_type.ftth);
        //app.startMarker.addListener('drag', function (LatLong) {
        //    app.fillLocation(LatLong, app.DE.end_type.ftth);
        //});
        //$(app.DE.slider_Ftth).slider('value', $(app.DE.txtBufferRadius_Ftth).val());

        //sf.map.setZoom(sf.map.getZoom() - 1);
    }

    this.clearMap = function () {
        if (app.feasiblePaths.length) {
            for (f_path of app.feasiblePaths) {
                if (f_path.length) {
                    for (sub_path of f_path) {
                        if (sub_path) {
                            sub_path.setMap(null);
                        }
                    }
                }
            }
        }

        if (app.startMarker)
            app.startMarker.setMap(null);
        if (app.endMarker)
            app.endMarker.setMap(null);
        if (app.distanceWidget_A)
            app.distanceWidget_A.set("map", null);
        if (app.distanceWidget_B)
            app.distanceWidget_B.set("map", null);
        if (app.distanceWidget)
            app.distanceWidget.set("map", null);
        if (app.savedPaths.length) {
            for (path of app.savedPaths) {
                path.setMap(null);
            }
        }

        if (app.startMarker_saved)
            app.startMarker_saved.setMap(null);
        if (app.endMarker_saved)
            app.endMarker_saved.setMap(null);

        if (app.infowindow)
            app.infowindow.setMap(null);
        if (app.detailsMenu) {
            app.detailsMenu.setMap(null);
        }
        $(app.DE.divLegends).hide();
        app.savedPaths = [];

    }

    this.resetFields = function () {
        $(app.DE.txtCustomerID).val('');
        $(app.DE.txtCustomerName).val('');
        $(app.DE.txtFseasibilityID).val('');
        $(app.DE.txtStartLat).val('');
        $(app.DE.txtStartLng).val('');
        $(app.DE.txtEndLat).val('');
        $(app.DE.txtEndLng).val('');
        $(app.DE.ftthLat).val('');
        $(app.DE.ftthLng).val('');
        $(app.DE.coresTextBox).val('');
        $(app.DE.gSearch).val('');
        $("#txtEntitySearch").val('');
        $("#btnlistFtth").attr("disabled", "disabled");
        app.EnableInputfields();

        app.disableBufferRadius(true, 'A');
        app.disableBufferRadius(true, 'B');
        app.disableBufferRadius(true, 'F');
        $(app.DE.txtBufferRadius_A).val(app.sliderDefaults.VALUE);
        $(app.DE.txtBufferRadius_B).val(app.sliderDefaults.VALUE);
        $(app.DE.txtBufferRadius_Ftth).val(app.sliderDefaults_Ftth.VALUE);
        app.initSliders();
        $(app.DE.sliderHandle_A).text(app.sliderDefaults.VALUE);
        $(app.DE.sliderHandle_B).text(app.sliderDefaults.VALUE);
        $(app.DE.sliderHandle_Ftth).text(app.sliderDefaults_Ftth.VALUE);
        $(app.DE.btnlist).attr("disabled", "disabled");
        $(app.DE.cableTypeddl).val('');
        $(app.DE.hdn_feasibility_id).val('');
        $(app.DE.txtStartPointName).val('');
        $(app.DE.txtEndPointName).val('');

        app.clearMap();
    }

    this.DMStoDD = function (dmsValue) {

        var decimal = /^-?\d+\.\d+$/;
        //var dmsRegex = /^([0-8]?[0-9]|90)°(\s[0-5]?[0-9]')?(\s[0-5]?[0-9](,[0-9])?")?$/;
        if (dmsValue == undefined || dmsValue.length == 0)
            return -1;

        if (decimal.test(dmsValue))
            return dmsValue;

        dmsValue = $.trim(dmsValue);//?.trim();
        let lastChar = dmsValue[dmsValue.length - 1];
        if (lastChar.trim() == 'N' || lastChar.trim() == 'E') {
            dmsValue = dmsValue.slice(0, -1).trim();
        }

        let parts = dmsValue.split('°').join(',').split("'").join(',').split('"').join(',').slice(0, -1).split(',');
        var dd = -1; // degree decimal
        if (parts.length == 3) {
            let degrees = parts[0].trim();
            let minutes = parts[1].trim();
            let seconds = parts[2].trim();

            if (!isNaN(degrees) && (degrees % 1) === 0 && !isNaN(minutes) && (minutes % 1) === 0 && !isNaN(seconds)) {
                dd = parseInt(degrees) + (parseInt(minutes) / 60) + (parseFloat(seconds) / 3600);
                dd = app.roundNumber(dd, 5);
            }
        }
        return dd;
    }

    this.validateInputFields = function () {
        let invalidCount = 0;
        $('.validate').each(function () {
            if (!$(this)[0].checkValidity()) {
                $(this).parent().addClass("has-error");
                invalidCount += 1;
            }
            else {
                $(this).parent().removeClass("has-error");
            }
        });


        if ($(app.DE.cableTypeddl).val() == '') {
            $(app.DE.divCableType).addClass("has-error");
            invalidCount += 1;
        }
        else {
            $(app.DE.divCableType).removeClass("has-error");
        }

        return !invalidCount;
    }

    this.bindEvents = function () {
        $(app.DE.cableTypeddl).change(function () {
            if ($(app.DE.cableTypeddl).val() != '')
                $(app.DE.divCableType).removeClass("has-error");
        });

        $(app.DE.closeModalPopup).on("click", function () {
            $(app.DE.iconPastFeas).removeClass('activeicon');
            $(app.DE.iconBulkFeas).removeClass('activeicon');
            $(app.DE.iconBulkFeasFtth).removeClass('activeicon');

            $(app.DE.iconPastFeasFtth).removeClass('activeicon');
            if ($('#hdnFeasibilityInput').val() == "ftth") {

                //$(app.DE.txtCustomerName).val('');
                //$(app.DE.txtCustomerID).val('');
                //$(app.DE.txtFseasibilityID).val('');
                //$(app.DE.ftthLat).val('');
                //$(app.DE.ftthLng).val('');
                //$(app.DE.txtBufferRadius_Ftth).val('');
                //$(app.DE.slider_Ftth).slider('value', '');
                //$(app.DE.sliderHandle_Ftth).text('');
                //$('#tblbatchLOS tbody').html('<tr><td colspan="10" valign=="center"><strong>No records found.</strong></td></tr>');
                ////app.startMarker.setMap(null);
                ////app.stepPolyline.setMap(null);
                //$('#hdnFeasibilityInput').val('');
                $(app.DE.btnResetFtth).trigger("click");

            }
        });

        $(app.DE.leftPanelOpenClose).on("click", function () {
            var width = $(window).width();
            var mwidth;
            $(app.DE.leftPanel).animate({
                width: "toggle"
            }, function () {
                if ($(this).css('display') === 'none') {
                    $(app.DE.sidectrlleft).removeClass('icon-arrow_right').addClass('icon-arrow_left');
                    if ($("#floating-panel-right").is(":visible")) {

                        $(".modal-manager").css({ 'width': '' + width - 300 + 'px', "left": "0" });
                    }
                    else {
                        $(".modal-manager").css({ 'width': '100%', 'left': '0' });
                    }
                }
                else {
                    $(app.DE.sidectrlleft).removeClass('icon-arrow_left').addClass('icon-arrow_right');
                    if ($("#floating-panel-right").is(":visible")) {

                        $(".modal-manager").css({ 'width': '' + width - 600 + 'px', "left": "300px" });
                    }
                    else {

                        $(".modal-manager").css({ 'width': '' + width - 300 + 'px', "left": "300px" });
                    }
                }
            });
        });

        $(app.DE.lyrRefresh).on("click", function () {
            app.reqver++;
            app.LoadLayersOnMap();
        });

        $(app.DE.rightPanelOpenClose).on("click", function () {
            var width = $(window).width();
            var mwidth;
            $(app.DE.rightPanel).animate({
                width: "toggle"
            }, function () {
                if ($(this).css('display') === 'none') {
                    $(app.DE.sidectrlright).css('right', 0);
                    $(app.DE.sidectrlright).removeClass('icon-arrow_left').addClass('icon-arrow_right');
                    $(app.DE.divLegends).css('right', 50);
                    if ($("#floating-panel-left").is(":visible")) {

                        $(".modal-manager").css({ 'width': '' + width - 300 + 'px', 'left': '300px' });
                    }
                    else {
                        $(".modal-manager").css({ 'width': '100%', 'left': '0' });
                    }
                }
                else {
                    $(app.DE.sidectrlright).css('right', $(this).width() - 40);
                    $(app.DE.sidectrlright).removeClass('icon-arrow_right').addClass('icon-arrow_left');
                    $(app.DE.divLegends).css('right', $(this).width() + 10);
                    if ($("#floating-panel-left").is(":visible")) {

                        $(".modal-manager").css({ 'width': '' + width - 600 + 'px', 'left': '300px' });
                    }
                    else {

                        $(".modal-manager").css({ 'width': '' + width - 300 + '', 'left': '0' });
                    }
                    // below line is to fix reverse class on tree view
                    $('.grpNetwork').find('.hitarea').removeClass('expandable-hitarea').removeClass('lastExpandable-hitarea').addClass('collapsable-hitarea lastCollapsable-hitarea');
                    //$('.grpNetwork').find('.hitarea').removeClass('expandable-hitarea').removeClass('lastExpandable-hitarea').addClass('expandable-hitarea lastExpandable-hitarea');
                    $(':checkbox:not(:enabled)').next().find('label').addClass('disable');
                }
            });
        });

        $(app.DE.selectStartLatLng).on("click", function () {
            if (!$(this).hasClass('disabled')) {
               app.selectLocation('A');    // select A location
                $('.select-icon').css('color', '');
                $(this).css('color', '#50b3ea');
            }
        });

        $(app.DE.selectEndLatLng).on("click", function () {
            if (!$(this).hasClass('disabled')) {
                app.selectLocation('B');    // select B location
                $('.select-icon').css('color', '');
                $(this).css('color', '#50b3ea');
                
            }
        });

        //FtthLatLng
        $(app.DE.ftthStartLatLng).on("click", function () {
            if (!$(this).hasClass('disabled')) {
                
                if (app.fadeMap) {
                    app.fadeMap.setMap(null);
                }
                app.selectLocation('F');    // select FTTH location

                $('.select-icon').css('color', '');
                $(this).css('color', '#50b3ea');
                if (app.polylines.length) {
                    for (var i = 0; i < app.polylines.length; i++) {
                        app.polylines[i].setMap(null);
                    }
                }
            }
        });

        $(app.DE.txtLatLng).change(function () {
           
            var text = $(this).val();
            var dd = app.DMStoDD(text);
            if (!$.isNumeric(text) && dd == -1) {
                $(this).val('');
                $(this).parent().addClass('has-error');
            }
            if (dd == -1) {
                dd = parseFloat(text);
            }
            var name = $(this).data('name');
            var latlng;
            if (text.length) {
                if (name == 'ALat') {

                    app.startLatLng.lat = dd;
                    app.moveMarker('A');
                    if (app.startLatLng.lng != undefined && app.startLatLng.lat != undefined) {
                        latlng = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
                        app.map.setCenter(latlng);
                    }
                    app.fitBoundsToBuffer();
                }
                else if (name == 'ALng') {
                    app.startLatLng.lng = dd;
                    app.moveMarker('A');
                    if (app.startLatLng.lng != undefined && app.startLatLng.lat != undefined) {
                        latlng = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
                        app.map.setCenter(latlng);
                    }
                    app.fitBoundsToBuffer();
                }
                else if (name == 'BLat') {
                    app.endLatLng.lat = dd;
                    app.moveMarker('B');
                    if (app.endLatLng.lng != undefined && app.endLatLng.lat != undefined) {
                        latlng = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
                        app.map.setCenter(latlng);
                    }
                    app.fitBoundsToBuffer();
                }
                else if (name == 'BLng') {
                    app.endLatLng.lng = dd;
                    app.moveMarker('B');
                    if (app.endLatLng.lng != undefined && app.endLatLng.lat != undefined) {
                        latlng = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
                        app.map.setCenter(latlng);
                    }
                    app.fitBoundsToBuffer();
                }
                else if (name == 'Lat') {
                    app.startLatLng.lat = dd;
                    if (app.startLatLng.lng != undefined && app.startLatLng.lat != undefined) 
                        latlng = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
                        app.fillLocationftth(latlng, app.DE.end_type.ftth);
                    
                }
                else if (name == 'Lng') {
                    app.startLatLng.lng = dd;
                    if (app.startLatLng.lng != undefined && app.startLatLng.lat != undefined) 
                        latlng = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
                        app.fillLocationftth(latlng, app.DE.end_type.ftth);
                    
                   
                }
             //  var bounds = new google.maps.LatLngBounds();
              // app.map.fitBounds(bounds);
            }
        });

        $(app.DE.txtBufferRadius).change(function () {
            var text = $(this).val();
            var name = $(this).data('name');
            if (text.length) {
                if (name == 'Start') {
                    app.updateBufferRadius(app.DE.end_type.START, parseInt(text));
                }
                else if (name == 'End') {
                    app.updateBufferRadius(app.DE.end_type.END, parseInt(text));
                }
            }
        });

        $('.validate').change(function () {
            if (!$(this)[0].checkValidity()) {
                $(this).parent().addClass("has-error");
            }
            else {
                $(this).parent().removeClass("has-error");
            }
        });
        $(app.DE.btnComputeFtth).on("click", function () {
            $('#myModal').hide();
            var count = 0;
            if ($("#FtthLat").val() == '' || $("#FtthLng").val() == '') {
                alert("Please select a location on map.");
            }
            else {
                $('#FFTH-layer-panel ul input[type= "checkbox"]').each(function () {
                    if ($(this).is(":checked")) {
                        count = count + 1;
                    }
                });
                if (count > 0) {
                    if (app.tbhtml) {
                        $("#tblbatchLOS tbody").html(app.tbhtml);
                        $('#myModal').show();
                        $("#myModal .modal-body").show();
                    }
                    else
                    {
                        alert("No Records Found");
                    }
                }
                else {
                    alert("Please check entity in FTTH layers");
                }
            }
        });


        $(app.DE.btnCompute).on("click", function () {
            //app.clearMap();
            if (!app.validateInputFields()) {
                alert(MultilingualKey.Required_Fields);
                return;
            }
            if (parseInt($("#cores").val()) > parseInt($("#cableType option:selected").text())) {
                alert("Required cores must be less than or equal to Cable Type");
                return;
            }

            let quotes = false;
            $(".checkQuotes").each(function () {
                if ($(this).val().includes('"') || $(this).val().includes("'")) {
                    $(this).parent().addClass("has-error");
                    quotes = true;
                    return;
                }
                else {
                    $(this).parent().removeClass("has-error");
                }

                $(this).val(app.htmlUnescape($(this).val()));
            });

            if (quotes) {
                alert("Quotes are not allowed!");
                return;
            }

            app.compute();
            event.preventDefault();
        });

        $(app.DE.btnReset).on("click", function (e) {
            e.preventDefault();
            app.resetFields();
        });
        $(app.DE.btnResetFtth).on("click", function (e) {
            e.preventDefault();
            app.resetFields();
            $(".details-menu").remove();
            CheckAllLayrs('Layers', false);
            if (app.fadeMap) {
                app.fadeMap.setMap(null);
            }
            if (app.distanceWidget) {
                app.distanceWidget.set("map", null);
                app.distanceWidget = null;
            }
            if (app.polylines.length) {
                for (var i = 0; i < app.polylines.length; i++) {
                    app.polylines[i].setMap(null);
                }
            }
        });

        $(app.DE.iconBulkFeasFtth).on("click", function () {
            app.feasibility_saved = [];
            app.rowHistMap = [];
            app.bulkFeasibility_popUp.LoadModalDialog('ModalPopUp_bulk', 'BulkFeasibility/Ftth', '{}', MultilingualKey.Bulk_Feasibility, 'modal-xl');
        });

        $(app.DE.iconBulkFeas).on("click", function () {
            app.feasibility_saved = [];
            app.rowHistMap = [];
            app.bulkFeasibility_popUp.LoadModalDialog('ModalPopUp_bulk', 'BulkFeasibility/Index', '{}', MultilingualKey.Bulk_Feasibility, 'modal-xl');
        });

        $(app.DE.iconPastFeas).on("click", function () {

            app.pastFeasibility_popUp.LoadModalDialog('ModalPopUp_hist', 'FeasibilityDetails/pastFeasibilities', '{}', MultilingualKey.Feasibility_History, 'modal-xxl');
        });

        $(app.DE.iconPastFeasFtth).on("click", function () {
            $('#hdnFeasibilityInput').val('');
            app.pastFeasibility_popUp.LoadModalDialog('ModalPopUp_hist', 'FeasibilityDetails/pastFeasibilitiesFtth', '{}', "FTTH Feasibility History", 'modal-xxl');
        });

        $(app.DE.multicons).on("click", function (e) {
            e.preventDefault();
            $(this).siblings().removeClass('activeicon');
            $(this).addClass('activeicon');
        });


        $(app.DE.feasTypeWrapper).on("click", function (e) {
            e.preventDefault();
            $(this).siblings().removeClass('activeWrapper');
            $(this).addClass('activeWrapper');
        });

        $(app.DE.spnLogout).on("click", function () {
            app.SignOut();
        });

        $(app.DE.spnCableType).on("click", function () {

            ajaxReq('main/hasAccess', { moduleAbbr: 'BOM' }, true, function (resp) {
                if (resp.Data.status == "OK") {
                    app.getCableType();
                }
                else
                    alert('User doesn\'t have the access to this Module!');
            }, true, true);

            $(app.DE.logoutpannel).slideToggle();
        });

        $(document).on('click', app.DE.LayerAccordin, function (e) {

            $(this).toggleClass('fa-plus').toggleClass('fa-minus');
            $(this).closest('div .layers').find('.mainlyr').slideToggle();
            $(this).closest('div .layers').find('#dvNetworkActions').slideToggle();
        });

        $(app.DE.chkAll).change(function () {
             
            //  $("input:checkbox").prop('checked', $(this).prop("checked")); // 

            if (this.checked) {
                //$('.network').find('input[type="checkbox"]').prop("checked", true);
                //$("input[name='networkALL']:checkbox").prop('checked', true);

                $('.network').find('input[type="checkbox"]').filter("[data-networktype!='L']").not(":disabled").prop("checked", true);
                $("input[name='networkALL']:checkbox").filter("[data-all!='L']").not(":disabled").not(":disabled").prop('checked', true);


                $(".checkbox-customgrp").not(":disabled").prop('checked', true)
            }
            else {
                 
                $('.network').find('input[type="checkbox"]').not(":disabled").prop("checked", false);
                $("input[name='networkALL']:checkbox").not(":disabled").prop('checked', false);
                $(".checkbox-customgrp").not(":disabled").prop('checked', false);


            }
        });

        $(".checkQuotes").keypress(function (e) {
            var k = "keyCode" in e ? e.keyCode : e.which;
            console.log(k);
            $return = (k == 39 || k == 34);
            if ($return) {
                return false;
            }
        });
    }

    this.toggleBaseMap = function () {
        app.toggleBaseMap_popUp.LoadModalDialog('ModalPopUp_toggleBaseMap', 'Main/MapManager', '{}', "Map Manager", 'modal-sm');
    }

    this.htmlUnescape = function (str) {
        return str
            //.replace(/&quot;/g, '"')
            //.replace(/&#39;/g, "'")
            .replace(/&lt;/g, '<')
            .replace(/&gt;/g, '>')
            .replace(/&#8208;/g, '-')
            .replace(/&amp;/g, '&');
    }

    this.setMapType = function (mapType) {
        var mapTypeID = google.maps.MapTypeId.ROADMAP;
        switch (mapType) {
            case 'roadmap':
                mapTypeID = google.maps.MapTypeId.ROADMAP;
                break;
            case 'terrain':
                mapTypeID = google.maps.MapTypeId.TERRAIN;
                break;
            case 'satellite':
                mapTypeID = google.maps.MapTypeId.SATELLITE;
                break;
            case 'hybrid':
                mapTypeID = google.maps.MapTypeId.HYBRID;
                break;
        }
        sf.map.setMapTypeId(mapTypeID);
    }

    this.showMapLyr = function (lyrName) {
        if (!app.gMapObj)
            app.gMapObj = {};

        if (app.gMapObj[lyrName]) {
            app.gMapObj[lyrName].setMap(null);
            app.gMapObj[lyrName] = undefined;
            return;
        }

        switch (lyrName) {
            case 'Weather':
                app.gMapObj[lyrName] = new google.maps.weather.WeatherLayer();
                app.map.setZoom(12);
                break;
            case 'Cloud':
                app.gMapObj[lyrName] = new google.maps.weather.CloudLayer();
                app.map.setZoom(6);
                break;
            case 'Panoramio':
                app.gMapObj[lyrName] = new google.maps.panoramio.PanoramioLayer();
                break;
            case 'Traffic':
                app.gMapObj[lyrName] = new google.maps.TrafficLayer();
                break;
        }

        if (app.gMapObj[lyrName])
            app.gMapObj[lyrName].setMap(app.map);
    }

    this.removeAddonLyr = function (lyrName) {
        if (app.gMapObj) {
            if (app.gMapObj[lyrName])
                app.gMapObj[lyrName].setMap(null);
        }
    }

    this.toggleWhiteMap = function () {
        var mt = sf.map.getMapTypeId();
        if (mt != 'coordinate')
            app.showWhiteMap();
        else
            sf.map.setMapTypeId(google.maps.MapTypeId.ROADMAP);
    }


    this.showWhiteMap = function () {

        function CoordMapType() {
        }

        CoordMapType.prototype.tileSize = new google.maps.Size(1024, 256);
        CoordMapType.prototype.maxZoom = 21;

        CoordMapType.prototype.getTile = function (coord, zoom, ownerDocument) {
            var div = ownerDocument.createElement('div');
            div.style.width = this.tileSize.width + 'px';
            div.style.height = this.tileSize.height + 'px';
            div.style.backgroundColor = '#fff';
            return div;
        };

        CoordMapType.prototype.name = 'Tile #s';
        CoordMapType.prototype.alt = 'Tile Coordinate Map Type';
        var coordinateMapType = new CoordMapType();

        sf.map.mapTypes.set('coordinate', coordinateMapType);
        sf.map.setMapTypeId('coordinate');
    }

    this.getCableType = function () {
        var url = "Main/GetCableType";
        popup.LoadModalDialog('ModalPopUp_bomStngs', url, {}, MultilingualKey.BOM_Settings, 'modal-lg');
    }

    this.deleteCableType = function (systemId, is_used) {
        if (is_used == 'True') {
            alert('This record cannot be deleted as the Cable Type is in use.');
        }
        else {
            showConfirm(MultilingualKey.delete_record, function () {
                ajaxReq('main/deleteCableType', { systemId: parseInt(systemId) }, true, function (resp) {
                    if (resp.Data.status == "OK") {
                        alert(resp.Data.message);
                        sf.GetCableTypeLst();
                    }
                    else
                        alert(resp.message);
                }, true, true);
            });
        }
    }
    this.GetCableTypeLst = function () {
        ajaxReq('Main/GetCableTypeLst', {}, true, function (resp) {
            $("#CableType_list").html(resp);
        }, false, false);
    }
    //this.GetpastFeasibilities = function () {
    //    ajaxReq('FeasibilityDetails/pastFeasibilities', {}, true, function (resp) {
    //        $("#divList").html(resp);
    //    }, false, false);
    //}

    //textbox can shown only numeric datatype
    this.allowOnlyNumber = function (evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;


        return true;
    }

    //textbox can shown only numeric datatype with one dot
    // Allow only 1 decimal point ('.')...

    this.allowNumberwithDot = function (evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }

    this.showHistory = function (_systemId, _entityType) {

        var formURL = "Main/GetHistory";

        //ajaxReq('Main/GetHistory', { systemId: _systemId, eType: _entityType }, true, function (resp) {
        //    $("#historyRecord").html(resp);

        //}, false, false);
        popup.LoadModalDialog('ChildPopUp', formURL, { systemId: _systemId, eType: _entityType }, MultilingualKey.History, 'modal-xxl');
        $("#btnBomHistoryExport").on("click", function () {
            sf.ExportBomHistory(_systemId);
        });
    }
    this.showBomRecord = function (_systemId) {

        ajaxReq('Main/GetCableTypeLst', { systemId: _systemId }, true, function (resp) {
            $("#CableType_list").html(resp);
        }, false, false);
        //popup.LoadModalDialog(formURL, { systemId: _systemId, eType: _entityType }, titleText, 'modal-lg');
    }
    this.ExportBomRecord = function (systemId) {
        window.location = appRoot + 'Main/ExportBomRecord?systemId=' + systemId;
    }
    this.ExportBomHistory = function (systemId) {
        window.location = appRoot + 'Main/ExportBomHistory?systemId=' + systemId;
    }
    //$('.icon-history').on("click", function () {
    //    popup.LoadModalDialog('FeasibilityDetails/PastFeasibilityView', { feasibilityDetailsInfo: "" }, 'Feasibility History', 'modal-xxl');
    //})
    this.changeColor = function (tableId) {

        var tbl = $('#' + tableId);
        var rowCount = $('#' + tableId + ' tr').length;

        if (rowCount > 0) {
            for (var i = 0; i < tbl[0].rows.length; i++) {
                for (var j = 0; j < tbl[0].rows[i].cells.length; j++) {
                    if (i > 1) {
                        if (tbl[0].rows[i].cells[j].innerHTML != tbl[0].rows[i - 1].cells[j].innerHTML) {
                            tbl[0].rows[i - 1].cells[j].style.color = "red";
                        }
                    }
                }
            }
        }

        $(tbl).find('tfoot td').removeAttr("style")
    }
    this.feasibilityDetailsInformation = function () {
        app.feasibilityDetailsInfo = [];
        app.feasibilityGeometry = [];
        for (var i = 0; i < app.feasiblePaths.length; i++) {
            var feasibilityInfo = {};

            var subPath = app.feasiblePaths[i];
            //subPath


            if (subPath.length) {
                for (path of subPath) {
                    var insidepath = app.insidePaths.filter(x => x.system_id == path.system_id);
                    var objfeasibilityGeometry = {};
                    var geom = '';
                    for (ll of path.getPath().getArray()) {
                        geom += ll.lng() + ' ' + ll.lat() + ', ';
                    }
                    switch (path.type) {
                        case app.cable_types.INSIDE_P:
                            feasibilityInfo.inside_P_Distance = parseFloat(insidepath[0].length).toFixed(2);
                            break;
                        case app.cable_types.INSIDE_A:
                            feasibilityInfo.inside_A_Distance = parseFloat(insidepath[0].length).toFixed(2);
                            break;
                        case app.cable_types.INSIDE:
                            feasibilityInfo.insideDistance = parseFloat(insidepath[0].length).toFixed(2);
                            break;
                        case app.cable_types.OUTSIDE_A_END:
                            feasibilityInfo.outside_A_Distance = parseFloat(path.distance).toFixed(2);
                            break;
                        case app.cable_types.OUTSIDE_B_END:
                            feasibilityInfo.outside_B_Distance = parseFloat(path.distance).toFixed(2);
                            break;
                        case app.cable_types.LMC_A_END:
                            feasibilityInfo.lmc_A_Distance = parseFloat(path.distance).toFixed(2);
                            break;
                        case app.cable_types.LMC_B_END:
                            feasibilityInfo.lmc_B_Distance = parseFloat(path.distance).toFixed(2);
                            break;
                    }

                    feasibilityInfo.totalDistance = parseFloat(path.totalDistance).toFixed(2);
                    feasibilityInfo.isSelected = path.isSelected;
                    objfeasibilityGeometry.isSelected = feasibilityInfo.isSelected;
                    objfeasibilityGeometry.cable_geometry = geom;
                    objfeasibilityGeometry.path_type = path.type;
                    if (path.type == 'outside') {
                        objfeasibilityGeometry.cable_length = parseFloat(path.self_length).toFixed(2);
                    }
                    else if (path.type.includes("inside")) {
                        objfeasibilityGeometry.cable_length = parseFloat(path.self_length).toFixed(2);
                    }
                    else {
                        objfeasibilityGeometry.cable_length = parseFloat(path.distance).toFixed(2);
                    }
                    objfeasibilityGeometry.system_id = (path.type.includes("inside")) ? insidepath[0].system_id : 0;
                    objfeasibilityGeometry.network_status = (path.type.includes("inside")) ? insidepath[0].network_status : "";
                    objfeasibilityGeometry.available_cores = (path.type.includes("inside")) ? insidepath[0].available_cores : 0;
                    objfeasibilityGeometry.total_cores = (path.type.includes("inside")) ? insidepath[0].total_cores : 0;
                    app.feasibilityGeometry.push(objfeasibilityGeometry);
                }
                let totalInsideLength = (app.FeasibilityDistances.insideDistance ? app.FeasibilityDistances.insideDistance : 0) +
                    (app.FeasibilityDistances.inside_P_Distance ? app.FeasibilityDistances.inside_P_Distance : 0) +
                    (app.FeasibilityDistances.inside_A_Distance ? app.FeasibilityDistances.inside_A_Distance : 0);
                app.feasibilityStatus = app.CalcfeasibilityStatus(totalInsideLength, app.FeasibilityDistances.totalDistance);

                feasibilityInfo.subPathCount = subPath.length;

                feasibilityInfo.core_level_result = app.is_core_feasibile ? 'Feasible' : 'Not Feasible';
                feasibilityInfo.feasibility_result = app.feasibilityStatus;
                app.feasibilityDetailsInfo.push(
                    {
                        "feasibilityName": $("#feasibilityID").val(),
                        "totalLength": feasibilityInfo.totalDistance,
                        "ExistingLength_P": app.FeasibilityDistances.inside_P_Distance ? app.FeasibilityDistances.inside_P_Distance.toFixed(2) : 0,
                        "ExistingLength_A": app.FeasibilityDistances.inside_A_Distance ? app.FeasibilityDistances.inside_A_Distance.toFixed(2) : 0,
                        "NewOutside_A_Length": feasibilityInfo.outside_A_Distance,
                        "NewOutside_B_Length": feasibilityInfo.outside_B_Distance,
                        "lmc_A_End_Path": feasibilityInfo.lmc_A_Distance,
                        "lmc_B_End_Path": feasibilityInfo.lmc_B_Distance,
                        "isSelected": feasibilityInfo.isSelected,
                        "subPathCount": feasibilityInfo.subPathCount,
                        "buffer_radius_a": parseFloat($("#startBuffer").val()),
                        "buffer_radius_b": parseFloat($("#endBuffer").val()),
                        "core_level_result": feasibilityInfo.core_level_result,
                        "feasibility_result": feasibilityInfo.feasibility_result
                    });
            }



        }
    }
    /*this.getCableGeometry = function (selectedIndex) {
        var subPath = app.feasiblePaths[selectedIndex];
        var x = 0;
        if (subPath.length) {
            for(path of subPath)
            {
                // var objfeasibilityGeometry = {};
                var geom = '';
                for(ll of path.getPath().getArray()) {
                        geom += ll.lng() + ' ' + ll.lat() + ', ';
                    }
                    $('#objGeometryPath_Type_' + x).val(path.type);
                    $('#objGeometryCable_Length_' + x).val(path.distance);
                    $('#objGeometryIsSelected_' + x).val(path.isSelected);
                    $('#objGeometryCable_geometry_' + x).val(geom);
                }              
                x++;
           
        }
    }*/
    //#regin MapLayer

    this.showMapLayer = function () {
        var formURL = "Main/GetMapLayer";

        ajaxReq(formURL, {}, true, function (resp) {
            $("#MapLayers").html(resp);
        }, false, false);
        //popup.LoadModalDialog(formURL, { systemId: _systemId, eType: _entityType }, titleText, 'modal-lg');
    }

    this.LoadLayersOnMap = function () {
        app.layerDetails = [];
        LayerFilters = [];
        app.SetRegionProvinceFilters();
        app.SetProjectSpecFilters();
        app.SetCableFilters();
        app.SetBuildingRFSFilters();
        app.SetFaultFilters();
        app.SetOwnershipFilters();


        // app.clearDraggableLibrary();

        if (app.ActiveRegionlayers.length == 0 || app.ActiveProvincelayers.length == 0) {
            alert(MultilingualKey.select_Region_Province);
            return;
        }

        if (app.ActiveProvincelayers.length > $("#hdnMapRegionProvinceLimit").val()) {
            alert($.validator.format(MultilingualKey.SF_GBL_GBL_JQ_FRM_015, $("#hdnMapRegionProvinceLimit").val()));
            return;
        }

        //show or hide labels...
        app._showLabel = false;//$(app.DE.chkShowLabelOnMap).prop("checked");

        //show loader
        $(app.DE.lyrRefresh).addClass('eaSpin');

        //blank layer object..
        app.layerManager = [];

        //load reg , province layers..
        var regProvinceFilter = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }];

        var layerParam = { Name: 'REG,PRO', DisplayName: "Region_Province_Layer", Filters: regProvinceFilter, MapFilePath: app.mapDirPath + "NetworkEntities" + (app._showLabel ? 'Label' : 'NoLabel') + ".map", isNetworkLayer: false, network_status: '', isWithLabel: false };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);




        var polygonLayer = app.getActivePolygonlayer(false);
        var polygonlayerFilter = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'faultFilter', value: app.filterFaultvalue }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];
        var layerParam = { Name: polygonLayer.length > 0 ? polygonLayer.join(",") : '', DisplayName: "Polygon Layer", Filters: polygonlayerFilter, MapFilePath: app.mapDirPath + "NetworkEntities" + (app._showLabel ? 'Label' : 'NoLabel') + ".map", isNetworkLayer: true, network_status: '', isWithLabel: false };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);

         //--------Map request-- OPTIMIZATION ---- START---





       //// app.ActiveDormentlayers = app.getActiveNetworkLayersNew('D');
       // //if (app.ActiveDormentlayers.length > 0) {

       // var genericLayerFilter = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'prjectspecificationFilter', value: app.filterprojectvalue }, { Field: 'bldRFSFilter', value: app.buildingRFSFilter }, { Field: 'faultFilter', value: '1 = 1' }, { Field: 'cableFilter', value: app.filtercablevalue }, { Field: 'ownershipFilter', value: app.filterOwnershipvalue }, { Field: 'splitterFilter', value: '1 = 1' }, { Field: 'sectorFilter', value: '1 = 1' }, { Field: 'PODFilter', value: '1 = 1' }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];

       // app.getAllActiveNetworkLayers(genericLayerFilter, false);

       // var layerParam = { Name: app.allActiveLayers.length > 0 ? app.allActiveLayers.join(",") : '', DisplayName: "All_Layers_without_Label", Filters: genericLayerFilter, MapFilePath: app.mapDirPath + "NetworkEntitiesNoLabel.map", isNetworkLayer: true, network_status: 'All', isWithLabel: false };
       // var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
       // app.addNewOverlay(overlayLayer);






         //--------Map request-- OPTIMIZATION ---- END---


        //load Dorment layers..
        app.ActiveDormentlayers = app.getActiveNetworkLayersNew('D');
        ////if (app.ActiveDormentlayers.length > 0) {

        var DormentLayerFilter = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'layerFilter', value: "[network_status] in ('D')" }, { Field: 'prjectspecificationFilter', value: app.filterprojectvalue }, { Field: 'bldRFSFilter', value: app.buildingRFSFilter }, { Field: 'faultFilter', value: '1 = 1' }, { Field: 'cableFilter', value: app.filtercablevalue }, { Field: 'ownershipFilter', value: app.filterOwnershipvalue }, { Field: 'splitterFilter', value: '1 = 1' }, { Field: 'sectorFilter', value: '1 = 1' }, { Field: 'PODFilter', value: '1 = 1' }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];
        var layerParam = { Name: app.ActiveDormentlayers.length > 0 ? app.ActiveDormentlayers.join(",") : '', DisplayName: "Dorment_Layers", Filters: DormentLayerFilter, MapFilePath: app.mapDirPath + "NetworkEntities" + (app._showLabel ? 'Label' : 'NoLabel') + ".map", isNetworkLayer: true, network_status: 'D', isWithLabel: false };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);

        //load planned layers..
        app.ActivePlannedlayers = app.getActiveNetworkLayersNew('P');
        //if (app.ActivePlannedlayers.length > 0) {

        var plannedLayerFilter = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'layerFilter', value: "[network_status] in ('P')" }, { Field: 'prjectspecificationFilter', value: app.filterprojectvalue }, { Field: 'bldRFSFilter', value: app.buildingRFSFilter }, { Field: 'faultFilter', value: '1 = 1' }, { Field: 'cableFilter', value: app.filtercablevalue }, { Field: 'ownershipFilter', value: app.filterOwnershipvalue }, { Field: 'splitterFilter', value: '1 = 1' }, { Field: 'sectorFilter', value: '1 = 1' }, { Field: 'PODFilter', value: '1 = 1' }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];
        var layerParam = { Name: app.ActivePlannedlayers.length > 0 ? app.ActivePlannedlayers.join(",") : '', DisplayName: "Planned_Layers", Filters: plannedLayerFilter, MapFilePath: app.mapDirPath + "NetworkEntities" + (app._showLabel ? 'Label' : 'NoLabel') + ".map", isNetworkLayer: true, network_status: 'P', isWithLabel: false };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);

        //}

        //load As Built layers..
        app.ActiveAsBuiltlayers = app.getActiveNetworkLayersNew('A');
        //if (app.ActiveAsBuiltlayers.length > 0) {

        var AsBuiltLayerFilter = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'layerFilter', value: "[network_status] in ('A')" }, { Field: 'prjectspecificationFilter', value: app.filterprojectvalue }, { Field: 'bldRFSFilter', value: app.buildingRFSFilter }, { Field: 'faultFilter', value: '1 = 1' }, { Field: 'cableFilter', value: app.filtercablevalue }, { Field: 'ownershipFilter', value: app.filterOwnershipvalue }, { Field: 'splitterFilter', value: '1 = 1' }, { Field: 'sectorFilter', value: '1 = 1' }, { Field: 'PODFilter', value: '1 = 1' }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];
        var layerParam = { Name: app.ActiveAsBuiltlayers.length > 0 ? app.ActiveAsBuiltlayers.join(",") : '', DisplayName: "AsBuilt_Layers", Filters: AsBuiltLayerFilter, MapFilePath: app.mapDirPath + "NetworkEntities" + (app._showLabel ? 'Label' : 'NoLabel') + ".map", isNetworkLayer: true, network_status: 'A', isWithLabel: false };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);

        ///////////////////// Layer with labels 



        var polygonLayerWithLabel = app.getActivePolygonlayer(true);
        var polygonlayerFilterWithLabel = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'faultFilter', value: '1=1' }];
        var layerParam = { Name: polygonLayerWithLabel.length > 0 ? polygonLayerWithLabel.join(",") : '', DisplayName: "Polygon Layer with lebel", Filters: regProvinceFilter, MapFilePath: app.mapDirPath + "NetworkEntities" + 'Label' + ".map", isNetworkLayer: true, network_status: '', isWithLabel: false };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);






        //Map request optimization------------START -- WITH LABEL


        //var genericLayerFilterWithLabel = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'prjectspecificationFilter', value: app.filterprojectvalue }, { Field: 'bldRFSFilter', value: app.buildingRFSFilter }, { Field: 'faultFilter', value: '1 = 1' }, { Field: 'cableFilter', value: app.filtercablevalue }, { Field: 'ownershipFilter', value: app.filterOwnershipvalue }, { Field: 'splitterFilter', value: '1 = 1' }, { Field: 'sectorFilter', value: '1 = 1' }, { Field: 'PODFilter', value: '1 = 1' }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];

        //app.getAllActiveNetworkLayers(genericLayerFilterWithLabel, true);

        //var layerParam = { Name: app.allActiveLayers.length > 0 ? app.allActiveLayers.join(",") : '', DisplayName: "All_Layers_with_Label", Filters: genericLayerFilterWithLabel, MapFilePath: app.mapDirPath + "NetworkEntities" + 'Label' + ".map",  isNetworkLayer: true, network_status: 'All', isWithLabel: true };
        //var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        //app.addNewOverlay(overlayLayer);


        //Map Request Optimization --------------END






         //--------------------------OLD-------------------------

        //load Dormant layers with labels
        app.ActiveDormantlayersWithlabels = app.getActiveNetworkLayersWithLabel('D');
        //if (app.ActiveDormantlayersWithlabels.length > 0) {

        var dormantLayerFilterWithLabel = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'layerFilter', value: "[network_status] in ('D')" }, { Field: 'prjectspecificationFilter', value: app.filterprojectvalue }, { Field: 'bldRFSFilter', value: app.buildingRFSFilter }, { Field: 'faultFilter', value: '1 = 1' }, { Field: 'cableFilter', value: app.filtercablevalue }, { Field: 'ownershipFilter', value: app.filterOwnershipvalue }, { Field: 'splitterFilter', value: '1 = 1' }, { Field: 'sectorFilter', value: '1 = 1' }, { Field: 'PODFilter', value: '1 = 1' }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];
        var layerParam = { Name: app.ActiveDormantlayersWithlabels.length > 0 ? app.ActiveDormantlayersWithlabels.join(",") : '', DisplayName: "Dorment_LayersLabels", Filters: dormantLayerFilterWithLabel, MapFilePath: app.mapDirPath + "NetworkEntities" + 'Label' + ".map", isNetworkLayer: true, network_status: 'D', isWithLabel: true };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);

        ////load planned layers with labels
        app.ActivePlannedlayersWithlabels = app.getActiveNetworkLayersWithLabel('P');
        //if (app.ActivePlannedlayersWithlabels.length > 0) {

        var plannedLayerFilterWithLabel = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'layerFilter', value: "[network_status] in ('P')" }, { Field: 'prjectspecificationFilter', value: app.filterprojectvalue }, { Field: 'bldRFSFilter', value: app.buildingRFSFilter }, { Field: 'faultFilter', value: '1 = 1' }, { Field: 'cableFilter', value: app.filtercablevalue }, { Field: 'ownershipFilter', value: app.filterOwnershipvalue }, { Field: 'splitterFilter', value: '1 = 1' }, { Field: 'sectorFilter', value: '1 = 1' }, { Field: 'PODFilter', value: '1 = 1' }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];
        var layerParam = { Name: app.ActivePlannedlayersWithlabels.length > 0 ? app.ActivePlannedlayersWithlabels.join(",") : '', DisplayName: "Planned_LayersLabels", Filters: plannedLayerFilterWithLabel, MapFilePath: app.mapDirPath + "NetworkEntities" + 'Label' + ".map", isNetworkLayer: true, network_status: 'P', isWithLabel: true };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);

        //}
        //load As-Built layers with labels
        app.ActiveAsBuiltlayersWithlabels = app.getActiveNetworkLayersWithLabel('A');
        //if (app.ActiveAsBuiltlayersWithlabels.length > 0) {

        var AsBuiltLayerFilterWithLabel = [{ Field: 'regionFilter', value: app.RegionFilter }, { Field: 'provinceFilter', value: app.ProvinceFilter }, { Field: 'layerFilter', value: "[network_status] in ('A')" }, { Field: 'prjectspecificationFilter', value: app.filterprojectvalue }, { Field: 'bldRFSFilter', value: app.buildingRFSFilter }, { Field: 'faultFilter', value: '1 = 1' }, { Field: 'cableFilter', value: app.filtercablevalue }, { Field: 'ownershipFilter', value: app.filterOwnershipvalue }, { Field: 'splitterFilter', value: '1 = 1' }, { Field: 'sectorFilter', value: '1 = 1' }, { Field: 'PODFilter', value: '1 = 1' }, { Field: 'NetworkFilter', value: '1 = 1' }, { Field: 'AutoPlanningNetworkFilter', value: '1 = 1' }];
        var layerParam = { Name: app.ActiveAsBuiltlayersWithlabels.length > 0 ? app.ActiveAsBuiltlayersWithlabels.join(",") : '', DisplayName: "AsBuilt_LayersLabels", Filters: AsBuiltLayerFilterWithLabel, MapFilePath: app.mapDirPath + "NetworkEntities" + 'Label' + ".map", isNetworkLayer: true, network_status: 'A', isWithLabel: true };
        var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        app.addNewOverlay(overlayLayer);
        //--------------------------OLD-------------------------
        //}


        //testing code...
        //var layerParam = { Name:'DCT,CBL', DisplayName: "test", Filters: [], MapFilePath: app.mapDirPath + "Test.map", isNetworkLayer: true, network_status: 'D', isWithLabel: false };
        //var overlayLayer = createOverlayLayer(layerParam, true, function () { $(app.DE.lyrRefresh).removeClass('eaSpin'); });
        //app.addNewOverlay(overlayLayer);
        // }
        app.reLoadGoogleLayers();

    }
   /* -----load layer for ftth----(by Navi)*/
  


    this.addPolygonLayer = function (layers, isWithLabel) {
         
        if (isWithLabel) {
            var areaObj = $(app.DE.ulNetworkLayers + " li input[data-layername='Area']");
            var isLabelOn = $(areaObj).next().next().children('.chknetwork-L').is(':checked')
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='Area']").is(':checked') && layers.includes('ARA') == false && isLabelOn) {
                //layers.push('ARA');
                layers.splice(0, 0, 'ARA');
            }
            var surveyObj = $(app.DE.ulNetworkLayers + " li input[data-layername='Area']");
            var isSurveyLabelOn = $(surveyObj).next().next().children('.chknetwork-L').is(':checked')
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='SurveyArea']").is(':checked') && layers.includes('SVA') == false && isSurveyLabelOn) {
                // layers.push('SVA');
                layers.splice(1, 0, 'SVA');
            }
            var subareaObj = $(app.DE.ulNetworkLayers + " li input[data-layername='Area']");
            var issubareaLabelOn = $(subareaObj).next().next().children('.chknetwork-L').is(':checked')
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='SubArea']").is(':checked') && layers.includes('SBA') == false && issubareaLabelOn) {
                //layers.push('SBA');
                layers.splice(2, 0, 'SBA');
            }
        } else {
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='Area']").is(':checked') && layers.includes('ARA') == false) {
                layers.push('ARA');
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='SurveyArea']").is(':checked') && layers.includes('SVA') == false) {
                layers.push('SVA');
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='SubArea']").is(':checked') && layers.includes('SBA') == false) {
                layers.push('SBA');
            }
        }
        return layers;
    }
    this.addNewOverlay = function (lyrObj) {
        var foundFlag = true;
        var newLyr = { Name: lyrObj.name, layerObject: lyrObj, Opacity: lyrObj.opacity, visible: true }
        for (var i = 0; i < app.layerManager.length; i++) {
            if (app.layerManager[i].Name == lyrObj.name) {
                app.layerManager[i] = newLyr;
                foundFlag = false;
                break;
            }
        }
        if (foundFlag)
            app.layerManager.push(newLyr);
        //app.reLoadGoogleLayers();
    }
    this.reLoadGoogleLayers = function () {
        app.removeAllWMSLayers();
        $.each(app.layerManager, function (i, item) {
            if (item.visible)
                app.map.overlayMapTypes.push(item.layerObject);
        });
    }
    this.removeAllWMSLayers = function () {
        for (i = app.map.overlayMapTypes.length - 1; i >= 0; i--) {
            app.map.overlayMapTypes.removeAt(i);
        }
    }

    //Entity mapserver Optimization -------------------------START---WITHOUT LABEL
    this.getAllActiveNetworkLayers = function (layerFilter, withLabel) {
        app.allActiveLayers = [];

        $.each(app.geomTypes, function (i, itm) {
            $.each($(app.DE.ulNetworkLayers + " li[data-geomtype='" + itm + "'] .mainLyr:checked").sort(function (a, b) {
                return parseInt($(a).data('maplayerseq')) - parseInt($(b).data('maplayerseq'))
            }), function (i, e) {
                var status = [];
                var $ele = $(e);
                var layerid = $ele.data("layerid");
                var isLabelChecked = $("#chk_netL_" + layerid).is(":checked");
                var isConsidered = withLabel === isLabelChecked ;


                if ($ele.is(":checked") && isConsidered) {
                    //if (!isNetworkTicket) {
                    //    $("#chk_netD_" + layerid).is(":checked") ? status.push("'D'") : "";
                    //}
                    $("#chk_netD_" + layerid).is(":checked") ? status.push("'D'") : "";
                    $("#chk_netP_" + layerid).is(":checked") ? status.push("'P'") : "";
                    $("#chk_netA_" + layerid).is(":checked") ? status.push("'A'") : "";

                    if (status.length != 0) {
                        var entityList = $ele.data("mapabbr").split(",");
                        $.each(entityList, function (i, e) {
                            layerFilter.push({ Field: e + 'layerFilter', value: "[network_status] in (" + status.join(",") + ")" });
                        });
                    }
                    if ($ele.data('isnetworktyperequired') == "True" || $ele.data("entity-geom-type") != 'Polygon') {
                        app.allActiveLayers.push($ele.data("mapabbr"));
                    }
                }
            });
        });
    }
    //Entity mapserver Optimization -------------------------END
    this.getActivePolygonlayer = function (isWithlabel) {
         
        var layers = [];
        if (!isWithlabel) {
            //$.each($(app.DE.ulNetworkLayers + ' li input[data-is-networktype-required="False"][data-is-network-entity="False"]'), function () {
            //    var mapAbbr = $(this).data().mapabbr;
            //    if ($(this).is(':checked') && layers.includes(mapAbbr) == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='" + mapAbbr + "'] .chknetwork-L:checked").length == 0) {
            //        layers.push(mapAbbr);
            //    }
            //})

            if ($(app.DE.ulNetworkLayers + " li input[data-layername='Area']").is(':checked') && layers.includes('ARA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='ARA'] .chknetwork-L:checked").length == 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="Area"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="Area"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('ARA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='SurveyArea']").is(':checked') && layers.includes('SVA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='SVA'] .chknetwork-L:checked").length == 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="SurveyArea"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="SurveyArea"] #MinMaxZoom').attr('data-max-zoom');
                // var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('SVA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='SubArea']").is(':checked') && layers.includes('SBA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='SBA'] .chknetwork-L:checked").length == 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="SubArea"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="SubArea"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('SBA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='Customer'][data-entity-geom-type='Polygon']").is(':checked') && layers.includes('CUS') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='CUS'] .chknetwork-L:checked").length == 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="SubArea"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="SubArea"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('CUS');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='ProjectArea']").is(':checked') && layers.includes('PRA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='PRA'] .chknetwork-L:checked").length == 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="ProjectArea"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="ProjectArea"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('PRA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='DSA']").is(':checked') && layers.includes('DSA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='DSA'] .chknetwork-L:checked").length == 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="DSA"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="DSA"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('DSA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='CSA']").is(':checked') && layers.includes('CSA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='CSA'] .chknetwork-L:checked").length == 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="CSA"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="CSA"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('CSA');
                // }
            }
            //if ($(app.DE.ulNetworkLayers + " li input[data-layername='ROW']").is(':checked') && layers.includes('ROW') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='ROW'] .chknetwork-L:checked").length == 0) {
            //    var minZoom = $('.mainlyr li[data-mapabbr="ROW"] #MinMaxZoom').attr('data-min-zoom');
            //    var maxZoom = $('.mainlyr li[data-mapabbr="ROW"] #MinMaxZoom').attr('data-max-zoom');
            //    var currentZoom = app.map.getZoom();
            //    // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
            //    layers.push('ROW');
            //    layers.push('ROWL');
            //    layers.push('PIT');
            //    // }
            //}

        } else {
            //$.each($(app.DE.ulNetworkLayers + ' li input[data-is-networktype-required="False"][data-is-network-entity="False"]'), function () {
            //    var mapAbbr = $(this).data().mapabbr;
            //    if ($(this).is(':checked') && layers.includes(mapAbbr) == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='" + mapAbbr + "'] .chknetwork-L:checked").length > 0) {
            //        layers.push(mapAbbr);
            //    }
            //})
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='Area']").is(':checked') && layers.includes('ARA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='ARA'] .chknetwork-L:checked").length > 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="Area"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="Area"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('ARA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='SurveyArea']").is(':checked') && layers.includes('SVA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='SVA'] .chknetwork-L:checked").length > 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="SurveyArea"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="SurveyArea"] #MinMaxZoom').attr('data-max-zoom');
                // var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('SVA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='SubArea']").is(':checked') && layers.includes('SBA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='SBA'] .chknetwork-L:checked").length > 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="SubArea"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="SubArea"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('SBA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='Customer']").is(':checked') && layers.includes('CUS') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='CUS'] .chknetwork-L:checked").length > 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="SubArea"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="SubArea"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('CUS');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='ProjectArea']").is(':checked') && layers.includes('PRA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='PRA'] .chknetwork-L:checked").length > 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="ProjectArea"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="ProjectArea"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('PRA');
                // }
            }

            if ($(app.DE.ulNetworkLayers + " li input[data-layername='DSA']").is(':checked') && layers.includes('DSA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='DSA'] .chknetwork-L:checked").length > 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="DSA"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="DSA"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('DSA');
                // }
            }
            if ($(app.DE.ulNetworkLayers + " li input[data-layername='CSA']").is(':checked') && layers.includes('CSA') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='CSA'] .chknetwork-L:checked").length > 0) {
                var minZoom = $('.mainlyr li[data-mapabbr="CSA"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="CSA"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                layers.push('CSA');
                // }
            }
            //if ($(app.DE.ulNetworkLayers + " li input[data-layername='ROW']").is(':checked') && layers.includes('ROW') == false && $(app.DE.ulNetworkLayers + " li[data-mapabbr='ROW'] .chknetwork-L:checked").length > 0) {
            //    var minZoom = $('.mainlyr li[data-mapabbr="ROW"] #MinMaxZoom').attr('data-min-zoom');
            //    var maxZoom = $('.mainlyr li[data-mapabbr="ROW"] #MinMaxZoom').attr('data-max-zoom');
            //    var currentZoom = app.map.getZoom();
            //    // if (currentZoom >= minZoom && currentZoom <= maxZoom) {
            //    layers.push('ROW');
            //    layers.push('ROWL');
            //    layers.push('PIT');
            //    // }
            //}
        }
        return layers;
    }
    this.getActiveNetworkLayersNew = function (layerType) {
        var lyrs = [];
         
        $.each(app.geomTypes, function (i, itm) {
            $.each($(app.DE.ulNetworkLayers + " li[data-geomtype='" + itm + "'] .mainLyr:checked").sort(function (a, b) { return parseInt($(a).attr('data-maplayerseq')) - parseInt($(b).attr('data-maplayerseq')) }), function () {
                var minZoom = $('.mainlyr li[data-mapabbr="' + $(this).attr('data-mapabbr') + '"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="' + $(this).attr('data-mapabbr') + '"] #MinMaxZoom').attr('data-max-zoom');
                var currentZoom = app.map.getZoom();
                // if (layerType == 'P') {                   
                // if ($(this).parent().find('.chknetwork-P:checked').length > 0 && $(this).parent().find('.chknetwork-L:checked').length == 0) {                       
                //     if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                //          lyrs.push($(this).attr('data-mapabbr'));
                //      }
                //      var layers = { Name: $(this).attr('data-mapabbr'), minZoom: minZoom, maxZoom: maxZoom, networkStatus: layerType, isWithLabel: false, isNetworkLayer: true };
                //        app.layerDetails.push(layers);
                //     }
                // }
                //  else {

                if ($(this).parent().find('.chknetwork-' + layerType + ':checked').length > 0 && ($(this).attr('data-isnetworktyperequired') == "True" && $(this).parent().find('.chknetwork-L:checked').length == 0)) {
                    // if ($(this).parent().find('.chknetwork-' + layerType + ':checked').length>0 && ($(this).attr('data-isnetworktyperequired') == "True")) {                  
                    if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                        lyrs.push($(this).attr('data-mapabbr'));
                    }
                    var layers = { Name: $(this).attr('data-mapabbr'), minZoom: minZoom, maxZoom: maxZoom, networkStatus: layerType, isWithLabel: false, isNetworkLayer: true };
                    app.layerDetails.push(layers);
                }
                if ($(app.DE.ulNetworkLayers + " li input[data-layername='ROW']").is(':checked') && $(app.DE.ulNetworkLayers + " li[data-mapabbr='ROW'] .chknetwork-L:checked").length == 0) {
                    var minZoom = $('.mainlyr li[data-mapabbr="ROW"] #MinMaxZoom').attr('data-min-zoom');
                    var maxZoom = $('.mainlyr li[data-mapabbr="ROW"] #MinMaxZoom').attr('data-max-zoom');
                    var currentZoom = app.map.getZoom();

                    lyrs.push('ROW');
                    lyrs.push('ROWL');
                    lyrs.push('PIT');
                    var layers = { Name: $(this).attr('data-mapabbr'), minZoom: minZoom, maxZoom: maxZoom, networkStatus: '', isWithLabel: true, isNetworkLayer: true };
                    app.layerDetails.push(layers);
                }

                // }

            });

        });
        //lyrs = app.addPolygonLayer(lyrs, false);

        return lyrs;
    }
    this.getActiveNetworkLayersWithLabel = function (layerType) {
        var lyrs = [];
         
        $.each(app.geomTypes, function (i, itm) {
            $.each($(app.DE.ulNetworkLayers + " li[data-geomtype='" + itm + "'] .mainLyr:checked").sort(function (a, b) { return parseInt($(a).attr('data-maplayerseq')) - parseInt($(b).attr('data-maplayerseq')) }), function () {
                var minZoom = $('.mainlyr li[data-mapabbr="' + $(this).attr('data-mapabbr') + '"] #MinMaxZoom').attr('data-min-zoom');
                var maxZoom = $('.mainlyr li[data-mapabbr="' + $(this).attr('data-mapabbr') + '"] #MinMaxZoom').attr('data-max-zoom');
                // if (layerType == 'P') {
                // if ($(this).parent().find('.chknetwork-P:checked').length > 0 && $(this).parent().find('.chknetwork-L:checked').length > 0) {
                //     var currentZoom = app.map.getZoom();
                //     if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                //          lyrs.push($(this).attr('data-mapabbr'));
                //      }

                //       var layers = { Name: $(this).attr('data-mapabbr'), minZoom: minZoom, maxZoom: maxZoom, networkStatus: layerType, isWithLabel: true, isNetworkLayer: true };
                //       app.layerDetails.push(layers);
                //    }
                // }
                //else {

                if ($(this).parent().find('.chknetwork-' + layerType + ':checked').length > 0 && ($(this).attr('data-isnetworktyperequired') == "True" && $(this).parent().find('.chknetwork-L:checked').length > 0)) {
                    var currentZoom = app.map.getZoom();
                    if (currentZoom >= minZoom && currentZoom <= maxZoom) {
                        lyrs.push($(this).attr('data-mapabbr'));
                    }

                    var layers = { Name: $(this).attr('data-mapabbr'), minZoom: minZoom, maxZoom: maxZoom, networkStatus: layerType, isWithLabel: true, isNetworkLayer: true };
                    app.layerDetails.push(layers);
                }
                if ($(app.DE.ulNetworkLayers + " li input[data-layername='ROW']").is(':checked') && $(app.DE.ulNetworkLayers + " li[data-mapabbr='ROW'] .chknetwork-L:checked").length > 0) {
                    var minZoom = $('.mainlyr li[data-mapabbr="ROW"] #MinMaxZoom').attr('data-min-zoom');
                    var maxZoom = $('.mainlyr li[data-mapabbr="ROW"] #MinMaxZoom').attr('data-max-zoom');
                    var currentZoom = app.map.getZoom();

                    lyrs.push('ROW');
                    lyrs.push('ROWL');
                    lyrs.push('PIT');
                    var layers = { Name: $(this).attr('data-mapabbr'), minZoom: minZoom, maxZoom: maxZoom, networkStatus: '', isWithLabel: true, isNetworkLayer: true };
                    app.layerDetails.push(layers);
                }
                //}
            });
            //lyrs = app.addPolygonLayer(lyrs, true)
        })
        return lyrs;

    }
    this.getActiveNetworkLayers = function (lyrType) {
        //lyrType= PLANNED,AS BUILD, DORMAENT
        var lyrs = []
        $.each($(app.DE.ulNetworkLayers + ' li input[type="checkbox"]:checked'), function () {
            var mapAbbr = $(this).attr('data-mapAbbr');
            if (mapAbbr != undefined && mapAbbr.toUpperCase() == "BLD") {
                lyrs.push(mapAbbr);
                lyrs.push("BLDP"); // TO SHOW POLYGON OF APPROVED BUILDING
                lyrs.push("BLDC"); // TO SHOW ICON AT CENTROID POINT OF APPROVED BUILDING
            }
            else {
                lyrs.push(mapAbbr); //data-mapAbbr contain short code for Layer which is further used in map file to render layers
            }
        });
        return lyrs;
    }
    this.getEntityNetworksType = function (lyr) {
        //lyrType= PLANNED,AS BUILD, DORMAENT
        var Networkslyrs = []
        var ntType = '';
        $.each($(app.DE.ulNetworkLayers + ' li input[type=checkbox]:checked').filter("[data-mapAbbr='" + lyr + "']"), function () {
            //var mapAbbr = $(this).attr('data-mapAbbr');
            var data_networkType = $(this).attr('data-networktype');
            if (data_networkType != undefined) {
                ntType = "'" + data_networkType + "'";
                Networkslyrs.push(ntType);
            }

        });
        return Networkslyrs;
    }
    this.handlePrClick = function (cb) {
        var Pid = $(cb).attr("data-all");

        if (Pid != 'L') {

            if (cb.checked) {
                $(".layers .network .checkbox-custom").not(":disabled").prop("checked", true);
                $(".checkbox-customgrp").not(":disabled").prop("checked", true);
                $(".chknetwork-" + Pid).not(":disabled").prop('checked', true);

            }
            else {
                $(".chknetwork-" + Pid).not(":disabled").prop('checked', false);

            }
        }
        else {

            $(".chknetwork-" + Pid).not(":disabled").prop('checked', cb.checked);
        }


        if (Pid != 'L' && $('.checkbox-custom1:checkbox:unchecked').filter("[data-all!='L']").length == $('.checkbox-custom1').filter("[data-all!='L']").length) {
            $(".checkbox-customgrp").not(":disabled").prop("checked", false);
            $(($('.layers .network li input[type=checkbox]:checked').filter('.checkbox-custom'))).not(":disabled").attr('checked', false);
            $('#checkAll').prop('checked', false);
            $(".chknetwork-L,#chklabelAll").not(":disabled").prop("checked", false);

        } else if (Pid == 'L' && $('.checkbox-custom1:checkbox:unchecked').filter("[data-all!='L']").length == $('.checkbox-custom1').filter("[data-all!='L']").length) {
            if (cb.checked) {
                $(".layers .network .checkbox-custom").not(":disabled").prop("checked", true);
                $(".checkbox-customgrp").not(":disabled").prop("checked", true);
                $(".chknetwork-" + Pid).not(":disabled").prop('checked', true);
                if ($('.checkbox-custom1:checkbox:unchecked').filter("[data-all!='L']").length == $('.checkbox-custom1').filter("[data-all!='L']").length) {
                    $(".chknetwork-P,#chkPlannedAll,#checkAll").not(":disabled").prop('checked', true);
                }

            }
        }

        if ($('.checkbox-custom1:checkbox:checked').filter("[data-all!='L']").length > 0) {
            $('#checkAll').prop('checked', true);
        }



        //$(app.DE.chkPlannedAll).change(function () {          

        //   // $('.mainlyr .checkbox-custom').prop('checked', this.checked);
        //});
    }
    this.handleChClick = function (cb) {
        // alert(cb.checked);
        var childId = $(cb).attr("data-layerid");
        var childnetworkClass = $(cb).attr("class").split(' ')[0];
        var childnetworkType = childnetworkClass.split('-')[1];
        var pAllId = app.getParentallid(childnetworkType);
        var parId = $(cb).attr("data-layerid").split('_')[1];
        var layerGroup = $(cb).attr("data-layergroup");
        var disCheckboxLength = $('input[type=checkbox]').filter('.' + childnetworkClass + '').filter('[data-ntp=False]').length;


        if (childnetworkType != 'L') {

            if (cb.checked) {
                if ($('input[type=checkbox]:checked').filter('[data-layerid=' + childId + ']').filter("[data-networktype!='L']").length > 0) {
                    $('[data-layerid=' + parId + ']').prop('checked', true);

                    if ($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').filter('[data-ntp=True]').length == ($('.' + childnetworkClass + '').length - disCheckboxLength)) {
                        $('#' + pAllId).prop('checked', true);
                    }
                }


            }
            else {
                if ($('input[type=checkbox]:checked').filter('[data-layerid=' + childId + ']').filter("[data-networktype!='L']").length == 0) {
                    $('[data-layerid=' + parId + ']').prop('checked', false);
                }
                if (($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').length
                    - disCheckboxLength) < ($('.' + childnetworkClass + '').length - disCheckboxLength)) {
                    $('#' + pAllId).prop('checked', false);
                }


            }
        }
        else {

            if (cb.checked) {
                if ($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').filter('[data-ntp=True]').length == ($('.' + childnetworkClass + '').length - disCheckboxLength)) {
                    $('#' + pAllId).prop('checked', true);
                }
            }
            else {
                if (($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').length
                    - disCheckboxLength) < ($('.' + childnetworkClass + '').length - disCheckboxLength)) {
                    $('#' + pAllId).prop('checked', false);
                }
            }

        }
        //// Group layer checked unchecked
        if ($('.layers .network li input[type=checkbox]:checked').filter('.checkbox-custom').filter('[data-layergroup=' + layerGroup + ']').length
            == $('.layers .network li input[type=checkbox]').filter('.checkbox-custom').filter('[data-layergroup=' + layerGroup + ']').length) {
            $('#' + layerGroup).not(":disabled").prop('checked', true);

        }
        else {
            $('#' + layerGroup).not(":disabled").prop('checked', false);
        }

        ///// Parent node checked/Unchecked

        if ($('.checkbox-customgrp:checkbox:checked').length == $('.checkbox-customgrp:checkbox').length) {
            $('#checkAll').not(":disabled").prop('checked', true);
        }
        else {
            $('#checkAll').not(":disabled").prop('checked', false);
        }

    }
    this.getParentallid = function (obj) {
        if (obj == 'P')
            return "chkPlannedAll";
        else if (obj == 'A')
            return "chkAsBuiltAll";
        else if (obj == 'D')
            return "chkDormentAll";
        else if (obj == 'L')
            return "chklabelAll";
    }
    this.getActiveRegionLayers = function () {
        var lyrs = []
        $.each($(app.DE.ulRegionLayers + '>li>input[type="checkbox"]:checked'), function () {
            lyrs.push($(this).attr('data-regionId'));
        });
        return lyrs;
    }
    this.getActiveProvinceLayers = function () {
        var lyrs = []
        $.each($(app.DE.ulProvinceLayers + '>li>input[type="checkbox"]:checked'), function () {
            lyrs.push($(this).attr('data-provinceId'));
        });
        return lyrs;
    }
    this.SetProjectSpecFilters = function () {

        app.projectcodevalue = $('#ddl_ProjectSpeciCode').val();
        app.planningcodevalue = $('#ddl_PlanningSpeciCode').val();
        app.workordercodevalue = $('#ddl_WorkorderSpeciCode').val();
        app.purposecodevalue = $('#ddl_PurposeSpeciCode').val();

        if (app.projectcodevalue != '0' && app.projectcodevalue != undefined) {
            app.filterprojectvalue = "([project_id] =" + app.projectcodevalue + ")";
        }

        if (app.planningcodevalue != '0' && app.planningcodevalue != undefined) {
            app.filterprojectvalue = "([planning_id] =" + app.planningcodevalue + ")";
        }
        if (app.workordercodevalue != '0' && app.workordercodevalue != undefined) {
            app.filterprojectvalue = " ([workorder_id] =" + app.workordercodevalue + ")";
        }
        if (app.purposecodevalue != '0' && app.purposecodevalue != undefined) {
            app.filterprojectvalue = "([purpose_id] =" + app.purposecodevalue + ")";
        }
        if (app.filterprojectvalue == "") {
            app.filterprojectvalue = "1 = 1";
        }
    }
    this.SetFaultFilters = function () {
        app.filterFaultvalue = "";
        app.fault_status = $('#ddlFilterFaultStatus').val();

        if (app.fault_status != '0' && app.fault_status != undefined) {
            app.filterFaultvalue += " ([fault_status] ='" + app.fault_status + "')";
        }
        if (app.filterFaultvalue == "") {
            app.filterFaultvalue = "1 = 1";
        }
    }
    this.SetOwnershipFilters = function () {
        app.filterOwnershipvalue = "";
        app.ownership = $("select#ddlFilterOwnership").val();
        if (app.ownership != '0' && app.ownership != undefined) {
            app.filterOwnershipvalue += " ([ownership] in (" + '\'' + app.ownership.join('\',\'') + '\'' + "))";
        }
        if (app.filterOwnershipvalue == "") {
            app.filterOwnershipvalue = "1 = 1";
        }
    }

    this.SetCableFilters = function () {
        app.filtercablevalue = "";
        app.cable_type = $('#ddlFilterCableType').val();
        app.cable_category = $('#ddlFilterCableCategory').val();

        if (app.cable_type != '0' && app.cable_type != undefined) {
            app.filtercablevalue += " ([cable_type] ='" + app.cable_type + "')";
        }
        if (app.cable_category != '0' && app.cable_category != undefined) {
            app.filtercablevalue += ((app.cable_type != '0' && app.cable_type != undefined) ? " and " : "") + " ([cable_category] ='" + app.cable_category + "')";
        }
        if (app.filtercablevalue == "") {
            app.filtercablevalue = "1 = 1";
        }


    }

    this.SetRegionProvinceFilters = function () {
        app.RegionFilter = '';
        app.ProvinceFilter = '';
        //get selected region provinces
        app.ActiveRegionlayers = app.getActiveRegionLayers();
        app.ActiveProvincelayers = app.getActiveProvinceLayers();
        //Prepare filters string
        app.RegionFilter = " [region_id] in (" + app.ActiveRegionlayers.join(",") + ")";
        app.ProvinceFilter = "([region_id] in (" + app.ActiveRegionlayers.join(",") + ") AND [province_id] in (" + app.ActiveProvincelayers.join(",") + "))";
    }

    this.SetBuildingRFSFilters = function () {

        app.RegionFilter = " [region_id] in (" + app.ActiveRegionlayers.join(",") + ")";
        var checkedItems = [];
        $('#chkBldRfs input:checked').each(function () {
            checkedItems.push("'" + $(this).attr('data-mapabbr') + "'");
        });

        if (checkedItems.length > 0) {
            app.ActiveBuildingRFSFilter = checkedItems;
            app.buildingRFSFilter = " [rfs_status] in (" + app.ActiveBuildingRFSFilter.join(",") + ")";
        }
        else {
            app.buildingRFSFilter = "1 = 1";
        }


        //if (app.buildingRFSFilter != '0' && app.buildingRFSFilter != undefined) {
        //    app.buildingRFSFilter = "(rfs_status ='" + app.buildingRFSFilter + "')";
        //}
        //else {
        //    app.buildingRFSFilter = "1 = 1";
        //}

    }
    this.getLatLongFromPT = function (dragPT) {
        return app.overlay.getProjection().fromContainerPixelToLatLng(dragPT);
    }

    this.clearTempNewEntity = function () {

        if (app.mapListners["TempLibItm"]) {
            app.mapListners["TempLibItm"].setMap(null);
            app.mapListners["TempLibItm"] = null;
        }
        if (app.mapListners["mapListners"])
            app.mapListners["mapListners"] = null;
        if (app.entityOBJ.length > 0) {
            $.each(app.entityOBJ, function (key, value) { value.setMap(null); });
            app.entityOBJ = [];
            app.map.setOptions({ draggableCursor: '' });
            app.removeEventListnrs('click');
        }



    }


    //this.clearDraggableLibrary = function () { //remove draggable library element
    //    app.clearTempNewEntity();
    //    app.ClearMapAddressTool();
    //    app.ClearJobPackAddressTool();
    //    app.clearBulkEntityInfo();
    //    //app.clearTPRelatedObjects();
    //    //remove draggable element while call through info tool
    //    app.clearEditObj();
    //}

    this.handleLayerSwitch = function () {
        $(app.DE.divLayers).slideToggle("slow");
        $(app.DE.btnlayerSwitch).toggleClass('lyrPlus').toggleClass('lyrminus');
    }

    this.filterInfoElements = function (filterType) {
         
        $('#txtInfoSrch').val('');
        var drpVal = $('#ddNetLayerType').val();
        if (filterType == "DisplayLayer") {
            $("#infoTable tr").hide();
            var checkedlyrs = [];
            $.each($(app.DE.ulNetworkLayers + " li .mainLyr:checked"), function () {
                checkedlyrs.push($(this).attr('data-layername').toUpperCase());
            });
            if (drpVal.toLowerCase() != 'all') {
                $.each($("#infoTable tr"), function () {

                    if (checkedlyrs.includes($(this).children().first().text().toUpperCase())
                        && $(this).children().last().children().val().toLowerCase().indexOf(drpVal.toLowerCase()) > -1
                    ) {
                        $(this).show();
                    }
                });
            } else {
                $.each($("#infoTable tr"), function () {
                    if (checkedlyrs.includes($(this).children().first().text().toUpperCase())) {
                        $(this).show();
                    }
                });
            }
        } else {
            if (drpVal.toLowerCase() != 'all') {
                $("#infoTable tr").hide();
                $.each($("#infoTable tr"), function () {
                    if ($(this).children().last().children().val().toLowerCase().indexOf(drpVal.toLowerCase()) > -1
                    ) {
                        $(this).show();
                    }
                });
            } else {
                $('#infoTable tr').show();
            }

        }
    }

    this.changeNetworkStage = function () {
         
        var drpVal = $('#ddNetLayerType').val();
        $('#txtInfoSrch').val('');
        if (drpVal.toLowerCase() != 'all') {
            $("#infoTable tr").hide();
            if ($('#rdbDisplayedLayer').is(':checked')) {
                var checkedlyrs = [];
                $.each($(app.DE.ulNetworkLayers + " li .mainLyr:checked"), function () {
                    checkedlyrs.push($(this).attr('data-layername').toUpperCase());
                });
                $.each($("#infoTable tr"), function () {
                    if (checkedlyrs.includes($(this).children().first().text().toUpperCase()) && $(this).children().last().children().val().toLowerCase().indexOf(drpVal.toLowerCase()) > -1) {
                        $(this).show();
                    }
                });
            } else {
                $('#infoTable tr').each(function () {
                    if ($('td input[type=hidden]', $(this)).val().toLowerCase().indexOf(drpVal.toLowerCase()) > -1) {
                        $(this).show();
                    } else {
                        $(this).hide();
                    }

                });
            }
        } else {
            $("#infoTable tr").hide();
            if ($('#rdbDisplayedLayer').is(':checked')) {
                var checkedlyrs = [];
                $.each($(app.DE.ulNetworkLayers + " li .mainLyr:checked"), function () {
                    checkedlyrs.push($(this).attr('data-layername').toUpperCase());
                });
                $.each($("#infoTable tr"), function () {
                    if (checkedlyrs.includes($(this).children().first().text().toUpperCase())) {
                        $(this).show();
                    }
                });
            } else {
                $("#infoTable tr").show();
            }
        }
    }
    this.GetProjecSpeciFilter = function () {
         
        if ($(".mainlyr input[type=checkbox]:checked").length > 0) {
            popup.LoadModalDialog(app.ParentModel, 'Library/ViewProjectSpecificFilter', { network_status: app.networkstatus, project_id: app.projectcodevalue, planning_id: app.planningcodevalue, workorder_id: app.workordercodevalue, purpose_id: app.purposecodevalue, cable_type: app.cable_type, cable_category: app.cable_category }, 'Advance Filters', 'modal-lg');
        }
        else {
            alert('Please select Network Layers!');
        }
    }
    $('.mainlyr .checkbox-custom').change(function () {
         
        if (this.checked) {
            var chkid = this.id;
            var id = $('#' + chkid).attr("data-layerid");
            var groupType = $('#' + chkid).attr("data-layergroup");
            $('input:checkbox#chk_netP_' + id).not(":disabled").prop('checked', this.checked);
            $('input:checkbox#chk_netA_' + id).not(":disabled").prop('checked', this.checked);
            $('input:checkbox#chk_netD_' + id).not(":disabled").prop('checked', this.checked);

            if ($('input:checkbox#chk_netP_' + id).is(':enabled') || $('input:checkbox#chk_netA_' + id).is(':enabled')
                || $('input:checkbox#chk_netD_' + id).is(':enabled')) {
                //$('input:checkbox#chk_netL_' + id).not(":disabled").prop('checked', this.checked);
            }

            if ($('.layers .network li input[type=checkbox]:checked').filter('.checkbox-custom').length
                < $('.layers .network li input[type=checkbox]').filter('.checkbox-custom').length) {
                $(app.DE.chkPlannedAll).not(":disabled").prop('checked', false);
                $(app.DE.chkAsBuiltAll).not(":disabled").prop('checked', false);
                $(app.DE.chkDormentAll).not(":disabled").prop('checked', false);
                $(app.DE.chklabelAll).not(":disabled").prop('checked', false);

            }


            //// group layer selection checked unchecked
            if ($('.layers .network li input[type=checkbox]:checked').filter('.checkbox-custom').filter('[data-layergroup=' + groupType + ']').length
                == $('.layers .network li input[type=checkbox]').filter('.checkbox-custom').filter('[data-layergroup=' + groupType + ']').length) {

                $('input:checkbox#' + groupType).prop('checked', true);

            }
            else {
                $('input:checkbox#' + groupType).prop('checked', false);
            }



        }
        else {
            var chkid = this.id;
            var id = $('#' + chkid).attr("data-layerid");
            var lyrName = $('#' + chkid).attr("data-mapabbr");
            var groupType = $('#' + chkid).attr("data-layergroup");
            $('input:checkbox#chk_netP_' + id).prop('checked', false);
            $('input:checkbox#chk_netA_' + id).prop('checked', false);
            $('input:checkbox#chk_netD_' + id).prop('checked', false);
            $('input:checkbox#chk_netL_' + id).prop('checked', false);

            if ($('.layers .network li input[type=checkbox]:checked').filter('.checkbox-custom').length
                < $('.layers .network li input[type=checkbox]').filter('.checkbox-custom').length) {
                $(app.DE.chkPlannedAll).prop('checked', false);
                $(app.DE.chkAsBuiltAll).prop('checked', false);
                $(app.DE.chkDormentAll).prop('checked', false);
                $(app.DE.chklabelAll).prop('checked', false);
            }

            //// group layer selection checked unchecked

            if ($('.layers .network li input[type=checkbox]:checked').filter('.checkbox-custom').filter('[data-layergroup=' + groupType + ']').length
                == $('.layers .network li input[type=checkbox]').filter('.checkbox-custom').filter('[data-layergroup=' + groupType + ']').length) {

                $('input:checkbox#' + groupType).prop('checked', true);

            }
            else {
                $('input:checkbox#' + groupType).prop('checked', false);
            }


            //var remLyrItem  app.DE.layerManager.includes(lyrName);

            // app.layerManager.pop(lyrName);


        }


        /// Parent node checked if all group layer checkbox checked

        if ($('.checkbox-customgrp:checkbox:checked').length
            == $('.checkbox-customgrp:checkbox').length) {
            $('#checkAll').prop('checked', true);
        }
        else {
            $('#checkAll').prop('checked', false);
        }



        /////
        if ($('.checkbox-custom2:checked').filter('[data-networktype=P]').length
            == $('.checkbox-custom2').filter('[data-networktype=P]').length) {
            $('#chkPlannedAll').prop('checked', true);
        }
        if ($('.checkbox-custom2:checked').filter('[data-networktype=A]').length
            == $('.checkbox-custom2').filter('[data-networktype=A]').length) {
            $('#chkAsBuiltAll').prop('checked', true);
        }
        if ($('.checkbox-custom2:checked').filter('[data-networktype=D]').length
            == $('.checkbox-custom2').filter('[data-networktype=D]').length) {
            $('#chkDormentAll').prop('checked', true);
        }
        if ($('.checkbox-custom2:checked').filter('[data-networktype=L]').length
            == $('.checkbox-custom2').filter('[data-networktype=L]').length) {
            $('#chklabelAll').prop('checked', true);
        }


    });

    $("input[name='latLngFrmt']").change(function () {
        app.LAT_LNG_FORMAT = $(this).val();
    });

    // for grop checkbox checked/unchecked
    $('.mainlyr .checkbox-customgrp').change(function () {
         
        if (this.checked) {
            $("[data-layergroup='" + this.id + "']").filter("[data-networktype!='L']").not(":disabled").prop('checked', this.checked);


        }
        else {
            $("[data-layergroup='" + this.id + "']").filter("[data-networktype!='L']").not(":disabled").prop('checked', this.checked);


        }

        /// check all group checkd or not

        if ($('.checkbox-customgrp:checkbox:checked').length == $('.checkbox-customgrp:checkbox').length) {
            $('#checkAll').prop('checked', true);
        }
        else {
            $('#checkAll').prop('checked', false);
        }

        ////

        if ($("#checkAll").prop('checked')) {
            if (app.ckhNetworksParentsALL('chknetwork-P')) {
                $('#chkPlannedAll').prop('checked', true);
            }
            if (app.ckhNetworksParentsALL('chknetwork-A')) {
                $('#chkAsBuiltAll').prop('checked', true);
            }
            if (app.ckhNetworksParentsALL('chknetwork-D')) {
                $('#chkDormentAll').prop('checked', true);
            }
        }
        else {
            if (app.ckhNetworksParentsALL('chknetwork-P')) {
                $('#chkPlannedAll').prop('checked', true);
            }
            else {
                $('#chkPlannedAll').prop('checked', false);
            }
            if (app.ckhNetworksParentsALL('chknetwork-A')) {
                $('#chkAsBuiltAll').prop('checked', true);
            }
            else {
                $('#chkAsBuiltAll').prop('checked', false);
            }
            if (app.ckhNetworksParentsALL('chknetwork-D')) {
                $('#chkDormentAll').prop('checked', true);
            }
            else {
                $('#chkDormentAll').prop('checked', false);
            }
        }


    });


    this.ckhNetworksParentsALL = function (childnetworkClass) {

        var disCheckboxLength = $('input[type=checkbox]').filter('.' + childnetworkClass + '').filter('[data-ntp=False]').length;
        if ($('input[type=checkbox]:checked').filter('.' + childnetworkClass + '').filter('[data-ntp=True]').length == ($('.' + childnetworkClass + '').length - disCheckboxLength)) {
            return true;
        }
        else {
            return false;
        }
    }
    $(app.DE.ulRegionLayers + ' li input[type="checkbox"]').on('change', function () {
        $(this).parent().find('input[type="checkbox"]').not(":disabled").prop("checked", this.checked);
    });
    $(app.DE.ulProvinceLayers + ' li input[type="checkbox"]').on('change', function () {

        if ($(this).parents().eq(1).find('input[id^="chk_pLyr_"]:checked').length > 0) {
            $(this).parents().eq(2).find('input[id^="chk_rLyr_"]').not(":disabled").prop("checked", true);
        }
        else {
            $(this).parents().eq(2).find('input[id^="chk_rLyr_"]').not(":disabled").prop("checked", false);
        }
    });
    //sapna
    //#end regin

    /*this.feasibilityDetailsInformation = function () {
        for (var i = 0; i < app.feasiblePaths.length; i++) {
            var feasibilityInfo = {};
    
            var subPath = app.feasiblePaths[i];
            //subPath
    
            if (subPath.length) {
                for(path of subPath)
                {
                    // var objfeasibilityGeometry = {};
                    if (!path.isSelected)
                        continue;
                    var geom = '';
                    for(ll of path.getPath().getArray()) {
                        geom += ll.lng() + ' ' + ll.lat() + ', ';
                    }
                    switch (path.type) {
                        case app.cable_types.INSIDE_P:
                            feasibilityInfo.inside_P_Distance = path.distance;
                            break;
                        case app.cable_types.INSIDE_A:
                            feasibilityInfo.inside_A_Distance = path.distance;
                            break;
                        case app.cable_types.INSIDE:
                            feasibilityInfo.insideDistance = path.distance;
                            break;
                        case app.cable_types.OUTSIDE:
                            feasibilityInfo.outsideDistance = path.distance;
                            break;
                        case app.cable_types.LMC_A_END:
                            feasibilityInfo.lmc_A_Distance = path.distance;
                            break;
                        case app.cable_types.LMC_B_END:
                            feasibilityInfo.lmc_B_Distance = path.distance;
                            break;
                    }
                    feasibilityInfo.totalDistance = path.totalDistance;
                    //objfeasibilityGeometry.cable_geometry = geom;
                    //objfeasibilityGeometry.path_type = path.type;
                    //objfeasibilityGeometry.cable_length = path.distance;
    
                    //app.feasibilityGeometry.push(objfeasibilityGeometry);
                }
            }
            app.feasibilityDetailsInfo.push(
                                                       {
                                                           "feasibilityName": $("#feasibilityID").val(),
                                                           "totalLength": feasibilityInfo.totalDistance,
                                                           "ExistingLength_P": feasibilityInfo.inside_P_Distance,
                                                           "ExistingLength_A": feasibilityInfo.inside_A_Distance,
                                                           "NewOutsideLength": feasibilityInfo.outsideDistance,
                                                           "lmc_A_End_Path": feasibilityInfo.lmc_A_Distance,
                                                           "lmc_B_End_Path": feasibilityInfo.lmc_B_Distance,
                                                           "geom": geom
                                                       });
    
    
        }
    }*/
    this.getFeasibilityDetails = function () {
        var feasibilityDetailsInfo = {};
        feasibilityDetailsInfo.objFeasibilityModel =
        {
            "feasibility_id": $("#hdn_feasibility_id").val(),
            "feasibility_name": $("#feasibilityID").val(),
            "customer_name": $("#customerName").val(),
            "customer_id": $("#customerID").val(),
            "start_point_name": $("#startPointName").val(),
            "end_point_name": $("#endPointName").val(),
            "start_lat_lng": app.DMStoDD($("#startLng").val()) + ',' + app.DMStoDD($("#startLat").val()),
            "end_lat_lng": app.DMStoDD($("#endLng").val()) + ',' + app.DMStoDD($("#endLat").val()),
            "cores_required": $("#cores").val(),
            "cable_type_id": parseInt($("#cableType option:selected").val()),
            "buffer_radius_a": parseFloat($("#startBuffer").val()),
            "buffer_radius_b": parseFloat($("#endBuffer").val()),
            "core_level_result": app.is_core_feasibile ? 'Feasible' : 'Not Feasible',
            "feasibility_result": app.feasibilityStatus
        };
        feasibilityDetailsInfo.lstFeasibilityDetails = app.feasibilityDetailsInfo;
        feasibilityDetailsInfo.lstFeasibilityGeometry = app.feasibilityGeometry;
        /*app.feasibilityGeometry.length = app.feasiblePaths.length;
        for (var i = 0; i < app.feasiblePaths.length; i++) {
            for (subPaths of app.feasiblePaths[i]) {
                subPaths.setMap(null);
            }
        }
        */

        popup.LoadModalDialog('ModalPopUp_Save', 'feasibilitydetails/Index', { 'feasibilityDetailsInfo': feasibilityDetailsInfo }, 'Feasibility Details', 'modal-xxl');
    }
    this.fillFeasibilityDetails = function (totalLength, ExistingLength_A, ExistingLength_P, NewOutside_A_Length, NewOutside_B_Length, lmc_A_End_Path, lmc_B_End_Path, buffer_radius_a, buffer_radius_b, selected_id, bCalcGeom) {
        // $($('.libTabs ul li a')[1]).trigger('click');
        $("#feasibilitybtnDv").css("display", "block");
        $("#totalLength").val(totalLength);
        $("#ExistingLength_P").val(ExistingLength_P);
        $("#ExistingLength_A").val(ExistingLength_A);
        $("#NewOutside_A_Length").val(NewOutside_A_Length);
        $("#NewOutside_B_Length").val(NewOutside_B_Length);
        $("#lmc_A_End_Path").val(lmc_A_End_Path);
        $("#lmc_B_End_Path").val(lmc_B_End_Path);
        $("#buffer_radius_a").val(buffer_radius_a);
        $("#buffer_radius_b").val(buffer_radius_b);

        $("#cableType_id").val(parseInt($("#cableType option:selected").val()));
        if (bCalcGeom) {
            // app.getCableGeometry(selected_id);

        }
        else {
            $.ajax({
                type: "POST",
                url: "feasibilitydetails/getfeasibilityHistoryDetails",
                data: JSON.stringify({ 'selectedFeasibilityID': selected_id }),
                contentType: "application/json; charset=utf-8",
                dataType: JSON,
                success: function (response) {
                    //alert("Hello:");
                    response = JSON.parse(response.responseText);
                    $("#feasibility_id").val(response.feasibility_name);
                    $("#customer_name").val(response.customer_name);
                    $("#customer_id").val(response.customer_id);
                    $("#cores_free").val(response.cores_required);
                    $("#start_lat").val(response.start_lat_lng);
                    $("#end_lat").val(response.end_lat_lng);
                    // $("#feasibilitydetailsDv").html(response);
                },
                failure: function (response) {

                    alert(response.responseText);
                },
                error: function (response) {
                    response = JSON.parse(response.responseText);
                    $("#feasibility_id").val(response.feasibility_name);
                    $("#customer_name").val(response.customer_name);
                    $("#customer_id").val(response.customer_id);
                    $("#cores_free").val(response.cores_required);
                    $("#start_lat").val(response.start_lat_lng);
                    $("#end_lat").val(response.end_lat_lng);
                    // alert(response.responseText);
                }
            });
        }
    }
    this.Savefeasibility = function () {
        var feasibilityDetailsInfo = {};

        feasibilityDetailsInfo.objFeasibilityModel =
        {
            "feasibility_id": $("#hdn_feasibility_id").val(),
            "feasibility_name": $("#feasibilityID").val(),
            "customer_name": $("#customerName").val(),
            "customer_id": $("#customerID").val(),
            "start_point_name": $("#startPointName").val(),
            "end_point_name": $("#endPointName").val(),
            "start_lat_lng": app.DMStoDD($("#startLng").val()) + ',' + app.DMStoDD($("#startLat").val()),
            "end_lat_lng": app.DMStoDD($("#endLng").val()) + ',' + app.DMStoDD($("#endLat").val()),
            "cores_required": $("#cores").val(),
            "cable_type_id": parseInt($("#cableType option:selected").val()),
            "buffer_radius_a": parseFloat($("#startBuffer").val()),
            "buffer_radius_b": parseFloat($("#endBuffer").val()),
            "core_level_result": app.is_core_feasibile ? 'Feasible' : 'Not Feasible',
            "feasibility_result": app.feasibilityStatus
        };
        feasibilityDetailsInfo.lstFeasibilityDetails = app.feasibilityDetailsInfo;
        feasibilityDetailsInfo.lstFeasibilityGeometry = app.feasibilityGeometry;

        $.ajax({
            type: "POST",
            url: "feasibilitydetails/SavefeasibilityDetails",
            data: JSON.stringify({ 'objFeasibilityDetailsView': feasibilityDetailsInfo }),
            dataType: "json",
            contentType: "application/json",
            success: function (result, status, xhr) {
                if (status == "success") {
                    alert(result.message);
                    app.resetFields();
                    // sf.feasibility_saved.push(result.result);
                    bulkFeasibility.AddArraySavedFeasibility(result.result);
                    bulkFeasibility.FeasibilityKMLModel = result.result;
                    //bulkFeasibility.DisplayDownloadFeasibleDiv();

                    bulkFeasibility.EnableKMZbutton();
                    //app.updateCableDetails(result.cableStatuses);
                    //app.processPaths();

                    // set history id
                    let resObj = JSON.parse(result.result);
                    if (resObj && resObj != undefined && resObj.history_id) {
                        bulkFeasibility.setHistoryID(resObj.history_id);
                    }
                }
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        });
    }

    this.SavefeasibilityForFtth = function () {

        var pathdist = app.NeDeviceDist;
        var objFTTHFeasibilityModel =
        {
            "feasibility_id": $("#hdn_feasibility_id").val(),
            "feasibility_name": $(app.DE.txtFseasibilityID).val(),
            "customer_name": $(app.DE.txtCustomerName).val(),
            "customer_id": $(app.DE.txtCustomerID).val(),
            "lat": app.DMStoDD($(app.DE.ftthLat).val()),
            "lng": app.DMStoDD($(app.DE.ftthLng).val()),
            "path_geometry": app.FtthLatLng,
            "entity_id": app.ftthEntityId,
            "path_distance": pathdist,
            "entity_location": app.neElmLoc.lat() + "," + app.neElmLoc.lng(),
            "buffer_radius": $(app.DE.txtBufferRadius_Ftth).val(),
        };

        $.ajax({
            type: "POST",
            url: "feasibilitydetails/SavefeasibilityDetailsFtth",
            data: JSON.stringify({ 'objFeasibilityModel': objFTTHFeasibilityModel }),
            dataType: "json",
            contentType: "application/json",
            success: function (result, status, xhr) {
                if (status == "success") {
                    alert(result.message);
                    app.resetFields();
                    bulkFeasibility.AddArraySavedFeasibility(result.result);
                    bulkFeasibility.FeasibilityKMLModel = result.result;

                    bulkFeasibility.EnableKMZbutton();

                    // set history id
                    let resObj = JSON.parse(result.result);
                    if (resObj && resObj != undefined && resObj.history_id) {
                        bulkFeasibility.setHistoryID(resObj.history_id);
                    }
                    $(app.DE.btnResetFtth).trigger("click");
                }
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        });
    }
    this.ExportPastFeasibilityRecord = function (searchBy, searchText) {
        var fromDate = $("#fromDate").val();
        var toDate = $("#toDate").val();
        window.location = appRoot + 'FeasibilityDetails/ExportPastFeasibilityRecord?search_by=' + searchBy + '&searchText=' + searchText + '&FromDate=' + fromDate + '&ToDate=' + toDate;
    }

    this.ExportMergedKML = function (searchBy, searchText) {
        var fromDate = $("#fromDate").val();
        var toDate = $("#toDate").val();
        window.location = appRoot + 'Report/ExportMergedKML?search_by=' + searchBy + '&searchText=' + searchText + '&FromDate=' + fromDate + '&ToDate=' + toDate;
    }

    this.setDateTimeCalendar_ExportEntities = function (startdateid, enddateid, startdateimgid, enddateimgid, isFutureDateAllowed) {




        //calendar.refresh();
        //var setup1 = showtime;
        Calendar.setup({
            inputField: startdateid,   // id of the input field
            button: startdateimgid,
            ifFormat: "%d-%b-%Y",       // format of the input field datetime format %d-%b-%Y %I:%M %p
            showsTime: false,
            timeFormat: "12",
            weekNumbers: false,

            //new Date(new Date().setMonth(new Date().getMonth() + 2))

            //maxDate: "+6m",
            //onUpdate: catcalc,

            disableFunc: function (date) {
                var endDateValue = document.getElementById(enddateid).value;

                var enddt = DateFormatter(endDateValue);
                enddt = new Date(enddt);
                if (date.getTime() > enddt.getTime()) {
                    return true;
                }
                ////
                if (!isFutureDateAllowed) {
                    var now = new Date();
                    now.setDate(now.getDate());
                    //now.setMonth(now.getMonth()+2);
                    if (date.getTime() > now.getTime()) {
                        return true;
                    }
                }
            }
        });

        //isFutureDateAllowed = true;
        Calendar.setup({
            inputField: enddateid,
            button: enddateimgid,
            ifFormat: "%d-%b-%Y",
            showsTime: false,
            timeFormat: "12",
            weekNumbers: false,
            //maxDate: new Date(new Date().setMonth(new Date().getMonth() + 6)),
            align: "Br",
            //maxDate: "+6m",
            disableFunc: function (date) {
                var startDateValue = document.getElementById(startdateid).value;

                var startdt = DateFormatter(startDateValue);
                startdt = new Date(startdt);
                if (date.getTime() < startdt.getTime()) {
                    return true;
                }
                if (!isFutureDateAllowed) {
                    var now = new Date();
                    now.setDate(now.getDate());
                    if (date.getTime() > now.getTime()) {
                        return true;
                    }
                }

            },

        });
    }

    //FTTH EXPORT

    this.ExportPastFeasibilityRecordFTTH = function (searchBy, searchText) {
        var fromDate = $("#fromDate").val();
        var toDate = $("#toDate").val();
        window.location = appRoot + 'FeasibilityDetails/ExportPastFeasibilityRecordFTTH?search_by=' + searchBy + '&searchText=' + searchText + '&FromDate=' + fromDate + '&ToDate=' + toDate;
    }

    this.ExportMergedKMLFTTH = function (searchBy, searchText) {
        var fromDate = $("#fromDate").val();
        var toDate = $("#toDate").val();
        window.location = appRoot + 'Report/ExportMergedKMLFTTH?search_by=' + searchBy + '&searchText=' + searchText + '&FromDate=' + fromDate + '&ToDate=' + toDate;
    }

}

function truncateText(str, length, ending) {
    if (length == null) {
        length = 50;
    }
    if (ending == null) {
        ending = '...';
    }
    if (str.length > length) {
        return str.substring(0, length - ending.length) + ending;
    } else {
        return str;
    }
}

// Code Section FTTH

$("#txtGoogleSearch,#txtEntitySearch").on("focus", function () {
    app.resetFields();
})

function showHideTab(value) {
     //$(app.DE.btnResetFtth).trigger("click");
   
    if (value == "network-panel") {
        $("#FFTH-layer-panel").parent().addClass("active");
        $("#MapLayers").show();
        $("#MapLayers").addClass("active");
        $("#btn-ftth-lyr").parent().removeClass("active");
        $("#btn-ntwrk-lyr").parent().addClass("active");
        $("#btn-wireless").parent().removeClass("active");
        $("#btn-wrless-lyr").parent().removeClass("active");
        $("#wireless_Layers").hide();
        $("#FFTH-layer-panel").hide();
    }
    else if (value == "ftth-panel") {
        $("#FFTH-layer-panel").addClass("active");
        $("#FFTH-layer-panel").show();
        $("#MapLayers").removeClass("active");
        $("#btn-ftth-lyr").parent().addClass("active");
        $("#btn-ntwrk-lyr").parent().removeClass("active");
        $("#btn-wireless").parent().removeClass("active");
        $("#btn-wrless-lyr").parent().removeClass("active");
        $("#wireless_Layers").hide();
        $("#MapLayers").hide();
    }
  
    else if (value == "wireless-panel") {
        $("#btn-ntwrk-lyr").parent().removeClass("active");
        $("#btn-ftth-lyr").parent().removeClass("active");
        $("#btn-wrless-lyr").parent().addClass("active");
        $("#wireless_Layers").show();
        $("#enterprise-layers").hide();
        $("#MapLayers").hide();
        $("#FFTH-layer-panel").hide();
    }
    
    else {
        $(app.DE.btnResetFtth).trigger("click");
        // app.LoadMap();
        //  app.resetFields();
        //   CheckAllLayrs('Layers', false);
        $("#FFTH-layers input[type='checkbox']").attr('checked', false);
        $("#ftth-sel-option").val($("#ftth-sel-option option:first").val());
        app.distanceWidget = undefined;
        if ($("#txtEntitySearch").is(":visible")) {
            $("#txtEntitySearch").val('');
            $("#txtEntitySearch").hide();
            $("#txtGoogleSearch").show();
            // $("#ftth-sel-option").hide();
        }
        $("#txtGoogleSearch").val('');
        if (value == "enterprise") {
            $(app.DE.btnReset).trigger("click");
            $(app.DE.closeModalPopup).trigger("click");
            //$(app.DE.btnResetFtth).trigger("click");
            $('#hdnFeasibilityInput').val('');
            $("#FFTH-layers").hide();
            $("#enterprise-layers").show();

            $("#FFTH-layer-panel").removeClass("active");
            $("#btn-wireless").removeClass("active");
            $("#MapLayers").addClass("active");
            $("#div-tab-panel").hide();
            $("#btn-wireless").parent().removeClass("active");

            $("#btn-enterprise").parent().addClass("active");

            $("#btn-ffth").parent().removeClass("active");
            $("#legend").hide();
            //$("#frmFeasibilityInput")[0].reset();
            app.resetFields();
            //$("#ad-searchftth").hide();
            if ($("#myModal").is(":visible")) {
                $("#myModal").hide();
            }
            if ($(".routeDistance").is(":visible")) {
                $(".routeDistance").html(' ');
                $(".routeDistance").hide();
            }

            app.lyrTbls = [];
            app.lyrNames = [];
            $("#txtGoogleSearch").removeClass("pac-target-input");
            $(".pac-container").hide();
            // $("#ftth-sel-option").hide();
            $("txtGoogleSearch").css('width', '100%!important');
            $("#myModal").hide();
            // $("#myModal").addClass("modal");
            //  $("#myModal").removeClass("custom-modal");
            isFtth = false;
        }



        else if (value == "ftth") {
            app.resetFields();
            $(app.DE.closeModalPopup).trigger("click");
            //$(app.DE.btnResetFtth).trigger("click");
            $("#FFTH-layers").show();
            $("#div-tab-panel").show();
            $("#enterprise-layers").hide();
            $("#btn-enterprise").parent().removeClass("active");
            $("#btn-ffth").parent().addClass("active");
            $("#btn-wireless").parent().removeClass("active");
            if ($("#btn-ftth-lyr").parent().hasClass("active")) {
                $("#btn-ftth-lyr").parent().removeClass("active");
                $("#btn-ntwrk-lyr").parent().addClass("active");
            }
            if ($("#FFTH-layer-panel").hasClass("active")) {

                $("#FFTH-layer-panel").removeClass("active");
                $("#MapLayers").addClass("active");
            }
            $("#legend").show();
            if ($(".legends").is(":visible")) {
                $(".legends").hide();
            }
            $("#frmFeasibilityInput")[0].reset();
            $("txtGoogleSearch").css('width', '73%!important');
            // $("#ftth-sel-option").show();
            var width = $(window).width();
           // $('#myModal').show();
           // $("#myModal .modal-body").show();
            if ($("#floating-panel-right").is(":visible")) {

                $(".modal-manager").css({ 'width': '' + width - 600 + 'px', 'left': '300px' });
            }
            else {
                $(".modal-manager").css({ 'width': '' + width - 300 + 'px', 'left': '300px' });
            }


            isFtth = true;
        }
        else if (value == "wireless") {
            $("#btn-ffth").parent().removeClass("active");
            $("#btn-enterprise").parent().removeClass("active");
            $("#btn-wireless").parent().addClass("active");
            $("#btn-wrless-lyr").parent().addClass("active");
            $("#btn-ntwrk-lyr").parent().removeClass("active");
            $("#wirelessBox").show();
            $("#enterprise-layers").hide();
            $("#FFTH-layers").hide();
        }
        
    }
}

function GetFtthInfo() {
    var st = $(app.DE.ftthLat).val() + "," + $(app.DE.ftthLng).val();
    var st_ac = new google.maps.places.Autocomplete(st);
    app.geocoder = new google.maps.Geocoder();
    app.BldInfo = new google.maps.InfoWindow({ content: '' });
    app.NeElmInfo = new google.maps.InfoWindow({ content: '' });
    app.service = new google.maps.places.PlacesService(sf.map);
    app.DR = new google.maps.DirectionsRenderer({ suppressMarkers: !0, polylineOptions: { strokeColor: '#EF4B41', strokeWeight: 2 } });
    google.maps.event.addListener(st_ac, 'place_changed', function () {
        var thisAdd = st.value;
        var place = st_ac.getPlace();
        if (app.SearchMarker) app.SearchMarker.sf.setMap(null);
        // 
        if (place.geometry) {
            if (place.geometry.viewport) {
                sf.map.fitBounds(place.geometry.viewport);
            } else {
                app.searchLoc = place.geometry.location.lat() + ' ' + place.geometry.location.lng();
                sf.map.setCenter(place.geometry.location);
                sf.map.setZoom(19);  // Why 17? Because it looks good.               
            }
            startWidget();

        }
        else
            GeoCodeIt(thisAdd);
        toggleSearch();
        $("#txtGoogleSearch").removeClass('ui-autocomplete-loading');
        if (app.searchLoc == '')
            app.searchLoc = place.geometry.location.lat() + ' ' + place.geometry.location.lng();
        if (thisAdd != "" && app.searchLoc)
            SaveFeasibilityFtth(app.searchAdd, thisAdd, app.searchLoc);//Used to save feasibility history which has been performed by user.
        //Add default calling
        //CheckLayer('P2P', true, !0);//Not in use

        //  $('.modal-title').html(thisAdd);
    });

}

function toggleSearchOpt() {
    if ($('.localAddSearch').is(':visible'))
        $('.localAddSearch').slideUp("slow");
    else
        $('.localAddSearch').slideDown("slow");

    $('ul.prolist').empty();
    //$('.footer_lower').slideToggle(function () {
    //    var txt = $("#poi_address").text();
    //    var is_visible = bottom_visible();

    //    if (is_visible) {
    //        $('.c_position_txt,#spn_location').hide();
    //        $('#header_layer_div').show();
    //    }
    //    else {
    //        $('.c_position_txt,#spn_location').show();
    //        $('#header_layer_div').hide();
    //    }
    //    toggleSearch();
    //});
}

function toggleSearch() {
    var txt = $(".modal-title").text();
    var is_visible = bottom_visible();
    var srch = $('.search_div');
    var lsrch = $('.optAddSearch');
    // var poi = $('.modal-title');

    //if (is_visible && txt != '') {
    //    srch.hide();
    //    var t = $('div.header div.right_side');
    //    poi.appendTo(t).css("line-height", t.height() + 'px');
    //    $('.f_tools').show();
    //}
    //else {
    //    var t = $('div.footer div.right_side');
    //    poi.appendTo(t).css("line-height", t.height() + 'px');
    //    srch.show();
    //    $('.f_tools').hide();
    //}
}

function closehvrpopup() {
    $(".pop").hide();
}

function bottom_visible() {
    return $('#FFTH-layers').is(':visible');
}



function addArrayItm(itmArray, item) {
    var found = $.inArray(item, itmArray);
    if (found < 0)
        itmArray.push(item);
    return itmArray;
}

function removeArrayItm(itmArray, item) {
    var found = $.inArray(item, itmArray);
    if (found >= 0)
        itmArray.splice(found, 1);

    return itmArray;
}

function CheckAllLayrs(typ, checked) {
  
    var elms = $('#FFTH-layer-panel ul:first-child input[name="' + typ + '"]');
    $(elms).prop('checked', checked);
    if (checked) {
        $.each(elms, function (indx, item) {
            var lyrTxt = $(this).closest('li').next().find('span').text();
            if (lyrTxt != '') addArrayItm(app.lyrNames, lyrTxt);
            var lyrTbl = $(item).attr('value');
            if (lyrTbl != null && lyrTbl != 'on')
                addArrayItm(app.lyrTbls, lyrTbl);
        });
    }
    else {
        //  app.resetFields();
        $.each(elms, function (indx, item) {

            var lyrTxt = $(this).closest('li').next().find('span').text();
            if (lyrTxt != '') removeArrayItm(app.lyrNames, lyrTxt)
            var lyrTbl = $(this).closest('li').next().find('input').attr('value');
            if (lyrTbl != null && lyrTbl != 'on')
                removeArrayItm(app.lyrTbls, lyrTbl);
        });
        $('#tblbatchLOS tbody').html('<tr><td colspan="10" valign=="center"><strong>No records found.</strong></td></tr>');
        $("#myModal").hide();
    }
    getNEDetails(app.distanceWidget);
}

function getNEDetails(wdgt) {
    if (app.polylines.length) {
        for (var i = 0; i < app.polylines.length; i++) {
            app.polylines[i].setMap(null);
        }
    }
    var bufferradius = $(app.DE.txtBufferRadius_Ftth).val();
    if (wdgt == undefined)
        return;
    //Clear previous drawing
    removeBMarker();
    if (app.lyrmarkers) {
        for (var i = 0; i < app.lyrmarkers.length; i++) {
            if (app.lyrmarkers[i].setmap != null) app.lyrmarkers[i].setmap(null);
        }
    }
    //APP.lyrMarkers = [];
    var rds = app.distanceWidget.get('distance') * 1000;
   // alert(rds)
    wdgt = app.distanceWidget;
    var center = wdgt.position.lng() + ' ' + wdgt.position.lat();
    $(app.DE.ftthLat).val(wdgt.position.lat());
    $(app.DE.ftthLng).val(wdgt.position.lng() );

    if (wdgt) {
        if (app.lyrNames.length > 0 && app.lyrTbls.length > 0) {
            ajaxReq('Main/GetNEntityDetails', { custLoc: center, radius: rds, lyrsTbl: app.lyrTbls, lyrsName: app.lyrNames }, true, function (resp) {
                //var thhtml = '<tr class="GridTableHeaderOrange"><th width="20"><span class="flaticon-undo19"></span></th>' +
                //        '<th width="40"></th>' +//<span class="old_flaticon-couple35"></span>
                //        '<th>Network  ID</th>' +
                //        '<th>Network Type</th>' +
                //        '<th>Name</th>' +
                //        '<th>Status</th>' +
                //        '<th>Total No. of Ports</th>' +
                //        '<th>Port Utilization</th>' +
                //        '<th>Faulty Port</th>' +
                //        '<th>Utilization</th></tr>';
                //$('#tblbatchLOS thead').html(thhtml);
                // $('#myModal').modal('show');
                // $('#myModal').draggable();
                // $("#myModal .modal-body").show();
                // $("#expand").hide();
                //$("#minimise").show();
                //  $(".custom-modal").attr('style', 'max-height:280px !important;');
              
                removeBMarker();
                var tbhtml = "";
                if (resp != null && (resp.Status && resp.Data != "[]")) {
                    $.each(resp.Data, function (indx, item) {

                        if (Math.abs(indx % 2) == 1)
                            tbhtml += '<tr class="odd">';
                        else
                            tbhtml += '<tr class="even">';
                        var ltLng = item.NeLat + ',' + item.NeLng;
                        var portUlzn = parseInt(item.Utilization);
                        var entityAtSameLocation = $(resp.Data).filter(function (i, n) { return (n.NeLat + ',' + n.NeLng) == ltLng; });
                        var neElmIcon = getNEntityLyrsImage(item.NetworkType, item.Status, portUlzn);
                        tbhtml += '<td><input type="radio" name="rdbFsbNeDetails" id="en' + indx + '" value="' + ltLng + '"data-id="' + item.NetworkId + '" onChange="getDirection(this,false)" /></td>';
                        tbhtml += '<td><img style="width:20px;" src="' + neElmIcon + '" alt="" /></td>';
                        tbhtml += '<td>' + item.NetworkId + '</td>';
                        tbhtml += '<td>' + item.NetworkType + '</td>';
                        tbhtml += '<td>' + item.Name + '</td>';
                        tbhtml += '<td>' + item.Status + '</td>';
                        tbhtml += '<td>' + item.TotalNofPorts + '</td>';
                        tbhtml += '<td>' + item.PortUtilization + '</td>';
                        tbhtml += '<td>' + item.FaultyPort + '</td>';
                        tbhtml += '<td>' + portUlzn + '</td></tr>';
                        var htmlOutput = EntityAtSameLocation(entityAtSameLocation);
                        var neElmPos = new google.maps.LatLng(parseFloat(item.NeLat), parseFloat(item.NeLng));
                        var neElmIcon = getNEntityLyrsImage(item.NetworkType, item.Status, portUlzn);
                        var mrkrOptions = { map: sf.map, position: neElmPos, title: item.NetworkId, icon: neElmIcon };
                        var crtElmMarker = new google.maps.Marker(mrkrOptions);
                        crtElmMarker.setMap(sf.map);
                        bindNeElmInfo(crtElmMarker, htmlOutput);
                        app.lyrMarkers.push(crtElmMarker);
                      
                    });
                    app.tbhtml = tbhtml;
                    if (app.tbhtml != undefined && $('#hdnFeasibilityInput').val() == "ftth") {
                        
                        $('#tblbatchLOS tbody').html(app.tbhtml);
                        $(this).parents('tr').find('.downloadBulkReports').css("visibility", "visible");

                    }

                    else {
                        $('#tblbatchLOS tbody').html('<tr><td colspan="10" valign=="center"><strong>Please click on Compute to get entity details.</strong></td></tr>');
                    }
                }

                else {
                    removeBMarker();
                    $('#myModal').hide();
                    app.tbhtml = undefined;
                    $('#tblbatchLOS tbody').html('<tr><td colspan="10" valign=="center"><strong>No records found.</strong></td></tr>');
                    if (app.tbhtml == undefined && $('#hdnFeasibilityInput').val() == "ftth") {
                        $('.downloadBulkReports').css("visibility", "hidden");
                        alert("No data available");
                    }

                }
                getBuildings();
                showNetworkLayers();
            }, true);
            
        }
    }
}

function removeBMarker() {
    if (app.BuildMrkr) app.BuildMrkr.setMap(null);
    if (app.BuildLine) app.BuildLine.setMap(null);
    if (app.DirRndr) { app.DirRndr.setMap(null); $(".routeDistance").hide(); };
    RemoveFromMap(app.DirRoutes);
    if (app.lyrMarkers) {
        for (var i = 0; i < app.lyrMarkers.length; i++) {
            if (app.lyrMarkers[i].setMap != null) app.lyrMarkers[i].setMap(null);
            google.maps.event.clearListeners(app.lyrMarkers[i], 'click');
        }
    }
}

function RemoveFromMap(arr) {
    arr = arr || [];
    for (i = 0; i < arr.length; i++)
        arr[i].setMap(null);
}

function getBuildings() {
    RemoveFromMap(app.Buildings);
    app.Buildings = [];
    if (app.distanceWidget)
        getBuildHere(app.distanceWidget.position);

}

function showNetworkLayers() {
    if (app.NetworkLayers && app.NetworkLayers.length > 0) {
        var layerParam = { Name: getComSepArrItems(APP.NetworkLayers), DisplayName: "CSABoundary", Filters: [], MapFilePath: mapDirPath + "NetworkEntitiesNoLabel_feasibility.map" };
        // var layerParam = { Name: "CSABoundary", DisplayName: "CSABoundary", Filters: [], MapFilePath: mapDirPath + "NetworkEntitiesLabel.map" };
        var overlayLayer = createOverlayLayer(layerParam, false, null);
        addNewOverlay(overlayLayer);
    }
    else {
        removeLayerIfFound("Network Layer");
    }
}

function addNewOverlay(lyrObj) {
    var foundFlag = true;
    var newLyr = { Name: lyrObj.name, layerObject: lyrObj, Opacity: lyrObj.opacity, visible: true }
    for (var i = 0; i < app.layerManager.length; i++) {
        if (app.layerManager[i].Name == lyrObj.name) {
            app.layerManager[i] = newLyr;
            foundFlag = false;
            break;
        }
    }
    if (foundFlag)
        app.layerManager.push(newLyr);
    reLoadGoogleLayers();
}

function reLoadGoogleLayers() {
    removeAllWMSLayers();
    $.each(app.layerManager, function (i, item) {
        if (item.visible)
            sf.map.overlayMapTypes.push(item.layerObject);
    });
}

function removeLayerIfFound(lyrName) {
    for (var i = 0; i < app.layerManager.length; i++) {
        if (app.layerManager[i].Name == lyrName) {
            app.layerManager.splice(i, 1);
            break;
        }
    }
    reLoadGoogleLayers();
}

function removeAllWMSLayers() {
    for (i = sf.map.overlayMapTypes.length - 1; i >= 0; i--) {
        sf.map.overlayMapTypes.removeAt(i)
    }
}

function getBuildHere(position) {

    var center = position.lng() + ' ' + position.lat();
    var rds = app.distanceWidget.get('distance') * 1000;
    //ajaxReq('do_it.ashx', 'BuildingsHere', { Center: center }, plotBuildings, true);
    ajaxReq('Main/GetBuildingsHere', { centerLatLngPoints: center, rdsValue: rds }, true, plotBuildings);
}

function plotBuildings(Brr) {
    for (i = 0; i < Brr.length; i++) { }
    createBuilding(Brr[i]);
}

function createBuilding(bld) {
    if (bld && bld.geom.length) {
        var boundaries = bld.geom.split('),(');
        var paths = [];
        //address,city,state,pincode,locality,house_type
        for (j = 0; j < boundaries.length; j++)
            paths.push(getLatLongArr(boundaries[j]));

        var poly = createPolygon(paths);

        google.maps.event.addListener(poly, 'click', function (e) {
            app.BldInfo.setContent('<strong>Address :</strong> ' + (bld.address || '') + '<br /><strong>City :</strong> ' + bld.city + '<br/><strong>Pincode :</strong> ' + (bld.pin_code || '') + ' ');
            app.BldInfo.setPosition(e.latLng);
            app.BldInfo.open(map);
        });

        app.Buildings.push(poly);
    }
}
function createPolygon(_path) {
    var _poly = new google.maps.Polygon({
        fillColor: '#ff00004d',
        strokeOpacity: 0.8,
        strokeWeight: 1,
        strokeColor: '#00ff00',
        fillOpacity: 1,
        map: map,
        paths: _path
    });
    return _poly;
}

function getAddressList() {
    $('#ddlGroupBody').hide();
    var _add = $('#txtGoogleSearch').val();
    var splt_add = _add.toLowerCase().split(' ');
    var matching_string = '';
    for (i = 0; i < splt_add.length; i++) {
        matching_string += (matching_string == '') ? '(' + splt_add[i] + ')' : ' | (' + splt_add[i] + ')';
    }

    if (_add.length > 2) {

        ajaxReq('Main/GetMatchedAddresses', { addString: _add }, true, function (resp) {
            try {
                //resp = $.parseJSON(j);
                var htmldata = '';
                if (resp.status) {
                    //Refresh Tree
                    if (resp.data.length > 0) {
                        for (var i = 0; i < resp.data.length; i++) {
                            //var add = resp.data[i].address.replace('/' + matching_string + '/gi', makeSpan);
                            var add = matchText(resp.data[i].address, { words: splt_add });
                            htmldata += "<li onclick=\"saveSiteNShow('" + resp.data[i].address + "','" + resp.data[i].latlng + "');\" > " + add + "</li>";
                        }
                        $('ul.prolist').empty();

                        $('ul.prolist').html(htmldata);
                        $('#ddlGroupBody').slideDown("slow");
                        app.searchAdd = _add;
                    }
                    else {
                        htmldata += "<li style=\"font-weight:700; text-align:center;\"><span>No records found.</span></li>";
                        $('ul.prolist').empty();
                        $('ul.prolist').html(htmldata);
                        $('#ddlGroupBody').slideDown("slow");
                    }
                }
                else {
                    alert("Oops! Something went wrong.");
                    $('div.loader').hide();
                }
            }
            catch (e) {
            }
        }, true);
    }
}

function matchText(stra, opts) {
    var words = opts.words || [],
        regex = RegExp(words.join('|'), 'gi'),
        replacement = '<span style="color:red;">$&</span>';
    return stra.replace(regex, replacement);
}

function saveSiteNShow(add, latlng) {
    var place = latlng.split(' ');
    var loc = new google.maps.LatLng(parseFloat(place[0]), parseFloat(place[1]));
    sf.map.setCenter(loc);
    sf.map.setZoom(17);  // Why 17? Because it looks good.     
    startWidget();
    SaveFeasibilityFtth(app.searchAdd, add, latlng);//Used to save feasibility history which has been performed by user.
    //showNetworkLayers(); //Function called already inside above function.
    //  $('.modal-title').text(add);
    $('#ddlGroupBody').hide();
}

function SaveFeasibilityFtth(searchAdd, locAdd, latLng) {
    ajaxReq('Main/SaveFeasibilityForFtth', { searchLoc: searchAdd, locAdd: locAdd, locPoints: latLng }, true, function (resp) {
        if (resp)
            var msg = resp;
    }, false);
}


function startWidget() {
    
    //Create Circle on Google Address Search
    initDistanceWidget(sf.map, app.iRadiusInKiloMeter);
    fadeOuter(sf.map.getCenter(), app.iRadiusInKiloMeter);
    // setPopupTop(map.getCenter());
    //getAOIDetail(APP.distanceWidget);
    //getAOIDetailNew(APP.distanceWidget);//Commented bcoz of duplicate call
    lyrsDefaultChkd(true);
    // $('#txtwithin').val('2.00');
}

function initDistanceWidget(map, mindistance) {
   
    if (app.distanceWidget) {
        app.distanceWidget.set("map", null);
        app.distanceWidget = null;
    }

    app.distanceWidget = new DistanceWidget_FTTH({
        map: app.map,
        distance: mindistance, // Starting distance in km.
        maxDistance: 2, // Twitter has a max distance of 2500km.
        color: '#3a79c8',
        activeColor: '#3a79c8',
        //sizerIcon: {
        //    url: baseUrl + appRoot + 'Content/Images/resize.png',
        //    size: new google.maps.Size(24, 24),
        //    origin: new google.maps.Point(0, 0),
        //    anchor: new google.maps.Point(12, 12)
        //},
        icon: {
            url: baseUrl + appRoot + 'Content/Images/center.png',
            size: new google.maps.Size(21, 21),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(10, 10)
        }
    });
}

function fadeOuter(_center, _radius) {


    if (app.fadeMap) {
        app.fadeMap.setMap(null);
    }
    var fadeOptions = {
        strokeWeight: 0,
        fillColor: '#ffffff',
        fillOpacity: 0.6,
        map: sf.map,
        paths: [app.fadedmap, drawCircle(_center, _radius, -1)]
    };
    app.fadeMap = new google.maps.Polygon(fadeOptions);

}

function lyrsDefaultChkd(checked) {
    var chkElms = $('.footer_lower ul:first-child input[type="checkbox"]');
    $(chkElms).prop('checked', checked);
    if (checked) {
        $.each(chkElms, function (indx, item) {
            var lyrTxt = $(this).closest('li').next().find('span').text();
            if (lyrTxt != '') addArrayItm(app.lyrNames, lyrTxt);
            var lyrTbl = $(item).attr('value');
            if (lyrTbl != null && lyrTbl != 'on')
                addArrayItm(app.lyrTbls, lyrTbl);
        });
    }
    else {
        $.each(chkElms, function (indx, item) {
            var lyrTxt = $(this).closest('li').next().find('span').text();
            if (lyrTxt != '') removeArrayItm(app.lyrNames, lyrTxt);
            var lyrTbl = $(item).attr('value');
            if (lyrTbl != null && lyrTbl != 'on')
                removeArrayItm(app.lyrTbls, lyrTbl);
        });
    }

    //GET default selected network entity layers details
    getNEDetails(app.distanceWidget);
}

function drawCircle(point, radius, dir) {
    var d2r = Math.PI / 180;   // degrees to radians
    var r2d = 180 / Math.PI;   // radians to degrees
    var earthsradius = 6371; // 3963 is the radius of the earth in miles
    var points = 360;

    // find the raidus in lat/lon
    var rlat = (radius / earthsradius) * r2d;
    var rlng = rlat / Math.cos(point.lat() * d2r);

    var extp = new Array();
    if (dir == 1) { var start = 0; var end = points + 1 } // one extra here makes sure we connect the ends
    else { var start = points + 1; var end = 0 }
    for (var i = start; (dir == 1 ? i < end : i > end); i = i + dir) {
        var theta = Math.PI * (i / (points / 2));
        ey = point.lng() + (rlng * Math.cos(theta)); // center a + radius x * cos(theta)
        ex = point.lat() + (rlat * Math.sin(theta)); // center b + radius y * sin(theta)
        extp.push(new google.maps.LatLng(ex, ey));
    }
    return extp;
}


function getNEntityLyrsImage(neType, status, utlzn) {
    var neIcon = baseUrl + appRoot;
    var plasb = "";
    if (status != '' && /planned/i.test(status))
        plasb = 'p';
    else
        plasb = 'a';

    switch (neType) {
        case "ONT":
            if (utlzn >= 0 && utlzn <= 50)
                neIcon += "Content/Images/plnasb/ont" + plasb + "-g.png";
            else if (utlzn > 50 && utlzn <= 80)
                neIcon += "Content/Images/plnasb/ont" + plasb + "-y.png";
            else if (utlzn > 80) //(utlzn > 80 && utlzn <= 100)
                neIcon += "Content/Images/plnasb/ont" + plasb + "-r.png";
            break;
        case "Secondary Splitter":
            if (utlzn >= 0 && utlzn <= 50)
                neIcon += "Content/Images/plnasb/SS" + plasb.toUpperCase() + "Range1.png";
            else if (utlzn > 50 && utlzn <= 80)
                neIcon += "Content/Images/plnasb/SS" + plasb.toUpperCase() + "Range2.png";
            else if (utlzn > 80) //(utlzn > 80 && utlzn <= 100)
                neIcon += "Content/Images/plnasb/SS" + plasb.toUpperCase() + "Range3.png";
            break;
        case "Primary Splitter":
            if (utlzn >= 0 && utlzn <= 50)
                neIcon += "Content/Images/plnasb/PS" + plasb.toUpperCase() + "Range1.png";
            else if (utlzn > 50 && utlzn <= 80)
                neIcon += "Content/Images/plnasb/PS" + plasb.toUpperCase() + "Range2.png";
            else if (utlzn > 80) //(utlzn > 80 && utlzn <= 100)
                neIcon += "Content/Images/plnasb/PS" + plasb.toUpperCase() + "Range3.png";
            break;
        default:
            neIcon += "Content/Images/LayerIcon/" + neType + ".png";

    }
    return neIcon;
}

function EntityAtSameLocation(entityAtSameLocation) {
    var html = ""; var tbhtml = "";
    for (var i = 0; i < entityAtSameLocation.length; i++) {
        var item = entityAtSameLocation[i];
        var ltLng = item.NeLat + ',' + item.NeLng;
        var portUlzn = parseInt(item.Utilization);

        html += '<ul style="border:2px solid #d6d5d5;" ><li style="margin:5px"><strong class="lsnp">Network Id:</strong><span class="rsnp"> ' + item.NetworkId + '</span></li>' +
            '<li style="margin:5px"><strong class="lsnp">Asset Id:</strong><span class="rsnp"> ' + item.AssetId + '</span></li>' +
            '<li style="margin:5px"><strong class="lsnp">Network Stage:</strong><span class="rsnp"> ' + item.Status + '</span></li>' +
            '<li style="margin:5px"><strong class="lsnp">Network Type:</strong><span class="rsnp"> ' + item.NetworkType + '</span></li>' +
            '<li style="margin:5px"><strong class="lsnp">Utilization:</strong><span class="rsnp"> ' + portUlzn + '</span></li>' +
            //"<li style='margin:5px'><strong class=\"lsnp\">Customer Count: </strong><a href=\"javascript:void(0)\" onclick=\"showAssociatedCustomers(this, '" + item.NetworkId + "', '" + item.NetworkType + "');\"><span class=\"rsnp\"> " + item.CustomerCount + "</a></span></li></ul>";
            "<li style='margin:5px'><strong class=\"lsnp\">Customer Count: </strong><span class=\"rsnp\"> " + item.CustomerCount + "</span></li></ul>";
    }
    return html;
}

function bindNeElmInfo(elmMarker, htmlContent) {
    google.maps.event.addListener(elmMarker, "click", function (e) {
        var nwId = $(this).attr('title');
        var infoHtml = '<div class="networkAttInfo"><h3 style="background-color:#50b3ea; color:#fff;text-align:center; padding: 5px 0px 5px 0px;">Network Info</h3><div> ' + htmlContent + '';
        infoHtml += '<ul><li><a href="javascript:void(0)" class="btn-copy"  onclick="fnCopyToSystemId(\'' + nwId + '\');">Click  To Copy</a></li></ul></div></div>';
        app.NeElmInfo.setContent(infoHtml);
        //APP.BldInfo.setPosition(e.latLng);
        app.NeElmInfo.open(sf.map, elmMarker);

    });
}






function getDirection(ctrl, ispast) {
     
   
    if (app.polylines.length) {
        for (var i = 0; i < app.polylines.length; i++) {
            app.polylines[i].setMap(null);
        }
    }
    if (sf.map) { if (app.DirRndr != undefined) app.DirRndr.setMap(null); $(".routeDistance").hide(); }
    if (app.DirRoutes.length) {
        for (i = 0; i < app.DirRoutes.length; i++) {
            app.DirRoutes[i].setMap(null);
        }
        if (app.polylines.length) {
            for (var i = 0; i < app.polylines.length; i++) {
                app.polylines[i].setMap(null);
            }
        }
        app.DirRoutes = [];
        $(".routeDistance").hide();
    }
    if (app.FtthLatLng.length) {
        app.FtthLatLng = [];
    }
    app.ftthEntityId = $(ctrl).attr('data-id');
    var points = $(ctrl).attr('value');
    var dstLtLng = points
    var srcLtLng = '';
    if (app.distanceWidget)
        srcLtLng = app.distanceWidget.position.lat() + ',' + app.distanceWidget.position.lng();

    if (dstLtLng == '' || srcLtLng == '')
        return;
    var rendererOptions = {
        draggable: false,
        polylineOptions: { strokeColor: '#EF4B41', strokeWeight: 3 },
        markerOptions: {
            icon: {
                //url: baseUrl + appRoot + 'images/red_m.png',
                url: 'http://maps.gstatic.com/mapfiles/markers2/dd-via.png',
                size: new google.maps.Size(20, 18),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(10, 9)
            }
        }
    };

    app.DirRndr = new google.maps.DirectionsRenderer(rendererOptions);
    app.DirSvc = new google.maps.DirectionsService();
    app.detailsMenuFTTH = new DetailsMenuFTTH();
    app.infowindow = new google.maps.InfoWindow();
    // app.editMenu = new EditMenu();
    app.TrvlMode = google.maps.TravelMode.DRIVING;
    var dirReq = {
        origin: srcLtLng,
        destination: dstLtLng,
        provideRouteAlternatives: false,
        travelMode: app.TrvlMode
    };

    app.DirSvc.route(dirReq, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            //  app.DirRndr.setDirections(response);//.routes[0].overview_path
            //$("#ttDistance").html(response.routes.length);
            // app.DirRndr.setMap(sf.map);
            //RemoveFromMap(APP.DirRoutes);
            //APP.DirRoutes = [];
            var DirPTs = ExtractStartEnd(response);
            var custLoc = app.distanceWidget.position;
            app.neElmLoc = new google.maps.LatLng(points.split(',')[0], points.split(',')[1])
            app.DirRoutes.push(getDottedLine([DirPTs.Start, custLoc]));
            var bounds = new google.maps.LatLngBounds()
            var legs = response.routes[0].legs;
            app.stepPolyline = new google.maps.Polyline(rendererOptions.polylineOptions);
            for (i = 0; i < legs.length; i++) {
                var steps = legs[i].steps;
                for (j = 0; j < steps.length; j++) {
                    var nextSegment = steps[j].path;

                    for (k = 0; k < nextSegment.length; k++) {
                        app.stepPolyline.getPath().push(nextSegment[k]);
                        bounds.extend(nextSegment[k]);
                        var latlng = nextSegment[k].lat().toFixed(6) + "," + nextSegment[k].lng().toFixed(6);
                        app.FtthLatLng.push(latlng);
                        //app.polylines.push(latlng);
                    }
                    sf.map.fitBounds(bounds);
                    if (!ispast) {
                        google.maps.event.addListener(app.stepPolyline, 'mouseover', function (evt) {
                            if (app.infowindow)
                                app.infowindow.setMap(null);

                            if (app.detailsMenuFTTH) {
                                app.detailsMenuFTTH.open(app.map, this, event.latLng, app.NeDeviceDist);
                            };
                        });
                    }
                    app.polylines = [];

                    app.polylines.push(app.stepPolyline);
                    app.stepPolyline.setMap(sf.map);
                }

            }
            app.DirRoutes.push(getDottedLine([DirPTs.End, app.neElmLoc]));
            app.NeDeviceDist = response.routes[0].legs[0].distance.text;
            var pathdist = app.NeDeviceDist;
            if (app.NeDeviceDist.split(' ')[1] == "m") {
                pathdist = parseInt(app.NeDeviceDist.split(' ')[0]) / 1000 + " km";
            }

            $(".routeDistance").text('Route Distance: ' + pathdist);
            $(".routeDistance").show();
            $("#btnlistFtth").removeAttr("disabled");
            if (!ispast) {
                for (subpath of app.DirRoutes) {
                    if (subpath) {
                        // console.log(ln.getPath().getAt(i).toUrlValue(6));
                        google.maps.event.addListener(subpath, 'mouseover', function (event) {
                            if (app.infowindow)
                                app.infowindow.setMap(null);

                            if (app.detailsMenuFTTH) {
                                app.detailsMenuFTTH.open(app.map, this, event.latLng, app.NeDeviceDist);
                            }
                        })
                    }
                }
            }
            sf.map.setZoom(sf.map.getZoom() - 1);



            //for (var i = 0; i < response.routes[0].getPath().getLength() ; i++) {
            //    //  bounds.extend(ln.getPath().getAt(i));
            //    app.FtthLatLng.push(response.routes[0].getPath().getAt(i).toUrlValue(6));

            //}

        }
    });
}


function ExtractStartEnd(result) {
    var stps = result.routes[0].legs[0].steps.length;
    var start = result.routes[0].legs[0].steps[0].start_location;
    var end = result.routes[0].legs[0].steps[stps - 1].end_location;
    return { Start: start, End: end };
}

//$(document).on("click","#close",function () {
//    $("#myModal").toggle();
//})

function getDottedLine(llArr) {
    var lineSymbol = { path: 'M 0,-0.25 0,0.25', strokeOpacity: .8, scale: 3 };
    var ln = new google.maps.Polyline({
        path: llArr,
        map: sf.map,
        strokeOpacity: 0,
        strokeWeight: 1,
        strokeColor: '#050505',
        icons: [{
            icon: lineSymbol,
            offset: '0',
            repeat: '10px',
        }],

    });
    for (var i = 0; i < ln.getPath().getLength(); i++) {
        //  bounds.extend(ln.getPath().getAt(i));
        app.FtthLatLng.push(ln.getPath().getAt(i).toUrlValue(6));
    }
    return ln;
}

function fnCopyToSystemId(str) {

    var aux = document.createElement("input");
    // Assign it the value of the specified element
    aux.setAttribute("value", str);
    // Append it to the body
    document.body.appendChild(aux);
    // Highlight its content
    aux.select();
    // Copy the highlighted text
    document.execCommand("copy");
    // Remove it from the body
    document.body.removeChild(aux);
}
function minmize(value) {
    $("#myModal .modal-body").hide();
    $("#expand").show();
    $("#minimise").hide();
    $(".custom-modal").attr('style', 'height:50px !important;');
}
function expand(value) {
   
    $("#myModal .modal-body").show();
    $("#expand").hide();
    $("#minimise").show();
    $(".custom-modal").attr('style', 'height:500px !important;');
    ;
}

function setPopUpPosition(_pos) {
    var _pt = getPointFromLatLng(_pos);
    $(".pop").css({ left: _pt.x - 104 }).show();
}

function setPopupTop(_pos) {
    var _pt = getPointFromLatLng(_pos);
    $(".pop").css({ top: _pt.y - 84 });
}

function setPopupContent(_txt) {
    $(".pop .drag-title").html(_txt);
}

function getPointFromLatLng(_latLng) {
    var topRight = sf.map.getProjection().fromLatLngToPoint(sf.map.getBounds().getNorthEast());
    var bottomLeft = sf.map.getProjection().fromLatLngToPoint(sf.map.getBounds().getSouthWest());
    var scale = Math.pow(2, sf.map.getZoom());
    var worldPoint = sf.map.getProjection().fromLatLngToPoint(_latLng);
    return new google.maps.Point((worldPoint.x - bottomLeft.x) * scale, (worldPoint.y - topRight.y) * scale);
}

function showMapManager2(_obj) {
    if (isFtth) {
        if ($('body').has('div#dvPopUp').length != 1) {
            var ofset = $(_obj).offset();
            //openIframePopupL('index.html', 'Legend Manager', 200, 341, ofset.left - 500, ofset.top);
            ajaxReq('Main/GetLegendManagerPartial', {}, true, function (resp) {
                if (resp != null) {
                    openPartialPopUp(resp, 'Legend Manager', 223, 341, ofset.left - 230, ofset.top + 30);
                }
            }, false);
        }
    }
    else {
        $(app.DE.divLegends).toggle();
    }
}

function openPartialPopUp(htmlContent, header, width, height, left, top) {
    $('#dvPopUp').remove();
    var popUpHmtl = '<div id="dvPopUp" class="popupMain"><div class="popupHeader">' + header +
        '<span class="tooltip" title="Close" ></span><i class="fa fa-close" style="position:relative;left:90px;cursor:pointer;"onclick="CloseDiv(\'#dvPopUp\');"></i>' +
        '</div><div><div id="dvPopUpContent">' + htmlContent + '</div>' +
        '</div></div>';
    $('body').append(popUpHmtl);

    if (width != 0) $('#dvPopUpContent').width(width);
    if (height != 0) $('#dvPopUpContent').height(height);
    //if (/legend/i.test(header)) $('#dvPopUpContent').css('overflow-y', 'auto');

    if (left && top) {
        $('#dvPopUp').css({ 'left': left, 'top': top }).show();
        //if (/legend/i.test(header))
        //    $('#dvPopUp').css({ 'left': '1625px', 'top': '66px' }).show();
        //else
        //    $('#dvPopUp').css({ 'left': left, 'top': top }).show();
    }
    else {
        var _w = $(window).width();
        var _h = $(window).height();

        var _l = (_w - width - 20) / 2;
        var _t = (_h - height - 45) / 2;

        if (_l < 0) _l = 0;
        if (_t < 0) _t = 0;

        $('#dvPopUp').css({ 'left': _l, 'top': _t }).show();
        //if (/legend/i.test(header))
        //    $('#dvPopUp').css({ 'left': '1625px', 'top': '66px' }).show();
        //else
        //    $('#dvPopUp').css({ 'left': _l, 'top': _t }).show();
    }
    $('#dvPopUp').draggable({ scroll: false, cancel: '.info_middle', containment: "window" });
}

function CloseDiv(selector) {
    $(selector).fadeOut(540);
    $('#dvPopUp').remove();
}

function showHideLegend(ctrl, cls) {
    if ($(ctrl).parent('div').find('i').hasClass('fa-plus-square-o')) {
        $(ctrl).parent('div').find('i').removeClass('fa-plus-square-o');
        $(ctrl).parent('div').find('i').addClass('fa-minus-square-o');
        $('.' + cls).slideDown("1000")
        $('div#dvPopUpContent').css({ 'overflow-y': 'auto', 'width': '240' });
    }
    else {
        $(ctrl).parent('div').find('i').removeClass('fa-minus-square-o');
        $(ctrl).parent('div').find('i').addClass('fa-plus-square-o');
        $('.' + cls).slideUp("1000")
        $('div#dvPopUpContent').css({ 'overflow-y': 'none', 'width': '223' });
    }
}

//$('#myModal').on('shown', function () {
//    $(document).off('focusin.modal');
//});
//$(document).on('shown.bs.modal', '#myModal', function () {
//    // alert("Hi");
//    $("#myModal").removeClass("modal");
//    $("#myModal").addClass("custom-modal");
//})

function CheckLayer(ctrl, lrTxt, checked, actAlso) {
    if (app.polylines.length) {
        for (var i = 0; i < app.polylines.length; i++) {
            app.polylines[i].setMap(null);
        }
    }
    if (checked) {
        //addArrayItm(APP.active_layers, lyr);
        var lrTbl = $(ctrl).attr('value');
        if (lrTbl != null && lrTbl != 'on') addArrayItm(app.lyrTbls, lrTbl);
        addArrayItm(app.lyrNames, lrTxt);
    }
    else {
        //removeArrayItm(APP.active_layers, lyr);
        var lrTbl = $(ctrl).attr('value');
        if (lrTbl != null && lrTbl != 'on')
          //  removeArrayItm(app.lyrTbls, lrTbl);
        removeArrayItm(app.lyrNames, lrTxt);
        $('#tblbatchLOS tbody').html('<tr><td colspan="10" valign=="center"><strong>No records found.</strong></td></tr>');
    }
    if (actAlso)
        getNEDetails(app.distanceWidget);
}

function revGeoCodeCntr(marker) {
    var pos = marker.getPosition();
    app.geocoder = new google.maps.Geocoder();
    app.geocoder.geocode({ 'latLng': pos }, function (results, status) {
        //if (status == google.maps.GeocoderStatus.OK) {
        // if (results[1])
        // $('.modal-title').text(results[1].formatted_address);
        // }
    });
}

$("#ftth-sel-option").on("change", function () {
    var value = $(this).val();

    if (value == 1) {
        $("#txtEntitySearch").show();
        $("#txtGoogleSearch").hide();
        $("#txtEntitySearch").val('');
    }
    else {
        $("#txtEntitySearch").hide();
        $("#txtGoogleSearch").show();
        $("#txtGoogleSearch").val('');
        // GetFtthInfo();
    }
})

$('#ModalPopUp_hist').on('hidden.bs.modal', function () {
    $(app.DE.iconPastFeasFtth).removeClass('activeicon');
});

$(document).on("click", ".wireless", function () {
    var checkbox = $(this);
    if (checkbox.is(':checked')) {
        $('.footerBox').slideDown();
        $('#floating-panel-left').fadeOut(300);
        $('#open-close').fadeOut(300);
    } else {
        $('.footerBox').slideUp();
        $('#floating-panel-left').fadeIn(300);
        $('#open-close').fadeIn(300);
    }
});



//END FTTH