var isMDUCall = false;
function ajaxReq(url, _data, isAsync, callback, is_request_JSON, isLoaderRequired, is_response_JSON) {
    isAuthenticate();
    if (isAsync === true) isasync = !0; else isAsync = !1;
    if (is_response_JSON == undefined) is_response_JSON = true;
    var ajaxParams = {
        type: "POST", timeout: 3600000, url: appRoot + url, async: isAsync, error: function (a, b, c, d) {
            if (a.status === 401 || a.status === 570 || (a.status === 0 && b != "timeout"))
                window.location.reload();
            if (isLoaderRequired) { hideProgress();  }
        }, success: callback,
        complete: function (a, b) {
            if (b === "timeout") { alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_138); }//Request timeout reached.
            if (isLoaderRequired) { hideProgress(); }
        },
        beforeSend: function () { if (isLoaderRequired) { showProgress();  } }
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
            if (b === "timeout") { alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_138); }//Request timeout reached.
            if (isLoaderRequired) { hideProgress(); }
        },
        beforeSend: function () { if (isLoaderRequired) { showProgress(); } }
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

function animateLine(_lineObj, offset) {
    if (_lineObj) {
        if (_lineObj.map) {
            var icons = _lineObj.get('icons');
            if (icons) {
                if (offset == 5)
                    offset = 0;
                else
                    offset++;
                icons[0].offset = offset + 'px';
                _lineObj.set('icons', icons);
                setTimeout(function () { animateLine(_lineObj, offset) }, 50);
            }
        }
    }
}

function getMeterDistanceFromZoom(_zoom) {
    var mtrBuffer = 50;
    switch (_zoom) {
        case 16:
            mtrBuffer = 50;
            break;
        case 17:
            mtrBuffer = 25;
            break;
        case 18:
            mtrBuffer = 15;
            break;
        case 19:
            mtrBuffer = 10;
            break;
        case 20:
            mtrBuffer = 5;
            break;
        case 21:
            mtrBuffer = 2;
            break;
        case 22:
            mtrBuffer = 2;
            break;
    }
    return mtrBuffer;
}
function roundNumber(num, dec) {
    var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec) + '';
    var r = result.indexOf('.');
    return result;
}

function ClosePageMessage() {
    $('div.alert-success, div.alert-danger').delay(4000).hide("slow");
}
function ShowPageMessage() {
    $('div.alert-success, div.alert-danger').show("slow");
}

function toTitleCase(str) {
    return str.replace(/\w+/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
}

function attachUnAttachEvt(elemnt, Evt, func) {
    elemnt.unbind(Evt);
    elemnt.bind(Evt, func);
}

function getPopUpModelClass(eType) {
     
    var modelClass = 'modal-lg';
    switch (eType) {
        case "Building":
            modelClass = 'modal-xl';
            break;
        case "Area":
        case "RestrictedArea":
            modelClass = 'modal-md';
            break;
        case "DSA":
            modelClass = 'modal-md';
            break;
        case "CSA":
            modelClass = 'modal-md';
            break;
        case "SubArea":
            modelClass = 'modal-md';
            break;
        case "SurveyArea":
            modelClass = 'modal-md';
            break;
        case "Structure":
            modelClass = 'modal-lg';
            break;
        case "TerminationPoint":
            modelClass = 'modal-sm';
            break;
        case "Floor":
            modelClass = 'modal-sm';
            break;
        case "TerminationPoint":
            modelClass = 'modal-sm';
            break;
        case "ParallelCable":
            modelClass = 'modal-sm';
            break;
        case "EntityAssociate":
            modelClass = 'modal-sm';
            break;
        case "ProjectArea":
            modelClass = 'modal-sm';
            break;
        case "ROW":
            modelClass = 'modal-xl';
            break;
        case "LogicalView":
            modelClass = 'modal-xl';
            break;
        case "Competitor":
            modelClass = 'modal-md';
            break;
        case "LandBaseLayer":
            modelClass = 'modal-xl';
            break;
        case "Loop":
            modelClass = 'modal-md';
            break;
    }
    return modelClass;
}


function getTemplatePopUpModelClass(eType, pEntityType) {
    var modelClass = 'modal-lg';
    switch (eType) {
        case "ROW":
            modelClass = 'modal-sm';
            break;
        case "PIT":
            modelClass = 'modal-sm';
            break;

    }
    return modelClass;
}

//function getMultilingualKeyForEntity(entityType)
//{
//    var Key = entityType;
//    entityType = entityType.toUpperCase();
//    switch (entityType) {
//        case "MANHOLE":
//            Key = MultilingualKey.Manhole;
//            break;
//        case "SPLICE CLOSURE":
//            Key = MultilingualKey.Splice_Closure;
//            break;
//        case "SPLITTER":
//            Key = MultilingualKey.Splitter;
//            break;
//        case "AREA":
//            Key = MultilingualKey.Area;
//            break;
//        case "SURVEYAREA":
//            Key = MultilingualKey.SurveyArea;
//            break;
//        case "SUBAREA":
//            Key = MultilingualKey.SubArea;
//            break;
//        case "DSA":
//            Key = MultilingualKey.Dsa;
//            break;
//        case "CSA":
//            Key = MultilingualKey.Csa;
//            break;
//        case "BUILDING":
//            Key = MultilingualKey.Building;
//            break;
//        case "ROW":
//            Key = MultilingualKey.Row;
//            break;
//        case "CUSTOMER":
//            Key = MultilingualKey.Customer;
//            break;
//        case "HTB":
//            Key = MultilingualKey.HTB;
//            break;
//        case "PROJECT AREA":
//            Key = MultilingualKey.Project_Area;
//            break;
//        case "CABLE":
//            Key = MultilingualKey.Cable;
//            break;
//        case "TRENCH":
//            Key = MultilingualKey.Trench;
//            break;
//        case "ADB":
//            Key = MultilingualKey.Adb;
//            break;
//        case "POLE":
//            Key = MultilingualKey.Pole;
//            break;
//        case "ONT":
//            Key = MultilingualKey.Ont;
//            break;
//        case "COUPLER":
//            Key = MultilingualKey.Coupler;
//            break;
//        case "MPOD":
//            Key = MultilingualKey.Mpod;
//            break;
//        case "CDB":
//            Key = MultilingualKey.Cdb;
//            break;
//        case "DUCT":
//            Key = MultilingualKey.Duct;
//            break;
//        case "POP":
//            Key = MultilingualKey.Pop;
//            break;
//        case "TREE":
//            Key = MultilingualKey.Tree;
//            break
//        case "WALL MOUNT":
//            Key = MultilingualKey.Wall_Mount;
//            break;
//        case "PATCH CORD":
//            Key = MultilingualKey.Patch_Cord;
//            break;
//        case "Recurring Charges":
//            Key = MultilingualKey.Recurring_Charges;
//            break;
//        case "Maintenance Charges":
//            Key = MultilingualKey.Maintenance_Charges;
//            break;
//        case "FMS LOGICAL VIEW":
//            Key = MultilingualKey.FMS_Logical_View;
//            break;
//        case "ONT LOGICAL VIEW":
//            Key = MultilingualKey.ONT_Logical_View;
//            break;
//        case "SPLICE CLOSURE TEMPLATE":
//            Key = MultilingualKey.Splice_Closure_Template;
//            break;
//        case "PATCH CORD TEMPLATE":
//            Key = MultilingualKey.patch_card_template;
//            break;
//        case "TEMPLATE":
//            Key = MultilingualKey.Template;
//            break;
//        case "ASSOCIATION DETAILS":
//            Key = MultilingualKey.Association_Details;
//            break;
//        case "WITHIN":
//            Key = MultilingualKey.Within;
//            break;
//        case "MTR":
//            Key = MultilingualKey.Mtr;
//            break;
//        case "STRUCTURE":
//            Key = MultilingualKey.Structure;
//            break;       
//    }
//    return Key;
//}

function getMultilingualKeyForLoadModalDialog(titleText) {
    // 
    var Key = titleText;
    titleText = titleText.toUpperCase();
    switch (titleText) {
        case "UPLOAD USER PROFILE":
            Key = MultilingualKey.SI_OSP_UPP_JQ_FRM_001;
            break;
        case "SPLICING":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_019;
            break;
        case "MANHOLE":
            Key = MultilingualKey.SI_OSP_MH_JQ_GBL_001;
            break;
        case "SPLICE CLOSURE":
            Key = MultilingualKey.SI_OSP_SC_JQ_FRM_003;
            break;
        case "SPLITTER":
            Key = MultilingualKey.SI_OSP_SPL_JQ_FRM_001;
            break;
        case "AREA":
            Key = MultilingualKey.SI_OSP_GBL_GBL_GBL_013;
            break;
        case "SURVEYAREA":
            Key = MultilingualKey.SI_OSP_SVA_JQ_FRM_006;
            break;
        case "SUBAREA":
            Key = MultilingualKey.SI_OSP_SBA_JQ_FRM_004;
            break;
        case "DSA":
            Key = MultilingualKey.SI_OSP_DSA_JQ_FRM_004;
            break;
        case "CSA":
            Key = MultilingualKey.SI_OSP_CSA_JQ_FRM_001;
            break;
        case "BUILDING":
            Key = MultilingualKey.SI_OSP_BUL_JQ_FRM_010;
            break;
        case "ROW":
            Key = MultilingualKey.SI_OSP_ROW_JQ_FRM_007;
            break;
        case "CUSTOMER":
            Key = MultilingualKey.SI_OSP_CUS_JQ_FRM_002;
            break;
        case "HTB":
            Key = MultilingualKey.SI_OSP_HTB_JQ_FRM_001;
            break;
        case "PROJECT AREA":
            Key = MultilingualKey.SI_OSP_PA_JQ_FRM_001;
            break;
        case "CABLE":
            Key = MultilingualKey.SI_OSP_CAB_JQ_FRM_004;
            break;
        case "TRENCH":
            Key = MultilingualKey.SI_OSP_TCH_JQ_FRM_001;
            break;
        case "ADB":
            Key = MultilingualKey.SI_OSP_ADB_JQ_FRM_002;
            break;
        case "POLE":
            Key = MultilingualKey.SI_OSP_POL_JQ_FRM_001;
            break;
        case "ONT":
            Key = MultilingualKey.SI_OSP_ONT_JQ_FRM_001;
            break;
        case "COUPLER":
            Key = MultilingualKey.SI_OSP_CPR_JQ_FRM_001;
            break;
        case "MPOD":
            Key = MultilingualKey.SI_OSP_MPOD_JQ_FRM_001;
            break;
        case "CDB":
            Key = MultilingualKey.SI_OSP_CDB_JQ_FRM_004;
            break;
        case "DUCT":
            Key = MultilingualKey.SI_OSP_DUC_JQ_FRM_001;
            break;
        case "POP":
            Key = MultilingualKey.SI_OSP_POP_JQ_FRM_001;
            break;
        case "TREE":
            Key = MultilingualKey.SI_OSP_TRE_JQ_FRM_001;
            break
        case "WALL MOUNT":
            Key = MultilingualKey.SI_OSP_WMT_JQ_FRM_001;
            break;
        case "PATCH CORD":
            Key = MultilingualKey.SI_OSP_PCD_JQ_FRM_002;
            break;
        case "RECURRING CHARGES":
            Key = MultilingualKey.SI_OSP_ROW_JQ_FRM_008;
            break;
        case "MAINTENANCE CHARGES":
            Key = MultilingualKey.SI_OSP_ROW_JQ_FRM_009;
            break;
        case "FMS LOGICAL VIEW":
            Key = MultilingualKey.SI_OSP_FMS_JQ_FRM_002;
            break;
        case "ONT LOGICAL VIEW":
            Key = MultilingualKey.SI_OSP_ONT_JQ_FRM_002;
            break;
        case "SPLICE CLOSURE TEMPLATE":
            Key = MultilingualKey.SI_OSP_SC_JQ_FRM_004;
            break;
        case "PATCH CORD TEMPLATE":
            Key = MultilingualKey.SI_OSP_PCD_JQ_FRM_001;
            break;
        case "TEMPLATE":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_001;
            break;
        case "ASSOCIATION DETAILS":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_001;
            break;
        case "WITHIN":
            Key = MultilingualKey.SI_OSP_GBL_GBL_GBL_039;
            break;
        case "MTR":
            Key = MultilingualKey.SI_OSP_GBL_GBL_GBL_034;
            break;
        case "STRUCTURE":
            Key = MultilingualKey.SI_OSP_STR_JQ_FRM_011;
            break;
        case "CONNECTION PATH FINDER":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_090;
            break;
        case "SURVEY BUILDING":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_098;
            break;
        case "SURVEY ASSIGNMENT":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_022;
            break;
        case "BOM/BOQ REPORT":
            Key = MultilingualKey.SI_OSP_GBL_JQ_RPT_004;
            break;
        case "ROW REPORT":
            Key = MultilingualKey.SI_OSP_GBL_JQ_RPT_005;
            break;
        case "TICKET MANAGER":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_099;
            break;
        case "LMC REPORT":
            Key = MultilingualKey.SI_OSP_GBL_JQ_RPT_013;
            break;
        case "INFORMATION":
            Key = MultilingualKey.GBL_GBL_GBL_JQ_GBL_001;
            break;
        case "ADVANCE FILTERS":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_100;
            break;
        case "PRINT MAP":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_101;
            break;
        case "BUILDING STATUS HISTORY":
            Key = MultilingualKey.SI_OSP_BUL_JQ_FRM_011;
            break;
        case "IMPORT REGION PROVINCE":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_102;
            break;
        case "AREA HISTORY":
            Key = MultilingualKey.SI_OSP_AR_JQ_HIS_001;
            break;
        case "DSA HISTORY":
            Key = MultilingualKey.SI_OSP_DSA_JQ_HIS_001;
            break;
        case "HTB HISTORY":
            Key = MultilingualKey.SI_OSP_HTB_JQ_HIS_001;
            break;
        case "WALL MOUNT HISTORY":
            Key = MultilingualKey.SI_OSP_WMT_JQ_HIS_001;
            break;
        case "SUBAREA HISTORY":
            Key = MultilingualKey.SI_OSP_SBA_JQ_HIS_001;
            break;
        case "COUPLER HISTORY":
            Key = MultilingualKey.SI_OSP_CPR_JQ_HIS_001;
            break;
        case "TREE HISTORY":
            Key = MultilingualKey.SI_OSP_TRE_JQ_HIS_001;
            break;
        case "MPOD HISTORY":
            Key = MultilingualKey.SI_OSP_MPOD_JQ_GBL_001;
            break;
        case "PROJECTAREA HISTORY":
            Key = MultilingualKey.SI_OSP_PA_JQ_HIS_001;
            break;
        case "CSA HISTORY":
            Key = MultilingualKey.SI_OSP_CSA_JQ_HIS_001;
            break;
        case "POP HISTORY":
            Key = MultilingualKey.SI_OSP_POP_JQ_HIS_001;
            break;
        case "TRENCH HISTORY":
            Key = MultilingualKey.SI_OSP_TCH_JQ_HIS_001;
            break;
        case "CUSTOMER HISTORY":
            Key = MultilingualKey.SI_OSP_CUS_JQ_HIS_001;
            break;
        case "BUILDING HISTORY":
            Key = MultilingualKey.SI_OSP_BUL_JQ_HIS_001;
            break;
        case "DUCT HISTORY":
            Key = MultilingualKey.SI_OSP_DUC_JQ_HIS_001;
            break;
        case "MANHOLE HISTORY":
            Key = MultilingualKey.SI_OSP_MH_JQ_HIS_001;
            break;
        case "POLE HISTORY":
            Key = MultilingualKey.SI_OSP_POL_JQ_HIS_001;
            break;
        case "ROW HISTORY":
            Key = MultilingualKey.SI_OSP_ROW_JQ_HIS_001;
            break;
        case "CDB HISTORY":
            Key = MultilingualKey.SI_OSP_CDB_JQ_HIS_001;
            break;

        case "ADB HISTORY":
            Key = MultilingualKey.SI_OSP_ADB_JQ_FRM_004;
            break;
        case "ONT HISTORY":
            Key = MultilingualKey.SI_OSP_ONT_JQ_HIS_001;
            break;
        case "SPLICE CLOSURE HISTORY":
            Key = MultilingualKey.SI_OSP_SC_JQ_HIS_001;
            break;
        case "CABLE HISTORY":
            Key = MultilingualKey.SI_OSP_CAB_JQ_HIS_001;
            break;
        case "UPLOAD DATA":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_103;
            break;
        case "BULK OPERATION":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_106;
            break;
        case "PROJECT SPECFICATION":
            Key = MultilingualKey.SI_OSP_GBL_GBL_FRM_001;
            break;
        case "FMS HISTORY":
            Key = MultilingualKey.SI_OSP_FMS_JQ_FRM_001;
            break;
        case "PROJECT AREA HISTORY":
            Key = MultilingualKey.SI_OSP_PA_JQ_HIS_002;
            break;
        case "SURVEYAREA HISTORY":
            Key = MultilingualKey.SI_OSP_SVA_JQ_HIS_001;
            break;
        case "BULK APPROVE BUILDING?":
            Key = MultilingualKey.SI_OSP_BUL_JQ_FRM_006;
            break;
        case "BULK REJECT BUILDING?":
            Key = MultilingualKey.SI_OSP_BUL_JQ_FRM_007;
            break;
        case "USER ASSIGNMENT":
            Key = MultilingualKey.SI_OSP_BUL_JQ_FRM_009;
            break;
        case "FDC":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_087;
            break;
        case "FDC HISTORY":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_088;
            break;
        case "FAT HISTORY":
            Key = MultilingualKey.SI_OSP_GBL_JQ_HIS_002;
            break;
        case "STRUCTURE HISTORY":
            Key = MultilingualKey.SI_OSP_STR_JQ_HIS_001;
            break;

        case "FAT":
            Key = MultilingualKey.SI_OSP_GBL_JQ_FRM_089;
            break;
        case "HTB LOGICAL VIEW":
            Key = MultilingualKey.SI_OSP_HTB_JQ_FRM_002;
            break;
        case "FMS TEMPLATE":
            Key = MultilingualKey.SI_OSP_FMS_JQ_FRM_003;
            break;
        case "FDB TEMPLATE":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_004;
            break;
        case "UTILIZATION REPORT":
            Key = MultilingualKey.SI_GBL_GBL_GBL_GBL_106;
            break;
        case "VIEW REPORT":
            Key = MultilingualKey.SI_OSP_GBL_GBL_GBL_042;
            break;
        case "FMS":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_008;
            break;
        case "UPLOAD IMAGE/DOCUMENT":
            Key = MultilingualKey.SI_GBL_GBL_GBL_GBL_132;
            break;
        case "UPDATE PORT STATUS":
            Key = MultilingualKey.SI_GBL_GBL_GBL_GBL_133;
            break;
        case "BDB HISTORY":
            Key = MultilingualKey.SI_GBL_GBL_GBL_FRM_014;
            break;
        case "IMPORT DATA":
            Key = MultilingualKey.SI_GBL_GBL_JQ_GBL_002;
            break;
        case "CABLE TO CABLE":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_017;
            break;
        case "APPROVE BUILDING?":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_053;
            break;
        case "REJECT BUILDING?":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_054;
            break;
        case "SPLIT CABLE DETAILS":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_019;
            break;
        case "LOSS DETAIL":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_027;
            break;
        case "CABLE TEMPLATE":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_029;
            break;
        case "POD HISTORY":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_033;
            break;
        case "FDB HISTORY":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_034;
            break;
        case "UNIT HISTORY":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_035;
            break;
        case "ODF/FMS TO CABLE":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_048;
            break;
        case "UPDATE CORE STATUS":
            Key = MultilingualKey.SI_GBL_GBL_GBL_GBL_130;
            break;
        case "SHAFT DETAIL":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_057;
            break;
        case "FLOOR DETAIL":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_058;
            break;
        case "MANUAL SPLICING":
            Key = MultilingualKey.SI_OSP_GBL_NET_GBL_043;
            break;
        case "LMC HISTORY":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_066;
            break;
        case "HELP & FAQ,S":
            Key = MultilingualKey.SI_GBL_GBL_JQ_FRM_068;
            break;
        case "CONNECTION EDITOR":
            Key = MultilingualKey.SI_ISP_GBL_NET_FRM_036;
            break;
        case "LAND BASE LAYER":
            Key = MultilingualKey.SI_ISP_GBL_NET_FRM_036;
            break
        case "BDB TEMPLATE":
            Key = MultilingualKey.SI_GBL_GBL_GBL_FRM_010;
            break;
    }
    return Key;
}
function getLayerDetail(lyrName) {
     
    var lyrDetail = '';
    ajaxReq('main/GetLayerDetail', { layerName: lyrName }, false, function (resp) {
        if (resp.status = 'OK' && resp.result != null && resp.result != undefined) {
            lyrDetail = resp.result;
             
            if (si != null) {
                var urlS = lyrDetail.layer_name.toUpperCase() == "NETWORK_TICKET" ? "NetworkTicket" : "Library";
                lyrDetail.layer_form_url = urlS + lyrDetail.layer_form_url;
                lyrDetail.save_entity_url = urlS + lyrDetail.save_entity_url;
                if (lyrName == 'Antenna' || lyrName == 'Tower' || lyrName == 'Sector' || lyrName == 'MicrowaveLink') {
                    lyrDetail.layer_form_url = lyrName + "/Add" + lyrName;
                    lyrDetail.save_entity_url = lyrName + "/Save" + lyrName;
                }


            }
            else if (isp != null) {
                lyrDetail.layer_form_url = 'ISP' + lyrDetail.layer_form_url;
                lyrDetail.save_entity_url = 'ISP' + lyrDetail.save_entity_url;
            }
        }
    }, true, false);
    return lyrDetail;
}

function getLayerMapping(lyrName) {
    var lyrMapping = '';
    ajaxReq('main/GetLayerMapping', { layerName: lyrName }, false, function (resp) {
        if (resp.status = 'OK' && resp.result != null && resp.result != undefined) {
            lyrMapping = resp.result;
        }
    }, true, false);
    return lyrMapping;
}
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

            //region additional-attributes
            var isOtherInfoDvError = $("#frmOtherInfo  div").hasClass('input-validation-error');
            var isOtherInfoInError = $("#frmOtherInfo  input").hasClass('input-validation-error');

            if (isGisInfoDvError || isGisInfoInError) {
                $($(".libTabs ul li a")[0]).trigger("click");
                isCallErrorTab = true;
            }
            else if (isItemSpecDvError || isItemSpecInError) {
                $($(".libTabs ul li a")[1]).trigger("click");
                isCallErrorTab = true;
            }
            else if (isOtherInfoDvError || isOtherInfoInError) {
                $($(".libTabs ul li a[href=#AdditionalAttributes]")[0]).trigger('click');
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

function hideDiv(elementId) {
    $(elementId).fadeOut();
}

function showDiv(elementId) {
    $(elementId).fadeIn();
}

function getMaskedNetworkId(networkid, systemid, networktype) {
    var _mask = $("<div/>").html(networkid).text();
    var _systemid = systemid;
    var _networktype = networktype;

    if (_networktype == 'M' && systemid == 0) {
        $("#txtMaskedNetworkId").dxTextBox({
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

function AddReferenceRow(point, RowIndex, SystemId, CreatedBy, entitytype) {
    var Row = "";
    if (RowIndex < 10) {
        if (point == "PointA") {
            Row = '<div class="row form-group mrgn_btm reference_row_PointA"><div class="col-md-4"><input name="EntityReference.listPointAReference[' + RowIndex + '].landmark" class="form-control" id="listPointAReference_' + RowIndex + '__landmark"  onclick = "removeBorder(this)" maxlength="200" placeholder="' + MultilingualKey.SI_OSP_GBL_GBL_GBL_101 + '" type="text" value=""></div>';
            Row += '<div class="col-md-4"><input name="EntityReference.listPointAReference[' + RowIndex + '].distance" class="form-control numbers-only" data-val="true" data-val-number="The field distance must be a number." id="listPointAReference_' + RowIndex + '__distance" maxlength="5"  onclick = "removeBorder(this)" onkeypress="return allowNumberwithOneDot(event,\'listPointAReference_' + RowIndex + '__distance\');" placeholder="' + MultilingualKey.SI_OSP_GBL_GBL_GBL_102 + '" type="text" value=""></div>';
            Row += '<div class="col-md-4"><select id="ddl_direction" name="EntityReference.listPointAReference[' + RowIndex + '].direction" class="chosen-select form-control"  onchange = "removeBorder(this)" ><option value="0">-Select-</option>';
            Row += '<option value="East" data-planningname="East">East</option><option value="West" data-planningname="West">West</option><option value="North" data-planningname="North">North</option><option value="South" data-planningname="South">South</option>';
            Row += '<option value="North-West" data-planningname="North-West">North-West</option><option value="North-East" data-planningname="North-East">North-East</option><option value="South-West" data-planningname="South-West">South-West</option><option value="South-East" data-planningname="South-East">South-East</option></select></div>';

            Row += '<input name="EntityReference.listPointAReference[' + RowIndex + '].id" data-val="true" data-val-number="The field id must be a number." data-val-required="The id field is required." id="listPointAReference_' + RowIndex + '__id" type="hidden" value="0">';
            Row += '<input name="EntityReference.listPointAReference[' + RowIndex + '].created_by" data-val="true" data-val-number="The field created_by must be a number." data-val-required="The created_by field is required." id="listPointAReference_' + RowIndex + '__created_by" type="hidden" value="' + CreatedBy + '">';
            Row += '<input name="EntityReference.listPointAReference[' + RowIndex + '].system_id" data-val="true" data-val-number="The field system_id must be a number." data-val-required="The system_id field is required." id="listPointAReference_' + RowIndex + '__system_id" type="hidden" value="' + SystemId + '">';
            Row += '<input name="EntityReference.listPointAReference[' + RowIndex + '].entity_type" id="listPointAReference_' + RowIndex + '__entity_type" type="hidden" value="' + entitytype + '">';
            Row += '<input name="EntityReference.listPointAReference[' + RowIndex + '].entry_point" id="listPointAReference_' + RowIndex + '__entry_point" type="hidden" value="PointA">';
            Row += '<input name="EntityReference.listPointAReference[' + RowIndex + '].modified_on" data-val="true" data-val-date="The field modified_on must be a date." id="listPointAReference_' + RowIndex + '__modified_on" type="hidden" value="">';
            Row += '<input name="EntityReference.listPointAReference[' + RowIndex + '].modified_by" data-val="true" data-val-number="The field modified_by must be a number." data-val-required="The modified_by field is required." id="listPointAReference_' + RowIndex + '__modified_by" type="hidden" value="0"> ';
        }
        else {
            Row = '<div class="row form-group mrgn_btm reference_row_PointB"><div class="col-md-4"><input name="EntityReference.listPointBReference[' + RowIndex + '].landmark" class="form-control" id="listPointBReference_' + RowIndex + '__landmark"  onclick = "removeBorder(this)"maxlength="200" placeholder="' + MultilingualKey.SI_OSP_GBL_GBL_GBL_101 + '" type="text" value=""></div>';
            Row += '<div class="col-md-4"><input name="EntityReference.listPointBReference[' + RowIndex + '].distance" class="form-control numbers-only" data-val="true" data-val-number="The field distance must be a number." id="listPointBReference_' + RowIndex + '__distance" maxlength="5" onclick = "removeBorder(this)" onkeypress="return allowNumberwithOneDot(event,\'listPointAReference_' + RowIndex + '__distance\');" placeholder="' + MultilingualKey.SI_OSP_GBL_GBL_GBL_102 + '" type="text" value=""></div>';
            Row += '<div class="col-md-4"><select id="ddl_direction" name="EntityReference.listPointBReference[' + RowIndex + '].direction" class="chosen-select form-control"  onchange = "removeBorder(this)" ><option value="0">-Select-</option>';
            Row += '<option value="East" data-planningname="East">East</option><option value="West" data-planningname="West">West</option><option value="North" data-planningname="North">North</option><option value="South" data-planningname="South">South</option>';
            Row += '<option value="North-West" data-planningname="North-West">North-West</option><option value="North-East" data-planningname="North-East">North-East</option><option value="South-West" data-planningname="South-West">South-West</option><option value="South-East" data-planningname="South-East">South-East</option></select></div>';

            Row += '<input name="EntityReference.listPointBReference[' + RowIndex + '].id" data-val="true" data-val-number="The field id must be a number." data-val-required="The id field is required." id="listPointBReference_' + RowIndex + '__id" type="hidden" value="0">';
            Row += '<input name="EntityReference.listPointBReference[' + RowIndex + '].created_by" data-val="true" data-val-number="The field created_by must be a number." data-val-required="The created_by field is required." id="listPointBReference_' + RowIndex + '__created_by" type="hidden" value="' + CreatedBy + '">';
            Row += '<input name="EntityReference.listPointBReference[' + RowIndex + '].system_id" data-val="true" data-val-number="The field system_id must be a number." data-val-required="The system_id field is required." id="listPointBReference_' + RowIndex + '__system_id" type="hidden" value="' + SystemId + '">';
            Row += '<input name="EntityReference.listPointBReference[' + RowIndex + '].entity_type" id="listPointBReference_' + RowIndex + '__entity_type" type="hidden" value="' + entitytype + '">';
            Row += '<input name="EntityReference.listPointBReference[' + RowIndex + '].entry_point" id="listPointBReference_' + RowIndex + '__entry_point" type="hidden" value="PointB">';
            Row += '<input name="EntityReference.listPointBReference[' + RowIndex + '].modified_on" data-val="true" data-val-date="The field modified_on must be a date." id="listPointBReference_' + RowIndex + '__modified_on" type="hidden" value="">';
            Row += '<input name="EntityReference.listPointBReference[' + RowIndex + '].modified_by" data-val="true" data-val-number="The field modified_by must be a number." data-val-required="The modified_by field is required." id="listPointBReference_' + RowIndex + '__modified_by" type="hidden" value="0"> ';
        }

        if (point == "PointA")
            $('#ReferencePointA').append(Row)
        else if (point == "PointB")
            $('#ReferencePointB').append(Row)
        $('.chosen-select').chosen({ width: '100%' });
    }
    else
        alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_074);//Only 10 Row can be added !!

}



function AddNewMaintainenceChargesDiv(DivIndex, SystemId, CreatedBy, entitytype) {


    var Div = "";
    if (DivIndex <= 9) {

        Div = '<div class="dvDynamicForm EMC_' + DivIndex + ' len">';
        Div += '<hr/>';
        Div += '<span class="icon-close removeAT" onclick="DeleteMaintainenceDiv(' + DivIndex + ')" title="Delete" style="float:right;"></span>';
        Div += '<div class="form-group clearfix">';
        Div += '<label class="control-label col-sm-2" for="ddltype_of_activity_charge">Activity Charge Type:</label>';
        Div += '<div class="col-sm-45">';
        Div += '<select id="ddl_type_of_activity_charge_' + DivIndex + '_id" name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].type_of_activity_charge" class="chosen-select form-group" onchange="removeBorder(this)"><option value="0">-Select-</option><option value="Hardware" data-planningname="Hardware">Hardware</option><option value="Service Charge" data-planningname="Service Charge">Service Charge</option></select>'
        Div += '</div>'
        Div += '<label class="control-label col-sm-2" for="ddlcharge_category">Charge Category:</label>'
        Div += '<div class="col-sm-45">';
        Div += '<select id="ddl_charge_category_' + DivIndex + '_id" name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].charge_category" class="chosen-select form-group" onchange="removeBorder(this)"><option value="0">-Select-</option><option value="NW Oprn" data-planningname="NW Oprn">NW Oprn</option><option value="Recurring Charge" data-planningname="Recurring Charge">Recurring Charge</option></select>'
        Div += '</div>';
        Div += '</div>'; //form-group div closed

        Div += '<div class="form-group clearfix">';
        Div += '<label class="control-label col-sm-2" for="txtActivityStartDate">Activity Start Date:</label>';
        Div += '<div class="col-sm-45">';
        Div += '<input name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].activity_start_date" class="form-control clsReadOnly" id="listEntityMaintainenceChargesRecords__' + DivIndex + '_activity_start_date" placeholder="DD-MMM-YYYY" readonly="True" style="background:#fff;" type="text" value="' + GetFormattedDate(new Date()) + '"><img id="imgActivityStartDate_' + DivIndex + '" class="assign-calen" src="/Content/images/calendar.png" style="margin-top:1px;">';
        Div += '</div>';
        Div += '<label class="control-label col-sm-2" for="txtActivityEndDate">Activity End Date:</label>';
        Div += '<div class="col-sm-45">';
        Div += '<input name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].activity_end_date" class="form-control clsReadOnly" id="listEntityMaintainenceChargesRecords__' + DivIndex + '_activity_end_date" placeholder="DD-MMM-YYYY" readonly="True" style="background:#fff;" type="text" value="' + GetFormattedDate(new Date()) + '"><img id="imgActivityEndDate_' + DivIndex + '" class="assign-calen" src="/Content/images/calendar.png" style="margin-top:1px;">';
        Div += '</div>';
        Div += '</div>'; //form-group div closed

        Div += '<div class="form-group clearfix">';
        Div += '<label class="control-label col-sm-2" for="txtTotalCost">Total Cost:</label>';
        Div += '<div class="col-sm-45">';
        Div += '<input type="text" class="form-control" id="listEntityMaintainenceChargesRecords_' + DivIndex + '__total_cost" name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].total_cost" maxlength="5" onkeypress="return allowNumberwithOneDot(event,"listEntityMaintainenceChargesRecords_' + DivIndex + '__total_cost");">';
        Div += '</div>';
        Div += '<label class="control-label col-sm-2" for="txtRemarks">Remarks:</label>';
        Div += '<div class="col-sm-45">';
        Div += '<input type="text" class="form-control" id="listEntityMaintainenceChargesRecords_' + DivIndex + '__remark" name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].remark" maxlength="150">';
        Div += '</div>';
        Div += '</div>'; //form-group div closed

        Div += '<input name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].id" data-val="true" data-val-number="The field id must be a number." data-val-required="The id field is required." id="listEntityMaintainenceChargesRecords_' + DivIndex + '__id" type="hidden" value="0">';
        Div += '<input name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].created_by" data-val="true" data-val-number="The field created_by must be a number." data-val-required="The created_by field is required." id="listEntityMaintainenceChargesRecords_' + DivIndex + '__created_by" type="hidden" value="' + CreatedBy + '">';
        Div += '<input name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].entity_id" data-val="true" data-val-number="The field system_id must be a number." data-val-required="The system_id field is required." id="listEntityMaintainenceChargesRecords_' + DivIndex + '__entity_id" type="hidden" value="' + SystemId + '">';
        Div += '<input name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].entity_type" id="listEntityMaintainenceChargesRecords_0__entity_type" type="hidden" value="' + entitytype + '">';
        Div += '<input name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].modified_on" data-val="true" data-val-date="The field modified_on must be a date." id="listEntityMaintainenceChargesRecords_' + DivIndex + '__modified_on" type="hidden" value="">';
        Div += '<input name="MaintainenceCharges.listEntityMaintainenceChargesRecords[' + DivIndex + '].modified_by" data-val="true" data-val-number="The field modified_by must be a number." id="listEntityMaintainenceChargesRecords_' + DivIndex + '__modified_by" type="hidden" value="">';

        Div += '</div>';


        $('#dvEntityMaintainenceCharges').append(Div);

        $('.chosen-select').chosen({ width: '100%' });

        for (var i = 0; i < $('#dvEntityMaintainenceCharges .assign-calen').length; i++) {
            $('#imgActivityStartDate_' + i + '').unbind('click');
            $('#imgActivityEndDate_' + i + '').unbind('click');
            if ($('#listEntityMaintainenceChargesRecords__' + i + '_activity_start_date').length > 0) {
                si.setSurveyDateTimeCalendar('listEntityMaintainenceChargesRecords__' + i + '_activity_start_date', 'imgActivityStartDate_' + i + '', '', true);
            }
            if ($('#listEntityMaintainenceChargesRecords__' + i + '_activity_end_date').length > 0) {
                si.setSurveyDateTimeCalendar('listEntityMaintainenceChargesRecords__' + i + '_activity_end_date', 'imgActivityEndDate_' + i + '', '', true);
            }

        }

        //si.setSurveyDateTimeCalendar('listEntityMaintainenceChargesRecords_' + DivIndex + '_id_activity_start_date', 'imgActivityStartDate_' + DivIndex + '', '', true);
        //si.setSurveyDateTimeCalendar('listEntityMaintainenceChargesRecords_' + DivIndex + '_id_activity_end_date', 'imgActivityEndDate_' + DivIndex + '', '', true);

    }
    else {
        alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_144);//Maximum 10 Entries can be added!
    }
}


function DeleteMaintainenceDiv(rowIndex) {
    //// 
    var dvID = 'EMC_' + rowIndex;
    var len = $('#dvEntityMaintainenceCharges > .len').length;
    if (len == 1) {
        alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_145);//Atleast 1 status row required!
    }
    else {
        //Are you sure you want to delete this row?
        confirm(MultilingualKey.SI_OSP_GBL_JQ_GBL_006, function () {

            $('.' + dvID).remove();
            //// 
            var MaintainenceRow = $("#dvEntityMaintainenceCharges .dvDynamicForm");

            MaintainenceRow.each(function (i, val) {

                if (rowIndex <= i) {
                    $(this).find("input,select,span").each(function (n, nval) {

                        if ($(this).attr('name') != undefined)
                            $(this).attr('name', $(this).attr('name').replace(/\[\d+\]/, "[" + (i) + "]"));

                        if ($(this).attr('id') != undefined)
                            $(this).attr('id', $(this).attr('id').replace(/\_\d+\_/, "_" + (i) + "_"));

                        if ($(this).attr('onclick') != undefined)
                            $(this).attr('onclick', $(this).attr('onclick').replace(/\(\d+\)/, "(" + (i) + ")"));


                    });

                }

                var cls = $(val).attr('class').replace(/[0-9]/, i);
                $(val).removeClass();
                $(val).addClass(cls);

            });

        });
    }


}




var rowIndexForDelte;
function AddATStatusRow(RowIndex, SystemId, CreatedBy, entitytype) {
    //<span class="shaftRowDelete removeAT" title="Delete" style="">x</span>
    //// 
    rowIndexForDelte = RowIndex;
    var Row = "";
    if (RowIndex <= 50) {
        Row = '<div class="row AT_row_' + RowIndex + ' len">';
        //Row += '<div class="col-md-4"><select id="ddl_AtStatus" name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].status" class="chosen-select form-group" onchange="removeBorder(this)"><option value="0">-Select-</option><option value="AT Ready" data-planningname="AT Ready">AT Ready</option><option value="AT Started" data-planningname="AT Started">AT Started</option><option value="AT Completed" data-planningname="AT Completed">AT Completed</option><option value="AT Pending" data-planningname="AT Pending">AT Pending</option><option value="AT On Hold" data-planningname="AT On Hold">AT On Hold</option></select></div>';
        //Row += '<div class="col-md-4"><input class="form-control clsReadOnly" name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].status_date" data-val="true" data-val-date="The field status_date must be a date." data-val-required="The status_date field is required." id="ATAcceptance.listAtStatusRecords[' + RowIndex + '].status_date" placeholder="DD-MMM-YYYY" readonly="True" style="background:#fff;" type="text" value="' + GetFormattedDate(new Date()) + '"><img id="imgTargetDate_' + RowIndex + '" class="assign-calen" src="/Content/images/calendar.png" style="margin-top:1px;"></div>';
        //Row += '<div class="col-md-4" ><input class="form-control" id="remark" name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].remark"  maxlength="150" name="remark" type="text" style="width: 195px;float: left;" value=""><span class="icon-close removeAT" onclick="delRow(' + RowIndex + ')" title="Delete" style=""></span></div>';

        Row += '<div class="col-md-3"><select id="ddl_AtStatus" name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].status" class="chosen-select form-group" onchange="removeBorder(this)"><option value="0">-Select-</option><option value="AT Ready" data-planningname="AT Ready">AT Ready</option><option value="AT Started" data-planningname="AT Started">AT Started</option><option value="AT Completed" data-planningname="AT Completed">AT Completed</option><option value="AT Pending" data-planningname="AT Pending">AT Pending</option><option value="AT On Hold" data-planningname="AT On Hold">AT On Hold</option></select></div>';
        Row += '<div class="col-md-3"><input class="form-control clsReadOnly" name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].status_date" data-val="true" data-val-date="The field status_date must be a date." data-val-required="The status_date field is required." id="listAtStatusRecords_' + RowIndex + '__id" placeholder="DD-MMM-YYYY" readonly="True" style="background:#fff;" type="text" value="' + GetFormattedDate(new Date()) + '"><img id="imgTargetDate_' + RowIndex + '" class="assign-calen" src="/Content/images/calendar.png" style="margin-top:1px;"></div>';
        Row += '<div class="col-md-5" ><input class="form-control" id="remark" name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].remark"  maxlength="150" name="remark" type="text" value=""></div>';
        Row += '<div class="col-md-1" ><span class="icon-close removeAT" onclick="delRow(' + RowIndex + ')" title="' + MultilingualKey.SI_GBL_GBL_GBL_GBL_002 + '" style=""></span></div>';

        Row += '<input name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].id" data-val="true" data-val-number="The field id must be a number." data-val-required="The id field is required." id="listAtAcceptance_' + RowIndex + '__id" type="hidden" value="0">';
        Row += '<input name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].created_by" data-val="true" data-val-number="The field created_by must be a number." data-val-required="The created_by field is required." id="listAtAcceptance_' + RowIndex + '__created_by" type="hidden" value="' + CreatedBy + '">';
        Row += '<input name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].system_id" data-val="true" data-val-number="The field system_id must be a number." data-val-required="The system_id field is required." id="listAtAcceptance_' + RowIndex + '__system_id" type="hidden" value="' + SystemId + '">';
        Row += '<input name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].entity_type" id="listAtAcceptance_0__entity_type" type="hidden" value="' + entitytype + '">';
        Row += '<input name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].modified_on" data-val="true" data-val-date="The field modified_on must be a date." id="listAtAcceptance_' + RowIndex + '__modified_on" type="hidden" value="">';
        Row += '<input name="ATAcceptance.listAtStatusRecords[' + RowIndex + '].modified_by" data-val="true" data-val-number="The field modified_by must be a number." id="listAtAcceptance_' + RowIndex + '__modified_by" type="hidden" value="">'

        Row += '</div>';

        $('#dvATAcceptance').append(Row);
        $('.chosen-select').chosen({ width: '100%' });

        for (var i = 0; i < $('#dvATAcceptance .assign-calen').length; i++) {
            $('#imgTargetDate_' + i + '').unbind('click');
            if ($('#listAtStatusRecords_' + i + '__id').length > 0) {
                si.setSurveyDateTimeCalendar('listAtStatusRecords_' + i + '__id', 'imgTargetDate_' + i + '', '', true);
            }
        }
    }
    else {
        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_007);//Maximum 50 rows can be added!
    }
}

function delRow(rowIndex) {
    //// 
    var dvID = 'AT_row_' + rowIndex;
    var len = $('#dvATAcceptance > .len').length;
    if (len == 1) {
        alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_145);//Atleast 1 status row required!
    }
    else {
        //Are you sure you want to delete this row?
        confirm(MultilingualKey.SI_OSP_GBL_JQ_GBL_006, function () {

            $('.' + dvID).remove();
            //// 
            var statusRow = $("#dvATAcceptance .row");

            statusRow.each(function (i, val) {

                if (rowIndex <= i) {
                    $(this).find("input,select,span").each(function (n, nval) {

                        if ($(this).attr('name') != undefined)
                            $(this).attr('name', $(this).attr('name').replace(/\[\d+\]/, "[" + (i) + "]"));

                        if ($(this).attr('id') != undefined)
                            $(this).attr('id', $(this).attr('id').replace(/\_\d+\_/, "_" + (i) + "_"));

                        if ($(this).attr('onclick') != undefined)
                            $(this).attr('onclick', $(this).attr('onclick').replace(/\(\d+\)/, "(" + (i) + ")"));


                    });

                }

                var cls = $(val).attr('class').replace(/[0-9]/, i);
                $(val).removeClass();
                $(val).addClass(cls);

            });

        });
    }


}

function AddRemarkRow(RowIndex, SystemId, rowStage, _rowRemarks) {
    rowIndexForDelte = RowIndex;
    var Row = "";
    if (RowIndex < 4) {
        Row = '<div class="row"><div class="col-md-2 col-sm-2">';
        Row += '<label class="control-label" style="margin-bottom: 8px;">Remark-' + (RowIndex + 1) + ':</label>';
        Row += '</div>';
        Row += '<div class="col-md-10 col-sm-10 rowremarks">';
        Row += ' <input name="remarksList[' + RowIndex + '].id" id="remarksList_' + RowIndex + '__id" type="hidden" value="0">';
        Row += '<input name="remarksList[' + RowIndex + '].row_system_id" id="remarksLists_' + RowIndex + '__row_system_id" type="hidden" value="' + SystemId + '">';
        Row += '<input name="remarksList[' + RowIndex + '].row_stage" id="remarksList_' + RowIndex + '__row_stage" type="hidden" value="' + rowStage + '">';
        Row += '<input type="text" class="form-control" name="remarksList[' + RowIndex + '].remarks" id="remarksList_' + RowIndex + '__remarks" maxlength="100" />';
        //Row += '<textarea class="form-control" cols="20" name="remarksList[' + RowIndex + '].remarks" id="remarksList_' + RowIndex + '__remarks" maxlength="100"  rows="2" style="resize:none"></textarea>';
        Row += '</div>';
        Row += '</div>';

        $('#' + _rowRemarks).append(Row);
    }
    else {
        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_008);//Maximum 4 remarks can be added!
    }
}


function entityMaintainence() {
    // 
    var columncount = 0;
    var msg = "";
    var errormessage = false;
    var colorcolumn = 0;
    $('.dvDynamicForm').each(function (index, item) {
        var context = $(this);
        if (errormessage == false) {
            columncount = 0;
            colorcolumn = 0;
            var row = index + 1;
            if (context.children().find('.col-sm-45').eq(0).find('.chosen-single').text().trim() != "-Select-" && context.children().find('.col-sm-45').eq(1).find('.chosen-single').text().trim() != "-Select-") {

                if (context.children().find('.col-sm-45').eq(4).find('input').val().trim() == "" && errormessage == false) {
                    msg = MultilingualKey.SI_OSP_GBL_JQ_GBL_054 + " " + row + "!";//Please enter total cost at Row
                    columncount++;
                    colorcolumn = 4;
                }

                if (columncount > 0 && columncount < 3) {
                    errormessage = true;
                    if (colorcolumn == 0 || colorcolumn == 1)
                        context.children().eq(colorcolumn).find('.form-control').css('border', '#e60000 solid 1px');
                    else if (colorcolumn == 4)
                        context.children().find('.col-sm-45').eq(colorcolumn).find('input').css('border', '#e60000 solid 1px');

                }
                else {
                    errormessage = false;
                    msg = "";
                    columncount = 0;
                    colorcolumn = 0;
                }

            }
        }
    });


    if (columncount > 0) {
        alert(msg);
        return false;
    }
    else
        return true;
}

function entityReferencevalidation() {

    var columncount = 0;
    var msg = "";
    var errormessage = false;
    var colorcolumn = 0;
    $('.reference_row_PointA').each(function (index, item) {
        var context = $(this);

        if (errormessage == false) {
            columncount = 0;
            colorcolumn = 0;
            var row = index + 1;
            if (context.children().eq(2).find('.chosen-single').text().trim() == "-Select-" && errormessage == false) {

                msg = $.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_009, row); //Please select direction at PointA Row
                columncount++;
                colorcolumn = 2;
            }
            if (context.children().eq(1).find('input').val() == "" && errormessage == false) {
                msg = $.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_010, row);//Please enter Distance at PointA Row
                columncount++;
                colorcolumn = 1;
            }
            if (context.children().eq(0).find('input').val() == "" && errormessage == false) {
                msg = $.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_011, row);//Please enter landmark at PointA Row
                columncount++;
                colorcolumn = 0;
            }
            if (columncount > 0 && columncount < 3) {
                errormessage = true;
                if (colorcolumn == 0 || colorcolumn == 1)
                    context.children().eq(colorcolumn).find('.form-control').css('border', '#e60000 solid 1px');
                else if (colorcolumn == 2)
                    context.children().eq(colorcolumn).find('.chosen-single').css('border', '#e60000 solid 1px');
            }
            else {
                errormessage = false;
                msg = "";
                columncount = 0;
                colorcolumn = 0;
            }
        }

    });



    $('.reference_row_PointB').each(function (index, item) {
        var context = $(this);

        if (errormessage == false) {
            columncount = 0;
            colorcolumn = 0;
            var row = index + 1;
            if (context.children().eq(2).find('.chosen-single').text().trim() == "-Select-" && errormessage == false) {
                msg = $.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_012, row);//Please select direction at PointB Row
                columncount++;
                colorcolumn = 2;

            }
            if (context.children().eq(1).find('input').val() == "" && errormessage == false) {
                msg = $.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_013, row);//Please enter Distance at PointB Row
                columncount++;
                colorcolumn = 1;
            }
            if (context.children().eq(0).find('input').val() == "" && errormessage == false) {
                msg = $.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_014, row);//Please enter landmark at PointB Row
                columncount++;
                colorcolumn = 0;
            }

            if (columncount > 0 && columncount < 3) {
                errormessage = true;
                if (colorcolumn == 0 || colorcolumn == 1)
                    context.children().eq(colorcolumn).find('.form-control').css('border', '#e60000 solid 1px');
                else if (colorcolumn == 2)
                    context.children().eq(colorcolumn).find('.chosen-single').css('border', '#e60000 solid 1px');
            }
            else {
                errormessage = false;
                msg = "";
                columncount = 0;
                colorcolumn = 0;
            }
        }

    });

    if (columncount > 0) {
        alert(msg);
        return false;
    }
    else
        return true;

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
function ValidateNetworkId(systemid, networkid, networktype, entype) {
    var network_type = networktype;
    var ret = false;
    if (network_type == 'M' && systemid == 0) {
        //var _netwoekidlength=0;
        var _requiredMaskedlength = 0;
        var layerTitle = getLayerTltle(entype);
        if (networkid.indexOf('x') > -1) {

            //_netwoekidlength = networkid.slice(0, networkid.indexOf('x')).length;
            //_requiredMaskedlength = networkid.length - _netwoekidlength;
            _requiredMaskedlength = countChar(networkid, 'x');
        }
        //else
        if (networkid.indexOf('n') > -1) {

            //_netwoekidlength += networkid.slice(0, networkid.indexOf('n')).length;
            //_requiredMaskedlength = networkid.length - _netwoekidlength;
            _requiredMaskedlength += countChar(networkid, 'n');
        }

        var inputvalue = $("#txtMaskedNetworkId").dxTextBox("option", "value").replace(/ /g, ''); //6
        var inputValueWithMask = $("#txtMaskedNetworkId").dxTextBox("option", "text"); //13
        if (inputvalue.length == _requiredMaskedlength) {
            // Validate network id for duplicate....
            //Ajax method


            ajaxReq('Library/IsNetworkIdExist', { networkId: inputValueWithMask, entityType: entype, networkStage: 'P' }, false, function (resp) {
                if (resp != undefined && resp.status == 'OK') {

                    alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_FRM_146, layerTitle))//Code already exist!
                    ret = false;
                }
                else {
                    ret = true;
                }
            }, true, false);

        }
        else {
            //code should be                                                          //character long!

            alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_018, layerTitle.toUpperCase(), networkid.length));
            ret = false;
        }


    }
    else {
        // Auto Id generation
        ret = true;
    }
    return ret;
}

function bindVendor(specification) {
    var ddlVendor = $('#ddlVendor');
    if (specification != '') {
        ajaxReq('ItemTemplate/GetVendorList', { specification: specification }, false,
      function (resp) {
          if (resp.status == "OK") {
              $('#txtItemCode').val('');
              $('#txtCategory').val('');
              $('#txtSubCategory1').val('');
              $('#txtSubCategory2').val('');
              //$('#txtSubCategory3').val('');
              $('#no_of_input_port').val('');
              $('#no_of_output_port').val('');
              $('#no_of_port').val('');
              $('#ddlIOP').val('');
              $('#ddlIOP').trigger("chosen:updated");
              $('#no_of_tube').val(0);
              $('#no_of_core_per_tube').val(0);
              ddlVendor.empty();
              ddlVendor.append($("<option></option>").val('').html('Select Vendor'));
              $.each(resp.result, function (data, value) {
                  ddlVendor.append($("<option></option>").val(value.key).html(value.value));
              });
          }
          else {
              $('#txtItemCode').val('');
              $('#txtCategory').val('');
              $('#txtSubCategory1').val('');
              $('#txtSubCategory2').val('');
              //$('#txtSubCategory3').val('');
              $('#no_of_input_port').val('');
              $('#no_of_output_port').val('');
              $('#no_of_port').val('');
              $('#ddlIOP').val('');
              $('#ddlIOP').trigger("chosen:updated");
              $('#no_of_tube').val(0);
              $('#no_of_core_per_tube').val(0);
              ddlVendor.empty();
              ddlVendor.append($("<option></option>").val('').html('Select Vendor'));
          }
      }, true, true);
    }
    else {
        $('#txtItemCode').val('');
        $('#txtCategory').val('');
        $('#txtSubCategory1').val('');
        $('#txtSubCategory2').val('');
        // $('#txtSubCategory3').val('');
        $('#no_of_input_port').val('');
        $('#no_of_output_port').val('');
        $('#no_of_port').val('');
        $('#ddlIOP').val('');
        $('#ddlIOP').trigger("chosen:updated");
        $('#no_of_tube').val(0);
        $('#no_of_core_per_tube').val(0);
        ddlVendor.empty();
        ddlVendor.append($("<option></option>").val('').html('Select Vendor'));
    }
    ddlVendor.trigger("chosen:updated");
}

function bindCateSubcat(entitytype, specification, vendorid) {
    if (vendorid != '') {
        ajaxReq('ItemTemplate/GetCatSubcatData', { entitytype: entitytype, specification: specification, vendorId: vendorid }, false,
                function (resp) {
                    $('#ddlIOP option:not(:selected)').removeAttr('disabled');
                    if (resp.status == "OK") {
                        var category = resp.result[0].category;
                        var subCat1 = resp.result[0].subCategory_1;
                        var subCat2 = resp.result[0].subCategory_2;
                        var subCat3 = resp.result[0].subCategory_3;
                        var auditTemplateId = resp.result[0].audit_item_master_id;
                        var no_of_tube = resp.result[0].no_of_tube;
                        var no_of_core_per_tube = resp.result[0].no_of_core_per_tube;
                        $("#audit_item_master_id").val(auditTemplateId);
                        var code = resp.result[0].code;
                        if (code != '') {
                            $('#txtItemCode').removeClass('input-validation-error')
                            $('#txtItemCode').val(code);
                        }
                        $('#txtCategory').val(category);
                        $('#txtSubCategory1').val(subCat1);
                        $('#txtSubCategory2').val(subCat2);
                        $('#txtSubCategory3').val(subCat3);
                        if (resp.result[0].no_of_input_port > 0) { $('#no_of_input_port').val(resp.result[0].no_of_input_port); }
                        if (resp.result[0].no_of_output_port > 0) { $('#no_of_output_port').val(resp.result[0].no_of_output_port); }
                        if (resp.result[0].no_of_port > 0) { $('#no_of_port').val(resp.result[0].no_of_port); }
                        if (entitytype != null && (entitytype.toUpperCase() == 'BDB' || entitytype.toUpperCase() == 'ADB' || entitytype.toUpperCase() == 'CDB' || entitytype.toUpperCase() == 'FDB' || entitytype.toUpperCase() == 'HTB' || entitytype.toUpperCase() == 'ONT' || entitytype.toUpperCase() == 'SPLITTER')) {
                            $('#ddlIOP').val(resp.result[0].no_of_input_port + ':' + resp.result[0].no_of_output_port);
                            $('#ddlIOP').trigger("chosen:updated");
                            $('#ddlIOP option:not(:selected)').attr('disabled', true).trigger("chosen:updated");
                            $('#ddlIOP_chosen .chosen-search-input').attr('disabled', true);
                        } else if (entitytype != null && (entitytype.toUpperCase() == 'CABLE' || entitytype.toUpperCase() == 'PATCHCORD')) {
                            $('#ddlIOP').val(resp.result[0].other);
                            $('#ddlIOP').trigger("chosen:updated");
                            $('#no_of_tube').val(no_of_tube);
                            $('#no_of_core_per_tube').val(no_of_core_per_tube);
                            $('#ddlIOP option:not(:selected)').attr('disabled', true).trigger("chosen:updated");
                            $('#ddlIOP_chosen .chosen-search-input').attr('disabled', true);
                            $("#hdn_no_of_core_per_tube").val($("#no_of_core_per_tube").val())
                            $("#hdn_no_of_tube").val($("#no_of_tube").val())
                            $('#hdn_unitValue').val($("#ddlIOP").val());
                            $("#hdn_vendor_id").val($("#ddlVendor").val())
                            $('#hdn_item_code').val($("#txtItemCode").val());
                            $("#hdn_specification").val($("#specification").val())
                        }

                    }
                    else {
                        $('#txtItemCode').val('');
                        $('#txtCategory').val('');
                        $('#txtSubCategory1').val('');
                        $('#txtSubCategory2').val('');
                        $('#txtSubCategory3').val('');
                        $('#no_of_input_port').val('');
                        $('#no_of_output_port').val('');
                        $('#no_of_port').val('');
                        $('#ddlIOP').val('');
                        $('#ddlIOP').trigger("chosen:updated");
                        $('#no_of_tube').val(0);
                        $('#no_of_core_per_tube').val(0);
                        $("#hdn_no_of_core_per_tube").val('')
                        $("#hdn_no_of_tube").val('')
                        $('#hdn_unitValue').val('');
                        $("#hdn_vendor_id").val('')
                        $('#hdn_item_code').val('');
                        $("#hdn_specification").val('')
                    }
                }, true, true);
    }
    else {
        $('#txtItemCode').val('');
        $('#txtCategory').val('');
        $('#txtSubCategory1').val('');
        $('#txtSubCategory2').val('');
        $('#txtSubCategory3').val('');
        $('#no_of_input_port').val('');
        $('#no_of_output_port').val('');
        $('#no_of_port').val('');
        $('#ddlIOP').val('');
        $('#ddlIOP').trigger("chosen:updated");
        $('#no_of_tube').val(0);
        $('#no_of_core_per_tube').val(0);
    }
}

function bindBrand(typeid) {
    var Layer_id = $("#Layer_id").val();
    var ddlBrand = $("#ddlBrand");
    if (typeid != '') {
        ajaxReq('ItemTemplate/GetBrand', { typeId: typeid, Layer_id: Layer_id }, false,
                function (resp) {
                    if (resp.status == "OK") {
                        $(ddlBrand).empty();
                        $(ddlBrand).append($("<option></option>").val('').html('Select Brand'));
                        $.each(resp.result, function (data, value) {
                            $(ddlBrand).append($("<option></option>").val(value.value).html(value.key));
                        });
                    }
                    else {
                        $(ddlBrand).empty();
                        $(ddlBrand).append($("<option></option>").val('').html('Select Brand'));
                    }
                }, true, true);
    }
    else {
        $(ddlBrand).empty();
        $(ddlBrand).append($("<option></option>").val('').html('Select Brand'));
    }
    bindModel($('#ddlBrand').val());
    $(ddlBrand).trigger("chosen:updated");

}

function bindModel(brandid) {
    var ddlModel = $("#ddlmodel");
    if (brandid != '') {
        ajaxReq('ItemTemplate/GetModel', { brandId: brandid }, false,
                function (resp) {
                    if (resp.status == "OK") {
                        $(ddlModel).empty();
                        $(ddlModel).append($("<option></option>").val('').html('Select Model'));
                        $.each(resp.result, function (data, value) {
                            $(ddlModel).append($("<option></option>").val(value.value).html(value.key));
                        });
                    }
                    else {
                        $(ddlModel).empty();
                        $(ddlModel).append($("<option></option>").val('').html('Select Model'));
                    }
                }, true, true);
    }
    else {
        $(ddlModel).empty();
        $(ddlModel).append($("<option></option>").val('').html('Select Model'));
    }
    $(ddlModel).trigger("chosen:updated");
}
function ddlChangeRemoveCss(selectorId, emType) {
    
    var emID = $("#" + selectorId + "_chosen");
    if (emID.children('a').text().trim() != "Select " + emType) {
        emID.removeClass('input-validation-error').next('span').removeClass('field-validation-error').addClass('field-validation-valid').html('');
    }
    else {
        emID.addClass('input-validation-error');
    }
}
//textbox can shown only numeric datatype
allowOnlyNumber = function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}
GetShaftNFloorList = function (entityType) {
    var structureid = $('#ddlStructureList').val();
    $('#divParentEntity').hide();
    $('#ddlUnitList').val('').trigger('chosen:updated');
    if (structureid == "") {
        $("#dvFloorDDL").hide();
        $("#dvShaftDDL").hide();
        $('#dvAssociateType').hide();
        $("#ddlShaftlist").val('').trigger("chosen:updated");
        $("#ddlFloorlist").val('').trigger("chosen:updated");
        $("#ddlAssociationtype").val('').trigger("chosen:updated");
        $('#txtAddress').val('');
    } else {
        $('#dvAssociateType').show();
        var selectType = $("select#ddlAssociationtype").val();
        if (selectType == "Shaft") {
            $("#dvFloorDDL").show();
            $("#dvShaftDDL").show();
            $("#ddlShaftlist").val('').trigger("chosen:updated");
            $("#ddlFloorlist").val('').trigger("chosen:updated");
        } else if (selectType == "Floor") {
            $("#dvShaftDDL").hide();
            $("#dvFloorDDL").show();
            $("#ddlShaftlist").val('').trigger("chosen:updated");
            $("#ddlFloorlist").val('').trigger("chosen:updated");
        }
        ajaxReq('Library/GetShaftNFloorByBld', { structureId: structureid, entityType: entityType, floorId: 0 }, false, function (resp) {
            if (resp.ShaftList.length > 0) {
                var optHTML = '<option value="">Select Shaft</option>';
                for (var i = 0; i < resp.ShaftList.length; i++) {
                    optHTML += '<option value=' + resp.ShaftList[i].systemid + '>' + resp.ShaftList[i].entityname + '</option>';
                }
                $("#ddlShaftlist").html(optHTML);
                $("#ddlShaftlist").trigger("chosen:updated");
            }
            if (resp.FloorList.length > 0) {
                var optHTML = '<option value="">Select Floor</option>';
                for (var i = 0; i < resp.FloorList.length; i++) {
                    optHTML += '<option value=' + resp.FloorList[i].systemid + '>' + resp.FloorList[i].entityname + '</option>';
                }
                $("#ddlFloorlist").html(optHTML);
                $("#ddlFloorlist").trigger("chosen:updated");
            }
            //if (resp.UnitList.length > 0 && $("#ddlUnitList").length > 0) {
            //    var optHTML = '<option value="">Select Unit</option>';
            //    for (var i = 0; i < resp.UnitList.length; i++) {
            //        optHTML += '<option value=' + resp.UnitList[i].system_id + '>' + resp.UnitList[i].room_name + '</option>';
            //    }
            //    $("#ddlUnitList").html(optHTML);
            //    $("#ddlUnitList").trigger("chosen:updated");
            //}
            //if (entityType == 'POD' || entityType == 'MPOD' || entityType == 'Customer' || entityType == 'ONT') {
            //    //$('#dvAssociateType').hide(); $("#ddlShaftlist").hide(); $('#dvFloorDDL').show();
            //    $("#ddlAssociationtype option[value='Shaft']").hide().trigger("chosen:updated");
            //    //$('#ddlAssociationtype option:not(:selected)').attr('disabled', true).trigger("chosen:updated"); $('#ddlAssociationtype_chosen .chosen-search-input').attr('disabled', true);
            //} //else { $('#dvAssociateType').show();  }
            //if ($('#hdnIsFloorElement').val() == 'True') {
            //    $('#ddlAssociationtype option:not(:selected)').attr('disabled', true).trigger("chosen:updated");
            //   $('#ddlAssociationtype_chosen .chosen-search-input').attr('disabled', true);
            //}
            if (entityType == 'Customer') {
                ajaxReq('Library/GetBuildingAddress', { structureId: structureid }, false, function (resp) {
                    $('#txtAddress').val(resp);
                }, false, false);
            }
        }, false, false);
    }

}
fncShaftDdlChange = function () {
    $("#dvONTFloorDDL").show();
    $('#ddlFloorlist option').attr('disabled', false).trigger("chosen:updated");
    if ($("#ddlShaftlist").val() != '') {
        ajaxReq('ISP/getShaftRange', { ShaftId: parseInt($("#ddlShaftlist").val()) }, false, function (resp) {
            if (resp.length > 0) {
                $('#ddlFloorlist option:not([value=""])').attr('disabled', true).trigger("chosen:updated");
                $("#ddlFloorlist option").filter(function () {
                    var systemId = parseInt($(this).attr('value'));
                    var index = resp.findIndex(function (m) { return systemId >= m.shaft_start_range && systemId <= m.shaft_end_range; });
                    if (index >= 0) {
                        $('#ddlFloorlist option[value="' + systemId + '"]').attr('disabled', false).trigger("chosen:updated");
                    }
                });
            }
        }, false, false);
    }
}
//refreshStructureInfo = function (systemId, entityType, NetworkId, message, isCallFromPageMessage) {
//    if (isp != null) {
//        isp.refreshStructureInfo();
//        // isp.refreshStructureInfo({ systemId: systemId, entityType: entityType, networkId: NetworkId, message: message }, isCallFromPageMessage);
//    }
//}
refreshStructureInfo = function (param) {
    if (isp != null) {
        isp.refreshStructureInfo();
        //isp.partialLoad.renderNew(param);
    }
}
//textbox can shown only numeric datatype
function allowNumberwithDot(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}
function allowNumberwithOneDot(evt, txtId) {
    var DotLength = 0;
    //if ($('#' + txtId).val() != '' || $('#' + txtId).val() != null)
    //{  }
    DotLength = $('#' + txtId).val().indexOf('.');

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

function allowNumberwithTwoDotWithComma(evt, txtId) {
     
    var DotLength = 0;
    var commaLength = 0;

    var value = $('#' + txtId).val();
   
    commaLength = value.indexOf(',');

    var charCode = (evt.which) ? evt.which : event.keyCode;

    if (charCode == 46) {
        if (value !='')
        {
        DotLength = value.match(/\./g).length;
        }
        
        if (DotLength >= 2)
            return false;
        else
            return true;
    }
    if (charCode == 44) {
        if (commaLength >= 0)
            return false;
        else
            return true;
    }
    
    if ((charCode > 31 || charCode < 43)  &&  (charCode < 48 || charCode > 57)) {
        return false;
    }
    if (charCode == 45 || charCode == 47) {
        return false;
    }
    return true;
}

function hasDecimalPlace(value, x) {
    var pointIndex = value.indexOf('.');
    return pointIndex >= 0 && pointIndex < value.length - x;
}
this.removeBorder = function () {
    $('.form-control').css('border', '');
    $('.chosen-single').css('border', '');
}

this.setDateTimeCalendar = function (startdateid, startdateimgid) {

    Calendar.setup({
        inputField: startdateid,   // id of the input field
        button: startdateimgid,
        ifFormat: "%d-%b-%Y",       // format of the input field datetime format %d-%b-%Y %I:%M %p
        showsTime: false,
        timeFormat: "12",
        weekNumbers: false,
        //onUpdate: catcalc,

        //disableFunc: function (date) {
        //    //
        //    if (!isBackDateAllowed) {
        //        var now = new Date();
        //        now.setDate(now.getDate() - 1);
        //        if (date.getTime() < now.getTime()) {
        //            return true;
        //        }
        //    }
        //}
    });

}
function notAllowZero(objControlls) {
    $(objControlls).keyup(function () {
        var digitsLength = $(this).val().length;
        if (digitsLength == 1) {
            if ($(this).val() === '0') {
                $(this).val('');
                return false;
            }
        }
    });
}
function disabledItem(event) {
    console.log('disabled item');
    event.stopPropagation();
    return false;
}


function setMeasuredLength(source, target) {
    $(source).keyup(function () {
        if ($('#cable_type').val() == 'ISP') {
            $(target).val($(this).val());
            $(target).removeClass('input-validation-error');
        }
    });
}

function ValidateCableLenght(measureLenCable, calculatedLenCable, cableName) {
    var measureLenCable = parseFloat($(measureLenCable).val());
    var calculatedLenCable = parseFloat($(calculatedLenCable).val());
    if (calculatedLenCable.toString() == "NaN") {
        calculatedLenCable = 0;
    }

    if (measureLenCable == 0) {
        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_015);//Measured length should be greater than 0 !!
        return false;
    }

    if (calculatedLenCable < measureLenCable) {
        //calculated length should be greater or equal to               //measured length !!
        alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_016, cableName, cableName));
        return false;
    }
}
function ValidateDuctLenght(measureLenDuct, calculatedLenDuct, ductName) {
    var measureLenDuct = parseFloat($(measureLenDuct).val());
    var calculatedLenDuct = parseFloat($(calculatedLenDuct).val());

    if (measureLenDuct == 0) {
        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_015);//Measured length should be greater than 0 !!
        return false;
    }

    if (calculatedLenDuct < measureLenDuct) {
        //calculated length should be greater or equal to               //measured length !!
        alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_016, ductName, ductName));
        return false;
    }
}
function ValidateFilterDate(fromDate, toDate) {
    var ret = false;

    if (fromDate == '' && toDate == '') {
        ret = true;
    }
    else if (fromDate != '' && toDate != '') {
        ret = true;
    }

    else {
        ret = false;
    }


    return ret;
}

//Get Current Date
function fncCurrentDate(objDate) {
    // //
    var currentDate = new Date();
    if ((objDate == undefined) && (objDate == ''))
        objDate = currentDate;
    var formatedDate = GetFormattedDate(objDate);
    return formatedDate;
}
//Get Current Time
function funcCurrentTime() {
    var dt = new Date();
    return GetFormattedTime(dt, '24', true);
}
function JsonDateTimeFormator(jsonDate) {
    var objDate = new Date(parseInt(jsonDate.substr(6)));
    return GetFormattedDateTime(objDate);
}
function JsonDateFormator(jsonDate) {
    var objDate = new Date(parseInt(jsonDate.substr(6)));
    return GetFormattedDate(objDate);
}
function JsonTimeFormator(jsonDate) {
    var objDate = new Date(parseInt(jsonDate.substr(6)));
    return GetFormattedTime(objDate, '12', true);
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



function GetFormattedDateTime(objDate) {
    if (objDate != undefined) {
        return GetFormattedDate(objDate) + ' ' + GetFormattedTime(objDate, '12', true);
    }
    return '';
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
function GetFormattedTime(objDate, timeFormat, includeSeconds) {
    if (objDate != undefined) {
        var strTime = '';
        var ampm = '';
        var hours = objDate.getHours();
        var minutes = objDate.getMinutes();
        var seconds = objDate.getSeconds();
        if (timeFormat == '12') {
            ampm = hours >= 12 ? 'PM' : 'AM';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
        }
        minutes = minutes < 10 ? '0' + minutes : minutes;
        seconds = seconds < 10 ? '0' + seconds : seconds;
        strTime = hours + ':' + minutes;
        if (includeSeconds)
            strTime += ':' + seconds;
        if (timeFormat == '12')
            strTime += ' ' + ampm;

        return strTime;
    }
    return '';

}

function getTimeStamp() {

    var date = new Date();
    return date.getFullYear() + "_" + (date.getMonth() + 1) + "_" + date.getHours() + "_" + date.getMinutes() + "_" + date.getSeconds() + "_" + date.getMilliseconds();
}




function addDefaultSortingIcons(gridControlId) {
    $('#' + gridControlId + '  tr th').addClass('bothorder'); // you can add both Icon Class here
}

// change sort icon asc or desc
// generic method for all grids
function onGridHeaderClick(gridControlId) {
     

    $('#' + gridControlId + ' tr th').addClass('bothorder');
    var dir = $('#dir').val(); //direction value
    var col = $('#col').val(); // header value

    //var clickedheader = $('th a[href*=' + col + ']');
    //var countTh = document.getElementsByTagName('th').length; //total column header

    var clickedheader = $('#' + gridControlId + '  tr th a[href*="' + col + '"]');
    var countTh = $('#' + gridControlId + '  tr th').length; //total column header

    for (var i = 1; i <= countTh; i++) {
        var txtTh = $('#' + gridControlId + '  tr th:nth-child(' + i + ')').text(); // header text
        if (txtTh.trim().toLowerCase() == "action") {
            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').removeClass('bothorder');
        }
        if (txtTh.trim().toLowerCase() == clickedheader.text().trim().toLowerCase() && dir == 'Ascending') {

            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').removeClass('bothorder');
            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').addClass('ascendingorder');
        }
        else if (txtTh.trim().toLowerCase() == clickedheader.text().trim().toLowerCase() && dir == 'Descending') {

            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').removeClass('bothorder');
            $('#' + gridControlId + '  tr th:nth-child(' + i + ')').addClass('descendingorder');
        }
        if (col == "jc") {
          var  clickedheaderJc = clickedheader.init(2)[0].innerHTML;

            if (txtTh.trim().toLowerCase() == clickedheaderJc.toLowerCase() && dir == 'Ascending') {
                $('#' + gridControlId + '  tr th:nth-child(' + i + ')').removeClass('bothorder');
                $('#' + gridControlId + '  tr th:nth-child(' + i + ')').addClass('ascendingorder');
            }
            else if (txtTh.trim().toLowerCase() == clickedheaderJc.toLowerCase() && dir == 'Descending') {

                $('#' + gridControlId + '  tr th:nth-child(' + i + ')').removeClass('bothorder');
                $('#' + gridControlId + '  tr th:nth-child(' + i + ')').addClass('descendingorder');
            }
        }
    }
}
function refreshNLayers(enType) {
    var cableCHKCount = $('#layersContainer ul input[data-mapabbr="' + enType + '"]').length;
    if (cableCHKCount == 0) { $('#layersContainer ul').append('<li><input id="chk_nLyr_" type="checkbox" data-mapabbr="' + enType + '" checked="checked"><span>' + enType + '</span></li>'); }
}

//#start End To End Schematic 

var FontSize = 0;
var zoom_scale = 1, dpi = 72, view_left = 180, view_top = 80, print_width = 0, print_height = 0, goReset = 0;
var equipmentId = "";
var portNo = 0;

function SetEndToEndPrintSize(pageSize, divId) {
    dpi = 300;
    var printMarginLeft = 0, printMarginTop = 0, printZoomScale, childCount = 0;
    switch (pageSize) {
        case 'A0':
            FontSize = 70;//90;
            print_width = 3370 * dpi / 72;
            print_height = 2348 * dpi / 72;
            break;
        case 'A1':
            FontSize = 60;//80;
            print_width = 2384 * dpi / 72;
            print_height = 1684 * dpi / 72;
            break;
        case 'A2':
            FontSize = 40;//80;
            print_width = 1684 * dpi / 72;
            print_height = 1191 * dpi / 72;
            break;
        case 'A3':
            FontSize = 40;//80;
            print_width = 1191 * dpi / 72;
            print_height = 842 * dpi / 72;
            break;
        case 'A4':
            FontSize = 30;
            print_width = 842 * dpi / 72;
            print_height = 595 * dpi / 72;
            break;
    }

    var overlay = $('svg[class^="overlay"]');
    var g = $('#' + divId + ' g:not([class])');
    var maxWidth = 0;
    $('#' + divId + ' .node').each(function (i) {
        var nodeValue = $(this).attr("transform").replace("translate(", "").split(',')[0];
        var value = parseInt(nodeValue);
        maxWidth = maxWidth > value ? maxWidth : value;
    });
    maxWidth = maxWidth < 1 ? 210 : maxWidth;
    childCount = maxWidth / 210;
    printZoomScale = print_width / maxWidth;
    overlay.attr("width", print_width + (printZoomScale * 240) + "px");
    overlay.attr("height", print_height + "px");


    printMarginLeft = 220 + (printZoomScale - 1) * 180;
    printMarginTop = 150;// print_height / 4;   

    g.attr("transform", "translate(" + printMarginLeft + "," + printMarginTop + ")scale(" + printZoomScale + ")");
}


function SaveSchematicDiagram(fileExtenstion, divId) {
    showProgress();
    var divId = divId;
    var streamTitle = ($('#chkSchematicUpStream').is(':checked') ? 'UpStream' : 'DownStream');
    goReset = 1;

    var pageSize = $('#pageSize').val();
    SetEndToEndPrintSize(pageSize, divId);

    var node = document.getElementById(divId);

    node.style.borderTop = "0px solid #d3d3d3";
    var canvas = document.createElement('canvas');
    canvas.width = node.scrollWidth + 300;
    canvas.height = node.scrollHeight;
    var options = {
        quality: 0.55,
        bgcolor: "#fff"
    };
    domtoimage.toJpeg(node, options).then(function (pngDataUrl) {

        var img = new Image();
        ResetToInitialView(divId);
        var filename = "SmartInventory" + getTimeStamp() + "." + fileExtenstion;

        if (fileExtenstion == 'png') {
            var link = document.createElement("a");
            link.download = filename;
            link.href = pngDataUrl;
            document.body.appendChild(link);
            link.trigger("click");
        }
        else if (fileExtenstion == 'pdf') {

            img.src = pngDataUrl;
            img.onload = function () {
                var pdf = new jsPDF('l', 'px', [print_width, print_height]);
                pdf.addImage(img, 'PNG', 0, 250, print_width, print_height);

                pdf.setFontSize(FontSize);


                var ProjectName = $('#hdnApplicationName').val();
                pdf.text(ProjectName, 20, 50); //("",left margin,top margin)

                var currentDate = "Date :" + GetFormattedDateTime(new Date());
                if (pageSize == "A4") {
                    pdf.text(currentDate, (print_width - 320), 50);
                }
                else if (pageSize == "A0" || pageSize == "A1") {
                    pdf.text(currentDate, (print_width - 730), 50);
                }
                else {
                    pdf.text(currentDate, (print_width - 420), 50);
                }
                var LineWidth = print_width - 20;

                pdf.line(20, 100, LineWidth, 100);
                // End Header

                var text = "Equipment Id : " + equipmentId + "(" + PdfPortNo + ")",
                xOffset = (pdf.internal.pageSize.width / 2) - (pdf.getStringUnitWidth(text) * pdf.internal.getFontSize() / 2);
                pdf.text(text, xOffset, 250);

                if (pageSize == "A0" || pageSize == "A1") {
                    pdf.text("Stream Type : " + streamTitle, xOffset, 320);
                }
                else {
                    pdf.text("Stream Type : " + streamTitle, xOffset, 280);
                }
                //pdf.text("Stream Type : " + streamTitle, xOffset, 280);

                //Start Footer

                yoffSet = print_height - 80;

                pdf.line(20, yoffSet, LineWidth, yoffSet);

                var FooterName = "";// "Powered by Lepton Software";
                pdf.text(FooterName, 20, (print_height - 30)); //("",left margin,top margin)

                pdf.text("Page 1", (print_width - 150), (print_height - 30));

                pdf.save(filename);
            }
            //end footer
        }

        node.style.paddingLeft = "0px";
        node.style.borderTop = "1px solid #d3d3d3";

    });
    setTimeout(function () {
        if ($('#chkSchematicUpStream').is(':checked')) {
            $('#chkSchematicUpStream').prop('checked', false);
            $('#chkSchematicUpStream').trigger("click");
        } else {
            $('#chkSchematicDownStream').prop('checked', false);
            $('#chkSchematicDownStream').trigger("click");
        }
        hideProgress();
    }, 1000)
}

function ResetToInitialView(divId) {
    if (goReset == 1) {
        goReset = 0;
        var g = $('#' + divId + ' g:not([class])');
        g.attr("transform", "translate(" + view_left + "," + view_top + ")scale(" + zoom_scale + ")");

    }
}

//# End

//# generic method for download div as a image and pdf using jspdf 

function GetPrintSize(pageSize, divId) {

    switch (pageSize) {
        case 'A0':
            FontSize = 45;
            print_width = 3370;
            print_height = 2348;
            break;
        case 'A1':
            FontSize = 30;
            print_width = 2384;
            print_height = 1684;
            break;
        case 'A2':
            FontSize = 25;
            print_width = 1684;
            print_height = 1191;
            break;
        case 'A3':
            FontSize = 20;
            print_width = 1191;
            print_height = 842;
            break;
        case 'A4':
            FontSize = 15;
            print_width = 842;
            print_height = 595;
            break;
    }
}


function DownloadDivAsPdfOrImg(fileExtenstion, divId, callback) {

    var pageSize = $('#pageSize').val();
    GetPrintSize(pageSize, divId);

    var node = document.getElementById(divId);

    var pageHeightWithHeaderFooter = (2 * FontSize) + 90;
    node = ResizeImageKeepAspectRatio(node, print_height, print_width, pageHeightWithHeaderFooter);

    var options = {
        quality: 2,
        bgcolor: "#fff"
    };
    GetDivAsPdfOrImg(node, options, fileExtenstion, callback, divId);
}


function GetDivAsPdfOrImg(node, options, fileExtenstion, callback, divId) {

    domtoimage.toJpeg(node, options).then(function (pngDataUrl) {

        var img = new Image();
        if (callback != null) { callback(); }


        var filename = "SmartInventory" + getTimeStamp() + "." + fileExtenstion;

        if (fileExtenstion == 'png') {
            var link = document.createElement("a");
            link.download = filename;
            link.href = pngDataUrl;
            document.body.appendChild(link);
            link.trigger("click");
        }
        else if (fileExtenstion == 'pdf') {
            var pdf = new jsPDF('l', 'px', [print_width, print_height]);
            pdf.setFontSize(FontSize);

            var ProjectName = $('#hdnApplicationName').val();
            var TopMarginForHeader = FontSize + 15;
            pdf.text(ProjectName, 20, TopMarginForHeader);


            var currentDate = "Date :" + GetFormattedDateTime(new Date());

            var leftMargin = print_width - (pdf.getStringUnitWidth(currentDate) * pdf.internal.getFontSize()) + pdf.internal.getFontSize();
            pdf.text(currentDate, leftMargin, TopMarginForHeader);

            var LineWidth = print_width - 20;

            pdf.line(20, (TopMarginForHeader + 10), LineWidth, (TopMarginForHeader + 10));


            img.src = pngDataUrl;
            pdf.addImage(img, 'PNG', 0, (TopMarginForHeader + 20), node.width, node.height);

            yoffSet = print_height - (FontSize + 35);

            pdf.line(20, yoffSet, LineWidth, yoffSet);

            var FooterName = "";// "Powered by Lepton Software";
            pdf.text(FooterName, 20, (print_height - 30)); //("",left margin,top margin)

            var pageNo = "Page " + " " + 1;
            var leftFooterMargin = print_width - (pdf.getStringUnitWidth(pageNo) * pdf.internal.getFontSize());
            pdf.text(pageNo, leftFooterMargin, (print_height - 30));

            pdf.save(filename);

            //end footer
        }

    });

}

function ResizeImageKeepAspectRatio(node, pageHeight, pageWidth, pageHeightWithHeaderFooter) {


    pageHeight = pageHeight - pageHeightWithHeaderFooter;
    if (node.offsetHeight != pageHeight || node.offsetWidth != pageWidth) {

        // Scaling
        var scaling;
        var scalingY = node.offsetHeight / pageHeight;
        var scalingX = node.offsetWidth / pageWidth;
        if (scalingX > scalingY) scaling = scalingX; else scaling = scalingY;

        var newWidth = (node.offsetWidth / scaling);
        var newHeight = (node.offsetHeight / scaling);

        // Correct float to int rounding
        if (newWidth < pageWidth) {
            node.width = pageWidth;
        }
        else {
            node.width = newWidth;
        }

        if (newHeight < pageHeight) {
            node.height = pageHeight;
        }
        else {
            node.height = newHeight;
        }
        return node;
    }
}

function clearLineAnimation() {
    $('#svgContainer').children().each(function () {
        var pathHtml = $(this).html();
        $(this).html(parseSVG(pathHtml.replace('pathAnimation', 'tempcss')));
    });

}
function parseSVG(s) {
    var div = document.createElementNS('http://www.w3.org/1999/xhtml', 'div');
    div.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg">' + s + '</svg>';
    var frag = document.createDocumentFragment();
    while (div.firstChild.firstChild)
        frag.appendChild(div.firstChild.firstChild);
    return frag;
}
//function ResizeImageKeepAspectRatio(node, pageHeight, pageWidth, pageHeightWithHeaderFooter) {
//    
//    var resizeWidth = node.offsetWidth;
//    var resizeHeight =node.offsetHeight;
//    var maxWidth=pageWidth;
//    var maxHeight=pageHeight;



//    var w =resizeWidth/ pageWidth  ;
//    var h = resizeHeight/pageHeight  ;

//    node.width = w;
//    node.height = h;

//        return node;
//}




//#end

function isAuthenticate() {
    $.ajax({
        url: appRoot + "Login/checkSession", success: function (result) {
            if (result == false) {
                location.href = baseUrl + appRoot;
            }
        }
    });
    //if (isp != null)
    //{ isp.bindCableRightPop(); }
}

// Entity list

function GetEntityChild(e_type) {

    return GetChild(e_type.toUpperCase())

}
function GetChild(e_type) {

    var arrChilds = [];
    switch (e_type) {
        case "BUILDING":
            return ["AddStructure", "LocationEdit", "UploadImage"];
            break;
        case "POLE":
            return ["AddSpliceClosure", "AddAdb", "AddCdb", "LocationEdit", "UploadImage"];
            break;
        case "CDB":
            return ["AddSplitter", "InfoTemplate", "ItemAssociate", "LocationEdit", "UploadImage"];
            break;
        case "SPLICECLOSURE":
            return ["SplitCable", "ItemAssociate", "AddCloneEntity", "LocationEdit", "UploadImage"];
            break;
        case "TREE":
            return ["AddSpliceClosure", "AddAdb", "AddCdb", "AddCloneEntity", "Adit", "UploadImage"];
            break;
        case "MANHOLE":
            return ["AddSpliceClosure", "AddAdb", "AddCdb", "AddCloneEntity", "LocationEdit", "UploadImage"];
            break;
        case "ONT":
            return ["AddCustomer", "LocationEdit", "UploadImage"];
            break;
        case "WALLMOUNT":
            return ["AddSpliceClosure", "AddAdb", "AddCdb", "AddCloneEntity", "LocationEdit", "UploadImage"];
            break;
        case "STRUCTURE":
            return ["AddBDB", "BDBTemplate", "AddONT", "ONTTemplate", "AddCloneEntity", "ISPView", "LocationEdit", "UploadImage"];
            break;
        case "CABLE":
            return ["ParallelCable", "ConvertCableType", "ItemAssociate", "ONTTemplate", "AddCloneEntity", "ISPView", "LocationEdit", "UploadImage"];
            break;
        case "ADB":
            return ["AddSplitter", "InfoTemplate", "ItemAssociate", "AddCloneEntity", "LocationEdit", "UploadImage"];
            break;
        case "DUCT":
            return ["ItemAssociate", "AddCloneEntity", "LocationEdit", "UploadImage", "InsideCable"];
            break;
        case "TRENCH":
            return ["ItemAssociate", "AddCloneEntity", "LocationEdit", "UploadImage", "InsideDuct"];
            break;
        case "POD":
            return ["AddFMS", "InfoTemplate", "LocationEdit", "UploadImage"];
            break;
        case "MPOD":
            return ["AddFMS", "InfoTemplate", "LocationEdit", "UploadImage"];
            break;
    }
}
//function GetChild(e_type) {

//    var arrChilds = [];
//    switch (e_type) {
//        case "BUILDING":
//            return ["addStructure", "edit", "uploadImage"];
//            break;
//        case "POLE":
//            return ["addSpliceClosure", "addAdb", "addCdb", "edit", "uploadImage"];
//            break;
//        case "CDB":
//            return ["addSplitter", "infoTemplate", "itemAssociate", "edit", "uploadImage"];
//            break;
//        case "SPLICECLOSURE":
//            return ["splitCable", "itemAssociate", "addCloneEntity", "edit", "uploadImage"];
//            break;
//        case "TREE":
//            return ["addSpliceClosure", "addAdb", "addCdb", "addCloneEntity", "edit", "uploadImage"];
//            break;
//        case "MANHOLE":
//            return ["addSpliceClosure", "addAdb", "addCdb", "addCloneEntity", "edit", "uploadImage"];
//            break;
//        case "ONT":
//            return ["addCustomer", "edit", "uploadImage"];
//            break;
//        case "WALLMOUNT":
//            return ["addSpliceClosure", "addAdb", "addCdb", "addCloneEntity", "edit", "uploadImage"];
//            break;
//        case "STRUCTURE":
//            return ["addBDB", "BDBTemplate", "addONT", "ONTTemplate", "addCloneEntity", "ISPView", "edit", "uploadImage"];
//            break;
//        case "CABLE":
//            return ["ParallelCable", "convertCableType", "itemAssociate", "ONTTemplate", "addCloneEntity", "ISPView", "edit", "uploadImage"];
//            break;
//        case "ADB":
//            return ["addSplitter", "infoTemplate", "itemAssociate", "addCloneEntity", "edit", "uploadImage"];
//            break;
//        case "DUCT":
//            return ["itemAssociate", "addCloneEntity", "edit", "uploadImage", "InsideCable"];
//            break;
//        case "TRENCH":
//            return ["itemAssociate", "addCloneEntity", "edit", "uploadImage", "InsideDuct"];
//            break;
//        case "POD":
//            return ["addFMS", "infoTemplate", "edit", "uploadImage"];
//            break;
//        case "MPOD":
//            return ["addFMS", "infoTemplate", "edit", "uploadImage"];
//            break;

//    }

//}

function BindPlanningDetail() {


    var project_id = $('#ddl_ProjectSpeciCode').val();
    allcleardropdownforprojectspeci();
    ajaxReq('itemTemplate/GetPlanning', { ddlproject_id: project_id }, false, function (resp) {
        if (resp.Data.length > 0) {


            $('#ddl_PlanningSpeciCode').empty();
            $('#ddl_PlanningSpeciCode').append($("<option></option>").val('0').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_PlanningSpeciCode').append($("<option data-planningname = " + (value.planning_name) + "></option>").val(value.system_id).html(value.planning_code));

            });

        }
        else {
            $('#ddl_PlanningSpeciCode').empty();
            $('#ddl_PlanningSpeciCode').append($("<option></option>").val('0').html('-Select-'));


            //$('#project_name').val('');
            //$('#planning_name').val('');
            //$('#workorder_name').val('');
            //$('#purpose_name').val('');
        }
    }, true, true);

    $('#ddl_PlanningSpeciCode').trigger("chosen:updated");
    $('#project_name').val($('#ddl_ProjectSpeciCode').find(':selected').data('projectname'));

}
function allcleardropdownforprojectspeci() {


    $('#ddl_PlanningSpeciCode').empty();
    $('#ddl_WorkorderSpeciCode').empty();
    $('#ddl_PurposeSpeciCode').empty();
    $('#project_name').val('');
    $('#planning_name').val('');
    $('#workorder_name').val('');
    $('#purpose_name').val('');


    $('#ddl_PlanningSpeciCode').append($("<option></option>").val('0').html('-Select-'));
    $('#ddl_WorkorderSpeciCode').append($("<option></option>").val('0').html('-Select-'));
    $('#ddl_PurposeSpeciCode').append($("<option></option>").val('0').html('-Select-'));

    $('#ddl_PlanningSpeciCode, #ddl_WorkorderSpeciCode, #ddl_PurposeSpeciCode').trigger("chosen:updated");
}
function BindWorkCodeDetail() {

    var planning_id = $('#ddl_PlanningSpeciCode').val();

    $('#workorder_name').val('');
    $('#purpose_name').val('');

    $('#ddl_PurposeSpeciCode').empty();
    $('#ddl_PurposeSpeciCode').append($("<option></option>").val('0').html('-Select-'));


    ajaxReq('itemTemplate/GetWokorder', { ddlplanning_id: planning_id }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_WorkorderSpeciCode').empty();
            $('#ddl_WorkorderSpeciCode').append($("<option></option>").val('0').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_WorkorderSpeciCode').append($("<option data-workordername = " + (value.workorder_name) + "></option>").val(value.system_id).html(value.workorder_code));
            });

        }
        else {
            $('#ddl_WorkorderSpeciCode').empty();
            $('#ddl_WorkorderSpeciCode').append($("<option></option>").val('0').html('-Select-'));
            //$('#workorder_name').val('');

        }
    }, true, true);

    $('#ddl_WorkorderSpeciCode, #ddl_PurposeSpeciCode').trigger("chosen:updated");

    $('#planning_name').val($('#ddl_PlanningSpeciCode').find(':selected').data('planningname'));

}

function BindPurposeCodeDetail() {

    var workorder_id = $('#ddl_WorkorderSpeciCode').val();
    $('#purpose_name').val('');

    ajaxReq('itemTemplate/GetPurpose', { ddlworkorder_id: workorder_id }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_PurposeSpeciCode').empty();
            $('#ddl_PurposeSpeciCode').append($("<option></option>").val('0').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_PurposeSpeciCode').append($("<option data-purposename = " + (value.purpose_name) + "></option>").val(value.system_id).html(value.purpose_code));
            });


        }
        else {
            $('#ddl_PurposeSpeciCode').empty();
            $('#ddl_PurposeSpeciCode').append($("<option></option>").val('0').html('-Select-'));
        }
    }, true, true);

    $('#ddl_PurposeSpeciCode').trigger("chosen:updated");
    $('#workorder_name').val($('#ddl_WorkorderSpeciCode').find(':selected').data('workordername'));

}

function setColorPicker(className) {
    $("." + className).spectrum({
        allowEmpty: true,
        //color: "#ECC",
        showInput: true,
        containerClassName: "full-spectrum",
        //showInitial: true,
        showPalette: true,
        showSelectionPalette: true,
        showAlpha: true,
        maxPaletteSize: 10,
        preferredFormat: "hex",
        localStorageKey: "spectrum.demo",
        move: function (color) {

        },
        show: function () {

        },
        beforeShow: function () {

        },
        hide: function (color) {

        },
        updateUI: function () {
             
        },

        palette: [
            ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)",
            "rgb(204, 204, 204)", "rgb(217, 217, 217)", "rgb(255, 255, 255)"],
            ["rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
            "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)"],
            ["rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
            "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
            "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
            "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
            "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
            "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
            "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
            "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
            "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
            "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
        ]
    });

};

function getLayerTltle(layerName) {
    var layer_title;
    var postData = { layerName: layerName };
    ajaxReq('Main/GetLayerTitle', postData, false, function (resp) {
        if (resp.status == "OK") {
            layer_title = resp.result
        }
    }, false, false);
    return layer_title;
}
function getLayerName(layerTitle) {
    var layer_name;
    var postData = { layerTitle: layerTitle };
    ajaxReq('Main/GetLayerName', postData, false, function (resp) {
        if (resp.status == "OK") {
            layer_name = resp.result
        }
    }, false, false);
    return layer_name;
}


// To avoid repetitive false logs entries. Disable update/Save buttons untill there is no change in any values.
function EnableDisableUpdateButton(frmId, btnID) {
    var allElements = document.querySelectorAll("#" + frmId + " input:not(.chosen-search-input), #" + frmId + " select,#" + frmId + " textarea");

    for (var i = 0; i < allElements.length; i++) {
        if (!inputs.includes(allElements[i])) {
            inputs.push(allElements[i]);
        }
    }
    for (var el of inputs) {
        if (el.oldValue == undefined) {
            el.oldValue = el.value + el.checked;
        }
    }
    // Enable/Disable update buttons logic
    //var setEnabled;
    //(setEnabled = function () {
    //    var e = true;
    //    for (var el of inputs) {
    //        if (el.oldValue !== (el.value + el.checked)) {
    //            e = false;
    //            break;
    //        }
    //    }
    //    document.getElementById(btnID).disabled = e;
    //})();
    //document.oninput = setEnabled;
    //document.onchange = setEnabled;
    document.getElementById(btnID).disabled = false;

}
// Get dynamically loaded inputs.
function appendDynamicInputs(formID) {
    var xyz = document.querySelectorAll("#" + formID + " input:not(.chosen-search-input), #" + formID + " select");
    /////////////
    for (var i = 0; i < xyz.length; i++) {
        if (!inputs.includes(xyz[i])) {
            inputs.push(xyz[i]);
        }
    }
    for (var el of inputs) {
        if (el.oldValue == undefined) {
            el.oldValue = el.value + el.checked;
        }
    }
}
//History popup
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

function EntityAuditReport(downloadEntity) {
    window.location = appRoot + 'Audit/' + downloadEntity;
}
function SiteAuditReport(downloadEntity) {
    window.location = appRoot + 'Audit/' + downloadEntity;
}
function LMCAuditReport(downloadEntity) {
    window.location = appRoot + 'Audit/' + downloadEntity;
}
function AccessoriesAuditReport(downloadEntity) {
    window.location = appRoot + 'Audit/' + downloadEntity;
}
//End History popup
function upperCaseNetWorkId() { $("#network_id").val($("#txtMaskedNetworkId").dxTextBox("option", "text").toUpperCase()); }


function getElementPosition(objElement, objParent) {
    var _offSetX = 0, _offSetY = 0;

    if (objParent) {
        _offSetX = (objElement.offset().left - objParent.offset().left) + ((objElement.width() + 2) / 2);
    }
    else {
        _offSetX = objElement.offset().left + ((objElement.width() + 2) / 2);
    }

    if (objParent) {
        _offSetY = (objElement.offset().top - objParent.offset().top) + ((objElement.height() + 2) / 2);
    }
    else {
        _offSetY = objElement.offset().top + ((objElement.height() + 2) / 2);
    }

    return { x: _offSetX, y: _offSetY };
}
function showHideShaftFloor(obj) {
    if (obj == 'ddlUnitList') {
        $('#ddlShaftlist').val('').trigger('chosen:updated');
    }
}
function getAllParentInFloor() {
    $('#divParentEntity').hide();
    $("#ddlUnitList").val('').trigger("chosen:updated");
    if ($('#ddlAssociationtype').val() == 'Floor' && $('#ddlFloorlist').val() != '') {
        ajaxReq('ISP/getAllParentInFloor', { structureId: parseInt($('#ddlStructureList').val()), floorId: parseInt($('#ddlFloorlist').val()), parentType: 'UNIT' }, false, function (resp) {
            var optHTML = '<option value="">Select Unit</option>';
            if (resp.length > 0 && $("#ddlUnitList").length > 0) {
                for (var i = 0; i < resp.length; i++) {
                    optHTML += '<option value=' + resp[i].entity_system_id + ' data-entity-type="UNIT" data-network-id="' + resp[i].network_id + '">' + resp[i].network_id + '</option>';
                }
                $("#ddlUnitList").html(optHTML).trigger("chosen:updated");
            } else if (resp.length == 0 && $("#ddlUnitList").length > 0) {
                $("#ddlUnitList").html(optHTML).val('').trigger("chosen:updated");
            }
        }, false, false);
        $('#divParentEntity').show();
    }
}

function copyPasteNumberOnly() {
    $("input.numbers-only").on("paste", function () {
        var thisvalue = this;

        setTimeout(function () {
            var OnlyNumber = new RegExp(/^[0-9]{1,10}$/);
             
            if (!OnlyNumber.test(thisvalue.value)) {
                thisvalue.value = "";
            }
        }, 0);
    });
}

function ExportDivToPDF(controlId, title, subTitle, jsPlumbConnector) {
    showProgress();
    var targetElem = $("#" + controlId);
    var filename = title.replace(" ", "_") + ".pdf";
    //this is for handle svg image
    if (jsPlumbConnector) {
        var elements = targetElem.find('svg').map(function () {
            var svg = $(this);
            var canvas = $('<canvas></canvas>').css({ position: 'absolute', fill: 'none', left: svg.css('left'), top: svg.css('top') });
            svg.replaceWith(canvas);
            // Get the raw SVG string and curate it
            var content = svg.wrap('<p></p>').parent().html();
            svg.unwrap();
            canvg(canvas[0], content);
            return {
                svg: svg,
                canvas: canvas
            };
        });
    }
    //end
    var HTML_Width = $(targetElem).width();
    var HTML_Height = $(targetElem).height();

    var top_left_margin = 15;
    var PDF_Width = HTML_Width + (top_left_margin * 2);
    var PDF_Height = HTML_Height + (top_left_margin * 2);
    var canvas_image_width = HTML_Width;
    var canvas_image_height = HTML_Height;

    var totalPDFPages = Math.ceil(HTML_Height / PDF_Height) - 1;

    // At this point the container has no SVG, it only has HTML and Canvases.
    html2canvas($(targetElem)[0], {
        width: $(targetElem)[0].scrollWidth, height: $(targetElem)[0].scrollHeight, quality: 2, scale: 2
    }, { allowTaint: true, useCORS: true }).then(function (canvas) {
        // Put the SVGs back in place
        if (jsPlumbConnector) {
            elements.each(function () {
                this.canvas.replaceWith(this.svg);
            });
        }
        //console.log(canvas.height + " " + canvas.width);
        canvas.getContext('2d');
        var imgData = canvas.toDataURL("image/jpeg", 1.0);
       
      
        var pdf = new jsPDF('p', 'pt', [PDF_Width, PDF_Height]);
        pdf.deletePage(1);
        pdf.addPage(PDF_Width, canvas_image_height + 100);
        pdf.text(title, (canvas_image_width / 2), 20, 'center');
        //pdf.text('JIOPARTNERFIBER', 10, 20);
        pdf.addImage($("#hdn_ClientLogoImageBytesForWeb").val(), 'JPEG', 10, 3, 100, 25);
        pdf.text("Created On: " + GetFormattedDateTime(new Date()), (pdf.internal.pageSize.width - 10), 20, 'right');
        if (subTitle != null && subTitle != '') {
            pdf.text(subTitle, (canvas_image_width / 2), top_left_margin + 35, 'center');
        }
        pdf.addImage(imgData, 'JPG', top_left_margin, top_left_margin + 40, canvas_image_width, canvas_image_height);

        //for (var i = 1; i <= totalPDFPages; i++) {
        //    pdf.addPage(PDF_Width, PDF_Height);

        //    pdf.addImage(imgData, 'JPG', top_left_margin, -(PDF_Height * i) + (top_left_margin * 4), canvas_image_width, canvas_image_height);
        //}

        pdf.setLineWidth(1);
        pdf.rect(10, 30, PDF_Width - 20, (canvas_image_height + 40));
       // pdf.text('Powered by Lepton Software', 10, canvas_image_height + 90);
        pdf.text('', 10, canvas_image_height + 90);
        pdf.height = canvas_image_height + 100;
        setTimeout(function () {
            //pdf.save("LogicalDiagram.pdf");
            pdf.save(filename);
            hideProgress();
        }, 0);
    });
};

// SIT CUSTOMER START--  
function showHideAddCustomerIcon() {
    // 
    if ($('#ddlSiteType').val() == 'Exclusive' && $('#dvSiteCustomerGrid' + ' tbody tr').length == 0) {
        //$('#addSiteCustomer').hide();
        $('#exportCustomer').hide();
        $('#addSiteCustomer').show();
        $('#addSiteCustomer').css('text-align', 'center');
    }
    else if ($('#ddlSiteType').val() == 'Exclusive' && $('#dvSiteCustomerGrid' + ' tbody tr').length > 0) {
        $('#addSiteCustomer').hide();

        $('#exportCustomer').show();


        //  $('#addSiteCustomer').css('text-align', 'right');
        // $('#addSiteCustomer').show();

    }
    else if ($('#ddlSiteType').val() == 'Shared' && $('#dvSiteCustomerGrid' + ' tbody tr').length > 0) {
        $('#exportCustomer').show();
        $('#addSiteCustomer').show();
        $('#addSiteCustomer').css('text-align', 'right');
    }
    else if ($('#ddlSiteType').val() == 'Shared' && $('#dvSiteCustomerGrid' + ' tbody tr').length == 0) {
        $('#exportCustomer').hide();
        $('#addSiteCustomer').show();
        $('#addSiteCustomer').css('text-align', 'center');


    }
    else {
        $('#addSiteCustomer').hide();
        $('#exportCustomer').hide();
    }
}
var SiteCustomer = function () {
    var app = this;
    this.tableScroll = { top: 0, left: 0 };

    this.refreshStructureInfo = function () {
        // 
        app.tableScroll.top = $('.table-scroll').scrollTop();
        app.tableScroll.left = $('.table-scroll').scrollLeft();
        $('#ancrStructureInfo').trigger("click");

    }

    this.SiteCustomerInfo = {

        addcustomer: function (_systemId, _siteId, _entitytType, _lmcType, _structureId) {
            // 
            var modelClass = getPopUpModelClass('');
            var ddlLMCType = $('#ddlLMCType').val();
            var titleText = 'Site Customer';
            var formURL = 'Library/AddSiteCustomer';
            if (_siteId == 0) {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_071);//Please Create a Site First!
                return false;
            }
            else {
                popup.LoadModalDialog(app.ChildModel, formURL, { systemId: _systemId, siteId: _siteId, entityType: _entitytType, lmcType: _lmcType, structureId: _structureId, featureName: 'SiteCustomer' }, titleText, modelClass, function () {

                });
            }
        },
        addCustomerResponse: function (_systemId, _siteId, _lmcType, _childModel) {

            if (_systemId > 0) {
                //alert('Customer has been updated successfully!');
                if (_childModel == "") {
                    alert(MultilingualKey.SI_OSP_CUS_JQ_FRM_003, 'success', 'success');//Customer updated successfully!
                }
                else {
                    alert(MultilingualKey.SI_OSP_CUS_JQ_FRM_003); //Customer updated successfully!
                }

            } else { alert(MultilingualKey.SI_OSP_CUS_JQ_FRM_004); }//Customer updated successfully!
            if ((_childModel == "" || _childModel == null) && _systemId > 0) {
                $('#closeModalPopup').trigger("click");
                app.refreshStructureInfo();

            }
            else {
                $('#ChildPopUp #closeChildPopup').trigger("click");
                ajaxReq('Library/SiteCustomerList', { systemId: _systemId, siteId: _siteId, lmcType: _lmcType }, true, function (resp) {
                    $('#dvSiteCustomerlist').html(resp);
                    showHideAddCustomerIcon();

                    //if ($('#ddlSiteType').val() == 'Exclusive' && $('#dvSiteCustomerGrid' + ' tbody tr').length > 0) {
                    //    $('#addSiteCustomer').hide();
                    //    $('#exportCustomer').show();
                    //} 
                    //else {
                    //    $('#addSiteCustomer').show();
                    //    $('#exportCustomer').show();
                    //    $('#addSiteCustomer').css('text-align', 'right');


                    // }

                }, false, true);
            }
        },
        deleteSiteCustomer: function (_systemId, _siteId, _lmcType) {
            // 
            var ListSystemIds = [];
            ListSystemIds.push(_systemId)
            if (ListSystemIds.length > 0) {
                var func = function () {
                    ajaxReq('Main/DeleteSiteCustomer', { ListSystem_Id: ListSystemIds }, true, function (j) {
                        alert(j.objPM.message, 'success', 'success');
                        ajaxReq('Library/SiteCustomerList', { systemId: _systemId, siteId: j.site_id, lmctype: j.lmc_type }, true, function (resp) { // To refresh the siteCustomer Grid
                            $('#dvSiteCustomerlist').html(resp);
                            showHideAddCustomerIcon();

                        }, false, true);
                        // $('#liDocumentUpload_' + _systemId).remove();
                    }, true, true)
                };
                showConfirm(MultilingualKey.SI_OSP_CUS_JQ_FRM_005, func);//Are you sure you want to delete this customer?
            } else {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_147, 'warning');//Please select any customer!
            }
        },
        attachment: {
            upload: function (systemId, entityType, siteId) {
                if ($('#SiteCustomerDocUpload').val() == '') {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_096);//Please select a file to upload!
                }
                else if ($('#SiteCustomerDocUpload').val().length > 60) {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_062);//Selected file name should be less than 60 character length!
                }
                else {
                    app.SiteCustomerInfo.attachment.uploadDocumentFile(systemId, entityType, siteId);
                }
            },
            uploadDocumentFile: function (systemId, entityType, siteId) {
                // 
                var frmData = new FormData();
                var filesize = $('#hdnMaxFileUploadSizeLimit').val();
                var uploadedfile = $('#SiteCustomerDocUpload').get(0).files[0];
                var Sizeinbytes = filesize * 1024;
                if (!app.SiteCustomerInfo.attachment.validatefilename(uploadedfile.name)) { return false; }
                else if ($('#ulSiteCustomerGrid  tbody tr[data-image-name="' + uploadedfile.name + '"]').length > 0) {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_055);//File already exist,please choose the different file!
                    return false;
                }
                else if ($('#SiteCustomerDocUpload').get(0).files[0].size > Sizeinbytes) {
                    alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_FRM_109, (filesize / 1024).toFixed(2))), 'warning');
                    //File size is too large. Maximum file size allowed is
                }
                else {

                    if (!app.SiteCustomerInfo.attachment.validateDocumentFileType()) { return false; }
                    frmData.append(uploadedfile.name, uploadedfile);
                    frmData.append('system_Id', systemId);
                    frmData.append('entity_type', entityType);
                    frmData.append('feature_name', 'SiteCustomer');

                    ajaxReqforFileUpload('Main/UploadDocument', frmData, true, function (resp) {
                        if (resp.status == "OK") {
                            app.SiteCustomerInfo.attachment.getAttachmentFiles(systemId, entityType);
                            //alert(resp.message, 'success', 'success');
                            alert(resp.message);
                            if ($('#SiteCustomerDocUpload')[0] != undefined)
                                $('#SiteCustomerDocUpload')[0].value = '';
                        }
                        else {
                            alert(resp.message, 'warning');
                        }

                    }, true);
                }
            },
            getAttachmentFiles: function (_system_Id, _entity_type) {
                ajaxReq('Library/GetSiteCustomerAttachment', { system_Id: _system_Id, entity_type: _entity_type, featureName: 'SiteCustomer' }, true, function (jResp) {
                    $('#divSiteCustomerAttachment').html(jResp);
                }, false, false, true);
            },
            validateDocumentFileType: function () {
                var validFilesTypes = ["dwg", "pdf", "jpeg", "jpg", "doc", "docx", "xls", "xlsx", "csv", "vsd", "ppt", "pptx", "png"];
                var file = $('#SiteCustomerDocUpload').val();
                var filepath = file;
                return app.SiteCustomerInfo.attachment.ValidateFileType(validFilesTypes, filepath);
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
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_059 + " " +           //Invalid File. Please upload a File with
                      " extension:\n\n" + validFilesTypes.join(", "), 'warning');
                }
                return isValidFile;
            },
            deleteFile: function (_systemId) {
                var ListSystemIds = [];
                // 
                ListSystemIds.push(_systemId)
                if (ListSystemIds.length > 0) {
                    var func = function () {
                        ajaxReq('Main/DeleteAttachmentFile', { system_Id: _systemId }, true, function (j) {
                            //alert(j.message, 'success', 'success');
                            alert(j.message);
                            $('#liDocumentUpload_' + _systemId).remove();
                        }, true, true)
                    };
                    showConfirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_004, func);//Are you sure you want to delete this file?'
                } else {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_005, 'warning');//Please select any file!
                }
            },
            downLoadFile: function (_systemId, _entityType) {
                // 
                var listPathName = [];
                var _entity_type = $('#hdEntityType').val();
                window.location = appRoot + 'Main/DownloadAll?system_Id=' + _systemId + '&entity_type=' + _entityType + '&entityFeature=SiteCustomer';
            },
            validatefilename: function (filename) {
                var isValid = true;
                if (filename.split('.').length > 2) {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_056);//Multiple dots (.) are not allowed!
                    isValid = false;
                } else if (filename.split('..').length > 1) {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_114);//Consucative dots (.) are not allowed!
                    isValid = false;
                }
                var regex = /^[0-9a-zA-Z_\.\s]*$/;
                if (!regex.test(filename)) { isValid = false; alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_063); }//Please enter valid file name.Only _ and . special character allowed!
                return isValid;
            }

        },
        clear: function () {
            $("#SiteCustomerDocUpload").val('');
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
                // 
                var emID = $("#" + startdateid).val();
                if (emID != "") {
                    $("#" + startdateid).removeClass('input-validation-error').removeClass('field-validation-error').addClass('field-validation-valid').html('');
                }
                else {
                    $("#" + startdateid).addClass('input-validation-error');
                }
            },
            disableFunc: function (date) {
                // chkDisabled 0 for backdate 1 for future date

                if (chkDisabled == '1') {
                    if (!isFutureDateAllowed) {
                        var now = new Date();
                        now.setDate(now.getDate());
                        if (date.getTime() > now.getTime()) {
                            return true;
                        }
                    }
                }
                else {
                    if (!isFutureDateAllowed) {
                        var now = new Date();
                        now.setDate(now.getDate() - 1);
                        if (date.getTime() < now.getTime()) {
                            return true;
                        }
                    }
                }
            }
        });

    }
    this.ddlChangeRemoveCss = function (selectorId, emType) {
        //         
        var emID = $("#" + selectorId + "_chosen");
        if (emID.children('a').text().trim() != "Select " + emType) {
            emID.removeClass('input-validation-error').next('span').removeClass('field-validation-error').addClass('field-validation-valid').html('');
        }
        else {
            emID.addClass('input-validation-error');
        }
    }
}
function getLoopDetailsForCable(_system_id) {

    ajaxReq('Library/getLoopDetailsForCable', { cable_system_id: _system_id }, true, function (resp) {
        $("#LoopDetails").html(resp);
        $("#LoopDetails").css('background-image', 'none');
        if (isp != null) { $('#tblLoopDetailForCable').addClass('webGrd'); }

    }, false, false);
}

function getMultilingualStringValue(p_key) {
    var elm = '<div></div>';
    var Mkey = $(elm).html($.parseHTML($.parseHTML(p_key)[0].textContent))[0].innerHTML;
    return Mkey;
}
function getAutoFiberLinkId(searchText) {
     
    $('#' + searchText).autocomplete({

        source: function (request, response) {
            var res = ajaxReq('main/GetAutoFiberLinkId', { SearchText: request.term }, true, function (data) {
                if (data.geonames.length == 0) {
                    var result = [
                        {
                            label: MultilingualKey.SI_GBL_GBL_JQ_GBL_001
                        }
                    ];
                    response(result);
                }
                else {
                    response($.map(data.geonames, function (item) {
                        return {
                            label: item.link_id, value: item.link_id
                        }//
                    }))
                }
            }, true, false);

        },
        minLength: 2,
        select: function (event, ui) {
            if (ui.item.label == MultilingualKey.SI_GBL_GBL_JQ_GBL_001) {
                // this prevents "no results" from being selected
                event.preventDefault();
            }
            else {
                //event.preventDefault();
                // app.gtype = ui.item.geomType;
                if (ui.item.entityName != null) {
                    $('#txtCableFiberLinkId').val(ui.item.value + ':' + ui.item.label);
                }
                else {
                    $('#txtCableFiberLinkId').val(ui.item.label + ':');
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
function pasteNumberOnly(thisvalue) {
     
    setTimeout(function () {
        var OnlyNumber = new RegExp(/^[0-9]{1,10}$/);
         
        if (!OnlyNumber.test(thisvalue.value.trim())) {
            thisvalue.value = "";
        }
        else {
            thisvalue.value = thisvalue.value.trim();
        }

    }, 0);
}
function removeWhiteSpace(thisvalue) {
    setTimeout(function () {
        thisvalue.value = thisvalue.value.trim();
    }, 0);
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
function updatePortStatus(response) {
    if (response.isGridCalling) {
        updateStatusInGrid(response);
    } else {
        splicing.resetEndPoint(response);
        if (conBuilder != null) { conBuilder.changePortStatus(response) }
    }
    if ($('#dvModalBody #system_id').length > 0) { $("#FiberDetail").html(''); si.GetCableFiberDetail($('#dvModalBody #system_id').val());}
}
function patchingSuccess(resp) {
    if (resp.isEquipmentPatching && conBuilder != null) { conBuilder.savePatchConnection(resp); }
    else { splicing.patchSuccess(resp); }
}
function updateStatusInGrid(resp) {
    var _status = resp.portStatus;
    var _comments = resp.comment;
    var portNo = resp.core_port_number.split(',');
    var _selectedStatus = resp.listPortStatus.filter(m=>m.system_id == _status);
    if (_selectedStatus) {
        for (var i = 0; i < portNo.length; i++) {
            $('#PortStatus_' + portNo[i]).text(_selectedStatus[0].status);
            $('#PortComments_' + portNo[i]).text(_comments);
        }
    }
    $('#closeChildPopup').trigger("click");
}
function getPopInBuffer(tower, distance) {
    var popAssociated = $("#popAssociated");
    popAssociated.empty();
    ajaxReq('Tower/GetPopInBuffer', { towerId: tower, distance: distance }, false,
        function (resp) {
            if (resp.length > 0) {
                $.each(resp, function (data, value) {
                    $(popAssociated).append($("<option></option>").val(value.system_id).html(value.network_id));
                    $("#btnAssociatePOD").removeClass("disabled");
                });
                $(popAssociated).prepend($("<option selected=true></option>").val("0").html("-Select-"));

            } else {
                $(popAssociated).prepend($("<option selected=true></option>").val("0").html("-Select-"));
                $("#btnAssociatePOD").addClass("disabled");

            }

        }, true, true);

    //bindModel($('#ddlBrand').val());

}
function saveAssociation(podId, towerID) {
    if (podId == 0) { alert("Please choose POP", 'warning'); return; }
    ajaxReq('Tower/saveAssociation', { podId: podId, towerId: towerID }, false,
        function (resp) {
            if (resp.status == "OK") {
                GetAssociations(towerID);
                alert(resp.error_msg, 'success');
            }
            else if (resp.status == "DUPLICATE_EXIST") {
                alert(resp.error_msg, 'warning');

            }
        }, true, true);
}

function DeAssociatePop(podId, towerID) {
    showConfirm(MultilingualKey.SI_OSP_GBL_GBL_FRM_094, function () {
        ajaxReq('Tower/DeAssociatePop', { podId: podId, towerId: towerID }, false,
        function (resp) {
            if (resp.status == "OK") {
                GetAssociations(towerID);
                alert(resp.error_msg, 'success');
            }
            else if (resp.status == "Failed") {
                alert(resp.error_msg, 'warning');

            }
        }, true, true);
    });
}

function GetAssociations(towerId) {
    ajaxReq('Tower/GetAssociation', { towerId: towerId }, true, function (resp) {
        $("#PopAssociateList").html(resp);
    }, false, false);
}
function getAdditionAttributes(_systemId, _entityType, obj) {
    if ($(obj).attr('data-is-loaded') == '0') {
        $('#dvAdditionProgress').show();
        ajaxReq('Library/AdditionalAttributes', { systemId: parseInt(_systemId), entityType: _entityType }, true, function (resp) {
            $("#divAdditionalAttributes").html(resp);
            $(obj).attr('data-is-loaded', '1');
        }, false, false);
    }
}
function setDateTimeCalendarAttributes(startdateid, startdateimgid) {
    Calendar.setup({
        inputField: startdateid,   // id of the input field
        button: startdateimgid,
        ifFormat: "%d-%b-%Y",       // format of the input field datetime format %d-%b-%Y %I:%M %p
        showsTime: false,
        timeFormat: "12",
        weekNumbers: false
    });
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

//## Additional Attribute region to serialize for saving
class AdditionalAttributesUtility {
    SetJsonValue = function (formOtherAttribute, hdnOtherInfo) {
        //set the correct system_id for the entity
        //$('#hdnSystemID').val($('#infoTB').attr('att_systemid'));
        $('#hdnSystemID').val($('#system_id').val());
        //var obj = $(formOtherAttribute).serializeJSON();              //--serialize as json if it is form
        var obj = $(formOtherAttribute).find('select, textarea, input').serializeJSON();   //--serialize as json if it is not a form
        var jsonString = JSON.stringify(obj);
        if (jsonString != null) {
            $(hdnOtherInfo).val(jsonString);
            return true;
        }
        else {
            return false;
        }
    }
}
//## end region

$('.textarea-maxlen-validate').on('keydown',function (event) {
    var key = event.keyCode;
    var maxLength = $(this).attr('maxlength');
    var length = this.value.length;
    var remain = maxLength - (length);
    if (length >= maxLength && key != 8) {
        event.preventDefault();
    }
});

var downloadTimeout;
var checkDownloadCookie = function () {
    //console.log(getCookie("downloadStarted"));
    if (getCookie("downloadStarted") == 1) {
        //console.log(getCookie("downloadStarted") + "RT");
        setCookie("downloadStarted", false, 100); //Expiration could be anything... As long as we reset the value
    } else {
        downloadTimeout = setTimeout(checkDownloadCookie, 1000); //Re-run this function in 1 second.
    }
}

var setCookie = function (name, value, expiracy) {
        var exdate = new Date();
        exdate.setTime(exdate.getTime() + expiracy * 1000);
        var c_value = escape(value) + ((expiracy == null) ? "" : "; expires=" + exdate.toUTCString());
        document.cookie = name + "=" + c_value + '; path=/';
    };

    var getCookie = function (name) {
        var i, x, y, ARRcookies = document.cookie.split(";");
        for (i = 0; i < ARRcookies.length; i++) {
            x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
            y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
            x = x.replace(/^\s+|\s+$/g, "");
            if (x == name) {
                return y ? decodeURI(unescape(y.replace(/\+/g, ' '))) : y; //;//unescape(decodeURI(y));
            }
        }
};

function getFormattedNumber(number) {
    
    console.log("Format Type:" + $('#hdnNumberFormatType').val());
    if ($('#hdnNumberFormatType').val().toUpperCase() == "SAARC")
        return number.toString().split('.')[0].length > 3 ? number.toString().substring(0, number.toString().split('.')[0].length - 3).replace(/\B(?=(\d{2})+(?!\d))/g, ",") + "," + number.toString().substring(number.toString().split('.')[0].length - 3) : number.toString();
    else if ($('#hdnNumberFormatType').val().toUpperCase() == "EUROPE")
        return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function UpdateFormattedNumber(tblID, applyOnAllColumns, excludeColumns)
{
    let applyOnSelectedColumns = false;
    if (applyOnAllColumns == null || applyOnAllColumns == undefined)
        applyOnAllColumns = true;
    if (excludeColumns == null || excludeColumns == undefined)
        applyOnAllColumns = true;

    const table = document.getElementById(tblID);
    if (table != null && table != undefined)
    {
        //Get Header Information of Tables==============================================================
        var table1 = document.getElementById(tblID);
        var headerRow = table1.querySelector("thead tr");
        var headers = headerRow.getElementsByTagName("th");
        var arrExcludeColumns = excludeColumns;//Sample Values ["JFP Total Capex", "Cost Per Unit (Rs)"];
        var headerIndex = [];
        var headerNames = [];
        for (var i = 0; i < headers.length; i++) {
            if ($.inArray(headers[i].textContent, arrExcludeColumns) !== -1)
            {
                headerIndex.push(i);
                headerNames.push(headers[i].textContent);
            }
        };

        // Loop through the rows
        for (let rowIndex = 0; rowIndex < table.rows.length; rowIndex++)
        {
            const row = table.rows[rowIndex];

            // Loop through the cells in the row
            for (let cellIndex = 0; cellIndex < row.cells.length; cellIndex++)
            {
                //============In case we do  need to apply formatting to only selected columns  
                if (applyOnAllColumns == false && $.inArray(cellIndex, headerIndex) !== -1)
                {
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