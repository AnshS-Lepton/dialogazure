var linksOnMap = [];

$(document).ready(function () {
    $('#chkSelectAll').change(function () {
        $('.chklink').prop('checked', $(this).prop('checked'));
    });

    $('#closeModalPopup').on('click', function () {
        clearLinksfromMap();
    });

    $('#chkCommonPath').on('click', function (event) {
        if ($(this).is(':checked')) {
            //$('#chkFiberLinks').prop('checked', false);
            $('#divLinkIdsSearchResult').show();
            $('#divLinksSearchResult').hide();
            $('#btnShowOnMapFiberLinks').hide();
            $('#btnShowOnMapLinkIds').show();
            $('#chkCommonPath').prop('disabled', true);
            if ($('#chkFiberLinks').is(':checked')) {
                $('#chkFiberLinks').prop('checked', false).prop('disabled', false);
            }
        }
        else {
            $('#divLinkIdsSearchResult').hide();
        }
    });
    $('#chkFiberLinks').on('click', function (event) {
        if ($(this).is(':checked')) {
           // $('#chkCommonPath').prop('checked', false);
            $('#divLinkIdsSearchResult').hide();
            $('#divLinksSearchResult').show();    //this div is for FiberLinks details
            $('#btnShowOnMapFiberLinks').show();
            $('#btnShowOnMapLinkIds').hide();
            $('#chkFiberLinks').prop('disabled', true);
            if ($('#chkCommonPath').is(':checked')) {
                $('#chkCommonPath').prop('checked', false).prop('disabled', false);
            }
        } else {
            $('#divLinksSearchResult').hide();
        }
    });

    $('#chkFiberSelectAll').change(function () {
        $('.chkFiberlink').prop('checked', $(this).prop('checked'));
    });
});

$(function () {
    let isResizing = false;
    const modal = document.querySelector('.modal-content');
    // modal.classList.add('resizable');
    const resizableDiv = modal;//$('#ModalPopUp');
    const corner = $('<div>').css({
        width: '10px',
        height: '10px',
        backgroundColor: 'transparent',
        position: 'absolute',
        bottom: 0,
        right: 0,
        cursor: 'se-resize'
    }).appendTo(resizableDiv);

    corner.mousedown(function (e) {
        isResizing = true;
        const initialWidth = resizableDiv.width();
        const initialHeight = resizableDiv.height();
        const initialMouseX = e.clientX;
        const initialMouseY = e.clientY;

        $(document).mousemove(function (e) {
            if (isResizing) {
                const widthDiff = e.clientX - initialMouseX;
                const heightDiff = e.clientY - initialMouseY;
                resizableDiv.width(initialWidth + widthDiff);
                resizableDiv.height(initialHeight + heightDiff);
            }
        });

        $(document).mouseup(function () {
            isResizing = false;
            $(document).off('mousemove mouseup');
        });
    });

});

function resetLinkdata() {
    $('#txtLinkIds').val('');
    $('#tblbodyLinks').empty();
    $('#tblbodyFiberLinks').empty();
    $('#divLinkIdsSearchResult').hide();
    $('#divLinksSearchResult').hide();
    $('#btnShowOnMapLinkIds').hide();
    $('#btnShowOnMapFiberLinks').hide();
    $('#divChkSearchResult').hide();
}


function SearchLinkIds(event) {
    event.preventDefault();
    let linkIds = $('#txtLinkIds').val();

    if (linkIds == '') {
        alert('Enter Link Ids');
        return false;
    }
    if (linkIds != '') {
    ajaxReq('CommonPathFinder/validateLinkIds', { linkIds: linkIds }, false, function (resp) {
        if (JSON.parse(resp.data[0]).invalidlinkids != null) {
            let invalid_link_ids = JSON.parse(resp.data[0]).invalidlinkids;
            alert("Invalid LinkIds :" + invalid_link_ids);
            $('#tblbodyLinks').empty();
            $('#tblbodyFiberLinks').empty();
            return false;
        }
            else {
        ajaxReq('CommonPathFinder/GetCableListByLinkIds', { linkIds: linkIds }, false, function (resp) {
                    if (resp.status == "OK") {
                let tblbody = $('#tblbodyLinks');
                let tblbodyfiber = $('#tblbodyFiberLinks');
                tblbody.empty();
                tblbodyfiber.empty();
                let linkdata = JSON.parse(resp.data).features;
                        // Parse the JSON response
                let parsedResponse = JSON.parse(resp.fiberData);

                        // Access the FiberLinkDetails array
                let fiberLinkData = JSON.parse(parsedResponse.result).FiberLinkDetails;

                        //let maxLinkCount = Math.max(...linkdata.map(item => item.properties.link_count));
                let maxLinkCount = 0;
                        if (linkdata != null) { maxLinkCount = Math.max(...linkdata.map(item => item.properties.link_count)); }

                        var strGeom = '';
                if (linkdata != null) {
                    linkdata.forEach(function (value) {
                        let isChecked = (value.properties.link_count === maxLinkCount) ? "checked" : "";
                        let bodyRow = $(`<tr>`);
                        bodyRow.append($(`<td>`).html(`<input ${isChecked} type="checkbox" class="chklink" geom='${value.geometry.coordinates.map(a => a.join(" ")).join(",")}' />`));
                        bodyRow.append($(`<td>`).text(value.properties.cable_name));
                        bodyRow.append($(`<td>`).text(value.properties.network_id));
                        bodyRow.append($(`<td>`).text(value.properties.a_location));
                        bodyRow.append($(`<td>`).text(value.properties.b_location));
                        bodyRow.append($(`<td>`).text(value.properties.link_count));

                                // strGeom = strGeom == '' ? linkdata[index].geometry.coordinates.map(a => a.join(" ")).join(",") : strGeom + ',' + linkdata[index].geometry.coordinates.map(a => a.join(" ")).join(",")
                        tblbody.append(bodyRow);
                    });

                    // ✅ Auto-check #chkSelectAll if all .chklink are checked
                    let totalChkLinks = $('.chklink').length;
                    let checkedChkLinks = $('.chklink:checked').length;
                    $('#chkSelectAll').prop('checked', totalChkLinks > 0 && totalChkLinks === checkedChkLinks);
                }
                if (fiberLinkData != null) {
                    fiberLinkData.forEach(function (value) {
                        let bodyRow = $(`<tr>`);
                        bodyRow.append($(`<td>`).html(`<input type="checkbox" class="chkFiberlink" geom='${value.geometry}' colorcode='${value.properties.ColorCode}' />`).css('background-color', value.properties.ColorCode));
                        bodyRow.append($(`<td>`).text(value.properties.network_id));
                        bodyRow.append($(`<td>`).text(value.properties.link_id));
                        bodyRow.append($(`<td>`).text(value.properties.link_name));
                        bodyRow.append($(`<td>`).text(value.properties.fiber_link_status));
                        bodyRow.append($(`<td>`).text(value.properties.link_type));

                        tblbodyfiber.append(bodyRow);
                    });

                    // ✅ Auto-check #chkFiberSelectAll if all .chkFiberlink are checked
                    let totalChkFiber = $('.chkFiberlink').length;
                    let checkedChkFiber = $('.chkFiberlink:checked').length;
                    $('#chkFiberSelectAll').prop('checked', totalChkFiber > 0 && totalChkFiber === checkedChkFiber);
                }
                        // Add event delegation for dynamic elements
                $(document).on('change', '.chklink', function () {
                    linkChange(this, tblbody);
                });
                $(document).on('change', '.chkFiberlink', function () {
                    linkChange(this, tblbodyfiber);
                });
                $('#chkCommonPath').prop('checked', true);
                if ($('#chkCommonPath').is(':checked')){
                    $('#chkCommonPath').prop('checked', true).prop('disabled', true);

                }
                $('#chkFiberLinks').prop('checked', false);
                $('#divLinkIdsSearchResult').show();
                $('#divLinksSearchResult').hide();
                $('#btnShowOnMapLinkIds').show();
                $('#divChkSearchResult').show();
                        //linkShowOnMapDefault(linkdata[0].geometry.coordinates.map(a => a.join(" ")).join(","));
                linkShowOnMap();

                const dargetedDiv = document.querySelector('.modal-content');
                dargetedDiv.classList.remove('resizable');
                        //dargetedDiv.classList.add('minimize');

                        // linkShowOnMapDefault(strGeom);
                    }
                    else {
                alert(resp.error_message);
            }
        });
            }
    });
}
}
function linkShowOnMapDefault(geom) {
    clearLinksfromMap();

    let playroute = new google.maps.Polyline({
        path: getGoogleLineGeomfromLatLngString(geom),
        geodesic: true,
        strokeColor: '#FF0000',// getRandomColor(),//apisource == "GoogleAPI" ? '#000000' : '#FF0000',
        strokeWeight: 2,
        draggable: false
    });

    playroute.setMap(si.map);
    linksOnMap.push(playroute);
    let bounds = new google.maps.LatLngBounds();
    linksOnMap.forEach(function (polyline) {
        polyline.getPath().forEach(function (point) {
            bounds.extend(point);
        });
    });

    // Fit map to bounds
    si.map.fitBounds(bounds);
    $('.minmizeModel').find('.fa-minus').click();

}
function linkShowOnMap() {
    clearLinksfromMap();
    if ($('.chklink:checked').length == 0) {
        alert(MultilingualKey.SI_OSP_GBL_NET_RPT_201);
        return;
    }
    $('.chklink:checked').each(function (item) {
        let playroute = new google.maps.Polyline({
            path: getGoogleLineGeomfromLatLngString($(this).attr('geom')),
            geodesic: true,
            strokeColor: '#FF0000',// getRandomColor(),//apisource == "GoogleAPI" ? '#000000' : '#FF0000',
            strokeWeight: 2,
            draggable: false
        });

        playroute.setMap(si.map);
        linksOnMap.push(playroute);
    });
    let bounds = new google.maps.LatLngBounds();
    linksOnMap.forEach(function (polyline) {
        polyline.getPath().forEach(function (point) {
            bounds.extend(point);
        });
    });

    // Fit map to bounds
    si.map.fitBounds(bounds);
    $('.minmizeModel').find('.fa-minus').click();

}
function FiberLinkShowOnMap() {
    clearLinksfromMap();
    if ($('.chkFiberlink:checked').length == 0) {
        alert(MultilingualKey.SI_OSP_GBL_NET_RPT_201);
        return;
    }
    //Reset the value of linksOnMap array
    linksOnMap = [];
    
    $('.chkFiberlink:checked').each(function (item) {
        // Split the geometry string by '|' and iterate over each geometry
        let geometries = $(this).attr('geom').split('|');
        
        let cblColorCode = $(this).attr('colorcode');
        geometries.forEach(function (geometry) {
            let playroute = new google.maps.Polyline({
                path: getGoogleLineGeomfromLatLngString(geometry),
                geodesic: true,
                strokeColor: cblColorCode,
                strokeWeight: 2,
                draggable: false
            });

            playroute.setMap(si.map);
            linksOnMap.push(playroute);
        });
    });
    let bounds = new google.maps.LatLngBounds();
    linksOnMap.forEach(function (polyline) {
        polyline.getPath().forEach(function (point) {
            bounds.extend(point);
        });
    });

    // Fit map to bounds
    si.map.fitBounds(bounds);
    $('.minmizeModel').find('.fa-minus').click();

}

function clearLinksfromMap() {
    if (linksOnMap.length > 0) {
        linksOnMap.forEach(function (polyline) {
            polyline.setMap(null);
        });
    }
}

function getGoogleLineGeomfromLatLngString(geomString) {
    let geom = [];
    let arr = geomString.split(',');
    arr.forEach(function (latlng) {
        latlng = latlng.trim();
        let ll = new google.maps.LatLng(latlng.split(' ')[1], parseFloat(latlng.split(' ')[0]))
        geom.push(ll);
    })
    return geom;
}


function linkChange(curobj, dataContainer) {
    let tblbody = $('#tblbodyLinks');
    let tblbodyfiber = $('#tblbodyFiberLinks');
    if (dataContainer.is(tblbody)) {
        if (!$(curobj).prop('checked')) {
            $('#chkSelectAll').prop('checked', false);
        } else {
            // Check if all checkboxes are checked
            if ($('.chklink:checked').length === $('.chklink').length) {
                $('#chkSelectAll').prop('checked', true);
            }
        }
    }
    else if (dataContainer.is(tblbodyfiber)) {
        if (!$(curobj).prop('checked')) {
            $('#chkFiberSelectAll').prop('checked', false);
        } else {
            // Check if all checkboxes are checked
            if ($('.chkFiberlink:checked').length === $('.chkFiberlink').length) {
                $('#chkFiberSelectAll').prop('checked', true);
            }
        }
    }
}

$('#closeModalPopup').on('click', function () {
    const dargetedDiv = document.querySelector('.modal-content');
    dargetedDiv.classList.remove('resizable');
    dargetedDiv.removeAttribute('style');
    const corner = dargetedDiv.querySelector('div[style*="cursor: se-resize"]'); // Find the corner div
    if (corner) {
        corner.remove();
    }
});

$('.close.minmizeModel.ml-auto.mr-05').on('click', function (event) {
    const dargetedDiv = document.querySelector('.modal-content');
    if (dargetedDiv.className != 'modal-content resizable') {
        if (event.target.className == 'close minmizeModel ml-auto mr-05') {
            dargetedDiv.classList.remove('resizable');

            dargetedDiv.classList.add('minimize');
            const iconElement = document.querySelector('.close .fa');
            iconElement.classList.add('fa-plus', 'fa-clone');
        }
        else if (event.target.className == 'fa fa-minus fa-plus fa-clone') {

            dargetedDiv.classList.add('resizable');
            dargetedDiv.classList.remove('minimize');
            const iconElement = document.querySelector('.close .fa');
            iconElement.classList.remove('fa-plus', 'fa-clone');

        }
        else if (event.target.className == 'fa fa-minus') {
            dargetedDiv.classList.remove('resizable');
            dargetedDiv.classList.remove('minimize');
            const iconElement = document.querySelector('.close .fa');
            iconElement.classList.add('fa-plus', 'fa-clone');
        }
    }
});