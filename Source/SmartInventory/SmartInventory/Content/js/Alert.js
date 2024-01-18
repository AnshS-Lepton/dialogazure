
$(document).ready(function () {   
    window.alert = function (msg, type, header) {
        if (isp != null && type != undefined && header != undefined && type == 'success') {
            var messageId = 'alertdiv_' + (Math.random()).toString().replace('.', '');
            //style="margin-top: ' + (($('#divAlretContainer').children().length*70)+3) + 'px;"
            var alertHTML = '<div class="allert alert-success" id="' + messageId + '">';
            alertHTML+='<div class="alert-iconbox icon-Success"></div>';
            alertHTML += '<div class="alert-message"><span>' + msg + '</span></div>';
            alertHTML += '<div class="alert-close icon-close"></div>';
            alertHTML += '</div>'
            $('#divAlretContainer').append(alertHTML);            
            $('#' + messageId).animate({ width: 'toggle' },500);
            setTimeout(function () {
                $('#' + messageId).animate({ width: 'hide' }, 500);
            },3000);
        } else {
            showMessageBox(msg, type, header);
        }
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
    if (isp != null && type != undefined) {
        HtmlContent = `<div id="alertMsgBox">
          <div id="alertMsgDiv" style="left:${lft}px;top:${top}px;" class="bootbox a_modal fade in alertContainer">
            <div class="a_modal-dialog">
              <div class="a_modal-content">
                <div class="a_modal-body">
                  <div class="modal-header d-flex">
                    <h4 class="modal-title">${header}</h4>
                    <span onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button icon-close ml-auto cur-poiner cur-poiner"></span>
                  </div>
                  <div class="msg--padd d-fx">
                    <div class="icon-info iconDetail"></div>
                    <div class="msg--content">${msg}</div>
                  </div>
                  <div class="ModalFooter d-flex">
                    <button onclick="hideAlert(\'alertMsgBox\');" class="a_btn btn-small btn-primary ml-auto" type="button" data-bb-handler="ok">${MultilingualKey.GBL_GBL_GBL_JQ_GBL_002}</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>`;
    } else {
        HtmlContent = `<div id="alertMsgBox">
        <div id="alertMsgDiv" style="left:${lft}px;top:${top}px;" class="bootbox a_modal fade in ' + type + ' alertContainer">
          <div class="a_modal-dialog">
              <div class="a_modal-body">
                <div class="modal-header d-flex">
                  <h4 class="modal-title">${header}</h4>
                  <span onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button icon-close ml-auto cur-poiner"></span>
                </div>
                  <div class="msg--padd d-fx">
                    <div class="icon-info iconDetail"></div>
                    <div class="msg--content">${msg} </div>
                  </div>
              </div>
              <div class="ModalFooter d-flex">
                <button onclick="hideAlert(\'alertMsgBox\');" class="btn btn-small btn-primary ml-auto" type="button" data-bb-handler="ok">${MultilingualKey.GBL_GBL_GBL_JQ_GBL_002}</button>
              </div>
            </div>
        </div>
      </div>`;
    }
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
    var HtmlContent = `<div id="alertMsgBox">
        <div id="alertMsgDiv"  style="left:${lft}px;top:${top}px;" class="bootbox a_modal fade in">
          <div class="a_modal-dialog">
            <div class="a_modal-content">
              <div class="a_modal-body">\
                <div class="modal-header d-flex">
                    <h4 class="modal-title">${MultilingualKey.SI_GBL_GBL_JQ_FRM_015}</h4>
                    <span onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button icon-close ml-auto cur-poiner" type="button"></span>
                  </div>
                  <div class="msg--padd d-fx">
                    <div class="icon-information iconDetail"></div>
                    <div class="msg--content">${msg} </div>
                  </div>
                </div>
                <div class="ModalFooter d-flex">
                    <button onclick="hideAlert(\'alertMsgBox\');" id="_CancelBtn" class="btn btn-small btn-warning ml-auto mr-01" type="button" data-bb-handler="ok">${CnclTxt}</button>
                    <button onclick="hideAlert(\'alertMsgBox\');" id="_ConfirmBtn" class="btn btn-small btn-primary" type="button" data-bb-handler="ok">${okTxt}</button>
                </div>
            </div>
          </div>
        </div>
      </div>`;
    if (isp != null) {
        HtmlContent = `<div id="alertMsgBox">
      <div id="alertMsgDiv" style="left:${lft}px;top:${top}px;" class="bootbox a_modal fade in alertContainer">
        <div class="a_modal-dialog">
          <div class="a_modal-content">
            <div class="a_modal-body">
                <div class="modal-header d-flex">
                  <h4 class="modal-title">${MultilingualKey.SI_GBL_GBL_JQ_FRM_015}</h4>
                  <span onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button icon-close ml-auto cur-poiner" type="button"></span>
                </div>
                <div class="msg--padd d-fx">
                  <div class="icon-information iconDetail confirmDelete"></div>
                  <div class="msg--content">${msg} </div>
                </div>
            </div>
            <div class="ModalFooter d-flex">
              <button onclick="hideAlert(\'alertMsgBox\');" id="_CancelBtn" class="btn btn-small btn-warning ml-auto mr-01" type="button" data-bb-handler="ok">${CnclTxt}</button>
              <button onclick="hideAlert(\'alertMsgBox\');" id="_ConfirmBtn" class="btn btn-small btn-primary" type="button" data-bb-handler="ok">${okTxt}</button>
          </div>
          </div>
        </div>
      </div>
    </div>`;
    }   
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


