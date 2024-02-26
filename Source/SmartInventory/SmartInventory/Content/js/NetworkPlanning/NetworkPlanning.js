
var NetworkPlanning = function () {
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
        "closePlanTool": "#closePlanTool",
        "divPlanTool": "#divPlanTool",
        "dvAutoPlanData": '#dvAutoPlanData',
        //"OpenCloseExternalView": '#closeExternalData,#openExternalData',
        "OpenCloseNetworkPlanningView": '#closeNetworkPlanningData, #openNetworkPlanningData',
        "end_type": { 'START': 'start', 'END': 'end' },
    }
    this.initApp = function () {
        this.bindEvents();
    }
    this.bindEvents = function () {
        $(app.NP.OpenCloseNetworkPlanningView).unbind('click');
        $(app.NP.OpenCloseNetworkPlanningView).on("click", function () {
            $(app.NP.dvAutoPlanData).animate({
                width: "toggle"
            }, function () {
                if ($(this).css('display') === 'none') {
                    $("#closeNetworkPlanningData").hide();
                    $("#openNetworkPlanningData").show();
                }
                else {
                    $("#closeNetworkPlanningData").show();
                    $("#openNetworkPlanningData").hide();
                }
            });
        });


        $(app.NP.closePlanTool).on("click", function () {
             
            //$(app.NP.divPlanTool).hide();
            $("#dvAutoPlanData").slideToggle('slow', function () {
                 
                si.ActiveNetworkPlanning = false;
                if (app.autoplanningplanid > 0 || si.autoplanid > 0) {
                    si.autoplanid = 0;
                    app.autoplanningplanid = 0;
                    si.LoadLayersOnMap();
                }
                $(app.NP.divPlanTool).hide();
                app.hideAllNetworkFile();
                removeOldMarkers();
                $('.Plantool').removeClass('activeToolBar');

            });
        });
    }

    this.hideAllNetworkFile = function () {
         
        app.deleteAllMiddleMarker();
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
        if (networkdata.StartTmpLine != undefined && networkdata.StartTmpLine != null) { networkdata.StartTmpLine.setMap(null); networkdata.StartTmpLine = null; }
        if (networkdata.EndTmpLine != undefined && networkdata.EndTmpLine != null) { networkdata.EndTmpLine.setMap(null); networkdata.EndTmpLine = null; }
    }

    this.ResetPlanForm = function () {
         
        app.temp_gemo = '';
        $('#geometry').val('');
        $('#lengthAutoPlaningDiv').empty()
        networkdata.NetworkStartPoint = null;
        networkdata.NetworkEndPoint = null;
        $('#ddledit_path').val('google').trigger("chosen:updated");
        $('#pole_manhole_distance').val('');
        $('#cable_length').val('');
        $('#start_point').val('');
        $('#end_point').val('');
        $('.mainPart').hide();
        $('#end_point_buffer').val(defaultBufer);
        $('#cable_type').val('').trigger("chosen:updated");
        $('#is_create_trench').prop('checked', false);
        $('#is_create_duct').prop('checked', false);
        $('#trenchduct').hide();
        $('#loop_length').val(0);
        $('#DvloopLength').hide();
        $('#is_loop_requiredNo').prop('checked', true);
        $('#is_loop_requiredYes').prop('checked', false);
        app.ResetOffSet();
        $('#ddlend_point_type').val('').trigger("chosen:updated");
        $("#startpointmap").removeClass('activemarker');
        $("#endpointmap").removeClass('activemarker');
        $("#btnBomCable").attr("disabled", false);
        networkdata.RestInputPicker();
        networkdata.hideAllNetworkFile();
        app.ResetBomDetails();
        si.gMapObj.infoEntity = null;
        si.gMapObj.libPath = null;
        si.point2pointgeom = [];
        $('#temp_plan_id').val(0);
        $('#ddledit_path').attr('disabled', false).trigger("chosen:updated");
        if (networkdata.StartTmpLine != undefined && networkdata.StartTmpLine != null) { networkdata.StartTmpLine.setMap(null); networkdata.StartTmpLine = null; }
        if (networkdata.EndTmpLine != undefined && networkdata.EndTmpLine != null) { networkdata.EndTmpLine.setMap(null); networkdata.EndTmpLine = null; }
    }

    this.ResetOffSet = function () {
         
        $('#is_offset_requiredYes').prop('checked', false);
        //app.CheckOffSetRequired();
        $('#is_offset_requiredRight').prop('checked', true);
        $('#is_offset_requiredNo').prop('checked', true);
        $('#offset_value').val(0);
        $('#optionDiv').hide();
        app.temp_gemo = '';

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

    this.CheckOffSetRequired = function () {
         
        var is_offset_required = $("input[name='is_offset_required']:checked").val();
        if (is_offset_required == "True") {
            $('#optionDiv').show();
        }
        else {
            $('#optionDiv').hide(); $('#offset_value').val(0);
            if (app.temp_gemo != '') { app.createOffsetCable(); }
            else {
                app.P2PNetworkManual();
            }

            $('#is_offset_requiredRight').prop('checked', true);
        }
    }


    this.ResetBomDetails = function () {
         
        $("#BomDetails").empty();
        $("#btnProcessPlan").attr("disabled", true);
        $("#ManageLoop").attr("disabled", true);
        app.CheckLoopRequired();

    }

    this.ViewAutoPlanningData = function () {
         
       
        if (app.autoplanningplanid > 0 || si.autoplanid > 0) {
            si.autoplanid = 0;
            app.autoplanningplanid = 0;
            si.LoadLayersOnMap();
        }
        $("#planInfo").html("");
        ajaxReq('Plan/GetPlanData', {}, true, function (resp) {
            $("#planInfo").html(resp);
            // $("#planInfo").css('padding-left', '11px');
            //$("#planInfo").css('padding-top', '10px');
            //$("#planInfo").css('background-image', 'none');
        }, false, false);
    }

    this.CheckLoopLenght = function (event) {

        var cableLength = parseFloat($('#cable_length').val());
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

    this.Manage_Loop = function () {
        if (isNaN($('#BomDetails').html())) {
            var temp_Plan_id = $('#temp_plan_id').val();
            var loop_length = $('#loop_length').val();
            var is_loop_updated = $('#is_loop_update').val();
            popup.LoadModalDialog(si.ParentModel, 'Plan/GetLoopManage', { tempPlanid: temp_Plan_id, looplength: loop_length, is_loop_updated: is_loop_updated }, 'Loop Management', 'modal-lg');
        }
        else {
            alert("First get BOM/BOQ");
        }

    }

    this.loopValidation = function () {
        if (!app.networkPlanningManualmode('start', false)) {
            return false;
        }

        if (!app.networkPlanningManualmode('end', false)) {
            return false;
        }


        var cableLength = parseFloat($('#cable_length').val());
        var loop_length = parseFloat($('#loop_length').val());
        var is_loop_required = $("input[name='is_loop_required']:checked").val();

        if (loop_length <= 0 && is_loop_required == "True") {
            $('#loop_length').addClass('form-control input-validation-error');
            return false;
        }
        if (loop_length >= cableLength) {
            alert("Loop length cannot be greater and equal than Cable Drum Length!");
            $('#loop_length').addClass('form-control input-validation-error');
            return false;
        }
        return true;
    }

    this.autoPlanningEndPoint = function () {

        var MinAutoPlanEndPointBuffer = parseFloat($('#MinAutoPlanEndPointBuffer').val());
        var MaxAutoPlanEndPointBuffer = parseFloat($('#MaxAutoPlanEndPointBuffer').val());
        var end_point_buffer = parseFloat($('#end_point_buffer').val());

        if (end_point_buffer >= MinAutoPlanEndPointBuffer && end_point_buffer <= MaxAutoPlanEndPointBuffer) {
            $('#lengthAutoPlaningDiv').empty();
            app.ResetBomDetails();
            app.RestInputPicker();
            this.NetworkEndPoint = null;
            app.hideAllNetworkFile();
            app.P2PNetworkManual('start');
            if (si.gMapObj.infoEntity != undefined) { si.gMapObj.infoEntity.setMap(null); }
            if (app.NetworkStartPoint != null) {
                app.createBufferCircle();
                var startlat = parseFloat(app.NetworkStartPoint.split(',')[0]);
                var startlong = parseFloat(app.NetworkStartPoint.split(',')[1]);

                var entity_type = $('#ddlend_point_type option:selected').val();
                if (entity_type != '') {
                    ajaxReq('plan/GetEndPointEntity', { lat: startlat, lng: startlong, entity_type: entity_type, buffer: end_point_buffer }, false, function (resp) {
                        if (resp.msg == "true") {
                            if (resp.Data.length > 0) {
                                var jsonData = $.parseJSON(resp.Data);
                                $('#demo').inputpicker({
                                    data: jsonData,
                                    fields: [
                                        { name: 'distance', text: 'Distance (m)' },
                                        { name: 'network_id', text: 'Network Id' },
                                        // { name: 'system_id', text: 'system_id' },
                                        { name: 'entity_name', text: 'Name' },
                                        { name: 'lat_long', text: 'lat long' }
                                    ],
                                    headShow: true,
                                    fieldText: 'network_id',
                                    fieldValue: 'system_id',
                                });
                                $('.inputpicker-input').val("--Select--");
                            }
                            else {
                                $('#demo').val("--No Entity found--");
                            }

                        }
                        else {
                            $('#demo').val("--No Entity found--");
                        }
                    }, true, true);
                }
            }
        }
    }

    this.RestInputPicker = function () {
        if ($('#demo').hasClass("inputpicker-original")) {
            $('#demo').inputpicker('destroy');
            $('#demo').val('--Select--');
            $('#demo').removeAttr("tabindex");
            $('#demo').removeClass("inputpicker-original");
            $('#demo').removeAttr("style");
            $('.inputpicker-div').remove();
            $('.inputpicker-overflow-hidden').remove();
        }
        $('#end_point').val('');
        app.NetworkEndPoint = null;
    }

    this.createAutoPlanNetwork = function () {
         
        if (app.selectedPlanningPath != null && app.selectedPlanningPath.length == undefined) {

            for (let j = 0; j < app.selectedPlanningPath.legs.length; j++) {
                for (var k = 0; k < app.selectedPlanningPath.legs[j].steps.length; k++) {
                    console.log(app.selectedPlanningPath.legs[j].steps[k].start_location.lng() + " " +
                        app.selectedPlanningPath.legs[j].steps[k].start_location.lat());
                    var manhole_geom = app.selectedPlanningPath.legs[j].steps[k].start_location.lng() + " " +
                        app.selectedPlanningPath.legs[j].steps[k].start_location.lat();

                    ajaxReq('Library/SaveManhole', {
                        geom: manhole_geom, networkIdType: 'A', isDirectSave: true
                    }, true, function (response) {
                        console.log(response + "1");
                    });
                    if (k == app.selectedPlanningPath.legs[j].steps.length - 1) {
                        var manhole_geom2 = app.selectedPlanningPath.legs[j].steps[k].end_location.lng() + " " +
                            app.selectedPlanningPath.legs[j].steps[k].end_location.lat();
                        ajaxReq('Library/SaveManhole', {
                            geom: manhole_geom2, networkIdType: 'A', isDirectSave: true
                        }, true, function (response) {
                            console.log(response + "2");
                        });
                    }
                }
            }
        }
        else {
            for (let i = 0; i < app.selectedPlanningPath.length; i++) {
                for (let j = 0; j < app.selectedPlanningPath[i].legs.length; j++) {
                    for (var k = 0; k < app.selectedPlanningPath[i].legs[j].steps.length; k++) {
                        console.log(app.selectedPlanningPath[i].legs[j].steps[k].start_location.lng() + " " +
                            app.selectedPlanningPath[i].legs[j].steps[k].start_location.lat());
                        var manhole_geom = app.selectedPlanningPath[i].legs[j].steps[k].start_location.lng() + " " +
                            app.selectedPlanningPath[i].legs[j].steps[k].start_location.lat();
                        ajaxReq('Library/SaveManhole', {
                            geom: manhole_geom, networkIdType: 'A', isDirectSave: true
                        }, true, function (response) {
                            console.log(response + "1");
                        }); if (k == app.selectedPlanningPath[i].legs[j].steps.length - 1) {
                            var manhole_geom2 = app.selectedPlanningPath[i].legs[j].steps[k].end_location.lng() + " " +
                                app.selectedPlanningPath[i].legs[j].steps[k].end_location.lat();
                            ajaxReq('Library/SaveManhole', {
                                geom: manhole_geom2, networkIdType: 'A', isDirectSave: true
                            }, true, function (response) {
                                console.log(response + "2");
                            });
                        }
                    }
                }
            }
        }



        ajaxReq('Plan/SaveProcess', $('form').serialize(), true, function (resp) {
            app.autoPlanningShowNetworkLayer(resp);
        }, false, true, false);
    }

    this.geomToForm = function () {
        if ($('#ddledit_path').val() == "manually") {
            si.point2pointgeom = [];
            var path = si.gMapObj.libPath;
            for (ll of path) {
                si.point2pointgeom.push(ll.lng() + ' ' + ll.lat());
            }
        }
        $('#geometry').val(si.point2pointgeom);
    }

    this.GetTempCableLengthGemo = function () {
         
        /* app.geomToForm();*/
        ajaxReq('Plan/GetTempCableLengthGemo', $('form').serialize(), true, function (resp) {
            const flightPlanCoordinates = [];
            if (resp.status.toLowerCase() == "ok") {

                var gemo = resp.result[0].geometry.split(',');
                for (i = 0; i < gemo.length; i++) {
                    var lng_lat = gemo[i];
                    flightPlanCoordinates.push(new google.maps.LatLng(lng_lat.split(' ')[1], parseFloat(lng_lat.split(' ')[0])));
                }
                directionsDisplay.setMap(null);
                //app.CreateplanCable(flightPlanCoordinates);

                app.networkPlanningcable(flightPlanCoordinates);
                $('#geometry').val(resp.result[0].geometry);
            }
        }, false, true, false);
    }

    this.autoPlanningShowNetworkLayer = function (resp) {
        if (resp.objPM.status == "True") {
            alert(resp.objPM.message);
            app.autoplanningPlanId = resp.planid;
            //$('.lyrRefresh').trigger("click");
            networkdata.ShowPlanEntityOnMap(resp.planid);
            app.hideAllNetworkFile();
            $("#btnProcessPlan").attr("disabled", true);
            $("#btnBomCable").attr("disabled", true);
            $("#ManageLoop").attr("disabled", true);
        }
        else {
            alert(resp.objPM.message);
        }
    }

    this.autoPlanningCableType = function () {
        $('#trenchduct').hide();
        $('#is_create_trench').prop('checked', false);
        $('#is_create_duct').prop('checked', false);
        var cabletype = $('#cable_type').val();
        $('#txtdistance').html('');
        if (cabletype == "Underground") {
            $('#ddledit_path').val('google').trigger("chosen:updated");
            $('#trenchduct').show();
            $('#ddledit_path').attr('disabled', false).trigger("chosen:updated");
            $('#txtdistance').append(MultilingualKey.SI_OSP_GBL_JQ_FRM_217+" Span(m)<i class='clsMandatory'>*</i>:");
        }
        else if (cabletype == "Overhead") {
            $('#ddledit_path').val('manually');
            $('#ddledit_path').attr('disabled', true).trigger("chosen:updated");
            $('#trenchduct').hide(); $('#txtdistance').append("Pole Span(m)<i class='clsMandatory'>*</i>:");
        }
        else { $('#txtdistance').append("Span(m)<i class='clsMandatory'>*</i>:"); }
        this.createFullNetwork();
    }

    this.createFullNetwork = function () {

        si.Network_Path = $('#ddledit_path').val();
        app.createCableBetweenMakers();
        app.createStartMarker();
        app.createEndMarker();
    }



    this.ShowPlanEntityOnMap = function (planId) {
        $('.pull-right').find('.activemarker').removeClass('activemarker')
        $('.glyphicon glyphicon-eye-open').removeClass('activemarker');
        ajaxReq('Plan/GetNetworkForMap', { plan_id: planId }, true, function (resp) {
            if (resp.status.toLowerCase() == "ok") {
                $('#dvHighlight' + '_' + planId).addClass('activemarker');
                app.autoplanid = planId;
                si.autoplanid = planId;
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

    this.ShowTempPlanEntityOnMap = function (planId) {

        const flightPlanCoordinates = [];
        ajaxReq('Plan/GetTempNetworkForMap', { plan_id: 1 }, true, function (resp) {

            if (resp.status.toLowerCase() == "ok") {
                var gemo = resp.result[0].geometry.split(',');
                for (i = 0; i < gemo.length; i++) {

                    var lng_lat = gemo[i];
                    flightPlanCoordinates.push(new google.maps.LatLng(lng_lat.split(' ')[1], parseFloat(lng_lat.split(' ')[0])));
                }
                directionsDisplay.setMap(null);
                app.networkPlanningcable(flightPlanCoordinates);
            }
        }, true, true);
    }



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


    this.deletePlan = function (planId) {
        showConfirm(MultilingualKey.SI_OSP_GBL_GBL_FRM_157, function () {
            ajaxReq('plan/DeletePlanByPlanId', { plan_id: planId }, true, function (resp) {
                if (resp.msg.toLowerCase() == "ok") {
                    alert(resp.strReturn);
                    $("#dv_kml_" + planId).remove();
                    //app.hideAllNetworkFile();
                    //app.RemoveOldFeature();
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


    this.downloadBomReport = function (planId) {
        window.location = appRoot + 'Report/ExportPlanBOMBOQReport?plan_id=' + planId;
    }

    this.loopUpdate = function () {
        $('#is_loop_update').val(true);
    }

    this.networkPlanningManualmode = function (end, is_marker_create) {

        var EntityPoint = $("#" + end + "_point").val();

        if (EntityPoint == '') {
            return false;
        }
        else if (EntityPoint.indexOf(',') > -1) {
            if (end == app.NP.end_type.START && is_marker_create) {
                EntityPoint = $("#start_point").val();
                app.NetworkStartPoint = EntityPoint;
                app.P2PNetworkManual('start');

            }
            else if (end == app.NP.end_type.END && is_marker_create) {
                EntityPoint = $("#end_point").val();
                app.NetworkEndPoint = EntityPoint;
                app.P2PNetworkManual('end');
            }
        }
        else {
            if (end == app.NP.end_type.START && si.startMarker) {
                si.startMarker.setMap(null);
            }
            else if (end == app.NP.end_type.END && si.endMarker) {
                si.endMarker.setMap(null);
            }
            if (si.gMapObj.infoEntity != undefined) { si.gMapObj.infoEntity.setMap(null); }
            si.clearMarkers();
            removeOldMarkersWithRemoveActive();
            alert("Invalid " + end + " point please enter valid (Latitude,Longitude)");
            return false;
        }
        if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
            app.fitElementOnMap(app.NetworkStartPoint, app.NetworkEndPoint);
            si.map.setZoom(si.map.getZoom() - 1);
        }
        return true;
    }

    this.NetworkPlanning = function (ismappicker, destination_point) {

        var cableType = $('#cable_type').val();
        if (cableType == '') {
            alert('Please Select Cable Type');
            return false;
        }

        if (ismappicker) {
            if (destination_point == 'start') {
                $("#startpointmap").addClass('activemarker');
                google.maps.event.addListener(si.map, 'click', function (e) {
                    app.startLatLng = e.latLng;
                    var startLatLog = e.latLng.lat().toFixed(6) + "," + e.latLng.lng().toFixed(6);
                    app.NetworkStartPoint = e.latLng.lat() + "," + e.latLng.lng();
                    $('#start_point').val(startLatLog);
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
                    $('#end_point').val(endLatLog);
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
                 
                app.ResetOffSet();
                app.ResetBomDetails();
            });
            si.startMarker.addListener('dragend', function (startLatlng) {
                if (app.plan_mode == 'auto') {
                    app.autoPlanningEndPoint();
                }
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
            si.endMarker.addListener('drag', function (endLatlng) {
                 
                app.ResetOffSet();
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

    //this.createCableBetweenMakers = function () {

    //    if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
    //        if (networkdata.StartTmpLine != undefined && networkdata.StartTmpLine != null) { networkdata.StartTmpLine.setMap(null); networkdata.StartTmpLine = null; }
    //        if (networkdata.EndTmpLine != undefined && networkdata.EndTmpLine != null) { networkdata.EndTmpLine.setMap(null); networkdata.EndTmpLine = null; }
    //        if ($("input[name='is_offset_required']:checked").val() == "True") {   app.ResetOffSet();}

    //        app.startLatLng = { lat: parseFloat(app.NetworkStartPoint.split(',')[0]), lng: parseFloat(app.NetworkStartPoint.split(',')[1]) };
    //        app.endLatLng = { lat: parseFloat(app.NetworkEndPoint.split(',')[0]), lng: parseFloat(app.NetworkEndPoint.split(',')[1]) };
    //        var network_Type = $('#ddledit_path').val();
    //        if (network_Type == "manually") {
    //            const flightPlanCoordinates = [];
    //            flightPlanCoordinates.push(new google.maps.LatLng(app.startLatLng.lat, parseFloat(app.startLatLng.lng)));
    //            flightPlanCoordinates.push(new google.maps.LatLng(app.endLatLng.lat, parseFloat(app.endLatLng.lng)));
    //            app.networkPlanningcable(flightPlanCoordinates);
    //        }
    //            else {

    //                var request = {
    //                    origin: app.startLatLng,
    //                    travelMode: google.maps.DirectionsTravelMode.DRIVING,
    //                    provideRouteAlternatives: true,

    //                };
    //                request.destination = app.endLatLng;
    //                app.CheckCableCreated();
    //                directionsService.route(request, function (response, status) {
    //                    if (status == google.maps.DirectionsStatus.OK) {
    //                         
    //                        var latLngArr = google.maps.geometry.encoding.decodePath(response.routes[0].overview_polyline);

    //                        directionsDisplay.setOptions({ suppressMarkers: true });
    //                        directionsDisplay.setDirections(response);
    //                        directionsDisplay.setMap(si.map);

    //                        var endIndex = latLngArr.length - 1;

    //                        const startPoint = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
    //                        const startPointLineArr = [];
    //                        startPointLineArr.push(latLngArr[0]);
    //                        startPointLineArr.push(startPoint);
    //                        app.StartTmpLine = app.createAutoPlanLine(startPointLineArr, false);
    //                        app.StartTmpLine.setMap(si.map);


    //                        const EndPoint = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
    //                        const EndPointLineArr = [];
    //                        EndPointLineArr.push(latLngArr[endIndex]);
    //                        EndPointLineArr.push(EndPoint);
    //                        app.EndTmpLine = app.createAutoPlanLine(EndPointLineArr, false);
    //                        app.EndTmpLine.setMap(si.map);

    //                        app.createDirectionMarker(startPoint, EndPoint);
    //                        latLngArr.splice(0, 0, startPoint);
    //                        latLngArr.push(EndPoint);

    //                        ShowAutoPlanLineLength(latLngArr);
    //                    }
    //                    else {
    //                        alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_016);//Directions not found between source and destination.
    //                    }
    //                });
    //        }



    //    }
    //}

    this.createCableBetweenMakers = function () {
        app.AllPlanningPaths = [];
        app.AllDistances = [];
        app.minDistance = 0;
        app.AllPathResponses = [];
        directionsDisplay.setMap(null);
        //$('#suggestBox').hide();

        //if (si.startMarker)
        //    si.startMarker.setMap(null);
        if (app.NetworkStartPoint != null && app.NetworkEndPoint != null) {
            app.ResetOffSet();
            app.startLatLng = { lat: parseFloat(app.NetworkStartPoint.split(',')[0]), lng: parseFloat(app.NetworkStartPoint.split(',')[1]) };
            app.endLatLng = { lat: parseFloat(app.NetworkEndPoint.split(',')[0]), lng: parseFloat(app.NetworkEndPoint.split(',')[1]) };
            if (networkdata.StartTmpLine != undefined && networkdata.StartTmpLine != null) { networkdata.StartTmpLine.setMap(null); networkdata.StartTmpLine = null; }
            if (networkdata.EndTmpLine != undefined && networkdata.EndTmpLine != null) { networkdata.EndTmpLine.setMap(null); networkdata.EndTmpLine = null; }
            if ($("input[name='is_offset_required']:checked").val() == "True") {   app.ResetOffSet(); }

            var request = {
                origin: app.startLatLng,
                travelMode: google.maps.DirectionsTravelMode.DRIVING,
                provideRouteAlternatives: true
            };

            request.destination = app.endLatLng;
            app.CheckCableCreated();
            var network_Type = $('#ddledit_path').val();

            directionsService.route(request, function (response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                     
                    app.AllPathResponses.push(response);
                    var Paths = response.routes;
                    for (let i = 0; i < Paths.length; i++) {
                        let totalDist = 0;

                        //for (var item in route) {
                        //    for (i = 0; i < item.legs.length; i++) {
                        for (let j = 0; j < Paths[i].legs.length; j++) {
                            totalDist += Paths[i].legs[j].distance.value;
                        }
                        app.AllDistances.push({ 'overview_polyline': Paths[i].overview_polyline, 'route_distance': totalDist, 'Is_Start': true });
                        if (app.minDistance != 0 && app.minDistance > totalDist) {
                            app.minDistance = totalDist;
                            app.AllPlanningPaths.push(Paths[i]);
                            //app.BindPlanningPaths(app.AllPlanningPaths, false);

                        }
                        else if (app.minDistance == 0) {
                            app.minDistance = totalDist;
                             
                            app.AllPlanningPaths.push(Paths[i]);
                            //app.BindPlanningPaths(app.AllPlanningPaths, false);
                        }
                        if (app.AllPlanningPaths.length > 0) {
                            app.BindPlanningPaths(app.AllPlanningPaths, true);
                        }

                        //}
                        //app.AllPlanningPaths.push(Paths[i]);


                    }


                    // if (network_Type != "manually") {
                    // directionsDisplay.setOptions({ suppressMarkers: true });
                    // directionsDisplay.setDirections(response);
                    // directionsDisplay.setMap(si.map);
                    // app.BindPlanningPaths(app.AllPlanningPaths, true);
                    // }
                    //result = response.filter(x => x.route == 1);
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
                     
                    if (network_Type != "manually") {
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

                            //}

                        }
                        //if (directionsDisplay)
                        //directionsDisplay.setMap(null);
                        // directionsDisplay.setOptions({ suppressMarkers: true });
                        // directionsDisplay.setDirections(response);
                        // directionsDisplay.setMap(si.map);
                        // app.BindPlanningPaths(app.AllPlanningPaths, true);


                        $("#is_bomboq_reqested").val(false);

                        //var latLngArr = google.maps.geometry.encoding.decodePath(response.routes[0].overview_polyline);
                        //var network_Type = $('#ddledit_path').val();

                    }

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
            console.log(result[0]);

            // if (app.AllPathResponses.length == 1) {
            // directionsDisplay.setMap(null);
            // directionsDisplay.setOptions({ suppressMarkers: true });
            // directionsDisplay.setDirections(app.AllPathResponses[0]);
            // directionsDisplay.setMap(si.map);

            // }
            // else {
            // for (let i = 0; i < app.AllPathResponses.length; i++) {
            // for (let j = 0; j < app.AllPathResponses[i].routes.length; j++) {
            // if (app.AllPathResponses[i].routes[j].overview_polyline == result[0].overview_polyline)
            // var response = app.AllPathResponses[i].routes.filter(x => x.overview_polyline == result[0].overview_polyline);
            // if (response != undefined) {
            // directionsDisplay.setMap(null);
            // directionsDisplay.setOptions({ suppressMarkers: true });
            // directionsDisplay.setDirections(app.AllPathResponses[i]);
            // directionsDisplay.setMap(si.map);
            // }
            // }
            // }
            // }
            //app.AllDistances.sort(function (a, b) { return a.route_distance - b.route_distance });

            // app.BindPlanningPaths(app.AllPlanningPaths, true);
            //result = sortedPlan.routes[0];




            if ($("input[name=suggestRoutes]").length == 0) {
                var route = app.sortedPlan.length;
                if (route > 1) {
                    route = route > 3 ? 3 : route;
                    $('#suggestBox').removeAttr("style");
                    $('.routeBox').removeAttr("style");
                    $('.routeBox').hide();
                    //$('#suggestBox').show();

                    for (var i = 1; i <= route; i++) {
                        $('.routeBox').append('<label class="suggestBoxN">  <input type="radio" name="suggestRoutes" value= "' + i + '" onclick="networkdata.SuggestedRoute(this);" > Route  ' + i + ' <span class="suggestMark"></span></label>');
                    }

                    //$('.routeBox').removeAttr("style");
                    $(".routeBox input:radio:first").attr('checked', true);
                    $('.routeBox').toggle();
                }
            }
            networkdata.SuggestedRoute();


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
        app.ResetOffSet();
        var startlat = parseFloat(NetworkStartPoint.lat());
        var startlong = parseFloat(NetworkStartPoint.lng());
        startLatlng = { lat: startlat, lng: startlong };

        if (si.startMarker)
            si.startMarker.setMap(null);
        si.startMarker = app.createAutoMarker(startLatlng, 'Content/images/Actual_Start.png', app.NP.end_type.START);
        si.startMarker.addListener('dragend', function (startLatlng) {
             


            app.ResetOffSet();
            app.NetworkStartPoint = startLatlng.latLng.lat() + "," + startLatlng.latLng.lng();
            $('#start_point').val(app.NetworkStartPoint);
            if (app.plan_mode == 'auto') {
                app.autoPlanningEndPoint();
            }
            else {
                app.P2PNetworkManual('');
                app.ResetBomDetails();
            }
        });
        si.startMarker.setMap(si.map);
        app.MarkerList.push(si.startMarker);

        //end point
         
        var endlat = parseFloat(NetworkEndPoint.lat());
        var endlong = parseFloat(NetworkEndPoint.lng());
        endLatlng = { lat: endlat, lng: endlong };

        if (si.endMarker)
            si.endMarker.setMap(null);
        si.endMarker = app.createAutoMarker(endLatlng, 'content/images/End.png', app.NP.end_type.END);
        si.endMarker.addListener('dragend', function (endLatlng) {
             

            app.NetworkEndPoint = endLatlng.latLng.lat() + "," + endLatlng.latLng.lng();
            app.P2PNetworkManual('');
            app.ResetBomDetails();
        });
        networkdata.TempNetworkEndPoint = endLatlng.lat + "," + endLatlng.lng;
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
        ShowAutoPlanLineLength(latLngArr);

    }

    this.networkPlanningcable = function (latLngArr, LineEdiTable = true) {
        app.CheckCableCreated();
        si.gMapObj.lineflag = true;
        si.gMapObj.lineEditflag = true;
        si.gMapObj.infoEntity = app.createAutoPlanLine(latLngArr, LineEdiTable);
        si.gMapObj.infoEntity.setEditable(LineEdiTable);
        si.gMapObj.infoEntity.setMap(si.map);
        si.gMapObj.libPath = latLngArr;

        app.initialClickForAutoEditLine();
    }

    this.CheckCableCreated = function () {
        if (networkdata.StartTmpLine != undefined && networkdata.StartTmpLine != null) { networkdata.StartTmpLine.setMap(null); networkdata.StartTmpLine = null; }
        if (networkdata.EndTmpLine != undefined && networkdata.EndTmpLine != null) { networkdata.EndTmpLine.setMap(null); networkdata.EndTmpLine = null; }
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


        google.maps.event.addListener(si.gMapObj.infoEntity, 'rightclick', function (event) {
             
            app.ResetOffSet();
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
        });
        google.maps.event.addListener(si.gMapObj.infoEntity.getPath(), 'set_at', function (indx) {
             
            //app.ResetOffSet();

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

            ShowAutoPlanLineLength();

        });

        google.maps.event.addListener(si.gMapObj.infoEntity.getPath(), 'insert_at', function (indx) {
             
            /* app.ResetOffSet();*/

            var newPath = si.gMapObj.infoEntity.getPath();
            si.gMapObj.libPath = newPath.getArray();
            ShowAutoPlanLineLength();
        });
        google.maps.event.addListener(si.gMapObj.infoEntity, 'click', function (evt) {
             
            app.ResetOffSet();

            var selectPoint = '';
            selectPoint = si.roundNumber(evt.latLng.lng(), 6) + ' ' + si.roundNumber(evt.latLng.lat(), 6);
            var _zoom = si.map.getZoom();

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


    this.planningmode = function (val) {
        app.ResetPlanForm();
        app.plan_mode = val;
        $('.mainPart').show();
        if (app.plan_mode == 'auto') {
            $('.auto').show();
            $('.Manual').hide();
            $('#offsetvalue').show();
            $("#end_point").attr("readonly", true);
            $("#endpointmap").addClass("disable-click");
        }
        else if (app.plan_mode == 'manual_planning') {
            $('.Manual').show();
            $('.auto').hide();
            $('#offsetvalue').show();
            $("#end_point").attr("readonly", false);
            $("#endpointmap").removeClass("disable-click");
        }
        else {
            $('.mainPart').hide();
        }
    }


    this.GetTemNetwork = function (resp) {
         
        const flightPlanCoordinates = [];
        this.deleteAllMiddleMarker();
        if (resp.status.toLowerCase() == "ok") {
            var gemo = resp.result[0].geometry.split(',');
            for (i = 0; i < gemo.length; i++) {

                var lng_lat = gemo[i];
                var lat_lng = new google.maps.LatLng(lng_lat.split(' ')[1], parseFloat(lng_lat.split(' ')[0]));
                flightPlanCoordinates.push(lat_lng);
                var middleMarkers = app.createAutoMarker(lat_lng, 'Content/images/Middle_Point1.png', 'Middle', false);
                app.middleMarkers.push(middleMarkers);
            }

            var requested = $("#is_bomboq_reqested").val();
            if (requested.toLowerCase() == 'false') {
                $("#is_bomboq_reqested").val(true);
            }
            if (requested.toLowerCase() == 'false') {
                if ($('#cable_type').val() == "Overhead") {
                    directionsDisplay.setMap(null);
                    app.networkPlanningcable(flightPlanCoordinates);

                    $('#temp_plan_id').val(resp.result[0].temp_plan_id);
                    $('#geometry').val(resp.result[0].geometry);
                    ShowAutoPlanLineLength(flightPlanCoordinates);
                }
            }
            app.setAllMiddleMarkerOnMap(true);

        }
        app.planBomAndBOQ();
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

    this.planBomAndBOQ = function () {

        $('#is_loop_update').val(false);
        ajaxReq('Plan/GetBomData', $('form').serialize(), true, function (resp) {
            $("#BomDetails").html(resp);
            app.CheckLoopRequired();
        }, false, true, false);

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

    this.SuggestedRoute = function () {
         
        if (networkdata.StartTmpLine != undefined && networkdata.StartTmpLine != null) { networkdata.StartTmpLine.setMap(null); networkdata.StartTmpLine = null; }
        if (networkdata.EndTmpLine != undefined && networkdata.EndTmpLine != null) { networkdata.EndTmpLine.setMap(null); networkdata.EndTmpLine = null; }

        var route = $('input[name="suggestRoutes"]:checked').val();
        if (route == undefined) route = 1;
        for (let i = 0; i < networkdata.AllPathResponses.length; i++) {
            for (let j = 0; j < networkdata.AllPathResponses[i].routes.length; j++) {
                console.log("route s", route);
                console.log("i:" + i + "j:" + j);
                if (networkdata.AllPathResponses[i].routes[j].overview_polyline ==
                    networkdata.sortedPlan[route - 1].overview_polyline) {
                    var response = networkdata.AllPathResponses[i].routes.filter(x => x.overview_polyline ==
                        networkdata.sortedPlan[route - 1].overview_polyline);
                    console.log("ryte response", response);
                    if (response != undefined) {
                        //directionsDisplay.setMap(null);
                        //directionsDisplay.setOptions({ suppressMarkers: true });
                        //directionsDisplay.setDirections(networkdata.AllPathResponses[i]);
                        //directionsDisplay.setMap(si.map);
                        //directionsDisplay.setDirections(networkdata.AllPathResponses[i]);
                        app.selectedPlanningPath = networkdata.AllPathResponses[i].routes[j];
                        console.log("Route Selected ", app.selectedPlanningPath);
                        break;
                    }

                    // app.selectedPlanningPath = result[0];
                }
            }
            if (response != undefined) {
                break;
            }
        }

        $("#is_bomboq_reqested").val(false);
        var latLngArr = google.maps.geometry.encoding.decodePath(app.selectedPlanningPath.overview_polyline);

        var network_Type = $('#ddledit_path').val();

        if (network_Type == "manually") {
            app.CreateplanCable(latLngArr);
        }
        else {
             


            var endIndex = latLngArr.length - 1;
            if (app.sortedPlan[route - 1].Is_Start == true) {
                const startPoint = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
                const startPointLineArr = [];
                startPointLineArr.push(latLngArr[0]);
                startPointLineArr.push(startPoint);
                app.StartTmpLine = app.createAutoPlanLine(startPointLineArr, false, true);
                app.StartTmpLine.setMap(si.map);


                const EndPoint = new google.maps.LatLng(app.endLatLng.lat, app.endLatLng.lng);
                const EndPointLineArr = [];
                for (let i = 0; i < latLngArr.length; i++) {
                    EndPointLineArr.push(latLngArr[i]);
                }
                EndPointLineArr.push(EndPoint);
                app.EndTmpLine = app.createAutoPlanLine(EndPointLineArr, false, true);
                app.EndTmpLine.setMap(si.map);

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
                app.StartTmpLine = app.createAutoPlanLine(startPointLineArr, false, true);
                app.StartTmpLine.setMap(si.map);


                const EndPoint = new google.maps.LatLng(app.startLatLng.lat, app.startLatLng.lng);
                const EndPointLineArr = [];
                for (let i = 0; i < latLngArr.length; i++) {
                    EndPointLineArr.push(latLngArr[i]);
                    //app.EndTmpLine = app.createAutoPlanLine(EndPointLineArr, false, true);
                }
                EndPointLineArr.push(EndPoint);
                app.EndTmpLine = app.createAutoPlanLine(EndPointLineArr, false, true);
                //EndPointLineArr.push(latLngArr[endIndex]);
                //EndPointLineArr.push(EndPoint);

                app.EndTmpLine.setMap(si.map);

                app.createDirectionMarker(startPoint, EndPoint);
                latLngArr.splice(0, 0, startPoint);
                latLngArr.push(EndPoint);

                ShowAutoPlanLineLength(latLngArr);
            }


        }
    };

    this.loopDetails = function () {

        var temp_plan_id = $('#temp_plan_id').val();
        /* $('#temp_plan_id').val(temp_plan_id);*/
        ajaxReq('plan/getLoopLength', { temp_plan_id: temp_plan_id }, false, function (resp) {

            if (resp.status.toLowerCase() == "ok") {
                var length = resp.result.length_qty;
                var cost_per_unit = resp.result.cost_per_unit;
                var service_cost_per_unit = resp.result.service_cost_per_unit;
                var amount = resp.result.amount;
                $('#tblRecurringCharges').find('#Loop > .lngqty').text(length);
                $('#tblRecurringCharges').find('#Loop > .cost_per_unit').text(cost_per_unit);
                $('#tblRecurringCharges').find('#Loop > .service_cost_per_unit').text(service_cost_per_unit);
                $('#tblRecurringCharges').find('#Loop > .amount').text(amount);
                $('#closeModalPopup').trigger("click");
            }
        }, true, false);
    }

    this.createOffsetCable = function () {
         
        //networkdata.geomToForm();
        var cablegemo = $('#geometry').val();
        if (cablegemo != '') {


            if (cablegemo != '') {
                if ($('#offset_value').val() == '') {
                    $('#offset_value').val(0);
                    //networkdata.createOffsetCable();
                    //alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_034);
                    return false;
                }
                else if (parseInt($('#offset_value').val()) > parseInt($('#hdnMaxOffsetNetworkPlanning').val())) {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_107);
                    return false;
                }
                var offSet = '';
                if ($('#is_offset_requiredLeft').prop('checked'))
                    offSet = '-' + parseFloat($('#offset_value').val()) / 100000;
                else
                    offSet = parseFloat($('#offset_value').val()) / 100000;

                if (app.temp_gemo == "") {
                    app.temp_gemo = cablegemo;
                }

                if ($('#offset_value').val() == 0) {
                    var latLngArr = [];
                    var longLatArr = app.temp_gemo.split(',');


                    for (var i = 0; i < longLatArr.length; i++) {
                        var LongLatsingle = longLatArr[i].split(' ');
                        latLngArr.push(new google.maps.LatLng(parseFloat(LongLatsingle[1]), parseFloat(LongLatsingle[0])));
                    }

                    var startlatlng = latLngArr[0].lat().toFixed(6) + ',' + latLngArr[0].lng().toFixed(6);
                    $('#start_point').val(startlatlng);
                    app.NetworkStartPoint = startlatlng;
                    app.createStartMarker();

                    if (app.plan_mode == 'manual_planning') {
                        var endIndex = latLngArr.length - 1;
                        var endLatLngr = latLngArr[endIndex].lat().toFixed(6) + ',' + latLngArr[endIndex].lng().toFixed(6);
                        $('#end_point').val(endLatLngr);
                        app.NetworkEndPoint = endLatLngr;
                        app.createEndMarker();
                    }
                    app.networkPlanningcable(latLngArr);
                    $('#geometry').val(longLatArr);
                    ShowAutoPlanLineLength(latLngArr);
                }
                else {

                    ajaxReq('plan/getOffSetPolyLineCurve', { cablegemo: app.temp_gemo, offset: offSet }, false, function (jSonResp) {
                        var latLngArr = [];
                        var longLat = jSonResp.offsetGeom.substring(jSonResp.offsetGeom.indexOf('(') + 1, jSonResp.offsetGeom.lastIndexOf(')')).replace(/\(/, '').replace(/\)/, '');
                        var longLatArr = longLat.split(',');

                        if ($('#is_offset_requiredLeft').prop('checked')) { longLatArr.reverse(); }

                        if (app.plan_mode == 'auto') {


                            var latlngEndPoint = null;
                            if ($('#ddledit_path').val() == "manually") {
                                latlngEndPoint = app.NetworkEndPoint.split(',');
                            }
                            else {
                                latlngEndPoint = networkdata.TempNetworkEndPoint.split(',');
                            }
                            longLatArr.push(latlngEndPoint[1] + ' ' + latlngEndPoint[0]);
                        }

                        for (var i = 0; i < longLatArr.length; i++) {
                            var LongLatsingle = longLatArr[i].split(' ');
                            latLngArr.push(new google.maps.LatLng(parseFloat(LongLatsingle[1]), parseFloat(LongLatsingle[0])));
                        }

                        var startlatlng = latLngArr[0].lat().toFixed(6) + ',' + latLngArr[0].lng().toFixed(6);
                        $('#start_point').val(startlatlng);
                        app.NetworkStartPoint = startlatlng;
                        app.createStartMarker();

                        if (app.plan_mode == 'manual_planning') {
                            var endIndex = latLngArr.length - 1;
                            var endLatLngr = latLngArr[endIndex].lat().toFixed(6) + ',' + latLngArr[endIndex].lng().toFixed(6);
                            $('#end_point').val(endLatLngr);
                            app.NetworkEndPoint = endLatLngr;
                            app.createEndMarker();
                        }
                        $('#geometry').val(longLatArr);
                        app.networkPlanningcable(latLngArr);
                        ShowAutoPlanLineLength(latLngArr);
                    }, true, false);
                }

            }
        }



    }

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

    this.createBufferCircle = function () {

        var centerPoint = app.NetworkStartPoint;
        var sepVals = centerPoint.split(',');
        if (sepVals.length == 1) {
            sepVals = address.split(' ');
        }
        if (sepVals.length == 2) {
            var lat = parseFloat(sepVals[0]);
            var lng = parseFloat(sepVals[1]);

            if (!isNaN(lat) && !isNaN(lng))
                gcFlag = false;

            if (lat > -90 && lat < 90 && lng > -180 && lng < 180) {

                var addrLocation = new google.maps.LatLng(lat, lng);
                si.map.setCenter(addrLocation);
                //app.map.setZoom(18);
                // app.showTempDownArrow(addrLocation, true);

                startWidget();

                //var addrLocation = new google.maps.LatLng(lat, lng);
                //var latlng = { lat: lat, lng: lng };
                //app.geocoder.geocode({ 'location': latlng }, function (results, status) {
                //    if (status === 'OK') {
                //        $('#placesSearch').val(results[0].formatted_address);
                //        si.placeChange(results[0]);
                //    }
                //    else {
                //        window.alert('Geocoder failed due to: ' + status);
                //    }
                //});
            }
        }
    };
}

function ShowAutoPlanLineLength(_path) {
     

    networkdata.ResetBomDetails();
    arrLinePath = _path || si.gMapObj.libPath.slice();

    var arr = [];

    for (ll of arrLinePath) {
        arr.push(ll.lng() + ' ' + ll.lat());
    }
    $('#geometry').val(arr);

    networkdata.GemoValidation(arr);

    ajaxReq('Plan/GetNetworkPlanningLineLength', { geom: $('#geometry').val() }, true, function (resp) {
        distance = resp.result;
        networkdata.cable_calculated_length = parseFloat(distance.toFixed(2));
        $('#lengthAutoPlaningDiv').html('<p>' + MultilingualKey.SI_OSP_ROW_NET_GBL_021 + ': <i>' + distance.toFixed(2) + '(m)' + '</i></p>');
    }, true, false, true);

    var startlatlng = arrLinePath[0];
    var endlatlng = arrLinePath[arrLinePath.length - 1];

    $('#start_point').val(startlatlng.lat().toFixed(6) + ',' + startlatlng.lng().toFixed(6));

    $('#end_point').val(endlatlng.lat().toFixed(6) + ',' + endlatlng.lng().toFixed(6));
}

function showAutoPlanDist(_path) {

    var distance = google.maps.geometry.spherical.computeLength(_path);
    if (parseInt(distance) > 1000)
        return (distance / 1000).toFixed(2) + ' km';
    else
        return distance.toFixed(2) + ' m';
}
$('#demo').change(function (input) {
    var id = $(this).val();
    var lat_long = $('tr[data-content="' + id + '"]').find('td:last').text();
    var lat_longArr = lat_long.split(',');
    var endpointLat_long = parseFloat(lat_longArr[0]).toFixed(6) + "," + parseFloat(lat_longArr[1]).toFixed(6);
    $('#end_point').val(endpointLat_long);
    $('#end_point_entity').val(id);
    //endLatlng = { lat: parseFloat(lat_longArr[0]), lng: parseFloat(lat_longArr[1]) };
    networkdata.NetworkEndPoint = lat_long;
    networkdata.P2PNetworkManual('end');
});

// create buffer circle
function startWidget() {
    si.fademap = [new google.maps.LatLng(85, 180), new google.maps.LatLng(85, 0), new google.maps.LatLng(85, -180), new google.maps.LatLng(-85, -180), new google.maps.LatLng(-85, 0), new google.maps.LatLng(-85, 180)];

    //Create Circle on Google Address Search
    var bufferArea = $('#end_point_buffer').val() / 1000;
    //var maxdistance = 2;
    var maxdistance = $('#MaxAutoPlanEndPointBuffer').val() / 1000;;
    var mindistance = $('#MinAutoPlanEndPointBuffer').val() / 1000;
    initDistanceWidget(si, bufferArea, maxdistance, mindistance);
    fadeOuter(si.map.getCenter(), bufferArea);

}

function initDistanceWidget(si, startdistance, maxdistance, mindistance) {

    if (si.distanceWidget) {
        si.distanceWidget.set("map", null);
        si.distanceWidget = null;
    }

    this.si.distanceWidget = new DistanceWidget({
        map: si.map,
        distance: startdistance, // Starting distance in km.
        maxDistance: maxdistance, // Twitter has a max distance of 2500km.
        minDistance: mindistance,
        color: '#3a79c8',
        activeColor: '#3a79c8',
        sizerIcon: {
            url: 'Content/images/resize.png',
            size: new google.maps.Size(24, 24),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(12, 12)
        },
        icon: {
            url: 'Content/images/center.png',
            size: new google.maps.Size(21, 21),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(10, 10)
        }
    });
}

function fadeOuter(_center, _radius) {

    if (si.fadeMap) {
        si.fadeMap.setMap(null);
    }
    var fadeOptions = {
        strokeWeight: 0,
        fillColor: '#ffffff',
        fillOpacity: 0.6,
        map: si.map,
        paths: [si.fademap, drawCircle(_center, _radius, -1)]
    };
    si.fadeMap = new google.maps.Polygon(fadeOptions);
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

function powerBackupTrue() {

    $("#txtPwrBkpCapacity").prop("readonly", false);
}


