class HTMLRenderer {
    CreateFieldLabel = function (fieldId) {
        var builder = "<div class='f-property-row'>";
        builder += "<label class=" + fieldId + ">Field Label</label>";
        builder += "<div class='form-fields'>";
        builder += "<input type='text' placeholder='' name=field_label value='' class='form-control' onkeyup=dynamicForm.controlGenerator.SetFieldLabel('" + fieldId + "',this.value) onmouseup=dynamicForm.controlGenerator.SetFieldLabel('" + fieldId + "',this.value) onfocusout=dynamicForm.controlValidator.FieldLabelValidation(this) />";
        builder += "</div>";
        builder += "</div>";
        builder += "<input type='hidden' name='id' value=" + fieldId + " >";
        builder += "<input type='hidden' name='entity_id' value=" + parseInt($(dynamicForm.DE.LayerName).val()) + " >";
        builder += "<input type='hidden' name='control_type' value='TEXT' >";
        builder += "<input type='hidden' name='field_name' class='field_name' />";
        builder += "<input type='hidden' name='control_id' value=" + fieldId + " />";
        return builder;
    }
    CreateFieldLabelForDropDown = function (fieldId) {
        var builder = "<div class='f-property-row'>";
        builder += "<label class=" + fieldId + ">Field Label</label>";
        builder += "<div class='form-fields'>";
        builder += "<input type='text' name='field_label' value='' class='form-control' onkeyup=dynamicForm.controlGenerator.SetFieldLabel('" + fieldId + "',this.value) onmouseup=dynamicForm.controlGenerator.SetFieldLabel('" + fieldId + "',this.value) onfocusout=dynamicForm.controlValidator.FieldLabelValidation(this) />";
        builder += "</div>";
        builder += "</div>";
        builder += "<input type='hidden' name='id' value=" + fieldId + " >";
        builder += "<input type='hidden' name='entity_id' value=" + parseInt($(dynamicForm.DE.LayerName).val()) + " >";
        builder += "<input type='hidden' name='control_type' value='DROPDOWN' >";
        builder += "<input type='hidden' class='field_name_ddl' name='field_name' />";
        builder += "<input type='hidden' name='control_id' value=" + fieldId + " />";
        builder += "<input type='hidden' name='control_value_type' value='alphanumeric' />";
        return builder;
    }
    CreateRequiredCheckbox = function (fieldId) {
        var builder = "<div class='d-flex'>";
        builder += "<label class='" + fieldId + " checkBlock'>Is Mandatory?";
        builder += "<input type='checkbox' placeholder='' name='is_mandatory' onclick=dynamicForm.htmlRenderer.CreateMandatoryIcon('" + fieldId + "',this) />";
        builder += "<span class='checkmark'></span>";
        builder += "</label >";
        builder += "</div>";
        return builder;
    }
    CreateTextBox = function (uniqueID) {
        var builder = "<div class ='f-property dragable selectField2'  id='" + uniqueID + "' data-attr='NEW' title='Drag to reorder.'>";
        builder += "<div class='fChild'  onclick=dynamicForm.controlGenerator.CreatePanel('" + uniqueID + "',this,'TEXT')>";
        //builder += "<span class='counter'>" + tempControlID + "</span>";
        builder += "<label class=" + uniqueID + ">Field Name </label>";
        builder += "<span class='requiredIcon' data-attr=" + uniqueID + " style='display:none'> *</span>";
        builder += " <div class='form-fields'>";
        builder += "<input type='text' placeholder='' disabled name='" + uniqueID + "' class='form-control' data-attr=" + uniqueID + " />";
        builder += " </div>";
        builder += " </div>";
        builder += " <div class='a-r-icons'>";
        //builder += " <img src='../Content/images/add.svg' alt='add' onclick=dynamicForm.CreateControls('TEXT') >";
        builder += "<img src='../Content/images/remove.svg'  title='Delete' alt='delete' onclick=dynamicForm.controlInitializer.RemoveField('" + uniqueID + "',this)  >";
        builder += "</div>";
        builder += " </div> ";
        $(dynamicForm.DE.divControlForm).append(builder);
        dynamicForm.controlInitializer.DisableControls();
        var htmlObject = $(builder);
        dynamicForm.controlGenerator.CreatePanel(uniqueID, htmlObject[0].firstElementChild, 'TEXT')

    }
    CreateDecimalRoundNumber = function (control) {
        var round_off = control.round_off;
        if (round_off == null) round_off = 0;
        var builder = "<div class='f-property-row round_off' style='display:none';>";
        builder += "<label>Round off number after decimal</label>";
        builder += "<div class='form-fields' > ";
        builder += "<input type = 'text' data-attr=" + control + " name='round_off' value='" + round_off + "' class='form-control' onfocusout=dynamicForm.controlValidator.FloatValidation(this,'" + control + "') /> ";
        builder += "</div> ";
        builder += "</div> ";
        return builder;
    }
    CreateMandatoryIcon = function (fieldId, value) {
        var isChecked = $(value).is(":checked");

        if (isChecked) {
            $(dynamicForm.DE.divControlForm).find('span[data-attr=' + fieldId + ']').show();
            $(value).parents().children().find(dynamicForm.DE.minRange).val(1);
            $(value).parents().children().find(dynamicForm.DE.maxRange).val(10000);
        }
        else {
            $(dynamicForm.DE.divControlForm).find('span[data-attr=' + fieldId + ']').hide();
            $(value).parents().children().find(dynamicForm.DE.minRange).val(0);
            $(value).parents().children().find(dynamicForm.DE.maxRange).val(10000);
        }
        $(value).parents(dynamicForm.DE.form_row_vert).find($("[name='default_value']")).val('');
    }
    CreateFieldType = function (fieldId) {
        var builder = "<div class='f-property-row'>";
        builder += "<label>Field Type</label>";
        builder += "<div class='form-fields'>";
        builder += "<select class='form-control' data-attr=" + fieldId + " name='control_value_type' onchange='dynamicForm.controlEvents.OnControlValueChange(this)' >";
        //builder += "<option value='0'>Select</option>";
        builder += "<option value='alphanumeric'>Alphanumeric</option>";
        builder += "<option value='character'>Character</option>";
        builder += "<option value='integer'>Integer(Without Decimal)</option>";
        builder += "<option value='float'>Float(With Decimal)</option>";
        builder += "</select>";
        builder += "</div>";
        builder += "</div>";
        return builder;
    }
    CreateRangeBox = function (fieldId) {
        var builder = "<fieldset class='fieldBox f-property-row'>";
        builder += "<legend class='l-width' name='range'>Character Length</legend>";
        builder += "<div class='rangeBox'> ";
        builder += "<div class='d-flex flex-d-colum'> ";
        builder += "<label> Min</label> ";
        builder += "<input type = 'text' class='minRange' placeholder = '' name='min_length'  data-attr=" + fieldId + " value='0' class='form-control' onfocusout=dynamicForm.controlValidator.RangeValidationMin(this,'" + fieldId + "') /> ";
        builder += "</div> ";
        builder += "<div class='d-flex flex-d-colum' > ";
        builder += "<label> Max</label> ";
        builder += "<input type = 'text' class='maxRange' placeholder = ''  name='max_length'  data-attr=" + fieldId + " value='10000' class='form-control' onfocusout=dynamicForm.controlValidator.RangeValidationMax(this,'" + fieldId + "') /> ";
        builder += "</div> ";
        builder += "</div> ";
        builder += "</fieldset>";
        return builder;
    }
    DefaultValues = function (fieldId) {
        var builder = "<div class='f-property-row'>";
        builder += "<label>Default Value</label>";
        builder += "<div class='form-fields' > ";
        builder += "<input type = 'text' placeholder = '' class='form-control'  name='default_value' onkeyup=dynamicForm.controlGenerator.SetDefaultLabel('" + fieldId + "',this.value) onmouseup=dynamicForm.controlGenerator.SetDefaultLabel('" + fieldId + "',this.value) />";
        builder += "</div> ";
        builder += "</div> ";
        return builder;
    }
    PlaceHolderText = function (fieldId) {
        var builder = "<div class='f-property-row'>";
        builder += "<label>Placeholder Text</label>";
        builder += "<div class='form-fields'>";
        builder += "<input type='text' placeholder='' class='form-control' name='placeholder_text' onkeyup=dynamicForm.controlGenerator.SetPlaceHolderLabel('" + fieldId + "',this.value) onmouseup=dynamicForm.controlGenerator.SetPlaceHolderLabel('" + fieldId + "',this.value) />";
        builder += "</div>";
        builder += "</div>";
        return builder;
    }
    CreateDate = function (uniqueID) {
        var builder = "<div class ='f-property dragable' id='" + uniqueID + "' data-attr='NEW' title='Drag to reorder.'>";
        builder += "<div class='fChild'  onclick=dynamicForm.controlGenerator.CreatePanel('" + uniqueID + "',this,'DATE')>";
        //builder += "<span class='counter'>" + tempControlID + "</span>";
        builder += "<label class=" + uniqueID + ">Field Name </label>";
        builder += "<span class='requiredIcon' data-attr=" + uniqueID + " style='display:none'> *</span>";
        builder += " <div class='form-fields'>";
        builder += "<input id='placefordate' type='text' disabled placeholder='' name='" + uniqueID + "' class='form-control' data-attr=" + uniqueID + " />";
        /*builder += "<img id='imgToDate' class='assign-calen' src='../Content/images/calendar.png'>"*/
        builder += " </div>";
        builder += " </div>";
        builder += " <div class='a-r-icons'>";
        //builder += " <img src='../Content/images/add.svg' alt='add' onclick=dynamicForm.CreateControls('TEXTBOX') >";
        builder += "<img src='../Content/images/remove.svg'  title='Delete' alt='delete' onclick=dynamicForm.controlInitializer.RemoveField('" + uniqueID + "')  >";
        builder += "</div>";
        builder += " </div> ";
        $(dynamicForm.DE.divControlForm).append(builder);
        dynamicForm.controlInitializer.DisableControls();
        /*setDateTimeCalendar('placefordate', 'imgToDate');*/

    }
    CreateDropdown = function (uniqueID) {
        var builder = "<div class ='f-property dragable selectField2' id='" + uniqueID + "' data-attr='NEW' title='Drag to reorder.'>";
        builder += "<div class='fChild'  onclick=dynamicForm.controlGenerator.CreatePanel('" + uniqueID + "',this,'DROPDOWN')>";
        //builder += "<span class='counter'>" + tempControlID + "</span>";
        builder += "<label class=" + uniqueID + ">Select a choice</label>";
        builder += "<span class='requiredIcon' data-attr=" + uniqueID + " style='display:none'> *</span>";
        builder += " <div class='form-fields'>";
        builder += "<select disabled name='" + uniqueID + "' class='form-control' data-attr=" + uniqueID + "><option>--Select--</option></select>";
        builder += " </div>";
        builder += " </div>";
        builder += " <div class='a-r-icons'>";
        //builder += " <img src='../Content/images/add.svg' alt='add' onclick=dynamicForm.CreateControls('TEXTBOX') >";
        builder += "<img src='../Content/images/remove.svg'  title='Delete' alt='delete' onclick=dynamicForm.controlInitializer.RemoveField('" + uniqueID + "')  >";
        builder += "</div>";
        builder += " </div> ";
        $(dynamicForm.DE.divControlForm).append(builder);
        dynamicForm.controlInitializer.DisableControls();
        var htmlObject = $(builder);
        dynamicForm.controlGenerator.CreatePanel(uniqueID, htmlObject[0].firstElementChild, 'DROPDOWN')
    }
    CreateDropDownOption = function (fieldId) {
         
        var builder = "<fieldset class='fieldBox f-property-row choices'>";
        builder += "<legend class='l-width' name='choice'>Choices</legend>";
        builder += "<div class='right--form_input'>";
        builder += "<input type = 'text'  placeholder = 'Enter Options' class='form-control mr-01 txtCustomObject' autocomplete = 'off' onkeyup='dynamicForm.controlEvents.Toggle(this);' />";
        builder += " <button id='btnAdd' type='button' title='Add' class='right--form_btn' onclick='dynamicForm.controlEvents.AddTextBox(this)'>";
       /* builder += "<img src='~/../../Areas/Admin/Content/images/plus.svg' style='width:14px'>";*/
        builder += "<img src='" + appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/plus.svg' + "' style='width:14px'>";
        builder += "</button>";
        builder += " <button id='btnResetDefault' type='button' title='Reset Default Options' class='right--form_btn_reset' onclick='dynamicForm.controlEvents.ResetDefault(this)'>";
        /*builder += "<img src='~/../../Areas/Admin/Content/images/refresh.svg' style='width:17px'>";*/
        builder += "<img src='" + appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/refresh.svg' + "' style='width:17px'>";
        builder += "</button>";
        builder += "</div>";
        builder += "<div class='TextBoxContainer'>";
        builder += "</div>";
        builder += "</fieldset>";
        return builder;
    }
}