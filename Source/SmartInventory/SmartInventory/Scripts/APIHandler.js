
//var applicationURL = '@System.Configuration.ConfigurationManager.AppSettings["SmartInventoryServiceURL"]';
//Check Session on every page load if session is not set then redirect to Login page
function checkSession() {
    if (sessionStorage.getItem('accessToken') == null) {
        window.location.href = window.location.origin + applicationURL + "/login";
    }
}


//Login and get token key using username and password
function GetTokenKey_Login(username, password) {
     
    var repValue = '';
    $.ajax({
        url: baseAPIUrl + '/token',
        method: 'POST',
        async: false,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            username: btoa(username),
            password: btoa(password),
            grant_type: 'password'
        },
        success: function (response) {
            if (response.status == "OK") {
                sessionStorage.setItem('accessToken', response.access_token);
                sessionStorage.setItem('token_type', response.token_type);
                sessionStorage.setItem('refresh_token', response.refresh_token);


                $.ajax({
                    url: applicationURL + "/Login/MaintainSession",
                    method: 'POST',
                    async: false,
                    contentType: 'application/json',
                    data: JSON.stringify({ token: response.access_token }),
                    success: function (data) {
                        if (data == 1)
                            repValue = response.status;
                    }
                });
            }
        },
        error: function (xhr) {
            message('error', 'Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
    return repValue;
}

function Login(username, password) {
    var repValue = '';
    $.ajax({
        url: baseAPIUrl + '/token',
        method: 'POST',
        async: false,
        contentType: 'application/x-www-form-urlencoded',
        data: {
            username: btoa(username),
            password: btoa(password),
            grant_type: 'password'
        },
        success: function (response) {
            sessionStorage.setItem('accessToken', response.access_token);
            sessionStorage.setItem('token_type', response.token_type);
            sessionStorage.setItem('refresh_token', response.refresh_token);
            repValue = response.status;
        },
        error: function (xhr) {
            message('error', 'Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
    return repValue;
}

//Log out and redirect to Login Page
function logout() {
    sessionStorage.removeItem('token_type');
    sessionStorage.removeItem('accessToken');
    //sessionStorage.removeItem('refresh_token');
    window.location.href = window.location.origin + applicationURL + "/login";
}


//Regresh Token
function refreshToken() {
    $.ajax({
        url: baseAPIUrl + '/token',
        method: 'POST',
        contentType: 'application/x-www-form-urlencoded',
        data: {
            refresh_token: sessionStorage.getItem('refresh_token'),
            grant_type: 'refresh_token'
        },
        success: function (response) {
            sessionStorage.setItem('accessToken', response.access_token);
            sessionStorage.setItem('token_type', response.token_type);
            //  sessionStorage.setItem('refresh_token', response.refresh_token);
        },
        error: function (xhr) {
            message('error', 'Request Status: ' + xhr.status + ' Status Text: ' + xhr.statusText + ' ' + xhr.responseText);
        }
    });
}


//Call API request
function APIRequest(url, requesttype, params, contentType, isfile, callback) {
     
    var handlerResult = '';
    var parmsdata = { "data": JSON.stringify(params) };
    params = requesttype == 'GET' ? $.param(params) : params;
    var baseAPIUrl='http://localhost:50034/';
    var ajaxsettings = {
        url: baseAPIUrl + url,
        type: requesttype,
        headers: { 'Authorization': sessionStorage.getItem('token_type') + ' ' + sessionStorage.getItem('accessToken') },
        async: callback == undefined ? false : true,
        data: parmsdata,
        'Content-Type': "application/json",
        processData: (isfile == false || isfile == undefined) ? true : false,
        dataType: "json",
        success: function (data) {
            if (callback != undefined) {
                callback(data);
            }
        },
        error: function (xhr) {
            if (xhr.status == '401') {
                message('warning', "Session Time Expired");
                window.location.href = window.location.origin + "/login";
            }
        }
    }
    if (isfile)
        ajaxsettings.contentType = contentType == undefined ? "application/json" : contentType;
    var respons = $.ajax(ajaxsettings).done(function (resp) {
        if (resp != '') {
            handlerResult = resp;
        }
    });

    return handlerResult;
}
