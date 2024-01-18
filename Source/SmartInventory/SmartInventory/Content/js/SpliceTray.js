var spliceTray = function () {
    var app = this;
    this.trayLstInfoD = {};
    this.DE = {
        "AddSpliceTrayLstRow": "#btnTrayRowAdd",
        "SpliceTrayInfoTable": "#tblSpliceTrayLstInfo",
        "AllowedPorts": "#divAllowdPort",
        "Ports": "#no_of_port",
        "Tablebody": "#tbodySpliceTray",
        "TrayInfo": "#TrayInfo",
        "TrayCount": "#lstSpliceTrayInfo_TrayCount",
        "NoTray": "#divNoTray",
        "NoOfPorts": ".noofport"
    }
    this.initApp = function () {
        //this.bindEvents();
    }
  
    this.getTrayInfo = function (_entityId, _entityType) {
        $(app.DE.AllowedPorts).html(MultilingualKey.SI_OSP_GBL_GBL_FRM_228+":" + $(app.DE.Ports).val());
        if ($(app.DE.TrayInfo).html().trim() == '') {
            ajaxReq('Library/GetSpliceTrayInfo', {
                entityId: _entityId, entityType: _entityType
            }, true, function (resp) {
                $(app.DE.TrayInfo).html(resp);
                //bindCurrentDivData('TrayInfo');
                if (_entityId > 0) {
                    //popup.disablebuttonWhenEditDisabled(editPermission);
                }

                $(app.DE.TrayInfo).css('background-image', 'none');
            }, false, false);
        }
    }

    $(document).on('click', app.DE.AddSpliceTrayLstRow, function (e) {
         
        var addRowIntrayInfoTbl = $(app.DE.SpliceTrayInfoTable + ' tbody');
        var rowCount = $(app.DE.SpliceTrayInfoTable + ' tbody tr').length;
        if (rowCount > parseInt($('#hdnTrayRowCount').val())) {
            $(app.DE.SpliceTrayInfoTable).show();
        }
        if (rowCount < parseInt($('#hdnTrayRowCount').val())) {
            var trayInfoVal = rowCount + 1;
            var count = 0;
            $.each($(app.DE.SpliceTrayInfoTable + " tr").get().reverse(), function (i, item) {
                count++;
                var trayName = $($(item).children()[0]).children().val();
                if (trayName != undefined && trayName.split('_').length > 1 && trayName.split('_')[1] != '' && !isNaN(trayName.split('_')[1])) {
                    trayInfoVal = parseInt(trayName.split('_')[1]) + count;
                }
            });


            $(app.DE.TrayCount).val(trayInfoVal);

            var trayInfoRowData = '<tr><td> <input id="lstSpliceTrayInfo_' + rowCount + '__network_name" name="lstSpliceTrayInfo[' + rowCount + '].network_name"  type="text" value="tray_' + trayInfoVal + '" class="valid shaftName form-control" maxlength="30"></td>';
            trayInfoRowData += '<td> <input id="lstSpliceTrayInfo_' + rowCount + '__no_of_ports" name="lstSpliceTrayInfo[' + rowCount + '].no_of_ports" type="text" value="" class="noofport valid shaftName form-control" onkeypress="return si.allowOnlyNumber(event);" onkeyup = "return st.validateSplicetray(this);" maxlength="3"></td>';

            trayInfoRowData += '<td><input data-val="true" data-val-number="The field system_id must be a number." data-val-required="The system_id field is required." id="lstSpliceTrayInfo_' + rowCount + '__system_id" name="lstSpliceTrayInfo[' + rowCount + '].system_id" type="hidden" value="0"><input data-val="true" data-val-number="The field system_id must be a number." data-val-required="The system_id field is required." id="lstSpliceTrayInfo_' + rowCount + '__tray_number" name="lstSpliceTrayInfo[' + rowCount + '].tray_number" type="hidden" value="' + trayInfoVal + '"><span class="trayRowDelete deletX" title="Delete" >x</span></td></tr>';
            addRowIntrayInfoTbl.append(trayInfoRowData);
            var rowCount = $(app.DE.SpliceTrayInfoTable + ' tbody tr').length;
            if (rowCount > parseInt($('#hdnTrayRowCount').val())) {
                $(app.DE.SpliceTrayInfoTable).show();
                $(app.DE.NoTray).hide();

            }


        }
        else {
            alert("Only "+parseInt($('#hdnTrayRowCount').val())+" row can be added !" );
        }


        $(document).on("keyup", app.DE.SpliceTrayInfoTable + ' input[type="text"]', function (e) {
            var count = 1;
            $.each($(app.DE.SpliceTrayInfoTable + ' tbody tr'), function (i, item) {
                var child = ($(this).children('td')[0]);
                if (($(child).children().attr('id') == $(e.currentTarget).attr('id'))) {
                    var trayName = $(e.currentTarget).val();
                    //if (trayName.split('_').length > 1 && trayName.split('_')[1] != '' && typeof parseInt(trayName.split('_')[1]) === 'number') {
                    if (trayName.split('_').length > 1 && trayName.split('_')[1] != '' && !isNaN(trayName.split('_')[1])) {
                        var count1 = parseInt(trayName.split('_')[1]);
                        $.each($(e.currentTarget).parent('td').parent().nextAll(), function (j, itm) {
                            var child2 = ($(this).children('td')[0]);
                            if ($(child2).children().val().toLowerCase().indexOf('tray_') == 0) {
                                count1++;
                                if (!isNaN($(child2).children().val().toLowerCase().split('tray_')[1])) {
                                    $(child2).children().val('tray_' + count1);
                                }
                            }
                        })
                        return false;
                    }
                    else {
                        var count = 0;
                        $.each($(e.currentTarget).parent().parent().prevAll(), function (indx, item) {

                            var trayName = $(item.children[0]).children().val().toLowerCase();
                            if (!isNaN(trayName.split('tray_')[1])) {
                                indx = indx + 2;
                                count = count + parseInt(trayName.split('tray_')[1]) + indx;
                                $.each($(e.currentTarget).parent('td').parent().nextAll(), function (j, itm) {
                                    var child2 = ($(this).children('td')[0]);
                                    if ($(child2).children().val().toLowerCase().indexOf('tray_') == 0) {

                                        if (!isNaN($(child2).children().val().toLowerCase().split('tray_')[1])) {
                                            $(child2).children().val('tray_' + count);
                                        }
                                        count++;
                                    }
                                })
                                return false;
                            }
                            //count = count + 1;
                        })
                    }
                }
                else {
                    // i = i + 1;                  
                    //if ($(child).children().val().indexOf('floor_') == 0) {
                    //    $(child).children().val('floor_' + i); count++;
                    //}
                }

            })
        });
    });
    $(document).on("click", app.DE.SpliceTrayInfoTable + " .trayRowDelete", function () {
         
        app.trayLstInfoD = {};
        var row = $(this).closest("tr");
        app.trayLstInfoD.selectedRow = row;
        app.trayLstInfoD.selectedId = $('#lstSpliceTrayInfo_' + row.index() + '__system_id').val();


        var func = function () {
            if (app.trayLstInfoD.selectedId == 0) {
                app.removeTrayInfoRow();
            }
            else {
                ajaxReq('Library/DeleteSpliceTrayInfoById', { spliceTrayId: app.trayLstInfoD.selectedId }, true, function (resp) {
                    if (resp.status == "OK") {
                        app.removeTrayInfoRow();
                    }
                    else {
                        alert(resp.message);
                        return false;
                    }
                }, true, true);

            }
        };
        confirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_080, func);
    });
    this.removeTrayInfoRow = function () {

        var row = app.trayLstInfoD.selectedRow;
        var removeRowindex = row.index();
        var count1 = removeRowindex;
        var currentFloor = $(row).children('td:first').children();
        var count = 1;


        $.each($(app.DE.SpliceTrayInfoTable + ' tbody tr'), function (i, item) {
            var child = ($(this).children('td')[0]);
            if (($(child).children().attr('id') == currentFloor.attr('id'))) {
                var trayName = currentFloor.val();
                if (trayName.split('_').length > 1 && trayName.split('_')[1] != '' && !isNaN(trayName.split('_')[1])) {
                    var count1 = parseInt(trayName.split('_')[1]) - 1;
                    $.each(currentFloor.parent('td').parent().nextAll(), function (j, itm) {
                        var child2 = ($(this).children('td')[0]);
                        if ($(child2).children().val().toLowerCase().indexOf('tray_') == 0) {
                            count1++;
                            if (!isNaN($(child2).children().val().toLowerCase().split('tray_')[1])) {
                                $(child2).children().val('tray_' + count1);
                            }
                        }
                    })
                    return false;
                }
                else {
                    var count = 0;
                    $.each(currentFloor.parent().parent().prevAll(), function (indx, item) {

                        var trayName = $(item.children[0]).children().val().toLowerCase();
                        if (!isNaN(trayName.split('tray_')[1])) {
                            indx = indx + 1;
                            count = count + parseInt(trayName.split('tray_')[1]) + indx;
                            $.each(currentFloor.parent('td').parent().nextAll(), function (j, itm) {
                                var child2 = ($(this).children('td')[0]);
                                if ($(child2).children().val().toLowerCase().indexOf('tray_') == 0) {

                                    if (!isNaN($(child2).children().val().toLowerCase().split('tray_')[1])) {
                                        $(child2).children().val('tray_' + count);
                                    }
                                    count++;
                                }
                            })
                            return false;
                        }
                        //count = count + 1;
                    })
                }
            }
            else {
                // i = i + 1;                  
                //if ($(child).children().val().indexOf('floor_') == 0) {
                //    $(child).children().val('floor_' + i); count++;
                //}
            }

        });


        row.remove();
        var trayTblbody = $(app.DE.SpliceTrayInfoTable + ' tbody tr');
        var rowCount = trayTblbody.length;
        var trayInfoVal = rowCount;
        $(app.DE.TrayCount).val(trayInfoVal);     



        trayTblbody.each(function (i, val) {
            if (removeRowindex <= i) {
                $(this).find("input,select").each(function (n, nval) {

                    if ($(this).attr('name') != undefined)
                        $(this).attr('name', $(this).attr('name').replace(/\[\d+\]/, "[" + (i) + "]"));

                    if ($(this).attr('id') != undefined)
                        $(this).attr('id', $(this).attr('id').replace(/\_\d+\_/, "_" + (i) + "_"));

                });
            }
        });
    }
    this.validateSplicetray = function (obj) {
         

        var totalUsedUnits = 0;
        $(app.DE.Tablebody).find('tr').each(function () {
            var eachtr = $(this);
            var currentVal = parseInt(eachtr.find(app.DE.NoOfPorts).val());
            if (isNaN(currentVal)) {
                currentVal = 0;
            }
            if (obj.id != eachtr.find(app.DE.NoOfPorts).attr('id')) {
                totalUsedUnits = (totalUsedUnits + currentVal);
            }
        })
        var structureTotalUnits = parseInt($(app.DE.Ports).val());
        if ($(obj).val() > (structureTotalUnits - totalUsedUnits)) {
            //Maximum ports are allowed in this structure!
            alert($.validator.format("Maximum {0} ports are allowed in this tray!", structureTotalUnits), 'warning');
            $(obj).val('');
            return false;
        }
    }

    this.ValidateTrayInfo = function () {
        if ($(app.DE.Tablebody).length > 0) {
            var totalUsedUnits = 0;
            $(app.DE.Tablebody).find('tr').each(function () {

                var currentVal = $(this).find('td:eq(1)').find(app.DE.NoOfPorts).val()
                if (isNaN(currentVal)) {
                    currentVal = 0;
                }
                totalUsedUnits = (totalUsedUnits + parseInt(currentVal));
            })
            if (totalUsedUnits > parseInt($(app.DE.Ports).val())) {
                alert($.validator.format("Maximum {0} ports are allowed in this tray!", $(app.DE.Ports).val()), 'warning');
                return false;
            }      
        }
        return true;

    };
}