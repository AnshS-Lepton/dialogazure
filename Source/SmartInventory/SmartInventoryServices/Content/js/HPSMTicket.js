var fileList = [];
$(document).ready(function () {
    $('.chosen-select').chosen({ width: '100%' });
    $('#txtDescription').keyup(function () {
         
        var charLen = this.value.length;
        $('#charLength').html(charLen);
    })
});
function validateDropdown(elementId) {
    $("#" + elementId + "_chosen").removeClass("highlight_chosen_select");
    if ($("#" + elementId).val() == '') {
        $("#" + 'category').addClass('input-validation-error').removeClass('valid');
        if (elementId == "ddlTicketTypes") {
            alert("Please Select a Category");
        }
        return false;

    }
    else {
        $('#' + 'category').addClass('valid').removeClass('input-validation-error');
        return true;
    }


}

function validateTextbox(elementId) {
    debugger;
    if ($('#' + elementId).length > 0) {
        var value = $('#' + elementId).val().trim();
        if ((value == null || value == "" || parseFloat(value) == 0.0)) {
            $('#' + elementId).addClass('input-validation-error').removeClass('valid');
            $('#' + elementId).blur();
            if (elementId == "txtDescription") {

                alert("Description is required");

            }
            return false;
        }
        var re = /^[a-zA-Z0-9 ,._%#@()+-=!?]+$/;
        if (re.test(value)) {
        }
        else {
            $('#' + elementId).blur();
            alert("Invalid characters");
            return false;
        }
        const emojiPattern = /[\uD83C-\uDBFF\uDC00-\uDFFF\u2600-\u26FF\u2700-\u27BF]/;
        if (emojiPattern.test(value)) {
            alert("Sorry, Emojies are Restricted.");
            $('#' + elementId).blur();
            drp = document.getElementById("myDropdown");
            drp.style.display = "none"; 
            return false;
        } 
        if ((value == null || value == "" || parseFloat(value) == 0.0)) {
            $('#' + elementId).addClass('input-validation-error').removeClass('valid');
            $('#' + elementId).blur();
            if (elementId == "txtDescription") {
                
                alert("Description is required");
               
            }           
            return false;
        }
        else {
            $('#' + elementId).addClass('valid').removeClass('input-validation-error');
            drp = document.getElementById("myDropdown");
            drp.style.display = "none"; 
            return true;
        }
       
    }
    return true;
}
var isSave = false;
function SaveHPSMTicket(RestrictedTicketAttachments, TicketAttachmentMaxSize) {
    var status = true;
    //isSave = true;
    if (!validateDropdown('ddlTicketTypes')) { return status = false; }
    if (!validateTextbox('txtDescription')) { return status = false; }
    if (!validateTextbox('txtCustomerName')) { return status = false; }
    if (!validateTextbox('txtContactNo')) { return status = false; }
    if (!validateTextbox('txtTicketReference')) { return status = false; }
    if (!status) { e.preventDefault(); }

    var fileUpload = $('#FileHPSMTicket').get(0);
    var files = fileUpload.files;
    var data = new FormData();
    if (files.length) {
       // if (!isValidUploadFile(RestrictedTicketAttachments, TicketAttachmentMaxSize)) {
       //     return false;
       // }
        for (var i = 0; i < fileList.length; i++) {
            data.append(fileList[i].name, fileList[i]);
        }
    }

    data.append("txtCustomerName", $('#txtCustomerName').val());
    data.append("txtContactNo", $('#txtContactNo').val());
    data.append("txtTicketReference", $('#txtTicketReference').val());
    data.append("ddlTicketTypes", $('#ddlTicketTypes').val());
    data.append("txtDescription", $('#txtDescription').val());
    data.append("FileHPSMTicket", $('#FileHPSMTicket').val());
    data.append("TicketType", $('#TicketType').val());
    data.append("user_id", $('#user_id').val());
    $('#dvProgress').show();
    ajaxReqforFileUpload('TicketManager/SaveHPSMTicket', data, true, function (Data) {
         

        if (Data.Status == "OK") {
            showMessageBox(Data.Message, 'HPSM Ticket', "success");
            $('#closeChildPopup').trigger("click");
            // $('#TicketType').val('');
            $('#lbltxt').text('Select Category');
            $('#fileName').text('');
            $('#FileHPSMTicket').val('');
            $('#ddlTicketTypes').val('');
            $('#txtDescription').val('');
            $('#liTicketForm').removeClass("active show");
            fileList = [];


        }
        else {
            alert(Data.Message);
        }
    }, true);

}

function isValidUploadFile(RestrictedTicketAttachments, TicketAttachmentMaxSize) {
    debugger;
    $('#fileName').val(null);
    var formData = new FormData();
    var RestrictedTicketAttachments = RestrictedTicketAttachments;
    var arrRestrictedTicketAttachments = RestrictedTicketAttachments.split(",");
    var acceptedFileSize = TicketAttachmentMaxSize;
    var postedFiles = document.getElementById('FileHPSMTicket');
    var Files = postedFiles.files;
    for (var i = 0; i < Files.length; i++) {
        var ext = Files[i].name.split(".").pop().toLowerCase();
        if (arrRestrictedTicketAttachments.some(ex => ex.trim() == ext)) {
            
        }
        else
        {
            $("#FileHPSMTicket").val(null);
            alert("Only " + RestrictedTicketAttachments.replaceAll(",", ", ") + " are allowed as attachment");
            return false;
        }

        const fsize = Files.item(i).size;
        const fileMB = fsize / (1024 * 1024);
        if (fileMB > acceptedFileSize) {
            $("#FileHPSMTicket").val(null);
            alert("Please select a file less than " + acceptedFileSize + "MB");
            return false;
        }
    }
   
    for (var i = 0; i < Files.length; i++) {
        if (isSave==false)
        {
            $('#fileName').append(Files[i].name + " ");
            fileList.push(Files[i]);
        }    
        
    }
    return true
    isSave = false;
}
