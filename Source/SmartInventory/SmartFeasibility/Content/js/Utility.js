function callFormValidator(formId) {
    $.validator.setDefaults({ ignore: ":hidden:not(select)" });
    $.validator.unobtrusive.parse("#" + formId);

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

function onGridHeaderClick(gridControlId) {


    $('#' + gridControlId + ' tr th').addClass('bothorder');
    var dir = $('#dir').val(); //direction value
    var col = $('#col').val(); // header value

    var clickedheader = $('th a[href*="' + col + '"]');
    var countTh = document.getElementsByTagName('th').length; //total column header

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

function ajaxReq(url, _data, isAsync, callback, is_request_JSON, isLoaderRequired, is_response_JSON) {
   // isAuthenticate();
    if (isAsync === true) isasync = !0; else isAsync = !1;
    if (is_response_JSON == undefined) is_response_JSON = true;
    var ajaxParams = {
        type: "POST", timeout: 3600000, url: appRoot + url, async: isAsync, error: function (a, b, c, d) {
            if (a.status === 401 || a.status === 570 || (a.status === 0 && b != "timeout"))
                window.location.reload();
            if (isLoaderRequired) { hideProgress(); }
        }, success: callback,
        complete: function (a, b) {
            if (b === "timeout") { alert('Request timeout reached.'); }
            if (isLoaderRequired) { hideProgress(); }
        },
        beforeSend: function () { if (isLoaderRequired) { showProgress(); } }
    };
    if (is_request_JSON === true) {
        ajaxParams = $.extend(ajaxParams, { contentType: "application/json; charset=utf-8" });
        // changed on : 24 July-2018
        if (is_response_JSON) { ajaxParams = $.extend(ajaxParams, { dataType: 'json' }); }

        _data = JSON.stringify(_data);
    }
    ajaxParams = $.extend(ajaxParams, { data: _data });
    $.ajax(ajaxParams);
}

function ajaxReqforFileUpload(url, _data, isAsync, callback, isLoaderRequired) {
    if (isAsync === true) isasync = !0; else isAsync = !1;
    var ajaxParams = {
        type: "POST",
        timeout: 550000,
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
            if (b === "timeout") { alert('Request timeout reached.'); }
            if (isLoaderRequired)
            { hideProgress(); }
        },
        beforeSend: function ()
        { if (isLoaderRequired) { showProgress(); } }
    };

    ajaxParams = $.extend(ajaxParams, { data: _data });
    $.ajax(ajaxParams);
}
function showProgress() {
    $("#dvProgress").show();
}
function hideProgress() {
    $("#dvProgress").hide();
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