class ControlGenerator {
    CreateControls = function (type) {
        var UniqueID = this.GenerateID();
        switch (type) {
            case dynamicForm.Controls.TEXT:
                dynamicForm.htmlRenderer.CreateTextBox(UniqueID);
                break;
            case dynamicForm.Controls.DROPDOWN:
                dynamicForm.htmlRenderer.CreateDropdown(UniqueID);
                break;
            case dynamicForm.Controls.DATE:
                dynamicForm.htmlRenderer.CreateDate(UniqueID);
                break;
            default:
        }
        dynamicForm.SELECTED_CONTROL = type;
    }
    GenerateID = function () {
        var result = '';
        var characters = dynamicForm.KeyString;
        var charactersLength = characters.length;
        for (var i = 0; i < 30; i++) {
            result += characters.charAt(Math.floor(Math.random() *
                charactersLength));
        }
        return result;
    }
    CreateControlsSettingsHtml = function (fieldId) {
        var builder = "<form class='form-row--vert ml-4'  id=" + fieldId + ">";
        builder += dynamicForm.htmlRenderer.CreateFieldLabel(fieldId);
        builder += dynamicForm.htmlRenderer.CreateRequiredCheckbox(fieldId);
        builder += dynamicForm.htmlRenderer.CreateFieldType(fieldId);
        builder += dynamicForm.htmlRenderer.CreateDecimalRoundNumber(fieldId);
        builder += dynamicForm.htmlRenderer.CreateRangeBox(fieldId);
        builder += dynamicForm.htmlRenderer.PlaceHolderText(fieldId);
        builder += dynamicForm.htmlRenderer.DefaultValues(fieldId);
        builder += " </form>";
        return builder;
    }
    CreateControlsDropdownSettingsHtml = function (fieldId) {
        var builder = "<form class='form-row--vert ml-4'  id=" + fieldId + ">";
        builder += dynamicForm.htmlRenderer.CreateFieldLabelForDropDown(fieldId);
        builder += dynamicForm.htmlRenderer.CreateRequiredCheckbox(fieldId);
        builder += dynamicForm.htmlRenderer.CreateDropDownOption(fieldId);
        builder += dynamicForm.renderExistingControls.RenderDefaultValuesDropdown(fieldId);
        builder += " </form>";
        return builder;

    }
    SetDefaultLabel = function (fieldId, value) {
        $(dynamicForm.DE.divControlForm).find('input[data-attr=' + fieldId + ']').val(value);
        dynamicForm.controlValidator.DynamicPatternValidator(fieldId);
        var resp = dynamicForm.controlValidator.ResourceRangeValidation(value, dynamicForm.DE.minRangePattern, dynamicForm.DE.maxRangePattern, dynamicForm.DE.pattern);
        if (resp == true)
            $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='default_value']")).removeClass(dynamicForm.DE.input_validation_error);
        else
            $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='default_value']")).addClass(dynamicForm.DE.input_validation_error);
    }
    SetPlaceHolderLabel = function (fieldId, value) {
        $(dynamicForm.DE.divControlForm).find('input[data-attr=' + fieldId + ']').attr("placeholder", value);
    }
    SetFieldLabel = function (fieldId, value) {
        $(dynamicForm.DE.divControlForm).children().find("." + fieldId).text(value);
    }
    IsSettingPanelCreated = function (fieldId) {
        var elements = $(dynamicForm.DE.divControlSettings);
        var count = elements.find("#" + fieldId).length;
        if (count == 0)
            return true;
        return false;
    }
    CreatePanelTextBox = function (field, control) {
        this.SetButtonText(control);
        $(dynamicForm.DE.divControlForm).children().removeClass(dynamicForm.Controls.selectedField);
        $("#" + field).addClass(dynamicForm.Controls.selectedField);
        this.ShowCurrentFieldPanel(field);
        var isNotCreated = dynamicForm.controlGenerator.IsSettingPanelCreated(field);
        if (isNotCreated) {
            var settingHtml = dynamicForm.controlGenerator.CreateControlsSettingsHtml(field);
            $(dynamicForm.DE.divControlSettings).append(settingHtml);
        }

    }
    CreatePanel = function (field, control, type) {
        dynamicForm.SELECTED_CONTROL = type;
        if (type == dynamicForm.Controls.TEXT) {
            this.CreatePanelTextBox(field, control);
        }
        else if (type == dynamicForm.Controls.DROPDOWN) {
            this.CreateDropdownPanel(field, control);
        }

    }
    CreateDropdownPanel = function (field, control) {
        this.SetButtonText(control);
        $(dynamicForm.DE.divControlForm).children().removeClass(dynamicForm.Controls.selectedField);
        $("#" + field).addClass(dynamicForm.Controls.selectedField);
        this.ShowCurrentFieldPanel(field);
        var isNotCreated = dynamicForm.controlGenerator.IsSettingPanelCreated(field);
        if (isNotCreated) {
            var settingHtml = dynamicForm.controlGenerator.CreateControlsDropdownSettingsHtml(field);
            $(dynamicForm.DE.divControlSettings).append(settingHtml);
        }
    }
    SetButtonText = function (control) {
        var controlType = $(control.parentElement).attr('data-attr');
        if (controlType == "NEW") {
            $(dynamicForm.DE.btnSave).val(dynamicForm.Operation.SAVE);
        }
        else {
            $(dynamicForm.DE.btnSave).val(dynamicForm.Operation.UPDATE);
        }
        $(dynamicForm.DE.btnSave).show();
    }
    GetExistingFields = function (entityID, entityName, isrendertrue) {
        if (entityID == 0) return;
        ajaxReq('DynamicForm/ExistingFields', { layerId: entityID }, false, function (response) {
            response.LayerName = entityName;
            dynamicForm.existingFields = response.dynamicControls;
            if (isrendertrue == true) {
                dynamicForm.controlGenerator.Rendering(response);
            }
        }, false, true);
    }
    Rendering = function (response) {
        for (var i = 0; i < response.dynamicControls.length; i++) {
            dynamicForm.renderExistingControls.RenderControls(response.dynamicControls[i]);
        }
        for (var i = 0; i < response.dynamicControls.length; i++) {
            switch (response.dynamicControls[i].control_type) {
                case dynamicForm.Controls.TEXT:
                    var builder = dynamicForm.renderExistingControls.RenderControlSettings(response.dynamicControls[i]);
                    break;
                case dynamicForm.Controls.DROPDOWN:
                    var builder = dynamicForm.renderExistingControls.RenderControlSettingsforDDL(response.dynamicControls[i]);
                    break;
            }
            $(dynamicForm.DE.divControlSettings).append(builder);
        }
        dynamicForm.renderExistingControls.OnRenderComplete();
        $(dynamicForm.DE.divControlSettings).children().hide();
    }
    GetExistingControl = function (controlID) {
        ajaxReq('DynamicForm/ExistingControl', { controlID: controlID }, true, function (response) {
            dynamicForm.renderExistingControls.RenderControls(response);
            switch (response.control_type) {
                case dynamicForm.Controls.TEXT:
                    var builder = dynamicForm.renderExistingControls.RenderControlSettings(response);
                    break;
                case dynamicForm.Controls.DROPDOWN:
                    var builder = dynamicForm.renderExistingControls.RenderControlSettingsforDDL(response);
                    break;
            }
            $(dynamicForm.DE.divControlSettings).append(builder);
            dynamicForm.renderExistingControls.OnRenderComplete();
            $(dynamicForm.DE.divControlSettings).children().hide();
            dynamicForm.existingFields.push(response);
            //dynamicForm.controlInitializer.ShowPanelAfterSaving(controlID, response.control_type);
        }, false, true);
    }
    ShowCurrentFieldPanel = function (field) {

        var isVisible = $(dynamicForm.DE.divControlSettings).find("#" + field).is(':visible');
        var roundOff = parseInt($(dynamicForm.DE.divControlSettings).find("#" + field).find(dynamicForm.DE.roundOff).find("input").val());
        if (!isVisible) {
            $(dynamicForm.DE.divControlSettings).children().fadeOut();
            $(dynamicForm.DE.divControlSettings).find("#" + field).fadeIn()
        }
        if (roundOff > 0) {
            $(dynamicForm.DE.divControlSettings).find("#" + field).find(dynamicForm.DE.roundOff).show();
        }
        else {
            $(dynamicForm.DE.divControlSettings).find("#" + field).find(dynamicForm.DE.roundOff).hide();
        }
    }
    GetExistingDynamicTextBox = function (value) {
        var builder = "<div class='right--form_input'>";
        builder += "<input type='textbox' name='DynamicTextBox' value='" + value.display_text + "' disabled class='form-control mr-01' />";
        builder += "<button type='button' title='Remove' class='right--form_btn mr-01' onclick='dynamicForm.controlEvents.RemoveTextBox(this)'>";
        builder += "<img src='../../Areas/Admin/Content/images/minus.svg' style='width:14px'>";
        builder += "</button>";
        /*builder += "<input type='radio' name='Default'";*/
        builder += "<label class='cust--radio' title='Default'>";
        builder += "<input  type='radio'";
        if (value.is_default == true) {
            builder += "checked";
        }
        builder += " class='mr-01' name='Default'  onclick = 'dynamicForm.controlEvents.SetDefaultValue(this)' /> ";
        builder += "<span class='radio--mark choice--r-c-top'></span>";
        builder += "</label>";
        builder += "<label class='checkBlock' title='Is Visible'>";
        builder += "<input type='checkbox' ";
        if (value.isvisible == true) {
            builder += " checked ";
        }
        builder += "name = 'IsVisible'/> ";
        builder += "<span class='checkmark choice--r-c-top'></span>";
        builder += "</label>";
        builder += "</div>";
        return builder;
    }
    GetDynamicTextBox = function (value) {
        var builder = "<div class='right--form_input'>";
        builder += "<input type='textbox' name='DynamicTextBox' value='" + value + "' disabled class='form-control mr-01' />";
        builder += "<button type='button' title='Remove' class='right--form_btn mr-01' onclick='dynamicForm.controlEvents.RemoveTextBox(this)'>";
        builder += "<img src='../../Areas/Admin/Content/images/minus.svg' style='width:14px'>";
        builder += "</button>";
        builder += "<label class='cust--radio' title ='Default'>";
        builder += "<input type='radio' name='Default'  class='mr-01' onclick = 'dynamicForm.controlEvents.SetDefaultValue(this)' /> ";
        builder += "<span class='radio--mark choice--r-c-top'></span>";
        builder += "</label>";
        builder += "<label class='checkBlock' title='Is Visible'>";
        builder += "<input type='checkbox' checked  name='IsVisible' />";
        builder += "<span class='checkmark choice--r-c-top'></span>";
        builder += "</label>";
        builder += "</div>";
        return builder;
    }
}