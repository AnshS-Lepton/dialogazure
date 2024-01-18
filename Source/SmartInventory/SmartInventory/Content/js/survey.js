//-------------global variables------------------------
//Define a variable with all map points.
var DirRendere = [];
var combinedResultsLists = [];
var allRouteMarkers = [];
var surveyLine = undefined;
//You can calculate directions (using a variety of methods of transportation) by using the DirectionsService object.
var directionsService = new google.maps.DirectionsService();
// Instantiate an info window to hold step text.
var objInfoWindow = new google.maps.InfoWindow();
//Define a DirectionsRenderer variable.
var _directionsRenderer = '';
var destinationMarker;
var destinationHTML;
var polylineOptions = {
    strokeColor: '#C83939',
    strokeOpacity: 1,
    strokeWeight: 4
};

function Opensurveyassignment(divObj) {
    popup.LoadModalDialog('PARENT', 'SurveyArea/SurveyAssignment', { eType: '' }, "Survey Assignment", 'modal-lg');
}
function zoomSearchedElement1(EntityType, entity, SystemID) {
    //parent.zoomElement(EntityType, SystemID);
    //parent.showTempLinePoint(SystemID, EntityType);
    //parent.minimizepopup();
     
    si.ShowEntityOnMap(SystemID, entity, EntityType);
}
function btnShowSurveyAssin(obj, sysId, dueDate, surveyName, networkcode) {

    if (dueDate != '') {

        popup.LoadModalDialog('CHILD', 'SurveyArea/GetUserAssignment', { system_id: sysId, network_code: networkcode, surveyarea_name: surveyName, due_date: dueDate }, "User Assignment", 'modal-md');
        chosenSelectAll();
    }
    else {
        alert(MultilingualKey.SI_OSP_SVA_JQ_FRM_005);
    }
}

function chosenSelectAll() {
    $('input.chk_Circles_all:checkbox').on('change', function () {
        $('#dvAllUserList ul.chosen-results li').not(".hide").find('input.chk_CircleName:checkbox').prop('checked', $(this).is(":checked"));
        validateSelectAll();
    });

    $('input.chk_CircleName:checkbox').on('change', function () {
        validateSelectAll();
    });
    validateSelectAll();
}
function validateSelectAll() {
    var cntAll = $("input.chk_CircleName:checkbox").length;
    var cntChecked = $("input.chk_CircleName:checkbox:checked").length;
    $('#lblUsrCount').html(cntChecked);
    $(".chk_Circles_all[type='checkbox']").prop("checked", (cntAll == cntChecked));
    $("#dvAllUserList ul.chosen-choices li.search-field [type='text']").prop("placeholder", cntChecked == 0 ? "Select Users" : "" + cntChecked + " Selected out of " + cntAll + "");
}

function fnValidateUser() {

    $('#dvAllUserList li.active-result').removeClass('hide');
    var circleNames = [];
    var checkedCircles = parent.$(".chk_CircleName[type='checkbox']:checked");
    var maxAssignedUsers = $("#hdnMaxAssignedUser").val();
    $.each(checkedCircles, function () {
        circleNames.push($(this).val());
    });
    if (circleNames.length == 0) {
        alert(MultilingualKey.SI_OSP_SVA_JQ_FRM_004);
        return false;
    }
    if (circleNames.length > maxAssignedUsers) {
       
        alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_004, maxAssignedUsers));
        return false;
    }
    var assignedId = circleNames.join(',');
    //   assignedId = parent.$("#hdnAccessibleCircles").val(circleNames.join(','));
    var systemid = $("#SurveyAreaSystemId").val();


    var func = function () { assignedUser(systemid, assignedId) };
    showConfirm(MultilingualKey.SI_OSP_SVA_JQ_FRM_002, func);

}
function assignedUser(systemid, assignedId) {

    ajaxReq('SurveyArea/SaveAssignedUser', { systemId: systemid, assignedUserId: assignedId }, true,
   function (resp) {
       if (resp.status == "OK") {
           $('#btnSearchSurveyArea').trigger("click");
           $('#closeChildPopup').trigger("click");
           alert(MultilingualKey.SI_OSP_SVA_JQ_FRM_001);

       }
       else {
           alert(MultilingualKey.SI_OSP_BUL_JQ_RPT_004);

       }
   }, true, true);

    return true;
}
function showSurveyEdit(system_id) {
    popup.LoadModalDialog('CHILD', 'Library/AddSurveyArea', _data = { systemId: system_id, entityType: "SurveyArea", geomType: "Polygon", childModel: "dvChildModalBody" }, "SurveyArea", 'modal-sm');
}
function showBldEdit(building_id, geomType) {
    popup.LoadModalDialog('CHILD', 'Library/AddBuilding', _data = { systemId: building_id, entityType: "Building", geomType: geomType, childModel: "dvChildModalBody" }, "Building", 'modal-lg');
}
function OpenSurveyBuilding(divObj) {
    popup.LoadModalDialog('PARENT', 'SurveyArea/SurveyBuilding', { eType: '' }, "Survey Building", 'modal-lg');
}
function OpenBuildingUpdation(divObj) {
    popup.LoadModalDialog('PARENT', 'SurveyArea/SurveyUpdateBuilding', { eType: '' }, "Update Building", 'modal-lg');
}
function OpenPathTracker(divObj) {    
    $("#SplicingDiv").hide();
    $("#InfoDiv").hide();
    popup.LoadModalDialog('PARENT', 'SurveyArea/GetSurveyLocationTrackingData', { eType: '' }, MultilingualKey.SI_OSP_GBL_JQ_FRM_092, 'modal-lg');
}
function showSiteCustomerEdit(system_id, site_id,lmcType,structureId) {
     
    popup.LoadModalDialog('CHILD', 'Library/AddSiteCustomer', _data = { systemId: system_id, siteId: site_id, lmcType: lmcType, structureId: structureId, entityType: '', childModel: "dvChildModalBody" }, "Update Site Customer", 'modal-lg');
}
function showBldComments(building_id) {
    popup.LoadModalDialog('CHILD', 'Library/GetBuildingComments', { buildingId: building_id,childModel: "dvChildModalBody" }, "Building Status History", 'modal-lg');
}

function bindAreaDD(cityid) {

    var ddlBulkArea = $("#ddlArea");
    if (cityid != '') {
        ajaxReq('SurveyArea/GetAreaList', { province_id: cityid }, false,
    function (resp) {

        if (resp.status == "OK") {

            ddlBulkArea.empty();
            ddlBulkArea.append($("<option></option>").val('').html('Select Area'));
            $.each(resp.result.lstArearea, function (data, value) {
                ddlBulkArea.append($("<option></option>").val(value.area_id).html(value.area_name));
            });
        }
        else {

            ddlBulkArea.empty();
            ddlBulkArea.append($("<option></option>").val('').html('Select Area'));
        }
    }, true, true);
    }
    ddlBulkArea.trigger("chosen:updated");
}
function showtrackBuilding(user_id, loginID, surveyareaID) {
    var _title = "";
    if (loginID > 0)
        _title = "Track Building";
    else
        _title = "Surveyed Building";
    popup.LoadModalDialog('CHILD', 'SurveyArea/GetTrackBuilding', { 'objFilterAttributes.user_id': user_id, 'objFilterAttributes.loginid': loginID, 'objFilterAttributes.surveyarea_id': surveyareaID }, _title, 'modal-lg');
}

function ShowUserCurrentLocation(_loginId) {
     
    removeOldMarkers();
    ajaxReq('SurveyArea/GetLocationTracking', { login_id: _loginId }, true,
  function (resp) {
      if (resp.status == "OK") {
          if (resp.result != null && resp.result != undefined) {

              $(popup.DE.MinimizeModel).trigger("click");
               
              var infoHtml = '<table width=100% cellspacing=0 cellpadding=2 style="font-size:12px;line-height: 20px;">' +
              '<tr><td><b>Track Date :&nbsp;&nbsp;</b></td> <td>' + JsonDateTimeFormator(resp.result.mobile_time) + '</td></tr>' +
              '<tr><td><b>Track Time :&nbsp;&nbsp;</b></td> <td>' + JsonTimeFormator(resp.result.mobile_time) + '</td></tr>' +
              '<tr><td><b>Latitude :&nbsp;&nbsp;</b></td><td>' + resp.result.latitude + '</td></tr>' +
              '<tr><td><b>Longitude :&nbsp;&nbsp;</b></td><td>' + resp.result.longitude + '</td></tr></table>';

              var myLatlng = new google.maps.LatLng(parseFloat(resp.result.latitude), parseFloat(resp.result.longitude));
              sMarker = createSurveyMarkerWithInfo(myLatlng, 'current.gif', infoHtml);

              si.map.setCenter(myLatlng);
              si.map.setZoom(17);
          }
          else {
              alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_003);
          }
      }
      else {
          alert(resp.message);

      }
  }, true, true);
}

function getcheckedvalue(_bldStatus,_bldComment) {


    var chkArray = [];

    /* look for all checkboes that have a class 'chkSurvey' attached to it and check if it was checked */
    $(".chkSurvey:checked").each(function () {
        chkArray.push($(this).val());
    });

    /* we join the array separated by the comma */
    var selectedID;
    selectedID = chkArray.join(',');

    /* check if there is selected checkboxes, by default the length is 1 as it contains one single comma */
    if (selectedID.length > 0) { 
        BulkApprovedUser(selectedID, _bldStatus, _bldComment) 
    }
    else {
        alert(MultilingualKey.SI_OSP_GBL_JQ_RPT_003);
    }
}



function BulkApprovedUser(systemid, _bldStatus, _bldComment) {
    ajaxReq('SurveyArea/BulkApproved', { systemId: systemid, status: _bldStatus, comment: _bldComment }, true,
function (resp) {
    if (resp.status == "OK") {
        if (_bldStatus == 'Approved') {
            alert(MultilingualKey.SI_OSP_BUL_JQ_FRM_003);
            si.loadLayerOnEntity();
            console.log('survey');
            $('#closeChildPopup').trigger("click");
        }
        else
            alert(MultilingualKey.SI_OSP_BUL_JQ_FRM_004);
        $('#closeChildPopup').trigger("click");
        $('#btnSurveyBuilding').trigger("click");
    }
    else {
        alert(MultilingualKey.SI_OSP_BUL_JQ_RPT_004);

    }
}, true, true);
    return true;
}

function SurveyAssignmentExport() {

    ajaxReq('SurveyArea/CheckExportSurveyAssignment', {}, false, function (status) {

        if (status != null && status != undefined) {
            if (status)  // check if true
            {
                window.location = appRoot + 'SurveyArea/ExportSurveyAssignment';
            }
            else {
                alert(MultilingualKey.SI_OSP_GBL_JQ_RPT_014);
            }
        }

    }, true, true);

}

function SurveyAssignmentReport() {
    window.location = appRoot + 'SurveyArea/DownloadSurveyAssignmentReport';
}
function LocationTrackingReport() {
    window.location = appRoot + 'SurveyArea/DownloadLocationTrackingReport';
}
function SurveyBuildingReport() {
    window.location = appRoot + 'SurveyArea/DownloadSurveyBuildingReport';
}
function fnSearchCircles(searchText) {

    var searchText = $.trim(searchText).toLowerCase();
    if (searchText.length > 1) {
        var records = $('#dvAllUserList li.active-result:not(#liSelectAllCircles)');
        $.each(records, function (idx, elem) {
            if ($(this).text().toLowerCase().indexOf(searchText) > -1)
                $(this).removeClass('hideUser');
            else
                $(this).addClass('hideUser');
        });
    }
    else
        $('#dvAllUserList li.active-result').removeClass('hideUser');
}
function clearAssignment() {
    $("#MultiUserAssignmentDiv").hide();
    $('#closeChildPopup').trigger("click");

}

function ShowRouteOnMap(_loginId, _isLogOut) {
     
    $(popup.DE.MinimizeModel).trigger("click");
    ajaxReq('SurveyArea/GetUserLocationTracking', { loginId: _loginId }, true,
        function (resp) {
            if (resp.status == "OK") {
                if (resp.result) {
                    DrawRouteOnMap(resp.result, _isLogOut);
                }
            }
            else {
                alert(resp.message);
            }
        }, true, true);
}

function createSurveyMarkerWithInfo(mrkrLatlng, imageName, infoContent) {
    
    var objMarker = new google.maps.Marker({
        position: mrkrLatlng,
        icon: 'Content/images/icons/lib/KMLICONS/' + imageName,
        draggable: false
    });
    objMarker.setMap(si.map);
    google.maps.event.addListener(objMarker, 'click', function () {
        objInfoWindow.setContent(infoContent);
        objInfoWindow.open(si.map, objMarker);
    });
    allRouteMarkers.push(objMarker);
    return objMarker;
}

function DrawRouteOnMap(data, _isLogOut) {
    var _mapPoints = [];
    var IsDirectionServiceEnable = $("#hdnIsDirectionServiceEnable").val();
    removeOldMarkers();
    //DirectionsRenderer() is a used to render the direction
    _directionsRenderer = new google.maps.DirectionsRenderer({ suppressMarkers: true, infoWindow: objInfoWindow, draggable: false });
    //Set the map for directionsRenderer
    _directionsRenderer.setMap(si.map);

    var oms = new OverlappingMarkerSpiderfier(si.map, {
        markersWontMove: true,
        markersWontHide: true,
        keepSpiderfied: true,
        circleFootSeparation: 35, // radius of circle 
        nearbyDistance: 30, // distance in which it include the markers in spiderfier..
        circleSpiralSwitchover: Infinity,//infinity= circle format, 0= spiral format with 9 element in one spiral round
        legWeight: 2.4
    });


    $.each(data, function (index, item) {
        _mapPoints.push(new google.maps.LatLng(parseFloat(data[index].latitude), parseFloat(data[index].longitude)));
    });
     
    if (IsDirectionServiceEnable=="True") {
        calcRoute(directionsService, _directionsRenderer, _mapPoints);
    }
    else { 
        // Creates the polyline object 

        surveyLine = new google.maps.Polyline({
                strokeColor: '#FF8800',
                strokeOpacity: 1,
                strokeWeight: 2,
                path: _mapPoints,
                //editable: true
            });
            surveyLine.setMap(si.map);
    }
   

    allRouteMarkers = [];
    var infoHtml = '';
    var sMarker = undefined;
    $.each(data, function (index, item) {
        infoHtml = '';
        if (index == 0) {
            //CREATE START MARKER..

            infoHtml = "<b> Tracked On:</b> " + JsonDateTimeFormator(data[index].server_time) + "<br/>";
            sMarker = createSurveyMarkerWithInfo(new google.maps.LatLng(parseFloat(data[index].latitude), parseFloat(data[index].longitude)), 'Actual_Start.png', infoHtml);
            oms.addMarker(sMarker);
        }
        if (index == data.length - 1) {
            infoHtml = "<b> Tracked On:</b> " + JsonDateTimeFormator(data[index].server_time) + "<br/>";
            sMarker = createSurveyMarkerWithInfo(new google.maps.LatLng(parseFloat(data[index].latitude), parseFloat(data[index].longitude)), _isLogOut ? 'End.png' : 'current.gif', infoHtml);
            oms.addMarker(sMarker);
        }
        if (data[index].entity_type != null && data[index].entity_type != '' && data[index].entity_type != undefined && data[index].entity_type.toLowerCase() == "building" && data[index].building_status != '' && data[index].building_status != null &&  data[index].building_status != undefined) {
            infoHtml = '<table width=100% cellspacing=0 cellpadding=2 style="font-size:12px;line-height:20px;">' +
                '<tr><td><b>Entity Type:</b></td> <td>' + (data[index].entity_type) + '</td></tr>' +
                '<tr><td><b>Building Code:</b></td> <td>' + (data[index].entity_network_id) + '</td></tr>' +
                '<tr><td><b>Building Status:</b></td> <td>' + data[index].building_status + '</td></tr>' +
               '<tr><td><b>Surveyed On:</b></td> <td>' + JsonDateTimeFormator(data[index].server_time) + '</td></tr>' +
              // '<tr><td><b>Building Code:</b></td> <td>' + data[index].entity_network_id + '</td></tr>' +
               
               '</table>';
            sMarker = createSurveyMarkerWithInfo(new google.maps.LatLng(parseFloat(data[index].latitude), parseFloat(data[index].longitude)), 'building_' + data[index].building_status.toLowerCase() + '.png', infoHtml);
            oms.addMarker(sMarker);
        }
        if (data[index].entity_type != null && data[index].entity_type != '' && data[index].entity_type != undefined && data[index].entity_type.toLowerCase() != "building") {
            infoHtml = '<table width=100% cellspacing=0 cellpadding=2 style="font-size:12px;line-height:20px;">' +
                '<tr><td><b>Entity Type:</b></td> <td>' + (data[index].entity_type) + '</td></tr>' +
                '<tr><td><b>Network Code:</b></td>  <td>' + (data[index].entity_network_id) + '</td></tr>' +
               '<tr><td><b>Created On:</b></td> <td>' + JsonDateTimeFormator(data[index].server_time) + '</td></tr>' +
               //'<tr><td><b>Network Code:</b></td> <td>' + data[index].entity_network_id + '</td></tr>' +
              // '<tr><td><b>Building Status :</b></td><td>' + data[index].building_status + '</td></tr>' +
               '</table>';
            sMarker = createSurveyMarkerWithInfo(new google.maps.LatLng(parseFloat(data[index].latitude), parseFloat(data[index].longitude)), data[index].entity_type +'.png', infoHtml);
            oms.addMarker(sMarker);
        }
    });

    if (allRouteMarkers.length > 0) {
        var bounds = new google.maps.LatLngBounds();
        for (var i = 0; i < allRouteMarkers.length; i++) {
            bounds.extend(allRouteMarkers[i].getPosition());
        }
        si.map.fitBounds(bounds);
    }
}
function calcRoute(directionsService, directionsDisplay, latLng) {
    var batches = [];
    var itemsPerBatch = 25; // google API max = 10 - 1 start, 1 stop, and 8 waypoints
    var itemsCounter = 0;
    var wayptsExist = latLng.length > 0;
    while (wayptsExist) {
        var subBatch = [];
        var subitemsCounter = 0;

        for (var j = itemsCounter; j < latLng.length; j++) {
            subitemsCounter++;
            subBatch.push({
                location: latLng[j],
                stopover: true
            });
            if (subitemsCounter == itemsPerBatch)
                break;
        }

        itemsCounter += subitemsCounter;
        batches.push(subBatch);
        wayptsExist = itemsCounter < latLng.length;
        // If it runs again there are still points. Minus 1 before continuing to
        // start up with end of previous tour leg
        itemsCounter--;
    }


    var unsortedResults = [{}]; // to hold the counter and the results themselves as they come back, to later sort
    var directionsResultsReturned = 0; 

        for (var k = 0; k < batches.length; k++) {
            var lastIndex = batches[k].length - 1;
            var start = batches[k][0].location;
            var end = batches[k][lastIndex].location;

            // trim first and last entry from array
            var waypts = [];
            waypts = batches[k];
            waypts.splice(0, 1);
            waypts.splice(waypts.length - 1, 1);
  
            var request = {
                origin: start,
                destination: end,
                waypoints: waypts,
                optimizeWaypoints: false,
                provideRouteAlternatives: false,
                travelMode: google.maps.DirectionsTravelMode.WALKING,
                unitSystem: google.maps.DirectionsUnitSystem.METRIC
            };
            (function (kk) {
                directionsService.route(request, function (result, status) {
                    if (status == window.google.maps.DirectionsStatus.OK) {

                        var unsortedResult = { order: kk, result: result };
                        unsortedResults.push(unsortedResult);

                        directionsResultsReturned++;

                        if (directionsResultsReturned == batches.length) // we've received all the results. put to map
                        {
                            // sort the returned values into their correct order
                            unsortedResults.sort(function (a, b) { return parseFloat(a.order) - parseFloat(b.order); });
                            var count = 0;

                            // now we should have a 2 dimensional array with a list of a list of waypoints
                            var combinedResults;
                            for (var key in unsortedResults) {
                                if (unsortedResults[key].result != null) {
                                    if (unsortedResults.hasOwnProperty(key)) {
                                        if (count == 0) // first results. new up the combinedResults object
                                        {
                                            combinedResults = unsortedResults[key].result;
                                            combinedResultsLists.push(combinedResults);
                                        }
                                        else {
                                            // only building up legs, overview_path, and bounds in my consolidated object. This is not a complete
                                            // directionResults object, but enough to draw a path on the map, which is all I need
                                            combinedResults.routes[0].legs = combinedResults.routes[0].legs.concat(unsortedResults[key].result.routes[0].legs);
                                            combinedResults.routes[0].overview_path = combinedResults.routes[0].overview_path.concat(unsortedResults[key].result.routes[0].overview_path);
                                            combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(unsortedResults[key].result.routes[0].bounds.getNorthEast());
                                            combinedResults.routes[0].bounds = combinedResults.routes[0].bounds.extend(unsortedResults[key].result.routes[0].bounds.getSouthWest());
                                            combinedResultsLists.push(combinedResults);
                                        }
                                        count++;
                                    }
                                }
                            }
                            //showSteps(result);

                            directionsDisplay.setMap(si.map);
                            directionsDisplay.setDirections(combinedResults);
                            DirRendere.push(directionsDisplay);

                            var totalDistance = computeTotalDistance(combinedResults);
                            attachInstructionText(destinationMarker, destinationHTML + "<b>Total route distance:</b> ", totalDistance);
                        }
                    }
                });
            })(k);
        } 
}
function attachInfoText(marker, text) {
    google.maps.event.addListener(marker, 'click', function () {
        objInfoWindow.setContent(text);
        objInfoWindow.open(si.map, marker);
    });
}
function attachInstructionText(marker, text, distance) {
    google.maps.event.addListener(marker, 'click', function () {
        objInfoWindow.setContent(text + distance);
        objInfoWindow.open(si.map, marker);
    });
}
function removeOldMarkers(menuName) {
     

    var menu = menuName == undefined ? '' : menuName;
    si.activeInactiveMenu($('.iconBaricomoonfooter'));
   // si.addRemoveActiveClass('');
    //clear direction list
    $(".gMapProjSpecific").addClass("bulkOhide");
    $(".gmapLineTools").addClass("bulkOhide");
   // $('.clearRout').addClass("activeToolBar");

    for (var i = 0 ; i < DirRendere.length ; i++) {
        if (DirRendere[i]) {
            DirRendere[i].setMap(null);
        }
    }
    if (combinedResultsLists.length > 0) {
        combinedResultsLists = [];
    }
    for (var i = 0; i < allRouteMarkers.length; i++) {
        allRouteMarkers[i].setMap(null);
    }
    if (surveyLine != undefined) {
        surveyLine.setMap(null);
    }
    if (si.imgLocationMarker != null) { si.imgLocationMarker.setMap(null); }
    si.clearMarkers();
    si.resetShapeTools()
    si.resetRulerDirection();
    si.RemoveOldFeature();
    var menuClass = menuName == 'All'?'': menuName == 'ProjectSpecification' ? '.PS' : '.BBR';
    // remove the footer menu active state
    $('.footerMenu').not(menuClass).removeClass('activeToolBar');
    $('.footerMenuOption').not(menuClass).removeClass('activeToolBar');
    $('.iconBaricomoon').not(menuClass).removeClass('activeToolBar');

    $('#gMapLineTool').toggle(false);
    if (menuName != 'ProjectSpecification' || menuName == 'All') {
        $('#gMapProjSpecification').toggle(false);
    }
    if (menuName != 'BOMBOQReport'|| menuName == 'All') {
        $('#gAreaPotential').toggle(false);
    }
   
    $('#gExportPotential').toggle(false);
    $('#lengthAreaDiv').empty();
    // 
    si.RemoveOldInfoWindow();
}

function removeOldMarkersWithRemoveActive(menuName) {
    var menu = menuName == undefined ? '' : menuName;
    //si.activeInactiveMenu($('.iconBaricomoonfooter'));
    //si.addRemoveActiveClass('');
    //clear direction list
    for (var i = 0 ; i < DirRendere.length ; i++) {
        if (DirRendere[i]) {
            DirRendere[i].setMap(null);
        }
    }
    if (combinedResultsLists.length > 0) {
        combinedResultsLists = [];
    }
    for (var i = 0; i < allRouteMarkers.length; i++) {
        allRouteMarkers[i].setMap(null);
    }
    if (surveyLine != undefined) {
        surveyLine.setMap(null);
    }
    if (si.imgLocationMarker != null) { si.imgLocationMarker.setMap(null); }
    si.clearMarkers();
    si.resetShapeTools()
    si.resetRulerDirection();
    si.RemoveOldFeature();
    

    $('#gMapLineTool').toggle(false);
    if (menuName != 'ProjectSpecification' || menuName == 'All') {
        $('#gMapProjSpecification').toggle(false);
    }
    if (menuName != 'BOMBOQReport' || menuName == 'All') {
        $('#gAreaPotential').toggle(false);
    }

    $('#gExportPotential').toggle(false);
    $('#lengthAreaDiv').empty();
    // 
    si.RemoveOldInfoWindow();
}

function removeOldMarkersonRuler(menuName) {
    var menu = menuName == undefined ? '' : menuName;
    //clear direction list
    for (var i = 0 ; i < DirRendere.length ; i++) {
        if (DirRendere[i]) {
            DirRendere[i].setMap(null);
        }
    }
    if (combinedResultsLists.length > 0) {
        combinedResultsLists = [];
    }
    for (var i = 0; i < allRouteMarkers.length; i++) {
        allRouteMarkers[i].setMap(null);
    }
    si.clearMarkers();
    
    si.removeEventListnrs('click');
    if (si.gMapObj.shapeObj)
        si.gMapObj.shapeObj.setMap(null);
    si.gMapObj.shapeObj = null;
    $('#lengthAreaDiv').empty();
    if (si.mapListners["TempLibItm"] && !si.lineBufferObj) {
        si.mapListners["TempLibItm"].setMap(null);
        si.mapListners["TempLibItm"] = null;
    }
    if (si.mapListners["mapListners"])
        si.mapListners["mapListners"] = null;
    if (si.entityOBJ.length > 0) {
        $.each(si.entityOBJ, function (key, value) { value.setMap(null); });
        si.entityOBJ = [];
        si.map.setOptions({ draggableCursor: '' });       
    }

    si.resetRulerDirection();
    // remove the footer menu active state
    $('.footerMenu').not('.DNR').removeClass('activeToolBar');
    $('.footerMenuOption').not('.DNR').removeClass('activeToolBar');

    //$('#gMapLineTool').toggle(false);
    $('#gMapProjSpecification').toggle(false);
    $('#gAreaPotential').toggle(false);
    $('#gExportPotential').toggle(false);
    $('#lengthAreaDiv').empty();

}
function computeTotalDistance(result) {
    var total = 0;
    var myroute = result.routes[0];
    for (var i = 0; i < myroute.legs.length; i++) {
        total += myroute.legs[i].distance.value;
    }
    total = total / 1000;
    return total + ' km';
}


//$(".Assignment_block").draggable({ scroll: false, containment: "window" });

$("#btnSearchSurveyArea").on("click", function () {

    if ($("#txtDateFrom") == '' && $("#txtDateFrom") != '') {
        alert(MultilingualKey.SI_OSP_SVA_JQ_FRM_007);
        return false;
    }
    if ($("#txtDateFrom") != '' && $("#txtDateFrom") == '') {
        alert(MultilingualKey.SI_OSP_SVA_JQ_RPT_001);
        return false;
    }

});

function FreezeArea(systemid, surveyStatus) {
     
    var msg = surveyStatus == 'Completed' ? "freeze survey area?" : "unfreeze survey area?";
    var func = function () { FreezeDefreeze(systemid, surveyStatus) };
   
    showConfirm($.validator.format(MultilingualKey.SI_OSP_BUL_JQ_RPT_001, msg), func);

}
function FreezeDefreeze(systemid, surveyStatus) {

    ajaxReq('SurveyArea/FreezeSurveyarea', { systemId: systemid, status: surveyStatus }, true,
 function (resp) {
    
     if (resp.status == "OK") {
         if (surveyStatus == "Completed") {
             alert(MultilingualKey.SI_OSP_BUL_JQ_RPT_002);
         }
         else {
             alert(MultilingualKey.SI_OSP_BUL_JQ_RPT_003);
         }
         $("#btnSearchSurveyArea").trigger("click");
     }
     else {
         alert(MultilingualKey.SI_OSP_BUL_JQ_RPT_004);

     }
 }, true, true);

    return true;

}
