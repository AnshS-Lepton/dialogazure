var RPDataUploader = function () {
    var app = this;
    var infowindow = new google.maps.InfoWindow();
    var rp_boundary_type;
    var uploadId;
    var currentAction;
    var currentStep;//START,UPLOAD,VALIDATION,EXECUTION,DONE
    var isValidated;
    this.Actions = {
        "PAGE_LOAD": "PAGE_LOAD",
        "ENTITY_TYPE_CHANGE": "ENTITY_TYPE_CHANGE",
        "FILE_UPLOAD_START": "FILE_UPLOAD_START",
        "FILE_UPLOADED_SUCCESS": "FILE_UPLOADED_SUCCESS",
        "FILE_UPLOADED_FAILED": "FILE_UPLOADED_FAILED",
        "FILE_INVALID_INPUTS": "FILE_INVALID_INPUTS",
        "FILE_INVALID_FILE": "FILE_INVALID_FILE",
        "VALIDATING": "VALIDATING",
        "VALIDATED": "VALIDATED",
        "EXECUTE": "EXECUTE",
        "EXECUTED": "EXECUTED",
        "EXECUTION_FAILED": "EXECUTION_FAILED",
        "DONE": "DONE",
        "PREVIOUS_1": "PREVIOUS_1",
        "PREVIOUS_2": "PREVIOUS_2",
        "PREVIOUS_3": "PREVIOUS_3",
        "INVALID_INPUTS": "INVALID_INPUTS",
        "INVALID_FILE": "INVALID_FILE",
        "COLUMN_MAPPING": "COLUMN_MAPPING"

    }
    this.Images = {
        "loader": baseUrl + appRoot + "/Content/images/loader.gif",
        "check": baseUrl + appRoot + "/Content/images/check.png",
        "failed": baseUrl + appRoot + "/Content/images/failed.png"

    }
    this.StatusMessages = {
        EXCEL_UPLOADED_SUCCESSFULLY: MultilingualKey.SI_GBL_GBL_JQ_FRM_039,
        VALIDATION_STARTED: MultilingualKey.SI_GBL_GBL_JQ_FRM_040,// "Please Wait... validation started...",
        VALIDATION_DONE: MultilingualKey.SI_GBL_GBL_JQ_FRM_041,//"Data Validated successfully",
        EXECUTING: MultilingualKey.SI_OSP_DU_JQ_FRM_004,// "Please Wait... valid records are executing",
        EXECUTION_FAILED: MultilingualKey.SI_GBL_GBL_JQ_FRM_042,//"Execution is failed!",
        OK: "OK",
        FAILED: "FAILED",
        VALIDATION_ISSUE: "There is some issue in Validation. please try again",
        EXCEL_FILE_REQUIRED: MultilingualKey.SI_GBL_GBL_JQ_FRM_043,//"Select Excel File to upload data!",
        VALID_EXCEL_FILE_REQUIRED: "Please select a valid Excel file! Valid extensions are: ",
        KML_FILE_REQUIRED: MultilingualKey.SI_GBL_GBL_JQ_FRM_045,
        VALID_KML_FILE_REQUIRED: "Please select a valid KML file! Valid extensions are: ",
        ENTITY_REQUIRED: "Please select entity type!",
        COLUMN_MAPPED_SUCCESS: "Column mapping has been done successfully!",
        SHP_FILE_REQUIRED: 'Please select Shape file to upload!',
        VALID_SHP_FILE_REQUIRED: "Please select a valid SHP file! Valid extensions are: ",
        MAX_SHP_FILE: "Maximum 4 files can be uploaded at a time!",
        TAB_FILE_REQUIRED: 'Please select Tab file to upload!',
        VALID_TAB_FILE_REQUIRED: "Please select a valid Tab file! Valid extensions are: ",
        DXF_FILE_REQUIRED: 'Please select DXF file to upload!',
        VALID_DXF_FILE_REQUIRED: "Please select a valid DXF file! Valid extensions are: ",
        TEMPLATE_FILE_TYPE_REQUIRED: "Please select file type!",
        VALID_FILE_REQUIRED: "Please choose a file to upload!"
    }
    this.DE = {
        "step0": "#step0",
        "step1": "#step1",
        "step2": "#step2",
        "stepMain": "#stepMain",
        "step1status": "#step1 .status",
        "step2status": "#step2 .status",
        "step4status": "#step4 .status",
        "step1badge": "#step1badge",
        "step2badge": "#step2badge",
        "step4badge": "#step4badge",
        "uploadinfo": "#uploadinfo",
        "badgeActive": "badgeActive",
        "chosen_updated": "chosen:updated",
        "step1imgcheck": "#step1 .imgcheck",
        "uploadcontrols": "#uploadcontrols",
        "divDxfSourceId": "#divDxfSourceId",
        'btnPrevious1': '#btnPrevious1',
        'btnPrevious2': '#btnPrevious2',
        "btnUpload": "#btnUpload",
        "lblMessage": "#lblMessage",
        "status": " .status",
        "imgcheck": " .imgcheck",
        'btnNext': '#btnNext',
        "TotalRecord": "#lblTotalRecord",
        "btnExecute": "#btnExecute",
        "Uploadlogs": "#aUploadlogs",
        "UploadLogsContainer": "#dvUploadLogsContainer",
        'DownloadTemplate': '#btnDownloadTemplate',
        "downloadFailedLogs": "#downloadFailedLogs",
        "DownloadIcon": "#spDownloadIcon",
        'chosen_select': '.chosen-select',
        'option_selected': 'option:selected',
        'radioBtnNew': '#rdbtnNew',
        "divUpdShape": "#divUpdShape",
        "clearShapeFile": "#clearShapeFile",
        'updSHP': '#updSHP'
    }

    this.InitializeUploader = function () {
        $(app.DE.chosen_select).chosen({ width: "100%" });
        app.showHideControls(app.Actions.PAGE_LOAD);
        app.bindEvents();
        app.uploadId = 0;
        app.currentStep = "step0";
        app.isValidated = false;
    }
    this.Steps = {
        step1: "step1",
        step2: "step2",
    }

    this.showHideControls = function (action, message, result) {

        app.currentAction = action;
        if (action == app.Actions.PAGE_LOAD) {
            app.PageLoad();
        }
        else if (action == app.Actions.FILE_UPLOAD_START) {
            app.FileUploadStart();
        }
        else if (action == app.Actions.FILE_UPLOADED_SUCCESS) {
            app.FileUploadedSuccess();
        }
        else if (action == app.Actions.FILE_UPLOADED_FAILED) {
            app.FileUploadedFailed(message)
        }
        else if (action == app.Actions.FILE_INVALID_INPUTS) {
            app.FileInvalidInputs(message);
        }
        else if (action == app.Actions.FILE_INVALID_FILE) {
            app.FileInvalidFile(message);
        }
        else if (action == app.Actions.VALIDATING) {
            app.Validating();
        }
        else if (action == app.Actions.VALIDATED) {
            app.Validated(result);
        }
        else if (action == app.Actions.EXECUTE) {
            app.Execute();
        }
        else if (action == app.Actions.PREVIOUS_1) {
            app.Previous1();
        }
        else if (action == app.Actions.PREVIOUS_2) {
            app.Previous2();
        }
        else if (action == app.Actions.PREVIOUS_3) {
            app.Previous3();
        }
        else if (action == app.Actions.COLUMN_MAPPING) {
            app.columnMapping();
        }
    }

    this.PageLoad = function () {
        $(app.DE.step1status).text('');
        $(app.DE.step2status).text('');
        $(app.DE.step3status).text('');
        app.hideSteps();
        app.showStep(app.DE.step0);
        $(app.DE.selEntity).val('0').change();
        $(app.DE.selEntity).trigger(app.DE.chosen_updated);
        $(app.DE.step1badge).removeClass(app.DE.badgeActive);
        $(app.DE.step2badge).removeClass(app.DE.badgeActive);
        $(app.DE.step4badge).removeClass(app.DE.badgeActive);
        $(app.DE.uploadinfo).hide();
        $(app.DE.step1imgcheck).attr('src', app.Images.loader);
        $(app.DE.btnPrevious1).removeAttr('disabled');
        $(app.DE.btnPrevious2).removeAttr('disabled');
        $(app.DE.radioBtnNew).prop('checked', true);

    }

    this.FileUploadStart = function () {
        app.hideSteps();
        app.showStep(app.DE.step1);
        app.currentStep = app.Steps.step1;

        $('#' + app.currentStep + app.DE.status).text('Uploading file...');
        $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.loader);
        $(app.DE.btnPrevious1).attr('disabled', 'disabled');
    }
    this.FileUploadedSuccess = function () {
        app.hideSteps();
        app.showStep(app.DE.step1);
        $(app.DE.step1badge).addClass(app.DE.badgeActive);
        $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXCEL_UPLOADED_SUCCESSFULLY);
        $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
    }
    this.FileUploadedFailed = function (message) {
        app.hideSteps();
        app.showStep(app.DE.step0);
        $(app.DE.lblMessage).html(message);
    }

    this.ResetUploadData = function () {
        $(app.DE.stepMain).hide();
        app.hideSteps();
        app.showStep(app.DE.step0);
        $(app.DE.step2badge).removeClass(app.DE.badgeActive);
        $(app.DE.step4badge).removeClass(app.DE.badgeActive);
        app.showHideControls(app.Actions.PREVIOUS_1);
    }

    this.FileInvalidInputs = function (message) {
        let _result = $('<textarea />').html(message).text();
        app.hideSteps();
        app.showStep(app.DE.step0);
        alert(_result);
    }
    this.FileInvalidFile = function (message) {
        app.hideSteps();
        app.showStep(app.DE.step0);
        $(app.DE.lblMessage).html(message);
    }

    this.Execute = function () {
        //app.currentStep = app.Steps.step4;
        $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXECUTING);
        $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.loader);
        app.hideSteps();
        //app.showStep(app.DE.step4);  
        $(app.DE.btnExecute).attr('disabled', 'disabled');
        //$(app.DE.btnDone).attr('disabled', 'disabled').show();
    }

    this.Previous1 = function () {
        app.hideSteps();
        app.showStep(app.DE.step0);
        $(app.DE.step1badge).removeClass(app.DE.badgeActive);
        $(app.DE.btnUpload).show();  
    }
    this.Previous2 = function () {
        app.hideSteps();
        app.showStep(app.DE.step1);
        $(app.DE.step2badge).removeClass(app.DE.badgeActive);
        $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXCEL_UPLOADED_SUCCESSFULLY);
        $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
        $(app.DE.btnPrevious1).removeAttr('disabled'); 
    }
    this.Previous3 = function () {
        app.hideSteps();
        app.showStep(app.DE.step2);
        $(app.DE.step4badge).removeClass(app.DE.badgeActive);
        $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.COLUMN_MAPPED_SUCCESS);
        $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
        $(app.DE.btnPrevious1).removeAttr('disabled');
    }


    this.Validatefiletype = function (filename, filecontrol) {
        var valid_extentions = filecontrol.get(0).accept.split(',');
        var position = filename.lastIndexOf('.');
        var extention = '';
        if (position != -1) {
            extention = filename.substr(position);
            return valid_extentions.includes(extention);
        }
    }

    this.ValidateSHPFile = function (shpFiles) {
        // check Shape file selected or not.
        if (shpFiles.length == 0) {
            $(app.DE.lblMessage).html(app.StatusMessages.SHP_FILE_REQUIRED);
            return false;
        }
        if (shpFiles.length > 4) {
            $(app.DE.lblMessage).html(app.StatusMessages.MAX_SHP_FILE);
            return false;
        }
        // check file extension.
        if (shpFiles.length > 0) {
            for (var i = 0; i < shpFiles.length; i++) {
                if (!app.Validatefiletype(shpFiles[i].name.toLowerCase(), $(app.DE.updSHP))) {
                    $(app.DE.lblMessage).html(app.StatusMessages.VALID_SHP_FILE_REQUIRED + $(app.DE.updSHP).get(0).accept);
                    return false;
                }
            }
        }
        return true;
    }

    this.UploadData = function () {
        if ($('#FileRegProvImport').val() == "") {
            $("#FileRegProvImport").css({ "width": "195px", "border": "1px solid red" });
            return false;
        }
        else {
            $("#FileRegProvImport").css({ "width": "195px", "border": "1px solid gray" });
            var boundary_type = $("#hdnBoundaryType").val();
            app.rp_boundary_type = $("#hdnBoundaryType").val();
            if (window.FormData !== undefined) {

                 
                $("#dvImportRegProvProgress").show();
                var fileUpload = $("#FileRegProvImport").get(0);
                var files = fileUpload.files;
                var fileData = new FormData();
                if (files.length < 4) {
                    $("#dvImportRegProvProgress").hide();
                    alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_104);//Minimum 4 files are required!
                    return;
                }
                if (files.length > 4) {
                    $("#dvImportRegProvProgress").hide();
                    alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_105);
                    return;
                }
                for (var i = 0; i < files.length; i++) {
                    var fileName = files[0].name.split('.')[0];
                     
                    var ext = files[i].name.substr((files[i].name.lastIndexOf('.') + 1)).toUpperCase();
                    if (fileName.toUpperCase() != files[i].name.split('.')[0].toUpperCase()) {
                        $("#dvImportRegProvProgress").hide();
                        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_106);
                        return;
                    }
                    else if (ext === "DBF" || ext === "PRJ" || ext === "SHP" || ext === "SHX") {
                        fileData.append(files[i].name, files[i]);
                    }
                    else {
                        $("#dvImportRegProvProgress").hide();
                        alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_107);//
                        //alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_011);
                        return;
                    }
                }
                fileData.append("boundary_type", boundary_type);
                app.showHideControls(app.Actions.FILE_UPLOAD_START);
                ajaxReqforFileUpload('RegionProvince/ImportRegionProvinceShapeFile', fileData, true, function (result) {
                    let status = result.status;
                    let message = result.status + " :" + (result.err_description || result.error_msg);
                    switch (status) {

                        case app.StatusMessages.OK:
                            app.uploadId = result.id;
                            app.showHideControls(app.Actions.FILE_UPLOADED_SUCCESS, null, result);
                            app.getMappingTemplate(boundary_type, 0);
                            break;
                        case app.StatusMessages.FAILED:
                            app.showHideControls(app.Actions.FILE_UPLOADED_FAILED, message);
                            break;
                        case app.Actions.INVALID_INPUTS:
                            app.showHideControls(app.Actions.FILE_INVALID_INPUTS, message);
                            break;
                        case app.Actions.INVALID_FILE:
                            app.showHideControls(app.Actions.FILE_INVALID_FILE, message);
                            break;
                    }

                }, false);

            } else {

                alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_057);//FormData is not supported.
            }
        }
    } 
    
    this.bindEvents = function () {
        $(app.DE.btnUpload).on("click", function () {
             
            if (app.uploadId == 0) {
                isAuthenticate();
                app.UploadData();
            }
            else {
                app.showHideControls(app.Actions.FILE_UPLOADED_SUCCESS, null);
            }
        });

        $(app.DE.btnExecute).on("click", function () {
            app.ExecuteData();
        });
        

        $(app.DE.btnPrevious1).on("click", function () {
            app.showHideControls(app.Actions.PREVIOUS_1);
        });

        $(app.DE.btnPrevious2).on("click", function () {
            app.showHideControls(app.Actions.PREVIOUS_2);
        });


        $(app.DE.btnNext).on("click", function () {
            app.showHideControls(app.Actions.COLUMN_MAPPING);
        }); 
    }
    this.RemoveOldFeature = function () {
        si.map.data.forEach(function (feature) {
            si.map.data.remove(feature);
        });
    } 
     
    this.columnMapping = function () {
        app.currentStep = app.Steps.step2;
        app.hideSteps();
        app.showStep(app.DE.step2);
        $(app.DE.step2badge).addClass(app.DE.badgeActive);
    }
    this.hideSteps = function () {
        $(app.DE.step0).hide();
        $(app.DE.step1).hide();
        $(app.DE.step2).hide();
        $(app.DE.step3).hide();
    }
    this.showStep = function (obj) {
        $(obj).show();
    }
    this.getMappingTemplate = function (_boundary_type, _templateId) {

        if ($('#ddlColumTemplates').val() > 0) { _templateId = parseInt($('#ddlColumTemplates').val()); }
        else
        {
            _templateId = 0;
        }
        if (_boundary_type == null) {
            _boundary_type = $("#hdnBoundaryType").val();
        }
        ajaxReq('RegionProvince/getColumnMappping', { boundary_type: _boundary_type, templateId: _templateId }, true, function (resp) {
            $(app.DE.step2).html(resp);
        }, false, true);
    }
    this.setTemplateTame = function () {
        var _isValid = true;
        $.each($('#step2 select[is-Required="1"]'), function (i, item) {
            if ($(item).val() == '') { _isValid = false; $(this).next('.chosen-container').addClass('notvalid'); }
        })
        if (_isValid) {
            popup.LoadModalDialog('CHILD', 'RegionProvince/templateName', null, 'Template Name', 'modal-sm');
        }
    }
    this.saveTemplate = function () {
        if ($('#txtTemplateName').val() == '') { $('#txtTemplateName').addClass('notvalid'); return false; }
        $('#hdnIsFinalMapping').val(false);
        $('#hdnTemplateName').val($('#txtTemplateName').val());
        $('#hdnboundary_type').val($("#hdnBoundaryType").val());
        var _isValid = true;
        $.each($('#ddlColumTemplates option'), function (i, item) {
            var _templateName = $(item).html();
            if (_templateName.toLowerCase() == $('#txtTemplateName').val().toLowerCase()) {
                _isValid = false;
                alert('Template name already exist!');
            }
        })
        if (_isValid) { app.resetSequence(); $('#frmMappColumns').submit(); }
    }
    this.closeTemplate = function () {
        $('.modal-backdrop').hide();
    }
    this.successMapping = function (resp) {
         
        if ($('#hdnIsFinalMapping').val().toLowerCase() == 'true') {
            if (resp.status == app.Actions.INVALID_INPUTS) {
                let message = resp.status + " :" + (resp.err_description || resp.error_msg);
                app.showHideControls(app.Actions.FILE_INVALID_INPUTS, message);
            }
            //else { app.ValidateData(); }
        } else {
            if (resp.objPM.status == 'OK') {
                alert(resp.objPM.message);
                $('#closeChildPopup').trigger("click");
                var option = '<option value=' + resp.id + '>' + resp.template_name + '</option>';
                $('#ddlColumTemplates').append(option).trigger('chosen:updated');
            }
        }

        $('.modal-backdrop').hide();
    }
    this.ChangeColumnMapping = function () {
        $('#hdnUploadId').val(app.uploadId);
        $('#hdnIsFinalMapping').val(true);
        var _isValid = true;
        $.each($('#step2 select[is-Required="1"]'), function (i, item) {
            if ($(item).val() == '') { _isValid = false; $(this).next('.chosen-container').addClass('notvalid'); }
        })
        if (_isValid) {
            app.resetSequence();
            $('#frmMappColumns').submit();

            app.showHideControls(app.Actions.EXECUTE);

            ajaxReq('Regionprovince/GetData', { boundary_type: app.rp_boundary_type }, true, function (resp) {
                $(app.DE.step4badge).addClass(app.DE.badgeActive);
                $(app.DE.stepMain).show();
                $(app.DE.stepMain).html(resp);
                $("#dvRegionProvinceFileInfo").show();
            }, false, true);
        }
    }
    this.getColumnsMapping = function () {
        var mappings = [];
        $.each($('#step2 select'), function (i, item) {
            if ($(item).val() != '') {
                let _templateColumnName = $(item).attr('template-column-name');
                let _templateDbColumnname = $(item).attr('template-db-column-name');
                let _importedColumnName = $(item).val();
                mappings.push({
                    template_db_column_name: _templateDbColumnname,
                    template_column_name: _templateColumnName,
                    imported_column_name: _importedColumnName
                })
            }
        })
        return mappings;
    }
    this.resetSequence = function () {
        $.each($('#frmMappColumns select'), function (i, item) {
            if ($(this).attr('name') != undefined)
                $(this).attr('name', $(this).attr('name').replace(/\[\d+\]/, "[" + (i) + "]"));
            if ($(this).attr('id') != undefined)
                $(this).attr('id', $(this).attr('id').replace(/\_\d+\_/, "_" + (i) + "_"));
        })
        $.each($('.hiddenTempColumns'), function (i, item) {
            $.each($(this).children('input[type="hidden"]'), function () {
                if ($(this).attr('name') != undefined)
                    $(this).attr('name', $(this).attr('name').replace(/\[\d+\]/, "[" + (i) + "]"));
                if ($(this).attr('id') != undefined)
                    $(this).attr('id', $(this).attr('id').replace(/\_\d+\_/, "_" + (i) + "_"));
            })
        })
    }
     
    this.validate = function (elementId) {

        if ($('#' + elementId).length >= 0) {
            var value = $('#' + elementId).val();

            if (value == null || value == "" || value == 0 || value == undefined || value == "--Select--") {
                $('#' + elementId).addClass('input-validation-error').removeClass('valid');
                return false;
            }
            else {
                $('#' + elementId).addClass('valid').removeClass('input-validation-error');
                return true;
            }
        }
        return true;
    }

}






