$('.chosen-select').chosen({ placeholder_text_multiple: MultilingualKey.SI_OSP_GBL_NET_RPT_307, width: '100%' });

$("#btnExportReportIntoExcel").on("click", function () {
    si.SiteReport.ExportSiteReport('Excel', 'All');
    $('.FlowDiv').slideUp();
});
$("#btnExportReportIntoKML").on("click", function () {
    si.SiteReport.ExportSiteReport('KML', 'All');
    $('.FlowDiv').slideUp();
});
function onChangeCustomDate() {

    var value = $('#customedate option:selected').val();

    SetFromAndToDate(value);
}
function SetFromAndToDate(value) {

    var endDate = new Date();
    var startDate = new Date();
    var startDateDiff = 0;
    var endDateDiff = 0;

    if (value == 1) // custom
    {
        $('#txtDateFrom').val("");
        $('#txtDateTo').val("");
        $("#imgFromDate").show();
        $('#imgToDate').show();
        return;
    }
    else if (value == 2) { //Today
        startDateDiff = 0;
        endDateDiff = 0;
        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }
    else if (value == 3) { //previous day
        startDateDiff = -1;
        endDateDiff = -1;
        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }
    else if (value == 4) {//last 7 days
        startDateDiff = -6;
        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }
    else if (value == 5) {//last 30 days
        startDateDiff = -29;
        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }
    else if (value == 6) {//last 3 Months
        startDateDiff = -89;
        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }
    else if (value == 7) {//last 6 Months
        startDateDiff = -179;
        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }
    else if (value == 8) {//last 6 Months
        startDateDiff = -364;
        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }

    startDate.setDate(startDate.getDate() + startDateDiff);
    endDate.setDate(endDate.getDate() + endDateDiff);

    var newStartDate = startDate.toDateString();
    newStartDate = new Date(Date.parse(newStartDate));

    var newEndDate = endDate.toDateString();
    newEndDate = new Date(Date.parse(newEndDate));

    $('#txtDateFrom').val(fncCurrentDate(newStartDate));
    $('#txtDateTo').val(fncCurrentDate(newEndDate));
    if (value == 0 || value == 9) {
        $('#txtDateFrom').val('');
        $('#txtDateTo').val('');
        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }
}
$('#btnExport').on('click', function () {
    $('.FlowDiv').slideToggle();

});
$('#btnExports').on('click', function () {
    $('.FlowDivs').slideToggle();

});

$(document).ready(function () {
    $('#hdnLayerName').val('POD');
    $('#tblExportReport thead tr th a').each(function () {
        var html = $(this).html(); $(this).parent().attr('data-column-name', html);
    })
    $('#tblExportReport tbody tr td').each(function () {
        var html = $(this).html(); $(this).attr('data-value', html);
    })
    $('#tblExportReport tbody tr').each(function (index, value) {
        var replacetdIndex = 1;
        var firsttdHtml = $('#tblExportReport  tbody tr:eq(' + index + ') td:eq(' + replacetdIndex + ')').html();
        $('#tblExportReport  tbody tr:eq(' + index + ') td:eq(' + replacetdIndex + ')').html('<div class="Created"></div>' + '<div style="width:150px;">' + firsttdHtml + '</div>')
        $('#tblExportReport thead tr th:eq(0)').html('' + MultilingualKey.SI_OSP_GBL_GBL_GBL_059 + '');
        var systemId = $('#tblExportReport  tbody tr:eq(' + index + ') td:eq(0)').attr('data-value');
        var region_id = $('#tblExportReport  tbody tr:eq(' + index + ') td:eq(31)').attr('data-value');
        var province_id = $('#tblExportReport  tbody tr:eq(' + index + ') td:eq(32)').attr('data-value');
        var rowAction = '<a href="#" data-value="' + systemId + '"  class="dropdown-toggle rowShowOnMap" title= "' + MultilingualKey.SI_OSP_GBL_GBL_GBL_036 + '">';
        rowAction = rowAction + '<i class="fa fa-globe fa-fw m-r-xs"></i></a>';
        rowAction = rowAction + ' &nbsp;&nbsp;<a href="#" data-value="' + systemId + ',' + region_id + ',' + province_id + '" class="dropdown-toggle rowShowSiteAwarding" title= "' + MultilingualKey.SI_OSP_GBL_GBL_RPT_002 + '"><i class="fa fa-user-plus"></i></a>';
        /* rowAction = rowAction + ' &nbsp;&nbsp;<a href="#" data-value="' + systemId + ',' + region_id + ',' + province_id + '" class="dropdown-toggle rowShowTopology" title= "' + 'Topology Plan' + '"><i class="icon-topology"></i></a>';*/
        rowAction = rowAction + ' &nbsp;&nbsp;<a href="#" data-value="' + systemId + '" class="dropdown-toggle rowShowTopology" title= "' + 'Topology Plan' + '"><span class="icon-topology"></span></a>';
        rowAction = rowAction + ' &nbsp;&nbsp;<a href="#" data-value="' + systemId + '" class="rowDataSync" title= "' + 'Update Service' + '"><span class="fa fa-refresh"></span></a>';

        $('#tblExportReport  tbody tr:eq(' + index + ') td:eq(0)').html(rowAction);
    });

    $('.rowShowSiteAwarding').on("click", function () {
        var systemId = $(this).attr('data-value');
        //si.SiteReport.SiteAwarding(null, systemId, null, this);
        si.SiteReport.ItemSiteAwarding(null, systemId, null, this,'','');
       
    });

    $('.rowShowOnMap').on("click", function () {
        
        var systemId = $(this).attr('data-value');
        si.ShowEntityOnMap(systemId, 'POD', 'Point'); $(popup.DE.MinimizeModel).trigger("click");
    });
    $('.rowShowTopology').on("click", function () {
        var systemId = $(this).attr('data-value');
        si.SiteReport.SiteTopology(null, systemId, null, this);

    });
    $('.rowDataSync').on("click", function () {

        var input = $(this).attr('data-value');
      
        ajaxReq('Report/updateSiteDataservice', { systemId: input }, true, function (data) {

            alert(data.message);
            $('#btnShowReportData').trigger("click");
            
        }, false, false);

    });

    if ($("#customedate_chosen").children('a').text().trim() != "Custom") {

        $('#imgFromDate').hide();
        $('#imgToDate').hide();
    }

    addDefaultSortingIcons('tblExportReport');
    si.setDateTimeCalendar_ExportEntities("txtDateFrom", "txtDateTo", "imgFromDate", "imgToDate", false);

    $('#btnEntityExportClear').on("click", function () {
        si.report.ResetRowReportFilter();
    });

    $('tfoot a').on("click", function () {
        $('form').attr('action', $(this).attr('href')).submit();
        return false;
    });

    //Post form on page header click..
    $('th a').on("click", function () {
        $('form').attr('action', $(this).attr('href')).submit();

        return false;
    });

    $("#btnShowReportData").on("click", function () {
        if ($('#ddlLayerColumns option:selected').text() != MultilingualKey.SI_OSP_GBL_NET_RPT_307 && $('#txtSearchText').val() == "") {
            alert(MultilingualKey.SI_OSP_GBL_JQ_RPT_006);
            return false;
        }
        else if ($('#ddlLayerColumns option:selected').text() == MultilingualKey.SI_OSP_GBL_NET_RPT_307 && $('#txtSearchText').val() != "") {
            alert(MultilingualKey.SI_OSP_GBL_JQ_RPT_007);
            return false;
        }
        
        else {
            return true;
        }
    });


    $('#tblSegmentReport thead tr th a').each(function () {
        var html = $(this).html(); $(this).parent().attr('data-column-name', html);
    })
    $('#tblSegmentReport tbody tr td').each(function () {
        var html = $(this).html(); $(this).attr('data-value', html);
    })
    $('#tblSegmentReport tbody tr').each(function (index, value) {
        var replacetdIndex = 1;
        var firsttdHtml = $('#tblSegmentReport  tbody tr:eq(' + index + ') td:eq(' + replacetdIndex + ')').html();
        $('#tblSegmentReport  tbody tr:eq(' + index + ') td:eq(' + replacetdIndex + ')').html('<div class="Created"></div>' + '<div style="width:150px;">' + firsttdHtml + '</div>')
        $('#tblSegmentReport thead tr th:eq(0)').html('' + MultilingualKey.SI_OSP_GBL_GBL_GBL_059 + '');
        var systemId = $('#tblSegmentReport  tbody tr:eq(' + index + ') td:eq(0)').attr('data-value');
        
        var rowAction = '<a href="#" data-value="' + systemId + '"  class="dropdown-toggle rowSegemetShowOnMap" title= "' + MultilingualKey.SI_OSP_GBL_GBL_GBL_036 + '">';
        rowAction = rowAction + '<i class="fa fa-globe fa-fw m-r-xs"></i></a>';
       /* rowAction = rowAction + ' &nbsp;&nbsp;<a href="#" data-value="' + systemId + '" class="dropdown-toggle rowDeleteSegmentt" title= "' + 'Delete Segment' + '"><span class="cptr icon-Delete ml-05"></span></a>';*/

        $('#tblSegmentReport  tbody tr:eq(' + index + ') td:eq(0)').html(rowAction);
    });

    $('.rowDeleteSegmentt').on("click", function () {
        var systemId = $(this).attr('data-value');
        si.SiteReport.SiteTopology(null, systemId, null, this);

    });
    $('.rowSegemetShowOnMap').on("click", function () {

        var systemId = $(this).attr('data-value');
        si.showRouteDetailsOnMap(systemId);
    });
    addDefaultSortingIcons('tblSegmentReport');

    $("#btnSegmentReportIntoExcel").on("click", function () {
        si.ExportSegmentReport('Excel', 'All');
        $('.FlowDiv').slideUp();
    });

});