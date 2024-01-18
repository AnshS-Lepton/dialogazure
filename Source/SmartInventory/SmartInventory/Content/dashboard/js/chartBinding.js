var totRec = null;
var jsonObj = null;


function drawChartPlaceHolder(jdata) {
    let yAxisFields = ['cable_measured_length', 'cable_calculated_length']
    let fields = Object.keys(jdata[0]);
    let divhtml = '';
    fields.forEach(function (item) {
        if (item != 'cityname') {
            divhtml += ` <div class="widgets__col--columns" id='div_${item}'>
            <div class="d-flex hpad">
                <div class="widgets__col--head">${item.replaceAll('_', ' ')}</div>
                <div class="otherDropD ml-auto d-flex">
                    <div class="iconsBox d-flex">
                        <a href="#"><img src="/Areas/Admin/Content/images/dpdf.png" onclick="fndownloadPdf('${item}');" /> </a>
                        <a href="#"><img src="/Areas/Admin/Content/images/dPng.svg" onclick="fndownloadImg('${item}');" /> </a>
                        <a href="#"><img src="/Areas/Admin/Content/images/dCsv.svg" onClick='tabletoExcel("${item}");' /></a>
                        <a href="javascript:void(0)" onclick="expandChart(this)" fieldname='${item}'><img src="/Areas/Admin/Content/images/expand.svg"  /></a>
                    </div>
                    <select id="charttype_${item}" onchange="drawChart(this.value,'${item}')">
                        <option value="BarChart" selected>Bar Chart</option>
                        <option value="PieChart" >Pie Chart</option>
                        <option value="LineChart">Line Chart</option>
                        <option value="DonutChart">Donut Chart</option>
                    </select>
                </div>
            </div>
            <div class="widgets__col--body" id="${item}"></div>
            <div class="widgets__col--fotter d-flex">
                <label>Total Records: &nbsp;</label><label id="dvrecord_${item}" class="fw-60"></label>
                <div class="ml-auto">
                    <label for="Islabelon" class="fw-60">Show Label &nbsp;</label>
                    <input type="checkbox" class="floatNone" id="Islabelon_${item}" fieldname='${item}' onclick="lableShowHide(this)">
                </div>
            </div>
        </div>`;
        }
    });
    $('#chartContainer').html(divhtml);
    fields.forEach(function (item) {
        if (item != 'cityname') {
            drawChart('BarChart', item);
        }
    });
}

function drawChartPlaceHolderSingle() {
    let divhtml = '';
    divhtml += ` <div class="widgets__col--columns" id='div_chartmain'><div id="chartHeader" style="display:none;">
            <div class="d-flex hpad">
                <div class="widgets__col--head" >
                    <label id="titlefsa_id">${$('#XAxisFieldName option:selected').text() == 'Select' ? '' : $('#XAxisFieldName option:selected').text()}</label>
                </div>
                <label class="flexCenter">Total Records: &nbsp;<span id="mainFooter" class="fw-60"></span></label>
                <div class="otherDropD ml-auto d-flex">
                    <div class="iconsBox d-flex">
                        <a href="javascript:void(0)" id="btnSaveTemplate" onclick="btnModalOpener()" title="Save Template"><img src="/images/template.svg"/> </a>
                        <a href="javascript:void(0)" onclick="fndownloadPdf(this);"><img src="/images/pdf.svg"/> </a>
                        <a href="javascript:void(0)"><img src="/images/png.svg" onclick="fndownloadImg('${$('#XAxisFieldName').val() }');" /> </a>
                        <a href="javascript:void(0)"><img src="/images/excel.svg" onClick='tabletoExcel("${$('#XAxisFieldName').val() }");' /></a>
                        <a href="javascript:void(0)" onclick="expandChart(this)" fieldname='${$('#XAxisFieldName').val() }'><img src="/images/expand.svg"  /></a>
                    </div>
                    <select id="charttype_fsa_id" onchange="changeChartType(this.value)" class="form-control ml-05">
                        <option value="BarChart" selected>Bar Chart</option>
                        <option value="LineChart">Line Chart</option>
                    </select>
                </div>
            </div> </div>
            <div class="widgets__col--body" id="div_chartbody"></div>
        </div>`;

    $('#chartContainer').html(divhtml);
    // drawChart($('#charttype_fsa_id').val(), $('#XAxisFieldName').val(), $('#XAxisFieldName').val());
}
function showchart(kpiname) {
    //Add or Remove Pie Chart option according to Y Axis field selection
    if ($("#YAxisFieldName :selected").length > 1) {
        $("#charttype_fsa_id option[value='PieChart']").remove();
    } else {
        $('#charttype_fsa_id').append(`<option value="PieChart">Pie Chart</option>`);
    }
    let charttype = $('#charttype_fsa_id').val();
    $('#titlefsa_id').text($('#XAxisFieldName option:selected').text());

    drawChart($('#charttype_fsa_id').val() == undefined ? 'BarChart' : $('#charttype_fsa_id').val(), $('#XAxisFieldName').val());
    $('#chartHeader').show();
}

function changeChartType(charttype) {
    drawChart(charttype, $('#XAxisFieldName').val());
}
function expandChart(chartid) {
    let charttype = $('#charttype_fsa_id').val();
    $('#chartContainer1').html('<label>Total Records: &nbsp;</label><label id="dvrecord1" class="fw-60"> ' + $('#tblContainer table tbody tr').length + '</label>');
    drawChart(charttype, $(chartid).attr('fieldname'), 'popbody', $("#Islabelon_" + $(chartid).attr('fieldname')).is(":checked"));
    $('#popupHeading').text($("#XAxisFieldName :selected").text());
    $('#popupBox').toggleClass('showHide');
}

function lableShowHide(chartid) {
    let charttype = $('#charttype_' + $(chartid).attr('fieldname')).val();
    drawChart(charttype, $(chartid).attr('fieldname'), $(chartid).attr('fieldnam'), $("#Islabelon_" + $(chartid).attr('fieldname')).is(":checked"));
}


function drawChart(graphValue, charval, modeldiv, showlabel) {

    jsonObj = JSON.parse($('#chartdata').text());
    if (jsonObj.length > 0)
        $('.totalPages').show();

    const jsonObject = jsonObj;
    let chart;
    let ismodel = false;
    var totRec = null;
    keytoDrawChart = charval;
    if ($('#chartContainer').html().length == 0) {
        drawChartPlaceHolderSingle();

    }
    let groupdata = returnGroupByKey(jsonObject, keytoDrawChart);
    let gglData = getJsonToGTable(groupdata);
    options = {};
    if (modeldiv != undefined) {
        keytoDrawChart = modeldiv;
        ismodel = true;
        options = {
            chartArea: {
                width: '98%',
                height: 300,
                left: '7%',
                top: 30
            },
            width: '100%',
            height: 500,
        }
    } else {
        options = {
            chartArea: {
                width: '98%',
                height: 300,
                left: '7%',
                top: 30
            },
            width: '100%',
            height: 500,
        }
    }

    // arrayToDataTable is a static method, "new" keyword not needed
    let data = google.visualization.arrayToDataTable(gglData);

    // Instantiate and draw our chart, passing in some options.
    var table = new google.visualization.Table(document.getElementById('tblContainer'));

    table.draw(data, { showRowNumber: true, width: '100%', height: '100%' });
    $('#mainFooter').text(groupdata.length);
    if (graphValue == 'DonutChart') {
        options.pieHole = 0.4;
    } else if (graphValue == 'BarChart') {
        options.bar = { groupWidth: "90%" },
            options.role = "annotation"
        //options.legend = { showlabel: "value" }
    }
    else {
        options.slices = {
            4: { offset: 0.2 },
            12: { offset: 0.3 },
            14: { offset: 0.4 },
            15: { offset: 0.5 },
        },
            options.pointsVisible = true,

            options.series = {
                0: { targetAxisIndex: 0 },
                1: { targetAxisIndex: 1 }
            };
    }
    options.pieSliceText = showlabel ? 'value' : 'label';

    if (graphValue != 'PieChart')
        options.legend = { position: 'top', alignment: 'start' };

    keytoDrawChart = ismodel ? modeldiv : 'div_chartbody';
    $('#' + keytoDrawChart).html('');
    switch (graphValue) {
        case 'PieChart':
            chart = new google.visualization.PieChart(document.getElementById(keytoDrawChart));
            chart.draw(data, options);
            break;
        case 'BarChart':
            chart = new google.visualization.ColumnChart(document.getElementById(keytoDrawChart));
            chart.draw(data, options);
            break;
        case 'LineChart':
            chart = new google.visualization.LineChart(document.getElementById(keytoDrawChart));
            chart.draw(data, options);
            break;
        //case 'ColumnChart':
        //    chart = new google.visualization.ColumnChart(document.getElementById(keytoDrawChart));
        //    chart.draw(data, options);
        //    break;
        case 'DonutChart':
            chart = new google.visualization.PieChart(document.getElementById(keytoDrawChart));
            chart.draw(data, options);
            break;
    }
    mychart = chart;
}

function tabletoExcel(name) {
    $('#tblContainer table th,#tblContainer table td').css('border', '1px solid #333333');
    var htmltable = document.getElementById('tblContainer');
    var html = htmltable.outerHTML;
    window.open('data:application/vnd.ms-excel,' + encodeURIComponent(html));
}
function getJsonToGTable(jsonData) {
    var gglData = [];
    if (jsonData.length > 0) {
        // load column headings
        var colHead = [];
        Object.keys(jsonData[0]).forEach(function (key) {
            colHead.push(key);
        });
        gglData.push(colHead);

        // load data rows
        jsonData.forEach(function (row) {
            var gglRow = [];
            Object.keys(row).forEach(function (key) {
                gglRow.push(row[key]);
            });
            gglData.push(gglRow);
        });
    }
    return gglData;
}
function fndownloadImg(imgTitle) {
    var a = document.createElement("a");
    a.href = mychart.getImageURI(); //Image Base64 Goes here
    a.download = "chart.png"; //File name Here
    a.click();
}

function fndownloadPdf(obj) {
    
    $(obj).addClass("disable-click");
    $('#divloader').removeClass("dNone");
    var kpi_name = $('#ddlKPIName option:selected').text();
    var kpi_category = $('#ddlKPICategory option:selected').text();
    var kpi_id = $('#ddlKPIName').val();
    var chart = mychart.getImageURI().split(",");
    var parameters = {
        chart: chart[1],
        kpi_id: kpi_id,
        kpi_name: kpi_name,
        kpi_category: kpi_category
    };


    fetch("/KPI/ExportPdf", {
        body: JSON.stringify(parameters),
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    })
        .then(response => response.blob())
        .then(response => {
            const blob = new Blob([response], { type: 'application/pdf' });
            const downloadUrl = URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = downloadUrl;
            a.download = "file.pdf";
            document.body.appendChild(a);
            a.click();
            $(obj).removeClass("disable-click");
            $('#divloader').addClass("dNone");
            
        });
}

function exportToCSV(fieldname, xAxisFieldName) {
    let filename = fieldname + ".csv";
    const jsonObject = returnGroupByKey(jsonObj, xAxisFieldName);
    let json = getDatabyKeyArray(jsonObject, fieldname, xAxisFieldName);
    let fields = Object.keys(json[0])
    let replacer = function (key, value) { return value === null ? '' : value }
    let csv = json.map(function (row) {
        return fields.map(function (fieldName) {
            return JSON.stringify(row[fieldName], replacer)
        }).join(',')
    });
    fields = fields.map(function (fn) {
        return toTitleCase(fn)
    });
    csv.unshift(fields.join(',')) // add header column
    csv = csv.join('\r\n');
    console.log(csv);

    let blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    downloadFile(filename, blob);
}
function toTitleCase(str) {
    str = str.replaceAll('_', ' ');
    return str.replace(
        /\w\S*/g,
        function (txt) {
            return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        }
    );
}
function downloadFile(fileName, urlData) {

    if (navigator.msSaveBlob) { // IE 10+
        navigator.msSaveBlob(urlData, fileName);
    } else {
        let link = document.createElement("a");
        if (link.download !== undefined) { // feature detection
            // Browsers that support HTML5 download attribute
            let url = URL.createObjectURL(urlData);
            link.setAttribute("href", url);
            link.setAttribute("download", fileName);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }
}
function getDatabyKeyArray(jsonobj, objkey, xAxisFieldName) {
    let finaldata = [];
    jsonobj.forEach(function (item, i) {
        let final = {};
        final[xAxisFieldName] = item[xAxisFieldName];
        final.value = item[objkey] == null ? 0 : item[objkey];
        finaldata.push(final);
    });
    return finaldata;
}
function getDatabyKeyArraywithYAxis(jsonobj, objkey) {
    let finaldata = [];
    let xAxisFieldName = $('#xAxisFieldName').val();
    jsonobj.forEach(function (item, i) {
        let final = {};
        final[xAxisFieldName] = item[xAxisFieldName];
        final.cable_measured_length = item.cable_measured_length;
        final.cable_calculated_length = item.cable_calculated_length;
        finaldata.push(final);
    });
    return finaldata;
}
function returnGroupByKey(jsonobj, xAxisField, YAxisFields) {
    var grphKey = new Array();
    var groupedObjects = new Array();
    YAxisFields = $("#YAxisFieldName").chosen().val();
    $.each(jsonobj, function (ix, obj) {
        var existingObj;
        if ($.inArray(obj[xAxisField], grphKey) >= 0) {
            existingObj = $.grep(groupedObjects, function (o, si) { return o[xAxisField] === obj[xAxisField]; });
            for (i = 0; i < YAxisFields.length; i++) {
                existingObj[0][YAxisFields[i]] += obj[YAxisFields[i]];
            }
        } else {
            let newobj = {};
            newobj[xAxisField] = obj[xAxisField];
            for (i = 0; i < YAxisFields.length; i++) {
                newobj[YAxisFields[i]] = obj[YAxisFields[i]];
            }
            groupedObjects.push(newobj);
            grphKey.push(obj[xAxisField]);
        }
    });
    return groupedObjects;
}

function BindKPICategory() {
    var grphKey = new Array();
    $('#ddlKPICategory').empty().append('<option>Select</option>');
    kpi_data.forEach(function (obj) {
        var existingObj;
        if ($.inArray(obj.category_id, grphKey) == -1) {
            $('#ddlKPICategory').append(`<option value='${obj.category_id}'>${obj.category_name}</option>`);
            grphKey.push(obj.category_id);
        }
    });
    if (kpi_temp_details != '') {
     
        $('#ddlKPICategory').val(kpi_temp_details.kpi_category_id);
        $('#ddlKPICategory').trigger('change');
    }

}

function BindKPI(catId) {
    $('#btnExcelData').css('display', 'none');
    $('#ddlKPIName').empty().append('<option>Select</option>');
    kpi_data.forEach(function (kpi) {
        if (kpi.category_id == catId) {
            $('#ddlKPIName').append(`<option value='${kpi.kpi_id}' x_axis_column='${kpi.x_axis_column}' y_axis_column='${kpi.y_axis_columns}'>${kpi.kpi_name}</option>`);
        }
    });
    if (kpi_temp_details != '') {
        $('#ddlKPIName').val(kpi_temp_details.kpi_category_name_id);
        $('#ddlKPIName').trigger('change');
    }
    
}
function fnSaveTemplate() {
    var templatename = $('#templatename').val();
    var kpi_name = $('#ddlKPIName').val();
    var kpi_category = $('#ddlKPICategory').val();
    var x_axis = $('#XAxisFieldName option:selected').text();
    var y_axis = $("#YAxisFieldName").chosen().val().toString();
    $('#btnSaveTemplatepp').removeClass('showHide');
    var parameters = {
        template_name: templatename,
        kpi_category_id: kpi_category,
        kpi_category_name_id: kpi_name,
        x_axis_column: x_axis,
        y_axis_columns: y_axis,
        created_by:0
    };
    fetch("/KPI/SaveKPItemplate", {
        body: JSON.stringify(parameters),
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }
        
    }).then(response => {
        if (response.statusText == "OK") {
            pageMessageFromhtml("Template Saved Successfully.",2);
            $('#templatename').val('');
            BindTemplate();
        }
    });
}



function BindXYValues() {
    setTimeout(function () {
        let kpi_id = $('#ddlKPIName').val();
        const grid = new MvcGrid(document.querySelector("#grdKPIList"));
        grid.url.searchParams.set("kpi_id", kpi_id);
        //$('#divClosureSummaryCity').remove();
        grid.reload();
    }, 200);

    $('#chartHeader').hide();
    let xAxisvalues = $('#ddlKPIName option:selected').attr('x_axis_column').split(',');
    let yAxisvalues = $('#ddlKPIName option:selected').attr('y_axis_column').split(',');
    $('#XAxisFieldName').empty().append('<option>Select</option>');
    $('#YAxisFieldName').empty().append('<option>Select</option>');
    
    xAxisvalues.forEach(function (item) {
        if (item != 'null')
            $('#XAxisFieldName').append(`<option>${item.trim()}</option>`);
    });

    yAxisvalues.forEach(function (item) {
        if (item != 'null')
            $('#YAxisFieldName').append(`<option>${item.trim()}</option>`);
    });

    $("#YAxisFieldName").chosen({ width: '100%' });
    $("#YAxisFieldName").trigger("chosen:updated");
    $("#chartFilter").show();
}

function showTemplate() {
    if (kpi_temp_details != '') {

        $('#XAxisFieldName').val(kpi_temp_details.x_axis_column);
        $('#YAxisFieldName').trigger('change');
        $('#YAxisFieldName').val(kpi_temp_details.y_axis_columns.split(","));
        $("#YAxisFieldName").chosen({ width: '100%' });
        $("#YAxisFieldName").trigger("chosen:updated");
        $("#chartFilter").show();

        setTimeout(function () {
            $('.tabBtn ').removeClass('operations__tab--active');
            $('#chartview').addClass('operations__tab--active');
            $('.operations__content').removeClass('operations__content--active');
            $('.operations__tab--2').addClass('operations__content--active');
            $('#chartview').click();
            $('#btnDrawChart').click();

        }, 10);
        setTimeout(function () {
            $('#btnDrawChart').click();
            $('#btnExcelData').hide();
        }, 150);
    }
}

document.addEventListener("reloadstart", e => {
    $('#divloader').removeClass("dNone");
});


document.addEventListener("reloadend", e => {
    $('#divloader').addClass("dNone");
    if ($('.mvc-grid-table').find('tbody').find('tr').length > 0 && !$('.mvc-grid-table').find('tbody').find('tr').hasClass('mvc-grid-empty-row')) {
        $('#totalPages').show();
        $('#btnExcelData').css('display', 'block');
        showTemplate();
    }
    else {
        $('#totalPages').hide();
        $('#btnExcelData').css('display', 'none');
    }
});

document.addEventListener("reloadfail", e => {
    $('.mvc-grid-table').empty().html('<table><thead><tr class="mvc-grid-headers"></tr></thead><tbody><tr class="mvc-grid-empty-row"><td colspan="0">No data found</td></tr></tbody></table>');
    $('.mvc-grid-pager').remove();
    $('#btnExcelData').css('display', 'none');

});
function showExcelButton() {
    if ($('.mvc-grid-table').find('tbody').find('tr').length > 0 && (!$('.mvc-grid-table').find('tbody').find('tr').hasClass('mvc-grid-empty-row'))) {
        setTimeout(function () {
            if ($('#tableview').hasClass('operations__tab--active')) {
                $('#btnExcelData').css('display', 'block');
                return;
            }
        }, 10);
    }
    
        $('#btnExcelData').css('display', 'none');
   
}

function resetYAxisdata(selvalue) {
    if (selvalue == 'Select') {
        $("#YAxisFieldName").val('').trigger("chosen:updated");
    }
}
function fnDownloadKPIData() {
    window.location = "/kpi/KPIExcelDownload";
}

function btnModalOpener() {
    $('#btnSaveTemplatepp').toggleClass('showHide');
    $('.closeBox').click(function () {
        $('.modalPopup').removeClass('showHide')
    });
    //fnSaveTemplate();
}
function TemplateReload() {
    $('.modalPopup').removeClass('showHide')
    location.reload();
}
function BindTemplate() {
    $.get("/KPI/ListTemplate", null, function (response) {
        if (response.status == "OK") {
            let htmldata = '<input type="text" id="txtSearchKPI" class="form-control txtSearchKPI" onkeyup="filtterKPITemplate(this)" placeholder="Enter Template Name" style="margin:0.5rem 0.9rem;width:88%;"/>';
            response.data.forEach(function (item) {
                htmldata += `<li class="d-flex maxtab_li">
                                <a href="/kpi/index?template_id=${item.template_id}">${item.template_name}</a>
                                <img src='/images/delete.svg' class='edDel ml-auto' onclick="DeleteTemplate('${item.template_id}')"/>
                            </li >`
            });
            $('#grdKPItemplate').empty().html(htmldata);
        }
    });
}



function DeleteTemplate(template_id) {
    if (confirm("Are you sure, you want to delete this record.") == true) {
        $.get("/KPI/DeleteKPITemplate", { template_id: template_id }, function () {
            BindTemplate();
        });
    }
}


function pageMessageFromhtml(message, mtype=1) {
    let mclass = mtype == 1 ? "alert-danger" : "alert-success";
    $('#dvPageMessage').html(`
    <div class="alert ${mclass} " style="display:flex">
        <i class="fa fa-ban-circle"></i><strong>${message}</strong>
        <a href="javascript:void(0)" class="closeBox close ml-auto" data-dismiss="alert" onclick="ClosePageMessage();">&#128473;</a>
    </div>`);
    autoClosePageMessage();
}

function filtterKPITemplate(curobj) {
    $("ul.maxTab li").removeClass('d-flex').css('display', 'none');
    if ($(curobj).val().length > 0)
        $(`#grdKPItemplate > li:contains('${$(curobj).val()}')`).addClass('d-flex');
        else
        $("#grdKPItemplate > li").addClass('d-flex');
}

function autoClosePageMessage() {
    $(function () {
        $('div.alert-success, div.alert-danger').delay(3000).hide("slow");
    });
}
function ClosePageMessage() {
    $('div.alert-success, div.alert-danger').hide("slow");
}