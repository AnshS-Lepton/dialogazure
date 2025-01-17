var FiberLink = function () {
    var app = this;
    //var infowindow = new google.maps.InfoWindow();
    this.CableFiberButton = false;
    this.StatusMessages = {
        CONFIRM_DELETE_LINK: MultilingualKey.SI_GBL_GBL_JQ_FRM_061,
        CONFIRM_EDIT_LINK: MultilingualKey.SI_GBL_GBL_JQ_FRM_062,
        SELECT_FROM_DATE: MultilingualKey.SI_OSP_GBL_JQ_RPT_010,
        SELECT_TO_DATE: MultilingualKey.SI_OSP_GBL_JQ_RPT_011,
        RESET_CREATE_LINK: MultilingualKey.SI_GBL_GBL_JQ_FRM_063,
        LINK_ASSOCIATED_SUCCESS: MultilingualKey.SI_GBL_GBL_JQ_FRM_064,
        NO_MATCH_FOUND: MultilingualKey.SI_GBL_GBL_JQ_GBL_001,
        Fiber_Link_Status: MultilingualKey.SI_OSP_GBL_GBL_GBL_148,
    }
    this.DE = {
        'chosen_select': '.chosen-select',
        'option_selected': 'option:selected',
        'btnExportLinks': '#btnExportLinks',
        'btnExportFiberLinkToKML': '#btnExportFiberLinkToKML',
        'btnResetSearchFilter': '#btnResetSearchFilter',
        'customedate': '#customedate',
        'txtDateFrom': '#txtDateFrom',
        'txtDateTo': '#txtDateTo',
        'txtSearchTxt': '#txtSearchTxt',
        'ddlSearchBy': '#ddlSearchBy',
        'btnLinkData': '#btnLinkData',
        'tblFiberLinkGrid': '#tblFiberLinkGrid',
        'ViewLink': '#ViewLink',
        'imgFromDate': '#imgFromDate',
        'imgToDate': '#imgToDate',
        'customedate_chosen': '#customedate_chosen',
        'btnclear': '#btnclear',
        'ddlStartPointtype': '#ddlStartPointtype',
        'ddlLinkPrifixType': '#ddlLinkPrifixType',
        'txtStartPointNwkID': '#txtStartPointNwkID',
        'ddlEndPointtype': '#ddlEndPointtype',
        'txtEndPointNwkID': '#txtEndPointNwkID',
        'txtHandoverDate': '#txtHandoverDate',
        'imgHandoverDate': '#imgHandoverDate',
        'txtHotoDate': '#txtHotoDate',
        'imgHoToDate': '#imgHoToDate',
        'crossStartIcon': '.crossStartIcon',
        'tickStartIcon': '.tickStartIcon',
        'crossEndIcon': '.crossEndIcon',
        'tickEndIcon': '.tickEndIcon',
        'txtROWAuthority': '#txtROWAuthority',
        'txtROWSegments': '#txtROWSegments',
        'txtROWLength': '#txtROWLength',
        'txtTotalRecurringCharges': '#txtTotalRecurringCharges',
        'crossIcon': '.crossIcon',
        'tickIcon': '.tickIcon',
        'txtLinkId': '#txtLinkId',
        'crossStartIconId': '#crossStartIcon',
        'IconCross': '#IconCross',
        'crossEndIconId': '#crossEndIcon',
        'frmAddFiberLink': '#frmAddFiberLink',
        'CreateFiberLink': '#CreateFiberLink',
        'frmViewLink': '#frmViewLink',
        'hdnFiberLinkSystemId': '#hdnFiberLinkSystemId',
        'CreateFiberLinkForm': "frmAddFiberLink",
        'dvAssociateLink': '#dvAssociateLink',
        'hdnModelLinkSystemId': '#hdnModelLinkSystemId',
        'hdnLinkSystemId': '#hdnLinkSystemId',
        'lblGrdLinkId_': '#lblGrdLinkId_',
        'hdnCableId': '#hdnCableId',
        'hdnFiberNumber': '#hdnFiberNumber',
        'txtCableFiberLinkId': '#txtCableFiberLinkId',
        'txtFiberLinkId': '#txtFiberLinkId',
        'ddlLinkType': '#ddlLinkType_chosen a',
        'ddlMainlinktype': '#ddlMainlinktype_chosen a',
        'txtmainlinkid': '#txtmainlinkid',
        'ddlredundantlinktype': '#ddlredundantlinktype_chosen a',
        'txtredundantlinkid': '#txtredundantlinkid',
        '': '#',
        'btnExportPDFLinks': '#btnExportPDFLinks',
    }
    this.InitApp = function () {
        // app.setDateTimeCalendar_viewLink("txtDateFrom", "txtDateTo", "imgFromDate", "imgToDate", false);
        if (($(app.DE.ddlSearchBy).val() != 'Select') && ($(app.DE.ddlSearchBy).val() != "")) {
            $(app.DE.txtSearchTxt).prop('readonly', false);
        }
        addDefaultSortingIcons("tblFiberLinkGrid");
        //onGridHeaderClick("tblFiberLinkGrid");
        app.GridColor();
        app.bindViewLinkEvents();
        app.bindCreateLinkEvents();
        app.bindAssociateLinkEvents();
    }

    this.bindViewLinkEvents = function () {
        $(app.DE.chosen_select).chosen({ width: "100%" });
        $(app.DE.btnExportLinks).on("click", function () {
            window.location = appRoot + 'FiberLink/ExportFiberLink';
            $(".dropbox_BulkExport").slideToggle();
        });
        $(app.DE.btnExportFiberLinkToKML).on("click", function () {
            var FiberLinkBulkKMLExportLimit = $('#hdnFiberLinkBulkKMLExportLimit').val()
            if (parseInt($('#hdnAssociatedCount').val()) > parseInt($('#hdnFiberLinkBulkKMLExportLimit').val())) {
                alert(MultilingualKey.SI_OSP_STR_JQ_FRM_002 + " " + FiberLinkBulkKMLExportLimit + " " + MultilingualKey.SI_OSP_GBL_NET_RPT_297);
            }
            else { window.location = appRoot + 'FiberLink/DownloadFiberLinkIntoKML'; $(".dropbox_BulkExport").slideToggle(); }
        });
        $(app.DE.btnResetSearchFilter).on("click", function () {
            $(app.DE.customedate).val(0).trigger("chosen:updated");
            $(app.DE.txtDateFrom).val('');
            $(app.DE.txtDateTo).val('');
            $(app.DE.txtSearchTxt).val('');
            $(app.DE.ddlSearchBy).val('');
        });
        if ($(app.DE.customedate_chosen).children('a').text().trim() != "Custom") {

            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }
        $(app.DE.btnLinkData).on("click", function () {
            if (($(app.DE.ddlSearchBy).val() == 'Select') && ($(app.DE.txtSearchTxt).val() == "")) {
                if (($(app.DE.customedate + ' option:selected').text() != "Select") && ($(app.DE.txtDateFrom).val() == "")) {
                    alert(app.StatusMessages.SELECT_FROM_DATE);
                    return false;
                }

                else if (($(app.DE.customedate + ' option:selected').text() != "Select") && ($(app.DE.txtDateTo).val() == "")) {
                    alert(app.StatusMessages.SELECT_TO_DATE);
                    return false;
                }

                else if ($(app.DE.txtDateFrom).val() != "" && $(app.DE.txtDateTo).val() != "") {
                    var startDate = new Date($(app.DE.txtDateFrom).val());
                    var endDate = new Date($(app.DE.txtDateTo).val());
                    var configuredDay = 1825;
                    return app.GetDaysDifference(startDate, endDate, configuredDay);
                }
                else {
                    return true;
                }
            }
            else {
                return true;
            }
        });
        $(app.DE.btnExportPDFLinks).on("click", function () {
            window.location = appRoot + 'FiberLink/ExportFiberLinkInPDF';
            $(".dropbox_BulkExport").slideToggle();
        });

    }
    this.GetDaysDifference = function (firstDate, secondDate, configuredDay) {

        var oneDay = 24 * 60 * 60 * 1000;
        var diff = 0;
        diff = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));


        if (diff > configuredDay) {
            alert(MultilingualKey.SI_OSP_GBL_JQ_RPT_015)
            return false;

        }
    }
    this.bindCreateLinkEvents = function () {
        // callFormValidator(app.DE.CreateFiberLinkForm);
        ///CREATE LINK PAGE 
        $(app.DE.chosen_select).chosen({ width: "100%" });

        if ($(app.DE.hdnFiberLinkSystemId).val() > 0) {
            $(app.DE.tickIcon).css('display', 'none');
            $(app.DE.crossIcon).css('display', 'none');
            $(app.DE.tickStartIcon).css('display', 'none');
            $(app.DE.crossStartIcon).css('display', 'none');
            $(app.DE.tickEndIcon).css('display', 'none');
            $(app.DE.crossEndIcon).css('display', 'none');
        }
        // START END ENTITY TYPE NETWORK ID AUTOCOMPLETE---START
        $(app.DE.txtEndPointNwkID).autocomplete({
            source: function (request, response) {
                var _searchtext = $(app.DE.ddlEndPointtype).val() + ':' + request.term;
                var res = ajaxReq('FiberLink/GetStartEndPointNetworkId', { SearchText: _searchtext }, true, function (data) {
                    if (data.geonames.length == 0) {
                        var result = [
                            {
                                //label: 'No match Found'
                                label: app.StatusMessages.NO_MATCH_FOUND
                            }
                        ];
                        response(result);
                    }
                    else {
                        response($.map(data.geonames, function (item) {
                            return {
                                label: item.label, value: item.label, geomType: item.geomType, entityName: item.entityName, entityTitle: item.entityTitle, systemID: item.systemId, status: item.status, entityId: item.entityID
                            }//
                        }))
                    }
                }, true, false);

            },
            minLength: 3,
            select: function (event, ui) {
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                }
                else {
                    event.preventDefault();
                    app.gtype = ui.item.geomType;
                    if (ui.item.entityName != null) {
                        $(app.DE.txtEndPointNwkID).val(ui.item.label);
                        //app.ShowEntityOnMap(ui.item.systemID, ui.item.entityName, ui.item.geomType);
                    }
                    else {
                        $(app.DE.txtEndPointNwkID).val(ui.item.label + ':');
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
        $(app.DE.txtStartPointNwkID).autocomplete({
            source: function (request, response) {
                //bindstatpointautocompleteeevnt()
                var _searchtext = $(app.DE.ddlStartPointtype).val() + ':' + request.term;
                var res = ajaxReq('FiberLink/GetStartEndPointNetworkId', { SearchText: _searchtext }, true, function (data) {
                    if (data.geonames.length == 0) {
                        var result = [
                            {
                                //label: 'No match Found'
                                label: app.StatusMessages.NO_MATCH_FOUND
                            }
                        ];
                        response(result);
                    }
                    else {
                        response($.map(data.geonames, function (item) {
                            return {
                                label: item.label, value: item.label, geomType: item.geomType, entityName: item.entityName, entityTitle: item.entityTitle, systemID: item.systemId, status: item.status, entityId: item.entityID
                            }//
                        }))
                    }
                }, true, false);
            },
            minLength: 3,
            select: function (event, ui) {
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                }
                else {
                    event.preventDefault();
                    app.gtype = ui.item.geomType;
                    if (ui.item.entityName != null) {
                        $(app.DE.txtStartPointNwkID).val(ui.item.label);
                    }
                    else {
                        $(app.DE.txtStartPointNwkID).val(ui.item.label + ':');
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
        // START END ENTITY TYPE NETWORK ID AUTOCOMPLETE---END
        $("input.numbers-only").on("paste", function () {
            pasteNumberOnly(this);
        });


        $('.remove-white-space').on("paste", function () {
            removeWhiteSpace(this);
        });

        if ($(app.DE.txtHandoverDate).val() != undefined && $(app.DE.imgHandoverDate).val() != undefined) {
            app.setDateTimeCalendar('txtHandoverDate', 'imgHandoverDate', '', false);
        }
        if ($(app.DE.txtHotoDate).val() != undefined && $(app.DE.imgHoToDate).val() != undefined) {
            app.setDateTimeCalendar('txtHotoDate', 'imgHoToDate', '', false);
        }

    }
    this.bindAssociateLinkEvents = function () {

        $(app.DE.chosen_select).chosen({ width: '164%' });
        // $(app.DE.dvAssociateLink).draggable({ containment: "parent" });
        if ($(app.DE.hdnModelLinkSystemId).val() > 0) {
            $(app.DE.tickIcon).css('display', 'block');
        }
        $(app.DE.hdnLinkSystemId).val($(app.DE.hdnModelLinkSystemId).val());
        $(app.DE.txtFiberLinkId).autocomplete({
            source: function (request, response) {
                var res = ajaxReq('main/GetAutoFiberLinkId', { SearchText: request.term }, true, function (data) {
                    if (data.geonames.length == 0) {
                        $("#btncreate").prop("disabled", false);
                        var result = [
                            {
                                //label: 'No match Found'
                                label: app.StatusMessages.NO_MATCH_FOUND
                            }
                        ];
                        response(result);
                    }
                    else {
                        $("#btncreate").prop("disabled", true);
                        response($.map(data.geonames, function (item) {
                            return {
                                label: item.link_id, value: item.link_id
                            }
                        }))
                    }
                }, true, false);

            },
            minLength: 2,
            select: function (event, ui) {
                if (ui.item.label == 'No match Found') {
                    $("#btncreate").prop("disabled", false);
                    event.preventDefault();
                }
                else {
                    $("#btncreate").prop("disabled", true);
                    if (ui.item.entityName != null) {
                        $(app.DE.txtCableFiberLinkId).val(ui.item.value + ':' + ui.item.label);
                    }
                    else {
                        $(app.DE.txtCableFiberLinkId).val(ui.item.label + ':');
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    }
    this.GridColor = function () {
        app.ActionColumn();
        $(app.DE.tblFiberLinkGrid + ' tbody tr').each(function (e) {
            var colIndex = $(app.DE.tblFiberLinkGrid + ' thead tr th[data-column-name= "' + MultilingualKey.SI_OSP_GBL_GBL_GBL_148 + '"]').index();
            var fiber_link_status = $($(this).children('td')[colIndex]).text().trim();
            if (fiber_link_status == "Free") {
                $(this).css('background-color', 'rgb(176 184 253 / 40%)');
            }
            else if (fiber_link_status == "Associated") {
                $(this).css('background-color', 'rgba(255 237 190 / 60%)');
            }
            else if (fiber_link_status == "Customer Associated") {
                $(this).css('background-color', 'rgba(77, 167, 115, 0.6)');
            }
        });
    }
    this.ActionColumn = function () {
        $(app.DE.tblFiberLinkGrid + ' thead tr th a').each(function () {
            var html = $(this).html();
            $(this).parent().attr('data-column-name', html);

        })
        $(app.DE.tblFiberLinkGrid + ' tbody tr td').each(function () {
            var html = $(this).html();
            $(this).attr('data-value', html);
        })

        $(app.DE.tblFiberLinkGrid + ' tbody tr').each(function (index, value) {

            var colIndex = $(app.DE.tblFiberLinkGrid + ' thead tr th[data-column-name="' + MultilingualKey.SI_OSP_GBL_GBL_GBL_148 + '"]').index();

            var fiber_link_status = $($(this).children('td')[colIndex]).text().trim();

            $(app.DE.tblFiberLinkGrid + ' thead tr th:eq(0)').html(MultilingualKey.SI_OSP_GBL_GBL_GBL_059);
            var Excel = "'EXCEL'";
            var Kml = "'KML'";
            var systemId = $(app.DE.tblFiberLinkGrid + ' tbody tr:eq(' + index + ') td:eq(0)').attr('data-value');
            var rowAction = ' <a href="#" id="lnkdownloadLink' + index + '" style="padding:0;" class="default dropfiles"  onclick="fiberLink.FiberLinkDrp(' + index + ')">Download<i class="fa fa-chevron-down ml-03" onclick="fiberLink.FiberLinkDrp(' + index + ')"></i></a>';
            rowAction = rowAction + '<div class="dropbox" id="dropbox' + index + '"  style="margin-right:787px"> <span onclick="fiberLink.ExportFiberLinkDetail(' + Excel + ',' + systemId + ',' + index + ')"><b>Excel</b></span>'
                + '<span  style="display:' + (fiber_link_status == "Free" ? "none" : "") + '" onclick="fiberLink.ExportFiberLinkIntoKML(' + Kml + ',' + systemId + ',' + index + ')"><b>Kml</b></span>  </div>';
                //Fiber link Deletion disabled due to its occupation to another entity
            rowAction = rowAction + '<span title="' + (fiber_link_status == "Free" ? MultilingualKey.SI_GBL_GBL_GBL_GBL_002 : MultilingualKey.SI_OSP_GBL_NET_RPT_419) + '"><i class="cptr icon-Delete ml-05' + (fiber_link_status == "Free" ? "" : " dvdisabled") + '" onclick="fiberLink.deleteFiberLinkById(' + systemId + ')"></i></span>';
            rowAction = rowAction + '<i class="cptr icon-map-view ' + (fiber_link_status == "Free" ? "dvdisabled" : "") + '" id="iconShowlinkOnMapp" title="' + MultilingualKey.SI_OSP_GBL_GBL_GBL_036 + '" style="padding-left: 7px " onclick="splicing.showFiberLinkOnMap(' + systemId + ')" ></i></a>';
            rowAction = rowAction + '<a href="#" data-value="' + systemId + '"  class="cptr fa  fa-edit" id="iconViewDetails" title="' + MultilingualKey.SI_GBL_GBL_GBL_GBL_003 + '" onclick="fiberLink.editFiberLinkById(' + systemId + ')" style="padding-left: 7px;"></a>';
            rowAction = rowAction + '<i class="cptr icon-CUSTOMER ' + (fiber_link_status == "Free" ? "dvdisabled" : "") + '" id="iconAssociateCustomer" onclick="fiberLink.GetFiberLinkCustomer(' + systemId + ')" title="' + MultilingualKey.SI_OSP_GBL_NET_FRM_430 + '" style="padding-left: 7px;"></i>';
            rowAction = rowAction + '<i class="cptr fa fa-history m-r-xs" title="' + MultilingualKey.SF_GBL_GBL_JQ_HIS_001 + '" onclick="fiberLink.GetFiberLinkHistory(' + systemId + ')" style="color: #1b9461; font-size: 14px; padding-left: 7px"></i>';
            $(app.DE.tblFiberLinkGrid + ' tbody tr:eq(' + index + ') td:eq(0)').html(rowAction);
        });
    }
    this.setDateTimeCalendar_viewLink = function (startdateid, enddateid, startdateimgid, enddateimgid, isFutureDateAllowed) {

        Calendar.setup({
            inputField: startdateid,   // id of the input field
            button: startdateimgid,
            ifFormat: "%d-%b-%Y",       // format of the input field datetime format %d-%b-%Y %I:%M %p
            showsTime: false,
            timeFormat: "12",
            weekNumbers: false,


            disableFunc: function (date) {
                var endDateValue = document.getElementById(enddateid).value;

                var enddt = DateFormatter(endDateValue);
                enddt = new Date(enddt);
                if (date.getTime() > enddt.getTime()) {
                    return true;
                }
                ////
                if (!isFutureDateAllowed) {
                    var now = new Date();
                    now.setDate(now.getDate());
                    if (date.getTime() > now.getTime()) {
                        return true;
                    }
                }
            }
        });

        //isFutureDateAllowed = true;
        Calendar.setup({
            inputField: enddateid,
            button: enddateimgid,
            ifFormat: "%d-%b-%Y",
            showsTime: false,
            timeFormat: "12",
            weekNumbers: false,
            //maxDate: new Date(new Date().setMonth(new Date().getMonth() + 6)),
            align: "Br",
            //maxDate: "+6m",
            disableFunc: function (date) {
                var startDateValue = document.getElementById(startdateid).value;

                var startdt = DateFormatter(startDateValue);
                startdt = new Date(startdt);
                if (date.getTime() < startdt.getTime()) {
                    return true;
                }
                if (!isFutureDateAllowed) {
                    var now = new Date();
                    now.setDate(now.getDate());
                    if (date.getTime() > now.getTime()) {
                        return true;
                    }
                }

            },

        });
    }
    this.setDateTimeCalendar = function (startdateid, startdateimgid, chkDisabled, isFutureDateAllowed) {
        Calendar.setup({
            inputField: startdateid,   // id of the input field
            button: startdateimgid,
            ifFormat: "%d-%b-%Y",       // format of the input field datetime format %d-%b-%Y %I:%M %p
            showsTime: false,
            timeFormat: "12",
            weekNumbers: false,
            onUpdate: function () {
                var emID = $("#" + startdateid).val();
                if (emID != "") {
                    $("#" + startdateid).removeClass('input-validation-error').removeClass('field-validation-error').addClass('field-validation-valid').html('');
                }
                else {
                    $("#" + startdateid).addClass('input-validation-error');
                }
            }
        });
    }
    this.enableSearchtext = function () {
        if (($(app.DE.ddlSearchBy).val() != 'Select') && ($(app.DE.ddlSearchBy).val() != "")) {
            $(app.DE.txtSearchTxt).prop('readonly', false);
        }
        else {
            $(app.DE.txtSearchTxt).prop('readonly', true);
            $(app.DE.txtSearchTxt).val('');
        }
    }

    this.FiberLinkDrp = function (id) {
        $(".dropbox:not(#dropbox" + id + ")").fadeOut();
        $(".dropfiles:not(#lnkdownloadLink" + id + ")").find('i.fa').removeClass('fa-chevron-up').addClass('fa-chevron-down');
    }
    this.deleteFiberLinkById = function (system_id) {
        showConfirm(app.StatusMessages.CONFIRM_DELETE_LINK, function () {
            ajaxReq('FiberLink/deleteFiberLinkById', { system_id: system_id, }, true, function (resp) {

                if (resp.status == "OK") {
                    alert(resp.message);
                    $(app.DE.frmViewLink).submit();
                }
                else {
                    alert(resp.message);
                }
            }, false, false)
        });
    }


    this.onChangeCustomDate = function () {
        var value = $(app.DE.customedate + ' option:selected').val();

        app.SetFromAndToDate(value);
    }
    this.SetFromAndToDate = function (value) {
        var endDate = new Date();
        var startDate = new Date();
        var startDateDiff = 0;
        var endDateDiff = 0;

        if (value == 1) // custom
        {
            $(app.DE.txtDateFrom).val("");
            $(app.DE.txtDateTo).val("");
            $(app.DE.imgFromDate).show();
            $(app.DE.imgToDate).show();
            return;
        }
        else if (value == 2) { //Today
            startDateDiff = 0;
            endDateDiff = 0;
            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }
        else if (value == 3) { //previous day
            startDateDiff = -1;
            endDateDiff = -1;
            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }
        else if (value == 4) {//last 7 days
            startDateDiff = -6;
            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }
        else if (value == 5) {//last 30 days
            startDateDiff = -29;
            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }
        else if (value == 6) {//last 3 Months
            startDateDiff = -89;
            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }
        else if (value == 7) {//last 6 Months
            startDateDiff = -179;
            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }
        else if (value == 8) {//last 6 Months
            startDateDiff = -364;
            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }

        startDate.setDate(startDate.getDate() + startDateDiff);
        endDate.setDate(endDate.getDate() + endDateDiff);

        var newStartDate = startDate.toDateString();
        newStartDate = new Date(Date.parse(newStartDate));

        var newEndDate = endDate.toDateString();
        newEndDate = new Date(Date.parse(newEndDate));

        $(app.DE.txtDateFrom).val(fncCurrentDate(newStartDate));
        $(app.DE.txtDateTo).val(fncCurrentDate(newEndDate));
        if (value == 0) {
            $(app.DE.txtDateFrom).val('');
            $(app.DE.txtDateTo).val('');
            $(app.DE.imgFromDate).hide();
            $(app.DE.imgToDate).hide();
        }
    }


    this.editFiberLinkById = function (system_id) {
        showConfirm(app.StatusMessages.CONFIRM_EDIT_LINK, function () {
            ajaxReq('FiberLink/AddFiberLink', { system_id: system_id }, true, function (resp) {
                $(app.DE.CreateFiberLink).html(resp);
                $("#liAddFiberLink a").trigger("click");
                //Create Link Page
                app.GetStartPointNetworkId();
                app.GetEndPointNetworkId();
                app.bindCreateLinkEvents();
            }, false, true);
        });
    }




    this.ExportFiberLinkDetail = function (reportType, systemId, index) {
        app.FiberLinkDrp(index);
        window.location = appRoot + 'FiberLink/ExportFiberLinkById?SystemId=' + parseInt(systemId) + '&ReportType=' + reportType + '';

    }

    this.attachment = {
        upload: function (systemId, parentEntityType, parentSystemId) {

            if ($('#FiberLinkDocUpload').val() == '') {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_096);
            }
            else if ($('#FiberLinkDocUpload').val().length > 60) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_062);
            }
            else {
                app.attachment.uploadDocumentFile(systemId, parentEntityType, parentSystemId);
            }
        },
        uploadDocumentFile: function (systemId, parentEntityType, parentSystemId) {

            var frmData = new FormData();
            var filesize = $('#hdnMaxFileUploadSizeLimit').val();
            var uploadedfile = $('#FiberLinkDocUpload').get(0).files[0];
            var Sizeinbytes = filesize * 1024;
            if (!app.attachment.validatefilename(uploadedfile.name)) { return false; }
            else if ($('#ulDocumentUpload  tbody tr[data-image-name="' + uploadedfile.name + '"]').length > 0) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_055);
                return false;
            }
            else if ($('#FiberLinkDocUpload').get(0).files[0].size > Sizeinbytes) {

                alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_FRM_109, (filesize / 1024).toFixed(2))));
            }
            else {

                if (!app.attachment.validateDocumentFileType()) { return false; }
                frmData.append(uploadedfile.name, uploadedfile);
                frmData.append('system_Id', systemId);
                frmData.append('entity_type', parentEntityType);
                frmData.append('feature_name', 'Fiberlink');

                ajaxReqforFileUpload('Main/UploadDocument', frmData, true, function (resp) {
                    if (resp.status == "OK") {
                        app.attachment.getAttachmentFiles(systemId, parentEntityType, parentSystemId);
                        alert(resp.message, 'success', 'success');
                        if ($('#FiberLinkDocUpload')[0] != undefined)
                            $('#FiberLinkDocUpload')[0].value = '';
                    }
                    else {
                        alert(resp.message, 'warning');
                    }

                }, true);
            }
        },
        getAttachmentFiles: function (systemId, parentEntityType, parentSystemId) {
            ajaxReq('Library/GetFiberLinkAttachment', { system_Id: systemId, entity_type: parentEntityType, featureName: 'Fiberlink' }, true, function (jResp) {
                $('#divFiberLinkAttachment').html(jResp);
            }, false, false, true);
        },
        validateDocumentFileType: function () {
            var validFilesTypes = ["dwg", "pdf", "jpeg", "jpg", "doc", "docx", "xls", "xlsx", "csv", "vsd", "ppt", "pptx", "png"];
            var file = $('#FiberLinkDocUpload').val();
            var filepath = file;
            return app.attachment.ValidateFileType(validFilesTypes, filepath);
        },
        ValidateFileType: function (validFilesTypes, filepath) {
            var ext = filepath.substring(filepath.lastIndexOf(".") + 1, filepath.length).toLowerCase();
            var isValidFile = false;
            for (var i = 0; i < validFilesTypes.length; i++) {
                if (ext == validFilesTypes[i]) {
                    isValidFile = true;
                    break;
                }
            }
            if (!isValidFile) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_059 +
                    " extension:\n\n" + validFilesTypes.join(", "), 'warning');
            }
            return isValidFile;
        },
        deleteFile: function (_systemId, _parentEntityId, _parentEntityType) {
            var ListSystemIds = [];
            ListSystemIds.push(_systemId)
            if (ListSystemIds.length > 0) {
                var func = function () {
                    ajaxReq('Main/DeleteAttachmentFile', { system_Id: _systemId }, true, function (j) {

                        ajaxReq('Library/GetFiberLinkAttachment', { system_Id: _parentEntityId, entity_type: "", featureName: 'Fiberlink' }, true, function (jResp) {
                            $('#divFiberLinkAttachment').html(jResp);
                        }, false, false, true);
                        alert(j.message, 'success', 'success');
                        $('#liDocumentUpload_' + _systemId).remove();

                    }, true, true)
                };
                showConfirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_004, func);
            } else {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_005, 'warning');
            }
        },
        downLoadFile: function (_parentEntityId, _parentEntityType) {
            var listPathName = [];
            var _entity_type = $('#hdEntityType').val();

            window.location = appRoot + 'Main/DownloadAll?system_Id=' + _parentEntityId + '&entity_type=' + _parentEntityType + '&entityFeature=' + 'Fiberlink';

        },
        validatefilename: function (filename) {
            var isValid = true;
            if (filename.split('.').length > 2) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_056);
                isValid = false;
            } else if (filename.split('..').length > 1) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_114);
                isValid = false;
            }
            //var regex = /^[0-9a-zA-Z_\.\s]*$/;
            //if (!regex.test(filename)) { isValid = false; alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_063); }
            return isValid;
        },
        clear: function () {
            $("#FiberLinkDocUpload").val('');
        }
    }

    this.setDateTimeCalendar = function (startdateid, startdateimgid, chkDisabled, isFutureDateAllowed) {
        Calendar.setup({
            inputField: startdateid,   // id of the input field
            button: startdateimgid,
            ifFormat: "%d-%b-%Y",       // format of the input field datetime format %d-%b-%Y %I:%M %p
            showsTime: false,
            timeFormat: "12",
            weekNumbers: false,
            onUpdate: function () {
                var emID = $("#" + startdateid).val();
                if (emID != "") {
                    $("#" + startdateid).removeClass('input-validation-error').removeClass('field-validation-error').addClass('field-validation-valid').html('');
                }
                else {
                    $("#" + startdateid).addClass('input-validation-error');
                }
            }
        });
    }

    this.GetStartPointNetworkId = function () {
        if ($(app.DE.ddlStartPointtype).val()) {
            $(app.DE.txtStartPointNwkID).prop("disabled", false);
        }
    }
    this.GetEndPointNetworkId = function () {
        if ($(app.DE.ddlEndPointtype).val()) {
            $(app.DE.txtEndPointNwkID).prop("disabled", false);
        }
    }

    this.resetCreateLink = function () {
        showConfirm(app.StatusMessages.RESET_CREATE_LINK, function () {
            ajaxReq('FiberLink/AddFiberLink', { system_id: 0 }, false, function (resp) {
                $(app.DE.CreateFiberLink).html(resp);
                app.bindCreateLinkEvents();
            }, false, true);
        });
    }


    this.GetStartPoint = function () {
        if ($(app.DE.ddlStartPointtype).val()) {
            $(app.DE.txtStartPointNwkID).val('');
            $(app.DE.crossStartIcon).css('display', 'none');
            $(app.DE.tickStartIcon).css('display', 'none');
            $(app.DE.txtStartPointNwkID).prop("disabled", false);
        }
        else {
            $(app.DE.txtStartPointNwkID).val('');
            $(app.DE.crossStartIcon).css('display', 'none');
            $(app.DE.tickStartIcon).css('display', 'none');
            $(app.DE.txtStartPointNwkID).prop("disabled", true);
            $(app.DE.txtStartPointNwkID).css('border-color', '');
        }
    }

    this.GetEndPoint = function () {
        if ($(app.DE.ddlEndPointtype).val()) {
            $(app.DE.txtEndPointNwkID).val('');
            $(app.DE.crossEndIcon).css('display', 'none');
            $(app.DE.tickEndIcon).css('display', 'none');
            $(app.DE.txtEndPointNwkID).prop("disabled", false);
        }
        else {
            $(app.DE.crossEndIcon).css('display', 'none');
            $(app.DE.tickEndIcon).css('display', 'none');
            $(app.DE.txtEndPointNwkID).val('');
            $(app.DE.txtEndPointNwkID).prop("disabled", true);
            $(app.DE.txtEndPointNwkID).css('border-color', '');
        }
    }

    this.enableDisableBasedOnAnyRowPortion = function (_value) {
        if (_value) {
            $(app.DE.txtROWAuthority).prop("readonly", false);
            $(app.DE.txtROWSegments).prop("readonly", false);
            $(app.DE.txtROWLength).prop("readonly", false);
            $(app.DE.txtTotalRecurringCharges).prop("readonly", false);
        }
        else {
            $(app.DE.txtROWAuthority).prop("readonly", true);
            $(app.DE.txtROWSegments).prop("readonly", true);
            $(app.DE.txtROWLength).prop("readonly", true);
            $(app.DE.txtTotalRecurringCharges).prop("readonly", true);
            $(app.DE.txtROWSegments).val('0');
            $(app.DE.txtROWLength).val('0');
            $(app.DE.txtTotalRecurringCharges).val('0');
            $(app.DE.txtROWAuthority).val('');
        }
    }


    this.createUpdateLink = function (IsNewLink) {
        $('#txtLinkId').val($('#txtLinkId').val().toUpperCase());
        $('#txtFiberLinkId').val($('#txtLinkId').val().toUpperCase());
        let prefix = $('#txtLinkId').val().substring(0, 2).toUpperCase();
        let selectedLinkPrefix = $('#ddlLinkPrifixType option[value="' + prefix + '"]');
        if (prefix != '') {
            if (selectedLinkPrefix.length > 0) {
                $('#ddlLinkPrifixType').val(prefix).trigger('chosen:updated');
            }
        }
        if (selectedLinkPrefix.length <= 0) {
            alert(MultilingualKey.SI_OSP_GBL_NET_RPT_417);
            return false;
        }
        if (prefix != selectedLinkPrefix.val()) {
            alert(MultilingualKey.SI_OSP_GBL_NET_RPT_418);
            return false;
        }

        if ($('#ddlLinkType').val() == "") {
            $('#ddlLinkType').css('border-color', 'red');
            $(app.DE.ddlLinkType).css('border-color', 'red');
            return false;
        } else {

            $(app.DE.ddlLinkType).css('border-color', '');
            $('#ddlLinkType').css('border-color', '');
        }
        if ($('#txtLinkName').val() == "") {
            $('#txtLinkName').css('border-color', 'red');
            return false;
        }
        else {
            $('#txtLinkName').css('border-color', '');
        }
        if (($('#ddlLinkType').val() == "Redundant Link") && ($('#ddlMainlinktype').val() == "")) {
            $('#ddlMainlinktype').css('border-color', 'red');
            $('#ddlMainlinktype_chosen a').css('border-color', 'red');
            return false;
        } else {

            $('#ddlMainlinktype').css('border-color', '');
            $('#ddlMainlinktype_chosen a').css('border-color', '');
        }
        if (($('#ddlLinkType').val() == "Redundant Link") && ($('#txtmainlinkid').val() == "")) {
            $('#txtmainlinkid').css('border-color', 'red');
            return false;
        } else {

            $('#txtmainlinkid').css('border-color', '');
        }


        if (($('#ddlLinkType').val() == "Main Link") && ($('#ddlredundantlinktype').val() == "")) {
            $('#ddlredundantlinktype').css('border-color', 'red');
            $('#ddlredundantlinktype_chosen a').css('border-color', 'red');
            return false;
        } else {

            $('#ddlredundantlinktype').css('border-color', '');
            $('#ddlredundantlinktype_chosen a').css('border-color', '');
        }
        if (($('#ddlLinkType').val() == "Main Link") && ($('#txtredundantlinkid').val() == "")) {
            $('#txtredundantlinkid').css('border-color', 'red');
            return false;
        } else {
            $('#txtredundantlinkid').css('border-color', '');
        }

        if ($(app.DE.crossStartIconId).is(":visible")) {
            $(app.DE.txtStartPointNwkID).css('border-color', 'red');
            return false;
        } else {
            $(app.DE.txtStartPointNwkID).css('border-color', '');
        }
        if ($(app.DE.IconCross).is(":visible")) {
            $(app.DE.txtLinkId).css('border-color', 'red');
            return false;
        } else {
            $(app.DE.txtLinkId).css('border-color', '');
        }

        if ($(app.DE.crossEndIconId).is(":visible")) {
            $(app.DE.txtEndPointNwkID).css('border-color', 'red');
            return false;
        } else {
            $(app.DE.txtEndPointNwkID).css('border-color', '');
        }
        if (IsNewLink) {
            $(app.DE.frmAddFiberLink).submit();
            if (app.CableFiberButton == true) {
                $("#closeChildPopup").click();
                app.AssociatelinkId();
            }
        }
    }

    this.validateLinkId = function (_searchText, _columnName) {

        if (_searchText.trim().length > 0) {

            ajaxReq('FiberLink/validateLinkIdByText', { searchText: _searchText.trim(), columnName: _columnName.trim() }, true, function (resp) {

                if (resp.status != 'OK') {
                    $(app.DE.crossIcon).css('display', 'none');
                    $(app.DE.tickIcon).css('display', 'block');
                    $(app.DE.txtLinkId).css('border-color', '');
                }
                else {
                    $(app.DE.tickIcon).css('display', 'none');
                    $(app.DE.crossIcon).css('display', 'block');

                }
            });
        }
        else {
            $(app.DE.tickIcon).css('display', 'none');
            $(app.DE.crossIcon).css('display', 'none');
        }
    }

    this.validateStartPointNetworkId = function (_networkId) {
        if (_networkId.trim().length > 0 && ($(app.DE.ddlStartPointtype).val())) {
            ajaxReq('FiberLink/validateFiberLinkPointNetworkId', { networkId: _networkId.trim(), columnName: $(app.DE.ddlStartPointtype).val() }, true, function (resp) {
                if (resp.msg == 'Ok') {
                    $(app.DE.crossStartIcon).css('display', 'none');
                    $(app.DE.tickStartIcon).css('display', 'block');
                    $(app.DE.txtStartPointNwkID).css('border-color', '');
                }
                else {
                    $(app.DE.tickStartIcon).css('display', 'none');
                    $(app.DE.crossStartIcon).css('display', 'block');
                }

            })

        }
        else {
            $(app.DE.tickStartIcon).css('display', 'none');
            $(app.DE.crossStartIcon).css('display', 'none');
        }
    }

    this.validateEndPointNetworkId = function (_networkId) {
        if (_networkId.trim().length > 0 && ($(app.DE.ddlEndPointtype).val())) {
            ajaxReq('FiberLink/validateFiberLinkPointNetworkId', { networkId: _networkId.trim(), columnName: $(app.DE.ddlEndPointtype).val() }, true, function (resp) {
                if (resp.msg == 'Ok') {
                    $(app.DE.crossEndIcon).css('display', 'none');
                    $(app.DE.tickEndIcon).css('display', 'block');
                    $(app.DE.txtEndPointNwkID).css('border-color', '');
                }
                else {
                    $(app.DE.tickEndIcon).css('display', 'none');
                    $(app.DE.crossEndIcon).css('display', 'block');
                }

            })

        }
        else {
            $(app.DE.tickEndIcon).css('display', 'none');
            $(app.DE.crossEndIcon).css('display', 'none');
        }
    }

    this.validateStartEndPoint = function () {

        if ($(app.DE.ddlStartPointtype).val()) {
            if ((($(app.DE.txtStartPointNwkID).val() != '') || ($(app.DE.txtStartPointNwkID).val() != null))) {
                $(app.DE.txtLinkId).css('border-color', 'red');
                return false;
            }
        }
    }

    this.SaveFiberLink = function (objFiberLink) {
        debugger;
        if (objFiberLink.pageMsg.status != "OK" || ($('#hdnCheckforCLP').val()  !== undefined && $('#hdnCheckforCLP').val() != '')) {
            $('#txtfiberlink').val(objFiberLink.link_id);
            $('#hdnCheckforCLP').val('');
            alert(objFiberLink.pageMsg.message);
            $("#closeChildPopup").click();
        }
        else {
            if (objFiberLink.CreateFL == 0) {
                alert(objFiberLink.pageMsg.message);
            }
            ajaxReq('FiberLink/AddFiberLink', { system_id: 0 }, true, function (resp) {
                $(app.DE.CreateFiberLink).html(resp);
            }, false, true);
            $(app.DE.frmViewLink).submit();
            $("#closeChildPopup").click();
        }
    }

    this.ExportFiberLinkIntoKML = function (reportType, _linkSystemId, Index) {
        app.FiberLinkDrp(Index);
        window.location = appRoot + 'FiberLink/ExportFiberLinkIntoKML?SystemId=' + parseInt(_linkSystemId) + '&ReportType=' + reportType + '';

    }
    this.GetFiberLinkCustomer = function (_system_id) {
        popup.LoadModalDialog('CHILD', 'FiberLink/GetFiberLinkCustomer', { LinkId: _system_id }, MultilingualKey.SI_OSP_GBL_NET_FRM_430, 'modal-xl');
    }
    this.FiberLinkCustomerExport = function () {
        window.location = appRoot + 'FiberLink/ExportFiberLinkCustomer';
    }
    this.GetFiberLinkHistory = function (_system_id) {
        popup.LoadModalDialog('CHILD', 'Audit/GetFiberLinkHistory', { LinkSystemId: _system_id }, MultilingualKey.SI_OSP_GBL_NET_FRM_431, 'modal-xl');
    }
    this.FiberLinkAuditReport = function (downloadEntity) {
        window.location = appRoot + 'Audit/' + downloadEntity;
    }
    this.btnclearLinkId = function () {

        $(app.DE.txtFiberLinkId).val('');
        $(app.DE.txtFiberLinkId).css('border-color', '');
        $(app.DE.crossIcon).css('display', 'none');
        $(app.DE.tickIcon).css('display', 'none');
    }
    this.btncreatelink = function () {
        debugger;
        app.CableFiberButton = true;
        // Get the link ID and convert it to uppercase
        var _linkId = $(app.DE.txtFiberLinkId).val().toUpperCase();

        if (_linkId == "") {
            $(app.DE.txtFiberLinkId).css('border-color', 'red');
        }
        else {
            popup.LoadModalDialog('CHILD', 'FiberLink/CreateFiberLink', { system_id: 0, link_id: _linkId }, "Create Link", 'modal-xl');
        }
    }
    this.closepopup = function () {

        $(app.DE.dvAssociateLink).hide();
    }

    // Associate Link-----
    this.updateLinkId = function () {
        // var _linkId = $(app.DE.txtFiberLinkId).val();
        // var _fiberNo = $(app.DE.hdnFiberNumber).val();
        //var _cableId = $(app.DE.hdnCableId).val();

        // app.validateFiberlinkId(_linkId, _fiberNo, _cableId);
        // var _linkSystemId = $(app.DE.hdnLinkSystemId).val();

    }
    this.AssociatelinkId = function () {
        var _linkId = $(app.DE.txtFiberLinkId).val();
        var _fiberNo = $(app.DE.hdnFiberNumber).val();
        var _cableId = $(app.DE.hdnCableId).val();
        $('.progressbarShow').css('display', 'block');
        ajaxReq('FiberLink/validateFiberlinkId', { linkId: _linkId.trim(), columnName: "link_id" }, true, function (resp) {
            if (resp.objfiberLink != null) {
                $(app.DE.hdnLinkSystemId).val(resp.objfiberLink.system_id);
                $(app.DE.crossIcon).css('display', 'none');
                $(app.DE.tickIcon).css('display', 'block');
                $(app.DE.txtFiberLinkId).css('border-color', '');
                app.associateFiberLink(resp.objfiberLink.system_id, _linkId, _fiberNo, _cableId, 'A');
                $('.progressbarShow').css('display', 'none');
            }
            else {
                if (parseInt($(app.DE.hdnLinkSystemId).val()) && $(app.DE.txtFiberLinkId).val().trim().length == 0) {
                    app.associateFiberLink($(app.DE.hdnLinkSystemId).val(), _linkId, _fiberNo, _cableId, 'D');
                    $('.progressbarShow').css('display', 'none');
                }
                else {
                    $(app.DE.crossIcon).css('display', 'block');
                    $(app.DE.tickIcon).css('display', 'none');
                    $(app.DE.txtFiberLinkId).css('border-color', 'red');
                    $('.progressbarShow').css('display', 'none');
                }
            }

        });
    }

    this.associateFiberLink = function (_linkSystemId, _linkId, _fiberNo, _cableId, _action) {

        var gridFiberNumber = _fiberNo;
        var cableId = _cableId;
        var linkSystemId = 0;
        if (_linkSystemId != "0")
            linkSystemId = _linkSystemId;
        else
            linkSystemId = $(app.DE.hdnLinkSystemId).val();

        ajaxReq('FiberLink/AssociateFiberLinkWithCable', { link_system_id: _linkSystemId, cable_id: _cableId, fiber_no: _fiberNo, action: _action }, true, function (resp) {
            if (resp.status == 'OK') {
                $(app.DE.crossIcon).css('display', 'none');
                $(app.DE.tickIcon).css('display', 'none');
                $(app.DE.dvAssociateLink).hide();
                alert(app.StatusMessages.LINK_ASSOCIATED_SUCCESS);
                if (linkSystemId > 0 && $(app.DE.txtFiberLinkId).val().trim().length > 0) {

                    $(app.DE.lblGrdLinkId_ + gridFiberNumber).html($(app.DE.txtFiberLinkId).val());
                }
                else {

                    $(app.DE.lblGrdLinkId_ + gridFiberNumber).html(MultilingualKey.SI_OSP_GBL_JQ_FRM_194);
                }
            }
            else {
                $(app.DE.crossIcon).css('display', 'block');
                $(app.DE.tickIcon).css('display', 'none');
                if (app.CableFiberButton == true) {
                    alert(resp.message);
                    $(app.DE.dvAssociateLink).hide();
                    app.CableFiberButton = false;
                }
                else if (resp.message == "Fiber Link created but not associated as ODF to ODF connectivity not found!") {
                    alert(MultilingualKey.SI_OSP_GBL_NET_RPT_298);
                    $(app.DE.dvAssociateLink).hide();
                }
                else {
                    alert(resp.message);
                    $(app.DE.dvAssociateLink).hide();
                }
            }
        });
    }
}
