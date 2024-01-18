class ControlInitializer {
    constructor() {
        this.bindEvents();
        this.InitializeControls();
    }
    bindEvents = function () {
        $(dynamicForm.DE.divControlForm).empty();
        $(dynamicForm.DE.btnCreateTextBox).unbind('click');
        $(dynamicForm.DE.btnCreateDropdown).unbind('click');
        $(dynamicForm.DE.btnCreateDate).unbind('click');

        $(dynamicForm.DE.btnCreateTextBox).on("click", function () {
            dynamicForm.controlGenerator.CreateControls(dynamicForm.Controls.TEXT);
        });
        $(dynamicForm.DE.btnCreateDropdown).on("click", function () {
            dynamicForm.controlGenerator.CreateControls(dynamicForm.Controls.DROPDOWN);
        });
        $(dynamicForm.DE.btnCreateDate).on("click", function () {
            dynamicForm.controlGenerator.CreateControls(dynamicForm.Controls.DATE);
        });
        $(dynamicForm.DE.btnAdd).on("click", function () {
            dynamicForm.controlEvents.AddTextBox();
        });
        $(dynamicForm.DE.btnSave).on("click", function () {
            var controlId = $(dynamicForm.DE.divControlForm).find(dynamicForm.DE.selectedField).attr("id");
            let panel = $(dynamicForm.DE.divControlSettings).find("#" + controlId + "");
            if ($(dynamicForm.DE.divControlSettings).find("#" + controlId + "").find($("[name='field_label']")).val().trim() == dynamicForm.Empty) {
                $(dynamicForm.DE.divControlSettings).find("#" + controlId + "").find($("[name='field_label']")).addClass(dynamicForm.DE.input_validation_error);
                new Noty({ type: 'error', text: dynamicForm.Messages.FIELD_LABEL_CANT_BE_EMPTY }).show();
                return;

            }
            var data = dynamicForm.controlInitializer.CustomSerializeArray(panel);
            data["is_mandatory"] = data["is_mandatory"] == "on" ? true : false;
            var entityId = $(dynamicForm.DE.divControlForm).parents().find('#LayerName option:selected').val();
            var entityName = $(dynamicForm.DE.divControlForm).parents().find('#LayerName option:selected').text();
            dynamicForm.controlGenerator.GetExistingFields(entityId, entityName, false);
            for (var i = 0; i < dynamicForm.existingFields.length; i++) {
                if (dynamicForm.existingFields[i].field_label == data.field_label && dynamicForm.existingFields[i].id != data.id) {
                    new Noty({ type: 'error', text: dynamicForm.Messages.FIELD_NAME_ALREADY_EXISTS }).show();
                    $(dynamicForm.DE.divControlSettings).find("#" + controlId + "").find($("[name='field_label']")).addClass(dynamicForm.DE.input_validation_error);
                    return false;
                }
            }
            var btnText = $(dynamicForm.DE.btnSave).val();
            var isvalid = dynamicForm.controlValidator.ValidateForm(data);
            if (isvalid == true) {
                if (btnText == "Save") {
                    dynamicForm.controlInitializer.Save(data);
                }
                else {
                    dynamicForm.controlInitializer.Update(data);
                }
            }
        });
    }
    Save = function (data) {
        if (dynamicForm.SELECTED_CONTROL == dynamicForm.Controls.DROPDOWN) {
            var values = [];
            var id = "";
            var is_default = [];
            var IsVisible = [];
            $(dynamicForm.DE.divControlSettings)
                .find("#" + data.id)
                .find(dynamicForm.DE.choices)
                .find(dynamicForm.DE.TextBoxContainer)
                .find('[name="DynamicTextBox"]')
                .each(function () {
                    values.push($(this).val());
                });
            $(dynamicForm.DE.divControlSettings)
                .find("#" + data.id)
                .find(dynamicForm.DE.choices)
                .find(dynamicForm.DE.TextBoxContainer)
                .find('[name="Default"]')
                .each(function () {
                    if (this.checked == true) {
                        is_default.push('true');
                    }
                    else {
                        is_default.push('false');
                    }
                });
            $(dynamicForm.DE.divControlSettings)
                .find("#" + data.id)
                .find(dynamicForm.DE.choices)
                .find(dynamicForm.DE.TextBoxContainer)
                .find('[name="IsVisible"]')
                .each(function () {
                    if (this.checked == true) {
                        IsVisible.push('true');
                    }
                    else {
                        IsVisible.push('false');
                    }
                });
            ajaxReq('DynamicForm/Save', { dynamicControls: data }, true, function (response) {
                $(dynamicForm.DE.btnSave).hide();
                $(dynamicForm.DE.btnCreateTextBox).removeAttr("disabled");
                $(dynamicForm.DE.btnCreateDropdown).removeAttr("disabled");
                $(dynamicForm.DE.divControlForm).find("#" + data.id).remove();
                $(dynamicForm.DE.divControlSettings).find("#" + data.id).remove();
                id = response.results.id;
                dynamicForm.ID = id;
                if (values != "") {
                    ajaxReq('DynamicForm/SaveDropDown', { DropDownAdd: values, is_default: is_default, IsVisible: IsVisible, id: id }, true, function (response) {

                    }, true, true);
                }
                new Noty({ type: 'success', text: response.error_message }).show();
                dynamicForm.controlGenerator.GetExistingControl(id);
                return false;
            }, true, true);
        }
        else if (dynamicForm.SELECTED_CONTROL == dynamicForm.Controls.TEXT) {
            ajaxReq('DynamicForm/Save', { dynamicControls: data }, true, function (response) {
                $(dynamicForm.DE.btnSave).hide();
                $(dynamicForm.DE.divControlForm).find("#" + data.id).remove();
                $(dynamicForm.DE.divControlSettings).find("#" + data.id).remove();
                dynamicForm.controlGenerator.GetExistingControl(response.results.id);
                dynamicForm.ID = response.results.id;
                $(dynamicForm.DE.btnCreateTextBox).removeAttr("disabled");
                $(dynamicForm.DE.btnCreateDropdown).removeAttr("disabled");
                new Noty({ type: 'success', text: response.error_message }).show();
                return false;
            }, true, true);
        }
    }
    Update = function (data) {
        if (dynamicForm.SELECTED_CONTROL == dynamicForm.Controls.DROPDOWN) {
            var values = [];
            var id = "";
            var is_default = [];
            var IsVisible = [];
            $(dynamicForm.DE.divControlSettings)
                .find("#" + data.id)
                .find(dynamicForm.DE.choices)
                .find(dynamicForm.DE.TextBoxContainer)
                .find('[name="DynamicTextBox"]')
                .each(function () {
                    values.push($(this).val());
                });
            $(dynamicForm.DE.divControlSettings)
                .find("#" + data.id)
                .find(dynamicForm.DE.choices)
                .find(dynamicForm.DE.TextBoxContainer)
                .find('[name="Default"]')
                .each(function () {
                    if (this.checked == true) {
                        is_default.push('true');
                    }
                    else {
                        is_default.push('false');
                    }
                });
            $(dynamicForm.DE.divControlSettings)
                .find("#" + data.id)
                .find(dynamicForm.DE.choices)
                .find(dynamicForm.DE.TextBoxContainer)
                .find('[name="IsVisible"]')
                .each(function () {
                    if (this.checked == true) {
                        IsVisible.push('true');
                    }
                    else {
                        IsVisible.push('false');
                    }
                });

            ajaxReq('DynamicForm/Update', { dynamicControls: data }, true, function (response) {
                $(dynamicForm.DE.btnSave).hide();
                $(dynamicForm.DE.btnCreateTextBox).removeAttr("disabled");
                $(dynamicForm.DE.btnCreateDropdown).removeAttr("disabled");
                $(dynamicForm.DE.divControlForm).find("#" + data.id).remove();
                $(dynamicForm.DE.divControlSettings).find("#" + data.id).remove();
                id = response.results.id;
                dynamicForm.ID = id;
                if (values != "") {
                    ajaxReq('DynamicForm/UpdateDropDown', { DropDownAdd: values, is_default: is_default, IsVisible: IsVisible, id: id }, false, function (response) {

                    }, true, true);
                }
                dynamicForm.controlGenerator.GetExistingControl(id);
                new Noty({ type: 'success', text: response.error_message }).show();
                return false;
            }, true, true);
        }
        else if (dynamicForm.SELECTED_CONTROL == dynamicForm.Controls.TEXT) {
            ajaxReq('DynamicForm/Update', { dynamicControls: data }, true, function (response) {
                $(dynamicForm.DE.btnSave).hide();
                $(dynamicForm.DE.divControlForm).find("#" + data.id).remove();
                $(dynamicForm.DE.divControlSettings).find("#" + data.id).remove();
                dynamicForm.controlGenerator.GetExistingControl(response.results.id);
                dynamicForm.ID = response.results.id;
                $(dynamicForm.DE.btnCreateTextBox).removeAttr("disabled");
                $(dynamicForm.DE.btnCreateDropdown).removeAttr("disabled");
                new Noty({ type: 'success', text: response.error_message }).show();
                return false;
            }, true, true);
        }
    }
    RefreshPage = function () {
        var entityId = parseInt($(dynamicForm.DE.LayerName).val());
        var entityName = $('#LayerName option:selected').text();
        $(dynamicForm.DE.divControlForm).empty();
        dynamicForm.controlGenerator.GetExistingFields(entityId, entityName, true);
        dynamicForm.controlInitializer.DisableControls();
    }
    InitializeControls = function () {
        $(dynamicForm.DE.tabs).hide();
        this.DisableControls();
    }
    ShowPanelAfterSaving = function (field, type) {
        var element = $(dynamicForm.DE.divControlForm).find(dynamicForm.DE.selectedField);

        dynamicForm.controlGenerator.CreatePanel(field, element, type);
    }
    CustomSerializeArray = function (panelSettings) {
        var panel = panelSettings.serializeArray();
        var data = {};
        $.each(panel, function () {
            if (data[this.name] !== undefined) {
                if (!data[this.name].push) {
                    data[this.name] = [data[this.name]];
                }
                data[this.name].push(this.value || '');
            } else {
                data[this.name] = this.value || '';
            }
        });
        return data;
    }
    DisableControls = function () {
        var isNewControl = false;
        for (var i = 0; i < $(dynamicForm.DE.divControlForm).children().length; i++) {
            var divId = $(dynamicForm.DE.divControlForm).children()[i].id;
            var val = $("#" + divId).attr('data-attr')
            if (val == "NEW") {
                isNewControl = true;
                $(dynamicForm.DE.btnSave).show();
                $(dynamicForm.DE.btnCreateTextBox).attr("disabled", "disabled");
                $(dynamicForm.DE.btnCreateDate).attr("disabled", "disabled");
                $(dynamicForm.DE.btnCreateDropdown).attr("disabled", "disabled");
                break;
                return;
            };
        }

        let controlCount = $(dynamicForm.DE.divControlForm).children().length;
        if (controlCount == 0) {
            $(dynamicForm.DE.btnSave).hide();
            $(dynamicForm.DE.btnCreateTextBox).removeAttr("disabled");
            $(dynamicForm.DE.btnCreateDate).removeAttr("disabled");
            $(dynamicForm.DE.btnCreateDropdown).removeAttr("disabled");
        }
        else if (controlCount > 0 && isNewControl) {
            $(dynamicForm.DE.btnSave).show();
            $(dynamicForm.DE.btnCreateTextBox).attr("disabled", "disabled");
            $(dynamicForm.DE.btnCreateDate).attr("disabled", "disabled");
            $(dynamicForm.DE.btnCreateDropdown).attr("disabled", "disabled");
        }
        else {
            $(dynamicForm.DE.btnSave).hide();
            $(dynamicForm.DE.btnCreateTextBox).removeAttr("disabled");
            $(dynamicForm.DE.btnCreateDate).removeAttr("disabled");
            $(dynamicForm.DE.btnCreateDropdown).removeAttr("disabled");
        }
    }
    RemoveField = function (field, button) {
        dynamicForm._fieldVal = field;
        dynamicForm._buttonVal = button;
        let fieldLabel = $("#" + field).find('label').text().trim();
        openModal();
        //set the message for field
        $('div.deleteModal h2 b#label').html(fieldLabel);
        $('div.deleteModal h3 b#confirmLabel').html(fieldLabel);

        if ($(dynamicForm.DE.btnDeleteControl).hasClass('btn--box_ok')) {
            document.getElementById("btnDeleteControl").classList.remove('btn--box_ok');
            document.getElementById("btnDeleteControl").classList.add('btn--box_ok_disable');
        }
        $(dynamicForm.DE.btnDeleteControl).attr('disabled', true);
        //let _controlName = $(dynamicForm.DE.deleteControlName).val();
        //confirm("Are you sure to delete this control? It will take some time.", function () {
        //    let val = $("#" + field).attr('data-attr');
        //    let entityId = $(button).parents().find('#LayerName option:selected').val();
        //    let entityName = $(button).parents().find('#LayerName option:selected').text();
        //    let fieldLabel = $("#" + field).find('label').text().trim();
        //    $("#" + field).remove();
        //    $(dynamicForm.DE.divControlSettings).find("#" + field).remove();
        //    dynamicForm.controlInitializer.DisableControlsIfNewFieldExistsInControls();
        //    dynamicForm.existingFields.length = 0;

        //    if (val == "EXISTING") {
        //        ajaxReq('DynamicForm/DeleteSingleField', { control_id: field, entityId: entityId, entityName: entityName, fieldLabel: fieldLabel }, true, function (response) {
        //            new Noty({ type: 'success', text: response.error_msg }).show();
        //        }, true, true, true);
        //    }
        //    else {
        //        new Noty({ type: 'success', text: dynamicForm.Messages.FIELD_DELETED_SUCCESSFULLY }).show();
        //    }
        //});
    }

    DeleteField = function () {
        //let fieldLabel = $(dynamicForm.DE.deleteControlName).val();
        //if (fieldLabel == null || fieldLabel == '') {
        //    new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_ENTER_CONTROL_NAME }).show();
        //    return false;
        //}
        let val = $("#" + dynamicForm._fieldVal).attr('data-attr');
        let entityId = $(dynamicForm._buttonVal).parents().find('#LayerName option:selected').val();
        let entityName = $(dynamicForm._buttonVal).parents().find('#LayerName option:selected').text();
        let fieldLabel = $("#" + dynamicForm._fieldVal).find('label').text().trim();
        $("#" + dynamicForm._fieldVal).remove();
        $(dynamicForm.DE.divControlSettings).find("#" + dynamicForm._fieldVal).remove();
        dynamicForm.controlInitializer.DisableControlsIfNewFieldExistsInControls();
        dynamicForm.existingFields.length = 0;

        

        if (val == "EXISTING") {
            ajaxReq('DynamicForm/DeleteSingleField', { control_id: dynamicForm._fieldVal, entityId: entityId, entityName: entityName, fieldLabel: fieldLabel }, true, function (response) {
                new Noty({ type: 'success', text: response.error_msg }).show();
            }, true, true, true);
        }
        else {
            new Noty({ type: 'success', text: dynamicForm.Messages.FIELD_DELETED_SUCCESSFULLY }).show();
        }
        dynamicForm._buttonVal = null;
        dynamicForm._fieldVal = null;
        closeAllModal();
    }
    DisableControlsIfNewFieldExistsInControls = function () {
        if ($(dynamicForm.DE.divControlForm).find("[data-attr='NEW']").length > 0) {
            $(dynamicForm.DE.btnSave).show();
            $(dynamicForm.DE.btnCreateTextBox).attr("disabled", "disabled");
            $(dynamicForm.DE.btnCreateDate).attr("disabled", "disabled");
            $(dynamicForm.DE.btnCreateDropdown).attr("disabled", "disabled");
            return;
        } else {
            $(dynamicForm.DE.btnCreateTextBox).removeAttr("disabled");
            $(dynamicForm.DE.btnCreateDate).removeAttr("disabled");
            $(dynamicForm.DE.btnCreateDropdown).removeAttr("disabled");
        }
    }
}