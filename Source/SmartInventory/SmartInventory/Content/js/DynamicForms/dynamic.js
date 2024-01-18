var DynamicFormsCreator = function () {
    var dynamicForm = this;
    var SELECTED_CONTROL;
    var existingFields = [];
    var _fieldVal;
    var _buttonVal;
    this.DE = {
        "btnCreateTextBox": "#btnCreateTextBox",
        "btnCreateDropdown": "#btnCreateDropdown",
        "btnCreateDate": "#btnCreateDate",
        "btnSave": "#btnSave",
        "divControlForm": "#divControlForm",
        "divControlSettings": "#divControlSettings",
        "tabs": ".tabs",
        "divExistingControls": "#divExistingControls",
        "lblMessage": "#lblMessage",
        "LayerName": "#LayerName",
        "selectedField": ".selectedField",
        "form_row_vert": ".form-row--vert",
        "right_form_input": ".right--form_input",
        "btnAdd": "#btnAdd",
        "minRange": ".minRange",
        "maxRange": ".maxRange",
        "roundOff": ".round_off",
        "field_name": ".field_name",
        "field_name_ddl": ".field_name_ddl",
        "txtCustomObject": ".txtCustomObject",
        'choices': '.choices',
        "TextBoxContainer": ".TextBoxContainer",
        "pattern": "pattern",
        "minRangePattern": "minRangePattern",
        "maxRangePattern": "maxRangePattern",
        "controlvaluetype": "controlvaluetype",
        "input_validation_error": "input-validation-error",
        "rangeBox": ".rangeBox",
        "deleteControlName": "#deleteControlName",
        "btnDeleteControl":"#btnDeleteControl"
    }
    this.Controls = {
        "TEXT": "TEXT",
        "DROPDOWN": "DROPDOWN",
        "DATE": "DATE",
        "selectedField": "selectedField"
    }
    this.Operation = {
        "SAVE": "Save",
        "UPDATE": "Update",
        "DELETE": "Delete",
        "INSERT": "Insert",
        "CLEAR": "Clear"
    }
    this.DataType = {
        "INTEGER": "integer",
        "FLOAT": "float",
        "ALPHANUMERIC": "alphanumeric",
        "CHARACTER": "character"
    }
    this.Messages = {
        "PLEASE_FILL_VALID_DETAILS": "Please fill valid details.",
        "MIN_RANGE_SHOULD_BE_LESS_THAN_MAX_RANGE": "Min Range should be less than Max Range.",
        "PLEASE_ENTER_SOME_VALUE_IN_TEXTBOX": "Please enter some value in textbox.",
        "VALUE_ALREADY_EXISTS": "Value already exists.",
        "FIELD_LABEL_CANT_BE_EMPTY": "Field Label can not be empty.",
        "FIELD_NAME_ALREADY_EXISTS": "Field Name Already Exists.",
        "FIELD_DELETED_SUCCESSFULLY": "Field Deleted Successfully.",
        "PLEASE_ENTER_ANY_FIELD_IN_DROPDOWN": "Please enter any field in dropdown.",
        "FIRST_CHARACTER_OF_FIELD_LABEL_MUST_START_WITH_ALPHABET": "First character of Field Label must start with alphabet.",
        "PLEASE_ENTER_CONTROL_NAME":"Please enter the control name!"
    }
    this.controlValueType={
        "Range": "Range",
        "Character_Length": "Character Length"
    }
    this.ID = 0;
    this.MinimumRange = 0;
    this.MaximumRange = 10000;
    this.Empty = '';
    this.KeyString = "abcdefghijklmnopqrstuvwxyz";
    this.initApp = function () {
        this.htmlRenderer = new HTMLRenderer();
        this.controlGenerator = new ControlGenerator();
        this.controlEvents = new ControlEvents();
        this.controlInitializer = new ControlInitializer();
        this.renderExistingControls = new RenderExistingControls();
        this.controlValidator = new ControlValidator();
    }
}

