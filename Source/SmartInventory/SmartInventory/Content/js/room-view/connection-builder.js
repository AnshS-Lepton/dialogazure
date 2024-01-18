const urlConnection = {
    getRacksData: 'ISP/GetRoomSpaceData',
    getRackChildren: 'ISP/GetRackChildren',
    getEquipmentChildren: 'ISP/GetEquipmentChildrenList',
    getChildModel: 'ISP/GetChildModelDetails',
    addConnection: 'Splicing/SaveConnectionInfo',
    removeConnection: 'Splicing/deleteConnection',
    //resetConnection: 'ISP/ResetConnection',
    GetEquipmentChildrenListSinglecount: 'ISP/GetEquipmentChildrenListSinglecount',
    getConnectedPorts: 'ISP/GetConnectedPorts',
    getModelPortConnections: 'ISP/GetModelPortConnections',
    getPortConnectionCount: 'ISP/GetPortConnectionCount',

};

var connectionBuilder = (function () {
    'use strict';
    var DE = {
        selectRackLeft: '#selectRackLeft',
        selectRackRight: '#selectRackRight',
        selectEquipmentRight: '#selectEquipmentRight',
        selectEquipmentLeft: '#selectEquipmentLeft',
        divTreeLeft: "#dvTreeLeft",
        divTreeRight: "#dvTreeRight",
        addConnection: "#addConnection",
        removeConnection: "#removeConnection",
        searchPoartA: "#searchPortA",
        searchPoartB: "#searchPortB",
        resetConnection: "#resetConnection",
        dvPortConnection: 'dvPortConnection',
        trayKey: '6',
        portKey: '5'
    };
    var _AllRackEquipment = [];
    var sourceData = {};
    var destinationData = {};
    var dataleft;
    var dataRight;
    var portConnectionLeft = {};
    var portConnectionRight = {};
    var action = {
        openConnectionContext: function () {
            render.connectionContext();
            event.onGetRack();
        },
        getRoomId: function () {
            let $room = $(DE.selectedRoomId);
            if ($room && $room.length) {
                return $room.val();
            }
            return -1;
        },
        getRack: function (res) {
            let param = {
                data: res,
                element: $(DE.selectRackLeft),
                value: 'db_id',
                text: 'name',
                defaultText: 'Select Rack',
                disabled: false
            };
            render.selector(param);
            param.element = $(DE.selectRackRight);
            render.selector(param);


        },
        getEquipmentLeft: function (res) {
            _AllRackEquipment = res.filter(x => x.model_id != DE.trayKey);
            let eqData = _AllRackEquipment.filter(x => x.parent == 1);
            let param = {
                data: eqData,
                element: $(DE.selectEquipmentLeft),
                value: 'db_id',
                text: 'name',
                defaultText: 'Select Equipment',
                disabled: false
            };
            render.selector(param);
            $(DE.divTreeLeft).html('');
        },
        getEquipmentRight: function (res) {
            _AllRackEquipment = res.filter(x => x.model_id != DE.trayKey);
            let eqData = _AllRackEquipment.filter(x => x.parent == 1);
            let param = {
                data: eqData,
                element: $(DE.selectEquipmentRight),
                value: 'db_id',
                text: 'name',
                defaultText: 'Select Equipment',
                disabled: false
            };
            render.selector(param);
            $(DE.divTreeRight).html('');
        },
        getEquipmentChildrenLeft: function (res) {
            sourceData = '';
            if ($('' + DE.selectEquipmentLeft + ' option:selected').text() != 'Select Equipment') {
                res[0].parent_id = 0;

            }
            dataleft = ''; dataleft = res;
            action.BindChildrenListData(DE.divTreeLeft, 'ulTreeLeft', res, 'spExpandAllLeft');

            var toggler = $(DE.divTreeLeft).find('.caret');
            var i;
            for (i = 0; i < toggler.length; i++) {

                toggler[i].addEventListener("click", function () {
                    let tmpData = this.parentElement.querySelector(".nested");
                    if (tmpData) {
                        tmpData.classList.toggle("active");
                    }
                    this.classList.toggle("caret-down");
                });
            }

            //Parent Child Toggle
            $("" + DE.divTreeLeft + " .caret").on("click", function () {

                if ($(this).hasClass('fa-plus-square')) {
                    $(this).removeClass("fa-plus-square").addClass("fa-minus-square");
                }
                else {
                    $(this).removeClass("fa-minus-square").addClass("fa-plus-square");
                }
            });


            //Expand all toggle
            $("#spExpandAllLeft").on("click", function () {

                if ($("#spExpandAllLeft").hasClass('fa-plus-square')) {
                    $("#spExpandAllLeft").removeClass("fa-plus-square").addClass("fa-minus-square");
                }
                else {
                    $("#spExpandAllLeft").removeClass("fa-minus-square").addClass("fa-plus-square");
                }


                if ($("#spExpandAllLeft").text() == 'Expand All') {
                    $("#spExpandAllLeft").text('Collapse All');
                }
                else {
                    $("#spExpandAllLeft").text('Expand All');
                }


                if ($("#spExpandAllLeft").text() == 'Expand All') {
                    $("" + DE.divTreeLeft + " .fa-minus-square").each(function () { $(this).trigger('click'); });
                }
                else {
                    $("" + DE.divTreeLeft + " .fa-plus-square").each(function () { $(this).trigger('click'); });
                }
            });


            //Port Selection
            let $li = $("" + DE.divTreeLeft + " .port").on("click", function () {

                $li.css('color', 'black');
                $(this).css('color', 'deepskyblue');
                API.call(urlConnection.getChildModel, { id: this.id }, action.PortSelectionLeft);


                //    for(var i =0;i<portConnectionLeft.length;i++)
                //    {
                //        $('.dvPortConnection table tbody').append('<tr><td>' + portConnectionLeft[i].destination_network_id + '</td><td>' + portConnectionLeft[i].destination_port_no + '</td></tr>');
                //    }


            });
            //action.getModelPortConnections({
            //    sourceId: $(DE.selectEquipmentLeft).val(),
            //    sourceType: 'Equipment',
            //    destinationId: '0',
            //    destinationType: ''
            //});
        },
        getEquipmentChildrenRight: function (res) {
            destinationData = '';
            if ($('' + DE.selectEquipmentRight + ' option:selected').text() != 'Select Equipment') {
                res[0].parent_id = 0;

            }
            dataRight = ''; dataRight = res;
            action.BindChildrenListData(DE.divTreeRight, 'ulTreeRight', res, 'spExpandAllRight');

            var toggler = $(DE.divTreeRight).find('.caret');

            var i;

            for (i = 0; i < toggler.length; i++) {

                toggler[i].addEventListener("click", function () {
                    let tmpData = this.parentElement.querySelector(".nested");
                    if (tmpData) {
                        tmpData.classList.toggle("active");
                    }

                    this.classList.toggle("caret-down");
                });
            }

            //Parent and Child List Toggle
            $("" + DE.divTreeRight + " .caret").on("click", function () {

                if ($(this).hasClass('fa-plus-square')) {
                    $(this).removeClass("fa-plus-square").addClass("fa-minus-square");
                }
                else {
                    $(this).removeClass("fa-minus-square").addClass("fa-plus-square");
                }
            });

            //Expand All toggle
            $("#spExpandAllRight").on("click", function () {

                if ($("#spExpandAllRight").hasClass('fa-plus-square')) {
                    $("#spExpandAllRight").removeClass("fa-plus-square").addClass("fa-minus-square");
                }
                else {
                    $("#spExpandAllRight").removeClass("fa-minus-square").addClass("fa-plus-square");
                }


                if ($("#spExpandAllRight").text() == 'Expand All') {
                    $("#spExpandAllRight").text('Collapse All');
                }
                else {
                    $("#spExpandAllRight").text('Expand All');
                }


                if ($("#spExpandAllRight").text() == 'Expand All') {
                    $("" + DE.divTreeRight + " .fa-minus-square").each(function () { $(this).trigger('click'); });
                }
                else {
                    $("" + DE.divTreeRight + " .fa-plus-square").each(function () { $(this).trigger('click'); });
                }
            });

            //Port Selection
            let $li = $("" + DE.divTreeRight + " .port").on("click", function () {

                $li.css('color', 'black');
                $(this).css('color', 'deepskyblue');
                API.call(urlConnection.getChildModel, { id: this.id }, action.PortSelectionRight);


            });
            //action.getModelPortConnections({
            //    sourceId: $(DE.selectEquipmentRight).val(),
            //    sourceType: 'Equipment',
            //    destinationId: '0',
            //    destinationType: ''
            //});
        },

        BindChildrenListData(dvID, ulID, data, spID) {
            var endMenu = getMenu(0);
            var Portcount;
            function getMenu(parentID) {
                return data.filter(function (node) { return (node.parent_id === parentID); }).map(function (node) {

                    var exists = data.some(function (childNode) {
                        return childNode.parent_id === node.child_id;
                    });

                    var subMenu = (exists) ? '<ul class="nested">' + getMenu(node.child_id).join('') + '</ul>' : "";

                    if (subMenu != "" || node.model_id != DE.portKey) {
                        //return '<li><span class="caret fa fa-plus-square"><i class="fa fa-circle" aria-hidden="true"></i>' + node.network_id + '  ( ' + node.child_name.toUpperCase() + ' )' + '</span>' + subMenu + '</li>';
                        return '<li><div><span class="caret fa fa-plus-square "></span><i class="fa fa-circle " aria-hidden="true"></i><span class="">' + node.network_id + '  ( ' + node.child_name.toUpperCase() + ' )' + '</span>' + subMenu + '</div></li>';
                    }
                    else {

                        if (node.connection_count == 0) {
                            return '<li class="port" id=' + node.child_id + ' style="cursor:pointer;">-- ' + node.network_id + '  ( ' + node.child_name.toUpperCase() + ' )' + '&nbsp;' + subMenu + '<input class="portColor" type="text" maxlength="2" value="' + node.connection_count + '" style="width:30px;height:18px;text-align:center;color:#02bc8f;" readonly />' +

                            '&nbsp;&nbsp;&nbsp;&nbsp; <div class="' + DE.dvPortConnection + '"></div></li>';  //
                        }
                        else {
                            return '<li class="port" id=' + node.child_id + ' style="cursor:pointer;">-- <span class="">' + node.network_id + '  ( ' + node.child_name.toUpperCase() + ' )' + '&nbsp;' + subMenu + '<input class="portColor" type="text" maxlength="2" value="' + node.connection_count + '" style="width:30px;height:18px;text-align:center;#4cd7ff;" readonly /></span>' +

                                '&nbsp;&nbsp;&nbsp;&nbsp; <div class="' + DE.dvPortConnection + '"></div></li>';  //

                        }
                    }
                });
            }
            $(dvID).html('');

            $(dvID).html('<ul class="treeView"><li><span id="' + spID + '" class="fa fa-plus-square" style="cursor:pointer;"><i class="fa fa-circle" aria-hidden-"true"></i>Expand All</li></ul> <ul id="' + ulID + '" class="treeView">' + endMenu.join('') + '</ul>');



        },

        searchPortA: function (value) {

            //let a = dataleft.filter(el => el.child_name.includes(value.toLowerCase()));
            //console.log(a);  


            //if (a.length==0){
            //    $('#lblmsgPortA').html('No record found!');
            //}
            //else{
            //    $('#lblmsgPortA').html('');

            //}
            let searchLen = 0;
            $("" + DE.divTreeLeft + " ul li").each(function () {

                if ($(this).text().search(new RegExp(value, "i")) < 0) {
                    $(this).hide();

                } else {
                    $(this).show();
                    searchLen++;
                }
            });

            if (searchLen == 0) {
                $('#lblmsgPortA').html('No record found!');
            }
            else {
                $('#lblmsgPortA').html('');

            }
        },

        searchPortB: function (value) {

            //let a = dataRight.filter(el => el.child_name.includes(value.toLowerCase()));
            //console.log(a);


            //if (a.length == 0) {
            //    $('#lblmsgPortB').html('No record found!');
            //}
            //else {
            //    $('#lblmsgPortB').html('');

            //}
            let searchLen = 0;
            $("" + DE.divTreeRight + "  ul li").each(function () {
                if ($(this).text().search(new RegExp(value, "i")) < 0) {
                    $(this).hide();
                } else {
                    $(this).show();
                    searchLen++;
                }
            });
            if (searchLen == 0) {
                $('#lblmsgPortB').html('No record found!');
            }
            else {
                $('#lblmsgPortB').html('');

            }
        },

        PortSelectionLeft: function (res) {

            sourceData = '';
            sourceData = res.lstEquipmentInfo;
            portConnectionLeft = res.lstPortConnection;
            let equipmentLeft = $(DE.selectEquipmentLeft).val();
            API.viewCall(urlConnection.getConnectedPorts, { parentId: equipmentLeft, portId: sourceData[0].system_id }, function (res) {
                render.connectedPorts(sourceData[0].system_id, res, '#ulTreeLeft');
            });

        },
        PortSelectionRight: function (res) {

            destinationData = '';
            destinationData = res.lstEquipmentInfo;

            let equipment = $(DE.selectEquipmentRight).val();
            API.viewCall(urlConnection.getConnectedPorts, { parentId: equipment, portId: destinationData[0].system_id }, function (res) {
                render.connectedPorts(destinationData[0].system_id, res, '#ulTreeRight');
            });

        },
        addConnection: function (res) {
            if (res.status) {

                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_082);//Connection has been saved successfully!
                //render.connectionCount(DE.divTreeLeft, "#" + sourceData[0].system_id + ">input.portColor");
                //render.connectionCount(DE.divTreeRight, "#" + sourceData[0].system_id + ">input.portColor");
                render.singlePortConnectionCount();
            }
            else {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_153);//Error Occured!
            }
        },
        setCountLeft: function (res, id) {
            id = id == undefined ? sourceData[0].system_id : id;
            $(DE.divTreeLeft + ' ' + '#' + id + ' .portColor').val('');
            $(DE.divTreeLeft + ' ' + '#' + id + ' .portColor').val(res);
        },
        setCountRight: function (res, id) {
            id = id == undefined ? destinationData[0].system_id : id;
            $(DE.divTreeRight + ' ' + '#' + id + ' .portColor').val('');
            $(DE.divTreeRight + ' ' + '#' + id + ' .portColor').val(res);
        },

        removeConnection: function (res) {
            if (res.status == "OK") {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_083);

            }
            else {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_153);//Error Occured!
            }
        },
        resetConnection: function (res) {

            if (res.status == "OK") {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_084);//All the connections are reset successfully!
                let equipmentLeft = $(DE.selectEquipmentLeft).val();
                let equipmentRight = $(DE.selectEquipmentRight).val();
                if (equipmentLeft) {
                    API.call(urlConnection.getEquipmentChildren, { equipmentID: equipmentLeft }, function (res) {
                        action.getEquipmentChildrenLeft(res);
                        if (equipmentRight) {
                            API.call(urlConnection.getEquipmentChildren, { equipmentID: equipmentRight }, action.getEquipmentChildrenRight);
                        }
                    });
                }

            }
            else {
                alert('Error Occured!');
            }
        },
        getModelPortConnections: function (param, callback) {
            //let param = {
            //    sourceId: '0',
            //    sourceType: 'Equipment',
            //    destinationId: '0',
            //    destinationType: ''
            //};
            //let sourceId = $(DE.selectEquipmentLeft).val();
            //let destinationId = $(DE.selectEquipmentRight).val();
            //if (sourceId && sourceId != '')
            //{
            //    param.sourceId = sourceId;
            //}
            //if (destinationId && destinationId != '') {
            //    param.destinationId = destinationId;
            //    param.destinationType = 'Equipment';
            //}
            API.call(urlConnection.getModelPortConnections, param, function (res) {
                //console.log(res);
                if (typeof callback == 'function') {
                    callback(res);
                }
            });

        },
        getConnectionCount: function (res, portId) {
            return res.filter(x=>(x.source_port_id == portId && x.destination_port_id) || (x.destination_port_id == portId && x.source_port_id)).length;
        },
        preAddConnection: function (res) {
            let connection = {
                source_system_id: sourceData[0].super_parent,
                source_network_id: sourceData[0].super_parent_network_id,
                source_entity_type: 'EQUIPMENT',
                source_port_no: sourceData[0].port_number,
                source_entity_sub_type: sourceData[0].super_parent_model_type,

                destination_system_id: destinationData[0].super_parent,
                destination_network_id: destinationData[0].super_parent_network_id,
                destination_entity_type: 'EQUIPMENT',
                destination_port_no: destinationData[0].port_number,
                destination_entity_sub_type: destinationData[0].super_parent_model_type,

                splicing_source: 'EQUIPMENT_SPLICING', //(si != null ? 'OSP_SPLICING' : 'ISP_SPLICING'), 
                is_source_cable_a_end: false,
                is_destination_cable_a_end: false,
                equipment_system_id: null,
                equipment_network_id: null,
                equipment_entity_type: null
            };
            API.call(urlConnection.addConnection, { objConnectionInfo: [connection] }, action.addConnection);
        }

    };


    var event = {
        onOpenConnectionContext: function () {
            action.openConnectionContext();
        },
        onGetRack: function () {
            let roomId = action.getRoomId();
            API.call(urlConnection.getRacksData, { parentId: roomId }, action.getRack)
        },
        onSelectRackLeft: function (e) {
            let $e = $(e.target);
            let rackId = $e.val();
            console.log($e.val());

            if ($(DE.selectRackLeft + ' option:selected').text() == 'Select Rack') {
                $(DE.divTreeLeft).html('');
                $(DE.selectEquipmentLeft).empty();
            }
            else {
                  API.call(urlConnection.getRackChildren, { rackId: rackId }, action.getEquipmentLeft);
            }


        },
        onSelectRackRight: function (e) {
            let $e = $(e.target);
            let rackId = $e.val();
            console.log($e.val());

            if ($(DE.selectRackRight + ' option:selected').text() == 'Select Rack') {
                $(DE.divTreeRight).html('');
                $(DE.selectEquipmentRight).empty();
            }
            else {
                API.call(urlConnection.getRackChildren, { rackId: rackId }, action.getEquipmentRight);
            }

        },
        onselectEquipmentLeft: function (e) {
            let $e = $(e.target);
            let equipmentID = $e.val();
            console.log($e.val());
            if (equipmentID != "" || equipmentID != 0) {
                API.call(urlConnection.getEquipmentChildren, { equipmentID: equipmentID }, action.getEquipmentChildrenLeft);
            }
            else {
                $(DE.divTreeLeft).html('')
            }


        },
        onselectEquipmentRight: function (e) {
            let $e = $(e.target);
            let equipmentID = $e.val();
            console.log($e.val());
            if (equipmentID != "" || equipmentID != 0) {
                API.call(urlConnection.getEquipmentChildren, { equipmentID: equipmentID }, action.getEquipmentChildrenRight);
            }
            else {
                $(DE.divTreeRight).html('')
            }
        },
        searchPortA: function (e) {
            let $e = $(e.target);
            let searchVal = $e.val();
            action.searchPortA(searchVal);
        },
        searchPortB: function (e) {
            let $e = $(e.target);
            let searchVal = $e.val();
            action.searchPortB(searchVal);
        },
        onAddConnection: function (e) {


            if ($(DE.selectRackLeft + ' option:selected').text() == 'Select Rack') {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_085);//Select Source Rack!
            }
            else if ($(DE.selectEquipmentLeft + ' option:selected').text() == 'Select Equipment' || $(DE.selectEquipmentLeft + ' option:selected').text() == "") {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_086);//Select Source Equipment!
            }
            else if ($.isEmptyObject(sourceData) == true) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_087);//Select Source Port!
            }
            else if ($(DE.selectRackRight + ' option:selected').text() == 'Select Rack') {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_088);//Select Destination Rack!
            }
            else if ($(DE.selectEquipmentRight + ' option:selected').text() == 'Select Equipment' || $(DE.selectEquipmentRight + ' option:selected').text() == "") {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_089);//Select Destination Equipment!
            }
            else if ($.isEmptyObject(destinationData) == true) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_090);//Select Destination Port!
            }
            else {
                if (sourceData[0].system_id == destinationData[0].system_id)
                { alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_091); }//Source and destination port must be different!
                else
                    validate.connectedPorts(sourceData[0].system_id, destinationData[0].system_id, action.preAddConnection);
            }

            //API.call(urlConnection.getPortConnectionCount, { parent_system_id: sourceData[0].parent_system_id, port_sequence_id: sourceData[0].system_id }, action.setCountLeft);
            //API.call(urlConnection.getPortConnectionCount, { parent_system_id: destinationData[0].parent_system_id, port_sequence_id: destinationData[0].system_id }, action.setCountRight);

        },
        removeConnection: function (e) {

            if ($(DE.selectRackLeft + ' option:selected').text() == 'Select Rack') {
                alert( MultilingualKey.SI_OSP_GBL_JQ_GBL_085);//'Select Source Rack!
            }
            else if ($(DE.selectEquipmentLeft + ' option:selected').text() == 'Select Equipment' || $(DE.selectEquipmentLeft + ' option:selected').text() == "") {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_086);//Select Source Equipment!
            }
            else if ($.isEmptyObject(sourceData) == true) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_087);//Select Source Port!
            }
            else if ($(DE.selectRackRight + ' option:selected').text() == 'Select Rack') {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_088);//Select Destination Rack!
            }

            else if ($(DE.selectEquipmentRight + ' option:selected').text() == 'Select Equipment' || $(DE.selectEquipmentRight + ' option:selected').text() == "") {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_089);//Select Destination Equipment!
            }
            else if ($.isEmptyObject(destinationData) == true) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_090);//Select Destination Port!
            }
            else if (sourceData[0].system_id == destinationData[0].system_id) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_092);//No connection found!
            }
            else {
                let deleteconnection = {
                    source_system_id: sourceData[0].super_parent,
                    source_entity_type: 'EQUIPMENT',
                    source_port_no: sourceData[0].port_number,

                    destination_system_id: destinationData[0].super_parent,
                    destination_entity_type: 'EQUIPMENT',
                    destination_port_no: destinationData[0].port_number,

                    is_source_cable_a_end: false,
                    is_destination_cable_a_end: false,
                    equipment_system_id: null,
                    equipment_entity_type: null
                };
                //Are you sure you want remove connection?
                confirm(MultilingualKey.SI_OSP_GBL_JQ_GBL_093, function () {
                    API.call(urlConnection.removeConnection, { objConnectionInfo: [deleteconnection] },
                        function (res) {
                            action.removeConnection(res);
                            render.singlePortConnectionCount();
                        });

                });

            }

            //API.call(urlConnection.GetEquipmentChildrenListSinglecount, { parent_system_id: sourceData[0].parent_system_id, port_sequence_id: sourceData[0].sequence_id }, action.setCountLeft);
            //API.call(urlConnection.GetEquipmentChildrenListSinglecount, { parent_system_id: destinationData[0].parent_system_id, port_sequence_id: destinationData[0].sequence_id }, action.setCountRight);

            render.singlePortConnectionCount();
        },

        onResetConnection: function (e) {
            //if only one equipment is selected
            //If  two equipment is selected
            let equipmentLeft = $(DE.selectEquipmentLeft).val();
            let equipmentRight = $(DE.selectEquipmentRight).val();

            if (!equipmentLeft || !equipmentRight) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_094);//Please select equipment in both side!
                return false;
            }
            //Are you sure you want to reset all connections?
            confirm("<b>Warning</b> <br> "+ MultilingualKey.SI_OSP_GBL_JQ_GBL_095, function () {


                let param = {
                    sourceId: equipmentLeft,
                    sourceType: 'Equipment',
                    destinationId: equipmentRight || '0',
                    destinationType: (equipmentRight) ? 'Equipment' : ''
                };
                if (param.sourceId == param.destinationId) {
                    param.destinationId = 0;
                    param.destinationType = '';
                }
                action.getModelPortConnections(param, function (res) {
                    let allConnections = [];
                    let filterCon = res.filter(x=>x.destination_port_id != null);
                    let conLen = filterCon.length;
                    for (let i = 0 ; i < conLen; i++) {
                        
                        allConnections.push({
                            source_system_id: filterCon[i].system_id,
                            source_entity_type: 'EQUIPMENT',
                            source_port_no: filterCon[i].core_port_number,

                            destination_system_id: filterCon[i].destination_system_id,
                            destination_entity_type: 'EQUIPMENT',
                            destination_port_no: filterCon[i].destination_port_no,

                            is_source_cable_a_end: false,
                            is_destination_cable_a_end: false,
                            equipment_system_id: null,
                            equipment_entity_type: null
                        });
                    }
                    if (allConnections.length > 0) {
                        API.call(urlConnection.removeConnection, { objConnectionInfo: allConnections }, action.resetConnection);
                    }
                    else {
                        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_092, 'error');//No connection found!
                    }
                });

            });

        }
    };
    var API = {
        call: function (url, param, callback) {
            ajaxReq(url, param, true, function (res) {
                if (callback && typeof callback == 'function')
                    callback(res);
            }, true, true, true);
        },
        viewCall: function (url, param, callback) {
            ajaxReq(url, param, true, function (res) {
                if (callback && typeof callback == 'function')
                    callback(res);
            }, true, true, false);
        },
    };
    var render = {
        connectionContext: function () {
            let $connectionContext = $(DE.connectionContext);
            $connectionContext.show();
        },
        /*DO NOT DELETE: Render selector input value sample
                {
                    data: res.result,
                    element: $(DE.selectVender),
                    value: 'value',
                    text: 'key',
                    defaultText: 'Select Vendor',
                    dataKey: [{ key: 'item_template_id', name: 'data-item-id' }],
                    disabled: false
                }
*/
        selector: function (d) {

            let option = "";
            if (d.defaultText)
                //option = "<option value disabled selected hidden>" + d.defaultText + "</option>";
                option = "<option value  selected >" + d.defaultText + "</option>";
            if (d && d.data) {
                let len = d.data.length;
                let keys = "";
                if (len > 0) {
                    for (let i = 0; i < len; i++) {
                        keys = "";
                        if (d.dataKey && d.dataKey.length) {
                            for (let j = 0; j < d.dataKey.length; j++) {
                                keys += " " + d.dataKey[j].name + "=" + d.data[i][d.dataKey[j].key];
                            }
                        }
                        option += "<option value='" + d.data[i][d.value] + "' " + keys + " >" + d.data[i][d.text] + "</option>";
                    }
                }
            }
            d.element.attr('disabled', d.disabled);
            d.element.html(option);
        },
        rack: function (d) {
            render.selector(d);
        },
        connectedPorts: function (id, res, context) {
            if (context)
            { $(context).find('#' + id + ' .' + DE.dvPortConnection).html(res); }
            else { $('#' + id + ' .' + DE.dvPortConnection).html(res); }
        },
        connectionCount: function (treeId, id) {
            let $counter = $(treeId).find(id);

            let count = $counter.val();
            $counter.val(count++);
        },
        singlePortConnectionCount: function () {
            let leftEq = $(DE.selectEquipmentLeft).val();
            let rightEq = $(DE.selectEquipmentRight).val();
            if (leftEq && leftEq != '' && sourceData[0]) {

                API.viewCall(urlConnection.getConnectedPorts, { parentId: leftEq, portId: sourceData[0].system_id }, function (res) {
                    render.connectedPorts(sourceData[0].system_id, res, '#ulTreeLeft');
                    if (leftEq == rightEq) {
                        //if (validate.isConnectedPortsRendered(sourceData[0].system_id), '#ulTreeRight')
                        render.connectedPorts(sourceData[0].system_id, res, '#ulTreeRight');
                    }

                    API.call(urlConnection.getPortConnectionCount, {
                        parentId: leftEq, portId: sourceData[0].system_id
                    }, function (res) {
                        action.setCountLeft(res);
                        if (leftEq == rightEq) {
                            action.setCountRight(res, sourceData[0].system_id);
                        }
                    });
                });
            }

            if (rightEq && rightEq != '' && destinationData[0]) {

                API.viewCall(urlConnection.getConnectedPorts, { parentId: rightEq, portId: destinationData[0].system_id }, function (res) {
                    render.connectedPorts(destinationData[0].system_id, res, '#ulTreeRight');
                    if (leftEq == rightEq) {
                        //if (validate.isConnectedPortsRendered(destinationData[0].system_id), '#ulTreeLeft')
                        render.connectedPorts(destinationData[0].system_id, res, '#ulTreeLeft');
                    }

                    API.call(urlConnection.getPortConnectionCount, {
                        parentId: rightEq, portId: destinationData[0].system_id
                    }, function (res) {
                        action.setCountRight(res);
                        if (leftEq == rightEq) {
                            action.setCountLeft(res, destinationData[0].system_id);
                        }
                    });
                });
            }

        }

    };
    var validate = {
        connectedPorts: function (sourceId, destinationId, callback) {
            let equipmentLeft = $(DE.selectEquipmentLeft).val();
            let equipmentRight = $(DE.selectEquipmentRight).val();
            let param = {
                sourceId: equipmentLeft,
                sourceType: 'Equipment',
                destinationId: equipmentRight || '0',
                destinationType: (equipmentRight) ? 'Equipment' : ''
            };
            if (param.sourceId == param.destinationId) {
                param.destinationId = 0;
                param.destinationType = '';
            }
            action.getModelPortConnections(param, function (res) {
                let filterCon = res.filter(x=> (x.source_port_id == sourceId && x.destination_port_id == destinationId) || (x.source_port_id == destinationId && x.destination_port_id == sourceId));
                if (filterCon.length == 0) {
                    if (typeof callback == 'function') {
                        callback(res);
                    }
                }
                else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_096, 'error');//Connection already exists!
                }
            });
        },
        isConnectedPortsRendered: function (id,context) {
            let count = $(context).find('#' + id).find('tr').length;
            return count > 0;
        }
    };
    var bind = {

        connectionOperation: function () {
            let $linkConnection = $(DE.linkConnection);
            $linkConnection.on('click', event.onOpenConnectionContext);

            let $rackLeft = $(DE.selectRackLeft);
            $rackLeft.on('change', event.onSelectRackLeft);

            let $rackRight = $(DE.selectRackRight);
            $rackRight.on('change', event.onSelectRackRight);

            let $selectEquipmentLeft = $(DE.selectEquipmentLeft);
            $selectEquipmentLeft.on('change', event.onselectEquipmentLeft);

            let $selectEquipmentRight = $(DE.selectEquipmentRight);
            $selectEquipmentRight.on('change', event.onselectEquipmentRight);

            let $addConnection = $(DE.addConnection);
            $addConnection.on('click', event.onAddConnection);

            let $searchPortA = $(DE.searchPoartA);
            $searchPortA.on('keyup', event.searchPortA);

            let $searchPortB = $(DE.searchPoartB);
            $searchPortB.on('keyup', event.searchPortB);

            let $removeConnection = $(DE.removeConnection);
            $removeConnection.on('click', event.removeConnection);

            let $resetConnection = $(DE.resetConnection);
            $resetConnection.on('click', event.onResetConnection);


        }
    };

    var load = {
        rack: function () {
            //let roomId = action.getRoomId();
            //API.call(urlConnection.getRacksData, { parentId  : roomId}, action.getRack)
        }
    };
    var setup = function () {

        //Load Data
        for (let e in load) {
            if (typeof load[e] == 'function')
                load[e]();
        }
        //Bind events
        for (let e in bind) {
            if (typeof bind[e] == 'function')
                bind[e]();
        }
    };
    var init = function (configs) {
        $.extend(true, DE, configs);
        setup();
    };
    return {
        init: init
    };

})();
