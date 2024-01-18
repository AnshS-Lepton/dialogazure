class ControlEvents {
    OnEntityChange = function (entity) {
        var isAnyUnsavedEntityExists = this.ChangeEntity(entity);
        if (isAnyUnsavedEntityExists) {
            confirm("Some data might be lost. Are you sure to change the entity?", function () {
                dynamicForm.controlEvents.ResetPage(entity);
                return true;
            }, function () {
                $(entity).val(dynamicForm.ID);
                $('.chosen-select').trigger("chosen:updated");
                return false;
            });
        }
        else {
            this.ResetPage(entity);
        }
    }
    ResetPage = function (entity) {
        var entityId = parseInt($(entity).val());
        dynamicForm.ID = entityId;
        var entityName = $('#LayerName option:selected').text()
        $(dynamicForm.DE.tabs).hide();
        $(dynamicForm.DE.tabs).show(500);
        $(dynamicForm.DE.divControlForm).empty();
        $(dynamicForm.DE.divControlSettings).empty();
        dynamicForm.controlGenerator.GetExistingFields(entityId, entityName,true);
        dynamicForm.controlInitializer.DisableControls();
    }
    OnControlValueChange = function (controlType) {
        var control = $(controlType).val();
        switch (control) {
            case dynamicForm.DataType.INTEGER:
                $(dynamicForm.DE.divControlSettings).find(dynamicForm.DE.roundOff).hide(500);
                $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='range']")).text(dynamicForm.controlValueType.Range);
                break;
            case dynamicForm.DataType.FLOAT:
                $(dynamicForm.DE.divControlSettings).find(dynamicForm.DE.roundOff).show(500);
                $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='range']")).text(dynamicForm.controlValueType.Range);
                break;
            case dynamicForm.DataType.ALPHANUMERIC:
                $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='range']")).text(dynamicForm.controlValueType.Character_Length);
                $(dynamicForm.DE.divControlSettings).find(dynamicForm.DE.roundOff).hide(500);
                break;
            case dynamicForm.DataType.CHARACTER:
                $(dynamicForm.DE.divControlSettings).find(dynamicForm.DE.roundOff).hide(500);
                $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='range']")).text(dynamicForm.controlValueType.Character_Length);
                break;
        }
        this.RangeSet(controlType);
        this.ClearDefaultValue(controlType);
        this.ClearPlaceHolder(controlType);

    }
    AddTextBox = function (button) {
        var item = [];
        $(button).parents(dynamicForm.DE.choices).children().find($("[name='DynamicTextBox']")).each(function () { item.push(this.value) });
        let optionValue = $(button).parents(dynamicForm.DE.choices).find(dynamicForm.DE.txtCustomObject).val();
        if (!item.includes(optionValue)) {
            if (optionValue != '') {
                var builder = dynamicForm.controlGenerator.GetDynamicTextBox(optionValue);
                $(button).parents(dynamicForm.DE.choices).find(dynamicForm.DE.TextBoxContainer).append(builder);
                $(button).parents(dynamicForm.DE.choices).find(dynamicForm.DE.txtCustomObject).val(dynamicForm.Empty);
            }
            else {
                $(button).parents(dynamicForm.DE.choices).find(dynamicForm.DE.txtCustomObject).addClass(dynamicForm.DE.input_validation_error);
                new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_ENTER_SOME_VALUE_IN_TEXTBOX }).show();
                return;
            }
        }
        else {
            $(button).parents(dynamicForm.DE.choices).find(dynamicForm.DE.txtCustomObject).addClass(dynamicForm.DE.input_validation_error);
            new Noty({ type: 'error', text: dynamicForm.Messages.VALUE_ALREADY_EXISTS }).show();
            return;
        }
    }
    RemoveTextBox = function (button) {
        confirm("Are you sure to delete?", function () {
            var defaultval = $(button).parents(dynamicForm.DE.form_row_vert).find($("[name='default_value']")).val();
            if ($(button).parent().find($("[name='DynamicTextBox']")).val() == defaultval)
                $(button).parents(dynamicForm.DE.form_row_vert).find($("[name='default_value']")).val('');
            $(button).parent().remove();
        });
    }
    SetDefaultValue = function (button) {
        var SelectedValue = $(button).parents(dynamicForm.DE.right_form_input).find($("[name='DynamicTextBox']")).val();
        $(button).parents(dynamicForm.DE.form_row_vert).find($("[name='default_value']")).val(SelectedValue);
    }
    EmptyDefault = function (range) {
        $(range).parents(dynamicForm.DE.form_row_vert).find($("[name='default_value']")).val(dynamicForm.Empty);
    }
    RangeSet = function (controlType) {
        if ($(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='is_mandatory']")).prop('checked') == false) {
            $(controlType).parents().children().find(dynamicForm.DE.minRange).val(dynamicForm.MinimumRange);
            $(controlType).parents().children().find(dynamicForm.DE.maxRange).val(dynamicForm.MaximumRange);
        }
    }
    ClearPlaceHolder = function (controlType) {
        $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='placeholder_text']")).val(dynamicForm.Empty);
    }
    ClearDefaultValue = function (controlType) {
        $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='default_value']")).val(dynamicForm.Empty);
    }
    Toggle = function (button) {
        $(button).removeClass(dynamicForm.DE.input_validation_error);
    }
    ResetDefault = function (button) {
        if ($(button).parents(dynamicForm.DE.choices).children().find($("[name='Default']:checked"))) {
            $(button).parents(dynamicForm.DE.choices).children().find($("[name='Default']:checked")).attr('checked', false);
            $(button).parents(dynamicForm.DE.form_row_vert).find($("[name='default_value']")).val('');
        }
    }
    ChangeEntity = function () {
        if ($(dynamicForm.DE.divControlForm).find("[data-attr='NEW']").length > 0) {
            return true;
        }
        return false;
    }
}