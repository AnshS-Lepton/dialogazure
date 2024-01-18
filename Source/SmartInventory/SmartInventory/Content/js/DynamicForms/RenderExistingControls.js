class RenderExistingControls {
    RenderControlSettings = function (control) {
        var builder = "<form class='form-row--vert ml-4' id=" + control.id + ">";
        builder += this.RenderFieldLabel(control);
        builder += this.RenderRequiredCheckbox(control);
        builder += this.RenderFieldType(control);
        builder += this.RenderDecimalRoundNumber(control);
        builder += this.RenderRangeBox(control);
        builder += this.RenderPlaceHolderText(control);
        builder += this.RenderDefaultValuesText(control);
        builder += " </form>";
        return builder;
    }
    RenderControlSettingsforDDL = function (control) {
        var builder = "<form class='form-row--vert ml-4' id=" + control.id + ">";
        builder += this.RenderFieldLabelDropDown(control);
        builder += this.RenderRequiredCheckbox(control);
        builder += this.RenderDropDownOption(control);
        builder += this.RenderDefaultValuesDropdown(control);
        builder += " </form>";
        return builder;
    }
    RenderFieldLabelDropDown = function (control) {

        var builder = "<div class='f-property-row'>";
        builder += "<label class=" + control.id + " name=" + control.id + ">Field Label</label>";
        builder += "<div class='form-fields'>";
        builder += "<input type='text' placeholder='" + control.placeholder_text + "' value='" + control.field_label + "' name='field_label' class='form-control' onkeyup=dynamicForm.controlGenerator.SetFieldLabel('" + control.id + "',this.value) onmouseup=dynamicForm.controlGenerator.SetFieldLabel('" + control.id + "',this.value)  onfocusout=dynamicForm.controlValidator.FieldLabelValidation(this) />";
        builder += "</div>";
        builder += "</div>";
        builder += "<input type='hidden' name='id' value=" + control.id + " >";
        builder += "<input type='hidden' name='entity_id' value=" + control.entity_id + " >";
        builder += "<input type='hidden' name='control_type' value='" + control.control_type + "' >";
        builder += "<input type='hidden' name='field_name' class='field_name_ddl'  />";
        builder += "<input type='hidden' name='control_id' value=" + control.id + " />";
        builder += "<input type='hidden' name='control_value_type' value='alphanumeric' />";
        return builder;
    }
    RenderFieldLabel = function (control) {

        var builder = "<div class='f-property-row'>";
        builder += "<label class=" + control.id + " name=" + control.id + ">Field Label</label>";
        builder += "<div class='form-fields'>";
        builder += "<input type='text' placeholder='" + control.placeholder_text + "' value='" + control.field_label + "' name='field_label' class='form-control' onkeyup=dynamicForm.controlGenerator.SetFieldLabel('" + control.id + "',this.value) onmouseup=dynamicForm.controlGenerator.SetFieldLabel('" + control.id + "',this.value) onfocusout=dynamicForm.controlValidator.FieldLabelValidation(this) />";
        builder += "</div>";
        builder += "</div>";
        builder += "<input type='hidden' name='id' value=" + control.id + " >";
        builder += "<input type='hidden' name='entity_id' value=" + control.entity_id + " >";
        builder += "<input type='hidden' name='control_type' value='" + control.control_type + "' >";
        builder += "<input type='hidden' name='field_name' class='field_name'  />";
        builder += "<input type='hidden' name='control_id' value=" + control.id + " />";
        return builder;
    }
    RenderRequiredCheckbox = function (control) {
        var isMandatory = control.is_mandatory;
        var builder = "<div class='d-flex'>";
        builder += "<label class='" + control.id + " checkBlock'>Is Mandatory?";
        if (isMandatory == false) {
            builder += "<input type='checkbox' name='is_mandatory' onclick=dynamicForm.htmlRenderer.CreateMandatoryIcon(" + control.id + ",this) />";
        }
        else {
            builder += "<input type='checkbox' checked   name='is_mandatory' onclick=dynamicForm.htmlRenderer.CreateMandatoryIcon(" + control.id + ",this) />";
            this.RenderMandatoryIcon(control.id, this);
        }
        builder += "<span class='checkmark'></span>";
        builder += "</label >";
        builder += "</div>";
        return builder;
    }
    RenderMandatoryIcon = function (fieldId, value) {
        var isChecked = true;
        if (isChecked) {
            $(dynamicForm.DE.divControlForm).find('span[data-attr=' + fieldId + ']').show();
            $(value).parents().children().find(dynamicForm.DE.minRange).val(1);
            $(value).parents().children().find(dynamicForm.DE.maxRange).val(10000);
        }
        else {
            $(dynamicForm.DE.divControlForm).find('span[data-attr=' + fieldId + ']').hide();
            $(value).parents().children().find(dynamicForm.DE.minRange).val('');
            $(value).parents().children().find(dynamicForm.DE.maxRange).val('');
        }
    }
    RenderTextBox = function (control) {
        var textValue = control.default_value;
        if (textValue == null) textValue = "";
        var placeHolderValue = control.placeholder_text;
        if (placeHolderValue == null) placeHolderValue = "";

        var builder = "<div class ='f-property dragable' id='" + control.id + "' data-attr='EXISTING' title='Drag to reorder.'>";
        builder += "<div class='fChild'  onclick=dynamicForm.controlGenerator.CreatePanel('" + control.id + "',this,'TEXT')>";
        //builder += "<span class='counter'>" + tempControlID + "</span>";
        builder += "<label class=" + control.id + "> " + control.field_label + "</label>";
        builder += "<span class='requiredIcon' data-attr=" + control.id + " style='display:none'> *</span>";
        builder += " <div class='form-fields'>";
        builder += "<input type='text' disabled name='default_value' placeholder='" + placeHolderValue + "' class='form-control' data-attr=" + control.id + "  value='" + textValue + "' />";
        builder += " </div>";
        builder += " </div>";
        builder += " <div class='a-r-icons'>";
        //builder += " <img src='../Content/images/add.svg' alt='add' onclick=dynamicForm.CreateControls('TEXTBOX') >";
        builder += "<img src='../Content/images/remove.svg' name='Delete' title='Delete' alt='delete' onclick=dynamicForm.controlInitializer.RemoveField('" + control.id + "',this)  >";
        builder += "</div>";
        builder += " </div> ";
        $(dynamicForm.DE.divControlForm).append(builder);
        dynamicForm.controlInitializer.DisableControls();

    }
    RenderDropDown = function (control) {
        var textValue = control.default_value;
        if (textValue == null) textValue = "";
        //var placeHolderValue = control.placeholder_text;
        //if (placeHolderValue == null) placeHolderValue = "";

        var builder = "<div class ='f-property dragable' id='" + control.id + "' data-attr='EXISTING' title='Drag to reorder.'>";
        builder += "<div class='fChild'  onclick=dynamicForm.controlGenerator.CreatePanel('" + control.id + "',this,'DROPDOWN')>";
        //builder += "<span class='counter'>" + tempControlID + "</span>";
        builder += "<label class=" + control.id + ">" + control.field_label + "</label>";
        builder += "<span class='requiredIcon' data-attr=" + control.id + " style='display:none'> *</span>";
        builder += " <div class='form-fields'>";
        builder += "<select disabled name='" + control.id + "' class='form-control' data-attr=" + control.id + "><option id='ddlPlaceHolder'>--Select--</option></select>";
        builder += " </div>";
        builder += " </div>";
        builder += " <div class='a-r-icons'>";
        //builder += " <img src='../Content/images/add.svg' alt='add' onclick=dynamicForm.CreateControls('TEXTBOX') >";
        builder += "<img src='../Content/images/remove.svg'  title='Delete' alt='delete' onclick=dynamicForm.controlInitializer.RemoveField('" + control.id + "',this)  >";
        builder += "</div>";
        builder += " </div> ";
        $(dynamicForm.DE.divControlForm).append(builder);
        dynamicForm.controlInitializer.DisableControls();

    }
    RenderDate = function (control) {
        var textValue = control.default_value;
        if (textValue == null) textValue = "";
        //var placeHolderValue = control.placeholder_text;
        //if (placeHolderValue == null) placeHolderValue = "";

        var builder = "<div class ='f-property dragable' id='" + control.id + "' data-attr='NEW' title='Click to edit. Drag to reorder.'>";
        builder += "<div class='fChild'  onclick=dynamicForm.controlGenerator.CreatePanel('" + control.id + "',this,'DATE')>";
        //builder += "<span class='counter'>" + tempControlID + "</span>";
        builder += "<label class=" + control.id + ">Field Name </label>";
        builder += "<span class='requiredIcon' data-attr=" + control.id + " style='display:none'> *</span>";
        builder += " <div class='form-fields'>";
        builder += "<input id='placefordate' type='text' disabled placeholder='' name='" + control.id + "' class='form-control' data-attr=" + control.id + " />";
        /*builder += "<img id='imgToDate' class='assign-calen' src='../Content/images/calendar.png'>"*/
        builder += " </div>";
        builder += " </div>";
        builder += " <div class='a-r-icons'>";
        //builder += " <img src='../Content/images/add.svg' alt='add' onclick=dynamicForm.CreateControls('TEXTBOX') >";
        builder += "<img src='../Content/images/remove.svg' alt='delete' onclick=dynamicForm.controlInitializer.RemoveField('" + control.id + "')  >";
        builder += "</div>";
        builder += " </div> ";
        $(dynamicForm.DE.divControlForm).append(builder);
        dynamicForm.controlInitializer.DisableControls();

    }
    RenderFieldType = function (control) {

        var builder = "<div class='f-property-row'>";
        builder += "<label>Field Type</label>";
        builder += "<div class='form-fields'>";
        builder += "<select class='form-control' data-attr=" + control.id + " name='control_value_type'  onchange='dynamicForm.controlEvents.OnControlValueChange(this)'>";
        //builder += "<option value='0'>Select</option>";
        if (control.control_value_type == "alphanumeric") {
            builder += "<option value='alphanumeric' selected>Alphanumeric</option>";
        }
        else {
            builder += "<option value='alphanumeric'>Alphanumeric</option>";
        }
        if (control.control_value_type == "character") {
            builder += "<option value='character' selected>Character</option>";
        }
        else {
            builder += "<option value='character'>Character</option>";
        }
        if (control.control_value_type == "integer") {
            builder += "<option value='integer' selected>Integer(Without Decimal)</option>";
        }
        else {
            builder += "<option value='integer'>Integer(Without Decimal)</option>";
        }
        if (control.control_value_type == "float") {
            builder += "<option value='float' selected>Float(With Decimal)</option>";
        }
        else {
            builder += "<option value='float'>Float(With Decimal)</option>";
        }
        builder += "</select>";
        builder += "</div>";
        builder += "</div>";
        return builder;
    }
    RenderRangeBox = function (control) {
        var builder = "<fieldset class='fieldBox f-property-row'>";
        builder += "<legend class='l-width' name='range'>Range</legend>";
        builder += "<div class='rangeBox'> ";
        builder += "<div class='d-flex flex-d-colum'> ";
        builder += "<label> Min</label> ";
        builder += "<input type = 'text'  name='min_length' class='minRange' value=" + control.min_length + "  data-attr=" + control.id + " class='form-control'  onfocusout=dynamicForm.controlValidator.RangeValidationMin(this,'" + control.id + "') /> ";
        builder += "</div> ";
        builder += "<div class='d-flex flex-d-colum' > ";
        builder += "<label> Max</label> ";
        builder += "<input type = 'text' name='max_length' class='maxRange' value=" + control.max_length + " data-attr=" + control.id + " class='form-control'  onfocusout=dynamicForm.controlValidator.RangeValidationMax(this,'" + control.id + "') /> ";
        builder += "</div> ";
        builder += "</div> ";
        builder += "</fieldset>";
        return builder;
    }
    RenderDecimalRoundNumber = function (control) {
        var round_off = control.round_off;
        if (round_off == null) round_off = 0;
        var builder = "<div class='f-property-row round_off'>";
        builder += "<label>Round off number after decimal</label>";
        builder += "<div class='form-fields' > ";
        builder += "<input type = 'text' data-attr=" + control.id + " name='round_off' value='" + round_off + "' class='form-control' onfocusout=dynamicForm.controlValidator.FloatValidation(this,'" + control.id + "') />";
        builder += "</div> ";
        builder += "</div> ";
        return builder;
    }
    RenderDefaultValuesText = function (control) {
        var defaultValue = control.default_value;
        if (defaultValue == null) defaultValue = "";
        var builder = "<div class='f-property-row'>";
        builder += "<label>Default Value</label>";
        builder += "<div class='form-fields' > ";
        builder += "<input type = 'text' data-attr=" + control.id + "  name='default_value' value='" + defaultValue + "' class='form-control'  onkeyup='dynamicForm.controlGenerator.SetDefaultLabel(" + control.id + ",this.value)' onmouseup='dynamicForm.controlGenerator.SetDefaultLabel(" + control.id + ",this.value)' />";
        builder += "</div> ";
        builder += "</div> ";
        return builder;
    }
    RenderDefaultValuesDropdown = function (control) {
        var defaultValue = control.default_value;
        if (defaultValue == null) defaultValue = "";
        var builder = "<div class='f-property-row'>";
        builder += "<label>Selected Value</label>";
        builder += "<div class='form-fields' > ";
        builder += "<input type ='text' readonly data-attr=" + control.id + "  name='default_value'  value='" + defaultValue + "' class='form-control p--event-none'  onkeyup='dynamicForm.controlGenerator.SetDefaultLabel(" + control.id + ",this.value)' onmouseup='dynamicForm.controlGenerator.SetDefaultLabel(" + control.id + ",this.value)'  />";
        builder += "</div> ";
        builder += "</div> ";
        return builder;
    }
    RenderPlaceHolderText = function (control) {
        var placeholder_text = control.placeholder_text;
        if (placeholder_text == null) placeholder_text = "";
        var builder = "<div class='f-property-row'>";
        builder += "<label>Placeholder Text</label>";
        builder += "<div class='form-fields'>";
        builder += "<input type='text' placeholder='" + placeholder_text + "'  name='placeholder_text' class='form-control'  value='" + placeholder_text + "'  onkeyup='dynamicForm.controlGenerator.SetPlaceHolderLabel(" + control.id + ",this.value)' onmouseup='dynamicForm.controlGenerator.SetPlaceHolderLabel(" + control.id + ",this.value)' />";
        builder += "</div>";
        builder += "</div>";
        return builder;
    }
    OnRenderComplete = function () {
        $(dynamicForm.DE.divControlSettings).find(dynamicForm.DE.roundOff).hide();
        //$(dynamicForm.DE.divControlForm).find(dynamicForm.DE.selectedField).find($("[name='default_value']")).attr('disabled', true);
    }
    RenderControls = function (control) {
        var builder = "";
        switch (control.control_type) {
            case dynamicForm.Controls.TEXT:
                builder = this.RenderTextBox(control);
                break;
            case dynamicForm.Controls.DROPDOWN:
                builder = this.RenderDropDown(control);
                break;
            case dynamicForm.Controls.DATE:
                builder = this.RenderDate(control);
                break;
            default:
        }
        $(dynamicForm.DE.divControlForm).append(builder);

    }
    RenderDropDownOption = function (control) {
        var builder = "<fieldset class='fieldBox f-property-row choices'>";
        builder += "<legend class='l-width' name='choice'>Choices</legend>";
        builder += "<div class='right--form_input'>";
        builder += "<input type = 'text' placeholder = 'Enter Options' class='form-control mr-01 txtCustomObject' autocomplete = 'off' onkeyup='dynamicForm.controlEvents.Toggle(this);'/>";
        builder += " <button id='btnAdd' type='button' title='Add' class='right--form_btn' onclick='dynamicForm.controlEvents.AddTextBox(this)'>";
        builder += "<img src='../../Areas/Admin/Content/images/plus.svg' style='width:14px'>";
        builder += "</button>";
        builder += " <button id='btnResetDefault' type='button' class='right--form_btn_reset' title='Reset Default Options' onclick='dynamicForm.controlEvents.ResetDefault(this)'>";
        builder += "<img src='../../Areas/Admin/Content/images/refresh.svg' style='width:17px'>";
        builder += "</button>";
        builder += "</div>";
        builder += this.RenderExistingOptions(control);
        builder += "</fieldset>";
        return builder;
    }
    RenderExistingOptions = function (control) {
        var builder = "<div class='TextBoxContainer'>";
        if (control.dynamicControlsDDLMasters != null) {
            for (var i = 0; i < control.dynamicControlsDDLMasters.length; i++) {
                builder += dynamicForm.controlGenerator.GetExistingDynamicTextBox(control.dynamicControlsDDLMasters[i]);
            }
        }
        builder += "</div>";
        return builder;
    }
}