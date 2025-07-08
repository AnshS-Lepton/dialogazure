
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
                if (app.autoplanningplanid > 0 || si.autobackboneplanid > 0) {
                    si.autobackboneplanid = 0;
                    app.autoplanningplanid = 0;
                    si.LoadLayersOnMap();
                }
                $(app.NP.divBackBonePlanTool).hide();
                app.hideAllNetworkFile();
                removeOldMarkers();
                $('.BackBonePlan').removeClass('activeToolBar');
                if (si.backboneself && Array.isArray(si.backboneself.polylines) && si.backboneself.polylines.length > 0) {
                    si.backboneself.polylines.forEach(line => {
                        if (line && line.setMap) {
                            line.setMap(null);
                        }
                    });
                    si.backboneself.polylines = [];
                }
                // Clear previous main polyline if exists
                if (app.routePolyline) {
                    app.routePolyline.setMap(null);
                    app.routePolyline = null;
                }
                $('#suggestBox').hide();
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
        $('#manhole_distance').val('');
        $('#pole_distance').val('');
        $('#cablelength').val('');
        $('#startpoint').val('');
        $('#endpoint').val('');
        $('#is_create_trench').prop('checked', false);
        $('#is_create_duct').prop('checked', false);
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
        if (si.backboneself && Array.isArray(si.backboneself.polylines) && si.backboneself.polylines.length > 0) {
            si.backboneself.polylines.forEach(line => {
                if (line && line.setMap) {
                    line.setMap(null);
                }
            });
            si.backboneself.polylines = [];
        }
        $('#plan_name').val('');
        $('#cablelength').val('');
        $('#loop_length').val('');
        // Clear previous main polyline if exists
        if (app.routePolyline) {
            app.routePolyline.setMap(null);
            app.routePolyline = null;
        }
        $('#suggestBox').hide();
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

    this.onSiteInputClick = function () {     
        let buffer = parseFloat($('#planbuffer').val());
        let geom = $('#geometry').val();
        let startPointNetworkId = $('#startpoint_network_id').val();
        let endPointNetworkId = $('#endpoint_network_id').val();       
        if (!isNaN(buffer)) {
            popup.LoadModalDialog('CHILD', 'BackBonePlan/GetBackboneNearestSiteList', { geom: geom, buffer: buffer, startPointNetworkId: startPointNetworkId, endPointNetworkId: endPointNetworkId }, "Nearest Site", 'modal-xl');
        }
    }
    this.PlanningBufferPoint = function () {
        let buffer = parseFloat($('#planbuffer').val());
        let geom = $('#geometry').val();
        let startPointNetworkId = $('#startpoint_network_id').val();
        let endPointNetworkId = $('#endpoint_network_id').val();        
        if (!isNaN(buffer)) {
            ajaxReq('BackBonePlan/GetBackboneNearestSiteBuffer', { geom: geom, buffer: buffer, startPointNetworkId: startPointNetworkId, endPointNetworkId: endPointNetworkId }, true, function (resp) {
                if (resp.status === "OK") {

                   showPolygonBufferGeometryOnMap(resp.result.buffer_geometry);
                }
            }, false, false);
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
            $tbody.empty();
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
            $('#siteDropdownToggle').val(`${firstSiteName},...`);
        } else {
            $('#siteDropdownToggle').val(`--Select Site--`);
        }
    }

    this.createBackbonePlanNetwork = function () {

        ajaxReq('BackBonePlan/SaveBackboneProcess', $('form').serialize(), true, function (resp) {
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
        $('i[id^="dvHighlight_"]').removeClass('activemarker'); // Remove from all

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

    this.downloadBackboneBomExcelReport = function (planId) {
        window.location = appRoot + 'Report/ExportBackbonePlanBOMBOQReport?plan_id=' + planId;
    }
    this.downloadBackboneBomKMLReport = function (planId) {
        window.location = appRoot + 'Report/ExportKMLBackbonePlanBOMBOQReport?plan_id=' + planId;
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
        $thead.hide();
        $tbody.empty();
        $tbody.append('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');

        if (ismappicker) {
            if (destination_point === 'start') {
                $("#startpointmap").addClass('activemarker activeicon');

                google.maps.event.addListener(si.map, 'click', function (e) {
                    app.startLatLng = e.latLng;
                    var startLatLog = e.latLng.lat().toFixed(6) + "," + e.latLng.lng().toFixed(6);
                    app.NetworkStartPoint = e.latLng.lat() + "," + e.latLng.lng();
                    $('#startpoint').val(startLatLog);
                    $('#start_point').val(startLatLog);

                   // app.GetNearByEntitiesBySiteLatLong(app.NetworkStartPoint, 'start_point')
                     //   .then(function (isSite) {
                      //      if (isSite) {
                    app.P2PNetworkManual(destination_point);
                    app.ResetBomDetails();
                     //       }
                     //   })
                      //  .catch(function (err) {
                      //      console.error("Error during GetNearByEntitiesBySiteLatLong:", err);
                     //   });
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

                  //  app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, 'end_point')
                      //  .then(function (isSite) {
                      //      if (isSite) {
                                app.P2PNetworkManual(destination_point);
                                app.ResetBomDetails();

                        //    }
                      //  })
                       // .catch(function (err) {
                       //     console.error("Error during GetNearByEntitiesBySiteLatLong:", err);
                      //  });
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
            $('#endpoint').val(endlat.toFixed(6) + ',' + endlong.toFixed(6));
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
            //app.GetNearByEntitiesBySiteLatLong(app.NetworkStartPoint, "start_point");
            app.createStartMarker();
        }

        if (end == app.NP.end_type.END && app.NetworkEndPoint != null) {

            //app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, "end_point");
            app.createEndMarker();

        }
        if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {

            //app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, "start_point");
            //app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, "end_point");
            app.createCableBetweenMakers();          

        }
    }


    this.createCableBetweenMakers = function () {
        app.AllPlanningPaths = [];
        app.AllDistances = [];
        app.minDistance = 0;
        app.AllPathResponses = [];
        directionsDisplay.setMap(null);

        if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
            app.startLatLng = {
                lat: parseFloat(app.NetworkStartPoint.split(',')[0]),
                lng: parseFloat(app.NetworkStartPoint.split(',')[1])
            };
            app.endLatLng = {
                lat: parseFloat(app.NetworkEndPoint.split(',')[0]),
                lng: parseFloat(app.NetworkEndPoint.split(',')[1])
            };

            if (backbonedata.StartTmpLine) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
            if (backbonedata.EndTmpLine) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }

            app.lastRequest = {
                origin: app.startLatLng,
                destination: app.endLatLng,
                travelMode: google.maps.DirectionsTravelMode.DRIVING,
                provideRouteAlternatives: true
            };

            directionsService.route(app.lastRequest, function (response, status) {
                if (status === google.maps.DirectionsStatus.OK) {
                    const Paths = response.routes;
                    app.AllPathResponses.push(response);

                    for (let i = 0; i < Paths.length; i++) {
                        let totalDist = 0;
                        for (let j = 0; j < Paths[i].legs.length; j++) {
                            totalDist += Paths[i].legs[j].distance.value;
                        }

                        app.AllDistances.push({
                            overview_polyline: Paths[i].overview_polyline,
                            route_distance: totalDist,
                            Is_Start: true
                        });

                        app.AllPlanningPaths.push(Paths[i]);
                    }

                    app.BindPlanningPaths(app.AllPlanningPaths, true);
                } else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_016);
                    directionsDisplay.setMap(null);
                }
            });

            const Reverserequest = {
                origin: app.endLatLng,
                destination: app.startLatLng,
                travelMode: google.maps.DirectionsTravelMode.DRIVING,
                provideRouteAlternatives: true
            };

            directionsService.route(Reverserequest, function (response, status) {
                if (status === google.maps.DirectionsStatus.OK) {
                    const reversePaths = response.routes;
                    app.AllPathResponses.push(response);

                    for (let i = 0; i < reversePaths.length; i++) {
                        let totalDist = 0;
                        for (let j = 0; j < reversePaths[i].legs.length; j++) {
                            totalDist += reversePaths[i].legs[j].distance.value;
                        }

                        app.AllDistances.push({
                            overview_polyline: reversePaths[i].overview_polyline,
                            route_distance: totalDist,
                            Is_Start: false
                        });

                        app.AllPlanningPaths.push(reversePaths[i]);
                    }

                    app.BindPlanningPaths(app.AllPlanningPaths, false);
                } else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_016);
                    directionsDisplay.setMap(null);
                }
            });
        }
    };

    this.BindPlanningPaths = function (paths, IsCheck) {
        app.selectedPlanningPath = [];
        app.sortedPlan = [];

        // Clear existing dropdown options and hide suggest box initially
        $("#routeSelector").empty();
        $("#suggestBox").hide();

        if (paths.length > 0) {
            // Sort all routes by ascending distance
            app.sortedPlan = app.AllDistances.sort((a, b) => a.route_distance - b.route_distance);

            // Set the shortest route by default
            const shortestRoute = app.AllPlanningPaths.find(
                x => x.overview_polyline === app.sortedPlan[0].overview_polyline
            );

            if (shortestRoute) {
                app.selectedPlanningPath = shortestRoute;
                backbonedata.SuggestedRouteIndex(0); // Draw shortest route (index 0)
            }

            if (app.sortedPlan.length > 1) {
                // Show dropdown
                $('#suggestBox').show();

                // Fill dropdown with available routes
                for (let i = 0; i < app.sortedPlan.length; i++) {
                    let dist = (app.sortedPlan[i].route_distance / 1000).toFixed(2);
                    $("#routeSelector").append(`<option value="${i}">Route ${i + 1}</option>`);
                }

                // Set default selection to shortest (first)
                $("#routeSelector").val("0");
            }

            // Optional: apply buffer logic
            let isBuffer = $('#planbuffer').val();
            if (isBuffer.trim() !== '' && isBuffer > 0) {
                app.PlanningBufferPoint();
            }
        }
    };


    this.SuggestedRouteIndex = function (index) {
        index = parseInt(index);
        if (isNaN(index)) index = 0;

        const selectedOverview = app.sortedPlan[index].overview_polyline;
        const isStart = app.sortedPlan[index].Is_Start;

        // Clear previous main polyline if exists
        if (app.routePolyline) {
            app.routePolyline.setMap(null);
            app.routePolyline = null;
        }

        // Clear connection lines
        if (backbonedata.StartTmpLine) {
            backbonedata.StartTmpLine.setMap(null);
            backbonedata.StartTmpLine = null;
        }
        if (backbonedata.EndTmpLine) {
            backbonedata.EndTmpLine.setMap(null);
            backbonedata.EndTmpLine = null;
        }

        // Find selected route from AllPathResponses
        let found = false;
        for (let i = 0; i < app.AllPathResponses.length && !found; i++) {
            for (let j = 0; j < app.AllPathResponses[i].routes.length && !found; j++) {
                if (app.AllPathResponses[i].routes[j].overview_polyline === selectedOverview) {
                    app.selectedPlanningPath = app.AllPathResponses[i].routes[j];
                    found = true;
                }
            }
        }

        if (!found) {
            alert("Selected route not found.");
            return;
        }

        // Decode full path of selected route
        const latLngArr = google.maps.geometry.encoding.decodePath(app.selectedPlanningPath.overview_polyline);

        // Draw the full route polyline
        app.routePolyline = new google.maps.Polyline({
            path: latLngArr,
            geodesic: true,
            strokeColor: "#FF8800",
            strokeOpacity: 1,
            strokeWeight: 2,
            editable: true
        });
        app.routePolyline.setMap(si.map);

        // Determine start/end points based on direction
        const startPoint = isStart
            ? new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng)
            : new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);

        const endPoint = isStart
            ? new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng)
            : new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);

        // Update NetworkStartPoint/EndPoint strings
        app.NetworkStartPoint = `${startPoint.lat()},${startPoint.lng()}`;
        app.NetworkEndPoint = `${endPoint.lat()},${endPoint.lng()}`;

        // Draw connection line from user point to route start
        const startPointLineArr = [latLngArr[0], startPoint];
        app.StartTmpLine = app.createAutoPlanLine(startPointLineArr, true, false);
        app.StartTmpLine.setMap(si.map);
        si.gMapObj.infoEntity = app.StartTmpLine;
        app.initialClickForAutoEditLine();

        // Draw connection line from route end to user end point
        const endPointLineArr = [...latLngArr, endPoint];
        app.EndTmpLine = app.createAutoPlanLine(endPointLineArr, true, false);
        app.EndTmpLine.setMap(si.map);
        si.gMapObj.infoEntity = app.EndTmpLine;
        app.initialClickForAutoEditLine();

        // Create arrow direction marker
        app.createDirectionMarker(startPoint, endPoint);

        // Update full path length
        latLngArr.unshift(startPoint);
        latLngArr.push(endPoint);
        ShowAutoPlanLineLength(latLngArr);
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
        // app.ResetOffSet();

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

            //app.GetNearByEntitiesBySiteLatLong(app.NetworkStartPoint, 'start_point')
            //    .then(function (isSite) {
            //        if (isSite) {
                        app.P2PNetworkManual('start');
                        app.ResetBomDetails();
             //       }
             //   })
             //   .catch(function (err) {
             //       console.error("Error fetching nearby start site:", err);
             //   });
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

           // app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, 'end_point')
           //     .then(function (isSite) {
            //        if (isSite) {
                        app.P2PNetworkManual('end');
                        app.ResetBomDetails();
             //       }
            //    })
             //   .catch(function (err) {
           //         console.error("Error fetching nearby end site:", err);
           //     });
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
            backbonedata.ResetBomDetails();

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

            ShowAutoPlanLineLength();
            let isBuffer = $('#planbuffer').val();
            if (isBuffer.trim() !== '' && isBuffer > 0) {
                app.PlanningBufferPoint();
            }
        });
      
        google.maps.event.addListener(si.gMapObj.infoEntity.getPath(), 'set_at', function (indx) {
            let newPath = si.gMapObj.infoEntity.getPath();
            let newLibPath = newPath.getArray();

            app.deleteAllMiddleMarker();

            // Update start or end marker only if needed
            if (indx === 0) {
                const startLatLng = newLibPath[0].lat() + ',' + newLibPath[0].lng();
                app.NetworkStartPoint = startLatLng;
                $('#start_point').val(startLatLng);
                app.createStartMarker();
            }

            if (indx === newLibPath.length - 1) {
                const endLatLng = newLibPath[newLibPath.length - 1].lat() + ',' + newLibPath[newLibPath.length - 1].lng();
                app.NetworkEndPoint = endLatLng;
                $('#end_point').val(endLatLng);
                app.createEndMarker();
            }

            si.gMapObj.libPath = newLibPath;
            if (app.routePolyline) {
                app.routePolyline.setMap(null);
                app.routePolyline = null;
            }
            ShowAutoPlanLineLength(newLibPath);

            let isBuffer = $('#planbuffer').val();
            if (isBuffer.trim() !== '' && isBuffer > 0) {
                app.PlanningBufferPoint();
            }
        });

        google.maps.event.addListener(si.gMapObj.infoEntity.getPath(), 'insert_at', function (indx) {
            let newPath = si.gMapObj.infoEntity.getPath();
            let newLibPath = newPath.getArray();

            si.gMapObj.libPath = newLibPath;
     
            // ✅ Remove old polyline if exists
            if (app.routePolyline) {
                app.routePolyline.setMap(null);
                app.routePolyline = null;
            }
            app.deleteAllMiddleMarker();
            ShowAutoPlanLineLength(newLibPath);

            let isBuffer = $('#planbuffer').val();
            if (isBuffer.trim() !== '' && isBuffer > 0) {
                app.PlanningBufferPoint();
            }
        });

        google.maps.event.addListener(si.gMapObj.infoEntity, 'click', function (evt) {
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
            $("#ManageLoop").attr("disabled", false);
             app.CheckLoopRequired();
            app.drawGeoJsonLinesOnMap(planId);
            si.startMarker.setOptions({
                draggable: false,
                clickable: false, // optional, if you also want to disable clicking
            });
            si.endMarker.setOptions({
                draggable: false,
                clickable: false, // optional, if you also want to disable clicking
            }); 
        }, false, true, false);
    }

    this.loopValidation = function () {
        var cableLength = parseFloat($('#cablelength').val());
        var loop_length = parseFloat($('#loop_length').val());
        var threshold = parseFloat($('#threshold').val());
        var pole_distance = parseFloat($('#pole_distance').val());
        var manhole_distance = parseFloat($('#manhole_distance').val());
        var is_loop_required = $("input[name='is_loop_required']:checked").val();

        if (loop_length <= 0 && is_loop_required == "True") {
            $('#loop_length').addClass('form-control input-validation-error');
            return false;
        }
        if (loop_length >= cableLength) {
            alert("Loop length cannot be greater and equal than Cable Route Length!");
            $('#loop_length').addClass('form-control input-validation-error');
            return false;
        }
        if (threshold >= cableLength) {
            alert("Threshold value cannot be greater and equal than Cable Route Length!");
            $('#loop_length').addClass('form-control input-validation-error');
            return false;
        }
        if (pole_distance >= cableLength) {
            alert("Pole distance cannot be greater and equal than Cable Route Length!");
            $('#loop_length').addClass('form-control input-validation-error');
            return false;
        }
        if (manhole_distance >= cableLength) {
            alert("Manhole distance cannot be greater and equal than Cable Route Length!");
            $('#loop_length').addClass('form-control input-validation-error');
            return false;
        }
        return true;
    }

    this.drawGeoJsonLinesOnMap = function (planId) {
        si.backboneself = this;
        si.gMapObj.infoEntity = null;
        //si.gMapObj.libPath = null;
        si.point2pointgeom = [];
        if (backbonedata.StartTmpLine != undefined && backbonedata.StartTmpLine != null) { backbonedata.StartTmpLine.setMap(null); backbonedata.StartTmpLine = null; }
        if (backbonedata.EndTmpLine != undefined && backbonedata.EndTmpLine != null) { backbonedata.EndTmpLine.setMap(null); backbonedata.EndTmpLine = null; }
        // Clear previously drawn polylines from the map
        if (si.backboneself && Array.isArray(si.backboneself.polylines) && si.backboneself.polylines.length > 0) {
            si.backboneself.polylines.forEach(line => {
                if (line && line.setMap) {
                    line.setMap(null);
                }
            });
            si.backboneself.polylines = [];
        }

        //const $thead = $('#nearestSitesTable thead');
        //const $tbody = $('#nearestSitesTable tbody');
        //$thead.hide();  // hide header when no data
        //$tbody.empty();
        //$tbody.append('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');
        ajaxReq('BackBonePlan/GetDraftLineGeometry', { planId: planId }, false, function (geometryList) {
            geometryList.forEach(feature => {
                const geo = JSON.parse(feature.geojson); // parse the GeoJSON string

                if (geo.type === 'LineString') {
                    const path = geo.coordinates.map(coord => new google.maps.LatLng(coord[1], coord[0]));
                    si.backbonePolyline = si.backboneself.createAutoPlanLine(path, false, false, 3);
                    si.backbonePolyline.setMap(si.map);

                    si.backboneself.polylines = si.backboneself.polylines || [];
                    si.backboneself.polylines.push(si.backbonePolyline);
                }
            });
        }, false, true, false);
    }

    this.SuggestedRoute = function () {
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
 

    this.onChangeEditPath = function (input) {
        app.Network_Path = input;
        app.createFullNetwork();
        //app.networkPlanningmode();
    }

    this.networkPlanningmode = function () {
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
    this.CheckLoopRequired = function () {

     
        var is_loop_required = $("input[name='is_loop_required']:checked").val();
        if (is_loop_required == "True") {
            $('#DvloopLength').show();
            $('#ManageLoop').show();
            return true;
        }
        else {
            $('#DvloopLength').hide();
            $('#ManageLoop').hide();
            $('#loop_length').val(0);
            return false;
        }
    }
    this.Manage_Loop = function () {
        if (isNaN($('#BomDetails').html())) {
            var loop_length = $('#loop_length').val();
            let temp_Plan_id = $('#plan_id').val();
            popup.LoadModalDialog(si.ParentModel, 'BackBonePlan/GetLoopManage', { planid: temp_Plan_id, looplength: loop_length, is_loop_updated: true }, 'Loop Management', 'modal-lg');
        }
        else {
            alert("First get BOM/BOQ");
        }

    }
    this.loopDetails = function () {

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
    //this.updateSelectedSites = function () {
    //    debugger;
    //    let sites = $("#nearestSites").val();
    //    $('#selectedSites').val(sites);

    //}
     this.bindNearestSites = function() {
         let selected = [];         
        $('.row-checkbox:checked').each(function () {
            let networkId = $(this).val();

            // Get corresponding dropdown using network ID
            let dropdown = $("select.sprout-dropdown[data-network-id='" + networkId + "']");

            if (dropdown.length > 0) {
                let selectedType = dropdown.val();

                if (selectedType && selectedType !== "") {
                    selected.push(networkId + '&' + selectedType);
                }
            }
        });

         $("#selectedSites").val(selected.join(','));
    }

}

function ShowAutoPlanLineLength(_path) {
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





