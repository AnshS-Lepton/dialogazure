var FeasibilityHistory = function () {
    var past = this;
    this.tablePastFeas = undefined;
    this.feasDetailsData = undefined;
    this.rowData = undefined;
    this.cableLengths = {};
    this.DE = {
        "tblPastFeas": "#tblPastFeas",
        "dataTable": ".dataTable",
        "tblBody": "#tblPastFeas tbody",
        "tabsDiv": "#tabs",
        "txtHistoryID": "#_historyID",
        "txtFeasibilityID": "#_feasibilityID",
        "txtfeasibilityName": "#_feasibilityName",
        "txtHistoryID": "#_historyID",
        "txtcustomerName": "#_customerName",
        "txtStartName": "#_startPointName",
        "txtStartLocation": "#_startLocation",
        "txtEndName": "#_endPointName",
        "txtEndLocation": "#_endLocation",
        "txtInside_A_Length": "#_inside_A_Length",
        "txtInside_P_Length": "#_inside_P_Length",
        // "txtInsideLength": "#_insideLength",
        // "txtOutsideLength": "#_outsideLength",
        "txtOutside_A_Length": "#_outside_A_Length",
        "txtOutside_B_Length": "#_outside_B_Length",
        "txtTotalLength": "#_totalLength",
        "txtCustomerID": "#_customerID",
        "txtLMC_A": "#_lmcA",
        "txtLMC_B": "#_lmcB",
        "txtRequiredCores": "#_requiredCores",
        "txtBufferRadA": "#_bufferRadA",
        "txtBufferRadB": "#_bufferRadB",
        "btnDisplayFeas": "#btnDisplayFeas",
        "btnComputeAgain": "#btnComputeAgain",
        "btnDisplayFeasFtth": "#btnDisplayFeasFtth",
        "btnComputeAgainFtth": "#btnComputeAgainFtth",
        "btnClosePastFeas": "#closeModalPopup"
    };
    this.tableColumns = [{ title: "" }, { title: "" }, { title: "" }, { title: MultilingualKey.Feasibility_ID }, { title: "" }, { title: MultilingualKey.Customer_Name }, { title: MultilingualKey.Start_Point_Name }, { title: MultilingualKey.Start_Point }, { title: MultilingualKey.End_Point_Name }, { title: MultilingualKey.End_Point },
                            { title: "" }, { title: MultilingualKey.Existing_Length }, { title: MultilingualKey.New_Length }, { title: MultilingualKey.Created_On }, { title: "" },
    { title: "" }, { title: "" }, { title: "" }, { title: "" }, { title: "" }];

    this.wrapperDiv = '<div class="dataTables_scroll" />';

    this.columnDefaults = [
    {
        "targets": 0,
        "data": null,
        "orderable": false,
        "defaultContent": "<input type='radio' class='form-check-input rdbPastFeas' name='rdbRowRadios'>",
        "className": "dt-center"
    },
    { "targets": [1], "visible": false }, { "targets": [2], "visible": false }, { "targets": [4], "visible": false }, { "targets": [10], "visible": false }, { "targets": [11], "visible": true }, { "targets": [12], "visible": true }, { "targets": [13], "visible": true },
    { "targets": [14], "visible": false }, { "targets": [15], "visible": false }, { "targets": [16], "visible": false }, { "targets": [17], "visible": false }, { "targets": [18], "visible": false }, { "targets": [19], "visible": false }];

    this.bindEvents = function () {
        // var btnClickedEvent = "";
        $(past.DE.btnDisplayFeas).on("click", function () {
            past.displayPastfeas();
            // btnClickedEvent = "display";
        });

        $(past.DE.btnComputeAgain).on("click", function () {
            // btnClickedEvent = "compute";
            var startLL = past.rowData.start_lat_lng.split(',');
            var endLL = past.rowData.end_lat_lng.split(',');
            sf.clearMap();

            sf.populateInputFields({
                customer_name: past.rowData.customer_name,
                customer_id: past.rowData.customer_id,
                feasibility_id: past.rowData.feasibility_name,
                start_lat: startLL[1],
                start_lng: startLL[0],
                end_lat: endLL[1],
                end_lng: endLL[0],
                start_buffer: past.rowData.buffer_radius_a,
                end_buffer: past.rowData.buffer_radius_b,
                cores_required: past.rowData.cores_required,
                cable_type_id: past.rowData.cable_type_id,
                start_point_name: past.rowData.start_point_name,
                end_point_name: past.rowData.end_point_name

            });

            if (!sf.validateInputFields()) {
                alert('Invalid Data!');
                return;
            }
            else {
                sf.compute();
                sf.DisableInputfields();
            }
        });

        $(past.DE.btnDisplayFeasFtth).on("click", function () {
            past.displayPastfeasFtth();
            // btnClickedEvent = "display";
        });

        /*$(past.DE.btnClosePastFeas).on("click", function () {
            if (btnClickedEvent == "display")
            {
                sf.clearMap();
            }            
        });*/
    }

    this.getFinalData = function (result) {
        var endData = result;
        var finalData = [];
        for (var ed of endData) {
            var a = [];
            a.push('');
            for(key of Object.keys(ed)) {
                a.push(ed[key]);
            }

            finalData.push(a);
        }

        return finalData;
    }

    this.getDescriptionHTML = function (subPath) {
        let title = sf.getTitle(subPath.cable_type) + ' Network';
        let distance = sf.getDistanceString(subPath.cable_length);

        let desc = '<![CDATA[<p><b>Distance:</b> ' + distance + '<br/>';
        desc += (subPath.cable_id ? ('<b>Cable ID: </b>' + subPath.cable_id) + '<br/>' : '');
        desc += (subPath.cable_name ? ('<b>Cable Name: </b>' + subPath.cable_name) : '');
        desc += '</p>]]>';

        return desc;
    }

    this.processPastKMLData = function () {
        let data = past.feasDetailsData;
        let tableData = past.rowData;
        let KMLData = [];

        if (tableData && data) {
            var startPoint = tableData.start_lat_lng;
            var endPoint = tableData.end_lat_lng;
            var ll1 = startPoint.trim().split(',');
            var ll2 = endPoint.trim().split(',');

            for(item of data) {
                var geom = '';
                var geomArray = item.cable_geometry.replace('LINESTRING(', '').replace(')', '').split(',');
                for(g of geomArray) {
                    var latLng = g.trim().split(' ');
                    geom += latLng[0] + ',' + latLng[1] + ',0 ';
                }

                if (geom.length) {
                    KMLData.push({
                        cable_id: item.cable_id,
                        cable_name: item.cable_name,
                        geom: geom,
                        description: past.getDescriptionHTML(item),
                        geom_type: 'LINE',
                        entity_name: item.cable_type,
                        entity_title: sf.getTitle(item.cable_type),
                        entity_type: item.cable_type,
                        distance: item.cable_length,
                        colorHex_8: sf.getKMLHexCode(sf.getColorByType(item.cable_type))
                    });
                }
            }

            if (ll1.length) {
                KMLData.push({
                    geom: ll1[0] + ',' + ll1[1] + ',0 ',
                    description: '<![CDATA[<h2>(' + ll1[0] + ',' + ll1[1] + ')</h2>]]>',
                    geom_type: 'POINT',
                    entity_name: 'Start Point',
                    entity_title: (tableData.start_point_name != null && tableData.start_point_name.length > 0) ? tableData.start_point_name : 'Start Point'
                });
            }
            if (ll2.length) {
                KMLData.push({
                    geom: ll2[0] + ',' + ll2[1] + ',0 ',
                    description: '<![CDATA[<h2>(' + ll2[0] + ',' + ll2[1] + ')</h2>]]>',
                    geom_type: 'POINT',
                    entity_name: 'End Point',
                    entity_title: (tableData.end_point_name != null && tableData.end_point_name.length > 0) ? tableData.end_point_name : 'End Point'
                });
            }
        }
        sf.pastKMLData = KMLData;
    }

    this.displayGeometries = function (data, tableData) {
        if (tableData) {
            var startPoint = tableData.start_lat_lng;
            var endPoint = tableData.end_lat_lng;

            sf.resetFields();

            for(item of data) {

                var path = [];
                var geom = item.cable_geometry.replace('LINESTRING(', '').replace(')', '').split(',');
                var system_id = item.edge_TargetID
                for(g of geom) {
                    var latLng = g.trim().split(' ');
                    path.push(new google.maps.LatLng(latLng[1], latLng[0]));
                }

                sf.savedPaths.push(new google.maps.Polyline({
                    path: path,
                    length: google.maps.geometry.spherical.computeLength(path),
                    strokeColor: sf.getColorByType(item.cable_type),
                    strokeOpacity: 1.0,
                    strokeWeight: 5,
                    zIndex: 10,
                    system_id: item.system_id,
                    network_status: item.network_status,
                    cable_type: item.cable_type,
                    cable_length: item.cable_length
                }));
            }

            // end point markers
            if (startPoint && endPoint) {
                var ll1 = startPoint.trim().split(',');
                var ll2 = endPoint.trim().split(',');
                var startLoc = new google.maps.LatLng(ll1[1], ll1[0]);
                var endLoc = new google.maps.LatLng(ll2[1], ll2[0]);

                //sf.startMarker_saved = new google.maps.Marker({
                //    position: startLoc,
                //    icon: '',
                //    draggable: false,
                //    title: 'A End',
                //    label: 'A',
                //    map: sf.map
                //});;
                sf.startMarker_saved = sf.createMarker(startLoc, 'Content/images/StartPoint.png', 'A');
                //sf.endMarker_saved = new google.maps.Marker({
                //    position: endLoc,
                //    icon: '',
                //    draggable: false,
                //    title: 'B End',
                //    label: 'B',
                //    map: sf.map
                //});;
                sf.endMarker_saved = sf.createMarker(endLoc, 'Content/images/EndPoint.png', 'B');

                // add buffer circles
                sf.addDistanceBuffer_A(1, startLoc, sf.startMarker_saved);
                sf.addDistanceBuffer_B(1, endLoc, sf.endMarker_saved);
                sf.updateBufferRadius(sf.DE.end_type.START, tableData.buffer_radius_a ? tableData.buffer_radius_a : 0);
                sf.updateBufferRadius(sf.DE.end_type.END, tableData.buffer_radius_b ? tableData.buffer_radius_b : 0);

                sf.setMapForSavedPaths();

                var bounds = new google.maps.LatLngBounds();
                bounds.extend(startLoc);
                bounds.extend(endLoc);
                sf.map.fitBounds(bounds);
                sf.map.setZoom(sf.map.getZoom() - 1);
            }
        }
    }

    this.fillLengths = function (data) {
        past.cableLengths = { insideLength: 0, outsideLength: 0, lmc_A_length: 0, lmc_B_length: 0, totalLength: 0 };
        if (data.length) {
            for(item of data) {
                if (item.cable_type.toLowerCase().startsWith('inside')) {
                    past.cableLengths.insideLength += item.cable_length;
                }
                else if (item.cable_type.toLowerCase() == 'outside') {
                    past.cableLengths.outsideLength += item.cable_length;
                }
                else if (item.cable_type.toLowerCase() == 'lmc_start') {
                    past.cableLengths.lmc_A_length += item.cable_length;
                }
                else if (item.cable_type.toLowerCase() == 'lmc_end') {
                    past.cableLengths.lmc_B_length += item.cable_length;
                }
                past.cableLengths.totalLength += item.cable_length;
            }
        }
    }

    this.getInsideCableData = function () {
        var data = [];
        if (past.feasDetailsData.length) {
            for(cable of past.feasDetailsData) {
                if (cable && cable.cable_id.trim().length) {
                    data.push(
                        {
                            cable_id: cable.cable_id,
                            cable_name: cable.cable_name,
                            network_status: cable.network_status,
                            total_cores: cable.total_cores,
                            used_cores: cable.total_cores - cable.available_cores,
                            available_cores: cable.available_cores,
                            cable_length: cable.cable_length.toFixed(2)
                        });
                }
            }
        }
        return data;
    }

    this.fillAppData = function () {
        if (past.rowData && past.feasDetailsData.length) {
            past.fillLengths(past.feasDetailsData);

            let hist = past.rowData;
            let startLL = hist.start_lat_lng.split(',');
            let endLL = hist.end_lat_lng.split(',');
            sf.startLatLng = { lat: startLL[1], lng: startLL[0] };
            sf.endLatLng = { lat: endLL[1], lng: endLL[0] };
            sf.cores_required = hist.cores_required;
            sf.FeasibilityDistances.totalDistance = past.cableLengths.totalLength;
            sf.FeasibilityDistances.insideDistance = past.cableLengths.insideLength;
            sf.FeasibilityDistances.outsideDistance = past.cableLengths.outsideLength;
            sf.FeasibilityDistances.lmc_A_Distance = past.cableLengths.lmc_A_length;
            sf.FeasibilityDistances.lmc_B_Distance = past.cableLengths.lmc_B_length;
            sf.core_level_result = hist.core_level_result ? hist.core_level_result : '';
            sf.feasibility_result = hist.feasibility_result ? hist.feasibility_result : '';
            sf.pastInsideCables = past.getInsideCableData();
            sf.percentInside = (past.cableLengths.insideLength * 100 / past.cableLengths.totalLength).toFixed(2)

            past.processPastKMLData();
            past.setPastBOMData();
        }
    }

    this.getOutsideTotals = function () {
        let totals = { length: 0, material_price: 0.0, service_price: 0.0, bom_material: 0.0, bom_service: 0.0, };
        if (past.feasDetailsData.length) {
            for (rec of past.feasDetailsData) {
                if (rec.cable_type == 'outside' || rec.cable_type == 'outside_start' || rec.cable_type == 'outside_end' || rec.cable_type == 'lmc_start' || rec.cable_type == 'lmc_end') {
                    totals.length += rec.cable_length;
                    totals.bom_material += rec.material_cost;
                    totals.bom_service += rec.service_cost;
                }
            }
        }

        if (totals.length) {
            totals.material_price = totals.bom_material / totals.length;
            totals.service_price = totals.bom_service / totals.length;
        }

        return totals;
    }

    this.setPastBOMData = function () {
        let totals = past.getOutsideTotals();
        sf.pastBOMData = [{
            cable_type_id: past.rowData.cable_type_id,
            cable_length: totals.length,
            cable_type: past.rowData.cable_type,
            material_unit_price: totals.material_price,
            service_unit_price: totals.service_price,
            total_material_cost: totals.bom_material,
            total_service_cost: totals.bom_service,
            isPastData: true
        }];
    }

    this.fillDetailsTab = function (data, history_id) {
        if (sf.pastFeasibilities.length && data.length) {
            past.fillAppData();
            var selectedHistory = $.grep(sf.pastFeasibilities, function (h) { return h.history_id == history_id; });

            if (selectedHistory.length) {
                let hist = selectedHistory[0];
                let totalLength = hist.outside_a_end + hist.outside_b_end + past.cableLengths.lmc_A_length + past.cableLengths.lmc_B_length + past.cableLengths.insideLength;
                totalLength = totalLength ? totalLength.toFixed(2) : 0;

                if (hist) {
                    $(past.DE.txtCustomerID).val(hist.customer_id);
                    $(past.DE.txtFeasibilityID).val(hist.feasibility_id);
                    $(past.DE.txtcustomerName).val(hist.customer_name);
                    $(past.DE.txtfeasibilityName).val(hist.feasibility_name);
                    $(past.DE.txtHistoryID).val(hist.history_display_id);
                    $(past.DE.txtStartName).val(hist.start_point_name);
                    $(past.DE.txtStartLocation).val(hist.start_lat_lng);
                    $(past.DE.txtEndName).val(hist.end_point_name);
                    $(past.DE.txtEndLocation).val(hist.end_lat_lng);
                    $(past.DE.txtInside_A_Length).val(hist.inside_a);
                    $(past.DE.txtInside_P_Length).val(hist.inside_p);
                    // $(past.DE.txtInsideLength).val(past.cableLengths.insideLength.toFixed(2));
                    $(past.DE.txtOutside_A_Length).val(hist.outside_a_end.toFixed(2));
                    $(past.DE.txtOutside_B_Length).val(hist.outside_b_end.toFixed(2));
                    $(past.DE.txtTotalLength).val(totalLength);

                    $(past.DE.txtLMC_A).val(past.cableLengths.lmc_A_length.toFixed(2));
                    $(past.DE.txtLMC_B).val(past.cableLengths.lmc_B_length.toFixed(2));
                    $(past.DE.txtRequiredCores).val(hist.cores_required);
                    $(past.DE.txtBufferRadA).val(hist.buffer_radius_a);
                    $(past.DE.txtBufferRadB).val(hist.buffer_radius_b);
                    $("#hdn_feasibility_id").val(hist.feasibility_id);

                }
                else {
                    alert('Error');
                }
            }
        }
    }

    this.displayPastfeas = function () {
        past.displayGeometries(past.feasDetailsData, past.rowData);
    }

    this.bindRadioEvent = function (evt) {
        // $(past.DE.tblBody).on('click', 'input[type="radio"]', function (evt) {
        var data = evt.defaultValue;// $(this).closest("tr").find("td:eq(0)").html();
        // var data = past.tablePastFeas.row($(this).parents('tr')).data();
        if (data.length) {
            // history id
            var history_id = data;

            $(past.DE.tabsDiv).tabs({ active: 1 });

            $.ajax({
                type: "POST",
                url: 'FeasibilityDetails/getDetails',
                data: JSON.stringify({ history_id: history_id }),
                dataType: 'json',
                contentType: "application/json",
                success: function (response) {
                    if (response.status == 'success') {
                        //  $($("#tabs ul li")[1]).attr("class", "ui-state-default ui-corner-top");
                        $($("#tabs ul li")[1]).attr("style", "display:block;");
                        $($("#tabs ul li")[1]).trigger("click");

                        if (response.data.length) {
                            past.feasDetailsData = response.data;
                            sf.pastFeasibilities = response.pastFeas;
                            past.rowData = data;
                            past.rowData = $.grep(sf.pastFeasibilities, function (h) { return h.history_id == history_id; });
                            if (past.rowData.length) {
                                past.rowData = past.rowData[0];
                                past.fillDetailsTab(response.data, history_id);
                            }
                        }
                    }
                    else {
                        alert('No Data Found');
                    }
                },
                error: function (error) {
                    alert("error");
                }
            });

        }
        //  });
    }

    this.loadTable = function (finalData) {
        if (past.tablePastFeas) {
            past.tablePastFeas.destroy();
        }

        past.tablePastFeas = $(past.DE.tblPastFeas).DataTable({
            data: finalData,
            order: [[3, "asc"], [9, "desc"]],
            columns: past.tableColumns,
            columnDefs: past.columnDefaults,
            searching: true,
            bLengthChange: false,
            pageLength: 10
        });

        $(past.DE.dataTable).wrap(past.wrapperDiv);

        past.bindRadioEvent();
    }

    this.loadTabs = function () {
        $(past.DE.tabsDiv).tabs({ active: 0 });
    }

    this.init = function () {
        past.loadTabs();
        //if (sf.pastFeasibilities.length) {
        //    var finalData = past.getFinalData(sf.pastFeasibilities);
        //    past.loadTable(finalData);
        //}

        past.bindEvents();
    }
    this.bindRadioEventFtth = function (evt) {
        // $(past.DE.tblBody).on('click', 'input[type="radio"]', function (evt) {
        var data = evt.defaultValue;// $(this).closest("tr").find("td:eq(0)").html();
        // var data = past.tablePastFeas.row($(this).parents('tr')).data();
        if (data.length) {
            // history id
            var history_id = data;

            $(past.DE.tabsDiv).tabs({ active: 1 });

            $.ajax({
                type: "POST",
                url: 'FeasibilityDetails/getDetailsFtth',
                data: JSON.stringify({ history_id: history_id }),
                dataType: 'json',
                contentType: "application/json",
                success: function (response) {
                    if (response.status == 'success') {
                        //  $($("#tabs ul li")[1]).attr("class", "ui-state-default ui-corner-top");
                        $($("#tabs ul li")[1]).attr("style", "display:block;");
                        $($("#tabs ul li")[1]).trigger("click");

                        if (response.data) {
                            past.feasDetailsData = response.data;
                            sf.pastFeasibilities = response.pastFeasFtth;
                            //past.rowData = data;
                            past.rowData = $.grep(sf.pastFeasibilities, function (h) { return h.history_id == history_id; });
                            if (past.rowData.length) {
                                past.rowData = past.rowData[0];
                                past.fillDetailsTabFtth(response.data, history_id);
                                past.processPastKMLDataFtth(response.data, history_id);
                            }
                        }
                    }
                    else {
                        alert('No Data Found');
                    }
                },
                error: function (error) {
                    alert("error");
                }
            });

        }
        //  });
    }
    this.getDescriptionHTMLFTTH = function (feasibility) {
        let title = feasibility .feasibility_name+ ' Network';
        let distance =feasibility.path_distance;

        let desc = '<![CDATA[<p><b>Distance:</b> ' + distance + '<br/>';
        desc += ('<b>Feasibility ID: </b>' + feasibility.feasibility_id);
        desc += ('<b>Feasibility Name: </b>' + feasibility.feasibility_name);
        desc += '</p>]]>';

        return desc;
    }

    this.processPastKMLDataFtth = function (data,id) {
        //let data = past.feasDetailsData;
        let tableData = data;
        let KMLData = [];

        if (tableData && data) {
            var startPoint = tableData.lat_lng;
            var endPoint = tableData.entity_loc;
            var ll1 = startPoint.trim().split(',');
            var ll2 = endPoint.trim().split(',');

            
                var geom = '';
                var geomArray = data.geometry.replace('LINESTRING(', '').replace(')', '').split(',');
                for(g of geomArray) {
                    var latLng = g.trim().split(' ');
                    geom += latLng[1] + ',' + latLng[0] + ',0 ';
                }

                if (geom.length) {
                    KMLData.push({
                        feasibility_id: data.feasibility_id,
                        feasibility_name: data.feasibility_name,
                        geometry: geom,
                        description: past.getDescriptionHTMLFTTH(data),
                        geom_type: 'LINE',
                        entity_id: data.entity_id,
                        path_distance: data.path_distance,
                        colorHex_8: sf.getKMLHexCode(sf.getColorByType())
                    });
                }
            

            if (ll1.length) {
                KMLData.push({
                    geometry: ll1[1] + ',' + ll1[0] + ',0 ',
                    description: '<![CDATA[<h2>(' + ll1[1] + ',' + ll1[0] + ')</h2>]]>',
                    geom_type: 'POINT',
                    feasibility_title: 'Customer Location'
                   // entity_title: (tableData.start_point_name != null && tableData.start_point_name.length > 0) ? tableData.start_point_name : 'Start Point'
                });
            }
            if (ll2.length) {
                KMLData.push({
                    geometry: ll2[1] + ',' + ll2[0] + ',0 ',
                    description: '<![CDATA[<h2>(' + ll2[1] + ',' + ll2[0] + ')</h2>]]>',
                    geom_type: 'POINT',
                    feasibility_title: 'Entity Location'
                   // entity_title: (tableData.end_point_name != null && tableData.end_point_name.length > 0) ? tableData.end_point_name : 'End Point'
                });
            }
        }
        sf.pastKMLDataFtth = KMLData;
    }

    this.fillDetailsTabFtth = function (data, history_id) {
        if (sf.pastFeasibilities.length) {
            // past.fillAppData();
            var selectedHistory = $.grep(sf.pastFeasibilities, function (h) { return h.history_id == history_id; });

            if (selectedHistory.length) {
                let hist = selectedHistory[0];


                if (hist) {
                    hist.buffer_radius = hist.buffer_radius + " m";
                    $("#_customerID").val(hist.customer_id);
                    $("#_customerName").val(hist.customer_name);
                    $("#_feasibilityName").val(hist.feasibility_name);
                    $("#_feasibilityID").val(hist.feasibility_id);
                    $("#_Location").val(hist.lat_lng);
                    $("#_entityId").val(hist.entity_id);
                    $("#_pathDistance").val(hist.path_distance);
                    $("#_bufferRadius").val(hist.buffer_radius);
                    $("#hdn_history_id").val(hist.history_id);

                }
                else {
                    alert('Error');
                }
            }
        }
    }
    this.displayPastfeasFtth = function () {
        past.displayGeometriesFtth(past.feasDetailsData, past.rowData);
    }

    this.displayGeometriesFtth = function (data, tableData) {
        if (tableData) {
            var startPoint = tableData.lat_lng;
            var endPoint = data.entity_loc;

            sf.resetFields();



            var path = [];
            var geom = data.geometry.replace('LINESTRING(', '').replace(')', '').split(',');
            var system_id = data.entity_id;
            for(g of geom) {
                var latLng = g.trim().split(' ');
                path.push(new google.maps.LatLng(latLng[0], latLng[1]));
            }

            //sf.savedPaths.push(new google.maps.Polyline({
            //    path: path,
            //    length: google.maps.geometry.spherical.computeLength(path),
            //    strokeColor: "#EF4B41",
            //    strokeOpacity: 1.0,
            //    strokeWeight: 3,
            //    zIndex: 10,
            //    system_id: data.entity_id,
            //    // network_status: item.network_status,
            //    // cable_type: item.cable_type,
            //    cable_length: data.path_distance
            //}));


            // end point markers
            if (startPoint) {
                var ll1 = startPoint.trim().split(',');
                var ll2 = endPoint.trim().split(',');
                var startLoc = new google.maps.LatLng(ll1[0], ll1[1]);
                var endLoc = new google.maps.LatLng(ll2[0], ll2[1]);
                sf.iRadiusInKiloMeter = data.buffer_radius / 1000;

                sf.GeoCodeIt(startPoint);
                var ctrl = '<input type="radio"  value="' + data.entity_loc + '"data-id="' + data.entity_id + '"  />';
                getDirection(ctrl,true);
                sf.populateInputFieldsFtth({
                    lat: ll1[0],
                    lng: ll1[1],
                    buffer_radius: data.buffer_radius,
                });
                sf.updateBufferRadius(sf.DE.end_type.ftth, data.buffer_radius)
            }
        }
    }
    $(past.DE.btnComputeAgainFtth).on("click", function () {
        // btnClickedEvent = "compute";
        var startLL = past.feasDetailsData.lat_lng.split(',');
        // var endLL = past.rowData.end_lat_lng.split(',');
        sf.clearMap();

        sf.populateInputFieldsFtth({
            lat: startLL[0],
            lng: startLL[1],
            buffer_radius: past.feasDetailsData.buffer_radius,
        });
        sf.iRadiusInKiloMeter = past.feasDetailsData.buffer_radius / 1000;
        sf.GeoCodeIt(past.feasDetailsData.lat_lng);
        var ctrl = '<input type="radio"  value="' + past.feasDetailsData.entity_loc + '"data-id="' + past.feasDetailsData.entity_id + '"  />';

        getDirection(ctrl,false);
       // sf.DisableInputfieldsFtth();

    });
}