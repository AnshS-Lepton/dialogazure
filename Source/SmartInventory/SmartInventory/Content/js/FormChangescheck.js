var divInitData = {};

function setDivInitData() {
    divInitData = {};
    $('#navTabs li a').each(function (i, item) {
        var divname = $(this).attr('href').replace('#', '');
        divInitData[divname] = $('#' + divname + ' :input').serialize();
    });
}

function bindCurrentDivData(loadingDivName) {
    setTimeout(function () {
        divInitData[loadingDivName] = $('#' + loadingDivName + ' :input').serialize();
    }, 300);
};

function validateDivDataforChange() {
    let validAllDiv = true;
    for (var key in divInitData) {
     
        var disabled = $('#' + key).find(':input[readonly]').prop('readonly', false);
        if ($('#' + key + ' :input').serialize() != divInitData[key]) {
            validAllDiv = false;
            disabled.prop('readonly', true);
            break;
        }
        disabled.prop('readonly', true);
    }

    if (validAllDiv)
        $('#btnUpdateItemTemplate').hide();
    else
        $('#btnUpdateItemTemplate').show();
}
