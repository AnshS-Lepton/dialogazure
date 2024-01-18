/// <reference path="jquery-ui.js" />

var OSPSplice = function () {
    var app = this;
    var gMapObj = {};
    this.connections = [];
    this.eqpmntConnectedPorts = 0;//ports which are already connected
    this.scid = '<s:property value="s.spcl_id"/>';
    this.scparent = '<s:property value="s.sc_parent"/>';
    this.occ = new Array();
    this.occt = new Array();
    this.ports = new Array();
    this.portsl = new Array();
    this.rmid = 0;//not set from controller.
    this.rno = '';//not set from controller.
    this.coreCount = 1;
    this.gMapObj = {};
    this.jsp1 = null;
    this.jsp2 = null;
    this.jsp3 = null;
    this.jsp4 = null;
    this.jsp5 = null;
    this.jsp6 = null;
    this.jsp7 = null;
    this.jsp8 = null;
    this.jsp9 = null;
    this.mainJSPlumb = null;
    this.AllConnections = [];
    this.rightCoreCount = 0;
    this.isAutoDeleteConnection = false;
    this.allConnectionId = [];
    this.Connections=[];
    this.ParentModel = 'PARENT';
    this.DE = {
        "LeftCableId": "#hdnLeftCableId",
        "LeftCableTube": "#hdnLeftCableTube",
        "LeftCableCore": "#hdnLeftCableCorePerTube",
        "RightCableId": "#hdnRightCableId",
        "RightCableTube": "#hdnRightCableTube",
        "RightCableCore": "#hdnRightCableCorePerTube",
        "equipmentid": "#equipment_id",
        "ddlCore": "#ddlCore",
        "btnCPFexport": "#btncpfExport",
        "btnCPFClear": "#btncpfClear",
        "btnShowRoute": "#btnShowRoute",
        "entityid": "#objFilterAttributes_entityid",
        "port_no": "#objFilterAttributes_port_no",
        "entity_type": "#objFilterAttributes_entity_type",
        "showOnMap": "#btnShowOnMap",
        "SourceCable": "#ddlSourceCable",
        "DestinationCable": "#ddlDestinationCable",
        "ConnectingEntity": "#ddlConnectingEntity",
        "SplicingDiv": "#SplicingDiv",
        "splicingMain": "#SplicingDiv .splicingMain",
        "btnopticalExport": "#btnopticalExport",
        "btnConnectedCustomerExport": "#btnConnectedCustomerExport",
        
    }
    this.initApp = function () {
        this.bindEvents();
    }

    this.bindEvents = function () {
        $(app.DE.equipmentid).autocomplete({
            source: function (request, response) {
                var res = ajaxReq('Splicing/GetEquipmentSearchResult', { SearchText: $.trim(request.term) }, true, function (data) {
                     
                    if(data.length ==0)
                    {
                        var result = [
                          {
                              label: MultilingualKey.SI_GBL_GBL_JQ_GBL_001,
                              entity_id:0
                             
                          }
                        ];
                        response(result);
                    }
                    else{
                        response($.map(data, function (item) {
                            return {
                                label: item.display_name, value: item.network_id, entity_type: item.entity_type, entity_id: item.system_id
                            }//
                        }))
                    }
                }, true, false);
            },
            minLength: 3,
            select: function (event, ui) {
                $('#dvViewConnectionPathFinder,#dvSchematicViewContainer').html('');
                $('#divNoRecordExist').show();
                var label = ui.item.value;
                if (ui.item.entity_id == 0) {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                }
                else {
                    event.preventDefault();
                    $(app.DE.equipmentid).val(ui.item.label);
                    $(app.DE.entity_type).val(ui.item.entity_type);
                    $(app.DE.entityid).val(ui.item.entity_id);
                    app.BindEquipementPort(ui.item.entity_id, ui.item.entity_type);//
                }
               
            }

        });

        $(app.DE.btnCPFexport).click(function () {
            app.downloadCPE();
        });
        $(app.DE.btnConnectedCustomerExport).click(function () {
            app.downloadConnectedCustomerDetails();
        });
        
        $(app.DE.btnopticalExport).click(function () {
            app.downloadOpticalLink();
        });

        $(app.DE.btnCPFClear).click(function () {
            app.clearCFPGrid();
        });

        $(app.DE.btnShowRoute).click(function () {
            var srchrtxt = $(app.DE.equipmentid).val();
            if (srchrtxt == '') {
                $(app.DE.equipmentid).addClass('input-validation-error');
                return false;
            }
            else {
                $(app.DE.equipmentid).removeClass('input-validation-error');
            }
            var selectedValue = $('#ddlCore option:selected').val();
            if (selectedValue == '0') {
                $(app.DE.port_no).val('0');
                $("#ddlCore_chosen").addClass('input-validation-error');
                return false;
            }
            else {
                $(app.DE.port_no).val(selectedValue);
                $("#ddlCore_chosen").removeClass('input-validation-error');
            }
        });
        $(app.DE.showOnMap).click(function () {
            app.showPath();
        });
        $(app.DE.SplicingDiv).draggable({ scroll: false, handle: "#splicingHeader", containment: "window" });

    }
    this.SinglePathFind = function (obj) {
        popup.LoadModalDialog('PARENT', 'Splicing/ConnectionPathFinder', { eType: '' }, "Connection Path Finder", 'modal-xxl');
    }

   


    this.BindEquipementPort = function (entityid, entitytype) {
       
        var ddlCore = $(app.DE.ddlCore);
        ajaxReq('Splicing/GetEquipmentPortInfo', { entity_id: entityid, entity_type: entitytype }, false, function (resp) {
            if (resp.status == 'OK') {
                $(app.DE.equipmentid).removeClass('input-validation-error');
                $("#ddlCore_chosen").removeClass('input-validation-error');
                ddlCore.empty();
                ddlCore.append($("<option></option>").val('0').html('--Select--'));
                $.each(resp.result, function (data, value) {
                    ddlCore.append($("<option></option>").val(value.port_value).html(value.port_text));
                });

            }
            else {
                $("#ddlCore_chosen").addClass('input-validation-error');
                alert(resp.message);
            }
        }, true, false);
        ddlCore.trigger("chosen:updated");
    }
    this.ddlcpfCore = function ddlcpfCore(obj) {

        var selectedValue = $('#ddlCore option:selected').val();
        if (selectedValue == '0') {
            $(app.DE.port_no).val('0');
            $("#ddlCore_chosen").addClass('input-validation-error');
        }
        else {
            $(app.DE.port_no).val(selectedValue);
            $("#ddlCore_chosen").removeClass('input-validation-error');
        }
    }

    


    /*==============START CODE FOR SPLICING========================*/
    this.cancelSplice = function () {
        $(app.DE.SplicingDiv).hide();
        if (app.gMapObj.pointObj != null) { app.gMapObj.pointObj.setMap(null); app.gMapObj.pointObj = null; }
        if (app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
        if (app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; }
        // si.clearTPRelatedObjects();
        $('#ModalPopUp div').removeClass('modal-xxl');
    }
    this.splicingWindow = function () {

        if ($(app.DE.ConnectingEntity).val() == '0') {
            alert('Please select connecting element!');
            return false;
        } else if ($(app.DE.SourceCable).val() == '0' && $(app.DE.DestinationCable).val() == '0') {
            alert('Please select source or destination cable!');
            return false;
        }else if($(app.DE.ConnectingEntity + ' :selected').attr('data-total-port') =='' || $(app.DE.ConnectingEntity + ' :selected').attr('data-total-port') && parseInt($(app.DE.ConnectingEntity + ' :selected').attr('data-total-port'))==0)
        {
            alert('Zero port does not allowed!');
            return false;
        }
        var dataURL = 'Splicing/SplicingWindow';
        var titleText = 'Manual Splicing';
        var objSplicingInput = {
            SSystem_Id: parseInt($(app.DE.SourceCable).val()),
            DSystem_Id: parseInt($(app.DE.DestinationCable).val()),
            CenSystem_Id: parseInt($(app.DE.ConnectingEntity).val()),
            CenType: $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type')
        };
        popup.LoadModalDialog(app.ParentModel, dataURL, objSplicingInput, titleText, 'modal-xxl', function () {
            $('#dvProgressSplice').show();
            //setTimeout(app.InitilizeJsPlumb, 300); UN-COMMENT WHEN WE NEED TO RUN THE SPLICING               
            app.AllConnections = []; app.isAutoDeleteConnection = false; app.rightCoreCount = 0; app.allConnectionId = []; app.Connections = [];
            $('#dvProgressSplice').hide();//COMMENT WHEN WE NEED TO RUN THE SPLICING 
        });

    }
    this.showConnectionInfo = function (s) {
        console.log('showConnectionInfo');
        $("#list").html(s);
        //$("#list").fadeIn({complete:function() { jsPlumb.repaintEverything(); }});
    }
    this.hideConnectionInfo = function () {
        console.log('hideConnectionInfo');
        $("#list").fadeOut({
            complete: function () {
                jsPlumb.repaintEverything();
            }
        });
    }
    this.updateConnections = function (conn, remove) {
        var connectionsInfo = [];
        var source = $('#' + conn.sourceId);
        var target = $('#' + conn.targetId);
        connectionsInfo = app.getConnectionsRequest(source, target);
        if (!remove) {
            var spliceSource='';
            if(isp!=null)
            {
                spliceSource='ISP_SPLICING';
            }else if(si!=null)
            {
                spliceSource='OSP_SPLICING';            
            }

            var _data = {
                source_system_id: source.attr('data-system-id'),
                source_network_id: source.attr('data-network-id'),
                source_entity_type: source.attr('data-entity-type'),
                source_port_no: source.attr('data-port-no'),
                destination_system_id: target.attr('data-system-id'),
                destination_network_id: target.attr('data-network-id'),
                destination_entity_type: target.attr('data-entity-type'),
                destination_port_no: target.attr('data-port-no'),
                splicing_source:spliceSource
            };
            var connectionId = app.saveConnection(_data, source, target);

            app.AllConnections.push(conn);
            if (connectionsInfo.length > 0) { app.saveBulkConnections(connectionsInfo); }

            //if (chk == 0) {
            //    if (app.eqpmntConnectedPorts < scp) {
            //        connections.push(conn);
            //        app.eqpmntConnectedPorts++;
            //        if (connections.length > 0) {
            //            var s = "<span><strong>Connection Summary</strong></span><br/><br/><table id=\"splice_conn_sheet_tbl\"><tr><th align=\"left\"><b>A End Cable ID</b></th><th align=\"left\"><b>A End Type</b></th><th align=\"left\"><b>A End Port/Core ID</b></th><th align=\"left\"><b>A End Tube No</b></th><th align=\"left\"><b>A End Core No</b></th><th align=\"left\"><b>B End Cable ID</b></th><th align=\"left\"><b>B End Type</b></th><th align=\"left\"><b>B End Port/Core ID</b></th><th align=\"left\"><b>B End Tube No</b></th><th align=\"left\"><b>B End Core No</b></th></tr>";
            //            for (var j = 0; j < connections.length; j++) {
            //                s = s + "<tr><td>" + $('#' + connections[j].sourceId).parent().attr('uid') + "</td><td>" + b1_type + "</td><td>" + $('#' + connections[j].sourceId).attr('no') + "</td><td>" + $('#' + connections[j].sourceId).attr('t') + "</td><td>" + $('#' + connections[j].sourceId).attr('c') + "</td><td>" + $('#' + connections[j].targetId).parent().attr('uid') + "</td><td>" + b2_type + "</td><td>" + $('#' + connections[j].targetId).attr('no') + "</td><td>" + $('#' + connections[j].targetId).attr('t') + "</td><td>" + $('#' + connections[j].targetId).attr('c') + "</td></tr>";
            //            }
            //            app.showConnectionInfo(s);
            //        }

            //        $('#' + conn.targetId).attr("valid", 1);
            //        if (parseInt($('#' + conn.targetId).attr("data-check")) == 1) {
            //            return;
            //        } else {
            //            ajaxify("eaproxy.aspx", { 'format': 'json', 'action_name': 'saveconnection', 'c.a_id': $('#' + conn.sourceId).parent().attr('id'), 'c.a_type': b1_type, 'c.a_sub_id': conn.sourceId, 'c.b_id': $('#' + conn.targetId).parent().attr('id'), 'c.b_type': '<%out.print(b2_type);%>', 'c.b_sub_id': conn.targetId, 'c.conn_color': conn.getPaintStyle().strokeStyle, 'c.sc_id': scid, 'c.total_ports': scp, 'c.status': "Spliced", 'c.sc_parent_id': scparent, 'c.a_name': '<%out.print(b1_name);%>', 'c.a_user_id': '<%out.print(b1_id);%>', 'c.b_name': '<%out.print(b2_name);%>', 'c.sc_uid': '<%out.print(sc_uid);%>', 'c.b_user_id': '<%out.print(b2_id);%>', 'c.ac_no': $('#' + conn.sourceId).attr('no'), 'c.bc_no': $('#' + conn.targetId).attr('no'), 'c.user': '<%out.print(suser);%>', 'utype': '<%out.print(ut);%>', 'c.userid': '<s:property value="uid"/>', 'c.approverid': '<s:property value="rmid"/>', 'c.req_no': '<s:property value="rno"/>' }, function (data) {
            //                if (data.STATUS != 'OK') { alert(data.STATUS); return; }
            //                else {
            //                    if (data.app == 'YES') {
            //                        $('#app').html('');
            //                        $('#app').html('Sent For Approval').fadeIn(250).fadeOut(3500);
            //                    }
            //                }
            //            });

            //        }
            //        /* code to update attr_details_cable_info table */
            //        //ajaxify("eaproxy.aspx",{'format':'json','action_name':'updatebinfo','binfo.cable_id':$('#'+conn.sourceId).parent().attr('id'),'binfo.fiber_id':$('#'+conn.sourceId).attr('no'),'binfo.fiber_status':'Spliced'},function(data){})
            //    } else {
            //        //console.log(scocc+" in no port")
            //        alert(" No More Ports Left in Splice Closure" + scid);
            //        jsPlumb.detach(conn);
            //        app.eqpmntConnectedPorts++
            //        //window.location.reload();
            //    }
            //} else {
            //    alert("Port Already Occupied");
            //    jsPlumb.detach(conn);
            //    app.eqpmntConnectedPorts++;
            //    //window.location.reload();

            //}
        }
        else {
            if (!app.isAutoDeleteConnection) {
                var source = $('#' + conn.sourceId);
                var target = $('#' + conn.targetId);
                var targetConId = parseInt(target.attr('data-connection-id'));
                var sourceConId = parseInt(source.attr('data-connection-id'));
                var connectionId = sourceConId != 0 ? sourceConId : targetConId;
                if (parseInt(connectionId) != 0) { app.deleteConnection(connectionId, conn.sourceId, conn.targetId); }
                if (connectionsInfo.length > 0) { app.deleteBulkConnections(connectionsInfo); }
            }
            //var idx = -1;
            //for (var i = 0; i < connections.length; i++) {
            //    if (connections[i] == conn) {
            //        idx = i;
            //        break;
            //    }
            //}
            //if (idx != -1)
            //    connections.splice(idx, 1);

            //var s = "<span><strong>Connection Summary</strong></span><br/><br/><table id=\"splice_conn_sheet_tbl\"><tr><th align=\"left\"><b>A End Cable ID</b></th><th align=\"left\"><b>A End Type</b></th><th align=\"left\"><b>A End Port/Core ID</b></th><th align=\"left\"><b>A End Tube No</b></th><th align=\"left\"><b>A End Core No</b></th><th align=\"left\"><b>B End Cable ID</b></th><th align=\"left\"><b>B End Type</b></th><th align=\"left\"><b>B End Port/Core ID</b></th><th align=\"left\"><b>B End Tube No</b></th><th align=\"left\"><b>B End Core No</b></th></tr>";
            //for (var j = 0; j < connections.length; j++) {
            //    s = s + "<tr><td>" + $('#' + connections[j].sourceId).parent().attr('uid') + "</td><td>" + b1_type + "</td><td>" + $('#' + connections[j].sourceId).attr('no') + "</td><td>" + $('#' + connections[j].sourceId).attr('t') + "</td><td>" + $('#' + connections[j].sourceId).attr('c') + "</td><td>" + $('#' + connections[j].targetId).parent().attr('uid') + "</td><td>" + "<%out.print(b2_type);%>" + "</td><td>" + $('#' + connections[j].targetId).attr('no') + "</td><td>" + $('#' + connections[j].targetId).attr('t') + "</td><td>" + $('#' + connections[j].targetId).attr('c') + "</td></tr>";
            //}
            //app.showConnectionInfo(s);

            ////console.log("tot occ "+scocc);
            //$('#' + conn.targetId).attr("valid", 0);
            //$('#' + conn.targetId).attr("data-check", 0);
            //ajaxify("eaproxy.aspx", { 'format': 'json', 'action_name': 'deleteconnection', 'c.a_sub_id': conn.sourceId, 'c.b_sub_id': conn.targetId, 'c.sc_id': scid, 'c.userid': user_id, 'utype': ut, 'c.user': 'suser', 'c.userid': user_id, 'c.approverid': rmid, 'c.req_no': rno }, function (data) {
            //    if (data.STATUS != 'OK') { alert(data.STATUS); return; } app.eqpmntConnectedPorts--;
            //    if (data.app == 'YES') {
            //        $('#app').html('');
            //        $('#app').html('Sent For Approval').fadeIn(250).fadeOut(3500);
            //    }
            //});

            ///* code to update attr_details_cable_info table */
            ////ajaxify("eaproxy.aspx",{'format':'json','action_name':'updatebinfo','binfo.cable_id':$('#'+conn.sourceId).parent().attr('id'),'binfo.fiber_id':$('#'+conn.sourceId).attr('no'),'binfo.fiber_status':'Dark'},function(data){})

        }

    }
    this.InitilizeJsPlumb = function () {         
        var t0 = performance.now();
        var leftCableId = $(app.DE.LeftCableId).val();
        var leftCableTube = $(app.DE.LeftCableTube).val();
        var leftCableCore = $(app.DE.LeftCableCore).val();
        var rightCableId = $(app.DE.RightCableId).val();
        var rightCableTube = $(app.DE.RightCableTube).val();
        var rightCableCore = $(app.DE.RightCableCore).val();
       
        var defaultOptions = app.getDefaultOptions();
        defaultOptions.Anchors = ["RightMiddle", "LeftMiddle"];
        jsPlumb.importDefaults(defaultOptions);
        //app.bindjsPlumbEvent();
        app.addEndPoint();
        var jspl1 = jsPlumb.getInstance();
        jspl1.importDefaults(app.getDefaultOptions());
        jspl1.setc               
        app.createTube(leftCableId, leftCableTube, leftCableCore, jspl1, 'left');            
        var jspl2 = jsPlumb.getInstance();
        jspl2.importDefaults(app.getDefaultOptions());
        jspl2.setc
        app.createTube(rightCableId, rightCableTube, rightCableCore, jspl2, 'right');
        app.createSplitter();
        app.createONT();
        app.getConnections();       
        interval = setInterval(function () {            
            if (app.rightCoreCount == (parseInt(rightCableTube) * parseInt(rightCableCore))) { $('#dvProgressSplice').hide(); clearInterval(interval); $('#divSpliceWindow').css('visibility', 'visible'); }
        }, 100);
        
        var t1 = performance.now();
        console.log("Call to doSomething took " + (t1 - t0) + " milliseconds.")
    }
    this.createTube = function (cableId, totalTube, totalcore, jspl, cablePosition) {
        try {
            if (cablePosition == 'right') { app.coreCount = 1; }
            for (var x = 1; x <= totalTube; x++) {
                var tubeSource = cableId;
                var tubeTarget = cableId + '_TUBE_' + x;
                var tubeColor = $("#" + tubeTarget).attr('data-color');
                var tublelabel = $("#" + tubeTarget).attr('data-label');
                var cableOptions = app.getOptions(tubeSource, tubeTarget, cablePosition, tubeColor, 'tube');
                cableOptions.endpoint = "Blank"
                cableOptions.overlays = [["Label", { cssClass: "component label", label: tublelabel, location: 1 }]];
                cableOptions.lineWidth = 8;                
                var cn1 = jspl.connect(cableOptions).setDetachable("false");
                app.createCore(cableId, tubeTarget, x, jspl, cablePosition, totalcore);
            }              
            // hideProgress();
        } catch (err) {

        }
    }
    this.createCore = function (cableId, source, x, jspl, cablePosition, totalCore) {
        try {
            for (var i = 1; i <= totalCore; i++) {
                var target = cableId + '_CORE_' + app.coreCount;
                var coreColor = $('#' + target).attr('data-color');
                var cableOptions = app.getOptions(source, target, cablePosition, coreColor, 'core');
                cableOptions.endpoint = "Blank"
                var cn2 = jspl.connect(cableOptions).setDetachable("false");
                //$('#' + b1 + '_' + i).attr('t', x)
                //$('#' + b1 + '_' + i).attr('c', y)
                app.coreCount = app.coreCount + 1;
                if (cablePosition == 'right') {
                    app.rightCoreCount = app.rightCoreCount + 1;
                }
            }
        } catch (err) {

        }
    }
    this.createSplitter = function () {
        try {
            var jspl = jsPlumb.getInstance();
            jspl.importDefaults(app.getDefaultOptions());
            jspl.setc
            var source = null;
            var target = null;
            var connectionsInfo = [];
            $.each($('.Splitter'), function (indx, itm) {
                var leftPort = $(this).children('.leftPort');
                var rightPort = $(this).children('.rightPort');
                $.each($(leftPort).children('.inputPort'), function (indx, itm) {
                    var sourceId = $(this).attr('id');
                    source = $(this);
                    $.each($(rightPort).children('.outPutPort'), function (indx, itm) {
                        var targetId = $(this).attr('id');
                        target = $(this);
                        var coreColor = 'blue';
                        var cableOptions = app.getOptions(sourceId, targetId, 'Left', coreColor, 'core');
                        cableOptions.connector[1].cornerRadius = 0;
                        cableOptions.paintStyle.lineWidth = 2.8;
                        cableOptions.endpoint = "Blank"
                        var cn2 = jspl.connect(cableOptions).setDetachable("false");
                    });
                });
            });
        } catch (err) {

        }
    }
    this.createONT = function () {
        try {
            var jspl = jsPlumb.getInstance();
            jspl.importDefaults(app.getDefaultOptions());
            jspl.setc
            var source = null;
            var target = null;
            var connectionsInfo = [];
            $.each($('.ONT'), function (indx, itm) {
                var leftPort = $(this).children('.leftPort');
                var rightPort = $(this).children('.rightPort');
                $.each($(leftPort).children('.inputPort'), function (indx, itm) {
                    var sourceId = $(this).attr('id');
                    source = $(this);
                    $.each($(rightPort).children('.outPutPort'), function (indx, itm) {
                        var targetId = $(this).attr('id');
                        target = $(this);
                        var coreColor = 'blue';
                        var cableOptions = app.getOptions(sourceId, targetId, 'Left', coreColor, 'core');
                        cableOptions.connector[1].cornerRadius = 0;
                        cableOptions.paintStyle.lineWidth = 2.8;
                        cableOptions.endpoint = "Blank"
                        var cn2 = jspl.connect(cableOptions).setDetachable("false");
                    });
                });
            });
        } catch (err) {

        }
    }
    this.formatDate = function () {
        var d = new Date(),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;
        return [year, month, day].join('-');
    }
    this.getOptions = function (sourceId, targetId, cablePosition, color, lineType) {
        try {
            var options = {};
            options.source = sourceId,
            options.target = targetId,
            options.connector = ["Flowchart", { cornerRadius: 10 }],
            options.paintStyle = { strokeStyle: color, lineWidth: lineType == 'tube' ? 8 : 1.5 }
            if (cablePosition == 'right') {
                options.anchor = ["LeftMiddle", "RightMiddle"]
            }
            return options;
        }
        catch (err) { }
    }
    this.createSourcePort = function () {
        try {
            var options = {};



            return options;
        }
        catch (err) { }
    }
    this.getEndPoints = function (endPointType) {
        try {
            var options = {};
            options.endpoint = ["Dot", { radius: 3 }];
            options.paintStyle = { fillStyle: "gray" };
            options.isSource = true;
            options.scope = "green dot";
            options.connectorStyle = { strokeStyle: "gray", lineWidth: 1 };
            options.connector = ["Bezier", { curviness: 100 }];
            options.maxConnections = 1
            if (endPointType == 'target') {
                options.isTarget = true;
                options.isSource = false;

                options.beforeDrop = function (params) {
                    var connectionId = $('#' + params.targetId).attr('data-connection-id');
                    if (parseInt(connectionId) > 0) { alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_064); return false; } else { return true; }// Port Already Occupied
                    //return confirm("Connect " + params.sourceId + " to " + params.targetId + "?");
                },

                options.dropOptions = { tolerance: "touch", hoverClass: "dropHover" };
                return options;
            } else { return options; }
        } catch (err) {

        }
    }
    this.getDefaultOptions = function () {
        var options = {};
        options.DragOptions = { cursor: 'pointer', zIndex: 2000 }
        options.PaintStyle = { strokeStyle: 'gray', lineWidth: 1 };
        options.EndpointStyle = { width: 20, height: 16, strokeStyle: 'gray' };
        options.Endpoint = ["Dot", { radius: 2 }];
        options.connector = ["Bezier", { curviness: 100 }];
        options.MaxConnections = 1       
        return options;
    }
    this.bindjsPlumbEvent = function (objJsPlumb) {
        objJsPlumb.bind("connection", function (info, originalEvent) {
            app.updateConnections(info.connection);
        });
        objJsPlumb.bind("connectionDetached", function (info, originalEvent) {
            //app.updateConnections(info.connection, true);
        });
    }
    this.addEndPoint = function () {
        try {
            
            jsPlumb.addEndpoint($(".legendsrc"), { anchor: "RightMiddle" }, app.getEndPoints('target'));
            jsPlumb.addEndpoint($(".src"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
            var jsp = jsPlumb.getInstance();
            jsPlumb.addEndpoint($(".trg"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
            if ($('#hdnIspArentExist').val() == '1') {
                app.jsp1 = jsPlumb.getInstance();
                app.jsp1.addEndpoint($(".src1"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
                app.jsp1.addEndpoint($(".trg1"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
                app.bindjsPlumbEvent(app.jsp1);
               
                app.jsp2 = jsPlumb.getInstance();
                app.jsp2.addEndpoint($(".src2"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
                app.jsp2.addEndpoint($(".trg2"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
                app.bindjsPlumbEvent(app.jsp2);

                app.jsp4 = jsPlumb.getInstance();
                app.jsp4.addEndpoint($(".src4"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
                app.jsp4.addEndpoint($(".trg4"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
                app.bindjsPlumbEvent(app.jsp4);
               
            } else {
                app.jsp6 = jsPlumb.getInstance();
                app.jsp6.addEndpoint($(".src1"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
                app.jsp6.addEndpoint($(".trg2"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
                app.bindjsPlumbEvent(app.jsp6);

                app.jsp7 = jsPlumb.getInstance();
                app.jsp7.addEndpoint($(".src6"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
                app.jsp7.addEndpoint($(".trg4"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
                app.bindjsPlumbEvent(app.jsp7);

                if($('.MidSection .ONT').length>0)
                {
                    app.jsp8 = jsPlumb.getInstance();
                    app.jsp8.addEndpoint($(".src1"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
                    app.jsp8.addEndpoint($(".trg6"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
                    app.bindjsPlumbEvent(app.jsp8);

                    app.jsp9 = jsPlumb.getInstance();
                    app.jsp9.addEndpoint($(".src7"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
                    app.jsp9.addEndpoint($(".trg4"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
                    app.b
                    indjsPlumbEvent(app.jsp9);
                }

            }
            app.jsp5 = jsPlumb.getInstance();
            app.jsp5.addEndpoint($(".src5"), { anchor: "RightMiddle" }, app.getEndPoints('source'));
            app.jsp5.addEndpoint($(".trg5"), { anchor: "LeftMiddle" }, app.getEndPoints('target'));
            app.bindjsPlumbEvent(app.jsp5);
        } catch (err) {
           
        }
    }
    this.getConnections = function () {
        var sourceId = $(app.DE.SourceCable).val();
        var connectorId = $(app.DE.ConnectingEntity).val();
        var destinationId = $(app.DE.DestinationCable).val();
        var eType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        var _data = { sourceId: parseInt(sourceId), connectorId: parseInt(connectorId), destinationId: destinationId == '' ? 0 : destinationId, connectorEntityType: eType };
        var res = ajaxReq('Splicing/getConnectionInfo', _data, false, function (data) {
            app.Connections=data;
            $.each(data, function (indx, item) {
                var sourceId = '';
                var targetId = '';
                if (item.source_entity_type.toLowerCase() == 'cable') {
                    sourceId = item.source_system_id + '_CORE_' + item.source_port_no;
                    var sourceTarget = app.getSourceTarget(item);
                    targetId = sourceTarget.target;
                    app.setConnectionId('', targetId, item.connection_id);
                } else if (item.destination_entity_type.toLowerCase() != 'cable') {
                    if ($('#hdnIspArentExist').val() == '1') {
                        if (parseInt($('#hdnChildCount').val()) > 0) {
                            var sourceTarget = app.getSourceTarget(item);
                            sourceId = sourceTarget.source;
                            targetId = sourceTarget.target;
                        } else {
                            var sourceTarget = app.getSourceTarget(item);
                            sourceId = sourceTarget.source;
                            targetId = sourceTarget.target;
                        }

                    } else {
                        var sourceTarget = app.getSourceTarget(item);
                        sourceId = sourceTarget.source;
                        targetId = sourceTarget.target;
                    }
                    app.setConnectionId(sourceId, targetId, item.connection_id);
                }

                if (item.destination_entity_type.toLowerCase() == 'cable') {
                    targetId = item.destination_system_id + '_CORE_' + item.destination_port_no;
                    var sourceTarget = app.getSourceTarget(item);
                    sourceId = sourceTarget.source;
                    app.setConnectionId(sourceId, '', item.connection_id);
                }
                app.setOccupiedPort(sourceId, targetId, item);
                app.createConnection(sourceId, targetId, item);
                app.setTitle(sourceId, targetId, item);               
                //app.setSourceNDestinationStatus(sourceId, targetId, item, data)
            });
            app.setCableTitle();
            $('.innerbndl').children('div').tooltip({ title: '', html: true, placement: "right" });
        }, true, false);
    }
    this.ExportConnections = function () {
        var sourceId = $(app.DE.SourceCable).val();
        var connectorId = $(app.DE.ConnectingEntity).val();
        var destinationId = $(app.DE.DestinationCable).val();
        var eType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        if (app.allConnectionId.length > 0) {
            window.location = appRoot + 'Splicing/ExportConnections?sourceId=' + sourceId + '&connectorId=' + connectorId + '&destinationId=' + destinationId + '&connectorEntityType=' + eType;
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_065);//Connections does not exist!
        }
    }

    this.createConnection = function (sourceId, targetId, item) {
        var cableColor = '#00ba8a';
        if ($('#' + targetId).length > 0 && $('#' + sourceId).length > 0) {
            app.allConnectionId.push(item.connection_id);
            app.mainJSPlumb = jsPlumb.getInstance();
            app.mainJSPlumb.importDefaults(app.getDefaultOptions());
            var cableOptions = app.getOptions(sourceId, targetId, 'Left', cableColor, 'core');
            cableOptions.connector = ["Bezier", { curviness: 50 }];
            cableOptions.paintStyle.lineWidth = 1;
            cableOptions.MaxConnections = 1;
            if (item.source_entity_type.toLowerCase() == 'cable' && item.source_system_id == parseInt($(app.DE.SourceCable).val())) { var cn2 = app.mainJSPlumb.connect(cableOptions); app.AllConnections.push(cn2); }
            else if (item.destination_entity_type.toLowerCase() == 'cable' && item.destination_system_id == parseInt($(app.DE.DestinationCable).val())) { var cn2 = app.mainJSPlumb.connect(cableOptions); app.AllConnections.push(cn2); }
            if (item.destination_entity_type.toLowerCase() != 'cable' && item.source_entity_type.toLowerCase() != 'cable') { var cn2 = app.mainJSPlumb.connect(cableOptions); app.AllConnections.push(cn2); }
            app.bindjsPlumbEvent(app.mainJSPlumb);
        }
        //app.AllConnections.push(cn2.getConnections());
    }
    this.setOccupiedPort = function (sourceId, targetId, item) {
        if ($('#' + targetId).length == 0 && item.source_entity_type.toLowerCase() == 'cable' && (item.source_system_id == parseInt($(app.DE.SourceCable).val()) || item.source_system_id != parseInt($(app.DE.SourceCable).val()))) {
            if ($('#hdnIspArentExist').val() == '1') 
            {
                app.jsp1.hide(sourceId, true);
            } 
            else 
            { 
                app.jsp6.hide(sourceId, true); 
                if(app.jsp8!=null){ app.jsp8.hide(sourceId, true); }
            }
        } else if ($('#' + targetId).length != 0 && item.source_entity_type.toLowerCase() == 'cable' && (item.source_system_id == parseInt($(app.DE.DestinationCable).val()))) {
            if ($('#hdnIspArentExist').val() == '1')
            {
                app.jsp1.hide(targetId, true);
            } 
            else 
            { 
                app.jsp7.hide(sourceId, true);                 
            }
        } else if ($('#' + targetId).length == 0) {
            if ($('#hdnIspArentExist').val() == '1') {
                app.jsp2.hide(sourceId, true);
                app.jsp4.hide(sourceId, true);                
            } else {
                app.jsp7.hide(sourceId, true); 
                if(app.jsp9!=null){ app.jsp9.hide(sourceId, true); }
                  
            }

            app.jsp5.hide(sourceId, true);

        }

        if (item.destination_entity_type.toLowerCase() == 'cable' && $('#' + sourceId).length == 0 && item.destination_system_id == $(app.DE.DestinationCable).val()) {
            if ($('#hdnIspArentExist').val() == '1') {
                app.jsp4.hide(targetId, true);
            } else {
                app.jsp7.hide(targetId, true); 
                if(app.jsp9!=null){ app.jsp9.hide(targetId, true);  }
              
            }

        } else if (item.destination_entity_type.toLowerCase() == 'cable' && item.destination_system_id == parseInt($(app.DE.SourceCable).val())) {
            if ($('#hdnIspArentExist').val() == '1') {
                app.jsp4.hide(sourceId, true);
            } else {
                app.jsp7.hide(sourceId, true);               
            }
        }
        else if ($('#' + sourceId).length == 0) {
            if ($('#hdnIspArentExist').val() == '1') {
                app.jsp1.hide(targetId, true);
                if (parseInt($('#hdnChildCount').val()) > 0) {
                    app.jsp2.hide(targetId, true);                    
                }
            } else {
                app.jsp6.hide(targetId, true); 
                if(app.jsp8!=null){  app.jsp8.hide(targetId, true);  }
               
            }
            app.jsp5.hide(targetId, true);
        }
    }
    this.getSourceTarget = function (item) {
        var sourceId = '';
        var targetId = '';
        if (item.source_port_no < 0 && item.destination_port_no < 0) {
            sourceId = item.source_system_id + '_' + item.source_entity_type.toLowerCase() + '_src_input_port_' + (-1 * item.source_port_no);
            targetId = item.destination_system_id + '_' + item.destination_entity_type.toLowerCase() + '_trg_input_port_' + (-1 * item.destination_port_no);
        } else if (item.source_port_no < 0 && item.destination_port_no > 0) {
            sourceId = item.source_system_id + '_' + item.source_entity_type.toLowerCase() + '_src_input_port_' + (-1 * item.source_port_no);
            targetId = item.destination_system_id + '_' + item.destination_entity_type.toLowerCase() + '_trg_output_port_' + (item.destination_port_no);
        } else if (item.source_port_no > 0 && item.destination_port_no < 0) {
            sourceId = item.source_system_id + '_' + item.source_entity_type.toLowerCase() + '_src_input_port_' + (item.source_port_no);
            targetId = item.destination_system_id + '_' + item.destination_entity_type.toLowerCase() + '_trg_input_port_' + (-1 * item.destination_port_no);
        } else {
            sourceId = item.source_system_id + '_' + item.source_entity_type.toLowerCase() + '_src_output_port_' + (item.source_port_no);
            targetId = item.destination_system_id + '_' + item.destination_entity_type.toLowerCase() + '_trg_output_port_' + (item.destination_port_no);
        }
        return { source: sourceId, target: targetId };
    }
    this.setConnectionId = function (sourceId, targetId, connectionId) {
        $('#' + sourceId).attr('data-connection-id', connectionId);
        $('#' + targetId).attr('data-connection-id', connectionId);
    }
    this.setTitle = function (sourceId, targetId, item) {
        if ((item.destination_entity_type.toLowerCase() == 'cable' && item.destination_system_id == parseInt($(app.DE.DestinationCable).val()))) {
            if(targetId.indexOf('CORE')==-1)
            {
                $('#' + targetId).attr('title', 'Connected to ' + item.source_network_id + '/' + (item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no))
                $('#' + targetId).tooltip({ title: '', html: false, placement: "right" });
            }
        }
        else if (item.source_entity_type.toLowerCase() == 'cable' && item.source_system_id == parseInt($(app.DE.SourceCable).val())) {
            if(sourceId.indexOf('CORE')==-1)
            {
                $('#' + sourceId).attr('title', 'Connected to ' + item.destination_network_id + '/' + (item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no));
                $('#' + sourceId).tooltip({ title: '', html: false, placement: "right" });
            }
        }
        else if (item.destination_entity_type.toLowerCase() == 'cable' && item.destination_system_id != parseInt($(app.DE.DestinationCable).val())) {
            if(sourceId.indexOf('CORE')==-1)
            {
                $('#' + sourceId).attr('title', 'Connected to ' + item.destination_network_id + '/' + (item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no));
                $('#' + sourceId).tooltip({ title: '', html: false, placement: "right" });
            }
        }
        else if (item.source_entity_type.toLowerCase() == 'cable' && item.source_system_id != parseInt($(app.DE.SourceCable).val())) {
            if(targetId.indexOf('CORE')==-1)
            {
                $('#' + targetId).attr('title', 'Connected to ' + item.source_network_id + '/' + (item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no))
                $('#' + targetId).tooltip({ title: '', html: false, placement: "right" });
            }
        }

        if (item.destination_entity_type.toLowerCase() != 'cable' && item.source_entity_type.toLowerCase() != 'cable') {
            $('#' + sourceId).attr('title', 'Connected to ' + item.destination_network_id + '/' + (item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no))
            $('#' + targetId).attr('title', 'Connected to ' + item.source_network_id + '/' + (item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no))
            $('#' + sourceId).tooltip({ title: '', html: false, placement: "right" });
            $('#' + targetId).tooltip({ title: '', html: false, placement: "right" });
        }

        //if (item.destination_entity_type.toLowerCase() == 'cable' && item.destination_system_id == parseInt($(app.DE.SourceCable).val())) {
        //    var coreId=item.destination_system_id+'_CORE_'+(item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no);
        //    //$('#' + coreId).attr('title', 'Connected to ' + item.source_network_id + '/' + (item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no))           
        //    $('#'+coreId).css('background-color','red');           
        //}
        //if (item.source_entity_type.toLowerCase() == 'cable' && item.source_system_id == parseInt($(app.DE.DestinationCable).val())) {
        //    var coreId=item.source_system_id+'_CORE_'+(item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no);
        //    //$('#' + coreId).attr('title', 'Connected to ' + item.destination_network_id + '/' + (item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no));
        //    $('#'+coreId).css('background-color','#1b9461');           
        //}
    }
     
    this.saveConnection = function (_data, sourceId, targetId) {
        ajaxReq('Splicing/SaveConnectionInfo', _data, true, function (resp) {
            $('#SlideMessage').text(MultilingualKey.SI_OSP_GBL_JQ_GBL_066);//Connection have saved successfully!
            $(".alert-info").slideDown(300);
            setTimeout(function () {
                $(".alert-info").slideUp(300);
            }, 2000)
            sourceId.attr('data-connection-id', resp.connection_id);
            targetId.attr('data-connection-id', resp.connection_id);
            app.allConnectionId.push(resp.connection_id);
        }, true, true);
    }
    this.saveBulkConnections = function (connections) {
        ajaxReq('Splicing/saveBulkConnections', { listConnections: connections }, true, function (resp) {

        }, true, true);
    }
    this.getConnectionsRequest = function (source, target) {
        var spliceSource='';
        if(isp!=null)
        {
            spliceSource='ISP_SPLICING';
        }else if(si!=null)
        {
            spliceSource='OSP_SPLICING';            
        }
        var connectionsInfo = [];
        if (target.attr('data-entity-type') == 'Splitter' || target.attr('data-entity-type') == 'ONT') {
            $.each(target.parent('.leftPort').siblings('.rightPort').children('.outPutPort[data-connection-id!="0"]'), function (indx, itm) {
                var _data = {
                    source_system_id: target.attr('data-system-id'),
                    source_network_id: target.attr('data-network-id'),
                    source_entity_type: target.attr('data-entity-type'),
                    source_port_no: target.attr('data-port-no'),

                    destination_system_id: $(this).attr('data-system-id'),
                    destination_network_id: $(this).attr('data-network-id'),
                    destination_entity_type: $(this).attr('data-entity-type'),
                    destination_port_no: parseInt($(this).attr('data-port-no')),
                    created_by: parseInt($('#hdnUserId').val()),
                    created_on: app.formatDate() + ' ' + new Date().toLocaleTimeString('en-US', { hour12: false }),
                    splicing_source:spliceSource
                };
                connectionsInfo.push(_data);
            });
        }
        if (source.attr('data-entity-type') == 'Splitter' || source.attr('data-entity-type') == 'ONT') {
            $.each(source.parent('.rightPort').siblings('.leftPort').children('.inputPort[data-connection-id!="0"]'), function (indx, itm) {
                var _data = {
                    source_system_id: $(this).attr('data-system-id'),
                    source_network_id: $(this).attr('data-network-id'),
                    source_entity_type: $(this).attr('data-entity-type'),
                    source_port_no: $(this).attr('data-port-no'),

                    destination_system_id: source.attr('data-system-id'),
                    destination_network_id: source.attr('data-network-id'),
                    destination_entity_type: source.attr('data-entity-type'),
                    destination_port_no: parseInt(source.attr('data-port-no')),
                    created_by: parseInt($('#hdnUserId').val()),
                    created_on: app.formatDate() + ' ' + new Date().toLocaleTimeString('en-US', { hour12: false }),
                    splicing_source:spliceSource
                };
                connectionsInfo.push(_data);
            });
        }
        return connectionsInfo;
    }
    this.deleteBulkConnections = function (connections) {
        if (connections.length > 0) {
            ajaxReq('Splicing/deleteBulkConnections', { listConnections: connections }, true, function (resp) { }, true, true);
        }
    }
    this.deleteConnection = function (connectionId, sourceId, targetId) {
        ajaxReq('Splicing/deleteConnection', { connectionId: connectionId }, true, function (resp) {
            if (resp.status == "OK") {
                $('#SlideMessage').text(MultilingualKey.SI_OSP_GBL_JQ_GBL_067);//Connection have deleted successfully!
                $(".alert-info").slideDown(300);
                setTimeout(function () {
                    $(".alert-info").slideUp(300);
                }, 2000)
                //$('#' + targetId).attr('data-connection-id', '0');
                //$('#' + sourceId).attr('data-connection-id', '0');
                $('.MidSection div[data-connection-id='+connectionId+']').attr('data-connection-id', '0');
                $('.leftCableSection div[data-connection-id='+connectionId+']').attr('data-connection-id', '0');
                $('.rightCableSection div[data-connection-id='+connectionId+']').attr('data-connection-id', '0');
                var index = osp.allConnectionId.indexOf(connectionId);
                app.allConnectionId.splice(index, 1);
            }
        }, true, true);
    }
    this.focusOnItem = function (ddlId) {
        $(ddlId + '_chosen a').attr('title', '');
        if ($(app.DE.DestinationCable).val() != '0' && $(app.DE.SourceCable).val() == $(app.DE.DestinationCable).val()) {
            $(app.DE.DestinationCable).val('0').trigger("chosen:updated");
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_068);//Please select diffirent cable!
            return false;
        }
        var systemID = $(ddlId).val();
        $(ddlId + '_chosen a').attr('title', $(ddlId + " :selected").text());
        var eType = $(ddlId + ' :selected').attr('data-entity-type');
        var gType = $(ddlId + ' :selected').attr('data-geom-type');
        var cableDirection = $(ddlId + ' :selected').attr('data-cable-direction');
        if (gType == 'Point' && app.gMapObj.pointObj != null) { app.gMapObj.pointObj.setMap(null); app.gMapObj.pointObj = null; }
        else if (gType == 'Line' && cableDirection == 'Source' && app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
        else if (gType == 'Line' && cableDirection == 'Destination' && app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; $(app.DE.SourceCable + ' option[value="' + $(app.DE.DestinationCable).val() + '"]').attr('disabled', true).trigger("chosen:updated"); }

        if (cableDirection == 'Source' && systemID == '0') { $(app.DE.DestinationCable + ' option').attr('disabled', false).trigger("chosen:updated"); return false; }
        else if (cableDirection == 'Destination' && systemID == '0') { $(app.DE.SourceCable + ' option').attr('disabled', false).trigger("chosen:updated"); return false; }
        if (cableDirection == 'Source') {
            $(app.DE.DestinationCable + ' option').attr('disabled', false).trigger("chosen:updated");
            var destOpt = app.DE.DestinationCable + ' option[value="' + $(app.DE.SourceCable).val() + '"]';
            $(destOpt).attr('disabled', true).trigger("chosen:updated");
        } else if (cableDirection == 'Destination') {
            $(app.DE.SourceCable + ' option').attr('disabled', false).trigger("chosen:updated");
            var sourceOpt = app.DE.SourceCable + ' option[value="' + $(app.DE.DestinationCable).val() + '"]';
            $(sourceOpt).attr('disabled', true).trigger("chosen:updated");
        }
        if (systemID == '0') return false;

        ajaxReq('main/getGeometryDetail', { systemId: systemID, entityType: eType, geomType: gType }, true, function (resp) {
            if (resp.status = 'OK') {
                if (resp.result != null && resp.result != undefined) {
                    var latLngArr = si.getLatLongArr(resp.result.sp_geometry);
                    app._focusMe(gType, latLngArr, cableDirection);
                }
            }
        }, true, false);
    }
    this._focusMe = function (geomType, latLngArr, cableDirection) {
        switch (geomType) {
            case 'Point':
                app.gMapObj.pointObj = si.createMarker(latLngArr[0], 'Content/images/dwnArrow.png');
                app.gMapObj.pointObj.setAnimation(google.maps.Animation.BOUNCE);
                app.gMapObj.pointObj.setMap(si.map);
                break;
            case 'Line':
                // PENDING : NEED TO CHECK THE SAME
            case 'Polygon':
                var lineObj = null;
                if (cableDirection == 'Source') {
                    app.gMapObj.sourceLineObj = si.createLine(latLngArr);
                    lineObj = app.gMapObj.sourceLineObj;
                } else if (cableDirection == 'Destination') {
                    app.gMapObj.destLineObj = si.createLine(latLngArr);
                    lineObj = app.gMapObj.destLineObj;
                }
                var _lineIcon = [{
                    icon: {
                        path: 'M -.5,-.5 .5,-.5, .5,.5 -.5,.5',
                        fillOpacity: 1,
                        fillColor: 'blue'
                    },
                    repeat: '8px'
                }];
                lineObj.strokeOpacity = 0;
                lineObj.strokeWeight = 4;
                lineObj.set('icons', _lineIcon);
                setTimeout(function () { animateLine(lineObj, 0) }, 20);
                lineObj.setMap(si.map);
                break;
        }
        //window.setTimeout(function () { if (app.gMapObj.infoHoverObj) { app.gMapObj.infoHoverObj.setMap(null) } app.gMapObj.infoHoverObj = null; }, 25000);
    }
    this.unSpliceAll = function () {
        if (app.AllConnections.length > 0) {
            //Are you sure,do you want to un-splice all connections!
            confirm(MultilingualKey.SI_OSP_GBL_JQ_GBL_069, function () {
                app.isAutoDeleteConnection = true;
                var sourceId = $(app.DE.SourceCable).val();
                var connectorId = $(app.DE.ConnectingEntity).val();
                var destinationId = $(app.DE.DestinationCable).val();
                var eType = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
                var _data = { sourceId: parseInt(sourceId), connectorId: parseInt(connectorId), destinationId: destinationId == '' ? 0 : destinationId, connectorEntityType: eType };
                var _data = {connectionIds:app.allConnectionId.join() };       
                var res = ajaxReq('Splicing/deleteAllConnection', _data, false, function (data) {
                    if (data.status == 'OK') {
                        $.each(app.AllConnections, function (indx, itm) {
                            jsPlumb.detach(itm);
                            app.setConnectionId(itm.sourceId, itm.targetId, '0');
                        });
                        setTimeout(function () { app.isAutoDeleteConnection = false; alert(data.message); }, 100);
                        app.AllConnections = [];
                        app.allConnectionId = [];
                    }
                });        
            });
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_065);//Connections does not exist!
        }
    }
    this.toggleConnectionUpload = function () {       
        var pageUrl = 'Splicing/UploadConnection';
        var modalClass = 'modal-sm';
        popup.LoadModalDialog('PARENT', pageUrl, { },'Bulk Splicing', modalClass);

    }
    this.setSourceNDestinationStatus = function (sourceId, targetId, item, data) {
        if(item.source_entity_type=='Cable')
        {   console.log(sourceId); 
            var portNo=(item.source_port_no < 0? (-1*item.source_port_no) : item.source_port_no);
            var index=0;
            //index= data.findIndex(p => p.destination_port_no ==portNo  && p.destination_entity_type==item.source_entity_type && p.destination_entity_type==item.source_system_id);
            //IE Solution
            index = data.findIndex(function (p) { return p.destination_port_no == portNo && p.destination_entity_type == item.source_entity_type && p.destination_entity_type == item.source_system_id; });
            if(index>0)
            {
                $('#'+sourceId).css('background-color','red');
            }
        }
    }

    this.getCustomer=function(pSystemId,pEntityType)
    {
        var res = ajaxReq('Splicing/getCustomer', { systemId: pSystemId,entityType:pEntityType }, false, function (data) {
            $.each(data,function(index,item){
                var options='<option value="@item.entity_system_id" data-cable-direction="Destination" data-geom-type="@item.geom_Type" data-entity-type="@item.entity_type">@(item.common_name + (item.total_core != null ? "-(" + item.total_core + ")" : ""))</option>';
            })
        }, true, false);
    }
    this.swapCables = function () {
        $('#divSpliceWindow').css('visibility', 'hidden');
        var sourceCable=$(app.DE.SourceCable).val();
        var destinationCable=$(app.DE.DestinationCable).val();

        $(app.DE.DestinationCable + ' option').attr('disabled', false);
        $(app.DE.SourceCable + ' option').attr('disabled', false);

        $(app.DE.DestinationCable).val(sourceCable).trigger("chosen:updated");
        $(app.DE.SourceCable).val(destinationCable).trigger("chosen:updated");

        var destOpt = app.DE.DestinationCable + ' option[value="' + $(app.DE.SourceCable).val() + '"]';
        $(destOpt).attr('disabled', true).trigger("chosen:updated");

      
        var sourceOpt = app.DE.SourceCable + ' option[value="' + $(app.DE.DestinationCable).val() + '"]';
        $(sourceOpt).attr('disabled', true).trigger("chosen:updated");
        //$('#closeModalPopup').click();
        //setTimeout(function(){$('#btnSplicing').click()},100);
 

        //$('.modal-backdrop').remove();
        //osp.splicingWindow();

        //innerSplicingWindow
        if ($(app.DE.ConnectingEntity).val() == '0') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_013);//Please select connecting element!
            return false;
        } else if ($(app.DE.SourceCable).val() == '0' && $(app.DE.DestinationCable).val() == '0') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_063)//Please select source or destination cable!
            return false;
        }else if($(app.DE.ConnectingEntity + ' :selected').attr('data-total-port') =='' || $(app.DE.ConnectingEntity + ' :selected').attr('data-total-port') && parseInt($(app.DE.ConnectingEntity + ' :selected').attr('data-total-port'))==0)
        {
            alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_021);//'Zero port does not allowed!
            return false;
        }
        var dataURL = 'Splicing/innerSplicingWindow';       
        var objSplicingInput = {
            SSystem_Id: parseInt($(app.DE.SourceCable).val()),
            DSystem_Id: parseInt($(app.DE.DestinationCable).val()),
            CenSystem_Id: parseInt($(app.DE.ConnectingEntity).val()),
            CenType: $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type')
        };        
        //ajaxReq(dataURL, objSplicingInput, true, function (response, status, xhr) {
        //    $('#divSpliceWindow').html(response );
        //    $('#dvProgressSplice').show();
        //    setTimeout(app.InitilizeJsPlumb, 300);                
        //    app.AllConnections = []; app.isAutoDeleteConnection = false; app.rightCoreCount = 0; app.allConnectionId = [];app.Connections=[];
        //}, true, true, false);


                $.ajax ({  
                    url: dataURL,
                    data:objSplicingInput,
                    contentType: 'application/html; charset=utf-8',  
                    type: 'GET' ,  
                    dataType: 'html'  
                })  
        .success (function (response) {  
            $('#divSpliceWindow').html(response );
            $('#dvProgressSplice').show();
            setTimeout(app.InitilizeJsPlumb, 300);                
            app.AllConnections = []; app.isAutoDeleteConnection = false; app.rightCoreCount = 0; app.allConnectionId = [];app.Connections=[];
        })  
        .error(function (xhr, status) {  
            alert(status) ;  
        });  




    }
    this.setCableTitle=function()
    {
        // var source=osp.Connections.filter(p=>p.source_entity_type=='Cable');
        //IE Soultion
        var source = osp.Connections.filter(function (p) { return p.source_entity_type == 'Cable'; });
        //var destination=osp.Connections.filter(p=>p.destination_entity_type=='Cable');
        //IE Solution
        var destination = osp.Connections.filter(function (p) { return p.destination_entity_type == 'Cable'; });
        var sourceDestination=[];

        $.each(source,function(index,item){
            var coreId=''
            if (item.source_entity_type.toLowerCase() == 'cable' && item.source_system_id == parseInt($(app.DE.DestinationCable).val())) {
                coreId=item.source_system_id+'_CORE_'+(item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no);                
                $('#'+coreId).addClass('greenColor');//.css('background-color','#1b9461');                                
            }else if (item.source_entity_type.toLowerCase() == 'cable' && item.destination_system_id !=$(app.DE.ConnectingEntity).val()) {
                coreId=item.source_system_id+'_CORE_'+(item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no);                
                $('#'+coreId).addClass('greenColor');//.css('background-color','#1b9461');    
            }
            var portNo=(item.source_port_no < 0? (-1*item.source_port_no) : item.source_port_no);
            //var check= destination.filter(p => (p.destination_port_no<0?(-1*p.destination_port_no):p.destination_port_no) ==portNo  && p.destination_entity_type==item.source_entity_type && p.destination_system_id==item.source_system_id);            
            //IE Solution
            var check = destination.filter(function (p) { return (p.destination_port_no < 0 ? (-1 * p.destination_port_no) : p.destination_port_no) == portNo && p.destination_entity_type == item.source_entity_type && p.destination_system_id == item.source_system_id; });
            if(check.length>0)
            {
                sourceDestination.push(item);                   
            }
            
            $('#' + coreId).attr('title', 'Connected to ' + item.destination_network_id + '/' + (item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no));
           
        })

        $.each(destination,function(index,item){
            var coreId='';
            if (item.destination_entity_type.toLowerCase() == 'cable' && item.destination_system_id == parseInt($(app.DE.SourceCable).val())) {
                coreId=item.destination_system_id+'_CORE_'+(item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no);                
                $('#'+coreId).addClass('redColor');//.css('background-color','red');                                  
            }else if (item.destination_entity_type.toLowerCase() == 'cable' && item.source_system_id !=$(app.DE.ConnectingEntity).val()) {
                coreId=item.destination_system_id+'_CORE_'+(item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no);                
                $('#'+coreId).addClass('redColor');//.css('background-color','red');
            }
            var portNo=(item.destination_port_no < 0? (-1*item.destination_port_no) : item.destination_port_no);
            // var check= source.filter(p => (p.source_port_no<0?(-1*p.source_port_no):p.source_port_no) ==portNo  && p.source_entity_type==item.destination_entity_type && p.source_system_id==item.destination_system_id);            
            //IE Solution
            var check = source.filter(function (p) { return (p.source_port_no < 0 ? (-1 * p.source_port_no) : p.source_port_no) == portNo && p.source_entity_type == item.destination_entity_type && p.source_system_id == item.destination_system_id; });
            if(check.length>0)
            {
                sourceDestination.push(item);                    
            }
            
            $('#' + coreId).attr('title', 'Connected to ' + item.source_network_id + '/' + (item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no))           
            
        })

        $.each(sourceDestination,function(index,item){
            
            var coreId='';
            if (item.source_entity_type.toLowerCase() == 'cable' && item.destination_system_id!=$(app.DE.ConnectingEntity).val()) {
                coreId=item.source_system_id+'_CORE_'+(item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no);                 
                $('#'+coreId).addClass('blueColor');
                         
            }
            if (item.destination_entity_type.toLowerCase() == 'cable'  && item.source_system_id!=$(app.DE.ConnectingEntity).val()) {
                coreId=item.destination_system_id+'_CORE_'+(item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no);              
                $('#'+coreId).addClass('blueColor');
                             
            }   
            if(item.source_entity_type.toLowerCase() == 'cable')
            {
                var coreId=coreId=item.source_system_id+'_CORE_'+(item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no);
                if($('#'+coreId).hasClass('redColor') || $('#'+coreId).hasClass('greenColor'))
                {
                    $('#'+coreId).removeClass('redColor').removeClass('greenColor');
                    $('#' + coreId).attr('title', '');
                }                                 
                if($('#'+coreId).hasClass('blueColor'))
                {
                    var title='Connected as S-' + item.destination_network_id + '/' + (item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no);
                    $('#' + coreId).attr('title', title); 
                }
            }
            if(item.destination_entity_type.toLowerCase() == 'cable')
            {
                var coreId=item.destination_system_id+'_CORE_'+(item.destination_port_no < 0 ? (-1 * item.destination_port_no) : item.destination_port_no);
                if($('#'+coreId).hasClass('redColor') || $('#'+coreId).hasClass('greenColor'))
                {
                    $('#'+coreId).removeClass('redColor').removeClass('greenColor');
                    $('#' + coreId).attr('title', '');
                }
                if($('#'+coreId).hasClass('blueColor'))
                {
                    var prevTitle=$('#' + coreId).attr('title');
                    var currentTitle=' Connected as D-' + item.source_network_id + '/' + (item.source_port_no < 0 ? (-1 * item.source_port_no) : item.source_port_no);
                    var title =(prevTitle!=''?prevTitle+'<br/>'+currentTitle:currentTitle);
                    $('#' + coreId).attr('title', title);       
                }
            }
        })

    }

    /*==============END CODE FOR SPLICING==============================*/
    this.downloadCPE = function () {
         
        window.location = appRoot + 'Splicing/DownloadConnectionPathFinderReport';
    }
    this.downloadConnectedCustomerDetails = function () {
         
        window.location = appRoot + 'Splicing/DownloadConnectedCustomerReport';
    }
    
    this.downloadOpticalLink = function () {
       
        window.location = appRoot + 'Splicing/DownloadOpticalLinkBudgetReport';
    }

    this.clearCFPGrid = function () {
        $(app.DE.equipmentid).val('');
        $(app.DE.ddlCore).val("0").trigger("chosen:updated");
        $(app.DE.entityid).val(0);
        $('#objFilterAttributes_port_no').val(0);
        $('#objFilterAttributes_entity_type').val('');
        app.clearCPFMarker();
        $(app.DE.btnShowRoute).click();

    }
    this.clearCPFMarker = function () {
        app.clearUserTempOverlay(gMapObj.TraceRoute);
        gMapObj.TraceRoute = [];
    }
    this.showPath = function () {
        ajaxReq('Splicing/GetCPFelementPath', {}, false, function (resp) {
            if (resp.status = 'OK') {
                if (resp.result != null && resp.result != undefined) {
                    $(popup.DE.MinimizeModel).click();
                    //  plotSinglePath(resp, '#ff0000', true)
                    var isBoth = true;
                    var _color = '#ff0000';
                    //if (gMapObj.TraceRoute && !isBoth)
                    app.clearUserTempOverlay(gMapObj.TraceRoute);

                    //if (!isBoth)    // for both path not clear TraceRoute and add into it..
                    gMapObj.TraceRoute = [];
                    app.gMapObj.pointMarkers = [];
                    var bounds = new google.maps.LatLngBounds();
                   

                    var highlightLineList=[];
                    for (var i = 0; i < resp.result.length; i++) {
                        _color = '#ff0000';
                        if (resp.result[i].en_type == 'Cable') {
                            
                         
                            //backward_path
                            var geometry = getLatLongArr(resp.result[i].sp_geometry);
                            for (var z = 0; z < geometry.length; z++) {
                                bounds.extend(geometry[z]);
                            }
                            var lineObj = si.createLine(geometry);
                            if (resp.result[i].backward_path == true)
                            {
                                _color='blue';
                            }
                            lineObj.strokeColor = _color;
                            var _lineIcon = [{
                                icon: {
                                    path: 'M -.5,-.5 .5,-.5, .5,.5 -.5,.5',
                                    fillOpacity: 1
                                    //fillColor: 'blue'
                                },
                                repeat: '6px'
                            }];

                            lineObj.strokeOpacity = 0;
                            lineObj.strokeWeight = 4;
                            lineObj.set('icons', _lineIcon);
                            highlightLineList.push(lineObj);
                            lineObj.setMap(si.map);
                            gMapObj.TraceRoute.push(lineObj);
                        }
                        else {
                           
                            var en_type = resp.result[i].en_type;
                            var system_id = resp.result[i].system_id;
                            var port_no = resp.result[i].port_no;
                            var network_id = resp.result[i].network_id;
                            var is_virtual_port_allowed = resp.result[i].is_virtual_port_allowed;

                            /// 
                            //    var oms = new OverlappingMarkerSpiderfier(si.map, {
                            //    markersWontMove: true,
                            //    markersWontHide: true,
                            //    keepSpiderfied: true,
                            //    circleFootSeparation: 35, // radius of circle 
                            //    nearbyDistance: 30, // distance in which it include the markers in spiderfier..
                            //    circleSpiralSwitchover: Infinity,//infinity= circle format, 0= spiral format with 9 element in one spiral round
                            //    legWeight: 2.4
                            //});



                            //var ptObj=undefined;
                            //var _points = jResp.Data.point;
                            //for (var i = 0; i < _points.length; i++) {
                            var geometry = getLatLongArr(resp.result[i].sp_geometry);
                            //check if marker already exist
                            //if(app.gMapObj.pointMarkers.filter(obj => { return obj.id === system_id && obj.eType==en_type }).length==0)
                            //IE Solution
                            if (app.gMapObj.pointMarkers.filter(function (obj) { return obj.id === system_id && obj.eType == en_type; }).length == 0)
                            {
                                var ptObj = si.createMarkerForPathFinder(geometry[0], 'Content/images/icons/lib/circle/' + resp.result[i].en_type + '.png', system_id, en_type, port_no, network_id, is_virtual_port_allowed);
                           
                                var SourceEquipmentId=$('#equipment_id').val().split(" ")[0];
                                //JIRA- SSSI-452 bug fix change done by Ram
                                bounds.extend(geometry[0]);
                                if(network_id ==SourceEquipmentId)
                                {
                               
                                    // current maps duration of one bounce (v3.13)
                                    ptObj.setAnimation(google.maps.Animation.BOUNCE);
                                    setTimeout(function () {
                                        ptObj.setAnimation(null);
                                    }, 700);
                                }
                                //ptObj.draggable=false;

                            

                                // add info window
                                var contentString = '<div id="content">' +
                                    '<input type="button" id="export" enType="' + en_type + '" systemId="' + system_id + '"  value="export"/>' +
                                    '</div>';


                                var objInfoWindow = new google.maps.InfoWindow();
                                var infowindow = new google.maps.InfoWindow({
                                    content: contentString
                                });
                           
                                google.maps.event.addListener(ptObj, 'click', (function (ptObj, i) {
                                    
                                    var PortNo=  ptObj.portNo>0? ptObj.portNo.toString() + " OUT": ptObj.portNo.toString().replace('-','') + " IN";
                                    return function () {

                                        var content = '<div><h4>' + ptObj.eType + ' Detail </h3><p><span  class="Info-content">Network Id : </span>' + ptObj.networkId + '</p>';
                                        if (!ptObj.isVirtualPortAllowed)
                                        {
                                            content+='<p><span  class="Info-content">Port No :</span> ' + PortNo + '</p>';
                                        }
                                        content+='<input class="btn btn-primary btn-xs" type="button" id="export" networkId="' + ptObj.networkId + '" portNo="' + ptObj.portNo + '" enType="' + ptObj.eType + '" systemId="' + ptObj.id + '"  value="Export connection detail "/></div>';
                                        objInfoWindow.setContent(content);
                                        objInfoWindow.open(si.map, ptObj);
                                    }
                                })(ptObj, i));
                                //oms.addMarker(ptObj);
                                ptObj.setMap(si.map);
                                gMapObj.TraceRoute.push(ptObj);
                                app.gMapObj.pointMarkers.push(ptObj);
                               
                            }
                            //}
                        }
                    }

                    // for highlight the  Path..
                    $.each(highlightLineList, function (index, item) {
                        setTimeout(function () { animateLine(item, 0) }, 20);
                    });

                    si.map.fitBounds(bounds);
                }
            }
        }, true, false);
    }

    this.clearUserTempOverlay = function (lyrObj) {
        if (lyrObj) {
            for (var i = 0; i < lyrObj.length; i++)
                lyrObj[i].setMap(null);
        }
        lyrObj = [];
    }
    this.SchematicView = function (obj) {
        popup.LoadModalDialog('PARENT', 'Splicing/SchematicView', { eType: '' }, "End To End Schematic View", 'modal-xl');
    }

    $(document).on("click", "#export", function () {
        var enType = $(this).attr('enType');
        var systemId = $(this).attr('systemId');
        var portNo = $(this).attr('portNo');;
        window.location = 'Splicing/exportConnectionPathForMarker?entity_type=' + enType + '&system_id=' + systemId + '&port_no=' + portNo;
    });




    // optical link budget with CPF

    this.OpticalLinkClearButton = function()
    {
        $('#ddlSourceNetworkid, #ddlDestinationNewtorkid, #ddl_waveLength').val('0').trigger('chosen:updated');
        $('#txt_transmit').val(0);
        $('#txt_receving').text('');
        $('#dvViewOpticalLinkBudgetDetails').empty();
        $('#btnopticalExport').attr('disabled', 'disabled');     
    }

 
    this.CalculateLinkBudgetDetail = function () {
        
        var  _entitysystemid =$(app.DE.entityid).val();
        var SourceEquipmentId=$('#equipment_id').val().split(" ")[0];
        var _equipmentid = SourceEquipmentId
        var _equipmentportid = $("#ddlCore").val();
        var _sourcenetworkid = $("#ddlSourceNetworkid option:selected").text();

        var _sourcesystemid = $("#ddlSourceNetworkid option:selected").val();
        var _sourceportno = $("#ddlSourceNetworkid option:selected").attr('data-portno');
        var _sourceentitytype = $("#ddlSourceNetworkid option:selected").attr('data-entityType');

        var _destinationnetworkid = $("#ddlDestinationNewtorkid option:selected").text();


        var _destinationsystemid = $("#ddlDestinationNewtorkid option:selected").val();
        var _destinationportno = $("#ddlDestinationNewtorkid option:selected").attr('data-portno');
        var _destinationentitytype = $("#ddlDestinationNewtorkid option:selected").attr('data-entityType');

        var  _Wavelengthid = $("#ddl_waveLength option:selected").val();
        var  _transmitpower = $("#txt_transmit").val();
        var _equipmenttype =$(app.DE.entity_type).val();

        if (_sourcenetworkid.toLowerCase() == '--select--') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_008);
            return false;
        }
        else if (_destinationnetworkid.toLowerCase() == '--select--') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_009);
            return false;
        }
        else if (_Wavelengthid.toLowerCase() == '--select--') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_010);
            return false;
        }

        else if (_transmitpower.toLowerCase() == '') {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_011);
            return false;
        }

        var data={ 
            equipmentsystemid: _equipmentid,
            equipmentPortid:_equipmentportid,
            sourcesystemid: _sourcesystemid, 
            sourceportno: _sourceportno,
            sourceentitytype:_sourceentitytype,
            destinationsystemid:_destinationsystemid,
            destinationportno:_destinationportno,
            destinationentitytype:_destinationentitytype,
            wavelengthid: _Wavelengthid,
            transmitpower: _transmitpower,
            equipmenttype : _equipmenttype,
            equipmentsystemid: _entitysystemid
        }

        ajaxReq('Splicing/GetLinkBudgetDetails',data, true, 
            function (resp) 
            {   
                $("#div_bindOpticalLinkGrid").html(resp);
                // $('#btnopticalExport').show();
                $('#txt_receving').text($("#hdnLastTransmitPower").val());
                //
                if($("#tblOpticalLinkBudgetDetails tbody tr").length>0)
                {
                    $("#btnopticalExport").removeAttr('disabled');
                }
            }, false, true);
    }


    // End optical link budget with CPF

    // Start Optical Link Budget Seprate Window
    this.OpticalLinkBudgetSeprate = function (obj) {
       
        popup.LoadModalDialog('PARENT', 'Splicing/OpticalLinkBudgetSeprate', { eType: '' }, "Optical Link Budget", 'modal-lg');
    }

    this.ddlopticalCore = function ddlopticalCore(obj) {
       
        $('#btnShowRoute').click();
        var selectedValue = $('#ddlCore option:selected').val();
        if (selectedValue == '0') {
            $(app.DE.port_no).val('0');
            $("#ddlCore_chosen").addClass('input-validation-error');
        }
        else {
            $(app.DE.port_no).val(selectedValue);
            $("#ddlCore_chosen").removeClass('input-validation-error');
        }
    }
    // ## EndO ptical Link Budget Seprate Window
}