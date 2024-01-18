var BulkFeasibility = function () {
    var bulk = this;
    this.tableInputs = undefined;
    this.bulk_API_Request_Data = {};
    this.bulkFileInput = [];
    this.FeasibilityKMLModel = {};
    this.DE = {
        "inputFile": ".input-file",
        "FeasFileUpload": "#FeasFileUpload",
        "btnChoose": "button.btn-choose",
        "fileInputControl": ".fileInput",
        "btnUpload": "#Upload",
        "btnUploadForFtth": "#UploadForFtth",
        "FeasFileUpload_ID": "FeasFileUpload",
        "tblInputs": "#tblInputs",
        "radioHTML": "<input type='radio' class='form-check-input bulkCompute' name='rdbRowRadios' title='Select to compute the Feasibility'>",
        "downloadSectionHTML": '<span class="downloadBulkReports" data-type="BOM" title="Download BOM Report"><i class="fa fa-file-excel-o fa-lg" aria-hidden="true"></i></span>&nbsp;' +
                               '<span class="downloadBulkReports" data-type="XLS" title="Download Feasibility Report"><i class="fa fa-file-excel-o fa-lg" aria-hidden="true"></i></span>&nbsp;' +
                               '<span class="downloadBulkReports" data-type="PDF" title="Download PDF"><i class="fa fa-file-pdf-o fa-lg" aria-hidden="true"></i></span>&nbsp' +
                               '<span class="downloadBulkReports" data-type="KML" title="Download KML"><i class="fa fa-map fa-lg" aria-hidden="true"></i></span></div>' +
                               '<span class="downloadBulkReports bulkSave" data-type="Save" title="Save"><i class="fa fa-save fa-lg" aria-hidden="true"></i></span></div>',
        "downloadSectionHTMLForftth":
            '<span class="downloadBulkReports" data-type="XLS" title="Download Feasibility Report"><i class="fa fa-file-excel-o fa-lg" aria-hidden="true"></i></span>&nbsp;' +
            '<span class="downloadBulkReports" data-type="PDF" title="Download PDF"><i class="fa fa-file-pdf-o fa-lg" aria-hidden="true"></i></span>&nbsp' +
            '<span class="downloadBulkReports" data-type="KML" title="Download KML"><i class="fa fa-map fa-lg" aria-hidden="true"></i></span>&nbsp' +
                               '<span class="downloadBulkReports bulkSave" data-type="Save" title="Save"><i class="fa fa-save fa-lg" aria-hidden="true"></i></span>',
        "downloadBulkReports": ".downloadBulkReports",
        "downloadTemplate": "#downloadTemplate",
        "downloadTemplateForftth":"#downloadTemplateForftth",
        "bulkSave": ".bulkSave"
    };

    this.tableColumns = [{ title: "" }, { title: MultilingualKey.Operations }, { title: MultilingualKey.Customer_Name }, { title: MultilingualKey.Customer_ID }, { title: MultilingualKey.Feasibility_ID }, { title: MultilingualKey.Start_Point_Name }, { title: MultilingualKey.Start_Lat },
                            { title: MultilingualKey.Start_Lng }, { title: MultilingualKey.Start_Buffer }, { title: MultilingualKey.End_Point_Name }, { title: MultilingualKey.End_Lat }, { title: MultilingualKey.End_Lng }, { title: MultilingualKey.End_Buffer },
                             { title: MultilingualKey.Required_Cores }, { title: MultilingualKey.Cable_Type }, { title: "rowID" }];


    this.tableColumnsForFtth = [{ title: "" }, { title: MultilingualKey.Operations }, { title: MultilingualKey.Customer_Name },
        { title: MultilingualKey.Customer_ID }, { title: MultilingualKey.Feasibility_ID },
        { title: "Lat" },
        { title: "Lng" }, { title: "Buffer Radius(m)" }, { title: "rowID" }];




    this.columnDefaults = [
        {
            "targets": 0,
            "data": null,
            "orderable": false,
            "defaultContent": bulk.DE.radioHTML,
            "className": "dt-center"
        },
        {
            "targets": 1,
            "data": null,
            "orderable": false,
            "defaultContent": bulk.DE.downloadSectionHTML,
            "className": "dt-center"
        },
        {
            "targets": 15,
            "visible": false
        }];


    this.columnDefaultsForFtth = [
        {
            "targets": 0,
            "data": null,
            "orderable": false,
            "defaultContent": bulk.DE.radioHTML,
            "className": "dt-center"
        },
        {
            "targets": 1,
            "data": null,
            "orderable": false,
            "defaultContent": bulk.DE.downloadSectionHTMLForftth,
            "className": "dt-center"
        },
        {
            "targets": 8,
            "visible": false
        }];

    this.init = function () {
        bulk.bindEvents();
    }

    this.bindEvents = function () {
        $(bulk.DE.FeasFileUpload).change(function () {
            var element = $(this);
            element.next(element).find('input').val((element.val()).split('\\').pop());
        });

        $(bulk.DE.btnChoose).on("click", function () {
            $('#btnResetFtth').trigger('click');
            $(bulk.DE.FeasFileUpload).trigger('click');
        });

        $(bulk.DE.fileInputControl).mousedown(function () {
            $(this).parents(bulk.DE.inputFile).prev().trigger('click');
            return false;
        });

        $(bulk.DE.btnUpload).on("click", function () {
            sf.feasibility_saved = [];
            sf.rowHistMap = [];
            var formData = new FormData();
            var files = document.getElementById(bulk.DE.FeasFileUpload_ID).files;
            $('#hdnFeasibilityInput').val("enterprise");
            if (files.length) {
                let fileParts = $(bulk.DE.FeasFileUpload).val().split('.');
                if (fileParts.length > 1) {
                    let lastIdx = fileParts.length - 1;
                    if (fileParts[lastIdx] === 'xls' || fileParts[lastIdx] === 'xlsx') {
                        formData.append(bulk.DE.FeasFileUpload_ID, files[0]);
                        sf.bulkFeasibilityData = formData;
                        bulk.postFormData(formData);
                    }
                    else {
                        alert(MultilingualKey.Only_xls_xlsx_allowed);
                        return;
                    }
                }
                else {
                    alert(MultilingualKey.Only_xls_xlsx_allowed);
                    return;
                }
            }
            else {
                alert(MultilingualKey.valid_file_Upload);
                return;
            }
        });

        $(bulk.DE.btnUploadForFtth).on("click", function () {
            sf.feasibility_saved = [];
            sf.rowHistMap = [];
            var formData = new FormData();
            $('#btnResetFtth').trigger('click');
            var files = document.getElementById(bulk.DE.FeasFileUpload_ID).files;
            $('#hdnFeasibilityInput').val("ftth");
            if (files.length) {
                let fileParts = $(bulk.DE.FeasFileUpload).val().split('.');
                if (fileParts.length > 1) {
                    let lastIdx = fileParts.length - 1;
                    if (fileParts[lastIdx] === 'xls' || fileParts[lastIdx] === 'xlsx') {
                        formData.append(bulk.DE.FeasFileUpload_ID, files[0]);
                        sf.bulkFeasibilityData = formData;
                        bulk.postFormDataForFtth(formData);
                    }
                    else {
                        alert(MultilingualKey.Only_xls_xlsx_allowed);
                        return;
                    }
                }
                else {
                    alert(MultilingualKey.Only_xls_xlsx_allowed);
                    return;
                }
            }
            else {
                alert(MultilingualKey.valid_file_Upload);
                return;
            }
        });

        $(bulk.DE.downloadTemplate).on("click", function () {
            bulk.downloadTemplate();
        });
        $(bulk.DE.downloadTemplateForftth).on("click", function () {
            bulk.downloadTemplateForftth();
        });
       
    }

    this.getFinalData = function (result) {
        var endData = JSON.parse(result);
        var finalData = [];
        var id = 0;
        for (var ed of endData) {
            id += 1;
            var a = [];
            a.push('');
            a.push('');
            for(key of Object.keys(ed)) {
                a.push(ed[key]);
            }
            a.push(id);

            finalData.push(a);
        }

        return finalData;
    }

    this.bindRadioEvent = function () {
        
        $('#tblInputs tbody').on('click', 'input[type="radio"]', function (evt) {
            if ($('#hdnFeasibilityInput').val() == "ftth") {

                $('#btnResetFtth').trigger('click');
            }
            $(bulk.DE.downloadBulkReports).css("visibility", "hidden");
            var data = bulk.tableInputs.row($(this).parents('tr')).data();
            let data_copy = data.map((x) => x);
            //data_copy.pop();
            data_copy.shift();
            data_copy.shift();
            sf.resetFields();
            var validation_result = $('#hdnFeasibilityInput').val() == "ftth" ? sf.validateFileDataForFtth(data_copy)
                : sf.validateFileData(data_copy);
            if (validation_result.status) {
                sf.lastRowID = $(this).parents('tr').attr('id');
                //if (!sf.validateInputFields()) {
                //    alert('Invalid Data!');
                //    return;
                //}
                //else {
                //var id = $(this).parents('tr').attr('id');
                $('#hdnFeasibilityInput').val() == "ftth" ? sf.computeFtth() : sf.compute();
                //console.log($(this).parents('tr').attr('id'));
                $(this).parents('tr').find(bulk.DE.downloadBulkReports).css("visibility", "visible");
                //bulk.DisplayDownloadFeasibleDiv();
                //}
                sf.fitBoundsToBuffer();
            }
            else {
                alert(validation_result.message);
            }
            
        });

        $('#tblInputs tbody').on('click', bulk.DE.downloadBulkReports, function (evt) {
            
            if ($('#hdnFeasibilityInput').val() == "ftth") {
               
                var Ischeck = $('#tblbatchLOS tbody tr td').find('[type="radio"]').is(':checked');
                if (!Ischeck) {
                    
                    //console.log(Ischeck);
                    alert("Please choose the below FTTH Network detail");
                    return false;
                }
            if ($(this).data('type') == 'Save' && !$($(this).context).hasClass('disabled')) {
                // sf.getFeasibilityDetails();
                if ($('#hdnFeasibilityInput').val() == "ftth") {
                    sf.SavefeasibilityForFtth();
                }
                else {
                    sf.Savefeasibility();
                }
               
                sf.lastRowID = $(this).parent().parent().attr('id');
                bulk.EnableKMZbutton();
            }
                if ($('#hdnFeasibilityInput').val() == "ftth") {

                    sf.dowloadFeasibilityReportFTTH($(this).data('type'));
                }
            } else {
                sf.dowloadFeasibilityReport($(this).data('type'));
            }
            
        });

    }

    this.getMappedHistoryID = function (rowID) {
        if (sf.rowHistMap) {
            for(item of sf.rowHistMap) {
                if (item.rowID == rowID) {
                    return item.histID;
                }
            }
        }
        return 0;
    }

    this.setHistoryID = function (historyID) {
        if (sf.lastRowID) {
            var feasibilityrow = $("#tblInputs tbody tr");
            $.each(feasibilityrow, function () {

                // get existing history id from mapping
                let mappedHistID = bulk.getMappedHistoryID($(this).attr('id'));
                if (mappedHistID) {
                    $(this).data('historyID', mappedHistID);
                }

                if ($(this).attr('id') == sf.lastRowID) {
                    $(this).data('historyID', historyID);
                    bulk.DisplayDownloadFeasibleDiv();
                    return false;
                }
            });
        }
    }

    this.DisplayDownloadFeasibleDiv = function()
    {
        var feasibilityrow = $("#tblInputs tbody tr");
            // [0].cells[4].innerText;
        $.each(feasibilityrow, function () {
            for(var i=0;i< sf.feasibility_saved.length;i++)
            {
                if (JSON.parse(sf.feasibility_saved[i]).history_id == $(this).data('historyID'))
                {
                    // $(this).find(bulk.DE.downloadBulkReports).css("visibility", "visible");
                    //$(this).find(bulk.DE.bulkSave).css("visibility", "hidden");
                    $(this).css('background', '#ddf7e6');
                    $(this).find(bulk.DE.bulkSave).addClass('disabled');
                    $(this).find(bulk.DE.bulkSave).attr('title', 'Already Saved');
                }
            }
        });
        
    }
    this.AddArraySavedFeasibility = function (savedData) {
        //var IsExist = false;
        //for (var i = 0; i < sf.feasibility_saved.length; i++) {
        //    if (JSON.parse(sf.feasibility_saved[i]).feasibility_name == JSON.parse(savedData).feasibility_name) {
        //        IsExist = true;
        //        break;
        //    }
        //}
        //if(IsExist == false)
        //{
        sf.feasibility_saved.push(savedData);
        var savedObj = JSON.parse(savedData);
        if (savedObj)
        {
            let rowID = sf.lastRowID;
            let histID = savedObj.history_id;
            let exists = false;
            if (sf.rowHistMap) {
                for(item of sf.rowHistMap) {
                    if (item.rowID == rowID) {
                        item.histID = histID;
                        exists = true;
                    }
                }
                if (!exists) {
                    sf.rowHistMap.push({ rowID: rowID, histID: histID });
                }
            }
        }
        //}
    }

    this.downloadTemplateForftth = function () {
        window.location = appRoot + 'Content/templates/FeasibilityInputFtth.xlsx';
    }

    this.downloadTemplate = function () {
        window.location = appRoot + 'Content/templates/FeasibilityInput.xlsx';
    }

    this.loadTable = function (finalData) {
        if (bulk.tableInputs) {
            bulk.tableInputs.destroy();
        }

        bulk.tableInputs = $(bulk.DE.tblInputs).DataTable({
            data: finalData,
            order: [],
            columns: bulk.tableColumns,
            columnDefs: bulk.columnDefaults,
            searching: false,
            bLengthChange: false,
            scrollX: true,
            pageLength: 10,
            rowId: function (a) {
                return 'bulk_' + a[15];
            }
        });

        bulk.bindRadioEvent();
    }
    

    this.loadTableForFtth = function (finalData) {
        if (bulk.tableInputs) {
            bulk.tableInputs.destroy();
        }

        bulk.tableInputs = $(bulk.DE.tblInputs).DataTable({
            data: finalData,
            order: [],
            columns: bulk.tableColumnsForFtth,
            columnDefs: bulk.columnDefaultsForFtth,
            searching: false,
            bLengthChange: false,
            scrollX: true,
            pageLength: 10,
            rowId: function (a) {
                return 'bulk_' + a[8];
            }
        });

        bulk.bindRadioEvent();
    }

    this.setBulkAPIRequest = function () {
        bulk.bulk_API_Request_Data = {};
        if (bulk.bulkFileInput.length) {
            let points = [];
            for (var i = 0; i < bulk.bulkFileInput.length; i++) {
                let point = {};
                if (bulk.bulkFileInput[i]) {
                    let rec = bulk.bulkFileInput[i];
                    point.FID = 'FID' + (i + 1).toString();
                    point.StartPoint = rec.Start_Lng + ' ' + rec.Start_Lat;
                    point.EndPoint = rec.End_Lng + ' ' + rec.End_Lat;
                    point.AEndRadInMtrs = rec.Start_Buffer;
                    point.BEndRadInMtrs = rec.End_Buffer;
                    point.DefaultRadius = 0;
                    points.push(point);
                }
            }
            if (points.length == bulk.bulkFileInput.length) {
                bulk.bulk_API_Request_Data.Points = points;
                bulk.bulk_API_Request_Data.IsDirected = true;
                bulk.bulk_API_Request_Data.IsDefaultRad = false;
            }
            else {
                alert('Invalid Data!');
            }
        }
        else {
            alert('No data found!');
        }
        //sf.bulkAPIRequest(bulk.bulk_API_Request_Data);
    }

    this.postFormData = function (formData, historyID) {
        $.ajax({
            type: "POST",
            url: baseUrl + appRoot + '/BulkFeasibility/Upload',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.status == 'success') {
                    var finalData = bulk.getFinalData(response.result);
                    // sf.bulkFeasibilityData = finalData;

                    bulk.bulkFileInput = JSON.parse(response.result);
                    //bulk.setBulkAPIRequest();
                    bulk.loadTable(finalData);
                    if (historyID) {
                        bulk.setHistoryID(historyID);
                    }
                }
                else {
                    alert('No Data');
                }

            },
            error: function (error) {
                alert("errror");
            }
        });
    }

    this.postFormDataForFtth = function (formData, historyID) {
        $.ajax({
            type: "POST",
            url: baseUrl + appRoot + '/BulkFeasibility/Upload',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.status == 'success') {
                    var finalData = bulk.getFinalData(response.result);
                    // sf.bulkFeasibilityData = finalData;

                    bulk.bulkFileInput = JSON.parse(response.result);
                    //bulk.setBulkAPIRequest();
                    bulk.loadTableForFtth(finalData);
                    if (historyID) {
                        bulk.setHistoryID(historyID);
                    }
                }
                else {
                    alert('No Data');
                }

            },
            error: function (error) {
                alert("errror");
            }
        });
    }
    this.EnableKMZbutton = function()
    {
        if (sf.feasibility_saved.length > 0)
        {
            //$("#downloadKMZFile").removeAttr("disabled");
            $("#downloadMergedFiles").removeAttr("disabled");
            
        }
        else
        {
            //$("#downloadKMZFile").attr("disabled", true);
            $("#downloadMergedFiles").attr("disabled", true);
        }
    }
    this.DownloadMergeKMLFile = function () {
        let ids = [];
        for(rec of sf.rowHistMap){
            ids.push(rec.histID);
        }
        //if (sf.feasibility_saved.length > 0)
        //window.location = appRoot + 'Report/DownloadKMZReport?feasibility_saved=' + JSON.stringify(sf.feasibility_saved); // JSON.stringify({ feasibility_saved: sf.feasibility_saved });
        if (ids.length)
            window.location = appRoot + 'Report/DownloadKMZReportByIds?historyIDs=' + ids.join();
        else
            alert('No Data!');
    }

    this.DownloadMergeKMLFileForFtth = function () {
        let ids = [];
        for (rec of sf.rowHistMap) {
            ids.push(rec.histID);
        }
        //if (sf.feasibility_saved.length > 0)
        //window.location = appRoot + 'Report/DownloadKMZReport?feasibility_saved=' + JSON.stringify(sf.feasibility_saved); // JSON.stringify({ feasibility_saved: sf.feasibility_saved });
        if (ids.length)
            window.location = appRoot + 'Report/DownloadKMZReportByIdsFtth?historyIDs=' + ids.join();
        else
            alert('No Data!');
    }

    this.DownloadMergeExcel = function () {
        let ids = [];
        for(rec of sf.rowHistMap) {
            ids.push(rec.histID);
        }
        if (ids.length)
            window.location = appRoot + 'Report/DownloadMergedExcel?historyIDs=' + ids.join();
        else
            alert('No Data!');
    }

    this.DownloadMergeExcelFTTH = function () {
        let ids = [];
        for (rec of sf.rowHistMap) {
            ids.push(rec.histID);
        }
        if (ids.length)
            window.location = appRoot + 'Report/DownloadMergedExcelFTTH?historyIDs=' + ids.join();
        else
            alert('No Data!');
    }
}