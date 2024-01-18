var ModalPopUp = function () {
    var app = this;
    this.DE = {
        "modalBackdrop": ".modal-backdrop",
        //Parent Pop Up..
        "ModalBody": "#ModalPopUp .modal-body",
        "ModalPopUp": "#ModalPopUp",
        "MinimizeModel": "#ModalPopUp .minmizeModel>i",
        "closeModalPopup": "#closeModalPopup",
        "ModalDialog": "#ModalPopUp .modal-dialog",
        "ModalTitle": "#ModalPopUp .modal-title",
        "modalContent": "#ModalPopUp .modal-content",
        "modalHeader": " .modal-header"

    }
    this.InitPopUp = function () {
        $(app.DE.ModalDialog).draggable({ handle: app.DE.modalHeader, containment: "window", scroll: false });
        //$(app.DE.closeModalPopup).on('click', function () {
        //    ////
        //    $(app.DE.ModalPopUp).modal('hide');
        //    if ($(app.DE.ModalPopUp).hasClass("min")) {
        //        $(app.DE.ModalPopUp).removeClass("min");
        //        $(app.DE.MinimizeModel).addClass('fa-minus').removeClass('fa-clone');
        //    }
        //    $(app.DE.ModalBody).html("");
        //    $(app.DE.ModalDialog).attr("style", "");
        //});

        $(app.DE.MinimizeModel).on('click', function () {

            $(app.DE.modalContent).toggleClass('minimize', 700, function () {
                $(app.DE.MinimizeModel).toggleClass('fa-plus').toggleClass('fa-clone');
            });
        });

        //$(app.DE.ModalDialog).draggable({ containment: 'window', scroll: false });
        //$(app.DE.ModalDialog).draggable({ handle: app.DE.modalHeader, containment: "window", scroll: false });

    }

    this.disablebuttonWhenEditDisabled = function (editPermission) {
        console.log("editPermission:" + editPermission);
        //var disableButtonList = "#btnApproveRow,#btnApplyRow,#btnAddRemarks,#btnLMCCable,#btnAddATStatusRow,.addChargeBtn,.removeAT,#btnSiteInfo,.referenceRowAdd,#dvAddMaintenanceCharge2 span[title='Add']";
        //var hideButtonList = ".attach";
        var disableButtonList = "#btnAddReferenceRowA";
        var hideButtonList = "";
        if (editPermission == 'False') {
            $(disableButtonList).addClass("dvdisabled");
            $(hideButtonList).hide();
        } else if (editPermission == 'True') {
            $(hideButtonList).show();
        }
    }

    this.LoadModalDialog = function (url, params, titleText, modalDialogClass) {
       
        //Close Pop up if opened...
        $(app.DE.closeModalPopup).trigger("click");
        $(app.DE.ModalBody).html('');
        ajaxReq(url, params, true, function (response, status, xhr) {
            //Manage Modal Popup height..
            app.AddClassToModalDialog(modalDialogClass);
            if (status == "error") {
                $(app.DE.ModalBody).html("<h6 style='color:red'>Error: " + xhr.status + " " + xhr.statusText + "</h6>");
            }
            else {
                $(app.DE.ModalBody).html(response);
            }
            $(app.DE.ModalPopUp).modal({ backdrop: 'static', keyboard: false, show: true });
            $(app.DE.ModalTitle).text(titleText);
        }, true, true, false);


        //$(app.DE.ModalBody).load(appRoot + url, params, function (response, status, xhr) {
        //    //Manage Modal Popup height..
        //    app.AddClassToModalDialog(modalDialogClass);
        //    if (status == "error") {
        //        $(app.DE.ModalBody).html("<h6 style='color:red'>Error: " + xhr.status + " " + xhr.statusText + "</h6>");
        //    }
        //    $(app.DE.ModalPopUp).modal({ backdrop: 'static', keyboard: false, show: true });
        //    $(app.DE.ModalTitle).text(titleText);

        //    hideProgress();
        //});
    }




    this.LoadModalDialogForAdmin = function (url, params, titleText, modalDialogClass) {
     
       

        //Close Pop up if opened...
        $(app.DE.closeModalPopup).trigger("click");
        $(app.DE.ModalBody).html('');
            //Manage Modal Popup height..
            app.AddClassToModalDialog(modalDialogClass);
           
            $(app.DE.ModalBody).html(url);
           
            $(app.DE.ModalPopUp).modal({ backdrop: 'static', keyboard: false, show: true });
            $(app.DE.ModalTitle).text(titleText);
      


        //$(app.DE.ModalBody).load(appRoot + url, params, function (response, status, xhr) {
        //    //Manage Modal Popup height..
        //    app.AddClassToModalDialog(modalDialogClass);
        //    if (status == "error") {
        //        $(app.DE.ModalBody).html("<h6 style='color:red'>Error: " + xhr.status + " " + xhr.statusText + "</h6>");
        //    }
        //    $(app.DE.ModalPopUp).modal({ backdrop: 'static', keyboard: false, show: true });
        //    $(app.DE.ModalTitle).text(titleText);

        //    hideProgress();
        //});


            //$('.chosen-select').chosen({ width: '100%' });

        //Close Pop up if opened...
        //$(app.DE.closeModalPopup).trigger("click");
        //$(app.DE.ModalBody).html('');
     
        //    //Manage Modal Popup height..
        //    app.AddClassToModalDialog(modalDialogClass);
        //    $(app.DE.ModalBody).html(url);
            
        //    $(app.DE.ModalPopUp).modal({ backdrop: 'static', keyboard: false, show: true });
        //    $(app.DE.ModalTitle).text(titleText);
          
    }
  

    this.AddClassToModalDialog = function (className) {

        objModalDialog = $(app.DE.ModalPopUp).find('.modal-dialog');
        //Remove all classed and add class mentioned in className param
        objModalDialog.removeClass('modal-sm');
        objModalDialog.removeClass('modal-lg');
        if (className && className != '') {
            objModalDialog.addClass(className);
        }
    }


    //this.testing = function ()
    //{
    //    alert("Hi in modal popup")
    //}
}