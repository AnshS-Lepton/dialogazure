var cordX = 0.2;
var cordX2 = 0.2;
// initalising 
function getRandomColor() {
    //
    var color_arr = ['#000000', '#f12957', '#7d0031', '#313aa0', '#313aa0', '#171d5b', '#7d12d0', '#060329', '#d518d6', '#b5181d'];
    //var color_arr = [ '#313aa0'];
    var arr_ind = Math.floor(Math.random() * 10);
    var a = color_arr[arr_ind];
    return a;
}

jsPlumb.importDefaults({
    DragOptions: { cursor: 'pointer', zIndex: 2000 },
    EndpointStyle: { width: 20, height: 16, strokeStyle: 'gray' },
    Endpoint: ["Dot", { radius: 2.25 }]

});

function drawStructureCables(_structId) {
    isp.clearDragableObject();
    ajaxReq('ISP/\', { structId: _structId }, false, function (data) {
        var position = 50;
        var xx = 0.2;
        if (data.length > 0 && data != undefined) {
            var cableCHKCount = $('#layersContainer ul input[data-mapabbr="CABLE"]').length;
            if (cableCHKCount == 0)
            { $('#layersContainer ul').append('<li class="ispLayerTree"><input id="chk_nLyr_" type="checkbox" data-mapabbr="CABLE"class="checkbox-custom"  checked="checked" style="margin-right: 6px;"><b><label for="chk_nLyr_" class="checkbox-custom-label"><span>Cable</span></label></b></li>'); }
            $.each(data, function (index, item) {
                //jsPlumb.connect(getOptions(item, position, xx)); position = parseFloat(position * 2); xx = (xx + 0.1);
                drawCables(item.line_geom, item.system_id, item.a_entity_type.toUpperCase() + '_' + item.a_system_id, item.b_entity_type.toUpperCase() + '_' + item.b_system_id, item.total_core, item.network_id, item.cable_type, item.network_status, item);
            });
        }
        $('.ISPViewContainer svg').css('z-index', '99')
        $('.ISPViewContainer .clsLabels').css('z-index', '999')
        $("path").mouseover(function () {
            $(this).parent('svg').next('.clsLabels').css({ "background-color": "green", "color": "#fff" }).show();;
        }).mouseout(function () {

            $('.clsLabels').css({ "background-color": "#e3f3d9", "color": "#131212" }).hide();
        });
    }, true, false);
}
function getOptions(item, labelPosition, xx) {
    var options = {};
    var strockXoffset = 0.5;
    var sourceId = item.a_entity_type.toUpperCase() + '_' + item.a_system_id;
    var targetId = item.b_entity_type.toUpperCase() + '_' + item.b_system_id;
    var sourceShaftId = $('#' + sourceId).attr('data-shaftid');
    var targetShaftId = $('#' + targetId).attr('data-shaftid');
    var dAnchor = [0.5, 0, 0, -1, 0]; //FOR TOP TO TOP CONNECTIVITY   
    var existingConnectors = jsPlumb.getConnections({ source: sourceId, target: targetId }).length + jsPlumb.getConnections({ source: targetId, target: sourceId }).length;
    var sourceConnector = jsPlumb.getConnections({ source: sourceId, target: '' }).length;
    var targetConnector = jsPlumb.getConnections({ source: '', target: targetId }).length;
    var totalConnector = sourceConnector + targetConnector;
    console.log('totalConnector' + totalConnector);
    if (sourceShaftId == targetShaftId) {
        var precision = 100; // 2 decimals
        var randomnum = Math.floor(Math.random() * (10 * precision - 1 * precision) + 1 * precision) / (1 * precision);
        console.log('randomnum' + randomnum % 1);
        var x = ((totalConnector) * ((randomnum % 1) + 2.5));
        console.log(x)
        if (sourceConnector == 0)
        { cordX = 0.2; }
        //dAnchor = [0.2, 0, 0, 0, x, 0];
        dAnchor = [cordX, 0, 0, 0, 0, 0];
        cordX = cordX + 0.12;
    }// else if (sourceShaftId != targetShaftId && totalConnector>0) {
        //    var x = -10 + totalConnector * 5;
        //    dAnchor = [0.5, 0, 0, -1, 0];;
        //    strockXoffset = totalConnector * (strockXoffset + 5);
        //}
    else {
        if (sourceConnector == 0)
        { cordX2 = 0.2; }
        dAnchor = [cordX2, 0, 0, -1, 0];//Right Shaft
        //dAnchor = ["Top","Bottom"];//Left Shaft       
        strockXoffset = existingConnectors * (strockXoffset + 5);
        cordX2 = cordX2 + 0.1;
    }
    var dConnectors = ["Flowchart", { cornerRadius: 5, stub: strockXoffset, events: { "click": function (label, evt) { showCableRightPop(targetId, item.system_id); } } }];
    options.source = sourceId,
    options.target = targetId,
    options.connector = dConnectors,
    options.paintStyle = { strokeStyle: '#0000FF', lineWidth: 1 },
    options.anchor = dAnchor,
    options.overlays = [["Label", {
        label: item.total_core + 'F', id: "label", cssClass: "clsLabels"
    }]]

    //Arrow Code: ["Arrow", { width: 8, length: 8, location: 0.67 }],
    //jsPlumb.makeSource(sourceId, {
    //    isSource : true
    //});
    //jsPlumb.makeTarget(targetId, {
    //    isTarget:true,         
    //});

    return options;
}

this.showCableRightPop = function (targetId, system_id, e) {
    //isp.isEditMode = false;
    //isp.editCableId = '';
    //isp.clearDragableObject();


    if ($('#' + targetId).length == 0) {
        var a_system_id = $('#ispCable_' + system_id).attr('data-a-system-id');
        var a_entity_type = $('#ispCable_' + system_id).attr('data-a-entity-type');
        var b_system_id = $('#ispCable_' + system_id).attr('data-b-system-id');
        var b_entity_type = $('#ispCable_' + system_id).attr('data-b-entity-type');

        if ($('#' + b_entity_type + '_' + b_system_id).length == 0) {
            targetId = a_entity_type + '_' + a_system_id;
        } else if ($('#' + a_entity_type + '_' + a_system_id).length == 0) {
            targetId = b_entity_type + '_' + b_system_id;
        }

    }
    $('.rel_pop').remove();
    var objHTML = $.parseHTML(isp.HTML);
    $(objHTML).find('.dvDeleteElement,.dvViewElement').attr('elementid', system_id);
    $(objHTML).find('.dvDeleteElement,.dvViewElement').attr('elementtype', 'ISP');
    $(objHTML).find('.dvEditgeom').attr('elementid', system_id).show().on("click", function () {
        isp.editCable(system_id);
    });
    $(objHTML).find('.dvSavegeom').attr('elementid', system_id).show().on("click", function () {
        isp.saveEdtitCablegeom(system_id);
    });

    $('#' + targetId).parent().parent().append(objHTML);
    if ($('#ispCable_' + system_id).attr('data-cable-type') == 'OSP') {
        $(objHTML).find('.dvDeleteElement,.dvViewElement').hide();
    }
    $(isp.DE.DeleteElement).unbind('click').bind('click', function (e) { isp.DeleteElement(e); });
    $(isp.DE.ViewElement).bind('click', function (e) { isp.currentTarget = e; isp.updateElement(e); });
    $(isp.DE.AddCableEvent).bind('click', function (e) { isp.currentTarget = e; app.AddCableEvent(e); });

    $('.closeRightPop').unbind('click').on("click", function () { $('.rel_pop').hide(); isp.clearDragableObject(); isp.isEditMode = false; isp.clearEditMode(); })
    $('.rel_pop').hide();
    $('.dvAddCable').remove();
    $('#' + targetId).parent().parent().children('.rel_pop').show();
    $('#' + targetId).parent().parent().children('.rel_pop').draggable({ scroll: false, containment: ".StructureInfo" });

    $('.rel_pop').css({ 'top': ((e.pageY)), 'left': (e.pageX) });
}
this.drawCables = function (path, system_id, sourceId, targetId, total_core, network_id, cable_type, network_status, item) {
    if (path != null) {
        var sShaft = $('#' + sourceId).attr('data-shaftid');
        var tShaft = $('#' + targetId).attr('data-shaftid');
        var color = isp.getColor();
        var NetworStatusClass = 'plannedCable';
        if (network_status == 'A')
        {
            NetworStatusClass = '';
        } else if (network_status == 'D')
        {
            NetworStatusClass = 'dormantCable';
        }
        var svgHtml = svgHtml = isp.parseSVG('<svg id="ispCable_' + system_id + '" data-system-id=' + system_id + ' data-total-core=' + total_core + ' networkId="' + network_id + '" data-cable-type=' + cable_type + ' data-network-status=' + network_status + ' data-a-system-id=' + item.a_system_id + ' data-a-entity-type=' + item.a_entity_type + ' data-b-system-id=' + item.b_system_id + ' data-b-entity-type=' + item.b_entity_type + ' data-a-location=' + item.a_location  +' data-b-location=' + item.b_location + ' style="position: absolute;z-index: 99;"   pointer-events="none"  version="1.1" xmlns="http://www.w3.org/1999/xhtml" ><g> <path class="tempcss ' + NetworStatusClass + '" stroke-linecap="round" d="' + path + '"  pointer-events="visibleStroke" version="1.1" xmlns="http://www.w3.org/1999/xhtml"  fill="none" stroke="' + color + '" stroke-width="3"> <title>' + network_id + '(' + total_core + 'F)' + '</title></path></g> </svg>');
        if (cable_type == 'OSP') {
            if ($('#' + item.b_entity_type + '_' + item.b_system_id).length > 0) {
                svgHtml = isp.parseSVG('<svg id="ispCable_' + system_id + '" data-system-id=' + system_id + ' data-total-core=' + total_core + ' networkId="' + network_id + '" data-cable-type=' + cable_type + ' data-network-status=' + network_status + ' data-a-system-id=' + item.a_system_id + ' data-a-entity-type=' + item.a_entity_type + ' data-b-system-id=' + item.b_system_id + ' data-b-entity-type=' + item.b_entity_type + ' style="position: absolute;z-index: 99;"   pointer-events="none"  version="1.1" xmlns="http://www.w3.org/1999/xhtml" ><g> <path class="tempcss ' + NetworStatusClass + '" stroke-linecap="round" d="' + path + '"  pointer-events="visibleStroke" version="1.1" xmlns="http://www.w3.org/1999/xhtml"  fill="none" stroke="' + color + '" stroke-width="3"> <title>' + network_id + '(' + total_core + 'F)' + '</title></path></g> </svg>');
            }
            else {
                svgHtml = isp.parseSVG('<svg id="ispCable_' + system_id + '" data-system-id=' + system_id + ' data-total-core=' + total_core + ' networkId="' + network_id + '" data-cable-type=' + cable_type + ' data-network-status=' + network_status + ' data-a-system-id=' + item.b_system_id + ' data-a-entity-type=' + item.b_entity_type + ' data-b-system-id=' + item.a_system_id + ' data-b-entity-type=' + item.a_entity_type + ' style="position: absolute;z-index: 99;"   pointer-events="none"  version="1.1" xmlns="http://www.w3.org/1999/xhtml" ><g> <path class="tempcss ' + NetworStatusClass + '" stroke-linecap="round" d="' + path + '"  pointer-events="visibleStroke" version="1.1" xmlns="http://www.w3.org/1999/xhtml"  fill="none" stroke="' + color + '" stroke-width="3"> <title>' + network_id + '(' + total_core + 'F)' + '</title></path></g> </svg>');
            }
        }
        
        $('#svgContainer').append(svgHtml);
        $('#ispCable_' + system_id + ' path').unbind('click').on("click", function (e) { showCableRightPop(targetId, system_id, e); });
        if (sShaft == tShaft) {
            var tcount = parseInt($('#' + targetId).attr('data-cable-count-v'));
            tcount += 1;
            $('#' + targetId).attr('data-cable-count-v', tcount)

            var scount = parseInt($('#' + sourceId).attr('data-cable-count-v'));
            scount += 1;
            $('#' + sourceId).attr('data-cable-count-v', scount)
        } else {
            var tcount = parseInt($('#' + targetId).attr('data-cable-count-h'));
            tcount += 1;
            $('#' + targetId).attr('data-cable-count-h', tcount)

            var scount = parseInt($('#' + sourceId).attr('data-cable-count-h'));
            scount += 1;
            $('#' + sourceId).attr('data-cable-count-h', scount)
        }
       
        isp.setSVGWIdthHeight();
    }
}



