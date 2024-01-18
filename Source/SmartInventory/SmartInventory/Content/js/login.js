
function pageValid(e) {
    var obj = $("#txtUsername").val(); //document.getElementById('txtUserName');
    if (obj == '') {
        alert(MultilingualKey.SI_GBL_GBL_JQ_FRM_002);
        obj.focus();
        window.event ? event.returnValue = false : e.preventDefault();
        return;
    }
    obj = $('#txtPassword').val();//document.getElementById('txtPassword');
    if (obj == '') {
        alert(MultilingualKey.SI_GBL_GBL_JQ_FRM_003);
        obj.focus();
        window.event ? event.returnValue = false : e.preventDefault();
        return;
    }
   let status = validateuser();
    if (status == "FAILED") {
        var r = confirm("This user is already LogedIn in other device, Do you want to force Login?"); 
        if (r == true) {
            return true;
        }
        else {
            window.event ? event.returnValue = false : e.preventDefault();
            return;
        }
   }
}

function mousedwnevt() {
    element = document.getElementById('dvBtn');
    element.style.backgroundPosition = "-68px 0px";
}

function mouseupevt() {
    element = document.getElementById('dvBtn');
    element.style.backgroundPosition = "0px 0px";
}

function validateuser() {
    let status = '';
    jQuery.ajax({
        url: appRoot + "Login/validateuser",
        data: { "userName": jQuery('#txtUsername').val() },
        async: false,
        type: "POST",
        success: function (resp) {
            status = resp.status;

        }
    });
    return status;
}

function allowOnlyNumber(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;

    } else {
        $("#txtotp").removeClass("OTPRequired").focus();
        return true;
    }     
}

function ValidateOTP(evt) {
    var obj = $("#txtotp").val(); //document.getElementById('txtUserName');  
    if (obj == '') {      
        $("#txtotp").addClass("OTPRequired").focus();       
        window.event ? event.returnValue = false : e.preventDefault();
        return false;        
    }
}