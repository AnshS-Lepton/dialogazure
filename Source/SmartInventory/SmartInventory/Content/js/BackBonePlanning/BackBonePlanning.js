
var BackbonePlanning = function () {
    var app = this;
    this.autoplanningPlanId = 0;
    this.endMarker = undefined;
    this.startMarker = undefined;
    this.NetworkStartPoint = null;
    this.NetworkEndPoint = null;
    this.TempNetworkEndPoint = null;
    this.startLatLng = {};
    this.endLatLng = {};
    this.MarkerList = [];
    this.plan_mode = '';
    this.Network_Path = 'google';
    this.cable_calculated_length = 0.0;
    this.temp_gemo = '';
    this.StartTmpLine = null;
    this.EndTmpLine = null;
    this.middleMarkers = [];
    this.AllPlanningPaths = [];
    this.AllDistances = [];
    this.AllPathResponses = [];
    this.record_count = 0;
    this.minDistance = 0;
    this.selectedPlanningPath = [];
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

            //$(app.NP.divPlanTool).hide();
            $("#dvAutoBackBonePlanData").slideToggle('slow', function () {

                si.ActiveNetworkPlanning = false;
                if (app.autoplanningplanid > 0 || si.autoplanid > 0) {
                    si.autoplanid = 0;
                    app.autoplanningplanid = 0;
                    si.LoadLayersOnMap();
                }
                $(app.NP.divBackBonePlanTool).hide();
                app.hideAllNetworkFile();
                removeOldMarkers();
                $('.BackBonePlan').removeClass('activeToolBar');

            });
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
        si.autoplanid = 0;
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
        $('#lengthAutoPlaningDiv').empty()
        backbonedata.NetworkStartPoint = null;
        backbonedata.NetworkEndPoint = null;
        $('#manhole_distance').val('');
        $('#pole_distance').val('');
        $('#cable_length').val('');
        $('#start_point').val('');
        $('#end_point').val('');
        $('#is_create_trench').prop('checked', false);
        $('#is_create_duct').prop('checked', false);
        $('#trenchduct').hide();
        //app.ResetOffSet();
        $("#startpointmap").removeClass('activemarker');
        $("#endpointmap").removeClass('activemarker');
        $("#btnBomCable").attr("disabled", false);
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
        $('#threshold').val('');
        $('#planbuffer').val('');
        const $thead = $('#nearestSitesTable thead');
        const $tbody = $('#nearestSitesTable tbody');
        $thead.hide(); // hide header on error
        $tbody.empty();
        $tbody.append('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');
        $('#siteDropdownToggle').val('--Select Site--');
        // Clear previously drawn polylines from the map
        if (si.backboneself.polylines && si.backboneself.polylines.length > 0) {
            si.backboneself.polylines.forEach(line => {
                if (line && line.setMap) {
                    line.setMap(null);
                }
            });
            si.backboneself.polylines = [];
        }
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
        $("#btnProcessPlan").attr("disabled", true);
        if (si.fadeMap) {
            si.fadeMap.setMap(null);
        }      
    }

    this.ViewBackbonePlanningData = function () {

        if (app.autoplanningplanid > 0 || si.autoplanid > 0) {
            si.autoplanid = 0;
            app.autoplanningplanid = 0;
            si.LoadLayersOnMap();
        }
        $("#planHistotry").html("");
        ajaxReq('BackBonePlan/GetBackbonePlanHistoryData', {}, true, function (resp) {
            $("#planHistotry").html(resp);
            $("#planHistotry").css('background-image', 'none');
        }, false, false);
    }

    this.PlanningBufferPoint = function () {
        let buffer = parseFloat($('#planbuffer').val());
        let geom = $('#geometry').val();
        let startPointNetworkId = $('#startpoint_network_id').val();
        let endPointNetworkId = $('#endpoint_network_id').val();
        $('#siteDropdownToggle').val('--Select Site--');
        if (!isNaN(buffer)) {
            ajaxReq('BackBonePlan/GetBackboneNearestSiteList', { geom: geom, buffer: buffer, startPointNetworkId: startPointNetworkId, endPointNetworkId: endPointNetworkId }, true, function (resp) {
                if (resp.status === "OK") {
                    
                    showPolygonBufferGeometryOnMap(resp.result.buffer_geometry);

                    const $tbody = $('#nearestSitesTable tbody');
                    const $thead = $('#nearestSitesTable thead');
                    $tbody.empty();

                    if (resp.result.sites.length > 0) {
                        $thead.show();  // show header when data exists
                        resp.result.sites.forEach(site => {
                            const row = `
                        <tr>
                            <td><input type="checkbox" class="rowCheckbox" data-id="${site.common_name}"></td>
                            <td>${site.common_name}</td>
                            <td>${site.display_name}</td>
                            <td>${site.network_status}</td>
                        </tr>`;
                            $tbody.append(row);
                        });
                    } else {
                        $thead.hide();  // hide header when no data
                        $tbody.append('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');
                    }
                } else {
                    const $thead = $('#nearestSitesTable thead');
                    const $tbody = $('#nearestSitesTable tbody');
                    $thead.hide(); // hide header on error
                    $tbody.empty();
                    $tbody.append('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');
                }
            }, false, false);
        }
        else {
            const $thead = $('#nearestSitesTable thead');
            const $tbody = $('#nearestSitesTable tbody');
            $thead.hide();  // hide header when no data
            $tbody.append('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');
        }
    };

    // Handle Select All checkbox
    $(document).on('change', '#selectAll', function () {
        const isChecked = $(this).is(':checked');
        $('.rowCheckbox').prop('checked', isChecked);
        app.updateSelectedSites();
    });

    $(document).on('change', '.rowCheckbox', function () {
        const total = $('.rowCheckbox').length;
        const checked = $('.rowCheckbox:checked').length;
        const $selectAll = $('#selectAll');

        if (checked === total) {
            $selectAll.prop('checked', true);
        } else {
            $selectAll.prop('checked', false);
        }
        app.updateSelectedSites();

    });

    $('#siteDropdownToggle').on('click', function () {
        const $table = $('#nearestSitesTable');
        const $thead = $table.find('thead');
        const $tbody = $table.find('tbody');
        // Show dropdown panel
        $('#siteDropdownPanel').toggle();

        // If table is empty, show "No Entity Found" row
        if ($tbody.children('tr').length === 0) {
            $thead.hide();
            $tbody.html('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');
        }
    });

    // Hide dropdown if clicking outside
    $(document).on('click', function (e) {
        const $target = $(e.target);
        if (!$target.closest('#siteLst').length) {
            $('#siteDropdownPanel').hide();
        }
    });
    this.updateSelectedSites = function () {
        const selectedSites = [];
        $('.rowCheckbox:checked').each(function (index) {
            selectedSites.push($(this).data('id'));
            if (index === 0) {
                firstSiteName = $(this).data('id');
            }
        });
        $('#selectedSites').val(selectedSites.join(','));
        if (selectedSites.length > 0) {
            $('#siteDropdownToggle').val(`Selected Site ${firstSiteName}...`);
        }
    }

    this.createBackbonePlanNetwork = function () {
        
        ajaxReq('BackBonePlan/SaveBackboneProcess', $('form').serialize(), true, function (resp) {
            app.autoPlanningShowNetworkLayer(resp);

            if (si.backboneself && si.backboneself.polylines.length > 0) {
                si.backboneself.polylines.forEach(line => {
                    if (line && line.setMap) {
                        line.setMap(null);
                    }
                });
                si.backboneself.polylines = [];
            }

        }, false, true, false);
    }


    this.autoPlanningShowNetworkLayer = function (resp) {
       
        if (resp.objPM.status == "True") {
            alert(resp.objPM.message);
             app.autoplanningPlanId = resp.v_plan_id;
            //$('.lyrRefresh').trigger("click");
            backbonedata.ShowBackbonePlanEntityOnMap(resp.plan_id);
            app.hideAllNetworkFile();
            $("#btnProcessPlan").attr("disabled", true);
        }
        else {
            alert(resp.objPM.message);
        }
    }

    //this.createFullNetwork = function () {
    //    debugger;
    //    si.Network_Path = $('#ddledit_path').val();
    //    app.createCableBetweenMakers();
    //    app.createStartMarker();
    //    app.createEndMarker();
    //}
    this.ShowBackbonePlanEntityOnMap = function (planId) {
        $('.pull-right').find('.activemarker').removeClass('activemarker')
        $('.glyphicon glyphicon-eye-open').removeClass('activemarker');

        ajaxReq('BackBonePlan/GetBackboneForMap', { plan_id: planId }, true, function (resp) {
            if (resp.status.toLowerCase() == "ok") {
               
                $('#dvHighlight' + '_' + planId).addClass('activemarker');
                app.autoplanid = planId;
                si.autoplanid = planId;
                si.LoadLayersOnMap();
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
               // si.LoadLayersOnMap();
                if (app.autoplanningplanid > 0 || si.autoplanid > 0) {
                    si.autoplanid = 0;
                    app.autoplanningplanid = 0;
                    si.LoadLayersOnMap();
                }
            }
        }, true, true);
    }

    //this.ShowTempPlanEntityOnMap = function (planId) {

    //    const flightPlanCoordinates = [];
    //    ajaxReq('Plan/GetTempNetworkForMap', { plan_id: 1 }, true, function (resp) {

    //        if (resp.status.toLowerCase() == "ok") {
    //            var gemo = resp.result[0].geometry.split(',');
    //            for (i = 0; i < gemo.length; i++) {

    //                var lng_lat = gemo[i];
    //                flightPlanCoordinates.push(new google.maps.LatLng(lng_lat.split(' ')[1], parseFloat(lng_lat.split(' ')[0])));
    //            }
    //            directionsDisplay.setMap(null);
    //            app.networkPlanningcable(flightPlanCoordinates);
    //        }
    //    }, true, true);
    //}



    //this.create_aerial_pathcable = function () {


    //}


    this.HighlightPointEntityOnMap = function (mrkrLatlng, imageUrl) {

        if (si.gMapObj.entitySrchObj)
            si.gMapObj.entitySrchObj.setMap(null);
        si.gMapObj.entitySrchObj = app.createAutoMarker(mrkrLatlng, 'Content/images/dwnArrow.png', 'end');
        si.gMapObj.entitySrchObj.setAnimation(google.maps.Animation.BOUNCE);
        si.gMapObj.entitySrchObj.setDraggable(false);
        si.gMapObj.entitySrchObj.setMap(si.map);
        // window.setTimeout(function () { if (si.gMapObj.entitySrchObj != 'undefined' || si.gMapObj.entitySrchObj !=null ) { si.gMapObj.entitySrchObj.setMap(null) } si.gMapObj.entitySrchObj = null; }, 50000);
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

    this.createAutoMarker = function (mrkrLatlng, imageUrl, label, draggable = true) {

        var gmarkernew = new google.maps.Marker({
            position: mrkrLatlng,
            icon: imageUrl,
            // draggable:true,
            draggable: ((label == 'start') ? true : (app.plan_mode == 'auto' || draggable == false) ? false : true)
            //title: ((label == 'start') ? 'Start Point' : 'End Point')
        });

        return gmarkernew;
    }


    //this.downloadBomReport = function (planId) {
    //    window.location = appRoot + 'Report/ExportPlanBOMBOQReport?plan_id=' + planId;
    //}

    this.downloadBackboneBomReport = function (planId) {
        window.location = appRoot + 'Report/ExportBackbonePlanBOMBOQReport?plan_id=' + planId;
    }

    //this.loopUpdate = function () {
    //    $('#is_loop_update').val(true);
    //}

    //this.networkPlanningManualmode = function (end, is_marker_create) {

    //    var EntityPoint = $("#" + end + "_point").val();

    //    if (EntityPoint == '') {
    //        return false;
    //    }
    //    else if (EntityPoint.indexOf(',') > -1) {
    //        if (end == app.NP.end_type.START && is_marker_create) {
    //            EntityPoint = $("#start_point").val();
    //            app.NetworkStartPoint = EntityPoint;
    //            app.P2PNetworkManual('start');

    //        }
    //        else if (end == app.NP.end_type.END && is_marker_create) {
    //            EntityPoint = $("#end_point").val();
    //            app.NetworkEndPoint = EntityPoint;
    //            app.P2PNetworkManual('end');
    //        }
    //    }
    //    else {
    //        if (end == app.NP.end_type.START && si.startMarker) {
    //            si.startMarker.setMap(null);
    //        }
    //        else if (end == app.NP.end_type.END && si.endMarker) {
    //            si.endMarker.setMap(null);
    //        }
    //        if (si.gMapObj.infoEntity != undefined) { si.gMapObj.infoEntity.setMap(null); }
    //        si.clearMarkers();
    //        removeOldMarkersWithRemoveActive();
    //        alert("Invalid " + end + " point please enter valid (Latitude,Longitude)");
    //        return false;
    //    }
    //    if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
    //        app.fitElementOnMap(app.NetworkStartPoint, app.NetworkEndPoint);
    //        si.map.setZoom(si.map.getZoom() - 1);
    //    }
    //    return true;
    //}

    this.NetworkPlanning = function (ismappicker, destination_point) {
       
        $('#endPointErrorMsg').text('');
        $('#startPointErrorMsg').text('');
        if (si.fadeMap) {
            si.fadeMap.setMap(null);
        }
        $('#siteDropdownPanel').hide();
        const $thead = $('#nearestSitesTable thead');
        const $tbody = $('#nearestSitesTable tbody');
        $thead.hide(); // hide header on error
        $tbody.empty();
        $tbody.append('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');

        if (ismappicker) {
            if (destination_point == 'start') {
                $("#startpointmap").addClass('activemarker');
                google.maps.event.addListener(si.map, 'click', function (e) {
                    app.startLatLng = e.latLng;
                    var startLatLog = e.latLng.lat().toFixed(6) + "," + e.latLng.lng().toFixed(6);
                    app.NetworkStartPoint = e.latLng.lat() + "," + e.latLng.lng();
                    // $('#start_point').val(startLatLog);
                    app.P2PNetworkManual(destination_point);
                });
            }
        }

        if (ismappicker) {
            if (destination_point == 'end') {
                $("#endpointmap").addClass('activemarker');
                google.maps.event.addListener(si.map, 'click', function (f) {
                    app.endLatLng = f.latLng;
                    var endLatLog = f.latLng.lat().toFixed(6) + "," + f.latLng.lng().toFixed(6);
                    app.NetworkEndPoint = f.latLng.lat() + "," + f.latLng.lng();
                    // $('#end_point').val(endLatLog);
                    app.P2PNetworkManual(destination_point);
                });
            }
        }
        return true;
    }


    this.createStartMarker = function () {

        if (app.NetworkStartPoint != null) {

            var startlat = parseFloat(app.NetworkStartPoint.split(',')[0]);
            var startlong = parseFloat(app.NetworkStartPoint.split(',')[1]);
            startLatlng = { lat: startlat, lng: startlong };

            if (si.startMarker)
                si.startMarker.setMap(null);
            si.startMarker = app.createAutoMarker(startLatlng, 'Content/images/Actual_Start.png', app.NP.end_type.START);
            si.startMarker.addListener('drag', function (startLatlng) {

                app.NetworkStartPoint = startLatlng.latLng.lat() + "," + startLatlng.latLng.lng();
                app.fillnetworkPlanningMarker(startLatlng, 'start');

                //app.ResetOffSet();
                app.ResetBomDetails();
            });
            //  si.startMarker.addListener('dragend', function (startLatlng) {
            //if (app.plan_mode == 'auto') {
            //  app.autoPlanningEndPoint();
            // }
            // });
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
            si.endMarker.addListener('drag', function (endLatlng) {

                //app.ResetOffSet();
                app.NetworkEndPoint = endLatlng.latLng.lat() + "," + endLatlng.latLng.lng();
                app.fillnetworkPlanningMarker(endLatlng, 'end');
                app.ResetBomDetails();
            });
            si.endMarker.setMap(si.map);

            app.MarkerList.push(si.endMarker);
        }
    }

    this.P2PNetworkManual = function (end) {
        debugger;
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
            app.GetNearByEntitiesBySiteLatLong(app.NetworkStartPoint, "start_point");
            app.createStartMarker();
        }

        if (end == app.NP.end_type.END && app.NetworkEndPoint != null) {

            app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, "end_point");
            app.createEndMarker();

        }
        if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
            
            //app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, "start_point");
            //app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, "end_point");
            app.createCableBetweenMakers();

        }
    }


    this.createCableBetweenMakers = function () {

        debugger;
        app.AllPlanningPaths = [];
        app.AllDistances = [];
        app.minDistance = 0;
        app.AllPathResponses = [];
        directionsDisplay.setMap(null);
        if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
            //app.ResetOffSet();
            app.startLatLng = { lat: parseFloat(app.NetworkStartPoint.split(',')[0]), lng: parseFloat(app.NetworkStartPoint.split(',')[1]) };
            app.endLatLng = { lat: parseFloat(app.NetworkEndPoint.split(',')[0]), lng: parseFloat(app.NetworkEndPoint.split(',')[1]) };
            if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
            if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }
            // if ($("input[name='is_offset_required']:checked").val() == "True") { app.ResetOffSet(); }

            var request = {
                origin: app.startLatLng,
                travelMode: google.maps.DirectionsTravelMode.DRIVING,
                provideRouteAlternatives: true
            };

            app.lastRequest = request;

            request.destination = app.endLatLng;
            app.CheckCableCreated();
            // var network_Type = $('#ddledit_path').val();

            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    app.AllPathResponses.push(response);

                    var Paths = response.routes;
                    //console.log(Paths);
                    for (let i = 0; i < Paths.length; i++) {
                        let totalDist = 0;
                        for (let j = 0; j < Paths[i].legs.length; j++) {
                            totalDist += Paths[i].legs[j].distance.value;
                        }
                        app.AllDistances.push({ 'overview_polyline': Paths[i].overview_polyline, 'route_distance': totalDist, 'Is_Start': true });
                        if (app.minDistance != 0 && app.minDistance > totalDist) {
                            app.minDistance = totalDist;
                            app.AllPlanningPaths.push(Paths[i]);
                        }
                        else if (app.minDistance == 0) {
                            app.minDistance = totalDist;

                            app.AllPlanningPaths.push(Paths[i]);
                        }
                        if (app.AllPlanningPaths.length > 0) {
                            app.BindPlanningPaths(app.AllPlanningPaths, true);
                        }
                    }
                    // console.log("path1" + app.AllPlanningPaths);

                }
                else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_016);//Directions not found between source and destination.
                    directionsDisplay.setMap(null);
                }
            });

            var Reverserequest = {
                origin: app.endLatLng,
                travelMode: google.maps.DirectionsTravelMode.DRIVING,
                provideRouteAlternatives: true
            };

            Reverserequest.destination = app.startLatLng;

            directionsService.route(Reverserequest, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {

                    // if (network_Type != "manually") {
                    var reversePaths = response.routes;
                    app.AllPathResponses.push(response);
                    for (let i = 0; i < reversePaths.length; i++) {
                        let totalDist = 0;

                        for (let j = 0; j < reversePaths[i].legs.length; j++) {
                            totalDist += reversePaths[i].legs[j].distance.value;
                        }

                        //app.AllPlanningPaths.push(reversePaths[i]);
                        app.AllDistances.push({
                            'overview_polyline': reversePaths[i].overview_polyline,
                            'route_distance': totalDist
                            , 'Is_Start': false
                        });
                        if (app.minDistance != 0 && app.minDistance > totalDist) {
                            app.minDistance = totalDist;
                            app.AllPlanningPaths.push(reversePaths[i]);
                            //app.BindPlanningPaths(app.AllPlanningPaths, false);

                        }
                        else if (app.minDistance == 0) {
                            app.minDistance = totalDist;

                            app.AllPlanningPaths.push(reversePaths[i]);
                            //app.BindPlanningPaths(app.AllPlanningPaths, false);
                        }
                        if (app.AllPlanningPaths.length > 0) {
                            app.BindPlanningPaths(app.AllPlanningPaths, false);
                        }
                        //app.BindPlanningPaths(app.AllPlanningPaths, false);
                        // console.log("path2" + app.AllPlanningPaths);
                        //}
                    }

                    //  }

                }
                else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_016);//Directions not found between source and destination.
                    directionsDisplay.setMap(null);
                }
            });
        }
    }
  
    this.BindPlanningPaths = function (paths, IsCheck) {
        
        app.selectedPlanningPath = [];
        var selectedResponse = [];
        app.sortedPlan = [];
        $(".routeBox").empty();
        $("#suggestBox").hide();
        let selectedPlanresponse = [];
        var result = [];
        //var paths = JSON.stringify(paths);
        if (paths.length > 0) {

            if (app.AllDistances.length > 1 && app.AllPlanningPaths.length) {
                app.sortedPlan = app.AllDistances.sort((p1, p2) => (p2.route_distance > p1.route_distance) ? -1 : (p2.route_distance < p1.route_distance) ? 1 : 0);
                result = app.AllPlanningPaths.filter(x => x.overview_polyline == app.sortedPlan[0].overview_polyline);
                $(".routeBox").empty();

            }
            else {
                app.sortedPlan = app.AllDistances;
                result = app.AllPlanningPaths;
            }
            if ($("input[name=suggestRoutes]").length == 0) {
                var route = app.sortedPlan.length;
                if (route > 1) {
                    route = route > 3 ? 3 : route;
                    $('#suggestBox').removeAttr("style");
                    $('.routeBox').removeAttr("style");
                    $('.routeBox').hide();
                    //$('#suggestBox').show();

                    for (var i = 1; i <= route; i++) {
                        $('.routeBox').append('<label class="suggestBoxN">  <input type="radio" name="suggestRoutes" value= "' + i + '" onclick="backbonedata.SuggestedRoute(this);" > Route  ' + i + ' <span class="suggestMark"></span></label>');
                    }
                    $(".routeBox input:radio:first").attr('checked', true);
                    $('.routeBox').toggle();
                }
            }
            backbonedata.SuggestedRoute();


        }
    }

    this.fillnetworkPlanningMarker = function (LatLong, end) {

        var lat;
        var lng;

        if (end == app.NP.end_type.START) {
            app.startLatLng = LatLong.latLng;
            lat = LatLong.latLng.lat();
            lng = LatLong.latLng.lng();
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
        //app.ResetOffSet();

        var startlat = parseFloat(NetworkStartPoint.lat());
        var startlong = parseFloat(NetworkStartPoint.lng());
        var startLatlng = { lat: startlat, lng: startlong };

        if (si.startMarker)
            si.startMarker.setMap(null);
        si.startMarker = app.createAutoMarker(startLatlng, 'Content/images/Actual_Start.png', app.NP.end_type.START);
        si.startMarker.addListener('dragend', function (event) {
           // app.ResetOffSet();
            app.NetworkStartPoint = event.latLng.lat() + "," + event.latLng.lng();
            // $('#start_point').val(app.NetworkStartPoint);
            // if (app.plan_mode == 'auto') {
            // app.autoPlanningEndPoint();
            // } else {
            //  app.GetNearByEntitiesBySiteLatLong(app.NetworkStartPoint, 'start_point');
            app.P2PNetworkManual('start');
            app.ResetBomDetails();
            // }
        });
        si.startMarker.setMap(si.map);
        app.MarkerList.push(si.startMarker);

        var endlat = parseFloat(NetworkEndPoint.lat());
        var endlong = parseFloat(NetworkEndPoint.lng());
        var endLatlng = { lat: endlat, lng: endlong };

        if (si.endMarker)
            si.endMarker.setMap(null);
        si.endMarker = app.createAutoMarker(endLatlng, 'content/images/End.png', app.NP.end_type.END);
        si.endMarker.addListener('dragend', function (event) {
           
            // $('#endpoint').val(app.NetworkStartPoint);
            app.NetworkEndPoint = event.latLng.lat() + "," + event.latLng.lng();
            // app.GetNearByEntitiesBySiteLatLong(app.NetworkStartPoint, 'end_point');
            app.P2PNetworkManual('end');
            app.ResetBomDetails();
        });
        backbonedata.TempNetworkEndPoint = endLatlng.lat + "," + endLatlng.lng;
        si.endMarker.setMap(si.map);
        app.MarkerList.push(si.endMarker);
    }




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
        //  si.gMapObj.infoEntity.setEditable(LineEdiTable);
        si.gMapObj.infoEntity.setMap(si.map);
        si.gMapObj.libPath = latLngArr;

        const path = si.gMapObj.infoEntity.getPath();

        const syncPath = () => {
            // Convert updated path to LatLng array
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
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
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
                strokeColor: '#FF8800',
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
        
        if (!si.gMapObj.infoEntity || !si.gMapObj.infoEntity.getPath) {
            return;
        }

        google.maps.event.addListener(si.gMapObj.infoEntity, 'rightclick', function (event) {
           
            app.deleteAllMiddleMarker();

            // app.ResetOffSet();
            if (event.vertex == undefined) {
                return;
            } else {
                /*check start and end latlng*/
                var latLngArr = si.gMapObj.infoEntity.getPath().getArray();
                var vextexlatlng = event.latLng.lat() + ' ' + event.latLng.lng();
                var cableStartlatlng = latLngArr[0].lat() + ' ' + latLngArr[0].lng();

                endIndex = latLngArr.length - 1;
                var cableendlatlng = latLngArr[endIndex].lat() + ' ' + latLngArr[endIndex].lng();

                if (vextexlatlng == cableStartlatlng) {
                    alert("Start Point Can Not Be Remove!");
                }
                else if (vextexlatlng == cableendlatlng) {
                    alert("End Point Can Not Be Remove!");
                }
                else {
                    si.removeNode(si.gMapObj.infoEntity, event.vertex);
                }
            }
            debugger;

            ShowAutoPlanLineLength();
        });
        google.maps.event.addListener(si.gMapObj.infoEntity.getPath(), 'set_at', function (indx) {
            debugger;
            //app.ResetOffSet();
            debugger;

            OldendIndex = si.gMapObj.libPath.length - 1;
            var newPath = si.gMapObj.infoEntity.getPath();
            var newLibPath = newPath.getArray();
            app.deleteAllMiddleMarker();
            if (indx == 0) {
                si.gMapObj.libPath = newLibPath;
                var startlatlng = newLibPath[0].lat() + ',' + newLibPath[0].lng();
                $('#start_point').val(startlatlng);
                app.NetworkStartPoint = startlatlng;
                app.createStartMarker();
            }
            else if (indx == OldendIndex) {
                si.gMapObj.libPath = newLibPath;
                var endLatLngr = newLibPath[OldendIndex].lat() + ',' + newLibPath[OldendIndex].lng();
                $('#end_point').val(endLatLngr);
                app.NetworkEndPoint = endLatLngr;
                app.createEndMarker();
            }
            else {
                si.gMapObj.libPath = newLibPath;
            };
            debugger;

            ShowAutoPlanLineLength();

            if (newPath.getLength() >= 2) {
                var newStart = newPath.getAt(0);
                var newEnd = newPath.getAt(newPath.getLength() - 1);

                app.NetworkStartPoint = newStart.lat() + "," + newStart.lng();
                app.NetworkEndPoint = newEnd.lat() + "," + newEnd.lng();
                // Extract waypoints if any
                var waypoints = [];
                for (let i = 1; i < newPath.getLength() - 1; i++) {
                    let latLng = newPath.getAt(i);
                    waypoints.push({
                        location: latLng,
                        stopover: false
                    });
                }

                // Build and call Google Directions API
                var request = {
                    origin: newStart,
                    destination: newEnd,
                    waypoints: waypoints,
                    travelMode: google.maps.TravelMode.DRIVING,
                    optimizeWaypoints: true
                };

                directionsService.route(request, function (response, status) {
                    if (status === google.maps.DirectionsStatus.OK) {
                        app.AllPathResponses.push(response);

                        var Paths = response.routes;
                        //console.log(Paths);
                        for (let i = 0; i < Paths.length; i++) {
                            let totalDist = 0;
                            for (let j = 0; j < Paths[i].legs.length; j++) {
                                totalDist += Paths[i].legs[j].distance.value;
                            }
                            app.AllDistances.push({ 'overview_polyline': Paths[i].overview_polyline, 'route_distance': totalDist, 'Is_Start': true });
                            if (app.minDistance != 0 && app.minDistance > totalDist) {
                                app.minDistance = totalDist;
                                app.AllPlanningPaths.push(Paths[i]);
                            }
                            else if (app.minDistance == 0) {
                                app.minDistance = totalDist;

                                app.AllPlanningPaths.push(Paths[i]);
                            }
                            if (app.AllPlanningPaths.length > 0) {
                               // app.BindPlanningPaths(app.AllPlanningPaths, true);
                                ShowAutoPlanLineLength();

                            }
                        }
                    } else {
                        alert('Directions request failed due to ' + status);
                    }
                });
                //app.createCableBetweenMakers();
            }
        });

        google.maps.event.addListener(si.gMapObj.infoEntity.getPath(), 'insert_at', function (indx) {
            debugger;
            /* app.ResetOffSet();*/
            debugger;
            app.deleteAllMiddleMarker();

            var newPath = si.gMapObj.infoEntity.getPath();
            si.gMapObj.libPath = newPath.getArray();
            ShowAutoPlanLineLength();

            if (newPath.getLength() >= 2) {
                var newStart = newPath.getAt(0);
                var newEnd = newPath.getAt(newPath.getLength() - 1);

                app.NetworkStartPoint = newStart.lat() + "," + newStart.lng();
                app.NetworkEndPoint = newEnd.lat() + "," + newEnd.lng();
                // Extract waypoints if any
                var waypoints = [];
                for (let i = 1; i < newPath.getLength() - 1; i++) {
                    let latLng = newPath.getAt(i);
                    waypoints.push({
                        location: latLng,
                        stopover: false
                    });
                }

                // Build and call Google Directions API
                var request = {
                    origin: newStart,
                    destination: newEnd,
                    waypoints: waypoints,
                    travelMode: google.maps.TravelMode.DRIVING,
                    optimizeWaypoints: true
                };

                directionsService.route(request, function (response, status) {
                    if (status === google.maps.DirectionsStatus.OK) {
                        app.AllPathResponses.push(response);

                        var Paths = response.routes;
                        //console.log(Paths);
                        for (let i = 0; i < Paths.length; i++) {
                            let totalDist = 0;
                            for (let j = 0; j < Paths[i].legs.length; j++) {
                                totalDist += Paths[i].legs[j].distance.value;
                            }
                            app.AllDistances.push({ 'overview_polyline': Paths[i].overview_polyline, 'route_distance': totalDist, 'Is_Start': true });
                            if (app.minDistance != 0 && app.minDistance > totalDist) {
                                app.minDistance = totalDist;
                                app.AllPlanningPaths.push(Paths[i]);
                            }
                            else if (app.minDistance == 0) {
                                app.minDistance = totalDist;

                                app.AllPlanningPaths.push(Paths[i]);
                            }
                            if (app.AllPlanningPaths.length > 0) {
                               // app.BindPlanningPaths(app.AllPlanningPaths, true);
                                ShowAutoPlanLineLength();

                            }
                        }
                    } else {
                        alert('Directions request failed due to ' + status);
                    }
                });

            }
        });
        google.maps.event.addListener(si.gMapObj.infoEntity, 'click', function (evt) {
            debugger;

            //app.ResetOffSet();

            var selectPoint = '';
            selectPoint = si.roundNumber(evt.latLng.lng(), 6) + ' ' + si.roundNumber(evt.latLng.lat(), 6);
            var _zoom = si.map.getZoom();
            app.deleteAllMiddleMarker();

            var lineArrayLst = si.gMapObj.infoEntity.getPath().getArray();
            var lenLineArr = lineArrayLst.length;
            if (lenLineArr < 1) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_032);
                return false;
            }
            else {
                var startPoint = si.roundNumber(lineArrayLst[0].lng(), 6) + ' ' + si.roundNumber(lineArrayLst[0].lat(), 6);
                var endPoint = si.roundNumber(lineArrayLst[lenLineArr - 1].lng(), 6) + ' ' + si.roundNumber(lineArrayLst[lenLineArr - 1].lat(), 6);
                if (startPoint == selectPoint || endPoint == selectPoint) {
                    var chkPointType = (startPoint == selectPoint) ? 'start' : 'end';
                }
            }
        });

    }

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

    this.planBomAndBOQ = function (resp) {
         $('#plan_id').val(resp.result.plan_id);
        let planId = $('#plan_id').val();

        ajaxReq('BackBonePlan/GetBomBOQData', $('form').serialize(), true, function (resp) {
            $("#BomBoqDetails").html(resp);
            $('#btnProcessPlan').prop('disabled', false);
            app.drawGeoJsonLinesOnMap(planId);
        }, false, true, false);
    }

    this.drawGeoJsonLinesOnMap = function (planId) {
        debugger;
        si.backboneself = this;
        si.gMapObj.infoEntity = null;
        si.gMapObj.libPath = null;
        si.point2pointgeom = [];
        if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
        if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }
        // Clear previously drawn polylines from the map
        if (si.backboneself.polylines && si.backboneself.polylines.length > 0) {
            si.backboneself.polylines.forEach(line => {
                if (line && line.setMap) {
                    line.setMap(null);
                }
            });
            si.backboneself.polylines = [];
        }
        if (si.fadeMap) {
            si.fadeMap.setMap(null);
        }  
        ajaxReq('BackBonePlan/GetDraftLineGeometry', { planId: planId }, false, function (geometryList) {
            geometryList.forEach(feature => {
                const geo = JSON.parse(feature.geojson); // parse the GeoJSON string

                if (geo.type === 'LineString') {
                    debugger;
                    const path = geo.coordinates.map(coord => new google.maps.LatLng(coord[1], coord[0]));
                    si.backbonePolyline = si.backboneself.createAutoPlanLine(path, true, false , 3);
                    si.backbonePolyline.setMap(si.map);

                    si.backboneself.polylines = si.backboneself.polylines || [];
                    si.backboneself.polylines.push(si.backbonePolyline);
                }
            });
        }, false, true, false);
    }

    this.SuggestedRoute = function () {
        debugger;
        if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
        if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }

        var route = $('input[name="suggestRoutes"]:checked').val();
        if (route == undefined) route = 1;
        for (let i = 0; i < backbonedata.AllPathResponses.length; i++) {
            for (let j = 0; j < backbonedata.AllPathResponses[i].routes.length; j++) {
                //console.log("route s", route);
                // console.log("i:" + i + "j:" + j);
                if (backbonedata.AllPathResponses[i].routes[j].overview_polyline ==
                    backbonedata.sortedPlan[route - 1].overview_polyline) {
                    var response = backbonedata.AllPathResponses[i].routes.filter(x => x.overview_polyline ==
                        backbonedata.sortedPlan[route - 1].overview_polyline);
                    // console.log("ryte response", response);
                    if (response != undefined) {
                        app.selectedPlanningPath = backbonedata.AllPathResponses[i].routes[j];
                        //console.log("Route Selected ", app.selectedPlanningPath);
                        break;
                    }
                }
            }
            if (response != undefined) {
                break;
            }
        }

        var latLngArr = google.maps.geometry.encoding.decodePath(app.selectedPlanningPath.overview_polyline);

        var endIndex = latLngArr.length - 1;
        if (app.sortedPlan[route - 1].Is_Start == true) {
            const startPoint = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
            const startPointLineArr = [];
            startPointLineArr.push(latLngArr[0]);
            startPointLineArr.push(startPoint);
            app.StartTmpLine = app.createAutoPlanLine(startPointLineArr, true, false);
            app.StartTmpLine.setMap(si.map);
            si.gMapObj.infoEntity = app.StartTmpLine; 
            app.initialClickForAutoEditLine();

            const EndPoint = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
            const EndPointLineArr = [];
            for (let i = 0; i < latLngArr.length; i++) {
                EndPointLineArr.push(latLngArr[i]);
            }
            EndPointLineArr.push(EndPoint);
            app.EndTmpLine = app.createAutoPlanLine(EndPointLineArr, true, false);
            app.EndTmpLine.setMap(si.map);
            si.gMapObj.infoEntity = app.EndTmpLine; 

            app.initialClickForAutoEditLine();

            app.createDirectionMarker(startPoint, EndPoint);
            latLngArr.splice(0, 0, startPoint);
            latLngArr.push(EndPoint);

            ShowAutoPlanLineLength(latLngArr);
        }
        else {
            app.NetworkStartPoint = app.endLatLng.lat + "," + app.endLatLng.lng;
            app.NetworkEndPoint = app.startLatLng.lat + "," + app.startLatLng.lng;
            const startPoint = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
            const startPointLineArr = [];
            startPointLineArr.push(latLngArr[0]);
            startPointLineArr.push(startPoint);
            app.StartTmpLine = app.createAutoPlanLine(startPointLineArr, true, false);
            app.StartTmpLine.setMap(si.map);
            si.gMapObj.infoEntity = app.StartTmpLine; 

            app.initialClickForAutoEditLine();

            const EndPoint = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
            const EndPointLineArr = [];
            for (let i = 0; i < latLngArr.length; i++) {
                EndPointLineArr.push(latLngArr[i]);
            }
            EndPointLineArr.push(EndPoint);
            app.EndTmpLine = app.createAutoPlanLine(EndPointLineArr, true, false);
            app.EndTmpLine.setMap(si.map);
            si.gMapObj.infoEntity = app.EndTmpLine; 

            app.initialClickForAutoEditLine();

            app.createDirectionMarker(startPoint, EndPoint);
            latLngArr.splice(0, 0, startPoint);
            latLngArr.push(EndPoint);

            ShowAutoPlanLineLength(latLngArr);
        }
    };
    //this.createOffsetCable = function () {

    //    //networkdata.geomToForm();
    //    var cablegemo = $('#geometry').val();
    //    if (cablegemo != '') {


    //        if (cablegemo != '') {
    //            if ($('#offset_value').val() == '') {
    //                $('#offset_value').val(0);
    //                //networkdata.createOffsetCable();
    //                //alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_034);
    //                return false;
    //            }
    //            else if (parseInt($('#offset_value').val()) > parseInt($('#hdnMaxOffsetNetworkPlanning').val())) {
    //                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_107);
    //                return false;
    //            }
    //            var offSet = '';
    //            if ($('#is_offset_requiredLeft').prop('checked'))
    //                offSet = '-' + parseFloat($('#offset_value').val()) / 100000;
    //            else
    //                offSet = parseFloat($('#offset_value').val()) / 100000;

    //            if (app.temp_gemo == "") {
    //                app.temp_gemo = cablegemo;
    //            }

    //            if ($('#offset_value').val() == 0) {
    //                var latLngArr = [];
    //                var longLatArr = app.temp_gemo.split(',');


    //                for (var i = 0; i < longLatArr.length; i++) {
    //                    var LongLatsingle = longLatArr[i].split(' ');
    //                    latLngArr.push(new google.maps.LatLng(parseFloat(LongLatsingle[1]), parseFloat(LongLatsingle[0])));
    //                }

    //                var startlatlng = latLngArr[0].lat().toFixed(6) + ',' + latLngArr[0].lng().toFixed(6);
    //                $('#start_point').val(startlatlng);
    //                app.NetworkStartPoint = startlatlng;
    //                app.createStartMarker();

    //                if (app.plan_mode == 'manual_planning') {
    //                    var endIndex = latLngArr.length - 1;
    //                    var endLatLngr = latLngArr[endIndex].lat().toFixed(6) + ',' + latLngArr[endIndex].lng().toFixed(6);
    //                    $('#end_point').val(endLatLngr);
    //                    app.NetworkEndPoint = endLatLngr;
    //                    app.createEndMarker();
    //                }
    //                app.networkPlanningcable(latLngArr);
    //                $('#geometry').val(longLatArr);
    //                ShowAutoPlanLineLength(latLngArr);
    //            }
    //            else {

    //                ajaxReq('plan/getOffSetPolyLineCurve', { cablegemo: app.temp_gemo, offset: offSet }, false, function (jSonResp) {
    //                    var latLngArr = [];
    //                    var longLat = jSonResp.offsetGeom.substring(jSonResp.offsetGeom.indexOf('(') + 1, jSonResp.offsetGeom.lastIndexOf(')')).replace(/\(/, '').replace(/\)/, '');
    //                    var longLatArr = longLat.split(',');

    //                    if ($('#is_offset_requiredLeft').prop('checked')) { longLatArr.reverse(); }

    //                    if (app.plan_mode == 'auto') {


    //                        var latlngEndPoint = null;
    //                        if ($('#ddledit_path').val() == "manually") {
    //                            latlngEndPoint = app.NetworkEndPoint.split(',');
    //                        }
    //                        else {
    //                            latlngEndPoint = backbonedata.TempNetworkEndPoint.split(',');
    //                        }
    //                        longLatArr.push(latlngEndPoint[1] + ' ' + latlngEndPoint[0]);
    //                    }

    //                    for (var i = 0; i < longLatArr.length; i++) {
    //                        var LongLatsingle = longLatArr[i].split(' ');
    //                        latLngArr.push(new google.maps.LatLng(parseFloat(LongLatsingle[1]), parseFloat(LongLatsingle[0])));
    //                    }

    //                    var startlatlng = latLngArr[0].lat().toFixed(6) + ',' + latLngArr[0].lng().toFixed(6);
    //                    $('#start_point').val(startlatlng);
    //                    app.NetworkStartPoint = startlatlng;
    //                    app.createStartMarker();

    //                    if (app.plan_mode == 'manual_planning') {
    //                        var endIndex = latLngArr.length - 1;
    //                        var endLatLngr = latLngArr[endIndex].lat().toFixed(6) + ',' + latLngArr[endIndex].lng().toFixed(6);
    //                        $('#end_point').val(endLatLngr);
    //                        app.NetworkEndPoint = endLatLngr;
    //                        app.createEndMarker();
    //                    }
    //                    $('#geometry').val(longLatArr);
    //                    app.networkPlanningcable(latLngArr);
    //                    ShowAutoPlanLineLength(latLngArr);
    //                }, true, false);
    //            }

    //        }
    //    }



    //}

    this.onChangeEditPath = function (input) {
        debugger;
        app.Network_Path = input;
        app.createFullNetwork();
        //app.networkPlanningmode();
    }

    this.networkPlanningmode = function () {
        debugger;
        //removeOldMarkers();
        // removeOldMarkersWithRemoveActive();
        if (si.gMapObj.infoEntity != undefined) { si.gMapObj.infoEntity.setMap(null); }
        if (app.NetworkEndPoint != null && app.NetworkStartPoint != null) {
            if (app.NetworkStartPoint != null) { app.P2PNetworkManual('start'); }
        }
        else {
            if (app.NetworkStartPoint != null) { app.P2PNetworkManual('start'); }
            if (app.NetworkEndPoint != null) { app.P2PNetworkManual('end'); }
        }
    }

    //this.getNewPathGeom = function (result) {
    //    si.point2pointgeom = [];
    //    var path = [];
    //    if (result.routes.length) {
    //        path = google.maps.geometry.encoding.decodePath(result.routes[0].overview_polyline);
    //        for(ll of path) {
    //            si.point2pointgeom.push(ll.lng() + ' ' + ll.lat());
    //        }
    //    }
    //    return si.path;
    //}

    //this.createBufferCircle = function () {

    //    var centerPoint = app.NetworkStartPoint;
    //    var sepVals = centerPoint.split(',');
    //    if (sepVals.length == 1) {
    //        sepVals = address.split(' ');
    //    }
    //    if (sepVals.length == 2) {
    //        var lat = parseFloat(sepVals[0]);
    //        var lng = parseFloat(sepVals[1]);

    //        if (!isNaN(lat) && !isNaN(lng))
    //            gcFlag = false;

    //        if (lat > -90 && lat < 90 && lng > -180 && lng < 180) {

    //            var addrLocation = new google.maps.LatLng(lat, lng);
    //            si.map.setCenter(addrLocation);
    //            //app.map.setZoom(18);
    //            // app.showTempDownArrow(addrLocation, true);

    //            startWidget();

    //            //var addrLocation = new google.maps.LatLng(lat, lng);
    //            //var latlng = { lat: lat, lng: lng };
    //            //app.geocoder.geocode({ 'location': latlng }, function (results, status) {
    //            //    if (status === 'OK') {
    //            //        $('#placesSearch').val(results[0].formatted_address);
    //            //        si.placeChange(results[0]);
    //            //    }
    //            //    else {
    //            //        window.alert('Geocoder failed due to: ' + status);
    //            //    }
    //            //});
    //        }
    //    }
    //};

    this.GetNearByEntitiesBySiteLatLong = function (geom, end_point_type) {

        debugger;
        let geomObj = geom.split(",");
        let lat = parseFloat(geomObj[0]);
        let lng = parseFloat(geomObj[1]);
        const latLng = new google.maps.LatLng(lat, lng);
        si.collapseRemove();
        si.map.setCenter(latLng);
        si.map.setZoom(20);
        $('#lengthAutoSitePlaningDiv').text('');

        $.ajax({
            url: '/Main/GetNearBySiteEntitiesByLatLong',
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
                        debugger;
                        end_point_type === "start_point" ? $('#start_point').val(latLng.lat().toFixed(6) + "," + latLng.lng().toFixed(6)) : $('#end_point').val(latLng.lat().toFixed(6) + "," + latLng.lng().toFixed(6))
                        end_point_type === "start_point" ? $('#startpoint_network_id').val(podEntity.common_name) : $('#endpoint_network_id').val(podEntity.common_name)
                    } else {
                        $('#start_point').val('');
                        $('#end_point').val('');
                        end_point_type === "start_point" ? app.NetworkStartPoint = null : app.NetworkEndPoint = null;
                        end_point_type === "start_point" ? $('#startPointErrorMsg').text('No sites are available near the selected location!') : $('#endPointErrorMsg').text('No sites are available near the selected location!');
                        $('#lengthAutoSitePlaningDiv').text('');
                        //ulNE.html('No Found Site.');
                        //$('#searchNBEntities').show();                      
                        if (si.endMarker) { si.endMarker.setMap(null); }
                        if (si.startMarker) { si.startMarker.setMap(null); }
                        if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
                        if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }

                    }
                }
                else {
                    $('#start_point').val('');
                    $('#end_point').val('');
                    end_point_type === "start_point" ? app.NetworkStartPoint = null : app.NetworkEndPoint = null;
                    end_point_type === "start_point" ? $('#startPointErrorMsg').text('No sites are available near the selected location!') : $('#endPointErrorMsg').text('No sites are available near the selected location!');
                    //$('#end_point').val('');
                    $('#lengthAutoSitePlaningDiv').text('');
                    if (si.endMarker) { si.endMarker.setMap(null); }
                    if (si.startMarker) { si.startMarker.setMap(null); }
                    if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
                    if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }

                }
            },
            error: function (xhr, status, error) {
                console.log('Error: ' + error);
            }
        });
    }
}
function ShowAutoPlanLineLength(_path) {

    debugger;
    backbonedata.ResetBomDetails();
    arrLinePath = _path || si.gMapObj.libPath.slice();

    var arr = [];

    for (ll of arrLinePath) {
        arr.push(ll.lng() + ' ' + ll.lat());
    }
    $('#geometry').val(arr);

    backbonedata.GemoValidation(arr);

    //ajaxReq('Plan/GetNetworkPlanningLineLength', { geom: $('#geometry').val() }, true, function (resp) {
    //    distance = resp.result;
    //    backbonedata.cable_calculated_length = parseFloat(distance.toFixed(2));
    //    $('#lengthAutoPlaningDiv').html('<p>' + MultilingualKey.SI_OSP_ROW_NET_GBL_021 + ': <i>' + distance.toFixed(2) + '(m)' + '</i></p>');
    //}, true, false, true);

    var startlatlng = arrLinePath[0];
    var endlatlng = arrLinePath[arrLinePath.length - 1];

   // $('#start_point').val(startlatlng.lat().toFixed(6) + ',' + startlatlng.lng().toFixed(6));

   // $('#end_point').val(endlatlng.lat().toFixed(6) + ',' + endlatlng.lng().toFixed(6));
}

//function showAutoPlanDist(_path) {

//    var distance = google.maps.geometry.spherical.computeLength(_path);
//    if (parseInt(distance) > 1000)
//        return (distance / 1000).toFixed(2) + ' km';
//    else
//        return distance.toFixed(2) + ' m';
//}
//$('#demo').on('change', function (input) {
//    var id = $(this).val();
//    var lat_long = $('tr[data-content="' + id + '"]').find('td:last').text();
//    var lat_longArr = lat_long.split(',');
//    var endpointLat_long = parseFloat(lat_longArr[0]).toFixed(6) + "," + parseFloat(lat_longArr[1]).toFixed(6);
//    $('#end_point').val(endpointLat_long);
//    $('#end_point_entity').val(id);
//    //endLatlng = { lat: parseFloat(lat_longArr[0]), lng: parseFloat(lat_longArr[1]) };
//    backbonedata.NetworkEndPoint = lat_long;
//    backbonedata.P2PNetworkManual('end');
//});

// create buffer circle
//function startWidget() {
//    si.fademap = [new google.maps.LatLng(85, 180), new google.maps.LatLng(85, 0), new google.maps.LatLng(85, -180), new google.maps.LatLng(-85, -180), new google.maps.LatLng(-85, 0), new google.maps.LatLng(-85, 180)];

//    //Create Circle on Google Address Search
//    var bufferArea = $('#end_point_buffer').val() / 1000;
//    //var maxdistance = 2;
//    var maxdistance = $('#MaxAutoPlanEndPointBuffer').val() / 1000;;
//    var mindistance = $('#MinAutoPlanEndPointBuffer').val() / 1000;
//    initDistanceWidget(si, bufferArea, maxdistance, mindistance);
//    fadeOuter(si.map.getCenter(), bufferArea);

//}

//function initDistanceWidget(si, startdistance, maxdistance, mindistance) {

//    if (si.distanceWidget) {
//        si.distanceWidget.set("map", null);
//        si.distanceWidget = null;
//    }

//    this.si.distanceWidget = new DistanceWidget({
//        map: si.map,
//        distance: startdistance, // Starting distance in km.
//        maxDistance: maxdistance, // Twitter has a max distance of 2500km.
//        minDistance: mindistance,
//        color: '#3a79c8',
//        activeColor: '#3a79c8',
//        sizerIcon: {
//            url: 'Content/images/resize.png',
//            size: new google.maps.Size(24, 24),
//            origin: new google.maps.Point(0, 0),
//            anchor: new google.maps.Point(12, 12)
//        },
//        icon: {
//            url: 'Content/images/center.png',
//            size: new google.maps.Size(21, 21),
//            origin: new google.maps.Point(0, 0),
//            anchor: new google.maps.Point(10, 10)
//        }
//    });
//}

//function fadeOuter(_center, _radius) {

//    if (si.fadeMap) {
//        si.fadeMap.setMap(null);
//    }
//    var fadeOptions = {
//        strokeWeight: 0,
//        fillColor: '#ffffff',
//        fillOpacity: 0.6,
//        map: si.map,
//        paths: [si.fademap, drawCircle(_center, _radius, -1)]
//    };
//    si.fadeMap = new google.maps.Polygon(fadeOptions);
//}

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
        paths: [coords]
    };

    si.fadeMap = new google.maps.Polygon(fadeOptions);
}


//function drawCircle(point, radius, dir) {

//    var d2r = Math.PI / 180;   // degrees to radians
//    var r2d = 180 / Math.PI;   // radians to degrees
//    var earthsradius = 6371; // 3963 is the radius of the earth in miles
//    var points = 360;

//    // find the raidus in lat/lon
//    var rlat = (radius / earthsradius) * r2d;
//    var rlng = rlat / Math.cos(point.lat() * d2r);

//    var extp = new Array();
//    if (dir == 1) { var start = 0; var end = points + 1 } // one extra here makes sure we connect the ends
//    else { var start = points + 1; var end = 0 }
//    for (var i = start; (dir == 1 ? i < end : i > end); i = i + dir) {
//        var theta = Math.PI * (i / (points / 2));
//        ey = point.lng() + (rlng * Math.cos(theta)); // center a + radius x * cos(theta)
//        ex = point.lat() + (rlat * Math.sin(theta)); // center b + radius y * sin(theta)
//        extp.push(new google.maps.LatLng(ex, ey));
//    }
//    return extp;
//}

//function powerBackupTrue() {

//    $("#txtPwrBkpCapacity").prop("readonly", false);
//}
//function validateSpanlength() {
//    var Spanlength = parseFloat(document.getElementById('pole_manhole_distance').value);
//    var Cabledrumlength = parseFloat(document.getElementById('cable_length').value);
//    var status = "true"

//    if (Spanlength > Cabledrumlength) {
//        alert('Span length equal or less then drum length');
//        document.getElementById('pole_manhole_distance').value = '';
//        status = "false"
//    }
//    return status;
//}




