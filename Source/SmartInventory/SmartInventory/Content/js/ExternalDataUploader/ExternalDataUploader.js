var ExternalDataUploader = function () {
    var app = this;
    this.geoXml = new geoXML3.parser({ map: si.map, singleInfoWindow: true, zoom: true });
    //this.currentKMLFileName = '';
    //this.currentKMLFilePath = '';
    this.currentKMLFileId = '';
    //this.currentType= '';
    this.KL = {
        "txtfilename": "#txtfilename",
        "fupload": "#fupload",
        "KmlLayerSearch": "#KmlLayerSearch",
        "rdbpublic": "#rdbpublic",
        "closeExtnlData": "#closeExtnlData",
        "dvExternalData": "#dvExternalData",
        "OpenCloseExternalView": '#closeExternalData,#openExternalData',
        "sidectrlright": ".sidectrlright",
    }
    this.initApp = function () {
        this.bindEvents();
    }
    this.bindEvents = function () {
        $(app.KL.OpenCloseExternalView).unbind('click');
        $(app.KL.OpenCloseExternalView).on("click", function () {
            $(app.KL.dvExternalData).animate({
                width: "toggle"
            }, function () {
                if ($(this).css('display') === 'none') {
                    $("#closeExternalData").hide();
                    $("#openExternalData").show();
                }
                else {
                    $("#closeExternalData").show();
                    $("#openExternalData").hide();
                }
            });
        });
        $(app.KL.closeExtnlData).on("click", function () {
            $("#dvExternalData").slideToggle('slow', function () {
                $(app.KL.txtfilename).val('');
                $(app.KL.KmlLayerSearch).val('');
                $(app.KL.dvExternalData).hide();
                $('.externalDataView').removeClass('activeToolBar');
                app.HideAllExternalFilesFromMap();
            });
        });
    }
    this.SortFileData = function (_isReverse) {
         
        var _sorttype = $("#hdnSortType").val();
        if (_isReverse) {
            _sorttype = $("#hdnSortType").val() == 'asc' ? 'desc' : 'asc';
        }
        var _sortcolumn = $("input[name='rdbExtnlDataValue']:checked").val();
        var _searchtext = $("#txtSearchTextExtnlData").val();
        $(".ExternalData_Loader").show();
        ajaxReq('FileUpload/GetExternalFileData', { currentPage: 1, searchText: _searchtext, sort: _sorttype, orderBy: _sortcolumn }, true, function (resp) {
            $("#rowdata").html(resp);
            $("#hdnCurrentPage").val(1);
            $("#hdnSortType").val(_sorttype);
            $("input[value='" + _sortcolumn + "']").prop("checked", true);
            $(".ExternalData_Loader").hide();
            app.HideAllExternalFilesFromMap();
        }, false, false);
    }
    $('#rowdata').scroll(function () {
        if ($(this).scrollTop() + $(this).innerHeight() >= $(this)[0].scrollHeight) {
            var _currentPage = parseInt($("#hdnCurrentPage").val());
            var _sorttype = $("#hdnSortType").val();
            var _sortcolumn = $("input[name='rdbExtnlDataValue']:checked").val();
            var _searchtext = $("#txtSearchTextExtnlData").val();
            _currentPage = _currentPage + 1;
            $(".ExternalData_Loader").show();
            ajaxReq('FileUpload/GetExternalFileData', { currentPage: _currentPage, searchText: _searchtext, sort: _sorttype, orderBy: _sortcolumn }, true, function (resp) {
                if (resp.trim() != "") {
                    $("#rowdata").append(resp);
                    $("#hdnCurrentPage").val(_currentPage);
                    $("#dvNoMoreRecord").remove();
                }
                else {
                    if ($("#dvNoMoreRecord").length == 0) {
                        $("#rowdata").append('<div id="dvNoMoreRecord" class="prowindowExtnlData">No more data found.</div>');
                    }
                }
                $(".ExternalData_Loader").hide();
            }, false, false);
        }
    });

    this.Validatefiletype = function (filename, filecontrol) {
        var valid_extentions = filecontrol.get(0).accept.split(',');
        var position = filename.lastIndexOf('.');
        var extention = '';
        if (position != -1) {
            extention = filename.substr(position);
            return valid_extentions.includes(extention);
        }

    }
    this.uploadExternalData = function () {
         
        var actualFileSize = 0;
        var _maxfilesize = $("#hdnExternalDataMaxFileSize").val();
        //This function upload the kml/kmz file to file system
        var fileUpload = $(app.KL.fupload).get(0);
        var files = fileUpload.files;

        if (fileUpload.files.length == 0) {
            // alert("Please select file to upload!");
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_110);
            return;
        }

        if (!app.validatefilename()) {
            $(app.KL.txtfilename).focus();
            return;
        }
        for (var i = 0; i < files.length; i++) {
            if (!app.Validatefiletype(files[i].name.toLowerCase(), $(app.KL.fupload))) {
                //Please select a valid file! Valid extensions are:
                //alert(MultilingualKey.SI_OSP_GBL_JQ_GBL_097+' '+ $(app.KL.fupload).get(0).accept);

                alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_097, $(app.KL.fupload).get(0).accept), 'warning');
                return;
            }
            actualFileSize = actualFileSize + files[i].size;
        }

        if (actualFileSize > _maxfilesize * 1000000) {

            alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_FRM_109, _maxfilesize)), 'warning');
            //alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_109+" " + _maxfilesize + " MB!");
            return;
        }
        
        var data = new FormData();
        data.append("txtfilename", $(app.KL.txtfilename).val());
        data.append("rdbAccess", $("input[name='rdbAccess']:checked").val());
        if ($('#ddlfiletype :selected').val() == "DXF") {
            if (!($('#inputpicker-1').val() == "")) {
                var SourceId = $('table .inputpicker-active').attr('data-value');
                data.append('SourceId', SourceId);
            } else {
                alert('Please Select Source Location');
                return;
            }
        }
        for (var i = 0; i < files.length; i++) {
            data.append(files[i].name, files[i]);
        }
        data.append("filetype", $("#ddlfiletype").val().toLowerCase());
        var uploadurl = "";
        switch ($("#ddlfiletype").val().toLowerCase()) {
            case "shape":
            case "tab":
            case  "dxf":
                uploadurl = "FileUpload/UploadOtherFile";
                break;
            case "kml_kmz":
                uploadurl = "FileUpload/UploadKMLZFile";
                break;
        }

        ajaxReqforFileUpload(uploadurl, data, true, function (result) {
             
            if (result != "") {
                 
                if (result.msg != null) {
                    if (result.msg.toLowerCase() == "success") {
                        alert(result.strReturn);
                        document.getElementById("fupload").value = "";
                        $(app.KL.txtfilename).val('');
                        $(app.KL.rdbpublic).prop("checked", true);
                        $('#closeChildPopup').trigger("click");
                        app.HideAllExternalFilesFromMap();
                        $("#frmGetExtnlData").submit();
                    }

                    else {
                        alert(result.strReturn);
                    }
                }
                else {
                    alert(result.strReturn);
                }
            
            }
        }, true);
    }
    this.HighlighFile = function (fileId) {
        if ($(".ActiveMultiSelect").length > 0) {
            $('#dvHighlight_' + fileId).addClass('activeBlue');
            $('#HighlightCircle_' + fileId).addClass('activeBlue');
            $('#dvHighlight_' + fileId).addClass('activeBlue').addClass('glyphicon-eye-close');
            $('#dvHighlight_' + fileId).attr('title', MultilingualKey.SI_OSP_GBL_GBL_FRM_005);
        }
        else{
             app.UnHighlighAll();
            $('#dvHighlight_' + fileId).addClass('activeBlue');
            $('#HighlightCircle_' + fileId).addClass('activeBlue');
            $('#dvHighlight_' + fileId).addClass('activeBlue').addClass('glyphicon-eye-close');
            $('#dvHighlight_' + fileId).attr('title', MultilingualKey.SI_OSP_GBL_GBL_FRM_005);
        }
    }
    this.UnHighlighAll = function (fileId) {
        $("#rowdata i[id^='HighlightCircle_']").removeClass('activeBlue');
        $("#rowdata i[id^='dvHighlight_']").removeClass('activeBlue');
        $("#rowdata i[id^='dvHighlight_']").removeClass('glyphicon-eye-close').addClass('glyphicon-eye-open');
        $("#rowdata i[id^='dvHighlight_']").each(function () {
            $(this).attr('title', MultilingualKey.SI_GBL_GBL_JQ_FRM_013);
        });
    }
    this.ShowExternalData = function (filename, _filePath, fileId, type) {
         
        if ($(".ActiveMultiSelect").length > 0) {
            if (!$('#dvHighlight_' + fileId).hasClass('activeBlue')) {
                app.currentKMLFileId = fileId;

                var _maxfileview = $("#hdnmaxExternalDataViewOption").val();
                if (geoXML3.instances.length >= parseInt(_maxfileview)) {
                    alert('Max ' + _maxfileview + ' files can be viewed at a time!');
                }
                else {
                    //This function show a selected file on google map
                    showProgress();
                    app.HighlighFile(fileId);
                    var filePath = _filePath.replace('~', '').replace(/\\/g, '/');
                    //var index = -1;
                    var path = appRoot + filePath.substring(1, filePath.length) + '/' + filename + "." + type;
                    //for (var i = 0; i < app.geoXml.docs.length; i++) {
                    //    app.geoXml.hideDocument(app.geoXml.docs[i]);
                    //}
                     
                    app.geoXml = new geoXML3.parser({ map: si.map, singleInfoWindow: true, zoom: true });
                    app.geoXml.parse(path);
                }

            }
            else {
                //if (app.currentKMLFileId != undefined && fileId == app.currentKMLFileId) {
                app.HideViewedFileFromMap(fileId);
            }
        }
        else {
            if (!$('#dvHighlight_' + fileId).hasClass('activeBlue')) {
                app.currentKMLFileId = fileId;
                //This function show a selected file on google map
                showProgress();
                app.HighlighFile(fileId);
                var filePath = _filePath.replace('~', '').replace(/\\/g, '/');
                //var index = -1;
                var path = appRoot + filePath.substring(1, filePath.length) + '/' + filename + "." + type;
                for (var i = 0; i < app.geoXml.docs.length; i++) {
                    app.geoXml.hideDocument(app.geoXml.docs[i]);
                }
                 
                app.geoXml = new geoXML3.parser({ map: si.map, singleInfoWindow: true, zoom: true });
                app.geoXml.parse(path);
            }
            else {
                if (app.currentKMLFileId != undefined && fileId == app.currentKMLFileId) {
                    app.HideAllExternalFilesFromMap();
                }
            }
        }
    }
    this.HideAllExternalFilesFromMap = function () {
         
        if ($(".ActiveMultiSelect").length > 0) {
            app.currentKMLFileId = 0;
            app.UnHighlighAll();
            //This function hide a all file from google map
            for (var i = 0; i < geoXML3.instances.length; i++) {
                geoXML3.instances[i].hideDocument();
                //geoXML3.instances.splice(i, 1);
            }
            geoXML3.instances = [];
        }
        else {
            app.currentKMLFileId = 0;
            app.UnHighlighAll();
            //This function hide a all file from google map
            for (var i = 0; i < app.geoXml.docs.length; i++) {
                app.geoXml.hideDocument(app.geoXml.docs[i]);
            }
            geoXML3.instances = [];
        }
    }
    this.HideViewedFileFromMap = function (fileId) {
         
        app.currentKMLFileId = 0;
        var _filename = $('#dvHighlight_' + fileId).attr("data-filename");
        var _filepath = $('#dvHighlight_' + fileId).attr("data-filepath");
        var _filetype = $('#dvHighlight_' + fileId).attr("data-type");
        var filePath = _filepath.replace('~', '').replace(/\\\\/g, '/');
        var _fileBaseurl = appRoot + filePath.substring(1, filePath.length) + '/' + _filename + "." + _filetype;
        for (var i = 0; i < geoXML3.instances.length; i++) {
            if (geoXML3.instances[i].docs[0].baseUrl == _fileBaseurl) {
                geoXML3.instances[i].hideDocument();
                $('#dvHighlight_' + fileId).removeClass('activeBlue');
                $('#HighlightCircle_' + fileId).removeClass('activeBlue');
                $('#dvHighlight_' + fileId).removeClass('glyphicon-eye-close').addClass('glyphicon-eye-open');
                $('#dvHighlight_' + fileId).attr('title', MultilingualKey.SI_GBL_GBL_JQ_FRM_013);
                geoXML3.instances.splice(i, 1);
                break;
            }
        }
    }
    this.deleteExternalDataFile = function (filename, fileid, _filePath, type) {
        //This function delete the selected file from file system as well as from google map
        showConfirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_004, function () {
            delKMLFile(filename, fileid, _filePath, type)
        });
    }
    this.downloadExternalFile = function (fileid) {
        downloadextfile(fileid)
    }
    function delKMLFile(filename, fileid, filePath, type) {

        $.ajax({
            data: { filename: filename, fileid: fileid, type: type },
            url: "FileUpload/DeleteExtrnlDataFile",
            success: function (result) {
                if (result.msg.toLowerCase() == "ok") {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_112);
                    if (app.currentKMLFileId != undefined && fileid == app.currentKMLFileId) {
                        app.HideAllExternalFilesFromMap();
                    }
                    $("#dv_kml_" + fileid).remove();
                }
            }
        });
        return true;
    }
    function downloadextfile(fileid) {
        window.location = appRoot + 'FileUpload/DownloadExternlDataFile?fileid=' + fileid;
    }
    this.validatefilename = function () {
        //This function validate the file name before upload
        var filename = $.trim($(app.KL.txtfilename).val());

        if (filename == "") {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_113);
            return false;
        }
        if (filename.split('..').length > 1) {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_114);
            return false;
        }

        if (filename.match(/^[0-9a-zA-Z_\.]*$/)) {
            return true;
        }
        else {
            return true;
        }
        //else {
        //    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_115);
        //    return false;
        //}
    }
}