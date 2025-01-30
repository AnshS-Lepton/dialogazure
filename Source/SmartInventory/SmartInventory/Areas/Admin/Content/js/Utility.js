function callFormValidator(formId) {

    $.validator.setDefaults({ ignore: ":hidden:not(select)" });
    // $.validator.unobtrusive.parse("#" + formId);

    var $validator = $("#" + formId).validate();
    var isCallErrorTab = false;

    $validator.settings.highlight = function (element, errorClass, validClass) {
        var $element = $(element);

        if ($element.hasClass("chosen-select")) {
            // It's a chosen element so move to the next element in the DOM 
            // which should be your container for chosen.  Add the error class to 
            // that instead of the hidden select
            $element.next().addClass(errorClass).removeClass(validClass);


        }
        else {
            $element.addClass(errorClass).removeClass(validClass);
        }
        if (!isCallErrorTab) {

            var isItemSpecDvError = $("#ItemSpecf div").hasClass('input-validation-error');
            var isItemSpecInError = $("#ItemSpecf input").hasClass('input-validation-error');
            var isGisInfoDvError = $("#GISInfo div").hasClass('input-validation-error');
            var isGisInfoInError = $("#GISInfo input").hasClass('input-validation-error');
            if (isGisInfoDvError || isGisInfoInError) {
                $($(".libTabs ul li a")[0]).trigger("click");
                isCallErrorTab = true;
            }
            else if (isItemSpecDvError || isItemSpecInError) {
                $($(".libTabs ul li a")[1]).trigger("click");
                isCallErrorTab = true;
            }
        }
    };
    // We're going to override the default unhighlight method from jQuery when removing
    // our css class for an error
    $validator.settings.unhighlight = function (element, errorClass, validClass) {
        var $element = $(element);
        if ($element.hasClass("chosen-select")) {
            // It's a chosen element so move to the next element in the DOM 
            // which should be your container for chosen.  Add the error class to 
            // that instead of the hidden select
            $element.next().removeClass(errorClass).addClass(validClass);
        } else {
            $element.removeClass(errorClass).addClass(validClass);
        }
        isCallErrorTab = false;
    };
}



function addDefaultSortingIcons(gridControlId) {
    // $('#' + gridControlId + '  tr th').addClass('bothorder'); // you can add both Icon Class here

    $('#' + gridControlId + '  tr th').each(function (i, e) {
        if ($(this).find("a").length == 1) { $(this).addClass('bothorder'); }
    });
}

// change sort icon asc or desc
// generic method for all grids
function onGridHeaderClick(gridControlId) {
    //$('#' + gridControlId + ' tr th').addClass('bothorder');
     
    addDefaultSortingIcons(gridControlId);
    var dir = $('#dir').val(); //direction value
    var col = $('#col').val(); // header value

    //var clickedheader = $('th a[href*=' + col + ']');
    //var countTh = document.getElementsByTagName('th').length; //total column header

    var clickedheader = $('#' + gridControlId + '  tr th a[href*="' + col + '"]');
    var countTh = $('#' + gridControlId + '  tr th').length; //total column header

    for (var i = 1; i <= countTh; i++) {
        var txtTh = $('#' + gridControlId + '  tr th:nth-child(' + i + ')').text(); // header text

        if (txtTh.trim().toLowerCase() == clickedheader.text().trim().toLowerCase() && dir == 'Ascending') {

            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').removeClass('bothorder');
            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').addClass('ascendingorder');
        }
        else if (txtTh.trim().toLowerCase() == clickedheader.text().trim().toLowerCase() && dir == 'Descending') {

            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').removeClass('bothorder');
            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').addClass('descendingorder');
        }
    }
}

function allowNumberwithOneDot(evt, obj) {
    var DotLength = $(obj).val().indexOf('.');
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode == 46) {
        if (DotLength >= 0)
            return false;
        else
            return true;
    }
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function allowNumberOnly(event, obj) {
    $(obj).val($(obj).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
    if (($(obj).val().length == 0 && event.which == 48)) {
        return false;
    }
}

function changeColor(tableId) {
    var tbl = $('#' + tableId);
    var rowCount = $('#' + tableId + ' tr').length;

    if (rowCount > 0) {
        for (var i = 0; i < tbl[0].rows.length; i++) {
            for (var j = 0; j < tbl[0].rows[i].cells.length; j++) {
                if (i > 1) {
                    if (tbl[0].rows[i].cells[j].innerHTML != tbl[0].rows[i - 1].cells[j].innerHTML) {
                        tbl[0].rows[i - 1].cells[j].style.background = "#E97786";
                    }
                }
            }
        }
    }
    $(tbl).find('tfoot td').removeAttr("style")
}
function getMaskedNetworkId(networkid, systemid, networktype) {

    var _mask = networkid;
    var _systemid = systemid;
    var _networktype = networktype;

    if (_networktype == 'M' && systemid == 0) {
        $("#txtMaskedNetworkId").dxTextBox({
            value: '',
            mask: _mask
        });
    }
    else {
        $("#txtMaskedNetworkId").dxTextBox({
            value: _mask,
            readOnly: true,
            hoverStateEnabled: false
        });
    }


}
function countChar(text, ch) {
    let count = 0;
    let len = text.length;
    for (let i = 0; i < len; i++) {
        if (text[i] === ch)
            count++;
    }
    return count;
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function isAuthenticate() {
    $.ajax({
        url: appRoot + "Login/checkSession", success: function (result) {
            if (result == false) {
                location.href = baseUrl + appRoot;
            }
        }
    });
}
function DateFormatter(datevalue) {
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    ////
    var date;
    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
    {
        date = datevalue.replace("-", " ");
    }
    else if (navigator.userAgent.indexOf("Firefox") > 0) {
        date = datevalue.replace(/-/g, '/');
    } else {
        date = datevalue;
    }
    var formatedDate = new Date(date);
    return formatedDate;
}
function fncCurrentDate(objDate) {
    // //
    var currentDate = new Date();
    if ((objDate == undefined) && (objDate == ''))
        objDate = currentDate;
    var formatedDate = GetFormattedDate(objDate);
    return formatedDate;
}
function GetFormattedDate(objDate) {
    if (objDate != undefined) {
        var month = objDate.getMonth() + 1;
        var day = objDate.getDate();
        var year = objDate.getFullYear();
        if (month < 10)
            month = '0' + month.toString();
        if (day < 10)
            day = '0' + day.toString();

        var monthName = new Array();
        monthName[0] = "Jan";
        monthName[1] = "Feb";
        monthName[2] = "Mar";
        monthName[3] = "Apr";
        monthName[4] = "May";
        monthName[5] = "Jun";
        monthName[6] = "Jul";
        monthName[7] = "Aug";
        monthName[8] = "Sep";
        monthName[9] = "Oct";
        monthName[10] = "Nov";
        monthName[11] = "Dec";

        var formatedDate = day + '-' + monthName[month - 1] + '-' + year;
        return formatedDate;
    }
    return '';
}

function ajaxReqforFileUpload(url, _data, isAsync, callback, isLoaderRequired) {
     
    if (isAsync === true) isasync = !0; else isAsync = !1;
    var ajaxParams = {
        type: "POST",
        timeout: 3600000,
        url: appRoot + url,
        async: isAsync,
        contentType: false,
        processData: false,
        error:
            function (a, b, c, d) {
                if (a.status === 401 || a.status === 570 || (a.status === 0 && b != "timeout"))
                    window.location.reload();
                if (isLoaderRequired) { hideProgress(); }
            },
        success: callback,
        complete: function (a, b) {
            if (b === "timeout") { alert("Request timeout reached."); }//Request timeout reached.
            if (isLoaderRequired) { hideProgress(); }
        },
        beforeSend: function () { if (isLoaderRequired) { showProgress(); } }
    };

    ajaxParams = $.extend(ajaxParams, { data: _data });
    $.ajax(ajaxParams);
}

function ValidateNumberDecimal(th, event, mxlen, decNum) {
    ////call>> onkeydown="return ValidateNumberDecimal(this,event,8,2);"
    decNum = (decNum == undefined || decNum == "" || decNum == null) ? 0 : decNum;
    var keyArr = [0, 8, 36, 37, 38, 39, 46, 190];
    var codes = event.keyCode || event.which;
    var charPos = getCurrentCursorIndex(th);
    var txtval = $(th).val();
    if ((txtval == "" && codes == 190) || (decNum == 0 && codes == 190) ||
        ((keyArr.indexOf(codes) == -1) && (codes < 48 || codes > 57))) {
        return false
    }
    else if ((keyArr.indexOf(codes) > 0)) {

        if ((txtval.indexOf('.') > 0 && codes == 190) || (codes == 190 && (txtval.length - charPos) > decNum)) {
            return false
        }
        setTimeout(function () {
            if ($(th).val().indexOf('.') == -1 && $(th).val().length > mxlen) {
                let str = $(th).val();
                $(th).val(str.substr(0, mxlen));
            }
        }, 10);
        if (txtval.indexOf('.') > 0) {
            setTimeout(function () {
                let str = $(th).val();
                if ($(th).val().indexOf('.') == 0) {
                    $(th).val(str.substr(1, str.length));
                }
                else if (str.substr(str.length - 1) == ".") {
                    $(th).val(str.substr(0, str.length - 1));
                }
            }, 15);
        }
        return true;
    }
    else {
        var dotPos = $(th).val().indexOf('.');
        if (dotPos < 0) {
            if (txtval.length < mxlen || codes == 46) {
                return true;
            }
            return false;
        }
        else {
            var number = th.value.split('.');
            var caratPos = getCurrentCursorIndex(th);
            if (txtval.length >= dotPos && txtval.length < dotPos + (decNum + 1) && codes != 46) {
                if (caratPos <= dotPos) {
                    if (number[0].length < mxlen) {
                        return true;
                    }
                    return false;
                }
                else if (caratPos > dotPos && dotPos > -1 && (number[1].length > (decNum))) {
                    return false;
                }
                return true;
            }
            if (number[0].length < mxlen && caratPos < dotPos + decNum) {
                return true;
            }
            else {
                return false;
            }
        }
    }
    function getCurrentCursorIndex(th) {
        if (th.createTextRange) {
            var r = document.selection.createRange().duplicate()
            r.moveEnd('character', th.value.length)
            if (r.text == '') return th.value.length
            return th.value.lastIndexOf(r.text)
        } else return th.selectionStart
    }  
}

function validateIntegerMinMaxValue(thisOfInput) {
    // onchange = "validateIntegerMinMaxValue(this)"; <input min="2" max="10" />
    var max = parseInt($(thisOfInput).attr('max'));
    var min = parseInt($(thisOfInput).attr('min'));
    if ($(thisOfInput).val() > max) {
        alert("Maximum " + max+" value allowed.");
        $(thisOfInput).val(max);
    }
    else if ($(thisOfInput).val() < min) {
        alert("Minimum " + min + " value allowed.");
        $(thisOfInput).val(min);
    }
}

function getMultilingualStringValue(p_key) {
    var elm = '<div></div>';
    var Mkey = $(elm).html($.parseHTML($.parseHTML(p_key)[0].textContent))[0].innerHTML;
    return Mkey;
}

function getFormattedNumber(number) {

    console.log("Format Type:" + $('#hdnNumberFormatType').val());
    if ($('#hdnNumberFormatType').val().toUpperCase() == "SAARC")
        return number.toString().split('.')[0].length > 3 ? number.toString().substring(0, number.toString().split('.')[0].length - 3).replace(/\B(?=(\d{2})+(?!\d))/g, ",") + "," + number.toString().substring(number.toString().split('.')[0].length - 3) : number.toString();
    else if ($('#hdnNumberFormatType').val().toUpperCase() == "EUROPE")
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function UpdateFormattedNumber(tblID, applyOnAllColumns, excludeColumns) {
    let applyOnSelectedColumns = false;
    if (applyOnAllColumns == null || applyOnAllColumns == undefined)
        applyOnAllColumns = true;
    if (excludeColumns == null || excludeColumns == undefined)
        applyOnAllColumns = true;

    const table = document.getElementById(tblID);
    if (table != null && table != undefined) {
        //Get Header Information of Tables==============================================================
        var table1 = document.getElementById(tblID);
        var headerRow = table1.querySelector("thead tr");
        var headers = headerRow.getElementsByTagName("th");
        var arrExcludeColumns = excludeColumns;//Sample Values ["JFP Total Capex", "Cost Per Unit (Rs)"];
        var headerIndex = [];
        var headerNames = [];
        for (var i = 0; i < headers.length; i++) {
            if ($.inArray(headers[i].textContent, arrExcludeColumns) !== -1) {
                headerIndex.push(i);
                headerNames.push(headers[i].textContent);
            }
        };

        // Loop through the rows
        for (let rowIndex = 0; rowIndex < table.rows.length; rowIndex++) {
            const row = table.rows[rowIndex];

            // Loop through the cells in the row
            for (let cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                //============In case we do  need to apply formatting to only selected columns  
                if (applyOnAllColumns == false && $.inArray(cellIndex, headerIndex) !== -1) {
                    continue;
                }

                const cell = row.cells[cellIndex];

                // Extract data from the cell
                const cellData = cell.textContent.trim();

                // Uncomment to see log data
                //console.log(`Data in Row ${rowIndex + 1}, Column ${cellIndex + 1}: ${cellData}`);
                if (cellData != null && cellData != "" && $.isNumeric(cellData))
                    cell.textContent = getFormattedNumber(cellData);
            }
        }
    }

}