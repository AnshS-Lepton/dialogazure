
var BackbonePlanning = function () {
    var app = this;
    this.autoplanningPlanId = 0;
    this.endMarker = undefined;
    this.startMarker = undefined;
    this.NetworkStartPoint = null;
    this.NetworkEndPoint = null;
    this.TempNetworkEndPoint = null;
    this.directionsRenderer = null;
    this.directionsSiteRenderer = null;
    this.startLatLng = {};
    this.endLatLng = {};
    this.MarkerList = [];
    this.plan_mode = '';
    this.Network_Path = 'google';
    this.cable_calculated_length = 0.0;
    this.temp_gemo = '';
    this.StartTmpLine = null;
    this.StartSiteTmpLine = null;
    this.sitePointMarker = null;
    this.EndTmpLine = null;
    this.EndSiteTmpLine = null;
    this.middleMarkers = [];
    this.AllPlanningPaths = [];
    this.AllDistances = [];
    this.AllPathResponses = [];
    this.record_count = 0;
    this.minDistance = 0;
    this.selectedPlanningPath = [];
    this.isUserChangingRoute = false;
    this.sortedPlan = [];
    this.NP = {
        "closeBackBonePlanTool": "#closeBackBonePlanTool",
        "divBackBonePlanTool": "#divBackBonePlanTool",
        "dvAutoBackBonePlanData": '#dvAutoBackBonePlanData',
        //"OpenCloseExternalView": '#closeExternalData,#openExternalData',
        "OpenCloseBackBonePlanningView": '#closeBackBonePlanningData, #openBackBonePlanningData',
        "end_type": { 'START': 'start', 'END': 'end' },
    }
    this.initApp = function () {
        this.bindEvents();
    }
    this.bindEvents = function () {
        $(app.NP.OpenCloseNetworkPlanningView).unbind('click');
        $(app.NP.OpenCloseNetworkPlanningView).on("click", function () {
            $(app.NP.dvAutoBackBonePlanData).animate({
                width: "toggle"
            }, function () {
                if ($(this).css('display') === 'none') {
                    $("#closeBackBonePlanningData").hide();
                    $("#openBackBonePlanningData").show();
                }
                else {
                    $("#closeBackBonePlanningData").show();
                    $("#openBackBonePlanningData").hide();
                }
            });
        });


        $(app.NP.closeBackBonePlanTool).on("click", function () {

            si.ViewBackboneplan();
        });
    }

    this.hideAllNetworkFile = function () {

        if (si.startMarker)
            si.startMarker.setMap(null);
        if (si.endMarker)
            si.endMarker.setMap(null);
        if (si.gMapObj.infoEntity != undefined) { si.gMapObj.infoEntity.setMap(null); }
        si.clearMarkers();
        removeOldMarkersWithRemoveActive();
        if (si.gMapObj.entitySrchObj)
            si.gMapObj.entitySrchObj.setMap(null);
        si.autobackboneplanid = 0;
        app.autoplanningPlanId = 0;
        if (si.distanceWidget) {
            si.distanceWidget.set("map", null);
            si.distanceWidget = null;
        }
        if (si.fadeMap) {
            si.fadeMap.setMap(null);
        }
        if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
        if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }
    }

    this.ResetPlanForm = function () {

        app.temp_gemo = '';
        $('#geometry').val('');
        backbonedata.NetworkStartPoint = null;
        backbonedata.NetworkEndPoint = null;
        $('#manholeSpan').val('');
        $('#poleSpan').val('');
        $('#cabledrumLength').val('');
        $('#cablelength').val('');
        $('#startpoint').val('');
        $('#endpoint').val('');
        $('#is_create_trench').prop('checked', false);
        $('#is_create_duct').prop('checked', false);
        $("#startpointmap").removeClass('activemarker');
        $("#endpointmap").removeClass('activemarker');
        $("#btnBomCable").attr("disabled", false);
        $("#btnNearestSite").attr("disabled", false);
        backbonedata.hideAllNetworkFile();
        app.ResetBomDetails();
        si.gMapObj.infoEntity = null;
        si.gMapObj.libPath = null;
        si.point2pointgeom = [];
        if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
        if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }
        $('#endPointErrorMsg').text('');
        $('#startPointErrorMsg').text('');
        if (si.fadeMap) {
            si.fadeMap.setMap(null);
        }
        $('#sproutFiberDropdown').val('').trigger("chosen:updated");
        $('#backboneFiberDropdown').val('').trigger("chosen:updated");
        $('#sproutThreshold').val('');
        $('#sproutlength').val('');
        $('#planbuffer').val('');
        $('#siteDropdownToggle').val('--Select Site--');        
        $('#plan_name').val('');
        $('#total_cable_length').val('');
        $('#loopLength').val('');
        $('#loopSpan').val('');
        if (app.routePolyline) {
            app.routePolyline.setMap(null);
            app.routePolyline = null;
        }
        if (app.directionsSiteRenderer) {
            app.directionsSiteRenderer.setMap(null);
        }
        if (app.directionsRenderer) {
            app.directionsRenderer.setMap(null);
            
        }  
        if (app.StartSiteTmpLine) {
            app.StartSiteTmpLine.setMap(null);
            app.StartSiteTmpLine = null;
        }
        if (app.EndSiteTmpLine) {
            app.EndSiteTmpLine.setMap(null);
            app.EndSiteTmpLine = null;
        }
        if (app.sitePointMarker) {
            app.sitePointMarker.setMap(null);
            app.sitePointMarker = null;
        }
        $('#closeModalPopup').trigger("click");
        $('#total_sprout_length').val('');
        $('#sproutlength').val('');
    }


    this.GemoValidation = function (geomArr) {
        if (geomArr.length == 0) {
            alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_300);
            return false;
        }

        if (geomArr.length <= 1) {
            alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_299);
            return false;
        }
        return true;
    }

    this.ResetBomDetails = function () {

        $("#BomBoqDetails").empty();
        $("#btnNearestSite").attr("disabled", true);
        $("#ManageLoop").attr("disabled", true);
        app.CheckLoopRequired();
        if (si.fadeMap) {
            si.fadeMap.setMap(null);
        }
    }

    this.ViewBackbonePlanningData = function () {

        if (app.autoplanningplanid > 0 || si.autobackboneplanid > 0) {
            si.autobackboneplanid = 0;
            app.autoplanningplanid = 0;
            si.LoadLayersOnMap();
        }
        $("#planHistotry").html("");
        ajaxReq('BackBonePlan/GetBackbonePlanHistoryData', {}, true, function (resp) {
            $("#planHistotry").html(resp);
            $("#planHistotry").css('background-image', 'none');
        }, false, false);
    }

    this.getNearestSite = function () {     
        let buffer = parseFloat($('#planbuffer').val());
        let geom = $('#geometry').val();            
        let planId = $('#plan_id').val();            
        if (!isNaN(buffer)) {
            popup.LoadModalDialog('PARENT', 'BackBonePlan/GetBackboneNearestSiteList', { geom: geom, buffer: buffer, planId: planId }, "Sprout Site", 'modal-xl');           
        }
        if (!si.startMarker.getMap()) {
            si.startMarker.setMap(si.map);
        }

        if (!si.endMarker.getMap()) {
            si.endMarker.setMap(si.map);
        }
    }
    this.PlanningBufferPoint = function () {
        let buffer = parseFloat($('#planbuffer').val());
        let geom = $('#geometry').val();              
        if (!isNaN(buffer)) {
            ajaxReq('BackBonePlan/GetBackboneNearestSiteBuffer', { geom: geom, buffer: buffer }, true, function (resp) {
                if (resp.status === "OK") {
                   
                   showPolygonBufferGeometryOnMap(resp.result.buffer_geometry);
                }
            }, false, false);
        }
    };

    this.createBackbonePlanNetwork = function (is_create_plan) {
       
        let cableLength = $('#cablelength').val();
        let sproutlength = $('#sproutlength').val();
        let formData = $('form').serialize();

        // Remove any existing cable_length from the serialized string
        formData = formData.replace(/&?(cable_length|sprout_route_length)=[^&]*/g, '');

        // Append the updated cable_length
        formData += '&cable_length=' + encodeURIComponent(cableLength);
        formData += '&sprout_route_length=' + encodeURIComponent(sproutlength);
        formData += '&create_plan=' + encodeURIComponent(is_create_plan);
        ajaxReq('BackBonePlan/SaveBackboneProcess', formData, true, function (resp) {
            $('#closeModalPopup').trigger("click");
            app.autoPlanningShowNetworkLayer(resp);
            app.ResetPlanForm();          
        }, false, true, false);
    }


    this.autoPlanningShowNetworkLayer = function (resp) {

        if (resp.objPM.status == "True") {
            alert(resp.objPM.message);
            app.autoplanningPlanId = resp.v_plan_id;
            //$('.lyrRefresh').trigger("click");
            backbonedata.ShowBackbonePlanEntityOnMap(resp.plan_id);
            app.hideAllNetworkFile();
            $("#btnNearestSite").attr("disabled", true);
        }
        else {
            alert(resp.objPM.message);
        }
    }
    this.ShowBackbonePlanEntityOnMap = function (planId,status = "Completed") {
        $('.pull-right').find('.activemarker').removeClass('activemarker')
        $('.glyphicon glyphicon-eye-open').removeClass('activemarker');
        $('i[id^="dvHighlight_"]').removeClass('activemarker'); // Remove from all
        if (status != "Completed") {
            alert('The planning process is currently in progress');
            return false;
        }
        ajaxReq('BackBonePlan/GetBackboneForMap', { plan_id: planId }, true, function (resp) {
            if (resp.status.toLowerCase() == "ok") {
               
                $('#dvHighlight' + '_' + planId).addClass('activemarker');
                app.autobackboneplanid = planId;
                si.autobackboneplanid = planId;
                // si.LoadLayersOnMap();
                si.resetTreeLayer();
                //si.map.setZoom(18);
                var startlatlong = resp.result.start_point.split(',');
                //  si.map.setCenter(new google.maps.LatLng(startlatlong[0], startlatlong[1]));
                $(si.DE.chkShowLabelOnMap).prop("checked", resp.result.has_label);

                var workSpaceLyrs = resp.result.layer_id.split(",");

                if (workSpaceLyrs.length > 0) {
                    for (var i = 0; i < workSpaceLyrs.length; i++) {
                        $('#chk_nLyr_' + workSpaceLyrs[i]).not(":disabled").prop('checked', true);
                        $('#chk_netP_' + workSpaceLyrs[i]).not(":disabled").prop('checked', true);
                        $('#chk_netA_' + workSpaceLyrs[i]).not(":disabled").prop('checked', true);
                        $('#chk_netD_' + workSpaceLyrs[i]).not(":disabled").prop('checked', true);
                    }
                }

                var networkRegion = resp.result.region_ids.split(",");

                if (networkRegion.length > 0) {
                    for (var i = 0; i < networkRegion.length; i++) {

                        $('#chk_rLyr_' + networkRegion[i]).not(":disabled").prop('checked', true);

                    }
                }

                var networkProvinces = resp.result.province_ids.split(",");

                if (networkProvinces.length > 0) {
                    for (var i = 0; i < networkProvinces.length; i++) {
                        $('#chk_pLyr_' + networkProvinces[i]).not(":disabled").prop('checked', true);
                    }
                }

                app.fitElementOnMap(resp.result.start_point, resp.result.end_point);
                si.map.setZoom(si.map.getZoom() - 1);
                si.LoadLayersOnMap();


            }
        }, true, true);
    }

    this.HighlightPointEntityOnMap = function (mrkrLatlng, imageUrl) {

        if (si.gMapObj.entitySrchObj)
            si.gMapObj.entitySrchObj.setMap(null);
        si.gMapObj.entitySrchObj = app.createAutoMarker(mrkrLatlng, 'Content/images/dwnArrow.png', 'end');
        si.gMapObj.entitySrchObj.setAnimation(google.maps.Animation.BOUNCE);
        si.gMapObj.entitySrchObj.setDraggable(false);
        si.gMapObj.entitySrchObj.setMap(si.map);
    }

    this.fitElementOnMap = function (startlatlong, endlatlong) {
        var startlatlong = startlatlong.split(',');
        var endlatlong = endlatlong.split(',');

        var southWest = new google.maps.LatLng(parseFloat(startlatlong[0]), parseFloat(startlatlong[1]));
        var northEast = new google.maps.LatLng(parseFloat(endlatlong[0]), parseFloat(endlatlong[1]));
        //var bounds = new google.maps.LatLngBounds(southWest, northEast);
        var bounds = new google.maps.LatLngBounds();
        bounds.extend(southWest);
        bounds.extend(northEast);
        si.map.fitBounds(bounds);

    }

    this.deleteBackbonePlan = function (planId) {
        showConfirm(MultilingualKey.SI_OSP_GBL_GBL_FRM_157, function () {
            ajaxReq('BackBonePlan/DeleteBackbonePlanByPlanId', { plan_id: planId }, true, function (resp) {
                if (resp.msg.toLowerCase() == "ok") {
                    alert(resp.strReturn);
                    $("#dv_kml_" + planId).remove();
                    $('.lyrRefresh').trigger("click");

                }
                else {
                    alert(resp.strReturn);
                }
            }, false, true, false);
        });
    }
    this.deleteSproutSitePlan = function (planId, networkId) {

        showConfirm("Do you really want to delete this sprout Network?", function () {
            debugger;

            ajaxReq('BackBonePlan/DeleteSproutPlanById',
                { planId: planId, networkId: networkId }, true,
                function (resp) {

                    // locate the latest checkbox row dynamically
                    let $row = $("input.row-checkbox[value='" + networkId + "']").closest("tr");

                    if (resp.msg.toLowerCase() == "ok") {
                        alert(resp.strReturn);
                        $('.lyrRefresh').trigger("click");
                        $row.remove();
                    }
                    else {
                        alert(resp.strReturn);
                        // revert checkbox
                        $row.find(".row-checkbox").prop("checked", true);
                    }

                }, false, true, false);
        },
            function () {
                // CANCEL CLICKED → restore checked state
                $("input.row-checkbox[value='" + networkId + "']").prop("checked", true);
            }
        );
    }

    this.convertToAsbuiltNetwork = function (planId) {
        debugger;
        ajaxReq('BackBonePlan/ConvertToAsbuiltNetwork', { planId: planId }, true, function (resp) {
            if (resp.status.toLowerCase() == "ok") {
                alert(resp.message);
                $('.lyrRefresh').trigger("click");
            }
            else {
                alert(resp.message);
            }
        }, false, true, false);
    }
    this.convertToPlannedNetwork = function (planId) {
        debugger;
        ajaxReq('BackBonePlan/ConvertToPlannedNetwork', { planId: planId }, true, function (resp) {
            if (resp.status.toLowerCase() == "ok") {
                alert(resp.message);
                $('.lyrRefresh').trigger("click");
            }
            else {
                alert(resp.message);
            }
        }, false, true, false);
    }

    this.createAutoMarker = function (mrkrLatlng, imageUrl, label, draggable = true) {

        var gmarkernew = new google.maps.Marker({
            position: mrkrLatlng,
            icon: imageUrl,
            draggable: ((label == 'start') ? true : (app.plan_mode == 'auto' || draggable == false) ? false : true)
        });

        return gmarkernew;
    }

    this.downloadBackboneBomExcelReport = function (planId) {
        window.location = appRoot + 'Report/ExportBackbonePlanBOMBOQReport?plan_id=' + planId;
    }
    this.downloadBackboneBomKMLReport = function (planId) {
        window.location = appRoot + 'Report/ExportKMLBackbonePlanBOMBOQReport?plan_id=' + planId;
    }
    this.downloadBackboneSiteHistoryReport = function (planId) {
        window.location = appRoot + 'Report/ExportSiteHistoryReport?plan_id=' + planId;
    }
    this.getSiteHistory = function (planId) {
        popup.LoadModalDialog('PARENT', 'BackBonePlan/GetBackboneSiteHistory', { planId: planId}, "Site History", 'modal-lg');    
    }

    this.NetworkPlanning = function (ismappicker, destination_point) {
        if (si.fadeMap) {
            si.fadeMap.setMap(null);
        }
        if (ismappicker) {
            if (destination_point === 'start') {
                $("#startpointmap").addClass('activemarker activeicon');

                google.maps.event.addListener(si.map, 'click', function (e) {
                    app.startLatLng = e.latLng;
                    var startLatLog = e.latLng.lat().toFixed(6) + "," + e.latLng.lng().toFixed(6);
                    app.NetworkStartPoint = e.latLng.lat() + "," + e.latLng.lng();
                    $('#startpoint').val(startLatLog);
                    $('#start_point').val(startLatLog);

                    app.P2PNetworkManual(destination_point);
                    app.ResetBomDetails();

                });
            }

            if (destination_point === 'end') {
                $("#endpointmap").addClass('activemarker activeicon');

                google.maps.event.addListener(si.map, 'click', function (f) {
                    app.endLatLng = f.latLng;
                    var endLatLog = f.latLng.lat().toFixed(6) + "," + f.latLng.lng().toFixed(6);
                    app.NetworkEndPoint = f.latLng.lat() + "," + f.latLng.lng();
                    $('#endpoint').val(endLatLog);
                    $('#end_point').val(endLatLog);

                    app.P2PNetworkManual(destination_point);
                    app.ResetBomDetails();
                });
            }
        }

        return true;
    };



    this.createStartMarker = function () {

        if (app.NetworkStartPoint != null) {

            var startlat = parseFloat(app.NetworkStartPoint.split(',')[0]);
            var startlong = parseFloat(app.NetworkStartPoint.split(',')[1]);
            startLatlng = { lat: startlat, lng: startlong };

            if (si.startMarker)
                si.startMarker.setMap(null);
            si.startMarker = app.createAutoMarker(startLatlng, 'Content/images/Actual_Start.png', app.NP.end_type.START);
            $('#startpoint').val(startlat.toFixed(6) + ',' + startlong.toFixed(6));
            si.startMarker.addListener('drag', function (startLatlng) {

                app.NetworkStartPoint = startLatlng.latLng.lat() + "," + startLatlng.latLng.lng();              
                app.fillnetworkPlanningMarker(startLatlng, 'start');
                app.ResetBomDetails();
            });

            si.startMarker.setMap(si.map);
            app.MarkerList.push(si.startMarker);
        }
    }

    this.createEndMarker = function () {


        if (app.NetworkEndPoint != null) {
            var endlat = parseFloat(app.NetworkEndPoint.split(',')[0]);
            var endlong = parseFloat(app.NetworkEndPoint.split(',')[1]);
            endLatlng = { lat: endlat, lng: endlong };

            if (si.endMarker)
                si.endMarker.setMap(null);
            si.endMarker = app.createAutoMarker(endLatlng, 'content/images/End.png', app.NP.end_type.END);
            $('#endpoint').val(endlat.toFixed(6) + ',' + endlong.toFixed(6));
            si.endMarker.addListener('drag', function (endLatlng) {
                app.NetworkEndPoint = endLatlng.latLng.lat() + "," + endLatlng.latLng.lng();
                app.fillnetworkPlanningMarker(endLatlng, 'end');             
                app.ResetBomDetails();
            });
            si.endMarker.setMap(si.map);

            app.MarkerList.push(si.endMarker);
        }
    }

    this.P2PNetworkManual = function (end) {       
        if (si.gMapObj.RulerLine) {
            si.removeEventListnrs('click');
            si.gMapObj.RulerLine.setMap(null);
            si.gMapObj.RulerLine = null;
            $('#lengthAreaDiv').empty();
        }

        ClearRuler();
        if (directionsDisplay) {
            directionsDisplay.setMap(null);
        }

        if (si.startMarker == undefined && si.endMarker == undefined) {
            si.clearMarkers();
            removeOldMarkersWithRemoveActive();
        }

        if (end == app.NP.end_type.START && app.NetworkStartPoint != null) {
            app.createStartMarker();
        }

        if (end == app.NP.end_type.END && app.NetworkEndPoint != null) {
            app.createEndMarker();

        }
        if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
            app.createCableBetweenMakers();          

        }
    }

    this.createCableBetweenMakers = function () {
        app.AllPlanningPaths = [];
        app.AllDistances = [];
        app.minDistance = 0;
        app.AllPathResponses = [];

        // 1. Reset existing directionsRenderer
        if (!app.directionsRenderer) {
            app.directionsRenderer = new google.maps.DirectionsRenderer({
                map: si.map,
                draggable: true,
                suppressMarkers: true,
                polylineOptions: {
                    strokeColor: '#db5333',
                    strokeOpacity: 1,
                    strokeWeight: 4,
                    zIndex: 10
                }
            });

            google.maps.event.addListener(app.directionsRenderer, 'directions_changed', function () {
                const dir = app.directionsRenderer.getDirections();
                if (!dir || dir.status !== 'OK') return;

                const path = google.maps.geometry.encoding.decodePath(dir.routes[0].overview_polyline);
                app.ShowAutoPlanLineLength(path);
                const buf = $('#planbuffer').val();
                if (buf && +buf > 0) app.PlanningBufferPoint();
                //$('#closeModalPopup').trigger("click");

                if (!si.startMarker.getMap()) si.startMarker.setMap(si.map);
                if (!si.endMarker.getMap()) si.endMarker.setMap(si.map);
            });
        } else {
            // Clear previous route
            // Clear previous route safely
            app.directionsRenderer.setMap(null);
            app.directionsRenderer.setDirections(null);
            app.directionsRenderer.setMap(si.map);
        }

        if (!app.NetworkStartPoint || !app.NetworkEndPoint) return;

        // 2. Parse input coordinates
        app.startLatLng = {
            lat: parseFloat(app.NetworkStartPoint.split(',')[0]),
            lng: parseFloat(app.NetworkStartPoint.split(',')[1])
        };
        app.endLatLng = {
            lat: parseFloat(app.NetworkEndPoint.split(',')[0]),
            lng: parseFloat(app.NetworkEndPoint.split(',')[1])
        };

        // 3. Remove previous temporary lines
        if (backbonedata.StartTmpLine) {
            backbonedata.StartTmpLine.setMap(null);
            backbonedata.StartTmpLine = null;
        }
        if (backbonedata.EndTmpLine) {
            backbonedata.EndTmpLine.setMap(null);
            backbonedata.EndTmpLine = null;
        }

        // 4. Create forward request
        const forwardRequest = {
            origin: app.startLatLng,
            destination: app.endLatLng,
            travelMode: google.maps.TravelMode.DRIVING,
            provideRouteAlternatives: true
        };

        directionsService.route(forwardRequest, function (response, status) {
            console.log("Directions API status:", status, response);

            if (status === google.maps.DirectionsStatus.OK) {
                const paths = response.routes;
                const firstRoute = paths[0];
                app.AllPathResponses.push(response);

                app.directionsRenderer.setOptions({ suppressMarkers: true });
                app.directionsRenderer.setDirections(response);
                app.directionsRenderer.setMap(si.map);

                const latLngArr = google.maps.geometry.encoding.decodePath(firstRoute.overview_polyline);
                const endIndex = latLngArr.length - 1;

                const startPoint = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
                const startPointLineArr = [latLngArr[0], startPoint];
                app.StartTmpLine = app.createAutoPlanLine(startPointLineArr, false, false);
                app.StartTmpLine.setMap(si.map);

                const EndPoint = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
                const EndPointLineArr = [latLngArr[endIndex], EndPoint];
                app.EndTmpLine = app.createAutoPlanLine(EndPointLineArr, false, false);
                app.EndTmpLine.setMap(si.map);

                app.createDirectionMarker(startPoint, EndPoint);
                latLngArr.splice(0, 0, startPoint);
                latLngArr.push(EndPoint);

                console.log('createCableBetweenMakers - Route drawn');
                app.ShowAutoPlanLineLength(latLngArr);

                const buf = $('#planbuffer').val();
                if (buf && +buf > 0) app.PlanningBufferPoint();
               // $('#closeModalPopup').trigger("click");

                if (!si.startMarker.getMap()) si.startMarker.setMap(si.map);
                if (!si.endMarker.getMap()) si.endMarker.setMap(si.map);

            } else {
                console.warn("No driving route found, drawing fallback line instead.");
                // Draw a straight polyline between start and end
                new google.maps.Polyline({
                    path: [app.startLatLng, app.endLatLng],
                    map: si.map,
                    strokeColor: "#db5333",
                    strokeOpacity: 1,
                    strokeWeight: 4,
                    zIndex: 10
                });
            }
        });
    };

    this.fillnetworkPlanningMarker = function (LatLong, end) {

        var lat;
        var lng;

        if (end == app.NP.end_type.START) {
            app.startLatLng = LatLong.latLng;
            lat = LatLong.latLng.lat();
            lng = LatLong.latLng.lng();
            $('#startpoint').val(lat.toFixed(6) + ',' + lng.toFixed(6));
            $('#start_point').val(lat.toFixed(6) + ',' + lng.toFixed(6));
            if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
                if (si.Network_Path == "manually" || si.gMapObj.infoEntity != undefined && si.gMapObj.infoEntity != null) {
                    app.CreateplanCable(app.startLatLng, end);
                }
            }

        }
        else if (end == app.NP.end_type.END && si.Network_Path == "manually" || si.gMapObj.infoEntity != undefined && si.gMapObj.infoEntity != null) {
            app.endLatLng = LatLong.latLng;
            lat = LatLong.latLng.lat();
            lng = LatLong.latLng.lng();
            $('#endpoint').val(lat.toFixed(6) + ',' + lng.toFixed(6));
            $('#end_point').val(lat.toFixed(6) + ',' + lng.toFixed(6));
            if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
                if (si.Network_Path == "manually" || si.gMapObj.infoEntity != undefined && si.gMapObj.infoEntity != null) {
                    app.CreateplanCable(app.endLatLng, end);
                }
            }

        }
    }


    this.createDirectionMarker = function (NetworkStartPoint, NetworkEndPoint) {
        app.MarkerList = [];
  
        var startlat = parseFloat(NetworkStartPoint.lat());
        var startlong = parseFloat(NetworkStartPoint.lng());
        var startLatlng = { lat: startlat, lng: startlong };

        if (si.startMarker)
            si.startMarker.setMap(null);
        si.startMarker = app.createAutoMarker(startLatlng, 'Content/images/Actual_Start.png', app.NP.end_type.START);
        $('#startpoint').val(startlat.toFixed(6) + ',' + startlong.toFixed(6));
        $('#start_point').val(startlat.toFixed(6) + ',' + startlong.toFixed(6));

        si.startMarker.addListener('dragend', function (event) {
            app.NetworkStartPoint = event.latLng.lat() + "," + event.latLng.lng();
                        app.P2PNetworkManual('start');
                        app.ResetBomDetails();
        });

        si.startMarker.setMap(si.map);
        app.MarkerList.push(si.startMarker);
       
        var endlat = parseFloat(NetworkEndPoint.lat());
        var endlong = parseFloat(NetworkEndPoint.lng());
        var endLatlng = { lat: endlat, lng: endlong };

        if (si.endMarker)
            si.endMarker.setMap(null);
        si.endMarker = app.createAutoMarker(endLatlng, 'content/images/End.png', app.NP.end_type.END);
        $('#endpoint').val(endlat.toFixed(6) + ',' + endlong.toFixed(6));
        $('#end_point').val(endlat.toFixed(6) + ',' + endlong.toFixed(6));

        si.endMarker.addListener('dragend', function (event) {
            app.NetworkEndPoint = event.latLng.lat() + "," + event.latLng.lng();
            app.P2PNetworkManual('end');
            app.ResetBomDetails();
        });

        backbonedata.TempNetworkEndPoint = endLatlng.lat + "," + endLatlng.lng;
        si.endMarker.setMap(si.map);
        app.MarkerList.push(si.endMarker);
    };

    this.CreateplanCable = function (LatLong, end) {

        var startIndex = 0; var endIndex = 0;

        var latLngArr;

        if (si.gMapObj.infoEntity != undefined) {
            var latLngArr = si.gMapObj.libPath;
        }
        else {
            var latLngArr = LatLong;
        }

        if (end == app.NP.end_type.START) {
            var startLatLng = app.startLatLng.lat() + ' ' + app.startLatLng.lng();
            oldcablelatlng = latLngArr[0].lat() + ' ' + latLngArr[0].lng();
            if (oldcablelatlng != startLatLng) {
                startflag = true;
                latLngArr.splice(startIndex, 1, app.startLatLng);
            }
        }
        else if (end == app.NP.end_type.END) {
            endIndex = latLngArr.length - 1;
            var endLatLng = app.endLatLng.lat() + ' ' + app.endLatLng.lng();
            oldcablelatlng = latLngArr[endIndex].lat() + ' ' + latLngArr[endIndex].lng();
            if (oldcablelatlng != endLatLng) {
                endflag = true;
                latLngArr.splice(endIndex, endIndex, app.endLatLng);

            }
        }
        else {

            var startLatLng = app.startLatLng.lat + ' ' + app.startLatLng.lng;
            oldcablelatlng = latLngArr[0].lat() + ' ' + latLngArr[0].lng();
            if (oldcablelatlng != startLatLng) {
                startflag = true;
                latLngArr.splice(startIndex, 1, new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng));
            }

            endIndex = latLngArr.length - 1;
            var endLatLng = app.endLatLng.lat + ' ' + app.endLatLng.lng;
            oldcablelatlng = latLngArr[endIndex].lat() + ' ' + latLngArr[endIndex].lng();
            if (oldcablelatlng != endLatLng) {
                endflag = true;
                latLngArr.splice(endIndex, endIndex, new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng));
            }
        }
        app.networkPlanningcable(latLngArr);
        // ShowAutoPlanLineLength(latLngArr);

    }

    this.networkPlanningcable = function (latLngArr, LineEdiTable = true) {
        app.CheckCableCreated();
        si.gMapObj.lineflag = true;
        si.gMapObj.lineEditflag = true;
        si.gMapObj.infoEntity = app.createAutoPlanLine(latLngArr, LineEdiTable);
        si.gMapObj.infoEntity.setMap(si.map);
        si.gMapObj.libPath = latLngArr;
        const path = si.gMapObj.infoEntity.getPath();
        const syncPath = () => {
            const updatedPath = path.getArray().map(p => new google.maps.LatLng(p.lat(), p.lng()));
            si.gMapObj.libPath = updatedPath;
            ShowAutoPlanLineLength(updatedPath);
        };

        path.addListener("set_at", syncPath);
        path.addListener("insert_at", syncPath);
        path.addListener("remove_at", syncPath);

        app.initialClickForAutoEditLine();
    };


    this.CheckCableCreated = function () {
        if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
        if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }
        if (directionsDisplay) {
            directionsDisplay.setMap(null);
        }

        if (si.gMapObj.infoEntity != undefined && si.gMapObj.infoEntity != null) { si.gMapObj.infoEntity.setMap(null); si.gMapObj.infoEntity = null; }
    }


    this.createAutoPlanLine = function (_path, LineEdiTable, Is_DashedLine = false, strokeWdth) {

        strokeWdth = (strokeWdth == undefined || strokeWdth == null || strokeWdth == '' || strokeWdth == 0) ? 2 : strokeWdth * 10;
        var tmpLine;
        if (Is_DashedLine == false) {
            tmpLine = new google.maps.Polyline({
                strokeColor: '#db5333',
                strokeOpacity: 1,
                strokeWeight: 4,
                path: _path,
                editable: LineEdiTable
            });
        }
        else {
            const lineSymbol = {
                path: "M 0,-1 0,1",
                strokeOpacity: 1,
                scale: 2,
            };

            strokeWdth = (strokeWdth == undefined || strokeWdth == null || strokeWdth == '' || strokeWdth == 0) ? 2 : strokeWdth * 10;
            tmpLine = new google.maps.Polyline({
                strokeColor: '#db5333',
                strokeOpacity: 0,
                //strokeWeight: 2,
                path: _path,
                icons: [{
                    icon: lineSymbol,
                    offset: '0',
                    repeat: '10px'
                }],
                editable: LineEdiTable
            });
        }


        return tmpLine;
    }

    this.initialClickForAutoEditLine = function () {
   
        if (!si.gMapObj.infoEntity || !si.gMapObj.infoEntity.getPath) return;
        google.maps.event.addListener(si.gMapObj.infoEntity, 'rightclick', function (e) {

            if (e.vertex == null) return;
            const path = si.gMapObj.infoEntity.getPath();
            const first = path.getAt(0), last = path.getAt(path.getLength() - 1);
            const same = (a, b) => a.lat() === b.lat() && a.lng() === b.lng();
            if (same(e.latLng, first) || same(e.latLng, last)) {
                alert("Start / end point cannot be removed.");
                return;
            }
            si.removeNode(si.gMapObj.infoEntity, e.vertex);
            app.ShowAutoPlanLineLength();
            const buf = $('#planbuffer').val();
            if (buf && +buf > 0) app.PlanningBufferPoint();
        });

        si.gMapObj.map = si.map;

        if (!app.directionsRenderer) {

            app.directionsRenderer = new google.maps.DirectionsRenderer({
                map: si.gMapObj.map,
                draggable: true,
                suppressMarkers: true,
                polylineOptions: { strokeColor: '#db5333', strokeWeight: 1 }
            });

            // Listen for drag or waypoint change
            google.maps.event.addListener(app.directionsRenderer, 'directions_changed', () => {
                const dir = app.directionsRenderer.getDirections();
                if (!dir || dir.status !== 'OK') return;

                const path = google.maps.geometry.encoding.decodePath(dir.routes[0].overview_polyline);
                app.ShowAutoPlanLineLength(path);
                //backbonedata.hideAllNetworkFile?.();
                let isBuffer = $('#planbuffer').val();
                if (isBuffer.trim() !== '' && isBuffer > 0) {
                    app.PlanningBufferPoint();
                }
            });
        }
        const pathObj = si.gMapObj.infoEntity.getPath();
        let debounce;

        const pathChanged = () => {
            clearTimeout(debounce);
            debounce = setTimeout(() => {
                const path = si.gMapObj.infoEntity.getPath();
                si.gMapObj.libPath = path.getArray();
                app.deleteAllMiddleMarker?.();
                if (app.routePolyline) {
                    app.routePolyline.setMap(null);
                    app.routePolyline = null;
                }
              

                app.fetchRoutesFromPolyline?.(path, app.directionsRenderer, false);
            }, 300);
        };

        google.maps.event.addListener(pathObj, 'insert_at', pathChanged);
        google.maps.event.addListener(pathObj, 'set_at', pathChanged);
        google.maps.event.addListener(pathObj, 'remove_at', pathChanged);
    };

    this.setAllMiddleMarkerOnMap = function (IsMiddleMarkerCreated = true) {
        for (let i = 0; i < app.middleMarkers.length; i++) {
            if (IsMiddleMarkerCreated) {
                app.middleMarkers[i].setMap(si.map);
            }
            else {
                app.middleMarkers[i].setMap(null);
            }
        }
    }

    this.deleteAllMiddleMarker = function () {

        if (app.middleMarkers != []) {
            app.setAllMiddleMarkerOnMap(false);
            app.middleMarkers = [];
        }
    }

    this.planBomAndBOQ = function (resp, totalSproutLength = 0) {     
        $('#plan_id').val(resp.result.plan_id);
        let planId = $('#plan_id').val();
        $('#sproutlength').val(totalSproutLength);
        let serialized = $('form').serialize();
        let uniqueSerialized = removeDuplicateParams(serialized);
        ajaxReq('BackBonePlan/GetBomBOQData', uniqueSerialized, true, function (resp) {
            $("#BomBoqDetails").html(resp);
            $('#btnNearestSite').prop('disabled', false);
            $("#ManageLoop").attr("disabled", false);
             app.CheckLoopRequired();
          //  app.drawGeoJsonLinesOnMap(planId);
            si.startMarker.setOptions({
                draggable: false,
                clickable: false, 
            });
            si.endMarker.setOptions({
                draggable: false,
                clickable: false, 
            }); 
        }, false, true, false);
    }

     removeDuplicateParams = function (serialized) {
        const params = new URLSearchParams(serialized);
        const unique = new Map();

        // Iterate in order, but only keep the first occurrence
        for (const [key, value] of params.entries()) {
            if (!unique.has(key)) {
                unique.set(key, value);
            }
        }

        // Build back the query string
        return Array.from(unique)
            .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
            .join('&');
    }

    this.loopValidation = function () {       
        var backboneCableLength = parseFloat($('#cablelength').val());
        var cableDrumLength = parseFloat($('#cabledrumLength').val());
        var loop_length = parseFloat($('#loopLength').val());
        var loop_span = parseFloat($('#loopSpan').val());
        var threshold = parseFloat($('#sproutThreshold').val());
        var pole_distance = parseFloat($('#poleSpan').val());
        var manhole_distance = parseFloat($('#manholeSpan').val());
        var is_loop_required = $("input[name='is_loop_required']:checked").val();
      
        if ((isNaN(loop_length) || loop_length === 0) && is_loop_required == "True") {
            $('#loopLength').addClass('form-control input-validation-error');
            return false;
        }
        if ((isNaN(loop_span) || loop_span === 0) && is_loop_required == "True") {
            $('#loopSpan').addClass('form-control input-validation-error');
            return false;
        }
        if (backboneCableLength < cableDrumLength) {
            $('#cabledrumLength').addClass('form-control input-validation-error');
            alert("Cable Drum length cannot be greater than Backbone Route Length!");
            return false;
        }       
        if (loop_length >= cableDrumLength) {
            alert("Loop length cannot be greater and equal than Cable Drum Length!");
            $('#loopLength').addClass('form-control input-validation-error');
            return false;
        }
        if (loop_span >= cableDrumLength) {
            alert("Loop Span cannot be greater and equal than Cable Drum Length!");
            $('#loopSpan').addClass('form-control input-validation-error');
            return false;
        }
        if (manhole_distance >= cableDrumLength) {
            alert("Manhole distance cannot be greater and equal than Cable Drum Length!");
            $('#manholeSpan').addClass('form-control input-validation-error');
            return false;
        }
        if (pole_distance >= cableDrumLength) {
            alert("Pole distance cannot be greater and equal than Cable Drum Length!");
            $('#poleSpan').addClass('form-control input-validation-error');
            return false;
        }
        if (threshold >= cableDrumLength) {
            alert("Sprout threshold value cannot be greater and equal than Cable Drum Length!");
            $('#sproutThreshold').addClass('form-control input-validation-error');
            return false;
        }     
        
        return true;
    }
    this.onChangeEditPath = function (input) {
        app.Network_Path = input;
        app.createFullNetwork();
        //app.networkPlanningmode();
    }

    this.networkPlanningmode = function () {
        if (si.gMapObj.infoEntity != undefined) { si.gMapObj.infoEntity.setMap(null); }
        if (app.NetworkEndPoint != null && app.NetworkStartPoint != null) {
            if (app.NetworkStartPoint != null) { app.P2PNetworkManual('start'); }
        }
        else {
            if (app.NetworkStartPoint != null) { app.P2PNetworkManual('start'); }
            if (app.NetworkEndPoint != null) { app.P2PNetworkManual('end'); }
        }
    }
    this.CheckLoopRequired = function () {    
        var is_loop_required = $("input[name='is_loop_required']:checked").val();
        if (is_loop_required == "True") {
            $('#DvloopLength').show();
            $('#dvLoopSpan').show();
            $('#ManageLoop').show();
            return true;
        }
        else {
            $('#DvloopLength').hide();
            $('#dvLoopSpan').hide();
            $('#ManageLoop').hide();
            $('#loopLength').val(0);
            $('#loopSpan').val(0);
            return false;
        }
    }
    this.Manage_Loop = function () {
        if (isNaN($('#BomDetails').html())) {
            var loop_length = $('#loopLength').val();
            let temp_Plan_id = $('#plan_id').val();
            let geometry = $('#geometry').val();
            let p_loopSpan = $('#loopSpan').val();
            popup.LoadModalDialog(si.ParentModel, 'BackBonePlan/GetLoopManage', { planid: temp_Plan_id, looplength: loop_length, is_loop_updated: true, line_geom: geometry, loopSpan: p_loopSpan }, 'Loop Management', 'modal-lg');
        }
        else {
            alert("First get BOM/BOQ");
        }

    }
    this.loopDetails = function () {

        alert("Loop Updated Successfully!");       

        let plan_id = $('#plan_id').val();
        let sproutDropdownType = $('#sproutFiberDropdown').val();
        let backboneDropdownType = $('#backboneFiberDropdown').val();
        let geometry = $('#geometry').val();
        let isTrench = $('#is_create_trench').is(':checked');
        let isDuct = $('#is_create_duct').is(':checked');
        ajaxReq('BackBonePlan/getLoopLength', { plan_id: plan_id, sproutType: sproutDropdownType, backboneType: backboneDropdownType, geometry: geometry, isCreateDuct : isDuct,isCreateTrench :isTrench }, false, function (resp) {

            if (resp.status.toLowerCase() == "ok") {
                var length = resp.result.cable_Length_qty;
                var cost_per_unit = resp.result.cost_per_unit;
                var service_cost_per_unit = resp.result.service_cost_per_unit;
                var amount = resp.result.total_cost;
                $('#tblRecurringCharges').find('#Loop > .lngqty').text(length);
                $('#tblRecurringCharges').find('#Loop > .cost_per_unit').text(cost_per_unit);
                $('#tblRecurringCharges').find('#Loop > .service_cost_per_unit').text(service_cost_per_unit);
                $('#tblRecurringCharges').find('#Loop > .amount').text(amount);
                $('#tblRecurringCharges tr#Loop').trigger('click');

                $('#closeModalPopup').trigger("click");
            }
        }, true, false);
    }

    this.GetNearByEntitiesBySiteLatLong = function (geom, end_point_type) {
        return new Promise((resolve, reject) => {
            let geomObj = geom.split(",");
            let lat = parseFloat(geomObj[0]);
            let lng = parseFloat(geomObj[1]);
            const latLng = new google.maps.LatLng(lat, lng);

            si.collapseRemove();
            si.map.setCenter(latLng);
            si.map.setZoom(20);

            $.ajax({
                url: 'Main/GetNearBySiteEntitiesByLatLong',
                type: 'GET',
                data: {
                    latitude: latLng.lat(),
                    longitude: latLng.lng(),
                    bufferInMtrs: getMeterDistanceFromZoom(20)
                },
                success: function (resp) {
                    if (resp.length >= 1) {
                        let podEntity = resp.find(item => item.entity_title === 'Site');
                        if (podEntity) {
                            if (end_point_type === "startpoint") {
                                $('#startpoint').val(latLng.lat().toFixed(6) + "," + latLng.lng().toFixed(6));
                                $('#start_point').val(latLng.lat().toFixed(6) + "," + latLng.lng().toFixed(6));
                                $('#startpoint_network_id').val(podEntity.common_name);
                            } else {
                                $('#endpoint').val(latLng.lat().toFixed(6) + "," + latLng.lng().toFixed(6));
                                $('#end_point').val(latLng.lat().toFixed(6) + "," + latLng.lng().toFixed(6));
                                $('#endpoint_network_id').val(podEntity.common_name);
                            }
                            resolve(true);
                            return;
                        }
                    }

                    // If no site found
                    $('#startpoint').val('');
                    $('#endpoint').val('');
                    $('#cablelength').val('');
                    if (end_point_type === "startpoint") {
                        $('#startPointErrorMsg').text('No sites are available near the selected location!');
                    } else {
                        $('#endPointErrorMsg').text('No sites are available near the selected location!');
                    }
                    app.NetworkStartPoint = null;
                    app.NetworkEndPoint = null;
                    if (si.endMarker) si.endMarker.setMap(null);
                    if (si.startMarker) si.startMarker.setMap(null);
                    if (backbonedata.StartTmpLine) backbonedata.StartTmpLine.setMap(null);
                    if (backbonedata.EndTmpLine) backbonedata.EndTmpLine.setMap(null);

                    resolve(false);
                },
                error: function (xhr, status, error) {
                    console.error('Error: ' + error);
                    reject(error);
                }
            });
        });
    }

    this.CheckLoopLenght = function (event) {

        var cableLength = parseFloat($('#cablelength').val());
        var loopLength = parseFloat($('#' + event.id).val());
        if (loopLength >= cableLength) {
            alert("Loop length cannot be greater and equal than Cable Drum Length!");
            $('#btnUpdatenetworkLoop').attr('disabled', true);
            return false;
        }
        else {
            $('#btnUpdatenetworkLoop').attr('disabled', false);
        }

    }
     this.bindNearestSites = function () {
        const selected = [];

         $('.row-checkbox:checked').each(function () {
         
            const checkbox = $(this);
            const networkId = checkbox.val();
            const row = checkbox.closest('tr');

            const siteId = row.find('[data-site-id]').data('site-id');
            const siteName = row.find('[data-site-name]').data('site-name');
            const sproutFiber = row.find('.sprout-dropdown').val();

            const lineGeom = row.find('.line-geom').text().trim();  

            selected.push({
                site_id: siteId,
                site_name: siteName,
                network_id: networkId,
                fiberType: sproutFiber,
                is_selected: true,
                plan_id: $('#plan_id').val(),
                line_geom: lineGeom        
            });
        });

        $('#nearestSites').val(JSON.stringify(selected));
    };
    this.ShowRoutesBetweenMarkers = function (sitegeom, cablegeom, networkId,systemId) {
        const startLL = {
            lat: parseFloat(sitegeom.split(',')[0]),
            lng: parseFloat(sitegeom.split(',')[1])
        };
        const endLL = {
            lat: parseFloat(cablegeom.split(',')[0]),
            lng: parseFloat(cablegeom.split(',')[1])
        };
        app.fixedStart = startLL;
        app.fixedEnd = endLL;

        // Clear previous temporary lines
        if (app.StartSiteTmpLine) {
            app.StartSiteTmpLine.setMap(null);
            app.StartSiteTmpLine = null;
        }
        if (app.EndSiteTmpLine) {
            app.EndSiteTmpLine.setMap(null);
            app.EndSiteTmpLine = null;
        }
        if (app.sitePointMarker) app.sitePointMarker.setMap(null);
        // Create renderer once
        if (!app.directionsSiteRenderer) {
            app.directionsSiteRenderer = new google.maps.DirectionsRenderer({
                map: si.map,
                draggable: true,          // user can drag midpoints
                suppressMarkers: true,
                polylineOptions: {
                    strokeColor: '#000000',
                    strokeOpacity: 1,
                    strokeWeight: 3,
                    zIndex: 10
                }
            });
            app.isUserChangingRoute = false;

            google.maps.event.addListener(app.directionsSiteRenderer, 'directions_changed', function () {
               
                if (!app.isUserChangingRoute) {
                    app.isUserChangingRoute = true;
                    $(popup.DE.MinimizeModel).trigger("click");
                    return;
                }
                const dir = app.directionsSiteRenderer.getDirections();
                if (!dir || dir.status !== 'OK') return;

                const path = google.maps.geometry.encoding.decodePath(dir.routes[0].overview_polyline);
                const fullPath = [startLL, ...path, endLL]; 
                const Linegeom = fullPath.map(pt => {
                    const lat = (typeof pt.lat === 'function') ? pt.lat() : pt.lat;
                    const lng = (typeof pt.lng === 'function') ? pt.lng() : pt.lng;
                    return `${lng} ${lat}`;
                }).join(', ');
                $('#line-geom-' + networkId).text(Linegeom);                  
                setTimeout(() => {
                    backbonedata.bindNearestSites();
                    backbonedata.UpdateNearestSitesRoute(systemId, Linegeom, networkId);
                }, 100); 
                let isBuffer = $('#planbuffer').val();
                if (isBuffer.trim() !== '' && isBuffer > 0) {
                    app.PlanningBufferPoint();
                }                  
            });
        }

        // Fetch route
        directionsService.route({
            origin: startLL,
            destination: endLL,
            travelMode: google.maps.TravelMode.DRIVING,
            provideRouteAlternatives: false
        }, function (resp, status) {
            if (status !== google.maps.DirectionsStatus.OK) {
                alert('No route found between selected points.');
                return;
            }

            const route = resp.routes[0];
            const latLngArr = google.maps.geometry.encoding.decodePath(route.overview_polyline);

            // Draw connector: from actual start point to road
            app.StartSiteTmpLine = new google.maps.Polyline({
                path: [startLL, latLngArr[0]],
                map: si.map,
                strokeColor: '#000000',
                strokeOpacity: 1,
                strokeWeight: 3
            });

            // Draw connector: from road end to actual end point
            //app.EndSiteTmpLine = new google.maps.Polyline({
            //    path: [latLngArr[latLngArr.length - 1], endLL],
            //    map: si.map,
            //    strokeColor: '#000000',
            //    strokeOpacity: 1,
            //    strokeWeight: 3
            //});

            app.sitePointMarker = new google.maps.Marker({
                position: startLL,
                map: si.map,
                title: "Site Marker",
                draggable: false,
                icon: {
                    url: 'Content/images/icons/map/POD/A/pod.png',
                }
            });
           
            app.isUserChangingRoute = false;
            app.directionsSiteRenderer.setDirections(resp);         
            app.sitePointMarker.setMap(si.map);
        });
    };

    this.UpdateNearestSitesRoute = function (systemId, lineGeom, networkId) {
       
        let p_geom = lineGeom;
        let planId = $('#plan_id').val();
        let p_systemId = systemId;
        if (p_systemId > 0) {
            ajaxReq('BackBonePlan/UpdateSiteRoute', { geom: p_geom, planId: planId, p_systemId: p_systemId }, true, function (result) {
                if (result.status) {
                    let resp = {
                        result: {
                            plan_id: planId
                        }
                    }
                    debugger;
                    $('#route-length-' + networkId).text(result.SpLength);                                           
                    backbonedata.planBomAndBOQ(resp, result.spTotalLength);
                }
            });
        }
    };

    this.ShowAutoPlanLineLength = function (_path) {
        backbonedata.ResetBomDetails();
        arrLinePath = _path || si.gMapObj.libPath.slice();

        var arr = [];

        for (ll of arrLinePath) {
            arr.push(ll.lng() + ' ' + ll.lat());
        }
        $('#geometry').val(arr);

        backbonedata.GemoValidation(arr);

        ajaxReq('Plan/GetNetworkPlanningLineLength', { geom: $('#geometry').val() }, true, function (resp) {
            distance = resp.result;
            $('#cablelength').val(parseFloat(distance.toFixed(2)));
        }, true, false, true);

    }
}


function showPolygonBufferGeometryOnMap(geojson) {
    if (si.fadeMap) {
        si.fadeMap.setMap(null);
    }

    // geojson is already a JS object, no need to parse

    // Convert GeoJSON polygon coordinates to Google Maps LatLngs
    const coords = geojson.coordinates[0].map(coord => ({ lat: coord[1], lng: coord[0] }));

    const fadeOptions = {
        strokeWeight: 0.6,
        fillColor: '#cccccc',
        fillOpacity: 0.6,
        map: si.map,
        paths: [coords],
        clickable: true,
        zIndex: 1
    };

    si.fadeMap = new google.maps.Polygon(fadeOptions);
}





