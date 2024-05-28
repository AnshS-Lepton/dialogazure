var DataUploader = function () {
    var app = this;
    var infowindow = new google.maps.InfoWindow();

    var uploadId;
    var planId;
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
        DATA_FETCHED_SUCCESSFULLY: 'Data fetched successfully',
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
        JSON_FILE_REQUIRED: "Select Json File to upload data!",
        VALID_KML_FILE_REQUIRED: "Please select a valid KML file! Valid extensions are: ",
        VALID_JSON_FILE_REQUIRED: "Please select a valid JSON file! Valid extensions are: ",
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

        "plantxt": "#txtPlan",
        "divTotalReordsdetails": "#divTotalReordsdetails",

        "step0": "#step0",
        "step1": "#step1",
        "step2": "#step2",
        "step3": "#step3",
        "step4": "#step4",

        "stepID0": "#stepID0",
        "stepID1": "#stepID1",
        "stepID2": "#stepID2",
        "stepID3": "#stepID3",
        "stepID4": "#stepID4",

        "step1status": "#step1 .status",
        "step2status": "#step2 .status",
        "step3status": "#step3 .status",
        "step4status": "#step4 .status",

        "stepID1status": "#stepID1 .status",
        "stepID2status": "#stepID2 .status",
        "stepID3status": "#stepID3 .status",
        "stepID4status": "#stepID4 .status",


        "selEntity": "#selEntity",

        "step1badge": "#step1badge",
        "step2badge": "#step2badge",
        "step3badge": "#step3badge",
        "step4badge": "#step4badge",

        "stepID1badge": "#stepID1badge",
        "stepID3badge": "#stepID3badge",
        "stepID4badge": "#stepID4badge",


        "uploadinfo": "#uploadinfo",
        "uploadinfoID": "#uploadinfoID",

        "badgeActive": "badgeActive",
        "chosen_updated": "chosen:updated",
        "step1imgcheck": "#step1 .imgcheck",
        "stepID11imgcheck": "#stepID11 .imgcheck",

        "uploadcontrols": "#uploadcontrols",

        "uploadcontrolsID": "#uploadcontrolsID",

        "divDxfSourceId": "#divDxfSourceId",
        'btnPrevious1': '#btnPrevious1',
        'btnPrevious2': '#btnPrevious2',
        'btnPrevious3': '#btnPrevious3',


        'btnImpDataPrevious1': '#btnImpDataPrevious1',
        'btnImpDataPrevious3': '#btnImpDataPrevious3',

        "divUpdExcel": "#divUpdExcel",
        "divUpdJson": "#divUpdJson",
        "divUpdKML": "#divUpdKML",
        "divUpdTab": "#divUpdTab",
        "divUpdDxf": "#divUpdDxf",
        "btnUpload": "#btnUpload",

        "btnIDUpload": "#btnIDUpload",


        "lblMessage": "#lblMessage",
        "IDlblMessage": "#IDlblMessage",

        "clearKMLFile": "#clearKMLFile",
        "clearExcelFile": "#clearExcelFile",
        "clearJsonFile": "#clearJsonFile",
        "clearDxfFile": "#clearDxfFile",
        "clearTabFile": "#clearTabFile",
        "updKML": "#updKML",
        "updExcel": "#updExcel",
        "updJson": "#updJson",
        "updTab": "#updTab",
        "updDxf": "#updDxf",
        "status": " .status",
        "imgcheck": " .imgcheck",

        'btnNext': '#btnNext',
        'btnIDNext': '#btnIDNext',

        "TotalRecord": "#lblTotalRecord",
        "ValidRecord": "#lblValidRecord",
        "InvalidRecord": "#lblInvalidRecord",


        "EntityName": "#lblEntityName",
        "TotalRecordID": "#lblIDTotalRecord",
        "ValidRecordID": "#lblIDValidRecord",
        "InvalidRecordID": "#lblIDInvalidRecord",


        "btnExecute": "#btnExecute",

        "btnIDExecute": "#btnIDExecute",
        "btnDone": "#btnDone",
        "btnSingleDone": "#btnSingleDone",
        "btnIDDone": "#btnIDDone",
        "Uploadlogs": "#aUploadlogs",
        "UploadLogsContainer": "#dvUploadLogsContainer",
        'DownloadTemplate': '#btnDownloadTemplate',
        "downloadFailedLogs": "#downloadFailedLogs",
        "downloadFailedLogsID": "#downloadFailedLogsID",
        "DownloadIcon": "#spDownloadIcon",
        "DownloadIconID": "#spDownloadIconID",

        'chosen_select': '.chosen-select',
        'option_selected': 'option:selected',
        'radioBtnNew': '#rdbtnNew',
        "divUpdShape": "#divUpdShape",
        "clearShapeFile": "#clearShapeFile",
        'updSHP': '#updSHP',
        'btnNextStep2': '#btnNextStep2',
        'btnTemplateCancel': '#btnTemplateCancel',
        'liImportDataUtility': '#liImportDataUtility',
        'liFileUploadUtility': '#liFileUploadUtility',
        'aFileUploadUtility': '#aFileUploadUtility'
    }
    this.activeTab = function () {
        return $(app.DE.liImportDataUtility).hasClass('active');
    }

    this.initActiveTab = () => {
        if ($("#liImportDataUtility").length == 0) {
            $("aFileUploadUtility").trigger("click");
        }
    }

    this.InitializeUploader = function () {
         
        app.initActiveTab();


        let isImportDataActivetab = app.activeTab();
        $(app.DE.chosen_select).chosen({ width: "100%" });
        app.showHideControls(app.Actions.PAGE_LOAD);

        app.bindEvents();
        app.uploadId = 0;

        if (isImportDataActivetab) {
            app.currentStep = "stepID0";
        }
        else {
            app.currentStep = "step0";
        }

        app.isValidated = false;

        $(app.DE.liImportDataUtility).on("click", function () {
             
            $(app.DE.liFileUploadUtility).remove('active')
            app.isValidated = false;

            app.PageLoad();

        });
        $(app.DE.liFileUploadUtility).on("click", function () {
            $(app.DE.liImportDataUtility).remove('active')
            app.resetValue();
            app.PageLoad();
        });
    }
    this.Steps = {
        step1: "step1",
        step2: "step2",
        step3: "step3",
        step4: "step4",

        stepID1: "stepID1",
        stepID2: "stepID2",
        stepID3: "stepID3",
        stepID4: "stepID4"
    }

    this.resetValue = function () {
        app.isValidated = false;
    }
    this.showHideControls = function (action, message, result) {
        app.currentAction = action;
        if (action == app.Actions.PAGE_LOAD) {
            app.PageLoad();
        }
        else if (action == app.Actions.ENTITY_TYPE_CHANGE) {
            app.EntityTypeChange();
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
        else if (action == app.Actions.EXECUTED) {
            app.Executed();
        }
        else if (action == app.Actions.SINGLE_EXECUTED) {
            app.Single_Executed();
        }
        else if (action == app.Actions.EXECUTION_FAILED) {
            app.ExecutionFailed();
        }
        else if (action == app.Actions.DONE) {
            app.Done();
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
        $(app.DE.step4status).text('');

        $(app.DE.stepID1status).text('');
        $(app.DE.stepID2status).text('');
        $(app.DE.stepID3status).text('');
        $(app.DE.stepID4status).text('');

        app.hideSteps();

        let isImportDataActivetab = app.activeTab();

        if (isImportDataActivetab) {
            app.showStep(app.DE.stepID0);
            $(app.DE.plantxt).val('');
            $(app.DE.stepID1badge).removeClass(app.DE.badgeActive);
            $(app.DE.stepID3badge).removeClass(app.DE.badgeActive);
            $(app.DE.stepID4badge).removeClass(app.DE.badgeActive);
            $(app.DE.uploadinfoID).hide();
            $(app.DE.divTotalReordsdetails).text('');
            $(app.DE.stepID1imgcheck).attr('src', app.Images.loader);
            $(app.DE.uploadcontrolsID).show();
            $(app.DE.btnImpDataPrevious1).removeAttr('disabled');
            $(app.DE.btnImpDataPrevious3).removeAttr('disabled');
        }
        else {
            app.showStep(app.DE.step0);
            $(app.DE.selEntity).val('0').change();
            $(app.DE.selEntity).trigger(app.DE.chosen_updated);
            $(app.DE.step1badge).removeClass(app.DE.badgeActive);
            $(app.DE.step2badge).removeClass(app.DE.badgeActive);
            $(app.DE.step3badge).removeClass(app.DE.badgeActive);
            $(app.DE.step4badge).removeClass(app.DE.badgeActive);
            $(app.DE.uploadinfo).hide();
            $(app.DE.step1imgcheck).attr('src', app.Images.loader);
            $(app.DE.uploadcontrols).hide();
            $(app.DE.btnPrevious1).removeAttr('disabled');
            $(app.DE.btnPrevious2).removeAttr('disabled');
            $(app.DE.btnPrevious3).removeAttr('disabled');
            $(app.DE.radioBtnNew).prop('checked', true);
        }
    }
    this.EntityTypeChange = function () {
        app.hideSteps();
        app.showStep(app.DE.step0);
        $(app.DE.divUpdExcel).hide();
        $(app.DE.divUpdJson).hide();
        $(app.DE.divUpdKML).hide();
        $(app.DE.btnUpload).show();
        $(app.DE.lblMessage).empty();
        $(app.DE.clearKMLFile).hide();
        $(app.DE.clearExcelFile).hide();
        $(app.DE.clearJsonFile).hide();
        $(app.DE.clearDxfFile).hide();
        $(app.DE.clearTabFile).hide();
        $(app.DE.updKML).val(null);
        $(app.DE.updExcel).val(null);
        $(app.DE.updJson).val(null);
        $(app.DE.updDxf).val(null);
        $(app.DE.updTab).val(null);
        $(app.DE.updSHP).val(null);
        $(app.DE.divUpdShape).hide();
        $(app.DE.divUpdDxf).hide();
        $(app.DE.divUpdTab).hide();
        $(app.DE.clearShapeFile).hide();
    }
    this.FileUploadStart = function () {
        app.hideSteps();
         
        let isImportDataActivetab = app.activeTab();

        if (isImportDataActivetab) {
            app.showStep(app.DE.stepID1);
            app.currentStep = app.Steps.stepID1;
            $('#' + app.currentStep + app.DE.status).text('Uploading file...');
            $(app.DE.stepID1).find(app.DE.imgcheck).attr('src', app.Images.loader);
            $(app.DE.btnImpDataPrevious1).attr('disabled', 'disabled');
        }
        else {
            app.showStep(app.DE.step1);
            app.currentStep = app.Steps.step1;
            $('#' + app.currentStep + app.DE.status).text('Uploading file...');
            $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.loader);
            $(app.DE.btnPrevious1).attr('disabled', 'disabled');
            //$(app.DE.btnNext).attr('disabled', 'disabled');
            $(app.DE.btnNextStep2).attr('disabled', 'disabled');
        }

    }
    this.FileUploadedSuccess = function () {
        app.hideSteps();
        let isImportDataActivetab = app.activeTab();
        if (isImportDataActivetab) {
            app.showStep(app.DE.stepID1);
            $(app.DE.stepID1badge).addClass(app.DE.badgeActive);
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.DATA_FETCHED_SUCCESSFULLY);
            $(app.DE.stepID1).find(app.DE.imgcheck).attr('src', app.Images.check);
            $(app.DE.btnImpDataPrevious1).removeAttr('disabled');
        }
        else {
            app.showStep(app.DE.step1);
            $(app.DE.step1badge).addClass(app.DE.badgeActive);
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXCEL_UPLOADED_SUCCESSFULLY);
            $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
            //$(app.DE.btnNext).removeAttr('disabled');
            $(app.DE.btnNextStep2).removeAttr('disabled');
            $(app.DE.btnPrevious1).removeAttr('disabled');
        }

    }
    this.FileUploadedFailed = function (message) {
        app.hideSteps();
        app.showStep(app.DE.step0);
        $(app.DE.lblMessage).html(message);

    }

    this.FileInvalidInputs = function (message) {
        let _result = $('<textarea />').html(message).text();
        app.hideSteps();
        app.showStep(app.DE.step2);
        alert(_result);
    }
    this.FileInvalidFile = function (message) {
        app.hideSteps();
        app.showStep(app.DE.step0);
        $(app.DE.lblMessage).html(message);
    }
    this.Validating = function () {

        let isImportDataActivetab = app.activeTab();

        if (isImportDataActivetab) {
            app.currentStep = app.Steps.stepID3;
            $('#' + app.currentStep + app.DE.status).show();
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.VALIDATION_STARTED);
            app.hideSteps();
            app.showStep(app.DE.stepID3);
            $(app.DE.imgcheck).attr('src', app.Images.loader).show();
            // $(app.DE.divTotalReordsdetails).hide();
            $(app.DE.btnSingleDone).hide();
            $(app.DE.TotalRecordID).hide();
            $(app.DE.ValidRecordID).hide();
            $(app.DE.InvalidRecordID).hide();
            $(app.DE.btnIDExecute).attr('disabled', 'disabled').show();
        }
        else {
            app.currentStep = app.Steps.step3;
            $('#' + app.currentStep + app.DE.status).show();
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.VALIDATION_STARTED);
            app.hideSteps();
            app.showStep(app.DE.step3);
            $(app.DE.imgcheck).attr('src', app.Images.loader).show();
            $(app.DE.uploadinfo).hide();
            $(app.DE.TotalRecord).hide();
            $(app.DE.ValidRecord).hide();
            $(app.DE.InvalidRecord).hide();
            $(app.DE.btnExecute).attr('disabled', 'disabled').show();
            $(app.DE.btnPrevious3).attr('disabled', 'disabled').show();
        }
    }
    this.Validated = function (result) {
         
        app.hideSteps();
        $(app.DE.btnSingleDone).hide();
        let isImportDataActivetab = app.activeTab();

        if (isImportDataActivetab) {
            app.showStep(app.DE.stepID3);
            app.currentStep = app.Steps.stepID3;
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.VALIDATION_DONE);
            $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.check);
            $(app.DE.stepID3badge).addClass(app.DE.badgeActive);
            $(app.DE.uploadinfoID).show();
            if (result != null) {
                $.each(result, function (i, item) {

                    let downloadIcon = '';
                    var str = '';
                    if (item != '') {
                         

                        downloadIcon = ($(app.DE.downloadFailedLogsID).attr("data-uploadid", result[i].id));
                        //$(app.DE.divTotalReordsdetails).append(($(app.DE.EntityName).text(item.entity_type)))  
                        //.append(':')
                        //.append('Total Record :').append(item.total_record)
                        //.append('| ')
                        //.append('Valid Record :').append(item.success_record)
                        //.append('| ')
                        //.append('InValid Record :').append(item.failed_record)
                        //.append(($(app.DE.downloadFailedLogsID).attr("data-uploadid", item.id)))
                        //.append('<br />');
                        //$(app.DE.downloadFailedLogsID).attr("data-uploadid", item.id)

                        str += '<div id="validate-summary">' + '<b>' + result[i].entity_type + '</b>' + ': ' + MultilingualKey.SI_OSP_GBL_GBL_GBL_035 + ':' + result[i].total_record + '<b>|</b> ' + MultilingualKey.SI_OSP_DU_NET_FRM_024 + ':'
                            + result[i].success_record + '<b>|</b> ' + MultilingualKey.SI_OSP_DU_NET_FRM_025 + ':' + result[i].failed_record;
                        str += '<div style="float: right;"><a title="Execute ' + result[i].entity_type + '" id="lnkExecuteImportDataLogs"  name="lnkExecuteImportDataLogs" data-entitytype =' + result[i].entity_type + '   data-ischild=' + result[i].is_child_entity + '  data-uploadId=' + result[i].id + ' class="icon-upload  ' + (result[i].is_child_entity == true ? 'dvdisabled' : '') + '">';
                        str += '</a></div><div style="float: right;margin-right: 19px;"><a title="Download ' + result[i].entity_type + ' Log" name="lnkdownloadImportDataLogs"  data-uploadId=' + result[i].id + ' class="icon-download">';
                        str += '</a></div></div>';
                    }
                    $(app.DE.divTotalReordsdetails).append(str);
                }

                );
            }
        }
        else {
            app.showStep(app.DE.step3);
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.VALIDATION_DONE);
            $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.check);
            $(app.DE.step3badge).addClass(app.DE.badgeActive);
            $(app.DE.uploadinfo).show();

            $(app.DE.TotalRecord).show();
            $(app.DE.ValidRecord).show();
            $(app.DE.InvalidRecord).show();
            if (result != null) {
                $(app.DE.TotalRecord).text(result.total_record);
                $(app.DE.ValidRecord).text(result.success_record);
                $(app.DE.InvalidRecord).text(result.failed_record);
            }
            $(app.DE.btnPrevious3).removeAttr('disabled');
            $(app.DE.btnExecute).removeAttr('disabled');
            $(app.DE.btnIDExecute).removeAttr('disabled');
        }

        $(document).on("click", 'a[name=lnkdownloadImportDataLogs]', function (e) {
             
            let uploadId = $(this).data("uploadid");
            app.fnDownloadUploadLogs(uploadId, 'ALL')
        });


        $(document).on("click", '#lnkExecuteImportDataLogs', function (e) {
             
            showProgress();
            //let uploadId = $(this).data("uploadid");
            var $entityBtn = $(this);
            let entity_type = $entityBtn.data("entitytype");
            ajaxReq('DataUploader/ProcessImportData', { planId: app.planId, entity_type: entity_type }, true, function (resp) {
                if (resp.status == app.StatusMessages.OK) {
                    app.showHideControls(app.Actions.SINGLE_EXECUTED);
                    $entityBtn.addClass('dvdisabled');
                    alert(entity_type + " Executed Successfully!");
                    hideProgress();
                    if (!$entityBtn.data('ischild')) {
                        var noOfParentEntity = $("#validate-summary a[data-ischild=false]").length;
                        var noOfProcessedParentEntity = $("#validate-summary a[data-ischild=false]").filter(function (index) {
                            return $(this).hasClass("dvdisabled");
                        }).length;
                        if (noOfParentEntity === noOfProcessedParentEntity) {
                            $("#validate-summary a[data-ischild=true]").removeClass('dvdisabled');
                        }
                    } else {
                        if ($("#validate-summary a[data-ischild].dvdisabled").length == $("#validate-summary a[data-ischild]").length) {
                            $("#btnSingleDone").trigger('click');
                        }
                    }
                }
                else {
                    hideProgress();
                    alert(entity_type + " Execution Failed!");
                    $entityBtn.addClass('dvdisabled');
                    app.showHideControls(app.Actions.EXECUTION_FAILED);
                }
            }, false, false);

        });


    }
    this.Execute = function () {

        let isImportDataActivetab = app.activeTab();

        if (isImportDataActivetab) {
            app.currentStep = app.Steps.stepID4;
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXECUTING);
            $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.loader);
            app.hideSteps();
            app.showStep(app.DE.stepID4);
            $(app.DE.btnIDExecute).attr('disabled', 'disabled');
        }
        else {
            app.currentStep = app.Steps.step4;
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXECUTING);
            $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.loader);
            app.hideSteps();
            $(app.DE.btnPrevious3).hide();
            $(app.DE.btnExecute).attr('disabled', 'disabled');
            app.showStep(app.DE.step4);
            $(app.DE.btnDone).attr('disabled', 'disabled').show();
        }

        //$(app.DE.btnPrevious2).hide();
        // $(app.DE.btnPrevious3).hide();
        //$(app.DE.btnExecute).attr('disabled', 'disabled');
        //$(app.DE.btnIDExecute).attr('disabled', 'disabled');
        //$(app.DE.btnDone).attr('disabled', 'disabled').show();
    }
    this.Executed = function () {
        $(app.DE.step4badge).addClass(app.DE.badgeActive);
        $(app.DE.stepID4badge).addClass(app.DE.badgeActive);
        $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.check);
        $(app.DE.btnExecute).hide();
        $(app.DE.btnIDExecute).hide();
        $(app.DE.btnDone).removeAttr('disabled');
        $(app.DE.btnIDDone).removeAttr('disabled');
    }
    this.Single_Executed = function () {
        //$(app.DE.step4badge).addClass(app.DE.badgeActive);
        //$(app.DE.stepID4badge).addClass(app.DE.badgeActive);
        $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.check);
        //$(app.DE.btnExecute).hide();
        //$(app.DE.btnIDExecute).hide();
        $(app.DE.btnSingleDone).show();
        //$(app.DE.btnIDDone).removeAttr('disabled');
    }
    this.ExecutionFailed = function () {
        $(app.DE.step4badge).addClass(app.DE.badgeActive);
        $(app.DE.stepID4badge).addClass(app.DE.badgeActive);
        $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXECUTION_FAILED);
        $('#' + app.currentStep + app.DE.imgcheck).attr('src', app.Images.failed);
        $(app.DE.btnDone).removeAttr('disabled');
        $(app.DE.btnIDDone).removeAttr('disabled');
    }
    this.Done = function () {
         
        app.showHideControls(app.Actions.PAGE_LOAD);
        $(app.DE.Uploadlogs).trigger("click");
        $(app.DE.btnExecute).hide();
        $(app.DE.btnIDExecute).hide();
    }
    this.Previous1 = function () {
        app.hideSteps();

        let isImportDataActivetab = app.activeTab();

        if (isImportDataActivetab) {
            app.showStep(app.DE.stepID0);
            $(app.DE.stepID1badge).removeClass(app.DE.badgeActive);
            $(app.DE.btnIDUpload).show();
        }
        else {
            app.showStep(app.DE.step0);
            $(app.DE.step1badge).removeClass(app.DE.badgeActive);
            $(app.DE.btnUpload).show();
            $(app.DE.clearKMLFile).hide();
            $(app.DE.clearShapeFile).hide();
            //$(app.DE.clearExcelFile).hide();
        }


    }
    this.Previous2 = function () {
        app.hideSteps();
        app.showStep(app.DE.step1);
        $(app.DE.step2badge).removeClass(app.DE.badgeActive);
        $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXCEL_UPLOADED_SUCCESSFULLY);
        $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
        //$(app.DE.btnNext).removeAttr('disabled');
        $(app.DE.btnNextStep2).removeAttr('disabled');
        $(app.DE.btnPrevious1).removeAttr('disabled');

    }
    this.Previous3 = function () {
        app.hideSteps();
         
        let isImportDataActivetab = app.activeTab();

        if (isImportDataActivetab) {
            app.showStep(app.DE.stepID1);
            $(app.DE.stepID3badge).removeClass(app.DE.badgeActive);
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.EXCEL_UPLOADED_SUCCESSFULLY);
            $(app.DE.stepID1).find(app.DE.imgcheck).attr('src', app.Images.check);
            $(app.DE.btnImpDataPrevious1).removeAttr('disabled');

        }
        else {
            app.showStep(app.DE.step2);
            $(app.DE.step3badge).removeClass(app.DE.badgeActive);
            $('#' + app.currentStep + app.DE.status).text(app.StatusMessages.COLUMN_MAPPED_SUCCESS);
            $(app.DE.step1).find(app.DE.imgcheck).attr('src', app.Images.check);
            //$(app.DE.btnNext).removeAttr('disabled');
            $(app.DE.btnNextStep2).removeAttr('disabled');
            $(app.DE.btnPrevious1).removeAttr('disabled');
        }



    }
    this.ShowUploadSummary = function () {
        app.showHideControls(app.Actions.PAGE_LOAD);
        ajaxReq('DataUploader/getUploadSummary', null, true, function (resp) {
            $(app.DE.UploadLogsContainer).html(resp);
        }, false, false);
    }

    this.ExecuteData = function () {
         
        app.showHideControls(app.Actions.EXECUTE);
        ajaxReq('DataUploader/ProcessData', { uploadId: app.uploadId }, true, function (resp) {
            if (resp.status == app.StatusMessages.OK) {
                app.showHideControls(app.Actions.EXECUTED);
            }
            else {
                app.showHideControls(app.Actions.EXECUTION_FAILED);
            }
        }, false, false);
    }

    this.ExecuteImportData = function () {
         
        app.showHideControls(app.Actions.EXECUTE);
        ajaxReq('DataUploader/ProcessImportData', { planId: app.planId }, true, function (resp) {
            if (resp.status == app.StatusMessages.OK) {
                app.showHideControls(app.Actions.EXECUTED);
            }
            else {
                app.showHideControls(app.Actions.EXECUTION_FAILED);
            }
        }, false, false);
    }
    this.ValidateData = function () {
         
        app.showHideControls(app.Actions.VALIDATING);
        ajaxReq('DataUploader/ValidateData', { uploadId: app.uploadId }, true, function (resp) {
             
            if (resp.status == app.StatusMessages.OK) {
                app.isValidated = true;
                app.showHideControls(app.Actions.VALIDATED, null, resp);
                if (resp.failed_record > 0) {
                    $(app.DE.downloadFailedLogs).show();
                    $(app.DE.DownloadIcon).show();
                }
                if (resp.success_record == 0 && resp.failed_record > 0) {
                    $(app.DE.btnExecute).attr('disabled', 'disabled');
                }
                else {
                    $(app.DE.btnExecute).removeAttr('disabled');
                }
            }
            else {
                alert(app.StatusMessages.VALIDATION_ISSUE);
            }
        }, false, false);
    }

    this.ValidateDataImport = function () {
         
        app.showHideControls(app.Actions.VALIDATING);
        ajaxReq('DataUploader/Insert_ValidateData', { planId: app.planId }, true, function (resp) {
             
            if (resp[0].status == app.StatusMessages.OK) {
                app.isValidated = true;
                app.showHideControls(app.Actions.VALIDATED, null, resp);
                if (resp.failed_record > 0) {
                    $(app.DE.downloadFailedLogsID).show();
                    $(app.DE.spDownloadIconID).show();
                }
                if (resp.success_record == 0 && resp.failed_record > 0) {
                    $(app.DE.btnIDExecute).attr('disabled', 'disabled');
                }
                else {
                    $(app.DE.btnIDExecute).removeAttr('disabled');
                }
            }
            else {
                alert(app.StatusMessages.VALIDATION_ISSUE);
            }
        }, false, false);
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
    this.ValidateExcelFile = function (excelFiles) {
        // check excel file selected or not.
        if (excelFiles.length == 0) {
            $(app.DE.lblMessage).html(app.StatusMessages.EXCEL_FILE_REQUIRED);
            return false;
        }
        // check file extension.
        if (excelFiles.length > 0) {
            if (!app.Validatefiletype(excelFiles[0].name.toLowerCase(), $(app.DE.updExcel))) {
                $(app.DE.lblMessage).html(app.StatusMessages.VALID_EXCEL_FILE_REQUIRED + $(app.DE.updExcel).get(0).accept);
                return false;
            }
        }
        return true;
    }
    this.ValidateKMLFile = function (kmlFiles) {
        // check KML file selected or not.
        if (kmlFiles.length == 0) {
            $(app.DE.lblMessage).html(app.StatusMessages.KML_FILE_REQUIRED);
            return false;
        }
        // check file extension.
        if (kmlFiles.length > 0) {
            if (!app.Validatefiletype(kmlFiles[0].name.toLowerCase(), $(app.DE.updKML))) {
                $(app.DE.lblMessage).html(app.StatusMessages.VALID_KML_FILE_REQUIRED + $(app.DE.updKML).get(0).accept);
                return false;
            }
        }
        return true;
    }
    this.ValidateJSONFile = function (jsonFiles) {
        // check KML file selected or not.
        if (jsonFiles.length == 0) {
            $(app.DE.lblMessage).html(app.StatusMessages.JSON_FILE_REQUIRED);
            return false;
        }
        // check file extension.
        if (jsonFiles.length > 0) {
            if (!app.Validatefiletype(jsonFiles[0].name.toLowerCase(), $(app.DE.updJson))) {
                $(app.DE.lblMessage).html(app.StatusMessages.VALID_JSON_FILE_REQUIRED + $(app.DE.updJson).get(0).accept);
                return false;
            }
        }
        return true;
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
    this.ValidateDxfFile = function (dxfFiles) {
        // check Shape file selected or not.
        if (dxfFiles.length == 0) {
            $(app.DE.lblMessage).html(app.StatusMessages.DXF_FILE_REQUIRED);
            return false;
        }

        // check file extension.
        if (dxfFiles.length > 0) {
            for (var i = 0; i < dxfFiles.length; i++) {
                if (!app.Validatefiletype(dxfFiles[i].name.toLowerCase(), $(app.DE.updDxf))) {
                    $(app.DE.lblMessage).html(app.StatusMessages.VALID_DXF_FILE_REQUIRED + $(app.DE.updDxf).get(0).accept);
                    return false;
                }
            }
        }
        return true;
    }
    this.ValidateTabFile = function (tabFiles) {
        // check Shape file selected or not.
        if (tabFiles.length == 0) {
            $(app.DE.lblMessage).html(app.StatusMessages.TAB_FILE_REQUIRED);
            return false;
        }

        // check file extension.
        if (tabFiles.length > 0) {
            for (var i = 0; i < tabFiles.length; i++) {
                if (!app.Validatefiletype(tabFiles[i].name.toLowerCase(), $(app.DE.updTab))) {
                    $(app.DE.lblMessage).html(app.StatusMessages.VALID_TAB_FILE_REQUIRED + $(app.DE.updTab).get(0).accept);
                    return false;
                }
            }
        }
        return true;
    }
    this.UploadData = function () {

        if (window.FormData !== undefined) {
            // var _templateType = $('#divTemplateType>.radio-inline>input[type="radio"]:checked').attr('data-template-type');
            var _templateType = $('#ddlTemplateType :selected').val();
            // Create FormData object
            var fileData = new FormData();
            let entity = $(app.DE.selEntity).val();
            //read excel files
            var ExcelFile = $(app.DE.updExcel).get(0);
            var excel = ExcelFile.files;

            if (_templateType == "0") {
                $(app.DE.lblMessage).html(app.StatusMessages.TEMPLATE_FILE_TYPE_REQUIRED);
                return false;
            }
            else {
                $(app.DE.lblMessage).html('');
            }

            if (excel.length == 0 && $(app.DE.updKML).get(0).files.length == 0 && $(app.DE.updSHP).get(0).files.length == 0 && $(app.DE.updDxf).get(0).files.length == 0 && $(app.DE.updTab).get(0).files.length == 0 && $(app.DE.updJson).get(0).files.length == 0) {
                $(app.DE.lblMessage).html(app.StatusMessages.VALID_FILE_REQUIRED);
                return false;
            }
            if (_templateType == "EXL" && !app.ValidateExcelFile(excel)) return;
            for (var i = 0; i < excel.length; i++) {
                fileData.append(excel[i].name, excel[i]);
            }

            // read kml files if geom type is Line or polygon
            var geomType = $(app.DE.option_selected, $(app.DE.selEntity)).attr('label');
            //if (geomType == "Line") {

            if (_templateType == "KML" || _templateType == "KMZ") {
                var KMLFile = $(app.DE.updKML).get(0);
                var kml = KMLFile.files;
                if (!app.ValidateKMLFile(kml)) return;
                for (var i = 0; i < kml.length; i++) {
                    fileData.append(kml[i].name, kml[i]);
                }
            }

            if (_templateType == "SHP") {
                var SHPFile = $(app.DE.updSHP).get(0);
                var SHP = SHPFile.files;
                if (!app.ValidateSHPFile(SHP)) return;
                for (var i = 0; i < SHP.length; i++) {
                    fileData.append(SHP[i].name, SHP[i]);
                }
            }

            if (_templateType == "DXF") {
                if (!app.validate('inputpicker-1') || $('table .inputpicker-active').attr('data-value') == undefined) { return false; }
                var SourceId = $('table .inputpicker-active').attr('data-value');
                fileData.append('SourceId', SourceId);
                var DxfFile = $(app.DE.updDxf).get(0);
                var Dxf = DxfFile.files;
                if (!app.ValidateDxfFile(Dxf)) return;
                for (var i = 0; i < Dxf.length; i++) {
                    fileData.append(Dxf[i].name, Dxf[i]);
                }

            }

            if (_templateType == "TAB") {
                var TABFile = $(app.DE.updTab).get(0);
                var Tab = TABFile.files;
                if (!app.ValidateTabFile(Tab)) return;
                for (var i = 0; i < Tab.length; i++) {
                    fileData.append(Tab[i].name, Tab[i]);
                }

            }
            if (_templateType == "JSON") {
                var JSONFile = $(app.DE.updJson).get(0);
                var JSON = JSONFile.files;
                if (!app.ValidateJSONFile(JSON)) return;
                for (var i = 0; i < JSON.length; i++) {
                    fileData.append(JSON[i].name, JSON[i]);
                }

            }
            fileData.append('entity', entity);
            app.showHideControls(app.Actions.FILE_UPLOAD_START);
            ajaxReqforFileUpload('Datauploader/UploadFiles', fileData, true, function (result) {
                let status = result.status;
                let message = result.status + " :" + (result.err_description || result.error_msg);
                 
                switch (status) {

                    case app.StatusMessages.OK:
                        app.uploadId = result.id;
                        app.showHideControls(app.Actions.FILE_UPLOADED_SUCCESS, null, result);
                        app.getMappingTemplate(0);
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


    this.UploadIDData = function () {

        if (window.FormData !== undefined) {

            var fileData = new FormData();
            var _planId = $('#txtPlan').val();
            var _selUserName = $('#selUserName').val();
            var _selPlanName = $('#selPlanName').val();

            if (_selUserName == "0" || _selUserName.trim() == '') {
                $('#selUserName').next().addClass('input-validation-error').removeClass('valid');
                return false;
            }
            else {
                $(app.DE.IDlblMessage).html('');
                $('#selUserName').next().addClass('valid').removeClass('input-validation-error');
            }

            if (_selPlanName == "0" || _selPlanName.trim() == '') {
                $('#selPlanName').next().addClass('input-validation-error').removeClass('valid');
                return false;
            }
            else {
                $(app.DE.IDlblMessage).html('');
                $('#selPlanName').next().addClass('valid').removeClass('input-validation-error');
            }


            //if (_planId == "0" || _planId.trim() == '') {
            //    $(app.DE.plantxt).css("border-color", "red");
            //    return false;
            //}
            //else {
            //    $(app.DE.IDlblMessage).html('');
            //    $(app.DE.plantxt).css("border-color", "");
            //}
            app.planId = $('#selPlanName :selected').val();
            fileData.append('planId', app.planId);
            app.showHideControls(app.Actions.FILE_UPLOAD_START);
            ajaxReqforFileUpload('Datauploader/ImportData', fileData, true, function (result) {
                 
                let status = result.status;
                let message = result.status + " :" + (result.err_description || result.error_msg);
                switch (status) {

                    case app.StatusMessages.OK:
                        app.uploadId = result.id;
                        app.showHideControls(app.Actions.FILE_UPLOADED_SUCCESS, null, result);
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




    this.fnDownloadUploadLogs = function (id, status) {
        isAuthenticate();
        app.checkLogFileExist(id, status)

    }

    this.DownloadLogs = function () {
        app.fnDownloadUploadLogs(app.uploadId, 'ALL')
    }
    this.bindEvents = function () {

        $(app.DE.selEntity).change(function (e) {
             
            app.uploadId = 0;
            let entityType = $(this).val();
            app.showHideControls(app.Actions.ENTITY_TYPE_CHANGE);
            $("#ddlTemplateType")[0].selectedIndex = 0;
            $('#ddlTemplateType').trigger("chosen:updated");
            if (entityType == '0') {
                $(app.DE.uploadcontrols).hide();
                $(app.DE.DownloadTemplate).hide();
                $(app.DE.btnUpload).hide();
            }
            else {
                app.resetValue();
                $(app.DE.uploadcontrols).show();
                $(app.DE.DownloadTemplate).show();

                //commented by shazia 
                //var _templateType = $('#ddlTemplateType :selected').val();
                //if (_templateType == "DXF") {
                //    $(app.DE.divDxfSourceId).show();
                //    $(app.DE.divUpdDxf).show();
                //} else {
                //    $(app.DE.divDxfSourceId).hide();
                //    $(app.DE.divUpdExcel).show();
                //}
                app.templateType();

                $(app.DE.btnUpload).show();
                $('#inlineRadio2').prop('checked', true);
            }
            $(app.DE.btnIDUpload).show();
            //var geomType = $(app.DE.option_selected, this).attr('label');
            //if (geomType == "Line") {
            //    $(app.DE.divUpdKML).show();
            //}

        });
        $(app.DE.btnUpload).on("click", function () {
             
            if (app.uploadId == 0) {
                isAuthenticate();
                app.UploadData();
            }
            else {
                app.showHideControls(app.Actions.FILE_UPLOADED_SUCCESS, null);
            }
        });


        $(app.DE.btnIDUpload).on("click", function () {
             
            if (app.uploadId == 0) {
                isAuthenticate();
                app.UploadIDData();
            }
            else {
                app.showHideControls(app.Actions.FILE_UPLOADED_SUCCESS, null);
            }
        });


        $(app.DE.btnExecute).on("click", function () {
            app.ExecuteData();
        });

        $(app.DE.btnIDExecute).on("click", function () {
            app.ExecuteImportData();
        });

        $(app.DE.btnDone).on("click", function () {
             
            app.showHideControls(app.Actions.DONE);
            app.showHideControls(app.Actions.PAGE_LOAD);
        });


        $(app.DE.btnIDDone).on("click", function () {
            app.showHideControls(app.Actions.DONE);
            app.showHideControls(app.Actions.PAGE_LOAD);
        });

        $(app.DE.btnSingleDone).on("click", function () {
            ajaxReq('DataUploader/ProcessImportData', { planId: app.planId, entity_type: null, isSingleExecutionFinished: true }, true, function (resp) {
                if (resp.status == app.StatusMessages.OK) {
                    app.showHideControls(app.Actions.DONE);
                    app.showHideControls(app.Actions.PAGE_LOAD);
                }
                else {
                    app.showHideControls(app.Actions.EXECUTION_FAILED);
                }
            }, false, false);
        });

        $(app.DE.btnPrevious1).on("click", function () {
            app.resetValue();
            app.showHideControls(app.Actions.PREVIOUS_1);

        });

        $(app.DE.btnImpDataPrevious1).on("click", function () {
             
            app.showHideControls(app.Actions.PREVIOUS_1);
        });


        $(app.DE.btnPrevious2).on("click", function () {
            app.showHideControls(app.Actions.PREVIOUS_2);
        });

        $(app.DE.btnPrevious3).on("click", function () {
            app.resetValue();
            app.showHideControls(app.Actions.PREVIOUS_3);
        });

        $(app.DE.btnImpDataPrevious3).on("click", function () {
            app.isValidated = false;
            $(app.DE.divTotalReordsdetails).text('');
            app.showHideControls(app.Actions.PREVIOUS_3);
        });

        $(app.DE.btnNextStep2).on("click", function () {
             
            if (app.isValidated == false)
                app.ValidateData();
            else {
                app.showHideControls(app.Actions.VALIDATING);
                app.showHideControls(app.Actions.VALIDATED);
            }
        });
        $(app.DE.btnNext).on("click", function () {
            app.showHideControls(app.Actions.COLUMN_MAPPING);
        });

        $(app.DE.btnIDNext).on("click", function () {
             
            if (app.isValidated == false)
                app.ValidateDataImport();
            else {
                app.showHideControls(app.Actions.VALIDATING);
                app.showHideControls(app.Actions.VALIDATED);
            }
        });

        $(app.DE.updExcel).change(function () {
            $(app.DE.clearExcelFile).show();
            app.uploadId = 0;
            app.isValidated = false;
        });
        $(app.DE.updJson).change(function () {
            $(app.DE.clearJsonFile).show();
            app.uploadId = 0;
            app.isValidated = false;
        });
        $(app.DE.updKML).change(function () {
            $(app.DE.clearKMLFile).show();
            app.uploadId = 0;
            app.isValidated = false;
        });
        $(app.DE.clearKMLFile).on("click", function () {
            $(app.DE.updKML).val(null);
            $(this).hide();
        });
        $(app.DE.clearExcelFile).on("click", function () {
            $(app.DE.updExcel).val(null);
            app.uploadId = 0;
            $(this).hide();
        });
        $(app.DE.clearJsonFile).on("click", function () {
            $(app.DE.updJson).val(null);
            app.uploadId = 0;
            $(this).hide();
        });
        $(app.DE.clearTabFile).on("click", function () {
            $(app.DE.updTab).val(null);
            $(this).hide();
        });

        $(app.DE.clearDxfFile).on("click", function () {
            $(app.DE.upd).val(null);
            $(this).hide();
        });
        $(app.DE.clearShapeFile).on("click", function () {
            $(app.DE.updSHP).val(null);
            $(this).hide();
        });
        $(app.DE.DownloadTemplate).on('click', function () {
            isAuthenticate();
            let _entityType = $(app.DE.selEntity).val();
            let _templateType = $('#ddlTemplateType :selected').val();
            var geomType = $(app.DE.option_selected, $(app.DE.selEntity)).attr('label');
            if (_entityType == '0') {
                $(app.DE.lblMessage).html(app.StatusMessages.ENTITY_REQUIRED);
            }
            else if (_templateType == '0') {
                $(app.DE.lblMessage).html(app.StatusMessages.TEMPLATE_FILE_TYPE_REQUIRED);
            }
            else {
                $(app.DE.lblMessage).html("");
                $.ajax({
                    url: applicationUrl + '/Datauploader/CheckTemplateExist',
                    type: "POST",
                    processData: true, // Not to process data
                    data: { entityType: _entityType },
                    success: function (result) {
                        if (result.status) {
                            window.location = appRoot + 'DataUploader/downloadTemplate?entityType=' + _entityType + '&templateType=' + _templateType + '&geomType=' + geomType;
                        } else {
                            alert(result.message);
                        }
                    },
                    error: function (err) {
                        alert(err.statusText);
                    }
                });
            }
        });

    }
    this.RemoveOldFeature = function () {
         
        si.map.data.forEach(function (feature) {
            si.map.data.remove(feature);
        });
    }

    this.fnShowEntityonMap = function (id) {
        //if ($(event).hasClass("glyphicon-eye-open")) {
         

        var bounds = new google.maps.LatLngBounds();
        ajaxReq('DataUploader/ShowOnMap', { id: id }, false, function (resp) {
            var eCheck = resp.features == null ? false : true;
            if (eCheck) {
                app.RemoveOldFeature();
                si.map.data.addListener('mouseover', app.mouseInToRegion);
                si.map.data.addListener('mouseout', app.mouseOutOfRegion);
                app.Upload_SummaryJSONResp = resp;
                app.loadMapShapes(true);
                //$('#closeModalPopup').trigger("click");
                $(popup.DE.MinimizeModel).trigger("click");
            }
            else {
                alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_066);
            }
        }, true, false);
    }


    this.mouseInToRegion = function mouseInToRegion(e) {
         
        var feature = e.feature;

        var geometry = feature.getGeometry().g;
        if (geometry === undefined)
            geometry = feature.getGeometry().i;

        if (feature.getProperty('entity_name').toUpperCase() != 'LANDBASE') {
            //app._focusMe("Line", geometry, 'black');
            var html = "<table style='font-size:11px;padding:1px;'>"
            html += "<tr style='font-weight: bold;color: #02747d;'>";
            html += "<td><b>Network ID :</b></td><td>" + feature.getProperty('display_name') + "</td>";
            html += "</tr>";
            html += "<tr style='font-weight: bold;color: #02747d;'>";
            html += "<td><b>Entity Type :</b></td>";
            html += "<td>" + feature.getProperty('entity_name') + "</td>";
            html += "</tr>";
            html += "<table>";

        }
        else {
            var html = "<table style='font-size:11px;padding:1px;'>"
            html += "<tr style='font-weight: bold;color: #02747d;'>";
            html += "<td><b>Network ID :</b></td><td>" + feature.getProperty('network_id') + "</td>";
            html += "</tr>";
            html += "<tr style='font-weight: bold;color: #02747d;'>";
            html += "<td><b>Name :</b></td>";
            html += "<td>" + feature.getProperty('name') + "</td>";
            html += "</tr>";
            html += "<tr style='font-weight: bold;color: #02747d;'>";
            html += "<td><b>Layer Name :</b></td>";
            html += "<td>" + feature.getProperty('display_name') + "</td>";
            html += "</tr>";
            html += "<table>";
        }
        infowindow.setContent(html);
        infowindow.setPosition(e.latLng);
        infowindow.setOptions({ pixelOffset: new google.maps.Size(0, -34) });
        infowindow.open(si.map);

    }
    this.mouseOutOfRegion = function mouseOutOfRegion(e) {
        infowindow.close();
        //e.feature.setProperty('state', 'normal');
        //app.removeInfoHoverItem();
    }

    this.loadMapShapes = function loadMapShapes(bSetBound) {
         
        var JSONData = app.concatGeoJSON(app.CableViewJSONResp, app.Upload_SummaryJSONResp);
        si.map.data.addGeoJson(JSONData, { idPropertyName: 'id' });
        var bounds = new google.maps.LatLngBounds();
        si.map.data.setStyle(function (feature) {
             

            var geomstyle
            var type = feature.getGeometry().getType();
            // var entity_name = feature.i.entity_name;
            var entity_name = feature.getProperty("entity_name");
            switch (type) {
                case "LineString":
                    //var fiberType = feature.getProperty('fiber_type');
                    //var fiberStatus = feature.getProperty('fiber_status').toUpperCase();

                    //var fiberColor = si.FiberColor(fiberType);
                    //var isOwnOrLease = si.IsOwnOrLease(fiberStatus);

                    //var color = feature.getProperty('color');
                    //geomstyle = si.GetFiberPath(color);

                    //if (bSetBound) {

                    //}
                    app.SetFiberBound(bounds, feature);
                    break;
                case "Polygon":
                    geomstyle = {
                        strokeWeight: 1,
                        strokeColor: '#000',
                        zIndex: 0.5,
                        fillColor: '#FF0000',
                        fillOpacity: 0.75
                    }
                    feature.getGeometry().forEachLatLng(function (latlng) {
                        bounds.extend(latlng);
                    });
                    if (bSetBound) {
                        si.map.fitBounds(bounds);
                    }
                    break;
                case "Point":
                     
                    //for(var i=0;i<objJSON.features.length;i++){
                    //si.SetPointTypeBound(bounds, objJSON.features[i].geometry);
                    if (entity_name.toUpperCase() != 'LANDBASE') {
                        var imageUrl = baseUrl + appRoot + '/Content/images/icons/lib/KMLICONS/' + entity_name.toUpperCase() + '.png';
                        geomstyle = {
                            icon: imageUrl,
                            //   title: feature.i.network_id
                            title: feature.getProperty("network_id")
                        };
                    }

                    feature.getGeometry().forEachLatLng(function (latlng) {
                        bounds.extend(latlng);
                        si.map.fitBounds(bounds);
                    });
                    //if (bSetBound) {
                    //    var objMarker = new google.maps.Marker({
                    //        icon: new google.maps.MarkerImage(appRoot + '/Content/images/Actual_Start.png')
                    //    });
                    //    //si.SetPointTypeBound(bounds, feature.getGeometry());
                    //    feature.getGeometry().forEachLatLng(function (latlng) {
                    //        bounds.extend(latlng);
                    //        si.map.fitBounds(bounds);
                    //    });

                    //    //objMarker.setMap(si.map);
                    //}
                    //}
                    break;
            }


            return geomstyle;
        });

    }
    this.SetFiberBound = function SetFiberBoundToMap(bounds, feature) {

        var geometryPath = feature.getGeometry();
        var slat, blat = 0;
        var slng, blng = 0;
        for (var i = 1; i < geometryPath.getLength(); i++) {
            var e = geometryPath.getAt(i);
            slat = ((slat < e.lat()) ? slat : e.lat());
            blat = ((blat > e.lat()) ? blat : e.lat());
            slng = ((slng < e.lng()) ? slng : e.lng());
            blng = ((blng > e.lng()) ? blng : e.lng());
            bounds.extend(new google.maps.LatLng(slat, slng));
            bounds.extend(new google.maps.LatLng(blat, blng));
        }
        si.map.fitBounds(bounds);
    }

    this.concatGeoJSON = function concatGeoJSON(g1, g2) {
        var objJSON = {};
        if (g1 == undefined) {
            objJSON = {
                "type": "FeatureCollection",
                "features": g2.features
            }
        }
        else if (g2 == undefined) {
            objJSON = {
                "type": "FeatureCollection",
                "features": g1.features
            }
        }
        else if (g1.features != undefined && g2.features != undefined) {
            objJSON = {
                "type": "FeatureCollection",
                "features": g1.features.concat(g2.features)
            }
        }
        return objJSON;
    }
    this.bindPlanName = function (obj) {
        $('#selUserName').next().addClass('valid').removeClass('input-validation-error');
        $('#selPlanName').next().addClass('valid').removeClass('input-validation-error');
        var _selUserName = $('#selUserName :selected').val();
        ajaxReq('datauploader/GetSmartPlannerPlanList', { user_name: _selUserName }, true, function (response) {
            $('#selPlanName').html('');
            var options = "<option value>--Select--</option>";
            for (var i = 0; i < response.length; i++) {
                options += '<option value="' + response[i].plan_id + '">' + response[i].plan_name + "(" + response[i].plan_type + ")" + '</option>';
            }
            $('#selPlanName').append(options).trigger("chosen:updated");
        }, false, false);
    }
    this.templateType = function (obj) {
        var _templateType = $('#ddlTemplateType :selected').val();
        // var _templateType = $(obj).attr('data-template-type');
        $(app.DE.divUpdKML).hide();
        $(app.DE.divUpdExcel).hide();
        $(app.DE.divUpdJson).hide();
        $(app.DE.divUpdTab).hide();
        $(app.DE.divUpdDxf).hide();
        $(app.DE.divUpdShape).hide();
        switch (_templateType) {
            case 'KML': { $(app.DE.divUpdKML).show(); $(app.DE.divDxfSourceId).hide(); $(app.DE.DownloadTemplate).removeAttr('disabled'); break; }
            case 'SHP': { $(app.DE.divUpdShape).show(); $(app.DE.divDxfSourceId).hide(); $(app.DE.DownloadTemplate).removeAttr('disabled'); break; }
            case 'DXF': {


                $(app.DE.divUpdDxf).show();
                $(app.DE.divDxfSourceId).show();
                $(app.DE.DownloadTemplate).removeAttr('disabled');
                ajaxReq('datauploader/getDxfSourceList', null, true, function (response) {
                    //$('#ddlSourceList').html('');
                     

                    var result = $.parseJSON(response);

                    $('#demo').inputpicker({
                        data: result,
                        //url: 'datauploader/getDxfSourceList',
                        fields: ['epsg', 'aoi', 'dsname'],
                        fieldText: 'aoi',
                        fieldValue: 'epsg',
                        filterOpen: true,
                        autoOpen: true,
                        width: '100%',
                        height: '200px'
                    });
                    $('#inputpicker-1').val("--Select--");
                    // console.log(result.length);
                    //console.log(result[0].aoi);
                    //var options = "<option value>--Select--</option>";
                    // for (var i = 0; i < result.length; i++) {
                    //     options += '<option value="' + result[i].epsg + '">' + result[i].aoi + '</option>';
                    // }
                    // $('#ddlSourceList').append(options);
                    // $('#ddlSourceList').trigger("chosen:updated");


                }, false, true);

                break;
            }
            case 'TAB': {
                $(app.DE.divUpdTab).show();
                $(app.DE.divDxfSourceId).hide();
                $(app.DE.DownloadTemplate).removeAttr('disabled');
                break;
            }
            case 'JSON': {
                $(app.DE.divUpdJson).show();
                $(app.DE.divDxfSourceId).hide();
                //$(app.DE.DownloadTemplate).attr('disabled', 'disabled');
                $(app.DE.DownloadTemplate).removeAttr('disabled');
                break;
            }
            default: { $(app.DE.divUpdExcel).show(); $(app.DE.divDxfSourceId).hide(); break; }
        }

    }
    this.columnMapping = function () {
        app.currentStep = app.Steps.step2;
        app.hideSteps();
        app.showStep(app.DE.step2);
        $(app.DE.step2badge).addClass(app.DE.badgeActive);
    }
    this.hideSteps = function () {
        let isImportDataActivetab = app.activeTab();

        if (isImportDataActivetab) {
            $(app.DE.stepID0).hide();
            $(app.DE.stepID1).hide();
            $(app.DE.stepID2).hide();
            $(app.DE.stepID3).hide();
            $(app.DE.stepID4).hide();
        }
        else {
            $(app.DE.step0).hide();
            $(app.DE.step1).hide();
            $(app.DE.step2).hide();
            $(app.DE.step3).hide();
            $(app.DE.step4).hide();
        }
    }
    this.showStep = function (obj) {
        $(obj).show();
    }
    this.getMappingTemplate = function () {
         
        let _templateId = 0;
        if ($('#ddlColumTemplates').val() > 0) { _templateId = parseInt($('#ddlColumTemplates').val()); }
        let _layerName = $(app.DE.selEntity).val();
        ajaxReq('DataUploader/getColumnMappping', { layerName: _layerName, templateId: _templateId }, true, function (resp) {
             
            $(app.DE.step2).html(resp);
        }, false, true);
    }
    this.setTemplateTame = function () {
         
        var _isValid = true;
        $.each($('#step2 select[is-Required="1"]'), function (i, item) {
            if ($(item).val() == '') { _isValid = false; $(this).next('.chosen-container').addClass('notvalid'); }
        })
        if (_isValid) {
            popup.LoadModalDialog('CHILD', 'DataUploader/templateName', null, 'Template Name', 'modal-sm');
        }
    }

    this.closeTemplate = function () {
        $('.modal-backdrop').hide();
    }

    this.saveTemplate = function () {
        if ($('#txtTemplateName').val().trim() == '') { $('#txtTemplateName').addClass('notvalid'); return false; }
        $('#hdnIsFinalMapping').val(false);
        $('#hdnTemplateName').val($('#txtTemplateName').val());
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

    this.successMapping = function (resp) {

         
        if ($('#hdnIsFinalMapping').val().toLowerCase() == 'true') {
            if (resp.status == app.Actions.INVALID_INPUTS) {
                let message = resp.status + " :" + (resp.err_description || resp.error_msg);
                app.showHideControls(app.Actions.FILE_INVALID_INPUTS, message);
            }
            else { app.ValidateData(); }
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
    this.checkLogFileExist = function (id, status) {
        ajaxReq('FileDownload/checkLogFileExist', { id: id, status: status }, false, function (resp) {
             
            if (resp.status == 'FAILED') {
                alert(resp.message);
            } else { window.location = applicationUrl + '/FileDownload/DownloadUploadLogs?id=' + id + '&status=' + status; }
        }, true, false);
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