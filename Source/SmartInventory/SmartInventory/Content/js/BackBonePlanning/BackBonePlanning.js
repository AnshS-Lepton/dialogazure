
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
        if (si.backboneself && Array.isArray(si.backboneself.polylines) && si.backboneself.polylines.length > 0) {
            si.backboneself.polylines.forEach(line => {
                if (line && line.setMap) {
                    line.setMap(null);
                }
            });
            si.backboneself.polylines = [];
        }
        $('#plan_name').val('');
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

    this.PlanningBufferPoint = function () {
        let buffer = parseFloat($('#planbuffer').val());
        let geom = $('#geometry').val();
        let startPointNetworkId = $('#startpoint_network_id').val();
        let endPointNetworkId = $('#endpoint_network_id').val();
        $('#siteDropdownToggle').val('--Select Site--');
        $('#selectAll').prop('checked', false);
        $('.rowCheckbox').prop('checked', false);
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

                    app.GetNearByEntitiesBySiteLatLong(app.NetworkStartPoint, 'start_point')
                        .then(function (isSite) {
                            if (isSite) {
                                app.P2PNetworkManual(destination_point);
                                app.ResetBomDetails();
                            }
                        })
                        .catch(function (err) {
                            console.error("Error during GetNearByEntitiesBySiteLatLong:", err);
                        });
                });
            }

            if (destination_point === 'end') {
                $("#endpointmap").addClass('activemarker activeicon');

                google.maps.event.addListener(si.map, 'click', function (f) {
                    app.endLatLng = f.latLng;
                    var endLatLog = f.latLng.lat().toFixed(6) + "," + f.latLng.lng().toFixed(6);
                    app.NetworkEndPoint = f.latLng.lat() + "," + f.latLng.lng();

                    app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, 'end_point')
                        .then(function (isSite) {
                            if (isSite) {
                                app.P2PNetworkManual(destination_point);
                                app.ResetBomDetails();
                            }
                        })
                        .catch(function (err) {
                            console.error("Error during GetNearByEntitiesBySiteLatLong:", err);
                        });
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
        debugger;
        app.MarkerList = [];
        // app.ResetOffSet();

        var startlat = parseFloat(NetworkStartPoint.lat());
        var startlong = parseFloat(NetworkStartPoint.lng());
        var startLatlng = { lat: startlat, lng: startlong };

        if (si.startMarker)
            si.startMarker.setMap(null);
        si.startMarker = app.createAutoMarker(startLatlng, 'Content/images/Actual_Start.png', app.NP.end_type.START);
        si.startMarker.addListener('dragend', function (event) {
            app.NetworkStartPoint = event.latLng.lat() + "," + event.latLng.lng();

            app.GetNearByEntitiesBySiteLatLong(app.NetworkStartPoint, 'start_point')
                .then(function (isSite) {
                    if (isSite) {
                        app.P2PNetworkManual('start');
                        app.ResetBomDetails();
                    }
                })
                .catch(function (err) {
                    console.error("Error fetching nearby start site:", err);
                });
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
            app.NetworkEndPoint = event.latLng.lat() + "," + event.latLng.lng();

            app.GetNearByEntitiesBySiteLatLong(app.NetworkEndPoint, 'end_point')
                .then(function (isSite) {
                    if (isSite) {
                        app.P2PNetworkManual('end');
                        app.ResetBomDetails();
                    }
                })
                .catch(function (err) {
                    console.error("Error fetching nearby end site:", err);
                });
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
            debugger;

            ShowAutoPlanLineLength();
        });
        google.maps.event.addListener(si.gMapObj.infoEntity.getPath(), 'set_at', function (indx) {
            debugger;

            OldendIndex = si.gMapObj.libPath.length - 1;
            var newPath = si.gMapObj.infoEntity.getPath();
            var newLibPath = newPath.getArray();
            app.deleteAllMiddleMarker();
            backbonedata.ResetBomDetails();

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
            backbonedata.ResetBomDetails();

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
        if (si.backboneself && Array.isArray(si.backboneself.polylines) && si.backboneself.polylines.length > 0) {
            si.backboneself.polylines.forEach(line => {
                if (line && line.setMap) {
                    line.setMap(null);
                }
            });
            si.backboneself.polylines = [];
        }

        const $thead = $('#nearestSitesTable thead');
        const $tbody = $('#nearestSitesTable tbody');
        $thead.hide();  // hide header when no data
        $tbody.append('<tr><td colspan="4" style="text-align:center;">-- No Entity Found --</td></tr>');
        ajaxReq('BackBonePlan/GetDraftLineGeometry', { planId: planId }, false, function (geometryList) {
            geometryList.forEach(feature => {
                const geo = JSON.parse(feature.geojson); // parse the GeoJSON string

                if (geo.type === 'LineString') {
                    debugger;
                    const path = geo.coordinates.map(coord => new google.maps.LatLng(coord[1], coord[0]));
                    si.backbonePolyline = si.backboneself.createAutoPlanLine(path, true, false, 3);
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

    this.GetNearByEntitiesBySiteLatLong = function (geom, end_point_type) {
        return new Promise((resolve, reject) => {
            let geomObj = geom.split(",");
            let lat = parseFloat(geomObj[0]);
            let lng = parseFloat(geomObj[1]);
            const latLng = new google.maps.LatLng(lat, lng);

            si.collapseRemove();
            si.map.setCenter(latLng);
            si.map.setZoom(20);
            $('#lengthAutoSitePlaningDiv').text('');

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
                            if (end_point_type === "start_point") {
                                $('#start_point').val(latLng.lat().toFixed(6) + "," + latLng.lng().toFixed(6));
                                $('#startpoint_network_id').val(podEntity.common_name);
                            } else {
                                $('#end_point').val(latLng.lat().toFixed(6) + "," + latLng.lng().toFixed(6));
                                $('#endpoint_network_id').val(podEntity.common_name);
                            }
                            resolve(true);
                            return;
                        }
                    }

                    // If no site found
                    $('#start_point').val('');
                    $('#end_point').val('');
                    $('#cable_length').val('');
                    if (end_point_type === "start_point") {
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

    ajaxReq('Plan/GetNetworkPlanningLineLength', { geom: $('#geometry').val() }, true, function (resp) {
        distance = resp.result;
        $('#cable_length').val(parseFloat(distance.toFixed(2)));
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





