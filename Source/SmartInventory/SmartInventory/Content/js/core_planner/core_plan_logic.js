$(document).ready(function () {
    $("input.numbers-only").on("paste", function () {
        var thisvalue = this;
        setTimeout(function () {
            var OnlyNumber = new RegExp(/^[0-9]{1,10}$/);

            if (!OnlyNumber.test(thisvalue.value)) {
                thisvalue.value = "";
            }
        }, 0);
    });

    $("#txtODF1, #txtODF2, #txtRequiredCore, #txtfiberlink").on("input", function () {
        if ($(this).val() != '') {
            $(this).removeClass("error-border");
        }
    });

    $('.chosen-select').chosen({ placeholder_text_multiple: '-All-', width: '100%' });
    //----------------------------------------------ODF1-------------------------------------------------------------------
    $('#txtODF1').on('keyup', function (e) {
        
        $('#txtODF1').autocomplete({

            source: function (request, response) {
                
                // var ntxt = request
                var IsSrchValid = false;
                //IsSrchValid = request.term.length >= 3 && (ntxt.length == 0 || ntxt.length == 1) ? true : ntxt.length > 1 && ntxt[1].length >= 3 ? true : false;
                IsSrchValid = request.term.length >= 3 ? true : false;
                if (IsSrchValid) {
                    var res = ajaxReq('Library/GetSearchResult', { searchText: request.term, search_type: 'ODF' }, true, function (data) {
                        if (data.length == 0) {
                            var result = [
                                {
                                    label: 'No match Found'
                                }
                            ];
                            response(result);
                            $("#txtODF1").removeClass('ui-autocomplete-loading');
                        }
                        else {
                            response($.map(data, function (item) {
                                return {
                                    label: item.network_id
                                }
                            }))
                        }
                    }, true, false);
                }
                else { $("#txtODF1").removeClass('ui-autocomplete-loading'); }
            },
            minLength: 3,
            select: function (event, ui) {
                
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                    $("#txtODF1").removeClass('ui-autocomplete-loading');
                }
                else {
                    event.preventDefault();

                    if (ui.item.networ_id != null) {
                        $("#txtODF1").val(ui.item.network_id);

                        $("#txtODF1").removeClass('ui-autocomplete-loading');
                    }
                    else {
                        $("#txtODF1").val(ui.item.label);
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    });
    $('#txtODF1').bind('paste', function (e) {
        $('#txtODF1').autocomplete({

            source: function (request, response) {
                
                // var ntxt = request
                var IsSrchValid = false;
                //IsSrchValid = request.term.length >= 3 && (ntxt.length == 0 || ntxt.length == 1) ? true : ntxt.length > 1 && ntxt[1].length >= 3 ? true : false;
                IsSrchValid = request.term.length >= 3 ? true : false;
                if (IsSrchValid) {
                    var res = ajaxReq('Library/GetSearchResult', { searchText: request.term, search_type: 'ODF' }, true, function (data) {
                        if (data.length == 0) {
                            var result = [
                                {
                                    label: 'No match Found'
                                }
                            ];
                            response(result);
                            $("#txtODF1").removeClass('ui-autocomplete-loading');
                        }
                        else {
                            response($.map(data, function (item) {
                                return {
                                    label: item.network_id
                                }
                            }))
                        }
                    }, true, false);
                }
                else { $("#txtODF1").removeClass('ui-autocomplete-loading'); }
            },
            minLength: 3,
            select: function (event, ui) {
                
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                    $("#txtODF1").removeClass('ui-autocomplete-loading');
                }
                else {
                    event.preventDefault();

                    if (ui.item.networ_id != null) {
                        $("#txtODF1").val(ui.item.network_id);

                        $("#txtODF1").removeClass('ui-autocomplete-loading');
                    }
                    else {
                        $("#txtODF1").val(ui.item.label);
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    });
    //----------------------------------------------ODF2-------------------------------------------------------------------
    $('#txtODF2').on('keyup', function (e) {
        
        $('#txtODF2').autocomplete({

            source: function (request, response) {
                
                // var ntxt = request
                var IsSrchValid = false;
                //IsSrchValid = request.term.length >= 3 && (ntxt.length == 0 || ntxt.length == 1) ? true : ntxt.length > 1 && ntxt[1].length >= 3 ? true : false;
                IsSrchValid = request.term.length >= 3 ? true : false;
                if (IsSrchValid) {
                    var res = ajaxReq('Library/GetSearchResult', { searchText: request.term, search_type: 'ODF' }, true, function (data) {
                        if (data.length == 0) {
                            var result = [
                                {
                                    label: 'No match Found'
                                }
                            ];
                            response(result);
                            $("#txtODF2").removeClass('ui-autocomplete-loading');
                        }
                        else {
                            response($.map(data, function (item) {
                                return {
                                    label: item.network_id
                                }
                            }))
                        }
                    }, true, false);
                }
                else { $("#txtODF2").removeClass('ui-autocomplete-loading'); }
            },
            minLength: 3,
            select: function (event, ui) {
                
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                    $("#txtODF2").removeClass('ui-autocomplete-loading');
                }
                else {
                    event.preventDefault();

                    if (ui.item.networ_id != null) {
                        $("#txtODF2").val(ui.item.network_id);

                        $("#txtODF2").removeClass('ui-autocomplete-loading');
                    }
                    else {
                        $("#txtODF2").val(ui.item.label);
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    });
    $('#txtODF2').bind('paste', function (e) {
        
        $('#txtODF2').autocomplete({

            source: function (request, response) {
                
                // var ntxt = request
                var IsSrchValid = false;
                //IsSrchValid = request.term.length >= 3 && (ntxt.length == 0 || ntxt.length == 1) ? true : ntxt.length > 1 && ntxt[1].length >= 3 ? true : false;
                IsSrchValid = request.term.length >= 3 ? true : false;
                if (IsSrchValid) {
                    var res = ajaxReq('Library/GetSearchResult', { searchText: request.term, search_type: 'ODF' }, true, function (data) {
                        if (data.length == 0) {
                            var result = [
                                {
                                    label: 'No match Found'
                                }
                            ];
                            response(result);
                            $("#txtODF2").removeClass('ui-autocomplete-loading');
                        }
                        else {
                            response($.map(data, function (item) {
                                return {
                                    label: item.network_id
                                }
                            }))
                        }
                    }, true, false);
                }
                else { $("#txtODF2").removeClass('ui-autocomplete-loading'); }
            },
            minLength: 3,
            select: function (event, ui) {
                
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                    $("#txtODF2").removeClass('ui-autocomplete-loading');
                }
                else {
                    event.preventDefault();

                    if (ui.item.networ_id != null) {
                        $("#txtODF2").val(ui.item.network_id);

                        $("#txtODF2").removeClass('ui-autocomplete-loading');
                    }
                    else {
                        $("#txtODF2").val(ui.item.label);
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    });
    // ----------------------------------------------FiberLink---------------------------------------------------------------------------
    $('#txtfiberlink').on('keyup', function (e) {
        
        $('#txtfiberlink').autocomplete({

            source: function (request, response) {
                
                // var ntxt = request
                var IsSrchValid = false;
                //IsSrchValid = request.term.length >= 3 && (ntxt.length == 0 || ntxt.length == 1) ? true : ntxt.length > 1 && ntxt[1].length >= 3 ? true : false;
                IsSrchValid = request.term.length >= 3 ? true : false;
                if (IsSrchValid) {
                    var res = ajaxReq('Library/GetSearchResult', { searchText: request.term, search_type: 'fiberlink' }, true, function (data) {
                        if (data.length == 0) {
                            var result = [
                                {
                                    label: 'No match Found'
                                }
                            ];
                            response(result);
                            $("#txtfiberlink").removeClass('ui-autocomplete-loading');
                        }
                        else {
                            response($.map(data, function (item) {
                                return {
                                    label: item.Link_Id,
                                    value: item.network_id,
                                }
                            }))
                        }
                    }, true, false);
                }
                else { $("#txtfiberlink").removeClass('ui-autocomplete-loading'); }
            },
            minLength: 3,
            select: function (event, ui) {
                
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                    $("#txtfiberlink").removeClass('ui-autocomplete-loading');
                }
                else {
                    event.preventDefault();

                    if (ui.item.networ_id != null) {
                        $("#txtfiberlink").val(ui.item.label);
                        $("#hdnFiberLinkNetworkId").val(ui.item.value);

                        $("#txtfiberlink").removeClass('ui-autocomplete-loading');
                    }
                    else {
                        $("#txtfiberlink").val(ui.item.label);
                        $("#hdnFiberLinkNetworkId").val(ui.item.value);
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    });

    $('#txtfiberlink').bind('paste', function (e) {
        
        $('#txtfiberlink').autocomplete({

            source: function (request, response) {
                
                // var ntxt = request
                var IsSrchValid = false;
                //IsSrchValid = request.term.length >= 3 && (ntxt.length == 0 || ntxt.length == 1) ? true : ntxt.length > 1 && ntxt[1].length >= 3 ? true : false;
                IsSrchValid = request.term.length >= 3 ? true : false;
                if (IsSrchValid) {
                    var res = ajaxReq('Library/GetSearchResult', { searchText: request.term, search_type: 'fiberlink' }, true, function (data) {
                        if (data.length == 0) {
                            var result = [
                                {
                                    label: 'No match Found'
                                }
                            ];
                            response(result);
                            $("#txtfiberlink").removeClass('ui-autocomplete-loading');
                        }
                        else {
                            response($.map(data, function (item) {
                                return {
                                    label: item.Link_Id,
                                    value: item.network_id,
                                }
                            }))
                        }
                    }, true, false);
                }
                else { $("#txtfiberlink").removeClass('ui-autocomplete-loading'); }
            },
            minLength: 3,
            select: function (event, ui) {
                
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                    $("#txtfiberlink").removeClass('ui-autocomplete-loading');
                }
                else {
                    event.preventDefault();

                    if (ui.item.networ_id != null) {
                        $("#txtfiberlink").val(ui.item.label);
                        $("#hdnFiberLinkNetworkId").val(ui.item.value);

                        $("#txtfiberlink").removeClass('ui-autocomplete-loading');
                    }
                    else {
                        $("#txtfiberlink").val(ui.item.label);
                        $("#hdnFiberLinkNetworkId").val(ui.item.value);
                    }
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });
    });
});

function validate() {
    
    if ($("#txtfiberlink").val() == "") {
        $("#txtfiberlink").addClass("error-border");
        alert('The field Fiber Link is required.'); return false;
    }

  /* if ($("#txtODF1").val() == $("#txtODF2").val()) {
        $("#txtODF1").addClass("error-border");
        $("#txtODF2").addClass("error-border");
        alert('Both ODF/Splice Closure can not be same');
        return false;
    }
    */
    var number = Number($("#txtRequiredCore").val());

    if (isNaN(number)) {
        $("#btnSubmit").prop("disabled", false);
        $("#txtRequiredCore").addClass("error-border");
        alert("Invalid input: " + "Please enter a valid number.");
        return false;
    }/* else if (number < 1 || number > 48) {
        $("#btnSubmit").prop("disabled", false);
        $("#txtRequiredCore").addClass("error-border");
        alert("Invalid input: " + "The required core must be between 1 and 48.");
        return false;
    }*/
    else {
        si.saveCorePlanLogic();
    }
}

$("#btncreatelinkCPL").on("click", function () {
    $('#txtfiberlink').val('');
    $('#hdnCheckforCLP').val('fined');
    si.createFiberlinkCPL();
});

$('#txtRequiredCore').on('keypress paste', function (event) {
    debugger;
    if (event.type === 'keypress') {
        if (event.which >= 48 && event.which <= 57) {
            var value = $(this).val() + String.fromCharCode(event.which);
            if (/^0+$/.test(value) || /^0\d+/.test(value)) {
                event.preventDefault();
            }
        } else {
            event.preventDefault();
        }
    } else if (event.type === 'paste') {
        let clipboardData = event.originalEvent.clipboardData || window.clipboardData;
        let pastedData = clipboardData.getData('Text');
        if (/^0+$/.test(pastedData) || /^0\d+/.test(pastedData)) {
            event.preventDefault();
        }
    }
});


function core_planner_info_tool(obj) {
    si.mapReport.clearSelection();
    var infoId = $('#' + obj.id);
    infoId.toggleClass('activeToolBar');

    si.ClearMapAddressTool();
    $('.infoSwitch').removeClass('activeToolBar');
    si.map.setOptions({ draggableCursor: 'crosshair' });
    google.maps.event.addListener(si.map, 'click', function (LatLong) {
        
        if (si.lineBufferObj != null) {
            si.lineBufferObj.setMap(null);
        }
        si.clearInfoRelatedObjects();
        si.GetNearByEntitiesByLatLong(LatLong.latLng, obj.id);
    });
    $(popup.DE.MinimizeModel).trigger("click");
}