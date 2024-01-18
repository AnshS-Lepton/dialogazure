//Path  =>   ~/Content/js/UserTimeSheet.js













var UserTimeSheet = function () {
    var comp = this;
    var editmode = '';
    this.AE = {
        "HdnUserId": "#userTimeSheetInput_user_id",
        "TxtStartDate": "#userTimeSheetInput_start_date",
        //"TxtStartTime": "#userTimeSheetInput_start_time",
        //"TxtEndDate": "#userTimeSheetInput_end_date",

        "TxtStartTime": "#start_date",
        "TxtEndDate": "#end_date",

        "TxtEndtime": "#userTimeSheetInput_end_time",
        "HdnUserTimeSheetId": "#userTimeSheetInput_user_timesheet_id",
        "WorkingDayChkBx": "input[name=chkWeekays]",
        "SaveUserTimeSheet": "#btnSaveUsertTimeSheet",
        "CancelUserTimeSheet": "#btnCancelUserTimeSheet"
    }
    this.initApp = function () {
        comp.bindEvents();
    }

    this.bindEvents = function () {

        //On Click To Working Day CheckBox
        $(comp.AE.WorkingDayChkBx).on("click", function () {

            var freqType = $(comp.AE.DDLActionType).val();
            var slvals = []
            var chkWeekLen = $('input:checkbox[name=chkWeekays]:checked').length;
            if (chkWeekLen > 1 && freqType == "Weekly") {
                alert('Only one day selection allowed!!');
                return false;
            }
            $('input:checkbox[name=chkWeekays]:checked').each(function () {

                slvals.push($(this).val())
            });
            $('#working_days').val(slvals);

        });
        //Function Call On Click To Submit Button
        $(comp.AE.SaveUserTimeSheet).on("click", function () {
            alert('HI');
            //  
            var slvals = []
            $('input:checkbox[name=chkWeekays]:checked').each(function () {
                slvals.push($(this).val())
            });

            var patrollerId = $("#userTimeSheetInput_user_id").val();
            //var routeID = $("#userTimeSheetInput_route_ref").val();
            //var chkFreqDay = $("#ddlFrequencyDay").val();
            //var selFreqType = $(comp.AE.DDLActionType).val();
            //if (editmode == "FutureMode")
            //  $("#userTimeSheetInput_assignTypeIn").val(selFreqType);

            var selFreqWorkingDays = $("#working_days").val();
            if (selFreqWorkingDays == "") {
                $("#working_days").val(slvals);
                selFreqWorkingDays = $("#working_days").val();
            }
            var selEndDate = $(comp.AE.TxtEndDate).val();
            var selStartDate = $(comp.AE.TxtStartDate).val();
            var selEndTime = $(comp.AE.TxtEndtime).val();
            var selStartTime = $(comp.AE.TxtStartTime).val();
            var ddlStartHH = $("#dvStartTime").find('select.ddlHoursTime').val();
            var ddlStartMM = $("#dvStartTime").find('select.ddlMinuteTime').val();
            var ddlStartAM = $("#dvStartTime").find('select.ddlSecondTime').val();
            var ddlEndHH = $("#dvEndTime").find('select.ddlHoursTime').val();
            var ddlEndMM = $("#dvEndTime").find('select.ddlMinuteTime').val();
            var ddlEndAM = $("#dvEndTime").find('select.ddlSecondTime').val();

            if (patrollerId == "") {
                alert('Please select FE User!!');
                return false;
            }


            if (ddlStartHH == '' || ddlStartMM == '' || ddlStartAM == '') {
                alert('Please select start time!!');
                return false;
            }
            else if (ddlEndHH == '' || ddlEndMM == '' || ddlEndAM == '') {
                alert('Please select end time!!');
                return false;
            }



            // 
            selStartTime = ddlStartHH + ':' + ddlStartMM + ' ' + ddlStartAM;
            $(comp.AE.TxtStartTime).val(selStartTime);
            selEndTime = ddlEndHH + ':' + ddlEndMM + ' ' + ddlEndAM;
            $(comp.AE.TxtEndtime).val(selEndTime);

            var user_timesheet_id = $("#userTimeSheetInput_user_timesheet_id").val();



            var retValueStartTime = sp.ValidateUserTimeSheetDetail(patrollerId, selStartDate, selStartTime, user_timesheet_id);
            var retValueEndTime = sp.ValidateUserTimeSheetDetail(patrollerId, selEndDate, selEndTime, user_timesheet_id);

            if (retValueStartTime == true || retValueEndTime == true) {
                alert("Roster Plan for this date already exist for this user.!!");
                return false;
            }
            selEndTime = comp.funcCurrentTime(sp.DateFormatter(selEndDate + ' ' + selEndTime));
            selStartTime = comp.funcCurrentTime(sp.DateFormatter(selStartDate + ' ' + selStartTime));

            //var chkValidTime = comp.fncChkDateTimeReq(selStartDate, selEndDate, selStartTime, selEndTime, selFreqType);
            //if ((chkValidTime != undefined) && (!chkValidTime))
            //    return false;
            //var diff_in_min = comp.GettimeDifference(selStartTime, selEndTime);
            //if (!diff_in_min) {
            //    alert("End time should be greater than start time!!");
            //    return false;
            //}

        });

        $(comp.AE.CancelUserTimeSheet).on("click", function () {
            $("#frmUserTimeSheet").submit();
            $(popup.DE.CloseChildPopup).trigger("click");

            //if ($('#frmUserTimesheet').length > 0) { sp.SubmitForm("frmUserTimesheet"); }




        });
    }

    //this.ValidateUserTimeSheetDetail = function (user_id, roster_date, roster_time, user_timesheet_id, status, message) {        
    //    app.ajaxReq('Main/ValidateUserTimeSheet', { user_id: user_id, roster_date: roster_date, roster_time: roster_time, user_timesheet_id: user_timesheet_id }, false, function (resp) {
    //         
    //        if (resp != null && resp != undefined) {
    //             
    //            if (resp != undefined) {
    //                status = resp.status;
    //                message = resp.message;
    //            }
    //        }
    //    }, true);
    //    return true;
    //}

    this.GettimeDifference = function (startTime, endTime) {
        var hhStarttime = parseInt(startTime.split(':')[0]);
        var mmStarttime = parseInt(startTime.split(':')[1]);
        var hhEndtime = parseInt(endTime.split(':')[0]);
        var mmEndtime = parseInt(endTime.split(':')[1]);

        hhStarttime = hhStarttime * 60;
        hhStarttime = hhStarttime + mmStarttime;
        hhEndtime = hhEndtime * 60;
        hhEndtime = hhEndtime + mmEndtime;

        if (hhStarttime > hhEndtime)
            return false;
        else
            return true;

        //if(hhStarttime>=hhEndtime)
        //{
        //    if (hhStarttime == hhEndtime)
        //    {
        //        if (mmStarttime > mmEndtime)
        //            return false;
        //        else
        //            return true;
        //    }
        //    else
        //    return false;
        //}
        //else
        //{
        //    return true;
        //}
    }
    //Get Current Date
    this.fncCurrentDate = function (objDate) {
        var dtToday = new Date();
        if ((objDate != undefined) && (objDate != ''))
            dtToday = objDate;

        //var month = dtToday.getMonth();
        var day = dtToday.getDate();
        if (day < 10)
            day = '0' + day.toString();

        //var maxDate = day + '-' + monthName[month] + '-' + year;
        var maxDate = day + '-' + dtToday.toLocaleString("en-us", { month: "short" }).toString() + '-' + dtToday.getFullYear().toString();
        return maxDate;
    }
    //Get Current Time
    this.funcCurrentTime = function (objTime) {
        var dt = new Date();

        if ((objTime != undefined) && (objTime != ''))
            dt = objTime;

        var time = dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
        return time;
    }
    //Call on Load of Page
    this.fncOnPostChange = function () {
        // 
        var hdnUserTimeSheetId = $("#userTimeSheetInput_user_timesheet_id").val();
        var hdnUserId = $("#userTimeSheetInput_user_id").val();
        var hdnManagerId = $("#userTimeSheetInput_manager_id").val();
        var hdnWorkingMode = $("#userTimeSheetInput_workingmode").val();

        var hdnConfirmMsg = $("#userTimeSheetInput_confirmmsg").val().split(':'); //alert(hdnConfirmMsg);
        //          

        if (hdnConfirmMsg[0] == "ValidationError")
            alert(hdnConfirmMsg[1] + '!!');
        else if (hdnConfirmMsg[0] == "SavedError")
            alert(hdnConfirmMsg[1] + '!!');
        else if (hdnConfirmMsg[0] == "SavedSuccess")
            alert(hdnConfirmMsg[1] + '!!');

         
        if (hdnWorkingMode == "EditMode") {


            // $("#userTimeSheetInput_user_id").attr("disabled", true);
            // comp.fncBlockDateCalen("userTimeSheetInput_start_date", "userTimeSheetInput_end_date", 1);
            // $(comp.AE.DDLActionType).attr("disabled", true);
            //$("#routeAssignedInput_route_ref").val($("#routeAssignedInput_routeRefId").val());
            $("#userTimeSheetInput_user_id").attr("disabled", true);//disabled patroller drop-down
            //$("#routeAssignedInput_route_ref").attr("disabled", true);
            //$(comp.AE.DDLActionType).attr("disabled", true);
            //var actionType = $(comp.AE.DDLActionType).val();
            //$("#dvIsActive").show();//display is-active checkbox on
            //var freqDays = $("#routeAssignedInput_day_of_month").val();
            var workingDays = $("#working_days").val();

            if (workingDays != 0) {
                workingDays = workingDays.split(',');
                $.each(workingDays, function (index, value) {
                    var selChkDay = value - 2;
                    $('input[name=chkWeekays]:eq(' + selChkDay + ')').attr("checked", true);
                });
            }
            comp.fncBlockDateCalen("userTimeSheetInput_start_date", "userTimeSheetInput_end_date", 1);


        }
        else if (hdnWorkingMode == "CreationMode") {
            ////             
            if (hdnConfirmMsg[0] == "ValidationError" || hdnConfirmMsg[0] == "SavedError") {
                /*
                var rtEndTime = $("#userTimeSheetInput_endtime").val().split(':');
                var rtStartTime = $("#userTimeSheetInput_start_time").val().split(':');
                $("#dvStartTime").find('select.ddlHoursTime').val(rtStartTime[0]);
                $("#dvStartTime").find('select.ddlMinuteTime').val(rtStartTime[1].split(' ')[0]);
                $("#dvStartTime").find('select.ddlSecondTime').val(rtStartTime[1].split(' ')[1]);
                $("#dvEndTime").find('select.ddlHoursTime').val(rtEndTime[0]);
                $("#dvEndTime").find('select.ddlMinuteTime').val(rtEndTime[1].split(' ')[0]);
                $("#dvEndTime").find('select.ddlSecondTime').val(rtEndTime[1].split(' ')[1]);
                // var actionType = $(comp.AE.DDLActionType).val();
                */
            }
            // comp.fncBlockDateCalen("userTimeSheetInput_start_date", "userTimeSheetInput_end_date", 1);
            $("#userTimeSheetInput_is_active").prop("checked", true);
        }



        /*
         if (hdnWorkingMode == "EditMode" && hdnUserTimeSheetId != 0 ) {//For Route Assign Table
 
             $("#userTimeSheetInput_user_id").attr("disabled", true);
             comp.fncBlockDateCalen("userTimeSheetInput_start_date", "userTimeSheetInput_enddate", 1);
             $(comp.AE.DDLActionType).attr("disabled", true);
 
         }
         else if (hdnWorkingMode == "EditMode" && hdnRouteScheduledId != 0 && hdnRouteAssignedId == 0) {// For Schedule Route table            
             $("#routeAssignedInput_route_ref").val($("#routeAssignedInput_routeRefId").val());
             $("#routeAssignedInput_user_id").attr("disabled", true);//disabled patroller drop-down
             $("#routeAssignedInput_route_ref").attr("disabled", true);
             $(comp.AE.DDLActionType).attr("disabled", true);
             var actionType = $(comp.AE.DDLActionType).val();
 
             $("#dvIsActive").show();//display is-active checkbox on
             var freqDays = $("#routeAssignedInput_day_of_month").val();
             var workingDays = $("#routeAssignedInput_working_days").val();
             if (actionType == "Monthly") {
                 $("#lblFrequencyDay").show();
                 $("#ddlFrequencyDay").parent().show();
                 if (freqDays != 0)
                     $("#ddlFrequencyDay").val(freqDays).trigger('change');
             }
             else {
                 $("#fsWorkingDays").show();
                 if (workingDays != 0) {
                     workingDays = workingDays.split(',');
                     $.each(workingDays, function (index, value) {
                         var selChkDay = value - 2;
                         $('input[name=chkWeekays]:eq(' + selChkDay + ')').attr("checked", true);
                     });
                 }
             }
             comp.fncBlockDateCalen("routeAssignedInput_start_date", "routeAssignedInput_enddate", 1);
         }
         else if (hdnWorkingMode == "CreationMode" || hdnWorkingMode == "UserSpecificMode") {
             //// 
             if (hdnWorkingMode == "UserSpecificMode") {
 
                 $("#routeAssignedInput_user_id").attr("disabled", true);
             }
             if (hdnConfirmMsg[0] == "ValidationError" || hdnConfirmMsg[0] == "SavedError") {
                 var rtEndTime = $("#routeAssignedInput_endtime").val().split(':');
                 var rtStartTime = $("#routeAssignedInput_start_time").val().split(':');
                 $("#dvStartTime").find('select.ddlHoursTime').val(rtStartTime[0]);
                 $("#dvStartTime").find('select.ddlMinuteTime').val(rtStartTime[1].split(' ')[0]);
                 $("#dvStartTime").find('select.ddlSecondTime').val(rtStartTime[1].split(' ')[1]);
                 $("#dvEndTime").find('select.ddlHoursTime').val(rtEndTime[0]);
                 $("#dvEndTime").find('select.ddlMinuteTime').val(rtEndTime[1].split(' ')[0]);
                 $("#dvEndTime").find('select.ddlSecondTime').val(rtEndTime[1].split(' ')[1]);
 
                 var actionType = $(comp.AE.DDLActionType).val();
 
 
                 var freqDays = $("#routeAssignedInput_day_of_month").val();
                 var workingDays = $("#routeAssignedInput_working_days").val();
                 if (actionType == "Monthly") {
                     $("#dvIsActive").show();//display is-active checkbox on
                     $("#lblFrequencyDay").show();
                     $("#ddlFrequencyDay").parent().show();
                     if (freqDays != 0)
                         $("#ddlFrequencyDay").val(freqDays).trigger('change');
                 }
                 else if (actionType != "OneTime") {
                     $("#dvIsActive").show();//display is-active checkbox on
                     $("#fsWorkingDays").show();
                     if (workingDays != 0) {
                         workingDays = workingDays.split(',');
                         $.each(workingDays, function (index, value) {
                             var selChkDay = value - 2;
                             $('input[name=chkWeekays]:eq(' + selChkDay + ')').attr("checked", true);
                         });
                     }
                 }
             }
 
             $("#userTimeSheetInput_is_active").prop("checked", true);
         }
         */

    }

    this.fncBlockDateCalen = function (objStartDate, objEndDate, chkType) {
        //var maxDate = comp.fncCurrentDate();
        // // 
        // var newStartDate = sp.DateFormatter($("#" + objStartDate).val());
        //var newEndDate = sp.DateFormatter($("#" + objEndDate).val());
        //   
        var newStartDateTime = sp.DateFormatter($(comp.AE.TxtStartDate).val() + ' ' + $(comp.AE.TxtStartTime).val());
        var newEndDateTime = sp.DateFormatter($(comp.AE.TxtEndDate).val() + ' ' + $(comp.AE.TxtEndtime).val());
        //current date
        var currDateTime = new Date();
        //difference in minutes       
        var chkStartCurrDiff = (newStartDateTime - currDateTime) / (1000 * 60);
        //difference in minutes
        var chkEndCurrDiff = (newEndDateTime - currDateTime) / (1000 * 60);

        var gtStartTime = ($("#userTimeSheetInput_start_time").val()).split(':');
        var gtEndTime = ($("#userTimeSheetInput_end_time").val()).split(':');
        $("#dvStartTime").find('select.ddlHoursTime').val(gtStartTime[0]);
        $("#dvStartTime").find('select.ddlMinuteTime').val(gtStartTime[1]);
        //17:00
        $("#dvStartTime").find('select.ddlMinuteTime').val(gtStartTime[1].split(' ')[0]);

        var is12hoursFormat = gtStartTime[1].split(' ').length == 1 ? false : true;

        if (gtStartTime[0] > 12) {
            var hhStartTime = gtStartTime[0] - 12;
            if (hhStartTime < 10) {

                hhStartTime = '0' + hhStartTime;
            }
            $("#dvStartTime").find('select.ddlHoursTime').val(hhStartTime);
        }
        if (is12hoursFormat) {
            $("#dvStartTime").find('select.ddlSecondTime').val(gtStartTime[1].split(' ')[1]);
        }
        else {
            if (gtStartTime[0] >= 12)
                $("#dvStartTime").find('select.ddlSecondTime').val('PM');
            else
                $("#dvStartTime").find('select.ddlSecondTime').val('AM');
        }



        $("#dvEndTime").find('select.ddlHoursTime').val(gtEndTime[0]);
        $("#dvEndTime").find('select.ddlMinuteTime').val(gtEndTime[1]);
        $("#dvEndTime").find('select.ddlMinuteTime').val(gtEndTime[1].split(' ')[0]);

        is12hoursFormat = gtEndTime[1].split(' ').length == 1 ? false : true;

        if (gtEndTime[0] > 12) {
            var hhEndTime = gtEndTime[0] - 12;
            if (hhEndTime < 10) {

                hhEndTime = '0' + hhEndTime;
            }
            $("#dvEndTime").find('select.ddlHoursTime').val(hhEndTime);
        }
        if (is12hoursFormat) {
            $("#dvEndTime").find('select.ddlSecondTime').val(gtEndTime[1].split(' ')[1]);
        }
        else {
            if (gtEndTime[0] >= 12)
                $("#dvEndTime").find('select.ddlSecondTime').val('PM');
            else
                $("#dvEndTime").find('select.ddlSecondTime').val('AM');
        }

        if (chkStartCurrDiff > 0 && chkEndCurrDiff > 0) {

            $("#userTimeSheetInput_user_id").attr("disabled", false);//disabled patroller drop-down
            $("#userTimeSheetInput_route_ref").attr("disabled", false);
            $(comp.AE.DDLActionType).attr("disabled", true);
            editmode = "FutureMode";
            // comp.ChangeValueRouteDDL();
        }
        else if (!(chkStartCurrDiff > 0) && chkEndCurrDiff > 0) {
            /*
           //$("#userTimeSheetInput_route_ref").attr("disabled", true);
          //  comp.ChangeValueRouteDDL();
            $("#userTimeSheetInput_start_date").attr("readonly", true);
            $("#imgStartDate").hide();
            $("#dvStartTime select").attr("disabled", true);
            $("#dvEndTime select").attr("disabled", true);
            $("#ddlFrequencyDay").attr('disabled', true);
            var freqType = $(comp.AE.DDLActionType).val();

            // in case of OneTime, if start time passed then every thing should be disabled...
            if (freqType == "OneTime") {

                $("#dvBtnAction").hide();
                $("#userTimeSheetInput_endd_ate").attr("disabled", true)
                $("#imgEndDate").hide();
                editmode = "PastMode";
            }
            else {
                //in case of scheduled end date should be enabled...         
                $("#userTimeSheetInput_endtime").attr('readonly', false);
                $("#userTimeSheetInput_end_date").attr("disabled", false);
                $("#userTimeSheetInput_is_active").attr('readonly', true);
                $(comp.AE.WorkingDayChkBx).attr("readonly", true);
                $(comp.AE.WorkingDayChkBx).attr('disabled', true);
                $("#dvBtnAction").show();
                $("#imgEndDate").show();
                editmode = "CurrentMode";
            }
            */
        }
        else if (!(chkStartCurrDiff > 0) && !(chkEndCurrDiff > 0)) {

            $("#dvBtnAction").hide();
            //$("#routeAssignedInput_route_ref").attr("disabled", true);
            // This 
            //comp.ChangeValueRouteDDL();

            $("#userTimeSheetInput_startdate, #userTimeSheetInput_end_date").attr("disabled", true)
            //$("#routeAssignedInput_start_time, #routeAssignedInput_endtime").attr("disabled", true)
            $("#imgStartDate, #imgEndDate").hide();
            $("#userTimeSheetInput_is_active").attr('disabled', true)
            $(comp.AE.WorkingDayChkBx).attr("disabled", true);
            $("#dvStartTime select").attr("disabled", true);
            $("#dvEndTime select").attr("disabled", true);
            $("#ddlFrequencyDay").attr('disabled', true);
            editmode = "PastMode";
        }
        if (chkType != 0) {

            //Used in future
            //$('#routeAssignedInput_startdate, #routeAssignedInput_enddate').attr('min', maxDate);

        }
        else {
            //Used in future
            //$('#routeAssignedInput_startdate, #routeAssignedInput_enddate').attr('min', maxDate);
            //$('#routeAssignedInput_startdate, #routeAssignedInput_enddate').val(maxDate);
        }
    }

    //Call Force Route Drop-Down Value Change in Case of Ref_ID of Route change 
    //this.ChangeValueRouteDDL = function () {
    //    var selectedRouteId = $("#routeAssignedInput_routeRefId").val().split('|||')[0];
    //    if (selectedRouteId != '') {
    //        $("#routeAssignedInput_route_ref option[value^='" + selectedRouteId + "']").prop("selected", true);
    //        $("#routeAssignedInput_route_ref").trigger('change');
    //    }
    //}

    //On Change of Patroller DropDownList Bind Value To Hidden Field
    this.fncSetUserId = function (e)
    {
        $("#userTimeSheetInput_user_id").val($(e).val());
        var UserId = $(e).val();
        ajaxReq('Workforce/GetServiceDetailByUserId', { userid: UserId }, true, function (resp) {
            $('#lbltransection').empty();
            $('#lblservice').empty();
            $('#lbljocategory').empty();
            $('#lblUsertype').empty();

            if (resp != null && resp != undefined) {
                $('#lbltransection').html(resp.transection);
                $('#lblservice').html(resp.service);
                $('#lbljocategory').html(resp.jo_category_name);
                $('#lblUsertype').html(resp.user_type);

            }
        }, true);


    }
    //On Change of Route DropDownList Bind Value To Hidden Field
    //this.fncSetRouteRefId = function (e) {

    //    $("#routeAssignedInput_routeRefId").val($(e).val());
    //}
    //On Change of Frequency Type DropdownList
    //this.fncRouteFreqChange = function (selectFreq) {
    //    // // 
    //    var selVal = $(selectFreq).val();
    //    //$("#routeAssignedInput_startdate, #routeAssignedInput_start_time").attr('readonly', false);
    //    // $("#routeAssignedInput_startdate, #routeAssignedInput_enddate").val("");
    //    if (selVal == "OneTime") {

    //        $("#dvIsActive, #fsWorkingDays").hide();
    //        $("#fsRouteFreq label:eq(1)").hide();
    //        $("#fsRouteFreq div:eq(3)").hide();
    //    }
    //    else if (selVal == "Monthly") {

    //        $("#fsWorkingDays").hide();
    //        $("#fsRouteFreq label:eq(1)").show();
    //        $("#fsRouteFreq div:eq(3)").show();
    //        $("#dvIsActive").show();
    //        $("#routeAssignedInput_is_active").prop("checked", true);
    //    }
    //    else if (selVal == "Daily" || selVal == "Weekly") {

    //        $("#dvIsActive, #fsWorkingDays").show();
    //        $("#fsRouteFreq label:eq(1)").hide();
    //        $("#fsRouteFreq div:eq(3)").hide();
    //        if (selVal == "Daily")
    //            $(comp.AE.WorkingDayChkBx).prop("checked", true);
    //        else
    //            $(comp.AE.WorkingDayChkBx).prop("checked", false);
    //    }
    //    else if (selVal == "") {//When Not Select Any Type
    //        //$("#routeAssignedInput_startdate, #routeAssignedInput_enddate").attr('readonly', true);
    //        $("#fsRouteFreq label:eq(1)").hide();
    //        $("#fsRouteFreq div:eq(3)").hide();
    //        $("#dvIsActive, #fsWorkingDays").hide();
    //    }
    //}
    //Set Value On Day of Month ModelField
    this.fncDayOfMonth = function (e) {

        $('#userTimeSheetInput_day_of_month').val($(e).val());
    }
    //On change startDate
    this.fncChangeStartDate = function (e) {
        alert('Hi');
         
        // $(comp.AE.TxtStartTime).attr('readonly', false);
        $(comp.AE.TxtEndDate).attr('readonly', false);
        //   $(comp.AE.TxtEndtime).attr('readonly', false);
        var fixdDate = $(comp.AE.TxtStartDate).val();

        /*var selFreqType = $(comp.AE.DDLActionType).val();
        var setDiffDate = 0;
        if (selFreqType == "OneTime") {
            $(comp.AE.TxtEndDate).attr('readonly', true);
        }
        else if (selFreqType == "Monthly") {
            setDiffDate = 31;
        }
        else if (selFreqType == "Daily") {
            setDiffDate = 1;
        }
        else if (selFreqType == "Weekly") {
            setDiffDate = 7;
        }
        */
        var setDiffDate = 0;

        setDiffDate = 7;

        // $('#routeAssignedInput_enddate').attr('min', fixdDate);
        var date1 = $(comp.AE.TxtStartDate).val();
        var date = new Date(Date.parse(date1));
        date.setDate(date.getDate() + setDiffDate);

        var newDate = date.toDateString();
        newDate = new Date(Date.parse(newDate));
        var maxDate = comp.fncCurrentDate(newDate);
        //Used in future
        //$('#routeAssignedInput_enddate').attr('min', maxDate);
        //if (selFreqType == "OneTime") {
        //    $(comp.AE.TxtEndDate).val(maxDate);
        //}

    }
    //On change EndDate
    this.fncChangeEndDate = function (e) {

        $("#end_date").attr('readonly', false);
    }


    //Check Date-Time Validate While Submit The Route
    this.fncChkDateTimeReq = function (objStartDate, objEndDate, objStartTime, objEndTime, objFreqType) {
        objStartDate = sp.DateFormatter(objStartDate);
        var newStartDate = objStartDate;

        objEndDate = sp.DateFormatter(objEndDate);
        var newEndDate = objEndDate;

        // var currDate = sp.DateFormatter(comp.fncCurrentDate());
        var currDate1 = new Date().setHours(0, 0, 0, 0);
        var currDate = new Date(currDate1);
        //var currDate1 = sp.DateFormatter(currDate.toString());
        //currDate = currDate;

        var currTime = comp.funcCurrentTime();

        /*
        var hdnRouteAssignedId = $("#routeAssignedInput_route_assigned_id").val();
        var hdnRouteScheduledId = $("#routeAssignedInput_scheduled_id").val();
        //var objStartDay = objStartTime.split(' ')[1];
        
        if (objFreqType == "OneTime") {
            if (newStartDate >= currDate) {
                if (objStartDate.toDateString() != objEndDate.toDateString()) {

                    alert("Start date should be equal to end date!!");
                    return false;
                }
                else if (!comp.GettimeDifference(currTime, objStartTime) && !(newStartDate > currDate)) {
                    alert("Start time should be greater than current time!!");
                    return false;
                }
            }
            else {

                if (hdnRouteAssignedId != 0 || hdnRouteScheduledId != 0) {
                    var chkDiffHD = newEndDate - currDate;
                    chkDiffHD = chkDiffHD / (1000 * 60 * 60 * 24);
                    if (chkDiffHD > 0) {
                    }
                    else {
                        alert("End date should be equal to current date!!");
                        return false;
                    }
                }
                else {
                    alert("Start Date should be greater or equal to current date!!");
                    return false;
                }
            }
        }
        else {
            //For Schedule Part
            var daysDiff = -1;
            if (newEndDate != undefined && newStartDate != undefined) {
                daysDiff = newEndDate - newStartDate;
                daysDiff = daysDiff / (1000 * 60 * 60 * 24);
            }

            if (newStartDate > currDate) {

                //validate days diff for scheduled route..
                var isValid = comp.ValidateScheduledDateDiff(daysDiff, objFreqType);
                if (!isValid) return false;

            }
            else {
                if ((hdnRouteAssignedId != 0 || hdnRouteScheduledId != 0) && editmode != "FutureMode") {
                    var chkDiffHD = newEndDate - currDate;
                    chkDiffHD = chkDiffHD / (1000 * 60 * 60 * 24);

                    if (chkDiffHD < 0) {
                    
                        alert("End date should be greater than or equal to current date!!");
                        return false;
                    }

                    //validate days diff for scheduled route for edit mode..
                    var isValid = comp.ValidateScheduledDateDiff(daysDiff, objFreqType);
                    if (!isValid) return false;
                }
                else {
                    alert("Start Date should be greater than current date!!");
                    return false;
                }
            }
        }
        */
    }

    this.ValidateScheduledDateDiff = function (daysDiff, FreqType) {
        if (FreqType == "Monthly") {
            //if (daysDiff < 31) {
            //    alert("End Date should be greater than 31 day to start date!!");
            //    return false;
            //}

        } else if (FreqType == "Daily") {
            //if (daysDiff < 1) {
            //    alert("End Date should be greater than 1 day to start date!!");
            //    return false;
            //}

        } else if (FreqType == "Weekly") {
            if (daysDiff < 7) {
                alert("End Date should be greater than 7 day to start date!!");
                return false;
            }
        }
        return true;
    }

    //Response After Save Data
    this.handleSuccess = function (data) {
         
        //var hdnWorkingMode = $("#userTimeSheetInput_workingmode").val();
        //var hdnConfirmMsg = $("#userTimeSheetInput_confirmmsg").val().split(':'); //alert(hdnConfirmMsg);
        var hdnConfirmMsg = data.split(':'); //alert(hdnConfirmMsg);        
        //          

        //if (hdnConfirmMsg[0] == "ValidationError")
        //    alert(hdnConfirmMsg[1] + '!!');


        ////if (data == "Success_User_Time_Sheet") {
        ////    alert("Saved successfully!!");
        ////    if (hdnWorkingMode == "EditMode") {

        ////        $(popup.DE.CloseChildPopup).trigger("click");
        ////        //refresh view route grid..
        ////        if ($('#frmUserTimesheet').length > 0) { sp.SubmitForm("frmUserTimesheet"); }
        ////        return false;
        ////    }
        ////}
        ////else if (data == "Failed_ScheduledRoute") {

        ////}
    }
    //Response After Error Get During Save Data
    this.handleError = function (data) {


    }
}

