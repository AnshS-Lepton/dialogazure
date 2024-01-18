
$(document).ready(function () {
    window.alert = function (msg) { showMessageBox(msg); }
    window.confirm = function (msgTxt, func, CnclFunc, okTxt, CnclTxt) {
        showConfirm(msgTxt, func, CnclFunc, okTxt, CnclTxt);
    }
});

function showMessageBox(msgTxt) {
    if (document.getElementById('alertMsgBox'))
        hideAlert();

    var l, t;
    // l = (window.innerWidth - 350) / 2;
    // t = (window.innerHeight - 70) / 2;
    l = window.innerWidth / 2 - 382 / 2;
    t = window.innerHeight / 2 - 155 / 2;
    var DivContent = document.createElement('div');
    DivContent.innerHTML = AlertMessage(msgTxt, l, t);
    document.body.appendChild(DivContent.firstChild);
}

function AlertMessage(msg, lft, top) {
    var HtmlContent = '<div id="alertMsgBox"><div  style="left:' + lft + 'px;top:' + top + 'px;" class="bootbox a_modal  in"><div class="a_modal-dialog"><div class="a_modal-content"><div class="a_modal-body"><button onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button close" type="button" style="margin-top: 4px; margin-right: 5px;">' +
    '×</button><div class="bootbox-body"><div class="infoHeading">'+MultilingualKey.Information+'</div><div style="overflow:auto; padding-left:10px;"> <table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td class="InformationImg"></td>' +
    '<td>' + msg + ' </td></tr></table></div></div></div><div class="a_modal-footer"><button onclick="hideAlert(\'alertMsgBox\');" class="btn btn-primary" type="button" data-bb-handler="ok" style="width: 90px">' + MultilingualKey.OK + '</button></div></div></div></div></div>';
    return HtmlContent;
}


function hideAlert(elementId) {
    if (elementId != undefined) {
        var objPU = document.getElementById(elementId);
        document.body.removeChild(objPU);
    }
   
}


function confirmMessage(msg, lft, top, okTxt, CnclTxt) {
    var HtmlContent = '<div id="alertMsgBox"><div  style="left:' + lft + 'px;top:' + top + 'px;" class="bootbox a_modal  in"><div class="a_modal-dialog"><div class="a_modal-content"><div class="a_modal-body"><button onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button close" type="button" style="margin-top: 4px;margin-right: 5px;">' +
    '×</button><div class="bootbox-body"><div class="infoHeading"><strong>' + MultilingualKey.Confirm + '</strong></div><table width="100%" border="0" cellspacing="0" cellpadding="0"><tr><td class="confirmationImg"></td>' +
    '<td>' + msg + ' </td></tr></table></div></div><div class="a_modal-footer"><button onclick="hideAlert(\'alertMsgBox\');" id="_ConfirmBtn" class="btn btn-primary" type="button" data-bb-handler="ok" style="width: 70px">' + MultilingualKey.OK + '</button><button onclick="hideAlert(\'alertMsgBox\');" id="_CancelBtn" class="a_btn btn-small btn-danger" type="button" data-bb-handler="ok" style="width: 70px">' + MultilingualKey.Cancel + '</button></div></div></div></div></div>';
    return HtmlContent;
}

function showConfirm(msgTxt, func, CnclFunc, okTxt, CnclTxt) {
    if (!CnclFunc) CnclFunc = function () { };
    if (!okTxt) okTxt = "OK";
    if (!CnclTxt) CnclTxt = "Cancel";
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
}

function attachUnAttachEvt(elemnt, Evt, func) {
    elemnt.unbind(Evt);
    elemnt.bind(Evt, func);
}


