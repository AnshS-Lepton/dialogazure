
function pageValid(e) {

    var obj = $("#txtUsername").val();
    if (obj == '') {
        alert(MultilingualKey.CheckUsermsg);
        obj.focus();
        window.event ? event.returnValue = false : e.preventDefault();
        return;
    }
    obj = $('#txtPassword').val();
    if (obj == '') {
        alert(MultilingualKey.CheckPasswordmsg);
        obj.focus();
        window.event ? event.returnValue = false : e.preventDefault();
        return;
    }
    $(".field-validation-error").text('');
}

function mousedwnevt() {
    element = document.getElementById('dvBtn');
    element.style.backgroundPosition = "-68px 0px";
}

function mouseupevt() {
    element = document.getElementById('dvBtn');
    element.style.backgroundPosition = "0px 0px";
}