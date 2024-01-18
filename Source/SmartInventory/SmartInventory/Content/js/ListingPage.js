//Path	=>   ~/Content/js/ListingPage.js







var ListingPage = function () {
    var comp = this;
   



 this.AE = {
        "SearchRoute": "#btnSearchRoute",
        "SearchRouteAssign": "#btnSearchRouteAssign",
        "TxtStartDate": "#txtStartDate",
        "TxtEndDate": "#txtEndDate",
        "DownloadReport": "#btnDownloadReport",
        "SearchUserAttendance": "#btnSearchUserAttendance",
        "SearchUserTimeSheet": "#btnUserTimeSheet",
        "DurationDrpDwn": "#ddlduration",
        "ddlRoleType": "#ddlRoleType",
        "ddlUser": "#ddlUser",
        "DownloadUserAttendanceReport": "#btnDownloadUserAttendanceReport",
        "DownloadAssignedRouteIssueReport": "#btnDownloadAssignedRouteIssueReport",
        "DownloadRouteIssueReport": "#btnDownloadRouteIssueReport"

    }
   
    this.initApp = function () {
        //  
        comp.ChkAddClass();
        comp.bindEvents();
       // $(".chosen-select").select2("destroy").select2();        //Add Icons on Header
        $(".webgrid tr th").addClass("bothorder"); // or you can add icon class here
        $('thead tr th').last().removeClass('bothorder');
        //$(comp.AE.ddlRoleType).val(3);
    }

    this.bindEvents = function () {

        $(comp.AE.SearchRoute).on("click", function () {
            return comp.ValidateSearchFilters();
        });

        $(comp.AE.SearchRouteAssign).on("click", function () {
            //var result = comp.ValidateSearchFilters();
            //if (result == true)
            //{
            //    comp.ResetFilters(frmViewRouteAssign);//frmViewRouteAssign is a complete form object.
            //}
            return comp.ValidateSearchFilters();
        });

        //Post form on page footer click..
        $('tfoot a').on("click", function () {
            $('form').attr('action', $(this).attr('href')).submit();
            return false;
        });

        //Post form on page header click..
        $('th a').on("click", function () {
            $('form').attr('action', $(this).attr('href')).submit();
            RL.ChkAddClass();
            return false;
        });

        $(comp.AE.DownloadReport).on("click", function () {
            //totalRowCount and maxExportReportCount defined on specific page for eg: _ViewAssignedRoute.cshtml
            if (totalRowCount > maxExportReportCount) {
                alert("Report Count should be less than " + maxExportReportCount + "");
                return false;
            }
            var reportUrl = 'Main/DownloadRouteAssignedReport';
            sp.DownloadReport(reportUrl);
        });
        $(comp.AE.DownloadUserAttendanceReport).on("click", function () {
            //totalRowCount and maxExportReportCount defined on specific page for eg: _ViewAssignedRoute.cshtml
            if (totalRowCount > maxExportReportCount) {
                alert("Report Count should be less than " + maxExportReportCount + "");
                return false;
            }
            var reportUrl = 'Main/DownloadUserAttendanceReport';
            sp.DownloadReport(reportUrl);
        });

        $(comp.AE.DownloadAssignedRouteIssueReport).on("click", function () {
            //totalRowCount and maxExportReportCount defined on specific page for eg: _ViewAssignedRoute.cshtml
            if (totalRowCount > maxExportReportCount) {
                alert("Report Count should be less than " + maxExportReportCount + "");
                return false;
            }
            var reportUrl = 'Main/DownloadRouteAssignIssueReport';
            sp.DownloadReport(reportUrl);
        });
        $(comp.AE.DownloadRouteIssueReport).on("click", function () {
            //totalRowCount and maxExportReportCount defined on specific page for eg: _ViewAssignedRoute.cshtml
            if (totalRowCount > maxExportReportCount) {
                alert("Report Count should be less than " + maxExportReportCount + "");
                return false;
            }
            var reportUrl = 'Main/DownloadRouteIssueReport';
            sp.DownloadReport(reportUrl);
        });

        $(comp.AE.SearchUserAttendance).on("click", function () {

            return comp.ValidateSearchFilters();
        });

        $(comp.AE.SearchUserTimeSheet).on("click", function () {

            return comp.ValidateSearchFilters();
        });


    }

    this.ChkAddClass = function () {
        $(".webgrid tr th").addClass("bothorder");
        $('thead tr th').last().removeClass('bothorder');

        var dir = $('#dir').val(); //direction value
        var col = $('#col').val(); // header value
        var clickedheader = $('th a[href*=' + col + ']');
        var countTh = document.getElementsByTagName('th').length; //total column header

        for (var i = 1; i <= countTh; i++) {

            var txtTh = $('.webgrid tr th:nth-child(' + i + ')').text(); // header text

            if (txtTh.trim() == clickedheader.text() && dir == 'Ascending') {

                $('.webgrid tr th:nth-child(' + i + ')').removeClass("bothorder");
                $('.webgrid tr th:nth-child(' + i + ')').addClass("ascendingorder");

            }

            else if (txtTh.trim() == clickedheader.text() && dir == 'Descending') {
                $('.webgrid tr th:nth-child(' + i + ')').removeClass("bothorder");
                $('.webgrid tr th:nth-child(' + i + ')').addClass("descendingorder");
            }
        }
    }


    this.ValidateSearchFilters = function () {
        // 
        var selStartDate = $(comp.AE.TxtStartDate).val();
        var selEndDate = $(comp.AE.TxtEndDate).val();
        if (selStartDate != "" && selEndDate != "") {
            selStartDate = sp.DateFormatter(selStartDate);
            var startDate = new Date(selStartDate);
            selEndDate = sp.DateFormatter(selEndDate);
            var endDate = new Date(selEndDate);
            if (startDate > endDate) {
                alert("To date should be greater than From date.");
                return false;
            }
        }
        return true;
    }

    //this.ResetFilters = function (form) {
    //    $(':input', form).each(function () {
    //        var type = this.type;
    //        var tag = this.tagName.toLowerCase();

    //        if (type == 'text' || type == 'date')
    //            this.value = "";
    //        else if (tag == 'select')
    //        {
    //            this.selectedIndex = 0;
    //            $(this).trigger("change");
    //        }
    //    });
    //}

    this.ToggleExportBtn = function (AssignCount, scheduledCount) {

        $(comp.AE.DownloadReport).removeAttr("disabled");

        if (modelRadioValue == 1 && AssignCount == 0) {
            $(comp.AE.DownloadReport).attr("disabled", "disabled");
        }
        if (modelRadioValue == 2 && scheduledCount == 0) {
            $(comp.AE.DownloadReport).attr("disabled", "disabled");
        }
    }

    this.ToggleElement = function (durationVal) {
        $("#imgFromDate").show();
        $("#imgToDate").show();

        if (durationVal != 1) {
            $("#imgFromDate").hide();
            $("#imgToDate").hide();
        }

        //_setDate(durationVal)
    }

    this.SetDate = function () {
       //  
        var sp = new Main();
        var drpDurationVal = $(comp.AE.DurationDrpDwn).val();
        $("#imgFromDate").show();
        $("#imgToDate").show();

        if (drpDurationVal != 1) {
            $("#imgFromDate").hide();
            $("#imgToDate").hide();
        }

        _setDate(drpDurationVal)
    }

    function _setDate(drpDurationVal) {
        
        var endDate = new Date();
        var startDate = new Date();
        var startDateDiff = 0;
        var endDateDiff = 0;

        if (drpDurationVal == 1) // custom
        {
            $(comp.AE.TxtStartDate).val("");
            $(comp.AE.TxtEndDate).val("");
            return;
        }
        else if (drpDurationVal == 3) { //previous day
            startDateDiff = -1;
            endDateDiff = -1;
        }
        else if (drpDurationVal == 4) {//last 7 days
            startDateDiff = -6;
        }
        else if (drpDurationVal == 5) {//last 30 days
            startDateDiff = -29;
        }

        startDate.setDate(startDate.getDate() + startDateDiff);
        endDate.setDate(endDate.getDate() + endDateDiff);

        var newStartDate = startDate.toDateString();
        newStartDate = new Date(Date.parse(newStartDate));

        var newEndDate = endDate.toDateString();
        newEndDate = new Date(Date.parse(newEndDate));
        var sp = new Main();
        $(comp.AE.TxtStartDate).val(fncCurrentDate(newStartDate));
       
        $(comp.AE.TxtEndDate).val(fncCurrentDate(newEndDate));
    }
    this.ToggleExportPatrollerAttendBtn = function (patrollerAttendanceCount) {

        $(comp.AE.DownloadPatrollerAttendanceRpt).removeAttr("disabled");

        if (patrollerAttendanceCount == 0) {
            $(comp.AE.DownloadPatrollerAttendanceRpt).attr("disabled", "disabled");
        }

    }

    this.fillRoleAndUsers = function () {
        // 
        var selectedRoleId = $(comp.AE.ddlRoleType).val();
        comp.BindUser(selectedRoleId);
        //$(comp.AE.ddlRoleType).val(1).trigger('change');
    }

    this.BindUser = function (vRoleId) {
        sp.ajaxReq('Main/GetUser', { roleId: vRoleId }, true, function (resp) {
            $(comp.AE.ddlUser).empty();
            $(comp.AE.ddlUser).append($("<option     />").val('').text("--All--"));
            if (resp != null && resp != undefined) {
                $.each(resp, function () {
                    $(comp.AE.ddlUser).append($("<option     />").val(this.user_id).text(this.full_name + " (" + this.user_name + ")"));
                });
            }
        }, true);
        $(comp.AE.ddlUser).trigger('change');
        $(comp.AE.ddlUser).val('').trigger('change');
    }
}