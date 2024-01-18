//////////////////////////////////////////View Model/////////////////////////////////////////////
var modelKeys = {
    portKey: 5,
    chessisKey: '2',
    equipmentKey: '1',
    trayKey: '6',
    slotKey: '3',
    cardKey: '4',
    labelKey: '7'
}
var isp_model_info = [];
var modelid;
var modelname;
//Getting Model Details Function
function GetModelDetails(mID, mName) {
    modelid = mID;
    modelname = mName;
    $('#iconEdit').show();
    $('#txtSearchText').val('');
    //$('#ddlSearchBy').val('');
    ajaxReq('/Equipment/GetModelStatusCount', { modelID: mID }, false, function (resp) {
        $('#dvStatusCount').html('');
        $('#dvStatusCount').append(resp);
        ajaxReq('/Equipment/GetModelDetails', { modelID: mID }, false, function (resp) {
            $('#dvViewEquipmentDetails').html('');
            $('#dvModelDetailsByModelId').html('');
            $('#dvModelDetailsByModelId').append(resp);
            $('#Heading').html('');
            $('#Heading').html(mName);
        }, false, true,false);

    }, false, true,false);

}

//Search Model Details Function
function SearchModelDetails() {

    if ($('#txtSearchText').val() == '' || $('#txtSearchText').val() == null) {
        $('#txtSearchText').focus();
    }
    else {
        ajaxReq('/Equipment/GetModelDetails', { searchText: $('#txtSearchText').val().trim(), searchBy: $('#ddlSearchBy option:selected').text(), modelID: modelid }, false, function (resp) {
            $('#dvViewEquipmentDetails').html('');
            $('#dvModelDetailsByModelId').html('');
            $('#dvModelDetailsByModelId').append(resp);
        }, false, true,false);
    }

}

function resetSearch() {
    let $searchText = $('#txtSearchText');
    $searchText.val('');
    //$('#ddlSearchBy').val('');
    ajaxReq('/Equipment/GetModelDetails', { searchText: '', searchBy: '', modelID: modelid }, false, function (resp) {
        $('#dvViewEquipmentDetails').html('');
        $('#dvModelDetailsByModelId').html('');
        $('#dvModelDetailsByModelId').append(resp);
    }, false, true,false);


}

//Delete Model Details Function
function DeleteModelDetails() {

    var selectedRow = [];
    $('#tblViewEquipmentDetails').find('input[type="checkbox"]:checked').each(function () {
        if ($(this).is(':checked')) {
            selectedRow.push($(this).attr('name'));
        }
    });

    if (selectedRow.length === 0) {
        alert("Select Model To Delete!");
    }
    else {
        confirm("<b>Warning</b> <br>Are you sure you want to delete the selected models?You won't be able to restore them!", function () {

            ajaxReq('/Equipment/DeleteModelDetailsById', { modelIds: selectedRow.join(",") }, false, function (resp) {
                if (resp.status == 'OK')
                    alert(resp.message, 'Success', 'success');
                else
                    alert(resp.message, 'Error', 'error');
                // $('.RadioDiv').find('input:radio[name="optradio"]:checked').trigger('click');
                GetModelDetails(modelid, modelname);
            }, false, true,true);

        });

    }
}



//Edit Model Details Function
function EditModelDetails() {
    $('#anchorModelDetails').addClass('active').addClass('show');
    $('#anchorSpecification').removeClass('active').removeClass('show');
    $('#ModelDetails').addClass('active').addClass('show');
    $('#SpecificationContext').removeClass('active').removeClass('show');
    var selectedRow = [];
    $('#tblViewEquipmentDetails').find('input[type="checkbox"]:checked').each(function () {

        if ($(this).is(':checked')) {
            selectedRow.push($(this).attr('name'));
        }
    });

    if (selectedRow.length === 0) {
        $('#iconEdit').removeAttr('data-toggle').removeAttr('data-target');
        alert('Select Model to edit!');
    }
    else if (selectedRow.length > 1) {
        $('#iconEdit').removeAttr('data-toggle').removeAttr('data-target');
        alert('You can only edit one model at a time!');
    }
    else {

        $('#iconEdit').attr('data-toggle', 'modal').attr('data-target', '#editModal');

        $('#tblViewEquipmentDetails').find('input[type="checkbox"]:checked').each(function () {

            if ($(this).is(':checked')) {
                $('#btnOpenWorkspace').attr('href', appRoot + '/Equipment/CreateModel?modelid=' + $(this).attr('name').trim());
            }
            if (modelid == modelKeys.equipmentKey) {
                $('.otherMenu').show();
            }
            else {
                $('.otherMenu').hide();
            }
             
            ajaxReq('/Equipment/EditModelInfo', { id: $(this).attr('name').trim() }, false, function (resp) {
                isp_model_info = resp;
                $('#txtModelMasterName').val(resp.model);
                $('#txtModelName').val(resp.model_name);
                $('#txtModelHeight').val(resp.height);
                $('#txtModelWidth').val(resp.width);
                $('#txtModelDepth').val(resp.depth);
                $('#txtModelTypeMasterName').val(resp.modeltypevalue);
                $("#txtCategory").val(resp.model_template.category);
                $("#txtItemCode").val(resp.model_template.item_code);
                $("#txtSubCategory1").val(resp.model_template.subcategory1);
                $("#txtSubCategory2").val(resp.model_template.subcategory2);
                $("#txtSubCategory3").val(resp.model_template.subcategory3);
                $("#no_of_port").val(resp.model_template.no_of_port);
                $("#itemTemplateId").val(resp.item_template_id);
                $('#txtEntityType').val(resp.model_template.entityType);
                fillSelector($("select#selectStatus"), resp.lstModelStatus, { value: 'id', text: 'name' }, resp.status_id);
                fillSelector($("select#specification"), resp.model_template.lstSpecification, { value: 'value', text: 'key' }, resp.model_template.specification);
                fillSelector($("select#ddlVendor"), resp.model_template.lstVendor, { value: 'key', text: 'value' }, resp.model_template.vendor_id);
                renderErrorFocus($('#txtModelName'), false);
                $('#txtModelUnit').val(resp.unit_size);
                if (resp.model_id == modelKeys.portKey)
                {
                    $('#chkIsMultiConnection').prop('checked', resp.is_multi_connection); 
                }
                showUnitContext(resp.model_id == modelKeys.equipmentKey);
                showMultiConnectionContext(resp.model_id == modelKeys.portKey);
                //$("select#specification").attr("disabled", true);
                //$("select#ddlVendor").attr("disabled", true);
            }, false, true, true);

        });

    }
}

//Save Model Details Function
function UpdateModelDetails() {
    //validate here 
    let isError = false;
    let isPortCountFailed = false;
    let $name = $('#txtModelName');
    let $status = $('#selectStatus');
    let $specification = $('select#specification');
    let $vendor = $('select#ddlVendor');
    let $no_of_port = $('#no_of_port');
   
    //validate name and status
    renderErrorFocus($name, false);
    renderErrorFocus($status, false);
    renderErrorFocus($specification, false);
    renderErrorFocus($vendor, false);
    if (!$name.val().trim()) {
        renderErrorFocus($name, true);
        isError = true;
    }
    if (!$status.val().trim()) {
        renderErrorFocus($status, true);
        isError = true;
    }
    if (modelid == modelKeys.equipmentKey && $status.val().trim() == 1) {
       
        activeSpecificationTab(!isError);
        if (!$specification.val()) {
            renderErrorFocus($specification, true);
            isError = true;
        }
        if (!$vendor.val()) {
            renderErrorFocus($vendor, true);
            isError = true;
        }
        if (!isError ) {
            renderErrorFocus($no_of_port, true);
            isPortCountFailed = true;

        }
    }
    if (isError) {
        return false;
    }
    var ISPModelInfo = {};
    ISPModelInfo.id = isp_model_info.id;
    ISPModelInfo.model_id = isp_model_info.model_id;
    ISPModelInfo.model_name = $('#txtModelName').val();
    ISPModelInfo.height = isp_model_info.height;
    ISPModelInfo.width = isp_model_info.width;
    ISPModelInfo.depth = isp_model_info.depth;
    ISPModelInfo.model_type_id = isp_model_info.model_type_id;
    ISPModelInfo.status_id = $("#selectStatus option:selected").val();
    ISPModelInfo.item_template_id = $("#itemTemplateId").val();
    ISPModelInfo.model_image_id = isp_model_info.model_image_id;
    ISPModelInfo.unit_size = isp_model_info.unit_size;
    ISPModelInfo.model_master_name = isp_model_info.model_master_name;
    ISPModelInfo.is_multi_connection = $("#chkIsMultiConnection").is(':checked');
    ISPModelInfo.border_color = isp_model_info.border_color;
    //$.ajax({
    //    url: "/Equipment/SaveModel",
    //    type: 'GET',
    //    data: ISPModelInfo,
    //    success: function (res) {
    //        if (res.page_message.status == "OK") {
    //            $('#btnCancelPopup').trigger('click');
    //            GetModelDetails(modelid, modelname);
    //            alert(res.page_message.message , 'Success', 'success');
    //        }
    //        else {
    //            alert(res.page_message.message);
    //        }

    //    }
    //});
   
    //ajaxReq('/Equipment/SaveModel',  ISPModelInfo , false, function (res) {

    //    if (res.page_message.status == "OK") {
    //                    $('#btnCancelPopup').trigger('click');
    //                    GetModelDetails(modelid, modelname);
    //                    alert(res.page_message.message , 'Success', 'success');
    //                }
    //                else {
    //                    alert(res.page_message.message);
    //                }

    //}, false, true,true);

    if (isPortCountFailed) {
        confirm("Number of port of equipment is not equal to item specification! Do you want to continue?", function () {
            callUpdateModelAPI(ISPModelInfo);
        });
    }
    else { callUpdateModelAPI(ISPModelInfo); }
}

function callUpdateModelAPI(ISPModelInfo) {
    ajaxReq('/Equipment/SaveModel', ISPModelInfo, false, function (res) {

        if (res.page_message.status == "OK") {
            $('#btnCancelPopup').trigger('click');
            GetModelDetails(modelid, modelname);
            alert(res.page_message.message, 'Success', 'success');
        }
        else {
            alert(res.page_message.message);
        }

    }, false, true, true);
}

function fillSelector($selector, filldata, attr, val) {
    $selector.empty();
    $.each(filldata, function (data, value) { $selector.append($("<option></option>").val(value[attr.value]).html(value[attr.text])); });
    $selector.val(val);
}
this.ddlChangeRemoveCss = function (selectorId, emType) {

    //var emID = $("#" + selectorId + "_chosen");
    //if (emID.children('a').text().trim() != "Select " + emType) {
    //    emID.removeClass('input-validation-error').next('span').removeClass('field-validation-error').addClass('field-validation-valid').html('');
    //}
    //else {
    //    emID.addClass('input-validation-error');
    //}
}

/*DO NOT DELETE: Render selector input value sample
                {
                    data: res.result,
                    element: $(DE.selectVender),
                    value: 'value',
                    text: 'key',
                    defaultText: 'Select Vendor',
                    dataKey: [{ key: 'item_template_id', name: 'data-item-id' }],
                    disabled: false
                }
*/
this.renderSelector = function (d) {

    let option = "";
    if (d.defaultText)
        //option = "<option value disabled selected hidden>" + d.defaultText + "</option>";
        option = "<option value  selected >" + d.defaultText + "</option>";
    if (d && d.data) {
        let len = d.data.length;
        let keys = "";
        if (len > 0) {
            for (let i = 0; i < len; i++) {
                keys = "";
                if (d.dataKey && d.dataKey.length) {
                    for (let j = 0; j < d.dataKey.length; j++) {
                        keys += " " + d.dataKey[j].name + "='" + d.data[i][d.dataKey[j].key] + "'";
                    }
                }
                option += "<option value='" + d.data[i][d.value] + "' " + keys + " >" + d.data[i][d.text] + "</option>";
            }
        }
    }
    d.element.attr('disabled', d.disabled);
    d.element.html(option);
}
this.renderColorSelector = function (d) {

    let option = "";
    if (d.defaultText)
        //option = "<option value disabled selected hidden>" + d.defaultText + "</option>";
        option = "<option style='background:#FFFFFF;color:#000000' value  selected >" + d.defaultText + "</option>";
    if (d && d.data) {
        let len = d.data.length;
        let keys = "";
        if (len > 0) {
            for (let i = 0; i < len; i++) {
                keys = "";
                if (d.dataKey && d.dataKey.length) {
                    for (let j = 0; j < d.dataKey.length; j++) {
                        keys += " " + d.dataKey[j].name + "='" + d.data[i][d.dataKey[j].key] + "'";
                    }
                }
                option += "<option style='background:" + d.data[i][d.bg] + ";color:" + d.data[i][d.color] + ";border:" + d.data[i][d.color] + " 3px solid;'" + " value='" + d.data[i][d.value] + "' " + keys + " ></option>";
            }
        }
    }
    d.element.attr('disabled', d.disabled);
    d.element.html(option);
}

this.renderErrorFocus = function ($e, flag) {
    if (flag)
        $e.css('border', '1px solid red').css('border-radius', '0.25rem');
    else {
        $e.css('border', '').css('border-radius', '');
        $e.removeClass('input-validation-error');

    }
};

this.activeSpecificationTab = function (flag) {
    if (flag) {

        $('#anchorSpecification').addClass('active').addClass('show');
        $('#anchorModelDetails').removeClass('active').removeClass('show');
        $('#SpecificationContext').addClass('active').addClass('show');
        $('#ModelDetails').removeClass('active').removeClass('show');
    }
    else {

        $('#anchorModelDetails').addClass('active').addClass('show');
        $('#anchorModelDetails').removeClass('active').removeClass('show');
        $('#ModelDetails').addClass('active').addClass('show');
        $('#SpecificationContext').removeClass('active').removeClass('show');
    }


}
var modelIDs = [];
var modelView = (function () {
    var DE = {};

    let action = {
        onModelCheck: function () {
            let len = $(DE.modelChk + ':checked').length;
            let $btnEdit = $(DE.btnEdit);
            $btnEdit.show();
            if (len > 1)
            { $btnEdit.hide(); }
        },
         onModelTypeCheck: function () {
            let len = $(DE.modelChk + ':checked').length;
             let $btnEdit = $(DE.iconEditRule);
            $btnEdit.show();
            if (len > 1) { $btnEdit.hide(); }
        }
        
    };
    let event = {
        onTypeColorChange: function (e) {
            render.modelColorSelection($(this));
        }
    };
    let render = {
        modelColorSelection: function ($e) {
            $e.attr("style", $('option:selected', $e).attr('style'));
            let model = $('#ddlModel').val();

            if ($e.val() && modelKeys.equipmentKey == model)
            {
                $e.addClass('equipment');
            } else { $e.removeClass('equipment'); }
        }

    };
    let bind = {
        modelList: function () {
            let $modelListContext = $(DE.modelListContext);
            $modelListContext.on('change', DE.modelChk, action.onModelCheck);


            let $dvTypeDetails = $(DE.dvTypeDetails);
            $dvTypeDetails.on('change', DE.modelChk, action.onModelTypeCheck);

            let $modelTypeColorSel = $(DE.modelTypeColorSel);
            $modelTypeColorSel.on('change', event.onTypeColorChange);
        }
    };
    var setup = function () {
        for (let e in bind) {
            if (typeof bind[e] == 'function') {
                bind[e]();
            }
        }
        loadModelId();
    }
    var init = function (config) {
        $.extend(true, DE, config);
        setup();
    }
    return { init: init };
}());

//////////////////////////////////////////END///////////////////////////////////////////////////




////////////////////////////////////////Model Type and Model Rule//////////////////////////////

//Create Rules Model Function
function CreateRules() {
    $('#btnCreateRules').attr('data-toggle', 'modal').attr('data-target', '#rulesModel');
    resetRuleFields();
    BindRules();
    BindModalType();
}

function BindRules() {
    ajaxReq('/Equipment/GetRuleDetails', {}, false, function (resp) {
        $('#dvViewRuleDetails').html('');
        $('#dvRuleDetails').html('');
        $('#dvRuleDetails').append(resp);
    }, false, true,false);
}

function BindModalType() {
    ajaxReq('/Equipment/GetModalTypeDetails', {}, false, function (resp) {
        $('#dvViewModalTypeDetails').html('');
        $('#dvTypeDetails').html('');
        $('#dvTypeDetails').append(resp);
    }, false, true,false);
}




function loadModelId () {
    modelIDs = [];
    ajaxReq('/Equipment/checkModalHasType', {}, false, function (resp) {
        for (var i = 0; i < resp.length; i++) {
                        modelIDs.push(JSON.stringify(resp[i].id));
                    }
    }, false, true, true);
};


let ruleID = $('#hdRuleID').val();
function BindSubType(val, subTypeID) {

    var modelID = val;
    var ModelType = $("#" + subTypeID + "");


    if (modelIDs.includes(modelID)) {
        ModelType.attr('disabled', 'disabled');
        ModelType.val('').trigger('chosen:updated');
        BindRuleDetailsByParentSubType();
    }
    else {
        ModelType.removeAttr('disabled');
        ajaxReq('/Equipment/GetModelSubType', { modelId: modelID }, false, function (resp) {


            ModelType.empty();
            //ModelType.append($('<option/>', {
            //    value: 0,
            //    text: "Select Type"
            //}));
            $.each(resp, function (index, itemData) {
                ModelType.append($('<option/>', {
                    value: itemData.id,
                    text: itemData.value
                }));
            });
            ModelType.trigger("chosen:updated");
        }, false, true,true);
    }



}

function SaveRule() {


    if ($('#selectParentModel option:selected').val() == 0) {
        alert('Select Parent Model!'); return false;
    }
    else if ($('#selectParentModelType option:selected').val() == 0) {

        if (!modelIDs.includes($('#selectParentModel option:selected').val())) {
            alert('Select Parent Type!'); return false;
        }
        else {
            if ($('#selectChildModel option:selected').val() == 0) {
                alert('Select Child Model!'); return false;
            }
            else {
                fnSaveRule($('#selectParentModel option:selected').val(), $('#selectParentModelType option:selected').val(), $('#selectChildModel option:selected').val(), $('#selectChildModelType option:selected').val());
            }

        }

    }
    else if ($('#selectChildModel option:selected').val() == 0) {
        alert('Select Child Model!'); return false;

    }
    else if ($('#selectChildModelType option:selected').length == 0) {
        if (!modelIDs.includes($('#selectChildModel option:selected').val())) {
            alert('Select Child Type!'); return false;
        }
        else {
            fnSaveRule($('#selectParentModel option:selected').val(), $('#selectParentModelType option:selected').val(), $('#selectChildModel option:selected').val(), $('#selectChildModelType option:selected').val());
        }
    }
    else {
        fnSaveRule($('#selectParentModel option:selected').val(), $('#selectParentModelType option:selected').val(), $('#selectChildModel option:selected').val(), $('#selectChildModelType option:selected').val());
    }

}

function fnSaveRule(parent_model, parent_model_type, child_model, child_model_type) {
    let ispModelRules = [];
    let types = getSelectedTypes();
    let typeLen = types.length;
    if (typeLen == 0) {
        ispModelRules.push({
            parent_model_id: parseInt(parent_model),
            parent_model_type_id: parseInt(parent_model_type),
            child_model_id: parseInt(child_model),
            child_model_type_id: 0
        });
    }
    for (let i = 0 ; i < typeLen; i++) {
       
        ispModelRules.push({
            parent_model_id: parseInt(parent_model),
            parent_model_type_id: parseInt(parent_model_type),
            child_model_id: parseInt(child_model),
            child_model_type_id: parseInt(types[i])
        });
    }

    //var dataStr = JSON.stringify(ispModelRules);
    //$.ajax({
    //    url: '/admin/Equipment/SaveBulkRule',
    //    type: 'POST',
    //    dataType:'json',
    //contentType: "application/json; charset=utf-8",
    //    data: dataStr,
    //    success: function (res) {
    //        if (res.status == "OK") {
    //            alert(res.message);
    //            $('#selectParentModel').val(0);
    //            $('#selectParentModelType').val(0);
    //            $('#selectChildModel').val(0);
    //            $('#selectChildModelType').val(0);
    //            //$('#btnclose').trigger('click');
    //            BindRules();
    //        }
    //        else {
    //            alert(res.message);
    //        }
    //    }
    //});
    ajaxReq('/Equipment/SaveBulkRule', ispModelRules, true, function (res) {
        if (res.status == "OK") {
            alert(res.message, 'Success', 'success');
            resetRuleFields();

            BindRules();
        }
        else {
            alert(res.message);
        }
    }, true, true, true);
}

function BindRuleDetailsByParentSubType() {
    let ISPModelRule = {};
    ISPModelRule.parent_model_id = $('#selectParentModel option:selected').val();
    ISPModelRule.parent_model_type_id = $('#selectParentModelType option:selected').val();
    //$.ajax({
    //    url: '/Equipment/GetRuleDetails',
    //    type: 'GET',
    //    data: ISPModelRule,
    //    success: function (res) {
    //        $('#dvRuleDetails').html('');
    //        $('#dvRuleDetails').html(res);
    //    }
    //});
    ajaxReq('/Equipment/GetRuleDetails', ISPModelRule, false, function (resp) {
       
        $('#dvRuleDetails').html('');
        $('#dvRuleDetails').append(resp);
    }, false, true, false);
}

function DeleteRule() {

    var selectedRow = [];
    $('#tblViewRuleDetails').find('input[type="checkbox"]:checked').each(function () {
        if ($(this).is(':checked')) {
            selectedRow.push($(this).attr('name'));
        }
    });

    if (selectedRow.length === 0) {
        alert("Select Record To Delete!");
    }
    else {
        confirm("<b>Warning</b> <br>Are you sure you want to delete the selected records?You won't be able to restore them!", function () {

            ajaxReq('/Equipment/DeleteRule', { ruleIds: selectedRow.join(",") }, false, function (resp) {
                alert(resp.message, 'Success', 'success');
                BindRules();

            }, false, true,true);

        });

    }
}

function EditRule() {

    var selectedRow = [];
    $('#tblViewRuleDetails').find('input[type="checkbox"]:checked').each(function () {

        if ($(this).is(':checked')) {
            selectedRow.push($(this).attr('name'));
        }
    });
    if (selectedRow.length === 0) {
        $('#iconEdit').removeAttr('data-toggle').removeAttr('data-target');
        alert('Select Rule Details to edit!');
    }
    else if (selectedRow.length > 1) {
        $('#iconEdit').removeAttr('data-toggle').removeAttr('data-target');
        alert('You can only edit one rule details at a time!');
    }
    else {
        $('#tblViewRuleDetails').find('input[type="checkbox"]:checked').each(function () {
            ruleID = $(this).attr('name').trim();
            ajaxReq('/Equipment/EditRule', { id: $(this).attr('name').trim() }, false, function (resp) {
                $('#selectParentModel').val(resp[0].parent_model_id);
                BindSubType(resp[0].parent_model_id, 'selectParentModelType');
                $('#selectParentModelType').val(resp[0].parent_model_type_id).trigger('chosen:updated');
                $('#selectChildModel').val(resp[0].child_model_id).trigger('chosen:updated');
                BindSubType(resp[0].child_model_id, 'selectChildModelType');
                $('#selectChildModelType').val(resp[0].child_model_type_id).trigger('chosen:updated');
                //$('#is_active').prop('checked', resp[0].is_active);
            }, false, true,true);
        });
    }
}

function UpdateRule() {

    let ISPModelRule = {};
    ISPModelRule.parent_model_id = $('#selectParentModel option:selected').val();
    ISPModelRule.parent_model_type_id = $('#selectParentModelType option:selected').val();
    ISPModelRule.child_model_id = $('#selectChildModel option:selected').val();
    ISPModelRule.child_model_type_id = $('#selectChildModelType option:selected').val();
    //ISPModelRule.is_active = $('#is_active').is(':checked');
    ISPModelRule.id = ruleID;

    if (ISPModelRule.parent_model_id == 0) {
        alert('Select Parent Model!'); return false;
    }
    else if (ISPModelRule.parent_model_type_id == 0) {
        if (!modelIDs.includes(ISPModelRule.parent_model_id)) {
            alert('Select Parent Type!'); return false;
        }
        else {
            if (ISPModelRule.child_model_id != 0) {
                alert('Select Child Model!'); return false;
            }

        }
    }
    else if (ISPModelRule.child_model_id == 0) {
        alert('Select Child Model!'); return false;

    }
    else if (ISPModelRule.child_model_type_id == 0) {
        if (!modelIDs.includes(ISPModelRule.child_model_id)) {
            alert('Select Child Type!'); return false;
        }
    }
    else {
        //$.ajax({
        //    url: '/Equipment/UpdateRule',
        //    type: 'GET',
        //    data: ISPModelRule,
        //    success: function (res) {
        //        if (res.status == "OK") {
        //            alert(res.message, 'Success', 'success');
        //            resetRuleFields();
        //            //$('#btnclose').trigger('click');
        //            BindRules();
        //        }
        //        else {
        //            alert(res.message);
        //        }
        //    }
        //});

        ajaxReq('/Equipment/UpdateRule', ISPModelRule, false, function (res) {
            if (res.status == "OK") {
                alert(res.message, 'Success', 'success');
                resetRuleFields();
                //$('#btnclose').trigger('click');
                BindRules();
            }
            else {
                alert(res.message);
            }
        }, false, true, true);
    }

}

function ResetRule() {
    resetRuleFields();
    BindRules();
}

function resetRuleFields() {
    $('#selectParentModel').val('').trigger('chosen:updated');
    $('#selectParentModelType').val('').trigger('chosen:updated');
    $('#selectChildModel').val('').trigger('chosen:updated');
    $('#selectChildModelType').val('').trigger('chosen:updated');
}
function SearchRuleDetails() {

    if ($('#txtSearchText').val() == '' || $('#txtSearchText').val() == null) {
        $('#txtSearchText').focus();
    }
    else {
        ajaxReq('/Equipment/GetRuleDetails', { searchText: $('#txtSearchText').val().trim(), searchBy: $('#ddlSearchBy option:selected').val() }, false, function (resp) {
            $('#dvViewRuleDetails').html('');
            $('#dvRuleDetails').html('');
            $('#dvRuleDetails').append(resp);
        }, false, true,false);
    }

}

function SearchTypeDetails() {

    if ($('#txtSearchTypeText').val() == '' || $('#txtSearchTypeText').val() == null) {
        $('#txtSearchTypeText').focus();
    }
    else {
        ajaxReq('/Equipment/GetModalTypeDetails', { searchText: $('#txtSearchTypeText').val().trim(), searchBy: $('#ddlSearchTypeBy option:selected').val() }, false, function (resp) {
            $('#dvViewModalTypeDetails').html('');
            $('#dvTypeDetails').html('');
            $('#dvTypeDetails').append(resp);
        }, false, true,false);
    }
}

function EditType() {

    var selectedRow = [];
    $('#tblViewModalTypeDetails').find('input[type="checkbox"]:checked').each(function () {

        if ($(this).is(':checked')) {
            selectedRow.push($(this).attr('name'));
        }
    });
    if (selectedRow.length === 0) {
        $('#iconEdit').removeAttr('data-toggle').removeAttr('data-target');
        alert('Select Type Details To Edit!');
    }
    else if (selectedRow.length > 1) {
        $('#iconEdit').removeAttr('data-toggle').removeAttr('data-target');
        alert('You can only edit one type details at a time!');
    }
    else {
        $('#tblViewModalTypeDetails').find('input[type="checkbox"]:checked').each(function () {
            ruleID = $(this).attr('name').trim();
            ajaxReq('/Equipment/EditModelType', { id: $(this).attr('name').trim() }, false, function (resp) {
                 
                $('#hdModalTypeID').val(resp[0].id);
                $('#ddlModel').val(resp[0].model_id).trigger('chosen:updated');
                hideShowMiddlewareOption();
                $('#is_middleware_model_type').prop('checked', resp[0].is_middleware_model_type);
                checkMiddleWare();
                // $('#txtModelTypeValue').val(resp[0].value);
                if (resp[0].is_middleware_model_type) {
                    $(".middleware").not(":hidden").val(resp[0].key);
                } else {
                    $('#txtModelTypeValue').val(resp[0].value);
                }
               
                $('#txtTypeAbbr').val(resp[0].type_abbr);
              
                if (resp[0].color_code == 'transparent') {
                    $('#txtColorCode').val('');
                }
                else {
                    $('#txtColorCode').val(resp[0].color_code);
                }

                if (resp[0].stroke_code == 'transparent') {
                    $('#txtStrokeCode').val('');
                }
                else {
                    $('#txtStrokeCode').val(resp[0].stroke_code);
                }

                $('#txtColorCode').next('div').find('div > div').css('background-color', resp[0].color_code).removeClass('sp-clear-display');
                $('#txtStrokeCode').next('div').find('div > div').css('background-color', resp[0].stroke_code).removeClass('sp-clear-display');
                $('#btnUpdateModelType').show();
                $('#btnSaveType').hide();
                $("#ddlModelTypeValue").val(resp[0].type_abbr);
                loadColorList(resp[0].model_id, resp[0].model_color_id);
            }, false, true,true);
        });
    }
}

function UpdateType() {
    let ISPModelTypeMaster = {};
    let $colorType = $('#ddlModelTypeColor option:selected');
    let color = $colorType.data('typeBgcolor');
    let stroke = $colorType.data('typeOutline');
    let colorId = $colorType.val();
    let isChecked = $("#is_middleware_model_type").is(":checked");
    let modelValue = isChecked == true ? $(".middleware :selected").text() : $(".middleware").not(":hidden").val().trim();
    ISPModelTypeMaster.key = $(".middleware").not(":hidden").val().trim().replace(" ", "");// $('#txtModelTypeValue').val().replace(" ", "");
    ISPModelTypeMaster.value = modelValue;
    ISPModelTypeMaster.color_code = color;
    ISPModelTypeMaster.stroke_code = stroke;
    ISPModelTypeMaster.model_id = $('#ddlModel option:selected').val();
    ISPModelTypeMaster.type_abbr = $('#txtTypeAbbr').val();
    ISPModelTypeMaster.id = $('#hdModalTypeID').val();
    ISPModelTypeMaster.model_color_id = colorId;
    ISPModelTypeMaster.is_middleware_model_type = $('#is_middleware_model_type').is(":checked");
     
    if (ISPModelTypeMaster.model_id == 0) {
        alert('Select Model!');
    }
    else if (ISPModelTypeMaster.key == '') {
        var msg = 'Enter Type Name!'
        if (isChecked) {
            msg = 'Please select Type Name'
        }
    }
    else if (ISPModelTypeMaster.type_abbr == '') {
        alert('Enter Type Abbreviation!');
    }
    else if (ISPModelTypeMaster.type_abbr.length < $('#hdTAminLenght').val() || ISPModelTypeMaster.type_abbr.length > $('#hdTAmaxLenght').val()) {
        alert('Type Abbreviation must be Minimum ' + $('#hdTAminLenght').val() + ' & Maximum ' + $('#hdTAmaxLenght').val() + ' characters long!')
    }
    else {
        //$.ajax({
        //    url: '/Equipment/UpdateModelType',
        //    type: 'GET',
        //    data: ISPModelTypeMaster,
        //    success: function (res) {
        //        if (res.status == "OK") {

        //            alert(res.message, 'Success', 'success');
        //            ResetType();//BindModalTypeById($('#ddlModel option:selected').val());
        //            $('#txtModelTypeKey').val('');
        //            $('#txtModelTypeValue').val('');
        //            $('#txtColorCode').val('');
        //            $('#txtStrokeCode').val('');
        //            $('#txtTypeAbbr').val('');
        //            $('#txtColorCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
        //            $('#txtStrokeCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
        //            //$('#btnclose').trigger('click');
        //            $('#btnUpdateModelType').hide();
        //            $('#btnSaveType').show();
        //        }
        //        else {
        //            alert(res.message);
        //        }

        //    }
        //});
        ajaxReq('/Equipment/UpdateModelType',  ISPModelTypeMaster , false, function (res) {
            if (res.status == "OK") {

                alert(res.message, 'Success', 'success');
                ResetType();//BindModalTypeById($('#ddlModel option:selected').val());
                $('#txtModelTypeKey').val('');
                $('#txtModelTypeValue').val('');
                $('#txtColorCode').val('');
                $('#txtStrokeCode').val('');
                $('#txtTypeAbbr').val('');
                $('#txtColorCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
                $('#txtStrokeCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
                //$('#btnclose').trigger('click');
                $('#btnUpdateModelType').hide();
                $('#btnSaveType').show();
                $('#ddlModelTypeColor').val('').trigger('change');
            }
            else {
                alert(res.message);
            }

        
        }, false, true, true);
    }

}

$(document).on("change", "#is_middleware_model_type", function () {
    checkMiddleWare();
}).on("change", "#ddlModelTypeValue", function () {
    var abbr = $('option:selected', this).attr('data-layer-abbr');
    $("#txtTypeAbbr").val(abbr);
});

function hideShowMiddlewareOption() {
    var modelType = $("#ddlModel :selected").text();
    $("#is_middleware_model_type").prop("checked", false);
    checkMiddleWare();
    $(".middlewareOption").hide();
    if (modelType.toUpperCase() == 'EQUIPMENT') {
        $(".middlewareOption").show();
    }
}


function checkMiddleWare()
{
    $(".middleware").val("").hide();
    $("#txtTypeAbbr").val("");
    $("#txtTypeAbbr").removeAttr("readonly");
    if ($("#is_middleware_model_type").is(":checked")) {
        $("#ddlModelTypeValue").val("");
        $("#ddlModelTypeValue").show();
        $("#txtTypeAbbr").attr("readonly",true);
    } else {
        $("#txtModelTypeValue").show();
       
    }
}
function SaveType() {
    let ISPModelTypeMaster = {};
    let $colorType = $('#ddlModelTypeColor option:selected');
    let color = $colorType.data('typeBgcolor');
    let stroke = $colorType.data('typeOutline');
    let colorId = $colorType.val();
    //let isChecked = $("#is_middleware_model_type").is(":checked");
    //ISPModelTypeMaster.key = isChecked == true ? $(".middleware :selected").text().replace(" ", "") : $(".middleware").not(":hidden").val().replace(" ", "");// $('#txtModelTypeValue').val().replace(" ", "");
    //ISPModelTypeMaster.value = $(".middleware").not(":hidden").val();//$('#txtModelTypeValue').val();
    let isChecked = $("#is_middleware_model_type").is(":checked");
    let modelValue = isChecked == true ? $(".middleware :selected").text() : $(".middleware").not(":hidden").val().trim();
    ISPModelTypeMaster.key = $(".middleware").not(":hidden").val().trim().replace(" ", "");// $('#txtModelTypeValue').val().replace(" ", "");
    ISPModelTypeMaster.value = modelValue;
    ISPModelTypeMaster.color_code = color;
    ISPModelTypeMaster.stroke_code = stroke;
    ISPModelTypeMaster.model_id = $('#ddlModel option:selected').val();
    ISPModelTypeMaster.type_abbr = $('#txtTypeAbbr').val();
    ISPModelTypeMaster.model_color_id = colorId;
    ISPModelTypeMaster.is_middleware_model_type = $("#is_middleware_model_type").is(":checked");
    if (ISPModelTypeMaster.model_id == 0) {
        alert('Select Model!');
    }
    else if (ISPModelTypeMaster.key == '') {
        var msg = 'Enter Type Name!'
        if (isChecked) {
            msg = 'Please select Type Name'
        }
        alert(msg);
    }
        //else if (ISPModelTypeMaster.value == '') {
        //    alert('Enter value.');
        //}
        //else if (ISPModelTypeMaster.color_code == '') {
        //    alert('Enter color code.');
        //}
        //else if (ISPModelTypeMaster.stroke_code == '') {
        //    alert('Enter stroke code.');
        //}
    else if (ISPModelTypeMaster.type_abbr == '') {
        alert('Enter Type Abbreviation!');
    }
    else if (ISPModelTypeMaster.type_abbr.length < $('#hdTAminLenght').val() || ISPModelTypeMaster.type_abbr.length > $('#hdTAmaxLenght').val()) {
        alert('Type Abbreviation must be Minimum ' + $('#hdTAminLenght').val() + ' & Maximum ' + $('#hdTAmaxLenght').val() + ' characters long!')
    }
    else {
        //$.ajax({
        //    url: '/Equipment/SaveModelType',
        //    type: 'GET',
        //    data: ISPModelTypeMaster,
        //    success: function (res) {
        //        if (res.status == "OK") {

        //            alert(res.message, 'Success', 'success');
        //            ResetType();//BindModalTypeById($('#ddlModel option:selected').val());
        //            $('#txtModelTypeKey').val('');
        //            $('#txtModelTypeValue').val('');
        //            $('#txtColorCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
        //            $('#txtStrokeCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
        //            //$('#btnclose').trigger('click');
        //        }
        //        else {
        //            alert(res.message);
        //        }

        //    }
        //});

        ajaxReq('/Equipment/SaveModelType', ISPModelTypeMaster, false, function (res) {
            if (res.status == "OK") {

                alert(res.message, 'Success', 'success');
                ResetType();//BindModalTypeById($('#ddlModel option:selected').val());
                $('#txtModelTypeKey').val('');
                $('#txtModelTypeValue').val('');

                $('#txtColorCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
                $('#txtStrokeCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
                //$('#btnclose').trigger('click');
                $('#ddlModelTypeColor').val('').trigger('change');
            }
            else {
                alert(res.message);
            }
        }, false, true, true);
    }


}

function DeleteType() {
    var selectedRow = [];
    $('#tblViewModalTypeDetails').find('input[type="checkbox"]:checked').each(function () {
        if ($(this).is(':checked')) {
            selectedRow.push($(this).attr('name'));
        }
    });

    if (selectedRow.length === 0) {
        alert("Select Record To Delete!");
    }
    else {
        confirm("<b>Warning</b> <br>Are you sure you want to delete the selected records?You won't be able to restore them!", function () {

            ajaxReq('/Equipment/DeleteModalType', { modeltypeids: selectedRow.join(",") }, false, function (resp) {
                alert(resp.message, 'Success', 'success');
                BindModalTypeById($('#ddlModel option:selected').val());
            }, false, true,true);

        });

    }
}

function ResetType() {
    $('#ddlModel').val('').trigger('chosen:updated');
    $('#txtModelTypeKey').val('');
    $('#txtModelTypeValue').val('');
    $('#txtColorCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
    $('#txtStrokeCode').next('div').find('div > div').css('background-color', 'transparent').addClass('sp-clear-display');
    $('#btnUpdateModelType').hide();
    $('#btnSaveType').show();
    $('#txtColorCode').val('');
    $('#txtStrokeCode').val('');
    $('#txtTypeAbbr').val('');
    $('#is_middleware_model_type').prop('checked',false);
    $('#ddlModelTypeColor').val('').trigger('change');
    BindModalType();
    hideShowMiddlewareOption();
}

function BindModalTypeById(val) {
    var ISPModelTypeMaster = {};
    ISPModelTypeMaster.model_id = val;
    hideShowMiddlewareOption();
    ajaxReq('/Equipment/GetModalTypeDetails',  ISPModelTypeMaster, false, function (resp) {
        $('#dvTypeDetails').html('');
        $('#dvTypeDetails').append(resp);
    }, false, true, false);
    loadColorList(val);
}

function loadColorList(model_id,selectedColor) {
    ajaxReq('/Equipment/GetModelSubTypeColors', { modelId: model_id }, false, function (resp) {
        let $colors = $('#ddlModelTypeColor');
        renderColorSelector({
            data: resp,
            element: $colors,
            value: 'id',
            //text: 'key',
            defaultText: 'Select Color',
            dataKey: [{ key: 'outline_color_code', name: 'data-type-outline' }, { key: 'fill_color_code', name: 'data-type-bgcolor' }],
            color: 'outline_color_code',
            bg: 'fill_color_code',
            disabled: false
        });
        if (model_id == modelKeys.equipmentKey) {
            $colors.find('option').addClass('equipment');
        }
        $colors.val(selectedColor);
        $colors.trigger('change');
    }, false, true, true);
}

function clearCheckbox() {
    $('#tblViewModalTypeDetails').find('input[type="checkbox"]:checked').each(function () {

        $(this).prop('checked', false);
    });

    $('#tblViewRuleDetails').find('input[type="checkbox"]:checked').each(function () {
        $(this).prop('checked', false);
    });
}

function AllowCharacters() {

    if ((event.keyCode > 64 && event.keyCode < 91) || (event.keyCode > 96 && event.keyCode < 123) || event.keyCode == 8)
        return true;
    else {
        return false;
    }

}
function getSelectedTypes() {
    var selectedValues = [];
    $("#selectChildModelType :selected").each(function () {
        selectedValues.push($(this).val());
    });
    console.log(selectedValues);
    return selectedValues;
}
function showUnitContext (flag) {
    if (flag)
        $('.unit_panel').show();
    else
        $('.unit_panel').hide();
}
function showMultiConnectionContext(flag) {
    if (flag)
        $('#multiConnectionContainer').show();
    else
        $('#multiConnectionContainer').hide();
}
////////////////////////////////////////END:Model Type and Model Rule//////////////////////////////