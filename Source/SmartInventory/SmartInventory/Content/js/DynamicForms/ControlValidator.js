class ControlValidator {
    ValidatePattern = function (txt, pattern) {
        let retvalue = true;
        if (txt.search(pattern) == 0)
            retvalue = true;
        else
            retvalue = false;
        return retvalue;
    }
    ResourceRangeValidation = function (inputString, minRangePattern, maxRangePattern, pattern) {
        let retvalue = true;
        if (this.ValidatePattern(inputString, pattern)) {
            inputString = parseFloat(inputString);
            if (!Number.isNaN(inputString)) {
                if (inputString >= minRangePattern && inputString <= maxRangePattern) {
                    retvalue = true;
                } else {
                    retvalue = false;
                }
            }
        }
        else {
            if (inputString.search(pattern) == 0)
                retvalue = true;
            else
                retvalue = false;
        }
        return retvalue;
    }
    DynamicPatternValidator = function (fieldId) {
        var controlType = $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='default_value']"));
        dynamicForm.DE.minRangePattern = $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='min_length']")).val();
        dynamicForm.DE.maxRangePattern = $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='max_length']")).val();
        dynamicForm.DE.controlvaluetype = $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='control_value_type']")).val();
        var round = $(controlType).parents(dynamicForm.DE.form_row_vert).find($("[name='round_off']")).val();
        switch (dynamicForm.DE.controlvaluetype) {
            case dynamicForm.DataType.INTEGER:
                dynamicForm.DE.pattern = new RegExp(`^[0-9]*$`);
                break;
            case dynamicForm.DataType.FLOAT:
                dynamicForm.DE.pattern = RegExp(`^[+]?([0-9]{0,})*[.]?([0-9]{0,${round}})?$`, 'g');

                break;
            case dynamicForm.DataType.ALPHANUMERIC:
                dynamicForm.DE.pattern = new RegExp(`[^'"]{${dynamicForm.DE.minRangePattern},${dynamicForm.DE.maxRangePattern}}$`);
                break;
            case dynamicForm.DataType.CHARACTER:
                dynamicForm.DE.pattern = new RegExp(`[^'"0-9]{${dynamicForm.DE.minRangePattern},${dynamicForm.DE.maxRangePattern}}$`);
                break;
        }
    }
    RangeValidationMin = function (rangeElement, fieldId) {
        dynamicForm.DE.pattern = new RegExp(`^[0-9]{0,}$`);
        if ($(rangeElement).val().search(dynamicForm.DE.pattern) == 0) {
            $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='min_length']")).removeClass(dynamicForm.DE.input_validation_error);
            this.MinSmallerThanMaxRange(rangeElement, fieldId);
        }
        else {
            $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='min_length']")).addClass(dynamicForm.DE.input_validation_error);
            return false;
        }
        dynamicForm.controlEvents.EmptyDefault(rangeElement);
        return true;
    }
    FloatValidation = function (Element, fieldId) {
        dynamicForm.DE.pattern = new RegExp(`^[0-9]{0,}$`);
        if ($(Element).val().search(dynamicForm.DE.pattern) == 0) {
            $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='round_off']")).removeClass(dynamicForm.DE.input_validation_error);
            return true;
        }
        else {
            $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='round_off']")).addClass(dynamicForm.DE.input_validation_error);
            return false;
        }
        dynamicForm.controlEvents.EmptyDefault(rangeElement);
    }
    RangeValidationMax = function (rangeElement, fieldId) {
        dynamicForm.DE.pattern = new RegExp(`^[0-9]{0,}$`);
        if ($(rangeElement).val().search(dynamicForm.DE.pattern) == 0) {
            $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='max_length']")).removeClass(dynamicForm.DE.input_validation_error);
            this.MaxGreaterThanMinRange(rangeElement, fieldId);
        }
        else {
            $(dynamicForm.DE.divControlSettings).find('#' + fieldId).find($("[name='max_length']")).addClass(dynamicForm.DE.input_validation_error);
            return false;
        }
        dynamicForm.controlEvents.EmptyDefault(rangeElement);
        return true;
    }
    MinSmallerThanMaxRange = function (rangeElement,fieldId) {
        var maxRange = $(rangeElement).parents().find('#' + fieldId).find(dynamicForm.DE.rangeBox).find($("[name='max_length']")).val();
        var minRange = $(rangeElement).val();
        if (parseInt(maxRange) < parseInt(minRange)) {
            $(rangeElement).addClass(dynamicForm.DE.input_validation_error);
            return false;
        }
        else {
            $(rangeElement).removeClass(dynamicForm.DE.input_validation_error);
            $(rangeElement).parents().find(dynamicForm.DE.rangeBox).find($("[name='max_length']")).removeClass(dynamicForm.DE.input_validation_error);
            return true;
        }
    }
    MaxGreaterThanMinRange = function (rangeElement,fieldId) {
        var minRange = $(rangeElement).parents().find('#' + fieldId).find(dynamicForm.DE.rangeBox).find($("[name='min_length']")).val();
        var maxRange = $(rangeElement).val();
        if (parseInt(maxRange) < parseInt(minRange)) {
            $(rangeElement).addClass(dynamicForm.DE.input_validation_error);
            return false;
        }
        else {
            $(rangeElement).removeClass(dynamicForm.DE.input_validation_error);
            $(rangeElement).parents().find(dynamicForm.DE.rangeBox).find($("[name='min_length']")).removeClass(dynamicForm.DE.input_validation_error);
            return true;
        }

    }
    FieldLabelValidation = function (field) {
        dynamicForm.DE.pattern = new RegExp(`^[a-zA-Z0-9_ ]*$`);
        var firstCharPattern = new RegExp(`^[a-zA-Z]*$`)
        let firstChar = $(field).val().charAt(0);
        if (firstChar.search(firstCharPattern) != 0) {
            $(field).addClass(dynamicForm.DE.input_validation_error);
            new Noty({ type: 'error', text: dynamicForm.Messages.FIRST_CHARACTER_OF_FIELD_LABEL_MUST_START_WITH_ALPHABET }).show();
            return false;
        }
        if ($(field).val().search(dynamicForm.DE.pattern) == 0) {
            $(field).removeClass(dynamicForm.DE.input_validation_error);
            return true;
        }
        else {
            $(field).addClass(dynamicForm.DE.input_validation_error);
            return false;
        }
    }
    ValidateForm = function (data) {
        var isvalid;
        if (data.control_type == 'TEXT') {
            isvalid = this.ValidateTextControl(data);
            return isvalid;
        } else {
            isvalid = this.ValidateDropDownControl(data);
            return isvalid;
        }
    }
    ValidateTextControl = function (data) {
        var minLength = $(dynamicForm.DE.divControlSettings).find('#' + data.id).find($("[name='min_length']"));
        var maxLength = $(dynamicForm.DE.divControlSettings).find('#' + data.id).find($("[name='max_length']"));
        var float = $(dynamicForm.DE.divControlSettings).find('#' + data.id).find($("[name='round_off']"));
        var field = $(dynamicForm.DE.divControlSettings).find('#' + data.id).find($("[name='field_label']"));
        var isValid = this.RangeValidationMin(minLength, data.id);
        if (!isValid) {
            new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_FILL_VALID_DETAILS }).show();
            return false;
        }

        isValid = this.RangeValidationMax(maxLength, data.id);

        if (!isValid) {
            new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_FILL_VALID_DETAILS }).show();
            return false;
        }

        isValid = this.MinSmallerThanMaxRange(minLength,data.id);
        if (!isValid) {
            new Noty({ type: 'error', text: dynamicForm.Messages.MIN_RANGE_SHOULD_BE_LESS_THAN_MAX_RANGE }).show();
            return false;
        }
        isValid = this.MaxGreaterThanMinRange(maxLength,data.id);
        if (!isValid) {
            new Noty({ type: 'error', text: dynamicForm.Messages.MIN_RANGE_SHOULD_BE_LESS_THAN_MAX_RANGE }).show();
            return false;
        }
        isValid = this.FloatValidation(float, data.id);
        if (!isValid) {
            new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_FILL_VALID_DETAILS }).show();
            return false;
        }
        isValid = this.FieldLabelValidation(field);
        if (!isValid) {
            new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_FILL_VALID_DETAILS }).show();
            return false;
        }

        //To save the default value of mandatory field uncomment code related to is_invalid variable
       // var is_invalid = ($(dynamicForm.DE.divControlSettings).find($("[name='default_value']")).hasClass(dynamicForm.DE.input_validation_error));
        var fieldLabel = ($(dynamicForm.DE.divControlSettings).find($("[name='field_label']")).hasClass(dynamicForm.DE.input_validation_error));
        if (/*is_invalid == true ||*/ fieldLabel == true) {
            new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_FILL_VALID_DETAILS }).show();
            return false;
        }
        return true;
    }
    ValidateDropDownControl = function (data) {
        var is_invalid = ($(dynamicForm.DE.divControlSettings).find($("[name='default_value']")).hasClass(dynamicForm.DE.input_validation_error));
        var fieldLabel = ($(dynamicForm.DE.divControlSettings).find($("[name='field_label']")).hasClass(dynamicForm.DE.input_validation_error));
        var txtCustomObject = ($(dynamicForm.DE.divControlSettings).find('#' + data.id).find($(".txtCustomObject")).hasClass(dynamicForm.DE.input_validation_error));
        //var DynamicTextBox = $(dynamicForm.DE.divControlSettings).find('.TextBoxContainer').find($("[name='DynamicTextBox']")).length;
        var DynamicTextBox = $(dynamicForm.DE.divControlSettings).find('#' + data.id).find('.TextBoxContainer').find($("[name='DynamicTextBox']")).length;
        var field = $(dynamicForm.DE.divControlSettings).find('#' + data.id).find($("[name='field_label']"));
        var isValid = this.FieldLabelValidation(field);
        if (!isValid) {
            new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_FILL_VALID_DETAILS }).show();
            return false;
        }
        //To save the default value of mandatory field uncomment code related to is_invalid variable
        if (/*is_invalid == true ||*/ fieldLabel == true || txtCustomObject == true) {
            new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_FILL_VALID_DETAILS }).show();
            return false;
        }
        if (DynamicTextBox < 1) {
            new Noty({ type: 'error', text: dynamicForm.Messages.PLEASE_ENTER_ANY_FIELD_IN_DROPDOWN }).show();
            $(dynamicForm.DE.divControlSettings).find('.txtCustomObject').addClass('input-validation-error');
            return false;
        }
        else {
            $(dynamicForm.DE.divControlSettings).find('.txtCustomObject').removeClass('input-validation-error');
        }
            return true;
    }

    ValidateForDelete = function () {
        let _fieldLabel = $("#" + dynamicForm._fieldVal).find('label').text().trim();
        let _txtfieldLabel = $(dynamicForm.DE.deleteControlName).val();
        if (_fieldLabel.toUpperCase() == _txtfieldLabel.toUpperCase()) {
            if ($(dynamicForm.DE.btnDeleteControl).hasClass('btn--box_ok_disable')) {
                document.getElementById("btnDeleteControl").classList.remove('btn--box_ok_disable');
                document.getElementById("btnDeleteControl").classList.add('btn--box_ok');
            }
            $(dynamicForm.DE.btnDeleteControl).attr('disabled',false);
        }
        else {
            $(dynamicForm.DE.btnDeleteControl).attr('disabled',true);
        }
    }
}