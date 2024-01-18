var ModalPopUp = function () {
    var app = this;
    this.DE = {
        // "modalBackdrop": ".modal-backdrop",
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
    this.ResetProperties = function (curmodelId) {
        app.DE = {
            // "modalBackdrop": ".modal-backdrop",
            //Parent Pop Up..
            "ModalBody": "#" + curmodelId + " .modal-body",
            "ModalPopUp": "#" + curmodelId,
            "MinimizeModel": "#" + curmodelId + " .minmizeModel>i",
            "closeModalPopup": "#closeModalPopup",
            "ModalDialog": "#" + curmodelId + " .modal-dialog",
            "ModalTitle": "#" + curmodelId + " .modal-title",
            "modalContent": "#" + curmodelId + " .modal-content",
            "modalHeader": " .modal-header"

        }
    }
    this.InitPopUp = function () {
        $(app.DE.MinimizeModel).on('click', function () {
            if ($(app.DE.modalContent).hasClass("minimize")) {
                $(app.DE.modalContent).removeClass('minimize', 700);
                $(this).addClass('fa-minus').removeClass('fa-plus');
                $(app.DE.ModalPopUp).css("height", "auto");
            } else {
                $(app.DE.modalContent).addClass('minimize', 700);
                $(this).addClass('fa-plus').removeClass('fa-minus');
                $(app.DE.ModalPopUp).css("height", "100px");
            }
        });

        $(app.DE.ModalDialog).draggable({ containment: 'window', scroll: false });
        $(app.DE.ModalDialog).draggable({ handle: app.DE.modalHeader, containment: "window", scroll: false });

    }

    this.LoadModalDialog = function (modelid, url, params, titleText, modalDialogClass) {
        app.ResetProperties(modelid);
        app.InitPopUp();
        // app.DE.ModalPopUp = "#" + modelid + " .modal-dialog";
        //Close Pop up if opened...
        $(app.DE.ModalPopUp).find('.icon-close').trigger("click");
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
            $(app.DE.ModalPopUp).modal({ backdrop: false, keyboard: false, show: true });
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