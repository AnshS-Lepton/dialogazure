var expression_stack = [];
var expression = '';
var display_expression = '';
var tempExpressionToValidate = '';

function resetExpressionData() {
    expression_stack = [];
    expression = '';
    display_expression = '';
    tempExpressionToValidate = '';
    $('#dvExpression').html('');
}

function isValidExp() {
    try {
        let val = eval(tempExpressionToValidate);

        if (val === 0)
            return true;

        return val && val != 'Infinity';
    }
    catch {
        return false;
    }
}

function validateExpression() {
    if (!isValidExp()) {
        $("#btnSavebom").prop("disabled", true);
        alert('Invalid Expression!');
        return false;
    }
    $("#btnSavebom").prop("disabled", false);
    alert('Validated Successfully!');
}

function displayExpression(obj) {
    let htmlToAppend = '';

    $('#dvExpression').html('');

    expression_stack.forEach(function (obj) {
        if (obj.type == 'variable') {
            htmlToAppend = `<div class="chips addedChip"> ${obj.displayText} </div>`;
        }
        else {
            htmlToAppend = `&nbsp; ${obj.value} &nbsp;`
        }

        $('#dvExpression').append(htmlToAppend);
    })

}


function generateExpression() {

    expression = '';
    display_expression = '';
    tempExpressionToValidate = '';

    expression_stack.forEach(function (obj) {
        expression += obj.value;
        display_expression += obj.displayText;
        tempExpressionToValidate += obj.type == 'variable' ? 5.631452 : obj.value;
    })

    displayExpression();
    $("#btnSavebom").prop("disabled", true);
}

function pushToExpressionStack(value, type, displayText = '') {
     
    if (expression_stack && expression_stack.length) {
        let top = expression_stack[expression_stack.length - 1];

        // restrict consecutive types
        // Exceptions - 1. If numeric then replace, 2. If bracket then allow
        if (top.type === type) {
            if (value != '(' && value != ')' && top.value != ')') {
                if (type === 'numeric') {
                    expression_stack.pop();
                }
                else {
                    alert(`Cannot add consecutive ${type}s in the expression!`, 'Error');
                    return;
                }
            }
        }
        // there should be an operator between a variable and a numeric value
        else if ((top.type == 'variable' && type == 'numeric') || (type == 'variable' && top.type == 'numeric')) {
            alert(`There should be an operator between a variable and a numeric value!`, 'Error');
            return;
        }
    }

    // restrict operator at beginning, allow opening bracket
    if (expression_stack.length === 0 && type === 'operator') {
        if (value == ')') {
            alert(`Expression cannot begin with a closing bracket!`, 'Error');
            return;
        }
        if (value != '(') {
            alert(`Expression cannot begin with an operator!`, 'Error');
            return;
        }
    }


    expression_stack.push(
        {
            'displayText': displayText, 'value': value, 'type': type
        });

    generateExpression();
}

function handleBackspace() {
    if (expression_stack.length) {
        let lastElement = expression_stack.at(-1);
        if (lastElement.type == 'numeric' && lastElement.value.length > 1) {
            let slicedValue = lastElement.value.slice(0, -1);
            expression_stack[expression_stack.length - 1]['value'] = slicedValue;
            expression_stack[expression_stack.length - 1].displayText = slicedValue;
        }
        else {
            expression_stack.pop();
        }

        generateExpression();
    }
}

function exp_length_limit(element) {
    var max_chars = 5;

    if (element.value.length > max_chars) {
        element.value = element.value.substr(0, max_chars);
    }
}

function closebom() {
    $("#closeModalPopup").trigger('click');
}
function clearbom() {
    resetExpressionData();

    $("#btnSavebom").prop("disabled", true);
}
