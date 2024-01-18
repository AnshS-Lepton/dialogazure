var Notification = function () {
    var app = this;
    app.hubs = null;
    app.initApp = function () {
        hubs = $.connection.SmartInventoryHub;
        $.connection.hub.logging = true;
        $.connection.hub.start().done(function () {
            hubs.server.broadCastInfo($.connection.hub.id);
            hubs.server.userName = $.connection.hub.id;
            console.log('Connected to Smart Inventory Hub...');
        });
        hubs.client.messageReceiver = function (resp) {
            if (resp.notificationType == 'Utilization') {
                app.Utilization.broadCastNotification(resp);
            }
            else if (resp.notificationType == 'Upload') {
                app.Upload.broadCastNotification(resp);
            }

            else if (resp.notificationType == 'PrintMap') {
                app.PrintMap.broadCastNotification(resp);
            }
        }
        $.connection.hub.disconnected(function () {
            setTimeout(function () {
                $.connection.hub.start();
            }, 5000); // Restart connection after 5 seconds.
        });
    },
        app.Utilization = {
            broadCastNotification: function (_notification) { $('#spnUtilizationCount').text(_notification.info); }
        }
    app.Upload = {
        broadCastNotification: function (_notification) {

            if (dataUploader != undefined && dataUploader.currentStep != undefined) {
                $("#" + dataUploader.currentStep + " .status").text(_notification.info);
            }
        }
    }
    app.PrintMap = {
        broadCastNotification: function (_notification) {           
            if (!isNaN(parseFloat(_notification.info))) {              
                if (parseFloat(_notification.info) >= 98.9) {                   
                    setTimeout(function () {$("#frmPrintLog").submit(); }, 2000)
                }
            }
            $("#frmPrintLog").submit();
        }
    }
}