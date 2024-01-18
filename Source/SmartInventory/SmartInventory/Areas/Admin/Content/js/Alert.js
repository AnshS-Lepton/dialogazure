
$(document).ready(function () {
    window.alert = function (msg, title, type) { showMessageBox(msg, title, type); }
    window.confirm = function (msgTxt, func, CnclFunc, okTxt, CnclTxt) {
        showConfirm(msgTxt, func, CnclFunc, okTxt, CnclTxt);
    }
});

function showMessageBox(msgTxt, title, type) {
    if (document.getElementById('alertMsgBox'))
        hideAlert();

    var l, t;
    // l = (window.innerWidth - 350) / 2;
    // t = (window.innerHeight - 70) / 2;
    l = window.innerWidth / 2 - 382 / 2;
    t = window.innerHeight / 2 - 155 / 2;
    var DivContent = document.createElement('div');
    DivContent.innerHTML = AlertMessage(msgTxt, l, t, title, type);
    document.body.appendChild(DivContent.firstChild);
    $('#alertMsgDiv').draggable({ scroll: false, handle: ".infoHeading", containment: "window" });
}

function AlertMessage(msg, lft, top) {
    var HtmlContent = `<div id="alertMsgBox">
      <div id="alertMsgDiv" style="left:${lft}px;top:${top}px;" class="bootbox a_modal fade in alertContainer">
        <div class="a_modal-dialog">
          <div class="a_modal-content">
            <div class="a_modal-body">
              <div class="modal-header d-flex">
                <h4 class="modal-title">${title}</h4>
                <span onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button icon-close ml-auto cur-poiner cur-poiner"></span>
              </div>
              <div class="msg--padd d-fx">
                <div class="icon-info iconDetail"></div>
                <div class="msg--content">${msg}</div>
              </div>
              <div class="ModalFooter d-flex">
                <button onclick="hideAlert(\'alertMsgBox\');" class="a_btn btn-small btn-primary ml-auto" type="button" data-bb-handler="ok">OK</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>`;
    return HtmlContent;
}

function AlertMessage(msg, lft, top, title, type) {
    var iconImg='';
    if (type == "success")
    {
        iconImg = 'SucessImg';
    }
    else {
        iconImg = 'InformationImg';
    }
    title = title == undefined ? 'Information' : title;
    var HtmlContent = `<div id="alertMsgBox">
          <div id="alertMsgDiv" style="left:${lft}px;top:${top}px;" class="bootbox a_modal fade in alertContainer">
            <div class="a_modal-dialog">
              <div class="a_modal-content">
                <div class="a_modal-body">
                  <div class="modal-header d-flex">
                    <h4 class="modal-title">${title}</h4>
                    <span onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button icon-close ml-auto cur-poiner cur-poiner"></span>
                  </div>
                  <div class="msg--padd d-fx">
                    <div class="icon-info iconDetail"></div>
                    <div class="msg--content">${msg}</div>
                  </div>
                  <div class="ModalFooter d-flex">
                    <button onclick="hideAlert(\'alertMsgBox\');" class="a_btn btn-small btn-primary ml-auto" type="button" data-bb-handler="ok">OK</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>`;
    return HtmlContent;
}


function hideAlert(elementId) {
    if (elementId != undefined) {
        var objPU = document.getElementById(elementId);
        document.body.removeChild(objPU);
    }
   
}


function confirmMessage(msg, lft, top, okTxt, CnclTxt) {
    var HtmlContent = `<div id="alertMsgBox">
          <div id="alertMsgDiv" style="left:${lft}px;top:${top}px;" class="bootbox a_modal fade in alertContainer">
            <div class="a_modal-dialog">
              <div class="a_modal-content">
                <div class="a_modal-body">
                  <div class="modal-header d-flex">
                    <h4 class="modal-title">Confirm</h4>
                    <span onclick="hideAlert(\'alertMsgBox\');" class="bootbox-close-button icon-close ml-auto cur-poiner cur-poiner"></span>
                  </div>
                  <div class="msg--padd d-fx">
                    <div class="icon-alert color-Danger iconDetail"></div>
                    <div class="msg--content">${msg}</div>
                  </div>
                  <div class="ModalFooter d-flex --gap-05">
				  <button onclick="hideAlert(\'alertMsgBox\');" id="_CancelBtn" class="a_btn btn-small btn-danger ml-auto" type="button" data-bb-handler="Cancel">Cancel</button>
                   <button onclick="hideAlert(\'alertMsgBox\');" id="_ConfirmBtn" class="a_btn btn-small btn-primary" type="button" data-bb-handler="ok">${okTxt}</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>`;
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
    $('#alertMsgDiv').draggable({ scroll: false, handle: ".infoHeading", containment: "window" });
}

function attachUnAttachEvt(elemnt, Evt, func) {
    elemnt.unbind(Evt);
    elemnt.bind(Evt, func);
}


