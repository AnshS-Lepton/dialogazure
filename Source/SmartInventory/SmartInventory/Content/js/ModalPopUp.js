var ModalPopUp = function () {
    var app = this;
    this.DE = {
        "frmAddHTB": "#frmAddHTB",
        "modalBackdrop": ".modal-backdrop",
        //Parent Pop Up..
        "ModalBody": "#ModalPopUp .modal-body",
        "ModalPopUp": "#ModalPopUp",
        "MinimizeModel": "#ModalPopUp .minmizeModel>i",
        "ParentMinMaxContainer": "#parentMinMaxContainer",
        "closeModalPopup": "#closeModalPopup",
        "ModalDialog": "#ModalPopUp .modal-dialog",
        "ModalTitle": "#ModalPopUp .modal-title",
        "modalContent": "#ModalPopUp .modal-content",
        "modalHeader": " .modal-header",

        //Child Pop Up..
        "ChildModalBody": "#ChildPopUp .modal-body",
        "ChildPopUp": "#ChildPopUp",
        "MinimizeChildModel": "#ChildPopUp .minmizeModel>i",
        "ChildMinMaxContainer": "#childMinMaxContainer",
        "CloseChildPopup": "#closeChildPopup",
        "ChildModalDialog": "#ChildPopUp .modal-dialog",
        "ChildModalTitle": "#ChildPopUp .modal-title",
        "ChildContent": "#ChildPopUp .modal-content",
        "libDetail": ".libDetail"

    }

    this.hideShowScrollMenu = () => {
        var allTabWidth = 0;
        $.each($('.libTabs .tabs-left li'), function (indx, itm) {
            allTabWidth += $(this).width();
        });
        if (allTabWidth > $('#dvModalBody').width()) {
            $('#spnRightMenu,#divRightMenu').show();
        }
        else {
            $('#spnRightMenu,#divRightMenu').hide();
        }
    }
    function resetActiveClassFromMenuItem() {
        $('.myMenu ul li a').each(function () {
            $(this).removeClass("activeToolBar");
        });
        $('.iconBaricomoonfooter a').each(function () {
            $(this).removeClass("activeToolBar");
        });

        $('.iconBaricomoonfooter').removeClass("activeToolBar");
    }
    this.InitPopUp = function () {

        $(app.DE.closeModalPopup).on('click', function () {
            resetActiveClassFromMenuItem();
            $(app.DE.ModalPopUp).modal('hide');
            if ($(app.DE.ModalPopUp).hasClass("min")) {
                $(app.DE.ModalPopUp).removeClass("min");
                $(app.DE.MinimizeModel).addClass('fa-minus').removeClass('fa-clone');
            }
            $(app.DE.ModalBody).html("");
            $(app.DE.ModalDialog).attr("style", "");
        });

        $(app.DE.MinimizeModel).on('click', function () {
             
            $apnData = $(app.DE.ModalPopUp);
            $(app.DE.ModalDialog).attr('style', '');
            $('.modal-backdrop').toggleClass('out').toggleClass('in');
            $(app.DE.modalContent).toggleClass('minimize', function () {
                $(app.DE.MinimizeModel).toggleClass('fa-plus').toggleClass('fa-clone');
                if ($('.modal-content').hasClass('minimize')) {
                    $('.modal-backdrop').hide();                   
                    $(app.DE.MinimizeModel.split(' ')[0]).removeClass('modal');
                    $(app.DE.MinimizeModel.split(' ')[0]).addClass('MinMax');
                   // $('#ModalPopUp').removeClass('modal');
                   // $(app.DE.ModalDialog).addClass('MinMax');
                    $apnData.addClass('ModalPopUpPosition');
                } else {
                    $('.modal-backdrop').show();                  
                    $(app.DE.MinimizeModel.split(' ')[0]).addClass('modal');
                    $(app.DE.MinimizeModel.split(' ')[0]).removeClass('MinMax');
                   // $('#ModalPopUp').addClass('modal');
                   // $(app.DE.ModalDialog).removeClass('MinMax');
                    $apnData.removeClass('ModalPopUpPosition');
                }
            });
        });

        $(app.DE.MinimizeChildModel).on('click', function () {
            $apnData = $(app.DE.ChildPopUp);
            $(app.DE.ModalDialog).attr('style', '');
            $('.modal-backdrop').toggleClass('out').toggleClass('in');
            $(app.DE.ChildContent).toggleClass('minimize', function () {
                $(app.DE.MinimizeChildModel).toggleClass('fa-plus').toggleClass('fa-clone');
                if ($('.modal-content').hasClass('minimize')) {
                    $('.modal-backdrop').hide();
                    $(app.DE.MinimizeChildModel.split(' ')[0]).removeClass('modal');
                    $(app.DE.MinimizeChildModel.split(' ')[0]).addClass('MinMax');
                    //$('#ModalPopUp').removeClass('modal');
                   // $(app.DE.ModalDialog).addClass('MinMax');
                    $apnData.addClass('ModalPopUpPosition');
                } else {
                    $('.modal-backdrop').show();
                    $(app.DE.MinimizeChildModel.split(' ')[0]).addClass('modal');
                    $(app.DE.MinimizeChildModel.split(' ')[0]).removeClass('MinMax');
                    //$('#ModalPopUp').addClass('modal');
                    //$(app.DE.ModalDialog).removeClass('MinMax');
                    $apnData.removeClass('ModalPopUpPosition');
                }
                $('.modal2').css("z-index", "4001");
            });
        });

        //$(app.DE.ModalDialog).draggable({ containment: 'window', scroll: false });
        $(app.DE.ModalDialog).draggable({
            handle: app.DE.modalHeader, containment: "window", scroll: false,
            start: function () {

            },
            stop: function () {

            }
        });
        $(app.DE.ChildModalDialog).draggable({ handle: app.DE.modalHeader, containment: "window", scroll: false });

        $(app.DE.closeModalPopup).on('click', function () {

            app.closeModalPopup();
            app.resetToolBar();


        });

        $(app.DE.CloseChildPopup).on('click', function () {
            // 
            $(app.DE.ChildPopUp).modal('hide');
            if ($(app.DE.ChildPopUp).hasClass("min")) {
                $(app.DE.ChildPopUp).removeClass("min");
                $(app.DE.MinimizeChildModel).find("i").addClass('fa-minus').removeClass('fa-clone');

            }

            $(app.DE.ChildModalBody).html("");
            $(app.DE.ChildModalDialog).attr("style", "");
            $(app.DE.ChildMinMaxContainer).hide();
            $('#dvProgress').css("display", "none");
            $('.modal-backdrop').hide();
        });

    }
    this.closeModalPopup = function () { ////
        if (typeof dataUploader !== 'undefined' && dataUploader != null) {
            dataUploader.RemoveOldFeature();
        } // 
        $("#dvUtilizationShowonmap").hide();
        if ($('#frmShowSurveyBuilding').length > 0) { removeOldMarkers(); }
        //$(app.DE.ModalPopUp).hide();
        if ($(app.DE.ModalPopUp).hasClass("min")) {
            $(app.DE.ModalPopUp).removeClass("min");
            $(app.DE.MinimizeModel).find("i").addClass('fa-minus').removeClass('fa-clone');
        }
        $(app.DE.ModalBody).html("");
        $(app.DE.ModalDialog).attr("style", "");
        $(app.DE.ParentMinMaxContainer).hide();
        if (splicing != null) { splicing.clearCPFMarker(); }
        //if (LandBase != null) { LandBase.clearTempNewEntity(); }

        clearLineAnimation();
        //$('#ModalPopUp div').removeClass('modal-xxl');
        $('.modal-backdrop').remove();
        $('#ModalPopUp div').removeClass('modal-sm').removeClass('modal-xl').removeClass('modal-lg').removeClass('modal-md').removeClass('modal-xxl').removeClass('modal-md-new');
        if (FiberCutTracing != null) {
            FiberCutTracing.reset();
        }
        $(app.DE.ModalPopUp).removeClass('MinMax');
        //if (isp != null)
        //{ isp.bindCableRightPop(); }
    }
    this.resetToolBar = function () { if ($('#dvROWReport').hasClass('activeToolBar')) { $('#dvROWReport').trigger("click"); $('#reportToolBar div,#reportToolBar div a').removeClass('activeToolBar'); } }
    //this.hideButtonsWhenLayerDisabled = function () {
    //    var editEnabled = $("#LayerEditPermission").val();
    //    var buttonList = " .floorRowDelete,.shaftRowDelete,.floorRowAdd,.shaftRowAdd,.icon-close,.referenceRowAdd,#dvAddMaintenanceCharge1";

    //    //$(app.DE.libDetail + " :input[type=submit],#btnApproveRow").prop("disabled", editEnabled == 'False');
    //    //$(app.DE.frmAddHTB + " :input[type=submit],#btnApproveRow").prop("disabled", editEnabled == 'False');
    //    $(app.DE.libDetail + " :input[type=submit],#btnApproveRow,:button").prop("disabled", editEnabled == 'False');
    //    $(app.DE.frmAddHTB + " :input[type=submit],#btnApproveRow,:button").prop("disabled", editEnabled == 'False');
    //    //$(app.DE.libDetail + ' .chosen-select').trigger("chosen:updated");
    //    //$(app.DE.frmAddHTB + ' .chosen-select').trigger("chosen:updated");
    //    if (editEnabled == 'False') {
    //        $(app.DE.libDetail + buttonList).hide();
    //    } else {
    //        $(app.DE.libDetail + buttonList).show();
    //    }
    //}

    this.disablebuttonWhenEditDisabled = function (editPermission) {
        console.log("editPermission:" + editPermission);
        var disableButtonList = "#btnApproveRow,#btnApplyRow,#btnAddRemarks,#btnLMCCable,#btnAddATStatusRow,.addChargeBtn,.removeAT,#btnSiteInfo,.referenceRowAdd,#dvAddMaintenanceCharge2 span[title='Add']";
        var hideButtonList = ".attach";
        if (editPermission == 'False') {
            $(disableButtonList).addClass("dvdisabled");
            $('.dvdisabled').off('click').attr('onclick', 'return disabledItem(event)');
            $(hideButtonList).hide();
        } else if (editPermission == 'True') {
            $(hideButtonList).show();
        }
    }
    this.LoadModalDialog = function (popUpType, url, params, titleText, modalDialogClass, callback, isasync) {
         
        $(app.DE.ModalPopUp).removeClass('ModalPopUpPosition');
        $(app.DE.modalContent).removeClass('minimize');
        $(app.DE.ChildContent).removeClass('minimize');
        $('#ModalPopUp').addClass('modal');
        $(app.DE.ModalDialog).removeClass('MinMax');
        //$(app.DE.ModalPopUp).animate({ width: "100%", height: '100%' });
        $(app.DE.MinimizeModel).removeClass('fa-plus').removeClass('fa-clone');
        if (popUpType != undefined) {
            titleText = getMultilingualKeyForLoadModalDialog(titleText);
            if (popUpType.toUpperCase() == 'PARENT') {
                //modalDialogClass = params.eType == 'UNIT' ? 'modal-sm' : modalDialogClass;
                //Close Pop up if opened...
                //$(app.DE.closeModalPopup).trigger("click");
                app.closeModalPopup();
                $(app.DE.ModalBody).html('');

                isasync = isasync != undefined ? isasync : true;
                ajaxReq(url, params, isasync, function (response, status, xhr) {
                    //Manage Modal Popup height..
                    app.AddClassToModalDialog(popUpType, modalDialogClass);

                    if (status == "error") {
                        $(app.DE.ModalBody).html("<h6 style='color:red'>Error: " + xhr.status + " " + xhr.statusText + "</h6>");
                    }
                    else {
                        $(app.DE.ModalBody).html(response);
                        setTimeout(app.hideShowScrollMenu, 500);
                        //app.hideButtonsWhenLayerDisabled();
                        $('.dvdisabled').off('click').attr('onclick', 'return disabledItem(event)');
                    }
                    $(app.DE.ModalPopUp).modal({ backdrop: 'static', keyboard: false, show: true });
                    $(app.DE.ModalTitle).text(titleText);
                    if (callback) { callback(); }
                }, true, true, false);
            }
            else if (popUpType.toUpperCase() == 'CHILD') {

                ajaxReq(url, params, true, function (response, status, xhr) {
                    //Manage Modal Popup height..

                    app.AddClassToModalDialog(popUpType, modalDialogClass);
                    if (status == "error") {
                        $(app.DE.ChildModalBody).html("<h6 style='color:red'>Error: " + xhr.status + " " + xhr.statusText + "</h6>");
                    }
                    else {
                        $(app.DE.ChildModalBody).html(response);
                    }
                     

                    $(app.DE.ChildPopUp).modal({ backdrop: 'static', keyboard: false, show: true });
                    $(app.DE.ChildModalTitle).text(titleText);
                    $('#dvMainProgress').hide();
                    //sp.hideProgress();
                    //Maximize modal if minimized...
                    //app.MaximizeChildPopUp();
                    //$(app.DE.ChildMinMaxContainer).show();
                }, true, true, false);

            }
        }


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

    this.AddClassToModalDialog = function (popUpType, className) {
        if (popUpType != undefined) {
            var objModalDialog;
            if (popUpType.toUpperCase() == 'PARENT') {

                objModalDialog = $(app.DE.ModalPopUp).find('.modal-dialog');
            }
            else {

                objModalDialog = $(app.DE.ChildPopUp).find('.modal-dialog');
            }
            //Remove all classed and add class mentioned in className param
            objModalDialog.removeClass('modal-sm').removeClass('modal-xl').removeClass('modal-lg').removeClass('modal-md').removeClass('modal-xxl').removeClass('modal-md-new');
            if (className && className != '') {
                objModalDialog.addClass(className);
            }
        }
    }
}