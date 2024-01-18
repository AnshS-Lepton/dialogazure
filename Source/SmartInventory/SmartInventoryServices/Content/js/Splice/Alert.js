
$(document).ready(function () {   
    window.alert = function (msg, type, header) {
    
            showMessageBox(msg, type, header);
       
    }
    window.confirm = function (msgTxt, func, CnclFunc, okTxt, CnclTxt) {
        showConfirm(msgTxt, func, CnclFunc, okTxt, CnclTxt);
    }
});

function showMessageBox(msgTxt, type, header) {
    if (document.getElementById('alertMsgBox'))
        hideAlert();
    
    var l, t;
    // l = (window.innerWidth - 350) / 2;
    // t = (window.innerHeight - 70) / 2;
    l = window.innerWidth / 2 - 382 / 2;
    t = window.innerHeight / 2 - 155 / 2;
    var DivContent = document.createElement('div');
    DivContent.innerHTML = AlertMessage(msgTxt, l, t, type, header);
    document.body.appendChild(DivContent.firstChild);
    $('#alertMsgDiv').draggable({ scroll: false, handle: ".infoHeading", containment: "window" });


}

function AlertMessage(msg, lft, top, type, header) {

    type = type == undefined ? 'success' : type;
    header = header == undefined ? MultilingualKey.GBL_GBL_GBL_JQ_GBL_001 : header;
    var HtmlContent = '';
   
         HtmlContent = '<div id="alertMsgBox"><div id="alertMsgDiv" style="left:' + lft + 'px;top:' + top + 'px;" class="bootbox a_modal fade in ' + type + ' alertContainer"><div class="a_modal-dialog"><div class="a_modal-content"><div class="a_modal-body"><button onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button close" type="button" style="margin-top: 0px; margin-right: 5px;">' +
       '×</button><div class="bootbox-body"><div class="infoHeading"><strong>' + header + '</strong></div><div class="messageBodyBox"> <table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td class="InformationImg"></td>' +
       '<td style="font-size: 14px;padding-right: 10px;">' + msg + ' </td></tr></table></div></div></div><div class="a_modal-footer"><button onclick="hideAlert(\'alertMsgBox\');" class="a_btn btn-small btn-primary" type="button" data-bb-handler="ok" style="width: 90px">' + MultilingualKey.GBL_GBL_GBL_JQ_GBL_002 + '</button></div></div></div></div></div>';
 
    return HtmlContent;
}


function hideAlert(elementId) {
    if (elementId != undefined) {
        var objPU = document.getElementById(elementId);
        document.body.removeChild(objPU);
    }
    $('#alertBackOverLay').fadeOut();
}


function confirmMessage(msg, lft, top, okTxt, CnclTxt) {
    var HtmlContent = '<div id="alertMsgBox"><div id="alertMsgDiv" style="left:' + lft + 'px;top:' + top + 'px;" class="bootbox a_modal  fade in"><div class="a_modal-dialog"><div class="a_modal-content"><div class="a_modal-body"><button onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button close" type="button" style="margin-top: 0px;margin-right: 5px;" >' +
     'x</button><div class="bootbox-body"><div class="infoHeading"><strong>'+MultilingualKey.SI_GBL_GBL_JQ_FRM_015+'</strong></div><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td class="confirmationImg"></td>' +
     '<td style="font-size: 14px; padding:10px;">' + msg + ' </td></tr></table></div></div><div class="a_modal-footer"><button onclick="hideAlert(\'alertMsgBox\');" id="_ConfirmBtn" class="a_btn btn-small btn-primary" type="button" data-bb-handler="ok" style="width: 70px">' + okTxt + '</button><button onclick="hideAlert(\'alertMsgBox\');" id="_CancelBtn" class="a_btn btn-small btn-danger" type="button" data-bb-handler="ok" style="width: 70px">' + CnclTxt + '</button></div></div></div></div></div>';
    
    return HtmlContent;
}

function showConfirm(msgTxt, func, CnclFunc, okTxt, CnclTxt) {
    
    if (!CnclFunc) CnclFunc = function () { };
    if (!okTxt) okTxt = MultilingualKey.GBL_GBL_GBL_JQ_GBL_002;
    if (!CnclTxt) CnclTxt = MultilingualKey.GBL_GBL_GBL_GBL_GBL_006;
    if (document.getElementById('alertMsgBox'))
        hideAlert();

    var l, t;
    l = (window.innerWidth - 350) / 2;
    t = (window.innerHeight - 105) / 2;
    var DivContent = document.createElement('div');
    DivContent.innerHTML = confirmMessage(msgTxt, l, t, okTxt, CnclTxt);
    document.body.appendChild(DivContent.firstChild);
    //attachUnAttachEvt($('#alertMsgBox div a.OK'), 'click', func);  
    attachUnAttachEvt($('#_ConfirmBtn'), 'click', func);
    attachUnAttachEvt($('#_CancelBtn'), 'click', CnclFunc);
    $('#alertMsgDiv').draggable({ scroll: false, handle: ".infoHeading", containment: "window" });
}

function attachUnAttachEvt(elemnt, Evt, func) {
    elemnt.unbind(Evt);
    elemnt.bind(Evt, func);
}


